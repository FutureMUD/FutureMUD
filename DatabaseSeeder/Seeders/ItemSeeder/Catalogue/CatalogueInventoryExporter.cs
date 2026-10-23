#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace DatabaseSeeder.Seeders;

// Developer export only. Checked-in C# projections are built normally and need no generation step.
internal static class CatalogueInventoryExporter
{
	private const int BucketCount = 16;
	private const int RowsPerMethod = 40;
	private const string Marker = "// Derived ownership index. Refresh with --export-compiled-item-inventory; edit catalogue definitions instead.";
	private static readonly JsonSerializerOptions LiteralOptions = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
	private static string Literal(string text) => JsonSerializer.Serialize(text, LiteralOptions);
	private static string Strings(IEnumerable<string> values) => $"[{string.Join(", ", values.Select(Literal))}]";

	internal static int Bucket(ItemSeederManifestEntry entry)
	{
		var hash = 2166136261u;
		foreach (var value in Encoding.UTF8.GetBytes(entry.EntityType + ":" + entry.StableKey))
			hash = unchecked((hash ^ value) * 16777619u);
		return (int)(hash % BucketCount);
	}

	internal static bool TryHandle(string[] args)
	{
		if (!args.Contains("--export-compiled-item-inventory", StringComparer.OrdinalIgnoreCase)) return false;
		var root = ItemSeederManifestCatalogue.FindRepositoryRoot();
		using var context = Program.CreateItemManifestCaptureContext();
		var document = new ItemSeeder().CaptureManifest(context, root);
		Export(root, document.Entries);
		Console.WriteLine($"Exported {document.Entries.Count:N0} ownership identities into {BucketCount} compiled index partitions.");
		return true;
	}

	internal static void Export(string root, IReadOnlyCollection<ItemSeederManifestEntry> entries)
	{
		ItemSeederManifestCatalogue.Validate(entries);
		var assemblies = Path.Combine(root, "DatabaseSeeder", "Seeders", "ItemSeeder", "Catalogue", "Assemblies");
		for (var bucket = 0; bucket < BucketCount; bucket++)
		{
			var name = $"Inventory{bucket:00}";
			var folder = Path.Combine(assemblies, name);
			var content = Path.Combine(folder, "Content");
			Directory.CreateDirectory(content);
			var rows = entries.Where(entry => Bucket(entry) == bucket)
				.OrderBy(entry => entry.EntityType, StringComparer.Ordinal)
				.ThenBy(entry => entry.StableKey, StringComparer.Ordinal)
				.ToArray();
			var files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var calls = new StringBuilder();
			for (var offset = 0; offset < rows.Length; offset += RowsPerMethod)
			{
				var method = $"Batch{offset / RowsPerMethod + 1:000}";
				var path = Path.Combine(content, method + ".cs");
				files.Add(path);
				var text = new StringBuilder($"{Marker}\n#nullable enable\n\nusing System.Collections.Generic;\n\nnamespace DatabaseSeeder;\n\npublic static partial class {name}Stock\n{{\n\tpublic static IReadOnlyList<ItemSeederManifestEntry> {method}() =>\n\t[\n");
				foreach (var entry in rows.Skip(offset).Take(RowsPerMethod))
				{
					text.Append($"\t\tnew({Literal(entry.EntityType)}, {Literal(entry.StableKey)}, {Literal(entry.Module)}, {Strings(entry.EraAdmissions)}, {Strings(entry.Dependencies)}, ItemSeederOwnershipPolicy.{entry.OwnershipPolicy}, {Literal(entry.Fingerprint)}),\n");
				}
				text.Append("\t];\n}\n");
				WriteIfChanged(path, text.ToString());
				calls.Append($"\t\tforeach (var entry in {method}()) yield return entry;\n");
			}
			foreach (var obsolete in Directory.EnumerateFiles(content, "Batch*.cs").Where(path => !files.Contains(path)))
			{
				if (!File.ReadLines(obsolete).First().Equals(Marker, StringComparison.Ordinal))
					throw new InvalidDataException($"Refusing to remove an unrecognised inventory source: {obsolete}");
				File.Delete(obsolete);
			}
			if (rows.Length == 0) calls.Append("\t\tyield break;\n");
			WriteIfChanged(Path.Combine(folder, "CatalogueBatch.cs"), $"{Marker}\n#nullable enable\n\nusing System.Collections.Generic;\n\nnamespace DatabaseSeeder;\n\npublic static partial class {name}Stock\n{{\n\tpublic static IEnumerable<ItemSeederManifestEntry> Entries()\n\t{{\n{calls}\t}}\n}}\n");
		}
	}

	private static void WriteIfChanged(string path, string value)
	{
		if (File.Exists(path) && File.ReadAllText(path).ReplaceLineEndings("\n") == value) return;
		File.WriteAllText(path, value, new UTF8Encoding(false));
	}
}
