$ErrorActionPreference = "Stop";
dotnet run --project build -- $args
if ($LASTEXITCODE -ne 0) { throw "EntityFramework.Storage build failed with exit code $LASTEXITCODE." }
