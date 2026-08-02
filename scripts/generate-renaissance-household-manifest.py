#!/usr/bin/env python3
"""Generate the Renaissance household item manifest from its canonical catalogues."""

from __future__ import annotations

import argparse
import json
import re
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
BASE = ROOT / "Design Documents/Seeding/FutureMUD_Renaissance_Household_Urban_Trade_Design_Reference.md"
VOLUME_INDICES = (
	ROOT / "Design Documents/Seeding/FutureMUD_Renaissance_Household_Furniture_Expansion_Catalogue_I.md",
	ROOT / "Design Documents/Seeding/FutureMUD_Renaissance_Household_Furniture_Expansion_Catalogue_II.md",
)
EXPANSION_SOURCES = (
	*sorted((ROOT / "Design Documents/Seeding").glob("FutureMUD_Renaissance_Household_Furniture_Expansion_Catalogue_I_*.md")),
	*sorted((ROOT / "Design Documents/Seeding").glob("FutureMUD_Renaissance_Household_Furniture_Expansion_Catalogue_II_*.md")),
	*sorted((ROOT / "Design Documents/Seeding").glob("FutureMUD_Renaissance_Container_Service_Expansion_Catalogue_*.md")),
)
OUTPUT = ROOT / "DatabaseSeeder/Seeders/ItemSeeder.Renaissance.HouseholdUrbanTradeManifestData.Generated.cs"

SIZE = {"T": "Tiny", "VS": "VerySmall", "S": "Small", "N": "Normal", "L": "Large", "VL": "VeryLarge", "H": "Huge", "EN": "Enormous"}
QUALITY = {"P": "Poor", "SS": "Substandard", "S": "Standard", "G": "Good", "VG": "VeryGood", "GR": "Great", "E": "Excellent"}
CATEGORY = {"T": "trade", "F": "furniture", "P": "personal", "D": "domestic"}
TAGS = {
	"TR-S": ("Functions / Container", "Market / Household Goods / Simple Wares"),
	"TR-N": ("Functions / Container", "Market / Household Goods / Standard Wares"),
	"TR-L": ("Functions / Container", "Market / Household Goods / Luxury Wares"),
	"FU-S": ("Functions / Container", "Functions / Household Items / Household Furniture", "Market / Household Goods / Simple Furniture"),
	"FU-N": ("Functions / Container", "Functions / Household Items / Household Furniture", "Market / Household Goods / Standard Furniture"),
	"FU-L": ("Functions / Container", "Functions / Household Items / Household Furniture", "Market / Household Goods / Luxury Furniture"),
	"PC-S": ("Functions / Container", "Market / Household Goods / Simple Wares"),
	"PC-N": ("Functions / Container", "Market / Household Goods / Standard Wares"),
	"PC-L": ("Functions / Container", "Market / Household Goods / Luxury Wares"),
	"DW-S": ("Functions / Container", "Functions / Household Items / Household Wares", "Market / Household Goods / Simple Wares"),
	"DW-N": ("Functions / Container", "Functions / Household Items / Household Wares", "Market / Household Goods / Standard Wares"),
	"DW-L": ("Functions / Container", "Functions / Household Items / Household Wares", "Market / Household Goods / Luxury Wares"),
	"OPEN": ("Functions / Container / Open Container",),
	"WATER": ("Functions / Container / Watertight Container",),
	"POROUS": ("Functions / Container / Porous Container",),
	"MIL": ("Market / Military Goods",),
	"COURT": ("Institution / Court",),
	"RELIGIOUS": ("Institution / Religious",),
	"GUILD": ("Institution / Guild",),
	"MARITIME": ("Institution / Maritime",),
	"PERFORMANCE": ("Institution / Performance",),
	"SERVICE": ("Institution / Service Household",),
}
CULTURES = {
	"WER": ("Culture / Renaissance / Shared / Western European Renaissance", "1450-1600"), "IBA": ("Culture / Renaissance / Shared / Iberian Atlantic", "1450-1600"),
	"CEN": ("Culture / Renaissance / Shared / Central European", "1450-1600"), "NBA": ("Culture / Renaissance / Shared / Northern Baltic", "1450-1600"),
	"CEF": ("Culture / Renaissance / Shared / Central Eastern Frontier", "1500-1600"), "EON": ("Culture / Renaissance / Shared / Eastern Orthodox Northern", "1500-1600"),
	"OTT": ("Culture / Renaissance / Shared / Ottoman Islamicate", "1450-1600"), "PIP": ("Culture / Renaissance / Shared / Persianate Indo-Persian", "1450-1600"),
	"SAS": ("Culture / Renaissance / Shared / South Asian", "1450-1600"), "EAL": ("Culture / Renaissance / Shared / East Asian Literati", "1400-1600"),
	"JPN": ("Culture / Renaissance / Shared / Japanese", "1450-1600"), "MEA": ("Culture / Renaissance / Shared / Maritime East Asian", "1450-1600"),
	"SEM": ("Culture / Renaissance / Shared / South-east Asian Mainland", "1450-1600"), "MSEA": ("Culture / Renaissance / Shared / Maritime South-east Asian", "1450-1600"),
	"STP": ("Culture / Renaissance / Shared / Steppe and Caravan", "1450-1600"), "ACA": ("Culture / Renaissance / Shared / African Court Atlantic", "1450-1600"),
	"SAI": ("Culture / Renaissance / Shared / Sahelian Islamic", "1450-1600"), "RSE": ("Culture / Renaissance / Shared / Red Sea", "1450-1600"),
	"IND": ("Culture / Renaissance / Shared / Indian Ocean", "1450-1600"), "MES": ("Culture / Renaissance / Shared / Mesoamerican", "1400-1600"),
	"AND": ("Culture / Renaissance / Shared / Andean", "1400-1600"), "CAR": ("Culture / Renaissance / Shared / Caribbean Contact", "1450-1600"),
	"NAC": ("Culture / Renaissance / Shared / North American Contact", "1450-1600"), "COL": ("Culture / Renaissance / Shared / Colonial Atlantic", "1500-1600"),
	"MAR": ("Culture / Renaissance / Shared / Global Maritime", "1450-1600"),
}


