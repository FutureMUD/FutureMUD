---
slug: crafting
category: Crafting and projects
summary: Build a glazed cider mug, bottle cider and forge nails. Choose materials, phase timing, tools, quality rules and products, and plan a harness repair.
---

# Crafting System Builder Workflows

A Wayfarer potter brings clay and a measure of slate-blue glaze to the wheel. A few minutes later the inn has an empty slate-blue cider mug for its next guest. Players can see where the colour came from, a cracked firing costs the potter materials, and a better kiln can improve the quality of the next batch. This chapter builds that recipe, then uses the same editor to mend a harness, bottle cider and forge nails.

Choose a **craft** when one maker performs a sequence of tasks with particular materials and tools. Choose a **project** when people need to supply a shared undertaking, take shifts or be paid for labour. A smith can craft nails for a carpenter to deliver to a bridge project. The bridge sponsor does not need to participate in the forging. [Project authoring](Projects_System_Builder_Workflows.md)

## Fire the Wayfarer Inn’s glazed cider mug

### Lay out the pottery workshop

| What to prepare in your world | How the recipe uses it |
| --- | --- |
| The Current `wayfarer_cider_mug` prototype with the `WayfarerGlaze` Variable component | Makes one 350 g stoneware mug, empty and able to hold 300 mL. The inn later pours a 250 mL serving. Keep stoneware as the prototype material. |
| A supply of potting clay and individual glaze portions | Weigh out 400 g of the clay commodity. Give the glaze portions a shared **Pottery Glaze** item tag and the same `WayfarerGlaze` Characteristic as the mug. For the first batch, put a slate-blue portion within reach. A moss-green portion of the same prototype makes a different order. |
| A potter’s wheel and kiln | Place both pieces of equipment in the workshop. Tag the wheel **Potter Wheel** and the kiln **Pottery Kiln**. A second potter can use a different model carrying the same tool tag. |
| An ordinary prototype for a handful of fired potsherds | This is what the potter takes out after a cracked firing. Give it a use or buyer if the pottery recycles fired ceramic as grog. |

Use your world's IDs wherever a command below uses angle brackets. `Pottery` is the example skill. The item guide gives the commands for the vessel, the glaze values and the description markup. Use that glaze-bearing revision for both the craft and the inn’s merchandise. [Build the mug and its glaze](Item_Authoring_Guidelines.md#give-each-mug-a-glaze-with-characteristics) · [Serve the cider](Hospitality_Builder_Guide.md)

### Open the recipe and decide who can make it

```text
craft edit new
craft set name Fire a Wayfarer cider mug
craft set category Pottery
craft set blurb Throw, glaze and fire a stoneware mug for the Wayfarer Inn.
craft set action making a glazed cider mug
craft set progress a cider mug in the making
craft set appear AlwaysTrue
craft set check 1 Pottery Normal
craft set threshold Failure
craft set interruptable
```

The example makes the recipe public. Use the stock `AlwaysTrue` if installed, or bind a Boolean prog with one Character parameter whose body is `return true`. A new craft starts with interruption disabled. The last command toggles it on so the potter can leave work on the bench and resume later. On an existing recipe, inspect `craft show` before toggling.

For a guild recipe, give `appear` a Boolean prog that checks the potter's knowledge. If the recipe should be visible to apprentices who cannot yet attempt it, leave it visible and put the restriction in `canuse`. Pair that with a one-character Text prog on `whycannotuse` that tells the apprentice what to do, such as “Have the master potter sign off your kiln training.” Use the Pottery check for the outcome of an allowed attempt. The `1` gives one extra opportunity for skill improvement; the outcome roll itself decides the firing. [FutureProg authoring](FutureProg_Builder_Guide.md)

### Specify what goes in, what stays and what comes out

Enter these on the open recipe, in order. Each input, tool and product list starts at 1, and failure products have their own numbering.

```text
craft set input add commodity
craft set input 1 material <clay-solid-id>
craft set input 1 weight 400g
craft set input add tagvariable
craft set input 2 tag <pottery-glaze-tag-id>
craft set input 2 quantity 1
craft set input 2 variable WayfarerGlaze
craft set tool add tag
craft set tool 1 tag <wheel-tag-id>
craft set tool 1 room
craft set tool add tag
craft set tool 2 tag <kiln-tag-id>
craft set tool 2 room
craft set product add variable
craft set product 1 item wayfarer_cider_mug
craft set product 1 quantity 1
craft set product 1 variable WayfarerGlaze 2
craft set failproduct add simple
craft set failproduct 1 item <potsherds-prototype-id>
craft set failproduct 1 quantity 1
```

