# FutureMUD Renaissance Clothing Catalogue — African, American, Contact, and Maritime Shared Cultures

## Purpose and sensitivity boundary

This volume defines **116 unique clothing prototypes** for:

- Shared African court / Atlantic;
- Shared Sahelian Islamic;
- Shared Red Sea;
- Shared Indian Ocean and Swahili coast;
- Shared Mesoamerican;
- Shared Andean;
- Shared Caribbean contact;
- Shared North American contact;
- Shared first-contact / colonial, Shared colonial Atlantic, and Shared global maritime overlays.

The design authority and common catalogue are in [FutureMUD Renaissance Clothing and Accessories Design Reference](./FutureMUD_Renaissance_Clothing_Accessories_Design_Reference.md).

These are builder-facing Shared groupings, not assertions that large regions had uniform dress. Every implementation row requires a narrower inspiration, date anchor, social/institutional setting, and local-admission note. Ritual, office, royal, diplomatic, military, mourning, initiation, and sacred garments must not enter ordinary shop stock merely because a prototype exists.

Contact and colonial rows must identify whether a form is local continuity, imported, imposed, mission-associated, made for export, or genuinely hybrid. They must not frame colonisation as neutral cultural exchange or treat imported European dress as automatic technological progression.

## Volume-specific dependency notes

High-priority materials:

- `raffia cloth` for woven and fringed palm-fibre garments;
- `barkcloth` for beaten-bark garments and capes;
- `camelid wool` for Andean textiles;
- `featherwork` for garments whose primary visible surface is structured feather mosaic or tied feather panels;
- `beadwork` for garments whose construction is a bead lattice or densely beaded panel rather than ordinary textile with decorative beads.

`cotton`, `linen`, `wool`, `leather`, `silk`, `felt`, and `canvas` are live and cover many rows. Exact `fur`, `hide`, `straw`, shell, horsehair, and wood-like dependencies must be audited against the maintained material export.

Additional semantic wearable profiles used here:

- `WP-DOUBLE-WRAP`: two coordinated wrapper panels authored as one garment where they are inseparable in silhouette;
- `WP-RECTANGULAR-BLOUSE`: rectangular sleeveless or short-sleeved upper garment with head opening;
- `WP-TRIANGLE-SHOULDER`: triangular or diamond-shaped shoulder garment;
- `WP-WRAP-DRESS`: one broad cloth wrapped as a full-length dress;
- `WP-BREECHCLOTH`: front-and-back panel lower garment distinct from a short loin wrap;
- `WP-MOCCASIN`: soft closed hide footwear with gathered or seamed upper;
- `WP-FEATHER-CROWN`: structured feather headwear occupying the hat layer;
- `WP-FULL-MASK`: full rigid face covering;
- `WP-HYBRID-TUNIC`: contact-zone tunic profile only where European fastening materially changes a local cut.

Use existing exact components when coverage/layering matches. Otherwise these are blocking wearable definitions, not new engine component types.

## A. Shared African court / Atlantic catalogue — 25

