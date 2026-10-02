using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.CommunicationStrategies;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicCastingPracticeTests
{
	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void OriginReceipt_CompletedPracticeOrManifest_CannotBeOverwrittenByReplayedPractice(bool priorManifest)
	{
		var (f, effects) = Setup(); var id = Guid.NewGuid();
		if (priorManifest)
			Assert.AreEqual(MagicCastingStatus.Succeeded, f.Service.Cast(f.Intent(1, false) with { OriginId = id }).Status);
		else
		{
			Assert.AreEqual(MagicCastingStatus.Started, f.Service.Cast(Intent(f) with { OriginId = id }).Status);
			f.Now += TimeSpan.FromSeconds(30); effects.OfType<MagicPracticeAction>().Single().ExpireEffect();
		}
		var terminal = f.Store.Operation(id)!; var balance = f.Balances[f.Resources[1]]; var writes = f.Store.Writes; var rolls = f.Rolls;
		f.Restart();
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(Intent(f) with { OriginId = id }).Status);
		Assert.AreEqual(terminal, f.Store.Operation(id)); Assert.AreEqual(balance, f.Balances[f.Resources[1]]);
		Assert.AreEqual(writes, f.Store.Writes); Assert.AreEqual(rolls, f.Rolls); Assert.IsFalse(effects.OfType<MagicPracticeAction>().Any());
	}
	private static (MagicCastingFixture Fixture, List<IEffect> Effects) Setup()
	{
		var f = new MagicCastingFixture(); var effects = new List<IEffect>();
		f.Actor.SetupGet(x => x.Combat).Returns((MudSharp.Combat.ICombat)null!);
		f.Actor.SetupGet(x => x.Movement).Returns((MudSharp.Movement.IMovement)null!);
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((e, _) => effects.Add(e));
		f.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((e, fire) =>
		{
			if (effects.Remove(e) && fire) e.RemovalEffect();
		});
		f.Actor.Setup(x => x.CombinedEffectsOfType<IActionEffect>()).Returns(() => effects.OfType<IActionEffect>());
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades practice fixture")));
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, f.Earth.Id, "Practice fixture enrolment").Allowed);
		return (f, effects);
	}

	private static MagicCastingIntent Intent(MagicCastingFixture f, int grade = 2, bool overreach = true) =>
		new(f.Actor.Object, f.Earth.Id, f.Spell.Id, grade, overreach, "", MagicCastingMode.Practice);

	[TestMethod]
	public void Quote_PracticeWithoutTarget_IsPureAndUsesFullSharedCost()
	{
		var (f, effects) = Setup(); var writes = f.Store.Writes;
		var quote = f.Service.Quote(Intent(f));
		Assert.IsTrue(quote.Allowed, quote.Reason); Assert.AreEqual(MagicCastingMode.Practice, quote.Invocation!.Mode);
		Assert.AreEqual(15.0, quote.Invocation.Costs.Single().Amount);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]); Assert.AreEqual(writes, f.Store.Writes);
		Assert.AreEqual(0, effects.Count); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples);
		Assert.IsFalse(f.Service.Quote(Intent(f) with { Targets = "self" }).Allowed);
	}

	[TestMethod]
	public void Practice_PaidCompletion_UsesOneTargetFreeCheckAndNeverAppliesSpellEffects()
	{
		var (f, effects) = Setup();
		var forbidden = new Mock<IMagicSpellEffectTemplate>();
		forbidden.Setup(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), It.IsAny<MudSharp.Framework.IPerceivable>(),
			It.IsAny<OpposedOutcomeDegree>(), It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(), It.IsAny<SpellAdditionalParameter[]>()))
			.Throws(new InvalidOperationException("Practice applied a forbidden caster effect."));
		((List<IMagicSpellEffectTemplate>)f.Spell.CasterSpellEffects).Add(forbidden.Object);
		var result = f.Service.Cast(Intent(f)); Assert.AreEqual(MagicCastingStatus.Started, result.Status, result.Message);
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses);
		var action = effects.OfType<MagicPracticeAction>().Single(); Assert.IsFalse(action.SavingEffect);
		f.Now += TimeSpan.FromSeconds(30); action.ExpireEffect(); action.ExpireEffect();
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]); Assert.AreEqual(1, f.Rolls); Assert.AreEqual(1, f.SkillUses); Assert.AreEqual(1, f.Samples);
		Assert.AreEqual(2, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		Assert.AreEqual("Completed", f.Store.Operation(result.OperationId!.Value)!.Stage); Assert.AreEqual(0, effects.Count);
		forbidden.Verify(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), It.IsAny<MudSharp.Framework.IPerceivable>(),
			It.IsAny<OpposedOutcomeDegree>(), It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(), It.IsAny<SpellAdditionalParameter[]>()), Times.Never);
		f.World.Verify(x => x.GetCheck(CheckType.ResistMagicSpellCheck), Times.Never);
		f.Check.Verify(x => x.CheckAgainstAllDifficulties(f.Actor.Object, It.IsAny<Difficulty>(), f.Traits[0], null,
			It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), Times.Once);
	}

	[DataTestMethod]
	[DataRow("quit")]
	[DataRow("death")]
	[DataRow("state")]
	[DataRow("movement")]
	[DataRow("body")]
	[DataRow("stop")]
	[DataRow("focus")]
	[DataRow("deleted")]
	[DataRow("melee")]
	public void Practice_InterruptedAction_RetainsPaymentAndDeadlinesWithoutProgressOrReplay(string kind)
	{
		var (f, effects) = Setup(); var started = f.Service.Cast(Intent(f));
		Assert.AreEqual(MagicCastingStatus.Started, started.Status, started.Message);
		var action = effects.OfType<MagicPracticeAction>().Single();
		switch (kind)
		{
			case "quit": f.Actor.Raise(x => x.OnQuit += null, f.Actor.Object); break;
			case "death": f.Actor.Raise(x => x.OnDeath += null, f.Actor.Object); break;
			case "state": f.Actor.Raise(x => x.OnStateChanged += null, f.Actor.Object); break;
			case "movement": f.Actor.Raise(x => x.OnStartMove += null, f.Actor.Object, null!); break;
			case "body": f.Actor.Raise(x => x.CurrentBodyChanged += null, f.Actor.Object, f.Body.Object, new Mock<IBody>().Object); break;
			case "stop": f.Actor.Object.RemoveEffect(action, true); break;
			case "focus": f.Service.InterruptPractice(f.Actor.Object, "Focus changed."); break;
			case "deleted": f.Actor.Raise(x => x.OnDeleted += null, f.Actor.Object); break;
			case "melee": f.Actor.Raise(x => x.OnEngagedInMelee += null, f.Actor.Object); break;
		}
		f.Now += TimeSpan.FromSeconds(30); action.ExpireEffect(); action.RemovalEffect();
		Assert.AreEqual("PracticeInterrupted", f.Store.Operation(started.OperationId!.Value)!.Stage);
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples);
		Assert.AreEqual(1, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		Assert.AreEqual(new DateTime(2026, 9, 27, 1, 10, 0, DateTimeKind.Utc), f.Service.Acquisition(f.Actor.Object, 1)!.NextMasteryUtc);
		Assert.AreEqual(0, f.Store.Unresolved().Count); Assert.AreEqual(0, effects.Count);
	}

	[DataTestMethod]
	[DataRow("speech")]
	[DataRow("hand")]
	[DataRow("capability")]
	[DataRow("configuration")]
	public void Practice_LostLiveRequirements_CannotAwardProgressAtCompletion(string kind)
	{
		var (f, effects) = Setup(); var start = f.Service.Cast(Intent(f)); var action = effects.OfType<MagicPracticeAction>().Single();
		if (kind == "speech") f.Body.Setup(x => x.Communications.CanVocalise(f.Body.Object)).Returns(false);
		if (kind == "hand") f.Body.SetupGet(x => x.FunctioningFreeHands).Returns([]);
		if (kind == "capability") f.ActiveCapabilities.Remove(f.Earth);
		if (kind == "configuration") Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades practice energy 2")));
		f.Now += TimeSpan.FromSeconds(30); action.ExpireEffect();
		Assert.AreEqual("PracticeInterrupted", f.Store.Operation(start.OperationId!.Value)!.Stage);
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples);
	}

	[TestMethod]
	public void Practice_PrematureExpiry_ConsumesNoProgressAndCannotResetSharedOpportunities()
	{
		var (f, effects) = Setup(); var start = f.Service.Cast(Intent(f));
		f.Now += TimeSpan.FromSeconds(29); effects.OfType<MagicPracticeAction>().Single().ExpireEffect();
		Assert.AreEqual("PracticeInterrupted", f.Store.Operation(start.OperationId!.Value)!.Stage);
		Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Service.Cast(f.Intent(2, true)).Status);
		Assert.AreEqual(1, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples); Assert.AreEqual(70.0, f.Balances[f.Resources[1]]);
	}

	[TestMethod]
	public void Practice_Invalidation_ImmediatelyCancelsWithoutWaitingForExpiry()
	{
		var (f, effects) = Setup(); var start = f.Service.Cast(Intent(f));
		f.ActiveCapabilities.Remove(f.Earth); f.Service.Reconcile(f.Actor.Object);
		Assert.AreEqual("PracticeInterrupted", f.Store.Operation(start.OperationId!.Value)!.Stage);
		Assert.AreEqual(0, effects.Count); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses);
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]);
	}

	[TestMethod]
	public void Practice_ActionAttachmentFault_ReleasesCallbacksAndQuarantinesPayment()
	{
		var (f, effects) = Setup();
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((e, _) =>
		{
			effects.Add(e); throw new InvalidOperationException("Scheduler attachment failed.");
		});
		var start = f.Service.Cast(Intent(f));
		Assert.AreEqual(MagicCastingStatus.NeedsReview, start.Status); Assert.AreEqual(0, effects.Count);
		f.Actor.Raise(x => x.OnQuit += null, f.Actor.Object); f.Service.InterruptPractice(f.Actor.Object, "Focus changed.");
		Assert.AreEqual("NeedsReview", f.Store.Operation(start.OperationId!.Value)!.Stage);
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Samples);
	}

	[TestMethod]
	public void Practice_InterruptedDuringCheck_CannotAwardSkillOrMastery()
	{
		var (f, effects) = Setup(); var start = f.Service.Cast(Intent(f));
		f.Checkpoint = stage => { if (stage == "PracticeChecked") f.Service.InterruptPractice(f.Actor.Object, "Lost focus."); };
		f.Now += TimeSpan.FromSeconds(30); effects.OfType<MagicPracticeAction>().Single().ExpireEffect();
		Assert.AreEqual("PracticeInterrupted", f.Store.Operation(start.OperationId!.Value)!.Stage);
		Assert.AreEqual(1, f.Rolls); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples);
		Assert.AreEqual(1, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
	}

	[TestMethod]
	public void Practice_ConfiguredPhysicalPermissions_AreUsedWithoutWeakeningManifestation()
	{
		var (f, effects) = Setup();
		foreach (var option in new[] { "speech false", "hand false", "movement true", "duration 10", "energy 2" })
			Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades practice " + option)));
		f.Body.Setup(x => x.Communications.CanVocalise(f.Body.Object)).Returns(false);
		f.Body.SetupGet(x => x.FunctioningFreeHands).Returns([]);
		Assert.IsFalse(f.Service.Quote(f.Intent()).Allowed);
		var start = f.Service.Cast(Intent(f)); Assert.AreEqual(MagicCastingStatus.Started, start.Status, start.Message);
		Assert.AreEqual(70.0, f.Balances[f.Resources[1]]);
		f.Actor.Raise(x => x.OnStartMove += null, f.Actor.Object, null!);
		Assert.AreEqual(1, effects.Count);
		f.Now += TimeSpan.FromSeconds(10); effects.OfType<MagicPracticeAction>().Single().ExpireEffect();
		Assert.AreEqual("Completed", f.Store.Operation(start.OperationId!.Value)!.Stage);
		Assert.AreEqual(1, f.SkillUses); Assert.AreEqual(1, f.Samples);
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void Practice_AndManifestation_ShareSkillAndMasteryOpportunities(bool practiceFirst)
	{
		var (f, effects) = Setup();
		if (practiceFirst)
		{
			Assert.AreEqual(MagicCastingStatus.Started, f.Service.Cast(Intent(f)).Status);
			f.Now += TimeSpan.FromSeconds(30); effects.OfType<MagicPracticeAction>().Single().ExpireEffect();
			Assert.AreEqual(MagicCastingStatus.Succeeded, f.Service.Cast(f.Intent(3, true)).Status);
		}
		else
		{
			Assert.AreEqual(MagicCastingStatus.Succeeded, f.Service.Cast(f.Intent(2, true)).Status);
			Assert.AreEqual(MagicCastingStatus.Started, f.Service.Cast(Intent(f, 3)).Status);
			f.Now += TimeSpan.FromSeconds(30); effects.OfType<MagicPracticeAction>().Single().ExpireEffect();
		}
		Assert.AreEqual(2, f.Rolls); Assert.AreEqual(1, f.SkillUses); Assert.AreEqual(1, f.Samples);
		Assert.AreEqual(2, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
	}

	[TestMethod]
	public void Practice_Restart_QuarantinesPersistedPrepaidActionWithoutResumeRefundOrReroll()
	{
		var (f, effects) = Setup(); var start = f.Service.Cast(Intent(f));
		var receipt = XElement.Parse(f.Store.Operation(start.OperationId!.Value)!.Definition);
		Assert.AreEqual(f.Now + TimeSpan.FromSeconds(30), (DateTime)receipt.Attribute("deadlineUtc")!);
		effects.Clear(); f.Restart(); f.Now += TimeSpan.FromHours(1);
		Assert.IsFalse(f.Service.Quote(Intent(f)).Allowed); Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(Intent(f)).Status);
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples);
		Assert.IsTrue(f.Service.ReconcileOperation(f.Staff.Object, f.Actor.Object, start.OperationId.Value, "Interrupted process; no proven sample").Allowed);
		Assert.AreEqual(1, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade); Assert.AreEqual(85.0, f.Balances[f.Resources[1]]);
	}

	[TestMethod]
	public void Practice_StaffReconciliation_RefusesRunningWorkAndCannotRecoverInterruptedProgress()
	{
		var (f, effects) = Setup(); var start = f.Service.Cast(Intent(f));
		var id = start.OperationId!.Value; var action = effects.OfType<MagicPracticeAction>().Single();
		Assert.IsFalse(f.Service.ReconcileOperation(f.Staff.Object, f.Actor.Object, id, "Live work").Allowed);
		Assert.AreEqual("Practising", f.Store.Operation(id)!.Stage);
		f.Actor.Object.RemoveEffect(action, true);
		var writes = f.Store.Writes;
		Assert.IsFalse(f.Service.ReconcileOperation(f.Staff.Object, f.Actor.Object, id, "Interrupted").Changed);
		Assert.AreEqual(writes, f.Store.Writes);
		f.Now += TimeSpan.FromSeconds(30); action.ExpireEffect(); action.RemovalEffect();
		Assert.AreEqual("PracticeInterrupted", f.Store.Operation(id)!.Stage);
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Practice_StaleAction_CannotOverwriteDurableFinalisation(bool cancel)
	{
		var (f, effects) = Setup(); var start = f.Service.Cast(Intent(f)); var id = start.OperationId!.Value;
		var action = effects.OfType<MagicPracticeAction>().Single();
		f.Store.Write(f.Store.Operation(id)! with { Stage = "Reconciled", Diagnostic = "Externally finalised" });
		f.Now += TimeSpan.FromSeconds(30);
		if (cancel) f.Actor.Object.RemoveEffect(action, true); else action.ExpireEffect();
		action.ExpireEffect(); action.RemovalEffect();
		Assert.AreEqual("Reconciled", f.Store.Operation(id)!.Stage);
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, effects.Count);
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples);
	}

	[DataTestMethod]
	[DataRow("BeforePayment", false, 100.0)]
	[DataRow("Paying", false, 100.0)]
	[DataRow("PaymentMutated", false, 85.0)]
	[DataRow("Committed", false, 85.0)]
	[DataRow("Practising", false, 85.0)]
	[DataRow("CheckingPractice", true, 85.0)]
	[DataRow("ImprovingSkill", true, 85.0)]
	[DataRow("MasterySampledBeforeWrite", true, 85.0)]
	public void Practice_FaultBoundary_QuarantinesUncertainWorkWithoutAutomaticReplay(string stage, bool afterStart, double balance)
	{
		var (f, effects) = Setup(); f.Checkpoint = s => { if (s == stage) throw new InvalidOperationException("Practice boundary fault"); };
		var start = f.Service.Cast(Intent(f));
		if (afterStart)
		{
			Assert.AreEqual(MagicCastingStatus.Started, start.Status); f.Now += TimeSpan.FromSeconds(30);
			effects.OfType<MagicPracticeAction>().Single().ExpireEffect();
			Assert.AreEqual("NeedsReview", f.Store.Operation(start.OperationId!.Value)!.Stage);
		}
		else Assert.AreEqual(stage == "BeforePayment" ? MagicCastingStatus.Refused : MagicCastingStatus.NeedsReview, start.Status);
		Assert.AreEqual(balance, f.Balances[f.Resources[1]]);
		Assert.AreEqual(1, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		f.Checkpoint = null; f.Restart();
		if (stage != "BeforePayment") Assert.IsFalse(f.Service.Quote(Intent(f)).Allowed);
		Assert.AreEqual(balance, f.Balances[f.Resources[1]]);
	}

	[DataTestMethod]
	[DataRow("enabled false")]
	[DataRow("max 1")]
	[DataRow("duration 0")]
	[DataRow("energy NaN")]
	[DataRow("difficulty impossible")]
	public void Practice_BuilderRestrictions_RefuseBeforeAccounting(string policy)
	{
		var (f, _) = Setup(); Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades practice " + policy)));
		Assert.IsFalse(f.Service.Quote(Intent(f)).Allowed); Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(Intent(f)).Status);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Rolls);
	}

	[TestMethod]
	public void Practice_ExplicitPlan_MissingComponentsRefusesAndDoesNotUseManifestPlan()
	{
		var (f, _) = Setup(); var template = new Mock<IInventoryPlanTemplate>(); var plan = new Mock<IInventoryPlan>();
		plan.Setup(x => x.PlanIsFeasible()).Returns(InventoryPlanFeasibility.NotFeasibleMissingItems);
		template.Setup(x => x.CreatePlan(f.Actor.Object)).Returns(plan.Object);
		typeof(MagicSpell).GetProperty(nameof(MagicSpell.PracticeInventoryPlanTemplate))!.SetValue(f.Spell, template.Object);
		Assert.IsFalse(f.Service.Quote(Intent(f)).Allowed); Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(Intent(f)).Status);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]); plan.Verify(x => x.ExecuteWholePlan(), Times.Never);
	}

	[TestMethod]
	public void Practice_PolicyRoundTripAndGradeSeven_KeepExplicitPowerMappingAndOrdinaryCap()
	{
		var (f, _) = Setup();
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry skill 1 30 60 relative")));
		f.Skills[1] = 57;
		f.Store.Write(acquired: f.Service.Acquisition(f.Actor.Object, 1)! with { ControlledGrade = 6 });
		var quote = f.Service.Quote(Intent(f, 7)); Assert.IsTrue(quote.Allowed, quote.Reason);
		Assert.AreEqual(SpellPower.ExtremelyStrong, quote.Invocation!.Power);
		Assert.IsNull(f.Spell.GradeProfile!.Practice!.MaximumGrade);
		var method = typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;
		var reload = new MagicSpell((MudSharp.Models.MagicSpell)method.Invoke(f.Spell, null)!, f.World.Object);
		Assert.AreEqual(f.Spell.GradeProfile.Practice, reload.GradeProfile!.Practice);
		Assert.IsNotNull(reload.PracticeInventoryPlanTemplate); Assert.AreEqual(0, reload.GradeConfigurationErrors().Count);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Practice_ExplicitMaterialPlan_ExecutesOnceAndFinalisesAfterCompletionOrInterruption(bool cancel)
	{
		var (f, effects) = Setup(); var xml = f.Spell.PracticeInventoryPlanTemplate!.SaveToXml();
		var item = new Mock<IGameItem>(); item.SetupGet(x => x.Id).Returns(123);
		var plan = new Mock<IInventoryPlan>(); var template = new Mock<IInventoryPlanTemplate>();
		plan.Setup(x => x.PlanIsFeasible()).Returns(InventoryPlanFeasibility.Feasible);
		plan.Setup(x => x.PeekPlanResults()).Returns([new InventoryPlanActionResult { PrimaryTarget = item.Object }]);
		template.Setup(x => x.CreatePlan(f.Actor.Object)).Returns(plan.Object); template.Setup(x => x.SaveToXml()).Returns(() => new XElement(xml));
		typeof(MagicSpell).GetProperty(nameof(MagicSpell.PracticeInventoryPlanTemplate))!.SetValue(f.Spell, template.Object);
		var start = f.Service.Cast(Intent(f)); Assert.AreEqual(MagicCastingStatus.Started, start.Status, start.Message);
		Assert.AreEqual(123L, (long)XElement.Parse(f.Store.Operation(start.OperationId!.Value)!.Definition).Element("Item")!.Attribute("id")!);
		plan.Verify(x => x.ExecuteWholePlan(), Times.Once); plan.Verify(x => x.FinalisePlan(), Times.Never);
		var action = effects.OfType<MagicPracticeAction>().Single();
		if (cancel) f.Actor.Object.RemoveEffect(action, true); else { f.Now += TimeSpan.FromSeconds(30); action.ExpireEffect(); }
		action.ExpireEffect(); plan.Verify(x => x.ExecuteWholePlan(), Times.Once); plan.Verify(x => x.FinalisePlan(), Times.Once);
	}

	[TestMethod]
	public void Practice_PlanBuilderRejectedInput_PreservesAuthoredPlan()
	{
		var (f, _) = Setup();
		f.Spell.PracticeInventoryPlanTemplate!.FirstPhase.AddAction(new InventoryPlanActionConsume(f.World.Object, 1, 0, 0, _ => true, _ => true));
		var before = f.Spell.PracticeInventoryPlanTemplate.SaveToXml().ToString();
		Assert.IsFalse(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades practice plan remove 1 trailing")));
		Assert.AreEqual(before, f.Spell.PracticeInventoryPlanTemplate.SaveToXml().ToString());
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades practice plan remove 1")));
		Assert.AreEqual(0, f.Spell.PracticeInventoryPlanTemplate.FirstPhase.Actions.Count());
	}

	[TestMethod]
	public void Practice_FailedPaidCheck_CanUseNativeImproverWithoutMasteryOrRefund()
	{
		var (f, effects) = Setup(); f.Outcome = Outcome.Fail;
		var start = f.Service.Cast(Intent(f)); Assert.AreEqual(MagicCastingStatus.Started, start.Status, start.Message);
		f.Now += TimeSpan.FromSeconds(30); effects.OfType<MagicPracticeAction>().Single().ExpireEffect();
		Assert.AreEqual("Completed", f.Store.Operation(start.OperationId!.Value)!.Stage);
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]); Assert.AreEqual(1, f.Rolls); Assert.AreEqual(1, f.SkillUses);
		Assert.AreEqual(0, f.Samples); Assert.AreEqual(1, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
	}

	[TestMethod]
	public void Practice_CancellationCleanup_RefusesReentrantStaffFinalisation()
	{
		var (f, effects) = Setup(); var xml = f.Spell.PracticeInventoryPlanTemplate!.SaveToXml();
		var plan = new Mock<IInventoryPlan>(); var template = new Mock<IInventoryPlanTemplate>();
		plan.Setup(x => x.PlanIsFeasible()).Returns(InventoryPlanFeasibility.Feasible);
		plan.Setup(x => x.PeekPlanResults()).Returns([]);
		template.Setup(x => x.CreatePlan(f.Actor.Object)).Returns(plan.Object); template.Setup(x => x.SaveToXml()).Returns(() => new XElement(xml));
		typeof(MagicSpell).GetProperty(nameof(MagicSpell.PracticeInventoryPlanTemplate))!.SetValue(f.Spell, template.Object);
		var start = f.Service.Cast(Intent(f)); Assert.AreEqual(MagicCastingStatus.Started, start.Status, start.Message);
		MagicCastingGrant? acknowledgement = null;
		plan.Setup(x => x.FinalisePlan()).Callback(() => acknowledgement = f.Service.ReconcileOperation(f.Staff.Object,
			f.Actor.Object, start.OperationId!.Value, "Re-entrant inventory cleanup"));
		var action = effects.OfType<MagicPracticeAction>().Single(); f.Actor.Object.RemoveEffect(action, true);
		Assert.IsNotNull(acknowledgement); Assert.IsFalse(acknowledgement.Allowed);
		Assert.AreEqual("PracticeInterrupted", f.Store.Operation(start.OperationId!.Value)!.Stage);
		f.Now += TimeSpan.FromSeconds(30); action.ExpireEffect(); action.RemovalEffect();
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Practice_PhysicalInputEvents_CancelBriefLossEvenIfRestoredBeforeHeartbeat(bool speech)
	{
		var (f, effects) = Setup(); var start = f.Service.Cast(Intent(f)); var action = effects.OfType<MagicPracticeAction>().Single();
		if (speech)
		{
			f.Body.Setup(x => x.Communications.CanVocalise(f.Body.Object)).Returns(false);
			f.Body.Raise(x => x.OnWounded += null, f.Body.Object, Mock.Of<IWound>());
			f.Body.Setup(x => x.Communications.CanVocalise(f.Body.Object)).Returns(true);
		}
		else
		{
			var hands = f.Body.Object.FunctioningFreeHands.ToArray();
			f.Body.SetupGet(x => x.FunctioningFreeHands).Returns([]);
			f.Body.Raise(x => x.OnInventoryChange += null, InventoryState.Dropped, InventoryState.Held, Mock.Of<IGameItem>());
			f.Body.SetupGet(x => x.FunctioningFreeHands).Returns(hands);
		}
		Assert.AreEqual("PracticeInterrupted", f.Store.Operation(start.OperationId!.Value)!.Stage);
		Assert.AreEqual(0, effects.Count); f.Now += TimeSpan.FromSeconds(30); action.ExpireEffect();
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples);
	}

	private static (EffectHandler Actor, EffectHandler Body) NativeInputEffects(MagicCastingFixture f, List<IEffect> actions)

	{
		var actor = new EffectHandler(f.Actor.Object); var body = new EffectHandler(f.Body.Object);
		f.World.SetupGet(x => x.MagicCasting).Returns(f.Service);
		f.Body.SetupGet(x => x.Actor).Returns(f.Actor.Object);
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(actor.AddEffect);
		f.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) =>
		{
			if (actions.Remove(effect)) { if (fire) effect.RemovalEffect(); }
			else actor.RemoveEffect(effect, fire);
		});
		f.Body.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(body.AddEffect);
		f.Body.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>(body.RemoveEffect);
		f.Body.Setup(x => x.CombinedEffectsOfType<ISilencedEffect>()).Returns(() =>
			actor.Effects.Concat(body.Effects).OfType<ISilencedEffect>());
		f.Body.SetupGet(x => x.Communications).Returns(RobotCommunicationStrategy.Instance);
		f.Body.Setup(x => x.OrganFunction<SpeechSynthesizer>()).Returns(1.0);
		return (actor, body);
	}

	[DataTestMethod]
	[DataRow(true, true, false)]
	[DataRow(true, false, false)]
	[DataRow(true, true, true)]
	[DataRow(true, false, true)]
	[DataRow(false, true, false)]
	[DataRow(false, false, false)]
	[DataRow(false, true, true)]
	[DataRow(false, false, true)]
	public void Practice_NativeSilenceBetweenHeartbeats_LatchesRequiredSpeechLoss(bool requiresSpeech, bool bodyOwner, bool expire)
	{
		var (f, actions) = Setup(); NativeInputEffects(f, actions);
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack($"grades practice speech {requiresSpeech}")));
		Assert.IsTrue(f.Body.Object.Communications.CanVocalise(f.Body.Object));
		var start = f.Service.Cast(Intent(f)); Assert.AreEqual(MagicCastingStatus.Started, start.Status, start.Message);
		var action = actions.OfType<MagicPracticeAction>().Single();
		var skillDeadline = f.Store.Opportunity(100, 1)!.NextUtc;
		var masteryDeadline = f.Service.Acquisition(f.Actor.Object, 1)!.NextMasteryUtc;
		Mock.Get(f.World.Object.HeartbeatManager).Raise(x => x.FuzzyFiveSecondHeartbeat += null);
		IPerceivable owner = bodyOwner ? f.Body.Object : f.Actor.Object;
		var parent = new MagicSpellParent(owner, f.Spell, f.Actor.Object);
		var silence = new SpellSilenceEffect(owner, parent); parent.AddSpellEffect(silence);
		owner.AddEffect(parent); owner.AddEffect(silence);
		Assert.IsFalse(f.Body.Object.Communications.CanVocalise(f.Body.Object));
		if (expire) silence.ExpireEffect(); else owner.RemoveEffect(silence, true);
		Assert.IsTrue(f.Body.Object.Communications.CanVocalise(f.Body.Object));
		Assert.AreEqual(requiresSpeech ? "PracticeInterrupted" : "Practising", f.Store.Operation(start.OperationId!.Value)!.Stage);
		f.Now += TimeSpan.FromSeconds(30); action.ExpireEffect(); action.ExpireEffect();
		Assert.AreEqual(requiresSpeech ? "PracticeInterrupted" : "Completed", f.Store.Operation(start.OperationId.Value)!.Stage);
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]);
		Assert.AreEqual(skillDeadline, f.Store.Opportunity(100, 1)!.NextUtc);
		Assert.AreEqual(masteryDeadline, f.Service.Acquisition(f.Actor.Object, 1)!.NextMasteryUtc);
		Assert.AreEqual(requiresSpeech ? 0 : 1, f.Rolls); Assert.AreEqual(requiresSpeech ? 0 : 1, f.SkillUses);
		Assert.AreEqual(requiresSpeech ? 0 : 1, f.Samples);
	}

	[DataTestMethod]
	[DataRow("CheckingPractice", 0)]
	[DataRow("PracticeChecked", 1)]
	public void Practice_NativeSilenceDuringCompletion_LatchesLossBeforeCheckOrProgress(string stage, int checks)
	{
		var (f, actions) = Setup(); NativeInputEffects(f, actions);
		var start = f.Service.Cast(Intent(f)); var action = actions.OfType<MagicPracticeAction>().Single();
		var skillDeadline = f.Store.Opportunity(100, 1)!.NextUtc;
		var masteryDeadline = f.Service.Acquisition(f.Actor.Object, 1)!.NextMasteryUtc;
		f.Checkpoint = checkpoint =>
		{
			if (checkpoint != stage) return;
			var parent = new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object);
			var silence = new SpellSilenceEffect(f.Actor.Object, parent); parent.AddSpellEffect(silence);
			f.Actor.Object.AddEffect(parent); f.Actor.Object.AddEffect(silence); silence.ExpireEffect();
		};
		f.Now += TimeSpan.FromSeconds(30); action.ExpireEffect(); action.ExpireEffect();
		Assert.IsTrue(f.Body.Object.Communications.CanVocalise(f.Body.Object));
		Assert.AreEqual("PracticeInterrupted", f.Store.Operation(start.OperationId!.Value)!.Stage);
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]); Assert.AreEqual(checks, f.Rolls);
		Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples);
		Assert.AreEqual(skillDeadline, f.Store.Opportunity(100, 1)!.NextUtc);
		Assert.AreEqual(masteryDeadline, f.Service.Acquisition(f.Actor.Object, 1)!.NextMasteryUtc);
	}

	[DataTestMethod]
	[DataRow(true, false)]
	[DataRow(true, true)]
	[DataRow(false, false)]
	[DataRow(false, true)]
	public void Practice_NativeManipulationEffectsBetweenHeartbeats_LatchRequiredHandLoss(bool requiresHand, bool limbRestriction)
	{
		var (f, actions) = Setup(); var (_, body) = NativeInputEffects(f, actions);
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack($"grades practice hand {requiresHand}")));
		var hands = f.Body.Object.FunctioningFreeHands.ToArray(); var limb = Mock.Of<ILimb>();
		f.Body.SetupGet(x => x.FunctioningFreeHands).Returns(() => hands.Where(hand =>
			!body.Effects.OfType<IBodypartIneffectiveEffect>().Any(x => x.Applies() && x.Bodypart == hand) &&
			!body.Effects.OfType<ILimbIneffectiveEffect>().Any(x => x.Applies(limb) && x.AppliesToLimb(limb))));
		f.Body.Setup(x => x.GetLimbFor(It.IsAny<IBodypart>())).Returns(limb);
		var start = f.Service.Cast(Intent(f)); var action = actions.OfType<MagicPracticeAction>().Single();
		var skillDeadline = f.Store.Opportunity(100, 1)!.NextUtc;
		var masteryDeadline = f.Service.Acquisition(f.Actor.Object, 1)!.NextMasteryUtc;
		Mock.Get(f.World.Object.HeartbeatManager).Raise(x => x.FuzzyFiveSecondHeartbeat += null);
		IEffect restriction = limbRestriction ? new LimbDamageEffect(f.Body.Object, limb) : new BodypartExcessivelyDamaged(f.Body.Object, hands.Single());
		f.Body.Object.AddEffect(restriction); Assert.IsFalse(f.Body.Object.FunctioningFreeHands.Any());
		f.Body.Object.RemoveEffect(restriction); Assert.IsTrue(f.Body.Object.FunctioningFreeHands.Any());
		Assert.AreEqual(requiresHand ? "PracticeInterrupted" : "Practising", f.Store.Operation(start.OperationId!.Value)!.Stage);
		f.Now += TimeSpan.FromSeconds(30); action.ExpireEffect(); action.ExpireEffect();
		Assert.AreEqual(requiresHand ? "PracticeInterrupted" : "Completed", f.Store.Operation(start.OperationId.Value)!.Stage);
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]); Assert.AreEqual(requiresHand ? 0 : 1, f.Rolls);
		Assert.AreEqual(requiresHand ? 0 : 1, f.SkillUses); Assert.AreEqual(requiresHand ? 0 : 1, f.Samples);
		Assert.AreEqual(skillDeadline, f.Store.Opportunity(100, 1)!.NextUtc);
		Assert.AreEqual(masteryDeadline, f.Service.Acquisition(f.Actor.Object, 1)!.NextMasteryUtc);
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void Practice_NativeParalysisBetweenHealthUpdates_CancelsBeforeStateRefresh(bool bodyOwner)
	{
		var (f, actions) = Setup(); var (actorEffects, bodyEffects) = NativeInputEffects(f, actions);
		f.Actor.Setup(x => x.CombinedEffectsOfType<IForceParalysisEffect>()).Returns(() =>
			actorEffects.Effects.Concat(bodyEffects.Effects).OfType<IForceParalysisEffect>());
		var start = f.Service.Cast(Intent(f)); var action = actions.OfType<MagicPracticeAction>().Single();
		var skillDeadline = f.Store.Opportunity(100, 1)!.NextUtc;
		var masteryDeadline = f.Service.Acquisition(f.Actor.Object, 1)!.NextMasteryUtc;
		IPerceivable owner = bodyOwner ? f.Body.Object : f.Actor.Object;
		var parent = new MagicSpellParent(owner, f.Spell, f.Actor.Object);
		var paralysis = new SpellParalysisEffect(owner, parent); parent.AddSpellEffect(paralysis);
		owner.AddEffect(parent); owner.AddEffect(paralysis);
		Assert.AreEqual(CharacterState.Awake, f.Actor.Object.State); paralysis.ExpireEffect();
		f.Now += TimeSpan.FromSeconds(30); action.ExpireEffect(); action.ExpireEffect();
		Assert.AreEqual("PracticeInterrupted", f.Store.Operation(start.OperationId!.Value)!.Stage);
		Assert.AreEqual(85.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples);
		Assert.AreEqual(skillDeadline, f.Store.Opportunity(100, 1)!.NextUtc);
		Assert.AreEqual(masteryDeadline, f.Service.Acquisition(f.Actor.Object, 1)!.NextMasteryUtc);
	}
}
