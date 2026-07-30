# FutureMUD Renaissance Clothing and Accessories Design Reference

## Status and scope

This document is the design authority for the Renaissance clothing branch, approximately 1400-1600 CE. It replaces the former catalogue scaffold with an implementation-oriented shared-culture catalogue. The branch owns clothing, underlayers, footwear, headwear, textile accessories, profession overlays, skins, outfits, and clothing crafts that are not supplied by the shared pre-industrial baseline.

The shared dependency foundation is implemented. HumanSeeder now supplies the nine clothing-specific profiles `Leg Wraps`, `Overshoes`, `Head Veil`, `Hood`, `Detachable Sleeves`, `Skirt Support`, `Partlet`, `Long Open Robe`, and the optional `Breechcloth`, with matching `Wear_*` components. UsefulSeeder supplies the complete Renaissance Shared culture hierarchy, clothing market/function tags, and institution tags. CoreDataSeeder and AgricultureSeeder supply the four exact textile materials and their production sources. The Renaissance clothing entry point validates these profiles, materials, full tag paths, and seeded components before any catalogue item can be authored. The ItemSeeder now derives stock item prototypes and 59 concrete `OutfitTemplate` manifests from the inferred manifest table below. Early Modern outfit seeding still ensures each specifically admitted Renaissance prototype needed by those later manifests without invoking the complete Renaissance branch. Skins and crafts remain later implementation work.

The expanded catalogue contains **471 unique proposed prototypes** and retains the **436 explicitly enumerated first-pass culture placements**. The 76 headwear/footwear expansion rows carry admission notes but are not counted as placements until a culture or outfit manifest explicitly adopts them. Forty-one placements deliberately admit an existing shared prototype into another culture grouping rather than cloning the same silhouette. The catalogue is split into this authority document and three regional volumes:

| Volume | Unique prototypes | Primary coverage |
| --- | ---: | --- |
| This authority and common-form catalogue | 71 | forms credible across several Shared groupings, dependency contracts, outfit rules |
| [Western, Mediterranean, and European Frontier Catalogue](./FutureMUD_Renaissance_Clothing_Catalogue_Western_Mediterranean.md) | 154 | Western European, Iberian Atlantic, Northern/Central/Eastern European, Ottoman-Islamicate |
| [Asian and Steppe Catalogue](./FutureMUD_Renaissance_Clothing_Catalogue_Asia_Steppe.md) | 130 | Persianate, Indo-Persian, South Asian, Ming/Joseon, Japanese/Ryukyuan, South-east Asian, steppe/caravan |
| [African, American, Contact, and Maritime Catalogue](./FutureMUD_Renaissance_Clothing_Catalogue_Africa_Americas_Maritime.md) | 116 | African court/Atlantic, Sahel/Red Sea/Swahili, Mesoamerican, Andean, Caribbean, North American contact, colonial and maritime overlays |
| **Total** | **471** | all Shared Renaissance material-culture groupings in the master culture manifest |

This is a design catalogue, not a claim that every listed form is common in every place between 1400 and 1600. Culture manifests, date gates, institutions, professions, crafts, shops, and outfit definitions control actual admission.

## Relationship to implemented shared stock

Do not clone the three implemented general accessories:

- `preindustrial_clothing_plain_leather_belt`
- `preindustrial_clothing_iron_buckled_leather_belt`
- `preindustrial_clothing_simple_woven_sash`

They should be admitted wherever historically credible and skinned for colour, textile, buckle finish, embroidery, household marks, or local terminology. A new belt or sash prototype is justified only by a materially different construction or gameplay role, such as a very broad wrapped waistcloth, a rigid ceremonial belt, a sword-bearing baldric, or a container-bearing working girdle.

## Catalogue admission rules

A proposed prototype is retained only when at least one of the following changes materially:

1. silhouette or visible construction;
2. primary material behaviour;
3. wearable coverage or layering profile;
4. functional item component;
5. production method;
6. institutional or professional gameplay role;
7. contact/export form that builders need to distinguish from local continuity.

Colour, trim, embroidery, woven motif, heraldry, guild or household marks, dynastic signs, religious symbols, exact local names, imported presentation, and most status differences are skins. Material substitutions remain skins only where the silhouette, weight, insulation, and craft inputs remain credible.

## Stable-reference and authoring contract

- Era-specific references use `renaissance_...`; genuinely multi-group forms in this document use `renaissance_shared_clothing_...`.
- Public keywords and short descriptions remain form-based and in-world. Builder notes may record historical inspirations and narrower local names.
- Portable garments include `Holdable`, a valid wearable component, `Destroyable_Clothing`, an insulation component appropriate to the material, and `Armour_LightClothing` unless a later armour decision explicitly supersedes it.
- `$colour` appears only where `Variable_BasicColour` is included and the material can reasonably be dyed.
- Decorative claims do not imply armour, concealment, storage, disguise, warmth, or weatherproofing mechanics unless a matching component exists.
- Paired garments such as hose, stockings, sleeves, leggings, gloves, and sandals are authored as one paired item unless a deliberate one-sided gameplay use is approved.
- Full descriptions describe cut, fastening, seams, drape, material, and visible wear. They do not mention seeding, tags, prototypes, or unsupported historical universality.

## Catalogue row schema

Each row records:

| Field | Meaning |
| --- | --- |
| Stable reference | Proposed idempotent prototype reference |
| Public form | Form-based noun phrase used to author keywords and short description |
| Material | Exact live primary material, or a named dependency from the material gap ledger |
| Wear profile | Abstract profile below; implementation must resolve it to an exact seeded component |
| Variation family | Minimum useful skin set; not additional prototypes unless construction changes |
| Admission | Shared culture, profession, institution, or date gate |

## Wear-profile dependency ledger

The maintained component export now contains every profile needed by this foundation. The catalogue retains semantic profile ids so future item rows select the appropriate exact component deliberately.

| Profile id | Required coverage/layering | Resolution status |
| --- | --- | --- |
| `WP-UNDER-WAIST` | drawers or loincloth beneath lower-body clothing | map to `Wear_Shorts` where coverage is correct |
| `WP-BREAST-WRAP` | wrapped breast support beneath upper layers | map to `Wear_Bra` |
| `WP-SHIRT` | short or long upper-body underlayer | map to `Wear_Shirt` where hem coverage is correct |
| `WP-LONG-UNDERLAYER` | ankle- or calf-length under-robe/shift | use `Wear_Gown`, `Wear_Dress`, or `Wear_Long_Skirt` according to the authored silhouette |
| `WP-VEST` | sleeveless torso layer above a shirt and below a coat | use `Wear_Vest` |
| `WP-JACKET` | waist/hip-length sleeved outer torso layer | use `Wear_Jacket` |
| `WP-ROBE-CLOSED` | long closed or pull-over robe covering torso and legs | use `Wear_Robe` or the appropriate gown component |
| `WP-ROBE-OPEN` | long front-opening robe/coat layered over clothing | use `Wear_Long_Open_Robe` |
| `WP-TROUSERS` | joined lower-body garment covering both legs | use `Wear_Trousers` or `Wear_Chausses` where separately fitted hose is intended |
| `WP-WRAP-SKIRT` | wrapped lower-body cloth with overlap | use `Wear_Skirt` or `Wear_Long_Skirt` according to length |
| `WP-SKIRT` | sewn or gathered lower-body garment | use `Wear_Skirt` or `Wear_Long_Skirt` |
| `WP-STOCKINGS` | paired close legwear extending above the knee | use `Wear_Stockings` |
| `WP-LEG-WRAPS` | paired lower-leg wraps or gaiters | use `Wear_Leg_Wraps` |
| `WP-CLOAK` | neck/shoulder-fastened outer wrap covering back and arms | use `Wear_Cloak_(Open)` or `Wear_Cloak_(Closed)` |
| `WP-SHOULDER` | mantle, shawl, or shoulder cloth that does not occupy coat layering | use `Wear_Mantle` |
| `WP-FOOT-SANDAL` | open footwear | use `Wear_Sandals` |
| `WP-FOOT-SHOE` | closed low footwear | use `Wear_Shoes` |
| `WP-FOOT-BOOT` | closed footwear extending above ankle | use `Wear_Boots` |
| `WP-OVERSHOE` | footwear worn over shoes or footwraps | use `Wear_Overshoes` |
| `WP-HEAD-CAP` | close or soft cap | use `Wear_Hat` |
| `WP-HEAD-HAT` | brimmed or structured hat | use `Wear_Hat` |
| `WP-HEADWRAP` | wound cloth occupying the headwear layer | use `Wear_Turban` |
| `WP-HEAD-VEIL` | veil draped from head without full face coverage | use `Wear_Head_Veil` |
| `WP-HOOD` | separate hood covering head and neck | use `Wear_Hood` |
| `WP-FACE-VEIL` | textile face covering compatible with headwear | use `Wear_Veil` |
| `WP-FACE-MASK` | rigid or textile mask occupying face layer | use `Wear_Mask` |
| `WP-HANDS` | paired gloves or mittens | use `Wear_Gloves` |
| `WP-NECK` | kerchief, collar, or neck cloth | use `Wear_Scarf` |
| `WP-SLEEVES` | detachable paired sleeves layered with bodice/doublet | use `Wear_Detachable_Sleeves` |
| `WP-HANDHELD` | fan or similar carried accessory | use `Holdable`; no wearable component |

