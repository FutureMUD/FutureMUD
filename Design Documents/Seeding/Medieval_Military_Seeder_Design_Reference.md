# FutureMUD Medieval Military Goods Seeder Design Reference

Post-implementation edition of `Medieval Military Seeder Design Reference.md` after project-owner decisions, review notes, and all first-wave catalogue passes.

This document consolidates the medieval weapons, ammunition, armour, shields, horse armour, barding, and directly related military gear guidance for the FutureMUD Item Seeder. It covers selected European, Mediterranean, Near Eastern, North African, Indian, Central Asian, and East Asian military material culture families during the period roughly 500AD to 1300AD, presenting shared rules, historical assumptions, component/material/tag grounding, loadout-manifest policy, and the implemented first-wave item catalogue in one stable reference.

The item catalogue sections below are now filled out with the completed first-wave catalogue. The live implementation is split between `DatabaseSeeder/Seeders/ItemSeeder.MedievalWeapons.cs` and `DatabaseSeeder/Seeders/ItemSeeder.MedievalArmour.cs`.

Dependency completion update: the existing 381 prototype identities are unchanged, but documented conservative combat fallbacks now use the newly seeded lance, poleblade, hooked-polearm, composite-bow, sabre, padded-armour, rigid-metal, coat-of-plates, and splinted-armour profiles. Mounted/couched charge and hook/pull/trip/anti-rider mechanics remain engine work in the [consolidated dependency ledger](./FutureMUD_Item_Content_Engine_Dependency_Ledger.md).

---

## Executive summary

- This seeder branch covers **finished medieval military goods**: melee weapons, ranged weapons, ammunition, shields, armour, sheaths, scabbards, quivers, racks, stands, belts or harness items, horse tack used for military riding, horse armour, barding, caparisons, and selected military-support gear.
- The completed first-wave catalogue contains **381 unique item prototypes**: 77 melee weapons, 105 armour/tack/barding items, 65 shields, 65 ranged/ammunition/thrown items, and 69 carrying/storage/support items.
- The chronological boundary is **strictly the culture-period slice already used by the medieval seeder: approximately 500AD to 1300AD, with each inspiration family anchored to its own reference period**. Explicit fantasy items and post-period late-medieval/Renaissance equipment are deferred to a separate fantasy or later-period seeder.
- The branch uses a **shared-first catalogue model**. Most weapon, armour, ammunition, horse-tack, and support prototypes should live in shared historical buckets. Culture-specific output should be concentrated where form, use, or presentation genuinely differs: shields, helmets, lamellar families, mounted-archer equipment, horse armour, Japanese armour, Indian shields, western knightly shields, Islamic round/adarga-like shields, East Asian ranged/armour silhouettes, and strongly regional barding or caparison forms.
- The target balance should be approximately **70-80% shared/common prototypes** and **20-30% culture, region, or tactical specialty prototypes**. Skins should handle heraldry, household marks, clan marks, devotional marks, unit colours, exact textile facings, elite decoration, inscriptions, setting-specific naming, and most culture-facing decorative variation.
- Armour coverage must be **layered and bodypart-complete**, not just torso pieces. The design must include underlayers, head and neck defences, torso protection, shoulder/upper-arm/elbow/forearm pieces, hand defences, hip and skirt defences, thigh/knee/shin/foot defences, commoner protection, militia protection, professional-soldier protection, elite protection, mounted-combat protection, and period-appropriate horse protection.
- Weapon coverage must include **every available medieval-useful weapon family** in the seeded components: knives, daggers, swords, axes, clubs, maces, hammers, picks, flails, spears, polearms, staffs, bows, crossbows, slings, thrown weapons, and training weapons. Each major family should have at least a common, worn/substandard, and good/elite implementation where the distinction is meaningful.
- Ammunition coverage now includes **bodkin arrows and bodkin bolts** as confirmed project components, in addition to ordinary broadhead, field-point, target, padded, sling, and lead sling ammunition. Ammunition should also use a stackable component, normally `Stack_Number`, so arrows, bolts, bullets, and stones can be handled in grouped quantities.
- Shields should be treated as the **most culturally expressive combat items**. Base prototypes may share mechanics, but shield silhouettes, materials, grip styles, bosses, painted surfaces, heraldry-ready fields, and regional forms should receive more distinct entries than ordinary swords or spears.
- Modern, early-modern, gunpowder, overtly magical, and explicitly fantasy military items are not part of this branch. The seeded component inventory includes firearms, cartridges, concussive ammunition, modern armour, and other later systems; those belong to later or fantasy-specific seeder branches unless a separate expansion is approved.

---

## Scope and era model

### Chronological band

The design band is **500AD to 1300AD**, interpreted through the already selected culture families and their reference anchors. This is not a single universal equipment year. A c. 800 Anglo-Saxon item, a c. 1066 Norman item, a c. 1200 Seljuk item, a c. 1200 Kamakura Japanese item, and a c. 1250 High English item may all be valid, but each must remain credible for its own builder-facing context.

Do not include explicitly fantasy items in this branch. Do not normalize equipment that is mainly post-1300 for the relevant culture. When a seeded component has a later-sounding name, it may be used only if the authored item can still be described as a conservative 500-1300 object. Otherwise, reserve it for the later fantasy or late-medieval seeder.

### Geographic coverage

This document covers the same broad world slice as the medieval clothing reference, adapted for military goods:

- Britain and Ireland
- Scandinavia and the North Sea
- Western, central, and southern Europe
- Iberia
- Byzantium
- The Levant
- Egypt and North Africa
- The Eurasian steppe
- Rus / Novgorod
- Northern and southern India
- China
- Korea
- Japan

### Historical inspiration families

The following labels are builder-facing organizational buckets, not required public item wording:

- Early Anglo-Saxon
- Anglo-Danish
- Norse / Viking Age
- Norman
- Anglo-Norman
- High English / British
- Irish / Gaelic
- Scottish / Gaelic-Lowland
- Carolingian / Frankish
- Capetian French
- Holy Roman Empire / German
- Christian Iberian
- Andalusian
- Byzantine
- Abbasid
- Fatimid
- Seljuk / Ayyubid-Mamluk
- Magyar
- Rus / Novgorod
- Steppe Turkic / Mongol
- North Indian / Rajput
- South Indian / Chola
- Song China
- Goryeo Korea
- Heian / Kamakura Japan

### Resolution

This branch creates **standard item prototypes**, not exhaustive museum-taxonomy subtypes. A base sword, axe, spear, helmet, mail shirt, lamellar vest, round shield, kite shield, saddle, caparison, or chanfron must be credible when unskinned. Skins and later local overrides should carry:

- local names
- exact heraldry
- clan or household marks
- devotional or dynastic symbols
- regiment or retinue colours
- decorative rivet layouts
- inscriptions
- enamel or inlay details
- lacquer colours
- braid, textile, or leather facing
- elite metalwork
- setting-specific motifs

### Exclusions

The strict branch excludes:

- full fifteenth-century plate harness as the default elite armour model
- mature sallets, armets, close helmets, burgonets, maximilian fluting, and other later fitted-plate forms
- mature brigandines as a default type, except where a conservative coat-of-plates descendant is clearly credible before 1300
- Renaissance rotellas and late fencing weapons as defaults
- firearms, powder flasks, blackpowder cartridges, musket balls, gunsmithing tools, and pistols/muskets
- modern ballistic armour, stab vests, riot shields, firearms, and modern cartridges
- explosives, magical ammunition, concussive ammunition, or alchemical military gear unless a separate fantasy or special-technology branch is approved
- craft tools in the first military-goods wave

### Strict-period boundary rule

Items are admitted by **period plausibility inside the selected culture slice**, not by whether a seeded component exists. The component list is the implementation vocabulary; it is not a historical permission list.

Use period-safe alternatives whenever a later object name would distort the branch. For example, a simple late-thirteenth-century plate-reinforced body defence may be acceptable as a coat of plates or pair of plates, but a full plate harness is not. A mail coif, nasal helm, kettle helm, great helm, conical helm, or spangenhelm is preferred over an armet or sallet. Boots or mail chausses are preferred over sabatons. Spaulders, mail sleeves, lamellar shoulder pieces, or leather guards are preferred over mature pauldrons.

Where the user has added useful components such as `Wear_Tassets`, use them only for period-plausible hip or skirt defences, not for mature fifteenth-century plate tassets. Where the object is not credible before 1300 for the relevant culture, defer it.

---

## Shared-first branch architecture

### Default rule

Create shared items wherever the object can plausibly serve multiple cultures with only presentation changes. Most spears, knives, axes, maces, shortbows, simple bows, crossbows, mail shirts, padded jacks, leather guards, scale vests, lamellar cuirasses, quivers, sheaths, racks, stands, and basic helmets should begin as shared prototypes.

### When to create a culture-specific or regional item

Create a separate culture-specific item when one or more of the following is true:

- The silhouette is mechanically or visually distinct enough that a skin would be misleading.
- The seeded component differs, such as `Shield_Kite` versus `Shield_Dhal` versus `Shield_Adarga`.
- The wear component differs, such as `Wear_Nasal_Helm` versus `Wear_Spangenhelm` versus `Wear_Greathelm`.
- The primary material materially changes behaviour, such as wood versus hide versus metal.
- The item represents a tactical system rather than decoration, such as steppe horse-archer equipment, Japanese lamellar harness, western mail-and-kite-shield knightly equipment, or Indian dhal/sword pairings.
- The item is most useful to builders as a distinctive regional anchor.

### What skins should carry instead

Do not create new prototypes merely for:

- colour
- paint
- heraldic device
- clan mark
- household badge
- banner motif
- prayer text
- gold trim
- richer polish
- different scabbard dye
- exact regional name
- setting-language name
- known owner mark
- unit number or militia sign

Those belong in skins unless they change behaviour, material, coverage, or major silhouette.

### Implemented catalogue balance

The completed first-wave catalogue follows the shared-first model while giving extra prototype attention to armour, shields, ranged systems, barding, and support gear where components or silhouettes justify it. The split is intentionally catalogue-level rather than culture-bucket-level: most entries remain shared prototypes, while skins carry heraldry, household marks, exact local decoration, and many regional names.

| Category | Implemented count | Balance note |
|---|---:|---|
| Melee weapons | 77 | Broad shared coverage across every available medieval-useful melee family, with quality variation for common, worn, professional, and elite examples. |
| Armour, horse tack, and barding | 105 | Bodypart-complete human armour plus period-appropriate horse tack, horse armour, barding, and over-armour display pieces. |
| Shields | 65 | Stronger regional and cultural-silhouette treatment than ordinary weapons, while still using shared shield mechanics where appropriate. |
| Ranged weapons, ammunition, and thrown weapons | 65 | Bows, crossbows, slings, staff slings, thrown weapons, and stackable ammunition using exact seeded ammunition components. |
| Carrying, storage, and support gear | 69 | Sheaths, scabbards, quivers, bolt cases, belts, racks, armour stands, bowstrings, and military cases/covers. |
| **Total** | **381** | Completed first-wave military-goods catalogue. |

---

## Culture coverage table

The culture table is builder-facing. Public item text should describe the object rather than naming a real-world culture unless the item form itself is already an accepted object name.

| Inspiration family | Reference anchor | Coverage boundary | Military-design focus |
|---|---:|---|---|
| Early Anglo-Saxon | c. 800AD | Lowland England before sustained Anglo-Danish synthesis | Spears, seaxes, short swords, axes, round wooden shields with bosses, spangenhelm or nasal-helm variants, padded clothing, and rare elite mail. |
| Anglo-Danish | c. 950AD | Late Anglo-Saxon England under strong Scandinavian influence | North Sea mix of spears, axes, seaxes, swords, round shields, simple bows, mail byrnies, nasal helmets, and elite helmet/mail sets. |
| Norse / Viking Age | c. 950AD | Scandinavia and diaspora before the close of the Viking Age | Axes, spears, swords, bows, round shields, mail byrnies, nasal/spangenhelm helmets, rare elite mail, and high-status sword variants. |
| Norman | c. 1066AD | Ducal Normandy and conquest-generation Norman sphere | Mounted lance, arming sword, kite shield, mail hauberk, mail coif, nasal helm, early chausses, and cavalry-focused loadouts. |
| Anglo-Norman | c. 1150AD | Post-Conquest England and Norman-influenced Britain | Mail hauberk and coif, kite-to-heater shield transition, lances, swords, maces, crossbows, infantry spears, and early surcoat-over-mail presentation. |
| High English / British | c. 1250AD | High-medieval England and Welsh March/British court-facing styles | Mail-and-coat-of-plates, heater shields, heraldic-ready shields, kettle hats, great helms, crossbows, self bows, spears, maces, and professional retinue equipment. |
| Irish / Gaelic | c. 1100AD | Gaelic Ireland before strong late-medieval English military influence | Spears, javelins, axes, knives, light shields, hide or wooden protection, little heavy armour by default, and strong light-infantry/mounted-raider options. |
| Scottish / Gaelic-Lowland | c. 1250AD | Medieval Scotland across Gaelic and Lowland spheres | Spears, axes, bows, simple shields, mail for elites, leather/textile defences for commoners, and Lowland knightly overlaps with British/French models. |
| Carolingian / Frankish | c. 800AD | Frankish court and early medieval western Europe | Spears, swords, oval/round shields, mail for elites, scale or lamellar-adjacent options where plausible, helmets, cavalry lances, and broad military baselines. |
| Capetian French | c. 1200AD | Northern and central French high-medieval sphere | Rich knightly mail, surcoat-over-mail, kite/heater shields, lances, swords, maces, crossbows, early coats of plates, and elite heraldic display. |
| Holy Roman Empire / German | c. 1200AD | German-speaking imperial and central European regions | Mail, kettle hats, great helms, early plate additions, crossbows, swords, axes, maces, spears, and strong town-militia/retinue variants. |
| Christian Iberian | c. 1200AD | Leonese, Castilian, Aragonese, Navarrese, and neighbouring Christian Iberian contexts | Lances, swords, javelins, spears, mail, leather shields, adarga-contact variants, cross-cultural helmet/shield forms, and cavalry plus frontier fighting. |
| Andalusian | c. 1100AD | al-Andalus and western Islamic Iberia | Lighter armour, mail where elite, leather/textile defences, round or adarga-like shields, javelins, swords, spears, bows, and mounted skirmishing equipment. |
| Byzantine | c. 1000AD | Middle Byzantine Constantinopolitan and provincial military context | Lamellar, scale, mail, conical helmets, mail coifs, kite or round shields, spears, lances, swords, maces, bows, and professional infantry/cavalry loadouts. |
| Abbasid | c. 850AD | Baghdad-centred early medieval Islamic military sphere | Mail, lamellar, textile/leather armour, round shields, spears, bows, maces, swords, javelins, and elite decorated arms mostly handled by skins. |
| Fatimid | c. 1050AD | Fatimid Egypt and North African contexts | Lighter warm-climate armour, textile padding, mail for elites, round shields, spears, javelins, bows, swords, maces, and mounted/urban military goods. |
| Seljuk / Ayyubid-Mamluk | c. 1200AD | Anatolia, Syria, Egypt, and crusading-era eastern Mediterranean | Mail, lamellar, conical helmets, round shields, lances, maces, sabres/curved sword skins over sword components, composite-bow skins, and mounted-archer equipment. |
| Magyar | c. 950AD | Carpathian Basin Magyar and steppe-to-central-Europe context | Mounted archery, lances, sabre-like swords via skins, lamellar/leather armour, light shields, belts, quivers, and mobile cavalry equipment. |
| Rus / Novgorod | c. 1100AD | Kievan Rus and Novgorod-facing northern Slavic/Norse/Byzantine intersections | Axes, spears, swords, bows, mail, lamellar, conical helmets, nasal helmets, round/kite shields, and fur-lined or cold-weather military overlays where appropriate. |
| Steppe Turkic / Mongol | c. 1200AD | Inner Eurasian mounted steppe before and during early Mongol expansion | Composite-bow skins, lances, sabres via sword components, lamellar, leather armour, felt/leather boots, light shields, quivers, bow cases, and mounted archery. |
| North Indian / Rajput | c. 1100AD | North and north-western Indian contexts before heavy late Sultanate/Mughal tailoring | Swords, spears, lances, bows, dhal shields, mail/scale/textile armour, helmets, elite weapon decoration through skins, and hot-climate equipment assumptions. |
| South Indian / Chola | c. 1050AD | Chola and neighbouring south Indian contexts | Spears, swords, bows, shields, lighter textile/leather protection, scale or mail for elites where used, and warm-climate troop equipment. |
| Song China | c. 1100AD | Northern and Southern Song military baseline | Crossbows, bows, spears, polearms, swords, shields, lamellar/laminar armour, helmets, textile padding, and strong infantry/ranged focus. |
| Goryeo Korea | c. 1150AD | Goryeo-period Korean military baseline | Bows, spears, swords, shields, lamellar or scale armour, helmets, textile padding, and East Asian court/military intersections. |
| Heian / Kamakura Japan | c. 1200AD | Late Heian to early Kamakura warrior culture | Lamellar armour, large shoulder guards, helmets, bows, tachi-like sword skins, naginata/spear/polearm skins, arrows, quivers, surcoat/over-armour display, and limited handheld-shield use. |

---

## Seeder and project grounding rules

- Use the project-standard `CreateItem(...)` seeder call shape during implementation.
- `uniqueReference` values use lowercase snake case ASCII and should remain stable once accepted.
- Recommended unique-reference prefix for this branch: `medieval_military_`.
- `noun` is compact and singular, usually the weapon, armour, shield, ammunition, sheath, quiver, or gear type.
- `sdesc` is player-facing, concise, and normally begins with `a`, `an`, or `a pair of`. It should not end with a full stop.
- `ldesc` is `null` for ordinary portable goods, allowing the normal room display to apply.
- `fdesc` is player-facing in-world prose. It should describe visible construction, shape, material, finish, wear, stitching, rivets, rings, plates, grip, straps, bosses, edges, shafts, sockets, fletching, padding, buckles, and how a wearable sits on the body.
- `material` must be an exact seeded solid material. Liquids and gases are not substitutes for the solid primary material argument.
- `tags` must be exact seeded hierarchical tag paths.
- `components` must be exact seeded component prototype names.
- `inherentCost` is denominated in farthings. Use whole-farthing values unless a fractional farthing is deliberately intended.
- Finished military goods should normally be skinnable and player-visible.
- Ordinary portable goods include `Holdable` unless a component or fixture use-case specifically makes them non-portable.
- No ordinary military good in this branch should use a morph target, morph emote, morph timer, long description, hidden-player flag, or non-skinnable flag unless a later implementation explicitly marks a special exception.

---

## Military implementation rules

### Weapons

Every ordinary portable weapon should normally include:

- `Holdable`
- exactly one primary weapon component, such as a `Melee_*`, `Shortbow`, `Longbow`, `Crossbow`, `Sling`, `Staff Sling`, or `Throwing_*` component
- `Destroyable_Weapon`
- `Beltable` where the item is plausibly attachable to a belt, especially knives, daggers, small axes, and some short swords

Do not stack multiple components that provide the same exclusive weapon interface unless the component data explicitly supports that combination.

### Ammunition

Ammunition should normally include:

- `Holdable`
- one stackable component, normally `Stack_Number`
- one ammunition component such as `Ammo_BroadheadArrow`, `Ammo_BodkinArrow`, `Ammo_FieldPointBolt`, `Ammo_BodkinBolt`, or `Ammo_LeadSlingBullet`
- a suitable destroyable component, normally `Destroyable_Weapon` or `Destroyable_Misc` depending on the implementation convention chosen for expendable projectiles

Use `Stack_Number` as the default for arrows, bolts, sling bullets, lead sling bullets, and other discrete ammunition because it keeps the visible quantity explicit. Use `Stack_Pile` only if a later catalogue entry intentionally represents an imprecise loose pile, such as gathered sling stones. Do not combine `Stack_Number` and `Stack_Pile` on the same ammunition item unless the engine data later establishes a supported reason to do so.

The currently seeded strict-branch ammunition components include target, field point, bodkin, broadhead, padded, sling bullet, and lead sling bullet types. Blowgun and concussive ammunition types are present in the broader seed set but are deferred out of the strict 500-1300 branch.

### Armour

Every human armour item should normally include:

- `Holdable`
- exactly one wearable coverage component
- exactly one armour component
- `Destroyable_Armour`
- a variable colour component when the visible textile, dyed leather, painted, or lacquered element should vary
- an insulation component only when the item materially affects insulation or heat behaviour

Armour descriptions must not claim precise protection values. The armour component supplies behaviour; the description supplies visible material and construction.

### Shields

Every shield should normally include:

- `Holdable`
- exactly one `Shield_*` component
- `Destroyable_Shield`
- a military shield tag

Do not add `Melee_Shield` to an item that already has a `Shield_*` component unless the engine data explicitly permits that combination. The shield component type already acts as shield and melee-weapon behaviour.

### Sheaths, scabbards, quivers, and racks

Military carrying and storage gear should use explicit support components where available. These items are in scope because they make weapons and armour usable, storable, or displayable; they are not catalogue filler.

**Sheaths and scabbards** should normally include:

- `Holdable`
- exactly one sheath component: `Sheath_Small`, `Sheath`, or `Sheath_Large`
- `Beltable` if the sheath or scabbard is meant to attach to a belt, baldric, hanger, or harness
- a suitable destroyable component, normally `Destroyable_Weapon` for rigid weapon furniture or `Destroyable_Clothing` for soft leather/textile cases
- a variable colour component only when dyed leather, textile facing, painted decoration, or lacquered finish should vary

Use `Sheath_Small` for knife and dagger sheaths, `Sheath` for normal sword-sized scabbards, and `Sheath_Large` for large swords, large axes, or other large melee weapons where a sheathing profile is plausible. Do not create holsters in this branch; `Holster_Small` and `Holster_Large` are firearm support components and belong to later/modern branches.

**Quivers and bolt cases** should normally include:

- `Holdable`
- `Container_Quiver`
- one wearable component if the quiver is worn, such as `Wear_Shoulder`, `Wear_Backpack`, or `Wear_Waist` where the carry style fits the exact item
- `Beltable` only when the quiver or bolt case should attach to a belt rather than being worn through its own wearable component
- `Destroyable_Clothing` for leather, textile, or hide quivers, or `Destroyable_Misc` for miscellaneous rigid cases
- a variable colour component for dyed leather, painted/lacquered cases, textile facings, or elite decorated quivers

Do not promise automatic ammunition sorting, concealed storage, waterproofing, or locking unless a future component explicitly supports that behaviour.

**Weapon racks** should normally include:

- `Container_Weapon_Rack`
- `Destroyable_WoodenHeavy` for ordinary wooden racks, `Destroyable_HeavyMetal` for metal racks, or `Destroyable_Furniture` if the implementation treats the rack as furniture
- no `Holdable` when the rack is an installed fixture, room feature, or barracks/armoury furnishing
- a non-null `ldesc` when the rack should present as installed room contents rather than as a loose inventory object

Portable weapon racks are a narrow exception: include `Holdable` only if the item is genuinely intended to be picked up and carried as inventory.

**Armour stands** should normally include:

- `Container_Armor_Stand`
- `Destroyable_WoodenHeavy` for ordinary wooden stands, `Destroyable_HeavyMetal` for metal stands, or `Destroyable_Furniture` if the implementation treats the stand as furniture
- no `Holdable` when the stand is an installed fixture, armoury furnishing, or display piece
- a non-null `ldesc` when the stand should appear as installed room contents

Use `Container_Armor_Stand` only for armour stands, armour trees, and display supports in this branch. Do not author training dummies, pell posts, quintains, archery targets, or humanoid practice targets in the military-goods catalogue.

**Military belts, sword belts, and hangers** should normally include:

- `Holdable`
- one wearable component, normally `Wear_Waist`, `Wear_Shoulder`, or `Wear_Sash` depending on the worn position
- one belt component only when attachment behaviour matters: `Belt_2`, `Belt_4`, `Belt_6`, or `Belt_Large`
- `Destroyable_Clothing` for leather, textile, or mixed strap goods
- a variable colour component only when dyed leather or textile should vary