The `2` in `variable WayfarerGlaze 2` points to the glaze input. The potter supplies the actual colour on that portion. A shared item tag makes several suppliers’ glazes interchangeable, while the Characteristic carries the value through firing. The mug keeps its own stoneware material and empty vessel component. For an inn that commissions only blue mugs, use `craft set product 1 variable WayfarerGlaze value slate blue` to prescribe the value instead.

A wheel set to `room` belongs on the floor at the workplace. A hand tool could require `held` or `wield`, and protective gear could require `worn`. Check the maker has enough hands for the tools needed in the **same phase**. The `usetool` toggle controls expenditure of usage duration on items with a hand- or power-tool component. Requiring a kiln by tag and location alone does not model its firing temperature.

### Write the work in the order a potter does it

```text
craft set phase add 30 "@ centre|centres $i1 on $t1 and draw|draws up the walls of a mug."
craft set phase add 20 "@ brush|brushes $i2 across the dried body, leaving the foot bare."
craft set phase add 90 "@ slide|slides the mug into $t2 and wait|waits for the glaze to fire."
craft set phase add 0 "@ take|takes $p1 from $t2 and set|sets it on the cooling shelf." "@ open|opens $t2 and sweep|sweeps out $f1 from the cracked firing."
craft set failphase 4
craft show
```

| Phase | What happens in the workshop |
| --- | --- |
| 1. Throw | `$i1` commits the clay and `$t1` calls for the wheel. The work now exists as a progress item. |
| 2. Glaze | `$i2` commits the selected portion. Its colour is available to the eventual mug product. Check `materials Fire a Wayfarer cider mug` before beginning when several eligible colours are present. |
| 3. Fire | `$t2` brings the kiln into the tool plan. The 90 seconds here separate loading from unloading. Durations separate successive phase actions, so put the firing time on the loading phase. |
| 4. Unload | The Pottery outcome is rolled here. An outcome at or below **Failure** follows the second echo and produces the potsherds (`$f1`). A better outcome follows the first echo and produces the glazed mug (`$p1`). Both batches used the clay and glaze. |

`@` stands for the crafter, and `brush|brushes` supplies the two grammatical forms of a verb. The numbered references also schedule work. Moving `$p1` into the loading echo would create the mug **before** the firing check. Give a phase an explicit failure echo wherever the two outcomes should diverge. Use `phase exertion` and `phase stamina` for sustained physical work such as turning a heavy wheel. [Emote syntax](../Markup/Emote_System.md)

**Quality is a separate decision from success.** The outcome check decides whether there is a mug. A quality check, the quality of inputs already committed and the qualities of tools used contribute to the reference quality for a product. Worlds can disable calculated craft quality with the `DisableCraftQualityCalculation` setting. With calculation enabled, input and tool `quality` settings are relative weights. For example, `craft set input 1 quality 3` and `craft set input 2 quality 1` give the body clay more influence than the glaze within the input contribution. A moss-green mug can be beautifully or poorly made. Its glaze colour is independent of workmanship.

### Approve the recipe and put it to work

Submit the open draft with `craft edit submit`. A reviewer opens `craft review <craft-id>` and uses `accept edit` to approve it. Once Current, the potter uses `crafts` or `craft categories` to find it, `craft view Fire a Wayfarer cider mug` to read it and `craft preview Fire a Wayfarer cider mug` (also `materials Fire a Wayfarer cider mug`) to see what the workshop will supply. The preview is especially useful for a named colour order: keep only the desired glaze in reach if the plan selects another portion.

Start with `craft begin Fire a Wayfarer cider mug`. If interrupted during this recipe, target the unfinished item with `craft resume <progress item>`. Already committed clay stays with the work. Bring the equipment and the materials still needed for the remaining phases. When the mug is ready, the inn receives an empty vessel. Filling it for a guest is a restaurant order.

Use `craft find <item>` to find known recipes that use or produce a particular item. Worlds that enable `analyse` can let a character inspect a carried item for possible known recipes. It does not teach the character a secret recipe. [Analyse capability](../Crafting/Analyse_Capability.md)

