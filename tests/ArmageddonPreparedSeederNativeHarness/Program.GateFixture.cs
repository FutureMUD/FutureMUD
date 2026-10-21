#nullable enable
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Magic;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private const string GateFixtureKind = "FutureMUD-S4-Nilaz-gate-v1";
	private static string GateFixtureRoot => Path.Combine(CatalogueRepository, ".artifacts", "s4-nilaz-gate-fixtures");
	private sealed record GateFixture(string Kind, string Database, string SqlFile, string SqlSha256,
		string InputFile, string InputSha256, string BaseManifestSha256, string PackageSourceSha256);

	private static void PrepareNilazGateFixture(TestDatabase database)
	{
		Func<FuturemudDatabaseContext> factory = () => NewIndependentContext(database.ConnectionString);
		var result = ArmageddonStockSpellInstaller.Install(factory);
		Require(result.Status == ArmageddonInstallStatus.Completed, "Nilaz stock install failed: " + string.Join("; ", result.Messages));
		using var db = factory();
		var settings = ArmageddonInstallation.Read(db)!;
		var ids = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.LogicalId != null)
			.ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
		var actor = db.Characters.AsNoTracking().Single(x => x.IsAdminAvatar &&
			x.AccountId == settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount]);
		var packageCaps = db.SeederManagedRecords.AsNoTracking().Where(x => x.EntityType == nameof(Db.MagicCapability) &&
			x.Module == ArmageddonProgressionInstaller.Module && !x.Retired).Select(x => x.LogicalId).ToList();
		Require(packageCaps.Count == 10 && packageCaps.All(x => x > 0) &&
			db.MagicCapabilities.AsNoTracking().Where(x => packageCaps.Contains(x.Id)).AsEnumerable().All(x =>
				XElement.Parse(x.Definition).Element("Casting") is { } casting && (bool?)casting.Attribute("enabled") == false &&
				!casting.Elements("Admission").Any()), "Nilaz fixture changed a package capability gate.");
		var routes = new[] { "arm.spell.arcane_mark", ArmageddonGateSpellContent.Key }.Select(key =>
		{
			var spell = db.MagicSpells.AsNoTracking().Single(x => x.Id == ids[key]);
			var school = db.MagicSchools.AsNoTracking().Single(x => x.Id == spell.MagicSchoolId);
			var definition = XElement.Parse(spell.Definition);
			var efficiency = definition.Element("ControlledPower")!.Element("Efficiency")!;
			var cost = new ControlledSpellEfficiency((double)efficiency.Attribute("minimum")!,
				(double)efficiency.Attribute("scale")!).Cost(1, 1);
			var reserve = ids["arm.resource.void"];
			Require(spell.MagicSchoolId == ids["arm.school.void"] && cost is > 0 and <= 100 &&
				(long)definition.Element("Costs")!.Element("Cost")!.Attribute("resource")! ==
				ids[ArmageddonProgressionInstaller.SourceResourceKey], "Nilaz route has an unexpected school or source cost.");
			var route = PreparedStockCampaignRoute(db, settings, key, spell, reserve,
				ids[ArmageddonProgressionInstaller.SourceResourceKey], 1);
			return new { key, spell = spell.Id, capability = route.Capability, merit = route.Merit,
				resource = reserve, cost, invoked = school.SchoolVerb.ToLowerInvariant() };
		}).ToArray();
		var leaf = db.GameItemProtos.AsNoTracking().Single(x => x.Name == "leaf" &&
			x.Keywords == "leaf parchment codex page" && x.ShortDescription == "a parchment codex leaf" && x.RevisionNumber == 0);
		var components = db.GameItemProtosGameItemComponentProtos.AsNoTracking()
			.Where(x => x.GameItemProtoId == leaf.Id && x.GameItemProtoRevision == leaf.RevisionNumber)
			.Select(x => x.GameItemComponent.Type).ToArray();
		Require(db.Materials.AsNoTracking().Any(x => x.Id == leaf.MaterialId && x.Organic) &&
			components.Contains("Destroyable") && components.Contains("Holdable"), "Nilaz mark leaf lacks its required components.");
		var input = Path.Combine(Directory.GetParent(CatalogueRepository)!.FullName,
			"prepared-stock-attunement-input_" + Guid.NewGuid().ToString("N") + ".json");
		File.WriteAllText(input, JsonSerializer.Serialize(new
		{
			character = actor.Id, body = actor.BodyId, package_capabilities = packageCaps,
			nilaz_gate = new { routes, item_prototype = leaf.Id }
		}, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }));
		Environment.SetEnvironmentVariable("FUTUREMUD_S4_ATTUNEMENT_STOCK_INPUT", input);
		Console.WriteLine("ARMS4-Nilaz-focused-input=" + input + " direct-stock-install separate-builder-routes package-gates-disabled");
	}

	private static void SaveNilazGateFixture(TestDatabase database, string baseManifestHash)
	{
		var folder = Path.Combine(GateFixtureRoot, DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture) +
			"_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(folder);
		RequireBasePath(folder, GateFixtureRoot, directory: true);
		var sql = new MySqlDatabaseBackupService().CreateBackup(database.ConnectionString, folder);
		RequireBaseDumpTargets(File.ReadAllText(sql), database.Name);
		var suppliedInput = Environment.GetEnvironmentVariable("FUTUREMUD_S4_ATTUNEMENT_STOCK_INPUT")!;
		var input = Path.Combine(folder, "nilaz-input.json");
		File.Copy(suppliedInput, input);
		var receipt = new GateFixture(GateFixtureKind, database.Name, Path.GetFileName(sql), BaseHash(sql),
			Path.GetFileName(input), BaseHash(input), baseManifestHash, CatalogueContentSourceHash());
		var manifest = Path.Combine(folder, "gate-fixture.json");
		File.WriteAllText(manifest, JsonSerializer.Serialize(receipt, BaseJsonOptions));
		Console.WriteLine("ARMS4-Nilaz-gate-fixture=" + manifest + " sha256:" + BaseHash(manifest));
	}

	private static int RestoreNilazGateFixture()
	{
		var supplied = Environment.GetEnvironmentVariable("FUTUREMUD_S4_GATE_FIXTURE") ??
			throw new InvalidOperationException("Missing explicit Nilaz fixture descriptor.");
		var expected = Environment.GetEnvironmentVariable("FUTUREMUD_S4_GATE_FIXTURE_SHA256") ?? "";
		var manifest = Path.GetFullPath(supplied);
		RequireBasePath(manifest, GateFixtureRoot);
		Require(Path.GetFileName(manifest) == "gate-fixture.json" && expected == BaseHash(manifest),
			"Nilaz fixture manifest differs from its explicit digest.");
		var fixture = JsonSerializer.Deserialize<GateFixture>(File.ReadAllText(manifest)) ??
			throw new InvalidOperationException("Malformed Nilaz fixture descriptor.");
		Require(fixture.Kind == GateFixtureKind && TestDatabase.HasOwnedPrefix(fixture.Database) &&
			fixture.PackageSourceSha256 == CatalogueContentSourceHash() &&
			fixture.BaseManifestSha256 == (Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE_SHA256") ?? ""),
			"Nilaz fixture kind, package source or sealed base pin differs.");
		Require(fixture.SqlFile == Path.GetFileName(fixture.SqlFile) && fixture.InputFile == "nilaz-input.json",
			"Nilaz fixture files must be exact siblings.");
		var folder = Path.GetDirectoryName(manifest)!;
		var sql = Path.Combine(folder, fixture.SqlFile);
		var input = Path.Combine(folder, fixture.InputFile);
		RequireBasePath(sql, GateFixtureRoot);
		RequireBasePath(input, GateFixtureRoot);
		Require(BaseHash(sql) == fixture.SqlSha256 && BaseHash(input) == fixture.InputSha256,
			"Nilaz SQL or input digest differs from its captured fixture.");
		RequireBaseDumpTargets(File.ReadAllText(sql), fixture.Database);
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(!db.Accounts.Any() && !db.SeederChoices.Any(), "Nilaz fixture restore needs a fresh owned target.");
		database.RestoreCatalogueFixture(sql, fixture.Database);
		ConfigureNativeDatabase(database.ConnectionString);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(!db.Database.GetPendingMigrations().Any() && !db.Database.HasPendingModelChanges(),
				"Restored Nilaz fixture schema differs from compiled runtime.");
			Require(db.SeederManagedRecords.Any(x => x.StableKey == ArmageddonGateSpellContent.Key && !x.Retired),
				"Restored Nilaz fixture lacks its installed stock gate spell.");
		}
		Environment.SetEnvironmentVariable("FUTUREMUD_S4_ATTUNEMENT_STOCK_INPUT", input);
		Console.WriteLine("ARMS4-Nilaz-restored=" + manifest + " owned-fresh-database=" + database.Name);
		RunPreparedReplayBoot(database, typeof(ArmageddonSettingSeeder).Assembly, "PreparedCatalogueBootSmoke.py", "prepared-catalogue-boot-native");
		WriteBaseRunReceipt(new { Status = "RESTORED_NILAZ_GATE", Manifest = manifest, ManifestSha256 = BaseHash(manifest),
			fixture.SqlSha256, fixture.InputSha256, Database = database.Name, Ownership = "fresh-owned-marker-resealed",
			PackageRerun = false, BaseManifestSha256 = fixture.BaseManifestSha256 });
		return 0;
	}
}