### Wrapped, sewn, and court textile garments — 12

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_africancourt_narrow_waistwrapper` | narrow woven waist wrapper | cotton | `WP-WRAP-SKIRT` | knee/calf; work, household, court-attendant, trade-cloth skins; narrower inspiration required |
| `renaissance_africancourt_broad_waistwrapper` | broad full-length waist wrapper | cotton | `WP-WRAP-SKIRT` | ankle length; plain, indigo, patterned, silk-import skins as locally admitted |
| `renaissance_africancourt_sewn_tubewrapper` | sewn tubular wrapper | cotton | `WP-TUBE-SKIRT` | narrow/full tube; market, household, court, ceremonial skins |
| `renaissance_africancourt_doublelayer_wrapper` | layered double wrapper | cotton | `WP-DOUBLE-WRAP` | two coordinated panels integral to silhouette; court, marriage, ceremony, prosperous urban gate |
| `renaissance_africancourt_sleeveless_straighttunic` | sleeveless straight woven tunic | cotton | `WP-RECTANGULAR-BLOUSE` | hip/knee; plain, striped, patterned, prestige skins |
| `renaissance_africancourt_shortsleeve_tunic` | short-sleeved straight tunic | cotton | `WP-SHIRT` | hip/knee; work, merchant, household, court-attendant skins |
| `renaissance_africancourt_longsleeve_sideslit_tunic` | long-sleeved side-slit tunic | cotton | `WP-SHIRT` | knee/calf; urban, court, diplomatic, merchant skins |
| `renaissance_africancourt_broadsleeve_courtrobe` | broad-sleeved full court robe | silk | `WP-ROBE-CLOSED` | court/office/diplomatic gate; imported textile status recorded in manifest |
| `renaissance_africancourt_fitted_shortjacket` | fitted short court jacket | cotton | `WP-JACKET` | waist/hip; court, guard, diplomatic-contact, silk skins |
| `renaissance_africancourt_long_open_courtcoat` | long open-front court coat | silk | `WP-ROBE-OPEN` | court/office/procession gate; brocade and imported-cloth skins |
| `renaissance_africancourt_full_gathered_courtskirt` | full gathered court skirt | cotton | `WP-SKIRT` | ankle length; plain/patterned/silk; court and ceremony admission |
| `renaissance_africancourt_panelled_danceskirt` | panelled ceremonial dance skirt | cotton | `WP-SKIRT` | performance, court, festival, ritual gate; bells and jewellery remain separate items |

### Raffia, barkcloth, beadwork, featherwork, and hide regalia — 9

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_africancourt_raffia_fringe_skirt` | fringed raffia skirt | raffia cloth | `WP-SKIRT` | **material dependency**; court, performance, initiation, festival only through local manifest |
| `renaissance_africancourt_raffia_shouldercape` | layered raffia shoulder cape | raffia cloth | `WP-SHOULDER` | **material dependency**; ceremonial/status gate; length and dye are skins |
| `renaissance_africancourt_barkcloth_straighttunic` | straight barkcloth tunic | barkcloth | `WP-RECTANGULAR-BLOUSE` | **material dependency**; local forest/central-African-facing admission only |
| `renaissance_africancourt_barkcloth_wrapskirt` | broad barkcloth wrap skirt | barkcloth | `WP-WRAP-SKIRT` | **material dependency**; work, household, court skin only where locally credible |
| `renaissance_africancourt_beadwork_ceremonialapron` | dense beaded ceremonial apron | beadwork | `WP-WRAP-SKIRT` | **material dependency**; court/office/ritual gate; beads are structural surface |
| `renaissance_africancourt_beaded_chestmantle` | lattice beaded chest mantle | beadwork | `WP-SHOULDER` | **material dependency**; court/regalia/diplomatic gate; not ordinary jewellery stock |
| `renaissance_africancourt_featherwork_shouldermantle` | layered feather shoulder mantle | featherwork | `WP-SHOULDER` | **material dependency**; court/ritual/military-status gate; species/pattern in skin notes |
| `renaissance_africancourt_hide_prestigecloak` | spotted-hide prestige cloak | leather | `WP-CLOAK` | use generic leather only temporarily; court/office/warrior-status gate and ethical source note |
| `renaissance_africancourt_indigo_worksmock` | loose indigo work smock | cotton | `WP-LONG-UNDERLAYER` | artisan, dyer, market, agricultural, household skins; colour family is a skin |

