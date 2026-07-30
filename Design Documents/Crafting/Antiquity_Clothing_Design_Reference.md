# FutureMUD Antiquity Clothing Seeder Design Reference

This document consolidates the accepted antiquity clothing guidance and outcomes for the FutureMUD Item Seeder. It covers the European antiquity phase and the later Mediterranean, Near Eastern, and North African antiquity expansion, presenting the shared rules, assumptions, set designs, and item catalogues in one stable reference.

## Executive summary

- Total accepted unique wearable items: **98**.
- Total outfit manifests: **29**.
- All accepted items are finished goods, skinnable, player-visible, and ordinary portable inventory items.
- Public item fields use culture-neutral, in-world descriptions. Builder-facing notes and outfit labels may still use historical inspiration labels for organization and source grounding.
- The item set is intended as a historically grounded base layer: skins may create local, fantasy, status, textile, and colour variants without duplicating the underlying behaviour-heavy item prototypes.

## Scope and era model

- Chronological band: broad antiquity, spanning early Iron Age clothing forms through classical and imperial-era Mediterranean forms where they are useful for a general seeder baseline.
- Geographic coverage in this consolidated document: European antiquity; the eastern Mediterranean; the Levant and Punic sphere; Achaemenid/Median-Persian inspired clothing; and ancient Egyptian inspired linen clothing.
- Historical inspiration families: Celtic, Germanic, Italic/Roman, Hellenic, Phoenician/Punic, Persian, and Egyptian. These labels are builder-facing organizational buckets, not required public item wording.
- Resolution: standard garment prototypes rather than exhaustive local subtypes. The defaults should be credible on their own, while later skins can handle exact regional trim, textile patterns, motifs, dyes, household marks, and world-specific names.
- Coverage deliberately leaves later medieval, early modern, modern, American, and many non-European/non-Mediterranean traditions for later passes.

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
- No accepted antiquity clothing item uses a morph target, morph emote, morph timer, long description, destroyed-item reference, hidden-player flag, or non-skinnable flag.

## Wearable implementation rules

- Every clothing item should have one wearable coverage component, such as `Wear_Tunic`, `Wear_Mantle`, `Wear_Trousers`, `Wear_Sandals`, or `Wear_Head` as appropriate.
- Ordinary portable wearables include `Holdable` so they can be picked up, moved, carried, stored, and otherwise handled as inventory objects.
- Clothing uses a destroyable component. Most textile/leather clothing uses `Destroyable_Clothing`; glass jewellery uses an appropriate glassware destroyable component.
- Wearables include a clothing insulation component and an armour component. These are not meant to make ordinary clothes battlefield armour; `Armour_LightClothing` and `Armour_HeavyClothing` represent ordinary protective clothing behaviour.
- Belts that should support attached items can include a belt component, such as `Belt_2`, in addition to the wearable waist component. Cloth sashes and girdles do not automatically become functional belts unless intentionally implemented that way.
- Component-dependent claims must be avoided unless the component exists. Descriptions may mention visible straps, hems, seams, folds, pins, or ties, but must not promise hidden storage, locks, identity concealment, or transformations without components.

## Player-facing description rules

- Do not mention skins, base prototypes, standard implementations, seeder mechanics, builders, later customization, or archaeological uncertainty in `noun`, `sdesc`, `ldesc`, or `fdesc`.
- Full descriptions must read as objects in the world. Good wording explains how the garment is cut, wrapped, belted, draped, folded, tied, layered, or worn.
- Public item text avoids real-world culture labels such as Celtic, Germanic, Roman, Greek, Hellenic, Phoenician, Punic, Persian, or Egyptian. These labels can be used in builder-facing manifests and source notes.
- Garment names are acceptable when they identify a form rather than merely naming a people. Examples used here include `braccae`, `tunica`, `toga`, `stola`, `palla`, `chiton`, `peplos`, `himation`, `chlamys`, `sarapis`, `anaxyrides`, `kandys`, `kyrbasia`, `shendyt`, `kalasiris`, and `usekh`.
- Where no useful garment name exists, public naming should use visible descriptors: short, long, straight, fitted, full, folded, bordered, checked, pleated, wrapped, woolly, one-shoulder, front-knotted, or similar.
- Avoid male-default naming. Do not create unmarked `tunic` plus marked `women's tunic` pairs when the meaningful distinction is length, fullness, sleeve form, drape, or layering. Gender can appear in builder-facing outfit manifests, but public text should prefer form-based distinctions.

## Skin strategy

- All accepted garments are skinnable because they are finished wearable goods.
- A skin can override presentation fields and quality without changing the underlying item behaviour.
- Skins should carry high-variance presentation: local fantasy-culture names, exact trim patterns, tablet-woven bands, embroidery, household marks, status colours, civic borders, brooch placement, dye intensity, material nuance, and fantasy-world motifs.
- Default unskinned items still need to be credible standalone garments. Skinability must not be used as an excuse for a bland, incomplete, or out-of-world base description.
- Player-facing base descriptions should never tell the player that a skin may later change the appearance.

## Colour-variable policy

- `Variable_BasicColour`: ordinary clean garments with common colours and simple light/dark variants. This is the default for simple wool, linen, and felt clothing.
- `Variable_FineColour`: fine, elite, formal, or otherwise richer garments where more varied and evocative colour words are appropriate.
- `Variable_2BasicColour`: ordinary two-colour woven or checked textiles.
- `Variable_2FineColour`: richer two-colour woven or checked textiles, especially where an elite or formal garment benefits from a more elaborate colour vocabulary.
- `Variable_DrabColour` and `Variable_2DrabColour`: only for deliberately dingy, grimy, tattered, filthy, degraded, or culturally grungy items. These are not neutral plain-clothing lists and are not used for normal clean antiquity garments.
- Some leather, hide, papyrus, and jewellery items omit colour variables when a natural fixed default reads better than allowing inappropriate colours.

