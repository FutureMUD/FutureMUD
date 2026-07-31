param(
	[switch]$Check
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$catalogueRoot = Join-Path $repoRoot "DatabaseSeeder\Seeders\FoodCatalogue"
$componentPath = Join-Path $repoRoot "Design Documents\Data\Seeded_Item_Components.json"
$liquidPath = Join-Path $repoRoot "Design Documents\Data\Seeded_Liquids.json"
$tagPath = Join-Path $repoRoot "Design Documents\Data\SeededTagHierarchy.csv"
$utf8 = [System.Text.UTF8Encoding]::new($false)

function Read-CatalogueRows {
	param([string]$Pattern)

	$rows = [System.Collections.Generic.List[pscustomobject]]::new()
	foreach ($file in Get-ChildItem -LiteralPath $catalogueRoot -Recurse -Filter $Pattern | Sort-Object FullName) {
		foreach ($row in Import-Csv -LiteralPath $file.FullName -Delimiter "`t") {
			$rows.Add($row)
		}
	}

	return $rows
}

function ConvertTo-StableJson {
	param([object[]]$Value)

	return ($Value | ConvertTo-Json -Depth 10) + [Environment]::NewLine
}

function Add-TagPath {
	param(
		[System.Collections.Generic.Dictionary[string, pscustomobject]]$Rows,
		[string]$Path
	)

	$parts = $Path -split " / "
	for ($index = 0; $index -lt $parts.Count; $index++) {
		$hierarchy = $parts[0..$index] -join " / "
		if ($Rows.ContainsKey($hierarchy)) {
			continue
		}

		$Rows[$hierarchy] = [pscustomobject][ordered]@{
			"Tag name" = $parts[$index]
			"Tag parent name" = if ($index -eq 0) { "" } else { $parts[$index - 1] }
			"Tag hierarchy" = $hierarchy
		}
	}
}

function Assert-OrWrite {
	param(
		[string]$Path,
		[string]$Content,
		[System.Collections.Generic.List[string]]$Differences
	)

	if ($Check) {
		$isCurrent = (Test-Path -LiteralPath $Path) -and
			[string]::Equals(
				[System.IO.File]::ReadAllText($Path).TrimEnd(),
				$Content.TrimEnd(),
				[System.StringComparison]::Ordinal)
		if (-not $isCurrent) {
			$Differences.Add((Split-Path -Leaf $Path))
		}
		return
	}

	[System.IO.File]::WriteAllText($Path, $Content, $utf8)
}

$items = Read-CatalogueRows "*.food-items.tsv"
$liquids = Read-CatalogueRows "*.food-liquids.tsv"
if ($items.Count -ne 2775) {
	throw "Expected 2,775 food item catalogue rows, found $($items.Count)."
}
if ($liquids.Count -ne 225) {
	throw "Expected 225 food liquid catalogue rows, found $($liquids.Count)."
}

$differences = [System.Collections.Generic.List[string]]::new()

$components = [System.Collections.Generic.List[object]]::new()
$existingComponents = Get-Content -Raw -LiteralPath $componentPath | ConvertFrom-Json
foreach ($component in $existingComponents) {
	if (-not $component."Component Name".StartsWith(
		"PreparedFood_Catalogue_",
		[System.StringComparison]::OrdinalIgnoreCase)) {
		$components.Add($component)
	}
}
foreach ($item in $items | Where-Object kind -eq "Prepared") {
	$components.Add([pscustomobject][ordered]@{
		"Component Name" = "PreparedFood_Catalogue_$($item.stable_reference)"
		"Component Description" = "Stock prepared-food profile for $($item.stable_reference)."
		"Component Type" = "PreparedFood"
	})
}
$componentContent = ConvertTo-StableJson @($components | Sort-Object "Component Name")
Assert-OrWrite $componentPath $componentContent $differences

$liquidByName = [System.Collections.Generic.Dictionary[string, object]]::new(
	[System.StringComparer]::OrdinalIgnoreCase)
$existingLiquids = Get-Content -Raw -LiteralPath $liquidPath | ConvertFrom-Json
foreach ($liquid in $existingLiquids) {
	$isCatalogueLiquid = @($liquid.Tags) |
		Where-Object { $_ -like "Food and Drink / Food Liquids / Pre-Industrial Catalogue*" } |
		Select-Object -First 1
	if (-not $isCatalogueLiquid) {
		$liquidByName[$liquid."Liquid Name"] = $liquid
	}
}
foreach ($liquid in $liquids) {
	$scopeName = if ($liquid.scope -eq "EarlyModern") { "Early Modern" } else { $liquid.scope }
	$familyName = [regex]::Replace($liquid.family, "([a-z])([A-Z])", '$1 $2')
	$liquidByName[$liquid.name] = [pscustomobject][ordered]@{
		"Liquid Name" = $liquid.name
		"Tags" = @(
			"Food and Drink / Food Liquids / Pre-Industrial Catalogue / Scope / $scopeName"
			"Food and Drink / Food Liquids / Pre-Industrial Catalogue / Family / $familyName"
		)
	}
}
$liquidContent = ConvertTo-StableJson @($liquidByName.Values | Sort-Object "Liquid Name")
Assert-OrWrite $liquidPath $liquidContent $differences

$tagRows = [System.Collections.Generic.Dictionary[string, pscustomobject]]::new(
	[System.StringComparer]::OrdinalIgnoreCase)
foreach ($row in Import-Csv -LiteralPath $tagPath -Delimiter "`t") {
	if ($row."Tag hierarchy" -notlike "Food and Drink / Prepared Foods / Pre-Industrial Catalogue*" -and
		$row."Tag hierarchy" -notlike "Food and Drink / Food Liquids / Pre-Industrial Catalogue*" -and
		$row."Tag hierarchy" -notlike "Materials / Food Products / Pre-Industrial Food Commodities*") {
		$tagRows[$row."Tag hierarchy"] = $row
	}
}

$preparedRoot = "Food and Drink / Prepared Foods / Pre-Industrial Catalogue"
$liquidRoot = "Food and Drink / Food Liquids / Pre-Industrial Catalogue"
$intermediateRoot = "Materials / Food Products / Pre-Industrial Food Commodities"
$intermediateCommodityTags = @(
	"Bran Commodity",
	"Cleaned Grain Commodity",
	"Dough Commodity",
	"Dried Fish Commodity",
	"Dried Meat Commodity",
	"Flour Commodity",
	"Fruit Must Commodity",
	"Grain Cleaning Stock",
	"Malted Grain Commodity",
	"Meal Commodity",
	"Oilseed Cake Commodity",
	"Oilseed Mash Commodity",
	"Raw Fish Commodity",
	"Raw Meat Commodity",
	"Salted Fish Commodity",
	"Salted Meat Commodity",
	"Smoked Fish Commodity",
	"Smoked Meat Commodity",
	"Wort Commodity"
)
foreach ($scope in @("Shared", "Medieval", "Renaissance", "Early Modern")) {
	Add-TagPath $tagRows "$preparedRoot / Scope / $scope"
	Add-TagPath $tagRows "$liquidRoot / Scope / $scope"
	Add-TagPath $tagRows "$intermediateRoot / Scope / $scope"
}
foreach ($family in @($items.family + $liquids.family | Sort-Object -Unique)) {
	$familyName = [regex]::Replace($family, "([a-z])([A-Z])", '$1 $2')
	Add-TagPath $tagRows "$preparedRoot / Family / $familyName"
	Add-TagPath $tagRows "$liquidRoot / Family / $familyName"
	Add-TagPath $tagRows "$intermediateRoot / Family / $familyName"
}
foreach ($commodityTag in $intermediateCommodityTags) {
	Add-TagPath $tagRows "$intermediateRoot / $commodityTag"
}
foreach ($register in @("Bleak", "Ordinary", "Rich")) {
	Add-TagPath $tagRows "$preparedRoot / Social Register / $register"
}

$tagLines = [System.Collections.Generic.List[string]]::new()
$tagLines.Add("Tag name`tTag parent name`tTag hierarchy")
foreach ($row in $tagRows.Values | Sort-Object "Tag hierarchy") {
	$tagLines.Add("$($row.'Tag name')`t$($row.'Tag parent name')`t$($row.'Tag hierarchy')")
}
$tagContent = ($tagLines -join [Environment]::NewLine) + [Environment]::NewLine
Assert-OrWrite $tagPath $tagContent $differences

if ($Check -and $differences.Count -gt 0) {
	throw "Pre-industrial food catalogue exports are out of date: $($differences -join ', ')"
}

if ($Check) {
	Write-Output "Pre-industrial food catalogue exports are current."
}
else {
	Write-Output "Synchronized prepared-food components, food liquids, and tag hierarchy exports."
}
