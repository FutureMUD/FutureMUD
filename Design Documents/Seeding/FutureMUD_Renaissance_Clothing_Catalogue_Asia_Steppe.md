# FutureMUD Renaissance Clothing Catalogue — Asian and Steppe Shared Cultures

## Purpose

This volume defines **130 unique clothing prototypes** for:

- Shared Persianate / Indo-Persian;
- Shared South Asian;
- Shared East Asian literati, including Ming and Joseon admissions;
- Shared Japanese and Shared maritime East Asian, including Ryukyuan admissions;
- Shared South-east Asian mainland and Shared maritime South-east Asian;
- Shared steppe and caravan.

The shared rules, exact-material contract, tag plan, common forms, and profile ledger are in [FutureMUD Renaissance Clothing and Accessories Design Reference](./FutureMUD_Renaissance_Clothing_Accessories_Design_Reference.md). Public names remain form-based. Historical names in notes are builder anchors and skin vocabulary, not universal in-world terminology.

## Volume-specific dependency notes

This volume makes the high-priority material gaps concrete:

- `ramie cloth` and `hemp cloth` are needed for faithful East Asian and Japanese common/work garments;
- `barkcloth` is needed for several maritime South-east Asian contexts;
- `brocade` and `silk gauze` are useful court materials but can initially be represented by `silk` skins if no craft behaviour depends on the distinction;
- exact `straw`, `fur`, and horsehair-like materials must be audited before rows using them are implemented.

Additional semantic wearable profiles used here are:

- `WP-SIDEFAST-ROBE`: long side-fastened or strongly asymmetric robe;
- `WP-DRAPED-FULL`: one unstitched garment wrapped around lower body and draped over torso/shoulder;
- `WP-TUBE-SKIRT`: sewn tubular lower-body garment;
- `WP-PLEATED-TROUSERS`: very full pleated divided or undivided lower garment;
- `WP-SHOULDER-WINGS`: sleeveless formal shoulder piece layered above a robe;
- `WP-SOCKS`: close foot garment worn under footwear;
- `WP-STRUCTURED-HEADWRAP`: turban or headcloth shaped around an internal cap/form;
- `WP-MONASTIC-DRAPE`: institution-gated draped robe occupying torso and shoulder layers.

Use an exact live wearable component where coverage and layering match. Otherwise seed the profile idempotently before its rows.

## A. Shared Persianate / Indo-Persian catalogue — 22

