# FutureMUD Medieval Jewellery Seeder Design Reference

This document defines the research framing, scope, component strategy, culture coverage, item-category plan, and first-pass catalogue architecture for decorative medieval jewellery in the FutureMUD Item Seeder.

It is focused on **decorative personal adornment**. Religious furnishings, devotional containers, ritual supplies, scripture supports, reliquaries, prayer tools, pilgrim badges, explicitly sacred amulets, and other devotional/religious goods belong to the existing religious-goods branch or a later devotional-personal-items branch. The jewellery branch may include socially meaningful, heraldic, civic, courtly, family, guild, romantic, seasonal, and festival jewellery, but base prototypes should not encode religious function or sacred text.

## Dependency review status

The previous dependency request has now been implemented on `master` and this design reference has been revised to treat those dependencies as **available implementation vocabulary**, not future gaps.

The current master dependency pass provides:

- precise wearable component profiles for brooches, paired brooches, pins, badges, hairpins, hair combs, hair ornaments, temple rings, circlets, diadems, coronets, crowns, chaplets, wreaths, head garlands, neck garlands, wrist garlands, ankle garlands, torcs, neck rings, waist chains, girdle ornaments, belt ornaments, belt plaques, waist ornaments, and forehead ornaments;
- ring-compatible medieval signet seal-stamp component prototypes for mechanically functional signet rings;
- finished-jewellery function tags, piercing refinements, garland/wreath refinements, and a full `Market / Jewellery` branch;
- solid-material support for coral, rock crystal, faience, enamel, niello, silver-gilt, gilded bronze, gilded copper, mother-of-pearl, nacre, cowrie shell, conch shell, tortoiseshell, flowers, petals, and named floral/leaf materials;
- optional jewellery variable components for motifs, flowers, metal finish, bead pattern, jewellery shape, and inlay style.

Therefore the full 400-row catalogue should not skip brooches, pins, badges, hair ornaments, circlets, garlands, wreaths, temple rings, torcs, or waist ornaments. They are now first-class targets.

## Executive summary

- Total target unique item prototypes for the first full catalogue: **about 400**.
- This document prepares the item-creation pass. It does not enumerate every final `CreateItem(...)` row.
- The catalogue should cover commoner adornment, urban and merchant jewellery, professional and guild display pieces, elite and high-noble jewellery, courtly display jewellery, child/apprentice jewellery, courtship and wedding gifts, and short-lived organic adornment such as fresh garlands, flower chaplets, herb bracelets, leaf wreaths, and blossom ankle garlands.
- The branch uses the same **500AD-1300AD** medieval world slice as the clothing, household, writing, and military references.
- The branch should be **shared-first**. Regional items are justified where visible form, body slot, material, social use, or craft tradition meaningfully differs. Skins should carry local motifs, household marks, guild marks, exact flower choices, exact gemstones, heraldry, inscriptions, and fantasy-world naming.
- All ordinary jewellery items are finished goods, skinnable, player-visible, and portable. Most should include `Holdable`, one precise wearable component, and one appropriate destroyable component.
- Decorative jewellery should not receive `Armour_*`, `Insulation_*`, container, lock, weapon, light-source, identity-obscuring, or trait-changing components unless a later mechanical branch explicitly designs that behaviour.
- Functional signet rings are now possible, but should be deliberate: a decorative signet-like ring uses `Wear_Ring`; a functional sealing ring uses `Wear_Ring` plus an appropriate `SealStamp_Medieval_*` component only when the item should actually apply seals.
- Fresh organic jewellery should normally use timed morph targets when authored: fresh item -> wilted item -> dried item where the dried target is worth retaining.
- The catalogue should deliberately include cheap and temporary pieces. Medieval jewellery should not be only noble gemstone jewellery; ordinary adornment, festival display, keepsakes, trade beads, copper rings, shell strings, bone beads, coloured glass, and fresh flowers are important for social texture.

## Scope and era model

- Chronological band: **500AD to 1300AD**.
- Geographic coverage: Britain and Ireland; Scandinavia and the North Sea; western, central, and southern Europe; Iberia; Byzantium; the Levant; Egypt and North Africa; the Eurasian steppe; Rus/Novgorod; northern and southern India; China; Korea; and Japan.
- Historical inspiration families: Early Anglo-Saxon, Anglo-Danish, Norse / Viking Age, Norman, Anglo-Norman, High English / British, Irish / Gaelic, Scottish / Gaelic-Lowland, Carolingian / Frankish, Capetian French, Holy Roman Empire / German, Christian Iberian, Andalusian, Byzantine, Abbasid, Fatimid, Seljuk / Ayyubid-Mamluk, Magyar, Rus / Novgorod, Steppe Turkic / Mongol, North Indian / Rajput, South Indian / Chola, Song China, Goryeo Korea, Heian / Kamakura Japan. These labels are builder-facing organizational buckets, not public item wording.
- Resolution: standard jewellery prototypes rather than museum-taxonomy subtypes. The default rows should be credible as unskinned objects; skins can provide exact motifs, owner marks, heraldry, enamel patterning, gemstone choices, workshop marks, rank marks, dynastic signs, textile colours, flower species, and world-specific naming.
- Culture labels are used to ensure design coverage. They should not be mechanically inserted into `noun`, `sdesc`, or `fdesc` unless the object term itself is useful and period-appropriate.
- Coverage deliberately excludes late medieval and Renaissance jewellery forms that primarily belong after 1300, such as mature Tudor and later early-modern pendant forms, post-medieval gimmal-ring fashion, Renaissance hat jewels, elaborate late chivalric livery badges, and modern jewellery categories.

### Culture coverage table

