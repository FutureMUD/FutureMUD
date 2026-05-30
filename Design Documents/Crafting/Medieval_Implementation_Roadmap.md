# Medieval Implementation Roadmap

This roadmap records the current implementation status after MED-CAT-001, MED-CAT-002, and MED-OUTFIT-001 through MED-OUTFIT-008, then defines the next goals for the medieval item and craft suite.

The key principle remains:

> Shared production chains are encouraged; shared final outfits and culture-visible final goods are not.

The completed outfit work has moved beyond the original generic status/culture matrix. The next work should now shift from package usability, layering validation, and non-clothing culture suites.

## Review Status After MED-OUTFIT-008

| Goal | Status | Review |
| --- | --- | --- |
| `MED-CAT-001` | Met | The code has an explicit medieval culture catalogue structure and exact documentation/test alignment. |
| `MED-OUTFIT-001` | Met | The code has outfit slot, outfit spec, and outfit-piece structures, with testing accessors. |
| `MED-OUTFIT-002` | Met | North Atlantic/British cultures have complete outfit rows and vocabulary tests. |
| `MED-OUTFIT-003` | Met | Continental Western/Central European cultures have complete outfit rows and vocabulary tests. |
| `MED-OUTFIT-004` | Met | Byzantine, Islamic, Rus, Steppe, and Song cultures have complete outfit rows and vocabulary tests. |
| `MED-OUTFIT-005` | Not done | Starter outfit package support remains a stub and should now be implemented or explicitly rejected after investigation. |
| `MED-OUTFIT-006` | Superseded by MED-OUTFIT-008 | The old selected-row polish pass has been replaced by full authored catalogue coverage. |
| `MED-OUTFIT-007` | Not done | Layering and wear-slot validation still needs a live component/wearability audit. |
| `MED-OUTFIT-008` | Met | Generated/override clothing has been replaced by a single authored clothing/outfit catalogue using shared era records and configuration. |
| `MED-CAT-002` | Met | Exact medieval culture catalogue references are classified as implemented, covered by outfit pieces, aliases, or deferred. |
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
- single-source authored clothing seeding through `MedievalClothingItemSpecs()` and `SeedEraItemSpecs(...)`
- shared era definition records in `ItemSeeder.Rework.EraDefinitions.cs`
- medieval and antiquity `EraSeederConfiguration` entries that express outfit completeness, generic baseline, culture tag, variable-colour, and material-stock policy as data
- explicit outfit-piece descriptions without generated catalogue or slot prose
- variable-colour components and craft product mappings for colourable outfit garments
- explicit outfit-piece crafts using named object craft names rather than `regional pattern`
- material-stock tests for several reviewed outfit-piece edge cases

This is enough to say MED-OUTFIT-001 through MED-OUTFIT-008 are functionally met, except for the separately scoped package and layering follow-ups.

### Caveat 1: explicit culture catalogue targets outside outfits are classified but not all implemented

The explicit culture catalogue contains military, food/beverage, writing/admin, and household/devotional stable references. Each reference is classified, but many remain aliases or deferred targets. Future suite work should convert the highest-value deferred entries into real item/craft content.

### Caveat 2: some non-paper writing surfaces still use paper-like components

The suite still needs a writing-component sanity pass. Wax tablets, wooden tablets, birchbark letters, and non-paper tablets should use `InscribableSurface`-style components where available rather than scroll/paper components.

### Caveat 3: generic military crafts still use regional-pattern names

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
  - MED-OUTFIT-008 completed this by replacing generated outfit descriptions and selected-row patching with a single authored catalogue.

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

## GOAL MED-OUTFIT-008: Collapse Generated/Override Clothing Into Authored Era Catalogue

Status:
  Completed.

Keep:
  - `ItemSeeder.Rework.EraDefinitions.cs` as the shared record/configuration home for era seeder concepts.
  - `MedievalClothingItemSpecs()` as the single source for explicit outfit-piece item specs.
  - `SeedEraItemSpecs(...)` for shared spec-driven seeding.
  - `EraSeederConfiguration` entries for both medieval and antiquity, with era differences expressed as data.
  - variable-colour item components and craft product mappings for colourable medieval garments.

Follow-up:
  - `MED-OUTFIT-005` still owns starter outfit package support.
  - `MED-OUTFIT-007` still owns live layering and wear-slot compatibility.

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

Status:
  Superseded by MED-OUTFIT-008.

Result:
  The selected-row polish model is no longer the maintained architecture. Explicit outfit pieces now resolve through a single authored catalogue path, with final descriptions, material/component data, variable-colour components, craft stock inputs, and outfit metadata available through the same seeder source seam.

Keep:
  - no generated player-facing outfit descriptions
  - no generated-then-patched outfit item flow
  - colourable garments carry variable colour components and matching `$colour`, `$colour1`, or `$colour2` tokens
  - craft products for colourable garments use variable product mappings
  - non-colourable role items are limited to rigid, paper, book, tag, token, liquid, or hardware-style objects
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

1. `MED-OUTFIT-005` - add starter outfit package/manifest support for builder usability.
2. `MED-OUTFIT-007` - audit layering and wear-slot compatibility for complete outfits.
3. `MED-WRITE-001` - fix writing/admin components and implement exact culture writing references.
4. `MED-FOOD-001` - implement real culture foodways.
5. `MED-MIL-001` - implement explicit military loadouts.
6. `MED-HOUSE-001` and `MED-JEWEL-001` - implement room goods, devotional goods, and adornment.
7. `MED-PROD-002` - close production-chain gaps uncovered by the previous passes.
8. `MED-TEST-002` and `MED-DOC-002` - keep quality gates and docs aligned as the above goals complete.
