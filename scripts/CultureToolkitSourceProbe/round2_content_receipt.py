#!/usr/bin/env python3
"""Render round-two authored counts and actual disposable MySQL receipts. No database writes."""
from pathlib import Path
import hashlib
import json
import subprocess

ROOT = Path(__file__).resolve().parents[2]
INPUT = ROOT / "Design Documents/Seeding/CultureSeederRedesignHandoff"
OUTPUT = ROOT / "Design Documents/Verification"
EVIDENCE = OUTPUT / "CultureSeederRound2"
ERAS = ["antiquity", "darkages", "medieval", "renaissance", "earlymodern"]


def load(path):
    return json.loads(path.read_text(encoding="utf-8-sig"))


def main():
    corpus = load(INPUT / "data/targeted_name_corpora.json")
    original = json.loads(subprocess.check_output([
        "git", "show", "2bbcdff3ed52e200ef17faa61dc3af0d9e9f3b86:Design Documents/Seeding/CultureSeederRedesignHandoff/data/targeted_name_corpora.json"
    ], cwd=ROOT))
    pools = {p["key"]: p for p in corpus["pools"]}
    old_pools = {p["key"]: p for p in original["pools"]}
    revised = {e["key"]: e for p in pools.values() for g in ("male", "female") for e in p["given_" + g]}
    old = {e["key"]: e for p in old_pools.values() for g in ("male", "female") for e in p["given_" + g]}
    assert len(old) == 389 and len(revised) == 585
    for key, entry in old.items():
        for field, value in entry.items():
            assert revised[key][field] == value, (key, field)
    lines = ["# CultureSeeder round-two content receipt", "",
             "Original evidence comparison against PR #732: **389/389 entries preserve every original field and value**. "
             "The runtime corpus has **585 entries**, including **196 authored additions**. "
             "Reconstructed full forms do not claim attestation. Botezata remains in evidence and has no playable birth-name era.", "",
             "## Playable stock naming", "",
             "Counts are post-era-filtered authored stock; builder deletions are not reversed to maintain this floor. "
             "The before column uses the PR #732 gameplay filter. Evidence classes describe the original entry classification, not a certification of historicity.", "",
             "| Pool | Era | Gender | Before | Families now | Displays now | Evidence classes |",
             "|---|---|---|---:|---:|---:|---|"]
    requirements = load(INPUT / "data/name_playability_policy.json")["requirements"]
    assert len(requirements) == 58
    for r in requirements:
        pool, era, gender = r["pool"], r["pack"], r["gender"]
        entries = [e for e in pools[pool]["given_" + gender] if era in e.get("playable_packs", e["packs"])]
        before = [e for e in old_pools[pool]["given_" + gender] if era in old_pools[pool]["packs"] and era in e["packs"]]
        families = len({e["family_key"].casefold() for e in entries})
        displays = len({e["display"].casefold() for e in entries})
        assert families >= r["minimum_distinct_families"] and displays == len(entries)
        classes = {kind: sum(e["evidence_type"] == kind for e in entries) for kind in sorted({e["evidence_type"] for e in entries})}
        lines.append(f"| {pool} | {era} | {gender} | {len(before)} | {families} | {displays} | " + "; ".join(f"{k}: {v}" for k, v in classes.items()) + " |")
    lines += ["", "## Actual clean-era imports", "",
              "IDs in the following receipts belong to the named disposable local MySQL databases. "
              "Each successful receipt includes the complete native-binding outcomes and one detail row per imported/generated accent, with stable identity, canonical language, policy, era mask, role, original/effective predicate, default status and overrides.", "",
              "| Era | Database | Status | Social cultures | Native bindings resolved | Accent rows | Out-of-era rows |",
              "|---|---|---|---:|---:|---:|---:|"]
    reports = {}
    for era in ERAS:
        path = EVIDENCE / f"{era}-live.json"
        if not path.exists():
            lines.append(f"| {era} | — | not run | — | — | — | — |")
            continue
        data = load(path)
        report = data.get("Report", {})
        if data["Status"] == "passed":
            reports[era] = report
        accents = [a for language in report.get("Accents", []) for a in language.get("Details", [])]
        resolved = sum(x["IsResolved"] for x in report.get("NativeBindings", []))
        lines.append(f"| [{era}](CultureSeederRound2/{era}-live.json) | `{data['Database']}` | {data['Status']} | "
                     f"{len(report.get('CultureIds', {}))} | {resolved} / {len(report.get('NativeBindings', []))} | {len(accents)} | "
                     f"{sum(not a['EraEligible'] for a in accents)} |")
    lines += ["", "## Exact source-native additions", "", "| Source identity | Era | Supplied references | Actual language IDs | Rule |", "|---|---|---|---|---|"]
    for rule in load(INPUT / "data/legacy_native_language_rules.json")["exact_source_bindings"]:
        for era in rule["packs"]:
            actual = next((b for b in reports.get(era, {}).get("NativeBindings", []) if b["SourceIdentity"] == rule["source_identity"]), None)
            if era in reports:
                assert actual and actual["IsResolved"], (era, rule["key"])
            lines.append(f"| {rule['source_identity']} | {era} | {', '.join(rule['languages'])} | "
                         f"{', '.join(map(str, actual['LanguageIds'])) if actual else 'not verified'} | {rule['key']} |")
    lines += ["", "## Antiquity source identities", "", "| Source identity | Resolved | References | Language IDs |", "|---|---|---|---|"]
    ancient = reports.get("antiquity", {}).get("NativeBindings", [])
    if ancient:
        assert len(ancient) == 56 and all(b["IsResolved"] for b in ancient)
    for b in ancient:
        lines.append(f"| {b['SourceIdentity']} | {b['IsResolved']} | {', '.join(b['References'])} | {', '.join(map(str, b['LanguageIds']))} |")
    lines += ["", "## Builder deviations and script graph", "",
              "The [MySQL builder-rerun receipt](CultureSeederRound2/builder-rerun.json) records the deleted/added script language IDs, "
              "actual custom linked trait, surviving graph, deleted/edited/custom names, overridden accent and both full installer reports. "
              "Edits are rolled back after verification. Runtime script eligibility is separately executed in `CultureToolkitScriptSeederTests`.", "",
              "## Runtime input hashes", "", "| Input | SHA-256 |", "|---|---|"]
    for folder in ("data", "research"):
        for path in sorted((INPUT / folder).glob("*.json")):
            lines.append(f"| {path.relative_to(INPUT).as_posix()} | `{hashlib.sha256(path.read_bytes()).hexdigest()}` |")
    (OUTPUT / "CultureSeeder_Round2_Content_Receipt.md").write_text("\n".join(lines) + "\n", encoding="utf-8")


if __name__ == "__main__":
    main()
