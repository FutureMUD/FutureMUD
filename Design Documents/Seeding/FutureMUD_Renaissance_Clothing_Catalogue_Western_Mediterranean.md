# FutureMUD Renaissance Clothing Catalogue — Western, Mediterranean, and European Frontier Shared Cultures

## Purpose

This volume defines **154 unique clothing prototypes** for the following Renaissance Shared material-culture groupings:

- Shared Western European Renaissance, including Italian, Franco-Flemish, British Isles, and German/HRE/Swiss admissions;
- Shared Iberian / Atlantic;
- Shared Northern / Baltic, Shared Central European, Shared Central/Eastern frontier, and Shared Eastern Orthodox / northern;
- Shared Reformation / Catholic institutional clothing where construction differs;
- Shared Ottoman-Islamicate.

The authority, dependency ledger, common forms, and implementation rules are in [FutureMUD Renaissance Clothing and Accessories Design Reference](./FutureMUD_Renaissance_Clothing_Accessories_Design_Reference.md). Historical terms in the notes are builder anchors, not mandatory public item names.

## Volume-specific wearable gaps

In addition to the common-profile audit, this volume requires exact wearable/layering definitions for fitted doublets, bodices, one-piece gowns, joined hose, trunk hose/breeches, structured skirt supports, collars/ruffs, liturgical over-vestments, and turban-over-cap constructions. Proposed semantic profiles used below are:

- `WP-FITTED-TORSO`: doublet, jerkin, or bodice layer;
- `WP-DRESS`: one-piece or bodice-and-skirt garment covering torso and legs;
- `WP-HOSE`: paired joined or split close legwear;
- `WP-BREECHES`: joined upper-leg garment compatible with stockings;
- `WP-SKIRT-SUPPORT`: farthingale or wheel support beneath a skirt;
- `WP-COLLAR`: partlet, ruff, or independent neck/upper-chest layer;
- `WP-VESTMENT`: ceremonial layer worn above a long religious garment;
- `WP-TURBAN-CAP`: wrapped headwear whose cap and winding are one item.

If an exact live component already supplies the coverage and layering, use it. Otherwise these are blocking new seeded wearable components, not new component types.

## A. Shared Western European Renaissance and cross-confessional institutional catalogue — 75

