param(
	[switch]$Check
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$catalogueRoot = Join-Path $repoRoot "DatabaseSeeder\Seeders\FoodCatalogue"
$seedingRoot = Join-Path $repoRoot "Design Documents\Seeding"

function Get-AdmissionDecision {
	param(
		[string]$Era,
		[string]$Profile
	)

	$eraWindow = switch ($Era) {
		"Medieval" { "500-1400 CE" }
		"Renaissance" { "1400-1600 CE" }
		"Early Modern" { "1600-1750 CE" }
	}

	$decision = [ordered]@{
		CultureScope = "Cultures where the ingredients and preparation form are locally documented"
		DateWindow = $eraWindow
		Availability = "Ordinary or specialist"
		TradeStatus = "Local production or ordinary interregional trade"
	}

	switch ($Profile) {
		"Universal" {
			$decision.CultureScope = "Shared households and institutions using locally available ingredients"
			$decision.Availability = "Ordinary"
			$decision.TradeStatus = "Local production or ordinary interregional trade"
		}
		"RegionalOldWorld" {
			$decision.CultureScope = "Old World cultures where the ingredients and preparation are locally established"
			$decision.Availability = "Regional ordinary"
		}
		"European" {
			$decision.CultureScope = "European and directly connected food cultures using this form"
			$decision.Availability = "Regional ordinary or specialist"
		}
		"Islamicate" {
			$decision.CultureScope = "Islamicate, Persianate, North African, and connected food cultures"
			$decision.Availability = "Regional ordinary or specialist"
		}
		"SouthAsian" {
			$decision.CultureScope = "South Asian and directly connected Indian Ocean food cultures"
			$decision.Availability = "Regional ordinary or specialist"
		}
		"EastAsian" {
			$decision.CultureScope = "East Asian and directly connected Inner Asian or maritime food cultures"
			$decision.Availability = "Regional ordinary or specialist"
		}
		"SubSaharanAfrican" {
			$decision.CultureScope = "Sub-Saharan African and directly connected food cultures"
			$decision.Availability = "Regional ordinary or specialist"
		}
		"IndigenousAmerican" {
			$decision.CultureScope = "Indigenous American food cultures using this preparation and its ingredients"
			$decision.Availability = "Regional ordinary or specialist"
		}
		"Mesoamerican" {
			$decision.CultureScope = "Mesoamerican food cultures using this preparation and its ingredients"
			$decision.Availability = "Regional ordinary or specialist"
		}
		"Andean" {
			$decision.CultureScope = "Andean food cultures using this preparation and its ingredients"
			$decision.Availability = "Regional ordinary or specialist"
		}
		"MaritimeTrade" {
			$decision.CultureScope = "Documented maritime, riverine, caravan, port, and entrepot food cultures"
			$decision.Availability = "Trade-gated specialist"
			$decision.TradeStatus = "Local at production centres; imported or export-only elsewhere"
		}
		"SugarTrade" {
			$decision.CultureScope = switch ($Era) {
				"Medieval" { "Late Medieval Mediterranean, Islamicate, South Asian, and connected sugar cultures" }
				"Renaissance" { "Mediterranean, South Asian, Atlantic, and documented sugar-trade cultures" }
				"Early Modern" { "Mediterranean, South Asian, Atlantic, colonial, company, and documented global sugar routes" }
			}
			$decision.DateWindow = switch ($Era) {
				"Medieval" { "1200-1400 CE" }
				"Renaissance" { "1400-1600 CE" }
				"Early Modern" { "1600-1750 CE" }
			}
			$decision.Availability = "Restricted luxury or trade food"
			$decision.TradeStatus = "Local in producing cultures; imported or export-only elsewhere"
		}
		"TeaTrade" {
			$decision.CultureScope = switch ($Era) {
				"Medieval" { "East Asian and connected Inner Asian or maritime tea cultures" }
				"Renaissance" { "East Asian tea cultures and explicitly admitted late maritime imports" }
				"Early Modern" { "East Asian tea cultures and documented global maritime tea routes" }
			}
			$decision.DateWindow = switch ($Era) {
				"Medieval" { "700-1400 CE" }
				"Renaissance" { "1400-1600 CE; European maritime admission from about 1580" }
				"Early Modern" { "1600-1750 CE" }
			}
			$decision.Availability = "Restricted beverage or trade food"
			$decision.TradeStatus = "Local in producing cultures; imported or export-only elsewhere"
		}
		"CoffeeTrade" {
			switch ($Era) {
				"Medieval" {
					$decision.CultureScope = "No default Medieval admission"
					$decision.DateWindow = "Not before the fifteenth-century coffee trade"
					$decision.Availability = "Not admitted"
					$decision.TradeStatus = "Later-era commodity and service network"
				}
				"Renaissance" {
					$decision.CultureScope = "Red Sea, Arabian, Ottoman, and connected Islamicate coffee cultures; Europe only at the late edge"
					$decision.DateWindow = "1450-1600 CE"
					$decision.Availability = "Restricted beverage or trade food"
					$decision.TradeStatus = "Local in producing cultures; imported or export-only elsewhere"
				}
				"Early Modern" {
					$decision.CultureScope = "Red Sea, Ottoman, Indian Ocean, European, colonial, company, and documented global coffee routes"
					$decision.Availability = "Restricted beverage or trade food"
					$decision.TradeStatus = "Local in producing cultures; imported or export-only elsewhere"
				}
			}
		}
		"CacaoTrade" {
			$decision.CultureScope = switch ($Era) {
				"Medieval" { "Mesoamerican cacao-producing and consuming cultures only" }
				"Renaissance" { "Mesoamerican cacao cultures and documented Atlantic contact routes" }
				"Early Modern" { "Mesoamerican, Spanish Atlantic, colonial, and documented global cacao routes" }
			}
			$decision.DateWindow = switch ($Era) {
				"Medieval" { "500-1400 CE" }
				"Renaissance" { "1400-1600 CE; transatlantic admission after about 1520" }
				"Early Modern" { "1600-1750 CE" }
			}
			$decision.Availability = "Restricted beverage, luxury, or trade food"
			$decision.TradeStatus = "Local in producing cultures; imported or export-only elsewhere"
		}
		"NewWorldPostContact" {
			$decision.CultureScope = switch ($Era) {
				"Medieval" { "Indigenous American cultures where the ingredients and preparation are locally established" }
				"Renaissance" { "Indigenous American cultures and documented post-1492 Atlantic contact zones" }
				"Early Modern" { "Indigenous American, Atlantic, colonial, and documented global receiving cultures" }
			}
			$decision.DateWindow = switch ($Era) {
				"Medieval" { "500-1400 CE in the Americas only" }
				"Renaissance" { "1400-1600 CE; Old World admission only after documented contact" }
				"Early Modern" { "1600-1750 CE, with local adoption still required" }
			}
			$decision.Availability = "Culture/contact-gated"
			$decision.TradeStatus = "Local in producing cultures; adopted or imported elsewhere only after contact"
		}
		default {
			throw "Unsupported shared food admission profile '$Profile'."
		}
	}

	return [pscustomobject]$decision
}

function Get-AdmittingContext {
	param([string]$Family)

	switch ($Family) {
		{ $_ -in @("Grain", "Bread", "Porridge", "Noodle", "Dumpling") } {
			return "household cook, baker, miller, street vendor, inn, monastery, military kitchen, or grain-food craft"
		}
		{ $_ -in @("Pulse", "Vegetable", "Soup", "Stew") } {
			return "household cook, market kitchen, inn, religious institution, military kitchen, or vegetable-food craft"
		}
		{ $_ -in @("Meat", "Poultry", "Offal", "Fish", "Shellfish", "Preserved") } {
			return "household cook, butcher, fishmonger, smokehouse, salter, inn, ship, army, or preservation craft"
		}
		{ $_ -in @("Dairy", "Egg") } {
			return "household, dairy, herding camp, cheesemaker, market, inn, or dairy-food craft"
		}
		{ $_ -in @("Fruit", "Nut", "Sweet") } {
			return "household cook, fruiterer, baker, confectioner, market, court, religious institution, or sweet-making craft"
		}
		{ $_ -in @("Condiment", "Sauce", "Oil", "Vinegar", "Syrup") } {
			return "household cook, oil presser, vinegar maker, sauce maker, apothecary, market, inn, or specialist food craft"
		}
		{ $_ -in @("Broth", "DairyDrink", "FruitDrink", "GrainDrink", "FermentedDrink", "Wine", "Spirit", "Tea", "Coffee", "Chocolate") } {
			return "household, brewer, vintner, dairy, tavern, inn, market, court, religious institution, or appropriate beverage-service venue"
		}
		"Intermediate" {
			return "household, mill, bakery, dairy, butcher, preserving yard, market, warehouse, or matching production craft"
		}
		default {
			return "household, market, institution, shop, or matching food-production craft"
		}
	}
}

function Read-SharedFoodCatalogue {
	$records = [System.Collections.Generic.List[pscustomobject]]::new()
	foreach ($file in Get-ChildItem -LiteralPath $catalogueRoot -Recurse -Filter "*.food-items.tsv") {
		$rows = Import-Csv -LiteralPath $file.FullName -Delimiter "`t"
		foreach ($row in $rows | Where-Object Scope -eq "Shared") {
			$records.Add([pscustomobject]@{
				StableReference = $row.stable_reference
				Kind = $row.kind
				Family = $row.family
				AdmissionProfile = $row.admission_profile
				ComponentReality = if ($row.kind -eq "Prepared") {
					"Direct item with a stock-owned PreparedFood profile, authored taste, standard nutrition band, bites, quality scaling, and freshness timers"
				}
				else {
					"Discrete stackable intermediate; not directly edible unless a later prepared-food row or craft consumes it"
				}
			})
		}
	}

	foreach ($file in Get-ChildItem -LiteralPath $catalogueRoot -Recurse -Filter "*.food-liquids.tsv") {
		$rows = Import-Csv -LiteralPath $file.FullName -Delimiter "`t"
		foreach ($row in $rows | Where-Object Scope -eq "Shared") {
			$records.Add([pscustomobject]@{
				StableReference = $row.stable_reference
				Kind = "Liquid"
				Family = $row.family
				AdmissionProfile = $row.admission_profile
				ComponentReality = "Persistent Liquid row with authored display, taste, smell, alcohol, water, food-satiation, and drink-satiation values"
			})
		}
	}

	$duplicates = $records | Group-Object StableReference | Where-Object Count -gt 1
	if ($duplicates) {
		throw "Duplicate shared food stable references: $($duplicates.Name -join ', ')"
	}

	if ($records.Count -ne 2250) {
		throw "Expected 2,250 shared food admission records, found $($records.Count)."
	}

	return $records | Sort-Object StableReference
}

function ConvertTo-FoodAdmissionManifest {
	param(
		[string]$Era,
		[System.Collections.IEnumerable]$Records
	)

	$lines = [System.Collections.Generic.List[string]]::new()
	$lines.Add("# FutureMUD $Era Shared Food Admission Manifest")
	$lines.Add("")
	$lines.Add("**Status:** complete populated admission registry.")
	$lines.Add("")
	$lines.Add("## Purpose")
	$lines.Add("")
	$lines.Add("This manifest governs culture, contact, date, institution, shop, and craft admission for every shared food catalogue record. It does not clone dishes merely because more than one culture or era uses the same food form.")
	$lines.Add("")
	$lines.Add("The contract contains **2,250 shared records**: **2,100 prepared or intermediate item prototypes** and **150 food liquids**.")
	$lines.Add("")
	$lines.Add("## Populated admission records")
	$lines.Add("")
	$lines.Add("<!-- food-admission-records:start -->")
	$lines.Add("| Stable reference | Kind | Family | Admission profile | Culture/contact scope | Date window | Admitting context | Availability | Production/trade status | Component reality |")
	$lines.Add("| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |")
	foreach ($record in $Records) {
		$decision = Get-AdmissionDecision $Era $record.AdmissionProfile
		$context = Get-AdmittingContext $record.Family
		$lines.Add("| ``$($record.StableReference)`` | $($record.Kind) | $($record.Family) | $($record.AdmissionProfile) | $($decision.CultureScope) | $($decision.DateWindow) | $context | $($decision.Availability) | $($decision.TradeStatus) | $($record.ComponentReality) |")
	}
	$lines.Add("<!-- food-admission-records:end -->")
	$lines.Add("")
	$lines.Add("## Acceptance")
	$lines.Add("")
	$lines.Add("- The record set exactly matches the 2,250-row shared source catalogue.")
	$lines.Add("- Shared food forms are not duplicated solely for national or cultural labels.")
	$lines.Add("- Ingredient-level crop and contact restrictions remain applicable even when the base dish form is broadly shared.")
	$lines.Add("- `Not admitted` is a completed historical decision, not an unfinished row.")
	$lines.Add("- Admission never adds mechanics beyond the PreparedFood, stack, or Liquid records identified above.")

	return $lines -join [Environment]::NewLine
}

$records = Read-SharedFoodCatalogue
$targets = @(
	@{ Era = "Medieval"; File = "FutureMUD_Medieval_Shared_Food_Admission_Manifest.md" },
	@{ Era = "Renaissance"; File = "FutureMUD_Renaissance_Shared_Food_Admission_Manifest.md" },
	@{ Era = "Early Modern"; File = "FutureMUD_EarlyModern_Shared_Food_Admission_Manifest.md" }
)

$differences = [System.Collections.Generic.List[string]]::new()
foreach ($target in $targets) {
	$path = Join-Path $seedingRoot $target.File
	$content = ConvertTo-FoodAdmissionManifest $target.Era $records
	if ($Check) {
		if (-not (Test-Path $path) -or (Get-Content -Raw -LiteralPath $path).TrimEnd() -ne $content.TrimEnd()) {
			$differences.Add($target.File)
		}
		continue
	}

	[System.IO.File]::WriteAllText(
		$path,
		$content + [Environment]::NewLine,
		[System.Text.UTF8Encoding]::new($false))
}

if ($Check -and $differences.Count -gt 0) {
	throw "Shared food admission manifests are out of date: $($differences -join ', ')"
}

if ($Check) {
	Write-Output "Shared food admission manifests are current: 3 files, 2,250 rows each."
}
else {
	Write-Output "Generated 3 shared food admission manifests with 2,250 rows each."
}
