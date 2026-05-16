# Antiquity Furniture and Container Crafting Suite

This document is the implementation plan for making the antiquity furniture, storage, vessel, tableware, lighting, and household furnishing items craftable from `ItemSeeder.Rework.cs` and `ItemSeeder.Rework.Antiquity.cs`.

The current target inventory is:

- `SeedAntiquityContainers`: 243 final item prototypes.
- `SeedAntiquityHouseholdFurniture`: 89 final item prototypes.
- Total coverage target: 332 final item crafts, plus the shared tools and commodity/intermediate crafts required to support them.

The implementation lives in `DatabaseSeeder/Seeders/ItemSeederCrafting.AntiquityHousehold.cs`, with supporting stock tools in `ItemSeeder.Rework.AntiquityHouseholdTools.cs`, new commodity/tool tags in `UsefulSeeder.Tags.cs`, and new skills in `SkillPackageSeeder.cs`.

## Design Goals

1. Every item currently seeded by `SeedAntiquityContainers` and `SeedAntiquityHouseholdFurniture` should have a stock craft path.
2. Commodity inputs should be preferred over full-item inputs wherever the input is a mass of material, fittings, fasteners, panels, cloth, clay, wax, glass, metal, reed, leather, or stone stock.
3. Full items should be used for tools and final products. Full item intermediates should only be used where the object is worth exposing as its own gameplay item.
4. Final crafts should be knowledge-gated so builders can control which shared techniques and which cultural household suites appear in a game.
5. The implementation should be testable by comparing the live target catalogue against a dynamic craft discovery pass that covers all antiquity items tagged under `Market / Household Goods / ...`.

## Implementation Plan

1. Discover the target item groups dynamically.
   - Rather than maintaining a 332-row stable-reference dictionary, the implemented craft suite discovers antiquity rework items whose seeded tags sit under `Market / Household Goods / ...`.
   - This keeps the coverage tied to the furniture/container catalogue itself and means future household additions are craftable as long as they are tagged consistently.
   - Focused tests assert the current target methods still contain 243 container and 89 household-furniture prototypes, and that the craft suite uses household market-tag discovery.

2. Seed missing tool prototypes.
   - Add period-appropriate tools in `SeedAntiquityHouseholdFurniture` or a new nearby helper such as `SeedAntiquityHouseholdCraftTools`.
   - Reuse existing tags where present, such as `Hand Saw`, `Wood Chisel`, `Adze`, `Wood Auger`, `Cooper's Adze`, `Basket Knife`, `Potter's Wheel`, `Blowpipe`, `Sewing Needle`, `Leather Stitching Pony`, `Kiln`, and `Lacquerer's Brush`.
   - Keep tools as full items so crafts can require them through `TagTool`.

3. Add the few missing tags needed for commodity stock.
   - Put new craft-facing commodity tags in `UsefulSeeder.Tags.cs`, under `Material Functions` unless a better existing parent already exists.
   - Prefer broad reusable tags over one-off tags tied to a single product.

4. Add the missing skills.
   - Existing skills cover most of the work: `Carpentry`, `Pottery`, `Glassworking`, `Blacksmithing`, `Silversmithing`, `Leathermaking`, `Tailoring`, `Weaving`, `Dyeing`, `Masonry`, `Scrimshawing`, `Candlemaking`, `Lumberjacking`, and `Smelting`.
   - Add `Basketry`, `Coopering`, `Ropemaking`, and `Lacquerwork` to the functional craft skill package.
   - Do not add a separate `Woodcarving` skill in this pass; use `Carpentry` with higher minimums and specialist tools unless later builder feedback asks for a narrower split.

5. Add shared upstream commodity crafts first.
   - Create worked timber, planks, panels, pegs, staves, hoops, basketry splints, reed matting, leather panels, wick stock, clay bodies, bisque blanks, glass vessel blanks, glass panes, cast vessel blanks, stone blanks, inlay stock, lacquer finish, and paint/pigment stock.
   - Reuse the existing textile chain for cloth, yarn, dyeing, fulling, and felt wherever possible.

6. Add final product crafts by family.
   - Each final craft produces one item through a direct simple product helper, or a direct variable product helper that copies `Colour` and `Fine Colour` from the first commodity input where the prototype has variable descriptions.
   - Culturally named products use culture-specific household knowledge.
   - Shared generic products use `Ancient Household Crafting`.

7. Add verification.
   - Add DatabaseSeeder unit tests that assert the target catalogue counts, dynamic household discovery, commodity tags, knowledge gates, tool tags, and skill names.
   - Run the focused ItemSeeder test filter and `git diff --check`.

## Knowledge Gates

Use one knowledge gate per craft, because the stock `AddCraft` knowledge overload is intentionally simple. Shared upstream crafts use technique knowledge, and final culturally named forms use cultural household knowledge. The upstream material chain already gives builders another way to control supply.

| Knowledge | Used For |
| --- | --- |
| `Ancient Household Crafting` | Shared generic furniture, containers, tableware, lamps, furnishings, and simple household goods. |
| `Ancient Woodworking and Joinery` | Worked timber, planks, pegs, furniture frames, shelves, tables, beds, desks, screens, cabinets, and wood tableware. |
| `Ancient Basketry` | Willow, papyrus, rush, reed, wicker, plaited, and basketry-adjacent goods. |
| `Ancient Coopering` | Dry casks, wet casks, barrels, rundlets, hogsheads, hooped containers, and stave-and-hoop stock. |
| `Ancient Leather Containers` | Pouches, bags, packs, waterskins, cases, skins, satchels, and leather-covered composite vessels. |
| `Ancient Ceramic Vesselmaking` | Earthenware, terracotta, fired clay, ceramic, glazed, slipped, bucchero, and painted clay vessels. |
| `Ancient Glassworking` | Glass vessels, cosmetic bottles, glass bowls, glass-fronted display cases, and glass pane stock. |
| `Ancient Metal Vesselmaking` | Bronze, iron, silver, and gold vessels, stands, lamps, bowls, tableware, braziers, and cast ornaments. |
| `Ancient Stone Bone and Horn Carving` | Alabaster, calcite, ivory, horn, and carved stone household vessels or boxes. |
| `Ancient Lighting and Heating` | Candles, torches, oil lamps, braziers, fire baskets, incense burners, and related wick/pitch stock. |

Culture-specific final forms should use:

| Culture Knowledge | Stable Reference Cues |
| --- | --- |
| `Hellenic Household Crafting` | `hellenic`, `kylix`, `skyphos`, `kantharos`, `phiale`, Hellenic painted pinakes. |
| `Roman Household Crafting` | `roman`, `italic`, `terra_sigillata`, `samian`, `poculum`, `lanx`, `dressel`. |
| `Egyptian Household Crafting` | `egyptian`, `black_topped`, `calcite`, Egyptian alabaster/glass forms. |
| `Kushite Household Crafting` | `kushite`, `nubian`, `meroitic`, `kerma`. |
| `Punic Household Crafting` | `punic`, Punic red slip, narrow Punic amphorae, Punic glass/tableware. |
| `Persian Household Crafting` | `persian`, lotiform cups, Persian metal tableware, court vessels. |
| `Etruscan Household Crafting` | `etruscan`, `bucchero`, Etruscan bronze display/tableware. |
| `Anatolian Household Crafting` | `anatolian`, Anatolian rhyta, red-burnished and painted forms. |
| `Scythian-Sarmatian Household Crafting` | `scythian`, `steppe`, riding packs, gorytoi, steppe skins and metal vessels. |
| `Celtic Household Crafting` | `celtic`, Celtic wooden, bronze, horn, and feast tableware. |
| `Germanic Household Crafting` | `germanic`, Germanic wooden, horn, and bread tableware. |

## Skill Additions

| Skill | Imperative Name | Group | Formula | Used For |
| --- | --- | --- | --- | --- |
| `Basketry` | `Basketry` | `Crafting` | `min(99,3*int + 2*wil)` | Baskets, reedwork, wicker, papyrus plaiting, rush seating, reed matting. |
| `Coopering` | `Cooper` | `Crafting` | `min(99,3*int + 2*wil)` | Casks, barrels, rundlets, hogsheads, stave preparation, hooping, wet/dry cask variants. |
| `Ropemaking` | `Ropemaker` | `Crafting` | `min(99,3*int + 2*wil)` | Rope, cordage, tied bundles, wicks, sling/strap stock where this is the primary process. |
| `Lacquerwork` | `Lacquerer` | `Crafting` | `min(99,3*int + 2*wil)` | Lacquered boxes, painted/lacquered finishes, polished lacquer layers. |

Existing skills remain the default for their domains:

| Existing Skill | Use |
| --- | --- |
| `Carpentry` | Furniture, cabinets, shelves, tables, beds, wooden boxes, carved screens, wooden tableware. |
| `Pottery` | Clay, earthenware, terracotta, ceramic, bucchero, glazed, slipped, and painted vessels. |
| `Glassworking` | Glass vessels, pane stock, glass bottles, glass bowls, and display-case glass. |
| `Blacksmithing` | Bronze, wrought-iron, and base-metal vessel/stand/lamp/brazier work. |
| `Silversmithing` | Silver and gold cups, bowls, platters, chargers, and fine tableware. |
| `Leathermaking` | Leather pouches, skins, bags, cases, waterskins, saddlebag packs, straps, and hide linings. |
| `Tailoring` | Sewn cloth bags, cushions, curtains, bolsters, blankets, tablecloths, textile screens, and cloth covers. |
| `Weaving` | Reed matting, rugs, carpets, hangings, cloth furnishing panels, and textile upstream support. |
| `Masonry` | Stone/alabaster shaping where the object is treated as stonecraft. |
| `Scrimshawing` | Horn and ivory vessels or inlay details. |
| `Candlemaking` | Beeswax candles and simple wax stock. |

## Commodity Tags

Prefer existing tags where they already communicate the material function:

- Existing material/function tags to reuse: `Worked Timber`, `Glass Panes`, `Lamp Oil`, `Candlemaking Wax`, `Reeds`, `String`, `Tie`, `Glue`, `Nail`, `Rivet`, `Peg`, `Meltable`, `Bead Stock`, `Garment Cloth`, `Spun Yarn`, `Fulled Cloth`, and `Prepared Textile Fibre`.

Add the following reusable commodity tags:

| Tag | Parent | Purpose |
| --- | --- | --- |
| `Household Craft Stock` | `Material Functions` | Parent for common household craft commodity stock. |
| `Furniture Timber Stock` | `Household Craft Stock` | Squared or dressed timber for furniture frames. |
| `Furniture Panel Stock` | `Household Craft Stock` | Boards or panels for chests, cabinets, boxes, and screens. |
| `Carved Wood Stock` | `Household Craft Stock` | Wood reserved for carved or high-quality furniture details. |
| `Coopered Staves` | `Household Craft Stock` | Prepared staves for dry and wet casks. |
| `Hoop Stock` | `Household Craft Stock` | Metal or wood hoop material for casks and barrels. |
| `Basketry Splint` | `Household Craft Stock` | Split willow, reed, papyrus, or similar basketry stock. |
| `Reed Matting` | `Household Craft Stock` | Flat woven reed or papyrus matting for pallets, mats, and seats. |
| `Prepared Leather Panel` | `Household Craft Stock` | Cut leather panels for pouches, packs, cases, and waterskins. |
| `Lamp Wick` | `Household Craft Stock` | Wick stock for lamps, candles, and torches. |
| `Prepared Pitch` | `Household Craft Stock` | Pitch or resin stock for torches and sealed containers. |
| `Pottery Clay Body` | `Household Craft Stock` | Prepared clay body ready for vessel forming. |
| `Wet Vessel Blank` | `Household Craft Stock` | Formed but unfired clay vessels. |
| `Bisque Vessel Blank` | `Household Craft Stock` | First-fired clay vessels ready for slip/glaze/finish. |
| `Glass Batch` | `Household Craft Stock` | Prepared glassmaking batch. |
| `Glass Vessel Blank` | `Household Craft Stock` | Formed glass bodies before finishing. |
| `Cast Vessel Blank` | `Household Craft Stock` | Rough cast metal vessel or stand bodies. |
| `Stone Vessel Blank` | `Household Craft Stock` | Rough stone, alabaster, calcite, ivory, or horn vessel blanks. |
| `Inlay Stock` | `Household Craft Stock` | Ivory, shell, glass, stone, metal, or coloured detail stock. |
| `Paint Pigment` | `Household Craft Stock` | Pigment stock for painted vessels, plaques, and furniture. |
| `Lacquer Finish` | `Household Craft Stock` | Lacquer stock for lacquered boxes and polished finishes. |

