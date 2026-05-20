# Agriculture Builder Workflows

## Seed First
Run `AgricultureSeeder` after core utility progs exist. It installs:

- stock field profiles for arable fields, garden beds, orchards/groves, pasture, wet fields, rocky fields, saline coastal fields, and managed woodland
- generic crop definitions
- generic herd definitions
- generic woodland definitions
- stock operations
- local project templates with agriculture completion actions

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
```

Useful examples:

```text
field profile set Pasture score Fence 80
field profile set Pasture use Crop on
field crop set "Cereal Grain" growth 100
field crop set "Cereal Grain" moisture 30 75
field herds set "Cattle Herd" npc cattle
field woodland set "Coppice Woodland" cycle 365
field operation set "Sow Crop" delta Moisture -3
field operation set "Sow Crop" project "Stock Agriculture: Sow Crop"
```

Deleting a definition is blocked while live fields or active agriculture contexts still reference it.

## Starting Field Work
Players and admins start work with:

```text
field start <operation> [target]
field harvest
```

Targets depend on the operation definition:

- crop operations target a crop definition
- herd operations target a herd definition
- woodland operations target a woodland definition
- improvement and clearing operations may have no target

The command starts the operation's local project. Workers still use the ordinary project commands to join labour and supply materials.

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
- orchard-like groves
- clearing woodland back to fallow

Use woodland definitions for growth cadence and operations for planting, coppicing, thinning, felling, and clearing.

## Project Authoring Notes
Agriculture operations should use local projects. Put labour, tools, seeds, fencing, fertiliser, drainage materials, and other inputs on the project template. The operation only stores the field-side effects and dynamic target context.

Every agriculture project template should include an `agriculture` project completion action. The stock seeder creates examples for all stock operations.
