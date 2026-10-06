#nullable enable

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Moq;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Events.Hooks;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MySql.Data.MySqlClient;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void QualifyCellSpatialContraction(TestDatabase database,
		IReadOnlyDictionary<string, string[]> originalSchema, IReadOnlyDictionary<string, string> originalValues,
		IReadOnlyDictionary<string, string> originalRetainedCellValues,
		long zoneId)
	{
		void Sql(string text) { using var c = database.OpenOwnedConnection(); using var cmd = new MySqlCommand(text, c); cmd.ExecuteNonQuery(); }
		long Scalar(string text) { using var c = database.OpenOwnedConnection(); using var cmd = new MySqlCommand(text, c); return Convert.ToInt64(cmd.ExecuteScalar()); }
		// Unrelated objects are permitted and must survive the real backup/restore path.
		Console.WriteLine("CellSpatialContraction-setup=create-unrelated-SQL-objects");
		foreach (var statement in new[] {
			"CREATE TABLE fixture_object_audit(Id bigint PRIMARY KEY)",
			"CREATE VIEW fixture_cell_view AS SELECT Id FROM cells",
			"CREATE PROCEDURE fixture_constant() SELECT 1",
			"CREATE TRIGGER fixture_area_audit AFTER UPDATE ON areas FOR EACH ROW DO 1",
			"CREATE EVENT fixture_disabled_event ON SCHEDULE EVERY 1 DAY DISABLE DO INSERT INTO fixture_object_audit VALUES(1)"
		}) Sql(statement);
		Console.WriteLine("CellSpatialContraction-setup=capture-expanded-backup");
		using var snapshot = database.OpenOwnedConnection();
		var schema = CaptureSpatialSchema(snapshot);
		var values = CaptureSpatialValues(snapshot, schema);
		var definitions = CaptureSpatialTableDefinitions(snapshot, schema);
		var historySchema = new Dictionary<string, string[]> { ["__efmigrationshistory"] = ["MigrationId", "ProductVersion"] };
		var history = CaptureSpatialValues(snapshot, historySchema);
		var objects = CaptureSpatialObjectDefinitions(snapshot);
		var provenanceSchema = schema.Where(x => x.Key is "cellroommigrationledger" or "cellroomareamigrationledger")
			.ToDictionary(x => x.Key, x => x.Value);
		var provenance = CaptureSpatialValues(snapshot, provenanceSchema);
		snapshot.Close();
		var directory = Path.Combine(Path.GetTempPath(), "futuremud-cell-contraction-backup_" + Guid.NewGuid().ToString("N"));
		var service = new MySqlDatabaseBackupService();
		string backup;
		using (var verified = database.OpenOwnedConnection()) backup = service.CreateBackup(database.ConnectionString, directory);
		void Restore()
		{
			using (var verified = database.OpenOwnedConnection()) service.RestoreBackup(database.ConnectionString, backup);
			var builder = new MySqlConnectionStringBuilder(database.ConnectionString) { Database = string.Empty };
			using (var server = new MySqlConnection(builder.ConnectionString))
			{
				OwnedConnections.Validate("contraction-restore-server-pool", server, allowServer: true);
				MySqlConnection.ClearPool(server);
			}
			using var restored = database.OpenOwnedConnection();
			var actualSchema = CaptureSpatialSchema(restored);
			Require(schema.Count == actualSchema.Count && schema.All(x => actualSchema.TryGetValue(x.Key, out var v) && x.Value.SequenceEqual(v)), "Restore recovers expanded schema exactly.");
			Require(SpatialValuesEqual(values, CaptureSpatialValues(restored, schema)), "Restore recovers every expanded key/value.");
			Require(SpatialValuesEqual(definitions, CaptureSpatialTableDefinitions(restored, schema)), "Restore recovers indexes, FKs, defaults, collations and counters.");
			Require(SpatialValuesEqual(history, CaptureSpatialValues(restored, historySchema)), "Restore recovers expanded migration history.");
			Require(SpatialValuesEqual(objects, CaptureSpatialObjectDefinitions(restored)), "Restore recovers all views, routines, triggers and events exactly.");
		}
		void Migrate(bool optIn = true, bool reconcile = false, string? fault = null)
		{
			using var db = NewIndependentContext(database.ConnectionString, fault is null ? null : new CellSpatialContractionFault(fault));
			db.Database.OpenConnection();
			if (optIn) db.Database.ExecuteSqlRaw("SET @FutureMUD_CellSpatialContractionMaintenance=1;");
			if (reconcile) db.Database.ExecuteSqlRaw("SET @FutureMUD_CellSpatialReconcile=1;");
			db.GetService<IMigrator>().Migrate(db.Database.GetMigrations().Single(x => x.EndsWith("_CellSpatialContraction")));
		}
		void Refuse(string label, string change, string expected, bool optIn = true, bool reconcile = false)
		{
			if (change.Length > 0) Sql(change);
			using var c = database.OpenOwnedConnection();
			var stateSchema = CaptureSpatialSchema(c);
			var stateValues = CaptureSpatialValues(c, stateSchema);
			var stateDefinitions = CaptureSpatialTableDefinitions(c, stateSchema);
			var stateHistory = CaptureSpatialValues(c, historySchema);
			var stateObjects = CaptureSpatialObjectDefinitions(c);
			try { Migrate(optIn, reconcile); throw new InvalidOperationException("Contraction unexpectedly accepted " + label); }
			catch (MySqlConnector.MySqlException ex) when (ex.Message.Contains(expected, StringComparison.OrdinalIgnoreCase)) { }
			Require(Scalar("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema=DATABASE() AND table_name='rooms'") == 1, "Refusal precedes Room deletion.");
			Require(Scalar("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema=DATABASE() AND table_name='cellroomcontractionledger'") == 0, "Refusal precedes game-table DDL.");
			Require(SpatialValuesEqual(stateValues, CaptureSpatialValues(c, stateSchema)), "Refusal changes no game data: " + label);
			var actualSchema = CaptureSpatialSchema(c);
			Require(stateSchema.Count == actualSchema.Count && stateSchema.All(x => actualSchema.TryGetValue(x.Key, out var v) && x.Value.SequenceEqual(v)), "Refusal changes no game schema: " + label);
			Require(SpatialValuesEqual(stateDefinitions, CaptureSpatialTableDefinitions(c, stateSchema)), "Refusal changes no game table definitions: " + label);
			Require(SpatialValuesEqual(stateHistory, CaptureSpatialValues(c, historySchema)), "Refusal changes no migration history: " + label);
			var actualObjects = CaptureSpatialObjectDefinitions(c);
			actualObjects.Remove("routine:fm_cell_spatial_contract_20261006161646");
			Require(SpatialValuesEqual(stateObjects, actualObjects), "Refusal preserves SQL objects and trigger ownership/definitions: " + label);
			c.Close(); Restore();
			Console.WriteLine("CellSpatialContraction-refusal=PASS game-data-and-tables-unchanged case=" + label);
		}
		Refuse("maintenance", "", "maintenance session", optIn: false);
		Refuse("multiple-children", $"INSERT INTO cells(Id,RoomId,ZoneId,X,Y,Z,EffectData) VALUES(8103,9000,{zoneId},17,-2,4,'<Effects/>');", "multiple Cells");
		Refuse("orphan-parent", "SET FOREIGN_KEY_CHECKS=0; UPDATE cells SET RoomId=999999 WHERE Id=8101; SET FOREIGN_KEY_CHECKS=1;", "orphan Cell");
		Refuse("referenced-empty", "INSERT INTO areas_rooms(AreaId,RoomId) VALUES(9002,9001);", "empty Room");
		Refuse("stale-cell-copy", "UPDATE cells SET X=1234 WHERE Id=8101;", "stale");
		Refuse("changed-source", "UPDATE rooms SET X=18 WHERE Id=9000;", "stale");
		Refuse("stale-area-copy", "DELETE FROM areas_cells WHERE AreaId=9000 AND CellId=8101;", "stale");
		Refuse("orphan-derived-area", "SET FOREIGN_KEY_CHECKS=0; INSERT INTO areas_cells VALUES(999999,8101); SET FOREIGN_KEY_CHECKS=1;", "orphan direct-cell");
		Refuse("missing-default-cell", $"SET FOREIGN_KEY_CHECKS=0; UPDATE zones SET DefaultCellId=999999 WHERE Id={zoneId}; SET FOREIGN_KEY_CHECKS=1;", "default", reconcile: true);
		Refuse("unknown-column", "CREATE TABLE fixture_unclassified(Id bigint PRIMARY KEY,RoomId bigint);", "unclassified RoomId");
		Refuse("storyteller-extension-column", "ALTER TABLE aistorytellersituations ADD COLUMN RoomId bigint DEFAULT 9000;", "unclassified RoomId");
		Refuse("unknown-fk", "CREATE TABLE fixture_unclassified_fk(Id bigint PRIMARY KEY,OldOwner bigint,FOREIGN KEY(OldOwner) REFERENCES rooms(Id));", "unknown foreign key");
		Refuse("view-reference", "CREATE VIEW fixture_room_view AS SELECT Id FROM rooms;", "unclassified or unreadable view");
		Refuse("routine-reference", "CREATE PROCEDURE fixture_room_routine() SELECT Id FROM rooms;", "unclassified or unreadable routine");
		Refuse("trigger-reference", "CREATE TRIGGER fixture_room_trigger AFTER INSERT ON fixture_object_audit FOR EACH ROW INSERT INTO fixture_object_audit VALUES((SELECT COUNT(*) FROM rooms));", "unclassified or unreadable trigger");
		Refuse("trigger-owner-rooms", "CREATE TRIGGER fixture_owner_rooms AFTER INSERT ON rooms FOR EACH ROW DO 1;", "fixture_owner_rooms");
		Refuse("trigger-owner-areas-rooms", "CREATE TRIGGER fixture_owner_areas_rooms AFTER INSERT ON areas_rooms FOR EACH ROW DO 1;", "fixture_owner_areas_rooms");
		Refuse("event-reference", "CREATE EVENT fixture_room_event ON SCHEDULE EVERY 1 DAY DISABLE DO DELETE FROM rooms WHERE Id=-1;", "unclassified or unreadable event");
		Refuse("typed-reference", "UPDATE cells SET EffectData='<Effects><AnchorType>Room</AnchorType><AnchorId>9000</AnchorId></Effects>' WHERE Id=8101;", "Room reference");
		Refuse("reconcile-trigger", "CREATE TRIGGER fixture_copy_trigger AFTER UPDATE ON cells FOR EACH ROW DO 1; UPDATE rooms SET X=18 WHERE Id=9000;", "trigger", reconcile: true);
		foreach (var fault in new[] { "partial-ddl", "before-history" })
		{
			try { Migrate(fault: fault); throw new InvalidOperationException("Fault was not injected: " + fault); }
			catch (InvalidOperationException ex) when (ex.Message == "Fixture contraction failure: " + fault) { }
			Require(Scalar("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema=DATABASE() AND table_name='areas_rooms'") == 0, "Destructive DDL persists; ordinary transaction rollback is not recovery.");
			Require(Scalar("SELECT COUNT(*) FROM __efmigrationshistory WHERE MigrationId LIKE '%CellSpatialContraction'") == 0, "Failed contraction remains unapplied.");
			Restore();
			Console.WriteLine("CellSpatialContraction-recovery=PASS full-schema-data-history-SQL-objects-restored case=" + fault);
		}
		// Final writer freeze: deliberate changes made by the prior Room-based writer are authoritative.
		Sql("UPDATE rooms SET X=18 WHERE Id=9000; DELETE FROM areas_rooms WHERE AreaId=9000 AND RoomId=9000; INSERT INTO areas_rooms VALUES(9000,9002);");
		Migrate(reconcile: true);
		Require(Scalar("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema=DATABASE() AND table_name IN('rooms','areas_rooms')") == 0, "Obsolete tables are removed.");
		Require(Scalar("SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='cells' AND column_name='RoomId'") == 0, "No live Room ownership column remains.");
		Require(Scalar("SELECT COUNT(*) FROM cells WHERE Id=8101 AND ZoneId=" + zoneId + " AND X=18 AND Y=-2 AND Z=4 AND UniqueName='Mirandola:Gate'") == 1, "Reconcile preserves real Cell ID/key and final owner metadata.");
		Require(Scalar("SELECT COUNT(*) FROM cellroomcontractionledger WHERE RoomId=9000 AND CellId=8101 AND X=18") == 1, "Final ledger preserves latest source metadata.");
		Require(Scalar("SELECT COUNT(*) FROM cellroomcontractionledger WHERE RoomId=9001 AND CellId IS NULL AND X=99 AND Warning IS NOT NULL") == 1, "Empty Room remains in provenance with warning.");
		Require(Scalar("SELECT COUNT(*) FROM areas_cells WHERE AreaId=9000 AND CellId=8102") == 1 && Scalar("SELECT COUNT(*) FROM areas_cells WHERE AreaId=9000 AND CellId=8101") == 0, "Reconcile reflects latest sole-child area mapping.");
		Require(Scalar("SELECT COUNT(*) FROM areas WHERE Id=9002") == 1, "Unrelated empty Area survives.");
		using (var c = database.OpenOwnedConnection())
		{
			Require(SpatialValuesEqual(provenance, CaptureSpatialValues(c, provenanceSchema)), "Initial expansion provenance remains immutable.");
			var retainedSchema = originalSchema.Where(x => x.Key is not ("rooms" or "areas_rooms"))
				.ToDictionary(x => x.Key, x => x.Key == "cells" ? x.Value.Where(v => v != "RoomId").ToArray() : x.Value);
			// Every original row and retained field is compared; source Room data is checked in both ledgers.
			var retainedExpected = originalValues.Where(x => x.Key is not ("rooms" or "areas_rooms" or "cells"))
				.ToDictionary(x => x.Key, x => x.Value);
			var retainedActual = CaptureSpatialValues(c, retainedSchema).Where(x => x.Key != "cells").ToDictionary(x => x.Key, x => x.Value);
			Require(SpatialValuesEqual(retainedExpected, retainedActual), "All original dependent rows, IDs and fields survive contraction.");
			Require(SpatialValuesEqual(originalRetainedCellValues, CaptureSpatialValues(c,
				new Dictionary<string,string[]> { ["cells"] = retainedSchema["cells"] })), "Every original retained Cell field and numeric ID survives.");
			Require(SpatialValuesEqual(objects, CaptureSpatialObjectDefinitions(c)), "Unrelated SQL objects remain unchanged.");
		}
		ValidateContractedSpatialRuntime(database, zoneId);
		Console.WriteLine("CellSpatialContraction-upgrade=PASS old-expanded-contracted real-IDs final-reconciliation initial-and-final-ledgers dependent-rows SQL-objects empty-Area empty-Room-warning");
	}

	private static Dictionary<string, string> CaptureSpatialObjectDefinitions(MySqlConnection connection)
	{
		var result = new Dictionary<string, string>(StringComparer.Ordinal);
		using var command = new MySqlCommand("SELECT CONCAT('view:',TABLE_NAME),VIEW_DEFINITION FROM information_schema.views WHERE table_schema=DATABASE() UNION ALL SELECT CONCAT('routine:',ROUTINE_NAME),ROUTINE_DEFINITION FROM information_schema.routines WHERE routine_schema=DATABASE() UNION ALL SELECT CONCAT('trigger:',TRIGGER_NAME),CONCAT(ACTION_TIMING,' ',EVENT_MANIPULATION,' ON ',EVENT_OBJECT_TABLE,' ',ACTION_ORIENTATION,' ',ACTION_STATEMENT) FROM information_schema.triggers WHERE trigger_schema=DATABASE() UNION ALL SELECT CONCAT('event:',EVENT_NAME),EVENT_DEFINITION FROM information_schema.events WHERE event_schema=DATABASE() ORDER BY 1", connection);
		using var reader = command.ExecuteReader();
		while (reader.Read()) result[reader.GetString(0)] = reader.IsDBNull(1) ? "NULL" : reader.GetString(1);
		return result;
	}

	private static void ValidateContractedSpatialRuntime(TestDatabase database, long zoneId)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var model = db.Cells.Include(x => x.CellOverlays).AsNoTracking().Single(x => x.Id == 8101);
		Require(model.ZoneId == zoneId && model.X == 18 && model.UniqueName == "Mirandola:Gate", "Contracted model hydrates final data.");
		// Use the same native Cell hydration path and frozen initial ledger, with final coordinates.
		ValidateExpandedSpatialRuntime(database, finalCoordinates: true);
	}

	private sealed class CellSpatialContractionFault(string phase) : DbCommandInterceptor
	{
		public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
		{
			if ((phase == "partial-ddl" && command.CommandText.Contains("DROP TABLE `Rooms`")) ||
				(phase == "before-history" && command.CommandText.Contains("INSERT INTO") && command.CommandText.Contains("__EFMigrationsHistory") && command.CommandText.Contains("CellSpatialContraction")))
				throw new InvalidOperationException("Fixture contraction failure: " + phase);
			return result;
		}
	}

	private static void QualifyContractedCellLifecycle(TestDatabase database, long sourceZoneId)
	{
		void Sql(string text) { using var c = database.OpenOwnedConnection(); using var cmd = new MySqlCommand(text, c); cmd.ExecuteNonQuery(); }
		long Scalar(string text) { using var c = database.OpenOwnedConnection(); using var cmd = new MySqlCommand(text, c); return Convert.ToInt64(cmd.ExecuteScalar()); }
		Sql($"UPDATE zones SET DefaultCellId=8101 WHERE Id={sourceZoneId}");
		using var db = NewIndependentContext(database.ConnectionString);
		var sourceModel = db.Zones.AsNoTracking().Single(x => x.Id == sourceZoneId);
		var newModel = new MudSharp.Models.Zone { Name = "Lifecycle destination", ShardId = sourceModel.ShardId };
		db.Zones.Add(newModel); db.SaveChanges();
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		world.SetupGet(x => x.SaveManager).Returns(new SaveManager());
		world.SetupGet(x => x.DefaultHooks).Returns(Array.Empty<IDefaultHook>());
		world.SetupGet(x => x.HearingProfiles).Returns(new All<IHearingProfile>());
		world.SetupGet(x => x.WeatherControllers).Returns(new All<MudSharp.Climate.IWeatherController>());
		world.SetupGet(x => x.Vehicles).Returns(new All<MudSharp.Vehicles.IVehicle>());
		var package = new Mock<ICellOverlayPackage>(); package.SetupGet(x => x.Id).Returns(9000); package.SetupGet(x => x.Name).Returns("Spatial fixture package"); package.SetupGet(x => x.RevisionNumber).Returns(0); package.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		world.Setup(x => x.CellOverlayPackages.Get(9000, 0)).Returns(package.Object);
		var terrain = new Mock<ITerrain>(); terrain.SetupGet(x => x.Id).Returns(9000); terrain.SetupGet(x => x.TerrainLayers).Returns(new[] { RoomLayer.GroundLevel }); world.Setup(x => x.Terrains.Get(9000)).Returns(terrain.Object);
		var shard = new Shard(db.Shards.AsNoTracking().Single(x => x.Id == sourceModel.ShardId), world.Object);
		world.SetupGet(x => x.Shards).Returns(new All<IShard> { shard });
		var source = new Zone(sourceModel, world.Object); var destination = new Zone(newModel, world.Object);
		var cells = new All<ICell>(); world.SetupGet(x => x.Cells).Returns(cells);
		world.Setup(x => x.Add(It.IsAny<ICell>())).Callback<ICell>(x => cells.Add(x));
		world.Setup(x => x.Destroy(It.IsAny<ICell>())).Callback<ICell>(x => { x.OwningZone.Unregister(x); cells.Remove(x); });
		world.SetupGet(x => x.ExitManager).Returns(new ExitManager(world.Object));
		foreach (var model in db.Cells.Include(x => x.CellOverlays).ThenInclude(x => x.CellOverlaysExits).AsNoTracking().OrderBy(x => x.Id).ToArray()) cells.Add(new Cell(model, source));
		var moved = (Cell)cells.Get(8101);
		Require(ReferenceEquals(source.DefaultCell, moved), "Explicit non-first default survives Cell loading order.");
		var clone = new Cell(package.Object, destination, moved, temporary: true);
		Require(clone.UniqueName is null && clone.StoredCoordinates == (0, 0, 0) && !clone.OwningAreas.Any(), "Native clone retains fresh-cell defaults.");
		Require(ReferenceEquals(destination.DefaultCell, clone) && destination.Changed, "First Cell selects a persisted default.");
		moved.SetNewZone(destination);
		using (new FMDB()) { moved.Save(); source.Save(); destination.Save(); FMDB.Context.SaveChanges(); }
		using (var reload = NewIndependentContext(database.ConnectionString))
		{
			Require(reload.Cells.AsNoTracking().Single(x => x.Id == moved.Id).ZoneId == destination.Id, "Rezone saves intrinsic owner.");
			Require(reload.Zones.AsNoTracking().Single(x => x.Id == source.Id).DefaultCellId == source.DefaultCell.Id, "Old owner persists its surviving default.");
			Require(reload.Zones.AsNoTracking().Single(x => x.Id == destination.Id).DefaultCellId == clone.Id, "First-cell default persists across reload.");
		}
		var area = new Area(clone, "Deletion fixture area");
		var exit = new Exit(world.Object, clone, moved, CardinalDirection.North, CardinalDirection.South, 1.0);
		((IEditableCellOverlay)clone.CurrentOverlay).AddExit(exit); ((IEditableCellOverlay)moved.CurrentOverlay).AddExit(exit);
		world.Object.ExitManager.UpdateCellOverlayExits(clone, clone.CurrentOverlay);
		world.Object.ExitManager.UpdateCellOverlayExits(moved, moved.CurrentOverlay);
		world.Object.SaveManager.Flush();
		Sql($"UPDATE characters SET Location={clone.Id} WHERE Id=(SELECT MIN(Id) FROM (SELECT Id FROM characters) fixture_characters)");
		clone.Destroy(moved);
		Require(!cells.Any(x => ReferenceEquals(x, clone)) && !destination.Cells.Any(x => ReferenceEquals(x, clone)) && !shard.Cells.Any(x => ReferenceEquals(x, clone)), "Deletion removes Cell registries.");
		Require(!area.Cells.Any() && ReferenceEquals(destination.DefaultCell, moved), "Deletion detaches Area and selects survivor default.");
		Require(Scalar($"SELECT COUNT(*) FROM cells WHERE Id={clone.Id}") == 0 && Scalar($"SELECT COUNT(*) FROM exits WHERE Id={exit.Id}") == 0 && Scalar($"SELECT COUNT(*) FROM areas_cells WHERE CellId={clone.Id}") == 0, "Direct Cell deletion persists Cell/exit/membership removal.");
		Require(Scalar($"SELECT COUNT(*) FROM areas WHERE Id={area.Id}") == 1, "Deletion preserves empty Area object.");
		Require(Scalar($"SELECT COUNT(*) FROM characters WHERE Location={moved.Id}") == 1, "Offline character survives at fallback.");
		Require(Scalar($"SELECT DefaultCellId FROM zones WHERE Id={destination.Id}") == moved.Id, "Deletion persists surviving default.");
		Require(Scalar("SELECT COUNT(*) FROM cellroomcontractionledger WHERE RoomId=9000 AND CellId=8101 AND X=18") == 1, "Historical provenance survives later rezone/deletion.");
		Console.WriteLine("CellSpatialContraction-lifecycle=PASS native-clone rezone-save-reload defaults direct-Cell-delete exits Areas registries offline-character-fallback provenance");
	}
}
