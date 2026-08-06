#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems.Components;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BlackPowderWeaponEnvironmentTests
{
	[TestMethod]
	public void CanHandlePowder_UnderwaterOrLiquidAtmosphere_ReturnsFalse()
	{
		Assert.IsFalse(BlackPowderWeaponEnvironment.CanHandlePowder(true, false));
		Assert.IsFalse(BlackPowderWeaponEnvironment.CanHandlePowder(false, true));
		Assert.IsTrue(BlackPowderWeaponEnvironment.CanHandlePowder(false, false));
	}

	[TestMethod]
	public void CanHandleExposedPowder_HeavyPrecipitation_ReturnsFalse()
	{
		Assert.IsTrue(BlackPowderWeaponEnvironment.CanHandleExposedPowder(true, 0.5));
		Assert.IsFalse(BlackPowderWeaponEnvironment.CanHandleExposedPowder(true, 0.5001));
		Assert.IsFalse(BlackPowderWeaponEnvironment.CanHandleExposedPowder(false, 0.0));
	}

	[TestMethod]
	public void CanSustainOpenFlame_VacuumOrHeavyPrecipitation_ReturnsFalse()
	{
		Assert.IsFalse(BlackPowderWeaponEnvironment.CanSustainOpenFlame(true, false, 0.0));
		Assert.IsFalse(BlackPowderWeaponEnvironment.CanSustainOpenFlame(true, true, 0.75));
		Assert.IsTrue(BlackPowderWeaponEnvironment.CanSustainOpenFlame(true, true, 0.0));
	}

	[TestMethod]
	public void CanPropagateSound_Vacuum_ReturnsFalse()
	{
		Assert.IsFalse(BlackPowderWeaponEnvironment.CanPropagateSound(false));
		Assert.IsTrue(BlackPowderWeaponEnvironment.CanPropagateSound(true));
	}
}
