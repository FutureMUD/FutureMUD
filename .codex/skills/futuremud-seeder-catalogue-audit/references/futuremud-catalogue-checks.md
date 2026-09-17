# FutureMUD Catalogue Checks

## Source Families

Use these as starting points, then search broadly for the live checkout:

- Item components: `Design Documents/Seeded_Item_Components.json`, `DatabaseSeeder/Seeders/*`, `CoreDataSeeder.cs`, `HumanSeederBodyparts.cs`, `CombatSeeder.cs`, `CookingSeeder.cs`, `HealthSeeder.cs`, `UsefulSeeder.ItemComponents.cs`, and runtime singleton/system component prototypes.
- Tags: `UsefulSeeder.Tags.cs`, `CoreDataSeeder.cs`, `CoreDataSeeder.Gases.cs`, `CoreDataSeeder.Terrain.cs`, `UsefulSeeder.Autobuilder.WildernessGroupedTerrain.cs`, `CookingSeeder.cs`, and `CombatAuxiliarySeederHelper.cs`.
- Materials and liquids: `CoreDataSeeder.Materials.cs`, material/liquid inventory JSON files, and any rename/repair logic in the seeder path.
- Gases and terrains: `CoreDataSeeder.Gases.cs`, `CoreDataSeeder.Terrain.cs`, breathable-atmosphere assignments, atmosphere ordering, vacuum/liquid-atmosphere cases, and terrain rerun repair.
- Stock races and creatures: `AnimalSeeder*`, `MythicalAnimalSeeder*`, `SupernaturalSeeder*`, `RobotSeeder*`, AI templates, diet helpers, attribute helpers, description/prose helpers, bodypart/body prototype helpers, attack aliases, and focused seeder tests.

## Component, Liquid, Gas, and Solid Seeder Source Map

Use this map to jump straight to seeders that create rows in the component/material media catalogues. Still re-run the source searches in the live checkout before declaring an export complete, because new partial seeder files are easy to miss.

Definitions for this map:

- `components` means `GameItemComponentProto` rows.
- `liquids` means `Liquid` rows.
- `gases` means `Gas` rows.
- `solids` means `Material` rows, usually with `MaterialType.Solid` or equivalent seeded type data.
- Relationship-only files are listed separately because they add component links, breathable media rows, terrain atmospheres, or edible material rows without creating the catalogue row itself.

Row-creating seeders and implementation files:

| Seeder | Files to inspect | Rows created |
| --- | --- | --- |
| `CoreDataSeeder` | `DatabaseSeeder/Seeders/CoreDataSeeder.cs` | Core component prototypes and the unknown material singleton. |
| `CoreDataSeeder` | `DatabaseSeeder/Seeders/CoreDataSeeder.Materials.cs` | Base and expanded solid materials, liquids, dried residues, and material/liquid tags and aliases. |
| `CoreDataSeeder` | `DatabaseSeeder/Seeders/CoreDataSeeder.Gases.cs` | Stock gases, breathable-atmosphere gas variants, economic gases, gas tags, and gas count-as repair. |
| `HumanSeeder` | `DatabaseSeeder/Seeders/HumanSeederBodyparts.cs` | Human/bodypart component prototypes and core human flesh, viscera, bone, and related solid materials. |
| `HumanSeeder` | `DatabaseSeeder/Seeders/HumanSeeder.cs` | Human blood/sweat liquids and dried blood/sweat solid residues. |
| `AnimalSeeder` | `DatabaseSeeder/Seeders/AnimalSeeder.cs` | Animal blood/sweat liquids and residues, attack liquids, animal spittle, animal acid, and animal-template blood/sweat variants. |
| `AnimalSeeder` | `DatabaseSeeder/Seeders/AnimalSeeder.TemplateApplication.cs` | Template-applied attack liquids. |
| `AnimalButcherySeeder` | `DatabaseSeeder/Seeders/AnimalButcherySeeder.cs` | Signature butchery solid materials such as dragon meat, draconic hide/scale, chitin, mythic feather, and blubber. |
| `CultureSeeder` | `DatabaseSeeder/Seeders/CultureSeederHeritage.cs` | Heritage-specific blood/sweat liquids and dried blood/sweat solid residues. |
| `RobotSeeder` | `DatabaseSeeder/Seeders/RobotSeeder.Shared.cs` | Robot-specific cloned solid materials and liquids. |
| `SupernaturalSeeder` | `DatabaseSeeder/Seeders/SupernaturalSeeder.Support.cs` | Supernatural solid materials such as spirit energy. |
| `CombatSeeder` | `DatabaseSeeder/Seeders/CombatSeeder.cs` | Combat, armor, shield, weapon, ranged-weapon, cartridge, and ammunition component prototypes. |
| `CookingSeeder` | `DatabaseSeeder/Seeders/CookingSeeder.cs` | Cooking/prepared-food component prototypes. |
| `HealthSeeder` | `DatabaseSeeder/Seeders/HealthSeeder.cs` | Drug-delivery example component prototypes. |
| `UsefulSeeder` | `DatabaseSeeder/Seeders/UsefulSeeder.ItemComponents.cs` | General, utility, modern, container, liquid-container, door/lock, writing, insulation, identity, destroyable, variable, furniture, worn, worn-trait, health, prosthetic, dice, lighting, water-source, repair-kit, builder-example, and smokeable component prototypes. |

