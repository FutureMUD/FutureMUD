#!/usr/bin/env python3
"""Generate ItemSeeder clothing outfit manifests from the canonical design references."""

from __future__ import annotations

import argparse
import re
from dataclasses import dataclass, replace
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ANTIQUITY_DOC = ROOT / "Design Documents/Crafting/Antiquity_Clothing_Design_Reference.md"
MEDIEVAL_DOC = ROOT / "Design Documents/Seeding/Medieval_Clothing_Seeder_Design_Reference.md"
EARLY_MODERN_DOC = ROOT / "Design Documents/Seeding/FutureMUD_EarlyModern_Clothing_Accessories_Design_Reference.md"
RENAISSANCE_MASTER_DOC = ROOT / "Design Documents/Seeding/FutureMUD_Renaissance_Clothing_Accessories_Design_Reference.md"
RENAISSANCE_WESTERN_DOC = ROOT / "Design Documents/Seeding/FutureMUD_Renaissance_Clothing_Catalogue_Western_Mediterranean.md"
RENAISSANCE_ASIA_DOC = ROOT / "Design Documents/Seeding/FutureMUD_Renaissance_Clothing_Catalogue_Asia_Steppe.md"
RENAISSANCE_AFRICA_AMERICAS_DOC = ROOT / "Design Documents/Seeding/FutureMUD_Renaissance_Clothing_Catalogue_Africa_Americas_Maritime.md"
RENAISSANCE_DOCS = (
	RENAISSANCE_MASTER_DOC,
	RENAISSANCE_WESTERN_DOC,
	RENAISSANCE_ASIA_DOC,
	RENAISSANCE_AFRICA_AMERICAS_DOC,
)
MEDIEVAL_SOURCE = ROOT / "DatabaseSeeder/Seeders/ItemSeeder.MedievalClothing.cs"
OUTPUT = ROOT / "DatabaseSeeder/Seeders/ItemSeeder.ClothingOutfitManifestData.Generated.cs"


@dataclass(frozen=True)
class Outfit:
	key: str
	name: str
	description: str
	items: tuple[str, ...]


@dataclass(frozen=True)
class Item:
	stable_reference: str
	noun: str
	short_description: str
	full_description: str
	size: str
	quality: str
	weight: str
	cost: str
	skinnable: bool
	material: str
	tags: tuple[str, ...]
	components: tuple[str, ...]
	builder_notes: str


BELT_CAPACITY_WEAR_COMPONENTS = {"Wear_Waist", "Wear_Sash", "Wear_Bandolier"}
SIX_SLOT_BELT_WEAR_COMPONENTS = {"Wear_Sash", "Wear_Bandolier"}
SIX_SLOT_BELT_ITEM_TERMS = {
	"baldric",
	"bandolier",
	"crossbelt",
	"harness",
	"obi",
	"sash",
}
BELT_LIKE_ITEM_TERMS = {
	"baldric",
	"bandolier",
	"belt",
	"crossbelt",
	"cummerbund",
	"girdle",
	"harness",
	"obi",
	"sash",
}


def with_belt_capacity(item: Item) -> Item:
	if not BELT_CAPACITY_WEAR_COMPONENTS.intersection(item.components):
		return item

	words = set(re.findall(r"[a-z0-9]+", f"{item.noun} {item.short_description}".casefold()))
	if not BELT_LIKE_ITEM_TERMS.intersection(words):
		return item

	if SIX_SLOT_BELT_WEAR_COMPONENTS.intersection(item.components) or SIX_SLOT_BELT_ITEM_TERMS.intersection(words):
		belt_component = "Belt_6"
	elif any(tag.startswith("Functions / Military Equipment") for tag in item.tags):
		belt_component = "Belt_4"
	else:
		belt_component = "Belt_2"

	existing_belt_component = next(
		(component for component in item.components if component.startswith("Belt_")),
		None,
	)
	if existing_belt_component is None:
		return replace(item, components=(*item.components, belt_component))
	if existing_belt_component != "Belt_2" or existing_belt_component == belt_component:
		return item
	return replace(
		item,
		components=tuple(
			belt_component if component == existing_belt_component else component
			for component in item.components
		),
	)


def read(path: Path) -> list[str]:
	return path.read_text(encoding="utf-8-sig").splitlines()


def parse_simple_outfits(path: Path, era: str) -> list[Outfit]:
	lines = read(path)
	active = False
	parsed: list[tuple[str, list[str]]] = []
	current: tuple[str, list[str]] | None = None
	for line in lines:
		if line.startswith("## "):
			if active:
				break
			active = line.casefold() == "## outfit manifests"
			continue
		if not active:
			continue
		if line.startswith("### "):
			current = (line[4:].strip(), [])
			parsed.append(current)
			continue
		if current is not None and (match := re.match(r"^[-*] `([^`]+)`", line)):
			current[1].append(match.group(1))

	return [
		Outfit(
			f"{era.casefold()}_outfit_{index:04d}",
			f"{era} {title}",
			f"Builder-facing {era.lower()} clothing outfit for {title}.",
			tuple(items),
		)
		for index, (title, items) in enumerate(parsed, 1)
	]


