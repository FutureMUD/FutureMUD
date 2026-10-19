#nullable enable
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static bool ReleaseStockSmoke => Environment.GetEnvironmentVariable("FUTUREMUD_RELEASE_STOCK_SMOKE") == "1";
	private static bool ReleaseRegionalSmoke => Environment.GetEnvironmentVariable("FUTUREMUD_RELEASE_REGIONAL_SMOKE") == "1";

	// The Debug owner validates/restores the immutable base; only this Release child installs magic.
	private static void RunReleaseStockChild(TestDatabase database)
	{
		Require(Environment.GetEnvironmentVariable("FUTUREMUD_BASE_PACKAGE_MODE") == "Simple" &&
			Environment.GetEnvironmentVariable("FUTUREMUD_BASE_EARLIER_CLASSES") == "On" && !S6Integration,
			"Release stock smoke requires its separate Simple/classes-on lane.");
		var path = Path.Combine(CatalogueRepository, "tests", "ArmageddonPreparedSeederNativeHarness", "bin", "Release", "net10.0", "GatheringNativePersistenceHarness.dll");
		Require(File.Exists(path), "Build the Release harness before the native acceptance lane.");
		var credential = typeof(ArmageddonSettingSeeder).Assembly.GetType("DatabaseSeeder.DebugSeederReplayProfiles", true)!
			.GetField("DebugPassword", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!.GetRawConstantValue() as string;
		Require(!string.IsNullOrWhiteSpace(credential), "Missing disposable fixture credential.");
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, CreateNoWindow = true,
			RedirectStandardOutput = true, RedirectStandardError = true, WorkingDirectory = CatalogueRepository };
		start.ArgumentList.Add(path); start.ArgumentList.Add("--release-stock-install-smoke-run"); start.ArgumentList.Add(database.Name);
		start.Environment["FUTUREMUD_RELEASE_FIXTURE_PASSWORD"] = credential;
		var ready = Path.Combine(Directory.GetParent(CatalogueRepository)!.FullName, "prepared-release-parent-ready_" + Guid.NewGuid().ToString("N") + ".txt");
		var token = Guid.NewGuid().ToString("N");
		start.Environment["FUTUREMUD_RELEASE_PARENT_READY_PATH"] = ready;
		start.Environment["FUTUREMUD_RELEASE_PARENT_READY_TOKEN"] = token;
		using var job = new PreparedBootProcessJob();
		using var child = Process.Start(start) ?? throw new InvalidOperationException("Release installer child did not start.");
		var stdout = child.StandardOutput.ReadToEndAsync(); var stderr = child.StandardError.ReadToEndAsync();
		try
		{
			job.Attach(child);
			File.WriteAllText(ready, token + ":" + child.Id);
			// Installation precedes the independently bounded 20-minute boot scenario.
			Require(child.WaitForExit(1_920_000), "Release stock child exceeded its 32-minute install/boot budget.");
		}
		finally
		{
			if (!job.Attached && !child.HasExited) child.Kill(entireProcessTree: true);
			job.StopAndVerify();
			Require(child.WaitForExit(30_000), "Release installer child did not stop before database disposal.");
			File.Delete(ready);
			Console.Write(stdout.GetAwaiter().GetResult().Replace(database.ConnectionString, "[REDACTED]").Replace(credential!, "[REDACTED]"));
			Console.Error.Write(stderr.GetAwaiter().GetResult().Replace(database.ConnectionString, "[REDACTED]").Replace(credential!, "[REDACTED]"));
		}
		Require(child.ExitCode == 0, "Release stock smoke failed; retain first-failure evidence.");
	}

	private static int ReleaseStockInstallSmoke(string databaseName)
	{
#if DEBUG
		throw new InvalidOperationException("The stock acceptance installer must be compiled Release.");
#else
		Require(ReleaseStockSmoke, "Missing explicit Release stock acceptance lane.");
		var ready = Environment.GetEnvironmentVariable("FUTUREMUD_RELEASE_PARENT_READY_PATH") ?? "";
		var token = Environment.GetEnvironmentVariable("FUTUREMUD_RELEASE_PARENT_READY_TOKEN") ?? "";
		Require(Path.GetDirectoryName(ready) == Directory.GetParent(CatalogueRepository)!.FullName &&
			Path.GetFileName(ready).StartsWith("prepared-release-parent-ready_", StringComparison.Ordinal) && token.Length == 32,
			"Invalid Release parent attachment receipt.");
		var attached = false;
		var deadline = Stopwatch.StartNew();
		while (!attached && deadline.Elapsed < TimeSpan.FromSeconds(30))
		{
			try { attached = File.ReadAllText(ready) == token + ":" + Environment.ProcessId; }
			catch (IOException) { }
			if (!attached) Thread.Sleep(50);
		}
		Require(attached, "Release child refuses SQL/process startup before confirmed parent job attachment.");
		using var database = TestDatabase.OpenExistingOwned(databaseName);
		ConfigureNativeDatabase(database.ConnectionString);
		string players;
		long capacity;
		using (var before = NewIndependentContext(database.ConnectionString))
		{
			RequireCleanBase(before);
			capacity = before.TraitDefinitions.Single(x => x.Name == "Willpower" && x.Type == 1 && x.OwnerScope == 0 && x.DerivedType == 0).Id;
		}
		players = TraditionPlayers(database);
		ExecuteBaseTargetStep(database, new ArmageddonSettingSeeder(), new Dictionary<string, string>
		{
			[ArmageddonSettingSeeder.FullInstallQuestion] = "yes", [ArmageddonSettingSeeder.ModeQuestion] = "Simple",
			[ArmageddonSettingSeeder.InstallQuestion] = "no", [ArmageddonSettingSeeder.CapacityQuestion] = capacity.ToString(),
			[ArmageddonSettingSeeder.NativeBindingsQuestion] = "{}", [ArmageddonSettingSeeder.BindingsQuestion] = "none"
		});
		Require(players == TraditionPlayers(database), "Release installation changed existing player state.");
		using var db = NewIndependentContext(database.ConnectionString);
		var settings = ArmageddonInstallation.Read(db)!;
		Require(settings.StockAvailabilityByDefault && settings.ClassesEnabled, "Fresh Release install did not retain active classes-on defaults.");
		Require(db.MagicCapabilities.Count() == 10 && db.MagicSpells.Count() == 152,
			"Release stock content count drifted.");
		var claims = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package).ToDictionary(x => x.StableKey);
		foreach (var cap in new ArmageddonStockProgression(settings.Mode).Capabilities)
		{
			Require((bool?)XElement.Parse(db.MagicCapabilities.Single(x => x.Id == claims[cap.Key].LogicalId).Definition).Element("Casting")?.Attribute("enabled") == true,
				"An original stock casting policy is disabled.");
			Require(db.ChargenRoles.Single(x => x.Id == claims[cap.RoleKey].LogicalId).AvailabilityProgId == settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg],
				"An original stock Class is unavailable.");
		}
		foreach (var key in new[] { ArmageddonStockSupportInstaller.GatheringAvailabilityKey,
			ArmageddonStockFocusDeviceInstaller.UseGateKey, ArmageddonStockFocusDeviceInstaller.CraftGateKey })
			Require(db.FutureProgs.Single(x => x.Id == claims[key].LogicalId).FunctionText == "return true", "Release stock permission is closed: " + key);
		var actor = db.Characters.Single(x => x.AccountId == settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount] && x.IsAdminAvatar);
		var schema = ((IEnumerable<object>)InstalledSqlSchema(db)).ToList();
		schema.Add(PreparedStockSqlTable<Db.Account>(db, "Accounts", [nameof(Db.Account.Id), nameof(Db.Account.AuthorityGroupId)]));
		schema.Add(PreparedStockSqlTable<Db.AuthorityGroup>(db, "AuthorityGroups", [nameof(Db.AuthorityGroup.Id), nameof(Db.AuthorityGroup.AuthorityLevel)]));
		var path = Path.Combine(Directory.GetParent(CatalogueRepository)!.FullName, "prepared-release-stock-input_" + Guid.NewGuid().ToString("N") + ".json");
		File.WriteAllText(path, JsonSerializer.Serialize(new { character = actor.Id, sql_schema = schema,
			release_originals = new Dictionary<string, object>
			{
				["MagicCapabilities"] = db.MagicCapabilities.AsEnumerable().Select(x => new { id = x.Id, definition = x.Definition }).ToArray(),
				["MagicSpells"] = db.MagicSpells.AsEnumerable().Select(x => new { id = x.Id, definition = x.Definition }).ToArray()
			} }));
		PrepareS5ProgressionFixture(database, path, regional: ReleaseRegionalSmoke);
		Environment.SetEnvironmentVariable("FUTUREMUD_RELEASE_STOCK_INPUT", path);
		RunPreparedReplayBoot(database, typeof(ArmageddonSettingSeeder).Assembly, "PreparedCatalogueBootSmoke.py", "prepared-catalogue-boot-native-release");
		Console.WriteLine(ReleaseRegionalSmoke
			? "ARMV1-regional-land=PASS original-stock Release-install/enrolment/shared-life/local-fallback/cold; Debug-owner-only"
			: "ARMV1-release-stock=PASS original-stock Release-install/enrolment/native-funding/paid-practice/cold; Debug-owner-only");
		return 0;
#endif
	}
}
