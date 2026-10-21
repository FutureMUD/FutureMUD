#nullable enable

using System.Collections;
using System.Text;
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
	private sealed record SettingFollowupReader(string Database, BaseStep Skill, string IdentitiesHash,
		long Character, long Profile, long Dwarf, long Mul, long[] History, string HistoryHash, string DifficultiesHash,
		SettingPhysiologyReceipt? Physiology = null);

	private static IDatabaseSeeder FollowupSkillSeeder() => (IDatabaseSeeder)Activator.CreateInstance(
		typeof(ArmageddonSettingSeeder).Assembly.GetType("DatabaseSeeder.Seeders.SkillPackageSeeder", true)!)!;

	private static void QualifySettingFollowups(TestDatabase database, IReadOnlyList<BaseStep> steps)
	{
		QualifyOrdinaryPresetPrefix();
		ConfigureNativeDatabase(database.ConnectionString);
		var skillStep = steps.Single(x => x.SeederType.EndsWith(".SkillPackageSeeder", StringComparison.Ordinal));
		ExecuteBaseTargetStep(database, FollowupSkillSeeder(), skillStep.Answers.ToDictionary(x => x.Id, x => x.Answer), expectedActive: 1);
		using (var db = NewPreparedReplayContext(database.ConnectionString)) RequireSavedPreset(db, "Armageddon");
		foreach (var step in steps.Skip(4))
		{
			var type = typeof(CoreDataSeeder).Assembly.GetType(step.SeederType, true)!;
			ExecuteBaseTargetStep(database, (IDatabaseSeeder)Activator.CreateInstance(type)!, step.Answers.ToDictionary(x => x.Id, x => x.Answer));
		}
		SettingFollowupReader receipt;
		using (var db = NewPreparedReplayContext(database.ConnectionString))
		{
			ArmageddonHeritage.Install(db, new List<string>());
			var owned = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package).ToDictionary(x => x.StableKey, x => x.LogicalId);
			var namingIds = owned.Where(x => x.Key.StartsWith("arm.naming.") && !x.Key.Contains(".gender") && x.Key != "arm.naming.personal").Select(x => x.Value!.Value).ToArray();
			Require(namingIds.Length == 15, "Approved logical naming culture count.");
			Require(db.RandomNameProfiles.Count(x => namingIds.Contains(x.NameCultureId)) == 75, "Five exact native gender profiles per logical tradition.");
			var descriptions = 0;
			foreach (var definition in db.NameCultures.Where(x => namingIds.Contains(x.Id)).Select(x => x.Definition))
			{
				using var curated = JsonDocument.Parse(XElement.Parse(definition).Element("CuratedNamingDefinition")!.Value);
				descriptions += curated.RootElement.GetProperty("culture").GetProperty("elements").GetArrayLength();
			}
			Require(descriptions == 78 && db.NameCultures.Where(x => namingIds.Contains(x.Id)).AsEnumerable().Sum(x => XElement.Parse(x.Definition).Element("Elements")!.Elements().Count()) == 78, "All exact curated and executable descriptions survived MySQL storage.");
			using var manifest = JsonDocument.Parse(typeof(ArmageddonSettingSeeder).Assembly.GetManifestResourceStream("DatabaseSeeder.Armageddon.naming.json")!);
			foreach (var source in manifest.RootElement.GetProperty("cultures").EnumerateArray())
			{
				var nativeId = owned["arm.naming." + source.GetProperty("id").GetString()]!.Value;
				var xml = XElement.Parse(db.NameCultures.Single(x => x.Id == nativeId).Definition);
				using var curated = JsonDocument.Parse(xml.Element("CuratedNamingDefinition")!.Value);
				Require(JsonSerializer.Serialize(source) == JsonSerializer.Serialize(curated.RootElement.GetProperty("culture")), "Native curated description/constraint content differs from the approved manifest.");
				Require(xml.Element("ExactRandomProfileGender")?.Value == "true" && xml.Element("PreserveNameCase")?.Value == "true", "Native exact gender/case opt-in missing.");
				foreach (var component in source.GetProperty("elements").EnumerateArray())
				{
					var native = xml.Element("Elements")!.Elements().Single(x => x.Attribute("Name")!.Value == component.GetProperty("label").GetString());
					Require(new MudSharp.Character.Name.NameCultureElement(native).ChargenBlurb == component.GetProperty("description").GetString(), "Executable native description differs from approved source.");
				}
				var pool = manifest.RootElement.GetProperty("personal_pools").GetProperty(source.GetProperty("personal_pool_id").GetString()!);
				string[] Values(string bucket) => pool.GetProperty(bucket).EnumerateArray().Select(x => x.GetString()!).ToArray();
				var nativeProfiles = db.RandomNameProfiles.Where(x => x.NameCultureId == nativeId).ToArray();
				Require(nativeProfiles.Length == 5 && nativeProfiles.Select(x => x.Gender).Order().SequenceEqual(Enumerable.Range(0, 5)), "Native profile gender inventory differs.");
				foreach (var profile in nativeProfiles)
				{
					IEnumerable<string> expected = profile.Gender switch { 2 => Values("male").Concat(Values("gender_unspecified")), 3 => Values("female").Concat(Values("gender_unspecified")), _ => Values("male").Concat(Values("female")).Concat(Values("gender_unspecified")) };
					if (!expected.Any()) expected = Values("male").Concat(Values("female")).Concat(Values("gender_unspecified"));
					var stored = db.RandomNameProfilesElements.Where(x => x.RandomNameProfileId == profile.Id).Select(x => x.Name).ToArray();
					Require(expected.ToHashSet(StringComparer.Ordinal).SetEquals(stored), "Native per-culture gender pool differs from the manifest.");
				}
			}
			var profiles = db.RandomNameProfiles.Where(x => namingIds.Contains(x.NameCultureId)).Select(x => x.Id).ToArray();
			var forms = db.RandomNameProfilesElements.Where(x => profiles.Contains(x.RandomNameProfileId)).AsEnumerable().ToArray();
			Require(forms.All(x => x.NameUsage == 0 && x.Weighting == 1) && forms.Select(x => x.Name).Distinct(StringComparer.Ordinal).Count() == 321,
				"Personal-only exact-display-form corpus differs after MySQL storage.");
			foreach (var name in new[] { "Ka’Cha", "Ka’Ka’Kyl", "Klik‐Chaka’da", "T’Chai" }) Require(forms.Any(x => x.Name == name), "Unicode form changed: " + name);
			Require(!db.Races.Any(x => x.Name.Contains("Pterran") || x.Name.Contains("Thri-kreen")), "Standalone naming traditions created races.");
			var skills = ArmageddonSkillPackage.Resolve(db); var peek = skills["arm.skill.peek"];
			peek.Name = "Builder Followup Peek"; peek.Expression.Expression = "87";
			var character = db.Characters.Single().Id;
			db.CharacterTraits.Add(new Db.CharacterTrait { CharacterId = character, TraitDefinitionId = peek.Id, Value = 57, AdditionalValue = 3 });
			var profileId = owned["arm.naming.thri_kreen.gender2"]!.Value;
			db.RandomNameProfiles.Single(x => x.Id == profileId).Name = "Builder Followup Names";
			db.RandomNameProfilesElements.Single(x => x.RandomNameProfileId == profileId && x.Name == "Ka’Cha").Weighting = 9;
			db.RandomNameProfilesElements.Remove(db.RandomNameProfilesElements.Single(x => x.RandomNameProfileId == profileId && x.Name == "Ka’Ka’Kyl"));
			db.RandomNameProfilesElements.Add(new Db.RandomNameProfilesElements { RandomNameProfileId = profileId, NameUsage = 0, Name = "Builder’Name", Weighting = 3 });
			db.RandomNameProfilesDiceExpressions.Single(x => x.RandomNameProfileId == profileId && x.NameUsage == 0).DiceExpression = "0";
			var dwarf = db.Races.Single(x => x.Id == owned["arm.race.dwarf"]); var mul = db.Races.Single(x => x.Id == owned["arm.race.mul"]);
			dwarf.SweatRateInLitresPerMinute = 0.007; dwarf.SweatLiquidId = null;
			dwarf.ChildAge = 5; dwarf.YouthAge = 12; dwarf.YoungAdultAge = 21; dwarf.AdultAge = 31; dwarf.ElderAge = 62; dwarf.VenerableAge = 82;
			mul.SweatRateInLitresPerMinute = 0.0;
			var difficultiesHash = FollowupDifficultyHash(db);
			db.SaveChanges();
			var history = db.SeederChoices.Where(x => x.Seeder == FollowupSkillSeeder().Name).Select(x => x.Id).ToArray();
			receipt = new(database.Name, skillStep, BaseValueHash(owned.OrderBy(x => x.Key, StringComparer.Ordinal).ToArray()), character, profileId, dwarf.Id, mul.Id,
				history, FollowupHistoryHash(db, history), difficultiesHash);
		}
		receipt = receipt with { Physiology = QualifySettingPhysiology(database) };
		RunItemReaderProcess(receipt, "--setting-followups-reader");
		RunItemReaderProcess(receipt, "--setting-followups-reader");
		var before = BaseContentHash(database);
		using (var db = NewPreparedReplayContext(database.ConnectionString))
		{
			try { FollowupSkillSeeder().SeedData(db, new Dictionary<string, string> { ["skillpreset"] = "Simple-Gerund", ["skillcapmodel"] = "flat", ["skillgainmodel"] = "rpi", ["branching"] = "yes" });
				throw new InvalidOperationException("Different preset was accepted on the installed Armageddon package."); }
			catch (InvalidOperationException error) when (!error.Message.StartsWith("Different preset was accepted", StringComparison.Ordinal)) { }
		}
		Require(before == BaseContentHash(database), "Rejected changed preset mutated persisted content.");
		Console.WriteLine("SETTING-followups-presets-naming-sweat=PASS one-active-Arm-prompt seven-saved-answers 15-cultures 75-exact-gender-profiles 321-forms 78-descriptions two-cold-process-reruns builder-player-age-zero-sweat-preservation changed-preset-refusal");
	}

	private static void RequireSavedPreset(FuturemudDatabaseContext db, string expected)
	{
		var answers = SeederAnswerMemory.GetLatestSeederAnswers(db, FollowupSkillSeeder().Name);
		Require(answers.Count == 7 && answers["skillpreset"] == expected, "Seven normalized latest preset answers were not persisted.");
		var arm = expected == "Armageddon";
		var legacy = new Dictionary<string, string> { ["complexity"] = arm ? "armageddon" : "complex", ["gerund"] = "no", ["modern"] = arm ? "no" : "yes",
			["skillcapmodel"] = arm ? "class" : "rpi", ["skillgainmodel"] = arm ? "armageddon" : "labmud", ["branching"] = arm ? "no" : "yes" };
		Require(legacy.All(x => answers[x.Key] == x.Value), "Normalized legacy preset policies differ from their exact recorded values.");
	}

	private static string FollowupHistoryHash(FuturemudDatabaseContext db, long[] ids) => BaseValueHash(db.SeederChoices.AsNoTracking()
		.Where(x => ids.Contains(x.Id)).OrderBy(x => x.Id).Select(x => new { x.Id, x.Choice, x.Answer, x.Version }).ToArray());
	private static string FollowupDifficultyHash(FuturemudDatabaseContext db) => BaseValueHash(db.CheckTemplateDifficulties.AsNoTracking()
		.OrderBy(x => x.CheckTemplateId).ThenBy(x => x.Difficulty).Select(x => new { x.CheckTemplateId, x.Difficulty, x.Modifier }).ToArray());

	private static int ReadSettingFollowups(string encoded)
	{
		var input = JsonSerializer.Deserialize<SettingFollowupReader>(Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		ExecuteBaseTargetStep(database, FollowupSkillSeeder(), input.Skill.Answers.ToDictionary(x => x.Id, x => x.Answer), expectedActive: 1);
		using var db = NewPreparedReplayContext(database.ConnectionString);
		ArmageddonHeritage.Install(db, new List<string>()); db.ChangeTracker.Clear();
		CheckExecutableNamingRows(database, db);
		RequireSavedPreset(db, "Armageddon");
		Require(input.History.All(id => db.SeederChoices.Any(x => x.Id == id)) && FollowupHistoryHash(db, input.History) == input.HistoryHash, "Preset resume changed original answer history.");
		Require(FollowupDifficultyHash(db) == input.DifficultiesHash, "Armageddon preserving rerun changed shared check difficulties.");
		var actual = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package).ToDictionary(x => x.StableKey, x => x.LogicalId);
		Require(BaseValueHash(actual.OrderBy(x => x.Key, StringComparer.Ordinal).ToArray()) == input.IdentitiesHash, "Cold rerun changed owned identities.");
		var peek = ArmageddonSkillPackage.Resolve(db)["arm.skill.peek"];
		Require(peek.Name == "Builder Followup Peek" && peek.Expression.Expression == "87" && db.CharacterTraits.Any(x => x.CharacterId == input.Character && x.TraitDefinitionId == peek.Id && x.Value == 57 && x.AdditionalValue == 3), "Cold rerun changed builder/player skill fields.");
		Require(db.RandomNameProfiles.Single(x => x.Id == input.Profile).Name == "Builder Followup Names" &&
			db.RandomNameProfilesElements.Single(x => x.RandomNameProfileId == input.Profile && x.Name == "Ka’Cha").Weighting == 9 &&
			!db.RandomNameProfilesElements.Any(x => x.RandomNameProfileId == input.Profile && x.Name == "Ka’Ka’Kyl") &&
			db.RandomNameProfilesElements.Any(x => x.RandomNameProfileId == input.Profile && x.Name == "Builder’Name" && x.Weighting == 3) &&
			db.RandomNameProfilesDiceExpressions.Single(x => x.RandomNameProfileId == input.Profile && x.NameUsage == 0).DiceExpression == "0", "Cold rerun changed builder name weights/entries/dice.");
		var dwarf = db.Races.Single(x => x.Id == input.Dwarf);
		Require(dwarf.SweatRateInLitresPerMinute == 0.007 && dwarf.SweatLiquidId == null && db.Races.Single(x => x.Id == input.Mul).SweatRateInLitresPerMinute == 0.0, "Cold rerun lost custom/zero sweat data.");
		Require(new[] { dwarf.ChildAge, dwarf.YouthAge, dwarf.YoungAdultAge, dwarf.AdultAge, dwarf.ElderAge, dwarf.VenerableAge }.SequenceEqual(new[] { 5, 12, 21, 31, 62, 82 }), "Cold rerun changed authored test age overrides.");
		var raceIds = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.EntityType == nameof(Db.Race) && x.StableKey.StartsWith("arm.race."))
			.Select(x => x.LogicalId!.Value).Distinct().ToArray();
		var never = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id;
		Require(raceIds.Length == 8, $"Follow-up qualification expected eight distinct physical race records, found {raceIds.Length}.");
		Require(db.Races.Where(x => raceIds.Contains(x.Id)).All(x => x.AvailabilityProgId == never), "Follow-up qualification enabled an incomplete physical profile.");
		Require(input.Physiology is not null, "Cold reader lacks the physiology qualification receipt.");
		CheckSettingPhysiologyCold(database, input.Physiology!);
		Console.WriteLine("SETTING-followups-cold-reader=PASS persistence-and-preserving-installer-only"); return 0;
	}

	private static void QualifyOrdinaryPresetPrefix()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		using (var db = NewPreparedReplayContext(database.ConnectionString)) db.Database.Migrate();
		var profile = BaseProfile(); var assembly = typeof(ArmageddonSettingSeeder).Assembly;
		var seeders = assembly.GetType("DatabaseSeeder.SeederCatalogue", true)!.GetMethod("GetEnabledSeeders", ReplayFlags)!.Invoke(null, null)!;
		var run = assembly.GetType("DatabaseSeeder.SeederReplayRunner", true)!.GetMethod("RunPrefixBefore", ReplayFlags)!;
		Func<FuturemudDatabaseContext> factory = () => NewPreparedReplayContext(database.ConnectionString);
		object Execute() => run.Invoke(null, [profile, seeders, typeof(EconomySeeder), factory, new Version(1, 0), (Action<string>)(x => Console.WriteLine("SETTING-followups-prefix=" + x))])!;
		var first = Execute(); Require((bool)first.GetType().GetProperty("Success", ReplayFlags | System.Reflection.BindingFlags.Instance)!.GetValue(first)!, "Ordinary profile prefix failed: " + first);
		using (var db = factory()) RequireSavedPreset(db, "Complex-NonGerund-Modern");
		var before = BaseContentHash(database); var second = Execute();
		Require(!(bool)second.GetType().GetProperty("Success", ReplayFlags | System.Reflection.BindingFlags.Instance)!.GetValue(second)! &&
			!((IEnumerable)second.GetType().GetProperty("CompletedSeeders")!.GetValue(second)!).Cast<object>().Any() &&
			((string?)second.GetType().GetProperty("Failure")!.GetValue(second))?.Contains("freshly migrated, unseeded", StringComparison.OrdinalIgnoreCase) == true && before == BaseContentHash(database),
			"Actual ordinary replay did not refuse its second attempt without mutation.");
		Console.WriteLine("SETTING-followups-ordinary=PASS actual-profile-prefix-through-SkillPackage actual-second-attempt-refusal unchanged-all-table-content=" + before);
		string difficultyHash;
		using (var db = factory())
		{
			difficultyHash = FollowupDifficultyHash(db);
			var template = db.CheckTemplates.Single(x => x.Name == "Skill Check").Id;
			db.CheckTemplateDifficulties.Remove(db.CheckTemplateDifficulties.First(x => x.CheckTemplateId == template)); db.SaveChanges();
		}
		var step = BaseSteps(profile).Single(x => x.SeederType.EndsWith(".SkillPackageSeeder", StringComparison.Ordinal));
		ExecuteBaseTargetStep(database, FollowupSkillSeeder(), step.Answers.ToDictionary(x => x.Id, x => x.Answer), expectedActive: 4);
		using (var db = factory()) Require(FollowupDifficultyHash(db) == difficultyHash, "Ordinary cold rerun failed to restore the missing difficulty without duplicate keys.");
		Console.WriteLine("SETTING-followups-ordinary-resume=PASS cold-context missing-difficulty-restored exact-shared-difficulty-hash");
	}
}