### Shirts, chemises, shifts, and work underlayers — 8

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_western_gathered_linen_shirt` | gathered linen shirt | linen | `WP-SHIRT` | low or moderate neck gathering; plain work, fine urban, and embroidered court skins |
| `renaissance_western_pleated_linen_shirt` | finely pleated linen shirt | linen | `WP-SHIRT` | narrow or broad pleats; detachable wrist frills remain skins unless coverage changes |
| `renaissance_western_high_collar_shirt` | high-collared linen shirt | linen | `WP-SHIRT` | standing or turned collar; late-fifteenth through sixteenth-century urban/court gate |
| `renaissance_western_open_neck_work_shirt` | open-neck work shirt | linen | `WP-SHIRT` | fuller cut and reinforced neck; labourer, sailor, artisan, and rural skins |
| `renaissance_western_square_neck_chemise` | square-necked chemise | linen | `WP-LONG-UNDERLAYER` | calf or ankle hem; narrow/full sleeves; Western court and urban outfits |
| `renaissance_western_high_neck_chemise` | high-necked chemise | linen | `WP-LONG-UNDERLAYER` | gathered or banded neck; plain/fine skins; sixteenth-century gate |
| `renaissance_western_work_smock` | loose work smock | linen | `WP-LONG-UNDERLAYER` | knee/calf length; reinforced or plain; field, workshop, laundry, and market work |
| `renaissance_western_long_shift` | long linen shift | linen | `WP-LONG-UNDERLAYER` | ankle/calf; sleeveless or long-sleeved skins only where the wear profile remains identical |

### Doublets, jerkins, and bodices — 18

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_western_sleeveless_doublet` | fitted sleeveless doublet | broadcloth | `WP-FITTED-TORSO` | plain work, merchant, quilted, and velvet court skins |
| `renaissance_western_short_skirted_doublet` | short-skirted doublet | broadcloth | `WP-FITTED-TORSO` | narrow tabs or continuous skirt; early and mid-century skins |
| `renaissance_western_long_skirted_doublet` | long-skirted doublet | broadcloth | `WP-FITTED-TORSO` | hip-length skirts; plain, guarded, and slashed skins |
| `renaissance_western_peascod_doublet` | pointed-front doublet | broadcloth | `WP-FITTED-TORSO` | late-sixteenth-century date gate; wool, satin, and velvet skins |
| `renaissance_western_padded_doublet` | padded fitted doublet | wool | `WP-FITTED-TORSO` | civilian warmth and shaped silhouette; no armour claim beyond clothing components |
| `renaissance_western_slashed_doublet` | slashed-sleeve doublet | broadcloth | `WP-FITTED-TORSO` | restrained, mercenary-influenced, and court skins; slash colours remain cosmetic |
| `renaissance_western_leather_jerkin` | fitted leather jerkin | leather | `WP-FITTED-TORSO` | sleeved/sleeveless only if component coverage matches; work, travel, guard skins |
| `renaissance_western_cloth_jerkin` | fitted cloth jerkin | broadcloth | `WP-FITTED-TORSO` | plain, guarded, slashed, and livery skins |
| `renaissance_western_sleeveless_jerkin` | sleeveless skirted jerkin | broadcloth | `WP-FITTED-TORSO` | short/long skirt skins; merchant, artisan, and court-servant admissions |
| `renaissance_western_long_skirted_jerkin` | long-skirted jerkin | wool | `WP-FITTED-TORSO` | thigh-length, buttoned or laced; Central/Western urban use |
| `renaissance_western_front_laced_bodice` | front-laced fitted bodice | broadcloth | `WP-FITTED-TORSO` | square/rounded neck; work, urban, and fine skins |
| `renaissance_western_side_laced_bodice` | side-laced fitted bodice | broadcloth | `WP-FITTED-TORSO` | one- or two-sided lacing; fitted court and prosperous urban use |
| `renaissance_western_back_laced_bodice` | back-laced fitted bodice | broadcloth | `WP-FITTED-TORSO` | requires assisted-dressing context only in builder notes, not mechanics |
| `renaissance_western_square_neck_bodice` | square-necked bodice | broadcloth | `WP-FITTED-TORSO` | plain, guarded, brocade-skin, and velvet-skin families |
| `renaissance_western_high_neck_bodice` | high-necked bodice | broadcloth | `WP-FITTED-TORSO` | sixteenth-century court/urban gate; detachable sleeves admitted separately |
| `renaissance_western_stiffened_pair_of_bodies` | stiffened fitted bodies | canvas | `WP-FITTED-TORSO` | late-sixteenth-century gate; canvas, satin-covered, and court skins; no modern corset terminology publicly |
| `renaissance_western_stomachered_bodice` | stomacher-fronted bodice | broadcloth | `WP-FITTED-TORSO` | separate-looking front panel is integral to this prototype; rich textile skins |
| `renaissance_western_sleeveless_overbodice` | sleeveless over-bodice | velvet | `WP-FITTED-TORSO` | worn above chemise or undergown; plain wool and luxury silk skins |

