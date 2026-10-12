#nullable enable
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MySql.Data.MySqlClient;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record CatalogueFixtureReceipt(string Database, string SqlFile, string SqlSha256, string ContentSourceSha256);
	private static string CatalogueRepository => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

	private static string CatalogueContentSourceHash()
	{
		var root = CatalogueRepository;
		var paths = new[] { "DatabaseSeeder", "MudsharpDatabaseLibrary" }
			.SelectMany(folder => Directory.EnumerateFiles(Path.Combine(root, folder), "*.cs", SearchOption.TopDirectoryOnly))
			.Concat(new[] { "DatabaseSeeder/Seeders", "MudsharpDatabaseLibrary/Models", "MudsharpDatabaseLibrary/Migrations", "MudsharpDatabaseLibrary/Database" }
				.SelectMany(folder => Directory.EnumerateFiles(Path.Combine(root, folder), "*.cs", SearchOption.AllDirectories)))
			.Append(Path.Combine(root, "Design Documents", "Magic", "Armageddon_Seeder_Completeness_Design_20261011.json"))
			.OrderBy(path => Path.GetRelativePath(root, path), StringComparer.Ordinal);
		var stamps = new StringBuilder();
		foreach (var path in paths) stamps.Append(Path.GetRelativePath(root, path).Replace('\\', '/')).Append('\0')
			.Append(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)))).Append('\n');
		return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(stamps.ToString())));
	}

	private static void SaveCatalogueFixture(TestDatabase database)
	{
		using (var owned = database.OpenOwnedConnection()) { }
		var folder = Path.Combine(CatalogueRepository, ".artifacts", "s3-catalogue-fixtures",
			DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ") + "_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(folder);
		var sql = new MySqlDatabaseBackupService().CreateBackup(database.ConnectionString, folder);
		var receipt = new CatalogueFixtureReceipt(database.Name, Path.GetFileName(sql),
			Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(sql))), CatalogueContentSourceHash());
		var manifest = Path.Combine(folder, "fixture.json");
		File.WriteAllText(manifest, JsonSerializer.Serialize(receipt));
		Console.WriteLine("ARMS3-catalogue-fixture=" + manifest + " exact-source-and-dump-hashes recorded before gameplay");
	}

	private static int RunCatalogueFixtureBoot()
	{
		var supplied = Environment.GetEnvironmentVariable("FUTUREMUD_S3_CATALOGUE_FIXTURE")
			?? throw new InvalidOperationException("Missing explicit owned catalogue fixture manifest.");
		var manifest = Path.GetFullPath(supplied);
		var fixtureRoot = Path.Combine(CatalogueRepository, ".artifacts", "s3-catalogue-fixtures") + Path.DirectorySeparatorChar;
		Require(manifest.StartsWith(fixtureRoot, StringComparison.OrdinalIgnoreCase) && File.Exists(manifest) &&
			Path.GetFileName(manifest) == "fixture.json", "Catalogue fixture must belong to this worktree's private artifacts.");
		for (var directory = new DirectoryInfo(Path.GetDirectoryName(manifest)!); directory is not null &&
			directory.FullName.StartsWith(fixtureRoot.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase); directory = directory.Parent)
			Require(!directory.Attributes.HasFlag(FileAttributes.ReparsePoint), "Fixture directory cannot redirect outside its owned root.");
		var receipt = JsonSerializer.Deserialize<CatalogueFixtureReceipt>(File.ReadAllText(manifest))
			?? throw new InvalidOperationException("Malformed catalogue fixture receipt.");
		Require(Path.GetFileName(receipt.SqlFile) == receipt.SqlFile, "Fixture dump must be a sibling file.");
		var sql = Path.Combine(Path.GetDirectoryName(manifest)!, receipt.SqlFile);
		Require(!File.GetAttributes(sql).HasFlag(FileAttributes.ReparsePoint) &&
			receipt.SqlSha256 == Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(sql))) &&
			receipt.ContentSourceSha256 == CatalogueContentSourceHash(), "Catalogue fixture dump or seeder/model source has changed.");
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		database.RestoreCatalogueFixture(sql, receipt.Database);
		ConfigureNativeDatabase(database.ConnectionString);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(!db.Database.GetPendingMigrations().Any() && !db.Database.HasPendingModelChanges(), "Restored catalogue schema is stale.");
			foreach (var blueprint in new ArmageddonStockCatalogue().Blueprints.Where(x => !x.DeferredToS4))
				Require(db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == blueprint.Key && !x.Retired),
					"Restored catalogue is missing " + blueprint.Key);
		}
		Console.WriteLine("ARMS3-restored-catalogue=verified exact dump/source hashes all-198-blueprints fresh ownership marker; not a new seeder replay");
		RunPreparedReplayBoot(database, typeof(ArmageddonMagicSeeder).Assembly, "PreparedCatalogueBootSmoke.py", "prepared-catalogue-boot-native");
		return 0;
	}

	private sealed partial class TestDatabase
	{
		public void RestoreCatalogueFixture(string sql, string originalDatabase)
		{
			Require(_created && _markerReady && HasOwnedPrefix(Name) && HasOwnedPrefix(originalDatabase) &&
				OwnershipMarkerMatches(), "Fixture import requires this exact newly owned database.");
			using (var owned = OpenDatabase()) { }
			_markerReady = false; OwnedConnections.Register(Name, _ownershipToken, ready: false);
			try
			{
				// Import issues USE on its server connection. Keep that session out of the empty-
				// database pool used by subsequent ownership/cleanup checks; normal runtime pooling stays enabled.
				var importConnection = new MySqlConnectionStringBuilder(ConnectionString) { Pooling = false }.ConnectionString;
				new DatabaseUpgradeCoordinator().ImportBlankDatabaseSnapshot(importConnection, sql, originalDatabase);
			}
			finally
			{
				using var connection = OpenDatabase();
				using var command = connection.CreateCommand();
				command.CommandText = $"DROP TABLE IF EXISTS `{OwnershipTable}`;";
				command.ExecuteNonQuery();
				WriteOwnershipMarker();
				_markerReady = true; OwnedConnections.Register(Name, _ownershipToken, ready: true);
			}
		}
	}
}
