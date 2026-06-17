# FutureMUD Primary Production Seeder Design Reference

## Purpose

This document defines a seeded-content package for primary production chains outside the existing agriculture, hunting, and finished-goods craft packages. The target systems are mining, quarrying, charcoal burning, metallurgical production, building-material production, and related intermediate processing.

The package is intended to make the economy move through bulk commodities, durable tools, repair work, and multi-stage handoffs. It should not simply add finished metal weapons, armour, furniture, or buildings. It should create the upstream economic matter that other characters, crafts, shops, projects, and settlements consume.

The design assumes the current FutureMUD distinction between:

- **Crafts**: short actions performed by one character over seconds or minutes.
- **Projects**: larger work, group work, cell-bound work, or work that should persist and advance over project ticks.
- **Commodities**: bulk material piles that are the preferred handoff unit between production stages.
- **Items**: durable tools, apparatus, fixtures, containers, finished goods, or special objects.

## Design Goals

1. Use commodities as the default output for raw and intermediate goods.
2. Create many handoff points where miners, haulers, charcoal burners, smelters, masons, carpenters, smiths, potters, glassworkers, and builders can each contribute part of the chain.
3. Use local projects for extraction, firing, smelting, quarrying, shaft work, and other multi-person or long-running work.
4. Use crafts for short bench-scale transformations, tool maintenance, sorting, trimming, mixing, dressing, and stock preparation.
5. Add enough durable tools and apparatus to create demand for repair, replacement, sharpening, hauling, and workshop infrastructure.
6. Keep the first implementation concrete and data-driven rather than implementing a full geology, depletion, hazard, or industrial plant simulation.
7. Preserve era flexibility by seeding a preindustrial baseline first, with later industrial extensions clearly separated.

## Non-Goals For The First Pass

The first implementation should not attempt to build all of the following systems:

- Full resource-node depletion, vein geometry, ore grade simulation, or geological survey mechanics.
- Automated mine hazards such as collapses, bad air, flooding, dust explosion, or heat stroke.
- A complete construction system for buildings, fortifications, bridges, roads, canals, and mines.
- A full industrial revolution plant model with steam engines, coke blast furnaces, rolling mills, puddling furnaces, Bessemer converters, or powered machinery.
- Automatic world generation of ore deposits or quarry sites.
- Complex environmental impacts such as tailings pollution, forestry depletion, or watershed damage.

Those are good later extensions, but the first pass should create useful content with the current craft, project, material, tag, and commodity models.

## Current-System Fit

### Agriculture Boundary

Agriculture already covers fields, crops, herds, apiaries, and managed woodland. It also already uses projects for field operations and commodities for harvested products. Primary production should not duplicate that system. Instead:

- Managed woodlands can supply timber, poles, brushwood, bark, firewood, charcoal wood, resin, gum, and similar plant commodities.
- Primary production can consume those woodland commodities to produce charcoal, lime, fired brick, smelted metal, and other industrial feedstocks.
- Agriculture remains the system for growing trees and managing woodland yield.
- Primary production is the system for turning woodland yield into fuel and industrial materials.

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
- Repair a tool handle.
- Sharpen a pick or chisel.

Crafts already support commodity inputs and commodity products. This makes them ideal for intermediate transforms.

### Project Boundary

Projects should handle larger or persistent work. Examples:

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
- Dig a new chamber or connect a new room by completion prog.

Local projects are the default because the work belongs to a location. Personal projects are useful only for off-site planning, paperwork, or jobs that abstract a workplace rather than simulating the cell.

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

- `Primary Production Ore Commodity`
- `Raw Ore Commodity`
- `Roasted Ore Commodity`
- `Charcoal Fuel Commodity`
- `Rough Stone Block Commodity`
- `Quicklime Commodity`

If a tag will be used as a broad functional category, put it under a primary production root. If a tag will be used by crafts to distinguish a processing stage, put it under a commodity-stage root.

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

Use aliases for common variants and spellings, for example:

- `cassiterite` as alias for `tin ore` or separate material if desired.
- `galena` as alias for `lead ore` or separate material if desired.
- `haematite` as alias for `hematite ore`.
- `lime` as alias for `quicklime` only if the ambiguity is acceptable.

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
| Primary Production Waste | Primary Production | Slag, tailings, rubble, wasters, spoil. |

### Commodity Stage Tags

Add these under `Functions / Material Functions / Primary Production` or under a child such as `Primary Production Commodity`.

| Tag | Parent | Purpose |
| --- | --- | --- |
| Primary Production Commodity | Primary Production | Root for commodity-pile process states. |
| Raw Ore Commodity | Primary Production Commodity | Unprocessed ore from a mine face. |
| Broken Ore Commodity | Primary Production Commodity | Ore broken small enough for sorting or washing. |
| Sorted Ore Commodity | Primary Production Commodity | Hand-sorted ore. |
| Washed Ore Commodity | Primary Production Commodity | Washed ore ready for roasting or smelting. |
| Roasted Ore Commodity | Primary Production Commodity | Roasted ore ready for smelting. |
| Ore Tailings Commodity | Primary Production Commodity | Waste from washing and sorting. |
| Mine Spoil Commodity | Primary Production Commodity | Waste rock and spoil from mine projects. |
| Charcoal Fuel Commodity | Primary Production Commodity | Industrial charcoal output. |
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

### Tool And Apparatus Tags

Add these under `Functions / Tools` or a `Primary Production Tools` child.

| Tag | Parent | Purpose |
| --- | --- | --- |
| Mining Tool | Tools | Picks, gad, wedge, shovel, windlass. |
| Quarrying Tool | Tools | Stone hammers, chisels, wedges, lifting tools. |
| Masonry Tool | Tools | Dressing and measuring tools. |
| Charcoal Burning Tool | Tools | Rakes, shovels, clamp tools. |
| Kiln Tool | Tools | Kiln rake, firing shovel, tongs. |
| Smelting Tool | Tools | Bellows, tongs, crucibles, moulds, furnace tools. |
| Hauling Tool | Tools | Baskets, barrows, sledges, panniers. |
| Surveying Tool | Tools | Plumb bob, measuring cord, straightedge. |

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
| malachite ore | Primary Production Ore | malachite | Copper carbonate ore. |
| tin ore | Primary Production Ore | cassiterite, ore tin | Tin source for bronze. |
| lead ore | Primary Production Ore | galena, ore lead | Lead and silver-bearing ore. |
| silver ore | Primary Production Ore | ore silver | Generic silver ore. |
| gold ore | Primary Production Ore | ore gold | Generic gold ore. |
| zinc ore | Primary Production Ore | calamine, smithsonite | Optional brass chain support. |

### Fuels And Fluxes

| Material | Tags | Aliases | Notes |
| --- | --- | --- | --- |
| charcoal | Primary Production Fuel, Hot Fire | hardwood charcoal | Core preindustrial industrial fuel. |
| coal | Primary Production Fuel, Hot Fire | mineral coal, sea coal | Later medieval/industrial fuel. |
| coke | Primary Production Fuel, Hot Fire | coked coal | Industrial extension. |
| limestone flux | Primary Production Flux | flux, limestone | If exact limestone is already a material, consider using limestone + tag rather than adding this. |
| potash | Primary Production Flux, Primary Production Glass Stock | pearl ash | Glass and soap feedstock. |
| soda ash | Primary Production Flux, Primary Production Glass Stock | natron, soda | Glass and chemical feedstock. |

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
| prepared clay | Primary Production Clay | clay body | Tempered clay ready for forming. |
| green brick | Primary Production Clay | unfired brick | Unfired brick stock. |
| fired brick | Primary Production Stone, Primary Production Clay | brick | Fired masonry unit. |
| roof tile | Primary Production Stone, Primary Production Clay | tile | Fired roofing material. |
| glass batch | Primary Production Glass Stock | batch | Mixed glass raw feedstock. |
| glass blank | Primary Production Glass Stock | glass stock | Generic glassworking stock. |

## Tool And Apparatus Catalogue

