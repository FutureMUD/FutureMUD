# FutureMUD Medieval Industry Tool Production Chain Second Pass

**Status:** second-pass production-chain audit after the medieval industry tool and intermediate-stock item catalogue landed.  
**Date:** 3 July 2026.  
**Scope:** identifies which newly seeded tools and workshop apparatus must themselves become craft outputs before the medieval finished-good craft pass can safely proceed.  
**Primary implementation target:** `DatabaseSeeder/Seeders/ItemSeeder.Crafting.Medieval.cs`, beginning with `SeedMedievalProductionChainCrafts` and only then moving to the category-specific medieval craft launchers.

This document does not implement crafts. It records the required dependency closure for the tools and stock now present in `SeedMedievalHouseholdCraftTools` and `SeedMedievalComponentGapItems`.

---

## Baseline findings

The first medieval industry item pass created 168 tool/workshop prototypes and 50 intermediate stock prototypes. That pass deliberately did not create medieval crafts. The important consequence is that most of those tools are now non-terminal items: they exist as seedable items, but they still need craft paths before they can be used as honest prerequisites in later production-chain crafts.

The chain design reference already states the relevant rule: craft tools should be real seeded items, and every non-terminal craft input should be either terminal or produced by another craft. The second pass extends that rule from finished goods to the tool catalogue itself.

### Working rule

Every newly seeded medieval tool or workshop apparatus is treated as **non-terminal** unless it is explicitly classified as an upstream primary-origin fixture. Tools should therefore have craft paths before a craft uses them as prerequisites.

Acceptable exceptions are rare:

- a tool is supplied by the shared historic foundation pass and already has an active cross-era craft path;
- a tool is deliberately modelled as an imported market good for a later economy pass;
- a primary-origin seeder owns the item because it is really a terrain/site fixture rather than a medieval craft output.

---

## Bootstrap and dependency order

The medieval tool chain has a bootstrapping problem: many tools are needed to make other tools. The cleanest solution is to start from shared historic foundation tools and terminal resources, then produce medieval-specific tools in increasing dependency order.

| Phase | Purpose | Example outputs | Required before |
|---:|---|---|---|
| 0 | Confirm terminal resource owners | logs, firewood, hides, wool, flax, ores, clay, sand, limestone, salt, water, beeswax, feathers, reeds | any production-chain craft that names terminal inputs |
| 1 | Use historic tools and terminal stock to make fuel and rough workshop stock | charcoal, rough timber, firewood, clay body, lime stone charge, raw hide stock | forge, furnace, kiln, lime, glass, pottery, leather, and timber chains |
| 2 | Build high-heat and large workshop apparatus | forge, smelting furnace, lime kiln, bake oven, glass glory hole, annealing lehr, papermaker's vat, ropewalk, fulling stocks, presses | advanced metal, glass, paper, book, rope, and cloth production |
| 3 | Make bulk intermediate stock | planks, shafts, cloth, leather panels, iron bars, bronze bars, wire, rivets, bricks, tiles, paper, parchment, glue, wax | hand-tool crafts and finished-item crafts |
| 4 | Make basic portable tools | axes, saws, chisels, augers, files, clamps, needles, shears, scrapers, trowels, punches | specialist tool families and most finished-item crafts |
| 5 | Make specialist trade tools | coopering set, fulling set, papermaking set, bookbinding set, lapidary set, jewellery set, glassworking set, medical instruments | high-skill branch crafts and finished-category crafts |
| 6 | Make toolkits and composed apparatus | locksmith kits, printing tool group, armourer's forming set, surgical set, specialist repair stock | lock, book, jewellery, military, repair, and medical crafts |

### Bootstrap notes

- `historic_workshop_anvil`, `historic_forge_tongs`, `historic_workshop_hammer`, `historic_bellows`, `historic_updraft_kiln`, `historic_drop_spindle`, `historic_textile_shears`, `historic_awl_punch`, `historic_dye_vat`, `historic_tanning_rack`, and both loom variants should be treated as the initial tool layer.
- The first medieval forge and bloomery can use the historic anvil, hammer, tongs, bellows, clay, stone, and charcoal. Once the medieval forge exists, most later metal tools should require it.
- The first medieval felling and splitting axes can be produced from terminal or imported iron bar plus ash/oak handle stock using historic metal tools. Later axes should use the normal metal chain.
- Do not let finished clothing, weapons, household goods, writing goods, jewellery, medical goods, or repair kits require a new medieval tool until the tool has a craft path or an explicit source exemption.

