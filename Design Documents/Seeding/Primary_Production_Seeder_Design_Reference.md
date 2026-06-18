# FutureMUD Primary Production Seeder Design Reference

## Purpose

This document defines a seeded-content package for primary production chains outside the existing agriculture, hunting, and finished-goods craft packages. The target systems are prospecting, mining, quarrying, charcoal burning, metallurgical production, building-material production, and related intermediate industry.

The package is intended to make the economy move through bulk commodities, durable tools, repair work, site discovery, and multi-stage handoffs. It should not simply add finished metal weapons, armour, furniture, or buildings. It should create the upstream economic matter that other characters, crafts, shops, projects, and settlements consume.

The design assumes the current FutureMUD distinction between:

- **Crafts**: short actions performed by one character over seconds or minutes.
- **Projects**: larger work, group work, cell-bound work, or work that should persist and advance over project ticks.
- **Commodities**: bulk material piles that are the preferred handoff unit between production stages.
- **Items**: durable tools, apparatus, fixtures, containers, finished goods, or special objects.
- **Sites and markers**: room/cell state, tags, or non-holdable props that represent discoverable and exploitable primary resources.

## Design Goals

1. Use commodities as the default output for raw and intermediate goods.
2. Create many handoff points where prospectors, miners, haulers, charcoal burners, smelters, masons, carpenters, smiths, potters, glassworkers, salt workers, tar burners, and builders can each contribute part of the chain.
3. Use local projects for prospecting, extraction, firing, smelting, quarrying, shaft work, and other multi-person or long-running work.
4. Use crafts for short bench-scale transformations, tool maintenance, sorting, trimming, washing, mixing, dressing, and stock preparation.
5. Add enough durable tools and apparatus to create demand for repair, replacement, sharpening, hauling, and workshop infrastructure.
6. Support discovery gameplay before extraction. A room may contain a hidden mineral resource tag or marker; prospecting work can reveal it as a visible surface deposit or known site.
7. Keep the first implementation concrete and data-driven rather than implementing full geology, depletion, hazards, or industrial plant simulation.
8. Preserve era flexibility by seeding a preindustrial baseline first, with later industrial extensions clearly separated.
9. Keep seeder responsibilities layered. Item prototypes and crafts can live with ItemSeeder partials, while project templates, discovery progs, and primary-production domain records can live in a dedicated primary-production package.

## Non-Goals For The First Pass

The first implementation should not attempt to build all of the following systems:

- Full resource-node depletion, vein geometry, ore grade simulation, or geological survey mechanics.
- Automated mine hazards such as collapses, bad air, flooding, dust explosion, or heat stroke.
- A complete construction system for buildings, fortifications, bridges, roads, canals, and mines.
- A full industrial revolution plant model with steam engines, coke blast furnaces, rolling mills, puddling furnaces, Bessemer converters, or powered machinery.
- Automatic world generation of ore deposits or quarry sites.
- Complex environmental impacts such as tailings pollution, forestry depletion, or watershed damage.
- A complete pre-built world or starting town with all resource sites already placed.

Those are good later extensions, but the first pass should create useful content with the current craft, project, material, tag, commodity, item, and FutureProg models.

## Current-System Fit

### Agriculture Boundary

Agriculture already covers fields, crops, herds, apiaries, and managed woodland. It also already uses projects for field operations and commodities for harvested products. Primary production should not duplicate that system. Instead:

- Managed woodlands can supply timber, poles, brushwood, bark, firewood, charcoal wood, resin, gum, fibre, thatch, and similar plant commodities.
- Primary production can consume those woodland commodities to produce charcoal, potash, tar, pitch, lime, fired brick, smelted metal, glass stock, and other industrial feedstocks.
- Agriculture remains the system for growing trees and managing woodland yield.
- Primary production is the system for turning woodland yield into fuel, alkali, waterproofing compounds, and industrial materials.

### Craft Boundary

Crafts should handle small work with one worker and a short duration. Examples:

- Break raw ore with a hammer.
- Sort broken ore by hand.
- Wash a small basket of ore.
- Roast a tray of ore.
- Consolidate an iron bloom.
- Draw a billet into bar stock.
- Dress a stone block.
- Slake a small batch of lime.
- Mix mortar.
- Temper clay body.
- Mould bricks.
- Leach ash into lye.
- Boil brine into salt.
- Render pine tar into pitch.
- Repair a tool handle.
- Sharpen a pick or chisel.

Crafts already support commodity inputs and commodity products. This makes them ideal for intermediate transforms.

### Project Boundary

Projects should handle larger or persistent work. Examples:

- Prospect a landscape for mineral signs.
- Expose a surface deposit.
- Open a mine working.
- Drive an adit.
- Sink a shaft.
- Shore a working.
- Extract ore from a working face.
- Clear a quarry face.
- Quarry rough blocks.
- Burn a charcoal clamp.
- Burn a lime kiln.
- Fire a brick clamp.
- Smelt an iron bloom.
- Smelt copper, tin, lead, or silver ore.
- Operate a glass furnace batch.
- Operate a salt pan or brine boiling house.
- Burn a tar kiln.
- Cut and dry peat turves.
- Dig a new chamber or connect a new room by completion prog.

Local projects are the default because the work belongs to a location. Personal projects are useful only for off-site planning, paperwork, employment abstraction, or jobs that abstract a workplace rather than simulating the cell.

### Discovery Boundary

Prospecting should sit before extraction. A room can contain a resource marker that is not necessarily visible to ordinary players. Prospecting projects can reveal those resources through one of two mechanisms:

1. **Room/cell resource tags**: the room has a hidden or builder-authored tag such as `Mineral Resource - Hematite`, `Mineral Resource - Cassiterite`, or `Mineral Resource - Limestone`. A prospecting completion action or helper prog checks those tags and creates a visible result if one is found.
2. **Hidden marker props**: the room contains a non-holdable, non-destroyable, hidden or builder-only prop representing the underlying deposit. Prospecting changes it, reveals it, or creates a public-facing deposit prop.

The preferred long-term model is cell resource tags plus optional visible deposit props. The lowest-risk first pass is visible/non-visible marker props if cell tags are not currently supported.

## Seeder Architecture And Dependencies

### Recommendation

Do not turn `ItemSeeder` into a monolithic pre-built world seeder. Instead, treat primary production as a layered content package:

1. **Foundational data layer**: tags, materials, aliases, and any required static configuration.
2. **Item and craft catalogue layer**: durable tools, apparatus, props, and commodity-transforming crafts. This can be implemented as `ItemSeeder` partials because the existing item/craft catalogue already lives there.
3. **Primary production domain layer**: local project templates, prospecting project templates, helper FutureProgs, resource discovery conventions, and domain documentation. This should be a dedicated `PrimaryProductionSeeder` or equivalent package.
4. **World placement layer**: optional pre-built mines, quarries, resource rooms, sample settlements, market placements, and demonstration areas. This should be a later `WorldStarterSeeder`, `PrebuiltWorldSeeder`, or scenario-specific seeder, not part of the reusable item catalogue.

This preserves the useful parts of `ItemSeeder` without making every game install the same pre-built mines, quarries, and industrial geography.

### Why Not Make ItemSeeder The World Seeder?

`ItemSeeder` is a good place for reusable item prototypes and craft recipes. It is a poor place for geography, room-specific resources, or live world placement because:

- Item prototypes and crafts should be reusable across many games and eras.
- World placement depends on zones, rooms, overlay packages, terrain, local climate, economy, and builder intent.
- A monolithic item/world seeder would acquire difficult dependencies and become harder to rerun safely.
- Prospecting resources should be placeable by builders in specific rooms rather than globally assumed.
- Some games may want the tools and crafts but not the stock project templates or sample mines.
- Some games may want primary production projects but not a pre-built settlement.

### Preferred File Ownership

Recommended file layout:

```text
Design Documents/Seeding/Primary_Production_Seeder_Design_Reference.md
DatabaseSeeder/Seeders/ItemSeeder.Rework.PrimaryProductionTools.cs
DatabaseSeeder/Seeders/ItemSeederCrafting.PrimaryProduction.cs
DatabaseSeeder/Seeders/PrimaryProductionSeeder.cs
DatabaseSeeder/Seeders/PrimaryProductionSeeder.Projects.cs
DatabaseSeeder/Seeders/PrimaryProductionSeeder.Progs.cs
DatabaseSeeder Unit Tests/PrimaryProductionSeederTests.cs
DatabaseSeeder Unit Tests/ItemSeederPrimaryProductionCraftingTests.cs
MudSharpCore/Work/Projects/Actions/CommodityOutputAction.cs
```

If a future world starter package is added:

```text
DatabaseSeeder/Seeders/WorldStarterSeeder.PrimaryProductionSites.cs
Design Documents/Seeding/World_Starter_Primary_Production_Sites.md
```

### Seeder Dependency Order

Recommended dependency order:

1. `CoreDataSeeder` or equivalent foundational package.
2. `UsefulSeeder` for common tags, item components, utility progs, and starter infrastructure.
3. `SkillPackageSeeder` or `SkillSeeder` for the traits used by projects and crafts.
4. `AgricultureSeeder` if woodland outputs are expected to feed charcoal, tar, potash, timber, or bark chains. This can be optional if primary production also accepts generic wood/firewood commodities.
5. `ItemSeeder` primary-production partials for tools, apparatus, props, and crafts.
6. `PrimaryProductionSeeder` for prospecting projects, extraction projects, smelting projects, kiln projects, and helper progs.
7. `EconomySeeder` or a later economy pass if shops, jobs, market categories, or populations should buy/sell the outputs.
8. Optional world starter/scenario seeder for placing actual hidden resource tags, visible deposit props, sample quarries, mines, kilns, and workshops.

### Proposed Metadata For PrimaryProductionSeeder

`PrimaryProductionSeeder` should be idempotent and repair-capable, similar to other stock content packages.

Suggested prerequisites:

- Core account exists.
- Core utility progs such as `AlwaysTrue` exist.
- Primary production tags have been installed by UsefulSeeder or this package's tag phase.
- Required materials exist.
- Required traits exist or fallback traits are available.
- Commodity project output action is registered before project definitions are installed.
- Primary production item/craft prototypes exist if project material requirements refer to their tags.

Suggested metadata summary:

- **Repeatability**: idempotent.
- **Update capability**: repair existing stock-owned definitions.
- **Ownership**: stock primary production content is tracked by stable tag names, material names, item stable references, craft names, project names, and helper FutureProg names.

## Required Engine Addition

### Project Action: commodityoutput

The current project action set can run progs, grant skill checks, or apply agriculture operations. Primary production needs a first-class project action that creates commodity piles directly.

Add a new project completion action with builder keyword:

```text
commodityoutput
```

The action should create a commodity pile through `CommodityGameItemComponentProto.CreateNewCommodity(...)`. It must not try to manually load the commodity item prototype because the commodity component is a system component and prevents manual loading.

#### Data Fields

The action should persist these fields in its XML definition:

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| MaterialId | solid material id | yes | Exact output material. |
| Weight | double mass in base units | yes | Must be positive. |
| TagId | tag id | no | Commodity pile tag, not necessarily a material tag. |
| UseIndirectDescription | bool | no | Defaults false. Useful for piles described indirectly. |
| Echo | text | no | Sent to project location when output is created. |
| Characteristics | XML child list | no | Optional fixed commodity characteristics, if implemented in first pass. |

#### Completion Behaviour

On completion:

1. Validate that the configured material still exists and is a solid.
2. Validate that the tag still exists if configured.
3. Create the commodity pile via the commodity factory method.
4. Add the item to the gameworld.
5. Place the item in the project location.
6. Set a sensible room layer from the project location or the first active worker.
7. Send the configured echo or a generic production message to the cell.

For local projects, place the output in the local project cell. For personal projects, place the output at the owner location if there is no project cell.

#### Builder Commands

Recommended builder commands:

```text
project set phase <phase> action <action> material <material>
project set phase <phase> action <action> weight <amount>
project set phase <phase> action <action> tag <tag|none>
project set phase <phase> action <action> indirect
project set phase <phase> action <action> echo <text|none>
project set phase <phase> action <action> characteristic <definition> <value|remove>
```

#### Tests

Add tests that prove:

- `commodityoutput` is listed as a valid project action type.
- The action round-trips through XML.
- Submit validation fails if material is missing or weight is non-positive.
- Project completion creates an `ICommodity` with expected material, weight, tag, and characteristics.
- Existing `prog`, `skilluse`, and `agriculture` actions continue to load unchanged.

## Optional Engine Addition: Resource Discovery

Prospecting can be implemented with existing `prog` project actions if FutureProg can inspect the project location, detect cell tags or marker props, and load/reveal a deposit prop. If that is not currently practical, add a native project action with builder keyword:

```text
resourcediscovery
```

### Resource Discovery Action Behaviour

On completion:

1. Inspect the local project location.
2. Search for hidden resource tags or hidden resource marker props.
3. If no matching resource is found, send a failure/no-discovery echo and optionally create a small sample of ordinary stone or soil.
4. If a resource is found, create or reveal a non-holdable deposit prop such as `hematite surface deposits`.
5. Optionally add a discovered/prospected tag to the room or prop.
6. Optionally grant a small commodity sample, such as `2 kg hematite ore` with `Sample Ore Commodity`.
7. Prevent duplicate visible deposit props for the same resource in the same room unless the builder explicitly permits multiple discoveries.

### Resource Discovery Action Data Fields

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| ResourceRootTagId | tag id | yes | Parent tag such as `Mineral Resource`. |
| DepositPropMap | XML map | yes | Maps resource tags to stable prop references or output descriptions. |
| SampleMaterialMap | XML map | no | Maps resource tags to sample commodity material. |
| SampleWeight | mass | no | Optional small sample output. |
| SuccessEcho | text | no | Echo for successful discovery. |
| FailureEcho | text | no | Echo for no discovery. |
| RevealExisting | bool | no | Reveals an existing hidden marker instead of loading a new prop. |
| AddDiscoveredTag | bool | no | Adds a discovered tag to the room or prop if supported. |