Add the following tool tags if they do not already exist:

| Tag | Parent | Purpose |
| --- | --- | --- |
| `Candlemaking Tools` | `Tools` | Parent for reusable candle-making tools. |
| `Candle Mould` | `Candlemaking Tools` | Required by moulded candle crafts. |
| `Casting Mould` | `Metalworking Tools` | Required by cast metal vessel, lamp, stand, and fitting crafts. |
| `Bow Drill` | `Woodcrafting Tools` | Shared boring tool for wood, horn, ivory, and soft stone work. |

## Tool Prototypes

The following full item tool prototypes should be seeded before the craft suite. Names are suggested stable references; exact descriptions can follow the style already used for textile tools.

| Stable Reference | Tags | Used By |
| --- | --- | --- |
| `antiquity_bronze_hand_saw` | `Hand Saw`, `Saws`, `Woodcrafting Tools` | Plank cutting, furniture, shelving, boxes. |
| `antiquity_bronze_adze` | `Adze`, `Woodcrafting Tools` | Shaping boards, bowls, planks, furniture frames. |
| `antiquity_bronze_wood_chisel` | `Wood Chisel`, `Chisel`, `Woodcrafting Tools` | Joinery, carving, inlay recesses. |
| `antiquity_bronze_wood_auger` | `Wood Auger`, `Woodcrafting Tools` | Peg holes, dowel holes, coopering bungs. |
| `antiquity_wooden_smoothing_plane` | `Planer`, `Woodcrafting Tools` | Dressed timber and finished furniture surfaces. |
| `antiquity_wooden_joinery_clamp` | `Wood Clamp`, `Clamp`, `Woodcrafting Tools` | Panel glue-ups, cases, doors, furniture frames. |
| `antiquity_bow_drill` | `Bow Drill`, `Woodcrafting Tools`, `Stoneworking Tools` | Fine boring in wood, bone, horn, stone, and inlay. |
| `antiquity_wood_rasp` | `Rasp`, `Wood File`, `Woodcrafting Tools` | Carving, shaping, smoothing. |
| `antiquity_coopers_adze` | `Cooper's Adze`, `Coopering Tools` | Staves and casks. |
| `antiquity_coopers_croze` | `Croze`, `Coopering Tools` | Barrel head grooves. |
| `antiquity_hoop_driver` | `Hoop Driver`, `Coopering Tools` | Driving hoops onto casks/barrels. |
| `antiquity_bung_borer` | `Bung Borer`, `Coopering Tools` | Wet casks and liquid barrels. |
| `antiquity_basket_knife` | `Basket Knife`, `Basketry Tools` | Basketry splints and plaiting. |
| `antiquity_reed_splitter` | `Reed Splitter`, `Basketry Tools` | Reed/papyrus/willow stock preparation. |
| `antiquity_weaving_bodkin` | `Weaving Bodkin`, `Basketry Tools` | Tight basket and reed mat weaving. |
| `antiquity_packing_bone` | `Packing Bone`, `Basketry Tools` | Packing woven rows tight. |
| `antiquity_leather_awl_punch` | `Awl Punch`, `Leatherworking Tools` | Pouches, skins, cases, strap holes. |
| `antiquity_leather_stitching_pony` | `Leather Stitching Pony`, `Leatherworking Tools` | Sewn leather goods. |
| `antiquity_leather_edge_beveller` | `Edge Beveller`, `Leatherworking Tools` | Finished leather edges. |
| `antiquity_slow_potters_wheel` | `Potter's Wheel`, `Pottery Tools` | Cups, bowls, jars, amphorae. |
| `antiquity_clay_knife` | `Clay Knife`, `Pottery Tools` | Cutting clay bodies and vessels. |
| `antiquity_potters_rib` | `Potter's Rib`, `Pottery Tools` | Smoothing vessel walls. |
| `antiquity_loop_tool` | `Loop Tool`, `Pottery Tools` | Trimming feet, rims, and reliefs. |
| `antiquity_wire_cutter` | `Wire Cutter`, `Pottery Tools` | Cutting vessels from wheel and clay blocks. |
| `antiquity_clay_stamp` | `Clay Stamp`, `Pottery Tools` | Moulded or stamped decoration. |
| `antiquity_updraft_kiln` | `Kiln`, `Pottery Tools`, `Hot Fire` | Firing clay, terracotta, ceramic, and glass-adjacent work. |
| `antiquity_glass_blowpipe` | `Blowpipe`, `Glassblowing Tools` | Blown glass vessels. |
| `antiquity_pontil_rod` | `Pontil Rod`, `Glassblowing Tools` | Glass finishing. |
| `antiquity_marver_slab` | `Marver Table`, `Glassblowing Tools` | Shaping hot glass. |
| `antiquity_glassworking_jacks` | `Jacks`, `Glassblowing Tools` | Glass mouths, necks, and feet. |
| `antiquity_glass_shears` | `Glass Shears`, `Glassblowing Tools` | Cutting hot glass. |
| `antiquity_annealing_lehr` | `Annealing Lehr`, `Glassblowing Tools` | Cooling glass vessels and panes. |
| `antiquity_casting_crucible` | `Crucible`, `Metalworking Tools`, `Smelting Tools` | Bronze/silver/gold casting. |
| `antiquity_crucible_tongs` | `Crucible Tongs`, `Smelting Tools` | Handling hot crucibles. |
| `antiquity_vessel_casting_mould` | `Casting Mould`, `Metalworking Tools` | Cast vessel and lamp blanks. |
| `antiquity_bronze_burnisher` | `Burnisher`, `Engraving Tools` | Polishing metal and stone surfaces. |
| `antiquity_stone_chisel` | `Stone Chisel`, `Stoneworking Tools` | Alabaster, calcite, stone bowl shaping. |
| `antiquity_stone_mallet` | `Stone Mallet`, `Stoneworking Tools` | Stone carving. |
| `antiquity_polishing_stone` | `Lacquer Polishing Stone`, `Stoneworking Tools` | Lacquer, stone, and fine finish polishing. |
| `antiquity_candle_mould` | `Candle Mould`, `Candlemaking Tools` | Moulded beeswax candles. |
| `antiquity_lamp_mould` | `Press Mold`, `Pottery Tools` | Moulded clay lamps. |
| `antiquity_lacquer_brush` | `Lacquerer's Brush`, `Lacquerwork Tools` | Painted/lacquered boxes and furniture. |
| `antiquity_lacquer_spatula` | `Lacquer Spatula`, `Lacquerwork Tools` | Lacquer application. |