### Implemented component foundation

The exact wearable definitions called out by the dependency audit are now seeded and exported. `Wear_Skirt_Support`, `Wear_Partlet`, and `Wear_Breechcloth` are also available for regional catalogue rows that require those constructions.

Together with the reused stock components, the resolved foundation covers:

- long closed robes and gowns;
- open robes and long coats;
- wrap skirts and waistcloths;
- paired hose/stockings and lower-leg wraps;
- shoulder mantles and shawls that should layer independently of cloaks;
- headwraps/turbans and head-draped veils;
- face veils that layer with headwear;
- detachable sleeves;
- overshoes worn above another footwear layer.

These remain wearable/layering components, not new item-component *types*. No new engine component type was required.

## Material dependency ledger

### Live exact materials used directly

`linen`, `wool`, `cotton`, `silk`, `leather`, `felt`, `canvas`, `broadcloth`, `velvet`, `satin`, `lace`, `taffeta`, `ribbon`, `calico`, `chintz`, `ramie cloth`, `barkcloth`, `camelid wool`, and `raffia cloth` are live exact materials. Agriculture outputs the last four through Ramie, Breadfruit, Raffia Palms, and Llama/Alpaca definitions respectively.

### Deliberately deferred material abstractions

| Proposed material | Priority | Why a separate material is useful | Fallback before seeding |
| --- | --- | --- | --- |
| `hemp cloth` | high | common hard-wearing fibre with different craft and trade chain from linen | `linen` or `canvas`, with reduced cultural specificity |
| `brocade` | medium | patterned supplementary-weft luxury textile with distinct cost/craft behaviour | `silk` or `satin` skin |
| `damask` | medium | reversible figured textile used across several court markets | `silk` or `linen` skin |
| `silk gauze` | medium | open, translucent silk behaviour needed for veils and court layers | `silk` |
| `featherwork` | medium | manufactured feather mosaic/panel composite used as a primary visible surface | `feather` if live, otherwise descriptive trim only |
| `beadwork` | low | composite surface for regalia and contact goods | keep beads as descriptive trim |

The deferred rows above are intentionally represented by existing exact materials or descriptive decoration until a later production-chain pass justifies distinct mechanics. They are not clothing-seeder blockers.

## Tag additions

`Era / Renaissance Era` and all culture-family paths below are now live maintained stock and are seeded idempotently before catalogue rows depend on them:

```text
Culture / Renaissance / Shared / Western European Renaissance
Culture / Renaissance / Shared / Iberian Atlantic
Culture / Renaissance / Shared / Central European
Culture / Renaissance / Shared / Northern Baltic
Culture / Renaissance / Shared / Central Eastern Frontier
Culture / Renaissance / Shared / Eastern Orthodox Northern
Culture / Renaissance / Shared / Ottoman Islamicate
Culture / Renaissance / Shared / Persianate Indo-Persian
Culture / Renaissance / Shared / South Asian
Culture / Renaissance / Shared / East Asian Literati
Culture / Renaissance / Shared / Japanese
Culture / Renaissance / Shared / Maritime East Asian
Culture / Renaissance / Shared / South-east Asian Mainland
Culture / Renaissance / Shared / Maritime South-east Asian
Culture / Renaissance / Shared / Steppe and Caravan
Culture / Renaissance / Shared / African Court Atlantic
Culture / Renaissance / Shared / Sahelian Islamic
Culture / Renaissance / Shared / Red Sea
Culture / Renaissance / Shared / Indian Ocean
Culture / Renaissance / Shared / Mesoamerican
Culture / Renaissance / Shared / Andean
Culture / Renaissance / Shared / Caribbean Contact
Culture / Renaissance / Shared / North American Contact
Culture / Renaissance / Shared / Colonial Atlantic
Culture / Renaissance / Shared / Global Maritime
```

The following functional/market paths should be audited and added only where absent:

```text
Market / Clothing / Work Clothing
Market / Clothing / Ceremonial Clothing
Market / Clothing / Religious Clothing
Market / Clothing / Maritime Clothing
Functions / Worn Items / Underlayers
Functions / Worn Items / Robes
Functions / Worn Items / Outerwear
Functions / Worn Items / Footwear
Functions / Worn Items / Headwear
Functions / Worn Items / Facewear
Functions / Worn Items / Handwear
Functions / Worn Items / Neckwear
Functions / Worn Items / Detachable Garment Parts
Institution / Court
Institution / Religious
Institution / Guild
Institution / Maritime
Institution / Performance
Institution / Service Household
```

Status and profession tags must not replace culture/date admission. A court garment can be locally made, imported, gifted, or inherited; those distinctions belong in skins, shops, manifests, and builder notes.

## Headwear and footwear expansion policy

This pass adds **76** proposed prototypes across the authority and regional volumes: **42 headwear** and **34 footwear** forms. It does not assign a quota to every culture or rewrite the outfit minimums. The new rows exist to let builders distinguish:

- field, workshop, port, shipboard, and domestic work;
- ordinary town and merchant dress;
- road, pilgrimage, mounted, caravan, and winter travel;
- court, diplomatic, academic, official, religious, and ceremonial dress;
- footwear worn directly from overshoes used to protect another shoe;
- soft indoor footwear from hard road, field, riding, and maritime footwear;
- caps, hoods, headwraps, structured hats, and status headdresses that cannot honestly be represented by one generic skin.

No new component type, wearable component, tag, or solid material is requested. The existing `WP-HEAD-CAP`, `WP-HEAD-HAT`, `WP-HEADWRAP`, `WP-HEAD-VEIL`, `WP-HOOD`, `WP-FOOT-SANDAL`, `WP-FOOT-SHOE`, `WP-FOOT-BOOT`, and `WP-OVERSHOE` mappings cover all rows. The inferred manifest table below is now the explicit admission source for the expansion rows it references; unreferenced expansion rows remain proposals rather than stock outfit placements.

## Common-form prototype catalogue — 71 unique prototypes

These forms are shared only in the catalogue sense: several culture families can plausibly use the same underlying construction. Each culture still requires an admission decision.

### Underlayers and close bodywear — 10

| Stable reference | Public form | Material | Wear profile | Variation family | Admission |
| --- | --- | --- | --- | --- | --- |
| `renaissance_shared_clothing_drawstring_drawers` | loose drawstring drawers | linen | `WP-UNDER-WAIST` | knee/calf length; plain/fine; undyed/dyed | broad urban, court, military, and maritime underlayer where sewn drawers are credible |
| `renaissance_shared_clothing_wrapped_loincloth` | wrapped loincloth | linen | `WP-UNDER-WAIST` | narrow/broad; tucked/tied; cotton skin | tropical, labour, maritime, American, African, South/South-east Asian admissions |
| `renaissance_shared_clothing_breast_wrap` | folded breast wrap | linen | `WP-BREAST-WRAP` | narrow/broad; tied front/back; cotton skin | multiple wrapped-underlayer traditions; not universal |
| `renaissance_shared_clothing_short_undershirt` | short undershirt | linen | `WP-SHIRT` | sleeveless/short sleeve; square/round neck | warm-climate and layered urban use |
| `renaissance_shared_clothing_long_undershirt` | long undershirt | linen | `WP-SHIRT` | thigh/knee hem; narrow/full sleeve | broad sewn-underlayer admission |
| `renaissance_shared_clothing_sleeveless_undervest` | sleeveless undervest | linen | `WP-VEST` | fitted/loose; low/high neck | layered court, military, and work dress where supported |
| `renaissance_shared_clothing_straight_underrobe` | straight under-robe | linen | `WP-LONG-UNDERLAYER` | calf/ankle; narrow/full sleeve; cotton skin | robe systems across Ottoman, Persianate, Asian, African, and European contexts |
| `renaissance_shared_clothing_footed_stockings` | footed cloth stockings | wool | `WP-STOCKINGS` | knee/thigh high; clocked/plain; silk skin | Western, Central/Eastern European, Ottoman and court contact admissions |
| `renaissance_shared_clothing_soleless_stockings` | soleless cloth stockings | wool | `WP-STOCKINGS` | knee/thigh high; stirrup/plain | cultures using separate footwraps or shoes |
| `renaissance_shared_clothing_leg_wraps` | paired lower-leg wraps | wool | `WP-LEG-WRAPS` | narrow/broad; spiral/cross-wrapped; linen skin | work, travel, military, northern, steppe, and rural admissions |

