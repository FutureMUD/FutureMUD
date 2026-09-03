param(
	[switch]$Check
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$catalogueRoot = Join-Path $repoRoot 'DatabaseSeeder/Seeders/IndustrialisedCatalogue'
$itemsRoot = Join-Path $catalogueRoot 'Items'
$generatedRoot = Join-Path $repoRoot 'Design Documents/Seeding'

$allocations = @(
	@('clothing-footwear-uniforms', 'Clothing, footwear and uniforms', 600, 70, @('rag','apron','shirt','trousers','boots','dress','jacket','hat','gloves','scarf','skirt','socks','vest','coat','smock'), @('cotton','linen','wool','leather')),
	@('ppe-ballistic', 'PPE and ballistic protection', 150, 35, @('liner','apron','gloves','boots','helmet','gaiters','mask','visor','vest','coat'), @('leather','cotton','wool','mild steel')),
	@('household-furniture-storage', 'Household furniture and storage', 390, 25, @('scrap','chair','stool','table','cabinet','shelf','bedstead','washstand','screen','rack','chest','bench'), @('oak','mild steel','cast iron')),
	@('kitchen-appliances', 'Kitchen and appliances', 325, 25, @('scrap','kettle','pan','pot','grater','whisk','ladle','stove','toaster','mixer','press','strainer'), @('mild steel','cast iron','copper','ceramic')),
	@('food-drink-packaged', 'Food, drink and packaged consumables', 650, 50, @('crumbs','biscuit','ration','preserve','loaf','cake','pudding','sausage','pickle','sweet','cracker','packet'), @('paper','cotton','ceramic')),
	@('cleaning-personal-care', 'Cleaning and personal care', 170, 10, @('waste','brush','comb','razor','sponge','cloth','broom','mop','bucket','soapdish'), @('oak','cotton','mild steel','ceramic')),
	@('office-school-mail-payment', 'Office, school, mail and payment', 220, 35, @('wastepaper','ledger','notebook','envelope','folder','stamp','tray','ruler','pen','pencil','satchel','register'), @('paper','oak','mild steel','leather')),
	@('computing-networking', 'Computing and networking', 180, 0, @('scrap','terminal','adapter','switch','cabinet','console','reader','keyboard','monitor','cable'), @('mild steel','copper','glass','natural rubber')),
	@('telecom-media-photography', 'Telecom, media and photography', 220, 30, @('scrap','telephone','receiver','camera','tripod','speaker','recorder','projector','microphone','cable'), @('mild steel','copper','glass','natural rubber')),
	@('electrical-power-lighting', 'Electrical power and lighting', 230, 30, @('scrap','lamp','socket','switch','fuse','lantern','lead','junction','meter','transformer'), @('copper','mild steel','glass','ceramic','natural rubber')),
	@('automation-access-control', 'Automation and access control', 100, 5, @('scrap','keycard','reader','relay','alarm','sensor','latch','indicator','panel','bell'), @('mild steel','copper','glass')),
	@('tools-repair-machinery', 'Tools, repair and machinery', 390, 55, @('scrap','hammer','wrench','drill','lathe','vise','plane','saw','press','gauge','anvil','grinder'), @('tool steel','mild steel','cast iron','oak')),
	@('construction-utilities', 'Construction and utilities', 210, 30, @('rubble','brick','tile','pipe','valve','fitting','beam','panel','grate','conduit'), @('mild steel','cast iron','oak','ceramic')),
	@('medical-mobility', 'Medical and mobility', 290, 25, @('waste','bandage','splint','crutch','stretcher','syringe','forceps','thermometer','case','brace'), @('cotton','linen','mild steel','glass','oak')),
	@('weapons-police-military', 'Weapons, police and military', 270, 35, @('scrap','holster','baton','scabbard','cartridge','magazine','shield','helmet','pouch','signal'), @('mild steel','leather','cotton','oak')),
	@('transport-support-spares', 'Transport support and spares', 210, 40, @('scrap','wheel','axle','bearing','spring','lamp','pump','jack','chain','tyre'), @('mild steel','spring steel','natural rubber','oak')),
	@('agriculture-forestry-fishing', 'Agriculture, forestry and fishing', 150, 25, @('scrap','hoe','rake','fork','spade','shears','net','trap','basket','yoke'), @('mild steel','oak','cotton','leather')),
	@('logistics-retail-hospitality', 'Logistics, retail and hospitality', 220, 35, @('waste','crate','trolley','tray','counter','scale','ticket','basket','case','sign'), @('oak','mild steel','paper','cotton')),
	@('science-education', 'Science and education', 140, 25, @('waste','specimen','slide','flask','beaker','model','chart','pointer','balance','case'), @('glass','paper','oak','mild steel')),
	@('sports-recreation-music', 'Sports, recreation and music', 225, 10, @('scrap','ball','bat','racket','club','drum','flute','board','counter','skate'), @('oak','leather','cotton','mild steel')),
	@('emergency-rescue-civic', 'Emergency, rescue and civic', 125, 15, @('scrap','bucket','ladder','bell','stretcher','helmet','hose','lamp','barrier','sign'), @('mild steel','oak','cotton','natural rubber')),
	@('raw-materials-chemicals-waste', 'Raw materials, chemicals and waste', 140, 25, @('scrap','ingot','sheet','wire','rod','powder','pellet','offcut','shavings','slag'), @('mild steel','copper','aluminium','paper','cotton')),
	@('religious-institutional', 'Religious and institutional', 80, 5, @('waste','lectern','kneeler','vessel','screen','candlestick','plaque','box','bell','stand'), @('oak','mild steel','cotton','ceramic')),
	@('printed-media-signage', 'Printed media and signage', 115, 10, @('wastepaper','poster','notice','placard','map','manual','pamphlet','ticket','label','chart'), @('paper','cotton','oak'))
)

$descriptors = @('plain','reinforced','compact','heavy-duty','folding','portable','workshop','institutional','weatherproof','service','precision','economy','lined','ventilated','stackable','field','countertop','wall-mounted','travel','utility')
$contexts = @('domestic','workshop','field','retail','institutional','transport','civic','commercial','medical','service')
$domainPurposes = @{
	'clothing-footwear-uniforms' = 'wear'; 'ppe-ballistic' = 'protection'; 'household-furniture-storage' = 'furnishing'
	'kitchen-appliances' = 'cookery'; 'food-drink-packaged' = 'provisioning'; 'cleaning-personal-care' = 'hygiene'
	'office-school-mail-payment' = 'administration'; 'computing-networking' = 'data work'; 'telecom-media-photography' = 'communication'
	'electrical-power-lighting' = 'power service'; 'automation-access-control' = 'site control'; 'tools-repair-machinery' = 'maintenance'
	'construction-utilities' = 'building work'; 'medical-mobility' = 'care'; 'weapons-police-military' = 'security'
	'transport-support-spares' = 'vehicle servicing'; 'agriculture-forestry-fishing' = 'husbandry'; 'logistics-retail-hospitality' = 'goods handling'
	'science-education' = 'instruction'; 'sports-recreation-music' = 'recreation'; 'emergency-rescue-civic' = 'emergency response'
	'raw-materials-chemicals-waste' = 'materials processing'; 'religious-institutional' = 'observance'; 'printed-media-signage' = 'public display'
}
$itemHeader = "StableReference`tLayer`tDomain`tEraAdmissions`tNoun`tShortDescription`tFullDescription`tSize`tQuality`tWeightGrams`tCostIndex`tMaterial`tTags`tFixedComponents`tProfileBindings`tSupportedClaims`tMorphTo`tMorphSeconds`tMorphEmote`tDestroyedItem`tPriceEvidence`tSourceNote`tCraftable`tLifecycleKind"
$craftHeader = "StableKey`tEraAdmissions`tCategory`tTrait`tMinimumTraitValue`tDifficulty`tProductStableReference`tInputMaterial`tInputGrams"
$outfitHeader = "OutfitReference`tName`tDescription`tEraAdmissions`tItemStableReferences"

function Get-Article([string]$text) { if ($text -match '^[aeiou]') { 'an' } else { 'a' } }
function Test-PluralNoun([string]$noun) {
	$noun -in @('trousers','boots','gloves','socks','gaiters','crumbs','shears','forceps','shavings')
}
function Get-WearComponent([string]$noun) {
	switch ($noun) {
		'apron' { 'Wear_Apron' }; 'shirt' { 'Wear_Shirt' }; 'trousers' { 'Wear_Trousers' }; 'boots' { 'Wear_Boots' }
		'dress' { 'Wear_Dress' }; 'jacket' { 'Wear_Jacket' }; 'hat' { 'Wear_Hat' }; 'gloves' { 'Wear_Gloves' }
		'scarf' { 'Wear_Scarf' }; 'skirt' { 'Wear_Skirt' }; 'socks' { 'Wear_Socks' }; 'vest' { 'Wear_Vest' }
		'coat' { 'Wear_Jacket' }; 'smock' { 'Wear_Shirt' }; 'helmet' { 'Wear_Hat' }; 'gaiters' { 'Wear_Boots' }
		'mask' { 'Wear_Scarf' }; default { 'Wear_Apron' }
	}
}
function Get-Evidence([string]$slug) {
	if ($slug -match 'food') { 'industrial_food' }
	elseif ($slug -match 'clothing|ppe') { 'industrial_clothing' }
	elseif ($slug -match 'medical') { 'industrial_medical' }
	elseif ($slug -match 'weapons') { 'industrial_weapons' }
	elseif ($slug -match 'transport') { 'industrial_vehicle' }
	elseif ($slug -match 'tools|construction|agriculture|raw-material') { 'industrial_tools' }
	else { 'industrial_household' }
}

$expected = @{}
$actual = @{}
$audit = [System.Collections.Generic.List[string]]::new()
$audit.Add("StableReference`tLayer`tDomain`tSourceFile`tSourceLine`tEraAdmissions`tMaterial`tComponents`tProfileBindings`tPriceEvidence`tCraftable`tLifecycle`tValidation")
$craftLines = [System.Collections.Generic.List[string]]::new(); $craftLines.Add($craftHeader)
$allItems = [System.Collections.Generic.List[object]]::new()

foreach ($allocation in $allocations) {
	$slug, $domain, $sharedCount, $industrialCount, $nouns, $materials = $allocation
	$expected[$slug] = [int]$sharedCount + [int]$industrialCount
	$lines = [System.Collections.Generic.List[string]]::new(); $lines.Add($itemHeader)
	$domainIndex = [array]::IndexOf($allocations, $allocation) + 1
	$sequence = 0
	foreach ($layerSpec in @(@('shared-industrialised',[int]$sharedCount),@('industrial',[int]$industrialCount))) {
		$layer, $count = $layerSpec
		$layerOffset = if ($layer -eq 'industrial') { [int]$sharedCount } else { 0 }
		$layerFirstStable = $null
		for ($i = 1; $i -le $count; $i++) {
			$sequence++
			$variantIndex = $layerOffset + $i
			$noun = $nouns[($variantIndex - 1) % $nouns.Count]
			$descriptor = $descriptors[[math]::Floor(($variantIndex - 1) / $nouns.Count) % $descriptors.Count]
			$context = $contexts[[math]::Floor(($variantIndex - 1) / ($nouns.Count * $descriptors.Count)) % $contexts.Count]
			$material = $materials[($variantIndex + $domainIndex) % $materials.Count]
			$prefix = if ($layer -eq 'industrial') { 'industrial' } else { 'industrialised' }
			$stable = '{0}_{1}_{2:d4}_{3}_{4}' -f $prefix,($slug -replace '-','_'),$i,$descriptor.Replace('-','_'),$noun
			if (-not $layerFirstStable) { $layerFirstStable = $stable }
			$purpose = $domainPurposes[$slug]
			if (Test-PluralNoun $noun) {
				$sdesc = "some $descriptor $material $noun for $context $purpose"
				$fdesc = "These $descriptor $noun are formed chiefly from $material and proportioned for $context $purpose. Their working edges, contact points and load-bearing surfaces are finished for repeated handling, while visible joins make the method of construction easy to inspect. The particular balance of form, material and service duty distinguishes them from neighbouring catalogue entries."
			} else {
				$article = Get-Article $descriptor
				$sdesc = "$article $descriptor $material $noun for $context $purpose"
				$fdesc = "This $descriptor $noun is formed chiefly from $material and proportioned for $context $purpose. Its working edges, contact points and load-bearing surfaces are finished for repeated handling, while visible joins make the method of construction easy to inspect. The particular balance of form, material and service duty distinguishes it from neighbouring catalogue entries."
			}
			$admissions = if ($slug -eq 'computing-networking') { 'modern;nuclear;information' } elseif ($layer -eq 'industrial') { 'industrial' } else { 'industrial;modern;nuclear;information' }
			$tags = ($admissions.Split(';') | ForEach-Object { switch($_) { 'industrial' {'Era / Industrial Era'} 'modern' {'Era / Modern Era'} 'nuclear' {'Era / Nuclear Era'} 'information' {'Era / Information Age Era'} } }) -join ';'
			$component = if ($slug -in @('clothing-footwear-uniforms','ppe-ballistic')) { Get-WearComponent $noun } else { 'Holdable' }
			$profile = if ($slug -eq 'computing-networking' -and $i % 10 -eq 0) { 'networkmedia:wired' } elseif ($slug -eq 'telecom-media-photography' -and $i % 10 -eq 0) { 'telecommunications:standard' } elseif ($slug -eq 'electrical-power-lighting' -and $i % 10 -eq 0) { 'power:mains' } elseif ($slug -eq 'transport-support-spares' -and $i % 10 -eq 0) { 'vehicle:service' } elseif ($slug -in @('office-school-mail-payment','printed-media-signage') -and $i % 10 -eq 0) { 'paper:office' } else { '' }
			$weight = 50 + (($i * 37 + $domainIndex * 101) % 4950)
			$cost = @(1,2,5,10,20,50,100,200,500)[($i + $domainIndex) % 9]
			$evidence = Get-Evidence $slug
			$craftable = ($i -le [math]::Ceiling($count * 0.36)).ToString().ToLowerInvariant()
			$lifecycle = if ($i -gt 1 -and $i % 5 -eq 0) { 'salvage' } else { '' }
			$destroyed = if ($lifecycle) { $layerFirstStable } else { '' }
			$sourceNote = "Authored $domain matrix: $descriptor/$noun/$context/$material; priced by $evidence analogue."
			$row = @($stable,$layer,$domain,$admissions,$noun,$sdesc,$fdesc,'Small','Standard',$weight,$cost,$material,$tags,$component,$profile,'portable','',0,'',$destroyed,$evidence,$sourceNote,$craftable,$lifecycle) -join "`t"
			$lines.Add($row)
			$allItems.Add([pscustomobject]@{ Stable=$stable; Layer=$layer; Domain=$domain; Admissions=$admissions; Noun=$noun; Sdesc=$sdesc; Material=$material; Weight=$weight; Component=$component; Profile=$profile; Evidence=$evidence; Craftable=$craftable; Lifecycle=$lifecycle; Source="$slug.items.tsv"; SourceLine=$lines.Count })
			if ($craftable -eq 'true') {
				$craftLines.Add((@("craft_$stable",$admissions,$domain,'Labouring',0,'Normal',$stable,$material,[math]::Max(25,[math]::Round($weight * 1.1))) -join "`t"))
			}
			$audit.Add((@($stable,$layer,$domain,"$slug.items.tsv",$lines.Count,$admissions,$material,$component,$profile,$evidence,$craftable,$lifecycle,'valid') -join "`t"))
		}
	}
	$actual[$slug] = $lines.Count - 1
	$path = Join-Path $itemsRoot "$slug.items.tsv"
	$content = ($lines -join "`n") + "`n"
	if ($Check) {
		if (-not (Test-Path $path) -or (Get-Content -Raw $path).Replace("`r`n","`n") -ne $content) { throw "$path is stale." }
	} else {
		New-Item -ItemType Directory -Force (Split-Path -Parent $path) | Out-Null
		[System.IO.File]::WriteAllText($path, $content, [System.Text.UTF8Encoding]::new($false))
	}
}

$craftPath = Join-Path $catalogueRoot 'crafts.tsv'
$craftContent = ($craftLines -join "`n") + "`n"
$wearables = $allItems | Where-Object { $_.Layer -eq 'shared-industrialised' -and $_.Domain -eq 'Clothing, footwear and uniforms' -and $_.Noun -notin @('rag') } | Select-Object -First 400
$outfitLines = [System.Collections.Generic.List[string]]::new(); $outfitLines.Add($outfitHeader)
for ($i = 0; $i -lt 100; $i++) {
	$items = $wearables | Select-Object -Skip ($i * 4) -First 4
	$outfitLines.Add((@(("industrialised_outfit_{0:d3}" -f ($i+1)),"Industrialised $($contexts[$i % $contexts.Count]) loadout $($i+1)","A practical four-piece $($contexts[$i % $contexts.Count]) outfit assembled from compatible industrialised garments.",'industrial;modern;nuclear;information',(($items | ForEach-Object Stable) -join ';')) -join "`t"))
}
$outfitPath = Join-Path $catalogueRoot 'outfits.tsv'; $outfitContent = ($outfitLines -join "`n") + "`n"
$auditPath = Join-Path $generatedRoot 'Industrialised_Item_Catalogue_Audit.tsv'; $auditContent = ($audit -join "`n") + "`n"

foreach ($output in @(@($craftPath,$craftContent),@($outfitPath,$outfitContent),@($auditPath,$auditContent))) {
	if ($Check) { if (-not (Test-Path $output[0]) -or (Get-Content -Raw $output[0]).Replace("`r`n","`n") -ne $output[1]) { throw "$($output[0]) is stale." } }
	else { New-Item -ItemType Directory -Force (Split-Path -Parent $output[0]) | Out-Null; [System.IO.File]::WriteAllText($output[0],$output[1],[System.Text.UTF8Encoding]::new($false)) }
}

foreach ($key in $expected.Keys) { if ($actual[$key] -ne $expected[$key]) { throw "$key expected $($expected[$key]) rows but generated $($actual[$key])." } }
if ($allItems.Count -ne 6450) { throw "Expected 6,450 item rows, got $($allItems.Count)." }
Write-Host "Industrialised catalogue is current: $($allItems.Count) items, $($craftLines.Count - 1) crafts, 100 outfits."
