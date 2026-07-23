# FutureMUD Medieval Clothing Seeder Design Reference

This document consolidates the medieval clothing guidance and target catalogue for the FutureMUD Item Seeder. It covers selected European, Mediterranean, Near Eastern, North African, Indian, Central Asian, and East Asian clothing families during the period roughly 500AD to 1300AD, presenting shared rules, historical assumptions, outfit manifests, and item-catalogue targets in one stable reference.

## Executive summary

- Total target unique wearable item prototypes in this reference: **408**.
- Post-implementation headwear and footwear expansion candidates in this revision: **44** — **22 headwear** and **22 footwear** rows. These are design candidates only and are not included in the implemented 408-row catalogue until matching seeder calls and full-description rows are added.
- Total outfit manifests: **164**.
- The ItemSeeder now upserts all **164** manifests as stock `OutfitTemplate` rows after the Medieval item phase, preserving document order as wear order and using each stable item reference as the template-local key.
- Each inspiration family has four builder-facing outfit manifests: common male, elite male, common female, and elite female.
- All target items are finished goods, skinnable, player-visible, and ordinary portable inventory items unless a later seeder implementation explicitly marks a narrow exception.
- Public item fields should use culture-neutral, in-world descriptions. Builder-facing notes and outfit labels may still use historical inspiration labels for organization and source grounding.
- The item set is intended as a historically grounded base layer: skins may create local, fantasy, status, textile, dye, motif, household, seasonal, and professional variants without duplicating behaviour-heavy item prototypes.
- The catalogue includes **34** cold-weather overlay items, **68** urban/professional overlay items, **79** religious-clothing overlay items, and **70** accessory/social-context overlay items; these overlays are optional additions or substitutions unless a builder-facing outfit manifest intentionally promotes them into a complete outfit. The accessory/social-context pass covers **30** worn accessories and fasteners, **14** travel/rural-specialist items, **10** household/service-labour items, **8** scholar/literate-professional items, and **8** child or apprentice items, while deliberately omitting wearable containers such as pouches, purses, alms bags, travel scrips, satchels, backpacks, and tool bags.

## Scope and era model

- Chronological band: 500AD to 1300AD.
- Geographic coverage in this consolidated document: Britain and Ireland; Scandinavia and the North Sea; western, central, and southern Europe; Iberia; Byzantium; the Levant; Egypt and North Africa; the Eurasian steppe; Rus/Novgorod; northern and southern India; China; Korea; and Japan.
- Historical inspiration families: Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Norman, Anglo-Norman, High English / British, Irish / Gaelic, Scottish / Gaelic-Lowland, Carolingian / Frankish, Capetian French, Holy Roman Empire / German, Christian Iberian, Andalusian, Byzantine, Abbasid, Fatimid, Seljuk / Ayyubid-Mamluk, Magyar, Rus / Novgorod, Steppe Turkic / Mongol, North Indian / Rajput, South Indian / Chola, Song China, Goryeo Korea, Heian / Kamakura Japan. These labels are builder-facing organizational buckets, not required public item wording.
- Resolution: standard garment prototypes rather than exhaustive local subtypes. The defaults should be credible on their own, while later skins can handle exact regional trim, textile patterns, motifs, dyes, household marks, rank marks, religious variants, monastic order variants, liturgical textile details, and world-specific names.
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


### Religious tradition coverage table

This table is builder-facing. It documents the religious clothing traditions implied by the selected culture families and keeps shared religious outfits from being duplicated as culture-specific priest, monk, nun, or scholar variants.

| Religious tradition | Covered culture families | Catalogue treatment | Notes and limits |
|---|---|---|---|
| Latin Catholic / Western Christian | Early Anglo-Saxon, Anglo-Danish, Norse after conversion, Norman, Anglo-Norman, High English / British, Irish / Gaelic, Scottish / Gaelic-Lowland, Carolingian / Frankish, Capetian French, Holy Roman Empire / German, Christian Iberian, Magyar | Shared western monastic, mendicant, clerical, liturgical, bishop, and archbishop outfits. | Order labels are builder-facing. Public item text should describe garment form, colour, and construction rather than naming a real-world order unless the garment name itself is useful and historically intelligible. |
| Eastern Orthodox / Byzantine-rite Christian | Byzantine, Rus / Novgorod, and Orthodox-facing neighbouring or frontier contexts | Shared Eastern Christian monastic, deacon, priest, and bishop outfits. | Byzantine-rite vestments are distinct from Latin vestments and should not be reskinned as western chasuble/dalmatic sets without intentional local design work. |
| Islam | Andalusian, Abbasid, Fatimid, Seljuk / Ayyubid-Mamluk, Islamic Indian and steppe contact settings | Imam/khatib, qadi/scholar, and Sufi clothing overlays using qamis, sirwal, jubba, turban, taylasan, khirqa, sash, and cap forms. | Islam is not modelled as having an ordained priesthood uniform. These are learned, mosque, judicial, and Sufi-specialist outfits rather than universal religious uniforms. |
| Judaism | Jewish communities in western Christian, Byzantine, Islamic Mediterranean, Near Eastern, North African, Iberian, and Indian Ocean urban settings | Prayer shawls, small prayer garments, skullcap, and learned robe overlays. | No forced badge, discriminatory dress, or other externally imposed status mark is included. Local everyday clothing should come from the surrounding culture family. |
| Hindu temple and ascetic traditions | North Indian / Rajput and South Indian / Chola | Temple-priest white dhoti/uttariya and ochre ascetic wrap variants. | The items are conservative clothing forms only; sect marks, sacred threads, beads, and ritual tools are left to later non-clothing passes or skins. |
| Jain Śvetāmbara | North Indian / Rajput and western/south Indian urban religious contexts | White ascetic robe and shoulder-wrap outfits for monks and nuns. | Digambara male ascetics are not represented with clothing prototypes because the distinctive practice is non-clothing. No mouth-cloth or broom is included in this clothing pass. |
| Buddhism | Song China, Goryeo Korea, Heian / Kamakura Japan, and residual Indian Buddhist contexts | Plain under-robes, patched kasaya/kesa mantles, nun robes, formal mantles, and travelling monastic layers. | Regional colour and sect nuance should be carried by skins; the base set focuses on robe and mantle construction rather than exhaustive school distinctions. |
| Daoism | Song China and neighbouring East Asian religious settings | Cross-collar robe, formal cloud-sleeved robe, and ritual cap overlays. | The base items avoid talismans, written charms, registers, or ritual implements. |
| Shinto | Heian / Kamakura Japan | Shrine-priest jōe robe, hakama, eboshi, and shrine-attendant kosode/hakama outfits. | The emphasis is on Heian-derived courtly shrine clothing. Later standardized modern shrine uniforms are outside scope. |
| Steppe animist / Tengri / shamanic specialist | Steppe Turkic / Mongol and steppe-influenced Magyar or frontier settings | Felt-lined coat, fur cap, and braided sash overlays worn over normal steppe clothing. | These are conservative clothing signals for ritual specialists, not a claim of standardized shamanic uniform. Drums, charms, masks, and other paraphernalia are intentionally excluded. |
| Poorly evidenced pre-Christian Norse, Slavic, Gaelic, and local folk-priesthood dress | Norse / Viking Age, Rus / Novgorod, Irish / Gaelic, Scottish / Gaelic-Lowland, and other local contexts | No separate base outfit. Use ordinary elite/common local garments plus later skins if a setting needs ritual colour, trim, or symbolic textile work. | This avoids inventing confident uniform systems where the evidence is thin or where religious clothing is not clearly separable from local elite or household dress. |

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
- Religious public text may use garment-form names such as habit, scapular, cowl, alb, stole, chasuble, dalmatic, cope, mitre, phelonion, sakkos, qamis, jubba, khirqa, tallit, kasaya, kesa, jōe, hakama, or eboshi when those are useful object names rather than merely culture labels. Builder-facing manifest labels may name orders or traditions; public `sdesc` and `fdesc` should still describe visible garment form, colour, textile, and construction.
- Avoid male-default naming. Do not create unmarked `tunic` plus marked `women's tunic` pairs when the meaningful distinction is length, fullness, sleeve form, drape, or layering. Gender can appear in builder-facing outfit manifests, but public text should prefer form-based distinctions.

## Skin strategy

- All target garments are skinnable because they are finished wearable goods.
- A skin can override presentation fields and quality without changing the underlying item behaviour.
- Skins should carry high-variance presentation: local fantasy-culture names, exact trim patterns, tablet-woven bands, embroidery, tiraz text, rank borders, official insignia, household marks, clan marks, religious or temple motifs, order-specific trim nuance, vestment embroidery, silk patterning, brooch placement, dye intensity, material nuance, and fantasy-world motifs.
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


### Latin Christian Benedictine monk

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_sandals` - a pair of plain leather sandals
- `medieval_latin_black_monastic_habit` - a black wool monastic habit
- `medieval_latin_black_scapular` - a black wool scapular
- `medieval_latin_black_cowl` - a black wool cowl

### Latin Christian Benedictine nun

- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_latin_simple_nun_habit` - a dark wool nun's habit
- `medieval_latin_black_scapular` - a black wool scapular
- `medieval_latin_nun_wimple_veil` - a white linen wimple and veil

### Latin Christian Cistercian monk

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_sandals` - a pair of plain leather sandals
- `medieval_latin_white_monastic_tunic` - an undyed wool monastic tunic
- `medieval_latin_black_over_scapular` - a black wool over-scapular
- `medieval_latin_black_cowl` - a black wool cowl

### Latin Christian Cistercian nun

- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_latin_white_monastic_tunic` - an undyed wool monastic tunic
- `medieval_latin_black_over_scapular` - a black wool over-scapular
- `medieval_latin_nun_wimple_veil` - a white linen wimple and veil

### Latin Christian Carthusian monk

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_sandals` - a pair of plain leather sandals
- `medieval_latin_white_carthusian_habit` - a white wool hermit's habit
- `medieval_latin_black_cowl` - a black wool cowl

### Latin Christian Augustinian canon

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_latin_canon_black_habit` - a black wool canon's habit
- `medieval_latin_white_rochet` - a white linen rochet

### Latin Christian Premonstratensian canon

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_sandals` - a pair of plain leather sandals
- `medieval_latin_white_monastic_tunic` - an undyed wool monastic tunic
- `medieval_latin_white_rochet` - a white linen rochet

### Latin Christian Franciscan friar

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_sandals` - a pair of plain leather sandals
- `medieval_latin_friar_grey_habit` - a grey wool friar's habit
- `medieval_latin_rope_cincture` - a knotted rope cincture

### Latin Christian Dominican friar

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_latin_white_friar_habit` - a white wool friar's habit
- `medieval_latin_black_friar_cappa` - a black wool friar's cappa
- `medieval_plain_leather_belt` - a plain leather belt

### Latin Christian Carmelite friar

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_plain_leather_sandals` - a pair of plain leather sandals
- `medieval_latin_brown_friar_habit` - a brown wool friar's habit
- `medieval_latin_white_friar_mantle` - a white wool friar's mantle
- `medieval_plain_leather_belt` - a plain leather belt

### Latin Christian Poor Clare nun

- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_plain_leather_sandals` - a pair of plain leather sandals
- `medieval_latin_poor_clare_grey_habit` - a grey wool sister's habit
- `medieval_latin_rope_cincture` - a knotted rope cincture
- `medieval_latin_poor_clare_veil` - a plain linen sister's veil

### Latin Christian parish cleric

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_latin_black_clerical_gown` - a black wool clerical gown
- `medieval_linen_coif` - a $colour linen coif

### Latin Christian priest at Mass

- `medieval_latin_amice` - a white linen amice
- `medieval_latin_white_alb` - a white linen alb
- `medieval_latin_linen_cincture` - a white linen cincture
- `medieval_latin_stole` - a $colour silk stole
- `medieval_latin_maniple` - a $colour silk maniple
- `medieval_latin_chasuble` - a fine $colour silk chasuble

### Latin Christian deacon at Mass

- `medieval_latin_amice` - a white linen amice
- `medieval_latin_white_alb` - a white linen alb
- `medieval_latin_linen_cincture` - a white linen cincture
- `medieval_latin_stole` - a $colour silk stole
- `medieval_latin_maniple` - a $colour silk maniple
- `medieval_latin_dalmatic_vestment` - a fine $colour silk dalmatic

### Latin Christian bishop in pontificals

