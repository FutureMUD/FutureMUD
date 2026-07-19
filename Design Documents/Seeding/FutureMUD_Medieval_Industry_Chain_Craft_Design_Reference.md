# FutureMUD Medieval Industry Chain and Craft Dependency Design Reference

**Status:** initial implementation-planning reference for the medieval craft rebuild.  
**Date:** 28 June 2026.  
**Era band:** approximately 500-1300 CE, matching the current medieval item-seeder slice.  
**Primary implementation target:** make every accepted medieval catalogue item craftable, then recursively close every non-terminal craft input until the chain reaches butchery, forage, agricultural, or primary-industry products.

This document is deliberately a craft and dependency reference, not a claim that medieval crafts are already implemented. It should guide the next code passes in `ItemSeeder.Crafting.Medieval.cs` and the likely companion tool/intermediate-stock item pass. It should be updated as craft rows, tool items, intermediary products, and tests land.

---

## Executive summary

The medieval item catalogue is now broad enough that craft authoring should not begin item-by-item in isolation. A direct craft pass against finished goods would immediately create untracked requirements for tools, workstations, and intermediate stock such as thread, cloth, leather panels, iron bars, rivets, wire, hinges, parchment, paper, ink, glue, wax, boards, bow staves, weapon heads, shield boards, dye baths, and medicine stock. The implementation therefore needs an explicit industry-chain model.

The craft rebuild should proceed in layers:

1. **Foundation audit pass:** enumerate all finished item prototypes by stable reference, material, tags, and component family.
2. **Industry closure pass:** assign every item to a production chain and identify immediate craft inputs.
3. **Tool and workstation item pass:** add full item prototypes for missing tools and apparatus before crafts require them.
4. **Intermediate stock pass:** create player-visible intermediate stock items only where they are useful as trade goods, named inputs, stackable stock, repair stock, or shared craft outputs; otherwise use craft commodity products/inputs.
5. **Terminal-source pass:** stop only at products that are explicitly butchery, forage, agricultural, pastoral/apiary, fishing, forestry, mining, quarrying, clay-working, salt-working, water-gathering, or other primary-industry outputs.
6. **Craft authoring pass:** add craft definitions in dependency order, starting with production-chain crafts and then the finished-item categories.
7. **Validation pass:** prove that every accepted catalogue item has a craft path and that every non-terminal input is itself craftable.

The goal is not to make one recipe per item with bespoke inputs. The goal is a maintainable dependency lattice: shared stock, shared tools, category-specific finishing crafts, and stable exceptions where a one-off craft is justified.

---

## Current project grounding

The current medieval rebuild is direct-call item seeding. The craft launch points exist but are intentionally no-op. This reference assumes the following source layout unless later refactoring changes it:

| Area | Current implementation or target file |
|---|---|
| Craft entry points | `DatabaseSeeder/Seeders/ItemSeeder.Crafting.Medieval.cs` |
| Shared craft helpers | `DatabaseSeeder/Seeders/ItemSeeder.Crafting.cs` |
| Shared historic foundation items | `DatabaseSeeder/Seeders/ItemSeeder.HistoricFoundation.cs` |
| Clothing item source | `DatabaseSeeder/Seeders/ItemSeeder.MedievalClothing.cs` |
| Household/container/furniture item sources | `ItemSeeder.MedievalContainers.cs`, `ItemSeeder.MedievalDoorsLocksStrongboxes.cs`, `ItemSeeder.MedievalFood.cs`, `ItemSeeder.MedievalFurniture.cs`, `ItemSeeder.MedievalJewellery.cs` |
| Military item sources | `ItemSeeder.MedievalWeapons.cs`, `ItemSeeder.MedievalArmour.cs` |
| Writing/document item source | `ItemSeeder.MedievalWriting.cs` |
| Treatment and repair item sources | `ItemSeeder.MedievalMedical.cs`, `ItemSeeder.MedievalRepairKits.cs` |
| Existing current-state audit | `Design Documents/Seeding/Medieval_Crafting_Audit.md` |

Recommended implementation files for this workstream:

```text
DatabaseSeeder/Seeders/ItemSeeder.MedievalIndustryTools.cs
DatabaseSeeder/Seeders/ItemSeeder.MedievalIndustryStock.cs
DatabaseSeeder/Seeders/ItemSeeder.Crafting.MedievalProduction.cs
DatabaseSeeder/Seeders/ItemSeeder.Crafting.MedievalFinishedGoods.cs
DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs
```

The exact split can change, but the craft code should keep terminal source crafts, intermediate stock crafts, tool crafts, and finished-good crafts easy to audit separately.

---

## Dependency model

### Terminal product classes

A craft chain may stop only at one of the following terminal classes. If an input does not clearly belong to one of these classes, it should be either a crafted intermediate or a full item requiring its own craft.

| Terminal class | Meaning | Examples |
|---|---|---|
| Butchery product | A direct output of carcass processing, skinning, slaughter, field dressing, fishing catch processing, or equivalent animal breakdown. | raw hide, fur, skin, sinew, gut, bone, horn, antler, shell, feather, tallow/suet, raw meat, offal, ivory/tusk, chitin. |
| Forage product | A gathered wild plant, mineral, resin, flower, root, fungus, reed, bark, dye plant, or naturally occurring small material. | flowers, herbs, reeds, rushes, bark, roots, berries, nuts, wild dye plants, gum/resin, naturally shed feathers where used as forage. |
| Agricultural product | A product of farming, horticulture, arboriculture, pastoralism, apiculture, sericulture, or managed food/fibre production. | flax, hemp, cotton crop, jute, wool, silk cocoons, straw, hay, grain, oilseed, grapes, olives, honeycomb, beeswax, milk, eggs, manure, food herbs, dye crops. |
| Primary industry product | A product of logging, mining, quarrying, clay digging, salt working, water drawing, charcoal burning, basic earthworks, or equivalent extractive industry. | logs, poles, firewood, charcoal, ore, native metal, sand, clay, chalk, limestone, gypsum, stone blocks, slate, salt, water, shell sand, soda ash, potash. |

### Non-terminal product classes

Everything below should be crafted or produced by another craft before it appears as a craft input:

- prepared textile fibre, roving, thread, yarn, cord, rope, cloth, felt, canvas, broadcloth, silk fabric, dye bath, mordant solution, fullered cloth, garment panels;
- scraped hide, parchment skin, tanned leather, leather panel, leather sole, leather strap, leather thong, hardened leather, leather scales;
- timber, board, plank, stave, shaft, dowel, handle, bow stave, shield board, wheel spoke, joined frame, furniture panel;
- ore concentrate, roasted ore, bloom, ingot, bar, rod, sheet, plate, wire, rivet, ring, chain link, armour lamella, armour scale, blade blank, weapon head, buckle, hinge, hasp, key blank, lock ward blank;
- clay body, slip, greenware, fired ceramic, brick, tile, glaze stock, glass batch, glass blank, bead blank, pane, lamp glass;
- lime, slaked lime, mortar, plaster, dressed stone, cut slate, carved stone block;
- parchment sheet, rag paper sheet, papyrus sheet, waxed tablet blank, ink, pigment cake, binder glue, seal wax, book board, quire, scroll rod;
- flour, meal, malt, wort, oil, cheese, butter, salted meat, smoked meat, dried fish, vinegar, wine stock, ale stock;
- poultice stock, salve base, decoction stock, suture thread, bandage roll, splint blank, prosthetic stock, medicine wrapper stock;
- bead, setting, wire loop, chain, clasp, brooch pin, circlet band, decorative plaque.

