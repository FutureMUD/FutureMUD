# Medieval Implementation Roadmap

This document gives goal-notation instructions for Codex or other coding agents working on the medieval item and craft slice.

The key principle is:

> Shared production chains are encouraged; shared final products are not.

The existing medieval implementation should be treated as a scaffold. It should not be expanded by adding more generic culture/status cross-products. The next work should add exact culture catalogues with explicit stable references, descriptions, craft names, inputs, documentation, and tests.

## Goal Notation Template

Use this format for each implementation task:

```text
GOAL MED-<AREA>-<NUMBER>: <short title>

Intent:
  What player/builder-facing quality this pass is meant to improve.

Files to touch:
  Exact expected files.

Required changes:
  Concrete implementation changes.

Catalogue requirements:
  Exact item-count and stable-reference requirements.

Craft requirements:
  Required craft coverage and input chains.

Documentation requirements:
  Which docs must be updated and how exact the stable-reference listing must be.

Test requirements:
  New or updated tests that must fail before the change and pass after it.

Non-goals:
  Things Codex must not attempt in this pass.

Acceptance criteria:
  Objective checks for reviewer sign-off.
```

## GOAL MED-CAT-001: Establish Exact Medieval Culture Catalogue

Intent:
  Replace generic culture cue expansion with an explicit per-culture catalogue that records named material-culture targets.

Files to touch:
  - `Design Documents/Crafting/Medieval_Culture_Catalogue.md`
  - `Design Documents/Crafting/Medieval_Crafting_Audit.md`
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Required changes:
  - Add a `MedievalCultureCatalogue` model or equivalent explicit catalogue structure.
  - Do not add more culture content by appending `ClothingCue`, `FoodCue`, or `WritingCue` to generic templates.
  - Preserve the current generated common/status wardrobe as common baseline only.
  - Add a testing accessor for explicit per-culture stable references.

Catalogue requirements:
  - For every medieval culture key, the catalogue must list explicit stable references grouped under:
    - Clothing
    - Military
    - Food and Beverage
    - Writing and Administration
    - Household and Devotional
  - Exact references must appear in `Medieval_Culture_Catalogue.md`.
  - Pattern-only documentation is not sufficient for explicit culture items.

Craft requirements:
  - No craft change required beyond any scaffolding needed for explicit final product craft generation.

Documentation requirements:
  - `Medieval_Crafting_Audit.md` must clearly distinguish generic baseline items from explicit culture items.
  - `Medieval_Culture_Catalogue.md` must become the authoritative exact catalogue.

Test requirements:
  - Add a test that fails if a culture has no explicit non-generic catalogue entries.
  - Add a test that no new explicit culture item has a short description beginning with `a regional`.
  - Add a test that exact stable references in the new catalogue are present in code.

Non-goals:
  - Do not implement all culture items in this goal.
  - Do not rewrite production chains yet.

Acceptance criteria:
  - The codebase has a catalogue structure ready for explicit content.
  - The documentation distinguishes generic baseline items from explicit culture items.

## GOAL MED-CLOTH-001: Add Explicit North Atlantic and British Wardrobe Catalogue

Intent:
  Make the six most Anglosphere-relevant cultures materially distinct in clothing rather than generic status outfits with cue text.

Cultures in scope:
  - `early_anglo_saxon`
  - `anglo_danish`
  - `norse`
  - `norman`
  - `high_british`
  - `gaelic`

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Culture_Catalogue.md`
  - `Design Documents/Crafting/Medieval_Clothing_Crafting_Suite.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Catalogue requirements:
  - Add at least 12 explicit named clothing/accessory items per culture.
  - Each culture must include:
    - 4 common/peasant/artisan items
    - 3 merchant/urban/status items
    - 3 noble/formal/religious items
    - 2 military/climate/riding items
  - Stable references must not use only generic status tokens like `work_tunic`, `lined_hat`, or `rough_cloak` unless the item is explicitly marked as common baseline.

Required vocabulary examples:
  - `early_anglo_saxon`: `tablet-banded`, `cloak brooch`, `linen head veil`, `seax belt`
  - `anglo_danish`: `long seax`, `shield-wall`, `reeve`, `panelled`
  - `norse`: `hangerok`, `oval brooch`, `sea cloak`, `runic`, `leg wraps`
  - `norman`: `split riding tunic`, `bliaut`, `mail surcoat`, `nasal`
  - `high_british`: `cote`, `surcoat`, `coif`, `wimple`, `arming`
  - `gaelic`: `brat`, `ring pin`, `léine` or `long shirt`, `bardic`, `pastoral`

