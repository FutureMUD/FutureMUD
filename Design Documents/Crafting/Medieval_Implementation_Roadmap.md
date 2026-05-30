# Medieval Implementation Roadmap

This roadmap records the current implementation status after MED-CAT-001 and MED-OUTFIT-001 through MED-OUTFIT-004, then defines the next goals for the medieval item and craft suite.

The key principle remains:

> Shared production chains are encouraged; shared final outfits and culture-visible final goods are not.

The completed outfit work is a major improvement over the original generic status/culture matrix. The next work should now shift from outfit completeness to implementation quality, builder usability, and non-clothing culture suites.

## Review Status After MED-OUTFIT-004

| Goal | Status | Review |
| --- | --- | --- |
| `MED-CAT-001` | Met | The code has an explicit medieval culture catalogue structure and exact documentation/test alignment. |
| `MED-OUTFIT-001` | Met | The code has outfit slot, outfit spec, and outfit-piece structures, with testing accessors. |
| `MED-OUTFIT-002` | Met | North Atlantic/British cultures have complete outfit rows and vocabulary tests. |
| `MED-OUTFIT-003` | Met | Continental Western/Central European cultures have complete outfit rows and vocabulary tests. |
| `MED-OUTFIT-004` | Met | Byzantine, Islamic, Rus, Steppe, and Song cultures have complete outfit rows and vocabulary tests. |
| `MED-OUTFIT-005` | Not done | Starter outfit package support remains a stub and should now be implemented or explicitly rejected after investigation. |
| `MED-FOOD-001` | Not done | Still a stub. Explicit foodway references exist as targets, but the suite needs real prepared-food items and food-input sanity tests. |
| `MED-MIL-001` | Not done | Still a stub. Generic military equipment exists, but explicit culture military loadouts still need item/craft implementation beyond outfit clothing/accessories. |
| `MED-WRITE-001` | Not done | Still a stub. Writing/admin culture references need real item/craft implementation and component sanity fixes. |
| `MED-HOUSE-001` | Not done | Still a stub. Household/devotional/luxury culture references need real item/craft implementation. |
| `MED-TEST-001` | Partially superseded | Outfit quality tests are now present. A new non-clothing quality-gate goal should replace the old stub. |

## Current Implementation Findings

### What is now good

The current implementation now has:

- explicit culture catalogue data
- outfit slot and outfit spec records
- 216 outfit definitions: 18 cultures x 2 sex/gender presentations x 6 social roles
- exact outfit documentation tests
- cluster tests for North Atlantic, Continental Western, and Eastern/Islamic/Rus/Steppe/Song outfit groups
- vocabulary tests for all three outfit clusters
- explicit outfit-piece crafts using named object craft names rather than `regional pattern`
- material-stock tests for several reviewed outfit-piece edge cases

This is enough to say MED-OUTFIT-001 through MED-OUTFIT-004 are functionally met.

### Resolved Caveat 1: explicit outfit piece descriptions are authored

MED-OUTFIT-008B replaces generated explicit outfit-piece prose with fail-closed authored rows. Every explicit outfit-piece stable reference now requires a literal row, and missing rows are seeder errors.

Further polish should edit the authored rows directly rather than reintroducing generated fallback prose.

### Caveat 2: explicit culture catalogue targets outside outfits are not yet guaranteed as real items

The explicit culture catalogue contains military, food/beverage, writing/admin, and household/devotional stable references. The next goals must ensure those are not only documentation targets. Each reference should become either:

- an implemented item with craft coverage
- a deliberate alias to an existing outfit/common item
- a documented deferred target

No exact catalogue reference should remain silently target-only.

### Caveat 3: some non-paper writing surfaces still use paper-like components

The suite still needs a writing-component sanity pass. Wax tablets, wooden tablets, birchbark letters, and non-paper tablets should use `InscribableSurface`-style components where available rather than scroll/paper components.

### Caveat 4: generic military crafts still use regional-pattern names

Explicit outfit-piece crafts are now named properly. Some generic military/equipment crafts still use `regional pattern` names. That is tolerable for generic scaffold items, but explicit military loadouts should use exact object craft names.

## Goal Notation Template

Use this format for future tasks:

