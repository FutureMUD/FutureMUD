using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems.Inventory;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicCastingTests
{
	[TestMethod]
	public void ChangedGradeProfileVersion_RefusesUntilExplicitStateReconciliation()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades version 2")));
		StringAssert.Contains(f.Service.Preflight(f.Actor.Object, 1, 1, 2, false)!, "version");
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent()).Status);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls);
	}

	[TestMethod]
	public void Grant_PersistenceFailureCannotOpenOrInferSkill_ExplicitRetrySucceeds()
	{
		var f = new MagicCastingFixture(); f.Skills.Remove(1);
		f.Store.BeforeWrite = _ => throw new InvalidOperationException("provider unavailable");
		Assert.IsFalse(f.Service.Grant(f.Staff.Object, f.Actor.Object, 1, 1, "approved").Allowed);
		Assert.IsFalse(f.Skills.ContainsKey(1)); Assert.IsNull(f.Service.Acquisition(f.Actor.Object, 1));
		f.Store.BeforeWrite = null;
		Assert.IsTrue(f.Service.Grant(f.Staff.Object, f.Actor.Object, 1, 1, "approved retry").Allowed);
		Assert.AreEqual(10.0, f.Skills[1]);
	}

	[TestMethod]
	public void Quarantine_PhysicalInputsRemainBlockedAfterTransfer_AndCorruptReceiptCanBeAcknowledged()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		var id = Guid.NewGuid();
		f.Store.Write(new MudSharp.Magic.Casting.CastingOperation(id, 777, 777, 888, 1, 1, 1, 11, "NeedsReview",
			"<Casting version='1'><Item id='123'/></Casting>", f.Now, f.Now));
		Assert.IsNotNull(f.Service.QuarantineReason(f.Actor.Object, itemIds: [123]));
		Assert.IsNull(f.Service.QuarantineReason(f.Actor.Object, spellId: 1, traitId: 1, reserveId: 11));
		f.Store.Write(f.Store.Operation(id)! with { CharacterId = 100, Definition = "invalid xml" });
		Assert.IsNotNull(f.Service.QuarantineReason(f.Actor.Object, spellId: 99));
		Assert.IsTrue(f.Service.ReconcileOperation(f.Staff.Object, f.Actor.Object, id, "Acknowledged damaged receipt; no progress recovered").Allowed);
		Assert.AreEqual(2, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
	}

	[DataTestMethod]
	[DataRow("1/0")]
	[DataRow("sqrt(-1)")]
	[DataRow("undefinedfunction(grade)")]
	public void InvalidNumericalResult_RefusesBeforePaymentInsteadOfFreeCast(string formula)
	{
		var f = new MagicCastingFixture(); f.Acquire();
		Assert.IsTrue(((MudSharp.Body.Traits.TraitExpression)f.Expressions[0]).BuildingCommand(f.Actor.Object, new StringStack("formula " + formula)));
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		StringAssert.Contains(result.Message, "cost/10");
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls);
	}

	[DataTestMethod]
	[DataRow("self")]
	[DataRow("outcome")]
	[DataRow("degrees")]
	public void ScalarBinding_UnsupportedContextVariable_DisablesReadiness(string parameter)
	{
		var f = new MagicCastingFixture();
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades scalar add target 0 boost Bonus " + parameter)));
		Assert.IsFalse(f.Spell.ReadyForGame);
		Assert.IsTrue(f.Spell.GradeConfigurationErrors().Any(x => x.Contains(parameter)));
	}

	[TestMethod]
	public void ProviderFailure_PreservesUnderlyingDiagnosticWithoutRecoveringUnwrittenSample()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		f.Store.BeforeWrite = op =>
		{
			if (op?.Stage == "MasterySampleRecorded")
				throw new InvalidOperationException("Update failed", new InvalidOperationException("native-provider-diagnostic"));
		};
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		StringAssert.Contains(f.Store.Operation(result.OperationId!.Value)!.Diagnostic, "native-provider-diagnostic");
		Assert.IsTrue(f.Service.ReconcileOperation(f.Staff.Object, f.Actor.Object, result.OperationId.Value, "audited").Allowed);
		Assert.AreEqual(2, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		Assert.AreEqual(1, f.Samples);
	}

	[TestMethod]
	public void Queries_EmptyReserveAndInactiveCapability_RetainKnowledgeWithoutMutation()
	{
		var f = new MagicCastingFixture(); f.Acquire(); f.Balances[f.Resources[1]] = 0;
		var writes = f.Store.Writes;
		Assert.AreEqual(2, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		Assert.IsTrue(f.Service.Routes(f.Actor.Object, 1).Any(x => x.Available));
		Assert.IsFalse(f.Service.Quote(f.Intent()).Allowed);
		f.ActiveCapabilities.Clear();
		Assert.IsNotNull(f.Service.Acquisition(f.Actor.Object, 1));
		Assert.IsTrue(f.Service.Routes(f.Actor.Object, 1).All(x => !x.Available));
		Assert.AreEqual(writes, f.Store.Writes); Assert.AreEqual(0, f.Flushes); Assert.AreEqual(0, f.Rolls);
	}

	[TestMethod]
	public void GradeResolution_ControlledTwoAndSkillFortyTwo_OnlyExplicitNextGradeCostsTwentyTwoPointFive()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		foreach (var grade in new[] { 1, 2 }) Assert.IsTrue(f.Service.Quote(f.Intent(grade, false)).Allowed);
		var quote = f.Service.Quote(f.Intent());
		Assert.IsTrue(quote.Allowed, quote.Reason);
		Assert.AreEqual(22.5, quote.Invocation!.Costs.Single().Amount);
		Assert.AreEqual(11L, quote.Invocation.Costs.Single().ResourceId);
		Assert.AreEqual(SpellPower.Weak, quote.Invocation.Power);
		foreach (var grade in new[] { 4, 7 }) Assert.IsFalse(f.Service.Quote(f.Intent(grade)).Allowed);
		Assert.IsFalse(f.Service.Quote(f.Intent(3, false)).Allowed);
		f.Skills[1] = 39;
		Assert.IsFalse(f.Service.Quote(f.Intent()).Allowed);
		Assert.AreEqual(0, f.Rolls);
	}

	[TestMethod]
	public void Cast_FailedNativeCheck_PaysOnceAndConsumesSharedDeadlines()
	{
		var f = new MagicCastingFixture { Outcome = Outcome.Fail }; f.Acquire();
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Failed, result.Status, result.Message + string.Join(";", f.Messages));
		Assert.AreEqual(77.5, f.Balances[f.Resources[1]]); Assert.AreEqual(100.0, f.Balances[f.Resources[0]]);
		Assert.AreEqual(1, f.Rolls); Assert.AreEqual(1, f.SkillUses); Assert.AreEqual(0, f.Samples);
		Assert.AreEqual(2, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		Assert.AreEqual(f.Now.AddSeconds(600), f.Service.Acquisition(f.Actor.Object, 1)!.NextMasteryUtc);
		Assert.AreEqual(f.Now.AddSeconds(60), f.Store.Opportunity(100, 1)!.NextUtc);
		f.Actor.Verify(x => x.AddEffect(It.IsAny<MagicSpellLockout>(), TimeSpan.FromSeconds(5)), Times.Once);
		f.Restart(); f.Service.Cast(f.Intent());
		Assert.AreEqual(55.0, f.Balances[f.Resources[1]]); Assert.AreEqual(1, f.SkillUses);
	}

	[TestMethod]
	public void Cast_AppliedNextGrade_SamplesOnceAndAdvancesSharedAcquisition()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message + string.Join(";", f.Messages));
		Assert.AreEqual(3, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		Assert.AreEqual(1, f.Samples); Assert.AreEqual(1, f.SkillUses);
		f.Restart();
		Assert.IsNull(f.Service.Preflight(f.Actor.Object, 2, 1, 3, false));
		Assert.AreEqual("Completed", f.Store.Operation(result.OperationId!.Value)!.Stage);
	}

	[TestMethod]
	public void NumericalCopies_ParallelRoutesAndGrades_DoNotContaminateCatalogueOrEachOther()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		Parallel.For(0, 24, i =>
		{
			var grade = i % 2 == 0 ? 1 : 3; var trait = f.Traits[i % 2];
			var copy = f.Spell.CastingCopy(f.Actor.Object, trait, grade, SpellPower.Weak, Difficulty.Normal);
			Assert.AreEqual(-grade, ((TraitBoostEffect)copy.SpellEffects.Single()).Bonus);
			Assert.AreEqual(f.Skills[trait.Id] + 10 * grade, copy.EffectDurationExpression.Evaluate(f.Actor.Object));
			Assert.AreEqual(5.0 * grade, copy.CastingCosts.Single().Value.Evaluate(f.Actor.Object));
			Assert.AreEqual(1L, copy.School.Id);
		});
		Assert.AreEqual(0.0, ((TraitBoostEffect)f.Spell.SpellEffects.Single()).Bonus);
		Assert.AreEqual(1L, f.Spell.CastingTrait.Id);
		var reload = new MagicSpell(f.Spell.SnapshotModel(), f.World.Object);
		Assert.AreEqual(0.0, ((TraitBoostEffect)reload.SpellEffects.Single()).Bonus);
		Assert.AreEqual("-grade", reload.GradeProfile!.ScalarBindings.Single().Expression);
	}

	[DataTestMethod]
	[DataRow("target 7 boost Bonus -grade")]
	[DataRow("target 0 damage Bonus -grade")]
	[DataRow("target 0 boost Arbitrary -grade")]
	public void ScalarBinding_InvalidField_DisablesReadinessBeforePayment(string binding)
	{
		var f = new MagicCastingFixture(); f.Acquire();
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades scalar add " + binding)));
		Assert.IsFalse(f.Spell.ReadyForGame);
		Assert.IsFalse(f.Service.Quote(f.Intent()).Allowed);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent()).Status);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls);
	}

	[TestMethod]
	public void Quote_FixedAndReboundSameResource_AggregatesBeforeAffordability()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		var model = f.Spell.SnapshotModel(); var xml = XElement.Parse(model.Definition);
		xml.Element("Costs")!.Add(new XElement("Cost", new XAttribute("resource", 11), new XAttribute("expression", 1)));
		model.Definition = xml.ToString(); f.Spells[0] = new MagicSpell(model, f.World.Object);
		var quote = f.Service.Quote(f.Intent());
		Assert.IsTrue(quote.Allowed, quote.Reason); Assert.AreEqual(37.5, quote.Invocation!.Costs.Single().Amount);
		f.Balances[f.Resources[1]] = 30;
		Assert.IsFalse(f.Service.Quote(f.Intent()).Allowed);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent()).Status);
		Assert.AreEqual(30.0, f.Balances[f.Resources[1]]);
	}

	[TestMethod]
	public void Cast_MissingActualComponentPlan_DoesNotCommitOrCheck()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		var template = new Mock<IInventoryPlanTemplate>(); var plan = new Mock<IInventoryPlan>();
		template.Setup(x => x.CreatePlan(f.Actor.Object)).Returns(plan.Object);
		template.Setup(x => x.SaveToXml()).Returns(XElement.Parse("<Plan><Phase/></Plan>"));
		plan.Setup(x => x.PlanIsFeasible()).Returns(InventoryPlanFeasibility.NotFeasibleMissingItems);
		f.Spell.InventoryPlanTemplate = template.Object;
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message + string.Join(";", f.Messages));
		Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(100.0, f.Balances[f.Resources[1]]);
	}

	[DataTestMethod]
	[DataRow("Paying")]
	[DataRow("PaymentMutated")]
	[DataRow("Committed")]
	[DataRow("EffectsExecuted")]
	[DataRow("ImprovingSkill")]
	[DataRow("MasterySampledBeforeWrite")]
	[DataRow("MasterySampleRecorded")]
	public void InterruptedPaidCast_RestartQuarantinesAlternateRoute_RecoveryNeverReplays(string stage)
	{
		var f = new MagicCastingFixture(); f.Acquire();
		f.Checkpoint = s => { if (s == stage) throw new InvalidOperationException("injected crash at " + s); };
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message + string.Join(";", f.Messages));
		var balance = f.Balances[f.Resources[1]]; var rolls = f.Rolls; var samples = f.Samples; var uses = f.SkillUses;
		f.Restart(); f.Checkpoint = null;
		Assert.IsFalse(f.Service.Quote(f.Intent(capability: 2)).Allowed);
		Assert.IsNotNull(f.Service.QuarantineReason(f.Actor.Object, traitId: 1));
		Assert.IsNotNull(f.Service.QuarantineReason(f.Actor.Object, reserveId: 11));
		Assert.IsNull(f.Service.QuarantineReason(f.Actor.Object, traitId: 3, reserveId: 10));
		var recovery = f.Service.ReconcileOperation(f.Staff.Object, f.Actor.Object, result.OperationId!.Value, "fixture audited");
		Assert.IsTrue(recovery.Allowed, recovery.Message);
		Assert.AreEqual(stage == "MasterySampleRecorded" ? 3 : 2, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		Assert.AreEqual(balance, f.Balances[f.Resources[1]]); Assert.AreEqual(rolls, f.Rolls);
		Assert.AreEqual(samples, f.Samples); Assert.AreEqual(uses, f.SkillUses);
	}

	[TestMethod]
	public void FailedMasteryWrite_DoesNotRecoverAnUncommittedSample()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		f.Store.BeforeWrite = op => { if (op?.Stage == "MasterySampleRecorded") throw new InvalidOperationException("provider rollback"); };
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message + string.Join(";", f.Messages)); f.Restart();
		Assert.IsTrue(f.Service.ReconcileOperation(f.Staff.Object, f.Actor.Object, result.OperationId!.Value, "inspected").Allowed);
		Assert.AreEqual(2, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade); Assert.AreEqual(1, f.Samples);
	}

	[TestMethod]
	public void DirectTrigger_ConfiguredKnownProgIsNotLegacy_IndependentLegacyGrantRestoresRoute()
	{
		var f = new MagicCastingFixture();
		Assert.IsFalse(f.Spell.HasLegacyRoute(f.Actor.Object));
		((ICastMagicTrigger)f.Spell.Trigger).DoTriggerCast(f.Actor.Object, new StringStack("Standard"));
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(100.0, f.Balances[f.Resources[0]]);
		var legacy = new Mock<IMagicCapability>(); legacy.SetupGet(x => x.School).Returns(f.School); legacy.SetupGet(x => x.Id).Returns(30);
		f.ActiveCapabilities.Add(legacy.Object);
		Assert.IsTrue(f.Spell.HasLegacyRoute(f.Actor.Object));
		// An independent grant keeps ordinary native pricing and does not create configured knowledge or opportunities.
		((MudSharp.Body.Traits.TraitExpression)f.Expressions[0]).BuildingCommand(f.Actor.Object, new StringStack("formula 10"));
		((MudSharp.Body.Traits.TraitExpression)f.Expressions[1]).BuildingCommand(f.Actor.Object, new StringStack("formula 60"));
		((ICastMagicTrigger)f.Spell.Trigger).DoTriggerCast(f.Actor.Object, new StringStack("Standard"));
		Assert.AreEqual(1, f.Rolls); Assert.AreEqual(90.0, f.Balances[f.Resources[0]]);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]);
		Assert.IsNull(f.Service.Acquisition(f.Actor.Object, 1)); Assert.AreEqual(0, f.Store.Opportunities.Count);
	}

	[TestMethod]
	public void Enrolment_ExplicitStaffOnlyAndIdempotent_OpensSkillWithoutTouchingReserve()
	{
		var f = new MagicCastingFixture(); f.Skills.Remove(1);
		Assert.IsFalse(f.Service.Enrol(f.Actor.Object, f.Actor.Object, 1, "unauthorised").Allowed);
		Assert.IsFalse(f.Service.Grant(f.Staff.Object, f.Actor.Object, 1, 1, "").Allowed);
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "approved enrolment").Changed);
		Assert.AreEqual(10.0, f.Skills[1]); Assert.AreEqual(1, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		f.Restart(); Assert.IsFalse(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "again").Changed);
		Assert.AreEqual(1, f.Store.Acquired.Count); Assert.AreEqual(1, f.Store.Enrolments.Count);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]);
	}

	[TestMethod]
	public void Policy_InvalidIdentifiersDuplicatesAndCycles_ReportsErrorsWithoutMutation()
	{
		var f = new MagicCastingFixture();
		var policy = XElement.Parse(f.Earth.SaveToXml()).Element("Casting")!;
		policy.SetAttributeValue("trait", 9876);
		var a = policy.Element("Admission")!;
		a.Add(new XElement("Prerequisite", new XAttribute("key", Guid.NewGuid()), new XAttribute("spell", 1), new XAttribute("grade", 1), new XAttribute("proficiency", 20)));
		var invalid = f.NewCapability(7, 1, 11, true, policy);
		Assert.IsTrue(invalid.CastingConfigurationErrors().Any(x => x.Contains("9876")));
		Assert.IsTrue(invalid.CastingConfigurationErrors().Any(x => x.Contains("cycle")));
		policy.Add(new XElement(a)); var duplicate = f.NewCapability(8, 1, 11, true, policy);
		Assert.IsTrue(duplicate.CastingConfigurationErrors().Any(x => x.Contains("Duplicate")));
	}
}
