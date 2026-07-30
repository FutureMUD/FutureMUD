# Room Layer System Primer

## Purpose and audience

This primer explains FutureMUD's room-layer system for builders, developers, and AI agents. It covers the runtime model, terrain options, exits, movement, perception, water, weather, falling, persistence, FutureProg integration, and practical building patterns.

The most important idea is:

> A cell is one horizontal location. A room layer is a vertical or environmental stratum inside that cell.

Layers let one cell represent a street and its roofs, a forest floor and canopy, a water surface and its depths, or a cliff face and open air without creating a separate cell for every vertical step. Separate cells are still useful when a location needs its own name, description, access rules, ownership, overlay history, or independently authored exits.

## The mental model

Every character and loose game item in a cell has a `RoomLayer`. The cell's effective terrain supplies the set of layers that are valid there.

Three concepts work together:

1. **Cell:** the horizontal location and main unit of room building.
2. **Terrain:** the environmental definition that supplies available room layers and other defaults.
3. **Room layer:** the character's or item's vertical/environmental position within the cell.

Room layers are not independent rooms:

- They share the same cell identity, room name, description, overlays, areas, zone, and most cell effects.
- They can contain different characters and items.
- Ordinary local output is commonly sent only to the relevant layer.
- Scanning, sound, ranged combat, falling, climbing, swimming, and flying can cross layers according to their own rules.
- Exits can appear in one layer, several layers, or no layers.

Do not confuse room layers with:

- **Cell overlays:** versioned builder revisions of a cell.
- **Planes:** supernatural or corporeality partitions.
- **RouteCell coordinates:** exact longitudinal positions inside a large linear cell.
- **Position states:** standing, sitting, climbing, swimming, flying, and similar physical states.
- **Outdoors type:** weather, light, and climate exposure. It is related to terrain but is not a layer.

## The layer catalogue

`RoomLayer` is a persisted enum. Its names and numeric values are compatibility contracts because characters, items, projects, and other records may store them.

| Layer | Relative height | Meaning | Common occupants |
| --- | ---: | --- | --- |
| `VeryDeepUnderwater` | -3 | The deepest supported water stratum | Deep-diving swimmers, submerged items |
| `DeepUnderwater` | -2 | A deeper submerged stratum | Swimmers, sinking objects |
| `Underwater` | -1 | The first layer below the surface | Swimmers, aquatic life |
| `GroundLevel` | 0 | The ordinary floor, land surface, or water surface | Walkers, boats, floating swimmers |
| `InTrees` | 1 | A climbable lower canopy | Climbers, arboreal creatures |
| `OnRooftops` | 1 | A climbable rooftop surface | Walkers on roofs, climbers |
| `HighInTrees` | 2 | A higher canopy | Climbers, arboreal creatures |
| `InAir` | 3 | Lower open air | Flying characters and airborne items |
| `HighInAir` | 4 | Higher open air | High flyers and falling objects |

`InTrees` and `OnRooftops` are parallel surface types at approximately the same height. They are not interchangeable, and neither is automatically considered higher than the other. `HighInTrees` is above both. Air layers are above ground, tree, and rooftop layers.

Code should use the supplied helpers rather than comparing enum integers:

- `IsHigherThan(...)`
- `IsLowerThan(...)`
- `LayerHeight()`
- `HighestLayer()`
- `LowestLayer()`
- `ClosestLayer(...)`
- `IntermediateLayers(...)`
- `IsUnderwater()`

The enum declaration order is not the vertical order.

## Terrain determines the available layers

The terrain's persisted `TerrainBehaviourMode` is parsed when the terrain loads. Builders set it with:

```text
terrain edit <terrain>
terrain set model <model>
```

Assign the finished terrain to a cell with:

```text
cell set terrain <terrain>
```

Terrain model names are case-insensitive. Water models require a liquid id or name when set through the builder.

### Land, tree, roof, cave, and air models

| Model | Layers | Intended use |
| --- | --- | --- |
| `indoors` | `GroundLevel` | Ordinary enclosed room |
| `outdoors` | `GroundLevel`, `InAir`, `HighInAir` | Open land or street |
| `cave` | `GroundLevel`, `InAir` | Tall enclosed/cave space with limited air |
| `cliff` | `InAir`, `HighInAir` | Open rock face or air-only cell |
| `trees` | `GroundLevel`, `InTrees`, `InAir`, `HighInAir` | Forest with a lower canopy |
| `talltrees` | `GroundLevel`, `InTrees`, `HighInTrees`, `InAir`, `HighInAir` | Forest with a tall canopy |
| `cavetrees` | `GroundLevel`, `InTrees` | Enclosed or underground climbable growth |
| `rooftops` | `GroundLevel`, `OnRooftops`, `InAir`, `HighInAir` | A street/courtyard and its accessible roofline |
| `rooftopsonly` | `OnRooftops`, `InAir`, `HighInAir` | A roofscape cell whose lowest supported surface is the roof |

