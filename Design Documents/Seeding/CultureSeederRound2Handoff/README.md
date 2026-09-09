# CultureSeeder round-two corrective handoff

## Entry point

Read `AGENT_TASK.md`, then `01_SCOPE_AND_FINDING_DISPOSITION.md`, `02_IMPLEMENTATION_BRIEF.md`, `03_CONTENT_CATALOGUE.md` and `04_ACCEPTANCE_AND_VERIFICATION.md`. All new content and policies are included under `data/`; evidence and the preservation ledger are under `research/`.

This is a corrective task for the implementation already in FutureMUD master at `2bbcdff3ed52e200ef17faa61dc3af0d9e9f3b86` (PR #732). Generic skill groups are already implemented by PR #730. Do not repeat either initial implementation.

The maintained runtime handoff directory in that commit is **`Design Documents/Seeding/CultureSeederRedesignHandoff/`**. The earlier proposed `Design Documents/Culture Seeder Enhancement/` directory is not the runtime resource location. Read the actual `DatabaseSeeder.csproj` before placing inputs.

The single-file companion `FutureMUD_CultureSeeder_Round2_Implementation_Brief.md` contains the documentation and every file in this pack. Transferring that one Markdown file is sufficient: its checksum-verified embedded ZIP payload can be extracted using the included standard-library script. The ZIP is the convenient alternative.

## Deliverable boundary

The pack supplies implementation instructions, exact missing language mappings, an explicit accent-era policy, a full replacement eight-pool targeted-name JSON, regression cases and a validator. It does not contain compiled C# changes or assert that engine/database tests have already passed.

The original 26 resource documents are already in the repository and need not be reattached. Only the resources listed in section 1 of the implementation brief change. If the checkout is newer, reconcile by stable keys and report changed assumptions; do not silently discard later work.

## Validate this pack

```text
python tools/validate_handoff.py
```

This checks supplied data and checksums, not FutureProg execution or engine behaviour. Required engine and database verification is specified separately.
