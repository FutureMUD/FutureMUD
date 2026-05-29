# Medieval Outfit Catalogue

This document defines the complete-outfit targets for the medieval clothing suite.

The goal is that a builder can dress a male or female character of each social class in each culture without inventing missing pieces. These are outfit definitions, not merely signature garment examples.

## Implementation Scaffold

`ItemSeeder.Rework.Medieval.cs` now mirrors this file with `MedievalOutfitSpec` definitions. Each code-side outfit records its culture key, sex/gender presentation, social class/role, display name, slot-to-stable-reference map, and intentionally shared/generic slots.

For MED-OUTFIT-001 the code scaffold resolved outfit slots to the current craftable common/status wardrobe plus craftable v1 role accessories. MED-OUTFIT-002 replaces that scaffold for `early_anglo_saxon`, `anglo_danish`, `norse`, `norman`, `high_british`, and `gaelic` with explicit outfit-piece item specs and final-product crafts generated from the exact target-piece rows below. MED-OUTFIT-003 extends the same explicit implementation to `carolingian`, `capetian`, `german_hre`, and `iberian_christian`. MED-OUTFIT-004 completes the explicit pass for `andalusi`, `byzantine`, `abbasid`, `fatimid`, `seljuk_ayyubid`, `rus_novgorod`, `steppe_turkic`, and `song_china`.

Explicit implemented outfit pieces use stable references shaped as:

```text
medieval_outfit_piece_{culture}_{sex}_{class}_{piece}
```

Those stable references are not generic wardrobe baselines. All 18 medieval culture keys now have explicit outfit-piece rows; the generated common/status wardrobe remains only a baseline fallback pattern for future non-catalogue use.

## Outfit Reference Pattern

```text
medieval_outfit_{culture}_{sex}_{class}
```

Sex keys:

- `male`
- `female`

Class keys:

- `peasant`
- `artisan`
- `merchant`
- `noble`
- `religious`
- `military`

## Required Slots

Every outfit should map to item stable references for:

- underlayer
- lower body
- leg or sock layer
- footwear
- bodywear
- outerwear
- headwear
- belt or sash
- worn container
- fastener or jewellery
- role item where applicable

The piece lists below are target contents. The implementation may split or merge exact item names where engine slots require it, but the resulting outfit must remain complete from head to foot.

## Complete Outfit Targets

### Early Anglo-Saxon / Insular (`early_anglo_saxon`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_early_anglo_saxon_male_peasant` | linen shirt; wool braies; wool leg wraps; soft ankle shoes; tablet-banded wool tunic; square work cloak; wool cap; rope belt; small belt pouch; plain disc brooch |
| `medieval_outfit_early_anglo_saxon_female_peasant` | linen shift; wool wrap skirt; wool footwraps; soft ankle shoes; tablet-banded wool gown; square cloak; linen head veil; woven girdle; small pouch; simple cloak brooch |
| `medieval_outfit_early_anglo_saxon_male_artisan` | linen work shirt; wool braies; wool leg wraps; leather shoes; short sleeved work tunic; workshop cloak; wool cap; seax belt; tool pouch; iron cloak pin |
| `medieval_outfit_early_anglo_saxon_female_artisan` | linen shift; wool work skirt; wool footwraps; leather shoes; belted work gown; apron; linen headcloth; woven belt; tool pouch; bronze ring pin |
| `medieval_outfit_early_anglo_saxon_male_merchant` | fine linen shirt; wool hose; leather shoes; bordered tunic; lined mantle; felt cap; leather purse belt; document pouch; silver cloak brooch; counting tally cord |
| `medieval_outfit_early_anglo_saxon_female_merchant` | fine linen shift; wool skirt; leather shoes; bordered gown; lined mantle; linen veil; decorated girdle; belt purse; silver brooch; bead necklace |
| `medieval_outfit_early_anglo_saxon_male_noble` | fine linen undertunic; wool hose; soft leather shoes; embroidered noble tunic; rich mantle; decorated cap; seax belt with mounts; document pouch; enamel brooch; bead necklace |
| `medieval_outfit_early_anglo_saxon_female_noble` | fine linen shift; embroidered undergown; soft shoes; embroidered noble gown; brooch-fastened mantle; long linen veil; decorated girdle; alms purse; enamel brooch; bead necklace |
| `medieval_outfit_early_anglo_saxon_male_religious` | plain linen undertunic; wool hose; sandals; monastic wool habit; heavy cowl cloak; tonsure cap or hood; cord belt; book pouch; wooden cross; wax tablet |
| `medieval_outfit_early_anglo_saxon_female_religious` | plain linen shift; wool undergown; sandals; monastic robe; heavy cowl cloak; linen veil; cord belt; book pouch; wooden cross; prayer tablet |
| `medieval_outfit_early_anglo_saxon_male_military` | arming shirt; wool braies; leg wraps; leather boots; padded shield-wall tunic; war cloak; padded cap; seax belt; field pouch; shield brooch; archer bracer |
| `medieval_outfit_early_anglo_saxon_female_military` | arming shift; wool trews or skirted leg wraps; leather boots; padded war gown; brooch-fastened war cloak; linen headwrap; arming belt; field pouch; cloak brooch; leather bracers |

