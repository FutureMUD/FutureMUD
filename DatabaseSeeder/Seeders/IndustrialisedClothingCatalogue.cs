#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DatabaseSeeder.Seeders;

internal enum ClothingProductionRoute { Hand, MachineAssisted, Batch }
internal enum ClothingReviewStatus { Draft, Reviewed }
internal enum ClothingInputKind { Item, Tag, Commodity, CommodityTag, Liquid }
internal enum ClothingProductKind { Item, Commodity, UnusedInput }
internal enum ClothingToolPlacement { Held, Wielded, InRoom }

internal sealed record ClothingSourceLocation(string File, int Line)
{
	public override string ToString() => $"{File}:{Line}";
	internal InvalidDataException Error(string message) => new($"{this}: {message}");
}

internal sealed record ClothingBaseRow(ClothingSourceLocation Source, string ItemReference, IReadOnlyList<string> EraAdmissions, string Family,
	ClothingProductionRoute ProductionRoute, string DesignRationale, string QualityRationale,
	ClothingReviewStatus ReviewStatus, string SourceNote);

internal sealed record ClothingSkinRow(ClothingSourceLocation Source, string StableReference, string BaseItemReference,
	IReadOnlyList<string> EraAdmissions, string Noun, string ShortDescription, string FullDescription,
	ClothingProductionRoute ProductionRoute, ItemQuality? QualityOverride, string QualityOverrideApproval,
	string DesignRationale, ClothingReviewStatus ReviewStatus, string SourceNote);

internal sealed record ClothingColourRow(ClothingSourceLocation Source, string PresentationReference, string Variable,
	string Definition, string Profile, IReadOnlyList<string> AllowedValues, string DefaultValue,
	string FixedValue, string ExceptionApproval, string SourceNote);

internal sealed record ClothingPaletteRow(ClothingSourceLocation Source, string Palette, string Variable, string Value);

internal sealed record ClothingOutfitRow(ClothingSourceLocation Source, string StableReference, string Name,
	string Description, IReadOnlyList<string> EraAdmissions, ClothingReviewStatus ReviewStatus, string SourceNote);

internal sealed record ClothingOutfitEntryRow(ClothingSourceLocation Source, string OutfitReference, string EntryKey,
	int Order, string ItemReference, string SkinReference, string WearProfile, OutfitTemplateItemPlacement Placement,
	string ContainerKey, string Palette, string SourceNote);

internal sealed record ClothingOutfitColourRow(ClothingSourceLocation Source, string OutfitReference, string EntryKey,
	string Variable, string Value);

internal sealed record ClothingCraftRow(ClothingSourceLocation Source, string StableReference, string Name,
	string Category, string Blurb, string Action, string ActiveItemDescription, IReadOnlyList<string> EraAdmissions,
	ClothingProductionRoute ProductionRoute, string Trait, int MinimumTraitValue, Difficulty Difficulty,
	Outcome FailureThreshold, int FreeChecks, int FailPhase, bool Interruptable,
	ClothingReviewStatus ReviewStatus, string SourceNote);

internal sealed record ClothingCraftPhaseRow(ClothingSourceLocation Source, string CraftReference, int Order,
	int Seconds, string Echo, string FailEcho);

internal sealed record ClothingCraftInputRow(ClothingSourceLocation Source, string CraftReference, int Order,
	ClothingInputKind Kind, string Reference, double Quantity, double QualityWeight);

internal sealed record ClothingCraftToolRow(ClothingSourceLocation Source, string CraftReference, int Order,
	string Tag, ClothingToolPlacement Placement, double QualityWeight, bool UseToolDuration);

internal sealed record ClothingCraftProductRow(ClothingSourceLocation Source, string CraftReference, int Order,
	bool FailureProduct, ClothingProductKind Kind, string Reference, string SkinReference, double Quantity,
	int? MaterialInputOrder);

internal sealed record ClothingCraftColourRow(ClothingSourceLocation Source, string CraftReference, int ProductOrder,
	bool FailureProduct, string Variable, string Value, int? InputOrder);