### Headwear and distinctive court footwear — 4

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_africancourt_tall_wovencap` | tall close-woven cap | cotton | `WP-HEAD-CAP` | plain, striped, embroidered, court/office skins |
| `renaissance_africancourt_soft_embroideredcap` | soft embroidered round cap | cotton | `WP-HEAD-CAP` | merchant, court, scholar/contact, household skins |
| `renaissance_africancourt_structured_crownheadcloth` | crown-shaped structured headcloth | cotton | `WP-STRUCTURED-HEADWRAP` | court/office/ceremony gate; fold and motif are skins |
| `renaissance_africancourt_raised_woodensandals` | pair of raised wooden court sandals | wood | `WP-FOOT-SANDAL` | court/household/ceremonial gate; low/high platform skins |

## B. Shared Sahelian Islamic, Red Sea, and Indian Ocean catalogue — 20

### Sahelian Islamic shared forms — 8

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_sahel_broad_rectangular_robe` | broad rectangular over-robe | cotton | `WP-ROBE-CLOSED` | boubou-like builder anchor; scholar, merchant, court, ordinary skins by local date/status |
| `renaissance_sahel_narrow_longtunic` | narrow long side-slit tunic | cotton | `WP-SHIRT` | knee/calf; work, rider, merchant, scholar skins |
| `renaissance_sahel_embroidered_necktunic` | broad embroidered-neck tunic | cotton | `WP-ROBE-CLOSED` | embroidery as skin; merchant, scholar, court, festival gate |
| `renaissance_sahel_veryfull_trousers` | very full drawstring trousers | cotton | `WP-TROUSERS` | ankle/calf gathering; riding, court, merchant, work skins |
| `renaissance_sahel_leather_ridingcoat` | long leather riding coat | leather | `WP-ROBE-OPEN` | caravan, cavalry, courier, elite travel; no armour claim |
| `renaissance_sahel_scholar_turban` | layered scholar's turban | cotton | `WP-STRUCTURED-HEADWRAP` | madrasa, court, legal, merchant skins; local institution/status gate |
| `renaissance_sahel_conical_leatherhat` | conical leather riding hat | leather | `WP-HEAD-HAT` | rider, caravan, rural, military-status skins |
| `renaissance_sahel_indigo_faceveil` | long indigo face veil | cotton | `WP-FACE-VEIL` | culture/gender/status gate; not universal Sahelian clothing |

### Ethiopian / Red Sea shared forms — 6

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_redsea_full_shoulderwrap` | full body-and-shoulder wrap | cotton | `WP-DRAPED-FULL` | highland/Red Sea-facing admission; plain, bordered, court, religious skins |
| `renaissance_redsea_narrow_cotton_tunic` | narrow long cotton tunic | cotton | `WP-SHIRT` | knee/calf; work, soldier, court-attendant, religious skins |
| `renaissance_redsea_long_courtshirt` | long full-sleeved court shirt | cotton | `WP-ROBE-CLOSED` | court, diplomatic, church-associated, merchant skins |
| `renaissance_redsea_embroidered_shouldercape` | embroidered shoulder cape | cotton | `WP-SHOULDER` | court, church, procession, diplomatic gate; symbols remain skins |
| `renaissance_redsea_leather_highlandcloak` | broad leather highland cloak | leather | `WP-CLOAK` | herder, rider, soldier, travel skins; hide source in builder note |
| `renaissance_redsea_white_headhood` | white head-draped hood | cotton | `WP-HOOD` | religious, court, household, mourning/status skins by local manifest |

### Swahili / Indian Ocean shared forms — 6

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_indianocean_long_coastalrobe` | long collarless coastal robe | cotton | `WP-ROBE-CLOSED` | Swahili/Indian Ocean urban gate; plain, striped, imported-silk skins |
| `renaissance_indianocean_short_coastaltunic` | short loose coastal tunic | cotton | `WP-SHIRT` | sailor, artisan, merchant, household, work skins |
| `renaissance_indianocean_open_merchantcoat` | light open-front merchant coat | cotton | `WP-ROBE-OPEN` | port merchant, broker, scholar, diplomatic-contact skins |
| `renaissance_indianocean_sewn_wrapskirt` | sewn coastal wrap skirt | cotton | `WP-TUBE-SKIRT` | household, market, maritime, fine imported-cloth skins |
| `renaissance_indianocean_embroidered_roundcap` | close embroidered round cap | cotton | `WP-HEAD-CAP` | merchant, scholar, mosque/madrasa, sailor skins; institution gate where relevant |
| `renaissance_indianocean_toeloop_sandals` | pair of toe-loop leather sandals | leather | `WP-FOOT-SANDAL` | urban, maritime, traveller, elite-decorated skins; distinct toe-loop construction |

## C. Shared Mesoamerican catalogue — 18

Public names remain descriptive. Builder notes may record narrower Mexica, Maya/Postclassic, Mixtec, or other anchors only where the associated local manifest supports them.