## Materials, sizes, quality, and cost assumptions

- Dominant primary materials are `linen`, `wool`, `leather`, `fur`, `felt`, `papyrus`, and `glass`, depending on the garment.
- Primary material represents the dominant physical substance of the item, not every decorative or fastening detail. A belt can mention bronze fittings while still using `leather` as its primary material.
- Sizes are mostly `Normal` for bodywear and outer garments, `Small` for underwear, footwear, headwear, belts, veils, collars, and small accessories.
- Quality is `Standard` for common pieces and `Good` for elite, fine, formal, or more labour-intensive pieces.
- Costs use farthing-denominated defaults and are intended as relative seeder baselines, not universal market prices.

## Historical and design assumptions by inspiration family

### Hellenic-inspired draped clothing

Built around rectangular and semi-rectangular draped garments: chiton, peplos, himation, chlamys, sash/girdle, veil, and sandals. Descriptions focus on shoulder fastening, overfolds, belting, and the way cloth falls rather than close tailoring.

### Italic/Roman-inspired civic and layered clothing

Built around tunica, toga, stola, palla, sandals, and waist ties. The family separates ordinary tunica-and-mantle outfits from formal civic or elite draped garments while keeping public text culture-neutral.

### Celtic-inspired checked and braccae clothing

Built around sleeved tunics, braccae, checked or striped cloaks, wool skirts, mantles, belts, and leather footwear. Public item text emphasizes braccae, checks, borders, rectangles, mantles, and wool layers rather than ethnic labels.

### Germanic-inspired northern layered clothing

Built around straight wool tunics, narrow trousers, heavy cloaks, overlapping skirts, checked scarves, head veils, and skin capes. The design emphasizes warmth, layered construction, simple rectangular textiles, and hide-backed outerwear.

### Phoenician/Punic-inspired fitted, robed, and mantled clothing

Built around short fitted tunics, patterned waistcloths, short-sleeved overblouses, long robes, one-shouldered mantles, front-knotted girdles, conical caps, loose hoods, and glass-heavy jewellery.

### Persian-inspired riding and court clothing

Separates riding/cavalry clothing from court/elite clothing. Riding outfits use trousers, boots, sarapis, kandys, and caps; court outfits use pleated robes or gowns, fine kandys, patterned anaxyrides, formal belts, low shoes, and distinctive headgear.

### Egyptian-inspired linen clothing

Built around warm-climate linen: shendyt, kalasiris, kilt-sash, sheer overshirts, headcloths, shawls or capes, papyrus sandals, and usekh collars. Common outfits are simpler; elite outfits add pleating, sheerness, fine sandals, and jewellery.

## Outfit manifests

The following outfit manifests are builder-facing. Their labels use historical inspiration buckets for organization, while the referenced public items remain culture-neutral.

### Celtic common male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_plain_leather_belt` - a plain brown leather belt
- `antiquity_soft_leather_shoes` - a pair of soft leather shoes
- `antiquity_sleeved_common_wool_tunic` - a $colour sleeved wool tunic
- `antiquity_wool_braccae` - a pair of $colour wool braccae
- `antiquity_rectangular_wool_cloak` - a $colour rectangular wool cloak

### Celtic elite male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_bronze_buckled_leather_belt` - a $colour bronze-buckled belt
- `antiquity_ankle_leather_boots` - a pair of ankle leather boots
- `antiquity_fine_bordered_wool_tunic` - a fine $colour bordered tunic
- `antiquity_fine_wool_braccae` - a pair of fine $colour braccae
- `antiquity_fine_checked_wool_cloak` - a $colour1 and $colour2 checked cloak

### Celtic common female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_plain_leather_belt` - a plain brown leather belt
- `antiquity_soft_leather_shoes` - a pair of soft leather shoes
- `antiquity_long_sleeved_wool_tunic` - a long $colour wool tunic
- `antiquity_wool_wrap_skirt` - a $colour wool wrap skirt
- `antiquity_broad_wool_mantle` - a $colour broad wool mantle

### Celtic elite female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_fine_woven_sash` - a fine $colour sash
- `antiquity_ankle_leather_boots` - a pair of ankle leather boots
- `antiquity_fine_sleeved_wool_gown` - a fine $colour sleeved gown
- `antiquity_fine_bordered_wool_mantle` - a fine $colour bordered mantle
- `antiquity_linen_shoulder_veil` - a $colour linen veil

### Germanic common male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_plain_leather_belt` - a plain brown leather belt
- `antiquity_ankle_leather_boots` - a pair of ankle leather boots
- `antiquity_straight_wool_tunic` - a straight $colour wool tunic
- `antiquity_narrow_wool_trousers` - a pair of narrow $colour trousers
- `antiquity_heavy_wool_cloak` - a heavy $colour wool cloak

### Germanic elite male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_bronze_buckled_leather_belt` - a $colour bronze-buckled belt
- `antiquity_ankle_leather_boots` - a pair of ankle leather boots
- `antiquity_fine_banded_wool_tunic` - a fine $colour banded tunic
- `antiquity_fine_tapered_wool_trousers` - a pair of fine $colour trousers
- `antiquity_fur_lined_wool_cloak` - a $colour fur-lined cloak