```text
GOAL MED-<AREA>-<NUMBER>: <short title>

Intent:
  What player/builder-facing quality this pass is meant to improve.

Files to touch:
  Exact expected files.

Required changes:
  Concrete implementation changes.

Catalogue requirements:
  Exact item-count, outfit-count, and stable-reference requirements.

Craft requirements:
  Required craft coverage and input chains.

Documentation requirements:
  Which docs must be updated and how exact the stable-reference or outfit listing must be.

Test requirements:
  New or updated tests that must fail before the change and pass after it.

Non-goals:
  Things Codex must not attempt in this pass.

Acceptance criteria:
  Objective checks for reviewer sign-off.
```

# Completed Goals

## GOAL MED-CAT-001: Establish Exact Medieval Culture Catalogue

Status:
  Completed.

Keep:
  - explicit culture catalogue structure
  - exact documentation tests
  - culture group boundaries

Follow-up:
  - `MED-CAT-002` must materialise or explicitly classify non-outfit catalogue references.

## GOAL MED-OUTFIT-001: Add Complete Outfit Catalogue Model

Status:
  Completed.

Keep:
  - `MedievalOutfitSpec`
  - outfit slots
  - test accessors
  - 216 outfit target
  - exact outfit reference documentation

Follow-up:
  - Continue clothing work against the authored outfit-piece table rather than the old generated prose fallback.

## GOAL MED-OUTFIT-002: Implement Complete North Atlantic and British Outfits

Status:
  Completed.

Cultures covered:
  - `early_anglo_saxon`
  - `anglo_danish`
  - `norse`
  - `norman`
  - `high_british`
  - `gaelic`

Follow-up:
  - Polish pass only. Do not reopen basic outfit-count goals unless tests reveal missing references.

## GOAL MED-OUTFIT-003: Implement Complete Continental Western and Central European Outfits

Status:
  Completed.

Cultures covered:
  - `carolingian`
  - `capetian`
  - `german_hre`
  - `iberian_christian`

Follow-up:
  - Polish pass only. Do not reopen basic outfit-count goals unless tests reveal missing references.

## GOAL MED-OUTFIT-004: Implement Complete Byzantine, Islamic, Rus, Steppe, and Song Outfits

Status:
  Completed.

Cultures covered:
  - `andalusi`
  - `byzantine`
  - `abbasid`
  - `fatimid`
  - `seljuk_ayyubid`
  - `rus_novgorod`
  - `steppe_turkic`
  - `song_china`

Follow-up:
  - Polish pass only. Do not reopen basic outfit-count goals unless tests reveal missing references.

# Immediate Follow-Up Goals

## GOAL MED-CAT-002: Materialise or Classify Explicit Culture Catalogue References

Intent:
  Ensure exact culture catalogue references outside the outfit system are not only documentation targets.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Culture_Catalogue.md`
  - `Design Documents/Crafting/Medieval_Crafting_Audit.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Required changes:
  - Add a status model for explicit culture catalogue references, or add code paths that materialise every reference.
  - Classify every non-outfit catalogue reference as one of:
    - `ImplementedItem`
    - `CoveredByOutfitPiece`
    - `AliasOfExistingStableReference`
    - `Deferred`
  - Do not leave any exact reference as target-only without a status.
  - If a reference is implemented, it must appear in `MedievalItemStableReferencesForTesting`.
  - If a reference is deferred, it must have a documented reason.

Catalogue requirements:
  - Every culture must still have catalogue groups:
    - Clothing
    - Military
    - Food and Beverage
    - Writing and Administration
    - Household and Devotional
  - Each exact reference in those groups must have a status.
  - For `AliasOfExistingStableReference`, record both the catalogue reference and the actual implementation reference.

Craft requirements:
  - Implemented non-outfit item references must have craft coverage unless explicitly marked as stock-only, morph target, or deferred.

Documentation requirements:
  - `Medieval_Culture_Catalogue.md` should show each entry’s implementation status.
  - `Medieval_Crafting_Audit.md` should summarise how many entries are implemented, aliased, covered by outfit pieces, or deferred.

