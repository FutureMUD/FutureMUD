param(
	[switch]$Check
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot 'DatabaseSeeder/DatabaseSeeder.csproj'
$command = if ($Check) { '--check-industrialised-catalogue' } else { '--export-industrialised-catalogue' }

# Sources are authored TSVs and approved planning records. Only the derived item, clothing-graph and dependency audits may be refreshed.
# Use the same strict parser as packaged ItemSeeder; there is no prose-generation path.
& dotnet run --project $project --configuration Debug --no-launch-profile --no-restore -p:BuildInParallel=false -p:NuGetAudit=false -- $command
if ($LASTEXITCODE -ne 0) {
	throw "Industrialised catalogue audit failed with exit code $LASTEXITCODE."
}
