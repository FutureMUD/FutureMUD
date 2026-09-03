# FutureMUD Vehicle Item Seeder Design Reference

## Purpose

This document is the implementation and authoring contract for the vehicle subcomponent of `ItemSeeder`. It is intended for agents and builders who add vehicle catalogues, review vehicle data, or extend the helper layer.

The first implementation provides:

1. rerunnable insertion of FutureMUD's canonical vehicle prototype graph;
2. safe creation of the exterior, access, cargo and equipment item prototypes associated with that graph;
3. intent-level helpers for the principal vehicle patterns currently supported by the engine;
4. validation of structural references, operational resources and plausible value ranges; and
5. a 57-vehicle catalogue, including 41 distinct vehicles admitted to one or more of Antiquity, Medieval,
   Renaissance and Early Modern.

Read this document together with:

- `Design Documents/Vehicle_System.md` for the runtime vehicle system;
- `Item_Authoring_Guidelines.md` for player-facing item descriptions;
- `FutureMUD_Item_Seeder_Working_Guidelines.md` for catalogue and rerun conventions;
- `Seeded_Item_Components.json` for ordinary reusable item components;
- `Seeded_Materials.json` and `Seeded_Liquids.json` for exact seeded resource names; and
- `FutureMUDLibrary/Vehicles/VehicleEnums.cs` for persisted enum meanings.

The catalogue is broad but still intended as reusable engine stock rather than an exhaustive record of every regional
vessel or carriage. Add culturally specific variants where form, capacity, access, propulsion, operation or use
meaningfully changes; do not create nominal variants merely to satisfy a per-culture quota.

## Runtime Architecture

A FutureMUD vehicle is a hybrid canonical-domain object with ordinary item projections. It is not just a large item carrying one XML component.

### Canonical vehicle graph

`VehicleProto` is the revisioned root. Its child records describe actual vehicle behaviour:

| Record | Responsibility |
|---|---|
| `VehicleCompartmentProto` | Logical occupied areas; for room-scale vehicles these correspond to persistent interior cells. |
| `VehicleCompartmentLinkProto` | Directed movement links between compartments; these become navigable interior links only for `RoomScale` vehicles. |
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

For `ItemScale` and `RoomContainer`, compartments organise occupancy, access and cargo but do not create separately navigable interior cells. Compartment links are retained as prototype topology but are not built as runtime exits. Do not model an internal corridor, stair or doorway with compartment links unless the vehicle is `RoomScale`.

Surface-water travel does not imply `RoomScale`. Item-scale and room-container craft use `CellExit` movement with `SurfaceWater` as their movement environment.

## Seeder API and Data Contract

### Dispatch and era selection

The public `ItemSeeder.SeedData` path used directly and through `IDatabaseSeeder` runs the established item and craft
passes first, then calls:

```csharp
SeedVehicleItemsAndPrototypes(eras);
```

This ordering is deliberate. Vehicle definitions resolve materials, liquids, traits, ordinary components, tags and previously seeded item prototypes from the caches populated by the ordinary item pass.

ItemSeeder presents readable canonical later-era names and maps them to the established vehicle tokens. The accepted tokens and tags are:

| ItemSeeder key | Compatibility alias / vehicle key | Era tag |
|---|---|---|
| `antiquity` | `antiquity` | `Era / Antiquity Era` |
| `medieval` | `medieval` | `Era / Medieval Era` |
| `renaissance` | `renaissance` | `Era / Renaissance Era` |
| `earlymodern` | `earlymodern` | `Era / Early Modern Era` |
| `industrial` | `revolution` | `Era / Industrial Era` |
| `modern` | `modern` | `Era / Modern Era` |
| `nuclear` | `atomic` | `Era / Nuclear Era` |
| `information` | `computer` | `Era / Information Age Era` |

Both forms are normalised before vehicle admission. Industrial, Modern, Nuclear and Information remain inactive at the ItemSeeder selection surface until their ordinary-item modules contain substantive content.