### Germanic common female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_plain_leather_belt` - a plain brown leather belt
- `antiquity_ankle_leather_boots` - a pair of ankle leather boots
- `antiquity_long_straight_wool_tunic` - a long straight $colour tunic
- `antiquity_overlapping_wool_skirt` - a $colour overlapping wool skirt
- `antiquity_checked_wool_scarf` - a $colour1 and $colour2 wool scarf
- `antiquity_woolly_skin_cape` - a woolly skin cape

### Germanic elite female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_fine_woven_sash` - a fine $colour sash
- `antiquity_ankle_leather_boots` - a pair of ankle leather boots
- `antiquity_fine_long_wool_gown` - a fine $colour long gown
- `antiquity_fine_heavy_wool_mantle` - a fine $colour wool mantle
- `antiquity_linen_head_veil` - a $colour linen head veil

### Italic common male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_plain_leather_belt` - a plain brown leather belt
- `antiquity_plain_leather_sandals` - a pair of plain leather sandals
- `antiquity_knee_length_wool_tunica` - a knee-length $colour tunica
- `antiquity_wool_travel_mantle` - a $colour wool travel mantle

### Italic elite male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_fine_leather_sandals` - a pair of fine $colour sandals
- `antiquity_fine_linen_tunica` - a fine $colour linen tunica
- `antiquity_wool_toga` - a $colour wool toga

### Italic common female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_simple_woven_sash` - a $colour woven sash
- `antiquity_plain_leather_sandals` - a pair of plain leather sandals
- `antiquity_long_wool_tunica` - a long $colour wool tunica
- `antiquity_wool_palla` - a $colour wool palla

### Italic elite female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_fine_woven_sash` - a fine $colour sash
- `antiquity_fine_leather_sandals` - a pair of fine $colour sandals
- `antiquity_fine_long_linen_tunica` - a fine long $colour tunica
- `antiquity_wool_stola` - a $colour wool stola
- `antiquity_fine_wool_palla` - a fine $colour palla

### Hellenic common male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_simple_woven_sash` - a $colour woven sash
- `antiquity_plain_leather_sandals` - a pair of plain leather sandals
- `antiquity_short_wool_chiton` - a short $colour wool chiton
- `antiquity_wool_himation` - a $colour wool himation

### Hellenic elite male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_fine_woven_sash` - a fine $colour sash
- `antiquity_fine_leather_sandals` - a pair of fine $colour sandals
- `antiquity_fine_linen_chiton` - a fine $colour linen chiton
- `antiquity_fine_wool_himation` - a fine $colour himation
- `antiquity_short_wool_chlamys` - a short $colour wool chlamys

### Hellenic common female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_simple_woven_sash` - a $colour woven sash
- `antiquity_plain_leather_sandals` - a pair of plain leather sandals
- `antiquity_full_length_wool_peplos` - a full-length $colour peplos
- `antiquity_full_wool_himation` - a full $colour wool himation

### Hellenic elite female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_fine_woven_sash` - a fine $colour sash
- `antiquity_fine_leather_sandals` - a pair of fine $colour sandals
- `antiquity_fine_long_linen_chiton` - a fine long $colour chiton
- `antiquity_fine_full_wool_himation` - a fine full $colour himation
- `antiquity_light_linen_head_veil` - a $colour linen head veil

### Phoenician/Punic-inspired common male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_plain_leather_belt` - a plain brown leather belt
- `antiquity_plain_leather_sandals` - a pair of plain leather sandals
- `antiquity_short_fitted_linen_tunic` - a short fitted $colour tunic
- `antiquity_conical_felt_cap` - a conical $colour felt cap

### Phoenician/Punic-inspired elite male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_fine_leather_sandals` - a pair of fine $colour sandals
- `antiquity_fine_front_knotted_girdle` - a fine $colour knotted girdle
- `antiquity_patterned_linen_waistcloth` - a patterned $colour waistcloth
- `antiquity_short_sleeved_linen_overblouse` - a short-sleeved $colour overblouse
- `antiquity_long_linen_inner_robe` - a long $colour linen robe
- `antiquity_one_shoulder_wool_mantle` - a $colour one-shoulder mantle
- `antiquity_fine_conical_felt_cap` - a fine $colour conical cap

### Phoenician/Punic-inspired ritual or temple male variant

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_fine_papyrus_sandals` - a pair of fine papyrus sandals
- `antiquity_fine_front_knotted_girdle` - a fine $colour knotted girdle
- `antiquity_star_bordered_linen_robe` - a star-bordered $colour robe
- `antiquity_one_shoulder_wool_mantle` - a $colour one-shoulder mantle
- `antiquity_fine_conical_felt_cap` - a fine $colour conical cap

### Phoenician/Punic-inspired common female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_plain_leather_sandals` - a pair of plain leather sandals
- `antiquity_front_knotted_girdle` - a $colour front-knotted girdle
- `antiquity_long_folded_linen_robe` - a long folded $colour robe
- `antiquity_loose_linen_hood` - a loose $colour linen hood

### Phoenician/Punic-inspired elite female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_fine_leather_sandals` - a pair of fine $colour sandals
- `antiquity_fine_front_knotted_girdle` - a fine $colour knotted girdle
- `antiquity_fine_full_linen_gown` - a fine full $colour gown
- `antiquity_left_shoulder_overdrape` - a $colour left-shoulder overdrape
- `antiquity_loose_linen_hood` - a loose $colour linen hood

### Persian-inspired riding male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_soft_leather_riding_boots` - a pair of soft leather riding boots
- `antiquity_wide_cloth_belt` - a wide $colour cloth belt
- `antiquity_sarapis_wool_tunic` - a $colour wool sarapis
- `antiquity_anaxyrides_wool_trousers` - a pair of $colour anaxyrides
- `antiquity_wool_kandys` - a $colour wool kandys
- `antiquity_rounded_felt_cap` - a rounded $colour felt cap

