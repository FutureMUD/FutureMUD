#nullable enable
using System.Text.Json;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RestoreFocusDeviceFixture()
	{
		var manifest = Path.GetFullPath(Environment.GetEnvironmentVariable("FUTUREMUD_S4_FOCUS_DEVICE_FIXTURE") ?? throw new InvalidOperationException("Missing focus/device fixture."));
		RequireBasePath(manifest, FocusDeviceFixtureRoot);
		Require(Path.GetFileName(manifest) == "focus-device.json" && BaseHash(manifest) == Environment.GetEnvironmentVariable("FUTUREMUD_S4_FOCUS_DEVICE_FIXTURE_SHA256"), "Focus/device descriptor digest differs.");
		var fixture = JsonSerializer.Deserialize<FocusDeviceFixture>(File.ReadAllText(manifest)) ?? throw new InvalidOperationException("Malformed focus/device fixture.");
		Require(fixture.Kind == "FutureMUD-S4-focus-device-prepared-v1" && TestDatabase.HasOwnedPrefix(fixture.Database) &&
			fixture.PackageSourceSha256 == CatalogueContentSourceHash() && fixture.BaseManifestSha256 == Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE_SHA256") &&
			fixture.SqlFile == Path.GetFileName(fixture.SqlFile) && fixture.InputFile == "focus-device-input.json", "Focus/device source, base or file identity differs.");
		var expectedAssemblies = new[] { "MudSharp.dll", "FutureMUDLibrary.dll", "MudsharpDatabaseLibrary.dll", "ExpressionEngine.dll" };
		Require(fixture.RuntimeAssemblies.Keys.Order().SequenceEqual(expectedAssemblies.Order()) && fixture.RuntimeAssemblies.All(x =>
			BaseHash(Path.Combine(CatalogueRepository, "MudSharpCore", "bin", "Debug", "net10.0", x.Key)) == x.Value), "Prepared fixture runtime assembly identity differs.");
		var folder = Path.GetDirectoryName(manifest)!;
		var sql = Path.Combine(folder, fixture.SqlFile); var savedInput = Path.Combine(folder, fixture.InputFile);
		RequireBasePath(sql, FocusDeviceFixtureRoot); RequireBasePath(savedInput, FocusDeviceFixtureRoot);
		Require(BaseHash(sql) == fixture.SqlSha256 && BaseHash(savedInput) == fixture.InputSha256, "Focus/device dump or input digest differs.");
		RequireBaseDumpTargets(File.ReadAllText(sql), fixture.Database);
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		using (var db = NewIndependentContext(database.ConnectionString)) Require(!db.Accounts.Any() && !db.SeederChoices.Any(), "Focus/device restore needs a fresh owned target.");
		database.RestoreCatalogueFixture(sql, fixture.Database); ConfigureNativeDatabase(database.ConnectionString);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(!db.Database.GetPendingMigrations().Any() && !db.Database.HasPendingModelChanges(), "Restored focus/device schema differs.");
			var keys = ArmageddonStockFocusDeviceInstaller.Keys.ToList();
			Require(db.SeederManagedRecords.Count(x => x.Seeder == ArmageddonMagicInstaller.Package && keys.Contains(x.StableKey) && !x.Retired) == 12, "Restored fixture lacks its twelve owned stock items.");
		}
		var input = Path.Combine(Directory.GetParent(CatalogueRepository)!.FullName, "prepared-stock-attunement-input_" + Guid.NewGuid().ToString("N") + ".json");
		File.Copy(savedInput, input); Environment.SetEnvironmentVariable("FUTUREMUD_S4_ATTUNEMENT_STOCK_INPUT", input);
		if (Environment.GetEnvironmentVariable("FUTUREMUD_S4_FAMILY_SMOKE_ONLY") == "1")
		{
			ReconcileS4VoiceSupport(database, input);
			if (Environment.GetEnvironmentVariable("FUTUREMUD_S4_FAMILY_CASES") == "kindle") ReconcileS4Kindle(database, input);
			PrepareS4FamilyFixture(database, input);
		}
		if (Environment.GetEnvironmentVariable("FUTUREMUD_S5_DOSE_SMOKE_ONLY") == "1") PrepareS5DoseFixture(database, input);
		Console.WriteLine("ARMS4-focus-device-restored=" + manifest + " fresh-owned-database no package replay");
		RunPreparedReplayBoot(database, typeof(ArmageddonSettingSeeder).Assembly, "PreparedCatalogueBootSmoke.py", "prepared-catalogue-boot-native");
		Require(BaseHash(sql) == fixture.SqlSha256 && BaseHash(savedInput) == fixture.InputSha256, "Immutable focus/device fixture changed.");
		WriteBaseRunReceipt(new { Status = "RESTORED_FOCUS_DEVICES", Manifest = manifest, ManifestSha256 = BaseHash(manifest), fixture.SqlSha256,
			fixture.InputSha256, Database = database.Name, Ownership = "fresh-owned-marker-resealed", PackageRerun = false });
		return 0;
	}
}