Relationship-only or consumer files that often matter during audits:

| Seeder/helper | Files to inspect | Rows affected |
| --- | --- | --- |
| `AnimalButcherySeeder` | `DatabaseSeeder/Seeders/AnimalButcherySeeder.cs` | Attaches existing item component prototypes to generated butchery item prototypes. |
| `CombatSeeder` | `DatabaseSeeder/Seeders/CombatSeeder.cs` | Attaches component prototypes to seeded combat item prototypes. |
| `CookingSeeder` | `DatabaseSeeder/Seeders/CookingSeeder.cs` | Attaches cooking component prototypes to seeded food item prototypes. |
| `CoreDataSeeder` | `DatabaseSeeder/Seeders/CoreDataSeeder.cs` | Attaches core component prototypes to singleton item prototypes. |
| `CoreDataSeeder` | `DatabaseSeeder/Seeders/CoreDataSeeder.Terrain.cs` | Consumes gases/liquids for terrain atmosphere and liquid-atmosphere assignments. |
| `HumanSeeder` | `DatabaseSeeder/Seeders/HumanSeeder.cs` | Adds breathable gas relationships for human races. |
| `AnimalSeeder` | `DatabaseSeeder/Seeders/AnimalSeeder.cs` | Adds breathable gas/liquid relationships for animal races. |
| `MythicalAnimalSeeder` | `DatabaseSeeder/Seeders/MythicalAnimalSeeder.cs` | Adds breathable gas/liquid relationships for mythical animal races. |
| `RobotSeeder` | `DatabaseSeeder/Seeders/RobotSeeder.Races.cs` | Clears breathable gas/liquid relationships for robot non-breather races. |
| `SupernaturalSeeder` | `DatabaseSeeder/Seeders/SupernaturalSeeder.cs` | Adds or clears breathable gas/liquid relationships for supernatural races. |
| `ItemSeeder` | `DatabaseSeeder/Seeders/ItemSeeder.cs` | Attaches existing component prototypes to item prototypes; does not create component prototypes. |
| `ItemSeeder` | `DatabaseSeeder/Seeders/ItemSeeder.Rework.cs` | Attaches existing component prototypes to rework item prototypes; does not create component prototypes. |
| `UsefulSeeder` | `DatabaseSeeder/Seeders/UsefulSeeder.ItemComponents.cs` | Attaches component prototypes to sample/provided item prototypes in addition to creating many component prototypes. |
| `NonHumanForageDietSeederHelper` | `DatabaseSeeder/Seeders/NonHumanForageDietSeederHelper.cs` | Adds edible-material relationships; does not create material rows. |

Fast refresh commands:

```powershell
rg "\.GameItemComponentProtos\.Add\(|new\s+GameItemComponentProto\b" DatabaseSeeder/Seeders -g "*.cs"
rg "\.Liquids\.Add\(|new\s+Liquid\b" DatabaseSeeder/Seeders -g "*.cs"
rg "\.Gases\.Add\(|new\s+Gas\b" DatabaseSeeder/Seeders -g "*.cs"
rg "\.Materials\.Add\(|new\s+Material\b" DatabaseSeeder/Seeders -g "*.cs"
rg "GameItemProtosGameItemComponentProtos\.Add|RacesBreathableGases\.Add|RacesBreathableLiquids\.Add|RacesEdibleMaterials\.Add" DatabaseSeeder/Seeders -g "*.cs"
```

## Invariants

- The artifact must be source-backed. Do not treat the old JSON/TSV as self-authoritative when drift is suspected.
- Preserve requested export shape exactly, especially column names, hierarchy strings, delimiter choice, and casing.
- Preserve original order for maintained catalogues when that keeps diffs reviewable.
- Include engine-created singleton/system prototypes when they are real seeded/runtime rows.
- Include sibling seeder sources before declaring a catalogue complete.
- Treat duplicate-safe helpers as first-definition-wins unless later repair logic intentionally changes the row.
- For rerunnable seeders, check both fresh-install and rerun repair behavior when the task touches seeded rows that may already exist.

## Known Pitfalls

- `HumanSeederBodyparts.cs` can be the authority for concrete wearable component names; stale generic `Wear_Profile_*` catalogue rows may not reflect current seeding.
- Core item-component singletons have included rows such as `Holdable`, `CurrencyPile`, `Corpse`, `Bodypart`, `ActiveCraft`, `Pile`, and `Commodity`.
- Tag hierarchy exports can be parent-order sensitive. Build a parent-aware hierarchy rather than assuming creation order is topological.
- `CoreDataSeeder.Materials.cs` has used repair/rename behavior such as `Water Soluable` to `Water Soluble`; final exports should reflect installed behavior.
- Atmosphere seeding can be order-sensitive when terrains refer to gases. Check whether gases exist before terrain assignment and whether rerun repair updates existing terrains.
- A one-off parser can undercount constructor/helper calls. Validate extraction counts against source searches and spot checks.
- Large assertion failure messages can break some test helpers. Use boolean source contains checks for source-invariant tests.