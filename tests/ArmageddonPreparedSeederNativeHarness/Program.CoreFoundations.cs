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
	private static int CoreFoundationQualification(bool setting = false, bool sourcePinOnly = false, bool followupsOnly = false)
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
			if (type == skillPackageType) { answers["skillgainmodel"] = "armageddon"; answers["modern"] = "no";
                answers["skillpreset"] = "Complex-NonGerund";
                if (setting) { answers["skillpreset"] = "Armageddon"; answers["complexity"] = "armageddon"; answers["skillcapmodel"] = "class"; answers["branching"] = "no"; answers["gerund"] = "no"; } }
			if (type == typeof(CombatSeeder)) { answers["installmuskets"] = "no"; answers["installguns"] = "no";
                if (setting) { answers["skilloption"] = "armageddon"; answers["parryoption"] = "yes"; } }
            if (setting && type == typeof(ChargenSeeder)) { answers["class"] = "yes"; answers["subclass"] = "yes"; }
			return new BaseStep(type.FullName!, answers.Select(x => new BaseAnswer(x.Key, x.Value)).ToArray());
		}).ToArray();
		var capturedSteps = setting ? steps.Take(3).ToArray() : steps;
        var sources = BaseSources(steps);
		var sourceHash = BaseValueHash(sources);
        if (sourcePinOnly) { Console.WriteLine("SETTING-source-pin=" + sourceHash); return 0; }
		var folder = Path.Combine(CatalogueRepository, ".artifacts", setting ? "setting-foundations-fixtures" : "core-foundations-fixtures", DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ") + "_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(folder);
		string descriptor;
		string descriptorHash;
		CoreFoundationFixture receipt;
        var supplied = Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE");
        var suppliedHash = Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE_SHA256");
		Require(!followupsOnly || setting && !string.IsNullOrWhiteSpace(supplied), "Follow-up qualification requires the explicit sealed three-step baseline.");
        if (string.IsNullOrWhiteSpace(supplied))
        {
            Require(string.IsNullOrWhiteSpace(suppliedHash), "A foundation fixture digest requires its explicit descriptor path.");
		using (var fresh = TestDatabase.CreateFresh("futuremud_land_"))
		{
			ConfigureNativeDatabase(fresh.ConnectionString);
			using (var db = NewPreparedReplayContext(fresh.ConnectionString)) db.Database.Migrate();
			foreach (var step in capturedSteps)
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
			receipt = new((setting ? "FutureMUD-Setting-foundations-v1" : "FutureMUD-Core-foundations-v1"), fresh.Name, Path.GetFileName(sql), BaseHash(sql), BaseSchemaHash(fresh), BaseContentHash(fresh), sourceHash, capturedSteps);
			descriptor = Path.Combine(folder, "core-foundations.json");
			File.WriteAllText(descriptor, JsonSerializer.Serialize(receipt, BaseJsonOptions));
			File.SetAttributes(sql, File.GetAttributes(sql) | FileAttributes.ReadOnly);
			File.SetAttributes(descriptor, File.GetAttributes(descriptor) | FileAttributes.ReadOnly);
			descriptorHash = BaseHash(descriptor);
			Console.WriteLine((setting ? "SETTING-pre-skill-base=PASS three-seeders no-Useful sealed-fixture=" : "CORE-foundation-fresh=PASS seven-seeders no-Useful sealed-fixture=") + descriptor);
		}
        }
        else
        {
            descriptor = Path.GetFullPath(supplied);
            RequireBasePath(descriptor, Path.Combine(CatalogueRepository, ".artifacts", setting ? "setting-foundations-fixtures" : "core-foundations-fixtures"));
            Require(Path.GetFileName(descriptor) == "core-foundations.json" && suppliedHash is not null &&
                System.Text.RegularExpressions.Regex.IsMatch(suppliedHash, "^[A-F0-9]{64}$") && BaseHash(descriptor) == suppliedHash,
                "Foundation descriptor differs from its explicit digest or filename.");
            descriptorHash = suppliedHash;
            receipt = JsonSerializer.Deserialize<CoreFoundationFixture>(File.ReadAllText(descriptor)) ?? throw new InvalidOperationException("Malformed foundation fixture.");
            Require(receipt.Kind == (setting ? "FutureMUD-Setting-foundations-v1" : "FutureMUD-Core-foundations-v1") &&
                (receipt.SourceSha256 == sourceHash || setting && CompatibleSettingFoundation(receipt, descriptorHash, sourceHash)) && BaseValueHash(receipt.Steps) == BaseValueHash(capturedSteps) &&
                TestDatabase.HasOwnedPrefix(receipt.Database) && Path.GetFileName(receipt.SqlFile) == receipt.SqlFile && receipt.SqlFile.EndsWith(".sql", StringComparison.OrdinalIgnoreCase),
                "Foundation fixture kind, source, answer inventory or sibling SQL identity differs; explicit new capture required.");
            folder = Path.GetDirectoryName(descriptor)!;
            Console.WriteLine("CORE-foundation-reuse=PASS explicitly-pinned sealed-fixture=" + descriptor);
        }


		var sqlPath = Path.Combine(folder, receipt.SqlFile);
		RequireBasePath(descriptor, Path.Combine(CatalogueRepository, ".artifacts", setting ? "setting-foundations-fixtures" : "core-foundations-fixtures"));
		RequireBasePath(sqlPath, Path.Combine(CatalogueRepository, ".artifacts", setting ? "setting-foundations-fixtures" : "core-foundations-fixtures"));
		Require(BaseHash(descriptor) == descriptorHash && BaseHash(sqlPath) == receipt.SqlSha256 && sourceHash == BaseValueHash(BaseSources(steps)), "Sealed foundation fixture or sources changed.");
        Require(File.GetAttributes(descriptor).HasFlag(FileAttributes.ReadOnly) && File.GetAttributes(sqlPath).HasFlag(FileAttributes.ReadOnly), "Foundation descriptor/SQL are not sealed read-only.");
        RequireBaseDumpTargets(File.ReadAllText(sqlPath), receipt.Database);
		using (var restored = TestDatabase.CreateFresh("futuremud_land_"))
		{
			restored.RestoreCatalogueFixture(sqlPath, receipt.Database);
			ConfigureNativeDatabase(restored.ConnectionString);
			Require(BaseSchemaHash(restored) == receipt.SchemaSha256 && BaseContentHash(restored) == receipt.ContentSha256, "Clean restore differs from sealed foundation schema/content.");
			// Verify the sealed historical schema/content FIRST. Only this newly owned restored
			// copy may receive the reviewed intrinsic-merit and nullable age-description extensions.
			if (setting)
			{
				using var upgrade = NewPreparedReplayContext(restored.ConnectionString);
				var pending = upgrade.Database.GetPendingMigrations().ToArray();
				Require(pending.Length == 0 || pending.SequenceEqual(new[] { "20261021005427_IntrinsicRacialMerits", "20261021023928_ConfigurableRaceAgeDescriptions" }),
					"Restored foundation has an unreviewed pending migration inventory.");
				if (pending.Length > 0) upgrade.Database.Migrate();
				Require(!upgrade.Database.GetPendingMigrations().Any() && !upgrade.Database.HasPendingModelChanges(),
					"Owned restored foundation upgrade left schema/model drift.");
			}
			if (followupsOnly)
			{
				QualifySettingFollowups(restored, steps);
				Require(BaseHash(descriptor) == descriptorHash && BaseHash(sqlPath) == receipt.SqlSha256 && sourceHash == BaseValueHash(BaseSources(steps)), "Follow-up qualification changed sealed baseline or source closure.");
				WriteBaseRunReceipt(new { Status = "PASS", Scope = "Focused native installer follow-ups only; no server/gameplay certificate", Descriptor = descriptor,
					DescriptorSha256 = descriptorHash, receipt.SqlSha256, SourceSha256 = sourceHash, BaselineCapturedThisRun = false,
					OrdinaryProfilePrefixAndSecondAttemptRefusal = true, ArmPresetActiveQuestions = 1, NormalizedSavedAnswers = 7,
					NamingCultures = 15, NativeGenderProfiles = 75, DistinctPersonalForms = 321, ElementDescriptions = 78, ExecutableElements = 78, NativeNamingChargenSaveResume = true,
					ColdProcessReaders = 2, BuilderPlayerAgeAndSweatOverridesPreserved = true, IncompleteProfilesRemainUnavailable = true,
					OriginalSealedFilesUnchanged = true });
				Console.WriteLine("SETTING-followups=PASS installer-only native-MySQL cold-process-reruns no-server-or-gameplay-certificate");
				return 0;
			}
			Dictionary<string, long?> ids;
			long custom;
            if (setting)
            {
                foreach (var step in steps.Skip(3))
                {
                    var type = types.Single(x => x.FullName == step.SeederType);
                    ExecuteBaseTargetStep(restored, (IDatabaseSeeder)Activator.CreateInstance(type)!, step.Answers.ToDictionary(x => x.Id, x => x.Answer));
                    if (type == skillPackageType)
                    {
                        using var customSkills = NewPreparedReplayContext(restored.ConnectionString);
                        var ownedSkills = ArmageddonSkillPackage.Resolve(customSkills);
                        ownedSkills["arm.skill.shield_use"].Name = "Builder Shield Defence";
                        ownedSkills["arm.skill.blind_fighting"].Name = "Builder Unseen Fighting";
                        ownedSkills["arm.skill.peek"].Expression.Expression = "87";
                        customSkills.SaveChanges();
                    }
                }
            }
            if (setting)
            {
                Dictionary<string, long> skillIds;
                using (var beforeRerun = NewPreparedReplayContext(restored.ConnectionString))
                {
                    skillIds = ArmageddonSkillPackage.Resolve(beforeRerun).ToDictionary(x => x.Key, x => x.Value.Id);
                    beforeRerun.CharacterTraits.Add(new MudSharp.Models.CharacterTrait
                    {
                        CharacterId = beforeRerun.Characters.Single().Id, TraitDefinitionId = skillIds["arm.skill.peek"], Value = 57, AdditionalValue = 3
                    });
                    beforeRerun.SaveChanges();
                }
                foreach (var type in new[] { skillPackageType, typeof(CombatSeeder) })
                {
                    var step = steps.Single(x => x.SeederType == type.FullName);
                    ExecuteBaseTargetStep(restored, (IDatabaseSeeder)Activator.CreateInstance(type)!, step.Answers.ToDictionary(x => x.Id, x => x.Answer));
                }
                using var afterRerun = NewPreparedReplayContext(restored.ConnectionString);
                var afterSkills = ArmageddonSkillPackage.Resolve(afterRerun);
                Require(afterRerun.CharacterTraits.Any(x => x.TraitDefinitionId == afterSkills["arm.skill.peek"].Id && x.Value == 57 && x.AdditionalValue == 3), "Early rerun changed saved player values.");
                Require(skillIds.All(x => afterSkills[x.Key].Id == x.Value) && afterSkills["arm.skill.shield_use"].Name == "Builder Shield Defence" &&
                    afterSkills["arm.skill.blind_fighting"].Name == "Builder Unseen Fighting" && afterSkills["arm.skill.peek"].Expression.Expression == "87",
                    "Early package rerun changed IDs or builder overrides.");
                Require(!afterRerun.TraitDefinitions.Any(x => x.Name == "Blind Fighting"), "Combat invented an unowned replacement after a builder rename.");
                foreach (var binding in ArmageddonSkillPackage.StaticTraitBindings)
                    Require(afterRerun.StaticConfigurations.Find(binding.Key)?.Definition == afterSkills[binding.Value].Id.ToString(), "Unresolved static trait binding: " + binding.Key);
                var never = afterRerun.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id;
                Require(afterRerun.ManualCombatCommands.Where(x => x.PrimaryVerb == "bash" || x.PrimaryVerb == "kick")
                    .All(x => x.PlayerUsable && x.NpcUsable && x.CombatActionId != null && x.UsabilityProgId != never), "Source impact commands were not enabled.");
                Require(afterRerun.CombatActions.Single(x => x.Name == "Bash").UsabilityProgId == never, "Generic Bash must remain gated.");
                foreach (var name in new[] { "Two Handed Sword", "Training Two Handed Sword", "Two Handed Axe", "Training Two Handed Axe" })
                {
                    var weapon = afterRerun.WeaponTypes.Single(x => x.Name == name);
                    Require(weapon.AttackTraitId == afterSkills["arm.skill.two_handed"].Id && weapon.ParryTraitId == afterSkills["arm.skill.parry"].Id,
                        "Approved Two Handed/Parry assignment differs: " + name);
                }
            }
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
				var assessment = new ArmageddonSettingSeeder().AssessSeedData(db);
				Require(assessment.Status != SeederAssessmentStatus.Blocked, "Reduced Armageddon preflight blocked: " + string.Join("; ", assessment.MissingPrerequisites));
			}
			long capacity;
			using (var db = NewPreparedReplayContext(restored.ConnectionString)) capacity = db.TraitDefinitions.Single(x => x.Name == "Willpower" && x.Type == 1 && x.OwnerScope == 0 && x.DerivedType == 0).Id;
			var settingAnswers = new Dictionary<string, string>
			{
				[ArmageddonSettingSeeder.InstallQuestion] = "no", [ArmageddonSettingSeeder.FullInstallQuestion] = "yes",
				[ArmageddonSettingSeeder.ModeQuestion] = "Simple", [ArmageddonSettingSeeder.CapacityQuestion] = capacity.ToString(),
				[ArmageddonSettingSeeder.NativeBindingsQuestion] = "{}", [ArmageddonSettingSeeder.BindingsQuestion] = "none"
			};
            Dictionary<string, long?> committed = new();
            if (setting)
            {
                Func<FuturemudDatabaseContext> factory = () => NewIndependentContext(restored.ConnectionString);
                ArmageddonInstallation.Prepare(factory, ArmageddonProgressionMode.Simple, capacity, enableStockForNewInstall: true);
                using (var db = factory()) ArmageddonMundaneMerits.Install(db, new List<string>());
                var partial = ArmageddonProgressionInstaller.Install(factory);
                Require(partial.Status == ArmageddonInstallStatus.Completed, "Could not prepare the committed phase-resume checkpoint.");
                using var snapshot = factory();
                committed = snapshot.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package).ToDictionary(x => x.StableKey, x => x.LogicalId);
                Console.WriteLine("SETTING-resume-checkpoint=committed settings merits progression; catalogue-unstarted");
            }
			ExecuteBaseTargetStep(restored, new ArmageddonSettingSeeder(), settingAnswers);
            if (setting) { CaptureSettingDebugCheckpoint(restored); RunSettingBindingAudit(restored,"installed"); QualifySettingNativeRolesAndHeritage(restored); }
			using (var db = NewPreparedReplayContext(restored.ConnectionString))
			{
				RequireNoUseful(db);
				var ownedSpells = db.SeederManagedRecords.Where(x => x.EntityType == "MagicSpell" && x.Seeder == ArmageddonMagicInstaller.Package && x.Module != ArmageddonTemplarInstaller.Module && !x.Retired).Select(x => x.LogicalId).ToArray();
                var spellIds = db.MagicSpells.Select(x => x.Id).ToHashSet();
                Require(ownedSpells.Length == 152 && ownedSpells.Distinct().Count() == 152 && ownedSpells.All(x => x is { } id && spellIds.Contains(id)), "Exact 152 canonical owned spells were not installed.");
				var sewer = db.Terrains.Single(x => x.Id == db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonRegionalLandInstaller.TerrainKey("sewer")).LogicalId);
				Require(sewer.StaminaCost == 40 && sewer.TerrainBehaviourMode == "cave", "Regional Sewer did not consume Core Sewer physical preset.");
                if (setting)
                {
                    var after = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package).ToDictionary(x => x.StableKey, x => x.LogicalId);
                    Require(committed.All(x => after.TryGetValue(x.Key, out var id) && id == x.Value), "Phase resume changed committed identities.");
                    committed = after;
                    var meritId = after[ArmageddonMundaneMerits.MercenaryKey];
                    var merit = db.Merits.Single(x => x.Id == meritId);
                    var definition = System.Xml.Linq.XElement.Parse(merit.Definition);
                    definition.SetAttributeValue("multiplier", "1.5");
                    merit.Definition = definition.ToString(); db.SaveChanges();
                }
			}
            if (setting)
            {
                ExecuteBaseTargetStep(restored, new ArmageddonSettingSeeder(), settingAnswers);
                using var db = NewPreparedReplayContext(restored.ConnectionString);
                var after = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package).ToDictionary(x => x.StableKey, x => x.LogicalId);
                Require(after.Count == committed.Count && committed.All(x => after.TryGetValue(x.Key, out var id) && id == x.Value), "Magic-phase rerun changed owned identities.");
                var merit = db.Merits.Single(x => x.Id == after[ArmageddonMundaneMerits.MercenaryKey]);
                Require((string?)System.Xml.Linq.XElement.Parse(merit.Definition).Attribute("multiplier") == "1.5", "Rerun overwrote builder merit customisation.");
                var peekId = after["arm.skill.peek"];
                Require(db.CharacterTraits.Any(x => x.TraitDefinitionId == peekId && x.Value == 57 && x.AdditionalValue == 3), "Magic-phase resume/rerun changed saved player values.");
                Console.WriteLine("SETTING-resume-rerun=PASS committed-identities builder-merit saved-player-values");
            }
			if (setting) RunSettingBindingAudit(restored,"rerun");
			RunCoreFoundationBoot(restored);
			Console.WriteLine("CORE-foundation-restored=PASS exact-roundtrip rerun-identities custom-binding full-Armageddon no-Useful");
		}
		Require(BaseHash(descriptor) == descriptorHash && BaseHash(sqlPath) == receipt.SqlSha256 && sourceHash == BaseValueHash(BaseSources(steps)), "Foundation fixtures/sources changed during qualification.");
		WriteBaseRunReceipt(new { Status = "PASS", Scope = setting ? "Combined classes-on Setting; actual native progression/heritage/impacts/openings/Pilot and seven physical profile bodies; race admission held" : "Core tags, Sewer/Ruins, reduced Combat/Armageddon baseline", Descriptor = descriptor,
			DescriptorSha256 = descriptorHash, receipt.SqlSha256, SourceSha256 = sourceHash, BaselineCapturedThisRun = string.IsNullOrWhiteSpace(supplied), CapturedBaselineSeeders = capturedSteps.Select(x=>x.SeederType), ReviewedThreeStepSourceCompatibility = receipt.SourceSha256 != sourceHash, RestoredSeeders = (setting ? types.Skip(3) : Array.Empty<Type>()).Select(x => x.Name), FullReplayExecuted = false,
			DumpRoundTripVerified = true, TagIdentitiesPreserved = true, CustomBindingPreserved = true, ZeroUsefulComponents = true,
			MagicPhaseResumeVerified = setting, MagicPhaseRerunVerified = setting, PlayerValuePreserved = setting,
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
            RunPreparedReplayBoot(database, typeof(ArmageddonSettingSeeder).Assembly);
            Console.WriteLine("CORE-foundation-boot-receipt=" + Path.Combine(output, "boot-latest.json"));
        }
        finally
        {
            Environment.SetEnvironmentVariable("FUTUREMUD_PREPARED_REPLAY_BOOT_SCRIPT", priorScript);
            Environment.SetEnvironmentVariable("FUTUREMUD_PREPARED_REPLAY_BOOT_OUTPUT", priorOutput);
        }
    }

	private static bool CompatibleSettingFoundation(CoreFoundationFixture receipt, string descriptorHash, string currentSourceHash)
	{
		// This receipt captured only Core/Time/Attributes. Its aggregate pin also included the four
		// not-yet-executed seeders and all Library interfaces. Reviewed local slices changed those
		// later packages/interfaces, not the captured three seeders or ExpressionEngine. The newer
		// physiology pin additionally admits the reviewed additive race-merit model/migration and
		// the room-free RoleSkillBonusClaims serializer inventory entry (absent from this baseline);
		// restoration checks the original schema/content before upgrading the owned copy above.
		// Exact old/new and sealed descriptor/SQL/schema/content pins allow this one restore only.
		return descriptorHash == "E26C6B5992C664F52BFA38488CB2F19143B8B80A59C7A3CB949D7B7F014C61A8" &&
			receipt.Kind == "FutureMUD-Setting-foundations-v1" && receipt.Steps.Count == 3 &&
			receipt.SourceSha256 == "C62FBE7A23C71B967C05BCE8EFBCB9E890E6A28F415FB7D52B3C77F75DED4C1B" &&
			(currentSourceHash == "9B7A1BD56AC27596120AF87246A370207B4C68303379974BC753DF56E96E7BA6" ||
			 currentSourceHash == ReviewedPhysiologyFoundationSourceHash || currentSourceHash == ReviewedPhysicalFoundationSourceHash) &&
			receipt.SqlSha256 == "64113F06E363B582ECE510A3C112984F51D28977DB7250A53D28A6DA790F7206" &&
			receipt.SchemaSha256 == "36CF37216ABAC521E308DC44FA83DDDA246414F171A7D1B27A7C634BE93A17B3" &&
			receipt.ContentSha256 == "8D8553E9D1353969FCFA7EE082F789FB98B9D777771905AAAB416DC8ACFF8C45";
	}
	// Exact current file inventory measured by the read-only --setting-source-pin lane.
	private const string ReviewedPhysiologyFoundationSourceHash = "7AECCB81F5E804E4C196E6C47DD12B44A9BEC502FDD9AB17BD298715DC21A2D2";
	// Physical slice: nullable lifecycle description mapping/migration, refreshed snapshot and
	// opt-in shared age/style interfaces. The historical seed/schema/content pins remain exact.
	private const string ReviewedPhysicalFoundationSourceHash = "2F859BB74250BDD29C11FE236C9E49E0E7FA86BFA302000C328E43E74F0B247E";
}
