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

## Migration Expectations

EF migrations for this subsystem should:

1. Add the new definition columns.
2. Backfill them from the old bigint values using `v1:<hex>` conversions.
3. Switch runtime and tools to the new definition columns.
4. Remove the obsolete bigint type columns once the new columns are authoritative.

Because the storage format is versioned, future internal representation changes can be handled without another schema redesign.
