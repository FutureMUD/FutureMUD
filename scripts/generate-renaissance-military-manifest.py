#!/usr/bin/env python3
"""Generate the Renaissance military item manifest from its canonical design table.

Descriptions are deliberately copied verbatim. This generator is a catalogue
validator, not a prose generator: authored item descriptions must never be
replaced by a generic template.
"""

from __future__ import annotations

import argparse
import csv
import json
import re
from collections import Counter
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "Design Documents/Seeding/FutureMUD_Renaissance_Military_Firearms_Armour_Design_Reference.md"
MATERIALS = ROOT / "Design Documents/Data/Seeded_Materials.json"
COMPONENTS = ROOT / "Design Documents/Data/Seeded_Item_Components.json"
TAGS = ROOT / "Design Documents/Data/SeededTagHierarchy.csv"
OUTPUT = ROOT / "DatabaseSeeder/Seeders/ItemSeeder.Renaissance.MilitaryManifestData.Generated.cs"

START = "<!-- RENAISSANCE MILITARY CATALOGUE START -->"
END = "<!-- RENAISSANCE MILITARY CATALOGUE END -->"
ARMOUR_OUTFIT_START = "<!-- RENAISSANCE ARMOUR OUTFIT MANIFEST START -->"
ARMOUR_OUTFIT_END = "<!-- RENAISSANCE ARMOUR OUTFIT MANIFEST END -->"
CATEGORIES = (
	"Melee weapons",
	"Ranged weapons",
	"Firearms & ammunition",
	"Artillery",
	"Armour & barding",
	"Shields",
	"Military support & field gear",
)
REFERENCE = re.compile(r"^renaissance_military_[a-z0-9_]+$")
OUTFIT_REFERENCE = re.compile(r"^renaissance_military_(?:armour|mount)_outfit_[a-z0-9_]+$")
ARTICLE_SDESC = re.compile(r"^(?:a|an) [a-z0-9][a-z0-9'\- ]+$", re.IGNORECASE)


def read_lines(path: Path) -> list[str]:
	return path.read_text(encoding="utf-8-sig").splitlines()


def table_rows() -> list[list[str]]:
	lines = read_lines(SOURCE)
	try:
		start = lines.index(START) + 1
		end = lines.index(END)
	except ValueError as error:
		raise ValueError("Renaissance military catalogue markers are missing") from error

	rows: list[list[str]] = []
	for line in lines[start:end]:
		if not line.startswith("| renaissance_military_"):
			continue
		cells = [cell.strip() for cell in line.strip().strip("|").split("|")]
		if len(cells) != 14:
			raise ValueError(f"Expected 14 cells in Renaissance military row: {line}")
		rows.append(cells)
	if not rows:
		raise ValueError("Renaissance military catalogue has no rows")
	return rows


def armour_outfit_rows() -> list[list[str]]:
	lines = read_lines(SOURCE)
	try:
		start = lines.index(ARMOUR_OUTFIT_START) + 1
		end = lines.index(ARMOUR_OUTFIT_END)
	except ValueError as error:
		raise ValueError("Renaissance armour outfit manifest markers are missing") from error

	rows: list[list[str]] = []
	for line in lines[start:end]:
		if not line.startswith("| renaissance_military_"):
			continue
		cells = [cell.strip() for cell in line.strip().strip("|").split("|")]
		if len(cells) != 5:
			raise ValueError(f"Expected five cells in Renaissance armour outfit row: {line}")
		rows.append(cells)
	if not rows:
		raise ValueError("Renaissance armour outfit manifest has no rows")
	return rows


def material_names() -> set[str]:
	return {
		entry["Material Name"]
		for entry in json.loads(MATERIALS.read_text(encoding="utf-8"))
	}


def component_names() -> set[str]:
	return {
		entry["Component Name"]
		for entry in json.loads(COMPONENTS.read_text(encoding="utf-8"))
	}


def tag_paths() -> set[str]:
	with TAGS.open(encoding="utf-8-sig", newline="") as source:
		return {
			row[2]
			for row in csv.reader(source, delimiter="\t")
			if len(row) >= 3 and row[2]
		}


def split_values(value: str) -> tuple[str, ...]:
	return tuple(part.strip() for part in value.split(";") if part.strip())


def sentence_count(description: str) -> int:
	return len(re.findall(r"[.!?](?:\s|$)", description))


def documented_counts() -> dict[str, int]:
	text = SOURCE.read_text(encoding="utf-8-sig")
	counts: dict[str, int] = {}
	for category in CATEGORIES:
		match = re.search(rf"^\| {re.escape(category)} \| (\d+) \|$", text, re.MULTILINE)
		if not match:
			raise ValueError(f"Catalogue totals table is missing {category}")
		counts[category] = int(match.group(1))
	total_match = re.search(r"^\| \*\*Total\*\* \| \*\*(\d+)\*\* \|$", text, re.MULTILINE)
	if not total_match:
		raise ValueError("Catalogue totals table is missing its total")
	counts["Total"] = int(total_match.group(1))
	return counts