| Inspiration family | Reference anchor | Coverage boundary | Jewellery-design focus |
|---|---:|---|---|
| Early Anglo-Saxon | c. 800AD | lowland England before sustained Anglo-Danish synthesis | bead strings, simple finger rings, disc brooches, pins, glass beads, amber, bone, bronze, silver, and elite garnet-like display |
| Anglo-Danish | c. 950AD | late Anglo-Saxon England under strong Scandinavian settlement influence | mixed North-Sea bead strings, arm rings, rings, paired brooches, pins, silver display, amber, glass, and imported beadwork |
| Norse / Viking Age | c. 950AD | Scandinavia and diaspora settlements before the close of the Viking Age | paired brooches, bead festoons, arm rings, neck rings, torcs, finger rings, amber, glass, silver, bronze, and portable wealth display |
| Norman | c. 1066AD | ducal Normandy and conquest-generation Norman sphere | simple rings, ring brooches, cloak pins, belt ornaments, modest silver/gilt display, and elite gems without later court extravagance |
| Anglo-Norman | c. 1150AD | post-Conquest England and Norman-influenced Britain | annular brooches, small rings, signet-like rings, courtly necklaces, girdle ornaments, enamel-like colour language, and urban merchant jewellery |
| High English / British | c. 1250AD | high medieval England and Welsh March/British court-facing styles | signet rings, gem rings, annular brooches, jewelled belt ornaments, chains, circlets, courtly pins, civic badges, and fine silver or gold merchant jewellery |
| Irish / Gaelic | c. 1100AD | Gaelic Ireland before strong late-medieval English tailoring influence | penannular and ring-brooch families, cloak pins, amber and glass beads, silver rings, torc-like collars where conservative, and elite interlace-ready display |
| Scottish / Gaelic-Lowland | c. 1250AD | medieval Scotland across Gaelic and Lowland spheres | brooches, cloak pins, ring pins, simple rings, amber and glass, silver display, and Lowland court jewellery overlapping western European forms |
| Carolingian / Frankish | c. 800AD | Frankish court and common household practice under Carolingian influence | brooches, belt ornaments, glass beads, bronze and silver rings, elite gold, garnet-like stones, and courtly necklace or pendant forms kept non-devotional |
| Capetian French | c. 1200AD | northern and central French high medieval sphere | courtly rings, chains, circlets, annular brooches, gem settings, enamel, silver-gilt, gold, and noble love or household tokens |
| Holy Roman Empire / German | c. 1200AD | German-speaking imperial regions and neighbouring central European towns | rings, brooches, chains, civic and guild ornaments, belt plaques, silver and goldsmith display, and merchant-class jewellery |
| Christian Iberian | c. 1200AD | Leonese, Castilian, Aragonese, Navarrese, and neighbouring Christian Iberian contexts | rings, earrings, filigree-like metalwork, court necklaces, belt ornaments, glass and precious stones, and western-Islamicate overlap without religious coding |
| Andalusian | c. 1100AD | al-Andalus and western Islamic Iberia | earrings, bracelets, anklets, necklaces, filigree-like work, coloured glass, pearls, coral, gold, silver, and urban luxury jewellery |
| Byzantine | c. 1000AD | middle Byzantine Constantinopolitan and provincial contexts | gold rings, earrings, necklaces, pearl and glass display, enamel panels, courtly chains, bracelets, diadems, and high-status ornament that stays decorative rather than liturgical |
| Abbasid | c. 850AD | Baghdad-centred early medieval Islamic urban and scholarly contexts | urban luxury rings, earrings, bracelets, anklets, filigree-like and granulated-looking metalwork, pearls, glass, gold, silver, and merchant display |
| Fatimid | c. 1050AD | Fatimid Egypt and North African urban/textile contexts | gold earrings, bracelets, bead strings, anklets, pearls, glass, coral, and fine urban jewellery suitable for warm-climate textile outfits |
| Seljuk / Ayyubid-Mamluk | c. 1200AD | Anatolia, Syria, Egypt, and crusading-era eastern Mediterranean | rings, armlets, bracelets, anklets, earrings, pendants, belt ornaments, turquoise-like and carnelian-like colour, gold, silver, and mounted-elite display |
| Magyar | c. 950AD | Carpathian Basin Magyar and steppe-to-central-Europe context | belt ornaments, temple rings, earrings, silver rings, pendants, beads, and mobile elite display |
| Rus / Novgorod | c. 1100AD | Kievan Rus and Novgorod-facing northern Slavic/Norse-Byzantine intersections | temple rings, bead strings, pendant necklaces, rings, bracelets, silver, glass, amber, and Byzantine-influenced elite pieces |
| Steppe Turkic / Mongol | c. 1200AD | Inner Eurasian mounted steppe before and during early imperial Mongol expansion | belt plaques, hair ornaments, temple ornaments, silver earrings, rings, bead strings, non-magical amulet-shaped ornaments, and portable elite display |
| North Indian / Rajput | c. 1100AD | north and north-western Indian contexts before heavy late Sultanate/Mughal tailoring dominance | bangles, anklets, toe rings, nose rings, earrings, necklaces, forehead ornaments, head ornaments, gold, silver, pearls, glass, conch shell, and fresh festival garlands |
| South Indian / Chola | c. 1050AD | Chola and neighbouring south Indian temple and court environments | gold necklaces, bangles, anklets, toe rings, ear ornaments, forehead ornaments, head ornaments, pearls, gems, conch shell, jasmine, lotus, marigold, and warm-climate court or festival adornment |
| Song China | c. 1100AD | Northern and Southern Song domestic, urban, and court baseline | hairpins, combs, hair ornaments, jade and gold ornaments, earrings where useful, bead strings, pendants, restrained court pieces, and scholar-household display |
| Goryeo Korea | c. 1150AD | Goryeo-period Korean court, Buddhist, and scholarly baseline | hairpins, hair ornaments, rings, pendants, beads, jade-like stones, gilt metal, and restrained elite display |
| Heian / Kamakura Japan | c. 1200AD | late Heian to early Kamakura court, temple, and warrior contexts | hair ornaments, combs, cords, bead strings, understated court ornaments, metal and lacquer-like visual treatment, and limited ring or earring use unless setting skins justify it |

### Religious and devotional boundary table

| Object family | Catalogue treatment | Notes and limits |
|---|---|---|
| Decorative rings, necklaces, bracelets, anklets, earrings, chains, circlets, bead strings, brooches, pins, badges, hair ornaments, waist ornaments, wreaths, and garlands | In scope | These are the core of this branch when their base purpose is adornment, status, fashion, affection, household identity, festival display, secular authority, or decorative rank. |
| Secular signet rings and seal-like rings | In scope with caution | A non-functional decorative signet ring can be ordinary jewellery. A mechanically functional signet ring may combine `Wear_Ring` with `SealStamp_Medieval_RingSignet`, `SealStamp_Medieval_PersonalSignetRing`, `SealStamp_Medieval_MerchantSignetRing`, or `SealStamp_Medieval_NobleSignetRing` when seal behaviour is deliberately intended. |
| Secular badges, household badges, civic badges, and guild/professional badges | In scope | Use `Wear_Badge` and keep exact household, civic, livery, guild, and heraldic signs in skins unless they change behaviour. |
| Pilgrim badges, prayer beads, rosaries, relic pendants, reliquary lockets, explicitly sacred amulets, votive jewellery, scripture-inscribed charms | Out of scope for this branch | These belong to the devotional/religious branch or a later personal-devotional branch. Do not duplicate them here merely because they are wearable. |
| Magical amulets, protective talismans, curse rings, healing gems, luck charms, disguise jewels | Out of scope by default | Decorative forms may exist, but no base row should imply magical or trait-changing behaviour unless a later fantasy mechanics branch approves it. |
| Coins, bullion, raw gemstones, jewellery wire, loose settings, bead stock | Mostly out of scope | Finished jewellery belongs here. Raw craft stock belongs under material/crafting supply branches and may use jewellery craft-stock tags. |

### Social and lifecycle coverage table

| Social band or lifecycle | Catalogue intent | Example public forms |
|---|---|---|
| Rural and commoner adornment | Cheap, local, repairable, improvised, and seasonal pieces | copper ring, bone bead string, wooden bead necklace, shell bracelet, braided cord anklet, herb wreath, leaf garland |
| Urban commoner and apprentice adornment | Slightly neater but still inexpensive goods | coloured glass necklace, brass bracelet, pewter ring, ribbon choker, small glass earrings, simple festival circlet |
| Merchant and craft-guild display | Durable, visible, respectable jewellery with secular status | silver ring, stamped signet ring, merchant chain, guild-coloured bead string, brass belt plaque string, enamel-coloured brooch |
| Lesser gentry and professional elites | Fine pieces without royal extravagance | silver-gilt necklace, garnet-like ring, pearl earrings, fine armlet, gem-set bracelet, decorative girdle ornament |
| High nobility and court display | Rare, expensive, symbolic, and high-craft pieces | gold circlet, gemstone collar, jewelled chain, pearl necklace, heavy gold ring, enamelled diadem |
| Festival, courtship, wedding, and household gift jewellery | Socially meaningful but not automatically religious | love-knot ring, paired bracelets, wreath, token necklace, household-colour bead string, bridal garland |
| Ephemeral organic adornment | Short-lived pieces with morph targets where useful | fresh flower garland, herb bracelet, leaf crown, blossom anklet, festival wreath, dried garland |
| Child and apprentice jewellery | Smaller, cheaper, safer-feeling pieces | tiny copper ring, wooden bead necklace, cord bracelet, shell anklet, coloured glass bead string |

## Shared-first branch architecture

### Default rule

Create shared prototypes wherever the object can plausibly serve many cultures with only presentation changes. Most simple rings, copper rings, brass rings, silver rings, bead necklaces, glass bead strings, bone bracelets, shell bracelets, simple earrings, fresh garlands, leaf wreaths, gold chains, gem rings, brooches, pins, badges, and pearl necklaces should begin as shared prototypes.

### When to create a culture-specific or regional item

Create a separate regional or culture-family item when one or more of the following is true:

- The body slot or component differs, such as nose ring, toe ring, paired anklets, armlet, temple rings, hairpin set, forehead ornament, waist chain, or neck garland.
- The visible silhouette differs enough that a skin would mislead, such as a torc, paired oval brooches, temple rings, hairpin sets, broad bangles, coronets, diadems, or court circlets.
- The primary material changes behaviour or major visual identity, such as metal versus glass versus bone versus leaf versus fresh flower.
- The item is a culturally important jewellery system rather than decoration only, such as North-Sea bead-and-brooch sets, South Asian bangle/anklet/toe-ring sets, steppe belt-plaque display, Byzantine pearl-and-gold court ornaments, or East Asian hair ornaments.
- The item fills a social or gameplay gap: commoner affordability, merchant respectability, high-noble rank display, festival ephemerality, child/apprentice scale, court gift exchange, or functional sealing.

