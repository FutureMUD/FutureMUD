using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Effects.Interfaces;
using MudSharp.RPG.Merits;
using System;

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
		character.SetupGet(x => x.Body).Returns(body.Object);
		character.SetupGet(x => x.Race).Returns(race.Object);
		character.SetupGet(x => x.Merits).Returns(Array.Empty<IMerit>());
		character.Setup(x => x.CombinedEffectsOfType<INeedRateEffect>())
			.Returns(Array.Empty<INeedRateEffect>());
		return character;
	}
}
