# Crafting System Builder Workflows

## Purpose
This document explains the practical builder workflow for creating, revising, validating, and testing crafts in game.

The intended audience is builders and staff users working through the `craft` command family rather than editing code directly.

## Mental Model
When you build a craft, you are authoring a staged work definition.

You are not just saying "this input becomes this item". You are defining:

- who can see or use the craft
- what the active progress item looks like
- which tools must be present, worn, wielded, or merely nearby
- which materials or target items the craft reserves and consumes
- which phase performs the success check
- what the crafter visibly does during each phase
- what comes out on success and on failure

If a craft feels wrong in play, the problem is often in one of those behavioural choices rather than in the final product alone.

## Normal Authoring Workflow
1. Find an existing craft to inspect, or create/clone one with `craft list`, `craft show`, `craft edit new`, or `craft clone`.
2. Set craft-wide metadata such as name, category, blurb, action text, progress-item text, and check settings.
3. Author the ordered phases with their timings, echoes, fail echoes, exertion, and stamina settings.
4. Add and configure inputs, tools, products, and fail products.
5. Assign availability and callback progs.
6. Validate with `craft show` and fix any errors shown at the bottom.
7. Test from the player side with `craft view`, `craft preview` or `materials`, and `craft begin`.
8. Submit and review through the normal revisable workflow once the craft is valid.

## Main Command Surface

### Player-facing commands
Use these to test the craft as a player would see it:

- `crafts`
- `craft begin <craft>`
- `craft resume <progress item>`
- `craft view <craft>`
- `craft preview <craft>`
- `materials <craft>`
- `craft categories`
- `craft find <item>`

### Builder-facing commands
Use these to author and review craft content:

- `craft list [filters]`
- `craft show <craft>`
- `craft edit <craft>`
- `craft edit new`
- `craft edit close`
- `craft edit submit`
- `craft review <id>|all|mine`
- `craft clone <craft> <newName>`
- `craft set <...>`

Important note:

- `craft set` is the general edit entry point once you have the craft open for editing.

## Craft-Wide Command Reference
The following options are the core `craft set` workflow.

| Command | What it sets | Notes |
| --- | --- | --- |
| `name <text>` | Craft name | Must remain globally unique across approved and in-design crafts |
| `category <text>` | Craft category | Used by craft list and category browsing |
| `blurb <text>` | Short player-facing description | Shown in the `crafts` list |
| `action <gerund text>` | Current-action description | Should read like `forging a sword blade` |
| `item <passive text>` or `progress <passive text>` | Active progress item short description | Should read like `a sword being forged` |
| `check <free rolls> <trait> <difficulty>` | Main craft outcome check | Controls skill gate and free extra improvement rolls |
| `practical` | Toggle practical versus theoretical check | Affects trait-use type on the outcome check |
| `threshold <outcome>` | Failure threshold | Outcome at or below this threshold fails the craft |
| `failphase <number>` | Phase where success/failure split occurs | Must be a real phase |
| `interruptable` | Toggle whether interruption pauses or cancels | Non-interruptable crafts are cancelled when interrupted |
| `appear <prog>` | Craft-list visibility prog | Required for craft validity |
| `canuse <prog>` | Start-permission prog | Must return boolean for one character |
| `whycannotuse <prog>` | Failure-message prog | Must return text for one character |
| `onstart <prog>` | Start callback | Must accept one character |
| `onfinish <prog>` | Completion callback | Must accept one character |
| `oncancel <prog>` | Cancellation callback | Must accept one character |

## Phase Authoring

### Phase commands
| Command | Purpose |
| --- | --- |
| `phase add <seconds> "<echo>" ["<failecho>"]` | Add a new phase |
| `phase edit <number> <seconds> "<echo>" ["<failecho>"]` | Replace an existing phase |
| `phase remove <number>` | Delete a phase |
| `phase swap <phase1> <phase2>` | Reorder two phases |
| `phase exertion <phase> <level>` | Set minimum exertion for that phase |
| `phase stamina <phase> <value>` | Set stamina cost for that phase |

Important current behaviour:

- if you omit the fail echo when adding or editing a phase, it defaults to the normal echo
- the fail phase is not a separate phase list; it is the phase where the check happens and the craft then follows either the success or failure branch

