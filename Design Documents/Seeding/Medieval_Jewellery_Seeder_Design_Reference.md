# FutureMUD Medieval Jewellery Seeder Design Reference

This document defines the research framing, scope, component strategy, culture coverage, item-category plan, and first-pass target catalogue architecture for decorative medieval jewellery in the FutureMUD Item Seeder.

It is intentionally focused on **decorative personal adornment**. Religious furnishings, devotional containers, ritual supplies, scripture supports, reliquaries, prayer tools, pilgrim badges, explicitly sacred amulets, and other devotional/religious goods belong to the existing religious-goods branch or a later devotional-personal-items branch. The jewellery branch may include socially meaningful, heraldic, civic, courtly, family, guild, romantic, seasonal, and festival jewellery, but its base prototypes should not encode religious function or sacred text.

## Executive summary

- Total target unique item prototypes for the first full catalogue: **about 400**.
- This pass prepares the design reference for later item creation. It does not yet enumerate every final `CreateItem(...)` row.
- The catalogue should cover commoner adornment, urban and merchant jewellery, professional and guild display pieces, elite and high-noble jewellery, courtly display jewellery, and short-lived organic adornment such as fresh garlands, leaf wreaths, herb bracelets, and festival flower strings.
- The branch uses the same 500AD-1300AD medieval world slice as the clothing, household, writing, and military references.
- The branch should be **shared-first**. Regional items are justified where the visible form, body slot, material, social use, or craft tradition meaningfully differs. Skins should carry many local motifs and exact cultural names.
- All ordinary jewellery items are finished goods, skinnable, player-visible, and portable. Most should include `Holdable`, one appropriate wearable component where one exists, and one appropriate destroyable component.
- Unlike ordinary clothing, jewellery should not receive `Armour_*` or `Insulation_*` components unless a later mechanical design explicitly creates protective jewellery. Decorative jewellery must not claim armour, concealment, magic, storage, lock, or seal-authority behaviour unless the components support it.
- Current seeded components strongly support rings, necklaces, chokers, bracelets, armlets, anklets, earrings, nose rings, toe rings, and headband-like ornaments. Current seeded components do **not** directly support brooches, cloak pins, garment badges, hairpins, combs, diadems, crowns, chaplets, or garlands as distinct wear slots; this document gives conservative implementation rules for those gaps.
- Short-lived fresh organic jewellery should use timed morph targets where authored: fresh garlands and herb wreaths can morph into wilted, dried, or faded variants.
- The first complete catalogue should deliberately include cheap and temporary pieces. Medieval jewellery should not be only noble gemstone jewellery; ordinary adornment, festival display, keepsakes, trade beads, copper rings, shell strings, bone beads, and coloured glass are important for social texture.

## Scope and era model

- Chronological band: **500AD to 1300AD**.
- Geographic coverage: Britain and Ireland; Scandinavia and the North Sea; western, central, and southern Europe; Iberia; Byzantium; the Levant; Egypt and North Africa; the Eurasian steppe; Rus/Novgorod; northern and southern India; China; Korea; and Japan.
- Historical inspiration families: Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Norman, Anglo-Norman, High English / British, Irish / Gaelic, Scottish / Gaelic-Lowland, Carolingian / Frankish, Capetian French, Holy Roman Empire / German, Christian Iberian, Andalusian, Byzantine, Abbasid, Fatimid, Seljuk / Ayyubid-Mamluk, Magyar, Rus / Novgorod, Steppe Turkic / Mongol, North Indian / Rajput, South Indian / Chola, Song China, Goryeo Korea, Heian / Kamakura Japan. These labels are builder-facing organizational buckets, not public item wording.
- Resolution: standard jewellery prototypes rather than museum-taxonomy subtypes. The default rows should be credible as unskinned objects; skins can provide local fantasy names, exact motifs, owner marks, heraldry, enamel patterning, gemstone choices, workshop marks, rank marks, dynastic signs, textile colours, and world-specific naming.
- Culture labels are used to ensure design coverage. They should not be mechanically inserted into `noun`, `sdesc`, or `fdesc` unless the object term itself is useful and period-appropriate.
- Coverage deliberately excludes late medieval and Renaissance jewellery forms that primarily belong after 1300, such as mature Tudor and later early-modern pendant forms, post-medieval gimmal-ring fashion, Renaissance hat jewels, elaborate late chivalric livery badges, and modern jewellery categories.

### Culture coverage table