The loader and builder also accept `rooftop` as an alias for `rooftops`, and `rooftoponly`, `rooftop-only`, and `rooftops-only` as aliases for `rooftopsonly`. Saving normalises these to `rooftops` or `rooftopsonly`.

The distinction between the two rooftop models is important:

- Use `rooftops` when the cell represents the street or ground below and the roofs above it.
- Use `rooftopsonly` when the cell represents only a roofscape or elevated deck. A character or item entering without a valid matching layer is placed on the roof because it is the lowest supported layer.

### Water models

In water terrain, `GroundLevel` represents the water surface, not a submerged floor.

| Model | Layers |
| --- | --- |
| `shallowwater <liquid>` or `water <liquid>` | `GroundLevel`, `Underwater`, `InAir`, `HighInAir` |
| `deepwater <liquid>` | `GroundLevel`, `Underwater`, `DeepUnderwater`, `InAir`, `HighInAir` |
| `verydeepwater <liquid>` | `GroundLevel`, `Underwater`, `DeepUnderwater`, `VeryDeepUnderwater`, `InAir`, `HighInAir` |
| `shallowwatertrees <liquid>` | `GroundLevel`, `Underwater`, `InTrees`, `InAir`, `HighInAir` |
| `shallowwatercave <liquid>` | `GroundLevel`, `Underwater` |
| `deepwatercave <liquid>` | `GroundLevel`, `Underwater`, `DeepUnderwater` |
| `verydeepwatercave <liquid>` | `GroundLevel`, `Underwater`, `DeepUnderwater`, `VeryDeepUnderwater` |
| `underwater <liquid>` | `Underwater` |
| `deepunderwater <liquid>` | `Underwater`, `DeepUnderwater` |
| `verydeepunderwater <liquid>` | `Underwater`, `DeepUnderwater`, `VeryDeepUnderwater` |

The liquid becomes the terrain's `WaterFluid`. It drives exposure, buoyancy, sinking, breathing context, and water-related movement.

Use the `*watercave` variants for enclosed water with no open-air layers. Use the `*underwater` variants for a cell that is entirely submerged and has no surface layer.

### Unknown and legacy model strings

An unrecognised model string currently falls back to `indoors`, which provides only `GroundLevel`. Treat spelling as data, not decoration. After editing a terrain, use `terrain show` and verify both its model string and the resulting layers.

## Terrain is not outdoors type

Terrain model answers, "Which layers exist?"

Cell outdoors type answers, "How exposed is this cell to weather, climate, and natural light?"

The cell commands are:

```text
cell set type outdoors
cell set type indoors
cell set type windows
cell set type cave
cell set type exposed
```

Typical combinations include:

- `rooftopsonly` plus `outdoors` for a normal open roof.
- `rooftopsonly` plus `exposed` for a covered elevated platform that feels the climate but blocks rain.
- `deepwatercave` plus `cave` for a dark underground lake.
- `talltrees` plus `outdoors` for a normal forest.

Set terrain first, then override outdoors type or light settings for special cases. Changing terrain can apply terrain defaults.

## How exits interact with layers

### Horizontal and non-vertical exits use layer intersection

For ordinary cardinal and non-cardinal exits, an exit appears in every layer that:

1. exists in the origin terrain,
2. exists in the destination terrain, and
3. is not in the exit's blocked-layer collection.

In set notation:

```text
visible layers = origin layers ∩ destination layers - blocked layers
```

This rule is why compatible terrain choices matter.

Examples:

- `outdoors` to `outdoors` connects at `GroundLevel`, `InAir`, and `HighInAir`.
- `rooftops` to `rooftopsonly` connects at `OnRooftops`, `InAir`, and `HighInAir`, but not at `GroundLevel`.
- `rooftopsonly` to `outdoors` connects only in the two air layers. A non-flyer on the roof cannot use that horizontal exit.
- `deepwater` to `shallowwater` connects at the surface, first underwater layer, and both air layers, but not at `DeepUnderwater`.
- `talltrees` to `trees` connects at ground, lower trees, and air, but not at `HighInTrees`.

The movement transition also checks what the mover can do. Shared air does not let a non-flyer walk through the sky, and shared underwater layers require swimming behavior.

### Up and down exits are endpoint exits

`Up` appears only at the origin cell's highest layer. `Down` appears only at its lowest layer. This is different from horizontal intersection.

For climb, fly, and fall exits:

