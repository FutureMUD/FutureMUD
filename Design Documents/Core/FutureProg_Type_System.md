# FutureProg Type System

## Overview

FutureProg no longer uses a `long`-backed enum as its runtime type model. The engine now uses a `readonly struct` named `ProgVariableTypes` that preserves the existing flags-style programming model while removing the 64-bit ceiling for new types.

Invocation frames, lexical scopes, static caching, recursion, allocation ownership, and benchmark invariants are documented in [FutureProg Execution Runtime](./FutureProg_Execution_Runtime.md).

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

## Naming Values

FutureProg exposes three first-class naming types:

- `NameCulture` is a reference type for the reusable naming rules and patterns.
- `RandomNameProfile` is a reference type for a weighted random-name source within a name culture.
- `PersonalName` is a value type. It retains its name culture and individual name elements when it crosses a variable-register boundary, rather than relying on the transient `FrameworkItem` ID of a generated name. Personal names compare by their culture and raw elements, case-insensitively.

The principal dot references are:

- `nameculture.id`, `nameculture.name`, `nameculture.randomnameprofiles`, and `nameculture.nameusages`.
- `randomnameprofile.id`, `randomnameprofile.name`, `randomnameprofile.culture`, `randomnameprofile.gender`, `randomnameprofile.ready`, `randomnameprofile.randomname`, and `randomnameprofile.names`.
- `personalname.culture`, formatted forms such as `name`, `givenname`, `simplefullname`, `affectionate`, `surname`, and `fullwithnickname`, plus `elements` and the individual raw element properties such as `birthname`, `diminutive`, `surnameelement`, `nickname`, `patronym`, and `regnalname`.

`character.personalname` exposes the character's real `PersonalName`; `character.currentname` exposes its current alias. `chargen.personalname` exposes the selected name or a typed null while it has not been selected. The established text properties (`character.name`, `fullname`, `surname`, `cname`, and their chargen equivalents) remain text for compatibility with existing progs.

Culture and ethnicity keep their existing text `namecultures` collections. Their new `namecultureobjects` collections provide the typed `NameCulture` values, and their gender-specific `malenameculture`, `femalenameculture`, `neuternameculture`, `nonbinarynameculture`, and `indeterminatenameculture` references provide individual configurations. Ethnicity-specific gender properties return null when the parent culture's configuration applies.

The built-in naming functions are:

- `tonameculture(number|text)`
- `torandomnameprofile(number|text)` and `torandomnameprofile(nameculture, text)`
- `getpersonalname(nameculture, text)`
- `randompersonalname(randomnameprofile)`

All lookup and creation functions return a typed null when a target is absent, a profile is not ready, or supplied name text does not validate. For `prog execute`, a `PersonalName` argument is entered as a quoted-or-unquoted name culture followed by the complete name text.

## Phase 1 Builder References

Phase 1 promotes globally resolvable, builder-authored configuration and world objects to exact FutureProg reference types. Each type is a collection item and a reference value, so it can be passed to typed built-ins, returned in collections, and stored in the variable register. `prog execute` resolves these exact types by ID or name.

Every Phase 1 type has an ID-or-name lookup with the following shape: `function(number|text)`. Missing targets return a typed null.

| Type | Lookup | Principal dot references | Typed integrations |
| --- | --- | --- | --- |
| `Tag` | `tag` | `id`, `name`, `fullname`, `parent` | `istagged(..., tag)` |
| `ItemPrototype` | `itemprototype` | `id`, `name`, `uniquename`, `status`, `revision`, `shortdesc`, `fulldesc`, `material`, `weight`, `preventmanualload` | `loaditem` |
| `NPCTemplate` | `npctemplate` | `id`, `name`, `uniquename`, `status`, `revision`, `templatetype` | `loadnpc` |
| `OutfitTemplate` | `outfittemplate` | `id`, `name`, `description`, `exclusivity`, `itemcount` | `loadoutfittemplate` |
| `Vehicle` | `vehicle` | `id`, `name`, `exterioritem`, `location`, `layer`, `routeposition`, `occupants`, `controller`, `activejourney`, `disabled`, `destroyed` | vehicle readiness, train-weight, and tow-stress functions |
| `CelestialObject` | `celestial` | `id`, `name`, `currentcelestialday`, `celestialdaysperyear`, `determinestimeofday` | celestial elevation, position, and astronomical-event functions |
| `Grid` | `grid` | `id`, `name`, `gridtype`, `locations` | `connecttogrid`, `extendgrid`, `withdrawgrid` |
| `CharacteristicDefinition` | `characteristicdefinition` | `id`, `name`, `description`, `type`, `parent`, `defaultvalue` | `getcharacteristicvalue`, `setcharacteristic` |
| `CharacteristicValue` | `characteristicvalue` | `id`, `name`, `definition`, `value`, `basicvalue`, `fancyvalue`, `pluralisation` | `getcharacteristicvalue`, `setcharacteristic` |
| `AgricultureFieldProfile` | `fieldprofile` | `id`, `name`, `description`, default-score and allowed-use configuration | `createfield` |
| `AgricultureCropDefinition` | `cropdefinition` | `id`, `name`, `description`, growth, harvest, climate, pollination, and yield configuration | `startfieldproject` |
| `AgricultureHerdDefinition` | `herddefinition` | `id`, `name`, `description`, grazing, condition, `npctemplate`, and output configuration | `startfieldproject`, `drawfieldherd`, `absorbnpcintofieldherd` |
| `AgricultureWoodlandDefinition` | `woodlanddefinition` | `id`, `name`, `description`, establishment, harvest-cycle, and yield configuration | `startfieldproject` |
| `AgricultureOperation` | `agricultureoperation` | `id`, `name`, `description`, operation and target types, uses, project, and result configuration | `startfieldproject` |

