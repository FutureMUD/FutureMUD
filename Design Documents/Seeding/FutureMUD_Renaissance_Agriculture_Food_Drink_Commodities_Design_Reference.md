# FutureMUD Renaissance Agriculture, Food, Drink, and Commodities Design Reference

## Scope

This branch coordinates Renaissance crops, processing outputs, consumables, and globally traded commodities while reusing shared containers.

## Live Agriculture coverage

Stock Agriculture already supplies maize, potatoes, sweet potatoes, cassava, sugarcane, cotton, indigo, rice and African rice, cacao, coffee, tea, cinnamon, black pepper, cloves, nutmeg, saffron, and numerous herbs. Exact outputs include `corn`, `potato`, `sweet potato`, `cassava`, `sugarcane`, `cotton crop`, `indigo crop`, `rice`, `african rice`, `cacao`, `coffee bean`, `tea leaf`, `cinnamon bark`, `black pepper`, `cloves`, `nutmeg`, and `saffron crocus`.

## Added foundations

Stock Agriculture now includes Tobacco, Cardamom, Allspice, Logwood, Chamomile, Lavender, Yarrow, Foxglove, Henbane, and Mandrake definitions. Nopal Cactus supplies cochineal as a secondary output, Nutmeg supplies mace, and Cacao supplies cacao beans without removing their existing primary outputs. Ramie supplies `ramie cloth`, Breadfruit supplies `barkcloth`, and Raffia Palms supply `raffia cloth` as secondary textile outputs. Llama and Alpaca herds now supply the exact `camelid wool` material rather than generic `wool`.

Material foundations now exist for sugar loaf, molasses, cacao beans/nibs, tobacco leaf/twist, cotton fibre, indigo dye cake, cochineal, tea bricks/cakes, roasted coffee, snuff, chocolate blocks, ramie cloth, barkcloth, camelid wool, and raffia cloth. Rum remains a live liquid. Finished item prototypes, packets/bales, and transformation crafts remain implementation work.

Use the live shared tea chest, coffee sack, cacao sack, tobacco bale, sugar hogshead, indigo cake box, cotton bale, and spice chest when the package fits. Packaging availability must not backdate consumption culture.

## Acceptance criteria

- Every named crop resolves to Agriculture source or an intentional secondary output.
- Every processing output resolves to a material/liquid/item before craft activation.
- Shared packages are reused.
- Colonial/contact-zone goods are intentionally scoped rather than treated as universal trade flavour.
