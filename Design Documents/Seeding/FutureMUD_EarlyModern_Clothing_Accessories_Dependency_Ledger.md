# FutureMUD Early Modern Clothing and Accessories Dependency Ledger

> Running dependency ledger for the civilian, military-uniform, and noble/jewellery catalogues.

## Status

- New component types required: **0**.
- New seeded component prototypes required: **2**.
- New solid materials required: **0**.
- This document assumes the two component prototypes below will be created by a later seeder implementation before item rows are seeded.
- The military-uniform and noble/jewellery catalogues add no further component or material dependencies.

## Required seeded component prototypes

### `Wear_Stays`

- **Component type:** `Wearable`.
- **Purpose:** permits an item to be worn as structured stays beneath a bodice, jacket, short gown, mantua, or similar outer torso garment.
- **Why a new profile is required:** mapping stays to `Wear_Doublet`, `Wear_Vest`, or `Wear_Gown` would collide with the outer bodice or gown layer in the same outfit and would erase a mechanically meaningful under-structure layer.
- **Coverage intent:** chest, abdomen, back, and waist; arms and legs excluded.
- **Layering intent:** above a shift or chemise; below bodices, jackets, gowns, coats, capes, and cloaks.
- **Removal/visibility intent:** normally covered in a complete public outfit but still a distinct removable wearable.
- **Armour/insulation:** supplied separately by the item row; the component itself should only define wearable coverage and layering.
- **Civilian item references:** `earlymodern_western_clothing_canvas_stays`.
- **Civilian outfit use:** 48 of 350 manifests.

### `Wear_Breeches`

- **Component type:** `Wearable`.
- **Purpose:** permits joined knee- or calf-length breeches to be worn as an outer lower-body garment while retaining separate drawers and stockings.
- **Why a new profile is required:** `Wear_Shorts` is used for under-drawers, while `Wear_Trousers` models full-length legwear. Reusing either profile would create layering collisions or lose the stocking-compatible silhouette central to many western and colonial Early Modern outfits.
- **Coverage intent:** waist, hips, seat, groin, and thighs to knee or upper calf; lower legs and feet excluded.
- **Layering intent:** above drawers; compatible with separate stockings, shoes, coats, waistcoats, belts, and sashes.
- **Armour/insulation:** supplied separately by the item row; the component itself should only define wearable coverage and layering.
- **Civilian item references:** `earlymodern_western_clothing_knee_breeches`, `earlymodern_dutch_clothing_full_canvas_breeches`, `earlymodern_centraleuropean_clothing_leather_knee_breeches`, `earlymodern_northern_clothing_wool_knee_breeches`, `earlymodern_colonial_clothing_plain_knee_breeches`.
- **Civilian outfit use:** 53 of 350 manifests.

## Military-uniform catalogue audit

- New component types required by the military-uniform catalogue: **0**.
- New seeded component prototypes required by the military-uniform catalogue: **0**.
- New solid materials required by the military-uniform catalogue: **0**.
- Generic rank and appointment accessories resolve to existing `Wear_Gorget`, `Wear_Epaulette`, `Wear_Spaulders`, `Wear_Shoulders`, `Wear_Badge`, `Wear_Shoulder`, `Wear_Bandolier`, `Wear_Sash`, `Wear_Waist`, and `Wear_Scarf` profiles.
- Uniform garments resolve to existing coat, jacket, vest, robe, trouser, skirt, footwear, and headwear profiles plus the already-declared `Wear_Breeches` dependency.
- The exact live tags `Functions / Military Equipment`, `Market / Clothing / Military Uniforms`, and `Era / Early Modern Era` support new rows.
- The current material export supplies every military-uniform primary material, including broadcloth, ribbon, silk, felt, fur, leather, cotton, ramie cloth, camelid wool, brass, and silver.

The military-uniform catalogue therefore assumes only the same two unresolved wearable profiles as the civilian catalogue. It must not introduce substitute layering profiles for convenience.

## Material audit

No new material is requested by any clothing catalogue. The current repository material export supplies every primary material used by the catalogue, including `barkcloth`, `camelid wool`, `chintz`, `horsehair`, and `ramie cloth` in addition to the established linen, wool, cotton, silk, leather, felt, canvas, broadcloth, velvet, lace, hemp, fur, and wood entries.

The older project snapshot supplied to the authoring environment predates several of those material rows. That snapshot mismatch is not a new-material request; the current repository export is the authority for implementation.

## Existing component reconciliation

The current repository component export already supplies the Renaissance clothing profiles used here, including `Wear_Breechcloth`, `Wear_Head_Veil`, `Wear_Hood`, and `Wear_Long_Open_Robe`. They are not dependencies to create. All other item components in the catalogue are existing seeded prototypes.

## Assumption contract

The main design reference treats `Wear_Stays` and `Wear_Breeches` as available across the civilian, military-uniform, and noble/court catalogues. If either profile is renamed during implementation, update every affected catalogue row and this ledger together; do not silently substitute a conflicting wearable profile.

## Noble clothing and jewellery catalogue audit

- New component types required by the noble/jewellery catalogue: **0**.
- New seeded component prototypes required by the noble/jewellery catalogue: **0**.
- New solid materials required by the noble/jewellery catalogue: **0**.
- Noble clothing reuses established robe, coat, jacket, gown, skirt, trouser, footwear, headwear, glove, sash, mantle, cloak, `Wear_Stays`, and `Wear_Breeches` profiles.
- Jewellery and regalia use existing ring, necklace, earring, bracelet, brooch, neck-ring, armlet, belt-plaque, waist-ornament, hair-ornament, forehead-ornament, diadem, coronet, and crown wearable profiles.
- The functional noble signet ring uses `SealStamp_Medieval_NobleSignetRing`; all other jewellery remains inert apart from wear and destruction behaviour.
- The noble/jewellery catalogue adds no armour, identity-obscuring, container, rank, or office mechanics to jewellery.
