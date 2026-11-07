# Autobuilder Builder How-To

## What The Seeders Give You

There are now two different stock autobuilder layers:

- Core seeding gives you shape templates such as `Rectangle`, `Terrain Rectangle`, and `Feature Rectangle`.
- Useful seeding gives you the wilderness grouped starter package:
  - room template: `Seeded Terrain Wilderness Grouped Description`
  - area template: `Seeded Terrain Wilderness Grouped Features`
  - supporting terrain feature tags for the seeded catalogue

The intended pairing is:

- core area template plus useful room template when you want to drive topology manually
- useful wilderness area template plus useful wilderness room template when you want stock terrain-aware feature tagging and prose layering out of the box

## Basic Workflow

### 1. Open a cell package

Autobuilders work inside cell overlay packages.

- `cell package new "My New Area"`
- `cell package open <id|name>`

If you do not have a package open, `cell new <template> ...` will not be useful.

### 2. Inspect the available templates

Use the show commands first:

- `show autoareas`
- `show autoarea <id|name>`
- `show autorooms`
- `show autoroom <id|name>`

Use the edit commands only when you are authoring or customising templates:

- `autoarea list`
- `autoarea show <id|name>`
- `autoarea edit <id|name>`
- `autoroom list`
- `autoroom show <id|name>`
- `autoroom edit <id|name>`

## Quick Start With Terrain Planner

### 1. Sign in and load the live catalogue

Open the hosted Terrain Planner URL supplied by the game operator and sign in with your existing FutureMUD account. The account must be registered, unsuspended, and have Admin authority or higher. The planner does not create or modify accounts. Terrain and hierarchical tag palettes load from the bundled read-only Engine API.

### 2. Paint the map

- Set grid width and height.
- Use paint, flood fill, rectangle, eyedropper, or erase tools on either the Terrain or Tag layer.
- A cell may have multiple tags. Terrain `0` removes the room and its tags.
- Leave any unused positions as `None` if you want holes in the shape.

### 3. Export the mask

Use the separate copy buttons for the comma-separated terrain mask and feature/tag mask. If clipboard permission is denied, select the visible text manually. Versioned project JSON can be saved separately for later editing.

Important rules:

- The mask is row-major from the bottom-left `(0,0)` cell: proceed right across the southern row, then move one row north.
- `0` means no room should be created at that coordinate.
- The number of entries must exactly match `height x width`.
- Feature-mask entries use positive tag IDs separated by `|`; an untagged cell is an empty entry. The Terrain Planner displays the names for you but exports the IDs, so duplicate or renamed tag names cannot change the target tag.

## Building An Area

### Stacked terrain plans

Create an area template once:

```text
autoarea edit new "Stacked Terrain" "terrain stack"
```

In Terrain Planner, add signed Z levels and paint their terrain/tags. All levels share the same width and height. Occupied neighbouring rooms connect north/south/east/west; matching XY positions on adjacent Z levels connect up/down. Select a room to suppress its up/down connection or configure climbing, difficulty and falling toward the lower room. A fall link without climbing requires flight for upward travel. Empty cells and gaps between Z levels are never bridged. Terrain Planner levels create separate game rooms, rather than elevations within one room.

Open an under-design overlay package as usual, then import using a room template from `show autorooms`:

```text
room new "Stacked Terrain" "My Room Template" FMS1:...
```

For larger exports, choose **Copy editor paste** and start:

```text
room new "Stacked Terrain" "My Room Template" editor
```

Paste the wrapped data, then enter `@` on its own line. Enter `*cancel` to abandon it. Unknown terrain/tag IDs, damaged exports or a changed overlay/template are rejected before rooms are created. Use `prog=<prog>` before the room-template argument for the existing post-processing behaviour. The importer transfers you to the first occupied room ordered by Z, then Y, then X.

Use **Copy token** only below 8,000 characters. Editor exports wrap at 1,000 characters to stay below connection-line limits; compression alone does not guarantee a safe single line. Existing rectangle imports still use the old masks, which apply only to the selected planner Z level. Save project JSON to preserve the full stack for later editing. New projects save as schema 2; old projects load as Z=0.

