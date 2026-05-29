# Medieval Crafting Catalogue Audit

This document records the source-backed audit boundary and the second-pass quality target for the medieval item and craft slice.

The first medieval pass established useful scaffolding: the `medieval` era dispatcher, shared `historic_*` workshop foundations, broad production-chain stock, 18 culture keys, status-role clothing axes, and craft-suite wiring. That scaffold should be retained. However, the first pass over-relied on generated culture/status matrices and generic cue text. The second pass must add exact outfit catalogues and quality tests so that cultures differ by named material culture, not only by builder notes, tags, and appended phrases.

MED-OUTFIT-006 now provides 180 manually written outfit-piece overrides, 10 per culture, with variable colour support and style-focused player-facing descriptions. Culture metadata remains in stable references, tags, tests, catalogue documentation, and builder notes rather than in visible item prose.

## Core Design Correction

Shared production chains are encouraged; shared final outfits are not.

Medieval upstream stock such as yarn, garment cloth, broadcloth, leather panels, mail rings, paper pulp, sealing wax, weapon blanks, shield boards, lockwork, glazing stock, and dairy/brewing stock should remain broad and reusable. Finished outfits, clothing pieces, foods, documents, weapons, devotional goods, household goods, and visible final craft names should be culturally and socially specific.

## Current Source Boundary

Current medieval item definitions are seeded from:

| Source | Item Surface |
| --- | --- |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs` | Shared `historic_*` foundations, 18 medieval culture slices, status-role clothing, military goods, household tools, writing/administration, measurement, medical/apothecary, jewellery/devotional, food/beverage, furniture/container, repair kits, and component-gap props. |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.cs` | Dispatcher wiring, builder-note culture/status inference, stable reference metadata, and cross-era historic foundation seeding when either `antiquity` or `medieval` is selected. |

Current medieval craft definitions are seeded from:

| Source | Craft Surface |
| --- | --- |
| `ItemSeederCrafting.Medieval.cs` | Historic foundations, medieval production-chain tools and intermediate stocks, clothing, equipment, writing/administration, medical/apothecary, jewellery/devotional, furniture/container, food/beverage, repair kits, and component-gap props. |
| `ItemSeederCrafting.cs` | Dispatcher registration through `SeedHistoricFoundationCrafts()`, `SeedMedievalProductionChainCrafts()`, and the medieval suite methods. |
| `ItemSeederCrafting.Antiquity.cs` | Shared stable-reference lookup helpers used by both antiquity and medieval craft products. |

## Current Culture Slices

The v1 scaffold uses 18 culture/time profiles:

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

These culture keys are still appropriate. The issue is not the selection; it is the shallow culture payload.

## Revised Clothing Target: Complete Outfits

The second pass must not merely add a few signature garments. It must add complete outfit catalogues.

For each culture, seed outfit definitions for:

| Axis | Required Values |
| --- | --- |
| Sex/gender presentation | `male`, `female` |
| Social class/role | `peasant`, `artisan`, `merchant`, `noble`, `religious`, `military` |

This creates **12 complete outfit definitions per culture** and **216 complete outfit definitions across 18 cultures**.

Each complete outfit should include all wearable pieces needed to dress a character, not only a main garment.

## Required Outfit Slots

Each outfit should satisfy all required slots unless explicitly documented as culturally inapplicable:

| Slot | Required? | Examples |
| --- | --- | --- |
| `underlayer` | Required | shirt, shift, chemise, qamis, rubakha, under-robe |
| `lower_body` | Required | braies, trousers, hose, sirwal, skirt, wrapped trews, lower robe layer |
| `leg_or_sock_layer` | Required unless culturally merged into footwear | footwraps, hose, onuchi, socks, leg wraps |
| `footwear` | Required | shoes, boots, sandals, slippers, cloth shoes |
| `bodywear` | Required | tunic, cote, robe, kaftan, dalmatic, hangerok, cross-collar robe |
| `outerwear` | Required for travel/cold cultures; otherwise recommended | cloak, mantle, burnous, riding coat, sagion, felt cloak |
| `headwear` | Required | coif, veil, wimple, cap, turban, fur hat, official cap |
| `belt_or_sash` | Required | belt, girdle, sash, cord, arming belt |
| `worn_container` | Required for non-noble and military; recommended for all | pouch, purse, book pouch, document sleeve, field pouch |
| `fastener_or_jewellery` | Required | brooch, ring pin, cloak clasp, badge, pendant, belt mount |
| `role_item` | Required for merchant, religious, military; recommended for artisan/noble | tool apron, book pouch, devotional item, scabbard, quiver, writing sleeve |

A complete outfit should normally contain 9-12 items. Some pieces may be shared across multiple outfits.

## Sharing Rules

Sharing is allowed and expected, but it must not erase culture identity.

Allowed broad sharing:

- Plain undergarments across nearby Western European cultures.
- Shoes, simple belts, simple pouches, and socks across adjacent classes.
- Military arming shirts and padding where the visual outer layers differ.
- Generic repair kits and workshop tools.
- Generic commoner items inside one culture where male/female differences are primarily headwear, bodywear, lower body, and fasteners.

Required distinctiveness:

- Every outfit must include at least **four culture-specific or culture-cluster-specific pieces**.
- Every outfit must include at least **two class-specific pieces**.
- Male and female variants for the same culture/class must differ in at least **two wearable slots**, unless the outfit is explicitly marked as intentionally unisex.
- Main bodywear, outerwear, and headwear should usually be culture-specific.
- A culture cannot be represented only by generic baseline tunics, generic lined hats, generic rough cloaks, and generic pouches.