### Kirtles, gowns, hose, breeches, and skirt structures — 23

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_western_plain_kirtle` | plain fitted kirtle | wool | `WP-DRESS` | work, townswoman, and household skins; sewn bodice-and-skirt silhouette |
| `renaissance_western_fitted_kirtle` | close-fitted kirtle | broadcloth | `WP-DRESS` | square/rounded neck and narrow/full skirt skins |
| `renaissance_western_sleeveless_kirtle` | sleeveless fitted kirtle | broadcloth | `WP-DRESS` | intended to display chemise or detachable sleeves |
| `renaissance_western_front_opening_kirtle` | front-opening kirtle | broadcloth | `WP-DRESS` | laced, hooked, or buttoned skins; no fastener mechanics claimed |
| `renaissance_western_high_waisted_gown` | high-waisted fitted gown | broadcloth | `WP-DRESS` | early Renaissance court and prosperous urban gate |
| `renaissance_western_square_neck_gown` | square-necked fitted gown | broadcloth | `WP-DRESS` | plain, velvet, satin, and guarded skins |
| `renaissance_western_round_gown` | full round gown | broadcloth | `WP-DRESS` | continuous full skirt; urban/court and formal household use |
| `renaissance_western_fitted_court_gown` | fitted court gown | velvet | `WP-DRESS` | silk/satin/brocade skins; heraldry and jewellery remain separate presentation |
| `renaissance_western_front_opening_gown` | front-opening overgown | broadcloth | `WP-ROBE-OPEN` | reveals kirtle/petticoat; plain, furred, and guarded skins |
| `renaissance_western_loose_house_gown` | loose house gown | wool | `WP-ROBE-OPEN` | domestic, scholar, invalid, and prosperous merchant skins |
| `renaissance_western_robe_style_gown` | long robe-style gown | broadcloth | `WP-ROBE-OPEN` | furred or unlined; civic, merchant, scholar, and court contexts |
| `renaissance_western_hanging_sleeve_overgown` | hanging-sleeve overgown | velvet | `WP-ROBE-OPEN` | open or closed hanging sleeve skins; court and ceremonial admission |
| `renaissance_western_false_sleeve_overgown` | false-sleeve overgown | broadcloth | `WP-ROBE-OPEN` | decorative dependent sleeves are integral silhouette |
| `renaissance_western_trained_gown` | long-trained formal gown | velvet | `WP-DRESS` | court, bridal, civic-pageant, and performance contexts; train is descriptive |
| `renaissance_western_joined_hose` | pair of joined hose | wool | `WP-HOSE` | footed/soleless and plain/guarded skins; fitted lower-body profile |
| `renaissance_western_split_hose` | pair of split hose | wool | `WP-HOSE` | two-colour mi-parti skin permitted; one paired item |
| `renaissance_western_trunk_hose` | rounded trunk hose | broadcloth | `WP-BREECHES` | modest/full volume; paning requires separate prototype below |
| `renaissance_western_paned_trunk_hose` | paned trunk hose | broadcloth | `WP-BREECHES` | contrasting lining and slash treatment remain skins |
| `renaissance_western_venetian_breeches` | loose knee-length breeches | broadcloth | `WP-BREECHES` | full but unpaned; urban, court, and maritime-contact skins |
| `renaissance_western_pluderhosen` | extremely full gathered breeches | broadcloth | `WP-BREECHES` | German/HRE/mercenary date and culture gate; lining colours are skins |
| `renaissance_western_spanish_farthingale` | conical skirt support | canvas | `WP-SKIRT-SUPPORT` | late-fifteenth/sixteenth-century Iberian and court diffusion gate |
| `renaissance_western_wheel_farthingale` | wheel-shaped skirt support | canvas | `WP-SKIRT-SUPPORT` | late-sixteenth-century elite gate; not ordinary workwear |
| `renaissance_western_full_petticoat` | full sewn petticoat | wool | `WP-SKIRT` | plain, quilted, guarded, and silk skins; under- or visible-skirt admission |

### Outerwear, headwear, collars, footwear, and gloves — 16

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_western_short_cloak` | short shoulder cloak | wool | `WP-CLOAK` | hip/thigh length; plain, guarded, and velvet skins |
| `renaissance_western_full_circle_cloak` | full circular cloak | broadcloth | `WP-CLOAK` | calf/ankle length; hooded version uses common hooded-cloak prototype instead |
| `renaissance_western_furred_gown` | fur-edged long gown | broadcloth | `WP-ROBE-OPEN` | scholar, magistrate, merchant, and court skins; audit exact fur material |
| `renaissance_western_flat_cap` | broad flat cap | felt | `WP-HEAD-CAP` | narrow/broad crown; plain, feathered, badge, and velvet skins |
| `renaissance_western_round_bonnet` | soft round bonnet | wool | `WP-HEAD-CAP` | brimmed/brimless skins only where component remains cap-like |
| `renaissance_western_linen_coif` | tied linen coif | linen | `WP-HEAD-CAP` | plain, embroidered, work, domestic, and under-hood skins |
| `renaissance_western_netted_caul` | close netted hair caul | silk | `WP-HEAD-CAP` | plain, pearl-pattern skin, metallic-thread skin; no jewellery component implied |
| `renaissance_western_french_hood` | crescent-framed court hood | velvet | `WP-HEAD-HAT` | restrained and jewelled-presentation skins; sixteenth-century court gate |
| `renaissance_western_gable_hood` | angular framed court hood | velvet | `WP-HEAD-HAT` | English/Tudor gate; plain and rich skins |
| `renaissance_western_partlet` | shoulder-covering partlet | linen | `WP-COLLAR` | opaque, silk, lace, and high-neck skins; layers above bodice/gown |
| `renaissance_western_standing_ruff` | stiff standing ruff | linen | `WP-COLLAR` | small, medium, and cartwheel skins only if coverage remains compatible; late-century gate |
| `renaissance_western_square_toed_shoes` | pair of broad square-toed shoes | leather | `WP-FOOT-SHOE` | plain, slashed, velvet-covered, and court skins |
| `renaissance_western_pattens` | pair of strapped pattens | wood | `WP-OVERSHOE` | low/high wood sole; wet-street and travel admission |
| `renaissance_western_chopines` | pair of tall platform overshoes | wood | `WP-OVERSHOE` | Venetian/Mediterranean elite gate; moderate and tall variants may need separate sizes, not prototypes |
| `renaissance_western_fitted_dress_gloves` | pair of fitted dress gloves | leather | `WP-HANDS` | plain, embroidered, slashed-cuff, scented-presentation skins |
| `renaissance_western_dress_apron` | fine full dress apron | linen | `WP-WRAP-SKIRT` | narrow/full, plain/lace-edged, domestic and status-display skins; distinct from work apron by cut |