### Closure rule

Every craft must be auditable through this question:

```text
For each input: is this input terminal, or is there a craft producing it?
For each tool: is this tool a seeded item, and does it carry the tag the craft requires?
For each product: is this product either a final catalogue item, a deliberate intermediate item, or a commodity product used by later crafts?
```

Crafts may use commodity inputs where grams of material matter more than a named object. They should use item inputs where identity matters: a lock, key blank, knife blade, quiver body, parchment sheet, codex board, weapon head, shield boss, specific tool, wax tablet blank, bow stave, or repair kit.

---

## Craft implementation principles

### Shared-first recipe architecture

Most medieval items should be covered by shared category recipes that vary by stable reference, material, tag, quality, and output. Do not create separate custom recipe structures for every visually different tunic, pouch, chest, sword, quiver, codex, or bracelet if they share the same actual production path.

A useful target pattern:

- one or a few **stock-making crafts** per material family;
- one **component-making craft** per reusable intermediate family;
- one **assembly craft family** per finished item family;
- separate **specialist finishing crafts** for locks, seals, armour, bookbinding, luxury jewellery, medical items, and other mechanically sensitive items.

### Tools are real items

A craft tool requirement should not be a purely imaginary string. If a craft requires `TagTool - Held - an item with the Hammer tag`, at least one seeded item must carry an exact matching hammer tag. The existing historic foundation pass already follows this model for shared hearths, kilns, looms, spindle, sewing needle, shears, awl, dye vat, tanning rack, quern, anvil, tongs, hammer, and bellows.

Use tool requirements by tag rather than by exact stable reference when multiple tools should work. Use exact stable references only when the craft depends on a singular apparatus or a special item.

### Tool item implementation

Most medieval craft tools can be ordinary item prototypes with:

- `Holdable` if portable;
- no `Holdable` if later implemented as a true installed fixture;
- exactly one destroyable component appropriate to material and size;
- a market tool tag such as `Market / Professional Tools / Standard Tools`;
- one or more exact functional tool tags under `Functions / Tools / ...`.

The current component inventory has a `HandTool` component type and now includes shared ordinary medieval `Tool_*_General` prototypes for common industry families. Tool matching should still rely on exact functional tags in craft requirements; the shared component prototypes supply runtime tool-duration and craft-speed behaviour for physical tool items that carry those tags. Locksmith tool kits remain the more specialised exception because exact locksmithing tool components exist.

### Workstations and fixtures

The current medieval household catalogue treats even bulky furniture and doors as moveable prototypes. The historic foundation items also use `Holdable` for cross-era tools and apparatus. The first craft pass should follow that convention for consistency, then later create fixture-only variants only where the runtime needs installed, immobile workshops.

Workstation items should be full items when they are visible, ownable, movable, damageable, or tradeable: looms, kilns, forges, smelting furnaces, presses, workbenches, vats, tanning racks, ropewalks, book presses, glass furnaces, mills, and large frames.

### Intermediate stock policy

Create a full item prototype when the intermediate is one or more of the following:

- a common trade good visible to players;
- an input shared by many finished goods;
- a stock item that plausibly sits in containers, workshops, markets, or inventories;
- a repair-kit or maintenance input;
- an input whose material, quality, or tags need to be inspected;
- an item likely to be stolen, traded, stored, burned, wetted, written on, sealed, worn, lit, locked, or otherwise acted on by item components.

Use commodity products when the stock is better represented as grams of material: flour, clay body, glue, pigment powder, bloom iron, simple timber stock, fibre stock, leather scraps, mortar, slag, and similar bulk matter.

### Quality propagation

Quality should propagate through important stages but not require every commodity to exist in twelve quality tiers. Prioritize quality on:

- expensive or skill-intensive tools;
- finished weapons, armour, locks, jewellery, books, and medical items;
- stock where quality materially changes many later outputs, such as fine cloth, fine leather, steel, parchment, paper, glass, and gemstone blanks.

### Regional variation

The medieval item references use a shared-first model with regional additions where material, form, component, or culture-facing silhouette differs. Crafting should follow the same rule. Use shared craft families for ordinary items and create regional craft variants only where the production technology genuinely differs: palm-leaf manuscripts, East Asian paper/brush goods, steppe bow and quiver work, Japanese lamellar equipment, adarga/dhal shield families, South Asian and East Asian textile work, Islamicate paper and qalam goods, and specific religious/administrative forms.

---

## Existing shared historic foundation tools

These full items already exist and should be reused before creating medieval-specific duplicates.

| Stable reference | Tool or apparatus | Existing role in craft planning |
|---|---|---|
| `historic_workshop_hearth` | unlit workshop hearth | General low heat, wax, glue, pitch, drying, small workshop heating. |
| `historic_lit_workshop_hearth` | lit workshop hearth | Hot-fire-capable lit state for low workshop heating. |
| `historic_updraft_kiln` | unlit updraft kiln | Pottery, lamp, tile, small ceramic, and sealed-ware firing. |
| `historic_lit_updraft_kiln` | lit updraft kiln | Hot-fire kiln state. |
| `historic_warp_weighted_loom` | warp-weighted loom | Broad cloth, bands, household textiles. |
| `historic_treadle_loom` | timber treadle loom | Larger bolts of cloth and higher-throughput weaving. |
| `historic_drop_spindle` | weighted drop spindle | Fibre-to-yarn spinning. |
| `historic_sewing_needle` | iron sewing needle | Clothing, book cloth, light leather, household sewing. |
| `historic_textile_shears` | iron textile shears | Cloth, thread, felt, and light leather cutting. |
| `historic_awl_punch` | iron awl punch | Leather, parchment, heavy cloth stitch holes. |
| `historic_dye_vat` | timber dye vat | Dyeing skeins, cloth, and small garments. |
| `historic_tanning_rack` | hide tanning rack | Stretching hides during drying, smoking, oiling, or tanning. |
| `historic_hand_quern` | rotary hand quern | Grain, dry pigment, dye material, and dry stock grinding. |
| `historic_oil_lamp` | unlit clay oil lamp | Lighting stock and a craftable historic foundation item. |
| `historic_lit_oil_lamp` | lit clay oil lamp | Lit lamp state, also a close-work light source. |
| `historic_workshop_anvil` | low workshop anvil | Metal fittings, rings, blades, sheet work, small forged stock. |
| `historic_forge_tongs` | forge tongs | Hot-work gripping and movement. |
| `historic_workshop_hammer` | workshop hammer | General metal, riveting, and shaping hammer. |
| `historic_bellows` | leather workshop bellows | Air supply for hearths and furnaces. |

Do not re-create these as medieval-specific items unless the medieval craft needs a different size, quality band, culture-specific tool, component combination, or tag path.

---

## Required tool and apparatus item targets

The tables below begin the required tool catalogue. These are item prototypes to add before or alongside the first craft implementation. Suggested stable references are intentionally conservative; exact names may change during item authoring, but the functional role should remain.

### Heat, fuel, furnace, and chemical-processing tools

