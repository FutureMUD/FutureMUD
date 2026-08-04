# DatabaseSeeder Repeatability Strategy and Audit

## Purpose
This document is the durable reference for how DatabaseSeeder packages should behave over time.

It has four jobs:

1. Define the seeder repeatability goals and contributor rules.
2. Record the verified current state of every live `IDatabaseSeeder`.
3. Classify which seeders are currently safe, additive, one-shot, or in need of deeper refactor work.
4. Keep the improvement backlog and conversion order in one place.

This document is based on verified code behavior in the current stock repo, not on intent alone.

## Target Principles
- Minimize questions. If multiple stock options can coexist safely, prefer shipping more stock content over asking the builder to choose only one path.
- Treat stock seed definitions as the base install truth. Clean installs should receive corrected content directly from the source definitions, not from follow-up repair passes.
- Reuse previous answers wherever practical. Shared setup answers should come from the generic `SeederChoice` answer-memory flow rather than bespoke helper logic.
- Present honest rerun semantics. If a seeder is additive, idempotent, or one-shot, the menu and package details should say so clearly.
- Prefer deterministic lookup-and-upsert behavior for stock-owned records. The default rule for repeatable seeders is: install missing stock records, update stock-owned canonical records when safe, and leave clearly user-customized records alone unless the seeder explicitly documents a different rule.
- Keep prerequisites explicit. A blocked seeder should explain what is missing instead of relying only on broad boolean probes.
- Treat the seeder framework as shared infrastructure. Foundational seeders should not solve repeatability in isolation.

## Base Install Truth
Stock seed definitions are the canonical output of a clean install. When stock content is wrong, incomplete, poorly named, or inconsistent with the authoring rules, fix the definition that creates it so a new database receives the corrected data immediately.

Do not seed known-bad stock data and then rely on a follow-up correction method to make the install usable. Compatibility update paths are acceptable only when they serve existing databases that may already have old stock rows, or when a seeder has an explicitly documented `RepairExisting` or `FullReconcile` ownership model.

Any repair-capable or compatibility update path must be:

- narrow to stock-owned rows with stable lookup keys
- documented in this strategy and in the affected seeder or subsystem design note
- safe for builder-customized data
- unnecessary for a clean install to be correct

For source-only catalogue fixes, prefer invariant tests that scan the seed definitions or generated specs so regressions fail before they become seeded data.

## Taxonomy
### Repeatability mode
- `OneShot`: not currently intended to be rerun safely.
- `Additive`: reruns are intended to add more stock content, not reconcile earlier stock records.
- `Idempotent`: reruns are expected not to duplicate stock-owned records.

### Update capability
- `None`: no supported repeatability beyond the initial install.
- `InstallMissing`: reruns can add missing stock records.
- `RepairExisting`: reruns can repair or refresh stock-owned records that already exist.
- `FullReconcile`: the seeder can fully reconcile stock-owned records against current stock definitions.

## 2026 Repeatability Reconciliation Update

- `CoreDataSeeder` is now `Idempotent` / `RepairExisting` only for its foundation catalogues. A detected rerun skips account and game-identity questions and never recreates bootstrap accounts, characters, cells, shards, zones, channels, settings, or helper progs. It reconciles stock tags, materials, liquids, gases, terrain foundations and forage profiles, units, colours, default planes, and hearing profiles by stable stock identities.
- `AttributeSeeder`, `HumanSeeder`, `AnimalSeeder`, and `CombatSeeder` are now repeat-safe and declared `Idempotent` / `FullReconcile`. Their recorded structural choices are reused on a rerun; when a choice cannot be recovered safely, the seeder preserves the installed shape rather than converting it.
- Rerunnable metadata seeders now all declare `SafeToRunMoreThanOnce`. `ItemSeeder` is `Idempotent` / `FullReconcile`, backed by an executable aggregate manifest and durable `SeederManagedRecords` provenance.
- Combat remains one user-facing menu package. Its reconciliation is internally split into foundation, melee/shield, ranged, early-firearm, modern-firearm, armour, era-dependency, and auxiliary/manual-command modules with dependency ordering. Missing firearm modules remain opt-in; installed firearm modules are repaired when Combat is rerun.
- Stock reconciliation adds missing named records and required links, refreshes canonical stock scalar data where a stable ownership key exists, and retains builder-added extensions. Duplicate natural keys, incompatible fixed IDs, or unresolvable structural choices are reported rather than guessed.