This action is optional for the first pass. The first implementation may instead seed a helper FutureProg called `Stock Primary Production - Reveal Mineral Resource` and use ordinary `prog` completion actions.

## Resource Discovery And Prospecting Model

### Core Concept

A room can have an underlying primary resource that players do not automatically know about. A project-based prospecting sequence turns hidden builder knowledge into visible player-facing economic opportunity.

Example:

```text
Room has hidden/builder resource marker:
  Mineral Resource - Hematite

Players start:
  Primary Production - Prospect for Iron Deposits

On completion:
  if hematite resource exists, create visible prop:
    hematite surface deposits

Players can then start:
  Primary Production - Open a Hematite Working
  Primary Production - Sink a Shaft for Hematite
  Primary Production - Extract Hematite Ore
```

### Resource Tag Hierarchy

Add resource-site tags separate from commodity-stage tags.

| Tag | Parent | Purpose |
| --- | --- | --- |
| Primary Production Resource | Primary Production | Root for cell or prop resource markers. |
| Mineral Resource | Primary Production Resource | Any underground/surface mineral resource. |
| Stone Resource | Primary Production Resource | Quarryable stone resources. |
| Clay Resource | Primary Production Resource | Clay pits and ceramic feedstocks. |
| Salt Resource | Primary Production Resource | Brine, salt pan, rock salt. |
| Fuel Resource | Primary Production Resource | Coal, peat, oil shale, bitumen, etc. |
| Alkali Resource | Primary Production Resource | Natron, soda, potash-relevant deposits. |
| Pigment Resource | Primary Production Resource | Ochre, cinnabar, malachite pigment, etc. |
| Sulfur Resource | Primary Production Resource | Sulfur or pyrite resources. |
| Hematite Resource | Mineral Resource | Hematite-bearing site. |
| Limonite Resource | Mineral Resource | Limonite/bog iron-bearing site. |
| Magnetite Resource | Mineral Resource | Magnetite-bearing site. |
| Cassiterite Resource | Mineral Resource | Tin ore site. |
| Malachite Resource | Mineral Resource | Copper carbonate site. |
| Galena Resource | Mineral Resource | Lead/silver-bearing site. |
| Native Copper Resource | Mineral Resource | Native copper site. |
| Gold-Bearing Gravel Resource | Mineral Resource | Placer gold site. |
| Limestone Resource | Stone Resource | Limestone quarry/lime site. |
| Sandstone Resource | Stone Resource | Sandstone quarry site. |
| Granite Resource | Stone Resource | Granite quarry site. |
| Slate Resource | Stone Resource | Slate quarry site. |
| Marble Resource | Stone Resource | Marble quarry site. |
| Clay Pit Resource | Clay Resource | Clay extraction site. |
| Silica Sand Resource | Stone Resource | Glass sand site. |
| Rock Salt Resource | Salt Resource | Rock salt mine site. |
| Brine Spring Resource | Salt Resource | Brine extraction site. |
| Peat Bog Resource | Fuel Resource | Peat cutting site. |
| Coal Seam Resource | Fuel Resource | Coal mining site. |
| Bitumen Seep Resource | Fuel Resource | Bitumen/asphalt collection site. |
| Natron Resource | Alkali Resource | Natron deposit. |
| Ochre Resource | Pigment Resource | Ochre pigment site. |
| Sulfur Deposit Resource | Sulfur Resource | Sulfur collection/mining site. |

### Visible Deposit Props

Prospecting should create visible props that players can look at and use as a launching point for further projects. These props should be non-holdable and indestructible by default. If the item system requires a component to make a static room prop, add or reuse an appropriate fixture/scenery component. Otherwise, omit `Holdable` and omit `Destroyable`.

| Stable Reference | Sdesc | Tags | Description Role |
| --- | --- | --- | --- |
| primary_production_hematite_surface_deposits | hematite surface deposits | Visible Resource Deposit, Hematite Resource | Red-black ironstone staining and exposed nodules. |
| primary_production_limonite_surface_deposits | limonite surface deposits | Visible Resource Deposit, Limonite Resource | Brown iron-rich crusts and bog ore signs. |
| primary_production_cassiterite_surface_deposits | cassiterite surface deposits | Visible Resource Deposit, Cassiterite Resource | Heavy dark grains or pebbles in stream gravels. |
| primary_production_malachite_surface_deposits | malachite-stained copper deposits | Visible Resource Deposit, Malachite Resource | Green-stained copper-bearing stone. |
| primary_production_galena_surface_deposits | galena-bearing surface deposits | Visible Resource Deposit, Galena Resource | Heavy grey lead ore signs. |
| primary_production_gold_bearing_gravel | gold-bearing gravel | Visible Resource Deposit, Gold-Bearing Gravel Resource | Placer gravels for panning and washing. |
| primary_production_limestone_outcrop | a limestone outcrop | Visible Resource Deposit, Limestone Resource | Quarry and lime feedstock. |
| primary_production_slate_outcrop | a slate outcrop | Visible Resource Deposit, Slate Resource | Slate quarry site. |
| primary_production_clay_bank | a workable clay bank | Visible Resource Deposit, Clay Pit Resource | Clay extraction site. |
| primary_production_brine_spring | a brine spring | Visible Resource Deposit, Brine Spring Resource | Salt production site. |
| primary_production_peat_cutting | a peat cutting | Visible Resource Deposit, Peat Bog Resource | Peat fuel site. |
| primary_production_natron_flats | natron-crusted flats | Visible Resource Deposit, Natron Resource | Alkali collection site. |
| primary_production_ochre_bank | an ochre-stained bank | Visible Resource Deposit, Ochre Resource | Pigment source. |

### Prospecting Project Series

Prospecting should be a series, not a single action. The series should scale from general survey through targeted discovery to development.

| Project | Purpose | Inputs | Output/Effect |
| --- | --- | --- | --- |
| Primary Production - Survey Mineral Signs | General prospecting in a plausible room. | Labour, surveying tools optional. | Reveals broad evidence or no evidence. |
| Primary Production - Prospect for Iron Deposits | Search for hematite, limonite, magnetite, bog iron. | Labouring/Mining, hammer, basket. | Reveals iron deposit prop or sample. |
| Primary Production - Prospect for Tin Deposits | Search for cassiterite, usually stream gravels or hard rock. | Labouring/Mining, pan/trough. | Reveals cassiterite deposit or sample. |
| Primary Production - Prospect for Copper Deposits | Search for malachite/native copper/copper ore. | Labouring/Mining, hammer. | Reveals copper deposit or sample. |
| Primary Production - Prospect for Lead And Silver Deposits | Search for galena/silver-bearing signs. | Labouring/Mining. | Reveals galena deposit or sample. |
| Primary Production - Prospect for Quarry Stone | Search for quarryable stone. | Masonry/Labouring, hammer, measuring cord. | Reveals outcrop prop. |
| Primary Production - Prospect for Clay | Search banks, pits, or floodplains. | Labouring/Pottery. | Reveals clay bank prop. |
| Primary Production - Prospect for Salt Or Brine | Search flats, springs, coastal pans, or rock salt. | Labouring/Survival. | Reveals brine spring, salt pan, or rock salt deposit. |
| Primary Production - Prospect for Fuel Deposits | Search for peat, coal, bitumen. | Labouring/Mining/Survival. | Reveals fuel resource prop. |
| Primary Production - Assay A Mineral Sample | Confirm value of a discovered resource. | Small commodity sample, tools, fire/acid if available. | Produces named sample or unlocks extraction fiction. |

The first pass can make the specific projects mechanically similar and vary only target resource tags, skill, difficulty, tools, and output props.

### Mine Development After Discovery

Once a visible deposit prop exists, players can start development projects:

```text
Visible surface deposit
  -> project: open shallow working
  -> project: shore working
  -> project: sink shaft or drive adit
  -> project: extract ore from working face
  -> project/craft chain: break, sort, wash, roast, smelt
```

Development projects should require either:

- the visible deposit prop in the room,
- a discovered/prospected room tag, or
- a resource marker item with the correct resource tag.

### Tag Versus Prop Decision

Use both if possible:

- **Tag** stores the underlying truth: this room has hematite, cassiterite, clay, limestone, brine, etc.
- **Prop** exposes that truth to players: `hematite surface deposits` is something they can see, inspect, and use as an obvious project anchor.

If only one can be implemented in the first pass, prefer visible/non-visible props because they are easier for players and builders to understand. If room/cell tags already exist or are easy to add, prefer tags plus props.

## Data Model And Naming Conventions

### Stable References

Every seeded item prototype, craft, project, knowledge, and helper prog should use a stable reference or deterministic name. Recommended prefixes:

| Content Type | Prefix |
| --- | --- |
| Item prototypes | `primary_production_` |
| Craft names | `primary production - ` or category-specific names |
| Project names | `Primary Production - ` |
| Knowledge | `Primary Production - ` |
| FutureProg helpers | `Stock Primary Production - ` |
| Test fixtures | `primaryproduction_` |

### Tag Naming

Tags are global and often builder-facing. Avoid short ambiguous names like `Raw Ore` or `Fuel` unless the hierarchy clearly disambiguates them. Prefer names such as:

- `Hematite Resource`
- `Visible Resource Deposit`
- `Primary Production Ore Commodity`
- `Raw Ore Commodity`
- `Roasted Ore Commodity`
- `Charcoal Fuel Commodity`
- `Rough Stone Block Commodity`
- `Quicklime Commodity`

If a tag will be used as a broad functional category, put it under a primary production root. If a tag will be used by crafts to distinguish a processing stage, put it under a commodity-stage root. If a tag will be used by prospecting, put it under a resource-site root.

### Material Naming

Material names should be plain lower-case material names where consistent with the existing material catalogue:

- `iron ore`
- `bog iron ore`
- `hematite ore`
- `charcoal`
- `iron bloom`
- `slag`
- `quicklime`
- `slaked lime`
- `lime mortar`
- `prepared clay`
- `green brick`
- `fired brick`
- `rock salt`
- `brine salt`
- `potash`
- `pine tar`
- `pitch`
- `peat`
- `ochre pigment`

Use aliases for common variants and spellings, for example:

- `cassiterite` as alias for `tin ore` or separate material if desired.
- `galena` as alias for `lead ore` or separate material if desired.
- `haematite` as alias for `hematite ore`.
- `lime` as alias for `quicklime` only if the ambiguity is acceptable.
- `sea coal` as alias for `coal`.
- `turf` as alias for `peat` or `dried peat`.

## Tag Catalogue

### Material Function Tags

Add these under `Functions / Material Functions` or a new `Primary Production` child under it.

| Tag | Parent | Purpose |
| --- | --- | --- |
| Primary Production | Material Functions | Root for primary-production material roles. |
| Primary Production Ore | Primary Production | Materials that represent ores. |
| Primary Production Flux | Primary Production | Materials used as fluxes. |
| Primary Production Fuel | Primary Production | Fuels used for industrial heat. |
| Primary Production Stone | Primary Production | Stone and quarryable bulk stone materials. |
| Primary Production Clay | Primary Production | Clay and ceramic feedstocks. |
| Primary Production Aggregate | Primary Production | Sand, gravel, rubble, road metal, aggregate. |
| Primary Production Metal Stock | Primary Production | Blooms, billets, ingots, bars, and similar metal feedstocks. |
| Primary Production Binder | Primary Production | Lime, mortar, plaster, cement-like materials. |
| Primary Production Glass Stock | Primary Production | Glass batch and intermediate glass stock. |
| Primary Production Salt | Primary Production | Salt, brine, and salt-production feedstocks. |
| Primary Production Alkali | Primary Production | Potash, soda ash, natron, lye-related feedstocks. |
| Primary Production Tar And Pitch | Primary Production | Tar, pitch, resin, bitumen, waterproofing feedstocks. |
| Primary Production Pigment | Primary Production | Mineral pigment and paint feedstocks. |
| Primary Production Refractory | Primary Production | Fireclay, crucible clay, furnace lining, firebrick. |
| Primary Production Waste | Primary Production | Slag, tailings, rubble, wasters, spoil. |

### Resource Site Tags

Add these under `Primary Production Resource` as described in the prospecting model.

| Tag | Parent | Purpose |
| --- | --- | --- |
| Primary Production Resource | Primary Production | Root for room/cell/prop resource markers. |
| Mineral Resource | Primary Production Resource | Mineral extraction resources. |
| Stone Resource | Primary Production Resource | Quarry resources. |
| Clay Resource | Primary Production Resource | Clay resources. |
| Salt Resource | Primary Production Resource | Salt and brine resources. |
| Fuel Resource | Primary Production Resource | Peat, coal, bitumen, and other fuel resources. |
| Alkali Resource | Primary Production Resource | Natron, soda, potash-related deposits. |
| Pigment Resource | Primary Production Resource | Mineral pigment resources. |
| Visible Resource Deposit | Primary Production Resource | Player-facing discovered resource prop. |
| Hidden Resource Marker | Primary Production Resource | Builder/engine marker if prop-based discovery is used. |

### Commodity Stage Tags

Add these under `Functions / Material Functions / Primary Production` or under a child such as `Primary Production Commodity`.