The established text fields on `AgricultureField` remain compatible. Its additional typed properties are `profiledefinition`, `cropdefinition`, and `woodlanddefinition`; these avoid forcing builders to parse legacy text names when chaining a field into the new definition types.

## Phase 2 Economy and Communication References

Phase 2 promotes the durable property workflow records, economic zones, and channels. Every promoted type is an exact reference type, can be placed in a collection or persistent variable register, and can be supplied to `prog execute`.

The globally named roots use ID-or-name lookups. Property keys, leases, lease orders, and sale orders use numeric ID lookups only because their names are scoped to their owning property rather than globally unique. The runtime resolves those child records through the live collections of their owning property, including retained expired lease and lease-order history.

| Type | Lookup | Principal dot references | Typed integrations |
| --- | --- | --- | --- |
| `Property` | `property(number|text)` | `economiczone`, `locations`, `saleorder`, `leaseorder`, `lease`, retained lease/order history, `keys`, sale/lease state | `property(location)`, property access queries |
| `PropertyKey` | `propertykey(number)` | `property`, `item`, `added`, `replacementcost`, `returned` | durable register values and property-key inspection |
| `PropertyLease` | `propertylease(number)` | `property`, `leaseorder`, leaseholder identity fields, payments, interval, dates, renewal, bond, tenant count | `ispropertytenant` |
| `PropertyLeaseOrder` | `propertyleaseorder(number)` | `property`, pricing, bond, interval, duration, renewal/relist/novation/rekey settings, consent counts, eligibility prog names | property lease inspection |
| `PropertySaleOrder` | `propertysaleorder(number)` | `property`, reserve price, status, start, duration, sale visibility, consent counts | property sale inspection |
| `EconomicZone` | `economiczone(number|text)` | `zone`, `currency`, `controllingclan`, financial-period configuration, `properties`, estate and service cells | property/economy selection |
| `Channel` | `channel(number|text)` | command words, presentation and command-tree settings, eligibility prog names, colour, Discord configuration | `sendchannel(channel, character, text)` |

`ispropertyowner(property, character)`, `ispropertyleaseholder(property, character)`, and `ispropertytenant(property, character)` use the property system's authoritative access checks. `sendchannel` is intentionally a void operation and delegates to `IChannel.Send(character, text)`, preserving the normal speaker, membership, listener, missed-listener, and Discord paths. It does not add a system-message overload or subscription mutation surface.

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

Variable-register persistence must be total for every type that can be registered and saved. A type is registerable only when it has a value serialisation or a stable, globally resolvable reference; runtime-scoped `Chargen`, `Exit`, `Effect`, `Trap`, `Outfit`, and `OutfitItem` values are rejected rather than being saved as unusable reference IDs. Value types, including `LiquidMixture`, serialise through value XML rather than reference IDs; unsupported or null preserved values must not create null `IVariableValue` entries. Resetting a stored register value removes the persisted override row and falls back to the default value.

Script-time helpers that search, roll, or evaluate user-authored formulas must enforce bounded work. Weekday occurrence helpers reject zero or excessive occurrence counts, dice formulas have explicit dice/sides/roll limits, exploding dice must not be guaranteed infinite, and formula evaluation fails closed on invalid custom-function arguments, overflow, or non-finite numeric output.

Writing text is not exposed through the `writing.text` FutureProg dot reference. Scripts may inspect writing metadata, but readable text still goes through the normal in-character read workflow so language, literacy, script, and access checks remain authoritative.

## Date, Time, And Celestial Event Values

`ProgVariableTypes.MudDateTime` remains the FutureProg type for in-game dates and times. `MudDateTime` values now expose a `mudinstant` dot reference that returns the absolute `MudInstant` storage string for the value.

Celestial event built-ins return `MudDateTime` and use the supplied room or zone as the observer geography:

- `nextsunrise(location|zone, celestialId|celestial, calendar[, occurrence])`
- `nextsunset(location|zone, celestialId|celestial, calendar[, occurrence])`
- `nextsolarlongitude(location|zone, celestialId|celestial, calendar, longitudeDegrees[, occurrence])`
- `nextnewmoon(location|zone, moonId|moon, calendar[, occurrence])`
- `nextfullmoon(location|zone, moonId|moon, calendar[, occurrence])`
- `nextvisiblecrescent(location|zone, sunId|sun, moonId|moon, calendar[, occurrence])`

The optional `occurrence` argument returns the nth next event. Invalid zones, calendars, celestial references, unsupported ephemeris types, or bounded-search failures return `MudDateTime.Never`.

## Migration Expectations

EF migrations for this subsystem should:

1. Add the new definition columns.
2. Backfill them from the old bigint values using `v1:<hex>` conversions.
3. Switch runtime and tools to the new definition columns.
4. Remove the obsolete bigint type columns once the new columns are authoritative.

Because the storage format is versioned, future internal representation changes can be handled without another schema redesign.
