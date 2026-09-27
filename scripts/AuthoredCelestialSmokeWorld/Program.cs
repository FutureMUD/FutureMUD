#nullable enable

using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AuthoredCelestialSmokeWorld;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MySql.Data.MySqlClient;

// This verification harness never uses the interactive seeder's default connection or credentials.
// The orchestrator supplies an owned, freshly initialized MySQL instance and checks its data directory.
var connection = Environment.GetEnvironmentVariable("AUTHORED_SMOKE_CONNECTION") ?? throw new InvalidOperationException("Missing owned connection.");
var expectedData = Path.GetFullPath(args[0]).TrimEnd(Path.DirectorySeparatorChar);
var receiptPath = args[1];
var builder = new MySqlConnectionStringBuilder(connection);
var snapshotImport = args.Length == 4 && args[2] == "--snapshot-import-verify";
var expectedDatabase = snapshotImport ? "authored_celestial_snapshot_import" : "authored_celestial_smoke";
if (builder.Server != "127.0.0.1" || builder.Database != expectedDatabase || builder.UserID != "root" || builder.Password.Length != 0)
	throw new InvalidOperationException("Only the owned loopback test instance is allowed.");
using var sql = new MySqlConnection(connection);
sql.Open();
using (var identity = new MySqlCommand("SELECT @@datadir", sql))
{
	var actual = Path.GetFullPath((string)identity.ExecuteScalar()!).TrimEnd(Path.DirectorySeparatorChar);
	if (!actual.Equals(expectedData, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Data directory mismatch; refusing mutation.");
}
if (snapshotImport)
{
	PersistenceVerification.ImportSnapshot(connection, args[3], receiptPath);
	return 0;
}
if (args.Length == 3 && args[2] == "--persistence-upgrade")
{
	PersistenceVerification.Upgrade(connection, receiptPath);
	return 0;
}
if (args.Length == 3 && args[2] == "--persistence-reload")
{
	PersistenceVerification.Reload(connection, receiptPath);
	return 0;
}
var password = Environment.GetEnvironmentVariable("AUTHORED_SMOKE_PASSWORD") ?? throw new InvalidOperationException("Missing ephemeral test account password.");
var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseLazyLoadingProxies().UseMySql(connection, ServerVersion.AutoDetect(connection)).Options;
Func<FuturemudDatabaseContext> factory = () => new(options);
using (var db = factory()) db.Database.Migrate();
var assembly = typeof(CelestialSeeder).Assembly;
const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
var profiles = (IEnumerable)assembly.GetType("DatabaseSeeder.DebugSeederReplayProfiles")!.GetProperty("All", flags)!.GetValue(null)!;
var profile = profiles.Cast<object>().First();
object? Property(object value, string name) => value.GetType().GetProperty(name, flags)!.GetValue(value);
foreach (var step in (IEnumerable)Property(profile, "Steps")!)
{
	if ((Type)Property(step, "SeederType")! != typeof(CoreDataSeeder)) continue;
	foreach (var answer in (IEnumerable)Property(step, "Answers")!)
	{
		if ((string)Property(answer, "Id")! == "password") answer.GetType().GetProperty("Answer", flags)!.SetValue(answer, password);
	}
}
var catalogue = assembly.GetType("DatabaseSeeder.SeederCatalogue")!.GetMethod("GetEnabledSeeders", flags)!.Invoke(null, null);
var run = assembly.GetType("DatabaseSeeder.SeederReplayRunner")!.GetMethod("Run", flags)!;
object Execute() => run.Invoke(null, [profile, catalogue, factory, assembly.GetName().Version!, (Action<string>)Console.WriteLine])!;
var first = Execute();
bool Success(object result) => (bool)Property(result, "Success")!;
if (!Success(first))
{
	Console.Error.WriteLine($"Replay failed at {Property(first, "FailedSeeder")}: {Property(first, "Failure")}");
	Console.Error.WriteLine(Property(first, "Exception"));
	Console.Error.WriteLine(JsonSerializer.Serialize(Property(Property(first, "Validation")!, "Errors")));
	return 1;
}
string DatabaseDigest()
{
	using var command = new MySqlCommand("SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() ORDER BY TABLE_NAME", sql);
	var names = new List<string>();
	using (var reader = command.ExecuteReader()) while (reader.Read()) names.Add(reader.GetString(0));
	var text = new StringBuilder();
	foreach (var name in names)
	{
		using var checksum = new MySqlCommand($"CHECKSUM TABLE `{name.Replace("`", "``")}` EXTENDED", sql);
		using var reader = checksum.ExecuteReader(); reader.Read();
		text.Append(name).Append(':').Append(reader.GetValue(1)).AppendLine();
	}
	return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text.ToString())));
}
var before = DatabaseDigest();
var second = Execute();
var after = DatabaseDigest();
if (Success(second) || before != after || ((IEnumerable)Property(second, "CompletedSeeders")!).Cast<object>().Any())
	throw new InvalidOperationException("Nonblank replay refusal changed the database or executed a seeder.");
using (var db = factory())
{
	// Speed remains slow enough for deterministic builder operations. A smoke phase can change it explicitly.
	var receipt = new { profile = Property(profile, "Id"), firstRun = "PASS", completed = Property(first, "CompletedSeeders"),
		secondRun = "REFUSED_WITHOUT_MUTATION", digestBefore = before, digestAfter = after,
		celestials = db.Celestials.Select(x => new { x.Id, x.CelestialType }).ToArray(),
		accounts = db.Accounts.Count(), characters = db.Characters.Count(), cells = db.Cells.Count() };
	File.WriteAllText(receiptPath, JsonSerializer.Serialize(receipt, new JsonSerializerOptions { WriteIndented = true }));
}
Console.WriteLine("PASS: complete native replay and unchanged nonblank refusal.");
return 0;