---

## Tool-production closure by family

| Tool family | Newly seeded tools needing craft paths | Production owner | Primary origins or upstream stock | Skill mapping | Priority |
|---|---|---|---|---|---:|
| Forge, smelting, lime, glass, oven, and heat apparatus | `medieval_workshop_forge`, `medieval_workshop_lit_forge`, `medieval_workshop_smelting_furnace`, `medieval_workshop_lit_smelting_furnace`, `medieval_workshop_lime_kiln`, `medieval_workshop_glass_glory_hole`, `medieval_workshop_annealing_lehr`, `medieval_workshop_bake_oven` | `SeedMedievalProductionChainCrafts` | clay, brick, stone, limestone/chalk, charcoal, sand, iron fittings, bellows | Masonry, Pottery, Blacksmithing, Smelting, Glassworking/Glassblowing, Baking | 0 |
| Furnace and ore-service tools | `medieval_tool_charcoal_rake`, `medieval_tool_ore_crusher`, `medieval_tool_ore_roaster`, `medieval_tool_charging_bucket`, `medieval_tool_slag_hammer`, `medieval_tool_slag_skimmer`, `medieval_tool_tap_rod`, `medieval_tool_crucible`, `medieval_tool_crucible_tongs`, `medieval_tool_lye_leaching_barrel` | `SeedMedievalProductionChainCrafts` | wrought iron bar, stone block, clay body, charcoal, ash, oak staves | Blacksmithing, Pottery, Smelting, Masonry, Coopering | 0 |
| Forestry, rough carpentry, and basic wood tools | `medieval_tool_felling_axe`, `medieval_tool_splitting_axe`, `medieval_tool_adze`, `medieval_tool_drawknife`, `medieval_tool_spokeshave`, `medieval_tool_hand_saw`, `medieval_tool_bow_saw`, `medieval_tool_forest_saw`, `medieval_tool_fine_saw`, `medieval_tool_wood_chisel`, `medieval_tool_wood_auger`, `medieval_tool_wood_file`, `medieval_tool_wood_clamp`, `medieval_workshop_pole_lathe` | `SeedMedievalProductionChainCrafts` | iron bar, sheet metal, wire, ash/oak/beech handles, planks, cord, glue | Blacksmithing, Carpentry, Lumberjacking, Bowmaking for bow-stave tools | 0 |
| Coopering tools | `medieval_tool_coopers_adze`, `medieval_tool_coopers_jointer`, `medieval_tool_croze`, `medieval_tool_hoop_driver`, `medieval_tool_bung_borer` | `SeedMedievalProductionChainCrafts`, then reused by `SeedMedievalFurnitureAndContainerCrafts` | iron bar, oak/beech body stock, handle blanks | Coopering, Carpentry, Blacksmithing | 1 |
| Fibre preparation, spinning, weaving, dyeing, and fulling | `medieval_tool_distaff`, `medieval_tool_wool_combs`, `medieval_tool_flax_hackle`, `medieval_tool_scutching_knife`, `medieval_tool_niddy_noddy`, `medieval_tool_warping_board`, `medieval_tool_shuttle`, `medieval_tool_beater_batten`, `medieval_tool_heddle_rod`, `medieval_tool_lease_rod`, `medieval_tool_weaving_reed`, `medieval_tool_tablet_weaving_cards`, `medieval_tool_dye_strainer`, `medieval_tool_mordant_cauldron`, `medieval_tool_dye_stirring_pole`, `medieval_tool_skein_rack`, `medieval_tool_dye_drying_line`, `medieval_tool_fullers_trough`, `medieval_tool_fullers_mallet`, `medieval_workshop_fulling_stocks`, `medieval_tool_tenter_frame`, `medieval_tool_teasel_frame`, `medieval_tool_napping_shears` | `SeedMedievalProductionChainCrafts`, then reused by `SeedMedievalClothingCrafts` | wool, flax/hemp/cotton, beech/oak/ash wood, iron teeth/blades, linen mesh, bronze cauldron stock | Spinning, Weaving, Tailoring, Dyeing, Fulling, Carpentry, Blacksmithing | 1 |
| Basketry and cordage | `medieval_tool_reed_splitter`, `medieval_tool_basket_knife`, `medieval_tool_weaving_bodkin`, `medieval_workshop_ropewalk`, `medieval_tool_rope_hook`, `medieval_tool_rope_top`, `medieval_tool_marlinespike` | `SeedMedievalProductionChainCrafts` | reeds, willow, cane, hemp/flax fibre, oak frame, iron hooks/spikes | Basketry, Ropemaking, Carpentry, Blacksmithing | 1 |
| Leather, parchment, and bone working | `medieval_tool_skinning_knife`, `medieval_tool_fleshing_knife`, `medieval_tool_hide_scraper`, `medieval_tool_tanning_beam`, `medieval_tool_dehairing_knife`, `medieval_tool_tanning_paddle`, `medieval_tool_currying_knife`, `medieval_tool_saddlers_clamp`, `medieval_tool_leather_creaser`, `medieval_tool_burnisher`, `medieval_tool_bone_saw`, `medieval_tool_bone_file`, `medieval_tool_parchment_stretching_frame`, `medieval_tool_parchment_lunellum`, `medieval_tool_parchment_pumice`, `medieval_tool_pounce_bag` | `SeedMedievalProductionChainCrafts`, then reused by `SeedMedievalWritingAdministrationCrafts`, `SeedMedievalEquipmentCrafts`, and repair crafts | hides, bone, horn, pumice/stone, linen, chalk, oak/ash frames, iron blades | Skinning, Butchering, Leathermaking, Parchmentmaking, Bonecarving/Scrimshawing, Carpentry, Blacksmithing | 0 |
| Smithing, weaponsmithing, and armouring | `medieval_tool_smithing_punch_set`, `medieval_tool_grindstone`, `medieval_tool_whetstone`, `medieval_tool_quenching_trough`, `medieval_tool_fuller_tool`, `medieval_tool_tang_punch`, `medieval_tool_sword_vise`, `medieval_tool_crossguard_fixture`, `medieval_tool_armourers_stake`, `medieval_tool_ball_stake`, `medieval_tool_dishing_form`, `medieval_tool_forming_bag`, `medieval_tool_planishing_hammer`, `medieval_tool_raising_hammer`, `medieval_tool_plate_snips`, `medieval_tool_armourers_pliers` | `SeedMedievalProductionChainCrafts`, then reused by `SeedMedievalEquipmentCrafts` | iron bar, steel/iron sheet, stone wheel, oak frames, leather forming bag, sand | Blacksmithing, Weaponcrafting, Armourcrafting, Carpentry, Masonry | 0 |
| Locksmithing, jewellery, and lapidary | `medieval_tool_locksmithing_fabrication_kit`, `medieval_tool_locksmithing_installation_kit`, `medieval_tool_drawplate`, `medieval_tool_jewellers_anvil`, `medieval_tool_jewellers_crimping_pliers`, `medieval_tool_jewellers_burnisher`, `medieval_tool_lapidary_saw`, `medieval_tool_lapidary_wheel`, `medieval_tool_drill_bow` | `SeedMedievalProductionChainCrafts`, then reused by `SeedMedievalJewelleryDevotionalCrafts` and household lock crafts | iron/bronze/brass bar, wire, leather roll, stone wheel, abrasive grit, gem rough, shell, amber, jet | Locksmithing, Goldsmithing, Silversmithing, Gemcraft, Lapidary, Blacksmithing, Carpentry | 1 |
| Pottery, masonry, and construction tools | `medieval_tool_potters_wheel`, `medieval_tool_potters_rib`, `medieval_tool_clay_knife`, `medieval_tool_wire_cutter`, `medieval_tool_press_mold`, `medieval_tool_clay_stamp`, `medieval_tool_masons_hammer`, `medieval_tool_point_chisel`, `medieval_tool_tooth_chisel`, `medieval_tool_masons_trowel`, `medieval_tool_masons_line`, `medieval_tool_masons_square` | `SeedMedievalProductionChainCrafts` | clay, fired clay, stone, oak/beech frames, iron blades/chisels, hemp line | Pottery, Masonry, Quarrying, Constructing, Blacksmithing, Carpentry | 1 |
| Glassworking tools | `medieval_tool_glass_blowpipe`, `medieval_tool_pontil_rod`, `medieval_tool_marver_table`, `medieval_tool_glass_jacks`, `medieval_tool_glass_shears`, `medieval_tool_glass_blocks` | `SeedMedievalProductionChainCrafts`, then reused by glass vessel/light-source crafts | iron rods, stone table, beech blocks, water, charcoal, glass batch | Glassblowing, Glassworking, Blacksmithing, Masonry, Carpentry | 1 |
| Papermaking, bookbinding, calligraphy, and printing | `medieval_tool_mould_and_deckle`, `medieval_tool_couching_blanket`, `medieval_tool_press_felt`, `medieval_tool_lay_press`, `medieval_tool_rag_sorting_knife`, `medieval_tool_paper_sizing_brush`, `medieval_tool_bookbinders_needle`, `medieval_tool_bookbinders_punch`, `medieval_tool_backing_hammer`, `medieval_tool_leather_paring_knife`, `medieval_tool_qalam_cutter`, `medieval_tool_quill_curing_sand`, `medieval_tool_ruling_board`, `medieval_tool_manuscript_pricker`, `medieval_tool_block_carving_knife`, `medieval_tool_block_clearing_chisel`, `medieval_tool_printing_baren`, `medieval_tool_ink_dauber`, `medieval_tool_impression_spoon`, plus the papermaker's vat, rag trough, book press, lying press, and sewing frame apparatus | `SeedMedievalProductionChainCrafts`, then reused by `SeedMedievalWritingAdministrationCrafts` | rags, linen, wool felt, beech/oak frames, wire/reed mesh, iron needles/knives, sand, bone, bamboo, ink stock | Papermaking, Bookbinding, Calligraphy, Scribing, Woodblock Printing, Carpentry, Blacksmithing | 1 |
| Food, brewing, apothecary, and medical tools | `medieval_tool_flour_sieve`, `medieval_tool_kneading_trough`, `medieval_tool_salting_trough`, `medieval_tool_smoking_rack`, `medieval_tool_oil_press`, `medieval_tool_fruit_press`, `medieval_tool_mashing_paddle`, `medieval_tool_mortar_and_pestle`, `medieval_tool_medicine_strainer`, `medieval_tool_ointment_spatula`, `medieval_tool_cupping_vessel`, `medieval_tool_cautery_iron`, `medieval_tool_suture_needle`, `medieval_tool_surgical_probe`, `medieval_tool_forceps`, plus brew copper, mash tun, and gyle tun apparatus | `SeedMedievalProductionChainCrafts`, then reused by medical, food, and repair crafts | wood, bronze, stone, linen mesh, glass, horn, iron/bronze fine metal stock, grain/fruit/herb origins | Milling, Baking, Brewing, Cooking, Apothecary/Pharmacology, Surgery/Patient Care, Carpentry, Blacksmithing, Glassblowing | 1 |

