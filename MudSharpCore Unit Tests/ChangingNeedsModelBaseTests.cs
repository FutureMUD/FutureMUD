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