- `medieval_latin_amice` - a white linen amice
- `medieval_latin_white_alb` - a white linen alb
- `medieval_latin_linen_cincture` - a white linen cincture
- `medieval_latin_stole` - a $colour silk stole
- `medieval_latin_chasuble` - a fine $colour silk chasuble
- `medieval_latin_processional_cope` - a fine $colour silk cope
- `medieval_latin_bishop_mitre` - a fine white silk mitre
- `medieval_latin_bishop_gloves` - a pair of fine white liturgical gloves

### Latin Christian archbishop in pallium vestments

- `medieval_latin_amice` - a white linen amice
- `medieval_latin_white_alb` - a white linen alb
- `medieval_latin_linen_cincture` - a white linen cincture
- `medieval_latin_stole` - a $colour silk stole
- `medieval_latin_chasuble` - a fine $colour silk chasuble
- `medieval_latin_processional_cope` - a fine $colour silk cope
- `medieval_latin_bishop_mitre` - a fine white silk mitre
- `medieval_latin_bishop_gloves` - a pair of fine white liturgical gloves
- `medieval_latin_archbishop_pallium` - a white wool pallium band

### Eastern Christian monk

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_eastern_black_riassa` - a black wool riassa
- `medieval_eastern_monastic_mantle` - a black wool monastic mantle
- `medieval_eastern_black_klobuk` - a black veiled klobuk

### Eastern Christian nun

- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_eastern_black_riassa` - a black wool riassa
- `medieval_eastern_monastic_mantle` - a black wool monastic mantle
- `medieval_eastern_womens_monastic_veil` - a black monastic veil

### Eastern Christian deacon

- `medieval_eastern_sticharion` - a white linen sticharion
- `medieval_eastern_orarion` - a $colour silk orarion
- `medieval_eastern_epimanikia` - a pair of $colour silk epimanikia
- `medieval_soft_leather_shoes` - a pair of soft leather shoes

### Eastern Christian priest

- `medieval_eastern_sticharion` - a white linen sticharion
- `medieval_eastern_epitrachelion` - a $colour silk epitrachelion
- `medieval_eastern_epimanikia` - a pair of $colour silk epimanikia
- `medieval_eastern_phelonion` - a fine $colour silk phelonion
- `medieval_eastern_kamilavkion` - a black felt kamilavkion

### Eastern Christian bishop

- `medieval_eastern_sticharion` - a white linen sticharion
- `medieval_eastern_epitrachelion` - a $colour silk epitrachelion
- `medieval_eastern_epimanikia` - a pair of $colour silk epimanikia
- `medieval_eastern_sakkos` - a fine $colour silk sakkos
- `medieval_eastern_omophorion` - a fine white silk omophorion
- `medieval_eastern_kamilavkion` - a black felt kamilavkion

### Islamic imam / khatib

- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_islamic_plain_imam_qamis` - a plain white cotton imam qamis
- `medieval_islamic_plain_imam_turban` - a plain white cotton imam turban
- `medieval_plain_leather_sandals` - a pair of plain leather sandals

### Islamic qadi / madrasa scholar

- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_linen_qamis` - a $colour linen qamis
- `medieval_islamic_scholars_jubba` - a dark wool scholar's jubba
- `medieval_islamic_taylasan` - a dark wool taylasan hood
- `medieval_islamic_qadi_turban` - a fine white scholar's turban
- `medieval_soft_leather_slippers` - a pair of soft leather slippers

### Islamic Sufi dervish

- `medieval_linen_qamis` - a $colour linen qamis
- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_sufi_patched_khirqa` - a patched wool khirqa
- `medieval_sufi_wool_sash` - a coarse wool Sufi sash
- `medieval_sufi_felt_cap` - a plain felt dervish cap

### Islamic Sufi sheikh

- `medieval_cotton_qamis` - a $colour cotton qamis
- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_islamic_scholars_jubba` - a dark wool scholar's jubba
- `medieval_sufi_coarse_wool_cloak` - a coarse wool Sufi cloak
- `medieval_islamic_plain_imam_turban` - a plain white cotton imam turban

### Jewish synagogue scholar

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_jewish_scholars_robe` - a dark wool scholar's robe
- `medieval_jewish_skullcap` - a plain wool skullcap
- `medieval_jewish_tallit_gadol` - a white wool tallit gadol

### Jewish prayer leader

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_simple_woven_sash` - a simple woven sash
- `medieval_jewish_tallit_katan` - a white wool tallit katan
- `medieval_jewish_tallit_gadol` - a white wool tallit gadol
- `medieval_jewish_skullcap` - a plain wool skullcap

### Hindu temple priest

- `medieval_hindu_white_priest_dhoti` - a white cotton priest's dhoti
- `medieval_hindu_white_uttariya` - a white cotton uttariya
- `medieval_plain_leather_sandals` - a pair of plain leather sandals

### Hindu ascetic

- `medieval_hindu_kaupina` - a plain cotton kaupina
- `medieval_hindu_ochre_ascetic_wrap` - an ochre cotton ascetic wrap
- `medieval_hindu_ochre_shoulder_cloth` - an ochre cotton shoulder cloth
- `medieval_plain_leather_sandals` - a pair of plain leather sandals

### Jain Śvetāmbara monk

- `medieval_jain_white_ascetic_robe` - a white cotton ascetic robe
- `medieval_jain_white_shoulder_wrap` - a white cotton shoulder wrap
- `medieval_plain_leather_sandals` - a pair of plain leather sandals

### Jain Śvetāmbara nun

- `medieval_jain_white_ascetic_robe` - a white cotton ascetic robe
- `medieval_jain_white_shoulder_wrap` - a white cotton shoulder wrap
- `medieval_linen_veil` - a $colour linen veil
- `medieval_plain_leather_sandals` - a pair of plain leather sandals

### Buddhist monk

- `medieval_buddhist_plain_underrobe` - a plain hemp monastic underrobe
- `medieval_buddhist_patched_kasaya` - a patched cotton kasaya
- `medieval_hemp_sandals` - a pair of hemp sandals

### Buddhist nun

- `medieval_buddhist_nun_robe` - a dark hemp nun robe
- `medieval_buddhist_patched_kasaya` - a patched cotton kasaya
- `medieval_hemp_sandals` - a pair of hemp sandals

### Buddhist formal officiant

- `medieval_buddhist_plain_underrobe` - a plain hemp monastic underrobe
- `medieval_buddhist_formal_kesa` - a fine silk kesa mantle
- `medieval_buddhist_travelling_mantle` - a dark wool monastic travelling mantle
- `medieval_hemp_sandals` - a pair of hemp sandals

### Buddhist travelling monk

- `medieval_buddhist_black_monastic_robe` - a black hemp monastic robe
- `medieval_buddhist_patched_kasaya` - a patched cotton kasaya
- `medieval_buddhist_travelling_mantle` - a dark wool monastic travelling mantle
- `medieval_hemp_sandals` - a pair of hemp sandals

### Daoist priest

- `medieval_song_cloth_shoes` - a pair of cloth shoes
- `medieval_daoist_cross_collar_robe` - a dark cotton Daoist robe
- `medieval_daoist_cloud_sleeved_robe` - a fine $colour cloud-sleeved robe
- `medieval_daoist_ritual_cap` - a black cloth ritual cap

### Daoist nun

- `medieval_song_cloth_shoes` - a pair of cloth shoes
- `medieval_daoist_cross_collar_robe` - a dark cotton Daoist robe
- `medieval_daoist_ritual_cap` - a black cloth ritual cap

### Shinto priest

- `medieval_shinto_white_joe_robe` - a white hemp jōe robe
- `medieval_shinto_priest_hakama` - a pair of white shrine hakama
- `medieval_shinto_priest_eboshi` - a black lacquered eboshi
- `medieval_hemp_sandals` - a pair of hemp sandals

### Shrine attendant

- `medieval_shinto_miko_white_kosode` - a white hemp shrine kosode
- `medieval_shinto_miko_red_hakama` - a pair of red shrine hakama
- `medieval_hemp_sandals` - a pair of hemp sandals

### Steppe ritual specialist

- `medieval_wool_deel` - a $colour wool deel
- `medieval_steppe_ritual_felt_coat` - a felt-lined ritual coat
- `medieval_steppe_ritual_fur_cap` - a fur-trimmed ritual cap
- `medieval_steppe_ritual_sash` - a braided wool ritual sash
- `medieval_high_leather_boots` - a pair of high leather boots


#### Shared travel, service, scholarly, and life-stage outfits

These social-context manifests are builder-facing overlays and complete outfit examples. They are shared across compatible culture families and should be adjusted with local skins rather than duplicated for every inspiration family. They intentionally omit wearable containers such as pouches, purses, alms bags, travel scrips, satchels, backpacks, or tool bags.

### Shared western road traveller

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_wool_tunic` - a $colour wool tunic
- `medieval_wool_hose` - a pair of $colour wool hose
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_travel_wool_gaiters` - a pair of $colour wool travel gaiters
- `medieval_travel_wool_cloak` - a weather-stained $colour wool travel cloak
- `medieval_broad_brim_felt_hat` - a broad-brim $colour felt travel hat
- `medieval_annular_bronze_brooch` - a bronze annular brooch

### Shared western pilgrim or pious road traveller

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_long_wool_tunic` - a long $colour wool tunic
- `medieval_hemp_waist_cord` - a plaited hemp waist cord
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_travel_wool_cloak` - a weather-stained $colour wool travel cloak
- `medieval_broad_brim_felt_hat` - a broad-brim $colour felt travel hat

### Islamic road traveller

- `medieval_linen_qamis` - a $colour linen qamis
- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_wrapped_cotton_turban` - a $colour cotton turban
- `medieval_soft_leather_slippers` - a pair of soft leather slippers
- `medieval_silk_robe_sash` - a fine $colour silk robe sash
- `medieval_oiled_linen_rain_cape` - an oiled linen rain cape

### Indian road traveller

- `medieval_cotton_dhoti` - a $colour cotton dhoti
- `medieval_cotton_uttariya` - a $colour cotton uttariya
- `medieval_wrapped_cotton_turban` - a $colour cotton turban
- `medieval_cotton_waist_cord` - a white cotton waist cord
- `medieval_rope_soled_hemp_sandals` - a pair of rope-soled hemp sandals
- `medieval_thick_cotton_shawl` - a thick $colour cotton shawl

### East Asian road traveller

- `medieval_song_cloth_shoes` - a pair of cloth shoes
- `medieval_song_work_shan_jacket` - a plain $colour work shan jacket
- `medieval_song_short_work_trousers` - a pair of short $colour work trousers
- `medieval_linen_headband` - a $colour linen headband
- `medieval_oiled_linen_rain_cape` - an oiled linen rain cape
- `medieval_travel_wool_gaiters` - a pair of $colour wool travel gaiters

### Steppe mounted courier

- `medieval_wool_deel` - a $colour belted steppe robe
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_steppe_felt_cap` - a $colour steppe felt cap
- `medieval_high_leather_boots` - a pair of high leather boots
- `medieval_silver_mounted_leather_belt` - a fine silver-mounted belt
- `medieval_steppe_riding_caftan` - a $colour riding caftan

### Shepherd or upland herder

- `medieval_wool_tunic` - a $colour wool tunic
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_rough_wool_gaiters` - a pair of rough $colour wool gaiters
- `medieval_shepherds_hooded_cloak` - a rough $colour shepherd's cloak
- `medieval_rough_felt_herder_hat` - a rough felt herder's hat
- `medieval_iron_cloak_pin` - an iron cloak pin

### Forester or hawking retainer

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_wool_tunic` - a $colour wool tunic
- `medieval_wool_hose` - a pair of $colour wool hose
- `medieval_ankle_leather_boots` - a pair of ankle leather boots
- `medieval_leather_hawking_glove` - a single leather hawking glove
- `medieval_wool_hood` - a $colour wool hood
- `medieval_cart_driver_wool_coat` - a belted $colour cart driver's coat

### Fisher or boatman

- `medieval_short_boatmans_tunic` - a short $colour boatman's tunic
- `medieval_fisher_headcloth` - a $colour fisher's headcloth
- `medieval_rope_soled_hemp_sandals` - a pair of rope-soled hemp sandals
- `medieval_oiled_linen_rain_cape` - an oiled linen rain cape
- `medieval_wool_leg_ties` - a pair of $colour wool leg ties

### Field labourer in poor weather

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_wool_tunic` - a $colour wool tunic
- `medieval_wool_leg_wraps` - a pair of $colour wool leg wraps
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_rough_wool_gaiters` - a pair of rough $colour wool gaiters
- `medieval_patched_wool_cloak` - a patched $colour wool cloak
- `medieval_iron_cloak_pin` - an iron cloak pin