---

## Missing or newly recommended tool targets

The first catalogue closed most of the originally proposed tool targets, but the second pass found tool or apparatus gaps that should be either added as item prototypes or explicitly ruled out before craft authoring relies on them.

| Proposed reference | Item or source gap | Why it matters | Recommended owner | Status |
|---|---|---|---|---|
| `medieval_workshop_charcoal_clamp` | charcoal clamp, covered pit, or charcoal-burning site | Charcoal is a root fuel for forge, smelting, lime, glass, and ceramic chains. The craft can be a site/process rather than a portable item, but it must be represented. | Primary production / `SeedMedievalProductionChainCrafts` | Add or explicitly make charcoal a terminal primary-industry product. |
| `medieval_tool_spinners_weights` | spare spindle weights | Useful for scaling spindle/loom repair and textile-tool crafting. | Textile production | Optional add. Existing historic drop spindle can cover basic spinning. |
| `medieval_tool_wheelwright_clamp` | wheelwright's clamp | Required only if carts, wheels, and large round assemblies become craftable. | Woodcrafting / future vehicle or wagon pass | Defer unless wheel/carte goods enter scope. |
| `medieval_tool_tenon_cutter` | tenon cutter | Useful for high-throughput chair, wheel, peg, and framed-furniture crafts. | Woodcrafting | Optional; can use chisel/auger/drawknife at first. |
| `medieval_tool_brain_tanning_bucket` | brain-tanning bucket | Only needed if the leather chain distinguishes brain/oil tanning from bark/alum/tawing. | Leather production | Optional; otherwise use tanning paddle/trough/vat. |
| `medieval_tool_dressing_axe` | stone dressing axe | Useful for dressed-stone and block work where hammer/chisel is insufficient. | Masonry/quarrying | Recommended if construction crafts consume dressed stone. |
| `medieval_tool_earth_rammer` | earth rammer | Needed for rammed earth, floors, packed foundations, and clay yard features. | Construction / primary earthworks | Optional until construction-fixture crafts exist. |
| `medieval_tool_pen_rest` | pen rest | Scriptorium support object, not required for closure. | Writing goods | Defer or keep as finished household/writing accessory. |
| `medieval_tool_pen_rack` | pen rack | Scriptorium support object, not required for closure. | Writing goods | Defer or keep as finished household/writing accessory. |
| `medieval_workshop_lauter_tun` | lauter tun | Brewing chain currently has mash tun and gyle tun but no lautering vessel. | Brewing | Add if full beer/ale craft closure is implemented. |
| `medieval_tool_mining_pick` | mining pick | If ore and stone extraction becomes craftable rather than terminal-source seeded, extraction should require a mining tool. | Primary mining source owner | Add in a primary-origin pass or mark mining outputs terminal. |
| `medieval_tool_quarry_wedges` | quarry wedges and feathers | If stone blocks/slate/limestone extraction becomes craftable, quarry tools are needed. | Primary quarrying source owner | Add in a primary-origin pass or mark quarry outputs terminal. |
| `medieval_tool_spade` / `medieval_tool_shovel` | digging tool | Clay, sand, salt, earthwork, ditching, and construction-origin crafts need an extraction tool if modelled. | Primary earthworks/clay/salt source owner | Add in primary-origin pass or leave extraction terminal. |
| `medieval_tool_salt_pan` / `medieval_tool_salt_rake` | salt production tools | Salt is needed for preservation, tanning, food, medicine, and trade. Saltworking needs source ownership. | Saltworking primary source owner | Add only if salt production is craftable. |

