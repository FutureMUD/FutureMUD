# FutureMUD Vehicle Item Seeder Design Reference

## Purpose

This document is the implementation and authoring contract for the vehicle subcomponent of `ItemSeeder`. It is intended for agents and builders who add vehicle catalogues, review vehicle data, or extend the helper layer.

The first implementation provides:

1. rerunnable insertion of FutureMUD's canonical vehicle prototype graph;
2. safe creation of the exterior, access, cargo and equipment item prototypes associated with that graph;
3. intent-level helpers for the principal vehicle patterns currently supported by the engine;
4. validation of structural references, operational resources and plausible value ranges; and
5. a demonstration catalogue containing two terrestrial and two aquatic vehicles in each item-seeder era.

Read this document together with:

- `Design Documents/Vehicle_System.md` for the runtime vehicle system;
- `Item_Authoring_Guidelines.md` for player-facing item descriptions;
- `FutureMUD_Item_Seeder_Working_Guidelines.md` for catalogue and rerun conventions;
- `Seeded_Item_Components.json` for ordinary reusable item components;
- `Seeded_Materials.json` and `Seeded_Liquids.json` for exact seeded resource names; and
- `FutureMUDLibrary/Vehicles/VehicleEnums.cs` for persisted enum meanings.

The demonstration catalogue is representative, not exhaustive. Later passes should add culturally and technologically specific vehicles where the differences affect form, capacity, access, propulsion, operation or use. Do not create nominal variants merely to satisfy a per-culture quota.

## Runtime Architecture

A FutureMUD vehicle is a hybrid canonical-domain object with ordinary item projections. It is not just a large item carrying one XML component.

### Canonical vehicle graph

`VehicleProto` is the revisioned root. Its child records describe actual vehicle behaviour:

| Record | Responsibility |
|---|---|
| `VehicleCompartmentProto` | Logical occupied areas; for room-scale vehicles these correspond to persistent interior cells. |
| `VehicleCompartmentLinkProto` | Directed movement links between compartments. |
| `VehicleOccupantSlotProto` | Driver, crew and passenger capacity; required staffing; propulsion contributors. |
| `VehicleControlStationProto` | Associates control authority with an occupant slot. |
| `VehicleMovementProfileProto` | Cell-exit or route movement, environment, closure checks and resource requirements. |
| `VehiclePropulsionProfileProto` | Selectable surface-water propulsion such as self-powered, rowed, sail or outboard. |
| `VehicleAccessPointProto` | Doors, hatches, ramps, canopies and service openings. |
| `VehicleCargoSpaceProto` | Canonical cargo areas represented to item systems by container projections. |
| `VehicleInstallationPointProto` | Mount type and functional role accepted from an installed module. |
| `VehicleTowPointProto` | Towing direction, type, rating, pull multiplier and stress behaviour. |
| `VehicleDamageZoneProto` | Weighted damage areas, thresholds and whole-zone effects. |
| `VehicleDamageZoneEffectProto` | Disables a movement profile, access point, cargo space, installation point, tow point or all movement. |

### Item projections

The vehicle graph is exposed to ordinary item commands through linked item prototypes:

- The **exterior item** is the visible vehicle shell. Its generated `Vehicle Exterior` component identifies the canonical `VehicleProto`.
- An **access projection** is a hidden item carrying a generated `Vehicle Access Point` component. The access component itself owns open, closed and lock behaviour; do not add an ordinary door component.
- A **cargo projection** is a hidden item carrying both a generated `Vehicle Cargo Space` component and one ordinary container component. The vehicle owns the cargo-space state; the container component owns normal contents behaviour.
- An **installed module** is an ordinary portable item with `Vehicle Installable` plus the machinery, battery, liquid-container or other components needed to operate it.

Access and cargo projections must remain hidden, non-portable and non-skinnable. They are implementation projections, not goods that builders should place or clone directly.

### Vehicle scales

