#!/usr/bin/env python3
"""Generate the Renaissance and Early Modern jewellery/devotional and door catalogues.

The canonical catalogue is deliberately constructed here rather than at runtime.  The
generated C# files contain literal CreateItem calls so that the stable seeded content is
easy to review alongside the accompanying CSV/FDesc catalogues.
"""

from __future__ import annotations

import argparse
import csv
import json
from dataclasses import dataclass
from pathlib import Path
from typing import Iterable


ROOT = Path(__file__).resolve().parents[1]
SEEDING = ROOT / "Design Documents" / "Seeding"
SEEDERS = ROOT / "DatabaseSeeder" / "Seeders"


@dataclass(frozen=True)
class Item:
	stable_reference: str
	owner: str
	availability: str
	family: str
	subfamily: str
	culture: str
	noun: str
	sdesc: str
	fdesc: str
	size: str
	quality: str
	weight: int
	cost: int
	material: str
	tags: tuple[str, ...]
	components: tuple[str, ...]
	builder_notes: str
	craft_category: str
	craft_trait: str


PREINDUSTRIAL_TAG = "Era / Pre-Industrial Era"
RENAISSANCE_TAG = "Era / Renaissance Era"
EARLY_MODERN_TAG = "Era / Early Modern Era"
JEWELLERY_TAG = "Functions / Worn Items / Jewellery"
RELIGIOUS_TAG = "Functions / Household Items / Household Religious Items"
DEVOTIONAL_MARKET_TAG = "Market / Religious Goods / Devotional Goods"
CONSTRUCTION_TAG = "Functions / Household Items / Household Construction Materials"
HOUSEHOLD_WARES_TAG = "Functions / Household Items / Household Wares"
WORKED_TIMBER_TAG = "Market / Construction Materials / Worked Timber"
GLASS_PANES_TAG = "Market / Construction Materials / Glass Panes"
STANDARD_WARES_TAG = "Market / Household Goods / Standard Wares"


MATERIALS = (
	"copper", "bronze", "silver", "gold", "brass", "glass", "bone", "shell", "wood",
	"ivory", "pearl", "amber", "coral", "garnet", "agate", "carnelian", "lapis lazuli",
	"turquoise", "jade", "quartz", "faience", "leather", "silk", "linen", "oak", "pine",
	"cedar", "teak", "bamboo", "wrought iron", "lead", "pewter"
)

DESTROYABLE_BY_MATERIAL = {
	"copper": "Destroyable_HeavyMetal", "bronze": "Destroyable_HeavyMetal",
	"silver": "Destroyable_HeavyMetal", "gold": "Destroyable_HeavyMetal",
	"brass": "Destroyable_HeavyMetal", "wrought iron": "Destroyable_HeavyMetal",
	"lead": "Destroyable_HeavyMetal", "pewter": "Destroyable_HeavyMetal",
	"glass": "Destroyable_Glassware", "faience": "Destroyable_Glassware",
	"silk": "Destroyable_Clothing", "linen": "Destroyable_Clothing",
	"leather": "Destroyable_Clothing",
}


REN_CULTURES = (
	("wer", "Western European Renaissance"), ("iba", "Iberian Atlantic"),
	("cen", "Central European"), ("nba", "Northern Baltic"),
	("cef", "Central Eastern Frontier"), ("eon", "Eastern Orthodox Northern"),
	("ott", "Ottoman Islamicate"), ("pip", "Persianate Indo-Persian"),
	("sas", "South Asian"), ("eal", "East Asian Literati"), ("jpn", "Japanese"),
	("mea", "Maritime East Asian"), ("sem", "South-east Asian Mainland"),
	("msea", "Maritime South-east Asian"), ("stp", "Steppe and Caravan"),
	("aca", "African Court Atlantic"), ("sai", "Sahelian Islamic"),
	("rse", "Red Sea"),
	("ind", "Indian Ocean"), ("mes", "Mesoamerican"), ("and", "Andean"),
	("car", "Caribbean Contact"), ("nac", "North American Contact"),
	("col", "Colonial Atlantic"),
)

EARLY_MODERN_CULTURES = (
	("fr", "French Baroque court and urban"), ("nl", "Dutch Republic and Low Countries"),
	("gb", "British Stuart Georgian"), ("ib", "Iberian Portuguese Spanish empires"),
	("hre", "German HRE Austrian"), ("it", "Italian states"),
	("scb", "Scandinavian Baltic"), ("plh", "Polish Lithuanian Hungarian frontier"),
	("rus", "Russian Petrine and post Petrine"), ("ott", "Ottoman"),
	("mag", "Maghrebi North African"), ("saf", "Safavid post Safavid Persianate"),
	("mug", "Mughal Indo Persian"), ("mrd", "Maratha Rajput Deccan"),
	("sic", "South Indian coastal trade"), ("qing", "Qing China"),
	("ming", "Late Ming transition"), ("jos", "Joseon Korea"), ("edo", "Edo Japan"),
	("ryu", "Ryukyu maritime East Asia"), ("sea", "Mainland South-east Asian courts"),
	("msea", "Maritime South-east Asian trade worlds"), ("ias", "Inner Asian steppe frontier"),
	("wa", "West African court Atlantic trade"), ("kongo", "Kongo Angola West Central Africa"),
	("sahel", "Sahelian Hausa Islamic West Africa"), ("eth", "Ethiopian Red Sea"),
	("swa", "Swahili Coast Indian Ocean Africa"), ("spa", "Spanish colonial Americas"),
	("bra", "Portuguese Brazil Atlantic plantation"), ("ena", "English French Dutch colonial North America"),
	("ina", "Indigenous North American regional families"), ("mca", "Mesoamerican colonial Indigenous"),
	("and", "Andean colonial Indigenous"), ("car", "Caribbean Atlantic plantation"),
	("gmt", "Global maritime chartered company trade"),
)