Stack limits are 200×200 per level, 32 levels, 200,000 total positions and 40,000 occupied rooms. Unresolved catalogue entries can remain in project files but must be resolved before game export. Imported coordinates describe connectivity and do not assign persistent world XYZ.

The main command is:

```text
cell new <area template> ...
```

For terrain-aware rectangles, the usual shape is:

```text
cell new "Terrain Rectangle" <height> <width> <room template> <terrain mask>
```

For the seeded wilderness package, the stock all-in-one shape is:

```text
cell new "Seeded Terrain Wilderness Grouped Features" <height> <width> <room template> <terrain mask>
```

Example:

```text
cell new "Seeded Terrain Wilderness Grouped Features" 5 7 "Seeded Terrain Wilderness Grouped Description" 12,12,12,15,15,15,15,12,12,12,15,15,15,15,0,0,12,12,12,15,15,0,0,12,12,12,15,15,18,18,18,12,12,12,15,18
```

That will:

- create the cells
- assign each cell the painted terrain
- link orthogonal exits
- generate wilderness feature tags for each room, including road topology where relevant
- ask the room template to generate names and descriptions from those tags

If you want diagonal exits too, use the diagonals variant.

## Choosing The Right Pairing

### Fast terrain bootstrap

Use:

- area template: `Seeded Terrain Wilderness Grouped Features`
- room template: `Seeded Terrain Wilderness Grouped Description`

This gives you two guaranteed physical prose layers per room, plus optional sensory/resource/road detail with no manual prose required.

### Manual terrain bootstrap with the stock room prose

Use:

- area template: `Terrain Rectangle`
- room template: `Seeded Terrain Wilderness Grouped Description`

This is useful when you want the seeded wilderness prose but prefer to supply feature tags some other way.

### Manual feature-tag control

Use:

- area template: `Feature Rectangle`
- room template: `Seeded Terrain Wilderness Grouped Description` or a custom random room template

`Feature Rectangle` takes an extra feature mask argument after the terrain mask. Each cell's tag IDs are separated with `|`, and cells are still separated with commas. It resolves those IDs directly to framework tags while supplying their names to the selected room template's descriptive feature rules.

That lets you drive extra descriptive tags into the room template deliberately.

## Editing Room Templates

`autoroom` is the authoring surface for room templates.

Useful commands:

- `autoroom edit new <name> <type>`
- `autoroom clone <old> <new>`
- `autoroom edit <which>`
- `autoroom set summary <text>`
- `autoroom set applytags`

For `room by terrain` templates, the most useful edits are:

- default room name
- default description
- default terrain
- default light multiplier
- outdoors/indoors/cave/shelter mode
- per-terrain overrides

For `room random description` templates, add these:

- random sentence count expression
- random description elements
- grouped elements
- road-aware elements

## Editing Area Templates

`autoarea` is the authoring surface for area templates.

Useful commands:

- `autoarea edit new <name> <type>`
- `autoarea clone <old> <new>`
- `autoarea edit <which>`
- `autoarea set summary <text>`

Common subtype-specific edits include:

- toggling diagonals on rectangle-based templates
- adding and editing feature groups on random-feature templates

## Good Practical Defaults

- Start with core stock area templates. They are stable and simple.
- Start with the Useful seeded wilderness area+room pair if you want immediate usable terrain-matched prose.
- Clone the stock templates before making game-specific heavy edits.
- Keep the Useful templates as a fallback reference, even if you later write more tailored ones.

## Common Mistakes

- Forgetting to open a cell package before building.
- Supplying a mask with the wrong number of entries.
- Forgetting that `0` in a terrain mask means no room at that coordinate.
- Expecting feature-based variation from a room template when the chosen area template does not provide any tags.
- Using road prose tags like `Trail` or `Dirt Road` for direction-aware sentences instead of topology tags such as `Trail Straight` or `Dirt Road Bend`.
- Editing the core shape templates when you really wanted to customise only the descriptive room template.
