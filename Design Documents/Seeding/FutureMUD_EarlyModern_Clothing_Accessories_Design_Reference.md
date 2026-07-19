# FutureMUD Early Modern Clothing and Accessories Design Reference

> First-pass civilian outfit and item catalogue. Full descriptions are intentionally deferred.

## Executive summary

- Master-table cultures covered: **35**. The `Global maritime and chartered-company trade` overlay is deliberately deferred.
- Complete civilian outfit manifests: **350** — **175 male** and **175 female**.
- Unique item references used: **317** across **2551 outfit placements**.
- Item-reference disposition: **133 new Early Modern prototypes**, **181 explicit Renaissance survivals/admissions**, and **3 implemented shared pre-industrial aliases**.
- Scope is limited to rural and urban commoners, artisans, market workers, merchants, clerks, learned professionals, and bourgeois or locally equivalent prosperous households.
- Military dress, uniforms, liveries, officer dress, court regalia, royal dress, aristocratic/noble dress, and the global-maritime overlay are outside this first pass.
- Every outfit is authored as a complete wearable set rather than a one- or two-piece suggestion. Every manifest contains an upper-body garment, a lower- or full-body garment, footwear, and headwear or a head covering.

## Scope and era model

- Era token: `EarlyModern`.
- Chronological band: approximately 1600-1750 CE, interpreted through the date and culture anchors in the Early Modern master reference rather than as one universal year.
- Public item wording remains form-based and in-world. Historical culture labels occur only in builder-facing headings and notes.
- Local names, colours, weave patterns, embroidery, trim, household marks, guild marks, maker marks, imported-textile presentation, and most status distinctions belong in skins.
- Culture-family entries are coverage buckets, not assertions that an entire region wore one uniform style. Broad entries such as West African, Indigenous North American, Mesoamerican, Andean, South-east Asian, and colonial/contact families require a narrower local manifest before shop or craft placement.
- Contact and colonial outfits must identify whether each adopted form is local continuity, imported, locally made from imported cloth, imposed, mission-associated, or genuinely hybrid. Clothing alone must not be used to infer legal status, ethnicity, enslavement, freedom, or cultural assimilation.

## Relationship to shared and earlier-era catalogues

- The implemented `preindustrial_clothing_*` belt and sash aliases are reused directly; they are not cloned under Early Modern references.
- Renaissance stable references are admitted only where the underlying construction remains credible after 1600. An implementation should ensure each admitted source row idempotently rather than invoking the entire Renaissance branch.
- A distinct `earlymodern_*` row is created where the silhouette, layering, primary material, component profile, production method, or culturally useful builder anchor changes.
- Cross-era reuse is an admission decision, not a claim of equal prevalence. Culture manifests, shops, crafts, professions, climate, and local world-building govern availability.

## First-pass authoring contract

All catalogue rows in this document use the following shared implementation assumptions unless a later implementation note overrides them:

- ordinary portable inventory item: `Holdable`;
- finished clothing damage profile: `Destroyable_Clothing`;
- exactly one wearable profile per item;
- one clothing armour profile and one insulation profile;
- an exact colour-variable component whenever `$colour`, `$colour1`, or `$colour2` appears in the sdesc;
- `skinnable = true`, `hideFromPlayers = false`, `ldesc = null`;
- no morph target, morph echo, morph timer, or destroyed-item reference;
- cost is denominated in farthings;
- no full description in this pass.

New Early Modern rows use `Era / Early Modern Era`. Reused Renaissance and pre-industrial references retain their source-era tag and are admitted by these outfit manifests. All market tags are exact live paths under `Market / Clothing`.

## Dependency ledger

Two new **Wearable component prototypes** are required: `Wear_Stays` and `Wear_Breeches`. No new component type and no new material are required. Their exact requirements, usage, and layering constraints are maintained in [FutureMUD_EarlyModern_Clothing_Accessories_Dependency_Ledger.md](./FutureMUD_EarlyModern_Clothing_Accessories_Dependency_Ledger.md). This first pass assumes both profiles will be seeded before item creation.

## Culture coverage and admission notes

| # | Master culture family | First-pass admission note |
|---:|---|---|
| 1 | French / Baroque court and urban | Civilian French-facing dress, c. 1650-1750. Court and noble display are excluded; the prosperous outfits represent master artisans, shopkeepers, clerks, and other urban bourgeois households. |
| 2 | Dutch Republic / Low Countries | Civilian Dutch and Low Countries dress, c. 1600-1720, emphasizing rural dairy and peat districts, urban crafts, market trade, and mercantile households without court or military clothing. |
| 3 | English / British Stuart-Georgian | Civilian English and British-facing dress, c. 1600-1750. The set covers agrarian work, small-town crafts, shops, and literate bourgeois occupations while omitting uniforms and aristocratic court dress. |
| 4 | Iberian / Portuguese-Spanish empires | Iberian civilian clothing, c. 1600-1750, covering agricultural, transport, artisanal, shopkeeping, and notarial settings. Colonial variants are handled in their own culture sections. |
| 5 | German / HRE / Austrian | Civilian central-European clothing, c. 1600-1750, with agrarian, guild, shop, and clerical-bourgeois ensembles. Regional skins should carry local town, confession, and textile distinctions. |
| 6 | Italian states | Italian-state civilian clothing, c. 1600-1750, emphasizing agricultural districts, city crafts, shops, and literate commercial households rather than court pageantry. |
| 7 | Scandinavian / Baltic | Civilian Scandinavian and Baltic dress, c. 1600-1750, covering farming, fishing communities, town crafts, port commerce, and literate households. Naval uniforms are explicitly excluded. |
| 8 | Polish-Lithuanian / Hungarian frontier | Civilian eastern-central European frontier dress, c. 1600-1750. Riding-capable forms are used for travel and herding, not as military uniforms; noble and magnate display is omitted. |
| 9 | Russian / Petrine and post-Petrine | Muscovite and early-imperial Russian civilian clothing, c. 1600-1750. The manifests deliberately include both long-lived local forms and selective urban westernising dress without making either universal. |
| 10 | Ottoman | Ottoman civilian dress, c. 1600-1750, ranging from provincial agricultural work to urban craft, retail, scribal, and prosperous household settings. Rank kaftans, command dress, and military uniforms are excluded. |
| 11 | Maghrebi / North African | Maghrebi civilian dress, c. 1600-1750, covering agrarian, pastoral, artisanal, market, mercantile, and learned urban contexts. Corsair and military dress are outside this pass. |
| 12 | Safavid / post-Safavid Persianate | Persianate civilian clothing, c. 1600-1750, with rural, caravan-adjacent, craft, mercantile, and scribal ensembles. Court robes and military command dress are excluded. |
| 13 | Mughal / Indo-Persian | Mughal and Indo-Persian civilian clothing, c. 1600-1750, spanning cultivators, artisans, shopkeepers, scribes, merchants, textile workers, and prosperous urban households without court regalia. |
| 14 | Maratha / Rajput / Deccan | Regional South Asian civilian dress, c. 1650-1750, emphasizing agricultural, textile, market, mercantile, and administrative settings. Warrior and courtly dress are deliberately omitted despite the master culture label. |
| 15 | South Indian / coastal trade | South Indian and coastal civilian dress, c. 1600-1750, covering agrarian, weaving, fishing-port, retail, and bookkeeping contexts. Court and military attire are excluded. |
| 16 | Qing China | Early Qing civilian clothing, c. 1644-1750, for farmers, artisans, shopkeepers, clerks, textile workers, and prosperous merchant households. Official rank dress and banner military uniforms are excluded. |
| 17 | Late Ming survival / transition | Late Ming and transition-period civilian forms, c. 1600-1650, retained only where the silhouette remains a distinct and plausible local continuity. Official and court dress are excluded. |
| 18 | Joseon Korea | Joseon civilian clothing, c. 1600-1750, covering farming, household, craft, market, shop, and literate occupations. Court rank garments and military clothing are excluded. |
| 19 | Edo Japan | Edo-period civilian clothing, c. 1600-1750, emphasizing farmers, artisans, shopkeepers, clerks, market workers, and merchant households. Samurai formalwear and military dress are excluded. |
| 20 | Ryukyu and maritime East Asia | Ryukyuan and maritime East Asian civilian clothing, c. 1600-1750, with agricultural, fishing, artisanal, mercantile, and clerical households. Tribute-court regalia is excluded. |
| 21 | Mainland South-east Asian courts | Mainland South-east Asian civilian clothing, c. 1600-1750, stripped of court regalia despite the master label. The outfits cover rice cultivation, river work, crafts, markets, shops, and literate urban households. |
| 22 | Maritime South-east Asian trade worlds | Maritime South-east Asian civilian clothing, c. 1600-1750, centered on spice cultivation, fishing, port crafts, retail, and mercantile households. Military and dynastic court clothing are excluded. |
| 23 | Inner Asian / steppe frontier | Inner Asian and steppe-frontier civilian dress, c. 1600-1750, covering pastoral, caravan, felt-working, market, and merchant households. Military and princely attire are excluded. |
| 24 | West African court and Atlantic trade | West African civilian and commercial clothing, c. 1600-1750. The master label includes court networks, but this pass retains only rural, artisan, market, merchant, and prosperous household dress; every implementation still requires a narrower local culture. |
| 25 | Kongo / Angola / West Central Africa | West Central African civilian clothing, c. 1600-1750, deliberately distinguishing local barkcloth and wrapper continuities from adopted trade-cloth forms. Legal status is not inferred from clothing; local manifests must record it separately. |
| 26 | Sahelian / Hausa / Islamic West Africa | Sahelian and Hausa-facing civilian clothing, c. 1600-1750, covering cultivation, herding, urban crafts, market exchange, scholarship, and prosperous merchant households. Every outfit requires narrower local admission. |
| 27 | Ethiopian / Red Sea | Ethiopian highland and Red Sea civilian clothing, c. 1600-1750, spanning cultivators, herders, artisans, merchants, scribes, and prosperous households. Ecclesiastical and military attire are outside this pass. |
| 28 | Swahili Coast / Indian Ocean Africa | Swahili and Indian Ocean African civilian clothing, c. 1600-1750, covering coastal agriculture, fishing, port crafts, retail, scholarship, and merchant households. Court and military clothing are excluded. |
| 29 | Spanish colonial Americas | Spanish-colonial civilian clothing, c. 1600-1750. The manifests mix local Indigenous continuities, locally made Iberian forms, and genuine hybrid garments; each implementation must identify region, community, and whether a form is local, imported, imposed, or hybrid. |
| 30 | Portuguese Brazil / Atlantic plantation | Brazilian and Portuguese-Atlantic civilian clothing, c. 1600-1750, with local, African-diasporic, Indigenous, and Portuguese-derived forms. Clothing does not encode free, enslaved, Indigenous, African, European, or mixed legal identity; local manifests must record those contexts explicitly. |
| 31 | English / French / Dutch colonial North America | Colonial North American civilian dress, c. 1600-1750, for farming, frontier labour, crafts, shops, clerical work, and merchant households. Imported, locally made, and Indigenous-derived pieces require explicit local admission. |
| 32 | Indigenous North American regional families | A deliberately broad placeholder for region-specific Indigenous North American catalogues, c. 1600-1750. Every outfit must be narrowed by local culture, ecology, season, and contact history before implementation; this set avoids treating the continent as one dress system. |
| 33 | Mesoamerican colonial and Indigenous | Mesoamerican colonial and Indigenous civilian clothing, c. 1600-1750. Local continuities, Spanish-derived pieces, mission-associated dress, and genuine hybrids are separated through outfit notes rather than presented as a single linear progression. |
| 34 | Andean colonial and Indigenous | Andean colonial and Indigenous civilian clothing, c. 1600-1750, pairing camelid-textile continuities with locally adopted colonial forms. Each implementation must record community, altitude, season, and contact context. |
| 35 | Caribbean / Atlantic plantation | Caribbean and Atlantic plantation-zone civilian clothing, c. 1600-1750, including local Caribbean, African-diasporic, and European-derived forms. Clothing does not encode legal status; local manifests must explicitly record free, enslaved, Indigenous, African, European, and mixed contexts. |

## Outfit manifests

The following manifests are builder-facing. They are ordered one outfit at a time and reference only items listed in the catalogue below. Repeated stable references are intentional reuse, not missing rows.

### 1. French / Baroque court and urban

> Civilian French-facing dress, c. 1650-1750. Court and noble display are excluded; the prosperous outfits represent master artisans, shopkeepers, clerks, and other urban bourgeois households.

#### Outfit 1 — French rural field labourer male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_broadcloth_work_coat` — a $colour broadcloth work coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `earlymodern_western_clothing_knitted_wool_stockings` — a pair of $colour knitted wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `renaissance_shared_clothing_soft_brimless_cap` — a soft $colour brimless cap

#### Outfit 2 — French prosperous rural vintner male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat
- `earlymodern_western_clothing_full_wool_cloak` — a full $colour wool cloak

#### Outfit 3 — French urban journeyman artisan male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `earlymodern_western_clothing_knitted_wool_stockings` — a pair of $colour knitted wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth
- `earlymodern_western_clothing_linen_day_cap` — a $colour linen day cap

#### Outfit 4 — French urban master tradesman male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_french_clothing_long_skirted_coat` — a long-skirted $colour broadcloth coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_western_clothing_cocked_felt_hat` — a $colour cocked felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 5 — French bourgeois clerk male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_western_clothing_long_broadcloth_coat` — a long $colour broadcloth coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_western_clothing_cocked_felt_hat` — a $colour cocked felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 6 — French rural field worker female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_western_clothing_linen_day_cap` — a $colour linen day cap
- `earlymodern_western_clothing_full_wool_cloak` — a full $colour wool cloak

#### Outfit 7 — French prosperous rural householder female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_british_clothing_checked_linen_apron` — a $colour1 and $colour2 checked linen apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_western_clothing_linen_day_cap` — a $colour linen day cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `earlymodern_western_clothing_full_wool_cloak` — a full $colour wool cloak

#### Outfit 8 — French urban seamstress female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_french_clothing_short_casaquin_jacket` — a short $colour flared jacket
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `earlymodern_french_clothing_lace_trimmed_day_cap` — a lace-trimmed linen day cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief

#### Outfit 9 — French urban shopkeeper female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_plain_mantua_gown` — a plain $colour mantua gown
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `earlymodern_french_clothing_lace_trimmed_day_cap` — a lace-trimmed linen day cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `earlymodern_western_clothing_full_wool_cloak` — a full $colour wool cloak

#### Outfit 10 — French bourgeois household manager female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_plain_mantua_gown` — a plain $colour mantua gown
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_western_clothing_plain_leather_mules` — a pair of plain leather mules
- `earlymodern_french_clothing_lace_trimmed_day_cap` — a lace-trimmed linen day cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 2. Dutch Republic / Low Countries

> Civilian Dutch and Low Countries dress, c. 1600-1720, emphasizing rural dairy and peat districts, urban crafts, market trade, and mercantile households without court or military clothing.

#### Outfit 11 — Dutch rural peat cutter male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_dutch_clothing_short_wool_work_jacket` — a short $colour wool work jacket
- `earlymodern_dutch_clothing_full_canvas_breeches` — a pair of full $colour canvas breeches
- `earlymodern_western_clothing_knitted_wool_stockings` — a pair of $colour knitted wool stockings
- `earlymodern_dutch_clothing_wooden_clogs` — a pair of plain wooden clogs
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `earlymodern_dutch_clothing_white_linen_cap` — a crisp white linen cap

#### Outfit 12 — Dutch prosperous dairy farmer male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_dutch_clothing_short_wool_work_jacket` — a short $colour wool work jacket
- `earlymodern_dutch_clothing_full_canvas_breeches` — a pair of full $colour canvas breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat

#### Outfit 13 — Dutch urban guild artisan male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_dutch_clothing_full_canvas_breeches` — a pair of full $colour canvas breeches
- `earlymodern_western_clothing_knitted_wool_stockings` — a pair of $colour knitted wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_dutch_clothing_white_linen_cap` — a crisp white linen cap
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 14 — Dutch urban shopkeeper male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_western_clothing_long_broadcloth_coat` — a long $colour broadcloth coat
- `earlymodern_dutch_clothing_full_canvas_breeches` — a pair of full $colour canvas breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 15 — Dutch mercantile clerk male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_french_clothing_long_skirted_coat` — a long-skirted $colour broadcloth coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_western_clothing_cocked_felt_hat` — a $colour cocked felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 16 — Dutch rural dairy worker female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `earlymodern_dutch_clothing_striped_wool_petticoat` — a $colour1 and $colour2 striped wool petticoat
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `earlymodern_dutch_clothing_wooden_clogs` — a pair of plain wooden clogs
- `earlymodern_dutch_clothing_white_linen_cap` — a crisp white linen cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief

#### Outfit 17 — Dutch prosperous rural householder female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_dutch_clothing_short_wool_overjacket` — a $colour short wool over-jacket
- `earlymodern_dutch_clothing_striped_wool_petticoat` — a $colour1 and $colour2 striped wool petticoat
- `earlymodern_british_clothing_checked_linen_apron` — a $colour1 and $colour2 checked linen apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_dutch_clothing_white_linen_cap` — a crisp white linen cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `earlymodern_western_clothing_full_wool_cloak` — a full $colour wool cloak

#### Outfit 18 — Dutch urban market vendor female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `earlymodern_dutch_clothing_striped_wool_petticoat` — a $colour1 and $colour2 striped wool petticoat
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `earlymodern_dutch_clothing_wooden_clogs` — a pair of plain wooden clogs
- `earlymodern_dutch_clothing_white_linen_cap` — a crisp white linen cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `renaissance_shared_clothing_neck_kerchief` — a tied $colour linen neck kerchief

#### Outfit 19 — Dutch urban craftswoman female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_dutch_clothing_short_wool_overjacket` — a $colour short wool over-jacket
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_dutch_clothing_white_linen_cap` — a crisp white linen cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief

#### Outfit 20 — Dutch burgher merchant household female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_plain_mantua_gown` — a plain $colour mantua gown
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `earlymodern_dutch_clothing_white_linen_cap` — a crisp white linen cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 3. English / British Stuart-Georgian

> Civilian English and British-facing dress, c. 1600-1750. The set covers agrarian work, small-town crafts, shops, and literate bourgeois occupations while omitting uniforms and aristocratic court dress.

#### Outfit 21 — English rural field labourer male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_british_clothing_linen_smock_frock` — a loose $colour linen smock-frock
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `earlymodern_western_clothing_knitted_wool_stockings` — a pair of $colour knitted wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `renaissance_shared_clothing_soft_brimless_cap` — a soft $colour brimless cap

#### Outfit 22 — English prosperous tenant farmer male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_western_clothing_broadcloth_work_coat` — a $colour broadcloth work coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat

#### Outfit 23 — English urban journeyman artisan male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `earlymodern_western_clothing_knitted_wool_stockings` — a pair of $colour knitted wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_western_clothing_linen_day_cap` — a $colour linen day cap
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 24 — English urban shopkeeper male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_western_clothing_long_broadcloth_coat` — a long $colour broadcloth coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_western_clothing_cocked_felt_hat` — a $colour cocked felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 25 — English bourgeois scrivener male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_french_clothing_long_skirted_coat` — a long-skirted $colour broadcloth coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 26 — English rural field worker female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_british_clothing_linen_mob_cap` — a gathered $colour linen mob cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief

#### Outfit 27 — English prosperous farm householder female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_british_clothing_wool_shortgown` — a $colour wool shortgown
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_british_clothing_checked_linen_apron` — a $colour1 and $colour2 checked linen apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_british_clothing_linen_mob_cap` — a gathered $colour linen mob cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `earlymodern_western_clothing_full_wool_cloak` — a full $colour wool cloak

#### Outfit 28 — English urban domestic worker female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_british_clothing_wool_shortgown` — a $colour wool shortgown
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_british_clothing_linen_mob_cap` — a gathered $colour linen mob cap
- `renaissance_shared_clothing_neck_kerchief` — a tied $colour linen neck kerchief

#### Outfit 29 — English urban market trader female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_british_clothing_checked_linen_apron` — a $colour1 and $colour2 checked linen apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_western_clothing_wool_bonnet` — a $colour wool bonnet
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `earlymodern_western_clothing_full_wool_cloak` — a full $colour wool cloak

#### Outfit 30 — English bourgeois shopkeeping female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_plain_mantua_gown` — a plain $colour mantua gown
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `earlymodern_british_clothing_linen_mob_cap` — a gathered $colour linen mob cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 4. Iberian / Portuguese-Spanish empires

> Iberian civilian clothing, c. 1600-1750, covering agricultural, transport, artisanal, shopkeeping, and notarial settings. Colonial variants are handled in their own culture sections.

#### Outfit 31 — Iberian rural field labourer male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_iberian_clothing_short_fitted_jacket` — a short fitted $colour wool jacket
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_iberian_clothing_woven_fibre_sandals` — a pair of $colour woven fibre sandals
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `earlymodern_iberian_clothing_broad_crowned_felt_hat` — a broad-crowned $colour felt hat

#### Outfit 32 — Iberian rural muleteer male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_iberian_clothing_short_fitted_jacket` — a short fitted $colour wool jacket
- `renaissance_shared_clothing_straight_trousers` — a pair of $colour straight wool trousers
- `renaissance_shared_clothing_ankle_boots` — a pair of ankle leather boots
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_iberian_clothing_broad_crowned_felt_hat` — a broad-crowned $colour felt hat
- `earlymodern_iberian_clothing_short_wool_cape` — a short $colour wool cape

#### Outfit 33 — Iberian urban leatherworker male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_iberian_clothing_broad_crowned_felt_hat` — a broad-crowned $colour felt hat
- `renaissance_shared_clothing_neck_kerchief` — a tied $colour linen neck kerchief

#### Outfit 34 — Iberian urban shopkeeper male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_western_clothing_long_broadcloth_coat` — a long $colour broadcloth coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_iberian_clothing_broad_crowned_felt_hat` — a broad-crowned $colour felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 35 — Iberian bourgeois notary male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_italian_clothing_light_short_coat` — a light $colour short coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 36 — Iberian rural harvest worker female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `earlymodern_iberian_clothing_woven_fibre_sandals` — a pair of $colour woven fibre sandals
- `earlymodern_iberian_clothing_lace_head_veil` — a lace-edged $colour head veil
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief

#### Outfit 37 — Iberian prosperous rural householder female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_british_clothing_checked_linen_apron` — a $colour1 and $colour2 checked linen apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_iberian_clothing_lace_head_veil` — a lace-edged $colour head veil
- `earlymodern_italian_clothing_fine_silk_shawl` — a fine $colour silk shawl

#### Outfit 38 — Iberian urban market vendor female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_iberian_clothing_lace_head_veil` — a lace-edged $colour head veil
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief

#### Outfit 39 — Iberian urban craftswoman female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_italian_clothing_light_short_coat` — a light $colour short coat
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `earlymodern_iberian_clothing_lace_head_veil` — a lace-edged $colour head veil
- `earlymodern_italian_clothing_fine_silk_shawl` — a fine $colour silk shawl

#### Outfit 40 — Iberian bourgeois merchant household female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_plain_mantua_gown` — a plain $colour mantua gown
- `earlymodern_italian_clothing_striped_cotton_petticoat` — a $colour1 and $colour2 striped cotton petticoat
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_italian_clothing_soft_backless_shoes` — a pair of soft backless leather shoes
- `earlymodern_iberian_clothing_lace_head_veil` — a lace-edged $colour head veil
- `earlymodern_italian_clothing_fine_silk_shawl` — a fine $colour silk shawl
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 5. German / HRE / Austrian

> Civilian central-European clothing, c. 1600-1750, with agrarian, guild, shop, and clerical-bourgeois ensembles. Regional skins should carry local town, confession, and textile distinctions.

#### Outfit 41 — Central European rural farmhand male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_centraleuropean_clothing_short_wool_coat` — a short $colour wool coat
- `earlymodern_centraleuropean_clothing_leather_knee_breeches` — a pair of leather knee breeches
- `earlymodern_western_clothing_knitted_wool_stockings` — a pair of $colour knitted wool stockings
- `renaissance_shared_clothing_ankle_boots` — a pair of ankle leather boots
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `earlymodern_centraleuropean_clothing_round_felt_hat` — a round-crowned $colour felt hat

#### Outfit 42 — Central European prosperous farmer male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_centraleuropean_clothing_short_wool_coat` — a short $colour wool coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_centraleuropean_clothing_round_felt_hat` — a round-crowned $colour felt hat

#### Outfit 43 — Central European urban guild artisan male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_centraleuropean_clothing_leather_knee_breeches` — a pair of leather knee breeches
- `earlymodern_western_clothing_knitted_wool_stockings` — a pair of $colour knitted wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_soft_brimless_cap` — a soft $colour brimless cap
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 44 — Central European urban shopkeeper male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_western_clothing_long_broadcloth_coat` — a long $colour broadcloth coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_centraleuropean_clothing_round_felt_hat` — a round-crowned $colour felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 45 — Central European bourgeois clerk male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `renaissance_frontier_close_buttoned_longcoat` — a close-buttoned $colour long coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_centraleuropean_clothing_round_felt_hat` — a round-crowned $colour felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 46 — Central European rural field worker female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_centraleuropean_clothing_laced_wool_bodice` — a laced $colour wool bodice
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_western_clothing_linen_day_cap` — a $colour linen day cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief

#### Outfit 47 — Central European prosperous rural householder female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_centraleuropean_clothing_laced_wool_bodice` — a laced $colour wool bodice
- `renaissance_shared_clothing_gathered_skirt` — a gathered $colour wool skirt
- `earlymodern_british_clothing_checked_linen_apron` — a $colour1 and $colour2 checked linen apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_western_clothing_wool_bonnet` — a $colour wool bonnet
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `earlymodern_western_clothing_full_wool_cloak` — a full $colour wool cloak

#### Outfit 48 — Central European urban workshop woman female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_centraleuropean_clothing_laced_wool_bodice` — a laced $colour wool bodice
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_western_clothing_linen_day_cap` — a $colour linen day cap
- `renaissance_shared_clothing_neck_kerchief` — a tied $colour linen neck kerchief

#### Outfit 49 — Central European urban shopkeeper female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_centraleuropean_clothing_laced_wool_bodice` — a laced $colour wool bodice
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `earlymodern_western_clothing_wool_bonnet` — a $colour wool bonnet
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `earlymodern_western_clothing_full_wool_cloak` — a full $colour wool cloak

#### Outfit 50 — Central European bourgeois household female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_plain_mantua_gown` — a plain $colour mantua gown
- `earlymodern_western_clothing_full_wool_petticoat` — a full $colour wool petticoat
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `earlymodern_western_clothing_wool_bonnet` — a $colour wool bonnet
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 6. Italian states

> Italian-state civilian clothing, c. 1600-1750, emphasizing agricultural districts, city crafts, shops, and literate commercial households rather than court pageantry.

#### Outfit 51 — Italian rural vineyard worker male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_broadcloth_work_coat` — a $colour broadcloth work coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_iberian_clothing_woven_fibre_sandals` — a pair of $colour woven fibre sandals
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `earlymodern_iberian_clothing_broad_crowned_felt_hat` — a broad-crowned $colour felt hat

#### Outfit 52 — Italian prosperous tenant farmer male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_italian_clothing_light_short_coat` — a light $colour short coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat

#### Outfit 53 — Italian urban artisan male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `earlymodern_western_clothing_knitted_wool_stockings` — a pair of $colour knitted wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_soft_brimless_cap` — a soft $colour brimless cap
- `renaissance_shared_clothing_neck_kerchief` — a tied $colour linen neck kerchief

#### Outfit 54 — Italian urban shopkeeper male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_italian_clothing_light_short_coat` — a light $colour short coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_italian_clothing_soft_backless_shoes` — a pair of soft backless leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 55 — Italian bourgeois bookkeeper male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_french_clothing_long_skirted_coat` — a long-skirted $colour broadcloth coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_italian_clothing_soft_backless_shoes` — a pair of soft backless leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 56 — Italian rural harvest worker female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `earlymodern_italian_clothing_striped_cotton_petticoat` — a $colour1 and $colour2 striped cotton petticoat
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `earlymodern_iberian_clothing_woven_fibre_sandals` — a pair of $colour woven fibre sandals
- `earlymodern_italian_clothing_linen_headcloth` — a $colour linen headcloth
- `earlymodern_italian_clothing_fine_silk_shawl` — a fine $colour silk shawl

#### Outfit 57 — Italian prosperous rural householder female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `earlymodern_italian_clothing_striped_cotton_petticoat` — a $colour1 and $colour2 striped cotton petticoat
- `earlymodern_british_clothing_checked_linen_apron` — a $colour1 and $colour2 checked linen apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_italian_clothing_linen_headcloth` — a $colour linen headcloth
- `earlymodern_italian_clothing_fine_silk_shawl` — a fine $colour silk shawl

#### Outfit 58 — Italian urban silk worker female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_italian_clothing_light_short_coat` — a light $colour short coat
- `earlymodern_italian_clothing_striped_cotton_petticoat` — a $colour1 and $colour2 striped cotton petticoat
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_italian_clothing_soft_backless_shoes` — a pair of soft backless leather shoes
- `earlymodern_italian_clothing_linen_headcloth` — a $colour linen headcloth
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief

#### Outfit 59 — Italian urban market shopkeeper female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_plain_mantua_gown` — a plain $colour mantua gown
- `earlymodern_italian_clothing_striped_cotton_petticoat` — a $colour1 and $colour2 striped cotton petticoat
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_italian_clothing_soft_backless_shoes` — a pair of soft backless leather shoes
- `earlymodern_italian_clothing_linen_headcloth` — a $colour linen headcloth
- `earlymodern_italian_clothing_fine_silk_shawl` — a fine $colour silk shawl

#### Outfit 60 — Italian bourgeois merchant household female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_plain_mantua_gown` — a plain $colour mantua gown
- `earlymodern_italian_clothing_striped_cotton_petticoat` — a $colour1 and $colour2 striped cotton petticoat
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_italian_clothing_soft_backless_shoes` — a pair of soft backless leather shoes
- `renaissance_shared_clothing_short_head_veil` — a short $colour head veil
- `earlymodern_italian_clothing_fine_silk_shawl` — a fine $colour silk shawl
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 7. Scandinavian / Baltic

> Civilian Scandinavian and Baltic dress, c. 1600-1750, covering farming, fishing communities, town crafts, port commerce, and literate households. Naval uniforms are explicitly excluded.

#### Outfit 61 — Northern rural farmhand male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_northern_clothing_fitted_wool_jacket` — a fitted $colour wool jacket
- `earlymodern_northern_clothing_wool_knee_breeches` — a pair of $colour wool knee breeches
- `earlymodern_western_clothing_knitted_wool_stockings` — a pair of $colour knitted wool stockings
- `renaissance_shared_clothing_ankle_boots` — a pair of ankle leather boots
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `earlymodern_northern_clothing_knitted_wool_cap` — a $colour knitted wool cap

#### Outfit 62 — Northern coastal fisherman male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_northern_clothing_fitted_wool_jacket` — a fitted $colour wool jacket
- `renaissance_shared_clothing_straight_trousers` — a pair of $colour straight wool trousers
- `renaissance_shared_clothing_footwraps` — a pair of $colour cloth footwraps
- `renaissance_shared_clothing_high_riding_boots` — a pair of high leather boots
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `renaissance_frontier_knitted_northern_cap` — a long $colour knitted wool cap
- `renaissance_shared_clothing_rain_cape` — a close-woven $colour rain cape

#### Outfit 63 — Northern prosperous farmer male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_northern_clothing_fitted_wool_jacket` — a fitted $colour wool jacket
- `earlymodern_northern_clothing_wool_knee_breeches` — a pair of $colour wool knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_northern_clothing_knitted_wool_cap` — a $colour knitted wool cap

#### Outfit 64 — Baltic urban artisan male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `earlymodern_western_clothing_knitted_wool_stockings` — a pair of $colour knitted wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_frontier_knitted_northern_cap` — a long $colour knitted wool cap
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 65 — Baltic bourgeois merchant male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_western_clothing_long_broadcloth_coat` — a long $colour broadcloth coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 66 — Northern rural dairy worker female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `earlymodern_northern_clothing_striped_wool_skirt` — a $colour1 and $colour2 striped wool skirt
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_ankle_boots` — a pair of ankle leather boots
- `earlymodern_northern_clothing_knitted_wool_cap` — a $colour knitted wool cap
- `earlymodern_northern_clothing_wool_mittens` — a pair of $colour wool mittens

#### Outfit 67 — Northern prosperous rural householder female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `earlymodern_northern_clothing_striped_wool_skirt` — a $colour1 and $colour2 striped wool skirt
- `earlymodern_british_clothing_checked_linen_apron` — a $colour1 and $colour2 checked linen apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_northern_clothing_knitted_wool_cap` — a $colour knitted wool cap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl
- `earlymodern_northern_clothing_fur_lined_wool_coat` — a $colour fur-lined wool coat

#### Outfit 68 — Baltic urban market woman female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `renaissance_shared_clothing_gathered_skirt` — a gathered $colour wool skirt
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `renaissance_frontier_knitted_northern_cap` — a long $colour knitted wool cap
- `renaissance_shared_clothing_neck_kerchief` — a tied $colour linen neck kerchief

#### Outfit 69 — Baltic urban craftswoman female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_british_clothing_wool_shortgown` — a $colour wool shortgown
- `earlymodern_northern_clothing_striped_wool_skirt` — a $colour1 and $colour2 striped wool skirt
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_northern_clothing_knitted_wool_cap` — a $colour knitted wool cap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 70 — Baltic bourgeois household female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_plain_mantua_gown` — a plain $colour mantua gown
- `earlymodern_northern_clothing_striped_wool_skirt` — a $colour1 and $colour2 striped wool skirt
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `earlymodern_northern_clothing_knitted_wool_cap` — a $colour knitted wool cap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 8. Polish-Lithuanian / Hungarian frontier

> Civilian eastern-central European frontier dress, c. 1600-1750. Riding-capable forms are used for travel and herding, not as military uniforms; noble and magnate display is omitted.

#### Outfit 71 — Frontier rural herder male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_frontier_wide_hungarian_trousers` — a pair of wide $colour wool trousers
- `renaissance_frontier_close_buttoned_longcoat` — a close-buttoned $colour long coat
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_frontier_felted_winter_boots` — a pair of felted winter boots
- `renaissance_frontier_fur_brimmed_cap` — a soft $colour fur-brimmed cap

#### Outfit 72 — Frontier prosperous smallholder male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_shared_clothing_straight_trousers` — a pair of $colour straight wool trousers
- `renaissance_frontier_belted_wool_caftan` — a belted $colour wool caftan
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_frontier_split_skirt_riding_boots` — a pair of stiff high leather boots
- `renaissance_frontier_fur_brimmed_cap` — a soft $colour fur-brimmed cap
- `renaissance_shared_clothing_rectangular_mantle` — a $colour rectangular shoulder mantle

#### Outfit 73 — Frontier town artisan male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `renaissance_frontier_close_buttoned_longcoat` — a close-buttoned $colour long coat
- `renaissance_frontier_wide_hungarian_trousers` — a pair of wide $colour wool trousers
- `renaissance_shared_clothing_ankle_boots` — a pair of ankle leather boots
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_centraleuropean_clothing_round_felt_hat` — a round-crowned $colour felt hat

#### Outfit 74 — Frontier urban merchant male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `renaissance_frontier_belted_wool_caftan` — a belted $colour wool caftan
- `renaissance_frontier_wide_hungarian_trousers` — a pair of wide $colour wool trousers
- `renaissance_frontier_split_skirt_riding_boots` — a pair of stiff high leather boots
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_frontier_fur_brimmed_cap` — a soft $colour fur-brimmed cap
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

#### Outfit 75 — Frontier bourgeois clerk male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `renaissance_frontier_close_buttoned_longcoat` — a close-buttoned $colour long coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_centraleuropean_clothing_round_felt_hat` — a round-crowned $colour felt hat

#### Outfit 76 — Frontier rural field worker female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `renaissance_shared_clothing_gathered_skirt` — a gathered $colour wool skirt
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_ankle_boots` — a pair of ankle leather boots
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 77 — Frontier prosperous rural householder female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_frontier_sleeveless_long_gown` — a sleeveless $colour long overgown
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_ankle_boots` — a pair of ankle leather boots
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl
- `renaissance_shared_clothing_mittens` — a pair of $colour wool mittens

#### Outfit 78 — Frontier town market woman female

- `earlymodern_western_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_fitted_wool_bodice` — a fitted $colour wool bodice
- `renaissance_shared_clothing_gathered_skirt` — a gathered $colour wool skirt
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `renaissance_shared_clothing_short_head_veil` — a short $colour head veil
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 79 — Frontier urban craftswoman female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_frontier_sleeveless_long_gown` — a sleeveless $colour long overgown
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `renaissance_shared_clothing_short_head_veil` — a short $colour head veil
- `renaissance_shared_clothing_travelling_coat` — a loose $colour travelling coat

#### Outfit 80 — Frontier prosperous townswoman female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_frontier_sleeveless_long_gown` — a sleeveless $colour long overgown
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_frontier_felted_winter_boots` — a pair of felted winter boots
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `renaissance_shared_clothing_fur_lined_coat` — a $colour fur-lined travelling coat
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 9. Russian / Petrine and post-Petrine

> Muscovite and early-imperial Russian civilian clothing, c. 1600-1750. The manifests deliberately include both long-lived local forms and selective urban westernising dress without making either universal.

#### Outfit 81 — Russian rural peasant male

- `earlymodern_russian_clothing_long_linen_rubakha` — a long $colour linen shirt
- `earlymodern_russian_clothing_narrow_wool_trousers` — a pair of narrow $colour wool trousers
- `earlymodern_russian_clothing_short_wool_zipun` — a close $colour short wool coat
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `earlymodern_russian_clothing_high_leather_boots` — a pair of high leather boots
- `earlymodern_russian_clothing_round_fur_cap` — a round fur cap

#### Outfit 82 — Russian rural winter traveller male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `earlymodern_russian_clothing_narrow_wool_trousers` — a pair of narrow $colour wool trousers
- `renaissance_frontier_muscovite_long_caftan` — a full-skirted $colour long caftan
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_frontier_felted_winter_boots` — a pair of felted winter boots
- `renaissance_frontier_fur_brimmed_cap` — a soft $colour fur-brimmed cap
- `renaissance_shared_clothing_mittens` — a pair of $colour wool mittens

#### Outfit 83 — Russian town artisan male

- `earlymodern_russian_clothing_long_linen_rubakha` — a long $colour linen shirt
- `earlymodern_russian_clothing_narrow_wool_trousers` — a pair of narrow $colour wool trousers
- `earlymodern_russian_clothing_short_wool_zipun` — a close $colour short wool coat
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_ankle_boots` — a pair of ankle leather boots
- `renaissance_frontier_knitted_northern_cap` — a long $colour knitted wool cap
- `renaissance_shared_clothing_travelling_coat` — a loose $colour travelling coat

#### Outfit 84 — Russian urban merchant male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `earlymodern_russian_clothing_narrow_wool_trousers` — a pair of narrow $colour wool trousers
- `renaissance_frontier_muscovite_long_caftan` — a full-skirted $colour long caftan
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_frontier_split_skirt_riding_boots` — a pair of stiff high leather boots
- `renaissance_frontier_fur_brimmed_cap` — a soft $colour fur-brimmed cap
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

#### Outfit 85 — Russian westernising clerk male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_western_clothing_plain_linen_shirt` — a $colour plain linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `renaissance_frontier_close_buttoned_longcoat` — a close-buttoned $colour long coat
- `earlymodern_western_clothing_knee_breeches` — a pair of $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_centraleuropean_clothing_round_felt_hat` — a round-crowned $colour felt hat

#### Outfit 86 — Russian rural field worker female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `earlymodern_russian_clothing_checked_wool_wrap_skirt` — a $colour1 and $colour2 checked wool wrap skirt
- `earlymodern_russian_clothing_linen_work_apron` — a $colour linen work apron
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `earlymodern_russian_clothing_high_leather_boots` — a pair of high leather boots
- `earlymodern_russian_clothing_long_linen_head_veil` — a long $colour linen head veil
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 87 — Russian prosperous rural householder female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_frontier_sleeveless_long_gown` — a sleeveless $colour long overgown
- `earlymodern_russian_clothing_linen_work_apron` — a $colour linen work apron
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `earlymodern_russian_clothing_high_leather_boots` — a pair of high leather boots
- `earlymodern_russian_clothing_long_linen_head_veil` — a long $colour linen head veil
- `renaissance_shared_clothing_fur_lined_coat` — a $colour fur-lined travelling coat

#### Outfit 88 — Russian town craftswoman female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_frontier_sleeveless_long_gown` — a sleeveless $colour long overgown
- `earlymodern_russian_clothing_linen_work_apron` — a $colour linen work apron
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_russian_clothing_long_linen_head_veil` — a long $colour linen head veil
- `renaissance_shared_clothing_travelling_coat` — a loose $colour travelling coat

#### Outfit 89 — Russian urban market trader female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `earlymodern_russian_clothing_checked_wool_wrap_skirt` — a $colour1 and $colour2 checked wool wrap skirt
- `earlymodern_russian_clothing_linen_work_apron` — a $colour linen work apron
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_ankle_boots` — a pair of ankle leather boots
- `earlymodern_russian_clothing_long_linen_head_veil` — a long $colour linen head veil
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 90 — Russian prosperous merchant household female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_frontier_sleeveless_long_gown` — a sleeveless $colour long overgown
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_frontier_felted_winter_boots` — a pair of felted winter boots
- `earlymodern_russian_clothing_long_linen_head_veil` — a long $colour linen head veil
- `renaissance_frontier_muscovite_long_caftan` — a full-skirted $colour long caftan
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 10. Ottoman

> Ottoman civilian dress, c. 1600-1750, ranging from provincial agricultural work to urban craft, retail, scribal, and prosperous household settings. Rank kaftans, command dress, and military uniforms are excluded.

#### Outfit 91 — Ottoman rural cultivator male

- `renaissance_ottoman_collarless_inner_shirt` — a $colour collarless long inner shirt
- `renaissance_ottoman_ankle_gathered_drawers` — a pair of full $colour ankle-gathered drawers
- `renaissance_ottoman_short_entari_coat` — a short fitted $colour front-opening coat
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_ottoman_wrapped_cap_turban` — a compact $colour cap-wound turban

#### Outfit 92 — Ottoman provincial smallholder male

- `renaissance_ottoman_collarless_inner_shirt` — a $colour collarless long inner shirt
- `renaissance_ottoman_full_salwar_trousers` — a pair of very full $colour cotton trousers
- `renaissance_ottoman_short_entari_coat` — a short fitted $colour front-opening coat
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `renaissance_ottoman_wrapped_cap_turban` — a compact $colour cap-wound turban
- `renaissance_shared_clothing_rectangular_mantle` — a $colour rectangular shoulder mantle

#### Outfit 93 — Ottoman urban craftsman male

- `renaissance_ottoman_collarless_inner_shirt` — a $colour collarless long inner shirt
- `renaissance_ottoman_full_salwar_trousers` — a pair of very full $colour cotton trousers
- `renaissance_ottoman_short_fitted_vest` — a short fitted $colour sleeveless vest
- `renaissance_ottoman_long_entari_robe` — a long fitted $colour front-opening robe
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_ottoman_pointed_slippers` — a pair of pointed leather slippers
- `renaissance_ottoman_wrapped_cap_turban` — a compact $colour cap-wound turban

#### Outfit 94 — Ottoman urban shopkeeper male

- `renaissance_ottoman_collarless_inner_shirt` — a $colour collarless long inner shirt
- `renaissance_ottoman_full_salwar_trousers` — a pair of very full $colour cotton trousers
- `renaissance_ottoman_fitted_kaftan` — a fitted $colour long kaftan
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_ottoman_pointed_slippers` — a pair of pointed leather slippers
- `renaissance_ottoman_wrapped_cap_turban` — a compact $colour cap-wound turban
- `renaissance_ottoman_sleeved_outer_robe` — a long-sleeved $colour outer robe

#### Outfit 95 — Ottoman bourgeois scribe male

- `renaissance_ottoman_collarless_inner_shirt` — a $colour collarless long inner shirt
- `renaissance_ottoman_full_salwar_trousers` — a pair of very full $colour cotton trousers
- `renaissance_ottoman_long_entari_robe` — a long fitted $colour front-opening robe
- `renaissance_ottoman_short_fitted_vest` — a short fitted $colour sleeveless vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_ottoman_pointed_slippers` — a pair of pointed leather slippers
- `renaissance_ottoman_wrapped_cap_turban` — a compact $colour cap-wound turban
- `renaissance_ottoman_sleeved_outer_robe` — a long-sleeved $colour outer robe

#### Outfit 96 — Ottoman rural field worker female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_ottoman_ankle_gathered_drawers` — a pair of full $colour ankle-gathered drawers
- `renaissance_ottoman_loose_layered_gown` — a loose layered $colour long gown
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 97 — Ottoman provincial household female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_ottoman_full_salwar_trousers` — a pair of very full $colour cotton trousers
- `renaissance_ottoman_loose_layered_gown` — a loose layered $colour long gown
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 98 — Ottoman urban market vendor female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_ottoman_full_salwar_trousers` — a pair of very full $colour cotton trousers
- `renaissance_ottoman_short_fitted_vest` — a short fitted $colour sleeveless vest
- `renaissance_ottoman_loose_layered_gown` — a loose layered $colour long gown
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_ottoman_pointed_slippers` — a pair of pointed leather slippers
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 99 — Ottoman urban craftswoman female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_ottoman_full_salwar_trousers` — a pair of very full $colour cotton trousers
- `renaissance_ottoman_loose_layered_gown` — a loose layered $colour long gown
- `renaissance_ottoman_short_fitted_vest` — a short fitted $colour sleeveless vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_ottoman_pointed_slippers` — a pair of pointed leather slippers
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil

#### Outfit 100 — Ottoman prosperous urban household female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_ottoman_full_salwar_trousers` — a pair of very full $colour cotton trousers
- `renaissance_ottoman_loose_layered_gown` — a loose layered $colour long gown
- `renaissance_ottoman_full_outdoor_overrobe` — a full enveloping $colour outdoor over-robe
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_ottoman_pointed_slippers` — a pair of pointed leather slippers
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 11. Maghrebi / North African

> Maghrebi civilian dress, c. 1600-1750, covering agrarian, pastoral, artisanal, market, mercantile, and learned urban contexts. Corsair and military dress are outside this pass.

#### Outfit 101 — Maghrebi rural herder male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `earlymodern_maghrebi_clothing_full_cotton_trousers` — a pair of full $colour cotton trousers
- `earlymodern_maghrebi_clothing_sleeveless_gandoura` — a long sleeveless $colour cotton robe
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `earlymodern_maghrebi_clothing_round_felt_cap` — a round $colour felt cap
- `earlymodern_maghrebi_clothing_hooded_wool_djellaba` — a hooded $colour wool over-robe

#### Outfit 102 — Maghrebi oasis cultivator male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_shared_clothing_full_trousers` — a pair of full $colour cotton trousers
- `earlymodern_maghrebi_clothing_sleeveless_gandoura` — a long sleeveless $colour cotton robe
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 103 — Maghrebi urban craftsman male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `earlymodern_maghrebi_clothing_full_cotton_trousers` — a pair of full $colour cotton trousers
- `earlymodern_maghrebi_clothing_sleeveless_gandoura` — a long sleeveless $colour cotton robe
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `earlymodern_maghrebi_clothing_pointed_leather_slippers` — a pair of pointed leather slippers
- `earlymodern_maghrebi_clothing_round_felt_cap` — a round $colour felt cap

#### Outfit 104 — Maghrebi urban merchant male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `earlymodern_maghrebi_clothing_full_cotton_trousers` — a pair of full $colour cotton trousers
- `earlymodern_maghrebi_clothing_short_fitted_caftan` — a fitted $colour cotton caftan
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `earlymodern_maghrebi_clothing_pointed_leather_slippers` — a pair of pointed leather slippers
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `earlymodern_maghrebi_clothing_full_body_haik` — a full $colour cotton body wrap

#### Outfit 105 — Maghrebi learned townsman male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `earlymodern_maghrebi_clothing_full_cotton_trousers` — a pair of full $colour cotton trousers
- `earlymodern_maghrebi_clothing_sleeveless_gandoura` — a long sleeveless $colour cotton robe
- `earlymodern_maghrebi_clothing_hooded_wool_djellaba` — a hooded $colour wool over-robe
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `earlymodern_maghrebi_clothing_pointed_leather_slippers` — a pair of pointed leather slippers
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 106 — Maghrebi rural field worker female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_shared_clothing_full_trousers` — a pair of full $colour cotton trousers
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `earlymodern_maghrebi_clothing_full_body_haik` — a full $colour cotton body wrap

#### Outfit 107 — Maghrebi rural household female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `earlymodern_maghrebi_clothing_full_cotton_trousers` — a pair of full $colour cotton trousers
- `earlymodern_maghrebi_clothing_short_fitted_caftan` — a fitted $colour cotton caftan
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `earlymodern_maghrebi_clothing_pointed_leather_slippers` — a pair of pointed leather slippers
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `earlymodern_maghrebi_clothing_full_body_haik` — a full $colour cotton body wrap

#### Outfit 108 — Maghrebi urban market vendor female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_shared_clothing_full_trousers` — a pair of full $colour cotton trousers
- `earlymodern_maghrebi_clothing_sleeveless_gandoura` — a long sleeveless $colour cotton robe
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `earlymodern_maghrebi_clothing_full_body_haik` — a full $colour cotton body wrap

#### Outfit 109 — Maghrebi urban textile worker female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `earlymodern_maghrebi_clothing_full_cotton_trousers` — a pair of full $colour cotton trousers
- `earlymodern_maghrebi_clothing_short_fitted_caftan` — a fitted $colour cotton caftan
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `earlymodern_maghrebi_clothing_pointed_leather_slippers` — a pair of pointed leather slippers
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil

#### Outfit 110 — Maghrebi prosperous merchant household female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `earlymodern_maghrebi_clothing_full_cotton_trousers` — a pair of full $colour cotton trousers
- `earlymodern_maghrebi_clothing_short_fitted_caftan` — a fitted $colour cotton caftan
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `earlymodern_maghrebi_clothing_pointed_leather_slippers` — a pair of pointed leather slippers
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `earlymodern_maghrebi_clothing_full_body_haik` — a full $colour cotton body wrap
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 12. Safavid / post-Safavid Persianate

> Persianate civilian clothing, c. 1600-1750, with rural, caravan-adjacent, craft, mercantile, and scribal ensembles. Court robes and military command dress are excluded.

#### Outfit 111 — Persianate rural cultivator male

- `renaissance_persianate_long_inner_shirt` — a long $colour collarless inner shirt
- `renaissance_persianate_close_ankle_trousers` — a pair of close $colour ankle trousers
- `renaissance_persianate_short_fitted_coat` — a short fitted $colour front-opening coat
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `earlymodern_persianate_clothing_soft_round_cap` — a soft $colour cotton round cap

#### Outfit 112 — Persianate pastoral traveller male

- `renaissance_persianate_long_inner_shirt` — a long $colour collarless inner shirt
- `renaissance_persianate_wide_pleated_trousers` — a pair of wide pleated $colour wool trousers
- `renaissance_persianate_long_fitted_coat` — a long fitted $colour front-opening coat
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_persianate_soft_ridingboots` — a pair of soft high leather boots
- `earlymodern_persianate_clothing_soft_round_cap` — a soft $colour cotton round cap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 113 — Persianate urban craftsman male

- `renaissance_persianate_long_inner_shirt` — a long $colour collarless inner shirt
- `renaissance_persianate_close_ankle_trousers` — a pair of close $colour ankle trousers
- `renaissance_persianate_short_fitted_coat` — a short fitted $colour front-opening coat
- `renaissance_persianate_sleeveless_longvest` — a sleeveless $colour long vest
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_persianate_pointed_slippers` — a pair of pointed leather slippers
- `earlymodern_persianate_clothing_soft_round_cap` — a soft $colour cotton round cap

#### Outfit 114 — Persianate urban merchant male

- `renaissance_persianate_long_inner_shirt` — a long $colour collarless inner shirt
- `renaissance_persianate_close_ankle_trousers` — a pair of close $colour ankle trousers
- `renaissance_persianate_long_fitted_coat` — a long fitted $colour front-opening coat
- `renaissance_persianate_sleeveless_longvest` — a sleeveless $colour long vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_persianate_pointed_slippers` — a pair of pointed leather slippers
- `renaissance_persianate_conical_turban` — a conical $colour structured turban
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 115 — Persianate bourgeois scribe male

- `renaissance_persianate_long_inner_shirt` — a long $colour collarless inner shirt
- `renaissance_persianate_close_ankle_trousers` — a pair of close $colour ankle trousers
- `renaissance_persianate_flared_skirt_coat` — a fitted $colour flared-skirt coat
- `renaissance_persianate_sleeveless_longvest` — a sleeveless $colour long vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_persianate_pointed_slippers` — a pair of pointed leather slippers
- `renaissance_persianate_conical_turban` — a conical $colour structured turban

#### Outfit 116 — Persianate rural field worker female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_persianate_close_ankle_trousers` — a pair of close $colour ankle trousers
- `renaissance_shared_clothing_wrap_jacket` — a $colour short wrap jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 117 — Persianate prosperous rural householder female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_persianate_close_ankle_trousers` — a pair of close $colour ankle trousers
- `renaissance_persianate_long_fitted_coat` — a long fitted $colour front-opening coat
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_persianate_pointed_slippers` — a pair of pointed leather slippers
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 118 — Persianate urban textile worker female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_persianate_close_ankle_trousers` — a pair of close $colour ankle trousers
- `renaissance_persianate_short_fitted_coat` — a short fitted $colour front-opening coat
- `renaissance_persianate_sleeveless_longvest` — a sleeveless $colour long vest
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_persianate_pointed_slippers` — a pair of pointed leather slippers
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 119 — Persianate urban market trader female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_persianate_close_ankle_trousers` — a pair of close $colour ankle trousers
- `renaissance_persianate_long_fitted_coat` — a long fitted $colour front-opening coat
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_persianate_pointed_slippers` — a pair of pointed leather slippers
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil

#### Outfit 120 — Persianate prosperous merchant household female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_persianate_close_ankle_trousers` — a pair of close $colour ankle trousers
- `renaissance_indopersian_diagonal_tie_jama` — a diagonally tied $colour long coat
- `renaissance_persianate_sleeveless_longvest` — a sleeveless $colour long vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_persianate_pointed_slippers` — a pair of pointed leather slippers
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 13. Mughal / Indo-Persian

> Mughal and Indo-Persian civilian clothing, c. 1600-1750, spanning cultivators, artisans, shopkeepers, scribes, merchants, textile workers, and prosperous urban households without court regalia.

#### Outfit 121 — Indo-Persian rural cultivator male

- `renaissance_southasian_stitched_loindrawers` — a pair of close $colour cotton loin drawers
- `renaissance_southasian_pleated_waistcloth` — a long pleated $colour cotton waistcloth
- `renaissance_southasian_short_sideslit_tunic` — a short $colour cotton side-slit tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_southasian_flat_wound_turban` — a broad flat-wound $colour cotton turban

#### Outfit 122 — Indo-Persian rural prosperous farmer male

- `renaissance_southasian_stitched_loindrawers` — a pair of close $colour cotton loin drawers
- `renaissance_southasian_bunched_ankletrousers` — a pair of narrow $colour bunched ankle trousers
- `renaissance_southasian_long_sideslit_tunic` — a long $colour cotton side-slit tunic
- `earlymodern_southasian_clothing_chintz_merchant_jacket` — a $colour patterned chintz jacket
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_southasian_flat_wound_turban` — a broad flat-wound $colour cotton turban

#### Outfit 123 — Indo-Persian urban artisan male

- `renaissance_southasian_stitched_loindrawers` — a pair of close $colour cotton loin drawers
- `renaissance_southasian_bunched_ankletrousers` — a pair of narrow $colour bunched ankle trousers
- `renaissance_southasian_short_sideslit_tunic` — a short $colour cotton side-slit tunic
- `renaissance_southasian_short_wrapjacket` — a short $colour cotton cross-tied jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_southasian_flat_wound_turban` — a broad flat-wound $colour cotton turban

#### Outfit 124 — Indo-Persian urban shopkeeper male

- `renaissance_southasian_stitched_loindrawers` — a pair of close $colour cotton loin drawers
- `renaissance_southasian_bunched_ankletrousers` — a pair of narrow $colour bunched ankle trousers
- `renaissance_indopersian_sidefastened_jama` — a side-fastened $colour flared long coat
- `earlymodern_southasian_clothing_chintz_merchant_jacket` — a $colour patterned chintz jacket
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_southasian_flat_wound_turban` — a broad flat-wound $colour cotton turban
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

#### Outfit 125 — Indo-Persian bourgeois scribe male

- `renaissance_southasian_stitched_loindrawers` — a pair of close $colour cotton loin drawers
- `renaissance_southasian_bunched_ankletrousers` — a pair of narrow $colour bunched ankle trousers
- `renaissance_indopersian_diagonal_tie_jama` — a diagonally tied $colour long coat
- `renaissance_persianate_sleeveless_longvest` — a sleeveless $colour long vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_southasian_flat_wound_turban` — a broad flat-wound $colour cotton turban
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

#### Outfit 126 — Indo-Persian rural field worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_pleated_waistcloth` — a long pleated $colour cotton waistcloth
- `renaissance_southasian_fitted_shortbodice` — a fitted $colour cotton short bodice
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

#### Outfit 127 — Indo-Persian rural household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_gathered_longskirt` — a full gathered $colour cotton skirt
- `renaissance_southasian_backtied_bodice` — a back-tied $colour cotton bodice
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 128 — Indo-Persian urban textile worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_gathered_longskirt` — a full gathered $colour cotton skirt
- `renaissance_southasian_fitted_shortbodice` — a fitted $colour cotton short bodice
- `renaissance_southasian_short_wrapjacket` — a short $colour cotton cross-tied jacket
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil

#### Outfit 129 — Indo-Persian urban market trader female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_shoulderdraped_garment` — a full $colour cotton shoulder-draped garment
- `renaissance_southasian_backtied_bodice` — a back-tied $colour cotton bodice
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

#### Outfit 130 — Indo-Persian prosperous merchant household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_panelled_longskirt` — a panelled flared $colour silk skirt
- `renaissance_southasian_long_sideslit_tunic` — a long $colour cotton side-slit tunic
- `earlymodern_southasian_clothing_chintz_merchant_jacket` — a $colour patterned chintz jacket
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

### 14. Maratha / Rajput / Deccan

> Regional South Asian civilian dress, c. 1650-1750, emphasizing agricultural, textile, market, mercantile, and administrative settings. Warrior and courtly dress are deliberately omitted despite the master culture label.

#### Outfit 131 — Deccan rural cultivator male

- `renaissance_southasian_stitched_loindrawers` — a pair of close $colour cotton loin drawers
- `renaissance_southasian_pleated_waistcloth` — a long pleated $colour cotton waistcloth
- `renaissance_southasian_short_sideslit_tunic` — a short $colour cotton side-slit tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_southasian_toepost_woodensandals` — a pair of toe-post wooden sandals
- `renaissance_southasian_flat_wound_turban` — a broad flat-wound $colour cotton turban

#### Outfit 132 — Deccan prosperous smallholder male

- `renaissance_southasian_stitched_loindrawers` — a pair of close $colour cotton loin drawers
- `renaissance_southasian_pleated_waistcloth` — a long pleated $colour cotton waistcloth
- `renaissance_southasian_long_sideslit_tunic` — a long $colour cotton side-slit tunic
- `earlymodern_southasian_clothing_chintz_merchant_jacket` — a $colour patterned chintz jacket
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_southasian_flat_wound_turban` — a broad flat-wound $colour cotton turban

#### Outfit 133 — Deccan urban textile artisan male

- `renaissance_southasian_stitched_loindrawers` — a pair of close $colour cotton loin drawers
- `renaissance_southasian_bunched_ankletrousers` — a pair of narrow $colour bunched ankle trousers
- `renaissance_southasian_short_sideslit_tunic` — a short $colour cotton side-slit tunic
- `renaissance_southasian_short_wrapjacket` — a short $colour cotton cross-tied jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_southasian_flat_wound_turban` — a broad flat-wound $colour cotton turban

#### Outfit 134 — Deccan urban merchant male

- `renaissance_southasian_stitched_loindrawers` — a pair of close $colour cotton loin drawers
- `renaissance_southasian_bunched_ankletrousers` — a pair of narrow $colour bunched ankle trousers
- `renaissance_southasian_long_crossover_robe` — a long $colour cotton cross-over robe
- `earlymodern_southasian_clothing_chintz_merchant_jacket` — a $colour patterned chintz jacket
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_southasian_flat_wound_turban` — a broad flat-wound $colour cotton turban
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

#### Outfit 135 — Deccan bourgeois record keeper male

- `renaissance_southasian_stitched_loindrawers` — a pair of close $colour cotton loin drawers
- `renaissance_southasian_bunched_ankletrousers` — a pair of narrow $colour bunched ankle trousers
- `renaissance_indopersian_diagonal_tie_jama` — a diagonally tied $colour long coat
- `renaissance_persianate_sleeveless_longvest` — a sleeveless $colour long vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_southasian_flat_wound_turban` — a broad flat-wound $colour cotton turban

#### Outfit 136 — Deccan rural field worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_pleated_waistcloth` — a long pleated $colour cotton waistcloth
- `renaissance_southasian_fitted_shortbodice` — a fitted $colour cotton short bodice
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_southasian_toepost_woodensandals` — a pair of toe-post wooden sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 137 — Deccan rural household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_gathered_longskirt` — a full gathered $colour cotton skirt
- `renaissance_southasian_backtied_bodice` — a back-tied $colour cotton bodice
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

#### Outfit 138 — Deccan urban spinner female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_gathered_longskirt` — a full gathered $colour cotton skirt
- `renaissance_southasian_fitted_shortbodice` — a fitted $colour cotton short bodice
- `renaissance_southasian_short_wrapjacket` — a short $colour cotton cross-tied jacket
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 139 — Deccan urban market trader female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_shoulderdraped_garment` — a full $colour cotton shoulder-draped garment
- `renaissance_southasian_backtied_bodice` — a back-tied $colour cotton bodice
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

#### Outfit 140 — Deccan prosperous merchant household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_panelled_longskirt` — a panelled flared $colour silk skirt
- `renaissance_southasian_long_sideslit_tunic` — a long $colour cotton side-slit tunic
- `earlymodern_southasian_clothing_chintz_merchant_jacket` — a $colour patterned chintz jacket
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

### 15. South Indian / coastal trade

> South Indian and coastal civilian dress, c. 1600-1750, covering agrarian, weaving, fishing-port, retail, and bookkeeping contexts. Court and military attire are excluded.

#### Outfit 141 — South Indian rural rice cultivator male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_southasian_pleated_waistcloth` — a long pleated $colour cotton waistcloth
- `renaissance_southasian_short_sideslit_tunic` — a short $colour cotton side-slit tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_southasian_toepost_woodensandals` — a pair of toe-post wooden sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 142 — South Indian coastal fisherman male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_southasian_pleated_waistcloth` — a long pleated $colour cotton waistcloth
- `renaissance_shared_clothing_short_undershirt` — a $colour short undershirt
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

#### Outfit 143 — South Indian urban weaver male

- `renaissance_southasian_stitched_loindrawers` — a pair of close $colour cotton loin drawers
- `renaissance_southasian_pleated_waistcloth` — a long pleated $colour cotton waistcloth
- `renaissance_southasian_short_sideslit_tunic` — a short $colour cotton side-slit tunic
- `renaissance_southasian_short_wrapjacket` — a short $colour cotton cross-tied jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 144 — South Indian port merchant male

- `renaissance_southasian_stitched_loindrawers` — a pair of close $colour cotton loin drawers
- `renaissance_southasian_pleated_waistcloth` — a long pleated $colour cotton waistcloth
- `renaissance_southasian_long_sideslit_tunic` — a long $colour cotton side-slit tunic
- `earlymodern_southasian_clothing_chintz_merchant_jacket` — a $colour patterned chintz jacket
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_southasian_flat_wound_turban` — a broad flat-wound $colour cotton turban
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

#### Outfit 145 — South Indian bourgeois bookkeeper male

- `renaissance_southasian_stitched_loindrawers` — a pair of close $colour cotton loin drawers
- `renaissance_southasian_bunched_ankletrousers` — a pair of narrow $colour bunched ankle trousers
- `renaissance_southasian_long_crossover_robe` — a long $colour cotton cross-over robe
- `renaissance_persianate_sleeveless_longvest` — a sleeveless $colour long vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_southasian_flat_wound_turban` — a broad flat-wound $colour cotton turban

#### Outfit 146 — South Indian rural field worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_shoulderdraped_garment` — a full $colour cotton shoulder-draped garment
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_southasian_toepost_woodensandals` — a pair of toe-post wooden sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 147 — South Indian rural household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_pleated_waistcloth` — a long pleated $colour cotton waistcloth
- `renaissance_southasian_fitted_shortbodice` — a fitted $colour cotton short bodice
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

#### Outfit 148 — South Indian urban textile worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_gathered_longskirt` — a full gathered $colour cotton skirt
- `renaissance_southasian_backtied_bodice` — a back-tied $colour cotton bodice
- `renaissance_southasian_short_wrapjacket` — a short $colour cotton cross-tied jacket
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 149 — South Indian market trader female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_shoulderdraped_garment` — a full $colour cotton shoulder-draped garment
- `renaissance_southasian_fitted_shortbodice` — a fitted $colour cotton short bodice
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

#### Outfit 150 — South Indian prosperous merchant household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_southasian_panelled_longskirt` — a panelled flared $colour silk skirt
- `renaissance_southasian_long_sideslit_tunic` — a long $colour cotton side-slit tunic
- `earlymodern_southasian_clothing_chintz_merchant_jacket` — a $colour patterned chintz jacket
- `renaissance_southasian_curvedtoe_shoes` — a pair of curved-toe leather shoes
- `renaissance_shared_clothing_long_head_veil` — a long $colour head veil
- `earlymodern_southasian_clothing_light_cotton_shawl` — a light $colour cotton shawl

### 16. Qing China

> Early Qing civilian clothing, c. 1644-1750, for farmers, artisans, shopkeepers, clerks, textile workers, and prosperous merchant households. Official rank dress and banner military uniforms are excluded.

#### Outfit 151 — Qing rural farm labourer male

- `earlymodern_qing_clothing_crosscollar_innerrobe` — a cross-collared $colour cotton inner robe
- `earlymodern_qing_clothing_wide_cotton_trousers` — a pair of wide $colour cotton trousers
- `earlymodern_qing_clothing_short_riding_jacket` — a short $colour cotton riding jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `earlymodern_qing_clothing_head_kerchief` — a $colour cotton head kerchief

#### Outfit 152 — Qing prosperous rural householder male

- `earlymodern_qing_clothing_crosscollar_innerrobe` — a cross-collared $colour cotton inner robe
- `earlymodern_qing_clothing_wide_cotton_trousers` — a pair of wide $colour cotton trousers
- `earlymodern_qing_clothing_long_sidefastened_robe` — a long side-fastened $colour cotton robe
- `earlymodern_qing_clothing_sleeveless_long_vest` — a sleeveless $colour long vest
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `earlymodern_qing_clothing_cloth_shoes` — a pair of $colour cloth shoes
- `earlymodern_qing_clothing_round_skullcap` — a round $colour cloth skullcap

#### Outfit 153 — Qing urban artisan male

- `earlymodern_qing_clothing_crosscollar_innerrobe` — a cross-collared $colour cotton inner robe
- `earlymodern_qing_clothing_wide_cotton_trousers` — a pair of wide $colour cotton trousers
- `earlymodern_qing_clothing_short_riding_jacket` — a short $colour cotton riding jacket
- `earlymodern_qing_clothing_sleeveless_long_vest` — a sleeveless $colour long vest
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `earlymodern_qing_clothing_cloth_shoes` — a pair of $colour cloth shoes
- `earlymodern_qing_clothing_head_kerchief` — a $colour cotton head kerchief

#### Outfit 154 — Qing urban shopkeeper male

- `earlymodern_qing_clothing_crosscollar_innerrobe` — a cross-collared $colour cotton inner robe
- `earlymodern_qing_clothing_wide_cotton_trousers` — a pair of wide $colour cotton trousers
- `earlymodern_qing_clothing_long_sidefastened_robe` — a long side-fastened $colour cotton robe
- `earlymodern_qing_clothing_sleeveless_long_vest` — a sleeveless $colour long vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `earlymodern_qing_clothing_cloth_shoes` — a pair of $colour cloth shoes
- `earlymodern_qing_clothing_round_skullcap` — a round $colour cloth skullcap
- `renaissance_shared_clothing_travelling_coat` — a loose $colour travelling coat

#### Outfit 155 — Qing bourgeois clerk male

- `earlymodern_qing_clothing_crosscollar_innerrobe` — a cross-collared $colour cotton inner robe
- `earlymodern_qing_clothing_wide_cotton_trousers` — a pair of wide $colour cotton trousers
- `earlymodern_qing_clothing_fine_sidefastened_robe` — a fine side-fastened $colour silk robe
- `earlymodern_qing_clothing_sleeveless_long_vest` — a sleeveless $colour long vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `earlymodern_qing_clothing_cloth_shoes` — a pair of $colour cloth shoes
- `earlymodern_qing_clothing_round_skullcap` — a round $colour cloth skullcap

#### Outfit 156 — Qing rural field worker female

- `earlymodern_qing_clothing_crosscollar_innerrobe` — a cross-collared $colour cotton inner robe
- `earlymodern_qing_clothing_wide_cotton_trousers` — a pair of wide $colour cotton trousers
- `earlymodern_qing_clothing_sidefastened_womens_jacket` — a side-fastened $colour cotton jacket
- `earlymodern_qing_clothing_work_apron` — a $colour cotton work apron
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `earlymodern_qing_clothing_head_kerchief` — a $colour cotton head kerchief

#### Outfit 157 — Qing prosperous rural householder female

- `earlymodern_qing_clothing_crosscollar_innerrobe` — a cross-collared $colour cotton inner robe
- `earlymodern_qing_clothing_pleated_long_skirt` — a pleated $colour cotton long skirt
- `earlymodern_qing_clothing_sidefastened_womens_jacket` — a side-fastened $colour cotton jacket
- `earlymodern_qing_clothing_work_apron` — a $colour cotton work apron
- `earlymodern_qing_clothing_cloth_shoes` — a pair of $colour cloth shoes
- `earlymodern_qing_clothing_head_kerchief` — a $colour cotton head kerchief
- `earlymodern_qing_clothing_padded_winter_coat` — a padded $colour cotton winter coat

#### Outfit 158 — Qing urban textile worker female

- `earlymodern_qing_clothing_crosscollar_innerrobe` — a cross-collared $colour cotton inner robe
- `earlymodern_qing_clothing_wide_cotton_trousers` — a pair of wide $colour cotton trousers
- `earlymodern_qing_clothing_sidefastened_womens_jacket` — a side-fastened $colour cotton jacket
- `earlymodern_qing_clothing_work_apron` — a $colour cotton work apron
- `earlymodern_qing_clothing_cloth_shoes` — a pair of $colour cloth shoes
- `earlymodern_qing_clothing_head_kerchief` — a $colour cotton head kerchief

#### Outfit 159 — Qing urban market trader female

- `earlymodern_qing_clothing_crosscollar_innerrobe` — a cross-collared $colour cotton inner robe
- `earlymodern_qing_clothing_pleated_long_skirt` — a pleated $colour cotton long skirt
- `earlymodern_qing_clothing_sidefastened_womens_jacket` — a side-fastened $colour cotton jacket
- `earlymodern_qing_clothing_work_apron` — a $colour cotton work apron
- `earlymodern_qing_clothing_cloth_shoes` — a pair of $colour cloth shoes
- `earlymodern_qing_clothing_head_kerchief` — a $colour cotton head kerchief
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 160 — Qing prosperous merchant household female

- `earlymodern_qing_clothing_crosscollar_innerrobe` — a cross-collared $colour cotton inner robe
- `earlymodern_qing_clothing_pleated_long_skirt` — a pleated $colour cotton long skirt
- `earlymodern_qing_clothing_sidefastened_womens_jacket` — a side-fastened $colour cotton jacket
- `earlymodern_qing_clothing_sleeveless_long_vest` — a sleeveless $colour long vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `earlymodern_qing_clothing_cloth_shoes` — a pair of $colour cloth shoes
- `earlymodern_qing_clothing_round_skullcap` — a round $colour cloth skullcap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

### 17. Late Ming survival / transition

> Late Ming and transition-period civilian forms, c. 1600-1650, retained only where the silhouette remains a distinct and plausible local continuity. Official and court dress are excluded.

#### Outfit 161 — Late Ming rural field worker male

- `renaissance_ming_crosscollar_innerrobe` — a cross-collared $colour ramie under-robe
- `renaissance_ming_wide_drawstring_trousers` — a pair of wide $colour cotton drawstring trousers
- `renaissance_ming_narrowsleeve_workerrobe` — a narrow-sleeved $colour cotton work robe
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_shared_clothing_close_cloth_cap` — a close-fitting $colour cloth cap

#### Outfit 162 — Late Ming prosperous rural householder male

- `renaissance_ming_crosscollar_innerrobe` — a cross-collared $colour ramie under-robe
- `renaissance_ming_wide_drawstring_trousers` — a pair of wide $colour cotton drawstring trousers
- `renaissance_ming_roundcollar_longrobe` — a round-collared $colour long robe
- `renaissance_ming_long_sleeveless_overvest` — a long sleeveless $colour silk over-vest
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_ming_cloth_courtboots` — a pair of high $colour cloth boots
- `renaissance_ming_soft_scholarcap` — a soft folded $colour scholar cap

#### Outfit 163 — Late Ming urban artisan male

- `renaissance_ming_crosscollar_innerrobe` — a cross-collared $colour ramie under-robe
- `renaissance_ming_wide_drawstring_trousers` — a pair of wide $colour cotton drawstring trousers
- `renaissance_ming_short_sidefastened_jacket` — a short side-fastened $colour cotton jacket
- `renaissance_ming_long_sleeveless_overvest` — a long sleeveless $colour silk over-vest
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `earlymodern_qing_clothing_cloth_shoes` — a pair of $colour cloth shoes
- `renaissance_shared_clothing_close_cloth_cap` — a close-fitting $colour cloth cap

#### Outfit 164 — Late Ming urban shopkeeper male

- `renaissance_ming_crosscollar_innerrobe` — a cross-collared $colour ramie under-robe
- `renaissance_ming_wide_drawstring_trousers` — a pair of wide $colour cotton drawstring trousers
- `renaissance_ming_roundcollar_longrobe` — a round-collared $colour long robe
- `renaissance_ming_long_sleeveless_overvest` — a long sleeveless $colour silk over-vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_ming_cloth_courtboots` — a pair of high $colour cloth boots
- `renaissance_ming_soft_scholarcap` — a soft folded $colour scholar cap

#### Outfit 165 — Late Ming bourgeois scholar-merchant male

- `renaissance_ming_crosscollar_innerrobe` — a cross-collared $colour ramie under-robe
- `renaissance_ming_wide_drawstring_trousers` — a pair of wide $colour cotton drawstring trousers
- `renaissance_ming_straightcollar_openrobe` — a straight-collared $colour open robe
- `renaissance_ming_long_sleeveless_overvest` — a long sleeveless $colour silk over-vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_ming_cloth_courtboots` — a pair of high $colour cloth boots
- `renaissance_ming_soft_scholarcap` — a soft folded $colour scholar cap

#### Outfit 166 — Late Ming rural field worker female

