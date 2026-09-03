param(
	[switch]$SelfTest,
	[switch]$CheckReview
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$seedingRoot = Join-Path $repoRoot 'Design Documents/Seeding'

function Read-PlanningTable {
	param([string]$Path, [string[]]$Columns)
	$lineNumber = 0
	foreach ($line in (Get-Content -LiteralPath $Path)) {
		$lineNumber++
		if (-not $line.StartsWith('|')) { continue }
		if ($line -match '^\|[- :|]+$' -or $line.StartsWith('| Key |')) { continue }
		if ($line -cnotmatch '^\| [a-z][a-z0-9_]* \|') { throw "${Path}:${lineNumber}: invalid planning key or table row." }
		$cells = $line.Trim('|').Split('|').Trim()
		if ($cells.Count -ne $Columns.Count) { throw "${Path}:${lineNumber}: expected $($Columns.Count) columns, got $($cells.Count)." }
		$row = [ordered]@{ File = $Path; Line = $lineNumber }
		for ($i = 0; $i -lt $Columns.Count; $i++) { $row[$Columns[$i]] = $cells[$i] }
		[pscustomobject]$row
	}
}

function Get-SkinSlugs {
	param($Garment)
	if ($Garment.Skins -eq '-') { return }
	foreach ($brief in $Garment.Skins.Split(';').Trim()) {
		($brief.ToLowerInvariant() -replace '[^a-z0-9]+', '-').Trim('-')
	}
}

function Test-PlanningGraph {
	param([object[]]$Garments, [object[]]$Outfits, [hashtable]$Existing, [string]$SourceText)
	$issues = [System.Collections.Generic.List[string]]::new()
	$byKey = @{}
	$reuseKeys = @{}
	$evidenceCodes = @('FND','TAL','MOD','KNIT','WORK','UNI','SPORT','OUT','ARCTIC','ACC','SYN','HAT','SHO','SOUTHASIA','EASTASIA','AFRICA','PACIFIC','AMERICAS','REL','STOCK')
	foreach ($row in $Garments) {
		if ($byKey.ContainsKey($row.Key)) { $issues.Add("Duplicate garment $($row.Key)") }
		$byKey[$row.Key] = $row
		if ($row.Eras -cnotmatch '^I?M?N?F?$' -or -not $row.Eras) { $issues.Add("Invalid admissions $($row.Key): $($row.Eras)") }
		if ($row.Route -cnotin @('H','S','B')) { $issues.Add("Invalid route $($row.Key)") }
		if ($row.Evidence -cnotin $evidenceCodes) { $issues.Add("Unknown evidence workstream $($row.Key)") }
		if (-not $row.Design -or -not $row.Family) { $issues.Add("Missing design/family $($row.Key)") }
		$skins = @(Get-SkinSlugs $row)
		if ($row.Skins -ne '-' -and @($skins | Where-Object { -not $_ }).Count) { $issues.Add("Empty skin brief $($row.Key)") }
		if (@($skins | Sort-Object -Unique).Count -ne $skins.Count) { $issues.Add("Duplicate skin slug $($row.Key)") }
		if ('plain' -cin $skins) { $issues.Add("Redundant plain skin $($row.Key)") }
		if ($row.Source -eq 'new') {
			$prefix = if ($row.Eras.Length -gt 1) { 'industrialised' } else { @{ I='industrial'; M='modern'; N='nuclear'; F='information' }[$row.Eras] }
			$plannedKey = "${prefix}_clothing_$($row.Key)"
			if ($Existing.ContainsKey($plannedKey)) { $issues.Add("New identity already exists $plannedKey") }
		} else {
			if ($reuseKeys.ContainsKey($row.Source)) { $issues.Add("Duplicate reuse identity $($row.Source)") }
			$reuseKeys[$row.Source] = $true
			if (-not $Existing.ContainsKey($row.Source)) { $issues.Add("Missing manifest reuse $($row.Source)") }
			$escapedKey = [regex]::Escape($row.Source)
			if ($SourceText -cnotmatch "(?:new|CreateItem)\(\s*`"${escapedKey}`"\s*," -and
				$SourceText -cnotmatch "CreatePreIndustrialAlias\(\s*`"[^`"]+`"\s*,\s*`"${escapedKey}`"\s*,") {
				$issues.Add("Missing live source reuse $($row.Source)")
			}
		}
	}
	$outfitKeys = @{}
	foreach ($outfit in $Outfits) {
		if ($outfitKeys.ContainsKey($outfit.Key)) { $issues.Add("Duplicate outfit $($outfit.Key)") }
		$outfitKeys[$outfit.Key] = $true
		if ($outfit.Eras -cnotmatch '^I?M?N?F?$' -or -not $outfit.Eras) { $issues.Add("Invalid outfit admissions $($outfit.Key)") }
		if ($outfit.Palette -cnotin @('neutral','blue','earth','formal','mourning','white','saffron','maroon','sport')) { $issues.Add("Unknown palette $($outfit.Key)") }
		$used = @{}
		foreach ($token in $outfit.Garments.Split(';').Trim()) {
			$parts = $token.Split('@')
			$key = $parts[0]
			if (-not $byKey.ContainsKey($key)) { $issues.Add("Unknown garment ${key} in $($outfit.Key)"); continue }
			if ($used.ContainsKey($key)) { $issues.Add("Duplicate garment ${key} in $($outfit.Key)") }
			$used[$key] = $true
			$garment = $byKey[$key]
			foreach ($era in $outfit.Eras.ToCharArray()) {
				if (-not $garment.Eras.Contains($era)) { $issues.Add("Admission ${era}: ${key} cannot appear in $($outfit.Key)") }
			}
			if ($parts.Count -gt 2 -or ($parts.Count -eq 2 -and $parts[1] -cnotin @(Get-SkinSlugs $garment))) { $issues.Add("Unknown skin ${token} in $($outfit.Key)") }
		}
	}
	$issues.ToArray()
}