Test requirements:
  - Add a test that every exact catalogue reference is classified.
  - Add a test that every `ImplementedItem` resolves to an item spec.
  - Add a test that every `ImplementedItem` has craft coverage or documented exemption.
  - Add a test that deferred entries have a reason string.

Non-goals:
  - Do not implement every non-clothing reference in this goal unless small enough.
  - Do not create new runtime systems.

Acceptance criteria:
  - Reviewers can tell which catalogue references are real items, which are aliases, and which are future targets.

## GOAL MED-OUTFIT-005: Add Starter Outfit Package Support

Intent:
  Let builders load, inspect, or copy complete outfits as coherent sets rather than manually searching for each piece.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - likely `DatabaseSeeder/Seeders/UsefulSeeder.cs` or the existing item-package seeder area, if item packages are the correct mechanism
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`
  - `Design Documents/Crafting/Medieval_Outfit_Catalogue.md`
  - `Design Documents/Crafting/Medieval_Clothing_Crafting_Suite.md`

Required changes:
  - Investigate current item package support.
  - If existing package support is suitable, seed package definitions for each `medieval_outfit_{culture}_{sex}_{class}`.
  - If existing package support is not suitable, add a documented lightweight outfit manifest only, and do not create runtime packages.
  - The package or manifest must map outfit reference to stable references by slot.
  - Packages should not duplicate item prototypes.

Catalogue requirements:
  - 216 outfit packages or manifests:
    - 18 cultures
    - 2 sex/gender presentations
    - 6 classes
  - Every package/manifest must include the required outfit slots.
  - Packages may include optional extras:
    - spare cloak
    - religious book
    - trade tool
    - weapon harness
    - seasonal outerwear

Craft requirements:
  - No new craft needed.
  - All package items must already be craftable or intentionally stock-only.

Documentation requirements:
  - `Medieval_Outfit_Catalogue.md` should identify whether each outfit is backed by a package or only by a manifest.
  - Add builder-facing notes explaining how to load or reconstruct the outfit.

Test requirements:
  - Add a test that every outfit has a package or manifest record.
  - If package seeding exists, add a test that package references resolve to item stable references.
  - Add a test that package item references are a subset of `MedievalItemStableReferencesForTesting`.

Non-goals:
  - Do not implement a new runtime item package system unless the existing package mechanism cannot be used and the change is separately approved.
  - Do not make package creation block outfit item seeding.

Acceptance criteria:
  - A builder can inspect `medieval_outfit_norse_female_peasant` and see/load every piece needed for the outfit.

## GOAL MED-OUTFIT-006: Outfit Piece Description, Component, and Material Polish

Intent:
  Upgrade explicit outfit pieces from generated slot descriptions to final-quality item prototypes with better prose, better component mapping, and more accurate material/cost/weight decisions. MED-OUTFIT-008B supersedes the partial override model by requiring a direct authored row for every explicit outfit-piece stable reference.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Clothing_Crafting_Suite.md`
  - `Design Documents/Crafting/Medieval_Testing_Quality_Gates.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Required changes:
  - Add optional override records for outfit pieces:
    - stable reference
    - noun
    - short description
    - full description
    - material
    - material type
    - quality
    - weight
    - cost
    - components
    - craft input override
  - Apply overrides for at least the most culture-defining pieces first:
    - Norse hangerok/apron dress and oval brooches
    - Byzantine silk dalmatic and sagion
    - Song cross-collar robe and official cap
    - Gaelic brat mantle and ring pin
    - Andalusi qamis, sirwal, burnous, and turban
    - Rus rubakha, fur-edged kaftan, onuchi, and fur hat
    - Steppe felt riding caftan and bowcase-and-quiver belt
  - Replace generic full descriptions of the form "belongs to the X outfit" with real visual and construction descriptions.
  - Keep outfit metadata in builder notes rather than making it the whole full description.
  - Audit role-item components so non-jewellery role items are not function-tagged as jewellery by default.

Catalogue requirements:
  - At least 10 override pieces per culture in the first polish pass.
  - All outfit role items must have appropriate function tags:
    - books and tablets: writing goods
    - pouches/satchels: containers or pouches
    - scabbards/quivers/bowcases: military equipment or weapon accessories
    - devotional pendants/beads: jewellery/devotional
    - tools: professional tools or relevant worn items

Craft requirements:
  - Override craft inputs should be materially plausible.
  - Metal hardware should consume metal stock, not cloth stock.
  - Books and notebooks should consume paper/parchment and bookbinding stock.
  - Fur-edged garments should consume fur stock.
  - Felt garments should consume felt/fulled-cloth stock.
  - Shoes and boots should consume footwear leather stock.

Documentation requirements:
  - Document override policy in `Medieval_Clothing_Crafting_Suite.md`.
  - Record which cultures have received the polish pass.

Test requirements:
  - Add a test that at least 10 outfit pieces per culture have non-generated full descriptions.
  - Add a test that role-item function tags are appropriate for the item type.
  - Add tests for representative material overrides:
    - books use bookbinding stock
    - quivers/bowcases use leather or scabbard stock
    - brooches/pins use metal stock
    - cloth shoes use textile stock
    - fur hats use fur stock
  - Add a test that full descriptions for override pieces do not simply begin with `This {pieceName} belongs to`.

Non-goals:
  - Do not rewrite every outfit item manually in one pass unless time permits.
  - Do not change outfit slot membership unless a slot is plainly wrong.

Acceptance criteria:
  - Signature outfit pieces read like real item prototypes, not catalogue placeholders.

## GOAL MED-OUTFIT-007: Layering and Wear-Slot Audit

Intent:
  Check whether the generated component choices let complete outfits actually be worn together without slot conflicts or inappropriate worn components.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`
  - `Design Documents/Crafting/Medieval_Clothing_Crafting_Suite.md`

