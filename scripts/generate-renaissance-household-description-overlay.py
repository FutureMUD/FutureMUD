#!/usr/bin/env python3
"""Create the curated-description overlay for the Renaissance household catalogue."""

from __future__ import annotations

import argparse
import runpy
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "scripts/generate-renaissance-household-manifest.py"
OUTPUT = ROOT / "Design Documents/Seeding/FutureMUD_Renaissance_Household_Description_Overlay.tsv"


def articleless(sdesc: str) -> str:
	for article in ("a ", "an ", "the "):
		if sdesc.casefold().startswith(article):
			return sdesc[len(article):]
	return sdesc


def family_for(stable_reference: str) -> str:
	return stable_reference.split("_", 2)[1]


def material_detail(material: str) -> str:
	if material in {"oak", "walnut", "cedar", "teak", "pine", "cypress", "mahogany", "ash", "bamboo", "rattan", "boxwood"}:
		return f"The {material} grain is visible along its boards and braces, with the exposed edges burnished smooth."
	if material in {"brass", "copper", "pewter", "wrought iron", "silver", "gold"}:
		return f"The {material} is worked into shallow planes and firm edges that catch the light without appearing machine-finished."
	if material in {"porcelain", "stoneware", "earthenware"}:
		return f"Its fired {material} body has a hard, even surface with a slightly thickened rim and foot."
	if material in {"glass", "soda-lime glass", "lead glass"}:
		return f"The {material} is gathered more thickly at the rim and base, giving the form a deliberate hand-made weight."
	if material in {"leather", "linen", "silk", "paper"}:
		return f"The {material} is cut neatly and secured wherever it meets the reinforced edges of the form."
	return f"The {material} surface is worked into clear planes and finished carefully around the principal edges."


def component_detail(components: tuple[str, ...]) -> str:
	if any(component.startswith("LContainer_") for component in components):
		return "A smoothed rim and weighted base keep the vessel stable during repeated service."
	if any(component.startswith("LockingContainer_") for component in components):
		return "A close-fitting lid or door meets a plainly mounted lock plate, with tight joins around the compartment."
	if any(component.startswith("Container_") for component in components):
		return "Its compartment is bordered by fitted panels, open shelving, or a lined interior, with the accessible edges worn smooth."
	if any(component.startswith("Table_") for component in components):
		return "The working top is carried on a braced base that leaves a clear surface above and sturdy supports below."
	if any(component.startswith(("Chair_", "Bench_")) for component in components):
		return "Its seat and supporting rails are set at a practical height, with contact edges rounded by use."
	if any(component.startswith("Wear_") for component in components):
		return "A narrow fastening or carrying point sits close to the body, keeping the compact form easy to secure."
	if any(component.startswith("Dice_") for component in components):
		return "The small faces are evenly cut and marked, while the corners are softened enough to sit comfortably in the hand."
	return "Its principal fittings are clearly visible, with the useful parts of the form left free of excessive decoration."


def form_detail(sdesc: str, components: tuple[str, ...]) -> str:
	words = sdesc.casefold()
	if any(word in words for word in ("wardrobe", "armoire", "cabinet", "cupboard", "bookcase", "press")):
		return "The upright body is broken into doors, panels, or shelves, with a firm plinth keeping the stored contents above the floor."
	if any(word in words for word in ("chest", "coffer", "box", "caddy", "lockbox", "case", "purse", "satchel")):
		return "A close-fitting top or flap sits over the compact compartment, while the corners are reinforced for regular handling."
	if any(word in words for word in ("table", "desk", "counter", "escritoire")):
		return "The working surface is carried on a braced base, leaving a clear top above and sturdy supports below."
	if any(word in words for word in ("bench", "chair", "stool")):
		return "Its seat and supporting rails are set at a practical height, with contact edges rounded by repeated use."
	if any(word in words for word in ("bottle", "bowl", "pot", "tankard", "ewer", "jar", "cup", "glass", "flask")):
		return "A smoothed rim and weighted base give the vessel a stable, practical form for repeated domestic service."
	if any(word in words for word in ("rack", "stand", "plinth", "display")):
		return "The raised supporting faces are kept level and open, allowing the displayed form to remain plainly visible."
	if "screen" in words:
		return "Its framed panels are linked in a light, folding run, with the lower edges kept broad enough to stand securely."
	if "mirror" in words:
		return "The reflective pane is held within a narrow protective frame, with the joint kept even around the visible face."
	if "clock" in words:
		return "The tall case frames its dial opening and lower panel in a balanced vertical arrangement, with the visible joinery kept neat."
	if "pipe" in words:
		return "The bowl and stem are proportioned as a single light form, with the handling end kept smooth against the fingers."
	if "dice" in words:
		return "The small faces are evenly cut and marked, while the corners are softened enough to sit comfortably in the hand."
	if "card" in words:
		return "The packet is cut to even edges and tied into a compact stack, with the outer faces kept flat and protected."
	if "board" in words:
		return "The broad face is bordered by a simple frame, leaving its marks and divisions clear from across a furnished room."
	return component_detail(components)


def full_description(stable_reference: str, sdesc: str, material: str, components: tuple[str, ...]) -> str:
	item = articleless(sdesc)
	stable_words = stable_reference.replace("renaissance_", "").split("_")
	finish = (
		"Small tool marks remain at the less-visible edges, while the parts handled most often have been polished smooth.",
		"The visible faces are finished with restraint, allowing the material and its construction to remain easy to read.",
		"Subtle changes in sheen around the joins make the separate pieces and their careful fitting visible.",
		"Its corners show small signs of fitting and repair, lending the surface a convincing history of use."
	)[sum(ord(letter) for letter in stable_reference) % 4]
	context = " ".join(stable_words[1:4]).replace("-", " ") or family_for(stable_reference)
	finish = finish[:-1] + f", in keeping with the {context} setting."
	return f"The {item} is built chiefly from {material}, with a deliberate silhouette and carefully fitted edges. {material_detail(material)} {form_detail(sdesc, components)} {finish}"


def long_description(sdesc: str, material: str, components: tuple[str, ...]) -> str:
	if "Holdable" in components:
		return ""
	return f"{sdesc.capitalize()} is positioned here, its {material} surfaces contributing a practical presence to the room."


def render() -> str:
	module = runpy.run_path(str(SOURCE))
	base = module["parse_base"](module["base_components"]())
	expansion = module["parse_expansion"](module["expansion_components"]())
	rows = []
	for stable, _, sdesc, _, _, _, _, _, material, _, components, _ in base + expansion:
		rows.append("\t".join((stable, long_description(sdesc, material, components), full_description(stable, sdesc, material, components))))
	if len(rows) != 1000:
		raise ValueError(f"Expected 1,000 Renaissance household descriptions, found {len(rows)}")
	return "StableReference\tLongDescription\tFullDescription\n" + "\n".join(rows) + "\n"


def main() -> int:
	parser = argparse.ArgumentParser()
	parser.add_argument("--check", action="store_true")
	args = parser.parse_args()
	content = render()
	if args.check:
		if not OUTPUT.exists() or OUTPUT.read_text(encoding="utf-8") != content:
			print(f"Renaissance household description overlay is stale: {OUTPUT.relative_to(ROOT)}")
			return 1
		return 0
	OUTPUT.write_text(content, encoding="utf-8", newline="\n")
	print(f"Wrote {OUTPUT.relative_to(ROOT)}")
	return 0


if __name__ == "__main__":
	raise SystemExit(main())