Plain straps, baldric-like descriptive bands, cords, or sashes should not claim attachment behaviour unless the relevant `Belt_*` or `Beltable` component relationship exists.

### Horse tack, horse armour, and barding

Military horse gear belongs in this branch when it is battlefield riding gear, warhorse equipment, or protective horse equipment. Ordinary civilian saddlery can be reused later by a riding/animal-equipment branch, but the military branch should include enough tack and barding for mounted warriors and armoured mounts.

Horse tack and barding should normally include:

- `Holdable` for ordinary portable tack pieces
- exactly one relevant horse wear component, such as `Wear_Saddle`, `Wear_Bridle`, `Wear_Bit`, `Wear_Caparison`, `Wear_Chanfron`, `Wear_Criniere`, `Wear_Croupiere`, `Wear_Flanchards`, or `Wear_Peytral`
- `Destroyable_Armour` for protective horse armour pieces
- `Destroyable_Clothing` or another approved textile/leather destroyable component for non-armoured caparisons and textile horse coverings
- an armour component only when the item is meant to provide armour behaviour
- a variable colour component for heraldic, livery, lacquered, textile, or dyed-leather display pieces where appropriate

Do not imply the presence of saddlebags, hidden storage, reins-as-control mechanics, or mount-control behaviour unless those mechanics are supported by components.

---

## Armour design policy

### Layering principle

Armour should be authored as a system of layers rather than one all-purpose item. A warrior might wear a padded layer, mail layer, helmet, shield, and selected limb defences. A common levy might wear only a padded jack and cap. A mounted elite might combine mail, coat of plates, helmet, gloves or mittens, chausses, and a shield. A warhorse might have a saddle, bridle, bit, caparison, and selected horse armour such as chanfron, peytral, flanchards, criniere, or croupiere where the culture and date support it.

### Social range

The armour catalogue should explicitly support:

- peasant levy and household defence
- town militia
- shipboard and raiding fighters
- light infantry
- archers and crossbowmen
- professional retainers
- mounted men-at-arms
- elite knights or nobles
- steppe horse archers
- Islamic and Byzantine professional soldiers
- East Asian armoured retainers and warriors
- Indian elite and common military roles
- military riding horses and protected warhorses

### Bodypart coverage matrix

| Body area | Shared coverage targets | Exact seeded wear components to prefer | Notes |
|---|---|---|---|
| Padded underlayers | arming cap, padded coif, gambeson, aketon, padded jack, padded riding coat, padded chausses | `Wear_Coif`, `Wear_Tunic`, `Wear_Long-Sleeved_Tunic`, `Wear_Doublet`, `Wear_Chausses`, `Wear_Trousers` | Usually `Armour_Padded` or `Armour_Padded`; use textile variables where dyed. |
| Mail torso | byrnie, haubergeon, hauberk, long mail shirt | `Wear_Haubergeon`, `Wear_Hauberk` | Use `Armour_Chainmail`; primary material usually `mild steel`, `carbon steel`, or `wrought iron` depending project convention. |
| Lamellar/scale/early plate torso | lamellar cuirass, scale corselet, coat of plates, pair of plates, plate-reinforced cuirass | `Wear_Cuirass`, `Wear_Breastplate`, `Wear_Backplate`, `Wear_Faulded_Cuirass`, `Wear_Culeted_Cuirass`, `Wear_Vest`, `Wear_Jerkin` | Use `Armour_Lamellar`, `Armour_MetalScale`, `Armour_LeatherScale`, `Armour_Laminar`, `Armour_BoiledLeather`, or cautious `Armour_RigidMetal` only for period-plausible transitional plate pieces. |
| Leather/textile torso | leather jerkin, boiled-leather cuirass, rivet-reinforced leather, padded vest | `Wear_Jerkin`, `Wear_Vest`, `Wear_Tunic`, `Wear_Cuirass` | Commoner and militia coverage should be substantial here. Studded leather is a game abstraction and should be described as riveted or reinforced construction, not fantasy-stud clichés. |
| Head | padded cap, mail coif, spangenhelm, nasal helm, conical helm, kettle hat, great helm, half helm | `Wear_Coif`, `Wear_Helmet`, `Wear_Half_Helmet`, `Wear_Spangenhelm`, `Wear_Nasal_Helm`, `Wear_Nasal_Spangenhelm`, `Wear_Aventail_Spangenhelm`, `Wear_Greathelm` | Defer `Wear_Armet`, `Wear_Barbute_Helmet`, `Wear_Open_Armet`, `Wear_Open_Sallet_Helmet`, and `Wear_Sallet_Helmet` to later/fantasy branches. |
| Neck and face | mail coif drape, aventail, pixane, early gorget-like collar | `Wear_Coif`, `Wear_Pixane`, `Wear_Gorget` | Mature bevors are outside the strict branch. Do not use `Wear_Bevor` here unless a later branch is being written. |
| Shoulders | mail shoulder coverage, leather shoulder guards, spaulders, lamellar shoulder guards, early splints, decorative ailettes | `Wear_Spaulders` | Use `Wear_Pauldrons` only in a later/fantasy branch. Period-safe shoulder coverage should come from mail sleeves, lamellar shoulders, leather guards, spaulders, or textile padding. |
| Upper arms | mail sleeves, leather bracers, brassarts, splinted upper-arm guards | `Wear_Brassarts`, `Wear_Bracers`, `Wear_Bracer` | Avoid over-representing fitted plate. Conservative splints and leather guards are safer before 1300. |
| Elbows | padded elbow caps, splinted elbows, early couter-like pieces | `Wear_Couters` | Use conservative descriptions; avoid mature articulated plate assumptions. |
| Forearms | leather bracers, vambraces, splinted vambraces, mail sleeves | `Wear_Bracers`, `Wear_Bracer`, `Wear_Vambraces` | `Wear_Vambraces` can cover splinted or plate forearm defences if the description remains period-safe. |
| Hands | leather gloves, mittens, mail mufflers, simple early gauntlet-like gloves | `Wear_Gloves`, `Wear_Mittens`, `Wear_Gauntlets` | Mature articulated plate gauntlets are later. Mail mufflers, leather gloves, and simple guarded gloves are safer in strict coverage. |
| Hips and skirt | mail skirt, coat-of-plates skirt, fauld-like lower plates, laced hip defences, tasset-like flaps where period-safe | `Wear_Faulded_Cuirass`, `Wear_Culeted_Cuirass`, `Wear_Tassets` | `Wear_Tassets` is available. Use it for conservative hip/skirt defences only; do not create mature later plate tassets in this branch. |
| Thighs | mail chausses, padded chausses, splinted thigh guards, cautious cuisses | `Wear_Chausses`, `Wear_Cuisses` | Mail chausses are central to elite western coverage; cuisses should be conservative if used before 1300. |
| Knees | padded knee guards, early poleyn-like caps | `Wear_Poleyns` | Use with caution and period-safe descriptions; avoid later fully articulated knee assemblies. |
| Shins | greaves, splinted greaves, leather greaves | `Wear_Greaves` | Good for leg coverage across multiple cultures if descriptions avoid overly late full-plate assumptions. |
| Feet | high boots, reinforced shoes, mail chausses with foot coverage | `Wear_High_Boots`, `Wear_Chausses` | Defer `Wear_Sabatons` to later/fantasy branches. Strict core should use boots, shoes, reinforced footwear, or mail chausses. |
| Over-armour display | surcoat, tabard, shield cover, armour coat, livery layer | `Wear_Tabard`, `Wear_Tunic` | These are primarily textile display/protection layers, usually with clothing armour or no armour component depending implementation. |
| Horse head | chanfron, textile face cover, leather face defence | `Wear_Chanfron` | Use `Armour_BoiledLeather`, `Armour_Lamellar`, `Armour_MetalScale`, or cautious plate only when protective. Textile or leather face covers may omit armour behaviour. |
| Horse neck | criniere, mail/scale/lamellar neck defence, textile neck cover | `Wear_Criniere` | Strongest in elite cavalry and cultures with horse-armour traditions. |
| Horse chest | peytral, breast defence, decorated front barding | `Wear_Peytral` | Can be leather, textile, mail, lamellar, or metal depending culture/date. |
| Horse flanks | flanchards, side plates or textile side barding | `Wear_Flanchards` | Use cautiously; avoid late full-plate barding outside period. |
| Horse hindquarters | croupiere, rump defence, rear caparison section | `Wear_Croupiere` | Useful for elite barding and display; protective version should have armour behaviour. |
| Horse cover/display | caparison, horse cloth, heraldic or livery covering | `Wear_Caparison` | Usually textile/leather display; only attach an armour component when actually protective. |
| Military tack | war saddle, bridle, bit | `Wear_Saddle`, `Wear_Bridle`, `Wear_Bit` | Include for mounted loadouts. These are not automatically armour unless separately authored as protective tack. |

### Armour technology coverage implemented

The implemented catalogue was built to cover the following armour technologies where period, component, and material support allow:

- padded cloth armour: gambeson, aketon, padded jack, padded cap, arming doublet or riding arming layer where period-safe
- heavy textile armour: quilted coat, stuffed jack, felt or layered-cloth armour where culturally plausible
- leather armour: simple leather jerkin, boiled leather cuirass, leather bracers, leather greaves, leather horse face/chest defences
- rivet-reinforced leather as a game-mechanical abstraction, used sparingly and described as reinforced construction rather than later fantasy-studded clichés
- mail armour: mail coif, mail shirt, mail haubergeon, mail hauberk, mail chausses, mail mittens/mufflers, mail aventail, and mail horse defences where appropriate
- scale armour: leather scale, metal scale, scale corselet, scale sleeves or skirts, and scale horse defences where useful
- lamellar armour: lamellar cuirass, lamellar skirt, lamellar shoulder guards, East Asian and steppe variations, and horse lamellar where appropriate
- laminar armour: banded/laminar cuirass and limb defences where appropriate
- early plate additions: coat of plates, pair of plates, plate-reinforced cuirass, conservative hip/skirt defences, and simple transitional pieces that are credible before 1300
- helmet families: padded cap, mail coif, spangenhelm, nasal helm, conical helm, kettle hat, great helm, half helm, aventail spangenhelm, culture-specific helmet skins
- limb armour: bracers, vambraces, couters, brassarts, gloves, mittens, simple gauntlet-like gloves, cuisses, poleyns, greaves, boots, and mail chausses
- horse equipment: saddle, bridle, bit, caparison, chanfron, criniere, peytral, flanchards, croupiere, and culturally appropriate barding systems

### Historical strictness guidance for selected requested items

The following items should be treated carefully inside the strict 500-1300 branch:

- **Bevor**: mature bevors are outside this branch. Defer to a later/fantasy seeder.
- **Pauldrons**: mature pauldrons are outside this branch. Use spaulders, mail sleeves, leather shoulder guards, lamellar shoulder pieces, or textile padding.
- **Sabatons**: mature sabatons are outside this branch. Use mail chausses, boots, reinforced shoes, or culturally appropriate footwear.
- **Tassets**: `Wear_Tassets` is available. Use it only for conservative period-plausible hip or skirt defences, not mature fifteenth-century plate tassets.
- **Full plate harness**: outside this branch. `Armour_RigidMetal` exists mechanically, but should not be used to create full plate suits in the strict 500-1300 catalogue.
- **Armet, sallet, barbute, close helm**: outside this branch by default. Use nasal helms, spangenhelms, conical helms, kettle hats, great helms, half helms, and coifs instead.
- **Horse plate barding**: avoid later full-plate barding. Use textile caparisons, leather barding, mail/scale/lamellar horse protection, and conservative elite pieces where culturally supported.

---

## Weapon design policy

### General weapon rules

- Every weapon family should have at least one common/shared item.
- Major battlefield weapon families should have at least three quality bands: worn/substandard, standard, and good/elite.
- Avoid making every elite weapon a different prototype. Use quality and skins for many elite distinctions.
- Use `Substandard` for old, hand-me-down, militia, damaged-looking, or poorly maintained goods.
- Use `Standard` for ordinary serviceable weapons.
- Use `Good` for well-forged, well-balanced, professional, noble, or elite weapons.
- Use `VeryGood` or `Great` sparingly for exceptional, imported, noble, ceremonial, named-household, or rare elite goods.
- Keep all public object forms credible for the relevant culture anchor. Do not use late-medieval or Renaissance weapon names merely because a component exists.

### Weapon family coverage matrix

| Family | Seeded components | Catalogue coverage target | Notes |
|---|---|---|---|
| Knives and small blades | `Melee_Knife`, `Melee_Dagger`, `Throwing_Knife`, training variants | knife, dagger, seax-like knife, utility fighting knife, throwing knife if setting supports it | Good family for commoner-to-elite quality variation. Avoid later dagger typologies as public defaults unless period-safe. |
| Short swords and sidearms | `Melee_Shortsword`, `Melee_Longsword`, `Melee_Dagger` | short sword, arming sword, worn sword, good sword, elite sword | Use `Melee_Longsword` as the engine component for many one-handed medieval swords if no separate arming-sword component exists. |
| Larger swords | `Melee_Longsword`, `Melee_Two Handed Sword`, `Melee_Estoc` | larger sword forms only when credible before 1300; otherwise defer | Avoid rapier, mature two-handed swords, and later estoc presentation in the strict branch. |
| Axes | `Melee_Axe`, `Melee_Two Handed Axe`, `Throwing_Axe`, training variants | hand axe, battle axe, broad axe, long axe, two-handed axe where period-safe, throwing axe | Norse, Anglo-Danish, Gaelic, Rus, and commoner/militia branches need strong axe coverage. |
| Clubs and staves | `Melee_Club`, `Melee_Quarterstaff`, training variants | club, cudgel, staff, quarterstaff, heavy staff | Good low-cost commoner and training family. |
| Maces | `Melee_Mace`, training variant | simple mace, flanged mace, elite mace | Strong across western, Byzantine, Islamic, Indian, and steppe elite contexts. |
| Hammers and picks | `Melee_Warhammer`, `Melee_Military Pick`, `Melee_Mattock`, training variants | hammer, military pick, mattock-like peasant weapon | Include conservative forms only; avoid specialized late armour-piercing weapon names when not period-safe. |
| Flails | `Melee_Flail`, training variant | military flail or agricultural flail-derived weapon where setting supports it | Treat conservatively; useful gameplay component but historically variable. |
| Spears and javelins | `Melee_Short Spear`, `Melee_Long Spear`, `Throwing_Spear`, training variants | spear, long spear, boar spear, lance-like spear, javelin | Every culture should have spear access. This is the most universal family. |
| Polearms | `Melee_Halberd`, `Melee_Pike`, `Melee_Long Spear`, `Melee_Quarterstaff`, training variants | pike, bill-like polearm, glaive-like polearm, naginata-like polearm, long spear | The `Melee_Halberd` component can support polearm behaviour, but public naming should avoid forms too late for 500-1300. |
| Bows | `Shortbow`, `Longbow` | shortbow, long self bow, composite-bow presentation, Japanese bow presentation, steppe bow presentation | Mechanics may use `Shortbow` or `Longbow`; skins carry exact bow traditions where no separate component exists. |
| Crossbows | `Crossbow`, `Hand Crossbow` | light crossbow, heavy crossbow, town crossbow, crossbow-and-pavise pairing | Hand crossbow should be rare or omitted from strict core unless a specific setting supports it before 1300. |
| Slings | `Sling`, `Staff Sling` | sling, staff sling | Cheap commoner, shepherd, militia, and skirmisher coverage. |
| Blowguns | `Blowgun` | normally omitted from this 500-1300 covered culture set | Reserve for other culture branches or special/fantasy branches. |
| Shields as weapons | `Shield_*`; avoid stacking `Melee_Shield` without approval | shield bash behaviour via shield component | Shields are their own section. |
| Training weapons | numerous `Melee_Training_*` components | wooden sword, blunted spear, padded mace, training staff, practice axe | Useful for schools, barracks, tournaments, and militia drills. |

### Weapon quality policy

The catalogue should include quality variation where form and game use justify it. Examples:

- **Worn/substandard sword**: old hand-me-down, nicked, plain hilt, imperfect balance, lower cost.
- **Standard sword**: serviceable, clean, practical crossguard and grip.
- **Good sword**: better polish, balanced blade, clean fuller, fine grip, better scabbard pairing if separate.
- **Elite sword**: probably a skin or rare `VeryGood`/`Great` prototype rather than a default mass catalogue item.

For cheap weapons like clubs, staffs, slings, and simple spears, quality variation can be lighter. For swords, helmets, mail, lamellar, crossbows, barding, and shields, quality variation should be more visible.

---

## Ammunition design policy

### Core ammunition types

The seeded components currently support the following strict-branch ammunition candidates:

- broadhead arrows
- bodkin arrows
- field-point arrows
- target arrows
- padded arrows
- broadhead bolts
- bodkin bolts
- field-point bolts
- target bolts
- padded bolts
- sling bullets
- lead sling bullets

### Special or deferred ammunition

The seeded components also include concussive arrows, concussive bolts, blowgun darts, and barbed blowgun darts. Concussive ammunition is deferred to fantasy, alchemical, magical, or special-technology branches. Blowgun ammunition is normally outside the selected 500-1300 culture set and should be reserved for other culture branches or special setting needs.

Do not claim armour-piercing, fire, explosive, poison, or magical behaviour in `fdesc` unless the component actually creates the intended mechanical effect.

### Ammunition material assumptions

Primary material should represent the dominant material of the item:

- arrow/bolt shafts: usually `wood`, `ash`, `birch`, or `bamboo` where appropriate
- heads: visible iron or steel described in fdesc, even if primary material remains wood
- sling bullets: `lead`, `stone`, `clay`, or another exact seeded material if added/confirmed
- padded ammunition: `leather`, `wool`, `linen`, or `hemp` depending construction

---

## Shield design policy

### Why shields get special treatment

Shields are both defensive equipment and public display surfaces. They vary by region, role, body coverage, grip style, material, and visual field. This branch should be more willing to create separate shield prototypes than separate sword prototypes.

### Shield component coverage

| Shield component | Core use in this branch | Culture or tactical focus |
|---|---|---|
| `Shield_Round` | Core broad use | Anglo-Saxon, Norse, Carolingian, Rus, Byzantine, Islamic, generic infantry, and many shared shields. |
| `Shield_Kite` | Core high-medieval western/Byzantine use | Norman, Anglo-Norman, Byzantine, western cavalry, and transitional shield styles. |
| `Shield_Heater` | Core high-medieval western use | High English/British, Capetian, HRE, Christian Iberian, heraldic and knightly equipment near the later end of the strict period. |
| `Shield_Buckler` | Core small shield | Town fighters, sidearm users, archers, travellers, and small-shield users where period-safe. |
| `Shield_Tower` | Specialist | Large infantry shield, standing shield, or tower-shield abstraction where appropriate. |
| `Shield_Pavise` | Specialist | Crossbowmen, siege contexts, urban militia, East Asian standing shields if suitable and period-safe. |
| `Shield_Hide` | Core low-tech/regional | Light infantry, commoner, African/North African, steppe, Indian, or improvised hide-shield contexts. |
| `Shield_Wicker` | Core low-tech/regional | Light shields, warm-climate shields, militia, or poorer contexts if material approximations are acceptable. |
| `Shield_Dhal` | Core regional | Indian shield family. |
| `Shield_Adarga` | Core regional | Iberian and western Islamic shield family. |
| `Shield_Targe` | Use cautiously | Only for period-safe small round shield abstractions or later local overlap. Avoid making mature later Scottish targes normal in the strict 1250 context. |
| `Shield_Improvised` | Core low-end | Doors, boards, makeshift planks, pot lids, or crude militia shield analogues where appropriate. |
| `Shield_Parma`, `Shield_Aspis`, `Shield_Thureos`, `Shield_Scutum` | Antiquity branch by default | Do not use in this medieval branch unless deliberately representing a setting-specific survival. |
| `Shield_Rotella` | Renaissance by default | Omit from strict branch. |
| `Shield_Trench`, `Shield_Riot`, `Shield_Ballistic` | Modern by default | Excluded. |

### Shield presentation rules

- Public base text may describe paint, leather facing, metal boss, rim binding, straps, handles, and dents.
- Public base text should avoid real-world culture labels unless the item name itself is a useful form name, such as `adarga`, `dhal`, or `pavise`.
- Heraldry, clan marks, dynastic emblems, religious signs, unit marks, and exact colours should normally be skins.
- A shield can be shared mechanically while receiving many skins. It should become a separate base item when its component, size, material, or major silhouette differs.
- Shields should normally use `Destroyable_Shield`.

---

## Military support gear policy

The implemented branch includes selected support goods where they are directly military, combat-equipment adjacent, or necessary for mounted military loadouts.

### Include in this branch

- sword scabbards
- knife and dagger sheaths
- axe loops or axe hangers if component-supported
- spear racks and weapon racks
- quivers
- bolt cases or bolt quivers
- bow cases if component-supported or purely holdable
- shield covers if useful
- armour stands
- weapon racks
- bowstrings and cordage if treated as finished goods
- arming belts and sword belts with actual belt components where needed
- baldric-like descriptive straps only if they do not claim unsupported storage
- tabards, surcoats, and over-armour display layers if the branch needs military display clothing
- military saddles
- bridles
- bits
- caparisons
- chanfrons
- crinieres
- peytrals
- flanchards
- croupieres
- complete barding or horse-armour loadout templates assembled from component-backed pieces

### Usually leave for later branches

- training dummies, pell posts, quintains, archery targets, humanoid practice targets, and other drill fixtures
- ordinary civilian tack not meant for battlefield riding
- saddlebags, pack saddles, cart harness, draught harness, and stable equipment unless a military use is explicit
- siege engines
- artillery
- battlefield tents and camp furniture
- cooking gear
- religious, ceremonial, or command regalia not directly wearable/weapon-related
- firearms and powder equipment
- magical, alchemical, or explicitly fantasy military gear
- armouring, weaponsmithing, bowyer, fletching, gunsmithing, and other craft tools in the first wave

### Horse armour and barding scope

Horse equipment should be divided into three catalogue families:

1. **Military tack**: saddle, bridle, bit, and simple riding gear used by mounted warriors.
2. **Display and protection cloth**: caparisons, horse cloths, livery covers, and textile barding; these are usually skinnable and colour-variable.
3. **Protective horse armour**: chanfron, criniere, peytral, flanchards, croupiere, and any mail/scale/lamellar/leather horse defences that fit the culture and date.

Do not treat every mounted culture as having full horse armour. Steppe, Byzantine, Islamic, Indian, Chinese/Korean, Japanese, and western elite cavalry contexts can all use mounted equipment, but the degree of horse protection should be set by period and culture rather than uniform catalogue symmetry.

---

## Player-facing description rules

- Do not mention skins, base prototypes, standard implementations, seeder mechanics, builders, later customization, or archaeological uncertainty in `noun`, `sdesc`, `ldesc`, or `fdesc`.
- Public text avoids real-world culture labels such as Anglo-Saxon, Norse, Norman, Frankish, Byzantine, Abbasid, Fatimid, Seljuk, Mongol, Indian, Chinese, Korean, or Japanese unless both useful and historically intelligible as an object name.
- Public text may use object-form names where useful: `seax`, `hauberk`, `haubergeon`, `byrnie`, `coif`, `spangenhelm`, `nasal helm`, `great helm`, `kettle helm`, `gambeson`, `aketon`, `coat of plates`, `lamellar`, `scale`, `cuirass`, `greaves`, `vambraces`, `bracers`, `gauntlets`, `chausses`, `dhal`, `adarga`, `pavise`, `buckler`, `kite shield`, `heater shield`, `longbow`, `crossbow`, `quiver`, `scabbard`, `sheath`, `sling`, `saddle`, `bridle`, `bit`, `caparison`, `chanfron`, `criniere`, `peytral`, `flanchards`, and `croupiere`.
- Descriptions must stay inspectable: visible shape, finish, material, construction, straps, rivets, rings, plates, layers, edges, shafts, grips, seams, bindings, and wear.
- Horse gear descriptions may describe browbands, cheekpieces, reins, padding, saddle tree shape, cloth covers, lames, scales, plates, straps, buckles, heraldic cloth, and protective coverage, but must not claim mount-control, storage, or hidden-function mechanics unless supported by components.
- Do not claim hidden storage, magical effects, identity concealment, exact armour rating, piercing behaviour, poison, fire, explosive behaviour, locking behaviour, or container behaviour unless a component supports it.
- Avoid modern typological labels when they break immersion. Use “long knife,” “broad-bladed axe,” “iron-headed spear,” “padded jack,” “lamellar cuirass,” or “cloth horse cover” when those read better than academic specificity.