Required changes:
  - Audit the component mapping for:
    - underlayers
    - lower-body layers
    - hose/legwraps/socks
    - shoes/boots/sandals
    - robes/gowns/tunics
    - cloaks/mantles/surcoats
    - belts/sashes
    - pouches
    - fasteners/jewellery
    - role items
  - Ensure underlayer and bodywear pieces do not collide unless the engine supports layering.
  - Ensure lower-body garments are not all represented as `Wear_Chausses` if skirts, robes, sirwal, trousers, or wraps need different slots.
  - Ensure role items such as field pouches, book pouches, quivers, bowcase belts, seal pouches, sleeve pouches, and tools are wearable or containable in sensible places.
  - Add intentionally-unwearable prop notes where a role item is a carried item rather than worn.

Catalogue requirements:
  - Every outfit slot should declare whether its item is:
    - worn
    - held/carried
    - contained in another worn item
    - decorative only

Craft requirements:
  - No new craft requirements unless component fixes alter items.

Documentation requirements:
  - Update clothing suite docs with layering assumptions and any known engine limitations.

Test requirements:
  - Add tests for component compatibility by slot category.
  - Add tests that role items are not all blindly treated as jewellery.
  - Add tests that pouches and containers include container/beltable/wear components where appropriate.
  - Add tests that books/tablets are not given clothing armour components unless worn as a pouch or case.

Non-goals:
  - Do not implement new wear-layer runtime systems in this goal.
  - If the engine lacks a required wear slot, document the limitation and pick the least-bad existing component.

Acceptance criteria:
  - A representative complete outfit can be loaded and worn logically by a builder or test harness without obvious slot/component contradictions.

# Non-Clothing Culture Suite Goals

## GOAL MED-FOOD-001: Implement Real Culture Foodway Items

Intent:
  Make medieval foodways produce actual food/beverage items instead of mostly tableware, trays, packets, and generic cue props.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Food_Beverage_Crafting_Suite.md`
  - `Design Documents/Crafting/Medieval_Culture_Catalogue.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Required changes:
  - For every `Food and Beverage` culture catalogue reference, implement an item spec or classify it under `MED-CAT-002`.
  - Split tableware from actual food:
    - food item
    - beverage item
    - vessel/tableware item
    - ration/container item
  - Use `PreparedFood`-style components if suitable.
  - If `PreparedFood` is not appropriate, tag as a food prop and record the limitation in builder notes.
  - Replace or supplement the old generic foodway examples so that each culture has real food names.