- an upward transition starts at the origin's highest layer and targets the destination's lowest layer;
- a downward transition starts at the origin's lowest layer and targets the destination's highest layer.

This lets a ground-only street connect upward to a rooftop-only cell:

```text
Street:       indoors or other ground-only terrain
Roof:         rooftopsonly
Connection:   up/down exit marked as a climb exit
Result:       GroundLevel -> OnRooftops
```

Be careful when adding `Up` to a cell with sky layers. In an `outdoors` or `rooftops` cell, the highest layer is `HighInAir`, so the `Up` exit is an aerial endpoint rather than a ladder from the ground or roof.

The blocked-layer collection is not consulted for `Up` or `Down`; those directions always use the origin's highest or lowest endpoint. Prefer appropriate terrain topology and climb/fall flags for vertical exits.

### Blocking an exit by layer

Use:

```text
cell exit block <exit> <layer>
cell exit unblock <exit> <layer>
```

Blocking is useful for horizontal and non-cardinal exits that should exist only at selected shared layers. Examples:

- Block `GroundLevel`, `InAir`, and `HighInAir` to make a water connection usable only underwater.
- Block `InAir` and `HighInAir` so a doorway is ground-only even when both cells have outdoor air layers.
- Block `GroundLevel` so two `rooftops` cells connect across their roofs and sky but not along the street.
- Block `OnRooftops` to stop a street exit from becoming an accidental roof bridge.

Exit edits are overlay-aware. If an exit is shared with another overlay package, the builder workflow can copy it into the current package before changing it. Verify both sides and the intended overlay.

### Hidden exits

Use:

```text
cell exit hide <exit> <prog>
cell exit unhide <exit>
```

The hide prog must return boolean and accept:

```text
(location, character)
```

When the prog returns true, the exit is visually hidden from that character. The location is the cell that owns that side's hidden-exit effect.

Hidden and blocked are different:

- A blocked-layer exit does not exist in that layer.
- A hidden exit exists in the layer but is removed from normal visual perception for characters to whom the prog applies.
- Administrators bypass ordinary hidden-exit perception.

Hide state is attached to a side's cell effect, so configure and verify each direction that should be hidden.

## Movement within and between layers

There are two different kinds of vertical movement:

1. **Changing layer inside the current cell.** The cell does not change; only the character's `RoomLayer` changes.
2. **Taking an exit to another cell.** The character leaves one cell, enters another, and the exit selects the arrival layer.

The command determines which kind is attempted:

| Command | In-cell behavior | Cross-cell behavior |
| --- | --- | --- |
| `up` / `down` | Never changes layer by itself | Takes an `Up` or `Down` exit visible at the current layer |
| `climb up` / `climb down` | Steps to the nearest climb-reachable higher/lower layer | Takes a climb-marked `Up`/`Down` exit only when there is no further in-cell layer in that direction |
| `ascend` | While flying or swimming, steps to the nearest higher layer | At the relevant top boundary, may take an `Up` exit |
| `dive` / `descend` | While flying or swimming, steps to the nearest lower layer | At the relevant bottom boundary, may take a `Down` exit |
| `fly` | Changes the character to the flying position state; it does not change layer | None by itself |
| `land` | Leaves the flying position state; it does not select a lower layer | None by itself |

Layer changes are deliberately incremental. One command moves one layer step, not directly from the bottom to the top.

### Ordinary movement

Normal direction commands use an exit visible in the mover's current layer. The exit computes a transition type and destination layer. Invalid layer transitions are rejected even if the underlying cell connection exists.

For horizontal and non-cardinal exits, ordinary movement normally preserves the current layer:

```text
Origin InTrees -> Destination InTrees
Origin OnRooftops -> Destination OnRooftops
Origin HighInAir -> Destination HighInAir
```

This requires the layer to exist on both sides and not be blocked.

`up` and `down` do not mean "move to the next room layer." They look for actual cardinal exits:

- `Up` can only be addressed from the origin terrain's highest layer.
- `Down` can only be addressed from the origin terrain's lowest layer.
- A plain, non-climb/non-fall vertical exit preserves the layer and is viable only if the destination also contains that layer.
- A climb/fly/fall vertical exit maps between boundary layers: upward to the destination's lowest layer, downward to its highest layer.

Therefore a character at `GroundLevel` in an `outdoors` cell cannot type `up` to reach `InAir`; there is no in-cell implicit upward movement. They must `fly` and `ascend`. Likewise, a swimmer uses `dive`/`descend` to move below the surface rather than typing `down`, unless a real `Down` exit exists at the water column's lowest layer.

Common transition types include:

- ground to ground;
- trees or rooftop to the corresponding shared climbable layer;
- swim-only;
- swim to land;
- fly-only;
- fall;
- no viable transition.

