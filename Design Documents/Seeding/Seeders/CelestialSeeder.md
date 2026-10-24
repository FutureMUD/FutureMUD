# CelestialSeeder

## Start here to add content

Edit [CelestialSeeder.cs](../../../DatabaseSeeder/Seeders/CelestialSeeder/CelestialSeeder.cs): **BuildSunDefinition / EnsureEarthSunPackage for physical packages; AuthoredCelestialPresets for authored examples**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Sets up Suns, Moons, etc.
Baseline availability: **Enabled**. The entrypoint is [CelestialSeeder.cs](../../../DatabaseSeeder/Seeders/CelestialSeeder/CelestialSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `installsun`, `suncalendar`, `sunname`, `sunepoch`, `installmoon`, `mooncalendar`, `moonname`, `moonepoch`, `installgasgiantmoon`, `gasgiantcalendar`, `gasgiantsunepoch`, `gasgiantmoonepoch`, `installauthored`, `authoredcalendar`, `installarmageddon`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Armageddon option

After choosing `Armageddon` in Time Seeder, choose yes for `installarmageddon`. Its default is yes when that calendar exists, otherwise no; the Earth sun/moon defaults become no for an Armageddon calendar. Existing package questions remain independent. The three bodies install together: Suk-Krath (`ScriptedSun`), Jihae and Lirathu (`ScriptedMoon`). No additional calendar or epoch question is needed: the canonical Armageddon calendar drives all three and midnight opening year 1 anchors the schedule.

The package requires a midnight-based, fixed-month, epoch-year-1 calendar of three 231-day months and a nine-hour 60-by-60 clock. Incompatible builder customisations fail validation and direct execution before any requested celestial installation writes. Clock speed and year-name edits do not invalidate the sky schedule. The package has no implicit shard attachment; builders select all three, placing Suk-Krath first if it should supply time of day, through `shard set <shard> celestials <ids...>`.

Stable `SeederPreset` markers `ArmageddonSun`, `ArmageddonJihae` and `ArmageddonLirathu` identify package members independently of display names. Reruns preserve existing IDs/definitions and restore only missing members. Any partial authored package reports additional packages available. Removing a seeder marker relinquishes that member's stock identity, as with other authored presets.

All three apparent paths repeat over 693 days using sparse fifteen-minute source keys and authoritative minute samples. They share a seasonal inclined track; turn rates are 1, 6/7 and 10/11 per day. The sun has a seven-hour horizon interval (dawn at hour 1, sunset at hour 8), two hours below the horizon and editable maximum elevations descending 60 to 30 degrees, remaining low at 30, then ascending to 60. This is a global authored fantasy sky; latitude/longitude do not change it. Local room/weather/perception modifiers still apply. The original's seven lit/twilight hours are represented as seven geometric above-horizon hours; native dawn/dusk classifications still use the existing altitude thresholds.

Exact common keyframes coincide at midnight every 77 days, below the horizon. `custom:conjunction` on each body exposes the shared schedule; it is a named authored event, not physical geometric-solver support. Months have three occurrences in `(month start, next month start]`, avoiding double-counting their opening instant. The full annual compiled-sample regression checks nine intended coincidences and no additional ones.

Moon phase channels preserve the source day bands independently of positions, using representative native phase fractions and jumps at day boundaries. Lirathu is full on days 115-118 and new on 1-2/230-231; Jihae is full on 106-109/222-225 and new on 49-51/164-167. Both crescent thicknesses retain distinct fractions within the native crescent phase. Sun peak 98,000 lux and moon peak 0.3/0.6 lux are editable starter tuning; moon light is phase-scaled once and zero below the horizon. These are not recovered physical measurements. No Echri is installed.

See [source evidence and accepted adaptation](../../Armageddon/Armageddon_Time_And_Celestial_Proposal.md). `ArmageddonTimeAndSkyTests` covers annual compiled geometry, daylight, phase dates, prerequisites and preserving partial reruns.

## Inputs, representative trace and practical authoring guide

**Existing physical record route.** `SeederQuestions` (CelestialSeeder.cs around 85-180) includes `installsun`, `suncalendar`, `sunname`, and `sunepoch`; filters hide follow-up questions unless the answer is yes and an equivalent package is missing. `SeedData` dispatches affirmative choices to `EnsureEarthSunPackage`. That helper first uses `GetEarthSunCandidates` and returns the existing candidate if found; otherwise it resolves the chosen calendar and feed clock, calls `BuildSunDefinition` with the user name/epoch plus `EarthSunPackage` stable package marker and constants (Earth orbital year, anomaly, eccentricity, radius, epoch, illumination and scattering parameters), creates a typed Sun via `CreateCelestial`, then `context.Celestials.Add(sun)` and `SaveChanges` (`CelestialSeeder.cs:865-887`). `HasEarthSunPackage` and `IsEarthSunCelestial` recognize a tagged package or legacy definition with matching canonical orbital fields, avoiding duplicate stock installs. Linked Moon and gas-giant examples comprise multiple related rows and are installed through separate Ensure helpers.

**Add-one recipe depends on content intent.** Do not add another `Celestial` initializer with guessed XML. A new geography-independent authored demo is supported by explicit preset registry in `MudSharpCore/Celestial/Authored/AuthoredCelestialPresets.cs`: `Names` currently has six literals (`RailSun`, `RailMoon`, `ScriptedSun`, `ScriptedMoon`, `MorningStar`, `MorningGlow`); `Create(preset, calendarId, minutesPerHour, hoursPerDay)` validates membership and maps naming conventions into `AuthoredCelestialDefinition` fields including kind, calendar, annual period, path, light mode/profile and moon phase; scripted/morning examples change track keys and add milestone/echo entries. To add `ExampleComet`, extend `Names` and add an explicit branch in `Create` (the current logic implicitly classifies based on name ending/starting with Moon/Scripted/Morning, so an arbitrary name would otherwise default to a rail sun). Add that preset's source trajectory/phase/lighting/milestones and test it through formatter/compiler. `EnsureAuthoredPackage` in [CelestialSeeder.Authored.cs](../../../DatabaseSeeder/Seeders/CelestialSeeder/CelestialSeeder.Authored.cs) loops names not already present, calls `Create`, compiles (`CompiledAuthoredCelestial`) using selected calendar and clock dimensions, serializes and adds a `Celestial` row. Because `HasAuthoredPackage` currently checks all preset names, adding one changes package completeness and enables incremental install.

**Transaction/ownership.** Main seeder wraps all requested choices in BeginTransaction, `SaveChanges`, Commit and catch/Rollback/rethrow (`CelestialSeeder.cs:185-217`). Physical packages identify ownership through serialized package metadata and conservative legacy matching. `SeederMetadataRegistry.cs:59-68` describes Additive / InstallMissing: existing IDs/customized definitions survive; missing package members are added. Requires Core account; Weather also depends on a celestial. It asks no separate explicit authored yes/no if already-installed (filter simply hides). Authored examples are not auto-bound to shards. Relevant tests are [AuthoredCelestialSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/AuthoredCelestialSeederTests.cs) and [CelestialSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/CelestialSeederTests.cs); not executed.

For physical package additions, inspect `GetPackageName`/`PackageMetadata` and each `Has...Package` detector alongside the `Ensure...Package` method; partial packages are often considered installed based on required type sets (e.g. gas giant checks Sun, PlanetaryMoon, PlanetFromMoon, SunFromPlanetaryMoon), while older data may be adopted by numerical signature. Add a related member to the creator, record the same stable package marker in its definition, and update completeness detection only if the new member is required to make the package complete; otherwise older installs could be needlessly reclassified. Names entered by the user (`sunname`, `moonname`) feed the generated definition and are not an identity key. For authored examples, `Names` is also the completeness manifest and matching serialized `SeederPreset` is the install identity; use a stable unique preset key and ensure it can round-trip in parser/compiler. A test should assert each catalog name has exactly one installed row of expected kind and that rerun retains its ID while adding only newly registered missing names. The outer transaction permits linked celestial records to be created in dependency order before final commit; `SaveChanges` inside the Sun helper makes the Sun ID available for related rows but remains within the transaction.


## Declared rerun and ownership contract

Metadata declares `Additive / InstallMissing`.

Adds missing stock physical packages and optional authored celestial examples.

Authored examples use stable preset identities; reruns preserve existing IDs and builder-edited definitions and add missing members. Physical packages retain their existing detection rules.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Armageddon verification result

The full DatabaseSeeder suite passed 2,204 cases with three Unix-only launcher permission cases skipped on Windows. The annual compiled-sample, phase and preserving-rerun regressions passed. Native MySQL run `run-37bf80360bf1` installed all three objects, reloaded definitions in fresh contexts, preserved retained IDs/customisations and restored only the deleted member. Its complete baseline replay/nonblank-refusal checksum gate and owned-instance shutdown also passed. No game-server/Telnet or climate calibration is claimed. See [the full evidence and remaining climate-template boundary](../../Armageddon/Armageddon_Time_And_Celestial_Proposal.md#implementation-verification).

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