/// <summary>
/// Authored presentation and graph contracts, separate from editorial acceptance and installed state.
/// Empty source tables reserve infrastructure, not accepted items or a completed content package.
/// </summary>
internal sealed record IndustrialisedClothingCatalogueDocument(
	IReadOnlyList<ClothingBaseRow> Bases, IReadOnlyList<ClothingSkinRow> Skins,
	IReadOnlyList<ClothingColourRow> Colours, IReadOnlyList<ClothingPaletteRow> Palettes,
	IReadOnlyList<ClothingOutfitRow> Outfits, IReadOnlyList<ClothingOutfitEntryRow> OutfitEntries,
	IReadOnlyList<ClothingOutfitColourRow> OutfitColours, IReadOnlyList<ClothingCraftRow> Crafts,
	IReadOnlyList<ClothingCraftPhaseRow> CraftPhases, IReadOnlyList<ClothingCraftInputRow> CraftInputs,
	IReadOnlyList<ClothingCraftToolRow> CraftTools, IReadOnlyList<ClothingCraftProductRow> CraftProducts,
	IReadOnlyList<ClothingCraftColourRow> CraftColours);

internal static class IndustrialisedClothingCatalogue
{
	internal static readonly IReadOnlyDictionary<string, string> Headers = new Dictionary<string, string>(StringComparer.Ordinal)
	{
		["bases.tsv"] = "ItemReference\tEraAdmissions\tFamily\tProductionRoute\tDesignRationale\tQualityRationale\tReviewStatus\tSourceNote",
		["skins.tsv"] = "StableReference\tBaseItemReference\tEraAdmissions\tNoun\tShortDescription\tFullDescription\tProductionRoute\tQualityOverride\tQualityOverrideApproval\tDesignRationale\tReviewStatus\tSourceNote",
		["colours.tsv"] = "PresentationReference\tVariable\tDefinition\tProfile\tAllowedValues\tDefaultValue\tFixedValue\tExceptionApproval\tSourceNote",
		["palettes.tsv"] = "Palette\tVariable\tValue",
		["outfits.tsv"] = "StableReference\tName\tDescription\tEraAdmissions\tReviewStatus\tSourceNote",
		["outfit-entries.tsv"] = "OutfitReference\tEntryKey\tOrder\tItemReference\tSkinReference\tWearProfile\tPlacement\tContainerKey\tPalette\tSourceNote",
		["outfit-colours.tsv"] = "OutfitReference\tEntryKey\tVariable\tValue",
		["crafts.tsv"] = "StableReference\tName\tCategory\tBlurb\tAction\tActiveItemDescription\tEraAdmissions\tProductionRoute\tTrait\tMinimumTraitValue\tDifficulty\tFailureThreshold\tFreeChecks\tFailPhase\tInterruptable\tReviewStatus\tSourceNote",
		["craft-phases.tsv"] = "CraftReference\tOrder\tSeconds\tEcho\tFailEcho",
		["craft-inputs.tsv"] = "CraftReference\tOrder\tKind\tReference\tQuantity\tQualityWeight",
		["craft-tools.tsv"] = "CraftReference\tOrder\tTag\tPlacement\tQualityWeight\tUseToolDuration",
		["craft-products.tsv"] = "CraftReference\tOrder\tFailureProduct\tKind\tReference\tSkinReference\tQuantity\tMaterialInputOrder",
		["craft-colours.tsv"] = "CraftReference\tProductOrder\tFailureProduct\tVariable\tValue\tInputOrder"
	}.AsReadOnly();

