# Agriculture Builder Workflows

## Seed First
Run `AgricultureSeeder` after core utility progs exist. It installs:

- stock field profiles for arable fields, garden beds, orchards/groves, pasture, wet fields, rocky fields, saline coastal fields, and managed woodland
- specific annual crop and perennial orchard/vineyard definitions with seed requirements and commodity outputs
- generic herd definitions
- specific managed woodland definitions with commodity outputs
- stock operations, including woodland yield multipliers and yield costs
- local project templates with agriculture completion actions, seed stock material requirements, deliberately substantial labour requirements, and Farming-based supervision labour

The stock package is idempotent. Reruns refresh stock-owned rows by stable names instead of duplicating them.

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
field crop set "Apples" perennial true
field crop set "Apples" cycle 220
field herds set "Cattle Herd" npc cattle
field woodland set "Hazel Coppice" cycle 365
field operation set "Sow Crop" delta Moisture -3
field operation set "Sow Crop" project "Stock Agriculture: Sow Crop"
field operation set "Coppice Woodland" woodlandyield 0.45 45
```

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
```

Drawdown requires the herd definition to have an NPC template. Absorb is for turning live livestock back into abstract field stock after validation.

## Managed Woodland
Managed woodland is a field use rather than a separate subsystem. Builders can represent:

- coppice stands
- pollarded groves
- timber stands
- clearing woodland back to fallow

Orchards, vineyards, nut groves, and plantation crops use the separate `Orchard` field use. They still use crop definitions, but those definitions are marked perennial and keep their crop state after harvest.

Use woodland definitions for growth cadence and operations for planting, coppicing, thinning, felling, and clearing.

Stock woodland definitions also define commodity outputs. Woodland operations can release a fraction of the stand's products and consume part of the stand's current yield, so coppicing, thinning, felling, and clearing can all be tuned separately.

Crafts can use the `field` craft input when a craft should require a local field state rather than a loose commodity pile. This is useful for actions such as collecting firewood, cutting poles, tapping or gathering from a managed grove, or any craft that should require a particular crop, orchard, or woodland, minimum health, minimum yield, and optionally consume some of that yield.

## Seeds and Planting Stock
Seed stock is modelled as commodity piles with the secondary `Seeds` tag. Seedable crop materials are tagged `Agriculture Seedable`, and stock sowing/planting projects require `commoditytag` material requirements that accept any commodity whose material has that tag and whose pile tag is `Seeds`.

Stock harvests also produce a small seed-tagged commodity output, and they tag the main yield as `Seeded Yield`. The generic `select seed stock` craft takes `Seeded Yield` agricultural commodity and returns a smaller `Seeds` commodity pile of the same material.

## Project Authoring Notes
Agriculture operations should use local projects. Put labour, tools, seeds, fencing, fertiliser, drainage materials, and other inputs on the project template. The operation only stores the field-side effects and dynamic target context.

Use `commoditytag` project material requirements when the project needs a class of commodity material rather than one exact material. The stock `Sow Crop` and `Plant Orchard` templates use this for seed stock.

Stock project templates use higher labour totals and smaller score deltas than early prototypes. This is intentional: field improvement should be gradual enough that offline projects and NPC labour matter without making near-perfect fields appear after only a few person-days.

Every agriculture project template should include an `agriculture` project completion action. The stock seeder creates examples for all stock operations.