- `ItemScale`: occupants remain in the exterior item's ordinary cell. Appropriate for handcarts, coracles, kayaks, dinghies and comparable small vehicles.
- `RoomContainer`: occupants remain in the exterior cell but are logically contained by vehicle compartments. This is appropriate for most carts, wagons, cars, coaches and non-room-scale vessels.
- `RoomScale`: compartments are persistent interior cells. Every compartment requires a valid `InteriorTerrainId`. The seeder supports the data contract, but the first catalogue does not include a room-scale example because terrain and world-topology choices are installation-specific.

Surface-water travel does not imply `RoomScale`. Item-scale and room-container craft use `CellExit` movement with `SurfaceWater` as their movement environment.

## Seeder API and Data Contract

### Dispatch and era selection

The database-seeder host invokes `ItemSeeder` through `IDatabaseSeeder`. The vehicle dispatch file runs the established item pass first, then calls:

```csharp
SeedVehicleItemsAndPrototypes(eras);
```

This ordering is deliberate. Vehicle definitions resolve materials, liquids, traits, ordinary components, tags and previously seeded item prototypes from the caches populated by the ordinary item pass.

The accepted era tokens and tags are:

| Token | Era tag |
|---|---|
| `antiquity` | `Era / Antiquity Era` |
| `medieval` | `Era / Medieval Era` |
| `renaissance` | `Era / Renaissance Era` |
| `earlymodern` | `Era / Early Modern Era` |
| `revolution` | `Era / Industrial Era` |
| `modern` | `Era / Modern Era` |
| `atomic` | `Era / Nuclear Era` |
| `computer` | `Era / Information Age Era` |

Only selected eras are inserted. Shared operating equipment is inserted when the selected technology family requires it.

### Stable references

Root references must match:

```text
vehicle_<era>_<specific_name>
```

They are lowercase snake case and globally unique. Examples include `vehicle_medieval_trading_cog` and `vehicle_computer_electric_city_car`.

Generated item references append a stable role suffix:

- exterior: `<root>_exterior`
- access: `<root>_access_<child_key>`
- cargo: `<root>_cargo_<child_key>`

Child keys are lowercase snake case and unique within their child family. Once a catalogue has been published, treat root references, child keys and child names as durable identities. A cosmetic rename of an existing key can create a second child rather than updating the original.

### Definition records

`VehicleSeedSpec` is the authoring boundary. It contains the root identity and all child collections. Dedicated records prevent raw table inserts from silently omitting required relationships:

- `VehicleItemSeedSpec`
- `VehicleCompartmentSeedSpec`
- `VehicleCompartmentLinkSeedSpec`
- `VehicleOccupantSlotSeedSpec`
- `VehicleControlStationSeedSpec`
- `VehicleMovementProfileSeedSpec`
- `VehiclePropulsionSeedSpec`
- `VehicleAccessPointSeedSpec`
- `VehicleCargoSpaceSeedSpec`
- `VehicleInstallationPointSeedSpec`
- `VehicleTowPointSeedSpec`
- `VehicleDamageZoneSeedSpec`
- `VehicleDamageEffectSeedSpec`

Use the intent-level factories wherever the topology fits:

```csharp
CreateDraftCargoVehicle(...)
CreatePoweredRoadVehicle(...)
CreatePaddleCraft(...)
CreateSailCraft(...)
CreateMotorCraft(...)
```

Write an explicit `VehicleSeedSpec` when the vehicle genuinely has different topology, such as multiple separately accessed cabins, several control stations, unusual tow geometry or a novel movement profile. Do not bypass validation merely because a vehicle is exceptional.

### Minimum structural contract

Every vehicle must contain:

- one exterior item;
- at least one compartment;
- at least one driver slot;
- exactly one primary control station attached to a driver slot;
- at least one movement profile and exactly one default movement profile;
- at least one damage zone;
- unique keys and display orders within each child family; and
- valid references between all child keys.

A surface-water movement profile additionally requires at least one explicit propulsion row and exactly one default propulsion row.

### Player-facing item contract

Exterior and support items follow the ordinary item-authoring rules.

- Use a compact singular noun.
- Use a concise short description, normally beginning with `a` or `an`.
- Write each full description individually. Describe visible construction, shape, finish, wear and obvious affordances.
- Do not expose stable references, database terminology, component names or hidden mechanics in player-facing text.
- Weight is empty inherent weight in grams. Cargo and installed module contents add their own weight.
- The primary material must exactly match the seeded material name and should represent the main structural substance.
- Portable support equipment includes `Holdable`; vehicle exteriors normally do not.
- Finished exterior goods may be skinnable. Projections never are.
- Cost uses the normal item-seeder base-currency convention.

