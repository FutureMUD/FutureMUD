#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonItemConditionHydrationTests
{
	[TestMethod]
	public void ItemConditionDefinitions_HydrateAsReadyNativeSpells()
	{
		var fixture = new MagicCastingFixture();
		foreach (var (index, contribution) in ArmageddonItemConditionSpellContent.Create().Select((x, i) => (i, x)))
		{
			var content = contribution.Content;
			var row = content.SpellRow(1, 1, 1);
			row.Id = 2200 + index;
			row.EffectDurationExpressionId = 2;
			row.Definition = content.BuildDefinition(10, 1, 0).ToString();
			var spell = new MagicSpell(row, fixture.World.Object);
			Assert.AreEqual(0, spell.GradeConfigurationErrors().Count, content.Key);
			Assert.IsTrue(spell.ReadyForGame, content.Key);
		}
	}
}
