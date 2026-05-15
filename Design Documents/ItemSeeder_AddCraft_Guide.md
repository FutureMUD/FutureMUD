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

## Where To Invoke It

Stock craft definitions live in `SeedCrafts()` in `DatabaseSeeder/Seeders/ItemSeederCrafting.cs`.

Add new calls after `_nextId` has been reset and after the item prototypes, tags, materials, liquids, traits, and FutureProgs they reference have already been seeded into the lookup dictionaries. In normal `ItemSeeder` flow, `CreateProgs()` and `SeedItems()` run before `SeedCrafts()`, so stock craft calls can use:

- `_traits["Skill Name"]` for the check trait
- named FutureProgs already cached in `_progs`
- item prototype references by `#id`
- tag/material/liquid/currency/NPC/blood-model names already present in the database

The helper is private to `ItemSeederCrafting.cs`, so agents should produce calls in that file rather than trying to call it from another seeder.

All names and `#id` values in examples below are templates. Before committing a generated craft, replace them with real seeded FutureProg names, trait names, tags, materials, liquids, currencies, NPC templates, blood models, and item prototype IDs that exist in the target database seed flow.

## Choosing An Overload

Use the normal import overload when the craft should rely on pre-existing access progs:

```csharp
AddCraft(
	name,
	category,
	blurb,
	action,
	itemsdesc,
	appearProg,
	canUseProg,
	whyCantProg,
	onFinishProg,
	trait,
	difficulty,
	threshold,
	freeChecks,
	failPhase,
	interrupatable,
	phases,
	inputs,
	tools,
	products,
	failProducts,
	productMaterialInputIndexes,
	failProductMaterialInputIndexes,
	onStartProg: null,
	onCancelProg: null);
```

Use the trait-gated import overload when the craft should generate its own deterministic appear/can/why progs from a trait name:

```csharp
AddCraft(
	name,
	category,
	blurb,
	action,
	itemsdesc,
	traitName,
	minimumTraitValue,
	difficulty,
	threshold,
	freeChecks,
	failPhase,
	interrupatable,
	phases,
	inputs,
	tools,
	products,
	failProducts,
	productMaterialInputIndexes,
	failProductMaterialInputIndexes,
	onFinishProg: null,
	onStartProg: null,
	onCancelProg: null);
```

Use the knowledge-gated import overload when a craft requires a stock `IKnowledge` record, with an optional minimum value in the same trait used for the craft check:

```csharp
AddCraft(
	name,
	category,
	blurb,
	action,
	itemsdesc,
	knowledgeName,
	traitName,
	minimumTraitValue,
	difficulty,
	threshold,
	freeChecks,
	failPhase,
	interrupatable,
	phases,
	inputs,
	tools,
	products,
	failProducts,
	productMaterialInputIndexes,
	failProductMaterialInputIndexes,
	onFinishProg: null,
	onStartProg: null,
	onCancelProg: null,
	knowledgeType: "Crafting",
	knowledgeSubtype: "General",
	knowledgeDescription: null,
	knowledgeLongDescription: null);
```

Use the typed spec overload only when the seeder code needs phase exertion/stamina metadata or a non-string authoring surface:

```csharp
AddCraft(new CraftDefinitionSpec
{
	Name = "...",
	Category = "...",
	AppearProg = _progs["HasBlacksmithing"],
	Trait = _traits["Blacksmithing"] ?? _traits["Blacksmith"] ?? _traits.First().Value,
	Phases = [...],
	Inputs = [...],
	Tools = [...],
	Products = [...],
	FailProducts = [...]
});
```

The import overload is usually the right choice for stock craft catalogue work because it is compact and matches the existing data style.

## Invocation Parameter Checklist

For the normal import overload, fill parameters in this order:

| Parameter | What To Put There |
| --- | --- |
| `name` | Stable craft name. This combines with `category` for rerun skip detection. |
| `category` | Craft category shown to builders and players, such as `Weaponcrafting`. |
| `blurb` | Short command-style description, such as `forge a sword blade`. |
| `action` | Active action text, such as `forging a sword blade`. |
| `itemsdesc` | Short description for the temporary active craft item. |
| `appearProg` | Existing FutureProg function name that decides whether the craft appears in craft lists. |
| `canUseProg` | Existing FutureProg function name that gates starting the craft, or `null`. |
| `whyCantProg` | Existing FutureProg function name that explains `canUseProg` failure, or `null`. |
| `onFinishProg` | Existing FutureProg function name run on successful completion, or `null`. |
| `trait` | TraitDefinition used for the skill check. Use `_traits["Name"] ?? fallback`. |
| `difficulty` | `Difficulty` enum value for the check. |
| `threshold` | `Outcome` value at or below which the craft fails. |
| `freeChecks` | Number of practical/free skill checks granted by the craft. Existing stock crafts usually use `5`. |
| `failPhase` | One-based phase number where success/failure is decided. |
| `interrupatable` | Whether the active craft can pause/resume. The parameter is misspelled in code; pass `true` or `false` positionally. |
| `phases` | Ordered phase tuples: `(seconds, success echo, fail echo)`. |
| `inputs` | Ordered input import strings. These are `$i1`, `$i2`, etc. |
| `tools` | Ordered tool import strings. These are `$t1`, `$t2`, etc. |
| `products` | Ordered success product import strings. These are `$p1`, `$p2`, etc. |
| `failProducts` | Ordered fail product import strings. These are `$f1`, `$f2`, etc. |
| `productMaterialInputIndexes` | Optional one-based `(Product, Input)` pairs for success product material inheritance. |
| `failProductMaterialInputIndexes` | Optional one-based `(Product, Input)` pairs for fail product material inheritance. |
| `onStartProg` | Optional named argument for a start hook. |
| `onCancelProg` | Optional named argument for a cancel hook. |

For the trait-gated overload, replace `appearProg`, `canUseProg`, `whyCantProg`, and `trait` with:

| Parameter | What To Put There |
| --- | --- |
| `traitName` | Trait name to look up in `_traits`, such as `"Weaponcrafting"`. |
| `minimumTraitValue` | Minimum trait value for visibility/use, or `null` to require only skill ownership. |

For the knowledge-gated overload, replace `appearProg`, `canUseProg`, `whyCantProg`, and `trait` with:

| Parameter | What To Put There |
| --- | --- |
| `knowledgeName` | Stable `Knowledges.Name` value to require. The helper upserts this row if it does not exist. |
| `traitName` | Trait name to look up in `_traits`; this remains the craft's check trait. |
| `minimumTraitValue` | Optional minimum trait value for access. Pass `null` for knowledge-only gating. |
| `knowledgeType` | Optional named argument for `Knowledges.Type`; use a broad grouping such as `Crafting`. |
| `knowledgeSubtype` | Optional named argument for `Knowledges.Subtype`; use a tighter grouping such as `Blacksmithing` or `Tailoring`. |
| `knowledgeDescription` | Optional short player-facing description of the knowledge. |
| `knowledgeLongDescription` | Optional longer builder/player description of what the knowledge represents. |

The generated access progs check the knowledge by ID. If `minimumTraitValue` is not `null`, they also check `GetTrait(@ch, ToTrait("<trait id>")) >= minimumTraitValue`.

## Descriptive Text Fields

These fields are visible in different parts of the crafting workflow. Write them as player-facing text, not internal notes.