---

## Skin strategy

All finished weapons, armour, shields, ammunition, horse equipment, and support goods should normally be skinnable.

Skins should carry high-variance presentation:

- heraldry
- clan marks
- household marks
- retinue colours
- rank indicators
- unit numbers
- command marks
- exact shield paint
- lacquer colours
- devotional marks
- dynasty or temple motifs
- blade inscriptions
- maker marks
- etched or inlaid decoration
- hilt wrap colour
- scabbard dye
- quiver facing
- saddle facing
- caparison colour and device
- barding textile pattern
- chanfron or peytral decoration
- exotic imported material presentation
- elite polish or gilding
- local object names
- setting-culture names

Default unskinned items must still be credible standalone objects. Skinability must not be used as an excuse for bland base descriptions.

---

## Colour-variable policy

Use colour-variable components only when the visible object surface should vary in ordinary play.

- `Variable_BasicColour`: ordinary dyed textile, leather facing, shield paint, scabbard dye, quiver cloth, padded armour cloth, saddle cloth, caparison cloth, or surcoat/tabard cloth.
- `Variable_FineColour`: elite textiles, fine scabbards, courtly shield covers, formal tabards, high-status quivers, heraldic caparisons, ceremonial wraps, or richer over-armour display.
- `Variable_2BasicColour`: ordinary two-colour shield paint, striped quiver facing, banded textile armour, simple heraldic fields, two-colour caparisons, or two-colour training weapons.
- `Variable_2FineColour`: elite heraldic, courtly, or caparison display where two rich colour fields matter.
- `Variable_DrabColour` and `Variable_2DrabColour`: only for deliberately dirty, degraded, tattered, grimy, battlefield-worn, or scavenged gear. They are not neutral “plain” military colours.
- Metal, natural wood, undyed leather, horn, bone, ordinary iron/steel weapons, and plain tack often omit colour variables.

---

## Materials, sizes, quality, and cost assumptions

### Exact seeded solid materials to prefer

The following exact seeded materials are useful for this branch.

**Metals**

- `wrought iron`
- `mild steel`
- `carbon steel`
- `crucible steel`
- `wootz steel`
- `bronze`
- `brass`
- `lead`
- `copper`
- `silver` and `gold` only for decorative primary-material edge cases, not ordinary weapons

Avoid modern metal names such as `stainless steel`, `galvanized steel`, `powder-coated steel`, `open hearth steel`, and `high tensile steel` unless the setting explicitly supports modern or anachronistic manufacture.

**Woods and plant materials**

- `wood`
- `ash`
- `yew`
- `oak`
- `elm`
- `birch`
- `beech`
- `bamboo`
- `boxwood`
- `hemp`
- `flax`

**Leather, hide, animal, and organic materials**

- `leather`
- `cow leather`
- `deer leather`
- `cow hide`
- `deer hide`
- `animal skin`
- `fur`
- `bone`
- `horn`
- `antler`
- `feather`
- `beeswax`
- `glue`
- `pitch`
- `tar`

**Textiles**

- `linen`
- `wool`
- `cotton`
- `silk`
- `hemp`
- `felt`
- `canvas`
- `burlap`
- `broadcloth`

### Newly confirmed useful military materials

The following historically useful materials are now confirmed as exact seeded solid materials and are used or available for this branch where appropriate:

- `rawhide`
- `sinew`
- `rattan`
- `wicker`
- `reed`
- `cane`
- `lacquer`
- `horsehair`
- `goat leather`
- `sheep leather`

These additions improve descriptive precision for shields, bows, bowstrings, East Asian lacquered gear, wicker shields, hide construction, and horse equipment. Catalogue entries should use these exact material names where they are the dominant primary material.

### Size assumptions

- Tiny or VerySmall: individual sling bullets, individual arrowheads if ever created as components or craft parts.
- Small: daggers, knives, ammunition bundles, helmets, gauntlets, bracers, sheaths, small shields/bucklers, belts, scabbards, quivers, bits, some bridles, and small tack pieces.
- Normal: swords, axes, maces, hammers, shields, bows, crossbows, torso armour, most wearable human armour, saddles, caparisons folded as inventory items, and many horse-armour pieces.
- Large: spears, long spears, pikes, halberds, longbows where the game treats them as large, large weapon racks, armour stands, full caparisons, and large horse armour pieces.
- VeryLarge: assembled full barding sets or very large horse coverings only if implementation treats them as single bulky items rather than separate armour pieces.

### Quality assumptions

- `Substandard`: old, crude, hand-me-down, levy, poorly maintained, rusty, cracked, poorly balanced, ill-fitting, patched, or battlefield-scavenged goods.
- `Standard`: ordinary serviceable military goods.
- `Good`: professional, elite, well-forged, well-fitted, well-balanced, well-riveted, well-maintained, or well-tailored goods.
- `VeryGood` / `Great`: exceptional, masterwork, imported, noble, ceremonial, named-household, or rare elite goods.

### Cost assumptions

Costs are denominated in farthings and should be relative seeder baselines, not universal market prices. Armour, swords, crossbows, and barding should be much more expensive than spears, clubs, staffs, and slings. Mail, lamellar, high-quality helmets, horse armour, elite saddles, decorated caparisons, elite swords, and decorated shields should have significantly higher baseline costs than ordinary commoner weapons.

---

## Exact seeded component appendix

This appendix lists components relevant to the design. The implemented catalogue was checked against these exact seeded component names at implementation time.

### Weapon components

**Melee weapons**

- `Melee_Axe`
- `Melee_Club`
- `Melee_Dagger`
- `Melee_Halberd`
- `Melee_Improvised Bludgeon`
- `Melee_Knife`
- `Melee_Long Spear`
- `Melee_Longsword`
- `Melee_Mace`
- `Melee_Mattock`
- `Melee_Pike`
- `Melee_Quarterstaff`
- `Melee_Short Spear`
- `Melee_Shortsword`
- `Melee_Two Handed Axe`
- `Melee_Two Handed Sword`
- `Melee_Warhammer`
- `Melee_Flail`
- `Melee_Military Pick`
- `Melee_Estoc`

**Training melee weapons**

- `Melee_Training Axe`
- `Melee_Training Club`
- `Melee_Training Dagger`
- `Melee_Training Halberd`
- `Melee_Training Knife`
- `Melee_Training Longsword`
- `Melee_Training Mace`
- `Melee_Training Mattock`
- `Melee_Training Pike`
- `Melee_Training Quarterstaff`
- `Melee_Training Shortsword`
- `Melee_Training Spear`
- `Melee_Training Two Handed Axe`
- `Melee_Training Two Handed Sword`
- `Melee_Training Warhammer`
- `Melee_Training Flail`
- `Melee_Training Military Pick`
- `Melee_Training Estoc`

**Ranged and thrown weapons**

- `Shortbow`
- `Longbow`
- `Crossbow`
- `Hand Crossbow`
- `Sling`
- `Staff Sling`
- `Blowgun`
- `Throwing_Axe`
- `Throwing_Knife`
- `Throwing_Spear`

### Ammunition components

Strict branch candidates:

- `Ammo_BodkinArrow`
- `Ammo_BodkinBolt`
- `Ammo_BroadheadArrow`
- `Ammo_BroadheadBolt`
- `Ammo_FieldPointArrow`
- `Ammo_FieldPointBolt`
- `Ammo_LeadSlingBullet`
- `Ammo_PaddedArrow`
- `Ammo_PaddedBolt`
- `Ammo_SlingBullet`
- `Ammo_TargetArrow`
- `Ammo_TargetBolt`

Deferred special/fantasy or other-culture candidates:

- `Ammo_BarbedBlowgunDart`
- `Ammo_BlowgunDart`
- `Ammo_ConcussiveArrow`
- `Ammo_ConcussiveBolt`

Excluded from strict medieval branch unless a later blackpowder/modern branch is approved:

- `Ammo_.25 ACP`
- `Ammo_.32 ACP`
- `Ammo_.38 ACP`
- `Ammo_.38 Super ACP`
- `Ammo_.45 ACP`
- `Ammo_9mm`

### Armour components

Strict or broad-medieval candidates:

- `Armour_BoiledLeather`
- `Armour_Chainmail`
- `Armour_Padded`
- `Armour_Lamellar`
- `Armour_Laminar`
- `Armour_LeatherScale`
- `Armour_LightClothing`
- `Armour_MetalScale`
- `Armour_StuddedLeather`
- `Armour_Padded`

Cautious mechanical candidate for period-plausible transitional plate only:

- `Armour_RigidMetal`

Excluded from this branch by default:

- boxing gloves
- hearing protection
- modern stab vests
- modern ballistic armour

### Shield components

Strict or broad-medieval candidates:

- `Shield_Improvised`
- `Shield_Buckler`
- `Shield_Kite`
- `Shield_Round`
- `Shield_Heater`
- `Shield_Tower`
- `Shield_Hide`
- `Shield_Wicker`
- `Shield_Dhal`
- `Shield_Adarga`
- `Shield_Targe`
- `Shield_Pavise`

Usually reserved for antiquity, Renaissance, or modern branches:

- `Shield_Parma`
- `Shield_Rotella`
- `Shield_Thureos`
- `Shield_Aspis`
- `Shield_Scutum`
- `Shield_Trench`
- `Shield_Riot`
- `Shield_Ballistic`

### Human wearable components for armour

Useful strict/broad medieval wearable coverage:

- `Wear_Aventail_Spangenhelm`
- `Wear_Backplate`
- `Wear_Bracer`
- `Wear_Bracers`
- `Wear_Brassarts`
- `Wear_Breastplate`
- `Wear_Chausses`
- `Wear_Coif`
- `Wear_Couters`
- `Wear_Cuirass`
- `Wear_Cuisses`
- `Wear_Culeted_Cuirass`
- `Wear_Doublet`
- `Wear_Faulded_Cuirass`
- `Wear_Gauntlets`
- `Wear_Gloves`
- `Wear_Gorget`
- `Wear_Greathelm`
- `Wear_Greaves`
- `Wear_Half_Helmet`
- `Wear_Haubergeon`
- `Wear_Hauberk`
- `Wear_Helmet`
- `Wear_High_Boots`
- `Wear_Jerkin`
- `Wear_Mittens`
- `Wear_Nasal_Helm`
- `Wear_Nasal_Spangenhelm`
- `Wear_Pixane`
- `Wear_Plackart`
- `Wear_Poleyns`
- `Wear_Spangenhelm`
- `Wear_Spaulders`
- `Wear_Tabard`
- `Wear_Tassets`
- `Wear_Trousers`
- `Wear_Tunic`
- `Wear_Vambraces`
- `Wear_Vest`

Use cautiously or defer because the public object name is mainly post-1300 in western usage:

- `Wear_Bevor`
- `Wear_Pauldrons`
- `Wear_Sabatons`
- `Wear_Armet`
- `Wear_Barbute_Helmet`
- `Wear_Open_Armet`
- `Wear_Open_Sallet_Helmet`
- `Wear_Sallet_Helmet`

### Horse tack and barding wearable components

Strict branch candidates when authored as period-plausible horse gear:

- `Wear_Saddle`
- `Wear_Bridle`
- `Wear_Bit`
- `Wear_Caparison`
- `Wear_Chanfron`
- `Wear_Criniere`
- `Wear_Croupiere`
- `Wear_Flanchards`
- `Wear_Peytral`

### Support components

**General handling and stacking**

- `Holdable`
- `Beltable`
- `Stack_Number`
- `Stack_Pile`

**Belts and worn support positions**

- `Belt_2`
- `Belt_4`
- `Belt_6`
- `Belt_Large`
- `Wear_Waist`
- `Wear_Shoulder`
- `Wear_Backpack`
- `Wear_Sash`

**Weapon and armour storage**

- `Sheath_Small`
- `Sheath`
- `Sheath_Large`
- `Container_Quiver`
- `Container_Weapon_Rack`
- `Container_Armor_Stand`

**Destroyable profiles**

- `Destroyable_Weapon`
- `Destroyable_Armour`
- `Destroyable_Shield`
- `Destroyable_Clothing`
- `Destroyable_WoodenHeavy`
- `Destroyable_HeavyMetal`
- `Destroyable_Furniture`
- `Destroyable_Misc`

---

## Exact seeded tag appendix

### Core functional tags

- `Era / Medieval Era`
- `Functions / Military Equipment`
- `Functions / Military Equipment / Military Weapons`
- `Functions / Military Equipment / Military Ammunition`
- `Functions / Military Equipment / Military Armour`
- `Functions / Military Equipment / Military Armour / Military Shields`

### Market tags

- `Market / Military Goods`
- `Market / Military Goods / Weapons`
- `Market / Military Goods / Weapons / Axes`
- `Market / Military Goods / Weapons / Bows`
- `Market / Military Goods / Weapons / Clubs`
- `Market / Military Goods / Weapons / Crossbows`
- `Market / Military Goods / Weapons / Daggers`
- `Market / Military Goods / Weapons / Hammers`
- `Market / Military Goods / Weapons / Maces`
- `Market / Military Goods / Weapons / Other Weapons`
- `Market / Military Goods / Weapons / Polearms`
- `Market / Military Goods / Weapons / Spears`
- `Market / Military Goods / Weapons / Swords`
- `Market / Military Goods / Ammunition`
- `Market / Military Goods / Ammunition / Arrows`
- `Market / Military Goods / Ammunition / Arrows / Bodkin Arrows`
- `Market / Military Goods / Ammunition / Bolts`
- `Market / Military Goods / Ammunition / Bolts / Bodkin Bolts`
- `Market / Military Goods / Ammunition / Bullets`
- `Market / Military Goods / Ammunition / Blackpowder` — excluded from strict core unless a blackpowder branch is approved
- `Market / Military Goods / Armour`
- `Market / Military Goods / Armour / Horse Armour`
- `Market / Military Goods / Armour / Leather Armour`
- `Market / Military Goods / Armour / Mail Armour`
- `Market / Military Goods / Armour / Plate Armour`
- `Market / Military Goods / Armour / Primitive Armour`
- `Market / Military Goods / Armour / Shields`
- `Market / Transportation / Horse Tack`

### Material-function and craft-stock tags

Current exact craft-stock tags use the parent `Antiquity Equipment Stock`. They may remain useful if the project later wants military craft stock or intermediate goods, but this military-goods catalogue should not depend on new medieval stock tags in the first pass.

- `Functions / Material Functions / Antiquity Equipment Stock / Weapon Blade Stock`
- `Functions / Material Functions / Antiquity Equipment Stock / Weapon Head Stock`
- `Functions / Material Functions / Antiquity Equipment Stock / Weapon Shaft Stock`
- `Functions / Material Functions / Antiquity Equipment Stock / Bow Stave`
- `Functions / Material Functions / Antiquity Equipment Stock / Fletching Stock`
- `Functions / Material Functions / Antiquity Equipment Stock / Military Cord Stock`
- `Functions / Material Functions / Antiquity Equipment Stock / Shield Board Stock`
- `Functions / Material Functions / Antiquity Equipment Stock / Shield Facing Stock`
- `Functions / Material Functions / Antiquity Equipment Stock / Helmet Bowl Stock`
- `Functions / Material Functions / Antiquity Equipment Stock / Armour Plate Stock`
- `Functions / Material Functions / Antiquity Equipment Stock / Armour Ring Stock`
- `Functions / Material Functions / Antiquity Equipment Stock / Armour Scale Stock`
- `Functions / Material Functions / Antiquity Equipment Stock / Armour Lamella Stock`
- `Functions / Material Functions / Antiquity Equipment Stock / Armour Textile Padding`

### Military-adjacent tool tags

These exact tags exist and may support a later craft-tools or crafting-support pass. They are **not** part of the first military-goods catalogue wave.

**Armouring tools**

- `Functions / Tools / Armouring Tools`
- `Functions / Tools / Armouring Tools / Armourer's Anvil`
- `Functions / Tools / Armouring Tools / Armourer's Forming Bags`
- `Functions / Tools / Armouring Tools / Armourer's Pliers`
- `Functions / Tools / Armouring Tools / Armourer's Stake`
- `Functions / Tools / Armouring Tools / Ball Stake`
- `Functions / Tools / Armouring Tools / Dishing Form`
- `Functions / Tools / Armouring Tools / Planishing Hammer`
- `Functions / Tools / Armouring Tools / Plate Snips`
- `Functions / Tools / Armouring Tools / Raising Hammer`
- `Functions / Tools / Armouring Tools / T-Stake`

**Weaponsmithing tools**

- `Functions / Tools / Weaponsmithing Tools`
- `Functions / Tools / Weaponsmithing Tools / Crossguard Fixture`
- `Functions / Tools / Weaponsmithing Tools / Forge Bellows`
- `Functions / Tools / Weaponsmithing Tools / Fuller Tool`
- `Functions / Tools / Weaponsmithing Tools / Grindstone`
- `Functions / Tools / Weaponsmithing Tools / Pommel Tightening Jig`
- `Functions / Tools / Weaponsmithing Tools / Quenching Trough`
- `Functions / Tools / Weaponsmithing Tools / Sword Anvil`
- `Functions / Tools / Weaponsmithing Tools / Sword Vise`
- `Functions / Tools / Weaponsmithing Tools / Swordsmith's Hammer`
- `Functions / Tools / Weaponsmithing Tools / Tang Punch`

**Bowyer and fletching tools**

- `Functions / Tools / Woodcrafting Tools / Bowyer Tools`
- `Functions / Tools / Woodcrafting Tools / Bowyer Tools / Bow Press`
- `Functions / Tools / Woodcrafting Tools / Bowyer Tools / Bow Scale`
- `Functions / Tools / Woodcrafting Tools / Bowyer Tools / Tillering Stick`
- `Functions / Tools / Woodcrafting Tools / Fletching Tools / Arrow Jig`

**Gunsmithing tools**

Gunsmithing tool tags exist but are outside this strict branch unless a blackpowder expansion is approved.

---


## Dependency completion for mechanically distinct profiles

The data profiles proposed by the earlier edition are now seeded and used by the relevant existing item rows. Only behaviour beyond the current attack system remains deferred; purely cosmetic variations remain skins or ordinary item variants.

### Weapon profile status

1. **`Melee_Lance` / mounted lance component**

   `Melee_Lance` is seeded, and `medieval_military_light_riding_spear` now uses it. The profile currently reuses supported long-spear attacks; mounted/couched charge mechanics remain deferred.

2. **`Melee_Glaive` or `Melee_Poleblade`**

   `Melee_Poleblade` is seeded and replaces the halberd fallback on `medieval_military_broad_poleblade`.

3. **`Melee_Bill` / `Melee_Guisarme` / hooked polearm component**

   `Melee_HookedPolearm` is seeded and replaces the halberd fallback on `medieval_military_hooked_polearm`. It currently exposes the supported halberd-family attacks; explicit hook, pull, trip, and anti-rider moves remain deferred.

4. **`CompositeBow` or `RecurveBow`**

   `CompositeBow_Light` and `CompositeBow_War` are seeded and now back the two recurved horn-bow rows.

5. **`Melee_Curved Sword` or `Melee_Sabre`**

   `Melee_Sabre` is seeded and now backs the culture-facing `medieval_military_wootz_war_sword`, whose description explicitly identifies its curved single-edged form.

### Armour profile status

1. **`Armour_Padded` or `Armour_Gambeson`**

   `Armour_Padded` is seeded and replaces the heavy-clothing fallbacks on the fourteen padded armour and barding rows.

2. **`Armour_RigidMetal` or `Armour_MetalPlate`**

   `Armour_RigidMetal` is seeded and replaces the mature-platemail fallback on the twenty rigid helmets, plates, and metal limb rows.

3. **`Armour_CoatOfPlates`**

   `Armour_CoatOfPlates` is seeded and used by `medieval_military_coat_of_plates`.

4. **`Armour_Splinted`**

   `Armour_Splinted` is seeded and replaces the studded-leather fallback on the four splinted limb rows.

### Explicitly not requested as component additions here

Do not add training-dummy, pell-post, quintain, or archery-target components for this branch. Training weapons remain in scope because they use existing weapon components and support schools, barracks, tournaments, and militia drills; training fixtures are not part of this first military-goods pass.

---

## Armour and military loadout manifests

The branch retains the “outfit manifest” section concept from the clothing design guide, but these are **builder-facing military loadouts** rather than bundle objects. They are not seeder calls and they do not create composite items. They are practical kit recipes using exact implemented `uniqueReference` values from the first-wave catalogue.

Each loadout lists a conservative default set plus optional substitutions. Builders should choose the option that matches local culture, date, status, and tactical role. Skins should carry heraldry, clan marks, household emblems, livery colours, rank display, exact local names, and most decorative regional identity.

### Shared levy spearman

Default catalogue refs:

- `medieval_military_padded_arming_cap`
- `medieval_military_quilted_militia_jack`
- `medieval_military_worn_levy_spear`
- `medieval_military_plain_rawhide_shield`
- `medieval_military_worn_fighting_knife`
- `medieval_military_plain_arming_belt`

Useful substitutions:

- `medieval_military_plain_wicker_shield`
- `medieval_military_worn_round_shield`
- `medieval_military_plank_shield`
- `medieval_military_plain_short_spear`
- `medieval_military_spearhead_rawhide_cover`

### Shared town militia guard

Default catalogue refs:

- `medieval_military_kettle_style_helmet`
- `medieval_military_hardened_leather_jerkin`
- `medieval_military_plain_short_spear`
- `medieval_military_plain_heater_shield`
- `medieval_military_plain_fighting_knife`
- `medieval_military_broad_weapon_belt`

Useful substitutions:

- `medieval_military_quilted_militia_jack`
- `medieval_military_riveted_leather_jack`
- `medieval_military_plain_iron_mace`
- `medieval_military_worn_short_sword`
- `medieval_military_plain_battle_axe`
- `medieval_military_steel_rimmed_buckler`
- `medieval_military_plain_round_shield`

### Shared archer

Default catalogue refs:

- `medieval_military_padded_arming_cap`
- `medieval_military_sleeveless_padded_jack`
- `medieval_military_plain_elm_shortbow`
- `medieval_military_field_point_arrow`
- `medieval_military_broadhead_arrow`
- `medieval_military_plain_shoulder_arrow_quiver`
- `medieval_military_plain_fighting_knife`
- `medieval_military_archers_utility_belt`

Useful substitutions:

- `medieval_military_light_aketon`
- `medieval_military_yew_longbow`
- `medieval_military_bodkin_arrow`
- `medieval_military_target_arrow`
- `medieval_military_padded_training_arrow`
- `medieval_military_back_arrow_quiver`
- `medieval_military_wicker_arrow_quiver`
- `medieval_military_canvas_longbow_sleeve`

### Shared crossbowman

Default catalogue refs:

- `medieval_military_kettle_style_helmet`
- `medieval_military_quilted_militia_jack`
- `medieval_military_plain_wooden_crossbow`
- `medieval_military_field_point_bolt`
- `medieval_military_broadhead_bolt`
- `medieval_military_plain_bolt_case`
- `medieval_military_crossbowmans_pavise`
- `medieval_military_plain_steel_dagger`

Useful substitutions:

- `medieval_military_short_mail_haubergeon`
- `medieval_military_horn_lath_crossbow`
- `medieval_military_heavy_siege_crossbow`
- `medieval_military_bodkin_bolt`
- `medieval_military_hardened_bodkin_bolt`
- `medieval_military_wooden_crossbow_bolt_box`
- `medieval_military_fine_bolt_case`
- `medieval_military_painted_standing_pavise`

### Shared shielded infantry

Default catalogue refs:

- `medieval_military_conical_steel_helm`
- `medieval_military_short_mail_haubergeon`
- `medieval_military_plain_war_spear`
- `medieval_military_plain_round_shield`
- `medieval_military_plain_steel_dagger`
- `medieval_military_plain_arming_belt`

Useful substitutions:

- `medieval_military_nasal_helm`
- `medieval_military_riveted_leather_jack`
- `medieval_military_plain_battle_axe`
- `medieval_military_plain_iron_mace`
- `medieval_military_plain_arming_sword`
- `medieval_military_plain_kite_shield`
- `medieval_military_plain_heater_shield`

### Western knightly / man-at-arms loadout

Default catalogue refs:

- `medieval_military_padded_arming_doublet`
- `medieval_military_long_mail_hauberk`
- `medieval_military_mail_coif`
- `medieval_military_nasal_helm`
- `medieval_military_mail_chausses`
- `medieval_military_mail_mufflers`
- `medieval_military_plain_kite_shield`
- `medieval_military_fine_war_spear`
- `medieval_military_balanced_arming_sword`
- `medieval_military_plain_sword_scabbard`
- `medieval_military_simple_sword_belt`
- `medieval_military_plain_war_saddle`
- `medieval_military_plain_war_bridle`
- `medieval_military_iron_curb_bit`

Useful substitutions:

- `medieval_military_fine_riveted_mail_hauberk`
- `medieval_military_flat_topped_greathelm`
- `medieval_military_fine_greathelm`
- `medieval_military_fine_knightly_heater_shield`
- `medieval_military_fine_sword_belt`
- `medieval_military_bordered_surcoat`
- `medieval_military_fine_livery_caparison`
- `medieval_military_leather_chanfron`

### Steppe mounted archer loadout

Default catalogue refs:

- `medieval_military_felt_riding_armour`
- `medieval_military_conical_steel_helm`
- `medieval_military_recurved_riders_shortbow`
- `medieval_military_field_point_arrow`
- `medieval_military_bodkin_arrow`
- `medieval_military_riders_side_quiver`
- `medieval_military_light_riding_spear`
- `medieval_military_light_riding_axe`
- `medieval_military_light_cavalry_round_shield`
- `medieval_military_riding_weapon_belt`
- `medieval_military_high_backed_war_saddle`
- `medieval_military_plain_war_bridle`
- `medieval_military_iron_curb_bit`

Useful substitutions:

- `medieval_military_rawhide_lamellar_cuirass`
- `medieval_military_lamellar_helmet`
- `medieval_military_sinew_backed_shortbow`
- `medieval_military_fine_horn_recurve_bow`
- `medieval_military_light_riders_throwing_axe`
- `medieval_military_horseman_throwing_spear`
- `medieval_military_padded_horse_cloth`

### Islamic cavalry / frontier fighter loadout

Default catalogue refs:

- `medieval_military_light_aketon`
- `medieval_military_short_mail_haubergeon`
- `medieval_military_conical_steel_helm`
- `medieval_military_plain_adarga`
- `medieval_military_light_riding_spear`
- `medieval_military_wootz_war_sword`
- `medieval_military_fine_riding_mace`
- `medieval_military_horseman_throwing_spear`
- `medieval_military_fine_horn_recurve_bow`
- `medieval_military_riders_side_quiver`
- `medieval_military_plain_war_saddle`
- `medieval_military_plain_war_bridle`
- `medieval_military_iron_curb_bit`

Useful substitutions:

- `medieval_military_lacquered_lamellar_cuirass`
- `medieval_military_mail_coif`
- `medieval_military_cavalry_adarga`
- `medieval_military_painted_adarga`
- `medieval_military_fine_balanced_javelin`
- `medieval_military_fine_dyed_sword_scabbard`
- `medieval_military_fine_livery_caparison`
- `medieval_military_leather_chanfron`

### Byzantine professional soldier loadout

Default catalogue refs:

- `medieval_military_heavy_gambeson`
- `medieval_military_iron_lamellar_cuirass`
- `medieval_military_nasal_spangenhelm`
- `medieval_military_plain_kite_shield`
- `medieval_military_plain_war_spear`
- `medieval_military_broad_war_sword`
- `medieval_military_bronze_headed_mace`
- `medieval_military_plain_war_saddle`
- `medieval_military_plain_war_bridle`
- `medieval_military_iron_curb_bit`

Useful substitutions:

- `medieval_military_short_mail_haubergeon`
- `medieval_military_aventail_spangenhelm`
- `medieval_military_plain_round_shield`
- `medieval_military_horn_lath_crossbow`
- `medieval_military_lamellar_chanfron`
- `medieval_military_lamellar_peytral`

### Japanese mounted warrior loadout

Default catalogue refs:

- `medieval_military_padded_arming_doublet`
- `medieval_military_lacquered_lamellar_cuirass`
- `medieval_military_lamellar_spaulders`
- `medieval_military_lamellar_skirt`
- `medieval_military_lamellar_bracers`
- `medieval_military_lamellar_helmet`
- `medieval_military_bamboo_shortbow`
- `medieval_military_bodkin_arrow`
- `medieval_military_lacquered_arrow_quiver`
- `medieval_military_fine_long_gripped_war_sword`
- `medieval_military_broad_poleblade`
- `medieval_military_high_backed_war_saddle`
- `medieval_military_plain_war_bridle`
- `medieval_military_iron_curb_bit`

Useful substitutions:

- `medieval_military_iron_lamellar_cuirass`
- `medieval_military_lamellar_cuisses`
- `medieval_military_bordered_surcoat`
- `medieval_military_fine_horn_recurve_bow`
- `medieval_military_fine_lacquered_bowcase`
- `medieval_military_lamellar_chanfron`
- `medieval_military_lamellar_peytral`
- `medieval_military_lamellar_croupiere`

### Indian elite fighter loadout

Default catalogue refs:

- `medieval_military_light_aketon`
- `medieval_military_metal_scale_corselet`
- `medieval_military_conical_steel_helm`
- `medieval_military_fine_polished_dhal`
- `medieval_military_wootz_war_sword`
- `medieval_military_plain_war_spear`
- `medieval_military_fine_horn_recurve_bow`
- `medieval_military_riders_side_quiver`
- `medieval_military_fine_dyed_sword_scabbard`
- `medieval_military_fine_sword_belt`
- `medieval_military_high_backed_war_saddle`
- `medieval_military_plain_war_bridle`
- `medieval_military_iron_curb_bit`

Useful substitutions:

- `medieval_military_bronze_scale_cuirass`
- `medieval_military_four_boss_dhal`
- `medieval_military_rawhide_dhal`
- `medieval_military_fine_balanced_javelin`
- `medieval_military_fine_lacquered_bowcase`
- `medieval_military_fine_livery_caparison`
- `medieval_military_scale_chanfron`
- `medieval_military_scale_croupiere`

### Shared military horse equipment and barded horse set

Default catalogue refs:

- `medieval_military_plain_war_saddle`
- `medieval_military_plain_war_bridle`
- `medieval_military_iron_curb_bit`
- `medieval_military_plain_caparison`

Useful mounted-warrior substitutions:

- `medieval_military_high_backed_war_saddle`
- `medieval_military_fine_livery_caparison`
- `medieval_military_padded_horse_cloth`

Protective barding refs, used only where culture, status, and date support them:

- `medieval_military_leather_chanfron`
- `medieval_military_scale_chanfron`
- `medieval_military_lamellar_chanfron`
- `medieval_military_mail_criniere`
- `medieval_military_leather_criniere`
- `medieval_military_leather_peytral`
- `medieval_military_lamellar_peytral`
- `medieval_military_mail_flanchards`
- `medieval_military_leather_flanchards`
- `medieval_military_scale_croupiere`
- `medieval_military_lamellar_croupiere`

### Garrison armoury support set

Default catalogue refs:

- `medieval_military_wall_spear_rack`
- `medieval_military_simple_weapon_rack`
- `medieval_military_sword_rack`
- `medieval_military_bow_rack`
- `medieval_military_crossbow_rack`
- `medieval_military_simple_armour_stand`

Useful substitutions:

- `medieval_military_polearm_rack`
- `medieval_military_iron_armoury_rack`
- `medieval_military_sturdy_armour_tree`
- `medieval_military_mail_armour_stand`
- `medieval_military_mounted_harness_stand`
- `medieval_military_barding_trestle`
- `medieval_military_fine_display_armour_stand`

---

## Implemented item catalogue

Implemented catalogue total: **381** unique item prototypes.

Catalogue line format: `uniqueReference` - public short description; noun; primary material; size/quality; weight/cost; components. Full descriptions and complete `CreateItem(...)` statements are in the two live Medieval military ItemSeeder partials and are not duplicated here.

### Implementation pass summary

- Melee weapons: **77** prototypes.
- Armour, horse tack, and barding: **105** prototypes.
- Shields: **65** prototypes.
- Ranged weapons, ammunition, and thrown weapons: **65** prototypes.
- Carrying, storage, and support gear: **69** prototypes.

All ordinary portable goods are authored as player-visible, skinnable finished goods. Installed racks and stands intentionally omit `Holdable` and use long descriptions. Ammunition includes an `Ammo_*` component plus `Stack_Number`.

### Melee weapons (77)

- `medieval_military_worn_fighting_knife` - a worn fighting knife; noun: `knife`; material: `wrought iron`; size/quality: `Small`/`Substandard`; weight/cost: 260.0g/12.0m; components: `Holdable`, `Melee_Knife`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_plain_fighting_knife` - a plain fighting knife; noun: `knife`; material: `carbon steel`; size/quality: `Small`/`Standard`; weight/cost: 280.0g/28.0m; components: `Holdable`, `Melee_Knife`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_broad_seax_knife` - a broad-backed seax; noun: `seax`; material: `carbon steel`; size/quality: `Small`/`Good`; weight/cost: 620.0g/96.0m; components: `Holdable`, `Melee_Knife`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_worn_iron_dagger` - a worn iron dagger; noun: `dagger`; material: `wrought iron`; size/quality: `Small`/`Substandard`; weight/cost: 330.0g/32.0m; components: `Holdable`, `Melee_Dagger`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_plain_steel_dagger` - a plain steel dagger; noun: `dagger`; material: `carbon steel`; size/quality: `Small`/`Standard`; weight/cost: 340.0g/72.0m; components: `Holdable`, `Melee_Dagger`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_balanced_steel_dagger` - a balanced steel dagger; noun: `dagger`; material: `carbon steel`; size/quality: `Small`/`Good`; weight/cost: 320.0g/160.0m; components: `Holdable`, `Melee_Dagger`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_crucible_steel_dagger` - a crucible-steel dagger; noun: `dagger`; material: `crucible steel`; size/quality: `Small`/`VeryGood`; weight/cost: 310.0g/360.0m; components: `Holdable`, `Melee_Dagger`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_worn_short_sword` - a worn short sword; noun: `shortsword`; material: `wrought iron`; size/quality: `Normal`/`Substandard`; weight/cost: 900.0g/120.0m; components: `Holdable`, `Melee_Shortsword`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_plain_short_sword` - a plain short sword; noun: `shortsword`; material: `carbon steel`; size/quality: `Normal`/`Standard`; weight/cost: 850.0g/260.0m; components: `Holdable`, `Melee_Shortsword`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_fine_short_sword` - a fine short sword; noun: `shortsword`; material: `carbon steel`; size/quality: `Normal`/`Good`; weight/cost: 820.0g/580.0m; components: `Holdable`, `Melee_Shortsword`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_hand_me_down_sword` - a hand-me-down sword; noun: `sword`; material: `wrought iron`; size/quality: `Normal`/`Substandard`; weight/cost: 1220.0g/260.0m; components: `Holdable`, `Melee_Longsword`, `Destroyable_Weapon`.
- `medieval_military_plain_arming_sword` - a plain arming sword; noun: `sword`; material: `carbon steel`; size/quality: `Normal`/`Standard`; weight/cost: 1180.0g/720.0m; components: `Holdable`, `Melee_Longsword`, `Destroyable_Weapon`.
- `medieval_military_broad_war_sword` - a broad war sword; noun: `sword`; material: `carbon steel`; size/quality: `Normal`/`Standard`; weight/cost: 1370.0g/820.0m; components: `Holdable`, `Melee_Longsword`, `Destroyable_Weapon`.
- `medieval_military_balanced_arming_sword` - a balanced arming sword; noun: `sword`; material: `carbon steel`; size/quality: `Normal`/`Good`; weight/cost: 1120.0g/1500.0m; components: `Holdable`, `Melee_Longsword`, `Destroyable_Weapon`.
- `medieval_military_crucible_steel_sword` - a crucible-steel sword; noun: `sword`; material: `crucible steel`; size/quality: `Normal`/`VeryGood`; weight/cost: 1080.0g/3000.0m; components: `Holdable`, `Melee_Longsword`, `Destroyable_Weapon`.
- `medieval_military_wootz_war_sword` - a watered war sword; noun: `sword`; material: `wootz steel`; size/quality: `Normal`/`VeryGood`; weight/cost: 1150.0g/3400.0m; components: `Holdable`, `Melee_Sabre`, `Destroyable_Weapon`.
- `medieval_military_plain_long_gripped_war_sword` - a long-gripped war sword; noun: `sword`; material: `carbon steel`; size/quality: `Large`/`Standard`; weight/cost: 2050.0g/1320.0m; components: `Holdable`, `Melee_Two Handed Sword`, `Destroyable_Weapon`.
- `medieval_military_fine_long_gripped_war_sword` - a fine long-gripped war sword; noun: `sword`; material: `carbon steel`; size/quality: `Large`/`Good`; weight/cost: 1980.0g/2600.0m; components: `Holdable`, `Melee_Two Handed Sword`, `Destroyable_Weapon`.
- `medieval_military_worn_hand_axe` - a worn hand axe; noun: `axe`; material: `wrought iron`; size/quality: `Normal`/`Substandard`; weight/cost: 720.0g/28.0m; components: `Holdable`, `Melee_Axe`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_plain_battle_axe` - a plain battle axe; noun: `axe`; material: `carbon steel`; size/quality: `Normal`/`Standard`; weight/cost: 980.0g/76.0m; components: `Holdable`, `Melee_Axe`, `Destroyable_Weapon`.
- `medieval_military_broad_bearded_axe` - a broad bearded axe; noun: `axe`; material: `carbon steel`; size/quality: `Normal`/`Good`; weight/cost: 1050.0g/180.0m; components: `Holdable`, `Melee_Axe`, `Destroyable_Weapon`.
- `medieval_military_light_riding_axe` - a light riding axe; noun: `axe`; material: `carbon steel`; size/quality: `Normal`/`Good`; weight/cost: 820.0g/190.0m; components: `Holdable`, `Melee_Axe`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_long_war_axe` - a long war axe; noun: `axe`; material: `carbon steel`; size/quality: `Large`/`Standard`; weight/cost: 1900.0g/140.0m; components: `Holdable`, `Melee_Two Handed Axe`, `Destroyable_Weapon`.
- `medieval_military_fine_long_war_axe` - a fine long war axe; noun: `axe`; material: `carbon steel`; size/quality: `Large`/`Good`; weight/cost: 1840.0g/340.0m; components: `Holdable`, `Melee_Two Handed Axe`, `Destroyable_Weapon`.
- `medieval_military_rough_wooden_bludgeon` - a rough wooden bludgeon; noun: `bludgeon`; material: `oak`; size/quality: `Normal`/`Substandard`; weight/cost: 900.0g/4.0m; components: `Holdable`, `Melee_Improvised Bludgeon`, `Destroyable_Weapon`.
- `medieval_military_hardwood_cudgel` - a hardwood cudgel; noun: `cudgel`; material: `oak`; size/quality: `Normal`/`Standard`; weight/cost: 760.0g/8.0m; components: `Holdable`, `Melee_Club`, `Destroyable_Weapon`.
- `medieval_military_iron_bound_club` - an iron-bound club; noun: `club`; material: `oak`; size/quality: `Normal`/`Good`; weight/cost: 1100.0g/40.0m; components: `Holdable`, `Melee_Club`, `Destroyable_Weapon`.
- `medieval_military_weighted_war_club` - a weighted war club; noun: `club`; material: `oak`; size/quality: `Normal`/`Good`; weight/cost: 1250.0g/52.0m; components: `Holdable`, `Melee_Club`, `Destroyable_Weapon`.
- `medieval_military_worn_quarterstaff` - a worn quarterstaff; noun: `quarterstaff`; material: `ash`; size/quality: `Large`/`Substandard`; weight/cost: 1500.0g/6.0m; components: `Holdable`, `Melee_Quarterstaff`, `Destroyable_Weapon`.
- `medieval_military_ash_quarterstaff` - an ash quarterstaff; noun: `quarterstaff`; material: `ash`; size/quality: `Large`/`Standard`; weight/cost: 1450.0g/14.0m; components: `Holdable`, `Melee_Quarterstaff`, `Destroyable_Weapon`.
- `medieval_military_heavy_fighting_staff` - a heavy fighting staff; noun: `staff`; material: `ash`; size/quality: `Large`/`Good`; weight/cost: 1850.0g/48.0m; components: `Holdable`, `Melee_Quarterstaff`, `Destroyable_Weapon`.
- `medieval_military_worn_iron_mace` - a worn iron mace; noun: `mace`; material: `wrought iron`; size/quality: `Normal`/`Substandard`; weight/cost: 1180.0g/72.0m; components: `Holdable`, `Melee_Mace`, `Destroyable_Weapon`.
- `medieval_military_plain_iron_mace` - a plain iron mace; noun: `mace`; material: `wrought iron`; size/quality: `Normal`/`Standard`; weight/cost: 1250.0g/140.0m; components: `Holdable`, `Melee_Mace`, `Destroyable_Weapon`.
- `medieval_military_bronze_headed_mace` - a bronze-headed mace; noun: `mace`; material: `bronze`; size/quality: `Normal`/`Standard`; weight/cost: 1180.0g/180.0m; components: `Holdable`, `Melee_Mace`, `Destroyable_Weapon`.
- `medieval_military_flanged_steel_mace` - a flanged steel mace; noun: `mace`; material: `carbon steel`; size/quality: `Normal`/`Good`; weight/cost: 1220.0g/320.0m; components: `Holdable`, `Melee_Mace`, `Destroyable_Weapon`.
- `medieval_military_fine_riding_mace` - a fine riding mace; noun: `mace`; material: `carbon steel`; size/quality: `Normal`/`Good`; weight/cost: 1050.0g/380.0m; components: `Holdable`, `Melee_Mace`, `Destroyable_Weapon`.
- `medieval_military_worn_war_hammer` - a worn war hammer; noun: `hammer`; material: `wrought iron`; size/quality: `Normal`/`Substandard`; weight/cost: 950.0g/64.0m; components: `Holdable`, `Melee_Warhammer`, `Destroyable_Weapon`.
- `medieval_military_plain_war_hammer` - a plain war hammer; noun: `hammer`; material: `carbon steel`; size/quality: `Normal`/`Standard`; weight/cost: 980.0g/160.0m; components: `Holdable`, `Melee_Warhammer`, `Destroyable_Weapon`.
- `medieval_military_fine_steel_war_hammer` - a fine steel war hammer; noun: `hammer`; material: `carbon steel`; size/quality: `Normal`/`Good`; weight/cost: 930.0g/360.0m; components: `Holdable`, `Melee_Warhammer`, `Destroyable_Weapon`.
- `medieval_military_worn_military_pick` - a worn military pick; noun: `pick`; material: `wrought iron`; size/quality: `Normal`/`Substandard`; weight/cost: 920.0g/56.0m; components: `Holdable`, `Melee_Military Pick`, `Destroyable_Weapon`.
- `medieval_military_plain_military_pick` - a plain military pick; noun: `pick`; material: `carbon steel`; size/quality: `Normal`/`Standard`; weight/cost: 940.0g/150.0m; components: `Holdable`, `Melee_Military Pick`, `Destroyable_Weapon`.
- `medieval_military_balanced_military_pick` - a balanced military pick; noun: `pick`; material: `carbon steel`; size/quality: `Normal`/`Good`; weight/cost: 900.0g/330.0m; components: `Holdable`, `Melee_Military Pick`, `Destroyable_Weapon`.
- `medieval_military_field_mattock_weapon` - a field mattock weapon; noun: `mattock`; material: `wrought iron`; size/quality: `Large`/`Substandard`; weight/cost: 2100.0g/18.0m; components: `Holdable`, `Melee_Mattock`, `Destroyable_Weapon`.
- `medieval_military_reinforced_war_mattock` - a reinforced war mattock; noun: `mattock`; material: `carbon steel`; size/quality: `Large`/`Standard`; weight/cost: 2300.0g/86.0m; components: `Holdable`, `Melee_Mattock`, `Destroyable_Weapon`.
- `medieval_military_repurposed_grain_flail` - a repurposed grain flail; noun: `flail`; material: `ash`; size/quality: `Large`/`Substandard`; weight/cost: 1550.0g/14.0m; components: `Holdable`, `Melee_Flail`, `Destroyable_Weapon`.
- `medieval_military_iron_shod_flail` - an iron-shod flail; noun: `flail`; material: `wrought iron`; size/quality: `Large`/`Standard`; weight/cost: 1700.0g/120.0m; components: `Holdable`, `Melee_Flail`, `Destroyable_Weapon`.
- `medieval_military_spiked_steel_flail` - a spiked steel flail; noun: `flail`; material: `carbon steel`; size/quality: `Large`/`Good`; weight/cost: 1750.0g/280.0m; components: `Holdable`, `Melee_Flail`, `Destroyable_Weapon`.
- `medieval_military_worn_levy_spear` - a worn levy spear; noun: `spear`; material: `wrought iron`; size/quality: `Large`/`Substandard`; weight/cost: 1650.0g/20.0m; components: `Holdable`, `Melee_Long Spear`, `Destroyable_Weapon`.
- `medieval_military_plain_short_spear` - a plain short spear; noun: `spear`; material: `carbon steel`; size/quality: `Large`/`Standard`; weight/cost: 1450.0g/44.0m; components: `Holdable`, `Melee_Short Spear`, `Destroyable_Weapon`.
- `medieval_military_boar_spear` - a lugged boar spear; noun: `spear`; material: `carbon steel`; size/quality: `Large`/`Good`; weight/cost: 1900.0g/110.0m; components: `Holdable`, `Melee_Short Spear`, `Destroyable_Weapon`.
- `medieval_military_plain_war_spear` - a plain war spear; noun: `spear`; material: `carbon steel`; size/quality: `Large`/`Standard`; weight/cost: 1750.0g/52.0m; components: `Holdable`, `Melee_Long Spear`, `Destroyable_Weapon`.
- `medieval_military_fine_war_spear` - a fine war spear; noun: `spear`; material: `carbon steel`; size/quality: `Large`/`Good`; weight/cost: 1700.0g/150.0m; components: `Holdable`, `Melee_Long Spear`, `Destroyable_Weapon`.
- `medieval_military_light_riding_spear` - a light riding spear; noun: `spear`; material: `carbon steel`; size/quality: `Large`/`Standard`; weight/cost: 1600.0g/78.0m; components: `Holdable`, `Melee_Lance`, `Destroyable_Weapon`.
- `medieval_military_worn_infantry_pike` - a worn infantry pike; noun: `pike`; material: `wrought iron`; size/quality: `Large`/`Substandard`; weight/cost: 3000.0g/36.0m; components: `Holdable`, `Melee_Pike`, `Destroyable_Weapon`.
- `medieval_military_plain_infantry_pike` - a plain infantry pike; noun: `pike`; material: `carbon steel`; size/quality: `Large`/`Standard`; weight/cost: 2950.0g/84.0m; components: `Holdable`, `Melee_Pike`, `Destroyable_Weapon`.
- `medieval_military_fine_steel_pike` - a fine steel-headed pike; noun: `pike`; material: `carbon steel`; size/quality: `Large`/`Good`; weight/cost: 2900.0g/180.0m; components: `Holdable`, `Melee_Pike`, `Destroyable_Weapon`.
- `medieval_military_plain_axe_headed_polearm` - an axe-headed polearm; noun: `polearm`; material: `carbon steel`; size/quality: `Large`/`Standard`; weight/cost: 2300.0g/160.0m; components: `Holdable`, `Melee_Halberd`, `Destroyable_Weapon`.
- `medieval_military_fine_axe_headed_polearm` - a fine axe-headed polearm; noun: `polearm`; material: `carbon steel`; size/quality: `Large`/`Good`; weight/cost: 2200.0g/380.0m; components: `Holdable`, `Melee_Halberd`, `Destroyable_Weapon`.
- `medieval_military_broad_poleblade` - a broad steel poleblade; noun: `poleblade`; material: `carbon steel`; size/quality: `Large`/`Standard`; weight/cost: 2050.0g/140.0m; components: `Holdable`, `Melee_Poleblade`, `Destroyable_Weapon`.
- `medieval_military_hooked_polearm` - a hooked polearm; noun: `polearm`; material: `carbon steel`; size/quality: `Large`/`Good`; weight/cost: 2150.0g/300.0m; components: `Holdable`, `Melee_HookedPolearm`, `Destroyable_Weapon`.
- `medieval_military_wooden_practice_knife` - a wooden practice knife; noun: `knife`; material: `beech`; size/quality: `Small`/`Standard`; weight/cost: 180.0g/4.0m; components: `Holdable`, `Melee_Training Knife`, `Destroyable_Weapon`.
- `medieval_military_wooden_practice_dagger` - a wooden practice dagger; noun: `dagger`; material: `beech`; size/quality: `Small`/`Standard`; weight/cost: 220.0g/6.0m; components: `Holdable`, `Melee_Training Dagger`, `Destroyable_Weapon`.
- `medieval_military_short_sword_waster` - a short sword waster; noun: `shortsword`; material: `beech`; size/quality: `Normal`/`Standard`; weight/cost: 620.0g/12.0m; components: `Holdable`, `Melee_Training Shortsword`, `Destroyable_Weapon`.
- `medieval_military_arming_sword_waster` - an arming sword waster; noun: `sword`; material: `beech`; size/quality: `Normal`/`Standard`; weight/cost: 760.0g/18.0m; components: `Holdable`, `Melee_Training Longsword`, `Destroyable_Weapon`.
- `medieval_military_two_handed_waster` - a two-handed waster; noun: `sword`; material: `beech`; size/quality: `Large`/`Standard`; weight/cost: 1350.0g/32.0m; components: `Holdable`, `Melee_Training Two Handed Sword`, `Destroyable_Weapon`.
- `medieval_military_practice_axe` - a wooden practice axe; noun: `axe`; material: `beech`; size/quality: `Normal`/`Standard`; weight/cost: 620.0g/12.0m; components: `Holdable`, `Melee_Training Axe`, `Destroyable_Weapon`.
- `medieval_military_practice_long_axe` - a wooden practice long axe; noun: `axe`; material: `beech`; size/quality: `Large`/`Standard`; weight/cost: 1200.0g/22.0m; components: `Holdable`, `Melee_Training Two Handed Axe`, `Destroyable_Weapon`.
- `medieval_military_training_club` - a padded training club; noun: `club`; material: `beech`; size/quality: `Normal`/`Standard`; weight/cost: 650.0g/10.0m; components: `Holdable`, `Melee_Training Club`, `Destroyable_Weapon`.
- `medieval_military_training_quarterstaff` - a training quarterstaff; noun: `quarterstaff`; material: `ash`; size/quality: `Large`/`Standard`; weight/cost: 1350.0g/12.0m; components: `Holdable`, `Melee_Training Quarterstaff`, `Destroyable_Weapon`.
- `medieval_military_training_mace` - a padded training mace; noun: `mace`; material: `beech`; size/quality: `Normal`/`Standard`; weight/cost: 760.0g/16.0m; components: `Holdable`, `Melee_Training Mace`, `Destroyable_Weapon`.
- `medieval_military_training_war_hammer` - a padded training hammer; noun: `hammer`; material: `beech`; size/quality: `Normal`/`Standard`; weight/cost: 720.0g/18.0m; components: `Holdable`, `Melee_Training Warhammer`, `Destroyable_Weapon`.
- `medieval_military_training_military_pick` - a blunted training pick; noun: `pick`; material: `beech`; size/quality: `Normal`/`Standard`; weight/cost: 700.0g/18.0m; components: `Holdable`, `Melee_Training Military Pick`, `Destroyable_Weapon`.
- `medieval_military_training_mattock` - a blunted training mattock; noun: `mattock`; material: `beech`; size/quality: `Large`/`Standard`; weight/cost: 1500.0g/22.0m; components: `Holdable`, `Melee_Training Mattock`, `Destroyable_Weapon`.
- `medieval_military_training_flail` - a padded training flail; noun: `flail`; material: `ash`; size/quality: `Large`/`Standard`; weight/cost: 1000.0g/24.0m; components: `Holdable`, `Melee_Training Flail`, `Destroyable_Weapon`.
- `medieval_military_training_spear` - a blunted training spear; noun: `spear`; material: `ash`; size/quality: `Large`/`Standard`; weight/cost: 1250.0g/16.0m; components: `Holdable`, `Melee_Training Spear`, `Destroyable_Weapon`.
- `medieval_military_training_pike` - a blunted training pike; noun: `pike`; material: `ash`; size/quality: `Large`/`Standard`; weight/cost: 2200.0g/28.0m; components: `Holdable`, `Melee_Training Pike`, `Destroyable_Weapon`.
- `medieval_military_training_axe_headed_polearm` - a wooden practice polearm; noun: `polearm`; material: `beech`; size/quality: `Large`/`Standard`; weight/cost: 1400.0g/32.0m; components: `Holdable`, `Melee_Training Halberd`, `Destroyable_Weapon`.

