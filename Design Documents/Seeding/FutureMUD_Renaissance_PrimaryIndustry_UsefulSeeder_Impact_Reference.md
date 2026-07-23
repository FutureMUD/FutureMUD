# FutureMUD Renaissance Primary Industry and UsefulSeeder Impact Reference

## Purpose and audit basis

This consolidated dependency ledger was checked against live skill, combat, material, tag, Agriculture, primary-production, shared ItemSeeder, and maintained catalogue sources. All Renaissance branch references depend on it.

## Dependency completion status

The data-only dependencies in this reference have been reviewed against current source. The three dry-container profiles, ten liquid-container profiles, nine new materials, and the later household expansion's six profiles are seeded and exported. `mother of pearl` is the existing alias of canonical `mother-of-pearl`. The only household prototype still deferred is `CashRegister_PreIndustrial_TillChest`; its requested lockability needs an engine change and is tracked in the [consolidated engine dependency ledger](./FutureMUD_Item_Content_Engine_Dependency_Ledger.md).

Previously completed Medieval jewellery, Medieval industry, Renaissance clothing-foundation, and primary-production dependencies were verified as existing source truth and were not reimplemented by this pass.

## Skills

| Status | Exact stock names | Notes |
| --- | --- | --- |
| Live craft choices | `Tailoring`/`Textilecraft`, `Armourcrafting`/`Armourer`, `Weaponcrafting`/`Weaponsmith`, `Blacksmithing`/`Metalcraft`, `Carpentry`/`Woodcraft`, `Pottery`, `Glassblowing`/`Glassblower`, `Glassworking`/`Glasswork`, `Bookbinding`/`Bookbinder`, `Papermaking`/`Papermaker`, `Calligraphy`/`Calligrapher`, `Scribing`/`Scribe`, `Woodblock Printing`/`Block Printer`, `Farming`, `Gardening`, `Herbalism`, `Apothecary`, `Cooking`/`Cookery`, `Brewing`, `Distilling`, `Winemaking`/`Winemaker`, `Ropemaking`/`Ropemaker`, `Coopering`/`Cooper` | Support stock gerund/imperative variants. |
| Conditional early-gun package | `Flintlocks`, `Pistols`, `Muskets` | Flintlock-centric and installed only by the optional CombatSeeder choice. |
| Optional professional choices | `Administration`/`Administrator`, `Law`/`Lawyer`, `Painting`/`Paint`, `Astronomy`, `Seafaring` | Must not be unconditional prerequisites. |
| Live historic specialist choices | `Movable Type Printing`/`Printer`, `Gunsmithing`/`Gunsmith`, `Powdermaking`/`Powdermaker`, `Lensmaking`/`Lensmaker`, `Clockmaking`/`Clockmaker`, `Instrument Making`/`Instrument Maker`, `Navigation`/`Navigator`, `Cartography`/`Cartographer`, `Surveying`/`Surveyor`, `Engraving`/`Engraver` | Available in the normal functional skill package in either naming mode. |
| Modern-option only | `Gunmaking`/`Gunmaker`, `Munitioning`/`Munitioner` | Remain modern-package skills; historical work should use Gunsmithing and Powdermaking. |

## Materials

Live exact materials include paper, parchment, oak, beech, walnut, linen, wool, cotton, silk, leather, felt, canvas, broadcloth, velvet, satin, lace, taffeta, ribbon, calico, chintz, ramie cloth, barkcloth, camelid wool, raffia cloth, gourd shell, papier-mache, birch bark, hemp cloth, brocade, damask, silk gauze, featherwork, beadwork, wrought iron, carbon steel, spring steel, brass, bronze, copper, lead, type metal, glass, glass blank, soda-lime glass, lead glass, porcelain, faience, earthenware, stoneware, clay, kaolinite clay, gunpowder, saltpeter, sulfur, brimstone, charcoal, flint, bone, horn, chalk, plaster, beeswax, logwood, cochineal, tobacco leaf, printing ink, molasses, sugar loaf, tobacco twist, snuff, roasted coffee, cacao bean, cacao nibs, chocolate block, tea brick, cotton fibre, and indigo dye cake. The maintained material export now contains the complete named dependency set. The six optional textile/composite additions are dependency records only; this pass does not add agricultural sources or production crafts for them.

## Tags

Live era tags are `Era / Pre-Industrial Era`, `Era / Renaissance Era`, and `Era / Early Modern Era`. Live functional families cover gunsmithing, movable-type and woodblock printing, papermaking, bookbinding, clockmaking, survey equipment, navigation, lensmaking, military equipment, writing goods/surfaces, textilecraft, armour, weapons, and professional tools. Live market families cover military goods, writing materials, household goods, and tea/coffee.