def parse_early_modern_outfits() -> list[Outfit]:
	lines = read(EARLY_MODERN_DOC)
	h2 = ""
	h3 = ""
	parsed: list[tuple[str, str, str, list[str]]] = []
	current: tuple[str, str, str, list[str]] | None = None
	for line in lines:
		if line.startswith("#### "):
			title = line[5:].strip()
			if re.search(r"\boutfit\b", title, re.IGNORECASE):
				current = (h2, h3, title, [])
				parsed.append(current)
			else:
				current = None
			continue
		if line.startswith("### "):
			h3 = re.sub(r"^\d+\.\s*", "", line[4:].strip())
			current = None
			continue
		if line.startswith("## "):
			h2 = line[3:].strip()
			h3 = ""
			current = None
			continue
		if current is not None and (match := re.match(r"^[-*] `([^`]+)`", line)):
			current[3].append(match.group(1))

	labels = [title.split("—", 1)[-1].strip() for _, _, title, _ in parsed]
	label_counts = {label.casefold(): sum(x.casefold() == label.casefold() for x in labels) for label in labels}
	draft_names: list[str] = []
	for (_, grouping, _, _), label in zip(parsed, labels, strict=True):
		if label_counts[label.casefold()] == 1 or not grouping:
			draft_names.append(f"Early Modern {label}")
			continue
		qualifier = re.split(r"\s*/\s*|\s+-\s+", grouping, maxsplit=1)[0]
		draft_names.append(f"Early Modern {qualifier} {label}")
	draft_name_counts = {
		name.casefold(): sum(x.casefold() == name.casefold() for x in draft_names)
		for name in draft_names
	}

	outfits: list[Outfit] = []
	for index, ((section, grouping, _, items), label, draft_name) in enumerate(
		zip(parsed, labels, draft_names, strict=True), 1
	):
		name = (
			f"Early Modern {grouping} {label}"
			if draft_name_counts[draft_name.casefold()] > 1
			else draft_name
		)
		description = f"Grouping: {grouping}. Collection: {section}."
		outfits.append(Outfit(f"earlymodern_outfit_{index:04d}", name, description, tuple(items)))
	return outfits


def parse_renaissance_outfits() -> list[Outfit]:
	lines = read(RENAISSANCE_MASTER_DOC)
	active = False
	outfits: list[Outfit] = []
	for line in lines:
		if line.startswith("## "):
			active = line.startswith("## Inferred outfit manifests")
			continue
		if not active or not line.startswith("| `renaissance_outfit_"):
			continue
		cells = [cell.strip() for cell in line.strip().strip("|").split("|")]
		if len(cells) != 5:
			raise ValueError(f"Malformed Renaissance outfit manifest row: {line}")
		stable_key = strip_ticks(cells[0])
		item_references = tuple(re.findall(r"`(renaissance_[^`]+)`", cells[4]))
		outfits.append(
			Outfit(
				stable_key,
				re.sub(r"^Renaissance:\s*", "Renaissance ", cells[1]),
				f"Admission: {cells[2]}. Purpose: {cells[3]}.",
				item_references,
			)
		)
	return outfits


def markdown_9_cell_rows() -> dict[str, list[str]]:
	rows: dict[str, list[str]] = {}
	for line in read(EARLY_MODERN_DOC):
		match = re.match(r"^\|\s*`([^`]+)`\s*\|", line)
		if match is None:
			continue
		cells = [cell.strip() for cell in line.strip().strip("|").split("|")]
		if len(cells) != 9:
			continue
		key = match.group(1)
		if key in rows and rows[key] != cells:
			raise ValueError(f"Conflicting catalogue rows for {key}")
		rows[key] = cells
	return rows


def strip_ticks(value: str) -> str:
	return value.strip().strip("`")


def split_tick_list(value: str) -> tuple[str, ...]:
	return tuple(re.findall(r"`([^`]+)`", value))