### Armour, horse tack, and barding (105)

- `medieval_military_padded_arming_cap` - a $colour padded arming cap; noun: `cap`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 320.0g/12.0m; components: `Holdable`, `Wear_Coif`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Moderate`, `Variable_BasicColour`.
- `medieval_military_heavy_padded_coif` - a heavy padded coif; noun: `coif`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 520.0g/24.0m; components: `Holdable`, `Wear_Coif`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Balanced_Heavy`.
- `medieval_military_light_aketon` - a $colour light aketon; noun: `aketon`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 2200.0g/72.0m; components: `Holdable`, `Wear_Long-Sleeved_Tunic`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Moderate`, `Variable_BasicColour`.
- `medieval_military_heavy_gambeson` - a thick quilted gambeson; noun: `gambeson`; material: `linen`; size/quality: `Large`/`Good`; weight/cost: 4300.0g/160.0m; components: `Holdable`, `Wear_Long-Sleeved_Tunic`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Balanced_Heavy`.
- `medieval_military_sleeveless_padded_jack` - a sleeveless padded jack; noun: `jack`; material: `canvas`; size/quality: `Normal`/`Standard`; weight/cost: 2500.0g/84.0m; components: `Holdable`, `Wear_Vest`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Moderate`.
- `medieval_military_quilted_militia_jack` - a quilted militia jack; noun: `jack`; material: `canvas`; size/quality: `Normal`/`Standard`; weight/cost: 3400.0g/120.0m; components: `Holdable`, `Wear_Jerkin`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Balanced_Heavy`.
- `medieval_military_felt_riding_armour` - a felt riding armour coat; noun: `coat`; material: `felt`; size/quality: `Large`/`Standard`; weight/cost: 3900.0g/144.0m; components: `Holdable`, `Wear_Long-Sleeved_Tunic`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Strong`.
- `medieval_military_padded_arming_doublet` - a fitted arming doublet; noun: `doublet`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 2600.0g/140.0m; components: `Holdable`, `Wear_Doublet`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Moderate`.
- `medieval_military_padded_chausses` - a pair of padded chausses; noun: `chausses`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 1750.0g/64.0m; components: `Holdable`, `Wear_Chausses`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Moderate`.
- `medieval_military_padded_fighting_trousers` - a pair of padded fighting trousers; noun: `trousers`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1600.0g/48.0m; components: `Holdable`, `Wear_Trousers`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Moderate`.
- `medieval_military_padded_mittens` - a pair of padded fighting mittens; noun: `mittens`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 430.0g/20.0m; components: `Holdable`, `Wear_Mittens`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Moderate`.
- `medieval_military_padded_forearm_guards` - a pair of padded forearm guards; noun: `bracers`; material: `canvas`; size/quality: `Small`/`Standard`; weight/cost: 460.0g/18.0m; components: `Holdable`, `Wear_Bracers`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Minor`.
- `medieval_military_rawhide_chestguard` - a rawhide chestguard; noun: `chestguard`; material: `rawhide`; size/quality: `Normal`/`Substandard`; weight/cost: 2100.0g/52.0m; components: `Holdable`, `Wear_Cuirass`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_boiled_leather_cuirass` - a boiled leather cuirass; noun: `cuirass`; material: `leather`; size/quality: `Normal`/`Standard`; weight/cost: 2800.0g/180.0m; components: `Holdable`, `Wear_Cuirass`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_hardened_leather_jerkin` - a hardened leather jerkin; noun: `jerkin`; material: `leather`; size/quality: `Normal`/`Standard`; weight/cost: 2100.0g/112.0m; components: `Holdable`, `Wear_Jerkin`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_riveted_leather_jack` - a rivet-reinforced leather jack; noun: `jack`; material: `leather`; size/quality: `Normal`/`Good`; weight/cost: 3200.0g/220.0m; components: `Holdable`, `Wear_Jerkin`, `Armour_StuddedLeather`, `Destroyable_Armour`.
- `medieval_military_leather_scale_vest` - a leather scale vest; noun: `vest`; material: `leather`; size/quality: `Normal`/`Standard`; weight/cost: 2600.0g/150.0m; components: `Holdable`, `Wear_Vest`, `Armour_LeatherScale`, `Destroyable_Armour`.
- `medieval_military_leather_scale_skirt` - a leather scale armour skirt; noun: `skirt`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 1250.0g/76.0m; components: `Holdable`, `Wear_Tassets`, `Armour_LeatherScale`, `Destroyable_Armour`.
- `medieval_military_boiled_leather_bracers` - a pair of boiled leather bracers; noun: `bracers`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 560.0g/36.0m; components: `Holdable`, `Wear_Bracers`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_splinted_leather_vambraces` - a pair of splinted leather vambraces; noun: `vambraces`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 780.0g/92.0m; components: `Holdable`, `Wear_Vambraces`, `Armour_Splinted`, `Destroyable_Armour`.
- `medieval_military_leather_greaves` - a pair of boiled leather greaves; noun: `greaves`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 980.0g/64.0m; components: `Holdable`, `Wear_Greaves`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_leather_cuisses` - a pair of boiled leather cuisses; noun: `cuisses`; material: `leather`; size/quality: `Normal`/`Standard`; weight/cost: 1200.0g/82.0m; components: `Holdable`, `Wear_Cuisses`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_guarded_leather_gloves` - a pair of guarded leather gloves; noun: `gloves`; material: `goat leather`; size/quality: `Small`/`Good`; weight/cost: 480.0g/60.0m; components: `Holdable`, `Wear_Gloves`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_leather_spaulders` - a pair of leather spaulders; noun: `spaulders`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 920.0g/78.0m; components: `Holdable`, `Wear_Spaulders`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_worn_mail_byrnie` - a worn mail byrnie; noun: `byrnie`; material: `wrought iron`; size/quality: `Normal`/`Substandard`; weight/cost: 5600.0g/420.0m; components: `Holdable`, `Wear_Haubergeon`, `Armour_Chainmail`, `Destroyable_Armour`.
- `medieval_military_short_mail_haubergeon` - a short mail haubergeon; noun: `haubergeon`; material: `mild steel`; size/quality: `Normal`/`Standard`; weight/cost: 7200.0g/820.0m; components: `Holdable`, `Wear_Haubergeon`, `Armour_Chainmail`, `Destroyable_Armour`.
- `medieval_military_long_mail_hauberk` - a long mail hauberk; noun: `hauberk`; material: `mild steel`; size/quality: `Large`/`Good`; weight/cost: 10800.0g/1560.0m; components: `Holdable`, `Wear_Hauberk`, `Armour_Chainmail`, `Destroyable_Armour`.
- `medieval_military_fine_riveted_mail_hauberk` - a fine riveted mail hauberk; noun: `hauberk`; material: `mild steel`; size/quality: `Large`/`VeryGood`; weight/cost: 9600.0g/2600.0m; components: `Holdable`, `Wear_Hauberk`, `Armour_Chainmail`, `Destroyable_Armour`.
- `medieval_military_mail_coif` - a riveted mail coif; noun: `coif`; material: `mild steel`; size/quality: `Small`/`Standard`; weight/cost: 2300.0g/280.0m; components: `Holdable`, `Wear_Coif`, `Armour_Chainmail`, `Destroyable_Armour`.
- `medieval_military_mail_pixane` - a mail pixane collar; noun: `pixane`; material: `mild steel`; size/quality: `Small`/`Standard`; weight/cost: 1500.0g/180.0m; components: `Holdable`, `Wear_Pixane`, `Armour_Chainmail`, `Destroyable_Armour`.
- `medieval_military_mail_mufflers` - a pair of mail mufflers; noun: `mufflers`; material: `mild steel`; size/quality: `Small`/`Good`; weight/cost: 980.0g/180.0m; components: `Holdable`, `Wear_Mittens`, `Armour_Chainmail`, `Destroyable_Armour`.
- `medieval_military_mail_chausses` - a pair of mail chausses; noun: `chausses`; material: `mild steel`; size/quality: `Normal`/`Good`; weight/cost: 6700.0g/940.0m; components: `Holdable`, `Wear_Chausses`, `Armour_Chainmail`, `Destroyable_Armour`.
- `medieval_military_mail_upper_arm_defences` - a pair of mail upper-arm defences; noun: `brassarts`; material: `mild steel`; size/quality: `Small`/`Standard`; weight/cost: 1450.0g/190.0m; components: `Holdable`, `Wear_Brassarts`, `Armour_Chainmail`, `Destroyable_Armour`.
- `medieval_military_mail_skirt` - a mail skirt; noun: `skirt`; material: `mild steel`; size/quality: `Small`/`Good`; weight/cost: 2600.0g/360.0m; components: `Holdable`, `Wear_Tassets`, `Armour_Chainmail`, `Destroyable_Armour`.
- `medieval_military_metal_scale_corselet` - a metal scale corselet; noun: `corselet`; material: `mild steel`; size/quality: `Normal`/`Standard`; weight/cost: 5200.0g/540.0m; components: `Holdable`, `Wear_Cuirass`, `Armour_MetalScale`, `Destroyable_Armour`.
- `medieval_military_bronze_scale_cuirass` - a bronze scale cuirass; noun: `cuirass`; material: `bronze`; size/quality: `Normal`/`Good`; weight/cost: 5600.0g/620.0m; components: `Holdable`, `Wear_Cuirass`, `Armour_MetalScale`, `Destroyable_Armour`.
- `medieval_military_metal_scale_skirt` - a metal scale armour skirt; noun: `skirt`; material: `mild steel`; size/quality: `Small`/`Standard`; weight/cost: 2100.0g/240.0m; components: `Holdable`, `Wear_Tassets`, `Armour_MetalScale`, `Destroyable_Armour`.
- `medieval_military_scale_spaulders` - a pair of scale spaulders; noun: `spaulders`; material: `mild steel`; size/quality: `Small`/`Standard`; weight/cost: 1150.0g/150.0m; components: `Holdable`, `Wear_Spaulders`, `Armour_MetalScale`, `Destroyable_Armour`.
- `medieval_military_scale_bracers` - a pair of scale bracers; noun: `bracers`; material: `mild steel`; size/quality: `Small`/`Standard`; weight/cost: 860.0g/110.0m; components: `Holdable`, `Wear_Bracers`, `Armour_MetalScale`, `Destroyable_Armour`.
- `medieval_military_scale_greaves` - a pair of scale greaves; noun: `greaves`; material: `mild steel`; size/quality: `Small`/`Standard`; weight/cost: 1350.0g/150.0m; components: `Holdable`, `Wear_Greaves`, `Armour_MetalScale`, `Destroyable_Armour`.
- `medieval_military_rawhide_lamellar_cuirass` - a rawhide lamellar cuirass; noun: `cuirass`; material: `rawhide`; size/quality: `Normal`/`Standard`; weight/cost: 4200.0g/260.0m; components: `Holdable`, `Wear_Cuirass`, `Armour_Lamellar`, `Destroyable_Armour`.
- `medieval_military_lacquered_lamellar_cuirass` - a lacquered leather lamellar cuirass; noun: `cuirass`; material: `leather`; size/quality: `Normal`/`Good`; weight/cost: 4800.0g/520.0m; components: `Holdable`, `Wear_Cuirass`, `Armour_Lamellar`, `Destroyable_Armour`.
- `medieval_military_iron_lamellar_cuirass` - an iron lamellar cuirass; noun: `cuirass`; material: `mild steel`; size/quality: `Normal`/`Good`; weight/cost: 6500.0g/900.0m; components: `Holdable`, `Wear_Cuirass`, `Armour_Lamellar`, `Destroyable_Armour`.
- `medieval_military_lamellar_spaulders` - a pair of lamellar shoulder guards; noun: `spaulders`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 1350.0g/210.0m; components: `Holdable`, `Wear_Spaulders`, `Armour_Lamellar`, `Destroyable_Armour`.
- `medieval_military_lamellar_bracers` - a pair of lamellar bracers; noun: `bracers`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 940.0g/150.0m; components: `Holdable`, `Wear_Bracers`, `Armour_Lamellar`, `Destroyable_Armour`.
- `medieval_military_lamellar_skirt` - a lamellar armour skirt; noun: `skirt`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 2400.0g/320.0m; components: `Holdable`, `Wear_Tassets`, `Armour_Lamellar`, `Destroyable_Armour`.
- `medieval_military_lamellar_cuisses` - a pair of lamellar thigh guards; noun: `cuisses`; material: `leather`; size/quality: `Normal`/`Good`; weight/cost: 1800.0g/260.0m; components: `Holdable`, `Wear_Cuisses`, `Armour_Lamellar`, `Destroyable_Armour`.
- `medieval_military_laminar_cuirass` - a banded laminar cuirass; noun: `cuirass`; material: `mild steel`; size/quality: `Normal`/`Good`; weight/cost: 5900.0g/720.0m; components: `Holdable`, `Wear_Cuirass`, `Armour_Laminar`, `Destroyable_Armour`.
- `medieval_military_laminar_arm_guards` - a pair of banded arm guards; noun: `vambraces`; material: `mild steel`; size/quality: `Small`/`Good`; weight/cost: 1050.0g/170.0m; components: `Holdable`, `Wear_Vambraces`, `Armour_Laminar`, `Destroyable_Armour`.
- `medieval_military_laminar_greaves` - a pair of banded greaves; noun: `greaves`; material: `mild steel`; size/quality: `Small`/`Good`; weight/cost: 1500.0g/210.0m; components: `Holdable`, `Wear_Greaves`, `Armour_Laminar`, `Destroyable_Armour`.
- `medieval_military_coat_of_plates` - a coat of plates; noun: `coat`; material: `mild steel`; size/quality: `Large`/`Good`; weight/cost: 7600.0g/1800.0m; components: `Holdable`, `Wear_Faulded_Cuirass`, `Armour_CoatOfPlates`, `Destroyable_Armour`.
- `medieval_military_plain_pair_of_plates` - a plain pair of plates; noun: `plates`; material: `mild steel`; size/quality: `Normal`/`Good`; weight/cost: 6200.0g/1500.0m; components: `Holdable`, `Wear_Cuirass`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_simple_breastplate` - a simple metal breastplate; noun: `breastplate`; material: `mild steel`; size/quality: `Normal`/`Good`; weight/cost: 3600.0g/960.0m; components: `Holdable`, `Wear_Breastplate`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_simple_backplate` - a simple metal backplate; noun: `backplate`; material: `mild steel`; size/quality: `Normal`/`Good`; weight/cost: 2900.0g/720.0m; components: `Holdable`, `Wear_Backplate`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_belly_plate` - a strapped belly plate; noun: `plackart`; material: `mild steel`; size/quality: `Small`/`Standard`; weight/cost: 1700.0g/300.0m; components: `Holdable`, `Wear_Plackart`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_laced_hip_plates` - a pair of laced hip plates; noun: `tassets`; material: `mild steel`; size/quality: `Small`/`Standard`; weight/cost: 1550.0g/280.0m; components: `Holdable`, `Wear_Tassets`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_early_metal_couters` - a pair of simple metal couters; noun: `couters`; material: `mild steel`; size/quality: `Small`/`Standard`; weight/cost: 720.0g/140.0m; components: `Holdable`, `Wear_Couters`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_early_poleyns` - a pair of simple poleyns; noun: `poleyns`; material: `mild steel`; size/quality: `Small`/`Standard`; weight/cost: 820.0g/150.0m; components: `Holdable`, `Wear_Poleyns`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_splinted_cuisses` - a pair of splinted cuisses; noun: `cuisses`; material: `leather`; size/quality: `Normal`/`Good`; weight/cost: 1700.0g/260.0m; components: `Holdable`, `Wear_Cuisses`, `Armour_Splinted`, `Destroyable_Armour`.
- `medieval_military_splinted_greaves` - a pair of splinted greaves; noun: `greaves`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 1550.0g/230.0m; components: `Holdable`, `Wear_Greaves`, `Armour_Splinted`, `Destroyable_Armour`.
- `medieval_military_guarded_gauntlets` - a pair of simple guarded gauntlets; noun: `gauntlets`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 760.0g/220.0m; components: `Holdable`, `Wear_Gauntlets`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_mail_backed_gauntlets` - a pair of mail-backed gauntlets; noun: `gauntlets`; material: `mild steel`; size/quality: `Small`/`Good`; weight/cost: 820.0g/210.0m; components: `Holdable`, `Wear_Gauntlets`, `Armour_Chainmail`, `Destroyable_Armour`.
- `medieval_military_leather_war_cap` - a hardened leather war cap; noun: `cap`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 620.0g/48.0m; components: `Holdable`, `Wear_Half_Helmet`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_iron_skullcap` - an iron skullcap; noun: `skullcap`; material: `wrought iron`; size/quality: `Small`/`Standard`; weight/cost: 1050.0g/120.0m; components: `Holdable`, `Wear_Half_Helmet`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_conical_steel_helm` - a conical steel helm; noun: `helm`; material: `mild steel`; size/quality: `Small`/`Standard`; weight/cost: 1350.0g/180.0m; components: `Holdable`, `Wear_Helmet`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_nasal_helm` - a nasal helm; noun: `helm`; material: `mild steel`; size/quality: `Small`/`Good`; weight/cost: 1550.0g/260.0m; components: `Holdable`, `Wear_Nasal_Helm`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_nasal_spangenhelm` - a nasal spangenhelm; noun: `spangenhelm`; material: `mild steel`; size/quality: `Small`/`Good`; weight/cost: 1650.0g/280.0m; components: `Holdable`, `Wear_Nasal_Spangenhelm`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_plain_spangenhelm` - a plain spangenhelm; noun: `spangenhelm`; material: `mild steel`; size/quality: `Small`/`Standard`; weight/cost: 1450.0g/210.0m; components: `Holdable`, `Wear_Spangenhelm`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_aventail_spangenhelm` - a spangenhelm with an aventail; noun: `spangenhelm`; material: `mild steel`; size/quality: `Small`/`Good`; weight/cost: 2600.0g/520.0m; components: `Holdable`, `Wear_Aventail_Spangenhelm`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_lamellar_helmet` - a lamellar helmet; noun: `helmet`; material: `mild steel`; size/quality: `Small`/`Good`; weight/cost: 1700.0g/340.0m; components: `Holdable`, `Wear_Helmet`, `Armour_Lamellar`, `Destroyable_Armour`.
- `medieval_military_kettle_style_helmet` - a broad-brimmed iron helmet; noun: `helmet`; material: `wrought iron`; size/quality: `Small`/`Standard`; weight/cost: 1850.0g/220.0m; components: `Holdable`, `Wear_Half_Helmet`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_flat_topped_greathelm` - a flat-topped great helm; noun: `greathelm`; material: `mild steel`; size/quality: `Normal`/`Good`; weight/cost: 2800.0g/760.0m; components: `Holdable`, `Wear_Greathelm`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_fine_greathelm` - a finely fitted great helm; noun: `greathelm`; material: `mild steel`; size/quality: `Normal`/`VeryGood`; weight/cost: 2650.0g/1250.0m; components: `Holdable`, `Wear_Greathelm`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_padded_gorget` - a padded throat guard; noun: `gorget`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 360.0g/16.0m; components: `Holdable`, `Wear_Gorget`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Minor`.
- `medieval_military_leather_gorget` - a hardened leather gorget; noun: `gorget`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 540.0g/56.0m; components: `Holdable`, `Wear_Gorget`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_iron_collar` - a simple iron collar; noun: `gorget`; material: `wrought iron`; size/quality: `Small`/`Good`; weight/cost: 780.0g/150.0m; components: `Holdable`, `Wear_Gorget`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_splinted_brassarts` - a pair of splinted brassarts; noun: `brassarts`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 1120.0g/170.0m; components: `Holdable`, `Wear_Brassarts`, `Armour_Splinted`, `Destroyable_Armour`.
- `medieval_military_lamellar_brassarts` - a pair of lamellar brassarts; noun: `brassarts`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 1040.0g/160.0m; components: `Holdable`, `Wear_Brassarts`, `Armour_Lamellar`, `Destroyable_Armour`.
- `medieval_military_plain_steel_vambraces` - a pair of plain steel vambraces; noun: `vambraces`; material: `mild steel`; size/quality: `Small`/`Good`; weight/cost: 1180.0g/240.0m; components: `Holdable`, `Wear_Vambraces`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_reinforced_riding_boots` - a pair of reinforced riding boots; noun: `boots`; material: `leather`; size/quality: `Normal`/`Good`; weight/cost: 1400.0g/120.0m; components: `Holdable`, `Wear_High_Boots`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_iron_greaves` - a pair of simple iron greaves; noun: `greaves`; material: `wrought iron`; size/quality: `Small`/`Good`; weight/cost: 1800.0g/320.0m; components: `Holdable`, `Wear_Greaves`, `Armour_RigidMetal`, `Destroyable_Armour`.
- `medieval_military_lamellar_greaves` - a pair of lamellar greaves; noun: `greaves`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 1350.0g/210.0m; components: `Holdable`, `Wear_Greaves`, `Armour_Lamellar`, `Destroyable_Armour`.
- `medieval_military_leather_poleyns` - a pair of hardened leather poleyns; noun: `poleyns`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 520.0g/56.0m; components: `Holdable`, `Wear_Poleyns`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_plain_surcoat` - a $colour plain surcoat; noun: `surcoat`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 650.0g/36.0m; components: `Holdable`, `Wear_Tabard`, `Armour_LightClothing`, `Destroyable_Clothing`, `Variable_BasicColour`.
- `medieval_military_bordered_surcoat` - a $colour bordered surcoat; noun: `surcoat`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 760.0g/96.0m; components: `Holdable`, `Wear_Tabard`, `Armour_LightClothing`, `Destroyable_Clothing`, `Variable_FineColour`.
- `medieval_military_weathered_armour_tabard` - a weathered armour tabard; noun: `tabard`; material: `canvas`; size/quality: `Normal`/`Substandard`; weight/cost: 720.0g/24.0m; components: `Holdable`, `Wear_Tabard`, `Armour_LightClothing`, `Destroyable_Clothing`.
- `medieval_military_linen_armour_cover` - a linen armour cover; noun: `cover`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 480.0g/18.0m; components: `Holdable`, `Wear_Tabard`, `Armour_LightClothing`, `Destroyable_Clothing`.
- `medieval_military_plain_war_saddle` - a plain war saddle; noun: `saddle`; material: `leather`; size/quality: `Large`/`Standard`; weight/cost: 5200.0g/220.0m; components: `Holdable`, `Wear_Saddle`, `Destroyable_Armour`.
- `medieval_military_high_backed_war_saddle` - a high-backed war saddle; noun: `saddle`; material: `leather`; size/quality: `Large`/`Good`; weight/cost: 6800.0g/420.0m; components: `Holdable`, `Wear_Saddle`, `Destroyable_Armour`.
- `medieval_military_plain_war_bridle` - a plain war bridle; noun: `bridle`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 780.0g/64.0m; components: `Holdable`, `Wear_Bridle`, `Destroyable_Armour`.
- `medieval_military_iron_curb_bit` - an iron curb bit; noun: `bit`; material: `wrought iron`; size/quality: `VerySmall`/`Standard`; weight/cost: 360.0g/36.0m; components: `Holdable`, `Wear_Bit`, `Destroyable_HeavyMetal`.
- `medieval_military_plain_caparison` - a $colour plain caparison; noun: `caparison`; material: `wool`; size/quality: `VeryLarge`/`Standard`; weight/cost: 2800.0g/120.0m; components: `Holdable`, `Wear_Caparison`, `Armour_LightClothing`, `Destroyable_Clothing`, `Variable_BasicColour`.
- `medieval_military_fine_livery_caparison` - a fine $colour caparison; noun: `caparison`; material: `wool`; size/quality: `VeryLarge`/`Good`; weight/cost: 3200.0g/300.0m; components: `Holdable`, `Wear_Caparison`, `Armour_LightClothing`, `Destroyable_Clothing`, `Variable_FineColour`.
- `medieval_military_padded_horse_cloth` - a padded horse cloth; noun: `caparison`; material: `canvas`; size/quality: `VeryLarge`/`Standard`; weight/cost: 5200.0g/260.0m; components: `Holdable`, `Wear_Caparison`, `Armour_Padded`, `Destroyable_Armour`, `Insulation_Balanced_Heavy`.
- `medieval_military_leather_chanfron` - a leather chanfron; noun: `chanfron`; material: `leather`; size/quality: `Normal`/`Standard`; weight/cost: 1600.0g/140.0m; components: `Holdable`, `Wear_Chanfron`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_scale_chanfron` - a metal scale chanfron; noun: `chanfron`; material: `mild steel`; size/quality: `Normal`/`Good`; weight/cost: 2600.0g/420.0m; components: `Holdable`, `Wear_Chanfron`, `Armour_MetalScale`, `Destroyable_Armour`.
- `medieval_military_lamellar_chanfron` - a lamellar chanfron; noun: `chanfron`; material: `leather`; size/quality: `Normal`/`Good`; weight/cost: 2300.0g/360.0m; components: `Holdable`, `Wear_Chanfron`, `Armour_Lamellar`, `Destroyable_Armour`.
- `medieval_military_mail_criniere` - a mail criniere; noun: `criniere`; material: `mild steel`; size/quality: `Large`/`Good`; weight/cost: 4200.0g/680.0m; components: `Holdable`, `Wear_Criniere`, `Armour_Chainmail`, `Destroyable_Armour`.
- `medieval_military_leather_criniere` - a leather criniere; noun: `criniere`; material: `leather`; size/quality: `Large`/`Standard`; weight/cost: 2500.0g/220.0m; components: `Holdable`, `Wear_Criniere`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_leather_peytral` - a leather peytral; noun: `peytral`; material: `leather`; size/quality: `Large`/`Standard`; weight/cost: 3000.0g/260.0m; components: `Holdable`, `Wear_Peytral`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_lamellar_peytral` - a lamellar peytral; noun: `peytral`; material: `leather`; size/quality: `Large`/`Good`; weight/cost: 4800.0g/760.0m; components: `Holdable`, `Wear_Peytral`, `Armour_Lamellar`, `Destroyable_Armour`.
- `medieval_military_mail_flanchards` - a pair of mail flanchards; noun: `flanchards`; material: `mild steel`; size/quality: `Large`/`Good`; weight/cost: 5200.0g/820.0m; components: `Holdable`, `Wear_Flanchards`, `Armour_Chainmail`, `Destroyable_Armour`.
- `medieval_military_leather_flanchards` - a pair of leather flanchards; noun: `flanchards`; material: `leather`; size/quality: `Large`/`Standard`; weight/cost: 3400.0g/340.0m; components: `Holdable`, `Wear_Flanchards`, `Armour_BoiledLeather`, `Destroyable_Armour`.
- `medieval_military_scale_croupiere` - a scale croupiere; noun: `croupiere`; material: `mild steel`; size/quality: `Large`/`Good`; weight/cost: 4300.0g/680.0m; components: `Holdable`, `Wear_Croupiere`, `Armour_MetalScale`, `Destroyable_Armour`.
- `medieval_military_lamellar_croupiere` - a lamellar croupiere; noun: `croupiere`; material: `leather`; size/quality: `Large`/`Good`; weight/cost: 3600.0g/540.0m; components: `Holdable`, `Wear_Croupiere`, `Armour_Lamellar`, `Destroyable_Armour`.

### Shields (65)

- `medieval_military_worn_round_shield` - a worn round shield; noun: `shield`; material: `linden`; size/quality: `Normal`/`Substandard`; weight/cost: 3200.0g/36.0m; components: `Holdable`, `Shield_Round`, `Destroyable_Shield`.
- `medieval_military_plain_round_shield` - a plain round shield; noun: `shield`; material: `poplar`; size/quality: `Normal`/`Standard`; weight/cost: 3000.0g/72.0m; components: `Holdable`, `Shield_Round`, `Destroyable_Shield`.
- `medieval_military_iron_bossed_round_shield` - an iron-bossed round shield; noun: `shield`; material: `linden`; size/quality: `Normal`/`Good`; weight/cost: 3400.0g/160.0m; components: `Holdable`, `Shield_Round`, `Destroyable_Shield`.
- `medieval_military_painted_round_shield` - a $colour painted round shield; noun: `shield`; material: `linden`; size/quality: `Normal`/`Standard`; weight/cost: 3200.0g/96.0m; components: `Holdable`, `Shield_Round`, `Destroyable_Shield`, `Variable_BasicColour`.
- `medieval_military_broad_war_round_shield` - a broad war round shield; noun: `shield`; material: `oak`; size/quality: `Normal`/`Good`; weight/cost: 4200.0g/180.0m; components: `Holdable`, `Shield_Round`, `Destroyable_Shield`.
- `medieval_military_light_cavalry_round_shield` - a light cavalry round shield; noun: `shield`; material: `willow`; size/quality: `Normal`/`Standard`; weight/cost: 2400.0g/90.0m; components: `Holdable`, `Shield_Round`, `Destroyable_Shield`.
- `medieval_military_leather_faced_round_shield` - a leather-faced round shield; noun: `shield`; material: `linden`; size/quality: `Normal`/`Standard`; weight/cost: 3600.0g/120.0m; components: `Holdable`, `Shield_Round`, `Destroyable_Shield`.
- `medieval_military_lacquered_round_shield` - a lacquered round shield; noun: `shield`; material: `wood`; size/quality: `Normal`/`Good`; weight/cost: 3100.0g/220.0m; components: `Holdable`, `Shield_Round`, `Destroyable_Shield`.
- `medieval_military_worn_kite_shield` - a worn kite shield; noun: `shield`; material: `poplar`; size/quality: `Large`/`Substandard`; weight/cost: 4800.0g/80.0m; components: `Holdable`, `Shield_Kite`, `Destroyable_Shield`.
- `medieval_military_plain_kite_shield` - a plain kite shield; noun: `shield`; material: `linden`; size/quality: `Large`/`Standard`; weight/cost: 5200.0g/180.0m; components: `Holdable`, `Shield_Kite`, `Destroyable_Shield`.
- `medieval_military_long_cavalry_kite_shield` - a long cavalry kite shield; noun: `shield`; material: `linden`; size/quality: `Large`/`Standard`; weight/cost: 5600.0g/240.0m; components: `Holdable`, `Shield_Kite`, `Destroyable_Shield`.
- `medieval_military_painted_kite_shield` - a $colour1 and $colour2 painted kite shield; noun: `shield`; material: `wood`; size/quality: `Large`/`Standard`; weight/cost: 5200.0g/220.0m; components: `Holdable`, `Shield_Kite`, `Destroyable_Shield`, `Variable_2BasicColour`.
- `medieval_military_leather_faced_kite_shield` - a leather-faced kite shield; noun: `shield`; material: `linden`; size/quality: `Large`/`Good`; weight/cost: 5900.0g/360.0m; components: `Holdable`, `Shield_Kite`, `Destroyable_Shield`.
- `medieval_military_fine_pointed_kite_shield` - a fine pointed kite shield; noun: `shield`; material: `linden`; size/quality: `Large`/`VeryGood`; weight/cost: 5400.0g/720.0m; components: `Holdable`, `Shield_Kite`, `Destroyable_Shield`, `Variable_2FineColour`.
- `medieval_military_compact_kite_shield` - a compact kite shield; noun: `shield`; material: `poplar`; size/quality: `Normal`/`Standard`; weight/cost: 4300.0g/160.0m; components: `Holdable`, `Shield_Kite`, `Destroyable_Shield`.
- `medieval_military_worn_heater_shield` - a worn heater shield; noun: `shield`; material: `poplar`; size/quality: `Normal`/`Substandard`; weight/cost: 3000.0g/90.0m; components: `Holdable`, `Shield_Heater`, `Destroyable_Shield`.
- `medieval_military_plain_heater_shield` - a plain heater shield; noun: `shield`; material: `linden`; size/quality: `Normal`/`Standard`; weight/cost: 3400.0g/220.0m; components: `Holdable`, `Shield_Heater`, `Destroyable_Shield`.
- `medieval_military_painted_heater_shield` - a $colour1 and $colour2 painted heater shield; noun: `shield`; material: `linden`; size/quality: `Normal`/`Good`; weight/cost: 3500.0g/360.0m; components: `Holdable`, `Shield_Heater`, `Destroyable_Shield`, `Variable_2BasicColour`.
- `medieval_military_fine_knightly_heater_shield` - a fine knightly heater shield; noun: `shield`; material: `linden`; size/quality: `Normal`/`VeryGood`; weight/cost: 3200.0g/800.0m; components: `Holdable`, `Shield_Heater`, `Destroyable_Shield`, `Variable_2FineColour`.
- `medieval_military_reinforced_heater_shield` - a reinforced heater shield; noun: `shield`; material: `oak`; size/quality: `Normal`/`Good`; weight/cost: 4100.0g/420.0m; components: `Holdable`, `Shield_Heater`, `Destroyable_Shield`.
- `medieval_military_compact_heater_shield` - a compact heater shield; noun: `shield`; material: `poplar`; size/quality: `Normal`/`Standard`; weight/cost: 2800.0g/180.0m; components: `Holdable`, `Shield_Heater`, `Destroyable_Shield`.
- `medieval_military_leather_covered_heater_shield` - a leather-covered heater shield; noun: `shield`; material: `linden`; size/quality: `Normal`/`Standard`; weight/cost: 3600.0g/260.0m; components: `Holdable`, `Shield_Heater`, `Destroyable_Shield`.
- `medieval_military_plain_wooden_buckler` - a plain wooden buckler; noun: `buckler`; material: `oak`; size/quality: `Small`/`Standard`; weight/cost: 900.0g/64.0m; components: `Holdable`, `Shield_Buckler`, `Destroyable_Shield`, `Beltable`.
- `medieval_military_iron_buckler` - an iron buckler; noun: `buckler`; material: `wrought iron`; size/quality: `Small`/`Standard`; weight/cost: 1250.0g/160.0m; components: `Holdable`, `Shield_Buckler`, `Destroyable_Shield`, `Beltable`.
- `medieval_military_steel_rimmed_buckler` - a steel-rimmed buckler; noun: `buckler`; material: `oak`; size/quality: `Small`/`Good`; weight/cost: 1100.0g/280.0m; components: `Holdable`, `Shield_Buckler`, `Destroyable_Shield`, `Beltable`.
- `medieval_military_leather_faced_buckler` - a $colour leather-faced buckler; noun: `buckler`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 850.0g/90.0m; components: `Holdable`, `Shield_Buckler`, `Destroyable_Shield`, `Beltable`, `Variable_BasicColour`.
- `medieval_military_worn_buckler` - a worn buckler; noun: `buckler`; material: `wood`; size/quality: `Small`/`Substandard`; weight/cost: 750.0g/24.0m; components: `Holdable`, `Shield_Buckler`, `Destroyable_Shield`, `Beltable`.
- `medieval_military_crossbowmans_pavise` - a $colour crossbowman's pavise; noun: `pavise`; material: `poplar`; size/quality: `Large`/`Standard`; weight/cost: 6500.0g/240.0m; components: `Holdable`, `Shield_Pavise`, `Destroyable_Shield`, `Variable_BasicColour`.
- `medieval_military_painted_standing_pavise` - a $colour1 and $colour2 painted standing pavise; noun: `pavise`; material: `linden`; size/quality: `VeryLarge`/`Good`; weight/cost: 9000.0g/480.0m; components: `Holdable`, `Shield_Pavise`, `Destroyable_Shield`, `Variable_2BasicColour`.
- `medieval_military_reinforced_siege_pavise` - a reinforced siege pavise; noun: `pavise`; material: `oak`; size/quality: `VeryLarge`/`Good`; weight/cost: 11000.0g/600.0m; components: `Holdable`, `Shield_Pavise`, `Destroyable_Shield`.
- `medieval_military_wicker_faced_pavise` - a wicker-faced pavise; noun: `pavise`; material: `wicker`; size/quality: `Large`/`Standard`; weight/cost: 5200.0g/180.0m; components: `Holdable`, `Shield_Pavise`, `Destroyable_Shield`.
- `medieval_military_campaign_pavise` - a plain campaign pavise; noun: `pavise`; material: `poplar`; size/quality: `Large`/`Standard`; weight/cost: 7200.0g/300.0m; components: `Holdable`, `Shield_Pavise`, `Destroyable_Shield`.
- `medieval_military_heavy_tower_shield` - a heavy tower shield; noun: `shield`; material: `oak`; size/quality: `VeryLarge`/`Standard`; weight/cost: 9000.0g/240.0m; components: `Holdable`, `Shield_Tower`, `Destroyable_Shield`.
- `medieval_military_plain_infantry_tower_shield` - a plain infantry tower shield; noun: `shield`; material: `poplar`; size/quality: `Large`/`Standard`; weight/cost: 7500.0g/180.0m; components: `Holdable`, `Shield_Tower`, `Destroyable_Shield`.
- `medieval_military_leather_faced_tower_shield` - a leather-faced tower shield; noun: `shield`; material: `oak`; size/quality: `VeryLarge`/`Good`; weight/cost: 8500.0g/360.0m; components: `Holdable`, `Shield_Tower`, `Destroyable_Shield`.
- `medieval_military_iron_rimmed_tower_shield` - an iron-rimmed tower shield; noun: `shield`; material: `oak`; size/quality: `VeryLarge`/`Good`; weight/cost: 10000.0g/460.0m; components: `Holdable`, `Shield_Tower`, `Destroyable_Shield`.
- `medieval_military_plain_rawhide_shield` - a plain rawhide shield; noun: `shield`; material: `rawhide`; size/quality: `Normal`/`Standard`; weight/cost: 2300.0g/48.0m; components: `Holdable`, `Shield_Hide`, `Destroyable_Shield`.
- `medieval_military_tall_rawhide_shield` - a tall rawhide shield; noun: `shield`; material: `rawhide`; size/quality: `Large`/`Standard`; weight/cost: 3600.0g/72.0m; components: `Holdable`, `Shield_Hide`, `Destroyable_Shield`.
- `medieval_military_thick_leather_hide_shield` - a thick leather hide shield; noun: `shield`; material: `cow leather`; size/quality: `Normal`/`Good`; weight/cost: 2900.0g/120.0m; components: `Holdable`, `Shield_Hide`, `Destroyable_Shield`.
- `medieval_military_light_horsemans_hide_shield` - a light horseman's hide shield; noun: `shield`; material: `rawhide`; size/quality: `Normal`/`Standard`; weight/cost: 1800.0g/60.0m; components: `Holdable`, `Shield_Hide`, `Destroyable_Shield`.
- `medieval_military_painted_hide_shield` - a $colour painted hide shield; noun: `shield`; material: `leather`; size/quality: `Normal`/`Good`; weight/cost: 2500.0g/150.0m; components: `Holdable`, `Shield_Hide`, `Destroyable_Shield`, `Variable_BasicColour`.
- `medieval_military_rugged_hide_shield` - a rugged hide shield; noun: `shield`; material: `rawhide`; size/quality: `Normal`/`Substandard`; weight/cost: 2000.0g/24.0m; components: `Holdable`, `Shield_Hide`, `Destroyable_Shield`.
- `medieval_military_plain_wicker_shield` - a plain wicker shield; noun: `shield`; material: `wicker`; size/quality: `Normal`/`Standard`; weight/cost: 1800.0g/32.0m; components: `Holdable`, `Shield_Wicker`, `Destroyable_Shield`.
- `medieval_military_cane_shield` - a woven cane shield; noun: `shield`; material: `cane`; size/quality: `Normal`/`Standard`; weight/cost: 1600.0g/28.0m; components: `Holdable`, `Shield_Wicker`, `Destroyable_Shield`.
- `medieval_military_rattan_shield` - a rattan shield; noun: `shield`; material: `rattan`; size/quality: `Normal`/`Good`; weight/cost: 1700.0g/64.0m; components: `Holdable`, `Shield_Wicker`, `Destroyable_Shield`.
- `medieval_military_reed_shield` - a rough reed shield; noun: `shield`; material: `reed`; size/quality: `Normal`/`Substandard`; weight/cost: 1300.0g/16.0m; components: `Holdable`, `Shield_Wicker`, `Destroyable_Shield`.
- `medieval_military_lacquered_wicker_shield` - a lacquered wicker shield; noun: `shield`; material: `wicker`; size/quality: `Normal`/`Good`; weight/cost: 1900.0g/90.0m; components: `Holdable`, `Shield_Wicker`, `Destroyable_Shield`.
- `medieval_military_plain_steel_dhal` - a plain steel dhal; noun: `dhal`; material: `mild steel`; size/quality: `Normal`/`Good`; weight/cost: 1900.0g/360.0m; components: `Holdable`, `Shield_Dhal`, `Destroyable_Shield`.
- `medieval_military_leather_dhal` - a leather dhal; noun: `dhal`; material: `cow leather`; size/quality: `Normal`/`Standard`; weight/cost: 1200.0g/120.0m; components: `Holdable`, `Shield_Dhal`, `Destroyable_Shield`.
- `medieval_military_four_boss_dhal` - a four-boss dhal; noun: `dhal`; material: `carbon steel`; size/quality: `Normal`/`Good`; weight/cost: 2000.0g/480.0m; components: `Holdable`, `Shield_Dhal`, `Destroyable_Shield`.
- `medieval_military_fine_polished_dhal` - a fine polished dhal; noun: `dhal`; material: `carbon steel`; size/quality: `Normal`/`VeryGood`; weight/cost: 1800.0g/900.0m; components: `Holdable`, `Shield_Dhal`, `Destroyable_Shield`.
- `medieval_military_rawhide_dhal` - a rawhide dhal; noun: `dhal`; material: `rawhide`; size/quality: `Normal`/`Standard`; weight/cost: 1000.0g/80.0m; components: `Holdable`, `Shield_Dhal`, `Destroyable_Shield`.
- `medieval_military_plain_adarga` - a plain adarga; noun: `adarga`; material: `rawhide`; size/quality: `Normal`/`Standard`; weight/cost: 1800.0g/120.0m; components: `Holdable`, `Shield_Adarga`, `Destroyable_Shield`.
- `medieval_military_heart_shaped_adarga` - a heart-shaped adarga; noun: `adarga`; material: `cow leather`; size/quality: `Normal`/`Good`; weight/cost: 1600.0g/220.0m; components: `Holdable`, `Shield_Adarga`, `Destroyable_Shield`.
- `medieval_military_fine_leather_adarga` - a fine $colour leather adarga; noun: `adarga`; material: `leather`; size/quality: `Normal`/`VeryGood`; weight/cost: 1400.0g/500.0m; components: `Holdable`, `Shield_Adarga`, `Destroyable_Shield`, `Variable_FineColour`.
- `medieval_military_cavalry_adarga` - a light cavalry adarga; noun: `adarga`; material: `rawhide`; size/quality: `Normal`/`Standard`; weight/cost: 1500.0g/160.0m; components: `Holdable`, `Shield_Adarga`, `Destroyable_Shield`.
- `medieval_military_painted_adarga` - a $colour painted adarga; noun: `adarga`; material: `leather`; size/quality: `Normal`/`Good`; weight/cost: 1650.0g/280.0m; components: `Holdable`, `Shield_Adarga`, `Destroyable_Shield`, `Variable_BasicColour`.
- `medieval_military_small_round_targe` - a small round targe; noun: `targe`; material: `oak`; size/quality: `Small`/`Standard`; weight/cost: 1800.0g/110.0m; components: `Holdable`, `Shield_Targe`, `Destroyable_Shield`, `Beltable`.
- `medieval_military_leather_faced_targe` - a leather-faced targe; noun: `targe`; material: `oak`; size/quality: `Small`/`Good`; weight/cost: 1600.0g/190.0m; components: `Holdable`, `Shield_Targe`, `Destroyable_Shield`, `Beltable`.
- `medieval_military_iron_bossed_targe` - an iron-bossed targe; noun: `targe`; material: `oak`; size/quality: `Small`/`Good`; weight/cost: 2100.0g/260.0m; components: `Holdable`, `Shield_Targe`, `Destroyable_Shield`, `Beltable`.
- `medieval_military_worn_small_targe` - a worn small targe; noun: `targe`; material: `wood`; size/quality: `Small`/`Substandard`; weight/cost: 1500.0g/36.0m; components: `Holdable`, `Shield_Targe`, `Destroyable_Shield`, `Beltable`.
- `medieval_military_plank_shield` - a crude plank shield; noun: `shield`; material: `wood`; size/quality: `Normal`/`Substandard`; weight/cost: 3500.0g/8.0m; components: `Holdable`, `Shield_Improvised`, `Destroyable_Shield`.
- `medieval_military_barrel_lid_shield` - a barrel-lid shield; noun: `shield`; material: `oak`; size/quality: `Normal`/`Substandard`; weight/cost: 2200.0g/6.0m; components: `Holdable`, `Shield_Improvised`, `Destroyable_Shield`.
- `medieval_military_pot_lid_shield` - a pot-lid shield; noun: `shield`; material: `wrought iron`; size/quality: `Small`/`Substandard`; weight/cost: 1400.0g/10.0m; components: `Holdable`, `Shield_Improvised`, `Destroyable_Shield`.
- `medieval_military_door_board_shield` - a door-board shield; noun: `shield`; material: `wood`; size/quality: `Large`/`Substandard`; weight/cost: 6500.0g/12.0m; components: `Holdable`, `Shield_Improvised`, `Destroyable_Shield`.

### Ranged weapons, ammunition, and thrown weapons (65)

- `medieval_military_worn_ash_shortbow` - a worn ash shortbow; noun: `shortbow`; material: `ash`; size/quality: `Normal`/`Substandard`; weight/cost: 680.0g/40.0m; components: `Holdable`, `Shortbow`, `Destroyable_Weapon`.
- `medieval_military_plain_elm_shortbow` - a plain elm shortbow; noun: `shortbow`; material: `elm`; size/quality: `Normal`/`Standard`; weight/cost: 720.0g/80.0m; components: `Holdable`, `Shortbow`, `Destroyable_Weapon`.
- `medieval_military_bamboo_shortbow` - a bamboo shortbow; noun: `shortbow`; material: `bamboo`; size/quality: `Normal`/`Standard`; weight/cost: 520.0g/92.0m; components: `Holdable`, `Shortbow`, `Destroyable_Weapon`.
- `medieval_military_recurved_riders_shortbow` - a recurved rider's shortbow; noun: `shortbow`; material: `horn`; size/quality: `Normal`/`Good`; weight/cost: 640.0g/180.0m; components: `Holdable`, `CompositeBow_Light`, `Destroyable_Weapon`.
- `medieval_military_sinew_backed_shortbow` - a sinew-backed shortbow; noun: `shortbow`; material: `yew`; size/quality: `Normal`/`Good`; weight/cost: 700.0g/210.0m; components: `Holdable`, `Shortbow`, `Destroyable_Weapon`.
- `medieval_military_fine_horn_recurve_bow` - a fine horn recurved bow; noun: `shortbow`; material: `horn`; size/quality: `Normal`/`VeryGood`; weight/cost: 620.0g/420.0m; components: `Holdable`, `CompositeBow_War`, `Destroyable_Weapon`.
- `medieval_military_rough_self_longbow` - a rough self longbow; noun: `longbow`; material: `ash`; size/quality: `Large`/`Substandard`; weight/cost: 860.0g/56.0m; components: `Holdable`, `Longbow`, `Destroyable_Weapon`.
- `medieval_military_elm_self_longbow` - an elm self longbow; noun: `longbow`; material: `elm`; size/quality: `Large`/`Standard`; weight/cost: 940.0g/110.0m; components: `Holdable`, `Longbow`, `Destroyable_Weapon`.
- `medieval_military_yew_longbow` - a yew longbow; noun: `longbow`; material: `yew`; size/quality: `Large`/`Good`; weight/cost: 900.0g/240.0m; components: `Holdable`, `Longbow`, `Destroyable_Weapon`.
- `medieval_military_heavy_war_longbow` - a heavy war longbow; noun: `longbow`; material: `yew`; size/quality: `Large`/`Good`; weight/cost: 1050.0g/320.0m; components: `Holdable`, `Longbow`, `Destroyable_Weapon`.
- `medieval_military_fine_horn_nocked_longbow` - a fine horn-nocked longbow; noun: `longbow`; material: `yew`; size/quality: `Large`/`VeryGood`; weight/cost: 980.0g/520.0m; components: `Holdable`, `Longbow`, `Destroyable_Weapon`.
- `medieval_military_worn_stirrup_crossbow` - a worn stirrup crossbow; noun: `crossbow`; material: `wood`; size/quality: `Normal`/`Substandard`; weight/cost: 2850.0g/160.0m; components: `Holdable`, `Crossbow`, `Destroyable_Weapon`.
- `medieval_military_plain_wooden_crossbow` - a plain wooden crossbow; noun: `crossbow`; material: `wood`; size/quality: `Normal`/`Standard`; weight/cost: 3100.0g/280.0m; components: `Holdable`, `Crossbow`, `Destroyable_Weapon`.
- `medieval_military_horn_lath_crossbow` - a horn-lathed crossbow; noun: `crossbow`; material: `horn`; size/quality: `Normal`/`Good`; weight/cost: 3350.0g/480.0m; components: `Holdable`, `Crossbow`, `Destroyable_Weapon`.
- `medieval_military_heavy_siege_crossbow` - a heavy siege crossbow; noun: `crossbow`; material: `wood`; size/quality: `Large`/`Good`; weight/cost: 5200.0g/640.0m; components: `Holdable`, `Crossbow`, `Destroyable_Weapon`.
- `medieval_military_fine_steel_fitted_crossbow` - a fine steel-fitted crossbow; noun: `crossbow`; material: `carbon steel`; size/quality: `Normal`/`VeryGood`; weight/cost: 3600.0g/900.0m; components: `Holdable`, `Crossbow`, `Destroyable_Weapon`.
- `medieval_military_hunting_crossbow` - a compact hunting crossbow; noun: `crossbow`; material: `wood`; size/quality: `Normal`/`Standard`; weight/cost: 2500.0g/300.0m; components: `Holdable`, `Crossbow`, `Destroyable_Weapon`.
- `medieval_military_small_latch_crossbow` - a small latch crossbow; noun: `crossbow`; material: `wood`; size/quality: `Small`/`Standard`; weight/cost: 1500.0g/180.0m; components: `Holdable`, `Hand Crossbow`, `Destroyable_Weapon`.
- `medieval_military_compact_hand_crossbow` - a compact hand crossbow; noun: `crossbow`; material: `wood`; size/quality: `Small`/`Good`; weight/cost: 1300.0g/320.0m; components: `Holdable`, `Hand Crossbow`, `Destroyable_Weapon`.
- `medieval_military_fine_hand_crossbow` - a fine hand crossbow; noun: `crossbow`; material: `horn`; size/quality: `Small`/`VeryGood`; weight/cost: 1200.0g/520.0m; components: `Holdable`, `Hand Crossbow`, `Destroyable_Weapon`.
- `medieval_military_braided_linen_sling` - a braided linen sling; noun: `sling`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 70.0g/4.0m; components: `Holdable`, `Sling`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_leather_pouch_sling` - a leather-pouch sling; noun: `sling`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 95.0g/10.0m; components: `Holdable`, `Sling`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_wool_braided_sling` - a wool-braided sling; noun: `sling`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 80.0g/5.0m; components: `Holdable`, `Sling`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_fine_cord_sling` - a fine cord sling; noun: `sling`; material: `hemp`; size/quality: `Small`/`Good`; weight/cost: 85.0g/16.0m; components: `Holdable`, `Sling`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_plain_staff_sling` - a plain staff sling; noun: `staff sling`; material: `ash`; size/quality: `Large`/`Standard`; weight/cost: 1250.0g/18.0m; components: `Holdable`, `Staff Sling`, `Destroyable_Weapon`.
- `medieval_military_heavy_oak_staff_sling` - a heavy oak staff sling; noun: `staff sling`; material: `oak`; size/quality: `Large`/`Good`; weight/cost: 1700.0g/32.0m; components: `Holdable`, `Staff Sling`, `Destroyable_Weapon`.
- `medieval_military_bamboo_staff_sling` - a bamboo staff sling; noun: `staff sling`; material: `bamboo`; size/quality: `Large`/`Standard`; weight/cost: 950.0g/24.0m; components: `Holdable`, `Staff Sling`, `Destroyable_Weapon`.
- `medieval_military_rough_field_point_arrow` - a rough field-point arrow; noun: `arrow`; material: `ash`; size/quality: `Small`/`Substandard`; weight/cost: 32.0g/1.0m; components: `Holdable`, `Ammo_FieldPointArrow`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_field_point_arrow` - a field-point arrow; noun: `arrow`; material: `ash`; size/quality: `Small`/`Standard`; weight/cost: 34.0g/2.0m; components: `Holdable`, `Ammo_FieldPointArrow`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_fine_field_point_arrow` - a fine field-point arrow; noun: `arrow`; material: `birch`; size/quality: `Small`/`Good`; weight/cost: 33.0g/4.0m; components: `Holdable`, `Ammo_FieldPointArrow`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_broadhead_arrow` - a broadhead arrow; noun: `arrow`; material: `ash`; size/quality: `Small`/`Standard`; weight/cost: 42.0g/3.0m; components: `Holdable`, `Ammo_BroadheadArrow`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_fine_broadhead_arrow` - a fine broadhead arrow; noun: `arrow`; material: `birch`; size/quality: `Small`/`Good`; weight/cost: 40.0g/6.0m; components: `Holdable`, `Ammo_BroadheadArrow`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_bodkin_arrow` - a bodkin arrow; noun: `arrow`; material: `ash`; size/quality: `Small`/`Standard`; weight/cost: 38.0g/4.0m; components: `Holdable`, `Ammo_BodkinArrow`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_tempered_bodkin_arrow` - a tempered bodkin arrow; noun: `arrow`; material: `birch`; size/quality: `Small`/`Good`; weight/cost: 39.0g/8.0m; components: `Holdable`, `Ammo_BodkinArrow`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_target_arrow` - a target arrow; noun: `arrow`; material: `poplar`; size/quality: `Small`/`Standard`; weight/cost: 30.0g/1.0m; components: `Holdable`, `Ammo_TargetArrow`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_padded_training_arrow` - a padded training arrow; noun: `arrow`; material: `ash`; size/quality: `Small`/`Standard`; weight/cost: 48.0g/3.0m; components: `Holdable`, `Ammo_PaddedArrow`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_rough_field_point_bolt` - a rough field-point bolt; noun: `bolt`; material: `ash`; size/quality: `Small`/`Substandard`; weight/cost: 46.0g/2.0m; components: `Holdable`, `Ammo_FieldPointBolt`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_field_point_bolt` - a field-point bolt; noun: `bolt`; material: `ash`; size/quality: `Small`/`Standard`; weight/cost: 50.0g/3.0m; components: `Holdable`, `Ammo_FieldPointBolt`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_broadhead_bolt` - a broadhead bolt; noun: `bolt`; material: `ash`; size/quality: `Small`/`Standard`; weight/cost: 58.0g/5.0m; components: `Holdable`, `Ammo_BroadheadBolt`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_bodkin_bolt` - a bodkin bolt; noun: `bolt`; material: `ash`; size/quality: `Small`/`Standard`; weight/cost: 56.0g/6.0m; components: `Holdable`, `Ammo_BodkinBolt`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_hardened_bodkin_bolt` - a hardened bodkin bolt; noun: `bolt`; material: `birch`; size/quality: `Small`/`Good`; weight/cost: 60.0g/10.0m; components: `Holdable`, `Ammo_BodkinBolt`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_target_bolt` - a target bolt; noun: `bolt`; material: `poplar`; size/quality: `Small`/`Standard`; weight/cost: 44.0g/2.0m; components: `Holdable`, `Ammo_TargetBolt`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_padded_training_bolt` - a padded training bolt; noun: `bolt`; material: `ash`; size/quality: `Small`/`Standard`; weight/cost: 70.0g/4.0m; components: `Holdable`, `Ammo_PaddedBolt`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_selected_sling_stone` - a selected sling stone; noun: `bullet`; material: `stone`; size/quality: `VerySmall`/`Standard`; weight/cost: 85.0g/1.0m; components: `Holdable`, `Ammo_SlingBullet`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_chipped_sling_stone` - a chipped sling stone; noun: `bullet`; material: `flint`; size/quality: `VerySmall`/`Standard`; weight/cost: 95.0g/1.0m; components: `Holdable`, `Ammo_SlingBullet`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_lead_sling_bullet` - a lead sling bullet; noun: `bullet`; material: `lead`; size/quality: `VerySmall`/`Standard`; weight/cost: 65.0g/3.0m; components: `Holdable`, `Ammo_LeadSlingBullet`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_fine_cast_lead_sling_bullet` - a fine cast lead sling bullet; noun: `bullet`; material: `lead`; size/quality: `VerySmall`/`Good`; weight/cost: 62.0g/5.0m; components: `Holdable`, `Ammo_LeadSlingBullet`, `Stack_Number`, `Destroyable_Weapon`.
- `medieval_military_worn_throwing_knife` - a worn throwing knife; noun: `knife`; material: `wrought iron`; size/quality: `Small`/`Substandard`; weight/cost: 180.0g/10.0m; components: `Holdable`, `Throwing_Knife`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_balanced_throwing_knife` - a balanced throwing knife; noun: `knife`; material: `carbon steel`; size/quality: `Small`/`Standard`; weight/cost: 210.0g/26.0m; components: `Holdable`, `Throwing_Knife`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_broad_throwing_knife` - a broad throwing knife; noun: `knife`; material: `carbon steel`; size/quality: `Small`/`Good`; weight/cost: 260.0g/52.0m; components: `Holdable`, `Throwing_Knife`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_fine_steel_throwing_knife` - a fine steel throwing knife; noun: `knife`; material: `carbon steel`; size/quality: `Small`/`VeryGood`; weight/cost: 220.0g/96.0m; components: `Holdable`, `Throwing_Knife`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_rough_throwing_hatchet` - a rough throwing hatchet; noun: `axe`; material: `wrought iron`; size/quality: `Small`/`Substandard`; weight/cost: 620.0g/22.0m; components: `Holdable`, `Throwing_Axe`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_balanced_throwing_axe` - a balanced throwing axe; noun: `axe`; material: `carbon steel`; size/quality: `Small`/`Standard`; weight/cost: 760.0g/48.0m; components: `Holdable`, `Throwing_Axe`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_bearded_throwing_axe` - a bearded throwing axe; noun: `axe`; material: `carbon steel`; size/quality: `Small`/`Good`; weight/cost: 820.0g/80.0m; components: `Holdable`, `Throwing_Axe`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_heavy_throwing_axe` - a heavy throwing axe; noun: `axe`; material: `carbon steel`; size/quality: `Normal`/`Standard`; weight/cost: 1050.0g/64.0m; components: `Holdable`, `Throwing_Axe`, `Destroyable_Weapon`.
- `medieval_military_fine_throwing_axe` - a fine throwing axe; noun: `axe`; material: `carbon steel`; size/quality: `Small`/`VeryGood`; weight/cost: 780.0g/140.0m; components: `Holdable`, `Throwing_Axe`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_light_riders_throwing_axe` - a light rider's throwing axe; noun: `axe`; material: `carbon steel`; size/quality: `Small`/`Good`; weight/cost: 650.0g/88.0m; components: `Holdable`, `Throwing_Axe`, `Destroyable_Weapon`, `Beltable`.
- `medieval_military_light_javelin` - a light javelin; noun: `javelin`; material: `ash`; size/quality: `Normal`/`Standard`; weight/cost: 850.0g/18.0m; components: `Holdable`, `Throwing_Spear`, `Destroyable_Weapon`.
- `medieval_military_rough_javelin` - a rough javelin; noun: `javelin`; material: `wrought iron`; size/quality: `Normal`/`Substandard`; weight/cost: 900.0g/10.0m; components: `Holdable`, `Throwing_Spear`, `Destroyable_Weapon`.
- `medieval_military_heavy_war_javelin` - a heavy war javelin; noun: `javelin`; material: `ash`; size/quality: `Normal`/`Good`; weight/cost: 1250.0g/42.0m; components: `Holdable`, `Throwing_Spear`, `Destroyable_Weapon`.
- `medieval_military_barbed_throwing_spear` - a barbed throwing spear; noun: `javelin`; material: `ash`; size/quality: `Normal`/`Standard`; weight/cost: 980.0g/34.0m; components: `Holdable`, `Throwing_Spear`, `Destroyable_Weapon`.
- `medieval_military_bamboo_war_dart` - a bamboo war dart; noun: `dart`; material: `bamboo`; size/quality: `Normal`/`Standard`; weight/cost: 520.0g/22.0m; components: `Holdable`, `Throwing_Spear`, `Destroyable_Weapon`.
- `medieval_military_horseman_throwing_spear` - a horseman's throwing spear; noun: `javelin`; material: `ash`; size/quality: `Normal`/`Good`; weight/cost: 950.0g/48.0m; components: `Holdable`, `Throwing_Spear`, `Destroyable_Weapon`.
- `medieval_military_fine_balanced_javelin` - a fine balanced javelin; noun: `javelin`; material: `ash`; size/quality: `Normal`/`VeryGood`; weight/cost: 880.0g/90.0m; components: `Holdable`, `Throwing_Spear`, `Destroyable_Weapon`.
- `medieval_military_short_throwing_spear` - a short throwing spear; noun: `spear`; material: `ash`; size/quality: `Normal`/`Standard`; weight/cost: 1100.0g/36.0m; components: `Holdable`, `Throwing_Spear`, `Destroyable_Weapon`.