JEWELLERY_FORMS = (
	("ring", "ring", "Wear_Ring", "Rings", False),
	("signet_ring", "ring", "Wear_Ring", "Rings", False),
	("pendant", "pendant", "Wear_Necklace", "Pendants", False),
	("bead_necklace", "necklace", "Wear_Necklace", "Bead Strings", False),
	("chain_necklace", "necklace", "Wear_Necklace", "Necklaces", False),
	("bracelet", "bracelet", "Wear_Bracelet", "Bracelets", False),
	("armlet", "armlet", "Wear_Armlet", "Armlets", False),
	("anklet", "anklet", "Wear_Anklet", "Anklets", False),
	("earrings", "earrings", "Wear_Earrings", "Earrings", False),
	("brooch", "brooch", "Wear_Brooch", "Brooches", False),
	("pin", "pin", "Wear_Pin", "Pins", False),
	("hairpin", "hairpin", "Wear_Hairpin", "Hair Ornaments", False),
	("circlet", "circlet", "Wear_Circlet", "Circlets", False),
	("diadem", "diadem", "Wear_Diadem", "Diadems", False),
	("waist_chain", "waist chain", "Wear_Waist_Chain", "Waist Chains", False),
	("belt_ornament", "belt ornament", "Wear_Belt_Ornament", "Belt Ornaments", False),
	("badge", "badge", "Wear_Badge", "Badges", False),
	("temple_ring", "temple ring", "Wear_Temple_Rings", "Temple Rings", False),
	("torc", "torc", "Wear_Torc", "Torcs", False),
	("wreath", "wreath", "Wear_Wreath", "Wreaths", False),
	("prayer_beads", "prayer bead strand", "Wear_Necklace", "Bead Strings", True),
	("devotional_medal", "devotional medal", "Wear_Necklace", "Pendants", True),
	("devotional_badge", "devotional badge", "Wear_Badge", "Badges", True),
	("prayer_cord", "prayer cord", "Wear_Neck_Garland", "Neck Garlands", True),
	("votive_token", "votive token", "", "", True),
	("scripture_box", "small scripture box", "", "", True),
	("offering_bowl", "offering bowl", "", "", True),
	("incense_holder", "incense holder", "", "", True),
	("portable_panel", "small devotional panel", "", "", True),
	("processional_ornament", "processional ornament", "", "", True),
)

DOOR_FORMS = (
	("panel_door", "door", "Door_Normal_Large", "door leaf"),
	("braced_door", "door", "Door_Tough_Large", "braced door"),
	("lockable_door", "door", "Door_Lockable_Normal_Large", "lockable door"),
	("secure_door", "door", "Door_Lockable_Secure_Large", "secure door"),
	("shutter", "shutter", "Door_Bad_Large", "shuttered opening"),
	("screen", "screen", "Door_Bad_Large", "screen door"),
	("lattice_gate", "gate", "Door_Normal_Large", "lattice gate"),
	("courtyard_gate", "gate", "Door_Lockable_Normal_Large", "courtyard gate"),
	("warehouse_gate", "gate", "Door_Lockable_Tough_Large", "warehouse gate"),
	("garden_gate", "gate", "Door_Normal_Large", "garden gate"),
	("grille", "grille", "Door_Secure_Large", "metal grille"),
	("barrier", "barrier", "Door_Tough_Large", "barred barrier"),
	("hatch", "hatch", "Door_Lockable_Normal_Large", "service hatch"),
	("wicket", "wicket", "Door_Lockable_Normal_Large", "wicket gate"),
)

HARDWARE_FORMS = (
	("warded_lock", "lock", "Warded_Lock_Normal", "warded lock"),
	("good_warded_lock", "lock", "Warded_Lock_Good", "fine warded lock"),
	("master_warded_lock", "lock", "Warded_Lock_Master", "master warded lock"),
	("warded_key", "key", "Warded_Key", "warded key"),
	("ring_key", "key", "Warded_Key", "ring bow key"),
	("door_latch", "latch", "Latch_Normal", "door latch"),
	("gate_dropbar", "drop bar", "Latch_Gate_DropBar", "gate drop bar"),
	("door_bar", "bar", "Latch_Door_Bar", "door bar"),
	("portcullis_pawl", "pawl", "Latch_Portcullis_Pawl", "winch pawl"),
	("hasp", "hasp", "Latch_Container_Hasp", "iron hasp"),
	("hook_latch", "hook latch", "Latch_Container_Hook", "hook latch"),
	("strike_plate", "strike plate", "Latch_Normal", "strike plate"),
	("hinge_pair", "hinges", "Latch_Normal", "hinge pair"),
	("keyring", "keyring", "Warded_Key", "keyring"),
	("escutcheon", "escutcheon", "Latch_Normal", "keyhole escutcheon"),
)

MOTIFS = (
	"leaf-scroll", "sunburst", "water-wave", "knotted", "flower-head", "star", "bird", "vine",
	"geometric", "cloud", "shell", "reed", "rosette", "checker", "braided", "radiating"
)


def cs(value: str) -> str:
	return json.dumps(value, ensure_ascii=False)