---

## Skill pass findings

The merged prerequisite pass already covers most specialist skills needed by the implemented tool catalogue. Existing skill entries cover the main craft families: armourcrafting, weaponcrafting, blacksmithing, bowmaking, fletching, pottery, weaving, spinning, dyeing, glassworking, glassblowing, lapidary, carpentry, basketry, coopering, ropemaking, fulling, lumberjacking, masonry, bonecarving, tailoring, cobbling, locksmithing, wheelmaking, candlemaking, leathermaking, parchmentmaking, papermaking, bookbinding, calligraphy, scribing, woodblock printing, brewing, cooking, smelting, mining, quarrying, farming, herbalism, butchering, and skinning.

### New skill decisions recommended

| Proposed skill | Need | Recommendation |
|---|---|---|
| Charcoal Burning | Distinct fuel-production expertise for charcoal clamps and pits. | Add only if the project wants fine-grained primary-industry skills. Otherwise map charcoal crafts to `Lumberjacking` plus `Smelting` or `Blacksmithing`. |
| Limeburning | Lime kiln work bridges masonry, tanning, parchment, plaster, and mortar. | Do not add yet. Use `Masonry` or `Smelting`/`Pottery` depending craft shape. Add later only if lime becomes a major profession. |
| Tanning | Current code has `Leathermaking`, but tool names and chain language use tanning heavily. | Prefer `Leathermaking` for now. Add `Tanning` only if you want to separate raw-hide processing from leather goods. |
| Saddlery | Saddler's clamp, tack, harness, quivers, belts, and horse gear are significant. | Do not add yet. Use `Leathermaking`, `Cobbling`, and `Riding`/equipment craft context. Add later if animal tack becomes a deep branch. |
| Surgical Instrument Making | Cupping vessels, cautery irons, probes, forceps, and needles are tool outputs, but production is metal/glass/bone work. | Do not add yet. Use `Blacksmithing`, `Goldsmithing`, `Glassblowing`, and medical-use skills only when the craft applies the tool, not when the tool is forged. |
| Mining Toolmaking | Mining tools are just metal/wood tools. | Do not add. Use `Blacksmithing` and `Carpentry`. |
| Farriery | Cautery irons and horse tack overlap farrier work. | Defer. Existing `Blacksmithing`, `Leathermaking`, and `Riding` are sufficient for current item scope. |

