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
- Only `AIStorytellerSeeder`, `UsefulSeeder`, `MythicalAnimalSeeder`, and `RobotSeeder` explicitly set `SafeToRunMoreThanOnce`.
- `CelestialSeeder`, `CurrencySeeder`, and `ClanSeeder` already behave like additive rerun packages in the menu flow, but they were not originally flagged as safe and relied on ambiguous UI.
- Shared answer reuse was previously limited to combat message style via `SeederChoice` and `CombatSeederMessageStyleHelper`.
- Critical seeders still use coarse installed-state checks such as `Accounts.Any()`, `WeaponAttacks.Any()`, `ClimateModels.Any()`, `ChargenScreenStoryboards.Any()`, or `SurgicalProcedures.Any()`.
- Duplicate `SortOrder` values existed in the live menu flow, which made ordering unstable whenever multiple seeders shared the same numeric order.
- `ExtraPackagesAvailable` was shown by color but not explained clearly in the package detail view.

## Seeder Audit Matrix
| Seeder | Sort | Current prerequisite logic | Current rerun signal | Current answer reuse | Current duplicate / update behavior | Current repair ability | Target classification | Complexity | Recommended next action |
| --- | ---: | --- | --- | --- | --- | --- | --- | --- | --- |
| `CoreDataSeeder` | 0 | No prerequisite beyond database itself | `MayAlreadyBeInstalled` if any account exists | None | Coarse gate on `Accounts.Any()` | None | `OneShot` / `None` until separate design | High | Create dedicated canonical-record plan before repeatability work |
| `TimeSeeder` | 5 | Requires an account | `MayAlreadyBeInstalled` if any clock exists | None | Coarse gate on `Clocks.Any()` | None | `OneShot` now, target repeatable later | Medium | Convert to deterministic calendar/clock lookup-and-upsert in a later wave |
| `CelestialSeeder` | 6 | Requires an account | `Ready`/`ExtraPackagesAvailable`/`MayAlreadyBeInstalled` by celestial count | None | Additive by count, no explicit reconciliation | None | `Additive` / `InstallMissing` | Low | Keep additive semantics, improve messaging and docs |
| `AttributeSeeder` | 10 | Requires an account | `MayAlreadyBeInstalled` if any attribute trait exists | None | Coarse gate on trait type | None | Intentional `OneShot` / `None` | Low | Document as deliberate one-shot for now |
| `SkillPackageSeeder` | 11 | Requires account and attribute traits | `MayAlreadyBeInstalled` if skill traits already exist | None | Coarse gate on skill trait existence | None | `OneShot` now, target repeatable later | Medium | Move to deterministic skill-package reconciliation in medium wave |
| `SkillSeeder` | 11 | Requires account and attribute traits | `MayAlreadyBeInstalled` if skill traits already exist | None | Coarse gate on skill trait existence | None | `OneShot` now, target repeatable later | Medium | Same as `SkillPackageSeeder`; evaluate merge or shared infrastructure |
| `CurrencySeeder` | 20 | Requires an account | `ExtraPackagesAvailable` if any currency exists | None | Additive by currency presence | None | `Additive` / `InstallMissing` | Low | Present clearly as additive rerun package |
| `HumanSeeder` | 50 | Requires account, skill traits, and calendars | `MayAlreadyBeInstalled` if `Humanoid` race exists | Human health-model answer can now be shared | Coarse gate on a single race name despite large seeded graph | None | `OneShot` / `None` until dedicated plan | High | Separate design for canonical humanoid ownership and safe upsert rules |
| `ClanSeeder` | 50 | Requires account, clock, and currency | `ExtraPackagesAvailable` if some templates missing | None | Additive by named template presence | None | `Additive` / `InstallMissing` | Low | Keep additive semantics, improve messaging and docs |
| `CombatSeeder` | 90 | Requires `Human` race | `MayAlreadyBeInstalled` if any weapon attack exists | Combat message style and damage randomness can now be shared | Coarse gate on `WeaponAttacks.Any()` despite many subpackages | None | `OneShot` / `None` until dedicated plan | High | Split into internal subpackages and design modular reconciliation |
| `ChargenSeeder` | 100 | Requires `Human` race | `MayAlreadyBeInstalled` if any chargen storyboard exists | None | Coarse gate on `ChargenScreenStoryboards.Any()` | None | `OneShot` now, target repeatable later | Medium | Move to storyboard-by-key reconciliation in medium wave |
| `CultureSeeder` | 101 | Requires `Human`, skill decorators, and `MaximumHeightChargen` prog | `MayAlreadyBeInstalled` if any random-name profile exists | None | Coarse gate on any name profile | None | `OneShot` now, target repeatable later | Medium | Convert to deterministic culture-pack reconciliation |
| `ArenaSeeder` | 110 | Requires an economic zone | `MayAlreadyBeInstalled` if default arena exists | None | Single named arena guard | None | `OneShot` now, target repeatable later | Medium | Convert to named stock arena reconciliation |
| `UsefulSeeder` / `Kickstart` | 200 | Requires an account | `ExtraPackagesAvailable` or `MayAlreadyBeInstalled` based on package parts | None before framework; now generic memory is available | Installs missing package parts and avoids duplicates for tracked content | No deliberate repair path | `Idempotent` / `InstallMissing` | Medium | Audit subpackages more deeply and document exact ownership boundaries |
| `AIStorytellerSeeder` | 215 | Requires an account | `ExtraPackagesAvailable` for partial install, `MayAlreadyBeInstalled` when full | None | Reuses and updates existing sample storyteller records by name/function name | Yes, for stock sample records | `Idempotent` / `RepairExisting` | Low | Keep as reference implementation for repair-capable packages |
| `HealthSeeder` | 250 | Requires account, `Organic Humanoid`, and tool tags | `MayAlreadyBeInstalled` if any surgical procedure exists | None | Coarse gate on `SurgicalProcedures.Any()` | None | `OneShot` now, target repeatable later | Medium | Convert procedures, knowledges, and stock drugs to deterministic upsert |
| `AnimalSeeder` | 300 | Requires `Humanoid` body and `Simple` name culture | `MayAlreadyBeInstalled` if `Quadruped Base` exists | Non-human health model, damage randomness, and combat message style are now shareable | Coarse gate on one body name despite very large seeded graph | None | `OneShot` / `None` until dedicated plan | High | Separate design for animal graph ownership, templates, and modular reconciliation |
| `WeatherSeeder` | 300 | Requires account and at least one celestial | `MayAlreadyBeInstalled` if any climate model exists | None | Coarse gate on `ClimateModels.Any()` | None | `OneShot` now, target repeatable later | Medium | Convert climate templates and stock weather records to deterministic upsert |
| `MythicalAnimalSeeder` | 302 | Requires human and animal body frameworks, corpse models, characteristic profiles, and non-human strategies | `MayAlreadyBeInstalled` only when all stock mythic races exist | Non-human health model, damage randomness, and combat message style are now shareable | Installs incrementally and skips existing stock mythic races | Install-missing only | `Idempotent` / `InstallMissing` | Medium | Document exact skip behavior and preserve as repeatable package |
| `RobotSeeder` | 305 | Requires humanoid and animal body frameworks, characteristic profiles, corpse models, tool tags, progs, and prerequisite attacks | `MayAlreadyBeInstalled` only when all tracked robot content exists | None | Installs incrementally and skips existing stock robot records | Install-missing only | `Idempotent` / `InstallMissing` | Medium | Document exact skip behavior and preserve as repeatable package |
| `ItemSeeder` | 400 | Requires Useful item component prerequisites | Always `ReadyToInstall` once prerequisites exist | None | No installed-state guard despite large stock content surface | None | `OneShot` / `None` until dedicated plan | High | Create explicit stock package ownership model before declaring rerun support |
| `LawSeeder` | 5000 | Requires account and currency | `MayAlreadyBeInstalled` if any legal authority exists | None | Coarse gate on `LegalAuthorities.Any()` | None | `OneShot` now, target repeatable later | Medium | Convert authorities, classes, and stock laws to deterministic upsert |

## Current Buckets
### Explicit rerunnable baseline
- `AIStorytellerSeeder`
- `UsefulSeeder`
- `MythicalAnimalSeeder`
- `RobotSeeder`

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
- `TimeSeeder`
- `WeatherSeeder`
- `ChargenSeeder`
- `CultureSeeder`
- `SkillPackageSeeder`
- `SkillSeeder`
- `HealthSeeder`
- `ArenaSeeder`
- `LawSeeder`
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
### Phase 1: completed in this pass
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
