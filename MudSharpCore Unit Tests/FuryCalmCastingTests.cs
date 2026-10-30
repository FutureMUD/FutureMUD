#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Character.Heritage;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Emotions;
using MudSharp.Magic.SpellEffects;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class FuryCalmCastingTests
{
	[TestMethod]
	[DataRow(1, 2400)]
	[DataRow(7, 16800)]
	public void PaidFury_UsesCasterTerrainAndExplicitGrade_ProducesOwnedScheduledAttributeBonus(int grade, int seconds)
	{
		using var f = new Fixture(EmotionalSpellKind.Fury);
		var result = f.Cast(grade); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var head = f.Parents.Single(); var child = (SpellSourceFuryEffect)head.SpellEffects.Single();
		Assert.AreEqual(grade, child.State.SourceGrade); Assert.AreEqual(5.0, child.State.Intensity);
		Assert.AreEqual(TimeSpan.FromSeconds(seconds), f.Scheduler.RemainingDuration(head));
		Assert.AreSame(f.Attribute, child.EnduranceTrait); Assert.AreEqual(0.5, child.UnitsPerSourcePoint);
		Assert.AreEqual(head.Power, child.State.Power); Assert.IsTrue(child.AppliesToTrait(f.Attribute));
		Assert.IsTrue(child.State.EndurancePoints >= 2 * grade && child.State.EndurancePoints <= Math.Max(grade, grade * grade / 2) + grade);
		Assert.AreEqual(1, f.F.Rolls); Assert.AreEqual(0, f.SaveCalls); Assert.IsTrue(f.Applied(result));
		Assert.IsTrue(f.F.Balances[f.F.Resources[1]] < 100.0); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[0]]);
	}

	[TestMethod]
	public void FuryRecast_RetainsExistingPowerModifierAndParent_AccumulatesToCapWithoutTransientRemoval()
	{
		using var f = new Fixture(EmotionalSpellKind.Fury);
		var old = f.Existing(EmotionalSpellKind.Fury, 2, 21000, SpellPower.ExtremelyWeak, 17);
		var child = (SpellSourceFuryEffect)old.SpellEffects.Single(); var state = child.State;
		var result = f.Cast(7); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreSame(old, f.Parents.Single()); Assert.AreSame(child, old.SpellEffects.Single()); Assert.AreEqual(state, child.State);
		Assert.AreEqual(SpellPower.ExtremelyWeak, old.Power); Assert.AreEqual(TimeSpan.FromSeconds(21600), f.Scheduler.RemainingDuration(old));
		Assert.AreEqual(0, f.EmotionalRemovals); Assert.IsTrue(f.Applied(result));
		var capped = f.Cast(7); Assert.AreEqual(MagicCastingStatus.Succeeded, capped.Status, capped.Message);
		Assert.IsFalse(f.Applied(capped), "An unchanged capped deadline is not a useful mastery operation.");
	}

	[TestMethod]
	public void ConfiguredFury_AddsSeparateNativeFrenzyOnlyForMatchingUnsuppressedRace()
	{
		using var eligible = new Fixture(EmotionalSpellKind.Fury); eligible.ConfigureFrenzy();
		var result = eligible.Cast(1); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var frenzy = eligible.Handler.Effects.OfType<CombatFrenzyEffect>().Single();
		Assert.AreEqual(15.0, frenzy.OffenceBonus); Assert.AreEqual(15.0, frenzy.DefenceBonus);
		Assert.AreEqual(5.0, frenzy.MeleeDamageBonus); Assert.AreEqual(4.0, frenzy.HungerMultiplier);
		Assert.AreEqual(eligible.Scheduler.RemainingDuration(eligible.Parents.Single()), eligible.Scheduler.RemainingDuration(frenzy));

		using var nonrace = new Fixture(EmotionalSpellKind.Fury); nonrace.ConfigureFrenzy();
		nonrace.F.Body.SetupGet(x => x.Race).Returns(Mock.Of<IRace>(x => x.Id == 202));
		Assert.AreEqual(MagicCastingStatus.Succeeded, nonrace.Cast(1).Status); Assert.IsFalse(nonrace.Handler.Effects.OfType<CombatFrenzyEffect>().Any());

		using var suppressed = new Fixture(EmotionalSpellKind.Fury); suppressed.ConfigureFrenzy(); suppressed.AddSuppressionDose(active: true);
		Assert.AreEqual(MagicCastingStatus.Succeeded, suppressed.Cast(1).Status); Assert.IsFalse(suppressed.Handler.Effects.OfType<CombatFrenzyEffect>().Any());

		using var latentSuppressed = new Fixture(EmotionalSpellKind.Fury); latentSuppressed.ConfigureFrenzy(); latentSuppressed.AddSuppressionDose(active: false);
		Assert.AreEqual(MagicCastingStatus.Succeeded, latentSuppressed.Cast(1).Status); Assert.IsFalse(latentSuppressed.Handler.Effects.OfType<CombatFrenzyEffect>().Any());
	}

	[TestMethod]
	public void LegacyEmotionalProfile_UnrelatedDrugMutationDoesNotInvalidatePreparedCast()
	{
		using var f = new Fixture(EmotionalSpellKind.Fury);
		var unrelatedDrug = Mock.Of<IDrug>(x => x.Id == 999);
		var dose = new DrugDosage { Drug = unrelatedDrug, Grams = 1.0, OriginalVector = DrugVector.Ingested,
			Originator = f.F.Actor.Object, SpellOriginFamily = string.Empty, SpellOriginFormula = string.Empty };
		f.F.Checkpoint = stage =>
		{
			if (stage == "Paying") f.F.Body.SetupGet(x => x.ActiveDrugDosages).Returns(new[] { dose });
		};
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.IsFalse(f.Handler.Effects.OfType<CombatFrenzyEffect>().Any());
	}

	[TestMethod]
	public void ConfiguredFuryRecast_ExtendsIndependentFrenzyDeadlineByResolvedFuryDuration()
	{
		using var f = new Fixture(EmotionalSpellKind.Fury); f.ConfigureFrenzy();
		var frenzy = new CombatFrenzyEffect(f.F.Actor.Object, 15, 15, 5, 4);
		f.Handler.AddEffect(frenzy, TimeSpan.FromSeconds(100));
		f.Existing(EmotionalSpellKind.Fury, 2, 1000);
		var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreSame(frenzy, f.Handler.Effects.OfType<CombatFrenzyEffect>().Single());
		Assert.AreEqual(TimeSpan.FromSeconds(3700), f.Scheduler.RemainingDuration(frenzy));
		Assert.AreEqual(TimeSpan.FromSeconds(3600), f.Scheduler.RemainingDuration(f.Parents.Single()));
	}

	[TestMethod]
	public void CappedFuryRecast_ReportsAppliedWhenOnlyIndependentFrenzyExtends()
	{
		using var f = new Fixture(EmotionalSpellKind.Fury); f.ConfigureFrenzy();
		var old = f.Existing(EmotionalSpellKind.Fury, 7, 21600);
		var frenzy = new CombatFrenzyEffect(f.F.Actor.Object, 15, 15, 5, 4);
		f.Handler.AddEffect(frenzy, TimeSpan.FromSeconds(100));
		var result = f.Cast(7); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreSame(old, f.Parents.Single()); Assert.AreEqual(TimeSpan.FromSeconds(21600), f.Scheduler.RemainingDuration(old));
		Assert.AreEqual(TimeSpan.FromSeconds(21700), f.Scheduler.RemainingDuration(frenzy));
		Assert.IsTrue(f.Applied(result), "Extending the independent frenzy is a useful applied effect even at the Fury cap.");
	}

	[TestMethod]
	public void ConfiguredCalm_SavedCastClearsFrenzyButExhaustedCounterDoesNot()
	{
		using var saved = new Fixture(EmotionalSpellKind.Calm); saved.ConfigureFrenzy(); saved.Resistance = Outcome.MajorPass;
		var frenzy = new CombatFrenzyEffect(saved.F.Actor.Object, 15, 15, 5, 4); saved.Handler.AddEffect(frenzy, TimeSpan.FromSeconds(900));
		var result = saved.Cast(3); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.IsFalse(saved.Handler.Effects.OfType<CombatFrenzyEffect>().Any()); Assert.IsFalse(saved.Parents.Any());

		using var exhausted = new Fixture(EmotionalSpellKind.Calm); exhausted.ConfigureFrenzy();
		var retained = new CombatFrenzyEffect(exhausted.F.Actor.Object, 15, 15, 5, 4); exhausted.Handler.AddEffect(retained, TimeSpan.FromSeconds(900));
		exhausted.Existing(EmotionalSpellKind.Fury, 7, 1200);
		var exhaustedResult = exhausted.Cast(3); Assert.AreEqual(MagicCastingStatus.Succeeded, exhaustedResult.Status, exhaustedResult.Message);
		Assert.AreEqual(0, exhausted.SaveCalls); Assert.AreSame(retained, exhausted.Handler.Effects.OfType<CombatFrenzyEffect>().Single());
	}

	[TestMethod]
	public void ConfiguredCalm_CallbackReplacementFrenzyIsPreservedAndQuarantined()
	{
		using var f = new Fixture(EmotionalSpellKind.Calm); f.ConfigureFrenzy(); f.Resistance = Outcome.MajorPass;
		var old = new CombatFrenzyEffect(f.F.Actor.Object, 15, 15, 5, 4); f.Handler.AddEffect(old, TimeSpan.FromSeconds(900));
		CombatFrenzyEffect? replacement = null;
		f.FrenzyRemovalCallback = () =>
		{
			replacement = new CombatFrenzyEffect(f.F.Actor.Object, 1, 2, 3, 1);
			f.Handler.AddEffect(replacement, TimeSpan.FromSeconds(600));
		};
		var result = f.Cast(3); Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.IsFalse(f.Handler.Effects.Contains(old)); Assert.IsNotNull(replacement);
		Assert.IsTrue(f.Handler.Effects.Contains(replacement)); Assert.AreEqual(1.0, replacement!.OffenceBonus);
		Assert.IsFalse(f.Parents.Any());
	}

	[TestMethod]
	[DataRow(EmotionalSpellKind.Fury, 3, 5, 2)]
	[DataRow(EmotionalSpellKind.Calm, 3, 5, 2)]
	public void StrongerOpposingEmotion_IsReducedWithoutSaveNewChildOrCessation(EmotionalSpellKind incoming, int grade, int opposingGrade, int remaining)
	{
		using var f = new Fixture(incoming);
		var old = f.Existing(incoming == EmotionalSpellKind.Fury ? EmotionalSpellKind.Calm : EmotionalSpellKind.Fury, opposingGrade, 1200,
			SpellPower.ExtremelyWeak, 13);
		var prior = ((SpellEmotionalEffect)old.SpellEffects.Single()).State;
		var result = f.Cast(grade); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var child = (SpellEmotionalEffect)old.SpellEffects.Single();
		Assert.AreSame(old, f.Parents.Single()); Assert.AreEqual(prior with { SourceGrade = remaining }, child.State);
		Assert.AreEqual(TimeSpan.FromSeconds(1200), f.Scheduler.RemainingDuration(old));
		Assert.AreEqual(0, f.SaveCalls); Assert.IsTrue(f.Applied(result));
	}

	[TestMethod]
	[DataRow(EmotionalSpellKind.Fury)]
	[DataRow(EmotionalSpellKind.Calm)]
	public void EqualCounter_RemovesOnlyAdmittedOpposingParent_NoSaveOrNewEffect(EmotionalSpellKind incoming)
	{
		using var f = new Fixture(incoming); var old = f.Existing(incoming == EmotionalSpellKind.Fury ? EmotionalSpellKind.Calm : EmotionalSpellKind.Fury, 4, 1200);
		var unrelated = new MagicSpellParent(f.F.Actor.Object, f.F.Spell, f.F.Actor.Object);
		var unrelatedChild = new SpellDetectInvisibleEffect(f.F.Actor.Object, unrelated); unrelated.AddSpellEffect(unrelatedChild);
		f.Handler.AddEffect(unrelatedChild); f.Handler.AddEffect(unrelated, TimeSpan.FromSeconds(900));
		var result = f.Cast(4); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.IsFalse(f.Handler.Effects.Contains(old)); Assert.IsFalse(f.Scheduler.IsScheduled(old)); Assert.AreEqual(0, f.SaveCalls);
		Assert.AreSame(unrelated, f.Parents.Single()); Assert.IsTrue(f.Handler.Effects.Contains(unrelatedChild)); Assert.IsTrue(f.Applied(result));
	}

	[TestMethod]
	public void PositiveCalmResidual_UsesResidualGradeSaveAndOriginalCasterRoll_IncludingSelf()
	{
		using var f = new Fixture(EmotionalSpellKind.Calm);
		f.Existing(EmotionalSpellKind.Fury, 2, 1200);
		Assert.IsTrue(f.Template.BuildingCommand(f.F.Actor.Object, new StringStack("save 5 Hard")));
		var result = f.Cast(7); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, f.F.Rolls); Assert.AreEqual(1, f.SaveCalls); Assert.AreEqual(Difficulty.Hard, f.SaveDifficulty);
		var calm = (SpellSourceCalmEffect)f.Parents.Single().SpellEffects.Single(); Assert.AreEqual(5, calm.State.SourceGrade);
		Assert.AreEqual(TimeSpan.FromSeconds(6000), f.Scheduler.RemainingDuration(f.Parents.Single())); Assert.IsTrue(calm.BreakOnAdmittedAttack);
	}

	[TestMethod]
	public void ResistedCalm_WithNoUsefulOperation_IsPaidFailureAndHasNoChild()
	{
		using var f = new Fixture(EmotionalSpellKind.Calm); f.Resistance = Outcome.MajorPass; f.F.Outcome = Outcome.MinorPass;
		var result = f.Cast(3); Assert.AreEqual(MagicCastingStatus.Failed, result.Status, result.Message);
		Assert.AreEqual(1, f.SaveCalls); Assert.AreEqual(1, f.F.Rolls); Assert.IsFalse(f.Parents.Any()); Assert.IsFalse(f.Applied(result));
		Assert.IsTrue(f.F.Balances[f.F.Resources[1]] < 100.0);
	}

	[TestMethod]
	public void UnsupportedCalmCessation_RefusesBeforeJournalDebitOrRoll()
	{
		using var f = new Fixture(EmotionalSpellKind.Calm); f.F.Actor.SetupGet(x => x.Combat).Returns(Mock.Of<ICombat>());
		var writes = f.F.Store.Writes; var result = f.Cast(3); Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		StringAssert.Contains(result.Message, "selective combat cessation"); Assert.AreEqual(writes, f.F.Store.Writes);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(0, f.SaveCalls); Assert.AreEqual(0, f.F.Rolls);
	}

	[TestMethod]
	public void ExhaustedCalmCounter_DoesNotRequireUnsupportedCessation()
	{
		using var f = new Fixture(EmotionalSpellKind.Calm); f.F.Actor.SetupGet(x => x.Combat).Returns(Mock.Of<ICombat>());
		f.Existing(EmotionalSpellKind.Fury, 7, 1200);
		var result = f.Cast(3); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(0, f.SaveCalls); Assert.IsTrue(f.Applied(result));
	}

	[TestMethod]
	public void ResistedCalm_PerformsOnlyItsPreparedCessation_ReportsObservedRemoval()
	{
		using var f = new Fixture(EmotionalSpellKind.Calm); f.Resistance = Outcome.MajorPass; f.F.Outcome = Outcome.MinorPass;
		var combat = new Mock<ICombat>(); var selective = combat.As<ICombatSelectiveCessation>();
		var ticket = Mock.Of<ICombatCessationAdmission>(x => x.IsCurrent);
		f.F.Actor.SetupProperty(x => x.Combat, combat.Object);
		selective.Setup(x => x.PrepareCessation(f.F.Actor.Object, null, combat.Object)).Returns(ticket);
		selective.Setup(x => x.CeaseCombatFor(ticket)).Returns(() => { f.F.Actor.Object.Combat = null!; return CombatCessationChanges.SubjectRemoved; });
		var result = f.Cast(3); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.IsTrue(f.Applied(result)); Assert.IsFalse(f.Parents.Any()); Assert.IsNull(f.F.Actor.Object.Combat);
		selective.Verify(x => x.CeaseCombatFor(ticket), Times.Once); combat.Verify(x => x.EndCombat(It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	[DataRow("BeforePayment", MagicCastingStatus.Refused)]
	[DataRow("Paying", MagicCastingStatus.NeedsReview)]
	public void DeadlineChangedAfterPreparation_PreservesReplacementScheduleAndNeverDebits(string checkpoint, MagicCastingStatus status)
	{
		using var f = new Fixture(EmotionalSpellKind.Fury); var old = f.Existing(EmotionalSpellKind.Fury, 2, 1200);
		f.F.Checkpoint = stage => { if (stage == checkpoint) f.Scheduler.Reschedule(old, TimeSpan.FromSeconds(2400)); };
		var result = f.Cast(3); Assert.AreEqual(status, result.Status, result.Message);
		Assert.AreSame(old, f.Parents.Single()); Assert.AreEqual(TimeSpan.FromSeconds(2400), f.Scheduler.RemainingDuration(old));
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(0, f.F.Rolls);
	}

	[TestMethod]
	public void SaveCallback_ReplacesCombatAfterPayment_QuarantinesAndNeverUsesStaleCessation()
	{
		using var f = new Fixture(EmotionalSpellKind.Calm); var combat = new Mock<ICombat>(); var selective = combat.As<ICombatSelectiveCessation>();
		var ticket = Mock.Of<ICombatCessationAdmission>(x => x.IsCurrent); var next = Mock.Of<ICombat>();
		f.F.Actor.SetupProperty(x => x.Combat, combat.Object);
		selective.Setup(x => x.PrepareCessation(f.F.Actor.Object, null, combat.Object)).Returns(ticket);
		f.SaveCallback = () => f.F.Actor.Object.Combat = next;
		var result = f.Cast(3); Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreSame(next, f.F.Actor.Object.Combat); Assert.IsFalse(f.Parents.Any()); Assert.IsTrue(f.F.Balances[f.F.Resources[1]] < 100.0);
		selective.Verify(x => x.CeaseCombatFor(It.IsAny<ICombatCessationAdmission>()), Times.Never);
	}

	[TestMethod]
	public void AdmittedIncomingMiss_BreaksOnlyOwnedCalmChild_PreservesSiblingAndParent()
	{
		using var f = new Fixture(EmotionalSpellKind.Calm); Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(3).Status);
		var head = f.Parents.Single(); var calm = (SpellSourceCalmEffect)head.SpellEffects.Single();
		var sibling = new SpellDetectInvisibleEffect(f.F.Actor.Object, head); head.AddSpellEffect(sibling); f.Handler.AddEffect(sibling);
		Assert.IsFalse(HostileAttackAdmission.TryNotify(f.F.Actor.Object, f.F.Actor.Object, () => false));
		Assert.IsTrue(f.Handler.Effects.Contains(calm), "A refused attempt must not break Calm.");
		Assert.IsTrue(HostileAttackAdmission.TryNotify(f.F.Actor.Object, f.F.Actor.Object));
		Assert.IsFalse(f.Handler.Effects.Contains(calm)); Assert.IsTrue(f.Handler.Effects.Contains(sibling));
		Assert.AreSame(head, f.Parents.Single()); Assert.AreSame(sibling, head.SpellEffects.Single()); Assert.IsTrue(f.Scheduler.IsScheduled(head));
	}

	[TestMethod]
	[DataRow(EmotionalSpellKind.Fury)]
	[DataRow(EmotionalSpellKind.Calm)]
	public void ParentAndChildReload_RetainsSourceStatePowerOwnershipAndRemainingDeadline(EmotionalSpellKind kind)
	{
		using var f = new Fixture(kind); var result = f.Cast(7); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var head = f.Parents.Single(); var state = ((SpellEmotionalEffect)head.SpellEffects.Single()).State; var identity = head.Identity;
		f.Clock.Advance(599); var remaining = f.Scheduler.RemainingDuration(head);
		var xml = head.SaveToXml(new Dictionary<IEffect, TimeSpan> { [head] = remaining }); f.Handler.RemoveEffect(head, true);
		MagicSpellParent.InitialiseEffectType(); SpellSourceCalmEffect.InitialiseEffectType(); SpellSourceFuryEffect.InitialiseEffectType();
		var loaded = (MagicSpellParent)Effect.LoadEffect(xml, f.F.Actor.Object); f.Handler.AddEffect(loaded, remaining);
		Assert.AreEqual(identity, loaded.Identity); Assert.AreEqual(state.Power, loaded.Power);
		var child = (SpellEmotionalEffect)loaded.SpellEffects.Single(); Assert.AreSame(loaded, child.ParentEffect);
		Assert.AreEqual(state, child.State); Assert.IsNull(child.DefinitionError); Assert.IsTrue(f.Handler.Effects.Contains(child));
		Assert.AreEqual(remaining, f.Scheduler.RemainingDuration(loaded));
		var recast = f.Cast(1); Assert.AreEqual(MagicCastingStatus.Succeeded, recast.Status, recast.Message);
		f.Handler.RemoveEffect(f.Parents.Single(), true); Assert.IsFalse(f.Parents.Any()); Assert.IsFalse(f.Handler.Effects.OfType<SpellEmotionalEffect>().Any());
	}

	[TestMethod]
	public void ReflectedFury_UsesPreparedCasterCohort_GrantsNoIntendedOperationProgress()
	{
		using var f = new Fixture(EmotionalSpellKind.Fury); f.ConfigureFrenzy(); f.Existing(EmotionalSpellKind.Calm, 2, 1200);
		var target = new Mock<MudSharp.Character.ICharacter>() { DefaultValue = DefaultValue.Mock };
		var body = new Mock<MudSharp.Body.IBody>() { DefaultValue = DefaultValue.Mock };
		body.SetupGet(x => x.Race).Returns(Mock.Of<IRace>(x => x.Id == 202));
		target.SetupGet(x => x.Id).Returns(401); target.SetupGet(x => x.InstanceId).Returns(401);
		target.SetupGet(x => x.Gameworld).Returns(f.F.World.Object); target.SetupGet(x => x.Body).Returns(body.Object);
		body.SetupGet(x => x.Actor).Returns(target.Object); body.SetupGet(x => x.Gameworld).Returns(f.F.World.Object);
		target.SetupGet(x => x.Location).Returns(f.F.Actor.Object.Location); target.SetupGet(x => x.Combat).Returns((ICombat)null!);
		target.SetupGet(x => x.CombatTarget).Returns((IPerceiver)null!);
		var effects = new EffectHandler(target.Object); target.SetupGet(x => x.Effects).Returns(() => effects.Effects);
		var ward = new Mock<IMagicInterdictionEffect>(); ward.SetupGet(x => x.Owner).Returns(target.Object);
		ward.SetupGet(x => x.Mode).Returns(MagicInterdictionMode.Reflect); ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming);
		ward.Setup(x => x.ShouldInterdict(f.F.Actor.Object, f.F.School)).Returns(true);
		effects.AddEffect(ward.Object);
		target.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns(new[] { ward.Object });
		f.F.Actor.Setup(x => x.TargetActorOrCorpse("self", It.IsAny<PerceiveIgnoreFlags>())).Returns(target.Object);
		f.F.Actor.Setup(x => x.CanSee(target.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		var result = f.Cast(7); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var child = (SpellSourceFuryEffect)f.Parents.Single().SpellEffects.Single(); Assert.AreEqual(5, child.State.SourceGrade);
		Assert.AreEqual(TimeSpan.FromSeconds(12000), f.Scheduler.RemainingDuration(f.Parents.Single()));
		Assert.IsFalse(effects.Effects.OfType<SpellEmotionalEffect>().Any()); Assert.IsTrue(effects.Effects.Contains(ward.Object));
		Assert.AreEqual(1, f.Handler.Effects.OfType<CombatFrenzyEffect>().Count());
		Assert.IsFalse(f.Applied(result)); Assert.AreEqual(1, f.F.Rolls);
	}

	[TestMethod]
	public void CalmCappedRecast_PreservesParentAndReportsNoChange()
	{
		using var f = new Fixture(EmotionalSpellKind.Calm);
		var old = f.Existing(EmotionalSpellKind.Calm, 7, 14400, f.Spell.GradeProfile!.Grades.Single(x => x.Grade == 7).Power);
		var result = f.Cast(7); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreSame(old, f.Parents.Single()); Assert.IsFalse(f.Applied(result)); Assert.AreEqual(1, f.SaveCalls);
	}

	[TestMethod]
	public void CalmRecast_RetainsStrongerSourcePowerAndStacksRemainingUnits()
	{
		using var f = new Fixture(EmotionalSpellKind.Calm); var old = f.Existing(EmotionalSpellKind.Calm, 7, 6001, SpellPower.ExtremelyWeak);
		var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var head = f.Parents.Single(); Assert.AreNotSame(old, head); Assert.IsFalse(f.Scheduler.IsScheduled(old));
		var child = (SpellSourceCalmEffect)head.SpellEffects.Single(); Assert.AreEqual(7, child.State.SourceGrade);
		Assert.AreEqual(SpellPower.ExtremelyWeak, child.State.Power); Assert.AreEqual(child.State.Power, head.Power);
		Assert.AreEqual(TimeSpan.FromSeconds(7800), f.Scheduler.RemainingDuration(head));
	}

	[TestMethod]
	public void TargetOutputCallback_ChangesRetainedSourceState_PreservesItWithoutCountering()
	{
		using var f = new Fixture(EmotionalSpellKind.Calm); var head = f.Existing(EmotionalSpellKind.Fury, 7, 1200);
		var fury = (SpellSourceFuryEffect)head.SpellEffects.Single();
		f.Spell.TargetEmote = "target echo";
		f.F.Actor.SetupGet(x => x.Corpse).Returns(() => null!);
		Mock.Get(f.F.Actor.Object.OutputHandler).SetupGet(x => x.Perceiver).Returns(f.F.Actor.Object);
		Mock.Get(f.F.Actor.Object.Location).SetupGet(x => x.RouteDefinition).Returns(() => null!);
		Mock.Get(f.F.Actor.Object.Location).SetupGet(x => x.Characters).Returns(new[] { f.F.Actor.Object });
		Mock.Get(f.F.Actor.Object.Location).Setup(x => x.LayerCharacters(It.IsAny<RoomLayer>())).Returns(new[] { f.F.Actor.Object });
		Mock.Get(f.F.Actor.Object.OutputHandler).Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<IOutput, bool, bool>((output, _, _) => { if (output is IEmoteOutput emote && emote.DefaultEmote.RawText.Contains("target echo")) fury.ReduceSourceGrade(1); });
		var result = f.Cast(3); Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreEqual(6, fury.State.SourceGrade, result.Message); Assert.AreSame(head, f.Parents.Single()); Assert.AreEqual(0, f.SaveCalls);
	}

	[TestMethod]
	public void SaveCallback_ChangesProfileBinding_PreservesRetainedEffectsAndQuarantines()
	{
		using var f = new Fixture(EmotionalSpellKind.Calm);
		f.SaveCallback = () => f.F.Traits.RemoveAll(x => x.Id == 11);
		var result = f.Cast(3); Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.IsFalse(f.Parents.Any()); Assert.AreEqual(1, f.SaveCalls);
	}

	[TestMethod]
	[DataRow(EmotionalSpellKind.Fury)]
	[DataRow(EmotionalSpellKind.Calm)]
	public void NativeSchedulerExpiry_RemovesOwnedParentAndChild(EmotionalSpellKind kind)
	{
		using var f = new Fixture(kind); Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(1).Status);
		var head = f.Parents.Single(); f.Scheduler.Unschedule(head, fireExpireAction: true);
		Assert.IsFalse(f.Parents.Any()); Assert.IsFalse(f.Handler.Effects.OfType<SpellEmotionalEffect>().Any()); Assert.IsFalse(f.Scheduler.IsScheduled(head));
	}

	[TestMethod]
	public void PreInsertionCallback_SaveCannotSerializeAnUnappliedChild()
	{
		using var f = new Fixture(EmotionalSpellKind.Calm); XElement? saved = null;
		f.BeforeChild = () =>
		{
			saved = f.Parents.Single().SaveToXml(new Dictionary<IEffect, TimeSpan>());
			throw new InvalidOperationException("attachment refused before owner insertion");
		};
		var result = f.Cast(3); Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.IsNotNull(saved); Assert.IsFalse(saved.Element("Effect")!.Element("Children")!.Elements().Any());
		Assert.IsFalse(f.Parents.Any()); Assert.IsFalse(f.Handler.Effects.OfType<SpellEmotionalEffect>().Any());
		MagicSpellParent.InitialiseEffectType(); var loaded = (MagicSpellParent)Effect.LoadEffect(saved, f.F.Actor.Object);
		Assert.IsFalse(loaded.SpellEffects.Any()); Assert.IsFalse(f.Handler.Effects.OfType<SpellEmotionalEffect>().Any());
	}

	[TestMethod]
	public void ParentAttachmentCallback_ChangedBody_LeavesNoUnappliedChildToReload()
	{
		using var f = new Fixture(EmotionalSpellKind.Fury);
		f.ParentCallback = () => f.F.Actor.SetupGet(x => x.Body).Returns(Mock.Of<MudSharp.Body.IBody>());
		var result = f.Cast(3); Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.IsFalse(f.Parents.Any()); Assert.IsFalse(f.Handler.Effects.OfType<SpellEmotionalEffect>().Any());
		Assert.IsFalse(f.Handler.Effects.Any(x => x.SavingEffect && x is MagicSpellParent), "No serialized owner may replay an unattached child.");
	}

	[TestMethod]
	public void OversizedCap_AdmitsActualSchedulableGradeOneDuration()
	{
		using var f = new Fixture(EmotionalSpellKind.Fury);
		var profile = new EmotionalStockProfile(EmotionalSpellKind.Fury, "test.largecap", int.MaxValue, 120, 20, 5, 10, 1, new(1, 1, 0), [], [], false);
		((List<IMagicSpellEffectTemplate>)f.Spell.SpellEffects)[0] = new SourceFuryEffect(new XElement("Effect", profile.SaveToXml()), f.Spell);
		var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(TimeSpan.FromSeconds(int.MaxValue), f.Scheduler.RemainingDuration(f.Parents.Single()));
	}

	[TestMethod]
	public void UnrepresentableNativeDeadline_RefusesBeforePayment()
	{
		using var f = new Fixture(EmotionalSpellKind.Fury);
		var profile = new EmotionalStockProfile(EmotionalSpellKind.Fury, "test.overflow", int.MaxValue, 120, 20, 5, 10, 1,
			new(120, 1, 0), [], [], false);
		((List<IMagicSpellEffectTemplate>)f.Spell.SpellEffects)[0] = new SourceFuryEffect(new XElement("Effect", profile.SaveToXml()), f.Spell);
		var writes = f.F.Store.Writes; var result = f.Cast(7);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(writes, f.F.Store.Writes); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(0, f.F.Rolls);
	}

	[TestMethod]
	public void PreparedPayload_RefusesWithoutEffectsOrChecks()
	{
		using var f = new Fixture(EmotionalSpellKind.Fury);
		Assert.ThrowsExactly<InvalidOperationException>(() => f.Spell.ResolveTriggeredSpell(f.F.Actor.Object, f.F.Actor.Object, SpellPower.Standard));
		Assert.IsFalse(f.Parents.Any()); Assert.AreEqual(0, f.F.Rolls);
	}

	private sealed class Clock(DateTime now) : TimeProvider
	{
		internal DateTime Now = now;
		public override DateTimeOffset GetUtcNow() => new(Now);
		internal void Advance(int seconds) => Now = Now.AddSeconds(seconds);
	}

	private sealed class Fixture : IDisposable
	{
		internal readonly MagicCastingFixture F = new();
		internal readonly Clock Clock;
		internal readonly EffectScheduler Scheduler;
		internal readonly EffectHandler Handler;
		internal readonly MagicSpell Spell;
		internal readonly ITraitDefinition Attribute;
		private readonly IDisposable _clockScope;
		internal SourceEmotionalEffect Template => (SourceEmotionalEffect)Spell.SpellEffects.Single();
		internal IEnumerable<MagicSpellParent> Parents => Handler.Effects.OfType<MagicSpellParent>();
		internal int SaveCalls;
		internal Difficulty SaveDifficulty;
		internal Outcome Resistance = Outcome.Fail;
		internal Action? SaveCallback;
		internal int EmotionalRemovals;
		internal Action? ParentCallback;
		internal Action? BeforeChild;
		internal Action? FrenzyRemovalCallback;
		private IRace _frenzyRace = null!;
		private IDrug _suppressionDrug = null!;

		internal Fixture(EmotionalSpellKind kind)
		{
			Clock = new(F.Now); _clockScope = RuntimeClock.Push(Clock);
			Scheduler = new(F.World.Object, Clock); F.World.SetupGet(x => x.EffectScheduler).Returns(Scheduler);
			Handler = new(F.Actor.Object); F.Actor.SetupGet(x => x.Effects).Returns(() => Handler.Effects);
			F.Body.SetupGet(x => x.Actor).Returns(F.Actor.Object);
			F.Actor.SetupGet(x => x.Combat).Returns((ICombat)null!);
			F.Actor.SetupGet(x => x.CombatTarget).Returns((IPerceiver)null!);
			F.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect =>
			{ if (effect is SpellEmotionalEffect) BeforeChild?.Invoke(); Handler.AddEffect(effect); });
			F.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, duration) =>
			{ Handler.AddEffect(effect, duration); if (effect is MagicSpellParent) ParentCallback?.Invoke(); });
			F.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) =>
			{ if (effect is SpellEmotionalEffect) EmotionalRemovals++; Handler.RemoveEffect(effect, fire); if (effect is CombatFrenzyEffect) FrenzyRemovalCallback?.Invoke(); });
			F.Actor.Setup(x => x.CombinedEffectsOfType<IAdmittedHostileAttackEffect>()).Returns(() => Handler.Effects.OfType<IAdmittedHostileAttackEffect>());
			F.Actor.Setup(x => x.EffectsOfType<IAdmittedHostileAttackEffect>(It.IsAny<Predicate<IAdmittedHostileAttackEffect>>()))
				.Returns<Predicate<IAdmittedHostileAttackEffect>>(predicate => Handler.Effects.OfType<IAdmittedHostileAttackEffect>().Where(x => predicate is null || predicate(x)));
			Attribute = Mock.Of<ITraitDefinition>(x => x.Id == 10 && x.TraitType == TraitType.Attribute);
			F.Traits.Add(Attribute); F.Traits.Add(Mock.Of<ITraitDefinition>(x => x.Id == 11 && x.TraitType == TraitType.Skill));
			var terrains = Enumerable.Range(101, 7).Select(id => Mock.Of<ITerrain>(x => x.Id == id)).ToArray();
			F.World.SetupGet(x => x.Terrains).Returns(MagicCastingFixture.Collection(() => terrains));
			Mock.Get(F.Actor.Object.Location).Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(terrains.Last());
			var known = F.World.Object.FutureProgs.Get(1)!; var allowed = new Mock<IFutureProg>();
			allowed.SetupGet(x => x.Id).Returns(20); allowed.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
			allowed.SetupGet(x => x.FunctionText).Returns("return true"); allowed.Setup(x => x.Compile()).Returns(true);
			allowed.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
			object permitted = true; allowed.Setup(x => x.ExecuteWithStatus(out permitted, It.IsAny<object[]>())).Returns(true);
			F.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => new[] { known, allowed.Object }));
			var model = (MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(F.Spell, [])!;
			model.Definition = (kind == EmotionalSpellKind.Fury ? ArmageddonRousedFuryStock.Definition(10, 1, FuryCalmStockProfileTests.Fury())
				: ArmageddonStillAngerStock.Definition(10, 1, FuryCalmStockProfileTests.Calm())).ToString();
			model.AppliedEffectsAreExclusive = true; model.TargetNullEmote = "No suitable target.";
			Spell = new(model, F.World.Object) { EffectDurationExpression = new TraitExpression("0", F.World.Object) };
			F.Spells.Clear(); F.Spells.Add(Spell);
			F.Skills[1] = 100; F.Acquire(7); F.Actor.Setup(x => x.TargetActorOrCorpse("self", It.IsAny<PerceiveIgnoreFlags>())).Returns(F.Actor.Object);
			var save = new Mock<ICheck>();
			save.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
				It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns<IPerceivableHaveTraits, Difficulty, ITraitDefinition, IPerceivable, double, TraitUseType, (string, object)[]>((_, difficulty, _, _, _, _, _) =>
				{ SaveCalls++; SaveDifficulty = difficulty; SaveCallback?.Invoke(); return Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck, Resistance)); });
			F.World.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Returns(save.Object);
			F.Actor.SetupGet(x => x.IsEmbodied).Returns(true);
			F.Body.SetupGet(x => x.CurrentStamina).Returns(100.0);
			F.Body.SetupGet(x => x.ActiveDrugDosages).Returns(Array.Empty<DrugDosage>());
			F.Body.SetupGet(x => x.LatentDrugDosages).Returns(Array.Empty<DrugDosage>());
		}

		internal void ConfigureFrenzy()
		{
			_frenzyRace = Mock.Of<IRace>(x => x.Id == 201);
			_suppressionDrug = Mock.Of<IDrug>(x => x.Id == 301);
			F.World.SetupGet(x => x.Races).Returns(MagicCastingFixture.Collection(() => new[] { _frenzyRace }));
			F.World.SetupGet(x => x.Drugs).Returns(MagicCastingFixture.Collection(() => new[] { _suppressionDrug }));
			F.Body.SetupGet(x => x.Race).Returns(_frenzyRace);
			var baseProfile = Spell.SpellEffects.Single() is SourceFuryEffect
				? FuryCalmStockProfileTests.Fury() : FuryCalmStockProfileTests.Calm();
			var profile = new EmotionalStockProfile(baseProfile.Kind, baseProfile.Group, baseProfile.UnitSeconds, baseProfile.CapUnits,
				baseProfile.EligibilityProgId, baseProfile.Intensity, baseProfile.TraitId, baseProfile.UnitsPerSourcePoint,
				baseProfile.Fallback, baseProfile.Terrains, baseProfile.Saves, baseProfile.BreakOnAdmittedAttack,
				baseProfile.CanonicalGradePolicy,
				baseProfile.Kind == EmotionalSpellKind.Fury ? new EmotionalFrenzyProfile(_frenzyRace.Id, _suppressionDrug.Id, 15, 15, 5, 4) : null,
				baseProfile.Kind == EmotionalSpellKind.Calm);
			((List<IMagicSpellEffectTemplate>)Spell.SpellEffects)[0] = baseProfile.Kind == EmotionalSpellKind.Fury
				? new SourceFuryEffect(new XElement("Effect", profile.SaveToXml()), Spell)
				: new SourceCalmEffect(new XElement("Effect", profile.SaveToXml()), Spell);
		}

		internal void AddSuppressionDose(bool active)
		{
			var dose = new DrugDosage { Drug = _suppressionDrug, Grams = 1.0, OriginalVector = DrugVector.Ingested,
				Originator = F.Actor.Object, SpellOriginFamily = string.Empty, SpellOriginFormula = string.Empty };
			F.Body.SetupGet(x => x.ActiveDrugDosages).Returns(active ? new[] { dose } : Array.Empty<DrugDosage>());
			F.Body.SetupGet(x => x.LatentDrugDosages).Returns(active ? Array.Empty<DrugDosage>() : new[] { dose });
		}

		internal MagicSpellParent Existing(EmotionalSpellKind kind, int grade, int seconds, SpellPower power = SpellPower.Standard, double endurance = 5)
		{
			var profile = kind == EmotionalSpellKind.Fury ? FuryCalmStockProfileTests.Fury() : FuryCalmStockProfileTests.Calm();
			var head = new MagicSpellParent(F.Actor.Object, Spell, F.Actor.Object, power);
			var state = new EmotionalRetainedState(grade, power, profile.Intensity, kind == EmotionalSpellKind.Fury ? endurance : 0);
			SpellEmotionalEffect child = kind == EmotionalSpellKind.Fury
				? new SpellSourceFuryEffect(F.Actor.Object, head, profile.Group, profile.UnitSeconds, profile.CapUnits, state, Attribute, profile.UnitsPerSourcePoint)
				: new SpellSourceCalmEffect(F.Actor.Object, head, profile.Group, profile.UnitSeconds, profile.CapUnits, state);
			head.AddSpellEffect(child); Handler.AddEffect(child); Handler.AddEffect(head, TimeSpan.FromSeconds(seconds)); return head;
		}
		internal MagicCastingResult Cast(int grade) { F.Now = Clock.Now; F.Balances[F.Resources[1]] = 100; return F.Service.Cast(F.Intent(grade, false)); }
		internal bool Applied(MagicCastingResult result) => (bool?)XElement.Parse(F.Store.Operations[result.OperationId!.Value].Definition).Attribute("applied") == true;
		public void Dispose() => _clockScope.Dispose();
	}
}
