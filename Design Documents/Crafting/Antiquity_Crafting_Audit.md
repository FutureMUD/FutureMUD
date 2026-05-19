# Antiquity Crafting Catalogue Audit

This document records the source-backed audit boundary for the current antiquity item and craft suite. It is a catalogue guide, not a replacement for the suite-specific implementation documents.

## Source Boundary

Current item definitions are seeded from:

| Source | Current Item Definitions |
| --- | ---: |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.Antiquity.cs` | 995 |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.AntiquityHouseholdTools.cs` | 63 |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.AntiquityMedical.cs` | 41 |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.AntiquityWriting.cs` | 33 |
| Total | 1132 |

Current craft definitions are seeded from:

| Source | Craft Surface |
| --- | --- |
| `ItemSeederCrafting.Antiquity.cs` | Textile, leather, and older shared antiquity craft chains. |
| `ItemSeederCrafting.AntiquityEquipment.cs` | Common clothing/accessories, equipment stock, military goods, and heat-source lighting crafts. |
| `ItemSeederCrafting.AntiquityHousehold.cs` | Household, container, vessel, door, gate, and furnishing craft discovery. |
| `ItemSeederCrafting.AntiquityMedical.cs` | Medical supplies, remedies, surgical tools, mobility aids, and prosthetics. |
| `ItemSeederCrafting.AntiquityWriting.cs` | Writing surfaces, implements, inks, scrolls, codices, and document containers. |

The suite-specific docs now catalogue every stable reference explicitly named by those craft source files. Dynamic discovery surfaces are documented by their source counts and inventory lists:

- 398 household/container/door/furniture targets discovered from household, writing, religious, lighting, heating, construction, and writing-product tag roots.
- 193 military equipment targets discovered from `Market / Military Goods`, split into 76 armour prototypes and 117 weapon, shield, ammunition, sling, and accessory prototypes.
- 356 stable references explicitly named in the current antiquity craft source files.

## Verified Invariants

- Every current antiquity `TagTool` requirement has at least one seeded item prototype carrying the exact leaf tool tag.
- Every explicit stable reference in `ItemSeederCrafting.Antiquity*.cs` is listed in the antiquity crafting docs.
- Lit workshop items have morph targets, morph timers, and their expected lit-state tags.
- Heat-source lighting crafts consume `charcoal`, produce the lit apparatus, and return the unlit apparatus on failure.
- Literal commodity product tags are either consumed downstream or documented as reusable stock outputs.
- Low-heat medical recipes that boil, warm, or steep in a cooking pot now also require an in-room `Fire` tool, so they use the lit workshop hearth convention.

## Remaining Logical Gaps

The current audit leaves these as deliberate follow-up gaps rather than hidden assumptions:

- `SeedAntiquityJewellery` currently contains 162 item prototypes in the main antiquity item source, but there is no dedicated jewellery craft file or per-reference jewellery crafting catalogue. Existing tests treat jewellery as an older covered surface; the next audit should either add jewellery crafts or explicitly mark jewellery as stock-only.
- Newly introduced support tools and unlit workshop apparatus are seeded prerequisites. Crafts to manufacture those tools and apparatus remain the second-pass boundary unless they already belong to an existing product suite.
- Glassworking currently uses `Glass Batch`, blowing tools, and the lit annealing lehr as the current craftable glass chain. A dedicated glass furnace or glory-hole style apparatus would be a better simulation if glasswork needs to move beyond this abstraction.
- The craft system still relies on commodity tags and item stable references rather than a generated external catalogue. The tests now protect the docs from drifting away from the current source, but the docs are maintained rather than generated.