| Suggested stable reference | Item target | Use | Status |
|---|---|---|---|
| `medieval_workshop_charcoal_clamp` | charcoal clamp or charcoal pit | Turns timber/firewood into charcoal; may be represented as a craft apparatus or a craft action with firewood input. | Required. |
| `medieval_workshop_charcoal_rake` | charcoal rake | Charcoal burning, kiln tending, brazier and furnace stoking. | Required. |
| `medieval_workshop_forge` | smithing forge | Blacksmithing, tool making, weapon and armour stock. | Required; historic hearth is not enough for all metalwork. |
| `medieval_workshop_lit_forge` | lit smithing forge | Hot state of forge with `Hot Fire` tag. | Required if forge uses morph state. |
| `medieval_workshop_smelting_furnace` | smelting furnace or bloomery | Ore-to-bloom and ore-to-metal production. | Required. |
| `medieval_workshop_lit_smelting_furnace` | lit smelting furnace | Hot state for smelting. | Required if furnace uses morph state. |
| `medieval_tool_ore_roaster` | ore roaster | Roasting sulphide ores and preparing ore charges. | Required for metal closure. |
| `medieval_tool_ore_crusher` | ore crusher | Crushing ore, slag, glass cullet, pigment stone. | Required. |
| `medieval_tool_charging_bucket` | charging bucket | Loading ore, charcoal, flux, glass batch, or kiln charge. | Required. |
| `medieval_tool_slag_hammer` | slag hammer | Breaking bloom and slag cakes. | Required. |
| `medieval_tool_slag_skimmer` | slag skimmer | Removing slag and clinker from furnace/crucible work. | Required. |
| `medieval_tool_tap_rod` | furnace tap rod | Opening/clearing furnace taps and channels. | Required for advanced smelting. |
| `medieval_tool_crucible` | ceramic crucible | Casting metals, melting glass, pigment calcining. | Required. |
| `medieval_tool_crucible_tongs` | crucible tongs | Handling hot crucibles. | Required. |
| `medieval_workshop_lime_kiln` | lime kiln | Limestone/chalk to quicklime; lime mortar, plaster, parchment, tanning, and building chain. | Required. |
| `medieval_workshop_glass_glory_hole` | glass glory hole | Hot-work glass shaping. | Required if glass goods are craftable beyond inert imports. |
| `medieval_workshop_annealing_lehr` | annealing lehr | Cooling glass goods without breakage. | Required for glass closure. |
| `medieval_tool_lye_leaching_barrel` | ash lye leaching barrel | Lye for soap, textile processing, and some cleaning/preparation chains. | Required unless lye remains a terminal commodity. |

### Forestry, carpentry, joinery, coopering, and wheelwright tools

| Suggested stable reference | Item target | Use | Status |
|---|---|---|---|
| `medieval_tool_felling_axe` | felling axe | Logs, poles, firewood, rough timber. | Required. |
| `medieval_tool_splitting_axe` | splitting axe | Planks, billets, staves, firewood. | Required. |
| `medieval_tool_adze` | adze | Hewing, bowl work, timber shaping, shipboard/rough carpentry. | Required. |
| `medieval_tool_drawknife` | drawknife | Shafts, handles, bow staves, staves, spokes. | Required. |
| `medieval_tool_spokeshave` | spokeshave | Shafts, handles, spokes, bow tillering. | Required. |
| `medieval_tool_hand_saw` | hand saw | General carpentry cuts. | Required. |
| `medieval_tool_bow_saw` | bow saw | Curved and frame cuts. | Required. |
| `medieval_tool_forest_saw` | forest saw | Large timber processing. | Required for timber chain. |
| `medieval_tool_fine_saw` | fine saw | Small boxes, inlay, book boards, fine furniture. | Required. |
| `medieval_tool_wood_chisel` | wood chisel | Joinery, carving, hinges, fittings seats, blocks. | Required. |
| `medieval_tool_wood_auger` | wood auger | Holes for pegs, furniture, barrels, shields, wheels. | Required. |
| `medieval_tool_wood_file` | wood file or rasp | Smoothing and fitting. | Required. |
| `medieval_tool_wood_clamp` | wood clamp | Glue-up, frames, cabinets, shields, book boards. | Required. |
| `medieval_workshop_lathe` | pole lathe or bow lathe | Bowls, beads, handles, pegs, spindles, turned furniture elements. | Required. |
| `medieval_tool_coopers_adze` | cooper's adze | Barrel and tub staves. | Required for casks/barrels/tubs. |
| `medieval_tool_coopers_jointer` | cooper's jointer | Truing staves. | Required. |
| `medieval_tool_croze` | croze | Barrel-head groove cutting. | Required. |
| `medieval_tool_hoop_driver` | hoop driver | Barrel hoops, pails, casks. | Required. |
| `medieval_tool_bung_borer` | bung borer | Liquid barrels and sealed casks. | Required. |
| `medieval_tool_wheelwright_clamp` | wheelwright's clamp | Wheels and large round assemblies. | Required for carts/wheels if added. |
| `medieval_tool_tenon_cutter` | tenon cutter | Spokes, shafts, chairs, frames. | Required for advanced joinery. |

### Textile, dyeing, fulling, basketry, and cordage tools

| Suggested stable reference | Item target | Use | Status |
|---|---|---|---|
| `medieval_tool_distaff` | distaff | Flax, wool, hemp, cotton spinning support. | Required. |
| `medieval_tool_wool_combs` | wool combs | Raw wool to combed fibre. | Required. |
| `medieval_tool_flax_hackle` | flax hackle | Flax/hemp fibre preparation. | Required. |
| `medieval_tool_scutching_knife` | scutching knife | Bast fibre preparation. | Required. |
| `medieval_tool_niddy_noddy` | skein winder or niddy-noddy | Yarn skeins, measured thread. | Required. |
| `medieval_tool_spinners_weights` | spinner's weights | Spindle weights and spinning setup. | Optional, useful. |
| `medieval_tool_warping_board` | warping board | Preparing warp before loom. | Required for weaving chain. |
| `medieval_tool_shuttle` | shuttle | Weaving cloth and bands. | Required. |
| `medieval_tool_beater_batten` | beater batten | Weaving. | Required. |
| `medieval_tool_heddle_rod` | heddle rod | Loom setup. | Required. |
| `medieval_tool_lease_rod` | lease rod | Loom setup. | Required. |
| `medieval_tool_weaving_reed` | weaving reed | Loom setup and cloth width. | Required for advanced weaving. |
| `medieval_tool_tablet_weaving_cards` | tablet weaving cards | Tablet-woven bands, trim, straps. | Required for many clothing trims. |
| `medieval_tool_dye_strainer` | dye strainer | Dye bath preparation. | Required. |
| `medieval_tool_mordant_cauldron` | mordant cauldron | Alum/iron/copper mordant treatment. | Required. |
| `medieval_tool_dye_stirring_pole` | dye stirring pole | Dye vat work. | Required. |
| `medieval_tool_skein_rack` | skein rack | Dyeing and drying yarn. | Required. |
| `medieval_tool_dye_drying_line` | dye drying line | Drying cloth/yarn after dyeing. | Required. |
| `medieval_tool_fullers_trough` | fuller's trough | Fulling wool cloth. | Required. |
| `medieval_tool_fullers_mallet` | fuller's mallet | Manual fulling. | Required. |
| `medieval_workshop_fulling_stocks` | fulling stocks | Water/foot-powered fulling alternative. | Optional advanced infrastructure. |
| `medieval_tool_tenter_frame` | tenter frame | Stretching fullered cloth. | Required. |
| `medieval_tool_teasel_frame` | teasel frame | Raising nap on cloth. | Required for broadcloth. |
| `medieval_tool_napping_shears` | napping shears | Finishing cloth surface. | Required for fine cloth. |
| `medieval_tool_reed_splitter` | reed splitter | Basketry and reedwork. | Required for baskets and mats. |
| `medieval_tool_basket_knife` | basket knife | Basketry. | Required. |
| `medieval_tool_weaving_bodkin` | weaving bodkin | Basketry and strap weaving. | Required. |
| `medieval_workshop_ropewalk` | ropewalk | Rope and heavy cordage. | Required for rope production. |
| `medieval_tool_rope_hook` | rope hook | Rope-making. | Required. |
| `medieval_tool_rope_top` | ropemaking top | Rope lay control. | Required. |
| `medieval_tool_marlinespike` | marlinespike | Rope work, knots, rigging goods. | Required. |

