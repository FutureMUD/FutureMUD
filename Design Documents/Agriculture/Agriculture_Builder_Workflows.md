# Agriculture Builder Workflows

Use `field crop validate <id|name>`, `field woodland validate <id|name>` and `field operation validate <id|name>` for read-only preflight. Crop and woodland checks inspect timings, ranges and output materials/tags/weights; harvest-apiary checks inspect multiplier, cost and outputs. Unsupported operation validators are reported explicitly. These checks neither activate definitions nor execute preview progs. See the [shared builder guide](../Building/Generic_Editable_Item_Builder_Workflows.md) for result semantics.

## Seed First
Run `AgricultureSeeder` after core utility progs exist. It installs:

- stock field profiles for arable fields, garden beds, orchards/groves, pasture, wet fields, rocky fields, saline coastal fields, and managed woodland
- additional cultivated, marginal, natural, and unimproved field profiles for paddies, dryland fields, terraces, floodplains, old fallows, scrub, woodland clearings, marshes, saline edges, heavy clay, eroded slopes, and similar land-expansion starting points
- a cultivated-field quality ladder from exhausted cropland through average, manured, irrigated, estate-quality, and paradise-garden profiles, plus international profiles such as chinampas, Andean raised fields, Sahel millet fields, monsoon rice terraces, mangrove rice polders, desert wadis, slash-and-burn swiddens, kraal-manured gardens, and laterite upland gardens
- specific annual crop and perennial orchard/vineyard/plantation definitions with seed requirements, commodity outputs, and concrete pollination metadata, including broad temperate, tropical, dryland, wetland, archaic, Americas, African, and medieval-to-early-industrial crop examples
- species-specific herd definitions with secondary product outputs for milk, wool, eggs, and manure, including cattle, oxen, buffalo, bison, sheep, goats, pigs, horses, donkeys, mules, camels, llamas, alpacas, reindeer, rabbits, guinea pigs, common poultry, game birds, peafowl, ostriches, emus, and rheas
- specific managed woodland definitions with commodity outputs, including dye, gum, resin, tannin, thatch, fibre/cane, parkland, shelterbelt, charcoal, wetland, temperate coppice, American stand, African parkland, and tropical timber products
- stock operations, including woodland and herd yield multipliers, yield costs, apiary install/tend/harvest/removal operations, and land-management families for soil preparation, weed control, irrigation, drainage, salinity remediation, fertility amendment, erosion control, raised beds and terracing, pasture infrastructure, orchard care, woodland management, and land clearing or reclamation
- local project templates with agriculture completion actions, seed stock material requirements, apiary installation requirements, deliberately substantial labour requirements, and Farming-based supervision labour

The stock package is idempotent. Reruns refresh stock-owned rows by stable names instead of duplicating them.

The stock agriculture catalogue is intentionally broad rather than setting-specific. Builders can use the installed profiles as starter land conditions and then tune or delete examples that do not fit their world. It includes an `Apiary Yard` profile for dedicated beeyards, but apiaries can also be installed on suitable crop, orchard, pasture, woodland, or fallow fields.

## Terrain Defaults
Use terrain defaults to make field creation fast:

```text
terrain set agriculture <profile>
terrain set agriculture none
```

This only sets the terrain default. It does not create fields in existing cells.

## Creating and Maintaining Fields
Stand in the target cell and use:

```text
field create [profile]
field delete
field look
field set <score> <0-100>
field reset
field tick
```

`field create` uses the explicit profile when supplied, otherwise the current terrain's default agriculture profile. `field reset` restores profile default scores. `field tick` lets an administrator force the daily agriculture update for testing.

## Editing Definition Content
Definition commands follow the same pattern:

```text
field profile list|show|create|set|delete
field crop list|show|create|set|delete
field herds list|show|create|set|delete
field woodland list|show|create|set|delete
field operation list|show|create|set|delete
field scoretype list|set|disable
```

