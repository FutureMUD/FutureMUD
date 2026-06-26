# FutureMUD Medieval Treatment Drugs and Repair Kits Design Reference

Status: implemented for dependency workstreams A-E; core material gap resolved for Workstream F. Final medieval treatment-item catalogue rows remain out of scope for this document and belong to the next item-catalogue pass.

This document records the implemented repo-local version of the medieval treatment, drug and repair-kit reference. It is the current source of truth for the enabling stock data that medieval medical and specialist-repair items depend on.

## Implementation Scope

The implemented slice covers:

- `HealthSeeder` support for a `medieval` tech level.
- Medieval health knowledge and procedure seeding by reusing the pre-modern human and veterinary branches.
- A medieval drug package of primitive drugs plus selected pre-modern remedies and three new medieval remedies.
- Revisions to mandrake, poppy latex and henbane respiratory-risk payloads, with poppy dependence data.
- Medicinal liquid rows for drinkable medieval remedies.
- Stock medicine-vessel component prototypes for 30ml vials, 100ml bottles, 250ml flasks and the default-loaded medieval remedies.
- Curated medical `IncenseBurner` component prototypes for fumigation stock.
- Specialist repair-kit component prototypes for glass, paper/parchment, lacquerware, cordage and composite bows.
- Supporting tag hierarchy and maintained export catalogue updates.
- Exact core material support for named medieval medical stock that should not be substituted with generic abstractions.

## Runtime Sources

Primary implementation files:

- `DatabaseSeeder/Seeders/HealthSeeder.cs`
- `DatabaseSeeder/Seeders/UsefulSeeder.ItemComponents.cs`
- `DatabaseSeeder/Seeders/UsefulSeeder.Tags.cs`
- `DatabaseSeeder/Seeders/UsefulSeeder.cs`
- `DatabaseSeeder/Seeders/CoreDataSeeder.Materials.cs`

Maintained export files:

- `Design Documents/Data/Seeded_Item_Components.json`
- `Design Documents/Data/Seeded_Liquids.json`
- `Design Documents/Data/Item_Component_Types.json`
- `Design Documents/Data/SeededTagHierarchy.csv`
- `Design Documents/Data/Seeded_Materials.json`

## Core Material Gap Closure

The catalogue-authoring review identified seven materials that were previously called out as things to avoid or substitute with broader stock materials. These should instead be authored as exact core materials because they are visible stock identities in medieval medical items.

Required exact material additions:

- `alum`
- `ephedra`
- `foxglove`
- `gut`
- `henbane`
- `mandrake`
- `yarrow`

These are now part of the catalogue contract. Use them directly where a final item is meant to be that named medical stock. Do not substitute the above with `herb`, `leaf`, `spice`, `salt`, `sinew`, or similar broad abstractions except for generic blends, neutral carriers, or packaging.

Recommended material usage for catalogue rows:

| Use | Exact material guidance |
| --- | --- |
| Yarrow styptic pads or loose yarrow | Use `yarrow`. |
| Mandrake draught stock, smoke stock, or root packets | Use `mandrake`. |
| Henbane smoke or fumigation stock | Use `henbane`. |
| Foxglove tincture stock | Use `foxglove`. |
| Ephedra brew stock | Use `ephedra`. |
| Alum styptic powder or mordant-like mineral medicine stock | Use `alum`. |
| Gut sutures and surgical thread | Use `gut`. |
| Generic mixed herbs or unnamed bundles | Use `herb`, `leaf`, or `spice` only when the item identity is intentionally generic. |

## Medieval Health Seeder Contract

The `techlevel` question accepts `primitive`, `pre-modern`, `medieval` and `modern`.

The `medieval` answer requires `Functions / Material Functions / Medical Craft Stock / Fumigation Stock`. The validator rejects medieval if that tag is absent so the curated incense components can be created against a real fuel-stock tag.

Medieval knowledge and surgery use the pre-modern branches:

- Human knowledges: `Chiurgery`, `Physical Medicine`.
- Optional quadruped knowledges: `Veterinary Medicine`, `Veterinary Chiurgery`.
- Human and veterinary procedure arrays match pre-modern coverage.

## Medieval Drug Catalogue

The medieval package includes these drugs:

- Primitive/revised: `Willow Bark Tea`, `Mandrake Draught`, `Honey Poultice`, `Garlic Salve`, `Mint Infusion`, `Ephedra Brew`, `Foxglove Tincture`, `Aloe Burn Salve`, `Poppy Latex Draught`, `Henbane Smoke`, `Yarrow Styptic`.
- Reused pre-modern helpers: `Mint and Ginger Tonic`, `Herbal Burn Salve`, `Bronchial Smoke`.
- New medieval remedies: `Alum Styptic`, `Theriac Electuary`, `Soporific Fumes`.

The standard HealthSeeder delivery-wrapper pass remains scan-based. It creates wrapper components for supported drug vectors, including:

- `TopicalCream_Alum_Styptic`
- `Pill_Theriac_Electuary`
- `Smokeable_Soporific_Fumes`

## Medicinal Liquids

Medieval medicinal liquids are upserted after drug rows and before default-loaded medicine vessels. Each liquid copies carrier defaults, links to its drug, uses `LiquidInjectionConsequence.Harmful`, and receives `Materials / Liquids / Medicine`.

Seeded liquids:

- `willow bark tea`, carrier `tea`, drug `Willow Bark Tea`, 2.0g/L.
- `mandrake draught`, carrier `watered red wine`, drug `Mandrake Draught`, 2.5g/L.
- `mint infusion`, carrier `tea`, drug `Mint Infusion`, 2.0g/L.
- `ephedra brew`, carrier `tea`, drug `Ephedra Brew`, 2.0g/L.
- `foxglove tincture`, carrier `white wine`, drug `Foxglove Tincture`, 2.0g/L.
- `poppy latex draught`, carrier `watered red wine`, drug `Poppy Latex Draught`, 2.5g/L.
- `mint and ginger tonic`, carrier `tea`, drug `Mint and Ginger Tonic`, 2.0g/L.
- `theriac syrup`, carrier `water`, drug `Theriac Electuary`, 5.0g/L.

## Medicine Vessels and Fumigation

Medicine-vessel component prototypes are opaque, closable, refillable liquid containers that can be emptied in-room. Generic vessels are empty by default; remedy-specific vessels load full of their matching medicinal liquid.

Curated fumigation components are `IncenseBurner` prototypes using `Fumigation Stock`, 250g maximum fuel weight, 60 seconds per unit weight, 30 second drug pulse, source scent range 1, drug range 0 and normal scent difficulty.

## Specialist Repair Kits

Specialist repair-kit components use the Wood-family quality settings:

- Standard: Grievous maximum severity, 1000 repair points, +0 check bonus.
- Good: Horrifying maximum severity, 1500 repair points, +1 check bonus.
- Poor: Severe maximum severity, 600 repair points, -1 check bonus.

Restriction model:

- Glass kits target exact materials `glass`, `silicate glass`, `soda-lime glass`, `lead glass`.
- Paper kits target exact materials `paper`, `parchment`, `papyrus`.
- Lacquer kits target exact material `lacquer`.
- Cordage kits require the `Functions / Repairing / Cordage` tag.
- Composite bow kits require the `Functions / Repairing / Composite Bow` tag.

## Workstream F Catalogue Authoring Notes

The final item-catalogue pass should create ordinary `CreateItem(...)` rows only. It must not reseed drugs, liquids, tags, item components or component types.

Catalogue rows should validate exact material names against `Seeded_Materials.json`, including the seven core medical additions above. The catalogue should also use the already-seeded drug wrappers, medicine-vessel components, fumigation components and repair-kit components by exact component name.

This reference intentionally does not define final `CreateItem(...)` catalogue rows for stocked medieval medicine or repair-supply items. The next medieval item-catalogue pass should use these stock components, liquids, tags, exact materials and repair-kit prototypes as prerequisites.