### Leather, parchment, horn, bone, and animal-product tools

| Suggested stable reference | Item target | Use | Status |
|---|---|---|---|
| `medieval_tool_skinning_knife` | skinning knife | Butchery-to-hide interface; may be required by butchery crafts but terminal products remain butchery. | Required. |
| `medieval_tool_fleshing_knife` | fleshing knife | Cleaning hides and parchment skins. | Required. |
| `medieval_tool_hide_scraper` | hide scraper | Dehairing and hide preparation. | Required. |
| `medieval_tool_tanning_beam` | tanning beam | Hide scraping/dehairing. | Required. |
| `medieval_tool_dehairing_knife` | leather dehairing knife | Hide preparation. | Required. |
| `medieval_tool_tanning_paddle` | tanning paddle | Vat tanning, washing hides. | Required. |
| `medieval_tool_brain_tanning_bucket` | brain-tanning bucket | Oil/brain tanning chain. | Optional but useful. |
| `medieval_tool_currying_knife` | currying knife | Finished leather dressing. | Required. |
| `medieval_tool_saddlers_clamp` | saddler's clamp or stitching pony | Harness, shoes, belts, quivers, saddle goods. | Required; may need new tag path. |
| `medieval_tool_leather_creaser` | leather creaser | Belts, straps, shoes, scabbards. | Required. |
| `medieval_tool_burnisher` | leather/bone burnisher | Edges, parchment, leather finish. | Required. |
| `medieval_tool_bone_saw` | bone saw | Bone, horn, antler blanks. | Required. |
| `medieval_tool_bone_file` | bone file | Bone/horn fitting, jewellery, tools. | Required. |
| `medieval_tool_parchment_stretching_frame` | parchment stretching frame | Parchment production. | Required. |
| `medieval_tool_parchment_lunellum` | lunellum | Parchment scraping. | Required. |
| `medieval_tool_parchment_pumice` | parchment pumice | Smoothing sheets. | Required. |
| `medieval_tool_pounce_bag` | pounce bag | Parchment/paper preparation. | Required. |

### Metalworking, armouring, weaponsmithing, locksmithing, and jewellery tools

| Suggested stable reference | Item target | Use | Status |
|---|---|---|---|
| `medieval_tool_smithing_punch_set` | smithing punch set | Rivets, holes, tangs, lock plates. | Required. |
| `medieval_tool_drawplate` | drawplate | Wire for jewellery, mail, pins, springs, rings. | Required. |
| `medieval_tool_grindstone` | grindstone | Blades, tools, needles, weapon edges. | Required. |
| `medieval_tool_whetstone` | whetstone | Finishing edges and sharp tools. | Required. |
| `medieval_tool_quenching_trough` | quenching trough | Blade/tool heat treatment. | Required. |
| `medieval_tool_fuller_tool` | fuller tool | Sword and blade fullers. | Required for swords. |
| `medieval_tool_tang_punch` | tang punch | Blades and weapon heads. | Required for weaponcraft. |
| `medieval_tool_sword_vise` | sword vise | Sword grip and hilt work. | Required for swords. |
| `medieval_tool_crossguard_fixture` | crossguard fixture | Sword hilt fitting. | Required for swords. |
| `medieval_tool_armourers_stake` | armourer's stake | Plate/scale/lamellar shaping. | Required. |
| `medieval_tool_ball_stake` | ball stake | Doming bowls, helmets, bosses. | Required. |
| `medieval_tool_dishing_form` | dishing form | Helmets, shield bosses, armour plates. | Required. |
| `medieval_tool_forming_bag` | armourer's forming bag | Plate and boss dishing. | Required. |
| `medieval_tool_planishing_hammer` | planishing hammer | Armour and sheet finishing. | Required. |
| `medieval_tool_raising_hammer` | raising hammer | Helmet bowls, cups, raised metal vessels. | Required. |
| `medieval_tool_plate_snips` | plate snips | Armour plate, sheet metal, scale cutting. | Required. |
| `medieval_tool_armourers_pliers` | armourer's pliers | Mail rings, rivets, lamella lacing. | Required. |
| `medieval_tool_locksmithing_fabrication_kit` | locksmithing fabrication tools | Locks, keys, wards, lockboxes. | Required; use exact `Locksmithing_Fabrication` component if authored as a kit. |
| `medieval_tool_locksmithing_installation_kit` | locksmithing installation tools | Installing locks/latches/hasps. | Required; use exact locksmithing component if authored as a kit. |
| `medieval_tool_jewellers_anvil` | jeweller's anvil | Rings, brooches, settings, pins. | Required; current tag gap may need new tag. |
| `medieval_tool_jewellers_crimping_pliers` | jeweller's crimping pliers | Crimps, clasps, settings. | Required. |
| `medieval_tool_jewellers_burnisher` | jeweller's burnisher | Polishing and closing settings. | Required. |
| `medieval_tool_lapidary_saw` | lapidary saw | Gem and stone blanks. | Required for gemstones. |
| `medieval_tool_lapidary_wheel` | lapidary wheel | Cabochons, beads, seal stones. | Required for gemstones. |
| `medieval_tool_drill_bow` | bow drill | Beads, shells, bone, soft stone. | Required. |

### Clay, stone, masonry, glass, and construction tools

| Suggested stable reference | Item target | Use | Status |
|---|---|---|---|
| `medieval_tool_potters_wheel` | potter's wheel | Thrown ceramic vessels. | Required. |
| `medieval_tool_potters_rib` | potter's rib | Shaping vessels. | Required. |
| `medieval_tool_clay_knife` | clay knife | Clay forming and trimming. | Required. |
| `medieval_tool_wire_cutter` | potter's wire cutter | Cutting clay and pottery stock. | Required. |
| `medieval_tool_press_mold` | press mold | Tiles, lamps, forms. | Required. |
| `medieval_tool_clay_stamp` | clay stamp | Seals, stamped ceramic decoration, marked tiles. | Optional but useful. |
| `medieval_tool_masons_hammer` | mason's hammer | Stone dressing and rough shaping. | Required. |
| `medieval_tool_point_chisel` | point chisel | Stone shaping. | Required. |
| `medieval_tool_tooth_chisel` | tooth chisel | Stone finishing. | Required. |
| `medieval_tool_dressing_axe` | stone dressing axe | Dressed stone and blocks. | Required. |
| `medieval_tool_masons_trowel` | mason's trowel | Mortar, plaster, tile work. | Required. |
| `medieval_tool_masons_line` | mason's line | Walls, doors, gates, buildings. | Required for construction goods. |
| `medieval_tool_masons_square` | mason's square | Construction alignment. | Required. |
| `medieval_tool_earth_rammer` | earth rammer | Rammed earth, floors, foundations. | Optional. |
| `medieval_tool_glass_blowpipe` | glass blowpipe | Glass vessels and lamps. | Required. |
| `medieval_tool_pontil_rod` | pontil rod | Glass shaping. | Required. |
| `medieval_tool_marver_table` | marver table | Glass shaping. | Required. |
| `medieval_tool_glass_jacks` | glass jacks | Opening and shaping glass. | Required. |
| `medieval_tool_glass_shears` | glass shears | Cutting hot glass. | Required. |
| `medieval_tool_glass_blocks` | glass blocks | Shaping glass. | Required. |