### Stable hand or carter

- `medieval_short_service_tunic` - a short $colour service tunic
- `medieval_narrow_wool_trousers` - a pair of narrow $colour wool trousers
- `medieval_ankle_leather_boots` - a pair of ankle leather boots
- `medieval_stablehand_wool_cap` - a $colour stablehand's wool cap
- `medieval_leather_stable_apron` - a leather stable apron
- `medieval_cart_driver_wool_coat` - a belted $colour cart driver's coat

### Western household servant

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_short_service_tunic` - a short $colour service tunic
- `medieval_wool_hose` - a pair of $colour wool hose
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_household_work_sash` - a $colour household work sash
- `medieval_linen_coif` - a $colour linen coif

### Kitchen worker or cook

- `medieval_short_service_tunic` - a short $colour service tunic
- `medieval_large_linen_work_apron` - a large $colour linen work apron
- `medieval_kitchen_sleeve_ties` - a pair of $colour kitchen sleeve ties
- `medieval_scullion_linen_coif` - a $colour scullion's linen coif
- `medieval_soft_leather_shoes` - a pair of soft leather shoes

### Laundress or washerwoman

- `medieval_linen_chemise` - a $colour linen chemise
- `medieval_tucked_work_kirtle` - a tucked $colour work kirtle
- `medieval_large_linen_work_apron` - a large $colour linen work apron
- `medieval_linen_laundry_headcloth` - a $colour laundry headcloth
- `medieval_soft_leather_shoes` - a pair of soft leather shoes

### Inn or tavern worker

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_short_service_tunic` - a short $colour service tunic
- `medieval_wool_hose` - a pair of $colour wool hose
- `medieval_large_linen_work_apron` - a large $colour linen work apron
- `medieval_inn_workers_wool_hood` - a $colour inn worker's wool hood
- `medieval_soft_leather_shoes` - a pair of soft leather shoes

### Byzantine or Islamic household servant

- `medieval_linen_qamis` - a $colour linen qamis
- `medieval_cotton_sirwal` - a pair of $colour cotton sirwal
- `medieval_household_work_sash` - a $colour household work sash
- `medieval_large_linen_work_apron` - a large $colour linen work apron
- `medieval_fisher_headcloth` - a $colour fisher's headcloth
- `medieval_soft_leather_slippers` - a pair of soft leather slippers

### East Asian household servant

- `medieval_japanese_work_kosode` - a plain $colour work kosode
- `medieval_japanese_work_hakama` - a pair of $colour work hakama
- `medieval_woven_kosode_sash` - a $colour woven kosode sash
- `medieval_japanese_hemp_work_apron` - a hemp work apron
- `medieval_linen_headband` - a $colour linen headband
- `medieval_japanese_hemp_sandals` - a pair of hemp sandals

### University student or junior scholar

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_student_wool_gown` - a plain $colour student gown
- `medieval_lined_scholars_hood` - a lined $colour scholar's hood
- `medieval_plain_black_skullcap` - a plain black skullcap
- `medieval_soft_leather_shoes` - a pair of soft leather shoes

### Master scholar or lecturer

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_master_scholar_gown` - a sober $colour scholar's gown
- `medieval_sober_lecture_mantle` - a sober $colour lecture mantle
- `medieval_fine_scholars_cap` - a fine $colour scholar's cap
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_silver_ring` - a fine silver finger ring

