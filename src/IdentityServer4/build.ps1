$ErrorActionPreference = "Stop";
dotnet run --project build -- $args
if ($LASTEXITCODE -ne 0) { throw "IdentityServer4 build failed with exit code $LASTEXITCODE." }