For staff-made stock, give the inn an employee who can perform the Current recipe, a reachable workspace with the wheel and kiln, and a supply of compatible clay and glaze. Configure a merchandise replenishment task for the mug, then choose who supplies ingredients and how many finished mugs the inn should hold. An existing automatic reorder can fill the same shortage, so give that stock line one purchasing or production policy. [Shop craft replenishment](Shop_and_Banking_Builder_Guide.md) · [Employment hosts](Employment_Hosts_Builder_Guide.md)

To change the firing time next season, open `craft edit <craft-id>`, edit phase 3 on the new revision and submit it. Put both the echo and the revised time in `phase edit`. Active progress items record the revision they started under. Clone a recipe when both versions should stay available, such as a public plain mug alongside the inn’s commissioned glaze.

## Three smaller workshop recipes

These blocks configure the distinctive parts of three **separate new recipes**. Start each with the same shell workflow as the mug: choose a unique name, category, blurb, action and progress text, set an appearance prog and select a suitable local skill. The slot numbers below start at 1 in each recipe.

### Mend a worn harness at the saddler’s bench

The stable wants the guest’s own harness back, with its identity and fittings intact. The saddler supplies fresh stitching cord, uses a harness awl, and takes the harness off the animal before starting. Give eligible harnesses the **Harness** item tag. Set the failure check on stitching the patch, then reserve the repair target on the successful finishing phase. This lets a bad seam waste the cord without taking the customer's harness as a consumable.

```text
craft set input add simple
craft set input 1 item <stitching-cord-prototype-id>
craft set input add repair
craft set input 2 tag <harness-tag-id>
craft set input 2 repair 25%
craft set tool add tag
craft set tool 1 tag <harness-awl-tag-id>
craft set tool 1 held
craft set phase add 25 "@ thread|threads $i1 and stitch|stitches the patch with $t1." "@ pull|pulls $i1 too tight with $t1 and split|splits the patch."
craft set phase add 0 "@ set|sets the seam and hand|hands back $i2." "@ gather|gathers up the spoiled stitching."
craft set failphase 1
```

**Current engine limitation.** The `repair` input selects a damaged tagged item and ordinary completion returns that same item. Its configured `25%` is intended to add 25 percentage points of condition, capped at full condition. The current completion path returns the harness without applying that increase. Keep this recipe out of paid stable services until condition restoration is fixed. Loading a fresh harness as a product would lose the identity this service is meant to preserve.

### Bottle cider for the shop

The inn pours draught cider into reusable mugs at the table. The shop sells a different product: one take-away bottle containing 500 mL. Start with an open supply cask of cider and an approved empty bottle prototype that can hold at least 500 mL. Keep its default liquid empty. Take one empty bottle out of stock for every filled bottle made.

```text
craft set input add simple
craft set input 1 item <bottle-prototype-id>
craft set input 1 quantity 1
craft set input 1 emptyliquid
craft set input add liquid
craft set input 2 liquid <cider-liquid-id>
craft set input 2 amount 500ml
craft set product add liquid
craft set product 1 item <bottle-prototype-id>
craft set product 1 quantity 1
craft set product 1 liquid <cider-liquid-id>
craft set product 1 volume 500ml
craft set phase add 15 "@ set|sets $i1 at the tap and draw|draws off $i2."
craft set phase add 0 "@ wipe|wipes the neck of $p1 for the shop shelf." "@ set|sets aside the spoiled batch."
craft set failphase 2
```

`emptyliquid` toggles an empty-vessel requirement on the first input. The second input draws a volume and leaves the supply cask in service. The product creates a **new** filled bottle of the chosen prototype, so use interchangeable shop bottles. A customer’s engraved bottle belongs in an ordinary pouring service. The finished liquid is the configured cider; to sell distinct vintages, decide how a recipe will preserve that distinction. If you change the output to two bottles, supply two empties and a full litre: `volume` is per bottle.

### Forge nails for the bridge crew

Start the smith with 2 kg of ordinary iron and the town’s tagged forge and hammer. For counted fasteners, make an approved stackable iron nail prototype weighing 20 g per unit. The following batch makes 100. The mass balance here is a recipe design choice. For a forge with waste, reduce the yield and give the offcuts a use.