When an exit requires climbing, swimming, or flying, ordinary directional movement uses the corresponding movement position and capability checks. Safe-movement settings can require the player to confirm entry into water with `!`.

### How a multi-layer vertical journey is resolved

Consider a `talltrees` cell:

```text
GroundLevel -> InTrees -> HighInTrees -> InAir -> HighInAir
```

A climber can use:

```text
climb up      GroundLevel -> InTrees
climb up      InTrees -> HighInTrees
```

They cannot climb from `HighInTrees` into `InAir`; that requires flight. A flyer can continue:

```text
fly           changes position state, but remains at HighInTrees
ascend        HighInTrees -> InAir
ascend        InAir -> HighInAir
```

Only at `HighInAir`, the cell's highest layer, can a visible `Up` exit be used. `ascend` will hand off to that exit automatically. If it is a special climb/fly/fall exit, the destination will be entered at its lowest layer.

The reverse journey works one nearest layer at a time with `descend` while flying. When the flyer reaches the current cell's lowest layer, a further `descend` can use a `Down` exit.

### Climbing

Player commands:

```text
climb up
climb down
climb roof
climb rooftop
climb rooftops
```

Within one cell, climbing can move through supported tree and rooftop layers. The next destination is the nearest valid higher or lower climbable layer. Characters must belong to a race that can climb, and movement checks/delays still apply.

Climbing stops at the top of the climbable structure:

- `GroundLevel -> InTrees -> HighInTrees` is climbable.
- `GroundLevel -> OnRooftops` is climbable.
- A tree or rooftop layer cannot be climbed upward into `InAir`.
- Climbing downward selects the nearest lower supported layer. Reaching `GroundLevel` restores an upright land position where possible; other layers retain the climbing posture. Climbing upward onto `OnRooftops` can also restore an upright posture.

If there is no further in-cell layer, an appropriately directed climb exit can carry the climber to another cell. Mark it with:

```text
cell exit climb <exit> <difficulty>
```

`climb roof` is a convenience for climbing up from `GroundLevel`; it does not select an arbitrary roof cell.

There is an important topology consequence: because `Up` exists at the highest terrain layer, an upward climb exit on `outdoors`, `rooftops`, `trees`, or `talltrees` is attached to `HighInAir`, not to the roof or canopy. An ordinary climber cannot reach that air layer. For a ladder or wall that should carry a climber between cells, use a source terrain whose highest layer is the intended climb surface, or represent the ladder with a separate ground-only/climb-only staging cell.

For example:

```text
Ground-only stairwell --Up (climb)--> rooftopsonly roof
GroundLevel                         -> OnRooftops
```

On the return side, `Down` is visible at the rooftop-only cell's lowest layer (`OnRooftops`), so `climb down` can take it.

### Flying

Player commands:

```text
fly
ascend
dive
descend
land
```

`fly` changes the character's position state but leaves them in the same layer. From the ground, the sequence is:

```text
fly
ascend
```

Flying characters then use `ascend` and `dive`/`descend` to move to the nearest supported higher or lower layer. At the highest or lowest layer, the same command can continue through an appropriate `Up` or `Down` exit.

Example in `rooftopsonly`:

```text
OnRooftops --ascend--> InAir --ascend--> HighInAir
HighInAir --descend--> InAir --descend--> OnRooftops
```

If an `Up` exit exists, another `ascend` from `HighInAir` attempts that exit. If a `Down` exit exists, another `descend` from `OnRooftops` attempts that exit.

`land` does not automatically choose the next solid layer. It changes position state in the current layer. Landing while still in an unsupported air layer can cause a fall; the command requires `land !` for the clearly dangerous high-air case.

Air layers are active simulation spaces:

- ordinary horizontal exits can connect them;
- non-flying characters and unsupported objects fall;
- flying consumes stamina and can end in exhaustion;
- ranged attacks and scans can involve other layers;
- `land` can be dangerous if there is no safe supporting layer.

`rooftopsonly` includes both air layers, so flyers can approach, cross, and descend onto its roof.

### Swimming and diving

Player commands:

```text
swim
ascend
dive
descend
```

In a terrain with underwater layers:

- `GroundLevel` is a swimming surface layer;
- underwater layers are both swimming and submerged;
- `IsSwimmingLayer(layer)` is true for the surface and supported submerged layers;
- `IsUnderwaterLayer(layer)` is true only below the surface.

`dive` and `descend` are aliases. A swimming character moves one layer downward per use:

```text
GroundLevel -> Underwater -> DeepUnderwater -> VeryDeepUnderwater
```

At the terrain's lowest layer, another `dive`/`descend` can use a visible `Down` exit. This is how a deep water column can continue into another submerged cell.

