# FutureMUD Early Modern Clothing and Accessories Design Reference

## Scope

This branch owns the Early Modern clothing delta beyond the three shared pre-industrial belt/sash accessories. It covers approximately 1600-1750 CE and uses `Era / Early Modern Era` for era-specific rows.

## Catalogue slices

- Western and central European shirts, shifts, stays, bodices, coats, waistcoats, breeches, petticoats, mantuas, gowns, riding habits, cloaks, stockings, buckled shoes, boots, wigs, cocked hats, bonnets, caps, gloves, fans, muffs, aprons, liveries, and uniforms.
- Ottoman, Persianate, Mughal, South Asian, Qing, Joseon, Edo, South-east Asian, African, Indigenous American, and colonial-hybrid silhouettes when a skin would misrepresent the form.
- Soldier, sailor, officer, servant, merchant, lawyer/notary, clerk, printer, natural philosopher, apothecary, surgeon, coffeehouse worker, and practical plantation/work clothing overlays.

Local names, colour, trim, textile motif, heraldry, household or regimental marks, and status variants should normally be skins. A new prototype requires a distinct silhouette, material behaviour, component, wear profile, production method, or institutional role.

## Dependency contract

Exact skill and material names must be selected from `FutureMUD_EarlyModern_PrimaryIndustry_UsefulSeeder_Impact_Reference.md`. The live palette supports linen, wool, cotton, silk, leather, felt, canvas, broadcloth, velvet, satin, lace, taffeta, ribbon, calico, and chintz work.

Use live clothing market tags plus `Era / Early Modern Era`. Culture-specific tag paths are not yet maintained stock and belong to the culture-manifest implementation phase.

## Implementation order

1. Define shared silhouettes and wear/component requirements.
2. Define culture-family exceptions.
3. Define profession, military, and livery overlays.
4. Add skins after base prototypes are stable.
5. Add crafts only after exact skills, material stock, and tool tags resolve.

## Acceptance criteria

- No late-Renaissance reskin is used where the underlying silhouette changed.
- Portable garments include `Holdable` and valid wearable/destroyable components.
- Public names are form-based; builder notes carry historical inspiration.
- Complete outfits declare intentionally shared slots and fail closed on missing authored pieces.
