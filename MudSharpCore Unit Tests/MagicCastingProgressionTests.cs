using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicCastingProgressionTests
{
	[TestMethod]
	public void Grant_RouteOpeningsAndRetry_PreserveGradeAndExistingSkill()
	{
		var f = new MagicCastingFixture(); f.Skills.Remove(1); f.Skills.Remove(2);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry skill 1 60 90 relative")));
		Assert.IsTrue(f.Sorcerer.BuildingCommand(f.Actor.Object, new StringStack("casting entry skill 1 30 90 relative")));
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "root").Allowed);
		Assert.AreEqual(60.0, f.Skills[1]); Assert.AreEqual(1, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 2, "alternate").Allowed);
		Assert.AreEqual(30.0, f.Skills[2]);
		f.Skills[1] = 77;
		Assert.IsTrue(f.Service.Grant(f.Staff.Object, f.Actor.Object, 1, 1, "retry").Allowed);
		Assert.AreEqual(77.0, f.Skills[1]); Assert.AreEqual(1, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		f.Skills.Remove(1);
		Assert.IsTrue(f.Service.Grant(f.Staff.Object, f.Actor.Object, 1, 1, "repair missing skill").Allowed);
		Assert.AreEqual(60.0, f.Skills[1]);
	}

	[DataTestMethod]
	[DataRow(60.0, 57.0)]
	[DataRow(90.0, 85.5)]
	public void CapRelativeGate_MaximumGrade_IsAttainableAtApprovedCaps(double cap, double threshold)
	{
		var f = new MagicCastingFixture(); f.Acquire(6);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack($"casting entry skill 1 30 {cap} relative")));
		f.Skills[1] = threshold - 0.01;
		Assert.IsFalse(f.Service.Quote(f.Intent(7)).Allowed);
		f.Skills[1] = threshold;
		var result = f.Service.Cast(f.Intent(7));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(7, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		Assert.AreEqual(threshold, f.Skills[1]); Assert.AreEqual(1, f.Samples);
		Assert.IsFalse(f.Service.Quote(f.Intent(8)).Allowed);
	}

	[TestMethod]
	public void LegacyGate_WithoutOptIn_RetainsAbsoluteThresholdAndCosts()
	{
		var f = new MagicCastingFixture(); f.Acquire(6); f.Skills[1] = 90;
		Assert.IsFalse(f.Service.Quote(f.Intent(7)).Allowed);
		f.Skills[1] = 95;
		Assert.AreEqual(52.5, f.Service.Quote(f.Intent(7)).Invocation!.Costs.Single().Amount);
		Assert.IsNull(f.Service.RawSkillImprovementCap(f.Actor.Object, 1));
	}

	[TestMethod]
	public void ImprovementCap_SharedTrait_SelectsHighestEnrolledPermanentRouteWithoutMutations()
	{
		var f = new MagicCastingFixture(); Permanent(f);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry skill 1 30 60 relative")));
		Assert.IsTrue(f.Sorcerer.BuildingCommand(f.Actor.Object, new StringStack("casting entry trait 1 1")));
		Assert.IsTrue(f.Sorcerer.BuildingCommand(f.Actor.Object, new StringStack("casting entry skill 1 30 90 relative")));
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "first").Allowed);
		Assert.AreEqual(60.0, f.Service.RawSkillImprovementCap(f.Actor.Object, 1));
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 2, "second").Allowed);
		Assert.AreEqual(90.0, f.Service.RawSkillImprovementCap(f.Actor.Object, 1));
		f.ActiveCapabilities.Remove(f.Sorcerer);
		Assert.AreEqual(60.0, f.Service.RawSkillImprovementCap(f.Actor.Object, 1));
		f.ActiveCapabilities.Clear(); var writes = f.Store.Writes;
		Assert.AreEqual(0.0, f.Service.RawSkillImprovementCap(f.Actor.Object, 1));
		Assert.AreEqual(writes, f.Store.Writes); Assert.AreEqual(42.0, f.Skills[1]);
		Assert.IsNull(f.Service.RawSkillImprovementCap(f.Actor.Object, 3));
	}

	[TestMethod]
	public void CapMarker_FailedAtomicGrant_DoesNotPersistAcquisitionOrOpenSkill_ExplicitRetryRepairs()
	{
		var f = new MagicCastingFixture(); Permanent(f); f.Skills.Remove(1);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry skill 1 30 60 relative")));
		f.Store.BeforeWrite = op => { if (op?.Stage == MagicCastingStateStore.SkillCapRecorded) throw new InvalidOperationException("cap marker unavailable"); };
		Assert.IsFalse(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "capped route").Allowed);
		Assert.IsNull(f.Service.Acquisition(f.Actor.Object, 1)); Assert.IsFalse(f.Skills.ContainsKey(1));
		f.ActiveCapabilities.Clear(); f.Restart();
		Assert.IsNull(f.Service.RawSkillImprovementCap(f.Actor.Object, 1));
		f.ActiveCapabilities.Add(f.Earth); f.Store.BeforeWrite = null;
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "retry capped route").Allowed);
		Assert.AreEqual(30.0, f.Skills[1]); Assert.AreEqual(60.0, f.Service.RawSkillImprovementCap(f.Actor.Object, 1));
	}

	[TestMethod]
	public void CapMarker_StaffReconciliation_CannotEraseTerminalHistory()
	{
		var f = new MagicCastingFixture(); Permanent(f);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry skill 1 30 60 relative")));
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "capped route").Allowed);
		var marker = f.Store.Operations.Values.Single(); var writes = f.Store.Writes;
		Assert.IsTrue(f.Service.ReconcileOperation(f.Staff.Object, f.Actor.Object, marker.Id, "terminal acknowledgement").Allowed);
		Assert.AreEqual(writes, f.Store.Writes);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry remove 1")));
		f.Restart();
		Assert.AreEqual(0.0, f.Service.RawSkillImprovementCap(f.Actor.Object, 1));
		Assert.AreEqual(MagicCastingStateStore.SkillCapRecorded, f.Store.Operation(marker.Id)!.Stage);
	}

	[TestMethod]
	public void ImprovementCap_UntouchedLegacyRoute_SharedWithDisabledCappedPolicy_RetainsNativeGains()
	{
		var f = new MagicCastingFixture();
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry skill 1 30 60 relative")));
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting enable off")));
		Assert.IsTrue(f.Sorcerer.BuildingCommand(f.Actor.Object, new StringStack("casting entry trait 1 1")));
		Assert.IsTrue(f.Service.Grant(f.Staff.Object, f.Actor.Object, 2, 1, "legacy grant").Allowed);
		Assert.IsNull(f.Service.RawSkillImprovementCap(f.Actor.Object, 1));
		var skill = ImprovingSkill(f, 59);
		Assert.IsTrue(skill.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []));
		Assert.AreEqual(64.0, skill.RawValue);
	}

	[DataTestMethod]
	[DataRow("casting entry remove 1")]
	[DataRow("casting entry trait 1 2")]
	[DataRow("casting entry skill 1 default")]
	public void ImprovementCap_RemovedOrReboundAdmission_SurvivesReloadAndPreservesHistory(string edit)
	{
		var f = new MagicCastingFixture(); Permanent(f);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry skill 1 30 60 relative")));
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "capped route").Allowed);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack(edit)));
		// Clearing the authored cap deliberately restores an enrolled uncapped route.
		if (edit.EndsWith("default"))
		{
			Assert.IsNull(f.Service.RawSkillImprovementCap(f.Actor.Object, 1));
			return;
		}
		f.Restart(); var writes = f.Store.Writes;
		Assert.AreEqual(0.0, f.Service.RawSkillImprovementCap(f.Actor.Object, 1));
		var skill = ImprovingSkill(f, 80); skill.Value += 5;
		Assert.IsFalse(skill.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []));
		Assert.AreEqual(80.0, skill.RawValue); Assert.AreEqual(writes, f.Store.Writes);
		Assert.AreEqual(0, f.Store.Unresolved().Count);
	}

	[TestMethod]
	public void NativeSkill_ActualImprovement_ClampsToRemainingRouteAllowance()
	{
		var f = new MagicCastingFixture(); Permanent(f);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry skill 1 30 60 relative")));
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "capped route").Allowed);
		var skill = ImprovingSkill(f, 59);
		Assert.IsTrue(skill.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []));
		Assert.AreEqual(60.0, skill.RawValue);
		Assert.IsFalse(skill.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []));
	}

	[TestMethod]
	public void NativeSkill_PositiveWritesAndLostEntitlement_ClampGainsAndRetainHistory()
	{
		var f = new MagicCastingFixture(); Permanent(f);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry skill 1 30 60 relative")));
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "first").Allowed);
		var definition = new SkillDefinition(new MudSharp.Models.TraitDefinition { Id = 1, Name = "Native",
			Type = (int)TraitType.Skill, OwnerScope = (int)TraitOwnerScope.Character }, f.World.Object)
			{ Cap = new TraitExpression("100", f.World.Object) };
		var skill = new Skill(definition, 59, f.Actor.Object);
		skill.Value += 5;
		Assert.AreEqual(60.0, skill.RawValue);
		f.ActiveCapabilities.Clear(); skill.Value += 5;
		Assert.AreEqual(60.0, skill.RawValue);
		var historic = new Skill(definition, 80, f.Actor.Object); historic.Value += 5;
		Assert.AreEqual(80.0, historic.RawValue);
		Assert.IsFalse(historic.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []));
		Assert.AreEqual(80.0, historic.RawValue);
	}

	[DataTestMethod]
	[DataRow(1, 50.0)]
	[DataRow(2, 25.0)]
	[DataRow(7, 7.0)]
	public void Efficiency_QuoteAndCommit_ReplaceLinearSourceCostOnce(int mastery, double expected)
	{
		var f = new MagicCastingFixture(); f.Acquire(mastery);
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades efficiency source 7 1")));
		var intent = f.Intent(1, false);
		Assert.AreEqual(expected, f.Service.Quote(intent).Invocation!.Costs.Single().Amount);
		var result = f.Service.Cast(intent);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(100 - expected, f.Balances[f.Resources[1]]);
		Assert.AreEqual(0, f.Samples); Assert.AreEqual(1, f.Rolls);
	}

	[TestMethod]
	public void Efficiency_OverreachAndSecondarySameReserve_AggregateConfiguredPenaltyOnce()
	{
		var f = new MagicCastingFixture(); f.Acquire(1);
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades efficiency source 7 1")));
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades overreach 1 1")));
		var xml = XElement.Parse(f.Spell.SnapshotModel().Definition);
		xml.Element("Costs")!.Add(new XElement("Cost", new XAttribute("resource", 11), new XAttribute("expression", 1)));
		var model = f.Spell.SnapshotModel(); model.Definition = xml.ToString(); f.Spells[0] = new MagicSpell(model, f.World.Object);
		Assert.AreEqual(85.0, f.Service.Quote(f.Intent(2)).Invocation!.Costs.Single().Amount);
		Assert.IsTrue(f.Spells[0].BuildingCommand(f.Actor.Object, new StringStack("grades overreach 1.5 1")));
		Assert.IsFalse(f.Service.Quote(f.Intent(2)).Allowed); // 112.5 source + 10 fixed secondary exceeds 100.
		f.Balances[f.Resources[1]] = 200;
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(200);
		Assert.AreEqual(122.5, f.Service.Quote(f.Intent(2)).Invocation!.Costs.Single().Amount);
	}

	[DataTestMethod]
	[DataRow("casting entry skill 1 61 60 relative")]
	[DataRow("casting entry skill 1 30 0 relative")]
	[DataRow("casting entry skill 1 NaN 90 relative")]
	public void Admission_InvalidSkillPolicy_RefusesBeforePayment(string command)
	{
		var f = new MagicCastingFixture(); f.Acquire();
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack(command)));
		Assert.IsTrue(f.Earth.CastingConfigurationErrors().Any());
		Assert.IsFalse(f.Service.Quote(f.Intent()).Allowed);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls);
	}

	[TestMethod]
	public void AdmissionSkillPolicy_XmlRoundTrip_RetainsOpeningsCapScaleAndIdentity()
	{
		var f = new MagicCastingFixture();
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry skill 1 60 90 relative")));
		var xml = f.Earth.SaveToXml();
		var reload = f.NewCapability(3, 1, 11, true, XElement.Parse(xml).Element("Casting"));
		Assert.AreEqual(f.Earth.CastingPolicy!.Identity, reload.CastingPolicy!.Identity);
		var admission = reload.CastingPolicy.Admissions.Single();
		Assert.AreEqual(60.0, admission.OpeningSkill); Assert.AreEqual(90.0, admission.RawSkillCap);
		Assert.IsTrue(admission.CapRelativeProficiency);
		Assert.AreEqual(f.Earth.CastingPolicy.Admissions.Single().Key, admission.Key);
		Assert.AreEqual(0, reload.CastingConfigurationErrors().Count);
	}

	[TestMethod]
	public void EfficiencyAndMasteryNumerics_ReloadAndParallelCopies_RetainExplicitIndependentInputs()
	{
		var f = new MagicCastingFixture();
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades efficiency source 7 2")));
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades scalar add target 0 boost Bonus -mastery")));
		var reload = new MagicSpell(f.Spell.SnapshotModel(), f.World.Object);
		Assert.AreEqual(new ControlledSpellEfficiency(7, 2), reload.GradeProfile!.Efficiency);
		System.Threading.Tasks.Parallel.For(1, 8, mastery =>
		{
			var copy = reload.CastingCopy(f.Actor.Object, f.Traits[0], 1, SpellPower.ExtremelyWeak, Difficulty.Normal, mastery);
			Assert.AreEqual(-mastery, ((MudSharp.Magic.SpellEffects.TraitBoostEffect)copy.SpellEffects.Single()).Bonus);
		});
		Assert.AreEqual(0.0, ((MudSharp.Magic.SpellEffects.TraitBoostEffect)reload.SpellEffects.Single()).Bonus);
	}

	private static Skill ImprovingSkill(MagicCastingFixture f, double value)
	{
		var improver = new Mock<IImprovementModel>(); improver.SetupGet(x => x.Id).Returns(99);
		improver.Setup(x => x.GetImprovement(It.IsAny<IHaveTraits>(), It.IsAny<ITrait>(), It.IsAny<Difficulty>(),
			It.IsAny<Outcome>(), It.IsAny<TraitUseType>())).Returns(5);
		f.World.SetupGet(x => x.ImprovementModels).Returns(MagicCastingFixture.Collection(() => new[] { improver.Object }));
		var definition = new SkillDefinition(new MudSharp.Models.TraitDefinition { Id = 1, Name = "Native", ImproverId = 99,
			Type = (int)TraitType.Skill, OwnerScope = (int)TraitOwnerScope.Character }, f.World.Object)
			{ Cap = new TraitExpression("100", f.World.Object) };
		return new Skill(definition, value, f.Actor.Object);
	}

	private static void Permanent(MagicCastingFixture f)
	{
		var merit = new Mock<IMagicCapabilityMerit>();
		merit.Setup(x => x.Applies(f.Actor.Object)).Returns(true);
		merit.SetupGet(x => x.Capabilities).Returns(() => f.ActiveCapabilities);
		f.Actor.SetupGet(x => x.Merits).Returns([merit.Object]);
	}
}