### Late Anglo-Saxon / Anglo-Danish (`anglo_danish`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_anglo_danish_male_peasant` | linen shirt; wool braies; wrapped trews; rough shoes; panelled wool tunic; rough cloak; wool hood; rope belt; belt pouch; plain ring pin |
| `medieval_outfit_anglo_danish_female_peasant` | linen shift; wool skirt; footwraps; rough shoes; panelled wool gown; rough cloak; head rail; woven belt; belt pouch; bronze ring pin |
| `medieval_outfit_anglo_danish_male_artisan` | work shirt; wool trews; leather shoes; narrow-braid tunic; work cloak; fitted cap; long seax belt; tool pouch; iron cloak pin; workshop mitts |
| `medieval_outfit_anglo_danish_female_artisan` | work shift; wool skirt; leather shoes; narrow-braid work gown; apron; head rail; leather belt; tool pouch; bronze brooch; sleeve ties |
| `medieval_outfit_anglo_danish_male_merchant` | fine shirt; wool hose; town shoes; embroidered collar tunic; lined mantle; felt cap; purse belt; reeve tally pouch; silver ring pin; seal cord |
| `medieval_outfit_anglo_danish_female_merchant` | fine shift; wool gown; town shoes; embroidered collar overgown; lined mantle; linen head rail; decorated girdle; purse; silver brooch; bead string |
| `medieval_outfit_anglo_danish_male_noble` | fine undertunic; wool hose; soft boots; panelled noble tunic; fur-edged cloak; decorated cap; mounted seax belt; document pouch; silver brooch; gold bead string |
| `medieval_outfit_anglo_danish_female_noble` | fine shift; embroidered gown; soft shoes; panelled overgown; fur-edged cloak; long veil; decorated girdle; alms purse; silver brooch; bead necklace |
| `medieval_outfit_anglo_danish_male_religious` | plain undertunic; wool hose; sandals; clerical robe; cowl; linen cap; cord belt; book pouch; wooden cross; wax tablet |
| `medieval_outfit_anglo_danish_female_religious` | plain shift; wool gown; sandals; modest robe; cowl cloak; linen veil; cord belt; book pouch; wooden cross; prayer strip |
| `medieval_outfit_anglo_danish_male_military` | arming shirt; wrapped trews; boots; padded shield-wall tunic; heavy cloak; nasal cap liner; long seax belt; field pouch; war brooch; leather bracers |
| `medieval_outfit_anglo_danish_female_military` | arming shift; wrapped trews; boots; padded shield-wall gown; heavy cloak; head rail under cap; arming belt; field pouch; war brooch; leather bracers |

### Norse (`norse`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_norse_male_peasant` | linen shirt; wool trousers; leg wraps; rough shoes; plain wool tunic; sea cloak; wool cap; leather belt; belt pouch; simple cloak pin |
| `medieval_outfit_norse_female_peasant` | linen underdress; wool lower skirt; leg wraps; rough shoes; hangerok apron dress; sea cloak; wool headcloth; woven belt; small pouch; oval brooch pair |
| `medieval_outfit_norse_male_artisan` | work shirt; wool trousers; leg wraps; leather shoes; trader tunic; short work cloak; wool cap; tool belt; tool pouch; ring pin |
| `medieval_outfit_norse_female_artisan` | linen underdress; wool skirt; leg wraps; leather shoes; work hangerok; apron; headcloth; woven belt; tool pouch; oval brooch pair |
| `medieval_outfit_norse_male_merchant` | fine shirt; wool trousers; leg wraps; polished shoes; trader kaftan; fur-edged sea cloak; cap; decorated belt; trade pouch; runic trade tag |
| `medieval_outfit_norse_female_merchant` | fine underdress; wool dress; leg wraps; polished shoes; bead-strung hangerok; fur-edged cloak; headcloth; decorated belt; purse; oval brooch pair and beads |
| `medieval_outfit_norse_male_noble` | fine linen shirt; wool trousers; high boots; decorated kaftan; fur-lined cloak; embroidered cap; silver-mounted belt; document pouch; silver brooch; bead necklace |
| `medieval_outfit_norse_female_noble` | fine underdress; embroidered gown; high shoes; rich hangerok; fur-lined cloak; silk head veil; decorated girdle; alms purse; oval brooch pair; bead strand |
| `medieval_outfit_norse_male_religious` | plain shirt; wool trousers; sandals; simple robe; heavy cloak; hood; cord belt; book pouch; wooden cross or amulet; wax tablet |
| `medieval_outfit_norse_female_religious` | plain shift; wool gown; sandals; modest robe; heavy cloak; linen veil; cord belt; book pouch; wooden cross or amulet; prayer tablet |
| `medieval_outfit_norse_male_military` | arming shirt; wool trousers; leg wraps; high boots; arming tunic; heavy sea cloak; padded cap; weapon belt; field pouch; axe loop; leather bracers |
| `medieval_outfit_norse_female_military` | arming shift; wool trousers or split skirt; leg wraps; high boots; arming hangerok or tunic; heavy sea cloak; headwrap under cap; weapon belt; field pouch; leather bracers |

### Norman / Angevin (`norman`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_norman_male_peasant` | linen shirt; braies; wool hose; rough shoes; plain long tunic; hooded cloak; linen coif; rope belt; belt pouch; simple cloak clasp |
| `medieval_outfit_norman_female_peasant` | linen shift; wool skirt; wool hose; rough shoes; plain gown; hooded cloak; linen veil; woven belt; belt pouch; simple cloak clasp |
| `medieval_outfit_norman_male_artisan` | work shirt; braies; hose; leather shoes; fitted work tunic; short cloak; coif; tool belt; tool pouch; iron buckle |
| `medieval_outfit_norman_female_artisan` | work shift; wool skirt; hose; leather shoes; work gown; apron; head veil; leather belt; tool pouch; bronze brooch |
| `medieval_outfit_norman_male_merchant` | fine shirt; braies; fitted hose; town shoes; long-sleeved cote; lined mantle; coif and cap; purse belt; document pouch; cloak clasp |
| `medieval_outfit_norman_female_merchant` | fine chemise; fitted gown; hose; town shoes; bliaut-style overgown; lined mantle; wimple and veil; girdle; purse; cloak brooch |
| `medieval_outfit_norman_male_noble` | fine undertunic; fitted hose; soft boots; split riding tunic; court mantle; decorated cap; mounted belt; seal pouch; rich cloak clasp; gloves |
| `medieval_outfit_norman_female_noble` | fine chemise; court bliaut; fine hose; soft shoes; noble mantle; wimple and veil; jeweled girdle; alms purse; cloak clasp; embroidered sleeve ties |
| `medieval_outfit_norman_male_religious` | linen undertunic; wool hose; sandals; clerical robe; cowl cloak; coif; cord belt; book pouch; cross pendant; wax tablet |
| `medieval_outfit_norman_female_religious` | linen shift; wool undergown; sandals; religious robe; cowl cloak; veil and wimple; cord belt; book pouch; cross pendant; prayer slip |
| `medieval_outfit_norman_male_military` | arming shirt; braies; chausses; riding boots; padded aketon; mail surcoat; nasal arming coif; arming belt; field pouch; scabbard harness |
| `medieval_outfit_norman_female_military` | arming shift; split riding skirt or chausses; riding boots; padded aketon gown; mail surcoat; nasal arming coif and veil; arming belt; field pouch; scabbard harness |