### Token meanings
Craft echoes are functional, not just cosmetic.

| Token | Meaning |
| --- | --- |
| `$iN` | input number `N` |
| `$tN` | tool number `N` |
| `$pN` | normal product number `N` |
| `$fN` | fail product number `N` |

### Why token placement matters
The engine uses token placement to decide timing.

Current verified rules:

- the first success-echo use of `$iN` decides when that input becomes consumed
- the presence of `$tN` in a phase makes that tool part of that phase's tool plan
- the first success-echo use of `$pN` decides when that normal product is produced
- the first fail-echo use of `$fN` decides when that fail product is produced

Defaulting rules if you never reference something:

- an unreferenced input is treated as consumed at phase 1
- an unreferenced tool is treated as required in every normal phase
- an unreferenced normal product defaults to the fail phase
- an unreferenced fail product defaults to the fail phase

Important fail-echo rule:

- fail products must only appear as `$fN` in fail echoes, never in normal echoes

## Inputs, Tools, Products, and Fail Products

### Inputs
Use:

- `input add <type>`
- `input remove <number>`
- `input <number> <subcommand>`

### Tools
Use:

- `tool add <type>`
- `tool remove <number>`
- `tool <number> <subcommand>`

### Products
Use:

- `product add <type>`
- `product remove <number>`
- `product <number> <subcommand>`

### Fail products
Use:

- `failproduct add <type>`
- `failproduct remove <number>`
- `failproduct <number> <subcommand>`

## Builder Reference: Inputs
| Alias | What it is for | Main subcommands | Important constraints |
| --- | --- | --- | --- |
| `simple` | Exact item prototype input | `item`, `quantity`, `quality` | Must set a target item |
| `simplematerial` | Items made from an exact material or material tag | `material`, `tag`, `quantity`, `quality` | Must set either the exact material or the material tag |
| `tag` | Tagged item input | `tag`, `quantity`, `quality` | Best for generic consumables like thread or padding |
| `tagvariable` | Tagged item input that also supplies characteristics | `tag`, `quantity`, `variable`, `quality` | Item must expose every declared characteristic definition |
| `commodity` | Exact-material commodity pile input | `material`, `weight`, optional `tag`, `quality` | Must set exact material and positive weight |
| `commoditytag` | Material-tagged commodity pile input | `material`, `weight`, optional `tag`, `quality` | Uses a material tag rather than one exact solid |
| `liquid` | Specific liquid input | `liquid`, `amount`, `quality` | Must set liquid and positive amount |
| `tagliquid` | Liquid input from a tagged source | `tag`, `amount`, `quality` | Useful for tagged vessels or apparatus |
| `repair` | Target item to repair | `tag`, `repair`, `quality` | Targets a tagged item and repairs condition by a percentage |

## Builder Reference: Tools
All tools also support the shared base tool options:

- `held`
- `worn`
- `wield`
- `room`
- `quality`
- `usetool`

| Alias | What it is for | Type-specific subcommands | Important constraints |
| --- | --- | --- | --- |
| `simple` | Require one exact item proto | `item` | Use when one specific prototype must be present |
| `tag` | Require any item matching a tag | `tag` | Use for tool families such as hammer, anvil, pliers, or sewing needle |

## Builder Reference: Products
| Alias | What it is for | Main subcommands | Important constraints |
| --- | --- | --- | --- |
| `simple` | Load ordinary items | `item`, `skin`, `quantity`, `material` | Must set an item proto; skin must match that proto and be approved |
| `variable` | Load items with characteristics coming from variable-aware inputs | `item`, `skin`, `quantity`, `variable <definition> <input#>` | Source input must be an `IVariableInput` that provides the definition |
| `inputvariable` | Load items whose characteristics depend on which item was used for an input | `item`, `skin`, `quantity`, `variable ...` | Requires explicit input index and per-item value mappings |
| `commodity` | Load commodity piles | `commodity`, `weight`, `tag`, `material` | Requires material and positive weight |
| `money` | Produce money | `currency`, `amount` | Requires currency and positive amount |
| `npc` | Spawn NPCs from a template | `template`, `quantity`, optional `prog` | Template must be approved; optional prog must be `void(Character)` |
| `prog` | Let a prog decide which items to load | `prog` | Prog must match the runtime signature expected by the product |
| `progvariable` | Load an item and fill characteristics from progs | `item`, `quantity`, `variable <definition> <prog>` | Use when characteristic values are better derived by code than by input lookup |
| `unusedinput` | Return a portion of a consumed item input | `input`, `percentage` | Target input must consume items or item groups |
| `scrap` | Convert part of an input into scrap commodity | `input`, `percentage`, `tag`, `material` | Good for salvage-style fail outputs |
| `dnatest` | Compare two liquid inputs | `input1`, `input2` | Both targeted inputs must consume liquids and must be different |
| `bloodtyping` | Test one liquid input against a blood model | `input`, `bloodmodel` | Requires a valid blood model plus a liquid-consuming input |

