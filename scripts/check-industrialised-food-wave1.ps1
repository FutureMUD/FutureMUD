param(
	[switch]$Refresh,
	[switch]$Check,
	[switch]$CheckReview,
	[switch]$SelfTest
)

$replacement = Join-Path $PSScriptRoot 'sync-industrialised-food-catalogue.ps1'
$requested = $false
foreach ($mode in @('Refresh', 'Check', 'CheckReview', 'SelfTest')) {
	if (Get-Variable -Name $mode -ValueOnly) {
		$requested = $true
		$forward = @{ $mode = $true }
		& $replacement @forward
		if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
	}
}
if (-not $requested) { & $replacement -Check }
exit $LASTEXITCODE

$ErrorActionPreference = 'Stop'
if ($Refresh -and $Check) { throw 'Use either -Refresh or -Check, not both.' }

$repoRoot = Split-Path $PSScriptRoot -Parent
$seedingRoot = Join-Path $repoRoot 'Design Documents/Seeding'
$catalogueRoot = Join-Path $repoRoot 'DatabaseSeeder/Seeders/FoodCatalogue'
$inventoryPath = Join-Path $seedingRoot 'Industrialised_Food_Wave1_Inventory.tsv'
$servingPath = Join-Path $seedingRoot 'Industrialised_Food_Wave1_Serving_Manifests.tsv'
$reusePath = Join-Path $seedingRoot 'Industrialised_Food_Wave1_Reuse_Review.tsv'
$reviewPath = Join-Path $seedingRoot 'Industrialised_Food_Wave1_Evidence_and_Coverage.md'
$utf8 = [System.Text.UTF8Encoding]::new($false)

function Assert-Headers {
	param([string]$Path, [string[]]$Expected)
	$header = (Get-Content -LiteralPath $Path -TotalCount 1).Split("`t")
	if (($header -join "`t") -cne ($Expected -join "`t")) {
		throw "$Path has unexpected headers. Expected: $($Expected -join ', ')"
	}
}

function Expand-Reservations {
	param([string]$Value)
	foreach ($token in $Value.Split(';').Trim()) {
		if ($token -cmatch '^(?<prefix>[a-z][a-z0-9_]*_)\{(?<first>\d{3})\.\.(?<last>\d{3})\}$') {
			$first = [int]$Matches.first
			$last = [int]$Matches.last
			if ($last -lt $first) { throw "Invalid identity range '$token'." }
			for ($index = $first; $index -le $last; $index++) { '{0}{1:d3}' -f $Matches.prefix, $index }
			continue
		}
		if ($token -cnotmatch '^[a-z][a-z0-9_]*_\d{3}$') { throw "Invalid identity reservation '$token'." }
		$token
	}
}