### Writing, papermaking, bookbinding, and printing tools

| Suggested stable reference | Item target | Use | Status |
|---|---|---|---|
| `medieval_tool_mould_and_deckle` | mould and deckle | Rag paper sheets. | Required. |
| `medieval_tool_papermakers_vat` | papermaker's vat | Paper pulp and sheet forming. | Required. |
| `medieval_tool_couching_blanket` | couching blanket | Paper sheet transfer. | Required. |
| `medieval_tool_press_felt` | press felt | Pressing paper sheets. | Required. |
| `medieval_tool_lay_press` | lay press | Paper pressing/drying. | Required. |
| `medieval_tool_rag_sorting_knife` | rag-sorting knife | Paper raw stock preparation. | Required. |
| `medieval_tool_rag_beating_trough` | rag-beating trough | Rag pulp preparation. | Required. |
| `medieval_tool_paper_sizing_brush` | sizing brush | Sizing paper and parchment. | Required. |
| `medieval_tool_book_press` | book press | Binding codices. | Required. |
| `medieval_tool_lying_press` | lying press | Bookbinding and trimming. | Required. |
| `medieval_tool_bookbinders_sewing_frame` | bookbinder's sewing frame | Sewing quires. | Required. |
| `medieval_tool_bookbinders_needle` | bookbinder's needle | Sewing books. | Required. |
| `medieval_tool_bookbinders_punch` | bookbinder's punch | Piercing quires and boards. | Required. |
| `medieval_tool_backing_hammer` | backing hammer | Shaping book spine. | Required. |
| `medieval_tool_leather_paring_knife` | leather paring knife | Book cover leather. | Required. |
| `medieval_tool_qalam_cutter` | qalam cutter | Reed pen and qalam shaping. | Required. |
| `medieval_tool_quill_curing_sand` | quill curing sand | Quill preparation. | Required. |
| `medieval_tool_pen_rest` | pen rest | Writing and calligraphy work. | Optional but presentable. |
| `medieval_tool_pen_rack` | pen rack | Writing workshop support. | Optional. |
| `medieval_tool_ruling_board` | ruling board | Manuscript ruling. | Required for codices. |
| `medieval_tool_manuscript_pricker` | manuscript pricker | Quire ruling layout. | Required. |
| `medieval_tool_block_carving_knife` | block carving knife | Woodblock printing. | Required for East Asian printing chain. |
| `medieval_tool_block_clearing_chisel` | block clearing chisel | Woodblock printing. | Required. |
| `medieval_tool_printing_baren` | printing baren | Woodblock printing. | Required. |
| `medieval_tool_ink_dauber` | ink dauber | Woodblock printing. | Required. |
| `medieval_tool_impression_spoon` | impression spoon | Woodblock printing and rubbing. | Required. |

### Food, beverage, medicine, and apothecary tools

| Suggested stable reference | Item target | Use | Status |
|---|---|---|---|
| `medieval_tool_flour_sieve` | flour sieve | Flour/meal preparation. | Required. |
| `medieval_tool_kneading_trough` | kneading trough | Bread, dough, paste. | Required for food items if expanded. |
| `medieval_workshop_bake_oven` | bake oven | Bread and baked goods. | Required for baked goods. |
| `medieval_tool_salting_trough` | salting trough | Meat/fish preservation. | Required. |
| `medieval_tool_smoking_rack` | smoking rack | Meat/fish preservation. | Required. |
| `medieval_tool_oil_press` | oil press | Olive, sesame, canola, nut oils. | Required. |
| `medieval_tool_fruit_press` | fruit press | Must, wine, cider, juice, medicinal syrups. | Required. |
| `medieval_workshop_brew_copper` | brew copper | Brewing, decoctions, large infusions. | Required. |
| `medieval_workshop_mash_tun` | mash tun | Beer/ale brewing. | Required if beverage crafts are included. |
| `medieval_workshop_lauter_tun` | lauter tun | Brewing. | Required for full brewing chain. |
| `medieval_workshop_fermenting_gyle_tun` | fermenting gyle tun | Ale/beer fermentation. | Required. |
| `medieval_tool_mashing_paddle` | mashing paddle | Brewing. | Required. |
| `medieval_tool_mortar_and_pestle` | mortar and pestle | Apothecary, pigments, spices, cooking. | Required. |
| `medieval_tool_medicine_strainer` | medicine strainer | Decoctions, tinctures, poultice stock. | Required. |
| `medieval_tool_ointment_spatula` | ointment spatula | Salves, creams, poultices. | Required. |
| `medieval_tool_cupping_vessel` | cupping vessel | Medical goods if cups are craftable/used. | Required for treatment chain. |
| `medieval_tool_cautery_iron` | cautery iron | Medical and farrier work. | Required if treatment items use cautery. |
| `medieval_tool_suture_needle` | surgical suture needle | Suture kits. | Required. |
| `medieval_tool_surgical_probe` | surgical probe | Treatment kits. | Required. |
| `medieval_tool_forceps` | forceps | Treatment and apothecary work. | Required. |

---

## Production chain sections

### 1. Forestry, carpentry, furniture, doors, containers, and general household goods

**Terminal roots:** logs, branches, poles, firewood, bark, reeds, rushes, bamboo, cane, willow, resin, pitch stock, quarry/mined fasteners where used.

**Intermediate stock:** timber, planks, boards, panels, dowels, staves, hoops, shingles, shafts, handles, pegs, wedges, glue, pitch, oil, leather straps, metal nails/hinges/hasps where needed.

**Finished categories:** chests, coffers, boxes, shelves, cupboards, wardrobes, tables, benches, beds, doors, gates, racks, handles, wooden tool parts, shield boards, scabbard cores, bow staves, book boards.

**Craft structure:**

1. Split logs into billets, planks, poles, and firewood.
2. Season/prepare timber stock where finished quality matters.
3. Produce component stock: staves, panels, pegs, handles, dowels, shafts, boards.
4. Add metal/leather/textile fittings through cross-chain inputs.
5. Assemble finished containers/furniture/doors by category.

**Tool dependencies:** felling axe, splitting axe, adze, drawknife, saw, chisel, auger, clamp, mallet/hammer, plane/rasp, lathe for turned goods, coopering tools for barrels and tubs.

**Initial craft families:**

- make rough timber stock from logs;
- split planks from timber stock;
- shape handles and shafts from timber stock;
- make staves and hoops for coopered goods;
- assemble simple lidded box;
- assemble chest/trunk/coffer;
- assemble open bin/crate/basket hybrid;
- assemble door/gate leaf;
- assemble furniture surface;
- assemble shelf/cupboard/wardrobe;
- assemble barrel/cask/tub with hoops.

### 2. Textiles, clothing, bedding, bags, and soft household goods

