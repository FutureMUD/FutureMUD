[CmdletBinding()]
param(
	[switch]$Check,
	[switch]$NoBuild
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repositoryRoot 'MudSharpCore\MudSharpCore.csproj'
$arguments = @(
	'run',
	'--project', $projectPath,
	'--configuration', 'Debug',
	'--no-restore'
)

if ($NoBuild) {
	$arguments += '--no-build'
}

$arguments += '--'
$arguments += '--audit-industrialised-prerequisites'
$arguments += $repositoryRoot
if ($Check) {
	$arguments += '--check'
}

& dotnet @arguments
if ($LASTEXITCODE -ne 0) {
	exit $LASTEXITCODE
}
