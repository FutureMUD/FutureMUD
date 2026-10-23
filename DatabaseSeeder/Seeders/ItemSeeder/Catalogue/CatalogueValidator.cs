#nullable enable

using System;
using System.IO;
using System.Linq;

namespace DatabaseSeeder.Seeders;

internal static class CatalogueValidator
{
	internal static void Validate(CatalogueMetadata metadata)
	{
		if (string.IsNullOrWhiteSpace(metadata.StableKey) || metadata.StableKey != metadata.StableKey.Trim() ||
			string.IsNullOrWhiteSpace(metadata.Source) || string.IsNullOrWhiteSpace(metadata.Module) ||
			metadata.EraAdmissions.Any(string.IsNullOrWhiteSpace) ||
			metadata.EraAdmissions.Distinct(StringComparer.OrdinalIgnoreCase).Count() != metadata.EraAdmissions.Count)
			throw new InvalidDataException($"Incomplete or ambiguous catalogue identity: {metadata.StableKey}.");
		if (metadata.Readiness != CatalogueReadiness.ProductionReady)
			throw new InvalidDataException($"Catalogue definition {metadata.StableKey} is {metadata.Readiness}, not ProductionReady.");
	}

	internal static void Validate(CatalogueItemDefinition item)
	{
		Validate(item.Metadata);
		if (!double.IsFinite(item.WeightInGrams) || item.WeightInGrams <= 0 || item.Cost < 0 || item.MorphSeconds < 0 ||
			string.IsNullOrWhiteSpace(item.Noun) || string.IsNullOrWhiteSpace(item.ShortDescription) ||
			string.IsNullOrWhiteSpace(item.FullDescription) || string.IsNullOrWhiteSpace(item.Material) ||
			!Enum.IsDefined(item.Size) || !Enum.IsDefined(item.Quality))
			throw new InvalidDataException($"Invalid physical catalogue item: {item.Metadata.StableKey}.");
	}

	internal static void Validate(CatalogueCraftDefinition craft)
	{
		Validate(craft.Metadata);
		if (craft.Phases.Count == 0 || craft.FailPhase < 1 || craft.FailPhase > craft.Phases.Count ||
			craft.Phases.Any(x => x.Seconds <= 0 || !double.IsFinite(x.Stamina) || x.Stamina < 0) ||
			craft.Inputs.Any(x => !double.IsFinite(x.Grams) || x.Grams <= 0 || !double.IsFinite(x.QualityWeight) || x.QualityWeight < 0) ||
			craft.Tools.Any(x => !Enum.IsDefined(x.State) || !double.IsFinite(x.QualityWeight) || x.QualityWeight < 0) ||
			craft.Products.Concat(craft.FailProducts).Any(x => x.Quantity <= 0 || string.IsNullOrWhiteSpace(x.ItemStableReference)))
			throw new InvalidDataException($"Invalid catalogue recipe: {craft.Metadata.StableKey}.");
	}

	internal static void Validate(CatalogueRecipeDefinition recipe)
	{
		Validate(recipe.Metadata);
		void Require(bool condition, string field)
		{
			if (!condition) throw new InvalidDataException($"Invalid catalogue recipe {recipe.Metadata.StableKey}: {field}.");
		}
		static bool Positive(double value) => double.IsFinite(value) && value > 0;
		static bool Reference(string? value) => !string.IsNullOrWhiteSpace(value);
		bool InputIndex(int index) => index > 0 && index <= recipe.Inputs.Count;
		Require(recipe.QualityPolicyVersion == 1, "quality policy version");
		Require(Reference(recipe.Name) && Reference(recipe.Category), "identity");
		Require(recipe.Phases.Count > 0 && recipe.FailPhase > 0 && recipe.FailPhase <= recipe.Phases.Count, "fail phase");
		Require(recipe.Phases.All(phase => phase.Seconds > 0 && double.IsFinite(phase.Stamina) && phase.Stamina >= 0 && Enum.IsDefined(phase.Exertion)), "phases");
		Require(recipe.RequiredAnyEra.All(Reference), "selection references");
		Require(recipe.CheckTraitChoices.All(Reference), "check trait choices");
		foreach (var input in recipe.Inputs)
		{
			Require(double.IsFinite(input.QualityWeight) && input.QualityWeight >= 0, "input quality weight");
			Require(input switch
			{
				CatalogueCommodityRecipeInput value => Positive(value.Grams) && Reference(value.Material) && Enum.IsDefined(value.CharacteristicMode),
				CatalogueCommodityTagRecipeInput value => Positive(value.Grams) && Reference(value.MaterialTag) && Enum.IsDefined(value.CharacteristicMode),
				CatalogueTagRecipeInput value => value.Quantity > 0 && Reference(value.Tag),
				CatalogueItemRecipeInput value => value.Quantity > 0 && Reference(value.ItemStableReference),
				CatalogueLiquidRecipeInput value => Positive(value.Litres) && Reference(value.Liquid),
				CatalogueLiquidTagRecipeInput value => Positive(value.Litres) && Reference(value.LiquidTag),
				_ => false
			}, "input");
		}
		foreach (var tool in recipe.Tools)
		{
			Require(Enum.IsDefined(tool.State) && double.IsFinite(tool.QualityWeight) && tool.QualityWeight >= 0, "tool state or quality");
			Require(tool switch { CatalogueTagRecipeTool value => Reference(value.Tag), CatalogueItemRecipeTool value => Reference(value.ItemStableReference), _ => false }, "tool reference");
		}
		foreach (var product in recipe.Products.Concat(recipe.FailProducts))
		{
			Require(product.MaterialDefiningInputIndex is null || product.MaterialDefiningInputIndex >= 0 && product.MaterialDefiningInputIndex < recipe.Inputs.Count, "material-defining input");
			Require(product switch
			{
				CatalogueItemRecipeProduct value => value.Quantity > 0 && Reference(value.ItemStableReference),
				CatalogueCookedRecipeProduct value => value.Quantity > 0 && Reference(value.ItemStableReference) && value.Ingredients.All(slot => InputIndex(slot.InputIndex) && Reference(slot.Role)),
				CatalogueProgCookedRecipeProduct value => Reference(value.Prog) && value.Ingredients.All(slot => InputIndex(slot.InputIndex) && Reference(slot.Role)),
				CatalogueLiquidRecipeProduct value => value.Quantity > 0 && Reference(value.ItemStableReference) && Reference(value.Liquid) && Positive(value.Litres),
				CatalogueVariableRecipeProduct value => value.Quantity > 0 && Reference(value.ItemStableReference) &&
					value.Variables.All(variable => Reference(variable.Definition) && InputIndex(variable.InputIndex)) &&
					value.Variables.Select(variable => variable.Definition).Distinct(StringComparer.OrdinalIgnoreCase).Count() == value.Variables.Count,
				CatalogueCommodityRecipeProduct value => Positive(value.Grams) && Reference(value.Material) &&
					value.Characteristics.All(characteristic => Reference(characteristic.Definition) &&
						(characteristic.InputIndex is { } index ? InputIndex(index) && characteristic.Value is null : Reference(characteristic.Value))),
				CatalogueProgRecipeProduct value => Reference(value.Prog),
				CatalogueScrapRecipeProduct value => InputIndex(value.InputIndex) && double.IsFinite(value.FractionRecovered) && value.FractionRecovered >= 0 && value.FractionRecovered <= 1,
				CatalogueUnusedRecipeProduct value => InputIndex(value.InputIndex) && double.IsFinite(value.FractionRecovered) && value.FractionRecovered >= 0 && value.FractionRecovered <= 1,
				_ => false
			}, "product");
		}
	}
}