def authored_full_description(
	short_description: str,
	noun: str,
	material: str,
	components: tuple[str, ...] | list[str],
	quality: str,
) -> str:
	article = short_description[0].upper() + short_description[1:]
	material_key = material.casefold()
	if material_key in {"gold", "silver", "brass", "bronze", "iron", "steel"}:
		construction = "worked and joined"
		material_detail = (
			f"The {material} has been smoothed on the broad faces while shallow tool traces remain around "
			"the joins and recessed edges."
		)
	elif material_key in {"leather", "deer leather", "rawhide", "fur"}:
		construction = "cut and stitched"
		material_detail = (
			f"The {material} shows a supple grain across the larger panels, with doubled edges and close stitching "
			"where repeated movement would otherwise pull it out of shape."
		)
	elif material_key in {"wood", "straw", "raffia cloth", "barkcloth", "featherwork", "beadwork", "horsehair"}:
		construction = "shaped and bound"
		material_detail = (
			f"The {material} keeps its natural texture visible, and the bindings follow the change from broad surfaces "
			"to narrower edges without hiding how the piece was assembled."
		)
	else:
		construction = "cut and sewn"
		material_detail = (
			f"The {material} falls in visible folds between reinforced seams, with the weave left clear at the hems "
			"and turned edges."
		)

	wear_component = next((component for component in components if component.startswith("Wear_")), "")
	if any(token in wear_component for token in ("Boot", "Shoe", "Sandal", "Stocking", "Leg_Wrap")):
		form_detail = (
			f"The {noun} is built around the foot and ankle, with a firm lower edge and an opening arranged for "
			"secure wear without disguising the shape described above."
		)
	elif any(token in wear_component for token in ("Hat", "Hood", "Turban", "Veil", "Mask", "Coif")):
		form_detail = (
			f"Its crown, folds, or framing edges hold the {noun} around the head while leaving the characteristic "
			"outline plainly visible from the front and side."
		)
	elif any(token in wear_component for token in ("Trousers", "Breeches", "Skirt", "Loincloth", "Breechcloth")):
		form_detail = (
			f"A reinforced waist carries the {noun}, from which the lower panels fall with enough room for ordinary "
			"movement while retaining their deliberate cut."
		)
	elif any(token in wear_component for token in ("Robe", "Dress", "Cloak", "Cape", "Mantle", "Tabard")):
		form_detail = (
			f"The main panels settle from the shoulders into a controlled fall, and the hem and opening give the {noun} "
			"its recognisable proportion when worn."
		)
	elif any(token in wear_component for token in ("Glove", "Sleeve", "Shirt", "Tunic", "Jacket", "Vest", "Bra")):
		form_detail = (
			f"The body and openings are proportioned for close, practical wear, with reinforcement placed where the "
			f"{noun} bends or fastens rather than spread as decoration."
		)
	else:
		form_detail = (
			f"The contact points and fastenings are kept smooth, while the visible body of the {noun} carries the "
			"shape and surface detail that distinguish it at a glance."
		)

	finish = {
		"Standard": "The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
		"Good": "Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
		"VeryGood": "Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
		"Great": "Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
	}.get(quality, "The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join.")
	return (
		f"{article} is {construction} from {material} so that the outward silhouette of the {noun} remains clear. "
		f"{material_detail} {form_detail} {finish}"
	)


def item_from_9_cell_row(row: list[str]) -> Item:
	stable_reference = strip_ticks(row[0])
	short_description = row[1]
	noun = strip_ticks(row[2])
	material = strip_ticks(row[3])
	size, quality = (strip_ticks(value) for value in row[4].split("/", 1))
	weight, cost = (value.strip() for value in row[5].split("/", 1))
	components = split_tick_list(row[6])
	tags = split_tick_list(row[7])
	return Item(
		stable_reference,
		noun,
		short_description,
		authored_full_description(short_description, noun, material, components, quality.replace(" ", "")),
		size.replace(" ", ""),
		quality.replace(" ", ""),
		weight.removesuffix("g").strip(),
		cost.removesuffix("m").strip(),
		"$" in short_description or any(component.startswith("Variable_") for component in components),
		material,
		tags,
		components,
		row[8],
	)


def split_csharp_arguments(text: str) -> list[str]:
	arguments: list[str] = []
	start = 0
	depth = 0
	in_string = False
	verbatim = False
	escaped = False
	for index, char in enumerate(text):
		if in_string:
			if verbatim:
				if char == '"':
					if index + 1 < len(text) and text[index + 1] == '"':
						continue
					in_string = False
			elif escaped:
				escaped = False
			elif char == "\\":
				escaped = True
			elif char == '"':
				in_string = False
			continue
		if char == '"':
			in_string = True
			verbatim = index > 0 and text[index - 1] == "@"
			continue
		if char in "([{":
			depth += 1
		elif char in ")]}":
			depth -= 1
		elif char == "," and depth == 0:
			arguments.append(text[start:index].strip())
			start = index + 1
	arguments.append(text[start:].strip())
	return arguments


def csharp_string(value: str) -> str:
	value = value.strip()
	if value.startswith('@"') and value.endswith('"'):
		return value[2:-1].replace('""', '"')
	if not (value.startswith('"') and value.endswith('"')):
		raise ValueError(f"Expected C# string literal, got {value[:80]}")
	body = value[1:-1]
	escapes = {"n": "\n", "r": "\r", "t": "\t", '"': '"', "\\": "\\", "'": "'"}
	return re.sub(r"\\(.)", lambda match: escapes.get(match.group(1), match.group(0)), body)


