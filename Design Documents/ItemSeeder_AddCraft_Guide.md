# ItemSeeder AddCraft Authoring Guide

This guide documents the private `AddCraft` helper in `DatabaseSeeder/Seeders/ItemSeederCrafting.cs`.

`AddCraft` is a spreadsheet-friendly bridge into the craft tables. It creates a `Craft`, its phases, inputs, tools, success products, and fail products from compact strings that are converted into the XML definitions used by the runtime craft factories.

For the broader runtime and builder model, see:

- `Design Documents/Crafting_System_Overview.md`
- `Design Documents/Crafting_System_Runtime_and_Extensibility.md`
- `Design Documents/Crafting_System_Builder_Workflows.md`

## Function Shape

```csharp
private MudSharp.Models.Craft? AddCraft(
	string name,
	string category,
	string blurb,
	string action,
	string itemsdesc,
	string appearProg,
	string? canUseProg,
	string? whyCantProg,
	string? onFinishProg,
	MudSharp.Models.TraitDefinition trait,
	Difficulty difficulty,
	Outcome threshold,
	int freeChecks,
	int failPhase,
	bool interrupatable,
	IEnumerable<(int Seconds, string Echo, string FailEcho)> phases,
	IEnumerable<string> inputs,
	IEnumerable<string> tools,
	IEnumerable<string> products,
	IEnumerable<string> failProducts,
	List<(int Product, int Input)>? productMaterialInputIndexes = null,
	List<(int Product, int Input)>? failProductMaterialInputIndexes = null)
```

The helper returns the created craft or `null` if any input, tool, or product string cannot be resolved against the seeded lookup data.

## Parameters

| Parameter | Meaning |
| --- | --- |
| `name` | Unique craft name, for example `forge sword blade`. |
| `category` | Craft category shown in craft lists, for example `Weaponcrafting`. |
| `blurb` | Short command-style description, for example `forge a sword blade`. |
| `action` | Active action description, for example `forging a sword blade`. |
| `itemsdesc` | Short description for the active craft progress item. |
| `appearProg` | FutureProg function name for list visibility. Must exist in `_progs`. |
| `canUseProg` | Optional FutureProg function name that gates craft use. |
| `whyCantProg` | Optional FutureProg function name explaining a failed `canUseProg`. |
| `onFinishProg` | Optional FutureProg function name run on successful completion. |
| `trait` | Trait checked by the craft outcome roll. |
| `difficulty` | `Difficulty` enum value for the craft check. |
| `threshold` | `Outcome` enum value at or below which the craft fails. |
| `freeChecks` | Number of practical/free skill checks granted by the craft. |
| `failPhase` | Phase number where the success/failure result is decided. |
| `interrupatable` | Whether the active craft can pause and resume. The parameter name is misspelled in the current helper. |
| `phases` | Ordered phase tuples: seconds, success echo, fail echo. |
| `inputs` | Ordered input conversion strings. Referenced in echoes as `$i1`, `$i2`, etc. |
| `tools` | Ordered tool conversion strings. Referenced in echoes as `$t1`, `$t2`, etc. |
| `products` | Ordered success product conversion strings. Referenced in success echoes as `$p1`, `$p2`, etc. |
| `failProducts` | Ordered failure product conversion strings. Referenced in fail echoes as `$f1`, `$f2`, etc. |
| `productMaterialInputIndexes` | Optional one-based `(Product, Input)` pairs. Product `1` with input `1` means `$p1` inherits material from `$i1`. |
| `failProductMaterialInputIndexes` | Same as above, but for fail products. |

## Phase Echoes

Phases are not just text. The runtime scans success and fail echoes for tokens and uses them to infer when inputs, tools, and products participate.

| Token | Meaning |
| --- | --- |
| `$iN` | Input N. The first success-echo reference sets the phase where the input is consumed. |
| `$tN` | Tool N. The tool is required in each success phase where it appears. |
| `$pN` | Success product N. The first success-echo reference sets the phase where the product appears. |
| `$fN` | Fail product N. Only valid in fail echoes; the first reference sets the fail-product phase. |

If an input is never referenced, it is consumed at phase 1. If a tool is never referenced, it is required in every normal phase. Unreferenced success and fail products default to `failPhase`.

## Conversion String Basics

All input, tool, and product strings use this basic shape:

```text
TypeName - details
```

Tools include a desired state between the type and details:

```text
TypeName - Held - details
TypeName - InRoom - details
```

Product strings may add semicolon-separated options after the main detail:

```text
SimpleProduct - 1x a cloak (#100); skin blue cloak skin
CookedFoodProduct - 1x a stew (#200); purify off; ingredient $i1=meat, $i2=vegetable
```