| Tag | Parent | Purpose |
| --- | --- | --- |
| Primary Production Commodity | Primary Production | Root for commodity-pile process states. |
| Sample Ore Commodity | Primary Production Commodity | Small sample recovered during prospecting. |
| Raw Ore Commodity | Primary Production Commodity | Unprocessed ore from a mine face. |
| Broken Ore Commodity | Primary Production Commodity | Ore broken small enough for sorting or washing. |
| Sorted Ore Commodity | Primary Production Commodity | Hand-sorted ore. |
| Washed Ore Commodity | Primary Production Commodity | Washed ore ready for roasting or smelting. |
| Roasted Ore Commodity | Primary Production Commodity | Roasted ore ready for smelting. |
| Ore Tailings Commodity | Primary Production Commodity | Waste from washing and sorting. |
| Mine Spoil Commodity | Primary Production Commodity | Waste rock and spoil from mine projects. |
| Charcoal Fuel Commodity | Primary Production Commodity | Industrial charcoal output. |
| Peat Fuel Commodity | Primary Production Commodity | Dried peat/turf fuel. |
| Coal Fuel Commodity | Primary Production Commodity | Coal fuel output, for later eras. |
| Coke Fuel Commodity | Primary Production Commodity | Coke fuel output, for industrial-era chains. |
| Bloom Commodity | Primary Production Commodity | Bloomery output before consolidation. |
| Slag Commodity | Primary Production Commodity | Metallurgical waste. |
| Metal Ingot Commodity | Primary Production Commodity | Cast metal ingots. |
| Metal Billet Commodity | Primary Production Commodity | Forged billet or bar feedstock. |
| Metal Bar Stock Commodity | Primary Production Commodity | Drawn bar stock for smithing. |
| Rough Stone Block Commodity | Primary Production Commodity | Rough quarried block. |
| Dressed Stone Block Commodity | Primary Production Commodity | Squared or dressed building block. |
| Stone Rubble Commodity | Primary Production Commodity | Rubble and spalls. |
| Aggregate Commodity | Primary Production Commodity | Crushed stone, gravel, or road metal. |
| Quicklime Commodity | Primary Production Commodity | Burned lime. |
| Slaked Lime Commodity | Primary Production Commodity | Hydrated lime. |
| Mortar Commodity | Primary Production Commodity | Mixed mortar. |
| Prepared Clay Commodity | Primary Production Commodity | Tempered clay body. |
| Green Brick Commodity | Primary Production Commodity | Unfired brick stock. |
| Fired Brick Commodity | Primary Production Commodity | Fired brick stock. |
| Roof Tile Commodity | Primary Production Commodity | Fired or green roof tile stock as configured. |
| Glass Batch Commodity | Primary Production Commodity | Mixed raw material batch for glassmaking. |
| Glass Blank Commodity | Primary Production Commodity | Melted or formed glass stock. |
| Salt Commodity | Primary Production Commodity | Produced salt. |
| Brine Commodity | Primary Production Commodity | Concentrated brine if modelled as solid-equivalent commodity; otherwise use liquid. |
| Potash Commodity | Primary Production Commodity | Potash or alkali stock. |
| Lye Commodity | Primary Production Commodity | Lye stock if represented as commodity; otherwise use liquid. |
| Tar Commodity | Primary Production Commodity | Wood tar or pine tar. |
| Pitch Commodity | Primary Production Commodity | Boiled pitch. |
| Pigment Commodity | Primary Production Commodity | Mineral pigment stock. |
| Fireclay Commodity | Primary Production Commodity | Refractory clay stock. |
| Crucible Clay Commodity | Primary Production Commodity | Refractory clay body. |

### Tool And Apparatus Tags

Add these under `Functions / Tools` or a `Primary Production Tools` child.

| Tag | Parent | Purpose |
| --- | --- | --- |
| Mining Tool | Tools | Picks, gad, wedge, shovel, windlass. |
| Prospecting Tool | Tools | Hammers, pans, sample bags, surveying tools. |
| Quarrying Tool | Tools | Stone hammers, chisels, wedges, lifting tools. |
| Masonry Tool | Tools | Dressing and measuring tools. |
| Charcoal Burning Tool | Tools | Rakes, shovels, clamp tools. |
| Kiln Tool | Tools | Kiln rake, firing shovel, tongs. |
| Smelting Tool | Tools | Bellows, tongs, crucibles, moulds, furnace tools. |
| Hauling Tool | Tools | Baskets, barrows, sledges, panniers. |
| Surveying Tool | Tools | Plumb bob, measuring cord, straightedge. |
| Saltworking Tool | Tools | Rakes, pans, boiling vessels, salt baskets. |
| Tar Burning Tool | Tools | Tar kiln, pitch kettle, tar rake. |
| Alkali Tool | Tools | Ash hopper, leaching tub, evaporating pan. |
| Peat Cutting Tool | Tools | Peat spade, turf knife, drying rack. |
| Pigment Processing Tool | Tools | Mortar, pestle, grinding slab, washing bowl. |

## Material Catalogue

### Ores

| Material | Tags | Aliases | Notes |
| --- | --- | --- | --- |
| iron ore | Primary Production Ore | ore iron | Generic iron ore for broad settings. |
| bog iron ore | Primary Production Ore | bog ore | Low-technology iron source. |
| hematite ore | Primary Production Ore | haematite, red iron ore | Rich iron ore. |
| limonite ore | Primary Production Ore | brown iron ore | Common hydrated iron ore. |
| magnetite ore | Primary Production Ore | black iron ore | Dense iron ore. |
| copper ore | Primary Production Ore | ore copper | Generic copper ore. |
| malachite ore | Primary Production Ore, Primary Production Pigment | malachite | Copper carbonate ore and pigment. |
| tin ore | Primary Production Ore | cassiterite, ore tin | Tin source for bronze. |
| lead ore | Primary Production Ore | galena, ore lead | Lead and silver-bearing ore. |
| silver ore | Primary Production Ore | ore silver | Generic silver ore. |
| gold ore | Primary Production Ore | ore gold | Generic gold ore. |
| zinc ore | Primary Production Ore | calamine, smithsonite | Optional brass chain support. |

### Fuels And Fluxes

| Material | Tags | Aliases | Notes |
| --- | --- | --- | --- |
| charcoal | Primary Production Fuel, Hot Fire | hardwood charcoal | Core preindustrial industrial fuel. |
| peat | Primary Production Fuel | turf | Cut wet peat. |
| dried peat | Primary Production Fuel | dry turf, peat fuel | Usable peat fuel commodity. |
| coal | Primary Production Fuel, Hot Fire | mineral coal, sea coal | Later medieval/industrial fuel. |
| coke | Primary Production Fuel, Hot Fire | coked coal | Industrial extension. |
| limestone flux | Primary Production Flux | flux, limestone | If exact limestone exists, consider using limestone + tag rather than adding this. |
| potash | Primary Production Flux, Primary Production Glass Stock, Primary Production Alkali | pearl ash | Glass, soap, dyeing, and alkali feedstock. |
| soda ash | Primary Production Flux, Primary Production Glass Stock, Primary Production Alkali | natron, soda | Glass and chemical feedstock. |
| natron | Primary Production Alkali, Primary Production Glass Stock | natural soda | Ancient/medieval alkali deposit. |

### Metallurgical Intermediates

| Material | Tags | Aliases | Notes |
| --- | --- | --- | --- |
| slag | Primary Production Waste | furnace slag | Common smelting waste. |
| iron bloom | Primary Production Metal Stock | bloom | Sponge iron before consolidation. |
| wrought iron billet | Primary Production Metal Stock | iron billet | Input to smithing stock crafts. |
| copper matte | Primary Production Metal Stock | matte | Optional copper intermediate. |
| copper ingot | Primary Production Metal Stock | ingot copper | Generic cast copper stock if copper metal material cannot be used directly. |
| tin ingot | Primary Production Metal Stock | ingot tin | Generic cast tin stock if tin metal material cannot be used directly. |
| lead ingot | Primary Production Metal Stock | ingot lead | Generic cast lead stock if lead metal material cannot be used directly. |
| bronze billet | Primary Production Metal Stock | bronze stock | Alloy stock. |
| brass billet | Primary Production Metal Stock | brass stock | Alloy stock. |

Implementation note: if base metals such as copper, tin, lead, bronze, brass, wrought iron, and steel already exist as materials, prefer using the existing metal material with a commodity-stage tag. Add separate `copper ingot`-style materials only where a distinct intermediate material is required by current content patterns.

### Stone, Lime, Clay, And Building Materials

| Material | Tags | Aliases | Notes |
| --- | --- | --- | --- |
| limestone | Primary Production Stone, Primary Production Flux | chalk, lime stone | Quarry stone and lime source. |
| sandstone | Primary Production Stone | building sandstone | Quarry stone. |
| granite | Primary Production Stone | hardstone | Hard quarry stone. |
| marble | Primary Production Stone | building marble | Decorative stone. |
| basalt | Primary Production Stone | traprock | Road metal and hard stone. |
| slate | Primary Production Stone | roofing slate | Roofing and paving. |
| stone rubble | Primary Production Waste, Primary Production Aggregate | rubble | Byproduct and low-value building material. |
| gravel | Primary Production Aggregate | aggregate | Roads and concrete-like mixes. |
| sand | Primary Production Aggregate, Primary Production Glass Stock | silica sand | Mortar, glass, casting. |
| quicklime | Primary Production Binder | burnt lime, lime | Caustic burned lime. |
| slaked lime | Primary Production Binder | hydrated lime | Mortar and plaster feedstock. |
| lime mortar | Primary Production Binder | mortar | Construction binder. |
| gypsum | Primary Production Binder | plaster stone | Plaster feedstock. |
| plaster | Primary Production Binder | gypsum plaster | Finished plaster stock. |
| clay | Primary Production Clay | potter's clay | Base clay feedstock. |
| fireclay | Primary Production Clay, Primary Production Refractory | refractory clay | Furnaces, crucibles, firebrick. |
| prepared clay | Primary Production Clay | clay body | Tempered clay ready for forming. |
| green brick | Primary Production Clay | unfired brick | Unfired brick stock. |
| fired brick | Primary Production Stone, Primary Production Clay | brick | Fired masonry unit. |
| firebrick | Primary Production Refractory, Primary Production Stone | refractory brick | Furnace and kiln lining. |
| roof tile | Primary Production Stone, Primary Production Clay | tile | Fired roofing material. |
| glass batch | Primary Production Glass Stock | batch | Mixed glass raw feedstock. |
| glass blank | Primary Production Glass Stock | glass stock | Generic glassworking stock. |

### Salt, Alkali, Tar, Pitch, And Pigments

| Material | Tags | Aliases | Notes |
| --- | --- | --- | --- |
| rock salt | Primary Production Salt | halite | Mined salt. |
| sea salt | Primary Production Salt | salt | Evaporated sea salt. |
| brine salt | Primary Production Salt | boiled salt | Salt from brine boiling. |
| lye | Primary Production Alkali | caustic lye | May already exist as a liquid/material; reuse if present. |
| wood ash | Primary Production Alkali, Primary Production Waste | ash | Feedstock for lye and potash. |
| pine tar | Primary Production Tar And Pitch | wood tar, tar | Waterproofing and naval stores. |
| pitch | Primary Production Tar And Pitch | boiled pitch | Thickened tar/pitch. |
| resin | Primary Production Tar And Pitch | pine resin | Woodland output and tar input. |
| bitumen | Primary Production Tar And Pitch, Primary Production Fuel | asphalt | Natural seep or mineral pitch. |
| ochre pigment | Primary Production Pigment | ochre | Generic pigment if colour-specific versions are not used. |
| red ochre pigment | Primary Production Pigment | red ochre | Iron oxide pigment. |
| yellow ochre pigment | Primary Production Pigment | yellow ochre | Iron oxide pigment. |
| malachite pigment | Primary Production Pigment | green earth, malachite green | Copper pigment. |
| azurite pigment | Primary Production Pigment | blue copper pigment | Optional blue pigment. |
| cinnabar pigment | Primary Production Pigment | vermilion ore | Pigment and mercury source if later modelled. |
| sulfur | Primary Production Pigment, Primary Production Ore | brimstone | Alchemy, medicine, gunpowder later. |
| saltpeter | Primary Production Alkali | nitre, niter, potassium nitrate | Gunpowder and chemistry later. |

## Tool And Apparatus Catalogue

The following tools should be seeded as durable item prototypes. Exact component choices should follow existing item authoring patterns for tools, holdables, wearables, containers, furniture, and destroyables.

### Prospecting, Mining, And Hauling Tools

| Stable Reference | Sdesc | Tags | Notes |
| --- | --- | --- | --- |
| primary_production_prospecting_hammer | a prospecting hammer | Prospecting Tool, Mining Tool | Small hammer for samples and outcrops. |
| primary_production_sample_bag | a set of sample bags | Prospecting Tool, Container | Holds ore samples. |
| primary_production_gold_pan | a shallow prospecting pan | Prospecting Tool | Placer prospecting and washing. |
| primary_production_mining_pick | a mining pick | Mining Tool, Digging, Tool | Main extraction tool. |
| primary_production_mattock | a heavy mattock | Mining Tool, Digging, Tool | Soil, clay, and shallow workings. |
| primary_production_shovel | a broad iron shovel | Mining Tool, Hauling Tool, Digging | Spoil, ore, lime, charcoal. |
| primary_production_gad | an iron mining gad | Mining Tool, Stone Cutting | Wedge-like mining tool. |
| primary_production_wedge_set | a set of iron wedges | Mining Tool, Quarrying Tool | Splitting stone and ore. |
| primary_production_sledgehammer | a heavy sledgehammer | Mining Tool, Quarrying Tool | Driving wedges and breaking rock. |
| primary_production_hand_hammer | a short hand hammer | Mining Tool, Masonry Tool | Breaking ore and dressing stone. |
| primary_production_crowbar | an iron crowbar | Mining Tool, Quarrying Tool | Prying and shifting rock. |
| primary_production_ore_basket | a wicker ore basket | Hauling Tool, Container | Commodity handling fiction; may be a container. |
| primary_production_ore_sack | a coarse ore sack | Hauling Tool, Container | Lower-quality hauling container. |
| primary_production_hand_barrow | a wooden hand-barrow | Hauling Tool | Two-person carrying aid if supported. |
| primary_production_wheelbarrow | a wooden wheelbarrow | Hauling Tool | Later medieval/early modern option. |
| primary_production_windlass | a timber mine windlass | Mining Tool, Hauling Tool | Room tool for shaft projects. |
| primary_production_pulley_block | a pulley block | Hauling Tool | Hoisting tool. |
| primary_production_hemp_rope_coil | a coil of heavy hemp rope | Hauling Tool, String | Consumable/usable support material. |
| primary_production_timber_prop | a rough timber prop | Mining Tool, Construction Material | Project material for shoring. |