Craft requirements:
  - Final craft names must include the named object, e.g. `sew a Norse hangerok apron dress`.
  - Do not use `regional pattern NN` for these final crafts.
  - Use existing medieval textile stocks where appropriate:
    - `Garment Cloth`
    - `Broadcloth Stock`
    - `Embroidered Trim Stock`
    - `Tablet-Woven Band Stock`
    - `Quilted Armour Padding`
    - `Turnshoe Upper Stock`
    - `Spun Yarn`

Test requirements:
  - Add a per-culture explicit clothing count test.
  - Add a vocabulary test for the six cultures.
  - Add a craft-name test rejecting `regional pattern` for explicit culture clothing.

Non-goals:
  - Do not add non-European clothing in this goal.
  - Do not change generic baseline clothing unless needed to separate it clearly from explicit culture items.

Acceptance criteria:
  - A builder searching for Norse, Gaelic, Anglo-Saxon, Norman, or High British clothing can find distinct named items without relying on builder notes.

## GOAL MED-CLOTH-002: Add Explicit Eastern, Near Eastern, North African, Steppe, and Song Wardrobe Catalogue

Intent:
  Bring non-Western and eastern medieval cultures up to the same quality level as the British/North Atlantic pass.

Cultures in scope:
  - `andalusi`
  - `byzantine`
  - `abbasid`
  - `fatimid`
  - `seljuk_ayyubid`
  - `rus_novgorod`
  - `steppe_turkic`
  - `song_china`

Catalogue requirements:
  - Add at least 12 explicit named clothing/accessory items per culture.

Required vocabulary examples:
  - `andalusi`: `qamis`, `sirwal`, `burnous`, `turban`, `tiraz`
  - `byzantine`: `silk dalmatic`, `sagion`, `court belt`, `icon pouch`, `skaramangion`
  - `abbasid`: `qamis`, `qaba`, `caftan`, `scholar robe`, `sash`
  - `fatimid`: `linen robe`, `tiraz-banded`, `cotton wrap`, `court kaftan`
  - `seljuk_ayyubid`: `riding caftan`, `quilted coat`, `high riding boots`, `bowcase belt`
  - `rus_novgorod`: `rubakha`, `fur-edged kaftan`, `onuchi`, `fur hat`, `birchbark`
  - `steppe_turkic`: `felt riding caftan`, `tied riding coat`, `high boots`, `bowcase-and-quiver`
  - `song_china`: `cross-collar`, `scholar robe`, `official cap`, `padded winter robe`, `cloth shoes`

Craft requirements:
  - Same as MED-CLOTH-001: product-specific craft names, no `regional pattern` final craft names.

Test requirements:
  - Add a vocabulary test for these cultures.
  - Add explicit count tests per culture.

Acceptance criteria:
  - None of these cultures should read like Western European garments with a different cue appended.

## GOAL MED-FOOD-001: Replace Culture Food Props With Actual Foodway Items

Intent:
  Make medieval foodways produce actual food/beverage items instead of mostly tableware, trays, packets, and generic cue props.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Food_Beverage_Crafting_Suite.md`
  - `Design Documents/Crafting/Medieval_Culture_Catalogue.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Required changes:
  - Reclassify current platter/bowl/cup objects as tableware or vessel items.
  - Add actual prepared-food and beverage prototypes for each culture.
  - Where possible, use `PreparedFood`-style components if suitable stock components exist; otherwise tag clearly as food props and document the limitation.

Catalogue requirements:
  - Add at least 8 food/beverage items per culture:
    - staple bread or grain dish
    - everyday cooked dish
    - preserved/travel food
    - elite/feast dish
    - beverage
    - dairy/oil/spice/condiment item where culturally appropriate
    - tableware/vessel item
    - market/ration item

Craft requirements:
  - Food crafts must consume food commodities or liquids, not `Furniture Panel Stock`, except for actual platters/trenchers/tableware.
  - Required acceptable inputs include:
    - `Flour Commodity`
    - grain or pulse commodity
    - prepared meat or fish stock
    - `Cheese Curd Stock` or `Cheese Wheel Stock`
    - `Ale Stock`, `Cider Stock`, `Mead Stock`, milk, tea, wine, or water
    - oil, honey, salt, spice, fruit, vegetable, broth, or preserved stock
  - Final craft names must include the food item, not `regional meal platter`.