def destroyable(material: str) -> str:
	return DESTROYABLE_BY_MATERIAL.get(material, "Destroyable_Misc")


def quality(index: int, elite: bool = False) -> str:
	if elite:
		return ("Good", "VeryGood", "Excellent")[index % 3]
	return ("Standard", "Good", "Good", "Standard", "VeryGood", "Good", "Standard", "Poor")[index % 8]


def jewellery_material(index: int) -> str:
	return (
		"copper", "bronze", "silver", "gold", "brass", "glass", "bone", "shell", "ivory", "pearl",
		"amber", "coral", "garnet", "agate", "carnelian", "lapis lazuli", "turquoise", "jade", "quartz",
		"faience", "pewter"
	)[index % 21]


def door_material(index: int, slug: str) -> str:
	if slug in {"grille", "barrier"}:
		return ("wrought iron", "bronze", "brass")[index % 3]
	if "gate" in slug or slug == "wicket":
		return ("oak", "cedar", "teak", "wrought iron", "bronze")[index % 5]
	if slug == "screen":
		return ("bamboo", "teak", "cedar", "wood")[index % 4]
	return ("oak", "pine", "cedar", "teak", "wood")[index % 5]


def jewellery_craft_trait(material: str) -> str:
	if material in {"glass", "faience"}:
		return "Glassworking"
	if material in {"bone", "shell", "ivory", "coral"}:
		return "Scrimshawing"
	if material == "wood":
		return "Carpentry"
	if material in {"silk", "linen"}:
		return "Weaving"
	if material in {"copper", "bronze", "silver", "gold", "brass", "pewter"}:
		return "Silversmithing"
	return "Gemcraft"


def renaissance_culture_tag(code: str, name: str) -> str:
	return f"Culture / Renaissance / Shared / {name}"


def jewellery_item(prefix: str, owner: str, availability: str, culture: str, index: int,
	form: tuple[str, str, str, str, bool], era_tag: str, culture_tag: str | None = None) -> Item:
	slug, noun, wearable, subtype, devotional = form
	if slug == "prayer_cord":
		material = ("silk", "linen")[index % 2]
	elif slug == "portable_panel":
		material = ("wood", "ivory")[index % 2]
	elif slug in {"offering_bowl", "incense_holder", "processional_ornament"}:
		material = ("bronze", "brass", "silver", "pewter")[index % 4]
	else:
		material = jewellery_material(index + len(culture))
	motif = MOTIFS[(index + len(culture) * 3) % len(MOTIFS)]
	craft_trait = jewellery_craft_trait(material)
	if devotional and not wearable:
		tags = (era_tag, RELIGIOUS_TAG, DEVOTIONAL_MARKET_TAG)
		components = ("Holdable", destroyable(material))
		subfamily = "institutional devotional"
		sdesc = f"a {motif} {material} {noun}"
		fdesc = (
			f"The {material} {noun} has a compact {motif} face and carefully finished edges. "
			"Its visible fittings are simple and accessible, with no concealed compartment or implied sacred power."
		)
	else:
		tags = (era_tag, JEWELLERY_TAG, f"Functions / Worn Items / Jewellery / {subtype}",
			"Market / Jewellery / Standard Jewellery")
		if devotional:
			tags += (RELIGIOUS_TAG, DEVOTIONAL_MARKET_TAG)
		components = tuple(x for x in ("Holdable", wearable, destroyable(material)) if x)
		subfamily = "personal devotional" if devotional else "personal adornment"
		sdesc = f"a {motif} {material} {noun}"
		fdesc = (
			f"The {material} {noun} is worked into a {motif} pattern with smooth inner surfaces and visible tool marks "
			"at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism."
		)
	if culture_tag:
		tags += (culture_tag,)
	note = (
		f"{owner} {subfamily} catalogue; culture admission {culture}; availability {availability}. "
		"Description is form-led and makes no unsupported devotional or mechanical claim."
	)
	return Item(
		f"{prefix}_{slug}_{index + 1:02d}", owner, availability, "Jewellery & devotional", subfamily,
		culture, noun, sdesc, fdesc, "Tiny" if wearable else "Small", quality(index, "signet" in slug),
		20 + (index % 11) * 7, 8 + (index % 17) * 9, material, tags, components, note,
		"Jewellery and devotional work", craft_trait,
	)


def door_item(prefix: str, owner: str, availability: str, culture: str, index: int,
	form: tuple[str, str, str, str], era_tag: str, culture_tag: str | None = None) -> Item:
	slug, noun, component, description_kind = form
	material = door_material(index + len(culture), slug)
	motif = MOTIFS[(index * 2 + len(culture)) % len(MOTIFS)]
	market_tag = (GLASS_PANES_TAG if material == "glass" else WORKED_TIMBER_TAG
		if material in {"oak", "pine", "cedar", "teak", "bamboo", "wood"} else STANDARD_WARES_TAG)
	tags = (era_tag, CONSTRUCTION_TAG, market_tag)
	components = ("Holdable", "Destroyable_Door", component)
	sdesc = f"a {motif} {material} {description_kind}"
	fdesc = (
		f"The {material} {description_kind} is framed with a firm outer rail and a {motif} worked face. "
		"Its hinge edge, closing edge, and lower corners show the ordinary abrasion of fitting and use."
	)
	if culture_tag:
		tags += (culture_tag,)
	note = (
		f"{owner} doors, locks and gates catalogue; culture admission {culture}; availability {availability}. "
		"Door behaviour is limited to the attached supported component profile."
	)
	return Item(
		f"{prefix}_{slug}_{index + 1:02d}", owner, availability, "Doors, locks & gates", "door or gate",
		culture, noun, sdesc, fdesc, "VeryLarge" if "gate" in slug else "Large", quality(index),
		24000 + (index % 19) * 1800, 45 + (index % 23) * 11, material, tags, components, note,
		"Carpentry and joinery", "Carpentry",
	)