### Underlayers, trousers, coats, and court robes — 17

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_persianate_long_inner_shirt` | long collarless inner shirt | linen | `WP-SHIRT` | thigh/knee length; narrow/full sleeve; plain, fine, cotton skins |
| `renaissance_persianate_asymmetric_underrobe` | side-fastened inner robe | cotton | `WP-SIDEFAST-ROBE` | calf/ankle; narrow/full sleeve; court, merchant, military skins |
| `renaissance_persianate_close_ankle_trousers` | close ankle-length trousers | cotton | `WP-TROUSERS` | plain, striped, silk, and riding skins; not identical to bunched South Asian legwear |
| `renaissance_persianate_wide_pleated_trousers` | wide pleated trousers | wool | `WP-TROUSERS` | moderate/very full; steppe-border, court, military, travel skins |
| `renaissance_persianate_short_fitted_coat` | short fitted front-opening coat | wool | `WP-JACKET` | hip/thigh, narrow/full sleeve; qaba-like builder anchor |
| `renaissance_persianate_long_fitted_coat` | long fitted front-opening coat | broadcloth | `WP-ROBE-OPEN` | calf/ankle, split/unsplit skirt; merchant, scholar, court skins |
| `renaissance_persianate_flared_skirt_coat` | fitted flared-skirt coat | broadcloth | `WP-ROBE-OPEN` | narrow torso and widening skirt; riding, court, official skins |
| `renaissance_persianate_broad_skirt_courtrobe` | broad-skirted court robe | silk | `WP-ROBE-OPEN` | full skirt and long sleeves; figured, satin, velvet skins |
| `renaissance_persianate_short_sleeve_overrobe` | short-sleeved long over-robe | silk | `WP-ROBE-OPEN` | inner sleeves visible; court, diplomatic, official admissions |
| `renaissance_persianate_sleeveless_overrobe` | sleeveless long over-robe | silk | `WP-ROBE-OPEN` | inner robe visible; court, merchant, household skins |
| `renaissance_indopersian_sidefastened_jama` | side-fastened flared long coat | cotton | `WP-SIDEFAST-ROBE` | moderate skirt, narrow sleeves; Timurid/Mughal/Deccan builder anchor |
| `renaissance_indopersian_fullskirt_jama` | full-skirted side-fastened coat | silk | `WP-SIDEFAST-ROBE` | broad circular skirt; court, cavalry, ceremony and fine cotton skins |
| `renaissance_indopersian_diagonal_tie_jama` | diagonally tied long coat | cotton | `WP-SIDEFAST-ROBE` | visible diagonal closure integral; plain, striped, court skins |
| `renaissance_persianate_quilted_longcoat` | quilted long coat | cotton | `WP-ROBE-OPEN` | light/heavy quilting; travel, winter, military underlayer; no armour claim |
| `renaissance_persianate_fur_edged_longcoat` | fur-edged long coat | broadcloth | `WP-ROBE-OPEN` | exact fur audit; northern Persianate, court, merchant, diplomatic skins |
| `renaissance_persianate_sleeveless_longvest` | sleeveless long vest | wool | `WP-VEST` | hip/knee/calf skins only where profile permits; plain, furred, velvet variants |
| `renaissance_persianate_broadsleeve_scholarrobe` | broad-sleeved scholar robe | silk | `WP-ROBE-OPEN` | scholar, judge, court, manuscript-house skins; institution/status gate |

### Structured headwear and distinctive footwear — 5

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_persianate_conical_turban` | conical structured turban | cotton | `WP-STRUCTURED-HEADWRAP` | compact/full winding; scholar, military, court, merchant skins |
| `renaissance_persianate_tall_turban` | tall structured turban | silk | `WP-STRUCTURED-HEADWRAP` | court/official/diplomatic gate; plume and jewels remain skin presentation |
| `renaissance_persianate_domed_courtcap` | domed fitted court cap | velvet | `WP-HEAD-CAP` | plain, embroidered, fur-edged, jewelled-presentation skins |
| `renaissance_persianate_pointed_slippers` | pair of pointed soft slippers | leather | `WP-FOOT-SHOE` | backless/closed back, plain, velvet-covered, embroidered skins |
| `renaissance_persianate_soft_ridingboots` | pair of soft high riding boots | leather | `WP-FOOT-BOOT` | knee/calf, turned/plain cuff; cavalry, courier, court, caravan skins |

## B. Shared South Asian catalogue — 22

