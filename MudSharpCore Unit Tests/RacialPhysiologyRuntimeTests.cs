#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class RacialPhysiologyRuntimeTests
{
	[TestMethod]
	[DataRow(5.0, 3.75, 3.75)]
	[DataRow(5.0, 7.0, 5.0)]
	[DataRow(0.0, 3.75, 0.0)]
	public void LoadedBloodCapacity_PreservesPartialVolumeAndClampsExcessIncludingZero(double capacity, double saved, double expected)
	{
		var body = TestObjectFactory.CreateUninitialized<ConcreteBody>();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		typeof(ConcreteBody).GetProperty(nameof(body.Gameworld))!.SetValue(body, world.Object);
		typeof(ConcreteBody).GetField("_currentBloodVolumeLitres", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, saved);
		body.TotalBloodVolumeLitres = capacity;
		body.ClampLoadedBloodVolumeToCapacity();
		Assert.AreEqual(expected, body.CurrentBloodVolumeLitres, 1e-9);
	}

	[TestMethod]
	public void IntrinsicRaceGrants_InheritByIdentityAndRetainParentGrantAfterDirectRemoval()
	{
		var world = new Mock<IFuturemud>();
		var merits = new All<IMerit>();
		var bodyMerit = new Mock<ICharacterMerit>();
		bodyMerit.SetupGet(x => x.Id).Returns(601);
		bodyMerit.SetupGet(x => x.MeritScope).Returns(MeritScope.Body);
		var characterMerit = new Mock<ICharacterMerit>();
		characterMerit.SetupGet(x => x.Id).Returns(602);
		characterMerit.SetupGet(x => x.MeritScope).Returns(MeritScope.Character);
		merits.Add(bodyMerit.Object); merits.Add(characterMerit.Object);
		world.SetupGet(x => x.Merits).Returns(merits);
		MudSharp.Character.Heritage.Race Race(params long[] ids)
		{
			var race = TestObjectFactory.CreateUninitialized<MudSharp.Character.Heritage.Race>();
			typeof(MudSharp.Character.Heritage.Race).GetProperty(nameof(race.Gameworld))!.SetValue(race, world.Object);
			typeof(MudSharp.Character.Heritage.Race).GetField("_intrinsicMeritIds", BindingFlags.Instance | BindingFlags.NonPublic)!
				.SetValue(race, ids.ToList());
			return race;
		}
		var parent = Race(601);
		var child = Race(601, 602);
		typeof(MudSharp.Character.Heritage.Race).GetProperty(nameof(child.ParentRace))!.SetValue(child, parent);
		Assert.AreEqual(1, child.DirectIntrinsicMerits.Count(), "Only body scope can contribute an intrinsic racial grant.");
		Assert.AreEqual(1, child.IntrinsicMerits.Count(), "The same parent and direct grant must apply once.");
		((List<long>)typeof(MudSharp.Character.Heritage.Race).GetField("_intrinsicMeritIds", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(child)!).Clear();
		Assert.AreEqual(0, child.DirectIntrinsicMerits.Count());
		Assert.AreSame(bodyMerit.Object, child.IntrinsicMerits.Single());
	}

	[TestMethod]
	public void GiantMetabolism_ConsumesThreeTimesCountersAndConfiguredWaterWithoutDoubleCounting()
	{
		var giant = NeedsCharacter(48, 24);
		var metabolism = new Mock<INeedRateChangingMerit>();
		metabolism.SetupGet(x => x.Id).Returns(123);
		metabolism.Setup(x => x.Applies(giant.Object)).Returns(true);
		metabolism.SetupGet(x => x.HungerMultiplier).Returns(3);
		metabolism.SetupGet(x => x.ThirstMultiplier).Returns(3);
		metabolism.SetupGet(x => x.DrunkennessMultiplier).Returns(1);
		giant.SetupGet(x => x.Merits).Returns(new IMerit[] { metabolism.Object, metabolism.Object });
		var model = new ActiveNeedsModel(giant.Object);
		var old = ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds;
		try
		{
			ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = 60;
			model.NeedsHeartbeat();
			Assert.AreEqual(45, model.FoodSatiatedHours, 1e-9);
			Assert.AreEqual(21, model.DrinkSatiatedHours, 1e-9);
			Assert.AreEqual(2.4, model.WaterLitres, 1e-9);
			Assert.AreEqual(7.0 / 8, model.DrinkSatiatedHours / 24, 1e-9, "Baseline satiation duration scales with both storage and consumption.");
		}
		finally { ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = old; }
	}

	[TestMethod]
	public void EnvironmentalNeeds_ShelterCancelsLossWithoutCreatingWaterAndExplicitSweatIsUnchanged()
	{
		var actor = NeedsCharacter(16, 8);
		var environment = new Mock<IEnvironmentalNeedsMerit>();
		environment.SetupGet(x => x.Id).Returns(456);
		environment.Setup(x => x.Applies(actor.Object)).Returns(true);
		environment.Setup(x => x.GetNeedsAdjustment(actor.Object)).Returns(new EnvironmentalNeedsAdjustment(0, -2, -0.4, MaximumThirstRate: 5));
		actor.SetupGet(x => x.Merits).Returns(new IMerit[] { environment.Object });
		var model = new ActiveNeedsModel(actor.Object);
		var old = ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds;
		try
		{
			ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = 60;
			model.NeedsHeartbeat();
			Assert.AreEqual(8, model.DrinkSatiatedHours, 1e-9);
			Assert.AreEqual(3, model.WaterLitres, 1e-9);
			model.FulfilNeeds(new NeedFulfiller { WaterLitres = -0.2, ThirstPoints = -1 });
			Assert.AreEqual(7, model.DrinkSatiatedHours, 1e-9);
			Assert.AreEqual(2.8, model.WaterLitres, 1e-9, "Environmental adaptation must not discount explicit sweat/other physical losses.");
			model.NeedsHeartbeat();
			Assert.AreEqual(7, model.DrinkSatiatedHours, 1e-9);
			Assert.AreEqual(2.8, model.WaterLitres, 1e-9, "Shelter prevents loss rather than refilling water.");
			environment.Verify(x => x.GetNeedsAdjustment(actor.Object), Times.Exactly(2));
		}
		finally { ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = old; }
	}

	[TestMethod]
	public void EnvironmentalRates_ApplyCapsToCombinedLossAndPreserveUnconfiguredBaseline()
	{
		Assert.AreEqual((1.0, 1.0, 0.2), EnvironmentalNeedsRates.Calculate(1, 1, 0.2, []));
		var result = EnvironmentalNeedsRates.Calculate(1, 1, 0.2,
			[new EnvironmentalNeedsAdjustment(ThirstRateDelta: 9, WaterLossLitresPerHour: 0.1, MaximumThirstRate: 5)]);
		Assert.AreEqual(5.0, result.Thirst);
		Assert.AreEqual(0.3, result.Water, 1e-9);
	}

	[TestMethod]
	public void InactiveBodyCapacity_UsesSelectedRaceAndRestoresActiveBodyEvenOnFailure()
	{
		var actor = TestObjectFactory.CreateUninitialized<MudSharp.Character.Character>();
		typeof(MudSharp.Character.Character).GetField("_merits", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(actor, new List<IMerit>());
		var human = new Mock<IBody>();
		var giant = new Mock<IBody>();
		var giantRace = new Mock<IRace>();
		giant.SetupGet(x => x.Race).Returns(giantRace.Object);
		var bodyProperty = typeof(MudSharp.Character.Character).GetProperty(nameof(actor.Body))!;
		bodyProperty.SetValue(actor, human.Object);
		var reserve = new Mock<IStaminaCapacityMerit>();
		reserve.SetupGet(x => x.Id).Returns(995);
		reserve.SetupGet(x => x.StaminaBonus).Returns(20);
		reserve.SetupGet(x => x.StaminaMultiplier).Returns(1);
		reserve.Setup(x => x.Applies(actor)).Returns(() => actor.Race == giantRace.Object);
		giant.SetupGet(x => x.Merits).Returns(new IMerit[] { reserve.Object });
		Assert.AreEqual(120, PhysiologyMeritEffects.StaminaCapacity(100, actor, giant.Object), 1e-9);
		Assert.AreSame(human.Object, actor.Body);
		reserve.Setup(x => x.Applies(actor)).Throws(new InvalidOperationException("fixture failure"));
		Assert.ThrowsExactly<InvalidOperationException>(() => PhysiologyMeritEffects.StaminaCapacity(100, actor, giant.Object));
		Assert.AreSame(human.Object, actor.Body);
	}

	[TestMethod]
	public void IntrinsicBodyMerits_AreEffectiveOnceNotExplicitAndCapacityChangesNeverRefill()
	{
		var body = TestObjectFactory.CreateUninitialized<ConcreteBody>();
		var actor = new Mock<ICharacter>();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		body.Actor = actor.Object;
		typeof(ConcreteBody).GetProperty(nameof(body.Gameworld))!.SetValue(body, world.Object);
		foreach (var name in new[] { "_traits", "_merits", "_implants", "_externalItems" })
		{
			var field = typeof(ConcreteBody).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
			field.SetValue(body, Activator.CreateInstance(field.FieldType));
		}
		var reserve = new Mock<IStaminaCapacityMerit>();
		reserve.SetupGet(x => x.Id).Returns(789);
		reserve.SetupGet(x => x.MeritScope).Returns(MeritScope.Body);
		reserve.SetupGet(x => x.StaminaBonus).Returns(60);
		reserve.SetupGet(x => x.StaminaMultiplier).Returns(1);
		reserve.Setup(x => x.Applies(actor.Object)).Returns(true);
		var racial = new List<ICharacterMerit> { reserve.Object, reserve.Object };
		var race = new Mock<IRace>();
		race.SetupGet(x => x.IntrinsicMerits).Returns(() => racial);
		typeof(ConcreteBody).GetProperty(nameof(body.Race))!.SetValue(body, race.Object);
		actor.SetupGet(x => x.Body).Returns(body);
		actor.SetupGet(x => x.Merits).Returns(() => body.Merits);
		actor.SetupGet(x => x.ExplicitMerits).Returns(Array.Empty<IMerit>());
		body.MaximumStamina = 100;
		body.CurrentStamina = 80;
		Assert.AreEqual(1, body.Merits.Count());
		Assert.AreEqual(0, body.ExplicitMerits.Count());
		Assert.IsFalse(body.AddMerit(reserve.Object), "Intrinsic grants must not become sticky explicit assignments.");
		Assert.IsFalse(body.RemoveMerit(reserve.Object));
		Assert.AreEqual(160, body.MaximumStamina, 1e-9);
		Assert.AreEqual(80, body.CurrentStamina, 1e-9);
		body.CurrentStamina = 150;
		racial.Clear();
		Assert.AreEqual(100, body.CurrentStamina, 1e-9);
		racial.Add(reserve.Object);
		Assert.AreEqual(160, body.MaximumStamina, 1e-9);
		Assert.AreEqual(100, body.CurrentStamina, 1e-9);
	}

	private static Mock<ICharacter> NeedsCharacter(double food, double drink)
	{
		var body = new Mock<IBody>();
		body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(18);
		body.SetupGet(x => x.LiverAlcoholRemovalKilogramsPerHour).Returns(0.1);
		body.SetupGet(x => x.WaterLossLitresPerHour).Returns(0.2);
		body.SetupGet(x => x.LongtermExertion).Returns(ExertionLevel.Rest);
		var race = new Mock<IRace>();
		race.SetupGet(x => x.MaximumFoodSatiatedHours).Returns(food);
		race.SetupGet(x => x.MaximumDrinkSatiatedHours).Returns(drink);
		race.SetupGet(x => x.HungerRate).Returns(1);
		race.SetupGet(x => x.ThirstRate).Returns(1);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Race).Returns(race.Object);
		actor.SetupGet(x => x.Merits).Returns(Array.Empty<IMerit>());
		actor.Setup(x => x.CombinedEffectsOfType<INeedRateEffect>()).Returns(Array.Empty<INeedRateEffect>());
		return actor;
	}
}
