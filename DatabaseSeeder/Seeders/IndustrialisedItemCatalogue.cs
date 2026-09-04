#nullable enable

using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace DatabaseSeeder.Seeders;

internal sealed record IndustrialisedItemCatalogueRow(
	string Source,
	int Line,
	string StableReference,
	string Layer,
	string Domain,
	IReadOnlyList<string> EraAdmissions,
	string Noun,
	string ShortDescription,
	string FullDescription,
	SizeCategory Size,
	ItemQuality Quality,
	double WeightGrams,
	decimal CostIndex,
	string Material,
	IReadOnlyList<string> Tags,
	IReadOnlyList<string> FixedComponents,
	IReadOnlyList<string> ProfileBindings,
	IReadOnlyList<string> SupportedClaims,
	string? MorphTo,
	int MorphSeconds,
	string? MorphEmote,
	string? DestroyedItem,
	IReadOnlyList<string> PriceEvidence,
	string SourceNote,
	bool Craftable,
	string? LifecycleKind);

internal sealed record IndustrialisedCraftCatalogueRow(
	string Source,
	int Line,
	string StableKey,
	IReadOnlyList<string> EraAdmissions,
	string Category,
	string Trait,
	int MinimumTraitValue,
	string Difficulty,
	string ProductStableReference,
	string InputMaterial,
	double InputGrams);

internal sealed record IndustrialisedOutfitCatalogueRow(
	string Source,
	int Line,
	string OutfitReference,
	string Name,
	string Description,
	IReadOnlyList<string> EraAdmissions,
	IReadOnlyList<string> ItemStableReferences);

internal sealed record IndustrialisedTechnologyBindingRow(
	string Source,
	int Line,
	string Profile,
	string Dimension,
	string Family,
	IReadOnlyList<string> Values,
	bool ComponentBacked);

internal sealed record IndustrialisedPriceEvidenceRow(
	string Source,
	int Line,
	string EvidenceId,
	string EraBand,
	string Locale,
	int StartYear,
	int EndYear,
	string Currency,
	decimal? NominalPrice,
	string QuotedUnit,
	decimal? DailyWage,
	decimal? LabourDays,
	decimal? CostIndex,
	string SourceClass,
	string ComparableFamily,
	string Confidence,
	string SourceUrl,
	string SourcePage,
	string Notes);

internal sealed record IndustrialisedItemCatalogueDocument(
	IReadOnlyList<IndustrialisedItemCatalogueRow> Items,
	IReadOnlyList<IndustrialisedCraftCatalogueRow> Crafts,
	IReadOnlyList<IndustrialisedOutfitCatalogueRow> Outfits,
	IReadOnlyList<IndustrialisedTechnologyBindingRow> TechnologyBindings,
	IReadOnlyList<IndustrialisedPriceEvidenceRow> PriceEvidence)
{
	public required IndustrialisedClothingCatalogueDocument Clothing { get; init; }
	public IndustrialisedFoodCatalogueDocument? Food { get; init; }
}

internal static class IndustrialisedItemCatalogue
{
	private const string ResourceMarker = ".IndustrialisedCatalogue.";
	private static readonly string[] ItemHeaders =
	[
		"StableReference", "Layer", "Domain", "EraAdmissions", "Noun", "ShortDescription",
		"FullDescription", "Size", "Quality", "WeightGrams", "CostIndex", "Material", "Tags",
		"FixedComponents", "ProfileBindings", "SupportedClaims", "MorphTo", "MorphSeconds",
		"MorphEmote", "DestroyedItem", "PriceEvidence", "SourceNote", "Craftable", "LifecycleKind"
	];
	private static readonly string[] CraftHeaders =
	[
		"StableKey", "EraAdmissions", "Category", "Trait", "MinimumTraitValue", "Difficulty",
		"ProductStableReference", "InputMaterial", "InputGrams"
	];
	private static readonly string[] OutfitHeaders =
	[
		"OutfitReference", "Name", "Description", "EraAdmissions", "ItemStableReferences"
	];
	private static readonly string[] TechnologyHeaders =
	[
		"Profile", "Dimension", "Family", "Values", "ComponentBacked"
	];
	private static readonly string[] PriceHeaders =
	[
		"EvidenceId", "EraBand", "Locale", "StartYear", "EndYear", "Currency", "NominalPrice",
		"QuotedUnit", "DailyWage", "LabourDays", "CostIndex", "SourceClass", "ComparableFamily",
		"Confidence", "SourceUrl", "SourcePage", "Notes"
	];

	private static readonly Lazy<IndustrialisedItemCatalogueDocument> LazyDocument = new(LoadInternal);

	internal static IndustrialisedItemCatalogueDocument Document => LazyDocument.Value;

	internal static IndustrialisedItemCatalogueDocument LoadForTesting() => LoadInternal();

