# Medieval ItemSeeder Rebuild Audit

The medieval `ItemSeeder.Rework` item and craft implementation was reset to launch stubs for a from-scratch rebuild. The rebuild has now begun with direct seeded clothing, household goods and furniture, military-goods prototypes, and writing/book/document prototypes.

## Current Runtime State

- `ItemSeeder.Rework.cs` still dispatches the medieval item launch methods when the `medieval` era is selected.
- `ItemSeederCrafting.cs` still dispatches the medieval craft launch methods.
- `SeedMedievalClothing` contains the direct clothing item `CreateItem(...)` calls.
- `SeedMedievalHouseholdFurniture` contains the direct household goods, furniture, lighting, vessel, water-source, door, lock, and decoration `CreateItem(...)` calls.
- `SeedMedievalWeaponsShieldsAccessories` contains the direct melee weapon, ranged weapon, ammunition, and thrown-weapon `CreateItem(...)` calls.
- `SeedMedievalArmour` contains the direct armour, horse tack, barding, shield, and military support-gear `CreateItem(...)` calls.
- `SeedMedievalWritingAdministrationAndDocuments` contains the direct writing-surface, book, document, seal, container, scribal-tool, and writing-support `CreateItem(...)` calls.
- The remaining `ItemSeeder.Rework.Medieval*.cs` category files are currently no-op launch points only.
- `ItemSeederCrafting.Medieval.cs` currently contains no-op medieval craft launch points only.
- The old authored outfit catalogue, explicit culture catalogue, generated helper/data model, and medieval craft helper families have been removed.

## Active Clothing Source

The live medieval clothing item source is intentionally direct-call only:

- Item prototypes live in `DatabaseSeeder/Seeders/ItemSeeder.Rework.MedievalClothing.cs`.
- Catalogue metadata lives in `Design Documents/Seeding/Medieval_Clothing_Seeder_Design_Reference.md`.
- Full descriptions live in `Design Documents/Seeding/Medieval_Clothing_FDesc_Catalogue.csv`.
- Each clothing garment is represented by exactly one `CreateItem(...)` call in `SeedMedievalClothing`.
- Clothing crafts are not rebuilt yet; `SeedMedievalClothingCrafts` remains a no-op.

## Active Military Goods Source

The live medieval military item source is intentionally direct-call only:

- Military design metadata lives in `Design Documents/Seeding/Medieval_Military_Seeder_Design_Reference.md`.
- Melee weapons, ranged weapons, ammunition, and thrown weapons live in `DatabaseSeeder/Seeders/ItemSeeder.Rework.MedievalWeapons.cs`.
- Armour, horse tack, barding, shields, and military support gear live in `DatabaseSeeder/Seeders/ItemSeeder.Rework.MedievalArmour.cs`.
- Each military-goods prototype is represented by exactly one `CreateItem(...)` call in its owning method.
- Military crafts are not rebuilt yet; the medieval craft launch points remain no-op methods.

## Active Household Goods and Furniture Source

The live medieval household goods and furniture item source is intentionally direct-call only:

- Item prototypes live in `DatabaseSeeder/Seeders/ItemSeeder.Rework.MedievalFurniture.cs`.
- Catalogue metadata lives in `Design Documents/Seeding/Medieval_Household_Goods_Furniture_Seeder_Design_Reference.md`.
- Each household-goods prototype is represented by exactly one `CreateItem(...)` call in `SeedMedievalHouseholdFurniture`.
- Furniture and container crafts are not rebuilt yet; `SeedMedievalFurnitureAndContainerCrafts` remains a no-op.

## Active Writing, Books, and Documents Source

The live medieval writing, books, and documents item source is intentionally direct-call only:

- Item prototypes live in `DatabaseSeeder/Seeders/ItemSeeder.Rework.MedievalWriting.cs`.
- Catalogue metadata lives in `Design Documents/Seeding/FutureMUD_Medieval_Writing_Books_Documents_Design_Reference.md`.
- Full descriptions live in `Design Documents/Seeding/FutureMUD_Medieval_Writing_Books_Documents_FDesc_Catalogue.csv`.
- Each writing, book, document, seal, container, scribal-tool, and writing-support prototype is represented by exactly one `CreateItem(...)` call in `SeedMedievalWritingAdministrationAndDocuments`.
- Writing and administration crafts are not rebuilt yet; `SeedMedievalWritingAdministrationCrafts` remains a no-op.

## Active Treatment, Drug, and Repair Dependencies

The live medieval medical and specialist-repair dependency source is implemented, but final medieval treatment item `CreateItem(...)` rows are intentionally deferred:

- Design metadata lives in `Design Documents/Seeding/FutureMUD_Medieval_Treatment_Drugs_Repair_Kits_Design_Reference.md`.
- Medieval health-tier seeding, medicinal liquids, medicine vessels, and fumigation components live in `DatabaseSeeder/Seeders/HealthSeeder.cs`.
- Specialist glass, paper, lacquer, cordage, and composite-bow repair kit components live in `DatabaseSeeder/Seeders/UsefulSeeder.ItemComponents.cs`.
- Supporting tag paths live in `DatabaseSeeder/Seeders/UsefulSeeder.Tags.cs` and the path-aware HealthSeeder liquid-tag helper.
- Maintained exports are synchronized in `Design Documents/Data/Seeded_Item_Components.json`, `Seeded_Liquids.json`, `Item_Component_Types.json`, and `SeededTagHierarchy.csv`.
- Final stocked medieval apothecary and repair-supply item rows remain a later item-catalogue workstream.

## Shared Historic Foundations

The shared `historic_*` workshop foundation content remains active for antiquity or medieval installs. It is not part of the medieval reset payload.

- Item prototypes live in `DatabaseSeeder/Seeders/ItemSeeder.Rework.HistoricFoundation.cs`.
- Crafts live in `DatabaseSeeder/Seeders/ItemSeederCrafting.HistoricFoundation.cs`.
- Focused tests keep these files separate from medieval-named partials so future work does not confuse cross-era foundations with medieval-specific stock.

## Documentation Policy

The retired medieval catalogue and suite documents were removed with the implementation they described. New medieval design documents should exist only when they are the current source of truth for a rebuilt slice or when an accepted implementation plan needs a durable specification.

New medieval documents should:

- describe the new from-scratch architecture rather than the retired outfit/catalogue model;
- identify exact source files and launch methods touched by the new slice;
- avoid claiming seeded item, outfit, craft, or catalogue coverage until code and tests actually provide it;
- update this audit or replace it with the new current-state document as each rebuilt slice lands.
