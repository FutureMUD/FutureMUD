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
- Reuse previous answers wherever practical. Shared setup answers should come from the generic `SeederChoice` answer-memory flow rather than bespoke helper logic.
- Present honest rerun semantics. If a seeder is additive, idempotent, or one-shot, the menu and package details should say so clearly.
- Prefer deterministic lookup-and-upsert behavior for stock-owned records. The default rule for repeatable seeders is: install missing stock records, update stock-owned canonical records when safe, and leave clearly user-customized records alone unless the seeder explicitly documents a different rule.
- Keep prerequisites explicit. A blocked seeder should explain what is missing instead of relying only on broad boolean probes.
- Treat the seeder framework as shared infrastructure. Foundational seeders should not solve repeatability in isolation.

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

## Verified Current Baseline
- Phase 1 is verified complete in code:
  - generic seeder metadata and structured assessment states are live
  - shared answer reuse is live through `SeederChoice`-backed shared answer keys
  - the menu and package-detail UI now explain blocked, ready, additive rerun, update-available, and current states
  - additive rerun messaging is wired through seeder metadata rather than only color
  - contributor guidance for repeatability now lives in `DatabaseSeeder/AGENTS.md`
- `AIStorytellerSeeder`, `UsefulSeeder`, `MythicalAnimalSeeder`, `RobotSeeder`, `CelestialSeeder`, `CurrencySeeder`, and `ClanSeeder` now explicitly set `SafeToRunMoreThanOnce`.
- `UsefulSeeder` now exposes its AI examples as one repeatable stock package question instead of the older `ai` / `ai2` split, and that package installs or refreshes stock-owned AI examples by stable names.
- Shared answer reuse is no longer combat-only. The live shared-answer wave covers combat message style, damage randomness, human health model, and non-human health model.
- Many legacy seeders still rely on coarse installed-state checks such as `Accounts.Any()`, `WeaponAttacks.Any()`, `ClimateModels.Any()`, `ChargenScreenStoryboards.Any()`, or `SurgicalProcedures.Any()`. Phase 2 is the wave intended to replace those with deterministic stock-key detection.
- Duplicate `SortOrder` values were previously unstable in the menu flow; the structured assessment/menu work now gives that ordering deterministic tie-breaking.
- Weather/climate regression coverage already exists in `MudSharpCore Climate Tests`, centered on `WeatherSeederOceanicClimateTests.cs` / `WeatherSeederClimateTests`, and should continue to carry the simulation-regression side of seeder verification.