The following tools should be seeded as durable item prototypes. Exact component choices should follow existing item authoring patterns for tools, holdables, wearables, containers, furniture, and destroyables.

### Mining And Hauling Tools

| Stable Reference | Sdesc | Tags | Notes |
| --- | --- | --- | --- |
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

### Charcoal, Kiln, And Furnace Tools

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

## Knowledge And Skill Coverage

Create one broad knowledge package and then specialised knowledge packages where existing content patterns require them.

| Knowledge | Purpose |
| --- | --- |
| Primary Production Fundamentals | Shared knowledge for basic extraction, hauling, fuel, and material prep. |
| Primary Mining | Mining projects and ore extraction. |
| Primary Quarrying | Quarrying and rough block production. |
| Primary Charcoal Burning | Charcoal clamp projects and fuel handling. |
| Primary Lime And Kiln Work | Lime, brick, tile, and kiln chains. |
| Primary Smelting | Ore smelting, bloomery, and non-ferrous smelting. |
| Primary Masonry Materials | Stone dressing, aggregate, mortar, plaster. |
| Primary Glass Batch | Glass batch preparation and furnace stock. |

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
| Supervising | Foreman and master-worker project labour. |
| Civil Engineering | Surveying, shaft/adit projects, large quarry projects if enabled. |

If the skill package does not always seed `Mining`, do not make first-pass stock content depend on it. Either add Mining as a stock professional skill or use Labouring/Masonry/Smelting with knowledge gates.

## Chain Design

### Chain A: Woodland To Charcoal

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
| Ash | ash if material exists, otherwise charcoal or wood ash material | Primary Production Waste |
| Charcoal fines | charcoal | Charcoal Fuel Commodity or Primary Production Waste |

Recommended project:

| Project | Type | Output |
| --- | --- | --- |
| Primary Production - Burn a Charcoal Clamp | local | 60-120 kg charcoal from 250-500 kg wood input. |

### Chain B: Mining And Ore Preparation

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

### Chain C: Bloomery Iron

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

### Chain D: Non-Ferrous Smelting And Alloying

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

### Chain E: Quarrying And Masonry Stone

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
| Primary Production - Quarry Granite Blocks | rough granite blocks + rubble. |
| Primary Production - Quarry Slate | slate block or roof slate commodity. |

### Chain F: Lime, Mortar, Plaster, And Binders

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

### Chain G: Clay, Bricks, Tiles, And Kilns

Purpose: support building materials and kiln economies.

```text
clay + sand/straw/grog temper
  -> craft: temper clay body
  -> prepared clay commodity
  -> craft: mould bricks / mould tiles
  -> green brick or green tile commodity
  -> project: fire brick clamp or kiln
  -> fired brick / roof tile commodity + wasters
```

First-pass crafts/projects:

| Type | Name | Inputs | Outputs |
| --- | --- | --- | --- |
| Craft | temper clay body | clay, sand/straw/grog, water | prepared clay |
| Craft | mould green bricks | prepared clay, brick mould | green brick |
| Craft | mould roof tiles | prepared clay, tile mould | roof tile or green tile |
| Project | Primary Production - Fire a Brick Clamp | green bricks, fuel | fired bricks, wasters |
| Project | Primary Production - Fire Roof Tiles | green tiles, fuel | roof tiles, wasters |

### Chain H: Glass Batch And Glass Stock

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

### Chain I: Timber Stock And Mine Support

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
- Optional endless labour: tending fire, hauling spoil, pumping water, carrying fuel.
- Materials: fuel, ore, flux, green bricks, limestone, etc.