### What skins should carry instead

Do not create new base prototypes merely for:

- local fantasy-culture names
- exact owner, household, clan, dynasty, guild, retinue, or court marks
- heraldic devices
- love mottoes
- exact gemstone colour variation when a variable gem component can carry it
- exact enamel patterning
- religious meaning that should live in a devotional branch
- floral species where `Variable_Flower` or skins can carry the exact flower without changing behaviour
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
- `medieval_jewellery_pin_...`
- `medieval_jewellery_badge_...`
- `medieval_jewellery_hair_...`
- `medieval_jewellery_waist_...`
- `medieval_jewellery_garland_...`
- `medieval_jewellery_regional_<family>_...`

Keep identifiers lowercase snake case. Do not encode a real-world culture label into public `sdesc`; the unique reference can use builder-facing buckets when needed.

## Seeder and project grounding rules

- Use the project-standard `CreateItem(...)` seeder call shape during implementation.
- `uniqueReference` values use lowercase snake case ASCII and should remain stable once accepted.
- `noun` is compact and singular, usually the object form: `ring`, `necklace`, `chain`, `bracelet`, `bangle`, `armlet`, `anklet`, `earring`, `stud`, `choker`, `circlet`, `wreath`, `garland`, `brooch`, `pin`, `badge`, `pendant`, `bead-string`, `hairpin`, `comb`, `torc`, `diadem`, `coronet`, or `waist-chain` where appropriate.
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

## Component support strategy

### Current supported jewellery wear slots

The seeded component inventory now directly supports the following jewellery-relevant wearable components. Use these exact component names rather than approximating with hats, headbands, generic waist wear, or inert objects.

| Jewellery form | Preferred component(s) | Notes |
|---|---|---|
| Finger rings | `Wear_Ring` | Use for decorative rings, love rings, signet-like rings, gem rings, and simple metal bands. |
| Functional signet rings | `Wear_Ring` plus one `SealStamp_Medieval_*` component | Add seal-stamp behaviour only when the ring should actually apply seals. |
| Necklaces and chains | `Wear_Necklace` | Use for bead strings, chains, pendants, loose collars, and many ordinary neck ornaments. |
| Chokers and close collars | `Wear_Choker` | Use for narrow collars, close bead strings, ribbon chokers, and tight court necklaces. |
| Torcs and rigid neck rings | `Wear_Torc`, `Wear_Neck_Ring` | Use `Wear_Torc` for torc-like forms and `Wear_Neck_Ring` for more generic rigid neck rings. |
| Bracelets and bangles | `Wear_Bracelet`, `Wear_Bracelets` | Use singular or paired components according to the item description. |
| Armlets | `Wear_Armlet` | Use for upper-arm ornaments, arm rings, and armlet rows. |
| Anklets | `Wear_Anklet`, `Wear_Anklets` | Use singular or paired anklets. |
| Earrings | `Wear_Earring`, `Wear_Earrings` | Use singular or paired earrings. Paired earrings should normally use the plural component. |
| Nose rings and nose studs | `Wear_Nose_Ring` | Use for South Asian and other culturally justified rows; do not make it a universal default. |
| Toe rings | `Wear_Toe_Ring` | Use mainly for South Asian-inspired rows unless a local fantasy setting expands the form. |
| Brooches | `Wear_Brooch`, `Wear_Brooches` | Use for annular, ring, disc, penannular, paired oval, and court brooch forms. |
| Pins | `Wear_Pin` | Use for cloak pins, ring pins, dress pins, straight pins, and ornamental garment pins. |
| Badges | `Wear_Badge` | Use for secular household, civic, love, merchant, guild, and livery-adjacent badges. Keep pilgrim badges out of this branch. |
| Hairpins | `Wear_Hairpin`, `Wear_Hairpins` | Use for single, paired, and set-like hairpin ornaments. |
| Hair combs | `Wear_Hair_Comb`, `Wear_Hair_Combs` | Use for comb-backed hair ornaments, including tortoiseshell, wood, ivory, and metal combs. |
| Generic hair ornaments | `Wear_Hair_Ornament`, `Wear_Hair_Ornaments` | Use for plaques, hair tablets, hair rings, and hard-to-classify hair jewellery. |
| Temple rings | `Wear_Temple_Rings` | Use for Rus/Novgorod, Magyar, Slavic, and steppe temple-ring-like ornaments. |
| Circlets | `Wear_Circlet` | Use for light metal circlets, pearl circlets, fine head jewellery, and court circlets. |
| Diadems | `Wear_Diadem` | Use for narrow elite forehead/head ornaments. |
| Coronets | `Wear_Coronet` | Use for lesser noble head regalia and high-status display without full crown language. |
| Crowns | `Wear_Crown` | Use sparingly for true crown/regalia pieces if the item catalogue wants accessible regalia. |
| Chaplets | `Wear_Chaplet` | Use for ribbon, leaf, flower, and festival chaplets. |
| Wreaths | `Wear_Wreath` | Use for leaf wreaths, laurel-style wreaths, ivy wreaths, dried wreaths, and festival wreaths. |
| Head garlands | `Wear_Head_Garland` | Use for head-worn flower garlands and blossom garlands. |
| Neck garlands | `Wear_Neck_Garland` | Use for neck-worn flower garlands, South Asian festival garlands, and court/festival garlands. |
| Wrist garlands | `Wear_Wrist_Garland` | Use for flower wrist garlands, herb wrist garlands, and festival bracelet-garlands. |
| Ankle garlands | `Wear_Ankle_Garland` | Use for blossom anklets and festival ankle adornment. |
| Waist chains | `Wear_Waist_Chain` | Use for decorative waist chains. Do not add belt capacity. |
| Girdle ornaments | `Wear_Girdle_Ornament` | Use for dangling girdle ornaments, girdle jewels, and decorative girdle mounts. |
| Belt ornaments and belt plaques | `Wear_Belt_Ornament`, `Wear_Belt_Plaques` | Use for jewellery-like belt-mounted display pieces. Do not use `Belt_*` unless attachment capacity is intended. |
| Generic waist ornaments | `Wear_Waist_Ornament` | Use for waist jewellery that is not specifically a chain, belt plaque, or girdle jewel. |
| Forehead ornaments | `Wear_Forehead_Ornament` | Use for South Asian head jewellery, forehead pendants, brow ornaments, and cautious court head ornaments. |
| Brow or lip rings | `Wear_Brow_Ring`, `Wear_Lip_Ring` | Not ordinary defaults for the strict 500-1300 culture set. Reserve for fantasy/local variants or explicitly justified regional content. |

### Signet-ring seal-stamp components

Use these only when the ring should actually apply seals. A purely decorative signet-like ring should not carry a seal-stamp component.

| Component | Use |
|---|---|
| `SealStamp_Medieval_RingSignet` | Generic functional wearable signet ring. |
| `SealStamp_Medieval_PersonalSignetRing` | Personal or household signet ring. |
| `SealStamp_Medieval_MerchantSignetRing` | Merchant, professional, notarial, guild, or trade signet ring. |
| `SealStamp_Medieval_NobleSignetRing` | Noble, court, or high-status signet ring. |

Seal designs, owner names, devices, guild signs, and heraldry should live in skins, seal metadata, or written content rather than being hardcoded into base rows.

### Ordinary component pattern

Most mechanically wearable jewellery rows should use:

- `Holdable`
- exactly one wearable component, except for functional signet rings that may additionally use one seal-stamp component
- exactly one destroyable component
- optional variable components where the public descriptions use their tokens

Most intentionally loose, display-only, or craft-stock-adjacent ornaments should use:

- `Holdable`
- exactly one destroyable component
- optional variable components

Do not add armour, insulation, pockets, containers, locks, writing surfaces, sealable targets, light sources, weapon components, or trait-changing components to ordinary decorative jewellery.

### Destroyable component mapping