### Core garments — 8

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_mesoamerican_panelled_loincloth` | long panelled loin garment | cotton | `WP-BREECHCLOTH` | narrow/broad front and back panels; work, warrior-status, court skins |
| `renaissance_mesoamerican_shouldertied_cloak` | shoulder-tied rectangular cloak | cotton | `WP-SHOULDER` | hip/ankle; plain, patterned, tribute, court skins; knot side is cosmetic |
| `renaissance_mesoamerican_rectangular_blouse` | rectangular sleeveless woven blouse | cotton | `WP-RECTANGULAR-BLOUSE` | hip/thigh; plain, patterned, court, household skins; huipil-like anchor |
| `renaissance_mesoamerican_triangle_shouldergarment` | triangular shoulder garment | cotton | `WP-TRIANGLE-SHOULDER` | short/long points; household, court, ceremony, fine skins; quechquemitl-like anchor |
| `renaissance_mesoamerican_short_wrapskirt` | short woven wrap skirt | cotton | `WP-WRAP-SKIRT` | knee/calf; household, market, work, patterned skins |
| `renaissance_mesoamerican_long_wrapskirt` | long woven wrap skirt | cotton | `WP-WRAP-SKIRT` | ankle; plain, patterned, court, ceremony skins |
| `renaissance_mesoamerican_side_seamed_tunic` | side-seamed sleeveless tunic | cotton | `WP-RECTANGULAR-BLOUSE` | knee/thigh; court, military-status, ritual, ordinary skins by local admission |
| `renaissance_mesoamerican_full_ceremonialtunic` | long full ceremonial tunic | cotton | `WP-ROBE-CLOSED` | court/ritual/office gate; woven and embroidered presentation are skins |

### Featherwork, barkcloth, shell/bead, hide, masks, and footwear — 10

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_mesoamerican_featherwork_mantle` | mosaic feather mantle | featherwork | `WP-SHOULDER` | **material dependency**; court/ritual/office/diplomatic gate |
| `renaissance_mesoamerican_featherwork_tunic` | panelled feather tunic | featherwork | `WP-RECTANGULAR-BLOUSE` | **material dependency**; elite/ritual/military-status only through local manifest |
| `renaissance_mesoamerican_tall_feathercrown` | tall radiating feather crown | featherwork | `WP-FEATHER-CROWN` | **material dependency**; office/ritual/court gate; exact species and colour are skins |
| `renaissance_mesoamerican_broad_featherheadband` | broad feathered headband | featherwork | `WP-HEAD-HAT` | court, warrior-status, dance, ritual skins; not ordinary universal wear |
| `renaissance_mesoamerican_woven_headcap` | close woven fibre cap | cotton | `WP-HEAD-CAP` | plain, patterned, court, work skins |
| `renaissance_mesoamerican_woven_fibresandals` | pair of woven fibre sandals | canvas | `WP-FOOT-SANDAL` | cotton/agave-like builder skins; exact agave fibre may be future material |
| `renaissance_mesoamerican_shellbead_apron` | shell-and-bead ceremonial apron | beadwork | `WP-WRAP-SKIRT` | **material dependency**; ritual/office/court gate |
| `renaissance_mesoamerican_painted_barkclothcape` | painted barkcloth shoulder cape | barkcloth | `WP-SHOULDER` | **material dependency**; ritual, tribute, court, festival gate |
| `renaissance_mesoamerican_spottedhide_mantle` | spotted-hide shoulder mantle | leather | `WP-SHOULDER` | court/warrior-status/ritual gate; source and symbolism in builder note |
| `renaissance_mesoamerican_carved_fullfacemask` | carved full-face mask | wood | `WP-FULL-MASK` | ritual, performance, court pageant gate; no disguise mechanic unless later supported |