Type names are matched case-insensitively, but the seeder writes the canonical runtime type names into the database.

## Input Types

| Type | Syntax | Notes |
| --- | --- | --- |
| `Tag` | `Tag - 2x an item with the Thread tag` | Consumes items with the tag. |
| `TagVariable` | `TagVariable - 1x an item with the Dyed Cloth tag with variables Colour, Pattern` | Like `Tag`, but also declares characteristic definitions available to variable products. |
| `SimpleItem` | `SimpleItem - 1x a short shaft (#123)` | Consumes exact item prototype. The `#id` is preferred if present. |
| `SimpleMaterial` | `SimpleMaterial - 1x an item with material tagged as Wood` | Consumes item prototypes whose material has the tag. |
| `Commodity` | `Commodity - 1 kilogram 500 grams of iron` | Consumes exact material commodity by mass. |
| `CommodityTag` | `CommodityTag - 500 grams of a material tagged as Fabric` | Consumes commodity whose material has the tag. |
| `LiquidUse` | `LiquidUse - 1 litre of Water` | Consumes a specific liquid. Alias: `Liquid`. |
| `LiquidTagUse` | `LiquidTagUse - 250 millilitres of a liquid tagged Cooking Oil` | Consumes a liquid from a tagged source. Alias: `TagLiquid`. |
| `ConditionRepair` | `ConditionRepair - 15.00% repair of an item with the Damaged Tool tag` | Targets a damaged tagged item and repairs condition rather than deleting the input. Alias: `Repair`. |

## Tool Types

Valid tool states are the `DesiredItemState` enum names. In seeded craft examples, the useful ones are usually `Held`, `Wielded`, `Worn`, and `InRoom`.

| Type | Syntax | Notes |
| --- | --- | --- |
| `TagTool` | `TagTool - Held - an item with the Sewing Needle tag` | Requires any item with the tag in the desired state. Alias: `Tag`. |
| `SimpleTool` | `SimpleTool - InRoom - a cast-iron stove (#456)` | Requires one exact item prototype in the desired state. Alias: `Simple`. |

Tools currently default to quality weight `1.0` and `UseToolDuration = true`.

## Product Types

Products and fail products use the same grammar.

| Type | Syntax | Notes |
| --- | --- | --- |
| `SimpleProduct` | `SimpleProduct - 1x a padded @material gambeson (#274)` | Loads item prototypes. Option: `skin <id|name>`. |
| `CookedFoodProduct` | `CookedFoodProduct - 1x a baked apple (#999); purify off; ingredient $i1=fruit` | Loads prepared food and transfers consumed inputs into the food ledger. Aliases: `CookedFood`, `Cooked`. |
| `SimpleVariableProduct` | `SimpleVariableProduct - 1x a dyed cloak (#1001); variable Colour=$i1` | Copies characteristic values from `IVariableInput` sources. Alias: `Variable`. |
| `InputVariable` | `InputVariable - 1x a filled vial (#1002); variable Colour=$i1; specific Colour: red dye (#501)=red, blue dye (#502)=blue` | Chooses output characteristic values based on which input item prototype was used. |
| `ProgVariableProduct` | `ProgVariableProduct - 1x a patterned cloth (#1003); variable Pattern=SelectPatternFromInputs` | Uses numeric FutureProgs to choose characteristic value IDs. Alias: `ProgVariable`. |
| `CommodityProduct` | `CommodityProduct - 500 grams of beeswax commodity; tag Candle Wax; characteristic Colour=yellow; characteristic Origin from $i1` | Produces commodity piles. Alias: `Commodity`. |
| `MoneyProduct` | `MoneyProduct - 10.00 of Gondorian Penny` | Produces a currency pile. The amount is the currency base amount. Alias: `Money`. |
| `NPCProduct` | `NPCProduct - 1x common rat (#42); prog SetupSpawnedRat` | Spawns NPCs from a template. Optional `prog` runs after load. Alias: `NPC`. |
| `Prog` | `Prog - LoadCraftProductsFromInputs` | Lets a FutureProg produce item or item collection products. Alias: `ProgProduct`. |
| `DNATest` | `DNATest - compare $i1 and $i2` | Compares two liquid-consuming inputs. |
| `BloodTyping` | `BloodTyping - test $i1 against the ABO blood model` | Tests one liquid-consuming input against a blood model. The older verbose syntax with liquid amount also works. |
| `ScrapInput` | `ScrapInput - 40.00% by weight of 1x an item with the Sheet Metal tag ($i1); tag Scrap Metal` | Converts part of an item input's weight into commodity salvage. Alias: `Scrap`. |
| `UnusedInput` | `UnusedInput - 50.00% of 20x an item with the Padding tag ($i3)` | Returns copies of a portion of a consumed input. |