### High Medieval Britain / Marcher (`high_british`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_high_british_male_peasant` | linen shirt; braies; wool hose; rough shoes; wool cote; rough cloak; linen coif; rope belt; belt pouch; simple clasp |
| `medieval_outfit_high_british_female_peasant` | linen shift; wool kirtle; hose; rough shoes; plain gown; rough cloak; linen headcloth; woven belt; belt pouch; simple clasp |
| `medieval_outfit_high_british_male_artisan` | work shirt; braies; hose; leather shoes; work cote; hood; coif; tool belt; tool pouch; work gloves |
| `medieval_outfit_high_british_female_artisan` | work shift; kirtle; hose; leather shoes; work gown; apron; headcloth; leather belt; tool pouch; pin brooch |
| `medieval_outfit_high_british_male_merchant` | fine shirt; braies; fitted hose; polished shoes; lined cote; travel mantle; hood; purse belt; document pouch; guild badge |
| `medieval_outfit_high_british_female_merchant` | fine chemise; kirtle; fitted hose; polished shoes; merchant gown; lined mantle; wimple; decorated girdle; purse; guild badge |
| `medieval_outfit_high_british_male_noble` | fine undertunic; fitted hose; soft shoes; silk-trimmed surcoat; fur-lined mantle; court cap; jeweled belt; alms purse; cloak brooch; gloves |
| `medieval_outfit_high_british_female_noble` | fine chemise; court gown; fine hose; soft shoes; silk-trimmed overgown; fur mantle; wimple and veil; jeweled girdle; alms purse; brooch |
| `medieval_outfit_high_british_male_religious` | linen undertunic; wool hose; sandals; clerical robe; cowl cloak; coif; cord belt; book pouch; cross pendant; small prayer book |
| `medieval_outfit_high_british_female_religious` | linen shift; wool robe; sandals; nun's habit or religious gown; cowl cloak; veil and wimple; cord belt; book pouch; cross pendant; prayer book |
| `medieval_outfit_high_british_male_military` | arming shirt; braies; padded chausses; riding boots; gambeson; surcoat; arming coif; arming belt; field pouch; archer bracer |
| `medieval_outfit_high_british_female_military` | arming shift; chausses or split riding skirt; boots; fitted gambeson; surcoat; arming coif with headcloth; arming belt; field pouch; archer bracer |

### Gaelic / Welsh / Highland (`gaelic`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_gaelic_male_peasant` | linen long shirt; wool trews; footwraps; deerskin shoes; plain leine-style tunic; brat mantle; wool cap; woven belt; pastoral pouch; ring pin |
| `medieval_outfit_gaelic_female_peasant` | linen shift; wool wrap skirt; footwraps; deerskin shoes; long wool gown; brat mantle; linen headcloth; woven belt; pastoral pouch; ring pin |
| `medieval_outfit_gaelic_male_artisan` | work shirt; wool trews; leather shoes; work long shirt; short hill cloak; wool cap; tool belt; tool pouch; ring pin; mitts |
| `medieval_outfit_gaelic_female_artisan` | work shift; wool skirt; leather shoes; work gown; apron; headcloth; woven belt; tool pouch; bronze ring pin; sleeve ties |
| `medieval_outfit_gaelic_male_merchant` | fine long shirt; wool trews; boots; bordered tunic; lined brat; cap; purse belt; document pouch; silver ring pin; tally cord |
| `medieval_outfit_gaelic_female_merchant` | fine shift; wool gown; boots; bordered overgown; lined brat; head veil; girdle; purse; silver ring pin; bead cord |
| `medieval_outfit_gaelic_male_noble` | fine linen shirt; wool trews; boots; embroidered tunic; bardic or lordly mantle; decorated cap; fine belt; document pouch; ornate ring pin; brooch |
| `medieval_outfit_gaelic_female_noble` | fine shift; embroidered gown; soft shoes; noble overgown; bardic mantle; long veil; fine girdle; alms purse; ornate ring pin; brooch |
| `medieval_outfit_gaelic_male_religious` | plain linen shirt; wool trews; sandals; monastic robe; cowl cloak; hood; cord belt; book pouch; wooden cross; note board |
| `medieval_outfit_gaelic_female_religious` | plain shift; wool robe; sandals; religious gown; cowl cloak; veil; cord belt; book pouch; wooden cross; prayer slip |
| `medieval_outfit_gaelic_male_military` | arming shirt; wool trews; boots; light padded coat; war brat; padded cap; spear carrier belt; field pouch; ring pin; bracers |
| `medieval_outfit_gaelic_female_military` | arming shift; split skirt or trews; boots; light padded war gown; war brat; headcloth under cap; spear carrier belt; field pouch; ring pin; bracers |