## D. Shared Andean catalogue — 14

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_andean_straight_sleevelesstunic` | straight sleeveless knee tunic | camelid wool | `WP-RECTANGULAR-BLOUSE` | **material dependency**; work, state service, local, plain skins |
| `renaissance_andean_fine_tapestrytunic` | fine tapestry-woven tunic | camelid wool | `WP-RECTANGULAR-BLOUSE` | **material dependency**; court/state/office/ceremonial gate; motif is skin |
| `renaissance_andean_short_worktunic` | short coarse work tunic | camelid wool | `WP-RECTANGULAR-BLOUSE` | **material dependency**; labour, messenger, rural, military-support skins |
| `renaissance_andean_wrapped_fulllengthdress` | wrapped full-length dress | camelid wool | `WP-WRAP-DRESS` | **material dependency**; anaku-like anchor; household, court, ceremony skins |
| `renaissance_andean_pinned_wrapskirt` | pinned broad wrap skirt | camelid wool | `WP-WRAP-SKIRT` | **material dependency**; pin is presentation unless fastening state is supported |
| `renaissance_andean_paired_shouldermantle` | broad pinned shoulder mantle | camelid wool | `WP-SHOULDER` | **material dependency**; household, market, court, local motif skins |
| `renaissance_andean_ceremonial_broadmantle` | long ceremonial mantle | camelid wool | `WP-SHOULDER` | **material dependency**; state/court/ritual/diplomatic gate |
| `renaissance_andean_fringed_camelidshawl` | fringed camelid-wool shawl | camelid wool | `WP-SHOULDER` | **material dependency**; ordinary, elite, messenger, cold-weather skins |
| `renaissance_andean_featherwork_tunic` | feather-panelled ceremonial tunic | featherwork | `WP-RECTANGULAR-BLOUSE` | **material dependency**; court/state/ritual gate and trade-status note |
| `renaissance_andean_feathered_headband` | broad feathered headband | featherwork | `WP-HEAD-HAT` | **material dependency**; office/ritual/status gate |
| `renaissance_andean_woven_earflapcap` | woven ear-flap cap | camelid wool | `WP-HEAD-CAP` | **material dependency**; regional/date/status skins; chullo-like anchor used cautiously |
| `renaissance_andean_long_headcloth` | long woven headcloth | camelid wool | `WP-HEADWRAP` | **material dependency**; wrapped, draped, court, local skins |
| `renaissance_andean_braided_fibresandals` | pair of braided fibre sandals | canvas | `WP-FOOT-SANDAL` | exact plant fibre future dependency; work, messenger, military-support skins |
| `renaissance_andean_fringed_ceremonialapron` | fringed ceremonial waist apron | camelid wool | `WP-WRAP-SKIRT` | **material dependency**; court/ritual/performance/status gate |

## E. Shared Caribbean and North American contact catalogue — 13

### Caribbean contact shared forms — 6

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_caribbean_cotton_wrapskirt` | short cotton wrap skirt | cotton | `WP-WRAP-SKIRT` | Taíno/Caribbean-facing local admission; plain, woven-pattern, status skins |
| `renaissance_caribbean_cotton_loinapron` | panelled cotton loin apron | cotton | `WP-BREECHCLOTH` | work, household, status, contact skins; narrower local anchor required |
| `renaissance_caribbean_sleeveless_cottontunic` | loose sleeveless cotton tunic | cotton | `WP-RECTANGULAR-BLOUSE` | local continuity or contact-hybrid status must be recorded |
| `renaissance_caribbean_woven_shouldercape` | short woven shoulder cape | cotton | `WP-SHOULDER` | status, ceremony, travel, contact skins; not universal ordinary dress |
| `renaissance_caribbean_feathered_headband` | narrow feathered headband | featherwork | `WP-HEAD-HAT` | **material dependency**; status/ritual/dance gate |
| `renaissance_caribbean_woven_fibresandals` | pair of woven fibre sandals | canvas | `WP-FOOT-SANDAL` | exact fibre future dependency; local/contact admission only |

### North American Indigenous contact shared forms — 7