| Inspiration family | Reference anchor | Coverage boundary | Jewellery-design focus |
|---|---:|---|---|
| Early Anglo-Saxon | c. 800AD | lowland England before sustained Anglo-Danish synthesis | bead strings, simple finger rings, disc brooches and pins as inert or future wearable items, glass beads, amber, bone, bronze, silver, and elite garnet-like display |
| Anglo-Danish | c. 950AD | late Anglo-Saxon England under strong Scandinavian settlement influence | mixed North-Sea bead strings, arm rings, rings, strap ornaments, brooch-like fasteners, silver display, amber, glass, and imported beadwork |
| Norse / Viking Age | c. 950AD | Scandinavia and diaspora settlements before the close of the Viking Age | paired brooch silhouettes as component-gap items, bead festoons, arm rings, neck rings, finger rings, amber, glass, silver, bronze, and hack-silver-like display |
| Norman | c. 1066AD | ducal Normandy and conquest-generation Norman sphere | simple rings, ring brooches, cloak pins, belt fittings, modest silver/gilt display, and elite gems without later court extravagance |
| Anglo-Norman | c. 1150AD | post-Conquest England and Norman-influenced Britain | annular brooches, small rings, seal-like rings, courtly necklaces, girdle ornaments, enamel-like colour language, and urban merchant jewellery |
| High English / British | c. 1250AD | high medieval England and Welsh March/British court-facing styles | signet rings, gem rings, annular brooches, jewelled belts, chains, circlets, courtly pins, civic badges, and fine silver or gold merchant jewellery |
| Irish / Gaelic | c. 1100AD | Gaelic Ireland before strong late-medieval English tailoring influence | penannular and ring-brooch families as component-gap items, cloak pins, amber and glass beads, silver rings, torc-like collars where conservative, and elite interlace-ready display |
| Scottish / Gaelic-Lowland | c. 1250AD | medieval Scotland across Gaelic and Lowland spheres | brooches and cloak pins, ring pins, simple rings, amber and glass, silver display, and Lowland court jewellery overlapping western European forms |
| Carolingian / Frankish | c. 800AD | Frankish court and common household practice under Carolingian influence | brooches, belt ornaments, glass beads, bronze and silver rings, elite gold, garnet-like stones, and courtly necklace or pendant forms kept non-devotional |
| Capetian French | c. 1200AD | northern and central French high medieval sphere | courtly rings, chains, circlets, annular brooches, gem settings, enamel-like colour, gilded silver, gold, and noble love or household tokens |
| Holy Roman Empire / German | c. 1200AD | German-speaking imperial regions and neighbouring central European towns | rings, brooches, chains, civic and guild ornaments, belt plaques, silver and goldsmith display, and merchant-class jewellery |
| Christian Iberian | c. 1200AD | Leonese, Castilian, Aragonese, Navarrese, and neighbouring Christian Iberian contexts | rings, earrings, filigree-like metalwork, court necklaces, belt ornaments, glass and precious stones, and western-Islamicate overlap without religious coding |
| Andalusian | c. 1100AD | al-Andalus and western Islamic Iberia | earrings, bracelets, anklets, necklaces, filigree-like work, coloured glass, pearls, gold, silver, and urban luxury jewellery |
| Byzantine | c. 1000AD | middle Byzantine Constantinopolitan and provincial contexts | gold rings, earrings, necklaces, pearl and glass display, enamel-like panels, courtly chains, bracelets, and high-status ornament that stays decorative rather than liturgical |
| Abbasid | c. 850AD | Baghdad-centred early medieval Islamic urban and scholarly contexts | urban luxury rings, earrings, bracelets, anklets, filigree-like and granulated-looking metalwork, pearls, glass, gold, silver, and merchant display |
| Fatimid | c. 1050AD | Fatimid Egypt and North African urban/textile contexts | gold earrings, bracelets, bead strings, anklets, pearls, glass, and fine urban jewellery suitable for warm-climate textile outfits |
| Seljuk / Ayyubid-Mamluk | c. 1200AD | Anatolia, Syria, Egypt, and crusading-era eastern Mediterranean | rings, armlets, bracelets, anklets, earrings, pendants, belt ornaments, turquoise-like and carnelian-like colour, gold, silver, and mounted-elite display |
| Magyar | c. 950AD | Carpathian Basin Magyar and steppe-to-central-Europe context | belt ornaments, temple-ring-like ornaments as component-gap items, earrings, silver rings, pendants, beads, and mobile elite display |
| Rus / Novgorod | c. 1100AD | Kievan Rus and Novgorod-facing northern Slavic/Norse-Byzantine intersections | temple rings as component-gap items, bead strings, pendant necklaces, rings, bracelets, silver, glass, amber, and Byzantine-influenced elite pieces |
| Steppe Turkic / Mongol | c. 1200AD | Inner Eurasian mounted steppe before and during early imperial Mongol expansion | belt plaques, hair and temple ornaments as component-gap items, silver earrings, rings, bead strings, amulet-like shapes kept non-magical, and portable elite display |
| North Indian / Rajput | c. 1100AD | north and north-western Indian contexts before heavy late Sultanate/Mughal tailoring dominance | bangles, anklets, toe rings, nose rings, earrings, necklaces, head ornaments, gold, silver, pearls, glass, and fresh festival garlands |
| South Indian / Chola | c. 1050AD | Chola and neighbouring south Indian temple and court environments | gold necklaces, bangles, anklets, toe rings, ear ornaments, head ornaments, pearls, gems, flowers, and warm-climate court or festival adornment |
| Song China | c. 1100AD | Northern and Southern Song domestic, urban, and court baseline | hairpins, combs, head ornaments as component-gap items, jade and gold ornaments, earrings where useful, bead strings, pendants, restrained court pieces, and scholar-household display |
| Goryeo Korea | c. 1150AD | Goryeo-period Korean court, Buddhist, and scholarly baseline | hairpins and head ornaments as component-gap items, rings, pendants, beads, jade-like stones, gilt metal, and restrained elite display |
| Heian / Kamakura Japan | c. 1200AD | late Heian to early Kamakura court, temple, and warrior contexts | hair ornaments and combs as component-gap items, cords, bead strings, understated court ornaments, metal and lacquer-like visual treatment, and limited ring or earring use unless setting skins justify it |

### Religious and devotional boundary table

| Object family | Catalogue treatment | Notes and limits |
|---|---|---|
| Decorative rings, necklaces, bracelets, anklets, earrings, chains, circlets, bead strings, and garlands | In scope | These are the core of this branch when their base purpose is adornment, status, fashion, affection, household identity, festival display, or secular authority. |
| Secular signet rings and seal-like rings | In scope with caution | A non-functional decorative signet ring can be ordinary jewellery. A mechanically functional seal ring should only use a `SealStamp_*` component if component compatibility is confirmed; otherwise use writing-package seal tools. |
| Brooches, cloak pins, badges, and dress fasteners | In scope as decorative forms, but component-limited | These are historically important but currently lack exact wear-slot components. Implement as inert portable jewellery or wait for `Wear_Brooch` / `Wear_Pin` / `Wear_Badge` if mechanical wearing is required. |
| Hairpins, combs, temple rings, diadems, crowns, and garlands | In scope, but component-limited | Use `Wear_Headband` or `Wear_Hat` only where the fit is acceptable. Otherwise keep as inert ornaments until a better wear-slot component exists. |
| Pilgrim badges, prayer beads, rosaries, relic pendants, reliquary lockets, explicitly sacred amulets, votive jewellery, scripture-inscribed charms | Out of scope for this branch | These belong to the devotional/religious branch or later personal-devotional branch. Do not duplicate them here merely because they are wearable. |
| Magical amulets, protective talismans, curse rings, healing gems, luck charms, disguise jewels | Out of scope by default | Decorative forms may exist, but no base row should imply magical or trait-changing behaviour unless a later fantasy mechanics branch approves it. |
| Coins, bullion, raw gemstones, jewellery wire, loose settings, bead stock | Mostly out of scope | Finished jewellery belongs here. Raw craft stock belongs under material/crafting supply branches and may use `Functions / Material Functions / Jewellery Craft Stock` tags. |

### Social and lifecycle coverage table

| Social band or lifecycle | Catalogue intent | Example public forms |
|---|---|---|
| Rural and commoner adornment | Cheap, local, repairable, improvised, and seasonal pieces | copper ring, bone bead string, wooden bead necklace, shell bracelet, braided cord anklet, herb wreath, leaf garland |
| Urban commoner and apprentice adornment | Slightly neater but still inexpensive goods | coloured glass necklace, brass bracelet, pewter ring, ribbon choker, small glass earrings, simple festival circlet |
| Merchant and craft-guild display | Durable, visible, respectable jewellery with secular status | silver ring, stamped signet ring, merchant chain, guild-coloured bead string, brass belt plaque string, enamel-coloured brooch |
| Lesser gentry and professional elites | Fine pieces without royal extravagance | silver-gilt necklace, garnet-like ring, pearl earrings, fine armlet, gem-set bracelet, decorative girdle ornament |
| High nobility and court display | Rare, expensive, symbolic, and high-craft pieces | gold circlet, gemstone collar, jewelled chain, pearl necklace, heavy gold ring, fine enamel-like bracelet |
| Festival, courtship, wedding, and household gift jewellery | Socially meaningful but not automatically religious | love-knot ring, paired bracelets, wreath, token necklace, household-colour bead string, bridal garland |
| Ephemeral organic adornment | Short-lived pieces with morph targets where useful | fresh flower garland, herb bracelet, leaf crown, blossom anklet, festival wreath, dried garland |
| Child and apprentice jewellery | Smaller, cheaper, safer-feeling pieces | tiny copper ring, wooden bead necklace, cord bracelet, shell anklet, coloured glass bead string |

