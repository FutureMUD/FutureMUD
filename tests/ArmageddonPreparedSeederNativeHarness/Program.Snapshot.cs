#nullable enable
using DatabaseSeeder;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MySql.Data.MySqlClient;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RefreshOwnedBlankSnapshot()
	{
		var repository = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
		var assets = Path.Combine(repository, "DatabaseSeeder", "Assets", "Database");
		Require(File.Exists(Path.Combine(repository, "DatabaseSeeder", "DatabaseSeeder.csproj")) &&
			!new DirectoryInfo(assets).Attributes.HasFlag(FileAttributes.ReparsePoint), "Snapshot output is not the checked-out asset directory.");
		var version = new Version(System.Xml.Linq.XDocument.Load(Path.Combine(repository, "DatabaseSeeder", "DatabaseSeeder.csproj"))
			.Descendants("Version").Single().Value);
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		var manifest = database.RefreshBlankSnapshot(assets, version);
		var sql = File.ReadAllText(BlankDatabaseSnapshotManifest.GetSnapshotPath(assets));
		Require(!sql.Contains(OwnershipTable, StringComparison.OrdinalIgnoreCase), "Snapshot contains a harness marker.");
		using var imported = TestDatabase.CreateFresh("futuremud_land_"); // Imports the just-generated snapshot.
		using var db = NewIndependentContext(imported.ConnectionString);
		Require(db.Database.GetAppliedMigrations().Last() == manifest.LatestMigrationId && !db.Database.GetPendingMigrations().Any(),
			"Fresh snapshot import did not match the current migration assembly.");
		Require(imported.ForeignKeys("tracks").Contains("CharacterInstanceId->characterinstances.Id", StringComparer.OrdinalIgnoreCase),
			"Imported track identity foreign key is missing.");
		Console.WriteLine($"ARMS3-snapshot=passed real-refresh fresh-import migration:{manifest.LatestMigrationId} version:{manifest.ProductVersion} no-harness-marker-in-dump");
		return 0;
	}

	private sealed partial class TestDatabase
	{
		public BlankDatabaseSnapshotManifest RefreshBlankSnapshot(string assets, Version version)
		{
			Require(_created && _markerReady && HasOwnedPrefix(Name) && OwnershipMarkerMatches(), "Snapshot refresh requires this exact owned database.");
			using (var connection = OpenDatabase()) { } // Validate the live UUID, port, data directory and marker before recreation.
			_markerReady = false; OwnedConnections.Register(Name, _ownershipToken, ready: false);
			try
			{
				return BlankDatabaseSnapshotManifest.Refresh(assets, new DatabaseUpgradeCoordinator(), version, ConnectionString);
			}
			finally
			{
				using var server = new MySqlConnection(_serverBuilder.ConnectionString);
				OwnedConnections.Validate("snapshot-reseal-before-connect", server, allowServer: true); server.Open();
				OwnedConnections.Validate("snapshot-reseal-connected", server, allowServer: true);
				if (DatabaseExists(server, Name))
				{
					// Recreation/export omits the marker; restore it only after export, before owned cleanup.
					if (!OwnershipMarkerMatches()) WriteOwnershipMarker();
					_markerReady = true; OwnedConnections.Register(Name, _ownershipToken, ready: true);
				}
			}
		}
	}
}