**Terminal roots:** wool, flax, hemp, cotton crop, jute, silk cocoons, straw, dye plants, mordants from mineral/agriculture roots, animal hair/fur where woven or felted.

**Intermediate stock:** cleaned fibre, combed wool, flax tow, roving, thread, yarn, skein, cord, cloth, felt, canvas, broadcloth, silk fabric, dyed cloth, fullered cloth, garment panel, padding, quilted panel.

**Finished categories:** clothing, veils, sacks, pouches, cushions, bedding, rugs, curtains, banners, caparisons, textile armour, bandages, sutures, book cloth, lamp wicks, rope/cordage.

**Craft structure:**

1. Clean, comb, hackle, or card raw fibre.
2. Spin fibre into yarn/thread.
3. Twist or ply thread into cord, rope, or sewing thread.
4. Weave yarn into cloth or banding.
5. Dye yarn or cloth where appropriate.
6. Full, nap, shear, tenter, or finish cloth when needed.
7. Cut panels and sew finished goods.

**Tool dependencies:** distaff, spindle, wool combs, hackles, scutching knife, loom, shuttle, warping board, heddle/lease rods, tablet cards, dye vat, strainer, mordant cauldron, fulling trough, tenter frame, shears, sewing needles, awl.

**Initial craft families:**

- clean raw fibre;
- spin yarn;
- twist cord/thread;
- weave plain cloth;
- weave canvas or heavy cloth;
- weave tablet band;
- dye yarn or cloth;
- full and finish wool cloth;
- cut garment panels;
- sew common clothing item;
- sew bag/pouch/sack;
- sew padded/quilted panel;
- sew textile armour or insulation layer;
- make bandage/suture stock.

### 3. Leather, parchment, fur, and animal-product processing

**Terminal roots:** raw hides, thick hides, fur skins, sinew, gut, bone, horn, antler, shell, feathers, tallow, suet, beeswax, honeycomb, raw wool where pastoral, raw milk/eggs where food.

**Intermediate stock:** scraped hide, limed hide, tanned leather, soft leather, hardened leather, leather panel, sole, strap, thong, rawhide cord, parchment skin, parchment sheet, bone blank, horn plate, sinew cord, glue, tallow, wax.

**Finished categories:** shoes, boots, belts, sheaths, scabbards, quivers, pouches, armour, saddlery, harness, book covers, parchment documents, wax tablets, candles, seal wax, jewellery, repair supplies.

**Craft structure:**

1. Clean/scrape hide.
2. Dehair, lime, rinse, and stretch hide.
3. Tan, oil, smoke, taw, or harden leather according to product family.
4. Cut panels, straps, soles, thongs, scales, and armour pieces.
5. Prepare parchment as a separate chain from selected skins.
6. Render fat/tallow and refine wax/glue.
7. Shape bone/horn/shell blanks as needed.

**Tool dependencies:** skinning knife, fleshing knife, hide scraper, tanning beam, tanning rack, dehairing knife, tanning paddle, brain-tanning bucket, awl, shears, creaser, burnisher, bone saw, bone file, parchment frame, lunellum, pumice, pounce bag.

**Initial craft families:**

- scrape raw hide;
- tan leather;
- harden leather panel;
- cut leather strap/thong/sole/panel;
- prepare fur lining;
- prepare parchment sheet;
- render tallow or glue stock;
- prepare sinew cord;
- carve bone/horn blank;
- assemble shoe/boot/belt/sheath/quiver/leather armour.

### 4. Metal mining, smelting, smithing, casting, locks, armour, weapons, and fittings

**Terminal roots:** ores, native metals, flux stone, charcoal, clay, sand, water, quarry stone, fuel.

**Intermediate stock:** crushed ore, roasted ore, bloom, ingot, bar, billet, rod, wire, sheet, plate, rivet, ring, nail, hinge, clasp, buckle, boss, lock blank, key blank, blade blank, weapon head, scale, lamella, chain link.

**Finished categories:** tools, weapons, armour, shields, locks, keys, latches, hardware, jewellery, buckles, fittings, containers, medical/surgical tools, writing tools, seal matrices, lamps.

**Craft structure:**

1. Crush and roast ore.
2. Smelt ore into bloom or ingot.
3. Refine bloom/ingot into bar, billet, or sheet.
4. Draw wire, make rings, rivets, nails, and small fittings.
5. Forge/cast component blanks.
6. Heat treat, grind, polish, and assemble final goods.
7. Build locks, armour, weapons, and jewellery from specialized stock.

**Tool dependencies:** smelting furnace, forge, anvil, hammer, tongs, bellows, crucibles, ore crusher, ore roaster, charging bucket, slag tools, drawplate, grindstone, punches, snips, armour stakes, locksmith kits, quenching trough.

**Initial craft families:**

- crush ore;
- roast ore;
- smelt bloom/ingot;
- forge bar/billet/plate/sheet/wire;
- make nails/rivets/rings;
- make blade/head/boss/hardware blanks;
- assemble simple tool;
- assemble lock/key/latch hardware;
- assemble weapon;
- assemble mail/lamellar/scale/plate armour;
- assemble jewellery settings and metal ornaments.

### 5. Clay, pottery, brick, tile, lime, plaster, stone, and glass

**Terminal roots:** clay, sand, limestone/chalk, gypsum, stone blocks, slate, mineral pigments, ashes, soda ash/potash, water, fuel.

**Intermediate stock:** clay body, slip, greenware, fired ceramic, brick, tile, lime, slaked lime, mortar, plaster, dressed stone, cut slate, glass batch, glass blank, glass rod, glass bead blank, glass pane, glaze stock.

**Finished categories:** vessels, lamps, jars, amphorae, tiles, bricks, construction materials, religious vessels, inkstones, slate tablets, glass vessels, beads, panes, lamps, bottles.

**Craft structure:**

1. Prepare clay body and temper.
2. Form vessel/tile/brick/greenware.
3. Dry, fire, glaze, or seal.
4. Burn limestone/chalk into quicklime and slake into lime or mortar.
5. Dress stone or slate into construction or writing goods.
6. Prepare glass batch, melt, gather, shape, and anneal.

**Tool dependencies:** kiln, potter's wheel, ribs, clay knife, wire cutter, molds, lime kiln, mason's tools, glass furnace/glory hole, blowpipe, pontil, jacks, marver, annealing lehr.

**Initial craft families:**

- prepare clay body;
- form greenware vessel;
- fire ceramic vessel;
- make brick/tile;
- make lime/mortar/plaster;
- dress stone/slate;
- prepare glass batch;
- blow/form glass good;
- make glass bead/pane.

### 6. Writing, administration, books, seals, paper, and manuscript production

**Terminal roots:** hides, rags, flax/hemp/cotton fibre, reeds, feathers, soot/charcoal, minerals, gum/resin, bark, leaves, bamboo, wood, wax, clay, water.

**Intermediate stock:** parchment sheet, bifolium, rag paper sheet, papyrus sheet, palm-leaf strip, birch-bark strip, bamboo slip, wax tablet blank, ink, pigment cake, pounce, quire, book board, cover leather, sewing cord, seal wax, scroll rod.

**Finished categories:** sheets, letters, scrolls, rolls, codices, ledgers, wax tablets, styluses, quills, qalams, brushes, inkstones, seals, document containers, writing kits, archive boxes, bookbinding tools.

**Craft structure:**