Phase 3: Clear and recover output
- Mandatory labour: sorting, hauling, drawing, unloading.
- Actions: commodityoutput for main output, optional commodityoutput for byproducts, optional skilluse.
```

Projects can have multiple completion actions. Use multiple commodity outputs for main product and byproduct. For example, smelting can output both `iron bloom` and `slag`.

### Priority 1 Projects

| Project Name | Skill/Trait | Key Inputs | Main Output | Notes |
| --- | --- | --- | --- | --- |
| Primary Production - Burn a Charcoal Clamp | Labouring or Charcoal Burning | firewood/charcoal wood, earth/clay if modelled, charcoal rake | charcoal | High priority because it fuels many chains. |
| Primary Production - Extract Iron Ore | Labouring or Mining | mining pick, baskets, supports optional | raw iron ore, mine spoil | Basic mine extraction. |
| Primary Production - Extract Copper Ore | Labouring or Mining | mining tools | raw copper ore, mine spoil | Bronze path. |
| Primary Production - Extract Tin Ore | Labouring or Mining | mining tools | raw tin ore, mine spoil | Bronze path. |
| Primary Production - Quarry Limestone Blocks | Masonry or Labouring | quarry tools, wedges, barrow | rough limestone block, rubble | Also feeds lime. |
| Primary Production - Quarry Sandstone Blocks | Masonry or Labouring | quarry tools | rough sandstone block, rubble | Common building stone. |
| Primary Production - Burn a Lime Kiln | Masonry or Labouring | limestone, charcoal, lime kiln | quicklime | Construction binder path. |
| Primary Production - Fire a Brick Clamp | Pottery or Labouring | green bricks, fuel, clamp/kiln | fired bricks, wasters | Building materials. |
| Primary Production - Smelt an Iron Bloom | Smelting or Blacksmithing | roasted iron ore, charcoal, flux, bloomery | iron bloom, slag | Important but should require more setup. |
| Primary Production - Smelt Copper Ore | Smelting | roasted copper ore, charcoal, flux | copper ingot/commodity, slag | Bronze path. |
| Primary Production - Smelt Tin Ore | Smelting | washed tin ore, charcoal | tin ingot/commodity, slag | Bronze path. |

### Priority 2 Projects

| Project Name | Skill/Trait | Key Inputs | Main Output | Notes |
| --- | --- | --- | --- | --- |
| Primary Production - Open a Shallow Mine Working | Labouring, Mining, Civil Engineering | timber props, rope, picks, shovels | optional mine marker or no output | Site-development project. |
| Primary Production - Shore a Mine Working | Carpentry or Labouring | timber props, beams, rope | optional safety marker | Consumes wood and creates maintenance loop. |
| Primary Production - Drive an Adit | Mining, Civil Engineering | tools, timber, hauling gear | new room via prog, spoil | Use completion prog if builder config exists. |
| Primary Production - Sink a Mine Shaft | Mining, Civil Engineering | windlass, rope, timber, tools | new room via prog, spoil | More expensive site-development project. |
| Primary Production - Quarry Granite Blocks | Masonry | wedges, hammers, lifting gear | rough granite block, rubble | Harder quarry. |
| Primary Production - Quarry Slate | Masonry | quarry tools | slate/roof slate | Roofing path. |
| Primary Production - Fire Roof Tiles | Pottery | green tiles, fuel | roof tiles, wasters | Building path. |
| Primary Production - Fire a Glass Furnace Batch | Glassworking | glass batch, charcoal, furnace | glass blank | Glass upstream path. |
| Primary Production - Smelt Lead Ore | Smelting | roasted lead ore, charcoal | lead ingot, slag | Lead and silver path. |

### Site-Creation Projects

Site creation should be conservative in the first pass. Use project completion progs for room creation rather than hardcoding mine geometry into the project system.

Candidate site projects:

| Project | Completion Prog | Notes |
| --- | --- | --- |
| Drive A New Mine Chamber | create new room from template, link to current mine room | Requires builder-supplied overlay package, zone, and template room. |
| Sink A Shaft Downward | create lower room and vertical exit | Needs exit-link helper prog and safe builder constraints. |
| Open A Quarry Bench | create quarry bench room or add marker item | Could be marker-only in first pass. |

Because `CreateCell` requires an overlay package and zone, the first stock package should not assume a universal package exists. Provide example progs and documentation, but keep room creation as optional builder configuration unless the seeder can safely create a stock package and template.

## Craft Catalogue

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

### Clay, Brick, Tile, And Glass Crafts

| Craft | Category | Skill | Inputs | Tools | Products |
| --- | --- | --- | --- | --- | --- |
| temper clay body | Primary Production / Clay | Pottery | clay, sand/straw/grog, water | trough | prepared clay |
| mould green bricks | Primary Production / Brickmaking | Pottery or Labouring | prepared clay | brick mould | green bricks |
| mould roof tiles | Primary Production / Tilemaking | Pottery | prepared clay | tile mould | green roof tiles |
| prepare glass batch | Primary Production / Glass | Glassworking | sand, soda ash/potash, lime | mixing trough | glass batch |

### Tool Maintenance Crafts

| Craft | Category | Skill | Inputs | Tools | Products |
| --- | --- | --- | --- | --- | --- |
| sharpen mining pick | Primary Production / Tool Maintenance | Blacksmithing or Labouring | target pick with repair input | whetstone/grindstone | repaired pick condition |
| rehaft mining pick | Primary Production / Tool Maintenance | Carpentry or Blacksmithing | target pick, handle stock | hammer/wedge | repaired pick condition |
| sharpen stone chisel | Primary Production / Tool Maintenance | Blacksmithing or Masonry | target chisel | grindstone | repaired chisel condition |
| repair ore basket | Primary Production / Tool Maintenance | Basketry | target basket, basketry splint | awl optional | repaired basket condition |
| patch bellows leather | Primary Production / Tool Maintenance | Leatherworking | target bellows, leather panel | awl, needle | repaired bellows condition |
| replace shovel handle | Primary Production / Tool Maintenance | Carpentry | target shovel, handle stock | woodworking tools | repaired shovel condition |

If repair input support is awkward in the seeder helpers, defer these to a second pass but keep tool durability consumption in the production crafts/projects.

## Quantities And Balance

The following quantities are deliberately approximate. They are intended for gameplay and not exact industrial archaeology.

### Suggested Project Output Sizes

| Process | Input | Main Output | Byproduct |
| --- | --- | --- | --- |
| Burn charcoal clamp | 300 kg wood | 75 kg charcoal | 10 kg ash/fines |
| Extract ore | 2-4 project-hours | 100-200 kg raw ore | 50-150 kg spoil |
| Quarry stone blocks | 4-8 project-hours | 250-500 kg rough stone | 100-300 kg rubble |
| Burn lime kiln | 200 kg limestone + 100 kg charcoal | 100 kg quicklime | 20 kg rubble/ash |
| Fire brick clamp | 250 kg green bricks + 100 kg fuel | 200 kg fired bricks | 25 kg wasters |
| Smelt iron bloom | 80 kg roasted ore + 60 kg charcoal + flux | 20 kg iron bloom | 25 kg slag |
| Smelt copper ore | 80 kg roasted ore + 50 kg charcoal + flux | 8-15 kg copper stock | 30 kg slag |
| Smelt tin ore | 50 kg washed ore + 25 kg charcoal | 5-10 kg tin stock | 15 kg slag |

### Suggested Craft Batch Sizes

| Craft | Input | Output |
| --- | --- | --- |
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

## Player-Facing Workflows

### Example: Iron Production

```text
1. Miners work a local extraction project and produce raw iron ore.
2. Labourers break, sort, and wash the raw ore through short crafts.
3. A smelter roasts the ore if the ore family requires it.
4. A smelting crew runs a bloomery project with roasted ore, charcoal, and flux.
5. The project produces iron bloom and slag commodities.
6. A smith consolidates bloom into wrought iron billet.
7. A smith draws billets into bar stock, weapon stock, tool stock, or armour stock.
8. Existing finished-good crafts consume the stock.
```

### Example: Stone Building Material

```text
1. Quarry workers run a local quarry project and produce rough limestone blocks and rubble.
2. Masons dress rough blocks into dressed block commodities.
3. Labourers break rubble into aggregate.
4. Kiln workers burn limestone into quicklime.
5. Masons slake quicklime and mix mortar.
6. Construction projects consume dressed blocks, aggregate, and mortar.
```

### Example: Brick And Mortar

```text
1. Workers dig or acquire clay.
2. Potters temper clay body.
3. Brickmakers mould green bricks.
4. A local firing project consumes green bricks and fuel.
5. Fired bricks enter the market as a commodity.
6. Builders combine fired bricks with mortar in later construction projects.
```

## Implementation Plan

### Phase 0: Audit Existing Content

1. Search existing materials for ores, charcoal, coal, stone, quicklime, lime, mortar, clay, brick, glass batch, slag, ingots, billets, and stock tags.
2. Search existing tags for ore, fuel, textile stock, household craft stock, military stock, construction materials, tools, and hot fire tags.
3. Search existing item prototypes for furnaces, kilns, anvils, bellows, hammers, picks, shovels, baskets, barrows, ropes, and masonry tools.
4. Record what can be reused and what must be added.
5. Do not duplicate materials or tags. Prefer adding aliases and tags to existing materials.

Deliverable: a short audit section in the PR description and any tests that document reused dependencies.

### Phase 1: Engine Support

1. Add `CommodityOutputProjectAction` under `MudSharpCore/Work/Projects/Actions`.
2. Register `commodityoutput` in `ProjectFactory.CreateAction`, `ProjectFactory.LoadAction`, and `ProjectFactory.ValidActionTypes`.
3. Add builder commands and validation.
4. Add completion behaviour to create and place the commodity pile.
5. Add tests for action registration, XML persistence, validation, and completion output.
6. Update project system documentation to include the new action.

This is the only required engine change for the first content pass.

### Phase 2: Tags And Materials

1. Add primary-production material-function tags to `UsefulSeeder.Tags.cs`.
2. Add commodity stage tags to `UsefulSeeder.Tags.cs`.
3. Add tool tags for mining, quarrying, masonry, kiln, smelting, charcoal burning, hauling, and surveying.
4. Add or update materials in `CoreDataSeeder.Materials.cs`.
5. Add aliases for common spellings and historical names.
6. Add unit tests verifying all tags and materials exist and have expected parent/tag relationships.

### Phase 3: Tools And Apparatus

1. Add durable item prototypes for priority mining, quarrying, masonry, hauling, kiln, charcoal, and smelting tools.
2. Ensure tools carry functional tags so crafts and projects can require tool families rather than exact prototypes where possible.
3. Ensure durable tools have destroyable/tool components appropriate to existing item authoring practice.
4. Add containers or carrying aids for ore baskets, sacks, and barrows where existing components support it.
5. Add static apparatus prototypes for room tools or fixtures such as lime kilns, bloomery furnaces, charcoal clamp sites, windlasses, and large bellows.
6. Add tests verifying prototypes resolve by stable reference and carry expected tags/components.

### Phase 4: Commodity Crafts

1. Add helper methods for commodity-input and commodity-product craft definitions if not already convenient.
2. Seed ore preparation crafts for iron, copper, tin, and lead.
3. Seed binder crafts for quicklime, slaked lime, mortar, and plaster.
4. Seed clay and brick preparation crafts.
5. Seed stone dressing and aggregate crafts.
6. Seed bloom consolidation and basic metal stock crafts.
7. Seed glass batch preparation.
8. Add craft tests for representative chains.

### Phase 5: Local Projects

1. Add local project seed helpers if there is no reusable project-seeding helper outside AgricultureSeeder.
2. Seed priority local projects for charcoal, ore extraction, quarrying, lime burning, brick firing, and smelting.
3. Use commodity material requirements for bulk inputs.
4. Use `commodityoutput` actions for outputs and byproducts.
5. Use `simple` labour for ordinary work and `supervision` labour for foremen/master workers.
6. Use `skilluse` actions sparingly where the project should grant learning at completion.
7. Add project tests verifying seeded project definitions load, validate, submit, and expose expected phases, materials, labour roles, and actions.

### Phase 6: Optional Site-Creation Progs

1. Add example FutureProgs for creating a mine chamber from a template room if the repository has a safe pattern for seeding such progs.
2. Keep site-creation projects optional unless the seeder can reliably determine overlay package, zone, and template room.
3. Document the builder workflow for attaching a completion prog to a local project.
4. Do not block the first primary-production package on this feature.

### Phase 7: Documentation And Integration

1. Update this design reference with any final implemented names.
2. Add a builder workflow section or separate guide if project/craft setup commands are important.
3. Cross-reference agriculture woodland outputs and downstream medieval/antiquity crafting packages.
4. Add PR notes explaining what is complete and what is left for future work.

## Suggested File Layout

Recommended new and modified files:

```text
Design Documents/Seeding/Primary_Production_Seeder_Design_Reference.md
DatabaseSeeder/Seeders/ItemSeeder.Rework.PrimaryProductionTools.cs
DatabaseSeeder/Seeders/ItemSeederCrafting.PrimaryProduction.cs
DatabaseSeeder/Seeders/PrimaryProductionSeeder.cs
DatabaseSeeder/Seeders/PrimaryProductionSeeder.Projects.cs
DatabaseSeeder Unit Tests/PrimaryProductionSeederTests.cs
DatabaseSeeder Unit Tests/ItemSeederPrimaryProductionCraftingTests.cs
MudSharpCore/Work/Projects/Actions/CommodityOutputAction.cs
```

If the repository style prefers all craft content under `ItemSeeder`, keep craft and tool seeders there. If primary-production projects become substantial, use a dedicated `PrimaryProductionSeeder` for project definitions and leave item/craft prototype work in `ItemSeeder` partials.

## Tests And Acceptance Criteria

### Engine Acceptance

- `commodityoutput` appears in valid action types.
- A builder can create, configure, show, submit, and reload a commodity output action.
- A completed local project creates the correct commodity in the project cell.
- A completed personal project creates the correct commodity in the owner location or a documented fallback location.
- Invalid action definitions fail submit validation.

### Data Acceptance

- All new tags seed idempotently and have expected parents.
- All new materials seed idempotently, have expected aliases, and carry expected material tags.
- All new item prototypes resolve by stable reference.
- All new crafts resolve their materials, tags, tools, products, knowledge, and traits.
- All new projects resolve their materials, tags, labour traits, actions, and phase requirements.
- At least one full iron chain can be validated from extraction project output to wrought iron billet.
- At least one full stone/lime chain can be validated from quarry project output to lime mortar.
- At least one full clay/brick chain can be validated from prepared clay to fired brick.

### Gameplay Acceptance

A fresh seeded game should allow builders to set up a basic production settlement where:

1. Woodlands or timber stock feed charcoal production.
2. Charcoal feeds lime, brick, glass, and smelting work.
3. Mines create raw ores as commodities.
4. Ore preparation crafts create smelt-ready ore.
5. Smelting projects create metal intermediates.
6. Smithing and other downstream crafts can consume metal stock.
7. Quarries create rough stone and rubble.
8. Masons and builders can transform quarry outputs into dressed stone, aggregate, lime, and mortar.
9. Tools wear, break, and need maintenance or replacement.

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

### Market And Employment Integration

Add economy seeder entries or job templates for:

- miner
- quarryman
- charcoal burner
- lime burner
- brickmaker
- smelter
- furnace tender
- ore washer
- mason
- haulier
- mine foreman
- quarry master

Where ongoing job integration uses personal projects, keep the actual production projects local and physical wherever possible.

## Implementation Notes For Codex Agents

- Do not bypass commodities with manually loaded item prototypes.
- Do not make extraction crafts that instantly produce finished goods.
- Do not duplicate existing materials, tags, tools, furnaces, or stock commodities.
- Prefer exact material reuse plus commodity pile tags over creating many near-duplicate materials.
- Keep project outputs as bulk material commodities, not individual finished items.
- Keep first-pass projects deterministic and easy to test.
- Use multiple project phases to create handoff, supervision, and material-supply opportunities.
- Add tests before expanding the catalogue too widely.
- Preserve existing seed repeatability and idempotency patterns.
- Keep player-facing descriptions concrete and physical.