Only selected eras are inserted. A vehicle may declare multiple supported eras; selecting any one of them installs the
same stable prototype and applies every era tag for which that design is suitable. This prevents duplicate near-identical
farm wagons, punts, barges, canoes and dhows. Shared operating equipment is inserted when the selected technology family
requires it.

### Stable references

Root references must match:

```text
vehicle_<era>_<specific_name>
vehicle_preindustrial_<shared_name>
```

They are lowercase snake case and globally unique. Use the `preindustrial` family token only when one authored prototype
is deliberately admitted to multiple pre-industrial eras. Examples include `vehicle_preindustrial_farm_wain`,
`vehicle_medieval_trading_cog` and `vehicle_computer_electric_city_car`.

Generated item references append a stable role suffix:

- exterior: `<root>_exterior`
- access: `<root>_access_<child_key>`
- cargo: `<root>_cargo_<child_key>`

Child keys are lowercase snake case and unique within their child family. Once a catalogue has been published, treat root references, child keys and child names as durable identities. A cosmetic rename of an existing key or persisted child name can create a second child rather than updating the original.

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

Every `CellExit` movement profile requires at least one explicit propulsion row and exactly one default propulsion row.
The environment and propulsion must agree: surface water accepts paddled, rowed, sail or outboard modes; unrestricted
terrestrial movement accepts engine, externally pulled, rider-powered or explicit `None`.

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
| Minimum engine power | mechanical watts; positive for engine movement | Calibrate against empty mass, intended grade and performance. Installed compatible engines are aggregated. |
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

Carts, wagons, carriages, sledges, chariots and drays use unrestricted `CellExit` movement with an explicit
`ExternallyPulled` propulsion profile plus tow points. A forward point is normally towable by draft gear; a rear point
may allow another load to be coupled behind.

The seeded harness items demonstrate ordinary and heavy-team ratings. A vehicle is not automatically harnessed, populated with draft animals or made ready to move.

The runtime now validates the incoming character or mount hitch graph, motive authority, connector gear, proximity,
pulling capacity and recursive vehicle train before departure. The seeder does not create or attach animals automatically;
builders must still provide suitable creatures, harness and live hitching.

### Powered terrestrial cell-exit vehicles

A powered road vehicle uses:

- a terrestrial `CellExit` movement profile;
- a required installed role such as `propulsion`;
- a compatible installation point such as `land_engine` or `electric_drive`; and
- a positive minimum mechanical engine-power requirement; and
- a separately seeded removable drive module implementing `IVehicleEngine`.

Fuelled modules combine `Vehicle Installable`, `Combustion Engine` and a fuel liquid container. Electric modules combine
`Vehicle Installable`, `Electric Engine` and the appropriate battery/power component. Their engine form factor exactly
matches the installation mount type. The seeder does not install, fill, switch on or charge a module automatically.

**V1 terrain and timing limitation:** ordinary terrestrial `CellExit` profiles use the runtime's `Unrestricted` movement environment. They do not enforce road terrain, wheel clearance, gradient, axle load, traction or other land-suitability rules, and they do not store a physical land speed. Movement timing and fuel consumption are coarse per-exit values and must be calibrated against the world's cell scale. A world that requires enforced roads or terrain suitability should use `Route` topology where appropriate or extend the runtime contract; descriptions and builder notes must not imply enforcement that does not exist.

### Route vehicles

`Route` movement is for vehicles constrained to `RouteCell` topology.

- `ExternallyPulled` route profiles require a valid motive-character hitch arrangement and pull capacity.
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

In source definitions, damage effects target stable child keys. During persistence those keys are resolved to the corresponding scoped database ids. Effects may disable:

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
- Named child rows are conservatively updated in place within their vehicle and revision.
- Existing links are resolved by their scoped endpoints and direction.
- Propulsion rows are resolved by movement profile and propulsion type.
- Damage effects are resolved by zone, target type and target id.
- Manually allocated child ids are cached per table for the run, and each vehicle graph is flushed once after its
  dependencies and projections have been assembled. Catalogue growth therefore does not perform a full-table maximum-id
  scan and several graph flushes for every child row.

