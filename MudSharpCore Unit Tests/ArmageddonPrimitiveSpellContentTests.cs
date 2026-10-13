#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.RPG.Checks;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonPrimitiveSpellContentTests
{
	[DataTestMethod]
	[DataRow(0L)]
	[DataRow(1L)]
	public void NativeFactory_EveryContributionRoundTripsAndEveryGradeBindsWithoutMutation(long filter)
	{
		var f = new MagicCastingFixture();
		Mock.Get(f.Traits[2]).SetupGet(x => x.TraitType).Returns(TraitType.Attribute);
		Mock.Get(f.Traits[2]).SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Body);
		foreach (var contribution in ArmageddonPrimitiveSpellContent.Create(new(3, 2, 1, 11, Difficulty.Normal)))
		{
			var content = contribution.Content;
			var spell = Load(f, content, filter);
			Assert.AreEqual(content.Key, spell.StockIdentity);
			Assert.AreEqual(0, spell.GradeConfigurationErrors().Count, string.Join("\n", spell.GradeConfigurationErrors()));
			var template = spell.SpellEffects.Single();
			var source = content.BuildDefinition(10, 1, filter).Element("Effects")!.Elements().Single();
			Assert.IsTrue(XNode.DeepEquals(source, template.SaveToXml()), content.Key);
			Assert.IsTrue(XNode.DeepEquals(source, template.Clone().SaveToXml()), content.Key);
			Assert.IsTrue(template.IsCompatibleWithTrigger(spell.Trigger), content.Key);
			var reload = new MagicSpell(spell.SnapshotModel(), f.World.Object);
			Assert.IsTrue(XNode.DeepEquals(template.SaveToXml(), reload.SpellEffects.Single().SaveToXml()), content.Key);
			var trigger = (CastingTriggerCharacter)reload.Trigger;
			Assert.AreEqual(filter, trigger.TargetFilterProgId, content.Key);
			if (filter == 0)
			{
				Assert.IsNull(trigger.TargetFilterError, content.Key);
				Assert.IsTrue(trigger.AllowsTarget(f.Actor.Object, f.Actor.Object), content.Key);
			}
			for (var grade = 1; grade <= 7; grade++)
			{
				var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade,
					(SpellPower)((int)SpellPower.ExtremelyWeak + grade - 1), Difficulty.Normal);
				Assert.AreEqual(content.DurationFormula == "0" ? 0.0 : 60.0 * grade, copy.EffectDurationExpression.Evaluate(f.Actor.Object), content.Key);
				if (copy.SpellEffects.Single() is TraitBoostEffect boost)
				{
					Assert.AreSame(f.Traits[2], boost.Trait);
					Assert.AreEqual(TraitType.Attribute, boost.Trait.TraitType);
					Assert.AreEqual(content.Key.EndsWith("sapping_weight") ? -grade : grade, boost.Bonus, content.Key);
				}
				Assert.AreEqual(content.MinimumEnergy * grade, copy.CastingCosts.Single().Value.Evaluate(f.Actor.Object), content.Key);
			}
			Assert.IsTrue(XNode.DeepEquals(source, template.SaveToXml()), "Invocation copies must leave authoring defaults intact: " + content.Key);
			var efficiency = spell.GradeProfile!.Efficiency!;
			Assert.AreEqual(50.0, efficiency.Cost(1, 1));
			Assert.AreEqual(Math.Max(content.MinimumEnergy, 7.0), efficiency.Cost(7, 1), content.Key);
			Assert.AreEqual(75.0, efficiency.Cost(2, 3));
			Assert.AreEqual(112.5, efficiency.Cost(2, 3) * spell.GradeProfile.OverreachMultiplier);
		}
	}

	[DataTestMethod]
	[DataRow(1, 0.0, 5.0)]
	[DataRow(7, 0.0, 35.0)]
	[DataRow(7, 98.0, 100.0)]
	[DataRow(1, 100.0, 100.0)]
	public void SecondBreath_SelectedGradeRestoresNativeCurrentStaminaWithoutOvercredit(int grade, double before, double after)
	{
		var f = new MagicCastingFixture();
		var body = TestObjectFactory.CreateUninitialized<ConcreteBody>();
		body.SetNoSave(true);
		body.MaximumStamina = 100;
		body.CurrentStamina = before;
		f.Actor.Setup(x => x.GainStamina(It.IsAny<double>())).Callback<double>(body.GainStamina);
		var content = ArmageddonPrimitiveSpellContent.Create(new(3, 2, 1, 11, Difficulty.Normal))
			.Single(x => x.Content.Key == "arm.spell.second_breath").Content;
		var copy = Load(f, content).CastingCopy(f.Actor.Object, f.Traits[0], grade,
			(SpellPower)((int)SpellPower.ExtremelyWeak + grade - 1), Difficulty.Normal);
		Assert.IsNull(copy.SpellEffects.Single().GetOrApplyEffect(f.Actor.Object, f.Actor.Object,
			OpposedOutcomeDegree.None, SpellPower.ExtremelyStrong, null!, []));
		Assert.AreEqual(after, body.CurrentStamina);
		f.Actor.Verify(x => x.GainStamina(5.0 * grade), Times.Once);
		f.Actor.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(1)]
	[DataRow(7)]
	public void EmptyAura_SelectedGradeTargetsExactConfiguredReserveWithoutCasterCredit(int grade)
	{
		var f = new MagicCastingFixture();
		var target = new Mock<MudSharp.Character.ICharacter>();
		var content = ArmageddonPrimitiveSpellContent.Create(new(3, 2, 1, 11, Difficulty.Normal))
			.Single(x => x.Content.Key == "arm.spell.empty_aura").Content;
		var copy = Load(f, content).CastingCopy(f.Actor.Object, f.Traits[0], grade,
			(SpellPower)((int)SpellPower.ExtremelyWeak + grade - 1), Difficulty.Normal);
		copy.SpellEffects.Single().GetOrApplyEffect(f.Actor.Object, target.Object,
			OpposedOutcomeDegree.None, SpellPower.ExtremelyStrong, null!, []);
		var resource = f.Resources.Single(x => x.Id == 11);
		target.Verify(x => x.AddResource(resource, -5.0 * grade), Times.Once);
		f.Actor.Verify(x => x.AddResource(It.IsAny<IMagicResource>(), It.IsAny<double>()), Times.Never);
	}

	private static MagicSpell Load(MagicCastingFixture f, ArmageddonUtilitySpellContent content, long filter = 1)
	{
		f.Expressions.Clear();
		var cost = content.CostRow(1); cost.Id = 1;
		var duration = content.DurationRow(1); duration.Id = 2;
		f.Expressions.Add(new TraitExpression(cost, f.World.Object));
		f.Expressions.Add(new TraitExpression(duration, f.World.Object));
		var row = content.SpellRow(1, 1, 1); row.Id = 900;
		row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, filter).ToString();
		return new MagicSpell(row, f.World.Object);
	}
}
