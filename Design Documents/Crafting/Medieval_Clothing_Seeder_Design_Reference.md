# FutureMUD Medieval Clothing Seeder Design Reference

This document consolidates the medieval clothing guidance and target catalogue for the FutureMUD Item Seeder. It covers selected European, Mediterranean, Near Eastern, North African, Indian, Central Asian, and East Asian clothing families during the period roughly 500AD to 1300AD, presenting shared rules, historical assumptions, outfit manifests, and item-catalogue targets in one stable reference.

## Executive summary

- Total target unique wearable item prototypes in this reference: **259**.
- Total outfit manifests: **100**.
- Each inspiration family has four builder-facing outfit manifests: common male, elite male, common female, and elite female.
- All target items are finished goods, skinnable, player-visible, and ordinary portable inventory items unless a later seeder implementation explicitly marks a narrow exception.
- Public item fields should use culture-neutral, in-world descriptions. Builder-facing notes and outfit labels may still use historical inspiration labels for organization and source grounding.
- The item set is intended as a historically grounded base layer: skins may create local, fantasy, status, textile, dye, motif, household, seasonal, and professional variants without duplicating behaviour-heavy item prototypes.
- The catalogue includes **34** cold-weather overlay items and **68** urban/professional overlay items; both are optional additions or substitutions rather than mandatory default outfit pieces.

## Scope and era model

- Chronological band: 500AD to 1300AD.
- Geographic coverage in this consolidated document: Britain and Ireland; Scandinavia and the North Sea; western, central, and southern Europe; Iberia; Byzantium; the Levant; Egypt and North Africa; the Eurasian steppe; Rus/Novgorod; northern and southern India; China; Korea; and Japan.
- Historical inspiration families: Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Norman, Anglo-Norman, High English / British, Irish / Gaelic, Scottish / Gaelic-Lowland, Carolingian / Frankish, Capetian French, Holy Roman Empire / German, Christian Iberian, Andalusian, Byzantine, Abbasid, Fatimid, Seljuk / Ayyubid-Mamluk, Magyar, Rus / Novgorod, Steppe Turkic / Mongol, North Indian / Rajput, South Indian / Chola, Song China, Goryeo Korea, Heian / Kamakura Japan. These labels are builder-facing organizational buckets, not required public item wording.
- Resolution: standard garment prototypes rather than exhaustive local subtypes. The defaults should be credible on their own, while later skins can handle exact regional trim, textile patterns, motifs, dyes, household marks, rank marks, religious variants, and world-specific names.
- Coverage deliberately excludes cultures outside this document's selected 500AD-1300AD seeder slice. South-east Asian, Tibetan, Ethiopian, West African, Mesoamerican, Andean, late medieval Ottoman, later Mamluk, late medieval European fitted-fashion, Renaissance, early modern, modern, and American traditions are left for separate references.
- Culture labels are used to ensure design coverage; they are not public naming requirements and should not be mechanically inserted into `noun`, `sdesc`, or `fdesc`.

### Culture coverage table

| Inspiration family | Reference anchor | Coverage boundary | Garment-design focus |
|---|---:|---|---|
| Early Anglo-Saxon | c. 800AD | lowland England before sustained Anglo-Danish synthesis | short wool tunics, trousers, leg wraps, straight gowns, brooched cloaks, veils, coifs, leather footwear |
| Anglo-Danish | c. 950AD | late Anglo-Saxon England under strong Scandinavian settlement influence | Anglo-Saxon base layers with Norse-style hangeroks, særks, leg wraps, brooched cloaks, tablet bands, and mixed insular/North Sea textile habits |
| Norse / Viking Age | c. 950AD | Scandinavia and diaspora settlements before the close of the Viking Age | wool tunics, trousers, leg wraps, særks, hangeroks, brooch-fastened cloaks, fur, flax linen, tablet-woven trim, imported silk for elites |
| Norman | c. 1066AD | ducal Normandy and the conquest-generation Norman sphere | bliaut-adjacent long tunics, split riding tunics, chausses, mantles, coifs, veils, leather shoes, stronger elite display through borders and fine cloth |
| Anglo-Norman | c. 1150AD | post-Conquest England and Norman-influenced elite fashion in Britain | chemises, braies, chausses, cottes, kirtles, bliauts, half-circle mantles, veils, wimples, girdles, and soft shoes |
| High English / British | c. 1250AD | high medieval England and Welsh March/British court-facing styles | cotte/kirtle layers, surcoats, gowns, hose, chausses, wimples, coifs, hoods, mantles, and sharper social separation by cloth quality |
| Irish / Gaelic | c. 1100AD | Gaelic Ireland before strong late-medieval English tailoring influence | léine-style shirts, brat cloaks, trews, mantles, simple belts, leather shoes, and finer bordered or pleated cloth for elites |
| Scottish / Gaelic-Lowland | c. 1250AD | medieval Scotland across Gaelic Highland and Lowland court-facing clothing habits | Gaelic léine/brat/trews layers beside Lowland kirtles, surcoats, mantles, hoods, veils, and leather shoes |
| Carolingian / Frankish | c. 800AD | Frankish court and common clothing under Carolingian influence | knee tunics, mantles, cloaks, leg wraps, hose, caps, belts, leather footwear, and elite silk or tablet-woven borders |
| Capetian French | c. 1200AD | northern and central French high medieval clothing | cotte, chainse, bliaut, kirtle, chausses, girdles, veils, mantles, surcoats, and fine wool or silk display garments |
| Holy Roman Empire / German | c. 1200AD | German-speaking imperial regions and neighbouring central European towns | tunic/cotte layers, gowns, gugel-style hoods, mantles, surcoats, chausses, coifs, caps, and strong wool-broadcloth use |
| Christian Iberian | c. 1200AD | Leonese, Castilian, Aragonese, Navarrese, and neighbouring Christian Iberian clothing | camisa, saya, pellote, mantle, toca, hose, leather shoes, belts, and a blend of wool, linen, cotton, and silk-facing elite cloth |
| Andalusian | c. 1100AD | al-Andalus and western Islamic Iberia | qamis/camisa layers, sirwal, jubba, burnous, izar, turbans, veils, slippers, fine cotton, linen, and silk, with tiraz or patterned bands for status |
| Byzantine | c. 1000AD | middle Byzantine Constantinopolitan and provincial court-facing clothing | kamision, dalmatic, skaramangion, sagion, tablion cloak, loros, maforion, veils, caps, silk, wool, and heavily bordered court garments |
| Abbasid | c. 850AD | Baghdad-centred early medieval Islamic court and urban clothing | qamis, sirwal, izar, jubba, turbans, slippers, robes of honour, tiraz bands, and layered cotton, linen, wool, and silk |
| Fatimid | c. 1050AD | Fatimid Egypt and North African elite/public textile culture | qamis, sirwal, robes, head veils, turbans, tiraz textiles, linen, cotton, silk, light outer mantles, and urban luxury cloth |
| Seljuk / Ayyubid-Mamluk | c. 1200AD | Anatolia, Syria, Egypt, and the late crusading-era Islamic eastern Mediterranean | qamis, sirwal, caftans, jubbas, boots, turbans, felt caps, riding coats, robes of honour, and warm over-robes |
| Magyar | c. 950AD | Carpathian Basin Magyar clothing with steppe and early central European influences | riding caftans, trousers, boots, belts, caps, cloaks, and richer silk, fur, metal-fitted belts, and decorated coats for elites |
| Rus / Novgorod | c. 1100AD | Kievan Rus and Novgorod-facing northern Slavic/Norse-Byzantine clothing intersections | rubakha shirts, porty trousers, onuchi leg wraps, poneva skirts, sleeveless overgowns, shuba coats, fur hats, boots, and embroidered borders |
| Steppe Turkic / Mongol | c. 1200AD | Inner Eurasian mounted steppe clothing before and during early imperial Mongol expansion | front-opening caftans, deel-like robes, trousers, felt boots, high boots, belts, felt caps, fur-lined coats, and silk-brocade elite garments |
| North Indian / Rajput | c. 1100AD | north and north-western Indian clothing before heavy late Sultanate/Mughal tailoring dominance | cotton dhoti or lower cloths, uttariya shoulder cloths, turbans, short bodices, sari-like drapes, silk borders, and light layered court textiles |
| South Indian / Chola | c. 1050AD | Chola and neighbouring south Indian clothing in a warm-climate temple and court textile environment | veshti-style lower drapes, angavastram shoulder cloths, sari-like drapes, breastbands or short bodices, cotton, silk, and rich borders |
| Song China | c. 1100AD | Northern and Southern Song clothing forms useful for a broad East Asian seeder baseline | cross-collar robes, round-collar robes, ru jackets, changqun skirts, trousers, beizi over-robes, cloth shoes, headscarves, and official caps |
| Goryeo Korea | c. 1150AD | Goryeo-period Korean clothing as an East Asian court and commoner family | cross-collar jackets, baji trousers, chima skirts, po over-robes, headcloths, caps, cloth shoes, and fine silk over-robes for elites |
| Heian / Kamakura Japan | c. 1200AD | late Heian to early Kamakura Japanese clothing | kosode, hakama, suikan, hitatare, uchigi, kariginu, eboshi, hemp sandals, layered silk courtwear, and simpler tight-sleeved robes for common dress |

## Seeder and project grounding rules

- Use the project-standard `CreateItem(...)` seeder call shape for item implementation.
- `uniqueReference` values use lowercase snake case ASCII and should remain stable once accepted.
- `noun` is compact and singular, usually the garment type or closest general noun.
- `sdesc` is player-facing, concise, and normally begins with `a`, `an`, or `a pair of`. It should not end with a full stop.
- `ldesc` is `null` for these ordinary portable wearables, allowing the normal room display to apply.
- `fdesc` is player-facing in-world prose: shape, visible material, cut, drape, folds, seams, ties, borders, closures, weight, warmth, and how the garment sits on the wearer.
- `material` must be an exact seeded solid material. Liquids and gases are not substitutes for solid primary materials.
- `tags` must be exact seeded hierarchical tag paths.
- `components` must be exact seeded component prototype names.
- `inherentCost` is denominated in farthings. Use whole-farthing values unless a fractional farthing is deliberately intended.
- No target medieval clothing item should use a morph target, morph emote, morph timer, long description, destroyed-item reference, hidden-player flag, or non-skinnable flag.

## Wearable implementation rules

- Every clothing item should have one wearable coverage component, such as `Wear_Tunic`, `Wear_Long-Sleeved_Tunic`, `Wear_Robe`, `Wear_Mantle`, `Wear_Cloak_(Open)`, `Wear_Cloak_(Closed)`, `Wear_Trousers`, `Wear_Chausses`, `Wear_Sandals`, `Wear_Shoes`, `Wear_Boots`, `Wear_Hat`, `Wear_Turban`, `Wear_Coif`, or `Wear_Veil` as appropriate.
- Ordinary portable wearables include `Holdable` so they can be picked up, moved, carried, stored, and otherwise handled as inventory objects.
- Clothing uses a destroyable component. Most textile/leather clothing uses `Destroyable_Clothing`; glass jewellery uses an appropriate glassware destroyable component.
- Wearables include a clothing insulation component and an armour component. These are not meant to make ordinary clothes battlefield armour; `Armour_LightClothing` and `Armour_HeavyClothing` represent ordinary protective clothing behaviour.
- Belts that should support attached items can include a belt component, such as `Belt_2`, in addition to the wearable waist component. Cloth sashes and girdles do not automatically become functional belts unless intentionally implemented that way.
- Every cloth item that would reasonably be dyed should use a variable colour component from `Variable_FineColour`, `Variable_BasicColour`, `Variable_2BasicColour`, or `Variable_2FineColour`.
- Component-dependent claims must be avoided unless the component exists. Descriptions may mention visible straps, hems, seams, folds, pins, brooches, bands, borders, closures, or ties, but must not promise hidden storage, locks, identity concealment, transformations, armour value, or container behaviour without components.

## Player-facing description rules

- Do not mention skins, base prototypes, standard implementations, seeder mechanics, builders, later customization, or archaeological uncertainty in `noun`, `sdesc`, `ldesc`, or `fdesc`.
- Full descriptions must read as objects in the world. Good wording explains how the garment is cut, wrapped, belted, draped, folded, tied, layered, fastened, trimmed, or worn.
- Public item text avoids real-world culture labels such as Anglo-Saxon, Norse, Norman, English, Irish, Scottish, Frankish, French, German, Iberian, Andalusian, Byzantine, Abbasid, Fatimid, Seljuk, Magyar, Rus, Turkic, Mongol, Indian, Chinese, Korean, or Japanese unless both absolutely unavoidable and contemporarily supported. These labels can be used in builder-facing manifests and source notes.
- Garment names are acceptable when they identify a form rather than merely naming a people. Examples used here include `braies`, `chausses`, `chainse`, `cotte`, `kirtle`, `bliaut`, `surcoat`, `saya`, `pellote`, `manto`, `toca`, `qamis`, `sirwal`, `jubba`, `izar`, `burnous`, `tiraz`, `kamision`, `dalmatic`, `skaramangion`, `sagion`, `loros`, `maforion`, `caftan`, `deel`, `rubakha`, `onuchi`, `poneva`, `shuba`, `dhoti`, `uttariya`, `sari`, `veshti`, `angavastram`, `cross-collar robe`, `beizi`, `baji`, `chima`, `po`, `kosode`, `hakama`, `suikan`, `hitatare`, `uchigi`, `kariginu`, and `eboshi`.
- Where no useful garment name exists, public naming should use visible descriptors: short, long, straight, fitted, full, folded, bordered, checked, pleated, wrapped, woolly, belted, sleeveless, front-opening, split-skirt, fur-lined, or similar.
- Avoid male-default naming. Do not create unmarked `tunic` plus marked `women's tunic` pairs when the meaningful distinction is length, fullness, sleeve form, drape, or layering. Gender can appear in builder-facing outfit manifests, but public text should prefer form-based distinctions.

## Skin strategy

- All target garments are skinnable because they are finished wearable goods.
- A skin can override presentation fields and quality without changing the underlying item behaviour.
- Skins should carry high-variance presentation: local fantasy-culture names, exact trim patterns, tablet-woven bands, embroidery, tiraz text, rank borders, official insignia, household marks, clan marks, religious or temple motifs, silk patterning, brooch placement, dye intensity, material nuance, and fantasy-world motifs.
- Default unskinned items still need to be credible standalone garments. Skinability must not be used as an excuse for a bland, incomplete, or out-of-world base description.
- Player-facing base descriptions should never tell the player that a skin may later change the appearance.

## Colour-variable policy

- `Variable_BasicColour`: ordinary clean garments with common colours and simple light/dark variants. This is the default for simple wool, linen, cotton, hemp, and felt clothing.
- `Variable_FineColour`: fine, elite, formal, imported, ceremonial, or otherwise richer garments where more varied and evocative colour words are appropriate.
- `Variable_2BasicColour`: ordinary two-colour woven, checked, striped, or banded textiles.
- `Variable_2FineColour`: richer two-colour woven, checked, striped, or banded textiles, especially where an elite or formal garment benefits from a more elaborate colour vocabulary.
- `Variable_DrabColour` and `Variable_2DrabColour`: only for deliberately dingy, grimy, tattered, filthy, degraded, or culturally grungy items. These are not neutral plain-clothing lists and are not used for normal clean garments.
- Some leather, metal, hide, fur, felt, hemp, glass, and jewellery items omit colour variables when a natural fixed default reads better than allowing inappropriate colours.

## Materials, sizes, quality, and cost assumptions

- Dominant primary materials are `linen`, `wool`, `leather`, `fur`, `felt`, `silk`, `cotton`, `hemp`, and `cashmere`, depending on the garment.
- Primary material represents the dominant physical substance of the item, not every decorative or fastening detail. A belt can mention iron fittings while still using `leather` as its primary material; a wool cloak can mention fur trim while still using `wool` as its primary material.
- Sizes are mostly `Normal` for bodywear and outer garments, `Small` for underwear, footwear, headwear, belts, veils, collars, and small accessories.
- Quality is `Standard` for common pieces and `Good` for elite, fine, formal, imported, silk, fur-lined, or more labour-intensive pieces.
- Costs use farthing-denominated defaults and are intended as relative seeder baselines, not universal market prices.
- Silk, cashmere, fur-lined, tiraz, tablion, official, court, and heavily bordered garments should be priced as elite goods even when their physical weight is lower than heavy wool garments.

