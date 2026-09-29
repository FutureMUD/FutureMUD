#nullable enable

using System.Globalization;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Combat;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatPsychicConfigurationTests
{
	[DataTestMethod]
	[DataRow("75%", 0.75, true)]
	[DataRow("0%", 0.0, true)]
	[DataRow("100%", 1.0, true)]
	[DataRow("150%", 0.25, false)]
	[DataRow("-1%", 0.25, false)]
	[DataRow("NaN", 0.25, false)]
	[DataRow("", 0.25, false)]
	public void PsychicWeight_CommandValidatesAndRebalancesWithoutChangingTheSelectedWeight(string input, double expected, bool changed)
	{
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		var settings = new Mock<ICharacterCombatSettings>();
		settings.SetupAllProperties();
		settings.Object.PsychicUsePercentage = 0.25;
		settings.Object.NaturalWeaponPercentage = 0.75;
		actor.SetupGet(x => x.CombatSettings).Returns(settings.Object);
		actor.SetupGet(x => x.Account.Culture).Returns(CultureInfo.InvariantCulture);
		typeof(CombatModule).GetMethod("CombatConfigPsychic", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [actor.Object, new StringStack(input)]);
		Assert.AreEqual(expected, settings.Object.PsychicUsePercentage);
		Assert.AreEqual(changed, settings.Object.Changed);
		Assert.AreEqual(1.0, settings.Object.PsychicUsePercentage + settings.Object.NaturalWeaponPercentage +
			settings.Object.WeaponUsePercentage + settings.Object.MagicUsePercentage + settings.Object.AuxiliaryPercentage, 0.00005);
	}
}