## Units and Recommended Ranges

The validator enforces hard safety rules; the ranges below are authoring guidance. A value passing validation may still be implausible.

| Value | Stored unit / hard rule | Recommended guidance |
|---|---|---|
| Item weight | grams; positive and finite | Start from plausible empty mass. Do not include cargo or fuel contents twice. |
| Item cost | base-currency decimal; non-negative | Preserve broad relative value rather than false historical precision. |
| Propulsion base move time | milliseconds; 250-300,000 | Usually 4,000-15,000 for small craft and 7,000-25,000 for heavy manual craft before runtime multipliers. |
| Route speed | metres/second; positive for `Route` | About 1-4 for slow draft/industrial travel, 5-15 for urban service, and 15-35 for faster road or rail examples. |
| Required power spike | watts; non-negative | Use for instantaneous cell-exit readiness only when an installed power producer is required. |
| Route power draw | watts; non-negative | Use for continuous electric route consumption. A powered route must consume fuel or power. |
| Fuel per cell exit | liquid volume; non-negative | Use for coarse cell-exit travel; calibrate against world cell scale. |
| Fuel per route metre | liquid volume/metre; non-negative | Use only for route movement. Keep values small and test long journeys. |
| Tow maximum | grams; positive and finite | Rate the tow point and available hitch gear together. The lower rating controls. |
| Character pull multiplier | multiplier; positive and finite | Around 1.0 is neutral; increase only where shafts, harness or gearing improve effective pull. |
| Tow warning ratio | ratio; positive | Commonly 0.75-0.9. Must not exceed failure-start ratio. |
| Tow maximum failure chance | probability; 0-1 | Keep low enough to warn before catastrophic repeated failures. |
| Damage maximum | abstract damage capacity; positive | Scale by structural importance and expected weapon/collision damage, not solely by mass. |
| Damage hit weight | relative weight; positive | Larger exposed zones receive larger weights. |
| Disabled threshold | fraction; `> 0` | Must be lower than destroyed threshold. |
| Destroyed threshold | fraction; `<= 1` | Must be greater than disabled threshold. |

Use finite values only. `NaN`, infinity and silent zero-speed route profiles are invalid.

## Propulsion and Movement Patterns

### Externally pulled terrestrial vehicles

Carts, wagons, carriages and drays use ordinary terrestrial movement plus tow points. They do not receive surface-water propulsion rows. A forward point is normally towable by draft gear; a rear point may allow another load to be coupled behind.

The seeded harness items demonstrate ordinary and heavy-team ratings. A vehicle is not automatically harnessed, populated with draft animals or made ready to move.

### Powered terrestrial cell-exit vehicles

A powered road vehicle uses:

- a terrestrial `CellExit` movement profile;
- a required installed role such as `propulsion`;
- a compatible installation point such as `land_engine` or `electric_drive`; and
- a separately seeded removable drive module.

Fuelled modules carry a fuel liquid container. Electric modules carry the appropriate battery/power components. The seeder does not install, fill, switch on or charge a module automatically.

### Route vehicles

`Route` movement is for vehicles constrained to `RouteCell` topology.

- `ExternallyPulled` route profiles depend on a valid towing arrangement.
- `Powered` route profiles must consume either fuel per metre or electrical power.
- Automatic operation is valid only for a powered route profile.
- Route speed must be positive.

World route topology, stops, signalling and scheduling are authored outside this item-seeder pass.

### Self-powered watercraft

`SelfPowered` is suitable for a coracle or kayak where the controller directly supplies effort. The profile resolves a suitable seeded trait from candidates such as Swimming, Rowing or Athletics. Speed and stamina expressions must remain finite over expected check outcomes.

### Rowed watercraft

`Rowed` requires one or more occupied slots marked `ContributesToPropulsion`. Every contributor must hold a usable item with the `Vehicle Oar` component. A nominal crew slot without an oar does not propel the vessel.

