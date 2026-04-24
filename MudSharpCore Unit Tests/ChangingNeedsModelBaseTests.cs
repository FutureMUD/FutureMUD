using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body;
using MudSharp.Body.Needs;

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
}
