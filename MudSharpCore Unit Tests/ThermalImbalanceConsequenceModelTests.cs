#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Health;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ThermalImbalanceConsequenceModelTests
{
    [TestMethod]
    public void StageSeverity_VeryMildStages_HaveNoMechanicalPenalty()
    {
        Assert.AreEqual(0.0,
            ThermalImbalanceConsequenceModel.StageSeverity(BodyTemperatureStatus.VeryMildHypothermia));
        Assert.AreEqual(0.0,
            ThermalImbalanceConsequenceModel.StageSeverity(BodyTemperatureStatus.VeryMildHyperthermia));
    }

    [TestMethod]
    public void MovementDelayMultiplier_SevereHypothermia_InterpolatesFromConfiguredMaximum()
    {
        double result = ThermalImbalanceConsequenceModel.MovementDelayMultiplier(
            BodyTemperatureStatus.SevereHypothermia,
            2.0,
            1.5);

        Assert.AreEqual(1.7, result, 0.0001);
    }

    [TestMethod]
    public void StaminaMultiplier_SevereHyperthermia_InterpolatesFromConfiguredMaximum()
    {
        double result = ThermalImbalanceConsequenceModel.StaminaMultiplier(
            BodyTemperatureStatus.SevereHyperthermia,
            1.5,
            1.75);

        Assert.AreEqual(1.525, result, 0.0001);
    }

    [TestMethod]
    public void StaminaRegenerationMultiplier_CriticalHyperthermia_UsesConfiguredMinimum()
    {
        double result = ThermalImbalanceConsequenceModel.StaminaRegenerationMultiplier(
            BodyTemperatureStatus.CriticalHyperthermia,
            0.5,
            0.4);

        Assert.AreEqual(0.4, result, 0.0001);
    }

    [TestMethod]
    public void OrganPenaltySeverity_SevereStage_UsesConfiguredFloorOnly()
    {
        double result = ThermalImbalanceConsequenceModel.OrganPenaltySeverity(
            BodyTemperatureStatus.SevereHyperthermia,
            0.0,
            1800.0,
            -1800.0,
            0.1,
            8.0);

        Assert.AreEqual(0.1, result, 0.0001);
    }

    [TestMethod]
    public void OrganPenaltySeverity_CriticalStage_RampsWithProgressAndClamps()
    {
        double halfway = ThermalImbalanceConsequenceModel.OrganPenaltySeverity(
            BodyTemperatureStatus.CriticalHyperthermia,
            7200.0,
            1800.0,
            -1800.0,
            0.1,
            8.0);
        double capped = ThermalImbalanceConsequenceModel.OrganPenaltySeverity(
            BodyTemperatureStatus.CriticalHypothermia,
            -20000.0,
            1800.0,
            -1800.0,
            0.1,
            8.0);

        Assert.AreEqual(0.55, halfway, 0.0001);
        Assert.AreEqual(1.0, capped, 0.0001);
    }
}
