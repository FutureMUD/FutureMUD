#nullable enable

using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Moq;
using MudSharp.Construction;
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
	private static int RunRoomSpatialExpansion(bool contract = false)
	{
		using var database = TestDatabase.CreateFresh(historicalExpanded: true);
		ConfigureNativeDatabase(database.ConnectionString);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var migrations = db.Database.GetMigrations().ToArray();
			Require(migrations.Any(x => x.EndsWith("_CellSpatialExpansion")), "Historical expansion migration must exist.");
			db.GetService<IMigrator>().Migrate(migrations.Single(x => x.EndsWith("_CellUniqueNames")));
		}
		var fixture = FixtureSeed.Create(database, "cell_spatial", existingWound: true);
		void Sql(string text) { using var c = database.OpenOwnedConnection(); using var command = new MySqlCommand(text, c); command.ExecuteNonQuery(); }
		long Scalar(string text) { using var c = database.OpenOwnedConnection(); using var command = new MySqlCommand(text, c); return Convert.ToInt64(command.ExecuteScalar()); }
		var sourceRoom = Scalar($"SELECT RoomId FROM cells WHERE Id={fixture.RoomId}");
		var zone = Scalar($"SELECT ZoneId FROM rooms WHERE Id={sourceRoom}");
		Sql($"INSERT INTO rooms(Id,ZoneId,X,Y,Z) VALUES(9000,{zone},17,-2,4),(9001,{zone},99,88,77),(9002,{zone},17,-2,4);");
		Sql("INSERT INTO cells(Id,RoomId,EffectData,Temporary,UniqueName) VALUES(8101,9000,'<Effects/>',0,'Mirandola:Gate'),(8102,9002,'<Effects/>',0,NULL);");
		Sql("INSERT INTO areas(Id,Name) VALUES(9000,'First area'),(9001,'Overlapping area'),(9002,'Unrelated empty area'); INSERT INTO areas_rooms(AreaId,RoomId) VALUES(9000,9000),(9001,9000),(9001,9002);");
		Sql("INSERT INTO editableitems(Id,RevisionNumber,RevisionStatus,BuilderAccountId,BuilderDate) VALUES(9000,0,0,0,'2026-01-01'); INSERT INTO celloverlaypackages(Id,RevisionNumber,Name,EditableItemId) VALUES(9000,0,'Spatial fixture package',9000);");
		Sql("INSERT INTO terrains(Id,Name,MovementRate,TerrainBehaviourMode,HideDifficulty,SpotDifficulty,StaminaCost,ForagableProfileId,InfectionType) VALUES(9000,'Test terrain',1,'outdoors',0,0,1,0,0);");
		foreach (var id in new[] { fixture.RoomId, 8101L, 8102L })
			Sql($"INSERT INTO celloverlays(Id,Name,CellName,CellDescription,CellOverlayPackageId,CellOverlayPackageRevisionNumber,CellId,TerrainId,OutdoorsType,AddedLight,SafeQuit) VALUES({id},'Fixture overlay','Fixture cell {id}','Preserved description',9000,0,{id},9000,0,0,1); UPDATE cells SET CurrentOverlayId={id} WHERE Id={id};");
		Sql("INSERT INTO exits(Id,CellId1,CellId2,Direction1,Direction2,TimeMultiplier,AcceptsDoor) VALUES(9000,8101,8102,1,3,1,0); INSERT INTO celloverlays_exits(CellOverlayId,ExitId) VALUES(8101,9000),(8102,9000);");
		Sql($"UPDATE zones SET DefaultCellId={fixture.RoomId} WHERE Id={zone};");
		// Existing dependent graphs, not just empty tables: independent physical instance,
		// vehicle interior/occupancy, item containment/body custody, route geometry, track and environment state.
		Sql("INSERT INTO materials(Id,Name,MaterialDescription,Density,Organic,Type,ThermalConductivity,ElectricalConductivity,SpecificHeatCapacity) VALUES(9000,'Fixture solid','Fixture material',1000,0,0,1,1,1);");
		Sql("INSERT INTO gameitemprotos(Id,RevisionNumber,Name,Keywords,MaterialId,EditableItemId,Size,Weight,MorphTimeSeconds,ShortDescription,FullDescription) VALUES(9000,0,'Fixture item','fixture',9000,9000,1,1,0,'a fixture item','A fixture item.');");
		Sql("INSERT INTO gameitems(Id,Quality,GameItemProtoId,GameItemProtoRevision,RoomLayer,MaterialId,Size,PositionModifier,EffectData,RoutePosition) VALUES(9000,5,9000,0,0,9000,1,0,'<Effects/>',42.125),(9001,5,9000,0,0,9000,1,0,'<Effects/>',NULL),(9002,5,9000,0,0,9000,1,0,'<Effects/>',NULL); UPDATE gameitems SET ContainerId=9000 WHERE Id=9001; INSERT INTO cells_gameitems VALUES(8101,9000);");
		Sql($"INSERT INTO bodies_gameitems(BodyId,GameItemId,EquippedOrder) VALUES({fixture.BodyId},9002,1);");
		Sql("INSERT INTO routecells(CellId,LengthMetres,DefaultPositionMetres,PositiveDirectionName,NegativeDirectionName,MetresPerRoomEquivalent,TopologyVersion) VALUES(8101,100,50,'north','south',10,7);");
		Sql($"INSERT INTO characterinstances(Id,CharacterId,BodyId,InstanceName,InstanceKind,ControlPolicy,DeathPolicy,PerceptionPolicy,PersistencePolicy,LocationId,RoomLayer,PositionId,PositionModifier,State,Status,IsPrimary,IsEmbodied,IsControllable,CreatedDateTime,EffectData) VALUES(9000,{fixture.CharacterId},{fixture.BodyId},'Fixture physical instance',0,0,0,0,0,8102,0,1,0,1,0,1,1,1,'2026-01-01','<Effects/>'); UPDATE characters SET Location=8102 WHERE Id={fixture.CharacterId};");
		Sql("INSERT INTO vehicleprotos(Id,RevisionNumber,EditableItemId,Name,Description,VehicleScale) VALUES(9000,0,9000,'Fixture vehicle','Fixture vehicle',2); INSERT INTO vehiclecompartmentprotos(Id,VehicleProtoId,VehicleProtoRevision,Name,Description,DisplayOrder,InteriorTerrainId) VALUES(9000,9000,0,'Cabin','Fixture cabin',0,9000); INSERT INTO vehicleoccupantslotprotos(Id,VehicleProtoId,VehicleProtoRevision,VehicleCompartmentProtoId,Name,SlotType,Capacity,RequiredForMovement) VALUES(9000,9000,0,9000,'Passenger',0,1,0);");
		Sql($"INSERT INTO vehicles(Id,VehicleProtoId,VehicleProtoRevision,Name,LocationType,CurrentCellId,CurrentRoomLayer,MovementStatus,CreatedDateTime) VALUES(9000,9000,0,'Fixture vehicle',1,{fixture.RoomId},0,0,'2026-01-01'); INSERT INTO vehiclecompartments(Id,VehicleId,VehicleCompartmentProtoId,Name,InteriorCellId) VALUES(9000,9000,9000,'Cabin',8102); UPDATE cells SET HostedVehicleId=9000,HostedVehicleCompartmentId=9000 WHERE Id=8102; INSERT INTO vehicleoccupancies(Id,VehicleId,CharacterId,CharacterInstanceId,VehicleOccupantSlotProtoId,IsController) VALUES(9000,9000,{fixture.CharacterId},9000,9000,0);");
		var bodyPrototype = Scalar($"SELECT BodyPrototypeID FROM bodies WHERE Id={fixture.BodyId}");
		Sql($"INSERT INTO tracks(Id,CharacterId,BodyPrototypeId,CellId,RoomLayer,FromDirectionExitId,TrackCircumstances,ExertionLevel,TrackIntensityVisual,TrackIntensityOlfactory,TurnedAround,RoutePosition,RouteDirection) VALUES(9000,{fixture.CharacterId},{bodyPrototype},8101,0,9000,0,0,0.75,0.5,0,42.125,1);");
		Sql($"INSERT INTO cells_magicresources(CellId,MagicResourceId,Amount) VALUES(8102,{fixture.ResourceId},12.5); INSERT INTO cellenvironmentalstates(CellId,SchemaVersion,Revision,ScarDamage,RecentPressure,PressureHalfLifeSeconds) VALUES(8102,1,7,2.5,4.25,3600);");
		using var beforeConnection = database.OpenOwnedConnection();
		var schema = CaptureSpatialSchema(beforeConnection);
		var before = CaptureSpatialValues(beforeConnection, schema);
		var retainedRoomSchema = new Dictionary<string,string[]> { ["cells"] = schema["cells"].Where(x => x != "RoomId").ToArray() };
		var retainedRoomValues = CaptureSpatialValues(beforeConnection, retainedRoomSchema);
		var definitions = CaptureSpatialTableDefinitions(beforeConnection, schema);
		var historySchema = new Dictionary<string,string[]> { ["__efmigrationshistory"] = ["MigrationId", "ProductVersion"] };
		var originalHistory = CaptureSpatialValues(beforeConnection, historySchema);
		beforeConnection.Close();
		var backupDirectory = Path.Combine(Path.GetTempPath(), "futuremud-cell-spatial-backup_" + Guid.NewGuid().ToString("N"));
		var backupService = new MySqlDatabaseBackupService();
		string backup;
		using (var verified = database.OpenOwnedConnection()) backup = backupService.CreateBackup(database.ConnectionString, backupDirectory);
		void Restore()
		{
			using (var verified = database.OpenOwnedConnection()) backupService.RestoreBackup(database.ConnectionString, backup);
			// Restore's USE changes a pooled server connection's current database. Clear only this
			// process's exact owned server pool before any subsequent server ownership check.
			var serverBuilder = new MySqlConnectionStringBuilder(database.ConnectionString) { Database=string.Empty };
			using (var candidate = new MySqlConnection(serverBuilder.ConnectionString))
			{
				OwnedConnections.Validate("restore-owned-server-pool", candidate, allowServer:true);
				MySqlConnection.ClearPool(candidate);
			}
			using var restored = database.OpenOwnedConnection();
			var actualSchema = CaptureSpatialSchema(restored);
			Require(schema.Count == actualSchema.Count && schema.All(x => actualSchema.TryGetValue(x.Key, out var v) && x.Value.SequenceEqual(v)), "Restore must recover the entire original table/column structure.");
			Require(SpatialValuesEqual(before, CaptureSpatialValues(restored, schema)), "Backup restore must recover every old key and value.");
			Require(SpatialValuesEqual(definitions, CaptureSpatialTableDefinitions(restored, schema)), "Restore must preserve exact column definitions, indexes, foreign keys, defaults, collations and counters.");
			Require(SpatialValuesEqual(originalHistory, CaptureSpatialValues(restored, historySchema)), "Restore must preserve the entire original migration history.");
			Require(Scalar("SELECT COUNT(*) FROM __efmigrationshistory WHERE MigrationId LIKE '%CellSpatialExpansion'") == 0, "Restore must recover old migration history.");
			Require(Scalar("SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema=DATABASE() AND routine_name='fm_cell_spatial_preflight_20261006143539'") == 0, "Full restore must remove the reserved refusal routine.");
		}
		void Migrate(bool optIn = true, bool fault = false, bool afterCopyFault = false)
		{
			using var db = NewIndependentContext(database.ConnectionString, fault || afterCopyFault ? new RoomSpatialCopyFault(afterCopyFault) : null);
			db.Database.OpenConnection();
			if (optIn) db.Database.ExecuteSqlRaw("SET @FutureMUD_CellSpatialMaintenance=1;");
			db.GetService<IMigrator>().Migrate(db.Database.GetMigrations().Single(x => x.EndsWith("_CellSpatialExpansion")));
		}
		void Refuse(string label, string change, string expected, bool optIn = true)
		{
			if (change.Length > 0) Sql(change);
			using var c = database.OpenOwnedConnection();
			var stateSchema = CaptureSpatialSchema(c); var state = CaptureSpatialValues(c, stateSchema);
			var stateDefinitions = CaptureSpatialTableDefinitions(c, stateSchema);
			try { Migrate(optIn); throw new InvalidOperationException("Preflight unexpectedly accepted " + label); }
			catch (MySqlConnector.MySqlException ex) when (ex.Message.Contains(expected, StringComparison.OrdinalIgnoreCase)) { }
			Require(Scalar("SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='cells' AND column_name='ZoneId'") == 0, "Refusal must precede expansion DDL: " + label);
			Require(SpatialValuesEqual(state, CaptureSpatialValues(c, stateSchema)), "Refusal must not change any fixture key/value: " + label);
			Require(SpatialValuesEqual(stateDefinitions, CaptureSpatialTableDefinitions(c, stateSchema)), "Refusal must not change any game table definition: " + label);
			Require(Scalar("SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema=DATABASE() AND routine_name='fm_cell_spatial_preflight_20261006143539'") == 1, "A refused guard leaves its documented reserved routine; no schema-clean claim is allowed.");
			Console.WriteLine("CellSpatialExpansion-refusal=PASS reserved-guard-routine=retained game-tables-unchanged case=" + label);
			c.Close(); Restore();
		}
		// The contraction lane still upgrades a populated historical world and proves its
		// original values. The unchanged expansion refusal/recovery suite has its own lane.
		if (!contract)
		{
		Refuse("maintenance-opt-in", "", "maintenance session", false);
		Refuse("preexisting-routine", "CREATE PROCEDURE fm_cell_spatial_preflight_20261006143539() SELECT 'preexisting fixture routine';", "already exists");
		Refuse("multiple-children", "INSERT INTO cells(Id,RoomId,EffectData) VALUES(8103,9000,'<Effects/>');", "multiple Cells");
		Refuse("orphan-cell", "SET FOREIGN_KEY_CHECKS=0; UPDATE cells SET RoomId=999999 WHERE Id=8101; SET FOREIGN_KEY_CHECKS=1;", "orphan Cell");
		Refuse("orphan-zone", "SET FOREIGN_KEY_CHECKS=0; UPDATE rooms SET ZoneId=999999 WHERE Id=9000; SET FOREIGN_KEY_CHECKS=1;", "orphan Room.ZoneId");
		Refuse("orphan-shard", $"SET FOREIGN_KEY_CHECKS=0; UPDATE zones SET ShardId=999999 WHERE Id={zone}; SET FOREIGN_KEY_CHECKS=1;", "orphan Zone.ShardId");
		Refuse("orphan-area", "SET FOREIGN_KEY_CHECKS=0; UPDATE areas_rooms SET AreaId=999999 WHERE AreaId=9000; SET FOREIGN_KEY_CHECKS=1;", "orphan Area");
		Refuse("foreign-overlay", "UPDATE cells SET CurrentOverlayId=8102 WHERE Id=8101;", "foreign current overlay");
		Refuse("referenced-empty-room", "INSERT INTO areas_rooms(AreaId,RoomId) VALUES(9002,9001);", "empty Room");
		Refuse("unknown-room-foreign-key", "CREATE TABLE fixture_room_extension(Id bigint PRIMARY KEY,RoomRef bigint,FOREIGN KEY(RoomRef) REFERENCES rooms(Id));", "unknown foreign key");
		Refuse("unknown-room-id-column", "CREATE TABLE fixture_room_id_extension(Id bigint PRIMARY KEY,RoomId bigint);", "unclassified RoomId");
		Refuse("typed-room-reference", $"UPDATE characters SET PositionTargetType='Room',PositionTargetId=9001 WHERE Id={fixture.CharacterId};", "Room reference");
		Refuse("xml-room-reference", "UPDATE cells SET EffectData='<Effects><Effect><AnchorType>Room</AnchorType><AnchorId>9000</AnchorId></Effect></Effects>' WHERE Id=8101;", "Room reference");
		Refuse("xml-attribute-reference", "UPDATE cells SET EffectData='<Token TargetType=\"Room\" TargetId=\"9000\" />' WHERE Id=8101;", "Room reference");
		Refuse("json-room-reference", "CREATE TABLE fixture_serialized_reference(Id bigint PRIMARY KEY,Definition json); INSERT INTO fixture_serialized_reference VALUES(1,'{\"AnchorType\":\"Room\",\"AnchorId\":9001}');", "Room reference");
		try { Migrate(fault: true); throw new Exception("Copy fault was not injected."); }
		catch (InvalidOperationException ex) when (ex.Message == "Fixture failure after expansion DDL, before data copy") { }
		Require(Scalar("SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='cells' AND column_name='ZoneId'") == 1, "MySQL expansion DDL persists after failure; transaction rollback is not recovery.");
		Require(Scalar("SELECT COUNT(*) FROM __efmigrationshistory WHERE MigrationId LIKE '%CellSpatialExpansion'") == 0, "Failed expansion is not recorded as applied.");
		Restore();
		Console.WriteLine("CellSpatialExpansion-recovery=PASS full-backup-restore after-partial-DDL all-original-schema-keys-values-history");
		try { Migrate(afterCopyFault:true); throw new Exception("History fault was not injected."); }
		catch (InvalidOperationException ex) when (ex.Message == "Fixture failure after committed data copy, before migration history") { }
		Require(Scalar("SELECT COUNT(*) FROM cells WHERE Id=8101 AND ZoneId IS NOT NULL AND X=17") == 1, "Committed copy persists after the history-boundary fault.");
		Require(Scalar("SELECT COUNT(*) FROM __efmigrationshistory WHERE MigrationId LIKE '%CellSpatialExpansion'") == 0, "History-boundary failure must remain unapplied.");
		Restore();
		Console.WriteLine("CellSpatialExpansion-recovery=PASS full-backup-restore after-committed-copy-before-history exact-schema-keys-values-history");
		}
		Migrate();
		using (var c = database.OpenOwnedConnection()) Require(SpatialValuesEqual(before, CaptureSpatialValues(c, schema)), "Expansion must preserve every original key and value, including all Cell dependents and UniqueName.");
		Require(Scalar("SELECT COUNT(*) FROM cells c JOIN rooms r ON c.RoomId=r.Id WHERE NOT(c.ZoneId<=>r.ZoneId) OR NOT(c.X<=>r.X) OR NOT(c.Y<=>r.Y) OR NOT(c.Z<=>r.Z)") == 0, "Every cell's afterimage must equal its real parent's metadata.");
		Require(Scalar("SELECT COUNT(*) FROM cellroommigrationledger") == Scalar("SELECT COUNT(*) FROM rooms"), "Every original Room must have provenance.");
		Require(Scalar("SELECT COUNT(*) FROM cellroommigrationledger m LEFT JOIN rooms r ON r.Id=m.RoomId LEFT JOIN cells c ON c.RoomId=r.Id WHERE r.Id IS NULL OR NOT(m.CellId<=>c.Id) OR m.ZoneId<>r.ZoneId OR m.X<>r.X OR m.Y<>r.Y OR m.Z<>r.Z") == 0, "Every ledger entry must preserve exact source values and child ID.");
		Require(Scalar("SELECT COUNT(*) FROM areas_cells") == Scalar("SELECT COUNT(*) FROM areas_rooms") && Scalar("SELECT COUNT(*) FROM cellroomareamigrationledger") == Scalar("SELECT COUNT(*) FROM areas_rooms"), "Membership cardinalities must match exactly.");
		Require(Scalar("SELECT COUNT(*) FROM areas_rooms a JOIN cells c ON c.RoomId=a.RoomId LEFT JOIN areas_cells n ON n.AreaId=a.AreaId AND n.CellId=c.Id LEFT JOIN cellroomareamigrationledger m ON m.AreaId=a.AreaId AND m.RoomId=a.RoomId AND m.CellId=c.Id WHERE n.CellId IS NULL OR m.CellId IS NULL") == 0, "Every membership must map through the real sole child.");
		Require(Scalar("SELECT COUNT(*) FROM cells WHERE Id IN(8101,8102) AND ZoneId=" + zone + " AND X=17 AND Y=-2 AND Z=4") == 2, "Copy unequal IDs and duplicate coordinates without deduplication.");
		Require(Scalar("SELECT COUNT(*) FROM cellroommigrationledger WHERE RoomId=9000 AND CellId=8101") == 1, "Ledger uses the real Room-to-Cell mapping.");
		Require(Scalar("SELECT COUNT(*) FROM cellroommigrationledger WHERE RoomId=9001 AND CellId IS NULL AND X=99 AND Warning IS NOT NULL") == 1, "Empty Room metadata and removal warning must be retained.");
		Require(Scalar("SELECT COUNT(*) FROM areas_cells WHERE (AreaId=9000 AND CellId=8101) OR (AreaId=9001 AND CellId IN(8101,8102))") == 3, "Every area relationship must map through the sole child.");
		Require(Scalar("SELECT COUNT(*) FROM areas WHERE Id=9002") == 1, "Unrelated Area objects must survive.");
		ValidateExpandedSpatialRuntime(database);
		var start = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet") { UseShellExecute=false, RedirectStandardOutput=true, RedirectStandardError=true, CreateNoWindow=true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--cell-spatial-expansion-reader"); start.ArgumentList.Add(database.Name);
		using (var reader = Process.Start(start)!)
		{
			var output = reader.StandardOutput.ReadToEndAsync(); var error = reader.StandardError.ReadToEndAsync();
			if (!reader.WaitForExit(60000)) { reader.Kill(entireProcessTree:true); reader.WaitForExit(10000); throw new TimeoutException("Cold reader exceeded 60 seconds; owned reader terminated."); }
			Require(reader.ExitCode==0, "Cold reader failed: " + error.GetAwaiter().GetResult()); Console.Write(output.GetAwaiter().GetResult());
		}
		Console.WriteLine($"CellSpatialExpansion-upgrade=PASS oldTables={before.Count} all-original-keys-values unequal-IDs duplicate-XYZ overlaps empty-ledger first-load cold-process");
		if (contract)
		{
			QualifyRoomSpatialContraction(database, schema, before, retainedRoomValues, zone);
			var cold = new ProcessStartInfo("dotnet") { UseShellExecute=false, RedirectStandardOutput=true, RedirectStandardError=true, CreateNoWindow=true };
			cold.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); cold.ArgumentList.Add("--cell-spatial-contraction-reader"); cold.ArgumentList.Add(database.Name);
			using var child = Process.Start(cold)!; var stdout = child.StandardOutput.ReadToEndAsync(); var stderr = child.StandardError.ReadToEndAsync();
			if (!child.WaitForExit(60000)) { child.Kill(true); throw new TimeoutException("Contracted reader exceeded 60 seconds."); }
			Console.Write(stdout.GetAwaiter().GetResult()); Require(child.ExitCode==0, stderr.GetAwaiter().GetResult());
			QualifyContractedRoomLifecycle(database, zone);
		}
		// Full restore recovers the original historical world and matching migration history.
		Restore();
		Console.WriteLine(contract
			? "CellSpatialContraction-restored=PASS original-schema-data-history"
			: "CellSpatialExpansion-restored=PASS checkpoint=expansion-only contraction=NOT_RUN");
		return 0;
	}

	private static int ReadRoomSpatialExpansion(string name, bool contracted = false)
	{
		using var database = TestDatabase.OpenExistingOwned(name);
		ConfigureNativeDatabase(database.ConnectionString);
		ValidateExpandedSpatialRuntime(database, contracted);
		Console.WriteLine((contracted ? "CellSpatialContraction" : "CellSpatialExpansion") + "-cold-reader=PASS independent-process native-Cell-hydration stored-zone-XYZ-key-identity");
		return 0;
	}

	private static void ValidateExpandedSpatialRuntime(TestDatabase database, bool finalCoordinates = false)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var model = db.Rooms.Include(x => x.RoomOverlays).AsNoTracking().Single(x => x.Id == 8101);
		var ledger = db.RoomSpatialMigrationLedgers.AsNoTracking().Single(x => x.LegacyRoomId == 9000);
		Require(model.Id==ledger.RoomId && model.ZoneId==ledger.ZoneId && model.X==(finalCoordinates ? 18 : 17) && model.Y==-2 && model.Z==4 && model.UniqueName=="Mirandola:Gate", "Fresh model must hydrate copied metadata and stable identity.");
		var world = new Mock<IFuturemud> { DefaultValue=DefaultValue.Mock };
		world.SetupGet(x => x.SaveManager).Returns(new SaveManager()); world.SetupGet(x => x.DefaultHooks).Returns(Array.Empty<IDefaultHook>());
		world.SetupGet(x => x.HearingProfiles).Returns(new All<IHearingProfile>());
		world.SetupGet(x => x.WeatherControllers).Returns(new All<MudSharp.Climate.IWeatherController>());
		world.SetupGet(x => x.Vehicles).Returns(new All<MudSharp.Vehicles.IVehicle>());
		var package = new Mock<IRoomOverlayPackage>(); package.SetupGet(x=>x.Id).Returns(9000); package.SetupGet(x=>x.Name).Returns("Spatial fixture package"); package.SetupGet(x=>x.RevisionNumber).Returns(0); package.SetupGet(x=>x.Status).Returns(RevisionStatus.Current);
		world.Setup(x=>x.RoomOverlayPackages.Get(9000,0)).Returns(package.Object);
		var terrain = new Mock<ITerrain>(); terrain.SetupGet(x=>x.Id).Returns(9000); world.Setup(x=>x.Terrains.Get(9000)).Returns(terrain.Object);
		var zone = new Mock<IZone> { DefaultValue=DefaultValue.Mock }; zone.SetupGet(x=>x.Id).Returns(ledger.ZoneId); zone.SetupGet(x=>x.Gameworld).Returns(world.Object);
		var room = new Room(model,zone.Object);
		var rooms = new All<IRoom>(); rooms.Add(room);
		var secondModel = db.Rooms.Include(x => x.RoomOverlays).AsNoTracking().Single(x => x.Id == 8102);
		var second = new Room(secondModel, zone.Object); rooms.Add(second);
		world.SetupGet(x => x.Rooms).Returns(rooms);
		var areas = db.Areas.Include(x => x.AreasRooms).AsNoTracking().Where(x => x.Id >= 9000).ToList()
			.Select(x => new Area(x, world.Object)).ToArray();
		Require(room.OwningAreas.Count() == (finalCoordinates ? 1 : 2) && second.OwningAreas.Count() == (finalCoordinates ? 2 : 1), "Native Area hydration preserves exact final memberships.");
		Require(!areas.Single(x => x.Id == 9002).Rooms.Any(), "Native hydration retains the empty Area object.");
		Require(room.Id==8101 && room.UniqueName=="Mirandola:Gate" && room.Zone.Id==ledger.ZoneId && room.X==(finalCoordinates ? 18 : 17) && room.Y==-2 && room.Z==4, "Direct runtime hydrates copied metadata without a Room owner.");
	}

	private static Dictionary<string,string[]> CaptureSpatialSchema(MySqlConnection connection)
	{
		var tables = new Dictionary<string,List<string>>(StringComparer.Ordinal);
		using var command = new MySqlCommand("SELECT TABLE_NAME,COLUMN_NAME FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name NOT IN('__efmigrationshistory','__gathering_harness_ownership') ORDER BY TABLE_NAME,ORDINAL_POSITION",connection);
		using var reader = command.ExecuteReader();
		while(reader.Read()) { var table=reader.GetString(0); if(!tables.TryGetValue(table,out var columns)) tables[table]=columns=[]; columns.Add(reader.GetString(1)); }
		return tables.ToDictionary(x=>x.Key,x=>x.Value.ToArray(),StringComparer.Ordinal);
	}

	private static Dictionary<string,string> CaptureSpatialTableDefinitions(MySqlConnection connection,IReadOnlyDictionary<string,string[]> schema)
	{
		var definitions = new Dictionary<string,string>(StringComparer.Ordinal);
		foreach (var table in schema.Keys)
		{
			using var command = new MySqlCommand($"SHOW CREATE TABLE `{table.Replace("`","``")}`",connection);
			using var reader = command.ExecuteReader(); Require(reader.Read(),"Table definition must exist: " + table);
			definitions[table]=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(reader.GetString(1))));
		}
		return definitions;
	}

	private static Dictionary<string,string> CaptureSpatialValues(MySqlConnection connection,IReadOnlyDictionary<string,string[]> schema)
	{
		var hashes = new Dictionary<string,string>(StringComparer.Ordinal);
		foreach(var (table,columns) in schema)
		{
			using var command = new MySqlCommand($"SELECT {string.Join(',',columns.Select(x=>$"`{x.Replace("`","``")}`"))} FROM `{table.Replace("`","``")}`",connection);
			using var reader = command.ExecuteReader(); var rows = new List<string>();
			while(reader.Read())
			{
				var values = new string?[columns.Length];
				for(var i=0;i<values.Length;i++) values[i]=reader.IsDBNull(i)?null:reader.GetValue(i) switch { byte[] b=>Convert.ToHexString(b),DateTime t=>t.ToString("O",CultureInfo.InvariantCulture),IFormattable v=>v.ToString(null,CultureInfo.InvariantCulture),var v=>v.ToString() };
				rows.Add(JsonSerializer.Serialize(values));
			}
			rows.Sort(StringComparer.Ordinal); hashes[table]=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(rows))));
		}
		return hashes;
	}
	private static bool SpatialValuesEqual(IReadOnlyDictionary<string,string> before,IReadOnlyDictionary<string,string> after) => before.Count==after.Count && before.All(x=>after.TryGetValue(x.Key,out var value)&&value==x.Value);

	private sealed class RoomSpatialCopyFault(bool afterCopy) : DbCommandInterceptor
	{
		public override InterceptionResult<int> NonQueryExecuting(DbCommand command,CommandEventData eventData,InterceptionResult<int> result)
		{
			if(!afterCopy && command.CommandText.Contains("START TRANSACTION;") && command.CommandText.Contains("INSERT INTO `CellRoomMigrationLedger`")) throw new InvalidOperationException("Fixture failure after expansion DDL, before data copy");
			if(afterCopy && command.CommandText.Contains("INSERT INTO") && command.CommandText.Contains("__EFMigrationsHistory") && command.CommandText.Contains("CellSpatialExpansion")) throw new InvalidOperationException("Fixture failure after committed data copy, before migration history");
			return result;
		}
	}
}