1. Prepare writing surface stock: parchment, paper, papyrus, palm leaf, birch bark, bamboo, wax board.
2. Prepare ink and pigment stock.
3. Make implements: quill, reed pen, qalam, brush, stylus, chisel.
4. Assemble writable single-surface items.
5. Assemble books/codices/ledgers/scrolls.
6. Add sealable behaviour where component supports it.
7. Make document containers and archive goods through household/wood/leather chains.

**Tool dependencies:** parchment frame/lunellum/pumice, mould and deckle, papermaker's vat, press felts, lay press, book press, sewing frame, needles, punches, qalam cutter, ruling board, pricker, block-printing tools.

**Initial craft families:**

- prepare parchment sheet;
- prepare rag paper sheet;
- prepare palm-leaf/birch-bark/bamboo writing stock;
- make ink/pigment cake;
- cut quill/qalam/brush/stylus;
- make wax tablet;
- make loose document/sheet/scroll;
- bind codex/ledger;
- make seal matrix;
- make document pouch/tube/box.

### 7. Military goods: weapons, armour, shields, ammunition, tack, and support gear

**Terminal roots:** timber, ore/metal stock, hides, leather, cloth, horn, sinew, feathers, glue, wax, fibre, charcoal, quarry/mined materials.

**Intermediate stock:** weapon shafts, bow staves, bow laminations, fletching, arrow shafts, bolt shafts, points, blades, heads, rings, rivets, mail links, lamellae, scales, shield boards, shield facing, shield boss, padded panels, leather straps, saddle tree, harness straps.

**Finished categories:** melee weapons, bows/crossbows/slings, ammunition, armour, shields, scabbards, sheaths, quivers, racks, horse tack, barding.

**Craft structure:**

1. Make stock and subassemblies through wood/metal/leather/textile chains.
2. Make weapon heads/blades and haft/shaft stock.
3. Assemble weapons with bindings, glue, grips, and fittings.
4. Make ammunition by combining shaft, head, fletching, and stack component behaviour.
5. Make armour by layer family: padded, leather, mail, scale, lamellar, plate-reinforced, horse armour.
6. Make shields from board/hide/metal/boss/strap chains.
7. Make carrying gear from leather/textile/wood stock.

**Tool dependencies:** smithing, weaponsmithing, armouring, woodworking, fletching, bowyer, leatherworking, textile, and cordage tools.

**Initial craft families:**

- make simple spear/javelin/axe/mace from shaft and head;
- make sword/dagger from blade, guard, grip, pommel;
- make bow and bowstring;
- make arrows/bolts/sling bullets as stackable products;
- make shield board/facing/boss assembly;
- make padded armour piece;
- make leather armour piece;
- make mail rings and mail garment;
- make lamellar/scales and laced armour;
- make sheath/scabbard/quiver;
- make horse tack and barding.

### 8. Food, beverage, preservation, medicine, and apothecary production

**Terminal roots:** grain, pulse, fruit, vegetable, herb, spice, oilseed, honey, milk, eggs, meat/fish, salt, water, fuel, medicinal plants, minerals, alcohol/vinegar stock.

**Intermediate stock:** flour, meal, malt, wort, dough, oil, pressed cake, must, wine/ale stock, cheese, butter, salted meat, smoked meat, dried fruit, broth/stock, decoction, tincture, syrup, salve base, poultice stock, bandage stock, suture stock, fumigation stock.

**Finished categories:** food-service items if edible components are later used, medicinal liquids, wrapped doses, salves, poultices, fumigation burners, treatment kits, repair supplies, apothecary containers.

**Craft structure:**

1. Process farm/forage products into edible or medicinal stock.
2. Preserve meat/fish through salting, smoking, drying.
3. Brew, ferment, press, or boil beverage and medicinal liquids.
4. Grind and mix apothecary dry stock.
5. Make drug-bearing wrappers and vessel-loaded products using existing component names.
6. Assemble treatment kits and repair kits using exact repair components.

**Tool dependencies:** quern, mortar and pestle, sieve, oven, salting trough, smoking rack, oil press, fruit press, brewing tools, medicine strainer, spatula, medical/surgical tools.

**Initial craft families:**

- mill grain to flour/meal;
- bake bread-like stock if edible items are implemented;
- press oil;
- press fruit must;
- brew wort and ferment ale/wine-like liquids;
- salt/smoke/dry meat or fish;
- grind herb/pigment stock;
- prepare decoction/tincture/syrup;
- prepare salve/poultice/bandage/suture;
- assemble medical/apothecary item;
- assemble repair-kit supply.

### 9. Jewellery, adornment, devotional ornaments, and luxury small goods

**Terminal roots:** precious metals, copper/bronze/brass/iron, gemstones, glass, shell, bone, horn, amber, jet, wood, flowers, leaves, fibre, silk, wax.

**Intermediate stock:** wire, sheet, bead blank, cabochon, carved bead, setting, clasp, chain link, brooch pin, circlet band, garland string, dried/wilted morph targets.

**Finished categories:** rings, brooches, bracelets, necklaces, earrings, anklets, pins, circlets, garlands, belt ornaments, chain ornaments, signet rings or seal-capable jewellery where implemented.

**Craft structure:**

1. Prepare metal wire/sheet/settings through metal chain.
2. Cut/polish stones, glass, shell, bone, horn, amber, or jet into beads/settings.
3. Make clasps, pins, and chain links.
4. Assemble jewellery by object family.
5. Handle fresh organic jewellery as morphing chains with fresh, wilted, and dried outputs.
6. Use seal-stamp components only where mechanical seal behaviour is intended.

**Tool dependencies:** jeweller's anvil, drawplate, bow drill, burnisher, lapidary saw/wheel, small files, crimping pliers, polishers, needle/thread for stringing, wax/seal tools for signets.

**Initial craft families:**

- draw jewellery wire;
- make bead blank;
- cut/polish cabochon;
- make clasp/pin/setting;
- string beads;
- make ring;
- make brooch/pin;
- make chain/necklace/bracelet;
- make fresh garland and morph chain;
- make functional signet ring or loose seal matrix.

---

## Intermediate stock item targets

These stock items should be considered for full item prototypes before finished crafts are emitted. Not every entry must become a player-visible item if the craft system can handle the stock cleanly as commodity products, but each entry must have an intentional representation.

| Stock family | Full item targets to strongly consider | Commodity-only candidates |
|---|---|---|
| Wood | plank bundle, timber beam, shaft blank, bow stave, stave bundle, shield board, book board, handle blank | rough timber stock, firewood, wood chips, sawdust. |
| Textile | yarn skein, sewing thread, cord bundle, rope coil, plain cloth bolt, canvas bolt, felt sheet, dyed cloth bolt, padding roll | cleaned fibre, roving, loose tow, dye bath. |
| Leather | leather panel, hardened leather panel, leather strap, leather thong, sole blank, leather scale bundle, parchment sheet, rawhide cord | scraped hide, tanning liquor, glue stock. |
| Metal | metal bar, wire coil, sheet metal, armour rings, rivet packet, nail packet, blade blank, spearhead, axe head, buckle blank, hinge blank, key blank | crushed ore, roasted ore, bloom, generic billet stock. |
| Clay/stone/glass | brick stack, tile stack, fired vessel blank, slate tablet blank, glass bead blank, glass pane, inkstone blank | clay body, slip, glass batch, lime, mortar, plaster. |
| Writing | parchment sheet, rag paper sheet, quire, book board pair, scroll rod pair, ink cake, pigment cake, seal-wax stick, wax tablet blank | pulp, pounce, sizing, binder glue. |
| Food/medical | bandage roll, suture thread, splint blank, salve base packet, poultice stock, dried herb packet, repair-supply packet | flour, meal, wort, decoction stock, oil mash. |
| Jewellery | wire coil, bead string, cabochon, brooch pin, setting packet, clasp, chain length | polishing dust, flux, simple metal scrap. |

