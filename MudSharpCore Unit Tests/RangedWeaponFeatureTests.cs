#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RangedWeaponFeatureTests
{
	[TestMethod]
	public void FireBlowgun_CheckClassification_MatchesRangedAttackChecks()
	{
		Assert.IsTrue(CheckType.FireBlowgun.IsPhysicalActivityCheck());
		Assert.IsTrue(CheckType.FireBlowgun.IsOffensiveCombatAction());
		Assert.IsTrue(CheckType.FireBlowgun.IsTargettedHostileCheck());
		Assert.IsTrue(CheckType.FireBlowgun.IsVisionInfluencedCheck());
		Assert.IsFalse(CheckType.FireBlowgun.IsDefensiveCombatAction());
		Assert.IsTrue((int)RangedWeaponType.Blowgun > (int)RangedWeaponType.Musket);
	}

}
