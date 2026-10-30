using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions.Characters;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Merits;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ChangingNeedsModelBaseTests
{
    [TestMethod]
    public void CalculateStarvationAndOversatiationLevelsMatchFoodState()
    {
        Assert.AreEqual(0.0, ChangingNeedsModelBase.CalculateStarvationLevel(4.0), 1e-6);
        Assert.AreEqual(3.5, ChangingNeedsModelBase.CalculateStarvationLevel(-3.5), 1e-6);
        Assert.AreEqual(0.0, ChangingNeedsModelBase.CalculateOversatiationLevel(12.0), 1e-6);
        Assert.AreEqual(4.0, ChangingNeedsModelBase.CalculateOversatiationLevel(16.0), 1e-6);
    }

    [TestMethod]
    public void CalculateOversatiationLevel_UsesRacialFoodLimit()
    {
        Assert.AreEqual(0.0, ChangingNeedsModelBase.CalculateOversatiationLevel(540.0, 720.0), 1e-6);
        Assert.AreEqual(60.0, ChangingNeedsModelBase.CalculateOversatiationLevel(600.0, 720.0), 1e-6);
    }

    [TestMethod]
    public void EffectiveSatiationLimit_RejectsTinyOrNonFiniteValues()
    {
        Assert.AreEqual(RacialSatiationDefaults.MaximumFoodSatiatedHours,
            ChangingNeedsModelBase.GetEffectiveFoodSatiationLimit(double.Epsilon), 1e-6);
        Assert.AreEqual(RacialSatiationDefaults.MaximumFoodSatiatedHours,
            ChangingNeedsModelBase.GetEffectiveFoodSatiationLimit(double.PositiveInfinity), 1e-6);
        Assert.AreEqual(1.0, ChangingNeedsModelBase.GetEffectiveFoodSatiationLimit(1.0), 1e-6);
    }

    [TestMethod]
    public void GetHungerStatus_ScalesThresholdsWithFoodLimit()
    {
        Assert.AreEqual(NeedsResult.AbsolutelyStuffed, ChangingNeedsModelBase.GetHungerStatus(540.0, 720.0));
        Assert.AreEqual(NeedsResult.Full, ChangingNeedsModelBase.GetHungerStatus(360.0, 720.0));
        Assert.AreEqual(NeedsResult.Peckish, ChangingNeedsModelBase.GetHungerStatus(180.0, 720.0));
        Assert.AreEqual(NeedsResult.Hungry, ChangingNeedsModelBase.GetHungerStatus(0.1, 720.0));
        Assert.AreEqual(NeedsResult.Starving, ChangingNeedsModelBase.GetHungerStatus(0.0, 720.0));
    }

    [TestMethod]
    public void GetThirstStatus_ScalesThresholdsWithDrinkLimit()
    {
        Assert.AreEqual(NeedsResult.Sated, ChangingNeedsModelBase.GetThirstStatus(180.0, 240.0));
        Assert.AreEqual(NeedsResult.NotThirsty, ChangingNeedsModelBase.GetThirstStatus(120.0, 240.0));
        Assert.AreEqual(NeedsResult.Thirsty, ChangingNeedsModelBase.GetThirstStatus(0.1, 240.0));
        Assert.AreEqual(NeedsResult.Parched, ChangingNeedsModelBase.GetThirstStatus(0.0, 240.0));
    }

    [TestMethod]
    public void NeedsEntryEvents_FireOnlyWhenEnteringHungryOrThirstyCategories()
    {
        Assert.AreEqual(EventType.CharacterHungry,
            ChangingNeedsModelBase.GetHungerEntryEvent(NeedsResult.Peckish, NeedsResult.Hungry));
        Assert.AreEqual(EventType.CharacterStarving,
            ChangingNeedsModelBase.GetHungerEntryEvent(NeedsResult.Hungry, NeedsResult.Starving));
        Assert.IsNull(ChangingNeedsModelBase.GetHungerEntryEvent(NeedsResult.Hungry, NeedsResult.Hungry));
        Assert.IsNull(ChangingNeedsModelBase.GetHungerEntryEvent(NeedsResult.Hungry, NeedsResult.Peckish));

        Assert.AreEqual(EventType.CharacterThirsty,
            ChangingNeedsModelBase.GetThirstEntryEvent(NeedsResult.NotThirsty, NeedsResult.Thirsty));
        Assert.AreEqual(EventType.CharacterParched,
            ChangingNeedsModelBase.GetThirstEntryEvent(NeedsResult.Thirsty, NeedsResult.Parched));
        Assert.IsNull(ChangingNeedsModelBase.GetThirstEntryEvent(NeedsResult.Thirsty, NeedsResult.Thirsty));
        Assert.IsNull(ChangingNeedsModelBase.GetThirstEntryEvent(NeedsResult.Thirsty, NeedsResult.NotThirsty));
    }

    [TestMethod]
    public void SatisfyNeeds_AdditionsCompensateForNativeNeedRateMultipliers()
    {
		var effect = new Mock<INeedRateEffect>();
		effect.SetupGet(x => x.HungerMultiplier).Returns(0.5);
		effect.SetupGet(x => x.ThirstMultiplier).Returns(2.0);
		effect.SetupGet(x => x.AppliesToActive).Returns(true);
		var character = CreateNeedsCharacter();
		effect.Setup(x => x.Applies(character.Object)).Returns(true);
		var inactive = new Mock<INeedRateEffect>();
		inactive.Setup(x => x.Applies(character.Object)).Returns(false);
		inactive.SetupGet(x => x.AppliesToActive).Returns(true);
		inactive.SetupGet(x => x.HungerMultiplier).Returns(100);
		inactive.SetupGet(x => x.ThirstMultiplier).Returns(100);
		character.Setup(x => x.CombinedEffectsOfType<INeedRateEffect>()).Returns([inactive.Object, effect.Object]);
		var model = new PassiveNeedsModel(character.Object);
		var multipliers = model.GetFulfilmentMultipliers();
		Assert.AreEqual(0.5, multipliers.Hunger, 1e-6);
		Assert.AreEqual(2.0, multipliers.Thirst, 1e-6);

		Assert.IsTrue(SatisfyNeedsFunction.TryGetSatisfactionAddition(true, 0.0, 8.0, multipliers.Hunger, out var foodAddition));
		Assert.AreEqual(16.0, foodAddition, 1e-6);
		Assert.IsTrue(SatisfyNeedsFunction.TryGetSatisfactionAddition(true, 0.0, 4.0, multipliers.Thirst, out var drinkAddition));
		Assert.AreEqual(2.0, drinkAddition, 1e-6);
		Assert.IsFalse(SatisfyNeedsFunction.TryGetSatisfactionAddition(true, 0.0, 8.0, 0.0, out _));
		Assert.IsTrue(SatisfyNeedsFunction.TryGetSatisfactionAddition(true, 8.0, 8.0, 0.0, out var alreadySatisfiedAddition));
		Assert.AreEqual(0.0, alreadySatisfiedAddition, 1e-6);
    }

	[TestMethod]
	public void SatisfyNeedsFunction_WithHalfAndDoubleMultipliers_ReachesFullAndNotThirsty()
	{
		double hungerMultiplier = 1.0;
		double thirstMultiplier = 1.0;
		var effect = new Mock<INeedRateEffect>();
		effect.SetupGet(x => x.HungerMultiplier).Returns(() => hungerMultiplier);
		effect.SetupGet(x => x.ThirstMultiplier).Returns(() => thirstMultiplier);
		effect.SetupGet(x => x.AppliesToActive).Returns(true);
		var character = CreateNeedsCharacter();
		character.Setup(x => x.CombinedEffectsOfType<INeedRateEffect>()).Returns([effect.Object]);
		var body = Mock.Get(character.Object.Body);
		var model = new PassiveNeedsModel(character.Object);
		character.SetupGet(x => x.NeedsModel).Returns(model);
		model.FulfilNeeds(new NeedFulfiller { SatiationPoints = -16.0, ThirstPoints = -8.0 });
		Assert.AreEqual(NeedsResult.Starving, model.Status & NeedsResult.HungerOnly);
		Assert.AreEqual(NeedsResult.Parched, model.Status & NeedsResult.ThirstOnly);

		body.Setup(x => x.FulfilNeeds(It.IsAny<INeedFulfiller>(), It.IsAny<bool>()))
			.Callback<INeedFulfiller, bool>((fulfiller, ignoreDelays) => model.FulfilNeeds(fulfiller, ignoreDelays))
			.Returns(NeedsResult.None);
		hungerMultiplier = 0.5;
		thirstMultiplier = 2.0;
		var arguments = new IFunction[]
		{
			CreateFunction(character.Object),
			CreateFunction(new BooleanVariable(true)),
			CreateFunction(new BooleanVariable(true))
		};
		var function = new SatisfyNeedsFunction(arguments);

		Assert.AreEqual(StatementResult.Normal, function.Execute(null!));
		Assert.AreEqual(true, function.Result.GetObject);
		Assert.AreEqual(NeedsResult.Full, model.Status & NeedsResult.HungerOnly);
		Assert.AreEqual(NeedsResult.NotThirsty, model.Status & NeedsResult.ThirstOnly);
	}

	[TestMethod]
	public void CharacterNeedsFutureProgGetter_MapsNoNeedsUnboundedValueToLargestDecimal()
	{
		var character = (global::MudSharp.Character.Character)RuntimeHelpers.GetUninitializedObject(typeof(global::MudSharp.Character.Character));
		typeof(global::MudSharp.Character.Character).GetProperty("NeedsModel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
			.SetValue(character, new NoNeedsModel());

		var value = ((IProgVariable)character).GetProperty("foodsatiatedhours");
		Assert.AreEqual(decimal.MaxValue, value.GetObject);
	}

	[TestMethod]
	public void NeedsTransitionDispatch_SnapshotsRoomAndWitnessesBeforeSelfHookAndSuppressesReentry()
	{
		var actor = new Mock<ICharacter>();
		var originalRoom = new Mock<IRoom>();
		var movedRoom = new Mock<IRoom>();
		var witness = new Mock<IHandleEvents>();
		var itemHook = new Mock<IHandleEvents>();
		actor.SetupGet(x => x.Location).Returns(originalRoom.Object);
		originalRoom.Setup(x => x.EventHandlersFor(actor.Object)).Returns([witness.Object, itemHook.Object]);
		object[]? actorPayload = null;
		object[]? witnessPayload = null;
		object[]? itemPayload = null;
		actor.Setup(x => x.HandleEvent(EventType.CharacterHungry, It.IsAny<object[]>()))
			.Callback<EventType, object[]>((_, arguments) =>
			{
				actorPayload = arguments;
				actor.SetupGet(x => x.Location).Returns(movedRoom.Object);
				ChangingNeedsModelBase.DispatchNeedsTransitions(actor.Object, movedRoom.Object,
					NeedsResult.Hungry, NeedsResult.Hungry);
			});
		witness.Setup(x => x.HandleEvent(EventType.CharacterInRoomHungry, It.IsAny<object[]>()))
			.Callback<EventType, object[]>((_, arguments) => witnessPayload = arguments);
		itemHook.Setup(x => x.HandleEvent(EventType.CharacterInRoomHungry, It.IsAny<object[]>()))
			.Callback<EventType, object[]>((_, arguments) => itemPayload = arguments);

		ChangingNeedsModelBase.DispatchNeedsTransitions(actor.Object, originalRoom.Object,
			NeedsResult.Peckish, NeedsResult.Hungry);
		ChangingNeedsModelBase.DispatchNeedsTransitions(actor.Object, movedRoom.Object,
			NeedsResult.Hungry, NeedsResult.Hungry);

		Assert.IsNotNull(actorPayload);
		Assert.AreSame(actor.Object, actorPayload[0]);
		Assert.AreSame(originalRoom.Object, actorPayload[1]);
		Assert.IsNotNull(witnessPayload);
		Assert.AreSame(actor.Object, witnessPayload[0]);
		Assert.AreSame(originalRoom.Object, witnessPayload[1]);
		Assert.AreSame(witness.Object, witnessPayload[2]);
		Assert.IsNotNull(itemPayload);
		Assert.AreSame(itemHook.Object, itemPayload[2]);
		actor.Verify(x => x.HandleEvent(EventType.CharacterHungry, It.IsAny<object[]>()), Times.Once);
		witness.Verify(x => x.HandleEvent(EventType.CharacterInRoomHungry, It.IsAny<object[]>()), Times.Once);
		itemHook.Verify(x => x.HandleEvent(EventType.CharacterInRoomHungry, It.IsAny<object[]>()), Times.Once);
	}

    [TestMethod]
    public void PositiveSatiationRecoverDeficitBeforeCreatingExcess()
    {
        double result = ChangingNeedsModelBase.ApplySatiationReserveFromFulfiller(-3.0, 8.0, 8.0);
        Assert.AreEqual(2.5, result, 1e-6);
    }

    [TestMethod]
    public void PositiveSatiationWithoutOversatiationDoesNotCreateExcess()
    {
        double result = ChangingNeedsModelBase.ApplySatiationReserveFromFulfiller(-3.0, 0.0, 8.0);
        Assert.AreEqual(0.0, result, 1e-6);
    }

    [TestMethod]
    public void NegativeSatiationStillReducesStoredReserve()
    {
        double result = ChangingNeedsModelBase.ApplySatiationReserveFromFulfiller(1.25, 10.0, -2.0);
        Assert.AreEqual(-0.75, result, 1e-6);
    }

    [TestMethod]
    public void StarvationDeficitMultiplierScalesAndClamps()
    {
        Assert.AreEqual(0.0, ChangingNeedsModelBase.GetStarvationSatiationDeficitMultiplier(0.0), 1e-6);
        Assert.AreEqual(0.25, ChangingNeedsModelBase.GetStarvationSatiationDeficitMultiplier(0.1), 1e-6);
        Assert.AreEqual(0.75, ChangingNeedsModelBase.GetStarvationSatiationDeficitMultiplier(0.75), 1e-6);
        Assert.AreEqual(1.0, ChangingNeedsModelBase.GetStarvationSatiationDeficitMultiplier(4.0), 1e-6);
    }

    [TestMethod]
	public void ExertionMultiplierOnlyAppliesAtHeavyOrAbove()
    {
        Assert.AreEqual(0.0, ChangingNeedsModelBase.GetExertionSatiationBurnMultiplier(ExertionLevel.Rest), 1e-6);
        Assert.AreEqual(0.5, ChangingNeedsModelBase.GetExertionSatiationBurnMultiplier(ExertionLevel.Heavy), 1e-6);
        Assert.AreEqual(1.0, ChangingNeedsModelBase.GetExertionSatiationBurnMultiplier(ExertionLevel.VeryHeavy), 1e-6);
		Assert.AreEqual(1.5,
			ChangingNeedsModelBase.GetExertionSatiationBurnMultiplier(ExertionLevel.ExtremelyHeavy), 1e-6);
	}

	[TestMethod]
	public void ActiveNoThirstNeedsModel_ConsumesFoodAndAlcoholWithoutTrackingThirst()
	{
		Mock<ICharacter> character = CreateNeedsCharacter();
		var model = new ActiveNoThirstNeedsModel(character.Object);
		double initialWater = model.WaterLitres;
		double initialFood = model.FoodSatiatedHours;

		NeedsResult fulfilled = model.FulfilNeeds(new NeedFulfiller
		{
			SatiationPoints = -2.0,
			ThirstPoints = -100.0,
			WaterLitres = 10.0,
			AlcoholLitres = 0.5
		}, true);

		Assert.AreEqual(ActiveNoThirstNeedsModel.ModelNameValue, model.ModelName);
		Assert.AreEqual(8.0, model.DrinkSatiatedHours, 1e-6);
		Assert.AreEqual(initialWater, model.WaterLitres, 1e-6);
		Assert.IsTrue(model.FoodSatiatedHours < initialFood);
		Assert.IsTrue(model.AlcoholLitres > 0.0);
		Assert.AreEqual(NeedsResult.Sated, fulfilled & NeedsResult.ThirstOnly);

		double foodBeforeHeartbeat = model.FoodSatiatedHours;
		double alcoholBeforeHeartbeat = model.AlcoholLitres;
		double oldTimeScale = ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds;
		try
		{
			ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = 60.0;
			model.NeedsHeartbeat();
		}
		finally
		{
			ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds = oldTimeScale;
		}

		Assert.IsTrue(model.FoodSatiatedHours < foodBeforeHeartbeat);
		Assert.IsTrue(model.AlcoholLitres < alcoholBeforeHeartbeat);
		Assert.AreEqual(8.0, model.DrinkSatiatedHours, 1e-6);
		Assert.AreEqual(initialWater, model.WaterLitres, 1e-6);
	}

	[TestMethod]
	public void ActiveNoThirstNeedsModel_FactoryAndConversionPreserveActiveState()
	{
		Mock<ICharacter> character = CreateNeedsCharacter();
		var savedNeeds = new Mock<INeedsModel>();
		savedNeeds.SetupGet(x => x.NeedsSave).Returns(true);
		savedNeeds.SetupGet(x => x.FoodSatiatedHours).Returns(4.0);
		savedNeeds.SetupGet(x => x.DrinkSatiatedHours).Returns(0.0);
		savedNeeds.SetupGet(x => x.AlcoholLitres).Returns(0.4);
		savedNeeds.SetupGet(x => x.WaterLitres).Returns(1.5);
		savedNeeds.SetupGet(x => x.SatiationReserve).Returns(0.75);

		INeedsModel converted = NeedsModelFactory.ConvertNeedsModel(ActiveNoThirstNeedsModel.ModelNameValue,
			character.Object, savedNeeds.Object);
		INeedsModel loaded = NeedsModelFactory.LoadNeedsModel(new MudSharp.Models.Character
		{
			NeedsModel = ActiveNoThirstNeedsModel.ModelNameValue,
			FoodSatiatedHours = 3.0,
			DrinkSatiatedHours = 0.0,
			AlcoholLitres = 0.3,
			WaterLitres = 1.2,
			SatiationReserve = 0.5
		}, character.Object);

		Assert.IsInstanceOfType(converted, typeof(ActiveNoThirstNeedsModel));
		Assert.AreEqual(4.0, converted.FoodSatiatedHours, 1e-6);
		Assert.AreEqual(0.4, converted.AlcoholLitres, 1e-6);
		Assert.AreEqual(0.75, converted.SatiationReserve, 1e-6);
		Assert.AreEqual(8.0, converted.DrinkSatiatedHours, 1e-6);
		Assert.AreEqual(ActiveNoThirstNeedsModel.ModelNameValue, loaded.ModelName);
		Assert.AreEqual(3.0, loaded.FoodSatiatedHours, 1e-6);
		Assert.AreEqual(8.0, loaded.DrinkSatiatedHours, 1e-6);
	}

	private static Mock<ICharacter> CreateNeedsCharacter()
	{
		var body = new Mock<IBody>();
		body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(5.0);
		body.SetupGet(x => x.LiverAlcoholRemovalKilogramsPerHour).Returns(0.1);
		body.SetupGet(x => x.WaterLossLitresPerHour).Returns(0.2);
		body.SetupGet(x => x.LongtermExertion).Returns(ExertionLevel.Rest);

		var race = new Mock<IRace>();
		race.SetupGet(x => x.MaximumFoodSatiatedHours).Returns(16.0);
		race.SetupGet(x => x.MaximumDrinkSatiatedHours).Returns(8.0);
		race.SetupGet(x => x.HungerRate).Returns(1.0);
		race.SetupGet(x => x.ThirstRate).Returns(1.0);

		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.GetObject).Returns(character.Object);
		character.SetupGet(x => x.Body).Returns(body.Object);
		character.SetupGet(x => x.Race).Returns(race.Object);
		character.SetupGet(x => x.Merits).Returns(Array.Empty<IMerit>());
		character.Setup(x => x.CombinedEffectsOfType<INeedRateEffect>())
			.Returns(Array.Empty<INeedRateEffect>());
		return character;
	}

	private static IFunction CreateFunction(IProgVariable result)
	{
		var function = new Mock<IFunction>();
		function.SetupGet(x => x.Result).Returns(result);
		function.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
		function.SetupGet(x => x.ErrorMessage).Returns(string.Empty);
		return function.Object;
	}
}
