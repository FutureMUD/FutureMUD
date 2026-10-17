using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CheckEnumExtensionsTests
{
	[TestMethod]
	public void GuardingReactions_ArePhysicalVisionGeneralActivitiesOutsideCombat()
	{
		foreach (var type in new[] { CheckType.GuardingExitChallengeCheck, CheckType.GuardingItemChallengeCheck })
		{
			Assert.IsTrue(type.IsPhysicalActivityCheck());
			Assert.IsTrue(type.IsVisionInfluencedCheck());
			Assert.IsTrue(type.IsGeneralActivityCheck());
			Assert.IsFalse(type.IsOffensiveCombatAction());
			Assert.IsFalse(type.IsDefensiveCombatAction());
		}
		Assert.AreEqual("false", MudSharp.Framework.DefaultStaticSettings.DefaultStaticConfigurations["ArmageddonHostileGuardAttemptsEnabled"]);
	}
	[TestMethod]
	public void IsPhysicalActivityCheck_TreatsElectricalWorkAsPhysicalButNotProgramming()
	{
		Assert.IsTrue(CheckType.InstallElectricalComponentCheck.IsPhysicalActivityCheck());
		Assert.IsTrue(CheckType.ConfigureElectricalComponentCheck.IsPhysicalActivityCheck());
		Assert.IsFalse(CheckType.ProgrammingComponentCheck.IsPhysicalActivityCheck());
	}

	[TestMethod]
	public void InstallProstheticSurgery_IsPhysicalFriendlyAndVisionInfluenced()
	{
		Assert.IsTrue(CheckType.InstallProstheticSurgery.IsPhysicalActivityCheck());
		Assert.IsTrue(CheckType.InstallProstheticSurgery.IsTargettedFriendlyCheck());
		Assert.IsTrue(CheckType.InstallProstheticSurgery.IsVisionInfluencedCheck());
	}
}