## Verified Current Baseline
- Phase 1 is verified complete in code:
  - generic seeder metadata and structured assessment states are live
  - shared answer reuse is live through `SeederChoice`-backed shared answer keys
  - the menu and package-detail UI now explain blocked, ready, additive rerun, update-available, and current states
  - additive rerun messaging is wired through seeder metadata rather than only color
  - contributor guidance for repeatability now lives in `DatabaseSeeder/AGENTS.md`
- `AIStorytellerSeeder`, `UsefulSeeder`, `MythicalAnimalSeeder`, `RobotSeeder`, `CelestialSeeder`, `CurrencySeeder`, and `ClanSeeder` now explicitly set `SafeToRunMoreThanOnce`.
- `UsefulSeeder` now exposes its AI examples as one repeatable stock package question instead of the older `ai` / `ai2` split, and that package installs or refreshes stock-owned AI examples by stable names, including the `BasicMount` stock `MountAI` definition used by mount-capable NPC imports.
- `CoreDataSeeder` now owns the stock terrain catalogue, terrain-tag taxonomy, and stock terrain forage-profile backfill so terrain-aware packages and animal grazing systems can rely on default terrain yield capacities without an extra prompt.
- `AgricultureSeeder` installs repair-capable stock agriculture profiles, crop/herd/woodland definitions, operations, and backing local project templates by stable names, including broad stock coverage for common crops and rough land-expansion profiles.
- `PrimaryProductionSeeder` installs repair-capable stock primary-production local projects by deterministic project names, using stock visible resource props, commodity outputs, and bulk commodity requirements.
- `StockMeritsSeeder` now provides a repair-capable stock merits and flaws package built around stable merit names and tag-driven helper FutureProgs.
- `ItemSeeder` now registers its stock items, crafts, outfits, supporting definitions, lifecycle relationships, and complete vehicle graphs through one executable manifest. The checked-in `Seeded_Item_Manifest.json` is a generated review artefact; the executable registry is authoritative.
- Shared answer reuse is no longer combat-only. The live shared-answer wave covers combat message style, damage randomness, human health model, and non-human health model.
- Many legacy seeders still rely on coarse installed-state checks such as `Accounts.Any()`, `WeaponAttacks.Any()`, `ClimateModels.Any()`, `ChargenScreenStoryboards.Any()`, or `SurgicalProcedures.Any()`. Phase 2 is the wave intended to replace those with deterministic stock-key detection.
- Duplicate `SortOrder` values were previously unstable in the menu flow; the structured assessment/menu work now gives that ordering deterministic tie-breaking.
- Seeder metadata now also declares its provider-seeder dependencies. The menu derives a stable topological installation order from those declarations, and startup rejects missing providers, self-dependencies, and dependency cycles. Database-backed prerequisite predicates remain authoritative so custom worlds can satisfy a requirement without having run its stock provider.
- Combat is explicitly blocked until the selected attribute/skill foundations, Human race, and UsefulSeeder crossbow spanning-tool tags exist. This prevents its era dependency repair from failing after the user has already answered the combat questions.
- Weather/climate regression coverage already exists in `MudSharpCore Climate Tests`, centered on `WeatherSeederOceanicClimateTests.cs`, and should continue to carry the slow, opt-in simulation-regression side of seeder verification rather than the default unit-test pass.
- `WeatherSeeder` owns the natural-light attenuation values on its canonical weather events. Clear, dry, and humid skies preserve full natural light; scattered cloud uses `0.8`; overcast uses `0.5`; and rain or snow steps down from `0.6` for light precipitation through `0.4`, `0.25`, and `0.1` as severity increases. Sleet uses `0.3`. Because weather-event rows are stock-owned and upserted by stable name, rerunning the seeder repairs these values on an existing seeded package as well as applying them to a clean install.

