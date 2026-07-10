# FutureMUD Early Modern Primary Industry and UsefulSeeder Impact Reference

## Purpose and audit basis

This is the consolidated dependency ledger for all Early Modern branch references. It was checked against `SkillPackageSeeder.cs`, `CombatSeeder.cs`, `CoreDataSeeder.Materials.cs`, `UsefulSeeder.Tags.cs`, `AgricultureSeeder.cs`, `PrimaryProductionSeeder.cs`, the shared pre-industrial ItemSeeder partials, and the maintained catalogue exports.

## Skills

| Status | Exact stock names | Use |
| --- | --- | --- |
| Live core craft choices | `Tailoring`/`Textilecraft`, `Armourcrafting`/`Armourer`, `Weaponcrafting`/`Weaponsmith`, `Blacksmithing`/`Metalcraft`, `Carpentry`/`Woodcraft`, `Pottery`, `Glassblowing`/`Glassblower`, `Glassworking`/`Glasswork`, `Bookbinding`/`Bookbinder`, `Papermaking`/`Papermaker`, `Calligraphy`/`Calligrapher`, `Scribing`/`Scribe`, `Woodblock Printing`/`Block Printer`, `Farming`, `Gardening`, `Herbalism`, `Apothecary`, `Cooking`/`Cookery`, `Brewing`, `Distilling`, `Winemaking`/`Winemaker`, `Ropemaking`/`Ropemaker`, `Coopering`/`Cooper` | Existing craft families. Support gerund/imperative variants. |
| Conditional combat package | `Flintlocks`, `Pistols`, `Muskets` | Created only when CombatSeeder installs early gunpowder weapons. |
| Optional professional choices | `Administration`/`Administrator`, `Law`/`Lawyer`, `Painting`/`Paint`, `Astronomy`, `Seafaring` | Do not use as unconditional seeder prerequisites. |
| Live historic specialist choices | `Movable Type Printing`/`Printer`, `Gunsmithing`/`Gunsmith`, `Powdermaking`/`Powdermaker`, `Lensmaking`/`Lensmaker`, `Clockmaking`/`Clockmaker`, `Instrument Making`/`Instrument Maker`, `Navigation`/`Navigator`, `Cartography`/`Cartographer`, `Surveying`/`Surveyor`, `Engraving`/`Engraver` | Available in the normal functional skill package in either naming mode. |
| Modern-option only | `Gunmaking`/`Gunmaker`, `Munitioning`/`Munitioner` | Remain modern-package skills; historical work should use Gunsmithing and Powdermaking. |

## Materials

Live exact materials include paper, parchment, oak, beech, linen, wool, cotton, silk, leather, felt, canvas, broadcloth, velvet, satin, lace, taffeta, ribbon, calico, chintz, wrought iron, carbon steel, spring steel, brass, bronze, copper, lead, type metal, glass, glass blank, soda-lime glass, lead glass, porcelain, faience, earthenware, stoneware, clay, kaolinite clay, gunpowder, saltpeter, sulfur, brimstone, charcoal, flint, bone, horn, logwood, cochineal, tobacco leaf, printing ink, molasses, sugar loaf, tobacco twist, snuff, roasted coffee, cacao bean, cacao nibs, chocolate block, tea brick, cotton fibre, and indigo dye cake. The maintained material export now contains the complete named foundation set.

## Tags

Live era tags are `Era / Pre-Industrial Era`, `Era / Renaissance Era`, and `Era / Early Modern Era`. Live functional families include `Functions / Tools / Gunsmithing Tools`, `Functions / Tools / Movable Type Printing Tools`, `Functions / Tools / Woodblock Printing Tools`, `Functions / Tools / Papermaking Tools`, `Functions / Tools / Bookbinding Tools`, `Functions / Tools / Metalworking Tools / Clockmaking Tools`, `Functions / Tools / Scientific Tools / Measurement Tools / Surveying Equipment`, `Functions / Tools / Scientific Tools / Navigational Tools`, `Lensmaking Tools`, `Functions / Military Equipment`, `Functions / Writing Goods`, and `Functions / Writing Surface`.

Live market families include military weapons/armour/ammunition/blackpowder, military uniforms, writing materials, professional tools, household goods, and luxury tea/coffee. Culture-family and institution-specific Early Modern tag paths are not maintained stock and must be added idempotently before branch items use them.

## Agriculture

Live crops and outputs cover maize/corn, potatoes, sweet potatoes, cassava, sugarcane, cotton/cotton crop, indigo/indigo crop, rice variants, cacao/cacao bean, coffee/coffee bean, tea/tea leaf, cinnamon/cinnamon bark, black pepper, cloves, nutmeg/mace, saffron, tobacco/tobacco leaf, cardamom, allspice, logwood, and explicit chamomile, lavender, yarrow, foxglove, henbane, and mandrake medicinal crops. Nopal cactus also supplies a cochineal secondary output.

## Primary production

The shared dispatcher already installs `SeedPrimaryProductionToolsAndProps()`. Live resource/item references include `primary_production_saltpeter_deposit`, `primary_production_sulfur_deposit`, `primary_production_kaolin_deposit`, ore and lead deposits, clay banks, charcoal tools/sites, glass furnaces, and the common prospecting/mining/quarrying/smelting apparatus. Live materials/tags support gunpowder ingredients, lead, glass, ceramics, common metals, tar/pitch, rope/canvas inputs, and pigments.

The named skills, materials, crop definitions, and generic primary-production commodity references now resolve. Still missing are complete gun-lock/barrel/stock/spring production projects, safe powder manufacture, cartridge and shot chains, functional publishing, lens/mirror-glass production, clock/watch parts, porcelain/glaze specialisation, finished textile/dye crafts, cash-crop processing crafts, and complete maritime-store projects.

## Gate for future implementation

No era stub may add a row until every exact material, tag, component, skill/craft trait, stable reference, and optional package prerequisite resolves. The foundation now resolves the named catalogue dependencies; full chains remain work for their owning project, craft, and item branches.