`ascend` moves one layer upward per use while the swimmer is underwater. In a normal water terrain it eventually reaches `GroundLevel`, the water surface. Once at the surface, `ascend` reports that there is nowhere further to rise; it does not use an `Up` exit from the surface. Use ordinary exit movement, climb, or flight for whatever comes after surfacing.

In a fully submerged `underwater`, `deepunderwater`, or `verydeepunderwater` cell, the highest supported layer is still underwater. From that highest submerged layer, `ascend` can use a visible `Up` exit to continue into another cell.

A flyer descending onto the `GroundLevel` surface of water cannot continue into the underwater layer while remaining in the flying state. They must `land`, which puts them into the swimming state there, and then use `dive`/`descend`.

Characters who are not maintaining an appropriate swimming/flying state can sink. Items are exposed to the terrain liquid and may float or sink according to buoyancy and item behavior.

### Worked cross-cell boundary examples

| Origin state | Exit | Destination terrain | Arrival |
| --- | --- | --- | --- |
| `GroundLevel` in a ground-only cell | `Up`, climb-marked | `rooftopsonly` | `OnRooftops`, using climbing movement |
| `HighInAir` in `outdoors` | `Up`, fly/fall-style | ground-only cell | Destination's lowest layer, normally `GroundLevel` |
| `OnRooftops` in `rooftopsonly` | `Down`, climb/fall-style | `outdoors` | Destination's highest layer, normally `HighInAir`; fall-exit handling then preserves a fall-safe posture or starts flight for a capable unsupported mover |
| `VeryDeepUnderwater` in `verydeepwater` | `Down`, swim transition | `deepunderwater` | Destination's highest layer, normally `Underwater` |
| `GroundLevel` in one ground-only cell | plain `Up` | another ground-only cell | `GroundLevel`; the direction changed cells, not layers |

The third example is intentionally hazardous: a special downward endpoint arrives at the destination's highest layer. If the intended result is a safe ladder from roof to street, make the exit climb-marked and test the complete movement/position outcome, or choose terrain on the lower cell whose highest layer is the intended arrival surface.

### Falling

The cell activates falling checks when it has air layers or a downward fall exit.

- `HighInAir` falls toward `InAir`.
- `InAir` falls toward the highest supported lower layer.
- Tree layers can break a fall before ground.
- Rooftops can catch a fall or lead onward to ground in the combined `rooftops` model.
- In `rooftopsonly`, the roof is the local landing surface. An explicit downward fall/climb exit can provide somewhere lower.
- A downward fall or climb exit can continue a fall into another cell.
- Entering water applies the water-fall grace and may carry the falling entity underwater.

Wind can dislodge suitable items and characters from tree or rooftop layers. Do not treat roofs and canopies as merely cosmetic.

## Placement, entry, and persistence

Characters and items persist their current `RoomLayer`.

When an entity enters a cell:

- If its current layer exists in the destination terrain, that layer is preserved.
- Otherwise the engine chooses a valid boundary layer, normally the lowest suitable layer unless the entity arrived from above the terrain's highest layer.

Consequences:

- An ordinary `GroundLevel` placement into `rooftopsonly` resolves to `OnRooftops`.
- A flyer moving horizontally between cells with shared air layers stays in the same air layer.
- A character entering an entirely submerged terrain may be placed in its shallowest underwater layer.

Changing a live cell's terrain can invalidate existing occupant layers. The engine reconciles loose items during fall-status refresh, while characters should be moved or checked deliberately by the builder. Avoid changing terrain beneath active players without a migration plan.

Room layer is also stored or projected by several systems, including active projects, character instances, vehicles, dropped items, corpses, and created/teleported entities. When writing new code, preserve layer alongside cell location unless the operation intentionally selects a new layer.

## Perception, output, and proximity

Layers partition local presentation.

- Characters and items are normally listed for the viewer's current layer.
- Layer-specific echoes are delivered to characters in that layer.
- Characters in the same cell but different layers are generally treated as very distant rather than immediately colocated.
- `quickscan`, `scan`, and `longscan` can inspect other layers as well as exits, subject to perception checks and layer visibility rules.
- Layer tags such as `[Rooftops]`, `[Air]`, or `[Underwater]` can be shown where a command presents cross-layer targets.
- Audio handling can describe sound from above or below across the cell's layers.

Visibility is intentionally not a simple absolute-height comparison. Underwater visibility has special adjacency rules, while longscan is more permissive for ordinary above-water layers.

For code that emits a local action, preserve the source layer and use layer-aware output methods. A whole-cell echo can leak information between strata that ordinary participants should not receive.

