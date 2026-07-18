# FutureProg Type System

## Overview

FutureProg no longer uses a `long`-backed enum as its runtime type model. The engine now uses a `readonly struct` named `ProgVariableTypes` that preserves the existing flags-style programming model while removing the 64-bit ceiling for new types.

The following usage patterns are intentionally preserved:

- `ProgVariableTypes.Character`
- `ProgVariableTypes.Character | ProgVariableTypes.Collection`
- `type.HasFlag(ProgVariableTypes.Collection)`
- `type.CompatibleWith(ProgVariableTypes.Perceivable)`
- `type == ProgVariableTypes.Boolean`

## Runtime Model

### `ProgVariableTypes`

`ProgVariableTypes` is a `readonly struct` backed by `BigInteger`.

It provides:

- static readonly fields for all legacy concrete types and aliases
- overflow-era exact types such as `ProgVariableTypes.LegalClass` that are no longer constrained by the legacy 64-bit enum bridge
- bitwise operators `|`, `&`, `^`, `~`
- equality operators `==`, `!=`
- compatibility helpers such as `HasFlag(...)`, `CompatibleWith(...)`
- modifier-aware helpers such as `ExactKind`, `ElementKind`, `IsCollection`, `IsDictionary`, `IsCollectionDictionary`, `IsLiteral`, and `IsExactType`
- persistence helpers `ToStorageString()`, `FromStorageString(string)`, `FromLegacyLong(long)`, and `TryParse(...)`

### `ProgTypeKind`

Exact singular dispatch is represented by a companion enum named `ProgTypeKind`.

This is used when code wants exact-type switching without depending on enum constants for the full mask value. Call sites should prefer:

- `switch (type.ExactKind)` for exact leaf types
- property patterns for modifier-aware dispatch
- guarded `case var t when ...` branches for alias-mask matching

### `ProgVariableTypeCode`

The engine also keeps a legacy enum bridge, `ProgVariableTypeCode`, for two compatibility scenarios:

- attribute arguments that cannot accept a custom struct
- legacy-style exact switching in places where a direct `switch` over constants is still the clearest shape

`ProgVariableTypes.LegacyCode` returns a matching `ProgVariableTypeCode` when the value is still representable as a legacy 64-bit value; otherwise it returns `Unknown`.

New exact types added after the legacy bit range, such as `LegalClass`, should generally be handled with `type.ExactKind`, `type == ProgVariableTypes.SomeType`, or other `ProgVariableTypes`-native checks rather than relying on `LegacyCode` switches.

## Registry

`ProgVariableTypeRegistry` is now the central registry for shared FutureProg type metadata and behavior. It owns:

- display names
- parse aliases
- exact-kind lookup
- flag enumeration
- description formatting

This centralises logic that was previously spread across multiple FutureProg helpers and switches.

## Persistence Format

FutureProg type persistence now uses a canonical versioned string definition:

- `v1:<hex-mask>`

Examples:

- `ProgVariableTypes.Text` -> `v1:1`
- `ProgVariableTypes.Character | ProgVariableTypes.Collection` -> `v1:408`

The hex mask is stored big-endian and lower-case. This format is stable for both legacy 64-bit masks and any future overflow values.

## Database Changes

The following persisted type columns now use string definitions instead of `BIGINT`:

- `FutureProgs.ReturnTypeDefinition`
- `FutureProgs_Parameters.ParameterTypeDefinition`
- `VariableDefaults.OwnerTypeDefinition`
- `VariableDefinitions.OwnerTypeDefinition`
- `VariableDefinitions.ContainedTypeDefinition`
- `VariableValues.ReferenceTypeDefinition`
- `VariableValues.ValueTypeDefinition`

The runtime loads and saves these string definitions directly via `ProgVariableTypes.FromStorageString(...)` and `ToStorageString()`.

The EF model classes keep non-mapped legacy-style `long` compatibility properties for importer/seeder convenience, but production persistence is driven by the definition-string columns.

## Command Surface And Builder Impact

Builder-facing parsing now routes through `ProgVariableTypes.TryParse(...)` rather than enum parsing.

Type display and description logic is centralised through the registry-backed `Describe()` behavior. Existing player/builder output continues to use the same symbolic type names where possible.

## Runtime Safety Invariants

FutureProg parameter and local-variable references are case-insensitive. Persisted parameter names retain their authored casing for display and integration schemas, while compiler and runtime variable spaces normalise those names for lookup. A prog may not define two parameters whose names differ only by case.

Collection variables must expose `IProgVariable` elements at runtime, even when a helper or dot reference builds a collection from scalar CLR values such as `string`, `decimal`, `bool`, `DateTime`, `TimeSpan`, `MudDateTime`, or `Gender`. The `CollectionVariable` constructor normalises those scalar elements so collection extension functions, admin result display, and dot references like `first`, `last`, and `reverse` all see the same element shape.

Variable-register persistence must be total for every type that can be registered and saved. Value types, including `LiquidMixture`, serialise through value XML rather than reference IDs; unsupported or null preserved values must not create null `IVariableValue` entries. Resetting a stored register value removes the persisted override row and falls back to the default value.

Script-time helpers that search, roll, or evaluate user-authored formulas must enforce bounded work. Weekday occurrence helpers reject zero or excessive occurrence counts, dice formulas have explicit dice/sides/roll limits, exploding dice must not be guaranteed infinite, and formula evaluation fails closed on invalid custom-function arguments, overflow, or non-finite numeric output.

Writing text is not exposed through the `writing.text` FutureProg dot reference. Scripts may inspect writing metadata, but readable text still goes through the normal in-character read workflow so language, literacy, script, and access checks remain authoritative.

## Date, Time, And Celestial Event Values

`ProgVariableTypes.MudDateTime` remains the FutureProg type for in-game dates and times. `MudDateTime` values now expose a `mudinstant` dot reference that returns the absolute `MudInstant` storage string for the value.

Celestial event built-ins return `MudDateTime` and use the supplied room or zone as the observer geography:

- `nextsunrise(location|zone, celestialId, calendar[, occurrence])`
- `nextsunset(location|zone, celestialId, calendar[, occurrence])`
- `nextsolarlongitude(location|zone, celestialId, calendar, longitudeDegrees[, occurrence])`
- `nextnewmoon(location|zone, moonId, calendar[, occurrence])`
- `nextfullmoon(location|zone, moonId, calendar[, occurrence])`
- `nextvisiblecrescent(location|zone, sunId, moonId, calendar[, occurrence])`

The optional `occurrence` argument returns the nth next event. Invalid zones, calendars, celestial IDs, unsupported ephemeris types, or bounded-search failures return `MudDateTime.Never`.

## Migration Expectations

EF migrations for this subsystem should:

1. Add the new definition columns.
2. Backfill them from the old bigint values using `v1:<hex>` conversions.
3. Switch runtime and tools to the new definition columns.
4. Remove the obsolete bigint type columns once the new columns are authoritative.

Because the storage format is versioned, future internal representation changes can be handled without another schema redesign.
