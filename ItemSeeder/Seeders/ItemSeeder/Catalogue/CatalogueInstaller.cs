#nullable enable

using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private GameItemProto? InstallIndustrialCatalogueItem(CatalogueItemDefinition item,
		IReadOnlyList<string> resolvedComponents, bool clothingRequiresSkins) =>
		InstallCatalogueItem(item with { Components = resolvedComponents, Skinnable = item.Skinnable || clothingRequiresSkins });

	private GameItemProto? InstallCatalogueItem(CatalogueItemDefinition item)
	{
		CatalogueValidator.Validate(item);
		return CreateItem(item.Metadata.StableKey, item.Noun, item.ShortDescription, item.LongDescription,
			item.FullDescription, item.Size, item.Quality, item.WeightInGrams, item.Cost, item.Skinnable,
			item.HiddenFromPlayers, item.Material, item.Tags, item.Components, item.MorphToStableReference,
			item.MorphEmote, item.MorphSeconds is { } seconds ? TimeSpan.FromSeconds(seconds) : null, item.DestroyedItemStableReference,
			item.BuilderNotes, item.AllowLegacyShortDescriptionMatch);
	}

	internal static PreIndustrialFoodItemCatalogueEntry ToLegacyFood(CatalogueFoodDefinition food)
	{
		CatalogueValidator.Validate(food.Metadata);
		return new(food.Metadata.StableKey, food.Scope, food.Kind, food.Family, food.Noun, food.ShortDescription,
			food.FullDescription, food.Taste, food.Material, food.Nutrition, food.Freshness, food.Quality,
			food.WeightInGrams, food.Cost, food.AdmissionProfile);
	}

	private static OutfitManifestSpec ToLegacyOutfit(CatalogueOutfitDefinition outfit)
	{
		CatalogueValidator.Validate(outfit.Metadata);
		return new(outfit.Metadata.StableKey, outfit.Name, outfit.Description, outfit.Items
			.Select(item => new OutfitManifestItemSpec(item.ItemStableReference, item.SkinStableReference)
			{
				EntryKey = item.EntryKey, WearProfile = item.WearProfile, Placement = item.Placement,
				ContainerKey = item.ContainerKey, LoadArguments = item.LoadArguments
			}).ToArray());
	}

	private Craft InstallCatalogueCraft(CatalogueCraftDefinition craft)
	{
		CatalogueValidator.Validate(craft);
		var gates = EnsureKnowledgeGateProgs(craft.Knowledge, craft.Trait, craft.MinimumTraitValue, "Crafting",
			craft.KnowledgeSubtype, null, null, 1, Difficulty.Normal, Difficulty.Normal,
			LearnableType.LearnableAtSkillUp | LearnableType.LearnableFromTeacher);
		CraftInputSpec Input(CatalogueCommodityInput input)
		{
			// Compatibility text preserves existing fingerprints. The writer uses the typed value directly.
			var text = CommodityInput(input.Grams, input.Material, input.PileTag);
			var details = text["Commodity - ".Length..].Split(';')[0];
			return new(text, "Commodity", details, input.PileTag is null ? [] : ["piletag " + input.PileTag], input.QualityWeight)
				{ CatalogueInput = input };
		}
		CraftToolSpec Tool(CatalogueTagTool tool)
		{
			var text = $"TagTool - {tool.State} - an item with the {tool.Tag} tag";
			return new(text, "TagTool", $"{tool.State} - an item with the {tool.Tag} tag", [], tool.QualityWeight, tool.UseToolDuration)
				{ CatalogueTool = tool };
		}
		CraftProductSpec Product(CatalogueSimpleProduct product)
		{
			var item = LookupReworkItem(product.ItemStableReference);
			var details = $"{product.Quantity}x {item.ShortDescription} (#{item.Id})";
			return new("SimpleProduct - " + details, "SimpleProduct", details, [], product.IsFailProduct, product.MaterialDefiningInputIndex)
				{ CatalogueProduct = product };
		}
		return AddCraft(new CraftDefinitionSpec
		{
			StableReference = craft.UseExplicitStableReference ? craft.Metadata.StableKey : null,
			PreserveAuthoredText = craft.PreserveAuthoredText, Name = craft.Name, Category = craft.Category,
			Blurb = craft.Blurb, Action = craft.Action, ActiveCraftItemSdesc = craft.ActiveCraftItemSdesc,
			AppearProg = gates.AppearProg, CanUseProg = gates.CanUseProg, WhyCannotUseProg = gates.WhyCannotUseProg,
			OnStartProg = craft.OnStartProg is null ? null : LookupProg(craft.OnStartProg),
			OnFinishProg = craft.OnFinishProg is null ? null : LookupProg(craft.OnFinishProg),
			OnCancelProg = craft.OnCancelProg is null ? null : LookupProg(craft.OnCancelProg),
			Trait = gates.Trait, Difficulty = craft.Difficulty, Threshold = craft.Threshold, FreeChecks = craft.FreeChecks,
			FailPhase = craft.FailPhase, Interruptable = craft.Interruptable, Phases = craft.Phases,
			Inputs = craft.Inputs.Select(Input).ToArray(), Tools = craft.Tools.Select(Tool).ToArray(),
			Products = craft.Products.Select(Product).ToArray(), FailProducts = craft.FailProducts.Select(Product).ToArray()
		});
	}
}