### Sailing craft

`Sail` speed uses the runtime `wind` variable. The demonstration sailing craft make sail the default and rowed propulsion a selectable fallback. Selection is explicit: the runtime does not silently switch from sail to oars when wind or staffing changes.

### Outboard craft

`OutboardMotor` uses the runtime `output` supplied by an installed functional motor. The vessel requires an `outboard_motor` installation point with the `propulsion` role. The seeded petrol outboard includes both `Vehicle Installable` and `Outboard Motor` components plus a fuel container.

The demonstration motor craft also provide emergency rowed propulsion. Rowing still requires staffed contributor slots and vehicle oars.

### Propulsion expressions

Supported expression variables depend on the propulsion type. The supplied helpers use:

- `outcome` for checked manual propulsion;
- `swimcost` for baseline stamina cost;
- `wind` for sail strength; and
- `output` for outboard performance.

Validation evaluates authored expressions across outcomes from -3 through +3 and requires positive finite speed and non-negative finite stamina cost. New variables are not created by naming them in a catalogue expression; the runtime must supply them first.

## Access, Cargo, Installation, Towing and Damage

### Access

Use access points for a real closable boundary: door, hatch, ramp, canopy or service opening. Set `MustBeClosedForMovement` when departure with the opening unsecured would be unsafe or mechanically prohibited. A cargo space may require one access point before its projection can be used.

### Cargo

Each cargo space needs exactly one suitable ordinary container component. Select capacity and open/closed behaviour from `Seeded_Item_Components.json`; do not invent a component name in the vehicle catalogue. The hidden projection receives both the container and generated cargo-space component.

### Installation points

`MountType` answers “what physical interface fits here?” while `RequiredRole` answers “what function must the installed item provide?”. Both must agree with the `Vehicle Installable` component on the module.

A movement-required installation point must declare a non-empty role. Damage can target the installation point and make an otherwise present module unusable.

### Towing

Tow points state whether they can tow, be towed, or both. `TowType` must be compatible at both ends. Hitch gear bridges users or vehicles to the tow system and imposes its own capacity.

All four optional stress values are specified together or omitted together:

- warning ratio;
- failure-start ratio;
- maximum failure chance; and
- damage multiplier.

### Damage

Every vehicle has weighted damage zones. Thresholds satisfy:

```text
0 < disabled < destroyed <= 1
```

Damage effects target stable child keys and may disable:

- all vehicle movement;
- one movement profile;
- an access point;
- a cargo space;
- an installation point; or
- a tow point.

Model an actual failure consequence. Do not add numerous cosmetic zones that have identical effects and no gameplay distinction.

## Idempotency and Stable Keys

The pass is designed to be rerun.

- Root ownership is resolved through the stable exterior item reference.
- Access and cargo ownership is resolved through stable projection item references.
- Generated component names are deterministic from root reference, role and child key.
- Named child rows are updated in place within their vehicle and revision.
- Existing links are resolved by their scoped endpoints and direction.
- Propulsion rows are resolved by movement profile and propulsion type.
- Damage effects are resolved by zone, target type and target id.

The seeder updates rows it can identify but does **not** delete omitted child rows. This is intentional: a builder may have extended a seeded vehicle after installation. Removing or structurally renaming a published child requires an explicit migration and a review of live instances.

Practical rules:

1. never recycle a root stable reference for a different vehicle;
2. never casually rename a published child key or name;
3. append new variants with new root references;
4. use deterministic ordering and exact seeded dependency names; and
5. test both a clean install and a rerun against the same database.

## Edge Cases and Failure Modes