def validate(rows: list[list[str]]) -> None:
	materials = material_names()
	components = component_names()
	tags = tag_paths()
	references: set[str] = set()
	descriptions: set[str] = set()
	for row in rows:
		(reference, category, admission, noun, sdesc, description, size, quality,
		 weight, cost, material, row_tags, row_components, skins) = row
		if not REFERENCE.fullmatch(reference):
			raise ValueError(f"Invalid stable reference: {reference}")
		if reference in references:
			raise ValueError(f"Duplicate stable reference: {reference}")
		references.add(reference)
		if category not in CATEGORIES:
			raise ValueError(f"Unsupported category for {reference}: {category}")
		if not admission or not noun:
			raise ValueError(f"Admission and noun are required for {reference}")
		if not ARTICLE_SDESC.fullmatch(sdesc) or not 3 <= len(sdesc.split()) <= 6:
			raise ValueError(f"SDesc must be an article-led three-to-six word product name: {reference}")
		if sentence_count(description) != 3:
			raise ValueError(f"Full description must contain exactly three sentences: {reference}")
		if description in descriptions:
			raise ValueError(f"Full descriptions must be unique: {reference}")
		descriptions.add(description)
		if any(forbidden in description.lower() for forbidden in ("seed", "builder", "component", "documented form")):
			raise ValueError(f"Full description contains implementation language: {reference}")
		if size not in {"Tiny", "Small", "Normal", "Large", "VeryLarge"}:
			raise ValueError(f"Unknown SizeCategory for {reference}: {size}")
		if quality not in {"Standard", "Good", "VeryGood"}:
			raise ValueError(f"Unknown ItemQuality for {reference}: {quality}")
		if not weight.isdigit() or not re.fullmatch(r"\d+(?:\.\d+)?", cost):
			raise ValueError(f"Weight and cost must be numeric for {reference}")
		if material not in materials:
			raise ValueError(f"Missing maintained material for {reference}: {material}")
		missing_tags = set(split_values(row_tags)) - tags
		if missing_tags:
			raise ValueError(f"Missing maintained tags for {reference}: {', '.join(sorted(missing_tags))}")
		component_values = split_values(row_components)
		missing_components = set(component_values) - components
		if missing_components:
			raise ValueError(f"Missing maintained components for {reference}: {', '.join(sorted(missing_components))}")
		if skins not in {"yes", "no"}:
			raise ValueError(f"Skins must be yes or no for {reference}")
		if category in {"Melee weapons", "Ranged weapons"} and not any(
			component.startswith(("Melee_", "Ammo_", "Longbow", "CompositeBow", "Yumi", "Crossbow", "Sling"))
			for component in component_values):
			raise ValueError(f"Weapon composition invariant failed for {reference}")
		if category == "Firearms & ammunition" and not any(
			component.startswith(("Musket_", "Pistol_", "MusketBall_", "MusketPaper"))
			for component in component_values):
			raise ValueError(f"Firearm composition invariant failed for {reference}")
		if category == "Artillery" and not any(component.startswith("Artillery") for component in component_values):
			raise ValueError(f"Artillery composition invariant failed for {reference}")
		if category == "Armour & barding" and not any(component.startswith("Armour_") for component in component_values):
			raise ValueError(f"Armour composition invariant failed for {reference}")
		if category == "Shields" and not any(component.startswith("Shield_") for component in component_values):
			raise ValueError(f"Shield composition invariant failed for {reference}")

	counts = Counter(row[1] for row in rows)
	for category in CATEGORIES:
		if not counts[category]:
			raise ValueError(f"Catalogue has no rows for {category}")
	declared = documented_counts()
	for category in CATEGORIES:
		if declared[category] != counts[category]:
			raise ValueError(
				f"Catalogue total for {category} is {declared[category]}, but the source table contains {counts[category]}")
	if declared["Total"] != len(rows):
		raise ValueError(
			f"Catalogue total is {declared['Total']}, but the source table contains {len(rows)}")