### Core upper- and lower-body forms — 12

| Stable reference | Public form | Material | Wear profile | Variation family | Admission |
| --- | --- | --- | --- | --- | --- |
| `renaissance_shared_clothing_straight_tunic` | straight-cut tunic | linen | `WP-SHIRT` | hip/knee length; split/unsplit hem; wool/cotton skins | broad continuity form where a regional silhouette is not more specific |
| `renaissance_shared_clothing_side_slit_tunic` | side-slit tunic | cotton | `WP-SHIRT` | short/long slit; narrow/full sleeve; silk skin | African, South Asian, Persianate, steppe, and maritime Asian admissions |
| `renaissance_shared_clothing_crossfront_jacket` | cross-front jacket | cotton | `WP-JACKET` | left/right overlap; tied/belted; silk/ramie skins | East, South-east, Central, and contact-zone Asian admissions |
| `renaissance_shared_clothing_wrap_jacket` | short wrap jacket | linen | `WP-JACKET` | sleeveless/long sleeve; inner/outer ties | work and warm-climate contexts across several groupings |
| `renaissance_shared_clothing_front_opening_long_coat` | front-opening long coat | wool | `WP-ROBE-OPEN` | calf/ankle; fitted/loose; fur-lined skin | steppe, Eastern European, Ottoman, Persianate, and northern travel contexts |
| `renaissance_shared_clothing_sleeveless_overvest` | sleeveless over-vest | wool | `WP-VEST` | short/long; open/fastened; leather skin | work, travel, military, and court layering |
| `renaissance_shared_clothing_quilted_jacket` | quilted jacket | cotton | `WP-JACKET` | hip/thigh length; light/heavy quilting; silk skin | civilian warmth and under-armour use; no armour claim beyond clothing components |
| `renaissance_shared_clothing_straight_trousers` | straight-legged trousers | wool | `WP-TROUSERS` | ankle/calf; drawstring/fly-front skin; cotton/linen skins | broad trouser-using culture groups |
| `renaissance_shared_clothing_narrow_trousers` | narrow-legged trousers | wool | `WP-TROUSERS` | footed/stirrup/plain; cotton/silk skins | riding, court, steppe, Ottoman, Persianate, South Asian, East Asian admissions |
| `renaissance_shared_clothing_full_trousers` | full gathered trousers | cotton | `WP-TROUSERS` | ankle/calf gathering; very full/moderate; silk skin | Ottoman, Persianate, South Asian, African and maritime Asian contexts |
| `renaissance_shared_clothing_wrap_skirt` | overlapping wrap skirt | cotton | `WP-WRAP-SKIRT` | knee/ankle; narrow/broad overlap; silk/barkcloth skins | South/South-east/East Asian, African, American, and maritime admissions |
| `renaissance_shared_clothing_gathered_skirt` | gathered sewn skirt | wool | `WP-SKIRT` | ankle/calf; narrow/full; linen/cotton/silk skins | European, African, American contact, and urban work admissions |

### Outerwear and shoulder layers — 8

| Stable reference | Public form | Material | Wear profile | Variation family | Admission |
| --- | --- | --- | --- | --- | --- |
| `renaissance_shared_clothing_rectangular_mantle` | rectangular shoulder mantle | wool | `WP-SHOULDER` | pinned/tied/draped; short/long; cotton/camelid skins | widespread draped outer layer with local skins |
| `renaissance_shared_clothing_hooded_cloak` | hooded full cloak | wool | `WP-CLOAK` | semicircular/rectangular; clasp/ties; lined/unlined | cool, wet, travel, maritime, and northern admissions |
| `renaissance_shared_clothing_sleeveless_cape` | sleeveless short cape | wool | `WP-CLOAK` | hip/knee; open/fastened; velvet skin | court, civic, clerical, travel, and military-overdress admissions |
| `renaissance_shared_clothing_shoulder_shawl` | broad shoulder shawl | wool | `WP-SHOULDER` | rectangular/triangular; plain/fringed; cotton/silk skins | Persianate, South Asian, steppe, African, Andean, European elite/contact admissions |
| `renaissance_shared_clothing_rain_cape` | close-woven rain cape | wool | `WP-CLOAK` | hooded/collarless; hip/knee | maritime, mountain, pastoral, and travel contexts; descriptive water shedding only |
| `renaissance_shared_clothing_fur_lined_coat` | fur-lined travelling coat | wool | `WP-ROBE-OPEN` | calf/ankle; broad/narrow collar; prestige/work skins | northern, steppe, Muscovite, Central/Eastern European admissions; audit exact fur material |
| `renaissance_shared_clothing_travelling_coat` | loose travelling coat | broadcloth | `WP-ROBE-OPEN` | knee/calf; hood/collar; canvas skin | merchant, courier, pilgrim, maritime, and caravan overlays |
| `renaissance_shared_clothing_ceremonial_mantle` | long ceremonial mantle | velvet | `WP-SHOULDER` | train/no train; clasp/ties; silk/brocade skins | court, civic, religious, diplomatic, and performance contexts only |

### Footwear — 8

| Stable reference | Public form | Material | Wear profile | Variation family | Admission |
| --- | --- | --- | --- | --- | --- |
| `renaissance_shared_clothing_leather_sandals` | pair of leather sandals | leather | `WP-FOOT-SANDAL` | thong/strap; flat/raised sole; plain/decorated | warm-climate, religious, labour, African, Asian, and American admissions |
| `renaissance_shared_clothing_textile_sandals` | pair of woven sandals | canvas | `WP-FOOT-SANDAL` | braided/woven; open/closed toe; hemp/raffia skins | East Asian, South-east Asian, African, maritime, and rural work contexts |
| `renaissance_shared_clothing_soft_slippers` | pair of soft slippers | leather | `WP-FOOT-SHOE` | backless/heeled-back; pointed/round; velvet/silk skins | court, domestic, Ottoman, Persianate, South Asian, East Asian, African urban admissions |
| `renaissance_shared_clothing_low_leather_shoes` | pair of low leather shoes | leather | `WP-FOOT-SHOE` | latchet/tie; round/pointed; plain/slashed skin | broad urban and rural admissions where closed sewn shoes are credible |
| `renaissance_shared_clothing_ankle_boots` | pair of ankle boots | leather | `WP-FOOT-BOOT` | side-laced/pull-on; soft/stiff shaft | travel, riding, work, military, maritime, and steppe contexts |
| `renaissance_shared_clothing_high_riding_boots` | pair of high riding boots | leather | `WP-FOOT-BOOT` | knee/thigh; soft/stiff shaft; turned cuff skin | cavalry, courier, steppe, frontier, elite travel admissions |
| `renaissance_shared_clothing_wooden_soled_overshoes` | pair of wooden-soled overshoes | wood | `WP-OVERSHOE` | low/high platform; strap/closed upper | wet streets, bathhouse, court, East Asian, Ottoman, European urban admissions where locally credible |
| `renaissance_shared_clothing_footwraps` | pair of cloth footwraps | linen | `WP-STOCKINGS` | square/long strip; wool skin | military, labour, rural, northern, steppe, and low-shoe admissions |

### Head, face, and neck forms — 9

