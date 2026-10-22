[CmdletBinding()]
param(
	[string]$ResultsRoot = (Join-Path $PSScriptRoot '../.artifacts/test-runs'),
	[string]$OutputPath = $env:GITHUB_STEP_SUMMARY
)

$ErrorActionPreference = 'Stop'
$lines = [Collections.Generic.List[string]]::new()
$lines.Add('## Unit-test timing and results')
$lines.Add('')
$files = @(if (Test-Path -LiteralPath $ResultsRoot) {
	Get-ChildItem -LiteralPath $ResultsRoot -Filter summary.json -Recurse -File
})

function Format-Cell([object]$Value) {
	([string]$Value).Replace('|', '\|').Replace("`r", ' ').Replace("`n", ' ').Replace('<', '&lt;').Replace('>', '&gt;')
}

function Format-Duration([object]$Phase) {
	if (-not $Phase.started_utc -or -not $Phase.ended_utc) { return 'not completed' }
	# ConvertFrom-Json returns DateTime on newer PowerShell and strings on older hosts.
	$elapsed = [DateTimeOffset]$Phase.ended_utc - [DateTimeOffset]$Phase.started_utc
	$elapsed.TotalSeconds.ToString('F1', [Globalization.CultureInfo]::InvariantCulture) + ' s'
}

if ($files.Count -eq 0) {
	$lines.Add('No completed receipt is available. This is **not** evidence of a passing run; inspect the job logs for bootstrap failure or interruption.')
}
foreach ($file in $files) {
	$receipt = Get-Content -LiteralPath $file.FullName -Raw | ConvertFrom-Json
	$lines.Add("Run ``$(Format-Cell $receipt.run_id)``: **$(Format-Cell $receipt.status)**; source stable: $(Format-Cell $receipt.source_stable); counts complete: $(Format-Cell $receipt.counts_complete).")
	$lines.Add('')
	$lines.Add('| Project | Build + restore | Tests | Passed | Failed | Skipped | Status |')
	$lines.Add('| --- | ---: | ---: | ---: | ---: | ---: | --- |')
	foreach ($invocation in $receipt.invocations) {
		$counts = $invocation.counts
		$lines.Add("| $(Format-Cell $invocation.project) | $(Format-Duration $invocation.build) | $(Format-Duration $invocation.test) | $(if ($null -ne $counts) { $counts.passed } else { '?' }) | $(if ($null -ne $counts) { $counts.failed } else { '?' }) | $(if ($null -ne $counts) { $counts.skipped } else { '?' }) | $(Format-Cell $invocation.status) |")
	}
	$lines.Add('')
}
$lines.Add('Build timings include restore. Test hosts overlap, so their durations must not be added to estimate job wall time. Download the structured evidence artifact for individual test timings and failure details.')
if ([string]::IsNullOrWhiteSpace($OutputPath)) { $lines -join "`n" }
else { Add-Content -LiteralPath $OutputPath -Value $lines -Encoding utf8 }