| Field | How The Engine Uses It | Writing Style |
| --- | --- | --- |
| `name` | Stable craft identifier and the main lookup/list name. `name + category` is also the rerun skip key. | Lower-case verb phrase, no period. Prefer `forge sword blade`, `assemble simple widget`, `sew padded vest`. Keep it stable once shipped. |
| `category` | Groups crafts in craft lists and builder displays. | Short stable category, usually title case or established stock spelling such as `Weaponcrafting`, `Armorcrafting`, `Cooking`. Do not make one-off categories unless the catalogue genuinely needs one. |
| `blurb` | Shown in craft detail output as the quick explanation of what the craft does. | Lower-case imperative/infinitive phrase with an object, no period: `forge a sword blade`, `temper a fine blade`, `assemble a simple widget`. |
| `action` | Used by active-craft effects and interruption messages such as "You can't move while you are ...". | Lower-case gerund phrase that reads after "are": `forging a sword blade`, `assembling a simple widget`, `sewing a padded vest`. |
| `itemsdesc` | Short description of the temporary active craft item/process in the room. | Lower-case noun phrase, normally with an article: `a sword-blademaking event`, `an unfinished sword assembly`, `a widget assembly process`. It should describe the visible work-in-progress, not the finished item unless that is literally what is present. |

Echoes are emote text. Use `$0` for the crafter, `$iN` for inputs, `$tN` for tools, `$pN` for success products, and `$fN` for fail products. Keep grammar natural with FutureMUD's `verb|verbs` format, for example `$0 hammer|hammers $i1 against $t2.`

## Minimal Import Example

This is the smallest practical shape for a normal stock craft:

```csharp
AddCraft(
	"assemble simple widget",
	"Engineering",
	"assemble a simple widget",
	"assembling a simple widget",
	"a widget assembly process",
	"HasEngineering",
	null,
	null,
	null,
	_traits["Engineering"] ?? _traits["Engineer"] ?? _traits.First().Value,
	Difficulty.Normal,
	Outcome.MinorFail,
	5,
	2,
	false,
	[
		(30, "$0 align|aligns $i1 with $i2.", "$0 align|aligns $i1 with $i2."),
		(45, "$0 fasten|fastens the pieces with $t1 and set|sets aside $p1.", "$0 spoil|spoils the assembly and recovers $f1.")
	],
	[
		"SimpleItem - 1x a metal widget frame (#1200)",
		"Tag - 4x an item with the Widget Fastener tag; quality 0.5"
	],
	[
		"TagTool - Held - an item with the Wrench tag; quality 1.5; usetool on"
	],
	[
		"SimpleProduct - 1x a finished @material widget (#1201)"
	],
	[
		"UnusedInput - 50.00% of 4x an item with the Widget Fastener tag ($i2)"
	],
	[(1, 1)]);
```

Important details in that example:

- `failPhase` is `2`, so unreferenced products would resolve at phase 2.
- `$i1`, `$i2`, `$t1`, `$p1`, and `$f1` correspond to the first input, second input, first tool, first success product, and first fail product.
- `[(1, 1)]` means success product 1 inherits material from input 1.
- The input quality option affects `CraftInput.InputQualityWeight`.
- The tool quality and `usetool` options affect `CraftTool.ToolQualityWeight` and `CraftTool.UseToolDuration`.
- The phase 1 fail echo repeats the success echo because failure has not been decided yet. Put distinctive failure narration only on `failPhase` or later.

## Full Import Example With Hooks

Use named optional arguments for lifecycle hooks so the meaning is obvious:

```csharp
AddCraft(
	"forge hooked blade",
	"Weaponcrafting",
	"forge a hooked blade",
	"forging a hooked blade",
	"a hooked blade forging process",
	"HasWeaponcrafting",
	"CanUseForge",
	"WhyCannotUseForge",
	"ApplyConstructionCooldown",
	_traits["Weaponcrafting"] ?? _traits["Weaponsmith"] ?? _traits.First().Value,
	Difficulty.Hard,
	Outcome.MinorFail,
	5,
	4,
	false,
	[
		(25, "$0 heat|heats $i1 in $t1.", "$0 heat|heats $i1 in $t1."),
		(45, "$0 hammer|hammers $i1 against $t2.", "$0 hammer|hammers $i1 against $t2."),
		(40, "$0 quench|quenches $i1 in $i2.", "$0 quench|quenches $i1 in $i2."),
		(30, "$0 finish|finishes $p1.", "$0 salvage|salvages $f1.")
	],
	[
		"CommodityTag - 700 grams of a material tagged as Forged Metal; piletag Blade Stock; characteristic Temper any",
		"LiquidUse - 1 litre of Water"
	],
	[
		"TagTool - InRoom - an item with the Hot Fire tag; usetool off",
		"TagTool - Held - an item with the Hammer tag; quality 2.0"
	],
	[
		"SimpleProduct - 1x &a_an[@material] hooked blade (#1300)"
	],
	[
		"SimpleProduct - 1x a twisted hunk of @material (#169)"
	],
	[(1, 1)],
	[(1, 1)],
	onStartProg: "LogCraftStarted",
	onCancelProg: "LogCraftCancelled");
```

