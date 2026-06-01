# Antiquity Crafting Catalogue Audit

This document records the source-backed audit boundary for the current antiquity item and craft suite. It is a catalogue guide, not a replacement for the suite-specific implementation documents.

## Source Boundary

Current item definitions are seeded from:

| Source | Current Item Definitions |
| --- | ---: |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.Antiquity.cs` | 1034 |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.AntiquityFood.cs` | 167 |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.AntiquityHouseholdTools.cs` | 65 |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.AntiquityMedical.cs` | 41 |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.AntiquityWriting.cs` | 33 |
| Total | 1340 |

Cross-era foundation note: `ItemSeeder.Rework.Medieval.cs` now owns the shared `historic_*` workshop apparatus that is seeded when either `antiquity` or `medieval` is selected. Antiquity-specific garment, weapon, jewellery, and foodway stable references remain unchanged; the shared items are additions rather than renames.

Current craft definitions are seeded from:

| Source | Craft Surface |
| --- | --- |
| `ItemSeederCrafting.Antiquity.cs` | Textile, leather, and older shared antiquity craft chains. |
| `ItemSeederCrafting.AntiquityAgriculture.cs` | Seed-stock selection, raw milk straining, and crop derivative stock for dyes and saffron. |
| `ItemSeederCrafting.AntiquityApiary.cs` | Apiary tools, hive equipment, and honeycomb processing. |
| `ItemSeederCrafting.AntiquityEquipment.cs` | Common clothing/accessories, equipment stock, military goods, and heat-source lighting crafts. |
| `ItemSeederCrafting.AntiquityFood.cs` | Grain, pulse, meat, preservation, beverage, and culture-gated foodway crafts. |
| `ItemSeederCrafting.AntiquityHousehold.cs` | Household, container, vessel, door, gate, and furnishing craft discovery. |
| `ItemSeederCrafting.AntiquityJewellery.cs` | Jewellery stock, beads, settings, wire, and final jewellery craft discovery. |
| `ItemSeederCrafting.AntiquityMedical.cs` | Medical supplies, remedies, surgical tools, mobility aids, and prosthetics. |
| `ItemSeederCrafting.AntiquityWriting.cs` | Writing surfaces, implements, inks, scrolls, codices, and document containers. |

The suite-specific docs now catalogue every stable reference explicitly named by those craft source files. Dynamic discovery surfaces are documented by their source counts and inventory lists:

- 398 household/container/door/furniture targets discovered from functional household and writing tag roots.
- 162 jewellery targets discovered from jewellery tags and catalogued in the dedicated jewellery suite.
- 193 military equipment targets discovered from `Functions / Military Equipment`, split into 76 armour prototypes and 117 weapon, shield, ammunition, sling, and accessory prototypes.
- the food and beverage suite documents the explicit `antiquity_food_` stable-reference prefix plus the shared amphora references used by liquid filling, fermentation, and aging crafts.
- 395 stable references explicitly named in the pre-food non-jewellery antiquity craft source files, plus the dynamically discovered jewellery catalogue, the food prefix catalogue, and source-audited partial seeder coverage for food, household tools, medical items, and writing items.

## Verified Invariants

- Every current antiquity `TagTool` requirement has at least one seeded item prototype carrying the exact leaf tool tag.
- Every explicit stable reference in `ItemSeederCrafting.Antiquity*.cs` is listed in the antiquity crafting docs.
- Lit workshop items have morph targets, morph timers, and their expected lit-state tags.
- Heat-source lighting crafts consume `charcoal`, produce the lit apparatus, and return the unlit apparatus on failure.
- Literal commodity product tags are either consumed downstream or documented as reusable stock outputs.
- Low-heat medical recipes that boil, warm, or steep in a cooking pot now also require an in-room `Fire` tool, so they use the lit workshop hearth convention.
- Jewellery uses a dedicated `Ancient Jewellery Crafting` knowledge gate with metal, wire, bead, and setting stock.
- Support tools and unlit workshop apparatus are now craftable through the expanded equipment toolmaking pass.
- Glassworking now uses a lit glory-hole furnace for hot working and a lit annealing lehr for cooling finished glass.
- Food and beverage crafts now add commodity-first grain, pulse, meat, preservation, broth, fermented drink, condiment, luxury beverage, and culture-gated prepared-food chains in the `ItemSeeder` rework path, assuming the butchery package has seeded its raw animal outputs.
- Food tools now carry exact functional tool tags and retain `Market / Professional Tools / Standard Tools` for economy use; the dynamic equipment toolmaking suite discovers them through functional tool roots.
- Empty food serving and fermenting amphorae are craftable through pottery recipes that consume fired-clay `Bisque Vessel Blank` and `Prepared Pitch`; finished beer, date beer, wine, kumis, garum, and spiced beverage amphorae remain fermentation or aging morph targets.
- Kumis beverage stock consumes milk instead of grain wort. Red and white wine remain core liquids supplied by `CoreDataSeeder.Materials.cs`, not duplicate food-seeder liquids.
- Raw hides from `AnimalButcherySeeder` now have a bridge craft into raw `animal skin` commodity stock before the existing prepared-hide and tanning chain.
- Agriculture herd definitions now define secondary outputs and the stock `Collect Herd Products` operation can release milk, wool, eggs, and manure commodity piles from established pasture herds, including a horse-herd milk path for kumis-facing cultures.
- Active antiquity craft paths now convert `Seeded Yield` agricultural commodity back into `Seeds`, strain raw milk into liquid milk amphorae, compost raw manure with crop refuse, and process indigo crop, pomegranate, walnut, and saffron crocus into derivative dye or spice stock.
- Shared `historic_*` foundation items are available to antiquity installs as cross-era workshop support without replacing existing `antiquity_*` stable references.

## Second-Pass Resolution

The next-pass implementation resolves the earlier follow-up gaps as follows:

- `SeedAntiquityJewelleryCrafts()` now discovers all 162 `SeedAntiquityJewellery` prototypes and `Antiquity_Jewellery_Crafting_Suite.md` catalogues every stable reference.
- The expanded equipment craft pass discovers stock support tools from functional tool roots and explicitly includes unlit workshop apparatus. Support tools and unlit workshop apparatus are now craftable instead of remaining stock-only prerequisites.
- Glassworking now has a glassworking glory-hole furnace pair, `antiquity_glory_hole_furnace` and `antiquity_lit_glory_hole_furnace`. Hot glassworking crafts require the lit glory-hole furnace, while annealing still requires the lit annealing lehr.
- The food pass is kept wholly in `ItemSeeder` partials: item, liquid, prepared-food and spoilage-rule setup runs with rework items, and the matching crafts run with the normal ItemSeeder craft pass.
- The food gap closure keeps this pattern: food tools use functional tool tags while retaining the standard tool market root for pricing, empty amphorae use the household pottery stock chain, fermented beverages and garum finish through morphing amphorae, and culture beverage inputs now branch kumis to milk before the grain-wort fallback.
- The pastoral and derivative closure keeps the same commodity-first style: AgricultureSeeder owns crop, herd, and managed woodland production, while ItemSeeder owns reusable processing crafts that turn field outputs into seed stock, liquid milk, compost, textile dye stock, or saffron.
- The external-catalogue risk is handled with source-backed regression tests rather than a generated file pipeline: tests parse the live seeder source, compare counts, inspect the partial seeder files, and verify that dynamic catalogues are represented in the dedicated design documents.

## Deferred Upstream Systems

The current closure is an ItemSeeder craftability pass plus agriculture source slices for apiaries, secondary herd outputs, seed stock, and common agricultural derivatives. Apiculture now has seeded upstream support through agriculture apiary operations, raw honeycomb, pressed honey, rendered beeswax, and antiquity hives, stands, smoke pots, honey knives, presses, and strainers. Herd operations now provide milk, wool, eggs, and manure; crop, woodland, and antiquity processing paths now cover seed selection, compost, indigo dye cake, pomegranate rind, walnut hull, saffron, madder root, weld, alkanet root, henna leaf, kermes grain, orchil lichen, and lac dye cake.

The pass deliberately does not add full primary-production systems for mining and quarrying, marine shellfish harvesting for murex purple, regional livestock breeds, queen breeding, or every local dye/spice crop. Those remain future subsystem expansions. The current antiquity recipes are closed against existing source truth: AgricultureSeeder crop, herd, woodland, and apiary commodities; AnimalButcherySeeder raw animal outputs; core liquids and materials; household pottery stock; and the dynamic equipment toolmaking surface.