## Shared Upstream Crafts

These are the upstream craft families to implement before final products. The exact `AddCraft` calls can use compact import strings and the knowledge-gated overload.

| Craft | Skill | Knowledge | Inputs | Tools | Products |
| --- | --- | --- | --- | --- | --- |
| Dress timber into household stock | Carpentry | Ancient Woodworking and Joinery | Exact wood commodity | Hand saw, adze, smoothing plane | Same material commodity tagged `Furniture Timber Stock`. |
| Saw timber into panels | Carpentry | Ancient Woodworking and Joinery | `Furniture Timber Stock` | Hand saw, plane, clamp | Same material commodity tagged `Furniture Panel Stock`. |
| Shape carved wood stock | Carpentry | Ancient Woodworking and Joinery | `Furniture Timber Stock` | Chisel, rasp, bow drill | Same material commodity tagged `Carved Wood Stock`. |
| Make wooden peg stock | Carpentry | Ancient Woodworking and Joinery | Wood commodity | Chisel, mallet, knife | Same material commodity tagged `Peg`. |
| Prepare basketry splints | Basketry | Ancient Basketry | Willow, papyrus, reed, or similar commodity | Basket knife, reed splitter | Commodity tagged `Basketry Splint`. |
| Weave reed matting | Basketry | Ancient Basketry | `Basketry Splint` | Weaving bodkin, packing bone | Commodity tagged `Reed Matting`. |
| Prepare staves for coopering | Coopering | Ancient Coopering | Oak, ash, or similar timber stock | Cooper's adze, jointer, croze | Commodity tagged `Coopered Staves`. |
| Prepare hoop stock | Coopering or Blacksmithing | Ancient Coopering | Willow, bronze, wrought iron, or wood commodity | Hoop driver or metal tools | Commodity tagged `Hoop Stock`. |
| Cut leather panels | Leathermaking | Ancient Leather Containers | Leather commodity | Awl punch, edge beveller, shears | Commodity tagged `Prepared Leather Panel`. |
| Twist lamp wicks | Ropemaking | Ancient Lighting and Heating | Linen, hemp, papyrus, or cotton fibre/yarn | Rope hook, twine shuttle | Commodity tagged `Lamp Wick`. |
| Prepare pitch stock | Ropemaking | Ancient Lighting and Heating | Resin/pitch commodity | Heating vessel, stirring tool | Commodity tagged `Prepared Pitch`. |
| Prepare pottery clay body | Pottery | Ancient Ceramic Vesselmaking | Clay or earthenware commodity, water | Pug mill or hand tools | Commodity tagged `Pottery Clay Body`. |
| Form wet vessel blanks | Pottery | Ancient Ceramic Vesselmaking | `Pottery Clay Body` | Potter's wheel, rib, clay knife | Commodity tagged `Wet Vessel Blank`. |
| Fire bisque vessel blanks | Pottery | Ancient Ceramic Vesselmaking | `Wet Vessel Blank`, fuel | Kiln | Commodity tagged `Bisque Vessel Blank`. |
| Prepare glass batch | Glassworking | Ancient Glassworking | Soda-lime glass inputs or glass commodity | Crucible, kiln/glory heat | Commodity tagged `Glass Batch`. |
| Blow glass vessel blanks | Glassworking | Ancient Glassworking | `Glass Batch` | Blowpipe, pontil, marver, jacks, annealing lehr | Commodity tagged `Glass Vessel Blank`. |
| Cast vessel blanks | Blacksmithing or Silversmithing | Ancient Metal Vesselmaking | Bronze, iron, silver, or gold commodity | Crucible, tongs, mould, hot fire | Commodity tagged `Cast Vessel Blank`. |
| Carve stone vessel blanks | Masonry or Scrimshawing | Ancient Stone Bone and Horn Carving | Alabaster, calcite, horn, or ivory commodity | Chisel, mallet, bow drill, polishing stone | Commodity tagged `Stone Vessel Blank`. |
| Prepare inlay stock | Scrimshawing, Glassworking, or Silversmithing | Ancient Household Crafting | Ivory, glass, shell, metal, or coloured stone commodity | Saw, chisel, polishing stone | Commodity tagged `Inlay Stock`. |
| Prepare painted or lacquered finish | Dyeing or Lacquerwork | Ancient Household Crafting | Pigment/lacquer commodity | Brush, spatula, polishing stone | Commodity tagged `Paint Pigment` or `Lacquer Finish`. |

## Final Craft Families

Each final craft should use one row from this family table and one stable product reference from the coverage inventory.