### Quarrying And Masonry Tools

| Stable Reference | Sdesc | Tags | Notes |
| --- | --- | --- | --- |
| primary_production_masons_hammer | a mason's hammer | Masonry Tool, Quarrying Tool | Dressing and shaping. |
| primary_production_point_chisel | a point chisel | Masonry Tool, Stone Cutting | Rough dressing. |
| primary_production_flat_chisel | a flat stone chisel | Masonry Tool, Stone Cutting | Fine dressing. |
| primary_production_pitching_tool | a pitching tool | Masonry Tool, Quarrying Tool | Edge work. |
| primary_production_stone_saw | a stone saw | Masonry Tool, Stone Cutting | Higher-skill/later option. |
| primary_production_wooden_mallet | a heavy wooden mallet | Masonry Tool | Chisel work. |
| primary_production_straightedge | a wooden straightedge | Masonry Tool, Surveying Tool | Quality tool. |
| primary_production_plumb_bob | a lead plumb bob | Surveying Tool, Masonry Tool | Layout and shaft work. |
| primary_production_measuring_cord | a knotted measuring cord | Surveying Tool | Layout. |
| primary_production_lifting_tongs | a pair of lifting tongs | Quarrying Tool, Hauling Tool | Moving stone blocks. |
| primary_production_block_and_tackle | a block and tackle | Hauling Tool | Project room tool. |

### Charcoal, Kiln, Furnace, Salt, Alkali, And Tar Tools

| Stable Reference | Sdesc | Tags | Notes |
| --- | --- | --- | --- |
| primary_production_charcoal_rake | a long charcoal rake | Charcoal Burning Tool, Kiln Tool | Working clamps and kilns. |
| primary_production_firing_shovel | a firing shovel | Kiln Tool, Smelting Tool | Fuel handling. |
| primary_production_fire_hook | a long fire hook | Kiln Tool, Smelting Tool | Hearth and furnace control. |
| primary_production_bellows | a pair of leather bellows | Smelting Tool | Furnace air supply. |
| primary_production_large_bellows | a large workshop bellows | Smelting Tool | Room tool for larger projects. |
| primary_production_clay_tuyere | a clay tuyere | Smelting Tool | Consumable furnace part. |
| primary_production_furnace_tongs | a pair of furnace tongs | Smelting Tool | Handling hot stock. |
| primary_production_crucible_tongs | a pair of crucible tongs | Smelting Tool | Crucible metallurgy. |
| primary_production_clay_crucible | a clay crucible | Smelting Tool | Consumable or low-durability tool. |
| primary_production_ingot_mould | an ingot mould | Smelting Tool | Casting output stock. |
| primary_production_slag_hook | a slag hook | Smelting Tool | Bloomery and furnace operation. |
| primary_production_charcoal_clamp_site | a prepared charcoal clamp site | Charcoal Burning Tool | Room fixture or project-created marker item. |
| primary_production_lime_kiln | a stone lime kiln | Kiln Tool | Room fixture. |
| primary_production_brick_clamp_site | a prepared brick clamp site | Kiln Tool | Room fixture or marker. |
| primary_production_bloomery_furnace | a clay bloomery furnace | Smelting Tool | Room fixture. |
| primary_production_smelting_furnace | a clay smelting furnace | Smelting Tool | Room fixture. |
| primary_production_salt_pan | a broad salt pan | Saltworking Tool | Evaporating salt water or brine. |
| primary_production_brine_bucket | a brine bucket | Saltworking Tool, Container | Brine handling. |
| primary_production_salt_rake | a salt rake | Saltworking Tool | Raking crystals. |
| primary_production_ash_hopper | an ash hopper | Alkali Tool | Leaching ash. |
| primary_production_leaching_tub | a leaching tub | Alkali Tool | Lye/potash process. |
| primary_production_evaporating_pan | an evaporating pan | Alkali Tool, Saltworking Tool | Concentration by boiling. |
| primary_production_tar_kiln | a clay-lined tar kiln | Tar Burning Tool | Pine tar production. |
| primary_production_pitch_kettle | an iron pitch kettle | Tar Burning Tool | Boiling tar into pitch. |
| primary_production_peat_spade | a peat spade | Peat Cutting Tool | Cutting turves. |
| primary_production_turf_knife | a turf knife | Peat Cutting Tool | Cutting and trimming peat. |
| primary_production_pigment_mortar | a stone pigment mortar | Pigment Processing Tool | Grinding pigments. |
| primary_production_grinding_slab | a pigment grinding slab | Pigment Processing Tool | Mineral pigment processing. |

## Knowledge And Skill Coverage

Create one broad knowledge package and then specialised knowledge packages where existing content patterns require them.

| Knowledge | Purpose |
| --- | --- |
| Primary Production Fundamentals | Shared knowledge for basic extraction, hauling, fuel, and material prep. |
| Primary Prospecting | Survey, resource signs, samples, and assay work. |
| Primary Mining | Mining projects and ore extraction. |
| Primary Quarrying | Quarrying and rough block production. |
| Primary Charcoal Burning | Charcoal clamp projects and fuel handling. |
| Primary Lime And Kiln Work | Lime, brick, tile, and kiln chains. |
| Primary Smelting | Ore smelting, bloomery, and non-ferrous smelting. |
| Primary Masonry Materials | Stone dressing, aggregate, mortar, plaster. |
| Primary Glass Batch | Glass batch preparation and furnace stock. |
| Primary Saltworking | Salt pans, brine boiling, and rock salt extraction. |
| Primary Alkali Making | Ash leaching, potash, lye, soda/natron collection. |
| Primary Tar And Pitch | Resin collection, tar kilns, and pitch boiling. |
| Primary Pigment Processing | Mineral pigment extraction, washing, and grinding. |
| Primary Peat Cutting | Cutting, drying, and stacking peat fuel. |

Use existing traits where possible:

| Trait | Use |
| --- | --- |
| Labouring | Generic hauling, digging, loading, tending. |
| Mining | Add if present; otherwise use Labouring for first pass and document future Mining trait. |
| Masonry | Quarrying, stone dressing, mortar, building materials. |
| Blacksmithing | Bloom consolidation, iron stock, furnace work. |
| Smelting | Use where present for smelting projects and crafts. |
| Carpentry | Timber props, barrows, shoring, clamps. |
| Pottery | Clay body, bricks, tiles, kiln work. |
| Glassworking | Glass batch and glass furnace chains. |
| Survival | Salt pans, peat cutting, pitch/tar work, prospecting fallback. |
| Supervising | Foreman and master-worker project labour. |
| Civil Engineering | Surveying, shaft/adit projects, large quarry projects if enabled. |

If the skill package does not always seed `Mining`, do not make first-pass stock content depend on it. Either add Mining as a stock professional skill or use Labouring/Masonry/Smelting with knowledge gates.

## Chain Design

### Chain A: Prospecting To Mine Development

Purpose: make mining start with discovery, not just immediate extraction.

```text
room has hidden resource tag or marker
  -> project: survey mineral signs
  -> project: prospect for specific resource family
  -> visible prop: hematite surface deposits, cassiterite gravels, galena outcrop, etc.
  -> project: assay sample
  -> project: open shallow working / sink shaft / drive adit
  -> project: extract raw ore
```

Recommended discovery outputs:

| Resource | Visible Prop | Optional Commodity Sample |
| --- | --- | --- |
| Hematite Resource | hematite surface deposits | hematite ore, Sample Ore Commodity |
| Cassiterite Resource | cassiterite surface deposits | tin ore, Sample Ore Commodity |
| Malachite Resource | malachite-stained copper deposits | malachite ore, Sample Ore Commodity |
| Galena Resource | galena-bearing surface deposits | lead ore, Sample Ore Commodity |
| Gold-Bearing Gravel Resource | gold-bearing gravel | gold ore or placer concentrate sample |
| Limestone Resource | limestone outcrop | limestone sample or no commodity |
| Clay Pit Resource | workable clay bank | clay sample |
| Brine Spring Resource | brine spring | brine/salt sample |
| Peat Bog Resource | peat cutting signs | peat sample |
| Ochre Resource | ochre-stained bank | ochre pigment sample |

### Chain B: Woodland To Charcoal

Purpose: turn managed woodland outputs into high-value industrial fuel.

```text
managed woodland / coppice
  -> field/project operation: fell, coppice, gather wood
  -> commodity: firewood, billets, brushwood, charcoal wood
  -> local project: build and burn charcoal clamp
  -> commodity: charcoal fuel commodity
  -> byproducts: ash, charcoal fines, poorly burned brands
```

First-pass outputs:

| Output | Material | Tag |
| --- | --- | --- |
| Charcoal | charcoal | Charcoal Fuel Commodity |
| Ash | ash or wood ash | Primary Production Waste / Alkali feedstock |
| Charcoal fines | charcoal | Charcoal Fuel Commodity or Primary Production Waste |

Recommended project:

| Project | Type | Output |
| --- | --- | --- |
| Primary Production - Burn a Charcoal Clamp | local | 60-120 kg charcoal from 250-500 kg wood input. |

### Chain C: Mining And Ore Preparation

Purpose: create raw ore commodities and process them into smelt-ready feedstock.

```text
mine site
  -> project: open working, drive adit, sink shaft, shore working
  -> project: extract ore from working face
  -> raw ore commodity + mine spoil
  -> craft: break ore
  -> broken ore commodity
  -> craft: sort ore
  -> sorted ore commodity + tailings/spoil
  -> craft: wash ore
  -> washed ore commodity + tailings
  -> craft/project: roast ore where appropriate
  -> roasted ore commodity
```

First-pass ore families:

| Ore Family | Raw Material | Final Smelt Input |
| --- | --- | --- |
| Iron | iron ore, bog iron ore, hematite ore, limonite ore, magnetite ore | washed or roasted iron ore |
| Copper | copper ore, malachite ore | washed or roasted copper ore |
| Tin | tin ore | washed tin ore |
| Lead/Silver | lead ore, silver ore | roasted lead/silver ore |
| Gold | gold ore | sorted or washed gold ore |

Recommended extraction projects:

| Project | Type | Output |
| --- | --- | --- |
| Primary Production - Extract Iron Ore | local | Raw iron ore + mine spoil. |
| Primary Production - Extract Copper Ore | local | Raw copper ore + mine spoil. |
| Primary Production - Extract Tin Ore | local | Raw tin ore + mine spoil. |
| Primary Production - Extract Lead Ore | local | Raw lead ore + mine spoil. |
| Primary Production - Extract Stone From Mine | local | Waste stone/rubble. |

Recommended preparation crafts:

| Craft | Input | Output |
| --- | --- | --- |
| break raw ore | raw ore commodity | broken ore commodity |
| sort broken ore | broken ore commodity | sorted ore + tailings |
| wash sorted ore | sorted ore + water access | washed ore + tailings |
| roast washed ore | washed ore + charcoal/fuel | roasted ore |

### Chain D: Bloomery Iron

Purpose: provide a preindustrial iron path without requiring blast furnaces.

```text
washed or roasted iron ore + charcoal + flux
  -> local project: smelt an iron bloom
  -> commodity: iron bloom + slag
  -> craft: consolidate iron bloom
  -> commodity: wrought iron billet
  -> craft: draw wrought iron bar stock
  -> commodity: metal bar stock / tool blank stock / weapon stock
```

Recommended projects and crafts:

| Type | Name | Inputs | Outputs |
| --- | --- | --- | --- |
| Project | Primary Production - Smelt an Iron Bloom | roasted iron ore, charcoal, flux, bloomery furnace, bellows | iron bloom, slag |
| Craft | consolidate iron bloom | iron bloom, charcoal, anvil, hammer, tongs | wrought iron billet, slag |
| Craft | draw wrought iron bar stock | wrought iron billet, forge tools | wrought iron bar stock |
| Craft | make iron tool blank stock | wrought iron billet | Tool Blank Stock commodity |

### Chain E: Non-Ferrous Smelting And Alloying

Purpose: support copper, bronze, brass, lead, pewter, silver, and precious metal chains.

```text
washed/roasted copper ore + charcoal + flux
  -> smelt copper
  -> copper ingot or copper commodity with Metal Ingot tag

washed tin ore + charcoal
  -> smelt tin
  -> tin ingot

copper ingot + tin ingot
  -> alloy bronze billet

copper ingot + zinc ore/calamine or zinc ingot
  -> alloy brass billet

roasted lead ore
  -> smelt lead
  -> lead ingot + optional silver-bearing byproduct later
```

First pass should seed copper, tin, lead, and bronze. Brass and silver refining can be a second pass if needed.

### Chain F: Quarrying And Masonry Stone

Purpose: produce building stone, rubble, aggregate, and dressed blocks.

```text
quarry face
  -> project: clear quarry face
  -> project: quarry rough blocks
  -> rough stone block commodity + rubble
  -> craft/project: dress stone block
  -> dressed stone block commodity + stone spalls
  -> craft: break rubble into aggregate
  -> aggregate commodity
```

Stone families:

| Stone | Use |
| --- | --- |
| limestone | blocks, lime, mortar, flux |
| sandstone | ordinary building blocks |
| granite | hard blocks, road metal |
| marble | high-status blocks and decorative stock |
| slate | roof, paving, writing tablets |
| basalt | road metal and hard stone |

