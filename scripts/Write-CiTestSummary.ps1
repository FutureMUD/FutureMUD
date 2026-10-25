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
	$resultFile = Join-Path $file.DirectoryName 'test-results.json'
	if (Test-Path -LiteralPath $resultFile) {
		$results = @(Get-Content -LiteralPath $resultFile -Raw | ConvertFrom-Json)
		$slow = @($results | Where-Object { $null -ne $_.duration_seconds -and $_.duration_seconds -gt 10 } | Sort-Object duration_seconds -Descending)
		$lines.Add("Cases over the 10-second review threshold: **$($slow.Count)**. Review fixture cost or move sustained integration/benchmark workloads behind explicit opt-in.")
		if ($slow.Count -gt 0) {
			$lines.Add('')
			$lines.Add('| Test (slowest 20) | Seconds |')
			$lines.Add('| --- | ---: |')
			foreach ($case in ($slow | Select-Object -First 20)) {
				$seconds = ([double]$case.duration_seconds).ToString('F2', [Globalization.CultureInfo]::InvariantCulture)
				$lines.Add("| $(Format-Cell ($case.fully_qualified_name + ' / ' + $case.name)) | $seconds |")
			}
		}
		if (@($results | Where-Object { $null -eq $_.duration_seconds }).Count -gt 0) {
			$lines.Add('Some case timings are unavailable; the threshold count is incomplete.')
		}
	} else { $lines.Add('Individual test timings are unavailable; the 10-second review threshold cannot be assessed.') }
	$lines.Add('')
}
$lines.Add('Build timings include restore. Test hosts overlap, so their durations must not be added to estimate job wall time. Download the structured evidence artifact for individual test timings and failure details.')
if ([string]::IsNullOrWhiteSpace($OutputPath)) { $lines -join "`n" }
else { Add-Content -LiteralPath $OutputPath -Value $lines -Encoding utf8 }