## Outfit Catalogue References

Use outfit package references even if there is no runtime item-package system yet. These references can be used in tests, docs, and optional future starter kits:

```text
medieval_outfit_{culture}_{sex}_{class}
```

Examples:

```text
medieval_outfit_norse_male_peasant
medieval_outfit_norse_female_peasant
medieval_outfit_song_china_male_scholar_noble
medieval_outfit_byzantine_female_religious
```

The implementation may represent outfit definitions as internal catalogue records that point to item stable references. If an existing item-package system is suitable, the seeder may additionally create loadable outfit packages, but this is not required for the second pass.

## Stable Reference Guidance For Outfit Pieces

Use stable references that encode culture and item identity:

```text
medieval_clothing_norse_female_hangerok_apron_dress
medieval_clothing_byzantine_male_silk_dalmatic
medieval_clothing_song_china_female_cross_collar_robe
medieval_clothing_gaelic_male_brat_mantle
```

Shared items may use `medieval_common_*` or cluster names:

```text
medieval_common_linen_braies
medieval_common_turnshoe_ankle_shoes
medieval_western_linen_coif
medieval_islamic_wrapped_turban
```

Generic baseline status items may remain, but they must not satisfy explicit outfit counts unless assigned to outfit slots and balanced by culture-specific pieces.

## Production Chain Invariants To Retain

`SeedMedievalProductionChainCrafts()` should remain the shared upstream foundation.

| Chain | Intermediate Stock |
| --- | --- |
| Textile base | `Spun Yarn`, `Garment Cloth`, `Fulled Cloth`, `Broadcloth Stock`, `Embroidered Trim Stock`, `Tablet-Woven Band Stock`, `Silk Brocade Panel`. |
| Leather and footwear | `Prepared Leather Panel`, `Leather Strap`, `Turnshoe Upper Stock`, `Scabbard Leather Stock`, `Bookbinding Leather Stock`. |
| Military | `Weapon Shaft Stock`, `Weapon Blade Stock`, `Weapon Head Stock`, `Fletching Stock`, `Military Cord Stock`, `Shield Board Stock`, `Shield Facing Stock`, `Armour Lamella Stock`, `Mail Wire Stock`, `Armour Ring Stock`, `Mail Panel Stock`, `Quilted Armour Padding`. |
| Crossbow manufacture | `Crossbow Prod Stock`, `Crossbow Tiller Stock`, `Crossbow Lockwork Stock`. |
| Writing/admin | `Paper Pulp Stock`, `Paper Sheet Stock`, `Parchment Sheet Stock`, `Seal Cord Stock`, `Sealing Wax Stock`, `Tally Stick Stock`. |
| Food/beverage | `Flour Commodity`, `Cheese Curd Stock`, `Cheese Wheel Stock`, `Brewing Mash Stock`, `Ale Stock`, `Cider Stock`, `Mead Stock`, `Coopered Staves`, `Hoop Stock`. |
| Household/luxury | `Pottery Clay Body`, `Glaze Slurry Stock`, `Tile Blank Stock`, `Glass Batch`, `Glass Vessel Blank`, `Stained Glass Quarry Stock`, `Lead Came Stock`, `Stained Glass Panel Stock`, `Lantern Pane Stock`, `Lockwork Stock`. |
| Trade and measures | `Standard Weight Blank`, `Sealable Bale Wrapper Stock`. |

## Revised Craft Naming Rule

Upstream production crafts should remain culture-neutral:

```text
spin wool yarn stock
full broadcloth stock
draw mail wire stock
compound sealing wax stock
```

Final product crafts should be product-specific and may name the culture or object:

```text
sew a Norse hangerok apron dress
tailor a Byzantine silk dalmatic
prepare a Song paper register
cook a Rus mushroom and fish pottage
forge a Norman nasal helmet
```

The previous rule that visible final craft names should avoid culture names is no longer a quality goal. It may be retained only for generic baseline stock.

## Required Second-Pass Tests

The second-pass test suite should not only check counts. It should verify outfit completeness.

1. **Outfit completeness tests**
   - Every culture has 12 outfit definitions.
   - Every outfit has the required slots.
   - Every outfit references existing item stable references.

2. **Culture identity tests**
   - Every outfit includes at least four culture-specific or cluster-specific items.
   - Required culture vocabulary appears in outfit pieces and final craft names.

3. **Class identity tests**
   - Every social class has at least two class-specific pieces.
   - Peasant, artisan, merchant, noble, religious, and military outfits should not collapse into the same clothing with different notes.

4. **Sex differentiation tests**
   - Male and female outfit variants for the same culture/class differ in at least two wearable slots unless deliberately marked unisex.

5. **Craft-name tests**
   - Explicit culture final crafts must not use `regional pattern NN`.
   - Generic baseline crafts may still use neutral names if clearly marked as generic.

6. **Exact documentation tests**
   - Exact outfit package references and expected slot contents must appear in `Medieval_Outfit_Catalogue.md`.
   - Exact explicit item references should appear in `Medieval_Culture_Catalogue.md` or in generated test-accessible catalogue data.

## Deferred Scope

The second pass should not attempt full plate armour, hand firearms, spectacles, mechanical clocks, printing presses, procedural surgery, cranequin/windlass/goat's-foot loading components, musical-instrument mechanics, rules-aware game sets, or animal tack systems.

It may add props for these areas if useful, but they should be labelled as deferred component-gap props and should not consume engineering time unless the runtime systems already exist.