### Physician

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_physician_wool_gown` - a long $colour physician's gown
- `medieval_lined_scholars_hood` - a lined $colour scholar's hood
- `medieval_fine_scholars_cap` - a fine $colour scholar's cap
- `medieval_fine_leather_shoes` - a pair of fine leather shoes
- `medieval_fine_silver_ring` - a fine silver finger ring

### Legal clerk or notary

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_notary_wool_gown` - a neat $colour notary's gown
- `medieval_plain_black_skullcap` - a plain black skullcap
- `medieval_scribe_linen_sleeves` - a pair of $colour linen scribe sleeves
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_simple_bronze_ring` - a simple bronze finger ring

### Common child

- `medieval_child_linen_shift` - a small $colour child's linen shift
- `medieval_child_wool_tunic` - a small $colour child's wool tunic
- `medieval_child_leather_shoes` - a pair of small leather child's shoes
- `medieval_child_wool_cloak` - a small $colour child's wool cloak

### Elite child

- `medieval_child_linen_shift` - a small $colour child's linen shift
- `medieval_child_silk_court_robe` - a small fine $colour silk court robe
- `medieval_child_leather_shoes` - a pair of small leather child's shoes
- `medieval_child_wool_cloak` - a small $colour child's wool cloak
- `medieval_silk_hair_ribbon` - a fine $colour silk hair ribbon

### Apprentice

- `medieval_linen_undertunic` - a $colour linen undertunic
- `medieval_apprentice_work_tunic` - a short $colour apprentice's tunic
- `medieval_apprentice_canvas_apron` - a $colour apprentice's canvas apron
- `medieval_apprentice_linen_cap` - a $colour apprentice's linen cap
- `medieval_soft_leather_shoes` - a pair of soft leather shoes
- `medieval_household_work_sash` - a $colour household work sash

## Item catalogue

Catalogue line format: `uniqueReference` - public short description; noun; primary material; size/quality; weight/cost; wear component; variable component. Seasonal, urban, professional, religious, accessory, and social-context add-on lines may append `matches:` to identify the outfit manifests they can be added to or substituted into. Full descriptions remain in the seeder calls and are not duplicated here.

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


### Religious clothing additions/substitutions by tradition

Religious clothing additions are shared overlays and outfit manifests rather than culture-specific duplicates. They cover clothing for monastic orders, friars, nuns, clergy, bishops, scholars, prayer leaders, temple priests, ascetics, Buddhist and Daoist monastics, Shinto shrine service, and steppe ritual specialists. They intentionally exclude non-clothing religious paraphernalia, carried objects, jewellery, badges, tools, books, masks, insignia not inherent to the garment, and mechanically meaningful ritual equipment.

- `medieval_latin_black_monastic_habit` - a black wool monastic habit; noun: `habit`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 900g/24.0m; wear: `Wear_Robe`; variables: none; matches: Latin Christian Benedictine monk/nun and other black-habit western monastic variants.
- `medieval_latin_black_scapular` - a black wool scapular; noun: `scapular`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 260g/8.0m; wear: `Wear_Tabard`; variables: none; matches: Latin Christian Benedictine monk/nun and black-habit monastic variants over a habit.
- `medieval_latin_black_cowl` - a black wool cowl; noun: `cowl`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 650g/18.0m; wear: `Wear_Cloak_(Closed)`; variables: none; matches: Latin Christian Benedictine, Cistercian, Carthusian, and other cloistered monastic variants.
- `medieval_latin_white_monastic_tunic` - an undyed wool monastic tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 820g/24.0m; wear: `Wear_Robe`; variables: none; matches: Latin Christian Cistercian, Premonstratensian, and white-habit monastic variants.
- `medieval_latin_black_over_scapular` - a black wool over-scapular; noun: `scapular`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 280g/8.0m; wear: `Wear_Tabard`; variables: none; matches: Latin Christian Cistercian monk/nun and other white-tunic black-scapular variants.
- `medieval_latin_white_carthusian_habit` - a white wool hermit's habit; noun: `habit`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 950g/30.0m; wear: `Wear_Robe`; variables: none; matches: Latin Christian Carthusian monk and austere white-hermit variants.
- `medieval_latin_canon_black_habit` - a black wool canon's habit; noun: `habit`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 850g/28.0m; wear: `Wear_Robe`; variables: none; matches: Latin Christian Augustinian canon and other black-robed canon variants.
- `medieval_latin_white_rochet` - a white linen rochet; noun: `rochet`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 320g/28.0m; wear: `Wear_Shirt`; variables: none; matches: Latin Christian Augustinian, Premonstratensian, cathedral, and choir-office clergy variants.
- `medieval_latin_friar_grey_habit` - a grey wool friar's habit; noun: `habit`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 850g/20.0m; wear: `Wear_Robe`; variables: none; matches: Latin Christian Franciscan friar and grey-friar mendicant variants.
- `medieval_latin_rope_cincture` - a knotted rope cincture; noun: `cincture`; material: `hemp`; size/quality: `Small`/`Standard`; weight/cost: 160g/4.0m; wear: `Wear_Sash`; variables: none; matches: Latin Christian Franciscan friar, Poor Clare nun, and other cord-belt mendicant variants.
- `medieval_latin_white_friar_habit` - a white wool friar's habit; noun: `habit`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 780g/28.0m; wear: `Wear_Robe`; variables: none; matches: Latin Christian Dominican friar and white-habit preaching-order variants.
- `medieval_latin_black_friar_cappa` - a black wool friar's cappa; noun: `cappa`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 720g/24.0m; wear: `Wear_Cloak_(Open)`; variables: none; matches: Latin Christian Dominican friar and black-cappa travelling or choir variants.
- `medieval_latin_brown_friar_habit` - a brown wool friar's habit; noun: `habit`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 800g/22.0m; wear: `Wear_Robe`; variables: none; matches: Latin Christian Carmelite friar and brown-habit mendicant variants.
- `medieval_latin_white_friar_mantle` - a white wool friar's mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 640g/28.0m; wear: `Wear_Mantle`; variables: none; matches: Latin Christian Carmelite friar and white-mantled mendicant variants.
- `medieval_latin_simple_nun_habit` - a dark wool nun's habit; noun: `habit`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 820g/22.0m; wear: `Wear_Robe`; variables: none; matches: Latin Christian Benedictine nun and other cloistered western nun variants.
- `medieval_latin_nun_wimple_veil` - a white linen wimple and veil; noun: `veil`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 220g/12.0m; wear: `Wear_Veil`; variables: none; matches: Latin Christian Benedictine, Cistercian, Augustinian, and comparable nun variants.
- `medieval_latin_poor_clare_grey_habit` - a grey wool sister's habit; noun: `habit`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 760g/18.0m; wear: `Wear_Robe`; variables: none; matches: Latin Christian Poor Clare nun and simple grey sister variants.
- `medieval_latin_poor_clare_veil` - a plain linen sister's veil; noun: `veil`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 180g/8.0m; wear: `Wear_Veil`; variables: none; matches: Latin Christian Poor Clare nun and austere mendicant-nun variants.
- `medieval_latin_black_clerical_gown` - a black wool clerical gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 780g/30.0m; wear: `Wear_Robe`; variables: none; matches: Latin Christian parish cleric, cathedral clerk, schoolman, and non-liturgical clergy variants.
- `medieval_latin_white_alb` - a white linen alb; noun: `alb`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 460g/36.0m; wear: `Wear_Robe`; variables: none; matches: Latin Christian priest, deacon, bishop, and higher liturgical vestment variants.
- `medieval_latin_amice` - a white linen amice; noun: `amice`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 120g/10.0m; wear: `Wear_Scarf`; variables: none; matches: Latin Christian priest, deacon, bishop, and higher liturgical vestment variants.
- `medieval_latin_linen_cincture` - a white linen cincture; noun: `cincture`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 80g/6.0m; wear: `Wear_Sash`; variables: none; matches: Latin Christian priest, deacon, bishop, and alb-belted liturgical variants.
- `medieval_latin_stole` - a $colour silk stole; noun: `stole`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 110g/50.0m; wear: `Wear_Scarf`; variables: Variable_FineColour; matches: Latin Christian priest, deacon, bishop, and formal liturgical variants.
- `medieval_latin_maniple` - a $colour silk maniple; noun: `maniple`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 60g/30.0m; wear: `Wear_Bracer`; variables: Variable_FineColour; matches: Latin Christian priest, deacon, bishop, and formal liturgical variants.
- `medieval_latin_chasuble` - a fine $colour silk chasuble; noun: `chasuble`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 720g/160.0m; wear: `Wear_Poncho`; variables: Variable_FineColour; matches: Latin Christian priest-at-Mass, bishop, and high liturgical variants over alb and stole.
- `medieval_latin_dalmatic_vestment` - a fine $colour silk dalmatic; noun: `dalmatic`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 650g/140.0m; wear: `Wear_Robe`; variables: Variable_FineColour; matches: Latin Christian deacon-at-Mass and episcopal layered vestment variants.
- `medieval_latin_processional_cope` - a fine $colour silk cope; noun: `cope`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 900g/180.0m; wear: `Wear_Cloak_(Open)`; variables: Variable_FineColour; matches: Latin Christian bishop, archbishop, procession, choir, and solemn-office variants.
- `medieval_latin_bishop_mitre` - a fine white silk mitre; noun: `mitre`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 160g/140.0m; wear: `Wear_Hat`; variables: none; matches: Latin Christian bishop and archbishop pontifical variants.
- `medieval_latin_bishop_gloves` - a pair of fine white liturgical gloves; noun: `gloves`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 90g/80.0m; wear: `Wear_Gloves`; variables: none; matches: Latin Christian bishop and archbishop pontifical variants.
- `medieval_latin_archbishop_pallium` - a white wool pallium band; noun: `pallium`; material: `wool`; size/quality: `Small`/`Good`; weight/cost: 140g/100.0m; wear: `Wear_Scarf`; variables: none; matches: Latin Christian archbishop and metropolitan-bishop variants only.
- `medieval_eastern_black_riassa` - a black wool riassa; noun: `riassa`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 850g/30.0m; wear: `Wear_Robe`; variables: none; matches: Eastern Christian monk, nun, priest, and non-liturgical clergy variants.
- `medieval_eastern_monastic_mantle` - a black wool monastic mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 950g/34.0m; wear: `Wear_Mantle`; variables: none; matches: Eastern Christian monk, nun, abbot, and travelling monastic variants.
- `medieval_eastern_black_klobuk` - a black veiled klobuk; noun: `klobuk`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 240g/18.0m; wear: `Wear_Hat`; variables: none; matches: Eastern Christian monk and senior monastic headwear variants.
- `medieval_eastern_womens_monastic_veil` - a black monastic veil; noun: `veil`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 200g/12.0m; wear: `Wear_Veil`; variables: none; matches: Eastern Christian nun and women's monastic variants.
- `medieval_eastern_sticharion` - a white linen sticharion; noun: `sticharion`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 480g/40.0m; wear: `Wear_Robe`; variables: none; matches: Eastern Christian deacon, priest, bishop, and altar-serving liturgical variants.
- `medieval_eastern_orarion` - a $colour silk orarion; noun: `orarion`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 130g/60.0m; wear: `Wear_Scarf`; variables: Variable_FineColour; matches: Eastern Christian deacon liturgical variants over a sticharion.
- `medieval_eastern_epitrachelion` - a $colour silk epitrachelion; noun: `epitrachelion`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 190g/70.0m; wear: `Wear_Scarf`; variables: Variable_FineColour; matches: Eastern Christian priest and bishop liturgical variants.
- `medieval_eastern_epimanikia` - a pair of $colour silk epimanikia; noun: `epimanikia`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 80g/50.0m; wear: `Wear_Bracers`; variables: Variable_FineColour; matches: Eastern Christian deacon, priest, bishop, and other vested clergy variants.
- `medieval_eastern_phelonion` - a fine $colour silk phelonion; noun: `phelonion`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 780g/170.0m; wear: `Wear_Poncho`; variables: Variable_FineColour; matches: Eastern Christian priest liturgical variants over sticharion and epitrachelion.
- `medieval_eastern_sakkos` - a fine $colour silk sakkos; noun: `sakkos`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 800g/210.0m; wear: `Wear_Robe`; variables: Variable_FineColour; matches: Eastern Christian bishop and high Byzantine-rite episcopal variants.
- `medieval_eastern_omophorion` - a fine white silk omophorion; noun: `omophorion`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 220g/120.0m; wear: `Wear_Scarf`; variables: none; matches: Eastern Christian bishop and high Byzantine-rite episcopal variants.
- `medieval_eastern_kamilavkion` - a black felt kamilavkion; noun: `kamilavkion`; material: `felt`; size/quality: `Small`/`Standard`; weight/cost: 170g/20.0m; wear: `Wear_Hat`; variables: none; matches: Eastern Christian priest, monk, and bishop headwear variants where appropriate.
- `medieval_islamic_plain_imam_qamis` - a plain white cotton imam qamis; noun: `qamis`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 420g/18.0m; wear: `Wear_Robe`; variables: none; matches: Islamic imam, khatib, mosque teacher, and plain learned-clergy variants across Andalusian, Abbasid, Fatimid, Seljuk, and Indian Muslim settings.
- `medieval_islamic_plain_imam_turban` - a plain white cotton imam turban; noun: `turban`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 260g/14.0m; wear: `Wear_Turban`; variables: none; matches: Islamic imam, khatib, teacher, and mosque-official variants where a plain wrapped turban is wanted.
- `medieval_islamic_scholars_jubba` - a dark wool scholar's jubba; noun: `jubba`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 850g/48.0m; wear: `Wear_Robe`; variables: none; matches: Islamic qadi, madrasa scholar, preacher, legal official, and Sufi sheikh variants.
- `medieval_islamic_taylasan` - a dark wool taylasan hood; noun: `taylasan`; material: `wool`; size/quality: `Small`/`Good`; weight/cost: 240g/30.0m; wear: `Wear_Hat`; variables: none; matches: Islamic qadi, madrasa scholar, judge, and learned urban variants.
- `medieval_islamic_qadi_turban` - a fine white scholar's turban; noun: `turban`; material: `cotton`; size/quality: `Small`/`Good`; weight/cost: 300g/26.0m; wear: `Wear_Turban`; variables: none; matches: Islamic qadi, madrasa scholar, senior teacher, and legal-office variants.
- `medieval_sufi_patched_khirqa` - a patched wool khirqa; noun: `khirqa`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 780g/18.0m; wear: `Wear_Robe`; variables: none; matches: Islamic Sufi dervish, wandering ascetic, and voluntary-poverty variants across the Islamic inspiration families.
- `medieval_sufi_wool_sash` - a coarse wool Sufi sash; noun: `sash`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 180g/6.0m; wear: `Wear_Sash`; variables: none; matches: Islamic Sufi dervish, Sufi sheikh, and ascetic robe variants.
- `medieval_sufi_felt_cap` - a plain felt dervish cap; noun: `cap`; material: `felt`; size/quality: `Small`/`Standard`; weight/cost: 140g/10.0m; wear: `Wear_Hat`; variables: none; matches: Islamic Sufi dervish, Sufi sheikh, and plain ascetic headwear variants.
- `medieval_sufi_coarse_wool_cloak` - a coarse wool Sufi cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 980g/22.0m; wear: `Wear_Cloak_(Open)`; variables: none; matches: Islamic Sufi sheikh, Sufi dervish, and poor travelling-preacher variants.
- `medieval_jewish_tallit_gadol` - a white wool tallit gadol; noun: `tallit`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 420g/40.0m; wear: `Wear_Mantle`; variables: none; matches: Jewish synagogue prayer leader, scholar, worshipper, and Sabbath or festival prayer variants.
- `medieval_jewish_tallit_katan` - a white wool tallit katan; noun: `tallit`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 230g/22.0m; wear: `Wear_Vest`; variables: none; matches: Jewish daily pious dress, prayer leader, scholar, and underlayer variants.
- `medieval_jewish_skullcap` - a plain wool skullcap; noun: `skullcap`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 60g/6.0m; wear: `Wear_Skullcap`; variables: none; matches: Jewish scholar, synagogue worshipper, prayer leader, and modest headwear variants.
- `medieval_jewish_scholars_robe` - a dark wool scholar's robe; noun: `robe`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 820g/44.0m; wear: `Wear_Robe`; variables: none; matches: Jewish scholar, synagogue elder, court physician, merchant-scholar, and learned urban variants.
- `medieval_hindu_white_priest_dhoti` - a white cotton priest's dhoti; noun: `dhoti`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 360g/16.0m; wear: `Wear_Loincloth`; variables: none; matches: Hindu temple priest, ritual assistant, and clean white priestly lower-drape variants in North Indian / Rajput and South Indian / Chola settings.
- `medieval_hindu_white_uttariya` - a white cotton uttariya; noun: `uttariya`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 180g/10.0m; wear: `Wear_Mantle`; variables: none; matches: Hindu temple priest, ritual assistant, and plain upper-cloth variants.
- `medieval_hindu_kaupina` - a plain cotton kaupina; noun: `kaupina`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 90g/4.0m; wear: `Wear_Loincloth`; variables: none; matches: Hindu ascetic, renunciant, and austere underlayer variants.
- `medieval_hindu_ochre_ascetic_wrap` - an ochre cotton ascetic wrap; noun: `wrap`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 420g/14.0m; wear: `Wear_Robe`; variables: none; matches: Hindu ascetic, renunciant, mendicant teacher, and saffron-or-ochre religious variants.
- `medieval_hindu_ochre_shoulder_cloth` - an ochre cotton shoulder cloth; noun: `cloth`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 160g/8.0m; wear: `Wear_Mantle`; variables: none; matches: Hindu ascetic, renunciant, and religious teacher upper-layer variants.
- `medieval_jain_white_ascetic_robe` - a white cotton ascetic robe; noun: `robe`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 420g/16.0m; wear: `Wear_Robe`; variables: none; matches: Jain Śvetāmbara monk, nun, and white-clad ascetic variants in Indian urban and temple settings.
- `medieval_jain_white_shoulder_wrap` - a white cotton shoulder wrap; noun: `wrap`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 160g/8.0m; wear: `Wear_Mantle`; variables: none; matches: Jain Śvetāmbara monk, nun, and white-clad ascetic upper-layer variants.
- `medieval_buddhist_plain_underrobe` - a plain hemp monastic underrobe; noun: `underrobe`; material: `hemp`; size/quality: `Normal`/`Standard`; weight/cost: 520g/16.0m; wear: `Wear_Robe`; variables: none; matches: Buddhist monk, nun, novice, and travelling monastic variants across Song China, Goryeo Korea, Heian / Kamakura Japan, and comparable Buddhist settings.
- `medieval_buddhist_patched_kasaya` - a patched cotton kasaya; noun: `kasaya`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 320g/24.0m; wear: `Wear_Mantle`; variables: none; matches: Buddhist monk, nun, and ordinary monastic mantle variants.
- `medieval_buddhist_nun_robe` - a dark hemp nun robe; noun: `robe`; material: `hemp`; size/quality: `Normal`/`Standard`; weight/cost: 560g/18.0m; wear: `Wear_Robe`; variables: none; matches: Buddhist nun, female novice, and modest monastic robe variants.
- `medieval_buddhist_formal_kesa` - a fine silk kesa mantle; noun: `kesa`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 360g/110.0m; wear: `Wear_Mantle`; variables: none; matches: Buddhist abbot, formal officiant, temple elder, and ceremonial Japanese or East Asian Buddhist variants.
- `medieval_buddhist_travelling_mantle` - a dark wool monastic travelling mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 760g/24.0m; wear: `Wear_Cloak_(Open)`; variables: none; matches: Buddhist travelling monk, cold-weather monastery, pilgrimage, and mendicant road variants.
- `medieval_buddhist_black_monastic_robe` - a black hemp monastic robe; noun: `robe`; material: `hemp`; size/quality: `Normal`/`Standard`; weight/cost: 540g/18.0m; wear: `Wear_Robe`; variables: none; matches: Buddhist monk, Zen/Chan-adjacent, travelling monastic, and severe black-robe variants.
- `medieval_daoist_cross_collar_robe` - a dark cotton Daoist robe; noun: `robe`; material: `cotton`; size/quality: `Normal`/`Standard`; weight/cost: 520g/24.0m; wear: `Wear_Robe`; variables: none; matches: Daoist priest, nun, temple assistant, and everyday ritual-service variants in Song China settings.
- `medieval_daoist_cloud_sleeved_robe` - a fine $colour cloud-sleeved robe; noun: `robe`; material: `silk`; size/quality: `Normal`/`Good`; weight/cost: 680g/130.0m; wear: `Wear_Robe`; variables: Variable_FineColour; matches: Daoist priest, senior ritualist, temple ceremony, and formal Song religious variants.
- `medieval_daoist_ritual_cap` - a black cloth ritual cap; noun: `cap`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 90g/12.0m; wear: `Wear_Hat`; variables: none; matches: Daoist priest, nun, and temple ritual headwear variants.
- `medieval_shinto_white_joe_robe` - a white hemp jōe robe; noun: `jōe`; material: `hemp`; size/quality: `Normal`/`Good`; weight/cost: 520g/42.0m; wear: `Wear_Robe`; variables: none; matches: Shinto priest, shrine ritualist, and Heian / Kamakura Japanese shrine-service variants.
- `medieval_shinto_priest_hakama` - a pair of white shrine hakama; noun: `hakama`; material: `hemp`; size/quality: `Normal`/`Good`; weight/cost: 480g/34.0m; wear: `Wear_Trousers`; variables: none; matches: Shinto priest, shrine ritualist, and ceremonial Japanese shrine-service variants.
- `medieval_shinto_priest_eboshi` - a black lacquered eboshi; noun: `eboshi`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 110g/38.0m; wear: `Wear_Hat`; variables: none; matches: Shinto priest, shrine ritualist, and formal Heian / Kamakura Japanese headwear variants.
- `medieval_shinto_miko_white_kosode` - a white hemp shrine kosode; noun: `kosode`; material: `hemp`; size/quality: `Normal`/`Standard`; weight/cost: 420g/24.0m; wear: `Wear_Robe`; variables: none; matches: Shrine attendant, miko, female temple-helper, and Japanese shrine household-service variants.
- `medieval_shinto_miko_red_hakama` - a pair of red shrine hakama; noun: `hakama`; material: `hemp`; size/quality: `Normal`/`Standard`; weight/cost: 460g/28.0m; wear: `Wear_Trousers`; variables: none; matches: Shrine attendant, miko, female temple-helper, and Japanese shrine-service variants.
- `medieval_steppe_ritual_felt_coat` - a felt-lined ritual coat; noun: `coat`; material: `felt`; size/quality: `Normal`/`Standard`; weight/cost: 1100g/34.0m; wear: `Wear_Long_Coat`; variables: none; matches: Steppe animist/Tengri ritual specialist, shamanic healer, winter rite, and nomad religious-service variants.
- `medieval_steppe_ritual_fur_cap` - a fur-trimmed ritual cap; noun: `cap`; material: `fur`; size/quality: `Small`/`Standard`; weight/cost: 220g/24.0m; wear: `Wear_Hat`; variables: none; matches: Steppe animist/Tengri ritual specialist, shamanic healer, winter rite, and nomad religious-service variants.
- `medieval_steppe_ritual_sash` - a braided wool ritual sash; noun: `sash`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 190g/12.0m; wear: `Wear_Sash`; variables: none; matches: Steppe animist/Tengri ritual specialist, shamanic healer, and nomad religious-service variants over a deel or felt coat.

### Accessory, travel, service, scholar, and life-stage additions/substitutions

These catalogue overlays add visible worn fasteners, jewellery, road clothing, rural specialist clothing, household-service dress, literate-professional clothing, and child or apprentice garments. They are shared overlays rather than culture-by-culture duplicates. Wearable containers are deliberately omitted from this pass: no pouches, purses, alms bags, travel scrips, satchels, backpacks, or tool bags are included, and descriptions should not imply hidden storage or attachment behaviour.

#### Worn accessories and fasteners

- `medieval_annular_bronze_brooch` - a bronze annular brooch; noun: `brooch`; material: `bronze`; size/quality: `Small`/`Standard`; weight/cost: 45g/18.0m; wear: `Wear_Shoulder`; variables: none; matches: Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Carolingian / Frankish, Irish / Gaelic, Scottish / Gaelic-Lowland, Rus / Novgorod, and western traveller cloak variants.
- `medieval_penannular_bronze_brooch` - a bronze penannular brooch; noun: `brooch`; material: `bronze`; size/quality: `Small`/`Good`; weight/cost: 55g/32.0m; wear: `Wear_Shoulder`; variables: none; matches: Irish / Gaelic, Scottish / Gaelic-Lowland, Norse / Viking Age, Anglo-Danish, Rus / Novgorod, and elite cloak variants using shoulder-fastened outerwear.
- `medieval_pair_oval_brooches` - a pair of bronze oval brooches; noun: `brooches`; material: `bronze`; size/quality: `Small`/`Good`; weight/cost: 110g/70.0m; wear: `Wear_Shoulders`; variables: none; matches: Norse / Viking Age and Anglo-Danish hangerok, særk, elite female, festive, and North Sea household variants.
- `medieval_plain_bronze_cloak_pin` - a plain bronze cloak pin; noun: `pin`; material: `bronze`; size/quality: `Small`/`Standard`; weight/cost: 35g/12.0m; wear: `Wear_Shoulder`; variables: none; matches: all cloak, brat, mantle, paenula, sagion, brat, and travel-cloak variants where a simple visible fastening is useful.
- `medieval_iron_cloak_pin` - an iron cloak pin; noun: `pin`; material: `wrought iron`; size/quality: `Small`/`Standard`; weight/cost: 45g/8.0m; wear: `Wear_Shoulder`; variables: none; matches: rural, servant, traveller, shepherd, herder, field-labour, and poorer cloak or mantle variants across northern, western, Rus, and steppe-facing families.
- `medieval_bronze_mounted_leather_belt` - a leather belt with bronze mounts; noun: `belt`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 300g/46.0m; wear: `Wear_Waist`; variables: none; matches: Norse / Viking Age, Rus / Novgorod, Magyar, Steppe Turkic / Mongol, merchant, retainer, elite town, and riding variants where a more visible fitted belt is wanted.
- `medieval_silver_mounted_leather_belt` - a fine silver-mounted belt; noun: `belt`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 280g/150.0m; wear: `Wear_Waist`; variables: none; matches: elite Norse / Viking Age, Rus / Novgorod, Magyar, Steppe Turkic / Mongol, Byzantine, court, merchant, and formal riding variants.
- `medieval_tablet_woven_garters` - a pair of $colour tablet-woven garters; noun: `garters`; material: `wool`; size/quality: `Small`/`Good`; weight/cost: 60g/16.0m; wear: `Wear_Leggings`; variables: Variable_FineColour; matches: hose, chausses, leg-wrap, high-medieval western, northern elite, and prosperous town variants.
- `medieval_wool_leg_ties` - a pair of $colour wool leg ties; noun: `leg ties`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 50g/5.0m; wear: `Wear_Leggings`; variables: Variable_BasicColour; matches: leg-wrap, onuchi, gaiter, trouser, herder, traveller, rural worker, and northern commoner variants.
- `medieval_linen_headband` - a $colour linen headband; noun: `headband`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 35g/4.0m; wear: `Wear_Headband`; variables: Variable_BasicColour; matches: common town, workshop, field-labour, East Asian, Indian, and warm-climate variants needing a simple hair or brow binding.
- `medieval_silk_hair_ribbon` - a fine $colour silk hair ribbon; noun: `ribbon`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 20g/30.0m; wear: `Wear_Headband`; variables: Variable_FineColour; matches: elite town, court, Byzantine, Islamic, Indian, Song China, Goryeo Korea, and Heian / Kamakura Japan hair-binding or decorative headwear variants.
- `medieval_silk_robe_sash` - a fine $colour silk robe sash; noun: `sash`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 130g/78.0m; wear: `Wear_Sash`; variables: Variable_FineColour; matches: Byzantine, Abbasid, Fatimid, Seljuk / Ayyubid-Mamluk, Song China, Goryeo Korea, Indian court, merchant, scholar, and formal robe variants.
- `medieval_woven_kosode_sash` - a $colour woven kosode sash; noun: `sash`; material: `silk`; size/quality: `Small`/`Standard`; weight/cost: 120g/34.0m; wear: `Wear_Sash`; variables: Variable_BasicColour; matches: Heian / Kamakura Japan kosode, work kosode, travelling kosode, household-service kosode, and modest court-underlayer variants.
- `medieval_hemp_waist_cord` - a plaited hemp waist cord; noun: `cord`; material: `hemp`; size/quality: `Small`/`Standard`; weight/cost: 70g/3.0m; wear: `Wear_Waist`; variables: none; matches: rural, servant, labourer, ascetic, apprentice, kitchen, field, and poor traveller variants where a plain cord substitutes for a belt or girdle.
- `medieval_cotton_waist_cord` - a white cotton waist cord; noun: `cord`; material: `cotton`; size/quality: `Small`/`Standard`; weight/cost: 55g/4.0m; wear: `Wear_Waist`; variables: none; matches: Indian, Jain, Hindu, Islamic warm-climate, South Indian / Chola, North Indian / Rajput, and light-cloth service variants.
- `medieval_glass_bead_necklace` - a glass bead necklace; noun: `necklace`; material: `glass`; size/quality: `Small`/`Standard`; weight/cost: 85g/20.0m; wear: `Wear_Necklace`; variables: none; matches: Norse / Viking Age, Rus / Novgorod, Byzantine, Islamic, Indian, East Asian, market, festive, and prosperous commoner variants.
- `medieval_amber_bead_necklace` - an amber bead necklace; noun: `necklace`; material: `amber`; size/quality: `Small`/`Good`; weight/cost: 70g/82.0m; wear: `Wear_Necklace`; variables: none; matches: Norse / Viking Age, Rus / Novgorod, Baltic-facing, northern merchant, elite female, and trade-linked festive variants.
- `medieval_shell_bead_necklace` - a shell bead necklace; noun: `necklace`; material: `shell`; size/quality: `Small`/`Standard`; weight/cost: 65g/18.0m; wear: `Wear_Necklace`; variables: none; matches: Indian Ocean, Islamic Mediterranean, Fatimid, Andalusian, North Indian / Rajput, South Indian / Chola, coastal, and warm-climate trade-linked variants.
- `medieval_bronze_armlet` - a bronze armlet; noun: `armlet`; material: `bronze`; size/quality: `Small`/`Standard`; weight/cost: 95g/24.0m; wear: `Wear_Armlet`; variables: none; matches: Norse / Viking Age, Rus / Novgorod, Steppe Turkic / Mongol, Magyar, Indian, Byzantine, and festive or elite commoner variants.
- `medieval_bronze_bracelets` - a pair of bronze bracelets; noun: `bracelets`; material: `bronze`; size/quality: `Small`/`Standard`; weight/cost: 90g/22.0m; wear: `Wear_Bracelets`; variables: none; matches: Byzantine, Islamic, Indian, Rus / Novgorod, market, household, festive, and prosperous commoner variants.
- `medieval_bronze_bangles` - a pair of bronze bangles; noun: `bangles`; material: `bronze`; size/quality: `Small`/`Standard`; weight/cost: 110g/24.0m; wear: `Wear_Bracelets`; variables: none; matches: North Indian / Rajput, South Indian / Chola, Islamic Mediterranean, Byzantine, steppe, and warm-climate festive variants.
- `medieval_bronze_anklets` - a pair of bronze anklets; noun: `anklets`; material: `bronze`; size/quality: `Small`/`Standard`; weight/cost: 115g/26.0m; wear: `Wear_Anklets`; variables: none; matches: Indian, Islamic warm-climate, Byzantine, court, dancer, festive, and household adornment variants where ankle jewellery is setting-appropriate.
- `medieval_silver_earrings` - a pair of silver earrings; noun: `earrings`; material: `silver`; size/quality: `Small`/`Good`; weight/cost: 18g/90.0m; wear: `Wear_Earrings`; variables: none; matches: Byzantine, Islamic, Indian, Rus / Novgorod, steppe, merchant, court, and elite town variants.
- `medieval_simple_bronze_ring` - a simple bronze finger ring; noun: `ring`; material: `bronze`; size/quality: `Small`/`Standard`; weight/cost: 10g/10.0m; wear: `Wear_Ring`; variables: none; matches: broad everyday, town, rural prosperous, merchant, servant-best, religious lay, and common festive variants.
- `medieval_fine_silver_ring` - a fine silver finger ring; noun: `ring`; material: `silver`; size/quality: `Small`/`Good`; weight/cost: 12g/80.0m; wear: `Wear_Ring`; variables: none; matches: elite, merchant, clerical, scholar, court, formal, and prosperous urban variants where a refined personal ornament is appropriate.
- `medieval_bone_hairpin` - a bone hairpin; noun: `hairpin`; material: `bone`; size/quality: `Small`/`Standard`; weight/cost: 12g/5.0m; wear: `Wear_Headband`; variables: none; matches: broad female, household, town, rural, East Asian, Byzantine, Islamic, and Indian hair-binding variants where a plain pin is useful.
- `medieval_bronze_hairpin` - a bronze hairpin; noun: `hairpin`; material: `bronze`; size/quality: `Small`/`Standard`; weight/cost: 18g/16.0m; wear: `Wear_Headband`; variables: none; matches: Byzantine, Islamic, Indian, Song China, Goryeo Korea, Heian / Kamakura Japan, merchant, and elite commoner hair variants.
- `medieval_silver_hairpins` - a pair of silver hairpins; noun: `hairpins`; material: `silver`; size/quality: `Small`/`Good`; weight/cost: 28g/110.0m; wear: `Wear_Headband`; variables: none; matches: elite Byzantine, Islamic, Indian, Song China, Goryeo Korea, Heian / Kamakura Japan, court, wedding, and formal town variants.
- `medieval_tablet_woven_belt` - a $colour tablet-woven belt; noun: `belt`; material: `wool`; size/quality: `Small`/`Good`; weight/cost: 95g/22.0m; wear: `Wear_Waist`; variables: Variable_FineColour; matches: Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Carolingian / Frankish, Capetian, Anglo-Norman, and prosperous textile-display variants.
- `medieval_decorated_leather_girdle` - a decorated leather girdle; noun: `girdle`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 170g/48.0m; wear: `Wear_Waist`; variables: none; matches: High English / British, Anglo-Norman, Capetian French, Holy Roman Empire / German, Christian Iberian, elite town, and courtly kirtle or gown variants.

#### Travel and rural specialist overlays

- `medieval_travel_wool_cloak` - a weather-stained $colour wool travel cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1150g/24.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_DrabColour; matches: western traveller, pilgrim, road worker, messenger, poorer merchant, shepherd, carter, and cold-road variants across Europe and Rus / Novgorod.
- `medieval_broad_brim_felt_hat` - a broad-brim $colour felt travel hat; noun: `hat`; material: `felt`; size/quality: `Small`/`Standard`; weight/cost: 190g/12.0m; wear: `Wear_Hat`; variables: Variable_BasicColour; matches: western traveller, pilgrim, market-road, herder, messenger, field-worker, and warm-season road variants.
- `medieval_travel_wool_gaiters` - a pair of $colour wool travel gaiters; noun: `gaiters`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 210g/8.0m; wear: `Wear_Leggings`; variables: Variable_BasicColour; matches: western traveller, messenger, carter, pilgrim, fisher, field-labour, and road variants worn over hose, trousers, or leg wraps.
- `medieval_oiled_linen_rain_cape` - an oiled linen rain cape; noun: `cape`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 520g/24.0m; wear: `Wear_Cape`; variables: none; matches: traveller, sailor, fisher, boatman, messenger, watchman, and cool wet-weather variants across western, Byzantine, Islamic, and East Asian settings where local skins support it.
- `medieval_shepherds_hooded_cloak` - a rough $colour shepherd's cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1320g/22.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_BasicColour; matches: shepherd, goatherd, upland herder, rural watch, poor traveller, and cold-field variants across northern, western, Rus, and steppe-edge cultures.
- `medieval_rough_felt_herder_hat` - a rough felt herder's hat; noun: `hat`; material: `felt`; size/quality: `Small`/`Standard`; weight/cost: 160g/8.0m; wear: `Wear_Hat`; variables: none; matches: shepherd, goatherd, steppe herder, Magyar, Rus / Novgorod, rural worker, and upland travel variants.
- `medieval_fisher_headcloth` - a $colour fisher's headcloth; noun: `headcloth`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 75g/5.0m; wear: `Wear_Kerchief`; variables: Variable_BasicColour; matches: fisher, boatman, laundress, dock worker, river trader, coastal servant, and warm-climate water-work variants.
- `medieval_short_boatmans_tunic` - a short $colour boatman's tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 470g/14.0m; wear: `Wear_Tunic`; variables: Variable_BasicColour; matches: fisher, boatman, ferryman, dock worker, river trader, sailor, and short work-tunic variants across western, Byzantine, Islamic, and Rus settings.
- `medieval_rope_soled_hemp_sandals` - a pair of rope-soled hemp sandals; noun: `sandals`; material: `hemp`; size/quality: `Small`/`Standard`; weight/cost: 260g/5.0m; wear: `Wear_Sandals`; variables: none; matches: fisher, boatman, warm-climate field worker, Indian, East Asian, Japanese, coastal, and poorer traveller variants.
- `medieval_leather_hawking_glove` - a single leather hawking glove; noun: `glove`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 170g/36.0m; wear: `Wear_Gloves`; variables: none; matches: forester, hunting retainer, elite rural retinue, falconry-facing court, and outdoor service variants where a single protective glove is appropriate.
- `medieval_rough_wool_gaiters` - a pair of rough $colour wool gaiters; noun: `gaiters`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 230g/6.0m; wear: `Wear_Leggings`; variables: Variable_DrabColour; matches: field labourer, shepherd, stable hand, carter, poor traveller, forester, and wet-weather rural variants.
- `medieval_patched_wool_cloak` - a patched $colour wool cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1040g/14.0m; wear: `Wear_Cloak_(Open)`; variables: Variable_DrabColour; matches: poor traveller, shepherd, field labourer, servant, beggar, rural commoner, and hard-weather common variants.
- `medieval_stablehand_wool_cap` - a $colour stablehand's wool cap; noun: `cap`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 95g/5.0m; wear: `Wear_Hat`; variables: Variable_BasicColour; matches: stable hand, carter, household servant, rural worker, porter, inn worker, and town yard-service variants.
- `medieval_cart_driver_wool_coat` - a belted $colour cart driver's coat; noun: `coat`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 820g/26.0m; wear: `Wear_Long_Coat`; variables: Variable_BasicColour; matches: carter, waggoner, messenger, road worker, stable servant, rural trader, and cool-weather travel variants.

#### Household and service-labour overlays

- `medieval_plain_service_gown` - a plain $colour service gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 700g/18.0m; wear: `Wear_Gown`; variables: Variable_BasicColour; matches: western household servant, castle servant, monastic lay helper, inn worker, kitchen worker, and female or ungendered service variants.
- `medieval_tucked_work_kirtle` - a tucked $colour work kirtle; noun: `kirtle`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 620g/18.0m; wear: `Wear_Gown`; variables: Variable_BasicColour; matches: laundress, kitchen worker, market worker, household servant, western town woman, and active service variants needing a shorter working line.
- `medieval_large_linen_work_apron` - a large $colour linen work apron; noun: `apron`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 180g/6.0m; wear: `Wear_Apron`; variables: Variable_BasicColour; matches: kitchen worker, baker, laundress, household servant, inn worker, market cook, butcher-adjacent service, and workshop-cleanliness variants.
- `medieval_linen_laundry_headcloth` - a $colour laundry headcloth; noun: `headcloth`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 80g/5.0m; wear: `Wear_Kerchief`; variables: Variable_BasicColour; matches: laundress, washerwoman, bathhouse helper, household servant, market washer, and warm-weather service variants.
- `medieval_short_service_tunic` - a short $colour service tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 470g/14.0m; wear: `Wear_Tunic`; variables: Variable_BasicColour; matches: male or ungendered household servant, inn worker, stable hand, porter, kitchen assistant, messenger, and town service variants.
- `medieval_household_work_sash` - a $colour household work sash; noun: `sash`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 110g/5.0m; wear: `Wear_Sash`; variables: Variable_BasicColour; matches: household servant, cook, laundress, inn worker, apprentice, East Asian service, Islamic household, and Byzantine household variants needing a simple waist tie.
- `medieval_kitchen_sleeve_ties` - a pair of $colour kitchen sleeve ties; noun: `sleeve ties`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 45g/3.0m; wear: `Wear_Bracers`; variables: Variable_BasicColour; matches: cook, baker, scullion, laundress, dyer, scribe, workshop, household, and service variants where sleeves need to be visibly gathered.
- `medieval_scullion_linen_coif` - a $colour scullion's linen coif; noun: `coif`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 85g/4.0m; wear: `Wear_Coif`; variables: Variable_BasicColour; matches: scullion, cook, kitchen servant, baker, inn worker, and western household service variants.
- `medieval_leather_stable_apron` - a leather stable apron; noun: `apron`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 420g/18.0m; wear: `Wear_Apron`; variables: none; matches: stable hand, farrier-adjacent service, carter, yard worker, animal handler, and heavy household-labour variants.
- `medieval_inn_workers_wool_hood` - a $colour inn worker's wool hood; noun: `hood`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 230g/10.0m; wear: `Wear_Hoodie`; variables: Variable_BasicColour; matches: inn worker, tavern servant, porter, market helper, kitchen worker, urban servant, and cool-weather service variants.

#### Scholars, students, physicians, and legal-administrative overlays

- `medieval_student_wool_gown` - a plain $colour student gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 760g/28.0m; wear: `Wear_Gown`; variables: Variable_BasicColour; matches: university student, junior scholar, clerk-in-training, Latin Christian school, urban literate, and western town variants.
- `medieval_master_scholar_gown` - a sober $colour scholar's gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 820g/60.0m; wear: `Wear_Gown`; variables: Variable_FineColour; matches: master scholar, lecturer, senior clerk, legal teacher, learned town professional, and formal school variants.
- `medieval_physician_wool_gown` - a long $colour physician's gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 840g/70.0m; wear: `Wear_Gown`; variables: Variable_FineColour; matches: physician, learned healer, court doctor, town medical practitioner, and formal literate-professional variants.
- `medieval_notary_wool_gown` - a neat $colour notary's gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 760g/54.0m; wear: `Wear_Gown`; variables: Variable_FineColour; matches: notary, legal clerk, court administrator, chancery worker, literate town official, and merchant-accounting variants.
- `medieval_lined_scholars_hood` - a lined $colour scholar's hood; noun: `hood`; material: `wool`; size/quality: `Small`/`Good`; weight/cost: 300g/30.0m; wear: `Wear_Hoodie`; variables: Variable_FineColour; matches: student, master scholar, physician, notary, clerk, university, and western learned-professional variants.
- `medieval_plain_black_skullcap` - a plain black skullcap; noun: `skullcap`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 45g/8.0m; wear: `Wear_Skullcap`; variables: none; matches: clerks, scholars, physicians, notaries, Jewish scholar variants where appropriate, and sober learned-professional headwear variants.
- `medieval_sober_lecture_mantle` - a sober $colour lecture mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 720g/52.0m; wear: `Wear_Mantle`; variables: Variable_FineColour; matches: master scholar, lecturer, senior clerk, legal professional, physician, and formal academic or administrative variants.
- `medieval_fine_scholars_cap` - a fine $colour scholar's cap; noun: `cap`; material: `wool`; size/quality: `Small`/`Good`; weight/cost: 80g/28.0m; wear: `Wear_Hat`; variables: Variable_FineColour; matches: master scholar, physician, notary, court administrator, senior clerk, and formal town learned variants.

#### Children and apprentices

- `medieval_child_wool_tunic` - a small $colour child's wool tunic; noun: `tunic`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 300g/8.0m; wear: `Wear_Tunic`; variables: Variable_BasicColour; matches: common child, servant child, rural child, town child, apprentice underlayer, and general child-population variants across most culture families.
- `medieval_child_linen_shift` - a small $colour child's linen shift; noun: `shift`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 150g/5.0m; wear: `Wear_Shirt`; variables: Variable_BasicColour; matches: common child, elite child underlayer, household child, novice child where appropriate, and warm-climate child variants.
- `medieval_child_leather_shoes` - a pair of small leather child's shoes; noun: `shoes`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 230g/10.0m; wear: `Wear_Shoes`; variables: none; matches: common child, elite child, apprentice, household child, town child, and general child-footwear variants.
- `medieval_child_wool_cloak` - a small $colour child's wool cloak; noun: `cloak`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 480g/12.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_BasicColour; matches: common child, elite child, apprentice, travelling child, cold-weather household child, and general child outerwear variants.
- `medieval_apprentice_work_tunic` - a short $colour apprentice's tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 440g/12.0m; wear: `Wear_Tunic`; variables: Variable_BasicColour; matches: apprentice, junior workshop helper, errand runner, town labour youth, stable apprentice, and service-training variants.
- `medieval_apprentice_linen_cap` - a $colour apprentice's linen cap; noun: `cap`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 55g/4.0m; wear: `Wear_Hat`; variables: Variable_BasicColour; matches: apprentice, shop boy, junior servant, errand runner, clerk-in-training, and town youth variants.
- `medieval_child_silk_court_robe` - a small fine $colour silk court robe; noun: `robe`; material: `silk`; size/quality: `Small`/`Good`; weight/cost: 360g/110.0m; wear: `Wear_Robe`; variables: Variable_FineColour; matches: elite child, court child, noble household child, East Asian court child, Byzantine court child, Islamic court child, and Indian court-child variants where local skins support it.
- `medieval_apprentice_canvas_apron` - a $colour apprentice's canvas apron; noun: `apron`; material: `canvas`; size/quality: `Small`/`Standard`; weight/cost: 220g/6.0m; wear: `Wear_Apron`; variables: Variable_BasicColour; matches: apprentice, workshop helper, kitchen trainee, stable apprentice, market youth, and junior craft variants across urban settings.

## Post-implementation headwear and footwear expansion candidates — 44

> These rows are a design expansion beyond the implemented **408**-prototype medieval clothing catalogue. They deliberately use table syntax rather than the implemented catalogue's bullet schema so repository tests and seeder/FDesc parity remain unchanged. When implementation is approved, each row must receive a matching `CreateItem(...)` call and full-description CSV entry before it is moved into the implemented count.

This pass adds options by **construction, use, status, climate, and institution**, not by assigning an arbitrary quota to every culture or outfit. Existing manifests remain valid. Builders may admit these rows as substitutions or optional additions where the local culture, date anchor, social role, route, climate, or institution supports them.

### Headwear candidates — 22

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Admission and implementation note |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `medieval_headwear_quilted_linen_undercap` | a quilted $colour linen undercap | `undercap` | `linen` | `Small` / `Standard` | 120g / 12.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Coif`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Standard Clothing` | Close quilted cap for labour, travel, cold interiors, or beneath another hat or helmet; not armour. |
| `medieval_headwear_chin_tied_wool_riding_cap` | a chin-tied $colour wool riding cap | `cap` | `wool` | `Small` / `Standard` | 180g / 18.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Standard Clothing` | Soft cap with integral chin ties for riders, couriers, carters, and windy upland travel. |
| `medieval_headwear_brimmed_straw_field_hat` | a broad-brimmed straw field hat | `hat` | `straw` | `Small` / `Standard` | 220g / 14.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Reflector` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Work Clothing` | Light field and fishing hat for warm, exposed work; brim width and weave are skins. |
| `medieval_headwear_short_brim_felt_town_hat` | a short-brimmed $colour felt town hat | `hat` | `felt` | `Small` / `Good` | 260g / 36.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Standard Clothing` | Structured urban hat distinct from the broad travel hat; suitable for artisans, shopkeepers, and prosperous townspeople. |
| `medieval_headwear_tall_conical_felt_court_cap` | a tall $colour felt court cap | `cap` | `felt` | `Small` / `Good` | 240g / 60.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Luxury Clothing` | Tall conical or gently tapering cap for Byzantine, Islamicate, Caucasian, steppe-facing, or courtly admissions; exact shape is locally gated. |
| `medieval_headwear_folded_silk_court_cap` | a folded $colour silk court cap | `cap` | `silk` | `Small` / `Good` | 95g / 84.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Luxury Clothing` | Soft folded cap for court, scholarly, diplomatic, or household-elite dress where the local silhouette supports it. |
| `medieval_headwear_silk_hair_caul` | a netted $colour silk hair caul | `caul` | `silk` | `Small` / `Good` | 55g / 72.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Luxury Clothing` | Close netted hair covering for prosperous and elite women; beads, pearls, and metal thread remain skins or separate jewellery. |
| `medieval_headwear_linen_fillet_veil` | a $colour linen fillet and veil | `veil` | `linen` | `Small` / `Good` | 130g / 42.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Head_Veil`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Standard Clothing` | Coordinated narrow head band and short veil authored as one stable silhouette; use only where the combined construction is customary. |
| `medieval_headwear_pleated_linen_headcloth` | a pleated $colour linen headcloth | `headcloth` | `linen` | `Small` / `Standard` | 150g / 24.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Head_Veil`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_BasicColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Standard Clothing` | Folded and pleated headcloth for married-status, market, travel, or warm-climate dress across locally admitted contexts. |
| `medieval_headwear_padded_turban_cap` | a padded $colour turban cap | `turban` | `cotton` | `Small` / `Standard` | 260g / 38.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Turban`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Standard Clothing` | Compact padded cap-and-winding for riding, travel, winter, or service dress; it carries no armour claim. |
| `medieval_headwear_low_cylindrical_felt_cap` | a low cylindrical $colour felt cap | `cap` | `felt` | `Small` / `Good` | 180g / 36.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Standard Clothing` | Low flat-topped felt cap for Byzantine, Islamicate, Mediterranean urban, merchant, and learned settings; exact height and trim require local admission. |
| `medieval_headwear_peaked_felt_riding_cap` | a peaked $colour felt riding cap | `cap` | `felt` | `Small` / `Standard` | 210g / 28.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Standard Clothing` | Forward-peaked cap for mounted travel, herding, and caravan work in Magyar, steppe, Rus, or frontier contexts. |
| `medieval_headwear_fur_brimmed_round_cap` | a fur-brimmed $colour wool cap | `cap` | `wool` | `Small` / `Good` | 320g / 72.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Strong`<br>`Variable_FineColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Luxury Clothing` | Round wool cap with a distinct fur brim for northern merchants, court retainers, and winter travellers. |
| `medieval_headwear_broad_fur_winter_hood` | a broad $colour fur-lined winter hood | `hood` | `wool` | `Small` / `Good` | 520g / 90.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hood`<br>`Armour_LightClothing`<br>`Insulation_Strong`<br>`Variable_FineColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Standard Clothing` | Separate deep hood covering head, neck, and shoulders for hard winter travel; fur species is a skin or local material decision. |
| `medieval_headwear_waxed_leather_seafarer_cap` | a close waxed leather seafarer cap | `cap` | `leather` | `Small` / `Standard` | 230g / 30.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Maritime Clothing` | Close cap for fishers, ferrymen, and shipboard labour. Waxed presentation is descriptive and does not grant waterproof mechanics. |
| `medieval_headwear_leather_road_hood` | a close brown leather road hood | `hood` | `leather` | `Small` / `Standard` | 380g / 34.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hood`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Work Clothing` | Close chin-tied hood for riders, couriers, pilgrims, ferrymen, and hard road travel; dressed leather is visible construction and grants no waterproof mechanic. |
| `medieval_headwear_stiffened_official_cap` | a stiffened $colour cloth official cap | `cap` | `silk` | `Small` / `Good` | 170g / 96.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Ceremonial Clothing` | Structured cap for court, chancery, judicial, or ceremonial office. Exact office and appendages require a local skin and manifest. |
| `medieval_headwear_woven_bamboo_rain_hat` | a broad woven bamboo rain hat | `hat` | `bamboo` | `Small` / `Standard` | 340g / 24.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Reflector` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Work Clothing` | Broad conical or shallow woven hat for field work, river travel, and rain in East Asian admissions; no waterproof mechanic. |
| `medieval_headwear_broad_sedge_travel_hat` | a broad woven travel hat | `hat` | `straw` | `Small` / `Standard` | 280g / 24.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Reflector` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Work Clothing` | Wide tied travel hat for Japanese road, messenger, pilgrim, or rural contexts; exact crown and ties are skins. |
| `medieval_headwear_black_gauze_scholar_hat` | a black gauze scholar hat | `hat` | `silk` | `Small` / `Good` | 110g / 100.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hat`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Ceremonial Clothing` | Light structured scholar or official hat for East Asian court and learned settings; wings or rank appendages need distinct skins or later prototypes. |
| `medieval_headwear_veiled_silk_court_cap` | a veiled $colour silk court cap | `cap` | `silk` | `Small` / `Good` | 160g / 100.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Head_Veil`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Luxury Clothing` | Fitted court cap with an integral short veil for locally admitted Byzantine, Islamicate, and Mediterranean elite women; jewels and borders remain skins. |
| `medieval_headwear_long_tailed_wool_hood` | a long-tailed $colour wool hood | `hood` | `wool` | `Small` / `Good` | 360g / 40.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Hood`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_FineColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Headwear`<br>`Market / Clothing / Standard Clothing` | Late-thirteenth-century elongated hood for western and central European town, student, travel, and court-facing dress; date-gate it near 1250-1300 rather than treating it as an early-medieval default. |