| Stable reference | Public form | Material | Wear profile | Variation family | Admission |
| --- | --- | --- | --- | --- | --- |
| `renaissance_shared_clothing_close_cloth_cap` | close-fitting cloth cap | linen | `WP-HEAD-CAP` | tied/untied; eared/plain; cotton/silk skins | under-hat, domestic, work, religious, East Asian, European, African admissions |
| `renaissance_shared_clothing_soft_brimless_cap` | soft brimless cap | wool | `WP-HEAD-CAP` | rounded/pointed; folded/unfolded; felt/silk skins | urban, rural, scholar, sailor, steppe, African and European admissions |
| `renaissance_shared_clothing_felt_brimmed_hat` | brimmed felt hat | felt | `WP-HEAD-HAT` | narrow/broad brim; low/high crown; band/feather skins | European, Ottoman contact, maritime, colonial, and trade contexts |
| `renaissance_shared_clothing_straw_brimmed_hat` | woven straw hat | straw | `WP-HEAD-HAT` | flat/conical crown; narrow/broad brim; chin ties | field, maritime, tropical, East/South-east Asian, African, American admissions; audit exact straw material |
| `renaissance_shared_clothing_cloth_headwrap` | long cloth headwrap | cotton | `WP-HEADWRAP` | narrow/broad; compact/voluminous; linen/silk skins | Ottoman, Persianate, South Asian, African, steppe, maritime and contact contexts |
| `renaissance_shared_clothing_long_head_veil` | long head-draped veil | linen | `WP-HEAD-VEIL` | shoulder/waist length; opaque/shear silk skin | religious, court, married-status, mourning, and regional admissions |
| `renaissance_shared_clothing_short_head_veil` | short head veil | linen | `WP-HEAD-VEIL` | pinned/tied; back/side drape; lace/silk skins | European, Mediterranean, Ottoman, African, and contact-zone admissions |
| `renaissance_shared_clothing_separate_hood` | separate cloth hood | wool | `WP-HOOD` | close/loose; shoulder cape/no cape; linen skin | European, northern, maritime, religious, travelling admissions |
| `renaissance_shared_clothing_face_veil` | draped face veil | silk | `WP-FACE-VEIL` | nose-to-chin/full lower face; tied/pinned; cotton skin | culture/date/status-gated Ottoman, Persianate, South Asian, African, and ceremonial admissions |

### Handheld and small worn accessories — 8

| Stable reference | Public form | Material | Wear profile | Variation family | Admission |
| --- | --- | --- | --- | --- | --- |
| `renaissance_shared_clothing_cloth_gloves` | pair of cloth gloves | wool | `WP-HANDS` | short/long cuff; plain/embroidered; silk skin | cool-climate, court, clerical, scholarly, ceremonial admissions |
| `renaissance_shared_clothing_leather_gloves` | pair of leather gloves | leather | `WP-HANDS` | short/gauntlet cuff; work/fine; perfumed skin | riding, work, court, falconry skin, maritime and military support |
| `renaissance_shared_clothing_mittens` | pair of cloth mittens | wool | `WP-HANDS` | short/long cuff; felt/fur-lined skins | northern, mountain, steppe, maritime cold-weather admissions |
| `renaissance_shared_clothing_neck_kerchief` | tied neck kerchief | linen | `WP-NECK` | narrow/broad; tied front/back; cotton/lace skins | work, sailor, artisan, domestic, court accessory admissions |
| `renaissance_shared_clothing_shoulder_scarf` | long shoulder scarf | silk | `WP-SHOULDER` | narrow/broad; fringed/plain; cotton/wool skins | court, diplomatic, religious, military-status and performance overlays |
| `renaissance_shared_clothing_folding_hand_fan` | folding hand fan | wood | `WP-HANDHELD` | paper/silk leaf; plain/painted; court/merchant skins | late-period East Asian, European court/contact, Ottoman and maritime trade admissions; date gate required |
| `renaissance_shared_clothing_half_mask` | shaped half mask | leather | `WP-FACE-MASK` | eye/upper-face; plain/painted; velvet skin | theatre, pageant, festival, court entertainment, disguise-support only if a component is later added |
| `renaissance_shared_clothing_detachable_sleeves` | pair of detachable sleeves | silk | `WP-SLEEVES` | tight/full; tied/laced; linen/wool/velvet skins | Western European, Ottoman, Persianate, South Asian and courtly layered admissions where construction supports it |