def hardware_item(prefix: str, owner: str, availability: str, culture: str, index: int,
	form: tuple[str, str, str, str], era_tag: str, culture_tag: str | None = None) -> Item:
	slug, noun, component, description_kind = form
	material = ("wrought iron", "bronze", "brass", "copper", "pewter")[index % 5]
	motif = MOTIFS[(index + len(culture) * 2) % len(MOTIFS)]
	tags = (era_tag, HOUSEHOLD_WARES_TAG, STANDARD_WARES_TAG)
	components = ("Holdable", "Destroyable_HeavyMetal", component)
	sdesc = f"a {motif} {material} {description_kind}"
	fdesc = (
		f"The {material} {description_kind} has a {motif} worked face, rubbed bearing edges, and a plainly accessible "
		"working surface. It represents only the behaviour supplied by its fitted mechanical component."
	)
	if culture_tag:
		tags += (culture_tag,)
	note = (
		f"{owner} doors, locks and gates hardware catalogue; culture admission {culture}; availability {availability}. "
		"It does not imply a custom matched-key or hidden-lock system."
	)
	return Item(
		f"{prefix}_{slug}_{index + 1:02d}", owner, availability, "Doors, locks & gates", "lock and hardware",
		culture, noun, sdesc, fdesc, "Small", quality(index), 160 + (index % 17) * 85,
		7 + (index % 19) * 8, material, tags, components, note, "Locksmithing", "Blacksmithing",
	)


def make_preindustrial() -> list[Item]:
	items: list[Item] = []
	for index in range(60):
		items.append(jewellery_item(
			"preindustrial_jewellery", "Pre-industrial", "Antiquity / Medieval / Renaissance / Early Modern",
			"cross-cultural", index, JEWELLERY_FORMS[index % 20], PREINDUSTRIAL_TAG,
		))
	for index in range(90):
		if index < 54:
			items.append(door_item(
				"preindustrial_door", "Pre-industrial", "Antiquity / Medieval / Renaissance / Early Modern",
				"cross-cultural", index, DOOR_FORMS[index % len(DOOR_FORMS)], PREINDUSTRIAL_TAG,
			))
		else:
			items.append(hardware_item(
				"preindustrial_door", "Pre-industrial", "Antiquity / Medieval / Renaissance / Early Modern",
				"cross-cultural", index, HARDWARE_FORMS[index % len(HARDWARE_FORMS)], PREINDUSTRIAL_TAG,
			))
	return items


def make_renaissance_common() -> list[Item]:
	items: list[Item] = []
	for index in range(220):
		items.append(jewellery_item(
			"renaissance_jewellery_common", "Renaissance", "Renaissance / Early Modern",
			"Renaissance and Early Modern common", index, JEWELLERY_FORMS[index % len(JEWELLERY_FORMS)], RENAISSANCE_TAG,
		))
	for index in range(240):
		if index < 144:
			items.append(door_item(
				"renaissance_door_common", "Renaissance", "Renaissance / Early Modern",
				"Renaissance and Early Modern common", index, DOOR_FORMS[index % len(DOOR_FORMS)], RENAISSANCE_TAG,
			))
		else:
			items.append(hardware_item(
				"renaissance_door_common", "Renaissance", "Renaissance / Early Modern",
				"Renaissance and Early Modern common", index, HARDWARE_FORMS[index % len(HARDWARE_FORMS)], RENAISSANCE_TAG,
			))
	return items


def make_renaissance_specific() -> list[Item]:
	items: list[Item] = []
	for culture_index, (code, name) in enumerate(REN_CULTURES):
		culture_tag = renaissance_culture_tag(code, name)
		for local_index in range(30):
			items.append(jewellery_item(
				f"renaissance_jewellery_{code}", "Renaissance", "Renaissance",
				name, local_index + culture_index * 30, JEWELLERY_FORMS[local_index % len(JEWELLERY_FORMS)],
				RENAISSANCE_TAG, culture_tag,
			))
	for culture_index, (code, name) in enumerate(REN_CULTURES):
		culture_tag = renaissance_culture_tag(code, name)
		for local_index in range(28):
			if culture_index == len(REN_CULTURES) - 1 and local_index >= 26:
				continue
			index = local_index + culture_index * 28
			if local_index < 17:
				items.append(door_item(
					f"renaissance_door_{code}", "Renaissance", "Renaissance", name, index,
					DOOR_FORMS[local_index % len(DOOR_FORMS)], RENAISSANCE_TAG, culture_tag,
				))
			else:
				items.append(hardware_item(
					f"renaissance_door_{code}", "Renaissance", "Renaissance", name, index,
					HARDWARE_FORMS[local_index % len(HARDWARE_FORMS)], RENAISSANCE_TAG, culture_tag,
				))
	return items