| Material or object family | Default destroyable component | Notes |
|---|---|---|
| Gold, silver, brass, bronze, copper, electrum, pewter, iron, steel, silver-gilt, gilded bronze, gilded copper, niello-dominant ornaments, and other dense metal jewellery | `Destroyable_HeavyMetal` | Use even for small objects when the item is primarily metal. |
| Glass bead strings, fragile glass earrings, lead-glass pieces, enamel-dominant ornaments, faience beads, and fragile inlay-dominant rows | `Destroyable_Glassware` | A metal ring with a glass, enamel, or faience detail can still use `Destroyable_HeavyMetal`. |
| Bone, horn, ivory, shell, cowrie shell, conch shell, mother-of-pearl, nacre, tortoiseshell, leather cord, wood, linen, silk, hemp, thread, leaf, herb, flower, petal, rush, and straw pieces | `Destroyable_Misc` or `Destroyable_Clothing` | `Destroyable_Misc` is usually adequate. Use `Destroyable_Clothing` only for textile-dominant wearable bands, ribbons, or fabric ornaments. |
| Paper or parchment festival tokens | `Destroyable_Paper` | Only for intentionally paper/parchment ornaments. |

### Ephemeral and morphing implementation rules

Fresh organic jewellery is a narrow exception to the no-morph default.

- Fresh garlands, wreaths, herb bracelets, flower strings, blossom anklets, and chaplets should normally have a wilted or dried morph target.
- The morph target should be authored before fresh variants that point to it.
- Use `morphToUniqueReference`, `morphEmote`, and `morphTimer` only when the row should automatically age.
- A simple policy is: fresh -> wilted after 6-24 in-game hours; wilted -> dried after 1-3 in-game days only if a dried version is useful as an object.
- Use exact seeded materials such as `flower`, `fresh flower`, `wilted flower`, `dried flower`, `petal`, `dried petal`, `rose`, `violet`, `daisy`, `jasmine`, `lotus flower`, `marigold`, `lily`, `chrysanthemum`, `blossom`, `ivy`, `laurel`, `rush`, or `straw` where appropriate.
- Fresh organic jewellery should have low cost and low weight. It should remain skinnable so local flowers, colours, festival meanings, and symbolic associations can be supplied without new behaviour rows.

Example morph pattern, not final item text:

1. `medieval_jewellery_garland_wilted_flower_chaplet` as the first morph target.
2. `medieval_jewellery_garland_dried_flower_chaplet` as the optional final retained object.
3. `medieval_jewellery_garland_fresh_flower_chaplet` morphs to the wilted target after a moderate timer.
4. Public `fdesc` describes visible freshness only; the morph emote handles wilting.

## Player-facing description rules

- Do not mention skins, base prototypes, standard implementations, seeder mechanics, builders, archaeology, uncertainty, or component implementation in `noun`, `sdesc`, `ldesc`, or `fdesc`.
- Public text should describe visible form and material: band, hoop, link, bead, wire, pin, catch, hinge, clasp, setting, bezel, cabochon, pendant loop, chain, tablet, plaque, brooch pin, hairpin point, comb teeth, filigree-like twist, stamped pattern, punched dotting, enamel colour, niello-dark inlay, faience glaze, glass inlay, pearl-like bead, polished surface, tarnish, wear, cord, knot, woven thread, fresh petals, wilted stems, or drying leaves.
- Public text should avoid real-world culture labels such as Anglo-Saxon, Norse, Norman, Frankish, Byzantine, Abbasid, Fatimid, Seljuk, Rus, Mongol, Indian, Chinese, Korean, or Japanese unless the object term itself is widely intelligible and appropriate.
- Form names are acceptable when they identify the visible object rather than a people. Examples: `ring`, `necklace`, `chain`, `bead string`, `bracelet`, `bangle`, `armlet`, `anklet`, `earring`, `nose ring`, `toe ring`, `choker`, `circlet`, `garland`, `wreath`, `chaplet`, `brooch`, `pin`, `badge`, `pendant`, `collar`, `torc`, `diadem`, `coronet`, `hairpin`, `comb`, `temple ring`, `forehead ornament`, `girdle ornament`, `waist chain`, and `belt plaque`.
- Do not make every item elite. Commoner descriptions should allow rough edges, simple wire, plain cord, mismatched beads, bone polish, shell chips, copper tarnish, and local material.
- Avoid gender-default naming. Do not create unmarked `ring` plus marked `woman's ring` unless gender is visible in the object form. Builder-facing manifests can place jewellery in gendered outfit plans, but public item text should usually prefer form and social context.
- Do not claim an item is sacred, protective, blessed, magical, royal by law, poison-bearing, a key, a seal, a coin purse, a storage locket, or a hiding place unless mechanics and branch scope support that claim.
- Inscriptions should be handled by skins, writing blocks, seal metadata, or another content system. Avoid embedding exact readable text in base `fdesc` unless a deliberate writing block is used.

## Skin strategy

- All finished decorative jewellery should normally be skinnable.
- A skin can override presentation fields and quality without changing behaviour.
- Skins should carry high-variance presentation: exact gemstones, glass colours, enamel colour, local fantasy names, workshop marks, heraldic devices, clan or household signs, guild marks, love knots, owner marks, dynastic emblems, animal motifs, foliage motifs, border patterns, knotwork, granulation-like texture, filigree-like wire patterning, stamped marks, bead ordering, court colours, flower species, festival associations, and decorative inscriptions.
- Default unskinned items still need to be credible standalone objects. Skinability must not excuse bland base descriptions.
- Player-facing base descriptions should never say that a skin may later change the appearance.
- For jewellery whose exact cultural name is important but not universal, put that name in the skin rather than the base row unless the form itself is the only clear player-facing noun.

## Colour, gem, and variable policy

Use variables when the same behaviour and construction can support multiple visible materials, colours, motifs, stones, or floral variants.

| Variable component | Token(s) | Jewellery use |
|---|---|---|
| `Variable_CommonStone` | `$commonstone`, `$stone` | Ordinary cabochons, glass-like stones, agate-like stones, bead strings, trade stones, and low-to-mid value jewellery. |
| `Variable_Gem` | `$gemcolor`, `$gem` | General gemstone jewellery where a broad gem vocabulary is desired. Good for merchant, gentry, and elite pieces that should vary by stone. |
| `Variable_FineGem` | `$finegemcolor` | High noble, court, royal, very fine, or rare gemstone jewellery. Use sparingly. |
| `Variable_BasicColour` | `$colour` | Textile cords, ribbons, simple enamel-like paint, fresh garland colours, and ordinary coloured glass where basic colour words suffice. |
| `Variable_FineColour` | `$colour`, `$finecolor` | Elite enamel panels, court ribbons, fine textile backing, special festival colours, and richer colour vocabulary. |
| `Variable_2BasicColour` | `$colour1`, `$colour2` | Simple two-colour bead strings, cords, or festival garlands. |
| `Variable_2FineColour` | `$colour1`, `$colour2` | Elite two-colour bead, enamel, or ribbon-backed jewellery. |
| `Variable_JewelleryMotif` | `$motif` | Decorative motifs on rings, brooches, pins, badges, chains, plaques, and circlets. |
| `Variable_Flower` | `$flower` | Fresh garlands, chaplets, wreaths, flower bracelets, blossom anklets, and festival garlands. |
| `Variable_MetalFinish` | `$finish` | Metal rings, bracelets, brooches, chains, badges, pins, plaques, and belt ornaments. |
| `Variable_BeadPattern` | `$beadpattern` | Bead strings, necklaces, bracelets, anklets, temple ornaments, and mixed bead rows. |
| `Variable_JewelleryShape` | `$shape` | Brooches, badges, pendants, plaques, beads, ring settings, and small enamel panels. |
| `Variable_InlayStyle` | `$inlay` | Enamel panels, niello-like lines, glass inlay, shell inlay, and decorative inlay language. |
| `Variable_DrabColour` and `Variable_2DrabColour` | `$colour`, `$drabcolor`, `$colour1`, `$colour2` | Only for intentionally tarnished, old, grimy, neglected, or poor-condition ornaments. Do not use as the default for simple common jewellery. |

Variable usage must be grammatical. Examples:

- `a $gem silver ring`
- `a $commonstone bead necklace`
- `a $colour braided cord bracelet`
- `a $colour1 and $colour2 glass bead string`
- `a fine $finegemcolor gold circlet`
- `a $motif silver brooch`
- `a $flower neck garland`
- `a $finish bronze pin`
- `a $beadpattern glass bead necklace`
- `a $shape enamel badge`
- `a $inlay silver-gilt hairpin`

Where the stone or ornament materially changes the primary material, create separate rows only if behaviour or category changes. A gold ring set with amber should usually use `gold` as primary material and mention amber in `fdesc` or a variable. A necklace made mostly of amber beads should use `amber` as primary material.

## Materials, sizes, quality, and cost assumptions

### Recommended primary materials

Use exact seeded material names. Suitable currently seeded materials include:

- Metals: `gold`, `silver`, `sterling silver`, `silver-gilt`, `copper`, `brass`, `bronze`, `gilded bronze`, `gilded copper`, `electrum`, `pewter`, `lead`, `wrought iron`, `mild steel`, `carbon steel`, `niello`.
- Gemstones and stones: `agate`, `amber`, `amethyst`, `aquamarine`, `aventurine`, `beryl`, `bloodstone`, `carnelian`, `chalcedony`, `citrine`, `diamond`, `emerald`, `garnet`, `jade`, `jasper`, `lapis lazuli`, `moonstone`, `onyx`, `opal`, `pearl`, `quartz`, `rock crystal`, `rose quartz`, `ruby`, `sapphire`, `smoky quartz`, `sunstone`, `topaz`, `turquoise`, `zircon`, and other exact seeded gemstone names.
- Shell, hard organic, and animal-derived ornament: `coral`, `mother-of-pearl`, `nacre`, `cowrie shell`, `conch shell`, `tortoiseshell`, `bone`, `horn`, `ivory`, `shell`, `jet`.
- Organic and low-cost materials: `leather`, `linen`, `silk`, `hemp`, `wool`, `leaf`, `lavender`, `chamomile`, `safflower`, `sunflower`, `flower`, `fresh flower`, `wilted flower`, `dried flower`, `petal`, `dried petal`, `rose`, `violet`, `daisy`, `jasmine`, `lotus flower`, `marigold`, `lily`, `chrysanthemum`, `blossom`, `ivy`, `laurel`, `rush`, `straw`, `wood`, `boxwood`, `rosewood`, `sandalwood`, `willow`, `bamboo`.
- Glass and ceramic-like materials: `glass`, `soda-lime glass`, `lead glass`, `faience`, `enamel`, `ceramic`, `porcelain` where form and period support them.

Aliases may exist for some materials, but item rows should use canonical exact material names. Prefer `silver-gilt` over `vermeil`, `gilded bronze` over `gilt bronze`, `gilded copper` over `gilt copper`, `mother-of-pearl` over `mother of pearl`, `cowrie shell` over `cowrie`, and `conch shell` over `conch` in the `material` field.

### Primary material policy

Primary material represents the dominant substance or behaviour-relevant body of the item, not every decorative inclusion.

- Metal ring with stone: primary material is usually the metal.
- Brooch made from silver-gilt: primary material can be `silver-gilt`.
- Gilded copper badge: primary material can be `gilded copper`.
- Niello-inlaid silver ring: primary material is usually `silver`; `niello` can appear in `fdesc` or as an inlay-style variable unless the item is niello-dominant craft stock.
- Bead necklace dominated by glass beads: primary material can be `glass`.
- Amber bead string: primary material can be `amber`.
- Pearl necklace: primary material can be `pearl`.
- Coral bead necklace: primary material can be `coral`.
- Mother-of-pearl hair comb or shell-inlay pendant: primary material can be `mother-of-pearl` when shell is dominant.
- Floral wreath or garland: primary material can be `fresh flower`, a specific flower, `ivy`, `laurel`, `rush`, or `straw` according to visible dominance and morph state.
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
| Hairpin, hair comb, temple rings | `Tiny`, `VerySmall`, or `Small` | 10-250g |
| Circlet, diadem, chaplet, fresh garland, wreath | `Small` | 25-400g |
| High regalia, coronet, crown, or heavy noble collar | `Small` or rarely `Normal` | 300-1500g |

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
| Ephemeral organic | 1-8m | fresh flower garland, herb bracelet, festival wreath |
| Cheap commoner | 2-24m | copper ring, bone bead bracelet, shell anklet, wooden bead necklace |
| Ordinary urban | 12-72m | glass bead necklace, brass bangle, pewter ring, simple earrings |
| Merchant / professional | 48-240m | silver ring, silver bracelet, small pearl earrings, respectable chain |
| Lesser noble / gentry | 180-960m | silver-gilt necklace, gold ring, gem-set bracelet, fine circlet |
| High noble / court | 960-5000m+ | jewelled gold chain, pearl collar, gemstone circlet, heavy noble regalia |

Metal and gemstone values should dominate cost more than weight. A tiny gold gem ring can cost far more than a heavy bronze armlet.

## Tagging policy

Use exact seeded tag paths. Finished decorative jewellery should use a jewellery function tag and one `Market / Jewellery` tag whenever possible. Do not classify finished jewellery as household wares unless it is also deliberately a household good.

### Core jewellery function tags

Use one or more of these exact function tags according to form:

```text
Functions / Worn Items / Jewellery
Functions / Worn Items / Jewellery / Anklets
Functions / Worn Items / Jewellery / Armlets
Functions / Worn Items / Jewellery / Badges
Functions / Worn Items / Jewellery / Bead Strings
Functions / Worn Items / Jewellery / Belt Ornaments
Functions / Worn Items / Jewellery / Belt Plaques
Functions / Worn Items / Jewellery / Bracelets
Functions / Worn Items / Jewellery / Brooches
Functions / Worn Items / Jewellery / Chaplets
Functions / Worn Items / Jewellery / Chokers
Functions / Worn Items / Jewellery / Circlets
Functions / Worn Items / Jewellery / Coronets
Functions / Worn Items / Jewellery / Crowns
Functions / Worn Items / Jewellery / Diadems
Functions / Worn Items / Jewellery / Earrings
Functions / Worn Items / Jewellery / Forehead Ornaments
Functions / Worn Items / Jewellery / Garlands
Functions / Worn Items / Jewellery / Girdle Ornaments
Functions / Worn Items / Jewellery / Hair Ornaments
Functions / Worn Items / Jewellery / Head Ornaments
Functions / Worn Items / Jewellery / Neck Garlands
Functions / Worn Items / Jewellery / Neck Rings
Functions / Worn Items / Jewellery / Necklaces
Functions / Worn Items / Jewellery / Pendants
Functions / Worn Items / Jewellery / Pins
Functions / Worn Items / Jewellery / Piercings
Functions / Worn Items / Jewellery / Rings
Functions / Worn Items / Jewellery / Temple Rings
Functions / Worn Items / Jewellery / Toe Rings
Functions / Worn Items / Jewellery / Torcs
Functions / Worn Items / Jewellery / Waist Chains
Functions / Worn Items / Jewellery / Waist Ornaments
Functions / Worn Items / Jewellery / Wreaths
```

### Piercing sub-tags

```text
Functions / Worn Items / Jewellery / Piercings / Ear Studs
Functions / Worn Items / Jewellery / Piercings / Nose Rings
Functions / Worn Items / Jewellery / Piercings / Nose Studs
```

### Garland and wreath refinements

Use these where an ephemeral or seasonal row benefits from finer search/classification:

```text
Functions / Worn Items / Jewellery / Garlands / Dried Garlands
Functions / Worn Items / Jewellery / Garlands / Flower Garlands
Functions / Worn Items / Jewellery / Garlands / Fresh Garlands
Functions / Worn Items / Jewellery / Garlands / Herb Garlands
Functions / Worn Items / Jewellery / Garlands / Leaf Garlands
Functions / Worn Items / Jewellery / Wreaths / Dried Wreaths
Functions / Worn Items / Jewellery / Wreaths / Flower Wreaths
Functions / Worn Items / Jewellery / Wreaths / Fresh Wreaths
Functions / Worn Items / Jewellery / Wreaths / Herb Wreaths
Functions / Worn Items / Jewellery / Wreaths / Leaf Wreaths
```

