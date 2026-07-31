# Medieval ItemSeeder Rebuild Audit

The medieval `ItemSeeder` item and craft implementation was reset for a from-scratch rebuild. The rebuild now includes direct seeded clothing, household goods and furniture, military-goods prototypes, writing/book/document prototypes, treatment and repair prototypes, decorative jewellery prototypes, the medieval industry tool and intermediate-stock item catalogue, the active Medieval production-chain crafts, and the active food, beverage, and preservation foundation.

## Current Runtime State

- `ItemSeeder.cs` still dispatches the medieval item launch methods when the `medieval` era is selected.
- `ItemSeeder.Crafting.cs` still dispatches the medieval craft launch methods.
- `SeedMedievalClothing` contains the direct clothing item `CreateItem(...)` calls.
- `SeedMedievalContainers` contains the direct household, trade, personal, and furniture-container `CreateItem(...)` calls.
- `SeedMedievalDoorsLocksAndStrongboxes` contains the direct door, gate, grate, lock, latch, key, and lock-hardware `CreateItem(...)` calls.
- `SeedMedievalFoodAndBeverageItems` contains the direct food-service, tableware, and household-vessel `CreateItem(...)` calls, then installs the Medieval-specific branch of the typed pre-industrial food catalogue.
- `SeedMedievalJewelleryAndDevotionalGoods` contains the direct decorative jewellery, religious container, and devotional furnishing `CreateItem(...)` calls.
- `SeedMedievalHouseholdFurniture` contains the direct furniture, lighting, heating, water-source, washing-fixture, and decoration `CreateItem(...)` calls.
- `SeedMedievalWeaponsShieldsAccessories` contains the direct melee weapon, ranged weapon, ammunition, and thrown-weapon `CreateItem(...)` calls.
- `SeedMedievalArmour` contains the direct armour, horse tack, barding, shield, and military support-gear `CreateItem(...)` calls.
- `SeedMedievalWritingAdministrationAndDocuments` contains the direct writing-surface, book, document, seal, container, scribal-tool, and writing-support `CreateItem(...)` calls.
- `SeedMedievalMedicalAndApothecaryItems` contains the direct treatment, apothecary, drug-delivery, mobility, casualty-transport, and prosthetic `CreateItem(...)` calls.
- `SeedMedievalRepairKits` contains the direct repair-kit and repair-supply `CreateItem(...)` calls.
- `SeedMedievalHouseholdCraftTools` now seeds the first medieval industry tool and workshop-apparatus item catalogue.
- `SeedMedievalComponentGapItems` now seeds the first medieval intermediate-stock item catalogue.
- `SeedMedievalProductionChainCrafts` now seeds the phase-ordered 35-craft Medieval industry foundation when the `medieval` era is selected.
- `SeedMedievalFoodProductionFoundationItems` seeds six food-production tools or apparatus and eleven prepared foods.
- `SeedMedievalFoodBeverageCrafts` now seeds the phase-ordered 48-craft Medieval food, beverage, and preservation foundation when the `medieval` era is selected.
- The other eight Medieval craft launch points remain explicit no-ops.
- The old authored outfit catalogue, explicit culture catalogue, generated helper/data model, and medieval craft helper families have been removed.

## Shared Baseline Admission

The [Medieval Shared Baseline Admission Manifest](./FutureMUD_Medieval_Shared_Baseline_Admission_Manifest.md) is the completed admission registry for the common `preindustrial_*` layer. It covers all 385 live shared stable references: 342 source-derived compatibility aliases and 43 shared-authored prototypes.

The manifest distinguishes installation from historical admission. It records ordinary, institutional, restricted, specialist, imported, and intentionally not-admitted decisions by culture/contact scope, date, and admitting context. In particular, the movable-type printing suite, telescope, specifically maritime astrolabe, and musket-era gunpowder-support suite are not admitted to the 500-1400 CE manifest. Earlier planispheric astrolabes, hand-gonne equipment, or artillery props require separately authored period-appropriate rows rather than reuse of these later forms. These decisions do not create or clone item prototypes.