- `renaissance_ming_crosscollar_innerrobe` — a cross-collared $colour ramie under-robe
- `renaissance_ming_wide_drawstring_trousers` — a pair of wide $colour cotton drawstring trousers
- `renaissance_ming_short_sidefastened_jacket` — a short side-fastened $colour cotton jacket
- `earlymodern_qing_clothing_work_apron` — a $colour cotton work apron
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_shared_clothing_close_cloth_cap` — a close-fitting $colour cloth cap

#### Outfit 167 — Late Ming prosperous rural householder female

- `renaissance_ming_crosscollar_innerrobe` — a cross-collared $colour ramie under-robe
- `renaissance_ming_pleated_panelskirt` — a pleated $colour silk panel skirt
- `renaissance_ming_long_sidefastened_jacket` — a long side-fastened $colour cotton jacket
- `earlymodern_qing_clothing_work_apron` — a $colour cotton work apron
- `earlymodern_qing_clothing_cloth_shoes` — a pair of $colour cloth shoes
- `renaissance_shared_clothing_close_cloth_cap` — a close-fitting $colour cloth cap

#### Outfit 168 — Late Ming urban textile worker female

- `renaissance_ming_crosscollar_innerrobe` — a cross-collared $colour ramie under-robe
- `renaissance_ming_wide_drawstring_trousers` — a pair of wide $colour cotton drawstring trousers
- `renaissance_ming_short_sidefastened_jacket` — a short side-fastened $colour cotton jacket
- `earlymodern_qing_clothing_work_apron` — a $colour cotton work apron
- `earlymodern_qing_clothing_cloth_shoes` — a pair of $colour cloth shoes
- `renaissance_shared_clothing_close_cloth_cap` — a close-fitting $colour cloth cap

#### Outfit 169 — Late Ming urban market trader female

- `renaissance_ming_crosscollar_innerrobe` — a cross-collared $colour ramie under-robe
- `renaissance_ming_pleated_panelskirt` — a pleated $colour silk panel skirt
- `renaissance_ming_long_sidefastened_jacket` — a long side-fastened $colour cotton jacket
- `earlymodern_qing_clothing_work_apron` — a $colour cotton work apron
- `earlymodern_qing_clothing_cloth_shoes` — a pair of $colour cloth shoes
- `renaissance_shared_clothing_close_cloth_cap` — a close-fitting $colour cloth cap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 170 — Late Ming prosperous merchant household female

- `renaissance_ming_crosscollar_innerrobe` — a cross-collared $colour ramie under-robe
- `renaissance_ming_pleated_panelskirt` — a pleated $colour silk panel skirt
- `renaissance_ming_long_sidefastened_jacket` — a long side-fastened $colour cotton jacket
- `renaissance_ming_long_sleeveless_overvest` — a long sleeveless $colour silk over-vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `earlymodern_qing_clothing_cloth_shoes` — a pair of $colour cloth shoes
- `renaissance_ming_soft_scholarcap` — a soft folded $colour scholar cap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

### 18. Joseon Korea

> Joseon civilian clothing, c. 1600-1750, covering farming, household, craft, market, shop, and literate occupations. Court rank garments and military clothing are excluded.

#### Outfit 171 — Joseon rural farmer male

- `renaissance_joseon_long_crossfront_underrobe` — a long cross-front $colour ramie under-robe
- `earlymodern_joseon_clothing_full_baji_trousers` — a pair of full $colour cotton trousers
- `renaissance_joseon_short_crossfront_jacket` — a short cross-front $colour ramie jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `earlymodern_joseon_clothing_white_cloth_socks` — a pair of white cloth socks
- `renaissance_joseon_white_clothshoes` — a pair of white cloth shoes
- `earlymodern_joseon_clothing_tied_headcloth` — a tied $colour cotton headcloth

#### Outfit 172 — Joseon prosperous rural householder male

- `renaissance_joseon_long_crossfront_underrobe` — a long cross-front $colour ramie under-robe
- `earlymodern_joseon_clothing_full_baji_trousers` — a pair of full $colour cotton trousers
- `renaissance_joseon_long_crossfront_jacket` — a long cross-front $colour silk jacket
- `earlymodern_joseon_clothing_plain_cotton_overcoat` — a plain $colour cotton overcoat
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `earlymodern_joseon_clothing_white_cloth_socks` — a pair of white cloth socks
- `renaissance_joseon_white_clothshoes` — a pair of white cloth shoes
- `earlymodern_joseon_clothing_tied_headcloth` — a tied $colour cotton headcloth

#### Outfit 173 — Joseon urban artisan male

- `renaissance_joseon_long_crossfront_underrobe` — a long cross-front $colour ramie under-robe
- `earlymodern_joseon_clothing_full_baji_trousers` — a pair of full $colour cotton trousers
- `renaissance_joseon_short_crossfront_jacket` — a short cross-front $colour ramie jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `earlymodern_joseon_clothing_white_cloth_socks` — a pair of white cloth socks
- `renaissance_joseon_white_clothshoes` — a pair of white cloth shoes
- `earlymodern_joseon_clothing_tied_headcloth` — a tied $colour cotton headcloth
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 174 — Joseon urban shopkeeper male

- `renaissance_joseon_long_crossfront_underrobe` — a long cross-front $colour ramie under-robe
- `earlymodern_joseon_clothing_full_baji_trousers` — a pair of full $colour cotton trousers
- `renaissance_joseon_long_crossfront_jacket` — a long cross-front $colour silk jacket
- `earlymodern_joseon_clothing_plain_cotton_overcoat` — a plain $colour cotton overcoat
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `earlymodern_joseon_clothing_white_cloth_socks` — a pair of white cloth socks
- `renaissance_joseon_white_clothshoes` — a pair of white cloth shoes
- `renaissance_joseon_tall_horsehairhat` — a tall translucent brimmed hat

#### Outfit 175 — Joseon bourgeois scholar male

- `renaissance_joseon_long_crossfront_underrobe` — a long cross-front $colour ramie under-robe
- `earlymodern_joseon_clothing_full_baji_trousers` — a pair of full $colour cotton trousers
- `renaissance_joseon_broadsleeve_scholaroverrobe` — a broad-sleeved $colour scholar over-robe
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `earlymodern_joseon_clothing_white_cloth_socks` — a pair of white cloth socks
- `renaissance_joseon_white_clothshoes` — a pair of white cloth shoes
- `renaissance_joseon_tall_horsehairhat` — a tall translucent brimmed hat

#### Outfit 176 — Joseon rural field worker female

- `renaissance_joseon_long_crossfront_underrobe` — a long cross-front $colour ramie under-robe
- `earlymodern_joseon_clothing_full_baji_trousers` — a pair of full $colour cotton trousers
- `renaissance_joseon_short_crossfront_jacket` — a short cross-front $colour ramie jacket
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `earlymodern_joseon_clothing_white_cloth_socks` — a pair of white cloth socks
- `renaissance_joseon_white_clothshoes` — a pair of white cloth shoes
- `earlymodern_joseon_clothing_tied_headcloth` — a tied $colour cotton headcloth

#### Outfit 177 — Joseon prosperous rural householder female

- `renaissance_joseon_long_crossfront_underrobe` — a long cross-front $colour ramie under-robe
- `renaissance_joseon_full_gathered_wrapskirt` — a full gathered $colour wrap skirt
- `renaissance_joseon_short_crossfront_jacket` — a short cross-front $colour ramie jacket
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `earlymodern_joseon_clothing_white_cloth_socks` — a pair of white cloth socks
- `renaissance_joseon_white_clothshoes` — a pair of white cloth shoes
- `earlymodern_joseon_clothing_tied_headcloth` — a tied $colour cotton headcloth
- `earlymodern_joseon_clothing_plain_cotton_overcoat` — a plain $colour cotton overcoat

#### Outfit 178 — Joseon urban textile worker female

- `renaissance_joseon_long_crossfront_underrobe` — a long cross-front $colour ramie under-robe
- `earlymodern_joseon_clothing_full_baji_trousers` — a pair of full $colour cotton trousers
- `renaissance_joseon_short_crossfront_jacket` — a short cross-front $colour ramie jacket
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `earlymodern_joseon_clothing_white_cloth_socks` — a pair of white cloth socks
- `renaissance_joseon_white_clothshoes` — a pair of white cloth shoes
- `earlymodern_joseon_clothing_tied_headcloth` — a tied $colour cotton headcloth
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 179 — Joseon urban market trader female

- `renaissance_joseon_long_crossfront_underrobe` — a long cross-front $colour ramie under-robe
- `renaissance_joseon_full_gathered_wrapskirt` — a full gathered $colour wrap skirt
- `renaissance_joseon_short_crossfront_jacket` — a short cross-front $colour ramie jacket
- `earlymodern_western_clothing_linen_work_apron` — a $colour linen work apron
- `earlymodern_joseon_clothing_white_cloth_socks` — a pair of white cloth socks
- `renaissance_joseon_white_clothshoes` — a pair of white cloth shoes
- `earlymodern_joseon_clothing_tied_headcloth` — a tied $colour cotton headcloth
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 180 — Joseon prosperous merchant household female

- `renaissance_joseon_long_crossfront_underrobe` — a long cross-front $colour ramie under-robe
- `renaissance_joseon_full_gathered_wrapskirt` — a full gathered $colour wrap skirt
- `renaissance_joseon_long_crossfront_jacket` — a long cross-front $colour silk jacket
- `earlymodern_joseon_clothing_plain_cotton_overcoat` — a plain $colour cotton overcoat
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `earlymodern_joseon_clothing_white_cloth_socks` — a pair of white cloth socks
- `renaissance_joseon_white_clothshoes` — a pair of white cloth shoes
- `earlymodern_joseon_clothing_tied_headcloth` — a tied $colour cotton headcloth

### 19. Edo Japan

> Edo-period civilian clothing, c. 1600-1750, emphasizing farmers, artisans, shopkeepers, clerks, market workers, and merchant households. Samurai formalwear and military dress are excluded.

#### Outfit 181 — Edo rural farmer male

- `renaissance_japanese_unlined_summerrobe` — an unlined $colour ramie summer robe
- `renaissance_japanese_field_trousers` — a pair of close $colour ramie field trousers
- `earlymodern_edo_clothing_narrow_woven_obi` — a narrow $colour woven waist sash
- `earlymodern_edo_clothing_woven_sandals` — a pair of $colour woven sandals
- `renaissance_japanese_splittoe_socks` — a pair of $colour split-toe socks
- `earlymodern_edo_clothing_tied_cotton_headcloth` — a tied $colour cotton headcloth
- `earlymodern_edo_clothing_narrow_work_apron` — a narrow $colour cotton work apron

#### Outfit 182 — Edo prosperous rural householder male

- `renaissance_japanese_smallsleeve_wraprobe` — a small-sleeved $colour wrap robe
- `renaissance_japanese_full_pleated_hakama` — a full pleated $colour lower garment
- `earlymodern_edo_clothing_narrow_woven_obi` — a narrow $colour woven waist sash
- `renaissance_japanese_wooden_clogs` — a pair of raised wooden clogs
- `renaissance_japanese_splittoe_socks` — a pair of $colour split-toe socks
- `renaissance_japanese_lacquered_conicalhat` — a lacquered wooden conical hat
- `renaissance_japanese_short_openjacket` — a short open-front $colour jacket

#### Outfit 183 — Edo urban artisan male

- `renaissance_japanese_smallsleeve_wraprobe` — a small-sleeved $colour wrap robe
- `renaissance_japanese_field_trousers` — a pair of close $colour ramie field trousers
- `earlymodern_edo_clothing_narrow_woven_obi` — a narrow $colour woven waist sash
- `earlymodern_edo_clothing_short_cotton_work_coat` — a short $colour cotton work coat
- `earlymodern_edo_clothing_woven_sandals` — a pair of $colour woven sandals
- `renaissance_japanese_splittoe_socks` — a pair of $colour split-toe socks
- `earlymodern_edo_clothing_tied_cotton_headcloth` — a tied $colour cotton headcloth

#### Outfit 184 — Edo urban shopkeeper male

- `renaissance_japanese_smallsleeve_wraprobe` — a small-sleeved $colour wrap robe
- `renaissance_japanese_full_pleated_hakama` — a full pleated $colour lower garment
- `earlymodern_edo_clothing_narrow_woven_obi` — a narrow $colour woven waist sash
- `renaissance_japanese_short_openjacket` — a short open-front $colour jacket
- `renaissance_japanese_wooden_clogs` — a pair of raised wooden clogs
- `renaissance_japanese_splittoe_socks` — a pair of $colour split-toe socks
- `earlymodern_edo_clothing_tied_cotton_headcloth` — a tied $colour cotton headcloth

#### Outfit 185 — Edo bourgeois clerk male

- `renaissance_japanese_smallsleeve_wraprobe` — a small-sleeved $colour wrap robe
- `renaissance_japanese_full_pleated_hakama` — a full pleated $colour lower garment
- `earlymodern_edo_clothing_narrow_woven_obi` — a narrow $colour woven waist sash
- `renaissance_japanese_short_openjacket` — a short open-front $colour jacket
- `renaissance_japanese_wooden_clogs` — a pair of raised wooden clogs
- `renaissance_japanese_splittoe_socks` — a pair of $colour split-toe socks
- `renaissance_japanese_lacquered_conicalhat` — a lacquered wooden conical hat

#### Outfit 186 — Edo rural field worker female

- `renaissance_japanese_unlined_summerrobe` — an unlined $colour ramie summer robe
- `renaissance_japanese_field_trousers` — a pair of close $colour ramie field trousers
- `earlymodern_edo_clothing_narrow_woven_obi` — a narrow $colour woven waist sash
- `earlymodern_edo_clothing_narrow_work_apron` — a narrow $colour cotton work apron
- `earlymodern_edo_clothing_woven_sandals` — a pair of $colour woven sandals
- `renaissance_japanese_splittoe_socks` — a pair of $colour split-toe socks
- `earlymodern_edo_clothing_tied_cotton_headcloth` — a tied $colour cotton headcloth

#### Outfit 187 — Edo prosperous rural householder female

- `renaissance_japanese_smallsleeve_wraprobe` — a small-sleeved $colour wrap robe
- `earlymodern_edo_clothing_wide_woven_obi` — a broad $colour woven waist sash
- `earlymodern_edo_clothing_narrow_work_apron` — a narrow $colour cotton work apron
- `renaissance_japanese_wooden_clogs` — a pair of raised wooden clogs
- `renaissance_japanese_splittoe_socks` — a pair of $colour split-toe socks
- `earlymodern_edo_clothing_tied_cotton_headcloth` — a tied $colour cotton headcloth
- `renaissance_japanese_short_openjacket` — a short open-front $colour jacket

#### Outfit 188 — Edo urban textile worker female

- `renaissance_japanese_smallsleeve_wraprobe` — a small-sleeved $colour wrap robe
- `earlymodern_edo_clothing_narrow_woven_obi` — a narrow $colour woven waist sash
- `earlymodern_edo_clothing_narrow_work_apron` — a narrow $colour cotton work apron
- `earlymodern_edo_clothing_woven_sandals` — a pair of $colour woven sandals
- `renaissance_japanese_splittoe_socks` — a pair of $colour split-toe socks
- `earlymodern_edo_clothing_tied_cotton_headcloth` — a tied $colour cotton headcloth
- `earlymodern_edo_clothing_short_cotton_work_coat` — a short $colour cotton work coat

#### Outfit 189 — Edo urban market trader female

- `renaissance_japanese_smallsleeve_wraprobe` — a small-sleeved $colour wrap robe
- `earlymodern_edo_clothing_wide_woven_obi` — a broad $colour woven waist sash
- `earlymodern_edo_clothing_narrow_work_apron` — a narrow $colour cotton work apron
- `renaissance_japanese_wooden_clogs` — a pair of raised wooden clogs
- `renaissance_japanese_splittoe_socks` — a pair of $colour split-toe socks
- `earlymodern_edo_clothing_tied_cotton_headcloth` — a tied $colour cotton headcloth
- `renaissance_japanese_short_openjacket` — a short open-front $colour jacket
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 190 — Edo prosperous merchant household female

- `renaissance_japanese_smallsleeve_wraprobe` — a small-sleeved $colour wrap robe
- `earlymodern_edo_clothing_wide_woven_obi` — a broad $colour woven waist sash
- `renaissance_japanese_short_openjacket` — a short open-front $colour jacket
- `renaissance_japanese_wooden_clogs` — a pair of raised wooden clogs
- `renaissance_japanese_splittoe_socks` — a pair of $colour split-toe socks
- `earlymodern_edo_clothing_tied_cotton_headcloth` — a tied $colour cotton headcloth
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

### 20. Ryukyu and maritime East Asia

> Ryukyuan and maritime East Asian civilian clothing, c. 1600-1750, with agricultural, fishing, artisanal, mercantile, and clerical households. Tribute-court regalia is excluded.

#### Outfit 191 — Ryukyuan rural field worker male

- `renaissance_ryukyu_widesleeve_summerrobe` — a broad-sleeved $colour ramie tropical robe
- `earlymodern_ryukyu_clothing_broad_woven_sash` — a broad $colour woven sash
- `earlymodern_ryukyu_clothing_woven_fibre_sandals` — a pair of $colour woven fibre sandals
- `earlymodern_ryukyu_clothing_tied_headcloth` — a tied $colour cotton headcloth
- `renaissance_seasia_short_maritime_trousers` — a pair of short full $colour cotton trousers

#### Outfit 192 — Ryukyuan coastal fisherman male

- `renaissance_ryukyu_short_wrapjacket` — a short $colour cotton tropical wrap jacket
- `renaissance_seasia_short_maritime_trousers` — a pair of short full $colour cotton trousers
- `earlymodern_ryukyu_clothing_broad_woven_sash` — a broad $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 193 — Ryukyuan urban artisan male

- `renaissance_ryukyu_widesleeve_summerrobe` — a broad-sleeved $colour ramie tropical robe
- `renaissance_ryukyu_short_wrapjacket` — a short $colour cotton tropical wrap jacket
- `earlymodern_ryukyu_clothing_broad_woven_sash` — a broad $colour woven sash
- `earlymodern_ryukyu_clothing_woven_fibre_sandals` — a pair of $colour woven fibre sandals
- `earlymodern_ryukyu_clothing_tied_headcloth` — a tied $colour cotton headcloth

#### Outfit 194 — Ryukyuan port merchant male

- `renaissance_ryukyu_widesleeve_summerrobe` — a broad-sleeved $colour ramie tropical robe
- `renaissance_ryukyu_short_wrapjacket` — a short $colour cotton tropical wrap jacket
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_ryukyu_clothing_tied_headcloth` — a tied $colour cotton headcloth
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 195 — Ryukyuan bourgeois clerk male

- `renaissance_ryukyu_widesleeve_summerrobe` — a broad-sleeved $colour ramie tropical robe
- `renaissance_ryukyu_short_wrapjacket` — a short $colour cotton tropical wrap jacket
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_ryukyu_clothing_tied_headcloth` — a tied $colour cotton headcloth
- `renaissance_shared_clothing_travelling_coat` — a loose $colour travelling coat

#### Outfit 196 — Ryukyuan rural field worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_ryukyu_pleated_wrapskirt` — a pleated $colour cotton tropical wrap skirt
- `renaissance_ryukyu_short_wrapjacket` — a short $colour cotton tropical wrap jacket
- `earlymodern_ryukyu_clothing_broad_woven_sash` — a broad $colour woven sash
- `earlymodern_ryukyu_clothing_woven_fibre_sandals` — a pair of $colour woven fibre sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 197 — Ryukyuan rural household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_ryukyu_widesleeve_summerrobe` — a broad-sleeved $colour ramie tropical robe
- `earlymodern_ryukyu_clothing_broad_woven_sash` — a broad $colour woven sash
- `earlymodern_ryukyu_clothing_woven_fibre_sandals` — a pair of $colour woven fibre sandals
- `earlymodern_ryukyu_clothing_tied_headcloth` — a tied $colour cotton headcloth
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 198 — Ryukyuan urban textile worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_ryukyu_pleated_wrapskirt` — a pleated $colour cotton tropical wrap skirt
- `renaissance_ryukyu_short_wrapjacket` — a short $colour cotton tropical wrap jacket
- `earlymodern_ryukyu_clothing_broad_woven_sash` — a broad $colour woven sash
- `earlymodern_ryukyu_clothing_woven_fibre_sandals` — a pair of $colour woven fibre sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `earlymodern_seasia_clothing_light_shoulder_cloth` — a light $colour cotton shoulder cloth

#### Outfit 199 — Ryukyuan urban market trader female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_ryukyu_pleated_wrapskirt` — a pleated $colour cotton tropical wrap skirt
- `renaissance_ryukyu_short_wrapjacket` — a short $colour cotton tropical wrap jacket
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_ryukyu_clothing_tied_headcloth` — a tied $colour cotton headcloth

#### Outfit 200 — Ryukyuan prosperous merchant household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_ryukyu_widesleeve_summerrobe` — a broad-sleeved $colour ramie tropical robe
- `renaissance_ryukyu_short_wrapjacket` — a short $colour cotton tropical wrap jacket
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_ryukyu_clothing_tied_headcloth` — a tied $colour cotton headcloth
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

### 21. Mainland South-east Asian courts

> Mainland South-east Asian civilian clothing, c. 1600-1750, stripped of court regalia despite the master label. The outfits cover rice cultivation, river work, crafts, markets, shops, and literate urban households.

#### Outfit 201 — Mainland South-east Asian rice farmer male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_seasia_pleated_courtwaistcloth` — a pleated $colour silk waistcloth
- `renaissance_seasia_short_collarless_jacket` — a short collarless $colour cotton jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_seasia_palmleaf_conicalhat` — a broad conical fibre hat

#### Outfit 202 — Mainland South-east Asian river worker male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_seasia_split_riding_waistcloth` — a divided $colour cotton riding waistcloth
- `renaissance_seasia_short_collarless_jacket` — a short collarless $colour cotton jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_seasia_palmleaf_conicalhat` — a broad conical fibre hat
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 203 — Mainland South-east Asian urban artisan male

- `renaissance_seasia_short_maritime_trousers` — a pair of short full $colour cotton trousers
- `renaissance_seasia_short_collarless_jacket` — a short collarless $colour cotton jacket
- `renaissance_seasia_sleeveless_courtvest` — a long sleeveless $colour silk vest
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 204 — Mainland South-east Asian urban merchant male

- `renaissance_seasia_split_riding_waistcloth` — a divided $colour cotton riding waistcloth
- `renaissance_seasia_long_crossfront_courtrobe` — a long cross-front $colour silk robe
- `renaissance_seasia_sleeveless_courtvest` — a long sleeveless $colour silk vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_seasia_clothing_wrapped_headcloth` — a wrapped $colour cotton headcloth
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 205 — Mainland South-east Asian bourgeois clerk male

- `renaissance_seasia_short_maritime_trousers` — a pair of short full $colour cotton trousers
- `renaissance_seasia_long_crossfront_courtrobe` — a long cross-front $colour silk robe
- `renaissance_seasia_sleeveless_courtvest` — a long sleeveless $colour silk vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_seasia_clothing_wrapped_headcloth` — a wrapped $colour cotton headcloth

#### Outfit 206 — Mainland South-east Asian rural field worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_seasia_sewn_tubeskirt` — a sewn $colour cotton tubular skirt
- `renaissance_seasia_short_collarless_jacket` — a short collarless $colour cotton jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_seasia_palmleaf_conicalhat` — a broad conical fibre hat

#### Outfit 207 — Mainland South-east Asian rural householder female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_seasia_sewn_tubeskirt` — a sewn $colour cotton tubular skirt
- `renaissance_seasia_short_collarless_jacket` — a short collarless $colour cotton jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `earlymodern_seasia_clothing_light_shoulder_cloth` — a light $colour cotton shoulder cloth

#### Outfit 208 — Mainland South-east Asian urban craftswoman female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_seasia_sewn_tubeskirt` — a sewn $colour cotton tubular skirt
- `renaissance_seasia_short_collarless_jacket` — a short collarless $colour cotton jacket
- `renaissance_seasia_sleeveless_courtvest` — a long sleeveless $colour silk vest
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 209 — Mainland South-east Asian market trader female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_seasia_pleated_courtwaistcloth` — a pleated $colour silk waistcloth
- `renaissance_seasia_short_collarless_jacket` — a short collarless $colour cotton jacket
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_seasia_clothing_wrapped_headcloth` — a wrapped $colour cotton headcloth
- `earlymodern_seasia_clothing_light_shoulder_cloth` — a light $colour cotton shoulder cloth

#### Outfit 210 — Mainland South-east Asian prosperous merchant household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_seasia_sewn_tubeskirt` — a sewn $colour cotton tubular skirt
- `renaissance_seasia_long_crossfront_courtrobe` — a long cross-front $colour silk robe
- `renaissance_seasia_sleeveless_courtvest` — a long sleeveless $colour silk vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_seasia_clothing_wrapped_headcloth` — a wrapped $colour cotton headcloth
- `earlymodern_seasia_clothing_light_shoulder_cloth` — a light $colour cotton shoulder cloth

### 22. Maritime South-east Asian trade worlds

> Maritime South-east Asian civilian clothing, c. 1600-1750, centered on spice cultivation, fishing, port crafts, retail, and mercantile households. Military and dynastic court clothing are excluded.

#### Outfit 211 — Maritime South-east Asian spice cultivator male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `earlymodern_maritimeseasia_clothing_patterned_sarong` — a $colour1 and $colour2 patterned cotton sarong
- `earlymodern_maritimeseasia_clothing_loose_long_tunic` — a loose $colour cotton long tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_seasia_palmleaf_conicalhat` — a broad conical fibre hat

#### Outfit 212 — Maritime South-east Asian fisherman male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_seasia_short_maritime_trousers` — a pair of short full $colour cotton trousers
- `renaissance_seasia_short_collarless_jacket` — a short collarless $colour cotton jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 213 — Maritime South-east Asian port artisan male

- `earlymodern_maritimeseasia_clothing_patterned_sarong` — a $colour1 and $colour2 patterned cotton sarong
- `earlymodern_maritimeseasia_clothing_loose_long_tunic` — a loose $colour cotton long tunic
- `renaissance_seasia_sleeveless_courtvest` — a long sleeveless $colour silk vest
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 214 — Maritime South-east Asian port merchant male

- `earlymodern_maritimeseasia_clothing_patterned_sarong` — a $colour1 and $colour2 patterned cotton sarong
- `earlymodern_maritimeseasia_clothing_loose_long_tunic` — a loose $colour cotton long tunic
- `renaissance_seasia_long_crossfront_courtrobe` — a long cross-front $colour silk robe
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_seasia_clothing_wrapped_headcloth` — a wrapped $colour cotton headcloth
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 215 — Maritime South-east Asian bourgeois scribe male

- `renaissance_seasia_short_maritime_trousers` — a pair of short full $colour cotton trousers
- `earlymodern_maritimeseasia_clothing_loose_long_tunic` — a loose $colour cotton long tunic
- `renaissance_seasia_long_crossfront_courtrobe` — a long cross-front $colour silk robe
- `renaissance_seasia_sleeveless_courtvest` — a long sleeveless $colour silk vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_seasia_clothing_wrapped_headcloth` — a wrapped $colour cotton headcloth

#### Outfit 216 — Maritime South-east Asian rural field worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `earlymodern_maritimeseasia_clothing_patterned_sarong` — a $colour1 and $colour2 patterned cotton sarong
- `earlymodern_maritimeseasia_clothing_light_open_blouse` — a light $colour cotton open blouse
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_seasia_palmleaf_conicalhat` — a broad conical fibre hat

#### Outfit 217 — Maritime South-east Asian fishing household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_seasia_sewn_tubeskirt` — a sewn $colour cotton tubular skirt
- `renaissance_seasia_short_collarless_jacket` — a short collarless $colour cotton jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `earlymodern_seasia_clothing_light_shoulder_cloth` — a light $colour cotton shoulder cloth

#### Outfit 218 — Maritime South-east Asian urban textile worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `earlymodern_maritimeseasia_clothing_patterned_sarong` — a $colour1 and $colour2 patterned cotton sarong
- `earlymodern_maritimeseasia_clothing_light_open_blouse` — a light $colour cotton open blouse
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 219 — Maritime South-east Asian market trader female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `earlymodern_maritimeseasia_clothing_patterned_sarong` — a $colour1 and $colour2 patterned cotton sarong
- `earlymodern_maritimeseasia_clothing_light_open_blouse` — a light $colour cotton open blouse
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_seasia_clothing_wrapped_headcloth` — a wrapped $colour cotton headcloth
- `earlymodern_seasia_clothing_light_shoulder_cloth` — a light $colour cotton shoulder cloth

#### Outfit 220 — Maritime South-east Asian prosperous merchant household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_seasia_sewn_tubeskirt` — a sewn $colour cotton tubular skirt
- `earlymodern_maritimeseasia_clothing_light_open_blouse` — a light $colour cotton open blouse
- `renaissance_seasia_sleeveless_courtvest` — a long sleeveless $colour silk vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_seasia_clothing_wrapped_headcloth` — a wrapped $colour cotton headcloth
- `earlymodern_seasia_clothing_light_shoulder_cloth` — a light $colour cotton shoulder cloth

### 23. Inner Asian / steppe frontier

> Inner Asian and steppe-frontier civilian dress, c. 1600-1750, covering pastoral, caravan, felt-working, market, and merchant households. Military and princely attire are excluded.

#### Outfit 221 — Steppe rural herder male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_steppe_wide_ridingtrousers` — a pair of wide $colour wool riding trousers
- `renaissance_steppe_short_split_ridingcoat` — a short split-skirt $colour riding coat
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_steppe_soft_highboots` — a pair of soft high leather boots
- `renaissance_steppe_pointed_feltcap` — a pointed $colour felt riding cap

#### Outfit 222 — Steppe winter pastoralist male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_steppe_wide_ridingtrousers` — a pair of wide $colour wool riding trousers
- `renaissance_steppe_quilted_ridingrobe` — a quilted side-fastened $colour riding robe
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_steppe_soft_highboots` — a pair of soft high leather boots
- `renaissance_steppe_fur_earflaphat` — a furred ear-flap hat
- `renaissance_shared_clothing_mittens` — a pair of $colour wool mittens

#### Outfit 223 — Steppe caravan worker male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_steppe_wide_ridingtrousers` — a pair of wide $colour wool riding trousers
- `renaissance_steppe_sleeveless_ridingvest` — a sleeveless leather riding vest
- `renaissance_steppe_sidefastened_furcoat` — a side-fastened $colour fur-lined long coat
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_steppe_soft_highboots` — a pair of soft high leather boots
- `renaissance_steppe_pointed_feltcap` — a pointed $colour felt riding cap

#### Outfit 224 — Steppe town artisan male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_steppe_wide_ridingtrousers` — a pair of wide $colour wool riding trousers
- `renaissance_steppe_short_split_ridingcoat` — a short split-skirt $colour riding coat
- `renaissance_steppe_sleeveless_ridingvest` — a sleeveless leather riding vest
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_steppe_soft_highboots` — a pair of soft high leather boots
- `renaissance_steppe_pointed_feltcap` — a pointed $colour felt riding cap

#### Outfit 225 — Steppe prosperous caravan merchant male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_steppe_wide_ridingtrousers` — a pair of wide $colour wool riding trousers
- `renaissance_steppe_sidefastened_furcoat` — a side-fastened $colour fur-lined long coat
- `renaissance_steppe_sleeveless_ridingvest` — a sleeveless leather riding vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_steppe_soft_highboots` — a pair of soft high leather boots
- `renaissance_steppe_fur_earflaphat` — a furred ear-flap hat
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

#### Outfit 226 — Steppe rural herder female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_shared_clothing_full_trousers` — a pair of full $colour cotton trousers
- `renaissance_steppe_short_split_ridingcoat` — a short split-skirt $colour riding coat
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_steppe_soft_highboots` — a pair of soft high leather boots
- `renaissance_steppe_pointed_feltcap` — a pointed $colour felt riding cap

#### Outfit 227 — Steppe winter household female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_shared_clothing_full_trousers` — a pair of full $colour cotton trousers
- `renaissance_steppe_quilted_ridingrobe` — a quilted side-fastened $colour riding robe
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_steppe_soft_highboots` — a pair of soft high leather boots
- `renaissance_steppe_fur_earflaphat` — a furred ear-flap hat
- `renaissance_shared_clothing_mittens` — a pair of $colour wool mittens

#### Outfit 228 — Steppe felt worker female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_shared_clothing_full_trousers` — a pair of full $colour cotton trousers
- `renaissance_steppe_sleeveless_ridingvest` — a sleeveless leather riding vest
- `renaissance_steppe_short_split_ridingcoat` — a short split-skirt $colour riding coat
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_steppe_soft_highboots` — a pair of soft high leather boots
- `renaissance_steppe_pointed_feltcap` — a pointed $colour felt riding cap

#### Outfit 229 — Steppe market trader female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_shared_clothing_full_trousers` — a pair of full $colour cotton trousers
- `renaissance_steppe_sidefastened_furcoat` — a side-fastened $colour fur-lined long coat
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_steppe_soft_highboots` — a pair of soft high leather boots
- `renaissance_steppe_pointed_feltcap` — a pointed $colour felt riding cap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 230 — Steppe prosperous merchant household female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_shared_clothing_full_trousers` — a pair of full $colour cotton trousers
- `renaissance_steppe_sidefastened_furcoat` — a side-fastened $colour fur-lined long coat
- `renaissance_steppe_sleeveless_ridingvest` — a sleeveless leather riding vest
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_steppe_soft_highboots` — a pair of soft high leather boots
- `renaissance_steppe_fur_earflaphat` — a furred ear-flap hat
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 24. West African court and Atlantic trade

> West African civilian and commercial clothing, c. 1600-1750. The master label includes court networks, but this pass retains only rural, artisan, market, merchant, and prosperous household dress; every implementation still requires a narrower local culture.

#### Outfit 231 — West African rural farmer male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_africancourt_shortsleeve_tunic` — a short-sleeved $colour straight cotton tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_africancourt_soft_embroideredcap` — a soft $colour embroidered round cap
- `renaissance_shared_clothing_rectangular_mantle` — a $colour rectangular shoulder mantle

#### Outfit 232 — West African rural palm worker male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_africancourt_narrow_waistwrapper` — a narrow $colour cotton waist wrapper
- `renaissance_africancourt_sleeveless_straighttunic` — a sleeveless $colour straight cotton tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 233 — West African urban artisan male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_africancourt_broad_waistwrapper` — a broad full-length $colour cotton waist wrapper
- `renaissance_africancourt_shortsleeve_tunic` — a short-sleeved $colour straight cotton tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_africancourt_soft_embroideredcap` — a soft $colour embroidered round cap