## Historical and design assumptions by inspiration family

### Early Anglo-Saxon

Reference anchor: c. 800AD. Built around practical wool-and-linen layers: knee-length tunics, narrow trousers, leg wraps, straight gowns, cloaks, coifs, veils, and leather shoes. The c. 800AD anchor gives better manuscript and vocabulary support than an earlier c. 700AD anchor while still keeping the family distinct from Anglo-Danish and Norman clothing. Elite pieces add tablet-woven bands, cleaner linen, finer dyes, and brooch-fastened mantles without becoming courtly high-medieval tailoring.

### Anglo-Danish

Reference anchor: c. 950AD. Built around late Anglo-Saxon clothing with North Sea Scandinavian influence: tunics, trousers, leg wraps, særk-like underlayers, hangerok-style overdresses, brooched mantles, checked wool, and leather boots. The c. 950AD anchor is preferable to c. 900AD because it better captures the mature Anglo-Danish synthesis while remaining before the Norman wardrobe shift.

### Norse / Viking Age

Reference anchor: c. 950AD. Built around Viking Age Scandinavian layers: wool tunics, trousers, leg wraps, linen særks, hangeroks, cloaks fastened at the shoulder or chest, felt caps, boots, and fur or imported silk for elite outfits. The c. 950AD anchor is deliberately earlier than c. 1100AD because 1100AD falls after the main Viking Age clothing horizon and overlaps more strongly with Christian high-medieval Scandinavian dress.

### Norman

Reference anchor: c. 1066AD. Built around conquest-generation Norman forms: linen braies, chausses, split-skirt tunics, longer gowns, half-circle mantles, coifs, veils, leather shoes, and richer bordered cloth for elites. The family is useful as a bridge between early medieval tunic-and-cloak clothing and Anglo-Norman/Capetian high-medieval layers.

### Anglo-Norman

Reference anchor: c. 1150AD. Built around post-Conquest high-medieval layering in Britain: braies, chausses, chainse, cotte, kirtle, bliaut, surcoat, wimple, veil, girdle, and half-circle mantle. This family should feel more courtly and textile-rich than the Norman family while still avoiding later fourteenth-century fitted tailoring.

### High English / British

Reference anchor: c. 1250AD. Built around c. 1250AD English and British high-medieval clothing: cotte, kirtle, surcoat, hose, chausses, hoods, wimples, coifs, mantles, and more sharply status-coded cloth. This family absorbs generic high-medieval British urban, court, and common outfits; highly local Welsh, Cornish, and regional skins can be layered later without creating duplicate base prototypes.

### Irish / Gaelic

Reference anchor: c. 1100AD. Built around Gaelic clothing forms rather than English court dress: long léine-style shirts, brat cloaks, trews, simple belts, veils, leather shoes, and broad wool mantles. Elite outfits use better linen, borders, pleating, and richer mantles while retaining the loose, draped, and cloak-heavy silhouette.

### Scottish / Gaelic-Lowland

Reference anchor: c. 1250AD. Built around a deliberate split between Gaelic and Lowland influences: léine, brat, trews, and riding-friendly layers for Gaelic-facing outfits; kirtle, cotte, chausses, surcoat, and wimple for Lowland or court-facing outfits. The c. 1250AD anchor is better than a generic c. 1200AD because it makes the Lowland high-medieval overlap clearer.

### Carolingian / Frankish

Reference anchor: c. 800AD. Built around early medieval Frankish layers: knee-length tunics, undertunics, trousers or hose, leg wraps, shoulder-fastened cloaks, caps, belts, and leather shoes. Elite pieces introduce borders, finer wool, imported silk trims, and cloak display while keeping the silhouette earlier and less tailored than Capetian clothing.

### Capetian French

Reference anchor: c. 1200AD. Built around c. 1200AD French high-medieval forms: chainse, cotte, kirtle, bliaut, chausses, surcoat, girdle, veil, and mantle. The family should carry the richest western European courtwear in this document without pushing into later fitted cotehardie or houppelande forms.

### Holy Roman Empire / German

Reference anchor: c. 1200AD. Built around central European high-medieval tunic, cotte, gown, hood, surcoat, and mantle combinations, with strong wool, broadcloth, belts, coifs, and leather footwear. The gugel-style hood and heavier mantle options help distinguish the family from Capetian and Anglo-Norman outfits.

### Christian Iberian

Reference anchor: c. 1200AD. Built around Christian Iberian layers: camisa, saya, pellote, manto, toca, hose, belts, leather shoes, and mixed wool, linen, cotton, and silk. This family should visibly sit between western European high-medieval dress and Andalusian textile habits without making public item text use ethnic or dynastic labels.

### Andalusian

Reference anchor: c. 1100AD. Built around western Islamic Iberian clothing: qamis or camisa-like shirts, sirwal, jubba, burnous, izar, turbans, veils, soft slippers, and patterned or tiraz-like elite textile display. It emphasizes light cotton and linen layers, warm-climate drape, and fine silk or wool outer garments for status.

### Byzantine

Reference anchor: c. 1000AD. Built around middle Byzantine layered and court-facing garments: kamision, dalmatic, skaramangion, sagion, tablion cloak, loros, maforion, soft leather shoes, and silk-heavy elite dress. Common outfits are still tunic-and-mantle practical; elite outfits are more formal, bordered, and ceremonial than western European equivalents.

### Abbasid

Reference anchor: c. 850AD. Built around early medieval Islamic urban and court layering: qamis, sirwal, izar, jubba, turban, slippers, and tiraz robe. The c. 850AD anchor keeps this family close to Abbasid Baghdad and early caliphal textile display, with cotton and linen common layers and silk reserved for wealthier outfits.

### Fatimid

Reference anchor: c. 1050AD. Built around Fatimid Egypt and North African textile culture: qamis, sirwal, veils, turbans, jubbas, burnous-like outerwear, and tiraz or luxury robes. The family favours light linen and cotton, but elite versions should use silk, gold-coloured language, fine veils, and robe-of-honour cues where components allow.

### Seljuk / Ayyubid-Mamluk

Reference anchor: c. 1200AD. Built around eastern Islamic and Anatolian/Levantine riding and urban layers: qamis, sirwal, caftan, jubba, boots, turbans, pointed felt caps, and robes of honour. The expanded label deliberately covers late twelfth- and thirteenth-century Ayyubid and early Mamluk-adjacent clothing forms without multiplying near-identical qamis/sirwal catalogues.

### Magyar

Reference anchor: c. 950AD. Built around mounted steppe-to-central-European clothing: belted riding coats, trousers, boots, caps, cloaks, and decorated belts. The c. 950AD anchor is preferable because it captures the conquest-era Magyar silhouette before the wardrobe becomes harder to distinguish from neighbouring Christian central Europe.

### Rus / Novgorod

Reference anchor: c. 1100AD. Built around northern Slavic, Norse, and Byzantine-facing layers: rubakha shirts, porty trousers, onuchi leg wraps, poneva skirts, sleeveless overgowns, shuba coats, fur hats, boots, and embroidered borders. Common outfits are linen and wool heavy; elite outfits add fur, silk-trimmed display, and cleaner embroidery cues.

### Steppe Turkic / Mongol

Reference anchor: c. 1200AD. Built around mounted Inner Eurasian clothing: front-opening caftans, deel-like robes, trousers, felt boots, high leather boots, belts, felt caps, and fur-lined coats. The family broadens the old Steppe Turkic bucket to include early Mongol-period silhouettes before 1300, because otherwise the document under-covers the thirteenth-century steppe.

### North Indian / Rajput

Reference anchor: c. 1100AD. Built around warm-climate north Indian draped and semi-stitched clothing: cotton dhoti or lower cloth, uttariya shoulder cloth, turban, short bodice, sari-like drapes, leather or simple sandals, and silk-bordered elite versions. The family avoids heavy Mughal-era tailoring and keeps the c. 1100AD baseline earlier and more draped.

### South Indian / Chola

Reference anchor: c. 1050AD. Built around Chola-period south Indian drapes: veshti, angavastram, sari-like wrap, breastband or short bodice, light sandals, and rich textile borders. The c. 1050AD anchor is better than a generic c. 1200AD Indian anchor because it creates a clear southern court and temple-textile family inside the 500-1300AD band.

### Song China

Reference anchor: c. 1100AD. Built around Song clothing forms: cross-collar robes, round-collar robes, ru jackets, long pleated skirts, trousers, beizi over-robes, cloth shoes, headscarves, and official caps. Common outfits should remain clean and restrained; elite outfits should use fine silk, caps, and over-robes rather than excessive jewellery or armour-like components.

### Goryeo Korea

Reference anchor: c. 1150AD. Built around Goryeo-period Korean layers: cross-collar jackets, baji trousers, chima skirts, po over-robes, headcloths, felt caps, cloth shoes, and silk over-robes. The family fills a major East Asian gap between Song China and Heian/Kamakura Japan without requiring public item text to name Korea.

### Heian / Kamakura Japan

Reference anchor: c. 1200AD. Built around late Heian and early Kamakura forms: kosode, hakama, suikan, hitatare, uchigi, kariginu, eboshi, hemp sandals, and headcloths. The c. 1200AD anchor is preferable to an earlier purely Heian anchor because it permits both aristocratic layered clothing and more practical warrior/governmental garments inside one seeder family.

## Outfit manifests

The following outfit manifests are builder-facing. Their labels use historical inspiration buckets for organization, while the referenced public items remain culture-neutral.

### Early Anglo-Saxon common male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_wool_tunic` - a $colour wool tunic
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_wool_leg_wraps` - a pair of $colour wool leg wraps
- `medieval_rectangular_wool_cloak` - a $colour rectangular wool cloak

### Early Anglo-Saxon elite male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt
- `medieval_ankle_leather_boots` - a pair of ankle leather boots
- `medieval_tablet_banded_wool_tunic` - a $colour tablet-banded tunic
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_fur_lined_cloak` - a $colour fur-lined cloak
- `medieval_linen_coif` - a $colour linen coif

### Early Anglo-Saxon common female
- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_pinned_wool_gown` - a pinned $colour wool gown
- `medieval_linen_veil` - a $colour linen veil
- `medieval_rectangular_wool_cloak` - a $colour rectangular wool cloak

### Early Anglo-Saxon elite female
- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_bordered_wool_gown` - a fine $colour bordered gown
- `medieval_fine_linen_veil` - a fine $colour linen veil
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle

### Anglo-Danish common male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_ankle_leather_boots` - a pair of ankle leather boots
- `medieval_tablet_banded_wool_tunic` - a $colour tablet-banded tunic
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_wool_leg_wraps` - a pair of $colour wool leg wraps
- `medieval_brooched_wool_mantle` - a $colour brooched wool mantle

### Anglo-Danish elite male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt
- `medieval_high_leather_boots` - a pair of high leather boots
- `medieval_tablet_banded_wool_tunic` - a $colour tablet-banded tunic
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_fine_checked_wool_mantle` - a fine $colour1 and $colour2 checked mantle
- `medieval_felt_cap` - a $colour felt cap

### Anglo-Danish common female
- `medieval_linen_saerk` - a $colour linen særk
- `medieval_simple_woven_sash` - a $colour woven sash
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_wool_hangerok` - a $colour wool hangerok
- `medieval_checked_wool_mantle` - a $colour1 and $colour2 checked mantle
- `medieval_linen_veil` - a $colour linen veil

### Anglo-Danish elite female
- `medieval_linen_saerk` - a $colour linen særk
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_wool_hangerok` - a fine $colour wool hangerok
- `medieval_fine_checked_wool_mantle` - a fine $colour1 and $colour2 checked mantle
- `medieval_fine_linen_veil` - a fine $colour linen veil

### Norse / Viking Age common male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_ankle_leather_boots` - a pair of ankle leather boots
- `medieval_wool_tunic` - a $colour wool tunic
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_wool_leg_wraps` - a pair of $colour wool leg wraps
- `medieval_heavy_wool_cloak` - a heavy $colour wool cloak

### Norse / Viking Age elite male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt
- `medieval_high_leather_boots` - a pair of high leather boots
- `medieval_tablet_banded_wool_tunic` - a $colour tablet-banded tunic
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_fur_lined_cloak` - a $colour fur-lined cloak
- `medieval_felt_cap` - a $colour felt cap

### Norse / Viking Age common female
- `medieval_linen_saerk` - a $colour linen særk
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_wool_hangerok` - a $colour wool hangerok
- `medieval_brooched_wool_mantle` - a $colour brooched wool mantle
- `medieval_linen_veil` - a $colour linen veil

### Norse / Viking Age elite female
- `medieval_linen_saerk` - a $colour linen særk
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_wool_hangerok` - a fine $colour wool hangerok
- `medieval_fine_checked_wool_mantle` - a fine $colour1 and $colour2 checked mantle
- `medieval_fur_lined_cloak` - a $colour fur-lined cloak

### Norman common male
- `medieval_linen_braies` - a $colour pair of linen braies
- `medieval_wool_chausses` - a pair of $colour wool chausses
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_norman_split_tunic` - a split-skirt $colour wool tunic
- `medieval_half_circle_mantle` - a $colour half-circle mantle
- `medieval_linen_coif` - a $colour linen coif

### Norman elite male
- `medieval_linen_braies` - a $colour pair of linen braies
- `medieval_wool_chausses` - a pair of $colour wool chausses
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_split_riding_tunic` - a fine split-skirt $colour tunic
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle
- `medieval_linen_coif` - a $colour linen coif

### Norman common female
- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_simple_woven_sash` - a $colour woven sash
- `medieval_straight_wool_gown` - a straight $colour wool gown
- `medieval_linen_veil` - a $colour linen veil
- `medieval_simple_wool_mantle` - a $colour wool mantle

### Norman elite female
- `medieval_linen_chainse` - a $colour linen chainse
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_fine_bliaut` - a fine $colour bliaut
- `medieval_fine_linen_veil` - a fine $colour linen veil
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle

### Anglo-Norman common male
- `medieval_linen_braies` - a $colour pair of linen braies
- `medieval_wool_chausses` - a pair of $colour wool chausses
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_capetian_cotte` - a $colour wool cotte
- `medieval_half_circle_mantle` - a $colour half-circle mantle
- `medieval_linen_coif` - a $colour linen coif

### Anglo-Norman elite male
- `medieval_linen_braies` - a $colour pair of linen braies
- `medieval_wool_chausses` - a pair of $colour wool chausses
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_wool_cotte` - a fine $colour wool cotte
- `medieval_fine_silk_surcoat` - a fine $colour silk surcoat
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle

### Anglo-Norman common female
- `medieval_linen_chainse` - a $colour linen chainse
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_simple_woven_sash` - a $colour woven sash
- `medieval_wool_kirtle` - a $colour wool kirtle
- `medieval_linen_wimple` - a $colour linen wimple
- `medieval_half_circle_mantle` - a $colour half-circle mantle

### Anglo-Norman elite female
- `medieval_linen_chainse` - a $colour linen chainse
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_fine_bliaut` - a fine $colour bliaut
- `medieval_barbette_veil` - a $colour linen barbette veil
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle

### High English / British common male
- `medieval_linen_braies` - a $colour pair of linen braies
- `medieval_wool_hose` - a pair of $colour wool hose
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_capetian_cotte` - a $colour wool cotte
- `medieval_simple_surcoat` - a sleeveless $colour surcoat
- `medieval_wool_hood` - a $colour wool hood

### High English / British elite male
- `medieval_linen_braies` - a $colour pair of linen braies
- `medieval_wool_chausses` - a pair of $colour wool chausses
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_wool_cotte` - a fine $colour wool cotte
- `medieval_fine_silk_surcoat` - a fine $colour silk surcoat
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle

### High English / British common female
- `medieval_linen_chainse` - a $colour linen chainse
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_simple_woven_sash` - a $colour woven sash
- `medieval_wool_kirtle` - a $colour wool kirtle
- `medieval_linen_wimple` - a $colour linen wimple
- `medieval_simple_wool_mantle` - a $colour wool mantle

### High English / British elite female
- `medieval_linen_chainse` - a $colour linen chainse
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_fine_wool_kirtle` - a fine $colour wool kirtle
- `medieval_barbette_veil` - a $colour linen barbette veil
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle

### Irish / Gaelic common male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_gaelic_leine` - a long $colour linen léine
- `medieval_wool_trews` - a pair of $colour wool trews
- `medieval_wool_brat` - a $colour wool brat

### Irish / Gaelic elite male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt
- `medieval_ankle_leather_boots` - a pair of ankle leather boots
- `medieval_fine_bordered_leine` - a fine $colour bordered léine
- `medieval_wool_trews` - a pair of $colour wool trews
- `medieval_fine_bordered_brat` - a fine $colour bordered brat

### Irish / Gaelic common female
- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_simple_woven_sash` - a $colour woven sash
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_gaelic_leine` - a long $colour linen léine
- `medieval_linen_veil` - a $colour linen veil
- `medieval_wool_brat` - a $colour wool brat

### Irish / Gaelic elite female
- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_bordered_leine` - a fine $colour bordered léine
- `medieval_fine_linen_veil` - a fine $colour linen veil
- `medieval_fine_bordered_brat` - a fine $colour bordered brat

### Scottish / Gaelic-Lowland common male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_ankle_leather_boots` - a pair of ankle leather boots
- `medieval_gaelic_leine` - a long $colour linen léine
- `medieval_wool_trews` - a pair of $colour wool trews
- `medieval_wool_brat` - a $colour wool brat

### Scottish / Gaelic-Lowland elite male
- `medieval_linen_braies` - a $colour pair of linen braies
- `medieval_wool_chausses` - a pair of $colour wool chausses
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_wool_cotte` - a fine $colour wool cotte
- `medieval_fine_bordered_brat` - a fine $colour bordered brat
- `medieval_linen_coif` - a $colour linen coif

### Scottish / Gaelic-Lowland common female
- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_simple_woven_sash` - a $colour woven sash
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_wool_kirtle` - a $colour wool kirtle
- `medieval_linen_veil` - a $colour linen veil
- `medieval_wool_brat` - a $colour wool brat

### Scottish / Gaelic-Lowland elite female
- `medieval_linen_chainse` - a $colour linen chainse
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_wool_kirtle` - a fine $colour wool kirtle
- `medieval_fine_linen_veil` - a fine $colour linen veil
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle

### Carolingian / Frankish common male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_frankish_knee_tunic` - a knee-length $colour wool tunic
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_wool_leg_wraps` - a pair of $colour wool leg wraps
- `medieval_carolingian_cloak` - a $colour shoulder-fastened cloak

### Carolingian / Frankish elite male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt
- `medieval_ankle_leather_boots` - a pair of ankle leather boots
- `medieval_fine_frankish_tunic` - a fine $colour bordered tunic
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_fur_lined_cloak` - a $colour fur-lined cloak
- `medieval_linen_coif` - a $colour linen coif

### Carolingian / Frankish common female
- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_straight_wool_gown` - a straight $colour wool gown
- `medieval_linen_veil` - a $colour linen veil
- `medieval_carolingian_cloak` - a $colour shoulder-fastened cloak

### Carolingian / Frankish elite female
- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_bordered_wool_gown` - a fine $colour bordered gown
- `medieval_fine_linen_veil` - a fine $colour linen veil
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle

### Capetian French common male
- `medieval_linen_braies` - a $colour pair of linen braies
- `medieval_wool_chausses` - a pair of $colour wool chausses
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_capetian_cotte` - a $colour wool cotte
- `medieval_half_circle_mantle` - a $colour half-circle mantle
- `medieval_linen_coif` - a $colour linen coif

### Capetian French elite male
- `medieval_linen_braies` - a $colour pair of linen braies
- `medieval_wool_chausses` - a pair of $colour wool chausses
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_wool_cotte` - a fine $colour wool cotte
- `medieval_fine_silk_surcoat` - a fine $colour silk surcoat
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle

### Capetian French common female
- `medieval_linen_chainse` - a $colour linen chainse
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_simple_woven_sash` - a $colour woven sash
- `medieval_wool_kirtle` - a $colour wool kirtle
- `medieval_linen_wimple` - a $colour linen wimple
- `medieval_half_circle_mantle` - a $colour half-circle mantle

### Capetian French elite female
- `medieval_linen_chainse` - a $colour linen chainse
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_fine_bliaut` - a fine $colour bliaut
- `medieval_barbette_veil` - a $colour linen barbette veil
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle

### Holy Roman Empire / German common male
- `medieval_linen_braies` - a $colour pair of linen braies
- `medieval_wool_hose` - a pair of $colour wool hose
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_ankle_leather_boots` - a pair of ankle leather boots
- `medieval_capetian_cotte` - a $colour wool cotte
- `medieval_gugel_hood` - a $colour wool gugel hood
- `medieval_heavy_wool_cloak` - a heavy $colour wool cloak

### Holy Roman Empire / German elite male
- `medieval_linen_braies` - a $colour pair of linen braies
- `medieval_wool_chausses` - a pair of $colour wool chausses
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt
- `medieval_high_leather_boots` - a pair of high leather boots
- `medieval_fine_wool_cotte` - a fine $colour wool cotte
- `medieval_fine_silk_surcoat` - a fine $colour silk surcoat
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle

### Holy Roman Empire / German common female
- `medieval_linen_chainse` - a $colour linen chainse
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_simple_woven_sash` - a $colour woven sash
- `medieval_wool_kirtle` - a $colour wool kirtle
- `medieval_linen_wimple` - a $colour linen wimple
- `medieval_gugel_hood` - a $colour wool gugel hood

### Holy Roman Empire / German elite female
- `medieval_linen_chainse` - a $colour linen chainse
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_fine_wool_kirtle` - a fine $colour wool kirtle
- `medieval_fine_linen_veil` - a fine $colour linen veil
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle

### Christian Iberian common male
- `medieval_christian_iberian_camisa` - a $colour linen camisa
- `medieval_wool_hose` - a pair of $colour wool hose
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_wool_tunic` - a $colour wool tunic
- `medieval_iberian_manto` - a $colour wool manto
- `medieval_linen_coif` - a $colour linen coif

### Christian Iberian elite male
- `medieval_christian_iberian_camisa` - a $colour linen camisa
- `medieval_wool_chausses` - a pair of $colour wool chausses
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_wool_cotte` - a fine $colour wool cotte
- `medieval_fine_silk_pellote` - a fine $colour silk pellote
- `medieval_fine_iberian_manto` - a fine $colour silk-lined manto

### Christian Iberian common female
- `medieval_christian_iberian_camisa` - a $colour linen camisa
- `medieval_simple_woven_sash` - a $colour woven sash
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_wool_saya` - a $colour wool saya
- `medieval_linen_toca` - a $colour linen toca
- `medieval_iberian_manto` - a $colour wool manto

### Christian Iberian elite female
- `medieval_linen_chainse` - a $colour linen chainse
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_wool_saya` - a fine $colour wool saya
- `medieval_fine_silk_pellote` - a fine $colour silk pellote
- `medieval_fine_iberian_manto` - a fine $colour silk-lined manto
- `medieval_fine_linen_veil` - a fine $colour linen veil

### Andalusian common male
- `medieval_linen_izar` - a $colour linen izar
- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_wrapped_cotton_turban` - a $colour cotton turban
- `medieval_soft_leather_slippers` - a pair of soft leather slippers
- `medieval_cotton_qamis` - a $colour cotton qamis
- `medieval_wool_burnous` - a $colour wool burnous

### Andalusian elite male
- `medieval_linen_izar` - a $colour linen izar
- `medieval_fine_silk_sirwal` - a pair of fine $colour silk sirwal
- `medieval_fine_silk_turban` - a fine $colour silk turban
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_silk_qamis` - a fine $colour silk qamis
- `medieval_fine_tiraz_robe` - a fine $colour tiraz robe
- `medieval_fine_burnous` - a fine $colour wool burnous

### Andalusian common female
- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_cotton_head_veil` - a $colour cotton head veil
- `medieval_soft_leather_slippers` - a pair of soft leather slippers
- `medieval_cotton_qamis` - a $colour cotton qamis
- `medieval_wool_jubba` - a $colour wool jubba
- `medieval_wool_burnous` - a $colour wool burnous

### Andalusian elite female
- `medieval_fine_silk_sirwal` - a pair of fine $colour silk sirwal
- `medieval_fine_silk_veil` - a fine $colour silk veil
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_silk_qamis` - a fine $colour silk qamis
- `medieval_fine_tiraz_robe` - a fine $colour tiraz robe
- `medieval_fine_burnous` - a fine $colour wool burnous

### Byzantine common male
- `medieval_linen_kamision` - a $colour linen kamision
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_soft_campagi_shoes` - a pair of soft leather campagi
- `medieval_wool_dalmatic` - a $colour wool dalmatic
- `medieval_sagion` - a $colour sagion
- `medieval_linen_coif` - a $colour linen coif

### Byzantine elite male
- `medieval_linen_kamision` - a $colour linen kamision
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_soft_campagi_shoes` - a pair of soft leather campagi
- `medieval_silk_dalmatic` - a fine $colour silk dalmatic
- `medieval_skaramangion` - a $colour skaramangion
- `medieval_tablion_cloak` - a $colour tablion cloak
- `medieval_loros_sash` - a fine $colour loros sash

### Byzantine common female
- `medieval_linen_kamision` - a $colour linen kamision
- `medieval_simple_woven_sash` - a $colour woven sash
- `medieval_soft_campagi_shoes` - a pair of soft leather campagi
- `medieval_wool_dalmatic` - a $colour wool dalmatic
- `medieval_maforion` - a $colour maforion veil
- `medieval_sagion` - a $colour sagion

### Byzantine elite female
- `medieval_linen_kamision` - a $colour linen kamision
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_soft_campagi_shoes` - a pair of soft leather campagi
- `medieval_silk_dalmatic` - a fine $colour silk dalmatic
- `medieval_skaramangion` - a $colour skaramangion
- `medieval_fine_maforion` - a fine $colour maforion veil
- `medieval_tablion_cloak` - a $colour tablion cloak

### Abbasid common male
- `medieval_linen_izar` - a $colour linen izar
- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_wrapped_cotton_turban` - a $colour cotton turban
- `medieval_soft_leather_slippers` - a pair of soft leather slippers
- `medieval_linen_qamis` - a $colour linen qamis
- `medieval_wool_jubba` - a $colour wool jubba

### Abbasid elite male
- `medieval_linen_izar` - a $colour linen izar
- `medieval_fine_silk_sirwal` - a pair of fine $colour silk sirwal
- `medieval_fine_silk_turban` - a fine $colour silk turban
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_silk_qamis` - a fine $colour silk qamis
- `medieval_fine_tiraz_robe` - a fine $colour tiraz robe

### Abbasid common female
- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_cotton_head_veil` - a $colour cotton head veil
- `medieval_soft_leather_slippers` - a pair of soft leather slippers
- `medieval_cotton_qamis` - a $colour cotton qamis
- `medieval_wool_jubba` - a $colour wool jubba
- `medieval_linen_izar` - a $colour linen izar

### Abbasid elite female
- `medieval_fine_silk_sirwal` - a pair of fine $colour silk sirwal
- `medieval_fine_silk_veil` - a fine $colour silk veil
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_silk_qamis` - a fine $colour silk qamis
- `medieval_fine_tiraz_robe` - a fine $colour tiraz robe
- `medieval_fine_silk_turban` - a fine $colour silk turban

### Fatimid common male
- `medieval_linen_izar` - a $colour linen izar
- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_wrapped_cotton_turban` - a $colour cotton turban
- `medieval_soft_leather_slippers` - a pair of soft leather slippers
- `medieval_cotton_qamis` - a $colour cotton qamis
- `medieval_wool_burnous` - a $colour wool burnous

### Fatimid elite male
- `medieval_linen_izar` - a $colour linen izar
- `medieval_fine_silk_sirwal` - a pair of fine $colour silk sirwal
- `medieval_fine_silk_turban` - a fine $colour silk turban
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_silk_qamis` - a fine $colour silk qamis
- `medieval_fine_tiraz_robe` - a fine $colour tiraz robe
- `medieval_fine_burnous` - a fine $colour wool burnous

### Fatimid common female
- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_cotton_head_veil` - a $colour cotton head veil
- `medieval_soft_leather_slippers` - a pair of soft leather slippers
- `medieval_cotton_qamis` - a $colour cotton qamis
- `medieval_wool_jubba` - a $colour wool jubba
- `medieval_wool_burnous` - a $colour wool burnous

### Fatimid elite female
- `medieval_fine_silk_sirwal` - a pair of fine $colour silk sirwal
- `medieval_fine_silk_veil` - a fine $colour silk veil
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_silk_qamis` - a fine $colour silk qamis
- `medieval_fine_tiraz_robe` - a fine $colour tiraz robe
- `medieval_fine_burnous` - a fine $colour wool burnous

### Seljuk / Ayyubid-Mamluk common male
- `medieval_linen_izar` - a $colour linen izar
- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_wrapped_cotton_turban` - a $colour cotton turban
- `medieval_ankle_leather_boots` - a pair of ankle leather boots
- `medieval_linen_qamis` - a $colour linen qamis
- `medieval_seljuk_caftan` - a $colour front-opening caftan
- `medieval_felt_turkic_cap` - a $colour pointed felt cap

### Seljuk / Ayyubid-Mamluk elite male
- `medieval_linen_izar` - a $colour linen izar
- `medieval_fine_silk_sirwal` - a pair of fine $colour silk sirwal
- `medieval_fine_silk_turban` - a fine $colour silk turban
- `medieval_high_leather_boots` - a pair of high leather boots
- `medieval_fine_silk_qamis` - a fine $colour silk qamis
- `medieval_fine_seljuk_caftan` - a fine $colour silk caftan
- `medieval_fine_tiraz_robe` - a fine $colour tiraz robe

### Seljuk / Ayyubid-Mamluk common female
- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_cotton_head_veil` - a $colour cotton head veil
- `medieval_soft_leather_slippers` - a pair of soft leather slippers
- `medieval_cotton_qamis` - a $colour cotton qamis
- `medieval_wool_jubba` - a $colour wool jubba
- `medieval_wool_burnous` - a $colour wool burnous

### Seljuk / Ayyubid-Mamluk elite female
- `medieval_fine_silk_sirwal` - a pair of fine $colour silk sirwal
- `medieval_fine_silk_veil` - a fine $colour silk veil
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_silk_qamis` - a fine $colour silk qamis
- `medieval_fine_seljuk_caftan` - a fine $colour silk caftan
- `medieval_fine_tiraz_robe` - a fine $colour tiraz robe

### Magyar common male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_high_leather_boots` - a pair of high leather boots
- `medieval_magyar_riding_coat` - a $colour belted riding coat
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_steppe_felt_cap` - a $colour steppe felt cap

### Magyar elite male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_steppe_silk_belt` - a fine $colour silk belt
- `medieval_high_leather_boots` - a pair of high leather boots
- `medieval_fine_magyar_coat` - a fine $colour fur-trimmed coat
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_fine_steppe_caftan` - a fine $colour silk-banded caftan
- `medieval_felt_turkic_cap` - a $colour pointed felt cap

### Magyar common female
- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_ankle_leather_boots` - a pair of ankle leather boots
- `medieval_magyar_riding_coat` - a $colour belted riding coat
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_linen_veil` - a $colour linen veil

### Magyar elite female
- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_steppe_silk_belt` - a fine $colour silk belt
- `medieval_high_leather_boots` - a pair of high leather boots
- `medieval_fine_magyar_coat` - a fine $colour fur-trimmed coat
- `medieval_fine_steppe_caftan` - a fine $colour silk-banded caftan
- `medieval_fine_linen_veil` - a fine $colour linen veil