| Family | Count | Main Skill | Knowledge | Input Pattern | Tool Pattern | Product Pattern |
| --- | ---: | --- | --- | --- | --- | --- |
| Woodworking and Joinery | 88 | Carpentry | Shared or culture-specific household knowledge | Exact wood `Furniture Timber Stock`, `Furniture Panel Stock`, optional `Peg`, optional bronze/iron `Nail` or `Rivet`, optional `Inlay Stock`, optional `Lacquer Finish` or `Paint Pigment`. | Saw, adze, chisel, auger, plane, clamp, rasp. | `StableSimpleProduct`, or variable product when `Variable_FineColour` is present. |
| Basketry and Reedwork | 9 | Basketry | Ancient Basketry or culture-specific household knowledge | Willow/papyrus/reed `Basketry Splint`, `Reed Matting`, optional yarn/tie. | Basket knife, reed splitter, bodkin, packing bone, basket clamp. | Stable product. |
| Textile and Leather Goods | 57 | Tailoring or Leathermaking | Ancient Leather Containers, Ancient Household Crafting, or culture-specific household knowledge | `Garment Cloth`, `Spun Yarn`, `Prepared Leather Panel`, hide/fur commodity, optional `Lamp Wick`, optional `Bead Stock`. | Sewing needle, shears, leather awl, stitching pony, edge beveller. | Stable simple/variable product. |
| Coopering | 7 | Coopering | Ancient Coopering | `Coopered Staves`, `Hoop Stock`, optional pitch or wax seal. | Cooper's adze, croze, hoop driver, bung borer, mallet. | Dry or liquid container stable product. |
| Pottery and Fired Clay | 85 | Pottery | Ancient Ceramic Vesselmaking or culture-specific household knowledge | `Pottery Clay Body`, `Wet Vessel Blank`, `Bisque Vessel Blank`, optional slip/glaze/pigment, optional wick for lamps. | Potter's wheel, rib, clay knife, loop tool, stamp, kiln. | Stable product; variable colour products when painted/glazed descriptors require it. |
| Glassworking | 10 | Glassworking | Ancient Glassworking or culture-specific household knowledge | `Glass Batch`, `Glass Vessel Blank`, optional `Glass Panes`, optional pigment/colour stock. | Blowpipe, pontil, marver, jacks, shears, annealing lehr. | Stable product. |
| Metal Vesselmaking and Casting | 49 | Blacksmithing or Silversmithing | Ancient Metal Vesselmaking or culture-specific household knowledge | Bronze/iron/silver/gold `Cast Vessel Blank`, optional `Rivet`, `Hoop Stock`, `Inlay Stock`, lamp wick. | Crucible, tongs, mould, hammer, anvil, burnisher. | Stable product. |
| Stone Bone and Horn Carving | 12 | Masonry or Scrimshawing | Ancient Stone Bone and Horn Carving or culture-specific household knowledge | `Stone Vessel Blank`, exact alabaster/calcite/horn/ivory commodity, optional polish/inlay. | Stone chisel, stone mallet, bow drill, polishing stone. | Stable product. |
| Candlemaking and Wicks | 2 | Candlemaking | Ancient Lighting and Heating | Beeswax commodity, `Lamp Wick`, optional dye/pigment. | Candle mould, knife. | Stable product. |
| Torchmaking and Pitchwork | 2 | Ropemaking or Carpentry | Ancient Lighting and Heating | Pine/wood shaft commodity, cloth/fibre, `Prepared Pitch`, `Lamp Wick`. | Knife, binding tool, heating vessel. | Stable product. |
| Composite or Special Case | 11 | Case-specific | Shared or culture-specific household knowledge | Mix of the above families. | Case-specific tools. | Stable product. |

## Composite Special Cases

These are target items that should not be forced into a single material-family template.

| Stable Reference | Proposed Craft Path |
| --- | --- |
| `antiquity_coarse_hemp_grain_sack` | Textile sack craft using hemp cloth/yarn, not basketry. |
| `antiquity_folded_textile_stand` | Basketry frame plus textile support, with `Basketry` primary and optional `Tailoring` upstream. |
| `antiquity_liquid_leather_belt_oil_flask` | Leather flask craft with sealant/pitch and belt fitting. |
| `antiquity_liquid_silver_tipped_belt_flask` | Leather flask craft with silver fitting stock and `Silversmithing` upstream. |
| `antiquity_liquid_tooled_leather_flask` | Leather flask craft with tooling tools and sealant. |
| `antiquity_papyrus_goods_stand` | Basketry/reed stand rather than wood joinery. |
| `antiquity_simple_sleeping_mat` | Reed matting craft from papyrus splints. |
| `antiquity_simple_woven_floor_mat` | Reed matting craft from papyrus splints. |
| `antiquity_steppe_gorytos_case` | Leather case craft with bow/quiver form; culture gate `Scythian-Sarmatian Household Crafting`. |
| `antiquity_tableware_scythian_leather_travel_cup` | Shaped leather cup using hardening/sealing, not generic pouch sewing. |
| `antiquity_woven_reed_pallet` | Reed matting plus binding, craft under `Basketry`. |

## Coverage Inventory

The following stable references are the design-audit coverage list from the implementation pass. The code now covers the live set dynamically through household market tags rather than copying this list into the craft seeder.

### Basketry and Reedwork

`antiquity_lidded_willow_storage_basket`, `antiquity_open_willow_carrying_basket`, `antiquity_papyrus_amulet_pouch`, `antiquity_papyrus_lidded_basket`, `antiquity_plaited_papyrus_market_basket`, `antiquity_plaited_papyrus_shoulder_bag`, `antiquity_rush_seated_stool`, `antiquity_shallow_offering_basket`, `antiquity_small_linen_spice_sachet`

### Candlemaking and Wicks

`antiquity_fine_beeswax_candle`, `antiquity_plain_wax_candle`

### Composite or Special Case

`antiquity_coarse_hemp_grain_sack`, `antiquity_folded_textile_stand`, `antiquity_liquid_leather_belt_oil_flask`, `antiquity_liquid_silver_tipped_belt_flask`, `antiquity_liquid_tooled_leather_flask`, `antiquity_papyrus_goods_stand`, `antiquity_simple_sleeping_mat`, `antiquity_simple_woven_floor_mat`, `antiquity_steppe_gorytos_case`, `antiquity_tableware_scythian_leather_travel_cup`, `antiquity_woven_reed_pallet`

### Coopering

`antiquity_liquid_ash_ale_cask`, `antiquity_liquid_hooped_trade_barrel`, `antiquity_liquid_oak_hogshead`, `antiquity_liquid_oak_wet_cask`, `antiquity_liquid_small_coopered_rundlet`, `antiquity_oak_stave_dry_cask`, `antiquity_tall_hooped_trade_barrel`

### Glassworking

`antiquity_glass_display_bowl`, `antiquity_glass_display_case`, `antiquity_glass_front_display_case`, `antiquity_liquid_blue_glass_cosmetic_bottle`, `antiquity_liquid_glass_alabastron`, `antiquity_liquid_glass_perfume_pendant`, `antiquity_liquid_purple_glass_perfume_flask`, `antiquity_liquid_small_glass_unguent_bottle`, `antiquity_tableware_egyptian_blue_glass_goblet`, `antiquity_tableware_punic_glass_cup`

### Metal Vesselmaking and Casting