## Shared-first branch architecture

### Default rule

Create shared prototypes wherever the object can plausibly serve many cultures with only presentation changes. Most simple rings, copper rings, brass rings, silver rings, bead necklaces, glass bead strings, bone bracelets, shell bracelets, simple earrings, fresh garlands, leaf wreaths, gold chains, gem rings, and pearl necklaces should begin as shared prototypes.

### When to create a culture-specific or regional item

Create a separate regional or culture-family item when one or more of the following is true:

- The body slot or component differs, such as nose ring, toe ring, anklet pair, armlet, choker, or headband.
- The visible silhouette differs enough that a skin would mislead, such as a torc-like collar, paired oval brooches, temple rings, hairpin sets, broad bangles, or court circlets.
- The primary material changes behaviour or major visual identity, such as metal versus glass versus bone versus leaf.
- The item is a culturally important jewellery system rather than decoration only, such as North-Sea bead-and-brooch sets, South Asian bangle/anklet/toe-ring sets, steppe belt-plaque display, Byzantine pearl-and-gold court ornaments, or East Asian hair ornaments.
- The item fills a social or gameplay gap: commoner affordability, merchant respectability, high-noble rank display, festival ephemerality, child/apprentice scale, or court gift exchange.

### What skins should carry instead

Do not create new base prototypes merely for:

- local fantasy-culture names
- exact owner, household, clan, dynasty, guild, retinue, or court marks
- heraldic devices
- love mottoes
- gemstone colour variation when a variable gem component can carry it
- exact enamel patterning
- religious meaning that should live in a devotional branch
- floral species where the base item is just a fresh garland and material support is generic
- decorative inscriptions without behaviour change
- richer polish on an otherwise identical item

### Shared category naming policy

The recommended unique-reference prefix is `medieval_jewellery_`. Use additional stable sub-prefixes for work packages when useful:

- `medieval_jewellery_ring_...`
- `medieval_jewellery_neck_...`
- `medieval_jewellery_wrist_...`
- `medieval_jewellery_ankle_...`
- `medieval_jewellery_ear_...`
- `medieval_jewellery_head_...`
- `medieval_jewellery_brooch_...`
- `medieval_jewellery_garland_...`
- `medieval_jewellery_regional_<family>_...`

Keep identifiers lowercase snake case. Do not encode a real-world culture label into public `sdesc`; the unique reference can use builder-facing buckets when needed.

## Seeder and project grounding rules

- Use the project-standard `CreateItem(...)` seeder call shape during implementation.
- `uniqueReference` values use lowercase snake case ASCII and should remain stable once accepted.
- `noun` is compact and singular, usually the object form: `ring`, `necklace`, `chain`, `bracelet`, `bangle`, `armlet`, `anklet`, `earring`, `stud`, `choker`, `circlet`, `wreath`, `garland`, `brooch`, `pin`, `badge`, `pendant`, `bead-string`, or `torc` where appropriate.
- `sdesc` is player-facing, concise, and normally begins with `a`, `an`, or `a pair of`. It should not end with a full stop.
- `ldesc` is normally `null` for ordinary portable jewellery, allowing the default room display to apply.
- `fdesc` is player-facing in-world prose: shape, visible material, polish, colour, setting, stone, beadwork, links, hinges, pins, clasps, twist, knotwork, stamped marks, surface wear, and how the item would sit when worn.
- `material` must be an exact seeded solid material. Liquids and gases are not substitutes for solid primary materials.
- `tags` must be exact seeded hierarchical tag paths.
- `components` must be exact seeded component prototype names.
- `inherentCost` is denominated in farthings. Use whole-farthing values unless a fractional farthing is deliberately intended.
- Ordinary decorative jewellery should be skinnable and player-visible.
- Ordinary portable jewellery should include `Holdable`.
- No ordinary jewellery row should use a destroyed-item reference, hidden-player flag, non-skinnable flag, or long description by default.
- Only ephemeral jewellery should normally use morph targets, morph emotes, or morph timers.

## Wearable implementation rules

### Current supported jewellery wear slots

The current seeded component inventory directly supports these jewellery-relevant wearable components:

| Jewellery form | Preferred component(s) | Notes |
|---|---|---|
| Finger rings | `Wear_Ring` | Use for decorative rings, love rings, signet-like rings, gem rings, and simple metal bands. |
| Necklaces and chains | `Wear_Necklace` | Use for bead strings, chains, pendants, collars where the necklace slot is acceptable, and many torc-like pieces if no better fit exists. |
| Chokers and close collars | `Wear_Choker` | Use for narrow collars, close bead strings, ribbon chokers, and tight court necklaces. |
| Bracelets and bangles | `Wear_Bracelet`, `Wear_Bracelets` | Use singular or paired components according to the item description. |
| Armlets | `Wear_Armlet` | Use for upper-arm ornaments, arm rings, and some torc-like arm display. |
| Anklets | `Wear_Anklet`, `Wear_Anklets` | Use singular or paired anklets. |
| Earrings | `Wear_Earring`, `Wear_Earrings` | Use singular or paired earrings. Paired earrings should normally use the plural component. |
| Nose rings | `Wear_Nose_Ring` | Use for South Asian and other culturally justified rows; do not make it a universal default. |
| Toe rings | `Wear_Toe_Ring` | Use mainly for South Asian-inspired rows unless a local fantasy setting expands the form. |
| Brow or lip rings | `Wear_Brow_Ring`, `Wear_Lip_Ring` | Not ordinary defaults for the 500-1300 culture set. Reserve for fantasy/local variants or explicitly justified regional content. |
| Headbands, wreaths, and light circlets | `Wear_Headband` | Use for simple circlets, leaf wreaths, chaplets, and headband-like ornaments when the wear slot is acceptable. |
| Crowns, coronets, large circlets, and rigid head ornaments | `Wear_Hat` or `Wear_Headband` | Use only when the body slot behaviour is acceptable. Very high-noble regalia may need a future `Wear_Circlet` or `Wear_Crown` component. |
| Decorative waist chains or girdle ornaments | `Wear_Waist` or inert `Holdable` | Use `Wear_Waist` only when the item is actually worn around the waist. Do not claim belt attachment unless belt behaviour exists. |

### Component gaps and conservative handling

The following historically important jewellery forms currently lack exact wear-slot support:

| Form | Recommended first-pass handling | Future component suggestion |
|---|---|---|
| Brooches, cloak pins, ring pins, badges, fibula-like fasteners | Author as inert portable jewellery with `Holdable` and a destroyable component, or defer mechanical wearing until slot support exists. Descriptions can say the item has a pin and catch, but must not claim it actually fastens clothing mechanically. | `Wear_Brooch`, `Wear_Pin`, `Wear_Badge` |
| Hairpins, hair combs, temple rings, hair plaques | Author as inert portable ornaments unless `Wear_Hat` or `Wear_Headband` is an acceptable approximation. | `Wear_Hairpin`, `Wear_Hair_Ornament`, `Wear_Hair_Combs`, `Wear_Temple_Rings` |
| Diadems, crowns, coronets, rigid circlets | Use `Wear_Headband` or `Wear_Hat` only where acceptable; otherwise defer. | `Wear_Circlet`, `Wear_Crown`, `Wear_Diadem` |
| Garlands and chaplets | Use `Wear_Headband`, `Wear_Necklace`, `Wear_Bracelets`, or `Wear_Anklets` according to the actual form. Use morphing for fresh examples. | `Wear_Garland`, `Wear_Chaplet` if more precise behaviour is wanted |
| Belt plaques, girdle mounts, dangling belt ornaments | Use `Wear_Waist` for full waist ornaments, or keep as inert jewellery. Do not use `Beltable` unless the item is intended to attach to a belt and the mechanics support it. | `Wear_Belt_Ornament`, `Belt_Ornament` |

### Ordinary component pattern

Most mechanically wearable jewellery rows should use:

- `Holdable`
- exactly one wearable component, such as `Wear_Ring`, `Wear_Necklace`, `Wear_Bracelet`, `Wear_Anklets`, or `Wear_Earrings`
- exactly one destroyable component
- optional variable components such as `Variable_Gem`, `Variable_FineGem`, `Variable_CommonStone`, `Variable_BasicColour`, or `Variable_FineColour` where the description uses variables

Most inert component-gap jewellery rows should use:

- `Holdable`
- exactly one destroyable component
- optional variable components

Do not add armour, insulation, pockets, containers, locks, writing surfaces, sealable targets, light sources, weapon components, or trait-changing components to ordinary decorative jewellery.

### Destroyable component mapping

| Material or object family | Default destroyable component | Notes |
|---|---|---|
| Gold, silver, brass, bronze, copper, electrum, pewter, iron, steel, and other dense metal jewellery | `Destroyable_HeavyMetal` | Use even for small objects when the item is primarily metal and should resist ordinary fragile-object damage. |
| Glass bead strings, fragile glass earrings, lead-glass pieces, enamel-like glass ornaments | `Destroyable_Glassware` | Use where the object is primarily glass or visibly fragile. A metal ring with a glass stone can still use `Destroyable_HeavyMetal`. |
| Bone, horn, ivory, shell, leather cord, wood, linen, silk, hemp, thread, leaf, herb, and flower-like organic pieces | `Destroyable_Misc` or `Destroyable_Clothing` | `Destroyable_Misc` is usually adequate. Use `Destroyable_Clothing` only for textile-dominant wearable bands, ribbons, or fabric ornaments. |
| Paper or parchment festival tokens | `Destroyable_Paper` | Only for intentionally paper/parchment ornaments. |

### Ephemeral and morphing implementation rules

Fresh organic jewellery is a narrow exception to the no-morph default.

- Fresh garlands, wreaths, herb bracelets, and flower strings should normally have a wilted or dried morph target.
- The morph target should be authored before fresh variants that point to it.
- Use `morphToUniqueReference`, `morphEmote`, and `morphTimer` only when the row should automatically age.
- A simple policy is: fresh -> wilted after 6-24 in-game hours; wilted -> dried after 1-3 in-game days only if a dried version is useful as an object.
- Use `leaf`, `lavender`, `chamomile`, `safflower`, `sunflower`, `linen`, or another exact seeded material where appropriate. If a precise flower material is not seeded, use the nearest exact material and carry the visual flower detail in `fdesc`.
- Fresh organic jewellery should have low cost and low weight. It should remain skinnable so local flowers, colours, and festival meanings can be supplied without new behaviour rows.

Example morph pattern, not final item text:

1. `medieval_jewellery_garland_wilted_leaf_chaplet` as the target.
2. `medieval_jewellery_garland_fresh_leaf_chaplet` morphs to the wilted target after a moderate timer.
3. Public `fdesc` describes visible freshness only; the morph emote handles wilting.

## Player-facing description rules

- Do not mention skins, base prototypes, standard implementations, seeder mechanics, builders, archaeology, uncertainty, or component gaps in `noun`, `sdesc`, `ldesc`, or `fdesc`.
- Public text should describe visible form and material: band, hoop, link, bead, wire, pin, catch, hinge, clasp, setting, bezel, cabochon, pendant loop, chain, tablet, plaque, filigree-like twist, stamped pattern, punched dotting, enamel-like colour, glass inlay, pearl-like bead, polished surface, tarnish, wear, cord, knot, woven thread, fresh leaves, or drying petals.
- Public text should avoid real-world culture labels such as Anglo-Saxon, Norse, Norman, Frankish, Byzantine, Abbasid, Fatimid, Seljuk, Rus, Mongol, Indian, Chinese, Korean, or Japanese unless the object term itself is widely intelligible and appropriate.
- Form names are acceptable when they identify the visible object rather than a people. Examples: `ring`, `necklace`, `chain`, `bead string`, `bracelet`, `bangle`, `armlet`, `anklet`, `earring`, `nose ring`, `toe ring`, `choker`, `circlet`, `garland`, `wreath`, `chaplet`, `brooch`, `pin`, `badge`, `pendant`, `collar`, `torc`, `girdle ornament`, and `belt plaque`.
- Do not make every item elite. Commoner descriptions should allow rough edges, simple wire, plain cord, mismatched beads, bone polish, shell chips, copper tarnish, and local material.
- Avoid gender-default naming. Do not create unmarked `ring` plus marked `woman's ring` unless gender is visible in the object form. Builder-facing manifests can place jewellery in gendered outfit plans, but public item text should usually prefer form and social context.
- Do not claim an item is sacred, protective, blessed, magical, royal by law, poison-bearing, a key, a seal, a coin purse, or a storage locket unless the mechanics and branch scope support that claim.
- Inscriptions should be handled by skins, writing blocks, or another content system. Avoid embedding exact readable text in base `fdesc` unless a deliberate writing block is used.

## Skin strategy

- All finished decorative jewellery should normally be skinnable.
- A skin can override presentation fields and quality without changing behaviour.
- Skins should carry high-variance presentation: exact gemstones, glass colours, enamel colour, local fantasy names, workshop marks, heraldic devices, clan or household signs, guild marks, love knots, owner marks, dynastic emblems, animal motifs, foliage motifs, border patterns, knotwork, granulation-like texture, filigree-like wire patterning, stamped marks, bead ordering, court colours, and festival flowers.
- Default unskinned items still need to be credible standalone objects. Skinability must not excuse bland base descriptions.
- Player-facing base descriptions should never say that a skin may later change the appearance.
- For jewellery whose exact cultural name is important but not universal, put that name in the skin rather than the base row unless the form itself is the only clear player-facing noun.

## Colour, gem, and variable policy

Use variables when the same behaviour and construction can support multiple visible materials, colours, or stones.

