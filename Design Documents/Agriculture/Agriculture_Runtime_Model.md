# Agriculture Runtime Model

## Persistence
The agriculture tables live in `MudsharpDatabaseLibrary`:

- `AgricultureFieldProfiles`
- `AgricultureCropDefinitions`
- `AgricultureHerdDefinitions`
- `AgricultureWoodlandDefinitions`
- `AgricultureOperations`
- `AgricultureFields`
- `AgricultureFieldCrops`
- `AgricultureFieldHerds`
- `AgricultureFieldWoodlands`
- `AgricultureProjectContexts`

`AgricultureFields.CellId` is unique, enforcing one field per cell. `Terrains.DefaultAgricultureFieldProfileId` stores the terrain-level default profile. `AgricultureProjectContexts.ActiveProjectId` links a live local project to its dynamic field operation context.

## Field Scores
Fields store these 0-100 scores:

- moisture
- drainage
- nutrients
- salinity
- topsoil
- tilth
- rockiness
- weeds
- pests
- fence integrity
- pasture biomass
- general condition

Player-facing output shows qualitative bands. Administrators see exact values through `field look`.

## Daily Tick
Agriculture schedules a main agriculture tick through `FuturemudLoaders`. Each field's `DailyTick()` samples the current cell's weather-adjacent state:

- current season
- current temperature
- current weather
- highest recent precipitation level

The tick applies coarse pressure:

- moisture rises with precipitation and falls with heat and drainage.
- crops gain growth days when conditions are reasonable.
- crop stress reduces health and yield potential.
- unmanaged weeds and pests increase slowly.
- herds consume pasture biomass and can damage fences, topsoil, compaction, and condition when overstocked.
- managed woodlands gain growth and yield potential slowly, with pest and condition pressure.

The point is to create agricultural consequences and timing, not a detailed soil simulator.

## Crop State
A crop field can hold one `AgricultureFieldCrop` row:

- crop definition
- growth stage
- growth days
- health
- yield potential
- planted MUD datetime text

Crop stages are `Planted`, `Germinating`, `Growing`, `Setting`, `Harvestable`, `Overripe`, and `Failed`. Sowing creates crop state, ticks advance it, and harvest operations clear it back to fallow.

## Herd State
Pasture fields can hold one or more `AgricultureFieldHerd` rows. Herds are abstract by default:

- herd definition
- head count
- condition

Animal-unit and daily-graze settings translate head count into pressure. Moderate grazing can be beneficial through nutrient cycling, while overstocking damages pasture biomass, topsoil, tilth, condition, and fencing.

When builders attach an NPC template to a herd definition, `field herd draw` can materialise live NPC livestock and decrement the abstract count. `field herd absorb` validates an eligible NPC, removes it from the active world, and increments the abstract herd.

## Woodland State
Woodland fields hold one `AgricultureFieldWoodland` row:

- woodland definition
- growth days
- health
- yield potential
- planted MUD datetime text

The same field model supports coppice, pollards, timber stands, orchards/groves, and clearing. Operations decide whether a field enters woodland use, improves it, harvests from it, or returns it to fallow.

## Operations and Projects
Agriculture operations are definition rows backed by normal project templates. Starting an operation creates an `ActiveLocalProject` and writes an `AgricultureProjectContext` row with:

- active project id
- field id
- operation id
- target type and target id/text
- initiating actor id

`AgricultureOperationAction` is the completion action for those projects. On phase completion it reloads the context, revalidates the field and operation, applies score and state changes, emits local feedback, runs any completion FutureProg, and removes the context.

The completion FutureProg receives the field and the best available character context, but the initiating character is not a delivery target. If the prog returns an item or collection of items, those products are inserted into the field's cell at ground level when the operation completes.

This keeps project labour/material requirements generic while preserving dynamic agriculture state outside the project template.

## Permission Model
When a field's cell belongs to a property, field work is allowed for administrators, authorised owners, and authorised leaseholders. Fields outside property are currently open to ordinary field commands unless the operation or project template applies its own FutureProg gates.

## FutureProg Surface
Agriculture registers the `field` FutureProg variable type and exposes `location.field`. Built-in helpers include:

- `FieldAt`
- `CreateField`
- `DeleteField`
- `StartFieldProject`
- `SetFieldScore`
- `AdjustFieldScore`
- `DrawFieldHerd`
- `AbsorbNpcIntoFieldHerd`

These are intended for land expansion, scripted estates, controlled wilderness development, seasonal events, and builder automation.
