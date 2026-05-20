# FutureMUD Celestial System Tests

## Purpose
Celestial changes are risky in two different ways:

- runtime math can drift or break observer-frame transforms
- seeded stock packages can become inconsistent with the runtime

The automated suite therefore keeps runtime regressions and seeder regressions separate.

## Coverage Split

| Test project | Scope |
| --- | --- |
| `MudSharpCore Unit Tests` | Deterministic numeric regression coverage for `Sun`, `PlanetaryMoon`, `PlanetFromMoon`, and `SunFromPlanetaryMoon`, plus non-24x60 direction sampling checks |
| `DatabaseSeeder Unit Tests` | Stock package creation, linkage, package dependency rules, and seeded-constant regression checks |

## Runtime Coverage
The modern runtime coverage lives in four focused suites:

- `CelestialTests.cs`
- `PlanetaryMoonTests.cs`
- `PlanetFromMoonTests.cs`
- `SunFromPlanetaryMoonTests.cs`

The intent is no longer "basic smoke coverage". Each supported modern model has deterministic numeric regression rows with at least three fixed data points. Those rows assert concrete outputs such as:

- current day number
- right ascension and declination when available or practically accessible
- elevation
- azimuth
- illumination
- phase classification
- eclipse outcome where relevant
- time-of-day classification
- movement direction

The linked models also assert relationships back to their source objects, such as:

- `PlanetFromMoon` remaining opposite the linked moon in equatorial coordinates
- `PlanetFromMoon` illumination remaining complementary to the linked moon
- `SunFromPlanetaryMoon` remaining close to, but not identical to, the root solar direction near conjunction

## Direction Sampling Coverage
All four runtime suites retain explicit non-24x60 clock coverage for movement direction sampling.

That matters because the direction logic compares the current sky position with the sky position one in-game minute earlier. The implementation must use the actual clock dimensions, not a hidden Earth-like `1 / 1440` assumption.

## Seeder Coverage
`DatabaseSeeder Unit Tests/CelestialSeederTests.cs` verifies:

- Earth sun package creation
- Earth moon-view package creation and linkage
- gas giant moon-view package creation and linkage
- dependency failure when the Earth moon package has no matching root sun
- rerun detection through `ShouldSeedData`
- seeded orbital and observer-frame constants for the stock packages

The seeded-constant assertions now cover the corrected modern data, including:

- solar eccentricity, semi-major axis, and apparent angular radius
- moon semi-major axis
- moon sidereal epoch and sidereal rate
- `PlanetFromMoon` stored `SunAngularRadius`

## Removed Legacy Coverage
There is no longer a supported `OldSun` runtime path, so there is no legacy compatibility suite for that type.

If an existing world still contains persisted `OldSun` data, load now fails explicitly and the content must be migrated to the modern `Sun` model.

## When to Add More Coverage
Add or expand numeric regression coverage whenever a change touches:

- orbital equations
- true-anomaly approximations
- angle normalization
- local sidereal time handling
- observer-frame transforms
- eclipse thresholds
- illumination formulas
- day-number or fractional-day handling

Seeder tests should be updated whenever a change touches:

- stock constants
- XML authoring shape
- linked IDs
- package detection
- package dependency rules
