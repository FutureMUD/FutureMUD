#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Form.Shape;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EmotionalCombatHooksTests
{
	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void RangedWrapper_AdmissionRefusalPreservesAimAndCostUnlessAnEarlierShotCommitted(bool earlierShotCommitted)
	{
		var f = new Fixture(); var aim = new Mock<IAimInformation>(); aim.SetupProperty(x => x.AimPercentage, 0.8);
		f.A.Aim = aim.Object;
		var weapon = new Mock<IRangedWeapon> { DefaultValue = DefaultValue.Mock };
		Mock.Get(weapon.Object.WeaponType).SetupGet(x => x.AimBonusLostPerShot).Returns(0.2);
		weapon.Setup(x => x.Fire(f.A, f.B, Outcome.Pass, Outcome.Pass, It.IsAny<OpposedOutcome>(), null!, null!, f.B))
			.Callback(() =>
			{
				if (earlierShotCommitted) HostileAttackAdmission.RecordShotCommitted();
				HostileAttackAdmission.RecordShotAdmissionRefused();
			});
		var move = new RangedWeaponAttackMove(f.A, f.B, weapon.Object);
		using var attempt = HostileAttackAdmission.BeginAttempt(f.A, f.B);
		var fire = typeof(RangedWeaponAttackBase).GetMethod("FireWeapon", BindingFlags.NonPublic | BindingFlags.Instance)!;
		var accepted = (bool)fire.Invoke(move, [f.A, f.B, Outcome.Pass, Outcome.Pass,
			new OpposedOutcome(Outcome.Fail, Outcome.Pass), null!, null!, f.B])!;
		Assert.AreEqual(earlierShotCommitted, accepted);
		Assert.AreSame(aim.Object, f.A.Aim);
		Assert.AreEqual(earlierShotCommitted ? 0.6 : 0.8, f.A.Aim.AimPercentage, 0.00001);
		Assert.AreEqual(earlierShotCommitted, move.UsesStaminaWithResult(accepted ? new CombatMoveResult() : CombatMoveResult.Irrelevant));
		weapon.Verify(x => x.Fire(f.A, f.B, Outcome.Pass, Outcome.Pass, It.IsAny<OpposedOutcome>(), null!, null!, f.B), Times.Once);
	}

	[TestMethod]
	public void AdmittedAttack_OneAttemptDeduplicatesRecipientsAndKeepsOtherEffects()
	{
		var f = new Fixture();
		var seen = new List<AdmittedHostileAttack>();
		var probe = new Probe(f.B, seen.Add);
		var sibling = new RemovalProbe(f.B, () => Assert.Fail("Unrelated effect was removed."));
		f.B.AddEffect(probe); f.B.AddEffect(sibling);
		using (HostileAttackAdmission.BeginAttempt(f.A, f.B))
		{
			Assert.IsTrue(HostileAttackAdmission.TryNotify(f.A, f.B));
			Assert.IsTrue(HostileAttackAdmission.TryNotify(f.A, f.B));
		}
		Assert.AreEqual(1, seen.Count);
		Assert.AreSame(f.A, seen[0].Attacker); Assert.AreSame(f.B, seen[0].Recipient);
		Assert.AreNotEqual(Guid.Empty, seen[0].OperationIdentity);
		Assert.IsTrue(f.B.Effects.Any(x => ReferenceEquals(x, sibling)));
	}

	[TestMethod]
	public void AdmittedAttack_NestedCountershotAndActualChildScopesHaveIndependentIdentities()
	{
		var f = new Fixture(); var seen = new List<AdmittedHostileAttack>();
		f.A.AddEffect(new Probe(f.A, seen.Add)); f.B.AddEffect(new Probe(f.B, seen.Add));
		using (HostileAttackAdmission.BeginAttempt(f.A, f.B))
		{
			Assert.IsTrue(HostileAttackAdmission.TryNotify(f.A, f.B));
			using (HostileAttackAdmission.BeginAttempt(f.B, f.A)) Assert.IsTrue(HostileAttackAdmission.TryNotify(f.B, f.A));
			Assert.IsTrue(HostileAttackAdmission.TryNotify(f.A, f.B));
		}
		using (HostileAttackAdmission.BeginAttempt(f.A, f.B)) Assert.IsTrue(HostileAttackAdmission.TryNotify(f.A, f.B));
		Assert.AreEqual(3, seen.Count); Assert.AreEqual(3, seen.Select(x => x.OperationIdentity).Distinct().Count());
	}

	[DataTestMethod]
	[DataRow("before-target-aba")]
	[DataRow("before-combat-aba")]
	[DataRow("during-target-aba")]
	[DataRow("during-focus-rebind")]
	public void AdmittedAttack_StaleOrCallbackReplacedPhysicalStateRefusesContinuation(string change)
	{
		var f = new Fixture(); var calls = 0;
		void Change()
		{
			if (change.Contains("combat")) { f.A.Combat = null!; f.A.Combat = f.Combat; }
			else if (change.Contains("focus")) Mock.Get(f.B.Body).SetupGet(x => x.Actor).Returns(f.A);
			else { f.A.CombatTarget = null!; f.A.CombatTarget = f.B; }
		}
		f.B.AddEffect(new Probe(f.B, _ => { ++calls; Change(); }));
		using var attempt = HostileAttackAdmission.BeginAttempt(f.A, f.B);
		if (change.StartsWith("before")) Change();
		Assert.IsFalse(HostileAttackAdmission.TryNotify(f.A, f.B));
		Assert.AreEqual(change.StartsWith("before") ? 0 : 1, calls);
	}

	[TestMethod]
	public void AdmittedAttack_ApplicabilityRemovalDoesNotNotifyRemovedSnapshotEntry()
	{
		var f = new Fixture(); var calls = 0;
		var probe = new Probe(f.B, _ => ++calls);
		probe.Applicable = () => { f.B.RemoveEffect(probe); return true; };
		f.B.AddEffect(probe);
		using var attempt = HostileAttackAdmission.BeginAttempt(f.A, f.B);
		Assert.IsTrue(HostileAttackAdmission.TryNotify(f.A, f.B)); Assert.AreEqual(0, calls);
	}

	[TestMethod]
	public void Cessation_ExactPairDetachesBothAndClearsOldWorkOnce()
	{
		var f = new Fixture(); var removal = 0;
		var old = new RemovalProbe(f.B, () => ++removal); f.B.AddEffect(old);
		var queued = SelectedCombatAction.GetEffectCharge(f.B, f.A); f.B.AddEffect(queued);
		var ticket = f.Combat.PrepareCessation(f.B, f.A, f.Combat)!;
		Assert.IsTrue(ticket.IsCurrent);
		Assert.AreEqual(CombatCessationChanges.SubjectRemoved | CombatCessationChanges.OpponentPairCleared, f.Combat.CeaseCombatFor(ticket));
		Assert.AreEqual(0, f.Combat.Combatants.Count());
		Assert.IsNull(f.A.Combat); Assert.IsNull(f.B.Combat); Assert.IsNull(f.A.CombatTarget); Assert.IsNull(f.B.CombatTarget);
		Assert.AreEqual(TimeSpan.MinValue, f.Scheduler.RemainingDuration(f.B, ScheduleType.Combat));
		Assert.AreEqual(1, removal); Assert.IsFalse(ticket.IsCurrent);
		Assert.IsFalse(f.B.Effects.Any(x => ReferenceEquals(x, queued)));
		Assert.AreEqual(CombatCessationChanges.None, f.Combat.CeaseCombatFor(ticket));
	}

	[TestMethod]
	public void AdmittedAttack_ApplicabilityRebindCannotNotifyAForeignOwnedSnapshot()
	{
		var f = new Fixture(); var calls = 0; var probe = new Probe(f.B, _ => ++calls);
		probe.Applicable = () => { f.B.RemoveEffect(probe); probe.RebindOwner(f.A); f.A.AddEffect(probe); return true; };
		f.B.AddEffect(probe); using var attempt = HostileAttackAdmission.BeginAttempt(f.A, f.B);
		Assert.IsTrue(HostileAttackAdmission.TryNotify(f.A, f.B)); Assert.AreEqual(0, calls);
		Assert.IsTrue(f.A.Effects.Any(x => ReferenceEquals(x, probe)));
	}

	[DataTestMethod]
	[DataRow("target")]
	[DataRow("combat")]
	[DataRow("new-incoming")]
	public void Cessation_PreparedTicketRejectsAbaAndChangedIncomingSetWithoutCallbacks(string change)
	{
		var f = new Fixture(); var ticket = f.Combat.PrepareCessation(f.B, f.A, f.Combat)!;
		if (change == "target") { f.B.CombatTarget = null!; f.B.CombatTarget = f.A; }
		if (change == "combat") { f.B.Combat = null!; f.B.Combat = f.Combat; }
		if (change == "new-incoming") { var c = Actor.Create(f.World.Object, 3); f.Add(c); c.CombatTarget = f.B; }
		var callbacks = 0; f.B.OnLeaveCombat += _ => ++callbacks;
		Assert.IsFalse(ticket.IsCurrent); Assert.AreEqual(CombatCessationChanges.None, f.Combat.CeaseCombatFor(ticket));
		Assert.AreSame(f.Combat, f.B.Combat); Assert.AreEqual(0, callbacks);
	}

	[DataTestMethod]
	[DataRow("leave-event")]
	[DataRow("aim-release")]
	[DataRow("effect-removal")]
	public void Cessation_CallbackReplacementKeepsNewCombatScheduleTargetMeleeAimAndQueue(string callback)
	{
		var f = new Fixture(); var next = new SimpleMeleeCombat(f.World.Object);
		var replacementAim = Mock.Of<IAimInformation>(); SelectedCombatAction? replacementEffect = null;
		var oldAim = new Mock<IAimInformation>(); f.B.Aim = oldAim.Object;
		void Replace()
		{
			next.JoinCombat(f.B); f.B.CombatTarget = f.A;
			f.B.MeleeRange = true; f.B.Aim = replacementAim;
			replacementEffect = SelectedCombatAction.GetEffectCharge(f.B, f.A); f.B.AddEffect(replacementEffect);
		}
		if (callback == "leave-event") f.B.OnLeaveCombat += _ => Replace();
		if (callback == "effect-removal") f.B.AddEffect(new RemovalProbe(f.B, Replace));
		if (callback == "aim-release")
		{
			oldAim.Setup(x => x.ReleaseEvents()).Callback(Replace);
		}
		var ticket = f.Combat.PrepareCessation(f.B, f.A, f.Combat)!;
		f.Combat.CeaseCombatFor(ticket);
		Assert.AreSame(next, f.B.Combat); Assert.AreSame(f.A, f.B.CombatTarget); Assert.IsTrue(f.B.MeleeRange);
		Assert.AreSame(replacementAim, f.B.Aim); Assert.IsTrue(f.B.Effects.Any(x => ReferenceEquals(x, replacementEffect)));
		Assert.AreNotEqual(TimeSpan.MinValue, f.Scheduler.RemainingDuration(f.B, ScheduleType.Combat));
		Assert.IsFalse(f.Combat.Combatants.Any(x => ReferenceEquals(x, f.B)));
		oldAim.Verify(x => x.ReleaseEvents(), Times.Once);
		oldAim.Raise(x => x.AimInvalidated += null, EventArgs.Empty);
		Assert.AreSame(replacementAim, f.B.Aim, "The captured old aim must not clear replacement aim on later invalidation.");
	}

	[TestMethod]
	public void Cessation_IncomingAcquireRetargetsThirdPartyWithoutRemovingEither()
	{
		var f = new Fixture(); var c = Actor.Create(f.World.Object, 3); f.Add(c); c.CombatTarget = f.A;
		f.A.Acquisition = () => f.A.CombatTarget = c;
		var ticket = f.Combat.PrepareCessation(f.B, f.A, f.Combat)!;
		Assert.AreEqual(CombatCessationChanges.SubjectRemoved | CombatCessationChanges.OpponentPairCleared, f.Combat.CeaseCombatFor(ticket));
		Assert.AreSame(c, f.A.CombatTarget); Assert.AreSame(f.A, c.CombatTarget);
		Assert.AreSame(f.Combat, f.A.Combat); Assert.AreSame(f.Combat, c.Combat); Assert.AreEqual(2, f.Combat.Combatants.Count());
	}

	[TestMethod]
	public void Cessation_ProgLeaveReplacementRunsOnceAndDoesNotDestroyReplacementSchedule()
	{
		var f = new Fixture(); var next = new SimpleMeleeCombat(f.World.Object); var prog = new Mock<IFutureProg>();
		var combat = new ProgCombat("test", "test", "exact-pair", false, null!, prog.Object, null!, null!, null!) { Gameworld = f.World.Object };
		f.Combat = combat; f.Add(f.A); f.Add(f.B); f.A.CombatTarget = f.B; f.B.CombatTarget = f.A;
		prog.Setup(x => x.Execute(It.IsAny<object[]>())).Callback<object[]>(args => { if (ReferenceEquals(args[0], f.B)) next.JoinCombat(f.B); });
		combat.CeaseCombatFor(combat.PrepareCessation(f.B, f.A, combat)!);
		Assert.AreSame(next, f.B.Combat); Assert.AreNotEqual(TimeSpan.MinValue, f.Scheduler.RemainingDuration(f.B, ScheduleType.Combat));
		prog.Verify(x => x.Execute(It.Is<object[]>(x => ReferenceEquals(x[0], f.B) && (string)x[1] == "exact-pair")), Times.Once);
	}

	[TestMethod]
	public void Cessation_ThrowingLeaveObserverKeepsCommittedDetachAndConsumesTicket()
	{
		var f = new Fixture(); var ticket = f.Combat.PrepareCessation(f.B, f.A, f.Combat)!;
		f.B.OnLeaveCombat += _ => throw new InvalidOperationException("observer");
		Assert.ThrowsException<InvalidOperationException>(() => f.Combat.CeaseCombatFor(ticket));
		Assert.IsNull(f.B.Combat); Assert.IsFalse(f.Combat.Combatants.Any(x => ReferenceEquals(x, f.B)));
		Assert.AreEqual(TimeSpan.MinValue, f.Scheduler.RemainingDuration(f.B, ScheduleType.Combat)); Assert.IsFalse(ticket.IsCurrent);
	}

	[TestMethod]
	public void Cessation_ThrowingEffectRemovalStillReleasesCapturedAim()
	{
		var f = new Fixture(); var oldAim = new Mock<IAimInformation>(); f.B.Aim = oldAim.Object;
		f.B.AddEffect(new RemovalProbe(f.B, () => throw new InvalidOperationException("effect removal")));
		var ticket = f.Combat.PrepareCessation(f.B, f.A, f.Combat)!;
		Assert.ThrowsException<InvalidOperationException>(() => f.Combat.CeaseCombatFor(ticket));
		oldAim.Verify(x => x.ReleaseEvents(), Times.Once); Assert.IsNull(f.B.Aim); Assert.IsNull(f.B.Combat); Assert.IsFalse(ticket.IsCurrent);
	}

	private sealed class Probe(IPerceivable owner, Action<AdmittedHostileAttack> callback) : Effect(owner), IAdmittedHostileAttackEffect
	{
		internal Func<bool>? Applicable;
		internal void RebindOwner(IPerceivable owner) => Owner = owner;
		public override bool Applies() => Applicable?.Invoke() ?? true;
		public void OnAdmittedHostileAttack(AdmittedHostileAttack attack) => callback(attack);
		protected override string SpecificEffectType => "TestAdmittedAttack";
		public override string Describe(IPerceiver voyeur) => "A test attack observer.";
	}

	[TestMethod]
	public void Cessation_TargetRemovalAbaPreservesCallbackRelationAndDoesNotRecaptureIt()
	{
		var f = new Fixture(); var c = Actor.Create(f.World.Object, 3); f.Add(c); c.CombatTarget = f.A;
		var effect = new TargetRemovalProbe(f.A, () => { f.A.CombatTarget = c; f.A.CombatTarget = f.B; }); f.A.AddEffect(effect);
		var changes = f.Combat.CeaseCombatFor(f.Combat.PrepareCessation(f.B, f.A, f.Combat)!);
		Assert.AreEqual(CombatCessationChanges.SubjectRemoved | CombatCessationChanges.OpponentPairCleared, changes);
		Assert.AreSame(f.Combat, f.A.Combat); Assert.AreSame(f.B, f.A.CombatTarget); Assert.IsNull(f.B.Combat);
	}

	[TestMethod]
	public void Cessation_InapplicableEndEffectIsPreservedAndAdmissionIsIssuerBound()
	{
		var f = new Fixture(); var effect = new RemovalProbe(f.B, () => Assert.Fail("Inapplicable effect was removed.")) { Applicable = () => false };
		f.B.AddEffect(effect); var ticket = f.Combat.PrepareCessation(f.B, f.A, f.Combat)!;
		var foreign = new SimpleMeleeCombat(f.World.Object);
		Assert.AreEqual(CombatCessationChanges.None, foreign.CeaseCombatFor(ticket)); Assert.IsTrue(ticket.IsCurrent);
		f.Combat.CeaseCombatFor(ticket); Assert.IsTrue(f.B.Effects.Any(x => ReferenceEquals(x, effect)));
	}

	private sealed class TargetRemovalProbe(IPerceivable owner, Action callback) : Effect(owner), ICombatEffectRemovedOnTargetChange
	{
		public override void RemovalEffect() => callback();
		protected override string SpecificEffectType => "TestTargetRemoval";
		public override string Describe(IPerceiver voyeur) => "A test target-removal observer.";
	}

	private sealed class RemovalProbe(IPerceivable owner, Action callback) : Effect(owner), IRemoveOnCombatEnd
	{
		internal Func<bool>? Applicable;
		public override bool Applies() => Applicable?.Invoke() ?? true;
		public override void RemovalEffect() => callback();
		protected override string SpecificEffectType => "TestCessationRemoval";
		public override string Describe(IPerceiver voyeur) => "A test removal observer.";
	}

	private sealed class Fixture
	{
		internal readonly Mock<IFuturemud> World = new() { DefaultValue = DefaultValue.Mock };
		internal readonly Scheduler Scheduler = new();
		internal CombatBase Combat;
		internal readonly Actor A;
		internal readonly Actor B;
		internal Fixture()
		{
			World.SetupGet(x => x.Scheduler).Returns(Scheduler); World.Setup(x => x.GetStaticDouble("PostCombatEngageDelaySeconds")).Returns(1);
			Combat = new SimpleMeleeCombat(World.Object); A = Actor.Create(World.Object, 1); B = Actor.Create(World.Object, 2);
			Add(A); Add(B); A.CombatTarget = B; B.CombatTarget = A;
		}
		internal void Add(Actor actor)
		{
			var members = (List<IPerceiver>)typeof(CombatBase).GetField("_combatants", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(Combat)!;
			members.Add(actor); actor.Combat = Combat;
			Scheduler.AddSchedule(new Schedule<IPerceiver>(actor, _ => { }, ScheduleType.Combat, TimeSpan.FromMinutes(1), "captured old work"));
		}
	}

	private sealed class Actor : MudSharp.Character.Character
	{
		private Actor() : base(null!, null!, true) { }
		internal Action? Acquisition;
		internal static Actor Create(IFuturemud world, long id)
		{
			var actor = TestObjectFactory.CreateUninitialized<Actor>();
			typeof(Actor).GetProperty(nameof(Gameworld))!.SetValue(actor, world); actor.SetNoSave(true); actor.Id = id;
			actor.QueuedMoveCommands = new Queue<string>();
			var body = new Mock<IBody>(); body.SetupProperty(x => x.Actor, actor); body.SetupGet(x => x.Gameworld).Returns(world);
			body.SetupGet(x => x.Bodyparts).Returns([]); body.SetupGet(x => x.Effects).Returns([]);
			body.Setup(x => x.EffectsOfType<IAdmittedHostileAttackEffect>(It.IsAny<Predicate<IAdmittedHostileAttackEffect>>())).Returns([]);
			actor.Body = body.Object;
			actor.CombatSettings = Mock.Of<ICharacterCombatSettings>();
			typeof(PerceivedItem).GetProperty(nameof(EffectHandler))!.SetValue(actor, new EffectHandler(actor));
			typeof(MudSharp.Character.Character).GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(actor, CharacterState.Awake);
			return actor;
		}
		public override void AcquireTarget() => Acquisition?.Invoke();
		public override bool HandleEvent(EventType type, params dynamic[] arguments) => false;
		public override string HowSeen(IPerceiver voyeur, bool proper = false, DescriptionType type = DescriptionType.Short,
			bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None) => "test combatant";
	}
}