Recommended projects:

| Project | Output |
| --- | --- |
| Primary Production - Clear a Quarry Face | rubble or no output; unlocks site fiction. |
| Primary Production - Quarry Limestone Blocks | rough limestone blocks + rubble. |
| Primary Production - Quarry Sandstone Blocks | rough sandstone blocks + rubble. |
| Primary Production - Quarry Granite Blocks | rough granite block + rubble. |
| Primary Production - Quarry Slate | slate block or roof slate commodity. |

### Chain G: Lime, Mortar, Plaster, And Binders

Purpose: create construction binders from stone and fuel.

```text
limestone + charcoal/fuel
  -> project: burn lime kiln
  -> quicklime commodity
  -> craft: slake lime with water
  -> slaked lime commodity
  -> craft: mix mortar with sand
  -> lime mortar commodity

gypsum + fuel
  -> craft/project: calcine gypsum
  -> plaster commodity
```

First-pass crafts/projects:

| Type | Name | Inputs | Outputs |
| --- | --- | --- | --- |
| Project | Primary Production - Burn a Lime Kiln | limestone, charcoal, lime kiln | quicklime |
| Craft | slake quicklime | quicklime, water source | slaked lime |
| Craft | mix lime mortar | slaked lime, sand | lime mortar |
| Craft | calcine gypsum plaster | gypsum, fuel | plaster |

### Chain H: Clay, Bricks, Tiles, Refractory, And Kilns

Purpose: support building materials, furnace construction, and kiln economies.

```text
clay + sand/straw/grog temper
  -> craft: temper clay body
  -> prepared clay commodity
  -> craft: mould bricks / mould tiles
  -> green brick or green tile commodity
  -> project: fire brick clamp or kiln
  -> fired brick / roof tile commodity + wasters

fireclay + grog
  -> craft: prepare refractory clay body
  -> crucible clay or firebrick clay commodity
  -> craft/project: mould crucibles / firebricks / furnace linings
```

First-pass crafts/projects:

| Type | Name | Inputs | Outputs |
| --- | --- | --- | --- |
| Craft | temper clay body | clay, sand/straw/grog, water | prepared clay |
| Craft | mould green bricks | prepared clay, brick mould | green brick |
| Craft | mould roof tiles | prepared clay, tile mould | roof tile or green tile |
| Project | Primary Production - Fire a Brick Clamp | green bricks, fuel | fired bricks, wasters |
| Project | Primary Production - Fire Roof Tiles | green tiles, fuel | roof tiles, wasters |
| Craft | prepare refractory clay | fireclay, grog, water | crucible clay or firebrick body |
| Craft | mould clay crucibles | crucible clay, mould | unfired crucibles or crucible stock |
| Project | Fire Refractory Bricks | green firebricks, fuel | firebrick |

### Chain I: Glass Batch And Glass Stock

Purpose: supply glassworking with upstream raw materials rather than direct finished glass.

```text
sand + flux + lime/stabiliser
  -> craft: prepare glass batch
  -> glass batch commodity
  -> project/craft: fire glass batch
  -> glass blank commodity
  -> downstream glassworking crafts
```

First-pass crafts/projects:

| Type | Name | Inputs | Outputs |
| --- | --- | --- | --- |
| Craft | prepare soda-lime glass batch | sand, soda ash/potash, lime | glass batch |
| Project | Fire a glass furnace batch | glass batch, charcoal/fuel, furnace | glass blank |

### Chain J: Timber Stock And Mine Support

Purpose: connect agriculture woodland to mining, quarrying, construction, and transport.

```text
timber / poles / logs
  -> craft/project: hew beam stock
  -> commodity: beam stock
  -> craft: split laths / planks / wedges
  -> commodity: lath stock, board stock, wedge stock
  -> projects consume props, beams, boards, laths, charcoal wood
```

Some of this may already exist in household or carpentry packages. Reuse existing timber stock tags and crafts when possible. Add only the mine/quarry-specific support outputs needed for shoring and hauling.

### Chain K: Salt And Brine

Purpose: create salt for food preservation, medicine, tanning, trade, and later chemical chains.

```text
salt resource
  -> project: prospect for salt or brine
  -> visible prop: brine spring, salt pan, rock salt seam
  -> project/craft: collect brine or mine rock salt
  -> craft/project: boil brine or evaporate salt pan
  -> salt commodity
```

First-pass salt processes:

| Type | Name | Inputs | Outputs |
| --- | --- | --- | --- |
| Project | Prospect for Salt Or Brine | salt resource room | brine spring / salt deposit prop |
| Project | Work a Salt Pan | salt pan resource, labour | sea salt commodity |
| Project | Boil Brine For Salt | brine, fuel, salt pan | brine salt commodity |
| Project | Mine Rock Salt | rock salt resource, mining tools | rock salt commodity |
| Craft | crush rock salt | rock salt commodity | salt commodity |

### Chain L: Ash, Alkali, Potash, Lye, And Natron

Purpose: supply glassmaking, soap, dyeing, leather work, washing, and chemical processes.

```text
wood ash / plant ash / natron deposit
  -> craft: leach ash
  -> lye or alkali liquor
  -> craft/project: evaporate lye
  -> potash commodity
  -> craft: calcine potash if needed
  -> refined potash / pearl ash
```

First-pass alkali processes:

| Type | Name | Inputs | Outputs |
| --- | --- | --- | --- |
| Craft | leach wood ash | wood ash, water, ash hopper | lye or weak alkali stock |
| Craft | evaporate lye to potash | lye, fuel, pan | potash commodity |
| Project | Collect Natron | natron resource, labour | natron / soda ash commodity |
| Craft | refine soda ash | natron/soda, water/fuel optional | soda ash commodity |

If lye is better represented as a liquid in the current material system, use liquid outputs for lye and commodity outputs for potash/soda ash.

### Chain M: Tar, Pitch, Resin, And Bitumen

Purpose: support shipbuilding, waterproofing, torches, roofing, sealed containers, roads, and repair work.

```text
resinous wood / pine roots / resin
  -> project: burn tar kiln
  -> pine tar commodity
  -> craft: boil tar into pitch
  -> pitch commodity

bitumen seep
  -> project/craft: collect bitumen
  -> bitumen commodity
```

First-pass tar and pitch processes:

| Type | Name | Inputs | Outputs |
| --- | --- | --- | --- |
| Project | Burn a Tar Kiln | resinous wood, fuel, tar kiln | pine tar, charcoal/ash byproduct |
| Craft | boil tar into pitch | pine tar, fuel, pitch kettle | pitch |
| Project | Collect Bitumen | bitumen seep resource | bitumen commodity |
| Craft | prepare pitch sealant | pitch, fibre/ash optional | prepared pitch commodity |

### Chain N: Peat, Coal, And Fuel Extension

Purpose: add regional fuel diversity and later industrial paths.

```text
peat bog / coal seam
  -> prospect for fuel deposits
  -> project: cut peat / mine coal
  -> commodity: wet peat or coal
  -> project/craft: dry peat / grade coal
  -> fuel commodity
  -> later: coke ovens for industrial extension
```

First-pass fuel processes:

| Type | Name | Inputs | Outputs |
| --- | --- | --- | --- |
| Project | Cut Peat Turves | peat bog resource, peat tools | wet peat commodity |
| Project | Dry Peat Turves | wet peat, drying area | dried peat fuel commodity |
| Project | Mine Coal | coal seam resource, mining tools | coal fuel commodity |
| Project | Burn Coke | coal, coke oven, fuel | coke fuel commodity |

Coal and coke can be tagged as later medieval/industrial if era separation is required.

### Chain O: Mineral Pigments

Purpose: feed painting, dyes, inks, cosmetics, heraldry, writing, and luxury crafts.

```text
ochre/malachite/azurite/cinnabar resource
  -> prospect pigment source
  -> collect pigment earth or ore
  -> wash, dry, grind
  -> pigment commodity
```

First-pass pigment processes:

| Type | Name | Inputs | Outputs |
| --- | --- | --- | --- |
| Project | Collect Ochre Earth | ochre bank resource | ochre pigment stock |
| Craft | wash ochre pigment | ochre stock, water | washed pigment |
| Craft | grind mineral pigment | pigment stock, grinding slab | fine pigment commodity |
| Craft | prepare malachite pigment | malachite ore, grinder | malachite pigment |
| Craft | prepare lampblack pigment | soot/charcoal, oil/water binder optional | black pigment |

## Project Catalogue

### Shared Project Shape

Each primary production local project should generally use this structure:

```text
Phase 1: Prepare the site
- Mandatory labour: labourers or skilled workers.
- Optional supervision: foreman/master worker.
- Materials: support timber, rope, fuel, tools, apparatus, or feedstock.

Phase 2: Do the main work
- Mandatory labour: miners, quarrymen, burners, furnace tenders, bellows workers, haulers.
- Optional endless labour: tending fire, pumping water, hauling spoil, carrying fuel.
- Materials: fuel, ore, flux, green bricks, limestone, brine, ash, resinous wood, etc.

Phase 3: Clear and recover output
- Mandatory labour: sorting, hauling, drawing, unloading.
- Actions: commodityoutput for main output, optional commodityoutput for byproducts, optional skilluse.
```

Projects can have multiple completion actions. Use multiple commodity outputs for main product and byproduct. For example, smelting can output both `iron bloom` and `slag`.

### Priority 1 Projects

| Project Name | Skill/Trait | Key Inputs | Main Output/Effect | Notes |
| --- | --- | --- | --- | --- |
| Primary Production - Survey Mineral Signs | Labouring, Mining, Survival | prospecting hammer, sample bags optional | discovery echo or broad clue | First step before targeted prospecting. |
| Primary Production - Prospect for Iron Deposits | Labouring or Mining | prospecting tools | hematite/limonite/magnetite/bog iron deposit prop or sample | Opens iron chain. |
| Primary Production - Prospect for Tin Deposits | Labouring or Mining | pan, hammer | cassiterite deposit prop or sample | Opens bronze chain. |
| Primary Production - Prospect for Copper Deposits | Labouring or Mining | hammer, sample bags | malachite/copper deposit prop or sample | Opens copper/bronze chain. |
| Primary Production - Prospect for Quarry Stone | Masonry or Labouring | mason's hammer, measuring cord | quarry outcrop prop | Opens quarry chain. |
| Primary Production - Burn a Charcoal Clamp | Labouring or Charcoal Burning | firewood/charcoal wood, earth/clay if modelled, charcoal rake | charcoal | High priority because it fuels many chains. |
| Primary Production - Extract Iron Ore | Labouring or Mining | mining pick, baskets, supports optional | raw iron ore, mine spoil | Basic mine extraction. |
| Primary Production - Extract Copper Ore | Labouring or Mining | mining tools | raw copper ore, mine spoil | Bronze path. |
| Primary Production - Extract Tin Ore | Labouring or Mining | mining tools | raw tin ore, mine spoil | Bronze path. |
| Primary Production - Quarry Limestone Blocks | Masonry or Labouring | quarry tools, wedges, barrow | rough limestone block, rubble | Also feeds lime. |
| Primary Production - Quarry Sandstone Blocks | Masonry or Labouring | quarry tools | rough sandstone block, rubble | Common building stone. |
| Primary Production - Burn a Lime Kiln | Masonry or Labouring | limestone, charcoal, lime kiln | quicklime | Construction binder path. |
| Primary Production - Fire a Brick Clamp | Pottery or Labouring | green bricks, fuel, clamp/kiln | fired bricks, wasters | Building materials. |
| Primary Production - Smelt an Iron Bloom | Smelting or Blacksmithing | roasted iron ore, charcoal, flux, bloomery | iron bloom, slag | Important but should require setup. |
| Primary Production - Smelt Copper Ore | Smelting | roasted copper ore, charcoal, flux | copper ingot/commodity, slag | Bronze path. |
| Primary Production - Smelt Tin Ore | Smelting | washed tin ore, charcoal | tin ingot/commodity, slag | Bronze path. |
| Primary Production - Boil Brine For Salt | Saltworking or Labouring | brine, fuel, salt pan | salt | Food preservation and trade. |
| Primary Production - Burn a Tar Kiln | Labouring or Survival | resinous wood, tar kiln | pine tar | Waterproofing and shipbuilding. |
| Primary Production - Cut Peat Turves | Labouring or Survival | peat bog resource, peat tools | wet peat | Regional fuel. |

### Priority 2 Projects

| Project Name | Skill/Trait | Key Inputs | Main Output/Effect | Notes |
| --- | --- | --- | --- | --- |
| Primary Production - Assay A Mineral Sample | Mining or Smelting | sample ore, tools | confirmation, refined sample, or knowledge gate | Optional gate between prospecting and extraction. |
| Primary Production - Open a Shallow Mine Working | Labouring, Mining, Civil Engineering | timber props, rope, picks, shovels | optional mine marker or no output | Site-development project. |
| Primary Production - Shore a Mine Working | Carpentry or Labouring | timber props, beams, rope | optional safety marker | Consumes wood and creates maintenance loop. |
| Primary Production - Drive an Adit | Mining, Civil Engineering | tools, timber, hauling gear | new room via prog, spoil | Use completion prog if builder config exists. |
| Primary Production - Sink a Mine Shaft | Mining, Civil Engineering | windlass, rope, timber, tools | new room via prog, spoil | More expensive site-development project. |
| Primary Production - Quarry Granite Blocks | Masonry | wedges, hammers, lifting gear | rough granite block, rubble | Harder quarry. |
| Primary Production - Quarry Slate | Masonry | quarry tools | slate/roof slate | Roofing path. |
| Primary Production - Fire Roof Tiles | Pottery | green tiles, fuel | roof tiles, wasters | Building path. |
| Primary Production - Fire a Glass Furnace Batch | Glassworking | glass batch, charcoal, furnace | glass blank | Glass upstream path. |
| Primary Production - Smelt Lead Ore | Smelting | roasted lead ore, charcoal | lead ingot, slag | Lead and silver path. |
| Primary Production - Work a Salt Pan | Saltworking or Labouring | salt pan resource | sea salt | Coastal/arid chain. |
| Primary Production - Collect Natron | Labouring or Survival | natron flats resource | natron/soda ash | Glass/alkali chain. |
| Primary Production - Dry Peat Turves | Labouring | wet peat, drying area | dried peat fuel | Fuel chain. |
| Primary Production - Mine Coal | Mining | coal seam resource, mining tools | coal | Later medieval/industrial. |
| Primary Production - Collect Ochre Earth | Labouring or Pigment Processing | ochre resource | ochre pigment stock | Pigment chain. |
| Primary Production - Collect Bitumen | Labouring | bitumen seep resource | bitumen | Waterproofing/road material. |

