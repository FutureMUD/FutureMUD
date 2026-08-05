#!/usr/bin/env python3
"""Generate the supported Early Modern military catalogue from its source ledger."""

from __future__ import annotations

import argparse
import json
import re
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "Design Documents/Seeding/FutureMUD_EarlyModern_Military_Firearms_Uniforms_Naval_Design_Reference.md"
COMPONENTS = ROOT / "Design Documents/Data/Seeded_Item_Components.json"
OUTPUT = ROOT / "DatabaseSeeder/Seeders/ItemSeeder.EarlyModern.MilitaryManifestData.Generated.cs"


def read(path: Path) -> list[str]:
	return path.read_text(encoding="utf-8-sig").splitlines()


def ticks(value: str) -> tuple[str, ...]:
	return tuple(re.findall(r"`([^`]+)`", value))


def profile_tags() -> dict[str, tuple[str, ...]]:
	profiles: dict[str, tuple[str, ...]] = {}
	for line in read(SOURCE):
		match = re.match(r"^\| `([^`]+)` \| (.+) \|$", line)
		if match and match.group(1).startswith("EM-"):
			profiles[match.group(1)] = leaf_tags(ticks(match.group(2)))
	if len(profiles) != 29:
		raise ValueError(f"Expected 29 Early Modern military tag profiles, found {len(profiles)}")
	return profiles


def leaf_tags(tags: tuple[str, ...]) -> tuple[str, ...]:
	"""Remove a hierarchy parent when its more-specific child is already assigned."""
	return tuple(
		tag for tag in tags
		if not any(other.startswith(f"{tag} / ") for other in tags)
	)


def available_components() -> set[str]:
	return {entry["Component Name"] for entry in json.loads(COMPONENTS.read_text(encoding="utf-8"))}


def parse_rows() -> tuple[list[tuple], list[tuple]]:
	profiles = profile_tags()
	available = available_components()
	supported = []
	deferred = []
	for line in read(SOURCE):
		if not line.startswith("| `earlymodern_"):
			continue
		cells = [cell.strip() for cell in line.strip().strip("|").split("|")]
		if len(cells) != 8:
			raise ValueError(f"Malformed Early Modern military catalogue row: {line}")
		stable, sdesc, noun, material, size_quality, weight_cost, components, profile = cells
		stable = ticks(stable)[0]
		noun = ticks(noun)[0]
		material = ticks(material)[0]
		size, quality = (ticks(value)[0] for value in size_quality.split("/"))
		weight, cost = (value.strip().removesuffix("g").removesuffix("m").replace(",", "") for value in weight_cost.split("/"))
		component_names = ticks(components)
		profile = ticks(profile)[0]
		if profile not in profiles:
			raise ValueError(f"Unknown tag profile {profile} for {stable}")
		row = (stable, noun, sdesc, material, size, quality, weight, cost, profiles[profile], component_names, profile)
		(supported if all(component in available for component in component_names) else deferred).append(row)
	if len(supported) + len(deferred) != 1664:
		raise ValueError("Early Modern military source should contain exactly 1,664 new rows")
	return supported, deferred


def cs(value: str) -> str:
	return json.dumps(value, ensure_ascii=False)