def lines(path: Path) -> list[str]:
	return path.read_text(encoding="utf-8-sig").splitlines()


def tick_values(value: str) -> tuple[str, ...]:
	return tuple(re.findall(r"`([^`]+)`", value))


def base_components() -> dict[str, tuple[str, ...]]:
	result: dict[str, tuple[str, ...]] = {}
	for line in lines(BASE):
		match = re.match(r"^(CP\d+)=(.+)$", line)
		if match:
			result[match.group(1)] = tuple(match.group(2).split(";"))
	if len(result) != 88:
		raise ValueError(f"Expected 88 base component profiles, found {len(result)}")
	return result


def expansion_components() -> dict[str, tuple[str, ...]]:
	result: dict[str, tuple[str, ...]] = {}
	for index in VOLUME_INDICES:
		for line in lines(index):
			match = re.match(r"^\| `?(FX\d+)`? \| (.+) \|", line)
			if match:
				result[match.group(1)] = tick_values(match.group(2))
	if len(result) != 39:
		raise ValueError(f"Expected 39 expansion component profiles, found {len(result)}")
	return result


def package_data() -> dict[str, tuple[str, str]]:
	result: dict[str, tuple[str, str]] = {}
	for line in lines(BASE):
		if not line.startswith("| P"):
			continue
		cells = [cell.strip() for cell in line.strip().strip("|").split("|")]
		match = re.match(r"P(\d+)", cells[0])
		if match and len(cells) >= 5:
			result[f"P{match.group(1)}"] = (tick_values(cells[1])[0], tick_values(cells[2])[0])
	if len(result) != 20:
		raise ValueError(f"Expected 20 base packages, found {len(result)}")
	return result


def noun(sdesc: str) -> str:
	words = re.sub(r"^(?:a|an|the|some|a pair of) ", "", sdesc, flags=re.IGNORECASE).split()
	return re.sub(r"[^a-zA-Z-]", "", words[-1]) if words else "item"


def tags(culture: str, tag_codes: str) -> tuple[str, ...]:
	result = ["Era / Renaissance Era", CULTURES[culture][0]]
	for code in tag_codes.split(","):
		result.extend(TAGS[code])
	return tuple(dict.fromkeys(result))


def description(sdesc: str, material: str) -> str:
	return f"{sdesc.capitalize()} is made chiefly from {material}. Its construction and fittings follow the documented Renaissance household form shown by its outward appearance."


def stable_reference(category: str, prefix: str, slug: str) -> str:
	"""Build a product-facing identifier without repeating a package path segment."""
	prefix_parts = prefix.split("_")
	slug_parts = slug.split("_")
	if prefix_parts and slug_parts and prefix_parts[-1].casefold() == slug_parts[0].casefold():
		slug_parts = slug_parts[1:]
	return f"renaissance_{CATEGORY[category]}_{prefix}_{'_'.join(slug_parts)}"


