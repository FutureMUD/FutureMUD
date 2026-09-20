param(
	[Parameter(Mandatory)][ValidateSet('fast', 'core', 'climate')][string]$Suite,
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
$started = [DateTime]::UtcNow
$deadlineSeconds = if ($PSBoundParameters.ContainsKey('TimeoutSeconds')) { $TimeoutSeconds } elseif ($OutputMode -eq 'Human') { $null } else { 1800 }
function Write-BootstrapFailure([string]$kind, [string]$detail, [int]$exitCode) {
	if ($OutputMode -eq 'Json') {
		@{ schema_version = 1; status = $(if ($exitCode -eq 2) { 'BLOCKED' } else { 'INCONCLUSIVE' }); issues = @(@{ kind = $kind; detail = $detail }) } | ConvertTo-Json -Depth 5 -Compress
	} else { [Console]::Error.WriteLine("Test reporting ${kind}: $detail") }
	exit $exitCode
}
if ($null -ne $deadlineSeconds -and $deadlineSeconds -le 0) { Write-BootstrapFailure 'INVALID_INVOCATION' 'TimeoutSeconds must be positive.' 2 }
$repoRoot = Split-Path -Parent $PSScriptRoot
$localDotnetRoot = Join-Path $repoRoot '.dotnet-cli\.dotnet'
$dotnetCommand = Get-Command dotnet -ErrorAction SilentlyContinue
$dotnet = if ($dotnetCommand) { $dotnetCommand.Source } else { $null }
if (-not $dotnet) {
	$dotnet = Join-Path $localDotnetRoot 'dotnet.exe'
	if (Test-Path -LiteralPath $dotnet) {
		$env:DOTNET_ROOT = $localDotnetRoot
		$env:PATH = "$localDotnetRoot;$env:PATH"
	} else {
		Write-BootstrapFailure 'MISSING_SDK' 'dotnet was not found. Run scripts/setup.ps1 to bootstrap a local SDK.' 2
	}
}
$bootstrapDotnet = if ($env:FUTUREMUD_REPORTER_BOOTSTRAP_DOTNET) { $env:FUTUREMUD_REPORTER_BOOTSTRAP_DOTNET } else { $dotnet }
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_NOLOGO = '1'
$bootstrap = Join-Path $repoRoot ('.artifacts\test-runs\bootstrap-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $bootstrap -Force | Out-Null
$output = Join-Path $bootstrap 'out'
$log = Join-Path $bootstrap 'bootstrap.log'
$intermediate = (Join-Path $bootstrap 'obj') + [IO.Path]::DirectorySeparatorChar
$bootstrapArgs = @('build', (Join-Path $PSScriptRoot 'TestReporting\TestReporting.csproj'), '-c', 'Release', '-m:1', '-p:RestoreBuildInParallel=false', '-p:NuGetAudit=false', '-o', $output, "-p:BaseIntermediateOutputPath=$intermediate", "-p:MSBuildProjectExtensionsPath=$intermediate")
$quotedArgs = ($bootstrapArgs | ForEach-Object {
	$value = $_.Replace('"', '\"')
	if ($value.EndsWith('\')) { $value += '\' }
	'"' + $value + '"'
}) -join ' '
try {
	$startInfo = New-Object System.Diagnostics.ProcessStartInfo
	$startInfo.FileName = $bootstrapDotnet
	$startInfo.Arguments = $quotedArgs
	$startInfo.UseShellExecute = $false
	$startInfo.CreateNoWindow = $true
	$startInfo.RedirectStandardOutput = $true
	$startInfo.RedirectStandardError = $true
	$process = [System.Diagnostics.Process]::Start($startInfo)
	$outStream = [System.IO.File]::Create($log)
	$errStream = [System.IO.File]::Create((Join-Path $bootstrap 'bootstrap.err.log'))
	$outCopy = $process.StandardOutput.BaseStream.CopyToAsync($outStream)
	$errCopy = $process.StandardError.BaseStream.CopyToAsync($errStream)
	$remaining = if ($null -ne $deadlineSeconds) { [Math]::Max(0, [int][Math]::Ceiling($deadlineSeconds - ([DateTime]::UtcNow - $started).TotalSeconds)) } else { $null }
	if ($null -ne $remaining -and $remaining -eq 0) { try { $process.Kill() } catch {}; Write-BootstrapFailure 'TIMEOUT' "Reporter bootstrap exceeded deadline; log: $log" 3 }
	if ($null -ne $remaining) { $completed = $process.WaitForExit([int]([Math]::Min([long]$remaining * 1000, [int]::MaxValue))) } else { $process.WaitForExit(); $completed = $true }
	$process.Refresh()
	if (-not $completed -or -not $process.HasExited) { try { $process.Kill() } catch {}; Write-BootstrapFailure 'TIMEOUT' "Reporter bootstrap exceeded deadline; log: $log" 3 }
	$process.WaitForExit()
	try { $drained = [System.Threading.Tasks.Task]::WaitAll(@($outCopy, $errCopy), 5000) } finally { $outStream.Dispose(); $errStream.Dispose() }
	if (-not $drained) { Write-BootstrapFailure 'REPORTING_ERROR' "Reporter bootstrap log capture did not complete; log: $log" 3 }
	if ($process.ExitCode -ne 0 -or -not (Test-Path -LiteralPath (Join-Path $output 'TestReporting.dll'))) { Write-BootstrapFailure 'REPORTING_ERROR' "Reporter bootstrap failed (exit $($process.ExitCode)); log: $log" 3 }
} catch {
	if ($outStream) { $outStream.Dispose() }
	if ($errStream) { $errStream.Dispose() }
	Write-BootstrapFailure 'REPORTING_ERROR' "Reporter bootstrap could not start: $($_.Exception.Message); log: $log" 3
}
$env:FUTUREMUD_REPORTER_BOOTSTRAP_LOG = $log
$arguments = @((Join-Path $output 'TestReporting.dll'), '--repo', $repoRoot, '--suite', $Suite, '--output-mode', $OutputMode.ToLowerInvariant(), '--configuration', $Configuration)
if ($Help) { $arguments = @((Join-Path $output 'TestReporting.dll'), '--help') }
else {
	if ($ResultsRoot) { $arguments += @('--results-root', $ResultsRoot) }
	if ($PSBoundParameters.ContainsKey('Filter')) { $arguments += @('--filter', $Filter) }
	if ($null -ne $deadlineSeconds) {
		$remaining = [int][Math]::Floor($deadlineSeconds - ([DateTime]::UtcNow - $started).TotalSeconds)
		if ($remaining -le 0) { Write-BootstrapFailure 'TIMEOUT' "Reporter bootstrap exhausted deadline; log: $log" 3 }
		$arguments += @('--timeout-seconds', [string]$remaining)
	}
	if ($FailOnSkipped) { $arguments += '--fail-on-skipped' }
	foreach ($item in $Project) { $arguments += @('--project', $item) }
}
& $dotnet @arguments
exit $LASTEXITCODE