RouteCells add exact longitudinal distance inside a layer. A matching `RoomLayer` does not by itself mean two RouteCell occupants are close; RouteCell proximity still applies.

## Combat and physical interaction

Room layer affects:

- whether combatants are colocated;
- whether melee can continue;
- ranged targeting and relative above/below descriptions;
- thrown, lobbed, scattered, and dropped item destinations;
- forced movement to another layer or through an exit;
- swooping and aerial attacks;
- drowning and dropper strategies;
- cover and perception of targets.

Most ordinary melee interactions require the same cell and layer. Ranged systems can cross layers when their visibility and range rules allow it.

Forced-movement attacks can be authored for exits, layers, or both. They still validate that the target layer exists and that the movement transition is possible.

## Weather, atmosphere, and environmental systems

### Weather and exposure

Weather controller selection comes from the terrain override, area, or zone, but exposure comes from the cell's outdoors type.

- Underwater occupants do not receive ordinary weather echoes.
- Rain accumulation and item/body precipitation exposure occur only where the cell is outdoors and the layer is not underwater.
- Surface liquid state is stored per room layer, so separate solid layers can have separate wet surfaces.
- Falling governs unsupported air occupants, while wind can dislodge occupants of tree and rooftop layers.

### Atmosphere and breathing

Terrain and cell atmosphere determine the ambient fluid. Underwater checks select the terrain's water fluid for breathing and exposure. A cell can override its atmosphere independently with:

```text
cell set atmosphere gas <gas>
cell set atmosphere liquid <liquid>
cell set atmosphere none
```

When building unusual environments—flooded gas chambers, vacuum air layers, or liquid atmospheres—test breathing behavior in every supported layer rather than assuming the terrain name is sufficient.

### Gravity

Terrain also selects normal or zero gravity. Zero gravity changes falling and movement:

- ordinary falling is suppressed;
- unsupported entities can float;
- worn propulsion can thrust through layers or exits;
- tethers and drift effects can control motion.

Room layers remain the coarse vertical strata even when gravity is zero.

## FutureProg surface

Use exact enum names such as `"OnRooftops"` and `"DeepUnderwater"` in prog text.

| Function | Purpose |
| --- | --- |
| `ROOMLAYERS(location)` | Returns the current terrain's layer names |
| `ROOMLAYERS(location, overlaypackage)` | Returns layers for a specific overlay's terrain |
| `LAYERCHARACTERS(location, layertext)` | Returns characters in one layer |
| `LAYERITEMS(location, layertext)` | Returns items in one layer |
| `LAYEREXITS(location, layertext)` | Returns exits that appear in one named layer |
| `LAYEREXITS(location, character)` | Returns exits for the character's current layer |
| `LAYEREXITS(location, item)` | Returns exits for the item's current layer |
| `ISUNDERWATER(location, layertext)` | Tests whether the layer is submerged in that cell |
| `ISSWIMLAYER(location, layertext)` | Tests whether the layer requires/supports swimming |
| `SETLAYER(character, layertext)` | Directly changes a character's layer |
| `SETLAYER(item, layertext)` | Directly changes an item's layer |

Other placement and teleport functions also have layer-aware overloads.

Safety guidance:

- Use `ROOMLAYERS` before selecting a dynamic destination.
- Treat `SETLAYER` as a low-level relocation tool. It parses the enum name but does not itself perform climbing, flying, swimming, stamina, echo, or normal movement checks.
- Validate that the requested layer belongs to the destination terrain before calling low-level placement helpers.
- Use `LAYEREXITS` rather than assuming every cell exit appears in every layer.
- Hidden-exit perception is a separate concern from layer membership.

## AI and pathfinding

Layer-aware AI must account for both topology and locomotion.

- `WhichLayersExitAppears()` describes the exit's layer topology.
- `CouldTransitionToLayer(...)` describes broad racial/character capability: ground, swim, climb/roof/tree, or fly.
- Normal pathfinding may be layer-sensitive or explicitly layer-agnostic depending on the caller.
- A path through a cell is not executable merely because the cells are adjacent; the actor must reach a layer in which the next exit exists.
- Group movement must choose a layer all required movers can use.

For NPC placement:

- swimmers placed in swimming terrain may adopt swimming position;
- flyers placed above ground can adopt flying position;
- ground-only NPCs should not spawn in rooftop-only or air-only routes unless their load/spawn logic intentionally resolves them to a safe surface.

For builder-authored AI, prefer querying the live terrain and layer rather than keying behavior from terrain names.

## Builder workflows and diagnostics

### Create a terrain with a layer model

```text
terrain new "Roofscape"
terrain edit "Roofscape"
terrain set model rooftopsonly
terrain set outdoors
terrain show
terrain close
```