#### Outfit 234 — West African market trader male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_africancourt_broad_waistwrapper` — a broad full-length $colour cotton waist wrapper
- `renaissance_africancourt_longsleeve_sideslit_tunic` — a long-sleeved $colour side-slit tunic
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_africancourt_soft_embroideredcap` — a soft $colour embroidered round cap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 235 — West African prosperous merchant male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_shared_clothing_full_trousers` — a pair of full $colour cotton trousers
- `renaissance_africancourt_longsleeve_sideslit_tunic` — a long-sleeved $colour side-slit tunic
- `earlymodern_westcentralafrica_clothing_short_tradecloth_coat` — a short $colour broadcloth coat
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `renaissance_africancourt_soft_embroideredcap` — a soft $colour embroidered round cap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 236 — West African rural field worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_narrow_waistwrapper` — a narrow $colour cotton waist wrapper
- `earlymodern_africanatlantic_clothing_short_cotton_blouse` — a short $colour cotton blouse
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `earlymodern_africanatlantic_clothing_full_headwrap` — a full $colour cotton headwrap

#### Outfit 237 — West African rural household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_sewn_tubewrapper` — a sewn $colour cotton tubular wrapper
- `renaissance_africancourt_shortsleeve_tunic` — a short-sleeved $colour straight cotton tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `earlymodern_africanatlantic_clothing_full_headwrap` — a full $colour cotton headwrap
- `renaissance_shared_clothing_rectangular_mantle` — a $colour rectangular shoulder mantle

#### Outfit 238 — West African urban textile worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_broad_waistwrapper` — a broad full-length $colour cotton waist wrapper
- `earlymodern_africanatlantic_clothing_short_cotton_blouse` — a short $colour cotton blouse
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `earlymodern_africanatlantic_clothing_full_headwrap` — a full $colour cotton headwrap

#### Outfit 239 — West African market trader female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_sewn_tubewrapper` — a sewn $colour cotton tubular wrapper
- `renaissance_africancourt_longsleeve_sideslit_tunic` — a long-sleeved $colour side-slit tunic
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `earlymodern_africanatlantic_clothing_full_headwrap` — a full $colour cotton headwrap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 240 — West African prosperous merchant household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_broad_waistwrapper` — a broad full-length $colour cotton waist wrapper
- `renaissance_africancourt_longsleeve_sideslit_tunic` — a long-sleeved $colour side-slit tunic
- `earlymodern_westcentralafrica_clothing_short_tradecloth_coat` — a short $colour broadcloth coat
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_africanatlantic_clothing_full_headwrap` — a full $colour cotton headwrap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

### 25. Kongo / Angola / West Central Africa

> West Central African civilian clothing, c. 1600-1750, deliberately distinguishing local barkcloth and wrapper continuities from adopted trade-cloth forms. Legal status is not inferred from clothing; local manifests must record it separately.

#### Outfit 241 — West Central African rural cultivator male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_africancourt_barkcloth_straighttunic` — a straight $colour barkcloth tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `renaissance_shared_clothing_rectangular_mantle` — a $colour rectangular shoulder mantle

#### Outfit 242 — West Central African rural palm worker male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_africancourt_barkcloth_wrapskirt` — a broad $colour barkcloth wrap skirt
- `renaissance_africancourt_sleeveless_straighttunic` — a sleeveless $colour straight cotton tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 243 — West Central African urban artisan male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_africancourt_broad_waistwrapper` — a broad full-length $colour cotton waist wrapper
- `earlymodern_westcentralafrica_clothing_tradecloth_shirt` — a loose $colour cotton trade-cloth shirt
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_africancourt_soft_embroideredcap` — a soft $colour embroidered round cap

#### Outfit 244 — West Central African port trader male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `earlymodern_westcentralafrica_clothing_tradecloth_shirt` — a loose $colour cotton trade-cloth shirt
- `earlymodern_westcentralafrica_clothing_short_tradecloth_coat` — a short $colour broadcloth coat
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat

#### Outfit 245 — West Central African bourgeois interpreter male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_westcentralafrica_clothing_short_tradecloth_coat` — a short $colour broadcloth coat
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat

#### Outfit 246 — West Central African rural field worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_barkcloth_wrapskirt` — a broad $colour barkcloth wrap skirt
- `earlymodern_africanatlantic_clothing_short_cotton_blouse` — a short $colour cotton blouse
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `earlymodern_africanatlantic_clothing_full_headwrap` — a full $colour cotton headwrap

#### Outfit 247 — West Central African rural household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_sewn_tubewrapper` — a sewn $colour cotton tubular wrapper
- `renaissance_africancourt_barkcloth_straighttunic` — a straight $colour barkcloth tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `earlymodern_africanatlantic_clothing_full_headwrap` — a full $colour cotton headwrap

#### Outfit 248 — West Central African urban craftswoman female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_broad_waistwrapper` — a broad full-length $colour cotton waist wrapper
- `earlymodern_africanatlantic_clothing_short_cotton_blouse` — a short $colour cotton blouse
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `earlymodern_africanatlantic_clothing_full_headwrap` — a full $colour cotton headwrap

#### Outfit 249 — West Central African market trader female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_colonial_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap

#### Outfit 250 — West Central African prosperous merchant household female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_colonial_clothing_short_wool_jacket` — a short $colour wool jacket
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap

### 26. Sahelian / Hausa / Islamic West Africa

> Sahelian and Hausa-facing civilian clothing, c. 1600-1750, covering cultivation, herding, urban crafts, market exchange, scholarship, and prosperous merchant households. Every outfit requires narrower local admission.

#### Outfit 251 — Sahelian rural cultivator male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_sahel_narrow_longtunic` — a narrow long $colour cotton tunic
- `renaissance_shared_clothing_full_trousers` — a pair of full $colour cotton trousers
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 252 — Sahelian rural herder male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_sahel_veryfull_trousers` — a pair of very full $colour cotton trousers
- `renaissance_sahel_narrow_longtunic` — a narrow long $colour cotton tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_ankle_boots` — a pair of ankle leather boots
- `renaissance_sahel_conical_leatherhat` — a conical leather riding hat
- `renaissance_shared_clothing_rectangular_mantle` — a $colour rectangular shoulder mantle

#### Outfit 253 — Sahelian urban artisan male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_sahel_veryfull_trousers` — a pair of very full $colour cotton trousers
- `renaissance_sahel_narrow_longtunic` — a narrow long $colour cotton tunic
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_africancourt_soft_embroideredcap` — a soft $colour embroidered round cap

#### Outfit 254 — Sahelian urban merchant male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_sahel_veryfull_trousers` — a pair of very full $colour cotton trousers
- `renaissance_sahel_broad_rectangular_robe` — a broad rectangular $colour cotton over-robe
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `renaissance_sahel_scholar_turban` — a layered $colour cotton turban
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 255 — Sahelian learned townsman male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_sahel_veryfull_trousers` — a pair of very full $colour cotton trousers
- `renaissance_sahel_embroidered_necktunic` — a broad-necked $colour cotton tunic
- `renaissance_sahel_broad_rectangular_robe` — a broad rectangular $colour cotton over-robe
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `renaissance_sahel_scholar_turban` — a layered $colour cotton turban

#### Outfit 256 — Sahelian rural field worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_narrow_waistwrapper` — a narrow $colour cotton waist wrapper
- `earlymodern_africanatlantic_clothing_short_cotton_blouse` — a short $colour cotton blouse
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 257 — Sahelian rural household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_broad_waistwrapper` — a broad full-length $colour cotton waist wrapper
- `renaissance_sahel_narrow_longtunic` — a narrow long $colour cotton tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `renaissance_shared_clothing_rectangular_mantle` — a $colour rectangular shoulder mantle

#### Outfit 258 — Sahelian urban textile worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_sewn_tubewrapper` — a sewn $colour cotton tubular wrapper
- `renaissance_sahel_narrow_longtunic` — a narrow long $colour cotton tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 259 — Sahelian market trader female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_broad_waistwrapper` — a broad full-length $colour cotton waist wrapper
- `renaissance_sahel_embroidered_necktunic` — a broad-necked $colour cotton tunic
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 260 — Sahelian prosperous merchant household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_sahel_veryfull_trousers` — a pair of very full $colour cotton trousers
- `renaissance_sahel_broad_rectangular_robe` — a broad rectangular $colour cotton over-robe
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

### 27. Ethiopian / Red Sea

> Ethiopian highland and Red Sea civilian clothing, c. 1600-1750, spanning cultivators, herders, artisans, merchants, scribes, and prosperous households. Ecclesiastical and military attire are outside this pass.

#### Outfit 261 — Ethiopian rural cultivator male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_redsea_narrow_cotton_tunic` — a narrow long $colour cotton tunic
- `earlymodern_redsea_clothing_narrow_cotton_trousers` — a pair of narrow $colour cotton trousers
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `renaissance_redsea_full_shoulderwrap` — a full $colour cotton body-and-shoulder wrap

#### Outfit 262 — Ethiopian highland herder male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `earlymodern_redsea_clothing_narrow_cotton_trousers` — a pair of narrow $colour cotton trousers
- `renaissance_redsea_narrow_cotton_tunic` — a narrow long $colour cotton tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_ankle_boots` — a pair of ankle leather boots
- `renaissance_redsea_leather_highlandcloak` — a broad leather highland cloak
- `renaissance_redsea_white_headhood` — a white head-draped cotton hood

#### Outfit 263 — Red Sea urban artisan male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `earlymodern_redsea_clothing_narrow_cotton_trousers` — a pair of narrow $colour cotton trousers
- `renaissance_redsea_narrow_cotton_tunic` — a narrow long $colour cotton tunic
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 264 — Red Sea urban merchant male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `earlymodern_redsea_clothing_narrow_cotton_trousers` — a pair of narrow $colour cotton trousers
- `renaissance_redsea_long_courtshirt` — a long full-sleeved $colour cotton shirt
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `renaissance_redsea_full_shoulderwrap` — a full $colour cotton body-and-shoulder wrap

#### Outfit 265 — Ethiopian bourgeois scribe male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `earlymodern_redsea_clothing_narrow_cotton_trousers` — a pair of narrow $colour cotton trousers
- `renaissance_redsea_long_courtshirt` — a long full-sleeved $colour cotton shirt
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `renaissance_redsea_white_headhood` — a white head-draped cotton hood
- `renaissance_redsea_embroidered_shouldercape` — an embroidered $colour cotton shoulder cape

#### Outfit 266 — Ethiopian rural field worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_redsea_full_shoulderwrap` — a full $colour cotton body-and-shoulder wrap
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 267 — Ethiopian highland household female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_redsea_full_shoulderwrap` — a full $colour cotton body-and-shoulder wrap
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_ankle_boots` — a pair of ankle leather boots
- `renaissance_redsea_white_headhood` — a white head-draped cotton hood
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 268 — Red Sea urban textile worker female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `earlymodern_redsea_clothing_narrow_cotton_trousers` — a pair of narrow $colour cotton trousers
- `renaissance_redsea_narrow_cotton_tunic` — a narrow long $colour cotton tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 269 — Red Sea market trader female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_redsea_full_shoulderwrap` — a full $colour cotton body-and-shoulder wrap
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `renaissance_redsea_embroidered_shouldercape` — an embroidered $colour cotton shoulder cape

#### Outfit 270 — Red Sea prosperous merchant household female

- `renaissance_shared_clothing_straight_underrobe` — a $colour straight under-robe
- `renaissance_redsea_long_courtshirt` — a long full-sleeved $colour cotton shirt
- `renaissance_redsea_full_shoulderwrap` — a full $colour cotton body-and-shoulder wrap
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `renaissance_redsea_white_headhood` — a white head-draped cotton hood
- `renaissance_redsea_embroidered_shouldercape` — an embroidered $colour cotton shoulder cape

### 28. Swahili Coast / Indian Ocean Africa

> Swahili and Indian Ocean African civilian clothing, c. 1600-1750, covering coastal agriculture, fishing, port crafts, retail, scholarship, and merchant households. Court and military clothing are excluded.

#### Outfit 271 — Swahili rural coastal farmer male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_indianocean_short_coastaltunic` — a short loose $colour cotton coastal tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `renaissance_indianocean_embroidered_roundcap` — a close $colour embroidered round cap
- `renaissance_shared_clothing_rectangular_mantle` — a $colour rectangular shoulder mantle

#### Outfit 272 — Swahili coastal fisherman male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_seasia_short_maritime_trousers` — a pair of short full $colour cotton trousers
- `renaissance_indianocean_short_coastaltunic` — a short loose $colour cotton coastal tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_indianocean_toeloop_sandals` — a pair of toe-loop leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 273 — Swahili port artisan male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_shared_clothing_full_trousers` — a pair of full $colour cotton trousers
- `renaissance_indianocean_short_coastaltunic` — a short loose $colour cotton coastal tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_indianocean_toeloop_sandals` — a pair of toe-loop leather sandals
- `renaissance_indianocean_embroidered_roundcap` — a close $colour embroidered round cap

#### Outfit 274 — Swahili port merchant male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_shared_clothing_full_trousers` — a pair of full $colour cotton trousers
- `renaissance_indianocean_long_coastalrobe` — a long collarless $colour cotton coastal robe
- `renaissance_indianocean_open_merchantcoat` — a light open-front $colour cotton merchant coat
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `renaissance_indianocean_embroidered_roundcap` — a close $colour embroidered round cap

#### Outfit 275 — Swahili bourgeois clerk male

- `renaissance_shared_clothing_long_undershirt` — a $colour long undershirt
- `renaissance_shared_clothing_full_trousers` — a pair of full $colour cotton trousers
- `renaissance_indianocean_long_coastalrobe` — a long collarless $colour cotton coastal robe
- `renaissance_indianocean_open_merchantcoat` — a light open-front $colour cotton merchant coat
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `renaissance_indianocean_embroidered_roundcap` — a close $colour embroidered round cap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 276 — Swahili rural field worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_indianocean_sewn_wrapskirt` — a sewn $colour cotton coastal wrap skirt
- `renaissance_indianocean_short_coastaltunic` — a short loose $colour cotton coastal tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_indianocean_toeloop_sandals` — a pair of toe-loop leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap

#### Outfit 277 — Swahili fishing household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_indianocean_sewn_wrapskirt` — a sewn $colour cotton coastal wrap skirt
- `renaissance_indianocean_short_coastaltunic` — a short loose $colour cotton coastal tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_indianocean_toeloop_sandals` — a pair of toe-loop leather sandals
- `renaissance_shared_clothing_cloth_headwrap` — a long $colour cloth headwrap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 278 — Swahili urban textile worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_indianocean_sewn_wrapskirt` — a sewn $colour cotton coastal wrap skirt
- `renaissance_indianocean_short_coastaltunic` — a short loose $colour cotton coastal tunic
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `earlymodern_indianocean_clothing_light_cotton_head_veil` — a light $colour cotton head veil

#### Outfit 279 — Swahili market trader female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_indianocean_sewn_wrapskirt` — a sewn $colour cotton coastal wrap skirt
- `renaissance_indianocean_long_coastalrobe` — a long collarless $colour cotton coastal robe
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_indianocean_toeloop_sandals` — a pair of toe-loop leather sandals
- `earlymodern_indianocean_clothing_light_cotton_head_veil` — a light $colour cotton head veil
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 280 — Swahili prosperous merchant household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_indianocean_sewn_wrapskirt` — a sewn $colour cotton coastal wrap skirt
- `renaissance_indianocean_long_coastalrobe` — a long collarless $colour cotton coastal robe
- `renaissance_indianocean_open_merchantcoat` — a light open-front $colour cotton merchant coat
- `earlymodern_shared_clothing_broad_wrapped_sash` — a broad $colour wrapped sash
- `renaissance_shared_clothing_soft_slippers` — a pair of soft leather slippers
- `earlymodern_indianocean_clothing_light_cotton_head_veil` — a light $colour cotton head veil
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

### 29. Spanish colonial Americas

> Spanish-colonial civilian clothing, c. 1600-1750. The manifests mix local Indigenous continuities, locally made Iberian forms, and genuine hybrid garments; each implementation must identify region, community, and whether a form is local, imported, imposed, or hybrid.

#### Outfit 281 — Spanish colonial rural smallholder male

- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_iberian_clothing_woven_fibre_sandals` — a pair of $colour woven fibre sandals
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `earlymodern_iberian_clothing_broad_crowned_felt_hat` — a broad-crowned $colour felt hat
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap

#### Outfit 282 — Spanish colonial rural muleteer male

- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_colonial_clothing_loose_canvas_trousers` — a pair of loose $colour canvas trousers
- `earlymodern_colonial_clothing_short_wool_jacket` — a short $colour wool jacket
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_ankle_boots` — a pair of ankle leather boots
- `earlymodern_iberian_clothing_broad_crowned_felt_hat` — a broad-crowned $colour felt hat
- `earlymodern_andeancolonial_clothing_wool_poncho` — a $colour wool poncho

#### Outfit 283 — Spanish colonial urban artisan male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_iberian_clothing_broad_crowned_felt_hat` — a broad-crowned $colour felt hat

#### Outfit 284 — Spanish colonial urban shopkeeper male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_italian_clothing_light_short_coat` — a light $colour short coat
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_iberian_clothing_broad_crowned_felt_hat` — a broad-crowned $colour felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 285 — Spanish colonial bourgeois notary male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_western_clothing_long_broadcloth_coat` — a long $colour broadcloth coat
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 286 — Spanish colonial rural Indigenous-continuity female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_mesoamerican_long_wrapskirt` — a long $colour cotton wrap skirt
- `renaissance_mesoamerican_rectangular_blouse` — a rectangular sleeveless $colour cotton blouse
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap
- `renaissance_mesoamerican_woven_fibresandals` — a pair of $colour woven fibre sandals
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap

#### Outfit 287 — Spanish colonial rural household female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_andeancolonial_clothing_full_wool_pollera` — a full gathered $colour wool skirt
- `earlymodern_colonial_clothing_linen_work_apron` — a $colour linen work apron
- `earlymodern_iberian_clothing_woven_fibre_sandals` — a pair of $colour woven fibre sandals
- `earlymodern_andeancolonial_clothing_shaped_wool_hat` — a shaped $colour wool hat
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap

#### Outfit 288 — Spanish colonial urban textile worker female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_colonial_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap

#### Outfit 289 — Spanish colonial market trader female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_andeancolonial_clothing_full_wool_pollera` — a full gathered $colour wool skirt
- `earlymodern_colonial_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_andeancolonial_clothing_shaped_wool_hat` — a shaped $colour wool hat
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap

#### Outfit 290 — Spanish colonial bourgeois merchant household female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_plain_mantua_gown` — a plain $colour mantua gown
- `earlymodern_andeancolonial_clothing_full_wool_pollera` — a full gathered $colour wool skirt
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `earlymodern_iberian_clothing_lace_head_veil` — a lace-edged $colour head veil
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 30. Portuguese Brazil / Atlantic plantation

> Brazilian and Portuguese-Atlantic civilian clothing, c. 1600-1750, with local, African-diasporic, Indigenous, and Portuguese-derived forms. Clothing does not encode free, enslaved, Indigenous, African, European, or mixed legal identity; local manifests must record those contexts explicitly.

#### Outfit 291 — Brazilian rural provision farmer male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_caribbean_sleeveless_cottontunic` — a loose sleeveless $colour cotton tunic
- `earlymodern_colonial_clothing_loose_canvas_trousers` — a pair of loose $colour canvas trousers
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `earlymodern_colonial_clothing_broad_felt_sunhat` — a broad-brimmed $colour felt hat

#### Outfit 292 — Brazilian cane-field worker male

- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_colonial_clothing_loose_canvas_trousers` — a pair of loose $colour canvas trousers
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `earlymodern_colonial_clothing_broad_felt_sunhat` — a broad-brimmed $colour felt hat
- `renaissance_shared_clothing_neck_kerchief` — a tied $colour linen neck kerchief

#### Outfit 293 — Brazilian urban artisan male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_colonial_clothing_short_wool_jacket` — a short $colour wool jacket
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_iberian_clothing_broad_crowned_felt_hat` — a broad-crowned $colour felt hat

#### Outfit 294 — Brazilian market trader male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_africancourt_broad_waistwrapper` — a broad full-length $colour cotton waist wrapper
- `earlymodern_westcentralafrica_clothing_tradecloth_shirt` — a loose $colour cotton trade-cloth shirt
- `earlymodern_colonial_clothing_short_wool_jacket` — a short $colour wool jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap

#### Outfit 295 — Brazilian bourgeois merchant clerk male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_italian_clothing_light_short_coat` — a light $colour short coat
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat

#### Outfit 296 — Brazilian rural garden worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_caribbean_cotton_wrapskirt` — a short $colour cotton wrap skirt
- `renaissance_caribbean_sleeveless_cottontunic` — a loose sleeveless $colour cotton tunic
- `earlymodern_colonial_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap

#### Outfit 297 — Brazilian laundry worker female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_colonial_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap

#### Outfit 298 — Brazilian market vendor female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_sewn_tubewrapper` — a sewn $colour cotton tubular wrapper
- `earlymodern_africanatlantic_clothing_short_cotton_blouse` — a short $colour cotton blouse
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `earlymodern_africanatlantic_clothing_full_headwrap` — a full $colour cotton headwrap

#### Outfit 299 — Brazilian urban craftswoman female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_colonial_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap

#### Outfit 300 — Brazilian bourgeois shopkeeping female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_plain_mantua_gown` — a plain $colour mantua gown
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `earlymodern_iberian_clothing_lace_head_veil` — a lace-edged $colour head veil
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 31. English / French / Dutch colonial North America

> Colonial North American civilian dress, c. 1600-1750, for farming, frontier labour, crafts, shops, clerical work, and merchant households. Imported, locally made, and Indigenous-derived pieces require explicit local admission.

#### Outfit 301 — Colonial North American settler farmer male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_colonial_clothing_short_wool_jacket` — a short $colour wool jacket
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `earlymodern_colonial_clothing_broad_felt_sunhat` — a broad-brimmed $colour felt hat

#### Outfit 302 — Colonial North American frontier labourer male

- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_colonial_clothing_loose_canvas_trousers` — a pair of loose $colour canvas trousers
- `earlymodern_colonialnorthamerica_clothing_hooded_wool_capote` — a hooded $colour wool capote
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_colonialnorthamerica_clothing_soft_moccasins` — a pair of soft leather moccasins
- `earlymodern_colonialnorthamerica_clothing_linen_work_cap` — a $colour linen work cap
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

#### Outfit 303 — Colonial North American urban artisan male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat

#### Outfit 304 — Colonial North American shopkeeper male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_western_clothing_long_broadcloth_coat` — a long $colour broadcloth coat
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_western_clothing_cocked_felt_hat` — a $colour cocked felt hat

#### Outfit 305 — Colonial North American bourgeois clerk male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_western_clothing_fine_broadcloth_waistcoat` — a fine $colour broadcloth waistcoat
- `earlymodern_french_clothing_long_skirted_coat` — a long-skirted $colour broadcloth coat
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat
- `earlymodern_western_clothing_linen_neckcloth` — a $colour linen neckcloth

#### Outfit 306 — Colonial North American farm worker female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_colonial_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_colonialnorthamerica_clothing_linen_work_cap` — a $colour linen work cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief

#### Outfit 307 — Colonial North American rural householder female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_colonialnorthamerica_clothing_wool_shortgown` — a $colour wool shortgown
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_british_clothing_checked_linen_apron` — a $colour1 and $colour2 checked linen apron
- `earlymodern_colonialnorthamerica_clothing_soft_moccasins` — a pair of soft leather moccasins
- `earlymodern_colonialnorthamerica_clothing_linen_work_cap` — a $colour linen work cap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 308 — Colonial North American urban craftswoman female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_colonialnorthamerica_clothing_wool_shortgown` — a $colour wool shortgown
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_colonial_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_colonialnorthamerica_clothing_linen_work_cap` — a $colour linen work cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief

#### Outfit 309 — Colonial North American market trader female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_british_clothing_checked_linen_apron` — a $colour1 and $colour2 checked linen apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_western_clothing_wool_bonnet` — a $colour wool bonnet
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `earlymodern_colonialnorthamerica_clothing_hooded_wool_capote` — a hooded $colour wool capote

#### Outfit 310 — Colonial North American bourgeois merchant household female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_plain_mantua_gown` — a plain $colour mantua gown
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `earlymodern_colonialnorthamerica_clothing_linen_work_cap` — a $colour linen work cap
- `earlymodern_western_clothing_linen_shoulder_kerchief` — a $colour linen shoulder kerchief
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 32. Indigenous North American regional families

> A deliberately broad placeholder for region-specific Indigenous North American catalogues, c. 1600-1750. Every outfit must be narrowed by local culture, ecology, season, and contact history before implementation; this set avoids treating the continent as one dress system.

#### Outfit 311 — Indigenous North American warm-season farmer male

- `renaissance_northamerican_hide_breechcloth` — a long hide breechcloth
- `renaissance_northamerican_paired_hideleggings` — a pair of long hide leggings
- `renaissance_northamerican_hide_shirt` — a loose hide shirt
- `renaissance_northamerican_soft_moccasins` — a pair of soft hide moccasins
- `earlymodern_northamerican_clothing_tied_tradecloth_headband` — a tied $colour wool headband
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt

#### Outfit 312 — Indigenous North American cold-season hunter male

- `renaissance_northamerican_hide_breechcloth` — a long hide breechcloth
- `renaissance_northamerican_paired_hideleggings` — a pair of long hide leggings
- `renaissance_northamerican_hide_shirt` — a loose hide shirt
- `renaissance_northamerican_fur_robe` — a broad fur robe
- `renaissance_northamerican_soft_moccasins` — a pair of soft hide moccasins
- `earlymodern_northamerican_clothing_tied_tradecloth_headband` — a tied $colour wool headband
- `renaissance_shared_clothing_mittens` — a pair of $colour wool mittens

