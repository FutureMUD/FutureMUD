#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DetectInvisibleLifetimeTests
{
	private const string Group = "armageddon.detect_invisibility";
	private const string Definition = "<Effect type='detectinvisible'><LifetimePolicy version='1' mode='accumulate' group='armageddon.detect_invisibility' unitSeconds='600' maximumUnits='48' retainStrongestGrade='true'/></Effect>";

	[DataTestMethod]
	[DataRow(1, 3600)]
	[DataRow(599, 3600)]
	[DataRow(600, 3600)]
	[DataRow(601, 4200)]
	[DataRow(1199, 4200)]
	[DataRow(1200, 4200)]
	[DataRow(1201, 4800)]
	public void PaidRecast_UsesNormalisedSourceWholeUnitBoundaries(int remaining, int expected)
	{
		using var f = new Fixture(); var old = f.AddExisting(remaining, 4, SpellPower.Weak);
		var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var current = f.Parents.Single();
		Assert.AreEqual(f.Clock.Now.AddSeconds(expected), f.Scheduler.ScheduledExpiry(current));
		Assert.AreEqual(4, current.LifetimeState!.Grade); Assert.AreEqual(SpellPower.Weak, current.Power);
		Assert.AreNotEqual(old.Identity, current.Identity); Assert.AreSame(f.F.Actor.Object, current.Caster);
		Assert.IsFalse(f.Handler.Effects.Contains(old)); Assert.IsFalse(f.Scheduler.IsScheduled(old)); Assert.IsFalse(old.SpellEffects.Any());
		Assert.AreEqual(95.0, f.F.Balances[f.F.Resources[1]]); Assert.IsTrue(f.Applied(result));
	}

	[TestMethod]
	public void SourceBoundary_ExpiryMinusOneAndIntegerUpdateAgreeWithNormalisation()
	{
		// codedump.c:49438 expiry = initiation + 600*duration - 1;
		// :172611 delta>0 => remaining duration = integer(delta/600)+1.
		foreach (var remaining in new[] { 2, 599, 600, 601, 1199, 1200, 1201 })
			Assert.AreEqual((remaining - 1) / 600 + 1, (int)Math.Ceiling(remaining / 600.0));
		// Native deadlines deliberately normalise the inclusive endpoint's final second.
		Assert.AreEqual(1.0, Math.Ceiling(1.0 / 600));
	}

	[DataTestMethod]
	[DataRow(7, SpellPower.ExtremelyWeak, 1, SpellPower.ExtremelyStrong, 7, SpellPower.ExtremelyWeak)]
	[DataRow(1, SpellPower.ExtremelyStrong, 7, SpellPower.ExtremelyWeak, 7, SpellPower.ExtremelyWeak)]
	public void Recast_RetainsStrongestSourceGradeIndependentlyOfEditablePower(int oldGrade, SpellPower oldPower,
		int newGrade, SpellPower newPower, int retainedGrade, SpellPower retainedPower)
	{
		using var f = new Fixture();
		var copy = f.Copy(newGrade, newPower); f.AddExisting(3000, oldGrade, oldPower);
		copy.ResolveTriggeredSpell(f.F.Actor.Object, f.F.Actor.Object, newPower);
		Assert.AreEqual(retainedGrade, f.Parents.Single().LifetimeState!.Grade);
		Assert.AreEqual(retainedPower, f.Parents.Single().Power);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]], "Prepared delivery must not charge again.");
	}

	[TestMethod]
	public void PreparedRoute_AccumulatesClampsAndRemovesPreviousChildren()
	{
		using var f = new Fixture(); var old = f.AddExisting(21000, 7, SpellPower.Strong);
		f.Copy(7, SpellPower.VeryStrong).ResolveTriggeredSpell(f.F.Actor.Object, f.F.Actor.Object, SpellPower.VeryStrong);
		var current = f.Parents.Single();
		Assert.AreEqual(f.Clock.Now.AddSeconds(28800), f.Scheduler.ScheduledExpiry(current));
		Assert.IsFalse(old.SpellEffects.Any()); Assert.IsFalse(f.Scheduler.IsScheduled(old));
		Assert.AreEqual(1, f.Handler.Effects.OfType<SpellDetectInvisibleEffect>().Count());
		f.Clock.Advance(TimeSpan.FromSeconds(28801)); f.Scheduler.CheckSchedules();
		Assert.IsFalse(f.Parents.Any()); Assert.IsFalse(f.Handler.Effects.OfType<SpellDetectInvisibleEffect>().Any());
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void CasterOnlyPolicy_EmptyPrimaryPhasePreservesAdmittedCohort(bool prepared)
	{
		using var f = new Fixture(); var template = f.Spell.SpellEffects.Single();
		((List<IMagicSpellEffectTemplate>)f.Spell.SpellEffects).Clear();
		((List<IMagicSpellEffectTemplate>)f.Spell.CasterSpellEffects).Add(template);
		var old = f.AddExisting(6000, 7, SpellPower.Strong);
		if (prepared) f.Copy(1, SpellPower.Weak).ResolveTriggeredSpell(f.F.Actor.Object, f.F.Actor.Object, SpellPower.Weak);
		else
		{
			var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		}
		var current = f.Parents.Single();
		Assert.AreEqual(7, current.LifetimeState!.Grade);
		Assert.AreEqual(SpellPower.Strong, current.Power);
		Assert.AreEqual(f.Clock.Now.AddSeconds(9000), f.Scheduler.ScheduledExpiry(current));
		Assert.IsFalse(f.Handler.Effects.Contains(old)); Assert.IsFalse(f.Scheduler.IsScheduled(old));
		Assert.AreEqual(1, current.SpellEffects.Count());
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void PreparedCallback_AfterAdmissionCannotChangeTargetOrCasterCohort(bool casterOnly)
	{
		using var f = new Fixture();
		if (casterOnly)
		{
			var template = f.Spell.SpellEffects.Single(); ((List<IMagicSpellEffectTemplate>)f.Spell.SpellEffects).Clear();
			((List<IMagicSpellEffectTemplate>)f.Spell.CasterSpellEffects).Add(template);
		}
		var old = f.AddExisting(6000, 7, SpellPower.Strong); var copy = f.Copy(1, SpellPower.Weak);
		copy.EffectDurationExpression = new CallbackExpression(f.F.World.Object,
			() => f.Scheduler.Reschedule(old, TimeSpan.FromSeconds(6600)));
		Assert.ThrowsException<InvalidOperationException>(() => copy.ResolveTriggeredSpell(f.F.Actor.Object, f.F.Actor.Object, SpellPower.Weak));
		Assert.AreSame(old, f.Parents.Single()); Assert.AreEqual(1, old.SpellEffects.Count());
		Assert.AreEqual(f.Clock.Now.AddSeconds(6600), f.Scheduler.ScheduledExpiry(old));
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
	}

	[TestMethod]
	public void Cohort_SumsSameSourceAcrossCasterAndSpellIdsAndPreservesIndependentGroups()
	{
		using var f = new Fixture();
		var other = f.F.NewSpell(2, "Other historical delivery", Definition);
		var secondCaster = Mock.Of<ICharacter>(x => x.Id == 555 && x.Gameworld == f.F.World.Object);
		var a = f.AddExisting(3000, 1, SpellPower.Weak);
		var b = f.AddExisting(6000, 6, SpellPower.VeryWeak, other, secondCaster);
		var custom = f.AddExisting(7000, 7, SpellPower.Strong, policy: null, untagged: true);
		var different = f.AddExisting(8000, 7, SpellPower.Strong, policy: new("other.detection", 600, 48));
		var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var current = f.Parents.Single(x => x.LifetimeGroup == Group);
		Assert.AreEqual(f.Clock.Now.AddSeconds(12000), f.Scheduler.ScheduledExpiry(current));
		Assert.AreEqual(6, current.LifetimeState!.Grade); Assert.AreEqual(SpellPower.VeryWeak, current.Power);
		Assert.IsFalse(f.Handler.Effects.Contains(a)); Assert.IsFalse(f.Handler.Effects.Contains(b));
		Assert.IsTrue(f.Handler.Effects.Contains(custom)); Assert.IsTrue(f.Handler.Effects.Contains(different));
		Assert.AreEqual(f.Clock.Now.AddSeconds(7000), f.Scheduler.ScheduledExpiry(custom));
		Assert.AreEqual(f.Clock.Now.AddSeconds(8000), f.Scheduler.ScheduledExpiry(different));
	}

	[DataTestMethod]
	[DataRow(0, false)]
	[DataRow(1, true)]
	public void AtCap_ReportsActualDeadlineAndStrengthInsteadOfOriginalDuration(int elapsed, bool changed)
	{
		using var f = new Fixture(); var old = f.AddExisting(28800, 7, SpellPower.ExtremelyStrong);
		var oldDeadline = f.Scheduler.ScheduledExpiry(old); f.Advance(elapsed);
		var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(changed, f.Applied(result)); Assert.AreEqual(95.0, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(oldDeadline!.Value.AddSeconds(elapsed), f.Scheduler.ScheduledExpiry(f.Parents.Single()));
		Assert.AreEqual(7, f.Parents.Single().LifetimeState!.Grade);
		Assert.AreEqual(0, f.F.Samples);
	}

	[TestMethod]
	public void NoChangeAtCap_OrdinaryOverreachPaysWithoutMasterySample()
	{
		using var f = new Fixture(); f.F.Acquire(1); f.AddExisting(28800, 7, SpellPower.ExtremelyStrong);
		var result = f.Cast(2, true);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message); Assert.IsFalse(f.Applied(result));
		Assert.AreEqual(1, f.F.Service.Acquisition(f.F.Actor.Object, 1)!.ControlledGrade); Assert.AreEqual(0, f.F.Samples);
		Assert.IsTrue(f.F.Balances[f.F.Resources[1]] < 100);
	}

	[TestMethod]
	public void AtCap_StrongerGradeReportsAppliedEvenWhenDeadlineDoesNotChange()
	{
		using var f = new Fixture(); var old = f.AddExisting(28800, 1, SpellPower.ExtremelyStrong);
		var deadline = f.Scheduler.ScheduledExpiry(old); var result = f.Cast(7);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message); Assert.IsTrue(f.Applied(result));
		Assert.AreEqual(deadline, f.Scheduler.ScheduledExpiry(f.Parents.Single()));
		Assert.AreEqual(7, f.Parents.Single().LifetimeState!.Grade);
	}

	[DataTestMethod]
	[DataRow(7, false)]
	[DataRow(0, true)]
	public void ParentMetadata_ReloadsStrengthOrPreservesInvalidBytesWithoutSilentDeletion(int grade, bool invalid)
	{
		using var f = new Fixture(); SpellDetectInvisibleEffect.InitialiseEffectType();
		var original = f.AddExisting(3000, 7, SpellPower.Weak);
		var xml = original.SaveToXml(new Dictionary<IEffect, TimeSpan> { [original] = TimeSpan.FromSeconds(3000) });
		xml.Element("Effect")!.Element("LifetimePolicy")!.SetAttributeValue("grade", grade);
		f.Handler.RemoveEffect(original, true);
		var restored = (MagicSpellParent)typeof(MagicSpellParent)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(XElement), typeof(IPerceivable)], null)!
			.Invoke([xml, f.F.Actor.Object]);
		f.Handler.AddEffect(restored, TimeSpan.FromSeconds(3000));
		Assert.AreEqual(original.Identity, restored.Identity); Assert.AreEqual(original.Power, restored.Power);
		Assert.AreEqual(Group, restored.LifetimeGroup); Assert.AreEqual(invalid, restored.LifetimePolicyError is not null);
		var saved = restored.SaveToXml(new Dictionary<IEffect, TimeSpan> { [restored] = TimeSpan.FromSeconds(3000) });
		Assert.IsTrue(XNode.DeepEquals(xml.Element("Effect")!.Element("LifetimePolicy"), saved.Element("Effect")!.Element("LifetimePolicy")));
		var result = f.Cast(1);
		Assert.AreEqual(invalid ? MagicCastingStatus.Refused : MagicCastingStatus.Succeeded, result.Status, result.Message);
		if (invalid) Assert.IsTrue(f.Handler.Effects.Contains(restored));
		else Assert.AreEqual(7, f.Parents.Single().LifetimeState!.Grade);
	}

	[DataTestMethod]
	[DataRow("0")]
	[DataRow("-600")]
	[DataRow("1/0")]
	[DataRow("3000*grade+1")]
	[DataRow("3000*grade+degrees")]
	public void InvalidIncrement_RefusesBeforePayment(string formula)
	{
		using var f = new Fixture(); f.Spell.EffectDurationExpression = new TraitExpression(formula, f.F.World.Object);
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message); Assert.IsNull(result.OperationId);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.IsFalse(f.F.Store.Operations.Any());
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void PermanentOrConflictingMetadata_RefusesWithoutDeletingParent(bool permanent)
	{
		using var f = new Fixture(); var old = f.AddExisting(3000, 4, SpellPower.Weak,
			policy: permanent ? null : new(Group, 601, 48));
		if (permanent) f.Scheduler.Unschedule(old);
		var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
		Assert.IsTrue(f.Handler.Effects.Contains(old)); Assert.AreEqual(1, old.SpellEffects.Count());
	}

	[TestMethod]
	public void BeforePaymentCallback_ChangedScheduleIsRefusedBeforeDebit()
	{
		using var f = new Fixture(); var old = f.AddExisting(3000, 1, SpellPower.Weak);
		f.F.Checkpoint = stage => { if (stage == "BeforePayment") f.Scheduler.Reschedule(old, TimeSpan.FromSeconds(3600)); };
		var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
		Assert.AreSame(old, f.Parents.Single());
	}

	[TestMethod]
	public void AfterPaymentCallback_ChangedCohortIsQuarantinedWithoutReplacementOrReplay()
	{
		using var f = new Fixture(); var old = f.AddExisting(3000, 1, SpellPower.Weak);
		f.F.Checkpoint = stage => { if (stage == "Committed") f.Scheduler.Reschedule(old, TimeSpan.FromSeconds(3600)); };
		var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreEqual(95.0, f.F.Balances[f.F.Resources[1]]); Assert.AreSame(old, f.Parents.Single());
		Assert.AreEqual(0, f.F.Samples); f.F.Checkpoint = null; f.F.Restart();
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.Cast(f.F.Intent(1, false)).Status);
		Assert.AreEqual(95.0, f.F.Balances[f.F.Resources[1]]);
	}

	[TestMethod]
	public void ChildAttachmentCallback_NewGroupedParentIsNeverDeleted()
	{
		using var f = new Fixture(); var old = f.AddExisting(3000, 1, SpellPower.Weak); MagicSpellParent? unrelated = null;
		f.OnChildAdded = _ => { f.OnChildAdded = null; unrelated = f.AddExisting(4200, 5, SpellPower.Strong); };
		var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.IsTrue(f.Handler.Effects.Contains(old)); Assert.IsTrue(f.Handler.Effects.Contains(unrelated!));
		Assert.AreEqual(f.Clock.Now.AddSeconds(4200), f.Scheduler.ScheduledExpiry(unrelated!));
		Assert.AreEqual(0, f.F.Samples);
	}

	[TestMethod]
	public void ParentAttachmentNotRetained_NeverReportsAppliedOrRemovesCohort()
	{
		using var f = new Fixture(); var old = f.AddExisting(3000, 1, SpellPower.Weak); f.RetainNewParent = false;
		var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreSame(old, f.Parents.Single()); Assert.IsFalse(f.Applied(result)); Assert.AreEqual(0, f.F.Samples);
	}

	[TestMethod]
	public void CleanupCallback_SeesAttachedReplacementAndCannotDeleteANewCohortMember()
	{
		using var f = new Fixture(); MagicSpellParent? inserted = null;
		var a = f.AddExisting(3000, 1, SpellPower.Weak, onRemoval: () =>
		{
			Assert.IsTrue(f.Parents.Any(x => x.LifetimeState!.Grade == 7 && f.Scheduler.IsScheduled(x)));
			inserted = f.AddExisting(6000, 5, SpellPower.Strong);
		});
		var b = f.AddExisting(3000, 2, SpellPower.Standard);
		var result = f.Cast(7); Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.IsFalse(f.Handler.Effects.Contains(a)); Assert.IsTrue(f.Handler.Effects.Contains(b));
		Assert.IsTrue(f.Handler.Effects.Contains(inserted!)); Assert.AreEqual(1, inserted!.SpellEffects.Count());
		Assert.IsTrue(f.Scheduler.IsScheduled(inserted)); Assert.IsFalse(f.Applied(result));
	}

	[TestMethod]
	public void PreparedChildAttachmentFailure_BoundsRetainedChildWithoutDeletingOldParent()
	{
		using var f = new Fixture(); var old = f.AddExisting(3000, 1, SpellPower.Weak);
		f.OnChildAdded = _ => throw new InvalidOperationException("after retention");
		Assert.ThrowsException<InvalidOperationException>(() => f.Copy(7, SpellPower.Strong)
			.ResolveTriggeredSpell(f.F.Actor.Object, f.F.Actor.Object, SpellPower.Strong));
		Assert.AreEqual(2, f.Parents.Count()); Assert.IsTrue(f.Handler.Effects.Contains(old));
		Assert.IsTrue(f.Parents.All(x => f.Scheduler.IsScheduled(x) && x.SpellEffects.Count() == 1));
	}

	[DataTestMethod]
	[DataRow("version='2' mode='accumulate' group='x' unitSeconds='600' maximumUnits='48' retainStrongestGrade='true'")]
	[DataRow("version='1' mode='accumulate' group='x' unitSeconds='0' maximumUnits='48' retainStrongestGrade='true'")]
	[DataRow("version='1' mode='accumulate' group='' unitSeconds='600' maximumUnits='48' retainStrongestGrade='true'")]
	[DataRow("version='1' mode='accumulate' group='x' unitSeconds='600' maximumUnits='48' retainStrongestGrade='false'")]
	public void InvalidPolicyXml_IsPreservedAndRefusedBeforePayment(string attributes)
	{
		using var f = new Fixture();
		var original = XElement.Parse($"<Effect type='detectinvisible'><LifetimePolicy {attributes}/></Effect>");
		var template = SpellEffectFactory.LoadEffect(original, f.Spell);
		((List<IMagicSpellEffectTemplate>)f.Spell.SpellEffects)[0] = template;
		Assert.IsNotNull(((IMagicSpellEffectLifetimePolicy)template).LifetimePolicyError);
		Assert.IsTrue(XNode.DeepEquals(original, template.SaveToXml()));
		Assert.AreEqual(MagicCastingStatus.Refused, f.Cast(1).Status);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
	}

	[TestMethod]
	public void TemplateBuilder_CloneAndReloadPreserveEditablePolicyAndOffCompatibility()
	{
		using var f = new Fixture();
		Assert.IsTrue(f.Spell.BuildingCommand(f.F.Actor.Object, new StringStack("effect 1 lifetime accumulate custom.group 300 96")));
		var template = f.Spell.SpellEffects.Single();
		Assert.AreEqual(new MagicSpellLifetimePolicy("custom.group", 300, 96), ((IMagicSpellEffectLifetimePolicy)template.Clone()).LifetimePolicy);
		Assert.IsTrue(template.Show(f.F.Actor.Object).Contains("custom.group"));
		Assert.IsTrue(f.Spell.BuildingCommand(f.F.Actor.Object, new StringStack("effect 1 lifetime off")));
		Assert.IsNull(((IMagicSpellEffectLifetimePolicy)template).LifetimePolicy);
		Assert.IsNull(template.SaveToXml().Element("LifetimePolicy"));
	}

	private sealed class Clock(DateTime now) : TimeProvider
	{
		public DateTime Now { get; private set; } = now;
		public override DateTimeOffset GetUtcNow() => new(Now);
		public void Advance(TimeSpan duration) => Now += duration;
	}

	private sealed class RemovalProbe(ICharacter owner, MagicSpellParent parent, Action callback)
		: SpellDetectInvisibleEffect(owner, parent)
	{
		public override void RemovalEffect() { base.RemovalEffect(); callback(); }
	}

	private sealed class CallbackExpression(IFuturemud world, Action callback) : TraitExpression("3000", world)
	{
		private int _evaluations;
		public override double EvaluateWith(IHaveTraits owner, ITraitDefinition variable = null!, TraitBonusContext context = TraitBonusContext.None,
			params (string Name, object Value)[] values)
		{
			if (++_evaluations == 2) callback();
			return 3000;
		}
	}

	private sealed class Fixture : IDisposable
	{
		public MagicCastingFixture F { get; } = new();
		public MagicSpell Spell { get; }
		public Clock Clock { get; }
		public EffectScheduler Scheduler { get; }
		public EffectHandler Handler { get; }
		private readonly IDisposable _time;
		public bool RetainNewParent { get; set; } = true;
		public Action<IEffect>? OnChildAdded { get; set; }
		public IEnumerable<MagicSpellParent> Parents => Handler.Effects.OfType<MagicSpellParent>();

		public Fixture()
		{
			Clock = new(F.Now); _time = RuntimeClock.Push(Clock);
			Scheduler = new(F.World.Object, Clock); F.World.SetupGet(x => x.EffectScheduler).Returns(Scheduler);
			Handler = new(F.Actor.Object); F.Actor.SetupGet(x => x.Effects).Returns(() => Handler.Effects);
			F.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect =>
			{ Handler.AddEffect(effect); if (effect is SpellDetectInvisibleEffect) OnChildAdded?.Invoke(effect); });
			F.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, duration) =>
			{ if (RetainNewParent) Handler.AddEffect(effect, duration); });
			F.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>(Handler.RemoveEffect);
			F.Actor.Setup(x => x.RemoveAllEffects<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>(), It.IsAny<bool>()))
				.Returns<Predicate<MagicSpellParent>, bool>((predicate, fire) => Handler.RemoveAllEffects(predicate, fire));
			F.Spells.Remove(F.Spell); Spell = F.NewSpell(1, "Lifetime detection", Definition);
			Assert.IsTrue(Spell.BuildingCommand(F.Actor.Object, new StringStack("grades fixture")));
			Assert.IsTrue(Spell.BuildingCommand(F.Actor.Object, new StringStack("exclusiveeffect")));
			Spell.EffectDurationExpression = new TraitExpression("3000*grade", F.World.Object); F.Skills[1] = 100; F.Acquire(7);
		}

		public MagicSpellParent AddExisting(double seconds, int grade, SpellPower power, IMagicSpell? spell = null,
			ICharacter? caster = null, MagicSpellLifetimePolicy? policy = null, bool untagged = false, Action? onRemoval = null)
		{
			var head = new MagicSpellParent(F.Actor.Object, spell ?? Spell, caster ?? F.Actor.Object, power)
			{ LifetimeState = untagged ? null : new(policy ?? new(Group, 600, 48), grade) };
			var child = onRemoval is null ? new SpellDetectInvisibleEffect(F.Actor.Object, head) : new RemovalProbe(F.Actor.Object, head, onRemoval);
			head.AddSpellEffect(child);
			Handler.AddEffect(child); Handler.AddEffect(head, TimeSpan.FromSeconds(seconds)); return head;
		}
		public void Advance(int seconds) { Clock.Advance(TimeSpan.FromSeconds(seconds)); F.Now = Clock.Now; }
		public MagicCastingResult Cast(int grade, bool overreach = false)
		{ F.Balances[F.Resources[1]] = 100; return F.Service.Cast(F.Intent(grade, overreach)); }
		public bool Applied(MagicCastingResult result) =>
			(bool?)XElement.Parse(F.Store.Operations[result.OperationId!.Value].Definition).Attribute("applied") == true;
		public MagicSpell Copy(int grade, SpellPower power) => (MagicSpell)typeof(MagicSpell)
			.GetMethod("CastingCopy", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(Spell, [F.Actor.Object, F.Traits[0], grade, power, Difficulty.Easy, 7])!;
		public void Dispose() => _time.Dispose();
	}
}