### Shared Reformation / Catholic institutional clothing — 10

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_institution_plain_cassock` | long close-buttoned cassock | broadcloth | `WP-ROBE-CLOSED` | clerical, academic, civic-official skins; institution/date gate |
| `renaissance_institution_preaching_gown` | full-sleeved preaching gown | broadcloth | `WP-ROBE-OPEN` | Reformation preacher, university, magistrate, and scholar skins |
| `renaissance_institution_monastic_habit` | long belted monastic habit | wool | `WP-ROBE-CLOSED` | order colour and cut mostly skins; local institution required |
| `renaissance_institution_monastic_scapular` | long monastic scapular | wool | `WP-VESTMENT` | narrow/broad and hooded-order skins; worn above habit |
| `renaissance_institution_full_cowl` | hooded monastic cowl | wool | `WP-CLOAK` | shoulder cape and deep hood integral; monastic gate |
| `renaissance_institution_linen_surplus` | loose white surplice | linen | `WP-VESTMENT` | knee/calf, narrow/full sleeve; church/chapel gate |
| `renaissance_institution_liturgical_alb` | long white liturgical robe | linen | `WP-ROBE-CLOSED` | plain, apparelled, and lace-trim skins; church gate |
| `renaissance_institution_chasuble` | sleeveless liturgical over-vestment | silk | `WP-VESTMENT` | broad/narrow cut; colour, embroidery, and seasonal use are skins/manifests |
| `renaissance_institution_processional_cope` | long clasped ceremonial cope | silk | `WP-CLOAK` | hooded shield and embroidery are skins; procession/liturgy gate |
| `renaissance_institution_academic_robe` | long academic robe | broadcloth | `WP-ROBE-OPEN` | faculty, university, legal, and civic skins; institution required |

## B. Shared Iberian / Atlantic catalogue — 10

These rows add Atlantic-facing work, pilgrimage, confraternity, and weather silhouettes not already covered by common or Western prototypes.

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_iberian_canvas_deck_smock` | short canvas deck smock | canvas | `WP-SHIRT` | sleeveless/long-sleeved only with matching coverage; sailor, dock, fishing skins |
| `renaissance_iberian_short_sailor_jacket` | short close-fitting sailor jacket | wool | `WP-JACKET` | single/double row presentation, plain/guarded; no modern naval-uniform claim |
| `renaissance_iberian_full_shipboard_breeches` | full calf-length shipboard breeches | canvas | `WP-TROUSERS` | drawstring or buttoned skins; Atlantic sailor and dockworker gate |
| `renaissance_iberian_hooded_weather_cape` | short hooded weather cape | broadcloth | `WP-CLOAK` | hip/knee length; shipboard, pilgrim, courier, and coastal skins |
| `renaissance_iberian_knitted_sailor_cap` | close knitted sailor cap | wool | `WP-HEAD-CAP` | rolled/unrolled edge, plain/striped; maritime gate |
| `renaissance_iberian_broad_crowned_hat` | broad-crowned felt hat | felt | `WP-HEAD-HAT` | narrow/broad brim and cord/feather skins; Iberian urban, rural, maritime gate |
| `renaissance_iberian_lace_edged_headveil` | lace-edged head veil | linen | `WP-HEAD-VEIL` | short/long, opaque/sheer skin; court, church, married-status admissions |
| `renaissance_iberian_confraternity_robe` | hooded confraternity robe | linen | `WP-ROBE-CLOSED` | colour, badge, and penitential skin; named institution/procession required |
| `renaissance_iberian_pilgrim_shoulder_cape` | short pilgrim shoulder cape | wool | `WP-SHOULDER` | hooded/unhooded skin; pilgrimage and shrine-route gate |
| `renaissance_iberian_high_sea_boots` | pair of high sea boots | leather | `WP-FOOT-BOOT` | soft/stiff shaft and turned cuff skins; maritime/travel gate, no waterproof mechanic claim |