	internal static IndustrialisedClothingCatalogueDocument Load(IEnumerable<IndustrialisedCatalogueSource> input)
	{
		var sources = input.ToArray();
		foreach (var source in sources)
		{
			if (!source.Name.StartsWith("Clothing/", StringComparison.Ordinal) || !Headers.ContainsKey(source.Name[9..]))
			{
				throw new InvalidDataException($"Unknown authored clothing source {source.Name}.");
			}
		}

		IReadOnlyList<T> Read<T>(string name, Func<ClothingSourceLocation, string[], T> parse)
		{
			var matches = sources.Where(x => x.Name == $"Clothing/{name}").ToArray();
			if (matches.Length != 1)
			{
				throw new InvalidDataException($"Authored clothing source Clothing/{name} is required exactly once.");
			}

			return Array.AsReadOnly(matches[0].Read(Headers[name].Split('\t'), (file, line, fields) => parse(new(file, line), fields)).ToArray());
		}

		var document = new IndustrialisedClothingCatalogueDocument(
			Read("bases.tsv", (s, x) => new ClothingBaseRow(s, Key(x[0]), Eras(x[1]), Key(x[2]), E<ClothingProductionRoute>(x[3]),
				Required(x[4]), Required(x[5]), E<ClothingReviewStatus>(x[6]), Required(x[7]))),
			Read("skins.tsv", (s, x) => new ClothingSkinRow(s, Key(x[0]), Key(x[1]), Eras(x[2]), Required(x[3]),
				Required(x[4]), Required(x[5]), E<ClothingProductionRoute>(x[6]), OptionalEnum<ItemQuality>(x[7]), x[8],
				Required(x[9]), E<ClothingReviewStatus>(x[10]), Required(x[11]))),
			Read("colours.tsv", (s, x) => new ClothingColourRow(s, Key(x[0]), Key(x[1]), Required(x[2]), Required(x[3]),
				IndustrialisedCatalogueValues.List(x[4]), Required(x[5]), x[6], x[7], Required(x[8]))),
			Read("palettes.tsv", (s, x) => new ClothingPaletteRow(s, Key(x[0]), Key(x[1]), Required(x[2]))),
			Read("outfits.tsv", (s, x) => new ClothingOutfitRow(s, Key(x[0]), Required(x[1]), Required(x[2]), Eras(x[3]),
				E<ClothingReviewStatus>(x[4]), Required(x[5]))),
			Read("outfit-entries.tsv", (s, x) => new ClothingOutfitEntryRow(s, Key(x[0]), Key(x[1]), PositiveInt(x[2]),
				Key(x[3]), OptionalKey(x[4]), x[5], E<OutfitTemplateItemPlacement>(x[6]), OptionalKey(x[7]), OptionalKey(x[8]), Required(x[9]))),
			Read("outfit-colours.tsv", (s, x) => new ClothingOutfitColourRow(s, Key(x[0]), Key(x[1]), Key(x[2]), Required(x[3]))),
			Read("crafts.tsv", (s, x) => new ClothingCraftRow(s, Key(x[0]), Required(x[1]), Required(x[2]), Required(x[3]),
				Required(x[4]), Required(x[5]), Eras(x[6]), E<ClothingProductionRoute>(x[7]), Required(x[8]), NonNegativeInt(x[9]),
				E<Difficulty>(x[10]), E<Outcome>(x[11]), NonNegativeInt(x[12]), PositiveInt(x[13]), bool.Parse(x[14]),
				E<ClothingReviewStatus>(x[15]), Required(x[16]))),
			Read("craft-phases.tsv", (s, x) => new ClothingCraftPhaseRow(s, Key(x[0]), PositiveInt(x[1]), PositiveInt(x[2]), Required(x[3]), Required(x[4]))),
			Read("craft-inputs.tsv", (s, x) => new ClothingCraftInputRow(s, Key(x[0]), PositiveInt(x[1]), E<ClothingInputKind>(x[2]),
				Required(x[3]), Positive(x[4]), NonNegative(x[5]))),
			Read("craft-tools.tsv", (s, x) => new ClothingCraftToolRow(s, Key(x[0]), PositiveInt(x[1]), Required(x[2]),
				E<ClothingToolPlacement>(x[3]), NonNegative(x[4]), bool.Parse(x[5]))),
			Read("craft-products.tsv", (s, x) => new ClothingCraftProductRow(s, Key(x[0]), PositiveInt(x[1]), bool.Parse(x[2]),
				E<ClothingProductKind>(x[3]), Required(x[4]), OptionalKey(x[5]), Positive(x[6]), OptionalIndex(x[7]))),
			Read("craft-colours.tsv", (s, x) => new ClothingCraftColourRow(s, Key(x[0]), PositiveInt(x[1]), bool.Parse(x[2]),
				Key(x[3]), x[4], OptionalIndex(x[5]))));
		ValidateStructure(document);
		return document;
	}

