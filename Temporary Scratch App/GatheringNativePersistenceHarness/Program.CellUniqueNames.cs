using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Moq;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Events.Hooks;
using MySql.Data.MySqlClient;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunCellUniqueNames()
	{
		using var database = TestDatabase.CreateFresh(historicalExpanded: true);
		ConfigureNativeDatabase(database.ConnectionString);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var migrations = db.Database.GetMigrations().ToArray();
			var identifierIndex = Array.FindIndex(migrations, x => x.EndsWith("_CellUniqueNames", StringComparison.Ordinal));
			Require(identifierIndex > 0, "Stage1 migration must exist.");
			db.GetService<IMigrator>().Migrate(migrations[Array.FindIndex(migrations, x => x.EndsWith("_CellUniqueNames", StringComparison.Ordinal)) - 1]); // Only this newly created, marker-verified database.
		}
		var fixture = FixtureSeed.Create(database, "cell_identity", existingWound: true);
		using var connection = database.OpenOwnedConnection();
		void Sql(string sql) { using var command = new MySqlCommand(sql, connection); command.ExecuteNonQuery(); }
		long Scalar(string sql) { using var command = new MySqlCommand(sql, connection); return Convert.ToInt64(command.ExecuteScalar()); }
		var roomId = Scalar($"SELECT RoomId FROM cells WHERE Id={fixture.CellId}");
		var zoneId = Scalar($"SELECT ZoneId FROM rooms WHERE Id={roomId}");
		Sql($"INSERT INTO rooms(Id,ZoneId,X,Y,Z) VALUES(9000,{zoneId},0,0,0),(9001,{zoneId},0,0,0);");
		Sql($"INSERT INTO cells(Id,RoomId,EffectData,Temporary) VALUES(8000,{roomId},'<Effects/>',0),(8001,{roomId},'<Effects/>',1),(8002,9000,'<Effects/>',0);");
		Sql("INSERT INTO areas(Id,Name) VALUES(9000,'Legacy area'); INSERT INTO areas_rooms(AreaId,RoomId) VALUES(9000,9000),(9000,9001);");
		Sql("INSERT INTO editableitems(Id,RevisionNumber,RevisionStatus,BuilderAccountId,BuilderDate) VALUES(9000,0,0,0,'2026-01-01');");
		Sql("INSERT INTO celloverlaypackages(Id,RevisionNumber,Name,EditableItemId) VALUES(9000,0,'Legacy overlay package',9000);");
		Sql("INSERT INTO terrains(Id,Name,MovementRate,TerrainBehaviourMode,HideDifficulty,SpotDifficulty,StaminaCost,ForagableProfileId,InfectionType) VALUES(9000,'Test terrain',1,'outdoors',0,0,1,0,0);");
		foreach (var id in new[] { fixture.CellId, 8000L, 8001L, 8002L })
		{
			Sql($"INSERT INTO celloverlays(Id,Name,CellName,CellDescription,CellOverlayPackageId,CellOverlayPackageRevisionNumber,CellId,TerrainId,OutdoorsType,AddedLight,SafeQuit) VALUES({id},'Legacy overlay','Legacy cell {id}','Preserved description',9000,0,{id},9000,0,0,1); UPDATE cells SET CurrentOverlayId={id} WHERE Id={id};");
		}
		Sql("INSERT INTO exits(Id,CellId1,CellId2,Direction1,Direction2,TimeMultiplier,AcceptsDoor) VALUES(9000,8000,8002,1,3,1,0); INSERT INTO celloverlays_exits(CellOverlayId,ExitId) VALUES(8000,9000),(8002,9000);");
		var before = CaptureLegacyCellData(connection);
		using (var db = NewIndependentContext(database.ConnectionString)) db.GetService<IMigrator>().Migrate(db.Database.GetMigrations().Single(x => x.EndsWith("_CellUniqueNames", StringComparison.Ordinal)));
		var after = CaptureLegacyCellData(connection);
		Require(before.Count == after.Count && before.All(x => after.TryGetValue(x.Key, out var hash) && hash == x.Value), "Upgrade must preserve every existing table row and column (except migration history)." );
		Require(Scalar("SELECT COUNT(*) FROM cells WHERE UniqueName IS NOT NULL") == 0, "No identifier backfill is allowed.");
		Require(Scalar($"SELECT COUNT(*) FROM cells WHERE RoomId={roomId}") == 3 && Scalar("SELECT COUNT(*) FROM cells WHERE RoomId=9001") == 0, "Multiple-cell and empty rooms must survive unchanged.");
		Require(Scalar("SELECT COUNT(*) FROM information_schema.statistics WHERE table_schema=DATABASE() AND table_name='cells' AND index_name='IX_Cells_UniqueName' AND non_unique=1") == 1, "Index must be nonunique.");
		Console.WriteLine($"CellUniqueNames-upgrade=PASS tables={before.Count} all-legacy-data-unchanged multiple-cells empty-room duplicate-coordinates distinct-IDs area-exit-overlay-character-wound-links no-backfill");

		// Qualify the reversible additive identifier migration while the historical schema
		// still exists. Contraction deliberately has no automatic downgrade.
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var migrations = db.Database.GetMigrations().ToArray();
			db.GetService<IMigrator>().Migrate(migrations[Array.FindIndex(migrations, x => x.EndsWith("_CellUniqueNames", StringComparison.Ordinal)) - 1]);
		}
		Require(Scalar("SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='cells' AND column_name='UniqueName'") == 0, "Historical identifier downgrade removes only additive storage/index.");
		var downgraded = CaptureLegacyCellData(connection);
		Require(before.Count == downgraded.Count && before.All(x => downgraded.TryGetValue(x.Key, out var hash) && hash == x.Value), "Historical identifier downgrade preserves every legacy key/value, including multi-cell and empty Rooms.");
		Console.WriteLine("CellUniqueNames-historical-downgrade=PASS original-keys-values multi-cell empty-parent");
		using (var db = NewIndependentContext(database.ConnectionString)) db.GetService<IMigrator>().Migrate(db.Database.GetMigrations().Single(x => x.EndsWith("_CellUniqueNames", StringComparison.Ordinal)));

		// Historical stage1 assertions above deliberately preserve legacy multi-cell Rooms.
		// Explicit test-fixture disposition before stage2 expansion; never repair a user world.
		Sql($"INSERT INTO rooms(Id,ZoneId,X,Y,Z) VALUES(9002,{zoneId},0,0,0),(9003,{zoneId},0,0,0); UPDATE cells SET RoomId=9002 WHERE Id=8000; UPDATE cells SET RoomId=9003 WHERE Id=8001; DELETE FROM areas_rooms WHERE RoomId=9001;");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.Database.OpenConnection(); db.Database.ExecuteSqlRaw("SET @FutureMUD_CellSpatialMaintenance=1; SET @FutureMUD_CellSpatialContractionMaintenance=1;"); db.Database.Migrate();
		}
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var cells = new All<ICell>();
		world.SetupGet(x => x.Cells).Returns(cells);
		world.SetupGet(x => x.SaveManager).Returns(new SaveManager());
		world.SetupGet(x => x.DefaultHooks).Returns(Array.Empty<IDefaultHook>());
		var package = new Mock<ICellOverlayPackage>();
		package.SetupGet(x => x.Id).Returns(9000);
		package.SetupGet(x => x.Name).Returns("Legacy overlay package");
		package.SetupGet(x => x.RevisionNumber).Returns(0);
		package.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		world.Setup(x => x.CellOverlayPackages.Get(9000, 0)).Returns(package.Object);
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(x => x.Id).Returns(9000);
		world.Setup(x => x.Terrains.Get(9000)).Returns(terrain.Object);
		world.SetupGet(x => x.HearingProfiles).Returns(new All<IHearingProfile>());
		var zone = new Mock<IZone> { DefaultValue = DefaultValue.Mock };
		zone.SetupGet(x => x.Gameworld).Returns(world.Object);
		zone.SetupGet(x => x.Id).Returns(zoneId);
		Cell Load(long id)
		{
			using var db = NewIndependentContext(database.ConnectionString);
			var model = db.Cells.Include(x => x.CellOverlays).AsNoTracking().Single(x => x.Id == id);
				return new Cell(model, zone.Object);
		}
		void Save(Cell cell) { using var scope = new FMDB(); cell.Save(); FMDB.Context.SaveChanges(); }
		var first = Load(8000); var second = Load(8001); cells.Add(first); cells.Add(second);
		Require(first.TrySetUniqueName("  Église:North Gate  ", out _) && first.UniqueName == "Église:North Gate", "Trim/preserve key.");
		Save(first);
		Require(Load(first.Id).UniqueName == "Église:North Gate", "Native Cell.Save and fresh EF hydration must preserve the key.");
		Require(!second.TrySetUniqueName("église:north gate", out _) && second.UniqueName is null, "Case-insensitive collision must refuse without mutation.");
		Require(first.TrySetUniqueName("ÉGLISE:North Gate", out _), "Case-only rename of the same cell must be allowed.");
		Save(first); Require(Load(first.Id).UniqueName == "ÉGLISE:North Gate", "Case-only rename must retain its chosen casing through save/reload.");
		Require(first.TrySetUniqueName("Renamed:Gate", out _) && cells.FindByUniqueName("Église:North Gate") is null, "Rename creates no alias.");
		Save(first); Require(Load(first.Id).UniqueName == "Renamed:Gate", "Rename must persist.");
		Require(first.TrySetUniqueName("Gate😀", out _), "Supplementary Unicode keys must be accepted.");
		Save(first); Require(Load(first.Id).UniqueName == "Gate😀", "Accepted supplementary Unicode must round-trip without substitution.");
		var clone = new Cell(package.Object, zone.Object, first);
		Require(clone.Id != first.Id && clone.UniqueName is null && Load(clone.Id).UniqueName is null, "Persistent template clones have a new ID and no copied key.");
		Require(first.TrySetUniqueName(null, out _) && second.TrySetUniqueName("  ", out _), "Blank keys can repeat.");
		Save(first); Save(second); Require(Load(first.Id).UniqueName is null && Load(second.Id).UniqueName is null, "Clear must persist as null.");
		Sql("UPDATE cells SET UniqueName='Gate' WHERE Id=8000; UPDATE cells SET UniqueName='gate' WHERE Id=8001;");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var rows = db.Cells.AsNoTracking().ToArray();
			try { typeof(Cell).GetMethod("ValidatePersistedUniqueNames", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.Invoke(null, [rows]); throw new Exception("Duplicate load was accepted."); }
			catch (System.Reflection.TargetInvocationException e) when (e.InnerException is InvalidOperationException && e.InnerException.Message.Contains("8000") && e.InnerException.Message.Contains("8001")) { }
		}
		Require(Scalar("SELECT COUNT(*) FROM cells WHERE UniqueName IN ('Gate','gate')") == 2, "Validation may not repair duplicate persisted data.");
		Sql("UPDATE cells SET UniqueName=NULL;"); // Explicit disposal-fixture disposition after asserting refusal.
		Require(Scalar($"SELECT COUNT(*) FROM cells WHERE Id IN ({fixture.CellId},8000,8001,8002,{clone.Id})") == 5, "Current runtime preserves legacy IDs and the new clone.");
		Console.WriteLine("CellUniqueNames-runtime=PASS contracted-schema native-save fresh-reload rename clear blank collision clone duplicate-load-refusal explicit-fixture-disposition");
		return 0;
	}

	private static Dictionary<string, string> CaptureLegacyCellData(MySqlConnection connection)
	{
		var tables = new Dictionary<string, List<string>>(StringComparer.Ordinal);
		using (var command = new MySqlCommand("SELECT TABLE_NAME,COLUMN_NAME FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name NOT IN ('__efmigrationshistory','__gathering_harness_ownership') AND NOT(table_name='cells' AND column_name='UniqueName') ORDER BY TABLE_NAME,ORDINAL_POSITION", connection))
		using (var reader = command.ExecuteReader())
			while (reader.Read())
			{
				var table = reader.GetString(0); if (!tables.TryGetValue(table, out var columns)) tables[table] = columns = [];
				columns.Add(reader.GetString(1));
			}
		var hashes = new Dictionary<string, string>(StringComparer.Ordinal);
		foreach (var (table, columns) in tables)
		{
			using var command = new MySqlCommand($"SELECT {string.Join(',', columns.Select(x => $"`{x.Replace("`", "``")}`"))} FROM `{table.Replace("`", "``")}`", connection);
			using var reader = command.ExecuteReader(); var rows = new List<string>();
			while (reader.Read())
			{
				var values = new string?[columns.Count];
				for (var i = 0; i < values.Length; i++) values[i] = reader.IsDBNull(i) ? null : reader.GetValue(i) switch
				{
					byte[] bytes => Convert.ToHexString(bytes), DateTime time => time.ToString("O", CultureInfo.InvariantCulture),
					IFormattable value => value.ToString(null, CultureInfo.InvariantCulture), var value => value.ToString()
				};
				rows.Add(JsonSerializer.Serialize(values));
			}
			rows.Sort(StringComparer.Ordinal);
			hashes[table] = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(rows))));
		}
		return hashes;
	}
}
