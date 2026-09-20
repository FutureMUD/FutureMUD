param(
	[ValidateSet('Human', 'Compact', 'Json')][string]$OutputMode = 'Human',
	[string]$ResultsRoot,
	[string]$Configuration = 'Debug',
	[string]$Filter,
	[int]$TimeoutSeconds,
	[switch]$FailOnSkipped,
	[switch]$Help
)
$ErrorActionPreference = 'Stop'
$forward = @{} + $PSBoundParameters
$forward.Suite = 'core'
& (Join-Path $PSScriptRoot 'invoke-test-reporting.ps1') @forward
exit $LASTEXITCODE