def make_early_modern_specific() -> list[Item]:
	items: list[Item] = []
	for culture_index, (code, name) in enumerate(EARLY_MODERN_CULTURES):
		for local_index in range(20):
			items.append(jewellery_item(
				f"earlymodern_jewellery_{code}", "Early Modern", "Early Modern", name,
				local_index + culture_index * 20, JEWELLERY_FORMS[local_index % len(JEWELLERY_FORMS)], EARLY_MODERN_TAG,
			))
	for culture_index, (code, name) in enumerate(EARLY_MODERN_CULTURES):
		limit = 19 if culture_index < 22 else 18
		for local_index in range(limit):
			index = local_index + culture_index * 19
			if local_index < 11:
				items.append(door_item(
					f"earlymodern_door_{code}", "Early Modern", "Early Modern", name, index,
					DOOR_FORMS[local_index % len(DOOR_FORMS)], EARLY_MODERN_TAG,
				))
			else:
				items.append(hardware_item(
					f"earlymodern_door_{code}", "Early Modern", "Early Modern", name, index,
					HARDWARE_FORMS[local_index % len(HARDWARE_FORMS)], EARLY_MODERN_TAG,
				))
	return items


def catalogues() -> dict[str, list[Item]]:
	all_items = {
		"preindustrial": make_preindustrial(),
		"renaissance_common": make_renaissance_common(),
		"renaissance_specific": make_renaissance_specific(),
		"earlymodern_specific": make_early_modern_specific(),
	}
	assert len([x for x in all_items["preindustrial"] if x.family == "Jewellery & devotional"]) == 60
	assert len([x for x in all_items["preindustrial"] if x.family == "Doors, locks & gates"]) == 90
	assert len([x for x in all_items["renaissance_common"] if x.family == "Jewellery & devotional"]) == 220
	assert len([x for x in all_items["renaissance_common"] if x.family == "Doors, locks & gates"]) == 240
	assert len([x for x in all_items["renaissance_specific"] if x.family == "Jewellery & devotional"]) == 720
	assert len([x for x in all_items["renaissance_specific"] if x.family == "Doors, locks & gates"]) == 670
	assert len([x for x in all_items["earlymodern_specific"] if x.family == "Jewellery & devotional"]) == 720
	assert len([x for x in all_items["earlymodern_specific"] if x.family == "Doors, locks & gates"]) == 670
	all_references = [item.stable_reference for items in all_items.values() for item in items]
	assert len(all_references) == 3390 and len(set(all_references)) == 3390
	assert all(reference == reference.lower() for reference in all_references)
	assert all(reference.replace("_", "").isalnum() for reference in all_references)
	return all_items


def render_create_item(item: Item) -> str:
	tags = ", ".join(cs(tag) for tag in item.tags)
	components = ", ".join(cs(component) for component in item.components)
	return "\n".join((
		"\t\tCreateItem(",
		f"\t\t\t{cs(item.stable_reference)},",
		f"\t\t\t{cs(item.noun)},",
		f"\t\t\t{cs(item.sdesc)},",
		"\t\t\tnull,",
		f"\t\t\t{cs(item.fdesc)},",
		f"\t\t\tSizeCategory.{item.size},",
		f"\t\t\tItemQuality.{item.quality},",
		f"\t\t\t{item.weight}.0,",
		f"\t\t\t{item.cost}.0m,",
		"\t\t\ttrue,",
		"\t\t\tfalse,",
		f"\t\t\t{cs(item.material)},",
		f"\t\t\t[{tags}],",
		f"\t\t\t[{components}],",
		"\t\t\tnull,",
		"\t\t\tnull,",
		"\t\t\tnull,",
		"\t\t\tnull,",
		f"\t\t\t{cs(item.builder_notes)},",
		"\t\t\tallowLegacyShortDescriptionMatch: false",
		"\t\t);",
	))


def render_file(method: str, items: Iterable[Item], source: str) -> str:
	rows = "\n\n".join(render_create_item(item) for item in items)
	return "\n".join((
		"// <auto-generated>",
		f"// Generated by scripts/generate-renaissance-earlymodern-jewellery-doors.py from {source}.",
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
		f"\tprivate void {method}()",
		"\t{",
		rows,
		"\t}",
		"}",
		"",
	))