### Headwear and footwear expansion — 16

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_shared_clothing_quilted_nightcap` | quilted linen nightcap | linen | `WP-HEAD-CAP` | Indoor, sleeping, scholar, invalid, and under-hat skins; embroidery may make a luxury undress version without changing the cap profile. |
| `renaissance_shared_clothing_leather_work_skullcap` | close leather work skullcap | leather | `WP-HEAD-CAP` | Close fitted cap for mining, smithing, shipboard labour, riding, and other work where a loose brim is inconvenient; no armour claim. |
| `renaissance_shared_clothing_tarred_canvas_deckcap` | close tarred canvas deck cap | canvas | `WP-HEAD-CAP` | Shipboard and dockside gate; tarred appearance is descriptive and grants no waterproof mechanic. |
| `renaissance_shared_clothing_chincord_ridinghat` | chin-cord riding hat | felt | `WP-HEAD-HAT` | Low or moderate crown with securing cord; courier, hunting, caravan, and mounted-work admissions. |
| `renaissance_shared_clothing_tallcrown_felthat` | tall-crowned felt hat | felt | `WP-HEAD-HAT` | Late-fifteenth through sixteenth-century town, merchant, court-contact, and frontier admissions; crown and brim proportions are skins. |
| `renaissance_shared_clothing_brimmed_leather_travelhat` | brimmed leather travel hat | leather | `WP-HEAD-HAT` | Hard-wearing travel, hunting, courier, pastoral, and maritime-contact form; no weatherproof mechanic. |
| `renaissance_shared_clothing_folded_pilgrimhat` | folded-brim pilgrim hat | felt | `WP-HEAD-HAT` | Road, shrine, confraternity, and long-distance travel admission; badges and shells remain separate accessories. |
| `renaissance_shared_clothing_furlined_winterhood` | fur-lined winter hood | wool | `WP-HOOD` | Deep separate hood for northern, mountain, caravan, and winter travel; exact fur and shoulder cape are skins where profile-compatible. |
| `renaissance_shared_clothing_thicksoled_workshoes` | pair of thick-soled work shoes | leather | `WP-FOOT-SHOE` | Field, workshop, building, stable, mine, and dock labour; hobnails or extra sole layers are skins unless behaviour changes. |
| `renaissance_shared_clothing_latchet_walkingshoes` | pair of latchet walking shoes | leather | `WP-FOOT-SHOE` | One- or two-latchet town, road, merchant, and pilgrim form; buckle and tie closures remain skins. |
| `renaissance_shared_clothing_sidelaced_townshoes` | pair of side-laced town shoes | leather | `WP-FOOT-SHOE` | Close ankle-height urban form distinct from low latchet shoes; ordinary, fine, and court-servant skins. |
| `renaissance_shared_clothing_frontlaced_ankleboots` | pair of front-laced ankle boots | leather | `WP-FOOT-BOOT` | Travel, work, guard, courier, and mounted admissions; lower and more fitted than high riding boots. |
| `renaissance_shared_clothing_low_ridingshoes` | pair of heeled low riding shoes | leather | `WP-FOOT-SHOE` | Riding, hunting, messenger, and prosperous travel form where tall boots are unnecessary; spur fittings remain separate equipment. |
| `renaissance_shared_clothing_ropesole_deckshoes` | pair of rope-soled deck shoes | canvas | `WP-FOOT-SHOE` | Warm-water maritime, fishing, galley, dock, and boatman admission; woven sole and closed upper distinguish them from sandals. |
| `renaissance_shared_clothing_felt_overshoes` | pair of felt overshoes | felt | `WP-OVERSHOE` | Northern, steppe, mountain, and winter-household layer worn over shoes or footwraps. |
| `renaissance_shared_clothing_fine_velvet_slippers` | pair of fine velvet slippers | velvet | `WP-FOOT-SHOE` | Court, prosperous household, diplomatic, scholar, and ceremonial indoor footwear; embroidery and jewels are skins. |

## Shared placement table — 41 deliberate reuses

The regional volumes contain 41 placements that point back to common or another regional prototype. They are admissions, not aliases and not additional item rows. Examples include low leather shoes admitted into colonial Atlantic outfits, shoulder shawls admitted into Andean elite/contact outfits, quilted jackets admitted into Japanese and steppe military underlayers, and cloth headwraps admitted across Ottoman, Persianate, South Asian, African, and maritime cultures. Each regional volume records its reuse count; all stable references remain unique.

## Inferred outfit manifests — 59 stock templates

These manifests turn the regional outfit minimums into concrete builder-facing stock. They are intentionally representative rather than universal: colour, trim, exact local terminology, gender admission, office, faith, rank, and narrower culture remain skins or downstream culture/date gates. Item order is inner-to-outer wear order. Every reference is explicit so seeding fails closed if a required prototype cannot be created.

The regional volumes provide only a broad North American contact placeholder and explicitly prohibit its use without a narrower regional design pass. No stock North American outfit is inferred here. Its prototype rows remain available for a future locally anchored manifest.

### Western, Mediterranean, and European frontier manifests — 14

| Stable key | Template name | Admission | Purpose | Ordered item references |
| --- | --- | --- | --- | --- |
| `renaissance_outfit_western_ordinary` | Renaissance: Western European Ordinary Town Dress | Western European Renaissance, 1450-1575 | ordinary urban or skilled-household dress | `renaissance_shared_clothing_drawstring_drawers`<br>`renaissance_western_gathered_linen_shirt`<br>`renaissance_western_cloth_jerkin`<br>`renaissance_western_joined_hose`<br>`renaissance_western_flat_cap`<br>`renaissance_western_square_toed_shoes` |
| `renaissance_outfit_western_status` | Renaissance: Western European Court Dress | Western European Renaissance, 1500-1600; court/status gate | court, diplomatic, or prosperous ceremonial dress | `renaissance_western_pleated_linen_shirt`<br>`renaissance_western_slashed_doublet`<br>`renaissance_western_paned_trunk_hose`<br>`renaissance_western_short_cloak`<br>`renaissance_western_feathered_velvet_bonnet`<br>`renaissance_western_fine_latchet_shoes` |
| `renaissance_outfit_iberian_ordinary` | Renaissance: Iberian Atlantic Mariner Dress | Iberian Atlantic, 1500-1600; maritime admission | sailor, dockworker, or coastal traveller | `renaissance_shared_clothing_drawstring_drawers`<br>`renaissance_western_open_neck_work_shirt`<br>`renaissance_iberian_full_shipboard_breeches`<br>`renaissance_iberian_short_sailor_jacket`<br>`renaissance_iberian_knitted_sailor_cap`<br>`renaissance_shared_clothing_ankle_boots` |
| `renaissance_outfit_iberian_status` | Renaissance: Iberian Atlantic Prosperous Dress | Iberian Atlantic, 1500-1600; prosperous/status gate | merchant, officer-presentation, or household formality | `renaissance_western_high_collar_shirt`<br>`renaissance_western_short_skirted_doublet`<br>`renaissance_western_venetian_breeches`<br>`renaissance_western_full_circle_cloak`<br>`renaissance_iberian_broad_crowned_hat`<br>`renaissance_western_fine_latchet_shoes` |
| `renaissance_outfit_central_european_ordinary` | Renaissance: Central European Town Dress | Central European, 1450-1600 | ordinary town, artisan, or travelling dress | `renaissance_shared_clothing_drawstring_drawers`<br>`renaissance_western_open_neck_work_shirt`<br>`renaissance_western_long_skirted_jerkin`<br>`renaissance_frontier_wide_hungarian_trousers`<br>`renaissance_frontier_fur_brimmed_cap`<br>`renaissance_shared_clothing_ankle_boots` |
| `renaissance_outfit_central_european_status` | Renaissance: Central European Noble Dress | Central European, 1500-1600; court/noble gate | court, civic office, or noble household dress | `renaissance_western_high_collar_shirt`<br>`renaissance_western_slashed_doublet`<br>`renaissance_western_pluderhosen`<br>`renaissance_frontier_split_sleeve_noblecoat`<br>`renaissance_frontier_tall_fur_hat`<br>`renaissance_frontier_split_skirt_riding_boots` |
| `renaissance_outfit_northern_baltic_ordinary` | Renaissance: Northern Baltic Work Dress | Northern Baltic, 1450-1600 | rural, port, workshop, or cold-road dress | `renaissance_shared_clothing_long_undershirt`<br>`renaissance_shared_clothing_straight_trousers`<br>`renaissance_shared_clothing_leg_wraps`<br>`renaissance_shared_clothing_hooded_cloak`<br>`renaissance_frontier_knitted_northern_cap`<br>`renaissance_shared_clothing_low_leather_shoes` |
| `renaissance_outfit_northern_baltic_status` | Renaissance: Northern Baltic Prosperous Winter Dress | Northern Baltic, 1450-1600; prosperous/status gate | merchant, civic officer, or prosperous household winter dress | `renaissance_western_high_collar_shirt`<br>`renaissance_frontier_wide_hungarian_trousers`<br>`renaissance_shared_clothing_fur_lined_coat`<br>`renaissance_frontier_fur_brimmed_cap`<br>`renaissance_frontier_felted_winter_boots` |
| `renaissance_outfit_central_eastern_frontier_ordinary` | Renaissance: Central-Eastern Frontier Ordinary Dress | Central Eastern Frontier, 1450-1600 | town, caravan, mounted-work, or household dress | `renaissance_shared_clothing_long_undershirt`<br>`renaissance_frontier_wide_hungarian_trousers`<br>`renaissance_frontier_belted_wool_caftan`<br>`renaissance_frontier_fur_brimmed_cap`<br>`renaissance_shared_clothing_ankle_boots` |
| `renaissance_outfit_central_eastern_frontier_status` | Renaissance: Central-Eastern Frontier Noble Dress | Central Eastern Frontier, 1450-1600; noble/status gate | noble household, diplomatic, or mounted-retainer dress | `renaissance_western_pleated_linen_shirt`<br>`renaissance_frontier_split_sleeve_noblecoat`<br>`renaissance_frontier_tall_fur_hat`<br>`renaissance_frontier_split_skirt_riding_boots` |
| `renaissance_outfit_eastern_orthodox_northern_ordinary` | Renaissance: Eastern Orthodox Northern Ordinary Dress | Eastern Orthodox Northern, 1450-1600 | ordinary town, household, or cold-weather travel dress | `renaissance_shared_clothing_straight_underrobe`<br>`renaissance_shared_clothing_full_trousers`<br>`renaissance_frontier_close_buttoned_longcoat`<br>`renaissance_frontier_knitted_northern_cap`<br>`renaissance_frontier_felted_winter_boots` |
| `renaissance_outfit_eastern_orthodox_northern_status` | Renaissance: Eastern Orthodox Northern Court Dress | Eastern Orthodox Northern, 1450-1600; court/status gate | court, embassy, senior household, or ceremonial dress | `renaissance_shared_clothing_long_undershirt`<br>`renaissance_frontier_sleeveless_long_gown`<br>`renaissance_frontier_long_furred_overcoat`<br>`renaissance_frontier_tall_fur_hat`<br>`renaissance_frontier_split_skirt_riding_boots` |
| `renaissance_outfit_ottoman_ordinary` | Renaissance: Ottoman Urban Ordinary Dress | Ottoman Islamicate, 1450-1600 | ordinary urban, merchant, artisan, or household dress | `renaissance_shared_clothing_drawstring_drawers`<br>`renaissance_ottoman_collarless_inner_shirt`<br>`renaissance_ottoman_full_salwar_trousers`<br>`renaissance_ottoman_short_entari_coat`<br>`renaissance_ottoman_wrapped_cap_turban`<br>`renaissance_ottoman_pointed_slippers` |
| `renaissance_outfit_ottoman_status` | Renaissance: Ottoman Court Dress | Ottoman Islamicate, 1450-1600; court/office gate | court, office, diplomatic, or ceremonial dress | `renaissance_ottoman_collarless_inner_shirt`<br>`renaissance_ottoman_full_salwar_trousers`<br>`renaissance_ottoman_fitted_kaftan`<br>`renaissance_ottoman_full_outdoor_overrobe`<br>`renaissance_ottoman_high_court_turban`<br>`renaissance_ottoman_pointed_slippers` |

### Asian and steppe manifests — 16

| Stable key | Template name | Admission | Purpose | Ordered item references |
| --- | --- | --- | --- | --- |
| `renaissance_outfit_persianate_ordinary` | Renaissance: Persianate Urban Ordinary Dress | Persianate Indo-Persian, 1501-1600 | ordinary urban, learned, merchant, or household dress | `renaissance_persianate_long_inner_shirt`<br>`renaissance_persianate_close_ankle_trousers`<br>`renaissance_persianate_short_fitted_coat`<br>`renaissance_persianate_conical_turban`<br>`renaissance_persianate_pointed_slippers` |
| `renaissance_outfit_persianate_status` | Renaissance: Persianate Safavid Court Dress | Persianate Indo-Persian, 1501-1600; court/status gate | court, diplomatic, office, or ceremonial dress | `renaissance_persianate_long_inner_shirt`<br>`renaissance_persianate_wide_pleated_trousers`<br>`renaissance_persianate_broad_skirt_courtrobe`<br>`renaissance_persianate_tall_turban`<br>`renaissance_persianate_closedback_embroideredslippers` |
| `renaissance_outfit_south_asian_ordinary` | Renaissance: South Asian Ordinary Dress | South Asian, 1450-1600; local admission required | ordinary household, market, artisan, or rural dress | `renaissance_southasian_stitched_loindrawers`<br>`renaissance_southasian_short_sideslit_tunic`<br>`renaissance_southasian_pleated_waistcloth`<br>`renaissance_southasian_flat_wound_turban`<br>`renaissance_southasian_toepost_woodensandals` |
| `renaissance_outfit_south_asian_status` | Renaissance: South Asian Court Dress | South Asian, 1526-1600; court/status gate | court, diplomatic, prosperous, or ceremonial dress | `renaissance_southasian_bunched_ankletrousers`<br>`renaissance_southasian_short_sideslit_tunic`<br>`renaissance_southasian_long_crossover_robe`<br>`renaissance_southasian_headbanded_courtveil`<br>`renaissance_southasian_curvedtoe_shoes` |
| `renaissance_outfit_east_asian_literati_ordinary` | Renaissance: Ming Literati Ordinary Dress | East Asian Literati, Ming-facing, 1450-1600 | scholar, clerk, merchant, or prosperous household dress | `renaissance_ming_crosscollar_innerrobe`<br>`renaissance_ming_wide_drawstring_trousers`<br>`renaissance_ming_long_sidefastened_jacket`<br>`renaissance_ming_soft_scholarcap`<br>`renaissance_ming_black_cloth_scholarshoes` |
| `renaissance_outfit_east_asian_literati_status` | Renaissance: Ming Official Dress | East Asian Literati, Ming-facing, 1450-1600; official gate | court, examination office, diplomatic, or senior learned dress | `renaissance_ming_crosscollar_innerrobe`<br>`renaissance_ming_wide_drawstring_trousers`<br>`renaissance_ming_roundcollar_longrobe`<br>`renaissance_ming_fourcorner_scholarcap`<br>`renaissance_ming_cloth_courtboots` |
| `renaissance_outfit_japanese_ordinary` | Renaissance: Japanese Ordinary Travel Dress | Japanese, 1450-1600; local and status admission required | ordinary travel, rural, messenger, or household dress | `renaissance_japanese_smallsleeve_wraprobe`<br>`renaissance_japanese_field_trousers`<br>`renaissance_japanese_splittoe_socks`<br>`renaissance_japanese_wooden_clogs`<br>`renaissance_japanese_lacquered_conicalhat` |
| `renaissance_outfit_japanese_status` | Renaissance: Japanese Formal Dress | Japanese, 1450-1600; court/warrior/status gate | formal audience, court, senior retainer, or ceremonial dress | `renaissance_japanese_smallsleeve_wraprobe`<br>`renaissance_japanese_full_pleated_hakama`<br>`renaissance_japanese_shoulderwing_vest`<br>`renaissance_japanese_stiff_lacquered_eboshi`<br>`renaissance_japanese_wooden_clogs` |
| `renaissance_outfit_maritime_east_asian_ordinary` | Renaissance: Ryukyuan Ordinary Dress | Maritime East Asian, Ryukyuan-facing, 1450-1600 | ordinary port, household, island-court service, or market dress | `renaissance_ryukyu_widesleeve_summerrobe`<br>`renaissance_ryukyu_pleated_wrapskirt`<br>`renaissance_ryukyu_short_wrapjacket`<br>`renaissance_shared_clothing_cloth_headwrap`<br>`renaissance_shared_clothing_textile_sandals` |
| `renaissance_outfit_maritime_east_asian_status` | Renaissance: Ryukyuan Court Dress | Maritime East Asian, Ryukyuan-facing, 1450-1600; court gate | court, embassy, maritime office, or ceremonial dress | `renaissance_ryukyu_widesleeve_summerrobe`<br>`renaissance_ryukyu_pleated_wrapskirt`<br>`renaissance_ryukyu_short_wrapjacket`<br>`renaissance_ryukyu_formal_courtcap`<br>`renaissance_shared_clothing_fine_velvet_slippers` |
| `renaissance_outfit_seasia_mainland_ordinary` | Renaissance: South-east Asian Mainland Ordinary Dress | South-east Asian Mainland, 1450-1600; local admission required | household, market, rural, or artisan dress | `renaissance_shared_clothing_short_undershirt`<br>`renaissance_seasia_sewn_tubeskirt`<br>`renaissance_seasia_short_collarless_jacket`<br>`renaissance_seasia_palmleaf_conicalhat`<br>`renaissance_shared_clothing_textile_sandals` |
| `renaissance_outfit_seasia_mainland_status` | Renaissance: South-east Asian Mainland Court Dress | South-east Asian Mainland, 1450-1600; court/status gate | court, diplomatic, ceremonial, or elite household dress | `renaissance_seasia_pleated_courtwaistcloth`<br>`renaissance_seasia_long_crossfront_courtrobe`<br>`renaissance_seasia_sleeveless_courtvest`<br>`renaissance_seasia_structured_headclothcrown`<br>`renaissance_seasia_embroidered_silk_courtshoes` |
| `renaissance_outfit_maritime_seasia_ordinary` | Renaissance: Maritime South-east Asian Ordinary Dress | Maritime South-east Asian, 1450-1600; port or shipboard gate | port, coastal, shipboard, or market dress | `renaissance_seasia_short_maritime_trousers`<br>`renaissance_seasia_short_collarless_jacket`<br>`renaissance_shared_clothing_cloth_headwrap`<br>`renaissance_shared_clothing_rain_cape`<br>`renaissance_shared_clothing_textile_sandals` |
| `renaissance_outfit_maritime_seasia_status` | Renaissance: Maritime South-east Asian Court Dress | Maritime South-east Asian, 1450-1600; court/status gate | court, port office, diplomatic, or ceremonial dress | `renaissance_seasia_pleated_courtwaistcloth`<br>`renaissance_seasia_long_crossfront_courtrobe`<br>`renaissance_seasia_sleeveless_courtvest`<br>`renaissance_seasia_structured_headclothcrown`<br>`renaissance_seasia_embroidered_silk_courtshoes` |
| `renaissance_outfit_steppe_ordinary` | Renaissance: Steppe and Caravan Riding Dress | Steppe and Caravan, 1450-1600 | ordinary mounted, caravan, herding, or courier dress | `renaissance_shared_clothing_long_undershirt`<br>`renaissance_steppe_wide_ridingtrousers`<br>`renaissance_steppe_short_split_ridingcoat`<br>`renaissance_steppe_pointed_feltcap`<br>`renaissance_steppe_soft_highboots` |
| `renaissance_outfit_steppe_status` | Renaissance: Steppe and Caravan Status Dress | Steppe and Caravan, 1450-1600; status/ceremonial gate | senior retainer, court, diplomatic, or winter ceremonial dress | `renaissance_shared_clothing_long_undershirt`<br>`renaissance_steppe_wide_ridingtrousers`<br>`renaissance_shared_clothing_quilted_jacket`<br>`renaissance_steppe_sidefastened_furcoat`<br>`renaissance_shared_clothing_shoulder_shawl`<br>`renaissance_steppe_fur_earflaphat`<br>`renaissance_steppe_soft_highboots` |

### African, American, contact, and maritime manifests — 18

| Stable key | Template name | Admission | Purpose | Ordered item references |
| --- | --- | --- | --- | --- |
| `renaissance_outfit_african_court_atlantic_ordinary` | Renaissance: African Court Atlantic Ordinary Dress | African Court Atlantic, 1450-1600; narrower local gate required | ordinary household, market, artisan, or local court-service dress | `renaissance_shared_clothing_wrapped_loincloth`<br>`renaissance_africancourt_shortsleeve_tunic`<br>`renaissance_africancourt_broad_waistwrapper`<br>`renaissance_shared_clothing_cloth_headwrap`<br>`renaissance_shared_clothing_leather_sandals` |
| `renaissance_outfit_african_court_atlantic_status` | Renaissance: African Court Atlantic Status Dress | African Court Atlantic, 1450-1600; narrower court/status gate required | court, office, diplomatic, or ceremonial dress | `renaissance_africancourt_broad_waistwrapper`<br>`renaissance_africancourt_longsleeve_sideslit_tunic`<br>`renaissance_africancourt_broadsleeve_courtrobe`<br>`renaissance_africancourt_raffia_shouldercape`<br>`renaissance_africancourt_structured_crownheadcloth`<br>`renaissance_africancourt_raised_woodensandals` |
| `renaissance_outfit_sahelian_ordinary` | Renaissance: Sahelian Islamic Ordinary Dress | Sahelian Islamic, 1450-1600; narrower local gate required | ordinary urban, caravan, learned, merchant, or household dress | `renaissance_sahel_narrow_longtunic`<br>`renaissance_sahel_veryfull_trousers`<br>`renaissance_sahel_broad_rectangular_robe`<br>`renaissance_shared_clothing_soft_brimless_cap`<br>`renaissance_shared_clothing_leather_sandals` |
| `renaissance_outfit_sahelian_status` | Renaissance: Sahelian Islamic Scholar-Court Dress | Sahelian Islamic, 1450-1600; scholar/court gate | scholar, court, legal, diplomatic, or senior merchant dress | `renaissance_sahel_embroidered_necktunic`<br>`renaissance_sahel_veryfull_trousers`<br>`renaissance_sahel_broad_rectangular_robe`<br>`renaissance_sahel_scholar_turban`<br>`renaissance_sahel_soft_ridingboots` |
| `renaissance_outfit_red_sea_ordinary` | Renaissance: Red Sea Highland Ordinary Dress | Red Sea, 1450-1600; narrower local gate required | ordinary highland, road, household, or rural dress | `renaissance_redsea_narrow_cotton_tunic`<br>`renaissance_redsea_full_shoulderwrap`<br>`renaissance_redsea_white_headhood`<br>`renaissance_redsea_rawhide_sandals` |
| `renaissance_outfit_red_sea_status` | Renaissance: Red Sea Court-Religious Dress | Red Sea, 1450-1600; court or institution gate | court, church-associated, diplomatic, or ceremonial dress | `renaissance_redsea_long_courtshirt`<br>`renaissance_redsea_full_shoulderwrap`<br>`renaissance_redsea_embroidered_shouldercape`<br>`renaissance_redsea_quilted_white_ridinghood`<br>`renaissance_redsea_rawhide_sandals` |
| `renaissance_outfit_indian_ocean_ordinary` | Renaissance: Indian Ocean Port Ordinary Dress | Indian Ocean, 1450-1600; narrower port gate required | ordinary port, sailor, artisan, merchant, or household dress | `renaissance_indianocean_short_coastaltunic`<br>`renaissance_indianocean_sewn_wrapskirt`<br>`renaissance_indianocean_embroidered_roundcap`<br>`renaissance_indianocean_toeloop_sandals` |
| `renaissance_outfit_indian_ocean_status` | Renaissance: Indian Ocean Merchant Dress | Indian Ocean, 1450-1600; merchant/status gate | merchant, broker, scholar, diplomatic, or ceremonial dress | `renaissance_indianocean_long_coastalrobe`<br>`renaissance_indianocean_open_merchantcoat`<br>`renaissance_indianocean_port_turban`<br>`renaissance_indianocean_stitched_coastal_slippers` |
| `renaissance_outfit_mesoamerican_ordinary` | Renaissance: Mesoamerican Ordinary Dress | Mesoamerican, 1450-1600; narrower polity/community gate required | ordinary household, agricultural, market, or porter dress | `renaissance_mesoamerican_panelled_loincloth`<br>`renaissance_mesoamerican_shouldertied_cloak`<br>`renaissance_mesoamerican_woven_headcap`<br>`renaissance_mesoamerican_woven_fibresandals` |
| `renaissance_outfit_mesoamerican_status` | Renaissance: Mesoamerican Office-Ceremonial Dress | Mesoamerican, 1450-1600; office/ritual/court gate | office, court, diplomatic, ritual, or military-status dress | `renaissance_mesoamerican_full_ceremonialtunic`<br>`renaissance_mesoamerican_painted_barkclothcape`<br>`renaissance_mesoamerican_featheredge_crown`<br>`renaissance_mesoamerican_heelstrap_sandals` |
| `renaissance_outfit_andean_ordinary` | Renaissance: Andean Highland Ordinary Dress | Andean, 1450-1600; narrower polity/community gate required | ordinary highland, labour, messenger, or household dress | `renaissance_andean_straight_sleevelesstunic`<br>`renaissance_andean_fringed_camelidshawl`<br>`renaissance_andean_woven_earflapcap`<br>`renaissance_andean_braided_fibresandals` |
| `renaissance_outfit_andean_status` | Renaissance: Andean State-Ceremonial Dress | Andean, 1450-1600; state/office/ritual gate | state, court, office, diplomatic, or ceremonial dress | `renaissance_andean_fine_tapestrytunic`<br>`renaissance_andean_ceremonial_broadmantle`<br>`renaissance_andean_fringed_ceremonialapron`<br>`renaissance_andean_long_headcloth`<br>`renaissance_andean_rawhide_roadsandals` |
| `renaissance_outfit_caribbean_contact_ordinary` | Renaissance: Caribbean Local-Continuity Dress | Caribbean Contact, locally anchored pre-contact or contact date | local-continuity household, work, or travel dress | `renaissance_caribbean_cotton_loinapron`<br>`renaissance_caribbean_sleeveless_cottontunic`<br>`renaissance_caribbean_woven_shouldercape`<br>`renaissance_caribbean_woven_fibresandals` |
| `renaissance_outfit_caribbean_contact_status` | Renaissance: Caribbean Contact Status Dress | Caribbean Contact, explicit local/contact date; status gate | local status, diplomatic, ritual, dance, or contact-hybrid dress | `renaissance_caribbean_cotton_wrapskirt`<br>`renaissance_caribbean_sleeveless_cottontunic`<br>`renaissance_caribbean_woven_shouldercape`<br>`renaissance_shared_clothing_cloth_headwrap`<br>`renaissance_caribbean_gathered_rawhide_shoes` |
| `renaissance_outfit_colonial_atlantic_ordinary` | Renaissance: Colonial Atlantic Contact Dress | Colonial Atlantic, explicit post-contact date and production-status gate | locally made hybrid, trade-cloth, mission-labour, or port dress | `renaissance_contact_hybrid_lacedtunic`<br>`renaissance_contact_tradecloth_panelskirt`<br>`renaissance_contact_broadbrim_sunhood`<br>`renaissance_shared_clothing_low_leather_shoes` |
| `renaissance_outfit_colonial_atlantic_status` | Renaissance: Colonial Atlantic Mission Dress | Colonial Atlantic, explicit post-contact mission/institution gate | mission issue, adopted, imposed, or locally made institutional dress | `renaissance_shared_clothing_long_undershirt`<br>`renaissance_contact_mission_catechistrobe`<br>`renaissance_shared_clothing_close_cloth_cap`<br>`renaissance_shared_clothing_low_leather_shoes` |
| `renaissance_outfit_global_maritime_ordinary` | Renaissance: Global Maritime Deck Dress | Global Maritime, 1450-1600; crew-local admission required | ordinary sailor, fisher, dockworker, or cargo-work dress | `renaissance_maritime_sleeveless_decksmock`<br>`renaissance_maritime_loose_decktrousers`<br>`renaissance_maritime_knitted_waistcoat`<br>`renaissance_globalmaritime_knotted_headscarf`<br>`renaissance_shared_clothing_ropesole_deckshoes` |
| `renaissance_outfit_global_maritime_status` | Renaissance: Global Maritime Officer Dress | Global Maritime, 1450-1600; officer/pilot/merchant gate | officer, pilot, factor, merchant, or senior shipboard dress | `renaissance_western_open_neck_work_shirt`<br>`renaissance_maritime_loose_decktrousers`<br>`renaissance_maritime_canvas_watchcoat`<br>`renaissance_shared_clothing_leather_gloves`<br>`renaissance_maritime_tarred_brimhat`<br>`renaissance_maritime_tarred_highshoes` |

### Profession and institution overlay manifests — 11

| Stable key | Template name | Admission | Purpose | Ordered item references |
| --- | --- | --- | --- | --- |
| `renaissance_outfit_overlay_field_labourer` | Renaissance: Western Field Labourer | Western European Renaissance, 1450-1600; field-work gate | field labourer minimum family | `renaissance_shared_clothing_drawstring_drawers`<br>`renaissance_western_open_neck_work_shirt`<br>`renaissance_western_work_smock`<br>`renaissance_western_dress_apron`<br>`renaissance_shared_clothing_straw_brimmed_hat`<br>`renaissance_western_hobnailed_fieldshoes` |
| `renaissance_outfit_overlay_artisan` | Renaissance: Western Urban Artisan | Western European Renaissance, 1450-1600; guild/workshop gate | artisan minimum family | `renaissance_shared_clothing_drawstring_drawers`<br>`renaissance_western_open_neck_work_shirt`<br>`renaissance_western_venetian_breeches`<br>`renaissance_western_sleeveless_jerkin`<br>`renaissance_western_dress_apron`<br>`renaissance_shared_clothing_close_cloth_cap`<br>`renaissance_shared_clothing_low_leather_shoes` |
| `renaissance_outfit_overlay_merchant` | Renaissance: Indian Ocean Port Merchant | Indian Ocean, 1450-1600; merchant/broker gate | merchant minimum family | `renaissance_shared_clothing_short_undershirt`<br>`renaissance_indianocean_long_coastalrobe`<br>`renaissance_indianocean_open_merchantcoat`<br>`renaissance_shared_clothing_leather_gloves`<br>`renaissance_indianocean_port_turban`<br>`renaissance_indianocean_stitched_coastal_slippers` |
| `renaissance_outfit_overlay_scholar_notary` | Renaissance: Western Scholar-Notary | Western European Renaissance, 1450-1600; university/legal/office gate | scholar, notary, or administrator minimum family | `renaissance_shared_clothing_drawstring_drawers`<br>`renaissance_western_high_collar_shirt`<br>`renaissance_western_long_skirted_doublet`<br>`renaissance_institution_academic_robe`<br>`renaissance_institution_academic_squarecap`<br>`renaissance_western_fine_latchet_shoes` |
| `renaissance_outfit_overlay_printer` | Renaissance: Western Printer-Book Worker | Western European Renaissance, 1450-1600; print workshop gate | printer or book-worker minimum family | `renaissance_shared_clothing_drawstring_drawers`<br>`renaissance_western_open_neck_work_shirt`<br>`renaissance_western_venetian_breeches`<br>`renaissance_western_dress_apron`<br>`renaissance_shared_clothing_close_cloth_cap`<br>`renaissance_shared_clothing_low_leather_shoes` |
| `renaissance_outfit_overlay_sailor` | Renaissance: Global Maritime Sailor | Global Maritime, 1450-1600; crew-local gate | sailor or dockworker minimum family | `renaissance_maritime_sleeveless_decksmock`<br>`renaissance_maritime_loose_decktrousers`<br>`renaissance_maritime_knitted_waistcoat`<br>`renaissance_maritime_canvas_watchcoat`<br>`renaissance_globalmaritime_knotted_headscarf`<br>`renaissance_shared_clothing_ropesole_deckshoes` |
| `renaissance_outfit_overlay_apothecary` | Renaissance: Western Apothecary-Medical Worker | Western European Renaissance, 1450-1600; medical/guild gate | apothecary or medical-worker minimum family | `renaissance_western_long_shift`<br>`renaissance_western_loose_house_gown`<br>`renaissance_western_dress_apron`<br>`renaissance_shared_clothing_cloth_gloves`<br>`renaissance_institution_physicians_roundcap`<br>`renaissance_western_fine_latchet_shoes` |
| `renaissance_outfit_overlay_artist_musician` | Renaissance: Western Court Artist-Musician | Western European Renaissance, 1450-1600; court/performance gate | artist or musician minimum family | `renaissance_shared_clothing_drawstring_drawers`<br>`renaissance_western_gathered_linen_shirt`<br>`renaissance_western_venetian_breeches`<br>`renaissance_western_sleeveless_jerkin`<br>`renaissance_western_dress_apron`<br>`renaissance_western_feathered_velvet_bonnet`<br>`renaissance_western_fine_latchet_shoes` |
| `renaissance_outfit_overlay_actor_pageant` | Renaissance: Western Actor-Pageant Costume | Western European Renaissance, 1500-1600; performance/pageant gate | actor or pageant-participant minimum family | `renaissance_shared_clothing_short_undershirt`<br>`renaissance_western_paned_trunk_hose`<br>`renaissance_shared_clothing_detachable_sleeves`<br>`renaissance_shared_clothing_ceremonial_mantle`<br>`renaissance_shared_clothing_half_mask`<br>`renaissance_western_laced_buskins` |
| `renaissance_outfit_overlay_guard_servant` | Renaissance: Ottoman Court Guard-Servant | Ottoman Islamicate, 1450-1600; court/service-household gate | guard or court-servant minimum family | `renaissance_ottoman_collarless_inner_shirt`<br>`renaissance_ottoman_full_salwar_trousers`<br>`renaissance_ottoman_short_fitted_vest`<br>`renaissance_ottoman_short_cavalry_jacket`<br>`renaissance_ottoman_small_court_turban`<br>`renaissance_shared_clothing_ankle_boots` |
| `renaissance_outfit_overlay_religious_functionary` | Renaissance: Western Monastic Functionary | Western European Renaissance, 1450-1600; named monastic institution gate | culture-specific religious-functionary minimum family | `renaissance_shared_clothing_long_undershirt`<br>`renaissance_institution_plain_cassock`<br>`renaissance_institution_monastic_scapular`<br>`renaissance_institution_full_cowl`<br>`renaissance_shared_clothing_close_cloth_cap`<br>`renaissance_institution_closed_monastic_sandals` |

## Profession and institution overlays

Profession overlays are outfits and skins unless a garment has a distinct construction or function. Minimum authored outfit families:

| Overlay | Required slots | Typical catalogue sources |
| --- | --- | --- |
| field labourer | underlayer, shirt/tunic, lower garment, head protection, footwear, optional apron | common forms plus local culture volume |
| artisan | shirt/tunic, lower garment, work apron or over-vest, cap/headwrap, footwear | common forms; Western, Asian, African urban variants |
| merchant | underlayer, core garment, outer robe/coat, headwear, footwear, optional gloves | all urban culture groupings |
| scholar/notary/administrator | underlayer, long garment or fitted suit, outer layer, headwear, footwear | Western, Ottoman/Persianate, East Asian literati, African Islamic volumes |
| printer/book worker | shirt/tunic, lower garment, ink-marked apron, cap/headwrap, shoes | Western and East Asian print contexts |
| sailor/dock worker | shirt or short tunic, drawers/trousers/wrap, kerchief/headwrap, weather layer, footwear or bare-foot admission | Atlantic, Indian Ocean, East/South-east Asian and contact volumes |
| apothecary/medical worker | washable underlayer, gown/robe/coat, apron, close headwear, shoes | culture-specific urban forms; no modern white-coat default |
| artist/musician | ordinary status outfit plus tool-safe apron or performance skin | Western court/civic, Ottoman/Persianate, Asian court, African court |
| actor/pageant participant | ordinary underlayer plus mask, mantle, detachable sleeves, staged costume skins | institutional performance tag; theatrical inspiration in builder notes |
| guard/court servant | local ordinary dress with livery/status skins; outer coat/tabard only where silhouette differs | court and civic volumes |
| religious functionary | culture-specific long garment, mantle/vestment, headwear and footwear | cross-confessional institutional sections; never a generic universal priest outfit |

Outfits must fail closed when a required authored slot is missing. Shared slots must be explicit. A skin may not change the wearable component or convert a one-piece garment into a multi-piece outfit.

## Craft and production implications

The catalogue assumes `Tailoring`/`Textilecraft` as the principal skill family. Implementation should group crafts by construction rather than create one craft per cosmetic skin:

- cut-and-sewn linen/cotton underlayers;
- wool broadcloth garments;
- fitted doublet/bodice and structured Western work;
- long robe/kaftan/coat construction;
- wrapped and draped garments;
- quilting and padded clothing;
- felt and woven headwear;
- shoemaking, sandal making, and boot making;
- glove making;
- veil, lace, ribbon, and fine accessory work;
- regional bast-fibre, barkcloth, camelid, raffia, featherwork, and beadwork chains once materials resolve.

Crafts must consume exact material stock and exact tools. Decorative skins should not multiply base crafts unless the decoration has its own material input and skill gate.

## Implementation sequence

1. Seed or resolve culture and functional tags.
2. Audit live wearable components and add the blocking profiles from the dependency ledger.
3. Seed high-priority materials where prototypes cannot be represented honestly by live stock.
4. Implement the complete 471-prototype standalone catalogue beyond the 208-item outfit dependency union.
5. Extend regional item coverage in dependency order: common admissions first, then distinct silhouettes not already required by an outfit.
6. Add skins, beginning with plain/work, standard urban, luxury/court, religious/institutional, and imported/contact variants.
7. The 59 stock outfits are implemented; add shop and downstream culture/date manifests where games need automatic admission.
8. Add crafts only after exact skills, tools, materials, tags, and output references resolve.

## Acceptance criteria

- All 471 stable references are unique and every one has an explicit culture/date admission.
- The 41 first-pass shared placements reuse an existing stable reference rather than cloning it. The 76 expansion rows remain uncounted as placements until explicit admission.
- Every portable item has `Holdable`; every garment has a valid exact wearable and destroyable component.
- No item uses a material, tag, component, skill, or stable reference that is absent at implementation time.
- Public descriptions are form-based and do not universalise a local historical name.
- Skins do not change silhouette, component behaviour, coverage, capacity, or production technology.
- Contact and colonial rows identify imported, imposed, hybrid, or local-continuity status rather than presenting colonial systems as culturally neutral.
- Religious and ceremonial clothing is admitted through an institution and date context, not a generic universal category.
- Complete outfits fail closed on missing authored pieces and identify intentionally shared slots.
- The generated stock set contains exactly 59 unique outfit manifests and 202 unique item dependencies.
- All eleven profession/institution overlay families have a concrete culture- or institution-gated outfit.
- No North American contact stock outfit is created until a narrower regional design replaces the broad placeholder.