This remains a deliberately broad placeholder pending region-specific later catalogues. Every use requires a narrower inspiration and should prefer skins or future regional prototypes over treating the continent as one culture.

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_northamerican_hide_breechcloth` | long hide breechcloth | leather | `WP-BREECHCLOTH` | regional/contact gate; tanned hide source and panel cut in builder note |
| `renaissance_northamerican_paired_hideleggings` | pair of long hide leggings | leather | `WP-LEG-WRAPS` | separate-looking paired item; regional cut, fringe, paint, bead skins |
| `renaissance_northamerican_hide_shirt` | loose hide shirt | leather | `WP-SHIRT` | regional/date/contact gate; fringe and beadwork are skins unless structural |
| `renaissance_northamerican_hide_wrapdress` | full hide wrap dress | leather | `WP-WRAP-DRESS` | regional/date/contact gate; belted or shouldered skins |
| `renaissance_northamerican_fur_robe` | broad fur robe | fur | `WP-SHOULDER` | exact fur material; cold-weather, prestige, trade, household skins |
| `renaissance_northamerican_feathered_mantle` | layered feather mantle | featherwork | `WP-SHOULDER` | **material dependency**; ritual/status/diplomatic gate, not generic stock |
| `renaissance_northamerican_soft_moccasins` | pair of soft hide moccasins | leather | `WP-MOCCASIN` | regional seams, cuff, bead/paint skins; distinct from hard-soled shoes |

## F. Shared first-contact, colonial Atlantic, and global maritime catalogue — 10

These rows exist only where construction creates a distinct prototype. Most imported clothing should use an existing stable reference plus an imported/contact skin and manifest record.

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_maritime_canvas_watchcoat` | long hooded canvas watch coat | canvas | `WP-ROBE-OPEN` | shipboard watch, storm, dock, voyage gate; no waterproof mechanic claim |
| `renaissance_maritime_treated_canvas_cape` | short treated-canvas deck cape | canvas | `WP-CLOAK` | shipboard/dock gate; tar/oil treatment descriptive until material or state exists |
| `renaissance_maritime_knitted_waistcoat` | sleeveless knitted wool waistcoat | wool | `WP-VEST` | sailor, fisher, dockworker, cold-watch skins; not a later formal waistcoat |
| `renaissance_maritime_loose_decktrousers` | loose ankle-length deck trousers | canvas | `WP-TROUSERS` | Atlantic/Indian Ocean/Asian shipboard admission by local crew context |
| `renaissance_maritime_sleeveless_decksmock` | sleeveless canvas deck smock | canvas | `WP-SHIRT` | hot-weather shipboard, dock, boatyard, cargo-work skins |
| `renaissance_contact_mission_catechistrobe` | plain long mission-associated robe | linen | `WP-ROBE-CLOSED` | imposed/adopted/locally made status must be recorded; mission institution required |
| `renaissance_contact_hybrid_lacedtunic` | side-slit cotton tunic with front lacing | cotton | `WP-HYBRID-TUNIC` | genuine contact-zone hybrid only; local cut plus introduced lacing materially visible |
| `renaissance_contact_hybrid_buttoned_wrapjacket` | wrap jacket with buttoned front | cotton | `WP-JACKET` | contact-zone craft/fastening gate; button functionality remains descriptive unless component supports it |
| `renaissance_contact_tradecloth_panelskirt` | pieced trade-cloth panel skirt | cotton | `WP-SKIRT` | imported cloth/local construction recorded explicitly; panel piecing integral |
| `renaissance_contact_broadbrim_sunhood` | broad-brimmed cloth sun hood | canvas | `WP-HOOD` | colonial/contact labour, maritime, mission, travel gate; never universal local continuity |

## Headwear and footwear expansion — 16

These rows are additional construction-level options. They do not alter the existing outfit minimums or first-pass shared-placement count; a builder should admit them only where status, work, route, climate, institution, or local dress practice justifies the distinct form.