```text
craft set input add commodity
craft set input 1 material <iron-solid-id>
craft set input 1 weight 2kg
craft set tool add tag
craft set tool 1 tag <forge-tag-id>
craft set tool 1 room
craft set tool add tag
craft set tool 2 tag <smith-hammer-tag-id>
craft set tool 2 wield
craft set product add simple
craft set product 1 item <iron-nail-prototype-id>
craft set product 1 quantity 100
craft set phase add 45 "@ heat|heats $i1 at $t1 and draw|draws out the nail stock."
craft set phase add 0 "@ head|heads the last of $p1 with $t2." "@ flatten|flattens the last spoiled point with $t2."
craft set failphase 2
```

If the bridge contract orders nails **by weight**, replace the counted product on a separate version of the recipe:

```text
craft set product remove 1
craft set product add commodity
craft set product 1 commodity <iron-solid-id>
craft set product 1 weight 2kg
craft set product 1 tag <nails-pile-tag-id>
```

The first contract can count 100 explicit items; the second can take 2 kg of iron carrying the **Nails** pile tag. A tag on the iron solid would describe the underlying material and would also match other forms of iron. Keep the pile tag for the shape the smith has produced. Choose one output per batch so the forge does not issue the same stock twice. [Commodities, counted items and project supply](Primary_Production_and_Agriculture_Builder_Guide.md)

## Choosing a product when inputs vary

| What should change from batch to batch? | Product to reach for |
| --- | --- |
| Nothing except ordinary product quality | `simple` for a fixed prototype, `commodity` for a bulk lot, `liquid` for filled vessels. |
| A visible feature already exists on an ingredient | `variable` copies the Characteristic from a variable-aware input, as with the mug’s glaze. A fixed value on the same product can specify another feature. |
| A particular ingredient prototype implies an output feature | `inputvariable` maps an item prototype from a numbered input to a value on one output prototype. Plan a mapping for each ingredient the recipe admits. |
| A rule must choose a Characteristic using several consumed inputs | `progvariable` asks a selector for a numeric characteristic-value ID. Supply a valid value of the output’s definition for each intended case. |
| The recipe can produce different item prototypes or a different number of objects | `prog` returns an item or an item collection. Its item inputs are grouped by craft slot; it can also receive the consumed liquid mixtures. Put a success-only effect on a normal product at or after the outcome check. |
| A meal needs to remember its ingredients | `cookedfood` starts with a prepared-food prototype and records the recipe ingredients. `progcookedfood` adds a prog-selected base. [Cooking and prepared foods](../Crafting/Food_and_Cooking_System.md) |

Use `onstart` for a consequence of beginning work, `oncancel` for a cancellation and `onfinish` for a consequence common to **both** completed outcomes. Each callback gets the character. It cannot inspect the consumed input slots through that parameter. A firing that finishes with potsherds still calls `onfinish`; the normal and failure products already distinguish those two outcomes.

## Main Command Surface

### Player-facing commands
Use these to find a recipe, plan a batch or continue work:

- `crafts`
- `craft begin <craft>`
- `craft resume <progress item>`
- `craft view <craft>`
- `craft preview <craft>`
- `materials <craft>`
- `craft categories`
- `craft find <item>`
- `analyse <carried item>` / `analyze <carried item>` — optional binary capability; see [configuration and matching limits](../Crafting/Analyse_Capability.md)

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
| `check <free rolls> <trait> <difficulty>` | Main craft outcome check | Selects the outcome check and extra improvement rolls |
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
| `simple` | Exact item prototype input | `item`, `quantity`, `quality` | Use `emptyliquid` to require an empty liquid-container instance |
| `simplematerial` | Items made from an exact material or material tag | `material`, `tag`, `quantity`, `quality` | Must set either the exact material or the material tag |
| `tag` | Tagged item input | `tag`, `quantity`, `quality` | Best for generic consumables like thread or padding |
| `tagvariable` | Tagged item input that also supplies characteristics | `tag`, `quantity`, `variable`, `quality` | Item must expose every declared characteristic definition |
| `commodity` | Exact-material commodity pile input | `material`, `weight`, optional `tag`, `quality` | Must set exact material and positive weight |
| `commoditytag` | Material-tagged commodity pile input | `material`, `weight`, optional `tag`, `quality` | Uses a material tag rather than one exact solid |
| `liquid` | Specific liquid input | `liquid`, `amount`, `quality` | Must set liquid and positive amount |
| `tagliquid` | Input accepting tagged liquids | `tag`, `amount`, `quality` | The liquid itself carries the tag |
| `repair` | Target item to repair | `tag`, `repair`, `quality` | Reserves a damaged tagged item. See the harness example for the completion limitation. |
| `field` | Local agriculture field input | `use`, `crop`, `woodland`, `condition`, `health`, `yield`, `consume` | Requires a matching field in the crafter's current cell and can consume crop or woodland yield |

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
| `variable` | Load items with inherited and/or deliberately selected characteristics | `item`, `skin`, `quantity`, `variable <definition> <input#>`, `variable <definition> value <value>` | Each definition has one rule; inherited values require a variable-aware input, selected values require an exact compatible value name or ID |
| `inputvariable` | Load items whose characteristics depend on which item was used for an input | `item`, `skin`, `quantity`, `variable ...` | Requires explicit input index and per-item value mappings |
| `commodity` | Load commodity piles | `commodity`, `weight`, `tag`, `material` | Requires material and positive weight |
| `liquid` | Load filled vessels | `item`, `quantity`, `liquid`, `volume` | Each new vessel must have capacity for the configured volume |
| `cookedfood` / `progcookedfood` | Prepare food from the consumed ingredients | `ingredient`, plus the base item or selector prog | Choose a prepared-food prototype |
| `money` | Produce money | `currency`, `amount` | Requires currency and positive amount |
| `npc` | Spawn NPCs from a template | `template`, `quantity`, optional `prog` | Template must be approved; optional prog must be `void(Character)` |
| `prog` | Let a prog decide which items to load | `prog` | Prog must match the runtime signature expected by the product |
| `progvariable` | Load an item and fill characteristics from progs | `item`, `quantity`, `variable <definition> <prog>` | Use when characteristic values are better derived by code than by input lookup |
| `unusedinput` | Return a portion of a consumed item input | `input`, `percentage` | Target input must consume items or item groups |
| `scrap` | Convert part of an input into scrap commodity | `input`, `percentage`, `tag`, `material` | Good for salvage-style fail outputs |
| `dnatest` | Compare two liquid inputs | `input1`, `input2` | Both targeted inputs must consume liquids and must be different |
| `bloodtyping` | Test one liquid input against a blood model | `input`, `bloodmodel` | Requires a valid blood model plus a liquid-consuming input |

For a variable product, `variable Colour 1` copies Colour from the first variable-aware input; `variable Accent value white` selects white for Accent on that product. Both rules can coexist when the item supports both definitions. An exact selected value affects crafted instances, not every copy of the prototype or skin. `variable <definition>` removes either rule, and configuring a new rule for the same definition replaces the old one. Invalid selections leave the previous configuration intact. Missing or incompatible input values stop product creation instead of silently choosing another colour. Check garment definitions and permitted palettes as part of content preflight; these product rules do not create characteristic channels on the item.

## When the workshop cannot fill an order

| What the builder or maker sees | Where to look |
| --- | --- |
| The approved recipe is missing from the potter’s list | Check the Current revision, validity and appearance prog. Inspect the recipe as the intended character; administrators bypass its appearance restriction. |
| The recipe is visible, but the potter cannot begin | Read the `whycannotuse` message and inspect `craft preview`. A visibility rule, a permission rule and a missing physical tool each have a different remedy. |
| The wrong glaze or bottle is being taken | Inspect the selected input with `materials <craft>`. Narrow its prototype, tag or Characteristic requirement, or move another eligible supply out of reach. An empty-container filter is useful on packaging. |
| The wheel is in the room but the tool shows as missing | Check tool state and phase references. A wielded hammer also needs a free wielder. An unreferenced tool is required in every normal phase. |
| A product appears on the wrong branch, or the craft refuses submission | Read the invalid references in `craft show`. Inspect the first mention of each numbered product and the fail phase. Recheck numbers after deleting a slot. Reference failure products in the failure echo. |
| The workshop changes stock but the inn never receives mugs | Compare the actual product prototype with the merchandise profile and check employee access to the kiln, inputs and shop storage. |

For a catalogue of existing work, `craft list` accepts a category, `+keyword`, `-keyword`, `*<item-proto-id>`, `&<tag>` or `^<liquid>`. Use `craft show` on the open definition to inspect its numbered lists and revisions before adapting it for a new customer.