### Market tags

Use one or more of these exact market tags according to social band and value:

```text
Market / Jewellery
Market / Jewellery / Children's Jewellery
Market / Jewellery / Commoner Jewellery
Market / Jewellery / Court Jewellery
Market / Jewellery / Ephemeral Jewellery
Market / Jewellery / Festival Jewellery
Market / Jewellery / Luxury Jewellery
Market / Jewellery / Merchant Jewellery
Market / Jewellery / Noble Jewellery
Market / Jewellery / Professional Jewellery
Market / Jewellery / Regalia
Market / Jewellery / Simple Jewellery
Market / Jewellery / Standard Jewellery
```

Market usage guidance:

- `Market / Jewellery / Simple Jewellery`: cheap copper, bone, shell, wood, cord, common glass, rush, straw, and simple commoner rows.
- `Market / Jewellery / Standard Jewellery`: ordinary market jewellery, pewter/brass/bronze rows, modest glass or silver rows.
- `Market / Jewellery / Luxury Jewellery`: precious metals, gems, pearls, fine craft, elite merchant pieces, and gentry jewellery.
- `Market / Jewellery / Court Jewellery`: court chains, circlets, diadems, high-status gold/gem rows, and formal display pieces.
- `Market / Jewellery / Festival Jewellery`: fresh garlands, flower chaplets, ribbons, courtship pieces, and seasonal rows.
- `Market / Jewellery / Children's Jewellery`: child-sized bead strings, tiny rings, cord bracelets, and harmless small adornments.
- `Market / Jewellery / Merchant Jewellery`: respectable urban display pieces, signet-like rings, merchant chains, guild-colour bead strings.
- `Market / Jewellery / Noble Jewellery`: high noble but not necessarily royal pieces.
- `Market / Jewellery / Regalia`: rare crowns, coronets, symbolic court collars, and regalia-adjacent rows.
- `Market / Jewellery / Ephemeral Jewellery`: short-lived organic pieces with morph targets or festival disposability.

Do not use `Functions / Material Functions / Jewellery Craft Stock` tags for finished jewellery. Those tags are for raw or intermediate jewellery-making stock, such as wire, settings, beads, and metal stock.

## Historical and design assumptions by inspiration family

### Early Anglo-Saxon

Reference anchor: c. 800AD. Support common bead strings, simple rings, copper or bronze pieces, glass and amber-like beads, disc brooches, pins, and elite gold/silver/garnet-like display. Public descriptions should focus on round shapes, punched decoration, inlay colour, and practical pin backs rather than ethnic labels.

### Anglo-Danish

Reference anchor: c. 950AD. Use a hybrid North-Sea vocabulary: bead strings, arm rings, simple rings, silver and bronze ornaments, glass beads, amber, brooches, paired brooches, and clothing ornaments. Hangerok-related brooches should not be duplicated from clothing unless the separate object matters to gameplay.

### Norse / Viking Age

Reference anchor: c. 950AD. This family needs bead festoons, arm rings, neck rings, torcs, simple finger rings, amber and glass bead strings, silver display pieces, and paired brooch silhouettes. Jewellery may be economically meaningful as portable wealth, but public descriptions should not call pieces currency unless they have currency behaviour.

### Norman

Reference anchor: c. 1066AD. The Norman plan should be restrained compared with later Capetian and High English jewellery: rings, simple brooches, cloak pins, belt fittings, silver or gilded ornament, and a few elite gem pieces. Avoid high-late-medieval livery badges and elaborate court jewels as defaults.

### Anglo-Norman

Reference anchor: c. 1150AD. Increase urban and court variety: annular brooches, ring brooches, simple signet-like rings, small chains, necklaces, girdle ornaments, glass or gem settings, and respectable merchant jewellery. Keep sacred badges and pilgrim signs outside this decorative branch.

### High English / British

Reference anchor: c. 1250AD. This branch supports the later end of the medieval slice: signet-like rings, functional signet rings where intended, gold rings, gem rings, annular brooches, fine chains, circlets, girdle ornaments, secular household badges, and high-noble display. Heraldry belongs mostly in skins.

### Irish / Gaelic

Reference anchor: c. 1100AD. Brooches, ring pins, penannular forms, cloak fasteners, amber/glass beads, silver rings, and torc-like collars are key visual anchors. Public text should use visible forms such as `ring brooch`, `cloak pin`, `bead necklace`, or `silver armlet` rather than culture labels.

### Scottish / Gaelic-Lowland

Reference anchor: c. 1250AD. Use both Gaelic cloak-fastener traditions and Lowland court/urban jewellery. Include silver brooches, rings, amber or glass bead strings, practical pins, merchant pieces, and noble pieces. Avoid projecting later specifically early-modern forms backwards.

### Carolingian / Frankish

Reference anchor: c. 800AD. Include brooches, belt ornaments, bead strings, simple rings, elite gold, garnet-like colour, silver and bronze pieces, and courtly display. Commoner pieces should remain mostly glass, bronze, copper, bone, shell, and cord.

### Capetian French

Reference anchor: c. 1200AD. This family can carry rich western court jewellery: gem rings, gold chains, silver-gilt necklaces, fine brooches, delicate circlets, love-token rings, and courtly belt ornaments. Avoid later fourteenth- and fifteenth-century extremes.

### Holy Roman Empire / German

Reference anchor: c. 1200AD. Use strong urban and court goldsmithing cues: rings, chains, brooches, civic ornaments, silver/gilt display, enamel colour, niello-like detail, and merchant-class respectability. Guild marks and city symbols should usually be skins.

### Christian Iberian

Reference anchor: c. 1200AD. The jewellery plan should sit between western European and Islamicate forms: earrings, rings, necklaces, bracelets, girdle ornaments, coloured glass, filigree-like wire, pearls, and gold/silver urban jewellery. Public base text should avoid dynastic or religious claims.

### Andalusian

Reference anchor: c. 1100AD. Include urban luxury jewellery: earrings, necklaces, bracelets, anklets, rings, filigree-like and granulated-looking work, pearls, coral, coloured glass, and gold or silver pieces. Keep religious inscriptions and devotional amulets out of base rows.

### Byzantine

Reference anchor: c. 1000AD. The family should have high-status gold, pearls, glass inlay, enamel panels, earrings, necklaces, bracelets, rings, court chains, diadems, and circlets. Decorative crosses and reliquary pendants belong elsewhere unless the item is deliberately secularized by skin.

### Abbasid

Reference anchor: c. 850AD. Include court and urban jewellery: rings, earrings, bracelets, anklets, bead strings, pearls, glass, gold, silver, and fine wire-like detail. Scholarly or official signet usage can be handled by decorative signet rings or functional signet-ring components where mechanical stamping is intended.

### Fatimid

Reference anchor: c. 1050AD. Warm-climate urban jewellery should emphasize earrings, bracelets, anklets, necklaces, beads, pearls, coral, glass, gold, and silver. Add merchant and court versions without making all pieces devotional or ceremonial.

### Seljuk / Ayyubid-Mamluk

Reference anchor: c. 1200AD. Include mounted-elite and urban jewellery: rings, armlets, bracelets, anklets, pendants, belt ornaments, chains, earrings, turquoise-like and carnelian-like colour, silver, and gold. The catalogue should support both practical travel display and courtly luxury.

### Magyar

Reference anchor: c. 950AD. Focus on mobile elite display: belt plaques, pendant ornaments, earrings, silver rings, temple rings, beads, and animal or geometric motif skins. Use the waist and belt ornament components for jewellery-like display rather than functional belt capacity.

### Rus / Novgorod

Reference anchor: c. 1100AD. Use bead strings, temple rings, silver rings, pendants, bracelets, amber, glass, and Byzantine-influenced elite forms. Cold-climate layers should not prevent visible jewellery; many items can be displayed at neck, wrist, ear, temple, or head.

### Steppe Turkic / Mongol

Reference anchor: c. 1200AD. Focus on portable wealth and mounted display: belt ornaments, belt plaques, earrings, rings, beads, pendants, silver and gilt pieces, head ornaments, and hair ornaments. Avoid magical talisman mechanics even where shapes look amulet-like.