### Persian-inspired court elite male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_low_strapped_leather_shoes` - a pair of strapped leather shoes
- `antiquity_fine_wide_cloth_belt` - a fine wide $colour belt
- `antiquity_fine_sarapis_linen_tunic` - a fine $colour sarapis
- `antiquity_fine_patterned_anaxyrides` - a pair of $colour1 and $colour2 anaxyrides
- `antiquity_pleated_court_robe` - a pleated $colour court robe
- `antiquity_fine_wool_kandys` - a fine $colour kandys
- `antiquity_fluted_felt_hat` - a fluted $colour felt hat

### Persian-inspired riding female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_soft_leather_riding_boots` - a pair of soft leather riding boots
- `antiquity_wide_cloth_belt` - a wide $colour cloth belt
- `antiquity_sarapis_wool_tunic` - a $colour wool sarapis
- `antiquity_anaxyrides_wool_trousers` - a pair of $colour anaxyrides
- `antiquity_wool_kandys` - a $colour wool kandys
- `antiquity_tall_kyrbasia` - a tall $colour kyrbasia

### Persian-inspired court elite female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_low_strapped_leather_shoes` - a pair of strapped leather shoes
- `antiquity_fine_wide_cloth_belt` - a fine wide $colour belt
- `antiquity_fine_pleated_court_gown` - a fine pleated $colour gown
- `antiquity_fine_wool_kandys` - a fine $colour kandys
- `antiquity_full_head_and_neck_veil` - a full $colour head veil

### Egyptian-inspired common male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_papyrus_sandals` - a pair of papyrus sandals
- `antiquity_simple_linen_shendyt` - a simple $colour linen shendyt
- `antiquity_wrapped_linen_headcloth` - a $colour linen headcloth

### Egyptian-inspired elite male

- `antiquity_linen_loincloth` - a $colour linen loincloth
- `antiquity_fine_papyrus_sandals` - a pair of fine papyrus sandals
- `antiquity_pleated_linen_shendyt` - a pleated $colour linen shendyt
- `antiquity_sheer_linen_overshirt` - a sheer $colour linen overshirt
- `antiquity_fine_linen_headcloth` - a fine $colour headcloth
- `antiquity_glass_usekh_collar` - a $colour glass usekh collar

### Egyptian-inspired common female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_papyrus_sandals` - a pair of papyrus sandals
- `antiquity_straight_linen_kalasiris` - a straight $colour kalasiris
- `antiquity_linen_shoulder_shawl` - a $colour linen shawl
- `antiquity_wrapped_linen_headcloth` - a $colour linen headcloth

### Egyptian-inspired elite female

- `antiquity_linen_breastband` - a $colour linen breastband
- `antiquity_fine_papyrus_sandals` - a pair of fine papyrus sandals
- `antiquity_fine_pleated_kalasiris` - a fine pleated $colour kalasiris
- `antiquity_fine_sheer_linen_cape` - a fine sheer $colour cape
- `antiquity_fine_linen_headcloth` - a fine $colour headcloth
- `antiquity_glass_usekh_collar` - a $colour glass usekh collar

## Item catalogue

Catalogue line format: `uniqueReference` - public short description; noun; primary material; size/quality; weight/cost; wear component; variable component. Full descriptions remain in the seeder calls and are not duplicated here.

### Shared European antiquity basics

- `antiquity_linen_loincloth` - a $colour linen loincloth; noun: `loincloth`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 90g/4.0m; wear: `Wear_Loincloth`; variables: Variable_BasicColour.
- `antiquity_linen_breastband` - a $colour linen breastband; noun: `breastband`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 80g/4.0m; wear: `Wear_Bra`; variables: Variable_BasicColour.
- `antiquity_plain_leather_belt` - a plain brown leather belt; noun: `belt`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 180g/10.0m; wear: `Wear_Waist`; variables: none.
- `antiquity_bronze_buckled_leather_belt` - a $colour bronze-buckled belt; noun: `belt`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 240g/36.0m; wear: `Wear_Waist`; variables: Variable_FineColour.
- `antiquity_plain_leather_sandals` - a pair of plain leather sandals; noun: `sandals`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 320g/12.0m; wear: `Wear_Sandals`; variables: none.
- `antiquity_fine_leather_sandals` - a pair of fine $colour sandals; noun: `sandals`; material: `leather`; size/quality: `Small`/`Good`; weight/cost: 280g/36.0m; wear: `Wear_Sandals`; variables: Variable_FineColour.
- `antiquity_soft_leather_shoes` - a pair of soft leather shoes; noun: `shoes`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 420g/18.0m; wear: `Wear_Shoes`; variables: none.
- `antiquity_ankle_leather_boots` - a pair of ankle leather boots; noun: `boots`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 650g/30.0m; wear: `Wear_Boots`; variables: none.
- `antiquity_simple_woven_sash` - a $colour woven sash; noun: `sash`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 120g/6.0m; wear: `Wear_Sash`; variables: Variable_BasicColour.
- `antiquity_fine_woven_sash` - a fine $colour sash; noun: `sash`; material: `wool`; size/quality: `Small`/`Good`; weight/cost: 100g/24.0m; wear: `Wear_Sash`; variables: Variable_FineColour.

### Celtic-inspired checked and braccae family