### Draped and stitched lower garments — 5

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_southasian_pleated_waistcloth` | long pleated waistcloth | cotton | `WP-WRAP-SKIRT` | knee/ankle, narrow/broad pleats, work/temple/court skins; dhoti-like builder anchor |
| `renaissance_southasian_split_riding_waistcloth` | divided riding waistcloth | cotton | `WP-PLEATED-TROUSERS` | wrapped to form two leg divisions; cavalry, courier, court, rural skins |
| `renaissance_southasian_stitched_loindrawers` | close stitched loin drawers | cotton | `WP-UNDER-WAIST` | short/knee, plain/fine; underlayer, labour, martial skins |
| `renaissance_southasian_gathered_longskirt` | full gathered long skirt | cotton | `WP-SKIRT` | ankle length; work, urban, dance, silk court skins |
| `renaissance_southasian_panelled_longskirt` | panelled flared long skirt | silk | `WP-SKIRT` | narrow/broad panels, plain/figured, court/merchant/ceremonial skins |

### Tunics, bodices, robes, and coats — 13

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_southasian_short_sideslit_tunic` | short side-slit tunic | cotton | `WP-SHIRT` | hip/thigh, narrow/full sleeve; work, urban, scholar skins |
| `renaissance_southasian_long_sideslit_tunic` | long side-slit tunic | cotton | `WP-ROBE-CLOSED` | knee/calf, narrow/full sleeve; court, temple, merchant, rural skins |
| `renaissance_southasian_flaredskirt_tunic` | flared-skirt long tunic | cotton | `WP-ROBE-CLOSED` | fitted torso and widened skirt; dance, court, prosperous urban skins |
| `renaissance_southasian_fitted_shortbodice` | fitted short bodice | cotton | `WP-FITTED-TORSO` | sleeveless/short sleeve only with exact profile; work, court, dance skins |
| `renaissance_southasian_backtied_bodice` | back-tied short bodice | cotton | `WP-FITTED-TORSO` | narrow/broad back ties; plain, silk, embroidered skins |
| `renaissance_southasian_longsleeve_bodice` | long-sleeved fitted bodice | silk | `WP-FITTED-TORSO` | court, dance, temple-patron, fine urban skins |
| `renaissance_southasian_shoulderdraped_garment` | full shoulder-draped garment | cotton | `WP-DRAPED-FULL` | lower wrap and shoulder fall are one item; sari-like builder anchor; silk court skins |
| `renaissance_southasian_short_wrapjacket` | short cross-tied jacket | cotton | `WP-JACKET` | waist/hip; work, household, martial, silk skins |
| `renaissance_southasian_long_crossover_robe` | long cross-over robe | cotton | `WP-SIDEFAST-ROBE` | knee/ankle; angarkha-like builder anchor; plain, court, scholar skins |
| `renaissance_southasian_sleeveless_courtovercoat` | sleeveless long court overcoat | silk | `WP-ROBE-OPEN` | calf/ankle; court, ministerial, diplomatic, merchant skins |
| `renaissance_southasian_quilted_cotton_longcoat` | quilted cotton long coat | cotton | `WP-ROBE-OPEN` | light/heavy quilting; winter, travel, cavalry, guard skins |
| `renaissance_southasian_bunched_ankletrousers` | narrow bunched ankle trousers | cotton | `WP-TROUSERS` | excess length gathered at ankle; court, cavalry, dance, silk skins |
| `renaissance_southasian_headbanded_courtveil` | headbanded long court veil | silk | `WP-HEAD-VEIL` | headband is integral; shoulder/waist length; court, bridal, ceremonial gate |

### Structured headwear and footwear — 4

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_southasian_fancrested_turban` | fan-crested structured turban | silk | `WP-STRUCTURED-HEADWRAP` | court, military command, procession, diplomatic skins; crest presentation is cosmetic |
| `renaissance_southasian_flat_wound_turban` | broad flat-wound turban | cotton | `WP-STRUCTURED-HEADWRAP` | work, merchant, scholar, court, regional skins |
| `renaissance_southasian_curvedtoe_shoes` | pair of curved-toe shoes | leather | `WP-FOOT-SHOE` | low/high vamp, plain, embroidered, velvet-covered skins |
| `renaissance_southasian_toepost_woodensandals` | pair of toe-post wooden sandals | wood | `WP-FOOT-SANDAL` | plain/carved, low/raised sole; household, temple, scholar, elite skins |

## C. Shared East Asian literati catalogue — Ming and Joseon — 26

### Ming-inspired shared forms — 17

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_ming_crosscollar_innerrobe` | cross-collared inner robe | ramie cloth | `WP-SIDEFAST-ROBE` | **material dependency**; hemp/linen fallback only by explicit temporary decision |
| `renaissance_ming_roundcollar_longrobe` | round-collared long robe | silk | `WP-ROBE-CLOSED` | scholar, official, merchant, court, cotton common skins |
| `renaissance_ming_straightcollar_openrobe` | straight-collared open robe | silk | `WP-ROBE-OPEN` | narrow/broad sleeves; scholar, household, court, merchant skins |
| `renaissance_ming_widesleeve_scholarrobe` | broad-sleeved scholar robe | silk | `WP-ROBE-CLOSED` | literati, teacher, ritual, court skins; institution/status admission |
| `renaissance_ming_narrowsleeve_workerrobe` | narrow-sleeved work robe | cotton | `WP-ROBE-CLOSED` | artisan, merchant, rural, servant, guard skins |
| `renaissance_ming_long_sleeveless_overvest` | long sleeveless over-vest | silk | `WP-VEST` | calf/ankle; court, household, official, merchant skins |
| `renaissance_ming_short_sidefastened_jacket` | short side-fastened jacket | cotton | `WP-JACKET` | waist/hip; work, household, child-size skin, silk court skin |
| `renaissance_ming_long_sidefastened_jacket` | long side-fastened jacket | cotton | `WP-JACKET` | thigh/knee; urban, rural, merchant, winter skins |
| `renaissance_ming_pleated_panelskirt` | pleated panel skirt | silk | `WP-SKIRT` | narrow/broad pleats; court, prosperous urban, cotton work skins |
| `renaissance_ming_horseface_panelskirt` | flat-fronted pleated panel skirt | silk | `WP-SKIRT` | broad front/back panels integral; court/urban/date gate |
| `renaissance_ming_wide_drawstring_trousers` | wide drawstring trousers | cotton | `WP-TROUSERS` | ankle/calf; work, household, martial, ramie skins |
| `renaissance_ming_narrow_ankletrousers` | narrow ankle-length trousers | cotton | `WP-TROUSERS` | riding, work, military, court-underlayer skins |
| `renaissance_ming_cloudshoulder_capelet` | shaped shoulder capelet | silk | `WP-SHOULDER` | lobed/rounded outline; court, ceremony, theatrical skins; motif is skin |
| `renaissance_ming_winged_officialhat` | winged formal official hat | silk | `WP-HEAD-HAT` | official institution/rank gate; wing length and surface skins |
| `renaissance_ming_soft_scholarcap` | soft folded scholar cap | silk | `WP-HEAD-CAP` | scholar, teacher, merchant, household skins |
| `renaissance_ming_gauze_officialhat` | structured gauze official hat | silk gauze | `WP-HEAD-HAT` | **material dependency**; official/date gate; plain silk fallback only temporarily |
| `renaissance_ming_cloth_courtboots` | pair of high cloth court boots | cotton | `WP-FOOT-BOOT` | black/plain, official, scholar, court, felt-lined winter skins |

