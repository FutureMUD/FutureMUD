param(
	[switch]$Check
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot 'DatabaseSeeder/DatabaseSeeder.csproj'
$command = if ($Check) { '--check-industrialised-catalogue' } else { '--export-industrialised-catalogue' }

# Sources are the compiled stock, specialised graphs and approved planning records.
# Refresh only derived audits using the same catalogue as packaged ItemSeeder; there is no prose-generation path.
& dotnet run --project $project --configuration Debug --no-launch-profile --no-restore -m:1 -p:BuildInParallel=false -p:UseSharedCompilation=false -p:NuGetAudit=false -- $command
if ($LASTEXITCODE -ne 0) {
	throw "Industrialised catalogue audit failed with exit code $LASTEXITCODE."
}