- `antiquity_sleeved_common_wool_tunic` - a $colour sleeved wool tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 620g/18.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour.
- `antiquity_wool_braccae` - a pair of $colour wool braccae; noun: `braccae`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 520g/16.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour.
- `antiquity_rectangular_wool_cloak` - a $colour rectangular wool cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1200g/28.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_BasicColour.
- `antiquity_fine_bordered_wool_tunic` - a fine $colour bordered tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 560g/60.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_FineColour.
- `antiquity_fine_wool_braccae` - a pair of fine $colour braccae; noun: `braccae`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 500g/48.0m; wear: `Wear_Trousers`; variables: Variable_FineColour.
- `antiquity_fine_checked_wool_cloak` - a $colour1 and $colour2 checked cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1150g/90.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_2FineColour.
- `antiquity_long_sleeved_wool_tunic` - a long $colour wool tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 680g/20.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour.
- `antiquity_wool_wrap_skirt` - a $colour wool wrap skirt; noun: `skirt`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 560g/16.0m; wear: `Wear_Long_Skirt`; variables: Variable_BasicColour.
- `antiquity_broad_wool_mantle` - a $colour broad wool mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1000g/26.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `antiquity_fine_sleeved_wool_gown` - a fine $colour sleeved gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 780g/72.0m; wear: `Wear_Long-Sleeved_Gown`; variables: Variable_FineColour.
- `antiquity_fine_bordered_wool_mantle` - a fine $colour bordered mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 950g/80.0m; wear: `Wear_Mantle`; variables: Variable_FineColour.
- `antiquity_linen_shoulder_veil` - a $colour linen veil; noun: `veil`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 120g/28.0m; wear: `Wear_Veil`; variables: Variable_FineColour.

### Germanic-inspired northern layered family

- `antiquity_straight_wool_tunic` - a straight $colour wool tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 650g/18.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour.
- `antiquity_narrow_wool_trousers` - a pair of narrow $colour trousers; noun: `trousers`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 540g/16.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour.
- `antiquity_heavy_wool_cloak` - a heavy $colour wool cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1250g/30.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_BasicColour.
- `antiquity_fine_banded_wool_tunic` - a fine $colour banded tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 600g/58.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_FineColour.
- `antiquity_fine_tapered_wool_trousers` - a pair of fine $colour trousers; noun: `trousers`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 520g/48.0m; wear: `Wear_Trousers`; variables: Variable_FineColour.
- `antiquity_fur_lined_wool_cloak` - a $colour fur-lined cloak; noun: `cloak`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1700g/96.0m; wear: `Wear_Cloak_(Closed)`; variables: Variable_FineColour.
- `antiquity_long_straight_wool_tunic` - a long straight $colour tunic; noun: `tunic`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 700g/20.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour.
- `antiquity_overlapping_wool_skirt` - a $colour overlapping wool skirt; noun: `skirt`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 600g/18.0m; wear: `Wear_Long_Skirt`; variables: Variable_BasicColour.
- `antiquity_checked_wool_scarf` - a $colour1 and $colour2 wool scarf; noun: `scarf`; material: `wool`; size/quality: `Small`/`Standard`; weight/cost: 240g/14.0m; wear: `Wear_Scarf`; variables: Variable_2BasicColour.
- `antiquity_woolly_skin_cape` - a woolly skin cape; noun: `cape`; material: `fur`; size/quality: `Normal`/`Standard`; weight/cost: 1400g/34.0m; wear: `Wear_Cape`; variables: none.
- `antiquity_fine_long_wool_gown` - a fine $colour long gown; noun: `gown`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 820g/70.0m; wear: `Wear_Long-Sleeved_Gown`; variables: Variable_FineColour.
- `antiquity_fine_heavy_wool_mantle` - a fine $colour wool mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1050g/78.0m; wear: `Wear_Mantle`; variables: Variable_FineColour.
- `antiquity_linen_head_veil` - a $colour linen head veil; noun: `veil`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 130g/26.0m; wear: `Wear_Veil`; variables: Variable_FineColour.

### Italic/Roman-inspired civic and layered family

- `antiquity_knee_length_wool_tunica` - a knee-length $colour tunica; noun: `tunica`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 560g/16.0m; wear: `Wear_Tunic`; variables: Variable_BasicColour.
- `antiquity_wool_travel_mantle` - a $colour wool travel mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 950g/24.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `antiquity_fine_linen_tunica` - a fine $colour linen tunica; noun: `tunica`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 380g/60.0m; wear: `Wear_Tunic`; variables: Variable_FineColour.
- `antiquity_wool_toga` - a $colour wool toga; noun: `toga`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 2800g/160.0m; wear: `Wear_Mantle`; variables: Variable_FineColour.
- `antiquity_long_wool_tunica` - a long $colour wool tunica; noun: `tunica`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 680g/18.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour.
- `antiquity_wool_palla` - a $colour wool palla; noun: `palla`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 900g/28.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `antiquity_fine_long_linen_tunica` - a fine long $colour tunica; noun: `tunica`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 420g/64.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_FineColour.
- `antiquity_wool_stola` - a $colour wool stola; noun: `stola`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 900g/100.0m; wear: `Wear_Sleeveless_Dress`; variables: Variable_FineColour.
- `antiquity_fine_wool_palla` - a fine $colour palla; noun: `palla`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 850g/90.0m; wear: `Wear_Mantle`; variables: Variable_FineColour.

### Hellenic-inspired draped family