Catalogue requirements:
  - At least 8 explicit food/beverage entries per culture:
    - staple bread or grain dish
    - everyday cooked dish
    - preserved/travel food
    - elite/feast dish
    - beverage
    - dairy/oil/spice/condiment item where appropriate
    - vessel/tableware item
    - market/ration item
  - Tableware counts only toward the vessel slot, not toward prepared-food coverage.

Craft requirements:
  - Food crafts must consume food commodities or liquids.
  - Prepared food must not be made from only `Furniture Panel Stock`.
  - Acceptable food inputs include:
    - `Flour Commodity`
    - grain or pulse commodity
    - prepared meat or fish stock
    - `Cheese Curd Stock`
    - `Cheese Wheel Stock`
    - `Ale Stock`
    - `Cider Stock`
    - `Mead Stock`
    - milk
    - tea/herb stock where available
    - wine or wine-style liquid where available
    - oil
    - honey
    - salt
    - spice
    - fruit
    - vegetables
    - broth or pottage base stock

Documentation requirements:
  - Update `Medieval_Food_Beverage_Crafting_Suite.md` with:
    - exact stable references
    - item type classification
    - craft input pattern
    - any food-prop limitations

Test requirements:
  - Add a test that every food catalogue reference is implemented or classified.
  - Add a food-input sanity test.
  - Add a test that tableware is not counted as prepared food.
  - Add representative culture vocabulary checks:
    - Norse stockfish and sour milk
    - Song tea and noodles/rice
    - Steppe kumis and dried curds
    - Andalusi/Fatimid/Abbasid flatbread, dates, oil, syrup, or yogurt
    - Rus rye, fish, mushrooms, honey drink
    - Western bread, pottage, ale/cider/wine, cheese

Non-goals:
  - Do not implement full nutrition, cooking, or recipe runtime mechanics if the existing prepared-food system is insufficient.
  - Do not create species-specific agriculture or livestock systems in this pass.

Acceptance criteria:
  - A builder can stock a tavern, hall, monastery, camp, ship, caravan, or market with recognisable food and drink from the selected culture.

## GOAL MED-MIL-001: Implement Culture-Specific Military Loadout Items

Intent:
  Replace the current one-armour/one-weapon/one-shield model with culturally distinctive military kits.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Equipment_Crafting_Suite.md`
  - `Design Documents/Crafting/Medieval_Culture_Catalogue.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Required changes:
  - Implement or classify every `Military` culture catalogue reference.
  - Add item specs and crafts for explicit military references beyond outfit arming clothing.
  - Final craft names should use object names, not `regional pattern`.
  - Keep generic military scaffold items only as baseline.

Catalogue requirements:
  - At least 8 explicit military/equipment items per culture, excluding:
    - generic armour package
    - generic weapon
    - generic shield
    - generic padded coif
    - generic sidearm harness
    - generic quiver
    - generic field pack
    - generic war banner
  - Each culture should include a mix of:
    - primary weapon
    - sidearm
    - missile weapon or ammunition
    - shield
    - helmet/head protection
    - body armour or padded layer
    - belt/harness/scabbard/quiver
    - campaign or guard accessory

Craft requirements:
  - Use existing medieval stock tags:
    - `Weapon Blade Stock`
    - `Weapon Head Stock`
    - `Weapon Shaft Stock`
    - `Shield Board Stock`
    - `Shield Facing Stock`
    - `Mail Panel Stock`
    - `Armour Lamella Stock`
    - `Quilted Armour Padding`
    - `Military Cord Stock`
    - `Leather Strap`
  - Crossbow variants must consume:
    - `Crossbow Tiller Stock`
    - `Crossbow Prod Stock`
    - `Crossbow Lockwork Stock`
    - `Military Cord Stock`

Documentation requirements:
  - Update `Medieval_Equipment_Crafting_Suite.md` with exact stable references and culture loadout notes.
  - Document which items are armour, weapon, ammunition, accessory, or prop.

Test requirements:
  - Add a test that every military catalogue reference is implemented or classified.
  - Add explicit military count tests per culture.
  - Add representative input tests:
    - mail uses mail stock
    - lamellar uses lamella stock
    - shields use board/facing stock
    - crossbows use crossbow stock
    - scabbards/quivers use leather stock
  - Add a test that explicit military crafts do not use `regional pattern`.