### Footwear candidates — 22

| Stable reference | SDesc | Noun | Material | Size / quality | Weight / cost | Components | Tags | Admission and implementation note |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `medieval_footwear_thick_soled_work_shoes` | a pair of thick-soled leather work shoes | `shoes` | `leather` | `Small` / `Standard` | 620g / 30.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Work Clothing` | Stout closed shoes for farm, workshop, stable, and building labour; sole reinforcement is visible construction only. |
| `medieval_footwear_strapped_walking_shoes` | a pair of strapped leather walking shoes | `shoes` | `leather` | `Small` / `Standard` | 480g / 24.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Standard Clothing` | Low walking shoes closed by one or more instep straps, useful for town, pilgrimage, and ordinary travel. |
| `medieval_footwear_side_laced_ankle_shoes` | a pair of side-laced leather ankle shoes | `shoes` | `leather` | `Small` / `Good` | 520g / 42.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Luxury Clothing` | Close ankle-height shoes with visible side lacing, suited to prosperous urban, court-servant, and riding-adjacent dress. |
| `medieval_footwear_turn_cuffed_riding_boots` | a pair of turn-cuffed leather riding boots | `boots` | `leather` | `Small` / `Good` | 980g / 90.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_High_Boots`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Luxury Clothing` | High riding boots with a distinct turned cuff; cavalry, courier, hunting, and elite travel admission. |
| `medieval_footwear_front_laced_riding_boots` | a pair of front-laced leather riding boots | `boots` | `leather` | `Small` / `Standard` | 900g / 64.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Boots`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Standard Clothing` | Calf-height riding and travel boots with front lacing, distinct from pull-on high boots. |
| `medieval_footwear_waxed_seafarer_shoes` | a pair of waxed leather seafarer shoes | `shoes` | `leather` | `Small` / `Standard` | 560g / 34.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_HeavyClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Maritime Clothing` | Close, low-profile shoes for decks, boats, docks, and fisheries. Waxed leather is descriptive and provides no waterproof mechanic. |
| `medieval_footwear_rope_soled_walking_shoes` | a pair of rope-soled canvas walking shoes | `shoes` | `canvas` | `Small` / `Standard` | 420g / 18.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Work Clothing` | Flexible closed or partly closed walking shoes with plaited rope soles for warm, dry travel and labour. |
| `medieval_footwear_felt_overshoes` | a pair of $colour felt overshoes | `overshoes` | `felt` | `Small` / `Standard` | 420g / 24.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Overshoes`<br>`Armour_LightClothing`<br>`Insulation_Strong`<br>`Variable_BasicColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Standard Clothing` | Slip-over felt footwear worn above shoes or footwraps for snow, cold floors, and winter travel. |
| `medieval_footwear_strapped_wooden_pattens` | a pair of strapped wooden pattens | `pattens` | `wood` | `Small` / `Standard` | 780g / 30.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Overshoes`<br>`Armour_HeavyClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Standard Clothing` | Raised wooden overshoes for mud, wet streets, yards, bath approaches, and foul work surfaces; admission is strongest near the later medieval boundary. |
| `medieval_footwear_fur_lined_winter_shoes` | a pair of fur-lined leather winter shoes | `shoes` | `leather` | `Small` / `Good` | 700g / 72.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Strong` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Luxury Clothing` | Low winter shoes lined for warmth where tall boots are impractical indoors or in settled town life. |
| `medieval_footwear_embroidered_court_slippers` | a pair of fine $colour embroidered court slippers | `slippers` | `silk` | `Small` / `Good` | 260g / 96.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Luxury Clothing` | Soft indoor or ceremonial footwear for court and elite households; embroidery and metallic thread are skin presentation. |
| `medieval_footwear_felt_house_shoes` | a pair of $colour felt house shoes | `shoes` | `felt` | `Small` / `Standard` | 320g / 22.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Moderate`<br>`Variable_BasicColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Standard Clothing` | Low felt shoes for cold interiors, religious houses, learned rooms, and settled winter town life; the felt body distinguishes them from leather slippers. |
| `medieval_footwear_laced_pilgrim_halfboots` | a pair of laced leather pilgrim half-boots | `boots` | `leather` | `Small` / `Standard` | 720g / 42.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Boots`<br>`Armour_HeavyClothing`<br>`Insulation_Moderate` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Religious Clothing`<br>`Institution / Religious` | Ankle-to-calf road boots with visible lacing for shrine routes, mendicant travel, messengers, and other long journeys; badges and religious identity remain separate. |
| `medieval_footwear_raised_wooden_bath_sandals` | a pair of raised wooden bath sandals | `sandals` | `wood` | `Small` / `Standard` | 650g / 32.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Standard Clothing` | Raised direct-wear sandals for bathhouses, warm urban interiors, and elite households in locally admitted Mediterranean, Byzantine, and Islamicate settings. |
| `medieval_footwear_pointed_urban_slippers` | a pair of pointed soft leather slippers | `slippers` | `leather` | `Small` / `Good` | 340g / 48.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Luxury Clothing` | Pointed soft slippers for Islamicate and eastern Mediterranean urban, learned, merchant, and household dress. |
| `medieval_footwear_curved_toe_court_shoes` | a pair of curved-toe $colour court shoes | `shoes` | `leather` | `Small` / `Good` | 420g / 84.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor`<br>`Variable_FineColour` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Luxury Clothing` | Curved-toe court or prosperous urban shoes for later Seljuk, north Indian, and connected luxury markets; date and local form are gated. |
| `medieval_footwear_toe_post_wooden_sandals` | a pair of carved toe-post wooden sandals | `sandals` | `wood` | `Small` / `Good` | 520g / 46.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Luxury Clothing` | Toe-post wooden sandals for South Asian household, temple, scholar, and elite settings; sole height is a skin within compatible coverage. |
| `medieval_footwear_black_cloth_official_boots` | a pair of black cloth official boots | `boots` | `cotton` | `Small` / `Good` | 620g / 72.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Boots`<br>`Armour_LightClothing`<br>`Insulation_Moderate` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Ceremonial Clothing` | High cloth boots for East Asian official, scholar, and ceremonial dress; exact sole and office admission are locally controlled. |
| `medieval_footwear_woven_straw_field_shoes` | a pair of woven straw field shoes | `shoes` | `straw` | `Small` / `Standard` | 240g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Work Clothing` | Low woven field and road shoes for East Asian farmers, porters, travellers, and poorer urban labour. |
| `medieval_footwear_cork_soled_leather_sandals` | a pair of cork-soled leather sandals | `sandals` | `cork` | `Small` / `Standard` | 300g / 18.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Standard Clothing` | Light sandals built around a thick cork sole for warm Mediterranean, North African, and coastal town or road use; strap shape remains a skin. |
| `medieval_footwear_tied_straw_travel_sandals` | a pair of tied straw travel sandals | `sandals` | `straw` | `Small` / `Standard` | 230g / 10.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Sandals`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Work Clothing` | Tied sandals with cords around foot and ankle for Japanese roads, pilgrimage, field work, and messengers. |
| `medieval_footwear_gathered_rawhide_shoes` | a pair of gathered rawhide shoes | `shoes` | `rawhide` | `Small` / `Standard` | 380g / 20.0m | `Holdable`<br>`Destroyable_Clothing`<br>`Wear_Shoes`<br>`Armour_LightClothing`<br>`Insulation_Minor` | `Era / Medieval Era`<br>`Functions / Worn Items / Footwear`<br>`Market / Clothing / Standard Clothing` | Soft closed shoes gathered around the foot for forest, steppe-edge, hunting, and cold-ground contexts; every admission needs a narrower local anchor. |