`antiquity_bronze_brazier`, `antiquity_bronze_cista_box`, `antiquity_bronze_display_stand`, `antiquity_bronze_household_figurine`, `antiquity_bronze_mirror_stand`, `antiquity_bronze_oil_lamp`, `antiquity_bronze_tripod_stand`, `antiquity_folding_curule_stool`, `antiquity_hanging_bronze_lamp`, `antiquity_iron_fire_basket`, `antiquity_liquid_anatolian_stag_rhyton`, `antiquity_liquid_animal_headed_rhyton`, `antiquity_liquid_bronze_belt_aryballos`, `antiquity_liquid_bronze_belt_flask`, `antiquity_liquid_bronze_caravan_water_jar`, `antiquity_liquid_bronze_feast_cauldron`, `antiquity_liquid_bronze_handwashing_basin`, `antiquity_liquid_bronze_libation_bowl`, `antiquity_liquid_bronze_situla_pail`, `antiquity_liquid_silver_court_cup`, `antiquity_liquid_steppe_bronze_kettle`, `antiquity_portable_incense_burner`, `antiquity_tableware_anatolian_bronze_cup`, `antiquity_tableware_anatolian_bronze_cup_stand`, `antiquity_tableware_anatolian_bronze_platter_bowl`, `antiquity_tableware_celtic_bronze_feast_tray`, `antiquity_tableware_celtic_bronze_feasting_cup`, `antiquity_tableware_celtic_bronze_serving_bowl`, `antiquity_tableware_etruscan_bronze_display_tray`, `antiquity_tableware_etruscan_bronze_ladle_cup`, `antiquity_tableware_etruscan_bronze_stemmed_bowl`, `antiquity_tableware_hellenic_bronze_cup_tray`, `antiquity_tableware_hellenic_phiale`, `antiquity_tableware_italic_lanx_platter`, `antiquity_tableware_italic_silver_wine_cup`, `antiquity_tableware_nubian_bronze_drinking_bowl`, `antiquity_tableware_nubian_bronze_offering_tray`, `antiquity_tableware_persian_bronze_banquet_tray`, `antiquity_tableware_persian_bronze_dish`, `antiquity_tableware_persian_bronze_rhyton`, `antiquity_tableware_persian_lotiform_cup`, `antiquity_tableware_persian_shallow_gold_bowl`, `antiquity_tableware_persian_silver_charger`, `antiquity_tableware_persian_silver_footed_bowl`, `antiquity_tableware_punic_bronze_libation_bowl`, `antiquity_tableware_punic_bronze_trade_tray`, `antiquity_tableware_scythian_gold_cup`, `antiquity_tableware_scythian_gold_offering_dish`, `antiquity_tall_lamp_stand`

### Pottery and Fired Clay

`antiquity_earthenware_brazier`, `antiquity_household_terracotta_figurine`, `antiquity_lidded_ceramic_pyxis`, `antiquity_liquid_anatolian_belt_oil_flask`, `antiquity_liquid_anatolian_libation_cup`, `antiquity_liquid_athlete_oil_aryballos`, `antiquity_liquid_black_topped_water_jar`, `antiquity_liquid_blue_glazed_unguent_jar`, `antiquity_liquid_broad_mixing_krater`, `antiquity_liquid_broad_table_krater`, `antiquity_liquid_bucchero_chalice`, `antiquity_liquid_bucchero_water_hydria`, `antiquity_liquid_bucchero_wine_pitcher`, `antiquity_liquid_cedar_stoppered_merchant_jar`, `antiquity_liquid_cedar_stoppered_wine_jar`, `antiquity_liquid_common_trade_amphora`, `antiquity_liquid_dressel_wine_amphora`, `antiquity_liquid_glazed_wine_ewer`, `antiquity_liquid_hydria_water_jar`, `antiquity_liquid_kerma_beaker`, `antiquity_liquid_linen_slung_clay_canteen`, `antiquity_liquid_low_wash_basin`, `antiquity_liquid_meroitic_painted_beer_jar`, `antiquity_liquid_narrow_punic_amphora`, `antiquity_liquid_oinochoe_wine_jug`, `antiquity_liquid_painted_kushite_canteen`, `antiquity_liquid_painted_kushite_libation_bowl`, `antiquity_liquid_painted_punic_cup`, `antiquity_liquid_papyrus_slung_water_jar`, `antiquity_liquid_pitch_lined_storage_jar`, `antiquity_liquid_plain_pouring_jug`, `antiquity_liquid_punic_oil_flask`, `antiquity_liquid_red_clay_water_jar`, `antiquity_liquid_red_slipped_anatolian_pitcher`, `antiquity_liquid_red_slipped_cup`, `antiquity_liquid_red_slipped_serving_jug`, `antiquity_liquid_reed_wrapped_canteen`, `antiquity_liquid_round_aryballos`, `antiquity_liquid_sauce_amphora`, `antiquity_liquid_sealed_oil_amphora`, `antiquity_liquid_shallow_kylix`, `antiquity_liquid_simple_clay_drinking_cup`, `antiquity_liquid_slung_lekythos_flask`, `antiquity_liquid_small_amphoriskos`, `antiquity_liquid_tall_kantharos`, `antiquity_liquid_terracotta_water_jar`, `antiquity_liquid_white_ground_lekythos`, `antiquity_moulded_terracotta_lamp`, `antiquity_painted_larnax_chest`, `antiquity_simple_clay_oil_lamp`, `antiquity_small_earthenware_amphoriskos`, `antiquity_small_painted_plaque`, `antiquity_tableware_anatolian_animal_rhyton`, `antiquity_tableware_anatolian_painted_ceramic_platter`, `antiquity_tableware_anatolian_red_burnished_bowl`, `antiquity_tableware_anatolian_red_burnished_goblet`, `antiquity_tableware_egyptian_baked_clay_beer_mug`, `antiquity_tableware_egyptian_bread_plate`, `antiquity_tableware_etruscan_bucchero_chalice`, `antiquity_tableware_etruscan_bucchero_dish`, `antiquity_tableware_etruscan_bucchero_kantharos`, `antiquity_tableware_etruscan_bucchero_platter`, `antiquity_tableware_hellenic_kantharos`, `antiquity_tableware_hellenic_kylix`, `antiquity_tableware_hellenic_painted_serving_pinax`, `antiquity_tableware_hellenic_pinax_plate`, `antiquity_tableware_hellenic_shallow_meze_dish`, `antiquity_tableware_hellenic_skyphos`, `antiquity_tableware_italic_plain_ceramic_poculum`, `antiquity_tableware_italic_plain_earthenware_plate`, `antiquity_tableware_italic_samian_bowl`, `antiquity_tableware_italic_terra_sigillata_cup`, `antiquity_tableware_italic_terra_sigillata_plate`, `antiquity_tableware_italic_terra_sigillata_serving_dish`, `antiquity_tableware_nubian_black_burnished_bowl`, `antiquity_tableware_nubian_black_burnished_cup`, `antiquity_tableware_nubian_blackware_serving_platter`, `antiquity_tableware_nubian_red_black_goblet`, `antiquity_tableware_nubian_redware_plate`, `antiquity_tableware_punic_red_slip_bowl`, `antiquity_tableware_punic_red_slip_cup`, `antiquity_tableware_punic_red_slip_serving_platter`, `antiquity_tableware_punic_shallow_fish_plate`, `antiquity_tall_narrow_trade_amphora`, `antiquity_terracotta_dry_amphora`