## Seeder Audit Matrix
| Seeder | Sort | Current prerequisite logic | Current rerun signal | Current answer reuse | Current duplicate / update behavior | Current repair ability | Target classification | Complexity | Recommended next action |
| --- | ---: | --- | --- | --- | --- | --- | --- | --- | --- |
| `CoreDataSeeder` | 0 | No prerequisite beyond database itself | Existing Core choice or bootstrap account | Core choices are retained but not re-asked | Fresh runs create bootstrap records; reruns reconcile only stock tags, materials, fluids, gases, terrain/forage, units, colours, planes, and hearing profiles | Does not alter accounts, characters, world records, settings, channels, or bootstrap progs | `Idempotent` / `RepairExisting` | High | Keep the bootstrap-world boundary explicit |
| `TimeSeeder` | 5 | Requires an account | Deterministic stock-key check on canonical seeded clocks/calendars/timezones | None | Upserts seeded calendars, clocks, timezones, and adds missing shard/zone links by stable stock names without mutating composite-key link rows | Repairs seeded package in place | `Idempotent` / `RepairExisting` | Medium | Keep adding focused rerun tests for changed-answer world-time updates |
| `CelestialSeeder` | 6 | Requires an account | `Ready`/`ExtraPackagesAvailable`/`MayAlreadyBeInstalled` by celestial count | None | Additive by count, no explicit reconciliation | None | `Additive` / `InstallMissing` | Low | Keep additive semantics, improve messaging and docs |
| `AttributeSeeder` | 10 | Requires an account | Installed attribute traits | Reuses recorded package/decorator choice | Retains the installed structural shape and reconciles stock stamina support | Preserves unrelated builder traits | `Idempotent` / `FullReconcile` | High | Keep shape inference conservative |
| `SkillPackageSeeder` | 11 | Requires account and attribute traits | Deterministic stock-key check on package-owned skills/language scaffolding | None | Upserts package skills, checks, decorators, improvers, and language scaffolding by stable names | Repairs seeded package in place | `Idempotent` / `RepairExisting` | Medium | Preserve mutual exclusivity with `SkillSeeder` and add repeatability coverage |
| `SkillSeeder` | 11 | Requires account and attribute traits | Deterministic stock-key check on example-skill markers | None | Upserts example skills, checks, and sample language records by stable names | Repairs seeded package in place | `Idempotent` / `RepairExisting` | Medium | Preserve alternative-path warning semantics and add repeatability coverage |
| `CurrencySeeder` | 20 | Requires an account | `ExtraPackagesAvailable` if any currency exists | None | Additive by currency presence | None | `Additive` / `InstallMissing` | Low | Present clearly as additive rerun package |
| `HumanSeeder` | 50 | Requires account, skill traits, and calendars | Installed Humanoid foundation and stock drift checks | Reuses recorded structural choices | Refreshes stock combat balance, satiation, language-trait, wear-profile, and disfigurement data without recreating the admin avatar | Existing human body/race shape is retained | `Idempotent` / `FullReconcile` | High | Extend stock graph manifests as new human content ships |
| `ClanSeeder` | 50 | Requires account, clock, and currency | `ExtraPackagesAvailable` if some templates missing | None | Additive by named template presence | None | `Additive` / `InstallMissing` | Low | Keep additive semantics, improve messaging and docs |
| `CombatSeeder` | 90 | Requires account, attributes, shared skill/check scaffolding, `Human`, and UsefulSeeder crossbow-spanning tags | Installed attack foundation and module checks | Reuses recorded structural choices; missing firearm modules remain opt-in | Dependency-ordered internal modules refresh stock formulas, expanded melee, ranged, era, modern-firearm, and auxiliary content | Builder-added records are retained | `Idempotent` / `FullReconcile` | High | Keep module identities and dependencies covered by tests |
| `ChargenSeeder` | 100 | Requires at least one account and the `Human` race | Deterministic stock-key check on canonical stages, helper progs, static settings, and default starting-location role | None | Upserts chargen resources, helper progs, special-application static settings, default starting-location role, and one canonical storyboard row per chargen stage | Repairs seeded package in place, collapses duplicate storyboard rows, preserves the existing free-knowledge body, and reconciles marked stock Culture/Health grant sections | `Idempotent` / `RepairExisting` | Medium | Keep expanding focused rerun tests and preserve builder-authored storyboard and free-knowledge content outside stock-managed sections |
| `CultureSeeder` | 101 | Requires `Human`, skill decorators, and chargen size progs | Deterministic stock-key check on seeded simple name cultures/profile markers and pack markers, plus safely repairable script-grant drift when Chargen exists | None | Upserts simple name cultures, random profiles, languages, accents, scripts, additive script-language memberships, mutual intelligibilities, ethnicities, cultures, stock blood/sweat materials, and its marked free-script-knowledge rules | Repairs seeded stock records in place, replaces drifted keyed ethnicity-characteristic joins, and preserves cross-pack, builder-added script memberships, and builder-authored free-knowledge logic | `Idempotent` / `RepairExisting` | Medium | Finish deeper race-specific Middle-earth rerun coverage and keep the language and heritage coverage matrices synchronized |
| `StockMeritsSeeder` | 102 | Requires account, `Human`, and a merit or quirk selection storyboard | Deterministic stock-key check on canonical stock merit names and helper progs | None | Upserts stock merits, flaws, and tag-driven helper progs by stable names | Repairs seeded package in place | `Idempotent` / `RepairExisting` | Medium | Keep the catalogue mode-neutral and expand it only where merit types have clear stock ownership boundaries |
| `ArenaSeeder` | 110 | Requires an economic zone | Deterministic named-arena stock-key check | None | Upserts named stock arena scaffold, classes, sides, event types, and helper progs | Repairs seeded package in place | `Idempotent` / `RepairExisting` | Medium | Add same-name rerun tests and keep live arena runtime data builder-owned |
| `UsefulSeeder` / `Kickstart` | 200 | Requires an account | Package-level readiness based on tracked AI examples plus legacy item/tag markers | None before framework; now generic memory is available | Installs or refreshes stock AI examples by stable names and installs missing tracked package parts without duplication | Repair path exists for the stock AI example package; other subpackages remain install-missing only | `Idempotent` / `InstallMissing` | Medium | Keep subpackages on stable ownership boundaries and expand repair-capable coverage only where names cleanly imply stock ownership |
| `AIStorytellerSeeder` | 215 | Requires an account | `ExtraPackagesAvailable` for partial install, `MayAlreadyBeInstalled` when full | None | Reuses and updates existing sample storyteller records by name/function name | Yes, for stock sample records | `Idempotent` / `RepairExisting` | Low | Keep as reference implementation for repair-capable packages |
| `AgricultureSeeder` | 220 | Requires account, `AlwaysTrue`, all stock agriculture tags, and the Farming trait | Deterministic stock-key check on agriculture operation names | None | Upserts stock field profiles, crops, herds, woodlands, operations, and backing local project templates by stable names | Repairs stock-owned agriculture definitions and project templates in place | `Idempotent` / `RepairExisting` | Medium | Keep field/cell assignments builder-owned and expand stock examples only where definitions remain setting-neutral |
| `HealthSeeder` | 250 | Requires account, `Organic Humanoid`, and tool tags | Deterministic stock-key check on seeded procedures/knowledges/drugs, plus safely repairable medical-grant drift when Chargen exists | None | Upserts stock procedures, phases, knowledges, targets, drugs, and its marked free-medical-knowledge rules | Repairs seeded package in place with forward-only tech upgrades while preserving builder-authored free-knowledge logic | `Idempotent` / `RepairExisting` | Medium | Keep the acquisition-prog and free-grant contracts covered together |
| `AnimalSeeder` | 300 | Requires `Humanoid` body and `Simple` name culture | Installed Quadruped Base and stock drift checks | Reuses recorded non-human structural choices | Refreshes stock bodies, catalogue backfills, combat balance, AI, diet, wear, disfigurement, and auxiliary links | Builder-added animal content is retained | `Idempotent` / `FullReconcile` | High | Keep the body/race manifest checks expanding with stock content |
| `WeatherSeeder` | 300 | Requires account and at least one celestial | Deterministic stock-key check on seeded climate/weather markers | None | Upserts stock weather events, including natural-light attenuation, plus seasons, climate models, regional climates, and rain settings by stable names; `full` and `soak` explicitly reconcile `PuddlesEnabled` to true and false respectively | Repairs seeded package in place | `Idempotent` / `RepairExisting` | Medium | Keep controller assignment builder-owned and expand regression coverage only where needed |
| `MythicalAnimalSeeder` | 302 | Requires human and animal body frameworks, corpse models, characteristic profiles, and non-human strategies | `MayAlreadyBeInstalled` only when all stock mythic races exist | Non-human health model, damage randomness, and combat message style are now shareable | Installs incrementally and skips existing stock mythic races | Install-missing only | `Idempotent` / `InstallMissing` | Medium | Document exact skip behavior and preserve as repeatable package |
| `RobotSeeder` | 305 | Requires humanoid and animal body frameworks, characteristic profiles, corpse models, tool tags, progs, and prerequisite attacks | `MayAlreadyBeInstalled` only when all tracked robot content exists | None | Installs incrementally and skips existing stock robot records | Install-missing only | `Idempotent` / `InstallMissing` | Medium | Document exact skip behavior and preserve as repeatable package |
| `ItemSeeder` | 400 | Requires Useful item component prerequisites | Provenance-backed fresh/current/update status | Recalls installed eras; reruns retain installed eras and may add implemented eras | Reconciles manifest-owned aggregate graphs by stable identity and last-applied fingerprint | Repairs untouched stock, restores required links, preserves builder-customized aggregates, retires removed definitions without deletion | `Idempotent` / `FullReconcile` | High | Keep every persistence path behind the manifest registry and check the generated manifest in CI |
| `PrimaryProductionSeeder` | 420 | Requires account, `AlwaysTrue`, primary-production tags/materials, primary-production visible resource props, bloomery apparatus, and stock labour traits | Deterministic stock-key check on `Stock Primary Production: ` project names | None | Upserts stock prospecting, extraction, quarrying, kiln, smelting, salt, tar, peat, pigment, and coal local project templates by deterministic names | Repairs stock-owned project definitions, labour, material requirements, and actions in place | `Idempotent` / `RepairExisting` | Medium | Keep resource-site placement builder-owned and expand database-backed rerun coverage after more primary-production chains ship |
| `LawSeeder` | 5000 | Requires account and currency | Deterministic stock-key check within legal authorities | None | Upserts named authorities, legal classes, witness profiles, enforcement groups, and stock laws by stable names | Repairs seeded package in place | `Idempotent` / `RepairExisting` | Medium | Add same-authority rerun tests and confirm live runtime references stay intact |

