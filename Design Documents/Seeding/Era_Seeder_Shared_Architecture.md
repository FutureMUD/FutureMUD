# Era Seeder Shared Architecture

This document describes the shared era item/craft records used by rework seeders. Antiquity currently uses this architecture for live seeded content. The medieval rework path has been reset to no-op launch stubs, so medieval-specific configuration and catalogue examples should be treated as retired context until the from-scratch rebuild adds new source truth.

## Shared Records

`DatabaseSeeder/Seeders/ItemSeeder.Rework.EraDefinitions.cs` owns the reusable records:

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

## No Patch-After-Create Clothing

Authored item rows are the source of truth. Do not create generated clothing and then repair selected rows with later patch records. A maintainer should be able to inspect one catalogue seam and understand the final item description, components, craft inputs/tools, craft products, variable-colour product mappings, and outfit usage.

For colourable garments, the item description must include the matching variables used by the item component: `$colour`, or `$colour1` and `$colour2`. Craft products for those garments must use `SimpleVariableProduct` mappings from colour-bearing inputs.