| Condition | Expected result / author response |
|---|---|
| Unknown era token | Ignored; if no recognised token remains, the vehicle pass does nothing. |
| Unknown material, liquid or ordinary component | Seeding fails with a specific dependency error. Add or correct the prerequisite rather than substituting silently. |
| Missing compartment, slot, access, cargo, installation, tow or movement key | Validation fails before insertion. |
| No driver or not exactly one primary station | Validation fails. |
| Several default movement or propulsion rows | Validation fails. |
| Powered route has neither fuel nor power consumption | Validation fails. |
| Route profile has zero speed | Validation fails. |
| Surface-water movement has no explicit propulsion | Validation fails. |
| Rowed mode has no contributor slot | Validation fails. |
| Outboard mode has no compatible mount and role | Validation fails. |
| Projection is visible, portable or skinnable | Validation fails. |
| Installed module is absent, damaged, empty, unpowered or switched off | Live movement preflight fails and reports readiness reasons. |
| Required access point or tow link is open | Departure fails when the movement profile requires closure. |
| Selected water propulsion becomes unavailable | Movement fails; no automatic fallback is chosen. |
| Surface-water craft is on dry ground or the wrong ground layer | It can exist or be carried but cannot initiate surface-water travel. |
| Route vehicle is outside valid route topology | Route operation cannot proceed. |
| A seeded child is removed from source | Existing database row remains. Use a deliberate migration if removal is required. |
| Free-coordinate navigation, collision, signalling or dispatch is expected | Outside this first-pass seeder and current V1 movement boundary. Do not imply unsupported behaviour in descriptions. |

## Demonstration Catalogue

Every era has two terrestrial and two aquatic examples.

| Era | Stable reference | Pattern |
|---|---|---|
| Antiquity | `vehicle_antiquity_two_wheeled_handcart` | Item-scale externally pulled cart with open cargo. |
| Antiquity | `vehicle_antiquity_heavy_ox_wagon` | Heavy room-container draft wagon. |
| Antiquity | `vehicle_antiquity_reed_coracle` | Item-scale self-powered craft. |
| Antiquity | `vehicle_antiquity_coastal_sailing_boat` | Sail default, rowed fallback, deck and hold. |
| Medieval | `vehicle_medieval_market_cart` | Compact market cart. |
| Medieval | `vehicle_medieval_covered_wagon` | Closable access and gated cargo. |
| Medieval | `vehicle_medieval_clinker_rowboat` | Explicit rowed propulsion and oar dependency. |
| Medieval | `vehicle_medieval_trading_cog` | Large sailing cargo vessel. |
| Renaissance | `vehicle_renaissance_city_carriage` | Enclosed passenger carriage. |
| Renaissance | `vehicle_renaissance_artillery_wagon` | High-rated military cargo wagon. |
| Renaissance | `vehicle_renaissance_ship_launch` | Many-rower passenger and cargo launch. |
| Renaissance | `vehicle_renaissance_lateen_pinnace` | Lateen sailcraft with rowing fallback. |
| Early Modern | `vehicle_earlymodern_stagecoach` | High-capacity access-controlled coach. |
| Early Modern | `vehicle_earlymodern_freight_dray` | Open urban freight platform. |
| Early Modern | `vehicle_earlymodern_whaleboat` | Coordinated rough-water rowing craft. |
| Early Modern | `vehicle_earlymodern_coastal_sloop` | Passenger and cargo sailing craft. |
| Industrial | `vehicle_revolution_horse_tram` | Externally pulled route vehicle. |
| Industrial | `vehicle_revolution_factory_delivery_wagon` | Heavy commercial draft transport. |
| Industrial | `vehicle_revolution_canal_skiff` | Shallow-water rowed workboat. |
| Industrial | `vehicle_revolution_sailing_cutter` | Faster sailing and cargo-hatch pattern. |
| Modern | `vehicle_modern_petrol_touring_car` | Fuelled installed drive module and road movement. |
| Modern | `vehicle_modern_diesel_delivery_lorry` | Heavy powered road vehicle with cargo bay. |
| Modern | `vehicle_modern_aluminium_dinghy` | Light rowed metal craft. |
| Modern | `vehicle_modern_petrol_motor_launch` | Outboard default and emergency rowing. |
| Nuclear | `vehicle_atomic_family_saloon` | Enclosed fuelled passenger car. |
| Nuclear | `vehicle_atomic_intercity_coach` | Powered fuelled route service. |
| Nuclear | `vehicle_atomic_fiberglass_runabout` | Recreational outboard craft. |
| Nuclear | `vehicle_atomic_cabin_cruiser` | Multi-compartment motor craft. |
| Information Age | `vehicle_computer_electric_city_car` | Battery-powered installed drive module. |
| Information Age | `vehicle_computer_autonomous_shuttle` | Automatic powered route service. |
| Information Age | `vehicle_computer_recreational_kayak` | Modern self-powered item-scale craft. |
| Information Age | `vehicle_computer_rescue_rib` | High-capacity outboard rescue craft. |