### Carolingian / Frankish (`carolingian`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_carolingian_male_peasant` | linen shirt; braies; leg wraps; rough shoes; high-belted tunic; broad-banded mantle; wool cap; rope belt; belt pouch; simple pin |
| `medieval_outfit_carolingian_female_peasant` | linen shift; wool gown; footwraps; rough shoes; high-belted work dress; broad-banded mantle; linen head veil; woven belt; pouch; simple brooch |
| `medieval_outfit_carolingian_male_artisan` | work shirt; braies; leg wraps; leather shoes; broad-banded work tunic; short cloak; cap; tool belt; tool pouch; iron pin |
| `medieval_outfit_carolingian_female_artisan` | work shift; wool dress; footwraps; leather shoes; broad-banded work gown; apron; head veil; leather belt; tool pouch; bronze pin |
| `medieval_outfit_carolingian_male_merchant` | fine shirt; hose; leather shoes; broad-banded tunic; lined mantle; felt cap; purse belt; capitulary estate-list pouch; silver fibula; tally cord |
| `medieval_outfit_carolingian_female_merchant` | fine shift; wool gown; shoes; bordered overgown; lined mantle; veil; decorated girdle; purse; silver brooch; bead strand |
| `medieval_outfit_carolingian_male_noble` | fine undertunic; hose; boots; high-belted noble tunic; court cloak; decorated cap; spatha belt; document pouch; noble fibula; gloves |
| `medieval_outfit_carolingian_female_noble` | fine shift; embroidered gown; soft shoes; court overgown; rich mantle; long veil; decorated girdle; alms purse; noble fibula; bead strand |
| `medieval_outfit_carolingian_male_religious` | linen undertunic; wool hose; sandals; clerical dalmatic-style robe; monastic cowl; hood; cord belt; book pouch; cross pendant; manuscript leaf |
| `medieval_outfit_carolingian_female_religious` | linen shift; wool robe; sandals; monastic robe; cowl cloak; veil; cord belt; book pouch; cross pendant; prayer tablet |
| `medieval_outfit_carolingian_male_military` | arming shirt; braies; leg wraps; boots; padded war tunic; broad-banded mantle; padded cap; spatha belt; field pouch; riding spurs |
| `medieval_outfit_carolingian_female_military` | arming shift; split skirt or trews; boots; padded war gown; broad-banded mantle; head veil under cap; arming belt; field pouch; cloak pin; bracers |

### Capetian / Low Countries (`capetian`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_capetian_male_peasant` | linen shirt; braies; wool hose; rough shoes; plain wool cote; rough cloak; linen coif; rope belt; pouch; clasp |
| `medieval_outfit_capetian_female_peasant` | linen shift; wool kirtle; hose; rough shoes; plain gown; rough cloak; head veil; woven belt; pouch; simple brooch |
| `medieval_outfit_capetian_male_artisan` | work shirt; braies; hose; leather shoes; guild work cote; guild apron; coif; tool belt; tool pouch; guild token |
| `medieval_outfit_capetian_female_artisan` | work shift; kirtle; hose; leather shoes; work gown; guild apron; headcloth; leather belt; tool pouch; guild token |
| `medieval_outfit_capetian_male_merchant` | fine shirt; braies; fitted hose; polished shoes; lined burgher gown; travel mantle; hood; purse belt; contract pouch; guild badge |
| `medieval_outfit_capetian_female_merchant` | fine chemise; fitted gown; fine hose; polished shoes; lined burgher overgown; mantle; wimple; decorated girdle; purse; guild badge |
| `medieval_outfit_capetian_male_noble` | fine undertunic; fitted hose; soft shoes; silk-trimmed cote; court mantle; court cap; jeweled belt; alms purse; cloak brooch; gloves |
| `medieval_outfit_capetian_female_noble` | fine chemise; bliaut-style gown; soft shoes; silk-trimmed overgown; rich mantle; wimple and veil; jeweled girdle; alms purse; brooch; gloves |
| `medieval_outfit_capetian_male_religious` | linen undertunic; hose; sandals; clerical robe; cowl cloak; coif; cord belt; book pouch; cross pendant; chapel book |
| `medieval_outfit_capetian_female_religious` | linen shift; wool robe; sandals; religious habit; cowl cloak; wimple and veil; cord belt; book pouch; cross pendant; prayer book |
| `medieval_outfit_capetian_male_military` | arming shirt; braies; chausses; boots; padded aketon; surcoat; arming coif; arming belt; field pouch; scabbard harness |
| `medieval_outfit_capetian_female_military` | arming shift; chausses or split skirt; boots; padded aketon gown; surcoat; arming coif and headcloth; arming belt; field pouch; scabbard harness |

### German / HRE / Alpine-North Italian (`german_hre`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_german_hre_male_peasant` | linen shirt; braies; wool hose; rough shoes; fitted wool tunic; winter cloak; alpine felt cap; rope belt; pouch; clasp |
| `medieval_outfit_german_hre_female_peasant` | linen shift; wool gown; hose; rough shoes; fitted work gown; winter cloak; headcloth; woven belt; pouch; simple pin |
| `medieval_outfit_german_hre_male_artisan` | work shirt; braies; hose; leather shoes; guild apron over tunic; short cloak; alpine felt cap; tool belt; tool pouch; guild mark |
| `medieval_outfit_german_hre_female_artisan` | work shift; wool gown; hose; leather shoes; guild apron; short cloak; headcloth; tool belt; tool pouch; guild mark |
| `medieval_outfit_german_hre_male_merchant` | fine shirt; fitted hose; polished shoes; civic gown; fur-lined mantle; town hat; purse belt; account pouch; guild badge; gloves |
| `medieval_outfit_german_hre_female_merchant` | fine shift; fitted gown; polished shoes; civic overgown; fur-lined mantle; fine hood; girdle; purse; guild badge; gloves |
| `medieval_outfit_german_hre_male_noble` | fine undertunic; silk hose; soft shoes; court gown; fur-lined mantle; court hat; jeweled belt; seal pouch; belt mounts; gloves |
| `medieval_outfit_german_hre_female_noble` | fine chemise; court gown; soft shoes; embroidered overgown; fur-lined mantle; fine hood or veil; jeweled girdle; alms purse; brooch; gloves |
| `medieval_outfit_german_hre_male_religious` | linen undertunic; hose; sandals; church robe; cowl cloak; coif; cord belt; book pouch; cross pendant; manuscript leaf |
| `medieval_outfit_german_hre_female_religious` | linen shift; wool robe; sandals; religious habit; cowl cloak; veil; cord belt; book pouch; cross pendant; prayer book |
| `medieval_outfit_german_hre_male_military` | arming shirt; braies; chausses; boots; arming jack; short mantle; padded cap; arming belt; field pouch; town crossbow militia hook |
| `medieval_outfit_german_hre_female_military` | arming shift; chausses or split skirt; boots; arming jack gown; short mantle; headcloth under cap; arming belt; field pouch; bracers; town crossbow militia hook |