### Joseon-inspired shared forms — 9

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_joseon_long_crossfront_underrobe` | long cross-front under-robe | ramie cloth | `WP-SIDEFAST-ROBE` | **material dependency**; hemp/linen temporary fallback; scholar/common/court skins |
| `renaissance_joseon_short_crossfront_jacket` | short cross-front jacket | ramie cloth | `WP-JACKET` | waist/hip; work, household, cotton, silk court skins |
| `renaissance_joseon_long_crossfront_jacket` | long cross-front jacket | silk | `WP-JACKET` | thigh/knee; early-Joseon date gate; court, scholar, prosperous urban skins |
| `renaissance_joseon_full_gathered_wrapskirt` | full gathered wrap skirt | silk | `WP-WRAP-SKIRT` | broad overlap and high waist; cotton/ramie common skins |
| `renaissance_joseon_broadsleeve_scholaroverrobe` | broad-sleeved scholar over-robe | silk | `WP-ROBE-OPEN` | scholar, official, teacher, ritual skins |
| `renaissance_joseon_pleated_officialovercoat` | pleated long official coat | silk | `WP-ROBE-OPEN` | official/rank/institution gate; colour and insignia are skins |
| `renaissance_joseon_tall_horsehairhat` | tall translucent brimmed hat | horsehair | `WP-HEAD-HAT` | **material audit**; scholar/official/date gate; no generic straw substitution |
| `renaissance_joseon_rounded_officialcap` | rounded formal official cap | silk | `WP-HEAD-CAP` | court/official/ritual gate; winged variants require a new silhouette |
| `renaissance_joseon_white_clothshoes` | pair of white cloth shoes | cotton | `WP-FOOT-SHOE` | plain, padded, court, mourning/status skins as admitted locally |

## D. Shared Japanese and maritime East Asian catalogue — 20

### Japanese shared forms — 16

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_japanese_smallsleeve_wraprobe` | small-sleeved wrap robe | silk | `WP-SIDEFAST-ROBE` | kosode-like anchor; hemp/cotton work, silk court, patterned skins |
| `renaissance_japanese_unlined_summerrobe` | unlined summer wrap robe | hemp cloth | `WP-SIDEFAST-ROBE` | **material dependency**; ramie/linen temporary decision only; katabira-like anchor |
| `renaissance_japanese_heavy_outerrobe` | heavy lined outer robe | silk | `WP-ROBE-OPEN` | court, warrior household, formal, theatrical skins; uchikake-like anchor |
| `renaissance_japanese_broadsleeve_formalrobe` | broad-sleeved formal robe | silk | `WP-SIDEFAST-ROBE` | court, shrine/temple, warrior ceremony, performance gate |
| `renaissance_japanese_shoulderwing_vest` | stiff shoulder-wing vest | broadcloth | `WP-SHOULDER-WINGS` | kataginu-like anchor; warrior, official, pageant, formal household admission |
| `renaissance_japanese_short_openjacket` | short open-front jacket | silk | `WP-JACKET` | early haori-like anchor; merchant, warrior, travel, cotton work skins |
| `renaissance_japanese_full_pleated_hakama` | full pleated lower garment | silk | `WP-PLEATED-TROUSERS` | undivided or visually skirt-like construction fixed by this prototype; court/ritual skins |
| `renaissance_japanese_divided_riding_hakama` | divided pleated riding trousers | silk | `WP-PLEATED-TROUSERS` | warrior, courier, equestrian, formal skins; distinct divided construction |
| `renaissance_japanese_field_trousers` | close field trousers | hemp cloth | `WP-TROUSERS` | **material dependency**; farmer, hunter, messenger, foot-soldier skins |
| `renaissance_japanese_quilted_sleevelessvest` | quilted sleeveless vest | cotton | `WP-VEST` | winter, worker, warrior-underlayer, household skins |
| `renaissance_japanese_straw_raincape` | layered straw rain cape | straw | `WP-CLOAK` | exact straw audit; farmer, traveller, courier, foot-soldier admissions |
| `renaissance_japanese_woven_strawcoat` | long woven straw coat | straw | `WP-ROBE-OPEN` | exact straw audit; rain/snow travel presentation, no waterproof mechanic claim |
| `renaissance_japanese_soft_folded_courtcap` | tall folded soft cap | silk | `WP-HEAD-CAP` | eboshi-like anchor; court, warrior, shrine, formal-status skins |
| `renaissance_japanese_lacquered_conicalhat` | lacquered conical hat | wood | `WP-HEAD-HAT` | traveller, warrior, monk, courier, official skins; lacquer is presentation unless material exists |
| `renaissance_japanese_splittoe_socks` | pair of split-toe socks | cotton | `WP-SOCKS` | ankle/calf, plain, padded, formal skins; compatible footwear profile required |
| `renaissance_japanese_wooden_clogs` | pair of raised wooden clogs | wood | `WP-FOOT-SHOE` | low/high teeth, plain/lacquered, urban/rural/rain skins |