Food uses its own larger shared layer and admission registry. The [Pre-Industrial Food Catalogue](./PreIndustrial_Food_Catalogue_Design_Reference.md) installs 2,100 shared prepared/intermediate items and 150 shared food liquids when Medieval is selected, followed by 225 Medieval-specific items and 25 Medieval-specific liquids. The Medieval shared-food admission manifest records culture/contact, date, institution/shop/craft context, availability, and production/trade status for all 2,250 shared rows without cloning common dishes into national variants.

## Prerequisite Routing Audit Shape

The medieval craft-completeness audit now tracks prerequisite ownership as well as finished-item craft coverage. Any generated or maintained audit table for the craft rebuild should include these columns:

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

Use `owning_resolution_pass` to route missing prerequisites to the shared owner before finished medieval craft authoring consumes them, for example `UsefulSeeder item component pass`, `UsefulSeeder tag pass`, `Primary production seeder`, `Agriculture seeder`, `Butchery seeder`, `Forage seeder`, or `Skill package seeder`.

## Shared Prerequisite Data Sync

The medieval industry prerequisite pass is reflected in the maintained data documents as follows:

- `Design Documents/Data/Seeded_Item_Components.json` includes the shared `Tool_*_General` `HandTool` prototypes seeded by `UsefulSeeder.ItemComponents.cs`.
- `Design Documents/Data/SeededTagHierarchy.csv` includes the required textile, household-stock, Primary Production commodity, Primary Production tool, apothecary, jewellery, lapidary, shared food-stock, foodmaking-tool, and raw meat/fish classification paths.
- `Seeded_Materials.json` now includes the already-live `prepared clay` and `fired brick` materials consumed by this chain; this repairs pre-existing export drift rather than introducing new materials.
- `Seeded_Item_Components.json` includes the four reusable Medieval `PreparedFood` component profiles.
- `Item_Component_Types.json`, `Seeded_Liquids.json`, `Seeded_Materials.json`, and `Seeded_Gases.json` were checked for the food slice and did not require further changes because it introduces no component type, liquid, material, or gas.
- `SkillPackageSeeder.cs` now includes the repeated medieval industry prerequisite skills `Goldsmithing`, `Glassblowing`, `Lapidary`, `Fulling`, `Parchmentmaking`, `Papermaking`, `Bookbinding`, `Calligraphy`, `Scribing`, `Woodblock Printing`, and `Quarrying`; there is no separate maintained skill-package data export under `Design Documents/Data`.

## Active Medieval Industry Tool and Stock Source

The first medieval industry foundation item source is implemented as current item-catalogue content:

- Tool and workshop-apparatus item prototypes live in `DatabaseSeeder/Seeders/ItemSeeder.MedievalHouseholdTools.cs`.
- Intermediate stock item prototypes live in `DatabaseSeeder/Seeders/ItemSeeder.MedievalComponentGaps.cs`.
- The six additional food-production tool and apparatus prototypes live in `DatabaseSeeder/Seeders/ItemSeeder.MedievalFoodProduction.cs`.
- Catalogue metadata lives in `Design Documents/Seeding/FutureMUD_Medieval_Industry_Tools_And_Stock_Item_Catalogue.md`.
- The combined item sources now create 174 tool/workshop prototypes and 50 intermediate stock prototypes.
- `SeedMedievalProductionChainCrafts` now creates 16 first-tier stock crafts, 17 tool/apparatus crafts, and two forge/furnace activation crafts under the `Medieval Industry Foundations` knowledge.
- Craft dependencies record phase and source ownership. Exact Medieval inputs come only from earlier phases; functional tools resolve through Historic Foundation, Primary Production, or earlier Medieval items.
- Primary Production owns charcoal, extraction tools, ore and metal preparation, quarrying, clay, brick, and salt inputs, so the Medieval slice does not duplicate those prototypes.
- `SeedMedievalComponentGapCrafts` remains a no-op for the later stock rows outside this first foundation.

## Active Food, Beverage, and Preservation Source