	internal static void ValidateStructure(IndustrialisedClothingCatalogueDocument d)
	{
		Unique(d.Bases, x => x.ItemReference, x => x.Source);
		Unique(d.Skins, x => x.StableReference, x => x.Source);
		Unique(d.Outfits, x => x.StableReference, x => x.Source);
		Unique(d.Crafts, x => x.StableReference, x => x.Source);
		Unique(d.Colours, x => $"{x.PresentationReference}/{x.Variable}", x => x.Source);
		Unique(d.Palettes, x => $"{x.Palette}/{x.Variable}", x => x.Source);
		Unique(d.OutfitEntries, x => $"{x.OutfitReference}/{x.EntryKey}", x => x.Source);
		Unique(d.OutfitColours, x => $"{x.OutfitReference}/{x.EntryKey}/{x.Variable}", x => x.Source);
		Unique(d.CraftColours, x => $"{x.CraftReference}/{x.FailureProduct}/{x.ProductOrder}/{x.Variable}", x => x.Source);
		var bases = d.Bases.ToDictionary(x => x.ItemReference, StringComparer.Ordinal);
		var skins = d.Skins.ToDictionary(x => x.StableReference, StringComparer.Ordinal);
		var outfits = d.Outfits.ToDictionary(x => x.StableReference, StringComparer.Ordinal);
		var crafts = d.Crafts.ToDictionary(x => x.StableReference, StringComparer.Ordinal);
		foreach (var skin in d.Skins)
		{
			if (!bases.TryGetValue(skin.BaseItemReference, out var item) || bases.ContainsKey(skin.StableReference))
				throw skin.Source.Error("Skin must identify one declared base and have a distinct presentation identity.");
			if (skin.ProductionRoute != item.ProductionRoute)
				throw skin.Source.Error("Skin changes the base's normal production route; use an explicitly designed economic base.");
			if (skin.EraAdmissions.Except(item.EraAdmissions, StringComparer.Ordinal).Any())
				throw skin.Source.Error("Skin is not admitted by its base in every source era.");
			if (skin.QualityOverride.HasValue != !string.IsNullOrWhiteSpace(skin.QualityOverrideApproval))
				throw skin.Source.Error("A skin quality override requires an explicit approval reference; leave both cells empty by default.");
			IndustrialisedClothingColourPlan.Channels(d, skin.BaseItemReference, skin.StableReference);
		}
		foreach (var colour in d.Colours)
		{
			if (!bases.ContainsKey(colour.PresentationReference) && !skins.ContainsKey(colour.PresentationReference))
				throw colour.Source.Error("Unknown colour presentation.");
			if (!colour.AllowedValues.Contains(colour.DefaultValue, StringComparer.Ordinal))
				throw colour.Source.Error("Default colour is not an exact permitted value.");
			if (colour.FixedValue.Length > 0 || colour.ExceptionApproval.Length > 0)
				throw colour.Source.Error("Wave 1 approves no fixed-colour exceptions. Put conventional colours in outfit defaults.");
		}
		foreach (var item in d.Bases)
		{
			if (!d.Colours.Any(x => x.PresentationReference == item.ItemReference))
				throw item.Source.Error("Every standalone base requires explicit variable colour bindings.");
		}
		foreach (var entry in d.OutfitEntries)
		{
			if (!outfits.ContainsKey(entry.OutfitReference) || !bases.ContainsKey(entry.ItemReference))
				throw entry.Source.Error("Outfit entry refers to an undeclared outfit or base.");
			CheckSkin(entry.SkinReference, entry.ItemReference, outfits[entry.OutfitReference].EraAdmissions, entry.Source);
			if (entry.Palette.Length > 0 && !d.Palettes.Any(x => x.Palette == entry.Palette))
				throw entry.Source.Error("Unknown outfit palette.");
			if (entry.ContainerKey.Length > 0 && !d.OutfitEntries.Any(x => x.OutfitReference == entry.OutfitReference &&
				x.EntryKey == entry.ContainerKey && x.Order < entry.Order))
				throw entry.Source.Error("Related outfit entry must exist earlier in wear order; cycles are not allowed.");
			if (entry.Placement is OutfitTemplateItemPlacement.Container or OutfitTemplateItemPlacement.AttachedToBelt or OutfitTemplateItemPlacement.Sheathed && entry.ContainerKey.Length == 0)
				throw entry.Source.Error("Related-item placement requires an explicit container/attachment entry key.");
		}
		foreach (var outfit in d.Outfits)
			Ordered(d.OutfitEntries.Where(x => x.OutfitReference == outfit.StableReference), x => x.Order, outfit.Source, "outfit entries", true);
		foreach (var colour in d.OutfitColours)
		{
			if (!d.OutfitEntries.Any(x => x.OutfitReference == colour.OutfitReference && x.EntryKey == colour.EntryKey))
				throw colour.Source.Error("Unknown outfit colour entry.");
		}
		foreach (var (reference, source) in d.CraftPhases.Select(x => (x.CraftReference, x.Source))
			.Concat(d.CraftInputs.Select(x => (x.CraftReference, x.Source)))
			.Concat(d.CraftTools.Select(x => (x.CraftReference, x.Source)))
			.Concat(d.CraftProducts.Select(x => (x.CraftReference, x.Source)))
			.Concat(d.CraftColours.Select(x => (x.CraftReference, x.Source))))
		{
			if (!crafts.ContainsKey(reference)) throw source.Error($"Unknown craft {reference}.");
		}
		foreach (var craft in d.Crafts)
		{
			var phases = d.CraftPhases.Where(x => x.CraftReference == craft.StableReference).ToArray();
			Ordered(phases, x => x.Order, craft.Source, "craft phases", true);
			Ordered(d.CraftInputs.Where(x => x.CraftReference == craft.StableReference), x => x.Order, craft.Source, "craft inputs", true);
			Ordered(d.CraftTools.Where(x => x.CraftReference == craft.StableReference), x => x.Order, craft.Source, "craft tools", true);
			Ordered(d.CraftProducts.Where(x => x.CraftReference == craft.StableReference && !x.FailureProduct), x => x.Order, craft.Source, "success products", true);
			Ordered(d.CraftProducts.Where(x => x.CraftReference == craft.StableReference && x.FailureProduct), x => x.Order, craft.Source, "failure products", false);
			if (craft.FailPhase > phases.Length) throw craft.Source.Error("Failure phase does not exist.");
		}
		foreach (var product in d.CraftProducts)
		{
			if (product.Kind == ClothingProductKind.Item)
			{
				if (!bases.TryGetValue(product.Reference, out var item)) throw product.Source.Error("Craft product must be a declared base.");
				CheckSkin(product.SkinReference, product.Reference, crafts[product.CraftReference].EraAdmissions, product.Source);
				if (!product.FailureProduct && item.ProductionRoute != crafts[product.CraftReference].ProductionRoute)
					throw product.Source.Error("Craft route differs from product's normal production route.");
				if (product.Quantity != Math.Truncate(product.Quantity)) throw product.Source.Error("Item product quantity must be an integer.");
			}
			else if (product.SkinReference.Length > 0 || product.MaterialInputOrder is not null)
				throw product.Source.Error("Only item products may select a skin or material-defining input.");
			if (product.MaterialInputOrder is { } index && !d.CraftInputs.Any(x => x.CraftReference == product.CraftReference && x.Order == index))
				throw product.Source.Error("Unknown material-defining input.");
			if (product.Kind == ClothingProductKind.UnusedInput && (!int.TryParse(product.Reference, out var input) ||
				!d.CraftInputs.Any(x => x.CraftReference == product.CraftReference && x.Order == input) || product.Quantity > 1.0))
				throw product.Source.Error("Unused-input product requires an existing one-based input and a fraction at most one.");
		}
		foreach (var colour in d.CraftColours)
		{
			if (!d.CraftProducts.Any(x => x.CraftReference == colour.CraftReference && x.Order == colour.ProductOrder &&
				x.FailureProduct == colour.FailureProduct && x.Kind == ClothingProductKind.Item))
				throw colour.Source.Error("Unknown item product for colour selection.");
			if ((colour.Value.Length > 0) == colour.InputOrder.HasValue)
				throw colour.Source.Error("Select exactly one explicit colour value or source input.");
			if (colour.InputOrder is { } index && !d.CraftInputs.Any(x => x.CraftReference == colour.CraftReference && x.Order == index))
				throw colour.Source.Error("Unknown colour source input.");
		}
		foreach (var entry in d.OutfitEntries)
			IndustrialisedClothingColourPlan.OutfitValues(d, entry);
		foreach (var product in d.CraftProducts.Where(x => x.Kind == ClothingProductKind.Item))
			IndustrialisedClothingColourPlan.CraftValues(d, product);

		void CheckSkin(string key, string item, IReadOnlyList<string> eras, ClothingSourceLocation source)
		{
			if (eras.Except(bases[item].EraAdmissions, StringComparer.Ordinal).Any())
				throw source.Error("Base is not admitted in every requested era.");
			if (key.Length == 0) return; // The unskinned default is a complete product.
			if (!skins.TryGetValue(key, out var skin) || skin.BaseItemReference != item)
				throw source.Error("Unknown skin or skin/base mismatch.");
			if (eras.Except(skin.EraAdmissions, StringComparer.Ordinal).Any())
				throw source.Error("Skin is not admitted in every requested era.");
		}
	}

