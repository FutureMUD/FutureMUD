using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MigrationModelCompiler;

public static class ProjectionWriter
{
	public sealed record CacheEntry(string Input, string Output);

	public static void Write(PartitionManifest manifest, string migrationsDirectory, string role, string outputDirectory)
	{
		Directory.CreateDirectory(outputDirectory);
		var cachePath = Path.Combine(outputDirectory, "projection-cache.json");
		var cache = new Dictionary<string, CacheEntry>();
		if (File.Exists(cachePath))
		{
			try { cache = JsonSerializer.Deserialize<Dictionary<string, CacheEntry>>(File.ReadAllText(cachePath)) ?? cache; }
			catch (JsonException) { /* A damaged intermediate cache is safely regenerated. */ }
		}
		var updated = new Dictionary<string, CacheEntry>();
		var toolHash = Hash(File.ReadAllBytes(typeof(ProjectionWriter).Assembly.Location));
		var expected = new HashSet<string>(OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
		foreach (var pair in manifest.Files.Where(x => role == "forwarders" || x.Value == role))
		{
			var source = PartitionManifest.Normalise(File.ReadAllText(Path.Combine(migrationsDirectory, pair.Key)));
			var inputHash = Hash(Encoding.UTF8.GetBytes(toolHash + role + pair.Value + source));
			var file = Path.Combine(outputDirectory, pair.Key + ".g.cs");
			if (cache.TryGetValue(pair.Key, out var entry) && entry is not null && entry.Input == inputHash &&
			    File.Exists(file) && entry.Output == Hash(File.ReadAllBytes(file)))
			{
				expected.Add(file);
				updated.Add(pair.Key, entry);
				continue;
			}
			// Separate files preserve each original designer's using and directive scopes.
			var projection = ModelProjection.Create(source, pair.Value, pair.Key);
			var generated = role == "forwarders" ? projection.Forwarder : projection.Helper;
			if (generated is null) continue;
			expected.Add(file);
			var bytes = Encoding.UTF8.GetBytes(generated);
			if (!File.Exists(file) || !File.ReadAllBytes(file).SequenceEqual(bytes)) File.WriteAllBytes(file, bytes);
			updated.Add(pair.Key, new CacheEntry(inputHash, Hash(bytes)));
		}
		foreach (var stale in Directory.GetFiles(outputDirectory, "*.Designer.cs.g.cs"))
		{
			if (!expected.Contains(stale)) File.Delete(stale);
		}
		File.WriteAllText(cachePath, JsonSerializer.Serialize(updated));
	}

	private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));
}