	private static IndustrialisedItemCatalogueDocument LoadInternal()
	{
		var assembly = typeof(IndustrialisedItemCatalogue).Assembly;
		return LoadSources(assembly.GetManifestResourceNames()
			.Where(x => x.Contains(ResourceMarker, StringComparison.Ordinal) && x.EndsWith(".tsv", StringComparison.Ordinal))
			.Select(name =>
			{
				using var stream = assembly.GetManifestResourceStream(name) ??
					throw new InvalidDataException($"Embedded catalogue resource {name} could not be opened.");
				using var reader = new StreamReader(stream);
				var relativeName = name[(name.IndexOf(ResourceMarker, StringComparison.Ordinal) + ResourceMarker.Length)..];
				return new IndustrialisedCatalogueSource(relativeName
					.Replace("Items.", "Items/", StringComparison.Ordinal)
					.Replace("Clothing.", "Clothing/", StringComparison.Ordinal)
					.Replace("Food.", "Food/", StringComparison.Ordinal)
					.Replace("Pricing.", "Pricing/", StringComparison.Ordinal), reader.ReadToEnd());
			}));
	}

	internal static IndustrialisedItemCatalogueDocument LoadDirectory(string directory) =>
		LoadSources(Directory.EnumerateFiles(directory, "*.tsv", SearchOption.AllDirectories)
			.Select(path => new IndustrialisedCatalogueSource(Path.GetRelativePath(directory, path).Replace('\\', '/'), File.ReadAllText(path))));

	internal static IndustrialisedItemCatalogueDocument LoadSources(IEnumerable<IndustrialisedCatalogueSource> input)
	{
		var sources = input.OrderBy(x => x.Name, StringComparer.Ordinal).ToArray();
		RequireUnique(sources, x => x.Name, "source path");
		var singletonNames = new[] { "crafts.tsv", "outfits.tsv", "technology-profile-bindings.tsv", "Pricing/historical-price-evidence.tsv" };
		foreach (var name in singletonNames)
		{
			if (sources.Count(x => x.Name == name) != 1)
			{
				throw new InvalidDataException($"Catalogue requires source {name} exactly once.");
			}
		}

		var itemSources = sources.Where(x => x.Name.StartsWith("Items/", StringComparison.Ordinal) &&
			x.Name.EndsWith(".items.tsv", StringComparison.Ordinal)).ToArray();
		var clothingSources = sources.Where(x => x.Name.StartsWith("Clothing/", StringComparison.Ordinal)).ToArray();
		var foodSources = sources.Where(x => x.Name.StartsWith("Food/", StringComparison.Ordinal)).ToArray();
		var clothing = IndustrialisedClothingCatalogue.Load(clothingSources);
		var food = foodSources.Length == 0
			? null
			: IndustrialisedFoodCatalogue.Load(foodSources.Select(x => x with { Name = x.Name["Food/".Length..] }));
		var unexpected = sources.Except(itemSources).Except(clothingSources).Except(foodSources).Where(x => !singletonNames.Contains(x.Name, StringComparer.Ordinal)).ToArray();
		if (unexpected.Length > 0 || itemSources.Length == 0)
		{
			throw new InvalidDataException($"Missing item sources or unrecognised catalogue sources: {string.Join(", ", unexpected.Select(x => x.Name))}.");
		}

		var items = itemSources.SelectMany(x => x.Read(ItemHeaders, ParseItem)).ToArray();
		var crafts = sources.Single(x => x.Name == "crafts.tsv").Read(CraftHeaders, ParseCraft).ToArray();
		var outfits = sources.Single(x => x.Name == "outfits.tsv").Read(OutfitHeaders, ParseOutfit).ToArray();
		var technology = sources.Single(x => x.Name == "technology-profile-bindings.tsv").Read(TechnologyHeaders, ParseTechnology).ToArray();
		var prices = sources.Single(x => x.Name == "Pricing/historical-price-evidence.tsv").Read(PriceHeaders, ParsePrice).ToArray();
		Validate(items, crafts, outfits, technology, prices);
		return new IndustrialisedItemCatalogueDocument(Array.AsReadOnly(items), Array.AsReadOnly(crafts),
			Array.AsReadOnly(outfits), Array.AsReadOnly(technology), Array.AsReadOnly(prices)) { Clothing = clothing, Food = food };
	}

	private static IndustrialisedItemCatalogueRow ParseItem(string source, int line, string[] x)
	{
		var size = IndustrialisedCatalogueValues.EnumValue<SizeCategory>(x[7]);
		var quality = IndustrialisedCatalogueValues.EnumValue<ItemQuality>(x[8]);
		return new IndustrialisedItemCatalogueRow(source, line, x[0], x[1], x[2], List(x[3]), x[4], x[5], x[6],
			size, quality, Double(x[9]), Decimal(x[10]), x[11], List(x[12]), List(x[13]), List(x[14]),
			List(x[15]), Null(x[16]), Int(x[17]), Null(x[18]), Null(x[19]), List(x[20]), x[21], Bool(x[22]), Null(x[23]));
	}

