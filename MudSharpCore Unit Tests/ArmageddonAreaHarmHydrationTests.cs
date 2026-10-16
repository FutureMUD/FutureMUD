#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Planes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonAreaHarmHydrationTests
{
	[TestMethod]
	public void NewAreaDefinitions_HydrateAsReadyNativeSpells()
	{
		var fixture = new MagicCastingFixture();
		foreach (var (index, contribution) in ArmageddonAreaHarmSpellContent.Create(1).Select((x, i) => (i, x)))
		{
			var content = contribution.Content;
			var row = content.SpellRow(1, 1, 1);
			row.Id = 2000 + index;
			row.EffectDurationExpressionId = 2;
			row.Definition = content.BuildDefinition(10, 1, 0).ToString();
			var spell = new MagicSpell(row, fixture.World.Object);
			Assert.AreEqual(0, spell.GradeConfigurationErrors().Count, content.Key);
			Assert.IsTrue(spell.ReadyForGame, content.Key);
		}
	}

	[TestMethod]
	public void LeapingLightning_HydratesAllSevenBoundedGradesWithTypedSourceBindings()
	{
		var fixture = new MagicCastingFixture();
		var tags = new[] { Mock.Of<ITag>(x => x.Id == 51), Mock.Of<ITag>(x => x.Id == 52) };
		var planes = new[] { Mock.Of<IPlane>(x => x.Id == 61), Mock.Of<IPlane>(x => x.Id == 62) };
		fixture.World.SetupGet(x => x.Tags).Returns(MagicCastingFixture.Collection(() => tags));
		fixture.World.SetupGet(x => x.Planes).Returns(MagicCastingFixture.Collection(() => planes));
		var content = ArmageddonLeapingLightningSpellContent.Create(51, 52, 61, 62).Content;
		var row = content.SpellRow(1, 1, 1);
		row.Id = 2010; row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, 0).ToString();
		var spell = new MagicSpell(row, fixture.World.Object);
		Assert.AreEqual(0, spell.GradeConfigurationErrors().Count,
			string.Join(" | ", spell.GradeConfigurationErrors()));
		Assert.IsTrue(spell.ReadyForGame, spell.ReadyForGame ? "" : spell.WhyNotReadyForGame(fixture.Actor.Object));
		for (var grade = 1; grade <= 7; grade++)
		{
			Assert.AreEqual(grade + 1, spell.GradeProfile!.Area!.ApplicationsAtGrade(grade));
			Assert.AreEqual(2 * grade + 1, spell.GradeProfile.Area.ApplicationsAtGrade(grade, true));
		}
		var cold = new MagicSpell(spell.SnapshotModel(), fixture.World.Object);
		Assert.IsTrue(cold.ReadyForGame, cold.ReadyForGame ? "" : cold.WhyNotReadyForGame(fixture.Actor.Object));
		Assert.AreEqual(ArmageddonLeapingLightningSpellContent.ProtectionTag,
			((MudSharp.Magic.SpellEffects.DamageEffect)cold.SpellEffects.Single()).InterdictionTag);
	}
}