## C. Shared Northern, Central, and Eastern European frontier catalogue — 15

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_frontier_fur_trimmed_short_coat` | fur-trimmed short coat | wool | `WP-JACKET` | hip/thigh length; merchant, noble, frontier, and winter skins |
| `renaissance_frontier_long_furred_overcoat` | long furred overcoat | broadcloth | `WP-ROBE-OPEN` | calf/ankle; broad/narrow collar; northern and Muscovite trade networks |
| `renaissance_frontier_close_buttoned_longcoat` | close-buttoned long coat | wool | `WP-ROBE-OPEN` | Polish-Lithuanian/Hungarian builder anchor; fitted and loose skins |
| `renaissance_frontier_split_sleeve_noblecoat` | split-sleeved noble coat | broadcloth | `WP-ROBE-OPEN` | decorative sleeve openings integral; court/frontier elite gate |
| `renaissance_frontier_belted_wool_caftan` | belted long wool coat | wool | `WP-ROBE-OPEN` | Eastern Orthodox, steppe-border, merchant, and town skins; sash admitted from baseline |
| `renaissance_frontier_sleeveless_long_gown` | sleeveless long overgown | broadcloth | `WP-DRESS` | sarafan-like builder anchor; under-robe visible, court/town/rural skins |
| `renaissance_frontier_muscovite_long_caftan` | long full-skirted caftan | broadcloth | `WP-ROBE-OPEN` | Muscovite court, merchant, service, and winter skins |
| `renaissance_frontier_short_riding_coat` | short split-skirt riding coat | wool | `WP-JACKET` | cavalry, courier, noble, and frontier skins |
| `renaissance_frontier_wide_hungarian_trousers` | wide riding trousers | wool | `WP-TROUSERS` | full upper leg, narrower calf; Hungarian/frontier riding gate |
| `renaissance_frontier_high_felt_cap` | high-crowned felt cap | felt | `WP-HEAD-HAT` | plain, corded, feathered, and military-status skins |
| `renaissance_frontier_fur_brimmed_cap` | soft fur-brimmed cap | wool | `WP-HEAD-CAP` | low/high crown; merchant, court, town, and winter skins |
| `renaissance_frontier_tall_fur_hat` | tall cylindrical fur hat | fur | `WP-HEAD-HAT` | Muscovite, frontier elite, and diplomatic gate; exact fur material required |
| `renaissance_frontier_knitted_northern_cap` | long knitted wool cap | wool | `WP-HEAD-CAP` | short/long crown, folded edge, sailor/rural/town skins |
| `renaissance_frontier_felted_winter_boots` | pair of felted winter boots | felt | `WP-FOOT-BOOT` | soft felt shaft, leather-soled skin; northern/steppe winter admission |
| `renaissance_frontier_split_skirt_riding_boots` | pair of stiff high riding boots | leather | `WP-FOOT-BOOT` | knee/thigh and turned-cuff skins; cavalry/frontier gate |

## D. Shared Ottoman-Islamicate catalogue — 30

Public names remain form-based. Builder notes may record Ottoman terms such as gömlek, şalvar, entari, kaftan, dolama, yelek, ferace, and kavuk only as inspirations.

### Underlayers, trousers, and core robes — 16

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_ottoman_collarless_inner_shirt` | collarless long inner shirt | linen | `WP-SHIRT` | knee/thigh, narrow/full sleeve, plain/fine skins |
| `renaissance_ottoman_ankle_gathered_drawers` | full ankle-gathered drawers | linen | `WP-TROUSERS` | moderate/very full; cotton and silk skins; underlayer or warm-weather use |
| `renaissance_ottoman_narrow_riding_trousers` | narrow riding trousers | wool | `WP-TROUSERS` | footed/stirrup/plain skins; cavalry, courier, court admission |
| `renaissance_ottoman_full_salwar_trousers` | very full gathered trousers | cotton | `WP-TROUSERS` | ankle/calf gathering; plain, striped, and silk skins |
| `renaissance_ottoman_short_entari_coat` | short fitted front-opening coat | cotton | `WP-JACKET` | hip/thigh, narrow/full sleeve, work/court skins |
| `renaissance_ottoman_long_entari_robe` | long fitted front-opening robe | silk | `WP-ROBE-OPEN` | calf/ankle, plain/striped/figured skins; urban and court admission |
| `renaissance_ottoman_fitted_kaftan` | fitted long kaftan | silk | `WP-ROBE-OPEN` | narrow skirt and sleeves; court/official/merchant skins |
| `renaissance_ottoman_loose_kaftan` | loose full long kaftan | silk | `WP-ROBE-OPEN` | broad skirt, side slits, plain/figured skins |
| `renaissance_ottoman_short_sleeve_kaftan` | short-sleeved long kaftan | silk | `WP-ROBE-OPEN` | intended to reveal inner sleeves; court and ceremonial admission |
| `renaissance_ottoman_sleeveless_kaftan` | sleeveless long kaftan | silk | `WP-ROBE-OPEN` | inner robe visible; court, official, and household skins |
| `renaissance_ottoman_broad_sleeve_ceremonial_kaftan` | broad-sleeved ceremonial kaftan | brocade | `WP-ROBE-OPEN` | **material dependency**; court gift, office, diplomatic, and ceremony gate |
| `renaissance_ottoman_fur_lined_winter_kaftan` | fur-lined winter kaftan | broadcloth | `WP-ROBE-OPEN` | exact fur audit; court, military command, merchant, winter skins |
| `renaissance_ottoman_dolman_riding_coat` | close-fitted split-skirt riding coat | wool | `WP-JACKET` | cavalry and courier gate; braid/guard skins |
| `renaissance_ottoman_short_cavalry_jacket` | short fitted cavalry jacket | wool | `WP-JACKET` | hip length and narrow sleeves; military/status skins without armour claim |
| `renaissance_ottoman_sleeved_outer_robe` | long sleeved outer robe | broadcloth | `WP-ROBE-OPEN` | plain, official, scholar, and merchant skins |
| `renaissance_ottoman_broad_sleeve_outerrobe` | broad-sleeved full outer robe | silk | `WP-ROBE-OPEN` | court, judge, scholar, and ceremonial skins |

