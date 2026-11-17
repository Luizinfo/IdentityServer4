$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    New-Item -ItemType Directory -Force -Path ./nuget | Out-Null
    dotnet tool restore
    if ($LASTEXITCODE -ne 0) { throw 'Tool restore failed.' }

    foreach ($component in @('Storage', 'IdentityServer4', 'EntityFramework.Storage', 'EntityFramework', 'AspNetIdentity')) {
        Push-Location (Join-Path $PSScriptRoot "src/$component")
        try { & ./build.ps1 @args }
        finally { Pop-Location }
    }
}
finally { Pop-Location }