### Expansion admission rules

- Do not add every option to every outfit. Use these rows to distinguish labour, ordinary town wear, court and learned dress, riding, road travel, winter, maritime work, and religious residence where the distinction is visible.
- A skin may change colour, trim, decorative stitching, local terminology, or minor fastening presentation. It may not turn a low shoe into a boot, a cap into a brimmed hat, an ordinary shoe into an overshoe, or an unlined item into a mechanically warmer item.
- `medieval_footwear_strapped_wooden_pattens` is strongest near the later edge of the 500-1300 band and should not be treated as universal early-medieval footwear.
- East Asian, South Asian, Islamicate, steppe, forest, and maritime rows require the same narrower local-admission discipline as the existing culture families.
- Full descriptions remain deferred until implementation; the present rows are sufficient for design review, stable-reference reservation, dependency checking, and later seeder authoring.

## Validation target

- The 44 post-implementation expansion references are unique, use exact live materials/components/tags, and remain outside the 408-row implemented count until code and FDesc parity are supplied.
- Expansion rows must not be converted to the implemented bullet schema piecemeal; implementation should move the complete approved batch with matching seeder and full-description updates.

- The target output contains no hidden or non-skinnable items.
- All target items use `null` for long description, morph target, morph emote, morph timer, and destroyed-item reference.
- The item catalogue uses exact seeded solid material names for primary materials.
- The item catalogue uses exact seeded wearable and variable component names where those components are listed.
- Public text avoids explicit skin/customization language and avoids culture labels in item names, short descriptions, and full descriptions.
- Outfit manifests reference only item catalogue entries in this document.
- Cold-weather additions are catalogue overlays with `matches:` notes; they are not mandatory default manifest entries unless a later implementation intentionally promotes them into a manifest.
- Urban and professional additions are catalogue overlays with `matches:` notes; they are not mandatory default manifest entries and should not imply formal uniforms, badges, hidden storage, or guild mechanics unless later supported by components or skins.
- Urban and professional additions distinguish town workers from rural commoners through cleaner headwear, work aprons, sleeve guards, work caps, short tunics or jackets, merchant and clerk robes, and culture-specific workshop garments.
- Religious clothing additions are shared tradition overlays and should not be duplicated as culture-specific priest, monk, nun, scholar, or ritual-specialist garments unless a later setting requires a genuinely different local form.
- Religious clothing additions intentionally omit non-clothing religious paraphernalia, carried ritual objects, books, rosaries, beads, staves, bowls, masks, instruments, sect marks painted on the body, and mechanically meaningful ritual equipment.
- Accessory and social-context additions are shared overlays with `matches:` notes. They should be used to distinguish road travellers, rural specialists, household servants, literate professionals, children, apprentices, and visibly accessorised outfits without duplicating every culture family.
- This pass intentionally omits wearable containers and container-adjacent clothing: pouches, purses, alms bags, travel scrips, satchels, backpacks, baskets, tool bags, and storage aprons are left for a separate container patch. Belts, sashes, girdles, pins, and brooches must not claim storage or mechanically meaningful attachment behaviour unless later implementation adds a supporting component.

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

