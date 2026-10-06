#nullable enable

using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Implementations;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Commands.Trees;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record NativeBoardingReader(string Database, FixtureIds Fixture, DateTime Now,
		long Vehicle, long Exterior, long Identity, long? Instance, long Body, double Stamina,
		long Slot, bool Boarded, bool CanonicalNative, string Scenario);

	private static void SeedNativeBoardingFixture(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var revision = new Db.EditableItem { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current };
		var proto = new Db.VehicleProto { Id = 920001, Name = "ARMBoarding cart", Description = "Disposable native boarding cart.", VehicleScale = (int)VehicleScale.ItemScale, EditableItem = revision };
		db.VehicleProtos.Add(proto); db.SaveChanges();
		var compartment = new Db.VehicleCompartmentProto { VehicleProtoId = proto.Id, Name = "deck", Description = "An ordinary deck." };
		db.VehicleCompartmentProtos.Add(compartment); db.SaveChanges();
		var driver = new Db.VehicleOccupantSlotProto { VehicleProtoId = proto.Id, VehicleCompartmentProtoId = compartment.Id, Name = "driver", SlotType = (int)VehicleOccupantSlotType.Driver, Capacity = 1 };
		var passenger = new Db.VehicleOccupantSlotProto { VehicleProtoId = proto.Id, VehicleCompartmentProtoId = compartment.Id, Name = "passenger", SlotType = (int)VehicleOccupantSlotType.Passenger, Capacity = 1 };
		db.VehicleOccupantSlotProtos.AddRange(driver, passenger); db.SaveChanges();
		db.VehicleControlStationProtos.Add(new() { VehicleProtoId = proto.Id, VehicleOccupantSlotProtoId = driver.Id, Name = "driver station", IsPrimary = true });
		db.VehicleMovementProfileProtos.Add(new() { VehicleProtoId = proto.Id, Name = "ordinary", MovementType = (int)VehicleMovementProfileType.CellExit, IsDefault = true, RequiredInstalledRole = "" });
		var exterior = new Db.GameItemComponentProto { Id = db.GameItemComponentProtos.Max(x => x.Id) + 1, Name = "ARMBoarding exterior", Type = "Vehicle Exterior", Description = "Native vehicle exterior.", Definition = $"<Definition><VehiclePrototypeId>{proto.Id}</VehiclePrototypeId></Definition>", EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
		db.GameItemComponentProtos.Add(exterior); db.SaveChanges();
		var item = new Db.GameItemProto { Id = db.GameItemProtos.Max(x => x.Id) + 1, Name = "ARMBoarding cart", Keywords = "cart", ShortDescription = "an acceptance cart", FullDescription = "A disposable native cart.", MaterialId = db.GameItemProtos.First().MaterialId, Size = 1, Weight = 1, BaseItemQuality = (int)ItemQuality.Standard, EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
		item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = exterior.Id });
		db.GameItemProtos.Add(item); db.SaveChanges(); proto.ExteriorItemProtoId = item.Id; proto.ExteriorItemProtoRevision = 0; db.SaveChanges();
	}

	private static void ConfigureNativeBoardingWorld(RetirementHost host, TestDatabase database)
	{
		var native = host.Native; var world = native.World;
		var components = world.ItemComponentProtos;
		using var db = NewIndependentContext(database.ConnectionString);
		var row = db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Name == "ARMBoarding exterior");
		var component = (IGameItemComponentProto)typeof(VehicleExteriorGameItemComponentProto).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(Db.GameItemComponentProto), typeof(IFuturemud)], null)!.Invoke([row, world]);
		var catalogue = new Mock<IUneditableRevisableAll<IGameItemComponentProto>>();
		catalogue.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<int>())).Returns<long, int>((id, revision) => id == component.Id ? component : components.Get(id, revision));
		native.WorldMock.SetupGet(x => x.ItemComponentProtos).Returns(catalogue.Object);
		var itemRow = db.GameItemProtos.Include(x => x.EditableItem).Include(x => x.GameItemProtosGameItemComponentProtos).Include(x => x.GameItemProtosTags).Single(x => x.Name == "ARMBoarding cart");
		host.Prototypes.Add(itemRow.Id, new GameItemProto(itemRow, world));
		var prototypes = new RevisableAll<IVehiclePrototype>();
		var prototype = new VehiclePrototype(db.VehicleProtos.Include(x => x.EditableItem).Include(x => x.Compartments).Include(x => x.OccupantSlots).Include(x => x.ControlStations).Include(x => x.MovementProfiles).ThenInclude(x => x.PropulsionProfiles).AsNoTracking().Single(x => x.Name == "ARMBoarding cart"), world);
		prototypes.Add(prototype); native.WorldMock.SetupGet(x => x.VehiclePrototypes).Returns(prototypes);
		var vehicles = new All<IVehicle>(); native.WorldMock.SetupGet(x => x.Vehicles).Returns(vehicles);
		native.WorldMock.Setup(x => x.Add(It.IsAny<IVehicle>())).Callback<IVehicle>(x => vehicles.Add(x));
		Require(prototype.CanCreateVehicle(out var reason), "Supported native cart factory validation: " + reason);
	}

	private static Vehicle ReloadNativeBoardingVehicle(TestDatabase database, IFuturemud world, long id)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		return new Vehicle(db.Vehicles.Include(x => x.Compartments).Include(x => x.Occupancies).AsNoTracking().Single(x => x.Id == id), world);
	}

	private static int RunNativeBoarding(TestDatabase database, RetirementHost host, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe, Func<ScriptedAiCharacterInstance> cast,
		Action<ScriptedAiCharacterInstance> restored, Action<ScriptedAiCharacterInstance, ICharacter, string> order, FixtureIds fixture)
	{
		using var globals = new CheckLearningGlobals();
		ConfigureNativeBoardingWorld(host, database);
		var native = host.Native; var world = native.World; var service = world.SpellOwnedCorpseAnimations!;
		ConfigureStormHands(native);
		typeof(Body).GetField("_encumbranceLimitExpression", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new TraitExpression("1000", world));
		typeof(CombatBase).GetProperty("RecoveryTimeExpression", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new TraitExpression("1", world));
		foreach (var (name, value) in new[] { ("EncumbranceLimitRatioHeavy", 0.8), ("EncumbranceLimitRatioModerate", 0.5), ("EncumbranceLimitRatioLight", 0.25) }) native.WorldMock.Setup(x => x.GetStaticDouble(name)).Returns(value);
		var settings = (CharacterCombatSettings)caster.CombatSettings;
		settings.WeaponUsePercentage = 1; settings.NaturalWeaponPercentage = settings.AuxiliaryPercentage = settings.MagicUsePercentage = settings.PsychicUsePercentage = 0;
		foreach (var race in world.Races)
		{
			Mock.Get(race).SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings { CanAttack = true, CanUseWeapons = true, CanDefend = false, DefaultCombatSetting = settings });
			Mock.Get(race).SetupGet(x => x.RaceUsesStamina).Returns(true);
		}
		Mock.Get(world.GetCheck(CheckType.CombatRecoveryCheck)).Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>())).Returns(CheckOutcome.SimpleOutcome(CheckType.CombatRecoveryCheck, Outcome.Pass));
		var vehicle = (Vehicle)VehicleFactory.CreateVehicle(world.VehiclePrototypes.Single(), caster.Location, caster.RoomLayer, caster);
		var slot = vehicle.Prototype.OccupantSlots.Single(x => x.SlotType == VehicleOccupantSlotType.Passenger);
		var first = true; var configured = new HashSet<CommandableAI>();
		foreach (var scenario in new[] { "ordered-valid", "queued-revoked", "board-policy-revoked", "independent-valid" })
		{
			var actor = first ? animated : cast(); first = false; var body = (Body)actor.Body; actor.CombatSettings = settings;
			var ai = actor.AIs.OfType<CommandableAI>().Single();
			if (configured.Add(ai)) Require(ai.BuildingCommand(caster, new StringStack("included embark")), "Explicit test whitelist must admit actual embark command.");
			void Expire()
			{
				var grant = service.CommandGrant(actor.InstanceId, caster.Id)!;
				var source = XElement.Parse(XElement.Parse(grant.Provenance).Element("Source")!.Value);
				clock.Advance(DateTime.Parse(source.Element("ControlUntilUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) - RuntimeClock.UtcNow);
				Require(!service.CanCommand(actor.InstanceId, caster.Id) && actor.IsEmbodied, "Only command duration expires; native body remains alive.");
			}
			order(actor, caster, "hit opponent"); actor.RemoveAllEffects<IdleCombatant>(null, true); actor.MeleeRange = false; foe.MeleeRange = false;
			Require(actor.Combat is SimpleMeleeCombat, "Actual native hit must establish combat.");
			var policyField = typeof(CommandableAI).GetField("_canCommandProg", BindingFlags.Instance | BindingFlags.NonPublic)!;
			var policy = (IFutureProg)policyField.GetValue(ai)!; var resolving = false; var callbacks = 0;
			var callbackPolicy = new Mock<IFutureProg>();
			callbackPolicy.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns<object[]>(arguments =>
			{
				if (resolving && scenario == "board-policy-revoked" && callbacks == 0 && new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(Vehicle) && x.GetMethod()?.Name == nameof(Vehicle.Board)))
				{ ++callbacks; Expire(); }
				return policy.ExecuteBool(arguments);
			});
			policyField.SetValue(ai, callbackPolicy.Object);
			try
			{
				var direct = scenario == "independent-valid";
				body.CurrentStamina = 100; world.SaveManager.Flush();
				if (direct) { Expire(); using var independent = CommandExecutionScope.EnterIndependent(); ((ICharacter)actor).ExecuteCommand("embark cart passenger"); }
				else order(actor, caster, "embark cart passenger");
				var move = actor.ChooseMove();
				Require(move is BoardVehicleCombatMove && CommandExecutionAuthority.IsOrdered(move) == !direct, "Actual command/ChooseMove selects native boarding with exact ordered provenance.");
				if (scenario == "queued-revoked") Expire();
				body.CurrentStamina = 100; world.SaveManager.Flush();
				resolving = true; try { actor.Combat!.CombatAction(actor, move); } finally { resolving = false; }
				var boarded = scenario is "ordered-valid" or "independent-valid";
				Require(vehicle.IsOccupant(actor) == boarded && Same(actor.CurrentStamina, boarded ? 95 : 100), "Only committed exact occupancy pays five stamina once.");
				Require(callbacks == (scenario == "board-policy-revoked" ? 1 : 0), "One actual final native Board policy callback.");
				world.SaveManager.Flush();
				var cold = ReloadNativeBoardingVehicle(database, world, vehicle.Id);
				Require(cold.Occupancies.Count() == (boarded ? 1 : 0) && (!boarded || ReferenceEquals(cold.Occupancies.Single().Occupant, actor) && cold.Occupancies.Single().CharacterInstanceId == actor.InstanceId && cold.Occupancies.Single().Slot.Id == slot.Id && !cold.Occupancies.Single().IsController), "Cold native vehicle loading resolves exact existing physical secondary, never canonical fallback.");
				RunItemReaderProcess(new NativeBoardingReader(database.Name, fixture, RuntimeClock.UtcNow, vehicle.Id, vehicle.ExteriorItem.Id, actor.Identity.Id, actor.InstanceId, body.Id, actor.CurrentStamina, slot.Id, boarded, false, scenario), "--native-boarding-reader");
				Console.WriteLine($"ARMNativeBoarding={scenario} passed actual-embark-supported-VehicleFactory-ChooseMove-CombatAction-native-Board cold-native-live-instance fresh-process-SQL cost:{100-actor.CurrentStamina} callbacks:{callbacks}");
			}
			finally { resolving = false; policyField.SetValue(ai, policy); }
			using (CommandExecutionScope.EnterIndependent())
			{
				vehicle.ForceDisembark(actor); Require(service.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var why), why); world.SaveManager.Flush(); restored(actor);
			}
		}
		using (CommandExecutionScope.EnterIndependent())
		{
			Require(caster.Combat is null, "Canonical independent control is outside combat.");
			SetPrivateMember(caster, "CommandTree", PlayerCommandTree.Instance);
			((Body)caster.Body).CurrentStamina = 100; caster.ExecuteCommand("embark cart passenger"); world.SaveManager.Flush();
			Require(vehicle.IsOccupant(caster) && Same(caster.CurrentStamina, 100), "Actual independent out-of-combat embark retains existing zero-cost contract.");
			RunItemReaderProcess(new NativeBoardingReader(database.Name, fixture, RuntimeClock.UtcNow, vehicle.Id, vehicle.ExteriorItem.Id, caster.Id, caster.InstanceId > 0 ? caster.InstanceId : null, caster.Body.Id, caster.CurrentStamina, slot.Id, true, true, "canonical-independent"), "--native-boarding-reader");
			vehicle.ForceDisembark(caster); world.SaveManager.Flush();
			Console.WriteLine("ARMNativeBoarding=canonical-independent passed actual-embark fresh-process-native-Vehicle-occupancy-exterior cost:0");
		}
		return 0;
	}

	private static int RunNativeBoardingReader(string[] arguments)
	{
		Require(arguments.Length == 1, "One native boarding reader payload is required.");
		var payload = JsonSerializer.Deserialize<NativeBoardingReader>(Encoding.UTF8.GetString(Convert.FromBase64String(arguments[0])))!;
		using var database = TestDatabase.OpenExistingOwned(payload.Database); ConfigureNativeDatabase(database.ConnectionString);
		using var db = NewIndependentContext(database.ConnectionString);
		var occupancies = db.VehicleOccupancies.AsNoTracking().Where(x => x.VehicleId == payload.Vehicle).ToArray();
		Require(occupancies.Length == (payload.Boarded ? 1 : 0) && (!payload.Boarded || occupancies.Single().CharacterId == payload.Identity && occupancies.Single().CharacterInstanceId == payload.Instance && occupancies.Single().VehicleOccupantSlotProtoId == payload.Slot && !occupancies.Single().IsController), "Fresh independent SQL reader must retain exact occupancy or refusal.");
		Require(Same(db.Bodies.AsNoTracking().Single(x => x.Id == payload.Body).CurrentStamina, payload.Stamina), "Fresh independent reader sees exact durable stamina.");
		var exterior = db.GameItems.AsNoTracking().Single(x => x.Id == payload.Exterior);
		Require(XElement.Parse(db.GameItemComponents.AsNoTracking().Single(x => x.GameItemId == payload.Exterior).Definition).Element("VehicleId")!.Value == payload.Vehicle.ToString(CultureInfo.InvariantCulture), "Durable native exterior links exact vehicle.");
		if (payload.CanonicalNative)
		{
			var clock = new HarnessClock(); clock.Advance(payload.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
			var host = PrepareRetirementHost(database, payload.Fixture, clock, corpseAnimationAnatomy: true, consumablesAnatomy: true);
			ConfigureNativeBoardingWorld(host, database);
			var cell = CreateAreaCell(host.Native, database.ConnectionString, payload.Fixture.CellId, false); SetPrivateMember(host.Native.Actor, "Location", cell);
			var vehicle = ReloadNativeBoardingVehicle(database, host.Native.World, payload.Vehicle);
			((All<IVehicle>)host.Native.World.Vehicles).Add(vehicle);
			Require(vehicle.Occupancies.Count() == 1 && ReferenceEquals(vehicle.Occupancies.Single().Occupant, host.Native.Actor) && vehicle.Occupancies.Single().CharacterInstanceId == payload.Instance && vehicle.Occupancies.Single().Slot.Id == payload.Slot, "Fresh-process native vehicle resolves exact canonical actor and slot.");
			Require(host.Native.World.TryGetItem(payload.Exterior, true).GetItemType<IVehicleExterior>()!.Vehicle == vehicle, "Fresh native item constructor resolves durable exterior link.");
		}
		Console.WriteLine($"ARMNativeBoarding-reader={payload.Scenario} passed fresh-process exact-row-stamina-exterior native-canonical:{payload.CanonicalNative}");
		return 0;
	}
}