def extract_create_item_calls(path: Path) -> dict[str, Item]:
	text = path.read_text(encoding="utf-8-sig")
	items: dict[str, Item] = {}
	position = 0
	while (start := text.find("CreateItem(", position)) >= 0:
		index = start + len("CreateItem(")
		depth = 1
		in_string = False
		escaped = False
		while index < len(text) and depth:
			char = text[index]
			if in_string:
				if escaped:
					escaped = False
				elif char == "\\":
					escaped = True
				elif char == '"':
					in_string = False
			elif char == '"':
				in_string = True
			elif char == "(":
				depth += 1
			elif char == ")":
				depth -= 1
			index += 1
		position = index
		arguments = split_csharp_arguments(text[start + len("CreateItem("):index - 1])
		if len(arguments) < 18 or not arguments[0].lstrip().startswith('"'):
			continue
		stable_reference = csharp_string(arguments[0])
		components = tuple(re.findall(r'"([^"]+)"', arguments[13]))
		tags = tuple(re.findall(r'"([^"]+)"', arguments[12]))
		items[stable_reference] = Item(
			stable_reference,
			csharp_string(arguments[1]),
			csharp_string(arguments[2]),
			csharp_string(arguments[4]),
			arguments[5].split(".")[-1],
			arguments[6].split(".")[-1],
			arguments[7].removesuffix("d").strip(),
			arguments[8].removesuffix("m").strip(),
			arguments[9].casefold() == "true",
			csharp_string(arguments[11]),
			tags,
			components,
			"Early Modern admission of an exact Medieval clothing definition.",
		)
	return items


def parse_full_bullet_specs(path: Path) -> dict[str, Item]:
	items: dict[str, Item] = {}
	pattern = re.compile(
		r"^- `(?P<ref>[^`]+)` - (?P<sdesc>.+?); noun: `(?P<noun>[^`]+)`; material: `(?P<material>[^`]+)`; "
		r"size/quality: `(?P<size>[^`]+)`/`(?P<quality>[^`]+)`; weight/cost: (?P<weight>[\d.]+)g/(?P<cost>[\d.]+)m; "
		r"wear: `(?P<wear>[^`]+)`; variables: (?P<variables>[^.;]+)"
	)
	for line in read(path):
		match = pattern.match(line)
		if match is None:
			continue
		data = match.groupdict()
		components = ["Holdable", data["wear"], "Destroyable_Clothing", "Armour_LightClothing", "Insulation_Minor"]
		variables = data["variables"].strip()
		if variables.casefold() != "none":
			components.extend(re.findall(r"Variable_[A-Za-z0-9_]+", variables))
		quality = data["quality"].replace(" ", "")
		market = "Luxury Clothing" if quality in {"Good", "VeryGood", "Great", "Excellent"} else "Standard Clothing"
		items[data["ref"]] = Item(
			data["ref"], data["noun"], data["sdesc"],
			authored_full_description(data["sdesc"], data["noun"], data["material"], components, quality),
			data["size"].replace(" ", ""), quality, data["weight"], data["cost"], "$" in data["sdesc"],
			data["material"],
			("Era / Antiquity Era", "Functions / Worn Items / Bodywear", f"Market / Clothing / {market}"),
			tuple(components), "Documented Antiquity outfit-manifest dependency."
		)
	return items


def renaissance_admission_items() -> dict[str, Item]:
	rows: dict[str, tuple[str, str, str, str]] = {}
	for line in read(RENAISSANCE_WESTERN_DOC):
		match = re.match(r"^\|\s*`(renaissance_institution_[^`]+)`\s*\|\s*([^|]+)\|\s*([^|]+)\|\s*`([^`]+)`", line)
		if match:
			rows[match.group(1)] = tuple(value.strip() for value in match.groups())  # type: ignore[assignment]
	wear = {
		"renaissance_institution_academic_robe": "Wear_Long_Open_Robe",
		"renaissance_institution_full_cowl": "Wear_Cloak_(Closed)",
		"renaissance_institution_linen_surplus": "Wear_Tabard",
		"renaissance_institution_monastic_scapular": "Wear_Tabard",
		"renaissance_institution_plain_cassock": "Wear_Robe",
		"renaissance_institution_preaching_gown": "Wear_Long_Open_Robe",
	}
	weights = {
		"renaissance_institution_academic_robe": ("1200.0", "120.0"),
		"renaissance_institution_full_cowl": ("900.0", "80.0"),
		"renaissance_institution_linen_surplus": ("650.0", "60.0"),
		"renaissance_institution_monastic_scapular": ("700.0", "55.0"),
		"renaissance_institution_plain_cassock": ("1050.0", "100.0"),
		"renaissance_institution_preaching_gown": ("1200.0", "120.0"),
	}
	items: dict[str, Item] = {}
	for key, component in wear.items():
		_, public_form, material, _ = rows[key]
		noun = public_form.split()[-1]
		short_description = f"a {public_form}"
		weight, cost = weights[key]
		insulation = "Insulation_Moderate" if material in {"wool", "broadcloth"} else "Insulation_Minor"
		items[key] = Item(
			key, noun, short_description,
			authored_full_description(short_description, noun, material,
				("Holdable", "Destroyable_Clothing", component, "Armour_LightClothing", insulation), "Good"),
			"Normal", "Good", weight, cost, False, material,
			("Era / Renaissance Era", "Market / Clothing / Religious Clothing", "Institution / Religious"),
			("Holdable", "Destroyable_Clothing", component, "Armour_LightClothing", insulation),
			"Renaissance institutional admission required by documented Early Modern religious outfits."
		)
	return items


