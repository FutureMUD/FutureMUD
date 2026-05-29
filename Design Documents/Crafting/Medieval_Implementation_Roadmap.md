# Medieval Implementation Roadmap

This document gives goal-notation instructions for Codex or other coding agents working on the medieval item and craft slice.

The key principle is:

> Shared production chains are encouraged; shared final outfits are not.

The existing medieval implementation should be treated as a scaffold. It should not be expanded by adding more generic culture/status cross-products. The next work should add complete outfit catalogues by culture, sex, and social class, with explicit stable references, descriptions, craft names, inputs, documentation, and tests.

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
  Exact outfit, item-count, and stable-reference requirements.

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

## GOAL MED-CAT-001: Establish Exact Medieval Culture Catalogue

Status:
  Implemented in the first pass. Retain the model, but extend it for outfit-level catalogues.

Follow-up requirement:
  Add outfit definitions and testing accessors. The catalogue should distinguish:
  - generic baseline clothing
  - shared common clothing
  - explicit culture-specific clothing
  - complete outfit definitions

## GOAL MED-OUTFIT-001: Add Complete Outfit Catalogue Model

Intent:
  Replace "a few culturally specific garments" with complete dressable outfits for men and women across social classes.

Files to touch:
  - `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs`
  - `DatabaseSeeder/Seeders/ItemSeederCrafting.Medieval.cs`
  - `Design Documents/Crafting/Medieval_Outfit_Catalogue.md`
  - `Design Documents/Crafting/Medieval_Clothing_Crafting_Suite.md`
  - `Design Documents/Crafting/Medieval_Testing_Quality_Gates.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Required changes:
  - Add a `MedievalOutfitSpec` or equivalent catalogue structure.
  - Each outfit spec should identify:
    - culture key
    - sex/gender presentation: `male` or `female`
    - social class/role
    - display name
    - slot-to-item stable reference map
    - intentionally shared/generic slots, if any
  - Add a `MedievalOutfitSlot` model or equivalent slot list.
  - Add testing accessors exposing outfit definitions and their item references.

Required outfit axes:
  - Cultures: all 18 medieval cultures.
  - Sex/gender presentation: `male`, `female`.
  - Social classes/roles: `peasant`, `artisan`, `merchant`, `noble`, `religious`, `military`.

Catalogue requirements:
  - 12 complete outfits per culture.
  - 216 complete outfits across all cultures.
  - Every outfit should include required slots:
    - underlayer
    - lower body
    - leg/sock layer
    - footwear
    - bodywear
    - outerwear or documented warm-weather/travel equivalent
    - headwear
    - belt or sash
    - worn container
    - fastener/jewellery
    - role item for merchant, religious, and military outfits

Craft requirements:
  - No need to create a single "outfit craft" unless useful.
  - Every item referenced by an outfit must be craftable or intentionally shared from a craftable common item.

Documentation requirements:
  - `Medieval_Outfit_Catalogue.md` must list every outfit by exact outfit reference.
  - The clothing suite doc must explain outfit slots, sharing rules, and tests.

Test requirements:
  - Add tests for outfit count, required slots, referenced item existence, and documentation.
  - Add tests that explicit outfit final item crafts do not use `regional pattern`.

Non-goals:
  - Do not implement all outfit items in this goal if scaffolding alone is being added.
  - Do not create runtime package support unless an existing package system makes it straightforward.

Acceptance criteria:
  - Codex and reviewers can inspect an outfit catalogue and see exactly what someone wears from head to foot.

## GOAL MED-OUTFIT-002: Implement Complete North Atlantic and British Outfits

Intent:
  Make the highest-priority Anglosphere-adjacent cultures fully dressable with complete male and female outfits across social classes.

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
  - `Design Documents/Crafting/Medieval_Outfit_Catalogue.md`
  - `Design Documents/Crafting/Medieval_Clothing_Crafting_Suite.md`
  - `DatabaseSeeder Unit Tests/ItemSeederMedievalCraftingTests.cs`

Catalogue requirements:
  - Implement 12 complete outfits per culture.
  - Each outfit must include at least 9 wearable pieces or documented slot equivalents.
  - Each outfit must include at least four culture-specific or culture-cluster-specific items.
  - Male and female variants for the same culture/class must differ in at least two wearable slots unless marked intentionally unisex.
  - Class variants must differ in at least two wearable slots.

Required vocabulary examples:
  - `early_anglo_saxon`: `tablet-banded`, `cloak brooch`, `linen head veil`, `seax belt`
  - `anglo_danish`: `long seax`, `shield-wall`, `panelled`, `reeve`
  - `norse`: `hangerok`, `oval brooch`, `sea cloak`, `runic`, `leg wraps`
  - `norman`: `split riding tunic`, `bliaut`, `mail surcoat`, `nasal`
  - `high_british`: `cote`, `surcoat`, `coif`, `wimple`, `arming`
  - `gaelic`: `brat`, `ring pin`, `léine` or `long shirt`, `bardic`, `pastoral`

Craft requirements:
  - Final craft names must include the named object, e.g. `sew a Norse hangerok apron dress`.
  - Do not use `regional pattern NN` for explicit outfit-item final crafts.
  - Use existing medieval textile stocks where appropriate:
    - `Garment Cloth`
    - `Broadcloth Stock`
    - `Embroidered Trim Stock`
    - `Tablet-Woven Band Stock`
    - `Quilted Armour Padding`
    - `Turnshoe Upper Stock`
    - `Spun Yarn`

Test requirements:
  - Add per-culture outfit completion tests.
  - Add vocabulary tests for the six cultures.
  - Add craft-name tests rejecting `regional pattern` for explicit outfit items.
  - Add tests that every outfit has footwear, headwear, bodywear, and belt/sash.

Non-goals:
  - Do not add eastern or Near Eastern complete outfits in this goal.
  - Do not change non-clothing suites except for references needed by military outfit role items.

Acceptance criteria:
  - A builder can dress male and female peasant, artisan, merchant, noble, religious, and military characters for each in-scope culture without improvising missing clothing.

## GOAL MED-OUTFIT-003: Implement Complete Continental Western and Central European Outfits

Intent:
  Bring the remaining Western and Central European cultures up to the complete-outfit standard.

Cultures in scope:
  - `carolingian`
  - `capetian`
  - `german_hre`
  - `iberian_christian`

Catalogue requirements:
  - Same as MED-OUTFIT-002: 12 complete outfits per culture, required slots, culture/class/sex differentiation.

Required vocabulary examples:
  - `carolingian`: high-belted tunic, broad-banded mantle, spatha belt, capitulary/monastic associations
  - `capetian`: cote, bliaut, burgher gown, wimple, guild apron
  - `german_hre`: guild apron, civic gown, alpine felt cap, fur-lined mantle, town crossbow/militia
  - `iberian_christian`: saya, pellote, manto, toca, frontier riding cloak

Craft requirements:
  - Same as MED-OUTFIT-002.
  - Iberian outfits may share some cloak, sash, and textile logic with Andalusi outfits, but final pieces must remain culturally distinct.

Acceptance criteria:
  - These cultures should not feel like minor edits of the English/French baseline.

## GOAL MED-OUTFIT-004: Implement Complete Byzantine, Islamic, Rus, Steppe, and Song Outfits

Intent:
  Bring the eastern, Near Eastern, North African, steppe, and Chinese slices up to the complete-outfit standard.

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
  - Same as MED-OUTFIT-002: 12 complete outfits per culture, required slots, culture/class/sex differentiation.

Required vocabulary examples:
  - `andalusi`: qamis, sirwal, burnous, turban, tiraz
  - `byzantine`: silk dalmatic, sagion, court belt, icon pouch, skaramangion
  - `abbasid`: qamis, qaba, caftan, scholar robe, sash
  - `fatimid`: linen robe, tiraz-banded, cotton wrap, court kaftan
  - `seljuk_ayyubid`: riding caftan, quilted coat, high riding boots, bowcase belt
  - `rus_novgorod`: rubakha, fur-edged kaftan, onuchi, fur hat, birchbark
  - `steppe_turkic`: felt riding caftan, tied riding coat, high boots, bowcase-and-quiver
  - `song_china`: cross-collar robe, scholar robe, official cap, padded winter robe, cloth shoes

Craft requirements:
  - Product-specific final craft names.
  - No `regional pattern` final craft names.
  - Use culture-appropriate material stocks: linen/cotton/silk/paper/lamellar/felt/fur where appropriate.

Acceptance criteria:
  - None of these cultures should read like Western European garments with a different cue appended.

## GOAL MED-OUTFIT-005: Add Optional Starter Outfit Package Support

Intent:
  Let builders load or inspect complete outfits as coherent sets if an existing package mechanism is convenient.

Required changes:
  - Investigate existing item package support.
  - If suitable, seed package definitions or package metadata for each `medieval_outfit_{culture}_{sex}_{class}`.
  - If not suitable, keep outfit specs as internal catalogue/test data only.

Non-goals:
  - Do not implement new runtime item-package systems unless already planned.
  - Do not block outfit item implementation on package support.

Acceptance criteria:
  - At minimum, tests and docs can resolve every outfit to item stable references.
  - If package support exists, builders can load complete outfits more conveniently.

## GOAL MED-FOOD-001: Replace Culture Food Props With Actual Foodway Items

Intent:
  Make medieval foodways produce actual food/beverage items instead of mostly tableware, trays, packets, and generic cue props.

This goal remains valid from the previous roadmap. It should occur after or in parallel with outfit work, but outfit work has higher priority for the clothing suite.

## GOAL MED-MIL-001: Expand Culture-Specific Military Loadouts

Intent:
  Replace the current one-armour/one-weapon/one-shield model with culturally distinctive military kits.

This goal should now coordinate with outfit work. The `military` outfit class for each culture should reference military clothing and at least one appropriate equipment accessory, but armour and weapons may remain in the equipment suite.

## GOAL MED-WRITE-001: Add Culture-Specific Writing, Administration, and Seal Media

Intent:
  Give each culture appropriate recordkeeping objects and writing surfaces instead of generic bundles and tablets.

This goal remains valid. It should support outfit role items such as book pouches, document pouches, scholar sleeve pouches, notary kits, or official document cases.

## GOAL MED-HOUSE-001: Add Culture-Specific Household, Devotional, and Luxury Goods

Intent:
  Make non-clothing everyday and prestige goods distinct by culture.

This goal remains valid. It should support outfit role items such as devotional pendants, reliquary lockets, pilgrim badges, icon pouches, official badges, or merchant seals.

## GOAL MED-TEST-001: Replace Count-Only Tests With Outfit Quality Tests

Intent:
  Prevent future agents from passing by generating generic cross-products.

Required test changes:
  - Keep dispatcher and basic coverage tests.
  - Add outfit count tests.
  - Add outfit slot completeness tests.
  - Add culture-specific item threshold tests.
  - Add sex differentiation tests.
  - Add class differentiation tests.
  - Add vocabulary tests.
  - Add craft-name quality tests.
  - Add exact outfit documentation tests.

Acceptance criteria:
  - A shallow generic matrix cannot pass the medieval clothing and outfit test suite.