## Current Buckets
### Explicit rerunnable baseline
- `AIStorytellerSeeder`
- `UsefulSeeder`
- `MythicalAnimalSeeder`
- `RobotSeeder`
- `TimeSeeder`
- `SkillPackageSeeder`
- `SkillSeeder`
- `ChargenSeeder`
- `StockMeritsSeeder`
- `CultureSeeder`
- `HealthSeeder`
- `WeatherSeeder`
- `ArenaSeeder`
- `AgricultureSeeder`
- `PrimaryProductionSeeder`
- `LawSeeder`

### Additive but originally ambiguous
- `CelestialSeeder`
- `CurrencySeeder`
- `ClanSeeder`

### Manifest-backed full reconciliation
- `ItemSeeder`, with foundations, shared pre-industrial, Antiquity, Medieval, Renaissance, Early Modern, lifecycle, outfit, craft, and vehicle modules

## ItemSeeder Ownership and Revision Rules

- Stable references identify logical item prototypes; several revisions of one logical ID are valid, while the same active reference on multiple logical IDs blocks the run.
- Provenance is stored per logical aggregate. Item, craft, outfit, and vehicle fingerprints include their owned child graphs.
- A legacy row is adopted only when its unique identity and complete canonical signature match. Drifted or ambiguous untracked rows remain unmanaged and block mutation.
- A managed aggregate is refreshed only while its live fingerprint matches the last applied stock fingerprint. Any builder modification preserves the entire aggregate.
- Builder-added records and relationships are retained. Removed stock is marked retired in provenance and is never deleted.
- Revolution, Modern, Atomic, and Computer are not selectable until executable modules contain real stock definitions.
- `--check-item-manifest` validates the checked-in registry without a database. `--export-item-manifest [path]` exports the same canonical document without connecting to a database.