Test requirements:
  - Add a food-input sanity test that fails if bread/pottage/stew/feast crafts consume only `Furniture Panel Stock`.
  - Add a test that each culture has at least 8 exact foodway stable references.
  - Add a test that `regional medieval meal platter` is not counted as prepared food.

Acceptance criteria:
  - A culture’s foodway feels playable at an inn, market, feast, monastery, camp, or household table.

## GOAL MED-MIL-001: Expand Culture-Specific Military Loadouts

Intent:
  Replace the current one-armour/one-weapon/one-shield model with culturally distinctive military kits.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Equipment_Crafting_Suite.md`
  - `Design Documents/Crafting/Medieval_Culture_Catalogue.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Catalogue requirements:
  - Add at least 8 explicit military/equipment items per culture, excluding the existing generic armour, weapon, shield, padded coif, harness, quiver, pack, and banner.
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
  - Crossbow variants must consume `Crossbow Tiller Stock`, `Crossbow Prod Stock`, `Crossbow Lockwork Stock`, and `Military Cord Stock`.

Test requirements:
  - Add explicit military count tests per culture.
  - Add vocabulary checks for culture-specific weapons and armour.
  - Add tests that crossbow final products consume crossbow-specific stocks.

Acceptance criteria:
  - Each culture has enough military goods to equip multiple roles, not a single representative soldier.

## GOAL MED-WRITE-001: Add Culture-Specific Writing, Administration, and Seal Media

Intent:
  Give each culture appropriate recordkeeping objects and writing surfaces instead of generic bundles and tablets.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Writing_Administration_Crafting_Suite.md`
  - `Design Documents/Crafting/Medieval_Culture_Catalogue.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Catalogue requirements:
  - Add at least 6 explicit writing/admin items per culture.
  - Include different media where appropriate:
    - parchment charters and rolls
    - wax tablets
    - paper decrees/contracts/registers
    - birchbark letters for Rus
    - printed notices/registers for Song China
    - runic trade tallies for Norse
    - seal tags and sealed packets for administrative cultures

Component requirements:
  - Paper/parchment sheets and scrolls use `PaperSheet`-style components.
  - Codices/books use `Book`-style components.
  - Wax, wood, birchbark, and tablet surfaces use `InscribableSurface`-style components where available.
  - Sealed documents use `Sealable`.
  - Seal matrices/stamps use `SealStamp`.

Test requirements:
  - Add a component sanity test so wooden tablets do not use `PaperSheet_Scroll`.
  - Add a per-culture writing/admin count test.
  - Add exact documentation tests.

Acceptance criteria:
  - Builders can distinguish chancery, monastery, market, court, guild, steppe messenger, and Song bureaucratic record objects.

## GOAL MED-HOUSE-001: Add Culture-Specific Household, Devotional, and Luxury Goods

Intent:
  Make non-clothing everyday and prestige goods distinct by culture.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Furniture_Container_Crafting_Suite.md`
  - `Design Documents/Crafting/Medieval_Jewellery_Devotional_Crafting_Suite.md`
  - `Design Documents/Crafting/Medieval_Culture_Catalogue.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Catalogue requirements:
  - Add at least 5 explicit household/devotional/luxury items per culture.
  - Do not satisfy this with `regional devotional token` clones.
  - Include at least two of:
    - household storage/furniture
    - lighting
    - devotional/religious object
    - tableware/luxury vessel
    - trade or craft prop
    - textile or wall furnishing

Test requirements:
  - Add a per-culture household/devotional count test.
  - Add a test excluding generic `regional devotional token` from explicit culture item counts.

Acceptance criteria:
  - A room furnished with a culture’s goods should have visible identity without relying on NPC clothing.

## GOAL MED-TEST-001: Replace Count-Only Tests With Quality Tests

Intent:
  Prevent future agents from passing by generating generic cross-products.

Required test changes:
  - Keep dispatcher and basic coverage tests.
  - Add explicit per-culture catalogue tests.
  - Add vocabulary tests.
  - Add food-input sanity tests.
  - Add component sanity tests for writing surfaces.
  - Add exact documentation tests.
  - Add tests rejecting `regional pattern` craft names for explicit culture final products.

Non-goals:
  - Do not make tests depend on exact item counts only.
  - Do not allow broad family patterns to count as documentation for explicit culture catalogue entries.

Acceptance criteria:
  - A shallow generic matrix cannot pass the medieval content test suite.