### Stone Bone and Horn Carving

`antiquity_alabaster_display_bowl`, `antiquity_alabaster_kohl_box`, `antiquity_ivory_chip_jewel_box`, `antiquity_liquid_alabaster_unguent_pendant`, `antiquity_liquid_calcite_perfume_jar`, `antiquity_liquid_horn_drinking_cup`, `antiquity_liquid_slender_alabastron`, `antiquity_tableware_celtic_horn_cup`, `antiquity_tableware_egyptian_alabaster_cup`, `antiquity_tableware_egyptian_alabaster_offering_bowl`, `antiquity_tableware_egyptian_alabaster_serving_stand`, `antiquity_tableware_germanic_horn_beaker`

### Textile and Leather Goods

`antiquity_beaded_kohl_pouch`, `antiquity_bright_linen_trade_pouch`, `antiquity_broad_linen_storage_bag`, `antiquity_canvas_travel_pack`, `antiquity_checked_wool_belt_pouch`, `antiquity_checked_wool_hanging`, `antiquity_checked_wool_market_bag`, `antiquity_deer_leather_game_bag`, `antiquity_double_strap_travel_pack`, `antiquity_felt_mirror_pouch`, `antiquity_felt_nomad_bundle`, `antiquity_felt_riding_pack`, `antiquity_felt_tent_carpet`, `antiquity_fine_bordered_tablecloth`, `antiquity_fine_embroidered_hanging`, `antiquity_fine_patterned_coverlet`, `antiquity_fine_patterned_cushion`, `antiquity_folded_tablet_wallet`, `antiquity_fur_lined_forager_bag`, `antiquity_fur_provision_pouch`, `antiquity_fur_throw`, `antiquity_heavy_wool_curtain`, `antiquity_hemp_shoulder_tote`, `antiquity_hide_sleeping_roll`, `antiquity_leather_dispatch_satchel`, `antiquity_leather_document_case`, `antiquity_leather_floor_cover`, `antiquity_leather_mirror_case`, `antiquity_linen_bolster`, `antiquity_linen_door_curtain`, `antiquity_linen_drawstring_purse`, `antiquity_liquid_birch_stoppered_mead_skin`, `antiquity_liquid_caravan_waterskin`, `antiquity_liquid_felt_covered_riding_canteen`, `antiquity_liquid_hide_ale_skin`, `antiquity_liquid_plain_leather_waterskin`, `antiquity_liquid_saddle_waterskin`, `antiquity_liquid_sailor_water_skin`, `antiquity_liquid_soldier_shoulder_canteen`, `antiquity_liquid_steppe_kumis_skin`, `antiquity_liquid_steppe_milk_skin`, `antiquity_liquid_wide_mouth_waterskin`, `antiquity_patterned_wool_carpet`, `antiquity_plain_leather_belt_pouch`, `antiquity_plain_linen_cushion`, `antiquity_plain_wool_floor_rug`, `antiquity_rope_corded_trade_bundle`, `antiquity_round_coin_purse`, `antiquity_simple_partition_screen`, `antiquity_smoked_hide_meat_bag`, `antiquity_steppe_saddlebag_pack`, `antiquity_stuffed_wool_cushion`, `antiquity_wide_belt_document_pouch`, `antiquity_wool_sleeping_blanket`, `antiquity_wool_sling_bundle`, `antiquity_wool_tablet_satchel`, `antiquity_woollen_bedroll`

### Torchmaking and Pitchwork

`antiquity_long_burning_torch`, `antiquity_pitch_soaked_torch`

### Woodworking and Joinery

`antiquity_acacia_linen_chest`, `antiquity_acacia_linen_stand`, `antiquity_ash_armour_stand`, `antiquity_birch_collapsible_rack`, `antiquity_birchbark_storage_box`, `antiquity_bronze_bound_chest`, `antiquity_bronze_footed_table`, `antiquity_carved_animal_legged_stool`, `antiquity_carved_elite_bed`, `antiquity_carved_household_bench`, `antiquity_carved_wooden_screen`, `antiquity_cedar_canopic_chest`, `antiquity_cedar_cosmetic_box`, `antiquity_cedar_display_hutch`, `antiquity_cedar_document_cabinet`, `antiquity_cedar_scribe_box`, `antiquity_cedar_trade_casket`, `antiquity_curved_back_klismos_chair`, `antiquity_cypress_capsa_scroll_box`, `antiquity_cypress_storage_cupboard`, `antiquity_document_shelves`, `antiquity_drawered_desk_chest`, `antiquity_ebony_inlaid_casket`, `antiquity_fine_banquet_couch`, `antiquity_fine_carved_headrest`, `antiquity_flat_wooden_carrying_tray`, `antiquity_high_back_elite_chair`, `antiquity_inlaid_luxury_table`, `antiquity_ivory_inlaid_box`, `antiquity_large_feasting_table`, `antiquity_large_storage_cupboard`, `antiquity_liquid_carved_wooden_cup`, `antiquity_long_hall_bench`, `antiquity_long_store_table`, `antiquity_low_backed_chair`, `antiquity_low_box_stool`, `antiquity_low_clothing_coffer`, `antiquity_low_dining_couch`, `antiquity_low_display_stand`, `antiquity_low_market_counter`, `antiquity_low_open_shelf`, `antiquity_low_sideboard`, `antiquity_narrow_display_shelves`, `antiquity_oak_trade_cabinet`, `antiquity_oak_weapon_rack`, `antiquity_open_wall_shelf`, `antiquity_padded_daybed`, `antiquity_painted_boxwood_coffer`, `antiquity_painted_household_shrine_stand`, `antiquity_painted_side_table`, `antiquity_plain_plank_bench`, `antiquity_plain_side_table`, `antiquity_plain_storage_chest`, `antiquity_plain_trestle_table`, `antiquity_plain_wooden_bed`, `antiquity_plain_wooden_stool`, `antiquity_plain_writing_table`, `antiquity_portable_garment_cabinet`, `antiquity_portable_lacquered_box`, `antiquity_pottery_display_stand`, `antiquity_rope_strung_bed`, `antiquity_round_three_legged_table`, `antiquity_scroll_capsa_sling`, `antiquity_scroll_pigeonhole_shelves`, `antiquity_sloped_writing_desk`, `antiquity_small_household_cupboard`, `antiquity_small_low_table`, `antiquity_small_offering_table`, `antiquity_stacked_sample_case`, `antiquity_tableware_celtic_oak_serving_board`, `antiquity_tableware_celtic_wooden_ale_cup`, `antiquity_tableware_celtic_wooden_trencher`, `antiquity_tableware_egyptian_palmwood_offering_tray`, `antiquity_tableware_germanic_bread_trencher`, `antiquity_tableware_germanic_wooden_beaker`, `antiquity_tableware_germanic_wooden_food_bowl`, `antiquity_tableware_germanic_wooden_serving_tray`, `antiquity_tableware_scythian_gold_mount_platter`, `antiquity_tableware_scythian_hide_lined_wooden_tray`, `antiquity_tableware_scythian_wooden_cup`, `antiquity_tableware_scythian_wooden_meat_platter`, `antiquity_three_legged_camp_stool`, `antiquity_walnut_grain_chest`, `antiquity_walnut_sideboard`, `antiquity_wax_tablet_locker`, `antiquity_wide_household_shelves`, `antiquity_wide_trade_shelves`, `antiquity_wooden_headrest`