---

## New or confirmed production lines

The implemented catalogue requires the following production lines before category-specific finished goods can close.

| Production line | Minimum outputs | Primary source owner | Notes |
|---|---|---|---|
| Charcoal and solid fuel | charcoal, ash, clinker/slag by-products if used | Primary production / forestry | Must precede high-heat work. Decide whether charcoal is terminal or craftable via charcoal clamp/pit. |
| Forestry and timber conversion | logs, firewood, planks, beams, shafts, bow staves, shield boards, handle blanks | Forestry / lumberjacking | Needed by almost every tool family. |
| Ore preparation and smelting | crushed ore, roasted ore, bloom/ingot, iron bar, bronze bar, sheet metal, wire, rivets, nails, rings | Mining + smelting | Must precede most metal tools, weapons, armour, jewellery, locks, and medical instruments. |
| Lime, mortar, plaster, and construction mineral work | quicklime, slaked lime, mortar, plaster, dressed stone, bricks, roof tiles | Quarrying / masonry / pottery | Needed for kiln/furnace building, masonry, parchment, tanning, and construction goods. |
| Clay and ceramic preparation | clay body, greenware, fired clay, ceramic crucibles, press moulds, bricks, tiles | Clay digging + pottery | Required before crucibles and several apparatus rows are craftable. |
| Glass batch and glass shaping | glass batch, glass blank, pane blank, bead blank, cupping vessel, glassware | Sand/ash/flux sources + glassworking | Requires fuel and annealing infrastructure. |
| Textile fibre to cloth and cordage | yarn, thread, cord, rope, cloth, felt, canvas, bandage rolls, couching blankets, press felts | Agriculture/pastoral + textile crafts | Tool chain itself consumes textile stock in strainers, lines, blankets, bags, and medical stock. |
| Hide, leather, parchment, bone, and glue | leather panels, straps, thongs, parchment sheets, glue cakes, bone/horn blanks | Butchery + leathermaking/parchmentmaking | Needed by tool grips, covers, saddlery, bookbinding, armour, and repair supplies. |
| Paper, book, writing, and printing stock | rag paper, book boards, ink cake, pounce, seal wax, wax tablet blanks | Paper/parchment/writing production | Required before many writing tools and document goods are craftable. |
| Jewellery and lapidary stock | wire, bead blanks, cabochons, settings, clasps, chain lengths | Mining/quarry/gem + metal/lapidary | Required by jewellery, signet, devotional, and luxury-small-goods crafts. |
| Food, beverage, apothecary, and medical stock | flour/meal, oil, must, wort, salve base, poultice stock, suture thread, splint blanks | Agriculture/forage/butchery + food/medicine crafts | Required for medical and repair items and some tool-use chains. |