### Iberian Christian (`iberian_christian`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_iberian_christian_male_peasant` | linen shirt; braies; wool hose; sandals; simple saya; short manto; cloth cap; rope belt; pouch; clasp |
| `medieval_outfit_iberian_christian_female_peasant` | linen shift; wool skirt; hose; sandals; simple saya gown; manto; toca head veil; woven belt; pouch; pin |
| `medieval_outfit_iberian_christian_male_artisan` | work shirt; braies; hose; leather shoes; narrow-sleeved tunic; short cloak; cap; tool belt; pouch; buckle |
| `medieval_outfit_iberian_christian_female_artisan` | work shift; wool gown; hose; leather shoes; narrow-sleeved work gown; apron; toca; leather belt; tool pouch; pin |
| `medieval_outfit_iberian_christian_male_merchant` | fine shirt; fitted hose; shoes; pellote over tunic; lined manto; cap; purse belt; contract pouch; belt mount; gloves |
| `medieval_outfit_iberian_christian_female_merchant` | fine shift; fitted gown; shoes; pellote overgown; lined manto; toca; decorated girdle; purse; brooch; gloves |
| `medieval_outfit_iberian_christian_male_noble` | fine undertunic; silk hose; boots; silk-trimmed saya; court manto; court cap; noble belt; seal pouch; cloak clasp; gloves |
| `medieval_outfit_iberian_christian_female_noble` | fine chemise; court gown; soft shoes; silk pellote; court manto; fine toca and veil; jeweled girdle; alms purse; brooch; gloves |
| `medieval_outfit_iberian_christian_male_religious` | linen undertunic; hose; sandals; clerical robe; pilgrim cloak; coif; cord belt; book pouch; cross pendant; chapel booklet |
| `medieval_outfit_iberian_christian_female_religious` | linen shift; wool robe; sandals; religious habit; pilgrim cloak; veil and toca; cord belt; book pouch; cross pendant; prayer slip |
| `medieval_outfit_iberian_christian_male_military` | arming shirt; braies; chausses; riding boots; quilted coat; knightly surcoat; arming cap; weapon belt; field pouch; cloak clasp; frontier riding cloak |
| `medieval_outfit_iberian_christian_female_military` | arming shift; split riding skirt; chausses; riding boots; quilted coat gown; knightly surcoat; head veil under cap; weapon belt; field pouch; cloak clasp; frontier riding cloak |

### al-Andalus / Maghreb (`andalusi`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_andalusi_male_peasant` | linen qamis; wool sirwal; footwraps; sandals; plain outer tunic; light burnous; simple turban; woven sash; belt pouch; cord amulet |
| `medieval_outfit_andalusi_female_peasant` | linen shift; loose sirwal or skirt; footwraps; sandals; plain long robe; light wrap cloak; veiled headcloth; woven sash; pouch; cord amulet |
| `medieval_outfit_andalusi_male_artisan` | work qamis; sirwal; leather sandals; workshop robe; short burnous; wrapped turban; leather sash; tool pouch; small amulet; sleeve ties |
| `medieval_outfit_andalusi_female_artisan` | work shift; loose trousers or skirt; sandals; work robe; apron wrap; veiled headcloth; sash; tool pouch; amulet; sleeve ties |
| `medieval_outfit_andalusi_male_merchant` | fine qamis; sirwal; soft slippers; qaba caftan; lined burnous; turban; merchant sash; contract pouch; signet ring; perfume flask |
| `medieval_outfit_andalusi_female_merchant` | fine shift; sirwal or under-robe; soft slippers; tiraz-banded robe; lined cloak; veil; decorated sash; purse; amulet pendant; perfume flask |
| `medieval_outfit_andalusi_male_noble` | silk qamis; fine sirwal; soft slippers; tiraz-banded court robe; rich burnous; fine turban; silk sash; seal pouch; signet ring; embroidered gloves |
| `medieval_outfit_andalusi_female_noble` | fine shift; silk under-robe; soft slippers; embroidered court robe; rich mantle; fine veil; silk sash; alms purse; pendant; perfume flask |
| `medieval_outfit_andalusi_male_religious` | plain qamis; sirwal; sandals; scholar robe; plain cloak; wrapped turban; cord sash; book pouch; prayer beads; writing tablet |
| `medieval_outfit_andalusi_female_religious` | plain shift; under-robe; sandals; modest robe; plain wrap; veil; cord sash; book pouch; prayer beads; devotional slip |
| `medieval_outfit_andalusi_male_military` | arming qamis; sirwal; riding boots; quilted coat; riding burnous; turban-helm liner; bowcase belt; field pouch; amulet; leather bracers |
| `medieval_outfit_andalusi_female_military` | arming shift; sirwal or split skirt; riding boots; quilted riding coat; riding burnous; veiled headwrap under cap; bowcase belt; field pouch; amulet; bracers |