### Rus / Novgorod common male
- `medieval_rus_linen_rubakha` - a $colour linen rubakha
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_ankle_leather_boots` - a pair of ankle leather boots
- `medieval_rus_wool_porty` - a pair of $colour wool porty
- `medieval_rus_onuchi` - a pair of $colour onuchi leg wraps
- `medieval_wool_brat` - a $colour wool brat
- `medieval_rus_fur_hat` - a fur-trimmed wool hat

### Rus / Novgorod elite male
- `medieval_fine_rus_rubakha` - a fine $colour embroidered rubakha
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt
- `medieval_high_leather_boots` - a pair of high leather boots
- `medieval_rus_wool_porty` - a pair of $colour wool porty
- `medieval_rus_shuba` - a $colour fur-lined shuba
- `medieval_rus_fur_hat` - a fur-trimmed wool hat

### Rus / Novgorod common female
- `medieval_rus_linen_rubakha` - a $colour linen rubakha
- `medieval_simple_woven_sash` - a $colour woven sash
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_rus_poneva` - a $colour1 and $colour2 woven poneva
- `medieval_rus_sleeveless_overgown` - a $colour sleeveless overgown
- `medieval_linen_veil` - a $colour linen veil

### Rus / Novgorod elite female
- `medieval_fine_rus_rubakha` - a fine $colour embroidered rubakha
- `medieval_fine_woven_girdle` - a fine $colour woven girdle
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_rus_poneva` - a $colour1 and $colour2 woven poneva
- `medieval_rus_sleeveless_overgown` - a $colour sleeveless overgown
- `medieval_rus_shuba` - a $colour fur-lined shuba
- `medieval_fine_linen_veil` - a fine $colour linen veil

### Steppe Turkic / Mongol common male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_steppe_felt_boots` - a pair of felt riding boots
- `medieval_wool_deel` - a $colour belted steppe robe
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_steppe_felt_cap` - a $colour steppe felt cap

### Steppe Turkic / Mongol elite male
- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_steppe_silk_belt` - a fine $colour silk belt
- `medieval_high_leather_boots` - a pair of high leather boots
- `medieval_fine_silk_deel` - a fine $colour silk steppe robe
- `medieval_fine_steppe_caftan` - a fine $colour silk-banded caftan
- `medieval_felt_turkic_cap` - a $colour pointed felt cap

### Steppe Turkic / Mongol common female
- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_plain_leather_belt` - a plain leather belt
- `medieval_steppe_felt_boots` - a pair of felt riding boots
- `medieval_wool_deel` - a $colour belted steppe robe
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_steppe_felt_cap` - a $colour steppe felt cap

### Steppe Turkic / Mongol elite female
- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_steppe_silk_belt` - a fine $colour silk belt
- `medieval_high_leather_boots` - a pair of high leather boots
- `medieval_fine_silk_deel` - a fine $colour silk steppe robe
- `medieval_fine_steppe_caftan` - a fine $colour silk-banded caftan
- `medieval_fine_silk_veil` - a fine $colour silk veil

### North Indian / Rajput common male
- `medieval_cotton_dhoti` - a $colour cotton dhoti
- `medieval_cotton_uttariya` - a $colour cotton uttariya
- `medieval_cotton_turban` - a $colour cotton turban
- `medieval_plain_leather_sandals` - a pair of plain leather sandals
- `medieval_plain_leather_belt` - a plain leather belt

### North Indian / Rajput elite male
- `medieval_fine_cotton_dhoti` - a fine $colour cotton dhoti
- `medieval_fine_silk_uttariya` - a fine $colour silk uttariya
- `medieval_fine_silk_turban_indian` - a fine $colour silk turban
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_woven_girdle` - a fine $colour woven girdle

### North Indian / Rajput common female
- `medieval_linen_breastband` - a $colour linen breastband
- `medieval_cotton_lower_cloth` - a $colour cotton lower cloth
- `medieval_cotton_sari` - a $colour cotton sari
- `medieval_short_cotton_bodice` - a short $colour cotton bodice
- `medieval_hemp_sandals` - a pair of woven hemp sandals
- `medieval_cotton_uttariya` - a $colour cotton uttariya

### North Indian / Rajput elite female
- `medieval_linen_breastband` - a $colour linen breastband
- `medieval_fine_silk_wrap_skirt` - a fine $colour silk wrap skirt
- `medieval_fine_silk_sari` - a fine $colour silk sari
- `medieval_fine_silk_bodice` - a fine $colour silk bodice
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_silk_uttariya` - a fine $colour silk uttariya

### South Indian / Chola common male
- `medieval_south_indian_veshti` - a $colour cotton veshti
- `medieval_south_indian_angavastram` - a $colour cotton angavastram
- `medieval_plain_leather_sandals` - a pair of plain leather sandals
- `medieval_cotton_turban` - a $colour cotton turban

### South Indian / Chola elite male
- `medieval_silk_bordered_veshti` - a fine $colour silk-bordered veshti
- `medieval_fine_angavastram` - a fine $colour silk angavastram
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_silk_turban_indian` - a fine $colour silk turban
- `medieval_fine_woven_girdle` - a fine $colour woven girdle

### South Indian / Chola common female
- `medieval_linen_breastband` - a $colour linen breastband
- `medieval_cotton_sari` - a $colour cotton sari
- `medieval_short_cotton_bodice` - a short $colour cotton bodice
- `medieval_hemp_sandals` - a pair of woven hemp sandals
- `medieval_south_indian_angavastram` - a $colour cotton angavastram

### South Indian / Chola elite female
- `medieval_linen_breastband` - a $colour linen breastband
- `medieval_fine_silk_sari` - a fine $colour silk sari
- `medieval_fine_silk_bodice` - a fine $colour silk bodice
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_angavastram` - a fine $colour silk angavastram
- `medieval_fine_woven_girdle` - a fine $colour woven girdle

### Song China common male
- `medieval_song_trousers` - a pair of $colour cloth trousers
- `medieval_song_cloth_shoes` - a pair of cloth shoes
- `medieval_song_cross_collar_robe` - a $colour cross-collar robe
- `medieval_song_headscarf` - a $colour cloth headscarf

### Song China elite male
- `medieval_song_trousers` - a pair of $colour cloth trousers
- `medieval_song_cloth_shoes` - a pair of cloth shoes
- `medieval_song_round_collar_robe` - a $colour round-collar robe
- `medieval_song_beizi` - a long $colour beizi over-robe
- `medieval_song_official_cap` - a black official cap

### Song China common female
- `medieval_song_cloth_shoes` - a pair of cloth shoes
- `medieval_song_ru_jacket` - a $colour cross-collar jacket
- `medieval_song_changqun_skirt` - a $colour long pleated skirt
- `medieval_song_headscarf` - a $colour cloth headscarf

### Song China elite female
- `medieval_song_cloth_shoes` - a pair of cloth shoes
- `medieval_song_ru_jacket` - a $colour cross-collar jacket
- `medieval_song_changqun_skirt` - a $colour long pleated skirt
- `medieval_song_beizi` - a long $colour beizi over-robe
- `medieval_fine_silk_veil` - a fine $colour silk veil

### Goryeo Korea common male
- `medieval_goryeo_baji` - a pair of $colour baji trousers
- `medieval_song_cloth_shoes` - a pair of cloth shoes
- `medieval_goryeo_cross_collar_jacket` - a $colour cross-collar jacket
- `medieval_goryeo_headcloth` - a $colour cloth headcloth

### Goryeo Korea elite male
- `medieval_goryeo_baji` - a pair of $colour baji trousers
- `medieval_song_cloth_shoes` - a pair of cloth shoes
- `medieval_goryeo_po_robe` - a $colour po over-robe
- `medieval_goryeo_felt_hat` - a $colour felt hat

### Goryeo Korea common female
- `medieval_song_cloth_shoes` - a pair of cloth shoes
- `medieval_goryeo_cross_collar_jacket` - a $colour cross-collar jacket
- `medieval_goryeo_chima` - a $colour chima skirt
- `medieval_goryeo_headcloth` - a $colour cloth headcloth

### Goryeo Korea elite female
- `medieval_song_cloth_shoes` - a pair of cloth shoes
- `medieval_goryeo_cross_collar_jacket` - a $colour cross-collar jacket
- `medieval_goryeo_chima` - a $colour chima skirt
- `medieval_goryeo_silk_overrobe` - a fine $colour silk over-robe
- `medieval_fine_silk_veil` - a fine $colour silk veil

### Heian / Kamakura Japan common male
- `medieval_japanese_hemp_sandals` - a pair of hemp sandals
- `medieval_japanese_kosode` - a $colour kosode robe
- `medieval_japanese_hakama` - a pair of $colour hakama
- `medieval_japanese_headcloth` - a $colour linen headcloth

### Heian / Kamakura Japan elite male
- `medieval_japanese_hemp_sandals` - a pair of hemp sandals
- `medieval_japanese_kosode` - a $colour kosode robe
- `medieval_japanese_hakama` - a pair of $colour hakama
- `medieval_japanese_suikan` - a $colour suikan robe
- `medieval_japanese_eboshi` - a black eboshi cap

### Heian / Kamakura Japan common female
- `medieval_japanese_hemp_sandals` - a pair of hemp sandals
- `medieval_japanese_kosode` - a $colour kosode robe
- `medieval_japanese_hakama` - a pair of $colour hakama
- `medieval_japanese_headcloth` - a $colour linen headcloth

### Heian / Kamakura Japan elite female
- `medieval_japanese_hemp_sandals` - a pair of hemp sandals
- `medieval_japanese_kosode` - a $colour kosode robe
- `medieval_japanese_hakama` - a pair of $colour hakama
- `medieval_japanese_uchigi` - a fine $colour uchigi robe
- `medieval_japanese_kariginu` - a fine $colour kariginu robe
- `medieval_japanese_eboshi` - a black eboshi cap

## Item catalogue

Catalogue line format: `uniqueReference` - public short description; noun; primary material; size/quality; weight/cost; wear component; variable component. Seasonal, urban, and professional add-on lines may append `matches:` to identify the outfit manifests they can be added to or substituted into. Full descriptions remain in the seeder calls and are not duplicated here.

### Shared medieval underlayers, belts, footwear, and common outerwear

- `medieval_linen_braies` - a $colour pair of linen braies; noun: `braies`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 160g/6.0m; wear: `Wear_Shorts`; variables: Variable_BasicColour.
- `medieval_linen_undertunic` - a $colour linen undertunic; noun: `undertunic`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 260g/8.0m; wear: `Wear_Shirt`; variables: Variable_BasicColour.
- `medieval_linen_chemise` - a $colour linen chemise; noun: `chemise`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 300g/10.0m; wear: `Wear_Shirt`; variables: Variable_BasicColour.
- `medieval_linen_breastband` - a $colour linen breastband; noun: `breastband`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 80g/4.0m; wear: `Wear_Bra`; variables: Variable_BasicColour.
- `medieval_plain_leather_belt` - a plain leather belt; noun: `belt`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 180g/10.0m; wear: `Wear_Waist`; variables: none.
- `medieval_iron_buckled_leather_belt` - an iron-buckled leather belt; noun: `belt`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 240g/30.0m; wear: `Wear_Waist`; variables: none.
- `medieval_simple_woven_sash` - a $colour woven sash; noun: `sash`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 120g/6.0m; wear: `Wear_Sash`; variables: Variable_BasicColour.
- `medieval_fine_woven_girdle` - a fine $colour woven girdle; noun: `girdle`; material: `wool`; size/quality: `Small`/`Good`; weight/cost: 100g/24.0m; wear: `Wear_Sash`; variables: Variable_FineColour.
- `medieval_soft_leather_shoes` - a pair of soft leather shoes; noun: `shoes`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 420g/18.0m; wear: `Wear_Shoes`; variables: none.
- `medieval_fine_leather_shoes` - a pair of fine leather shoes; noun: `shoes`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 380g/44.0m; wear: `Wear_Shoes`; variables: none.
- `medieval_ankle_leather_boots` - a pair of ankle leather boots; noun: `boots`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 650g/30.0m; wear: `Wear_Boots`; variables: none.
- `medieval_high_leather_boots` - a pair of high leather boots; noun: `boots`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 900g/64.0m; wear: `Wear_High_Boots`; variables: none.
- `medieval_plain_leather_sandals` - a pair of plain leather sandals; noun: `sandals`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 320g/12.0m; wear: `Wear_Sandals`; variables: none.
- `medieval_hemp_sandals` - a pair of woven hemp sandals; noun: `sandals`; material: `hemp`; size/quality: `Small`/`Standard`; weight/cost: 220g/6.0m; wear: `Wear_Sandals`; variables: none.
- `medieval_wool_hose` - a pair of $colour wool hose; noun: `hose`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 220g/10.0m; wear: `Wear_Stockings`; variables: Variable_BasicColour.
- `medieval_wool_chausses` - a pair of $colour wool chausses; noun: `chausses`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 320g/14.0m; wear: `Wear_Chausses`; variables: Variable_BasicColour.
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers; noun: `trousers`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 450g/16.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour.
- `medieval_wool_tunic` - a $colour wool tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 520g/16.0m; wear: `Wear_Tunic`; variables: Variable_BasicColour.
- `medieval_long_wool_tunic` - a long $colour wool tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 680g/20.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour.
- `medieval_heavy_wool_cloak` - a heavy $colour wool cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1250g/34.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_BasicColour.
- `medieval_simple_wool_mantle` - a $colour wool mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 820g/22.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `medieval_fur_lined_cloak` - a $colour fur-lined cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1550g/110.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_FineColour.
- `medieval_linen_coif` - a $colour linen coif; noun: `coif`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 90g/5.0m; wear: `Wear_Coif`; variables: Variable_BasicColour.
- `medieval_linen_veil` - a $colour linen veil; noun: `veil`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 120g/7.0m; wear: `Wear_Veil`; variables: Variable_BasicColour.
- `medieval_linen_wimple` - a $colour linen wimple; noun: `wimple`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 160g/9.0m; wear: `Wear_Veil`; variables: Variable_BasicColour.
- `medieval_wool_hood` - a $colour wool hood; noun: `hood`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 260g/14.0m; wear: `Wear_Hoodie`; variables: Variable_BasicColour.
- `medieval_felt_cap` - a $colour felt cap; noun: `cap`; material: `felt`; size/quality: `Small`/`Standard`; weight/cost: 120g/8.0m; wear: `Wear_Hat`; variables: Variable_BasicColour.
### Insular, North Sea, and Gaelic families

- `medieval_rectangular_wool_cloak` - a $colour rectangular wool cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 950g/24.0m; wear: `Wear_Cloak_(Open)`; variables: Variable_BasicColour.
- `medieval_brooched_wool_mantle` - a $colour brooched wool mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 900g/28.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `medieval_tablet_banded_wool_tunic` - a $colour tablet-banded tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 560g/44.0m; wear: `Wear_Tunic`; variables: Variable_FineColour.
- `medieval_straight_wool_gown` - a straight $colour wool gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 760g/22.0m; wear: `Wear_Gown`; variables: Variable_BasicColour.
- `medieval_pinned_wool_gown` - a pinned $colour wool gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 780g/24.0m; wear: `Wear_Gown`; variables: Variable_BasicColour.
- `medieval_fine_bordered_wool_gown` - a fine $colour bordered gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 820g/70.0m; wear: `Wear_Gown`; variables: Variable_FineColour.
- `medieval_wool_leg_wraps` - a pair of $colour wool leg wraps; noun: `leg wraps`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 180g/6.0m; wear: `Wear_Leggings`; variables: Variable_BasicColour.
- `medieval_linen_saerk` - a $colour linen særk; noun: `særk`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 340g/12.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour.
- `medieval_wool_hangerok` - a $colour wool hangerok; noun: `hangerok`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 620g/22.0m; wear: `Wear_Sleeveless_Dress`; variables: Variable_BasicColour.
- `medieval_fine_wool_hangerok` - a fine $colour wool hangerok; noun: `hangerok`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 620g/64.0m; wear: `Wear_Sleeveless_Dress`; variables: Variable_FineColour.
- `medieval_checked_wool_mantle` - a $colour1 and $colour2 checked mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 900g/30.0m; wear: `Wear_Mantle`; variables: Variable_2BasicColour.
- `medieval_fine_checked_wool_mantle` - a fine $colour1 and $colour2 checked mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 920g/86.0m; wear: `Wear_Mantle`; variables: Variable_2FineColour.
- `medieval_gaelic_leine` - a long $colour linen léine; noun: `léine`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 520g/18.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour.
- `medieval_fine_bordered_leine` - a fine $colour bordered léine; noun: `léine`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 560g/58.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_FineColour.
- `medieval_wool_brat` - a $colour wool brat; noun: `brat`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 980g/28.0m; wear: `Wear_Cloak_(Open)`; variables: Variable_BasicColour.
- `medieval_fine_bordered_brat` - a fine $colour bordered brat; noun: `brat`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1000g/82.0m; wear: `Wear_Cloak_(Open)`; variables: Variable_FineColour.
- `medieval_wool_trews` - a pair of $colour wool trews; noun: `trews`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 480g/18.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour.
### Western and central European high medieval families

- `medieval_frankish_knee_tunic` - a knee-length $colour wool tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 500g/16.0m; wear: `Wear_Tunic`; variables: Variable_BasicColour.
- `medieval_fine_frankish_tunic` - a fine $colour bordered tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 540g/54.0m; wear: `Wear_Tunic`; variables: Variable_FineColour.
- `medieval_carolingian_cloak` - a $colour shoulder-fastened cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 880g/26.0m; wear: `Wear_Cloak_(Open)`; variables: Variable_BasicColour.
- `medieval_norman_split_tunic` - a split-skirt $colour wool tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 620g/22.0m; wear: `Wear_Tunic`; variables: Variable_BasicColour.
- `medieval_fine_split_riding_tunic` - a fine split-skirt $colour tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 640g/66.0m; wear: `Wear_Tunic`; variables: Variable_FineColour.
- `medieval_capetian_cotte` - a $colour wool cotte; noun: `cotte`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 620g/22.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour.
- `medieval_fine_wool_cotte` - a fine $colour wool cotte; noun: `cotte`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 600g/58.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_FineColour.
- `medieval_linen_chainse` - a $colour linen chainse; noun: `chainse`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 320g/12.0m; wear: `Wear_Shirt`; variables: Variable_BasicColour.
- `medieval_wool_kirtle` - a $colour wool kirtle; noun: `kirtle`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 700g/24.0m; wear: `Wear_Dress`; variables: Variable_BasicColour.
- `medieval_fine_wool_kirtle` - a fine $colour wool kirtle; noun: `kirtle`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 720g/72.0m; wear: `Wear_Dress`; variables: Variable_FineColour.
- `medieval_fine_bliaut` - a fine $colour bliaut; noun: `bliaut`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 880g/96.0m; wear: `Wear_Gown`; variables: Variable_FineColour.
- `medieval_simple_surcoat` - a sleeveless $colour surcoat; noun: `surcoat`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 560g/20.0m; wear: `Wear_Sleeveless_Tunic`; variables: Variable_BasicColour.
- `medieval_fine_silk_surcoat` - a fine $colour silk surcoat; noun: `surcoat`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 420g/150.0m; wear: `Wear_Sleeveless_Tunic`; variables: Variable_FineColour.
- `medieval_half_circle_mantle` - a $colour half-circle mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 950g/30.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `medieval_fur_trimmed_mantle` - a $colour fur-trimmed mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1200g/120.0m; wear: `Wear_Mantle`; variables: Variable_FineColour.
- `medieval_gugel_hood` - a $colour wool gugel hood; noun: `hood`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 300g/18.0m; wear: `Wear_Hoodie`; variables: Variable_BasicColour.
- `medieval_barbette_veil` - a $colour linen barbette veil; noun: `veil`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 150g/10.0m; wear: `Wear_Veil`; variables: Variable_BasicColour.
- `medieval_fine_linen_veil` - a fine $colour linen veil; noun: `veil`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 110g/24.0m; wear: `Wear_Veil`; variables: Variable_FineColour.
- `medieval_christian_iberian_camisa` - a $colour linen camisa; noun: `camisa`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 300g/11.0m; wear: `Wear_Shirt`; variables: Variable_BasicColour.
- `medieval_wool_saya` - a $colour wool saya; noun: `saya`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 720g/24.0m; wear: `Wear_Dress`; variables: Variable_BasicColour.
- `medieval_fine_wool_saya` - a fine $colour wool saya; noun: `saya`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 730g/76.0m; wear: `Wear_Dress`; variables: Variable_FineColour.
- `medieval_iberian_pellote` - a sleeveless $colour pellote; noun: `pellote`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 620g/26.0m; wear: `Wear_Sleeveless_Gown`; variables: Variable_BasicColour.
- `medieval_fine_silk_pellote` - a fine $colour silk pellote; noun: `pellote`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 460g/150.0m; wear: `Wear_Sleeveless_Gown`; variables: Variable_FineColour.
- `medieval_iberian_manto` - a $colour wool manto; noun: `manto`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 900g/30.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `medieval_fine_iberian_manto` - a fine $colour silk-lined manto; noun: `manto`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1050g/128.0m; wear: `Wear_Mantle`; variables: Variable_FineColour.
- `medieval_linen_toca` - a $colour linen toca; noun: `toca`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 120g/8.0m; wear: `Wear_Veil`; variables: Variable_BasicColour.
### Byzantine and Islamic Mediterranean families

- `medieval_linen_kamision` - a $colour linen kamision; noun: `kamision`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 320g/14.0m; wear: `Wear_Shirt`; variables: Variable_BasicColour.
- `medieval_wool_dalmatic` - a $colour wool dalmatic; noun: `dalmatic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 760g/30.0m; wear: `Wear_Robe`; variables: Variable_BasicColour.
- `medieval_silk_dalmatic` - a fine $colour silk dalmatic; noun: `dalmatic`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 520g/180.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `medieval_skaramangion` - a $colour skaramangion; noun: `skaramangion`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 640g/170.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `medieval_sagion` - a $colour sagion; noun: `sagion`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 780g/32.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `medieval_tablion_cloak` - a $colour tablion cloak; noun: `cloak`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 820g/210.0m; wear: `Wear_Cloak_(Open)`; variables: Variable_FineColour.
- `medieval_loros_sash` - a fine $colour loros sash; noun: `sash`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 240g/160.0m; wear: `Wear_Sash`; variables: Variable_FineColour.
- `medieval_maforion` - a $colour maforion veil; noun: `maforion`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 180g/14.0m; wear: `Wear_Veil`; variables: Variable_BasicColour.
- `medieval_fine_maforion` - a fine $colour maforion veil; noun: `maforion`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 160g/70.0m; wear: `Wear_Veil`; variables: Variable_FineColour.
- `medieval_soft_campagi_shoes` - a pair of soft leather campagi; noun: `campagi`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 420g/54.0m; wear: `Wear_Shoes`; variables: none.
- `medieval_linen_qamis` - a $colour linen qamis; noun: `qamis`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 420g/14.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour.
- `medieval_cotton_qamis` - a $colour cotton qamis; noun: `qamis`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 420g/16.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour.
- `medieval_fine_silk_qamis` - a fine $colour silk qamis; noun: `qamis`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 340g/120.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_FineColour.
- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal; noun: `sirwal`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 360g/14.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour.
- `medieval_fine_silk_sirwal` - a pair of fine $colour silk sirwal; noun: `sirwal`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 300g/80.0m; wear: `Wear_Trousers`; variables: Variable_FineColour.
- `medieval_wool_jubba` - a $colour wool jubba; noun: `jubba`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 850g/34.0m; wear: `Wear_Robe`; variables: Variable_BasicColour.
- `medieval_fine_tiraz_robe` - a fine $colour tiraz robe; noun: `robe`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 620g/220.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `medieval_linen_izar` - a $colour linen izar; noun: `izar`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 280g/10.0m; wear: `Wear_Loincloth`; variables: Variable_BasicColour.
- `medieval_wool_burnous` - a $colour wool burnous; noun: `burnous`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 980g/34.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_BasicColour.
- `medieval_fine_burnous` - a fine $colour wool burnous; noun: `burnous`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1000g/96.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_FineColour.
- `medieval_wrapped_cotton_turban` - a $colour cotton turban; noun: `turban`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 220g/12.0m; wear: `Wear_Turban`; variables: Variable_BasicColour.
- `medieval_fine_silk_turban` - a fine $colour silk turban; noun: `turban`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 180g/70.0m; wear: `Wear_Turban`; variables: Variable_FineColour.
- `medieval_cotton_head_veil` - a $colour cotton head veil; noun: `veil`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 140g/9.0m; wear: `Wear_Veil`; variables: Variable_BasicColour.
- `medieval_fine_silk_veil` - a fine $colour silk veil; noun: `veil`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 100g/56.0m; wear: `Wear_Veil`; variables: Variable_FineColour.
- `medieval_soft_leather_slippers` - a pair of soft leather slippers; noun: `slippers`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 300g/18.0m; wear: `Wear_Shoes`; variables: none.
- `medieval_seljuk_caftan` - a $colour front-opening caftan; noun: `caftan`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 900g/42.0m; wear: `Wear_Long_Coat`; variables: Variable_BasicColour.
- `medieval_fine_seljuk_caftan` - a fine $colour silk caftan; noun: `caftan`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 680g/190.0m; wear: `Wear_Long_Coat`; variables: Variable_FineColour.
- `medieval_felt_turkic_cap` - a $colour pointed felt cap; noun: `cap`; material: `felt`; size/quality: `Small`/`Standard`; weight/cost: 150g/10.0m; wear: `Wear_Hat`; variables: Variable_BasicColour.
### Steppe, Magyar, and Rus families