#### Outfit 313 — Indigenous North American hideworker male

- `renaissance_northamerican_hide_breechcloth` — a long hide breechcloth
- `renaissance_northamerican_paired_hideleggings` — a pair of long hide leggings
- `renaissance_northamerican_hide_shirt` — a loose hide shirt
- `renaissance_northamerican_soft_moccasins` — a pair of soft hide moccasins
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `earlymodern_northamerican_clothing_tied_tradecloth_headband` — a tied $colour wool headband
- `renaissance_shared_clothing_rectangular_mantle` — a $colour rectangular shoulder mantle

#### Outfit 314 — Indigenous North American trade intermediary male

- `renaissance_northamerican_hide_breechcloth` — a long hide breechcloth
- `renaissance_northamerican_paired_hideleggings` — a pair of long hide leggings
- `earlymodern_northamerican_clothing_tradecloth_shirt` — a loose $colour wool trade-cloth shirt
- `earlymodern_northamerican_clothing_wrapped_blanket_coat` — a wrapped $colour wool blanket coat
- `renaissance_northamerican_soft_moccasins` — a pair of soft hide moccasins
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_northamerican_clothing_tied_tradecloth_headband` — a tied $colour wool headband

#### Outfit 315 — Indigenous North American prosperous trader male

- `renaissance_northamerican_hide_breechcloth` — a long hide breechcloth
- `renaissance_northamerican_paired_hideleggings` — a pair of long hide leggings
- `earlymodern_northamerican_clothing_tradecloth_shirt` — a loose $colour wool trade-cloth shirt
- `earlymodern_northamerican_clothing_wrapped_blanket_coat` — a wrapped $colour wool blanket coat
- `renaissance_northamerican_soft_moccasins` — a pair of soft hide moccasins
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat

#### Outfit 316 — Indigenous North American warm-season farmer female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_northamerican_hide_wrapdress` — a full hide wrap dress
- `renaissance_northamerican_soft_moccasins` — a pair of soft hide moccasins
- `earlymodern_northamerican_clothing_tied_tradecloth_headband` — a tied $colour wool headband
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt

#### Outfit 317 — Indigenous North American hideworker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_northamerican_hide_wrapdress` — a full hide wrap dress
- `renaissance_northamerican_paired_hideleggings` — a pair of long hide leggings
- `renaissance_northamerican_soft_moccasins` — a pair of soft hide moccasins
- `earlymodern_northamerican_clothing_tied_tradecloth_headband` — a tied $colour wool headband
- `renaissance_shared_clothing_rectangular_mantle` — a $colour rectangular shoulder mantle

#### Outfit 318 — Indigenous North American cold-season householder female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_northamerican_hide_wrapdress` — a full hide wrap dress
- `renaissance_northamerican_paired_hideleggings` — a pair of long hide leggings
- `renaissance_northamerican_fur_robe` — a broad fur robe
- `renaissance_northamerican_soft_moccasins` — a pair of soft hide moccasins
- `earlymodern_northamerican_clothing_tied_tradecloth_headband` — a tied $colour wool headband
- `renaissance_shared_clothing_mittens` — a pair of $colour wool mittens

#### Outfit 319 — Indigenous North American market trader female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `earlymodern_northamerican_clothing_tradecloth_wrap_skirt` — a $colour wool trade-cloth wrap skirt
- `earlymodern_northamerican_clothing_tradecloth_shirt` — a loose $colour wool trade-cloth shirt
- `renaissance_northamerican_soft_moccasins` — a pair of soft hide moccasins
- `earlymodern_northamerican_clothing_tied_tradecloth_headband` — a tied $colour wool headband
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 320 — Indigenous North American prosperous trade household female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `earlymodern_northamerican_clothing_tradecloth_wrap_skirt` — a $colour wool trade-cloth wrap skirt
- `earlymodern_northamerican_clothing_tradecloth_shirt` — a loose $colour wool trade-cloth shirt
- `earlymodern_northamerican_clothing_wrapped_blanket_coat` — a wrapped $colour wool blanket coat
- `renaissance_northamerican_soft_moccasins` — a pair of soft hide moccasins
- `earlymodern_northamerican_clothing_tied_tradecloth_headband` — a tied $colour wool headband
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

### 33. Mesoamerican colonial and Indigenous

> Mesoamerican colonial and Indigenous civilian clothing, c. 1600-1750. Local continuities, Spanish-derived pieces, mission-associated dress, and genuine hybrids are separated through outfit notes rather than presented as a single linear progression.

#### Outfit 321 — Mesoamerican rural cultivator male

- `renaissance_mesoamerican_panelled_loincloth` — a long panelled $colour cotton loin garment
- `renaissance_mesoamerican_side_seamed_tunic` — a side-seamed sleeveless $colour cotton tunic
- `renaissance_mesoamerican_shouldertied_cloak` — a shoulder-tied $colour cotton rectangular cloak
- `renaissance_mesoamerican_woven_fibresandals` — a pair of $colour woven fibre sandals
- `renaissance_mesoamerican_woven_headcap` — a close $colour woven fibre cap
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt

#### Outfit 322 — Mesoamerican rural market carrier male

- `renaissance_mesoamerican_panelled_loincloth` — a long panelled $colour cotton loin garment
- `renaissance_caribbean_sleeveless_cottontunic` — a loose sleeveless $colour cotton tunic
- `renaissance_mesoamerican_shouldertied_cloak` — a shoulder-tied $colour cotton rectangular cloak
- `renaissance_mesoamerican_woven_fibresandals` — a pair of $colour woven fibre sandals
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash

#### Outfit 323 — Mesoamerican town artisan male

- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_colonial_clothing_loose_canvas_trousers` — a pair of loose $colour canvas trousers
- `earlymodern_colonial_clothing_short_wool_jacket` — a short $colour wool jacket
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `renaissance_mesoamerican_woven_fibresandals` — a pair of $colour woven fibre sandals
- `earlymodern_colonial_clothing_broad_felt_sunhat` — a broad-brimmed $colour felt hat

#### Outfit 324 — Mesoamerican prosperous market trader male

- `renaissance_mesoamerican_panelled_loincloth` — a long panelled $colour cotton loin garment
- `renaissance_mesoamerican_side_seamed_tunic` — a side-seamed sleeveless $colour cotton tunic
- `earlymodern_colonial_clothing_short_wool_jacket` — a short $colour wool jacket
- `renaissance_mesoamerican_shouldertied_cloak` — a shoulder-tied $colour cotton rectangular cloak
- `renaissance_mesoamerican_woven_fibresandals` — a pair of $colour woven fibre sandals
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt

#### Outfit 325 — Mesoamerican bourgeois municipal clerk male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat

#### Outfit 326 — Mesoamerican rural field worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_mesoamerican_short_wrapskirt` — a short $colour cotton wrap skirt
- `renaissance_mesoamerican_rectangular_blouse` — a rectangular sleeveless $colour cotton blouse
- `renaissance_mesoamerican_woven_fibresandals` — a pair of $colour woven fibre sandals
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash

#### Outfit 327 — Mesoamerican rural weaver female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_mesoamerican_long_wrapskirt` — a long $colour cotton wrap skirt
- `renaissance_mesoamerican_rectangular_blouse` — a rectangular sleeveless $colour cotton blouse
- `renaissance_mesoamerican_triangle_shouldergarment` — a triangular $colour cotton shoulder garment
- `renaissance_mesoamerican_woven_fibresandals` — a pair of $colour woven fibre sandals
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap

#### Outfit 328 — Mesoamerican urban market trader female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_mesoamerican_long_wrapskirt` — a long $colour cotton wrap skirt
- `renaissance_mesoamerican_rectangular_blouse` — a rectangular sleeveless $colour cotton blouse
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap
- `renaissance_mesoamerican_woven_fibresandals` — a pair of $colour woven fibre sandals
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap

#### Outfit 329 — Mesoamerican town craftswoman female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_colonial_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_mesoamerican_woven_fibresandals` — a pair of $colour woven fibre sandals
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap

#### Outfit 330 — Mesoamerican prosperous merchant household female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_andeancolonial_clothing_full_wool_pollera` — a full gathered $colour wool skirt
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap

### 34. Andean colonial and Indigenous

> Andean colonial and Indigenous civilian clothing, c. 1600-1750, pairing camelid-textile continuities with locally adopted colonial forms. Each implementation must record community, altitude, season, and contact context.

#### Outfit 331 — Andean highland cultivator male

- `renaissance_andean_short_worktunic` — a short coarse $colour camelid-wool tunic
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footwraps` — a pair of $colour cloth footwraps
- `renaissance_andean_braided_fibresandals` — a pair of $colour braided fibre sandals
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `renaissance_andean_woven_earflapcap` — a woven $colour camelid-wool ear-flap cap
- `earlymodern_andeancolonial_clothing_wool_poncho` — a $colour wool poncho

#### Outfit 332 — Andean rural herder male

- `renaissance_andean_straight_sleevelesstunic` — a straight sleeveless $colour camelid-wool tunic
- `renaissance_shared_clothing_straight_trousers` — a pair of $colour straight wool trousers
- `renaissance_shared_clothing_footwraps` — a pair of $colour cloth footwraps
- `renaissance_andean_braided_fibresandals` — a pair of $colour braided fibre sandals
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_andean_woven_earflapcap` — a woven $colour camelid-wool ear-flap cap
- `renaissance_andean_fringed_camelidshawl` — a fringed $colour camelid-wool shawl

#### Outfit 333 — Andean town artisan male

- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_colonial_clothing_loose_canvas_trousers` — a pair of loose $colour canvas trousers
- `earlymodern_colonial_clothing_short_wool_jacket` — a short $colour wool jacket
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_andean_braided_fibresandals` — a pair of $colour braided fibre sandals
- `earlymodern_andeancolonial_clothing_shaped_wool_hat` — a shaped $colour wool hat
- `earlymodern_andeancolonial_clothing_wool_poncho` — a $colour wool poncho

#### Outfit 334 — Andean market trader male

- `renaissance_andean_straight_sleevelesstunic` — a straight sleeveless $colour camelid-wool tunic
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footwraps` — a pair of $colour cloth footwraps
- `renaissance_andean_braided_fibresandals` — a pair of $colour braided fibre sandals
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `earlymodern_andeancolonial_clothing_shaped_wool_hat` — a shaped $colour wool hat
- `renaissance_andean_paired_shouldermantle` — a broad pinned $colour camelid-wool shoulder mantle

#### Outfit 335 — Andean bourgeois municipal clerk male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `earlymodern_andeancolonial_clothing_shaped_wool_hat` — a shaped $colour wool hat
- `earlymodern_andeancolonial_clothing_wool_poncho` — a $colour wool poncho

#### Outfit 336 — Andean rural field worker female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_andean_wrapped_fulllengthdress` — a wrapped full-length $colour camelid-wool dress
- `renaissance_andean_paired_shouldermantle` — a broad pinned $colour camelid-wool shoulder mantle
- `renaissance_andean_braided_fibresandals` — a pair of $colour braided fibre sandals
- `renaissance_andean_long_headcloth` — a long $colour camelid-wool headcloth
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash

#### Outfit 337 — Andean rural weaver female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_andean_pinned_wrapskirt` — a pinned broad $colour camelid-wool wrap skirt
- `renaissance_mesoamerican_rectangular_blouse` — a rectangular sleeveless $colour cotton blouse
- `renaissance_andean_fringed_camelidshawl` — a fringed $colour camelid-wool shawl
- `renaissance_andean_braided_fibresandals` — a pair of $colour braided fibre sandals
- `renaissance_andean_long_headcloth` — a long $colour camelid-wool headcloth

#### Outfit 338 — Andean urban market trader female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_andeancolonial_clothing_full_wool_pollera` — a full gathered $colour wool skirt
- `earlymodern_colonial_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_andean_braided_fibresandals` — a pair of $colour braided fibre sandals
- `earlymodern_andeancolonial_clothing_shaped_wool_hat` — a shaped $colour wool hat
- `renaissance_andean_paired_shouldermantle` — a broad pinned $colour camelid-wool shoulder mantle

#### Outfit 339 — Andean town craftswoman female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_andean_wrapped_fulllengthdress` — a wrapped full-length $colour camelid-wool dress
- `earlymodern_colonial_clothing_short_wool_jacket` — a short $colour wool jacket
- `renaissance_andean_fringed_camelidshawl` — a fringed $colour camelid-wool shawl
- `renaissance_andean_braided_fibresandals` — a pair of $colour braided fibre sandals
- `earlymodern_andeancolonial_clothing_shaped_wool_hat` — a shaped $colour wool hat

#### Outfit 340 — Andean prosperous merchant household female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_andeancolonial_clothing_full_wool_pollera` — a full gathered $colour wool skirt
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_andeancolonial_clothing_shaped_wool_hat` — a shaped $colour wool hat
- `renaissance_andean_fringed_camelidshawl` — a fringed $colour camelid-wool shawl
- `earlymodern_andeancolonial_clothing_wool_poncho` — a $colour wool poncho

### 35. Caribbean / Atlantic plantation

> Caribbean and Atlantic plantation-zone civilian clothing, c. 1600-1750, including local Caribbean, African-diasporic, and European-derived forms. Clothing does not encode legal status; local manifests must explicitly record free, enslaved, Indigenous, African, European, and mixed contexts.

#### Outfit 341 — Caribbean rural provision farmer male

- `renaissance_caribbean_cotton_loinapron` — a panelled $colour cotton loin apron
- `renaissance_caribbean_sleeveless_cottontunic` — a loose sleeveless $colour cotton tunic
- `renaissance_caribbean_woven_fibresandals` — a pair of $colour woven fibre sandals
- `earlymodern_colonial_clothing_broad_felt_sunhat` — a broad-brimmed $colour felt hat
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_caribbean_woven_shouldercape` — a short $colour cotton shoulder cape

#### Outfit 342 — Caribbean cane-field worker male

- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_colonial_clothing_loose_canvas_trousers` — a pair of loose $colour canvas trousers
- `preindustrial_clothing_plain_leather_belt` — a plain leather belt
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `earlymodern_colonial_clothing_broad_felt_sunhat` — a broad-brimmed $colour felt hat
- `renaissance_shared_clothing_neck_kerchief` — a tied $colour linen neck kerchief

#### Outfit 343 — Caribbean dockside artisan male

- `renaissance_shared_clothing_wrapped_loincloth` — a $colour wrapped loincloth
- `renaissance_africancourt_broad_waistwrapper` — a broad full-length $colour cotton waist wrapper
- `earlymodern_westcentralafrica_clothing_tradecloth_shirt` — a loose $colour cotton trade-cloth shirt
- `earlymodern_colonial_clothing_short_wool_jacket` — a short $colour wool jacket
- `preindustrial_clothing_simple_woven_sash` — a $colour woven sash
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap

#### Outfit 344 — Caribbean market trader male

- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat
- `renaissance_caribbean_woven_shouldercape` — a short $colour cotton shoulder cape

#### Outfit 345 — Caribbean bourgeois clerk male

- `renaissance_shared_clothing_drawstring_drawers` — a pair of $colour loose drawstring drawers
- `earlymodern_colonial_clothing_plain_linen_shirt` — a plain $colour linen shirt
- `earlymodern_western_clothing_plain_wool_waistcoat` — a plain $colour wool waistcoat
- `earlymodern_italian_clothing_light_short_coat` — a light $colour short coat
- `earlymodern_colonial_clothing_plain_knee_breeches` — a pair of plain $colour knee breeches
- `renaissance_shared_clothing_footed_stockings` — a pair of $colour footed wool stockings
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `preindustrial_clothing_iron_buckled_leather_belt` — an iron-buckled leather belt
- `renaissance_shared_clothing_felt_brimmed_hat` — a $colour brimmed felt hat

#### Outfit 346 — Caribbean rural provision farmer female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_caribbean_cotton_wrapskirt` — a short $colour cotton wrap skirt
- `renaissance_caribbean_sleeveless_cottontunic` — a loose sleeveless $colour cotton tunic
- `renaissance_caribbean_woven_fibresandals` — a pair of $colour woven fibre sandals
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap
- `renaissance_caribbean_woven_shouldercape` — a short $colour cotton shoulder cape

#### Outfit 347 — Caribbean laundry worker female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_colonial_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_textile_sandals` — a pair of $colour woven sandals
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap

#### Outfit 348 — Caribbean market vendor female

- `renaissance_shared_clothing_breast_wrap` — a $colour folded breast wrap
- `renaissance_africancourt_sewn_tubewrapper` — a sewn $colour cotton tubular wrapper
- `earlymodern_africanatlantic_clothing_short_cotton_blouse` — a short $colour cotton blouse
- `renaissance_shared_clothing_leather_sandals` — a pair of leather sandals
- `earlymodern_africanatlantic_clothing_full_headwrap` — a full $colour cotton headwrap
- `renaissance_shared_clothing_shoulder_shawl` — a broad $colour wool shoulder shawl

#### Outfit 349 — Caribbean urban craftswoman female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_colonial_clothing_plain_wool_bodice` — a plain $colour wool bodice
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_colonial_clothing_linen_work_apron` — a $colour linen work apron
- `renaissance_shared_clothing_low_leather_shoes` — a pair of low leather shoes
- `earlymodern_colonial_clothing_cotton_headwrap` — a full $colour cotton headwrap
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap

#### Outfit 350 — Caribbean prosperous shopkeeping female

- `earlymodern_colonial_clothing_long_linen_shift` — a long $colour linen shift
- `earlymodern_western_clothing_canvas_stays` — a pair of $colour canvas stays
- `earlymodern_western_clothing_plain_mantua_gown` — a plain $colour mantua gown
- `earlymodern_colonial_clothing_full_cotton_skirt` — a full $colour cotton skirt
- `earlymodern_western_clothing_fine_linen_apron` — a fine $colour linen apron
- `earlymodern_western_clothing_buckled_leather_shoes` — a pair of buckled leather shoes
- `earlymodern_iberian_clothing_lace_head_veil` — a lace-edged $colour head veil
- `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` — a long $colour cotton shoulder wrap
- `renaissance_shared_clothing_leather_gloves` — a pair of leather gloves

## Item catalogue

This catalogue contains the **317 stable references actually used by the first-pass manifests**. Full descriptions are deferred. Usage count records how many of the 350 manifests include the reference.

Rows marked as cross-era admissions are not duplicate Early Modern prototypes. Their source references should be ensured idempotently when the Early Modern branch is installed.

### 1. Cross-era Renaissance admissions — 32

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_shared_clothing_drawstring_drawers` | a pair of $colour loose drawstring drawers | `drawers` | `linen` | `Small` / `Standard` | 140g / 7.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shorts`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 52 outfit(s). |
| `renaissance_shared_clothing_low_leather_shoes` | a pair of low leather shoes | `shoes` | `leather` | `Small` / `Standard` | 650g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 54 outfit(s). |
| `renaissance_shared_clothing_soft_brimless_cap` | a soft $colour brimless cap | `cap` | `wool` | `Small` / `Standard` | 190g / 13.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_shared_clothing_footed_stockings` | a pair of $colour footed wool stockings | `stockings` | `wool` | `Small` / `Standard` | 280g / 13.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Stockings`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 39 outfit(s). |
| `renaissance_shared_clothing_felt_brimmed_hat` | a $colour brimmed felt hat | `hat` | `felt` | `Small` / `Standard` | 170g / 12.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 21 outfit(s). |
| `renaissance_shared_clothing_leather_gloves` | a pair of leather gloves | `gloves` | `leather` | `Small` / `Standard` | 220g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Gloves`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 22 outfit(s). |
| `renaissance_shared_clothing_neck_kerchief` | a tied $colour linen neck kerchief | `kerchief` | `linen` | `Small` / `Standard` | 90g / 7.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Scarf`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 8 outfit(s). |
| `renaissance_shared_clothing_straight_trousers` | a pair of $colour straight wool trousers | `trousers` | `wool` | `Normal` / `Standard` | 620g / 22.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_shared_clothing_ankle_boots` | a pair of ankle leather boots | `boots` | `leather` | `Small` / `Standard` | 950g / 51.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Boots`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 13 outfit(s). |
| `renaissance_shared_clothing_gathered_skirt` | a gathered $colour wool skirt | `skirt` | `wool` | `Normal` / `Standard` | 770g / 28.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_shared_clothing_short_head_veil` | a short $colour head veil | `veil` | `linen` | `Small` / `Standard` | 120g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Head_Veil`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_shared_clothing_footwraps` | a pair of $colour cloth footwraps | `footwraps` | `linen` | `Small` / `Standard` | 200g / 12.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Stockings`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_shared_clothing_high_riding_boots` | a pair of high leather boots | `boots` | `leather` | `Small` / `Good` | 1220g / 151.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_High_Boots`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_shared_clothing_rain_cape` | a close-woven $colour rain cape | `cape` | `wool` | `Normal` / `Standard` | 1260g / 50.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Cloak_(Closed)`<br>`Armour_HeavyClothing`<br>`Insulation_Strong`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_shared_clothing_shoulder_shawl` | a broad $colour wool shoulder shawl | `shawl` | `wool` | `Normal` / `Standard` | 680g / 29.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 46 outfit(s). |
| `renaissance_shared_clothing_long_undershirt` | a $colour long undershirt | `undershirt` | `linen` | `Normal` / `Standard` | 260g / 14.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 26 outfit(s). |
| `renaissance_shared_clothing_rectangular_mantle` | a $colour rectangular shoulder mantle | `mantle` | `wool` | `Normal` / `Standard` | 680g / 29.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 10 outfit(s). |
| `renaissance_shared_clothing_long_head_veil` | a long $colour head veil | `veil` | `linen` | `Small` / `Standard` | 120g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Head_Veil`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 18 outfit(s). |
| `renaissance_shared_clothing_straight_underrobe` | a $colour straight under-robe | `under-robe` | `linen` | `Normal` / `Standard` | 780g / 46.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long-Sleeved_Gown`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 32 outfit(s). Mapped to a dedicated under-gown layer so it can sit beneath robes and coats. |
| `renaissance_shared_clothing_mittens` | a pair of $colour wool mittens | `mittens` | `wool` | `Small` / `Standard` | 240g / 13.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mittens`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |
| `renaissance_shared_clothing_travelling_coat` | a loose $colour travelling coat | `coat` | `broadcloth` | `Normal` / `Standard` | 1340g / 94.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_shared_clothing_fur_lined_coat` | a $colour fur-lined travelling coat | `coat` | `wool` | `Normal` / `Good` | 1230g / 127.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_shared_clothing_leather_sandals` | a pair of leather sandals | `sandals` | `leather` | `Small` / `Standard` | 410g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 49 outfit(s). |
| `renaissance_shared_clothing_cloth_headwrap` | a long $colour cloth headwrap | `headwrap` | `cotton` | `Small` / `Standard` | 220g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Turban`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 49 outfit(s). |
| `renaissance_shared_clothing_soft_slippers` | a pair of soft leather slippers | `slippers` | `leather` | `Small` / `Standard` | 650g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 24 outfit(s). |
| `renaissance_shared_clothing_full_trousers` | a pair of full $colour cotton trousers | `trousers` | `cotton` | `Normal` / `Standard` | 500g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 13 outfit(s). |
| `renaissance_shared_clothing_wrap_jacket` | a $colour short wrap jacket | `jacket` | `linen` | `Normal` / `Standard` | 520g / 30.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_shared_clothing_breast_wrap` | a $colour folded breast wrap | `wrap` | `linen` | `Small` / `Standard` | 80g / 6.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Bra`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 65 outfit(s). |
| `renaissance_shared_clothing_wrapped_loincloth` | a $colour wrapped loincloth | `loincloth` | `linen` | `Small` / `Standard` | 100g / 5.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Loincloth`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 21 outfit(s). |
| `renaissance_shared_clothing_short_undershirt` | a $colour short undershirt | `undershirt` | `linen` | `Normal` / `Standard` | 260g / 14.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_shared_clothing_textile_sandals` | a pair of $colour woven sandals | `sandals` | `canvas` | `Small` / `Standard` | 370g / 11.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 24 outfit(s). |
| `renaissance_shared_clothing_close_cloth_cap` | a close-fitting $colour cloth cap | `cap` | `linen` | `Small` / `Standard` | 140g / 12.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |

### 2. Shared Western European Early Modern — 22

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_western_clothing_plain_linen_shirt` | a $colour plain linen shirt | `shirt` | `linen` | `Normal` / `Standard` | 260g / 14.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 38 outfit(s). |
| `earlymodern_western_clothing_broadcloth_work_coat` | a $colour broadcloth work coat | `coat` | `broadcloth` | `Normal` / `Standard` | 1500g / 105.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Coat`<br>`Armour_HeavyClothing`<br>`Insulation_Strong`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 3 outfit(s). |
| `earlymodern_western_clothing_knee_breeches` | a pair of $colour knee breeches | `breeches` | `broadcloth` | `Normal` / `Standard` | 560g / 41.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Breeches`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 27 outfit(s). Uses planned Wear_Breeches dependency. |
| `earlymodern_western_clothing_knitted_wool_stockings` | a pair of $colour knitted wool stockings | `stockings` | `wool` | `Small` / `Standard` | 280g / 13.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Stockings`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 11 outfit(s). |
| `earlymodern_western_clothing_plain_wool_waistcoat` | a plain $colour wool waistcoat | `waistcoat` | `wool` | `Normal` / `Standard` | 400g / 22.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Vest`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 22 outfit(s). |
| `earlymodern_western_clothing_buckled_leather_shoes` | a pair of buckled leather shoes | `shoes` | `leather` | `Small` / `Good` | 650g / 69.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 32 outfit(s). |
| `earlymodern_western_clothing_full_wool_cloak` | a full $colour wool cloak | `cloak` | `wool` | `Normal` / `Standard` | 1260g / 50.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Cloak_(Closed)`<br>`Armour_HeavyClothing`<br>`Insulation_Strong`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 9 outfit(s). |
| `earlymodern_western_clothing_linen_neckcloth` | a $colour linen neckcloth | `neckcloth` | `linen` | `Small` / `Standard` | 90g / 7.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Scarf`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 21 outfit(s). |
| `earlymodern_western_clothing_linen_day_cap` | a $colour linen day cap | `cap` | `linen` | `Small` / `Standard` | 140g / 12.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 6 outfit(s). |
| `earlymodern_western_clothing_fine_broadcloth_waistcoat` | a fine $colour broadcloth waistcoat | `waistcoat` | `broadcloth` | `Normal` / `Good` | 430g / 71.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Vest`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 18 outfit(s). |
| `earlymodern_western_clothing_cocked_felt_hat` | a $colour cocked felt hat | `hat` | `felt` | `Small` / `Good` | 170g / 25.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 5 outfit(s). |
| `earlymodern_western_clothing_long_broadcloth_coat` | a long $colour broadcloth coat | `coat` | `broadcloth` | `Normal` / `Good` | 1500g / 221.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Coat`<br>`Armour_HeavyClothing`<br>`Insulation_Strong`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 8 outfit(s). |
| `earlymodern_western_clothing_long_linen_shift` | a long $colour linen shift | `shift` | `linen` | `Normal` / `Standard` | 740g / 42.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Gown`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 37 outfit(s). |
| `earlymodern_western_clothing_canvas_stays` | a pair of $colour canvas stays | `stays` | `canvas` | `Normal` / `Good` | 690g / 71.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Stays`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 48 outfit(s). Uses planned Wear_Stays dependency. |
| `earlymodern_western_clothing_fitted_wool_bodice` | a fitted $colour wool bodice | `bodice` | `wool` | `Normal` / `Standard` | 620g / 35.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Doublet`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 16 outfit(s). |
| `earlymodern_western_clothing_full_wool_petticoat` | a full $colour wool petticoat | `petticoat` | `wool` | `Normal` / `Standard` | 770g / 28.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 20 outfit(s). |
| `earlymodern_western_clothing_linen_work_apron` | a $colour linen work apron | `apron` | `linen` | `Small` / `Standard` | 210g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Apron`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 19 outfit(s). |
| `earlymodern_western_clothing_linen_shoulder_kerchief` | a $colour linen shoulder kerchief | `kerchief` | `linen` | `Normal` / `Standard` | 500g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 24 outfit(s). |
| `earlymodern_western_clothing_fine_linen_apron` | a fine $colour linen apron | `apron` | `linen` | `Small` / `Good` | 210g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Apron`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 21 outfit(s). |
| `earlymodern_western_clothing_plain_mantua_gown` | a plain $colour mantua gown | `gown` | `broadcloth` | `Normal` / `Good` | 1340g / 196.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 13 outfit(s). |
| `earlymodern_western_clothing_plain_leather_mules` | a pair of plain leather mules | `mules` | `leather` | `Small` / `Standard` | 650g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 1 outfit(s). |
| `earlymodern_western_clothing_wool_bonnet` | a $colour wool bonnet | `bonnet` | `wool` | `Small` / `Standard` | 190g / 13.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 5 outfit(s). |

