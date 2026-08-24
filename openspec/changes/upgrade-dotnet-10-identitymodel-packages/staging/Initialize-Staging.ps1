[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$artifactRoot = Join-Path $PSScriptRoot 'artifacts'
$identityRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..\..\..'))
$packagesFeed = Join-Path $identityRoot 'nuget-packages'
$globalPackages = Join-Path $artifactRoot 'group-5-zinfo-global-packages'
$httpCache = Join-Path $artifactRoot 'group-5-zinfo-http-cache'
$pluginsCache = Join-Path $artifactRoot 'group-5-zinfo-plugins-cache'

@($packagesFeed, $globalPackages, $httpCache, $pluginsCache) | ForEach-Object {
    New-Item -ItemType Directory -Path $_ -Force | Out-Null
}

$env:NUGET_PACKAGES = $globalPackages
$env:NUGET_HTTP_CACHE_PATH = $httpCache
$env:NUGET_PLUGINS_CACHE_PATH = $pluginsCache

Write-Output "NuGetConfig=$(Join-Path $PSScriptRoot 'NuGet.Config')"
Write-Output "PackagesFeed=$packagesFeed"
Write-Output "GlobalPackages=$globalPackages"
Write-Output "HttpCache=$httpCache"
Write-Output "PluginsCache=$pluginsCache"
