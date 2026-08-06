param(
	[switch]$Check
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$seedingDirectory = Join-Path $repoRoot "Design Documents\Seeding"
$aliasCataloguePath = Join-Path $seedingDirectory "PreIndustrial_Item_Seeder_Alias_Catalogue.md"
$sharedSourcePath = Join-Path $repoRoot "DatabaseSeeder\Seeders\ItemSeeder.PreIndustrialBaseline.cs"

function Get-Family {
	param([string]$StableReference)

	switch -Regex ($StableReference) {
		"^preindustrial_clothing_" { return "Clothing accessory" }
		"^preindustrial_door_" { return "Door or gate" }
		"^preindustrial_military_support_" { return "Military support" }
		"^preindustrial_time_" { return "Timekeeping" }
		"^preindustrial_water_" { return "Water or civic fixture" }
		"^preindustrial_writing_" { return "Writing" }
		"^preindustrial_trade_" { return "Trade container or packaging" }
		"^preindustrial_tool_" { return "Craft tool" }
		"^preindustrial_workshop_" { return "Workshop fixture" }
		"^preindustrial_printing_" { return "Printing" }
		"^preindustrial_navigation_" { return "Navigation" }
		"^preindustrial_surveying_" { return "Surveying" }
		"^preindustrial_optics_" { return "Optics" }
		"^preindustrial_science_" { return "Science" }
		"^preindustrial_firearms_" { return "Gunpowder support" }
		"^preindustrial_artillery_" { return "Gunpowder support" }
		default { throw "Unrecognised pre-industrial family: $StableReference" }
	}
}

function Get-RegionalCultureScope {
	param(
		[string]$EraLabel,
		[string]$StableReference,
		[string]$SourceStableReference
	)

	$combined = "$StableReference $SourceStableReference"
	switch -Regex ($combined) {
		"east_asian|chinese|song_|goryeo|joseon|japanese|heian|kamakura|ming_|qing_|ryuk" {
			return "East Asian cultures and contact zones using this form"
		}
		"qalam|islam|abbasid|fatimid|andalus|persian|ottoman|maghreb|mamluk|seljuk" {
			return "Islamicate, Persianate, and connected trade cultures using this form"
		}
		"south_asian|indian|rajput|chola|mughal|deccan" {
			return "South Asian cultures and connected trade zones using this form"
		}
		"norse|viking|north_sea|steppe|mongol|caravan" {
			return "Northern, steppe, or caravan cultures using this form"
		}
		default {
			return "Shared $EraLabel cultures where the form and technology are locally credible"
		}
	}
}

function Get-CraftContext {
	param([string]$StableReference)

	switch -Regex ($StableReference) {
		"writing|book|paper|parchment|scrib|calligraph|qalam|quill|pen_|stylus" {
			return "scribe, notary, school, archive, bookshop, or religious writing institution"
		}
		"smith|forge|smelt|metal|armour|weapon|sword|blade|anvil" {
			return "smithy, armourer, weaponsmith, mint, or metalworking guild"
		}
		"textile|spin|weav|dye|fuller|tenter|rope|loom|sewing|tailor" {
			return "textile workshop, dyer, fuller, tailor, sailmaker, or ropemaker"
		}
		"potter|pottery|clay|kiln|ceramic" {
			return "potter, kiln yard, tileworks, or ceramic workshop"
		}
		"glass|lens|optics" {
			return "glasshouse, glazier, lens grinder, optician, or learned institution"
		}
		"mason|stone|quarry|brick|lime" {
			return "mason, quarry, construction yard, or civic works"
		}
		"leather|tann|saddl|bone|horn|hide" {
			return "tanner, leatherworker, saddler, boneworker, or related guild"
		}
		"wood|carpent|join|cooper|lathe|saw|timber" {
			return "carpenter, joiner, turner, cooper, shipwright, or woodcraft guild"
		}
		"food|brew|mash|oil|fruit|flour|kitchen|oven" {
			return "kitchen, bakery, brewery, vintner, oil press, or food-processing workshop"
		}
		"apothec|medical|medicine|surg|mortar|pestle" {
			return "apothecary, healer, surgeon, infirmary, or medical workshop"
		}
		"jewel|lapidar|goldsmith|silversmith" {
			return "jeweller, lapidary, goldsmith, silversmith, or court workshop"
		}
		default {
			return "relevant workshop, profession, guild, estate, or civic institution"
		}
	}
}

function Get-ComponentReality {
	param(
		[string]$Family,
		[string]$StableReference,
		[bool]$IsAlias
	)

	if ($IsAlias) {
		switch ($Family) {
			"Trade container or packaging" {
				return "Copied source container components; contents and commodity availability are not implied"
			}
			"Writing" {
				return "Copied source writing or implement components; language and preset content are not implied"
			}
			"Military support" {
				return "Copied source carry, wear, or storage components; no weapon capability is added"
			}
			"Timekeeping" {
				return "Copied source components; no additional clock or scheduling mechanic is implied"
			}
			"Water or civic fixture" {
				return "Copied source components; no additional water-network mechanic is implied"
			}
			default {
				return "Copied source components; admission adds no mechanics"
			}
		}
	}

	if ($StableReference -eq "preindustrial_printing_blank_form") {
		return "Writable paper surface; no preset administrative workflow"
	}

	if ($StableReference -match "^preindustrial_printing_(broadside_sheet|pamphlet|almanac|printed_map_sheet)$") {
		return "Descriptive printed content; no fixed-content or publishing workflow mechanic"
	}

	if ($StableReference -match "^preindustrial_printing_(type_case)$") {
		return "Functional container; type contents and printing workflow are not supplied"
	}

	if ($StableReference -match "^preindustrial_printing_") {
		return "Descriptive printing tool or fixture; no production workflow mechanic"
	}

	if ($StableReference -match "^preindustrial_(navigation|surveying|optics|science)_") {
		if ($StableReference -match "chart_case|specimen_jar") {
			return "Functional container; sensing, accuracy, and scientific workflow are not supplied"
		}

		return "Descriptive instrument or fixture; no sensing, accuracy, or navigation mechanic"
	}

	if ($StableReference -match "^preindustrial_(firearms|artillery)_") {
		return "Seeded holdable, container, stack, and functional tags support physical loading; no firearm or ammunition component is implied"
	}

	if ($StableReference -match "^preindustrial_trade_") {
		return "Functional container or package only; named commodity and contents are not supplied"
	}

	return "Seeded components only; admission adds no mechanics"
}

function New-AdmissionRecord {
	param(
		[string]$EraKey,
		[string]$EraLabel,
		[pscustomobject]$Item
	)

	$stableReference = $Item.StableReference
	$sourceStableReference = $Item.SourceStableReference
	$isAlias = $sourceStableReference -ne "shared-authored"
	$family = Get-Family $stableReference
	$cultureScope = Get-RegionalCultureScope $EraLabel $stableReference $sourceStableReference

	switch ($EraKey) {
		"medieval" {
			$dateWindow = "500-1400 CE"
		}
		"renaissance" {
			$dateWindow = "1400-1600 CE"
		}
		"earlymodern" {
			$dateWindow = "1600-1750 CE"
		}
		default {
			throw "Unknown era key: $EraKey"
		}
	}

	$admittingContext = Get-CraftContext $stableReference
	$availability = "Specialist"
	$tradeStatus = "Local or interregional continuity"

	switch ($family) {
		"Clothing accessory" {
			$admittingContext = "household, clothier, outfitter, tailor, traveller, or ordinary market"
			$availability = "Ordinary"
			$tradeStatus = "Local manufacture or ordinary interregional trade"
		}
		"Door or gate" {
			$admittingContext = "household, farm, workshop, warehouse, civic, military, or religious building"
			$availability = "Ordinary or institutional"
			$tradeStatus = "Local construction tradition"
		}
		"Military support" {
			$admittingContext = "approved unit, armoury, guard, mounted service, archery, naval, or pageant package"
			$availability = "Military-controlled or specialist"
			$tradeStatus = "Local issue, captured stock, or military trade"
		}
		"Timekeeping" {
			$admittingContext = "court, civic watch, religious institution, school, observatory, or prosperous household"
			$availability = "Institutional or elite"
			$tradeStatus = "Local continuity, learned transfer, or imported instrument"
		}
		"Water or civic fixture" {
			$admittingContext = "civic works, estate, religious complex, bathhouse, garden, or irrigation system"
			$availability = "Institutional"
			$tradeStatus = "Local infrastructure"
		}
		"Writing" {
			$admittingContext = "scribe, notary, school, archive, court, merchant house, or religious institution"
			$availability = "Specialist or institutional"
			$tradeStatus = "Local production or literate interregional trade"
		}
		"Trade container or packaging" {
			$admittingContext = "merchant, warehouse, dock, caravan, customs house, estate store, or matching commodity craft"
			$availability = "Ordinary by form; named cargo remains gated"
			$tradeStatus = "Local, interregional, import, or export according to the named cargo"
		}
		"Craft tool" {
			$availability = "Specialist"
			$tradeStatus = "Local manufacture or craft transfer"
		}
		"Workshop fixture" {
			$availability = "Specialist or institutional"
			$tradeStatus = "Local installation or specialist transfer"
		}
	}

	if ($stableReference -match "^preindustrial_printing_") {
		$admittingContext = "approved print shop, bookshop, civic or religious press, university, court, or administration"
		$availability = "Restricted specialist"
		$tradeStatus = "Technology- and institution-gated"
		switch ($EraKey) {
			"medieval" {
				$cultureScope = "No default Medieval admission; use a separately authored woodblock form or later-era package"
				$dateWindow = "Not before 1450 CE for this movable-type form"
				$admittingContext = "none in the default Medieval manifest"
				$availability = "Not admitted"
				$tradeStatus = "Later-era technology"
			}
			"renaissance" {
				$cultureScope = "European movable-type regions after local adoption; other print cultures require form-specific rows"
				$dateWindow = "1450-1600 CE"
			}
			"earlymodern" {
				$cultureScope = "Admitted print cultures and institutions; technology is not universal"
				$dateWindow = "1600-1750 CE"
			}
		}
	}

	if ($stableReference -match "^preindustrial_(firearms|artillery)_") {
		$admittingContext = "approved firearm unit, armoury, gunsmith, powder store, ship, fort, or military supplier"
		$availability = "Restricted military"
		$tradeStatus = "Controlled military technology or contact transfer"
		switch ($EraKey) {
			"medieval" {
				$cultureScope = "No admission for these musket-era firearm-support forms; separately authored early hand-gonne or artillery props require their own package"
				$dateWindow = "Not before 1450 CE for this firearm-support suite"
				$admittingContext = "none in the default Medieval manifest"
				$availability = "Not admitted"
				$tradeStatus = "Later-era military technology"
			}
			"renaissance" {
				$cultureScope = "Approved pike-and-shot, artillery, naval, fortress, or contact-zone firearm cultures"
				$dateWindow = "1450-1600 CE"
			}
			"earlymodern" {
				$cultureScope = "Approved state, naval, fortress, militia, company, or contact-zone firearm cultures"
				$dateWindow = "1600-1750 CE"
			}
		}
	}

	switch ($stableReference) {
		"preindustrial_navigation_magnetic_compass" {
			if ($EraKey -eq "medieval") {
				$cultureScope = "East Asian, Indian Ocean, Islamicate, and later Mediterranean maritime cultures"
				$dateWindow = "1100-1400 CE"
			}
			$admittingContext = "navigator, pilot, ship, port, cartographer, or maritime school"
			$availability = "Restricted specialist"
			$tradeStatus = "Maritime knowledge transfer or imported instrument"
		}
		"preindustrial_navigation_cross_staff" {
			if ($EraKey -eq "medieval") {
				$cultureScope = "Late Medieval astronomical and maritime cultures using the form"
				$dateWindow = "1300-1400 CE"
			}
			$admittingContext = "navigator, astronomer, surveyor, pilot, or learned institution"
			$availability = "Restricted specialist"
			$tradeStatus = "Learned or maritime technology transfer"
		}
		"preindustrial_navigation_mariner_astrolabe" {
			if ($EraKey -eq "medieval") {
				$cultureScope = "No admission for this specifically maritime form; planispheric astrolabes require a separately authored row"
				$dateWindow = "Not before the late fifteenth century"
				$admittingContext = "none in the default Medieval manifest"
				$availability = "Not admitted"
				$tradeStatus = "Later-era maritime technology"
			}
			else {
				$admittingContext = "navigator, pilot, ship, port, cartographer, astronomer, or maritime school"
				$availability = "Restricted specialist"
				$tradeStatus = "Maritime knowledge transfer or imported instrument"
				if ($EraKey -eq "renaissance") {
					$dateWindow = "1450-1600 CE"
				}
			}
		}
		"preindustrial_surveying_plane_table" {
			$admittingContext = "surveyor, military engineer, cartographer, estate office, or civic works"
			$availability = "Restricted specialist"
			$tradeStatus = "Professional technology transfer"
			if ($EraKey -eq "medieval") {
				$cultureScope = "No default Medieval admission"
				$dateWindow = "Not before 1500 CE for this form"
				$admittingContext = "none in the default Medieval manifest"
				$availability = "Not admitted"
				$tradeStatus = "Later-era technology"
			}
		}
		"preindustrial_optics_spectacles" {
			$admittingContext = "scholar, scribe, court, merchant, physician, optician, or learned institution"
			$availability = "Elite or specialist"
			$tradeStatus = "Local specialist manufacture or imported optical goods"
			if ($EraKey -eq "medieval") {
				$cultureScope = "Late Medieval Latin European and connected learned or mercantile cultures"
				$dateWindow = "1280-1400 CE"
			}
		}
		"preindustrial_optics_magnifying_lens" {
			$admittingContext = "scholar, scribe, physician, jeweller, lens worker, court, or learned institution"
			$availability = "Elite or specialist"
			$tradeStatus = "Local specialist manufacture or imported optical goods"
			if ($EraKey -eq "medieval") {
				$cultureScope = "High and Late Medieval optical cultures with documented convex-lens manufacture"
				$dateWindow = "1000-1400 CE"
			}
		}
		"preindustrial_optics_telescope" {
			$admittingContext = "approved navigator, court, observatory, academy, military engineer, or optician"
			$availability = "Restricted specialist"
			$tradeStatus = "Imported or locally made optical technology"
			switch ($EraKey) {
				"medieval" {
					$cultureScope = "No Medieval admission"
					$dateWindow = "Not before 1608 CE"
					$admittingContext = "none"
					$availability = "Not admitted"
					$tradeStatus = "Later-era technology"
				}
				"renaissance" {
					$cultureScope = "No Renaissance admission"
					$dateWindow = "Not before 1608 CE"
					$admittingContext = "none"
					$availability = "Not admitted"
					$tradeStatus = "Later-era technology"
				}
				"earlymodern" {
					$cultureScope = "Cultures and institutions with documented optical-instrument access"
					$dateWindow = "1608-1750 CE"
				}
			}
		}
	}

	switch ($stableReference) {
		"preindustrial_trade_tea_chest" {
			switch ($EraKey) {
				"medieval" {
					$cultureScope = "East Asian and connected Inner Asian or maritime tea-trade cultures"
					$dateWindow = "700-1400 CE"
				}
				"renaissance" {
					$cultureScope = "East Asian tea cultures and explicitly admitted late maritime imports"
					$dateWindow = "1400-1600 CE; European maritime admission from about 1580"
				}
				"earlymodern" {
					$cultureScope = "East Asian tea cultures and documented global maritime tea routes"
					$dateWindow = "1600-1750 CE"
				}
			}
			$admittingContext = "tea producer, merchant, warehouse, caravan, port, company, court, or tea-service institution"
			$availability = "Restricted commodity packaging"
			$tradeStatus = "Local in producing cultures; imported or export-only elsewhere"
		}
		"preindustrial_trade_coffee_sack" {
			switch ($EraKey) {
				"medieval" {
					$cultureScope = "No default Medieval admission"
					$dateWindow = "Not before the fifteenth-century coffee trade"
					$admittingContext = "none in the default Medieval manifest"
					$availability = "Not admitted"
					$tradeStatus = "Later-era commodity network"
					break
				}
				"renaissance" {
					$cultureScope = "Red Sea, Arabian, Ottoman, and connected Islamicate coffee cultures; Europe only at the late edge"
					$dateWindow = "1450-1600 CE"
				}
				"earlymodern" {
					$cultureScope = "Red Sea, Ottoman, Indian Ocean, European, colonial, and company coffee routes where documented"
					$dateWindow = "1600-1750 CE"
				}
			}
			if ($EraKey -ne "medieval") {
				$admittingContext = "coffee producer, merchant, warehouse, port, company, court, or coffeehouse"
				$availability = "Restricted commodity packaging"
				$tradeStatus = "Local in producing cultures; imported or export-only elsewhere"
			}
		}
		"preindustrial_trade_cacao_sack" {
			switch ($EraKey) {
				"medieval" {
					$cultureScope = "Mesoamerican cacao-producing and tribute-trade cultures only"
					$dateWindow = "500-1400 CE"
				}
				"renaissance" {
					$cultureScope = "Mesoamerican cacao cultures and documented Atlantic contact routes"
					$dateWindow = "1400-1600 CE; transatlantic admission after about 1520"
				}
				"earlymodern" {
					$cultureScope = "Mesoamerican, Spanish Atlantic, colonial, and documented global cacao routes"
					$dateWindow = "1600-1750 CE"
				}
			}
			$admittingContext = "producer, tribute store, merchant, warehouse, port, company, court, or chocolate-service institution"
			$availability = "Restricted commodity packaging"
			$tradeStatus = "Local in producing cultures; imported or export-only elsewhere"
		}
		"preindustrial_trade_tobacco_bale" {
			switch ($EraKey) {
				"medieval" {
					$cultureScope = "Indigenous American tobacco cultures only; this wrapped bale form requires local approval"
					$dateWindow = "500-1400 CE"
				}
				"renaissance" {
					$cultureScope = "Indigenous American tobacco cultures and documented Atlantic contact routes"
					$dateWindow = "1400-1600 CE; transatlantic admission after about 1550"
				}
				"earlymodern" {
					$cultureScope = "Indigenous American, Atlantic, colonial, company, and documented global tobacco routes"
					$dateWindow = "1600-1750 CE"
				}
			}
			$admittingContext = "producer, merchant, warehouse, port, company, court, tobacconist, or tobacco-service institution"
			$availability = "Restricted commodity packaging"
			$tradeStatus = "Local in producing cultures; imported or export-only elsewhere"
		}
		"preindustrial_trade_sugar_hogshead" {
			switch ($EraKey) {
				"medieval" {
					$cultureScope = "Late Medieval Mediterranean, Islamicate, and connected sugar-trade cultures only"
					$dateWindow = "1200-1400 CE"
				}
				"renaissance" {
					$cultureScope = "Mediterranean, Atlantic, plantation, and documented long-distance sugar routes"
					$dateWindow = "1400-1600 CE"
				}
				"earlymodern" {
					$cultureScope = "Mediterranean, Atlantic, colonial, plantation, company, and documented global sugar routes"
					$dateWindow = "1600-1750 CE"
				}
			}
			$admittingContext = "sugar producer, refinery, merchant, warehouse, port, company, court, or confectioner"
			$availability = "Restricted commodity packaging"
			$tradeStatus = "Local in producing cultures; imported or export-only elsewhere"
		}
		"preindustrial_trade_spice_chest" {
			$cultureScope = "Asian, African, Middle Eastern, Mediterranean, and connected overland or maritime spice routes"
			$admittingContext = "spice merchant, caravan, warehouse, port, customs house, court, or apothecary"
			$availability = "Specialist commodity packaging"
			$tradeStatus = "Local, interregional, imported, or export-only by spice and route"
		}
		"preindustrial_trade_indigo_cake_box" {
			$cultureScope = "Indigo-producing cultures and documented textile-dye trade routes"
			$admittingContext = "dyer, producer, merchant, warehouse, caravan, port, customs house, or textile guild"
			$availability = "Specialist commodity packaging"
			$tradeStatus = "Local in producing cultures; imported or export-only elsewhere"
		}
		"preindustrial_trade_porcelain_packing_crate" {
			$cultureScope = "East Asian porcelain-producing cultures and documented maritime or overland export routes"
			$admittingContext = "kiln merchant, warehouse, caravan, port, customs house, company, court, or luxury-goods shop"
			$availability = "Restricted commodity packaging"
			$tradeStatus = "Local in producing cultures; imported or export-only elsewhere"
		}
		"preindustrial_trade_glass_bottle_crate" {
			$cultureScope = "Glass-producing cultures and documented bottle-trade routes"
			$admittingContext = "glasshouse, vintner, apothecary, merchant, warehouse, port, or bottle shop"
			$availability = "Specialist commodity packaging"
			$tradeStatus = "Local manufacture or documented import/export"
		}
		"preindustrial_trade_silk_bale" {
			$cultureScope = "Silk-producing cultures and documented Silk Road, Indian Ocean, Mediterranean, or maritime routes"
			$admittingContext = "producer, textile guild, merchant, caravan, warehouse, port, customs house, or court"
			$availability = "Restricted commodity packaging"
			$tradeStatus = "Local in producing cultures; imported or export-only elsewhere"
		}
		"preindustrial_trade_cotton_bale" {
			$cultureScope = "Cotton-producing cultures and documented overland, Indian Ocean, Mediterranean, or Atlantic routes"
			$admittingContext = "producer, textile guild, merchant, caravan, warehouse, port, customs house, or court"
			$availability = "Restricted commodity packaging"
			$tradeStatus = "Local in producing cultures; imported or export-only elsewhere"
		}
	}

	[pscustomobject]@{
		StableReference = $stableReference
		Source = $sourceStableReference
		Family = $family
		CultureScope = $cultureScope
		DateWindow = $dateWindow
		AdmittingContext = $admittingContext
		Availability = $availability
		TradeStatus = $tradeStatus
		ComponentReality = Get-ComponentReality $family $stableReference $isAlias
	}
}

function ConvertTo-ManifestMarkdown {
	param(
		[pscustomobject]$Era,
		[System.Collections.Generic.List[pscustomobject]]$Items
	)

	$records = $Items |
		ForEach-Object { New-AdmissionRecord $Era.Key $Era.Label $_ } |
		Sort-Object StableReference

	$lines = [System.Collections.Generic.List[string]]::new()
	$lines.Add("# FutureMUD $($Era.Title) Shared Baseline Admission Manifest")
	$lines.Add("")
	$lines.Add("**Status:** complete populated admission registry.")
	$lines.Add("")
	$lines.Add("## Purpose")
	$lines.Add("")
	$lines.Add("This manifest governs admission of the implemented shared pre-industrial item layer into $($Era.Label) content, $($Era.Range). It records a decision for every live shared prototype and never clones or authors item prototypes.")
	$lines.Add("")
	$lines.Add("The inventory is derived from ``PreIndustrial_Item_Seeder_Alias_Catalogue.md`` and ``PreIndustrialNewItemSpecs`` in ``ItemSeeder.PreIndustrialBaseline.cs``. The current contract is **390 unique shared rows**: **342 compatibility aliases** and **48 shared-authored rows**.")
	$lines.Add("")
	$lines.Add("## Admission contract")
	$lines.Add("")
	$lines.Add("Each record supplies the shared stable reference, its live source, family, culture/contact scope, date window, admitting institution/profession/shop/craft, prevalence, import/export status, and current component reality. ``Not admitted`` is an intentional completed decision, not a missing record.")
	$lines.Add("")
	$lines.Add("Automatic catalogue installation only makes a prototype available to builders. A row becomes ordinary world content only when a culture, outfit, craft, shop, institution, military package, or local builder decision satisfies this manifest.")
	$lines.Add("")
	$lines.Add("## Populated admission records")
	$lines.Add("")
	$lines.Add("<!-- admission-records:start -->")
	$lines.Add("| Shared stable reference | Live source | Family | Culture/contact scope | Date window | Admitting context | Availability | Trade/contact status | Component reality |")
	$lines.Add("| --- | --- | --- | --- | --- | --- | --- | --- | --- |")
	foreach ($record in $records) {
		$lines.Add("| ``$($record.StableReference)`` | ``$($record.Source)`` | $($record.Family) | $($record.CultureScope) | $($record.DateWindow) | $($record.AdmittingContext) | $($record.Availability) | $($record.TradeStatus) | $($record.ComponentReality) |")
	}
	$lines.Add("<!-- admission-records:end -->")
	$lines.Add("")
	$lines.Add("## Maintenance and acceptance")
	$lines.Add("")
	$lines.Add("- The record set exactly matches the live 390-row shared baseline and contains no duplicate stable references.")
	$lines.Add("- Every record has a culture/contact scope, date window, admitting context, availability, trade/contact decision, and component-reality statement.")
	$lines.Add("- High-risk printing, optics, gunpowder-support, and named global-trade packages have explicit era-specific gates.")
	$lines.Add("- Shared rows remain tagged ``Era / Pre-Industrial Era``; admission does not create era-prefixed clones.")
	$lines.Add("- Functional claims do not exceed the live components. Named packages do not imply contents, crops, processing chains, firearms, explosives, publishing systems, scientific sensing, or trade prevalence.")
	$lines.Add("- Regenerating this document creates no item prototypes and changes no database state.")
	$lines.Add("- Historical technology anchors: the [Smithsonian mariner's astrolabe](https://www.si.edu/object/nmah_997159) dates mariners' use of that form to the late fifteenth century, the [Smithsonian telescope timeline](https://www.sil.si.edu/exhibitions/chasing-venus/pop_timeline.htm) dates the telescope to 1608, and the Metropolitan Museum's [sixteenth-century powder flask](https://www.metmuseum.org/art/collection/search/33791) supports the Renaissance/Early Modern gate for this exact firearm-support suite.")

	return ($lines -join [Environment]::NewLine)
}

$items = [System.Collections.Generic.List[pscustomobject]]::new()
$aliasPattern = '^\| `(?<source>[^`]+)` \| `(?<stable>[^`]+)` \|$'
foreach ($line in Get-Content $aliasCataloguePath) {
	if ($line -match $aliasPattern) {
		$items.Add([pscustomobject]@{
			SourceStableReference = $Matches.source
			StableReference = $Matches.stable
		})
	}
}

$sharedSource = Get-Content -Raw $sharedSourcePath
$newItemPattern = 'new\(PreIndustrialItemGroup\.[A-Za-z]+, "(?<stable>preindustrial_[a-z0-9_]+)"'
foreach ($match in [regex]::Matches($sharedSource, $newItemPattern)) {
	$items.Add([pscustomobject]@{
		SourceStableReference = "shared-authored"
		StableReference = $match.Groups["stable"].Value
	})
}

$uniqueCount = ($items.StableReference | Sort-Object -Unique).Count
if ($items.Count -ne 390 -or $uniqueCount -ne 390) {
	throw "Expected 390 unique shared pre-industrial rows, found $($items.Count) rows and $uniqueCount unique references."
}

$eras = @(
	[pscustomobject]@{
		Key = "medieval"
		Label = "Medieval"
		Title = "Medieval"
		Range = "approximately 500-1400 CE"
		FileName = "FutureMUD_Medieval_Shared_Baseline_Admission_Manifest.md"
	},
	[pscustomobject]@{
		Key = "renaissance"
		Label = "Renaissance"
		Title = "Renaissance"
		Range = "approximately 1400-1600 CE"
		FileName = "FutureMUD_Renaissance_Shared_Baseline_Admission_Manifest.md"
	},
	[pscustomobject]@{
		Key = "earlymodern"
		Label = "Early Modern"
		Title = "Early Modern"
		Range = "approximately 1600-1750 CE"
		FileName = "FutureMUD_EarlyModern_Shared_Baseline_Admission_Manifest.md"
	}
)

$differences = [System.Collections.Generic.List[string]]::new()
foreach ($era in $eras) {
	$path = Join-Path $seedingDirectory $era.FileName
	$content = ConvertTo-ManifestMarkdown $era $items
	if ($Check) {
		if (-not (Test-Path $path) -or (Get-Content -Raw $path).TrimEnd() -ne $content.TrimEnd()) {
			$differences.Add($era.FileName)
		}
		continue
	}

	[System.IO.File]::WriteAllText(
		$path,
		$content + [Environment]::NewLine,
		[System.Text.UTF8Encoding]::new($false))
}

if ($Check -and $differences.Count -gt 0) {
	throw "Admission manifests are out of date: $($differences -join ', ')"
}

if ($Check) {
	Write-Output "Admission manifests are current: 3 files, 390 rows each."
}
else {
	Write-Output "Generated 3 admission manifests with 390 rows each."
}
