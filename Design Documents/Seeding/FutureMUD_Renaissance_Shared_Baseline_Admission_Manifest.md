# FutureMUD Renaissance Shared Baseline Admission Manifest

## Purpose

This manifest governs historical admission of implemented shared pre-industrial stock into Renaissance content, approximately 1400-1600 CE. It maps existing rows to cultures, dates, institutions, professions, shops, military systems, and crafts; it never clones item prototypes.

## Admission record

Every use of shared stock must record the stable reference or family, culture/contact-zone grouping, supported date window, admitting institution/profession/shop/craft, prevalence, import/export status, and whether current components make it functional or descriptive.

## Shared families

| Family | Renaissance admission rule |
| --- | --- |
| `preindustrial_writing_*` | Broad manuscript/account use; print-specific exposure remains regional and institutional. |
| `preindustrial_trade_*` | Admit the container form by real trade context; named packaging does not prove its commodity exists. |
| `preindustrial_door_*` | Admit promoted ordinary forms; culture-specific architecture remains separate. |
| `preindustrial_clothing_*` | Admit the three generic accessories only; fitted and regional clothing belongs to the clothing branch. |
| `preindustrial_tool_*` and `preindustrial_workshop_*` | Admit by technology and craft. Tool availability does not imply a completed production chain. |
| `preindustrial_military_support_*` | Admit through a concrete weapon, armour, unit, armoury, ship, or pageant context. |
| `preindustrial_time_*` and `preindustrial_water_*` | Admit by settlement/institution and chronology. |
| `preindustrial_printing_*` | Admit movable-type stock by region and print institution; do not universalise it. |
| navigation/science families | Admit by profession and culture. The telescope is restricted to the very late edge around 1600. |
| `preindustrial_firearms_*` | Admit only beside an approved firearm system; these are support objects, not weapons or explosives. |

## High-risk named stock

The telescope, gunpowder-support set, tea chest, coffee sack, cacao sack, tobacco bale, sugar hogshead, indigo cake box, porcelain crate, bottle crate, silk bale, and cotton bale require explicit date/culture/trade or military admission. Automatic catalogue installation must not create ordinary fifteenth- or sixteenth-century availability.

## Institution overlays

At minimum support court, civic government, guild, church/chapel, mosque/madrasa, synagogue, temple/shrine, university/school, print shop, bookshop, armoury, military company, dock/ship, customs house, merchant house, workshop, early colonial office, and mission/contact-zone contexts.

## Acceptance criteria

- No shared row is cloned merely to gain a `renaissance_*` prefix.
- Every specialist shared row is gated by date, culture, and institution/profession/craft.
- References resolve to live `preindustrial_*`, `historic_*`, or `primary_production_*` rows.
- Functional claims match components.
- Running or regenerating the manifest creates no item prototypes.