The generic Medieval subsistence foundation is now an active item-and-craft source:

- Items and four reusable `PreparedFood` components live in `DatabaseSeeder/Seeders/ItemSeeder.MedievalFoodProduction.cs`.
- The 48-craft catalogue lives in `DatabaseSeeder/Seeders/ItemSeeder.Crafting.MedievalFood.cs`.
- Shared commodity ownership lives in `DatabaseSeeder/Seeders/ItemSeeder.PreIndustrialFoodFoundation.cs`.
- Current design metadata lives in `Design Documents/Seeding/Medieval_Food_Beverage_Preservation_Foundation.md`.
- Phase 1 creates 17 tools and apparatus, phase 2 creates 18 processed stocks, and phase 3 creates eleven prepared foods plus filled ale and wine casks.
- Cooking oil, amber ale, and red wine reuse existing liquids and existing Medieval vessels through direct `LiquidProduct` outputs.
- Animal Butchery owns raw cut supply and now distinguishes fish from non-fish cuts beneath the compatible `Raw Meat Cut` parent.
- Regional foodways, feasts, sweets, dairy, mead, vinegar, dried fruit, specialist condiments, and culture-specific beverages remain deferred.

## Active Clothing Source

The live medieval clothing item source is intentionally direct-call only:

- Item prototypes live in `DatabaseSeeder/Seeders/ItemSeeder.MedievalClothing.cs`.
- Catalogue metadata lives in `Design Documents/Seeding/Medieval_Clothing_Seeder_Design_Reference.md`.
- Full descriptions live in `Design Documents/Seeding/Medieval_Clothing_FDesc_Catalogue.csv`.
- Each clothing garment is represented by exactly one `CreateItem(...)` call in `SeedMedievalClothing`.
- Clothing crafts are not rebuilt yet; `SeedMedievalClothingCrafts` remains a no-op.

## Active Military Goods Source

The live medieval military item source is intentionally direct-call only:

- Military design metadata lives in `Design Documents/Seeding/Medieval_Military_Seeder_Design_Reference.md`.
- Melee weapons, ranged weapons, ammunition, and thrown weapons live in `DatabaseSeeder/Seeders/ItemSeeder.MedievalWeapons.cs`.
- Armour, horse tack, barding, shields, and military support gear live in `DatabaseSeeder/Seeders/ItemSeeder.MedievalArmour.cs`.
- Each military-goods prototype is represented by exactly one `CreateItem(...)` call in its owning method.
- Military crafts are not rebuilt yet; the medieval craft launch points remain no-op methods.

## Active Household Goods and Furniture Source

The live medieval household goods and furniture item source is intentionally direct-call only:

- Item prototypes live across `DatabaseSeeder/Seeders/ItemSeeder.MedievalContainers.cs`, `DatabaseSeeder/Seeders/ItemSeeder.MedievalDoorsLocksStrongboxes.cs`, `DatabaseSeeder/Seeders/ItemSeeder.MedievalFood.cs`, `DatabaseSeeder/Seeders/ItemSeeder.MedievalFurniture.cs`, and `DatabaseSeeder/Seeders/ItemSeeder.MedievalJewellery.cs`.
- Catalogue metadata lives in `Design Documents/Seeding/Medieval_Household_Goods_Furniture_Seeder_Design_Reference.md`.
- Each household-goods prototype is represented by exactly one `CreateItem(...)` call in its owning medieval household method.
- Furniture and container crafts are not rebuilt yet; `SeedMedievalFurnitureAndContainerCrafts` remains a no-op.

## Active Decorative Jewellery Source

The live medieval decorative jewellery item source is intentionally direct-call only:

- Decorative jewellery item prototypes live in `DatabaseSeeder/Seeders/ItemSeeder.MedievalJewellery.cs`.
- Jewellery design metadata lives in `Design Documents/Seeding/Medieval_Jewellery_Seeder_Design_Reference.md`.
- Full structured item rows live in `Design Documents/Seeding/FutureMUD_Medieval_Jewellery_Item_Catalogue_Full.csv`.
- Full descriptions live in `Design Documents/Seeding/FutureMUD_Medieval_Jewellery_FDesc_Catalogue.csv`.
- Each decorative jewellery prototype is represented by exactly one `CreateItem(...)` call in `SeedMedievalJewelleryAndDevotionalGoods`.
- Jewellery and devotional crafts are not rebuilt yet; `SeedMedievalJewelleryDevotionalCrafts` remains a no-op.

## Active Writing, Books, and Documents Source

The live medieval writing, books, and documents item source is intentionally direct-call only:

- Item prototypes live in `DatabaseSeeder/Seeders/ItemSeeder.MedievalWriting.cs`.
- Catalogue metadata lives in `Design Documents/Seeding/FutureMUD_Medieval_Writing_Books_Documents_Design_Reference.md`.
- Full descriptions live in `Design Documents/Seeding/FutureMUD_Medieval_Writing_Books_Documents_FDesc_Catalogue.csv`.
- Each writing, book, document, seal, container, scribal-tool, and writing-support prototype is represented by exactly one `CreateItem(...)` call in `SeedMedievalWritingAdministrationAndDocuments`.
- Writing and administration crafts are not rebuilt yet; `SeedMedievalWritingAdministrationCrafts` remains a no-op.

## Active Treatment, Drug, and Repair Source

The live medieval treatment, drug-delivery, mobility, prosthetic, and specialist-repair source is intentionally direct-call only:

- Item prototypes live in `DatabaseSeeder/Seeders/ItemSeeder.MedievalMedical.cs` and `DatabaseSeeder/Seeders/ItemSeeder.MedievalRepairKits.cs`.
- Merged design metadata and final catalogue rows live in `Design Documents/Seeding/FutureMUD_Medieval_Treatment_Drugs_Repair_Kits_Design_Reference.md`.
- Each treatment, apothecary, drug-delivery, mobility, prosthetic, repair-kit, and repair-supply prototype is represented by exactly one `CreateItem(...)` call in its owning method.
- Medieval health-tier seeding, medicinal liquids, medicine vessels, and fumigation components live in `DatabaseSeeder/Seeders/HealthSeeder.cs`.
- Specialist glass, paper, lacquer, cordage, and composite-bow repair kit components live in `DatabaseSeeder/Seeders/UsefulSeeder.ItemComponents.cs`.
- Supporting tag paths live in `DatabaseSeeder/Seeders/UsefulSeeder.Tags.cs` and the path-aware HealthSeeder liquid-tag helper.
- Maintained exports are synchronized in `Design Documents/Data/Seeded_Item_Components.json`, `Seeded_Liquids.json`, `Item_Component_Types.json`, `Seeded_Materials.json`, and `SeededTagHierarchy.csv`.
- Treatment, drug, and repair crafts are not rebuilt yet; `SeedMedievalMedicalApothecaryCrafts` and `SeedMedievalRepairKitCrafts` remain no-op methods.

## Shared Historic Foundations

The shared `historic_*` workshop foundation content remains active for antiquity or medieval installs. It is not part of the medieval reset payload.

- Item prototypes live in `DatabaseSeeder/Seeders/ItemSeeder.HistoricFoundation.cs`.
- Crafts live in `DatabaseSeeder/Seeders/ItemSeeder.Crafting.HistoricFoundation.cs`.
- Focused tests keep these files separate from medieval-named partials so future work does not confuse cross-era foundations with medieval-specific stock.

## Documentation Policy

The retired medieval catalogue and suite documents were removed with the implementation they described. New medieval design documents should exist only when they are the current source of truth for a rebuilt slice or when an accepted implementation plan needs a durable specification.

New medieval documents should:

- describe the new from-scratch architecture rather than the retired outfit/catalogue model;
- identify exact source files and launch methods touched by the new slice;
- avoid claiming seeded item, outfit, craft, or catalogue coverage until code and tests actually provide it;
- update this audit or replace it with the new current-state document as each rebuilt slice lands.