def validate_armour_outfits(outfits: list[list[str]], rows: list[list[str]]) -> None:
	items = {row[0]: row for row in rows}
	keys: set[str] = set()
	names: set[str] = set()
	kinds = Counter()
	for key, name, description, kind, references in outfits:
		if not OUTFIT_REFERENCE.fullmatch(key):
			raise ValueError(f"Invalid Renaissance armour outfit key: {key}")
		if key in keys:
			raise ValueError(f"Duplicate Renaissance armour outfit key: {key}")
		keys.add(key)
		if not name or len(name) > 200:
			raise ValueError(f"Renaissance armour outfit name is missing or exceeds 200 characters: {key}")
		if name.casefold() in names:
			raise ValueError(f"Duplicate Renaissance armour outfit name: {name}")
		names.add(name.casefold())
		if not description:
			raise ValueError(f"Renaissance armour outfit description is required: {key}")
		if kind not in {"Personal", "Mount"}:
			raise ValueError(f"Unsupported Renaissance armour outfit type for {key}: {kind}")
		if kind == "Personal" and not key.startswith("renaissance_military_armour_outfit_"):
			raise ValueError(f"Personal Renaissance armour outfit has the wrong key prefix: {key}")
		if kind == "Mount" and not key.startswith("renaissance_military_mount_outfit_"):
			raise ValueError(f"Mount Renaissance armour outfit has the wrong key prefix: {key}")
		kinds[kind] += 1
		references = split_values(references)
		if not references:
			raise ValueError(f"Renaissance armour outfit has no item references: {key}")
		if len(set(references)) != len(references):
			raise ValueError(f"Renaissance armour outfit repeats item references: {key}")
		missing = set(references) - items.keys()
		if missing:
			raise ValueError(f"Renaissance armour outfit references missing military items for {key}: {', '.join(sorted(missing))}")
		wear_components: list[str] = []
		for reference in references:
			item = items[reference]
			if item[1] != "Armour & barding":
				raise ValueError(f"Renaissance armour outfit references a non-armour item for {key}: {reference}")
			profiles = [component for component in split_values(item[12]) if component.startswith("Wear_")]
			if not profiles:
				raise ValueError(f"Renaissance armour outfit item has no wearable profile for {key}: {reference}")
			wear_components.extend(profiles)
		duplicates = sorted({component for component in wear_components if wear_components.count(component) > 1})
		if duplicates:
			raise ValueError(f"Renaissance armour outfit repeats wearable profiles for {key}: {', '.join(duplicates)}")
	if len(outfits) != 62 or kinds != Counter({"Personal": 56, "Mount": 6}):
		raise ValueError(
			"Expected exactly 56 personal and 6 mount Renaissance armour outfits; "
			f"found Personal={kinds['Personal']}, Mount={kinds['Mount']}")


def cs(value: str) -> str:
	return json.dumps(value, ensure_ascii=False)


def render(row: list[str]) -> str:
	(reference, category, admission, noun, sdesc, description, size, quality,
	 weight, cost, material, row_tags, row_components, skins) = row
	tags = ", ".join(cs(tag) for tag in split_values(row_tags))
	components = ", ".join(cs(component) for component in split_values(row_components))
	builder_notes = (
		f"Renaissance military catalogue; category {category}; admission {admission}; "
		f"component profile {', '.join(split_values(row_components))}."
	)
	return (
		f"\t\tnew({cs(reference)}, {cs(category)}, {cs(admission)}, {cs(noun)}, {cs(sdesc)}, "
		f"{cs(description)}, SizeCategory.{size}, ItemQuality.{quality}, {weight}, {cost}m, "
		f"{str(skins == 'yes').lower()}, {cs(material)}, [{tags}], [{components}], {cs(builder_notes)}),"
	)


def render_outfit(row: list[str]) -> str:
	key, name, description, _kind, references = row
	items = ", ".join(cs(reference) for reference in split_values(references))
	return f"\t\tnew({cs(key)}, {cs(name)}, {cs(description)}, [{items}]),"


def generate() -> str:
	rows = table_rows()
	validate(rows)
	outfits = armour_outfit_rows()
	validate_armour_outfits(outfits, rows)
	counts = Counter(row[1] for row in rows)
	return "\n".join([
		"// <auto-generated>",
		"// Generated by scripts/generate-renaissance-military-manifest.py from the canonical Renaissance military reference.",
		"// Full descriptions are authored source text and must not be edited here.",
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
		"\tprivate static readonly RenaissanceMilitaryItemSpec[] RenaissanceMilitaryItemSpecs =",
		"\t[",
		*(render(row) for row in rows),
		"\t];",
		"",
		"\tprivate static readonly OutfitManifestSpec[] RenaissanceMilitaryArmourOutfitManifestSpecs =",
		"\t[",
		*(render_outfit(outfit) for outfit in outfits),
		"\t];",
		"",
		*(f"\tinternal const int RenaissanceMilitary{re.sub(r'[^A-Za-z0-9]', '', category.title())}RowCount = {counts[category]};" for category in CATEGORIES),
		f"\tinternal const int RenaissanceMilitaryCatalogueRowCount = {len(rows)};",
		f"\tinternal const int RenaissanceMilitaryArmourOutfitCount = {len(outfits)};",
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
			print(f"Generated Renaissance military manifest source is stale: {OUTPUT.relative_to(ROOT)}")
			return 1
		return 0
	OUTPUT.write_text(content, encoding="utf-8", newline="\n")
	print(f"Wrote {OUTPUT.relative_to(ROOT)}")
	return 0


if __name__ == "__main__":
	raise SystemExit(main())