### Ryukyuan and maritime East Asian shared forms — 4

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_ryukyu_widesleeve_summerrobe` | broad-sleeved tropical wrap robe | ramie cloth | `WP-SIDEFAST-ROBE` | **material dependency**; court, maritime merchant, household, ceremonial skins |
| `renaissance_ryukyu_short_wrapjacket` | short tropical wrap jacket | cotton | `WP-JACKET` | household, work, maritime, court silk skins |
| `renaissance_ryukyu_pleated_wrapskirt` | pleated tropical wrap skirt | cotton | `WP-WRAP-SKIRT` | ankle/calf, plain/patterned, work/court skins |
| `renaissance_ryukyu_formal_courtcap` | structured island court cap | silk | `WP-HEAD-HAT` | tribute/diplomatic/court gate; colour and rank presentation are skins |

## E. Shared South-east Asian mainland and maritime catalogue — 12

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_seasia_sewn_tubeskirt` | sewn tubular skirt | cotton | `WP-TUBE-SKIRT` | ankle/calf; plain, patterned, silk court, work skins |
| `renaissance_seasia_pleated_courtwaistcloth` | pleated formal waistcloth | silk | `WP-WRAP-SKIRT` | broad front pleats; mainland court, temple patron, diplomatic skins |
| `renaissance_seasia_split_riding_waistcloth` | divided riding waistcloth | cotton | `WP-PLEATED-TROUSERS` | riding, martial, court, travel skins; wrapped divided construction |
| `renaissance_seasia_short_collarless_jacket` | short collarless jacket | cotton | `WP-JACKET` | waist/hip, narrow/full sleeve, work, merchant, court skins |
| `renaissance_seasia_long_crossfront_courtrobe` | long cross-front court robe | silk | `WP-SIDEFAST-ROBE` | mainland court/official/ritual gate; patterned and brocade skins |
| `renaissance_seasia_sleeveless_courtvest` | long sleeveless court vest | silk | `WP-VEST` | hip/knee; court, diplomatic, temple-service skins |
| `renaissance_seasia_fitted_dancebodice` | fitted ceremonial dance bodice | silk | `WP-FITTED-TORSO` | performance/temple/court gate; bead and metallic decoration are skins |
| `renaissance_seasia_full_ceremonialskirt` | full panelled ceremonial skirt | silk | `WP-SKIRT` | court, temple, performance, bridal skins; train only as description |
| `renaissance_seasia_structured_headclothcrown` | crown-shaped structured headcloth | silk | `WP-STRUCTURED-HEADWRAP` | court/official/ritual gate; folding and colour are skins |
| `renaissance_seasia_palmleaf_conicalhat` | broad palm-leaf conical hat | barkcloth | `WP-HEAD-HAT` | **material dependency or exact palm/straw audit**; field, river, market, maritime skins |
| `renaissance_seasia_wrapped_monasticrobe` | draped monastic shoulder robe | cotton | `WP-MONASTIC-DRAPE` | named Buddhist institution and local date gate; colour/order skins |
| `renaissance_seasia_short_maritime_trousers` | short full maritime trousers | cotton | `WP-TROUSERS` | sailor, fisher, dockworker, boatman, martial skins |

