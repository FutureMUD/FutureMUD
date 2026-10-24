#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health.Breathing;
using MudSharp.Planes;
using MudSharp.PerceptionEngine;
using ConcreteBody = MudSharp.Body.Implementations.Body;
using ConcreteRace = MudSharp.Character.Heritage.Race;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BreathingRateTests
{
	[TestMethod]
	[DataRow(1.0, 1.0, 10.0, 7.0 / 6)]
	[DataRow(1.0, 1.0, 60.0, 7.0)]
	[DataRow(0.001, 1.0, 10.0, 7000.0 / 6)]
	[DataRow(0.5, 2.5, 60.0, 35.0)]
	[DataRow(2.0, 0.5, 3.0, 0.0875)]
	public void LungBreather_ConfiguredLitresPerMinute_WithdrawsElapsedBaseVolume(
		double litresPerBaseUnit, double multiplier, double seconds, double expected)
	{
		var f = new SupplyFixture(litresPerBaseUnit, multiplier);
		f.Strategy.Breathe(f.Body.Object, TimeSpan.FromSeconds(seconds));
		Assert.AreEqual(expected, f.InitialVolume - f.Tank.GasVolumeAtOneAtmosphere, 1e-8);
		Assert.AreEqual(7 * multiplier, f.Race.BreathingRate(f.Body.Object, f.Gas.Object), 1e-8,
			"The configured expression and its L/min contract must remain unchanged.");
	}

	[TestMethod]
	public void LungBreather_SixTenSecondIntervals_ConsumeSevenLitres()
	{
		var f = new SupplyFixture();
		for (var i = 0; i < 6; i++) f.Strategy.Breathe(f.Body.Object);
		Assert.AreEqual(7, f.InitialVolume - f.Tank.GasVolumeAtOneAtmosphere, 1e-8);
	}

	[TestMethod]
	public void LungBreather_ExactLastBreath_SucceedsBeforeEmptyTankExposesUnbreathableAmbient()
	{
		var f = new SupplyFixture();
		f.Tank.GasVolumeAtOneAtmosphere = 7;
		f.Body.Object.HeldBreathTime = TimeSpan.FromSeconds(120);
		f.Strategy.Breathe(f.Body.Object, TimeSpan.FromMinutes(1));
		Assert.AreEqual(0, f.Tank.GasVolumeAtOneAtmosphere, 1e-8);
		Assert.IsNull(f.Device.Gas);
		Assert.AreEqual(TimeSpan.FromSeconds(60), f.Body.Object.HeldBreathTime);
		Assert.IsTrue(EnvironmentalExposureService.For(f.World.Object).LastBreathWasSupplied(f.Body.Object));
		f.Strategy.Breathe(f.Body.Object);
		Assert.AreEqual(TimeSpan.FromSeconds(70), f.Body.Object.HeldBreathTime);
		Assert.AreEqual(0, f.Tank.GasVolumeAtOneAtmosphere, 1e-8);
	}

	[TestMethod]
	public void LungBreather_PartialSupply_RejectsWholeIntervalWithoutOverdrawing()
	{
		var f = new SupplyFixture();
		f.Tank.GasVolumeAtOneAtmosphere = 1;
		f.Strategy.Breathe(f.Body.Object);
		Assert.AreEqual(1, f.Tank.GasVolumeAtOneAtmosphere, 1e-8);
		Assert.AreEqual(TimeSpan.FromSeconds(10), f.Body.Object.HeldBreathTime);
		Assert.IsFalse(EnvironmentalExposureService.For(f.World.Object).LastBreathWasSupplied(f.Body.Object));
		// A shorter interval fits the same remaining supply.
		f.Strategy.Breathe(f.Body.Object, TimeSpan.FromSeconds(3));
		Assert.AreEqual(0.65, f.Tank.GasVolumeAtOneAtmosphere, 1e-8);
		Assert.AreEqual(TimeSpan.FromSeconds(7), f.Body.Object.HeldBreathTime);
	}

	[TestMethod]
	public void HasWorkingSupply_UsesRequestedIntervalAndBaseUnitsWithoutDebiting()
	{
		var f = new SupplyFixture(0.001, 2);
		f.Tank.GasVolumeAtOneAtmosphere = 1200;
		Assert.IsTrue(BreathingStrategyHelper.HasWorkingSupply(f.Body.Object, TimeSpan.FromSeconds(5)));
		Assert.IsFalse(BreathingStrategyHelper.HasWorkingSupply(f.Body.Object, TimeSpan.FromSeconds(10)));
		Assert.AreEqual(1200, f.Tank.GasVolumeAtOneAtmosphere);
	}

	[TestMethod]
	[DataRow(1.0, 1.0)]
	[DataRow(0.001, 2.0)]
	public void LungBreather_Filter_DegradesByElapsedBaseVolumeAndPassesAmbientAfterExhaustion(
		double litresPerBaseUnit, double multiplier)
	{
		var f = new SupplyFixture(litresPerBaseUnit, multiplier);
		var proto = TestObjectFactory.CreateUninitialized<BreathingFilterGameItemComponentProto>();
		SetProperty(proto, nameof(proto.VolumePerFilter), 7 * multiplier / litresPerBaseUnit);
		SetField(proto, "<FilteredGases>k__BackingField", new Dictionary<IGas, IGas> { [f.Ambient.Object] = f.Gas.Object });
		var cartridge = new Mock<IGameItem>();
		cartridge.SetupProperty(x => x.Condition, 1.0);
		var parent = f.NewItem();
		parent.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object);
		var filter = new BreathingFilterGameItemComponent(proto, parent.Object, true);
		filter.Put(null, cartridge.Object);
		f.Worn.Setup(x => x.GetItemType<IProvideGasForBreathing>()).Returns(filter);
		f.Strategy.Breathe(f.Body.Object);
		Assert.AreEqual(5.0 / 6, cartridge.Object.Condition, 1e-8);
		for (var i = 0; i < 5; i++) f.Strategy.Breathe(f.Body.Object);
		Assert.AreEqual(0, cartridge.Object.Condition, 1e-8);
		// Set exactly zero to avoid a floating-point remainder deciding the functional state.
		cartridge.Object.Condition = 0;
		Assert.AreSame(f.Ambient.Object, filter.Gas);
		f.Strategy.Breathe(f.Body.Object);
		Assert.AreEqual(0, cartridge.Object.Condition, 1e-8);
		Assert.AreEqual(TimeSpan.FromSeconds(10), f.Body.Object.HeldBreathTime);
		cartridge.Object.Condition = 1;
		f.Body.Object.HeldBreathTime = TimeSpan.FromSeconds(120);
		f.Strategy.Breathe(f.Body.Object, TimeSpan.FromMinutes(1));
		Assert.AreEqual(0, cartridge.Object.Condition);
		Assert.AreEqual(TimeSpan.FromSeconds(60), f.Body.Object.HeldBreathTime,
			"The last filtered breath uses the delivered gas before the cartridge is exhausted.");
	}

	[TestMethod]
	[DataRow("simple")]
	[DataRow("gills")]
	[DataRow("blowhole")]
	[DataRow("partless")]
	[DataRow("non-breather")]
	public void BreathingStrategies_ZeroElapsed_DoNotConsumeOrChangeHeldBreath(string name)
	{
		var f = new SupplyFixture();
		f.Body.Object.HeldBreathTime = TimeSpan.FromSeconds(4);
		Strategy(name).Breathe(f.Body.Object, TimeSpan.Zero);
		Assert.AreEqual(f.InitialVolume, f.Tank.GasVolumeAtOneAtmosphere);
		Assert.AreEqual(TimeSpan.FromSeconds(4), f.Body.Object.HeldBreathTime);
	}

	[TestMethod]
	[DataRow("simple")]
	[DataRow("gills")]
	[DataRow("blowhole")]
	[DataRow("partless")]
	public void BreathingStrategies_UnbreathableInterval_AccumulatesActualHeldBreath(string name)
	{
		var f = new SupplyFixture();
		f.Worn.Setup(x => x.GetItemType<IProvideGasForBreathing>()).Returns((IProvideGasForBreathing)null!);
		Strategy(name).Breathe(f.Body.Object, TimeSpan.FromSeconds(2.5));
		Assert.AreEqual(TimeSpan.FromSeconds(2.5), f.Body.Object.HeldBreathTime);
	}

	[TestMethod]
	[DataRow("simple")]
	[DataRow("gills")]
	[DataRow("blowhole")]
	[DataRow("partless")]
	public void BreathingStrategies_BreathableInterval_RecoversActualHeldBreath(string name)
	{
		var f = new SupplyFixture();
		f.Worn.Setup(x => x.GetItemType<IProvideGasForBreathing>()).Returns((IProvideGasForBreathing)null!);
		Mock.Get(f.Body.Object.Location).SetupGet(x => x.Atmosphere).Returns(f.Gas.Object);
		f.Body.Object.HeldBreathTime = TimeSpan.FromSeconds(4);
		Strategy(name).Breathe(f.Body.Object, TimeSpan.FromSeconds(2.5));
		Assert.AreEqual(TimeSpan.FromSeconds(1.5), f.Body.Object.HeldBreathTime);
	}

	[TestMethod]
	public void StaminaHeartbeat_StartRestartAndGlobalPhase_ChargeOnlyElapsedActiveTime()
	{
		var clock = new ManualClock();
		using var time = RuntimeClock.Push(clock);
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var heartbeat = new HeartbeatManager(world.Object);
		world.SetupGet(x => x.HeartbeatManager).Returns(heartbeat);
		var room = new Mock<IRoom>();
		var body = TestObjectFactory.CreateUninitialized<ConcreteBody>();
		SetField(body, "_merits", new List<MudSharp.RPG.Merits.IMerit>());
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.CurrentBody).Returns(body);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		actor.SetupGet(x => x.Location).Returns(room.Object);
		body.Actor = actor.Object;
		SetProperty(body, nameof(body.Gameworld), world.Object);
		SetProperty(body, nameof(body.Prototype), Mock.Of<IBodyPrototype>(x => x.BasePlanarPresence == PlanarPresenceDefinition.DefaultMaterial(1)));
		SetProperty(body, nameof(body.EffectHandler), new EffectHandler(body));
		SetField(body, "_noSave", true);
		body.MaximumStamina = 100;
		body.CurrentStamina = 100;
		var strategy = new Mock<IBreathingStrategy>();
		strategy.SetupGet(x => x.NeedsToBreathe).Returns(true);
		strategy.Setup(x => x.CanBreathe(body)).Returns(true);
		var intervals = new List<TimeSpan>();
		strategy.Setup(x => x.Breathe(body, It.IsAny<TimeSpan>())).Callback<IBody, TimeSpan>((_, elapsed) => intervals.Add(elapsed));
		SetField(body, "_breathingStrategy", strategy.Object);

		body.StartStaminaTick();
		body.StartStaminaTick();
		Assert.AreEqual(0, intervals.Count, "Immediate/repeated startup must not charge a breath.");
		clock.Advance(2);
		heartbeat.ManuallyFireHeartbeat10Second();
		Assert.AreEqual(TimeSpan.FromSeconds(2), intervals.Single(), "The global heartbeat can arrive before ten active seconds.");
		heartbeat.ManuallyFireHeartbeat10Second();
		Assert.AreEqual(1, intervals.Count, "Duplicate callbacks at the same time cannot charge twice.");
		clock.Advance(3);
		body.EndStaminaTick(true);
		clock.Advance(60);
		body.StartStaminaTick();
		Assert.AreEqual(TimeSpan.FromSeconds(3), intervals.Last(), "Restart settles only the retained active fraction.");
		clock.Advance(10);
		heartbeat.ManuallyFireHeartbeat10Second();
		Assert.AreEqual(TimeSpan.FromSeconds(10), intervals.Last());
		clock.Advance(45);
		heartbeat.ManuallyFireHeartbeat10Second();
		Assert.AreEqual(TimeSpan.FromSeconds(45), intervals.Last(), "A delayed callback uses elapsed time, without inventing extra ticks.");
		Assert.AreEqual(7, intervals.Sum(x => 7 * x.TotalMinutes), 1e-8);
		body.EndStaminaTick(true);
	}

	private static IBreathingStrategy Strategy(string name) => name switch
	{
		"simple" => new LungBreather(), "gills" => new GillBreather(), "blowhole" => new BlowholeBreather(),
		"partless" => new PartlessBreather(), _ => new NonBreather()
	};

	private sealed class ManualClock : TimeProvider
	{
		private long _ticks;
		public override long TimestampFrequency => TimeSpan.TicksPerSecond;
		public override long GetTimestamp() => _ticks;
		public void Advance(double seconds) => _ticks += TimeSpan.FromSeconds(seconds).Ticks;
	}

	private sealed class SupplyFixture
	{
		public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IBody> Body { get; } = new();
		public Mock<IGas> Gas { get; } = new();
		public Mock<IGas> Ambient { get; } = new();
		public Mock<IGameItem> Worn { get; }
		public ConcreteRace Race { get; } = TestObjectFactory.CreateUninitialized<ConcreteRace>();
		public LungBreather Strategy { get; } = new();
		public GasContainerGameItemComponent Tank { get; }
		public RebreatherGameItemComponent Device { get; }
		public double InitialVolume => 10000;
		public SupplyFixture(double litresPerBaseUnit = 1, double multiplier = 1)
		{
			SetField(Race, "_intrinsicMeritIds", new List<long>());
			World.Setup(x => x.UnitManager.BaseFluidToLitres).Returns(litresPerBaseUnit);
			World.SetupGet(x => x.MagicalSubstances).Returns(new All<MudSharp.Magic.IMagicalSubstance>());
			Gas.Setup(x => x.CountsAs(Gas.Object)).Returns(true);
			Gas.Setup(x => x.CountAsQuality(Gas.Object)).Returns(ItemQuality.Standard);
			Gas.Setup(x => x.CountsAsMultiplier(Gas.Object)).Returns(1);
			SetField(Race, "_removeBreathableFluids", new List<IFluid>());
			SetField(Race, "_fluidBreathingMultipliers", new Dictionary<IFluid, double> { [Gas.Object] = multiplier });
			var expression = new Mock<ITraitExpression>();
			expression.Setup(x => x.EvaluateWith(Body.Object, null, TraitBonusContext.BreathingRate, It.IsAny<(string, object)[]>())).Returns(7);
			Race.BreathingVolumeExpression = expression.Object;
			Body.SetupGet(x => x.Race).Returns(Race);
			Body.SetupGet(x => x.Gameworld).Returns(World.Object);
			Body.SetupGet(x => x.BreathingStrategy).Returns(Strategy);
			Body.SetupProperty(x => x.HeldBreathTime, TimeSpan.Zero);
			Body.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			Body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			Body.Setup(x => x.OrganFunction<HeartProto>()).Returns(1);
			Body.Setup(x => x.OrganFunction<LungProto>()).Returns(1);
			Body.Setup(x => x.OrganFunction<TracheaProto>()).Returns(1);
			var mouth = TestObjectFactory.CreateUninitialized<MouthProto>();
			Body.SetupGet(x => x.Bodyparts).Returns(new IExternalBodypart[]
			{
				mouth, TestObjectFactory.CreateUninitialized<GillProto>(), TestObjectFactory.CreateUninitialized<BlowholeProto>()
			});
			var actor = new Mock<ICharacter>();
			actor.SetupGet(x => x.Body).Returns(Body.Object);
			actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Body.SetupGet(x => x.Actor).Returns(actor.Object);
			var room = new Mock<IRoom>();
			room.SetupGet(x => x.Atmosphere).Returns(Ambient.Object);
			Body.SetupGet(x => x.Location).Returns(room.Object);
			Tank = new GasContainerGameItemComponent(TestObjectFactory.CreateUninitialized<GasContainerGameItemComponentProto>(), NewItem().Object, true) { GasVolumeAtOneAtmosphere = InitialVolume, Gas = Gas.Object };
			Device = new RebreatherGameItemComponent(TestObjectFactory.CreateUninitialized<RebreatherGameItemComponentProto>(), NewItem().Object, true);
			SetProperty(Device, nameof(Device.ConnectedGasSupply), Tank);
			Worn = NewItem();
			Worn.Setup(x => x.GetItemType<IProvideGasForBreathing>()).Returns(Device);
			Body.Setup(x => x.WornItemsFor(mouth)).Returns(new[] { Worn.Object });
		}
		public Mock<IGameItem> NewItem()
		{
			var item = new Mock<IGameItem>();
			item.SetupGet(x => x.Gameworld).Returns(World.Object);
			return item;
		}
	}

	private static void SetProperty(object target, string name, object value) =>
		target.GetType().GetProperty(name)!.SetValue(target, value);

	private static void SetField(object target, string name, object value)
	{
		for (var type = target.GetType(); type is not null; type = type.BaseType)
		{
			var field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			if (field is null) continue;
			field.SetValue(target, value);
			return;
		}
		Assert.Fail($"Missing fixture field {name}.");
	}
}