| Variable component | Jewellery use |
|---|---|
| `Variable_CommonStone` | Ordinary cabochons, glass-like stones, agate-like stones, bead strings, trade stones, and low-to-mid value jewellery. |
| `Variable_Gem` | General gemstone jewellery where a broad gem vocabulary is desired. Good for merchant, gentry, and elite pieces that should vary by stone. |
| `Variable_FineGem` | High noble, court, royal, very fine, or rare gemstone jewellery. Use sparingly. |
| `Variable_BasicColour` | Textile cords, ribbons, simple enamel-like paint, fresh garland colours, and ordinary coloured glass where basic colour words suffice. |
| `Variable_FineColour` | Elite enamel-like panels, court ribbons, fine textile backing, special festival colours, and richer colour vocabulary. |
| `Variable_2BasicColour` | Simple two-colour bead strings, cords, or festival garlands. |
| `Variable_2FineColour` | Elite two-colour bead, enamel-like, or ribbon-backed jewellery. |
| `Variable_DrabColour` and `Variable_2DrabColour` | Only for intentionally tarnished, old, grimy, neglected, or poor-condition ornaments. Do not use as the default for simple common jewellery. |

Variable usage must be grammatical. Examples:

- `a $gem silver ring`
- `a $commonstone bead necklace`
- `a $colour braided cord bracelet`
- `a $colour1 and $colour2 glass bead string`
- `a fine $finegemcolor gold circlet`

Where the stone materially changes the primary material, create separate rows only if behaviour or category changes. A gold ring set with amber should usually use `gold` as primary material and mention amber in `fdesc` or a variable. A necklace made mostly of amber beads should use `amber` as primary material.

## Materials, sizes, quality, and cost assumptions

### Recommended primary materials

Use exact seeded material names. Suitable currently seeded materials include:

- Metals: `gold`, `silver`, `sterling silver`, `copper`, `brass`, `bronze`, `electrum`, `pewter`, `lead`, `wrought iron`, `mild steel`, `carbon steel`.
- Gemstones and stones: `agate`, `amber`, `amethyst`, `aquamarine`, `aventurine`, `beryl`, `bloodstone`, `carnelian`, `chalcedony`, `citrine`, `diamond`, `emerald`, `garnet`, `jade`, `jasper`, `lapis lazuli`, `moonstone`, `onyx`, `opal`, `pearl`, `quartz`, `rose quartz`, `ruby`, `sapphire`, `smoky quartz`, `sunstone`, `topaz`, `turquoise`, `zircon`, and other exact seeded gemstone names.
- Organic and low-cost materials: `bone`, `horn`, `ivory`, `shell`, `jet`, `leather`, `linen`, `silk`, `hemp`, `wool`, `leaf`, `lavender`, `chamomile`, `safflower`, `sunflower`, `wood`, `boxwood`, `rosewood`, `sandalwood`, `willow`, `bamboo`.
- Glass and ceramic-like materials: `glass`, `soda-lime glass`, `lead glass`, `ceramic`, `porcelain` where form and period support them.

If a precise material such as enamel, niello, coral, rock crystal, or flower is not seeded, do not invent it as the primary `material`. Use the nearest exact seeded material and describe the visible detail in `fdesc` or through skins.

### Primary material policy

Primary material represents the dominant substance or behaviour-relevant body of the item, not every decorative inclusion.

- Metal ring with stone: primary material is usually the metal.
- Bead necklace dominated by glass beads: primary material can be `glass`.
- Amber bead string: primary material can be `amber`.
- Pearl necklace: primary material can be `pearl`.
- Floral wreath with a twined backing: primary material can be `leaf` or an exact herb/plant material where available.
- Cord bracelet with a tiny metal charm: primary material is usually the cord material, such as `hemp`, `linen`, or `leather`.

### Size and weight assumptions

Most jewellery is `Tiny`, `VerySmall`, or `Small`.

| Form | Typical size | Typical weight range |
|---|---|---:|
| Finger ring, toe ring, small stud | `Tiny` | 2-25g |
| Ordinary earring or pair of earrings | `Tiny` or `VerySmall` | 3-60g |
| Simple bracelet, bangle, armlet, anklet | `VerySmall` or `Small` | 15-250g |
| Heavy arm ring, torc-like collar, court chain | `Small` | 100-800g |
| Necklace, bead string, choker | `VerySmall` or `Small` | 20-350g |
| Brooch, pin, badge | `Tiny`, `VerySmall`, or `Small` | 10-250g |
| Circlet, chaplet, fresh garland, wreath | `Small` | 25-400g |
| High regalia or heavy noble collar | `Small` or rarely `Normal` | 300-1500g |

### Quality assumptions

- `Poor`, `Substandard`, or `Standard`: cheap, roughly made, improvised, worn, commoner, apprentice, rural, or aged jewellery.
- `Standard`: ordinary market jewellery and most simple metal/glass/bone/shell pieces.
- `Good`: respectable merchant, professional, gentry, fine craft, imported, or carefully finished jewellery.
- `VeryGood`: high noble, court, rare gemstone, exceptional craft, or highly visible status jewellery.
- `Great` and above: very rare. Reserve for exceptional regalia, named treasures, special event pieces, or a later legendary/fantasy pass.

### Cost assumptions

Costs are relative seeder baselines in farthings, not universal market prices.

| Band | Typical cost range | Examples |
|---|---:|---|
| Ephemeral organic | 1-8m | fresh leaf garland, herb bracelet, festival wreath |
| Cheap commoner | 2-24m | copper ring, bone bead bracelet, shell anklet, wooden bead necklace |
| Ordinary urban | 12-72m | glass bead necklace, brass bangle, pewter ring, simple earrings |
| Merchant / professional | 48-240m | silver ring, silver bracelet, small pearl earrings, respectable chain |
| Lesser noble / gentry | 180-960m | silver-gilt necklace, gold ring, gem-set bracelet, fine circlet |
| High noble / court | 960-5000m+ | jewelled gold chain, pearl collar, gemstone circlet, heavy noble regalia |

Metal and gemstone values should dominate cost more than weight. A tiny gold gem ring can cost far more than a heavy bronze armlet.

## Tagging policy

Use exact seeded tag paths. Current jewellery-relevant tags include:

- `Functions / Worn Items / Jewellery`
- `Functions / Worn Items / Jewellery / Anklets`
- `Functions / Worn Items / Jewellery / Bracelets`
- `Functions / Worn Items / Jewellery / Earrings`
- `Functions / Worn Items / Jewellery / Necklaces`
- `Functions / Worn Items / Jewellery / Piercings`
- `Functions / Worn Items / Jewellery / Rings`
- `Functions / Worn Items / Fashion Accessories`
- `Market / Household Goods / Simple Wares`
- `Market / Household Goods / Standard Wares`
- `Market / Household Goods / Luxury Wares`

Do not use `Functions / Material Functions / Jewellery Craft Stock` tags for finished jewellery. Those tags are for raw or intermediate jewellery-making stock, such as wire, settings, beads, and metal stock.

The current hierarchy has no dedicated `Market / Jewellery` branch. Until one is added, use household-wares market tags by value tier, usually:

- `Market / Household Goods / Simple Wares` for cheap, common, or ephemeral jewellery.
- `Market / Household Goods / Standard Wares` for ordinary market jewellery.
- `Market / Household Goods / Luxury Wares` for precious-metal, gemstone, merchant, noble, court, and ceremonial jewellery.

Recommended future tag additions:

- `Market / Jewellery`
- `Market / Jewellery / Simple Jewellery`
- `Market / Jewellery / Standard Jewellery`
- `Market / Jewellery / Luxury Jewellery`
- `Functions / Worn Items / Jewellery / Brooches`
- `Functions / Worn Items / Jewellery / Head Ornaments`
- `Functions / Worn Items / Jewellery / Hair Ornaments`
- `Functions / Worn Items / Jewellery / Garlands`
- `Functions / Worn Items / Jewellery / Belts and Girdle Ornaments`

## Historical and design assumptions by inspiration family

### Early Anglo-Saxon

Reference anchor: c. 800AD. The jewellery plan should support common bead strings, simple rings, copper or bronze pieces, glass and amber-like beads, and elite gold/silver/garnet-like display. Disc brooches and pins are important but component-limited; include some inert brooch rows unless a brooch wear slot is added. Public descriptions should focus on round shapes, punched decoration, inlay-like colour, and practical pin backs rather than ethnic labels.

### Anglo-Danish

Reference anchor: c. 950AD. Use a hybrid North-Sea vocabulary: bead strings, arm rings, simple rings, silver and bronze ornaments, glass beads, amber, and brooch-like clothing ornaments. Hangerok-related brooches should not be duplicated from clothing unless the separate object matters to gameplay. In the absence of a brooch component, make them inert portable jewellery or reserve them for a later component pass.

### Norse / Viking Age

Reference anchor: c. 950AD. This family needs bead festoons, arm rings, neck rings, simple finger rings, amber and glass bead strings, silver display pieces, and paired brooch silhouettes. Jewellery may be economically meaningful as portable wealth, but public descriptions should not call pieces currency unless they have currency behaviour.

### Norman

Reference anchor: c. 1066AD. The Norman plan should be restrained compared with later Capetian and High English jewellery: rings, simple brooches, cloak pins, belt fittings, silver or gilded ornament, and a few elite gem pieces. Avoid high-late-medieval livery badges and elaborate court jewels as defaults.

### Anglo-Norman

Reference anchor: c. 1150AD. Increase urban and court variety: annular brooches, ring brooches, simple signet-like rings, small chains, necklaces, girdle ornaments, glass or gem settings, and respectable merchant jewellery. Keep sacred badges and pilgrim signs outside this decorative branch.

### High English / British

Reference anchor: c. 1250AD. This branch supports the later end of the medieval slice: signet-like rings, gold rings, gem rings, annular brooches, fine chains, circlets, girdle ornaments, secular household badges, and high-noble display. Heraldry belongs mostly in skins.

### Irish / Gaelic

Reference anchor: c. 1100AD. Brooches, ring pins, penannular forms, cloak fasteners, amber/glass beads, silver rings, and torc-like collars are key visual anchors. Treat brooches as important component-gap objects. Public text should use visible forms such as `ring brooch`, `cloak pin`, `bead necklace`, or `silver armlet` rather than culture labels.

### Scottish / Gaelic-Lowland

Reference anchor: c. 1250AD. Use both Gaelic cloak-fastener traditions and Lowland court/urban jewellery. Include silver brooches as inert or future-wearable rows, rings, amber or glass bead strings, practical pins, and a few merchant/noble pieces. Avoid projecting later specifically early-modern forms backwards.

### Carolingian / Frankish

Reference anchor: c. 800AD. Include brooches, belt ornaments, bead strings, simple rings, elite gold, garnet-like colour, silver and bronze pieces, and courtly display. Commoner pieces should remain mostly glass, bronze, copper, bone, shell, and cord.

### Capetian French

Reference anchor: c. 1200AD. This family can carry rich western court jewellery: gem rings, gold chains, silver-gilt necklaces, fine brooches, delicate circlets, love-token rings, and courtly belt ornaments. Avoid later fourteenth- and fifteenth-century extremes.

### Holy Roman Empire / German

Reference anchor: c. 1200AD. Use strong urban and court goldsmithing cues: rings, chains, brooches, civic ornaments, silver/gilt display, enamel-like colour, and merchant-class respectability. Guild marks and city symbols should usually be skins.

### Christian Iberian

Reference anchor: c. 1200AD. The jewellery plan should sit between western European and Islamicate forms: earrings, rings, necklaces, bracelets, girdle ornaments, coloured glass, filigree-like wire, pearls, and gold/silver urban jewellery. Public base text should avoid dynastic or religious claims.

### Andalusian

Reference anchor: c. 1100AD. Include urban luxury jewellery: earrings, necklaces, bracelets, anklets, rings, filigree-like and granulated-looking work, pearls, coloured glass, and gold or silver pieces. Keep religious inscriptions and devotional amulets out of base rows.

### Byzantine

Reference anchor: c. 1000AD. The family should have high-status gold, pearls, glass inlay, enamel-like panels, earrings, necklaces, bracelets, rings, court chains, and circlets. Decorative crosses and reliquary pendants belong elsewhere unless the item is deliberately secularized by skin.

### Abbasid

Reference anchor: c. 850AD. Include court and urban jewellery: rings, earrings, bracelets, anklets, bead strings, pearls, glass, gold, silver, and fine wire-like detail. Scholarly or official signet usage can be handled by decorative signet rings or by writing-package seal tools if mechanical stamping is required.

### Fatimid

Reference anchor: c. 1050AD. Warm-climate urban jewellery should emphasize earrings, bracelets, anklets, necklaces, beads, pearls, glass, gold, and silver. Add merchant and court versions without making all pieces devotional or ceremonial.

### Seljuk / Ayyubid-Mamluk

Reference anchor: c. 1200AD. Include mounted-elite and urban jewellery: rings, armlets, bracelets, anklets, pendants, belt ornaments, chains, earrings, turquoise-like and carnelian-like colour, silver, and gold. The catalogue should support both practical travel display and courtly luxury.

### Magyar

Reference anchor: c. 950AD. Focus on mobile elite display: belt plaques, pendant ornaments, earrings, silver rings, beads, and animal or geometric motif skins. Keep belt ornaments inert unless a waist or belt ornament component is added.

### Rus / Novgorod

Reference anchor: c. 1100AD. Use bead strings, temple-ring-like ornaments as component-gap items, silver rings, pendants, bracelets, amber, glass, and Byzantine-influenced elite forms. Cold-climate layers should not prevent visible jewellery; many items can be displayed at neck, wrist, ear, or head.

### Steppe Turkic / Mongol

Reference anchor: c. 1200AD. Focus on portable wealth and mounted display: belt ornaments, plaques, earrings, rings, beads, pendants, silver and gilt pieces, and head/hair ornaments as component-gap items. Avoid magical talisman mechanics even where shapes look amulet-like.

### North Indian / Rajput

