# Core foundation qualification

Build this harness in Debug, then run `Run-IsolatedPreparedSeeder.ps1 -Mode core-foundations -EvidenceRoot <absolute-new-output-path>`. The existing wrapper creates and verifies a unique loopback MySQL instance, database names, port, UUID and TEMP datadir. It owns and cleans only its own resources.

The scenario executes Core, Time, Attribute, SkillPackage (Armageddon model, preindustrial), Human, Combat (no firearms) and Chargen through the shared executor. It verifies no Useful packages, captures a new reduced base under `.artifacts/core-foundations-fixtures`, seals the SQL and descriptor read-only, and restores into a second owned database with matching schema/content hashes. It tests Core rerun ID/custom-setting preservation, full Armageddon opt-in installation, native target IDs, and the normal first/cold boot, Admin login, LOOK, save and graceful shutdown path.

To retry only later qualification after harness-only changes, supply `-BaseFixture <absolute-core-foundations.json> -BaseFixtureSha256 <exact-uppercase-sha256>`. Kind, source/answer inventory, read-only seal, sibling SQL hash and database identity must still match. Changed seeder/runtime inputs require a new capture. Historical full-world fixtures are never patched or implicitly reused.

Saved wrapper/harness/boot receipts report actual checks and cleanup. This reduced lane does not truncate the production Debug replay, validate every optional package, activate an existing world, or certify gameplay balance. No silt behavior is included.

## Staged Setting foundations

`-Mode setting-foundations -EarlierClasses On -PackageMode Simple` uses explicit Armageddon SkillPackage/Combat choices and requires Class/Subclass. It captures and seals only Core/Time/Attributes once, then restores that pre-skill SQL into an owned database before the new packages. Reusing `-BaseFixture` plus its exact digest restores that sealed baseline instead of repeating Core content. Setting and the historical core fixture use distinct kind/source/answer inventories and separate fixture directories.

This lane checks the current combined Setting, including mundane progression, heritage, physical/lifecycle/appearance defaults, Pilot and combat actions, preserving rerun/resume, cold readers and first/cold server boot. Admission remains held. The descriptor records captured steps separately from steps applied after restore. See the [implementation ledger](../../Design%20Documents/Armageddon/Armageddon_Setting_Implementation_20261019.md) for actual completed evidence and limits.

## Full optional-package replay

`-Mode setting-full-replay -EarlierClasses On -PackageMode Simple` derives a complete Armageddon profile from the validated maintained Medieval inventory. Only Item and the mutually exclusive SkillSeeder alternative are omitted. All questions remain inventoried and the selected dependency plan is recomputed. The actual Debug replay executor installs the world and must refuse a second attempt without changing any table. Native production-loader checks cover Horse body/health/natural attacks/butchery, the owned Skinning binding, Longsword Slashing and Razor Weapons, plus surgical procedures. The normal first/cold executable boot then verifies Admin login, LOOK, save and graceful shutdown.

Before runtime qualification the completed install is backed up and sealed under `.artifacts/full-setting-replay-fixtures`. For a harness-only retry, pass `-BaseFixture <absolute-full-replay.json> -BaseFixtureSha256 <uppercase-digest>`. Restore verifies the read-only descriptor/SQL, complete production source closure, complete answer inventory, and exact schema/content before running any loaded-content or boot checks. It performs no seeding. A compatible checkpoint blocks another fresh install unless `-FreshInstallCheck` is explicit. Changed production inputs require a new capture. This compatibility smoke does not certify every optional gameplay system or turn generic content into Armageddon canon; the seven playable profile rows remain unavailable and the common Elf parent stays nonplayable.