---

## Craft method pass order

The current empty medieval craft methods are already a sensible high-level split. The recommended implementation order is:

1. `SeedMedievalProductionChainCrafts`
   - terminal-to-intermediate conversion;
   - stock crafts for wood, textile, leather, metal, clay, stone, paper, food, and medicine;
   - reusable tool and workstation crafts;
   - no finished catalogue item claims until stock closure is available.

2. `SeedMedievalClothingCrafts`
   - garment panels, sewing, clothing, footwear, veils, belts, cloth bags;
   - depends on textile and leather stock.

3. `SeedMedievalEquipmentCrafts`
   - weapons, armour, shields, tack, ammunition, military supports;
   - depends heavily on metal, leather, wood, textile, horn/sinew, and glue stock.

4. `SeedMedievalWritingAdministrationCrafts`
   - paper/parchment/surfaces, books, scrolls, pens, seals, document containers;
   - depends on leather/parchment, wood, paper, ink, metal, wax, and textile stock.

5. `SeedMedievalMedicalApothecaryCrafts`
   - medicine delivery items, treatment kits, prosthetics, drug-bearing liquids and wrappers;
   - depends on food/medicine stock and many small crafted vessels/tools.

6. `SeedMedievalJewelleryDevotionalCrafts`
   - jewellery, garlands, signets, devotional small goods where present;
   - depends on metal, wire, beads, stones, glass, flowers, textile thread, wax.

7. `SeedMedievalFurnitureAndContainerCrafts`
   - household containers, furniture, doors, locks, water and light fixtures;
   - depends on wood, metal, leather, textile, ceramic/glass, and lighting stock.

8. `SeedMedievalFoodBeverageCrafts`
   - table-ready food/drink items and preservation chain if/when edible item prototypes are authored;
   - should remain careful about existing liquid and food component support.

9. `SeedMedievalRepairKitCrafts`
   - repair kits and repair supplies;
   - depends on specialist stock items and exact repair-kit components.

10. `SeedMedievalComponentGapCrafts`
    - temporary bridge crafts for component-gap items, tag-gap tools, or transitional stock that does not belong to a stable final category.

---

## Validation and audit plan

A craft-complete catalogue needs tests and a machine-readable audit. Recommended checks:

1. **Catalogue item coverage:** every direct `CreateItem(...)` call in the medieval item partials has at least one craft product that creates its stable reference, except explicitly exempt terminal products if any are later added.
2. **Input closure:** every `StableSimpleItemInput`, exact-item input, or tagged item input used by a medieval craft either points to an item crafted elsewhere or is explicitly listed as terminal/exempt.
3. **Tool existence:** every `TagTool` requirement has at least one seeded item with a matching tag and appropriate portability/location expectation.
4. **Tool craftability:** every required tool item has a craft path unless deliberately classified as a terminal primary-industry item, which should be rare and generally avoided for tools.
5. **Intermediate stock closure:** every named intermediate item or commodity class has either a craft producing it or a terminal-source explanation.
6. **No impossible component claims:** craft outputs reference seeded component names already accepted by the item prototypes.
7. **No retired-catalogue dependency:** medieval crafts do not depend on retired generated catalogue data or removed helper families.
8. **Culture scope:** regional craft variants exist only where production materially differs, not just where output descriptions differ.

Suggested audit output shape:

```text
stable_reference | item_source_file | craft_name | craft_method | immediate_inputs | missing_input_crafts | terminal_inputs | terminal_source_class | terminal_source_owner | missing_terminal_source | required_tools | missing_tool_items | missing_tool_components | missing_tool_tags | missing_component_types | missing_component_prototypes | missing_materials | missing_tags | required_skill | missing_skill_package_entry | owning_resolution_pass | resolution_status
```

This audit can live as a generated markdown or CSV artifact during development. It does not need to be checked in unless it becomes a maintained source of truth.

---

## Known tag/component support notes

The current tag hierarchy already contains many exact tool tags for the first craft pass, including agriculture, armouring, bookbinding, brewing, butchery, calligraphy, construction/masonry, cooking, glassblowing, metalworking, papermaking, parchmentmaking, pottery, smelting, tanning, textilecraft, weaponsmithing, woodblock printing, and woodcrafting.

Important implementation notes:

- Glass tool tags are under `Functions / Tools / Glassblowing Tools`, not `Glassworking Tools`.
- Lapidary, jewellery, and apothecary tool tag branches are now present in `UsefulSeeder.Tags.cs` and the maintained tag export. Craft requirements should use those exact branches instead of broad `Functions / Tools` matches where the tool role matters.
- Locksmithing is unusually well-supported by exact component prototypes. Prefer proper locksmithing tool kits for lock and key fabrication instead of plain inert props.
- Some tag paths include modern tools or later technology, such as chain saws, sewing machines, Hollander beaters, surgical staplers, and modern medical diagnostic devices. The medieval pass must not use those even when the tags exist.
- The shared historic foundation items already seed enough basic workshop apparatus to support early craft prototypes, but they do not cover the full medieval catalogue.

---

## First implementation milestone recommendation

The first production milestone should not try to make all finished items craftable at once. It should land a durable foundation:

1. Add the missing **medieval industry tool items** for the common tool set: felling axe, saws, drawknife, wood chisel, auger, distaff, wool combs, hackle, shuttle, warping board, dye strainer, hide scraper, tanning beam, forge, smelting furnace, ore crusher, crucible, grindstone, parchment frame, mould and deckle, book press, mortar and pestle.
2. Add the first **intermediate stock items**: planks, shafts, yarn, thread, cloth bolt, leather panel, leather strap, parchment sheet, metal bar, wire coil, rivet packet, sheet metal, clay body, fired brick/tile, ink cake, glue stock, wax stick.
3. Add `SeedMedievalProductionChainCrafts` entries to produce those intermediate stocks from terminal resources or earlier stock.
4. Add tests that prove those stock and tool crafts are discoverable and that all tool tags have at least one matching item.
5. Only then begin category-specific finished-good crafts.

A thin first craft PR that only creates finished clothing, weapons, or furniture without this closure layer would likely make later auditing harder.

---

## Open issues for later passes

- Decide whether broad intermediate stock should be represented mainly by commodity products, item prototypes, or both.
- Decide whether high-throughput fixtures should remain `Holdable` like the current moveable catalogue or gain separate non-portable installed variants.
- Decide which physical tool and workstation item prototypes should attach each shared `Tool_*_General` `HandTool` component, while keeping exact craft matching on the functional tag paths.
- Decide how strongly to model regional production differences for East Asian paper, South Asian palm leaf, steppe composite bows, Japanese lamellar armour, and Islamicate paper/qalam/calligraphy goods.
- Decide whether all food and beverage items should become actual edible/drinkable component-bearing goods before their crafts are authored, or whether food-service vessel crafts remain separate from edible food production.
- Decide whether active liquid contents such as ink, medicinal liquids, oils, and beverages should be seeded as craft products, default-loaded container contents, or both.
