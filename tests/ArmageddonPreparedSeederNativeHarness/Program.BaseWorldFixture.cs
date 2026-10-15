#nullable enable
using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MySql.Data.MySqlClient;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private const string BaseFixtureKind = "FutureMUD-pre-Armageddon-base-v1";
	private const string BaseProfileId = "medieval-standard";
	private const string BaseContentAlgorithm = "sha256-table-sorted-row-digests-v1";
	private static string BaseFixtureRoot => Path.Combine(CatalogueRepository, ".artifacts", "pre-armageddon-base-fixtures");
	private sealed record BaseAnswer(string Id, string Answer);
	private sealed record BaseStep(string SeederType, IReadOnlyList<BaseAnswer> Answers);
	private sealed record BaseSource(string Path, string Sha256);
	private sealed record BaseWorldFixture(string Kind, string Database, string SqlFile, string SqlSha256,
		string Profile, string StopBefore, IReadOnlyList<BaseStep> BaseSteps, IReadOnlyList<string> AppliedMigrations,
		string SchemaSha256, string BaseSourceSha256, IReadOnlyList<BaseSource> BaseSources,
		string BlankSqlSha256, string BlankManifestSha256, string OwnershipMarker, string BaseContentSha256, string BaseContentHashAlgorithm);
	private static readonly BindingFlags ReplayFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
	private static readonly JsonSerializerOptions BaseJsonOptions = new() { WriteIndented = true };

	private static string BaseHash(string path)
	{
		using var stream = File.OpenRead(path);
		return Convert.ToHexString(SHA256.HashData(stream));
	}
	private static string BaseValueHash<T>(T value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value))));
	private static object BaseProfile()
	{
		var assembly = typeof(ArmageddonMagicSeeder).Assembly;
		var profiles = (IEnumerable)assembly.GetType("DatabaseSeeder.DebugSeederReplayProfiles", true)!
			.GetProperty("All", ReplayFlags)!.GetValue(null)!;
		var profile = profiles.Cast<object>().Single(x => (string)x.GetType().GetProperty("Id")!.GetValue(x)! == BaseProfileId);
		var seeders = assembly.GetType("DatabaseSeeder.SeederCatalogue", true)!.GetMethod("GetEnabledSeeders", ReplayFlags)!.Invoke(null, null)!;
		var validation = assembly.GetType("DatabaseSeeder.SeederReplayRunner", true)!.GetMethod("Validate", ReplayFlags)!.Invoke(null, [profile, seeders])!;
		Require((bool)validation.GetType().GetProperty("IsValid", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(validation)!,
			"Complete replay inventory has drifted: " + JsonSerializer.Serialize(validation.GetType().GetProperty("Errors")!.GetValue(validation)));
		return profile;
	}
	private static IReadOnlyList<BaseStep> BaseSteps(object profile)
	{
		var steps = ((IEnumerable)profile.GetType().GetProperty("Steps")!.GetValue(profile)!).Cast<object>().ToList();
		var boundary = steps.FindIndex(x => (Type)x.GetType().GetProperty("SeederType")!.GetValue(x)! == typeof(ArmageddonMagicSeeder));
		Require(boundary > 0, "Armageddon must be a declared noninitial replay boundary.");
		return steps.Take(boundary).Select(step => new BaseStep(
			((Type)step.GetType().GetProperty("SeederType")!.GetValue(step)!).FullName!,
			((IEnumerable)step.GetType().GetProperty("Answers")!.GetValue(step)!).Cast<object>()
				.Select(answer => new BaseAnswer((string)answer.GetType().GetProperty("Id")!.GetValue(answer)!,
					(string)answer.GetType().GetProperty("Answer")!.GetValue(answer)!)).ToList())).ToList();
	}
	private static IReadOnlyList<BaseSource> BaseSources(IReadOnlyList<BaseStep> steps)
	{
		var root = CatalogueRepository;
		var directories = steps.Select(x => Path.Combine(root, "DatabaseSeeder", "Seeders", x.SeederType.Split('.').Last()))
			.Where(Directory.Exists)
			.Concat(new[] { "DatabaseSeeder/Seeders/Utilities", "DatabaseSeeder/Seeders/IndustrialisedCatalogue", "DatabaseSeeder/Assets/Database",
				"MudsharpDatabaseLibrary", "FutureMUDLibrary", "ExpressionEngine", "Design Documents/Data", "Design Documents/Seeding/CultureSeederRedesignHandoff" }
				.Select(x => Path.Combine(root, x)));
		var files = directories.SelectMany(x => Directory.EnumerateFiles(x, "*", SearchOption.AllDirectories))
			.Concat(Directory.EnumerateFiles(Path.Combine(root, "DatabaseSeeder", "Seeders"), "*.cs", SearchOption.TopDirectoryOnly))
			.Concat(Directory.EnumerateFiles(Path.Combine(root, "DatabaseSeeder"), "*.cs", SearchOption.TopDirectoryOnly)
				.Where(x => Path.GetFileName(x) != "DebugSeederReplay.cs" && Path.GetFileName(x) != "Program.cs"))
			.Append(Path.Combine(root, "DatabaseSeeder", "DatabaseSeeder.csproj"))
			.Append(Path.Combine(root, "DatabaseSeeder", "Assets", "Manifests", "Monster_AI_Recommendations.json"))
			.Concat(BaseCompilerPins.Select(x => Path.Combine(root, x.Path)))
			.Append(Path.Combine(root, "Design Documents", "Seeding", "Seeded_Item_Manifest.json"))
			.Where(x => !Regex.IsMatch(Path.GetRelativePath(root, x).Replace('\\', '/'), @"(^|/)(bin|obj)/|^FutureMUDLibrary/Magic/Stock/"))
			.Where(x => !IsS4OnlyBaseSource(Path.GetRelativePath(root, x).Replace('\\', '/')))
			.Where(x => new[] { ".cs", ".json", ".tsv", ".sql", ".props", ".targets", ".csproj" }.Contains(Path.GetExtension(x)))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => Path.GetRelativePath(root, x).Replace('\\', '/'), StringComparer.Ordinal);
		var runner = File.ReadAllText(Path.Combine(root, "DatabaseSeeder", "DebugSeederReplay.cs"));
		var profileStart = runner.IndexOf("internal static class DebugSeederReplayProfiles", StringComparison.Ordinal);
		Require(profileStart > 0, "Replay source boundary changed.");
		return files.Select(x => new BaseSource(Path.GetRelativePath(root, x).Replace('\\', '/'), BaseHash(x)))
			.Append(new BaseSource("DatabaseSeeder/DebugSeederReplay.cs#runner", Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(runner[..profileStart])))))
			.OrderBy(x => x.Path, StringComparer.Ordinal).ToList();
	}
	private static bool IsS4OnlyBaseSource(string path) => path.StartsWith("FutureMUDLibrary/Magic/Stock/", StringComparison.Ordinal) ||
		path == "FutureMUDLibrary/Construction/Autobuilder/IAutobuilderRoomPresentation.cs" ||
		path.StartsWith("FutureMUDLibrary/Magic/", StringComparison.Ordinal) &&
			(Path.GetFileName(path).StartsWith("ISpellOwned", StringComparison.Ordinal) ||
			 Path.GetFileName(path).EndsWith("Configuration.cs", StringComparison.Ordinal) ||
			 Path.GetFileName(path).EndsWith("Anchor.cs", StringComparison.Ordinal) ||
			 Path.GetFileName(path) is "ISpellOwnedLifecycle.cs" or "SpellNpcMissionInputs.cs" or "IMagicSpellEffectInvocationInputs.cs");

	private static void RequireCleanBase(FuturemudDatabaseContext db)
	{
		Require(db.Accounts.Any() && db.SeederChoices.Any(), "Fixture has no seeded base world.");
		Require(!db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package || x.StableKey.StartsWith("arm.")) &&
			!db.SeederChoices.Any(x => x.Seeder == new ArmageddonMagicSeeder().Name || x.Seeder == ArmageddonMagicInstaller.Package ||
				x.Choice == ArmageddonInstallation.ModeAnswerKey || x.Choice == ArmageddonInstallation.ClassesAnswerKey || x.Choice == ArmageddonInstallation.BindingsAnswerKey),
			"Base contains Armageddon package claims, settings, or recorded package answers.");
		Require(ArmageddonInstallation.Read(db) is null, "Base contains Armageddon installation settings.");
		Require(!db.Database.GetPendingMigrations().Any() && !db.Database.HasPendingModelChanges(), "Base schema/model is stale.");
		Require(db.Database.GetAppliedMigrations().SequenceEqual(db.Database.GetMigrations()), "Base migration inventory differs from compiled migrations.");
	}
	private static string BaseSchemaHash(TestDatabase database)
	{
		using var connection = database.OpenOwnedConnection();
		var rows = new List<string>();
		foreach (var query in new[]
		{
			"SELECT TABLE_NAME, ENGINE, TABLE_COLLATION FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=@schema ORDER BY TABLE_NAME",
			"SELECT TABLE_NAME, COLUMN_NAME, ORDINAL_POSITION, COLUMN_TYPE, IS_NULLABLE, COLUMN_DEFAULT, EXTRA, CHARACTER_SET_NAME, COLLATION_NAME, GENERATION_EXPRESSION FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=@schema ORDER BY TABLE_NAME, ORDINAL_POSITION",
			"SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE, SEQ_IN_INDEX, COLUMN_NAME, COLLATION, SUB_PART, INDEX_TYPE FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA=@schema ORDER BY TABLE_NAME, INDEX_NAME, SEQ_IN_INDEX",
			"SELECT TABLE_NAME, CONSTRAINT_NAME, COLUMN_NAME, ORDINAL_POSITION, REFERENCED_TABLE_NAME, REFERENCED_COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=@schema ORDER BY TABLE_NAME, CONSTRAINT_NAME, ORDINAL_POSITION",
			"SELECT TABLE_NAME, CONSTRAINT_NAME, REFERENCED_TABLE_NAME, UPDATE_RULE, DELETE_RULE FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=@schema ORDER BY TABLE_NAME, CONSTRAINT_NAME"
		})
		{
			using var command = connection.CreateCommand(); command.CommandText = query; command.Parameters.AddWithValue("@schema", database.Name);
			using var reader = command.ExecuteReader();
			while (reader.Read()) rows.Add(JsonSerializer.Serialize(Enumerable.Range(0, reader.FieldCount)
				.Select(i => reader.IsDBNull(i) ? null : Convert.ToString(reader.GetValue(i), System.Globalization.CultureInfo.InvariantCulture)).ToArray()));
		}
		return BaseValueHash(rows);
	}
	private static void AppendBaseTableDigest(IncrementalHash digest, string table, IEnumerable<string> rowDigests)
	{
		// Each row is hashed independently. Never serialize an entire table as one JSON string.
		var name = Encoding.UTF8.GetBytes(table);
		digest.AppendData(Encoding.UTF8.GetBytes("table:" + name.Length.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":"));
		digest.AppendData(name);
		var count = 0;
		foreach (var row in rowDigests.Order(StringComparer.Ordinal))
		{
			digest.AppendData(Encoding.ASCII.GetBytes("row:" + row)); count++;
		}
		digest.AppendData(Encoding.ASCII.GetBytes("end:" + count.ToString(System.Globalization.CultureInfo.InvariantCulture) + ";"));
	}
	private static string BaseTableDigest(string table, params object[][] rows)
	{
		using var digest = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
		AppendBaseTableDigest(digest, table, rows.Select(BaseValueHash));
		return Convert.ToHexString(digest.GetHashAndReset());
	}
	private static string BaseContentHash(TestDatabase database)
	{
		using var connection = database.OpenOwnedConnection();
		using var command = connection.CreateCommand();
		command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=@schema ORDER BY TABLE_NAME";
		command.Parameters.AddWithValue("@schema", database.Name);
		var tables = new List<string>();
		using (var reader = command.ExecuteReader()) while (reader.Read()) tables.Add(reader.GetString(0));
		using var digest = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
		foreach (var table in tables.Where(x => x != OwnershipTable))
		{
			command.Parameters.Clear(); command.CommandText = "SELECT * FROM `" + table.Replace("`", "``", StringComparison.Ordinal) + "`";
			var rows = new List<string>();
			using (var reader = command.ExecuteReader()) while (reader.Read())
			{
				var values = new object[reader.FieldCount]; reader.GetValues(values); rows.Add(BaseValueHash(values));
			}
			AppendBaseTableDigest(digest, table, rows);
		}
		return Convert.ToHexString(digest.GetHashAndReset());
	}
	private static void RequireBasePath(string path, string root, bool directory = false)
	{
		var full = Path.GetFullPath(path);
		var boundary = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar);
		Require(full.StartsWith(boundary + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase), "Fixture path escapes its owned root.");
		Require(directory ? Directory.Exists(full) : File.Exists(full), "Fixture path does not exist.");
		for (FileSystemInfo? current = directory ? new DirectoryInfo(full) : new FileInfo(full); current is not null; current = current is DirectoryInfo d ? d.Parent : ((FileInfo)current).Directory)
		{
			Require(!current.Attributes.HasFlag(FileAttributes.ReparsePoint), "Fixture path cannot contain reparse points.");
			if (current.FullName.Equals(Path.GetPathRoot(full), StringComparison.OrdinalIgnoreCase)) break;
		}
	}
	private static (BaseWorldFixture Receipt, string Sql, string ManifestHash) ReadBaseFixture(string supplied, string expectedManifestHash,
		IReadOnlyList<BaseStep> steps, IReadOnlyList<BaseSource> sources)
	{
		var manifest = Path.GetFullPath(supplied); RequireBasePath(manifest, BaseFixtureRoot);
		Require(Path.GetFileName(manifest) == "base-world.json" && Regex.IsMatch(expectedManifestHash, "^[A-F0-9]{64}$") &&
			BaseHash(manifest) == expectedManifestHash, "Fixture descriptor digest or filename differs from its explicit pin.");
		var receipt = JsonSerializer.Deserialize<BaseWorldFixture>(File.ReadAllText(manifest)) ?? throw new InvalidOperationException("Malformed base descriptor.");
		Require(receipt.Kind == BaseFixtureKind && receipt.Profile == BaseProfileId && receipt.StopBefore == typeof(ArmageddonMagicSeeder).FullName &&
			receipt.OwnershipMarker == OwnershipTable && receipt.BaseContentHashAlgorithm == BaseContentAlgorithm && Regex.IsMatch(receipt.Database, "^futuremud_land_[a-zA-Z0-9_]+$") && TestDatabase.HasOwnedPrefix(receipt.Database),
			"Wrong fixture kind, replay boundary or database ownership marker.");
		Require(!string.IsNullOrWhiteSpace(receipt.SqlFile) && Path.GetFileName(receipt.SqlFile) == receipt.SqlFile &&
			receipt.SqlFile.EndsWith(".sql", StringComparison.OrdinalIgnoreCase), "Fixture SQL must be a sibling dump.");
		var sql = Path.Combine(Path.GetDirectoryName(manifest)!, receipt.SqlFile); RequireBasePath(sql, BaseFixtureRoot);
		Require(BaseHash(sql) == receipt.SqlSha256 && BaseValueHash(receipt.BaseSteps) == BaseValueHash(steps) &&
			CompatibleBaseSources(receipt, sources, out _),
			"Base dump, exact prefix answers, or base/schema sources have changed; rebuild the base explicitly.");
		Require(receipt.BlankSqlSha256 == BaseHash(Path.Combine(CatalogueRepository, "DatabaseSeeder", "Assets", "Database", "BlankDatabaseSnapshot.sql")) &&
			receipt.BlankManifestSha256 == BaseHash(Path.Combine(CatalogueRepository, "DatabaseSeeder", "Assets", "Database", "BlankDatabaseSnapshot.manifest.json")), "Blank installer snapshot has changed.");
		Require(receipt.AppliedMigrations.Count > 0 && receipt.AppliedMigrations.Distinct(StringComparer.Ordinal).Count() == receipt.AppliedMigrations.Count,
			"Malformed migration inventory.");
		RequireBaseDumpTargets(File.ReadAllText(sql), receipt.Database);
		return (receipt, sql, BaseHash(manifest));
	}
	private static int ValidateBaseWorldFixture(string manifest, string expectedHash)
	{
		var steps = BaseSteps(BaseProfile());
		var fixture = ReadBaseFixture(manifest, expectedHash, steps, BaseSources(steps));
		Console.WriteLine($"ARMBASE-descriptor=PASS sha256:{fixture.ManifestHash} source-and-answer-compatible no-database");
		return 0;
	}
	private static void RequireBaseDumpTargets(string sql, string database)
	{
		var targets = Regex.Matches(sql, @"(?im)^\s*(?:USE\s+|(?:CREATE|DROP)\s+DATABASE\s+(?:IF\s+(?:NOT\s+)?EXISTS\s+)?)`([^`]+)`");
		Require(targets.Count > 0 && targets.All(x => x.Groups[1].Value == database), "Dump contains an unexpected database target.");
		Require(sql.Contains("`" + OwnershipTable + "`", StringComparison.Ordinal), "Dump lacks its captured ownership marker.");
	}
	private static void WriteBaseRunReceipt(object receipt)
	{
		var json = JsonSerializer.Serialize(receipt, BaseJsonOptions);
		var output = Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_RECEIPT");
		if (!string.IsNullOrWhiteSpace(output)) File.WriteAllText(output, json);
		Console.WriteLine("ARMBASE-receipt=" + JsonSerializer.Serialize(receipt));
	}
	private static int CaptureBaseWorldFixture()
	{
		var profile = BaseProfile(); var steps = BaseSteps(profile); var sources = BaseSources(steps);
		if (Directory.Exists(BaseFixtureRoot) && Environment.GetEnvironmentVariable("FUTUREMUD_BASE_FRESH_INSTALL_CHECK") != "1")
		{
			foreach (var existing in Directory.EnumerateFiles(BaseFixtureRoot, "base-world.json", SearchOption.AllDirectories))
			{
				RequireBasePath(existing, BaseFixtureRoot);
				var prior = JsonSerializer.Deserialize<BaseWorldFixture>(File.ReadAllText(existing));
				Require(prior is null || !CompatibleBaseSources(prior, sources, out _) || BaseValueHash(prior.BaseSteps) != BaseValueHash(steps),
					"A compatible base descriptor already exists. Restore its pinned immutable dump; use an explicit fresh-install check only when needed.");
			}
		}
		using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		using (var db = NewPreparedReplayContext(database.ConnectionString)) db.Database.Migrate();
		var assembly = typeof(ArmageddonMagicSeeder).Assembly;
		var seeders = assembly.GetType("DatabaseSeeder.SeederCatalogue", true)!.GetMethod("GetEnabledSeeders", ReplayFlags)!.Invoke(null, null)!;
		var run = assembly.GetType("DatabaseSeeder.SeederReplayRunner", true)!.GetMethod("RunPrefixBefore", ReplayFlags)!;
		Func<FuturemudDatabaseContext> factory = () => NewPreparedReplayContext(database.ConnectionString);
		var result = run.Invoke(null, [profile, seeders, typeof(ArmageddonMagicSeeder), factory, new Version(1, 0), (Action<string>)(x => Console.WriteLine("ARMBASE-progress=" + x))])!;
		Require((bool)result.GetType().GetProperty("Success", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(result)!,
			"Base prefix failed: " + result);
		var seededContent = BaseContentHash(database);
		var repeated = run.Invoke(null, [profile, seeders, typeof(ArmageddonMagicSeeder), factory, new Version(1, 0),
			(Action<string>)(x => Console.WriteLine("ARMBASE-refusal-progress=" + x))])!;
		Require(!(bool)repeated.GetType().GetProperty("Success", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(repeated)! &&
			!((IEnumerable)repeated.GetType().GetProperty("CompletedSeeders")!.GetValue(repeated)!).Cast<object>().Any(),
			"A second prefix replay must refuse the seeded target before executing any step.");
		Require(seededContent == BaseContentHash(database), "Refused second prefix changed the seeded base.");
		Console.WriteLine("ARMBASE-second-prefix-refusal=PASS unchanged-content=" + seededContent);
		IReadOnlyList<string> migrations;
		using (var db = factory()) { RequireCleanBase(db); migrations = db.Database.GetAppliedMigrations().ToList(); }
		var folder = Path.Combine(BaseFixtureRoot, DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ") + "_" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(folder);
		RequireBasePath(folder, BaseFixtureRoot, directory: true);
		var sql = new MySqlDatabaseBackupService().CreateBackup(database.ConnectionString, folder);
		RequireBaseDumpTargets(File.ReadAllText(sql), database.Name);
		Require(BaseValueHash(sources) == BaseValueHash(BaseSources(steps)), "Base sources changed during capture.");
		var receipt = new BaseWorldFixture(BaseFixtureKind, database.Name, Path.GetFileName(sql), BaseHash(sql), BaseProfileId,
			typeof(ArmageddonMagicSeeder).FullName!, steps, migrations, BaseSchemaHash(database), BaseValueHash(sources), sources,
			BaseHash(Path.Combine(CatalogueRepository, "DatabaseSeeder", "Assets", "Database", "BlankDatabaseSnapshot.sql")),
			BaseHash(Path.Combine(CatalogueRepository, "DatabaseSeeder", "Assets", "Database", "BlankDatabaseSnapshot.manifest.json")), OwnershipTable,
			BaseContentHash(database), BaseContentAlgorithm);
		var manifest = Path.Combine(folder, "base-world.json"); File.WriteAllText(manifest, JsonSerializer.Serialize(receipt, BaseJsonOptions));
		File.SetAttributes(sql, File.GetAttributes(sql) | FileAttributes.ReadOnly); File.SetAttributes(manifest, File.GetAttributes(manifest) | FileAttributes.ReadOnly);
		WriteBaseRunReceipt(new { Status = "CAPTURED", Manifest = manifest, ManifestSha256 = BaseHash(manifest), receipt.SqlSha256, receipt.BaseSourceSha256, BaseStepCount = steps.Count,
			DumpRoundTripVerified = false, SecondPrefixRefused = true, RefusedReplayContentUnchanged = true, LastBaseSeeder = steps.Last().SeederType, StopBefore = receipt.StopBefore, LatestMigration = migrations.Last(), CleanArmageddonPackage = true });
		return 0;
	}
	private static int RestoreBaseWorldFixture(bool installPackage)
	{
		var profile = BaseProfile(); var steps = BaseSteps(profile); var sources = BaseSources(steps);
		var fixture = ReadBaseFixture(Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE") ?? throw new InvalidOperationException("Missing explicit base fixture."),
			Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE_SHA256") ?? throw new InvalidOperationException("Missing pinned base descriptor digest."), steps, sources);
		var mode = Environment.GetEnvironmentVariable("FUTUREMUD_BASE_PACKAGE_MODE") ?? "Classic";
		var optional = Environment.GetEnvironmentVariable("FUTUREMUD_BASE_LATER_STEPS") ?? "";
		var requested = optional.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		Require(mode is "Classic" or "Simple", "Choose Classic or Simple for the targeted package.");
		Require(requested.Distinct(StringComparer.Ordinal).Count() == requested.Length && requested.All(x => x is "PsionicsSeeder" or "EnvironmentalExposureSeeder"), "Only explicit later optional steps are supported.");
		Require(installPackage || requested.Length == 0, "Clean restore cannot execute optional later steps.");
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		using (var db = NewIndependentContext(database.ConnectionString)) Require(!db.Accounts.Any() && !db.SeederChoices.Any() && !db.SeederManagedRecords.Any(), "Restore requires a fresh empty owned target.");
		database.RestoreCatalogueFixture(fixture.Sql, fixture.Receipt.Database); ConfigureNativeDatabase(database.ConnectionString);
		using (var db = NewPreparedReplayContext(database.ConnectionString))
		{
			RequireCleanBase(db);
			Require(db.Database.GetAppliedMigrations().SequenceEqual(fixture.Receipt.AppliedMigrations), "Restored migration history differs from descriptor.");
		}
		Require(BaseSchemaHash(database) == fixture.Receipt.SchemaSha256 && BaseContentHash(database) == fixture.Receipt.BaseContentSha256,
			"Restored actual schema or base content differs from captured fixture.");
		var executed = new List<string>();
		if (installPackage)
		{
			long capacity;
			using (var db = NewPreparedReplayContext(database.ConnectionString)) capacity = db.TraitDefinitions.Single(x => x.Name == "Willpower" && x.Type == 1 && x.OwnerScope == 0 && x.DerivedType == 0).Id;
			ExecuteBaseTargetStep(database, new ArmageddonMagicSeeder(), new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				[ArmageddonMagicSeeder.InstallQuestion] = "no", [ArmageddonMagicSeeder.FullInstallQuestion] = "yes", [ArmageddonMagicSeeder.ModeQuestion] = mode,
				[ArmageddonMagicSeeder.CapacityQuestion] = capacity.ToString(System.Globalization.CultureInfo.InvariantCulture), [ArmageddonMagicSeeder.NativeBindingsQuestion] = "{}", [ArmageddonMagicSeeder.BindingsQuestion] = "none"
			}); executed.Add(typeof(ArmageddonMagicSeeder).FullName!);
			foreach (var type in new[] { typeof(PsionicsSeeder), typeof(EnvironmentalExposureSeeder) }.Where(x => requested.Contains(x.Name)))
			{
				var step = ((IEnumerable)profile.GetType().GetProperty("Steps")!.GetValue(profile)!).Cast<object>().Single(x => (Type)x.GetType().GetProperty("SeederType")!.GetValue(x)! == type);
				var answers = ((IEnumerable)step.GetType().GetProperty("Answers")!.GetValue(step)!).Cast<object>().ToDictionary(x => (string)x.GetType().GetProperty("Id")!.GetValue(x)!, x => (string)x.GetType().GetProperty("Answer")!.GetValue(x)!);
				ExecuteBaseTargetStep(database, (IDatabaseSeeder)Activator.CreateInstance(type)!, answers); executed.Add(type.FullName!);
			}
			if (Environment.GetEnvironmentVariable("FUTUREMUD_S4_EXCHANGE_SMOKE_ONLY") == "1") PrepareExchangeFixture(database);
			else if (Environment.GetEnvironmentVariable("FUTUREMUD_S4_GATE_SMOKE_ONLY") == "1")
			{
				PrepareNilazGateFixture(database);
				SaveNilazGateFixture(database, fixture.ManifestHash);
			}
			else if (Environment.GetEnvironmentVariable("FUTUREMUD_S4_ATTUNEMENT_STOCK_SMOKE") == "1")
			{
				PreparedStockAttunementAcceptance(database);
				if (Environment.GetEnvironmentVariable("FUTUREMUD_S4_REMAINING_SMOKE_ONLY") == "1") PrepareRemainingStockFixture(database);
			}
			RunPreparedReplayBoot(database, typeof(ArmageddonMagicSeeder).Assembly, "PreparedCatalogueBootSmoke.py", "prepared-catalogue-boot-native");
		}
		Require(BaseHash(fixture.Sql) == fixture.Receipt.SqlSha256 &&
			BaseHash(Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE")!) == fixture.ManifestHash &&
			BaseValueHash(sources) == BaseValueHash(BaseSources(steps)), "Canonical descriptor/dump or base source changed during restore/targeted run.");
		WriteBaseRunReceipt(new { Status = installPackage ? "RESTORED_AND_TARGETED_PACKAGE" : "RESTORED_CLEAN", fixture.ManifestHash, fixture.Receipt.SqlSha256,
			fixture.Receipt.BaseSourceSha256, CurrentBaseSourceSha256 = BaseValueHash(sources),
			BaseSourceCompatibilityRule = CompatibleBaseSources(fixture.Receipt, sources, out var rule) ? rule : "INVALID", Database = database.Name, Ownership = "fresh-owned-marker-resealed", ExecutedSeeders = executed, FullReplayExecuted = false, CanonicalSqlUnchanged = true, DumpRoundTripVerified = true,
			PackageMode = installPackage ? mode : null, LaterSteps = requested, PackageContentSourceSha256 = installPackage ? CatalogueContentSourceHash() : null });
		return 0;
	}
	// Pure descriptor safety checks: no server, database, or native process is opened.
	private static int BaseFixtureSafetyChecks()
	{
		var folder = Path.Combine(BaseFixtureRoot, "safety_" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(folder);
		var sql = Path.Combine(folder, "fixture.sql"); var manifest = Path.Combine(folder, "base-world.json");
		var database = "futuremud_land_safety";
		File.WriteAllText(sql, $"USE `{database}`;\nCREATE TABLE `{OwnershipTable}` (token varchar(64));");
		IReadOnlyList<BaseStep> steps = [new("DatabaseSeeder.Seeders.CoreDataSeeder", [new("answer", "exact")])];
		IReadOnlyList<BaseSource> sources = [new("DatabaseSeeder/base.cs", new string('A', 64))];
		var original = new BaseWorldFixture(BaseFixtureKind, database, "fixture.sql", BaseHash(sql), BaseProfileId,
			typeof(ArmageddonMagicSeeder).FullName!, steps, ["migration"], new string('B', 64), BaseValueHash(sources), sources,
			BaseHash(Path.Combine(CatalogueRepository, "DatabaseSeeder", "Assets", "Database", "BlankDatabaseSnapshot.sql")),
			BaseHash(Path.Combine(CatalogueRepository, "DatabaseSeeder", "Assets", "Database", "BlankDatabaseSnapshot.manifest.json")), OwnershipTable, new string('C', 64), BaseContentAlgorithm);
		var checks = 0;
		void Write(BaseWorldFixture value) => File.WriteAllText(manifest, JsonSerializer.Serialize(value, BaseJsonOptions));
		void Refuses(Action action)
		{
			try { action(); } catch (InvalidOperationException) { checks++; return; }
			throw new InvalidOperationException("Descriptor safety check unexpectedly accepted unsafe input.");
		}
		try
		{
			Write(original); ReadBaseFixture(manifest, BaseHash(manifest), steps, sources); checks++;
			var rows = new object[][] { [1, "same"], [2, "other"] };
			var content = BaseTableDigest("native", rows);
			Require(content == BaseTableDigest("native", rows.Reverse().ToArray()), "Native row ordering must not alter the content digest."); checks++;
			Require(content != BaseTableDigest("native", [1, "changed"], [2, "other"]), "Changed native data must alter the digest."); checks++;
			Require(content != BaseTableDigest("native", rows.Concat(rows.Take(1)).ToArray()), "Duplicated native rows must alter the digest."); checks++;
			Require(content != BaseTableDigest("different", rows), "Table identity must be part of the digest."); checks++;
			foreach (var path in new[] { "FutureMUDLibrary/Magic/Stock/ArmageddonShelterSpellContent.cs",
				"FutureMUDLibrary/Magic/ISpellOwnedShelterService.cs", "FutureMUDLibrary/Construction/Autobuilder/IAutobuilderRoomPresentation.cs" })
			{ Require(IsS4OnlyBaseSource(path), "S4-only contracts must not invalidate the unrelated seeded base."); checks++; }
			Require(!IsS4OnlyBaseSource("FutureMUDLibrary/Magic/MagicSchool.cs"), "General shared inputs remain fingerprinted."); checks++;
			IReadOnlyList<BaseSource> compatibilitySources = [new(DamageInterfacePath, BaseDamageInterfaceHash),
				new(RoomReferenceTypesPath, BaseRoomReferenceTypesHash), new("base.cs", new string('D', 64))];
			var compatibilityReceipt = original with { BaseSources = compatibilitySources, BaseSourceSha256 = BaseValueHash(compatibilitySources) };
			var live = compatibilitySources.Select(x => x.Path == DamageInterfacePath ? x with { Sha256 = ReviewedDamageInterfaceHash } :
				x.Path == RoomReferenceTypesPath ? x with { Sha256 = ReviewedRoomReferenceTypesHash } : x)
				.Concat(BaseCompilerPins).Concat(ReviewedS4SourceAdditions).OrderBy(x => x.Path, StringComparer.Ordinal).ToArray();
			Require(CompatibleBaseSources(compatibilityReceipt, live, out var applied) && applied == S4BaseCompatibility, "Exact reviewed inert receipt compatibility must retain the sealed base."); checks++;
			foreach (var drift in new[]
			{
				live.Select(x => x.Path == "base.cs" ? x with { Sha256 = new string('E', 64) } : x).ToArray(),
				live.Select(x => x.Path == DamageInterfacePath ? x with { Sha256 = new string('F', 64) } : x).ToArray(),
				live.Select(x => x.Path == RoomReferenceTypesPath ? x with { Sha256 = new string('F', 64) } : x).ToArray(),
				live.Concat([new BaseSource("FutureMUDLibrary/Health/unreviewed.cs", new string('A', 64))]).ToArray(),
				live.Select(x => x.Path == BaseCompilerPins[0].Path ? x with { Sha256 = new string('F', 64) } : x).ToArray(),
				live.Where(x => x.Path != ReviewedS4SourceAdditions[0].Path).ToArray()
			}) { Require(!CompatibleBaseSources(compatibilityReceipt, drift, out _), "Unreviewed source drift must invalidate the base."); checks++; }
			Refuses(() => ReadBaseFixture(manifest, new string('0', 64), steps, sources));
			foreach (var changed in new[] { original with { Kind = "post-S3" }, original with { SqlFile = "../fixture.sql" },
				original with { Database = "user_database" }, original with { OwnershipMarker = "other" },
				original with { BaseContentHashAlgorithm = "unrecognised" },
				original with { StopBefore = typeof(PsionicsSeeder).FullName! }, original with { SqlSha256 = new string('0', 64) },
				original with { BaseSourceSha256 = new string('0', 64) }, original with { AppliedMigrations = ["duplicate", "duplicate"] },
				original with { BaseSteps = [new("DatabaseSeeder.Seeders.CoreDataSeeder", [new("answer", "changed")])] } })
			{
				Write(changed); Refuses(() => ReadBaseFixture(manifest, BaseHash(manifest), steps, sources));
			}
			Write(original); File.AppendAllText(sql, "\n-- changed"); Refuses(() => ReadBaseFixture(manifest, BaseHash(manifest), steps, sources));
			Refuses(() => RequireBasePath(manifest, Path.Combine(folder, "nested")));
			Refuses(() => RequireBaseDumpTargets("USE `user_database`;", database));
			Refuses(() => RequireBaseDumpTargets($"USE `{database}`;", database));
			Console.WriteLine($"ARMBASE-safety=PASS checks:{checks} descriptor-kind-boundary-digest-sibling-prefix-marker-source-answer-migration-dump-target-refusals no-database");
			return 0;
		}
		finally
		{
			// Delete only the exact newly generated safety directory after checking the owned boundary.
			RequireBasePath(folder, BaseFixtureRoot, directory: true); Directory.Delete(folder, recursive: true);
		}
	}
	private static void ExecuteBaseTargetStep(TestDatabase database, IDatabaseSeeder seeder, IReadOnlyDictionary<string, string> supplied)
	{
		using var db = NewPreparedReplayContext(database.ConnectionString);
		Require(seeder.Enabled && seeder.AssessSeedData(db).Status != SeederAssessmentStatus.Blocked, "Targeted step is blocked: " + seeder.Name);
		var questions = seeder.Questions.ToList(); var answers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		Require(questions.Select(x => x.Id).ToHashSet(StringComparer.OrdinalIgnoreCase).SetEquals(supplied.Keys), "Targeted question inventory differs from supplied answers.");
		foreach (var question in questions)
		{
			if (!question.Filter(db, answers)) continue;
			Require(supplied.TryGetValue(question.Id, out var answer), "Missing active targeted answer: " + question.Id);
			var valid = question.Validator(answer!, db); Require(valid.Success, "Invalid targeted answer: " + question.Id + ": " + valid.error); answers[question.Id] = answer!;
		}
		var execution = typeof(ArmageddonMagicSeeder).Assembly.GetType("DatabaseSeeder.SeederExecutionService", true)!
			.GetMethod("Execute", ReplayFlags)!.Invoke(null, [db, seeder, questions, answers, new Version(1, 0)])!;
		Require((bool)execution.GetType().GetProperty("Success")!.GetValue(execution)!, "Targeted step failed: " + execution);
		Console.WriteLine("ARMBASE-targeted-step=" + seeder.GetType().FullName);
	}
}