Then, while editing the intended overlay package:

```text
cell set terrain "Roofscape"
cell set type outdoors
cell show
cell exit list all
```

The exact commands for creating or switching overlay packages are documented in the [Room Building Builder Guide](../Building/Room_Building_Builder_Guide.md).

### Verify an exit's layers

Use builder inspection plus a character placed in each important layer.

At minimum:

1. `cell show`
2. `cell exit list all`
3. confirm both cells' terrain models;
4. confirm blocked layers;
5. test the exit at every intended shared layer;
6. test an unintended layer and confirm the exit is absent;
7. test the reverse side;
8. test with a non-flyer/non-swimmer as well as a capable character where relevant.

### Common failure modes

| Symptom | Likely cause |
| --- | --- |
| Exit exists in the database but not from the current position | The current layer is not shared, is blocked, or is the wrong up/down endpoint |
| Street exit unexpectedly appears in the sky | Both terrains have air layers and those layers were not blocked |
| Roof character cannot cross to the next cell | The destination lacks `OnRooftops`, or that layer is blocked |
| Ground character cannot take an up ladder | The origin has higher sky/canopy layers, so `Up` is attached to its highest layer |
| Character can see a secret exit underwater | The hide prog returns false for that character or only the reverse side was hidden |
| Character enters a room at an unexpected level | Their old layer did not exist and entry reconciliation selected a boundary layer |
| Non-flyer falls after an admin layer change | They were placed into an air layer without an appropriate position/capability |
| Water behavior occurs at `GroundLevel` | In water terrain, ground level is the water surface and is a swimming layer |
| Room is rainy despite a cave-like model | Outdoors type, not the layer model, controls exposure |

## Imaginative building patterns

### 1. A street with accessible roofs in the same cell

Use `rooftops` for the street cell. Characters begin at `GroundLevel` and can `climb roof` to `OnRooftops`.

Use the same terrain on adjoining street cells when both their streets and roofs should connect. Block layers selectively:

- leave `GroundLevel` and `OnRooftops` open for parallel street and roof travel;
- block `OnRooftops` where a roofline is broken;
- block `GroundLevel` where a wall or courtyard gap prevents street travel but roofs join;
- block both air layers on ordinary doorways if flyers should not treat them as aerial corridors.

This pattern is compact and makes the roof chase occupy the same geographic cells as the street chase.

### 2. A large rooftop district above smaller street cells

Use `rooftopsonly` for dedicated roofscape cells and `rooftops` for street cells that have a climbable roof.

A horizontal exit between those cells naturally exists at `OnRooftops` and both air layers, but not at ground. A character climbs within the street cell, then crosses into the dedicated roofscape.

This is useful when roof geography is coarser or different from street geography: one broad roof cell can connect to several street cells without exposing a ground-level shortcut.

### 3. A separate roof room reached by a ladder

If the roof needs its own description, ownership, or door topology:

1. Use a ground-only terrain for the interior/street cell.
2. Use `rooftopsonly` for the roof cell.
3. Connect them with `Up`/`Down`.
4. Mark the exit as a climb exit with an appropriate difficulty.

Because the origin's highest layer is ground and the roof's lowest layer is rooftop, the vertical transition maps cleanly.

Avoid adding the ladder as `Up` from an `outdoors` street unless you intend it to start in `HighInAir`.

### 4. A hidden underwater tunnel

Use compatible water terrain on both sides. On the tunnel exit:

1. Block `GroundLevel`, `InAir`, and `HighInAir`.
2. Leave only the intended underwater depth unblocked.
3. Add a hide prog that returns true until the character has the required knowledge, perception result, equipment, or quest state.
4. Configure the reverse side separately if it should also be hidden.

A surface swimmer will not see or use the tunnel because the exit does not exist at the surface. A diver at the correct depth can discover it if the hide prog permits.

For a tunnel mouth that exists only at `DeepUnderwater`, use deep water on both sides and block `Underwater` as well.

### 5. A flooded cave with a concealed air pocket

Build the main cave with `deepwatercave`. Add a non-cardinal exit from its surface layer to a ground-only or cave cell representing the air pocket.

Block the exit's underwater layers so it is reachable only at the water surface. Hide it behind a character-sensitive prog. The result is a refuge that divers can miss unless they surface and search.

### 6. A cliff with climb and flight routes

Use a ground/cave cell for the base, `cliff` cells for open rock-face stages, and a ground or rooftop-only cell for the ledge/top.

Connect stages vertically with climb exits. Add downward fall exits where losing grip should propagate into the cell below. Flyers can use fly transitions; climbers face authored difficulties; falling entities accumulate distance across the chain.

