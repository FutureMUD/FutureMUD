#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Combat;
using MudSharp.Combat.Strategies;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatStrategyLoadoutTests
{
	[TestMethod]
	public void DesiredMeleeWeaponCount_DualWieldSetup_RequiresTwoWeapons()
	{
		var settings = new Mock<ICharacterCombatSettings>();
		settings.SetupGet(x => x.PreferredWeaponSetup).Returns(AttackHandednessOptions.DualWieldOnly);

		Assert.AreEqual(2, StrategyBase.DesiredMeleeWeaponCount(settings.Object));
	}

	[TestMethod]
	public void DesiredMeleeWeaponCount_OtherSetups_RequireOneWeapon()
	{
		foreach (var setup in new[]
		         {
			         AttackHandednessOptions.Any,
			         AttackHandednessOptions.OneHandedOnly,
			         AttackHandednessOptions.TwoHandedOnly
		         })
		{
			var settings = new Mock<ICharacterCombatSettings>();
			settings.SetupGet(x => x.PreferredWeaponSetup).Returns(setup);

			Assert.AreEqual(1, StrategyBase.DesiredMeleeWeaponCount(settings.Object), setup.ToString());
		}
	}
}