The database schema does not store every source-level child key. For non-projected children, a published name or scoped relationship therefore participates in persistence identity. Renaming such a child may add a replacement while leaving the previous row intact. This is safer than deleting a builder-extended row, but it makes names part of the migration contract.

The seeder updates rows it can identify but does **not** delete omitted child rows. This is intentional: a builder may have extended a seeded vehicle after installation. Removing or structurally renaming a published child requires an explicit migration and a review of live instances.

Practical rules:

1. never recycle a root stable reference for a different vehicle;
2. never casually rename a published child key or persisted child name;
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
| Cell-exit movement has no explicit propulsion | Validation fails. |
| Rowed mode has no contributor slot | Validation fails. |
| Outboard mode has no compatible mount and role | Validation fails. |
| Projection is visible, portable or skinnable | Validation fails. |
| Installed module is absent, damaged, empty, unpowered or switched off | Live movement preflight fails and reports readiness reasons. |
| Required access point or tow link is open or disabled | Departure fails when the movement profile requires valid closures or links. |
| Selected water propulsion becomes unavailable | Movement fails; no automatic fallback is chosen. |
| Surface-water craft is on dry ground or the wrong ground layer | It can exist or be carried but cannot initiate surface-water travel. |
| Route vehicle is outside valid route topology | Route operation cannot proceed. |
| Cell-exit draft vehicle has no valid motive puller | Movement preflight fails with hitch, authority, connector or capacity diagnostics. |
| Cell-exit terrestrial vehicle is off-road or on unsuitable terrain | The V1 `Unrestricted` environment does not enforce road or terrain suitability. Use world controls, `Route` topology or a runtime extension where this matters. |
| Non-room-scale compartment links are expected to create interior exits | They do not. Only `RoomScale` vehicles build navigable compartment links; use logical compartments or adopt `RoomScale`. |
| A seeded child is removed from source | Existing database row remains. Use a deliberate migration if removal is required. |
| Free-coordinate navigation, collision, signalling or dispatch is expected | Outside this first-pass seeder and current V1 movement boundary. Do not imply unsupported behaviour in descriptions. |

## Demonstration Catalogue

The catalogue contains 93 unique prototypes. Shared-era admission produces the following effective coverage:

| Era selection | Available prototypes | Representative breadth |
|---|---:|---|
| Antiquity | 16 | handcart, ox wagon, war and racing chariots, farm wain, sledge, coracle, canoe, punt, skiff, ferries and barges, river galley, trireme, dhow and coastal trader |
| Medieval | 15 | market and covered carts, farm and timber wagons, sledge, rowboat, wherry, longship, cog, dhow, punts, ferries and cargo barges |
| Renaissance | 18 | carriage, artillery wagon and limber, timber and farm wagons, launch, pinnace, caravel, carrack, galleon, dhow, wherry and inland working craft |
| Early Modern | 24 | stagecoach, hackney coach, post chaise, dray, wagons and limber, whaleboat, sloop, schooner, packet, frigate, ship of the line, earlier ocean traders and inland craft |
| Industrial | 40 | horse tram, independent route-bound rail cars and freight wagons, urban and commercial draft vehicles, canal and river craft, and coastal sailing vessels |
| Modern | 4 | petrol touring car, diesel lorry, aluminium dinghy and motor launch |
| Nuclear | 4 | family saloon, intercity coach, runabout and cabin cruiser |
| Information Age | 4 | electric city car, autonomous shuttle, kayak and rescue RIB |

