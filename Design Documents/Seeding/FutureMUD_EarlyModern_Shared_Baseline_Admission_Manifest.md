# FutureMUD Early Modern Shared Baseline Admission Manifest

## Purpose

This manifest governs when stock from the implemented shared pre-industrial baseline is historically admitted into Early Modern content. It is a dependency and availability policy for the approximate period 1600-1750 CE; it does not author or clone item prototypes.

The implementation sources remain `PreIndustrial_Item_Seeder_Design_Reference.md`, `PreIndustrial_Item_Seeder_Alias_Catalogue.md`, and `SeedSharedPreIndustrialBaselineItems()`.

## Admission record

Every outfit, craft, shop, institution, military package, or culture package that exposes a shared row must record:

- shared stable reference or reference family;
- culture or contact-zone grouping;
- earliest and latest supported date anchor;
- institution, profession, military system, shop, or craft that admits it;
- whether it is ordinary, specialist, elite, imported, export-only, military-controlled, or restricted;
- whether the row is functional or descriptive with its current components.

## Shared families

| Family | Early Modern admission rule |
| --- | --- |
| `preindustrial_writing_*` | Broadly admissible, but literacy, language, institution, and document format remain local concerns. |
| `preindustrial_trade_*` | Admit by actual commodity route and container form; packaging does not imply that its named commodity is seeded. |
| `preindustrial_door_*` | Admit ordinary promoted forms; specialised architecture remains culture- or institution-owned. |
| `preindustrial_clothing_*` | Admit the three generic accessories only; period clothing is owned by the clothing branch. |
| `preindustrial_tool_*` and `preindustrial_workshop_*` | Admit by trade and production technology. A tagged tool is not proof that a complete craft chain exists. |
| `preindustrial_military_support_*` | Admit through a concrete weapon, armour, unit, ship, or armoury context. |
| `preindustrial_time_*` and `preindustrial_water_*` | Admit by settlement infrastructure and local chronology. |
| `preindustrial_printing_*` | Admit movable-type stock by region, institution, and print ecology. |
| `preindustrial_navigation_*`, `preindustrial_surveying_*`, `preindustrial_optics_*`, `preindustrial_science_*` | Admit by profession and date; telescope stock is not universal. |
| `preindustrial_firearms_*` | Admit only alongside an approved firearm system; these rows are support goods, not firearms or explosives. |

## High-risk named stock

The tea chest, coffee sack, cacao sack, tobacco bale, sugar hogshead, indigo cake box, porcelain crate, bottle crate, silk bale, cotton bale, telescope, and gunpowder-support rows require explicit culture/date/trade or military admission. Their automatic catalogue presence must never create universal shop or craft availability.

## Culture and institution overlays

Use the culture groupings in `FutureMUD_EarlyModern_Culture_Manifest_Reference.md`. The minimum institution overlays are state military, naval, court, guild, company, customs/port, postal, legal/notarial, university/academy, coffeehouse/tavern, religious, plantation, mission, and colonial office.

Sensitive plantation, enslavement, mission, company, and colonial contexts must be named deliberately and must not be reduced to neutral commodity flavour.

## Acceptance criteria

- No shared prototype is cloned merely to gain an `earlymodern_*` prefix.
- Admission entries reference live `preindustrial_*`, `historic_*`, or `primary_production_*` rows.
- Date and culture gates exist before shared specialist stock becomes ordinary availability.
- Functional claims match components; descriptive stock remains described as descriptive.
- The manifest can be rerun or regenerated without adding item prototypes.
