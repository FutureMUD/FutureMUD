#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Inventory;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.Magic.Generators;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.Vancian;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using MudSharp.Testing.EnvironmentalMagic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LandRejuvenationTests
{
	[TestMethod, TestCategory("R-T01"), TestCategory("R-T02"), TestCategory("R-T13")]
	public void Template_FactoryBuilderCloneAndCompatibility_ArePureAndExplicit()
	{
		using var f = new Fixture();
		var template = (RejuvenateLandEffect)SpellEffectFactory.LoadEffectFromBuilderInput("rejuvenateland", new StringStack(""), f.Spell).Trigger;
		Assert.IsTrue(template.BuildingCommand(f.Environment.Builder, new StringStack("budget 12")));
		Assert.IsTrue(template.BuildingCommand(f.Environment.Builder, new StringStack("rate power + outcome")));
		Assert.IsTrue(template.BuildingCommand(f.Environment.Builder, new StringStack("desc The scarred earth is slowly mending.")));
		Assert.IsTrue(template.BuildingCommand(f.Environment.Builder, new StringStack("colour green")));
		Assert.IsFalse(template.BuildingCommand(f.Environment.Builder, new StringStack("rate unknownParameter")));
		var clone = (RejuvenateLandEffect)template.Clone();
		Assert.AreEqual(template.SaveToXml().ToString(), clone.SaveToXml().ToString());
		Assert.AreEqual("rejuvenateland", (string?)template.SaveToXml().Attribute("type"));
		var loaded = (RejuvenateLandEffect)SpellEffectFactory.LoadEffect(template.SaveToXml(), f.Spell);
		Assert.AreEqual(template.SaveToXml().ToString(), loaded.SaveToXml().ToString());
		StringAssert.Contains(SpellEffectFactory.BuilderInfoForType("rejuvenateland").BuilderHelp, "rate <expression>");
		Assert.IsFalse(template.BuildingCommand(f.Environment.Builder, new StringStack("help")));
		Assert.IsNull(clone.DefinitionError);
		StringAssert.Contains(clone.Show(f.Environment.Builder), "12");
		Assert.IsFalse(template.IsInstantaneous);
		Assert.IsTrue(template.RequiresTarget);
		Assert.IsTrue(template.IsCompatibleWithTrigger(f.Spell.Trigger));
		Assert.IsFalse(SubstanceSpellResolver.Supported(template));
		Assert.AreEqual("Unsupported", ScrollSpellCompatibility.Inventory.Single(x => x.Type == "rejuvenateland").Status);
		Assert.AreEqual(20.0, f.Scar);
		Assert.AreEqual(0, f.Environment.Coordinator.ActiveTreatmentCount);
		Assert.AreEqual(1, f.Environment.SecondSubscriptions);
		var malformed = template.SaveToXml();
		malformed.SetAttributeValue("version", 99);
		Assert.IsNotNull(((RejuvenateLandEffect)SpellEffectFactory.LoadEffect(malformed, f.Spell)).DefinitionError);
	}

	[TestMethod, TestCategory("R-T03"), TestCategory("R-T04"), TestCategory("R-T07"), TestCategory("R-I07")]
	public void Treatment_ExampleA_RepairsTwoThenDispelPreservesCompletedWork()
	{
		using var f = new Fixture();
		f.Environment.Edit("magicalrepaircap 1");
		var child = f.Install(12, 2, 600);
		Assert.AreEqual(20.0, f.Scar);
		f.Advance(120);
		Assert.AreEqual(18.0, f.Scar);
		Assert.AreEqual(10.0, f.Progress.RemainingBudget);
		Assert.AreEqual(2.0, f.Progress.TotalRepaired);
		f.Environment.Clock.Advance(TimeSpan.FromSeconds(30));
		f.Cell.RemoveEffect(child.ParentEffect, true);
		f.Advance(600);
		Assert.AreEqual(18.0, f.Scar);
		Assert.AreEqual(0, f.Environment.Coordinator.ActiveTreatmentCount);
		Assert.IsFalse(f.Cell.Effects.OfType<SpellRejuvenateLandEffect>().Any());
		Assert.AreEqual(LandRejuvenationStatus.Cancelled, f.Progress.Status);
	}

	[DataTestMethod, DataRow(true), DataRow(false), TestCategory("R-T03"), TestCategory("R-I06"), TestCategory("R-I07")]
	public void Treatment_ExampleB_FinalHalfMinuteIsAccountedOnce(bool parentExpiresFirst)
	{
		using var f = new Fixture();
		var child = f.Install(2.5, 1, 150);
		f.Advance(120);
		f.Environment.Clock.Advance(TimeSpan.FromSeconds(30));
		if (parentExpiresFirst) child.ParentEffect.ExpireEffect(); else f.Environment.Coordinator.Pump();
		child.ParentEffect.ExpireEffect();
		f.Environment.Coordinator.Pump();
		Assert.AreEqual(17.5, f.Scar);
		Assert.AreEqual(0.0, f.Progress.RemainingBudget);
		Assert.AreEqual(2.5, f.Progress.TotalRepaired);
		Assert.AreEqual(2L, f.Progress.AcknowledgedSequence);
	}

	[TestMethod, TestCategory("R-T05"), TestCategory("R-T06"), TestCategory("R-I21")]
	public void CastSpell_ExclusiveAndCompositeConflict_PreservesOldParentAndSiblings()
	{
		using var f = new Fixture();
		f.Spell.CastSpell(f.Actor.Object, f.Cell, SpellPower.Standard);
		var original = f.Cell.Effects.OfType<MagicSpellParent>().Single();
		var child = original.SpellEffects.Single();
		var sibling = new SpellRoomLightEffect(f.Cell, original, null!, 1, "", Telnet.Green);
		original.AddSpellEffect(sibling);
		f.Cell.AddEffect(sibling);
		var extra = new Mock<IMagicSpellEffectTemplate>();
		((List<IMagicSpellEffectTemplate>)f.Spell.SpellEffects).Add(extra.Object);
		f.Spell.CastSpell(f.Actor.Object, f.Cell, SpellPower.Standard);
		Assert.AreSame(original, f.Cell.Effects.OfType<MagicSpellParent>().Single());
		Assert.IsTrue(f.Cell.Effects.Contains(child));
		Assert.IsTrue(f.Cell.Effects.Contains(sibling));
		extra.Verify(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), It.IsAny<IPerceivable>(), It.IsAny<OpposedOutcomeDegree>(),
			It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(), It.IsAny<SpellAdditionalParameter[]>()), Times.Never);
		Assert.IsFalse(f.Template.TryPrepareApplication(f.Actor.Object, f.Cell, OpposedOutcomeDegree.Moderate, SpellPower.Strong,
			TimeSpan.FromSeconds(600), out _, out _));
		Assert.AreEqual(2, f.CostPayments);
	}

	[TestMethod, TestCategory("R-T12"), TestCategory("R-I21")]
	public void CastSpell_RepeatedRoomAndIndependentTargets_PaysOnceAndInstallsPerCell()
	{
		using var f = new Fixture(count: 2);
		var second = (Cell)f.Environment.Cells.At(1);
		f.Environment.Coordinator.ApplyOperation(second, new(Guid.NewGuid(), null, "test scars", Damage: 20));
		f.Spell.CastSpell(f.Actor.Object, new PerceivableGroup([f.Cell, second, f.Cell]), SpellPower.Standard);
		Assert.AreEqual(1, f.CostPayments);
		Assert.AreEqual(1, f.Cell.Effects.OfType<MagicSpellParent>().Count());
		Assert.AreEqual(1, second.Effects.OfType<MagicSpellParent>().Count());
		Assert.AreEqual(2, f.Environment.Coordinator.ActiveTreatmentCount);
		Assert.AreEqual(20.0, f.Scar);
	}

	[TestMethod, TestCategory("R-T10"), TestCategory("R-I01"), TestCategory("R-I02")]
	public void Repair_InvalidUnrelatedOutputAndNoField_StillRepairsWithoutRefill()
	{
		using var f = new Fixture();
		f.Environment.Edit("output 1 maximum max(0,100-scardamage)");
		Assert.IsTrue(f.Environment.Coordinator.TryMutateResource(f.Cell, f.Environment.Resources.At(0), EnvironmentalResourceMutation.Set, 30, out var set));
		Assert.IsTrue(set);
		f.Environment.Edit("output 1 rate 1 / (20 - scardamage)");
		Assert.IsFalse(f.Environment.Coordinator.Inspect(f.Cell).IsValid);
		f.Install(5, 5, 60);
		f.Advance(60);
		Assert.AreEqual(15.0, f.Scar);
		Assert.AreEqual(30.0, f.Cell.MagicResourceAmounts[f.Environment.Resources.At(0)]);
		f.Environment.Edit("output 1 rate 0");
		Assert.AreEqual(85.0, f.Environment.Coordinator.Inspect(f.Cell).Outputs.Single().Maximum);
	}

	[DataTestMethod, DataRow(100.0, 100.0, "1"), DataRow(0.0, 0.0, "1"), DataRow(100.0, 30.0, "0"), TestCategory("R-I01")]
	public void Treatment_FullZeroCapacityAndDormantCells_AdvanceWithoutResourceProduction(double maximum, double balance, string rate)
	{
		using var f = new Fixture();
		f.Environment.Edit($"output 1 maximum {maximum:R}");
		f.Environment.Edit($"output 1 rate {rate}");
		var resource = f.Environment.Resources.At(0);
		Assert.IsTrue(f.Environment.Coordinator.TryMutateResource(f.Cell, resource, EnvironmentalResourceMutation.Set, balance, out _));
		f.Environment.Coordinator.Pump();
		Assert.IsTrue(f.Environment.Coordinator.Inspect(f.Cell).IsValid);
		f.Install();
		f.Advance(60);
		Assert.AreEqual(19.0, f.Scar);
		Assert.AreEqual(11.0, f.Progress.RemainingBudget);
		Assert.AreEqual(balance, f.Cell.MagicResourceAmounts[resource]);
	}

	[TestMethod, TestCategory("R-I02")]
	public void Treatment_InvalidOrganicDefinition_DoesNotBlockRepair()
	{
		using var f = new Fixture();
		var definition = XElement.Parse(f.Environment.Profile.ExportDefinition());
		definition.Element("Organic")?.Remove();
		definition.Add(new XElement("Organic", new XAttribute("version", 1), new XElement("Sources"),
			new XElement("Penalties", new XElement("Penalty", new XAttribute("channel", NativeOrganicPenaltyChannel.CropYieldRecovery), "2"))));
		var profile = f.Environment.AddProfile(2, definition);
		Assert.IsTrue(profile.OrganicValidationErrors.Count > 0);
		Assert.IsFalse(profile.EvaluateOrganicPenalty(NativeOrganicPenaltyChannel.CropYieldRecovery, new Dictionary<string, double>()).IsValid);
		f.Environment.Coordinator.SetBinding(f.Cell, EnvironmentalMagicBindingMode.Explicit, profile.Id);
		f.Install();
		f.Advance(60);
		Assert.AreEqual(19.0, f.Scar);
		Assert.AreEqual(1.0, f.Progress.TotalRepaired);
	}

	[DataTestMethod, DataRow(false), DataRow(true), TestCategory("R-I02")]
	public void Admission_InvalidRepairPolicyOrScalarState_PreservesEvidenceWithoutInstallation(bool invalidState)
	{
		using var f = new Fixture();
		if (invalidState)
		{
			f.Cell.LoadEnvironmentForTest(f.Cell.EnvironmentState with { ScarDamage = double.NaN });
		}
		else
		{
			var definition = XElement.Parse(f.Environment.Profile.ExportDefinition());
			definition.SetElementValue("MagicalRepairLimitPerMinute", "invalid");
			var profile = f.Environment.AddProfile(2, definition);
			f.Environment.Coordinator.SetBinding(f.Cell, EnvironmentalMagicBindingMode.Explicit, profile.Id);
		}
		Assert.IsFalse(f.Template.TryPrepareApplication(f.Actor.Object, f.Cell, OpposedOutcomeDegree.Moderate,
			SpellPower.Standard, TimeSpan.FromMinutes(1), out _, out var error));
		Assert.IsFalse(string.IsNullOrEmpty(error));
		Assert.AreEqual(0, f.Environment.Coordinator.ActiveTreatmentCount);
		Assert.AreEqual(0, f.Environment.Operations.Treatments.Count);
		if (invalidState) Assert.IsTrue(double.IsNaN(f.Scar)); else Assert.AreEqual(20.0, f.Scar);
	}

	[TestMethod, TestCategory("R-T04"), TestCategory("R-T09"), TestCategory("R-I19")]
	public void NaturalRepair_IsNotSpentFromBudget_AndZeroTransitionEndsBeforeNewDamage()
	{
		using var f = new Fixture();
		f.Environment.Edit("repair 1");
		f.Environment.Coordinator.Pump();
		f.Install(12, 1, 600);
		f.Advance(60);
		Assert.AreEqual(18.0, f.Scar, 0.00001);
		Assert.AreEqual(11.0, f.Progress.RemainingBudget, 0.00001);
		Assert.IsTrue(f.Environment.Coordinator.ApplyOperation(f.Cell, new(Guid.NewGuid(), null, "staff repair", Repair: 100)).Success);
		Assert.AreEqual(0, f.Environment.Coordinator.ActiveTreatmentCount);
		Assert.IsTrue(f.Environment.Coordinator.ApplyOperation(f.Cell, new(Guid.NewGuid(), null, "new damage", Damage: 5)).Success);
		f.Environment.Edit("repair 0");
		f.Environment.Coordinator.Pump();
		f.Advance(120);
		Assert.AreEqual(5.0, f.Scar);
	}

	[DataTestMethod, DataRow(false), DataRow(true), TestCategory("R-I19")]
	public void NaturalZeroThenDamage_BeforePump_TerminatesOldTreatment(bool failTerminationSave)
	{
		using var f = new Fixture(scar: 1);
		f.Environment.Edit("repair 1");
		f.Environment.Coordinator.Pump();
		var child = f.Install();
		f.Environment.Clock.Advance(TimeSpan.FromSeconds(60));
		var request = new EnvironmentalMagicOperationRequest(Guid.NewGuid(), null, "later damage", Damage: 5);
		if (failTerminationSave)
		{
			f.Environment.Operations.FailTreatmentSave = true;
			Assert.IsFalse(f.Environment.Coordinator.ApplyOperation(f.Cell, request).Success);
			Assert.IsFalse(f.Environment.Operations.Receipts.ContainsKey(request.OperationId));
			Assert.IsFalse(f.Environment.Operations.Treatments[child.TreatmentId].CancellationRequested);
			Assert.AreEqual(1.0, f.Scar, "Failed termination must not commit new damage or consume the natural sample.");
			f.Environment.Operations.FailTreatmentSave = false;
		}
		Assert.IsTrue(f.Environment.Coordinator.ApplyOperation(f.Cell, request).Success);
		Assert.AreEqual(0, f.Environment.Coordinator.ActiveTreatmentCount);
		Assert.IsTrue(f.Environment.Operations.Treatments[child.TreatmentId].CancellationRequested);
		Assert.AreEqual(0.0, f.Progress.TotalRepaired);
		Assert.AreEqual(1.0, f.Progress.RemainingBudget);
		f.Environment.Edit("repair 0");
		f.Environment.Coordinator.Pump();
		f.Advance(120);
		Assert.AreEqual(5.0, f.Scar);
	}

	[TestMethod, TestCategory("R-I03"), TestCategory("R-I04"), TestCategory("R-I05")]
	public void ProfileCap_EditIsProspective_AndDisableIsTerminal()
	{
		using var f = new Fixture();
		Assert.IsNull(f.Environment.Profile.MagicalRepairLimitPerMinute);
		f.Environment.Edit("magicalrepaircap 1");
		f.Install(12, 10, 600);
		f.Environment.Clock.Advance(TimeSpan.FromSeconds(30));
		f.Environment.Edit("magicalrepaircap 2");
		Assert.AreEqual(19.5, f.Scar);
		f.Advance(60);
		Assert.AreEqual(17.5, f.Scar);
		f.Environment.Clock.Advance(TimeSpan.FromSeconds(30));
		f.Environment.Edit("magicalrepaircap 0.5");
		Assert.AreEqual(16.5, f.Scar, "The old cap owns the elapsed half-minute before the decrease.");
		f.Advance(60);
		Assert.AreEqual(16.0, f.Scar);
		Assert.AreEqual(10.0, f.Progress.Rate);
		Assert.AreEqual(12.0, f.Progress.InitialBudget);
		f.Environment.Edit("magicalrepaircap 0");
		Assert.AreEqual(0, f.Environment.Coordinator.ActiveTreatmentCount);
		f.Environment.Edit("magicalrepaircap none");
		f.Advance(600);
		Assert.AreEqual(16.0, f.Scar);
		Assert.AreEqual(0.0, f.Environment.Profile.NaturalRepairPerMinute);
		Assert.IsFalse(f.Environment.Profile.BuildingCommand(f.Environment.Builder, new StringStack("magicalrepaircap NaN")));
	}

	[DataTestMethod, DataRow(false), DataRow(true), TestCategory("R-I05")]
	public void Binding_DisableOrReplaceThenRestore_DoesNotReviveTreatment(bool replace)
	{
		using var f = new Fixture();
		f.Install();
		var replacement = f.Environment.AddProfile(2, XElement.Parse(f.Environment.Profile.ExportDefinition()));
		f.Environment.Coordinator.SetBinding(f.Cell, replace ? EnvironmentalMagicBindingMode.Explicit : EnvironmentalMagicBindingMode.Disabled,
			replace ? replacement.Id : null);
		f.Environment.Coordinator.SetBinding(f.Cell, EnvironmentalMagicBindingMode.Inherit, null);
		f.Advance(120);
		Assert.AreEqual(20.0, f.Scar);
		Assert.AreEqual(0, f.Environment.Coordinator.ActiveTreatmentCount);
		Assert.AreEqual(LandRejuvenationStatus.Cancelled, f.Progress.Status);
	}

	[TestMethod, TestCategory("R-I17")]
	public void ForeignPendingOperation_IsNeverAcknowledged_AndBlockedTimeEarnsNothing()
	{
		using var f = new Fixture();
		f.Install();
		var foreign = new EnvironmentalMagicOperationRequest(Guid.NewGuid(), null, "foreign uncertainty", Damage: 1);
		f.Environment.Operations.FailAfterCommit = true;
		Assert.IsFalse(f.Environment.Coordinator.ApplyOperation(f.Cell, foreign).Success);
		f.Advance(120);
		Assert.AreEqual(0L, f.Progress.AcknowledgedSequence);
		Assert.AreEqual(0.0, f.Progress.EarnedWork);
		Assert.AreEqual(480.0, f.Progress.RemainingSeconds);
		f.Environment.Operations.FailAfterCommit = false;
		Assert.IsTrue(f.Environment.Coordinator.ApplyOperation(f.Cell, foreign).Success);
		f.Advance(60);
		Assert.AreEqual(20.0, f.Scar);
		Assert.AreEqual(1.0, f.Progress.TotalRepaired);
		Assert.AreNotEqual(foreign.OperationId, f.Progress.LastOperationId);
	}

	[TestMethod, TestCategory("R-T07"), TestCategory("R-I10"), TestCategory("R-I24")]
	public void Continuation_ReentrantMutationFailsClosed_AndDiscardsElapsedWork()
	{
		using var f = new Fixture();
		var prog = new Mock<MudSharp.FutureProg.IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(88L);
		prog.SetupGet(x => x.StaticType).Returns(MudSharp.FutureProg.FutureProgStaticType.NotStatic);
		prog.SetupGet(x => x.ReturnType).Returns(MudSharp.FutureProg.ProgVariableTypes.Boolean);
		prog.SetupGet(x => x.Parameters).Returns([ProgVariableTypes.Character, ProgVariableTypes.Location]);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<MudSharp.FutureProg.ProgVariableTypes>>())).Returns(true);
		var running = false;
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(() =>
		{
			if (running) f.Environment.Coordinator.ApplyOperation(f.Cell, new(Guid.NewGuid(), null, "forbidden", Damage: 10));
			return true;
		});
		((EnvironmentalMagicTestRegistry<MudSharp.FutureProg.IFutureProg>)f.Environment.World.Object.FutureProgs).Add(prog.Object);
		Assert.IsTrue(f.Template.BuildingCommand(f.Environment.Builder, new StringStack("continuation 88")));
		f.Install();
		running = true;
		f.Advance(60);
		Assert.AreEqual(20.0, f.Scar);
		Assert.AreEqual(0.0, f.Progress.TotalRepaired);
		Assert.AreEqual(LandRejuvenationStatus.Cancelled, f.Progress.Status);
	}

	[DataTestMethod, DataRow("0", "1", 600), DataRow("-1", "1", 600), DataRow("1", "0", 600),
	 DataRow("1 / 0", "1", 600), DataRow("12", "1", 0), DataRow("1e308", "1e308", 600000), TestCategory("R-T01"), TestCategory("R-T02")]
	public void Admission_InvalidValuesDoNotAttachOrRepair(string budget, string rate, int seconds)
	{
		using var f = new Fixture();
		var template = new RejuvenateLandEffect(new XElement("Effect", new XAttribute("version", 1),
			new XElement("Budget", budget), new XElement("Rate", rate)), f.Spell);
		Assert.IsFalse(template.TryPrepareApplication(f.Actor.Object, f.Cell, OpposedOutcomeDegree.Moderate, SpellPower.Standard,
			TimeSpan.FromSeconds(seconds), out _, out var error));
		Assert.IsFalse(string.IsNullOrEmpty(error));
		Assert.AreEqual(20.0, f.Scar);
		Assert.AreEqual(0, f.Environment.Operations.Treatments.Count);
	}

	[TestMethod, TestCategory("R-I13"), TestCategory("R-I14")]
	public void Precision_SubUlpWorkAccumulatesWithoutReceipts_AndTinyOrdinaryRepairIsValid()
	{
		using var f = new Fixture(scar: 1e12);
		f.Install(1, 1e-6, 60000);
		var receipts = f.Environment.Operations.Commits;
		f.Advance(60);
		Assert.AreEqual(1e12, f.Scar);
		Assert.AreEqual(1.0, f.Progress.RemainingBudget);
		Assert.AreEqual(receipts, f.Environment.Operations.Commits);
		Assert.AreEqual(1e-6, f.Progress.EarnedWork);
		f.Advance(10000);
		Assert.IsTrue(f.Progress.TotalRepaired > 0.0);
		Assert.IsTrue(f.Progress.TotalRepaired <= 1e-6 * 10060 / 60);
		var tiny = ConservativeScarRepair.Calculate(1e-5, 1e-6);
		Assert.IsTrue(tiny.Applied > 0.0 && tiny.Applied <= 1e-6);
		foreach (var scar in new[] { 1e-12, 0.1, 1.0, 1e12 })
			foreach (var allowance in new[] { double.Epsilon, 1e-6, 0.1, scar, Math.BitDecrement(scar) })
			{
				var result = ConservativeScarRepair.Calculate(scar, allowance);
				Assert.IsTrue(result.Remaining >= 0 && result.Applied >= 0 && result.Applied <= allowance);
				Assert.AreEqual(scar - result.Remaining, result.Applied);
			}
	}

	[DataTestMethod, DataRow(false), DataRow(true), TestCategory("R-I15"), TestCategory("R-I16")]
	public void AtomicStep_RollbackOrLostAck_ReconcilesExactIdentityWithoutDoubleRepair(bool committed)
	{
		using var f = new Fixture();
		f.Install();
		f.Environment.Operations.FailAfterCommit = committed;
		f.Environment.Operations.FailAfterClaim = !committed;
		f.Advance(60);
		var request = f.Progress.PendingRequest!;
		Assert.IsNotNull(request);
		Assert.IsFalse(f.Environment.Coordinator.CanInstallTreatment(f.Cell, out _));
		var stored = f.Environment.Operations.Treatments[f.Progress.Id];
		Assert.AreEqual(committed ? 11.0 : 12.0, stored.RemainingBudget);
		f.Environment.Operations.FailAfterCommit = false;
		f.Environment.Operations.FailAfterClaim = false;
		Assert.IsTrue(f.Environment.Coordinator.ConfirmTreatment(f.Cell, f.Progress.Id, out var error), error);
		Assert.AreEqual(committed ? 19.0 : 20.0, f.Scar, "Staff confirmation must never apply repair.");
		if (!committed) f.Advance(60);
		Assert.AreEqual(19.0, f.Scar);
		Assert.AreEqual(11.0, f.Progress.RemainingBudget);
		Assert.AreEqual(request.OperationId, f.Progress.LastOperationId);
		Assert.IsTrue(f.Environment.Coordinator.ConfirmTreatment(f.Cell, f.Progress.Id, out error), error);
		Assert.AreEqual(19.0, f.Scar);
	}

	[TestMethod, TestCategory("R-I18")]
	public void Dispel_UnresolvedRollback_CancelsWithoutRetryOrResurrection()
	{
		using var f = new Fixture();
		var child = f.Install();
		f.Environment.Operations.FailAfterClaim = true;
		f.Advance(60);
		var id = f.Progress.Id;
		f.Cell.RemoveEffect(child.ParentEffect, true);
		Assert.IsTrue(f.Progress.CancellationRequested);
		f.Environment.Operations.FailAfterClaim = false;
		Assert.IsTrue(f.Environment.Coordinator.ConfirmTreatment(f.Cell, id, out var error), error);
		f.Advance(120);
		Assert.AreEqual(20.0, f.Scar);
		Assert.AreEqual(LandRejuvenationStatus.Cancelled, f.Progress.Status);
	}

	[TestMethod, TestCategory("R-I08"), TestCategory("R-I09")]
	public void Caster_IndependentLogoutContinues_LocalQuitCancelsWithoutLoading()
	{
		using var f = new Fixture();
		var independent = f.Install();
		f.Actors.Remove(f.Actor.Object);
		f.Actor.Raise(x => x.OnQuit += null, f.Actor.Object);
		f.Advance(60);
		Assert.AreEqual(19.0, f.Scar);
		f.Cell.RemoveEffect(independent, true);
		f.Actors.Add(f.Actor.Object);
		f.Template.BuildingCommand(f.Environment.Builder, new StringStack("local on"));
		f.Install();
		f.Actor.Raise(x => x.OnQuit += null, f.Actor.Object);
		f.Advance(120);
		Assert.AreEqual(19.0, f.Scar);
		f.Environment.World.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod, TestCategory("R-T08"), TestCategory("R-I11"), TestCategory("R-I12")]
	public void ParentChild_XmlReload_UsesAuthoritativeBudgetAndFreshOnlineEpoch()
	{
		var store = new EnvironmentalMagicTestOperationStore();
		XElement saved;
		using (var first = new Fixture(operations: store))
		{
			var child = first.Install();
			first.Advance(60);
			saved = child.ParentEffect.SaveToXml([]);
			Assert.IsTrue(saved.Descendants("Type").Any(x => x.Value == "SpellRejuvenateLand"));
		}
		using var f = new Fixture(operations: store, scar: null);
		// Independent environmental load uses the committed scalar, never the former process clock.
		f.Cell.LoadEnvironmentForTest(store.PersistedStates[f.Cell.Id]);
		MagicSpellParent.InitialiseEffectType();
		SpellRejuvenateLandEffect.InitialiseEffectType();
		var parent = (MagicSpellParent)Effect.LoadEffect(saved, f.Cell);
		Assert.AreEqual(19.0, f.Scar);
		f.Cell.AddEffect(parent, TimeSpan.FromSeconds(540));
		Assert.AreEqual(0, f.Environment.Coordinator.ActiveTreatmentCount);
		parent.Login();
		f.Environment.Coordinator.Pump();
		Assert.AreEqual(19.0, f.Scar);
		Assert.AreEqual(11.0, f.Progress.RemainingBudget);
		f.Advance(60);
		Assert.AreEqual(18.0, f.Scar);
		Assert.AreEqual(10.0, f.Progress.RemainingBudget);
	}

	[TestMethod, TestCategory("R-I11"), TestCategory("R-I12")]
	public void SaveBetweenVisits_CheckpointsEarnedWorkWithoutRepair_ThenRestoresFinalInterval()
	{
		var store = new EnvironmentalMagicTestOperationStore();
		XElement saved;
		using (var first = new Fixture(operations: store))
		{
			var child = first.Install(12, 1, 600);
			first.Advance(60);
			first.Environment.Clock.Advance(TimeSpan.FromSeconds(30));
			saved = child.ParentEffect.SaveToXml(new() { [child.ParentEffect] = TimeSpan.FromSeconds(510) });
			Assert.AreEqual(19.0, first.Scar);
			Assert.AreEqual(510.0, first.Progress.RemainingSeconds);
			Assert.AreEqual(0.5, first.Progress.EarnedWork);
		}
		using var f = new Fixture(operations: store, scar: null);
		f.Cell.LoadEnvironmentForTest(store.PersistedStates[f.Cell.Id]);
		MagicSpellParent.InitialiseEffectType();
		SpellRejuvenateLandEffect.InitialiseEffectType();
		var parent = (MagicSpellParent)Effect.LoadEffect(saved, f.Cell);
		f.Cell.AddEffect(parent, TimeSpan.FromSeconds(510));
		parent.Login();
		f.Environment.Coordinator.Pump();
		f.Environment.Clock.Advance(TimeSpan.FromSeconds(510));
		parent.ExpireEffect();
		Assert.AreEqual(10.0, f.Scar);
		Assert.AreEqual(0.0, f.Progress.RemainingSeconds);
		Assert.AreEqual(10.0, f.Progress.TotalRepaired);
	}

	[TestMethod, TestCategory("R-I12")]
	public void ReloadMissingSource_TerminatesPersistedSlot_WithoutRepair()
	{
		using var f = new Fixture();
		var child = f.Install();
		var saved = child.ParentEffect.SaveToXml([]);
		// Model a fresh coordinator after an unclean stop, with the source deleted before activation.
		f.Environment.Coordinator.Unregister(f.Cell);
		((All<IMagicSpell>)f.Environment.World.Object.MagicSpells).Remove(f.Spell);
		MagicSpellParent.InitialiseEffectType();
		SpellRejuvenateLandEffect.InitialiseEffectType();
		var parent = (MagicSpellParent)Effect.LoadEffect(saved, f.Cell);
		f.Cell.AddEffect(parent, TimeSpan.FromSeconds(600));
		parent.Login();
		f.Environment.Coordinator.Pump();
		Assert.AreEqual(20.0, f.Scar);
		Assert.AreEqual(LandRejuvenationStatus.Cancelled, f.Progress.Status);
		Assert.IsTrue(f.Environment.Coordinator.CanInstallTreatment(f.Cell, out var error), error);
	}

	[TestMethod, TestCategory("R-I22"), TestCategory("R-I23"), TestCategory("R-I24")]
	public void Scheduling_ThirtyThousandCells_VisitsOnlyIndexedTreatmentsAndInspectionIsPure()
	{
		using var f = new Fixture(count: 30000);
		f.Environment.Cells.ForbidEnumeration = true;
		f.Environment.Fields.ForbidEnumeration = true;
		var reads = f.Environment.Operations.TreatmentReads;
		f.Advance(60);
		Assert.AreEqual(reads, f.Environment.Operations.TreatmentReads);
		Assert.AreEqual(0L, f.Environment.Coordinator.TreatmentVisits);
		var child = f.Install();
		for (var i = 0; i < 5; i++) _ = child.Describe(f.Environment.Builder);
		Assert.AreEqual(20.0, f.Scar);
		f.Advance(60);
		Assert.AreEqual(1L, f.Environment.Coordinator.TreatmentVisits);
		Assert.AreEqual(19.0, f.Scar);
		Assert.IsTrue(f.Environment.Coordinator.Diagnostics.LastCellVisits <= f.Environment.Coordinator.Options.MaximumCellVisits);
		f.Environment.Coordinator.Unregister(f.Cell);
		Assert.AreEqual(0, f.Environment.Coordinator.ActiveTreatmentCount);
	}

	[TestMethod, TestCategory("R-I23")]
	public void Dispose_RemovesActiveWorkAndHeartbeatWithoutCrossWorldReferences()
	{
		using var first = new Fixture();
		var child = first.Install();
		first.Dispose();
		Assert.AreEqual(0, first.Environment.Coordinator.ActiveTreatmentCount);
		Assert.AreEqual(0, first.Environment.SecondSubscriptions);
		Assert.IsNull(first.Environment.Coordinator.InspectTreatment(first.Cell, child.TreatmentId));
		first.Advance(120);
		Assert.AreEqual(20.0, first.Scar);
		using var next = new Fixture();
		next.Install();
		next.Advance(60);
		Assert.AreEqual(19.0, next.Scar);
		Assert.AreEqual(20.0, first.Scar);
		Assert.AreEqual(1, next.Environment.SecondSubscriptions);
	}

	[DataTestMethod, DataRow(false), DataRow(true), TestCategory("R-I04")]
	public void Expressions_CaptureCastingTraitAndSustainedBonusContext(bool vancianContext)
	{
		using var f = new Fixture();
		f.Actor.Setup(x => x.TraitValue(f.Spell.CastingTrait, TraitBonusContext.SpellDuration)).Returns(4.0);
		Assert.IsTrue(f.Template.BuildingCommand(f.Environment.Builder, new StringStack("budget variable + 1")));
		Assert.IsTrue(f.Template.BuildingCommand(f.Environment.Builder, new StringStack("rate variable")));
		if (vancianContext) ScrollSpellCompatibility.Bind(f.Spell, new SpellNumericalContext(0, 1, 3, SpellPower.Standard, Outcome.Pass, false), null);
		f.Spell.CastSpell(f.Actor.Object, f.Cell, SpellPower.Standard);
		Assert.AreEqual(5.0, f.Progress.InitialBudget);
		Assert.AreEqual(4.0, f.Progress.Rate);
		f.Actor.Setup(x => x.TraitValue(f.Spell.CastingTrait, TraitBonusContext.SpellDuration)).Returns(100.0);
		f.Template.BuildingCommand(f.Environment.Builder, new StringStack("rate 100"));
		f.Advance(60);
		Assert.AreEqual(16.0, f.Scar);
		Assert.AreEqual(1.0, f.Progress.RemainingBudget);
	}

	[DataTestMethod, DataRow("none"), DataRow("0"), DataRow("1.25"), TestCategory("R-I03")]
	public void ProfileCeiling_RoundTripsIndependentlyFromNaturalRepair(string value)
	{
		using var f = new Fixture();
		f.Environment.Edit("repair 0.5");
		f.Environment.Edit($"magicalrepaircap {value}");
		var clone = new EnvironmentalMagicGenerator(new MudSharp.Models.MagicGenerator
		{ Id = 2, Name = "Copy", Type = "environmental", Definition = f.Environment.Profile.ExportDefinition() }, f.Environment.World.Object);
		Assert.AreEqual(f.Environment.Profile.MagicalRepairLimitPerMinute, clone.MagicalRepairLimitPerMinute);
		Assert.AreEqual(0.5, clone.NaturalRepairPerMinute);
		var xml = XElement.Parse(clone.ExportDefinition());
		xml.SetElementValue("MagicalRepairLimitPerMinute", "invalid");
		var malformed = new EnvironmentalMagicGenerator(new MudSharp.Models.MagicGenerator
		{ Id = 3, Name = "Malformed", Type = "environmental", Definition = xml.ToString() }, f.Environment.World.Object);
		Assert.IsTrue(malformed.RepairValidationErrors.Count > 0);
		Assert.AreEqual(0, malformed.ValidationErrors.Count, "Repair-specific failure must preserve valid legacy output policy.");
		Assert.AreEqual(20.0, f.Scar);
	}

	[TestMethod, TestCategory("R-T10")]
	public void Repair_ClosesOldProductionSample_AndLaterRegenerationUsesRestoredCapacity()
	{
		using var f = new Fixture();
		var resource = f.Environment.Resources.At(0);
		f.Environment.Edit("output 1 maximum max(0,100-scardamage)");
		f.Environment.Edit("output 1 rate max(0,20-scardamage)");
		Assert.IsTrue(f.Environment.Coordinator.TryMutateResource(f.Cell, resource, EnvironmentalResourceMutation.Set, 30, out _));
		f.Environment.Coordinator.Pump();
		f.Install(5, 5, 60);
		f.Advance(60);
		Assert.AreEqual(15.0, f.Scar);
		Assert.AreEqual(85.0, f.Environment.Coordinator.Inspect(f.Cell).Outputs.Single().Maximum);
		Assert.AreEqual(30.0, f.Cell.MagicResourceAmounts[resource], "Restored rate must not produce during the old scarred interval.");
		f.Advance(60);
		Assert.AreEqual(35.0, f.Cell.MagicResourceAmounts[resource], 1e-9);
	}

	[DataTestMethod, DataRow("move"), DataRow("layer"), DataRow("plane"), DataRow("death"), DataRow("stasis"), DataRow("instance"), TestCategory("R-I09")]
	public void LocalMaintenance_OriginalInstanceEligibilityLossEndsTreatment(string change)
	{
		using var f = new Fixture(count: 2);
		f.Template.BuildingCommand(f.Environment.Builder, new StringStack("local on"));
		f.Install();
		switch (change)
		{
			case "move": f.Actor.SetupGet(x => x.Location).Returns(f.Environment.Cells.At(1)); break;
			case "layer": f.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InTrees); break;
			case "plane": f.Actor.SetupGet(x => x.Body.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(2L)); break;
			case "death": f.Actor.Raise(x => x.OnDeath += null, f.Actor.Object); break;
			case "stasis": f.Actor.SetupGet(x => x.State).Returns(CharacterState.Stasis); f.Actor.Raise(x => x.OnStateChanged += null, f.Actor.Object); break;
			case "instance": f.Actor.SetupGet(x => x.InstanceId).Returns(12L); break;
		}
		f.Advance(60);
		Assert.AreEqual(LandRejuvenationStatus.Cancelled, f.Progress.Status);
		Assert.AreEqual(20.0, f.Scar);
		f.Environment.World.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod, TestCategory("R-I09"), TestCategory("R-I10")]
	public void Continuation_FalsePolicyEndsRemoteTreatment()
	{
		using var f = new Fixture(count: 2);
		var permit = true;
		var prog = new Mock<MudSharp.FutureProg.IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(88L);
		prog.SetupGet(x => x.StaticType).Returns(MudSharp.FutureProg.FutureProgStaticType.NotStatic);
		prog.SetupGet(x => x.ReturnType).Returns(MudSharp.FutureProg.ProgVariableTypes.Boolean);
		prog.SetupGet(x => x.Parameters).Returns([ProgVariableTypes.Character, ProgVariableTypes.Location]);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<MudSharp.FutureProg.ProgVariableTypes>>())).Returns(true);
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(() => permit);
		f.Environment.Progs.Add(prog.Object);
		f.Template.BuildingCommand(f.Environment.Builder, new StringStack("continuation 88"));
		f.Install();
		f.Actor.SetupGet(x => x.Location).Returns(f.Environment.Cells.At(1));
		f.Advance(60);
		Assert.AreEqual(19.0, f.Scar);
		permit = false;
		f.Advance(60);
		Assert.AreEqual(19.0, f.Scar);
		Assert.AreEqual(LandRejuvenationStatus.Cancelled, f.Progress.Status);
	}

	[DataTestMethod, DataRow("any"), DataRow("supertype"), DataRow("return"), DataRow("static"), DataRow("compile"), TestCategory("R-I10")]
	public void Policy_BuilderLoadAndEvaluationRejectInexactOrInvalidSignature(string invalid)
	{
		using var f = new Fixture();
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(88L);
		prog.SetupGet(x => x.StaticType).Returns(FutureProgStaticType.NotStatic);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.SetupGet(x => x.Parameters).Returns([ProgVariableTypes.Character, ProgVariableTypes.Location]);
		// This broad compatibility API deliberately accepts all rows: the contract requires exact types.
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		f.Environment.Progs.Add(prog.Object);
		Assert.IsTrue(f.Template.BuildingCommand(f.Environment.Builder, new StringStack("eligibility 88")));
		var xml = f.Template.SaveToXml();
		switch (invalid)
		{
			case "any": prog.SetupGet(x => x.AcceptsAnyParameters).Returns(true); break;
			case "supertype": prog.SetupGet(x => x.Parameters).Returns([ProgVariableTypes.Perceivable, ProgVariableTypes.Location]); break;
			case "return": prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number); break;
			case "static": prog.SetupGet(x => x.StaticType).Returns(FutureProgStaticType.FullyStatic); break;
			case "compile": prog.SetupGet(x => x.CompileError).Returns("Invalid source"); break;
		}
		Assert.IsFalse(f.Template.BuildingCommand(f.Environment.Builder, new StringStack("continuation 88")));
		var restored = new RejuvenateLandEffect(xml, f.Spell);
		Assert.IsNotNull(restored.DefinitionError);
		Assert.IsFalse(restored.TryPrepareApplication(f.Actor.Object, f.Cell, OpposedOutcomeDegree.Moderate,
			SpellPower.Standard, TimeSpan.FromMinutes(1), out _, out _));
		Assert.IsFalse(f.Environment.Coordinator.EvaluateRepairPolicy(f.Cell, f.Actor.Object, prog.Object, out var error));
		Assert.IsFalse(string.IsNullOrEmpty(error));
		prog.Verify(x => x.ExecuteBool(It.IsAny<object[]>()), Times.Never);
		Assert.AreEqual(20.0, f.Scar);
		Assert.AreEqual(0, f.Environment.Operations.Treatments.Count);
	}

	[TestMethod, TestCategory("R-I12")]
	public void DuplicateLoadedParentIdentity_IsIdempotent()
	{
		var store = new EnvironmentalMagicTestOperationStore();
		XElement saved;
		using (var first = new Fixture(operations: store)) saved = first.Install().ParentEffect.SaveToXml([]);
		using var f = new Fixture(operations: store, scar: null);
		f.Cell.LoadEnvironmentForTest(store.PersistedStates[f.Cell.Id]);
		MagicSpellParent.InitialiseEffectType(); SpellRejuvenateLandEffect.InitialiseEffectType();
		for (var i = 0; i < 2; i++)
		{
			var parent = (MagicSpellParent)Effect.LoadEffect(saved, f.Cell);
			f.Cell.AddEffect(parent, TimeSpan.FromSeconds(600)); parent.Login();
		}
		f.Environment.Coordinator.Pump();
		Assert.AreEqual(1, f.Environment.Coordinator.ActiveTreatmentCount);
		Assert.AreEqual(12.0, f.Progress.RemainingBudget);
		f.Advance(60);
		Assert.AreEqual(19.0, f.Scar);
		Assert.AreEqual(1L, f.Progress.AcknowledgedSequence);
	}

	[DataTestMethod, DataRow("version"), DataRow("versiontext"), DataRow("budget"), DataRow("pending"), TestCategory("R-I12")]
	public void MalformedLoadedProgress_RemainsInspectableWithoutWork(string malformed)
	{
		var store = new EnvironmentalMagicTestOperationStore();
		XElement saved;
		Guid id;
		using (var first = new Fixture(operations: store))
		{
			var child = first.Install(); id = child.TreatmentId; saved = child.ParentEffect.SaveToXml([]);
		}
		var original = store.Treatments[id];
		if (malformed is "version" or "versiontext") saved.Descendants("Effect").Single(x => x.Element("TreatmentId") is not null)
			.SetAttributeValue("version", malformed == "version" ? "99" : "invalid");
		else store.Treatments[id] = malformed == "budget" ? original with { TotalRepaired = 12 } : original with { Status = LandRejuvenationStatus.Pending };
		using var f = new Fixture(operations: store, scar: null);
		f.Cell.LoadEnvironmentForTest(store.PersistedStates[f.Cell.Id]);
		MagicSpellParent.InitialiseEffectType(); SpellRejuvenateLandEffect.InitialiseEffectType();
		var parent = (MagicSpellParent)Effect.LoadEffect(saved, f.Cell);
		f.Cell.AddEffect(parent, TimeSpan.FromSeconds(600)); parent.Login();
		f.Advance(120);
		Assert.AreEqual(0, f.Environment.Coordinator.ActiveTreatmentCount);
		Assert.AreEqual(20.0, f.Scar);
		Assert.AreEqual(12.0, f.Progress.RemainingBudget);
		Assert.IsFalse(f.Environment.Coordinator.CanInstallTreatment(f.Cell, out _));
	}

	[TestMethod, TestCategory("R-I16")]
	public void FinalCommittedRepair_LostAcknowledgementRemainsCompletedAfterConfirmation()
	{
		using var f = new Fixture(scar: 1);
		f.Install();
		f.Environment.Operations.FailAfterCommit = true;
		f.Advance(60);
		var id = f.Progress.Id;
		f.Environment.Operations.FailAfterCommit = false;
		Assert.IsTrue(f.Environment.Coordinator.ConfirmTreatment(f.Cell, id, out var error), error);
		Assert.AreEqual(0.0, f.Scar);
		Assert.AreEqual(LandRejuvenationStatus.Completed, f.Progress.Status);
		Assert.AreEqual(f.Progress, f.Environment.Operations.Treatments[id]);
		Assert.AreEqual(0, f.Environment.Coordinator.ActiveTreatmentCount);
	}

	[TestMethod, TestCategory("R-I15"), TestCategory("R-I19")]
	public void PreparedRetry_NaturalRecoveryFinishesWithoutMagicReceipt()
	{
		var store = new EnvironmentalMagicTestOperationStore();
		XElement saved;
		EnvironmentalMagicOperationRequest pending;
		using (var first = new Fixture(scar: 2, operations: store))
		{
			var child = first.Install(seconds: 180);
			store.FailAfterClaim = true; first.Advance(60);
			pending = first.Progress.PendingRequest!;
			Assert.IsNotNull(pending);
			saved = child.ParentEffect.SaveToXml([]);
		}
		store.FailAfterClaim = false;
		using var f = new Fixture(scar: null, operations: store);
		f.Cell.LoadEnvironmentForTest(store.PersistedStates[f.Cell.Id]);
		f.Environment.Edit("repair 1"); f.Environment.Coordinator.Pump();
		MagicSpellParent.InitialiseEffectType(); SpellRejuvenateLandEffect.InitialiseEffectType();
		var parent = (MagicSpellParent)Effect.LoadEffect(saved, f.Cell);
		f.Cell.AddEffect(parent, TimeSpan.FromSeconds(120)); parent.Login(); f.Environment.Coordinator.Pump();
		f.Environment.Clock.Advance(TimeSpan.FromSeconds(120));
		parent.ExpireEffect();
		Assert.AreEqual(0.0, f.Scar);
		Assert.AreEqual(0.0, f.Progress.TotalRepaired);
		Assert.AreEqual(0L, f.Progress.AcknowledgedSequence);
		Assert.IsNull(f.Environment.Operations.Find(pending.OperationId));
		Assert.IsTrue(f.Progress.IsTerminal);
	}

	[TestMethod, TestCategory("R-T05")]
	public void Admission_DifferentSpellAndCasterCannotResetExistingBudget()
	{
		using var f = new Fixture();
		f.Install(); f.Advance(60);
		var otherSpell = new Mock<IMagicSpell>();
		otherSpell.SetupGet(x => x.Gameworld).Returns(f.Environment.World.Object);
		otherSpell.SetupGet(x => x.Id).Returns(99L);
		var otherCaster = new Mock<ICharacter>();
		otherCaster.SetupGet(x => x.Gameworld).Returns(f.Environment.World.Object);
		otherCaster.SetupGet(x => x.Id).Returns(99L);
		var effect = new RejuvenateLandEffect(f.Template.SaveToXml(), otherSpell.Object);
		var before = f.Progress;
		Assert.IsFalse(effect.TryPrepareApplication(otherCaster.Object, f.Cell, OpposedOutcomeDegree.Major, SpellPower.Standard,
			TimeSpan.FromSeconds(600), out _, out var error));
		StringAssert.Contains(error, "active or unresolved");
		Assert.AreEqual(before, f.Progress);
	}

	[TestMethod, TestCategory("R-I20")]
	public void RepairCommit_RejectsNestedSameCellLandDebit_AndAllowsOtherCell()
	{
		using var f = new Fixture(count: 2);
		var other = (Cell)f.Environment.Cells.At(1);
		var attempted = false;
		f.Environment.Operations.BeforeCommit = cell =>
		{
			if (cell != f.Cell) return;
			attempted = true;
			Assert.IsFalse(f.Environment.Coordinator.TryApplyLandDebitGroup(f.Cell,
				[new(f.Environment.Resources.At(0), 1)], [], out var ambient, out var native, out var error));
			Assert.AreEqual(0, ambient.Count); Assert.AreEqual(0, native.Count);
			StringAssert.Contains(error, "mutation in progress");
			Assert.IsTrue(f.Environment.Coordinator.ApplyOperation(other, new(Guid.NewGuid(), null, "independent cell", Damage: 1)).Success);
		};
		f.Install(); f.Advance(60);
		Assert.IsTrue(attempted);
		Assert.AreEqual(19.0, f.Scar);
		Assert.AreEqual(1.0, other.EnvironmentState.ScarDamage);
	}

	[TestMethod, TestCategory("R-I20")]
	public void LandDebit_RejectsNestedRepair_WithoutBlockingAnotherCell()
	{
		using var f = new Fixture(count: 2);
		var other = (Cell)f.Environment.Cells.At(1);
		var resource = f.Environment.Resources.At(0);
		f.Environment.Edit("organic source add crop");
		Assert.IsTrue(f.Environment.Coordinator.TryMutateResource(f.Cell, resource, EnvironmentalResourceMutation.Set, 30, out _));
		f.Environment.ResetSavedFlags();
		var attempted = false;
		f.Environment.Saves.Setup(x => x.Add(It.IsAny<MudSharp.Framework.Save.ISaveable>())).Callback<MudSharp.Framework.Save.ISaveable>(item =>
		{
			if (!ReferenceEquals(item, f.Cell) || attempted) return;
			attempted = true;
			var result = f.Environment.Coordinator.ApplyOperation(f.Cell, new(Guid.NewGuid(), null, "nested repair", Repair: 1));
			Assert.IsFalse(result.Success); StringAssert.Contains(result.Error, "mutation in progress");
			Assert.IsTrue(f.Environment.Coordinator.ApplyOperation(other, new(Guid.NewGuid(), null, "other cell", Damage: 1)).Success);
		});
		Assert.IsTrue(f.Environment.Coordinator.TryApplyLandDebitGroup(f.Cell, [new(resource, 1)], [], out var applied, out _, out var error), error);
		Assert.IsTrue(attempted);
		Assert.AreEqual(1, applied.Count);
		Assert.AreEqual(29.0, f.Cell.MagicResourceAmounts[resource]);
		Assert.AreEqual(20.0, f.Scar);
		Assert.AreEqual(1.0, other.EnvironmentState.ScarDamage);
	}

	private sealed class Fixture : IDisposable
	{
		public EnvironmentalMagicTestWorld Environment { get; }
		public Cell Cell => (Cell)Environment.Cells.At(0);
		public double Scar => Cell.EnvironmentState.ScarDamage;
		public LandRejuvenationProgress Progress => Environment.Coordinator.InspectTreatments(Cell).OrderByDescending(x => x.Revision).First();
		public Mock<ICharacter> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public All<ICharacter> Actors { get; } = new();
		public MagicSpell Spell { get; }
		public RejuvenateLandEffect Template { get; }
		public int CostPayments { get; private set; }
		public Fixture(int count = 1, double? scar = 20, EnvironmentalMagicTestOperationStore? operations = null)
		{
			Environment = new(count: count, operations: operations);
			Environment.Edit("output 1 rate 0");
			var school = new Mock<IMagicSchool>(); school.SetupGet(x => x.Id).Returns(1L);
			var trait = new Mock<ITraitDefinition>(); trait.SetupGet(x => x.Id).Returns(1L);
			var schools = new All<IMagicSchool>(); schools.Add(school.Object);
			var traits = new All<ITraitDefinition>(); traits.Add(trait.Object);
			Environment.World.SetupGet(x => x.MagicSchools).Returns(schools);
			Environment.World.SetupGet(x => x.Traits).Returns(traits);
			Environment.World.SetupGet(x => x.Actors).Returns(Actors);
			Environment.World.SetupGet(x => x.LegalAuthorities).Returns(new All<ILegalAuthority>());
			Actor.SetupGet(x => x.Gameworld).Returns(Environment.World.Object);
			Actor.SetupGet(x => x.Id).Returns(10L);
			Actor.SetupGet(x => x.InstanceId).Returns(11L);
			Actor.SetupGet(x => x.Location).Returns(Cell);
			Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Actor.SetupGet(x => x.Body.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1L));
			Actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
			Actor.Setup(x => x.CanUseResource(It.IsAny<IMagicResource>(), It.IsAny<double>())).Returns(true);
			Actor.Setup(x => x.UseResource(It.IsAny<IMagicResource>(), It.IsAny<double>())).Callback(() => CostPayments++).Returns(true);
			Actors.Add(Actor.Object);
			var cost = new Mock<ITraitExpression>(); cost.SetupGet(x => x.Id).Returns(1L);
			cost.Setup(x => x.EvaluateWith(It.IsAny<IHaveTraits>(), It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>(), It.IsAny<(string, object)[]>())).Returns(3.0);
			var expressions = new All<ITraitExpression>(); expressions.Add(cost.Object);
			Environment.World.SetupGet(x => x.TraitExpressions).Returns(expressions);
			var check = new Mock<ICheck>();
			check.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
				It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.CastSpellCheck, Outcome.Pass)));
			Environment.World.Setup(x => x.GetCheck(It.IsAny<CheckType>())).Returns(check.Object);
			Spell = new MagicSpell(new MudSharp.Models.MagicSpell
			{
				Id = 1, Name = "Mend the Land", MagicSchoolId = 1, CastingTraitDefinitionId = 1,
				MinimumSuccessThreshold = (int)Outcome.Pass,
				Definition = "<Spell><NoTrigger/><Costs><Cost resource='1' expression='1'/></Costs><Effects/><CasterEffects/><Plan><Phase/></Plan></Spell>"
			}, Environment.World.Object) { CastingEmote = "casting", FailCastingEmote = "failure", AppliedEffectsAreExclusive = true,
				EffectDurationExpression = new TraitExpression("600", Environment.World.Object) };
			var trigger = new Mock<IMagicTrigger>(); trigger.SetupGet(x => x.TargetTypes).Returns("rooms"); trigger.SetupGet(x => x.TriggerYieldsTarget).Returns(true);
			Spell.Trigger = trigger.Object;
			var plan = new Mock<IInventoryPlan>(); plan.Setup(x => x.PlanIsFeasible()).Returns(InventoryPlanFeasibility.Feasible);
			var planTemplate = new Mock<IInventoryPlanTemplate>(); planTemplate.Setup(x => x.CreatePlan(Actor.Object)).Returns(plan.Object);
			Spell.InventoryPlanTemplate = planTemplate.Object;
			var spells = new All<IMagicSpell>(); spells.Add(Spell);
			Environment.World.SetupGet(x => x.MagicSpells).Returns(spells);
			Template = (RejuvenateLandEffect)SpellEffectFactory.LoadEffectFromBuilderInput("rejuvenateland", new StringStack(""), Spell).Trigger;
			((List<IMagicSpellEffectTemplate>)Spell.SpellEffects).Add(Template);
			if (scar.HasValue) Assert.IsTrue(Environment.Coordinator.ApplyOperation(Cell, new(Guid.NewGuid(), null, "fixture", Damage: scar.Value)).Success);
		}
		public SpellRejuvenateLandEffect Install(double budget = 12, double rate = 1, double seconds = 600)
		{
			Assert.IsTrue(Template.BuildingCommand(Environment.Builder, new StringStack($"budget {budget:R}")));
			Assert.IsTrue(Template.BuildingCommand(Environment.Builder, new StringStack($"rate {rate:R}")));
			Assert.IsTrue(Template.TryPrepareApplication(Actor.Object, Cell, OpposedOutcomeDegree.Moderate, SpellPower.Standard,
				TimeSpan.FromSeconds(seconds), out var application, out var error), error);
			var parent = new MagicSpellParent(Cell, Spell, Actor.Object) { ResolvedDuration = TimeSpan.FromSeconds(seconds) };
			var child = (SpellRejuvenateLandEffect)application!.Create(parent);
			parent.AddSpellEffect(child);
			Cell.AddEffect(child);
			Assert.AreEqual(0, Environment.Coordinator.ActiveTreatmentCount);
			Cell.AddEffect(parent, TimeSpan.FromSeconds(seconds));
			Assert.AreEqual(1, Environment.Coordinator.ActiveTreatmentCount, string.Join(";", Environment.Messages));
			return child;
		}
		public void Advance(double seconds) { Environment.Clock.Advance(TimeSpan.FromSeconds(seconds)); Environment.Coordinator.Pump(); }
		public void Dispose() => Environment.Dispose();
	}
}

internal static class RejuvenationCellTestExtensions
{
	public static void LoadEnvironmentForTest(this Cell cell, EnvironmentalMagicState state)
	{
		var model = new MudSharp.Models.Cell { Id = cell.Id, EnvironmentalState = new MudSharp.Models.CellEnvironmentalState() };
		typeof(Cell).GetMethod("CopyEnvironmentState", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!
			.Invoke(null, [state, model.EnvironmentalState]);
		typeof(Cell).GetMethod("LoadEnvironment", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
			.Invoke(cell, [model]);
	}
}
