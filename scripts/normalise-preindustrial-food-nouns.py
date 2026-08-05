#!/usr/bin/env python3
"""Keep food catalogue nouns aligned with the head noun in each short description."""

from __future__ import annotations

import argparse
import re
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
CATALOGUE_ROOT = ROOT / "DatabaseSeeder/Seeders/FoodCatalogue"
EXPECTED_HEADER = (
	"stable_reference\tscope\tkind\tfamily\tnoun\tshort_description\tfull_description\ttaste\tmaterial\t"
	"nutrition\tfreshness\tquality\tweight_grams\tcost\tadmission_profile"
)
LEADING_ARTICLE = re.compile(r"^(?:a|an|the|some)\s+", re.IGNORECASE)
PREPOSITIONS = (
	" with ", " in ", " under ", " over ", " for ", " from ", " on ", " beside ", " alongside ",
	" around ", " through ", " upon ", " into ", " atop ", " across ",
)
TRAILING_VERBS = {
	"baked", "bound", "braised", "cut", "dressed", "filled", "folded", "fried", "glazed", "grilled",
	"moulded", "packed", "poached", "roasted", "served", "set", "shaped", "soaked", "steamed", "stewed",
	"stuffed", "topped", "wrapped",
}


def clean_token(value: str) -> str:
	return value.strip(".,;:!?()[]{}\"\u201c\u201d").casefold()


def head_noun(short_description: str) -> str:
	phrase = LEADING_ARTICLE.sub("", short_description.strip())
	if not phrase:
		raise ValueError("Short description has no words")

	lower = phrase.casefold()
	if " of " in lower:
		phrase = phrase[: lower.index(" of ")]
	else:
		cut = len(phrase)
		for preposition in PREPOSITIONS:
			position = lower.find(preposition)
			if position >= 0:
				cut = min(cut, position)
		phrase = phrase[:cut]

	tokens = [clean_token(token) for token in phrase.split()]
	tokens = [token for token in tokens if token]
	if not tokens:
		raise ValueError(f"Cannot determine a noun from {short_description!r}")
	if len(tokens) > 1 and tokens[-1] in TRAILING_VERBS:
		return tokens[-2]
	return tokens[-1]


def normalized_content(path: Path) -> str:
	lines = path.read_text(encoding="utf-8-sig").splitlines()
	if not lines or lines[0] != EXPECTED_HEADER:
		raise ValueError(f"Unexpected food item header in {path.relative_to(ROOT)}")

	result = [EXPECTED_HEADER]
	for line_number, line in enumerate(lines[1:], 2):
		if not line or line.startswith("#"):
			result.append(line)
			continue
		cells = line.split("\t")
		if len(cells) != 15:
			raise ValueError(f"{path.relative_to(ROOT)} line {line_number} has {len(cells)} cells")
		cells[4] = head_noun(cells[5])
		result.append("\t".join(cells))
	return "\n".join(result) + "\n"


def main() -> int:
	parser = argparse.ArgumentParser()
	parser.add_argument("--check", action="store_true")
	args = parser.parse_args()

	paths = sorted(CATALOGUE_ROOT.rglob("*.food-items.tsv"))
	stale: list[Path] = []
	for path in paths:
		content = normalized_content(path)
		if path.read_text(encoding="utf-8-sig") == content:
			continue
		if args.check:
			stale.append(path)
		else:
			path.write_text(content, encoding="utf-8", newline="\n")

	if stale:
		print("Food catalogue nouns are stale: " + ", ".join(str(path.relative_to(ROOT)) for path in stale))
		return 1
	if args.check:
		print(f"Food catalogue nouns are current across {len(paths)} item files.")
	else:
		print(f"Normalised food catalogue nouns across {len(paths)} item files.")
	return 0


if __name__ == "__main__":
	raise SystemExit(main())
