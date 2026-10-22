using System.Text;
using System.Text.Json;
using MigrationModelCompiler;

try
{
	if (args.Length < 2) throw new ArgumentException("Usage: MigrationModelCompiler <update|check|generate> <repository> [forwarders|BatchNNNN] [output]");
	var repository = Path.GetFullPath(args[1]);
	var migrationDirectory = Path.Combine(repository, "MudsharpDatabaseLibrary", "Migrations");
	var modelDirectory = Path.Combine(repository, "MigrationModels");
	var manifestPath = Path.Combine(modelDirectory, "partitions.json");
	if (args[0] == "update")
	{
		var updated = PartitionManifest.Partition(migrationDirectory);
		Directory.CreateDirectory(modelDirectory);
		WriteIfChanged(manifestPath, JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true }) + "\n");
		foreach (var batch in updated.Files.Values.Distinct())
		{
			var projectDirectory = Path.Combine(modelDirectory, batch);
			Directory.CreateDirectory(projectDirectory);
			WriteIfChanged(Path.Combine(projectDirectory, batch + ".csproj"),
				"<Project Sdk=\"Microsoft.NET.Sdk\">\n\t<Import Project=\"../MigrationModelBatch.props\" />\n\t<Import Project=\"../MigrationModelGeneration.targets\" />\n</Project>\n");
		}
		Console.WriteLine($"Assigned {updated.Files.Count} designers to {updated.Files.Values.Distinct().Count()} batches.");
		return 0;
	}

	var manifest = PartitionManifest.Load(manifestPath);
	manifest.Validate(migrationDirectory, args.Length == 4 && args[0] == "generate" && args[2] != "forwarders" ? args[2] : null);
	var projects = Directory.GetFiles(modelDirectory, "Batch*.csproj", SearchOption.AllDirectories)
		.Select(Path.GetFileNameWithoutExtension).OrderBy(x => x, StringComparer.Ordinal);
	if (!projects.SequenceEqual(manifest.Files.Values.Distinct().OrderBy(x => x, StringComparer.Ordinal)))
	{
		throw new InvalidDataException("Batch projects differ from the partition manifest; remove obsolete batch projects after updating partitions.");
	}
	if (args[0] == "check") return 0;
	if (args[0] != "generate" || args.Length != 4) throw new ArgumentException("Invalid generation arguments.");
	var role = args[2];
	if (role != "forwarders" && !manifest.Files.ContainsValue(role)) throw new ArgumentException("Unknown model batch.");
	ProjectionWriter.Write(manifest, migrationDirectory, role, Path.GetFullPath(args[3]));
	return 0;
}
catch (Exception exception)
{
	Console.Error.WriteLine("Migration model generation failed: " + exception.Message);
	return 1;
}

static void WriteIfChanged(string path, string content)
{
	if (File.Exists(path) && File.ReadAllText(path) == content) return;
	File.WriteAllText(path, content, new UTF8Encoding(false));
}