RENAISSANCE_WEAR_COMPONENTS = {
	"WP-BREAST-WRAP": "Wear_Bra",
	"WP-BREECHCLOTH": "Wear_Breechcloth",
	"WP-BREECHES": "Wear_Breeches",
	"WP-COLLAR": "Wear_Partlet",
	"WP-CLOAK": "Wear_Cloak_(Open)",
	"WP-DRAPED-FULL": "Wear_Robe",
	"WP-DOUBLE-WRAP": "Wear_Robe",
	"WP-DRESS": "Wear_Dress",
	"WP-FACE-MASK": "Wear_Mask",
	"WP-FACE-VEIL": "Wear_Veil",
	"WP-FEATHER-CROWN": "Wear_Hat",
	"WP-FITTED-TORSO": "Wear_Vest",
	"WP-FOOT-BOOT": "Wear_Boots",
	"WP-FOOT-SANDAL": "Wear_Sandals",
	"WP-FOOT-SHOE": "Wear_Shoes",
	"WP-FULL-MASK": "Wear_Mask",
	"WP-HANDS": "Wear_Gloves",
	"WP-HANDHELD": None,
	"WP-HEAD-CAP": "Wear_Hat",
	"WP-HEAD-HAT": "Wear_Hat",
	"WP-HEAD-VEIL": "Wear_Head_Veil",
	"WP-HEADWRAP": "Wear_Turban",
	"WP-HOOD": "Wear_Hood",
	"WP-HOSE": "Wear_Chausses",
	"WP-HYBRID-TUNIC": "Wear_Shirt",
	"WP-JACKET": "Wear_Jacket",
	"WP-LEG-WRAPS": "Wear_Leg_Wraps",
	"WP-LONG-UNDERLAYER": "Wear_Robe",
	"WP-MOCCASIN": "Wear_Shoes",
	"WP-MONASTIC-DRAPE": "Wear_Robe",
	"WP-NECK": "Wear_Scarf",
	"WP-OVERSHOE": "Wear_Overshoes",
	"WP-PLEATED-TROUSERS": "Wear_Trousers",
	"WP-RECTANGULAR-BLOUSE": "Wear_Shirt",
	"WP-ROBE-CLOSED": "Wear_Robe",
	"WP-ROBE-OPEN": "Wear_Long_Open_Robe",
	"WP-SHIRT": "Wear_Shirt",
	"WP-SHOULDER": "Wear_Mantle",
	"WP-SHOULDER-WINGS": "Wear_Vest",
	"WP-SIDEFAST-ROBE": "Wear_Robe",
	"WP-SKIRT": "Wear_Long_Skirt",
	"WP-SLEEVES": "Wear_Detachable_Sleeves",
	"WP-SOCKS": "Wear_Stockings",
	"WP-SKIRT-SUPPORT": "Wear_Skirt_Support",
	"WP-STOCKINGS": "Wear_Stockings",
	"WP-STRUCTURED-HEADWRAP": "Wear_Turban",
	"WP-TRIANGLE-SHOULDER": "Wear_Mantle",
	"WP-TROUSERS": "Wear_Trousers",
	"WP-TUBE-SKIRT": "Wear_Long_Skirt",
	"WP-TURBAN-CAP": "Wear_Turban",
	"WP-UNDER-WAIST": "Wear_Shorts",
	"WP-VEST": "Wear_Vest",
	"WP-VESTMENT": "Wear_Tabard",
	"WP-WRAP-SKIRT": "Wear_Long_Skirt",
	"WP-WRAP-DRESS": "Wear_Dress",
}


RENAISSANCE_COMPONENT_WEIGHTS = {
	None: 80,
	"Wear_Bra": 180,
	"Wear_Breechcloth": 180,
	"Wear_Breeches": 620,
	"Wear_Boots": 1050,
	"Wear_Chausses": 520,
	"Wear_Cloak_(Open)": 1050,
	"Wear_Dress": 980,
	"Wear_Detachable_Sleeves": 260,
	"Wear_Gloves": 180,
	"Wear_Hat": 190,
	"Wear_Head_Veil": 180,
	"Wear_Hood": 300,
	"Wear_Jacket": 760,
	"Wear_Leg_Wraps": 360,
	"Wear_Long_Open_Robe": 1220,
	"Wear_Long_Skirt": 680,
	"Wear_Mantle": 620,
	"Wear_Mask": 230,
	"Wear_Overshoes": 380,
	"Wear_Partlet": 170,
	"Wear_Robe": 980,
	"Wear_Scarf": 120,
	"Wear_Sandals": 420,
	"Wear_Shirt": 480,
	"Wear_Shoes": 690,
	"Wear_Shorts": 270,
	"Wear_Skirt_Support": 900,
	"Wear_Stockings": 280,
	"Wear_Tabard": 720,
	"Wear_Trousers": 620,
	"Wear_Turban": 260,
	"Wear_Veil": 180,
	"Wear_Vest": 560,
}