## System-Level Findings
### Menu and status flow
- The old menu ordered only by `SortOrder`, so duplicate values produced unstable ordering.
- The old package-detail view only warned for `PrerequisitesNotMet` and `MayAlreadyBeInstalled`. `ExtraPackagesAvailable` had no explanatory detail.
- The new framework introduces structured seeder assessments so the menu can distinguish blocked, ready, additive rerun, update-available, and current packages.

### Answer memory
- The old answer-memory behavior was effectively custom combat-message reuse only.
- The new framework adds generic shared answer keys on top of `SeederChoice`, while keeping the existing schema.
- The first shared-answer wave covers:
  - combat message style
  - damage randomness
  - human health model
  - non-human health model

### Prerequisites
- Most legacy prerequisite checks were broad booleans embedded in `ShouldSeedData`.
- The new framework adds explicit prerequisite metadata so package detail can name what is missing, even while legacy `ShouldSeedData` remains in place for compatibility.
- `DependencySeederTypes` supplies the separate ordering graph. Do not use it as the runtime readiness check: a custom database may provide equivalent records without having used the stock source seeder. Keep the exact database predicate in `Prerequisites` and declare only genuine stock-provider relationships as ordering edges.
- Dependencies must remain acyclic. The database-seeder unit suite reflects every discoverable seeder, verifies the graph has no errors, and asserts the critical Useful → Combat/Item/Economy and Animal → Mythical → Supernatural paths.