### Byzantine (`byzantine`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_byzantine_male_peasant` | linen under-robe; simple trousers; footwraps; sandals; wool tunic; short sagion cloak; cloth cap; plain belt; pouch; wooden cross |
| `medieval_outfit_byzantine_female_peasant` | linen shift; lower gown; footwraps; sandals; plain long tunic; wool wrap; head veil; woven belt; pouch; wooden cross |
| `medieval_outfit_byzantine_male_artisan` | work under-robe; trousers; shoes; workshop tunic; short cloak; cap; tool belt; tool pouch; small icon token; gloves |
| `medieval_outfit_byzantine_female_artisan` | work shift; undergown; shoes; work gown; apron wrap; head veil; tool belt; pouch; icon token; sleeve ties |
| `medieval_outfit_byzantine_male_merchant` | fine under-robe; trousers; soft shoes; belted skaramangion robe; lined sagion; cap; purse belt; account pouch; seal ring; gloves |
| `medieval_outfit_byzantine_female_merchant` | fine shift; undergown; soft shoes; formal gown; lined mantle; veil; decorated girdle; purse; icon pendant; gloves |
| `medieval_outfit_byzantine_male_noble` | silk under-robe; fine trousers; soft boots; silk dalmatic; court sagion; court cap; court belt; seal pouch; enamel pendant; gloves |
| `medieval_outfit_byzantine_female_noble` | silk shift; fine gown; soft shoes; embroidered silk robe; court mantle; fine veil; court girdle; alms purse; enamel pendant; icon pouch |
| `medieval_outfit_byzantine_male_religious` | plain under-robe; trousers; sandals; monastic robe; cowl cloak; hood; cord belt; book pouch; cross pendant; icon tablet |
| `medieval_outfit_byzantine_female_religious` | plain shift; wool robe; sandals; monastic robe; cowl cloak; veil; cord belt; book pouch; cross pendant; prayer tablet |
| `medieval_outfit_byzantine_male_military` | arming under-robe; trousers; boots; military padded tunic; lamellar coat cover; padded cap; military belt; field pouch; icon token; scabbard harness |
| `medieval_outfit_byzantine_female_military` | arming shift; trousers or split skirt; boots; military padded robe; lamellar coat cover; head veil under cap; military belt; field pouch; icon token; scabbard harness |

### Abbasid / Persianate (`abbasid`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_abbasid_male_peasant` | linen qamis; wool sirwal; footwraps; sandals; plain robe; light cloak; simple turban; woven sash; pouch; amulet cord |
| `medieval_outfit_abbasid_female_peasant` | linen shift; loose sirwal or skirt; footwraps; sandals; plain robe; wrap cloak; head veil; woven sash; pouch; amulet cord |
| `medieval_outfit_abbasid_male_artisan` | work qamis; sirwal; sandals; work caftan; short cloak; turban; tool sash; tool pouch; amulet; sleeve ties |
| `medieval_outfit_abbasid_female_artisan` | work shift; loose trousers; sandals; work robe; apron wrap; veil; sash; tool pouch; amulet; sleeve ties |
| `medieval_outfit_abbasid_male_merchant` | fine qamis; sirwal; soft slippers; qaba caftan; lined cloak; turban; merchant sash; contract pouch; signet ring; ink case |
| `medieval_outfit_abbasid_female_merchant` | fine shift; under-robe; soft slippers; fine robe; lined cloak; veil; decorated sash; purse; pendant; scent flask |
| `medieval_outfit_abbasid_male_noble` | silk qamis; fine sirwal; soft slippers; belted court robe; rich cloak; fine turban; silk sash; seal pouch; signet ring; gloves |
| `medieval_outfit_abbasid_female_noble` | silk shift; fine under-robe; soft slippers; court robe; rich mantle; fine veil; silk sash; alms purse; pendant; gloves |
| `medieval_outfit_abbasid_male_religious` | plain qamis; sirwal; sandals; scholar robe; plain cloak; turban; cord sash; book pouch; prayer beads; notebook |
| `medieval_outfit_abbasid_female_religious` | plain shift; modest robe; sandals; scholar or devotional robe; plain wrap; veil; cord sash; book pouch; prayer beads; prayer slip |
| `medieval_outfit_abbasid_male_military` | arming qamis; sirwal; riding boots; lamellar-sleeved coat; riding cloak; turban-helm liner; weapon sash; field pouch; amulet; bowcase belt |
| `medieval_outfit_abbasid_female_military` | arming shift; sirwal or split skirt; riding boots; lamellar-sleeved riding coat; riding cloak; veiled headwrap; weapon sash; field pouch; amulet; bowcase belt |