## Material-Inheriting Products

Use `productMaterialInputIndexes` and `failProductMaterialInputIndexes` when an output should inherit material from an input:

```csharp
[(1, 1)]
```

That means product 1 inherits material from input 1. The values are one-based to match `$p1` and `$i1` echo notation. The helper converts them to the zero-based index expected by the runtime.

## Examples

### Simple Assembly

```csharp
AddCraft(
	"assemble simple spear",
	"Weaponcrafting",
	"assemble a simple spear",
	"assembling a simple spear",
	"an unfinished spear assembly",
	"HasWeaponcrafting",
	null,
	null,
	null,
	_traits["Weaponcrafting"] ?? _traits["Weaponsmith"] ?? _traits.First().Value,
	Difficulty.Normal,
	Outcome.MinorFail,
	5,
	3,
	false,
	[
		(30, "$0 fit|fits $i2 onto $i1.", "$0 fumble|fumbles the fitting of $i2 onto $i1."),
		(30, "$0 secure|secures the head with $i3 and $t1.", "$0 ruin|ruins the binding but saves $f1."),
		(20, "$0 set|sets aside $p1.", "$0 set|sets aside $f1.")
	],
	[
		"Tag - 1x an item with the Pole tag",
		"Tag - 1x an item with the Spearhead tag",
		"Tag - 1x an item with the Tie tag"
	],
	[
		"TagTool - Held - an item with the Pliers tag"
	],
	[
		"SimpleProduct - 1x a long, reinforced pike with &a_an[@material] spearhead (#13)"
	],
	[
		"SimpleProduct - 1x &a_an[@material] spearhead (#214)"
	],
	[(1, 2)],
	[(1, 2)]);
```

### Prepared Food With Ingredient Roles

```csharp
AddCraft(
	"bake spiced apple",
	"Cooking",
	"bake a spiced apple",
	"baking a spiced apple",
	"an apple-baking process",
	"HasCooking",
	null,
	null,
	null,
	_traits["Cooking"] ?? _traits["Cook"] ?? _traits.First().Value,
	Difficulty.Easy,
	Outcome.MinorFail,
	3,
	2,
	true,
	[
		(30, "$0 score|scores $i1 and dust|dusts it with $i2.", "$0 spill|spills too much $i2 over $i1."),
		(60, "$0 bake|bakes $i1 into $p1.", "$0 scorch|scorches the apple and ends up with $f1.")
	],
	[
		"Tag - 1x an item with the Apple tag",
		"Tag - 1x an item with the Spice tag"
	],
	[
		"TagTool - InRoom - an item with the Oven tag"
	],
	[
		"CookedFoodProduct - 1x a baked apple (#999); ingredient $i1=fruit, $i2=seasoning"
	],
	[
		"SimpleProduct - 1x some burnt food (#998)"
	]);
```

## Recommended Upgrades

The helper is useful, but it is still a spreadsheet import shim. The next version would be easier to maintain if it moved toward a typed seeder API while keeping this compact syntax as an optional import layer.

Recommended upgrades:

- Add a trait-gating overload such as `AddCraft(..., string traitName, int? minimumTraitValue)` that creates deterministic `Appear`, `CanUse`, and `WhyCannotUse` progs internally.
- Make crafts upsert by stable name/category instead of always appending rows. The seeder rework will be safer if reruns can update stock craft suites deterministically.
- Replace raw strings with typed records like `CraftInputSpec`, `CraftToolSpec`, and `CraftProductSpec`, plus parser helpers for spreadsheet import. That keeps bulk authoring easy while making tests and refactors less brittle.
- Add per-input, per-tool, and per-product quality weights. The current helper always uses `1.0`.
- Add tool options for `UseToolDuration` and quality weighting, especially for power tools, consumable hand tools, and high-quality apparatus.
- Add phase metadata for exertion and stamina usage. The current tuple only captures seconds and echoes, but runtime phases can carry more behaviour.
- Add craft-wide start and cancel progs. The helper exposes completion only, even though runtime crafts also support start and cancel hooks.
- Add first-class support for commodity input characteristic requirements, commodity pile tags, exact-material `SimpleMaterial` inputs, and fixed or copied commodity output characteristics through typed specs.
- Make validation report all unresolved references with craft name and string text instead of returning `null` silently. That would make large content-suite imports much easier to debug.
- Add focused tests for every conversion grammar. A simple parser test suite would catch broken regexes before a large stock-content import silently skips crafts.