function Test-ReviewSnapshot {
	param($Report, [string]$ReviewText)
	$issues = [System.Collections.Generic.List[string]]::new()
	$measures = [ordered]@{
		'New bases'=$Report.NewBases; 'Reused bases'=$Report.ReusedBases
		'Total distinct bases'=($Report.NewBases + $Report.ReusedBases); 'Additional skin briefs'=$Report.PlannedSkins
		'New-base normal recipes'=$Report.NewBaseRecipes; 'Skin-product normal recipes'=$Report.SkinProductRecipes
		'Reused-default production obligations'=$Report.ReusedDefaultProductionObligations; 'Outfits across all four bands'=$Report.ProposedOutfits
		'Shared new bases'=$Report.SharedNewBases; 'Industrial-only new bases'=$Report.IndustrialNewBases
		'Modern-only new bases'=$Report.ModernNewBases; 'Nuclear-only new bases'=$Report.NuclearNewBases; 'Information-only new bases'=$Report.InformationNewBases
	}
	foreach ($measure in $measures.GetEnumerator()) {
		$expected = "| $($measure.Key) | $($measure.Value) |"
		$rows = [regex]::Matches($ReviewText, ('(?m)^\| ' + [regex]::Escape($measure.Key) + ' \|[^\r\n]+'))
		if ($rows.Count -ne 1 -or $rows[0].Value -cne $expected) { $issues.Add("Stale review measure: $($measure.Key)") }
	}
	foreach ($family in $Report.Families) {
		$expected = "| $($family.Family) | $($family.New) | $($family.Reused) | $($family.Skins) |"
		$rows = [regex]::Matches($ReviewText, ('(?m)^\| ' + [regex]::Escape($family.Family) + ' \|[^\r\n]+'))
		if ($rows.Count -ne 1 -or $rows[0].Value -cne $expected) { $issues.Add("Stale family count: $($family.Family)") }
	}
	$eraNames = @{ I='Industrial'; M='Modern'; N='Nuclear'; F='Information' }
	foreach ($admission in $Report.Admissions) {
		$expected = "| $($eraNames[$admission.Era]) | $($admission.New) | $($admission.Reused) | $($admission.Skins) | $($admission.Outfits) |"
		$rows = [regex]::Matches($ReviewText, ('(?m)^\| ' + $eraNames[$admission.Era] + ' \| \d+ \|[^\r\n]+'))
		if ($rows.Count -ne 1 -or $rows[0].Value -cne $expected) { $issues.Add("Stale admission count: $($admission.Era)") }
	}
	foreach ($hash in @($Report.InventorySha256,$Report.OutfitsSha256,$Report.SourcePresenceSha256)) {
		if (-not $ReviewText.Contains($hash)) { $issues.Add("Stale review fingerprint: $hash") }
	}
	$issues.ToArray()
}

