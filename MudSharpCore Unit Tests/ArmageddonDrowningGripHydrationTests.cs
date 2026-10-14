#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonDrowningGripHydrationTests
{
	[TestMethod]
	public void StockDefinition_HydratesReadyNativeBreathingEffect()
	{
		var fixture = new MagicCastingFixture();
		var content = ArmageddonDrowningGripSpellContent.Create(1).Content;
		var row = content.SpellRow(1, 1, 1);
		row.Id = 2299;
		row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, 0).ToString();
		var spell = new MagicSpell(row, fixture.World.Object);
		Assert.AreEqual(0, spell.GradeConfigurationErrors().Count);
		Assert.IsTrue(spell.ReadyForGame);
		Assert.IsInstanceOfType(spell.SpellEffects.Single(), typeof(DrowningGripEffect));
	}
}
