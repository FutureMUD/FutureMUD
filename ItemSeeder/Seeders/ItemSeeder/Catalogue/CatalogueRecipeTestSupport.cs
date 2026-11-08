#nullable enable

using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	internal int CompareRecipeSerializersForTesting(IEnumerable<CatalogueRecipeDefinition> recipes)
	{
		var definitions = recipes.ToArray();
		// This verifies serialization, not Core prerequisite installation. Supply deterministic
		// symbolic providers absent from the small capture fixture to both serializers.
		long fixtureId = 1_000_000;
		void Tag(string? name)
		{
			if (name is not null && !_tags.ContainsKey(name) && !_tagsByFullPath.ContainsKey(name))
				_tags[name] = new Tag { Id = fixtureId++, Name = name };
		}
		void Material(string name)
		{
			if (!_materials.ContainsKey(name)) _materials[name] = new Material { Id = fixtureId++, Name = name };
		}
		void Liquid(string name)
		{
			if (!_liquids.ContainsKey(name)) _liquids[name] = new Liquid { Id = fixtureId++, Name = name };
		}
		foreach (var recipe in definitions)
		{
			foreach (var input in recipe.Inputs)
				switch (input)
				{
					case CatalogueCommodityRecipeInput value: Material(value.Material); Tag(value.PileTag); break;
					case CatalogueCommodityTagRecipeInput value: Tag(value.MaterialTag); Tag(value.PileTag); break;
					case CatalogueTagRecipeInput value: Tag(value.Tag); break;
					case CatalogueLiquidRecipeInput value: Liquid(value.Liquid); break;
					case CatalogueLiquidTagRecipeInput value: Tag(value.LiquidTag); break;
				}
			foreach (var tool in recipe.Tools.OfType<CatalogueTagRecipeTool>()) Tag(tool.Tag);
			foreach (var product in recipe.Products.Concat(recipe.FailProducts))
				switch (product)
				{
					case CatalogueCommodityRecipeProduct value: Material(value.Material); Tag(value.Tag); break;
					case CatalogueLiquidRecipeProduct value: Liquid(value.Liquid); break;
					case CatalogueScrapRecipeProduct value: Tag(value.Tag); break;
				}
		}
		var comparisons = 0;
		var previousCapture = _manifestCaptureOnly;
		_manifestCaptureOnly = true;
		try
		{
			void Equal(string key, (string TypeName, string Definition) typed, (string TypeName, string Definition) legacy)
			{
				if (typed.TypeName != legacy.TypeName || !XNode.DeepEquals(XElement.Parse(typed.Definition), XElement.Parse(legacy.Definition)))
					throw new InvalidDataException($"Recipe serializer mismatch for {key}:\nTyped: {typed}\nLegacy: {legacy}");
				comparisons++;
			}
			foreach (var recipe in definitions)
			{
				var craft = new Craft();
				for (var i = 0; i < recipe.Inputs.Count; i++)
					craft.CraftInputs.Add(new CraftInput { Id = i + 1, OriginalAdditionTime = DateTime.UnixEpoch.AddTicks(i) });
				foreach (var input in recipe.Inputs)
				{
					var typed = BuildRecipeInput(input);
					Equal(recipe.Metadata.StableKey, typed, BuildInputDefinition(RecipeProjection(typed.TypeName, input.Compatibility).Text));
				}
				foreach (var tool in recipe.Tools)
				{
					var typed = BuildRecipeTool(tool);
					var legacy = BuildToolDefinition(RecipeProjection(typed.TypeName, tool.Compatibility).Text);
					if (typed.State != legacy.State) throw new InvalidDataException($"Recipe tool state differs for {recipe.Metadata.StableKey}.");
					Equal(recipe.Metadata.StableKey, (typed.TypeName, typed.Definition), (legacy.TypeName, legacy.Definition));
				}
				foreach (var product in recipe.Products.Concat(recipe.FailProducts))
				{
					var typed = BuildRecipeProduct(craft, product);
					Equal(recipe.Metadata.StableKey, typed, BuildProductDefinition(craft, RecipeProjection(typed.TypeName, product.Compatibility).Text));
				}
			}
			return comparisons;
		}
		finally { _manifestCaptureOnly = previousCapture; }
	}
}
