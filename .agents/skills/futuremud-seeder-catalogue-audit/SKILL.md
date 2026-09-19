---
name: futuremud-seeder-catalogue-audit
description: Audit and refresh FutureMUD seeded catalogue artifacts from live source truth. Use when the user asks to ensure seeded JSON, TSV, material, liquid, gas, terrain, tag, item-component, stock-content, or similar DatabaseSeeder inventory files are up to date, complete, source-backed, or exported with exact formatting.
---

# FutureMUD Seeder Catalogue Audit

## Overview

Use this skill when a FutureMUD seeded catalogue or export must reflect current seeder/runtime behavior rather than stale hand-maintained data.

The user's usual intent is a source-backed completeness pass, not a narrow edit to the currently visible artifact.

## Workflow

1. Identify the catalogue contract.
   - Determine whether the requested artifact is JSON, TSV, documentation, seeded source, or a generated export.
   - Preserve requested column names, hierarchy strings, casing, sort/order, and file location exactly unless the user asks to change them.

2. Find every source of truth before editing.
   - Search sibling seeders and runtime singleton/system registrations, not only the named file.
   - For source families, pitfalls, and the complete component/liquid/gas/solid seeder source map, read `references/futuremud-catalogue-checks.md`.

3. Decide whether the artifact is maintained or generated.
   - If maintained, preserve reviewable ordering and insert/update only the needed entries.
   - If generated/exported, prefer deterministic regeneration from source definitions.

4. Reconcile with first-definition semantics.
   - Many seeder helpers are duplicate-safe; treat the first live creation as authoritative unless runtime repair logic later renames or updates it.
   - Do not invent rows from naming patterns alone. Tie every row to source, runtime, or explicit user direction.

5. Validate invariants.
   - Check duplicates by stable key.
   - Check missing expected rows from the source audit.
   - Check stale rows that no longer map to live source.
   - Check exact format constraints such as JSON parseability, tab-delimited column count, sorted sections, or preserved source order.

6. Add or update focused tests when the catalogue is part of seeded behavior.
   - Prefer source/invariant tests for seeders and template tests for stock catalogue entries.
   - When checking large source strings, prefer `Assert.IsTrue(source.Contains(...))` over assertion helpers that format huge failure messages poorly.

7. Verify and report.
   - Run the focused DatabaseSeeder tests or the relevant project tests when practical.
   - Run `git diff --check` on edited artifacts.
   - Report counts, missing/duplicate/stale checks, and any sibling catalogue that was inspected but did not need changes.

## Common Commands

Use fast source search first:

```powershell
rg "Ensure.*Tag|Seed.*Tags|GameItemComponentProto|SeedMaterials|SeedLiquids|BreathableAtmosphere" DatabaseSeeder MudSharpCore FutureMUDLibrary
rg "\.GameItemComponentProtos\.Add\(|new\s+GameItemComponentProto\b|\.Liquids\.Add\(|new\s+Liquid\b|\.Gases\.Add\(|new\s+Gas\b|\.Materials\.Add\(|new\s+Material\b" DatabaseSeeder/Seeders -g "*.cs"
```

Use the repo's Windows test guidance:

```powershell
dotnet restore MudSharp.sln -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false
dotnet test 'DatabaseSeeder Unit Tests\DatabaseSeeder Unit Tests.csproj' -c Debug --no-restore -m:1
git diff --check -- <changed-file>
```

## Recovery Rules

If a mechanical rewrite damages a maintained catalogue, rebuild from the `HEAD` version plus the live source audit instead of trying to patch corrupted output by hand.

If a parser undercounts source calls, inspect the source shape and rerun the extraction with a corrected parser before trusting the output.

If a sibling catalogue could plausibly share the same omission, inspect it and report whether it changed.