## F. Shared steppe and caravan catalogue — 8

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_steppe_sidefastened_furcoat` | side-fastened fur-lined long coat | wool | `WP-SIDEFAST-ROBE` | exact fur audit; court, caravan, herder, winter, diplomatic skins |
| `renaissance_steppe_short_split_ridingcoat` | short split-skirt riding coat | wool | `WP-JACKET` | cavalry, courier, herder, caravan guard skins |
| `renaissance_steppe_sleeveless_ridingvest` | sleeveless riding vest | leather | `WP-VEST` | leather, felt, wool, fur-edged skins; travel and mounted work |
| `renaissance_steppe_quilted_ridingrobe` | quilted side-fastened riding robe | cotton | `WP-SIDEFAST-ROBE` | light/heavy quilting; winter, military, caravan, court skins |
| `renaissance_steppe_wide_ridingtrousers` | wide reinforced riding trousers | wool | `WP-TROUSERS` | leather-seat presentation skin, plain/felted, elite/work variants |
| `renaissance_steppe_pointed_feltcap` | pointed felt riding cap | felt | `WP-HEAD-CAP` | plain, fur-edged, feathered-presentation skins |
| `renaissance_steppe_fur_earflaphat` | furred ear-flap hat | fur | `WP-HEAD-HAT` | **exact material required**; tied-up/down is descriptive unless component supports state |
| `renaissance_steppe_soft_highboots` | pair of soft high riding boots | leather | `WP-FOOT-BOOT` | knee/calf, turned/plain cuff, felt-lined winter skin |

## G. Headwear and footwear expansion — 20

These rows are additional construction-level options. They do not alter the existing outfit minimums or first-pass shared-placement count; a builder should admit them only where status, work, route, climate, institution, or local dress practice justifies the distinct form.

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_persianate_twelvegore_tallcap` | twelve-gored tall felt cap | felt | `WP-HEAD-HAT` | Safavid-facing military, court, devotional, and diplomatic anchor; gore count, colour, and attached winding require local date and institution. |
| `renaissance_persianate_furedge_domedcap` | fur-edged domed court cap | velvet | `WP-HEAD-CAP` | Northern Persianate, merchant, court, diplomatic, and winter skins; distinct fur edge from the existing plain domed cap. |
| `renaissance_persianate_quilted_ridingturban` | quilted riding turban | cotton | `WP-STRUCTURED-HEADWRAP` | Cavalry, courier, caravan, and cold-weather admission; no armour claim. |
| `renaissance_southasian_chintied_ridingturban` | chin-tied riding turban | cotton | `WP-STRUCTURED-HEADWRAP` | Mounted courier, hunting, military-retainer, and long-road use; chin tie is integral silhouette. |
| `renaissance_southasian_beaded_silk_hairnet` | beaded silk court hairnet | silk | `WP-HEAD-CAP` | Netted court hair covering for locally admitted women in court, dance, ceremonial, and prosperous household settings; bead and pearl patterns are skins. |
| `renaissance_ming_fourcorner_scholarcap` | four-cornered scholar cap | silk | `WP-HEAD-HAT` | Scholar, teacher, ritual, court, and theatrical gate; exact corner and wing treatment locally admitted. |
| `renaissance_ming_woven_bamboo_rainhat` | broad ribbed bamboo rain hat | bamboo | `WP-HEAD-HAT` | Farmer, boatman, courier, porter, soldier, and traveller admission; closely set ribs and a tied crown distinguish it from the simpler medieval travel form; no waterproof mechanic. |
| `renaissance_joseon_narrowbrim_horsehairhat` | narrow-brim horsehair scholar hat | horsehair | `WP-HEAD-HAT` | Less formal and less expansive than the tall broad-brim official form; scholar, town, and travel gate. |
| `renaissance_joseon_padded_winterhood` | padded winter head hood | cotton | `WP-HOOD` | Women, children, household, travel, and cold-season town dress; exact shape and status locally gated. |
| `renaissance_japanese_stiff_lacquered_eboshi` | stiff lacquered folded cap | silk | `WP-HEAD-CAP` | Warrior, court, shrine, official, and formal household gate; distinct rigid treatment from the soft court cap. |
| `renaissance_japanese_broad_sedge_travelhat` | broad sedge travel hat | straw | `WP-HEAD-HAT` | Traveller, pilgrim, courier, rural, river, and foot-soldier contexts; ties and crown depth are skins. |
| `renaissance_persianate_hardsole_ridingshoes` | pair of hard-soled riding shoes | leather | `WP-FOOT-SHOE` | Low heeled or flat mounted footwear for court, caravan, courier, and military-retainer use where high boots are unnecessary. |
| `renaissance_persianate_closedback_embroideredslippers` | pair of closed-back embroidered slippers | leather | `WP-FOOT-SHOE` | Prosperous urban, court, scholarly, and diplomatic footwear distinct from backless pointed slippers. |
| `renaissance_southasian_leather_fieldsandals` | pair of hard-wearing field sandals | leather | `WP-FOOT-SANDAL` | Agricultural, porter, messenger, martial, and long-road use; broad straps and stout sole distinguish them from fine sandals. |
| `renaissance_ming_black_cloth_scholarshoes` | pair of black cloth scholar shoes | cotton | `WP-FOOT-SHOE` | Scholar, teacher, merchant, ritual, and household dress; padded and fine silk skins where locally credible. |
| `renaissance_ming_woven_straw_rainovershoes` | pair of woven straw rain overshoes | straw | `WP-OVERSHOE` | Slip-over woven protection for cloth shoes during field, river, porter, and wet-road use; no waterproof mechanic. |
| `renaissance_joseon_straw_everydayshoes` | pair of woven straw everyday shoes | straw | `WP-FOOT-SHOE` | Common rural, market, porter, and travel footwear for locally admitted Joseon contexts. |
| `renaissance_joseon_white_leather_courtboots` | pair of white leather court boots | leather | `WP-FOOT-BOOT` | Court, guard, ritual, diplomatic, and formal riding gate; colour and office require local admission. |
| `renaissance_japanese_tied_straw_travelsandals` | pair of tightly braided straw road sandals | straw | `WP-FOOT-SANDAL` | Road, pilgrimage, courier, field, and foot-soldier use; dense braid and long securing cords distinguish this form from generic woven sandals. |
| `renaissance_seasia_embroidered_silk_courtshoes` | pair of embroidered silk court shoes | silk | `WP-FOOT-SHOE` | Closed court footwear for mainland and maritime South-east Asian court, temple-service, dance, diplomatic, and elite household admission. |

