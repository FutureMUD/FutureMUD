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

The enum also reserves `Custom1` through `Custom12`. These slots are disabled by default through the `AgricultureCustomScoreTypes` static configuration. When a slot is enabled, the configuration supplies its display name and whether higher values are beneficial or harmful. Built-in scores remain stored in their existing `AgricultureFields` columns; custom live field values are stored in the field definition XML under `<CustomScores>`.

Apiary adjunct state is also stored in the field definition XML, under `<Apiary>`. This keeps hive count, colony health, stores, yield potential, pollination radius, and derived pollination strength beside the field state without adding another EF table.

Profiles, operations, and crop definitions persist score references by stable enum slot name. This means a builder can rename `Custom1` from one local term to another without rewriting existing field profile, operation, crop, or field XML.

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
- crop definitions can include additional score ranges, including custom score ranges, and out-of-range values count as stress.
- happy apiaries build stores and yield potential, and crop or orchard fields can receive capped pollination support from the best happy apiary within range.
- unmanaged weeds and pests increase slowly.
- herds consume pasture biomass and can damage fences, topsoil, compaction, and condition when overstocked.
- managed woodlands gain growth and yield potential slowly, with pest and condition pressure.

The point is to create agricultural consequences and timing, not a detailed soil simulator.

## Crop State
A crop field can hold one `AgricultureFieldCrop` row:

- crop definition
- growth stage
- growth days
- harvest count
- health
- yield potential
- planted MUD datetime text

Crop stages are `Planted`, `Germinating`, `Growing`, `Setting`, `Harvestable`, `Overripe`, and `Failed`. Sowing creates annual crop state, ticks advance it, and annual harvest operations clear it back to fallow.

Crop definitions can include planting windows, seed requirements, and one or more commodity outputs in their XML definition. Planting windows are saved as stable group or season names under `<PlantingWindows>`. No planting windows means unrestricted, which preserves older custom crops.

Sow and orchard planting operations check the crop's planting windows against the field's current local season before a project starts. The agriculture completion action revalidates through the same operation path, so a long-running project that finishes after the valid planting window has closed does not bypass the crop rule.

When a cell has a weather controller, the current season is matched by season group, season name, or display name. If the cell has no current season, group windows fall back to the `AgricultureSeasonGroupWindows` static configuration, which maps broad groups such as Winter, Spring, Summer, and Autumn to normalized celestial-year fractions. Exact season-name windows require a live local season and do not use the fallback.

Harvesting creates commodity piles in the field cell, scaled by the crop's current health and yield potential. Overripe crops can still be harvested, but their output multiplier is lower than a clean harvest.

Seed stock is represented as ordinary commodity piles with a secondary `Seeds` tag. Stock crop definitions also tag their main edible or usable yield as `Seeded Yield`, which lets the generic `select seed stock` craft convert a portion of harvested commodity into seed-tagged commodity of the same material.

Stock crop outputs use the ordinary seeded material catalogue. Where a crop name would collide with a non-food material, such as walnut wood, the agriculture output uses a distinct food material such as `walnut nut`.

Optional crop score ranges are saved under `<ScoreRanges>`. They are independent of moisture and temperature, so a non-terrestrial crop can require a configured field score such as magical saturation, radiation, nutrient medium quality, or other setting-specific resources.

Crop definitions can also save pollination metadata. `None` crops ignore apiaries, `Beneficial` and `Strong` crops can receive capped daily health and yield bonuses during `Growing` and `Setting`, and `Required` crops take a stress penalty while setting fruit if no happy apiary support is in range. The stock crop catalogue uses concrete crop rules instead of assigning pollination to whole crop families.

## Orchard State
Orchards, vineyards, nut groves, and similar perennial crops use the `Orchard` field use and the same `AgricultureFieldCrop` persistence row as annual crops. Their crop definitions set `perennial=true`, use `growthDays` for establishment time, and use `harvestCycleDays` for recurring harvest cadence after the first successful harvest.

`PlantOrchard` operations require a perennial crop target and leave the field in `Orchard` use. Harvest operations for orchard fields release commodity outputs, increment the crop harvest count, reset growth days to the next cycle, and leave the crop in place rather than clearing the field to fallow.

## Herd State
Pasture fields can hold one or more `AgricultureFieldHerd` rows. Herds are abstract by default:

- herd definition
- head count
- condition

Animal-unit and daily-graze settings translate head count into pressure. Moderate grazing can be beneficial through nutrient cycling, while overstocking damages pasture biomass, topsoil, tilth, condition, and fencing.

