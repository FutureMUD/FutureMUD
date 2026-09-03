#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DatabaseSeeder.Seeders;

namespace DatabaseSeeder;

/// <summary>
/// Joins every approved garment to its proposed physical dependencies and source-owned stock names.
/// This is a planning audit, not an installer or certification of component XML, fit or finished prose.
/// </summary>
internal static class IndustrialisedClothingDependencyAudit
{
	internal const string RelativePath = "Design Documents/Seeding/Industrialised_Clothing_Dependency_Plan.tsv";
	internal const string InventoryPath = "Design Documents/Seeding/Industrialised_Clothing_Wave1_Inventory.md";
	internal const string OutfitsPath = "Design Documents/Seeding/Industrialised_Clothing_Wave1_Outfits.md";
	internal const string MaterialsPath = "Design Documents/Data/Seeded_Materials.json";
	internal const string ComponentsPath = "Design Documents/Data/Seeded_Item_Components.json";
	internal const string TagsPath = "Design Documents/Data/SeededTagHierarchy.csv";
	internal static readonly IReadOnlyList<string> InputPaths = Array.AsReadOnly(new[]
		{ InventoryPath, OutfitsPath, MaterialsPath, ComponentsPath, TagsPath });
	internal const string Header = "PlanningKey\tItemReference\tFamily\tEraAdmissions\tProductionRoute\tReused\tInventoryFile\tInventoryLine\tBindingSourceFile\tBindingSourceLine\tBindingSourceKind\tMaterial\tCandidateComponents\tRequiredTags\tRequestedWearProfile\tRequiredLayerWeight\tMissingStockNames\tComponentCapabilities\tComponentCompositionIssues\tOutfitReferences\tAdditionalSkinBriefs\tOpenRequirements\tValidation\tDependencySourceSha256";
	private static readonly string[] InventoryColumns = ["Key", "Family", "Eras", "Route", "Source",
		"Physical/economic design and reason for a separate base", "Skin briefs", "Evidence"];
	private static readonly string[] OutfitColumns = ["Key", "Eras", "Garments", "Palette", "Coverage purpose"];
	private sealed record PlanningRow(int Line, string[] Fields);

	internal static string Generate(string repositoryRoot) => Generate(repositoryRoot, IndustrialisedClothingDependencyPlan.Rows);