| Stable reference | Public form | Material | Wear profile | Variation and admission notes |
| --- | --- | --- | --- | --- |
| `renaissance_sahel_quilted_ridingcap` | quilted cotton riding cap | cotton | `WP-HEAD-CAP` | Close quilted cap for Sahelian cavalry, courier, caravan, herder, and long-road contexts; every admission requires a narrower local anchor. |
| `renaissance_redsea_quilted_white_ridinghood` | quilted white cotton riding hood | cotton | `WP-HOOD` | Close highland and Red Sea hood for mounted travel, courier work, cool uplands, and locally admitted military-road use; distinct from the unquilted ceremonial head hood. |
| `renaissance_indianocean_port_turban` | compact coastal port turban | cotton | `WP-STRUCTURED-HEADWRAP` | Indian Ocean merchant, sailor, broker, scholar, and mosque-associated admission; not universal coastal dress. |
| `renaissance_mesoamerican_tied_cotton_headcloth` | tied woven cotton headcloth | cotton | `WP-HEAD-CAP` | Market, agricultural, porter, warrior-status, and household contexts; exact knot and community require local admission. |
| `renaissance_mesoamerican_featheredge_crown` | feather-edged woven crown | feather | `WP-FEATHER-CROWN` | Court, military-status, ritual, diplomatic, or performance gate only; species, colour, rank, and sacred meaning are local. |
| `renaissance_andean_fourcorner_wovencap` | four-cornered camelid-wool cap | camelid wool | `WP-HEAD-CAP` | Locally woven cap with a visibly four-cornered crown for Andean regional, office, festival, and status admissions; tassels and motifs remain skins. |
| `renaissance_globalmaritime_knotted_headscarf` | knotted cotton deck headscarf | cotton | `WP-HEADWRAP` | Cross-cultural shipboard, fishing, dock, boat, and hot-weather port form; knot, stripe, and local terminology remain skins. |
| `renaissance_northamerican_soft_hide_winterhood` | soft hide winter hood | animal skin | `WP-HOOD` | Cold-weather travel, hunting, camp, and local ceremonial skins only where the specific community supports a separate hood form. |
| `renaissance_africancourt_raffia_brimhat` | structured raffia brimmed hat | raffia cloth | `WP-HEAD-HAT` | Court, office, market, diplomatic, festival, or ritual gate under a narrower African culture manifest; trim is skin presentation. |
| `renaissance_sahel_soft_ridingboots` | pair of soft leather riding boots | leather | `WP-FOOT-BOOT` | Cavalry, courier, caravan, court-retainer, and long-road contexts; distinct softer shaft from European frontier boots. |
| `renaissance_redsea_rawhide_sandals` | pair of rawhide highland sandals | rawhide | `WP-FOOT-SANDAL` | Highland, pastoral, military-road, rural, and pilgrimage use; local cut and hide source required. |
| `renaissance_indianocean_stitched_coastal_slippers` | pair of stitched coastal slippers | leather | `WP-FOOT-SHOE` | Port, merchant, scholar, household, and ceremonial skins; distinct stitched closed upper from toe-loop sandals. |
| `renaissance_mesoamerican_heelstrap_sandals` | pair of heel-strapped woven sandals | leather | `WP-FOOT-SANDAL` | Road, market, warrior-status, court, agricultural, and porter contexts where a heel strap materially changes the form. |
| `renaissance_andean_rawhide_roadsandals` | pair of rawhide road sandals | rawhide | `WP-FOOT-SANDAL` | Highland road, agricultural, porter, courier, and military-support contexts; altitude and local community required. |
| `renaissance_caribbean_gathered_rawhide_shoes` | pair of side-stitched rawhide shoes | rawhide | `WP-FOOT-SHOE` | Soft closed footwear for narrower Caribbean and Atlantic-contact admissions where a side-stitched hide upper is locally supported; not universal island dress. |
| `renaissance_northamerican_furlined_moccasins` | pair of fur-lined soft moccasins | deer leather | `WP-MOCCASIN` | Cold-weather local form distinct from the existing unlined moccasin; ecology, community, season, and contact history required. |

## Shared admissions — 15 placements, no new prototypes

| Existing stable reference | Admitted grouping/context | Reason no clone is needed |
| --- | --- | --- |
| `renaissance_shared_clothing_wrapped_loincloth` | African, South/Indian Ocean, Caribbean, Mesoamerican, North American work contexts | ordinary wrapped form; panelled breechcloths remain distinct |
| `renaissance_shared_clothing_breast_wrap` | African, Caribbean, South/Indian Ocean and contact outfits | same folded support layer where locally admitted |
| `renaissance_shared_clothing_side_slit_tunic` | Sahel, Indian Ocean, African court, contact-zone ordinary dress | same sewn tunic silhouette; local weave/motif are skins |
| `renaissance_shared_clothing_straight_tunic` | Red Sea, African court, Caribbean/contact, North American trade-cloth use | plain continuity form where no rectangular-blouse or hide construction applies |
| `renaissance_shared_clothing_full_trousers` | Sahel, Red Sea cavalry/contact, Indian Ocean port use | same gathered trouser form |
| `renaissance_shared_clothing_wrap_skirt` | African, Caribbean, Mesoamerican, Andean ordinary forms | use only where overlap construction matches; tube/pinned/dress forms remain distinct |
| `renaissance_shared_clothing_gathered_skirt` | African court, contact, mission and port outfits | same sewn gathered skirt where local panel/tube form is absent |
| `renaissance_shared_clothing_rectangular_mantle` | Sahel, Red Sea, Mesoamerican, Andean, North American | ordinary rectangular drape; tied/pinned or feather/hide forms remain distinct |
| `renaissance_shared_clothing_shoulder_shawl` | Red Sea, Indian Ocean, Andean, Sahelian and contact elite use | same rectangular shoulder layer; fibre and motif are skins/material variants |
| `renaissance_shared_clothing_leather_sandals` | Sahel, Red Sea, African court, Caribbean/contact | ordinary strapped sandal; toe-loop, raised wood, and woven forms remain distinct |
| `renaissance_shared_clothing_textile_sandals` | African, Mesoamerican, Andean, Caribbean fibre footwear | same woven sandal where fibre structure does not change silhouette |
| `renaissance_shared_clothing_cloth_headwrap` | African court, Sahel, Red Sea, Indian Ocean, Caribbean/contact | unstructured winding; crown-shaped and scholar turbans remain distinct |
| `renaissance_shared_clothing_straw_brimmed_hat` | African/Indian Ocean field, Caribbean, colonial/contact labour | same brimmed woven sun hat; conical palm forms remain distinct |
| `renaissance_shared_clothing_soft_brimless_cap` | African court, Sahel, Indian Ocean port, contact crews | same soft cap; embroidery and weave are skins |
| `renaissance_shared_clothing_rain_cape` | Andean highland, Indian Ocean, Caribbean/contact travel | textile rain cape where credible; barkcloth, straw, hide and treated-canvas forms remain distinct |