- `antiquity_short_wool_chiton` - a short $colour wool chiton; noun: `chiton`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 420g/16.0m; wear: `Wear_Tunic`; variables: Variable_BasicColour.
- `antiquity_wool_himation` - a $colour wool himation; noun: `himation`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 950g/28.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `antiquity_fine_linen_chiton` - a fine $colour linen chiton; noun: `chiton`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 300g/72.0m; wear: `Wear_Tunic`; variables: Variable_FineColour.
- `antiquity_fine_wool_himation` - a fine $colour himation; noun: `himation`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 850g/84.0m; wear: `Wear_Mantle`; variables: Variable_FineColour.
- `antiquity_short_wool_chlamys` - a short $colour wool chlamys; noun: `chlamys`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 600g/30.0m; wear: `Wear_Cape`; variables: Variable_BasicColour.
- `antiquity_full_length_wool_peplos` - a full-length $colour peplos; noun: `peplos`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 720g/24.0m; wear: `Wear_Sleeveless_Dress`; variables: Variable_BasicColour.
- `antiquity_full_wool_himation` - a full $colour wool himation; noun: `himation`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 900g/30.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `antiquity_fine_long_linen_chiton` - a fine long $colour chiton; noun: `chiton`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 360g/80.0m; wear: `Wear_Long-Sleeved_Dress`; variables: Variable_FineColour.
- `antiquity_fine_full_wool_himation` - a fine full $colour himation; noun: `himation`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 820g/88.0m; wear: `Wear_Mantle`; variables: Variable_FineColour.
- `antiquity_light_linen_head_veil` - a $colour linen head veil; noun: `veil`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 110g/28.0m; wear: `Wear_Veil`; variables: Variable_FineColour.

### Shared Mediterranean, Near Eastern, and North African accessories

- `antiquity_papyrus_sandals` - a pair of papyrus sandals; noun: `sandals`; material: `papyrus`; size/quality: `Small`/`Standard`; weight/cost: 150g/8.0m; wear: `Wear_Sandals`; variables: none.
- `antiquity_fine_papyrus_sandals` - a pair of fine papyrus sandals; noun: `sandals`; material: `papyrus`; size/quality: `Small`/`Good`; weight/cost: 140g/24.0m; wear: `Wear_Sandals`; variables: none.
- `antiquity_low_strapped_leather_shoes` - a pair of strapped leather shoes; noun: `shoes`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 420g/28.0m; wear: `Wear_Shoes`; variables: none.
- `antiquity_soft_leather_riding_boots` - a pair of soft leather riding boots; noun: `boots`; material: `leather`; size/quality: `Small`/`Standard`; weight/cost: 780g/44.0m; wear: `Wear_Boots`; variables: none.
- `antiquity_wrapped_linen_headcloth` - a $colour linen headcloth; noun: `headcloth`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 130g/8.0m; wear: `Wear_Kerchief`; variables: Variable_BasicColour.
- `antiquity_fine_linen_headcloth` - a fine $colour headcloth; noun: `headcloth`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 110g/28.0m; wear: `Wear_Kerchief`; variables: Variable_FineColour.
- `antiquity_linen_shoulder_shawl` - a $colour linen shawl; noun: `shawl`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 260g/14.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `antiquity_fine_linen_shoulder_shawl` - a fine $colour linen shawl; noun: `shawl`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 220g/38.0m; wear: `Wear_Mantle`; variables: Variable_FineColour.
- `antiquity_front_knotted_girdle` - a $colour front-knotted girdle; noun: `girdle`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 110g/8.0m; wear: `Wear_Waist`; variables: Variable_BasicColour.
- `antiquity_fine_front_knotted_girdle` - a fine $colour knotted girdle; noun: `girdle`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 95g/26.0m; wear: `Wear_Waist`; variables: Variable_FineColour.
- `antiquity_conical_felt_cap` - a conical $colour felt cap; noun: `cap`; material: `felt`; size/quality: `Small`/`Standard`; weight/cost: 120g/12.0m; wear: `Wear_Hat`; variables: Variable_BasicColour.
- `antiquity_fine_conical_felt_cap` - a fine $colour conical cap; noun: `cap`; material: `felt`; size/quality: `Small`/`Good`; weight/cost: 110g/32.0m; wear: `Wear_Hat`; variables: Variable_FineColour.
- `antiquity_rounded_felt_cap` - a rounded $colour felt cap; noun: `cap`; material: `felt`; size/quality: `Small`/`Standard`; weight/cost: 105g/10.0m; wear: `Wear_Hat`; variables: Variable_BasicColour.
- `antiquity_tall_kyrbasia` - a tall $colour kyrbasia; noun: `kyrbasia`; material: `felt`; size/quality: `Small`/`Good`; weight/cost: 210g/42.0m; wear: `Wear_Hat`; variables: Variable_FineColour.
- `antiquity_fluted_felt_hat` - a fluted $colour felt hat; noun: `hat`; material: `felt`; size/quality: `Small`/`Good`; weight/cost: 170g/40.0m; wear: `Wear_Hat`; variables: Variable_FineColour.
- `antiquity_glass_usekh_collar` - a $colour glass usekh collar; noun: `collar`; material: `glass`; size/quality: `Small`/`Good`; weight/cost: 360g/90.0m; wear: `Wear_Necklace`; variables: Variable_FineColour.

### Phoenician/Punic-inspired fitted, robed, and mantled family

