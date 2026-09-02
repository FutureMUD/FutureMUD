# Era Seeder Shared Architecture

This document describes the shared era item/craft records used by rework seeders. Antiquity and Medieval both have live seeded catalogues. The shared pre-industrial baseline now supplies stable cross-era workshop, production, writing, trade, civic, and military-support stock to Antiquity, Medieval, Renaissance, and Early Modern selections without renaming either era's source rows.

## Shared Records

`DatabaseSeeder/Seeders/ItemSeeder.EraDefinitions.cs` owns the reusable records:

- `EraItemSpec` for seeded item prototypes.
- `EraClothingPieceSpec` for clothing entries that also know outfit usage and craft data.
- `EraOutfitSpec` and `EraOutfitSlotSpec` for outfit catalogues.
- `EraCraftSpec`, `EraCraftInputSpec`, `EraCraftToolSpec`, and `EraCraftProductSpec` for craft definitions, including product definitions that need seed-time item IDs.
- `EraCultureSpec` for era culture keys and display/tag names.
- `EraVariableColourPolicy` for default colour-component policy.
- `EraSeederConfiguration` for era-level differences.

Shared records must stay era-neutral. They should not contain assumptions from any one era such as fixed social roles, complete outfit requirements, or retired era-specific planning labels. Those details belong in the era-specific configuration or data when that era has a live implementation.

## Configuration

Each era declares an `EraSeederConfiguration`.

Antiquity sets complete outfit catalogues to false and keeps its authored clothing suites without forcing them into a complete outfit matrix. A future medieval rebuild should choose its own configuration when the new catalogue exists.

Configuration should carry differences such as:

- era key and root tag
- culture and status/social-role tag roots
- stable-reference prefix policy
- default market tags
- default variable-colour policy
- craft knowledge/category defaults
- common material stock names
- clothing slot definitions
- whether complete outfit catalogues are required
- whether generic baseline wardrobe generation is allowed
- whether player-facing descriptions may include culture names

## Adding Future Era Content

Add future era content by defining data first:

1. Add or extend the era configuration.
2. Add culture specs and optional slot definitions.
3. Add `EraItemSpec` entries for common items.
4. Add outfit specs only if the era owns complete outfits.
5. Add `EraClothingPieceSpec` entries when an item needs outfit metadata and craft data together.
6. Seed through shared helpers such as `SeedEraItemSpecs(...)` where possible.

If an era needs different behaviour, express it through configuration or a small strategy helper. Do not copy a retired helper family and edit the copy.

## Industrialised era registry and installer boundary

The later-era programme is registered centrally by `ItemSeeder.IndustrialisedArchitecture.cs`. Canonical ordinary-item keys are `industrial`, `modern`, `nuclear` and `information`; `revolution`, `atomic` and `computer` are compatibility aliases used by the established vehicle catalogue. Registry presence is not activation: all four entries remain non-selectable until their executable manifest modules contain real, validated stock.

The installer experience remains inside ItemSeeder. ItemSeeder asks for the world technology profile and any custom composition, maps public era names to internal helpers, and reports missing prerequisites. UsefulSeeder continues to own reusable component prototypes, and domain TSV loaders may live in dedicated helper files. Do not add a second user-facing later-era package or expose UsefulSeeder implementation questions as a substitute for the ItemSeeder profile.

Later eras share `shared-industrialised`, then add one canonical era module. Durable unchanged goods continue to use the existing shared pre-industrial stable identities. See the [Industrialised ItemSeeder Programme](./FutureMUD_Industrialised_Item_Seeder_Programme.md) and [Shared Industrialised Baseline](./FutureMUD_Industrialised_Shared_Baseline_Design_Reference.md).

## Shared pre-industrial compatibility layer

Cross-era reuse does not directly invoke broad earlier-era catalogues. Existing `historic_*` and `primary_production_*` rows retain their stable references, while selected Antiquity and Medieval forms receive `preindustrial_*` aliases with copied physical/component data and source-attribution builder notes. Alias lifecycle targets must resolve to aliases when both endpoints are promoted.

The shared historic workshop and primary-production craft routes are installed for every implemented pre-industrial era. They are the common source for craftable workshop tools and primary commodity transforms; an era-specific finished-item craft may rely on them without duplicating its own tool stock or tool tags.

See [Pre-Industrial Shared Item Seeder Design Reference](./PreIndustrial_Item_Seeder_Design_Reference.md) and its [complete alias catalogue](./PreIndustrial_Item_Seeder_Alias_Catalogue.md). Era admission is governed by the populated [Medieval](./FutureMUD_Medieval_Shared_Baseline_Admission_Manifest.md), [Renaissance](./FutureMUD_Renaissance_Shared_Baseline_Admission_Manifest.md), and [Early Modern](./FutureMUD_EarlyModern_Shared_Baseline_Admission_Manifest.md) manifests. Each contains one decision for all 385 live shared stable references and is generated from source truth with `scripts/generate-preindustrial-admission-manifests.ps1`.

Prepared food, food-liquid, and discrete culinary-intermediate content uses a separate 3,000-record layer described by the [Pre-Industrial Food Catalogue Design Reference](./PreIndustrial_Food_Catalogue_Design_Reference.md). Its 2,250 shared rows are installed once when any of Medieval, Renaissance, or Early Modern is selected; each era then adds 250 genuinely era-specific rows. The shared food admission manifests are generated independently because ingredient contact, crop availability, consumption culture, and trade status are more restrictive than physical item compatibility.

## No Patch-After-Create Clothing

Authored item rows are the source of truth. Do not create generated clothing and then repair selected rows with later patch records. A maintainer should be able to inspect one catalogue seam and understand the final item description, components, craft inputs/tools, craft products, variable-colour product mappings, and outfit usage.

For colourable garments, the item description must include the matching variables used by the item component: `$colour`, or `$colour1` and `$colour2`. Craft products for those garments must use `SimpleVariableProduct` mappings from colour-bearing inputs.