The `Culture / Renaissance / Shared` hierarchy, its 25 culture-family children, the six `Institution` children, clothing market categories, and worn-item function categories are maintained stock. UsefulSeeder resolves parents through full hierarchy paths so repeated names in separate branches cannot mis-parent these rows.

## Agriculture

Live crops/outputs cover maize/corn, potatoes, sweet potatoes, cassava, sugarcane, cotton, indigo, rice variants, cacao/cacao bean, coffee, tea, cinnamon, black pepper, cloves, nutmeg/mace, saffron, tobacco, cardamom, allspice, logwood, and explicit chamomile, lavender, yarrow, foxglove, henbane, and mandrake medicinal crops. Nopal cactus supplies a cochineal secondary output. Ramie, Breadfruit, and Raffia Palms now expose `ramie cloth`, `barkcloth`, and `raffia cloth`; Llama and Alpaca herds expose `camelid wool` instead of generic sheep wool.

## Primary production

Renaissance selection already receives `SeedPrimaryProductionToolsAndProps()`. Live references include `primary_production_saltpeter_deposit`, `primary_production_sulfur_deposit`, `primary_production_kaolin_deposit`, metal-ore/lead deposits, clay banks, charcoal sites/tools, glass furnaces, and common prospecting/mining/quarrying/smelting apparatus.

The named skills, materials, crop definitions, and generic primary-production commodity references now resolve. Complete chains still require matchlock/wheellock barrels and locks, springs/screws/stocks, safe blackpowder and shot manufacture, functional press crafts, lens/mirror-glass production, clock parts, porcelain/glaze specialisation, finished textile/dye crafts, global-crop processing crafts, and maritime-store projects.

## Gate for future implementation

Every era row must resolve exact material, tag, component, skill/trait, stable reference, and optional-package prerequisite before implementation. The named foundation dependencies now resolve; full production chains remain in their owning seeder branches.

## Household, urban, trade, container, and liquid-container dependency requests

The 400-row Renaissance household catalogue deliberately assumes the following reusable dependency additions. These are **component prototypes using existing engine component types**, not requests for new runtime component types. They should be seeded idempotently, exported to the maintained data documents, and covered by source-truth tests before item implementation.

### Requested item component prototypes

