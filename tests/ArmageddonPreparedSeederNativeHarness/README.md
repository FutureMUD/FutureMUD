# Core foundation qualification

Build this harness in Debug, then run `Run-IsolatedPreparedSeeder.ps1 -Mode core-foundations -EvidenceRoot <absolute-new-output-path>`. The existing wrapper creates and verifies a unique loopback MySQL instance, database names, port, UUID and TEMP datadir. It owns and cleans only its own resources.

The scenario executes Core, Time, Attribute, SkillPackage (Armageddon model, preindustrial), Human, Combat (no firearms) and Chargen through the shared executor. It verifies no Useful packages, captures a new reduced base under `.artifacts/core-foundations-fixtures`, seals the SQL and descriptor read-only, and restores into a second owned database with matching schema/content hashes. It tests Core rerun ID/custom-setting preservation, full Armageddon opt-in installation, native target IDs, and the normal first/cold boot, Admin login, LOOK, save and graceful shutdown path.

To retry only later qualification after harness-only changes, supply `-BaseFixture <absolute-core-foundations.json> -BaseFixtureSha256 <exact-uppercase-sha256>`. Kind, source/answer inventory, read-only seal, sibling SQL hash and database identity must still match. Changed seeder/runtime inputs require a new capture. Historical full-world fixtures are never patched or implicitly reused.

Saved wrapper/harness/boot receipts report actual checks and cleanup. This reduced lane does not truncate the production Debug replay, validate every optional package, activate an existing world, or certify gameplay balance. No silt behavior is included.

## Staged Setting foundations

`-Mode setting-foundations -EarlierClasses On -PackageMode Simple` uses explicit Armageddon SkillPackage/Combat choices and requires Class/Subclass. It captures and seals only Core/Time/Attributes once, then restores that pre-skill SQL into an owned database before the new packages. Reusing `-BaseFixture` plus its exact digest restores that sealed baseline instead of repeating Core content. Setting and the historical core fixture use distinct kind/source/answer inventories and separate fixture directories.

This lane checks the early packages and renamed **magic phase** with the current schema and native first/cold boot. It does not certify unfinished mundane progression, heritage, required Pilot/action semantics or a complete combined Setting. The descriptor records captured steps separately from steps applied after restore. Full replay remains exhaustive and separate.
