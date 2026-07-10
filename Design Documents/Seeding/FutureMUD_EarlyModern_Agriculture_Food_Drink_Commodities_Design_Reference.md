# FutureMUD Early Modern Agriculture, Food, Drink, and Commodities Design Reference

## Scope

This branch coordinates Early Modern crops, processed commodities, consumables, service goods, and trade packaging. It does not duplicate the shared containers.

## Live Agriculture coverage

The stock Agriculture seeder already supplies maize, potatoes, sweet potatoes, cassava, sugarcane, cotton, indigo, rice and African rice, cacao, coffee, tea, cinnamon, black pepper, cloves, nutmeg, saffron, and numerous culinary or medicinal herbs. Exact live outputs include `corn`, `potato`, `sweet potato`, `cassava`, `sugarcane`, `cotton crop`, `indigo crop`, `rice`, `african rice`, `cacao`, `coffee bean`, `tea leaf`, `cinnamon bark`, `black pepper`, `cloves`, `nutmeg`, and `saffron crocus`.

## Added Agriculture foundations

Stock Agriculture now includes Tobacco, Cardamom, Allspice, Logwood, Chamomile, Lavender, Yarrow, Foxglove, Henbane, and Mandrake definitions. Nopal Cactus supplies cochineal as a secondary output, Nutmeg supplies mace, and Cacao supplies cacao beans without removing their existing primary outputs. Every added crop output has an exact seeded material and appropriate seedability.

## Processing packages

Material foundations now exist for sugar loaf, molasses, tobacco leaf/twist, snuff, roasted coffee, cacao beans/nibs, chocolate blocks, tea bricks/cakes, cotton fibre, indigo dye cake, and cochineal. Rum remains a live liquid. Finished item prototypes, packets/bales, and transformation crafts remain implementation work and must reuse the shared packages below.

Use `preindustrial_trade_tea_chest`, `preindustrial_trade_coffee_sack`, `preindustrial_trade_cacao_sack`, `preindustrial_trade_tobacco_bale`, `preindustrial_trade_sugar_hogshead`, `preindustrial_trade_indigo_cake_box`, `preindustrial_trade_cotton_bale`, and `preindustrial_trade_spice_chest` where their package form fits.

## Sensitive contexts

Plantation, enslavement, mission, company, and colonial-administration systems require explicit optional packages and documentation. A neutral crop or package must not silently encode coercive labour.

## Acceptance criteria

- Every named crop resolves to an Agriculture definition or an intentional secondary output.
- Every output resolves to an exact material/liquid/item reference before a craft is enabled.
- Packaging stable references are reused.
- Consumption goods use real edible, liquid-container, smokeable, or other relevant components rather than prose-only claims.
