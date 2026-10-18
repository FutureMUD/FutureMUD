#nullable enable

using System.Collections;
using System.Reflection;
using System.Text.Json;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record CoreFoundationFixture(string Kind, string Database, string SqlFile, string SqlSha256,
		string SchemaSha256, string ContentSha256, string SourceSha256, IReadOnlyList<BaseStep> Steps);

	// A separate reduced scenario. The production Debug replay remains exhaustive and untruncated.
	private static int CoreFoundationQualification()
	{
        var skillPackageType = typeof(CoreDataSeeder).Assembly.GetType("DatabaseSeeder.Seeders.SkillPackageSeeder", true)!;
		var types = new[] { typeof(CoreDataSeeder), typeof(TimeSeeder), typeof(AttributeSeeder), skillPackageType,
			typeof(HumanSeeder), typeof(CombatSeeder), typeof(ChargenSeeder) };
		var profile = BaseProfile();
		var broad = ((IEnumerable)profile.GetType().GetProperty("Steps")!.GetValue(profile)!).Cast<object>();
		var steps = types.Select(type =>
		{
			var step = broad.Single(x => (Type)x.GetType().GetProperty("SeederType")!.GetValue(x)! == type);
			var answers = ((IEnumerable)step.GetType().GetProperty("Answers")!.GetValue(step)!).Cast<object>()
				.ToDictionary(x => (string)x.GetType().GetProperty("Id")!.GetValue(x)!, x => (string)x.GetType().GetProperty("Answer")!.GetValue(x)!);
			if (type == skillPackageType) { answers["skillgainmodel"] = "armageddon"; answers["modern"] = "no"; }
			if (type == typeof(CombatSeeder)) { answers["installmuskets"] = "no"; answers["installguns"] = "no"; }
			return new BaseStep(type.FullName!, answers.Select(x => new BaseAnswer(x.Key, x.Value)).ToArray());
		}).ToArray();
		var sources = BaseSources(steps);
		var sourceHash = BaseValueHash(sources);
		var folder = Path.Combine(CatalogueRepository, ".artifacts", "core-foundations-fixtures", DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ") + "_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(folder);
		string descriptor;
		string descriptorHash;
		CoreFoundationFixture receipt;
        var supplied = Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE");
        var suppliedHash = Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE_SHA256");
        if (string.IsNullOrWhiteSpace(supplied))
        {
            Require(string.IsNullOrWhiteSpace(suppliedHash), "A foundation fixture digest requires its explicit descriptor path.");
		using (var fresh = TestDatabase.CreateFresh("futuremud_land_"))
		{
			ConfigureNativeDatabase(fresh.ConnectionString);
			using (var db = NewPreparedReplayContext(fresh.ConnectionString)) db.Database.Migrate();
			foreach (var step in steps)
			{
				var type = types.Single(x => x.FullName == step.SeederType);
				ExecuteBaseTargetStep(fresh, (IDatabaseSeeder)Activator.CreateInstance(type)!, step.Answers.ToDictionary(x => x.Id, x => x.Answer));
				using var db = NewPreparedReplayContext(fresh.ConnectionString);
				RequireNoUseful(db);
			}
			using (var db = NewPreparedReplayContext(fresh.ConnectionString))
			{
				RequireCleanBase(db);
				AuditCoreFoundations(db);
			}
			var sql = new MySqlDatabaseBackupService().CreateBackup(fresh.ConnectionString, folder);
			RequireBaseDumpTargets(File.ReadAllText(sql), fresh.Name);
			receipt = new("FutureMUD-Core-foundations-v1", fresh.Name, Path.GetFileName(sql), BaseHash(sql), BaseSchemaHash(fresh), BaseContentHash(fresh), sourceHash, steps);
			descriptor = Path.Combine(folder, "core-foundations.json");
			File.WriteAllText(descriptor, JsonSerializer.Serialize(receipt, BaseJsonOptions));
			File.SetAttributes(sql, File.GetAttributes(sql) | FileAttributes.ReadOnly);
			File.SetAttributes(descriptor, File.GetAttributes(descriptor) | FileAttributes.ReadOnly);
			descriptorHash = BaseHash(descriptor);
			Console.WriteLine("CORE-foundation-fresh=PASS seven-seeders no-Useful sealed-fixture=" + descriptor);
		}
        }
        else
        {
            descriptor = Path.GetFullPath(supplied);
            RequireBasePath(descriptor, Path.Combine(CatalogueRepository, ".artifacts", "core-foundations-fixtures"));
            Require(Path.GetFileName(descriptor) == "core-foundations.json" && suppliedHash is not null &&
                System.Text.RegularExpressions.Regex.IsMatch(suppliedHash, "^[A-F0-9]{64}$") && BaseHash(descriptor) == suppliedHash,
                "Foundation descriptor differs from its explicit digest or filename.");
            descriptorHash = suppliedHash;
            receipt = JsonSerializer.Deserialize<CoreFoundationFixture>(File.ReadAllText(descriptor)) ?? throw new InvalidOperationException("Malformed foundation fixture.");
            Require(receipt.Kind == "FutureMUD-Core-foundations-v1" && receipt.SourceSha256 == sourceHash && BaseValueHash(receipt.Steps) == BaseValueHash(steps) &&
                TestDatabase.HasOwnedPrefix(receipt.Database) && Path.GetFileName(receipt.SqlFile) == receipt.SqlFile && receipt.SqlFile.EndsWith(".sql", StringComparison.OrdinalIgnoreCase),
                "Foundation fixture kind, source, answer inventory or sibling SQL identity differs; explicit new capture required.");
            folder = Path.GetDirectoryName(descriptor)!;
            Console.WriteLine("CORE-foundation-reuse=PASS explicitly-pinned sealed-fixture=" + descriptor);
        }


		var sqlPath = Path.Combine(folder, receipt.SqlFile);
		RequireBasePath(descriptor, Path.Combine(CatalogueRepository, ".artifacts", "core-foundations-fixtures"));
		RequireBasePath(sqlPath, Path.Combine(CatalogueRepository, ".artifacts", "core-foundations-fixtures"));
		Require(BaseHash(descriptor) == descriptorHash && BaseHash(sqlPath) == receipt.SqlSha256 && sourceHash == BaseValueHash(BaseSources(steps)), "Sealed foundation fixture or sources changed.");
        Require(File.GetAttributes(descriptor).HasFlag(FileAttributes.ReadOnly) && File.GetAttributes(sqlPath).HasFlag(FileAttributes.ReadOnly), "Foundation descriptor/SQL are not sealed read-only.");
        RequireBaseDumpTargets(File.ReadAllText(sqlPath), receipt.Database);
		using (var restored = TestDatabase.CreateFresh("futuremud_land_"))
		{
			restored.RestoreCatalogueFixture(sqlPath, receipt.Database);
			ConfigureNativeDatabase(restored.ConnectionString);
			Require(BaseSchemaHash(restored) == receipt.SchemaSha256 && BaseContentHash(restored) == receipt.ContentSha256, "Clean restore differs from sealed foundation schema/content.");
			Dictionary<string, long?> ids;
			long custom;
			using (var db = NewPreparedReplayContext(restored.ConnectionString))
			{
				AuditCoreFoundations(db);
				RequireNoUseful(db);
				ids = db.SeederManagedRecords.Where(x => x.Seeder == nameof(CoreDataSeeder) && x.Module == "TagCatalogue").ToDictionary(x => x.StableKey, x => x.LogicalId);
				var tag = new MudSharp.Models.Tag { Name = "Foundation qualification custom ramrod" };
				db.Tags.Add(tag); db.SaveChanges(); custom = tag.Id;
				db.StaticConfigurations.Find("MusketRamrodTag")!.Definition = custom.ToString();
				db.SaveChanges();
			}
			var core = steps[0];
			ExecuteBaseTargetStep(restored, new CoreDataSeeder(), core.Answers.ToDictionary(x => x.Id, x => x.Answer));
			using (var db = NewPreparedReplayContext(restored.ConnectionString))
			{
				AuditCoreFoundations(db);
				Require(db.Accounts.Count() == 1 && db.StaticConfigurations.Find("MusketRamrodTag")!.Definition == custom.ToString(), "Core rerun changed bootstrap or custom setting.");
				var afterIds = db.SeederManagedRecords.Where(x => x.Seeder == nameof(CoreDataSeeder) && x.Module == "TagCatalogue").ToDictionary(x => x.StableKey, x => x.LogicalId);
                Require(ids.Count == afterIds.Count && ids.All(x => afterIds.GetValueOrDefault(x.Key) == x.Value), "Core rerun changed tag IDs.");
				RequireNoUseful(db);
				var assessment = new ArmageddonMagicSeeder().AssessSeedData(db);
				Require(assessment.Status != SeederAssessmentStatus.Blocked, "Reduced Armageddon preflight blocked: " + string.Join("; ", assessment.MissingPrerequisites));
			}
			long capacity;
			using (var db = NewPreparedReplayContext(restored.ConnectionString)) capacity = db.TraitDefinitions.Single(x => x.Name == "Willpower" && x.Type == 1 && x.OwnerScope == 0 && x.DerivedType == 0).Id;
			ExecuteBaseTargetStep(restored, new ArmageddonMagicSeeder(), new Dictionary<string, string>
			{
				[ArmageddonMagicSeeder.InstallQuestion] = "no", [ArmageddonMagicSeeder.FullInstallQuestion] = "yes",
				[ArmageddonMagicSeeder.ModeQuestion] = "Simple", [ArmageddonMagicSeeder.CapacityQuestion] = capacity.ToString(),
				[ArmageddonMagicSeeder.NativeBindingsQuestion] = "{}", [ArmageddonMagicSeeder.BindingsQuestion] = "none"
			});
			using (var db = NewPreparedReplayContext(restored.ConnectionString))
			{
				RequireNoUseful(db);
				var ownedSpells = db.SeederManagedRecords.Where(x => x.EntityType == "MagicSpell" && x.Seeder == ArmageddonMagicInstaller.Package && !x.Retired).Select(x => x.LogicalId).ToArray();
                var spellIds = db.MagicSpells.Select(x => x.Id).ToHashSet();
                Require(ownedSpells.Length == 152 && ownedSpells.Distinct().Count() == 152 && ownedSpells.All(x => x is { } id && spellIds.Contains(id)), "Exact 152 canonical owned spells were not installed.");
				var sewer = db.Terrains.Single(x => x.Id == db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonRegionalLandInstaller.TerrainKey("sewer")).LogicalId);
				Require(sewer.StaminaCost == 40 && sewer.TerrainBehaviourMode == "cave", "Regional Sewer did not consume Core Sewer physical preset.");
			}
			RunCoreFoundationBoot(restored);
			Console.WriteLine("CORE-foundation-restored=PASS exact-roundtrip rerun-identities custom-binding full-Armageddon no-Useful");
		}
		Require(BaseHash(descriptor) == descriptorHash && BaseHash(sqlPath) == receipt.SqlSha256 && sourceHash == BaseValueHash(BaseSources(steps)), "Foundation fixtures/sources changed during qualification.");
		WriteBaseRunReceipt(new { Status = "PASS", Scope = "Core tags, Sewer/Ruins, reduced Combat/Armageddon baseline", Descriptor = descriptor,
			DescriptorSha256 = descriptorHash, receipt.SqlSha256, SourceSha256 = sourceHash, FreshSeeders = types.Select(x => x.Name), FullReplayExecuted = false,
			DumpRoundTripVerified = true, TagIdentitiesPreserved = true, CustomBindingPreserved = true, ZeroUsefulComponents = true,
			OriginalSealedFilesUnchanged = true, OwnedDatabasesDisposed = true });
		return 0;
	}

	private static void RequireNoUseful(FuturemudDatabaseContext db)
	{
		var names = new[] { "StockItemMarkersForTesting", "StockModernItemMarkersForTesting" }
            .SelectMany(property => (IReadOnlyCollection<string>)typeof(UsefulSeeder).GetProperty(property, ReplayFlags)!.GetValue(null)!).Distinct().ToArray();
        Require(!db.SeederChoices.Any(x => x.Seeder == new UsefulSeeder().Name || x.Seeder == nameof(UsefulSeeder)) &&
            !db.SeederManagedRecords.Any(x => x.Seeder == nameof(UsefulSeeder)) &&
            !db.GameItemComponentProtos.Any(x => names.Contains(x.Name) || x.Type == "Sheath"),
            "Reduced scenario contains Useful execution, claims or optional component content.");
	}

	private static void AuditCoreFoundations(FuturemudDatabaseContext db)
	{
		var claims = db.SeederManagedRecords.Where(x => x.Seeder == nameof(CoreDataSeeder) && x.Module == "TagCatalogue").ToArray();
		Require(claims.Length == 2335 && claims.Select(x => x.LogicalId).Distinct().Count() == 2335, "Core canonical tag identity count differs.");
		var tagIds = db.Tags.Select(x => x.Id).ToHashSet();
		Require(claims.All(x => x.LogicalId is { } id && tagIds.Contains(id)), "Core has unresolved tag claims.");
		Require(db.Terrains.Single(x => x.Name == "Sewer").StaminaCost == 40 && db.Terrains.Single(x => x.Name == "Ruins").StaminaCost == 20, "Core terrain costs differ.");
		foreach (var name in new[] { "MusketRamrodTag", "MusketCleaningRamrodTag", "MusketUnjammingToolTag", "MusketMatchCordTag", "MusketIgnitionSourceTag", "WadItemTagId" })
			Require(long.TryParse(db.StaticConfigurations.Find(name)?.Definition, out var id) && db.Tags.Any(x => x.Id == id), "Unresolved firearm tag setting " + name);
		Require(long.TryParse(db.StaticConfigurations.Find("GunpowderMaterialId")?.Definition, out var materialId) && db.Materials.Any(x => x.Id == materialId && x.Name == "gunpowder"), "Unresolved gunpowder binding.");
	}

	private static void RunCoreFoundationBoot(TestDatabase database)
    {
        // Use the normal first/cold login/LOOK/save/graceful-shutdown lane, including process jobs and attachment handshake.
        var laneRoot = Directory.GetParent(CatalogueRepository)!.FullName;
        var source = Path.Combine(CatalogueRepository, "tests", "ArmageddonPreparedSeederNativeHarness", "PreparedReplayBootSmoke.py");
        var copy = Path.Combine(laneRoot, "PreparedReplayBootSmoke.py");
        if (!File.Exists(copy)) File.Copy(source, copy);
        Require(File.ReadAllBytes(source).SequenceEqual(File.ReadAllBytes(copy)), "Foundation boot script handoff differs from the checked-in source.");
        var output = Path.Combine(laneRoot, "prepared-replay-boot-native_foundations_" + Guid.NewGuid().ToString("N"));
        var priorScript = Environment.GetEnvironmentVariable("FUTUREMUD_PREPARED_REPLAY_BOOT_SCRIPT");
        var priorOutput = Environment.GetEnvironmentVariable("FUTUREMUD_PREPARED_REPLAY_BOOT_OUTPUT");
        try
        {
            Environment.SetEnvironmentVariable("FUTUREMUD_PREPARED_REPLAY_BOOT_SCRIPT", copy);
            Environment.SetEnvironmentVariable("FUTUREMUD_PREPARED_REPLAY_BOOT_OUTPUT", output);
            RunPreparedReplayBoot(database, typeof(ArmageddonMagicSeeder).Assembly);
            Console.WriteLine("CORE-foundation-boot-receipt=" + Path.Combine(output, "boot-latest.json"));
        }
        finally
        {
            Environment.SetEnvironmentVariable("FUTUREMUD_PREPARED_REPLAY_BOOT_SCRIPT", priorScript);
            Environment.SetEnvironmentVariable("FUTUREMUD_PREPARED_REPLAY_BOOT_OUTPUT", priorOutput);
        }
    }
}