### Fatimid Egypt / Ifriqiya (`fatimid`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_fatimid_male_peasant` | linen qamis; cotton lower wrap; sandals; light linen robe; shoulder cloth; simple turban; woven sash; pouch; amulet cord; headcloth |
| `medieval_outfit_fatimid_female_peasant` | linen shift; cotton wrap skirt; sandals; light robe; shoulder wrap; veiled headcloth; woven sash; pouch; amulet cord; bead string |
| `medieval_outfit_fatimid_male_artisan` | work qamis; cotton wrap; sandals; linen work robe; apron cloth; turban; tool sash; pouch; amulet; sleeve bands |
| `medieval_outfit_fatimid_female_artisan` | work shift; cotton wrap skirt; sandals; linen work gown; apron; veil; sash; tool pouch; amulet; sleeve bands |
| `medieval_outfit_fatimid_male_merchant` | fine linen qamis; light trousers; soft sandals; tiraz-banded tunic; lined robe; turban; merchant sash; tax pouch; signet ring; perfume flask |
| `medieval_outfit_fatimid_female_merchant` | fine shift; cotton under-robe; soft sandals; tiraz-banded robe; light mantle; veil; decorated sash; purse; pendant; perfume flask |
| `medieval_outfit_fatimid_male_noble` | silk qamis; fine trousers; soft slippers; court kaftan; formal cuffs; fine turban; silk sash; seal pouch; signet ring; gloves |
| `medieval_outfit_fatimid_female_noble` | silk shift; fine robe; soft slippers; court robe; light mantle; fine veil; silk sash; alms purse; pendant; scent flask |
| `medieval_outfit_fatimid_male_religious` | plain qamis; light trousers; sandals; scholar robe; plain cloak; turban; cord sash; book pouch; prayer beads; endowment slip |
| `medieval_outfit_fatimid_female_religious` | plain shift; modest robe; sandals; devotional robe; plain wrap; veil; cord sash; book pouch; prayer beads; prayer slip |
| `medieval_outfit_fatimid_male_military` | arming qamis; trousers; riding boots; padded coat with scale panels; guard cloak; turban-helm liner; weapon belt; field pouch; amulet; archer quiver |
| `medieval_outfit_fatimid_female_military` | arming shift; trousers or split skirt; riding boots; padded guard coat; guard cloak; veiled headwrap; weapon belt; field pouch; amulet; archer quiver |

### Seljuk / Ayyubid / early Mamluk (`seljuk_ayyubid`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_seljuk_ayyubid_male_peasant` | linen qamis; wool sirwal; footwraps; boots; plain caftan; felt cloak; simple turban; sash; pouch; amulet |
| `medieval_outfit_seljuk_ayyubid_female_peasant` | linen shift; loose sirwal or skirt; footwraps; boots; plain long robe; felt cloak; veiled headwrap; sash; pouch; amulet |
| `medieval_outfit_seljuk_ayyubid_male_artisan` | work qamis; sirwal; boots; quilted coat; short cloak; turban; tool sash; tool pouch; amulet; gloves |
| `medieval_outfit_seljuk_ayyubid_female_artisan` | work shift; sirwal or skirt; boots; quilted work robe; apron wrap; veil; sash; tool pouch; amulet; gloves |
| `medieval_outfit_seljuk_ayyubid_male_merchant` | fine qamis; sirwal; soft boots; riding caftan; lined cloak; turban; merchant sash; contract pouch; signet ring; gloves |
| `medieval_outfit_seljuk_ayyubid_female_merchant` | fine shift; under-robe; soft boots; fine caftan robe; lined mantle; veil; decorated sash; purse; pendant; gloves |
| `medieval_outfit_seljuk_ayyubid_male_noble` | silk qamis; fine sirwal; high boots; court caftan; fur-edged cloak; fine turban; silk sash; seal pouch; belt plaques; gloves |
| `medieval_outfit_seljuk_ayyubid_female_noble` | silk shift; fine under-robe; soft boots; embroidered court robe; fur-edged mantle; fine veil; silk sash; alms purse; pendant; gloves |
| `medieval_outfit_seljuk_ayyubid_male_religious` | plain qamis; sirwal; sandals; scholar robe; plain cloak; turban; cord sash; book pouch; prayer beads; madrasa notebook |
| `medieval_outfit_seljuk_ayyubid_female_religious` | plain shift; modest robe; sandals; devotional robe; plain wrap; veil; cord sash; book pouch; prayer beads; prayer slip |
| `medieval_outfit_seljuk_ayyubid_male_military` | arming qamis; sirwal; high riding boots; quilted riding coat; lamellar coat cover; turban-helm liner; bowcase belt; field pouch; amulet; leather gloves |
| `medieval_outfit_seljuk_ayyubid_female_military` | arming shift; sirwal or split skirt; high riding boots; quilted riding robe; lamellar coat cover; veiled headwrap under cap; bowcase belt; field pouch; amulet; gloves |

### Kyivan Rus / Novgorod (`rus_novgorod`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_rus_novgorod_male_peasant` | linen rubakha; porty trousers; onuchi footwraps; bast or leather shoes; wool tunic; rough cloak; fur cap; woven belt; belt pouch; simple cross cord |
| `medieval_outfit_rus_novgorod_female_peasant` | linen shift; wool skirt; onuchi footwraps; leather shoes; rubakha-style gown; rough cloak; headscarf; woven belt; pouch; simple cross cord |
| `medieval_outfit_rus_novgorod_male_artisan` | work rubakha; porty trousers; onuchi; boots; work kaftan; short cloak; fur cap; tool belt; tool pouch; bronze cross; mittens |
| `medieval_outfit_rus_novgorod_female_artisan` | work shift; wool skirt; onuchi; boots; work gown; apron; headscarf; tool belt; tool pouch; bronze cross; mittens |
| `medieval_outfit_rus_novgorod_male_merchant` | fine rubakha; porty trousers; boots; fur-edged kaftan; lined cloak; fur hat; purse belt; birchbark document pouch; silver cross; gloves |
| `medieval_outfit_rus_novgorod_female_merchant` | fine shift; wool gown; boots; fur-edged overgown; lined cloak; head veil; decorated girdle; purse; silver cross; document pouch |
| `medieval_outfit_rus_novgorod_male_noble` | fine shirt; trousers; soft boots; embroidered kaftan; fur-lined mantle; fur hat; rich belt; seal pouch; Orthodox pendant; gloves |
| `medieval_outfit_rus_novgorod_female_noble` | fine shift; embroidered gown; soft boots; fur-edged court robe; fur-lined mantle; fine veil; rich girdle; alms purse; Orthodox pendant; gloves |
| `medieval_outfit_rus_novgorod_male_religious` | plain rubakha; trousers; sandals; monastic robe; heavy cloak; hood; cord belt; book pouch; wooden cross; prayer slip |
| `medieval_outfit_rus_novgorod_female_religious` | plain shift; wool robe; sandals; monastic robe; heavy cloak; veil; cord belt; book pouch; wooden cross; prayer slip |
| `medieval_outfit_rus_novgorod_male_military` | arming rubakha; trousers; boots; padded war coat; fur-edged cloak; padded cap or helm liner; warrior belt; field pouch; bronze cross; axe loop |
| `medieval_outfit_rus_novgorod_female_military` | arming shift; trousers or split skirt; boots; padded war coat; fur-edged cloak; headscarf under cap; warrior belt; field pouch; bronze cross; axe loop |