| Exact component name | Existing component type | Required configuration | Why it is needed | Safe temporary fallback |
| --- | --- | --- | --- | --- |
| `Container_PreIndustrial_CompartmentBox` | `Container` | Small closable dry container; many tiny goods share one inventory; non-transparent; non-locking; compartmenting is descriptive rather than independent subcontainers | Fitted sample, weight, seal, gem, vial, spice, bead, and utensil boxes need a smaller capacity than trunks without pretending multiple independent containers exist | `Container_Seal_Box` or `Container_Archive_Box`, accepting less precise capacity |
| `Container_PreIndustrial_LiddedBasket` | `Container` | Normal closable dry container for woven baskets; porous; non-transparent; non-locking | Domestic food baskets and shoulder baskets need closure without being misrepresented as open bins or soft sacks | `Container_Open_Bin`, losing closure |
| `Container_PreIndustrial_LiddedHamper` | `Container` | Large closable dry container for deep woven hampers and panniers; porous; non-transparent; non-locking | Market, tribute, spice, textile, and maize baskets need a larger closable capacity than the normal lidded basket | `Container_Open_Bin`, losing closure |
| `CashRegister_PreIndustrial_TillChest` | `CashRegister` | Normal chest-sized shop till; opaque; openable; built-in lock; modest cash capacity; no electronic behaviour | Merchant, guild, tavern, bazaar, apothecary, and customs packages need the shop till interaction rather than a merely descriptive cash box | `LockingContainer_Footlocker`, losing shop interaction |
| `LContainer_PreIndustrial_Cup_150ml` | `Liquid Container` | 150 ml; open; reusable; opaque; non-locking; non-self-refilling | Small wine, medicine, coffee, cacao, horn, gourd, and court cups need a period-neutral small capacity | `LContainer_SmallWineGlass`, with less suitable form assumptions |
| `LContainer_PreIndustrial_Bowl_750ml` | `Liquid Container` | 750 ml; open; reusable; opaque; non-locking; non-self-refilling | Liquid-capable household, serving, rice, cacao, and drinking bowls need behaviour distinct from dry `Container_Plate` | `LContainer_DrinkingGlass`, with incorrect capacity and form |
| `LContainer_PreIndustrial_Basin_5L` | `Liquid Container` | 5 L; open; reusable; opaque; non-locking; non-self-refilling | Portable wash, kitchen, dispensary, and court basins need finite liquid capacity without becoming self-refilling sinks | `Sink_5L`, which incorrectly supplies water-source behaviour |
| `LContainer_PreIndustrial_Ewer_2L` | `Liquid Container` | 2 L; open; reusable; opaque; non-locking; non-self-refilling; pouring form | Ewers and kendi-like pouring vessels need a shared capacity distinct from bottles and jugs | `LContainer_Jug`, accepting less precise form/capacity |
| `LContainer_PreIndustrial_Pitcher_4L` | `Liquid Container` | 4 L; open; reusable; opaque; non-locking; non-self-refilling | Large household, guildhall, shipboard, dairy, kitchen, tavern, and colonial pitchers need a larger shared capacity | `LContainer_GallonBottle` or `LContainer_Jug` |
| `LContainer_PreIndustrial_Pot_12L` | `Liquid Container` | 12 L; open; reusable; opaque; non-locking; non-self-refilling | Cooking pots and cauldrons need an open medium capacity rather than the closable storage-jar profile | `LContainer_PreIndustrial_StorageJar_12L`, accepting incorrect closure semantics |
| `LContainer_PreIndustrial_StorageJar_12L` | `Liquid Container` | 12 L; closable; reusable; opaque; non-locking; non-self-refilling | Oil, ghee, freshwater, perfume, medicine, and medium pantry jars need a common finite capacity | `LContainer_GallonBottle`, accepting modern bottle assumptions |
| `LContainer_PreIndustrial_StorageJar_30L` | `Liquid Container` | 30 L; closable; reusable; opaque; non-locking; non-self-refilling | Large household, trade, maritime, tribute, and pantry jars need a capacity below vats and casks | `LContainer_Cask`, accepting coopered-cask assumptions |
| `LContainer_PreIndustrial_Vat_125L` | `Liquid Container` | 125 L; open; reusable; opaque; non-locking; non-self-refilling; intended for installed vats | Rinsing, washing, dyeing, maceration, perfume, and scullery fixtures need finite process-vat capacity | `Sink_50L`, with wrong capacity and water-source semantics |
| `LContainer_PreIndustrial_Vat_500L` | `Liquid Container` | 500 L; open; reusable; opaque; non-locking; non-self-refilling; intended for installed vats | Shipboard water butts, fermentation vessels, public-household storage vats, and large process vessels need a large finite capacity | `Bathtub`, with incorrect bathing/water-source semantics |

### Requested exact solid materials

| Exact material name | Proposed material role | Why a separate material is useful | Safe temporary fallback |
| --- | --- | --- | --- |
| `gourd shell` | Manufactured/natural composite solid: dried, hollowed, cured bottle-gourd shell | A finished gourd vessel has shell-like hardness, buoyancy, breakage, and craft behaviour unlike edible `bottle gourd` flesh | `bottle gourd`, accepting food-crop behaviour |
| `papier-mache` | Manufactured paper composite | Lacquered and painted moulded cases and trays need a light rigid paper-composite material rather than loose `paper` | `paper`, accepting incorrect rigidity |
| `mother of pearl` | Animal-product decorative shell sheet/composite | Shell-faced export caskets and inlaid trays need a primary surface material distinct from generic `shell` or `pearl` | `shell`, losing nacre-specific economic and visual identity |
| `birch bark` | Prepared bark sheet/container material | Folded North American contact document boxes and similar bark containers need behaviour distinct from solid `birch` wood | `birch`, accepting incorrect density and construction |

### Implementation contract

- Seed component prototypes before `SeedRenaissanceItems()` validates or creates the household catalogue.
- Ordinary vessels and vats are finite liquid containers. Do not replace them with `WaterSource` components or any self-refilling profile.
- The cup, bowl, basin, ewer, pitcher, pot, jar, and vat profiles must remain mutually exclusive liquid-container providers.
- The compartment box, lidded basket, lidded hamper, and till chest each provide the container interface and must not be stacked with another dry-container or locking-container component.
- Add the four materials to the maintained material seeder/export with suitable physical and economic properties; catalogue implementation then uses the exact names without fallback.
- Update `Design Documents/Data/Seeded_Item_Components.json`, `Design Documents/Data/Seeded_Materials.json`, and the corresponding seeder tests in the same implementation change.
- No additional tags are requested by this catalogue; it uses the live Renaissance culture hierarchy, institution tags, household market/function tags, and container modifiers.