If the cliff is represented inside one `talltrees`-like multi-layer cell, climbing is easier to author but every layer shares one room description. Separate cells are better when ledges have distinct content.

### 7. An aerial highway

Give neighboring outdoor cells shared `InAir` and `HighInAir` exits while blocking ground or rooftop layers on selected connections. Flying creatures can follow wind lanes or sky bridges that ground travelers cannot use.

Use `HighInAir` for long, exposed routes and `InAir` for low approaches. Add terrain or weather hazards, ranged threats, and safe rooftop-only landing nodes.

Remember that ordinary cardinal exits already appear in shared air layers. Blocking is often what defines the highway rather than adding extra exits.

### 8. A canopy civilization

Use `talltrees` across the settlement.

- `GroundLevel` is the forest floor.
- `InTrees` is the lower branch network.
- `HighInTrees` is the high settlement.
- Air layers support flyers above the canopy.

Block ground on branch bridges and block tree layers on animal trails. Use climb exits or in-cell climbing for trunks. A hidden non-cardinal exit at `HighInTrees` can represent a camouflaged platform entrance.

### 9. A harbour with surface, underwater, and aerial routes

Use `deepwater` or `verydeepwater`.

- Boats operate at `GroundLevel`, the water surface.
- Divers use underwater exits to wrecks, intake tunnels, or reefs.
- Flyers use air-layer approaches.
- GroundLevel-only exits can represent docks or beaches.

Block layers so a dock exit is surface-only, a sewer grate is underwater-only, and a crane platform is rooftop/air-accessible in its adjacent land cell.

Do not describe the surface layer as the seabed just because its enum name is `GroundLevel`.

### 10. A roof edge with a real fall

Use `rooftopsonly` for the roof. Add an explicit downward fall/climb exit to the street or courtyard below. The roof remains the normal landing surface, but a fall that takes the downward endpoint can continue into the lower cell.

This pattern is better than inventing a nonexistent `GroundLevel` inside a roofscape cell. It keeps the street and roof descriptions separate and makes the vertical danger explicit.

### 11. A secret layered shortcut

Two ordinary rooms may be adjacent at ground, roof, canopy, and air simultaneously. Use blocked layers to make the obvious exit ground-only, then add a differently named non-cardinal exit available only at the elevated layer.

The same two cells now have:

- a public street door;
- a hidden roof hatch or branch crossing;
- an aerial route for flyers.

This creates spatial depth without multiplying cells.

## Choosing layers versus separate cells

Prefer layers when:

- the strata share one room identity and description;
- they are vertically aligned;
- movement between them is climbing, swimming, flying, or falling;
- exits mostly overlap and can be selected with blocking;
- you want scans and cross-layer sound to treat them as one location.

Prefer separate cells when:

- each stratum needs a distinct room name or description;
- ownership, law, atmosphere, lighting, weather exposure, or overlays differ;
- the horizontal maps have different granularity;
- a door, lock, script, or building workflow must belong to only one stratum;
- a vertical journey should accumulate distance or pass multiple authored stages.

Hybrid designs are often strongest: use layers for local vertical play and separate cells where the fiction or topology genuinely changes.

## Guidance for developers and AI agents

When changing layer-aware code:

1. Start from `RoomLayer` and `ConstructionExtensions`; do not infer height from enum integers.
2. Get the effective terrain through `cell.Terrain(voyeur)` when viewer overlays or terrain overrides matter.
3. Validate the layer against `TerrainLayers`.
4. Preserve both cell and layer when moving or creating perceivables.
5. Use `WhichLayersExitAppears()` and `MovementTransition(...)` for exit behavior.
6. Keep hidden-exit perception separate from layer topology.
7. Use layer-specific character/item collections and output helpers.
8. Consider surface water, underwater breathing, falling, zero gravity, and position state.
9. Test origin and destination, both directions, all intended layers, and at least one unintended layer.
10. Update this primer and the room-building guide when builder-visible behavior changes.

Useful implementation entry points:

- `FutureMUDLibrary/Construction/ConstructionExtensions.cs`
- `FutureMUDLibrary/Construction/ITerrain.cs`
- `FutureMUDLibrary/Construction/ICell.cs`
- `MudSharpCore/Construction/Terrain.cs`
- `MudSharpCore/Construction/Cell.cs`
- `MudSharpCore/Construction/Boundary/CellExit.cs`
- `MudSharpCore/Framework/PerceiverItem.cs`
- `MudSharpCore/Character/CharacterMovement.cs`
- `MudSharpCore/Commands/Modules/PositionModule.cs`
- `MudSharpCore/Commands/Modules/RoomBuilderModule.cs`
- `MudSharpCore/FutureProg/Functions/Location/`
