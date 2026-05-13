# ItemSeeder AddCraft Authoring Guide

This guide documents the craft-authoring slice in `DatabaseSeeder/Seeders/ItemSeederCrafting.cs`.

`AddCraft` now uses a typed seeder API internally. The compact spreadsheet-style strings are still supported, but they are treated as an import layer: strings are parsed into typed phase, input, tool, and product specs, validated as a group, and then inserted through one implementation path.

For the broader runtime and builder model, see:

- `Design Documents/Crafting_System_Overview.md`
- `Design Documents/Crafting_System_Runtime_and_Extensibility.md`
- `Design Documents/Crafting_System_Builder_Workflows.md`

## Typed Shape

The core typed specs are:

| Spec | Purpose |
| --- | --- |
| `CraftDefinitionSpec` | Craft metadata, access progs, lifecycle progs, trait check, phases, inputs, tools, products, and fail products. |
| `CraftPhaseSpec` | Phase seconds, success echo, fail echo, exertion level, and stamina use. |
| `CraftInputSpec` | Original import text, input type, main details, import options, and input quality weight. |
| `CraftToolSpec` | Original import text, tool type, main details, import options, tool quality weight, and `UseToolDuration`. |
| `CraftProductSpec` | Original import text, product type, main details, import options, success/fail status, and material-defining input index. |

The old string-heavy `AddCraft` overload remains for stock spreadsheet imports. It builds these specs and calls the typed implementation.

## Rerun Behavior

Craft creation is insert-or-skip by stable `Name + Category`.

If a matching craft already exists, `AddCraft` returns that existing row unchanged. It does not refresh the craft and does not add duplicate phases, inputs, tools, products, or fail products.

This is deliberate narrow repeatability for stock craft rows only. It is not a claim that all `ItemSeeder` item/component content is fully repeatable or repair-capable.

## Validation

New craft specs are validated before insertion. Validation accumulates all input, tool, product, fail-product, phase, trait, and prog errors it can find, then throws one `ApplicationException`.

Error lines include:

- craft name
- spec kind, such as `input`, `tool`, `product`, or `fail product`
- original import text where the error came from
- the specific lookup or grammar failure

Existing `Name + Category` rows skip before validation because the chosen rerun policy is "do not touch existing stock craft rows".

## Lifecycle And Trait Progs

Typed specs support all craft lifecycle hooks:

- `OnStartProg`
- `OnFinishProg`
- `OnCancelProg`

The legacy string overload still exposes completion directly and also accepts optional start/cancel prog names.

Trait-gated overloads accept a `traitName` and optional `minimumTraitValue`. They create deterministic FutureProgs:

```text
ItemSeederAppear<Trait><Minimum?>
ItemSeederCanUse<Trait><Minimum?>
ItemSeederWhyCannotUse<Trait><Minimum?>
```

Without a minimum, the generated boolean progs check whether the character has the skill. With a minimum, they check `GetTrait(@ch, ToTrait("<id>")) >= minimum`. The generated why prog returns a short builder-safe explanation.

## Conversion String Basics

Inputs, tools, and products still use this shape:

```text
TypeName - details; option; option
```

Tools include desired item state:

```text
TagTool - Held - an item with the Sewing Needle tag
SimpleTool - InRoom - a cast-iron stove (#456)
```

Type names and options are case-insensitive.

## Shared Import Options

Inputs and tools support:

```text
quality <number>
```

Examples:

```text
Tag - 2x an item with the Thread tag; quality 2.5
TagTool - Held - an item with the Sewing Needle tag; quality 1.75
```

Tools also support:

```text
usetool on
usetool off
```

This controls the persisted `CraftTool.UseToolDuration` flag. The default is `on`.

Products do not currently have a runtime quality-weight contract. Per-product quality weights are intentionally deferred until the craft runtime has a field and behavior for them.

## Input Types

| Type | Syntax | Notes |
| --- | --- | --- |
| `Tag` | `Tag - 2x an item with the Thread tag` | Consumes items with the tag. |
| `TagVariable` | `TagVariable - 1x an item with the Dyed Cloth tag with variable Colour, Pattern` | Declares characteristic definitions available to variable products. |
| `SimpleItem` | `SimpleItem - 1x a short shaft (#123)` | Consumes an exact item prototype. The `#id` is preferred. |
| `SimpleMaterial` | `SimpleMaterial - 1x an item with material tagged as Wood` | Consumes an item whose material has the tag. |
| `SimpleMaterial` | `SimpleMaterial - 1x an item made of iron` | Consumes an item whose material is exactly `iron`. |
| `Commodity` | `Commodity - 1 kilogram 500 grams of iron` | Consumes exact material commodity by mass. |
| `CommodityTag` | `CommodityTag - 500 grams of a material tagged as Fabric` | Consumes commodity whose material has the tag. |
| `LiquidUse` | `LiquidUse - 1 litre of Water` | Consumes a specific liquid. Alias: `Liquid`. |
| `LiquidTagUse` | `LiquidTagUse - 250 millilitres of a liquid tagged Cooking Oil` | Consumes a liquid from a tagged source. Alias: `TagLiquid`. |
| `ConditionRepair` | `ConditionRepair - 15.00% repair of an item with the Damaged Tool tag` | Repairs condition rather than deleting the input. Alias: `Repair`. |

