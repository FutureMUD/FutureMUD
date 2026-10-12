#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SownSicknessThermalTests
{
	[TestMethod]
	public void NativeFeverIsBoundedAndAmbientProgressKeepsItsOwnEffect()
	{
		var world = new Mock<IFuturemud>();
		var owner = new Mock<IPerceivable>(); owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var sickness = new SownSicknessSymptoms(owner.Object, 7);
		var ambient = new ThermalImbalance(owner.Object) { TemperatureStatus = BodyTemperatureStatus.CriticalHypothermia, ImbalanceProgress = -100 };
		Assert.IsFalse(sickness.SavingEffect);
		Assert.IsFalse(sickness.IsAmbientExposureDriven);
		Assert.AreSame(ambient, ThermalImbalance.DominantAmbientExposureEffect([sickness, ambient]));
		Assert.IsNull(ThermalImbalance.DominantAmbientExposureEffect([sickness]));
		Assert.AreEqual(BodyTemperatureStatus.ModerateHyperthermia, sickness.TemperatureStatus);
		Assert.AreEqual(BodyTemperatureStatus.VeryMildHyperthermia, SownSicknessSymptoms.StatusForGrade(1));
		Assert.AreEqual(BodyTemperatureStatus.VeryMildHyperthermia, SownSicknessSymptoms.StatusForGrade(3));
		Assert.AreEqual(BodyTemperatureStatus.MildHyperthermia, SownSicknessSymptoms.StatusForGrade(4));
		Assert.AreEqual(BodyTemperatureStatus.MildHyperthermia, SownSicknessSymptoms.StatusForGrade(6));
		Assert.AreEqual(1.7, sickness.Multiplier, 0.000001);
		Assert.AreEqual(0.65, ((IStaminaRegenerationRateEffect)sickness).Multiplier, 0.000001);
	}
}