Non-goals:
  - Do not implement full plate armour, hand firearms, gunpowder, or late medieval artillery.
  - Do not implement horse tack runtime systems unless already available.

Acceptance criteria:
  - Each culture has enough military goods to equip multiple roles, not a single representative soldier.

## GOAL MED-WRITE-001: Implement Culture-Specific Writing, Administration, and Seal Media

Intent:
  Give each culture appropriate recordkeeping objects and writing surfaces instead of generic bundles and tablets.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Writing_Administration_Crafting_Suite.md`
  - `Design Documents/Crafting/Medieval_Culture_Catalogue.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Required changes:
  - Implement or classify every `Writing and Administration` culture catalogue reference.
  - Fix non-paper writing surfaces:
    - wax tablets
    - wooden tablets
    - birchbark letters
    - note boards
    - tally sticks
  - Use `InscribableSurface`-style components where the engine supports them.
  - Use `PaperSheet` or `Book` only for appropriate paper/parchment/codex items.
  - Use `Sealable` for sealed packets and documents.
  - Use `SealStamp` for seal matrices, signets, guild stamps, and official chops.

Catalogue requirements:
  - At least 6 explicit writing/admin items per culture.
  - Include culture-appropriate media:
    - parchment charters and rolls
    - wax tablets
    - paper decrees/contracts/registers
    - birchbark letters for Rus
    - printed notices/registers for Song China
    - runic trade tallies for Norse
    - seal tags and sealed packets for administrative cultures

Craft requirements:
  - Paper documents consume `Paper Sheet Stock`.
  - Parchment documents consume `Parchment Sheet Stock`.
  - Sealed documents consume `Sealing Wax Stock` and `Seal Cord Stock`.
  - Books/codices consume sheet stock and `Bookbinding Leather Stock`.
  - Tallies consume `Tally Stick Stock`.
  - Seal stamps consume metal `Tool Blank Stock`.

Documentation requirements:
  - Update writing/admin docs with exact stable references and component choices.
  - Document any component limitation if `InscribableSurface` is not usable for a specific target.

Test requirements:
  - Add a test that every writing/admin catalogue reference is implemented or classified.
  - Add a component sanity test so wooden/wax/birchbark/tablet references do not use `PaperSheet_Scroll`.
  - Add tests for `SealStamp`, `Sealable`, `Book`, and `PaperSheet` use where appropriate.
  - Add vocabulary checks:
    - Rus birchbark
    - Song official chop/printed notice/register
    - Norse runic tally
    - Byzantine chrysobull
    - Islamic paper contract/decree/chancery

Non-goals:
  - Do not implement new writing-component runtime systems unless separately approved.
  - Do not add printing-press workflows; Song printed notices may be seeded as printed paper objects or props.

Acceptance criteria:
  - Builders can distinguish chancery, monastery, market, court, guild, steppe messenger, and Song bureaucratic record objects.

## GOAL MED-HOUSE-001: Implement Culture-Specific Household, Devotional, and Luxury Goods

Intent:
  Make non-clothing everyday and prestige goods distinct by culture.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Furniture_Container_Crafting_Suite.md`
  - `Design Documents/Crafting/Medieval_Jewellery_Devotional_Crafting_Suite.md`
  - `Design Documents/Crafting/Medieval_Culture_Catalogue.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Required changes:
  - Implement or classify every `Household and Devotional` culture catalogue reference.
  - Split household room goods from worn jewellery/devotional goods.
  - Ensure culture-specific room goods can make an environment visibly distinct without relying on NPC clothing.
  - Keep existing common medieval furniture as baseline.

Catalogue requirements:
  - At least 5 explicit household/devotional/luxury items per culture.
  - Each culture should include at least two of:
    - household storage/furniture
    - lighting
    - devotional/religious object
    - tableware/luxury vessel
    - trade or craft prop
    - textile or wall furnishing