def parse_base(cp: dict[str, tuple[str, ...]]) -> list[tuple]:
	packages = package_data()
	current = ""
	result = []
	for line in lines(BASE):
		match = re.match(r"^### P(\d+)", line)
		if match:
			current = f"P{match.group(1)}"
			continue
		if not re.match(r"^[TFPD]\|", line):
			continue
		cells = line.split("|")
		if len(cells) != 8 or not current:
			raise ValueError(f"Malformed base household row: {line}")
		category, slug, sdesc, material, size_quality, weight_cost, component_code, tag_codes = cells
		prefix, culture_tag = packages[current]
		size, quality = size_quality.split("/")
		weight, cost = weight_cost.split("/")
		culture_code = next(code for code, (tag, _) in CULTURES.items() if tag == culture_tag)
		result.append((stable_reference(category, prefix, slug), noun(sdesc), sdesc, description(sdesc, material), SIZE[size], QUALITY[quality], weight, cost, material, tags(culture_code, tag_codes), cp[component_code], culture_code))
	if len(result) != 400:
		raise ValueError(f"Expected 400 base household rows, found {len(result)}")
	return result


def parse_expansion(fx: dict[str, tuple[str, ...]]) -> list[tuple]:
	result = []
	for source in EXPANSION_SOURCES:
		for line in lines(source):
			if not line.startswith("renaissance_"):
				continue
			cells = line.split("|")
			if len(cells) != 9:
				raise ValueError(f"Malformed expansion household row: {line}")
			stable, sdesc, material, size_quality, weight_cost, component_data, culture, tag_codes, _ = cells
			size, quality = size_quality.split("/")
			weight, cost = weight_cost.split("/")
			components = fx[component_data] if component_data.startswith("FX") else tuple(component_data.split(";"))
			result.append((stable, noun(sdesc), sdesc, description(sdesc, material), SIZE[size], QUALITY[quality], weight, cost, material, tags(culture, tag_codes), components, culture))
	if len(result) != 600:
		raise ValueError(f"Expected 600 expansion household rows, found {len(result)}")
	return result


def cs(value: str) -> str:
	return json.dumps(value, ensure_ascii=False)


def render_row(row: tuple) -> str:
	stable, item_noun, sdesc, fdesc, size, quality, weight, cost, material, item_tags, components, culture = row
	item_tags_source = ", ".join(cs(value) for value in item_tags)
	components_source = ", ".join(cs(value) for value in components)
	return f'\t\tnew({cs(stable)}, {cs(item_noun)}, {cs(sdesc)}, {cs(fdesc)}, SizeCategory.{size}, ItemQuality.{quality}, {weight}.0, {cost}.0m, {cs(material)}, [{item_tags_source}], [{components_source}], {cs(f"Renaissance household catalogue; culture admission {culture} ({CULTURES[culture][1]}).")}),'


def generate() -> str:
	items = parse_base(base_components()) + parse_expansion(expansion_components())
	if len(items) != 1000 or len({item[0] for item in items}) != 1000:
		raise ValueError("Renaissance household catalogue must contain 1,000 unique stable references")
	if len({item[2] for item in items}) != 1000:
		raise ValueError("Renaissance household catalogue short descriptions must be unique")
	items = [(item[0], item[1], item[2], item[3], item[4], item[5], item[6], item[7], "mother-of-pearl" if item[8] == "mother of pearl" else item[8], item[9], item[10], item[11]) for item in items]
	return "\n".join([
		"// <auto-generated>",
		"// Generated by scripts/generate-renaissance-household-manifest.py from the canonical Renaissance household catalogues.",
		"// Do not edit this file by hand.",
		"// </auto-generated>",
		"#nullable enable",
		"",
		"using MudSharp.GameItems;",
		"",
		"namespace DatabaseSeeder.Seeders;",
		"",
		"public partial class ItemSeeder",
		"{",
		"\tprivate static readonly RenaissanceHouseholdItemSpec[] RenaissanceHouseholdItemSpecs =",
		"\t[",
		*(render_row(item) for item in items),
		"\t];",
		"}",
		"",
	])


def main() -> int:
	parser = argparse.ArgumentParser()
	parser.add_argument("--check", action="store_true")
	args = parser.parse_args()
	content = generate()
	if args.check:
		if not OUTPUT.exists() or OUTPUT.read_text(encoding="utf-8") != content:
			print(f"Generated household manifest source is stale: {OUTPUT.relative_to(ROOT)}")
			return 1
		return 0
	OUTPUT.write_text(content, encoding="utf-8", newline="\n")
	print(f"Wrote {OUTPUT.relative_to(ROOT)}")
	return 0


if __name__ == "__main__":
	raise SystemExit(main())
