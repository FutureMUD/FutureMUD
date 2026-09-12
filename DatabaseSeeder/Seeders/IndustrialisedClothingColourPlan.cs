#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

/// <summary>
/// Resolves authored colour choices, not random runtime samples. Values remain exact characteristic
/// names here; database preflight must bind them to IDs before producing runtime load arguments.
/// </summary>
internal static class IndustrialisedClothingColourPlan
{
	internal static IReadOnlyDictionary<string, ClothingColourRow> Channels(
		IndustrialisedClothingCatalogueDocument document, string itemReference, string skinReference)
	{
		var channels = document.Colours.Where(x => x.PresentationReference == itemReference)
			.ToDictionary(x => x.Variable, StringComparer.Ordinal);
		foreach (var colour in document.Colours.Where(x => x.PresentationReference == skinReference))
		{
			if (!channels.TryGetValue(colour.Variable, out var inherited) || colour.Definition != inherited.Definition ||
				colour.Profile != inherited.Profile || colour.AllowedValues.Except(inherited.AllowedValues, StringComparer.Ordinal).Any())
			{
				throw colour.Source.Error("A skin may narrow a base colour palette, not change or add characteristic bindings.");
			}

			channels[colour.Variable] = colour;
		}

		return channels.AsReadOnly();
	}

	internal static IReadOnlyDictionary<string, string> OutfitValues(IndustrialisedClothingCatalogueDocument document,
		ClothingOutfitEntryRow entry, IReadOnlyDictionary<string, string>? instanceOverrides = null)
	{
		var channels = Channels(document, entry.ItemReference, entry.SkinReference);
		var palette = document.Palettes.Where(x => x.Palette == entry.Palette)
			.ToDictionary(x => x.Variable, x => x.Value, StringComparer.Ordinal);
		var defaults = document.OutfitColours.Where(x => x.OutfitReference == entry.OutfitReference && x.EntryKey == entry.EntryKey)
			.ToDictionary(x => x.Variable, x => x.Value, StringComparer.Ordinal);
		foreach (var variable in palette.Keys.Concat(defaults.Keys).Concat(instanceOverrides?.Keys ?? []).Distinct(StringComparer.Ordinal))
		{
			if (!channels.ContainsKey(variable))
			{
				throw entry.Source.Error($"Unknown outfit colour channel {variable} on {entry.EntryKey}.");
			}
		}

		var resolved = new Dictionary<string, string>(StringComparer.Ordinal);
		foreach (var (variable, channel) in channels.OrderBy(x => x.Key, StringComparer.Ordinal))
		{
			var value = instanceOverrides?.GetValueOrDefault(variable) ?? defaults.GetValueOrDefault(variable) ?? palette.GetValueOrDefault(variable);
			if (value is null || !channel.AllowedValues.Contains(value, StringComparer.Ordinal))
			{
				throw entry.Source.Error($"Outfit {entry.OutfitReference}/{entry.EntryKey} requires an exact permitted default or selection for {variable}; got '{value ?? "<missing>"}'.");
			}

			resolved.Add(variable, value);
		}

		return resolved.AsReadOnly();
	}

	internal static IReadOnlyDictionary<string, ClothingCraftColourRow> CraftValues(IndustrialisedClothingCatalogueDocument document,
		ClothingCraftProductRow product)
	{
		var channels = Channels(document, product.Reference, product.SkinReference);
		var selections = document.CraftColours.Where(x => x.CraftReference == product.CraftReference &&
			x.ProductOrder == product.Order && x.FailureProduct == product.FailureProduct)
			.ToDictionary(x => x.Variable, StringComparer.Ordinal);
		if (!channels.Keys.OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(selections.Keys.OrderBy(x => x, StringComparer.Ordinal)))
		{
			throw product.Source.Error("Every item-product colour channel needs one explicit value or input mapping, with no unknown channels.");
		}

		foreach (var (variable, selection) in selections)
		{
			if (selection.Value.Length > 0 && !channels[variable].AllowedValues.Contains(selection.Value, StringComparer.Ordinal))
			{
				throw selection.Source.Error($"Craft colour '{selection.Value}' is not permitted for {variable}.");
			}
		}

		return selections.AsReadOnly();
	}
}
