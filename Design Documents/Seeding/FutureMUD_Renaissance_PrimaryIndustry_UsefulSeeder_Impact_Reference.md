# FutureMUD Renaissance Primary Industry and UsefulSeeder Impact Reference

## Purpose and audit basis

This consolidated dependency ledger was checked against live skill, combat, material, tag, Agriculture, primary-production, shared ItemSeeder, and maintained catalogue sources. All Renaissance branch references depend on it.

## Skills

| Status | Exact stock names | Notes |
| --- | --- | --- |
| Live craft choices | `Tailoring`/`Textilecraft`, `Armourcrafting`/`Armourer`, `Weaponcrafting`/`Weaponsmith`, `Blacksmithing`/`Metalcraft`, `Carpentry`/`Woodcraft`, `Pottery`, `Glassblowing`/`Glassblower`, `Glassworking`/`Glasswork`, `Bookbinding`/`Bookbinder`, `Papermaking`/`Papermaker`, `Calligraphy`/`Calligrapher`, `Scribing`/`Scribe`, `Woodblock Printing`/`Block Printer`, `Farming`, `Gardening`, `Herbalism`, `Apothecary`, `Cooking`/`Cookery`, `Brewing`, `Distilling`, `Winemaking`/`Winemaker`, `Ropemaking`/`Ropemaker`, `Coopering`/`Cooper` | Support stock gerund/imperative variants. |
| Conditional early-gun package | `Flintlocks`, `Pistols`, `Muskets` | Flintlock-centric and installed only by the optional CombatSeeder choice. |
| Optional professional choices | `Administration`/`Administrator`, `Law`/`Lawyer`, `Painting`/`Paint`, `Astronomy`, `Seafaring` | Must not be unconditional prerequisites. |
| Live historic specialist choices | `Movable Type Printing`/`Printer`, `Gunsmithing`/`Gunsmith`, `Powdermaking`/`Powdermaker`, `Lensmaking`/`Lensmaker`, `Clockmaking`/`Clockmaker`, `Instrument Making`/`Instrument Maker`, `Navigation`/`Navigator`, `Cartography`/`Cartographer`, `Surveying`/`Surveyor`, `Engraving`/`Engraver` | Available in the normal functional skill package in either naming mode. |
| Modern-option only | `Gunmaking`/`Gunmaker`, `Munitioning`/`Munitioner` | Remain modern-package skills; historical work should use Gunsmithing and Powdermaking. |

## Materials

Live exact materials include paper, parchment, oak, beech, walnut, linen, wool, cotton, silk, leather, felt, canvas, broadcloth, velvet, satin, lace, taffeta, ribbon, calico, chintz, wrought iron, carbon steel, spring steel, brass, bronze, copper, lead, type metal, glass, glass blank, soda-lime glass, lead glass, porcelain, faience, earthenware, stoneware, clay, kaolinite clay, gunpowder, saltpeter, sulfur, brimstone, charcoal, flint, bone, horn, chalk, plaster, beeswax, logwood, cochineal, tobacco leaf, printing ink, molasses, sugar loaf, tobacco twist, snuff, roasted coffee, cacao bean, cacao nibs, chocolate block, tea brick, cotton fibre, and indigo dye cake. The maintained material export now contains the complete named foundation set.

## Tags

Live era tags are `Era / Pre-Industrial Era`, `Era / Renaissance Era`, and `Era / Early Modern Era`. Live functional families cover gunsmithing, movable-type and woodblock printing, papermaking, bookbinding, clockmaking, survey equipment, navigation, lensmaking, military equipment, writing goods/surfaces, textilecraft, armour, weapons, and professional tools. Live market families cover military goods, writing materials, household goods, and tea/coffee.

Renaissance culture-family and institution-specific tag paths are not maintained stock and must be designed and seeded idempotently before branch rows depend on them.

## Agriculture

Live crops/outputs cover maize/corn, potatoes, sweet potatoes, cassava, sugarcane, cotton, indigo, rice variants, cacao/cacao bean, coffee, tea, cinnamon, black pepper, cloves, nutmeg/mace, saffron, tobacco, cardamom, allspice, logwood, and explicit chamomile, lavender, yarrow, foxglove, henbane, and mandrake medicinal crops. Nopal cactus supplies a cochineal secondary output.

## Primary production

Renaissance selection already receives `SeedPrimaryProductionToolsAndProps()`. Live references include `primary_production_saltpeter_deposit`, `primary_production_sulfur_deposit`, `primary_production_kaolin_deposit`, metal-ore/lead deposits, clay banks, charcoal sites/tools, glass furnaces, and common prospecting/mining/quarrying/smelting apparatus.

The named skills, materials, crop definitions, and generic primary-production commodity references now resolve. Complete chains still require matchlock/wheellock barrels and locks, springs/screws/stocks, safe blackpowder and shot manufacture, functional press crafts, lens/mirror-glass production, clock parts, porcelain/glaze specialisation, finished textile/dye crafts, global-crop processing crafts, and maritime-store projects.

## Gate for future implementation

Every era row must resolve exact material, tag, component, skill/trait, stable reference, and optional-package prerequisite before implementation. The named foundation dependencies now resolve; full production chains remain in their owning seeder branches.
