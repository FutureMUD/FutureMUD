using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CraftPhase = MudSharp.Models.CraftPhase;

#nullable enable

namespace DatabaseSeeder.Seeders;

public class CookingSeeder : IDatabaseSeeder
{
	private const string StockPrefix = "CookingSeeder";

	public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		Enumerable.Empty<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>();

	public int SortOrder => 390;
	public string Name => "Prepared Food and Cooking";
	public string Tagline => "Prepared-food prototypes, direct food items and sample cooking recipes";
	public bool SafeToRunMoreThanOnce => true;

	public string FullDescription => @"This package installs stock prepared-food component prototypes, directly loadable prepared food items, stackable serving examples, and sample cooking crafts that output CookedFoodProduct records.

It intentionally leaves the legacy Food component untouched. The direct items can be loaded, sold, foraged, or created by progs and spells without passing through any craft.";

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!PrerequisitesMet(context))
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		return MissingStockRecords(context).Any()
			? ShouldSeedResult.ExtraPackagesAvailable
			: ShouldSeedResult.MayAlreadyBeInstalled;
	}

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (!PrerequisitesMet(context))
		{
			return "Prepared food and cooking prerequisites are not installed.";
		}

		var created = new List<string>();
		var now = DateTime.UtcNow;
		var account = context.Accounts.First();
		var tags = context.Tags.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		var components = context.GameItemComponentProtos.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		var materials = context.Materials.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

		var foodTag = EnsureTag(context, tags, "Food", null);
		var preparedFoodTag = EnsureTag(context, tags, "Prepared Food", foodTag);
		var forageFoodTag = EnsureTag(context, tags, "Forageable Food", preparedFoodTag);
		var cookingIngredientTag = EnsureTag(context, tags, "Cooking Ingredient", preparedFoodTag);
		var stockCookingAppleTag = EnsureTag(context, tags, "Stock Cooking Apple", cookingIngredientTag);
		var stockCookingFruitTag = EnsureTag(context, tags, "Stock Cooking Fruit", cookingIngredientTag);

		var appleComponent = EnsurePreparedFoodComponent(context, components, account, now, "PreparedFood_Apple",
			"Stock prepared-food profile for a directly loaded apple.",
			FoodDefinition("WholeItem", 2.0, 0.08, 1.0, 0.0, 4.0, 0.08, 0.25, 0.0, 0.01, 14, 28,
				"It tastes crisp, sweet, and faintly tart.",
				"{freshness} {primary}",
				"A {quality} apple. It is made of {ingredients}.",
				("primary", "apple", "fresh apple")));
		if (appleComponent.WasCreated)
		{
			created.Add("apple prepared-food component");
		}

		var berryComponent = EnsurePreparedFoodComponent(context, components, account, now, "PreparedFood_Berry_Stack",
			"Stock prepared-food profile for stackable berries.",
			FoodDefinition("PerStackUnit", 0.35, 0.015, 0.2, 0.0, 1.0, 0.08, 0.25, 0.0, 0.003, 5, 9,
				"It tastes like a small burst of sweet berry juice.",
				"{servings} {freshness} berries",
				"These berries are a stackable prepared food. Each unit is its own serving. They are made of {ingredients}.",
				("primary", "blueberry", "sweet berry")));
		if (berryComponent.WasCreated)
		{
			created.Add("berry stack prepared-food component");
		}

		var mushroomComponent = EnsurePreparedFoodComponent(context, components, account, now, "PreparedFood_Mushroom_Stack",
			"Stock prepared-food profile for stackable forage mushrooms.",
			FoodDefinition("PerStackUnit", 0.6, 0.01, 0.0, 0.0, 2.0, 0.08, 0.15, 0.0, 0.004, 3, 6,
				"It tastes earthy and damp.",
				"{servings} {freshness} mushrooms",
				"These mushrooms are food items suitable for foraging examples. They are made of {ingredients}.",
				("primary", "mushroom", "earthy mushroom")));
		if (mushroomComponent.WasCreated)
		{
			created.Add("mushroom stack prepared-food component");
		}

		var muffinComponent = EnsurePreparedFoodComponent(context, components, account, now, "PreparedFood_Muffin_Template",
			"Stock prepared-food profile for recipe-created muffins.",
			FoodDefinition("WholeItem", 4.0, 0.03, 0.0, 0.0, 5.0, 0.1, 0.3, 0.0, 0.02, 3, 7,
				"It tastes of baked grain, soft crumb, and {additives}.",
				"a {quality} muffin with {additives}",
				"This muffin was created with the prepared-food system. Its ingredient ledger records {ingredients}.",
				("base", "muffin batter", "baked muffin")));
		if (muffinComponent.WasCreated)
		{
			created.Add("muffin prepared-food component");
		}

		var bakedAppleComponent = EnsurePreparedFoodComponent(context, components, account, now, "PreparedFood_Baked_Apple",
			"Stock prepared-food profile for a cooked apple recipe product.",
			FoodDefinition("WholeItem", 2.3, 0.06, 0.5, 0.0, 4.0, 0.1, 0.3, 0.0, 0.015, 4, 8,
				"It tastes soft, sweet, and warmly cooked, with traces of {ingredients}.",
				"a baked apple with a soft split skin",
				"This baked apple is a recipe-initialised prepared food. Its ledger records {ingredients}.",
				("base", "baked apple", "warm baked apple")));
		if (bakedAppleComponent.WasCreated)
		{
			created.Add("baked apple prepared-food component");
		}

		var apple = EnsureItem(context, account, now, materials["apple"], "apple", "a crisp red apple",
			"A crisp red apple lies here.",
			"The apple is round and firm-skinned, with a red blush over pale green undertones.",
			120, 2.0M,
			[preparedFoodTag, forageFoodTag, cookingIngredientTag, stockCookingAppleTag, stockCookingFruitTag],
			[components["Holdable"], appleComponent.Component]);
		if (apple.WasCreated)
		{
			created.Add("direct apple item");
		}

		var berries = EnsureItem(context, account, now, materials["blueberry"], "berries", "a handful of blueberries",
			"A handful of blueberries have been scattered here.",
			"The blueberries are small, dark, and slightly dusty-skinned.",
			50, 1.5M,
			[preparedFoodTag, forageFoodTag, cookingIngredientTag, stockCookingFruitTag],
			[components["Holdable"], components["Stack_Number"], berryComponent.Component]);
		if (berries.WasCreated)
		{
			created.Add("stackable berry item");
		}

		var mushrooms = EnsureItem(context, account, now, materials["mushroom"], "mushrooms", "a handful of edible mushrooms",
			"A handful of edible mushrooms have been left here.",
			"The mushrooms are pale brown and irregularly shaped, with slightly damp gills.",
			80, 1.0M,
			[preparedFoodTag, forageFoodTag, cookingIngredientTag],
			[components["Holdable"], components["Stack_Number"], mushroomComponent.Component]);
		if (mushrooms.WasCreated)
		{
			created.Add("stackable mushroom item");
		}

		var muffin = EnsureItem(context, account, now, materials["muffin"], "muffin", "a plain muffin",
			"A plain muffin rests here.",
			"The muffin has a rounded top and a dense, golden crumb.",
			110, 3.0M,
			[preparedFoodTag, cookingIngredientTag],
			[components["Holdable"], muffinComponent.Component]);
		if (muffin.WasCreated)
		{
			created.Add("muffin recipe target item");
		}

		var bakedApple = EnsureItem(context, account, now, materials["apple"], "baked apple", "a baked apple with a soft split skin",
			"A baked apple with a soft split skin rests here.",
			"The apple has collapsed slightly from cooking, its split skin revealing soft, fragrant flesh.",
			105, 3.0M,
			[preparedFoodTag],
			[components["Holdable"], bakedAppleComponent.Component]);
		if (bakedApple.WasCreated)
		{
			created.Add("baked apple recipe target item");
		}

		var craft = EnsureBakedAppleCraft(context, account, now, stockCookingAppleTag, bakedApple.Item);
		if (craft)
		{
			created.Add("sample CookedFoodProduct recipe");
		}

		context.SaveChanges();
		return created.Any()
			? $"Installed {created.ListToString()}."
			: "Prepared food and cooking stock records were already installed.";
	}

	private static bool PrerequisitesMet(FuturemudDatabaseContext context)
	{
		return context.Accounts.Any() &&
		       context.GameItemComponentProtos.Any(x => x.Name == "Holdable") &&
		       context.GameItemComponentProtos.Any(x => x.Name == "Stack_Number") &&
		       new[] { "apple", "blueberry", "mushroom", "muffin" }.All(material => context.Materials.Any(x => x.Name == material)) &&
		       context.TraitDefinitions.Any();
	}

	private static IEnumerable<string> MissingStockRecords(FuturemudDatabaseContext context)
	{
		foreach (var name in new[]
		         {
			         "PreparedFood_Apple", "PreparedFood_Berry_Stack", "PreparedFood_Mushroom_Stack",
			         "PreparedFood_Muffin_Template", "PreparedFood_Baked_Apple"
		         })
		{
			if (!context.GameItemComponentProtos.Any(x => x.Name == name && x.Type == "PreparedFood"))
			{
				yield return name;
			}
		}

		foreach (var sdesc in new[]
		         {
			         "a crisp red apple", "a handful of blueberries", "a handful of edible mushrooms",
			         "a plain muffin", "a baked apple with a soft split skin"
		         })
		{
			if (!context.GameItemProtos.Any(x => x.ShortDescription == sdesc))
			{
				yield return sdesc;
			}
		}

		if (!context.Crafts.Any(x => x.Name == "bake apple"))
		{
			yield return "bake apple";
		}
	}

	private static Tag EnsureTag(FuturemudDatabaseContext context, IDictionary<string, Tag> tags, string name, Tag? parent)
	{
		if (tags.TryGetValue(name, out var tag))
		{
			return tag;
		}

		tag = new Tag
		{
			Id = tags.Values.Any() ? tags.Values.Max(x => x.Id) + 1 : 1,
			Name = name,
			Parent = parent,
			ParentId = parent?.Id
		};
		context.Tags.Add(tag);
		tags[name] = tag;
		return tag;
	}

	private static (GameItemComponentProto Component, bool WasCreated) EnsurePreparedFoodComponent(
		FuturemudDatabaseContext context,
		IDictionary<string, GameItemComponentProto> components,
		Account account,
		DateTime now,
		string name,
		string description,
		string definition)
	{
		if (components.TryGetValue(name, out var component))
		{
			return (component, false);
		}

		component = new GameItemComponentProto
		{
			Id = components.Values.Any() ? components.Values.Max(x => x.Id) + 1 : 1,
			Name = name,
			Description = description,
			Type = "PreparedFood",
			RevisionNumber = 0,
			Definition = definition,
			EditableItem = Editable(account, now)
		};
		context.GameItemComponentProtos.Add(component);
		components[name] = component;
		return (component, true);
	}

	private static string FoodDefinition(string scope, double satiation, double water, double thirst, double alcohol,
		double bites, double qualityScale, double staleMultiplier, double spoiledMultiplier, double absorption,
		int staleDays, int spoilDays, string taste, string shortTemplate, string fullTemplate,
		(string Role, string Description, string Taste) ingredient)
	{
		return new XElement("Definition",
			new XAttribute("ServingScope", scope),
			new XAttribute("Satiation", satiation),
			new XAttribute("Water", water),
			new XAttribute("Thirst", thirst),
			new XAttribute("Alcohol", alcohol),
			new XAttribute("Bites", bites),
			new XAttribute("QualityScale", qualityScale),
			new XAttribute("StaleMultiplier", staleMultiplier),
			new XAttribute("SpoiledMultiplier", spoiledMultiplier),
			new XAttribute("LiquidAbsorption", absorption),
			new XAttribute("StaleAfterSeconds", TimeSpan.FromDays(staleDays).TotalSeconds),
			new XAttribute("SpoilAfterSeconds", TimeSpan.FromDays(spoilDays).TotalSeconds),
			new XAttribute("Decorator", 0),
			new XElement("Taste", new XCData(taste)),
			new XElement("Short", new XCData(shortTemplate)),
			new XElement("Full", new XCData(fullTemplate)),
			new XElement("OnEatProg", 0),
			new XElement("OnStaleProg", 0),
			new XElement("Ingredients",
				new XElement("Ingredient",
					new XAttribute("role", ingredient.Role),
					new XAttribute("source", 0),
					new XAttribute("material", 0),
					new XAttribute("liquid", 0),
					new XAttribute("weight", 0),
					new XAttribute("volume", 0),
					new XAttribute("quality", (int)ItemQuality.Standard),
					new XElement("Description", new XCData(ingredient.Description)),
					new XElement("Taste", new XCData(ingredient.Taste))
				)
			),
			new XElement("DrugDoses"),
			new XElement("StaleDrugDoses")
		).ToString();
	}

	private static (GameItemProto Item, bool WasCreated) EnsureItem(
		FuturemudDatabaseContext context,
		Account account,
		DateTime now,
		Material material,
		string noun,
		string sdesc,
		string ldesc,
		string fdesc,
		double weight,
		decimal cost,
		IEnumerable<Tag> tags,
		IEnumerable<GameItemComponentProto> components)
	{
		var existing = context.GameItemProtos.FirstOrDefault(x => x.ShortDescription == sdesc);
		if (existing is not null)
		{
			return (existing, false);
		}

		var item = new GameItemProto
		{
			Id = NextGameItemProtoId(context),
			Name = noun,
			Keywords = sdesc.Strip_A_An().Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct().ListToCommaSeparatedValues(" "),
			MaterialId = material.Id,
			EditableItem = Editable(account, now),
			RevisionNumber = 0,
			Size = (int)SizeCategory.Small,
			Weight = weight,
			ReadOnly = false,
			LongDescription = ldesc,
			BaseItemQuality = (int)ItemQuality.Standard,
			MorphEmote = "$0 $?1|morphs into $1|decays into nothing$.",
			ShortDescription = sdesc,
			FullDescription = fdesc,
			PermitPlayerSkins = true,
			CostInBaseCurrency = cost,
			IsHiddenFromPlayers = false
		};
		foreach (var tag in tags.DistinctBy(x => x.Id))
		{
			item.GameItemProtosTags.Add(new GameItemProtosTags
			{
				GameItemProto = item,
				Tag = tag,
				TagId = tag.Id,
				GameItemProtoRevisionNumber = 0
			});
		}

		foreach (var component in components.DistinctBy(x => x.Id))
		{
			item.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{
				GameItemProto = item,
				GameItemComponent = component,
				GameItemComponentProtoId = component.Id,
				GameItemComponentRevision = component.RevisionNumber,
				GameItemProtoRevision = 0
			});
		}

		context.GameItemProtos.Add(item);
		return (item, true);
	}

	private static bool EnsureBakedAppleCraft(FuturemudDatabaseContext context, Account account, DateTime now, Tag appleTag,
		GameItemProto bakedApple)
	{
		if (context.Crafts.Any(x => x.Name == "bake apple"))
		{
			return false;
		}

		var appearProg = EnsureAppearProg(context, account, now);
		if (appearProg.Id == 0)
		{
			context.SaveChanges();
		}
		var trait = context.TraitDefinitions
		                   .FirstOrDefault(x => x.Name == "Cooking" || x.Name == "Cook") ??
		            context.TraitDefinitions.First();
		var craft = new Craft
		{
			Id = NextCraftId(context),
			RevisionNumber = 0,
			EditableItem = Editable(account, now),
			Name = "bake apple",
			Category = "Cooking",
			Blurb = "bake an apple into a simple prepared-food example",
			ActionDescription = "baking an apple",
			ActiveCraftItemSdesc = "an apple-baking process",
			AppearInCraftsListProg = appearProg,
			AppearInCraftsListProgId = appearProg.Id,
			CheckTrait = trait,
			CheckTraitId = trait.Id,
			CheckDifficulty = (int)Difficulty.Trivial,
			FailThreshold = (int)Outcome.MajorFail,
			FreeSkillChecks = 1,
			FailPhase = 2,
			Interruptable = true,
			QualityFormula = "5 + (outcome/3) + (variable/20)",
			CheckQualityWeighting = 1.0,
			InputQualityWeighting = 1.0,
			ToolQualityWeighting = 1.0,
			IsPracticalCheck = true
		};
		craft.CraftPhases.Add(new CraftPhase
		{
			Craft = craft,
			PhaseNumber = 1,
			PhaseLengthInSeconds = 30,
			Echo = "$0 wash|washes and score|scores $i1, preparing it for heat.",
			FailEcho = "$0 mishandle|mishandles $i1 while preparing it."
		});
		craft.CraftPhases.Add(new CraftPhase
		{
			Craft = craft,
			PhaseNumber = 2,
			PhaseLengthInSeconds = 60,
			Echo = "$0 cook|cooks $i1 until the skin splits and the flesh softens.",
			FailEcho = "$0 overcook|overcooks $i1, ruining the example."
		});
		craft.CraftInputs.Add(new CraftInput
		{
			Craft = craft,
			InputQualityWeight = 1.0,
			OriginalAdditionTime = now,
			InputType = "Tag",
			Definition = new XElement("Definition",
				new XElement("TargetTagId", appleTag.Id),
				new XElement("Quantity", 1)
			).ToString()
		});
		craft.CraftProducts.Add(new CraftProduct
		{
			Craft = craft,
			IsFailProduct = false,
			OriginalAdditionTime = now,
			ProductType = "CookedFoodProduct",
			Definition = new XElement("Definition",
				new XElement("ProductProducedId", bakedApple.Id),
				new XElement("Quantity", 1),
				new XElement("Skin", 0),
				new XElement("RemoveDrugsAndFoodEffects", false),
				new XElement("IngredientSlots")
			).ToString()
		});
		context.Crafts.Add(craft);
		return true;
	}

	private static FutureProg EnsureAppearProg(FuturemudDatabaseContext context, Account account, DateTime now)
	{
		var existing = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "AlwaysTrue") ??
		               context.FutureProgs.FirstOrDefault(x => x.FunctionName == $"{StockPrefix}AlwaysTrue");
		if (existing is not null)
		{
			return existing;
		}

		var prog = new FutureProg
		{
			FunctionName = $"{StockPrefix}AlwaysTrue",
			Category = "Crafting",
			Subcategory = "Cooking",
			ReturnType = (long)ProgVariableTypes.Boolean,
			FunctionComment = "Used by the cooking seeder to make stock cooking examples visible.",
			FunctionText = "return true",
			StaticType = (int)FutureProgStaticType.NotStatic,
			AcceptsAnyParameters = false
		};
		context.FutureProgs.Add(prog);
		return prog;
	}

	private static EditableItem Editable(Account account, DateTime now)
	{
		return new EditableItem
		{
			RevisionNumber = 0,
			RevisionStatus = 4,
			BuilderAccountId = account.Id,
			BuilderDate = now,
			BuilderComment = "Auto-generated by the system",
			ReviewerAccountId = account.Id,
			ReviewerComment = "Auto-generated by the system",
			ReviewerDate = now
		};
	}

	private static long NextGameItemProtoId(FuturemudDatabaseContext context)
	{
		var existing = context.GameItemProtos.Any() ? context.GameItemProtos.Max(x => x.Id) : 0;
		var local = context.GameItemProtos.Local.Any() ? context.GameItemProtos.Local.Max(x => x.Id) : 0;
		return Math.Max(existing, local) + 1;
	}

	private static long NextCraftId(FuturemudDatabaseContext context)
	{
		var existing = context.Crafts.Any() ? context.Crafts.Max(x => x.Id) : 0;
		var local = context.Crafts.Local.Any() ? context.Crafts.Local.Max(x => x.Id) : 0;
		return Math.Max(existing, local) + 1;
	}
}