## Current Framework Decisions
- Keep the legacy tuple-based `SeederQuestions`, `SafeToRunMoreThanOnce`, and `ShouldSeedData` members for compatibility.
- Add richer concepts alongside them:
  - `SeederQuestion`
  - `SeederMetadata`
  - `SeederAssessment`
  - shared answer keys backed by `SeederChoice`
- Prefer central registries and shared framework code over mass rewriting every seeder at once.
- Do not enable Pomelo `EnableStringComparisonTranslations` as the default seeder fix. Provider opt-in is lower-touch, but Pomelo warns it can reduce index usage depending on collation and comparison mode, so the preferred seeder pattern is explicit query shaping: use SQL only for safe prefilters where practical, then finish case-insensitive matching in memory.

## Backlog and Conversion Order
### Phase 1: verified complete
- Add generic seeder metadata and assessment framework.
- Add generic question enrichment and shared answer memory.
- Update the menu and package-detail UI to use structured assessment states.
- Align `CelestialSeeder`, `CurrencySeeder`, and `ClanSeeder` with additive-rerun messaging.
- Update contributor guidance and record the audit.

### Phase 2: quick wins and medium conversions
- Convert `TimeSeeder`, `WeatherSeeder`, `HealthSeeder`, `CultureSeeder`, `SkillPackageSeeder`, `SkillSeeder`, `ChargenSeeder`, `ArenaSeeder`, and `LawSeeder` to deterministic lookup-and-upsert behavior where practical.
- Keep the default rule for this wave:
  - install missing stock records
  - update canonical stock-owned records when safe
  - leave clearly user-customized records alone unless explicitly documented

### Phase 3: high-complexity individual design plans
- `CoreDataSeeder`
- `HumanSeeder`
- `CombatSeeder`
- `AnimalSeeder`
- `ItemSeeder`

These five seeders need separate design work before repeatability claims are expanded because they seed foundational or very large interdependent graphs.

## Contributor Checklist
- When adding or changing a seeder, update both its metadata and any shared-answer mapping that applies.
- Fix incorrect stock content at the seed definition first. Do not add a correction layer that is required for a clean install to be correct.
- If a seeder becomes safely rerunnable, document whether it is additive, install-missing, repair-capable, or full-reconcile.
- Add compatibility repair/update code only for existing databases or explicitly owned rerun reconciliation, and keep it narrow and documented.
- Do not rely on `ExtraPackagesAvailable` alone to communicate rerun safety.
- Do not pass `StringComparison`-based predicates directly into EF-translated seeder queries. Use `AsEnumerable()` explicitly or shape the query so EF only handles the safe prefilter.
- Prefer deterministic, stock-owned lookup keys over “anything exists” installed-state checks.
- If a seeder cannot yet be made repeatable safely, document why in this file instead of implying support.