Reference anchor: c. 1100AD. This family needs broad jewellery coverage across body slots: necklaces, earrings, bangles, bracelets, anklets, toe rings, nose rings, head ornaments, pearls, gold, silver, glass, and fresh garlands. This is one of the strongest justifications for `Wear_Nose_Ring`, `Wear_Toe_Ring`, paired bracelets, and paired anklets.

### South Indian / Chola

Reference anchor: c. 1050AD. Use gold-heavy court and temple-adjacent decorative forms while keeping devotional function out of the base rows: layered necklaces, bangles, anklets, toe rings, earrings, head ornaments, pearls, gems, and fresh flower garlands. Floral jewellery should be represented as short-lived morphing objects where useful.

### Song China

Reference anchor: c. 1100AD. Rings are less central than hair and head ornaments, pins, combs, pendants, jade-like pieces, gold, and restrained bead strings. Because the component set lacks hairpin and comb slots, many rows should be inert or use conservative `Wear_Headband` / `Wear_Hat` approximations only where credible.

### Goryeo Korea

Reference anchor: c. 1150AD. Include restrained court ornaments, hairpin-like pieces as component-gap rows, pendants, jade-like stones, beads, rings where useful, and gilt metal. Avoid overproducing western-style ring and necklace forms for this family.

### Heian / Kamakura Japan

Reference anchor: c. 1200AD. Jewellery coverage should emphasize hair ornaments, combs, cords, bead strings, small pendants, and understated elite display rather than abundant finger rings. Many core forms need future hair ornament components; use inert rows rather than forcing inaccurate wear slots.

## Category implementation rules

### Rings and signet-like rings

- Rings are the safest and most broadly supported jewellery category because `Wear_Ring` exists.
- Include common copper and bronze rings, simple iron rings, bone rings, silver rings, gold rings, gem rings, love-token rings, merchant signet-like rings, and court rings.
- Use non-functional signet descriptions unless `SealStamp_Medieval_BronzeSignet` or another seal-stamp component is intentionally paired and validated.
- Do not promise legal authority, seal compatibility, or ownership recognition without seal-stamp mechanics and appropriate item state.
- Gem-set rings should generally use the metal as primary material and use `Variable_CommonStone`, `Variable_Gem`, or `Variable_FineGem` for visible stones.

### Necklaces, chains, chokers, collars, and torc-like pieces

- Use `Wear_Necklace` for most necklaces, chains, pendants, bead strings, and loose collars.
- Use `Wear_Choker` for close-fitting collars, ribbons, neck rings, and short bead strings.
- Torc-like forms can be included where culturally conservative, but avoid making every northern or steppe collar a torc.
- Include common bead strings, cord necklaces, shell necklaces, glass beads, amber beads, pearl necklaces, silver chains, gold chains, gem collars, and court display collars.
- Religious pendants, cross pendants, scripture pendants, relic lockets, and prayer beads are not part of this branch.

### Bracelets, bangles, armlets, and anklets

- Use `Wear_Bracelet` or `Wear_Bracelets` for wrist ornaments.
- Use `Wear_Armlet` for upper-arm rings and armlets.
- Use `Wear_Anklet` or `Wear_Anklets` for ankle ornaments.
- Include common cord bracelets, copper bracelets, bone bracelets, shell bracelets, glass bangles, brass bangles, silver bracelets, gold bangles, arm rings, and elite gem-set pieces.
- South Asian coverage should include bangles, anklets, and toe rings as major categories rather than minor variants.

### Earrings and piercings

- Use `Wear_Earring` and `Wear_Earrings` for ordinary earrings.
- Use `Wear_Nose_Ring` and `Wear_Toe_Ring` only where culturally justified, especially South Asian-inspired rows.
- Avoid default use of `Wear_Brow_Ring`, `Wear_Lip_Ring`, `Wear_Bellybutton_Ring`, `Wear_Nipple_Ring`, `Wear_Tongue_Ring`, and `Wear_Penis_Ring` in the strict 500-1300 catalogue unless a fantasy/local expansion asks for them.
- Include studs, hoops, pendant earrings, bead earrings, pearl earrings, simple wire earrings, and elite gold or gem earrings.

### Brooches, pins, badges, and fasteners

- Brooches and pins are historically central but mechanically under-supported.
- First-pass implementation should either author them as inert portable decorative items or wait for a wear-slot component.
- Inert brooch rows should still be useful for trade, display, treasure, crafting outputs, and later skins.
- Descriptions may mention a pin, catch, shank, boss, terminal, hinge, or clasp as visible construction, but must not promise functional clothing fastening without a component.
- Exclude pilgrim badges and devotional badges from this decorative branch.
- Include secular badges, household badges, love badges, ring brooches, annular brooches, disc brooches, cloak pins, strap-end ornaments, and courtly jewelled pins where appropriate.

### Head ornaments, circlets, garlands, hair ornaments, and combs

- Use `Wear_Headband` for light circlets, chaplets, wreaths, and garlands when the body slot is acceptable.
- Use `Wear_Hat` for larger coronets or crown-like ornaments only if the behaviour fits.
- Hairpins, combs, temple rings, and rigid diadems are component-gap items. Keep inert or add future components.
- Include simple leaf wreaths, flower garlands, ribbon chaplets, brass circlets, silver circlets, gold circlets, pearl circlets, and high noble gem circlets.
- Floral and leafy head ornaments are a major way to include commoner and festival jewellery.

### Waist ornaments, chains, plaques, and girdle jewellery

- Decorative belts already overlap with clothing. This branch should only include clearly jewellery-like waist ornaments: waist chains, dangling plaques, girdle jewel mounts, and decorative belt plaques.
- Use `Wear_Waist` only for complete waist-worn ornaments.
- Do not use `Belt_2`, `Belt_4`, `Belt_6`, or `Belt_Large` unless the item is a functional belt designed to carry beltable items. Most jewellery waist chains are not functional belts.
- Do not use `Beltable` unless the item should attach to a belt as a separate beltable object.

### Bead strings and commoner jewellery

- Include a wide range of cheap bead and cord rows.
- Beads may be glass, bone, shell, wood, amber, jet, clay/ceramic, or stone.
- Commoner items should often be `Standard`, `Substandard`, or `Poor`, but should still be visually interesting.
- Avoid making commoner jewellery only shabby. Many inexpensive pieces can be neat, colourful, and meaningful.

### Merchant, guild, and professional jewellery

- Merchant and guild pieces should be respectable, legible, and durable without always being noble luxury.
- Suitable forms include silver rings, brass chains, guild-colour bead strings, stamped bracelets, non-functional signet rings, small brooches, belt plaques, and neat necklaces.
- Guild marks and exact professional signs should mostly be skins. Base rows can use generic phrases such as `a stamped silver ring` or `a brass merchant chain`.

### Noble, court, and regalia-adjacent jewellery

- High noble jewellery should not crowd out common pieces, but it must be present.
- Include gold rings, gem rings, pearl earrings, gem necklaces, gold bracelets, jewelled circlets, court chains, and heavy collars.
- Use `VeryGood` sparingly; reserve `Great` for a handful of exceptional regalia-like prototypes if the later implementation wants them.
- Do not create explicit royal crowns as default everyday jewellery unless the MUD wants accessible regalia objects. A `gold circlet` is usually a safer base row than a named crown.

