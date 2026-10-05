#nullable enable
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MySqlConnector;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int PreparedSeederReplayNative()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.Database.Migrate(); Require(!db.Accounts.Any() && !db.SeederChoices.Any(), "Owned replay target is not blank.");
		}
		// Execute the actual internal Debug API and complete stock profile. Reflection keeps this
		// dedicated harness out of the production API and avoids editing shared friend/dispatch files.
		var assembly = typeof(ArmageddonMagicSeeder).Assembly;
		var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		var profiles = (IEnumerable)assembly.GetType("DatabaseSeeder.DebugSeederReplayProfiles", true)!.GetProperty("All", flags)!.GetValue(null)!;
		var profile = profiles.Cast<object>().Single(x => (string)x.GetType().GetProperty("Id")!.GetValue(x)! == "medieval-standard");
		var seeders = assembly.GetType("DatabaseSeeder.SeederCatalogue", true)!.GetMethod("GetEnabledSeeders", flags)!.Invoke(null, null)!;
		var run = assembly.GetType("DatabaseSeeder.SeederReplayRunner", true)!.GetMethod("Run", flags)!;
		Func<FuturemudDatabaseContext> contextFactory = () => NewPreparedReplayContext(database.ConnectionString);
		void Progress(string text)
		{
			Console.WriteLine("ARMPREP-replay-progress=" + text);
			if (text == "Running Human Seeder...") VerifyReplayColourLoading(database.ConnectionString);
		}
		object Execute() => run.Invoke(null, [profile, seeders, contextFactory, new Version(1, 0), (Action<string>)Progress])!;
		var first = Execute(); var firstType = first.GetType();
		var success = (bool)firstType.GetProperty("Success", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(first)!;
		Console.WriteLine("ARMPREP-replay-first=" + JsonSerializer.Serialize(new
		{
			Profile = "medieval-standard", Success = success,
			Completed = firstType.GetProperty("CompletedSeeders")!.GetValue(first),
			Failed = firstType.GetProperty("FailedSeeder")!.GetValue(first),
			Unstarted = firstType.GetProperty("UnstartedSeeders")!.GetValue(first),
			Failure = firstType.GetProperty("Failure")!.GetValue(first),
			Exception = (firstType.GetProperty("Exception")!.GetValue(first) as Exception)?.ToString()
		}));
		var before = PreparedDatabaseChecksum(database); var second = Execute(); var after = PreparedDatabaseChecksum(database);
		var failure = (string?)second.GetType().GetProperty("Failure")!.GetValue(second);
		Require(failure?.Contains("freshly migrated, unseeded", StringComparison.OrdinalIgnoreCase) == true && before == after, "Actual replay rerun did not refuse without mutation.");
		Console.WriteLine($"ARMPREP-replay-refusal=passed actual-Debug-runner all-table-content-sha256:{before}");
		if (!success)
		{
			Console.WriteLine("ARMPREP-replay-completion=BLOCKED actual first profile failure retained; earlier seeder commits preserved. No complete-profile certificate."); return 2;
		}
		using (var db = NewIndependentContext(database.ConnectionString)) Require(!db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package), "Standard decline profile installed magic content.");
		Console.WriteLine("ARMPREP-replay-completion=passed complete-medieval-profile optional-Armageddon-declined");
		RunPreparedReplayBoot(database, assembly);
		return 0;
	}
	private static FuturemudDatabaseContext NewPreparedReplayContext(string connectionString)
	{
		// Production replay enables proxies. Keep native/module contexts and their ownership
		// serializers unchanged; this context is only for the full stock replay workflow.
		using var candidate = new MySqlConnection(connectionString);
		OwnedConnections.Validate("prepared-replay-context-before-autodetect", candidate);
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseLazyLoadingProxies()
			.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
			.AddInterceptors(new FMDB.ValidatedConnectionInterceptor((boundary, connection) =>
				OwnedConnections.ValidateIndependentConnection(boundary, connection, connectionString)))
			.Options;
		return new FuturemudDatabaseContext(options);
	}
	private static void VerifyReplayColourLoading(string connectionString)
	{
		using var native = NewIndependentContext(connectionString);
		var unloaded = native.CharacteristicDefinitions.Single(x => x.Name == "Colour");
		var stored = native.CharacteristicValues.Count(x => x.DefinitionId == unloaded.Id);
		Require(stored > 0 && unloaded.CharacteristicValues.Count == 0,
			"Original no-proxy context did not reproduce the unloaded Core Colour navigation.");
		using var replay = NewPreparedReplayContext(connectionString);
		var loaded = replay.CharacteristicDefinitions.Single(x => x.Id == unloaded.Id);
		Require(loaded.CharacteristicValues.Count == stored,
			"Production-equivalent replay context did not load the stored Core Colour values.");
		Console.WriteLine($"ARMPREP-replay-colour-loading=passed definition:{loaded.Id} stored-values:{stored} no-proxy-navigation:0 replay-navigation:{loaded.CharacteristicValues.Count} no-inserted-values original-failing-characteristic-not-recorded");
	}
	private static void RunPreparedReplayBoot(TestDatabase database, Assembly assembly)
	{
		var suppliedScript = Environment.GetEnvironmentVariable("FUTUREMUD_PREPARED_REPLAY_BOOT_SCRIPT");
		if (string.IsNullOrWhiteSpace(suppliedScript)) return;
		var repository = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
		var laneRoot = Directory.GetParent(repository)!.FullName;
		var script = Path.Combine(laneRoot, "PreparedReplayBootSmoke.py");
		Require(Path.GetFullPath(suppliedScript) == script && File.Exists(script),
			"The optional boot handoff only accepts this lane's explicit smoke script.");
		Require(File.ReadAllBytes(script).SequenceEqual(File.ReadAllBytes(Path.Combine(repository,
			"tests", "ArmageddonPreparedSeederNativeHarness", "PreparedReplayBootSmoke.py"))),
			"The executor-local boot script must match the fingerprinted lane source exactly.");
		var suppliedOutput = Environment.GetEnvironmentVariable("FUTUREMUD_PREPARED_REPLAY_BOOT_OUTPUT")
			?? throw new InvalidOperationException("Missing owned boot evidence directory.");
		var output = Path.GetFullPath(suppliedOutput);
		Require(Path.GetDirectoryName(output) == laneRoot && Path.GetFileName(output).StartsWith("prepared-replay-boot-native", StringComparison.Ordinal),
			"Boot output must have its own lane-local evidence namespace.");
		var password = assembly.GetType("DatabaseSeeder.DebugSeederReplayProfiles", true)!
			.GetField("DebugPassword", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!.GetRawConstantValue() as string
			?? throw new InvalidOperationException("Missing disposable replay account password.");
		using (var connection = database.OpenOwnedConnection()) { }
		var start = new ProcessStartInfo("python")
		{
			UseShellExecute = false, CreateNoWindow = true, WorkingDirectory = laneRoot,
			RedirectStandardOutput = true, RedirectStandardError = true
		};
		start.ArgumentList.Add("-B"); start.ArgumentList.Add(script);
		start.Environment["FUTUREMUD_PREPARED_REPLAY_BOOT_CONNECTION"] = database.ConnectionString;
		start.Environment["FUTUREMUD_PREPARED_REPLAY_LOGIN_PASSWORD"] = password;
		start.Environment["FUTUREMUD_PREPARED_REPLAY_BOOT_OUTPUT"] = output;
		start.Environment["FUTUREMUD_PREPARED_REPLAY_REPOSITORY"] = repository;
		var ready = Path.Combine(laneRoot, "prepared-replay-boot-parent-ready_" + Guid.NewGuid().ToString("N") + ".txt");
		var token = Guid.NewGuid().ToString("N");
		start.Environment["FUTUREMUD_PREPARED_REPLAY_PARENT_READY_PATH"] = ready;
		start.Environment["FUTUREMUD_PREPARED_REPLAY_PARENT_READY_TOKEN"] = token;
		using var processes = new PreparedBootProcessJob();
		using var child = Process.Start(start) ?? throw new InvalidOperationException("Unable to start owned boot child.");
		var stdout = child.StandardOutput.ReadToEndAsync(); var stderr = child.StandardError.ReadToEndAsync();
		try
		{
			processes.Attach(child);
			// The child waits before SQL/MUD startup until attachment is confirmed. This
			// closes the process scheduling race; every subsequently spawned MUD is in our job.
			using (var readyReceipt = new FileStream(ready, FileMode.CreateNew, FileAccess.Write, FileShare.None))
				readyReceipt.Write(Encoding.UTF8.GetBytes(token + ":" + child.Id));
			if (!child.WaitForExit(660_000)) throw new TimeoutException("Owned boot child exceeded its 660-second handoff deadline.");
		}
		finally
		{
			if (!child.HasExited)
			{
				if (processes.Attached) processes.StopAndVerify();
				else child.Kill(); // Attachment failed; the child never received its startup receipt.
			}
			Require(child.WaitForExit(30_000), "Owned boot child did not stop before database disposal.");
			processes.StopAndVerify();
			File.Delete(ready);
			Console.Write(stdout.GetAwaiter().GetResult().Replace(database.ConnectionString, "[REDACTED]").Replace(password, "[REDACTED]"));
			Console.Error.Write(stderr.GetAwaiter().GetResult().Replace(database.ConnectionString, "[REDACTED]").Replace(password, "[REDACTED]"));
			Console.WriteLine($"ARMPREP-boot-child=stopped pid:{child.Id} exit:{child.ExitCode} before-database-disposal");
			Console.WriteLine("ARMPREP-boot-owned-tree=stopped parent-job-active-processes:0 before-database-disposal");
		}
		Require(child.ExitCode == 0, "Owned boot child failed; see retained first-failure evidence.");
		using var receipt = JsonDocument.Parse(File.ReadAllText(Path.Combine(output, "boot-latest.json")));
		Require(receipt.RootElement.GetProperty("status").GetString() == "PASS" &&
			receipt.RootElement.GetProperty("owned_mud_processes_stopped").GetBoolean(),
			"Boot receipt did not verify both owned MUD process trees stopped before database disposal.");
		Console.WriteLine("ARMPREP-replay-boot=passed native-login-LOOK-save-graceful-shutdown-cold-restart-login owned-processes-stopped");
	}
	private static string PreparedDatabaseChecksum(TestDatabase database)
	{
		using var connection = new MySqlConnection(database.ConnectionString);
		OwnedConnections.Validate("prepared-checksum-before-connect", connection); connection.Open();
		OwnedConnections.Validate("prepared-checksum-connected", connection);
		using var command = connection.CreateCommand(); command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @name ORDER BY TABLE_NAME";
		command.Parameters.AddWithValue("@name", database.Name); var tables = new List<string>();
		using (var reader = command.ExecuteReader()) while (reader.Read()) tables.Add(reader.GetString(0));
		using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
		foreach (var table in tables)
		{
			command.Parameters.Clear(); command.CommandText = "SELECT * FROM `" + table.Replace("`", "``") + "`";
			var rows = new List<string>();
			using (var reader = command.ExecuteReader())
			while (reader.Read())
			{
				var values = new object[reader.FieldCount]; reader.GetValues(values); rows.Add(JsonSerializer.Serialize(values));
			}
			hash.AppendData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { table, rows = rows.Order(StringComparer.Ordinal).ToArray() })));
		}
		return Convert.ToHexString(hash.GetHashAndReset());
	}
}
