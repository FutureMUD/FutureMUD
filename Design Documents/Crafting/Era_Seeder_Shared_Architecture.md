# Era Seeder Shared Architecture

This document describes the shared item/craft/outfit seeder architecture used by the antiquity and medieval rework paths.

## Shared Records

`DatabaseSeeder/Seeders/ItemSeeder.Rework.EraDefinitions.cs` owns the reusable records:

- `EraItemSpec` for seeded item prototypes.
- `EraClothingPieceSpec` for clothing entries that also know outfit usage and craft data.
- `EraOutfitSpec` and `EraOutfitSlotSpec` for outfit catalogues.
- `EraCraftSpec`, `EraCraftInputSpec`, `EraCraftToolSpec`, and `EraCraftProductSpec` for craft definitions, including product definitions that need seed-time item IDs.
- `EraCultureSpec` for era culture keys and display/tag names.
- `EraVariableColourPolicy` for default colour-component policy.
- `EraSeederConfiguration` for era-level differences.

Shared records must stay era-neutral. They should not contain medieval-only assumptions such as six social roles, complete outfit requirements, or `MED-OUTFIT` labels. Those details belong in the medieval configuration or medieval data.

## Configuration

Each era declares an `EraSeederConfiguration`.

Medieval sets complete outfit catalogues to required, allows the generic baseline wardrobe as retained fallback content, and disables direct culture names in player-facing clothing descriptions. Antiquity sets complete outfit catalogues to false and keeps its authored clothing suites without forcing them into the medieval outfit matrix.

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

If an era needs different behaviour, express it through configuration or a small strategy helper. Do not copy the medieval helper family and edit the copy.

## No Patch-After-Create Clothing

Authored item rows are the source of truth. Do not create generated clothing and then repair selected rows with later patch records. A maintainer should be able to inspect one catalogue seam and understand the final item description, components, craft inputs/tools, craft products, variable-colour product mappings, and outfit usage.

For colourable garments, the item description must include the matching variables used by the item component: `$colour`, or `$colour1` and `$colour2`. Craft products for those garments must use `SimpleVariableProduct` mappings from colour-bearing inputs.