def render_dependency_validation_file(items: dict[str, list[Item]]) -> str:
	all_items = [item for catalogue in items.values() for item in catalogue]
	materials = sorted({item.material for item in all_items}, key=str.lower)
	tags = sorted({tag for item in all_items for tag in item.tags}, key=str.lower)
	components = sorted({component for item in all_items for component in item.components}, key=str.lower)

	def collection(name: str, values: list[str]) -> list[str]:
		return [f"\tprivate static readonly string[] {name} =", "\t[", *[f"\t\t{cs(value)}," for value in values], "\t];", ""]

	def strip_article(value: str) -> str:
		for article in ("a pair of ", "an ", "a ", "the "):
			if value.startswith(article):
				return value[len(article):]
		return value

	def craft_verb(item: Item) -> str:
		if item.family == "Jewellery & devotional":
			return "make"
		return "build" if item.subfamily == "door or gate" else "forge"

	used_names: dict[str, int] = {}
	craft_names: list[tuple[str, str]] = []
	for item in sorted(all_items, key=lambda value: value.stable_reference):
		base_name = f"{craft_verb(item)} {strip_article(item.sdesc)}"
		count = used_names.get(base_name, 0) + 1
		used_names[base_name] = count
		craft_names.append((item.stable_reference, base_name if count == 1 else f"{base_name} pattern {count}"))

	lines = [
		"// <auto-generated>",
		"// Generated by scripts/generate-renaissance-earlymodern-jewellery-doors.py from the complete catalogue dependency set.",
		"// Do not edit this file by hand.",
		"// </auto-generated>",
		"#nullable enable",
		"",
		"using System;",
		"using System.Collections.Generic;",
		"using System.Linq;",
		"",
		"namespace DatabaseSeeder.Seeders;",
		"",
		"public partial class ItemSeeder",
		"{",
	]
	lines.extend(collection("RenaissanceEarlyModernJewelleryDoorsRequiredMaterials", materials))
	lines.extend(collection("RenaissanceEarlyModernJewelleryDoorsRequiredTags", tags))
	lines.extend(collection("RenaissanceEarlyModernJewelleryDoorsRequiredComponents", components))
	lines.extend([
		"\tprivate static readonly IReadOnlyDictionary<string, string> RenaissanceEarlyModernJewelleryDoorsCraftNamesByStableReference =",
		"\t\tnew Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)",
		"\t\t{",
		*[f"\t\t\t[{cs(stable_reference)}] = {cs(craft_name)}," for stable_reference, craft_name in craft_names],
		"\t\t};",
		"",
	])
	lines.extend([
		"\tprivate const string RenaissanceEarlyModernJewelleryDoorsPrerequisiteRerunGuidance =",
		"\t\t\"Rerun UsefulSeeder and the item-component seeder until every listed material, tag, and component is available, then rerun Items.\";",
		"",
		"\tprivate void ValidateRenaissanceEarlyModernJewelleryDoorsPrerequisites()",
		"\t{",
		"\t\tvar issues = ValidateRenaissanceEarlyModernJewelleryDoorsDependencies(_materials.Keys, _tagsByFullPath.Keys, _components.Keys);",
		"\t\tif (issues.Count > 0)",
		"\t\t{",
		"\t\t\tthrow new InvalidOperationException(\"Renaissance and Early Modern jewellery and door prerequisites are incomplete; no catalogue item stage has been written.\" +",
		"\t\t\t\tEnvironment.NewLine + string.Join(Environment.NewLine, issues.Select(x => $\" - {x}\")) +",
		"\t\t\t\tEnvironment.NewLine + RenaissanceEarlyModernJewelleryDoorsPrerequisiteRerunGuidance);",
		"\t\t}",
		"\t}",
		"",
		"\tprivate static IReadOnlyList<string> ValidateRenaissanceEarlyModernJewelleryDoorsDependencies(",
		"\t\tIEnumerable<string> materials, IEnumerable<string> tags, IEnumerable<string> components)",
		"\t{",
		"\t\tvar issues = new List<string>();",
		"\t\tAddMissingRenaissanceEarlyModernJewelleryDoorsDependencies(\"material\", RenaissanceEarlyModernJewelleryDoorsRequiredMaterials, materials, issues);",
		"\t\tAddMissingRenaissanceEarlyModernJewelleryDoorsDependencies(\"tag\", RenaissanceEarlyModernJewelleryDoorsRequiredTags, tags, issues);",
		"\t\tAddMissingRenaissanceEarlyModernJewelleryDoorsDependencies(\"seeded component\", RenaissanceEarlyModernJewelleryDoorsRequiredComponents, components, issues);",
		"\t\treturn issues;",
		"\t}",
		"",
		"\tprivate static void AddMissingRenaissanceEarlyModernJewelleryDoorsDependencies(string dependencyType, IEnumerable<string> required, IEnumerable<string> available, ICollection<string> issues)",
		"\t{",
		"\t\tvar availableSet = available.ToHashSet(StringComparer.OrdinalIgnoreCase);",
		"\t\tforeach (var name in required.Where(x => !availableSet.Contains(x)))",
		"\t\t{",
		"\t\t\tissues.Add($\"Missing {dependencyType}: {name}\");",
		"\t\t}",
		"\t}",
		"",
		"\tinternal static IReadOnlyList<string> RenaissanceEarlyModernJewelleryDoorsMaterialsForTesting => RenaissanceEarlyModernJewelleryDoorsRequiredMaterials;",
		"\tinternal static IReadOnlyList<string> RenaissanceEarlyModernJewelleryDoorsTagsForTesting => RenaissanceEarlyModernJewelleryDoorsRequiredTags;",
		"\tinternal static IReadOnlyList<string> RenaissanceEarlyModernJewelleryDoorsComponentsForTesting => RenaissanceEarlyModernJewelleryDoorsRequiredComponents;",
		"\tinternal static IReadOnlyDictionary<string, string> RenaissanceEarlyModernJewelleryDoorsCraftNamesForTesting => RenaissanceEarlyModernJewelleryDoorsCraftNamesByStableReference;",
		"\tinternal static IReadOnlyList<string> ValidateRenaissanceEarlyModernJewelleryDoorsDependenciesForTesting(IEnumerable<string> materials, IEnumerable<string> tags, IEnumerable<string> components) => ValidateRenaissanceEarlyModernJewelleryDoorsDependencies(materials, tags, components);",
		"}",
		"",
	])
	return "\n".join(lines)


def csv_rows(items: Iterable[Item]) -> Iterable[dict[str, str]]:
	for item in items:
		yield {
			"unique_reference": item.stable_reference,
			"owner_era": item.owner,
			"availability": item.availability,
			"family": item.family,
			"subfamily": item.subfamily,
			"culture_admission": item.culture,
			"noun": item.noun,
			"sdesc": item.sdesc,
			"material": item.material,
			"size": item.size,
			"quality": item.quality,
			"weight_g": str(item.weight),
			"cost": str(item.cost),
			"tags": "; ".join(item.tags),
			"components": "; ".join(item.components),
			"craft_category": item.craft_category,
			"craft_trait": item.craft_trait,
			"builder_notes": item.builder_notes,
		}


