using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MigrationModelCompiler;

public sealed class PartitionManifest
{
	public const int MaximumBatchBytes = 16 * 1024 * 1024;
	public SortedDictionary<string, string> Files { get; set; } = new(StringComparer.Ordinal);
	public static string Normalise(string text) => text.Replace("\r\n", "\n");

	public static PartitionManifest Load(string path)
	{
		using var document = JsonDocument.Parse(File.ReadAllText(path));
		var root = document.RootElement;
		if (root.ValueKind != JsonValueKind.Object || root.EnumerateObject().Count() != 1 ||
		    !root.TryGetProperty(nameof(Files), out var files) || files.ValueKind != JsonValueKind.Object)
		{
			throw new InvalidDataException("Expected one Files object in the migration partition manifest.");
		}
		var result = new PartitionManifest();
		foreach (var file in files.EnumerateObject())
		{
			if (file.Value.ValueKind != JsonValueKind.String ||
			    !result.Files.TryAdd(file.Name, file.Value.GetString()!))
			{
				throw new InvalidDataException($"Invalid or duplicate migration partition assignment: {file.Name}");
			}
		}
		return result;
	}

	public void Validate(string migrationsDirectory, string? batch = null)
	{
		var actual = Directory.GetFiles(migrationsDirectory, "*.Designer.cs").Select(Path.GetFileName)
			.OrderBy(x => x, StringComparer.Ordinal).ToArray();
		if (!actual.SequenceEqual(Files.Keys.OrderBy(x => x, StringComparer.Ordinal)))
		{
			throw new InvalidDataException("Migration partition assignments differ from designer files. Run the partition update command.");
		}
		foreach (var group in Files.GroupBy(x => x.Value))
		{
			if (!Regex.IsMatch(group.Key, "^Batch[0-9]{4}$")) throw new InvalidDataException("Invalid migration batch name.");
			if (batch is not null && group.Key != batch) continue;
			var size = group.Sum(x => (long)Encoding.UTF8.GetByteCount(Normalise(File.ReadAllText(Path.Combine(migrationsDirectory, x.Key)))));
			if (size > MaximumBatchBytes) throw new InvalidDataException($"{group.Key} exceeds the 16 MiB source limit.");
		}
	}

	public static PartitionManifest Partition(string migrationsDirectory)
	{
		var manifest = new PartitionManifest();
		var batch = 1;
		var size = 0;
		foreach (var file in Directory.GetFiles(migrationsDirectory, "*.Designer.cs").OrderBy(Path.GetFileName, StringComparer.Ordinal))
		{
			var bytes = Encoding.UTF8.GetByteCount(Normalise(File.ReadAllText(file)));
			if (bytes > MaximumBatchBytes) throw new InvalidDataException($"Designer exceeds the batch source limit: {file}");
			if (size + bytes > MaximumBatchBytes) { batch++; size = 0; }
			manifest.Files.Add(Path.GetFileName(file), $"Batch{batch:D4}");
			size += bytes;
		}
		return manifest;
	}
}
