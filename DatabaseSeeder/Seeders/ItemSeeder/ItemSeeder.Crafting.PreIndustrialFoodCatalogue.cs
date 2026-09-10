#nullable enable

using MudSharp.Models;
using MudSharp.FutureProg;
using MudSharp.Database;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string PreIndustrialFoodCatalogueKnowledge = "Pre-Industrial Food Catalogue Production";
	private const string PreIndustrialFoodCatalogueCraftCategory = "Pre-Industrial Foodmaking";
	private const string AgricultureFoodSource = "agriculture";
	private const string AnimalButcheryFoodSource = "animal_butchery";
	private const string SharedFoodCatalogueSource = "preindustrial_food_catalogue";
	private const string SharedLiquidVesselSource = "preindustrial_food_liquid_vessel";
	private const string SeededYieldTag = "Seeded Yield";
	private const string RawMilkTag = "Raw Milk";
	private const string EggProductTag = "Egg Product";
	private const string PressedHoneyTag = "Pressed Honey";
	private const string FoodCropMaterialTag = "Food Crop";
	private const string RawNonFishMeatCutTag = "Raw Non-Fish Meat Cut";
	private const string RawFishCutTag = "Raw Fish Cut";
	private const string PreIndustrialFoodLiquidVessel = "preindustrial_food_catalogue_liquid_amphora";
	private static readonly IReadOnlySet<string> AgricultureVegetableMaterials = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		"bean", "chickpea", "corn", "lentil", "pea", "vegetable"
	};
	private static readonly IReadOnlySet<string> AgricultureCropMaterials = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		"barley", "millet", "oat", "rice", "rye", "sorghum", "wheat"
	};

	internal sealed record PreIndustrialFoodCatalogueDependencyTestData(
		string Import,
		string? StableReference,
		string SourceOwner,
		int SourcePhase);

	internal sealed record PreIndustrialFoodCatalogueCraftSpecTestData(
		FoodCatalogueScope Scope,
		string StableReference,
		FoodCatalogueKind Kind,
		FoodCatalogueFamily Family,
		string Trait,
		int MinimumTraitValue,
		Difficulty Difficulty,
		string KnowledgeSubtype,
		int Phase,
		string Category,
		IReadOnlyCollection<string> Inputs,
		IReadOnlyCollection<string> Tools,
		IReadOnlyCollection<string> Products,
		IReadOnlyCollection<PreIndustrialFoodCatalogueDependencyTestData> Dependencies,
		IReadOnlyCollection<string> SourceOwnership);

	private sealed record PreIndustrialFoodCatalogueInput(
		string Import,
		string? StableReference,
		string SourceOwner,
		int SourcePhase);

	private sealed record PreIndustrialFoodCatalogueOutput(
		string Import,
		IReadOnlyList<string> Contracts,
		string? Liquid = null);

	private sealed record PreIndustrialFoodCatalogueCraftSpec(
		FoodCatalogueScope Scope,
		string StableReference,
		FoodCatalogueKind Kind,
		FoodCatalogueFamily Family,
		string Name,
		string Trait,
		int MinimumTraitValue,
		Difficulty Difficulty,
		string KnowledgeSubtype,
		int Phase,
		string Category,
		IReadOnlyList<PreIndustrialFoodCatalogueInput> Inputs,
		PreIndustrialFoodCatalogueOutput Output,
		string? SelectorProgName = null);

	internal static IReadOnlyCollection<PreIndustrialFoodCatalogueCraftSpecTestData>
		PreIndustrialFoodCatalogueCraftSpecsForTesting =>
			PreIndustrialFoodCatalogueCraftSpecs()
				.Select(x =>
				{
					var dependencies = x.Inputs
						.Select(input => new PreIndustrialFoodCatalogueDependencyTestData(
							input.Import,
							input.StableReference,
							input.SourceOwner,
							input.SourcePhase))
						.ToArray();
					return new PreIndustrialFoodCatalogueCraftSpecTestData(
						x.Scope,
						x.StableReference,
						x.Kind,
						x.Family,
						x.Trait,
						x.MinimumTraitValue,
						x.Difficulty,
						x.KnowledgeSubtype,
						x.Phase,
						x.Category,
						x.Inputs.Select(y => y.Import).ToArray(),
						[FoodCatalogueTool(x.Family)],
						x.Output.Contracts,
						dependencies,
						dependencies.Select(y => y.SourceOwner)
							.Append(x.Output.Liquid is null ? SharedFoodCatalogueSource : SharedLiquidVesselSource)
							.Distinct(StringComparer.Ordinal).ToArray());
				})
				.ToArray();

	internal static IReadOnlyCollection<string> PreIndustrialFoodCatalogueOutputContractsForTesting =>
		PreIndustrialFoodCatalogueCraftSpecs()
			.SelectMany(x => x.Output.Contracts)
			.ToArray();

	internal static bool ShouldSeedPreIndustrialFoodCatalogueCraftsForTesting(string? eras)
	{
		return !string.IsNullOrWhiteSpace(eras) &&
			   HasAnyEra(eras, "medieval", "renaissance", "earlymodern");
	}

	internal void SeedPreIndustrialFoodCatalogueCraftsForTesting(FuturemudDatabaseContext context, string eras = "medieval")
	{
		InitialiseCraftAuthoringForTesting(context);
		_questionAnswers = new Dictionary<string, string>
		{
			["eras"] = eras
		};
		var previousDeferCraftProductSave = _deferCraftProductSave;
		_deferCraftProductSave = true;
		try
		{
			SeedPreIndustrialFoodCatalogueCrafts();
		}
		finally
		{
			_deferCraftProductSave = previousDeferCraftProductSave;
		}
		context.SaveChanges();
	}

	private bool ShouldSeedPreIndustrialFoodCatalogueCrafts()
	{
		return _questionAnswers?.TryGetValue("eras", out var eras) == true &&
			   ShouldSeedPreIndustrialFoodCatalogueCraftsForTesting(eras);
	}

	private void SeedPreIndustrialFoodCatalogueCrafts()
	{
		if (!ShouldSeedPreIndustrialFoodCatalogueCrafts())
		{
			return;
		}

		var eras = _questionAnswers!["eras"];
		var scopes = new HashSet<FoodCatalogueScope> { FoodCatalogueScope.Shared };
		if (eras.Contains("medieval", StringComparison.InvariantCultureIgnoreCase))
		{
			scopes.Add(FoodCatalogueScope.Medieval);
		}

		if (eras.Contains("renaissance", StringComparison.InvariantCultureIgnoreCase))
		{
			scopes.Add(FoodCatalogueScope.Renaissance);
		}

		if (eras.Contains("earlymodern", StringComparison.InvariantCultureIgnoreCase))
		{
			scopes.Add(FoodCatalogueScope.EarlyModern);
		}

		var craftSpecs = PreIndustrialFoodCatalogueCraftSpecs()
			.Where(x => scopes.Contains(x.Scope))
			.OrderBy(x => x.Scope)
			.ThenBy(x => x.Kind)
			.ThenBy(x => x.Family)
			.ThenBy(x => x.StableReference, StringComparer.Ordinal)
			.Select(spec => spec.Output.Liquid is null
				? spec with { SelectorProgName = EnsurePreIndustrialFoodCatalogueSelectorProg(spec) }
				: spec)
			.ToArray();
		var selectorProgs = craftSpecs
			.Where(x => x.SelectorProgName is not null)
			.Select(x => _progs[x.SelectorProgName!])
			.ToArray();
		SaveFutureProgsIfRequired(selectorProgs);
		foreach (var craftSpec in craftSpecs)
		{
			AddPreIndustrialFoodCatalogueCraft(craftSpec);
		}
	}

	private Craft? AddPreIndustrialFoodCatalogueCraft(PreIndustrialFoodCatalogueCraftSpec spec)
	{
		var inputImports = spec.Inputs
			.Select(x => x.StableReference is null ? x.Import : StableSimpleItemInput(x.StableReference))
			.ToArray();
		var productImport = spec.Output.Liquid is not null
			? $"LiquidProduct - {StableSimpleItemDescription(PreIndustrialFoodLiquidVessel)} filled with 10 litres of {spec.Output.Liquid}"
			: spec.Kind == FoodCatalogueKind.Prepared
				? $"ProgCookedFoodProduct - {spec.SelectorProgName}"
				: $"ProgProduct - {spec.SelectorProgName}";
		IEnumerable<string> tools = [FoodCatalogueTool(spec.Family)];
		var phaseEchoes = PreIndustrialFoodCatalogueCraftingPhases(spec);
		var action = spec.Kind == FoodCatalogueKind.Intermediate
			? $"preparing {FamilyDisplay(spec.Family).ToLowerInvariant()} stock"
			: $"preparing {FamilyDisplay(spec.Family).ToLowerInvariant()} food";

		return AddCraft(
			spec.Name,
			PreIndustrialFoodCatalogueCraftCategory,
			$"A catalogue recipe for {spec.StableReference}.",
			action,
			$"a {FamilyDisplay(spec.Family).ToLowerInvariant()} preparation",
			PreIndustrialFoodCatalogueKnowledge,
			spec.Trait,
			spec.MinimumTraitValue,
			spec.Difficulty,
			Outcome.MinorFail,
			5,
			3,
			false,
			phaseEchoes,
			inputImports,
			tools,
			[productImport],
			[],
			knowledgeSubtype: spec.KnowledgeSubtype,
			knowledgeDescription: "Generalised production recipes for the shared and era-specific pre-industrial food catalogue.",
			knowledgeLongDescription: "These recipes turn agriculture, animal-product, and earlier catalogue stock into the named pre-industrial dishes and filled food liquids.");
	}

	private string EnsurePreIndustrialFoodCatalogueSelectorProg(PreIndustrialFoodCatalogueCraftSpec spec)
	{
		var progName = $"ItemSeederPreIndustrialFood_{spec.Scope}_{spec.Kind}_{spec.Family}_{SanitiseProgPart(spec.Output.Contracts.First())}";
		var body = new List<string> { "var products as item collection" };
			body.AddRange(spec.Output.Contracts.Select(x => $"additem products loaditem(\"{x}\")"));
		body.Add("return collectionfirst(collectionshuffle(@products))");
		const string generatedComment =
			"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.";
		var prog = EnsureFutureProg(
			progName,
			"Crafting",
			"Pre-Industrial Food Catalogue",
			ProgVariableTypes.Item,
			generatedComment,
			[],
			string.Join(Environment.NewLine, body));
		var expectedBody = string.Join(Environment.NewLine, body);
		if (prog.FunctionComment.Equals(generatedComment, StringComparison.Ordinal) &&
			(!prog.FunctionText.Equals(expectedBody, StringComparison.Ordinal) ||
			 prog.ReturnType != (long)ProgVariableTypes.Item))
		{
			prog.FunctionText = expectedBody;
			prog.ReturnType = (long)ProgVariableTypes.Item;
		}

		return progName;
	}

	private static string SanitiseProgPart(string value)
	{
		var chars = value.Select(x => char.IsLetterOrDigit(x) ? x : '_').ToArray();
		return new string(chars).Trim('_');
	}

	private static IEnumerable<(int Seconds, string Echo, string FailEcho)> PreIndustrialFoodCatalogueCraftingPhases(
		PreIndustrialFoodCatalogueCraftSpec spec)
	{
		var subject = FamilyDisplay(spec.Family).ToLowerInvariant();
		if (spec.Output.Liquid is not null)
		{
			return spec.Family switch
			{
				FoodCatalogueFamily.GrainDrink or FoodCatalogueFamily.FermentedDrink or
				FoodCatalogueFamily.Wine or FoodCatalogueFamily.Spirit =>
				[
					(15, $"$0 crush|crushes $i1 and stir|stirs it into the brewing vessel.",
						$"$0 spill|spills the grain and lose|loses the cleanest portion before the mash can begin."),
					(25, $"$0 mash|mashes the {subject} stock in hot water, then draw|draws off the sweet wort.",
						$"$0 scorch|scorches the mash and have|has to discard the bitter liquid."),
					(20, $"$0 strain|strains $p1 and set|sets it aside to ferment and settle.",
						$"$0 taste|tastes the failed brew and set|sets it aside as sour stock.")
				],
				FoodCatalogueFamily.FruitDrink =>
				[
					(15, $"$0 crush|crushes $i1 and load|loads the fruit into the press.",
						$"$0 bruise|bruises the fruit and spill|spills the clean juice into the work area."),
					(25, $"$0 press|presses the fruit down and draw|draws off its bright juice.",
						$"$0 press|presses unevenly and leave|leaves the must cloudy and thin."),
					(20, $"$0 strain|strains $p1 and set|sets the drink aside to settle.",
						$"$0 strain|strains the poor drink and save|saves what can be used in a sauce.")
				],
				FoodCatalogueFamily.Oil =>
				[
					(15, $"$0 crush|crushes $i1 into a damp oilseed mash.",
						$"$0 scatter|scatters the oilseed and gather|gathers the usable mash again."),
					(25, $"$0 press|presses the mash hard, letting the clear oil run free.",
						$"$0 overfill|overfills the press and lose|loses much of the oil into the cake."),
					(20, $"$0 decant|decants $p1 and set|sets the oil aside to settle.",
						$"$0 skim|skims the cloudy oil and save|saves it for rough cooking.")
				],
				FoodCatalogueFamily.DairyDrink =>
				[
					(15, $"$0 warm|warms $i1 gently, watching for the first skin to form.",
						$"$0 heat|heats the milk too fiercely and scorch|scorches the vessel."),
					(25, $"$0 stir|stirs the dairy stock until its body becomes smooth and drinkable.",
						$"$0 curdle|curdles the dairy stock and strain|strains away the lumps."),
					(20, $"$0 pour|pours $p1 into a clean vessel and let|lets it cool.",
						$"$0 smell|smells the failed drink and consign|consigns it to the sour-stock jar.")
				],
				FoodCatalogueFamily.Tea or FoodCatalogueFamily.Coffee or FoodCatalogueFamily.Chocolate =>
				[
					(15, $"$0 roast|roasts and crack|cracks $i1 until its fragrance rises.",
						$"$0 scorch|scorches the aromatic stock and scrape|scrapes away the blackened bits."),
					(25, $"$0 steep|steeps the {subject} stock in clean water, drawing out its colour.",
						$"$0 steep|steeps the drink too long and taste|tastes the harsh result."),
					(20, $"$0 strain|strains $p1 into a serving vessel and let|lets it cool.",
						$"$0 pour|pours off the weak drink and reserve|reserves it for cooking.")
				],
				FoodCatalogueFamily.Vinegar =>
				[
					(15, $"$0 warm|warms $i1 and prepare|prepares it for a slow souring.",
						$"$0 heat|heats the stock too hard and drive|drives off its useful sharpness."),
					(25, $"$0 leave|leaves the liquid open to the air so the souring can take hold.",
						$"$0 cover|covers the vessel too soon and stall|stalls the souring."),
					(20, $"$0 decant|decants $p1 into a stoppered jar to finish settling.",
						$"$0 taste|tastes the weak vinegar and save|saves it for a second batch.")
				],
				_ =>
				[
					(15, $"$0 crush|crushes $i1 and lay|lays out the {subject} stock.",
						$"$0 spill|spills the {subject} stock and recover|recovers what can be saved."),
					(25, $"$0 work|works the {subject} stock into a smooth liquid, following a household method.",
						$"$0 mishandle|mishandles the {subject} stock and rework|reworks it before it separates."),
					(20, $"$0 inspect|inspects $p1 and set|sets the finished drink aside to settle.",
						$"$0 inspect|inspects the poor drink and salvage|salvages the usable stock.")
				]
			};
		}

		if (spec.Kind == FoodCatalogueKind.Intermediate)
		{
			return
			[
				(15, $"$0 sort|sorts $i1, removing husks and damaged pieces from the {subject} stock.",
					$"$0 scatter|scatters the {subject} stock and recover|recovers the cleanest pieces."),
				(25, $"$0 process|processes the {subject} stock into a clean, useful household supply.",
					$"$0 spoil|spoils the {subject} stock and set|sets aside only the salvageable portion."),
				(20, $"$0 weigh|weighs $p1 and store|stores the finished {subject} stock for later work.",
					$"$0 inspect|inspects the poor stock and reserve|reserves it for a lesser use.")
			];
		}

		return spec.Family switch
		{
			FoodCatalogueFamily.Bread =>
			[
				(15, $"$0 sift|sifts $i1 and work|works the grain into a pliable bread dough.",
					$"$0 spill|spills the flour and gather|gathers enough to begin again."),
				(25, $"$0 knead|kneads the dough, shape|shapes it, and set|sets it near the heat.",
					$"$0 tear|tears the dough while shaping and have|has to fold it over again."),
				(20, $"$0 bake|bakes the loaf and set|sets $p1 aside to cool before serving.",
					$"$0 open|opens the oven to a collapsed loaf and salvage|salvages the crust.")
			],
			FoodCatalogueFamily.Grain or FoodCatalogueFamily.Porridge or FoodCatalogueFamily.Pulse =>
			[
				(15, $"$0 sort|sorts $i1 and rinse|rinses the grain before the pot is heated.",
					$"$0 drop|drops the grain and pick|picks through it for clean pieces."),
				(25, $"$0 simmer|simmers the grain stock until it softens and thickens.",
					$"$0 scorch|scorches the bottom and scrape|scrapes the usable portion free."),
				(20, $"$0 taste|tastes $p1, season|seasons it, and set|sets it aside to serve.",
					$"$0 taste|tastes the thin result and reserve|reserves it as animal feed.")
			],
			FoodCatalogueFamily.Noodle or FoodCatalogueFamily.Dumpling =>
			[
				(15, $"$0 mix|mixes $i1 with water and work|works it into a firm dough.",
					$"$0 splash|splashes too much water into the dough and add|adds more meal."),
				(25, $"$0 roll|rolls and cut|cuts the dough into the {subject} pieces.",
					$"$0 stick|sticks the pieces together and pull|pulls them apart to rescue the batch."),
				(20, $"$0 boil|boils $p1 until tender and drain|drains it for the table.",
					$"$0 overcook|overcooks the pieces and save|saves the broken bits for soup.")
			],
			FoodCatalogueFamily.Vegetable =>
			[
				(15, $"$0 trim|trims $i1 and cut|cuts away the tough ends.",
					$"$0 nick|nicks the good pieces and pare|pares away the damaged parts."),
				(25, $"$0 chop|chops the vegetables and cook|cooks them until their colour softens.",
					$"$0 char|chars the vegetables and scrape|scrapes away the bitter edges."),
				(20, $"$0 fold|folds $p1 into the serving dish and season|seasons it to taste.",
					$"$0 taste|tastes the failed vegetables and reserve|reserves them for broth.")
			],
			FoodCatalogueFamily.Soup or FoodCatalogueFamily.Stew =>
			[
				(15, $"$0 chop|chops $i1 and lay|lays the pieces beside the cooking pot.",
					$"$0 drop|drops the stock and pick|picks through it for usable pieces."),
				(25, $"$0 simmer|simmers the {subject} slowly, stirring so the bottom does not catch.",
					$"$0 let|lets the {subject} catch and thin|thins it with water to save the pot."),
				(20, $"$0 taste|tastes $p1, season|seasons it, and send|sends it to the table.",
					$"$0 taste|tastes the weak {subject} and reserve|reserves it for another pot.")
			],
			FoodCatalogueFamily.Meat or FoodCatalogueFamily.Poultry or FoodCatalogueFamily.Offal =>
			[
				(15, $"$0 trim|trims $i1 and portion|portions the meat for the hearth.",
					$"$0 nick|nicks the meat while trimming and cut|cuts away the damaged edge."),
				(25, $"$0 sear|sears the meat, then cook|cooks it through over a steady heat.",
					$"$0 scorch|scorches the outside and lower|lowers the heat to save the centre."),
				(20, $"$0 rest|rests $p1, then slice|slices it for the meal.",
					$"$0 pierce|pierces the tough result and reserve|reserves it for a stew.")
			],
			FoodCatalogueFamily.Fish or FoodCatalogueFamily.Shellfish =>
			[
				(15, $"$0 clean|cleans $i1 and remove|removes the scales, shells, and offal.",
					$"$0 crack|cracks the shell and salvage|salvages the sound pieces."),
				(25, $"$0 season|seasons the catch and cook|cooks it over the heat until firm.",
					$"$0 dry|dries the catch at the fire and turn|turns it before it burns."),
				(20, $"$0 inspect|inspects $p1 and serve|serves the finished catch while it is fresh.",
					$"$0 smell|smells the failed catch and set|sets it aside for stock.")
			],
			FoodCatalogueFamily.Dairy or FoodCatalogueFamily.Egg =>
			[
				(15, $"$0 break|breaks and combine|combines $i1 in the cooking bowl.",
					$"$0 spill|spills the dairy or egg stock and wipe|wipes the bowl clean."),
				(25, $"$0 warm|warms the mixture slowly until it thickens and holds together.",
					$"$0 curdle|curdles the mixture and strain|strains it into a fresh bowl."),
				(20, $"$0 set|sets $p1 aside to cool before serving.",
					$"$0 taste|tastes the poor result and reserve|reserves it for a sauce.")
			],
			FoodCatalogueFamily.Preserved =>
			[
				(15, $"$0 portion|portions $i1 and rub|rubs the pieces with preserving salt.",
					$"$0 spill|spills the salt and repack|repacks the pieces carefully."),
				(25, $"$0 dry|dries or smoke|smokes the provision until its surface tightens.",
					$"$0 dampen|dampens the provision and rehang|rehangs it where the air is clean."),
				(20, $"$0 wrap|wraps $p1 and store|stores it away from damp and vermin.",
					$"$0 inspect|inspects the poor ration and salvage|salvages the sound portion.")
			],
			FoodCatalogueFamily.Fruit or FoodCatalogueFamily.Nut or FoodCatalogueFamily.Sweet =>
			[
				(15, $"$0 crush|crushes $i1 and pick|picks out stones and skins.",
					$"$0 bruise|bruises the fruit and pare|pares away the spoiled pieces."),
				(25, $"$0 mix|mixes the {subject} stock into an even preparation.",
					$"$0 scorch|scorches the sweet stock and scrape|scrapes the pan clean."),
				(20, $"$0 shape|shapes $p1 and set|sets it aside to cool and firm.",
					$"$0 break|breaks the finished sweet and save|saves the crumbs for porridge.")
			],
			FoodCatalogueFamily.Condiment or FoodCatalogueFamily.Sauce or FoodCatalogueFamily.Syrup =>
			[
				(15, $"$0 grind|grinds $i1 and measure|measures the sharp stock into the bowl.",
					$"$0 spill|spills the ground stock and gather|gathers enough to continue."),
				(25, $"$0 stir|stirs the {subject} until its flavour and body come together.",
					$"$0 let|lets the {subject} catch and thin|thins it before it burns."),
				(20, $"$0 jar|jars $p1 and label|labels it for the next meal.",
					$"$0 taste|tastes the harsh {subject} and reserve|reserves it for a stronger batch.")
			],
			_ =>
			[
				(15, $"$0 gather|gathers $i1 and lay|lays out the {subject} stock, checking its quality.",
					$"$0 spill|spills the {subject} stock and recover|recovers what can be saved."),
				(25, $"$0 work|works the {subject} stock into the finished preparation, following a practiced household method.",
					$"$0 mishandle|mishandles the {subject} stock and rework|reworks it before it sets."),
				(20, $"$0 inspect|inspects $p1 and set|sets the completed preparation aside.",
					$"$0 inspect|inspects the poor result and salvage|salvages the usable stock.")
			]
		};
	}

	private static IReadOnlyList<PreIndustrialFoodCatalogueCraftSpec> PreIndustrialFoodCatalogueCraftSpecs()
	{
		var itemSpecs = PreIndustrialFoodCatalogue.Items
			.GroupBy(x => (x.Scope, x.Kind, x.Family, x.Material))
			.Select(CreateItemCraftSpec);
		var liquidSpecs = PreIndustrialFoodCatalogue.Liquids.Select(CreateLiquidCraftSpec);
		return itemSpecs.Concat(liquidSpecs).ToArray();
	}

	private static PreIndustrialFoodCatalogueCraftSpec CreateItemCraftSpec(
		IGrouping<(FoodCatalogueScope Scope, FoodCatalogueKind Kind, FoodCatalogueFamily Family, string Material),
			PreIndustrialFoodItemCatalogueEntry> group)
	{
		var entry = group.First();
		var output = new PreIndustrialFoodCatalogueOutput(
			string.Empty,
			group.Select(x => x.StableReference).OrderBy(x => x, StringComparer.Ordinal).ToArray());
		return new PreIndustrialFoodCatalogueCraftSpec(
			entry.Scope,
			entry.StableReference,
			entry.Kind,
			entry.Family,
			$"make {entry.Scope} {entry.Kind} {FamilyDisplay(entry.Family).ToLowerInvariant()} {entry.Material} catalogue stock",
			TraitFor(entry.Family, entry.Material, false),
			entry.Kind == FoodCatalogueKind.Intermediate ? 10 : 15,
			entry.Kind == FoodCatalogueKind.Intermediate ? Difficulty.Easy : Difficulty.Normal,
			FamilyDisplay(entry.Family),
			entry.Kind == FoodCatalogueKind.Intermediate ? 2 : 3,
			entry.Kind == FoodCatalogueKind.Intermediate ? "Catalogue Stock" : "Prepared Food",
			[FoodCatalogueInput(entry.Family, entry.Material)],
			output);
	}

	private static PreIndustrialFoodCatalogueCraftSpec CreateLiquidCraftSpec(PreIndustrialFoodLiquidCatalogueEntry entry)
	{
		var input = FoodCatalogueInput(entry.Family, null);
		var output = new PreIndustrialFoodCatalogueOutput(
			string.Empty,
			[$"liquid:{entry.StableReference}:{entry.Name}:10"],
			entry.Name);
		var alcoholic = entry.Family is FoodCatalogueFamily.FermentedDrink or FoodCatalogueFamily.Wine or FoodCatalogueFamily.Spirit;
		return new PreIndustrialFoodCatalogueCraftSpec(
			entry.Scope,
			entry.StableReference,
			FoodCatalogueKind.Prepared,
			entry.Family,
			$"fill catalogue liquid {entry.StableReference}",
			TraitFor(entry.Family, null, true),
			alcoholic ? 20 : 15,
			alcoholic ? Difficulty.Hard : Difficulty.Normal,
			FamilyDisplay(entry.Family),
			3,
			"Filled Catalogue Liquid",
			[input],
			output);
	}

	private static PreIndustrialFoodCatalogueInput FoodCatalogueInput(FoodCatalogueFamily family, string? material)
	{
		if (string.Equals(material, "honey", StringComparison.OrdinalIgnoreCase))
		{
			return CommodityTagInput(1.0, "Food", AgricultureFoodSource, PressedHoneyTag);
		}

		var familyInput = family switch
		{
			FoodCatalogueFamily.Meat or FoodCatalogueFamily.Poultry =>
				TagInput(RawNonFishMeatCutTag, AnimalButcheryFoodSource),
			FoodCatalogueFamily.Offal => TagInput("Offal", AnimalButcheryFoodSource),
			FoodCatalogueFamily.Fish or FoodCatalogueFamily.Shellfish =>
				TagInput(RawFishCutTag, AnimalButcheryFoodSource),
			FoodCatalogueFamily.Dairy or FoodCatalogueFamily.DairyDrink => CommodityTagInput(1.0, RawMilkTag, AgricultureFoodSource),
			FoodCatalogueFamily.Egg => CommodityTagInput(1.0, EggProductTag, AgricultureFoodSource),
			FoodCatalogueFamily.Fruit or FoodCatalogueFamily.FruitDrink or FoodCatalogueFamily.Wine =>
				CommodityTagInput(1.0, "Fruit", AgricultureFoodSource, SeededYieldTag),
			FoodCatalogueFamily.Oil => CommodityTagInput(1.0, "Oil Crop", AgricultureFoodSource, SeededYieldTag),
			_ => null
		};
		if (familyInput is not null)
		{
			return familyInput;
		}

		if (material is not null && AgricultureCropMaterials.Contains(material))
		{
			return CommodityTagInput(1.0, FoodCropMaterialTag, AgricultureFoodSource, SeededYieldTag);
		}

		if (string.Equals(material, "tree nut", StringComparison.OrdinalIgnoreCase) ||
			string.Equals(material, "chickpea", StringComparison.OrdinalIgnoreCase))
		{
			return CommodityTagInput(1.0, FoodCropMaterialTag, AgricultureFoodSource, SeededYieldTag);
		}

		if (material is not null && AgricultureVegetableMaterials.Contains(material))
		{
			return CommodityTagInput(1.0, "Vegetable", AgricultureFoodSource, SeededYieldTag);
		}

		if (string.Equals(material, "fruit", StringComparison.OrdinalIgnoreCase) ||
			string.Equals(material, "tree nut", StringComparison.OrdinalIgnoreCase))
		{
			return CommodityTagInput(1.0, "Fruit", AgricultureFoodSource, SeededYieldTag);
		}

		if (string.Equals(material, "meat", StringComparison.OrdinalIgnoreCase))
		{
			return TagInput(RawNonFishMeatCutTag, AnimalButcheryFoodSource);
		}

		if (string.Equals(material, "fish", StringComparison.OrdinalIgnoreCase))
		{
			return TagInput(RawFishCutTag, AnimalButcheryFoodSource);
		}

		if (string.Equals(material, "cheese", StringComparison.OrdinalIgnoreCase) ||
			string.Equals(material, "yoghurt", StringComparison.OrdinalIgnoreCase))
		{
			return CommodityTagInput(1.0, RawMilkTag, AgricultureFoodSource);
		}

		if (string.Equals(material, "egg", StringComparison.OrdinalIgnoreCase))
		{
			return CommodityTagInput(1.0, EggProductTag, AgricultureFoodSource);
		}

		return CommodityTagInput(1.0, FoodCropMaterialTag, AgricultureFoodSource, SeededYieldTag);
	}

	private static string FoodCatalogueTool(FoodCatalogueFamily family)
	{
		return family is FoodCatalogueFamily.GrainDrink or FoodCatalogueFamily.FermentedDrink or
			FoodCatalogueFamily.Wine or FoodCatalogueFamily.Spirit
			? "TagTool - InRoom - an item with the Brew Copper tag"
			: "TagTool - InRoom - an item with the Cooking tag";
	}

	private static PreIndustrialFoodCatalogueInput CommodityTagInput(
		double kilograms,
		string materialTag,
		string sourceOwner,
		string? pileTag = null)
	{
		return new PreIndustrialFoodCatalogueInput(
			$"CommodityTag - {kilograms:0.###} kilogram of a material tagged as {materialTag}; piletag {pileTag ?? materialTag}",
			null,
			sourceOwner,
			0);
	}

	private static PreIndustrialFoodCatalogueInput TagInput(string tag, string sourceOwner)
	{
		return new PreIndustrialFoodCatalogueInput(
			$"Tag - 1x an item with the {tag} tag",
			null,
			sourceOwner,
			0);
	}

	private static string TraitFor(FoodCatalogueFamily family, string? material, bool liquid)
	{
		if (liquid)
		{
			return family is FoodCatalogueFamily.FermentedDrink or FoodCatalogueFamily.Wine or FoodCatalogueFamily.Spirit
				? "Brewing"
				: "Cooking";
		}

		if (family is FoodCatalogueFamily.Meat or FoodCatalogueFamily.Poultry or FoodCatalogueFamily.Offal or
			FoodCatalogueFamily.Fish or FoodCatalogueFamily.Shellfish ||
			string.Equals(material, "meat", StringComparison.OrdinalIgnoreCase) ||
			string.Equals(material, "fish", StringComparison.OrdinalIgnoreCase))
		{
			return "Butchering";
		}

		if (family is FoodCatalogueFamily.Bread or FoodCatalogueFamily.Noodle or FoodCatalogueFamily.Dumpling)
		{
			return "Baking";
		}

		if (family is FoodCatalogueFamily.Grain or FoodCatalogueFamily.Porridge or FoodCatalogueFamily.Pulse ||
			(material is not null && AgricultureCropMaterials.Contains(material)))
		{
			return "Milling";
		}

		return "Cooking";
	}
}