Useful examples:

```text
field profile set Pasture score Fence 80
field profile set Pasture use Crop on
field crop set "Wheat" growth 115
field crop set "Wheat" moisture 35 80
field crop set "Wheat" planting group Autumn Spring
field crop set "Moonwheat" planting season "Early Thaw"
field crop set "Moonwheat" planting none
field crop set "Moonwheat" output add 180 "Wheat Grain" tag "Seeded Yield"
field crop set "Moonwheat" output list
field crop set "Moonwheat" output set 1 220 "Wheat Grain" tag "Seeded Yield"
field crop set "Moonwheat" output remove 1
field crop set "Moonwheat" seeds add 30 "Wheat Grain" tag Seeds
field crop set "Moonwheat" seeds clear
field crop set "Apples" pollination Strong 1 2
field crop set "Apples" perennial true
field crop set "Apples" cycle 220
field herds set "Cattle Herd" npc cattle
field herds set "Cattle Herd" output add 3500 milk tag Raw Milk
field woodland set "Hazel Coppice" cycle 365
field woodland set "Hazel Coppice" output add 2400 "Hazel Wood" tag "Seeded Yield"
field woodland set "Hazel Coppice" output list
field woodland set "Hazel Coppice" output set 1 2600 "Hazel Wood" tag "Seeded Yield"
field woodland set "Hazel Coppice" output remove 1
field woodland set "Hazel Coppice" output clear
field operation set "Sow Crop" delta Moisture -3
field operation set "Sow Crop" project "Stock Agriculture: Sow Crop"
field operation set "Coppice Woodland" woodlandyield 0.45 45
field operation set "Collect Herd Products" herdyield 1.0 55
field operation set "Install Apiary" allowed Crop on
field operation set "Install Apiary" apiaryinstall 2 3
field operation set "Tend Apiary" apiarytend 5 10 -2
field operation set "Harvest Apiary" apiaryyield 1.0 35 add 600 "Raw Honeycomb" tag "Apiary Product"
field operation set "Harvest Apiary" apiaryyield 1.0 35 list
```

Crop and woodland `output` lists and crop `seeds` lists are edited with 1-based indices. `add` and `set` take a positive finite weight in grams, a material, and an optional `tag <tag name>` selector; quote material or tag names containing spaces. `clear` removes the full list. These selectors must resolve to existing materials and tags. `apiaryyield` takes a positive finite multiplier and a 0-100 stores/yield cost before its list action. A `HarvestApiary` operation must have a positive multiplier and at least one valid output to apply. `apiaryinstall` takes a positive hive count and non-negative pollination radius; `apiarytend` takes signed health, stores, and yield deltas, so negative deltas remain valid.

Crop `SeedRequirements` are definition metadata. They do not automatically become project material requirements or get consumed by sowing. Configure seed consumption independently on the sowing project's material requirements; this keeps the crop's descriptive seed recipe separate from the project template that controls what workers must supply.

For a persistence and harvest acceptance pass, create a crop and woodland, author at least one output on each and seed metadata on the crop, save/reload their definitions, then apply a harvest-ready `Harvest` operation and a mature woodland yield operation through `IAgricultureField.ApplyOperation`. Assert the resulting commodity piles use the authored material, tag and weight scaling. For apiaries, create a `HarvestApiary` operation with output, save/reload it, install an apiary with `InstallApiary`, raise stores/yield potential, then apply harvest and assert piles; separately verify empty outputs, zero multiplier and invalid material/tag references make `WhyCannotApply` reject the operation. Existing runtime seams are `AgricultureField.ApplyOperation`, `AgricultureOperation.WhyCannotApply`, and the `AgricultureOperationTests` fixture patterns.

Deleting a definition is blocked while live fields or active agriculture contexts still reference it.

## Custom Score Types
Games can enable up to twelve custom agriculture score slots for setting-specific growing pressures:

```text
field scoretype list
field scoretype set custom1 good "Ley Saturation"
field scoretype set custom2 bad "Hard Radiation"
field scoretype disable custom1
```

The `good` or `bad` flag tells agriculture whether higher values are beneficial or harmful for project delta scaling. Enabled custom scores appear anywhere ordinary field scores are accepted:

```text
field set "Ley Saturation" 65
field profile set "Arable Field" score "Ley Saturation" 50
field crop set "Moonwheat" score "Ley Saturation" 40 85
field crop set "Moonwheat" score "Ley Saturation" none
field operation set "Irradiate Field" delta "Hard Radiation" 8
```

Custom score labels are presentation and parsing aliases over stable enum slots like `Custom1`. Existing XML keeps using the slot name, so renaming a custom score does not break saved data.

## Planting Seasons
Crop definitions can restrict when they may be planted:

```text
field crop set "Wheat" planting group Autumn Spring
field crop set "Moonwheat" planting season "Early Thaw" "Mid Thaw"
field crop set "Moonwheat" planting none
```

Group windows are broad labels such as Winter, Spring, Summer, and Autumn. They match the current local season's group when the cell has weather. If there is no local weather season, group windows use the `AgricultureSeasonGroupWindows` static configuration and the current celestial-year fraction as a fallback.

Season windows are exact local season names or display names. Use these when a game has detailed local seasons and a crop should only be planted during specific named seasons.

Planting windows are hard gates for `Sow Crop` and `Plant Orchard`. They are checked when the field project starts and again when the project completion action applies the operation.

## Apiaries and Pollination
Apiaries are installed on fields as adjunct state instead of changing the field's primary use. This lets hives support nearby crops, orchards, pasture edges, woodland clearings, or a dedicated `Apiary Yard` without making bees compete with the land use they support.

Crop definitions can declare how much they care about pollination:

```text
field crop set "Wheat" pollination None
field crop set "Apples" pollination Strong 1 2
field crop set "Cucumbers" pollination Required 1 2
```

The optional numbers are capped daily health and yield bonuses while the crop is `Growing` or `Setting`. Required crops take a stress penalty during `Setting` when no happy apiary is in range.

Apiary operations use `AllowedUses`, so a single operation can run on fallow, crop, orchard, pasture, or woodland fields:

```text
field operation set "Install Apiary" allowed Crop on
field operation set "Install Apiary" allowed Orchard on
field operation set "Harvest Apiary" allowed Woodland on
```

The stock install project consumes two bee hives and one hive stand. Tend, harvest, and remove apiary operations are labour and skill operations; their reusable tools are represented through ordinary item and craft paths rather than consumed project materials.

## Starting Field Work
Players and admins start work with:

```text
field start <operation> [target]
field harvest
```

Targets depend on the operation definition:

- crop operations target a crop definition
- orchard planting operations target a perennial crop definition
- herd operations target a herd definition
- woodland operations target a woodland definition
- improvement and clearing operations may have no target

The command starts the operation's local project. Workers still use the ordinary project commands to join labour and supply materials.

## Farming Skill and Supervision
Agriculture projects record the Farming skill of workers as they contribute hours. The completion action uses the weighted result to adjust the field operation's final impact, including crop establishment, score deltas, harvested commodity weights, seed recovery, and commodity quality.

Stock project templates include:

- `Field Labour`: mandatory general labour that supplies the required hours
- `Agricultural Supervision`: optional `supervision` labour requiring Farming 15+, capped at one worker, with a skill-scaled multiplier up to 135%

Use the supervision role when a skilled farmer should coordinate lower-skilled hired labour. Builders can tune this through the normal project labour commands, especially `trait`, `minskill`, `difficulty`, `multiplier`, and `scaled`.

## Pasture and Herds
Use herd operations to put abstract livestock onto a field. Once a field has abstract herds:

```text
field herd draw <herd> [count]
field herd absorb <npc> <herd>
field herd drive <herd> <direction> [count]
field start "Collect Herd Products" <herd>
```