- Religious expansion source: Catholic Encyclopedia, `Vestments` - used for the Latin liturgical set, including amice, alb, cincture, maniple, stole, dalmatic, chasuble, cope, bishop gloves, mitre, and archbishop pallium.
- Religious expansion source: Catholic Encyclopedia, `Pontifical Mass` - used as a check on bishop and papal-style pontifical layering, especially the compatibility of alb, cincture, stole, dalmatic, gloves, chasuble, pallium, and mitre.
- Religious expansion source: Dominican Friars / Order of Preachers habit descriptions - used for the white habit, scapular/capuce, black cappa, and the black-friar visual distinction; rosary references were intentionally excluded as non-clothing.
- Religious expansion source: Franciscan habit and Carmelite habit summaries - used for the cord-belt, grey/brown mendicant habit, and white-mantled Carmelite variants while avoiding late or modern order-specific accessories.
- Religious expansion source: Cistercian habit summaries - used for the white or grey habit with black scapular distinction and the c. 1098 Cistercian reform context.
- Religious expansion source: Orthodox vestment guides and OrthodoxWiki / Greek Orthodox sources - used for sticharion, orarion, epitrachelion, epimanikia, phelonion, sakkos, omophorion, kamilavkion, and monastic black-robe assumptions.
- Religious expansion source: Encyclopaedia Iranica and Britannica on `khirqa` - used for the Sufi patched wool cloak/robe and the caution that colour and exact shape were not uniform.
- Religious expansion source: Jainpedia, `Monastic clothing` and `Monastic equipment` - used for white Śvetāmbara mendicant robe assumptions and the decision to omit Digambara male ascetic clothing prototypes.
- Religious expansion source: Britannica and museum/object sources on tallit and kesa/kasaya - used for Jewish tallit/tallit katan/fringe assumptions and Buddhist rectangular patched mantle assumptions.
- Religious expansion source: Nippon.com, Britannica, and museum records on Shintō vestments and kariginu - used for Heian-derived shrine clothing assumptions, including white jōe-style robe, hakama, and eboshi headwear.
- Religious expansion source: World History Encyclopedia, `Clothing in the Mongol Empire` - used conservatively for steppe felt, fur, long coat, and hat materials in the steppe ritual-specialist overlay.

