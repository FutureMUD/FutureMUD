#nullable enable
using System.Collections;
using System.Reflection;
using System.Text.Json;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
#if DEBUG
	private static int FullSettingReplayNative()
	{
		var assembly = typeof(ArmageddonSettingSeeder).Assembly;
		const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		var profiles = (IEnumerable)assembly.GetType("DatabaseSeeder.DebugSeederReplayProfiles", true)!
			.GetProperty("All", flags)!.GetValue(null)!;
		var stock = profiles.Cast<object>().Single(x => (string)x.GetType().GetProperty("Id")!.GetValue(x)! == "medieval-standard");
		var catalogue = ((IEnumerable<IDatabaseSeeder>)assembly.GetType("DatabaseSeeder.SeederCatalogue", true)!
			.GetMethod("GetEnabledSeeders", flags)!.Invoke(null, null)!).ToArray();
		var runner = assembly.GetType("DatabaseSeeder.SeederReplayRunner", true)!;
		var validation = runner.GetMethod("Validate", flags)!.Invoke(null, [stock, catalogue])!;
		Require((bool)validation.GetType().GetProperty("IsValid", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(validation)!,
			"Maintained full Medieval profile drifted; do not project an incomplete inventory.");
		Require(catalogue.Count(x => x.GetType() == typeof(ItemSeeder)) == 1, "Expected exactly one authorized Item omission.");
		var selected = catalogue.Where(x => x.GetType() != typeof(ItemSeeder)).ToArray();
		var profile = FullSettingReplayProfile(stock, selected);
		var supplied = Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE");
		if (!string.IsNullOrWhiteSpace(supplied)) return RestoreFullSettingReplay(supplied, profile);
		var sources = FullReplayProductionHash();
		if (Directory.Exists(FullReplayFixtureRoot) && Environment.GetEnvironmentVariable("FUTUREMUD_BASE_FRESH_INSTALL_CHECK") != "1")
			foreach (var descriptor in Directory.EnumerateFiles(FullReplayFixtureRoot, "full-replay.json", SearchOption.AllDirectories))
			{
				var prior = JsonSerializer.Deserialize<FullReplayFixture>(File.ReadAllText(descriptor))!;
				Require(prior.ProductionSha256 != sources || prior.AnswersSha256 != FullReplayAnswersHash(profile),
					"A compatible sealed full install exists. Restore its explicit descriptor/digest instead of reseeding, or request an explicit fresh-install check.");
			}
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var run = runner.GetMethod("Run", flags)!;
		Func<FuturemudDatabaseContext> contextFactory = () => NewPreparedReplayContext(database.ConnectionString);
		void Progress(string message)
		{
			Console.WriteLine("SETTING-full-progress=" + message);
			if (message != $"Running {selected.Single(x => x is ArmageddonSettingSeeder).Name}...") return;
			using var db = contextFactory();
			// Explicit smoke policy for the pinned LabMUD attribute selection, matching the
			// combined lane. Resolve its real generated ID only after Attribute has committed.
			var capacity = db.TraitDefinitions.Single(x => x.Name == "Willpower" && x.Type == 1 &&
				x.DerivedType == 0 && x.OwnerScope == 0 && (x.TraitGroup == "Mental" || x.TraitGroup == "Spiritual"));
			var steps = (IEnumerable)profile.GetType().GetProperty("Steps")!.GetValue(profile)!;
			var step = steps.Cast<object>().Single(x => (Type)x.GetType().GetProperty("SeederType")!.GetValue(x)! == typeof(ArmageddonSettingSeeder));
			var answers = (IList)step.GetType().GetProperty("Answers")!.GetValue(step)!;
			var index = Enumerable.Range(0, answers.Count).Single(i => (string)answers[i]!.GetType().GetProperty("Id")!.GetValue(answers[i])! == ArmageddonSettingSeeder.CapacityQuestion);
			answers[index] = Activator.CreateInstance(answers[index]!.GetType(), ArmageddonSettingSeeder.CapacityQuestion, capacity.Id.ToString());
			Console.WriteLine("SETTING-full-capacity=explicit LabMUD Willpower id:" + capacity.Id);
		}
		object Execute() => run.Invoke(null, [profile, selected, contextFactory, new Version(1, 0),
			(Action<string>)Progress])!;
		var first = Execute();
		var type = first.GetType();
		var success = (bool)type.GetProperty("Success", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(first)!;
		Console.WriteLine("SETTING-full-replay=" + JsonSerializer.Serialize(new
		{
			Profile = "armageddon-all-except-item", Success = success,
			Completed = type.GetProperty("CompletedSeeders")!.GetValue(first),
			Failed = type.GetProperty("FailedSeeder")!.GetValue(first),
			Unstarted = type.GetProperty("UnstartedSeeders")!.GetValue(first),
			Failure = type.GetProperty("Failure")!.GetValue(first),
			Exception = (type.GetProperty("Exception")!.GetValue(first) as Exception)?.ToString()
		}));
		var before = PreparedDatabaseChecksum(database);
		var second = Execute();
		Require(((string?)second.GetType().GetProperty("Failure")!.GetValue(second))?
			.Contains("freshly migrated, unseeded", StringComparison.OrdinalIgnoreCase) == true && before == PreparedDatabaseChecksum(database),
			"Full replay retry must refuse without mutating any table.");
		Console.WriteLine("SETTING-full-refusal=PASS all-table-sha256:" + before);
		if (!success) return 2;
		var checkpoint = SealFullSettingReplay(database, profile, sources);
		// Qualify an exact restored copy, retaining the clean post-install baseline for
		// bounded runtime retries and proving that the new backup actually round-trips.
		return RestoreFullSettingReplay(checkpoint.Descriptor, profile, checkpoint.Sha256);
	}

	private static int QualifyFullSettingReplay(TestDatabase database)
	{
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var settings = ArmageddonInstallation.Read(db);
			var willpower = db.TraitDefinitions.Single(x => x.Name == "Willpower" && x.Type == 1 && x.DerivedType == 0 &&
				x.OwnerScope == 0 && (x.TraitGroup == "Mental" || x.TraitGroup == "Spiritual"));
			Require(settings is { Mode: ArmageddonProgressionMode.Simple, ClassesEnabled: true } && settings.CapacityAttribute == willpower.Id,
				"Full replay checkpoint must retain Simple/classes-on and the explicit ordinary LabMUD Willpower capacity policy.");
		}
		RequirePhysicalAdmissionDefaults(database);
		using (var world = LoadProductionRestartWorld(new HarnessClock()))
		using (new FMDB())
		{
			((IFuturemudLoader)world).LoadSurgery();
			using var db = NewIndependentContext(database.ConnectionString);
			var skills = ArmageddonSkillPackage.Resolve(db);
			var horse = world.Races.Single(x => x.Name == "Horse");
			Require(horse.BaseBody is not null && horse.DefaultHealthStrategy is not null &&
				horse.NaturalWeaponAttacks.Any() && horse.ButcheryProfile is not null,
				"Production Horse must load its body, health, natural attacks and butchery profile.");
			Require(world.WeaponTypes.Any(x => x.Name == "Longsword") && world.WeaponTypes.Any(x => x.Name == "Razor Weapon") &&
				world.SurgicalProcedures.Any() && world.HealthStrategies.Any() && world.RaceButcheryProfiles.Any(),
				"Production optional health/combat/butchery registries did not load representative content.");
			var longsword = world.WeaponTypes.Single(x => x.Name == "Longsword");
			var razor = world.WeaponTypes.Single(x => x.Name == "Razor Weapon");
			Require(horse.ButcheryProfile.HasBreakdown("") && horse.ButcheryProfile.Products.Any(), "Loaded Horse has no usable breakdown/products.");
			Require(horse.ButcheryProfile.BreakdownCheck("").Trait.Id == skills["arm.skill.skinning"].Id,
				$"Horse breakdown trait {horse.ButcheryProfile.BreakdownCheck("").Trait.Id} != Skinning {skills["arm.skill.skinning"].Id}.");
			Require(longsword.AttackTrait.Id == skills["arm.skill.slashing_weapons"].Id && longsword.Attacks.Any(),
				$"Longsword trait {longsword.AttackTrait.Id} != Slashing {skills["arm.skill.slashing_weapons"].Id}, attacks:{longsword.Attacks.Count()}.");
			Require(razor.AttackTrait.Id == skills["arm.skill.razor_weapons"].Id && razor.Attacks.Any(),
				$"Razor trait {razor.AttackTrait.Id} != Razor Weapons {skills["arm.skill.razor_weapons"].Id}, attacks:{razor.Attacks.Count()}.");
			Console.WriteLine("SETTING-full-loaded=" + JsonSerializer.Serialize(new
			{
				Races = world.Races.Count(), Weapons = world.WeaponTypes.Count(), Health = world.HealthStrategies.Count(),
				Surgery = world.SurgicalProcedures.Count(), Butchery = world.RaceButcheryProfiles.Count(),
				Horse = new { horse.Id, Body = horse.BaseBody.Id, Health = horse.DefaultHealthStrategy.Id,
					Attacks = horse.NaturalWeaponAttacks.Count(), Butchery = horse.ButcheryProfile.Id,
					ButcheryTrait = horse.ButcheryProfile.BreakdownCheck("").Trait.Id },
				LongswordTrait = longsword.AttackTrait.Id, RazorTrait = razor.AttackTrait.Id
			}));
		}
		RunCoreFoundationBoot(database);
		Console.WriteLine("SETTING-full-completion=PASS all-enabled-except-Item-and-alternative-SkillSeeder first-and-cold-native-boot six-playable-profiles");
		return 0;
	}

	private static object FullSettingReplayProfile(object stock, IReadOnlyList<IDatabaseSeeder> selected)
	{
		var overrides = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal)
		{
			["SkillPackageSeeder"] = new() { ["skillpreset"] = "Armageddon", ["branching"] = "no", ["skillcapmodel"] = "class",
				["skillgainmodel"] = "armageddon", ["complexity"] = "armageddon", ["gerund"] = "no", ["modern"] = "no" },
			[nameof(CombatSeeder)] = new() { ["skilloption"] = "armageddon", ["installmuskets"] = "no", ["installguns"] = "no" },
			[nameof(ChargenSeeder)] = new() { ["class"] = "yes", ["subclass"] = "yes" },
			[nameof(ArmageddonSettingSeeder)] = new() { [ArmageddonSettingSeeder.FullInstallQuestion] = "yes", [ArmageddonSettingSeeder.ModeQuestion] = "Simple" }
		};
		var steps = ((IEnumerable)stock.GetType().GetProperty("Steps")!.GetValue(stock)!).Cast<object>().ToArray();
		var stepType = steps[0].GetType();
		var result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(stepType))!;
		foreach (var step in steps)
		{
			var seederType = (Type)stepType.GetProperty("SeederType")!.GetValue(step)!;
			if (seederType == typeof(ItemSeeder)) continue;
			if (!overrides.TryGetValue(seederType.Name, out var replacements)) { result.Add(step); continue; }
			var answers = ((IEnumerable)stepType.GetProperty("Answers")!.GetValue(step)!).Cast<object>().ToArray();
			var answerType = answers[0].GetType();
			var next = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(answerType))!;
			foreach (var answer in answers)
			{
				var id = (string)answerType.GetProperty("Id")!.GetValue(answer)!;
				next.Add(replacements.Remove(id, out var value) ? Activator.CreateInstance(answerType, id, value) : answer);
			}
			Require(replacements.Count == 0, "Full Armageddon replay override question inventory drifted: " + seederType.Name);
			result.Add(Activator.CreateInstance(stepType, seederType, next));
		}
		// Omitting Item also removes its soft ordering edge. Recompute the exact supplied
		// catalogue plan rather than assuming that deleting one step preserves its order.
		var plan = SeederMetadataRegistry.GetDependencyPlan(selected);
		Require(!plan.Errors.Any(), "Full smoke projected dependency catalogue is invalid.");
		var byType = result.Cast<object>().ToDictionary(x => (Type)stepType.GetProperty("SeederType")!.GetValue(x)!);
		var ordered = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(stepType))!;
		foreach (var seeder in plan.OrderedSeeders.Where(x => x.GetType().Name != "SkillSeeder")) ordered.Add(byType[seeder.GetType()]);
		return Activator.CreateInstance(stock.GetType(), "armageddon-all-except-item", "Armageddon full optional-package smoke",
			"Private Simple/classes-on compatibility smoke; only Item and mutually exclusive SkillSeeder omitted.", ordered)!;
	}
#endif
}