## Pigment and Dye Suite

The household and textile crafting pass also seeds a broad stock set of ancient pigments and dyes. The intent is not an exhaustive conservation catalogue, but enough common and culturally distinctive stock for antiquity textile, paint, pottery, furniture, and finishing workflows.

New commodity tags:

| Tag | Parent | Purpose |
| --- | --- | --- |
| `Textile Dye Stock` | `Textile Commodity` | Prepared dye concentrate used by cloth dyeing crafts. |
| `Lake Pigment` | `Paint Pigment` | Organic dye fixed onto an inert pigment body for paint and decoration. |

New raw dyestuff and pigment materials:

| Family | Materials |
| --- | --- |
| Organic textile dyes | `woad leaves`, `weld`, `kermes grain`, `alkanet root`, `henna leaf`, `pomegranate rind`, `walnut hull`, `oak gall`, `orchil lichen`, `lac dye cake`, `murex purple dye`. |
| Mineral and synthetic pigments | `orpiment`, `realgar`, `verdigris pigment`, `lead white pigment`, `red ochre pigment`, `yellow ochre pigment`, `egyptian blue frit`, `bone black pigment`. |
| Existing materials reused | `madder root`, `indigo dye cake`, `ochre pigment`, `saffron`, `soot`, `chalk dust`, `azurite`, `lapis lazuli`, `malachite`, `hematite`, `cinnabar`, `gypsum`, `lead`, and `alum mordant`. |

Craft families:

| Craft Family | Skill | Knowledge | Output |
| --- | --- | --- | --- |
| Dye extraction and concentration | `Dyeing` | `Ancient Textile Production` | Raw dyestuffs become `Textile Dye Stock` commodities carrying `Colour` and `Fine Colour`. |
| Cloth dyeing | `Dyeing` | `Ancient Textile Production` | Wool, linen, cotton, and felt cloth consume `Textile Dye Stock` plus mordant/water to become `Dyed Cloth`. |
| Mineral pigment preparation | `Dyeing` | `Ancient Household Crafting` | Mineral, soot, chalk, and synthetic pigment inputs become `Paint Pigment`. |
| Lake pigment preparation | `Dyeing` | `Ancient Household Crafting` | Organic `Textile Dye Stock` plus alum and chalk becomes `Lake Pigment`. |

Colour characteristic updates:

- `Basic_Colours` includes `olive` so basic outputs can use all simple colour groups emitted by the craft suite.
- `Fine_Colours` and `Most_Colours` include ancient pigment names such as `madder red`, `kermes scarlet`, `tyrian purple`, `woad blue`, `egyptian blue`, `malachite green`, `orpiment yellow`, `cinnabar red`, `lead white`, `lamp black`, `walnut brown`, `oak-gall black`, `pomegranate yellow`, `saffron yellow`, and `henna orange`.
- `Drab_Colours` includes muted pigment-derived values such as `faded madder red`, `dull ochre`, `dull egyptian blue`, `faded woad blue`, `tarnished lead white`, and `faded tyrian purple`.
- Craft outputs use only `Colour` and `Fine Colour`; the drab additions are for builder profile completeness.

The toxic historical pigments lead white, cinnabar, orpiment, and realgar are included as seeded craft materials because they are important ancient pigment stock. This pass does not add new poison or material-hazard mechanics.

## Test Plan

Add focused DatabaseSeeder tests:

| Test | Purpose |
| --- | --- |
| `AntiquityHouseholdSeeder_CurrentCatalogueHasFurnitureAndContainerTargets` | Extracts the two target seeder methods and asserts the current 243 container plus 89 household-furniture prototype counts. |
| `AntiquityHouseholdCrafts_DiscoverAllHouseholdGoodsByMarketTags` | Asserts the craft suite discovers target products through `Market / Household Goods / ...` tags rather than a stale hand-maintained reference list. |
| `ItemSeeder_AntiquityFurnitureAndContainers_KnowledgeGatesAreSeeded` | Asserts shared and culture-specific knowledge names are created and used by at least one craft. |
| `ItemSeeder_AntiquityFurnitureAndContainers_ToolTagsResolve` | Asserts every `TagTool` referenced by the craft suite has a seeded tag and at least one seeded tool prototype where appropriate. |
| `ItemSeeder_AntiquityFurnitureAndContainers_NewSkillsResolve` | Asserts `Basketry`, `Coopering`, `Ropemaking`, and `Lacquerwork` exist when the stock skill package is installed. |
| `ItemSeeder_AntiquityFurnitureAndContainers_CommodityTagsResolve` | Asserts new commodity tags exist and can be referenced by the craft import parser. |

Recommended local verification:

```powershell
dotnet test 'DatabaseSeeder Unit Tests\DatabaseSeeder Unit Tests.csproj' -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510 --filter ItemSeeder
git diff --check
```

If the implementation changes shared `AddCraft` parsing rather than only adding craft rows, also build `DatabaseSeeder` and `MudSharpCore` because craft product/input subtype mismatches can surface across the seeder/runtime boundary.