---

## Primary origins to add or explicitly assign

The finished craft chain should not silently invent raw materials. The following terminal origins need either an upstream seeder owner or an explicit terminal-source exemption.

| Origin class | Required terminal outputs | Current concern | Recommended owner |
|---|---|---|---|
| Forestry | oak, ash, beech, birch, pine, willow, cedar/cypress where regionally used, poles, firewood, bark/tanbark, resin/pitch | Tool handles, planks, boards, presses, vats, racks, charcoal, bark tanning | Forestry / lumberjacking source pass |
| Managed fibre agriculture | flax, hemp, cotton crop, jute, straw, reed if cultivated, silk cocoons | Yarn, thread, cloth, rope, paper rag inputs, bandages, couching blankets | Agriculture / sericulture source pass |
| Pastoral and butchery | wool, hides, thick hides, rawhide, sinew, gut, bone, horn, antler, shell, feathers, tallow/suet | Leather, parchment, glue, suture, tool handles, quills, bone/burnishing tools | Pastoral/butchery/skinning source pass |
| Apiary | beeswax, honeycomb, propolis if used | seal wax, wax tablet blanks, candles, salve base | Apiary / beekeeping source pass |
| Mining | iron ore, copper ore, tin ore, lead ore, silver/gold ores, fuel/flux minerals where used | Smelting, bars, wire, rivets, settings, medical instruments, lock work | Mining source pass |
| Quarrying and stone | limestone, chalk, gypsum, slate, pumice, sandstone/granite, whetstone/grindstone blanks, gem rough | lime, mortar, plaster, writing tablets, whetstones, grindstones, lapidary stock | Quarrying / gem source pass |
| Clay, sand, and earth | clay, grog/temper, silica sand, soda/potash ash, loam, building earth | pottery, crucibles, brick, tile, glass batch, rammed earth | Clay and glass-source pass |
| Salt and brine | salt, salt water/brine, sea salt, rock salt | preservation, tanning, medicine, trade goods | Saltworking source pass |
| Water | potable water, river/lake/spring water, salt water where relevant | brewing, dyeing, tanning, papermaking, medicine, clay, lime | Existing water/liquid source pass |
| Forage and wild plants | dye plants, medicinal herbs, reeds, rushes, gum/resin, bark, flowers, berries/nuts, wild fibres | dyes, medicine, basketry, ink binders, garlands, pounce/binder recipes | Forage/herbalism source pass |
| Marine and riverine | fish, shell, shell sand, pearls/mother-of-pearl if used, fish glue/isenglass if later used | shell tools, jewellery, glue, preservation, food | Fishing/forage/butchery source pass |