## Contact and sensitivity metadata required on every row

Every implementation manifest entry in this volume must record:

| Field | Required content |
| --- | --- |
| narrower inspiration | named local society, polity, region, port, or contact zone; “African” or “Indigenous” alone is insufficient |
| date window | supported years or century segment within 1400-1600 |
| context | ordinary, labour, court, office, ritual, military-status, mission, maritime, diplomatic, mourning, initiation, performance, or trade |
| production status | local continuity, locally made imported fibre, imported finished garment, tribute, diplomatic gift, imposed dress, mission issue, export production, or hybrid |
| prevalence | ubiquitous/common/limited/rare/unique ceremonial stock as appropriate |
| sensitivity note | sacred/status restriction, colonial coercion, protected motif, animal source, or uncertain broad-placeholder status |
| skin boundary | which motif, colour, bead, feather, shell, hide pattern, or imported textile belongs in skins rather than prototypes |

## Outfit minimums

The authority's inferred manifest table implements eighteen stock ordinary/status outfits from the usable groupings in this volume. It intentionally omits a North American stock outfit because the broad placeholder below requires a narrower regional design before use.

| Shared grouping | Ordinary outfit | Restricted/status outfit |
| --- | --- | --- |
| African court / Atlantic | wrapper or skirt, tunic/blouse as locally admitted, headcloth/cap, sandals or bare-foot admission | court robe/coat, structured headcloth, bead/feather/hide regalia, raised sandals |
| Sahelian Islamic | tunic, full trousers or wrapper, robe, cap/turban, sandals | scholar/court robe, structured turban, riding coat, status footwear |
| Red Sea / Indian Ocean | wrap/tunic/robe, headcloth/cap, sandals | court/church/mosque-associated outer layer, embroidered cape, merchant coat |
| Mesoamerican | loin garment or skirt, blouse/tunic, tied cloak, sandals | featherwork, hide mantle, ceremonial tunic, office/ritual headwear |
| Andean | tunic or wrapped dress/skirt, shoulder mantle, headcloth/cap, sandals | tapestry/feather tunic, ceremonial mantle/apron, status headwear |
| Caribbean / North American contact | local continuity outfit first; imported pieces only by explicit contact date | diplomatic, ritual, trade, mission, imposed, or hybrid outfit with status recorded |
| global maritime | crew-local ordinary outfit plus shipboard weather/work layer | officer, pilot, merchant, mission, or colonial-office skins only when historically admitted |

## Volume acceptance

- Exactly 116 unique stable references are defined here.
- The 15 shared admissions create no new prototype.
- Every row has a narrower inspiration and date/context gate before implementation.
- Ritual, royal, office, sacred, military-status, and diplomatic regalia is not ordinary market stock.
- Featherwork, beadwork, barkcloth, raffia, camelid wool, fur, hide, shell, and imported textile dependencies resolve exactly or fail closed.
- Contact rows distinguish local continuity, import, coercion, mission issue, export, and hybrid construction.
- North American contact remains an explicit placeholder and cannot be used without a narrower regional design pass.
