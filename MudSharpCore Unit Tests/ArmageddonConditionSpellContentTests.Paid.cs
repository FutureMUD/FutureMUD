#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Health;
using MudSharp.Health.Infections;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;
using MudSharp.Planes;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonConditionSpellContentTests
{
	[TestMethod]
	public void DiseasePair_AllGradesBindExactCostDurationAndNativeIntensityWithoutMutatingAuthoring()
	{
		foreach (var suffix in new[] { "sow_sickness", "purge_disease" })
		{
			var f = new Fixture(); var spell = LoadPaid(f, suffix); var original = spell.SpellEffects.Single().SaveToXml();
			Assert.AreEqual(0, spell.GradeConfigurationErrors().Count, string.Join("\n", spell.GradeConfigurationErrors()));
			for (var grade = 1; grade <= 7; grade++)
			{
				var power = grade % 2 == 1 ? SpellPower.ExtremelyStrong : SpellPower.ExtremelyWeak;
				var copy = spell.CastingCopy(f.Base.Actor.Object, f.Base.Traits[0], grade, power, Difficulty.Normal);
				Assert.AreEqual((suffix == "sow_sickness" ? 8.0 : 7.0) * grade, copy.CastingCosts.Single().Value.Evaluate(f.Base.Actor.Object));
				Assert.AreEqual(suffix == "sow_sickness" ? 60.0 * grade : 0, copy.EffectDurationExpression.Evaluate(f.Base.Actor.Object));
				if (copy.SpellEffects.Single() is DiseaseEffect disease)
				{
					Assert.AreEqual((double)grade, disease.IntensityExpression.Evaluate(f.Base.Actor.Object));
					Assert.AreEqual(1.0, disease.VirulenceExpression.Evaluate(f.Base.Actor.Object));
					Assert.AreSame(f.Base.Traits[1], copy.OpposedTrait);
					Assert.AreEqual(Difficulty.Normal, copy.OpposedDifficulty);
				}
			}
			Assert.IsTrue(XNode.DeepEquals(original, spell.SpellEffects.Single().SaveToXml()));
		}
	}

	[DataTestMethod]
	[DataRow(1)]
	[DataRow(7)]
	public void SowSickness_PaidRouteCreatesOneRealNativeInfectionAtSelectedGrade(int grade)
	{
		var f = new Fixture(); using var time = RuntimeClock.Push(new TestClock(f.Base.Now)); LoadPaid(f, "sow_sickness");
		var result = f.Base.Service.Cast(f.Base.Intent(grade, false));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var infection = (SownSicknessInfection)f.Infections.Single();
		Assert.AreEqual(grade, infection.Grade); Assert.AreEqual((double)grade, infection.Intensity); Assert.AreEqual(1.0, infection.Virulence);
		Assert.AreEqual(grade == 1 ? 92.0 : 50.0, f.Base.Balances[f.Base.Resources[1]]);
		Assert.AreEqual(1, f.Base.Rolls);
		var parent = f.Effects.OfType<MagicSpellParent>().Single();
		Assert.AreEqual(TimeSpan.FromSeconds(60 * grade), f.Base.World.Object.EffectScheduler.RemainingDuration(parent));
	}

	[TestMethod]
	public void SowSickness_PaidExposureCallbackQuarantinesAndKeepsNativeWriteWithoutRefundOrReplay()
	{
		var f = new Fixture(); LoadPaid(f, "sow_sickness");
		f.Base.Body.Setup(x => x.AddInfection(It.IsAny<IInfection>())).Callback<IInfection>(x =>
		{ f.Infections.Add(x); f.ChangeBody(); });
		var result = f.Base.Service.Cast(f.Base.Intent(7, false));
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreEqual(1, f.Infections.Count); Assert.AreEqual(50.0, f.Base.Balances[f.Base.Resources[1]]);
		Assert.IsTrue(f.Base.Store.Operations.Values.Any(x => x.Stage == "NeedsReview"));
		Assert.IsTrue(f.Base.Body.Object.Changed);
		f.Base.Actor.SetupGet(x => x.Body).Returns(f.Base.Body.Object);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Base.Service.Cast(f.Base.Intent(7, false)).Status);
		Assert.AreEqual(1, f.Infections.Count); Assert.AreEqual(50.0, f.Base.Balances[f.Base.Resources[1]]); Assert.AreEqual(1, f.Base.Rolls);
	}

	[TestMethod]
	public void SowSickness_ResistantRecipientUsesDeclaredNativeSaveAndReceivesNoInfection()
	{
		var f = new Fixture(); LoadPaid(f, "sow_sickness");
		var recipient = new Mock<ICharacter>() { DefaultValue = DefaultValue.Mock };
		var body = new Mock<IBody>() { DefaultValue = DefaultValue.Mock };
		recipient.SetupGet(x => x.Id).Returns(101); recipient.SetupGet(x => x.InstanceId).Returns(101);
		recipient.SetupGet(x => x.Gameworld).Returns(f.Base.World.Object); recipient.SetupGet(x => x.Body).Returns(body.Object);
		recipient.SetupGet(x => x.GetObject).Returns(recipient.Object); recipient.SetupGet(x => x.State).Returns(CharacterState.Awake);
		recipient.SetupGet(x => x.Location).Returns(f.Base.Actor.Object.Location);
		body.SetupGet(x => x.Id).Returns(201); body.SetupGet(x => x.Gameworld).Returns(f.Base.World.Object);
		body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		f.Base.Actor.Setup(x => x.TargetActorOrCorpse("recipient", It.IsAny<PerceiveIgnoreFlags>())).Returns(recipient.Object);
		var resistance = new Mock<ICheck>();
		resistance.Setup(x => x.CheckAgainstAllDifficulties(recipient.Object, Difficulty.Normal, f.Base.Traits[1],
			f.Base.Actor.Object, It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck, Outcome.MajorPass)));
		f.Base.World.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Returns(resistance.Object);
		var result = f.Base.Service.Cast(f.Base.Intent(7, false) with { Targets = "recipient" });
		Assert.AreEqual(MagicCastingStatus.Failed, result.Status, result.Message);
		Assert.AreEqual(50.0, f.Base.Balances[f.Base.Resources[1]]); Assert.AreEqual(0, f.Infections.Count);
		body.Verify(x => x.AddInfection(It.IsAny<IInfection>()), Times.Never);
		resistance.Verify(x => x.CheckAgainstAllDifficulties(recipient.Object, Difficulty.Normal, f.Base.Traits[1],
			f.Base.Actor.Object, It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), Times.Once);
	}

	[TestMethod]
	public void PurgeDisease_PaidRemovalCallbackQuarantinesAfterExactNativeDeletionWithoutRefund()
	{
		var f = new Fixture(); LoadPaid(f, "purge_disease");
		var effect = f.LoadOwned(91, "arm.effect.disease"); f.Effects.Add(effect);
		f.Base.Body.Setup(x => x.RemoveInfection(It.IsAny<IInfection>())).Callback<IInfection>(x =>
		{ f.Infections.Remove(x); f.ChangeBody(); });
		var result = f.Base.Service.Cast(f.Base.Intent(7, false));
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreEqual(0, f.Infections.Count); Assert.AreEqual(1, f.Deleted.Count);
		Assert.AreEqual(50.0, f.Base.Balances[f.Base.Resources[1]]);
		f.Base.Actor.SetupGet(x => x.Body).Returns(f.Base.Body.Object);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Base.Service.Cast(f.Base.Intent(7, false)).Status);
		Assert.AreEqual(1, f.Deleted.Count); Assert.AreEqual(50.0, f.Base.Balances[f.Base.Resources[1]]);
	}

	[TestMethod]
	public void PurgeDisease_PaidNoChangeConsumesOneCommitWithoutInventingNativeDeletion()
	{
		var f = new Fixture(); LoadPaid(f, "purge_disease");
		var result = f.Base.Service.Cast(f.Base.Intent(1, false));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(93.0, f.Base.Balances[f.Base.Resources[1]]); Assert.AreEqual(0, f.Deleted.Count);
		Assert.IsFalse((bool)XElement.Parse(f.Base.Store.Operations.Values.Single().Definition).Attribute("applied")!);
	}

	[DataTestMethod]
	[DataRow("IntensityFormula", "0")]
	[DataRow("IntensityFormula", "8")]
	[DataRow("IntensityFormula", "grade/0")]
	[DataRow("VirulenceFormula", "0.5")]
	[DataRow("VirulenceFormula", "1/0")]
	public void SowSickness_InvalidOwnedNumbersRefuseBeforePaymentRollOrExposure(string field, string formula)
	{
		var f = new Fixture(); var row = LoadPaid(f, "sow_sickness").SnapshotModel();
		var definition = XElement.Parse(row.Definition); definition.Element("Effects")!.Elements().Single().Element(field)!.Value = formula;
		row.Definition = definition.ToString(); f.Base.Spells[0] = new MagicSpell(row, f.Base.World.Object);
		var result = f.Base.Service.Cast(f.Base.Intent(7, false));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(100.0, f.Base.Balances[f.Base.Resources[1]]); Assert.AreEqual(0, f.Base.Rolls);
		Assert.AreEqual(0, f.Infections.Count); Assert.AreEqual(0, f.Base.Store.Operations.Count);
	}

	[TestMethod]
	public void PurgeDisease_InvalidOwnedFamilySurvivesCloneAndRefusesBeforePaymentOrDeletion()
	{
		var f = new Fixture(); var row = LoadPaid(f, "purge_disease").SnapshotModel();
		var definition = XElement.Parse(row.Definition); definition.Element("Effects")!.Elements().Single().Element("OwnedFamily")!.Value = "";
		row.Definition = definition.ToString(); var invalid = new MagicSpell(row, f.Base.World.Object); f.Base.Spells[0] = invalid;
		var template = (RemoveDiseaseEffect)invalid.SpellEffects.Single();
		Assert.IsNotNull(template.DefinitionError); Assert.AreEqual("", template.Clone().SaveToXml().Element("OwnedFamily")!.Value);
		f.Effects.Add(f.LoadOwned(91, "arm.effect.disease"));
		Assert.AreEqual(MagicCastingStatus.Refused, f.Base.Service.Cast(f.Base.Intent(7, false)).Status);
		Assert.AreEqual(100.0, f.Base.Balances[f.Base.Resources[1]]); Assert.AreEqual(0, f.Base.Rolls);
		Assert.AreEqual(1, f.Infections.Count); Assert.AreEqual(0, f.Deleted.Count); Assert.AreEqual(0, f.Base.Store.Operations.Count);
	}

	private static MagicSpell LoadPaid(Fixture fixture, string suffix)
	{
		var f = fixture.Base;
		f.Actor.Setup(x => x.TargetActorOrCorpse("self", It.IsAny<PerceiveIgnoreFlags>())).Returns(f.Actor.Object);
		Mock.Get(f.Actor.Object.Location).SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var content = ArmageddonConditionSpellContent.Create(new(5, 2)).Single(x => x.Content.Key == "arm.spell." + suffix).Content;
		f.Expressions.Clear(); var cost = content.CostRow(1); cost.Id = 1; var duration = content.DurationRow(1); duration.Id = 2;
		f.Expressions.Add(new TraitExpression(cost, f.World.Object)); f.Expressions.Add(new TraitExpression(duration, f.World.Object));
		var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2; row.Definition = content.BuildDefinition(10, 1, 0).ToString();
		var spell = new MagicSpell(row, f.World.Object); f.Spells[0] = spell; f.Skills[1] = 100; f.Acquire(7);
		var scheduler = new EffectScheduler(f.World.Object, RuntimeClock.TimeProvider); f.World.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		f.Body.Setup(x => x.AddInfection(It.IsAny<IInfection>())).Callback<IInfection>(fixture.Infections.Add);
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect => { fixture.Effects.Add(effect); effect.InitialEffect(); });
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, time) =>
		{ fixture.Effects.Add(effect); scheduler.AddSchedule(new EffectSchedule(effect, time)); effect.InitialEffect(); });
		return spell;
	}
}