Craft requirements:
  - Wood goods consume `Furniture Timber Stock` or `Furniture Panel Stock`.
  - Lockable/sealable goods consume `Lockwork Stock`, `Sealing Wax Stock`, or relevant metal stock.
  - Lamps and metal devotional goods consume metal stock and relevant fuel/wick/glass stock where appropriate.
  - Glazed wares consume ceramic/glaze stock.
  - Felt/tent/carpet goods consume textile/felt stock.
  - Reliquaries, icons, pendants, and devotional goods use plausible metal, wood, pigment, textile, or container stock.

Documentation requirements:
  - Update furniture/container docs for room goods.
  - Update jewellery/devotional docs for worn or small devotional goods.
  - Document exact stable references and component choices.

Test requirements:
  - Add a test that every household/devotional catalogue reference is implemented or classified.
  - Add per-culture count tests.
  - Add tests excluding generic `regional devotional token` from explicit culture item counts.
  - Add component checks for containers, lighting, sealable chests, offering/devotional objects, and worn jewellery.

Non-goals:
  - Do not implement rules-aware musical instruments, game sets, or animal tack systems unless already available.
  - Prop versions of these are acceptable if documented as component gaps.

Acceptance criteria:
  - A room furnished with a culture’s goods should have visible identity without relying on NPC clothing.

## GOAL MED-JEWEL-001: Culture-Specific Worn Jewellery and Devotional Accessories

Intent:
  Improve the worn personal jewellery/devotional layer, especially where outfits currently use generated fastener or role-item pieces.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Jewellery_Devotional_Crafting_Suite.md`
  - `Design Documents/Crafting/Medieval_Outfit_Catalogue.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Required changes:
  - Add or refine culture-specific:
    - brooches
    - ring pins
    - cloak clasps
    - pilgrim badges
    - prayer beads
    - icon pendants
    - amulet cords
    - signet rings
    - official badges
    - belt mounts
  - Where outfit fastener items are generated placeholders, either keep them as outfit pieces or alias them to richer jewellery/devotional items.

Catalogue requirements:
  - Every culture should have at least two implemented personal adornment/devotional stable references.
  - At least one should be suitable for outfit `fastener_or_jewellery`.

Craft requirements:
  - Metal jewellery consumes bronze/silver/gold stock.
  - Bead strings consume bead/cord stock.
  - Seal rings use `SealStamp` where appropriate.
  - Devotional containers or reliquaries use container/sealable components where appropriate.

Documentation requirements:
  - Update jewellery/devotional suite with exact culture references.
  - Mark which items are used by outfit definitions.

Test requirements:
  - Add tests that outfit fastener references resolve to suitable jewellery/devotional or fastening items.
  - Add tests that metal jewellery does not use textile craft paths.
  - Add tests that signets/seals have seal components.

Non-goals:
  - Do not add magical/religious mechanics.
  - Do not add denominational specificity beyond what is useful for broad material culture.

Acceptance criteria:
  - Outfit fasteners and devotional accessories feel like meaningful cultural/status markers.

## GOAL MED-PROD-002: Close Production Chains For Explicit Culture Suites

Intent:
  Ensure the explicit non-clothing culture items can be crafted from existing medieval production stock or from new documented upstream stock.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - relevant suite docs
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Required changes:
  - Audit every explicit non-outfit item craft input.
  - Add upstream stock for missing recurring processes.
  - Avoid one-off ad hoc commodity inputs where a reusable stock boundary would be better.

Candidate new stock outputs:
  - `Printed Paper Stock`
  - `Birchbark Sheet Stock`
  - `Wax Tablet Blank`
  - `Icon Panel Stock`
  - `Devotional Bead Stock`
  - `Ceramic Glaze Vessel Blank`
  - `Felt Tent Panel Stock`
  - `Leather Bowcase Stock`
  - `Composite Bow Stave Stock`
  - `Tea Service Stock`
  - `Porcelain Vessel Blank` if the material exists, otherwise `Fine Ceramic Vessel Blank`

Craft requirements:
  - Each new stock output must be consumed downstream or documented as reusable stock.
  - Tools must have seeded TagTool support.

Documentation requirements:
  - Update the audit’s production-chain table.
  - Note any intentionally reusable stock outputs.

Test requirements:
  - Add tests that every new commodity tag is either consumed downstream or documented as reusable stock.
  - Add TagTool coverage tests for new tools.
  - Add representative final craft tests per new chain.