	private static IndustrialisedCraftCatalogueRow ParseCraft(string source, int line, string[] x) =>
		new(source, line, x[0], List(x[1]), x[2], x[3], Int(x[4]), x[5], x[6], x[7], Double(x[8]));

	private static IndustrialisedOutfitCatalogueRow ParseOutfit(string source, int line, string[] x) =>
		new(source, line, x[0], x[1], x[2], List(x[3]), List(x[4]));

	private static IndustrialisedTechnologyBindingRow ParseTechnology(string source, int line, string[] x) =>
		new(source, line, x[0], x[1], x[2], List(x[3]), Bool(x[4]));

	private static IndustrialisedPriceEvidenceRow ParsePrice(string source, int line, string[] x) =>
		new(source, line, x[0], x[1], x[2], Int(x[3]), Int(x[4]), x[5], NullableDecimal(x[6]), x[7], NullableDecimal(x[8]),
			NullableDecimal(x[9]), NullableDecimal(x[10]), x[11], x[12], x[13], x[14], x[15], x[16]);

	private static void Validate(
		IReadOnlyList<IndustrialisedItemCatalogueRow> items,
		IReadOnlyList<IndustrialisedCraftCatalogueRow> crafts,
		IReadOnlyList<IndustrialisedOutfitCatalogueRow> outfits,
		IReadOnlyList<IndustrialisedTechnologyBindingRow> technology,
		IReadOnlyList<IndustrialisedPriceEvidenceRow> prices)
	{
		if (items.Count(x => x.Layer == "shared-industrialised") != 5800 || items.Count(x => x.Layer == "industrial") != 650)
		{
			throw new InvalidDataException("The Stage 2 catalogue must contain exactly 5,800 shared-industrialised and 650 Industrial rows.");
		}
		RequireUnique(items, x => x.StableReference, "item stable reference");
		RequireUnique(crafts, x => x.StableKey, "craft stable key");
		RequireUnique(outfits, x => x.OutfitReference, "outfit reference");
		RequireUnique(prices, x => x.EvidenceId, "price evidence id");
		if (outfits.Count != 100)
		{
			throw new InvalidDataException($"The Stage 2 catalogue must contain exactly 100 outfits; found {outfits.Count}.");
		}
		var itemKeys = items.Select(x => x.StableReference).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var evidenceKeys = prices.Select(x => x.EvidenceId).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var admissionReadyEvidence = prices
			.Where(x => x.NominalPrice > 0 && x.DailyWage > 0 && x.LabourDays > 0 && x.CostIndex > 0)
			.Select(x => x.EvidenceId)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var item in items)
		{
			if (!System.Text.RegularExpressions.Regex.IsMatch(item.StableReference, "^(industrialised|industrial)_[a-z0-9_]+$"))
			{
				throw new InvalidDataException($"{item.Source}:{item.Line}: invalid stable reference {item.StableReference}.");
			}
			if (item.EraAdmissions.Count == 0 || item.EraAdmissions.Any(x => x is not ("industrial" or "modern" or "nuclear" or "information")))
			{
				throw new InvalidDataException($"{item.Source}:{item.Line}: invalid or empty era admissions.");
			}
			if (item.WeightGrams <= 0 || item.CostIndex <= 0 || item.PriceEvidence.Count == 0 || item.PriceEvidence.Any(x => !evidenceKeys.Contains(x)))
			{
				throw new InvalidDataException($"{item.Source}:{item.Line}: invalid physical or pricing data.");
			}
			if (item.PriceEvidence.All(x => !admissionReadyEvidence.Contains(x)))
			{
				throw new InvalidDataException($"{item.Source}:{item.Line}: price evidence is only a gateway and is not admission-ready.");
			}
			if (item.Noun.Contains(' ') || item.FullDescription.Length < 140 || string.IsNullOrWhiteSpace(item.SourceNote))
			{
				throw new InvalidDataException($"{item.Source}:{item.Line}: noun/prose/source-note quality gate failed.");
			}
		}
		foreach (var craft in crafts.Where(x => !itemKeys.Contains(x.ProductStableReference)))
		{
			throw new InvalidDataException($"{craft.Source}:{craft.Line}: missing product {craft.ProductStableReference}.");
		}
		foreach (var outfit in outfits)
		{
			var missing = outfit.ItemStableReferences.Where(x => !itemKeys.Contains(x)).ToArray();
			if (missing.Length > 0)
			{
				throw new InvalidDataException($"{outfit.Source}:{outfit.Line}: missing outfit items {string.Join(", ", missing)}.");
			}
		}
		var craftCoverage = crafts.Select(x => x.ProductStableReference).Distinct(StringComparer.OrdinalIgnoreCase).Count() / (double)items.Count;
		var lifecycleCoverage = items.Count(x => !string.IsNullOrWhiteSpace(x.LifecycleKind)) / (double)items.Count;
		if (craftCoverage < 0.35 || lifecycleCoverage < 0.20)
		{
			throw new InvalidDataException($"Stage 2 graph coverage is too low: crafts {craftCoverage:P1}, lifecycle {lifecycleCoverage:P1}.");
		}
		foreach (var profile in new[] { "neutral", "northamerican", "continentaleuropean", "britishirish", "australasian", "japanese", "chinese" })
		{
			foreach (var dimension in new[] { "power", "paper", "telecommunications", "networkmedia", "vehicle" })
			{
				if (technology.Count(x => x.Profile == profile && x.Dimension == dimension) != 1)
				{
					throw new InvalidDataException($"Technology profile {profile} must define dimension {dimension} exactly once.");
				}
			}
		}
	}

	private static void RequireUnique<T>(IEnumerable<T> rows, Func<T, string> key, string label)
	{
		var duplicates = rows.GroupBy(key, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() > 1).Select(x => x.Key).Take(10).ToArray();
		if (duplicates.Length > 0)
		{
			throw new InvalidDataException($"Duplicate {label}: {string.Join(", ", duplicates)}.");
		}
	}

	private static IReadOnlyList<string> List(string text) => IndustrialisedCatalogueValues.List(text);
	private static string? Null(string text) => string.IsNullOrWhiteSpace(text) ? null : text;
	private static int Int(string text) => IndustrialisedCatalogueValues.Int(text);
	private static double Double(string text) => IndustrialisedCatalogueValues.Double(text);
	private static decimal Decimal(string text) => IndustrialisedCatalogueValues.Decimal(text);
	private static decimal? NullableDecimal(string text) => string.IsNullOrWhiteSpace(text) ? null : Decimal(text);
	private static bool Bool(string text) => bool.Parse(text);
}