RENAISSANCE_COMPONENT_COSTS = {
	None: 8,
	"Wear_Bra": 5,
	"Wear_Breechcloth": 5,
	"Wear_Breeches": 18,
	"Wear_Boots": 30,
	"Wear_Chausses": 14,
	"Wear_Cloak_(Open)": 28,
	"Wear_Dress": 28,
	"Wear_Detachable_Sleeves": 9,
	"Wear_Gloves": 10,
	"Wear_Hat": 10,
	"Wear_Head_Veil": 9,
	"Wear_Hood": 12,
	"Wear_Jacket": 24,
	"Wear_Leg_Wraps": 8,
	"Wear_Long_Open_Robe": 36,
	"Wear_Long_Skirt": 18,
	"Wear_Mantle": 22,
	"Wear_Mask": 12,
	"Wear_Overshoes": 12,
	"Wear_Partlet": 8,
	"Wear_Robe": 30,
	"Wear_Scarf": 6,
	"Wear_Sandals": 12,
	"Wear_Shirt": 14,
	"Wear_Shoes": 22,
	"Wear_Shorts": 8,
	"Wear_Skirt_Support": 24,
	"Wear_Stockings": 10,
	"Wear_Tabard": 24,
	"Wear_Trousers": 18,
	"Wear_Turban": 12,
	"Wear_Veil": 9,
	"Wear_Vest": 18,
}


RENAISSANCE_MATERIAL_COST_FACTORS = {
	"animal skin": 1.6,
	"bamboo": 0.7,
	"barkcloth": 0.8,
	"beadwork": 5.0,
	"brocade": 4.0,
	"broadcloth": 2.0,
	"camelid wool": 1.7,
	"canvas": 0.9,
	"cotton": 1.2,
	"deer leather": 2.5,
	"feather": 2.5,
	"featherwork": 5.0,
	"felt": 1.2,
	"fur": 3.5,
	"hemp cloth": 0.8,
	"horsehair": 0.8,
	"leather": 2.0,
	"linen": 1.1,
	"raffia cloth": 1.0,
	"ramie cloth": 1.0,
	"rawhide": 1.3,
	"silk": 4.0,
	"silk gauze": 4.5,
	"straw": 0.6,
	"velvet": 5.0,
	"wood": 0.8,
	"wool": 1.4,
}


def renaissance_catalogue_rows() -> dict[str, list[str]]:
	rows: dict[str, list[str]] = {}
	for path in RENAISSANCE_DOCS:
		for line in read(path):
			match = re.match(r"^\|\s*`(renaissance_[^`]+)`\s*\|", line)
			if match is None:
				continue
			cells = [cell.strip() for cell in line.strip().strip("|").split("|")]
			if len(cells) < 5 or not cells[3].startswith("`WP-"):
				continue
			key = match.group(1)
			if key in rows and rows[key] != cells:
				raise ValueError(f"Conflicting Renaissance catalogue rows for {key}")
			rows[key] = cells
	return rows


def renaissance_short_description(public_form: str) -> str:
	if public_form.startswith("pair of "):
		return f"a pair of $colour {public_form.removeprefix('pair of ')}"
	return f"a $colour {public_form}"


def renaissance_item_from_catalogue_row(row: list[str]) -> Item:
	stable_reference = strip_ticks(row[0])
	public_form = row[1]
	material = strip_ticks(row[2])
	wear_profile = strip_ticks(row[3])
	if wear_profile not in RENAISSANCE_WEAR_COMPONENTS:
		raise ValueError(f"No live wear-component mapping for {wear_profile} on {stable_reference}")
	component = RENAISSANCE_WEAR_COMPONENTS[wear_profile]
	if material not in RENAISSANCE_MATERIAL_COST_FACTORS:
		raise ValueError(f"No material cost factor for {material} on {stable_reference}")
	noun = re.sub(r"[^a-z-]", "", public_form.casefold().split()[-1])
	short_description = renaissance_short_description(public_form)
	luxury_materials = {"beadwork", "featherwork", "silk", "silk gauze", "velvet"}
	quality = "Good" if material in luxury_materials else "Standard"
	market = "Luxury Clothing" if quality == "Good" else "Standard Clothing"
	weight = RENAISSANCE_COMPONENT_WEIGHTS[component]
	cost = RENAISSANCE_COMPONENT_COSTS[component] * RENAISSANCE_MATERIAL_COST_FACTORS[material]
	insulation = (
		"Insulation_Moderate"
		if material in {"broadcloth", "camelid wool", "felt", "fur", "wool"}
		else "Insulation_Minor"
	)
	notes = " ".join(row[4:])
	components = (
		"Holdable", "Destroyable_Clothing", *([component] if component else []), "Armour_LightClothing",
		insulation, "Variable_BasicColour"
	)
	return Item(
		stable_reference,
		noun,
		short_description,
		authored_full_description(short_description, noun, material, components, quality),
		"Small" if component in {
			"Wear_Boots", "Wear_Gloves", "Wear_Hat", "Wear_Head_Veil", "Wear_Hood", "Wear_Leg_Wraps",
			"Wear_Mask", "Wear_Sandals", "Wear_Shoes", "Wear_Stockings", "Wear_Turban"
		} else "Normal",
		quality,
		f"{weight}.0",
		f"{cost:.1f}",
		True,
		material,
		("Era / Renaissance Era", f"Market / Clothing / {market}"),
		components,
		f"Inferred Renaissance outfit-manifest dependency. Catalogue admission: {notes}",
	)


