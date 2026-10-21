#nullable enable

using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Celestial;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.RPG.Merits.Interfaces;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record SettingPhysiologyReceipt(long Character, long Body, long SevereRoom, long ShelterRoom,
		double Food, double Drink, double Water, double Blood, double Stamina, string PreservedHash);

	private static SettingPhysiologyReceipt QualifySettingPhysiology(TestDatabase database)
	{
		long severe, shelter;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var source = db.Rooms.AsNoTracking().First(x => x.CurrentOverlayId != null && x.ZoneId > 0);
			var overlay = db.RoomOverlays.AsNoTracking().Single(x => x.Id == source.CurrentOverlayId);
			long Room(string name, string terrain, int outdoors)
			{
				var room = (Db.Room)db.Entry(source).CurrentValues.Clone().ToObject();
				room.Id = 0; room.UniqueName = null; room.CurrentOverlayId = null;
				room.X = db.Rooms.Where(x => x.ZoneId == source.ZoneId).Max(x => x.X) + 1;
				db.Rooms.Add(room); db.SaveChanges();
				var copy = (Db.RoomOverlay)db.Entry(overlay).CurrentValues.Clone().ToObject();
				copy.Id = 0; copy.RoomId = room.Id; copy.RoomName = name;
				copy.TerrainId = db.Terrains.Single(x => x.Name == terrain).Id; copy.OutdoorsType = outdoors;
				db.RoomOverlays.Add(copy); db.SaveChanges(); room.CurrentOverlayId = copy.Id; db.SaveChanges();
				return room.Id;
			}
			severe = Room("Physiology qualification desert", "Sandy Desert", 2);
			shelter = Room("Physiology qualification shelter", "Residence", 0);
		}

		SettingPhysiologyReceipt receipt;
		var clock = new HarnessClock();
		using (RuntimeClock.Push(clock))
		using (var world = LoadProductionRestartWorld(clock))
		using (new FMDB())
		{
			((IFuturemudLoader)world).LoadChargen();
			var account = FMDB.Context.Accounts.OrderBy(x => x.Id).First().Id;
			var culture = FMDB.Context.Cultures.Single(x => x.Name == "Admin").Id;
			var app = NewSettingQualificationApplication(world, account, culture, "QualificationPhysiology");
			FinaliseAndSaveSettingApplication(app, SettingQualificationFreeProg(world));
			var actor = PersistSettingQualificationCharacter(world, app);
			ExerciseSettingPhysiology(database, world, actor, severe, shelter, 20);
			// Persist only an ordinary Human fixture, never a Character made from a gated profile.
			actor.Body.CurrentBloodVolumeLitres = actor.Body.TotalBloodVolumeLitres * 0.75;
			var needs = new ActiveNeedsModel(actor);
			needs.FulfilNeeds(new NeedFulfiller());
			var oldScale = ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds;
			try { ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = 1; needs.NeedsHeartbeat(); }
			finally { ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = oldScale; }
			SetPrivateMember(actor, "NeedsModel", needs);
			actor.Body.CurrentStamina = Math.Min(50, actor.Body.MaximumStamina / 2);
			actor.Changed = true; world.SaveManager.Flush();
			using var saved = NewIndependentContext(database.ConnectionString);
			var row = saved.Characters.AsNoTracking().Single(x => x.Id == actor.Id);
			var body = saved.Bodies.AsNoTracking().Single(x => x.Id == actor.Body.Id);
			Near(row.FoodSatiatedHours, needs.FoodSatiatedHours, "Native needs save food");
			Near(row.DrinkSatiatedHours, needs.DrinkSatiatedHours, "Native needs save drink");
			Near(row.WaterLitres, needs.WaterLitres, "Native needs save water");
			Near(body.CurrentBloodVolume, actor.Body.CurrentBloodVolumeLitres, "Native body save partial blood");
			Near(body.CurrentStamina, actor.Body.CurrentStamina, "Native body save stamina");
			Require(!saved.PerceiverMerits.Any(x => x.BodyId == body.Id || x.CharacterId == actor.Id),
				"Transient intrinsic grants leaked into explicit saved merits.");
			receipt = new(actor.Id, body.Id, severe, shelter, row.FoodSatiatedHours, row.DrinkSatiatedHours,
				row.WaterLitres, body.CurrentBloodVolume, body.CurrentStamina, string.Empty);
			QualifyExecutableNaming(database, world);
		}

		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var ids = SettingQualificationIds(database);
			// Builder-authored subtype data and an extra intrinsic link must survive both cold reruns.
			var merit = db.Merits.Single(x => x.Id == ids["arm.merit.stamina.reserve"]);
			var xml = XElement.Parse(merit.Definition); xml.SetElementValue("Bonus", 23); merit.Definition = xml.ToString();
			var custom = new Db.Merit { Name = "Qualification builder reserve", Type = merit.Type,
				MeritScope = merit.MeritScope, MeritType = merit.MeritType, Definition = merit.Definition };
			var definition = XElement.Parse(custom.Definition); definition.SetElementValue("Bonus", 7); custom.Definition = definition.ToString();
			db.Merits.Add(custom); db.SaveChanges();
			db.RacesMerits.Add(new Db.RacesMerits { RaceId = ids["arm.race.half_giant"], MeritId = custom.Id });
			db.SaveChanges(); receipt = receipt with { PreservedHash = SettingPhysiologyPreservationHash(db) };
		}
		Console.WriteLine("SETTING-physiology=PASS native-loaded-racial-grants native-needs-metabolism environmental-progs stamina no-explicit-leak saved-counters gates-unchanged");
		return receipt;
	}

	private static void CheckSettingPhysiologyCold(TestDatabase database, SettingPhysiologyReceipt receipt)
	{
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(SettingPhysiologyPreservationHash(db) == receipt.PreservedHash,
				"Rerun changed builder intrinsic links or physiology merit subtype data.");
			var row = db.Characters.AsNoTracking().Single(x => x.Id == receipt.Character);
			Near(row.FoodSatiatedHours, receipt.Food, "Cold saved food");
			Near(row.DrinkSatiatedHours, receipt.Drink, "Cold saved drink");
			Near(row.WaterLitres, receipt.Water, "Cold saved water");
			Near(db.Bodies.Single(x => x.Id == receipt.Body).CurrentBloodVolume, receipt.Blood, "Cold saved partial blood");
			Near(db.Bodies.Single(x => x.Id == receipt.Body).CurrentStamina, receipt.Stamina, "Cold saved stamina");
			Require(!db.PerceiverMerits.Any(x => x.BodyId == receipt.Body || x.CharacterId == receipt.Character),
				"Cold rerun materialised intrinsic grants as explicit assignments.");
		}
		var clock = new HarnessClock();
		using var time = RuntimeClock.Push(clock);
		using var world = LoadProductionRestartWorld(clock);
		using var session = new FMDB();
		var actor = world.TryGetCharacter(receipt.Character, true);
		Near(actor.NeedsModel.FoodSatiatedHours, receipt.Food, "Native reloaded food");
		Near(actor.NeedsModel.DrinkSatiatedHours, receipt.Drink, "Native reloaded drink");
		Near(actor.NeedsModel.WaterLitres, receipt.Water, "Native reloaded water");
		Near(actor.Body.CurrentBloodVolumeLitres, receipt.Blood, "Native reloaded partial blood");
		Near(actor.Body.CurrentStamina, receipt.Stamina, "Native reloaded stamina");
		ExerciseSettingPhysiology(database, world, actor, receipt.SevereRoom, receipt.ShelterRoom, 30);
		Console.WriteLine("SETTING-physiology-cold=PASS native-race-link-merit-load actual-consumers saved-counters builder-link-and-definition-preservation no-profile-activation");
	}

	private static string SettingPhysiologyPreservationHash(FuturemudDatabaseContext db)
	{
		var ids = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.StableKey.StartsWith("arm.race.")).Select(x => x.LogicalId!.Value).ToArray();
		var links = db.RacesMerits.AsNoTracking().Where(x => ids.Contains(x.RaceId)).OrderBy(x => x.RaceId).ThenBy(x => x.MeritId)
			.Select(x => new { x.RaceId, x.MeritId }).ToArray();
		var merits = links.Select(x => x.MeritId).Distinct().ToArray();
		return BaseValueHash(new { Links = links, Merits = db.Merits.AsNoTracking().Where(x => merits.Contains(x.Id))
			.OrderBy(x => x.Id).Select(x => new { x.Id, x.Type, x.MeritScope, x.Definition }).ToArray() });
	}

	private static void ExerciseSettingPhysiology(TestDatabase database, IFuturemud world, ICharacter actor,
		long severeRoom, long shelterRoom, double giantBonus)
	{
		var ids = SettingQualificationIds(database);
		var body = (MudSharp.Body.Implementations.Body)actor.Body; var originalRace = body.Race; var location = actor.Location;
		var position = actor.PositionState; var maximum = body.MaximumStamina; var stamina = body.CurrentStamina;
		var scale = ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds;
		void Race(string key) => typeof(MudSharp.Body.Implementations.Body).GetProperty("Race")!
			.SetValue(body, world.Races.Get(ids["arm.race." + key]));
		try
		{
			Race("half_giant"); SetPrivateMember(actor, "Location", null!);
			Require(body.ExplicitMerits.Count() == 0 && actor.Merits.Select(x => x.Id).Distinct().Count() == actor.Merits.Count(),
				"Loaded intrinsic body grants are explicit or counted more than once.");
			var giant = new ActiveNeedsModel(actor); giant.FulfilNeeds(new NeedFulfiller());
			Near(giant.FoodSatiatedHours, 48, "Giant food capacity"); Near(giant.DrinkSatiatedHours, 24, "Giant drink capacity");
			var water = giant.WaterLitres;
			ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = 60; giant.NeedsHeartbeat();
			Near(giant.FoodSatiatedHours, 45, "Giant hunger consumption"); Near(giant.DrinkSatiatedHours, 21, "Giant thirst consumption");
			Near(giant.WaterLitres, Math.Max(0, water - body.WaterLossLitresPerHour * 3), "Giant passive water consumption");
			body.MaximumStamina = 100; body.CurrentStamina = 80;
			Near(body.MaximumStamina, 100 + giantBonus, "Giant loaded intrinsic stamina"); Near(body.CurrentStamina, 80, "Capacity grant refilled stamina");
			Race("desert_elf");
			Near(body.MaximumStamina, 160, "Desert elf loaded intrinsic stamina");
			var reserve = actor.Merits.OfType<IStaminaCapacityMerit>().Single();
			Require(!body.AddMerit(reserve) && !body.ExplicitMerits.Any(), "Intrinsic grant became a sticky explicit body merit.");
			body.CurrentStamina = 150; Race("human"); Near(body.CurrentStamina, 100, "Loss of intrinsic capacity did not clamp");
			Race("desert_elf"); Near(body.CurrentStamina, 100, "Regaining capacity refilled stamina");
			SetPrivateMember(actor, "Location", world.Rooms.Get(severeRoom)); actor.PositionState = PositionStanding.Instance;
			var normal = (IEnvironmentalNeedsMerit)world.Merits.Get(ids["arm.merit.environment.normal"]);
			var desert = (IEnvironmentalNeedsMerit)world.Merits.Get(ids["arm.merit.environment.desert_elf"]);
			var daylight = actor.Location.CurrentTimeOfDay.Describe(); var day = daylight is "Morning" or "Afternoon" ? 1 : 0;
			Near(normal.GetNeedsAdjustment(actor).ThirstRateDelta, 4 + day, "Normal severe-sector native prog");
			Near(desert.GetNeedsAdjustment(actor).ThirstRateDelta, 2 + day, "Desert adaptation native prog");
			actor.PositionState = PositionSitting.Instance;
			Near(desert.GetNeedsAdjustment(actor).ThirstRateDelta, 1 + day, "Native lower-posture credit");
			SetPrivateMember(actor, "Location", world.Rooms.Get(shelterRoom));
			var sheltered = new ActiveNeedsModel(actor); sheltered.FulfilNeeds(new NeedFulfiller());
			var drink = sheltered.DrinkSatiatedHours; sheltered.NeedsHeartbeat();
			Near(sheltered.DrinkSatiatedHours, drink, "Shelter should cancel passive thirst without generation");
			sheltered.FulfilNeeds(new NeedFulfiller { WaterLitres = -0.01, ThirstPoints = -1 });
			Near(sheltered.DrinkSatiatedHours, drink - 1, "Environmental merit changed explicit thirst fulfilment");
		}
		finally
		{
			typeof(MudSharp.Body.Implementations.Body).GetProperty("Race")!.SetValue(body, originalRace);
			SetPrivateMember(actor, "Location", location); actor.PositionState = position;
			body.MaximumStamina = maximum; body.CurrentStamina = stamina;
			ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = scale;
		}
	}

	private static void Near(double actual, double expected, string context) => Require(Math.Abs(actual - expected) < 1e-8,
		$"{context}: expected {expected:R}, actual {actual:R}.");
}