- Accessory/social-context expansion source: Egan and Pritchard / Museum of London-style medieval dress-accessory catalogues - used cautiously for brooches, cloak pins, belts, rings, earrings, hairpins, garters, and small worn jewellery, with later high-medieval material kept conservative for the 500AD-1300AD seeder window.
- Accessory/social-context expansion source: the existing Viking, Rus, steppe, Indian, Islamic, and East Asian textile source notes - reused for oval brooches, bead necklaces, tablet-woven bands, metal-fitted belts, robe sashes, waist cords, bangles, hairpins, and warm-climate jewellery without making culture labels player-facing.
- Travel, service, and rural-specialist expansion source: the existing winter, urban/professional, and manuscript-iconography source base - used for road cloaks, broad-brim hats, gaiters, hooded shepherd cloaks, fisher headcloths, boatman tunics, stable aprons, sleeve ties, kitchen aprons, and service coifs as practical social-context clothing rather than uniformed occupational dress.
- Scholar, child, and apprentice expansion source: medieval urban, clerical, and university clothing assumptions already present in the clerk, scholar, and religious sections - used for sober gowns, scholar hoods, skullcaps, apprentice tunics, child tunics, small cloaks, and elite child robe variants without adding books, badges, seals, degree regalia, or other paraphernalia.

## Appendix A: colour variable value lists

These lists are included to preserve the design distinction between ordinary, fine, and deliberately degraded colour vocabularies.

### basic_colours

black, white, grey, light grey, dark grey, red, dark red, blue, dark blue, green, brown, dark green, orange, light blue, light green, yellow, light red, purple, pink

### fine_colours

light grey, dark grey, red, dark red, blue, dark blue, green, brown, dark green, pale white, olive, caramel, ebony, emerald green, cerulean, violet, sandy brown, light brown, dark brown, auburn, onyx, obsidian, midnight black, ink black, jet black, pitch black, ivory, seashell, snow white, gleaming white, pure white, pearl white, bright white, bone white, ghost white, mist grey, charcoal grey, thistle grey, smoky grey, slate grey, silver grey, soft grey, ash grey, crimson, scarlet, ruby red, blood red, rose red, wine red, flame red, coral, copper, fiery orange, ochre, sunset orange, amber, goldenrod, pale yellow, golden yellow, sand yellow, topaz hued, gold-coloured, spring green, sea green, hunter green, olive green, sage green, pine green, bright green, rich green, pale green, verdant green, forest green, chartreuse, slate blue, bright blue, powder blue, sapphire blue, royal blue, ocean blue, teal, cornflour blue, sky blue, azure, beryl, cobalt, rich indigo, deep indigo, vivid indigo, earthen brown, deep brown, rich brown, burnt sienna, chocolate, cinnamon, mahogany, nut brown, umber, amethyst, mauve, mulbery, plum, lavender, royal purple, orange, light blue, light green, pale blue, yellow, cyan, navy blue, reddish brown, beige

### drab_colours

faded black, tattered black, shabby black, grimy black, off-white, dingy grey, bland yellow, faded green, faded blue, faded indigo, drab brown, dim grey, dusky slate grey, sooty grey, chalky pale grey, dull mist grey, ashen off-white, dirty bone-white, wan ivory, spotted white, stained white, blotched white, dingy off-white, stained ivory, shabby sallow-coloured, lurid pale yellow, dingy yellow, gaudy mustard yellow, sickly pale yellow, shabby pale yellow, murky brown, stained brown, dreary brown, bland brown, spotted muddy brown, dismal sand brown, dreary beige, grimy beige, shabby beige, dirty beige, tattered beige, bland wheat-coloured, drab olive, murky olive, dim olive, dingy green, shabby green, dull green, sickly greyish-green, grisly brownish-green, discoloured green, blotchy green, grimy rust-red, blotchy rust-red, grimy salmon, stained salmon, blotched red, dull red, faded red, stained red, dingy red, faded salmon, well-worn blue, faded slate blue, pallid blue, stained blue, grimy blue, dim blue-black, faded blue-black, dreary blue-black, dull orange, faded reddish-orange, tattered reddish-orange, discoloured orange, stained orange-red, drab peach-coloured, lurid peach-coloured, sickly peach-coloured, tattered violet, grimy lavender, spotted lavender, discoloured purple, dirty purple, dingy purple, faded purple, stained purple, dusty faded purple

