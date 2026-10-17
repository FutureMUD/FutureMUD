$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$workflow = Get-Content (Join-Path $repo '.github/workflows/unit-tests.yml') -Raw
$match = [regex]::Match($workflow, '(?ms)        run: \|\r?\n(.*?)      - uses: actions/checkout')
if (-not $match.Success) { throw 'Coverage probe not found in workflow.' }
$probe = ($match.Groups[1].Value -split '\r?\n' | ForEach-Object { $_ -replace '^          ', '' }) -join "`n"
$folder = Join-Path ([IO.Path]::GetTempPath()) ('futuremud-ci-coverage-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $folder | Out-Null
$script = Join-Path $folder 'probe.ps1'
@'
function gh {
	if ($args -contains 'repos/owner/repo/pulls') {
		$global:LASTEXITCODE = [int]$env:MOCK_PR_EXIT
		$env:MOCK_PRS
	} else {
		$global:LASTEXITCODE = [int]$env:MOCK_RUN_EXIT
		$env:MOCK_RUNS
	}
}
'@ + "`n" + $probe | Set-Content $script
$pr = '[{"number":7,"head":{"sha":"abc","repo":{"full_name":"owner/repo"}}}]'
function RunJson([string]$status, [string]$conclusion, [int]$number = 7, [string]$sha = 'abc') {
	@{ workflow_runs = @(@{ head_sha = $sha; status = $status; conclusion = $conclusion; pull_requests = @(@{ number = $number }) }) } | ConvertTo-Json -Depth 6 -Compress
}
$running = RunJson 'in_progress' ''
$cases = @(
	@{ Name = 'matching PR run'; Expected = 'true' }
	@{ Name = 'queued PR run'; Runs = (RunJson 'queued' ''); Expected = 'true' }
	@{ Name = 'successful PR run'; Runs = (RunJson 'completed' 'success'); Expected = 'true' }
	@{ Name = 'failed PR run'; Runs = (RunJson 'completed' 'failure'); Expected = 'true' }
	@{ Name = 'no open PR'; Prs = '[]' }
	@{ Name = 'PR workflow absent or conflicted'; Runs = '{"workflow_runs":[]}' }
	@{ Name = 'cancelled PR run'; Runs = (RunJson 'completed' 'cancelled') }
	@{ Name = 'another revision'; Runs = (RunJson 'completed' 'success' 7 'old') }
	@{ Name = 'another PR'; Runs = (RunJson 'completed' 'success' 8) }
	@{ Name = 'PR lookup denied'; PrExit = '1' }
	@{ Name = 'workflow lookup denied'; RunExit = '1' }
	@{ Name = 'malformed response'; Prs = 'invalid' }
	@{ Name = 'master push'; Branch = 'master' }
	@{ Name = 'tag push'; RefType = 'tag' }
	@{ Name = 'PR event'; Event = 'pull_request' }
	@{ Name = 'manual dispatch'; Event = 'workflow_dispatch' }
)
foreach ($case in $cases) {
	$output = Join-Path $folder ($case.Name + '.txt')
	$start = [Diagnostics.ProcessStartInfo]::new((Get-Process -Id $PID).Path)
	$start.UseShellExecute = $false
	$start.CreateNoWindow = $true
	$start.RedirectStandardOutput = $true
	$start.RedirectStandardError = $true
	foreach ($arg in @('-NoProfile', '-File', $script)) { $start.ArgumentList.Add($arg) }
	$values = @{
		REPOSITORY = 'owner/repo'; REPO_OWNER = 'owner'; HEAD_SHA = 'abc'; DEFAULT_BRANCH = 'master'
		HEAD_BRANCH = ($case.Branch ?? 'feature'); EVENT_NAME = ($case.Event ?? 'push'); REF_TYPE = ($case.RefType ?? 'branch')
		GITHUB_OUTPUT = $output; MOCK_PRS = ($case.Prs ?? $pr); MOCK_RUNS = ($case.Runs ?? $running)
		MOCK_PR_EXIT = ($case.PrExit ?? '0'); MOCK_RUN_EXIT = ($case.RunExit ?? '0')
	}
	foreach ($key in $values.Keys) { $start.Environment[$key] = $values[$key] }
	$process = [Diagnostics.Process]::Start($start)
	$stdout = $process.StandardOutput.ReadToEndAsync()
	$stderr = $process.StandardError.ReadToEndAsync()
	if (-not $process.WaitForExit(10000)) { $process.Kill($true); throw "Probe timed out: $($case.Name)" }
	$expected = 'skip=' + ($case.Expected ?? 'false')
	if ($process.ExitCode -ne 0 -or (Get-Content $output -Raw).Trim() -ne $expected) {
		throw "Probe failed: $($case.Name). $($stdout.Result) $($stderr.Result)"
	}
	$process.Dispose()
}
Write-Host "PASS: $($cases.Count) coverage scenarios; temporary evidence: $folder"
