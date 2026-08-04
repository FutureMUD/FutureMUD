#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DatabaseSeeder;

internal enum ItemSeederOwnershipPolicy
{
	StockAggregate,
	RequiredRelationship,
	BuilderTextProtected
}

internal enum ItemSeederManifestInspectionStatus
{
	Missing,
	Current,
	Repairable,
	Customized,
	Retired,
	Blocked
}

internal sealed record ItemSeederManifestEntry(
	string EntityType,
	string StableKey,
	string Module,
	IReadOnlyCollection<string> EraAdmissions,
	IReadOnlyCollection<string> Dependencies,
	ItemSeederOwnershipPolicy OwnershipPolicy,
	string Fingerprint);

internal sealed record ItemSeederManifestDocument(
	string ManifestVersion,
	string SourceFingerprint,
	DateTime GeneratedAtUtc,
	IReadOnlyCollection<ItemSeederManifestEntry> Entries);

internal sealed record ItemSeederManifestInspection(
	ItemSeederManifestEntry Entry,
	ItemSeederManifestInspectionStatus Status,
	string? Diagnostic = null);

internal sealed record ItemSeederReconciliationResult(
	string Module,
	int Inserted = 0,
	int Updated = 0,
	int Linked = 0,
	int Unchanged = 0,
	int Customized = 0,
	int Retired = 0,
	int Blocked = 0);

internal interface IItemSeederManifestModule
{
	string Key { get; }
	string DisplayName { get; }
	IReadOnlyCollection<string> Dependencies { get; }
	IReadOnlyCollection<string> EraAdmissions { get; }
	IReadOnlyCollection<ItemSeederManifestInspection> Inspect(
		IReadOnlyCollection<ItemSeederManifestEntry> entries);
	ItemSeederReconciliationResult Reconcile(
		IReadOnlyCollection<ItemSeederManifestInspection> inspections);
}

internal sealed class ItemSeederManifestModule(
	string key,
	string displayName,
	IReadOnlyCollection<string> dependencies,
	IReadOnlyCollection<string> eraAdmissions) : IItemSeederManifestModule
{
	public string Key { get; } = key;
	public string DisplayName { get; } = displayName;
	public IReadOnlyCollection<string> Dependencies { get; } = dependencies;
	public IReadOnlyCollection<string> EraAdmissions { get; } = eraAdmissions;

	public IReadOnlyCollection<ItemSeederManifestInspection> Inspect(
		IReadOnlyCollection<ItemSeederManifestEntry> entries)
	{
		return entries
			.Where(x => x.Module.Equals(Key, StringComparison.OrdinalIgnoreCase))
			.Select(x => new ItemSeederManifestInspection(x, ItemSeederManifestInspectionStatus.Current))
			.ToArray();
	}

	public ItemSeederReconciliationResult Reconcile(
		IReadOnlyCollection<ItemSeederManifestInspection> inspections)
	{
		return new ItemSeederReconciliationResult(
			Key,
			Unchanged: inspections.Count(x => x.Status == ItemSeederManifestInspectionStatus.Current),
			Customized: inspections.Count(x => x.Status == ItemSeederManifestInspectionStatus.Customized),
			Retired: inspections.Count(x => x.Status == ItemSeederManifestInspectionStatus.Retired),
			Blocked: inspections.Count(x => x.Status == ItemSeederManifestInspectionStatus.Blocked));
	}
}

