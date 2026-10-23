#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DatabaseSeeder.Seeders;

internal static class CatalogueCaptureCommand
{
	internal static bool TryHandle(string[] args)
	{
		if (args.Contains("--benchmark-item-inventory"))
		{
			var allocated = GC.GetTotalAllocatedBytes(true);
			var timer = Stopwatch.StartNew();
			var inventory = ItemSeederManifestCatalogue.LoadForRuntime();
			timer.Stop();
			allocated = GC.GetTotalAllocatedBytes(true) - allocated;
			Console.WriteLine(JsonSerializer.Serialize(new
			{
				Aggregates = inventory.Entries.Count, ColdMilliseconds = timer.Elapsed.TotalMilliseconds,
				AllocatedBytes = allocated,
				Cached = ReferenceEquals(inventory, ItemSeederManifestCatalogue.LoadForRuntime()),
				Counts = inventory.Entries.GroupBy(entry => entry.EntityType).ToDictionary(group => group.Key, group => group.Count())
			}));
			return true;
		}
		if (args.Contains("--benchmark-compiled-item-stock"))
		{
			if (args.Contains("--legacy")) throw new ArgumentException("Use the frozen baseline executable for legacy measurements.");
			CompiledStockCatalogue.Benchmark();
			return true;
		}
		if (!args.Contains("--capture-item-catalogue") && !args.Contains("--capture-item-authoring") && !args.Contains("--benchmark-item-catalogue") && !args.Contains("--compare-item-catalogue")) return false;
		string Option(string key, string fallback)
		{
			var index = Array.IndexOf(args, key);
			if (index < 0) return fallback;
			if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
				throw new ArgumentException($"{key} requires a value.");
			return args[index + 1];
		}
		var output = Path.GetFullPath(Option("--output", ".artifacts/item-catalogue/" + Guid.NewGuid().ToString("N")));
		var root = ItemSeederManifestCatalogue.FindRepositoryRoot();
		var eras = Option("--eras", "antiquity medieval renaissance earlymodern industrial");
		var profile = Option("--technology-profile", "neutral");
		const bool unified = true;
		if (args.Contains("--legacy")) throw new ArgumentException("Legacy authoring was removed. Use the separately frozen baseline executable for historical comparisons.");
		if (Directory.Exists(output) && Directory.EnumerateFileSystemEntries(output).Any())
			throw new IOException("Catalogue output requires a new or empty directory; previous evidence will not be overwritten.");
		Directory.CreateDirectory(output);
		if (args.Contains("--compare-item-catalogue"))
		{
			var differences = Compare(Option("--before", ""), Option("--after", ""));
			WriteJson(Path.Combine(output, "comparison.json"), new { Passed = differences.Count == 0, Differences = differences });
			if (differences.Count > 0) throw new InvalidDataException($"Catalogue definitions differ in {differences.Count} fields/identities; see comparison.json.");
			Console.WriteLine("Full definition comparison passed.");
			return true;
		}
		if (args.Contains("--benchmark-item-catalogue"))
		{
			Benchmark(output, root, eras, profile, unified);
			return true;
		}
		using var context = Program.CreateItemManifestCaptureContext();
		var seeder = new ItemSeeder();
		var authoringItems = args.Contains("--capture-item-authoring") ? new List<CatalogueAuthoringItem>() : null;
		if (authoringItems is not null) seeder.CatalogueAuthoringTrace = authoringItems.Add;
		var authoringCrafts = args.Contains("--capture-item-authoring") ? new List<object>() : null;
		if (authoringCrafts is not null) seeder.CatalogueCraftAuthoringTrace = authoringCrafts.Add;
		var initialFingerprint = ItemSeederManifestCatalogue.ComputeSourceFingerprint(root);
		var document = seeder.CaptureFullCatalogue(context, root, eras, profile);
		if (authoringItems is not null) WriteJson(Path.Combine(output, "authoring-items.json"), authoringItems);
		if (authoringCrafts is not null) WriteJson(Path.Combine(output, "authoring-crafts.json"), authoringCrafts);
		using (var definitions = File.CreateText(Path.Combine(output, "definitions.ndjson")))
		using (var ledger = File.CreateText(Path.Combine(output, "field-ledger.ndjson")))
		{
			foreach (var definition in document.Definitions)
			{
				WriteLine(definitions, new
				{
					definition.Identity, definition.DefinitionType, definition.Definition, definition.Supplement, definition.SourceFile, definition.SourceLine,
					SymbolicDefinition = CatalogueDefinitionCapture.SymbolicDefinition(definition, seeder.CapturedItemKeys)
				});
				WriteLine(ledger, new
				{
					definition.Identity.EntityType, definition.Identity.StableKey, definition.DefinitionType,
					definition.SourceFile, definition.SourceLine,
					DefinitionFields = CatalogueDefinitionCapture.FieldPaths(definition.Definition),
					SupplementFields = CatalogueDefinitionCapture.FieldPaths(definition.Supplement),
					Status = "captured", Target = RepresentativeCatalogue.TargetFor(definition.Identity.StableKey)
				});
			}
		}
		WriteJson(Path.Combine(output, "source-graphs.json"), seeder.CaptureSourceGraphs());
		WriteJson(Path.Combine(output, "item-references.json"), seeder.CapturedItemKeys);
		var sourceFiles = CatalogueSourceInventory.Archive(root, Path.Combine(output, "sources.zip"));
		if (initialFingerprint != ItemSeederManifestCatalogue.ComputeSourceFingerprint(root))
			throw new InvalidOperationException("Catalogue sources changed during capture; discard this incomplete capture.");
		WriteJson(Path.Combine(output, "source-inventory.json"), sourceFiles);
		using (var ledger = File.CreateText(Path.Combine(output, "source-field-ledger.ndjson")))
		{
			foreach (var source in sourceFiles.Where(x => x.Path.EndsWith(".tsv", StringComparison.Ordinal)))
			{
				var lines = File.ReadAllLines(Path.Combine(root, source.Path));
				if (lines.Length == 0) continue;
				var columns = lines[0].Split('\t');
				for (var i = 1; i < lines.Length; i++)
				{
					if (string.IsNullOrWhiteSpace(lines[i])) continue;
					WriteLine(ledger, new { Source = source.Path, Line = i + 1, Fields = columns,
						Identity = lines[i].Split('\t')[0], Status = "exact source retained in sources.zip" });
				}
			}
		}
		var artifacts = Directory.GetFiles(output).Where(x => Path.GetFileName(x) != "receipt.json")
			.OrderBy(x => x, StringComparer.Ordinal)
			.Select(x => new { Path = Path.GetFileName(x), Bytes = new FileInfo(x).Length,
				Sha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(x))).ToLowerInvariant() }).ToArray();
		WriteJson(Path.Combine(output, "receipt.json"), new
		{
			document.Contract, document.Eras, document.TechnologyProfile, Unified = unified,
			BinarySha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(typeof(ItemSeeder).Assembly.Location))).ToLowerInvariant(),
			SourceFingerprint = ItemSeederManifestCatalogue.ComputeSourceFingerprint(root),
			Counts = document.Definitions.GroupBy(x => x.Identity.EntityType).ToDictionary(x => x.Key, x => x.Count()),
			SourceFiles = sourceFiles.Count, Artifacts = artifacts,
			Evidence = "Authoring capture with synthetic IDs; no relational install or prerequisite qualification."
		});
		Console.WriteLine($"Captured {document.Definitions.Count:N0} complete definitions and {sourceFiles.Count:N0} source files to {output}.");
		return true;
	}

	private static void WriteLine(StreamWriter writer, object value) => writer.WriteLine(JsonSerializer.Serialize(value, CatalogueDefinitionCapture.JsonOptions));
	internal static void WriteJson(string path, object value) => File.WriteAllText(path,
		JsonSerializer.Serialize(value, new JsonSerializerOptions(CatalogueDefinitionCapture.JsonOptions) { WriteIndented = true }) + "\n");

	internal static IReadOnlyList<string> Compare(string first, string second)
	{
		static string Key(JsonNode node) => $"{node["identity"]!["entityType"]}\u001f{node["identity"]!["stableKey"]}";
		static IEnumerable<JsonNode> Read(string directory)
		{
			string? previous = null;
			foreach (var line in File.ReadLines(Path.Combine(directory, "definitions.ndjson")))
			{
				var node = JsonNode.Parse(line) ?? throw new InvalidDataException("Empty catalogue definition.");
				if (node["identity"] is not JsonObject || node["symbolicDefinition"] is null || node["supplement"] is null ||
					string.IsNullOrWhiteSpace(node["identity"]!["entityType"]?.GetValue<string>()) ||
					string.IsNullOrWhiteSpace(node["identity"]!["stableKey"]?.GetValue<string>()))
					throw new InvalidDataException("Incomplete catalogue definition.");
				var key = Key(node);
				if (previous is not null && StringComparer.Ordinal.Compare(previous, key) >= 0)
					throw new InvalidDataException("Catalogue identities must be unique and sorted.");
				previous = key;
				yield return node;
			}
			if (previous is null) throw new InvalidDataException("Catalogue capture contains no definitions.");
		}
		using var before = Read(first).GetEnumerator();
		using var after = Read(second).GetEnumerator();
		var differences = new List<string>();
		var hasBefore = before.MoveNext();
		var hasAfter = after.MoveNext();
		while (hasBefore || hasAfter)
		{
			var left = hasBefore ? before.Current : null;
			var right = hasAfter ? after.Current : null;
			var key = left is null ? Key(right!) : Key(left);
			var order = left is null ? 1 : right is null ? -1 : StringComparer.Ordinal.Compare(Key(left), Key(right));
			if (order != 0)
			{
				differences.Add((order < 0 ? Key(left!) : Key(right!)) + ": missing identity");
				if (order < 0) hasBefore = before.MoveNext(); else hasAfter = after.MoveNext();
				continue;
			}
			foreach (var field in new[] { "symbolicDefinition", "supplement" })
				if (!JsonNode.DeepEquals(left![field], right![field])) differences.Add(key + ": " + field);
			foreach (var field in new[] { "module", "eraAdmissions", "dependencies", "ownershipPolicy" })
				if (!JsonNode.DeepEquals(left!["identity"]![field], right!["identity"]![field])) differences.Add(key + ": " + field);
			hasBefore = before.MoveNext(); hasAfter = after.MoveNext();
		}
		var beforeGraphs = JsonNode.Parse(File.ReadAllText(Path.Combine(first, "source-graphs.json")));
		var afterGraphs = JsonNode.Parse(File.ReadAllText(Path.Combine(second, "source-graphs.json")));
		if (!JsonNode.DeepEquals(beforeGraphs, afterGraphs)) differences.Add("source-only catalogue graphs differ");
		return differences;
	}

	private static void Benchmark(string output, string root, string eras, string profile, bool unified)
	{
		var saved = Console.Out;
		var timer = Stopwatch.StartNew();
		var allocated = GC.GetTotalAllocatedBytes(true);
		ItemSeederManifestDocument manifest;
		try
		{
			Console.SetOut(TextWriter.Null);
			using var context = Program.CreateItemManifestCaptureContext();
			manifest = new ItemSeeder().CaptureManifest(context, root, eras,
				new Dictionary<string, string> { ["technologyprofile"] = profile });
		}
		finally { Console.SetOut(saved); }
		timer.Stop();
		WriteJson(Path.Combine(output, "benchmark.json"), new
		{
			Unified = unified, Eras = eras, Profile = profile, ElapsedMilliseconds = timer.Elapsed.TotalMilliseconds,
			AllocatedBytes = GC.GetTotalAllocatedBytes(true) - allocated,
			PeakWorkingSetBytes = Process.GetCurrentProcess().PeakWorkingSet64, Aggregates = manifest.Entries.Count,
			DefinitionDigest = ItemSeederManifestCatalogue.Fingerprint(manifest.Entries),
			Scope = "Cold process EF in-memory authoring/manifest capture; excludes relational writes and export serialization.",
			RepresentativeConstruction = CataloguePerformance.MeasureConstruction(!unified, 42756)
		});
		Console.WriteLine($"{(unified ? "Unified" : "Legacy")}: {manifest.Entries.Count:N0} aggregates, {timer.Elapsed.TotalMilliseconds:N1}ms.");
	}
}