### Carrying, storage, and support gear (69)

- `medieval_military_rough_rawhide_knife_sheath` - a rough rawhide knife sheath; noun: `sheath`; material: `rawhide`; size/quality: `Small`/`Substandard`; weight/cost: 70.0g/4.0m; components: `Holdable`, `Sheath_Small`, `Beltable`, `Destroyable_Clothing`.
- `medieval_military_plain_leather_knife_sheath` - a plain leather knife sheath; noun: `sheath`; material: `cow leather`; size/quality: `Small`/`Standard`; weight/cost: 90.0g/8.0m; components: `Holdable`, `Sheath_Small`, `Beltable`, `Destroyable_Clothing`.
- `medieval_military_tooled_leather_knife_sheath` - a $colour tooled leather knife sheath; noun: `sheath`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 110.0g/18.0m; components: `Holdable`, `Sheath_Small`, `Beltable`, `Destroyable_Clothing`, `Variable_BasicColour`.
- `medieval_military_broad_seax_sheath` - a broad seax sheath; noun: `sheath`; material: `leather`; size/quality: `Normal`/`Good`; weight/cost: 260.0g/36.0m; components: `Holdable`, `Sheath_Small`, `Beltable`, `Destroyable_Weapon`.
- `medieval_military_plain_dagger_sheath` - a plain dagger sheath; noun: `sheath`; material: `cow leather`; size/quality: `Small`/`Standard`; weight/cost: 140.0g/14.0m; components: `Holdable`, `Sheath_Small`, `Beltable`, `Destroyable_Clothing`.
- `medieval_military_reinforced_dagger_sheath` - a reinforced dagger sheath; noun: `sheath`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 220.0g/32.0m; components: `Holdable`, `Sheath_Small`, `Beltable`, `Destroyable_Weapon`.
- `medieval_military_fine_dyed_dagger_sheath` - a fine $colour dagger sheath; noun: `sheath`; material: `goat leather`; size/quality: `Small`/`VeryGood`; weight/cost: 210.0g/72.0m; components: `Holdable`, `Sheath_Small`, `Beltable`, `Destroyable_Weapon`, `Variable_FineColour`.
- `medieval_military_spearhead_rawhide_cover` - a rawhide spearhead cover; noun: `cover`; material: `rawhide`; size/quality: `Small`/`Standard`; weight/cost: 85.0g/5.0m; components: `Holdable`, `Sheath_Small`, `Destroyable_Clothing`.
- `medieval_military_worn_sword_scabbard` - a worn sword scabbard; noun: `scabbard`; material: `leather`; size/quality: `Normal`/`Substandard`; weight/cost: 620.0g/18.0m; components: `Holdable`, `Sheath`, `Beltable`, `Destroyable_Weapon`.
- `medieval_military_plain_sword_scabbard` - a plain sword scabbard; noun: `scabbard`; material: `leather`; size/quality: `Normal`/`Standard`; weight/cost: 700.0g/36.0m; components: `Holdable`, `Sheath`, `Beltable`, `Destroyable_Weapon`.
- `medieval_military_leather_bound_short_sword_scabbard` - a short sword scabbard; noun: `scabbard`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 520.0g/32.0m; components: `Holdable`, `Sheath`, `Beltable`, `Destroyable_Weapon`.
- `medieval_military_bronze_fitted_sword_scabbard` - a bronze-fitted sword scabbard; noun: `scabbard`; material: `leather`; size/quality: `Normal`/`Good`; weight/cost: 820.0g/96.0m; components: `Holdable`, `Sheath`, `Beltable`, `Destroyable_Weapon`.
- `medieval_military_fine_dyed_sword_scabbard` - a fine $colour sword scabbard; noun: `scabbard`; material: `goat leather`; size/quality: `Normal`/`VeryGood`; weight/cost: 780.0g/180.0m; components: `Holdable`, `Sheath`, `Beltable`, `Destroyable_Weapon`, `Variable_FineColour`.
- `medieval_military_long_sword_scabbard` - a long sword scabbard; noun: `scabbard`; material: `leather`; size/quality: `Large`/`Standard`; weight/cost: 1050.0g/72.0m; components: `Holdable`, `Sheath_Large`, `Beltable`, `Destroyable_Weapon`.
- `medieval_military_reinforced_large_weapon_sheath` - a reinforced large weapon sheath; noun: `sheath`; material: `cow leather`; size/quality: `Large`/`Good`; weight/cost: 1450.0g/120.0m; components: `Holdable`, `Sheath_Large`, `Beltable`, `Destroyable_Weapon`.
- `medieval_military_rawhide_axe_edge_cover` - a rawhide axe-edge cover; noun: `cover`; material: `rawhide`; size/quality: `Small`/`Standard`; weight/cost: 120.0g/6.0m; components: `Holdable`, `Sheath_Small`, `Destroyable_Clothing`.
- `medieval_military_rough_leather_arrow_quiver` - a rough leather arrow quiver; noun: `quiver`; material: `cow leather`; size/quality: `Normal`/`Substandard`; weight/cost: 520.0g/16.0m; components: `Holdable`, `Container_Quiver`, `Wear_Shoulder`, `Destroyable_Clothing`.
- `medieval_military_plain_shoulder_arrow_quiver` - a plain shoulder arrow quiver; noun: `quiver`; material: `leather`; size/quality: `Normal`/`Standard`; weight/cost: 680.0g/32.0m; components: `Holdable`, `Container_Quiver`, `Wear_Shoulder`, `Destroyable_Clothing`.
- `medieval_military_back_arrow_quiver` - a back-worn arrow quiver; noun: `quiver`; material: `leather`; size/quality: `Normal`/`Standard`; weight/cost: 760.0g/40.0m; components: `Holdable`, `Container_Quiver`, `Wear_Backpack`, `Destroyable_Clothing`.
- `medieval_military_riders_side_quiver` - a rider's side quiver; noun: `quiver`; material: `rawhide`; size/quality: `Normal`/`Good`; weight/cost: 820.0g/72.0m; components: `Holdable`, `Container_Quiver`, `Wear_Waist`, `Destroyable_Clothing`.
- `medieval_military_wicker_arrow_quiver` - a wicker arrow quiver; noun: `quiver`; material: `wicker`; size/quality: `Normal`/`Standard`; weight/cost: 430.0g/24.0m; components: `Holdable`, `Container_Quiver`, `Wear_Shoulder`, `Destroyable_Misc`.
- `medieval_military_lacquered_arrow_quiver` - a lacquered arrow quiver; noun: `quiver`; material: `lacquer`; size/quality: `Normal`/`VeryGood`; weight/cost: 720.0g/160.0m; components: `Holdable`, `Container_Quiver`, `Wear_Shoulder`, `Destroyable_Misc`.
- `medieval_military_broad_war_arrow_quiver` - a broad war-arrow quiver; noun: `quiver`; material: `cow leather`; size/quality: `Large`/`Good`; weight/cost: 980.0g/80.0m; components: `Holdable`, `Container_Quiver`, `Wear_Shoulder`, `Destroyable_Clothing`.
- `medieval_military_plain_bolt_case` - a plain bolt case; noun: `case`; material: `leather`; size/quality: `Normal`/`Standard`; weight/cost: 620.0g/36.0m; components: `Holdable`, `Container_Quiver`, `Wear_Shoulder`, `Destroyable_Clothing`.
- `medieval_military_belt_bolt_case` - a belt-hung bolt case; noun: `case`; material: `cow leather`; size/quality: `Small`/`Standard`; weight/cost: 440.0g/28.0m; components: `Holdable`, `Container_Quiver`, `Beltable`, `Destroyable_Clothing`.
- `medieval_military_wooden_crossbow_bolt_box` - a wooden crossbow bolt box; noun: `box`; material: `oak`; size/quality: `Normal`/`Good`; weight/cost: 1250.0g/64.0m; components: `Holdable`, `Container_Quiver`, `Wear_Shoulder`, `Destroyable_WoodenHeavy`.
- `medieval_military_fine_bolt_case` - a fine bolt case; noun: `case`; material: `goat leather`; size/quality: `Normal`/`VeryGood`; weight/cost: 720.0g/120.0m; components: `Holdable`, `Container_Quiver`, `Wear_Shoulder`, `Destroyable_Clothing`.
- `medieval_military_sling_stone_pouch` - a sling-stone pouch; noun: `pouch`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 180.0g/8.0m; components: `Holdable`, `Container_Pouch`, `Beltable`, `Destroyable_Clothing`.
- `medieval_military_lead_bullet_pouch` - a lead bullet pouch; noun: `pouch`; material: `cow leather`; size/quality: `Small`/`Good`; weight/cost: 240.0g/16.0m; components: `Holdable`, `Container_Pouch`, `Beltable`, `Destroyable_Clothing`.
- `medieval_military_staff_sling_ammunition_satchel` - a staff-sling ammunition satchel; noun: `satchel`; material: `canvas`; size/quality: `Normal`/`Standard`; weight/cost: 540.0g/20.0m; components: `Holdable`, `Container_Pack`, `Wear_Shoulder`, `Destroyable_Clothing`.
- `medieval_military_plain_bowcase` - a plain leather bowcase; noun: `bowcase`; material: `leather`; size/quality: `Large`/`Standard`; weight/cost: 720.0g/36.0m; components: `Holdable`, `Container_Pack`, `Wear_Shoulder`, `Destroyable_Clothing`.
- `medieval_military_riders_short_bowcase` - a rider's short bowcase; noun: `bowcase`; material: `rawhide`; size/quality: `Normal`/`Good`; weight/cost: 680.0g/72.0m; components: `Holdable`, `Container_Pack`, `Wear_Waist`, `Destroyable_Clothing`.
- `medieval_military_canvas_longbow_sleeve` - a canvas longbow sleeve; noun: `sleeve`; material: `canvas`; size/quality: `Large`/`Standard`; weight/cost: 360.0g/18.0m; components: `Holdable`, `Container_Pack`, `Wear_Shoulder`, `Destroyable_Clothing`.
- `medieval_military_crossbow_carrying_case` - a crossbow carrying case; noun: `case`; material: `cow leather`; size/quality: `Large`/`Good`; weight/cost: 1500.0g/96.0m; components: `Holdable`, `Container_Pack`, `Wear_Shoulder`, `Destroyable_Clothing`.
- `medieval_military_fine_lacquered_bowcase` - a fine lacquered bowcase; noun: `bowcase`; material: `lacquer`; size/quality: `Normal`/`VeryGood`; weight/cost: 760.0g/180.0m; components: `Holdable`, `Container_Pack`, `Wear_Waist`, `Destroyable_Misc`.
- `medieval_military_round_shield_cover` - a round shield cover; noun: `cover`; material: `canvas`; size/quality: `Normal`/`Standard`; weight/cost: 260.0g/12.0m; components: `Holdable`, `Destroyable_Clothing`.
- `medieval_military_kite_shield_cover` - a kite shield cover; noun: `cover`; material: `linen`; size/quality: `Large`/`Standard`; weight/cost: 420.0g/20.0m; components: `Holdable`, `Destroyable_Clothing`.
- `medieval_military_heater_shield_cover` - a $colour heater shield cover; noun: `cover`; material: `canvas`; size/quality: `Normal`/`Good`; weight/cost: 320.0g/28.0m; components: `Holdable`, `Destroyable_Clothing`, `Variable_BasicColour`.
- `medieval_military_pavise_canvas_cover` - a pavise canvas cover; noun: `cover`; material: `canvas`; size/quality: `Large`/`Good`; weight/cost: 780.0g/48.0m; components: `Holdable`, `Destroyable_Clothing`.
- `medieval_military_buckler_pouch` - a buckler carrying pouch; noun: `pouch`; material: `leather`; size/quality: `Normal`/`Standard`; weight/cost: 420.0g/24.0m; components: `Holdable`, `Container_Pouch`, `Wear_Shoulder`, `Destroyable_Clothing`.
- `medieval_military_plain_arming_belt` - a plain arming belt; noun: `belt`; material: `cow leather`; size/quality: `Small`/`Standard`; weight/cost: 380.0g/24.0m; components: `Holdable`, `Wear_Waist`, `Belt_2`, `Destroyable_Clothing`.
- `medieval_military_broad_weapon_belt` - a broad weapon belt; noun: `belt`; material: `cow leather`; size/quality: `Small`/`Good`; weight/cost: 560.0g/48.0m; components: `Holdable`, `Wear_Waist`, `Belt_4`, `Destroyable_Clothing`.
- `medieval_military_heavy_war_belt` - a heavy war belt; noun: `belt`; material: `cow leather`; size/quality: `Normal`/`Good`; weight/cost: 820.0g/84.0m; components: `Holdable`, `Wear_Waist`, `Belt_6`, `Destroyable_Clothing`.
- `medieval_military_simple_sword_belt` - a simple sword belt; noun: `belt`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 420.0g/32.0m; components: `Holdable`, `Wear_Waist`, `Belt_2`, `Destroyable_Clothing`.
- `medieval_military_double_hanger_sword_belt` - a double-hanger sword belt; noun: `belt`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 520.0g/64.0m; components: `Holdable`, `Wear_Waist`, `Belt_4`, `Destroyable_Clothing`.
- `medieval_military_riding_weapon_belt` - a riding weapon belt; noun: `belt`; material: `cow leather`; size/quality: `Small`/`Good`; weight/cost: 620.0g/72.0m; components: `Holdable`, `Wear_Waist`, `Belt_4`, `Destroyable_Clothing`.
- `medieval_military_leather_baldric` - a leather weapon baldric; noun: `baldric`; material: `leather`; size/quality: `Normal`/`Standard`; weight/cost: 480.0g/36.0m; components: `Holdable`, `Wear_Sash`, `Belt_2`, `Destroyable_Clothing`.
- `medieval_military_broad_weapon_baldric` - a broad weapon baldric; noun: `baldric`; material: `cow leather`; size/quality: `Normal`/`Good`; weight/cost: 720.0g/72.0m; components: `Holdable`, `Wear_Sash`, `Belt_4`, `Destroyable_Clothing`.
- `medieval_military_archers_utility_belt` - an archer's utility belt; noun: `belt`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 460.0g/40.0m; components: `Holdable`, `Wear_Waist`, `Belt_4`, `Destroyable_Clothing`.
- `medieval_military_fine_sword_belt` - a fine $colour sword belt; noun: `belt`; material: `goat leather`; size/quality: `Small`/`VeryGood`; weight/cost: 500.0g/140.0m; components: `Holdable`, `Wear_Waist`, `Belt_4`, `Destroyable_Clothing`, `Variable_FineColour`.
- `medieval_military_mounted_weapon_harness` - a mounted weapon harness; noun: `harness`; material: `cow leather`; size/quality: `Normal`/`Good`; weight/cost: 980.0g/120.0m; components: `Holdable`, `Wear_Bandolier`, `Belt_6`, `Destroyable_Clothing`.
- `medieval_military_wall_spear_rack` - a wall-mounted spear rack; noun: `rack`; material: `oak`; size/quality: `VeryLarge`/`Standard`; weight/cost: 18000.0g/96.0m; components: `Container_Weapon_Rack`, `Destroyable_WoodenHeavy`.
- `medieval_military_simple_weapon_rack` - a simple weapon rack; noun: `rack`; material: `pine`; size/quality: `VeryLarge`/`Standard`; weight/cost: 22000.0g/120.0m; components: `Container_Weapon_Rack`, `Destroyable_WoodenHeavy`.
- `medieval_military_sword_rack` - a sword rack; noun: `rack`; material: `oak`; size/quality: `Large`/`Good`; weight/cost: 14000.0g/110.0m; components: `Container_Weapon_Rack`, `Destroyable_WoodenHeavy`.
- `medieval_military_bow_rack` - a bow rack; noun: `rack`; material: `ash`; size/quality: `VeryLarge`/`Standard`; weight/cost: 16000.0g/100.0m; components: `Container_Weapon_Rack`, `Destroyable_WoodenHeavy`.
- `medieval_military_crossbow_rack` - a crossbow rack; noun: `rack`; material: `oak`; size/quality: `VeryLarge`/`Good`; weight/cost: 28000.0g/180.0m; components: `Container_Weapon_Rack`, `Destroyable_WoodenHeavy`.
- `medieval_military_polearm_rack` - a polearm rack; noun: `rack`; material: `oak`; size/quality: `VeryLarge`/`Good`; weight/cost: 34000.0g/200.0m; components: `Container_Weapon_Rack`, `Destroyable_WoodenHeavy`.
- `medieval_military_portable_campaign_weapon_rack` - a portable campaign weapon rack; noun: `rack`; material: `ash`; size/quality: `Large`/`Standard`; weight/cost: 9500.0g/88.0m; components: `Holdable`, `Container_Weapon_Rack`, `Destroyable_WoodenHeavy`.
- `medieval_military_iron_armoury_rack` - an iron armoury rack; noun: `rack`; material: `wrought iron`; size/quality: `VeryLarge`/`Good`; weight/cost: 42000.0g/360.0m; components: `Container_Weapon_Rack`, `Destroyable_HeavyMetal`.
- `medieval_military_simple_armour_stand` - a simple armour stand; noun: `stand`; material: `pine`; size/quality: `Large`/`Standard`; weight/cost: 12000.0g/80.0m; components: `Container_Armor_Stand`, `Destroyable_WoodenHeavy`.
- `medieval_military_sturdy_armour_tree` - a sturdy armour tree; noun: `stand`; material: `oak`; size/quality: `VeryLarge`/`Good`; weight/cost: 24000.0g/160.0m; components: `Container_Armor_Stand`, `Destroyable_WoodenHeavy`.
- `medieval_military_mail_armour_stand` - a mail armour stand; noun: `stand`; material: `oak`; size/quality: `VeryLarge`/`Good`; weight/cost: 26000.0g/170.0m; components: `Container_Armor_Stand`, `Destroyable_WoodenHeavy`.
- `medieval_military_mounted_harness_stand` - a mounted harness stand; noun: `stand`; material: `oak`; size/quality: `VeryLarge`/`Good`; weight/cost: 36000.0g/240.0m; components: `Container_Armor_Stand`, `Destroyable_WoodenHeavy`.
- `medieval_military_barding_trestle` - a barding trestle; noun: `trestle`; material: `oak`; size/quality: `VeryLarge`/`Good`; weight/cost: 32000.0g/220.0m; components: `Container_Armor_Stand`, `Destroyable_WoodenHeavy`.
- `medieval_military_fine_display_armour_stand` - a fine armour display stand; noun: `stand`; material: `walnut`; size/quality: `VeryLarge`/`VeryGood`; weight/cost: 28000.0g/480.0m; components: `Container_Armor_Stand`, `Destroyable_WoodenHeavy`.
- `medieval_military_bundle_of_hemp_bowstrings` - a bundle of hemp bowstrings; noun: `bowstring`; material: `hemp`; size/quality: `VerySmall`/`Standard`; weight/cost: 45.0g/6.0m; components: `Holdable`, `Stack_Number`, `Destroyable_Misc`.
- `medieval_military_bundle_of_linen_bowstrings` - a bundle of linen bowstrings; noun: `bowstring`; material: `linen`; size/quality: `VerySmall`/`Good`; weight/cost: 40.0g/12.0m; components: `Holdable`, `Stack_Number`, `Destroyable_Misc`.
- `medieval_military_sinew_bowstring_bundle` - a bundle of sinew bowstrings; noun: `bowstring`; material: `sinew`; size/quality: `VerySmall`/`Good`; weight/cost: 42.0g/18.0m; components: `Holdable`, `Stack_Number`, `Destroyable_Misc`.
- `medieval_military_spare_crossbow_string` - a spare crossbow string; noun: `string`; material: `hemp`; size/quality: `VerySmall`/`Good`; weight/cost: 85.0g/20.0m; components: `Holdable`, `Destroyable_Misc`.