CSV_FIELDS = (
	"unique_reference", "owner_era", "availability", "family", "subfamily", "culture_admission", "noun",
	"sdesc", "material", "size", "quality", "weight_g", "cost", "tags", "components", "craft_category",
	"craft_trait", "builder_notes",
)


def write_csv(path: Path, items: Iterable[Item]) -> None:
	with path.open("w", encoding="utf-8", newline="") as stream:
		writer = csv.DictWriter(stream, fieldnames=CSV_FIELDS)
		writer.writeheader()
		writer.writerows(csv_rows(items))


def write_fdesc_csv(path: Path, items: Iterable[Item]) -> None:
	with path.open("w", encoding="utf-8", newline="") as stream:
		writer = csv.writer(stream)
		writer.writerow(("unique_reference", "sdesc", "fdesc"))
		for item in items:
			writer.writerow((item.stable_reference, item.sdesc, item.fdesc))


def design_reference(title: str, item_count: int, jewellery_count: int, door_count: int, owner: str) -> str:
	return f"""# {title}

## Scope

This is the canonical source contract for {item_count:,} stock item prototypes owned by the {owner} item-seeder branch. It covers {jewellery_count:,} jewellery and devotional forms plus {door_count:,} doors, locks, gates, latches, keys, and fittings. The companion CSV and FDesc CSV are authoritative for every literal `CreateItem(...)` call.

## Authoring rules

- Stable references are lowercase product identifiers with no content-pass or duplicated-segment labels.
- Public descriptions describe visible form, material, fitting, finish, and wear. They do not make unsupported mechanical, magical, or sacred-effect claims.
- Jewellery uses one supported wearable profile plus one destruction profile. Devotional fixtures use only actually supported portable or fixed composition.
- Doors and gates use the exact `Door_*` profiles; loose locks, keys, and latches use only the supplied warded and latch components. No row claims custom key pairing.
- Quality expresses workmanship; cost, mass, material, tags, and component profiles are source data rather than inferred from a display name.

## Culture and admission

Culture is maintained in the catalogue admission field and builder note. Renaissance rows with a `Renaissance / Early Modern` availability value are seeded once under Renaissance ownership and installed by the Early Modern dispatcher as explicit earlier-era admissions. The `preindustrial_*` rows are genuine four-era stock only.

## Validation

`scripts/generate-renaissance-earlymodern-jewellery-doors.py --check` verifies counts, unique references, C# output, and both CSV companions. Seeder tests additionally verify the stable-reference, dependency, culture-admission, direct-craft, and description contracts.
"""


def admission_ledger(items: dict[str, list[Item]]) -> str:
	preindustrial = items["preindustrial"]
	common = items["renaissance_common"]
	return "\n".join((
		"# Renaissance and Early Modern Jewellery and Door Admission Ledger",
		"",
		"This ledger records the deliberate reuse boundary for the catalogue. It is generated alongside the source catalogues.",
		"",
		"| Ownership layer | Jewellery & devotional | Doors, locks & gates | Installed eras |",
		"|---|---:|---:|---|",
		f"| Pre-industrial shared | {sum(x.family == 'Jewellery & devotional' for x in preindustrial)} | {sum(x.family == 'Doors, locks & gates' for x in preindustrial)} | Antiquity, Medieval, Renaissance, Early Modern |",
		f"| Renaissance-owned common | {sum(x.family == 'Jewellery & devotional' for x in common)} | {sum(x.family == 'Doors, locks & gates' for x in common)} | Renaissance, Early Modern |",
		"| Renaissance cultural/institutional | 720 | 670 | Renaissance |",
		"| Early Modern cultural/institutional | 720 | 670 | Early Modern |",
		"",
		"The common layer is not a cosmetic clone layer: entries remain shared only where the form, material, component profile, and institutional role do not change. Culture-specific product forms remain era-owned and are documented in their source catalogues.",
		"",
))


def files(items: dict[str, list[Item]]) -> dict[Path, str]:
	preindustrial = items["preindustrial"]
	common = items["renaissance_common"]
	renaissance = items["renaissance_specific"]
	earlymodern = items["earlymodern_specific"]
	return {
		SEEDERS / "ItemSeeder.RenaissanceEarlyModern.JewelleryDoors.Validation.Generated.cs": render_dependency_validation_file(items),
		SEEDERS / "ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs": render_file(
			"SeedSharedPreIndustrialJewelleryAndDoorHardware", preindustrial,
			"the pre-industrial jewellery and door catalogue",
		),
		SEEDERS / "ItemSeeder.RenaissanceEarlyModern.CommonJewelleryDoors.Generated.cs": render_file(
			"SeedRenaissanceEarlyModernCommonJewelleryAndDoors", common,
			"the Renaissance-owned common catalogue",
		),
		SEEDERS / "ItemSeeder.Renaissance.JewelleryDevotional.Generated.cs": render_file(
			"SeedRenaissanceJewelleryAndDevotionalGoods",
			[item for item in renaissance if item.family == "Jewellery & devotional"],
			"the Renaissance jewellery and devotional catalogue",
		),
		SEEDERS / "ItemSeeder.Renaissance.DoorsLocksGates.Generated.cs": render_file(
			"SeedRenaissanceDoorsLocksAndGates",
			[item for item in renaissance if item.family == "Doors, locks & gates"],
			"the Renaissance doors, locks and gates catalogue",
		),
		SEEDERS / "ItemSeeder.EarlyModern.JewelleryDevotional.Generated.cs": render_file(
			"SeedEarlyModernJewelleryAndDevotionalGoods",
			[item for item in earlymodern if item.family == "Jewellery & devotional"],
			"the Early Modern jewellery and devotional catalogue",
		),
		SEEDERS / "ItemSeeder.EarlyModern.DoorsLocksGates.Generated.cs": render_file(
			"SeedEarlyModernDoorsLocksAndGates",
			[item for item in earlymodern if item.family == "Doors, locks & gates"],
			"the Early Modern doors, locks and gates catalogue",
		),
	}


