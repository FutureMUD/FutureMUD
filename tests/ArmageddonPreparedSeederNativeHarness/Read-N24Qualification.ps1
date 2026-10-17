param(
	[Parameter(Mandatory)][string]$Qualification,
	[Parameter(Mandatory)][string]$QualificationSha256,
	[Parameter(Mandatory)][string]$CurrentSources,
	[Parameter(Mandatory)][string]$BaseFixtureSha256,
	[Parameter(Mandatory)][string]$PackageMode,
	[Parameter(Mandatory)][string]$EarlierClasses,
	[Parameter(Mandatory)][string]$Destination
)
$ErrorActionPreference = 'Stop'
function Read-PinnedJson([string]$Path, [string]$Sha256) {
	if (-not [IO.Path]::IsPathFullyQualified($Path) -or $Sha256 -cnotmatch '^[A-F0-9]{64}$' -or
		-not (Test-Path -LiteralPath $Path -PathType Leaf) -or (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash -cne $Sha256) {
		throw 'N24 reuse requires an exact accessible qualified receipt digest.'
	}
	return Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
}
$qualificationData = Read-PinnedJson $Qualification $QualificationSha256
if ($qualificationData.status -ne 'QUALIFIED_N24_ONLY') { throw 'N24 reuse requires the explicitly closed four-case scope.' }
$originalRun = Split-Path $qualificationData.summary.path -Parent
if (-not [IO.Path]::GetFullPath($qualificationData.summary.path).Equals([IO.Path]::GetFullPath((Join-Path $originalRun 'summary.json')), [StringComparison]::OrdinalIgnoreCase) -or
	-not [IO.Path]::GetFullPath($qualificationData.campaign.path).Equals([IO.Path]::GetFullPath((Join-Path $originalRun 'n24-maintenance/receipt.json')), [StringComparison]::OrdinalIgnoreCase) -or
	-not [IO.Path]::GetFullPath($qualificationData.sources.path).Equals([IO.Path]::GetFullPath((Join-Path $originalRun 'source-start.json')), [StringComparison]::OrdinalIgnoreCase)) {
	throw 'N24 campaign, summary and sources must belong to the same original evidence run.'
}
$campaign = Read-PinnedJson $qualificationData.campaign.path $qualificationData.campaign.sha256
$summary = Read-PinnedJson $qualificationData.summary.path $qualificationData.summary.sha256
$priorSources = @(Read-PinnedJson $qualificationData.sources.path $qualificationData.sources.sha256)
if ($campaign.Status -ne 'PASS' -or -not $campaign.OriginalWorldUnchanged -or @($campaign.Cases).Count -ne 4 -or
	@($campaign.Cases | Where-Object Status -ne 'PASS').Count -ne 0 -or
	(($campaign.Cases.Case | Sort-Object) -join '|') -ne 'deleted|genuine-interrupted-first-install|retained-edit-clone|retired') {
	throw 'N24 reuse does not contain all four successful actual maintenance cases.'
}
if (-not $summary.source_stable -or $summary.source_start -cne $summary.source_end -or
	$summary.source_start -cne $qualificationData.sources.sha256 -or
	$summary.base_fixture_sha256 -cne $BaseFixtureSha256 -or $summary.package_mode -ne $PackageMode -or
	$summary.earlier_classes -ne $EarlierClasses) { throw 'N24 source stability, immutable base or mode differs.' }
# Pin all seeder/shared/runtime/persistence inputs, plus the exact executable maintenance case.
# UI/protocol-only harness repairs and documentation do not silently invalidate or relabel this closed scope.
$pattern = '^(global\.json|Directory\.[^/]+|NuGet\.[Cc]onfig|FutureMUD\.sln)$|^(DatabaseSeeder|FutureMUDLibrary|MudsharpDatabaseLibrary|ExpressionEngine|MudSharpCore)/|^Temporary Scratch App/GatheringNativePersistenceHarness/.*\.(cs|csproj|props|targets)$|^tests/(ArmageddonInstallerNativeHarness|ArmageddonTraditionNativeHarness|ArmageddonPreparedSeederNativeHarness)/.*\.(cs|csproj|props|targets)$'
$old = @($priorSources | Where-Object path -match $pattern | Sort-Object path)
$current = @(Get-Content -LiteralPath $CurrentSources -Raw | ConvertFrom-Json | Where-Object path -match $pattern | Sort-Object path)
if ($old.Count -lt 1000 -or @($old | Where-Object path -ceq 'tests/ArmageddonPreparedSeederNativeHarness/Program.S6Maintenance.cs').Count -ne 1 -or $old.Count -ne $current.Count) { throw 'N24 scoped input inventory changed.' }
for ($i = 0; $i -lt $old.Count; $i++) {
	$currentHash = $current[$i].sha256
	if ($current[$i].path -ceq 'tests/ArmageddonPreparedSeederNativeHarness/Program.BaseWorldFixture.cs') {
		# Only the explicit evidence-reuse guard is outside the original N24 execution.
		$callerPath = Join-Path (Split-Path $PSScriptRoot -Parent | Split-Path -Parent) $current[$i].path
		$caller = [IO.File]::ReadAllText($callerPath)
		$guard = 'if (S6Readiness && Environment.GetEnvironmentVariable("FUTUREMUD_S6_N24_REUSED") != "1") QualifyS6Maintenance(database, fixture.Sql, fixture.Receipt.Database);'
		$original = 'if (S6Readiness) QualifyS6Maintenance(database, fixture.Sql, fixture.Receipt.Database);'
		if (-not $caller.Contains($guard)) { throw 'N24 reuse caller differs from the reviewed guarded form.' }
		if ($currentHash -cne $old[$i].sha256) {
			$currentHash = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($caller.Replace($guard, $original))))
		}
	}
	if ($old[$i].path -cne $current[$i].path -or $old[$i].sha256 -cne $currentHash) {
		throw "N24 scoped source changed: $($old[$i].path)"
	}
}
$reference = [ordered]@{
	status = 'REUSED_QUALIFIED'; qualification = $Qualification; qualification_sha256 = $QualificationSha256
	original_campaign = $qualificationData.campaign; original_summary = $qualificationData.summary
	original_sources = $qualificationData.sources; matched_scoped_inputs = $old.Count; caller_normalization = 'Only the explicit N24 evidence-reuse guard; all other native harness inputs exact'
	base_fixture_sha256 = $BaseFixtureSha256; package_mode = $PackageMode; earlier_classes = $EarlierClasses
	limits = 'Four actual N24 cases retain their original world/run scope. Current seeder/shared/runtime/persistence and maintenance code inputs match exactly; no new maintenance execution is claimed.'
}
$reference | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $Destination -Encoding utf8
return [pscustomobject]$reference