- `medieval_magyar_riding_coat` - a $colour belted riding coat; noun: `coat`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 850g/38.0m; wear: `Wear_Long_Coat`; variables: Variable_BasicColour.
- `medieval_fine_magyar_coat` - a fine $colour fur-trimmed coat; noun: `coat`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1050g/120.0m; wear: `Wear_Long_Coat`; variables: Variable_FineColour.
- `medieval_steppe_riding_caftan` - a $colour riding caftan; noun: `caftan`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 880g/40.0m; wear: `Wear_Long_Coat`; variables: Variable_BasicColour.
- `medieval_fine_steppe_caftan` - a fine $colour silk-banded caftan; noun: `caftan`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 920g/125.0m; wear: `Wear_Long_Coat`; variables: Variable_FineColour.
- `medieval_steppe_felt_boots` - a pair of felt riding boots; noun: `boots`; material: `felt`; size/quality: `Small`/`Standard`; weight/cost: 700g/24.0m; wear: `Wear_Boots`; variables: none.
- `medieval_steppe_felt_cap` - a $colour steppe felt cap; noun: `cap`; material: `felt`; size/quality: `Small`/`Standard`; weight/cost: 140g/9.0m; wear: `Wear_Hat`; variables: Variable_BasicColour.
- `medieval_steppe_silk_belt` - a fine $colour silk belt; noun: `belt`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 120g/64.0m; wear: `Wear_Waist`; variables: Variable_FineColour.
- `medieval_wool_deel` - a $colour belted steppe robe; noun: `robe`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 980g/46.0m; wear: `Wear_Robe`; variables: Variable_BasicColour.
- `medieval_fine_silk_deel` - a fine $colour silk steppe robe; noun: `robe`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 700g/210.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `medieval_rus_linen_rubakha` - a $colour linen rubakha; noun: `rubakha`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 430g/14.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour.
- `medieval_fine_rus_rubakha` - a fine $colour embroidered rubakha; noun: `rubakha`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 450g/54.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_FineColour.
- `medieval_rus_wool_porty` - a pair of $colour wool porty; noun: `porty`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 420g/16.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour.
- `medieval_rus_onuchi` - a pair of $colour onuchi leg wraps; noun: `onuchi`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 160g/6.0m; wear: `Wear_Leggings`; variables: Variable_BasicColour.
- `medieval_rus_poneva` - a $colour1 and $colour2 woven poneva; noun: `poneva`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 620g/24.0m; wear: `Wear_Long_Skirt`; variables: Variable_2BasicColour.
- `medieval_rus_sleeveless_overgown` - a $colour sleeveless overgown; noun: `overgown`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 620g/26.0m; wear: `Wear_Sleeveless_Gown`; variables: Variable_BasicColour.
- `medieval_rus_shuba` - a $colour fur-lined shuba; noun: `shuba`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1500g/120.0m; wear: `Wear_Long_Coat`; variables: Variable_FineColour.
- `medieval_rus_fur_hat` - a fur-trimmed wool hat; noun: `hat`; material: `wool`; size/quality: `Small`/`Good`; weight/cost: 220g/36.0m; wear: `Wear_Hat`; variables: none.
### Indian subcontinent families

- `medieval_cotton_dhoti` - a $colour cotton dhoti; noun: `dhoti`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 280g/10.0m; wear: `Wear_Loincloth`; variables: Variable_BasicColour.
- `medieval_fine_cotton_dhoti` - a fine $colour cotton dhoti; noun: `dhoti`; material: `cotton`; size/quality: `Small`/`Good`; weight/cost: 260g/36.0m; wear: `Wear_Loincloth`; variables: Variable_FineColour.
- `medieval_cotton_uttariya` - a $colour cotton uttariya; noun: `uttariya`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 240g/10.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `medieval_fine_silk_uttariya` - a fine $colour silk uttariya; noun: `uttariya`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 180g/72.0m; wear: `Wear_Mantle`; variables: Variable_FineColour.
- `medieval_cotton_turban` - a $colour cotton turban; noun: `turban`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 200g/11.0m; wear: `Wear_Turban`; variables: Variable_BasicColour.
- `medieval_fine_silk_turban_indian` - a fine $colour silk turban; noun: `turban`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 170g/70.0m; wear: `Wear_Turban`; variables: Variable_FineColour.
- `medieval_cotton_sari` - a $colour cotton sari; noun: `sari`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 520g/18.0m; wear: `Wear_Robe`; variables: Variable_BasicColour.
- `medieval_fine_silk_sari` - a fine $colour silk sari; noun: `sari`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 420g/140.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `medieval_short_cotton_bodice` - a short $colour cotton bodice; noun: `bodice`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 180g/10.0m; wear: `Wear_Shirt`; variables: Variable_BasicColour.
- `medieval_fine_silk_bodice` - a fine $colour silk bodice; noun: `bodice`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 140g/54.0m; wear: `Wear_Shirt`; variables: Variable_FineColour.
- `medieval_cotton_lower_cloth` - a $colour cotton lower cloth; noun: `cloth`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 260g/9.0m; wear: `Wear_Loincloth`; variables: Variable_BasicColour.
- `medieval_south_indian_veshti` - a $colour cotton veshti; noun: `veshti`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 300g/10.0m; wear: `Wear_Loincloth`; variables: Variable_BasicColour.
- `medieval_silk_bordered_veshti` - a fine $colour silk-bordered veshti; noun: `veshti`; material: `cotton`; size/quality: `Small`/`Good`; weight/cost: 320g/44.0m; wear: `Wear_Loincloth`; variables: Variable_FineColour.
- `medieval_south_indian_angavastram` - a $colour cotton angavastram; noun: `angavastram`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 230g/10.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `medieval_fine_angavastram` - a fine $colour silk angavastram; noun: `angavastram`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 170g/70.0m; wear: `Wear_Mantle`; variables: Variable_FineColour.
- `medieval_cotton_wrap_skirt` - a $colour cotton wrap skirt; noun: `skirt`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 360g/14.0m; wear: `Wear_Long_Skirt`; variables: Variable_BasicColour.
- `medieval_fine_silk_wrap_skirt` - a fine $colour silk wrap skirt; noun: `skirt`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 300g/80.0m; wear: `Wear_Long_Skirt`; variables: Variable_FineColour.
### East Asian families

