# Medieval Crafting Catalogue Audit

This document records the source-backed audit boundary for the medieval item and craft slice. It also records the second-pass quality target after review of the first merged implementation.

The first medieval pass established useful scaffolding: the `medieval` era dispatcher, shared `historic_*` workshop foundations, broad production-chain stock, 18 culture keys, status-role clothing axes, and craft-suite wiring. That scaffold should be retained. However, the first pass over-relied on generated culture/status matrices and generic cue text. The second pass must add exact culture catalogues and quality tests so that cultures differ by named material culture, not only by builder notes, tags, and appended phrases.

## Core Design Correction

Shared production chains are encouraged; shared final products are not.

Medieval upstream stock such as yarn, garment cloth, broadcloth, leather panels, mail rings, paper pulp, sealing wax, weapon blanks, shield boards, lockwork, glazing stock, and dairy/brewing stock should remain broad and reusable. Finished clothing, foods, documents, weapons, devotional goods, household goods, and visible final craft names should be culturally and socially specific.

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

## Status Roles

Status/role remains useful, especially for Western European clothing:

| Key | Status/Role |
| --- | --- |
| `peasant` | Peasant |
| `artisan` | Artisan |
| `merchant` | Merchant/Burgher |
| `noble` | Noble/Court |
| `clergy` | Clergy/Monastic |
| `military` | Military |

The status-role axis should remain a baseline wardrobe axis. It must not be the only source of medieval clothing variety.

## Current Quality Gap

The first pass mostly uses this pattern:

```text
culture cue + status garment + generic wardrobe slot
```

This produces high counts but low identity. For example, Norse, Byzantine, Song, Rus, Gaelic, and Andalusi items should not simply be the same work tunic, lined hat, rough cloak, pouch, and footwear with a different cue appended.

The second pass must add explicit culture catalogues that use named material-culture forms such as:

- Norse hangerok/apron dress, oval brooches, bead-strung straps, sea cloaks, runic trade tags.
- Byzantine silk dalmatics, sagia, court belts, icon pouches, lamellar under-robes.
- Song cross-collar robes, scholar caps, tea wares, paper registers, official chop documents.
- Rus rubakhi, fur-edged kaftans, onuchi, birchbark letters, Orthodox icon shelves.
- Andalusi qamis, sirwal, burnous, turban, glazed wares, paper contracts.
- Gaelic brats, ring-pins, léine-style shirts, bardic mantles, pastoral milk vessels.

## Second-Pass Catalogue Targets

The next implementation pass should add exact culture-specific items on top of the existing generic baseline. Minimum targets:

| Surface | Minimum Explicit Culture Catalogue Target |
| --- | --- |
| Clothing and worn accessories | At least 12 explicit named clothing/accessory items per culture. |
| Military/equipment | At least 8 explicit military/equipment items per culture, excluding the current generic armour/weapon/shield/accessory set. |
| Food and beverage | At least 8 explicit food/beverage items per culture; tableware may count only as the vessel slot, not as prepared food. |
| Writing and administration | At least 6 explicit writing/admin items per culture, with culture-appropriate media. |
| Household/devotional/luxury goods | At least 5 explicit non-clothing, non-military household/devotional/luxury items per culture. |

The first improvement pass may prioritise the British/North Atlantic cultures, then apply the same standard to the eastern, Near Eastern, North African, steppe, and Song slices.

## Production Chain Invariants To Retain

`SeedMedievalProductionChainCrafts()` should remain the shared upstream foundation. It should continue to support:

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

## Runtime Component Expectations

Implemented live components should be used when appropriate:

| Component family | Medieval use |
| --- | --- |
| `PaperSheet` / `Book` | Paper sheets, parchment charters, rolls, codices, registers, notebooks. |
| `InscribableSurface` | Wax tablets, wooden tablets, birchbark letters, short-record boards, ostraca-like reusable surfaces. |
| `SealStamp` | Signet rings, guild stamps, office seals, official chops where appropriate. |
| `Sealable` | Charters, sealed envelopes, sealed bales, document satchels, strongboxes, ledger chests. |
| `MeasuringInstrument` | Balance scales, weights, grain/liquid measures, tax/customs kits. |
| `PreparedFood` if available and suitable | Real prepared food items rather than tableware props. |

Length/surveying measurement, musical instruments, rules-aware game sets, and animal tack/harness remain legitimate deferred component gaps.

## Required Second-Pass Tests

The second-pass test suite should not only check counts. It should verify quality.

1. **Explicit culture catalogue tests**
   - Every culture must have minimum explicit counts by surface.
   - Generic baseline items do not count toward explicit culture targets.

2. **Vocabulary tests**
   - Each culture must include required vocabulary in item descriptions and/or final craft names.

3. **Craft-name tests**
   - Explicit culture final crafts must not use `regional pattern NN`.
   - Generic baseline crafts may still use neutral names if clearly marked as generic.

4. **Food-input sanity tests**
   - Bread, pottage, stew, feast, ration, and beverage crafts must consume food commodities/liquids.
   - `Furniture Panel Stock` may only be used for tableware, trenchers, platters, and vessels.

5. **Writing component sanity tests**
   - Wooden, wax, birchbark, and tablet surfaces must not use `PaperSheet_Scroll` unless they are deliberately scroll-like paper/parchment objects.
   - Use `InscribableSurface` where the engine supports it.

6. **Exact documentation tests**
   - Explicit culture catalogue entries must appear by exact stable reference in `Medieval_Culture_Catalogue.md`.
   - Broad patterns such as `medieval_clothing_{culture}_{status}_{piece}` should document only generic baseline items, not explicit catalogue targets.

## Stable Reference Families

The current family patterns remain useful for generated baseline content:

| Surface | Stable Reference Family |
| --- | --- |
| Historic foundations | `historic_*` |
| Generic status clothing | `medieval_clothing_{culture}_{status}_{piece}` |
| Explicit culture clothing | `medieval_clothing_{culture}_{specific_item}` or `medieval_clothing_{culture}_{status}_{specific_item}` where status is genuinely part of the item identity. |
| Military | `medieval_military_{culture}_{specific_item}`, `medieval_weapon_{culture}_{specific_weapon}`, `medieval_shield_{culture}_{specific_shield}` |
| Food and beverage | `medieval_food_{culture}_{specific_food_or_vessel}` |
| Writing/admin | `medieval_writing_{culture}_{specific_document_or_tool}` |
| Household/devotional | `medieval_household_{culture}_{specific_item}`, `medieval_devotional_{culture}_{specific_item}`, `medieval_jewellery_{culture}_{specific_item}` |
| Trade and measures | `medieval_trade_{specific_item}` or `medieval_trade_{culture}_{specific_item}` |
| Repair kits | `medieval_textile_repair_kit`, `medieval_leather_repair_kit`, `medieval_metal_repair_kit` |
| Component-gap props | `medieval_music_*`, `medieval_game_*`, `medieval_horse_*` |

## Deferred Scope

The second pass should not attempt full plate armour, hand firearms, spectacles, mechanical clocks, printing presses, procedural surgery, cranequin/windlass/goat's-foot loading components, musical-instrument mechanics, rules-aware game sets, or animal tack systems.

It may add props for these areas if useful, but they should be labelled as deferred component-gap props and should not consume engineering time unless the runtime systems already exist.
