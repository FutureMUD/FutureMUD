#nullable enable

using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record SettingPhysicalReceipt(string Profile, long Application, long Character, long Body,
		long Race, long Ethnicity, int Age, string Birthday, string Attributes, string Characteristics,
		double Food, double Drink, double Water, double Blood, double Stamina);

	private static readonly (string Key, int Minimum, int Maximum)[] PhysicalAgePolicies =
	[
		("human", 20, 80), ("city_elf", 24, 93), ("desert_elf", 24, 93), ("dwarf", 44, 178),
		("half_elf", 22, 87), ("half_giant", 36, 145), ("mul", 15, 60)
	];

	private static void RequirePhysicalAdmissionDefaults(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var keys = DatabaseSeeder.Seeders.ArmageddonHeritage.Profiles.Select(x => "arm.race." + x.Key)
			.Append("arm.race.elf").ToArray();
		// Full Setting also owns magic creature races under arm.race.*. This gate is the
		// exact seven heritage profiles plus their common Elf parent, not that wider prefix.
		var records = db.SeederManagedRecords.Where(x => x.Seeder == DatabaseSeeder.Seeders.ArmageddonMagicInstaller.Package &&
			!x.Retired && x.EntityType == nameof(Db.Race) && keys.Contains(x.StableKey)).ToArray();
		var ids = records.Where(x => x.LogicalId.HasValue).Select(x => x.LogicalId!.Value).Distinct().ToArray();
		var never = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id;
		var available = db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue").Id;
		var held = db.Races.Count(x => ids.Contains(x.Id) && x.AvailabilityProgId == never);
		var enabled = records.Where(x => x.StableKey != "arm.race.elf" && x.StableKey != "arm.race.half_elf").Select(x => x.LogicalId!.Value).ToArray();
		Require(new HashSet<string>(records.Select(x => x.StableKey), StringComparer.Ordinal).SetEquals(keys) && ids.Length == 8 && held == 2 && enabled.Length == 6 && db.Races.Count(x => enabled.Contains(x.Id) && x.AvailabilityProgId == available) == 6,
			$"Required heritage admission inventory differs: {records.Length} managed records, {ids.Length} distinct race IDs, {held} unavailable rows; expected six enabled profiles and two held ancestry rows.");
	}

	private static string PhysicalAttributes(IBody body) => BaseValueHash(body.Traits.OrderBy(x => x.Definition.Id)
		.Select(x => new { x.Definition.Id, x.Value }).ToArray());
	private static string PhysicalCharacteristics(ICharacter actor) => BaseValueHash(actor.Race.Characteristics(actor.Gender.Enum)
		.OrderBy(x => x.Id).Select(x => new { x.Id, Value = actor.Body.GetCharacteristic(x, actor)?.Id }).ToArray());

	private static SettingPhysicalReceipt[] QualifySettingPhysical(TestDatabase database, IFuturemud world)
	{
		RequirePhysicalAdmissionDefaults(database);
		var ids = SettingQualificationIds(database);
		var receipts = new List<SettingPhysicalReceipt>();
		using var session = new FMDB();
		var account = FMDB.Context.Accounts.OrderBy(x => x.Id).First().Id;
		var storyboard = world.ChargenStoryboard.StageScreenMap[ChargenStage.SelectBirthday];
		foreach (var (key, minimum, maximum) in PhysicalAgePolicies)
		{
			var race = world.Races.Get(ids["arm.race." + key]);
			var culture = ids[key == "desert_elf" ? "arm.culture.desert_elven" : "arm.culture.allanaki"];
			var app = NewSettingQualificationApplication(world, account, culture, "QualificationPhysical" + key.Replace("_", ""), race);
			var model = race.DefaultHeightWeightModel(app.SelectedGender);
			// Fixed representative fixture dimensions, not distribution or range qualification.
			app.SelectedHeight = model.MeanHeight; app.SelectedWeight = model.MeanWeight!.Value;
			Require(!race.ChargenAvailable(app), "A physical profile became admitted through fixture construction.");
			var screen = storyboard.GetScreen(app);
			Require(screen.HandleCommand((minimum - 1).ToString()).Contains("must select an age", StringComparison.OrdinalIgnoreCase), "Age screen accepted below the approved creation minimum.");
			Require(screen.HandleCommand((maximum + 1).ToString()).Contains("must select an age", StringComparison.OrdinalIgnoreCase), "Age screen accepted above the approved creation maximum.");
			screen.HandleCommand(maximum.ToString()); screen.HandleCommand("random");
			Require(screen.State == ChargenScreenState.Complete, "Age screen rejected the approved maximum.");
			screen = storyboard.GetScreen(app); screen.HandleCommand(minimum.ToString()); screen.HandleCommand("random");
			Require(screen.State == ChargenScreenState.Complete, "Age screen rejected the approved minimum.");
			app.SelectedSdesc = "an &age $frame &race";
			FinaliseAndSaveSettingApplication(app, SettingQualificationFreeProg(world));
			var character = PersistSettingQualificationCharacter(world, app);
			Require(character.AgeInYears == minimum && race.DescribeAge(character.AgeInYears) == "adult", "Native birthday/age description differs from the approved creation age.");
			Require(character.Body.ParseCharacteristics("&age", character) == "adult", "Live age markup did not use the configured descriptive band.");
			Require(race.BodypartDamageMultiplier == 1 && race.PainToleranceModifier == 1 && character.Body.TotalBloodVolumeLitres > 0,
				"Native physical body health/blood initialization failed.");
			var bonus = key == "desert_elf" ? 60 : key is "half_giant" or "mul" ? 20 : 0;
			Near(character.Body.MaximumStamina, MudSharp.Character.Character.BaseMaximumStaminaFor(character) + bonus, "Native profile stamina capacity");
			foreach (var definition in race.Characteristics(character.Gender.Enum).Where(x => x.Name is "Hair Style" or "Facial Hair Style"))
				if (key is "dwarf" or "mul")
					Require(character.PossibleStyles(definition).All(x => x.GrowthStage == 0), "Hairless native profile permits normal regrowth/styling.");
			character.Body.CurrentBloodVolumeLitres = character.Body.TotalBloodVolumeLitres * 0.75;
			var needs = new ActiveNeedsModel(character); needs.FulfilNeeds(new NeedFulfiller());
			var oldScale = ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds;
			var location = character.Location;
			try
			{
				SetPrivateMember(character, "Location", null!); ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = 60;
				needs.NeedsHeartbeat();
				Near(needs.FoodSatiatedHours, key == "half_giant" ? 45 : 15, "Native profile hunger consumption");
				Near(needs.DrinkSatiatedHours, key == "half_giant" ? 21 : 7, "Native profile thirst consumption");
			}
			finally { SetPrivateMember(character, "Location", location); ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = oldScale; }
			SetPrivateMember(character, "NeedsModel", needs);
			character.Body.CurrentStamina = character.Body.MaximumStamina / 2;
			character.Changed = true; world.SaveManager.Flush();
			Require(!FMDB.Context.PerceiverMerits.Any(x => x.CharacterId == character.Id || x.BodyId == character.Body.Id), "Profile intrinsic merit leaked into explicit assignments.");
			receipts.Add(new(key, app.Id, character.Id, character.Body.Id, race.Id, character.Ethnicity.Id, minimum,
				character.Birthday.GetDateString(), PhysicalAttributes(character.Body), PhysicalCharacteristics(character),
				needs.FoodSatiatedHours, needs.DrinkSatiatedHours, needs.WaterLitres, character.Body.CurrentBloodVolumeLitres, character.Body.CurrentStamina));
		}
		RequirePhysicalAdmissionDefaults(database);
		Console.WriteLine("SETTING-physical=PASS seven-real-profile-characters native-age-picker native-bodies descriptions health styling needs stamina saved-admission-gates-unchanged");
		return receipts.ToArray();
	}

	private static void CheckSettingPhysicalCold(TestDatabase database, IFuturemud world, SettingPhysicalReceipt[] receipts)
	{
		Require(receipts.Length == 7, "Physical cold receipt lacks the seven approved profiles.");
		using var session = new FMDB();
		foreach (var receipt in receipts)
		{
			var actor = world.TryGetCharacter(receipt.Character, true);
			Require(actor.Body.Id == receipt.Body && actor.Race.Id == receipt.Race && actor.Ethnicity.Id == receipt.Ethnicity &&
				actor.AgeInYears == receipt.Age && actor.Birthday.GetDateString() == receipt.Birthday, "Cold native profile identity/birthday changed.");
			Require(PhysicalAttributes(actor.Body) == receipt.Attributes && PhysicalCharacteristics(actor) == receipt.Characteristics,
				"Cold native profile attributes/appearance changed.");
			Near(actor.NeedsModel.FoodSatiatedHours, receipt.Food, "Cold profile food"); Near(actor.NeedsModel.DrinkSatiatedHours, receipt.Drink, "Cold profile drink");
			Near(actor.NeedsModel.WaterLitres, receipt.Water, "Cold profile water"); Near(actor.Body.CurrentBloodVolumeLitres, receipt.Blood, "Cold profile blood");
			Near(actor.Body.CurrentStamina, receipt.Stamina, "Cold profile stamina");
			var row = FMDB.Context.Chargens.AsNoTracking().Single(x => x.Id == receipt.Application);
			var app = new Chargen(row, world, FMDB.Context.Accounts.Single(x => x.Id == row.AccountId));
			Require(app.SelectedRace.Id == receipt.Race && app.SelectedBirthday.GetDateString() == receipt.Birthday &&
				app.SelectedCharacteristics.All(x => x.Item2 is not null), "Native chargen profile save/resume changed race, birthday or appearance.");
		}
		RequirePhysicalAdmissionDefaults(database);
		Console.WriteLine("SETTING-physical-cold=PASS seven-profile-bodies birthdays attributes appearance needs blood stamina chargen-resume six-playable-profiles");
	}
}