The 41 pre-industrial prototypes are intentionally not duplicated per era. Stable references beginning
`vehicle_preindustrial_` identify shared farm, winter and inland-water patterns. An era-specific reference identifies the
first or defining era, while `SupportedEraKeys` admits the unchanged design to later eras. Ocean-going types range from
small coastal boats and dhows through longships, cogs, caravels, carracks, galleons, packets and schooners to frigates and
ships of the line. Large stock vessels remain `RoomContainer`: `RoomScale` interiors require installation-specific terrain
and world topology and should be authored as a deliberate follow-up.

### Shared support equipment

The pass also seeds reusable operating examples:

- pre-industrial wooden oar;
- modern laminated oar;
- petrol outboard motor;
- ordinary draft harness and traces;
- heavy team harness, yoke and traces;
- ordinary rigid tow bar;
- heavy articulated tow bar;
- petrol terrestrial drive module with a 90 kW combustion engine;
- diesel terrestrial drive module with a 400 kW combustion engine; and
- battery-powered electric drive module with a 180 kW traction engine.

These are dependencies and templates. The seeder does not automatically install them in, attach them to, fuel or charge a vehicle.

## Authoring Checklist

### Identity and catalogue scope

- [ ] Root reference is globally unique, lowercase snake case and begins `vehicle_<era>_` or the deliberate shared-family form `vehicle_preindustrial_`.
- [ ] Primary and additional era keys agree with the intended era tags.
- [ ] Child keys are unique, permanent lowercase identifiers.
- [ ] Published child names are treated as durable persistence identities where the schema has no key column.
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
- [ ] Non-room-scale compartments are used as logical groupings only and do not imply navigable interiors.
- [ ] Passenger and cargo service flags agree with actual slots and cargo spaces.

### Movement and resources

- [ ] Exactly one movement profile is default.
- [ ] Route speed is positive and calibrated to intended topology.
- [ ] Powered movement consumes declared fuel or power.
- [ ] Every required installed role has a compatible mount and seeded module.
- [ ] Automatic operation appears only on a powered route profile.
- [ ] Every cell-exit movement has environment-compatible explicit propulsion and exactly one default.
- [ ] Rowed modes have contributor slots and credible oar supply.
- [ ] Propulsion expressions use runtime-supported variables and remain finite.
- [ ] A cell-exit draft vehicle's puller capacity, hitch gear, authority and recursive train workflow are explicitly reviewed.
- [ ] Terrestrial cell-exit timing, fuel use and suitability assumptions are reviewed against its coarse `Unrestricted` per-exit semantics.

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
- [ ] For cell-exit draft examples, test ordinary hitching/dragging separately from direct controlled movement and record the runtime boundary.
- [ ] Test terrestrial cell-exit examples on representative road and non-road cells and record any world-level restrictions used to compensate for the `Unrestricted` runtime environment.
- [ ] Reload or restart after testing any new topology or generated component pattern.

## Current Boundaries and Future Extension

The first pass intentionally does not provide:

- automatic installation, hitching, staffing, fuelling or charging;
- automatic fallback between water propulsion modes;
- dedicated externally-pulled enforcement for ordinary `CellExit` vehicle movement;
- road, terrain, gradient, traction or physical-speed enforcement for terrestrial `CellExit` movement;
- navigable compartment links outside `RoomScale` vehicles;
- installation-specific room-scale terrain and interior-cell catalogues;
- free-coordinate 2D/3D movement;
- collision, signalling, dispatch or timetable systems; or
- aircraft-, submarine- or rail-consist-specific movement strategies that do not yet exist in the runtime. Industrial rail rows are independent route-bound objects; they do not claim coupling, dispatch, signalling or steam-drive behaviour.

Add a new archetype helper when several future vehicles share a genuinely new topology, such as bicycles, coupled rail vehicles, room-scale ships or aircraft. Add a one-off explicit specification when a vehicle is exceptional. In both cases retain the validation boundary, add tests, update this document, seed all new support components through rerunnable helpers, and avoid claiming runtime behaviour the engine does not yet implement.
