# FutureMUD Medieval Industry Chain Prerequisite Addendum

**Status:** implemented prerequisite-foundation addendum for the medieval craft rebuild.
**Date:** 28 June 2026.
**Applies to:** `FutureMUD_Medieval_Industry_Chain_Craft_Design_Reference.md`.

This addendum records the accepted review caveats for the medieval craft-rebuild design. Where this document conflicts with the initial industry-chain reference, this document should be treated as the newer policy until the main reference is folded forward.

**Implementation note, 30 June 2026:** The shared prerequisite pass now seeds general `HandTool` component prototypes for the recommended medieval tool families in `UsefulSeeder`, adds missing lapidary, jewellery, and apothecary tool tag branches, expands the stock skill package with repeated medieval industry specialties that were still absent, synchronizes the maintained seeded component and tag exports, and updates the current medieval crafting audit with the prerequisite-routing columns below. Primary-production source gaps called out here, especially gemstone rough and extractive commodities, are already owned by the current primary-production seeder and its tests.

The synchronized maintained data exports are `Design Documents/Data/Seeded_Item_Components.json` for the new `Tool_*_General` `HandTool` prototypes and `Design Documents/Data/SeededTagHierarchy.csv` for the new lapidary, jewellery, and apothecary tool tag paths. No `Item_Component_Types.json`, `Seeded_Materials.json`, `Seeded_Liquids.json`, or `Seeded_Gases.json` changes were required by this pass because it added no new component type, material, liquid, or gas.

---

## Core review decision

The craft rebuild should not work around missing engine-seeded prerequisites merely because the medieval item seeder can avoid them. The medieval item seeder is not yet published, so there is no need to preserve compatibility with an earlier already-run version of the medieval craft chain.

The correct response to a missing prerequisite is therefore:

1. identify the missing component type, component prototype, tag, material, source product, or skill;
2. assign it to the correct owning seeder;
3. add it there as an idempotent, rerunnable prerequisite; and
4. treat it as present when authoring the medieval industry item and craft pass.

The seeder must still be rerunnable for future installs. That means prerequisite seeders should create or update by stable name/key and avoid duplicate rows, but the medieval item/craft pass does not need fallback branches for pre-prerequisite historical installs.

---

## Component and component-type prerequisite policy

If a craft chain needs a missing item component type or a missing item component prototype, the design should recommend adding it in the usual component-seeding location, normally `UsefulSeeder`, before the medieval item and craft implementation consumes it.

This applies especially to tool behaviour. A medieval craft tool may still be matched by tags in individual craft definitions, but it should not be forced to remain an inert tag-only prop if a meaningful component profile is missing. The desired end state is:

- the tool item has exact functional tags for craft matching;
- the tool item has an appropriate exact component prototype where runtime behaviour exists or should exist;
- the component prototype is seeded in `UsefulSeeder` or the relevant shared component seeder;
- the craft pass assumes that component prototype exists; and
- tests validate the component prototype and tag path together.

The seed data has a `HandTool` component type and now includes broad ordinary `Tool_*_General` prototypes for the medieval tool families below. These prototypes should be treated as shared prerequisite data for physical tool items that need `HandTool` runtime behaviour, while craft matching should continue to use exact functional tag paths.

### Recommended tool component families

The exact component names can be settled during implementation, but the following families should be considered prerequisites before broad craft authoring:

| Component family | Purpose | Owning seeder |
|---|---|---|
| `Tool_Blacksmithing_*` or `HandTool_Blacksmithing_*` | Hammers, tongs, punches, drawplates, swages, chisels, quenching and finishing tools. | UsefulSeeder item components. |
| `Tool_Armouring_*` | Armourer's stakes, dishing forms, planishing hammers, plate snips, mail pliers, lamellar punches. | UsefulSeeder item components. |
| `Tool_Weaponsmithing_*` | Fuller tools, sword vises, tang punches, grindstones, blade finishing tools. | UsefulSeeder item components. |
| `Tool_Woodcrafting_*` | Axes, saws, adzes, drawknives, augers, chisels, clamps, lathes. | UsefulSeeder item components. |
| `Tool_Coopering_*` | Cooper's adze, jointer, croze, hoop driver, bung borer. | UsefulSeeder item components. |
| `Tool_Textilecraft_*` | Wool combs, hackles, distaffs, shuttles, warping boards, tablet cards, niddy-noddies. | UsefulSeeder item components. |
| `Tool_Dyeing_Fulling_*` | Dye strainers, mordant cauldrons, fulling troughs, fulling mallets, tenter frames, teasel frames. | UsefulSeeder item components. |
| `Tool_Leatherworking_*` | Fleshing knives, hide scrapers, tanning beams, currying knives, saddler clamps, leather creasers. | UsefulSeeder item components. |
| `Tool_Parchmentmaking_*` | Stretching frames, lunellae, pumice, pounce bags, scraping beams. | UsefulSeeder item components. |
| `Tool_Papermaking_*` | Mould-and-deckle, vats, couching blankets, press felts, lay presses, rag tools. | UsefulSeeder item components. |
| `Tool_Bookbinding_*` | Book presses, lying presses, sewing frames, punches, needles, backing hammers, paring knives. | UsefulSeeder item components. |
| `Tool_Pottery_*` | Potter's wheel, ribs, clay knives, wire cutters, moulds, stamps. | UsefulSeeder item components. |
| `Tool_Masonry_*` | Mason's hammers, chisels, trowels, lines, squares, rammers. | UsefulSeeder item components. |
| `Tool_Glassblowing_*` | Blowpipes, pontils, jacks, shears, marvers, glass blocks, lehrs. | UsefulSeeder item components. |
| `Tool_Lapidary_*` | Lapidary saws, wheels, bow drills, polishers, bead drills. | UsefulSeeder item components. |
| `Tool_Jewellery_*` | Jeweller's anvil, small stakes, burnishers, crimping pliers, setting tools. | UsefulSeeder item components. |
| `Tool_Apothecary_*` | Mortars, pestles, medicine strainers, spatulas, preparation boards. | UsefulSeeder item components. |
| `Tool_Medical_*` | Forceps, probes, suture needles, cautery irons, cupping vessels where mechanical tool behaviour matters. | UsefulSeeder item components. |
| `Tool_Printing_Woodblock_*` | Block carving knives, clearing chisels, ink daubers, barens, impression spoons. | UsefulSeeder item components. |