## Seeder Audit Matrix
| Seeder | Sort | Current prerequisite logic | Current rerun signal | Current answer reuse | Current duplicate / update behavior | Current repair ability | Target classification | Complexity | Recommended next action |
| --- | ---: | --- | --- | --- | --- | --- | --- | --- | --- |
| `CoreDataSeeder` | 0 | No prerequisite beyond database itself | `MayAlreadyBeInstalled` if any account exists | None | Coarse gate on `Accounts.Any()` | None | `OneShot` / `None` until separate design | High | Create dedicated canonical-record plan before repeatability work |
| `TimeSeeder` | 5 | Requires an account | Deterministic stock-key check on canonical seeded clocks/calendars/timezones | None | Upserts seeded calendars, clocks, timezones, and shard links by stable stock names | Repairs seeded package in place | `Idempotent` / `RepairExisting` | Medium | Keep adding focused rerun tests for changed-answer world-time updates |
| `CelestialSeeder` | 6 | Requires an account | `Ready`/`ExtraPackagesAvailable`/`MayAlreadyBeInstalled` by celestial count | None | Additive by count, no explicit reconciliation | None | `Additive` / `InstallMissing` | Low | Keep additive semantics, improve messaging and docs |
| `AttributeSeeder` | 10 | Requires an account | `MayAlreadyBeInstalled` if any attribute trait exists | None | Coarse gate on trait type | None | Intentional `OneShot` / `None` | Low | Document as deliberate one-shot for now |
| `SkillPackageSeeder` | 11 | Requires account and attribute traits | Deterministic stock-key check on package-owned skills/language scaffolding | None | Upserts package skills, checks, decorators, improvers, and language scaffolding by stable names | Repairs seeded package in place | `Idempotent` / `RepairExisting` | Medium | Preserve mutual exclusivity with `SkillSeeder` and add repeatability coverage |
| `SkillSeeder` | 11 | Requires account and attribute traits | Deterministic stock-key check on example-skill markers | None | Upserts example skills, checks, and sample language records by stable names | Repairs seeded package in place | `Idempotent` / `RepairExisting` | Medium | Preserve alternative-path warning semantics and add repeatability coverage |
| `CurrencySeeder` | 20 | Requires an account | `ExtraPackagesAvailable` if any currency exists | None | Additive by currency presence | None | `Additive` / `InstallMissing` | Low | Present clearly as additive rerun package |
| `HumanSeeder` | 50 | Requires account, skill traits, and calendars | `MayAlreadyBeInstalled` if `Humanoid` race exists | Human health-model answer can now be shared | Coarse gate on a single race name despite large seeded graph | None | `OneShot` / `None` until dedicated plan | High | Separate design for canonical humanoid ownership and safe upsert rules |
| `ClanSeeder` | 50 | Requires account, clock, and currency | `ExtraPackagesAvailable` if some templates missing | None | Additive by named template presence | None | `Additive` / `InstallMissing` | Low | Keep additive semantics, improve messaging and docs |
| `CombatSeeder` | 90 | Requires `Human` race | `MayAlreadyBeInstalled` if any weapon attack exists | Combat message style and damage randomness can now be shared | Coarse gate on `WeaponAttacks.Any()` despite many subpackages | None | `OneShot` / `None` until dedicated plan | High | Split into internal subpackages and design modular reconciliation |
| `ChargenSeeder` | 100 | Requires `Human` race | Deterministic stock-key check on seeded stages/progs/resources | None | Upserts chargen resources, helper progs, roles, and storyboard graphs by stable keys | Repairs seeded package in place | `Idempotent` / `RepairExisting` | Medium | Add storyboard rerun tests and keep preserving builder-authored screen definitions |
| `CultureSeeder` | 101 | Requires `Human`, skill decorators, and chargen size progs | Deterministic stock-key check on seeded simple name cultures/profile markers and pack markers | None | Upserts simple name cultures, random profiles, languages, scripts, ethnicities, cultures, and stock blood/sweat materials by stable names | Repairs seeded stock records in place | `Idempotent` / `RepairExisting` | Medium | Finish deeper race-specific Middle-earth rerun coverage and chargen-size-prog preservation tests |
| `ArenaSeeder` | 110 | Requires an economic zone | Deterministic named-arena stock-key check | None | Upserts named stock arena scaffold, classes, sides, event types, and helper progs | Repairs seeded package in place | `Idempotent` / `RepairExisting` | Medium | Add same-name rerun tests and keep live arena runtime data builder-owned |
| `UsefulSeeder` / `Kickstart` | 200 | Requires an account | Package-level readiness based on tracked AI examples plus legacy item/terrain/tag markers | None before framework; now generic memory is available | Installs or refreshes stock AI examples by stable names and installs missing tracked package parts without duplication | Repair path exists for the stock AI example package; other subpackages remain install-missing only | `Idempotent` / `InstallMissing` | Medium | Keep subpackages on stable ownership boundaries and expand repair-capable coverage only where names cleanly imply stock ownership |
| `AIStorytellerSeeder` | 215 | Requires an account | `ExtraPackagesAvailable` for partial install, `MayAlreadyBeInstalled` when full | None | Reuses and updates existing sample storyteller records by name/function name | Yes, for stock sample records | `Idempotent` / `RepairExisting` | Low | Keep as reference implementation for repair-capable packages |
| `HealthSeeder` | 250 | Requires account, `Organic Humanoid`, and tool tags | Deterministic stock-key check on seeded procedures/knowledges/drugs | None | Upserts stock procedures, phases, knowledges, targets, and drugs by stable names | Repairs seeded package in place with forward-only tech upgrades | `Idempotent` / `RepairExisting` | Medium | Add primitive-to-modern rerun coverage |
| `AnimalSeeder` | 300 | Requires `Humanoid` body and `Simple` name culture | `MayAlreadyBeInstalled` if `Quadruped Base` exists | Non-human health model, damage randomness, and combat message style are now shareable | Coarse gate on one body name despite very large seeded graph | None | `OneShot` / `None` until dedicated plan | High | Separate design for animal graph ownership, templates, and modular reconciliation |
| `WeatherSeeder` | 300 | Requires account and at least one celestial | Deterministic stock-key check on seeded climate/weather markers | None | Upserts stock weather events, seasons, climate models, regional climates, and rain settings by stable names | Repairs seeded package in place | `Idempotent` / `RepairExisting` | Medium | Keep controller assignment builder-owned and expand regression coverage only where needed |
| `MythicalAnimalSeeder` | 302 | Requires human and animal body frameworks, corpse models, characteristic profiles, and non-human strategies | `MayAlreadyBeInstalled` only when all stock mythic races exist | Non-human health model, damage randomness, and combat message style are now shareable | Installs incrementally and skips existing stock mythic races | Install-missing only | `Idempotent` / `InstallMissing` | Medium | Document exact skip behavior and preserve as repeatable package |
| `RobotSeeder` | 305 | Requires humanoid and animal body frameworks, characteristic profiles, corpse models, tool tags, progs, and prerequisite attacks | `MayAlreadyBeInstalled` only when all tracked robot content exists | None | Installs incrementally and skips existing stock robot records | Install-missing only | `Idempotent` / `InstallMissing` | Medium | Document exact skip behavior and preserve as repeatable package |
| `ItemSeeder` | 400 | Requires Useful item component prerequisites | Always `ReadyToInstall` once prerequisites exist | None | No installed-state guard despite large stock content surface | None | `OneShot` / `None` until dedicated plan | High | Create explicit stock package ownership model before declaring rerun support |
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
- `CultureSeeder`
- `HealthSeeder`
- `WeatherSeeder`
- `ArenaSeeder`
- `LawSeeder`

### Additive but originally ambiguous
- `CelestialSeeder`
- `CurrencySeeder`
- `ClanSeeder`

### Intentional one-shot for now
- `AttributeSeeder`

### High-risk or coarse-gated seeders
- `CoreDataSeeder`
- `HumanSeeder`
- `CombatSeeder`
- `ItemSeeder`
- `AnimalSeeder`

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

## Current Framework Decisions
- Keep the legacy tuple-based `SeederQuestions`, `SafeToRunMoreThanOnce`, and `ShouldSeedData` members for compatibility.
- Add richer concepts alongside them:
  - `SeederQuestion`
  - `SeederMetadata`
  - `SeederAssessment`
  - shared answer keys backed by `SeederChoice`
- Prefer central registries and shared framework code over mass rewriting every seeder at once.

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
- If a seeder becomes safely rerunnable, document whether it is additive, install-missing, repair-capable, or full-reconcile.
- Do not rely on `ExtraPackagesAvailable` alone to communicate rerun safety.
- Prefer deterministic, stock-owned lookup keys over “anything exists” installed-state checks.
- If a seeder cannot yet be made repeatable safely, document why in this file instead of implying support.
