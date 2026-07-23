# FutureMUD Early Modern Clothing and Accessories Dependency Ledger

> Running dependency ledger for the civilian first pass, military-uniform second pass, noble/jewellery third pass, religious clothing/accessories fourth pass, and headwear/footwear fifth pass.

## Status

- New component types required: **0**.
- New seeded component prototypes required: **2**.
- New solid materials required: **0**.
- Implementation status: **complete**. `Wear_Stays` and `Wear_Breeches` are seeded through `HumanSeeder`, exported in `Seeded_Item_Components.json`, and covered by exact bodypart/layering tests. The fourth and fifth passes introduce no additional dependency.
- Unrelated engine-dependent item requests are tracked in the [consolidated engine dependency ledger](./FutureMUD_Item_Content_Engine_Dependency_Ledger.md).

## Required seeded component prototypes

### `Wear_Stays`

- **Component type:** `Wearable`.
- **Purpose:** permits an item to be worn as structured stays beneath a bodice, jacket, short gown, mantua, or similar outer torso garment.
- **Why a new profile is required:** mapping stays to `Wear_Doublet`, `Wear_Vest`, or `Wear_Gown` would collide with the outer bodice or gown layer in the same outfit and would erase a mechanically meaningful under-structure layer.
- **Coverage intent:** chest, abdomen, back, and waist; arms and legs excluded.
- **Layering intent:** above a shift or chemise; below bodices, jackets, gowns, coats, capes, and cloaks.
- **Removal/visibility intent:** normally covered in a complete public outfit but still a distinct removable wearable.
- **Armour/insulation:** supplied separately by the item row; the component itself should only define wearable coverage and layering.
- **First-pass item references:** `earlymodern_western_clothing_canvas_stays`.
- **First-pass outfit use:** 48 of 350 manifests.

### `Wear_Breeches`

- **Component type:** `Wearable`.
- **Purpose:** permits joined knee- or calf-length breeches to be worn as an outer lower-body garment while retaining separate drawers and stockings.
- **Why a new profile is required:** `Wear_Shorts` is used for under-drawers, while `Wear_Trousers` models full-length legwear. Reusing either profile would create layering collisions or lose the stocking-compatible silhouette central to many western and colonial Early Modern outfits.
- **Coverage intent:** waist, hips, seat, groin, and thighs to knee or upper calf; lower legs and feet excluded.
- **Layering intent:** above drawers; compatible with separate stockings, shoes, coats, waistcoats, belts, and sashes.
- **Armour/insulation:** supplied separately by the item row; the component itself should only define wearable coverage and layering.
- **First-pass item references:** `earlymodern_western_clothing_knee_breeches`, `earlymodern_dutch_clothing_full_canvas_breeches`, `earlymodern_centraleuropean_clothing_leather_knee_breeches`, `earlymodern_northern_clothing_wool_knee_breeches`, `earlymodern_colonial_clothing_plain_knee_breeches`.
- **First-pass outfit use:** 53 of 350 manifests.

## Material audit

No new material is requested by this pass. The current repository material export supplies every primary material used by the catalogue, including `barkcloth`, `camelid wool`, `chintz`, `horsehair`, and `ramie cloth` in addition to the established linen, wool, cotton, silk, leather, felt, canvas, broadcloth, velvet, lace, hemp, fur, and wood entries.

The older project snapshot supplied to the authoring environment predates several of those material rows. That snapshot mismatch is not a new-material request; the current repository export is the authority for implementation.

## Existing component reconciliation

The current repository component export already supplies the Renaissance clothing profiles used here, including `Wear_Breechcloth`, `Wear_Head_Veil`, `Wear_Hood`, and `Wear_Long_Open_Robe`. They are not dependencies to create. All other item components in the catalogue are existing seeded prototypes.

## Assumption contract