### Women's layered forms, vests, padded clothing, and religious outerwear — 8

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_ottoman_fitted_frontopening_gown` | fitted front-opening long gown | silk | `WP-DRESS` | narrow/full skirt, open/closed neckline, urban/court skins |
| `renaissance_ottoman_loose_layered_gown` | loose layered long gown | cotton | `WP-DRESS` | indoor, household, merchant, and fine silk skins |
| `renaissance_ottoman_full_outdoor_overrobe` | full enveloping outdoor over-robe | broadcloth | `WP-ROBE-OPEN` | urban/status/date gate; face covering remains separate item |
| `renaissance_ottoman_broad_sleeveless_vest` | broad sleeveless long vest | wool | `WP-VEST` | hip/knee length; plain, furred, velvet skins |
| `renaissance_ottoman_short_fitted_vest` | short fitted sleeveless vest | silk | `WP-VEST` | household, merchant, court, musician skins |
| `renaissance_ottoman_quilted_undercoat` | quilted long undercoat | cotton | `WP-ROBE-CLOSED` | light/heavy quilting; civilian warmth and under-armour admission |
| `renaissance_ottoman_dervish_wool_cloak` | broad rough wool cloak | wool | `WP-CLOAK` | Sufi institution and order gate; colour/patch skins require local manifest |
| `renaissance_ottoman_tall_dervish_cap` | tall felt devotional cap | felt | `WP-HEAD-HAT` | Sufi institution/order gate; exact shape and colour as skins where silhouette remains stable |

### Structured turbans and distinctive footwear — 6

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_ottoman_wrapped_cap_turban` | compact cap-wound turban | cotton | `WP-TURBAN-CAP` | work, scholar, merchant, and official skins; cap and winding one prototype |
| `renaissance_ottoman_small_court_turban` | small structured court turban | silk | `WP-TURBAN-CAP` | official, court-servant, scholar, and diplomatic skins |
| `renaissance_ottoman_high_court_turban` | high structured court turban | silk | `WP-TURBAN-CAP` | office/rank/date-gated; exact winding and aigrette presentation are skins |
| `renaissance_ottoman_quilted_turban` | padded quilted turban | cotton | `WP-TURBAN-CAP` | military/cavalry/court skins; no armour mechanics beyond clothing |
| `renaissance_ottoman_pointed_slippers` | pair of pointed soft slippers | leather | `WP-FOOT-SHOE` | backless/heeled-back, plain/embroidered/velvet-covered skins |
| `renaissance_ottoman_high_bath_sandals` | pair of high wooden bath sandals | wood | `WP-OVERSHOE` | bathhouse and elite household gate; strap decoration as skins |