## Shared admissions — 14 placements, no new prototypes

| Existing stable reference | Admitted grouping/context | Reason no clone is needed |
| --- | --- | --- |
| `renaissance_shared_clothing_straight_underrobe` | Persianate, South Asian, Ming/Joseon, South-east Asian underlayers | straight robe construction unchanged; local collar/trim skins |
| `renaissance_shared_clothing_narrow_trousers` | Persianate, South Asian, Ming/Joseon, Japanese riding/work | same joined narrow-trouser behaviour |
| `renaissance_shared_clothing_full_trousers` | Persianate, South Asian, South-east Asian maritime | same full gathered construction where no distinctive fastening exists |
| `renaissance_shared_clothing_crossfront_jacket` | Ming/Joseon, Japanese/Ryukyuan, mainland South-east Asian work | same cross-front short-jacket silhouette; skins carry local cut |
| `renaissance_shared_clothing_wrap_skirt` | South Asian work, East/South-east Asian, Ryukyuan | ordinary overlapping wrap where tube/panel construction is absent |
| `renaissance_shared_clothing_quilted_jacket` | Persianate, Japanese, steppe, Ming/Joseon winter/military underlayers | same short quilted construction |
| `renaissance_shared_clothing_shoulder_shawl` | Persianate, South Asian, steppe, East Asian court/contact | same rectangular shoulder layer; fibre/motif skins |
| `renaissance_shared_clothing_textile_sandals` | Japanese, Ming/Joseon rural, South-east Asian, South Asian work | same woven sandal form where split-toe or toe-post construction is absent |
| `renaissance_shared_clothing_soft_slippers` | Persianate, South Asian, Ming/Joseon court/household | rounded ordinary slipper; pointed/curved forms remain distinct rows |
| `renaissance_shared_clothing_cloth_headwrap` | Persianate, South Asian, South-east Asian, steppe work | unstructured winding; structured turbans retain distinct prototypes |
| `renaissance_shared_clothing_long_head_veil` | Persianate, South Asian, South-east Asian court/religious | ordinary head-draped veil; headbanded form remains distinct |
| `renaissance_shared_clothing_close_cloth_cap` | under-turban, scholar, household, religious use | same close cap construction |
| `renaissance_shared_clothing_footwraps` | steppe, Japanese field, Ming/Joseon work and military | same cloth wrapping under footwear |
| `renaissance_shared_clothing_rain_cape` | Japanese, South-east Asian, Himalayan/steppe-border travel | use where textile cape is correct; straw coats remain distinct |