- `medieval_song_cross_collar_robe` - a $colour cross-collar robe; noun: `robe`; material: `silk`; size/quality: `Normal`/`Standard`; weight/cost: 620g/48.0m; wear: `Wear_Robe`; variables: Variable_BasicColour.
- `medieval_song_round_collar_robe` - a $colour round-collar robe; noun: `robe`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 650g/96.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `medieval_song_beizi` - a long $colour beizi over-robe; noun: `beizi`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 520g/92.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `medieval_song_ru_jacket` - a $colour cross-collar jacket; noun: `jacket`; material: `silk`; size/quality: `Normal`/`Standard`; weight/cost: 360g/34.0m; wear: `Wear_Jacket`; variables: Variable_BasicColour.
- `medieval_song_changqun_skirt` - a $colour long pleated skirt; noun: `skirt`; material: `silk`; size/quality: `Normal`/`Standard`; weight/cost: 480g/36.0m; wear: `Wear_Long_Skirt`; variables: Variable_BasicColour.
- `medieval_song_trousers` - a pair of $colour cloth trousers; noun: `trousers`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 360g/14.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour.
- `medieval_song_cloth_shoes` - a pair of cloth shoes; noun: `shoes`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 240g/12.0m; wear: `Wear_Shoes`; variables: none.
- `medieval_song_official_cap` - a black official cap; noun: `cap`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 150g/70.0m; wear: `Wear_Hat`; variables: none.
- `medieval_song_headscarf` - a $colour cloth headscarf; noun: `headscarf`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 90g/6.0m; wear: `Wear_Kerchief`; variables: Variable_BasicColour.
- `medieval_goryeo_cross_collar_jacket` - a $colour cross-collar jacket; noun: `jacket`; material: `silk`; size/quality: `Normal`/`Standard`; weight/cost: 380g/34.0m; wear: `Wear_Jacket`; variables: Variable_BasicColour.
- `medieval_goryeo_baji` - a pair of $colour baji trousers; noun: `baji`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 380g/16.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour.
- `medieval_goryeo_chima` - a $colour chima skirt; noun: `chima`; material: `silk`; size/quality: `Normal`/`Standard`; weight/cost: 480g/40.0m; wear: `Wear_Long_Skirt`; variables: Variable_BasicColour.
- `medieval_goryeo_po_robe` - a $colour po over-robe; noun: `po`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 620g/90.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `medieval_goryeo_silk_overrobe` - a fine $colour silk over-robe; noun: `over-robe`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 650g/140.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `medieval_goryeo_headcloth` - a $colour cloth headcloth; noun: `headcloth`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 100g/6.0m; wear: `Wear_Kerchief`; variables: Variable_BasicColour.
- `medieval_goryeo_felt_hat` - a $colour felt hat; noun: `hat`; material: `felt`; size/quality: `Small`/`Standard`; weight/cost: 150g/10.0m; wear: `Wear_Hat`; variables: Variable_BasicColour.
- `medieval_japanese_kosode` - a $colour kosode robe; noun: `kosode`; material: `silk`; size/quality: `Normal`/`Standard`; weight/cost: 560g/48.0m; wear: `Wear_Robe`; variables: Variable_BasicColour.
- `medieval_japanese_hakama` - a pair of $colour hakama; noun: `hakama`; material: `silk`; size/quality: `Normal`/`Standard`; weight/cost: 420g/38.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour.
- `medieval_japanese_suikan` - a $colour suikan robe; noun: `suikan`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 600g/100.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `medieval_japanese_hitatare` - a $colour hitatare robe; noun: `hitatare`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 620g/110.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `medieval_japanese_uchigi` - a fine $colour uchigi robe; noun: `uchigi`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 640g/150.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `medieval_japanese_kariginu` - a fine $colour kariginu robe; noun: `kariginu`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 620g/150.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `medieval_japanese_eboshi` - a black eboshi cap; noun: `eboshi`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 80g/44.0m; wear: `Wear_Hat`; variables: none.
- `medieval_japanese_hemp_sandals` - a pair of hemp sandals; noun: `sandals`; material: `hemp`; size/quality: `Small`/`Standard`; weight/cost: 180g/6.0m; wear: `Wear_Sandals`; variables: none.
- `medieval_japanese_headcloth` - a $colour linen headcloth; noun: `headcloth`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 90g/6.0m; wear: `Wear_Kerchief`; variables: Variable_BasicColour.

### Cold-weather additions/substitutions by inspiration family

The following entries are seasonal overlays rather than default manifest requirements. `matches:` lists the builder-facing outfit manifests that can use the item as an additional winter layer or as a substitution for lighter outerwear, footwear, headwear, or accessories. `All [family] outfits` means the common male, elite male, common female, and elite female manifests for that inspiration family. For warm-climate families, these entries represent cool-season, upland, rainy-season, desert-night, or winter-travel substitutions rather than everyday heavy winter dress.

#### Shared northern and western winter layers

- `medieval_wool_over_tunic` - a heavy $colour wool overtunic; noun: `overtunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 760g/24.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour; matches: all Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Norman, Anglo-Norman, High English / British, Irish / Gaelic, Scottish / Gaelic-Lowland, Carolingian / Frankish, Capetian French, Holy Roman Empire / German, Christian Iberian, Byzantine, Rus / Novgorod, Magyar, and Steppe Turkic / Mongol outfits as a winter body-layer substitution.
- `medieval_lined_wool_hose` - a pair of thick $colour wool hose; noun: `hose`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 300g/16.0m; wear: `Wear_Stockings`; variables: Variable_BasicColour; matches: all Norman, Anglo-Norman, High English / British, Scottish / Gaelic-Lowland, Carolingian / Frankish, Capetian French, Holy Roman Empire / German, Christian Iberian, Byzantine, Rus / Novgorod, Magyar, and Steppe Turkic / Mongol outfits where hose, boots, shoes, or winter travel layers are appropriate.
- `medieval_wool_footwraps` - a pair of thick $colour wool footwraps; noun: `footwraps`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 160g/6.0m; wear: `Wear_Socks`; variables: Variable_BasicColour; matches: all Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Irish / Gaelic, Scottish / Gaelic-Lowland, Carolingian / Frankish, Rus / Novgorod, Magyar, and Steppe Turkic / Mongol common outfits with shoes, boots, onuchi, or leg-wrap layers.
- `medieval_wool_mittens` - a pair of $colour wool mittens; noun: `mittens`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 120g/8.0m; wear: `Wear_Mittens`; variables: Variable_BasicColour; matches: all northern and high-medieval European common outfits, plus Rus / Novgorod, Magyar, Steppe Turkic / Mongol, Song China, Goryeo Korea, and Heian / Kamakura Japan common winter-travel outfits.
- `medieval_leather_winter_mittens` - a pair of fur-lined leather mittens; noun: `mittens`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 180g/34.0m; wear: `Wear_Mittens`; variables: none; matches: elite Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Norman, Anglo-Norman, High English / British, Scottish / Gaelic-Lowland, Carolingian / Frankish, Capetian French, Holy Roman Empire / German, Rus / Novgorod, Magyar, and Steppe Turkic / Mongol outfits.
- `medieval_lined_leather_boots` - a pair of lined leather boots; noun: `boots`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 980g/56.0m; wear: `Wear_Boots`; variables: none; matches: all northern, western, Byzantine, Rus / Novgorod, Magyar, Steppe Turkic / Mongol, and North Indian / Rajput winter-travel outfits that would substitute boots for shoes, sandals, or slippers.
- `medieval_heavy_hooded_wool_cloak` - a heavy $colour hooded wool cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1650g/46.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_BasicColour; matches: all Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Norman, Anglo-Norman, High English / British, Irish / Gaelic, Scottish / Gaelic-Lowland, Carolingian / Frankish, Capetian French, Holy Roman Empire / German, Christian Iberian, Byzantine, Rus / Novgorod, and Magyar common outfits as an outdoor winter cloak.
- `medieval_fur_faced_winter_cloak` - a fine $colour fur-faced cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1950g/170.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_FineColour; matches: elite Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Norman, Anglo-Norman, High English / British, Irish / Gaelic, Scottish / Gaelic-Lowland, Carolingian / Frankish, Capetian French, Holy Roman Empire / German, Christian Iberian, Byzantine, Rus / Novgorod, and Magyar outfits as a high-status winter cloak.
- `medieval_waxed_skin_cape` - a waxed skin cape; noun: `cape`; material: `animal skin`; size/quality: `Normal`/`Standard`; weight/cost: 1100g/32.0m; wear: `Wear_Cape`; variables: none; matches: Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Irish / Gaelic, Scottish / Gaelic-Lowland, Rus / Novgorod, Magyar, and Steppe Turkic / Mongol common travel outfits as a wet-weather or harsh-weather outside layer.

#### Insular, North Sea, and Gaelic winter overlays

- `medieval_heavy_checked_wool_mantle` - a heavy $colour1 and $colour2 checked mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1250g/42.0m; wear: `Wear_Mantle`; variables: Variable_2BasicColour; matches: all Anglo-Danish, Norse / Viking Age, Irish / Gaelic, and Scottish / Gaelic-Lowland outfits as a winter substitute for a lighter checked mantle or ordinary brat.
- `medieval_heavy_wool_brat` - a heavy $colour wool brat; noun: `brat`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1400g/42.0m; wear: `Wear_Cloak_(Open)`; variables: Variable_BasicColour; matches: all Irish / Gaelic and Scottish / Gaelic-Lowland outfits as a colder-weather substitute for `medieval_wool_brat` or a simple mantle.
- `medieval_fur_trimmed_brat` - a fine $colour fur-trimmed brat; noun: `brat`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1500g/120.0m; wear: `Wear_Cloak_(Open)`; variables: Variable_FineColour; matches: elite Irish / Gaelic and Scottish / Gaelic-Lowland outfits as a high-status winter brat.

#### Western, central, Iberian, and Byzantine winter overlays

- `medieval_wool_cowl_hood` - a $colour wool cowl hood; noun: `hood`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 360g/20.0m; wear: `Wear_Hoodie`; variables: Variable_BasicColour; matches: all Anglo-Norman, High English / British, Scottish / Gaelic-Lowland, Capetian French, Holy Roman Empire / German, Christian Iberian, Byzantine, and Seljuk / Ayyubid-Mamluk common winter-travel outfits.
- `medieval_heavy_wool_surcoat` - a heavy $colour wool surcoat; noun: `surcoat`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 760g/32.0m; wear: `Wear_Sleeveless_Tunic`; variables: Variable_BasicColour; matches: all Anglo-Norman, High English / British, Scottish / Gaelic-Lowland, Capetian French, Holy Roman Empire / German, and Christian Iberian outfits as an extra sleeveless wool layer over a cotte, kirtle, saya, or gown.
- `medieval_lined_wool_manto` - a lined $colour wool manto; noun: `manto`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1100g/42.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour; matches: all Christian Iberian outfits as a cold-weather substitute for `medieval_iberian_manto`, and Andalusian elite travel outfits where local skins want a western Iberian-facing cloak.
- `medieval_wool_paenula` - a hooded $colour wool paenula; noun: `paenula`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1250g/38.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_BasicColour; matches: all Byzantine common outfits and practical Byzantine travel variants as a weather-protective hooded cloak.
- `medieval_heavy_sagion` - a heavy $colour sagion cloak; noun: `sagion`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1150g/72.0m; wear: `Wear_Cloak_(Open)`; variables: Variable_FineColour; matches: all Byzantine elite outfits and military-facing Byzantine skins as a colder-weather substitute for `medieval_sagion`.

#### Islamic Mediterranean and Near Eastern cool-season overlays

- `medieval_heavy_wool_burnous` - a heavy $colour wool burnous; noun: `burnous`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1300g/42.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_BasicColour; matches: all Andalusian, Fatimid, Abbasid, and Seljuk / Ayyubid-Mamluk common outfits as a cool-season, mountain, desert-night, or winter-travel cloak.
- `medieval_padded_wool_jubba` - a padded $colour wool jubba; noun: `jubba`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1250g/58.0m; wear: `Wear_Robe`; variables: Variable_BasicColour; matches: all Abbasid, Fatimid, Andalusian, and Seljuk / Ayyubid-Mamluk outfits as a warmer substitute for an ordinary `medieval_wool_jubba` or light robe.
- `medieval_wool_izar_wrap` - a thick $colour wool izar; noun: `izar`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 780g/28.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour; matches: all Abbasid, Fatimid, Andalusian, and Seljuk / Ayyubid-Mamluk common outfits as an extra wrapped outer layer over qamis, sirwal, or jubba.

#### Steppe, Magyar, and Rus winter overlays

- `medieval_fur_lined_caftan` - a fine $colour fur-lined caftan; noun: `caftan`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1400g/150.0m; wear: `Wear_Long_Coat`; variables: Variable_FineColour; matches: elite Seljuk / Ayyubid-Mamluk, Magyar, Rus / Novgorod, and Steppe Turkic / Mongol outfits as a status winter riding coat.
- `medieval_felt_winter_deel` - a $colour felt-lined steppe robe; noun: `robe`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1350g/56.0m; wear: `Wear_Robe`; variables: Variable_BasicColour; matches: all Steppe Turkic / Mongol outfits and Magyar riding-facing skins as a colder-weather substitute for `medieval_wool_deel` or `medieval_steppe_riding_caftan`.
- `medieval_quilted_winter_trousers` - a pair of quilted $colour winter trousers; noun: `trousers`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 650g/30.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour; matches: all Magyar, Rus / Novgorod, and Steppe Turkic / Mongol outfits as a winter substitute for ordinary trousers or porty.
- `medieval_fur_earflap_hat` - a fur earflap hat; noun: `hat`; material: `fur`; size/quality: `Small`/`Good`; weight/cost: 260g/50.0m; wear: `Wear_Hat`; variables: none; matches: elite and severe-weather Rus / Novgorod, Magyar, Steppe Turkic / Mongol, Song China, and Goryeo Korea outfits as a cold-weather headwear substitution.
- `medieval_felt_stockings` - a pair of thick felt stockings; noun: `stockings`; material: `felt`; size/quality: `Small`/`Standard`; weight/cost: 220g/12.0m; wear: `Wear_Stockings`; variables: none; matches: all Steppe Turkic / Mongol, Magyar, and Rus / Novgorod common winter outfits, especially with felt boots, high boots, or lined leather boots.

#### Indian cool-season and upland overlays

- `medieval_wool_shawl` - a thick $colour wool shawl; noun: `shawl`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 500g/22.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour; matches: North Indian / Rajput common and elite outfits, especially winter, upland, north-western, or night-travel variants; usable for South Indian / Chola upland travel skins only when the local setting supports wool.
- `medieval_thick_cotton_shawl` - a thick $colour cotton shawl; noun: `shawl`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 420g/14.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour; matches: all North Indian / Rajput and South Indian / Chola outfits as a modest cool-season or rainy-season shoulder wrap.
- `medieval_fine_goat_hair_shawl` - a fine $colour goat-hair shawl; noun: `shawl`; material: `cashmere`; size/quality: `Small`/`Good`; weight/cost: 320g/120.0m; wear: `Wear_Mantle`; variables: Variable_FineColour; matches: elite North Indian / Rajput outfits, and highland or courtly South Indian / Chola skins only when the local setting supports imported northern or Himalayan luxury textiles.
- `medieval_heavy_cotton_angavastram` - a thick $colour cotton angavastram; noun: `angavastram`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 320g/14.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour; matches: all South Indian / Chola outfits as a cool-night, monsoon, or temple-courtyard substitute for `medieval_south_indian_angavastram`.

#### East Asian winter layering overlays

- `medieval_padded_silk_overrobe` - a padded $colour silk over-robe; noun: `over-robe`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 820g/180.0m; wear: `Wear_Robe`; variables: Variable_FineColour; matches: elite Song China, Goryeo Korea, and Heian / Kamakura Japan outfits as a winter over-robe or court-travel layer.
- `medieval_heavy_beizi_overrobe` - a heavy $colour beizi over-robe; noun: `beizi`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 760g/130.0m; wear: `Wear_Robe`; variables: Variable_FineColour; matches: all Song China female outfits and Song China elite male robe skins as a colder-weather substitute for `medieval_song_beizi`.
- `medieval_padded_po_robe` - a padded $colour po robe; noun: `po`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 820g/135.0m; wear: `Wear_Robe`; variables: Variable_FineColour; matches: all Goryeo Korea outfits as a warmer substitute for `medieval_goryeo_po_robe` or `medieval_goryeo_silk_overrobe`.
- `medieval_lined_kosode` - a lined $colour kosode robe; noun: `kosode`; material: `silk`; size/quality: `Normal`/`Standard`; weight/cost: 680g/62.0m; wear: `Wear_Robe`; variables: Variable_BasicColour; matches: all Heian / Kamakura Japan common outfits as a winter substitute for `medieval_japanese_kosode`.
- `medieval_layered_uchigi_set` - a layered $colour uchigi set; noun: `uchigi`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 1100g/220.0m; wear: `Wear_Robe`; variables: Variable_FineColour; matches: Heian / Kamakura Japan elite female outfits, and elite male court skins only where a layered court robe is locally appropriate.

### Urban and professional additions/substitutions by inspiration family

These catalogue overlays distinguish town-dwellers, skilled artisans, shopkeepers, scribes, merchants, apprentices, and workshop labourers from the rural poor. They are not default manifest entries and do not imply formal uniforms, guild badges, or household liveries unless a later skin or culture-specific implementation supplies that detail. Treat them as additions or substitutions for common and middling town variants: cleaner headwear, washable aprons, protective sleeve layers, trade-friendly belts or sashes, shorter work garments, and modestly better shop clothing.

#### Shared town and workshop basics

- `medieval_linen_work_apron` - a $colour linen work apron; noun: `apron`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 180g/8.0m; wear: `Wear_Apron`; variables: Variable_BasicColour; matches: all common and middling town-worker outfits as a clean protective layer over ordinary working garments.
- `medieval_hemp_work_apron` - a coarse hemp work apron; noun: `apron`; material: `hemp`; size/quality: `Small`/`Standard`; weight/cost: 260g/6.0m; wear: `Wear_Apron`; variables: none; matches: all common workshop outfits where a rough protective apron is plausible, especially apprentices, porters, cooks, dyers, potters, and stable workers.
- `medieval_leather_smith_apron` - a leather smith's apron; noun: `apron`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 650g/28.0m; wear: `Wear_Apron`; variables: none; matches: all urban smith, metalworker, saddler, leatherworker, and heavy craft variants across Europe, Byzantium, Rus / Novgorod, Magyar, Steppe Turkic / Mongol, Islamic, Indian, and East Asian towns.
- `medieval_linen_sleeve_guards` - a pair of $colour linen sleeve guards; noun: `sleeve guards`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 80g/4.0m; wear: `Wear_Bracers`; variables: Variable_BasicColour; matches: all scribal, dyeing, cooking, weaving, and workshop variants where sleeves are tied or covered for cleanliness.
- `medieval_leather_work_gloves` - a pair of leather work gloves; noun: `gloves`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 160g/12.0m; wear: `Wear_Gloves`; variables: none; matches: all smith, mason, carter, tanner, saddler, porter, and heavier workshop variants where hand protection is useful.
- `medieval_clean_linen_coif` - a clean $colour linen coif; noun: `coif`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 85g/10.0m; wear: `Wear_Coif`; variables: Variable_BasicColour; matches: western, central European, Norman, Anglo-Norman, High English / British, Capetian French, Holy Roman Empire / German, and Christian Iberian town-worker and clerk variants as cleaner urban headwear.
- `medieval_linen_work_cap` - a $colour linen work cap; noun: `cap`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 95g/6.0m; wear: `Wear_Hat`; variables: Variable_BasicColour; matches: all common town-worker outfits that need a simple washable cap rather than a felt cap, veil, turban, or hood.
- `medieval_wide_work_sash` - a wide $colour work sash; noun: `sash`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 140g/8.0m; wear: `Wear_Sash`; variables: Variable_BasicColour; matches: all town artisans, porters, stallholders, and market workers as a practical substitute for a fine girdle or narrow belt.
- `medieval_canvas_work_tabard` - a $colour canvas work tabard; noun: `tabard`; material: `canvas`; size/quality: `Normal`/`Standard`; weight/cost: 360g/14.0m; wear: `Wear_Tabard`; variables: Variable_BasicColour; matches: western and central European, Byzantine, Rus / Novgorod, and workshop-facing town variants as a sleeveless over-layer protecting the main garment.
- `medieval_drab_dyers_apron` - a stained $colour dye-worker's apron; noun: `apron`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 230g/7.0m; wear: `Wear_Apron`; variables: Variable_DrabColour; matches: all urban dye-house, fulling, tanning, pigment-grinding, and textile-finishing variants where deliberate staining is part of the professional presentation.

#### Insular, North Sea, Gaelic, and early Frankish town overlays

- `medieval_short_wool_work_tunic` - a short $colour wool work tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 440g/14.0m; wear: `Wear_Tunic`; variables: Variable_BasicColour; matches: Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Carolingian / Frankish, Irish / Gaelic, Scottish / Gaelic-Lowland, Rus / Novgorod, and Magyar male town-worker variants as a more mobile alternative to a long tunic.
- `medieval_clean_town_wool_tunic` - a neat $colour town tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 520g/30.0m; wear: `Wear_Tunic`; variables: Variable_FineColour; matches: Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Carolingian / Frankish, Norman, Irish / Gaelic, and Scottish / Gaelic-Lowland skilled artisan or shopkeeper variants.
- `medieval_tucked_wool_work_gown` - a tucked $colour work gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 700g/24.0m; wear: `Wear_Gown`; variables: Variable_BasicColour; matches: Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Norman, Irish / Gaelic, and Scottish / Gaelic-Lowland female town-worker variants as a cleaner, more mobile substitute for a straight gown.
- `medieval_linen_market_veil` - a plain $colour market veil; noun: `veil`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 110g/8.0m; wear: `Wear_Veil`; variables: Variable_BasicColour; matches: Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Norman, Irish / Gaelic, Scottish / Gaelic-Lowland, Carolingian / Frankish, Capetian French, Holy Roman Empire / German, and Christian Iberian female town-market variants.
- `medieval_brooched_market_mantle` - a neat $colour brooched mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 860g/48.0m; wear: `Wear_Mantle`; variables: Variable_FineColour; matches: Anglo-Danish, Norse / Viking Age, Early Anglo-Saxon, Carolingian / Frankish, Irish / Gaelic, Scottish / Gaelic-Lowland, and Rus / Novgorod prosperous town-dweller variants.
- `medieval_leather_trader_belt` - a broad leather trader's belt; noun: `belt`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 260g/34.0m; wear: `Wear_Waist`; variables: none; matches: Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Carolingian / Frankish, Norman, Irish / Gaelic, Scottish / Gaelic-Lowland, Rus / Novgorod, Magyar, and Steppe Turkic / Mongol town-trader variants as a stronger substitute for a plain belt.
- `medieval_wool_market_headcloth` - a $colour wool market headcloth; noun: `headcloth`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 130g/8.0m; wear: `Wear_Kerchief`; variables: Variable_BasicColour; matches: Insular, North Sea, Gaelic, Rus / Novgorod, Magyar, and steppe town-worker variants where a hood is too bulky for indoor work.
- `medieval_town_leine` - a clean $colour town léine; noun: `léine`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 520g/34.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_FineColour; matches: Irish / Gaelic and Scottish / Gaelic-Lowland town-dweller, skilled craft, and market variants as a cleaner substitute for a rough common léine.

#### Western and central European urban craft and trade overlays

- `medieval_work_wool_cotte` - a plain $colour wool work cotte; noun: `cotte`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 540g/22.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour; matches: Norman, Anglo-Norman, High English / British, Capetian French, Holy Roman Empire / German, Christian Iberian, and Scottish / Gaelic-Lowland male town-worker variants.
- `medieval_town_work_kirtle` - a tucked $colour work kirtle; noun: `kirtle`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 650g/26.0m; wear: `Wear_Dress`; variables: Variable_BasicColour; matches: Anglo-Norman, High English / British, Capetian French, Holy Roman Empire / German, Christian Iberian, and Scottish / Gaelic-Lowland female town-worker variants.
- `medieval_sleeveless_work_surcoat` - a plain $colour sleeveless work surcoat; noun: `surcoat`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 560g/20.0m; wear: `Wear_Sleeveless_Tunic`; variables: Variable_BasicColour; matches: Anglo-Norman, High English / British, Capetian French, Holy Roman Empire / German, Christian Iberian, and Scottish / Gaelic-Lowland apprentices, servants, shop workers, and workshop assistants.
- `medieval_plain_wool_shop_gown` - a neat $colour wool shop gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 760g/52.0m; wear: `Wear_Gown`; variables: Variable_FineColour; matches: High English / British, Capetian French, Holy Roman Empire / German, Anglo-Norman, and Christian Iberian prosperous shopkeeper or master-craft variants without becoming court elite clothing.
- `medieval_clerk_wool_gown` - a plain $colour clerk's gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 780g/56.0m; wear: `Wear_Gown`; variables: Variable_FineColour; matches: Anglo-Norman, High English / British, Capetian French, Holy Roman Empire / German, Christian Iberian, Byzantine, and urban Islamic scribal or administrative variants as a literate professional overlay.
- `medieval_neat_wool_town_hood` - a neat $colour wool town hood; noun: `hood`; material: `wool`; size/quality: `Small`/`Good`; weight/cost: 240g/24.0m; wear: `Wear_Hoodie`; variables: Variable_FineColour; matches: Norman, Anglo-Norman, High English / British, Capetian French, Holy Roman Empire / German, Christian Iberian, and Scottish / Gaelic-Lowland town-dweller variants as a cleaner substitute for a plain hood.
- `medieval_merchants_wool_hood` - a fine $colour merchant's hood; noun: `hood`; material: `wool`; size/quality: `Small`/`Good`; weight/cost: 260g/44.0m; wear: `Wear_Hoodie`; variables: Variable_FineColour; matches: High English / British, Capetian French, Holy Roman Empire / German, Anglo-Norman, Christian Iberian, and prosperous Scottish / Gaelic-Lowland urban merchant variants.
- `medieval_linen_shop_wimple` - a clean $colour shop wimple; noun: `wimple`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 150g/14.0m; wear: `Wear_Veil`; variables: Variable_BasicColour; matches: Anglo-Norman, High English / British, Capetian French, Holy Roman Empire / German, Christian Iberian, and Scottish / Gaelic-Lowland female shopkeeper, alewife, and craftswoman variants.
- `medieval_linen_baker_cap` - a $colour linen baker's cap; noun: `cap`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 100g/7.0m; wear: `Wear_Hat`; variables: Variable_BasicColour; matches: all western and central European baker, cook, brewer, miller, and food-stall variants; also usable for Byzantine and Islamic urban food trades when local skins prefer a cap over a veil or turban.

#### Iberian, Byzantine, and Mediterranean urban overlays

- `medieval_light_linen_work_camisa` - a light $colour linen work camisa; noun: `camisa`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 300g/10.0m; wear: `Wear_Shirt`; variables: Variable_BasicColour; matches: Christian Iberian, Andalusian, and warm-climate Mediterranean town-worker variants as a lighter substitute for a heavier underlayer.
- `medieval_cotton_market_toca` - a $colour cotton market toca; noun: `toca`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 120g/9.0m; wear: `Wear_Veil`; variables: Variable_BasicColour; matches: Christian Iberian and Andalusian female town-market variants, especially vendors, shopkeepers, and household workers.
- `medieval_wool_work_pellote` - a plain $colour wool work pellote; noun: `pellote`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 620g/26.0m; wear: `Wear_Sleeveless_Gown`; variables: Variable_BasicColour; matches: Christian Iberian urban craft, shop, and servant variants as a plainer substitute for a fine pellote.
- `medieval_plain_work_dalmatic` - a plain $colour work dalmatic; noun: `dalmatic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 740g/32.0m; wear: `Wear_Robe`; variables: Variable_BasicColour; matches: Byzantine common and middling town-worker variants as a practical robe layer below elite decorated dalmatics.
- `medieval_short_work_sagion` - a short $colour work sagion; noun: `sagion`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 720g/26.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour; matches: Byzantine, Christian Iberian, Norman, and Mediterranean town-worker or messenger variants as a short outdoor mantle.
- `medieval_linen_shop_maforion` - a plain $colour shop maforion; noun: `maforion`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 130g/10.0m; wear: `Wear_Veil`; variables: Variable_BasicColour; matches: Byzantine female town-dweller, shopkeeper, market, and workshop variants as a practical substitute for a fine maforion.