### Child, apprentice, festival, and courtship jewellery

- Include smaller and cheaper pieces that help populate social scenes: child bead necklaces, cord bracelets, shell anklets, tiny copper rings, festival wreaths, paired courtship bracelets, token rings, and seasonal garlands.
- Courtship and wedding pieces may be decorative and secular. Avoid religious vow language or sacred ceremony claims.
- Fresh festival pieces should use morphs where useful.

## Catalogue distribution target

The first complete implementation should aim for about 400 item prototypes. A useful starting distribution is:

| Category | Target rows | Notes |
|---|---:|---|
| Shared commoner and ephemeral adornment | 52 | cord, shell, bone, wood, leaf, herb, flower, glass, cheap copper, child/apprentice pieces |
| Rings and signet-like rings | 55 | simple rings, gem rings, love rings, merchant signets, court rings |
| Neck adornment | 54 | bead strings, necklaces, chokers, chains, collars, pendants without devotional meaning |
| Wrist, arm, ankle, and toe adornment | 58 | bracelets, bangles, armlets, anklets, toe rings, cord bands, elite sets |
| Earrings and culturally justified piercings | 34 | studs, hoops, pendant earrings, paired earrings, nose rings, limited toe/nose emphasis for South Asian rows |
| Brooches, pins, badges, and fasteners | 58 | many may be inert until wear-slot support exists; exclude pilgrim/devotional badges |
| Head ornaments, hair ornaments, circlets, wreaths, and garlands | 45 | `Wear_Headband` / `Wear_Hat` where appropriate; many hair forms may remain inert |
| Waist ornaments, belt plaques, hanging ornaments, and court chains | 26 | non-functional ornament unless belt or waist components are deliberately used |
| Elite sets, regional statement pieces, and rare court goods | 18 | limited very high value items; avoid overproducing regalia |
| **Total** | **400** | Adjust during implementation if component support changes. |

### Suggested work packages

| Pass | Target focus | Approximate rows |
|---|---|---:|
| 1 | Shared cheap/commoner jewellery, bead strings, simple rings, organic garlands | 45 |
| 2 | Shared rings, merchant signet-like rings, gem rings | 45 |
| 3 | Necklaces, chokers, chains, collars, bead strings | 45 |
| 4 | Bracelets, bangles, armlets, anklets, toe rings | 45 |
| 5 | Earrings and culturally justified piercings | 30 |
| 6 | Brooches, pins, badges, and fasteners, using inert handling if no new component exists | 50 |
| 7 | Head ornaments, circlets, garlands, hairpins, combs, and component-gap items | 45 |
| 8 | Regional shared groups: northern, western, Mediterranean, Islamicate, South Asian, East Asian, steppe | 65 |
| 9 | High noble, court, regalia-adjacent, and rare elite jewellery | 25 |
| 10 | Review, balance, duplicate removal, tag/material validation, and ephemeral morph validation | 5-10 net additions or replacements |

## Builder-facing jewellery set manifests

This branch does not need clothing-style outfit manifests for every culture, but the implementation should support builder-facing jewellery sets. These sets are optional scene-building collections rather than mandatory clothing outfits.

### Commoner festival set

- fresh leaf chaplet
- braided cord bracelet
- shell bead necklace
- copper ring
- wilted garland morph target

### Urban merchant set

- stamped silver ring
- brass merchant chain
- glass bead necklace
- neat silver bracelet
- small secular badge or inert brooch

### Northern elite set

- silver arm ring
- amber bead necklace
- gem-set ring
- ornate cloak brooch or inert pin
- silver bead string

### Western court set

- gold gem ring
- pearl earrings
- silver-gilt necklace
- fine circlet
- jewelled brooch or inert court pin

### Islamicate urban luxury set

- gold earrings
- silver filigree-like bracelet
- pearl necklace
- gem-set ring
- paired anklets

### South Asian festival set

- fresh flower garland
- paired bangles
- paired anklets
- nose ring
- toe ring
- gold bead necklace

### East Asian court ornament set

- jade-like pendant
- gilt hairpin or inert comb
- restrained bead string
- delicate gold ring where useful
- headband-like court ornament if slot behaviour fits

### Steppe and caravan display set

- silver earrings
- belt plaque ornament
- bead necklace
- silver ring
- pendant chain

## Implementation validation checklist

Before emitting any `CreateItem(...)` row for this branch, confirm:

- The item is decorative jewellery or secular personal adornment, not devotional/religious goods.
- The `uniqueReference` is lowercase snake case and begins with a stable jewellery prefix.
- `noun`, `sdesc`, and `fdesc` are public, in-world, concise, and culture-neutral unless the form name itself is useful.
- `ldesc` is `null` unless the row is deliberately a display fixture, which should be rare or out of branch.
- `material` exactly matches a seeded solid material.
- Every tag exactly matches `SeededTagHierarchy.csv`.
- Finished jewellery uses appropriate exact jewellery/function tags.
- The item uses `Holdable` unless there is a specific reason not to.
- The item uses exactly one destroyable component.
- A wearable component is used only where a current exact component fits the form.
- Component-gap objects do not falsely claim working wear, fastening, storage, sealing, magic, or protection.
- No ordinary jewellery row receives `Armour_*`, `Insulation_*`, `Container_*`, `LockingContainer_*`, `Sealable_*`, `Light`, `Weapon`, or trait-changing components by default.
- Variable components match variables actually used in public descriptions.
- Ephemeral items have valid morph targets, timers, and morph emotes when they are meant to age.
- Costs and weights are plausible relative to material, size, and status.
- Skinnable is normally `true`; `hideFromPlayers` is normally `false`.

## Open implementation questions

These questions do not block the first design reference, but they should be resolved before or during the full 400-row item creation pass:

1. Should the component inventory add dedicated `Wear_Brooch`, `Wear_Pin`, `Wear_Hairpin`, `Wear_Circlet`, and `Wear_Garland` components before the jewellery catalogue is implemented?
2. Should the tag hierarchy add a `Market / Jewellery` branch and finer jewellery function tags for brooches, head ornaments, hair ornaments, garlands, and waist ornaments?
3. Should mechanically functional signet rings be allowed to combine `Wear_Ring` with `SealStamp_Medieval_BronzeSignet`, or should all functional seals remain separate writing-package tools?
4. Should fresh garlands use a fixed real-time timer, an in-game time timer, or setting-specific decay progs?
5. Should high noble regalia be ordinary player-visible goods, admin-only rare goods, or left to world-specific skins and treasure objects?

## First-pass conclusion

The medieval jewellery branch should make the world feel socially inhabited rather than merely wealthy. The 400-row target should include rough copper rings beside gold gem rings, shell bracelets beside court pearls, fresh garlands beside formal circlets, and merchant signets beside child bead strings. The most important implementation constraint is mechanical honesty: where wear slots exist, use them; where they do not, either keep the object inert and portable or add the missing component before claiming wearable behaviour.