Drawdown requires the herd definition to have an NPC template. Builders may select that template by numeric id or by its unique template name. Absorb is for turning live livestock back into abstract field stock after validation.

Drawn livestock recognise the drawing character as an ally, allowing that character to return them to matching stock. Other animals must explicitly recognise the absorbing character as an ally, or be handled by an administrator. Absorb requires the configured NPC template to match and refuses active riding/drag relationships; template-less definitions do not admit live animals.

Driving a herd moves abstract animals from the current field into an adjacent field through the named exit. Omit the count, or use `all`, to move the whole herd. The destination field must already exist, support pasture use, and be fallow or pasture. Herders can drive animals into unowned fields or fields they are authorised to use, which allows wild grazing grounds and semi-nomadic pastoral movement without making owned fields freely available.

Herd definitions can also define per-head secondary commodity outputs. Stock herds use this to provide raw milk, wool, eggs, and manure from species-specific definitions rather than mixed livestock placeholders; the horse herd remains the stock pastoral milk source for kumis-facing cultures. Fed herds build secondary yield potential on the daily tick, and `Collect Herd Products` releases the configured commodities without changing the pasture use.

## Managed Woodland
Managed woodland is a field use rather than a separate subsystem. Builders can represent:

- coppice stands
- pollarded groves
- timber stands
- clearing woodland back to fallow

Orchards, vineyards, nut groves, and plantation crops use the separate `Orchard` field use. They still use crop definitions, but those definitions are marked perennial and keep their crop state after harvest.

Use woodland definitions for growth cadence and operations for planting, coppicing, thinning, felling, and clearing.

Stock woodland definitions also define commodity outputs. Woodland operations can release a fraction of the stand's products and consume part of the stand's current yield, so coppicing, thinning, felling, tapping, bark stripping, gathering, charcoal burning, and clearing can all be tuned separately. The stock examples cover dye products such as kermes grain, orchil lichen, and lac dye cake, plus gum, resin, bark, thatch, fibre/cane, charcoal, parkland nuts and fruit, wetland woods, shelterbelts, and tropical timber as managed woodland products.

Crafts can use the `field` craft input when a craft should require a local field state rather than a loose commodity pile. This is useful for actions such as collecting firewood, cutting poles, tapping or gathering from a managed grove, or any craft that should require a particular crop, orchard, or woodland, minimum health, minimum yield, and optionally consume some of that yield.

## Seeds and Planting Stock
Seed stock is modelled as commodity piles with the secondary `Seeds` tag. Seedable crop materials are tagged `Agriculture Seedable`, and stock sowing/planting projects require `commoditytag` material requirements that accept any commodity whose material has that tag and whose pile tag is `Seeds`.

Stock harvests also produce a small seed-tagged commodity output, and they tag the main yield as `Seeded Yield`. The active antiquity `select antiquity seed stock` craft takes `Seeded Yield` agricultural commodity and returns a smaller `Seeds` commodity pile of the same material, which lets harvested grain, pulses, roots, orchard crops, spice crops, and dye crops bootstrap future sowing projects. The companion compost craft turns raw herd manure and crop refuse into reusable compost stock for games that want a visible manure-to-fertiliser economy.

## Project Authoring Notes
Agriculture operations should use local projects. Put labour, tools, seeds, fencing, fertiliser, drainage materials, and other inputs on the project template. The operation only stores the field-side effects and dynamic target context.

Use `commoditytag` project material requirements when the project needs a class of commodity material rather than one exact material. The stock `Sow Crop` and `Plant Orchard` templates use this for seed stock.

Stock project templates use higher labour totals and smaller score deltas than early prototypes. This is intentional: field improvement should be gradual enough that offline projects and NPC labour matter without making near-perfect fields appear after only a few person-days.

Every agriculture project template should include an `agriculture` project completion action. The stock seeder creates examples for all stock operations.