### Shared support equipment

The pass also seeds reusable operating examples:

- pre-industrial wooden oar;
- modern laminated oar;
- petrol outboard motor;
- ordinary draft harness and traces;
- heavy team harness, yoke and traces;
- ordinary rigid tow bar;
- heavy articulated tow bar;
- petrol terrestrial drive module;
- diesel terrestrial drive module; and
- battery-powered electric drive module.

These are dependencies and templates. The seeder does not automatically install them in, attach them to, fuel or charge a vehicle.

## Authoring Checklist

### Identity and catalogue scope

- [ ] Root reference is globally unique, lowercase snake case and begins `vehicle_<era>_`.
- [ ] Era key agrees with the root reference and intended era tag.
- [ ] Child keys are unique, permanent lowercase identifiers.
- [ ] The variant represents a meaningful form or use difference rather than quota-filling duplication.

### Player-facing quality

- [ ] Noun, short description and full description follow the item-authoring guidance.
- [ ] Full description is individually written and visually grounded.
- [ ] No hidden component, database or seeder terminology appears to players.
- [ ] Empty weight, size, quality, primary material and cost are plausible.
- [ ] Every material, liquid and ordinary component exists under its exact seeded name.

### Structure and service

- [ ] At least one compartment, driver slot, primary station, default movement and damage zone exist.
- [ ] Every child reference resolves.
- [ ] Display orders are unique within each family.
- [ ] Room-scale compartments have valid terrain ids.
- [ ] Passenger and cargo service flags agree with actual slots and cargo spaces.

### Movement and resources

- [ ] Exactly one movement profile is default.
- [ ] Route speed is positive and calibrated to intended topology.
- [ ] Powered movement consumes declared fuel or power.
- [ ] Every required installed role has a compatible mount and seeded module.
- [ ] Automatic operation appears only on a powered route profile.
- [ ] Surface-water movement has explicit propulsion and exactly one default.
- [ ] Rowed modes have contributor slots and credible oar supply.
- [ ] Propulsion expressions use runtime-supported variables and remain finite.

### Projections, towing and damage

- [ ] Access and cargo projections are hidden, non-portable and non-skinnable.
- [ ] Every cargo projection has one suitable ordinary container component.
- [ ] Closure requirements agree with visible design and safe operation.
- [ ] Tow type, direction, rating and available hitch gear agree.
- [ ] Damage thresholds satisfy `0 < disabled < destroyed <= 1`.
- [ ] Every damage effect targets an existing child and models a real failure.

### Verification

- [ ] Run `ValidateVehicleExamplesForTesting()` and the database-seeder unit tests.
- [ ] Seed each affected era into a fresh database.
- [ ] Rerun the same era selection and confirm no duplicate root or child rows.
- [ ] Create each changed vehicle and inspect `vehiclestatus`.
- [ ] Test boarding, control, access, cargo, installation, fuel/power readiness, movement, propulsion selection, towing and damage as applicable.
- [ ] Reload or restart after testing any new topology or generated component pattern.

## Current Boundaries and Future Extension

The first pass intentionally does not provide:

- automatic installation, hitching, staffing, fuelling or charging;
- automatic fallback between water propulsion modes;
- installation-specific room-scale terrain and interior-cell catalogues;
- free-coordinate 2D/3D movement;
- collision, signalling, dispatch or timetable systems; or
- aircraft-, submarine- or rail-consist-specific movement strategies that do not yet exist in the runtime.

Add a new archetype helper when several future vehicles share a genuinely new topology, such as bicycles, coupled rail vehicles, room-scale ships or aircraft. Add a one-off explicit specification when a vehicle is exceptional. In both cases retain the validation boundary, add tests, update this document, seed all new support components through rerunnable helpers, and avoid claiming runtime behaviour the engine does not yet implement.
