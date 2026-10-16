#nullable enable
using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static bool S6Integration => Environment.GetEnvironmentVariable("FUTUREMUD_S6_INTEGRATION_SMOKE_ONLY") == "1";
	private static bool S6Classes => (Environment.GetEnvironmentVariable("FUTUREMUD_BASE_EARLIER_CLASSES") ?? "Off") switch
	{
		"Off" => false, "On" => true, _ => throw new InvalidOperationException("Earlier classes must be Off or On.")
	};

	// A test-only variant of the complete maintained profile. Its exact answers remain sealed
	// independently in the descriptor; production profiles and runner validation are unchanged.
	private static object S6ClassProfile(object profile)
	{
		if (!S6Classes) return profile;
		var steps = ((IEnumerable)profile.GetType().GetProperty("Steps")!.GetValue(profile)!).Cast<object>().ToArray();
		var stepType = steps[0].GetType();
		var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(stepType))!;
		var changed = 0;
		foreach (var step in steps)
		{
			if ((Type)stepType.GetProperty("SeederType")!.GetValue(step)! != typeof(ChargenSeeder)) { list.Add(step); continue; }
			var answers = ((IEnumerable)stepType.GetProperty("Answers")!.GetValue(step)!).Cast<object>().ToArray();
			var answerType = answers[0].GetType();
			var next = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(answerType))!;
			foreach (var answer in answers)
			{
				if ((string)answerType.GetProperty("Id")!.GetValue(answer)! != "class") { next.Add(answer); continue; }
				Require((string)answerType.GetProperty("Answer")!.GetValue(answer)! == "no", "Maintained earlier-class baseline drifted.");
				next.Add(Activator.CreateInstance(answerType, "class", "yes")); changed++;
			}
			list.Add(Activator.CreateInstance(stepType, typeof(ChargenSeeder), next));
		}
		Require(changed == 1, "Classes-on variant must change exactly the earlier Chargen class answer.");
		return Activator.CreateInstance(profile.GetType(), BaseProfileId,
			profile.GetType().GetProperty("Name")!.GetValue(profile),
			profile.GetType().GetProperty("Description")!.GetValue(profile), list)!;
	}

	private static int CaptureS6ReplayBase()
	{
		Require(S6Classes, "S6 replay capture must create the missing authentic classes-on prefix.");
		var profile = BaseProfile(); var steps = BaseSteps(profile); var sources = BaseSources(steps);
		if (Directory.Exists(BaseFixtureRoot))
			foreach (var priorPath in Directory.EnumerateFiles(BaseFixtureRoot, "base-world.json", SearchOption.AllDirectories))
			{
				RequireBasePath(priorPath, BaseFixtureRoot);
				var prior = JsonSerializer.Deserialize<BaseWorldFixture>(File.ReadAllText(priorPath));
				Require(prior is null || !CompatibleBaseSources(prior, sources, out _) || BaseValueHash(prior.BaseSteps) != BaseValueHash(steps),
					"A compatible classes-on base already exists; restore it instead of repeating foundational replay.");
			}
		using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		using (var db = NewPreparedReplayContext(database.ConnectionString)) db.Database.Migrate();
		var assembly = typeof(ArmageddonMagicSeeder).Assembly;
		var seeders = assembly.GetType("DatabaseSeeder.SeederCatalogue", true)!.GetMethod("GetEnabledSeeders", ReplayFlags)!.Invoke(null, null)!;
		var run = assembly.GetType("DatabaseSeeder.SeederReplayRunner", true)!.GetMethod("Run", ReplayFlags)!;
		Func<MudSharp.Database.FuturemudDatabaseContext> factory = () => NewPreparedReplayContext(database.ConnectionString);
		string? manifest = null;
		void Progress(string message)
		{
			Console.WriteLine("ARMS6-replay-progress=" + message);
			if (message != "Running " + new ArmageddonMagicSeeder().Name + "...") return;
			Require(manifest is null, "Armageddon replay boundary executed twice.");
			using var db = factory(); RequireCleanBase(db);
			Require(ArmageddonInstallation.ClassesEnabled(db), "Authentic classes-on prefix did not persist matching RolePicker and answer.");
			var folder = Path.Combine(BaseFixtureRoot, DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ") + "_" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(folder); RequireBasePath(folder, BaseFixtureRoot, directory: true);
			var sql = new MySqlDatabaseBackupService().CreateBackup(database.ConnectionString, folder);
			RequireBaseDumpTargets(File.ReadAllText(sql), database.Name);
			var descriptor = new BaseWorldFixture(BaseFixtureKind, database.Name, Path.GetFileName(sql), BaseHash(sql), BaseProfileId,
				typeof(ArmageddonMagicSeeder).FullName!, steps, db.Database.GetAppliedMigrations().ToArray(), BaseSchemaHash(database),
				BaseValueHash(sources), sources,
				BaseHash(Path.Combine(CatalogueRepository, "DatabaseSeeder", "Assets", "Database", "BlankDatabaseSnapshot.sql")),
				BaseHash(Path.Combine(CatalogueRepository, "DatabaseSeeder", "Assets", "Database", "BlankDatabaseSnapshot.manifest.json")),
				OwnershipTable, BaseContentHash(database), BaseContentAlgorithm);
			manifest = Path.Combine(folder, "base-world.json");
			File.WriteAllText(manifest, JsonSerializer.Serialize(descriptor, BaseJsonOptions));
			File.SetAttributes(sql, File.GetAttributes(sql) | FileAttributes.ReadOnly);
			File.SetAttributes(manifest, File.GetAttributes(manifest) | FileAttributes.ReadOnly);
		}
		object Execute() => run.Invoke(null, [profile, seeders, factory, new Version(1, 0), (Action<string>)Progress])!;
		var result = Execute();
		Require((bool)result.GetType().GetProperty("Success", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(result)!, "S6 complete replay failed: " + result);
		Require(manifest is not null, "Complete replay did not cross the clean Armageddon capture boundary.");
		var before = BaseContentHash(database); var second = Execute();
		Require(!(bool)second.GetType().GetProperty("Success", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(second)! &&
			!((IEnumerable)second.GetType().GetProperty("CompletedSeeders")!.GetValue(second)!).Cast<object>().Any() && before == BaseContentHash(database),
			"Second complete Debug replay must refuse before any step without changing any table.");
		Require(BaseValueHash(sources) == BaseValueHash(BaseSources(steps)), "Base source changed during complete replay.");
		WriteBaseRunReceipt(new { Status = "CAPTURED_WITH_COMPLETE_REPLAY", Manifest = manifest, ManifestSha256 = BaseHash(manifest!),
			EarlierClasses = true, FullReplayExecuted = true, OptionalArmageddonDeclined = true,
			CompletedSeeders = result.GetType().GetProperty("CompletedSeeders")!.GetValue(result), SecondReplayRefused = true,
			RefusedReplayContentUnchanged = true, AllTableContentSha256 = before, DumpRoundTripVerified = false });
		return 0;
	}

	private static void AuditS6Installation(TestDatabase database, string existingPlayers, int existingNpcs)
	{
		Require(existingPlayers == TraditionPlayers(database), "Fresh package installation mutated existing player state.");
		using var db = NewIndependentContext(database.ConnectionString);
		var settings = ArmageddonInstallation.Read(db)!;
		Require(settings.ClassesEnabled == S6Classes && ArmageddonInstallation.ClassesEnabled(db) == S6Classes, "Earlier classes direction changed.");
		Require(db.Npcs.Count() == existingNpcs, "Package installation spawned live NPCs.");
		var stock = new ArmageddonStockProgression(settings.Mode);
		ArmageddonInstallation.Prepare(() => NewIndependentContext(database.ConnectionString), settings.Mode);
		var claims = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && !x.Retired)
			.ToDictionary(x => x.StableKey, StringComparer.Ordinal);
		var blueprints = new ArmageddonStockCatalogue().Blueprints;
		var missing = blueprints.Where(x => !claims.ContainsKey(x.Key)).Select(x => x.Key + " (" + x.Kind + ")").ToArray();
		Require(blueprints.Count == 210 && missing.Length == 0, "Required blueprint count=" + blueprints.Count + "; missing=" + string.Join(", ", missing));
		// Settings preflight validates alias records against their typed native targets.
		// They intentionally carry no LogicalId and are not owned copies of shared rows.
		foreach (var blueprint in blueprints)
		{
			var claim = claims[blueprint.Key];
			if (blueprint.Kind is ArmageddonBlueprintKind.MaterialBinding or ArmageddonBlueprintKind.LiquidBinding or ArmageddonBlueprintKind.Disease)
			{
				Require(claim.EntityType == "ArmageddonCatalogueAlias" && claim.Module == "stock-catalogue-links" && claim.LogicalId is null && claim.RevisionNumber is null,
					"Incompatible native alias blueprint: " + blueprint.Key);
				continue;
			}
			var type = blueprint.Kind switch
			{
				ArmageddonBlueprintKind.Creature => typeof(Db.NpcTemplate), ArmageddonBlueprintKind.AiBundle => typeof(Db.ArtificialIntelligence),
				ArmageddonBlueprintKind.RaceBundle => typeof(Db.Race), ArmageddonBlueprintKind.Shelter => typeof(Db.AutobuilderRoomTemplate),
				ArmageddonBlueprintKind.Culture => typeof(Db.Culture), ArmageddonBlueprintKind.Liquid => typeof(Db.Liquid),
				ArmageddonBlueprintKind.Poison => typeof(Db.Drug), _ => typeof(Db.GameItemProto)
			};
			Require(claim.EntityType == type.Name && claim.LogicalId is > 0 && db.Find(type,
				claim.RevisionNumber is { } revision ? [claim.LogicalId.Value, revision] : [claim.LogicalId.Value]) is not null,
				"Missing or incompatible typed native blueprint row: " + blueprint.Key + " type=" + claim.EntityType + " id=" + claim.LogicalId);
		}
		Require(ArmageddonStockSpellInstaller.CanonicalKeys.Count == 152 && ArmageddonStockSpellInstaller.CanonicalKeys.All(x =>
			claims.TryGetValue(x, out var claim) && claim.EntityType == nameof(Db.MagicSpell) && db.MagicSpells.Any(s => s.Id == claim.LogicalId)), "Missing a canonical stock spell.");
		var roles = claims.Values.Where(x => x.EntityType == nameof(Db.ChargenRole)).ToArray();
		Require(roles.Length == (S6Classes ? 10 : 0), "Package Class role count differs from earlier choice.");
		foreach (var cap in stock.Capabilities)
		{
			var capability = db.MagicCapabilities.Single(x => x.Id == claims[cap.Key].LogicalId);
			var casting = XElement.Parse(capability.Definition).Element("Casting")!;
			var traits = stock.Bindings.Select(x => x.TraitKey).Distinct().ToDictionary(x => x, x => claims[x].LogicalId!.Value);
			var spells = cap.SpellKeys.ToDictionary(x => x, x => claims[x].LogicalId!.Value);
			var expectedCasting = stock.Casting(cap, traits, claims[cap.ResourceKey].LogicalId!.Value, spells, claims[ArmageddonProgressionInstaller.SourceResourceKey].LogicalId!.Value);
			expectedCasting.SetAttributeValue("enabled", false);
			if (ArmageddonStockProgression.IsSorcerer(cap)) expectedCasting.Add(new XElement("SupportGrant",
				new XAttribute("key", ArmageddonStockProgression.Identity(cap.Key, ArmageddonProgressionInstaller.GatheringTraitKey)),
				new XAttribute("trait", claims[ArmageddonProgressionInstaller.GatheringTraitKey].LogicalId!.Value),
				new XAttribute("starting", true), new XAttribute("opening", 10), new XAttribute("rawCap", 90)));
			Require(XNode.DeepEquals(expectedCasting, casting), "Installed inactive admission/support graph differs from the locked complete policy: " + cap.Key);
			var merit = db.Merits.Single(x => x.Id == claims[cap.MeritKey].LogicalId);
			Require((long?)XElement.Parse(merit.Definition).Element("Capabilities")?.Element("Capability") == capability.Id, "Capability merit references the wrong native identity.");
			if (!S6Classes) continue;
			var roleId = claims[cap.RoleKey].LogicalId!.Value;
			var role = db.ChargenRoles.Single(x => x.Id == roleId);
			Require(role.AvailabilityProgId == settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg] &&
				db.ChargenRolesMerits.Where(x => x.ChargenRoleId == roleId).Select(x => x.MeritId).ToArray().SequenceEqual([merit.Id]), "Class availability/merit identity differs.");
			var expected = stock.Bindings.Where(x => x.CapabilityKey == cap.Key && x.Starting)
				.GroupBy(x => claims[x.TraitKey].LogicalId!.Value).ToDictionary(x => x.Key, x => x.Max(y => y.Opening));
			if (ArmageddonStockProgression.IsSorcerer(cap)) expected.Add(claims[ArmageddonProgressionInstaller.GatheringTraitKey].LogicalId!.Value, 10);
			var actual = db.ChargenRolesTraits.Where(x => x.ChargenRoleId == roleId).ToDictionary(x => x.TraitId, x => x.Amount);
			Require(actual.Count == expected.Count && expected.All(x => actual.TryGetValue(x.Key, out var amount) && amount == x.Value), "Class starting skills/supports differ from selected mode.");
		}
		Require(stock.Capabilities.Count == 10 && stock.Bindings.All(x => claims.ContainsKey(x.TraitKey)), "Incomplete capability/support graph.");
		var receipt = new { Status = "PASS", Mode = settings.Mode.ToString(), EarlierClasses = S6Classes, Capabilities = 10,
			Merits = 10, ClassRoles = roles.Length, CanonicalSpells = 152, Blueprints = 210,
			ProgressionBindings = stock.Bindings.Count + 3, SpellAdmissions = stock.Bindings.Count(x => !x.Support), SupportGrants = stock.Bindings.Count(x => x.Support) + 3,
			GatheringPolicyStatus = "CONFIGURED-independent-availability-false", ExistingPlayerStateUnchanged = true, NoLiveNpcsSpawned = true, PackageAvailabilityDisabled = true,
			ClassMeritsAndStartingTraitsExact = true };
		var path = Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_RECEIPT")!;
		File.WriteAllText(Path.Combine(Path.GetDirectoryName(path)!, "s6-installation.json"), JsonSerializer.Serialize(receipt, BaseJsonOptions));
		Console.WriteLine("ARMS6-installation=" + JsonSerializer.Serialize(receipt));
	}
}
