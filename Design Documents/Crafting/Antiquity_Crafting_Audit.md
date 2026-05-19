# Antiquity Crafting Catalogue Audit

This document records the source-backed audit boundary for the current antiquity item and craft suite. It is a catalogue guide, not a replacement for the suite-specific implementation documents.

## Source Boundary

Current item definitions are seeded from:

| Source | Current Item Definitions |
| --- | ---: |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.Antiquity.cs` | 995 |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.AntiquityHouseholdTools.cs` | 65 |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.AntiquityMedical.cs` | 41 |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.AntiquityWriting.cs` | 33 |
| Total | 1134 |

Current craft definitions are seeded from:

| Source | Craft Surface |
| --- | --- |
| `ItemSeederCrafting.Antiquity.cs` | Textile, leather, and older shared antiquity craft chains. |
| `ItemSeederCrafting.AntiquityEquipment.cs` | Common clothing/accessories, equipment stock, military goods, and heat-source lighting crafts. |
| `ItemSeederCrafting.AntiquityHousehold.cs` | Household, container, vessel, door, gate, and furnishing craft discovery. |
| `ItemSeederCrafting.AntiquityJewellery.cs` | Jewellery stock, beads, settings, wire, and final jewellery craft discovery. |
| `ItemSeederCrafting.AntiquityMedical.cs` | Medical supplies, remedies, surgical tools, mobility aids, and prosthetics. |
| `ItemSeederCrafting.AntiquityWriting.cs` | Writing surfaces, implements, inks, scrolls, codices, and document containers. |

The suite-specific docs now catalogue every stable reference explicitly named by those craft source files. Dynamic discovery surfaces are documented by their source counts and inventory lists:

- 398 household/container/door/furniture targets discovered from household, writing, religious, lighting, heating, construction, and writing-product tag roots.
- 162 jewellery targets discovered from jewellery tags and catalogued in the dedicated jewellery suite.
- 193 military equipment targets discovered from `Market / Military Goods`, split into 76 armour prototypes and 117 weapon, shield, ammunition, sling, and accessory prototypes.
- 356 stable references explicitly named in the current non-jewellery antiquity craft source files, plus the dynamically discovered jewellery catalogue.

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

## Second-Pass Resolution

The next-pass implementation resolves the earlier follow-up gaps as follows:

- `SeedAntiquityJewelleryCrafts()` now discovers all 162 `SeedAntiquityJewellery` prototypes and `Antiquity_Jewellery_Crafting_Suite.md` catalogues every stable reference.
- The expanded equipment craft pass discovers stock support tools from `Market / Professional Tools / Standard Tools` and explicitly includes unlit workshop apparatus. Support tools and unlit workshop apparatus are now craftable instead of remaining stock-only prerequisites.
- Glassworking now has a glassworking glory-hole furnace pair, `antiquity_glory_hole_furnace` and `antiquity_lit_glory_hole_furnace`. Hot glassworking crafts require the lit glory-hole furnace, while annealing still requires the lit annealing lehr.
- The external-catalogue risk is handled with source-backed regression tests rather than a generated file pipeline: tests parse the live seeder source, compare counts, and verify that dynamic catalogues are represented in the dedicated design documents.
