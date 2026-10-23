#nullable enable

using MudSharp.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DatabaseSeeder.Seeders;

internal sealed record CapturedCatalogueDefinition(
	ItemSeederManifestEntry Identity,
	string DefinitionType,
	JsonElement Definition,
	JsonElement Supplement,
	string SourceFile,
	int SourceLine);

internal sealed record CapturedCatalogueDocument(
	string Contract,
	string Eras,
	string TechnologyProfile,
	IReadOnlyList<CapturedCatalogueDefinition> Definitions);

internal sealed class CatalogueDefinitionCapture
{
	internal static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new JsonStringEnumConverter() }
	};
	private readonly Dictionary<string, CapturedCatalogueDefinition> _definitions = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<string, JsonElement> _supplements = new(StringComparer.OrdinalIgnoreCase);
	private static string Key(string type, string key) => $"{type}\u001f{key.Trim()}";

	internal void Supplement(string type, string key, object value) =>
		_supplements.TryAdd(Key(type, key), JsonSerializer.SerializeToElement(value, JsonOptions));

	internal void Register(ItemSeederManifestEntry identity, object definition, string sourceFile, int sourceLine)
	{
		var key = Key(identity.EntityType, identity.StableKey);
		_definitions.TryAdd(key, new CapturedCatalogueDefinition(identity, definition.GetType().Name,
			JsonSerializer.SerializeToElement(definition, definition.GetType(), JsonOptions),
			_supplements.GetValueOrDefault(key, JsonSerializer.SerializeToElement(new { })),
			sourceFile.Replace('\\', '/').Split("/DatabaseSeeder/").Last(), sourceLine));
	}

	internal CapturedCatalogueDocument Finish(string eras, string profile)
	{
		// Keep the original definition too. The symbolic projection is used by the comparator, never as a destructive rewrite.
		return new("1", eras, profile, _definitions.Values
			.OrderBy(x => x.Identity.EntityType, StringComparer.Ordinal)
			.ThenBy(x => x.Identity.StableKey, StringComparer.Ordinal)
			.ToArray());
	}

	internal static JsonElement SymbolicDefinition(CapturedCatalogueDefinition entry, IReadOnlyDictionary<long, string> itemKeys)
	{
		var node = JsonNode.Parse(entry.Definition.GetRawText())!;
		void Visit(JsonNode current)
		{
			if (current is JsonObject obj)
			{
				foreach (var pair in obj.ToArray())
				{
					if (pair.Value is JsonValue value && pair.Key is "importText" or "details" && value.TryGetValue<string>(out var text))
						obj[pair.Key] = Regex.Replace(text, @"\(#(?<id>\d+)\)", match =>
							itemKeys.TryGetValue(long.Parse(match.Groups["id"].Value, System.Globalization.CultureInfo.InvariantCulture), out var key)
								? $"(@{key})" : throw new InvalidDataException($"Unresolved captured item reference in {entry.Identity.StableKey}: {match.Value}"));
					else if (pair.Value is not null) Visit(pair.Value);
				}
			}
			else if (current is JsonArray array) foreach (var value in array) if (value is not null) Visit(value);
		}
		Visit(node);
		return JsonSerializer.SerializeToElement(node, JsonOptions);
	}

	internal static IReadOnlyList<string> FieldPaths(JsonElement value)
	{
		var paths = new SortedSet<string>(StringComparer.Ordinal);
		void Visit(JsonElement element, string path)
		{
			if (element.ValueKind == JsonValueKind.Object)
				foreach (var property in element.EnumerateObject()) Visit(property.Value, $"{path}.{property.Name}");
			else if (element.ValueKind == JsonValueKind.Array)
			{
				paths.Add(path + "[]");
				foreach (var child in element.EnumerateArray()) Visit(child, path + "[]");
			}
			else paths.Add(path);
		}
		Visit(value, "$");
		return paths.ToArray();
	}
}

internal sealed record CatalogueSourceFile(string Path, long Bytes, string Sha256);

internal sealed record CatalogueAuthoringItem(string Source, int Line, string Member, int? MaterialType, CatalogueItemDefinition Definition);

internal static class CatalogueSourceInventory
{
	internal static IReadOnlyList<string> Paths(string root)
	{
		var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{ ".cs", ".csproj", ".targets", ".tsv", ".csv", ".json", ".md", ".py", ".ps1", ".sh" };
		return new[] { "DatabaseSeeder", "scripts", "Design Documents/Seeding", "Design Documents/Data",
			"Design Documents/Crafting", "Design Documents/Items" }
			.Where(folder => Directory.Exists(Path.Combine(root, folder)))
			.SelectMany(folder => Directory.EnumerateFiles(Path.Combine(root, folder), "*", SearchOption.AllDirectories))
			.Where(path => !path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
				.Any(segment => segment is "bin" or "obj" or ".artifacts" or "__pycache__"))
			.Where(path => extensions.Contains(Path.GetExtension(path)))
			.Select(path => Path.GetRelativePath(root, path).Replace('\\', '/'))
			.OrderBy(path => path, StringComparer.Ordinal)
			.ToArray();
	}

