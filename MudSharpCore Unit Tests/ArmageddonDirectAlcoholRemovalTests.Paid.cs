#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonDirectAlcoholRemovalTests
{
	[TestMethod]
	[DataRow(1)]
	[DataRow(7)]
	public void ClearHead_SelectedGradeBoundPaidRouteAppliesExactNativeAlcoholBudget(int grade)
	{
		var (f, needs) = Fixture(0.1);
		var spell = LoadPaidContent(f);
		var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, SpellPower.ExtremelyStrong, Difficulty.Normal);
		Assert.AreEqual(-0.01 * grade, ((NeedDeltaEffect)copy.SpellEffects.Single()).DrunkDelta, 1e-12);
		Assert.IsTrue(((NeedDeltaEffect)copy.SpellEffects.Single().Clone()).DirectAlcoholRemoval);
		var result = f.Service.Cast(f.Intent(grade, false));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(Math.Max(0, 0.1 - 0.01 * grade), needs.AlcoholLitres, 1e-12);
		Assert.AreEqual(grade == 1 ? 93.0 : 50.0, f.Balances[f.Resources[1]]);
		Assert.AreEqual(1, f.Rolls);
	}

	[TestMethod]
	public void ClearHead_InvalidDirectDefinitionRefusesBeforePayment()
	{
		var (f, _) = Fixture(0.1);
		var row = LoadPaidContent(f).SnapshotModel();
		var definition = XElement.Parse(row.Definition);
		definition.Element("Effects")!.Elements().Single().Element("Drunk")!.Value = "NaN";
		row.Definition = definition.ToString();
		var spell = new MagicSpell(row, f.World.Object); f.Spells[0] = spell;
		Assert.IsTrue(spell.GradeConfigurationErrors().Any(x => x.Contains("needdelta")));
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent(7, false)).Status);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]);
		Assert.AreEqual(0, f.Rolls);
		Assert.AreEqual(0, f.Store.Operations.Count);
	}

	[TestMethod]
	[DataRow("0.01*grade")]
	[DataRow("-1000000*grade")]
	public void ClearHead_InvalidReboundScalarRefusesBeforePayment(string formula)
	{
		var (f, needs) = Fixture(0.1);
		var row = LoadPaidContent(f).SnapshotModel();
		var definition = XElement.Parse(row.Definition);
		definition.Element("ControlledPower")!.Element("ScalarBindings")!.Element("Binding")!.Value = formula;
		row.Definition = definition.ToString(); f.Spells[0] = new MagicSpell(row, f.World.Object);
		var result = f.Service.Cast(f.Intent(7, false));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(0.1, needs.AlcoholLitres, 1e-12);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]);
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Store.Operations.Count);
	}

	[TestMethod]
	public void ClearHead_PaidNativeSetterBodyCallbackQuarantinesWithoutRefundOrReplay()
	{
		var (f, needs) = Fixture(0.1);
		LoadPaidContent(f);
		var replacement = new Mock<IBody>();
		replacement.SetupGet(x => x.Id).Returns(999);
		replacement.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(5);
		Mock.Get(f.Actor.Object.OutputHandler).Setup(x => x.Send("native status callback", It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback(() => f.Actor.SetupGet(x => x.Body).Returns(replacement.Object));
		f.Body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(() =>
		{
			if (needs.AlcoholLitres < 0.1) f.Actor.Object.OutputHandler.Send("native status callback");
			return 5.0;
		});
		var result = f.Service.Cast(f.Intent(7, false));
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreEqual(0.03, needs.AlcoholLitres, 1e-12);
		Assert.AreEqual(50.0, f.Balances[f.Resources[1]], "A paid write is retained when its post-write ownership check is uncertain.");
		Assert.IsTrue(f.Store.Operations.Values.Any(x => x.Stage == "NeedsReview"));
		Assert.IsTrue(f.Actor.Object.Changed);
		Assert.IsTrue(f.Body.Object.Changed);
		Assert.IsFalse(replacement.Object.Changed);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent(7, false)).Status);
		Assert.AreEqual(0.03, needs.AlcoholLitres, 1e-12);
		Assert.AreEqual(50.0, f.Balances[f.Resources[1]]);
		Assert.AreEqual(1, f.Rolls);
	}

	private static MagicSpell LoadPaidContent(MagicCastingFixture f)
	{
		f.Actor.Setup(x => x.TargetActorOrCorpse("self", It.IsAny<PerceiveIgnoreFlags>())).Returns(f.Actor.Object);
		var content = ArmageddonPrimitiveSpellContent.CreateAlcoholRemovalContribution().Content;
		f.Expressions.Clear();
		var cost = content.CostRow(1); cost.Id = 1;
		var duration = content.DurationRow(1); duration.Id = 2;
		f.Expressions.Add(new TraitExpression(cost, f.World.Object));
		f.Expressions.Add(new TraitExpression(duration, f.World.Object));
		var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, 0).ToString();
		var spell = new MagicSpell(row, f.World.Object);
		f.Spells[0] = spell;
		f.Skills[1] = 100;
		f.Acquire(7);
		return spell;
	}
}