Use `onFinishProg` in the positional completion-prog slot. Use `onStartProg:` and `onCancelProg:` by name at the end.

## Trait-Gated Example

The trait-gated overload creates appear/can/why progs automatically. Use it when the only access rule is skill ownership or a minimum skill value:

```csharp
AddCraft(
	"temper fine blade",
	"Weaponcrafting",
	"temper a fine blade",
	"tempering a fine blade",
	"a blade tempering process",
	"Weaponcrafting",
	46,
	Difficulty.Hard,
	Outcome.MinorFail,
	5,
	3,
	false,
	[
		(30, "$0 heat|heats $i1 in $t1.", "$0 heat|heats $i1 in $t1."),
		(45, "$0 work|works the blade with $t2.", "$0 work|works the blade with $t2."),
		(30, "$0 set|sets aside $p1.", "$0 set|sets aside $f1.")
	],
	[
		"SimpleItem - 1x &a_an[@material] sword blade (#217)"
	],
	[
		"TagTool - InRoom - an item with the Hot Fire tag",
		"TagTool - Held - an item with the Hammer tag"
	],
	[
		"SimpleProduct - 1x &a_an[@material] tempered sword blade (#1301)"
	],
	[
		"SimpleProduct - 1x &a_an[@material] sword blade (#217)"
	],
	[(1, 1)],
	[(1, 1)]);
```

That call generates and uses:

```text
ItemSeederAppearWeaponcrafting46
ItemSeederCanUseWeaponcrafting46
ItemSeederWhyCannotUseWeaponcrafting46
```

Pass `null` instead of `46` if merely having the trait is enough.

## Knowledge-Gated Examples

Use the knowledge-gated overload when a craft needs a learnable `IKnowledge` gate. The craft still rolls the `traitName` trait for its normal crafting check; `minimumTraitValue` only controls access.

Knowledge-only gate:

```csharp
AddCraft(
	"cut master pattern",
	"Tailoring",
	"cut a master garment pattern",
	"cutting a master garment pattern",
	"a master pattern-cutting process",
	"Master Pattern Cutting",
	"Tailoring",
	null,
	Difficulty.Hard,
	Outcome.MinorFail,
	5,
	3,
	false,
	[
		(30, "$0 measure|measures $i1 against $i2.", "$0 measure|measures $i1 against $i2."),
		(45, "$0 mark|marks careful lines over $i1 with $t1.", "$0 mark|marks careful lines over $i1 with $t1."),
		(30, "$0 cut|cuts the pattern cleanly and set|sets aside $p1.", "$0 cut|cuts the pattern poorly and recovers $f1.")
	],
	[
		"Tag - 1x an item with the Pattern Paper tag",
		"Tag - 1x an item with the Finished Garment tag"
	],
	[
		"TagTool - Held - an item with the Tailor Chalk tag",
		"TagTool - Held - an item with the Scissors tag"
	],
	[
		"SimpleProduct - 1x a master garment pattern (#1500)"
	],
	[
		"UnusedInput - 50.00% of 1x an item with the Pattern Paper tag ($i1)"
	],
	knowledgeType: "Crafting",
	knowledgeSubtype: "Tailoring",
	knowledgeDescription: "master garment pattern cutting",
	knowledgeLongDescription: "Specialised tailoring knowledge for drafting reusable master garment patterns.");
```

Knowledge plus minimum skill gate:

```csharp
AddCraft(
	"temper pattern-welded blade",
	"Weaponcrafting",
	"temper a pattern-welded blade",
	"tempering a pattern-welded blade",
	"a pattern-welded blade tempering process",
	"Pattern Welding",
	"Weaponcrafting",
	55,
	Difficulty.VeryHard,
	Outcome.MinorFail,
	5,
	4,
	false,
	[
		(30, "$0 heat|heats $i1 evenly in $t1.", "$0 heat|heats $i1 evenly in $t1."),
		(45, "$0 watch|watches the pattern colours shift across $i1.", "$0 watch|watches the pattern colours shift across $i1."),
		(45, "$0 quench|quenches $i1 in $i2.", "$0 quench|quenches $i1 in $i2."),
		(30, "$0 polish|polishes the blade and set|sets aside $p1.", "$0 inspect|inspects the warped blade and recovers $f1.")
	],
	[
		"SimpleItem - 1x &a_an[@material] pattern-welded sword blade (#1501)",
		"LiquidUse - 1 litre of Oil"
	],
	[
		"TagTool - InRoom - an item with the Hot Fire tag",
		"TagTool - Held - an item with the Forge Tongs tag"
	],
	[
		"SimpleProduct - 1x &a_an[@material] tempered pattern-welded blade (#1502)"
	],
	[
		"SimpleProduct - 1x a warped @material blade blank (#1503)"
	],
	[(1, 1)],
	[(1, 1)],
	knowledgeType: "Crafting",
	knowledgeSubtype: "Weaponcrafting",
	knowledgeDescription: "pattern welding",
	knowledgeLongDescription: "Specialised weaponsmithing knowledge for controlling pattern-welded metals during tempering.");
```

Those calls upsert the named knowledge and generate deterministic progs in these shapes:

```text
ItemSeederAppearKnowledge<Knowledge>
ItemSeederCanUseKnowledge<Knowledge>
ItemSeederWhyCannotUseKnowledge<Knowledge>

ItemSeederAppearKnowledge<Knowledge><Trait><Minimum>
ItemSeederCanUseKnowledge<Knowledge><Trait><Minimum>
ItemSeederWhyCannotUseKnowledge<Knowledge><Trait><Minimum>
```

## Typed Spec Example

Use the typed API when you need phase exertion or stamina:

```csharp
AddCraft(new CraftDefinitionSpec
{
	Name = "polish precision lens",
	Category = "Glassworking",
	Blurb = "polish a precision lens",
	Action = "polishing a precision lens",
	ActiveCraftItemSdesc = "a precision lens polishing process",
	AppearProg = _progs["HasGlassworking"],
	CanUseProg = null,
	WhyCannotUseProg = null,
	OnStartProg = _progs["LogCraftStarted"],
	OnFinishProg = _progs["LogCraftCompleted"],
	OnCancelProg = _progs["LogCraftCancelled"],
	Trait = _traits["Glassworking"] ?? _traits["Glassworker"] ?? _traits.First().Value,
	Difficulty = Difficulty.Hard,
	Threshold = Outcome.MinorFail,
	FreeChecks = 5,
	FailPhase = 2,
	Interruptable = false,
	Phases =
	[
		new CraftPhaseSpec
		{
			Seconds = 60,
			Echo = "$0 grind|grinds $i1 with $t1.",
			Exertion = ExertionLevel.Low,
			Stamina = 1.0
		},
		new CraftPhaseSpec
		{
			Seconds = 90,
			Echo = "$0 polish|polishes $i1 into $p1.",
			FailEcho = "$0 scratch|scratches $i1 and recovers $f1.",
			Exertion = ExertionLevel.Normal,
			Stamina = 2.0
		}
	],
	Inputs =
	[
		new CraftInputSpec(
			"SimpleMaterial - 1x an item made of glass; quality 1.25",
			"SimpleMaterial",
			"1x an item made of glass",
			["quality 1.25"],
			1.25)
	],
	Tools =
	[
		new CraftToolSpec(
			"TagTool - Held - an item with the Lens Polisher tag; quality 2.0; usetool on",
			"TagTool",
			"Held - an item with the Lens Polisher tag",
			["quality 2.0", "usetool on"],
			2.0,
			true)
	],
	Products =
	[
		new CraftProductSpec(
			"SimpleProduct - 1x a polished @material lens (#1400)",
			"SimpleProduct",
			"1x a polished @material lens (#1400)",
			[],
			false,
			0)
	],
	FailProducts =
	[
		new CraftProductSpec(
			"SimpleProduct - 1x a scratched @material lens blank (#1401)",
			"SimpleProduct",
			"1x a scratched @material lens blank (#1401)",
			[],
			true,
			0)
	]
});
```

