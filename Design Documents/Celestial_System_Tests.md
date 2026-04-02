# FutureMUD Celestial System Tests

## Purpose
This document explains the current automated coverage for the celestial subsystem and where that coverage lives.

The celestial system has two main kinds of tests:

- runtime tests in `MudSharpCore Unit Tests`
- seeder tests in `DatabaseSeeder Unit Tests`

Both matter. Celestial work is usually a combination of numeric orbital math and linked content authoring, and those two concerns fail in different ways.

## Current Coverage Split

| Test project | Scope |
| --- | --- |
| `MudSharpCore Unit Tests` | Runtime celestial math, visibility behavior, phase behavior, time-of-day logic, trigger behavior, and non-24x60 clock support |
| `DatabaseSeeder Unit Tests` | Stock package creation, linkage correctness, dependency rules, and repeatability state |

## Runtime Coverage
The runtime celestial tests are currently concentrated in four files.

### `CelestialTests.cs`
This suite covers the current `Sun` implementation, which is the `NewSun` runtime class.

The suite verifies:

- orbital math regression for the modern solar implementation
- mean anomaly continuity across fractional day progression
- support for `CurrentDayNumberOffset`
- direction sampling using actual clock length instead of a hard-coded `1440`
- trigger selection behavior

These tests are the numeric safety net for changes to `NewSun`.

### `PlanetaryMoonTests.cs`
This suite covers the moon viewed from a parent planet.

The suite verifies:

- phase cycle classification
- illumination behavior at key points such as full and new moon
- trigger selection behavior
- direction sampling using actual clock length

These tests protect the phase math, the local sky transform, and the recent clock-length fix.

### `PlanetFromMoonTests.cs`
This suite covers the parent planet viewed from the moon.

The suite verifies:

- right-ascension and declination opposition relative to the linked moon
- illumination complement relative to the moon phase
- eclipse detection against the linked root sun
- tidally locked behavior assumptions
- direction sampling using actual clock length

These tests are especially important because `PlanetFromMoon` is mostly derived behavior rather than standalone orbital data.

### `SunFromPlanetaryMoonTests.cs`
This suite covers the moon-local sun representation.

The suite verifies:

- altitude changes over time
- time-of-day classification
- direction sampling using actual clock length

These tests protect the linked-object transformation from root solar coordinates into a moon-local sky frame.

## Seeder Coverage
The seeder coverage currently lives in `DatabaseSeeder Unit Tests/CelestialSeederTests.cs`.

That suite verifies:

- Earth moon package creation creates all three linked objects
- Earth moon package blocks when no suitable Earth-facing sun exists
- gas giant moon package is self-contained and creates all expected linked objects
- `ShouldSeedData` reports partial-package installations as `ExtraPackagesAvailable`

These tests are the protection against broken stock content, broken linked IDs, and regressions in rerun behavior.

## What the Current Suite Proves
The current test suite gives a reasonable level of confidence in five areas.

| Area | What is currently proven |
| --- | --- |
| `NewSun` orbital math | The modern solar pipeline still produces expected numeric behavior |
| Moon phase and illumination | `PlanetaryMoon` phase and light output remain consistent |
| Parent-planet inversion | `PlanetFromMoon` remains the inverted representation of the linked moon |
| Moon-local solar frame | `SunFromPlanetaryMoon` stays synchronized with the linked root sun and moon timing |
| Seeder package coherence | Stock celestial packages remain linked, repeatable, and dependency-aware |

## Known Gaps
The current gaps should be treated as real documentation of current state, not as implied future commitments.

### `OldSun` gap
No dedicated unit-test suite was found for the legacy `OldSun` implementation.

That means:

- the legacy type is documented and still loadable
- its current behavior is mostly protected by runtime stability and manual compatibility, not by a comparable modern numeric regression suite

Any change to `OldSun` should therefore be treated conservatively.

### Zone and cache integration gap
There is not currently a dedicated focused unit-test suite for:

- zone celestial cache initialization
- zone light recalculation across minute updates
- shard-to-zone celestial reassignment behavior

Those behaviors are exercised by runtime use, but not isolated by a dedicated celestial integration suite.

### Command-surface gap
The player and builder command surfaces that consume celestial data are not primarily tested in the celestial suites.

Examples:

- `time`
- `shard set <shard> celestials ...`
- `wc set celestial ...`
- `season set celestial ...`

Those commands are important integration points, but the core celestial tests do not attempt to prove command text or builder workflow behavior end to end.

## When to Add Numeric Regression Coverage
Add or expand runtime numeric tests when a change touches:

- any orbital equation
- any angle normalization rule
- phase or illumination formulas
- time-of-day classification thresholds
- coordinate transforms between equatorial and local sky frames
- `CurrentDayNumber`, `CurrentCelestialDay`, or sidereal timing rules
- linked-object behavior where one celestial derives from another

As a rule, if a code change alters how a celestial computes an angle or illumination value, it should come with at least one deterministic numeric regression test.

## When Seeder Changes Need Tests
Add or expand seeder tests when a change touches:

- package detection
- linked object creation
- package dependency rules
- package markers
- stock constants
- rerun behavior
- legacy compatibility detection

Seeder changes are especially prone to silent regressions because they often operate on XML authoring rules rather than runtime behavior. Tests should therefore assert actual generated definitions and link relationships.

## When Non-24x60 Clock Scenarios Must Be Tested
FutureMUD supports clocks that are not exactly 24 hours by 60 minutes.

Whenever a change touches:

- minute sampling for direction detection
- day fractions
- current-time calculations
- celestial motion logic that implicitly assumes a fixed day length

you should add or update non-24x60 clock coverage.

The recent direction-sampling fix is a good example: it needed explicit tests because a hidden `1 / 1440` assumption would otherwise look correct on ordinary Earth-like clocks while still being wrong for custom worlds.

## Recommended Test Strategy for Future Celestial Types
For any new celestial type, aim to add three layers of coverage:

1. numeric runtime tests for the local sky calculation pipeline
2. linkage tests if the type depends on other celestial objects
3. seeder tests if the type ships as stock content

If the type can drive time-of-day or illumination, add those tests too.

## Practical Verification Guidance
For day-to-day development:

- run the relevant targeted `MudSharpCore Unit Tests` when changing runtime celestial code
- run `DatabaseSeeder Unit Tests` when changing seeder packages or authoring rules
- use implementor celestial analysis or weather analysis when you need broader integration sanity checking

The test suite is strongest when changes are small and covered close to the formulas they affect.