## Outfit minimums by Shared grouping

The authority's inferred manifest table implements these minimums as sixteen stock ordinary/status outfits. The concrete item lists, admission text, and inner-to-outer wear order live in that table.

| Grouping | Ordinary outfit | High-status/institutional outfit |
| --- | --- | --- |
| Persianate / Indo-Persian | inner shirt, trousers, fitted coat or jama, headwrap/cap, slippers or boots | layered silk robe, structured turban/cap, court footwear, optional shawl |
| South Asian | waistcloth or skirt, tunic/bodice or draped full garment, headcloth, sandals | silk panelled skirt or jama/robe, structured turban/veil, curved shoes, court overcoat |
| Ming / Joseon literati | inner robe, trousers/skirt, jacket/robe, cloth shoes, cap | broad-sleeved or official robe, institution-gated hat, court boots/shoes |
| Japanese / Ryukyuan | wrap robe, hakama/skirt as admitted, socks/sandals/clogs, cap/hat | formal robe, shoulder-wing vest or outer robe, pleated lower garment, formal cap |
| South-east Asian | tube/wrap skirt or maritime trousers, short jacket/upper cloth, sandals/hat | silk court robe/skirt/vest, structured headcloth, ceremonial footwear |
| steppe / caravan | shirt/robe, riding trousers, side-fastened coat/vest, cap, boots | fur-lined or quilted robe, formal cap, shawl, high riding boots |

Outfits use local admission and date gates. They must not automatically attach modern ethnic identity, national uniformity, or universal gender rules to a silhouette.

## Volume acceptance

- Exactly 130 unique stable references are defined here.
- The 14 shared admissions create no prototype.
- Wrapped, side-fastened, cross-collared, and pleated forms are not forced into Western dress profiles.
- Ming, Joseon, Japanese, Ryukyuan, Persianate, South Asian, South-east Asian, and steppe rows retain distinct silhouettes where a skin would misrepresent construction.
- `ramie cloth`, `hemp cloth`, `barkcloth`, silk-gauze, straw, fur, and horsehair dependencies fail closed or use an explicitly documented temporary fallback.
- Court, official, scholar, monastic, and ritual garments require institutional/status admission rather than ordinary market ubiquity.
