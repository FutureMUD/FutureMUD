#nullable enable

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Decorators;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using Db = MudSharp.Models;
using RuntimeBody = MudSharp.Body.Implementations.Body;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record ConsumableReader(string Database, FixtureIds Fixture, DateTime Now, long Partial, long Exhausted,
		long Permanent, long Light, long Vessel, long TransferVessel, long ForeignGear, Guid[] Origins, double FoodHours, double Water, double Volume, string Action);

	private sealed class ConsumableGlobals : IDisposable
	{
		private readonly FieldInfo _layer = typeof(RuntimeBody).GetField("_maximumLayerWeight", BindingFlags.NonPublic | BindingFlags.Static)!;
		private readonly object? _oldLayer;
		private readonly double _oldTime = ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds;
		public ConsumableGlobals() { _oldLayer = _layer.GetValue(null); _layer.SetValue(null, 5.0); ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = 1; }
		public void Dispose() { _layer.SetValue(null, _oldLayer); ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = _oldTime; }
	}

	private static void SeedConsumables(TestDatabase database, FixtureIds fixture, long material)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var actor = db.Characters.Find(fixture.CharacterId)!; actor.NeedsModel = "Active"; actor.FoodSatiatedHours = 1; actor.DrinkSatiatedHours = 1; actor.WaterLitres = 0.05;
		var decorator = new Db.StackDecorator { Name = "ARM03C2 food bites", Description = "Replacement food presentation", Type = "Bites",
			Definition = "<Definition><Range Min='0' Max='99' Item='partly eaten {0}'/></Definition>" };
		db.StackDecorators.Add(decorator);
		foreach (var name in new[] { "water", "wine", "oil" }) db.Liquids.Add(new()
		{
			Name = "ARM03C2 " + name, Description = name, LongDescription = name, TasteText = name, VagueTasteText = name,
			SmellText = name, VagueSmellText = name, TasteIntensity = 1, SmellIntensity = 1, Density = 1000, Viscosity = 1,
			DisplayColour = "white", WaterLitresPerLitre = name == "water" ? 1 : name == "wine" ? 0.88 : 0,
			AlcoholLitresPerLitre = name == "wine" ? 0.12 : 0, DrinkSatiatedHoursPerLitre = name == "oil" ? 0 : 2,
			DampDescription = "damp", WetDescription = "wet", DrenchedDescription = "drenched", DampShortDescription = "damp",
			WetShortDescription = "wet", DrenchedShortDescription = "drenched", RelativeEnthalpy = 1
		});
		var profile = new Db.WearProfile { Name = "ARM03C2 around recipient", Type = "Direct", BodyPrototypeId = db.Bodies.Find(fixture.BodyId)!.BodyPrototypeId,
			Description = "Replacement hovering-light location", WearStringInventory = "hovering around", WearAction1st = "wear", WearAction3rd = "wears", WearAffix = "around",
			WearlocProfiles = $"<Profiles><Profile Bodypart='{2000000000L + db.Bodies.Find(fixture.BodyId)!.BodyPrototypeId * 100 + 2}' Mandatory='true' Transparent='true' NoArmour='true' PreventsRemoval='false' HidesSevered='false'/></Profiles>" };
		db.WearProfiles.Add(profile); db.SaveChanges();
		db.Planes.Add(new() { Name = "ARM03C2 configured water-plane fixture", Alias = "fixturewater", Description = "Authored replacement water-plane admission fixture", IsDefault = false }); db.SaveChanges();
		long componentId = db.GameItemComponentProtos.Max(x => x.Id), prototypeId = db.GameItemProtos.Max(x => x.Id);
		Db.GameItemComponentProto Component(string type, string definition, string? persistedType = null)
		{
			var model = new Db.GameItemComponentProto { Id = ++componentId, Name = "ARM03B2B C2 " + type, Type = persistedType ?? type, Description = "ARM03C2 labeled replacement content",
				Definition = definition, EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			db.GameItemComponentProtos.Add(model); db.SaveChanges(); return model;
		}
		var hold = db.GameItemComponentProtos.Single(x => x.Name == "ARM03B2B Holdable");
		var food = Component("Food", $"<Definition Satiation='2' Water='0' Thirst='0' Alcohol='0' Bites='4' Decorator='{decorator.Id}'><Taste>mild bread</Taste><OnEatProg>0</OnEatProg></Definition>");
		var wear = Component("Wearable", $"<Definition Bulky='false' DisplayInventoryWhenWorn='true'><Profiles Default='{profile.Id}'><Profile>{profile.Id}</Profile></Profiles><LayerWeightConsumption>1</LayerWeightConsumption></Definition>");
		var light = Component("ProgLight", "<Definition><IlluminationProvided>40</IlluminationProvided></Definition>", persistedType: "Prog Light");
		var vessel = Component("LiquidContainer", "<Definition LiquidCapacity='5000' Closable='true' Transparent='true' WeightLimit='10000' OnceOnly='false' CanBeEmptiedWhenInRoom='true'/>");
		foreach (var (name, components) in new[] { ("meal", new[] { hold, food }), ("light", new[] { hold, wear, light }), ("vessel", new[] { hold, vessel }), ("gear", new[] { hold, wear }) })
		{
			var proto = new Db.GameItemProto { Id = ++prototypeId, Name = "ARM03B2B C2 " + name, Keywords = name, ShortDescription = "a replacement " + name,
				FullDescription = "ARM03C2 authored replacement content. Numerical stats are not asserted as recovered historical data.",
				MaterialId = material, Size = 1, Weight = 0.1, BaseItemQuality = (int)ItemQuality.Standard,
				EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			foreach (var component in components) proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id });
			db.GameItemProtos.Add(proto); db.SaveChanges();
		}
	}

	private static void ConfigureConsumablesWorld(NativeRuntime native, string connection)
	{
		using var db = NewIndependentContext(connection);
		var configs = db.StaticConfigurations.ToDictionary(x => x.SettingName, x => x.Definition);
		native.WorldMock.Setup(x => x.GetStaticConfiguration(It.IsAny<string>())).Returns<string>(x => configs.GetValueOrDefault(x) ?? "0");
		native.WorldMock.Setup(x => x.GetStaticConfiguration("BaseFluidUOMToLitres")).Returns("0.001");
		native.WorldMock.SetupGet(x => x.UnitManager).Returns(new MudSharp.Framework.Units.UnitManager(native.World));
		var planes = new All<MudSharp.Planes.IPlane>(); foreach (var row in db.Planes.AsNoTracking()) planes.Add(new MudSharp.Planes.Plane(row, native.World));
		native.WorldMock.SetupGet(x => x.Planes).Returns(planes);
		native.WorldMock.SetupGet(x => x.DefaultPlane).Returns(planes.GetByName("ARM03C2 configured water-plane fixture")!);
		var liquids = new All<ILiquid>(); foreach (var row in db.Liquids.AsNoTracking()) liquids.Add(new Liquid(row, native.World));
		native.WorldMock.SetupGet(x => x.Liquids).Returns(liquids);
		var decorators = new All<IStackDecorator>(); foreach (var row in db.StackDecorators.AsNoTracking()) decorators.Add(StackDecorator.LoadStackDecorator(row));
		native.WorldMock.SetupGet(x => x.StackDecorators).Returns(decorators);
		var profiles = new All<IWearProfile>(); foreach (var row in db.WearProfiles.AsNoTracking().Where(x => x.Name.StartsWith("ARM03C2"))) profiles.Add(WearProfile.LoadWearProfile(row, native.World));
		native.WorldMock.SetupGet(x => x.WearProfiles).Returns(profiles);
		var race = Mock.Get(native.Body.Race); race.SetupGet(x => x.MaximumFoodSatiatedHours).Returns(100); race.SetupGet(x => x.MaximumDrinkSatiatedHours).Returns(100);
		race.Setup(x => x.CanEatFoodMaterial(It.IsAny<ISolid>())).Returns(true); race.Setup(x => x.GetMaximumLiftWeight(It.IsAny<ICharacter>())).Returns(10000);
		race.SetupGet(x => x.NaturalPerceptionTypes).Returns(PerceptionTypes.DirectVisual);
		Mock.Get(native.Actor.Location).SetupGet(x => x.Perceivables)
			.Returns(() => native.Actor.Location.GameItems.Cast<IPerceivable>().Append(native.Actor));
		SetPrivateMember(native.Actor, "NeedsModel", new ActiveNeedsModel(db.Characters.Find(native.Actor.Id)!, native.Actor));
		SetPrivateField(native.Actor, "_dubs", new List<IDub>());
		native.Actor.SetNoSave(false);
		foreach (var hand in native.Body.HoldLocs)
			Mock.Get(hand).Setup(x => x.CanGrab(It.IsAny<IGameItem>(), It.IsAny<IInventory>()))
				.Returns(() => native.Body.FreeHands.Contains(hand) ? WearlocGrabResult.Success : WearlocGrabResult.FailFull);
		native.Body.CalculateOrganFunctions(initialCalculation: true);
		var esophagus = native.Body.Organs.OfType<EsophagusProto>().Single();
		Require(native.Body.HitpointsForBodypart(esophagus) > 0 && native.Body.OrganFunction<EsophagusProto>() >= 0.5,
			"Consumable fixture requires a real healthy esophagus initialized through native organ calculation.");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static WeakReference[] AbandonEmptyConsumablePlans(MagicSpell spell, ICharacter actor)
	{
		return Enumerable.Range(0, 16).Select(_ =>
		{
			var plan = spell.InventoryPlanTemplate.CreatePlan(actor);
			Require(plan.AssociatedEffects.Count == 0, "Finalizer fixture must use real unexecuted empty plans.");
			return new WeakReference(plan);
		}).ToArray();
	}

	private static int RunCreatedConsumablesChecks()
	{
		using var globals = new ConsumableGlobals();
		using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		Console.WriteLine($"ARM03C2-created={database.Name}");
		var fixture = FixtureSeed.Create(database, "arm03c2_consumables", true);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id); SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
		SeedConsumables(database, fixture, seed.World.Materials.First().Id);
		var host = PrepareRetirementHost(database, fixture, clock, wielding: true, consumablesAnatomy: true); var native = host.Native; var actor = native.Actor; var world = native.World;
		var owned = new SpellOwnedItemService(world); native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(owned);
		GameItemProto Proto(string name) => host.Prototypes.Values.Single(x => x.Name == "ARM03B2B C2 " + name);
		GameItem New(string name) { var x = (GameItem)Proto(name).CreateNew(actor); world.Add(x); actor.Location.Insert(x, true); x.Login(); world.SaveManager.Flush(); return x; }
		var cap = (SkillLevelBasedMagicCapability)native.Capability; var skill = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		MagicSpell Spell(string name, string trigger, string[] effects)
		{
			var x = new MagicSpell("ARM03C2 " + name, cap.School); ((All<IMagicSpell>)world.MagicSpells).Add(x);
			foreach (var command in new[] { "trigger new " + trigger, $"trait {skill.Id}", "difficulty easy", "threshold minorpass", "duration ARM02 Duration",
				$"cost {native.Resource.Id} ARM02 Cost", $"prog {world.FutureProgs.GetByName("rejuvenation_known")!.Id}", "castemote A created fixture takes shape.", "failcastemote The fixture fails.", "grades fixture" }.Concat(effects))
				Require(x.BuildingCommand(actor, new StringStack(command)), "Consumable builder refused " + command);
			foreach (var command in new[] { $"casting trait {skill.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive", $"casting entry add {x.Id}", $"casting entry starting {x.Id} on", "casting enable on" })
				Require(cap.BuildingCommand(actor, new StringStack(command)), "Consumable route builder refused " + command);
			return x;
		}
		var foodSpell = Spell("Sustain Meal replacement", "room", ["effect add createitem", $"effect 1 item {Proto("meal").Id}", "effect 1 lifecycle temporarycleanup", "effect 1 family sustain-meal", "effect 1 count grade", "effect 1 lifetime 1800*grade"]);
		var lightSpell = Spell("Hovering Light replacement", "self", ["effect add createitem", $"effect 1 item {Proto("light").Id}", "effect 1 lifecycle temporarycleanup", "effect 1 family hovering-light", "effect 1 placement wornlight", "effect 1 lifetime 600*grade"]);
		var invalidLightSpell = Spell("invalid permanent light alternate", "room", ["effect add createitem", $"effect 1 item {host.Prototypes.Values.Single(x => x.Name == "ARM03B2B flame knife").Id}", "effect 1 lifecycle temporarycleanup", "effect 1 family invalid-light", "effect 1 lifetime 60"]);
		var water = world.Liquids.GetByName("ARM03C2 water")!; var wine = world.Liquids.GetByName("ARM03C2 wine")!; var oil = world.Liquids.GetByName("ARM03C2 oil")!;
		var waterSpell = Spell("Create Water replacement", "item", ["effect add createliquid", $"effect 1 liquid {water.Id}", "effect 1 containerfill on", "effect 1 litres 0.5*grade", $"effect 1 compatible {wine.Id}", $"effect 1 bonusplane {world.DefaultPlane.Id} 2"]);
		var wineSpell = Spell("Draw Wine replacement", "item", ["effect add createliquid", $"effect 1 liquid {wine.Id}", "effect 1 containerfill on", "effect 1 litres 0.25*grade", $"effect 1 compatible {water.Id}"]);
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]); world.SaveManager.Flush();
		var staff = new Mock<ICharacter>(); staff.SetupGet(x => x.Id).Returns(999); staff.Setup(x => x.IsAdministrator(MudSharp.Accounts.PermissionLevel.JuniorAdmin)).Returns(true);
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1, flush: () => { world.SaveManager.Flush(); FlushCasting(native); }); native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff.Object, actor, cap.Id, "Consumable native checkpoint").Allowed, "Consumable enrolment failed.");
		foreach (var spell in new[] { foodSpell, lightSpell, waterSpell, wineSpell, invalidLightSpell })
			new MagicCastingStateStore().Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		Require(invalidLightSpell.BuildingCommand(actor, new StringStack($"effect 1 permanent 7 {Proto("light").Id}")), "Alternate light refusal fixture edit failed.");
		actor.SetTraitValue(skill, 100);
		MagicCastingResult Cast(MagicSpell spell, int grade, string target = "") { actor.AddResource(native.Resource, 100); FlushCasting(native); return casting.Cast(new(actor, cap.Id, spell.Id, grade, false, target)); }
		actor.AddResource(native.Resource, 100); FlushCasting(native); var beforeAlternate = actor.MagicResourceAmounts[native.Resource];
		var alternate = casting.Cast(new(actor, cap.Id, invalidLightSpell.Id, 7, false, ""));
		Require(alternate.Status == MagicCastingStatus.Refused && alternate.OperationId is null && alternate.Message.Contains("overrides cannot select lights") &&
			actor.MagicResourceAmounts[native.Resource] == beforeAlternate && !host.Items.Any(x => x.Prototype.Id == Proto("light").Id), "Permanent alternate bypassed light placement admission or spent payment.");
		Console.WriteLine("ARM03C2-alternate-light-refusal=passed native-approved-weapon-with-approved-light-grade-seven-alternate cell-target-refused-before-payment-operation-creation-or-exposure mandatory-worn-admission-conserved");
		Require(invalidLightSpell.BuildingCommand(actor, new StringStack("effect 1 permanent none")), "Alternate light refusal fixture restoration failed.");
		var meals = new List<IGameItem>(); var origins = new List<Guid>();
		for (var grade = 1; grade <= 7; ++grade)
		{
			var result = Cast(foodSpell, grade); Require(result.Status == MagicCastingStatus.Succeeded, "Food casting failed: " + result.Message);
			var batch = host.Items.Where(x => x.Prototype.Id == Proto("meal").Id && !meals.Contains(x)).ToArray();
			Require(batch.Length == grade && batch.All(x => x.SpellCreationOrigin?.DeadlineUtc == RuntimeClock.UtcNow.AddSeconds(1800 * grade)), "Food count or declared replacement deadline differed from selected grade.");
			meals.AddRange(batch); origins.AddRange(batch.Select(x => x.SpellCreationOrigin!.LifecycleId));
		}
		Require(meals.Count == 28 && origins.Distinct().Count() == 28, "Grade-count outputs did not each own exactly one origin.");
		Console.WriteLine("ARM03C2-grade-food=passed real-paid-native-casts grades-one-through-seven 28-separate-food-objects exact-single-leaf-claims selected-grade-count common-grade-deadlines replacement-1-second-per-event-unit-declared no-mon-permanence");
		var foreignActorEffect = new MudSharp.Effects.Concrete.SpellEffects.SpellInvisibilityEffect(actor,
			new MudSharp.Effects.Concrete.MagicSpellParent(actor, foodSpell, actor, SpellPower.Weak, default), null!);
		actor.AddEffect(foreignActorEffect);
		var finalizerFood = actor.NeedsModel.FoodSatiatedHours; var finalizerWater = actor.NeedsModel.WaterLitres;
		var finalizerDrink = actor.NeedsModel.DrinkSatiatedHours; var finalizerBalance = actor.MagicResourceAmounts[native.Resource];
		var abandonedPlans = AbandonEmptyConsumablePlans(foodSpell, actor);
		GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
		Require(abandonedPlans.All(x => !x.IsAlive) && actor.Effects.Contains(foreignActorEffect) &&
			actor.NeedsModel.FoodSatiatedHours == finalizerFood && actor.NeedsModel.WaterLitres == finalizerWater &&
			actor.NeedsModel.DrinkSatiatedHours == finalizerDrink && actor.MagicResourceAmounts[native.Resource] == finalizerBalance,
			"Empty-plan native finalization retained plans or changed foreign actor effects, needs or payment.");
		actor.RemoveEffect(foreignActorEffect);
		Console.WriteLine("ARM03C2-empty-plan-finalization=passed real-abandoned-native-inventory-plans forced-GC-finalizers-collected foreign-actor-effect-needs-and-resource-balance-conserved");
		var partial = meals[0]; var edible = partial.GetItemType<IEdible>()!;
		native.Body.Get(partial, silent: true); var before = actor.NeedsModel.FoodSatiatedHours;
		Require(native.Body.Eat(edible, null, null, 1.25, null) && edible.BitesRemaining == 2.75 && actor.NeedsModel.FoodSatiatedHours == before + 0.625, "Native partial eating did not conserve bites and nutrition.");
		world.SaveManager.Flush();
		var exhausted = meals[1]; native.Body.Get(exhausted, silent: true);
		var effect = new MudSharp.Effects.Concrete.SpellEffects.SpellInvisibilityEffect(exhausted, new MudSharp.Effects.Concrete.MagicSpellParent(exhausted, foodSpell, actor, SpellPower.Weak, default), null!); exhausted.AddEffect(effect);
		Require(native.Body.SilentEat(exhausted.GetItemType<IEdible>()!, 0) && !exhausted.Deleted && exhausted.GetItemType<IEdible>()!.BitesRemaining == 0, "Controlled held deletion did not retain exhausted zero remainder.");
		Require(!native.Body.SilentEat(exhausted.GetItemType<IEdible>()!, 0), "Exhausted held food delivered nutrition twice.");
		exhausted.RemoveEffect(effect); world.SaveManager.Flush();
		var permanentOrigin = new SpellLifecycleOrigin(Guid.NewGuid(), foodSpell.Id, 7, actor.Id, "ordinary-permanent-food-fixture", SpellLifecycleMode.Permanent, RuntimeClock.UtcNow, null, "No expiry does not prevent ordinary consumption.");
		var permanent = owned.Create(Proto("meal"), actor, ItemQuality.Standard, permanentOrigin); world.Add(permanent); actor.Location.Insert(permanent, true); permanent.Login();
		Require(native.Body.SilentEat(permanent.GetItemType<IEdible>()!, 0) && permanent.Deleted && host.Store.Find(permanentOrigin.Id)!.State == SpellLifecycleState.Completed, "Permanent native food could not be ordinarily consumed.");
		Console.WriteLine("ARM03C2-native-eating=passed real-Body-Eat-fractional-bites nutrition-before-and-after native-SilentEat-complete held-retirement-zero-remainder no-second-meal ordinary-permanent-food-consumed no-recreation");
		var gear = New("gear"); gear.Get(native.Body); native.Body.WearExternally(gear); Require(native.Body.WornItems.Contains(gear), "Foreign gear fixture was not worn.");
		Require(!native.Body.FreeHands.Any(), "Worn-light fixture must have both native hands occupied.");
		var handBalance = actor.MagicResourceAmounts[native.Resource];
		var handRefusal = casting.Cast(new(actor, cap.Id, lightSpell.Id, 3, false, ""));
		Require(handRefusal.Status == MagicCastingStatus.Refused && handRefusal.OperationId is null &&
			actor.MagicResourceAmounts[native.Resource] == handBalance, "Ordinary casting bypassed the native free-hand manipulation rule.");
		// Effect placement can address an occupied recipient independently of the caster's manipulation admission.
		var occupiedCopy = (MagicSpell)typeof(MagicSpell).GetMethod("CastingCopy", BindingFlags.NonPublic | BindingFlags.Instance)!
			.Invoke(lightSpell, [actor, skill, 3, SpellPower.Weak, MudSharp.RPG.Checks.Difficulty.Easy, 7])!;
		Require(((CreateItemEffect)occupiedCopy.SpellEffects.Single()).TryPrepareApplication(actor, actor, default, SpellPower.Weak,
			TimeSpan.Zero, out var occupiedApplication, out _), "Native worn-light effect refused an occupied recipient.");
		occupiedApplication!.Create(null!);
		var occupiedLight = host.Items.Single(x => x.Prototype.Id == Proto("light").Id); world.SaveManager.Flush();
		Require(native.Body.WornItems.Contains(occupiedLight) && !native.Body.FreeHands.Any() && actor.IlluminationProvided == 40 &&
			native.Body.WornItems.Contains(gear), "Native effect placement consumed a hand or lost occupied-recipient illumination/foreign gear.");
		occupiedLight.Delete(); world.SaveManager.Flush();
		Require(occupiedLight.Deleted && host.Store.Find(occupiedLight.SpellCreationOrigin!.LifecycleId)!.State == SpellLifecycleState.Completed &&
			actor.IlluminationProvided == 0 && native.Body.WornItems.Contains(gear), "Occupied-recipient placement fixture did not retire its exact auxiliary light.");
		Console.WriteLine("ARM03C2-worn-light-placement=passed actual-native-effect-application recipient-both-hands-occupied native-wear-and-40-lux foreign-gear-preserved auxiliary-exact-removal normal-casting-free-hand-admission-retained-no-payment");
		native.Body.Drop(exhausted, silent: true); Require(native.Body.FunctioningFreeHands.Any(), "Paid light casting requires a functioning free manipulation hand.");
		var litResult = Cast(lightSpell, 3); Require(litResult.Status == MagicCastingStatus.Succeeded, "Worn light cast failed: " + litResult.Message);
		var light = host.Items.Single(x => x.Prototype.Id == Proto("light").Id); origins.Add(light.SpellCreationOrigin!.LifecycleId);
		Require(native.Body.WornItems.Contains(light) && light.GetItemType<IProduceLight>()!.CurrentIllumination == 40 && actor.IlluminationProvided == 40 && native.Body.WornItems.Contains(gear) &&
			light.SpellCreationOrigin!.DeadlineUtc == RuntimeClock.UtcNow.AddSeconds(600 * 3), "Created native light was not lit, worn, correctly timed or contributing actual recipient illumination.");
		Console.WriteLine("ARM03C2-worn-light=passed real-paid-native-self-cast native-wearable-profile initial-lit-state-persisted real-worn-membership recipient-40-lux no-hand-required foreign-worn-gear-preserved replacement-timing-declared");
		var vessel = New("vessel"); var container = vessel.GetItemType<ILiquidContainer>()!; container.Open();
		Require(actor.CanSee(vessel) && ReferenceEquals(actor.TargetItem("vessel"), vessel), "Native item parser must discover the real visible room vessel.");
		var fill = Cast(waterSpell, 3, "vessel"); Require(fill.Status == MagicCastingStatus.Succeeded && container.LiquidVolume == 3000, "Empty native water fill or configured plane doubling failed: " + fill.Message);
		Require(native.Body.SilentDrink(container, 250) && container.LiquidVolume == 2750 && actor.NeedsModel.WaterLitres > 0.05, "Native drinking did not fulfill needs and reduce conserved mixture.");
		fill = Cast(wineSpell, 7, "vessel"); Require(fill.Status == MagicCastingStatus.Succeeded && container.LiquidVolume == 4500 && container.LiquidMixture.Instances.Any(x => x.Liquid.Id == water.Id) && container.LiquidMixture.Instances.Any(x => x.Liquid.Id == wine.Id), "Wine fill replaced existing water.");
		fill = Cast(waterSpell, 7, "vessel"); Require(fill.Status == MagicCastingStatus.Succeeded && container.LiquidVolume == 5000, "Liquid overfill was not clamped to real native capacity.");
		var beforeFull = actor.MagicResourceAmounts[native.Resource]; var refused = casting.Cast(new(actor, cap.Id, waterSpell.Id, 3, false, "vessel"));
		Require(refused.Status == MagicCastingStatus.Refused && refused.OperationId is null && actor.MagicResourceAmounts[native.Resource] == beforeFull && container.LiquidVolume == 5000, "Full container refusal spent resources or changed liquid.");
		var transfer = container.RemoveLiquidAmount(250, actor, "fixture transfer"); Require(transfer.TotalVolume == 250 && container.LiquidVolume == 4750, "Native transfer did not conserve the created mixed liquid.");
		var transferVessel = New("vessel"); transferVessel.GetItemType<ILiquidContainer>()!.Open(); transferVessel.GetItemType<ILiquidContainer>()!.MergeLiquid(transfer, actor, "fixture transfer");
		Require(transferVessel.GetItemType<ILiquidContainer>()!.LiquidVolume == 250 && container.LiquidVolume + transferVessel.GetItemType<ILiquidContainer>()!.LiquidVolume == 5000, "Transferred mixture was not conserved in its foreign destination.");
		var other = New("vessel"); var incompatible = other.GetItemType<ILiquidContainer>()!; incompatible.Open(); incompatible.MergeLiquid(new LiquidMixture(oil, 100, world), actor, "fixture");
		// Resolve this exact foreign item without a display-name ambiguity.
		var copy = (MagicSpell)typeof(MagicSpell).GetMethod("CastingCopy", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(waterSpell, [actor, skill, 3, SpellPower.Weak, MudSharp.RPG.Checks.Difficulty.Easy, 7])!;
		var creation = (CreateLiquidEffect)copy.SpellEffects.Single();
		Require(!creation.TryPrepareApplication(actor, other, default, SpellPower.Weak, TimeSpan.Zero, out _, out var reason) && reason!.Contains("incompatible") && incompatible.LiquidVolume == 100 && incompatible.LiquidMixture.Instances.Single().Liquid.Id == oil.Id, "Configured incompatible-liquid refusal converted foreign contents.");
		var emptyWine = New("vessel"); emptyWine.GetItemType<ILiquidContainer>()!.Open();
		var wineCopy = (MagicSpell)typeof(MagicSpell).GetMethod("CastingCopy", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(wineSpell, [actor, skill, 7, SpellPower.Weak, MudSharp.RPG.Checks.Difficulty.Easy, 7])!;
		Require(((CreateLiquidEffect)wineCopy.SpellEffects.Single()).TryPrepareApplication(actor, emptyWine, default, SpellPower.Weak, TimeSpan.Zero, out var wineApplication, out _), "Empty wine container admission failed.");
		wineApplication!.Create(null!); Require(emptyWine.GetItemType<ILiquidContainer>()!.LiquidVolume == 1750, "Empty container wine path did not use selected grade.");
		Require(waterSpell.BuildingCommand(actor, new StringStack("effect 1 bonusplane none")), "Ordinary water plane fixture edit failed.");
		var ordinaryWater = New("vessel"); ordinaryWater.GetItemType<ILiquidContainer>()!.Open();
		var ordinaryCopy = (MagicSpell)typeof(MagicSpell).GetMethod("CastingCopy", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(waterSpell, [actor, skill, 3, SpellPower.Weak, MudSharp.RPG.Checks.Difficulty.Easy, 7])!;
		var ordinaryCreation = (CreateLiquidEffect)ordinaryCopy.SpellEffects.Single();
		Require(ordinaryCreation.TryPrepareApplication(actor, ordinaryWater, default, SpellPower.Weak, TimeSpan.Zero, out var waterApplication, out _), "Ordinary plane water admission failed.");
		waterApplication!.Create(null!); Require(ordinaryWater.GetItemType<ILiquidContainer>()!.LiquidVolume == 1500, "Ordinary plane water retained the configured plane bonus.");
		ordinaryWater.GetItemType<ILiquidContainer>()!.Close();
		Require(!ordinaryCreation.TryPrepareApplication(actor, ordinaryWater, default, SpellPower.Weak, TimeSpan.Zero, out _, out _) && ordinaryWater.GetItemType<ILiquidContainer>()!.LiquidVolume == 1500, "Closed drink container was filled.");
		Console.WriteLine("ARM03C2-liquid-empty-ordinary=passed native-empty-wine-container selected-grade-7-1750 native-ordinary-plane-grade-3-1500 versus-configured-bonus-3000 closed-container-refusal no-mutation");
		Console.WriteLine("ARM03C2-liquid-conservation=passed actual-paid-native-item-casts empty-water configured-water-plane-double existing-water-plus-wine capacity-clamp full-prepayment-refusal real-SilentDrink reduced-volume-and-needs configured-oil-refusal preserved-foreign-container-and-mixture no-liquid-lifecycle-or-expiry");
		world.SaveManager.Flush();
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			foreach (var item in host.Items.Where(x => x.InInventoryOf is null && x.ContainedIn is null))
				if (!db.RoomsGameItems.Any(x => x.GameItemId == item.Id)) db.RoomsGameItems.Add(new() { RoomId = fixture.RoomId, GameItemId = item.Id });
			db.SaveChanges();
		}
		var state = new ConsumableReader(database.Name, fixture, RuntimeClock.UtcNow, partial.Id, exhausted.Id, permanent.Id, light.Id, vessel.Id, transferVessel.Id, gear.Id, origins.ToArray(),
			actor.NeedsModel.FoodSatiatedHours, actor.NeedsModel.WaterLitres, container.LiquidVolume, "reload");
		RunItemReaderProcess(state, "--created-consumables-reader");
		RunItemReaderProcess(state with { Now = RuntimeClock.UtcNow.AddSeconds(12601), Action = "expire" }, "--created-consumables-reader");
		Console.WriteLine("ARM03C2-acceptance=passed food-liquid-worn-light-native-boundaries separate-process-reload-and-expiry controlled-unscripted-replacement-content installed-stock-and-full-N14-N16-not-qualified");
		return 0;
	}

	private static int RunCreatedConsumablesReader(string[] args)
	{
		Require(args.Length == 1, "Consumable reader needs one receipt.");
		var input = JsonSerializer.Deserialize<ConsumableReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args[0])))!;
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, consumablesAnatomy: true); var native = host.Native; var world = native.World;
		var owned = new SpellOwnedItemService(world); native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(owned);
		using (var db = NewIndependentContext(database.ConnectionString)) native.Body.LoadInventory(db.Bodies.Include(x => x.BodiesGameItems).Single(x => x.Id == native.Body.Id));
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			foreach (var id in db.RoomsGameItems.Where(x => x.RoomId == input.Fixture.RoomId).Select(x => x.GameItemId).ToArray())
			{
				var item = world.TryGetItem(id, true)!;
				Require(item.InInventoryOf is null && item.ContainedIn is null, "Persisted room membership conflicts with another native custodian.");
				native.Actor.Location.Insert(item, true);
			}
		}
		if (input.Action == "fault")
		{
			var retainedMeal = world.TryGetItem(input.Partial, true)!; var retainedLight = world.TryGetItem(input.Light, true)!;
			Require(retainedMeal.GetItemType<IEdible>()!.BitesRemaining == 2.75 && native.Body.WornItems.Contains(retainedLight) &&
				retainedLight.GetItemType<IProduceLight>()!.CurrentIllumination == 40 && native.Body.WornItems.Any(x => x.Id == input.ForeignGear) &&
				native.Actor.NeedsModel.FoodSatiatedHours == input.FoodHours && native.Actor.NeedsModel.WaterLitres == input.Water &&
				host.Store.Find(retainedMeal.SpellCreationOrigin!.LifecycleId)!.State == SpellLifecycleState.Retiring &&
				host.Store.Find(retainedLight.SpellCreationOrigin!.LifecycleId)!.State == SpellLifecycleState.Retiring,
				"Fresh fault reader lost fractional food, lit worn custody, foreign gear, needs or exact retirement intent.");
			Console.WriteLine("ARM03C2-reader-fault=passed new-native-process after-real-provider-DELETE-fault-and-normal-save fractional-food-unrestored worn-lit-light-and-foreign-gear-conserved exact-retiring-claims player-needs-unchanged");
			return 0;
		}
		var partial = world.TryGetItem(input.Partial, true)!; var exhausted = world.TryGetItem(input.Exhausted, true)!;
		var light = world.TryGetItem(input.Light, true)!; var vessel = world.TryGetItem(input.Vessel, true)!; var gear = world.TryGetItem(input.ForeignGear, true)!;
		var transferVessel = world.TryGetItem(input.TransferVessel, true)!;
		Require(partial.GetItemType<IEdible>()!.BitesRemaining == 2.75 && exhausted.GetItemType<IEdible>()!.BitesRemaining == 0 &&
			!native.Body.SilentEat(exhausted.GetItemType<IEdible>()!, 0) && native.Actor.NeedsModel.FoodSatiatedHours == input.FoodHours && native.Actor.NeedsModel.WaterLitres == input.Water,
			"Fresh process restored consumed portions or lost/doubled native needs.");
		Require(native.Body.WornItems.Contains(light) && native.Body.WornItems.Contains(gear) && native.Actor.IlluminationProvided == 40 && vessel.GetItemType<ILiquidContainer>()!.LiquidVolume == input.Volume && transferVessel.GetItemType<ILiquidContainer>()!.LiquidVolume == 250,
			"Fresh native component/custody reload lost worn illumination, foreign gear or conserved liquid.");
		using (var db = NewIndependentContext(database.ConnectionString)) Require(!db.GameItems.Any(x => x.Id == input.Permanent), "Consumed permanent food was recreated.");
		if (input.Action == "reload") Console.WriteLine("ARM03C2-reader-reload=passed new-native-process real-food-and-liquid-and-wearable-XML fractional-2.75-and-exhausted-zero-bites persisted-needs no-duplicate-nutrition consumed-permanent-absent actual-worn-illumination foreign-gear-and-4750-volume-conserved");
		else
		{
			foreach (var id in input.Origins)
			{
				var life = host.Store.Find(id)!; foreach (var entity in life.Entities) if (entity.Kind == SpellOwnedEntityKind.GameItem && world.TryGetItem(entity.Id, true) is { } item) item.FinaliseLoadTimeTasks();
			}
			using (var triggerDb = NewIndependentContext(database.ConnectionString))
				triggerDb.Database.ExecuteSqlRaw($"CREATE TRIGGER arm03c2_delete_fault BEFORE DELETE ON GameItems FOR EACH ROW BEGIN IF OLD.Id={input.Light} OR OLD.Id={input.Partial} THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='ARM03C2 food/light removal fault'; END IF; END");
			try
			{
				owned.ReconcileRetirements(RuntimeClock.UtcNow);
				Require(!partial.Deleted && partial.GetItemType<IEdible>()!.BitesRemaining == 2.75 && !light.Deleted && native.Body.WornItems.Contains(light) && native.Actor.IlluminationProvided == 40,
					"Provider failure did not restore exact fractional-food and lit worn-light custody.");
				world.SaveManager.Flush();
				RunItemReaderProcess(input with { Action = "fault" }, "--created-consumables-reader");
			}
			finally { using var faultDb = NewIndependentContext(database.ConnectionString); faultDb.Database.ExecuteSqlRaw("DROP TRIGGER arm03c2_delete_fault"); }
			Console.WriteLine("ARM03C2-removal-rollback=passed actual-MySQL-final-DELETE-fault real-food-and-worn-light native-compensation exact-2.75-bites-lit-state-and-body-membership ordinary-save fresh-reader foreign-gear-needs-conserved retry-with-same-origins");
			owned.ReconcileRetirements(RuntimeClock.UtcNow); world.SaveManager.Flush();
			using var db = NewIndependentContext(database.ConnectionString);
			var retired = input.Origins.Select(x => host.Store.Find(x)!).ToArray();
			var outputIds = retired.SelectMany(x => x.Entities).Select(x => x.Id).ToArray();
			Console.WriteLine("ARM03C2-expiry-state=" + JsonSerializer.Serialize(new
			{
				remaining = retired.Where(x => x.State != SpellLifecycleState.Completed).Select(x => new { x.Origin.Id, x.State, x.Diagnostic }),
				remainingItems = db.GameItems.Where(x => outputIds.Contains(x.Id)).Select(x => x.Id).ToArray(),
				worn = native.Body.WornItems.Select(x => x.Id).ToArray(), illumination = native.Actor.IlluminationProvided,
				foodHours = native.Actor.NeedsModel.FoodSatiatedHours, water = native.Actor.NeedsModel.WaterLitres
			}));
			Require(input.Origins.All(x => host.Store.Find(x)!.State == SpellLifecycleState.Completed) && !db.GameItems.Any(x => outputIds.Contains(x.Id)) &&
				!db.GameItemComponents.Any(x => outputIds.Contains(x.GameItemId)) &&
				db.GameItems.Any(x => x.Id == input.Vessel) && db.GameItems.Any(x => x.Id == input.TransferVessel) && db.GameItems.Any(x => x.Id == input.ForeignGear) && native.Body.WornItems.Contains(gear) && native.Actor.IlluminationProvided == 0 && vessel.GetItemType<ILiquidContainer>()!.LiquidVolume == input.Volume && transferVessel.GetItemType<ILiquidContainer>()!.LiquidVolume == 250,
				"Native post-restart expiry did not retire only the exact food/light outputs or preserve foreign state.");
			Require(native.Actor.NeedsModel.FoodSatiatedHours == input.FoodHours && native.Actor.NeedsModel.WaterLitres == input.Water,
				"Native expiry changed already fulfilled player needs.");
			Console.WriteLine("ARM03C2-reader-expiry=passed new-native-process exact-28-food-and-one-worn-light-origins terminal-claims actual-native-Delete-and-worn-detachment no-created-item-component-rows foreign-gear-container-liquid-and-player-needs-conserved no-consumption-replay");
		}
		return 0;
	}
}