Non-goals:
  - Do not add new primary-production systems for mining, forestry, or agriculture unless already planned.

Acceptance criteria:
  - Explicit culture items do not depend on unexplained stock inputs.

## GOAL MED-TEST-002: Non-Clothing Quality Gates

Intent:
  Extend the quality-gate approach that now works for outfits to food, military, writing, household, devotional, and catalogue materialisation.

Files to touch:
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`
  - `Design Documents/Crafting/Medieval_Testing_Quality_Gates.md`

Required test changes:
  - Keep dispatcher and outfit tests.
  - Add explicit culture catalogue materialisation tests.
  - Add non-clothing count tests per culture and group.
  - Add vocabulary tests by suite.
  - Add food-input sanity tests.
  - Add military-stock input tests.
  - Add writing component sanity tests.
  - Add household/devotional component tests.
  - Add exact documentation tests.
  - Add tests rejecting `regional pattern` for explicit non-clothing final crafts.

Acceptance criteria:
  - A shallow generic matrix cannot pass any medieval content suite.

## GOAL MED-DOC-002: Documentation Status and Review Hygiene

Intent:
  Keep design documents useful as implementation proceeds, without mixing target catalogues, implemented catalogues, and deferred wishlists.

Files to touch:
  - all `Design Documents/Crafting/Medieval_*.md`
  - `Design Documents/README.md` if needed

Required changes:
  - Add status notation to exact catalogue entries:
    - `Implemented`
    - `Implemented as outfit piece`
    - `Alias`
    - `Deferred`
    - `Target only`
  - Remove `Target only` entries from pass criteria once a goal claims completion.
  - Add review notes for each completed goal.

Documentation requirements:
  - `Medieval_Crafting_Audit.md` should remain a summary, not a giant catalogue.
  - `Medieval_Culture_Catalogue.md` should show exact item status.
  - `Medieval_Outfit_Catalogue.md` should show exact outfit references and package/manifest status.
  - Suite-specific docs should show craft chains and implementation details.

Test requirements:
  - Add tests that completed goals do not leave `Target only` entries in their scope.
  - Add tests that exact refs in code and docs match for completed scopes.

Acceptance criteria:
  - Future Codex runs and reviewers can tell what is implemented, what is targeted, and what is deliberately deferred.

## GOAL MED-MED-001: Optional Regional Medical and Apothecary Expansion

Intent:
  Add regional flavour to the medical/apothecary suite without turning it into a full medical-runtime project.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Medical_Apothecary_Crafting_Suite.md`
  - tests

Required changes:
  - Add a small number of regionally flavoured medical/apothecary props and kits.
  - Keep common medical items as baseline.
  - Document component gaps.

Catalogue suggestions:
  - Western/monastic infirmary chest
  - Islamic urban apothecary cabinet
  - Byzantine infirmary lamp or hospital ledger
  - Rus/Steppe travel herb pouch
  - Song scholar-physician box

Craft requirements:
  - Use existing medical/herbal/tool stock where available.
  - Do not add unsupported medical mechanics.

Test requirements:
  - Basic coverage and component sanity tests only.

Non-goals:
  - Do not implement new surgery, disease, or pharmacology systems.

Acceptance criteria:
  - Medical content has enough regional flavour for rooms and props, without requiring new runtime features.

# Suggested Next Task Order

1. `MED-CAT-002` — classify/materialise exact culture references so future work has a clean target ledger.
2. `MED-OUTFIT-008B` — keep explicit outfit pieces on the fail-closed authored source path.
3. `MED-OUTFIT-005` — add starter outfit package/manifest support for builder usability.
4. `MED-WRITE-001` — fix writing/admin components and implement exact culture writing references.
5. `MED-FOOD-001` — implement real culture foodways.
6. `MED-MIL-001` — implement explicit military loadouts.
7. `MED-HOUSE-001` and `MED-JEWEL-001` — implement room goods, devotional goods, and adornment.
8. `MED-PROD-002` — close production-chain gaps uncovered by the previous passes.
9. `MED-TEST-002` and `MED-DOC-002` — keep quality gates and docs aligned as the above goals complete.