def cs(value: str) -> str:
	return '"' + value.replace("\\", "\\\\").replace('"', '\\"').replace("\r", "").replace("\n", "\\n") + '"'


def array(values: tuple[str, ...]) -> str:
	return "[" + ", ".join(cs(value) for value in values) + "]"


def render_manifest_array(name: str, outfits: list[Outfit]) -> list[str]:
	lines = [f"\tprivate static readonly OutfitManifestSpec[] {name} =", "\t["]
	for outfit in outfits:
		lines.append(f"\t\tnew({cs(outfit.key)}, {cs(outfit.name)}, {cs(outfit.description)}, {array(outfit.items)}),")
	lines.extend(["\t];", ""])
	return lines


def render_item_array(name: str, items: list[Item]) -> list[str]:
	lines = [f"\tprivate static readonly DocumentedClothingItemSpec[] {name} =", "\t["]
	for item in items:
		lines.append(
			f"\t\tnew({cs(item.stable_reference)}, {cs(item.noun)}, {cs(item.short_description)}, "
			f"BuildDocumentedClothingFullDescription({cs(item.short_description)}, {cs(item.noun)}, {cs(item.material)}, "
			f"{array(item.components)}, ItemQuality.{item.quality}), SizeCategory.{item.size}, ItemQuality.{item.quality}, {item.weight}, {item.cost}m, "
			f"{str(item.skinnable).lower()}, {cs(item.material)}, {array(item.tags)}, {array(item.components)}, {cs(item.builder_notes)}),"
		)
	lines.extend(["\t];", ""])
	return lines


