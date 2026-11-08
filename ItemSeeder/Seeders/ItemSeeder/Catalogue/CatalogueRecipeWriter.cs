#nullable enable

using MudSharp.GameItems.Inventory.Plans;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedCompiledCatalogueCrafts(string eras)
	{
		var selected = ParseEraTokens(eras).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var recipes = CompiledRecipeCatalogue.Select(selected, _manifestCaptureOnly)
			.Where(recipe => _industrialisedCatalogueOverride is null ||
				!(recipe.RequiredAnyEra.Contains("industrial") || recipe.IndustrialCaptureOnly))
			.ToArray();
		var supportProgs = new HashSet<FutureProg>();
		foreach (var recipe in recipes)
		{
			foreach (var name in recipe.Products.Concat(recipe.FailProducts).Select(product => product switch
			{
				CatalogueProgRecipeProduct value => value.Prog,
				CatalogueProgCookedRecipeProduct value => value.Prog,
				_ => null
			}).Where(name => name is not null).Distinct(StringComparer.Ordinal))
			{
				if (IsManifestAggregateRegistered("prog", name!) || RecipeSupportStock.Find(name!) is not { } support)
				{
					if (_progs.TryGetValue(name!, out var existing)) supportProgs.Add(existing);
					continue;
				}
				using var supportModule = UseManifestModule(support.Metadata.Module, support.Metadata.EraAdmissions.ToArray());
				supportProgs.Add(EnsureFutureProg(support.Name, support.Category, support.Subcategory, support.ReturnType,
					support.Comment, support.Parameters.Select(parameter => (parameter.Type, parameter.Name)),
					support.Text.Replace("\r\n", Environment.NewLine, StringComparison.Ordinal)));
			}
		}
		// Product XML contains numeric program references, not EF navigations.
		// Flush the support batch once before any product captures those identities.
		SaveFutureProgsIfRequired(supportProgs.ToArray());
		foreach (var recipe in recipes)
		{
			using var module = UseManifestModule(recipe.Metadata.Module, recipe.Metadata.EraAdmissions.ToArray());
			InstallCatalogueRecipe(recipe);
		}
		if (_industrialisedCatalogueOverride is not null && selected.Contains("industrial"))
			SeedIndustrialisedCatalogueCrafts(eras);
	}

	private long RecipeProductProgId(string name)
	{
		var id = LookupProg(name).Id;
		if (!_manifestCaptureOnly && id <= 0)
			throw new InvalidOperationException($"Recipe selector program '{name}' must have a permanent database identity before product XML is created.");
		return id;
	}

	private XElement RecipeCharacteristics(CatalogueCharacteristicMode mode,
		IReadOnlyList<CatalogueCharacteristicRequirement> requirements) =>
		new("Characteristics", new XAttribute("mode", mode.ToString().ToLowerInvariant()),
			mode == CatalogueCharacteristicMode.Specific
				? requirements.Select(value => new XElement("Characteristic",
					new XAttribute("definition", LookupCharacteristicDefinition(value.Definition).Id),
					new XAttribute("value", value.Value is null ? 0 :
						LookupCharacteristicValue(LookupCharacteristicDefinition(value.Definition), value.Value).Id)))
				: []);

	private (string TypeName, string Definition) BuildRecipeInput(CatalogueRecipeInput input)
	{
		XElement Mass(string materialElement, long material, double grams, string? tag,
			CatalogueCharacteristicMode mode, IReadOnlyList<CatalogueCharacteristicRequirement> characteristics) =>
			new("Definition", new XElement(materialElement, material), new XElement("Weight", grams),
				new XElement("CommodityPileTag", tag is null ? 0 : LookupTag(tag).Id), RecipeCharacteristics(mode, characteristics));
		var (type, xml) = input switch
		{
			CatalogueCommodityRecipeInput value => ("Commodity", Mass("Material", LookupMaterial(value.Material).Id,
				value.Grams, value.PileTag, value.CharacteristicMode, value.Characteristics)),
			CatalogueCommodityTagRecipeInput value => ("CommodityTag", Mass("MaterialTag", LookupTag(value.MaterialTag).Id,
				value.Grams, value.PileTag, value.CharacteristicMode, value.Characteristics)),
			CatalogueTagRecipeInput value => ("Tag", new XElement("Definition",
				new XElement("TargetTagId", LookupTag(value.Tag).Id), new XElement("Quantity", value.Quantity))),
			CatalogueItemRecipeInput value => ("SimpleItem", new XElement("Definition",
				new XElement("TargetItemId", LookupReworkItem(value.ItemStableReference).Id), new XElement("Quantity", value.Quantity))),
			CatalogueLiquidRecipeInput value => ("LiquidUse", new XElement("Definition",
				new XElement("Liquid", LookupLiquid(value.Liquid).Id), new XElement("Amount", value.Litres))),
			CatalogueLiquidTagRecipeInput value => ("LiquidTagUse", new XElement("Definition",
				new XElement("Tag", LookupTag(value.LiquidTag).Id), new XElement("Amount", value.Litres))),
			_ => throw new InvalidOperationException($"Unsupported catalogue input {input.GetType().Name}.")
		};
		return (type, xml.ToString());
	}

	private (string TypeName, DesiredItemState State, string Definition) BuildRecipeTool(CatalogueRecipeTool tool) => tool switch
	{
		CatalogueTagRecipeTool value => ("TagTool", value.State,
			new XElement("Definition", new XElement("TargetItemTag", LookupTag(value.Tag).Id)).ToString()),
		CatalogueItemRecipeTool value => ("SimpleTool", value.State,
			new XElement("Definition", new XElement("TargetItemId", LookupReworkItem(value.ItemStableReference).Id)).ToString()),
		_ => throw new InvalidOperationException($"Unsupported catalogue tool {tool.GetType().Name}.")
	};

	private (string TypeName, string Definition) BuildRecipeProduct(Craft craft, CatalogueRecipeProduct product)
	{
		XElement Item(string key, int quantity, string? skin)
		{
			var item = LookupReworkItem(key);
			return new("Definition", new XElement("ProductProducedId", item.Id), new XElement("Quantity", quantity),
				new XElement("Skin", skin is null ? 0 : LookupSkin(item, "@" + skin)!.Id));
		}
		XElement Slots(IReadOnlyList<CatalogueIngredientSlot> ingredients) => new("IngredientSlots",
			ingredients.Select(slot => new XElement("Slot", new XAttribute("input", GetInputByOneBasedIndex(craft, slot.InputIndex).Id),
				new XAttribute("role", slot.Role))));
		XElement xml;
		string type;
		switch (product)
		{
			case CatalogueItemRecipeProduct value:
				type = "SimpleProduct"; xml = Item(value.ItemStableReference, value.Quantity, value.SkinStableReference); break;
			case CatalogueCookedRecipeProduct value:
				type = "CookedFoodProduct"; xml = Item(value.ItemStableReference, value.Quantity, value.SkinStableReference);
				xml.Add(new XElement("RemoveDrugsAndFoodEffects", value.RemoveDrugsAndFoodEffects), Slots(value.Ingredients)); break;
			case CatalogueProgCookedRecipeProduct value:
				type = "ProgCookedFoodProduct";
				xml = new("Definition", new XElement("ItemProg", RecipeProductProgId(value.Prog)), new XElement("Quantity", 1),
					new XElement("Skin", 0), new XElement("RemoveDrugsAndFoodEffects", value.RemoveDrugsAndFoodEffects), Slots(value.Ingredients)); break;
			case CatalogueLiquidRecipeProduct value:
				type = "LiquidProduct";
				var item = LookupReworkItem(value.ItemStableReference);
				xml = new("Definition", new XElement("ProductProducedId", item.Id), new XElement("Quantity", value.Quantity),
					new XElement("Liquid", LookupLiquid(value.Liquid).Id), new XElement("LiquidVolume", value.Litres),
					new XElement("Skin", value.SkinStableReference is null ? 0 : LookupSkin(item, "@" + value.SkinStableReference)!.Id)); break;
			case CatalogueVariableRecipeProduct value:
				type = "SimpleVariableProduct"; xml = Item(value.ItemStableReference, value.Quantity, value.SkinStableReference);
				var variables = value.Variables.Select(variable =>
					(Definition: LookupCharacteristicDefinition(variable.Definition).Id, variable.InputIndex)).ToArray();
				if (variables.Any(variable => variable.InputIndex <= 0) ||
					variables.Select(variable => variable.Definition).Distinct().Count() != variables.Length)
					throw new ApplicationException("Each variable-product characteristic must have one positive source input.");
				xml.Add(variables.Select(variable => new XElement("Variable",
					new XAttribute("inputindex", variable.InputIndex - 1), variable.Definition))); break;
			case CatalogueCommodityRecipeProduct value:
				type = "CommodityProduct";
				xml = new("Definition", new XElement("Material", LookupMaterial(value.Material).Id), new XElement("Weight", value.Grams),
					new XElement("Tag", value.Tag is null ? 0 : LookupTag(value.Tag).Id),
					new XElement("Characteristics", value.Characteristics.Select(characteristic => new XElement("Characteristic",
						new XAttribute("definition", LookupCharacteristicDefinition(characteristic.Definition).Id),
						new XAttribute("value", characteristic.Value is null ? 0 : LookupCharacteristicValue(
							LookupCharacteristicDefinition(characteristic.Definition), characteristic.Value).Id),
						new XAttribute("input", characteristic.InputIndex is { } index ? index - 1 : -1))))); break;
			case CatalogueProgRecipeProduct value:
				type = "Prog"; xml = new("Definition", new XElement("ItemProg", RecipeProductProgId(value.Prog))); break;
			case CatalogueScrapRecipeProduct value:
				type = "ScrapInput"; xml = new("Definition", new XElement("WhichInputId", GetInputByOneBasedIndex(craft, value.InputIndex).Id),
					new XElement("PercentageRecovered", value.FractionRecovered), new XElement("Tag", value.Tag is null ? 0 : LookupTag(value.Tag).Id)); break;
			case CatalogueUnusedRecipeProduct value:
				type = "UnusedInput"; xml = new("Definition", new XElement("WhichInputId", GetInputByOneBasedIndex(craft, value.InputIndex).Id),
					new XElement("PercentageRecovered", value.FractionRecovered)); break;
			default: throw new InvalidOperationException($"Unsupported catalogue product {product.GetType().Name}.");
		}
		return (type, xml.ToString());
	}

	private string ResolveRecipeText(string text) => Regex.Replace(text, @"\(@(?<key>[^)]+)\)",
		match => $"(#{LookupReworkItem(match.Groups["key"].Value).Id.ToString(CultureInfo.InvariantCulture)})");

	private (string Text, string Details, string[] Options) RecipeProjection(string type, CatalogueImportProjection? compatibility)
	{
		if (compatibility is null) throw new InvalidOperationException("An ownership compatibility projection is required for this recipe contract.");
		var details = ResolveRecipeText(compatibility.Details);
		var options = compatibility.Options.Select(ResolveRecipeText).ToArray();
		return (type + " - " + details + string.Concat(options.Select(option => "; " + option)), details, options);
	}

	private TraitDefinition? ResolveCatalogueRecipeCheckTrait(CatalogueRecipeDefinition recipe)
	{
		foreach (var name in recipe.CheckTraitChoices)
			if (LookupLoadedTraitDefinition(name) is { } trait) return trait;
		return null;
	}

	private Craft InstallCatalogueRecipe(CatalogueRecipeDefinition recipe)
	{
		CatalogueValidator.Validate(recipe);
		if (recipe.QualityPolicyVersion != 1) throw new InvalidOperationException($"Unknown craft quality policy {recipe.QualityPolicyVersion}.");
		FutureProg? appear, canUse, whyCannotUse;
		TraitDefinition trait;
		var configuredTrait = ResolveCatalogueRecipeCheckTrait(recipe);
		switch (recipe.Gate)
		{
			case CatalogueKnowledgeRecipeGate gate:
				(appear, canUse, whyCannotUse, trait, _) = EnsureKnowledgeGateProgs(gate.Knowledge, configuredTrait?.Name ?? gate.Trait, gate.MinimumTraitValue,
					gate.Type, gate.Subtype, gate.Description, gate.LongDescription, gate.LearningSessionsRequired,
					gate.LearnDifficulty, gate.TeachDifficulty, gate.Learnable); break;
			case CatalogueTraitRecipeGate gate:
				(appear, canUse, whyCannotUse, trait) = EnsureTraitGateProgs(configuredTrait?.Name ?? gate.Trait, gate.MinimumTraitValue); break;
			default:
				appear = recipe.AppearProg is null ? null : LookupProg(recipe.AppearProg);
				canUse = recipe.CanUseProg is null ? null : LookupProg(recipe.CanUseProg);
				whyCannotUse = recipe.WhyCannotUseProg is null ? null : LookupProg(recipe.WhyCannotUseProg);
				trait = configuredTrait ?? LookupTraitDefinition(recipe.Trait!); break;
		}
		trait = configuredTrait ?? trait;
		CraftInputSpec Input(CatalogueRecipeInput input)
		{
			var type = input switch { CatalogueCommodityRecipeInput => "Commodity", CatalogueCommodityTagRecipeInput => "CommodityTag",
				CatalogueTagRecipeInput => "Tag", CatalogueItemRecipeInput => "SimpleItem", CatalogueLiquidRecipeInput => "LiquidUse",
				CatalogueLiquidTagRecipeInput => "LiquidTagUse", _ => throw new InvalidOperationException() };
			var projection = RecipeProjection(type, input.Compatibility);
			return new(projection.Text, type, projection.Details, projection.Options, input.QualityWeight) { RecipeInput = input };
		}
		CraftToolSpec Tool(CatalogueRecipeTool tool)
		{
			var type = tool is CatalogueTagRecipeTool ? "TagTool" : "SimpleTool";
			var projection = RecipeProjection(type, tool.Compatibility);
			return new(projection.Text, type, projection.Details, projection.Options, tool.QualityWeight, tool.UseToolDuration) { RecipeTool = tool };
		}
		CraftProductSpec Product(CatalogueRecipeProduct product)
		{
			var type = product switch { CatalogueItemRecipeProduct => "SimpleProduct", CatalogueCookedRecipeProduct => "CookedFoodProduct",
				CatalogueProgCookedRecipeProduct => "ProgCookedFoodProduct", CatalogueLiquidRecipeProduct => "LiquidProduct",
				CatalogueVariableRecipeProduct => "SimpleVariableProduct", CatalogueCommodityRecipeProduct => "CommodityProduct",
				CatalogueProgRecipeProduct => "ProgProduct", CatalogueScrapRecipeProduct => "ScrapInput", CatalogueUnusedRecipeProduct => "UnusedInput",
				_ => throw new InvalidOperationException() };
			var projection = RecipeProjection(type, product.Compatibility);
			return new(projection.Text, type, projection.Details, projection.Options, product.IsFailProduct,
				product.MaterialDefiningInputIndex) { RecipeProduct = product };
		}
		return AddCraft(new CraftDefinitionSpec
		{
			StableReference = recipe.UseExplicitStableReference ? recipe.Metadata.StableKey : null,
			PreserveAuthoredText = recipe.PreserveAuthoredText, Name = recipe.Name, Category = recipe.Category,
			Blurb = recipe.Blurb, Action = recipe.Action, ActiveCraftItemSdesc = recipe.ActiveCraftItemSdesc,
			AppearProg = appear, CanUseProg = canUse, WhyCannotUseProg = whyCannotUse, Trait = trait,
			OnStartProg = recipe.OnStartProg is null ? null : LookupProg(recipe.OnStartProg),
			OnFinishProg = recipe.OnFinishProg is null ? null : LookupProg(recipe.OnFinishProg),
			OnCancelProg = recipe.OnCancelProg is null ? null : LookupProg(recipe.OnCancelProg),
			Difficulty = recipe.Difficulty, Threshold = recipe.Threshold, FreeChecks = recipe.FreeChecks,
			FailPhase = recipe.FailPhase, Interruptable = recipe.Interruptable,
			Phases = recipe.Phases.Select(phase => new CraftPhaseSpec { Seconds = phase.Seconds, Echo = phase.Echo,
				FailEcho = phase.FailEcho, Exertion = phase.Exertion, Stamina = phase.Stamina }).ToArray(),
			Inputs = recipe.Inputs.Select(Input).ToArray(), Tools = recipe.Tools.Select(Tool).ToArray(),
			Products = recipe.Products.Select(Product).ToArray(), FailProducts = recipe.FailProducts.Select(Product).ToArray()
		});
	}
}
