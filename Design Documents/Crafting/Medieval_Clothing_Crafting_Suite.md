# Medieval Clothing Crafting Suite

The medieval clothing suite has two layers:

1. **Common status-role baseline clothing**: a reusable generic wardrobe for peasant, artisan, merchant, noble, clergy, and military roles.
2. **Explicit culture catalogue clothing**: named material-culture garments and accessories that make each culture visually distinct.

The first merged implementation mostly created layer 1. The second pass must add layer 2.

## Design Principle

Shared textile production is correct. Shared final wardrobes are not enough.

The shared textile chain should remain culture-neutral: spinning, weaving, fulling, broadcloth, trim, tablet-woven bands, silk brocade, quilted padding, leather panels, turnshoe uppers, and yarn stock. Final garment assembly should produce named cultural objects.

## Baseline Status Wardrobe

The existing status-role wardrobe may remain as generic common stock:

| Status | Baseline Bodywear |
| --- | --- |
| `peasant` | work tunic |
| `artisan` | apron-front tunic |
| `merchant` | lined gown |
| `noble` | court surcoat |
| `clergy` | clerical robe |
| `military` | padded arming coat |

The baseline wardrobe also includes underlayers, headwear, hoods/cowls, outerwear, legwear, handwear, sockwear, footwear, belts, and pouches.

These items are useful as generic medieval starter stock, but they should not satisfy explicit culture-clothing quality targets.

## Explicit Culture Wardrobe Targets

Each culture should receive at least 12 explicit named clothing/accessory items. The catalogue for exact references lives in `Medieval_Culture_Catalogue.md`.

Each culture should include:

| Category | Minimum Count |
| --- | ---: |
| Common/peasant/artisan items | 4 |
| Merchant/urban/status items | 3 |
| Noble/formal/religious items | 3 |
| Military/climate/riding items | 2 |

## Culture Vocabulary Requirements

The suite should assert vocabulary in tests so cultures cannot pass as generic re-skins.

| Culture | Required Clothing Vocabulary Examples |
| --- | --- |
| `early_anglo_saxon` | tablet-banded, cloak brooch, linen head veil, seax belt |
| `anglo_danish` | long seax, shield-wall, panelled, reeve |
| `norse` | hangerok, oval brooch, sea cloak, runic, leg wraps |
| `norman` | split riding tunic, bliaut, mail surcoat, nasal |
| `high_british` | cote, surcoat, coif, wimple, arming |
| `gaelic` | brat, ring pin, léine or long shirt, bardic, pastoral |
| `andalusi` | qamis, sirwal, burnous, turban, tiraz |
| `byzantine` | silk dalmatic, sagion, court belt, icon pouch, skaramangion |
| `abbasid` | qamis, qaba, caftan, scholar robe, sash |
| `fatimid` | linen robe, tiraz-banded, cotton wrap, court kaftan |
| `seljuk_ayyubid` | riding caftan, quilted coat, high riding boots, bowcase belt |
| `rus_novgorod` | rubakha, fur-edged kaftan, onuchi, fur hat, birchbark |
| `steppe_turkic` | felt riding caftan, tied riding coat, high boots, bowcase-and-quiver |
| `song_china` | cross-collar robe, scholar robe, official cap, padded winter robe, cloth shoes |

## Craft Inputs

Final garment crafts should consume the stock that expresses their construction:

| Garment Type | Suggested Stock Inputs |
| --- | --- |
| ordinary linen/wool clothing | `Garment Cloth`, `Spun Yarn` |
| merchant or respectable town clothing | `Broadcloth Stock`, `Spun Yarn` |
| noble/court clothing | `Silk Brocade Panel`, `Embroidered Trim Stock`, `Spun Yarn` |
| Anglo-Saxon/Norse/banded garments | `Tablet-Woven Band Stock`, `Garment Cloth`, `Spun Yarn` |
| military underlayers | `Quilted Armour Padding`, `Garment Cloth`, `Spun Yarn` |
| shoes/boots/slippers | `Turnshoe Upper Stock`, `Leather Strap` |
| fur-edged or winter garments | hair/fur-tagged material input where available, plus garment cloth |
| belts/pouches/harnesses | `Prepared Leather Panel`, `Leather Strap`, metal `Tool Blank Stock` where fittings are present |

## Craft Naming

For explicit culture garments, use product-driven names:

```text
sew a Norse hangerok apron dress
tailor a Byzantine silk dalmatic
stitch an Andalusi linen qamis
sew a Rus fur-edged kaftan
tailor a Song cross-collar scholar robe
```

Do not use `regional pattern NN` for explicit culture final crafts.

Generic baseline crafts may retain neutral naming, but should be clearly labelled as generic in code comments, tests, and documentation.

## Stable Reference Rules

Use exact culture-specific stable references:

```text
medieval_clothing_norse_hangerok_apron_dress
medieval_clothing_byzantine_silk_dalmatic
medieval_clothing_song_china_cross_collar_robe
```

Avoid using only generic status tokens for explicit culture items:

```text
medieval_clothing_norse_peasant_work_tunic
medieval_clothing_song_china_merchant_lined_hat
```

Those may exist only as generic baseline items.

## Test Requirements

Add tests that verify:

- Each culture has at least 12 explicit culture clothing references.
- Generic baseline clothing is excluded from explicit culture counts.
- Each culture contains required vocabulary in item short descriptions, full descriptions, or final craft names.
- Explicit culture clothing crafts do not use `regional pattern`.
- Exact culture clothing stable references appear in `Medieval_Culture_Catalogue.md`.