Commodity inputs additionally support:

```text
piletag <tag name>
characteristic any
characteristic none
characteristic <definition> any
characteristic <definition> <value>
characteristic <definition>=<value>
```

These write the runtime `CommodityCharacteristicRequirement` XML shape:

```xml
<Characteristics mode="specific">
  <Characteristic definition="1" value="2" />
</Characteristics>
```

## Tool Types

Valid tool states are the `DesiredItemState` enum names. Seeded examples usually use `Held`, `Wielded`, `Worn`, and `InRoom`.

| Type | Syntax | Notes |
| --- | --- | --- |
| `TagTool` | `TagTool - Held - an item with the Sewing Needle tag` | Requires any item with the tag in the desired state. Alias: `Tag`. |
| `SimpleTool` | `SimpleTool - InRoom - a cast-iron stove (#456)` | Requires one exact item prototype in the desired state. Alias: `Simple`. |

## Product Types

Products and fail products use the same grammar.

| Type | Syntax | Notes |
| --- | --- | --- |
| `SimpleProduct` | `SimpleProduct - 1x a padded @material gambeson (#274)` | Loads item prototypes. Option: `skin <id|name>`. |
| `CookedFoodProduct` | `CookedFoodProduct - 1x a baked apple (#999); purify off; ingredient $i1=fruit` | Loads prepared food and transfers consumed inputs into the food ledger. Aliases: `CookedFood`, `Cooked`. |
| `SimpleVariableProduct` | `SimpleVariableProduct - 1x a dyed cloak (#1001); variable Colour=$i1` | Copies characteristic values from `IVariableInput` sources. Alias: `Variable`. |
| `InputVariable` | `InputVariable - 1x a filled vial (#1002); variable Colour=$i1; specific Colour: red dye (#501)=red` | Chooses output characteristic values based on which input item prototype was used. |
| `ProgVariableProduct` | `ProgVariableProduct - 1x a patterned cloth (#1003); variable Pattern=SelectPatternFromInputs` | Uses a FutureProg to choose characteristic output values. Alias: `ProgVariable`. |
| `CommodityProduct` | `CommodityProduct - 500 grams of beeswax commodity; tag Candle Wax; characteristic Colour=yellow; characteristic Origin from $i1` | Produces commodity piles with optional tag and fixed/copied characteristics. Alias: `Commodity`. |
| `MoneyProduct` | `MoneyProduct - 10.00 of Gondorian Penny` | Produces a currency pile. Alias: `Money`. |
| `NPCProduct` | `NPCProduct - 1x common rat (#42); prog SetupSpawnedRat` | Spawns NPCs from a template. Optional `prog` runs after load. Alias: `NPC`. |
| `Prog` | `Prog - LoadCraftProductsFromInputs` | Lets a FutureProg produce item or item collection products. Alias: `ProgProduct`. |
| `DNATest` | `DNATest - compare $i1 and $i2` | Compares two liquid-consuming inputs. |
| `BloodTyping` | `BloodTyping - test $i1 against the ABO blood model` | Tests one liquid-consuming input against a blood model. |
| `ScrapInput` | `ScrapInput - 40.00% by weight of 1x an item with the Sheet Metal tag ($i1); tag Scrap Metal` | Converts part of an item input's weight into commodity salvage. Alias: `Scrap`. |
| `UnusedInput` | `UnusedInput - 50.00% of 20x an item with the Padding tag ($i3)` | Returns copies of a portion of a consumed input. |

## Material-Inheriting Products

Use `productMaterialInputIndexes` and `failProductMaterialInputIndexes` when an output should inherit material from an input:

```csharp
[(1, 1)]
```

That means product 1 inherits material from input 1. The values are one-based to match `$p1` and `$i1` echo notation. The helper converts them to the zero-based index expected by the runtime.

## Phase Echoes

The runtime scans success and fail echoes for tokens and uses them to infer when inputs, tools, and products participate.

| Token | Meaning |
| --- | --- |
| `$iN` | Input N. The first success-echo reference sets the phase where the input is consumed. |
| `$tN` | Tool N. The tool is required in each success phase where it appears. |
| `$pN` | Success product N. The first success-echo reference sets the phase where the product appears. |
| `$fN` | Fail product N. Only valid in fail echoes; the first reference sets the fail-product phase. |

If an input is never referenced, it is consumed at phase 1. If a tool is never referenced, it is required in every normal phase. Unreferenced success and fail products default to `failPhase`.