Not every physical tool must require a unique component prototype. Use a shared component profile where the craft-speed or craft-quality behaviour is the same. Create separate prototypes where a tool should have distinct tool effectiveness, quality interaction, allowed-craft scope, or future runtime handling.

### Tags still matter

Craft definitions should still prefer tag-based tool matching when multiple physical items should satisfy the same craft requirement. The presence of tool components does not replace functional tags. The preferred model is both:

```text
item has Tool/HandTool component profile
item has exact Functions / Tools / ... tag path
craft requires a TagTool or exact item only as appropriate
```

Exact stable-reference tool requirements should remain reserved for singular apparatus or named workstations where interchangeability would be misleading.

---

## Upstream production-chain gap policy

The industry-chain audit may discover that a desired terminal source is not actually available from its owning production system. That should not force the medieval craft chain to invent a manufactured substitute or silently treat a non-existent product as terminal.

If a terminal-source gap is found, recommend expanding the owning upstream production seeder before the medieval craft chain consumes the product.

| Gap type | Example | Owning resolution |
|---|---|---|
| Butchery product gap | missing sinew, gut, horn, antler, hide grade, bone grade, shell, feather, tallow, or specific animal skin product. | Expand the butchery/carcass product seeder. |
| Forage product gap | missing wild dye plant, resin, reed, rush, bark, medicinal herb, flower, wild fibre, gum, or naturally gathered pigment/mineral. | Expand the forage product seeder. |
| Agricultural product gap | missing flax, hemp, cotton crop, silk cocoon, wool grade, straw, oilseed, food crop, dye crop, honeycomb, beeswax, milk, egg, or managed herb crop. | Expand the agriculture, husbandry, pastoral, apiary, or sericulture seeder. |
| Primary-industry gap | missing gemstone rough, precious stone source, ore grade, charcoal, salt, clay, sand, limestone, gypsum, slate, stone block, timber log, or quarry product. | Expand the primary production / extraction seeder. |

Gemstones are the clearest likely case. If the medieval jewellery chain needs rough gemstones but the primary production seeder does not produce gem rough or mine/quarry gemstone outputs, the design should recommend a primary-production pass for gem production rather than treating polished gemstones as terminal or smuggling them in through jewellery crafts.

The same policy applies to ore, flux, pigments, glass sand, soda/potash ash, chalk, clay, slate, pearl/shell, amber/jet, and specialist stone sources. The final medieval craft chain should consume upstream source products only after those source products have an explicit seeder owner.

---

## Skill-package prerequisite policy

If a craft family needs a medieval or historical skill that does not already exist in the configured skill package, the design should recommend adding it to the skill package seeder rather than collapsing the craft into a vague generic skill.

The craft authoring pass should first reuse existing sensible aliases and stock skills. If a missing skill would repeatedly appear across an industry chain, add it as a prerequisite skill package item.

Likely skill additions or confirmations include:

| Skill or skill family | Uses |
|---|---|
| Blacksmithing / Smithing | General forge work, tools, hardware, fittings, nails, hinges, bars. |
| Smelting | Ore roasting, bloomery/furnace operation, metal extraction. |
| Armourcrafting / Armourer | Mail, lamellar, scale, plate-reinforced, helmets, shields. |
| Weaponcrafting / Weaponsmithing | Blades, heads, weapon assembly, sharpening and fitting. |
| Locksmithing | Locks, keys, wards, latches, lock installation. |
| Silversmithing / Goldsmithing | Fine metal jewellery, seal matrices, luxury mounts. |
| Gemcutting / Lapidary / Gemcraft | Gem rough, cabochons, beads, signet stones, polished settings. |
| Glassworking / Glassblowing | Glass vessels, beads, panes, lamps, annealing. |
| Pottery | Clay body, greenware, vessels, tiles, bricks, kiln firing. |
| Masonry / Stoneworking | Dressed stone, slate, mortar, plaster, construction fittings. |
| Carpentry / Joinery | Furniture, containers, doors, boards, frames, handles. |
| Coopering | Barrels, casks, tubs, staves, hoops. |
| Wheelmaking / Wheelwrighting | Wheels, spokes, hubs, carts if future catalogue branches require them. |
| Bowmaking / Bowyer | Bows, bow staves, composite-bow stock, tillering. |
| Fletching / Fletcher | Arrows, bolts, fletching stock, shafts. |
| Ropemaking | Cordage, ropes, bowstrings, rigging, heavy bindings. |
| Spinning | Fibre to thread/yarn. |
| Weaving | Cloth, bands, canvas, broadcloth, tablet weaving where not separated. |
| Fulling | Fullered cloth, felt-like finishing, broadcloth preparation. |
| Dyeing | Dye baths, mordants, dyed yarn/cloth, pigment preparation. |
| Tailoring | Clothing, bags, soft goods, padded panels. |
| Cobbling | Shoes, boots, soles, footwear repairs. |
| Leathermaking / Tanning | Hide preparation, leather panels, straps, hardened leather. |
| Parchmentmaking | Parchment skins, sheets, bifolia, prepared writing surfaces. |
| Papermaking | Rag paper, East Asian paper, pulp, sheet formation. |
| Bookbinding | Quires, codices, ledgers, covers, boards, presses. |
| Calligraphy / Scribing | Pens, written-good preparation, manuscript layout, qalam/brush use. |
| Woodblock Printing | Blocks, barens, daubers, printed sheets/books. |
| Apothecary / Herbalism / Medicine | Medicinal stock, salves, poultices, drug-bearing items, treatment supplies. |
| Brewing / Vintning / Distilling | Beverage and medicinal liquid chains where supported by liquids. |
| Cooking / Baking / Milling | Food production, flour, meal, bread-like goods. |
| Butchering | Butchery terminal products, carcass breakdown, skins, bone, sinew. |
| Foraging | Wild plant/mineral source products. |
| Farming / Agriculture / Husbandry / Beekeeping | Managed crops, fibre crops, animals, apiary products, silk where applicable. |
| Mining / Quarrying / Lumberjacking | Primary resource extraction. |

The exact skill names should follow the current project skill naming conventions. Where the existing skill package already has a suitable skill and aliases, use it. Where it lacks a repeated industry-critical skill, add the skill in the skill package seeder as a prerequisite rather than encoding many unrelated crafts under `Crafting` or `Labouring`.

---

## Audit changes required by this addendum

The craft-completeness audit should add prerequisite-oriented columns so gaps are routed to the correct upstream pass instead of being lost in finished-item craft work.

Recommended additional audit columns:

```text
stable_reference
item_source_file
craft_name
craft_method
immediate_inputs
missing_input_crafts
terminal_inputs
terminal_source_class
terminal_source_owner
missing_terminal_source
required_tools
missing_tool_items
missing_tool_components
missing_tool_tags
missing_component_types
missing_component_prototypes
missing_materials
missing_tags
required_skill
missing_skill_package_entry
owning_resolution_pass
resolution_status
```

`owning_resolution_pass` should name the place where the fix belongs, for example `UsefulSeeder item component pass`, `UsefulSeeder tag pass`, `Primary production seeder`, `Agriculture seeder`, `Butchery seeder`, `Forage seeder`, or `Skill package seeder`.

---

## Updated milestone order

The first implementation milestone should be reframed as a prerequisite-and-foundation milestone rather than only a medieval item/craft milestone:

1. Run the industry audit against all accepted medieval catalogue items.
2. Identify missing component types, component prototypes, tags, materials, terminal-source products, and skills.
3. Add missing components, component prototypes, tags, and shared prerequisite data in `UsefulSeeder` or the relevant shared prerequisite seeder.
4. Add missing terminal products in their owning upstream production seeders: butchery, forage, agriculture, pastoral/apiary/sericulture, primary production, mining, quarrying, forestry, clay, salt, or gem production as appropriate.
5. Add missing skills to the skill package seeder where they are repeatedly required by the medieval industry model.
6. Add medieval tool and intermediate-stock item prototypes that consume those prerequisites.
7. Add production-chain crafts only after the prerequisite seeders are in place.
8. Add finished-good crafts by category.
9. Validate that every final item, intermediate item, tool item, source product, component, tag, material, and skill resolves through the audit.

This makes the medieval craft rebuild a coordinated data-model expansion rather than a narrow recipe-writing pass.
