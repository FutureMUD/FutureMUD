#!/usr/bin/env python3
"""Check that the original CultureSeeder JSON handoff files arrived intact.

Usage: python tools/check_json_inputs.py [path/to/handoff]
Uses only the Python standard library; does not modify any files.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import re
import sys
from pathlib import Path, PurePosixPath


def check_inputs(root: Path) -> int:
    manifest = root / "JSON_SHA256SUMS.txt"
    if not manifest.is_file():
        print(f"MISSING: {manifest}", file=sys.stderr)
        return 2
    expected: dict[str, str] = {}
    for number, line in enumerate(manifest.read_text(encoding="utf-8").splitlines(), 1):
        if not line.strip():
            continue
        parts = line.split(maxsplit=1)
        if len(parts) != 2 or not re.fullmatch(r"[0-9a-f]{64}", parts[0]):
            print(f"Invalid manifest line {number}.", file=sys.stderr)
            return 2
        relative = PurePosixPath(parts[1])
        if (relative.is_absolute() or ".." in relative.parts
                or relative.parts[0] not in {"data", "research"}
                or relative.suffix != ".json" or str(relative) in expected):
            print(f"Invalid or duplicate manifest path on line {number}.", file=sys.stderr)
            return 2
        expected[str(relative)] = parts[0]
    data_count = sum(name.startswith("data/") for name in expected)
    research_count = sum(name.startswith("research/") for name in expected)
    if (data_count, research_count) != (22, 4):
        print(f"Unexpected manifest coverage: {data_count} data and {research_count} research files.", file=sys.stderr)
        return 2
    failures = 0
    for relative, digest in sorted(expected.items()):
        target = root.joinpath(*PurePosixPath(relative).parts)
        try:
            raw = target.read_bytes()
            json.loads(raw.decode("utf-8"))
            if hashlib.sha256(raw).hexdigest() != digest:
                failures += 1
                print(f"CHANGED: {relative} (does not match the original handoff)")
            else:
                print(f"OK: {relative}")
        except FileNotFoundError:
            failures += 1
            print(f"MISSING: {relative}")
        except (OSError, UnicodeError, ValueError) as exc:
            failures += 1
            print(f"INVALID: {relative}: {exc}")
    print(f"\n{len(expected) - failures}/{len(expected)} JSON files present, parseable and checksum-matched.")
    return 1 if failures else 0


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("root", nargs="?", type=Path, default=Path(__file__).resolve().parents[1])
    args = parser.parse_args()
    try:
        return check_inputs(args.root.resolve())
    except OSError as exc:
        print(f"Cannot check handoff: {exc}", file=sys.stderr)
        return 2


if __name__ == "__main__":
    raise SystemExit(main())
