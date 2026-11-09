#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatSeederSourceTests
{
	[TestMethod]
	public void ApplySeededMaximumTargets_RepairsNonDefaultCapsAndIsIdempotent()
	{
		var attack = new MudSharp.Models.WeaponAttack { MaximumTargets = 1 };

		Assert.IsTrue(CombatSeeder.ApplySeededMaximumTargets(attack, 3));
		Assert.AreEqual(3, attack.MaximumTargets);
		Assert.IsFalse(CombatSeeder.ApplySeededMaximumTargets(attack, 3));
		Assert.IsFalse(CombatSeeder.ApplySeededMaximumTargets(attack, 1));
		Assert.AreEqual(3, attack.MaximumTargets);
	}

}