	internal static IReadOnlyList<CatalogueSourceFile> Archive(string root, string destination)
	{
		var files = new List<CatalogueSourceFile>();
		using var archive = ZipFile.Open(destination, ZipArchiveMode.Create);
		foreach (var path in Paths(root))
		{
			var bytes = File.ReadAllBytes(Path.Combine(root, path));
			var entry = archive.CreateEntry(path, CompressionLevel.Optimal);
			entry.LastWriteTime = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
			using (var stream = entry.Open()) stream.Write(bytes);
			files.Add(new(path, bytes.LongLength, Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant()));
		}
		return files;
	}
}

public partial class ItemSeeder
{
	private CatalogueDefinitionCapture? _definitionCapture;
	internal Action<CatalogueAuthoringItem>? CatalogueAuthoringTrace { get; set; }
	internal Action<object>? CatalogueCraftAuthoringTrace { get; set; }
	private readonly Dictionary<string, object> _capturedCraftGates = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<string, object> _capturedRecipeGates = new(StringComparer.Ordinal);
	internal IReadOnlyDictionary<long, string> CapturedItemKeys => _itemStableReferencesById;
	internal IReadOnlyDictionary<string, object> CapturedCraftGates => _capturedCraftGates;
	internal IReadOnlyDictionary<string, object> CapturedRecipeGates => _capturedRecipeGates;

	internal static void PrepareSyntheticCapturePrerequisites(FuturemudDatabaseContext context, string eras)
	{
		// A real database must supply its own Core prerequisites. Never flush its pending changes here.
		if (!HasAnyEra(eras, "antiquity") || HasAnyEra(eras, "medieval", "renaissance", "earlymodern") ||
			!context.Database.IsInMemory() || context.Tags.Any(x => x.Name == "Meat")) return;
		context.Tags.Add(new MudSharp.Models.Tag { Name = "Meat" });
		context.SaveChanges();
	}

	internal CapturedCatalogueDocument CaptureFullCatalogue(FuturemudDatabaseContext context, string root,
		string eras = "antiquity medieval renaissance earlymodern industrial", string profile = "neutral")
	{
		var selection = ValidateEraSelection(eras);
		if (!selection.Success) throw new ArgumentException(selection.Error, nameof(eras));
		var technology = ValidateTechnologyProfile(profile);
		if (!technology.Success) throw new ArgumentException(technology.Error, nameof(profile));
		var capture = new CatalogueDefinitionCapture();
		var previousTrace = CatalogueCraftAuthoringTrace;
		CatalogueCraftAuthoringTrace ??= _ => { };
		_definitionCapture = capture;
		try
		{
			CaptureManifest(context, root, eras, new Dictionary<string, string> { ["technologyprofile"] = profile });
			return capture.Finish(eras, profile);
		}
		finally { _definitionCapture = null; CatalogueCraftAuthoringTrace = previousTrace; }
	}

	internal object CaptureSourceGraphs() => new
	{
		Industrialised = IndustrialisedCatalogue,
		EraChoices = EraDefinitions,
		TechnologyChoices = TechnologyProfiles,
		SharedAntiquityAliases = PreIndustrialAntiquityAliasStableReferences,
		ApprovedHistoricalClothingReuse = ApprovedHistoricalClothingReuseReferences.OrderBy(x => x, StringComparer.Ordinal).ToArray(),
		FoodItems = PreIndustrialFoodCatalogue.Items,
		FoodLiquids = PreIndustrialFoodCatalogue.Liquids,
		RenaissanceMedical = EraMedicalRepairCatalogue.Entries
			.Where(item => item.StableReference.StartsWith("renaissance_", StringComparison.Ordinal)).ToArray(),
		EarlyModernMedical = EraMedicalRepairCatalogue.Entries
			.Where(item => item.StableReference.StartsWith("earlymodern_", StringComparison.Ordinal)).ToArray(),
		Vehicles = VehicleExampleSpecs,
		AntiquityOutfits = AntiquityOutfitManifestSpecs,
		MedievalOutfits = MedievalOutfitManifestSpecs,
		RenaissanceOutfits = RenaissanceOutfitManifestSpecs,
		EarlyModernOutfits = EarlyModernOutfitManifestSpecs,
		ProductionPlans = new
		{
			MedievalProduction = MedievalProductionCraftSpecsForTesting,
			MedievalFood = MedievalFoodCraftSpecsForTesting,
			PrimaryProduction = PrimaryProductionCraftSpecsForTesting,
			BlackPowder = BlackPowderCraftSpecsForTesting,
			RenaissanceToolProviders = RenaissanceCraftToolRequirementsForTesting,
			FoodCatalogue = PreIndustrialFoodCatalogueCraftSpecsForTesting
		}
	};
}