def generate() -> str:
	antiquity = parse_simple_outfits(ANTIQUITY_DOC, "Antiquity")
	medieval = parse_simple_outfits(MEDIEVAL_DOC, "Medieval")
	renaissance = parse_renaissance_outfits()
	early_modern = parse_early_modern_outfits()
	all_outfits = antiquity + medieval + renaissance + early_modern

	if (len(antiquity), len(medieval), len(renaissance), len(early_modern)) != (29, 164, 59, 883):
		raise ValueError(
			f"Unexpected outfit counts: Antiquity={len(antiquity)}, Medieval={len(medieval)}, "
			f"Renaissance={len(renaissance)}, EarlyModern={len(early_modern)}"
		)
	if len({outfit.key.casefold() for outfit in all_outfits}) != len(all_outfits):
		raise ValueError("Generated outfit manifest keys are not unique")
	if len({outfit.name.casefold() for outfit in all_outfits}) != len(all_outfits):
		raise ValueError("Generated outfit manifest names are not unique")
	for outfit in all_outfits:
		if not outfit.items:
			raise ValueError(f"Outfit {outfit.key} has no item references")
		if len({item.casefold() for item in outfit.items}) != len(outfit.items):
			raise ValueError(f"Outfit {outfit.key} repeats an item reference")
		if len(outfit.key) > 100 or len(outfit.name) > 200 or any(len(item) > 100 for item in outfit.items):
			raise ValueError(f"Outfit {outfit.key} exceeds an outfit-template database text limit")

	rows = markdown_9_cell_rows()
	medieval_source_items = extract_create_item_calls(MEDIEVAL_SOURCE)
	renaissance_admissions = renaissance_admission_items()
	early_modern_outfit_refs = {item for outfit in early_modern for item in outfit.items}
	fifth_pass_refs = {
		stable_reference
		for stable_reference in rows
		if stable_reference.startswith(("earlymodern_headwear_", "earlymodern_footwear_"))
	}
	if (len(fifth_pass_refs),
		sum(reference.startswith("earlymodern_headwear_") for reference in fifth_pass_refs),
		sum(reference.startswith("earlymodern_footwear_") for reference in fifth_pass_refs)) != (84, 48, 36):
		raise ValueError("Unexpected Early Modern fifth-pass standalone clothing catalogue coverage")
	early_modern_refs = early_modern_outfit_refs | fifth_pass_refs
	early_modern_items: dict[str, Item] = {}
	for stable_reference in sorted(early_modern_refs):
		if stable_reference in rows:
			early_modern_items[stable_reference] = item_from_9_cell_row(rows[stable_reference])
		elif stable_reference in medieval_source_items:
			early_modern_items[stable_reference] = medieval_source_items[stable_reference]
		elif stable_reference in renaissance_admissions:
			early_modern_items[stable_reference] = renaissance_admissions[stable_reference]
		else:
			raise ValueError(f"No documented or live-source item definition for {stable_reference}")
	early_modern_items = {
		stable_reference: with_belt_capacity(item)
		for stable_reference, item in early_modern_items.items()
	}
	if len(early_modern_items) != 1033:
		raise ValueError(f"Unexpected Early Modern clothing catalogue count: {len(early_modern_items)}")
	for item in early_modern_items.values():
		wear_components = [component for component in item.components if component.startswith("Wear_")]
		if len(wear_components) != 1:
			raise ValueError(
				f"Outfit item {item.stable_reference} must define exactly one wearable component; found {wear_components}"
			)

	renaissance_rows = renaissance_catalogue_rows()
	if len(renaissance_rows) != 471:
		raise ValueError(f"Unexpected Renaissance clothing catalogue count: {len(renaissance_rows)}")
	renaissance_items = {
		stable_reference: with_belt_capacity(
			early_modern_items.get(
				stable_reference,
				renaissance_item_from_catalogue_row(row)
			)
		)
		for stable_reference, row in sorted(renaissance_rows.items())
	}
	if len(renaissance_items) != 471:
		raise ValueError(f"Unexpected Renaissance clothing item count: {len(renaissance_items)}")
	for item in renaissance_items.values():
		wear_components = [component for component in item.components if component.startswith("Wear_")]
		if len(wear_components) > 1:
			raise ValueError(
				f"Renaissance clothing item {item.stable_reference} must define at most one wearable component; "
				f"found {wear_components}"
			)
	for outfit in renaissance:
		wear_components = [
			next(
				component
				for component in renaissance_items[stable_reference].components
				if component.startswith("Wear_")
			)
			for stable_reference in outfit.items
		]
		duplicates = sorted({component for component in wear_components if wear_components.count(component) > 1})
		if duplicates:
			raise ValueError(
				f"Renaissance outfit {outfit.key} repeats default wearable components: {duplicates}"
			)

	antiquity_source_items = parse_full_bullet_specs(ANTIQUITY_DOC)
	antiquity_missing_refs = {
		"antiquity_fine_pleated_kalasiris",
		"antiquity_fine_sheer_linen_cape",
		"antiquity_pleated_linen_shendyt",
		"antiquity_sheer_linen_overshirt",
		"antiquity_simple_linen_shendyt",
		"antiquity_straight_linen_kalasiris",
	}
	antiquity_items = [with_belt_capacity(antiquity_source_items[key]) for key in sorted(antiquity_missing_refs)]
	for item in [*antiquity_items, *renaissance_items.values(), *early_modern_items.values()]:
		description = authored_full_description(
			item.short_description, item.noun, item.material, item.components, item.quality
		)
		if len(description) < 300 or description.count(".") < 4:
			raise ValueError(f"Clothing description is not substantive enough for {item.stable_reference}")
		if "recognisable form and drape" in description or "documented form" in description:
			raise ValueError(f"Clothing description retains generic scaffold prose for {item.stable_reference}")

	lines = [
		"// <auto-generated>",
		"// Generated by scripts/generate-item-seeder-outfit-manifests.py from the canonical clothing design references.",
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
	]
	lines.extend(render_item_array("AntiquityOutfitSupplementalItemSpecs", antiquity_items))
	lines.extend(render_item_array("RenaissanceClothingItemSpecs", list(renaissance_items.values())))
	lines.extend(render_item_array("EarlyModernOutfitReferencedItemSpecs", list(early_modern_items.values())))
	lines.extend(render_manifest_array("AntiquityOutfitManifestSpecs", antiquity))
	lines.extend(render_manifest_array("MedievalOutfitManifestSpecs", medieval))
	lines.extend(render_manifest_array("RenaissanceOutfitManifestSpecs", renaissance))
	lines.extend(render_manifest_array("EarlyModernOutfitManifestSpecs", early_modern))
	lines.append("}")
	lines.append("")
	return "\n".join(lines)


def main() -> int:
	parser = argparse.ArgumentParser()
	parser.add_argument("--check", action="store_true", help="Fail if the checked-in generated source is stale.")
	args = parser.parse_args()
	content = generate()
	if args.check:
		if not OUTPUT.exists() or OUTPUT.read_text(encoding="utf-8") != content:
			print(f"Generated outfit manifest source is stale: {OUTPUT.relative_to(ROOT)}")
			return 1
		return 0
	OUTPUT.write_text(content, encoding="utf-8", newline="\n")
	print(f"Wrote {OUTPUT.relative_to(ROOT)}")
	return 0


if __name__ == "__main__":
	raise SystemExit(main())