---

## Recommended next implementation slice

The next craft PR should be a **tool-chain foundation PR**, not a finished-goods craft PR.

Recommended minimal contents:

1. Add or explicitly exempt charcoal production and at least the primary-origin tool decision for mining, quarrying, clay, sand, salt, and forestry.
2. Add craft families for first-tier stock: charcoal, planks, handle blanks, iron bar, bronze bar, wire, rivets, clay body, bricks, leather panel, parchment sheet, yarn/thread, cloth, glue cake, seal wax, and bandage stock.
3. Add craft families for first-tier tools: felling axe, saw, chisel, auger, drawknife, hide scraper, tanning beam, parchment frame, forge, smelting furnace, crucible, grindstone, mould and deckle, book press, mortar and pestle, and surgical needle/probe.
4. Add audit tests proving that every tool tag required by the stock crafts has at least one seeded item and that every exact tool item used by a craft is either crafted or explicitly source-exempt.
5. Defer finished clothing, weapon, armour, household, jewellery, medical, and writing item crafts until the tool-chain foundation passes.

---

## Validation updates to carry forward

Add these checks to the craft-completeness audit when craft rows are created:

- `required_tools` should include both functional tag requirements and exact apparatus references.
- `tool_source_status` should classify each tool as `historic_foundation`, `medieval_tool_crafted`, `primary_origin_exempt`, or `missing`.
- `tool_craft_phase` should record the phase number from this document so a craft cannot require a later-phase tool.
- `primary_origin_owner` should be populated for every terminal source class.
- `skill_mapping_status` should flag any craft using a skill not present in `SkillPackageSeeder.cs`.

If the implementation cannot support those audit columns immediately, include the same facts in a generated markdown table until a machine-readable audit exists.
