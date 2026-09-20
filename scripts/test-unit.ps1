param(
	[ValidateSet('Human', 'Compact', 'Json')][string]$OutputMode = 'Human',
	[string]$ResultsRoot,
	[string]$Configuration = 'Debug',
	[string]$Filter,
	[int]$TimeoutSeconds,
	[switch]$FailOnSkipped,
	[string[]]$Project,
	[switch]$Help
)
$ErrorActionPreference = 'Stop'
$forward = @{} + $PSBoundParameters
$forward.Suite = 'fast'
& (Join-Path $PSScriptRoot 'invoke-test-reporting.ps1') @forward
exit $LASTEXITCODE
