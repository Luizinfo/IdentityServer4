param(
    [switch]$SkipMigrations,
    [string]$LocalDbInstance = 'IdentityPrReadiness20261117'
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path $PSScriptRoot -Parent

function Invoke-DotNet {
    & dotnet @args
    if ($LASTEXITCODE -ne 0) { throw "dotnet $($args[0]) failed with exit code $LASTEXITCODE." }
}

Push-Location $repositoryRoot
try {
    Invoke-DotNet --version
    foreach ($quickstart in Get-ChildItem -LiteralPath samples/Quickstarts -Directory | Sort-Object Name) {
        $solution = Get-ChildItem -LiteralPath $quickstart.FullName -Filter '*.sln' | Select-Object -First 1
        if ($solution) { Invoke-DotNet build $solution.FullName -c Release --nologo }
    }
    Invoke-DotNet build samples/Clients/Clients.sln -c Release --nologo '-p:EnableWindowsTargeting=true'

    foreach ($component in @('Storage', 'IdentityServer4', 'EntityFramework.Storage', 'EntityFramework', 'AspNetIdentity')) {
        $solution = Get-ChildItem -LiteralPath "src/$component" -Filter '*.sln' | Select-Object -First 1
        $auditJson = & dotnet list $solution.FullName package --vulnerable --include-transitive --format json --no-restore
        if ($LASTEXITCODE -ne 0) { throw "Dependency audit failed for $component." }
        $audit = ($auditJson -join "`n") | ConvertFrom-Json
        foreach ($project in $audit.projects) {
            foreach ($framework in $project.frameworks) {
                foreach ($package in @($framework.topLevelPackages) + @($framework.transitivePackages)) {
                    if ($package.vulnerabilities) { throw "Vulnerable dependency in $component`: $($package.id)." }
                }
            }
        }
        Write-Output "Dependency audit passed: $component"
    }

    $sampleSolutions = @(Get-ChildItem -LiteralPath samples/Quickstarts -Filter '*.sln' -Recurse)
    $sampleSolutions += Get-Item -LiteralPath samples/Clients/Clients.sln
    foreach ($solution in $sampleSolutions) {
        $auditJson = & dotnet list $solution.FullName package --vulnerable --include-transitive --format json --no-restore
        if ($LASTEXITCODE -ne 0) { throw "Dependency audit failed for $($solution.FullName)." }
        $audit = ($auditJson -join "`n") | ConvertFrom-Json
        foreach ($project in $audit.projects) {
            foreach ($framework in $project.frameworks) {
                foreach ($package in @($framework.topLevelPackages) + @($framework.transitivePackages)) {
                    if ($package.vulnerabilities) { throw "Vulnerable sample dependency: $($package.id)." }
                }
            }
        }
        Write-Output "Sample dependency audit passed: $($solution.Directory.Name)"
    }

    if ($SkipMigrations -or -not $IsWindows) {
        Write-Output 'SQL Server migration validation skipped; run on Windows with LocalDB to complete this gate.'
        return
    }

    if ($LocalDbInstance -notmatch '^IdentityPrReadiness[A-Za-z0-9_]*$') {
        throw 'Use a dedicated LocalDB instance whose name starts with IdentityPrReadiness.'
    }
    # Some LocalDB versions return zero when a named-instance lookup fails.
    # Compare the instance inventory instead of trusting that lookup's exit code.
    $instances = @(& sqllocaldb info | ForEach-Object { $_.Trim() })
    if ($LASTEXITCODE -ne 0) { throw 'Unable to list LocalDB instances.' }
    if ($instances -notcontains $LocalDbInstance) {
        & sqllocaldb create $LocalDbInstance
        $instances = @(& sqllocaldb info | ForEach-Object { $_.Trim() })
        if ($instances -notcontains $LocalDbInstance) { throw 'The dedicated migration test instance was not created.' }
    }
    & sqllocaldb start $LocalDbInstance
    if ($LASTEXITCODE -ne 0) { throw 'Unable to start the dedicated migration test instance.' }

    Invoke-DotNet tool restore
    $runId = [Guid]::NewGuid().ToString('N')
    $migrationChecks = @(
        @{ Project = 'src/EntityFramework.Storage/migrations/SqlServer'; Context = 'ConfigurationDbContext' },
        @{ Project = 'src/EntityFramework.Storage/migrations/SqlServer'; Context = 'PersistedGrantDbContext' },
        @{ Project = 'src/AspNetIdentity/migrations/SqlServer'; Context = 'ApplicationDbContext' }
    )
    $previousConnection = $env:ConnectionStrings__db
    try {
        foreach ($check in $migrationChecks) {
            $context = $check.Context
            $migrationProject = $check.Project
            $database = "IdentityPrReadiness_$($context)_$runId"
            $env:ConnectionStrings__db = "Server=(localdb)\$LocalDbInstance;Database=$database;Integrated Security=true;TrustServerCertificate=true"
            Invoke-DotNet ef migrations has-pending-model-changes --project $migrationProject --context $context
            Invoke-DotNet ef database update --project $migrationProject --context $context --no-build
            Invoke-DotNet ef migrations has-pending-model-changes --project $migrationProject --context $context --no-build
            Invoke-DotNet ef database update --project $migrationProject --context $context --no-build
            Write-Output "Migration validation passed: $context ($database)"
        }
    }
    finally { $env:ConnectionStrings__db = $previousConnection }
}
finally { Pop-Location }