$garments = @(Read-PlanningTable (Join-Path $seedingRoot 'Industrialised_Clothing_Wave1_Inventory.md') @('Key','Family','Eras','Route','Source','Design','Skins','Evidence'))
$outfits = @(Read-PlanningTable (Join-Path $seedingRoot 'Industrialised_Clothing_Wave1_Outfits.md') @('Key','Eras','Garments','Palette','Purpose'))
if (-not $garments.Count -or -not $outfits.Count) { throw 'Planning tables must not be empty.' }
$manifest = Get-Content (Join-Path $seedingRoot 'Seeded_Item_Manifest.json') -Raw | ConvertFrom-Json -Depth 70
$existing = @{}
foreach ($entry in $manifest.entries) { if ($entry.entityType -eq 'item') { $existing[$entry.stableKey] = $true } }
$sourceFiles = @(Get-ChildItem -LiteralPath (Join-Path $repoRoot 'DatabaseSeeder/Seeders') -Filter 'ItemSeeder*.cs' | Sort-Object Name)
$sourceText = ($sourceFiles | ForEach-Object { Get-Content -LiteralPath $_.FullName -Raw }) -join "`n"
$sourceHashInput = (($sourceFiles | ForEach-Object { "$($_.Name):$((Get-FileHash -LiteralPath $_.FullName).Hash)" }) -join "`n") + "`n" + (Get-FileHash -LiteralPath (Join-Path $seedingRoot 'Seeded_Item_Manifest.json')).Hash
$sourceHash = [Convert]::ToHexString([System.Security.Cryptography.SHA256]::HashData([System.Text.Encoding]::UTF8.GetBytes($sourceHashInput))).ToLowerInvariant()
$issues = @(Test-PlanningGraph $garments $outfits $existing $sourceText)
if ($issues.Count) { $issues | Write-Output; throw "$($issues.Count) Wave 1 planning graph issues." }

if ($SelfTest) {
	$sample = [pscustomobject]@{ Key='sample'; Family='upper'; Eras='IMNF'; Route='H'; Source='new'; Design='A deliberately simple test garment'; Skins='embroidered edge'; Evidence='TAL' }
	$sampleOutfit = [pscustomobject]@{ Key='sample_outfit'; Eras='I'; Garments='sample@embroidered-edge'; Palette='neutral' }
	if (@(Test-PlanningGraph @($sample) @($sampleOutfit) @{} '').Count) { throw 'Self-test valid graph rejected.' }
	$reuseSample = [pscustomobject]@{ Key='sample'; Family='upper'; Eras='IMNF'; Route='H'; Source='stock'; Design='Existing garment'; Skins='embroidered edge'; Evidence='STOCK' }
	foreach ($sourceFixture in @('new("stock", "shirt", "a shirt")', "CreateItem(`n`t`"stock`",`n`t`"shirt`")", 'CreatePreIndustrialAlias("legacy", "stock", "shirt")')) {
		if (@(Test-PlanningGraph @($reuseSample) @($sampleOutfit) @{stock=$true} $sourceFixture).Count) { throw 'Self-test literal source shape rejected.' }
	}
	$mutations = @(
		@{ Garments=@($sample,$sample); Outfits=@($sampleOutfit); Expected='Duplicate garment' },
		@{ Garments=@($sample); Outfits=@([pscustomobject]@{Key='bad';Eras='I';Garments='absent';Palette='neutral'}); Expected='Unknown garment' },
		@{ Garments=@($sample); Outfits=@([pscustomobject]@{Key='bad';Eras='I';Garments='sample@absent';Palette='neutral'}); Expected='Unknown skin' },
		@{ Garments=@([pscustomobject]@{Key='sample';Family='upper';Eras='M';Route='H';Source='new';Design='Test';Skins='embroidered edge';Evidence='TAL'}); Outfits=@($sampleOutfit); Expected='Admission' },
		@{ Garments=@([pscustomobject]@{Key='sample';Family='upper';Eras='IMNF';Route='H';Source='missing_stock';Design='Test';Skins='embroidered edge';Evidence='STOCK'}); Outfits=@($sampleOutfit); Expected='Missing manifest reuse' },
		@{ Garments=@([pscustomobject]@{Key='sample';Family='upper';Eras='IMNF';Route='H';Source='new';Design='Test';Skins='plain';Evidence='TAL'}); Outfits=@(); Expected='Redundant plain skin' },
		@{ Garments=@($reuseSample); Outfits=@($sampleOutfit); Expected='Missing live source reuse' },
		@{ Garments=@([pscustomobject]@{Key='sample';Family='upper';Eras='IMNF';Route='H';Source='new';Design='Test';Skins='embroidered; ';Evidence='TAL'}); Outfits=@(); Expected='Empty skin brief' }
	)
	foreach ($mutation in $mutations) {
		$result = @(Test-PlanningGraph $mutation.Garments $mutation.Outfits @{} '')
		if (-not @($result | Where-Object { $_.StartsWith($mutation.Expected) }).Count) { throw "Self-test did not detect $($mutation.Expected)." }
	}
	"Self-tests passed: valid graph, three literal source shapes and $($mutations.Count) negative cases."
}