## E. Headwear and footwear expansion — 24

These rows are additional construction-level options. They do not alter the existing outfit minimums or first-pass shared-placement count; a builder should admit them only where status, work, route, climate, institution, or local dress practice justifies the distinct form.

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_western_slashed_puffed_cap` | slashed and puffed court cap | velvet | `WP-HEAD-CAP` | Early- and mid-sixteenth-century court, mercenary-influenced, pageant, and prosperous urban gate; lining colours are skins. |
| `renaissance_western_feathered_velvet_bonnet` | feathered velvet bonnet | velvet | `WP-HEAD-CAP` | Court, merchant, diplomatic, and performance skins; feather placement remains cosmetic. |
| `renaissance_western_sugarloaf_felthat` | sugarloaf-crowned felt hat | felt | `WP-HEAD-HAT` | Late-sixteenth-century Western and Central European town, traveller, religious-dissenter precursor, and colonial-contact admission. |
| `renaissance_western_broad_plumed_ridinghat` | broad plumed riding hat | felt | `WP-HEAD-HAT` | Mounted court, hunting, courier, officer-presentation, and pageant use; plume and hatband are skins. |
| `renaissance_western_embroidered_linen_nightcap` | embroidered linen nightcap | linen | `WP-HEAD-CAP` | Late-sixteenth-century prosperous and elite informal indoor wear; simpler quilted skins may admit scholars and merchants. |
| `renaissance_western_quilted_silk_dressingcap` | quilted silk dressing cap | silk | `WP-HEAD-CAP` | Luxury undress cap for private chambers, learned households, convalescence, and informal audiences. |
| `renaissance_western_wired_linen_coif` | wired-front linen coif | linen | `WP-HEAD-CAP` | Structured female head cap with wired or stiffened front edge; date, region, and veil combination require local admission. |
| `renaissance_western_rolled_velvet_balzo` | rolled velvet court headdress | velvet | `WP-HEAD-HAT` | Italian and Mediterranean court-facing rolled headdress; jewels, caul, and exact profile are skins where silhouette remains stable. |
| `renaissance_western_pointed_attifet_hood` | pointed-front court hood | velvet | `WP-HEAD-HAT` | Late-sixteenth-century French-facing court and mourning gate; distinct pointed front from the crescent French hood. |
| `renaissance_institution_academic_squarecap` | structured academic square cap | broadcloth | `WP-HEAD-HAT` | University, legal, medical, clerical, and learned-office gate; faculty colours and tassels are skins. |
| `renaissance_institution_physicians_roundcap` | full round physician cap | velvet | `WP-HEAD-CAP` | Physician, senior scholar, jurist, and civic learned-office dress where a round cap is locally supported. |
| `renaissance_iberian_folded_montera_cap` | folded felt riding cap | felt | `WP-HEAD-CAP` | Iberian rural, riding, courier, guard, pastoral, and travel admission; exact folds are skins. |
| `renaissance_maritime_tarred_brimhat` | tarred canvas brimmed seaman hat | canvas | `WP-HEAD-HAT` | Atlantic and Mediterranean shipboard, fishing, dock, and coastal-weather admission; no waterproof mechanic. |
| `renaissance_frontier_furlined_ridingcap` | fur-lined riding cap | wool | `WP-HEAD-CAP` | Central/Eastern frontier, Muscovite, Baltic, courier, and mounted winter dress; distinguish from tall ceremonial fur hats. |
| `renaissance_western_laced_buskins` | pair of calf-laced leather buskins | leather | `WP-FOOT-BOOT` | Calf-high laced footwear for travel, hunting, pageant, theatre, and classical-revival court presentation; distinct from low square-toed shoes. |
| `renaissance_western_side_laced_halfboots` | pair of side-laced leather half-boots | leather | `WP-FOOT-BOOT` | Ankle-to-lower-calf town and travel boots with a visible side closure; merchant, courier, guard, and country-household admissions. |
| `renaissance_western_fine_latchet_shoes` | pair of fine latchet shoes | leather | `WP-FOOT-SHOE` | Prosperous town, court, legal, diplomatic, and formal household footwear; tie or buckle skin. |
| `renaissance_western_furlined_lowshoes` | pair of fur-lined leather low shoes | leather | `WP-FOOT-SHOE` | Warm low footwear for northern and central European winter town, court, household, coach, and road use; lining is structural rather than cosmetic. |
| `renaissance_western_heeled_courtshoes` | pair of low-heeled court shoes | leather | `WP-FOOT-SHOE` | Late-sixteenth-century court and elite urban gate; heel remains moderate and period-bound. |
| `renaissance_western_hardsole_merchantshoes` | pair of hard-soled merchant shoes | leather | `WP-FOOT-SHOE` | Durable but respectable footwear for merchants, factors, notaries, travellers, and masters of craft. |
| `renaissance_western_hobnailed_fieldshoes` | pair of hobnailed field shoes | leather | `WP-FOOT-SHOE` | Agricultural, military-camp, mining, building, and long-road use; nails are visible reinforcement only. |
| `renaissance_western_shortcuff_ridingboots` | pair of short-cuffed riding boots | leather | `WP-FOOT-BOOT` | Calf-height riding, hunting, messenger, and country-gentry form distinct from high frontier boots. |
| `renaissance_maritime_tarred_highshoes` | pair of tarred high deck shoes | leather | `WP-FOOT-SHOE` | Ankle-covering deck, dock, fishing, and coastal-weather footwear; tar treatment is descriptive. |
| `renaissance_institution_closed_monastic_sandals` | pair of closed monastic sandals | leather | `WP-FOOT-SANDAL` | Institution-gated hybrid of strapped sandal and protective closed upper for mendicant or monastic travel; not universal order dress. |

## Shared admissions — 12 placements, no new prototypes

| Existing stable reference | Admitted grouping/context | Reason no clone is needed |
| --- | --- | --- |
| `renaissance_shared_clothing_drawstring_drawers` | Western, frontier, Iberian work outfits | same loose under-breech construction |
| `renaissance_shared_clothing_long_undershirt` | frontier and Ottoman provincial outfits | local naming and trim do not change form |
| `renaissance_shared_clothing_footed_stockings` | Western court, Central European town, Ottoman contact | same paired close legwear; skins carry clocks/colour |
| `renaissance_shared_clothing_leg_wraps` | Northern/Baltic labour and frontier military | same wrapped lower-leg construction |
| `renaissance_shared_clothing_hooded_cloak` | Iberian pilgrimage, Northern travel, Ottoman frontier | same cloak construction; use local skins |
| `renaissance_shared_clothing_low_leather_shoes` | Western work, frontier town, Ottoman provincial | ordinary low sewn shoe |
| `renaissance_shared_clothing_ankle_boots` | Iberian travel and Central European work | ordinary ankle boot construction |
| `renaissance_shared_clothing_felt_brimmed_hat` | Western, Iberian, colonial-contact outfit admission | brim/crown cosmetics are skins unless structure changes |
| `renaissance_shared_clothing_close_cloth_cap` | under-hood, domestic, scholar, and religious use | same close cap layer |
| `renaissance_shared_clothing_leather_gloves` | riding, guard, maritime, and work outfits | same glove construction; cuff treatment is cosmetic |
| `renaissance_shared_clothing_neck_kerchief` | artisan, sailor, servant, and rural outfits | same tied cloth form |
| `renaissance_shared_clothing_detachable_sleeves` | Western bodice/kirtle and Ottoman court layering | same paired detachable sleeve behaviour; skins carry local cut/trim |

## Skin minimums

Every implemented prototype should receive, where historically credible:

1. plain work or household skin;
2. standard urban/rural skin;
3. prosperous merchant or skilled-artisan skin;
4. court, civic, religious, or military-status skin only when admitted;
5. imported/contact skin only when the manifest records trade status.

No prototype needs every tier. Fur, brocade, damask, lace, metallic-thread, and jewelled-presentation skins must fail closed when their exact material inputs or intended status market are unavailable.

## Volume acceptance

- Exactly 154 unique stable references are defined here.
- The 12 shared admissions create no item prototype.
- Ottoman, Eastern European, and Iberian forms are not reduced to cosmetic Western skins where silhouette differs.
- Religious rows require a concrete institution and do not create a universal priest costume.
- Late silhouettes such as peascod doublets, standing ruffs, wheel farthingales, and structured late court dress are date-gated within the Renaissance window.
- Public names remain form-based; local historical terms remain builder anchors and skin notes.