def description(stable: str, sdesc: str, noun: str, material: str, quality: str) -> str:
	"""Build substantive, product-specific stock prose from the canonical row fields.

	The catalogue deliberately keeps prose generated: every row's stable product/form,
	material, and quality remain the source of truth while avoiding implementation notes.
	"""
	form_words = [word for word in stable.removeprefix("earlymodern_military_").split("_")
		if word not in {"accessory", "armour", "armor", "artillery", "firearm", "melee", "military",
			"naval", "ranged", "tool", "issue", "reinforced", "ornate", "service"}]
	form = " ".join(form_words)
	if "armor" in stable or "armour" in stable:
		profile = f"The {form} arrangement is shaped to overlap and follow the body, leaving the edges and fastenings plainly visible."
	elif "shield" in stable:
		profile = f"The {form} arrangement gives it a broad face, clear rim, and readily visible hand fittings."
	elif "melee" in stable or "boarding" in stable:
		profile = f"The {form} arrangement balances the working end against a firm grip or shaft, giving the weapon a direct, martial line."
	elif "firearm" in stable or "ranged" in stable:
		profile = f"The {form} arrangement sets its stock, barrel, and small fittings in a compact, deliberate line."
	elif "artillery" in stable or "cannon" in stable or "gun" in stable:
		profile = f"The {form} arrangement uses heavy fittings and reinforced working surfaces for a stout, service-built appearance."
	elif "uniform" in stable or "coat" in stable or "sash" in stable:
		profile = f"The {form} cut is defined by its seams and visible fastenings, giving the garment a disciplined, formal appearance."
	else:
		profile = f"The {form} arrangement sets its fittings and working surfaces in a clear, practical pattern."
	quality_details = {
		"ExtremelyPoor": "roughly finished, with uneven edges and a neglected surface",
		"VeryPoor": "plainly made, with tool marks left visible across the surface",
		"Poor": "serviceable but roughly worked, with small irregularities at its joins",
		"Substandard": "workmanlike but spare, with a few coarse marks in the finish",
		"Standard": "plainly finished, with practical edges and uncomplicated fittings",
		"Good": "carefully finished, with clean joins and a restrained, even surface",
		"VeryGood": "finely finished, with crisp details and a deliberately polished surface",
		"Great": "expertly finished, with precise joins and a richly maintained surface",
		"Excellent": "exceptionally finished, with precise details and a lustrous, controlled surface",
		"Legendary": "lavishly finished, with immaculate details and a striking, ceremonial surface",
	}.get(quality, "carefully finished, with clean joins and an even surface")
	return (
		f"{sdesc.capitalize()} is fashioned chiefly from {material}, with the {noun} kept clear in its silhouette. "
		f"{profile} "
		f"The {material} is {quality_details}. "
		f"Close inspection picks out the proportions and joinery of this {form} pattern."
	)


def render(row: tuple) -> str:
	stable, noun, sdesc, material, size, quality, weight, cost, tags, components, profile = row
	builder_notes = cs(f"Early Modern military source profile {profile}; only currently supported component dependencies are seeded.")
	parts = [
		f'\t\tnew({cs(stable)}, {cs(noun)}, {cs(sdesc)}, BuildEarlyModernMilitaryDescription({cs(stable)}, {cs(sdesc)}, {cs(noun)}, {cs(material)}, ItemQuality.{quality}), ',
		f'SizeCategory.{size}, ItemQuality.{quality}, {weight}, {cost}m, {cs(material)}, ',
		f'[{", ".join(cs(tag) for tag in tags)}], [{", ".join(cs(component) for component in components)}], {builder_notes}),',
	]
	return "".join(parts)


def generate() -> str:
	supported, deferred = parse_rows()
	if not supported:
		raise ValueError("Expected supported Early Modern military rows")
	return "\n".join([
		"// <auto-generated>",
		"// Generated by scripts/generate-earlymodern-supported-military-manifest.py from the canonical Early Modern military reference and maintained component export.",
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
		"\tprivate static readonly EarlyModernMilitaryItemSpec[] EarlyModernSupportedMilitaryItemSpecs =",
		"\t[",
		*(render(row) for row in supported),
		"\t];",
		"",
		f"\tinternal const int EarlyModernMilitaryDeferredSourceRows = {len(deferred)};",
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
			print(f"Generated supported military manifest source is stale: {OUTPUT.relative_to(ROOT)}")
			return 1
		return 0
	OUTPUT.write_text(content, encoding="utf-8", newline="\n")
	print(f"Wrote {OUTPUT.relative_to(ROOT)}")
	return 0


if __name__ == "__main__":
	raise SystemExit(main())