### 3. Implemented shared pre-industrial accessories — 3

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `preindustrial_clothing_plain_leather_belt` | a plain leather belt | `belt` | `leather` | `Small` / `Standard` | 180g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Waist`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Belt_2` | `Era / Pre-Industrial Era`<br>`Market / Clothing / Standard Clothing` | Implemented pre-industrial alias; used in 19 outfit(s). Implemented shared pre-industrial dependency. |
| `preindustrial_clothing_iron_buckled_leather_belt` | an iron-buckled leather belt | `belt` | `leather` | `Small` / `Good` | 240g / 30.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Waist`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Belt_4` | `Era / Pre-Industrial Era`<br>`Market / Clothing / Standard Clothing` | Implemented pre-industrial alias; used in 51 outfit(s). Implemented shared pre-industrial dependency. |
| `preindustrial_clothing_simple_woven_sash` | a $colour woven sash | `sash` | `wool` | `Small` / `Standard` | 120g / 6.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sash`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Pre-Industrial Era`<br>`Market / Clothing / Standard Clothing` | Implemented pre-industrial alias; used in 96 outfit(s). Implemented shared pre-industrial dependency. |

### 4. French / Baroque urban — 3

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_french_clothing_long_skirted_coat` | a long-skirted $colour broadcloth coat | `coat` | `broadcloth` | `Normal` / `Good` | 1500g / 221.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Coat`<br>`Armour_HeavyClothing`<br>`Insulation_Strong`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 5 outfit(s). |
| `earlymodern_french_clothing_short_casaquin_jacket` | a short $colour flared jacket | `jacket` | `broadcloth` | `Normal` / `Good` | 780g / 107.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 1 outfit(s). |
| `earlymodern_french_clothing_lace_trimmed_day_cap` | a lace-trimmed linen day cap | `cap` | `linen` | `Small` / `Good` | 140g / 25.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 3 outfit(s). |

### 5. English / British Stuart-Georgian — 4

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_british_clothing_checked_linen_apron` | a $colour1 and $colour2 checked linen apron | `apron` | `linen` | `Small` / `Standard` | 210g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Apron`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_2BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 10 outfit(s). |
| `earlymodern_british_clothing_linen_smock_frock` | a loose $colour linen smock-frock | `smock-frock` | `linen` | `Normal` / `Standard` | 480g / 22.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long-Sleeved_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 1 outfit(s). |
| `earlymodern_british_clothing_linen_mob_cap` | a gathered $colour linen mob cap | `cap` | `linen` | `Small` / `Standard` | 140g / 12.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). |
| `earlymodern_british_clothing_wool_shortgown` | a $colour wool shortgown | `shortgown` | `wool` | `Normal` / `Standard` | 720g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 3 outfit(s). |

### 6. Dutch Republic / Low Countries — 6

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_dutch_clothing_short_wool_work_jacket` | a short $colour wool work jacket | `jacket` | `wool` | `Normal` / `Standard` | 720g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |
| `earlymodern_dutch_clothing_full_canvas_breeches` | a pair of full $colour canvas breeches | `breeches` | `canvas` | `Normal` / `Standard` | 530g / 19.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Breeches`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). Uses planned Wear_Breeches dependency. |
| `earlymodern_dutch_clothing_wooden_clogs` | a pair of plain wooden clogs | `clogs` | `wood` | `Small` / `Standard` | 700g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 3 outfit(s). |
| `earlymodern_dutch_clothing_white_linen_cap` | a crisp white linen cap | `cap` | `linen` | `Small` / `Standard` | 140g / 12.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 7 outfit(s). |
| `earlymodern_dutch_clothing_striped_wool_petticoat` | a $colour1 and $colour2 striped wool petticoat | `petticoat` | `wool` | `Normal` / `Standard` | 770g / 28.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_2BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 3 outfit(s). |
| `earlymodern_dutch_clothing_short_wool_overjacket` | a $colour short wool over-jacket | `jacket` | `wool` | `Normal` / `Good` | 720g / 69.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 2 outfit(s). |

### 7. Iberian / Portuguese-Spanish empires — 5

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_iberian_clothing_short_fitted_jacket` | a short fitted $colour wool jacket | `jacket` | `wool` | `Normal` / `Standard` | 720g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |
| `earlymodern_iberian_clothing_woven_fibre_sandals` | a pair of $colour woven fibre sandals | `sandals` | `canvas` | `Small` / `Standard` | 370g / 11.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 6 outfit(s). |
| `earlymodern_iberian_clothing_broad_crowned_felt_hat` | a broad-crowned $colour felt hat | `hat` | `felt` | `Small` / `Standard` | 170g / 12.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 10 outfit(s). |
| `earlymodern_iberian_clothing_short_wool_cape` | a short $colour wool cape | `cape` | `wool` | `Normal` / `Standard` | 720g / 31.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Cape`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 1 outfit(s). |
| `earlymodern_iberian_clothing_lace_head_veil` | a lace-edged $colour head veil | `veil` | `lace` | `Small` / `Good` | 80g / 67.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Head_Veil`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 8 outfit(s). |

### 8. Italian states — 5

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_italian_clothing_light_short_coat` | a light $colour short coat | `coat` | `broadcloth` | `Normal` / `Good` | 780g / 107.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 8 outfit(s). |
| `earlymodern_italian_clothing_fine_silk_shawl` | a fine $colour silk shawl | `shawl` | `silk` | `Normal` / `Good` | 400g / 175.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 7 outfit(s). |
| `earlymodern_italian_clothing_striped_cotton_petticoat` | a $colour1 and $colour2 striped cotton petticoat | `petticoat` | `cotton` | `Normal` / `Standard` | 630g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_2BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 6 outfit(s). |
| `earlymodern_italian_clothing_soft_backless_shoes` | a pair of soft backless leather shoes | `shoes` | `leather` | `Small` / `Standard` | 650g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 6 outfit(s). |
| `earlymodern_italian_clothing_linen_headcloth` | a $colour linen headcloth | `headcloth` | `linen` | `Small` / `Standard` | 120g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Head_Veil`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). |

### 9. German / HRE / Austrian — 4

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_centraleuropean_clothing_short_wool_coat` | a short $colour wool coat | `coat` | `wool` | `Normal` / `Standard` | 720g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |
| `earlymodern_centraleuropean_clothing_leather_knee_breeches` | a pair of leather knee breeches | `breeches` | `leather` | `Normal` / `Standard` | 590g / 36.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Breeches`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). Uses planned Wear_Breeches dependency. |
| `earlymodern_centraleuropean_clothing_round_felt_hat` | a round-crowned $colour felt hat | `hat` | `felt` | `Small` / `Standard` | 170g / 12.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 7 outfit(s). |
| `earlymodern_centraleuropean_clothing_laced_wool_bodice` | a laced $colour wool bodice | `bodice` | `wool` | `Normal` / `Standard` | 620g / 35.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Doublet`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). |

### 10. Polish-Lithuanian / Hungarian frontier survivals — 2

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_frontier_close_buttoned_longcoat` | a close-buttoned $colour long coat | `coat` | `wool` | `Normal` / `Standard` | 1230g / 61.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_frontier_wide_hungarian_trousers` | a pair of wide $colour wool trousers | `trousers` | `wool` | `Normal` / `Standard` | 620g / 22.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |

### 11. Scandinavian / Baltic — 6

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_northern_clothing_fitted_wool_jacket` | a fitted $colour wool jacket | `jacket` | `wool` | `Normal` / `Standard` | 720g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 3 outfit(s). |
| `earlymodern_northern_clothing_wool_knee_breeches` | a pair of $colour wool knee breeches | `breeches` | `wool` | `Normal` / `Standard` | 520g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Breeches`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). Uses planned Wear_Breeches dependency. |
| `earlymodern_northern_clothing_knitted_wool_cap` | a $colour knitted wool cap | `cap` | `wool` | `Small` / `Standard` | 190g / 13.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 6 outfit(s). |
| `earlymodern_northern_clothing_striped_wool_skirt` | a $colour1 and $colour2 striped wool skirt | `skirt` | `wool` | `Normal` / `Standard` | 770g / 28.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_2BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). |
| `earlymodern_northern_clothing_wool_mittens` | a pair of $colour wool mittens | `mittens` | `wool` | `Small` / `Standard` | 240g / 13.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mittens`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 1 outfit(s). |
| `earlymodern_northern_clothing_fur_lined_wool_coat` | a $colour fur-lined wool coat | `coat` | `wool` | `Normal` / `Good` | 1380g / 143.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Coat`<br>`Armour_HeavyClothing`<br>`Insulation_Strong`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 1 outfit(s). |

### 12. Northern/frontier survivals — 2

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_frontier_knitted_northern_cap` | a long $colour knitted wool cap | `cap` | `wool` | `Small` / `Standard` | 190g / 13.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_frontier_felted_winter_boots` | a pair of felted winter boots | `boots` | `felt` | `Small` / `Standard` | 760g / 34.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Boots`<br>`Armour_LightClothing`<br>`Insulation_Moderate` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |

### 13. Shared Early Modern cross-cultural forms — 1

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_shared_clothing_broad_wrapped_sash` | a broad $colour wrapped sash | `sash` | `cotton` | `Small` / `Standard` | 140g / 8.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sash`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 72 outfit(s). |

### 14. Central/Eastern frontier survivals — 3

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_frontier_fur_brimmed_cap` | a soft $colour fur-brimmed cap | `cap` | `wool` | `Small` / `Standard` | 190g / 13.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_frontier_belted_wool_caftan` | a belted $colour wool caftan | `caftan` | `wool` | `Normal` / `Standard` | 1230g / 61.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_frontier_split_skirt_riding_boots` | a pair of stiff high leather boots | `boots` | `leather` | `Small` / `Good` | 1220g / 151.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_High_Boots`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |

### 15. Russian / frontier survivals — 1

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_frontier_sleeveless_long_gown` | a sleeveless $colour long overgown | `overgown` | `broadcloth` | `Normal` / `Standard` | 1020g / 58.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Dress`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |

### 16. Russian / Petrine and post-Petrine — 8

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_russian_clothing_long_linen_rubakha` | a long $colour linen shirt | `shirt` | `linen` | `Normal` / `Standard` | 480g / 22.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long-Sleeved_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |
| `earlymodern_russian_clothing_narrow_wool_trousers` | a pair of narrow $colour wool trousers | `trousers` | `wool` | `Normal` / `Standard` | 620g / 22.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). |
| `earlymodern_russian_clothing_short_wool_zipun` | a close $colour short wool coat | `coat` | `wool` | `Normal` / `Standard` | 720g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |
| `earlymodern_russian_clothing_high_leather_boots` | a pair of high leather boots | `boots` | `leather` | `Small` / `Standard` | 1220g / 72.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_High_Boots`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 3 outfit(s). |
| `earlymodern_russian_clothing_round_fur_cap` | a round fur cap | `cap` | `fur` | `Small` / `Standard` | 220g / 28.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_HeavyClothing`<br>`Insulation_Strong` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 1 outfit(s). |
| `earlymodern_russian_clothing_checked_wool_wrap_skirt` | a $colour1 and $colour2 checked wool wrap skirt | `skirt` | `wool` | `Normal` / `Standard` | 770g / 28.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_2BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |
| `earlymodern_russian_clothing_linen_work_apron` | a $colour linen work apron | `apron` | `linen` | `Small` / `Standard` | 210g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Apron`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). |
| `earlymodern_russian_clothing_long_linen_head_veil` | a long $colour linen head veil | `veil` | `linen` | `Small` / `Standard` | 120g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Head_Veil`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 5 outfit(s). |

### 17. Russian survivals — 1

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_frontier_muscovite_long_caftan` | a full-skirted $colour long caftan | `caftan` | `broadcloth` | `Normal` / `Good` | 1340g / 196.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |

### 18. Ottoman survivals — 12

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_ottoman_collarless_inner_shirt` | a $colour collarless long inner shirt | `shirt` | `linen` | `Normal` / `Standard` | 260g / 14.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_ottoman_ankle_gathered_drawers` | a pair of full $colour ankle-gathered drawers | `drawers` | `linen` | `Normal` / `Standard` | 450g / 20.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_ottoman_short_entari_coat` | a short fitted $colour front-opening coat | `coat` | `cotton` | `Normal` / `Standard` | 580g / 32.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_ottoman_wrapped_cap_turban` | a compact $colour cap-wound turban | `turban` | `cotton` | `Small` / `Standard` | 220g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Turban`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_ottoman_full_salwar_trousers` | a pair of very full $colour cotton trousers | `trousers` | `cotton` | `Normal` / `Standard` | 500g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 8 outfit(s). |
| `renaissance_ottoman_short_fitted_vest` | a short fitted $colour sleeveless vest | `vest` | `silk` | `Normal` / `Good` | 230g / 134.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Vest`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_ottoman_long_entari_robe` | a long fitted $colour front-opening robe | `robe` | `silk` | `Normal` / `Good` | 580g / 255.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). Treated as the inner robe layer beneath a separate outdoor over-robe. |
| `renaissance_ottoman_pointed_slippers` | a pair of pointed leather slippers | `slippers` | `leather` | `Small` / `Standard` | 650g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |
| `renaissance_ottoman_fitted_kaftan` | a fitted $colour long kaftan | `kaftan` | `silk` | `Normal` / `Good` | 580g / 255.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). Treated as the inner robe layer beneath a separate outdoor over-robe. |
| `renaissance_ottoman_sleeved_outer_robe` | a long-sleeved $colour outer robe | `robe` | `broadcloth` | `Normal` / `Good` | 1340g / 196.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_ottoman_loose_layered_gown` | a loose layered $colour long gown | `gown` | `cotton` | `Normal` / `Standard` | 830g / 44.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Gown`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_ottoman_full_outdoor_overrobe` | a full enveloping $colour outdoor over-robe | `over-robe` | `broadcloth` | `Normal` / `Good` | 1340g / 196.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |

### 19. Maghrebi / North African — 7

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_maghrebi_clothing_full_cotton_trousers` | a pair of full $colour cotton trousers | `trousers` | `cotton` | `Normal` / `Standard` | 500g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 7 outfit(s). |
| `earlymodern_maghrebi_clothing_sleeveless_gandoura` | a long sleeveless $colour cotton robe | `robe` | `cotton` | `Normal` / `Standard` | 320g / 16.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sleeveless_Gown`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 5 outfit(s). |
| `earlymodern_maghrebi_clothing_round_felt_cap` | a round $colour felt cap | `cap` | `felt` | `Small` / `Standard` | 170g / 12.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |
| `earlymodern_maghrebi_clothing_hooded_wool_djellaba` | a hooded $colour wool over-robe | `over-robe` | `wool` | `Normal` / `Standard` | 1230g / 61.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |
| `earlymodern_maghrebi_clothing_pointed_leather_slippers` | a pair of pointed leather slippers | `slippers` | `leather` | `Small` / `Standard` | 650g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 6 outfit(s). |
| `earlymodern_maghrebi_clothing_short_fitted_caftan` | a fitted $colour cotton caftan | `caftan` | `cotton` | `Normal` / `Good` | 1010g / 121.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 4 outfit(s). |
| `earlymodern_maghrebi_clothing_full_body_haik` | a full $colour cotton body wrap | `wrap` | `cotton` | `Normal` / `Standard` | 560g / 27.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 5 outfit(s). |

### 20. Persianate survivals — 10

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_persianate_long_inner_shirt` | a long $colour collarless inner shirt | `shirt` | `linen` | `Normal` / `Standard` | 260g / 14.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_persianate_close_ankle_trousers` | a pair of close $colour ankle trousers | `trousers` | `cotton` | `Normal` / `Standard` | 500g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 9 outfit(s). |
| `renaissance_persianate_short_fitted_coat` | a short fitted $colour front-opening coat | `coat` | `wool` | `Normal` / `Standard` | 720g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_persianate_wide_pleated_trousers` | a pair of wide pleated $colour wool trousers | `trousers` | `wool` | `Normal` / `Standard` | 620g / 22.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_persianate_long_fitted_coat` | a long fitted $colour front-opening coat | `coat` | `broadcloth` | `Normal` / `Good` | 1340g / 196.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_persianate_soft_ridingboots` | a pair of soft high leather boots | `boots` | `leather` | `Small` / `Standard` | 1220g / 72.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_High_Boots`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_persianate_sleeveless_longvest` | a sleeveless $colour long vest | `vest` | `wool` | `Normal` / `Standard` | 400g / 22.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Vest`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 8 outfit(s). |
| `renaissance_persianate_pointed_slippers` | a pair of pointed leather slippers | `slippers` | `leather` | `Small` / `Standard` | 650g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 7 outfit(s). |
| `renaissance_persianate_conical_turban` | a conical $colour structured turban | `turban` | `cotton` | `Small` / `Standard` | 220g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Turban`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_persianate_flared_skirt_coat` | a fitted $colour flared-skirt coat | `coat` | `broadcloth` | `Normal` / `Good` | 1340g / 196.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |

### 21. Safavid / post-Safavid Persianate — 1

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_persianate_clothing_soft_round_cap` | a soft $colour cotton round cap | `cap` | `cotton` | `Normal` / `Standard` | 320g / 16.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Skullcap`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 3 outfit(s). |

### 22. Indo-Persian survivals — 2

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_indopersian_diagonal_tie_jama` | a diagonally tied $colour long coat | `coat` | `cotton` | `Normal` / `Standard` | 810g / 40.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_indopersian_sidefastened_jama` | a side-fastened $colour flared long coat | `coat` | `cotton` | `Normal` / `Standard` | 810g / 40.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |

### 23. South Asian survivals — 15

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_southasian_stitched_loindrawers` | a pair of close $colour cotton loin drawers | `drawers` | `cotton` | `Small` / `Standard` | 150g / 7.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shorts`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 13 outfit(s). |
| `renaissance_southasian_pleated_waistcloth` | a long pleated $colour cotton waistcloth | `waistcloth` | `cotton` | `Normal` / `Standard` | 630g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 10 outfit(s). |
| `renaissance_southasian_short_sideslit_tunic` | a short $colour cotton side-slit tunic | `tunic` | `cotton` | `Normal` / `Standard` | 300g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |
| `renaissance_southasian_flat_wound_turban` | a broad flat-wound $colour cotton turban | `turban` | `cotton` | `Small` / `Standard` | 220g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Turban`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 12 outfit(s). |
| `renaissance_southasian_bunched_ankletrousers` | a pair of narrow $colour bunched ankle trousers | `trousers` | `cotton` | `Normal` / `Standard` | 500g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 8 outfit(s). |
| `renaissance_southasian_long_sideslit_tunic` | a long $colour cotton side-slit tunic | `tunic` | `cotton` | `Normal` / `Standard` | 810g / 40.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |
| `renaissance_southasian_curvedtoe_shoes` | a pair of curved-toe leather shoes | `shoes` | `leather` | `Small` / `Standard` | 650g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 16 outfit(s). |
| `renaissance_southasian_short_wrapjacket` | a short $colour cotton cross-tied jacket | `jacket` | `cotton` | `Normal` / `Standard` | 580g / 32.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |
| `renaissance_southasian_fitted_shortbodice` | a fitted $colour cotton short bodice | `bodice` | `cotton` | `Normal` / `Standard` | 500g / 34.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Doublet`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |
| `renaissance_southasian_gathered_longskirt` | a full gathered $colour cotton skirt | `skirt` | `cotton` | `Normal` / `Standard` | 630g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_southasian_backtied_bodice` | a back-tied $colour cotton bodice | `bodice` | `cotton` | `Normal` / `Standard` | 500g / 34.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Doublet`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_southasian_shoulderdraped_garment` | a full $colour cotton shoulder-draped garment | `garment` | `cotton` | `Normal` / `Standard` | 760g / 36.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Dress`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_southasian_panelled_longskirt` | a panelled flared $colour silk skirt | `skirt` | `silk` | `Normal` / `Good` | 460g / 168.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_southasian_toepost_woodensandals` | a pair of toe-post wooden sandals | `sandals` | `wood` | `Small` / `Standard` | 450g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_southasian_long_crossover_robe` | a long $colour cotton cross-over robe | `robe` | `cotton` | `Normal` / `Standard` | 810g / 40.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |

### 24. South Asian textile-trade Early Modern — 1

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_southasian_clothing_chintz_merchant_jacket` | a $colour patterned chintz jacket | `jacket` | `chintz` | `Normal` / `Good` | 580g / 113.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 8 outfit(s). |

### 25. South Asian Early Modern — 1

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_southasian_clothing_light_cotton_shawl` | a light $colour cotton shawl | `shawl` | `cotton` | `Normal` / `Standard` | 560g / 27.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 15 outfit(s). |

### 26. Qing China — 13

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_qing_clothing_crosscollar_innerrobe` | a cross-collared $colour cotton inner robe | `robe` | `cotton` | `Normal` / `Standard` | 880g / 48.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long-Sleeved_Gown`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 10 outfit(s). Uses the under-gown layer beneath jackets and long robes. |
| `earlymodern_qing_clothing_wide_cotton_trousers` | a pair of wide $colour cotton trousers | `trousers` | `cotton` | `Normal` / `Standard` | 500g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 7 outfit(s). |
| `earlymodern_qing_clothing_short_riding_jacket` | a short $colour cotton riding jacket | `jacket` | `cotton` | `Normal` / `Standard` | 580g / 32.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |
| `earlymodern_qing_clothing_head_kerchief` | a $colour cotton head kerchief | `kerchief` | `cotton` | `Small` / `Standard` | 80g / 6.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Kerchief`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 6 outfit(s). |
| `earlymodern_qing_clothing_long_sidefastened_robe` | a long side-fastened $colour cotton robe | `robe` | `cotton` | `Normal` / `Standard` | 810g / 40.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |
| `earlymodern_qing_clothing_sleeveless_long_vest` | a sleeveless $colour long vest | `vest` | `silk` | `Normal` / `Good` | 230g / 134.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Vest`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 5 outfit(s). |
| `earlymodern_qing_clothing_cloth_shoes` | a pair of $colour cloth shoes | `shoes` | `cotton` | `Small` / `Standard` | 470g / 23.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 13 outfit(s). |
| `earlymodern_qing_clothing_round_skullcap` | a round $colour cloth skullcap | `cap` | `cotton` | `Normal` / `Standard` | 320g / 16.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Skullcap`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). |
| `earlymodern_qing_clothing_fine_sidefastened_robe` | a fine side-fastened $colour silk robe | `robe` | `silk` | `Normal` / `Good` | 580g / 255.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 1 outfit(s). |
| `earlymodern_qing_clothing_sidefastened_womens_jacket` | a side-fastened $colour cotton jacket | `jacket` | `cotton` | `Normal` / `Standard` | 580g / 32.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 5 outfit(s). |
| `earlymodern_qing_clothing_work_apron` | a $colour cotton work apron | `apron` | `cotton` | `Small` / `Standard` | 230g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Apron`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 8 outfit(s). |
| `earlymodern_qing_clothing_pleated_long_skirt` | a pleated $colour cotton long skirt | `skirt` | `cotton` | `Normal` / `Standard` | 630g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 3 outfit(s). |
| `earlymodern_qing_clothing_padded_winter_coat` | a padded $colour cotton winter coat | `coat` | `cotton` | `Normal` / `Standard` | 1120g / 65.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Coat`<br>`Armour_HeavyClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Winter Clothing` | New Early Modern prototype; used in 1 outfit(s). |

### 27. Late Ming transition survivals — 11

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_ming_crosscollar_innerrobe` | a cross-collared $colour ramie under-robe | `under-robe` | `ramie cloth` | `Normal` / `Standard` | 780g / 55.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long-Sleeved_Gown`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 10 outfit(s). Mapped to a dedicated under-gown layer beneath jackets and robes. |
| `renaissance_ming_wide_drawstring_trousers` | a pair of wide $colour cotton drawstring trousers | `trousers` | `cotton` | `Normal` / `Standard` | 500g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 7 outfit(s). |
| `renaissance_ming_narrowsleeve_workerrobe` | a narrow-sleeved $colour cotton work robe | `robe` | `cotton` | `Normal` / `Standard` | 810g / 40.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_ming_roundcollar_longrobe` | a round-collared $colour long robe | `robe` | `silk` | `Normal` / `Good` | 580g / 255.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_ming_long_sleeveless_overvest` | a long sleeveless $colour silk over-vest | `vest` | `silk` | `Normal` / `Good` | 230g / 134.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Vest`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_ming_cloth_courtboots` | a pair of high $colour cloth boots | `boots` | `cotton` | `Small` / `Standard` | 680g / 36.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Boots`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_ming_soft_scholarcap` | a soft folded $colour scholar cap | `cap` | `silk` | `Small` / `Good` | 110g / 81.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_ming_short_sidefastened_jacket` | a short side-fastened $colour cotton jacket | `jacket` | `cotton` | `Normal` / `Standard` | 580g / 32.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_ming_straightcollar_openrobe` | a straight-collared $colour open robe | `robe` | `silk` | `Normal` / `Good` | 730g / 370.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_ming_pleated_panelskirt` | a pleated $colour silk panel skirt | `skirt` | `silk` | `Normal` / `Good` | 460g / 168.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_ming_long_sidefastened_jacket` | a long side-fastened $colour cotton jacket | `jacket` | `cotton` | `Normal` / `Standard` | 580g / 32.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |

### 28. Joseon survivals — 7

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_joseon_long_crossfront_underrobe` | a long cross-front $colour ramie under-robe | `under-robe` | `ramie cloth` | `Normal` / `Standard` | 780g / 55.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long-Sleeved_Gown`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 10 outfit(s). Mapped to a dedicated under-gown layer beneath jackets and over-robes. |
| `renaissance_joseon_short_crossfront_jacket` | a short cross-front $colour ramie jacket | `jacket` | `ramie cloth` | `Normal` / `Standard` | 520g / 36.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |
| `renaissance_joseon_white_clothshoes` | a pair of white cloth shoes | `shoes` | `cotton` | `Small` / `Standard` | 470g / 23.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 10 outfit(s). |
| `renaissance_joseon_long_crossfront_jacket` | a long cross-front $colour silk jacket | `jacket` | `silk` | `Normal` / `Good` | 420g / 202.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_joseon_tall_horsehairhat` | a tall translucent brimmed hat | `hat` | `horsehair` | `Small` / `Good` | 170g / 25.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_joseon_broadsleeve_scholaroverrobe` | a broad-sleeved $colour scholar over-robe | `over-robe` | `silk` | `Normal` / `Good` | 730g / 370.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_joseon_full_gathered_wrapskirt` | a full gathered $colour wrap skirt | `skirt` | `silk` | `Normal` / `Good` | 460g / 168.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |

### 29. Joseon Korea — 4

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_joseon_clothing_full_baji_trousers` | a pair of full $colour cotton trousers | `trousers` | `cotton` | `Normal` / `Standard` | 500g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 7 outfit(s). |
| `earlymodern_joseon_clothing_white_cloth_socks` | a pair of white cloth socks | `socks` | `cotton` | `Small` / `Standard` | 100g / 6.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Socks`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 10 outfit(s). |
| `earlymodern_joseon_clothing_tied_headcloth` | a tied $colour cotton headcloth | `headcloth` | `cotton` | `Small` / `Standard` | 140g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Head_Veil`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 8 outfit(s). |
| `earlymodern_joseon_clothing_plain_cotton_overcoat` | a plain $colour cotton overcoat | `coat` | `cotton` | `Normal` / `Standard` | 1010g / 58.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). |

### 30. Japanese survivals — 8

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_japanese_unlined_summerrobe` | an unlined $colour ramie summer robe | `robe` | `ramie cloth` | `Normal` / `Standard` | 720g / 46.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_japanese_field_trousers` | a pair of close $colour ramie field trousers | `trousers` | `ramie cloth` | `Normal` / `Standard` | 450g / 24.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_japanese_splittoe_socks` | a pair of $colour split-toe socks | `socks` | `cotton` | `Small` / `Standard` | 100g / 6.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Socks`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 10 outfit(s). |
| `renaissance_japanese_smallsleeve_wraprobe` | a small-sleeved $colour wrap robe | `robe` | `silk` | `Normal` / `Good` | 580g / 255.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 8 outfit(s). |
| `renaissance_japanese_full_pleated_hakama` | a full pleated $colour lower garment | `hakama` | `silk` | `Normal` / `Good` | 360g / 134.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_japanese_wooden_clogs` | a pair of raised wooden clogs | `clogs` | `wood` | `Small` / `Standard` | 700g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |
| `renaissance_japanese_lacquered_conicalhat` | a lacquered wooden conical hat | `hat` | `wood` | `Small` / `Standard` | 230g / 8.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_japanese_short_openjacket` | a short open-front $colour jacket | `jacket` | `silk` | `Normal` / `Good` | 420g / 202.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |

### 31. Edo Japan — 6

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_edo_clothing_narrow_woven_obi` | a narrow $colour woven waist sash | `sash` | `silk` | `Small` / `Good` | 100g / 54.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sash`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 7 outfit(s). |
| `earlymodern_edo_clothing_woven_sandals` | a pair of $colour woven sandals | `sandals` | `hemp` | `Small` / `Standard` | 310g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). |
| `earlymodern_edo_clothing_tied_cotton_headcloth` | a tied $colour cotton headcloth | `headcloth` | `cotton` | `Small` / `Standard` | 80g / 6.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Kerchief`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 8 outfit(s). |
| `earlymodern_edo_clothing_narrow_work_apron` | a narrow $colour cotton work apron | `apron` | `cotton` | `Small` / `Standard` | 230g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Apron`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 5 outfit(s). |
| `earlymodern_edo_clothing_short_cotton_work_coat` | a short $colour cotton work coat | `coat` | `cotton` | `Normal` / `Standard` | 580g / 32.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |
| `earlymodern_edo_clothing_wide_woven_obi` | a broad $colour woven waist sash | `sash` | `silk` | `Small` / `Good` | 100g / 54.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sash`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 3 outfit(s). |

### 32. Ryukyuan survivals — 3

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_ryukyu_widesleeve_summerrobe` | a broad-sleeved $colour ramie tropical robe | `robe` | `ramie cloth` | `Normal` / `Standard` | 720g / 46.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |
| `renaissance_ryukyu_short_wrapjacket` | a short $colour cotton tropical wrap jacket | `jacket` | `cotton` | `Normal` / `Standard` | 580g / 32.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 8 outfit(s). |
| `renaissance_ryukyu_pleated_wrapskirt` | a pleated $colour cotton tropical wrap skirt | `skirt` | `cotton` | `Normal` / `Standard` | 630g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |

### 33. Ryukyu and maritime East Asia — 3

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_ryukyu_clothing_broad_woven_sash` | a broad $colour woven sash | `sash` | `cotton` | `Small` / `Standard` | 140g / 8.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sash`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 6 outfit(s). |
| `earlymodern_ryukyu_clothing_woven_fibre_sandals` | a pair of $colour woven fibre sandals | `sandals` | `hemp` | `Small` / `Standard` | 310g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 5 outfit(s). |
| `earlymodern_ryukyu_clothing_tied_headcloth` | a tied $colour cotton headcloth | `headcloth` | `cotton` | `Small` / `Standard` | 80g / 6.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Kerchief`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 7 outfit(s). |

### 34. South-east Asian survivals — 8

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_seasia_short_maritime_trousers` | a pair of short full $colour cotton trousers | `trousers` | `cotton` | `Normal` / `Standard` | 500g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 7 outfit(s). |
| `renaissance_seasia_pleated_courtwaistcloth` | a pleated $colour silk waistcloth | `waistcloth` | `silk` | `Normal` / `Good` | 460g / 168.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_seasia_short_collarless_jacket` | a short collarless $colour cotton jacket | `jacket` | `cotton` | `Normal` / `Standard` | 580g / 32.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 9 outfit(s). |
| `renaissance_seasia_palmleaf_conicalhat` | a broad conical fibre hat | `hat` | `barkcloth` | `Small` / `Standard` | 140g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_seasia_split_riding_waistcloth` | a divided $colour cotton riding waistcloth | `waistcloth` | `cotton` | `Normal` / `Standard` | 500g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_seasia_sleeveless_courtvest` | a long sleeveless $colour silk vest | `vest` | `silk` | `Normal` / `Good` | 230g / 134.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Vest`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 8 outfit(s). |
| `renaissance_seasia_long_crossfront_courtrobe` | a long cross-front $colour silk robe | `robe` | `silk` | `Normal` / `Good` | 580g / 255.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_seasia_sewn_tubeskirt` | a sewn $colour cotton tubular skirt | `skirt` | `cotton` | `Normal` / `Standard` | 630g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |

### 35. South-east Asian Early Modern — 2

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_seasia_clothing_light_shoulder_cloth` | a light $colour cotton shoulder cloth | `cloth` | `cotton` | `Normal` / `Standard` | 560g / 27.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 7 outfit(s). |
| `earlymodern_seasia_clothing_wrapped_headcloth` | a wrapped $colour cotton headcloth | `headcloth` | `cotton` | `Small` / `Standard` | 220g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Turban`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 8 outfit(s). |

### 36. Maritime South-east Asian trade worlds — 3

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_maritimeseasia_clothing_patterned_sarong` | a $colour1 and $colour2 patterned cotton sarong | `sarong` | `cotton` | `Normal` / `Standard` | 630g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_2BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 6 outfit(s). |
| `earlymodern_maritimeseasia_clothing_loose_long_tunic` | a loose $colour cotton long tunic | `tunic` | `cotton` | `Normal` / `Standard` | 540g / 23.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long-Sleeved_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). |
| `earlymodern_maritimeseasia_clothing_light_open_blouse` | a light $colour cotton open blouse | `blouse` | `cotton` | `Normal` / `Standard` | 580g / 32.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). |

### 37. Steppe survivals — 8

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_steppe_wide_ridingtrousers` | a pair of wide $colour wool riding trousers | `trousers` | `wool` | `Normal` / `Standard` | 620g / 22.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_steppe_short_split_ridingcoat` | a short split-skirt $colour riding coat | `coat` | `wool` | `Normal` / `Standard` | 720g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_steppe_soft_highboots` | a pair of soft high leather boots | `boots` | `leather` | `Small` / `Standard` | 1220g / 72.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_High_Boots`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 10 outfit(s). |
| `renaissance_steppe_pointed_feltcap` | a pointed $colour felt riding cap | `cap` | `felt` | `Small` / `Standard` | 170g / 12.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |
| `renaissance_steppe_quilted_ridingrobe` | a quilted side-fastened $colour riding robe | `robe` | `cotton` | `Normal` / `Standard` | 810g / 40.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_HeavyClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_steppe_fur_earflaphat` | a furred ear-flap hat | `hat` | `fur` | `Small` / `Standard` | 220g / 28.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_HeavyClothing`<br>`Insulation_Strong` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_steppe_sleeveless_ridingvest` | a sleeveless leather riding vest | `vest` | `leather` | `Normal` / `Standard` | 450g / 30.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Vest`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_steppe_sidefastened_furcoat` | a side-fastened $colour fur-lined long coat | `coat` | `wool` | `Normal` / `Good` | 990g / 88.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |

### 38. African court / Atlantic survivals — 7

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_africancourt_shortsleeve_tunic` | a short-sleeved $colour straight cotton tunic | `tunic` | `cotton` | `Normal` / `Standard` | 470g / 19.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_africancourt_soft_embroideredcap` | a soft $colour embroidered round cap | `cap` | `cotton` | `Small` / `Good` | 150g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |
| `renaissance_africancourt_narrow_waistwrapper` | a narrow $colour cotton waist wrapper | `wrapper` | `cotton` | `Normal` / `Standard` | 630g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_africancourt_sleeveless_straighttunic` | a sleeveless $colour straight cotton tunic | `tunic` | `cotton` | `Normal` / `Standard` | 380g / 17.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sleeveless_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_africancourt_broad_waistwrapper` | a broad full-length $colour cotton waist wrapper | `wrapper` | `cotton` | `Normal` / `Standard` | 630g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 10 outfit(s). |
| `renaissance_africancourt_longsleeve_sideslit_tunic` | a long-sleeved $colour side-slit tunic | `tunic` | `cotton` | `Normal` / `Standard` | 540g / 23.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long-Sleeved_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_africancourt_sewn_tubewrapper` | a sewn $colour cotton tubular wrapper | `wrapper` | `cotton` | `Normal` / `Standard` | 630g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |

### 39. Kongo / Angola / West Central Africa — 2

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_westcentralafrica_clothing_short_tradecloth_coat` | a short $colour broadcloth coat | `coat` | `broadcloth` | `Normal` / `Good` | 780g / 107.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Early Modern Era`<br>`Market / Clothing / Luxury Clothing` | New Early Modern prototype; used in 4 outfit(s). |
| `earlymodern_westcentralafrica_clothing_tradecloth_shirt` | a loose $colour cotton trade-cloth shirt | `shirt` | `cotton` | `Normal` / `Standard` | 300g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). |

### 40. West African / Atlantic — 2

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_africanatlantic_clothing_short_cotton_blouse` | a short $colour cotton blouse | `blouse` | `cotton` | `Normal` / `Standard` | 300g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 7 outfit(s). |
| `earlymodern_africanatlantic_clothing_full_headwrap` | a full $colour cotton headwrap | `headwrap` | `cotton` | `Small` / `Standard` | 220g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Turban`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 10 outfit(s). |

### 41. West/Central African survivals — 2

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_africancourt_barkcloth_straighttunic` | a straight $colour barkcloth tunic | `tunic` | `barkcloth` | `Normal` / `Standard` | 360g / 14.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sleeveless_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_africancourt_barkcloth_wrapskirt` | a broad $colour barkcloth wrap skirt | `skirt` | `barkcloth` | `Normal` / `Standard` | 600g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |

### 42. Shared colonial Atlantic — 10

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_colonial_clothing_plain_knee_breeches` | a pair of plain $colour knee breeches | `breeches` | `canvas` | `Normal` / `Standard` | 530g / 19.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Breeches`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 18 outfit(s). Uses planned Wear_Breeches dependency. |
| `earlymodern_colonial_clothing_plain_linen_shirt` | a plain $colour linen shirt | `shirt` | `linen` | `Normal` / `Standard` | 260g / 14.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 21 outfit(s). |
| `earlymodern_colonial_clothing_long_linen_shift` | a long $colour linen shift | `shift` | `linen` | `Normal` / `Standard` | 740g / 42.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Gown`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 21 outfit(s). |
| `earlymodern_colonial_clothing_plain_wool_bodice` | a plain $colour wool bodice | `bodice` | `wool` | `Normal` / `Standard` | 620g / 35.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Doublet`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 15 outfit(s). |
| `earlymodern_colonial_clothing_full_cotton_skirt` | a full $colour cotton skirt | `skirt` | `cotton` | `Normal` / `Standard` | 630g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 15 outfit(s). |
| `earlymodern_colonial_clothing_linen_work_apron` | a $colour linen work apron | `apron` | `linen` | `Small` / `Standard` | 210g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Apron`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 13 outfit(s). |
| `earlymodern_colonial_clothing_cotton_headwrap` | a full $colour cotton headwrap | `headwrap` | `cotton` | `Small` / `Standard` | 220g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Turban`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 18 outfit(s). |
| `earlymodern_colonial_clothing_short_wool_jacket` | a short $colour wool jacket | `jacket` | `wool` | `Normal` / `Standard` | 720g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 10 outfit(s). |
| `earlymodern_colonial_clothing_loose_canvas_trousers` | a pair of loose $colour canvas trousers | `trousers` | `canvas` | `Normal` / `Standard` | 630g / 16.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 7 outfit(s). |
| `earlymodern_colonial_clothing_broad_felt_sunhat` | a broad-brimmed $colour felt hat | `hat` | `felt` | `Small` / `Standard` | 170g / 12.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 6 outfit(s). |

### 43. Spanish colonial Americas — 1

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_spanishcolonial_clothing_cotton_shoulder_wrap` | a long $colour cotton shoulder wrap | `wrap` | `cotton` | `Normal` / `Standard` | 560g / 27.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 18 outfit(s). |

### 44. Sahelian survivals — 6

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_sahel_narrow_longtunic` | a narrow long $colour cotton tunic | `tunic` | `cotton` | `Normal` / `Standard` | 540g / 23.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long-Sleeved_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_sahel_veryfull_trousers` | a pair of very full $colour cotton trousers | `trousers` | `cotton` | `Normal` / `Standard` | 500g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_sahel_conical_leatherhat` | a conical leather riding hat | `hat` | `leather` | `Small` / `Standard` | 210g / 18.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_sahel_broad_rectangular_robe` | a broad rectangular $colour cotton over-robe | `over-robe` | `cotton` | `Normal` / `Standard` | 810g / 40.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_sahel_scholar_turban` | a layered $colour cotton turban | `turban` | `cotton` | `Small` / `Good` | 220g / 31.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Turban`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_sahel_embroidered_necktunic` | a broad-necked $colour cotton tunic | `tunic` | `cotton` | `Normal` / `Good` | 540g / 49.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long-Sleeved_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |

### 45. Ethiopian / Red Sea survivals — 6

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_redsea_narrow_cotton_tunic` | a narrow long $colour cotton tunic | `tunic` | `cotton` | `Normal` / `Standard` | 540g / 23.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long-Sleeved_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_redsea_full_shoulderwrap` | a full $colour cotton body-and-shoulder wrap | `wrap` | `cotton` | `Normal` / `Standard` | 760g / 36.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Dress`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |
| `renaissance_redsea_leather_highlandcloak` | a broad leather highland cloak | `cloak` | `leather` | `Normal` / `Standard` | 1150g / 57.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Cloak_(Open)`<br>`Armour_HeavyClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_redsea_white_headhood` | a white head-draped cotton hood | `hood` | `cotton` | `Small` / `Standard` | 230g / 14.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hood`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_redsea_long_courtshirt` | a long full-sleeved $colour cotton shirt | `shirt` | `cotton` | `Normal` / `Good` | 810g / 84.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_redsea_embroidered_shouldercape` | an embroidered $colour cotton shoulder cape | `cape` | `cotton` | `Normal` / `Good` | 560g / 57.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |

### 46. Ethiopian / Red Sea — 1

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_redsea_clothing_narrow_cotton_trousers` | a pair of narrow $colour cotton trousers | `trousers` | `cotton` | `Normal` / `Standard` | 500g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Trousers`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 6 outfit(s). |

### 47. Swahili / Indian Ocean survivals — 6

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_indianocean_short_coastaltunic` | a short loose $colour cotton coastal tunic | `tunic` | `cotton` | `Normal` / `Standard` | 470g / 19.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 6 outfit(s). |
| `renaissance_indianocean_embroidered_roundcap` | a close $colour embroidered round cap | `cap` | `cotton` | `Small` / `Good` | 150g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_indianocean_toeloop_sandals` | a pair of toe-loop leather sandals | `sandals` | `leather` | `Small` / `Standard` | 410g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_indianocean_long_coastalrobe` | a long collarless $colour cotton coastal robe | `robe` | `cotton` | `Normal` / `Standard` | 810g / 40.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_indianocean_open_merchantcoat` | a light open-front $colour cotton merchant coat | `coat` | `cotton` | `Normal` / `Good` | 1010g / 121.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Renaissance Era`<br>`Market / Clothing / Luxury Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_indianocean_sewn_wrapskirt` | a sewn $colour cotton coastal wrap skirt | `skirt` | `cotton` | `Normal` / `Standard` | 630g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |

### 48. Swahili Coast / Indian Ocean Africa — 1

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_indianocean_clothing_light_cotton_head_veil` | a light $colour cotton head veil | `veil` | `cotton` | `Small` / `Standard` | 140g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Head_Veil`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 3 outfit(s). |

### 49. Andean colonial and Indigenous — 3

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_andeancolonial_clothing_wool_poncho` | a $colour wool poncho | `poncho` | `wool` | `Normal` / `Standard` | 940g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Poncho`<br>`Armour_HeavyClothing`<br>`Insulation_Strong`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 5 outfit(s). |
| `earlymodern_andeancolonial_clothing_full_wool_pollera` | a full gathered $colour wool skirt | `skirt` | `wool` | `Normal` / `Standard` | 770g / 28.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 6 outfit(s). |
| `earlymodern_andeancolonial_clothing_shaped_wool_hat` | a shaped $colour wool hat | `hat` | `wool` | `Small` / `Standard` | 190g / 13.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 8 outfit(s). |

### 50. Mesoamerican survivals — 9

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_mesoamerican_long_wrapskirt` | a long $colour cotton wrap skirt | `skirt` | `cotton` | `Normal` / `Standard` | 630g / 26.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_mesoamerican_rectangular_blouse` | a rectangular sleeveless $colour cotton blouse | `blouse` | `cotton` | `Normal` / `Standard` | 380g / 17.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sleeveless_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_mesoamerican_woven_fibresandals` | a pair of $colour woven fibre sandals | `sandals` | `canvas` | `Small` / `Standard` | 370g / 11.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 9 outfit(s). |
| `renaissance_mesoamerican_panelled_loincloth` | a long panelled $colour cotton loin garment | `loincloth` | `cotton` | `Small` / `Standard` | 140g / 7.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Breechcloth`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_mesoamerican_side_seamed_tunic` | a side-seamed sleeveless $colour cotton tunic | `tunic` | `cotton` | `Normal` / `Standard` | 380g / 17.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sleeveless_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_mesoamerican_shouldertied_cloak` | a shoulder-tied $colour cotton rectangular cloak | `cloak` | `cotton` | `Normal` / `Standard` | 560g / 27.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_mesoamerican_woven_headcap` | a close $colour woven fibre cap | `cap` | `cotton` | `Small` / `Standard` | 150g / 13.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_mesoamerican_short_wrapskirt` | a short $colour cotton wrap skirt | `skirt` | `cotton` | `Normal` / `Standard` | 510g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_mesoamerican_triangle_shouldergarment` | a triangular $colour cotton shoulder garment | `garment` | `cotton` | `Normal` / `Standard` | 560g / 27.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |

### 51. Caribbean survivals — 5

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_caribbean_sleeveless_cottontunic` | a loose sleeveless $colour cotton tunic | `tunic` | `cotton` | `Normal` / `Standard` | 380g / 17.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sleeveless_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_caribbean_cotton_wrapskirt` | a short $colour cotton wrap skirt | `skirt` | `cotton` | `Normal` / `Standard` | 510g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_caribbean_cotton_loinapron` | a panelled $colour cotton loin apron | `apron` | `cotton` | `Small` / `Standard` | 140g / 7.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Breechcloth`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_caribbean_woven_fibresandals` | a pair of $colour woven fibre sandals | `sandals` | `canvas` | `Small` / `Standard` | 370g / 11.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_caribbean_woven_shouldercape` | a short $colour cotton shoulder cape | `cape` | `cotton` | `Normal` / `Standard` | 560g / 27.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |

### 52. English / French / Dutch colonial North America — 4

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_colonialnorthamerica_clothing_hooded_wool_capote` | a hooded $colour wool capote | `coat` | `wool` | `Normal` / `Standard` | 1230g / 61.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |
| `earlymodern_colonialnorthamerica_clothing_soft_moccasins` | a pair of soft leather moccasins | `moccasins` | `leather` | `Small` / `Standard` | 650g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |
| `earlymodern_colonialnorthamerica_clothing_linen_work_cap` | a $colour linen work cap | `cap` | `linen` | `Small` / `Standard` | 140g / 12.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 5 outfit(s). |
| `earlymodern_colonialnorthamerica_clothing_wool_shortgown` | a $colour wool shortgown | `shortgown` | `wool` | `Normal` / `Standard` | 720g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Jacket`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |

### 53. North American Indigenous survivals — 6

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_northamerican_hide_breechcloth` | a long hide breechcloth | `breechcloth` | `leather` | `Small` / `Standard` | 200g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Breechcloth`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 5 outfit(s). |
| `renaissance_northamerican_paired_hideleggings` | a pair of long hide leggings | `leggings` | `leather` | `Normal` / `Standard` | 520g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Leggings`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 7 outfit(s). |
| `renaissance_northamerican_hide_shirt` | a loose hide shirt | `shirt` | `leather` | `Normal` / `Standard` | 410g / 21.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shirt`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_northamerican_soft_moccasins` | a pair of soft hide moccasins | `moccasins` | `leather` | `Small` / `Standard` | 650g / 33.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 10 outfit(s). |
| `renaissance_northamerican_fur_robe` | a broad fur robe | `robe` | `fur` | `Normal` / `Standard` | 810g / 60.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_HeavyClothing`<br>`Insulation_Strong` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_northamerican_hide_wrapdress` | a full hide wrap dress | `dress` | `leather` | `Normal` / `Standard` | 1060g / 51.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Dress`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |

### 54. Indigenous North American regional families — 4

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `earlymodern_northamerican_clothing_tied_tradecloth_headband` | a tied $colour wool headband | `headband` | `wool` | `Normal` / `Standard` | 390g / 16.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Headband`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 9 outfit(s). |
| `earlymodern_northamerican_clothing_tradecloth_shirt` | a loose $colour wool trade-cloth shirt | `shirt` | `wool` | `Normal` / `Standard` | 360g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shirt`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 4 outfit(s). |
| `earlymodern_northamerican_clothing_wrapped_blanket_coat` | a wrapped $colour wool blanket coat | `coat` | `wool` | `Normal` / `Standard` | 1230g / 61.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Open_Robe`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 3 outfit(s). |
| `earlymodern_northamerican_clothing_tradecloth_wrap_skirt` | a $colour wool trade-cloth wrap skirt | `skirt` | `wool` | `Normal` / `Standard` | 770g / 28.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Early Modern Era`<br>`Market / Clothing / Standard Clothing` | New Early Modern prototype; used in 2 outfit(s). |

### 55. Andean survivals — 9

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Status, usage, and notes |
|---|---|---|---|---|---:|---|---|---|
| `renaissance_andean_short_worktunic` | a short coarse $colour camelid-wool tunic | `tunic` | `camelid wool` | `Normal` / `Standard` | 440g / 20.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sleeveless_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |
| `renaissance_andean_braided_fibresandals` | a pair of $colour braided fibre sandals | `sandals` | `canvas` | `Small` / `Standard` | 370g / 11.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 8 outfit(s). |
| `renaissance_andean_woven_earflapcap` | a woven $colour camelid-wool ear-flap cap | `cap` | `camelid wool` | `Small` / `Standard` | 180g / 15.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_andean_straight_sleevelesstunic` | a straight sleeveless $colour camelid-wool tunic | `tunic` | `camelid wool` | `Normal` / `Standard` | 440g / 20.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sleeveless_Tunic`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_andean_fringed_camelidshawl` | a fringed $colour camelid-wool shawl | `shawl` | `camelid wool` | `Normal` / `Standard` | 650g / 32.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 4 outfit(s). |
| `renaissance_andean_paired_shouldermantle` | a broad pinned $colour camelid-wool shoulder mantle | `mantle` | `camelid wool` | `Normal` / `Standard` | 650g / 32.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Mantle`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 3 outfit(s). |
| `renaissance_andean_wrapped_fulllengthdress` | a wrapped full-length $colour camelid-wool dress | `dress` | `camelid wool` | `Normal` / `Standard` | 890g / 42.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Dress`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_andean_long_headcloth` | a long $colour camelid-wool headcloth | `headcloth` | `camelid wool` | `Small` / `Standard` | 250g / 18.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Turban`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 2 outfit(s). |
| `renaissance_andean_pinned_wrapskirt` | a pinned broad $colour camelid-wool wrap skirt | `skirt` | `camelid wool` | `Normal` / `Standard` | 740g / 31.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Long_Skirt`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Renaissance Era`<br>`Market / Clothing / Standard Clothing` | Cross-era Renaissance stable-reference admission; used in 1 outfit(s). |

## Implementation order

1. Seed or verify `Wear_Stays` and `Wear_Breeches` from the dependency ledger.
2. Ensure the three implemented `preindustrial_clothing_*` aliases.
3. Ensure admitted Renaissance stable references individually and idempotently.
4. Create new `earlymodern_*` rows in catalogue order.
5. Add culture manifests and profession/shop admissions only after every referenced item exists.
6. Add skins and full descriptions in the later presentation pass.
7. Add crafts only after exact material, skill, tool, and product-tag dependencies resolve.

## Validation result

- **PASS** — 35 expected cultures, no global-maritime culture.
- **PASS** — 350 outfits, exactly ten per culture.
- **PASS** — five male and five female outfits per culture.
- **PASS** — no outfit heading contains military, uniform, or noble scope.
- **PASS** — no duplicate complete manifest within a culture and no duplicate item within an outfit.
- **PASS** — every outfit has upper-body, lower/full-body, footwear, and headwear/head-covering coverage.
- **PASS** — no outfit uses the same exact wearable profile twice.
- **PASS** — every sdesc begins with an article, is 3-10 words, and has matching variable components.
- **PASS** — materials, live components, tags, weight, quality, size, and cost fields validate against the current project vocabulary plus the two declared component dependencies.

## Deferred work

- full descriptions;
- military, naval-uniform, officer, livery, and regimental clothing;
- noble, court, royal, and diplomatic regalia;
- global maritime and chartered-company trade outfits;
- profession-specific overlays beyond the civilian social range represented here;
- skins, crafts, and shop/region prevalence manifests.