function Get-ReuseRows {
	$rows = [System.Collections.Generic.List[object]]::new()
	foreach ($file in Get-ChildItem -LiteralPath $catalogueRoot -Recurse -File | Where-Object Name -Match '\.food-(items|liquids)\.tsv$' | Sort-Object FullName) {
		$relative = [System.IO.Path]::GetRelativePath($repoRoot, $file.FullName).Replace('\', '/')
		$line = 1
		foreach ($row in Import-Csv -LiteralPath $file.FullName -Delimiter "`t") {
			$line++
			$kind = if ($file.Name.EndsWith('.food-liquids.tsv')) { 'Liquid' } else { $row.kind }
			$adopted = $kind -eq 'Intermediate' -or ($kind -eq 'Liquid' -and $row.family -cin @('Broth','DairyDrink','Oil','Sauce','Syrup','Vinegar'))
			$rows.Add([pscustomobject][ordered]@{
				stable_reference = $row.stable_reference
				source_file = $relative
				source_line = $line
				source_scope = $row.scope
				kind = $kind
				family = $row.family
				disposition = if ($adopted) { 'adopted-later-era-dependency' } else { 'unchanged-reuse' }
				rationale = if ($adopted) {
					'Admitted as reusable input or liquid stock for later production; exact historical admissions and existing identity remain authoritative.'
				} else {
					'Retained as a valid earlier food or drink without cloning; later availability remains subject to its admission record.'
				}
				review_status = 'gate1-reviewed'
			})
		}
	}
	$rows
}

function ConvertTo-Tsv {
	param([object[]]$Rows, [string[]]$Headers)
	$lines = [System.Collections.Generic.List[string]]::new()
	$lines.Add($Headers -join "`t")
	foreach ($row in $Rows) {
		$values = foreach ($header in $Headers) {
			$value = [string]$row.$header
			if ($value.Contains("`t") -or $value.Contains("`r") -or $value.Contains("`n")) { throw "TSV value for $header contains a forbidden control character." }
			$value
		}
		$lines.Add($values -join "`t")
	}
	($lines -join [Environment]::NewLine) + [Environment]::NewLine
}

$inventoryHeaders = @('planning_reference','family','shared_imnf_bases','industrial_i_bases','modern_mnf_bases','nuclear_nf_bases','information_f_bases','identity_reservations','coverage','production_routes','reuse_rule','evidence_packages','design_brief','review_status')
$servingHeaders = @('manifest_reference','admissions','context','service_name','ordered_entries','course_order','portion_rationale','coverage','review_status')
$reuseHeaders = @('stable_reference','source_file','source_line','source_scope','kind','family','disposition','rationale','review_status')
Assert-Headers $inventoryPath $inventoryHeaders
Assert-Headers $servingPath $servingHeaders

$issues = [System.Collections.Generic.List[string]]::new()
$inventory = @(Import-Csv -LiteralPath $inventoryPath -Delimiter "`t")
$servings = @(Import-Csv -LiteralPath $servingPath -Delimiter "`t")
if ($inventory.Count -ne 20) { $issues.Add("Expected 20 family inventory rows, found $($inventory.Count).") }

$planningKeys = @{}
$identities = @{}
$scopeCounts = [ordered]@{ shared_imnf=0; industrial_i=0; modern_mnf=0; nuclear_nf=0; information_f=0 }
foreach ($row in $inventory) {
	if ($planningKeys.ContainsKey($row.planning_reference)) { $issues.Add("Duplicate planning reference $($row.planning_reference).") }
	$planningKeys[$row.planning_reference] = $true
	if ($row.review_status -cne 'proposed') { $issues.Add("Inventory row $($row.planning_reference) is not proposed.") }
	if (-not $row.coverage -or -not $row.production_routes -or -not $row.reuse_rule -or -not $row.evidence_packages -or -not $row.design_brief) { $issues.Add("Incomplete inventory row $($row.planning_reference).") }
	$expected = 0
	foreach ($scope in @($scopeCounts.Keys)) {
		$column = "${scope}_bases"
		$value = 0
		if (-not [int]::TryParse($row.$column, [ref]$value) -or $value -lt 0) { $issues.Add("Invalid $column for $($row.planning_reference)."); continue }
		$scopeCounts[$scope] += $value
		$expected += $value
	}
	$expanded = @(Expand-Reservations $row.identity_reservations)
	if ($expanded.Count -ne $expected) { $issues.Add("Identity reservation count for $($row.planning_reference) is $($expanded.Count), expected $expected.") }
	foreach ($identity in $expanded) {
		if ($identities.ContainsKey($identity)) { $issues.Add("Duplicate reserved identity $identity.") }
		$identities[$identity] = $row
	}
}

$expectedScopes = [ordered]@{ shared_imnf=262; industrial_i=45; modern_mnf=81; nuclear_nf=30; information_f=46 }
foreach ($scope in $expectedScopes.Keys) {
	if ($scopeCounts[$scope] -ne $expectedScopes[$scope]) { $issues.Add("Unexpected $scope count $($scopeCounts[$scope]); expected $($expectedScopes[$scope]).") }
}

$manifestKeys = @{}
foreach ($row in $servings) {
	if ($manifestKeys.ContainsKey($row.manifest_reference)) { $issues.Add("Duplicate serving manifest $($row.manifest_reference).") }
	$manifestKeys[$row.manifest_reference] = $true
	if ($row.review_status -cne 'proposed') { $issues.Add("Serving manifest $($row.manifest_reference) is not proposed.") }
	$eras = @($row.admissions.Split(';').Trim())
	if (-not $eras.Count -or @($eras | Where-Object { $_ -cnotin @('industrial','modern','nuclear','information') }).Count) { $issues.Add("Invalid admissions for $($row.manifest_reference).") }
	$entries = @($row.ordered_entries.Split(';').Trim())
	$courses = @($row.course_order.Split(';').Trim())
	if ($entries.Count -ne $courses.Count) { $issues.Add("Entry/course count mismatch for $($row.manifest_reference).") }
	foreach ($entry in $entries) {
		if ($entry -cnotmatch '^(item|liquid):(?<key>[a-z][a-z0-9_]*):(?<amount>[1-9]\d*)(g|ml)$') { $issues.Add("Invalid serving entry '$entry'."); continue }
		$key = $Matches.key
		$amount = [int]$Matches.amount
		$entryType = $entry.Split(':')[0]
		if (-not $identities.ContainsKey($key)) { $issues.Add("Unknown serving identity $key."); continue }
		if ($amount -gt 5000) { $issues.Add("Implausible unchecked serving amount in '$entry'.") }
		$identityEras = if ($key.StartsWith('industrialised_')) { @('industrial','modern','nuclear','information') }
		elseif ($key.StartsWith('industrial_')) { @('industrial') }
		elseif ($key.StartsWith('modern_')) { @('modern','nuclear','information') }
		elseif ($key.StartsWith('nuclear_')) { @('nuclear','information') }
		else { @('information') }
		foreach ($era in $eras) {
			if ($era -cnotin $identityEras) { $issues.Add("Admission $era cannot use $key in $($row.manifest_reference).") }
		}
		$family = $identities[$key].family
		$liquidFamilies = @('non-alcoholic-drinks','stimulant-infusions','alcoholic-drinks')
		$expectedType = if ($family -cin $liquidFamilies) { 'liquid' } else { 'item' }
		if ($entryType -cne $expectedType) { $issues.Add("Serving entry $key must use type $expectedType, not $entryType.") }
	}
	if (-not $row.context -or -not $row.portion_rationale -or -not $row.coverage) { $issues.Add("Incomplete serving manifest $($row.manifest_reference).") }
}

$reuseRows = @(Get-ReuseRows)
if ($reuseRows.Count -ne 3000) { $issues.Add("Expected 3,000 reuse rows, found $($reuseRows.Count).") }
if (@($reuseRows | Where-Object kind -eq 'Liquid').Count -ne 225) { $issues.Add('Expected 225 audited liquids.') }
if (@($reuseRows | Where-Object kind -ne 'Liquid').Count -ne 2775) { $issues.Add('Expected 2,775 audited food items.') }
$reuseDuplicates = @($reuseRows | Group-Object stable_reference | Where-Object Count -ne 1)
if ($reuseDuplicates.Count) { $issues.Add("Duplicate live reuse references: $($reuseDuplicates.Name -join ', ').") }
$validDispositions = @('unchanged-reuse','adopted-later-era-dependency','superseded-by-explicit-era-version','unsuitable-duplication','deferred-dependency')
if (@($reuseRows | Where-Object disposition -cnotin $validDispositions).Count) { $issues.Add('Invalid reuse disposition.') }

$reuseContent = ConvertTo-Tsv $reuseRows $reuseHeaders
if ($Refresh) {
	[System.IO.File]::WriteAllText($reusePath, $reuseContent, $utf8)
}
if ($Check) {
	Assert-Headers $reusePath $reuseHeaders
	if (-not [string]::Equals([System.IO.File]::ReadAllText($reusePath), $reuseContent, [System.StringComparison]::Ordinal)) { $issues.Add('Reuse review is stale; run with -Refresh and review the diff.') }
}

$linkTargets = @(
	'Design Documents/README.md',
	'Design Documents/Seeding/FutureMUD_Industrialised_Item_Seeder_Programme.md',
	'Design Documents/Seeding/FutureMUD_Industrialised_Shared_Baseline_Design_Reference.md',
	'Design Documents/Seeding/FutureMUD_Industrial_Item_Seeder_Master_Era_Design_Reference.md',
	'Design Documents/Seeding/FutureMUD_Industrialised_Component_Material_Tag_Gap_Reference.md',
	'Design Documents/Items/Item_System_Content_Workflows.md'
)
foreach ($relative in $linkTargets) {
	if ((Get-Content -Raw -LiteralPath (Join-Path $repoRoot $relative)) -cnotmatch 'FutureMUD_Industrialised_Food_Drink_Design_Reference\.md') { $issues.Add("Missing food design cross-link in $relative.") }
}

$report = [ordered]@{
	FamilyRows = $inventory.Count
	SharedBases = $scopeCounts.shared_imnf
	IndustrialBases = $scopeCounts.industrial_i
	ModernBases = $scopeCounts.modern_mnf
	NuclearBases = $scopeCounts.nuclear_nf
	InformationBases = $scopeCounts.information_f
	AllProposedBases = $identities.Count
	ServingManifests = $servings.Count
	AuditedItems = @($reuseRows | Where-Object kind -ne 'Liquid').Count
	AuditedLiquids = @($reuseRows | Where-Object kind -eq 'Liquid').Count
	AdoptedDependencies = @($reuseRows | Where-Object disposition -eq 'adopted-later-era-dependency').Count
	UnchangedReuse = @($reuseRows | Where-Object disposition -eq 'unchanged-reuse').Count
	ProposedStage2SharedOrdinaryTotal = 4773 + $scopeCounts.shared_imnf
	ProposedStage2IndustrialOrdinaryTotal = 550 + $scopeCounts.industrial_i
	InventorySha256 = (Get-FileHash $inventoryPath).Hash.ToLowerInvariant()
	ServingSha256 = (Get-FileHash $servingPath).Hash.ToLowerInvariant()
	ReuseSha256 = ([Convert]::ToHexString([System.Security.Cryptography.SHA256]::HashData([System.Text.Encoding]::UTF8.GetBytes($reuseContent)))).ToLowerInvariant()
}

function Test-Review {
	param([hashtable]$Report, [string]$Text)
	$reviewIssues = [System.Collections.Generic.List[string]]::new()
	$measures = [ordered]@{
		'Shared IMNF new bases'=$Report.SharedBases; 'Industrial-only new bases'=$Report.IndustrialBases; 'Modern-forward MNF new bases'=$Report.ModernBases
		'Nuclear-forward NF new bases'=$Report.NuclearBases; 'Information-only new bases'=$Report.InformationBases; 'All proposed new bases'=$Report.AllProposedBases
		'Existing food items audited'=$Report.AuditedItems; 'Existing liquids audited'=$Report.AuditedLiquids; 'Adopted later-era dependencies'=$Report.AdoptedDependencies
		'Unchanged reusable records'=$Report.UnchangedReuse; 'Serving manifests proposed'=$Report.ServingManifests
		'Proposed Stage 2 shared ordinary total'=$Report.ProposedStage2SharedOrdinaryTotal; 'Proposed Stage 2 Industrial-only ordinary total'=$Report.ProposedStage2IndustrialOrdinaryTotal
	}
	foreach ($entry in $measures.GetEnumerator()) {
		$expected = "| $($entry.Key) | $($entry.Value) |"
		if ([regex]::Matches($Text, ('(?m)^' + [regex]::Escape($expected) + '$')).Count -ne 1) { $reviewIssues.Add("Stale or missing review measure: $($entry.Key).") }
	}
	foreach ($entry in ([ordered]@{'Inventory'=$Report.InventorySha256;'Serving manifests'=$Report.ServingSha256;'Reuse review'=$Report.ReuseSha256}).GetEnumerator()) {
		$expected = '| ' + $entry.Key + ' | `' + $entry.Value + '` |'
		if ([regex]::Matches($Text, ('(?m)^' + [regex]::Escape($expected) + '$')).Count -ne 1) { $reviewIssues.Add("Stale or missing review fingerprint: $($entry.Key).") }
	}
	$reviewIssues
}

if ($CheckReview) {
	$reviewIssues = @(Test-Review $report (Get-Content -Raw -LiteralPath $reviewPath))
	foreach ($reviewIssue in $reviewIssues) { $issues.Add($reviewIssue) }
}

if ($SelfTest) {
	if (@(Expand-Reservations 'industrialised_food_test_{001..003}').Count -ne 3) { throw 'Self-test rejected a valid range.' }
	$detected = $false
	try { Expand-Reservations 'Bad Range' | Out-Null } catch { $detected = $true }
	if (-not $detected) { throw 'Self-test did not reject an invalid identity range.' }
	$stale = '| Shared IMNF new bases | 0 |'
	if (-not @(Test-Review $report $stale | Where-Object { $_ -like 'Stale or missing review measure*' }).Count) { throw 'Self-test did not detect review drift.' }
	'Self-tests passed: identity expansion, malformed range rejection and maintained-review drift.'
}

if ($issues.Count) {
	$issues | Write-Output
	throw "$($issues.Count) Industrialised food Wave 1 planning issue(s)."
}

[pscustomobject]$report | ConvertTo-Json -Depth 3
'PASS: structural Gate 1 planning checks only. This does not grant scope approval or production acceptance.'
