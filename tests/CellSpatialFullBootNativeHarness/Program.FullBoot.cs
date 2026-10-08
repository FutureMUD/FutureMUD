using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MudSharp.Database;
using MySql.Data.MySqlClient;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	internal static int RunRoomSpatialFullBoot(string[] args)
	{
		OwnedConnections.Install();
		if (args is ["--probe"]) return Probe();
		if (args is ["--regex-regression"]) return RunRoomSpatialRegexRegression();
		Require(args is ["--run"], "Full boot acceptance requires --run or --probe.");
		string Env(string key) => Environment.GetEnvironmentVariable(key) ?? throw new InvalidOperationException("Missing private acceptance input: " + key);
		var repository = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
		var workspace = Directory.GetParent(repository)!.FullName;
		var baseline = Path.GetFullPath(Env("FUTUREMUD_CELL_FULLBOOT_BASELINE"));
		var output = Path.GetFullPath(Env("FUTUREMUD_CELL_FULLBOOT_OUTPUT"));
		Require(baseline == Path.Combine(workspace, "fullboot-baseline") && File.Exists(Path.Combine(baseline, "MudSharpCore", "MudSharpCore.csproj")), "Historical baseline must be the isolated archive.");
		Require(output.StartsWith(Path.Combine(workspace, "fullboot-qualification") + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) && !Directory.Exists(output), "Acceptance output must be a new private workspace directory.");
		Directory.CreateDirectory(output);
		var runtime = Path.Combine(output, "runtime"); Directory.CreateDirectory(runtime);
		using var listener = new TcpListener(IPAddress.Loopback, 0); listener.Start(); var port = ((IPEndPoint)listener.LocalEndpoint).Port; listener.Stop();
		File.WriteAllText(Path.Combine(runtime, "Connection.config"), $"127.0.0.1\n{port}\n127.0.0.1,::1\n", Encoding.ASCII);
		using var database = TestDatabase.CreateFresh("futuremud_land_", historicalExpanded: true);
		ConfigureNativeDatabase(database.ConnectionString);
		using (var db = NewIndependentContext(database.ConnectionString))
			db.GetService<IMigrator>().Migrate(db.Database.GetMigrations().Single(x => x.EndsWith("_CellUniqueNames")));
		long Scalar(string sql) { using var c = database.OpenOwnedConnection(); return Convert.ToInt64(new MySqlCommand(sql, c).ExecuteScalar()); }
		void Sql(string sql) { using var c = database.OpenOwnedConnection(); new MySqlCommand(sql, c) { CommandTimeout = 180 }.ExecuteNonQuery(); }
		string token; using (var c = database.OpenOwnedConnection()) token = (string)new MySqlCommand("SELECT RunToken FROM __gathering_harness_ownership", c).ExecuteScalar()!;
		var processes = new List<object>();
		void Child(string phase, string binary, string? hook = null)
		{
			Require(File.Exists(binary) && (hook is null || File.Exists(hook)), "Missing frozen acceptance binary: " + phase);
			using (var verified = database.OpenOwnedConnection()) { }
			var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, CreateNoWindow = true, WorkingDirectory = runtime, RedirectStandardOutput = true, RedirectStandardError = true };
			start.ArgumentList.Add(binary);
			if (hook is not null) { start.ArgumentList.Add("MySql.Data.MySqlClient"); start.ArgumentList.Add(database.ConnectionString); }
			start.Environment["FUTUREMUD_CELL_FULLBOOT_CONNECTION"] = database.ConnectionString;
			start.Environment["FUTUREMUD_CELL_FULLBOOT_TOKEN"] = token;
			start.Environment["FUTUREMUD_CELL_FULLBOOT_PHASE"] = phase;
			start.Environment["FUTUREMUD_CELL_FULLBOOT_DESCRIPTOR"] = Path.Combine(output, "scenario.json");
			start.Environment["FUTUREMUD_CELL_FULLBOOT_RECEIPT"] = Path.Combine(output, phase + "-scenario.json");
			var ready = Path.Combine(runtime, ".acceptance-ready-" + Guid.NewGuid().ToString("N"));
			var readyToken = Guid.NewGuid().ToString("N");
			start.Environment["FUTUREMUD_CELL_FULLBOOT_READY_PATH"] = ready;
			start.Environment["FUTUREMUD_CELL_FULLBOOT_READY_TOKEN"] = readyToken;
			if (hook is null) start.Environment.Remove("DOTNET_STARTUP_HOOKS"); else start.Environment["DOTNET_STARTUP_HOOKS"] = hook;
			using var job = new PreparedBootProcessJob();
			using var process = Process.Start(start) ?? throw new InvalidOperationException("Unable to start acceptance child.");
			var stdout = process.StandardOutput.ReadToEndAsync(); var stderr = process.StandardError.ReadToEndAsync();
			var graceful = false;
			try
			{
				job.Attach(process);
				File.WriteAllText(ready, readyToken + ":" + process.Id);
				if (!process.WaitForExit(600_000)) throw new TimeoutException("Bounded normal entrypoint/replay deadline exceeded: " + phase);
				graceful = process.ExitCode == 0;
			}
			finally
			{
				if (!process.HasExited) { if (job.Attached) job.StopAndVerify(); else process.Kill(); }
				Require(process.WaitForExit(30_000), "Owned child did not stop before database disposal.");
				job.StopAndVerify();
				var text = (stdout.GetAwaiter().GetResult() + stderr.GetAwaiter().GetResult()).Replace(database.ConnectionString, "[REDACTED]").Replace(token, "[REDACTED]");
				File.WriteAllText(Path.Combine(output, phase + "-console.log"), text);
				File.Delete(ready);
				processes.Add(new { phase, process.Id, ExitCode = process.ExitCode, Graceful = graceful, OwnedJobActiveProcesses = 0, Binary = binary, BinarySha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(binary))), Hook = hook, HookSha256 = hook is null ? null : Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(hook))) });
				Console.WriteLine($"CellFullBoot-process phase={phase} exit={process.ExitCode} graceful={graceful} owned-job-active:0");
			}
			Require(graceful, "Acceptance child failed; retained console: " + phase);
			if (hook is not null)
			{
				using var receipt = JsonDocument.Parse(File.ReadAllText(Path.Combine(output, phase + "-scenario.json")));
				Require(receipt.RootElement.GetProperty("Status").GetString() == "PASS", "Scenario assertions failed: " + phase);
				var text = File.ReadAllText(Path.Combine(output, phase + "-console.log"));
				Require(text.Contains("Done Finalising Cells.") && text.Contains("MUD is now ready to connect") && !text.Contains("[FATAL"), "Normal full boot did not prove final cell/magic load: " + phase);
			}
		}
		Child("seed", Env("FUTUREMUD_CELL_FULLBOOT_SEEDER"));
		// All records below are deliberately generated from the genuine seeded world.
		// Copy every scalar column explicitly; source Room/Cell identities differ.
		void CloneRow(string table, string where, Dictionary<string, string> replacements)
		{
			using var c = database.OpenOwnedConnection();
			var columns = new List<string>();
			using (var cmd = new MySqlCommand($"SELECT COLUMN_NAME FROM information_schema.columns WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='{table}' ORDER BY ORDINAL_POSITION", c))
			using (var reader = cmd.ExecuteReader()) while (reader.Read()) columns.Add(reader.GetString(0));
			Require(columns.Count > 0 && replacements.Keys.All(columns.Contains), "Fixture clone schema mismatch: " + table);
			var select = columns.Select(x => replacements.GetValueOrDefault(x, "`" + x + "`"));
			Require(new MySqlCommand($"INSERT INTO `{table}` ({string.Join(',', columns.Select(x => "`" + x + "`"))}) SELECT {string.Join(',', select)} FROM `{table}` WHERE {where}", c).ExecuteNonQuery() == 1, "Fixture clone did not copy exactly one row: " + table);
		}
		var stockRoom = Scalar("SELECT MIN(Id) FROM cells"); var stockRoom = Scalar($"SELECT RoomId FROM cells WHERE Id={stockRoom}"); var zone = Scalar($"SELECT ZoneId FROM rooms WHERE Id={stockRoom}");
		CloneRow("zones", $"Id={zone}", new() { ["Id"] = "9000", ["Name"] = "'Qualification destination'", ["DefaultCellId"] = "NULL" });
		for (var i = 1; i <= 4; i++)
		{
			CloneRow("rooms", $"Id={stockRoom}", new() { ["Id"] = (9100 + i).ToString(), ["X"] = i <= 2 ? "17" : (17 + i).ToString(), ["Y"] = "-2", ["Z"] = "4" });
			CloneRow("cells", $"Id={stockRoom}", new() { ["Id"] = (8100 + i).ToString(), ["RoomId"] = (9100 + i).ToString(), ["CurrentOverlayId"] = "NULL", ["Temporary"] = i >= 3 ? "1" : "0", ["UniqueName"] = $"'acceptance_cell_{i}'", ["EffectData"] = "'<Effects/>'" });
			CloneRow("celloverlays", $"Id=(SELECT CurrentOverlayId FROM cells WHERE Id={stockRoom})", new() { ["Id"] = (8200 + i).ToString(), ["CellId"] = (8100 + i).ToString(), ["CellName"] = $"'Qualification cell {i}'" });
			Sql($"UPDATE cells SET CurrentOverlayId={8200 + i} WHERE Id={8100 + i}");
		}
		Sql($"UPDATE zones SET DefaultCellId=8102 WHERE Id={zone}; INSERT INTO areas(Id,Name) VALUES(9000,'Qualification main'),(9001,'Qualification overlap'),(9002,'Qualification dwelling'); INSERT INTO areas_rooms(AreaId,RoomId) VALUES(9000,9101),(9000,9102),(9001,9102),(9002,9103),(9002,9104)");
		Sql("INSERT INTO exits(Id,CellId1,CellId2,Direction1,Direction2,TimeMultiplier,AcceptsDoor) VALUES(9000,8101,8102,0,4,1,0),(9001,8103,8104,0,4,1,0),(9002,8103,8101,10,10,1,0); INSERT INTO celloverlays_exits(CellOverlayId,ExitId) VALUES(8201,9000),(8202,9000),(8203,9001),(8204,9001),(8203,9002),(8201,9002)");
		Sql("UPDATE exits SET Keywords1='outside',Keywords2='building',PrimaryKeyword1='outside',PrimaryKeyword2='building',Verb1='leave',Verb2='enter',InboundDescription1='from',InboundDescription2='from',OutboundDescription1='towards',OutboundDescription2='towards',OutboundTarget1='the outside',OutboundTarget2='the building',InboundTarget1='the building',InboundTarget2='the outside' WHERE Id=9002");
		Sql("UPDATE staticconfigurations SET Definition='<EmailServer><Version>2</Version><Enabled>false</Enabled></EmailServer>' WHERE SettingName='EmailServer'; UPDATE staticconfigurations SET Definition='false' WHERE SettingName='UseDiscordBot'; UPDATE staticconfigurations SET Definition='127.0.0.1' WHERE SettingName='DiscordBotIpAddress'; UPDATE staticconfigurations SET Definition='1' WHERE SettingName='DiscordBotPort'");
		PrepareFullBootPrototypes(database, CloneRow);
		var native = Path.Combine(repository, "tests", "CellSpatialFullBootNativeHarness");
		Child("prepare", Path.Combine(baseline, "MudSharpCore", "bin", "Debug", "net10.0", "MudSharp.dll"), Env("FUTUREMUD_CELL_FULLBOOT_LEGACY_HOOK"));
		if (Environment.GetEnvironmentVariable("FUTUREMUD_CELL_FULLBOOT_WARD_PROBE") == "1")
		{
			using (var c = database.OpenOwnedConnection()) File.WriteAllText(Path.Combine(output, "legacy-before-cell-effectdata.xml"), (string)new MySqlCommand("SELECT EffectData FROM cells WHERE Id=8101", c).ExecuteScalar()!);
			try { Child("legacy-read", Path.Combine(baseline, "MudSharpCore", "bin", "Debug", "net10.0", "MudSharp.dll"), Env("FUTUREMUD_CELL_FULLBOOT_LEGACY_HOOK")); }
			finally { using var c = database.OpenOwnedConnection(); File.WriteAllText(Path.Combine(output, "legacy-after-cell-effectdata.xml"), (string)new MySqlCommand("SELECT EffectData FROM cells WHERE Id=8101", c).ExecuteScalar()!); }
			return 0; // Attribution probe deliberately performs no spatial migration.
		}
		using var beforeConnection = database.OpenOwnedConnection();
		var schema = CaptureSpatialSchema(beforeConnection); var values = CaptureSpatialValues(beforeConnection, schema); var definitions = CaptureSpatialTableDefinitions(beforeConnection, schema);
		var historySchema = new Dictionary<string, string[]> { ["__efmigrationshistory"] = ["MigrationId", "ProductVersion"] };
		var history = CaptureSpatialValues(beforeConnection, historySchema);
		var retainedRoomsSchema = new Dictionary<string, string[]> { ["cells"] = schema["cells"].Where(x => x != "RoomId").ToArray() };
		var retainedRooms = CaptureSpatialValues(beforeConnection, retainedRoomsSchema); beforeConnection.Close();
		Dictionary<string, string> Files(string root) => Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).ToDictionary(x => Path.GetRelativePath(root, x), x => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(x))));
		void CopyFiles(string source, string destination)
		{
			Require(!Directory.Exists(destination), "Backup/restore requires a new owned file directory.");
			Directory.CreateDirectory(destination);
			foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
			{
				var target = Path.Combine(destination, Path.GetRelativePath(source, file)); Directory.CreateDirectory(Path.GetDirectoryName(target)!); File.Copy(file, target, overwrite: false);
			}
			Require(SpatialValuesEqual(Files(source), Files(destination)), "Private backup/restoration did not preserve exact file bytes.");
		}
		var oldBinaries = Path.Combine(baseline, "MudSharpCore", "bin", "Debug", "net10.0");
		var restoredBinaries = Path.Combine(output, "backup", "historical-server");
		var fileBackup = Path.Combine(output, "backup", "private-runtime-files");
		CopyFiles(oldBinaries, restoredBinaries); CopyFiles(runtime, fileBackup);
		var originalFileHashes = Files(runtime); var originalBinaryHashes = Files(oldBinaries);
		var service = new MySqlDatabaseBackupService(); string backup; using (var verified = database.OpenOwnedConnection()) backup = service.CreateBackup(database.ConnectionString, Path.Combine(output, "backup"));
		using (var verified = database.OpenOwnedConnection()) service.RestoreBackup(database.ConnectionString, backup);
		using (var serverPool = new MySqlConnection(new MySqlConnectionStringBuilder(database.ConnectionString) { Database = string.Empty }.ConnectionString)) { OwnedConnections.Validate("fullboot-restore-pool", serverPool, allowServer: true); MySqlConnection.ClearPool(serverPool); }
		using (var restored = database.OpenOwnedConnection())
		{
			var restoredSchema = CaptureSpatialSchema(restored);
			Require(schema.Count == restoredSchema.Count && schema.All(x => restoredSchema.TryGetValue(x.Key, out var v) && x.Value.SequenceEqual(v)), "Full seeded backup schema restoration failed.");
			Require(SpatialValuesEqual(values, CaptureSpatialValues(restored, schema)) && SpatialValuesEqual(definitions, CaptureSpatialTableDefinitions(restored, schema)), "Full seeded backup values/DDL restoration failed.");
			Require(SpatialValuesEqual(history, CaptureSpatialValues(restored, historySchema)), "Full seeded backup migration history restoration failed.");
		}
		var restoredFiles = Path.Combine(output, "restored-runtime"); CopyFiles(fileBackup, restoredFiles); runtime = restoredFiles;
		File.WriteAllText(Path.Combine(output, "backup-restoration.json"), JsonSerializer.Serialize(new { Status = "PASS", DatabaseValues = values, History = history, OriginalFiles = originalFileHashes, RestoredFiles = Files(runtime), OriginalBinaries = originalBinaryHashes, RestoredBinaries = Files(restoredBinaries), Backup = backup }, new JsonSerializerOptions { WriteIndented = true }));
		Child("restored", Path.Combine(restoredBinaries, "MudSharp.dll"), Env("FUTUREMUD_CELL_FULLBOOT_LEGACY_HOOK"));
		Console.WriteLine("CellFullBoot-backup-restore=PASS full seeded world exact schema/values/DDL/history; matching private files and historical binaries restored and normally booted; all writers stopped");
		// The restored historical server legitimately writes on graceful shutdown.
		// Freeze a fresh source baseline after that rehearsal for migration comparisons.
		using (var finalSource = database.OpenOwnedConnection()) { values = CaptureSpatialValues(finalSource, schema); retainedRooms = CaptureSpatialValues(finalSource, retainedRoomsSchema); }
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.Database.OpenConnection();
			db.Database.ExecuteSqlRaw("SET @FutureMUD_CellSpatialMaintenance=1; SET @FutureMUD_CellSpatialContractionMaintenance=1;");
			try { db.Database.Migrate(); }
			catch (Exception failure)
			{
				var diagnostics = new Dictionary<string, object?> { ["Failure"] = failure.ToString() };
				try
				{
					using var command = db.Database.GetDbConnection().CreateCommand();
					command.CommandText = "SELECT @fm_cell_spatial_query, @fm_cell_spatial_pattern, @@regexp_time_limit";
					using (var reader = command.ExecuteReader())
					{
						Require(reader.Read(), "Missing failed-session migration diagnostic.");
						diagnostics["CandidateQuery"] = reader.IsDBNull(0) ? null : reader.GetString(0);
						diagnostics["CandidatePattern"] = reader.IsDBNull(1) ? null : reader.GetString(1);
						diagnostics["RegexpTimeLimit"] = reader.GetValue(2);
					}
					diagnostics["RepresentativePayload"] = DiagnoseRegexPayload(database, output,
						diagnostics["CandidateQuery"] as string, diagnostics["CandidatePattern"] as string);
					using var failed = database.OpenOwnedConnection();
					diagnostics["SchemaAfterFailure"] = CaptureSpatialSchema(failed);
					diagnostics["HistoryAfterFailure"] = CaptureSpatialValues(failed, historySchema);
					diagnostics["OriginalColumnValuesAfterFailure"] = CaptureSpatialValues(failed, schema);
				}
				catch (Exception captureFailure) { diagnostics["DiagnosticCaptureFailure"] = captureFailure.ToString(); }
				File.WriteAllText(Path.Combine(output, "migration-failure.json"), JsonSerializer.Serialize(diagnostics, new JsonSerializerOptions { WriteIndented = true }));
				throw;
			}
		}
		Require(Scalar("SELECT COUNT(*) FROM information_schema.tables WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN('rooms','areas_rooms')") == 0, "Legacy tables survived contraction.");
		Require(Scalar("SELECT COUNT(*) FROM cells WHERE Id IN(8101,8102) AND ZoneId=" + zone + " AND X=17 AND Y=-2 AND Z=4") == 2, "Duplicate XYZ or numeric identity lost.");
		Require(Scalar("SELECT COUNT(*) FROM areas_cells WHERE (AreaId=9000 AND CellId IN(8101,8102)) OR (AreaId=9001 AND CellId=8102) OR (AreaId=9002 AND CellId IN(8103,8104))") == 5, "Unequal Room-to-Cell area remap lost membership.");
		var retainedSchema = schema.Where(x => x.Key is not ("rooms" or "areas_rooms" or "cells" or "__efmigrationshistory")).ToDictionary(x => x.Key, x => x.Value);
		using (var after = database.OpenOwnedConnection())
		{
			Require(SpatialValuesEqual(CaptureSpatialValues(after, retainedSchema), values.Where(x => retainedSchema.ContainsKey(x.Key)).ToDictionary(x => x.Key, x => x.Value)), "Migration changed retained old-world data.");
			Require(SpatialValuesEqual(retainedRooms, CaptureSpatialValues(after, retainedRoomsSchema)), "Migration changed retained Cell fields/identities/references.");
			File.WriteAllText(Path.Combine(output, "migration-postflight.json"), JsonSerializer.Serialize(new { Status = "PASS", OriginalSchema = schema, OriginalValues = values, OriginalHistory = history, RetainedRooms = retainedRooms, RetainedValues = CaptureSpatialValues(after, retainedSchema), CurrentSchema = CaptureSpatialSchema(after), CurrentHistory = CaptureSpatialValues(after, historySchema) }, new JsonSerializerOptions { WriteIndented = true }));
		}
		Console.WriteLine("CellFullBoot-migration-postflight=PASS retained old-world rows unequal identities duplicate XYZ areas defaults custody hosted magic preserved");
		var hook = Path.Combine(native, "StartupHook", "bin", "Debug", "net10.0", "CellSpatialFullBootStartupHook.dll");
		Child("operate", Path.Combine(repository, "MudSharpCore", "bin", "Debug", "net10.0", "MudSharp.dll"), hook);
		Child("cold", Path.Combine(repository, "MudSharpCore", "bin", "Debug", "net10.0", "MudSharp.dll"), hook);
		File.WriteAllText(Path.Combine(output, "fullboot-receipt.json"), JsonSerializer.Serialize(new { Status = "PASS", database.Name, Processes = processes, Backup = backup, BackupSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(backup))), WorldTables = schema.Count, NormalEntrypoint = true, RoomFinalization = true, ConfigurationSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(runtime, "Connection.config")))), WritersFrozenDuringMigration = true }, new JsonSerializerOptions { WriteIndented = true }));
		Console.WriteLine("CellFullBoot-completion=PASS genuine historical normal boot -> full restore -> frozen migration -> normal full boot lifecycle -> graceful flush/stop -> normal cold boot; exact owned cleanup follows");
		return 0;
	}
}