### Site-Creation Projects

Site creation should be conservative in the first pass. Use project completion progs for room creation rather than hardcoding mine geometry into the project system.

Candidate site projects:

| Project | Completion Prog | Notes |
| --- | --- | --- |
| Drive A New Mine Chamber | create new room from template, link to current mine room | Requires builder-supplied overlay package, zone, and template room. |
| Sink A Shaft Downward | create lower room and vertical exit | Needs exit-link helper prog and safe builder constraints. |
| Open A Quarry Bench | create quarry bench room or add marker item | Could be marker-only in first pass. |
| Open A Salt Gallery | create salt mine room or marker item | For rock salt deposits. |
| Cut A Peat Field | create or reveal a peat cutting marker | Could remain marker-only. |

Because room creation requires overlay-package and zone context, the first stock package should not assume a universal package exists. Provide example progs and documentation, but keep room creation as optional builder configuration unless the seeder can safely create a stock package and template.

## Craft Catalogue

### Prospecting And Sample Crafts

| Craft | Category | Skill | Inputs | Tools | Products |
| --- | --- | --- | --- | --- | --- |
| chip mineral sample | Primary Production / Prospecting | Mining or Labouring | visible deposit prop or ore commodity if supported | prospecting hammer | sample ore commodity |
| pan placer gravel | Primary Production / Prospecting | Mining or Survival | gold-bearing gravel commodity | gold pan, water | sorted gold-bearing concentrate |
| assay iron sample | Primary Production / Prospecting | Mining or Smelting | sample iron ore | hammer, small hearth optional | identified sample or refined sample |
| assay copper sample | Primary Production / Prospecting | Mining or Smelting | sample copper ore | hammer, small hearth optional | identified sample or refined sample |

If crafts cannot target static props as inputs cleanly, keep sample extraction as project output rather than a craft.

### Ore Preparation Crafts

| Craft | Category | Skill | Inputs | Tools | Products |
| --- | --- | --- | --- | --- | --- |
| break raw iron ore | Primary Production / Ore Preparation | Labouring or Mining | raw iron ore commodity | hand hammer or sledgehammer | broken iron ore commodity |
| sort broken iron ore | Primary Production / Ore Preparation | Mining or Labouring | broken iron ore commodity | ore basket optional | sorted iron ore commodity, tailings |
| wash sorted iron ore | Primary Production / Ore Preparation | Mining or Labouring | sorted iron ore commodity, water access | trough/basket optional | washed iron ore commodity, tailings |
| roast washed iron ore | Primary Production / Ore Preparation | Smelting | washed iron ore commodity, charcoal | hearth/furnace tool | roasted iron ore commodity |

Repeat the same pattern for copper, tin, lead, and silver where appropriate. Use a helper method to avoid large copy-paste blocks.

### Smelting And Metal Stock Crafts

| Craft | Category | Skill | Inputs | Tools | Products |
| --- | --- | --- | --- | --- | --- |
| consolidate iron bloom | Primary Production / Ironworking | Blacksmithing or Smelting | iron bloom, charcoal | anvil, hammer, tongs | wrought iron billet, slag |
| draw wrought iron bar stock | Primary Production / Ironworking | Blacksmithing | wrought iron billet, charcoal | forge tools | wrought iron bar stock |
| cast copper ingot | Primary Production / Non-Ferrous | Smelting | copper metal/matte, charcoal | crucible, tongs, mould | copper ingot commodity |
| cast tin ingot | Primary Production / Non-Ferrous | Smelting | tin metal, charcoal | crucible, mould | tin ingot commodity |
| alloy bronze billet | Primary Production / Alloying | Smelting or Blacksmithing | copper ingot, tin ingot, charcoal | crucible, mould | bronze billet commodity |
| alloy brass billet | Primary Production / Alloying | Smelting | copper ingot, zinc ore/ingot, charcoal | crucible, mould | brass billet commodity |

### Stone And Binder Crafts

| Craft | Category | Skill | Inputs | Tools | Products |
| --- | --- | --- | --- | --- | --- |
| dress rough limestone block | Primary Production / Masonry | Masonry | rough limestone block | mason's hammer, chisel | dressed limestone block, rubble |
| dress rough sandstone block | Primary Production / Masonry | Masonry | rough sandstone block | mason's hammer, chisel | dressed sandstone block, rubble |
| break rubble into aggregate | Primary Production / Masonry | Labouring or Masonry | stone rubble | sledgehammer | aggregate |
| slake quicklime | Primary Production / Binders | Masonry | quicklime, water | trough/shovel optional | slaked lime |
| mix lime mortar | Primary Production / Binders | Masonry | slaked lime, sand, water | mortar trough, shovel | lime mortar |
| calcine gypsum plaster | Primary Production / Binders | Masonry | gypsum, fuel | kiln/hearth | plaster |

### Clay, Brick, Tile, Refractory, And Glass Crafts

| Craft | Category | Skill | Inputs | Tools | Products |
| --- | --- | --- | --- | --- | --- |
| temper clay body | Primary Production / Clay | Pottery | clay, sand/straw/grog, water | trough | prepared clay |
| mould green bricks | Primary Production / Brickmaking | Pottery or Labouring | prepared clay | brick mould | green bricks |
| mould roof tiles | Primary Production / Tilemaking | Pottery | prepared clay | tile mould | green roof tiles |
| prepare refractory clay | Primary Production / Refractory | Pottery or Smelting | fireclay, grog, water | trough | refractory clay body |
| mould clay crucibles | Primary Production / Refractory | Pottery or Smelting | refractory clay body | crucible mould | clay crucibles or crucible stock |
| prepare soda-lime glass batch | Primary Production / Glass | Glassworking | sand, soda ash/potash, lime | mixing trough | glass batch |

### Salt, Alkali, Tar, Peat, And Pigment Crafts

| Craft | Category | Skill | Inputs | Tools | Products |
| --- | --- | --- | --- | --- | --- |
| crush rock salt | Primary Production / Salt | Labouring | rock salt | hammer or millstone | salt commodity |
| leach wood ash | Primary Production / Alkali | Labouring or Survival | wood ash, water | ash hopper, leaching tub | lye or alkali stock |
| evaporate lye to potash | Primary Production / Alkali | Labouring | lye/alkali stock, fuel | evaporating pan | potash commodity |
| boil tar into pitch | Primary Production / Tar And Pitch | Labouring or Survival | pine tar, fuel | pitch kettle | pitch commodity |
| prepare pitch sealant | Primary Production / Tar And Pitch | Labouring | pitch, fibre/ash optional | kettle or mixing tool | prepared pitch commodity |
| grind ochre pigment | Primary Production / Pigment | Painting or Labouring | ochre stock | grinding slab | fine pigment commodity |
| prepare malachite pigment | Primary Production / Pigment | Painting or Labouring | malachite ore | mortar, grinding slab | malachite pigment commodity |
| cut dried peat blocks | Primary Production / Peat | Labouring | dried peat | turf knife | peat fuel commodity |

### Tool Maintenance Crafts

| Craft | Category | Skill | Inputs | Tools | Products |
| --- | --- | --- | --- | --- | --- |
| sharpen mining pick | Primary Production / Tool Maintenance | Blacksmithing or Labouring | target pick with repair input | whetstone/grindstone | repaired pick condition |
| rehaft mining pick | Primary Production / Tool Maintenance | Carpentry or Blacksmithing | target pick, handle stock | hammer/wedge | repaired pick condition |
| sharpen stone chisel | Primary Production / Tool Maintenance | Blacksmithing or Masonry | target chisel | grindstone | repaired chisel condition |
| repair ore basket | Primary Production / Tool Maintenance | Basketry | target basket, basketry splint | awl optional | repaired basket condition |
| patch bellows leather | Primary Production / Tool Maintenance | Leatherworking | target bellows, leather panel | awl, needle | repaired bellows condition |
| replace shovel handle | Primary Production / Tool Maintenance | Carpentry | target shovel, handle stock | woodworking tools | repaired shovel condition |
| patch salt pan | Primary Production / Tool Maintenance | Blacksmithing or Tinkering | target salt pan, metal patch stock | hammer, rivets | repaired pan condition |
| repair tar kiln lining | Primary Production / Tool Maintenance | Pottery or Masonry | target kiln, clay/fireclay | trowel | repaired kiln condition |

If repair input support is awkward in the seeder helpers, defer these to a second pass but keep tool durability consumption in the production crafts/projects.

## Quantities And Balance

The following quantities are deliberately approximate. They are intended for gameplay and not exact industrial archaeology.

### Suggested Project Output Sizes

| Process | Input | Main Output | Byproduct |
| --- | --- | --- | --- |
| Prospecting | 2-4 project-hours | visible resource prop or 1-2 kg sample | no-discovery echo if none |
| Burn charcoal clamp | 300 kg wood | 75 kg charcoal | 10 kg ash/fines |
| Extract ore | 2-4 project-hours | 100-200 kg raw ore | 50-150 kg spoil |
| Quarry stone blocks | 4-8 project-hours | 250-500 kg rough stone | 100-300 kg rubble |
| Burn lime kiln | 200 kg limestone + 100 kg charcoal | 100 kg quicklime | 20 kg rubble/ash |
| Fire brick clamp | 250 kg green bricks + 100 kg fuel | 200 kg fired bricks | 25 kg wasters |
| Smelt iron bloom | 80 kg roasted ore + 60 kg charcoal + flux | 20 kg iron bloom | 25 kg slag |
| Smelt copper ore | 80 kg roasted ore + 50 kg charcoal + flux | 8-15 kg copper stock | 30 kg slag |
| Smelt tin ore | 50 kg washed ore + 25 kg charcoal | 5-10 kg tin stock | 15 kg slag |
| Boil brine | 100 kg brine equivalent + fuel | 5-15 kg salt | spent brine/ash optional |
| Burn tar kiln | 200 kg resinous wood | 20-40 kg pine tar | charcoal/ash byproduct |
| Cut peat | 4 project-hours | 150 kg wet peat | none |
| Dry peat | 150 kg wet peat | 75 kg dried peat | spoil/wastage optional |

### Suggested Craft Batch Sizes

| Craft | Input | Output |
| --- | --- | --- |
| chip sample | deposit prop or raw ore | 1 kg sample ore |
| break ore | 20 kg raw ore | 18 kg broken ore + 2 kg waste |
| sort ore | 20 kg broken ore | 12 kg sorted ore + 8 kg tailings |
| wash ore | 20 kg sorted ore | 16 kg washed ore + 4 kg tailings |
| roast ore | 20 kg washed ore + 2 kg charcoal | 18 kg roasted ore |
| consolidate bloom | 10 kg iron bloom + 2 kg charcoal | 6 kg wrought iron billet + 3 kg slag |
| dress stone block | 100 kg rough block | 80 kg dressed block + 20 kg rubble |
| slake quicklime | 20 kg quicklime + water | 25 kg slaked lime |
| mix mortar | 10 kg slaked lime + 30 kg sand + water | 40 kg lime mortar |
| temper clay | 30 kg clay + temper + water | 35 kg prepared clay |
| mould bricks | 35 kg prepared clay | 30 kg green brick stock |
| leach ash | 20 kg ash + water | 10 kg lye/alkali stock |
| evaporate potash | 10 kg alkali stock + fuel | 2-4 kg potash |
| boil tar to pitch | 20 kg pine tar + fuel | 12-15 kg pitch |
| grind pigment | 5 kg pigment stone | 4 kg pigment commodity |

## Player-Facing Workflows

### Example: Prospecting And Iron Production

```text
1. A builder marks a hillside room with `Hematite Resource` or places a hidden hematite resource marker.
2. Prospectors run `Primary Production - Survey Mineral Signs`.
3. Prospectors run `Primary Production - Prospect for Iron Deposits`.
4. The project reveals `hematite surface deposits` and may produce a small sample.
5. Miners run `Primary Production - Open a Shallow Mine Working` or `Sink a Shaft for Hematite`.
6. Miners work a local extraction project and produce raw iron ore.
7. Labourers break, sort, and wash the raw ore through short crafts.
8. A smelter roasts the ore if the ore family requires it.
9. A smelting crew runs a bloomery project with roasted ore, charcoal, and flux.
10. The project produces iron bloom and slag commodities.
11. A smith consolidates bloom into wrought iron billet.
12. A smith draws billets into bar stock, weapon stock, tool stock, or armour stock.
13. Existing finished-good crafts consume the stock.
```

### Example: Stone Building Material

```text
1. Prospectors find a limestone outcrop.
2. Quarry workers clear the quarry face.
3. Quarry workers run a local quarry project and produce rough limestone blocks and rubble.
4. Masons dress rough blocks into dressed block commodities.
5. Labourers break rubble into aggregate.
6. Kiln workers burn limestone into quicklime.
7. Masons slake quicklime and mix mortar.
8. Construction projects consume dressed blocks, aggregate, and mortar.
```

### Example: Salt And Preservation