### Steppe Turkic / Cuman / Mongol-adjacent (`steppe_turkic`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_steppe_turkic_male_peasant` | linen under-shirt; riding trousers; felt footwraps; high boots; tied riding coat; felt cloak; fur cap; sash; travel pouch; amulet |
| `medieval_outfit_steppe_turkic_female_peasant` | linen shift; riding trousers or split skirt; felt footwraps; high boots; tied long riding coat; felt cloak; fur cap or headwrap; sash; travel pouch; amulet |
| `medieval_outfit_steppe_turkic_male_artisan` | work shirt; trousers; boots; work caftan; short felt cloak; fur cap; tool sash; tool pouch; amulet; gloves |
| `medieval_outfit_steppe_turkic_female_artisan` | work shift; trousers or skirt; boots; work coat; apron wrap; fur cap/headwrap; tool sash; tool pouch; amulet; gloves |
| `medieval_outfit_steppe_turkic_male_merchant` | fine shirt; riding trousers; high boots; felt riding caftan; lined cloak; fur cap; merchant sash; trade pouch; seal tag; gloves |
| `medieval_outfit_steppe_turkic_female_merchant` | fine shift; riding trousers or skirt; high boots; long riding caftan; lined cloak; headwrap; decorated sash; purse; seal tag; gloves |
| `medieval_outfit_steppe_turkic_male_noble` | silk under-shirt; fine trousers; high boots; embroidered riding caftan; fur-lined cloak; ornate fur cap; silk sash; seal pouch; belt plaques; gloves |
| `medieval_outfit_steppe_turkic_female_noble` | silk shift; fine trousers or skirt; high boots; embroidered long caftan; fur-lined cloak; ornate headwrap; silk sash; alms purse; belt plaques; gloves |
| `medieval_outfit_steppe_turkic_male_religious` | plain shirt; trousers; boots; sober caftan; felt cloak; fur cap; cord sash; pouch; prayer beads or amulet; herd tally |
| `medieval_outfit_steppe_turkic_female_religious` | plain shift; trousers or skirt; boots; sober long coat; felt cloak; headwrap; cord sash; pouch; prayer beads or amulet; herd tally |
| `medieval_outfit_steppe_turkic_male_military` | arming shirt; riding trousers; high boots; lamellar coat cover; felt war cloak; fur cap helm liner; bowcase-and-quiver belt; field pouch; amulet; horseman gloves |
| `medieval_outfit_steppe_turkic_female_military` | arming shift; riding trousers; high boots; lamellar riding coat cover; felt war cloak; fur cap/headwrap; bowcase-and-quiver belt; field pouch; amulet; horseman gloves |

### Song China (`song_china`)

| Outfit Reference | Target Pieces |
| --- | --- |
| `medieval_outfit_song_china_male_peasant` | plain under-robe; narrow trousers; cloth socks; cloth shoes; short working jacket; rain cape; cloth headwrap; simple sash; belt pouch; wooden tally |
| `medieval_outfit_song_china_female_peasant` | plain shift; skirt or trousers; cloth socks; cloth shoes; cross-collar work robe; rain cape; headcloth; simple sash; pouch; hair pin |
| `medieval_outfit_song_china_male_artisan` | work under-robe; narrow trousers; cloth shoes; work jacket; apron; cloth cap; tool sash; sleeve pouch; workshop tally; gloves |
| `medieval_outfit_song_china_female_artisan` | work shift; skirt or trousers; cloth shoes; work robe; apron; headcloth; tool sash; sleeve pouch; hair pin; gloves |
| `medieval_outfit_song_china_male_merchant` | fine under-robe; trousers; cloth shoes; merchant robe; lined outer robe; scholar-style cap; silk sash; account sleeve pouch; seal cord; gloves |
| `medieval_outfit_song_china_female_merchant` | fine shift; skirt; cloth shoes; cross-collar merchant robe; lined outer robe; headcloth or cap; silk sash; purse; hair ornament; account sleeve pouch |
| `medieval_outfit_song_china_male_noble` | silk under-robe; fine trousers; soft shoes; scholar robe; padded winter robe; official cap; silk sash; document sleeve pouch; official badge; gloves |
| `medieval_outfit_song_china_female_noble` | silk shift; fine skirt; soft shoes; elegant cross-collar robe; padded winter robe; formal headwear; silk sash; alms purse; hair ornament; pendant |
| `medieval_outfit_song_china_male_religious` | plain under-robe; trousers; sandals or cloth shoes; scholar-monastic robe; plain cloak; simple cap; cord sash; book pouch; prayer beads; notebook |
| `medieval_outfit_song_china_female_religious` | plain shift; skirt or trousers; cloth shoes; religious robe; plain cloak; head veil or cloth; cord sash; book pouch; prayer beads; prayer slip |
| `medieval_outfit_song_china_male_military` | arming under-robe; trousers; boots; padded military vest; lamellar cover robe; military cap; weapon sash; field pouch; guard token; bracers |
| `medieval_outfit_song_china_female_military` | arming shift; trousers or split skirt; boots; padded military vest; lamellar cover robe; headcloth under cap; weapon sash; field pouch; guard token; bracers |