- `antiquity_short_fitted_linen_tunic` - a short fitted $colour tunic; noun: `tunic`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 360g/16.0m; wear: `Wear_Tunic`; variables: Variable_BasicColour.
- `antiquity_patterned_linen_waistcloth` - a patterned $colour waistcloth; noun: `waistcloth`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 240g/34.0m; wear: `Wear_Loincloth`; variables: Variable_FineColour.
- `antiquity_short_sleeved_linen_overblouse` - a short-sleeved $colour overblouse; noun: `overblouse`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 320g/30.0m; wear: `Wear_Shirt`; variables: Variable_FineColour.
- `antiquity_long_linen_inner_robe` - a long $colour linen robe; noun: `robe`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 700g/28.0m; wear: `Wear_Robe`; variables: Variable_BasicColour.
- `antiquity_one_shoulder_wool_mantle` - a $colour one-shoulder mantle; noun: `mantle`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 980g/38.0m; wear: `Wear_Mantle`; variables: Variable_BasicColour.
- `antiquity_long_folded_linen_robe` - a long folded $colour robe; noun: `robe`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 780g/30.0m; wear: `Wear_Robe`; variables: Variable_BasicColour.
- `antiquity_loose_linen_hood` - a loose $colour linen hood; noun: `hood`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 140g/12.0m; wear: `Wear_Veil`; variables: Variable_BasicColour.
- `antiquity_fine_full_linen_gown` - a fine full $colour gown; noun: `gown`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 860g/58.0m; wear: `Wear_Gown`; variables: Variable_FineColour.
- `antiquity_left_shoulder_overdrape` - a $colour left-shoulder overdrape; noun: `overdrape`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 300g/28.0m; wear: `Wear_Mantle`; variables: Variable_FineColour.
- `antiquity_star_bordered_linen_robe` - a star-bordered $colour robe; noun: `robe`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 820g/70.0m; wear: `Wear_Robe`; variables: Variable_FineColour.

### Persian-inspired riding and court family

- `antiquity_sarapis_wool_tunic` - a $colour wool sarapis; noun: `sarapis`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 720g/32.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_BasicColour.
- `antiquity_fine_sarapis_linen_tunic` - a fine $colour sarapis; noun: `sarapis`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 520g/54.0m; wear: `Wear_Long-Sleeved_Tunic`; variables: Variable_FineColour.
- `antiquity_anaxyrides_wool_trousers` - a pair of $colour anaxyrides; noun: `anaxyrides`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 620g/30.0m; wear: `Wear_Trousers`; variables: Variable_BasicColour.
- `antiquity_fine_patterned_anaxyrides` - a pair of $colour1 and $colour2 anaxyrides; noun: `anaxyrides`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 560g/66.0m; wear: `Wear_Trousers`; variables: Variable_2FineColour.
- `antiquity_wool_kandys` - a $colour wool kandys; noun: `kandys`; material: `wool`; size/quality: `Normal`/`Standard`; weight/cost: 1350g/58.0m; wear: `Wear_Cloak_(Open)`; variables: Variable_BasicColour.
- `antiquity_fine_wool_kandys` - a fine $colour kandys; noun: `kandys`; material: `wool`; size/quality: `Normal`/`Good`; weight/cost: 1250g/110.0m; wear: `Wear_Cloak_(Open)`; variables: Variable_FineColour.
- `antiquity_pleated_court_robe` - a pleated $colour court robe; noun: `robe`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 980g/92.0m; wear: `Wear_Robe`; variables: Variable_FineColour.
- `antiquity_fine_pleated_court_gown` - a fine pleated $colour gown; noun: `gown`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 880g/96.0m; wear: `Wear_Gown`; variables: Variable_FineColour.
- `antiquity_wide_cloth_belt` - a wide $colour cloth belt; noun: `belt`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 170g/14.0m; wear: `Wear_Waist`; variables: Variable_BasicColour.
- `antiquity_fine_wide_cloth_belt` - a fine wide $colour belt; noun: `belt`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 150g/34.0m; wear: `Wear_Waist`; variables: Variable_FineColour.
- `antiquity_full_head_and_neck_veil` - a full $colour head veil; noun: `veil`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 190g/30.0m; wear: `Wear_Veil`; variables: Variable_FineColour.

### Egyptian-inspired linen, kilt, kalasiris, and collar family

- `antiquity_simple_linen_shendyt` - a simple $colour linen shendyt; noun: `shendyt`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 230g/12.0m; wear: `Wear_Loincloth`; variables: Variable_BasicColour.
- `antiquity_pleated_linen_shendyt` - a pleated $colour linen shendyt; noun: `shendyt`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 280g/42.0m; wear: `Wear_Loincloth`; variables: Variable_FineColour.
- `antiquity_sheer_linen_overshirt` - a sheer $colour linen overshirt; noun: `overshirt`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 210g/36.0m; wear: `Wear_Shirt`; variables: Variable_FineColour.
- `antiquity_straight_linen_kalasiris` - a straight $colour kalasiris; noun: `kalasiris`; material: `linen`; size/quality: `Normal`/`Standard`; weight/cost: 520g/24.0m; wear: `Wear_Dress`; variables: Variable_BasicColour.
- `antiquity_fine_pleated_kalasiris` - a fine pleated $colour kalasiris; noun: `kalasiris`; material: `linen`; size/quality: `Normal`/`Good`; weight/cost: 480g/64.0m; wear: `Wear_Dress`; variables: Variable_FineColour.
- `antiquity_wrapped_linen_kilt_sash` - a $colour kilt-sash; noun: `sash`; material: `linen`; size/quality: `Small`/`Standard`; weight/cost: 260g/16.0m; wear: `Wear_Sash`; variables: Variable_BasicColour.
- `antiquity_fine_sheer_linen_cape` - a fine sheer $colour cape; noun: `cape`; material: `linen`; size/quality: `Small`/`Good`; weight/cost: 180g/46.0m; wear: `Wear_Cape`; variables: Variable_FineColour.

## Validation outcome