When builders attach an NPC template to a herd definition, `field herd draw` can materialise live NPC livestock and decrement the abstract count. `field herd absorb` validates an eligible NPC, removes it from the active world, and increments the abstract herd.

Herders can also drive abstract herds between adjacent agriculture fields with `field herd drive`. The destination must already have an agriculture field, must support pasture use, and must currently be fallow or pasture. Owned destination fields require the actor to be authorised for the property; unowned fields can receive herds, which supports wild pasture or semi-nomadic pastoral use cases.

## Woodland State
Woodland fields hold one `AgricultureFieldWoodland` row:

- woodland definition
- growth days
- health
- yield potential
- planted MUD datetime text

The same field model supports coppice, pollards, timber stands, and clearing. Orchards and vineyards are represented by the `Orchard` field use instead of woodland because they behave like perennial crop systems with harvest windows.

Woodland definitions can include commodity outputs such as poles, rods, timber, fruitwood, bamboo, and firewood. Woodland operations can define a yield multiplier and a yield cost. When such an operation completes, the field releases commodities scaled by woodland health, current yield potential, and the operation multiplier, then consumes the configured amount of woodland yield.

## Apiary State
Apiaries are field adjuncts rather than a primary `AgricultureFieldUse`. A field can remain fallow, crop, orchard, pasture, or woodland while also carrying hive state:

- hive count
- colony health
- stores
- yield potential
- pollination radius
- pollination strength

A happy apiary requires colony health of at least 50, stores of at least 25, field condition of at least 40, pests no higher than 70, and an acceptable field temperature. Happy apiaries supply pollination support out to their configured radius, defaulting to two cell hops in seeded operations.

Install, tend, harvest, and remove apiary operations use `AllowedUses` so they can run across suitable primary uses without changing the field's current use. Harvesting can release raw honeycomb, pressed honey, and rendered beeswax commodity piles, scaled by colony state and the operation's harvest settings.

## Operations and Projects
Agriculture operations are definition rows backed by normal project templates. Starting an operation creates an `ActiveLocalProject` and writes an `AgricultureProjectContext` row with:

- active project id
- field id
- operation id
- target type and target id/text
- initiating actor id

`AgricultureOperationAction` is the completion action for those projects. On phase completion it reloads the context, revalidates the field and operation, applies score and state changes, emits local feedback, runs any completion FutureProg, and removes the context.

During each project tick, agriculture projects record a Farming-weighted labour summary into the context XML. The summary stores total contributing hours, weighted worker skill, supervision hours, and weighted supervisor skill. Completion converts that summary into a capped work outcome:

- beneficial score deltas are strengthened or weakened
- harmful score deltas are dampened or amplified
- sowing and orchard planting adjust initial crop health and yield potential
- crop, orchard, and woodland outputs adjust commodity weight
- seed-tagged outputs receive their own seed recovery multiplier
- released commodities receive an output quality based on the Farming outcome
- apiary operations can install hives, improve or weaken colony state, harvest commodity outputs, or remove hive state

This means hired labour still supplies scale, but skilled farmers and supervisors influence the result that comes out of the project.

The completion FutureProg receives the field and the best available character context, but the initiating character is not a delivery target. If the prog returns an item or collection of items, those products are inserted into the field's cell at ground level when the operation completes.

This keeps project labour/material requirements generic while preserving dynamic agriculture state outside the project template.

Crafts can also depend on agriculture state through the `field` / `AgricultureField` craft input. It resolves the current cell's agriculture field, can require a particular field use, crop, orchard, woodland, minimum field condition, minimum crop or woodland health, and minimum crop or woodland yield, and can consume a configured amount of crop or woodland yield during reservation. Use this for forestry, orchard, or gathering crafts that should operate on a live field rather than consuming a pre-existing commodity pile.

## Permission Model
When a field's cell belongs to a property, field work is allowed for administrators, authorised owners, and authorised leaseholders. Fields outside property are currently open to ordinary field commands unless the operation or project template applies its own FutureProg gates.

## FutureProg Surface
Agriculture registers the `field` FutureProg variable type and exposes `location.field`. Built-in helpers include:

- `FieldAt`
- `CreateField`
- `DeleteField`
- `StartFieldProject`
- `FieldScore`
- `SetFieldScore`
- `AdjustFieldScore`
- `DrawFieldHerd`
- `AbsorbNpcIntoFieldHerd`

These are intended for land expansion, scripted estates, controlled wilderness development, seasonal events, and builder automation.

Field dot properties include crop, woodland, and apiary state such as health, yield, growth days, crop stage, harvest readiness, hive count, apiary happiness, apiary stores, and pollination strength.