When writing typed specs by hand:

- `ImportText` should still be the full original string because validation errors echo it back.
- `InputType`, `ToolType`, and `ProductType` should be the import type token, such as `Tag`, `TagTool`, or `SimpleProduct`.
- `Details` should be the part after `Type - ` and before any semicolon options.
- `Options` should be each semicolon option without the semicolon.
- `MaterialDefiningInputIndex` is zero-based in `CraftProductSpec`; use `0` for `$i1`.
- If `CraftPhaseSpec.FailEcho` is blank, the seeder persists the normal `Echo` as the fail echo. This is useful before `FailPhase`, where the failure path has not diverged yet.

## Rerun Behavior

Craft creation is insert-or-skip by stable `Name + Category`.

If a matching craft already exists, `AddCraft` returns that existing row unchanged. It does not refresh the craft and does not add duplicate phases, inputs, tools, products, or fail products.

The seed craft call is still the base install truth. If a craft has incorrect visible text, inputs, tools, products, or metadata, fix the `AddCraft` call or typed spec so a clean install creates the correct craft directly. Do not seed a wrong craft and then rely on a second correction pass for new databases.

The skip happens before parsing imports or generating/upserting trait and knowledge helper progs. This protects already-installed craft rows from accidental duplicate child records. If an existing database needs a stock-craft compatibility update, implement that as explicit, narrow migration or compatibility work with documented stock ownership rather than by rerunning the same craft call.

This is deliberate narrow repeatability for stock craft rows only. It is not a claim that all `ItemSeeder` item/component content is fully repeatable or repair-capable.

## Validation

New craft specs are validated before insertion. Validation accumulates all input, tool, product, fail-product, phase, trait, and prog errors it can find, then throws one `ApplicationException`.

Error lines include:

- craft name
- spec kind, such as `input`, `tool`, `product`, or `fail product`
- original import text where the error came from
- the specific lookup or grammar failure

Existing `Name + Category` rows skip before validation because the chosen rerun policy is "do not touch existing stock craft rows".

## Lifecycle, Trait, And Knowledge Progs

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

Knowledge-gated overloads accept `knowledgeName`, `traitName`, and optional `minimumTraitValue`. They upsert the `Knowledges` row by name and create deterministic FutureProgs:

```text
ItemSeederAppearKnowledge<Knowledge><TraitMinimum?>
ItemSeederCanUseKnowledge<Knowledge><TraitMinimum?>
ItemSeederWhyCannotUseKnowledge<Knowledge><TraitMinimum?>
```

Without a minimum, the generated boolean progs check `@ch.Knowledges.Any(x, @x.Id == <knowledge id>)`. With a minimum, they first check the knowledge and then check the trait value. The generated why prog explains the missing knowledge before the missing trait value.

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

Failure is not decided until `failPhase`. Before that phase:

- Fail echoes are not used as failure output before `failPhase`, so they should not carry meaningful failure narration. In tuple import calls, repeat the normal echo before `failPhase`; in typed specs, omit `FailEcho` to have the seeder copy `Echo`.
- Do not reference `$fN` before `failPhase`. Fail products before the fail phase should never be used.
- Avoid referencing `$pN` before `failPhase` unless the product is intentionally created before success/failure diverges and should exist even if the later check fails.

At `failPhase` and later, write distinct fail echoes where useful. This is where `$fN` belongs, and where success echoes should usually introduce `$pN`.