### North Indian / Rajput

Reference anchor: c. 1100AD. This family needs broad jewellery coverage across body slots: necklaces, earrings, bangles, bracelets, anklets, toe rings, nose rings, forehead ornaments, head ornaments, pearls, gold, silver, glass, conch shell, and fresh garlands. This is one of the strongest justifications for `Wear_Nose_Ring`, `Wear_Toe_Ring`, `Wear_Forehead_Ornament`, paired bracelets, paired anklets, and neck garlands.

### South Indian / Chola

Reference anchor: c. 1050AD. Use gold-heavy court and temple-adjacent decorative forms while keeping devotional function out of the base rows: layered necklaces, bangles, anklets, toe rings, earrings, forehead ornaments, head ornaments, pearls, gems, conch shell, jasmine, lotus, marigold, and fresh flower garlands. Floral jewellery should be represented as short-lived morphing objects where useful.

### Song China

Reference anchor: c. 1100AD. Rings are less central than hair and head ornaments, pins, combs, pendants, jade-like pieces, gold, and restrained bead strings. Use the hairpin, hair comb, hair ornament, circlet, and forehead ornament components where they fit visible form.

### Goryeo Korea

Reference anchor: c. 1150AD. Include restrained court ornaments, hairpins, hair ornaments, pendants, jade-like stones, beads, rings where useful, and gilt metal. Avoid overproducing western-style ring and necklace forms for this family.

### Heian / Kamakura Japan

Reference anchor: c. 1200AD. Jewellery coverage should emphasize hair ornaments, combs, cords, bead strings, small pendants, and understated elite display rather than abundant finger rings. Use hairpin, hair comb, hair ornament, and head ornament slots where appropriate.

## Category implementation rules

### Rings and signet-like rings

- Rings are the safest and most broadly supported jewellery category because `Wear_Ring` exists.
- Include common copper and bronze rings, simple iron rings, bone rings, silver rings, gold rings, gem rings, love-token rings, merchant signet-like rings, functional signet rings, and court rings.
- Decorative signet-like rings use only `Wear_Ring`.
- Functional signet rings use `Wear_Ring` plus exactly one appropriate `SealStamp_Medieval_*` component.
- Do not promise legal authority, seal compatibility, or ownership recognition unless the row includes seal-stamp behaviour and the relevant presentation exists in skin, metadata, or written content.
- Gem-set rings should generally use the metal as primary material and use `Variable_CommonStone`, `Variable_Gem`, or `Variable_FineGem` for visible stones.

### Necklaces, chains, chokers, collars, neck rings, and torcs

- Use `Wear_Necklace` for most necklaces, chains, pendants, bead strings, and loose collars.
- Use `Wear_Choker` for close-fitting collars, ribbons, and short bead strings.
- Use `Wear_Torc` for torc-like rigid collars.
- Use `Wear_Neck_Ring` for non-torc rigid neck rings.
- Use `Wear_Neck_Garland` for flower, leaf, and festival garlands worn around the neck.
- Include common bead strings, cord necklaces, shell necklaces, glass beads, amber beads, coral bead necklaces, pearl necklaces, silver chains, gold chains, gem collars, and court display collars.
- Religious pendants, cross pendants, scripture pendants, relic lockets, and prayer beads are not part of this branch.

### Bracelets, bangles, armlets, anklets, toe rings, and garlands

- Use `Wear_Bracelet` or `Wear_Bracelets` for wrist ornaments.
- Use `Wear_Armlet` for upper-arm rings and armlets.
- Use `Wear_Anklet` or `Wear_Anklets` for ankle ornaments.
- Use `Wear_Toe_Ring` for toe rings, mainly in South Asian-inspired rows.
- Use `Wear_Wrist_Garland` and `Wear_Ankle_Garland` for flower/herb wrist and ankle garlands.
- Include common cord bracelets, copper bracelets, bone bracelets, shell bracelets, glass bangles, brass bangles, silver bracelets, gold bangles, arm rings, paired anklets, toe rings, and elite gem-set pieces.
- South Asian coverage should include bangles, anklets, toe rings, nose rings, and garlands as major categories rather than minor variants.

### Earrings and piercings

- Use `Wear_Earring` and `Wear_Earrings` for ordinary earrings.
- Use `Wear_Nose_Ring` for nose rings and nose studs where culturally justified.
- Use `Wear_Toe_Ring` for toe rings where culturally justified.
- Avoid default use of `Wear_Brow_Ring`, `Wear_Lip_Ring`, `Wear_Bellybutton_Ring`, `Wear_Nipple_Ring`, `Wear_Tongue_Ring`, and `Wear_Penis_Ring` in the strict 500-1300 catalogue unless a fantasy/local expansion asks for them.
- Include studs, hoops, pendant earrings, bead earrings, pearl earrings, simple wire earrings, and elite gold or gem earrings.

### Brooches, pins, badges, and fasteners

- Brooches and pins are historically central and are now mechanically supported.
- Use `Wear_Brooch` for single brooches and `Wear_Brooches` for paired brooch sets.
- Use `Wear_Pin` for cloak pins, ring pins, straight pins, dress pins, and decorative garment pins.
- Use `Wear_Badge` for secular badges.
- Descriptions may mention a pin, catch, shank, boss, terminal, hinge, clasp, and back where visible. Do not promise hidden storage, locking, or legal status without components.
- Exclude pilgrim badges and devotional badges from this decorative branch.
- Include secular badges, household badges, love badges, ring brooches, annular brooches, disc brooches, cloak pins, strap-end ornaments, and courtly jewelled pins where appropriate.

### Head ornaments, circlets, diadems, coronets, crowns, garlands, hair ornaments, and combs

- Use `Wear_Circlet` for light metal circlets, pearl circlets, and most court head rings.
- Use `Wear_Diadem` for narrow high-status forehead/head ornaments.
- Use `Wear_Coronet` for lesser noble head regalia.
- Use `Wear_Crown` sparingly for true crown/regalia pieces.
- Use `Wear_Chaplet`, `Wear_Wreath`, `Wear_Head_Garland`, and `Wear_Forehead_Ornament` where the form fits.
- Use `Wear_Hairpin`, `Wear_Hairpins`, `Wear_Hair_Comb`, `Wear_Hair_Combs`, `Wear_Hair_Ornament`, and `Wear_Hair_Ornaments` for hair jewellery.
- Include simple leaf wreaths, flower garlands, ribbon chaplets, brass circlets, silver circlets, gold circlets, pearl circlets, high noble gem circlets, tortoiseshell combs, jade-like hairpins, and gold hair ornaments.
- Floral and leafy head ornaments are a major way to include commoner and festival jewellery.

### Waist ornaments, chains, plaques, and girdle jewellery

- Decorative belts already overlap with clothing. This branch should include clearly jewellery-like waist ornaments: waist chains, dangling plaques, girdle jewel mounts, decorative belt plaques, and belt-mounted display pieces.
- Use `Wear_Waist_Chain` for complete waist chains.
- Use `Wear_Girdle_Ornament` for girdle jewels and dangling girdle ornaments.
- Use `Wear_Belt_Ornament` for a single belt-mounted ornament.
- Use `Wear_Belt_Plaques` for plaque arrays.
- Use `Wear_Waist_Ornament` as the fallback waist-jewellery slot.
- Do not use `Belt_2`, `Belt_4`, `Belt_6`, or `Belt_Large` unless the item is a functional belt designed to carry beltable items. Most jewellery waist chains are not functional belts.
- Do not use `Beltable` unless the item should attach to a belt as a separate beltable object.

### Bead strings and commoner jewellery

- Include a wide range of cheap bead and cord rows.
- Beads may be glass, faience, bone, shell, cowrie shell, conch shell, wood, amber, jet, clay/ceramic, coral, or stone.
- Commoner items should often be `Standard`, `Substandard`, or `Poor`, but should still be visually interesting.
- Avoid making commoner jewellery only shabby. Many inexpensive pieces can be neat, colourful, and meaningful.

### Merchant, guild, and professional jewellery