internal sealed record IndustrialisedComponentTypeMetadata(
	string Name,
	IReadOnlyList<string> Capabilities,
	IReadOnlyList<string> ExclusiveTypes,
	IReadOnlyList<string> RequiredSiblingTypes,
	bool ContextDependentRequirements,
	bool PreventsManualLoad);

internal sealed record IndustrialisedComponentPrototypeMetadata(string Name, string Type);

internal sealed record IndustrialisedComponentMetadataDocument(
	IReadOnlyDictionary<string, IndustrialisedComponentTypeMetadata> Types,
	IReadOnlyDictionary<string, IndustrialisedComponentPrototypeMetadata> Prototypes);

internal static class IndustrialisedComponentMetadataCatalogue
{
	private static readonly Lazy<IndustrialisedComponentMetadataDocument> LazyDocument = new(Load);
	internal static IndustrialisedComponentMetadataDocument Document => LazyDocument.Value;

	private static IndustrialisedComponentMetadataDocument Load()
	{
		var assembly = typeof(IndustrialisedComponentMetadataCatalogue).Assembly;
		using var types = OpenJson(assembly, "Item_Component_Types.json");
		using var prototypes = OpenJson(assembly, "Seeded_Item_Components.json");
		var typeRows = types.RootElement.EnumerateArray().Select(x => new IndustrialisedComponentTypeMetadata(
			x.GetProperty("Component Type Name").GetString()!,
			Strings(x, "Component Capabilities"),
			Strings(x, "Exclusive Types"),
			Strings(x, "Required Sibling Types"),
			x.GetProperty("Has Context-Dependent Requirements").GetBoolean(),
			x.GetProperty("Prevents Manual Load").GetBoolean())).ToArray();
		var prototypeRows = prototypes.RootElement.EnumerateArray().Select(x => new IndustrialisedComponentPrototypeMetadata(
			x.GetProperty("Component Name").GetString()!,
			x.GetProperty("Component Type").GetString()!)).ToArray();
		return new IndustrialisedComponentMetadataDocument(
			typeRows.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase),
			prototypeRows.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase));
	}

	private static JsonDocument OpenJson(Assembly assembly, string suffix)
	{
		var resource = assembly.GetManifestResourceNames().SingleOrDefault(x => x.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) ??
			throw new InvalidDataException($"Embedded Stage 1 component metadata {suffix} was not found.");
		using var stream = assembly.GetManifestResourceStream(resource) ??
			throw new InvalidDataException($"Embedded Stage 1 component metadata {resource} could not be opened.");
		return JsonDocument.Parse(stream);
	}

	private static IReadOnlyList<string> Strings(JsonElement row, string property) => row.GetProperty(property)
		.EnumerateArray().Select(x => x.GetString()!).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
}
