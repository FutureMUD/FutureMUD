#nullable enable
using System.Collections;
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
		Func<FuturemudDatabaseContext> contextFactory = () => NewIndependentContext(database.ConnectionString);
		object Execute() => run.Invoke(null, [profile, seeders, contextFactory, new Version(1, 0), (Action<string>)(text => Console.WriteLine("ARMPREP-replay-progress=" + text))])!;
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
		Console.WriteLine("ARMPREP-replay-completion=passed complete-medieval-profile optional-Armageddon-declined"); return 0;
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
