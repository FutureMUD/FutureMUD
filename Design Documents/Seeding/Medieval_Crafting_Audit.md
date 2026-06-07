# Medieval ItemSeeder Rebuild Audit

The medieval `ItemSeeder.Rework` item and craft implementation was reset to launch stubs for a from-scratch rebuild. The rebuild has now begun with direct seeded clothing prototypes.

## Current Runtime State

- `ItemSeeder.Rework.cs` still dispatches the medieval item launch methods when the `medieval` era is selected.
- `ItemSeederCrafting.cs` still dispatches the medieval craft launch methods.
- `SeedMedievalClothing` is the first rebuilt medieval item slice and now contains the direct clothing item `CreateItem(...)` calls.
- The other `ItemSeeder.Rework.Medieval*.cs` category files are currently no-op launch points only.
- `ItemSeederCrafting.Medieval.cs` currently contains no-op medieval craft launch points only.
- The old authored outfit catalogue, explicit culture catalogue, generated helper/data model, and medieval craft helper families have been removed.

## Active Clothing Source

The live medieval clothing item source is intentionally direct-call only:

- Item prototypes live in `DatabaseSeeder/Seeders/ItemSeeder.Rework.MedievalClothing.cs`.
- Catalogue metadata lives in `Design Documents/Crafting/Medieval_Clothing_Seeder_Design_Reference.md`.
- Full descriptions live in `Design Documents/Crafting/Medieval_Clothing_FDesc_Catalogue.csv`.
- Each clothing garment is represented by exactly one `CreateItem(...)` call in `SeedMedievalClothing`.
- Clothing crafts are not rebuilt yet; `SeedMedievalClothingCrafts` remains a no-op.

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
