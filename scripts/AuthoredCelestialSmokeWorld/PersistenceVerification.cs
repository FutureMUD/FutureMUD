#nullable enable

extern alias EngineCompiler;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using MySql.Data.MySqlClient;
using EngineCompiler::MudSharp.Celestial.Authored;

namespace AuthoredCelestialSmokeWorld;

/// <summary>Runs only after Program verifies the owned instance and database identity.</summary>
internal static class PersistenceVerification
{
	private sealed record PreservedRow(long Id, string Type, long ClockId, int Minutes, int Year, int Bump, string DefinitionHash);
	private sealed record DenseRow(long Id, int Samples, int SourceBytes, string DefinitionHash);
	private sealed record Receipt(string Status, string Migration, string BeforeType, string AfterType,
		PreservedRow[] ExistingRows, DenseRow[] DenseRows, bool ReloadedAfterRestart, string[] AppliedMigrations);
	private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
	private static FuturemudDatabaseContext Context(string connection) => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseMySql(connection, ServerVersion.AutoDetect(connection)).Options);
	private static string Hash(string text) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)));
	private static PreservedRow[] Rows(FuturemudDatabaseContext db) => db.Celestials.AsNoTracking().OrderBy(x => x.Id)
		.AsEnumerable().Select(x => new PreservedRow(x.Id, x.CelestialType, x.FeedClockId, x.Minutes, x.CelestialYear,
			x.LastYearBump, Hash(x.Definition))).ToArray();
	private static void Require(bool condition, string message)
	{
		if (!condition) throw new InvalidOperationException(message);
	}
	private static string ColumnType(MySqlConnection sql)
	{
		using var command = new MySqlCommand("SELECT DATA_TYPE FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='celestials' AND COLUMN_NAME='Definition'", sql);
		return (string)command.ExecuteScalar()!;
	}
	private static Dictionary<long, string> StoredHashes(MySqlConnection sql)
	{
		using var command = new MySqlCommand("SELECT Id, SHA2(CAST(Definition AS BINARY),256) FROM celestials ORDER BY Id", sql);
		using var reader = command.ExecuteReader();
		var result = new Dictionary<long, string>();
		while (reader.Read()) result.Add(reader.GetInt64(0), reader.GetString(1));
		return result;
	}

	public static void Upgrade(string connection, string receiptPath)
	{
		using var sql = new MySqlConnection(connection);
		sql.Open();
		var beforeType = ColumnType(sql);
		Require(beforeType == "text", "Upgrade proof requires the original TEXT schema; refusing an already upgraded target.");
		using var db = Context(connection);
		var before = Rows(db);
		Require(before.Length > 0 && db.Celestials.Any(x => x.Definition.StartsWith("<")) &&
			db.Celestials.Any(x => x.Definition.StartsWith("{")), "Populated legacy XML and authored JSON rows are required.");
		var hashes = StoredHashes(sql);
		var service = new EfDatabaseMigrationService();
		var pending = service.GetPendingMigrations(connection);
		Require(pending.Count is 1 or 2 && pending[^1].EndsWith("_WidenCelestialDefinition", StringComparison.Ordinal) &&
			pending.Take(pending.Count - 1).All(x => x.EndsWith("_LandRejuvenationTreatments", StringComparison.Ordinal)),
			"Expected the celestial widening migration and at most the preceding base-branch land treatment migration.");
		new MySqlDatabaseBackupService().CreateBackup(connection, Path.Combine(Path.GetDirectoryName(receiptPath)!, "backup"));
		File.WriteAllText(receiptPath + ".before.json", JsonSerializer.Serialize(before, JsonOptions));
		service.ApplyMigrations(connection, pending);
		var afterType = ColumnType(sql);
		Require(afterType == "longtext", "Definition must be LONGTEXT after the production migration service runs.");
		Require(before.SequenceEqual(Rows(db)), "Existing celestial row fields changed during upgrade.");
		Require(hashes.OrderBy(x => x.Key).SequenceEqual(StoredHashes(sql).OrderBy(x => x.Key)),
			"Stored definition bytes changed during upgrade.");
		var calendar = db.Calendars.AsNoTracking().OrderBy(x => x.Id).First();
		var rows = new List<DenseRow>();
		foreach (var samples in new[] { 1440, 40000, 40320 })
		{
			var definition = AuthoredCelestialPresets.Create("ScriptedSun", calendar.Id, 60, 24);
			definition.Name = $"dense persistence {samples}";
			definition.Path.Mode = AuthoredPathMode.Dense;
			definition.Path.Period = samples;
			definition.Path.Keys = Enumerable.Range(0, samples)
				.Select(i => new PositionKey(i, i % 360, 60 * Math.Sin(i * 2 * Math.PI / samples))).ToList();
			var compiled = new CompiledAuthoredCelestial(definition, 60, 60, 24);
			var source = compiled.Serialize();
			var bytes = Encoding.UTF8.GetByteCount(source);
			Require(bytes > 65535 && bytes < new AuthoredCelestialLimits().SourceBytes, "Dense fixture must exceed TEXT and fit the default budget.");
			var row = new Celestial { CelestialType = definition.Kind.ToString(), FeedClockId = calendar.FeedClockId, Definition = source };
			db.Celestials.Add(row);
			db.SaveChanges();
			rows.Add(new DenseRow(row.Id, samples, bytes, Hash(source)));
		}
		var receipt = new Receipt("PASS", pending[^1], beforeType, afterType, before, rows.ToArray(), false, pending.ToArray());
		Verify(connection, receipt);
		File.WriteAllText(receiptPath, JsonSerializer.Serialize(receipt, JsonOptions));
		Console.WriteLine($"PASS: {before.Length} existing rows preserved; dense sizes: {string.Join(", ", rows.Select(x => $"{x.Samples} samples = {x.SourceBytes} bytes"))}.");
	}

	public static void Reload(string connection, string receiptPath)
	{
		var receipt = JsonSerializer.Deserialize<Receipt>(File.ReadAllText(receiptPath), JsonOptions)!;
		Verify(connection, receipt);
		File.WriteAllText(receiptPath, JsonSerializer.Serialize(receipt with { ReloadedAfterRestart = true }, JsonOptions));
		Console.WriteLine("PASS: existing and dense rows reloaded with matching bytes and numerical evaluation after MySQL restart.");
	}

	public static void ImportSnapshot(string connection, string snapshotPath, string receiptPath)
	{
		using var sql = new MySqlConnection(connection);
		sql.Open();
		using (var command = new MySqlCommand("SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE()", sql))
			Require(Convert.ToInt64(command.ExecuteScalar()) == 0, "Snapshot import requires an empty owned database.");
		new DatabaseUpgradeCoordinator().ImportBlankDatabaseSnapshot(connection, snapshotPath, "__FUTUREMUD_DATABASE__");
		using var db = Context(connection);
		Require(ColumnType(sql) == "longtext", "Imported snapshot must contain LONGTEXT.");
		Require(!db.Database.GetPendingMigrations().Any(), "Imported snapshot is missing migrations.");
		Require(!db.Celestials.Any(), "The blank snapshot must not contain celestial fixture data.");
		File.WriteAllText(receiptPath, JsonSerializer.Serialize(new
		{
			status = "PASS", columnType = ColumnType(sql), latestMigration = db.Database.GetAppliedMigrations().Last(),
			pendingMigrations = 0, celestialRows = 0, snapshotSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(snapshotPath)))
		}, JsonOptions));
		Console.WriteLine("PASS: production snapshot importer restores LONGTEXT and the complete migration history into an empty owned database.");
	}

	private static void Verify(string connection, Receipt receipt)
	{
		using var db = Context(connection);
		var existingIds = receipt.ExistingRows.Select(x => x.Id).ToHashSet();
		Require(receipt.ExistingRows.SequenceEqual(Rows(db).Where(x => existingIds.Contains(x.Id))), "Existing rows differ on reload.");
		foreach (var expected in receipt.DenseRows)
		{
			var row = db.Celestials.AsNoTracking().Single(x => x.Id == expected.Id);
			Require(Hash(row.Definition) == expected.DefinitionHash && Encoding.UTF8.GetByteCount(row.Definition) == expected.SourceBytes,
				"Dense definition failed an exact UTF-8 roundtrip.");
			var compiled = new CompiledAuthoredCelestial(AuthoredCelestialFormat.Parse(row.Definition), 60, 60, 24);
			Require(compiled.CopyDefinition().Path.Keys.Count == expected.Samples, "Dense samples were lost.");
			foreach (var minute in new[] { -1L, 0L, expected.Samples / 4L, expected.Samples - 1L, expected.Samples * 1000000L + 17 })
			{
				var index = (int)((minute % expected.Samples + expected.Samples) % expected.Samples);
				var expectedElevation = 60 * Math.Sin(index * 2 * Math.PI / expected.Samples) * Math.PI / 180;
				Require(Math.Abs(compiled.Evaluate(minute * 60).Elevation - expectedElevation) < 1e-10, "Dense evaluation changed after reload.");
			}
		}
	}
}