internal static class ItemSeederManifestCatalogue
{
	public const string ManifestVersion = "1";
	public const string DefaultRelativePath = "Design Documents/Seeding/Seeded_Item_Manifest.json";

	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = true,
		Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
	};

	public static IReadOnlyCollection<IItemSeederManifestModule> Modules { get; } =
	[
		new ItemSeederManifestModule("foundations", "Foundations", [], []),
		new ItemSeederManifestModule("shared-preindustrial", "Shared Pre-Industrial Content", ["foundations"],
			["antiquity", "medieval", "renaissance", "earlymodern"]),
		new ItemSeederManifestModule("antiquity", "Antiquity", ["shared-preindustrial"], ["antiquity"]),
		new ItemSeederManifestModule("medieval", "Medieval", ["shared-preindustrial"], ["medieval"]),
		new ItemSeederManifestModule("renaissance", "Renaissance", ["shared-preindustrial"], ["renaissance"]),
		new ItemSeederManifestModule("earlymodern", "Early Modern", ["shared-preindustrial"], ["earlymodern"]),
		new ItemSeederManifestModule("lifecycle", "Lifecycle Links", ["shared-preindustrial"],
			["antiquity", "medieval", "renaissance", "earlymodern"]),
		new ItemSeederManifestModule("outfits", "Outfits", ["antiquity", "medieval", "renaissance", "earlymodern"],
			["antiquity", "medieval", "renaissance", "earlymodern"]),
		new ItemSeederManifestModule("crafts", "Crafts", ["foundations", "shared-preindustrial"],
			["antiquity", "medieval", "renaissance", "earlymodern"]),
		new ItemSeederManifestModule("vehicles", "Vehicles", ["foundations"],
			["antiquity", "medieval", "renaissance", "earlymodern"])
	];

	public static string Fingerprint(object? value)
	{
		var json = JsonSerializer.Serialize(value, JsonOptions);
		return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant();
	}

	public static ItemSeederManifestDocument BuildDocument(
		IEnumerable<ItemSeederManifestEntry> entries,
		string sourceFingerprint,
		DateTime? generatedAtUtc = null)
	{
		var ordered = entries
			.OrderBy(x => x.Module, StringComparer.Ordinal)
			.ThenBy(x => x.EntityType, StringComparer.Ordinal)
			.ThenBy(x => x.StableKey, StringComparer.Ordinal)
			.ToArray();
		Validate(ordered);
		return new ItemSeederManifestDocument(
			ManifestVersion,
			sourceFingerprint,
			generatedAtUtc ?? DateTime.UtcNow,
			ordered);
	}

	public static void Validate(IReadOnlyCollection<ItemSeederManifestEntry> entries)
	{
		var moduleKeys = Modules.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var duplicateKeys = entries
			.GroupBy(x => $"{x.EntityType}\u001f{x.StableKey}", StringComparer.OrdinalIgnoreCase)
			.Where(x => x.Count() > 1)
			.Select(x => x.Key.Replace('\u001f', ':'))
			.ToArray();
		if (duplicateKeys.Length > 0)
		{
			throw new InvalidDataException(
				$"The ItemSeeder manifest contains duplicate stable identities: {string.Join(", ", duplicateKeys)}.");
		}

		var unknownModules = entries
			.Where(x => !moduleKeys.Contains(x.Module))
			.Select(x => x.Module)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
		if (unknownModules.Length > 0)
		{
			throw new InvalidDataException(
				$"The ItemSeeder manifest contains unknown modules: {string.Join(", ", unknownModules)}.");
		}

		var entryKeys = entries
			.Select(x => $"{x.EntityType}:{x.StableKey}")
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var unresolvedDependencies = entries
			.SelectMany(entry => entry.Dependencies.Select(dependency => (entry, dependency)))
			.Where(x => !moduleKeys.Contains(x.dependency) && !entryKeys.Contains(x.dependency))
			.Select(x => $"{x.entry.EntityType}:{x.entry.StableKey} -> {x.dependency}")
			.ToArray();
		if (unresolvedDependencies.Length > 0)
		{
			throw new InvalidDataException(
				$"The ItemSeeder manifest contains unresolved dependencies: {string.Join(", ", unresolvedDependencies)}.");
		}
	}

	public static string Serialize(ItemSeederManifestDocument document)
	{
		return JsonSerializer.Serialize(document, JsonOptions) + Environment.NewLine;
	}

	public static ItemSeederManifestDocument Deserialize(string json)
	{
		return JsonSerializer.Deserialize<ItemSeederManifestDocument>(json, JsonOptions) ??
		       throw new InvalidDataException("The ItemSeeder manifest is empty or invalid.");
	}

	public static ItemSeederManifestDocument Load(string path)
	{
		var document = Deserialize(File.ReadAllText(path));
		Validate(document.Entries);
		return document;
	}

	public static ItemSeederManifestDocument LoadForRuntime()
	{
		var packagedPath = Path.Combine(AppContext.BaseDirectory, "Seeded_Item_Manifest.json");
		if (File.Exists(packagedPath))
		{
			return Load(packagedPath);
		}

		var repositoryRoot = FindRepositoryRoot();
		var canonicalPath = Path.Combine(repositoryRoot,
			DefaultRelativePath.Replace('/', Path.DirectorySeparatorChar));
		var document = Load(canonicalPath);
		var sourceFingerprint = ComputeSourceFingerprint(repositoryRoot);
		if (!document.SourceFingerprint.Equals(sourceFingerprint, StringComparison.OrdinalIgnoreCase))
		{
			throw new InvalidDataException(
				$"Seeded_Item_Manifest.json is stale: recorded source {document.SourceFingerprint}, current source {sourceFingerprint}.");
		}

		return document;
	}

	public static string FindRepositoryRoot(string? startingPath = null)
	{
		var directory = new DirectoryInfo(startingPath ?? Directory.GetCurrentDirectory());
		while (directory is not null)
		{
			if (Directory.Exists(Path.Combine(directory.FullName, "DatabaseSeeder")) &&
			    Directory.Exists(Path.Combine(directory.FullName, "Design Documents")))
			{
				return directory.FullName;
			}

			directory = directory.Parent;
		}

		throw new DirectoryNotFoundException(
			"Could not find the FutureMUD repository root containing DatabaseSeeder and Design Documents.");
	}

	public static string ComputeSourceFingerprint(string repositoryRoot)
	{
		var sourceDirectory = Path.Combine(repositoryRoot, "DatabaseSeeder", "Seeders");
		using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
		foreach (var path in Directory
		         .EnumerateFiles(sourceDirectory, "ItemSeeder*.cs", SearchOption.TopDirectoryOnly)
		         .OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
		{
			var relativePath = Path.GetRelativePath(repositoryRoot, path).Replace('\\', '/');
			hash.AppendData(Encoding.UTF8.GetBytes(relativePath));
			hash.AppendData([0]);
			hash.AppendData(File.ReadAllBytes(path));
			hash.AppendData([0]);
		}

		return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
	}

	public static bool TryHandleCommand(string[] args)
	{
		var exportIndex = Array.FindIndex(args,
			x => x.Equals("--export-item-manifest", StringComparison.OrdinalIgnoreCase));
		var check = args.Any(x => x.Equals("--check-item-manifest", StringComparison.OrdinalIgnoreCase));
		if (exportIndex < 0 && !check)
		{
			return false;
		}

		var repositoryRoot = FindRepositoryRoot();
		var canonicalPath = Path.Combine(repositoryRoot, DefaultRelativePath.Replace('/', Path.DirectorySeparatorChar));
		var document = Load(canonicalPath);
		var currentSourceFingerprint = ComputeSourceFingerprint(repositoryRoot);
		if (!document.SourceFingerprint.Equals(currentSourceFingerprint, StringComparison.OrdinalIgnoreCase))
		{
			throw new InvalidDataException(
				$"Seeded_Item_Manifest.json is stale: recorded source {document.SourceFingerprint}, current source {currentSourceFingerprint}.");
		}

		if (check)
		{
			Console.WriteLine(
				$"ItemSeeder manifest is current ({document.Entries.Count:N0} aggregates, source {currentSourceFingerprint}).");
			return true;
		}

		var outputPath = exportIndex + 1 < args.Length && !args[exportIndex + 1].StartsWith("--", StringComparison.Ordinal)
			? Path.GetFullPath(args[exportIndex + 1])
			: canonicalPath;
		Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? repositoryRoot);
		File.WriteAllText(outputPath, Serialize(document));
		Console.WriteLine($"Exported {document.Entries.Count:N0} ItemSeeder aggregates to {outputPath}.");
		return true;
	}
}