The main design reference treats `Wear_Stays` and `Wear_Breeches` as available. If either profile is renamed during implementation, update every affected catalogue row and this ledger together; do not silently substitute a conflicting wearable profile.


## Third-pass noble clothing and jewellery audit

- New component types required by the third pass: **0**.
- New seeded component prototypes required by the third pass: **0**.
- New solid materials required by the third pass: **0**.
- Noble clothing reuses established robe, coat, jacket, gown, skirt, trouser, footwear, headwear, glove, sash, mantle, cloak, `Wear_Stays`, and `Wear_Breeches` profiles.
- Jewellery and regalia use existing ring, necklace, earring, bracelet, brooch, neck-ring, armlet, belt-plaque, waist-ornament, hair-ornament, forehead-ornament, diadem, coronet, and crown wearable profiles.
- The functional noble signet ring uses `SealStamp_Medieval_NobleSignetRing`; all other jewellery remains inert apart from wear and destruction behaviour.
- The third pass adds no armour, identity-obscuring, container, rank, or office mechanics to jewellery.


## Fourth-pass religious clothing and accessories audit

- New component types required by the fourth pass: **0**.
- New seeded component prototypes required by the fourth pass: **0**.
- New solid materials required by the fourth pass: **0**.
- New fourth-pass rows: **73**.
- Earlier-era religious stable references admitted: **74**.
- The pass uses established robe, open-robe, tabard, poncho, cloak, cape, mantle, scarf, sash, shoulders, headwear, veil, coif, mask, headband, bandolier, bracer, bracelet, ring, necklace, neck-ring, hair-comb, girdle-ornament, skirt, vest, trouser, shorts, shoe, sandal, boot, and long-coat profiles.
- `Wear_Mask` is used for the Jain mouthcloth without `Gag`, `IdentityObscurer`, or `Obscurer`; the item therefore has no unsupported speech or disguise behaviour.
- Sikh kachera, kara, and kanga use existing `Wear_Shorts`, `Wear_Bracelet`, and `Wear_Hair_Comb`. The kirpan is a weapon dependency owned by the military/weapons branch, not a missing clothing component.
- Quaker plain-dress manifests reuse ordinary current-authority clothing and require no new prototype; plainness is carried by skins and stock admission.
- Zoroastrian sudreh, kusti, prayer cap, priestly robe, and padan use existing shirt, sash, skullcap, robe, and mask profiles. The padan has no gag, identity-obscuring, or breathing component.
- Religious bead strings, the Eastern prayer rope, crosses, medallions, rings, and collars use existing wearable jewellery/accessory profiles and remain inert apart from wear and destruction behaviour.
- No new special ritual, ordination, sect, rank, performance, or legal-status mechanic is requested.


## Fifth-pass headwear and footwear audit

- New component types required by the fifth pass: **0**.
- New seeded component prototypes required by the fifth pass: **0**.
- New solid materials required by the fifth pass: **0**.
- New fifth-pass rows: **84** — **48 headwear** and **36 footwear**.
- Headwear uses existing `Wear_Hat`, `Wear_Hood`, `Wear_Turban`, `Wear_Kerchief`, and `Wear_Wig` profiles. Wigs also use the existing `Wig` presentation component.
- Footwear uses existing `Wear_Shoes`, `Wear_Boots`, `Wear_High_Boots`, `Wear_Sandals`, and `Wear_Overshoes` profiles.
- The pass reuses exact live materials including leather, rawhide, linen, cotton, wool, felt, broadcloth, silk, velvet, lace, canvas, straw, bamboo, wood, fur, horsehair, camelid wool, and raffia cloth.
- Every row uses exact maintained Early Modern era, headwear/footwear function, and clothing-market tags.
- No row requests waterproofing, disguise, rank, office, mount-control, weapon, armour-harness, or legal-status behaviour.
- The fifth pass does not alter `Wear_Stays` or `Wear_Breeches` and therefore leaves the two-item unresolved dependency list unchanged.