	internal static string Generate(string repositoryRoot, IReadOnlyList<ClothingDependencyPlanRow> plan)
	{
		var sources = InputPaths.ToDictionary(x => x, x => Normalize(File.ReadAllText(Path.Combine(repositoryRoot, x))), StringComparer.Ordinal);
		var inventory = ReadTable(sources[InventoryPath], InventoryPath, InventoryColumns);
		var outfits = ReadTable(sources[OutfitsPath], OutfitsPath, OutfitColumns);
		var bindings = Unique(plan, x => x.ItemReference, "dependency-plan item");
		var approved = Unique(inventory, x => ItemReference(x), "approved item");
		Unique(inventory, x => x.Fields[0], "approved planning key");
		Unique(outfits, x => x.Fields[0], "outfit key");
		var missing = approved.Keys.Except(bindings.Keys, StringComparer.Ordinal).ToArray();
		var extra = bindings.Keys.Except(approved.Keys, StringComparer.Ordinal).ToArray();
		if (missing.Length > 0 || extra.Length > 0)
			throw new InvalidDataException($"Dependency-plan coverage differs from {InventoryPath}: missing [{string.Join(", ", missing)}]; unapproved [{string.Join(", ", extra)}].");
		foreach (var row in plan)
		{
			if (row.RequiredLayerWeight is { } weight && (!double.IsFinite(weight) || weight < 0))
				throw row.Source.Error("Requested layer consumption must be finite and nonnegative.");
			Unique(row.Components, x => x, $"component in {row.ItemReference}", StringComparer.OrdinalIgnoreCase);
			Unique(row.Tags, x => x, $"tag in {row.ItemReference}", StringComparer.OrdinalIgnoreCase);
		}

		var materials = ReadNames(sources[MaterialsPath], MaterialsPath, "Material Name");
		var components = ReadComponents(sources[ComponentsPath]);
		var tags = ReadTags(sources[TagsPath]);
		var byPlanningKey = inventory.ToDictionary(x => x.Fields[0], StringComparer.Ordinal);
		var outfitReferences = new Dictionary<string, List<string>>(StringComparer.Ordinal);
		foreach (var outfit in outfits)
		{
			var admissions = Admissions(outfit.Fields[1], OutfitsPath, outfit.Line);
			foreach (var entry in IndustrialisedCatalogueValues.List(outfit.Fields[2]))
			{
				var parts = entry.Split('@');
				var key = parts[0];
				if (!byPlanningKey.TryGetValue(key, out var garment))
					throw new InvalidDataException($"{OutfitsPath}:{outfit.Line}: unknown garment {key}.");
				var skins = garment.Fields[6] == "-" ? [] : garment.Fields[6].Split(';', StringSplitOptions.TrimEntries)
					.Select(x => Regex.Replace(x.ToLowerInvariant(), "[^a-z0-9]+", "-", RegexOptions.CultureInvariant).Trim('-')).ToArray();
				if (parts.Length > 2 || parts.Length == 2 && !skins.Contains(parts[1], StringComparer.Ordinal))
					throw new InvalidDataException($"{OutfitsPath}:{outfit.Line}: unknown skin selection {entry}.");
				if (admissions.Except(Admissions(garment.Fields[2], InventoryPath, garment.Line), StringComparer.Ordinal).Any())
					throw new InvalidDataException($"{OutfitsPath}:{outfit.Line}: incompatible garment admissions for {key}.");
				if (!outfitReferences.TryGetValue(key, out var references)) outfitReferences.Add(key, references = []);
				references.Add(outfit.Fields[0]);
			}
		}

		using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
		foreach (var (path, content) in sources.OrderBy(x => x.Key, StringComparer.Ordinal))
		{
			hash.AppendData(Encoding.UTF8.GetBytes(path + "\0" + content + "\0"));
		}
		hash.AppendData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(plan.OrderBy(x => x.ItemReference, StringComparer.Ordinal))));
		hash.AppendData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(IndustrialisedComponentMetadataCatalogue.Document.Types
			.OrderBy(x => x.Key, StringComparer.Ordinal))));
		var fingerprint = Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
		var output = new StringBuilder(Header).Append('\n');
		foreach (var (reference, garment) in approved.OrderBy(x => x.Key, StringComparer.Ordinal))
		{
			var row = bindings[reference];
			var admissions = Admissions(garment.Fields[2], InventoryPath, garment.Line);
			if (!admissions.SequenceEqual(row.EraAdmissions, StringComparer.Ordinal) || row.Reused != (garment.Fields[4] != "new") ||
				(!row.Reused && row.PlanningKey != garment.Fields[0]))
				throw row.Source.Error($"Dependency-plan identity/admissions differ from {InventoryPath}:{garment.Line} for {reference}.");
			var absent = new List<string>();
			if (!materials.Contains(row.Material)) absent.Add($"material:{row.Material}");
			absent.AddRange(row.Components.Where(x => !components.ContainsKey(x)).Select(x => $"component:{x}"));
			absent.AddRange(row.Tags.Where(x => !tags.Contains(x)).Select(x => $"tag:{x}"));
			var types = row.Components.Where(components.ContainsKey).Select(x => components[x]).ToArray();
			var capabilities = types.SelectMany(x => x.Capabilities).ToHashSet(StringComparer.Ordinal);
			var issues = new List<string>();
			foreach (var exclusive in types.SelectMany(x => x.ExclusiveTypes).Distinct(StringComparer.Ordinal))
				if (types.Count(x => x.Capabilities.Contains(exclusive, StringComparer.Ordinal)) > 1)
					issues.Add($"duplicate-exclusive:{exclusive}");
			foreach (var requirement in types.SelectMany(x => x.RequiredSiblingTypes).Distinct(StringComparer.Ordinal).Where(x => !capabilities.Contains(x)))
				issues.Add($"missing-sibling:{requirement}");
			foreach (var required in new[] { "IHoldable", "IWearable", "IVariable" }.Where(x => !capabilities.Contains(x)))
				issues.Add($"missing-garment-capability:{required}");
			foreach (var type in types.Where(x => x.ContextDependentRequirements)) issues.Add($"context-validation:{type.Name}");
			var cells = new[]
			{
				garment.Fields[0], reference, garment.Fields[1], string.Join(';', admissions), garment.Fields[3], row.Reused ? "true" : "false",
				InventoryPath, garment.Line.ToString(CultureInfo.InvariantCulture), row.Source.File, row.Source.Line.ToString(CultureInfo.InvariantCulture),
				row.Reused ? "historical-source-provider" : "authored-dependency-decision", row.Material,
				JsonSerializer.Serialize(row.Components), JsonSerializer.Serialize(row.Tags), row.WearProfile ?? "",
				row.RequiredLayerWeight?.ToString(CultureInfo.InvariantCulture) ?? "", JsonSerializer.Serialize(absent.OrderBy(x => x, StringComparer.Ordinal)),
				JsonSerializer.Serialize(capabilities.OrderBy(x => x, StringComparer.Ordinal)), JsonSerializer.Serialize(issues.OrderBy(x => x, StringComparer.Ordinal)),
				JsonSerializer.Serialize(outfitReferences.GetValueOrDefault(garment.Fields[0], []).Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal)),
				JsonSerializer.Serialize(garment.Fields[6] == "-" ? Array.Empty<string>() : garment.Fields[6].Split(';', StringSplitOptions.TrimEntries)),
				JsonSerializer.Serialize(row.OpenRequirements), "scope-reconciled;stock-names-audited;physical-unverified;production-unreviewed", fingerprint
			};
			if (cells.Any(x => x.IndexOfAny(['\t', '\r', '\n']) >= 0)) throw row.Source.Error("Invalid delimiter in dependency audit field.");
			output.AppendJoin('\t', cells).Append('\n');
		}
		return output.ToString();
	}

	private static Dictionary<string, T> Unique<T>(IEnumerable<T> rows, Func<T, string> key, string label, StringComparer? comparer = null)
	{
		var result = new Dictionary<string, T>(comparer ?? StringComparer.Ordinal);
		foreach (var row in rows)
			if (!result.TryAdd(key(row), row)) throw new InvalidDataException($"Duplicate {label}: {key(row)}.");
		return result;
	}

	private static PlanningRow[] ReadTable(string text, string path, string[] columns)
	{
		var rows = new List<PlanningRow>();
		var header = false;
		var lines = text.Split('\n');
		for (var index = 0; index < lines.Length; index++)
		{
			var line = lines[index];
			if (!line.StartsWith('|')) continue;
			if (Regex.IsMatch(line, @"^\|[- :|]+$", RegexOptions.CultureInvariant)) continue;
			var cells = line.Trim('|').Split('|', StringSplitOptions.TrimEntries);
			if (cells.SequenceEqual(columns, StringComparer.Ordinal)) { header = true; continue; }
			if (!header || cells.Length != columns.Length || !Regex.IsMatch(cells[0], "^[a-z][a-z0-9_]*$", RegexOptions.CultureInvariant))
				throw new InvalidDataException($"{path}:{index + 1}: invalid approved planning table row or header.");
			rows.Add(new(index + 1, cells));
		}
		if (!header || rows.Count == 0) throw new InvalidDataException($"{path}: approved planning table is missing or empty.");
		return rows.ToArray();
	}

	private static string[] Admissions(string letters, string path, int line)
	{
		if (!Regex.IsMatch(letters, "^(?=.)I?M?N?F?$", RegexOptions.CultureInvariant))
			throw new InvalidDataException($"{path}:{line}: invalid era admissions {letters}.");
		return letters.Select(x => x switch { 'I' => "industrial", 'M' => "modern", 'N' => "nuclear", 'F' => "information", _ => throw new InvalidOperationException() }).ToArray();
	}

	private static string ItemReference(PlanningRow row)
	{
		var admissions = Admissions(row.Fields[2], InventoryPath, row.Line);
		return row.Fields[4] == "new" ? $"{(admissions.Length == 1 ? admissions[0] : "industrialised")}_clothing_{row.Fields[0]}" : row.Fields[4];
	}

	private static HashSet<string> ReadNames(string text, string path, string property)
	{
		using var document = ParseArray(text, path);
		return Unique(document.RootElement.EnumerateArray().Select(x => RequiredText(x, property, path)), x => x, path, StringComparer.OrdinalIgnoreCase).Keys.ToHashSet(StringComparer.Ordinal);
	}

	private static Dictionary<string, IndustrialisedComponentTypeMetadata> ReadComponents(string text)
	{
		using var document = ParseArray(text, ComponentsPath);
		return Unique(document.RootElement.EnumerateArray(), x => RequiredText(x, "Component Name", ComponentsPath), ComponentsPath, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x =>
			{
				var name = RequiredText(x.Value, "Component Type", ComponentsPath);
				if (!IndustrialisedComponentMetadataCatalogue.Document.Types.TryGetValue(name, out var type) || type.Name != name)
					throw new InvalidDataException($"{ComponentsPath}: unknown or noncanonical component type {name} on {x.Key}.");
				return type;
			}, StringComparer.Ordinal);
	}

	private static JsonDocument ParseArray(string text, string path)
	{
		JsonDocument document;
		try { document = JsonDocument.Parse(text); }
		catch (JsonException ex) { throw new InvalidDataException($"{path}: invalid JSON: {ex.Message}", ex); }
		if (document.RootElement.ValueKind == JsonValueKind.Array) return document;
		document.Dispose();
		throw new InvalidDataException($"{path}: expected a JSON array.");
	}

	private static string RequiredText(JsonElement row, string property, string path)
	{
		if (row.ValueKind != JsonValueKind.Object || !row.TryGetProperty(property, out var value) ||
			value.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(value.GetString()))
			throw new InvalidDataException($"{path}: missing or invalid {property}.");
		var text = value.GetString()!;
		if (text != text.Trim()) throw new InvalidDataException($"{path}: padded {property}: {text}.");
		return text;
	}

	private static HashSet<string> ReadTags(string text)
	{
		var rows = new IndustrialisedCatalogueSource(TagsPath, text).Read(["Tag name", "Tag parent name", "Tag hierarchy"], (_, _, fields) => fields[2]);
		return Unique(rows, x => x, TagsPath, StringComparer.OrdinalIgnoreCase).Keys.ToHashSet(StringComparer.Ordinal);
	}

	private static string Normalize(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').TrimStart('\uFEFF');
}