	private static void Unique<T>(IEnumerable<T> rows, Func<T, string> key, Func<T, ClothingSourceLocation> source)
	{
		var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var row in rows)
			if (!seen.Add(key(row))) throw source(row).Error($"Duplicate key {key(row)}.");
	}

	private static void Ordered<T>(IEnumerable<T> rows, Func<T, int> order, ClothingSourceLocation source, string label, bool required)
	{
		var values = rows.Select(order).OrderBy(x => x).ToArray();
		if ((required && values.Length == 0) || !values.SequenceEqual(Enumerable.Range(1, values.Length)))
			throw source.Error($"{label} must have unique contiguous one-based order.");
	}

	private static string Required(string text) => string.IsNullOrWhiteSpace(text) ? throw new FormatException("Required value is empty.") : text;
	private static string Key(string text) => Regex.IsMatch(text, "^[a-z][a-z0-9_]*$", RegexOptions.CultureInvariant)
		? text : throw new FormatException($"Invalid stable key '{text}'.");
	private static string OptionalKey(string text) => text.Length == 0 ? text : Key(text);
	private static T E<T>(string text) where T : struct, Enum => IndustrialisedCatalogueValues.EnumValue<T>(text);
	private static T? OptionalEnum<T>(string text) where T : struct, Enum => text.Length == 0 ? null : E<T>(text);
	private static int? OptionalIndex(string text) => text.Length == 0 ? null : PositiveInt(text);
	private static int PositiveInt(string text) => IndustrialisedCatalogueValues.Int(text) is > 0 and var value ? value : throw new FormatException("Expected a positive integer.");
	private static int NonNegativeInt(string text) => IndustrialisedCatalogueValues.Int(text) is >= 0 and var value ? value : throw new FormatException("Expected a non-negative integer.");
	private static double Positive(string text) => IndustrialisedCatalogueValues.Double(text) is > 0 and var value ? value : throw new FormatException("Expected a positive number.");
	private static double NonNegative(string text) => IndustrialisedCatalogueValues.Double(text) is >= 0 and var value ? value : throw new FormatException("Expected a non-negative number.");
	private static IReadOnlyList<string> Eras(string text)
	{
		var values = IndustrialisedCatalogueValues.List(text);
		var canonical = new[] { "industrial", "modern", "nuclear", "information" };
		if (values.Count == 0 || !values.SequenceEqual(canonical.Where(values.Contains)))
			throw new FormatException("Era admissions must be exact later-era tokens in chronological order.");
		return values;
	}
}
