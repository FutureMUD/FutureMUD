# Medieval Culture Catalogue

This document is the authoritative second-pass catalogue target for explicit medieval culture content.

The complete clothing requirement has moved to `Medieval_Outfit_Catalogue.md`. The existing generated status-role wardrobe can remain as common baseline stock, but it must not satisfy explicit outfit coverage unless those common pieces are deliberately referenced inside complete outfit definitions.

## Catalogue Rules

- Exact outfit references live in `Medieval_Outfit_Catalogue.md`.
- Explicit culture items should be implemented as named item specs and final product crafts, not as cue text appended to generic templates.
- Generic baseline items must be labelled as generic baseline.
- Food items should be actual food/beverage items unless explicitly marked as tableware or vessel stock.
- Wooden, wax, birchbark, and non-paper writing surfaces should use `InscribableSurface`-style components where available.
- Military, writing, food, household, and devotional catalogues should coordinate with outfit role items.

## Outfit Coverage Summary

Every culture requires 12 complete outfit definitions:

| Sex | Classes |
| --- | --- |
| male | peasant, artisan, merchant, noble, religious, military |
| female | peasant, artisan, merchant, noble, religious, military |

The exact outfit references use:

```text
medieval_outfit_{culture}_{sex}_{class}
```

## Non-Clothing Catalogue Targets

The earlier non-clothing targets still apply:

| Surface | Minimum Explicit Culture Catalogue Target |
| --- | --- |
| Military/equipment | At least 8 explicit military/equipment items per culture, excluding the current generic armour/weapon/shield/accessory set. |
| Food and beverage | At least 8 explicit food/beverage items per culture; tableware may count only as the vessel slot, not as prepared food. |
| Writing and administration | At least 6 explicit writing/admin items per culture, with culture-appropriate media. |
| Household/devotional/luxury goods | At least 5 explicit non-clothing, non-military household/devotional/luxury items per culture. |

## Culture List

| Key | Slice |
| --- | --- |
| `early_anglo_saxon` | Early Anglo-Saxon/Insular |
| `anglo_danish` | Late Anglo-Saxon/Anglo-Danish |
| `norse` | Norse |
| `norman` | Norman/Angevin |
| `high_british` | High Medieval Britain/Marcher |
| `gaelic` | Gaelic/Welsh/Highland |
| `carolingian` | Carolingian/Frankish |
| `capetian` | Capetian/Low Countries |
| `german_hre` | German/HRE/Alpine-North Italian |
| `iberian_christian` | Iberian Christian |
| `andalusi` | al-Andalus/Maghreb |
| `byzantine` | Byzantine |
| `abbasid` | Abbasid/Persianate |
| `fatimid` | Fatimid Egypt/Ifriqiya |
| `seljuk_ayyubid` | Seljuk/Ayyubid/early Mamluk |
| `rus_novgorod` | Kyivan Rus/Novgorod |
| `steppe_turkic` | Steppe Turkic/Cuman/Mongol-adjacent |
| `song_china` | Song China |

## Documentation Boundary

This document intentionally does not list every clothing piece in every outfit. That list belongs in `Medieval_Outfit_Catalogue.md`.

Use this document for non-clothing explicit culture goods and for links between clothing outfit role items and the wider craft suites.