- Merchant and guild pieces should be respectable, legible, and durable without always being noble luxury.
- Suitable forms include silver rings, brass chains, merchant signet rings, guild-colour bead strings, stamped bracelets, small brooches, badges, belt plaques, and neat necklaces.
- Guild marks and exact professional signs should mostly be skins. Base rows can use generic phrases such as `a stamped silver ring`, `a merchant signet ring`, or `a brass merchant chain`.

### Noble, court, and regalia-adjacent jewellery

- High noble jewellery should not crowd out common pieces, but it must be present.
- Include gold rings, gem rings, pearl earrings, gem necklaces, gold bracelets, jewelled circlets, diadems, coronets, court chains, and heavy collars.
- Use `VeryGood` sparingly; reserve `Great` for a handful of exceptional regalia-like prototypes if the later implementation wants them.
- Do not create explicit royal crowns as default everyday jewellery unless the MUD wants accessible regalia objects. A `gold circlet`, `fine coronet`, or `jewelled diadem` is usually a safer base row than a named crown.

### Child, apprentice, festival, and courtship jewellery

- Include smaller and cheaper pieces that help populate social scenes: child bead necklaces, cord bracelets, shell anklets, tiny copper rings, festival wreaths, paired courtship bracelets, token rings, and seasonal garlands.
- Courtship and wedding pieces may be decorative and secular. Avoid religious vow language or sacred ceremony claims.
- Fresh festival pieces should use morphs where useful.

## Catalogue distribution target

The first complete implementation should aim for about 400 item prototypes. A useful starting distribution is:

| Category | Target rows | Notes |
|---|---:|---|
| Shared commoner and ephemeral adornment | 52 | cord, shell, bone, wood, flower, leaf, herb, rush, straw, glass, cheap copper, child/apprentice pieces |
| Rings and signet-like rings | 55 | simple rings, gem rings, love rings, merchant signets, functional signets, court rings |
| Neck adornment | 54 | bead strings, necklaces, chokers, chains, collars, neck rings, torcs, pendants without devotional meaning |
| Wrist, arm, ankle, and toe adornment | 58 | bracelets, bangles, armlets, anklets, toe rings, cord bands, elite sets, wrist/ankle garlands |
| Earrings and culturally justified piercings | 34 | studs, hoops, pendant earrings, paired earrings, nose rings, nose studs, toe rings |
| Brooches, pins, badges, and fasteners | 58 | wearable brooches, paired brooches, pins, secular badges, love badges, court pins |
| Head ornaments, hair ornaments, circlets, wreaths, and garlands | 45 | circlets, diadems, coronets, hairpins, combs, head garlands, wreaths, chaplets |
| Waist ornaments, belt plaques, hanging ornaments, and court chains | 26 | waist chains, girdle ornaments, belt plaques, belt ornaments, waist ornaments |
| Elite sets, regional statement pieces, and rare court goods | 18 | limited very high value items; avoid overproducing regalia |
| **Total** | **400** | Adjust during implementation if balance demands it. |

### Suggested work packages

| Pass | Target focus | Approximate rows |
|---|---|---:|
| 1 | Shared cheap/commoner jewellery, bead strings, simple rings, organic garlands | 45 |
| 2 | Shared rings, merchant signets, functional signet rings, gem rings | 45 |
| 3 | Necklaces, chokers, chains, collars, neck rings, torcs, bead strings | 45 |
| 4 | Bracelets, bangles, armlets, anklets, toe rings, garland bands | 45 |
| 5 | Earrings and culturally justified piercings | 30 |
| 6 | Brooches, paired brooches, pins, badges, and fasteners | 50 |
| 7 | Head ornaments, circlets, diadems, coronets, garlands, hairpins, combs | 45 |
| 8 | Regional shared groups: northern, western, Mediterranean, Islamicate, South Asian, East Asian, steppe | 65 |
| 9 | High noble, court, regalia-adjacent, and rare elite jewellery | 25 |
| 10 | Review, balance, duplicate removal, tag/material validation, and ephemeral morph validation | 5-10 net additions or replacements |

## Builder-facing jewellery set manifests

This branch does not need clothing-style outfit manifests for every culture, but the implementation should support builder-facing jewellery sets. These sets are optional scene-building collections rather than mandatory clothing outfits.

### Commoner festival set

- fresh flower chaplet
- braided cord bracelet
- shell bead necklace
- copper ring
- wilted garland morph target

### Urban merchant set

- stamped silver ring
- functional or decorative merchant signet ring
- brass merchant chain
- glass bead necklace
- neat silver bracelet
- small secular badge or brooch

### Northern elite set

- silver arm ring
- amber bead necklace
- gem-set ring
- ornate cloak brooch
- paired brooches where culturally suitable
- silver bead string

### Western court set

- gold gem ring
- pearl earrings
- silver-gilt necklace
- fine circlet
- jewelled brooch
- court pin or badge

### Islamicate urban luxury set

- gold earrings
- silver filigree-like bracelet
- pearl necklace
- gem-set ring
- paired anklets
- coral or coloured-glass bead string

### South Asian festival set

- fresh flower neck garland
- paired bangles
- paired anklets
- nose ring or nose stud
- toe ring
- forehead ornament
- gold bead necklace

### East Asian court ornament set

- jade-like pendant
- gilt hairpin
- decorative hair comb
- restrained bead string
- delicate gold ring where useful
- court head ornament if slot behaviour fits

### Steppe and caravan display set

- silver earrings
- belt plaque ornament
- bead necklace
- silver ring
- pendant chain
- temple-ring or hair ornament where culturally suitable

## Implementation validation checklist

Before emitting any `CreateItem(...)` row for this branch, confirm:

- The item is decorative jewellery or secular personal adornment, not devotional/religious goods.
- The `uniqueReference` is lowercase snake case and begins with a stable jewellery prefix.
- `noun`, `sdesc`, and `fdesc` are public, in-world, concise, and culture-neutral unless the form name itself is useful.
- `ldesc` is `null` unless the row is deliberately a display fixture, which should be rare or out of branch.
- `material` exactly matches a seeded solid material.
- Every tag exactly matches `SeededTagHierarchy.csv`.
- Finished jewellery uses appropriate exact jewellery/function tags and a `Market / Jewellery` tag.
- The item uses `Holdable` unless there is a specific reason not to.
- The item uses exactly one destroyable component.
- A wearable component is used where the item is meant to be worn.
- Functional signet rings use exactly one seal-stamp component and should not pretend to carry a specific owner/device unless metadata, skin, or writing supports it.
- No ordinary jewellery row receives `Armour_*`, `Insulation_*`, `Container_*`, `LockingContainer_*`, `Sealable_*`, `Light`, `Weapon`, identity-obscuring, hidden-storage, or trait-changing components by default.
- Variable components match variables actually used in public descriptions.
- Ephemeral items have valid morph targets, timers, and morph emotes when they are meant to age.
- Costs and weights are plausible relative to material, size, and status.
- Skinnable is normally `true`; `hideFromPlayers` is normally `false`.

## Remaining implementation questions

These questions do not block the full 400-row item creation pass, but they should be resolved while authoring the final catalogue:

1. Which signet rings should be mechanically functional versus decorative only?
2. Should high noble regalia use ordinary public market tags, `Market / Jewellery / Regalia`, or a later rare/admin treasure policy?
3. What default morph timers should be used for fresh flowers in the target MUD's time scale?
4. Should the devotional branch later reuse `Wear_Badge`, `Wear_Brooch`, `Wear_Neck_Garland`, or `Wear_Ring` for pilgrim badges, prayer garlands, reliquary pendants, and sacred rings, or should devotional wearables receive a separate reference?
5. Should any region-specific jewellery sets be implemented as grouped craft outputs or left as individual item rows plus builder manifests?

## First-pass conclusion

The medieval jewellery branch should make the world feel socially inhabited rather than merely wealthy. The 400-row target should include rough copper rings beside gold gem rings, shell bracelets beside court pearls, fresh garlands beside formal circlets, merchant signets beside child bead strings, and everyday brooches beside court badges. The dependency pass has removed the earlier component/tag/material blockers, so the implementation should now use precise jewellery components, dedicated jewellery tags, dedicated jewellery market tags, exact solid materials, and honest mechanics rather than substitutions.