## Review and Debugging Workflow

### Inspect the craft definition
Use:

- `craft show <craft>`
- `craft show` while editing

This is the most important builder command because it shows:

- current settings
- phase ordering
- current input, tool, product, and fail-product numbering
- validity state
- specific validation errors at the bottom when something is wrong

### Inspect discoverability
Use:

- `craft list [filters]`
- `craft categories`
- `craft find <item>`

Useful list filters:

- `+keyword`
- `-keyword`
- category name
- `*<item proto id>`
- `&<tag>`
- `^<liquid>`

### Inspect player feasibility
Use:

- `craft view <craft>`
- `craft preview <craft>`
- `materials <craft>`

Interpretation:

- missing tools usually means wrong tool state, wrong tool definition, or not enough wielders/hands
- missing materials usually means the input definitions do not match what the test character actually has access to

### Test actual execution
Use:

- `craft begin <craft>`
- `craft resume <progress item>`

Watch for:

- the active progress item description
- whether the correct phase echoes fire
- whether the right tools are required in the right phases
- whether products and fail products appear when expected

## Practical Examples

### Simple item assembly
The seeded assembly crafts are the clearest pattern for multi-input, multi-tool item output.

Typical shape:

- `tag` inputs for components
- `tag` or `simple` tools
- `simple` output product
- phase echoes that reference inputs and tools in the order they are used

### Material-driven output
Use this when the output should inherit material from an input rather than always loading in its base proto material.

Typical pattern:

- a material-bearing input such as `commodity` or another item-consuming input
- a `simple` product
- `product <n> material <input#>` to tie the output material to that input

### Fail-product salvage
This is common in seeded metalworking examples.

Typical pattern:

- a normal success product such as a finished blade
- a fail product such as a twisted hunk of metal or an `unusedinput` return
- fail echo uses `$fN` to show the salvage result

### Room tool requirements
Forge-style crafts are a good example.

Typical pattern:

- several `tag` tools
- tools set to `room` state
- phase echoes reference the appropriate room tools

This is the right model for anvils, forges, bellows, and other stationary apparatus.

## Common Mistakes
- Forgetting to set the appear prog. A craft without it is invalid even if everything else looks complete.
- Using the wrong FutureProg signature for `appear`, `canuse`, `whycannotuse`, `onstart`, `onfinish`, or `oncancel`.
- Referencing `$iN`, `$tN`, `$pN`, or `$fN` numbers that do not exist after reordering or deleting child entries.
- Using fail products in regular echoes instead of only in fail echoes.
- Expecting a product to inherit material automatically without setting its material-defining input.
- Forgetting that tools not referenced in normal echoes are treated as needed in every normal phase.
- Forgetting that inputs not referenced in normal echoes default to phase 1 consumption.
- Choosing the wrong tool state and then reading the problem as a missing-item issue rather than a hold, wield, wear, or room-placement issue.
- Setting a skin before setting the item proto, or choosing a skin that belongs to a different item proto.
- Confusing player commands such as `craft view` and `craft preview` with builder editing commands such as `craft set`.
- Renaming or cloning a craft into a name that is already in use by another approved or in-design craft.

## Final Builder Checklist
Before submitting a craft for review, confirm all of the following:

- `craft show` says the craft is valid
- the appear prog is set
- the fail phase is correct
- the progress-item description reads naturally
- each input, tool, product, and fail product is configured and numbered as expected
- phase echoes reference the right `$i`, `$t`, `$p`, and `$f` tokens
- `craft preview` shows the intended tools and materials
- a real `craft begin` test behaves as expected on both success and interruption