$new = @($garments | Where-Object Source -eq 'new')
$reused = @($garments | Where-Object Source -ne 'new')
$skinCount = @($garments | ForEach-Object { Get-SkinSlugs $_ }).Count
$shared = @($new | Where-Object { $_.Eras.Length -ge 2 }).Count
$industrial = @($new | Where-Object Eras -eq 'I').Count
$report = [ordered]@{
	GarmentFamilies = @($garments.Family | Sort-Object -Unique).Count
	NewBases = $new.Count
	ReusedBases = $reused.Count
	PlannedSkins = $skinCount
	NewBaseRecipes = $new.Count
	SkinProductRecipes = $skinCount
	ReusedDefaultProductionObligations = $reused.Count
	ProposedOutfits = $outfits.Count
	SharedNewBases = $shared
	IndustrialNewBases = $industrial
	ModernNewBases = @($new | Where-Object Eras -eq 'M').Count
	NuclearNewBases = @($new | Where-Object Eras -eq 'N').Count
	InformationNewBases = @($new | Where-Object Eras -eq 'F').Count
	ProposedStage2SharedOrdinaryTotal = 5200 + $shared
	ProposedStage2IndustrialOrdinaryTotal = 580 + $industrial
	ProposedStage2OrdinaryTotal = 5780 + $shared + $industrial
	InventorySha256 = (Get-FileHash (Join-Path $seedingRoot 'Industrialised_Clothing_Wave1_Inventory.md')).Hash.ToLowerInvariant()
	OutfitsSha256 = (Get-FileHash (Join-Path $seedingRoot 'Industrialised_Clothing_Wave1_Outfits.md')).Hash.ToLowerInvariant()
	SourcePresenceSha256 = $sourceHash
	Families = @($garments | Group-Object Family | Sort-Object Name | ForEach-Object {
		[ordered]@{ Family=$_.Name; New=@($_.Group | Where-Object Source -eq 'new').Count; Reused=@($_.Group | Where-Object Source -ne 'new').Count; Skins=@($_.Group | ForEach-Object { Get-SkinSlugs $_ }).Count }
	})
	Admissions = @('I','M','N','F' | ForEach-Object {
		$eraLetter = $_
		$admitted = @($garments | Where-Object { $_.Eras.Contains($eraLetter) })
		[ordered]@{ Era=$eraLetter; New=@($admitted | Where-Object Source -eq 'new').Count; Reused=@($admitted | Where-Object Source -ne 'new').Count; Skins=@($admitted | ForEach-Object { Get-SkinSlugs $_ }).Count; Outfits=@($outfits | Where-Object { $_.Eras.Contains($eraLetter) }).Count }
	})
}
if ($CheckReview) {
	$reviewText = Get-Content -LiteralPath (Join-Path $seedingRoot 'Industrialised_Clothing_Wave1_Evidence_and_Coverage.md') -Raw
	$reviewIssues = @(Test-ReviewSnapshot $report $reviewText)
	if ($reviewIssues.Count) { $reviewIssues | Write-Output; throw 'Maintained scope review is stale; re-review changes before updating its counts or fingerprints.' }
	$familyReview = Get-Content -LiteralPath (Join-Path $seedingRoot 'Industrialised_Clothing_Wave1_Reuse_Review.md') -Raw
	foreach ($family in $report.Families) {
		if ([regex]::Matches($familyReview, ('(?m)^\| ' + [regex]::Escape($family.Family) + ' \|')).Count -ne 1) { throw "Missing or duplicate family review: $($family.Family)" }
	}
	if ($SelfTest) {
		$staleText = $reviewText.Replace("| New bases | $($report.NewBases) |", '| New bases | 0 |')
		if (-not @(Test-ReviewSnapshot $report $staleText | Where-Object { $_ -eq 'Stale review measure: New bases' }).Count) { throw 'Self-test failed to detect review count drift.' }
		$staleText = $reviewText.Replace($report.InventorySha256, ('0' * 64))
		if (-not @(Test-ReviewSnapshot $report $staleText | Where-Object { $_.StartsWith('Stale review fingerprint:') }).Count) { throw 'Self-test failed to detect fingerprint drift.' }
		$staleText = $reviewText + "`n| New bases | $($report.NewBases) |"
		if (-not @(Test-ReviewSnapshot $report $staleText | Where-Object { $_ -eq 'Stale review measure: New bases' }).Count) { throw 'Self-test failed to detect a duplicate review measure.' }
		'Self-tests passed: three maintained-review drift cases.'
	}
	'PASS: maintained review counts, fingerprints and family coverage; this does not grant user approval.'
}
[pscustomobject]$report | ConvertTo-Json -Depth 5
'PASS: structural scope graph and existing-reference presence only. This does not approve scope, prices, prose, chronology, cultural fidelity or runtime layering.'