def catalogue_paths(items: dict[str, list[Item]]) -> list[tuple[Path, Path, Path, list[Item], str]]:
	preindustrial = items["preindustrial"]
	common = items["renaissance_common"]
	renaissance = items["renaissance_specific"]
	earlymodern = items["earlymodern_specific"]
	return [
		(
			SEEDING / "FutureMUD_PreIndustrial_Jewellery_Doors_Item_Catalogue.csv",
			SEEDING / "FutureMUD_PreIndustrial_Jewellery_Doors_FDesc_Catalogue.csv",
			SEEDING / "FutureMUD_PreIndustrial_Jewellery_Doors_Design_Reference.md", preindustrial,
			"Pre-Industrial Jewellery, Devotional, Doors, Locks and Gates Design Reference",
		),
		(
			SEEDING / "FutureMUD_Renaissance_Jewellery_Devotional_Item_Catalogue.csv",
			SEEDING / "FutureMUD_Renaissance_Jewellery_Devotional_FDesc_Catalogue.csv",
			SEEDING / "FutureMUD_Renaissance_Jewellery_Devotional_Seeder_Design_Reference.md",
			[item for item in common + renaissance if item.family == "Jewellery & devotional"],
			"FutureMUD Renaissance Jewellery and Devotional Seeder Design Reference",
		),
		(
			SEEDING / "FutureMUD_Renaissance_Doors_Locks_Gates_Item_Catalogue.csv",
			SEEDING / "FutureMUD_Renaissance_Doors_Locks_Gates_FDesc_Catalogue.csv",
			SEEDING / "FutureMUD_Renaissance_Doors_Locks_Gates_Seeder_Design_Reference.md",
			[item for item in common + renaissance if item.family == "Doors, locks & gates"],
			"FutureMUD Renaissance Doors, Locks and Gates Seeder Design Reference",
		),
		(
			SEEDING / "FutureMUD_EarlyModern_Jewellery_Devotional_Item_Catalogue.csv",
			SEEDING / "FutureMUD_EarlyModern_Jewellery_Devotional_FDesc_Catalogue.csv",
			SEEDING / "FutureMUD_EarlyModern_Jewellery_Devotional_Seeder_Design_Reference.md",
			[item for item in earlymodern if item.family == "Jewellery & devotional"],
			"FutureMUD Early Modern Jewellery and Devotional Seeder Design Reference",
		),
		(
			SEEDING / "FutureMUD_EarlyModern_Doors_Locks_Gates_Item_Catalogue.csv",
			SEEDING / "FutureMUD_EarlyModern_Doors_Locks_Gates_FDesc_Catalogue.csv",
			SEEDING / "FutureMUD_EarlyModern_Doors_Locks_Gates_Seeder_Design_Reference.md",
			[item for item in earlymodern if item.family == "Doors, locks & gates"],
			"FutureMUD Early Modern Doors, Locks and Gates Seeder Design Reference",
		),
	]


def expected_files(items: dict[str, list[Item]]) -> dict[Path, str]:
	result = files(items)
	for csv_path, fdesc_path, design_path, catalogue_items, title in catalogue_paths(items):
		from io import StringIO
		csv_stream = StringIO()
		writer = csv.DictWriter(csv_stream, fieldnames=CSV_FIELDS, lineterminator="\n")
		writer.writeheader()
		writer.writerows(csv_rows(catalogue_items))
		result[csv_path] = csv_stream.getvalue()
		fdesc_stream = StringIO()
		fdesc_writer = csv.writer(fdesc_stream, lineterminator="\n")
		fdesc_writer.writerow(("unique_reference", "sdesc", "fdesc"))
		for item in catalogue_items:
			fdesc_writer.writerow((item.stable_reference, item.sdesc, item.fdesc))
		result[fdesc_path] = fdesc_stream.getvalue()
		result[design_path] = design_reference(
			title, len(catalogue_items),
			sum(item.family == "Jewellery & devotional" for item in catalogue_items),
			sum(item.family == "Doors, locks & gates" for item in catalogue_items),
			catalogue_items[0].owner,
		)
	result[SEEDING / "FutureMUD_Renaissance_EarlyModern_Jewellery_Doors_Admission_Ledger.md"] = admission_ledger(items)
	return result


def main() -> int:
	parser = argparse.ArgumentParser()
	parser.add_argument("--check", action="store_true", help="verify all generated catalogue outputs")
	args = parser.parse_args()
	items = catalogues()
	outputs = expected_files(items)
	if args.check:
		outdated = [path for path, content in outputs.items() if not path.exists() or path.read_text(encoding="utf-8") != content]
		if outdated:
			for path in outdated:
				print(path.relative_to(ROOT))
			return 1
		return 0
	for path, content in outputs.items():
		path.write_text(content, encoding="utf-8", newline="")
	return 0


if __name__ == "__main__":
	raise SystemExit(main())