- The ItemSeeder now upserts all **29** documented outfit manifests as stock `OutfitTemplate` rows after the selected-era item phases complete.
- Six Egyptian manifest dependencies that were previously documented but absent from the direct item catalogue are now seeded from their exact catalogue specifications before outfit upsert.
- The accepted output contains no hidden or non-skinnable items.
- All accepted items use `null` for long description, morph target, morph emote, morph timer, and destroyed-item reference.
- The accepted item catalogues were checked against seeded component, material, and tag names during generation.
- Public text avoids explicit skin/customization language and avoids culture labels in item names, short descriptions, and full descriptions.

## Source notes

- Metropolitan Museum of Art, "Ancient Greek Dress" - used for chiton, peplos, himation, chlamys, strophion, and sandal/shoe/boot grounding.
- Encyclopaedia Britannica, "Dress - Ancient Rome, Tunic, Toga" and "Toga" - used for tunica and toga grounding, including the toga as a formal draped wool garment.
- LacusCurtius / Diodorus Siculus Book V - used for bracae, embroidered/dyed shirts, and striped or checked cloaks as a Celtic/Gaulish-facing inspiration bucket.
- National Museum of Denmark, "The Huldremose woman's clothes" - used for checked skirt/scarf, skin capes, leather strap, and northern layered textile grounding.
- George Rawlinson, *History of Phoenicia*, Chapter XII - used for short fitted tunics, conical caps, upper-class waistcloths, short-sleeved overgarments, one-shouldered mantles, long robes, front-knotted girdles, hoods/caps, and heavy jewellery conventions.
- Encyclopaedia Iranica, "CLOTHING ii. In the Median and Achaemenid periods" and "CANDYS" - used for court dress versus cavalry dress, sarapis/chiton undergarments, anaxyrides trousers, kandys outer garments, headgear, low shoes, riding boots, pleated court robes, and the limits of evidence for women's clothing.
- National Archaeological Museum, "Barbarian clothing or the garments of the foreigners" - used as a supporting source for ependytis, anaxyrides, kandys, long-sleeved chiton, kyrbasia, and eastern closed footwear.
- Metropolitan Museum of Art, "Kilt or Sash" - used for wrapped linen kilt/sash construction, rolled hems, woven selvage, and kilt plus shirt pairing.
- UCL Petrie Museum, "Tarkhan dress" and UCL News on the Tarkhan Dress - used for early linen dress survival, pleating around neck and sleeves, and the idea of worn linen dresses as real garments rather than only iconography.
- Albany Institute, "Ancient Egyptian Tunic" - used for kalasiris/tunic grounding, linen construction, wide fabric sewn along one side, gathering at the waist, belt loops, fringe, and use by both sexes.

## Appendix A: colour variable value lists

These lists are included to preserve the design distinction between ordinary, fine, and deliberately degraded colour vocabularies.

### basic_colours

black, white, grey, light grey, dark grey, red, dark red, blue, dark blue, green, brown, dark green, orange, light blue, light green, yellow, light red, purple, pink

### fine_colours

light grey, dark grey, red, dark red, blue, dark blue, green, brown, dark green, pale white, olive, caramel, ebony, emerald green, cerulean, violet, sandy brown, light brown, dark brown, auburn, onyx, obsidian, midnight black, ink black, jet black, pitch black, ivory, seashell, snow white, gleaming white, pure white, pearl white, bright white, bone white, ghost white, mist grey, charcoal grey, thistle grey, smoky grey, slate grey, silver grey, soft grey, ash grey, crimson, scarlet, ruby red, blood red, rose red, wine red, flame red, coral, copper, fiery orange, ochre, sunset orange, amber, goldenrod, pale yellow, golden yellow, sand yellow, topaz hued, gold-coloured, spring green, sea green, hunter green, olive green, sage green, pine green, bright green, rich green, pale green, verdant green, forest green, chartreuse, slate blue, bright blue, powder blue, sapphire blue, royal blue, ocean blue, teal, cornflour blue, sky blue, azure, beryl, cobalt, rich indigo, deep indigo, vivid indigo, earthen brown, deep brown, rich brown, burnt sienna, chocolate, cinnamon, mahogany, nut brown, umber, amethyst, mauve, mulbery, plum, lavender, royal purple, orange, light blue, light green, pale blue, yellow, cyan, navy blue, reddish brown, beige

### drab_colours

faded black, tattered black, shabby black, grimy black, off-white, dingy grey, bland yellow, faded green, faded blue, faded indigo, drab brown, dim grey, dusky slate grey, sooty grey, chalky pale grey, dull mist grey, ashen off-white, dirty bone-white, wan ivory, spotted white, stained white, blotched white, dingy off-white, stained ivory, shabby sallow-coloured, lurid pale yellow, dingy yellow, gaudy mustard yellow, sickly pale yellow, shabby pale yellow, murky brown, stained brown, dreary brown, bland brown, spotted muddy brown, dismal sand brown, dreary beige, grimy beige, shabby beige, dirty beige, tattered beige, bland wheat-coloured, drab olive, murky olive, dim olive, dingy green, shabby green, dull green, sickly greyish-green, grisly brownish-green, discoloured green, blotchy green, grimy rust-red, blotchy rust-red, grimy salmon, stained salmon, blotched red, dull red, faded red, stained red, dingy red, faded salmon, well-worn blue, faded slate blue, pallid blue, stained blue, grimy blue, dim blue-black, faded blue-black, dreary blue-black, dull orange, faded reddish-orange, tattered reddish-orange, discoloured orange, stained orange-red, drab peach-coloured, lurid peach-coloured, sickly peach-coloured, tattered violet, grimy lavender, spotted lavender, discoloured purple, dirty purple, dingy purple, faded purple, stained purple, dusty faded purple