---

## Validation result

The implemented catalogue was authored and checked against these validation rules and outcomes:

- No hidden or non-skinnable ordinary finished goods unless explicitly approved.
- Ordinary portable items include `Holdable` unless a component or fixture use-case requires otherwise.
- Weapons use exactly one weapon component unless an explicitly permitted combination exists.
- Ammunition uses the exact seeded ammunition component matching its type, including bodkin arrows and bodkin bolts where appropriate, and normally includes `Stack_Number` as its stackable component.
- Armour uses exactly one wearable coverage component and exactly one armour component.
- Shields use exactly one shield component, use `Destroyable_Shield`, and do not stack unsupported weapon components.
- Horse tack and barding use exact horse wear components and do not imply unsupported storage or mount-control mechanics.
- All primary materials are exact seeded solid material names.
- Liquids and gases are not used as solid primary materials.
- All tags are exact seeded hierarchical tag paths.
- All components are exact seeded component prototype names.
- Public text avoids seeder mechanics, skin/customization language, builder-facing labels, and unsupported behaviour claims.
- Public text generally avoids culture names unless the object-form name is useful and player-facing.
- The catalogue covers many bodyparts, including head, face/neck, torso, shoulders, arms, elbows, forearms, hands, hips, thighs, knees, shins, and feet.
- The catalogue covers horse tack and period-appropriate horse protection: saddle, bridle, bit, caparison, chanfron, criniere, peytral, flanchards, and croupiere.
- The catalogue covers commoner, militia, professional, mounted, and elite armour.
- The catalogue covers every medieval-useful weapon family supported by components.
- Major weapon families include meaningful quality variation.
- Shields receive stronger culture/regional presentation than ordinary weapon forms.
- Training dummies, pell posts, quintains, archery targets, humanoid practice targets, and similar drill fixtures are excluded from this branch.
- Explicitly fantasy, modern, blackpowder, and mainly post-1300 equipment is excluded or deferred out of this branch.
- Outfit/loadout templates can now be mapped to exact catalogue entries from the implemented item list.
- Mechanically distinct component gaps are recorded as project feedback only and are not treated as usable component names until seeded.