```text
1. Prospectors discover a brine spring or rock salt deposit.
2. Salt workers operate a pan, boiling house, or salt gallery project.
3. The project produces salt as a commodity.
4. Cooks, butchers, tanners, doctors, traders, or alchemists consume the salt in downstream crafts.
```

### Example: Tar, Pitch, And Waterproofing

```text
1. Woodland workers produce resinous wood, resin, or pine roots.
2. Tar burners run a local tar kiln project.
3. The project produces pine tar.
4. A short craft boils tar into pitch.
5. Pitch feeds shipbuilding, waterproof containers, roofing, torches, road work, and repair crafts.
```

## Implementation Progress

### Phase 0 Audit - Completed 2026-06-18

The initial audit found that the first implementation can reuse several existing foundations rather than inventing parallel content:

- `CoreDataSeeder.Materials.cs` already seeds core ores and industrial feedstocks including hematite, magnetite, cassiterite, galena, malachite, native copper, limestone, clay, fire clay, peat, salt, brick, glass, mortar, pitch, tar, charcoal, lye, slaked lime, calcium oxide, calcium hydroxide, sulfur, wrought iron, and sponge iron.
- `UsefulSeeder.Tags.cs` already supplies broad stock and tool roots including `Material Functions`, `Ore Deposit`, `Hot Fire`, `Household Craft Stock`, `Prepared Pitch`, `Glass Batch`, `Tool Blank Stock`, `Tools`, `Hammer`, `Chisel`, `Wheelbarrow`, `Professional Tools`, `Construction Materials`, `Stone Blocks`, `Aggregate`, and `Lime`.
- `ICell` already implements `IHaveTags`, `Cell` already persists tags through `CellsTags`, and FutureProg `istagged` already supports `ProgVariableTypes.Location`. Active projects expose `project.location`, so prospecting discovery can use cell resource tags through ordinary `prog` project actions.
- The lowest-risk discovery model for this pass is therefore cell resource tags plus visible/non-holdable deposit props. A native `resourcediscovery` action is deferred until a real builder workflow gap appears.

Reusable dependencies are covered by `PrimaryProductionSeederSourceTests`, which intentionally checks source seams rather than generated database rows for this audit phase.

### Phase 1 Engine Support - Completed 2026-06-18

The project engine now has a first-class `commodityoutput` completion action:

- `CommodityOutputProjectAction` persists `MaterialId`, `Weight`, optional `TagId`, optional `UseIndirectDescription`, optional `Echo`, and fixed commodity `Characteristics` under the project action XML definition.
- `ProjectFactory.CreateAction`, `ProjectFactory.LoadAction`, and `ProjectFactory.ValidActionTypes` register `commodityoutput` alongside the existing `prog`, `skilluse`, and `agriculture` actions.
- Completion creates the pile through `CommodityGameItemComponentProto.CreateNewCommodity(...)`, adds it to the gameworld, places it in the active project's concrete location when available, falls back to the owner location for personal/interface-only cases, chooses the first active worker's room layer in that cell, and emits the configured or generic local echo.
- Builder commands are implemented for `material`, `weight`, `tag`, `indirect`, `echo`, and fixed output `characteristic` pairs.
- `CanSubmit` validates missing/stale materials, non-positive mass, and stale configured tags.
- `Projects_System_Overview.md` and `Projects_System_Builder_Workflows.md` now list the new action and its builder commands.

The Phase 0 discovery decision still holds: cell tags plus ordinary `prog` project actions are sufficient for first-pass prospecting, so no native `resourcediscovery` action was added in this phase.

Phase 1 behaviour is covered by `CommodityOutputProjectActionTests`, including action registration, existing action type loading, XML round-trip, validation, and actual commodity creation through the commodity factory.

### Phase 2 Tags And Materials - Completed 2026-06-18

The foundational tag and material layer is now seeded:

- `UsefulSeeder.Tags.cs` now adds the primary-production material-function hierarchy, resource-site tags, commodity process-state tags, and functional tool-family tags under existing roots.
- `CoreDataSeeder.Materials.cs` now also creates the primary-production material-function roots during base material seeding because `CoreDataSeeder` applies material tags before `UsefulSeeder` runs. This keeps material tagging self-contained and lets `UsefulSeeder` reuse the same names later without duplication.
- Existing canonical material names are retained where they already existed. Common ore, historical, and builder-friendly names such as `hematite ore`, `haematite`, `tin ore`, `lead ore`, `rock salt`, `quicklime`, `fireclay`, `pine tar`, and `nitre` are aliases rather than duplicate materials.
- New process-state and missing feedstock materials cover limonite ore, bog iron ore, iron bloom, wrought iron billet, slag, dried peat, coal, coke, stone rubble, gravel, prepared clay, green brick, fired brick, roof tile, glass batch, glass blank, potash, natron, bitumen, and mineral pigments.

The main design decision in this phase was seeding-order related rather than domain related: primary-production tag roots must exist before materials are tagged, so the core material seeder owns the minimum base hierarchy while the useful tag seeder owns the broader reusable catalogue. `PrimaryProductionSeederSourceTests` now verifies both source paths.

### Phase 3 Tools, Apparatus, And Resource Props - Completed 2026-06-18

The reusable item layer is now seeded through `ItemSeeder.Rework.PrimaryProductionTools.cs` and is wired into the shared historic item install path for antiquity and medieval selections:

- Portable durable tools now cover prospecting, surveying, mining, quarrying, masonry, hauling, charcoal burning, kiln work, smelting, saltworking, alkali work, tar/pitch work, peat cutting, and pigment processing.
- Carrying aids now include sample bags, ore baskets, ore sacks, a hand-barrow, a wheelbarrow, rope, timber props, and process containers such as the ash hopper.
- Visible resource deposit props now cover hematite, limonite, cassiterite, malachite, galena, native copper, gold-bearing gravel, limestone, slate, clay, brine, peat, natron, ochre, sulfur, and bitumen.
- Static apparatus prototypes now cover a bloomery furnace, lime kiln, brick clamp, charcoal clamp site, salt pan, tar kiln, mine windlass, furnace bellows, glass furnace, and peat drying rack.
- Portable tools reuse existing `Holdable`, `Destroyable_*`, and `Container_*` component prototypes. Visible deposits and static apparatus intentionally have no `Holdable` component; visible deposits also omit `Destroyable_*`, making them fixed/non-gettable first-pass site markers without adding a new scenery component.

The main design decision in this phase was to avoid adding a new fixture/scenery component until the runtime needs one. The existing item model can already represent non-gettable props by omitting `Holdable`, so Phase 3 uses that lower-risk pattern and records the convention in each stock item's builder notes. `PrimaryProductionSeederSourceTests` now verifies stable references, component choices, functional tags, resource tags, and the dispatcher hook.

### Phase 4 Commodity Crafts - Completed 2026-06-18

The reusable craft layer is now seeded through `ItemSeederCrafting.PrimaryProduction.cs` and is wired immediately after `SeedHistoricFoundationCrafts()` in the shared craft seeder:

- The catalogue adds deterministic `primary production - ...` craft names under `Primary Production / ...` categories and gates them behind `Primary Production - Historic Commodity Work` knowledge.
- Ore-preparation crafts now cover sample assay plus break, sort, wash, and roast chains for iron, copper, tin, and lead ores.
- Masonry and binder crafts now cover dressed limestone and sandstone blocks, aggregate, slaked lime, lime mortar, and gypsum plaster.
- Clay, brick, tile, refractory, and glass batch crafts now cover prepared clay, green bricks, roof tile stock, crucible clay stock, and soda-lime glass batch.
- Metal stock crafts now cover iron bloom consolidation, wrought iron bar stock, copper ingots, tin ingots, and bronze billet stock.
- Salt, alkali, tar/pitch, peat, and pigment crafts now cover crushed salt, lye, potash, pitch, prepared pitch sealant, ochre pigment, malachite pigment, and peat fuel blocks.
- Craft imports use exact material commodities plus process-stage pile tags. For example, `hematite` remains the material while `Raw Ore Commodity`, `Washed Ore Commodity`, and `Roasted Ore Commodity` describe the process state.

Two design decisions came up during review. First, visible resource props are not used as direct craft inputs because consuming a marker prop would remove the discovered site. Sample extraction from a deposit remains a Phase 5 project responsibility where a project can output sample commodities without destroying the marker. Second, the stock skill alias list does not yet include dedicated `Mining`, `Saltworking`, or `Pigment Processing` traits, so the first-pass craft gates use stable existing traits such as `Labouring`, `Surviving`, `Masonry`, `Pottery`, `Smelting`, `Blacksmithing`, `Glassworking`, and `Painting`; functional tool tags carry the more specific domain requirements.

Phase 4 is covered by `PrimaryProductionSeederSourceTests`, which verifies the craft dispatcher hook, deterministic names, representative chain coverage, commodity tags, functional tool tags, and safe trait gates. A targeted `DatabaseSeeder` build and filtered `PrimaryProductionSeederSourceTests` pass both succeeded after the phase.

### Phase 5 Local Projects - Completed 2026-06-18

The local project layer is now seeded through a dedicated `PrimaryProductionSeeder` package rather than another `ItemSeeder` partial:

- `PrimaryProductionSeeder` is an idempotent rerunnable seeder with sort order `420`, after the item/craft package that provides visible resource props and apparatus prototypes.
- Stock project templates use deterministic names under the `Stock Primary Production: ` prefix and are tracked as seeder-owned stock templates.
- Prospecting templates now cover broad mineral signs, iron, tin, copper, lead/silver, quarry stone, clay, salt/brine, fuel, alkali, pigment earth, sulfur, and bitumen seeps.
- Production templates now cover charcoal burning, ore extraction, quarrying, lime burning, brick and tile firing, iron/copper/tin smelting, brine salt, natron collection, glass-furnace batch firing, tar burning, peat cutting, ochre collection, bitumen collection, and coal mining.
- Bulk consumed inputs use `commodity` or `commoditytag` project material requirements. Durable tools and apparatus are described in the labour/project text but are not consumed as `simple` material requirements because the current material requirement model deletes supplied items.
- Completion outputs use `commodityoutput` actions for commodity products and byproducts such as raw ores, mine spoil, rough blocks, quicklime, fired bricks, metal intermediates, slag, salt, tar, peat fuel, pigments, and coal.

This phase included a small design review correction. The earlier assumption that ordinary FutureProgs were enough for visible resource discovery did not hold cleanly once stock project templates needed duplicate-safe prop creation. The engine now includes a native `resourcediscovery` project action. It checks an optional required location tag, avoids duplicates by configured marker tag or prototype, creates the configured visible marker prototype in the project cell, and emits configurable success/already-present/failure echoes. `ProjectFactory` registers `resourcediscovery` for builder and seeded-template use.

Phase 5 is covered by `ResourceDiscoveryProjectActionTests`, the earlier `CommodityOutputProjectActionTests`, and `PrimaryProductionSeederSourceTests`. The targeted core project-action test filter, targeted `DatabaseSeeder` build, and filtered primary-production seeder tests all passed after the phase.

### Phase 6 Optional Site-Creation Progs - Completed 2026-06-18

Site-creation remains a builder-configured optional workflow rather than seeded stock content:

- The engine already has the necessary FutureProg functions for builder-authored topology changes: `CreateOverlay`, `ReviseOverlay`, `CreateCell`/`CreateRoom`, `NameCell`/`NameRoom`, `DescribeCell`/`DescribeRoom`, `SetIndoorsNoLight`, `SetTerrain`, `LinkCells`, and `ApproveOverlay`.
- A project `prog` action can execute a FutureProg that accepts a single `project` parameter, so builders can attach a local-project completion prog that reads `project.location`, creates a new cell in a chosen editable overlay package, links it to the project location, and optionally approves the package.
- The primary-production seeder does not seed these FutureProgs or site-creation projects because it cannot safely infer the builder account, overlay package/revision policy, zone, template room, direction, terrain, naming convention, or approval policy for a host game's world.
- Builders who want mine-adit or quarry-extension projects should clone a stock extraction project, add a `prog` action after its commodity outputs, and bind that action to a world-specific completion prog.

This was a design-review phase rather than a runtime expansion. Seeding a generic topology-changing project would be materially worse for correctness and maintainability because it would hard-code worldbuilding choices in a setting-neutral content package. The builder workflow is now documented in `Projects_System_Builder_Workflows.md`, and `PrimaryProductionSeederSourceTests` guards the decision that the stock package documents this path without seeding unsafe topology modifiers.

### Phase 7 Documentation And Integration - Completed 2026-06-18

The final implemented names and integration points are now documented:

- Engine action keywords: `commodityoutput` and `resourcediscovery`.
- Seeder package: `PrimaryProductionSeeder`, sort order `420`, package name `Primary Production`, stock project prefix `Stock Primary Production: `, metadata mode `Idempotent` / `RepairExisting`.
- Craft knowledge: `Primary Production - Historic Commodity Work`.
- Craft categories: `Primary Production / Prospecting`, `Ore Preparation`, `Masonry`, `Binders`, `Clay`, `Brickmaking`, `Tilemaking`, `Refractory`, `Glass`, `Ironworking`, `Non-Ferrous`, `Alloying`, `Salt`, `Alkali`, `Tar And Pitch`, `Pigment`, and `Peat`.
- Project action testing surfaces: `CommodityOutputProjectActionTests`, `ResourceDiscoveryProjectActionTests`, and `PrimaryProductionSeederSourceTests`.
- Builder workflow docs: `Projects_System_Overview.md` and `Projects_System_Builder_Workflows.md` now list `commodityoutput`, `resourcediscovery`, and optional site-creation completion progs.
- Repeatability docs: `DatabaseSeeder_Repeatability_Strategy.md` now lists `PrimaryProductionSeeder` as a repair-capable stock project package.
- Final review aligned `PrimaryProductionSeeder` metadata prerequisites with the seeder's labour-trait alias handling so installer assessment and runtime installation agree.

Integration notes:

- Agriculture woodland outputs can feed the primary-production chains. Stock agriculture already includes resin/gum, fibre/cane, thatch, parkland, shelterbelt, fuelwood, and charcoal woodland examples; primary production consumes fuel wood, charcoal, resinous wood/tarwood, wood ash, tar, pitch, potash, and related commodities for charcoal, alkali, tar/pitch, glass, lime, brick, and smelting workflows.
- The downstream antiquity and medieval content packages should treat primary-production outputs as reusable upstream stock rather than reseeding parallel materials. Useful examples include wrought iron billets, metal ingots, bronze billet stock, quicklime, slaked lime, mortar, glass batch, fired brick, roof tile stock, pitch, potash, salt, pigments, coal, and peat fuel.
- Future world-starter packages should place resource-site tags on real cells, then optionally place visible resource props where the world begins with known deposits. Recommended starter tags include ore resources, quarry stone, clay pit, brine spring, peat bog, natron, ochre, sulfur, bitumen, and coal.

PR notes:

- Complete: core project actions, primary-production tag/material foundations, tools/resource props/static apparatus, commodity crafts, rerunnable project templates, metadata, and builder documentation.
- Complete by design, not seeded: optional topology-changing site-creation progs. Builders can attach them to local projects with a `prog` action when their world has concrete overlay, zone, template-room, direction, and approval choices.
- Future work: richer live seeder integration tests against a seeded database, dedicated mining/saltworking/pigment traits if the skill package grows them, broader ore/resource varieties, and a fixture/scenery component only if the item runtime develops a stronger need than non-holdable stock props.

## Implementation Plan

### Phase 0: Audit Existing Content

Status: completed 2026-06-18. See "Phase 0 Audit" above and `PrimaryProductionSeederSourceTests`.

1. Search existing materials for ores, charcoal, coal, stone, quicklime, lime, mortar, clay, brick, glass batch, slag, ingots, billets, salt, potash, lye, tar, pitch, peat, pigments, sulfur, and stock tags.
2. Search existing tags for ore, fuel, textile stock, household craft stock, military stock, construction materials, tools, hot fire tags, mining tags, quarrying tags, and cell/room tag support.
3. Search existing item prototypes for furnaces, kilns, anvils, bellows, hammers, picks, shovels, baskets, barrows, ropes, masonry tools, salt pans, tar kilns, ash hoppers, and pigment tools.
4. Search for existing support for tags on rooms/cells/locations. If absent, decide between prop-based resource markers and a small cell tag feature.
5. Record what can be reused and what must be added.
6. Do not duplicate materials or tags. Prefer adding aliases and tags to existing materials.

Deliverable: a short audit section in the PR description and tests that document reused dependencies.

### Phase 1: Engine Support

Status: completed 2026-06-18. See "Phase 1 Engine Support" above and `CommodityOutputProjectActionTests`.

1. Add `CommodityOutputProjectAction` under `MudSharpCore/Work/Projects/Actions`.
2. Register `commodityoutput` in `ProjectFactory.CreateAction`, `ProjectFactory.LoadAction`, and `ProjectFactory.ValidActionTypes`.
3. Add builder commands and validation.
4. Add completion behaviour to create and place the commodity pile.
5. Add tests for action registration, XML persistence, validation, and completion output.
6. Update project system documentation to include the new action.
7. Audit whether room/cell tags exist. If they exist, document how prospecting progs/actions should inspect them. If they do not exist, choose one:
   - add minimal room/cell tag support, or
   - implement resource discovery via hidden/non-holdable marker props.
8. If FutureProg cannot cleanly reveal resource tags/props, add a small `resourcediscovery` project action as described above.

### Phase 2: Tags And Materials

Status: completed 2026-06-18. See "Phase 2 Tags And Materials" above and `PrimaryProductionSeederSourceTests`.

1. Add primary-production material-function tags to `UsefulSeeder.Tags.cs` or the appropriate tag seeding partial.
2. Add resource-site tags for mineral, stone, clay, salt, fuel, alkali, pigment, and visible deposits.
3. Add commodity stage tags.
4. Add tool tags for prospecting, mining, quarrying, masonry, kiln, smelting, charcoal burning, hauling, saltworking, alkali work, tar burning, peat cutting, and pigment processing.
5. Add or update materials in `CoreDataSeeder.Materials.cs` or the appropriate material package.
6. Add aliases for common spellings and historical names.
7. Add unit tests verifying all tags and materials exist and have expected parent/tag relationships.

### Phase 3: Tools, Apparatus, And Resource Props

Status: completed 2026-06-18. See "Phase 3 Tools, Apparatus, And Resource Props" above and `PrimaryProductionSeederSourceTests`.

1. Add durable item prototypes for priority prospecting, mining, quarrying, masonry, hauling, kiln, charcoal, smelting, salt, alkali, tar, peat, and pigment tools.
2. Add visible deposit prop prototypes for hematite, limonite, cassiterite, malachite, galena, gold-bearing gravel, limestone, slate, clay, brine, peat, natron, ochre, sulfur, and bitumen.
3. Ensure visible deposit props are non-holdable and indestructible. If the item system needs a fixture/scenery component, add or reuse one.
4. Ensure tools carry functional tags so crafts and projects can require tool families rather than exact prototypes where possible.
5. Ensure durable tools have destroyable/tool components appropriate to existing item authoring practice.
6. Add containers or carrying aids for ore baskets, sample bags, sacks, pans, and barrows where existing components support it.
7. Add static apparatus prototypes for room tools or fixtures such as lime kilns, bloomery furnaces, charcoal clamp sites, salt pans, tar kilns, windlasses, and large bellows.
8. Add tests verifying prototypes resolve by stable reference and carry expected tags/components.

### Phase 4: Commodity Crafts

Status: completed 2026-06-18. See "Phase 4 Commodity Crafts" above and `PrimaryProductionSeederSourceTests`.

1. Add helper methods for commodity-input and commodity-product craft definitions if not already convenient.
2. Seed prospecting/sample crafts where item/prop targeting is practical.
3. Seed ore preparation crafts for iron, copper, tin, and lead.
4. Seed binder crafts for quicklime, slaked lime, mortar, and plaster.
5. Seed clay, brick, tile, refractory, and glass batch preparation crafts.
6. Seed stone dressing and aggregate crafts.
7. Seed bloom consolidation and basic metal stock crafts.
8. Seed salt, alkali, tar/pitch, peat, and pigment crafts.
9. Add craft tests for representative chains.

### Phase 5: Local Projects

Status: completed 2026-06-18. See "Phase 5 Local Projects" above, `ResourceDiscoveryProjectActionTests`, and `PrimaryProductionSeederSourceTests`.

1. Add local project seed helpers if there is no reusable project-seeding helper outside AgricultureSeeder.
2. Seed prospecting projects for mineral signs, iron, tin, copper, lead/silver, quarry stone, clay, salt/brine, fuel, alkali, pigment, and sulfur resources.
3. Seed priority local projects for charcoal, ore extraction, quarrying, lime burning, brick firing, smelting, salt boiling, tar burning, peat cutting, and pigment collection.
4. Use commodity material requirements for bulk inputs.
5. Use `commodityoutput` actions for outputs and byproducts.
6. Use `prog` or `resourcediscovery` actions for prospecting discovery effects.
7. Use `simple` labour for ordinary work and `supervision` labour for foremen/master workers.
8. Use `skilluse` actions sparingly where the project should grant learning at completion.
9. Add project tests verifying seeded project definitions load, validate, submit, and expose expected phases, materials, labour roles, and actions.

### Phase 6: Optional Site-Creation Progs

Status: completed 2026-06-18. See "Phase 6 Optional Site-Creation Progs" above and `PrimaryProductionSeederSourceTests`.

1. Add example FutureProgs for creating a mine chamber from a template room if the repository has a safe pattern for seeding such progs.
2. Keep site-creation projects optional unless the seeder can reliably determine overlay package, zone, and template room.
3. Document the builder workflow for attaching a completion prog to a local project.
4. Do not block the first primary-production package on this feature.

### Phase 7: Documentation And Integration

Status: completed 2026-06-18. See "Phase 7 Documentation And Integration" above and `PrimaryProductionSeederSourceTests`.

1. Update this design reference with any final implemented names.
2. Add a builder workflow section or separate guide if project/craft setup commands are important.
3. Cross-reference agriculture woodland outputs and downstream medieval/antiquity crafting packages.
4. Add PR notes explaining what is complete and what is left for future work.
5. Add seeder metadata for `PrimaryProductionSeeder` if it becomes a standalone seeder.
6. Add a future world-starter note for placing example resource sites.

## Tests And Acceptance Criteria

### Engine Acceptance

- `commodityoutput` appears in valid action types.
- A builder can create, configure, show, submit, and reload a commodity output action.
- A completed local project creates the correct commodity in the project cell.
- A completed personal project creates the correct commodity in the owner location or a documented fallback location.
- Invalid action definitions fail submit validation.
- Prospecting can check room/cell tags or marker props through either helper progs or a native resource discovery action.
- Prospecting does not create duplicate visible resource props unless explicitly configured.

### Data Acceptance

- All new tags seed idempotently and have expected parents.
- All new materials seed idempotently, have expected aliases, and carry expected material tags.
- All new item prototypes resolve by stable reference.
- All new visible deposit props resolve by stable reference and are non-holdable/indestructible or equivalent.
- All new crafts resolve their materials, tags, tools, products, knowledge, and traits.
- All new projects resolve their materials, tags, labour traits, actions, and phase requirements.
- At least one full prospecting chain can be validated from hidden resource marker to visible deposit prop.
- At least one full iron chain can be validated from prospecting to wrought iron billet.
- At least one full stone/lime chain can be validated from quarry discovery to lime mortar.
- At least one full clay/brick chain can be validated from clay discovery to fired brick.
- At least one non-original industry chain, such as salt, tar/pitch, potash, peat, or pigment, can be validated end-to-end.

### Gameplay Acceptance

A fresh seeded game should allow builders to set up a basic production settlement where:

1. Builders can mark rooms as containing hidden resources.
2. Prospectors can discover those resources and create visible deposit props.
3. Woodlands or timber stock feed charcoal, potash, and tar production.
4. Charcoal feeds lime, brick, glass, and smelting work.
5. Mines create raw ores as commodities.
6. Ore preparation crafts create smelt-ready ore.
7. Smelting projects create metal intermediates.
8. Smithing and other downstream crafts can consume metal stock.
9. Quarries create rough stone and rubble.
10. Masons and builders can transform quarry outputs into dressed stone, aggregate, lime, and mortar.
11. Salt, tar, alkali, peat, and pigment chains create useful non-metal industries.
12. Tools wear, break, and need maintenance or replacement.

## Future Work

### Resource Sites And Depletion

Introduce a `PrimaryProductionSite` or generic `ResourceSite` model attached to a cell. It could track:

- resource type
- remaining mass or abstract depletion score
- richness/grade
- work difficulty
- stability
- water ingress
- ventilation
- overburden
- discovered state
- legal owner or claim
- permitted project templates

Extraction projects would then consume site potential rather than being indefinitely repeatable.

### Hazards

Add project impacts or completion effects for:

- cave-ins
- bad air
- flooding
- burns
- lime burns
- smoke inhalation
- tool injury
- furnace explosions
- exhaustion and heat stress
- salt pan scalds
- tar burns
- alkali chemical burns

This should be built after the basic economy works, because hazards need careful balance and good player feedback.

### Industrial Era Extension

Add later chains:

- coal mining
- coke ovens
- blast furnaces
- pig iron
- finery forge
- puddling furnace
- rolling mill
- cement kiln
- Portland cement
- concrete aggregate
- steam-powered pumps and hoists
- powered crushers and stamps
- chemical alkali processes
- sulfuric acid chambers

These should be tagged by era and should not replace the preindustrial baseline.

### Construction System Integration

Once construction projects are ready, consume these commodities directly:

- rough logs, beams, planks, laths
- dressed stone blocks
- rubble and aggregate
- lime mortar
- fired bricks
- roof tiles
- plaster
- glass blanks or panes
- iron nails, clamps, straps, hinges
- pitch, tar, bitumen, and sealants
- salt and alkali where relevant to preservation or specialist works

### Market And Employment Integration

Add economy seeder entries or job templates for:

- prospector
- miner
- quarryman
- charcoal burner
- lime burner
- brickmaker
- smelter
- furnace tender
- ore washer
- mason
- salt worker
- tar burner
- peat cutter
- pigment grinder
- haulier
- mine foreman
- quarry master

Where ongoing job integration uses personal projects, keep the actual production projects local and physical wherever possible.

### Pre-Built World Seeder

If FutureMUD wants a stronger out-of-the-box world, add a separate pre-built world/scenario seeder that depends on primary production but is not the same package. It could place:

- a sample iron outcrop
- a sample clay bank
- a sample limestone quarry
- a sample woodland/charcoal site
- a sample brine spring or salt pan
- a sample peat bog
- a sample workshop district
- market buyers and sellers for key commodities

This should remain optional, because resource placement is world design rather than general content design.

## Implementation Notes For Codex Agents

- Do not bypass commodities with manually loaded item prototypes.
- Do not make extraction crafts that instantly produce finished goods.
- Do not duplicate existing materials, tags, tools, furnaces, or stock commodities.
- Prefer exact material reuse plus commodity pile tags over creating many near-duplicate materials.
- Keep project outputs as bulk material commodities, not individual finished items.
- Keep prospecting discovery separate from extraction output.
- Use visible resource props as player-facing anchors even if room tags store the hidden truth.
- Keep first-pass projects deterministic and easy to test.
- Use multiple project phases to create handoff, supervision, and material-supply opportunities.
- Add tests before expanding the catalogue too widely.
- Preserve existing seed repeatability and idempotency patterns.
- Keep player-facing descriptions concrete and physical.