#### Islamic Mediterranean and Near Eastern urban overlays

- `medieval_short_cotton_work_qamis` - a short $colour cotton work qamis; noun: `qamis`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 360g/14.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour; matches: Andalusian, Abbasid, Fatimid, and Seljuk / Ayyubid-Mamluk artisans, porters, cooks, dyers, sailors, and market workers as a mobile substitute for a long qamis.
- `medieval_linen_work_sirwal` - a pair of $colour linen work sirwal; noun: `sirwal`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 330g/12.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour; matches: Andalusian, Abbasid, Fatimid, and Seljuk / Ayyubid-Mamluk town-worker variants where washable linen trousers fit hot work better than heavier cotton or silk sirwal.
- `medieval_linen_shop_turban` - a clean $colour linen shop turban; noun: `turban`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 170g/12.0m; wear: `Wear_Turban`; variables: Variable_BasicColour; matches: Andalusian, Abbasid, Fatimid, and Seljuk / Ayyubid-Mamluk shopkeeper, scribe, merchant, and urban artisan variants as cleaner working headwear.
- `medieval_cotton_waist_apron` - a $colour cotton waist apron; noun: `apron`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 170g/7.0m; wear: `Wear_Apron`; variables: Variable_BasicColour; matches: Andalusian, Abbasid, Fatimid, Seljuk / Ayyubid-Mamluk, North Indian / Rajput, South Indian / Chola, Song China, and Goryeo Korea warm-climate or textile-work variants.
- `medieval_sleeveless_work_jubba` - a sleeveless $colour work jubba; noun: `jubba`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 650g/30.0m; wear: `Wear_Sleeveless_Tunic`; variables: Variable_BasicColour; matches: Andalusian, Abbasid, Fatimid, and Seljuk / Ayyubid-Mamluk workshop, market, and travelling-trade variants as a plainer substitute for a robe or heavy jubba.
- `medieval_bazaar_merchant_jubba` - a neat $colour bazaar jubba; noun: `jubba`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 820g/64.0m; wear: `Wear_Robe`; variables: Variable_FineColour; matches: Andalusian, Abbasid, Fatimid, and Seljuk / Ayyubid-Mamluk prosperous shopkeeper and merchant variants without using courtly tiraz or robe-of-honour language.
- `medieval_scribe_linen_sleeves` - a pair of $colour linen scribe sleeves; noun: `sleeves`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 70g/5.0m; wear: `Wear_Bracers`; variables: Variable_BasicColour; matches: Abbasid, Fatimid, Andalusian, Seljuk / Ayyubid-Mamluk, Byzantine, Song China, Goryeo Korea, and western clerk variants where a literate professional protects sleeves during ink work.
- `medieval_market_izar_wrap` - a $colour market izar wrap; noun: `izar`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 300g/10.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour; matches: Andalusian, Abbasid, Fatimid, and Seljuk / Ayyubid-Mamluk market, porter, and light-labour variants as an extra wrap or work-cloth substitute.

#### Steppe, Magyar, and Rus town-trade overlays

- `medieval_short_work_caftan` - a short $colour work caftan; noun: `caftan`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 720g/30.0m; wear: `Wear_Long_Coat`; variables: Variable_BasicColour; matches: Magyar, Rus / Novgorod, Steppe Turkic / Mongol, and Seljuk / Ayyubid-Mamluk urban craft, caravan, stable, and market variants as a shorter working coat.
- `medieval_rus_town_rubakha` - a neat $colour town rubakha; noun: `rubakha`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 430g/30.0m; wear: `Wear_Shirt`; variables: Variable_FineColour; matches: Rus / Novgorod skilled artisan, trader, shopkeeper, and town-dweller variants as a cleaner substitute for a plain rubakha.
- `medieval_tucked_poneva_work_skirt` - a tucked $colour poneva work skirt; noun: `poneva`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 520g/20.0m; wear: `Wear_Long_Skirt`; variables: Variable_BasicColour; matches: Rus / Novgorod female town-worker, market, dairy, textile, and household craft variants as a more mobile substitute for an ordinary poneva.
- `medieval_furriers_leather_apron` - a leather furrier's apron; noun: `apron`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 520g/24.0m; wear: `Wear_Apron`; variables: none; matches: Rus / Novgorod, Norse / Viking Age, Anglo-Danish, Magyar, and Steppe Turkic / Mongol furrier, leatherworker, saddler, and winter-goods workshop variants.
- `medieval_felt_market_cap` - a neat $colour felt market cap; noun: `cap`; material: `felt`; size/quality: `Small`/`Standard`; weight/cost: 140g/10.0m; wear: `Wear_Hat`; variables: Variable_BasicColour; matches: Magyar, Rus / Novgorod, Steppe Turkic / Mongol, Seljuk / Ayyubid-Mamluk, and Goryeo Korea town-market variants as a simple urban substitute for a riding cap or fur hat.
- `medieval_wide_leather_work_belt` - a wide leather work belt; noun: `belt`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 300g/22.0m; wear: `Wear_Waist`; variables: none; matches: Magyar, Rus / Novgorod, Steppe Turkic / Mongol, Seljuk / Ayyubid-Mamluk, and northern European town-trade variants where a stronger belt distinguishes working dress from field clothing.
- `medieval_mounted_trader_coat` - a neat $colour mounted trader's coat; noun: `coat`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 900g/70.0m; wear: `Wear_Long_Coat`; variables: Variable_FineColour; matches: Magyar, Steppe Turkic / Mongol, Seljuk / Ayyubid-Mamluk, and Rus / Novgorod prosperous trader or caravan-market variants without becoming an elite court caftan.

