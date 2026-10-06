[CmdletBinding()]
param([string]$EvidenceRoot)

$ErrorActionPreference = 'Stop'
$workspaceRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$wrapper = Join-Path $workspaceRoot 'Temporary Scratch App\GatheringNativePersistenceHarness\Run-IsolatedAcceptance.ps1'
$harnessDll = Join-Path $workspaceRoot 'Temporary Scratch App\GatheringNativePersistenceHarness\bin\Debug\net10.0\GatheringNativePersistenceHarness.dll'
if (-not (Test-Path -LiteralPath $harnessDll)) { throw 'Build the current Debug native harness before running fixture isolation.' }
$evidenceParent = if ($EvidenceRoot) { [IO.Path]::GetFullPath($EvidenceRoot) } else { Join-Path $workspaceRoot '.artifacts\ammo-fixture-isolation' }
$evidenceDirectory = Join-Path $evidenceParent ([DateTime]::UtcNow.ToString('yyyyMMddTHHmmssZ') + '_' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $evidenceDirectory | Out-Null
$previousSelection = [Environment]::GetEnvironmentVariable('FUTUREMUD_AMMO_ACCEPTANCE_CASE')
$assemblyBefore = (Get-FileHash -LiteralPath $harnessDll -Algorithm SHA256).Hash
$runs = @()
try {
	foreach ($scenario in @(
		@{ name = 'after-splits'; cases = 'split-throw,split-valid,load-provider-refusal'; inheritedCapacity = 2 },
		@{ name = 'alone'; cases = 'load-provider-refusal'; inheritedCapacity = 8 }
	)) {
		$env:FUTUREMUD_AMMO_ACCEPTANCE_CASE = $scenario.cases
		$log = Join-Path $evidenceDirectory ($scenario.name + '.log')
		# The existing wrapper creates, guards and deletes a new owned database/server for each run.
		& pwsh -NoProfile -File $wrapper -AmmoConservationOnly *> $log
		$runExit = $LASTEXITCODE
		$output = [IO.File]::ReadAllText($log)
		if ($runExit -ne 0 -or $output -notmatch '(?m)^nativeHarnessExit=0\s*$' -or
			$output -notmatch 'temporaryMySqlCleanup=gracefully-shut-down-and-deleted-owned-instance' -or
			$output -match 'cleanupFailure=') { throw "Fixture isolation failed in $($scenario.name); see $log" }
		foreach ($selectedCase in $scenario.cases.Split(',')) {
			if ($output -notmatch ('(?m)^ARMAmmo=' + [regex]::Escape($selectedCase) + ' passed ')) {
				throw "Missing exact native completion for $selectedCase; see $log"
			}
		}
		$expectedCapacity = "ARMAmmo-fixture=load-provider-refusal before:$($scenario.inheritedCapacity) capacity:8"
		if (-not $output.Contains($expectedCapacity)) { throw "Missing actual shared-prototype capacity boundary: $expectedCapacity" }
		if ($scenario.name -eq 'after-splits' -and
			($output -notmatch '(?m)^ARMAmmo-fixture=split-throw before:8 capacity:2\s*$' -or
			 $output -notmatch '(?m)^ARMAmmo-fixture=split-valid before:2 capacity:2\s*$')) {
			throw 'The sequenced regression must exercise both native split cases at capacity two.'
		}
		if ((Get-FileHash -LiteralPath $harnessDll -Algorithm SHA256).Hash -ne $assemblyBefore) { throw 'Harness changed during fixture isolation.' }
		$runs += @{ name = $scenario.name; cases = $scenario.cases; exit = $runExit; capacityBoundary = $expectedCapacity;
			log = $log; logSha256 = (Get-FileHash -LiteralPath $log -Algorithm SHA256).Hash }
	}
	@{ status = 'PASS'; scope = 'Actual native load-provider-refusal alone and after both split cases; unchanged capacity, conservation, custody and cold-reader assertions.';
		harnessSha256 = $assemblyBefore; runs = $runs } | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $evidenceDirectory 'receipt.json')
	Write-Output "ammoFixtureIsolation=PASS evidence=$evidenceDirectory harness=$assemblyBefore"
} finally {
	[Environment]::SetEnvironmentVariable('FUTUREMUD_AMMO_ACCEPTANCE_CASE', $previousSelection)
}