---

## Source notes

### Project sources

- `Medieval Military Seeder Design Reference.md` — used as the starting draft and structural source. It retained many clothing-guide remnants, including a clothing title, clothing catalogue lines, and religious/urban clothing validations. This revised draft replaces those with military-goods rules while preserving the useful seeder structure.
- `FutureMUD Item Seeder Working Guidelines.md` — used for exact seeder conventions, material/component/tag grounding, `Holdable` convention, cost units, source-of-truth policy, and component exclusivity cautions.
- `Item Authoring Guidelines.md` — used for item field expectations, description rules, size/quality categories, skins, writing blocks, and general item-authoring concepts.
- `Seeded_Item_Components.json` — used for exact component names, including the newly available `Destroyable_Shield`, `Wear_Tassets`, `Ammo_BodkinArrow`, `Ammo_BodkinBolt`, and horse-tack/barding wearable components.
- `Item_Component_Types.json` — used for component behaviour and exclusive-interface checks.
- `Seeded_Materials.json` — used for exact solid material names.
- `SeededTagHierarchy.csv` — used for exact hierarchical tags. The file is tab-separated despite its `.csv` extension.
- `Medieval_Clothing_Seeder_Design_Reference.md` and `FutureMUD Antiquity Clothing Seeder Design Reference.md` — used as structural precedent for scope, culture-neutral public text, shared-item policy, skin policy, and validation format.
- `DatabaseSeeder/Seeders/ItemSeeder.MedievalWeapons.cs` and `DatabaseSeeder/Seeders/ItemSeeder.MedievalArmour.cs` — live implementation of all completed first-wave catalogue passes.
- Completed pass files: `medieval_military_melee_weapons_pass_1.cs`, `medieval_military_armour_sets_pass_1.cs`, `medieval_military_shields_pass_1.cs`, `medieval_military_ranged_ammunition_thrown_pass_1.cs`, and `medieval_military_carrying_storage_support_pass_1.cs`.

### Historical reference sources used for design principles

- Metropolitan Museum of Art, `Arms and Armor in Medieval Europe` — used for broad early-medieval to high-medieval western equipment trajectories: spangenhelm, mail, scale, shields, spears, swords, axes, bows, knightly mail, kite/heater-like shield development, and the later shift toward plate.
- Metropolitan Museum of Art, `The Function of Armor in Medieval and Renaissance Europe` — used for mail dominance, padded underlayers, material-composite armour, and the importance of layering.
- Royal Armouries, `Hundred Years War: Armour and Weapons` — used to distinguish strict-period equipment from mainly post-1300 armour pieces, especially many familiar plate components.
- National Museum of Denmark, `Viking weapons` — used for Viking Age weapon mix, the social cost distinction between swords and more common weapons, and rarity of helmets/mail.
- Metropolitan Museum of Art, `Islamic Arms and Armor` — used for Islamic and eastern Mediterranean/steppe-facing principles: lighter armour assumptions, mail continuity, round shields, recurve bows, maces, axes, javelins, and elite decorative skin policy.
- Metropolitan Museum of Art, `The Art of the Samurai: Japanese Arms and Armor, 1156-1868` — used for Japanese military material-culture coverage, including armour, swords, archery equipment, equestrian equipment, banners, surcoats, and rank/display items.
- Metropolitan Museum of Art, `Fashion in European Armor` — used for the principle that armour form, decoration, and regional fashion change over time and are therefore good skin or regional-override targets.

---

## Resolved project-owner decisions applied in this revision

1. **Strict period scope**

   The catalogue should stick to the time periods from the available cultures. Explicitly fantasy items and mainly post-1300 equipment are deferred to a separate fantasy or later-period seeder.

2. **Shield destroyability**

   `Destroyable_Shield` is now available and should be the standard destroyable component for shields.

3. **Tassets component**

   `Wear_Tassets` is now available. It should be used only for period-plausible hip/skirt defences inside the 500-1300 branch, not for mature later plate tassets.

4. **Bodkin ammunition**

   `Ammo_BodkinArrow` and `Ammo_BodkinBolt` are now available and should be included in strict medieval ammunition coverage.

5. **Medieval equipment stock tags**

   No additional medieval equipment-stock tag decision is needed for this design pass. Existing exact tags are documented only as available project vocabulary.

6. **Horse armour and barding**

   Horse tack, horse armour, and barding are in scope for this branch. The design includes `Wear_Saddle`, `Wear_Bridle`, `Wear_Bit`, `Wear_Chanfron`, `Wear_Criniere`, `Wear_Croupiere`, `Wear_Flanchards`, `Wear_Peytral`, and `Wear_Caparison`.

7. **Craft tools**

   Craft tools are left out of the first military-goods catalogue wave. Tool tags remain documented only for a later crafting-support pass.

8. **Ammunition stackability**

   Ammunition should include a stackable component. `Stack_Number` is the default for discrete ammunition; `Stack_Pile` is reserved for intentionally imprecise loose piles.

9. **Support-item component documentation**

   Sheaths, scabbards, quivers, bolt cases, weapon racks, armour stands, military belts, and hangers now have explicit component-combination rules. The rules distinguish portable goods using `Holdable` from installed racks and stands that should normally omit `Holdable` and use an `ldesc`.

10. **Training dummies excluded**

   Training weapons remain in scope, but training dummies, pell posts, quintains, archery targets, humanoid practice targets, and other drill fixtures are excluded from this branch.

11. **Component-gap feedback**

   The proposed data profiles are now seeded and used by the relevant existing catalogue rows. The remaining mounted/couched charge and hook/pull/trip/anti-rider behaviours are tracked in the consolidated engine dependency ledger.

12. **Material additions resolved**

   The previously requested materials are now present in `Seeded_Materials.json`: `rawhide`, `sinew`, `rattan`, `wicker`, `reed`, `cane`, `lacquer`, `horsehair`, `goat leather`, and `sheep leather`. The implemented catalogue uses these exact materials where they are the dominant primary material.

13. **First-wave catalogue implemented**

   The first-wave catalogue is implemented as 381 `CreateItem(...)` calls split between `ItemSeeder.MedievalWeapons.cs` and `ItemSeeder.MedievalArmour.cs`.

### Remaining engine improvements

No material or item-component data blockers remain for the first-wave catalogue. Dedicated mounted/couched charge and hook/pull/trip/anti-rider behaviour remains deferred; the current profiles expose only attacks the engine can execute honestly.