#### Indian urban craft and market overlays

- `medieval_short_cotton_work_dhoti` - a short $colour cotton work dhoti; noun: `dhoti`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 220g/8.0m; wear: `Wear_Loincloth`; variables: Variable_BasicColour; matches: North Indian / Rajput artisans, labourers, dyers, potters, grooms, and market workers as a shorter working substitute for a fuller dhoti.
- `medieval_cotton_work_waistcloth` - a $colour cotton workshop waistcloth; noun: `waistcloth`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 190g/7.0m; wear: `Wear_Apron`; variables: Variable_BasicColour; matches: North Indian / Rajput and South Indian / Chola textile, kitchen, market-stall, and workshop variants as a practical cloth tied over the lower garment.
- `medieval_weavers_cotton_shouldercloth` - a $colour weaver's shouldercloth; noun: `shouldercloth`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 190g/9.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour; matches: North Indian / Rajput and South Indian / Chola weaver, dyer, merchant, and workshop variants as a narrow substitute for a larger uttariya or angavastram.
- `medieval_market_cotton_turban` - a clean $colour market turban; noun: `turban`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 190g/12.0m; wear: `Wear_Turban`; variables: Variable_BasicColour; matches: North Indian / Rajput and South Indian / Chola town merchant, artisan, guard, and market variants as cleaner working headwear.
- `medieval_trade_bordered_uttariya` - a $colour trade-bordered uttariya; noun: `uttariya`; material: `cotton`; size/quality: `Small`/`Good`; weight/cost: 230g/30.0m; wear: `Wear_Mantle`; variables: Variable_FineColour; matches: North Indian / Rajput prosperous artisan, merchant, scribe, and temple-market variants as a middling urban substitute for a plain cotton uttariya.
- `medieval_south_indian_work_veshti` - a tucked $colour cotton work veshti; noun: `veshti`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 250g/9.0m; wear: `Wear_Loincloth`; variables: Variable_BasicColour; matches: South Indian / Chola artisans, market workers, boatmen, dyers, and temple-service variants as a shorter or tucked working substitute for a fuller veshti.
- `medieval_cotton_bazaar_sari` - a tucked $colour cotton bazaar sari; noun: `sari`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 480g/18.0m; wear: `Wear_Robe`; variables: Variable_BasicColour; matches: North Indian / Rajput and South Indian / Chola female town-market, stallholder, textile-worker, and household craft variants as a more work-ready substitute for a loose full drape.

#### East Asian urban craft and market overlays

- `medieval_song_work_shan_jacket` - a plain $colour work shan jacket; noun: `jacket`; material: `ramie`; size/quality: `Normal`/`Standard`; weight/cost: 330g/18.0m; wear: `Wear_Jacket`; variables: Variable_BasicColour; matches: Song China town-worker, servant, shop assistant, textile-worker, and market variants as a practical short jacket over trousers or skirt.
- `medieval_song_short_work_trousers` - a pair of short $colour work trousers; noun: `trousers`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 300g/12.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour; matches: Song China artisans, porters, boatmen, cooks, and market workers as a mobile substitute for longer robe-based dress.
- `medieval_song_shopkeeper_robe` - a neat $colour shopkeeper's robe; noun: `robe`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 560g/76.0m; wear: `Wear_Robe`; variables: Variable_FineColour; matches: Song China prosperous shopkeeper, clerk, teacher, and merchant variants as a middling urban substitute for official or court robes.
- `medieval_song_merchant_headscarf` - a neat $colour merchant headscarf; noun: `headscarf`; material: `cotton`; size/quality: `Small`/`Good`; weight/cost: 90g/10.0m; wear: `Wear_Kerchief`; variables: Variable_FineColour; matches: Song China shopkeeper, merchant, clerk, and educated town-dweller variants as cleaner headwear than a plain work headscarf.
- `medieval_song_cotton_work_apron` - a $colour cotton work apron; noun: `apron`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 160g/7.0m; wear: `Wear_Apron`; variables: Variable_BasicColour; matches: Song China cooks, dyers, textile-workers, tea-house workers, stallholders, and workshop variants.
- `medieval_goryeo_work_jeogori` - a plain $colour work jeogori; noun: `jeogori`; material: `ramie`; size/quality: `Normal`/`Standard`; weight/cost: 340g/18.0m; wear: `Wear_Jacket`; variables: Variable_BasicColour; matches: Goryeo Korea common and middling town-worker, textile, market, and workshop variants as a plain working jacket.
- `medieval_goryeo_work_baji` - a pair of $colour work baji; noun: `baji`; material: `ramie`; size/quality: `Normal`/`Standard`; weight/cost: 340g/14.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour; matches: Goryeo Korea town-worker, artisan, porter, messenger, and market variants as practical trousers below a jeogori or po.
- `medieval_goryeo_market_chima` - a plain $colour market chima; noun: `chima`; material: `ramie`; size/quality: `Normal`/`Standard`; weight/cost: 430g/20.0m; wear: `Wear_Long_Skirt`; variables: Variable_BasicColour; matches: Goryeo Korea female town-market, craft, shop, and household-worker variants as a plainer substitute for a silk chima.
- `medieval_goryeo_shop_po` - a neat $colour shop po; noun: `po`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 580g/82.0m; wear: `Wear_Robe`; variables: Variable_FineColour; matches: Goryeo Korea prosperous shopkeeper, clerk, merchant, and town elder variants as a middling substitute for a fine court over-robe.
- `medieval_japanese_work_kosode` - a plain $colour work kosode; noun: `kosode`; material: `ramie`; size/quality: `Normal`/`Standard`; weight/cost: 480g/22.0m; wear: `Wear_Robe`; variables: Variable_BasicColour; matches: Heian / Kamakura Japan common town-worker, servant, workshop, and market variants as a plainer, hard-wearing substitute for a silk kosode.
- `medieval_japanese_work_hakama` - a pair of $colour work hakama; noun: `hakama`; material: `hemp`; size/quality: `Normal`/`Standard`; weight/cost: 380g/18.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour; matches: Heian / Kamakura Japan male and female town-worker, messenger, servant, and practical household variants below a kosode.
- `medieval_japanese_hemp_work_apron` - a hemp work apron; noun: `apron`; material: `hemp`; size/quality: `Small`/`Standard`; weight/cost: 170g/6.0m; wear: `Wear_Apron`; variables: none; matches: Heian / Kamakura Japan kitchen, workshop, dyeing, stable, market, and household craft variants as a rough protective layer.
- `medieval_japanese_yumaki_wrap` - a $colour yumaki waist wrap; noun: `yumaki`; material: `hemp`; size/quality: `Small`/`Standard`; weight/cost: 190g/8.0m; wear: `Wear_Apron`; variables: Variable_BasicColour; matches: Heian / Kamakura Japan female and household-service town-worker variants as an apron-like waist wrap over a work kosode.

## Validation target

- The target output contains no hidden or non-skinnable items.
- All target items use `null` for long description, morph target, morph emote, morph timer, and destroyed-item reference.
- The item catalogue uses exact seeded solid material names for primary materials.
- The item catalogue uses exact seeded wearable and variable component names where those components are listed.
- Public text avoids explicit skin/customization language and avoids culture labels in item names, short descriptions, and full descriptions.
- Outfit manifests reference only item catalogue entries in this document.
- Cold-weather additions are catalogue overlays with `matches:` notes; they are not mandatory default manifest entries unless a later implementation intentionally promotes them into a manifest.
- Urban and professional additions are catalogue overlays with `matches:` notes; they are not mandatory default manifest entries and should not imply formal uniforms, badges, hidden storage, or guild mechanics unless later supported by components or skins.
- Urban and professional additions distinguish town workers from rural commoners through cleaner headwear, work aprons, sleeve guards, work caps, short tunics or jackets, merchant and clerk robes, and culture-specific workshop garments.

## Source notes

- Project source: `FutureMUD Antiquity Clothing Seeder Design Reference` - used as the structural model for executive summary, scope, project rules, design assumptions, outfit manifests, item catalogue format, validation target, and colour-variable appendix.
- Project source: `Item Authoring Guidelines.md` and `FutureMUD Item Seeder Working Guidelines.md` - used for item field rules, material/component grounding, cost units, skinnability, and ordinary wearable implementation assumptions.
- University of Glasgow, Old English Teaching, `Unit 5 Clothing` - used for Anglo-Saxon evidence cautions, tunic/trouser/leg-wrap/gown/cloak/veil grounding, and wool/linen/leather status assumptions.
- National Museum of Denmark, `The clothes and jewellery of the Vikings` - used for Viking Age tunic, trouser, strap-dress, undergarment, cloak, brooch, wool/flax, fur, silk, and economic-status grounding.
- Fashion History Timeline, `Byzantine` - used for Byzantine dalmatic, chlamys/sagion-adjacent cloak, decorated tunics, campagi, tablion, and court-status textile cues.
- Metropolitan Museum of Art, `Tiraz: Inscribed Textiles from the Early Islamic Period` - used for tiraz textile status, robe-of-honour assumptions, and Abbasid/Fatimid/Islamic textile display grounding.
- Encyclopaedia.com, `Clothing and Textiles` / `Middle East: History of Islamic Dress` - used for qamis/kamis, sirwal, izar, jubba, turban, and layered Islamic clothing assumptions.
- Encyclopaedia Iranica, `CLOTHING vii. Of the Iranian Tribes on the Pontic Steppes and in the Caucasus` - used as a comparative source for steppe caftans, trousers, riding-friendly side slits, felt/leather/fur, belts, and boots.
- Victoria and Albert Museum, `Indian textiles` - used for Indian textile-material grounding, especially cotton, silk, sheep-wool, yak-hair, goat-hair, regional textile richness, dyes, weaving, and courtly fabric display.
- Kyoto Museum of Traditional Crafts / Google Arts & Culture, `The ancient history making and wearing a kimono` - used for kosode, hakama, layered Heian clothing, Kamakura utility shift, and Japanese court/common clothing assumptions.
- Encyclopaedia Britannica, `Goryeo dynasty` - used for the Goryeo chronological and cultural boundary; garment details should be refined with more specialist Korean dress sources before final fdesc writing.

- Viking Ship Museum, `Viking Age People` - used for winter-specific Viking clothing cues: wool and linen garments, fur-decorated clothing, leather rainwear, leather shoes and boots, and fabric or fur winter caps.
- History of Istanbul, `Clothing in Constantinople: 330-1453` - used for Byzantine and late Roman weather-protection garments including the paenula, sagum, khlamus, mandyas/mantion, weather-facing wool cloaks, trousers, and more substantial shoes and boots.
- World History Encyclopedia, `Clothing in the Mongol Empire` - used for steppe winter overlays: fur or felt coats over the robe, quilted or padded winter trousers, felt stockings, felt/leather boots, and fur/felt hats with ear protection.
- Victoria and Albert Museum, `Kimono` - used for Japanese winter-layering policy, especially the distinction between unlined humid-summer robes and multi-lined winter robes.

- Encyclopaedia Britannica, `Guild` - used for the urban craft and merchant context behind the professional overlay section, especially the thirteenth-century prominence of guilds, town councils, workshop organization, trade regulation, and craft standards.
- Different Visions, `Pain Quotidien: Images of Bakers in Medieval France` - used for baker and food-trade grounding, especially the shift of bakeries into an increasingly urban profession by the thirteenth-century Livre de Métiers and the use of manuscript/seal imagery to read clothing, gestures, and tools.
- China Online Museum, `Along the River During the Qingming Festival` - used for Song urban-market context, especially street scenes, rich-to-poor social breadth, economic activities, and glimpses of period clothing in a city setting.
- Encyclopaedia.com, `Clothing for Men` - used for Song urban headwear, robe, tight-sleeved gown, short over-jacket, conservative colour, and scholar/commoner clothing distinctions.
- Victoria and Albert Museum, `Hanbok - traditional Korean dress` - used for Korean garment-structure grounding: jeogori, baji, chima, po/durumagi, wrap direction, and long-lived outer-coat forms.
- Minneapolis Institute of Art, `Japanese Kimono` - used for the Heian-to-Kamakura transition from many courtly layers toward kosode-based dress, and with caution for the broader principle that garments and styles can mark activity or profession.
- Urban/professional source limitation: most catalogue items here are practical overlays, not exact uniforms. For non-European town workers especially, the design extrapolates from documented base garments, material cultures, and market/workshop iconography rather than asserting standardized occupational dress.

## Appendix A: colour variable value lists

These lists are included to preserve the design distinction between ordinary, fine, and deliberately degraded colour vocabularies.

### basic_colours

black, white, grey, light grey, dark grey, red, dark red, blue, dark blue, green, brown, dark green, orange, light blue, light green, yellow, light red, purple, pink

### fine_colours

light grey, dark grey, red, dark red, blue, dark blue, green, brown, dark green, pale white, olive, caramel, ebony, emerald green, cerulean, violet, sandy brown, light brown, dark brown, auburn, onyx, obsidian, midnight black, ink black, jet black, pitch black, ivory, seashell, snow white, gleaming white, pure white, pearl white, bright white, bone white, ghost white, mist grey, charcoal grey, thistle grey, smoky grey, slate grey, silver grey, soft grey, ash grey, crimson, scarlet, ruby red, blood red, rose red, wine red, flame red, coral, copper, fiery orange, ochre, sunset orange, amber, goldenrod, pale yellow, golden yellow, sand yellow, topaz hued, gold-coloured, spring green, sea green, hunter green, olive green, sage green, pine green, bright green, rich green, pale green, verdant green, forest green, chartreuse, slate blue, bright blue, powder blue, sapphire blue, royal blue, ocean blue, teal, cornflour blue, sky blue, azure, beryl, cobalt, rich indigo, deep indigo, vivid indigo, earthen brown, deep brown, rich brown, burnt sienna, chocolate, cinnamon, mahogany, nut brown, umber, amethyst, mauve, mulbery, plum, lavender, royal purple, orange, light blue, light green, pale blue, yellow, cyan, navy blue, reddish brown, beige

### drab_colours

faded black, tattered black, shabby black, grimy black, off-white, dingy grey, bland yellow, faded green, faded blue, faded indigo, drab brown, dim grey, dusky slate grey, sooty grey, chalky pale grey, dull mist grey, ashen off-white, dirty bone-white, wan ivory, spotted white, stained white, blotched white, dingy off-white, stained ivory, shabby sallow-coloured, lurid pale yellow, dingy yellow, gaudy mustard yellow, sickly pale yellow, shabby pale yellow, murky brown, stained brown, dreary brown, bland brown, spotted muddy brown, dismal sand brown, dreary beige, grimy beige, shabby beige, dirty beige, tattered beige, bland wheat-coloured, drab olive, murky olive, dim olive, dingy green, shabby green, dull green, sickly greyish-green, grisly brownish-green, discoloured green, blotchy green, grimy rust-red, blotchy rust-red, grimy salmon, stained salmon, blotched red, dull red, faded red, stained red, dingy red, faded salmon, well-worn blue, faded slate blue, pallid blue, stained blue, grimy blue, dim blue-black, faded blue-black, dreary blue-black, dull orange, faded reddish-orange, tattered reddish-orange, discoloured orange, stained orange-red, drab peach-coloured, lurid peach-coloured, sickly peach-coloured, tattered violet, grimy lavender, spotted lavender, discoloured purple, dirty purple, dingy purple, faded purple, stained purple, dusty faded purple

