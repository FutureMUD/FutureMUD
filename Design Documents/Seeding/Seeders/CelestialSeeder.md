# CelestialSeeder

## Start here to add content

Edit [CelestialSeeder.cs](../../../DatabaseSeeder/Seeders/CelestialSeeder/CelestialSeeder.cs): **BuildSunDefinition / EnsureEarthSunPackage for physical packages; AuthoredCelestialPresets for authored examples**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Sets up Suns, Moons, etc.
Baseline availability: **Enabled**. The entrypoint is [CelestialSeeder.cs](../../../DatabaseSeeder/Seeders/CelestialSeeder/CelestialSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `packages`, `suncalendar`, `sunepoch`, `mooncalendar`, `moonepoch`, `gasgiantcalendar`, `gasgiantsunepoch`, `gasgiantmoonepoch`, `authoredcalendar`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Existing physical record route.** `SeederQuestions` includes `packages`, `suncalendar`, and `sunepoch`; filters hide follow-up questions unless EarthSun is selected and an equivalent package is missing. A sole installed calendar is automatic. `SeedData` dispatches the selection to `EnsureEarthSunPackage`. That helper first uses `GetEarthSunCandidates` and returns the existing candidate if found; otherwise it resolves the chosen calendar and feed clock, calls `BuildSunDefinition` with the stock name and user epoch plus `EarthSunPackage` stable package marker and constants (Earth orbital year, anomaly, eccentricity, radius, epoch, illumination and scattering parameters), creates a typed Sun via `CreateCelestial`, then `context.Celestials.Add(sun)` and `SaveChanges` (`CelestialSeeder.cs:865-887`). `HasEarthSunPackage` and `IsEarthSunCelestial` recognize a tagged package or legacy definition with matching canonical orbital fields, avoiding duplicate stock installs. Linked Moon and gas-giant examples comprise multiple related rows and are installed through separate Ensure helpers.

**Add-one recipe depends on content intent.** Do not add another `Celestial` initializer with guessed XML. A new geography-independent authored demo is supported by explicit preset registry in `MudSharpCore/Celestial/Authored/AuthoredCelestialPresets.cs`: `Names` currently has six literals (`RailSun`, `RailMoon`, `ScriptedSun`, `ScriptedMoon`, `MorningStar`, `MorningGlow`); `Create(preset, calendarId, minutesPerHour, hoursPerDay)` validates membership and maps naming conventions into `AuthoredCelestialDefinition` fields including kind, calendar, annual period, path, light mode/profile and moon phase; scripted/morning examples change track keys and add milestone/echo entries. To add `ExampleComet`, extend `Names` and add an explicit branch in `Create` (the current logic implicitly classifies based on name ending/starting with Moon/Scripted/Morning, so an arbitrary name would otherwise default to a rail sun). Add that preset's source trajectory/phase/lighting/milestones and test it through formatter/compiler. `EnsureAuthoredPackage` in [CelestialSeeder.Authored.cs](../../../DatabaseSeeder/Seeders/CelestialSeeder/CelestialSeeder.Authored.cs) loops selected preset names not already present, calls `Create`, compiles (`CompiledAuthoredCelestial`) using selected calendar and clock dimensions, serializes and adds a `Celestial` row. Because `HasAuthoredPackage` checks all preset names, a new preset must also be added to the selection mapping in `CelestialSeeder.Selection.cs` so it can be installed and package completeness can be reached.

**Transaction/ownership.** Main seeder wraps all requested choices in BeginTransaction, `SaveChanges`, Commit and catch/Rollback/rethrow (`CelestialSeeder.cs:185-217`). Physical packages identify ownership through serialized package metadata and conservative legacy matching. `SeederMetadataRegistry.cs:59-68` describes Additive / InstallMissing: existing IDs/customized definitions survive; missing package members are added. Requires Core account; Weather also depends on a celestial. The initial `packages` selection controls follow-up questions and skips completed selections. Authored examples are not auto-bound to shards. Relevant tests are [AuthoredCelestialSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/AuthoredCelestialSeederTests.cs) and [CelestialSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/CelestialSeederTests.cs); not executed.

For physical package additions, inspect `GetPackageName`/`PackageMetadata` and each `Has...Package` detector alongside the `Ensure...Package` method; partial packages are often considered installed based on required type sets (e.g. gas giant checks Sun, PlanetaryMoon, PlanetFromMoon, SunFromPlanetaryMoon), while older data may be adopted by numerical signature. Add a related member to the creator, record the same stable package marker in its definition, and update completeness detection only if the new member is required to make the package complete; otherwise older installs could be needlessly reclassified. Stock names (`The Sun`, `The Moon`) feed the generated definition and are not an identity key. For authored examples, `Names` is also the completeness manifest and matching serialized `SeederPreset` is the install identity; use a stable unique preset key and ensure it can round-trip in parser/compiler. A test should assert each catalog name has exactly one installed row of expected kind and that rerun retains its ID while adding only newly registered missing names. The outer transaction permits linked celestial records to be created in dependency order before final commit; `SaveChanges` inside the Sun helper makes the Sun ID available for related rows but remains within the transaction.


## Declared rerun and ownership contract

Metadata declares `Additive / InstallMissing`.

Adds missing stock physical packages and optional authored celestial examples.

Authored examples use stable preset identities; reruns preserve existing IDs and builder-edited definitions and add missing members. Physical packages retain their existing detection rules.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.

## Selection workflow

See [Celestial System Seeder](../../World/Celestial_System_Seeder.md) for prompt guidance. The first screen accepts `EarthSun EarthMoon Jupiter Rails Scripts MorningStar MorningGlow` in any combination, separated by spaces, or `none`. Only selected incomplete packages ask questions. With one calendar, `EarthSun EarthMoon` asks only the two epoch dates; stock names are automatic. Multiple calendars activate calendar selection for each selected physical package and one shared choice for selected authored examples. All Debug profiles inventory the new `packages` question and retain inactive calendar/epoch answers.
