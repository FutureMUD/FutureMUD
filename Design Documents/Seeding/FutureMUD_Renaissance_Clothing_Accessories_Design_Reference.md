# FutureMUD Renaissance Clothing and Accessories Design Reference

## Scope

This branch owns the Renaissance clothing delta beyond the three generic shared pre-industrial accessories. Era-specific rows use `Era / Renaissance Era` and cover approximately 1400-1600 CE.

## Catalogue slices

- Western/central European shirts, chemises, smocks, doublets, jerkins, bodices, gowns, kirtles, hose, breeches, farthingales, ruffs, partlets, cloaks, capes, caps, hats, coifs, veils, gloves, masks, and fans.
- Ottoman, Safavid, Indo-Persian, South Asian, Ming, Joseon, Japanese, Ryukyuan, and South-east Asian robes, jackets, skirts, trousers, kaftans, jamas, turbans, sashes, hakama, kosode, kataginu, and court/scholar headgear where form differs.
- African, Sahelian, Ethiopian, Swahili, Mesoamerican, Andean, Caribbean, and North American Indigenous garment families where textile, hide, featherwork, beadwork, or wrapped construction justifies prototypes.
- Printer, scholar, merchant, notary, sailor, artisan, apothecary, artist, musician, actor/pageant, guard, and court-servant overlays.

Use skins for colour, trim, local names, household/guild/heraldic marks, religious signs, exact textile motifs, and status. New prototypes require a distinct silhouette, material behaviour, component, wear profile, production method, or institutional role.

## Dependency contract

Use the exact ledger in `FutureMUD_Renaissance_PrimaryIndustry_UsefulSeeder_Impact_Reference.md`. Linen, wool, cotton, silk, leather, felt, canvas, broadcloth, velvet, satin, lace, taffeta, ribbon, calico, and chintz are live exact materials.

## Implementation order and acceptance

Define shared silhouettes, culture-family exceptions, professional overlays, and authored outfit slots before skins and crafts. Portable garments include `Holdable` and valid wearable/destroyable components. Public text stays form-based, and complete outfits fail closed on missing authored pieces.
