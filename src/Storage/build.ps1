$ErrorActionPreference = "Stop";
dotnet run --project build -- $args
if ($LASTEXITCODE -ne 0) { throw "Storage build failed with exit code $LASTEXITCODE." }
