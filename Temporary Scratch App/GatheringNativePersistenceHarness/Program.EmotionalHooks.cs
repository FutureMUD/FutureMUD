#nullable enable
using MudSharp.Character;
using MudSharp.Body.Traits;
using System.Reflection;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using MudSharp.NPC.AI;
using Moq;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static bool _emotionalHooks;
	private static int RunEmotionalHooks(string lane)
	{
		_emotionalHooks = true;
		return lane switch
		{
			"melee" => RunRaiseServitorStockChecks(orderedCallbacks: true, selectedMeleeCheckOnly: true, defendedMeleeOnly: true),
			"firearm" => RunRaiseServitorStockChecks(orderedCallbacks: true, firearmAuthorityOnly: true),
			"countershot" => RunRaiseServitorStockChecks(orderedCallbacks: true, countershotAuthorityOnly: true),
			"cessation" => RunRaiseServitorStockChecks(orderedCallbacks: true, emotionalCessationOnly: true),
			_ => throw new ArgumentOutOfRangeException(nameof(lane))
		};
	}

	private sealed class EmotionalAttackProbe(IPerceivable owner, Action<AdmittedHostileAttack>? callback = null)
		: Effect(owner), IAdmittedHostileAttackEffect
	{
		internal readonly List<AdmittedHostileAttack> Attacks = [];
		public void OnAdmittedHostileAttack(AdmittedHostileAttack attack)
		{
			Attacks.Add(attack);
			Owner.RemoveEffect(this, true);
			callback?.Invoke(attack);
		}
		protected override string SpecificEffectType => "NativeAcceptanceAdmittedAttack";
		public override string Describe(IPerceiver voyeur) => "A temporary native acceptance observer.";
	}

	private sealed class EmotionalSibling(IPerceivable owner) : Effect(owner)
	{
		protected override string SpecificEffectType => "NativeAcceptanceSibling";
		public override string Describe(IPerceiver voyeur) => "An unrelated native acceptance effect.";
	}

	private sealed class EmotionalProbeLease : IDisposable
	{
		private readonly ICharacter _recipient;
		private readonly EmotionalSibling _sibling;
		internal readonly EmotionalAttackProbe Probe;
		internal EmotionalProbeLease(ICharacter recipient, Action<AdmittedHostileAttack>? callback = null)
		{
			_recipient = recipient; Probe = new EmotionalAttackProbe(recipient.Body, callback);
			_sibling = new EmotionalSibling(recipient.Body); recipient.Body.AddEffect(Probe); recipient.Body.AddEffect(_sibling);
		}
		internal void Verify(string scenario, bool admitted)
		{
			Require(Probe.Attacks.Count == (admitted ? 1 : 0), $"Exact physical admission count: {scenario}, observed:{Probe.Attacks.Count}.");
			Require(_recipient.Body.Effects.Any(x => ReferenceEquals(x, _sibling)) &&
				_recipient.Body.Effects.Any(x => ReferenceEquals(x, Probe)) == !admitted,
				"An admitted attack must remove only its opted-in child, including a native miss; refusal must retain it.");
			if (admitted) Require(ReferenceEquals(Probe.Attacks.Single().Recipient, _recipient) && Probe.Attacks.Single().OperationIdentity != Guid.Empty,
				"Notification must identify the exact physical recipient and a real admitted operation.");
			Console.WriteLine($"ARMEmotional-attack={scenario} passed count:{Probe.Attacks.Count} operation:{Probe.Attacks.FirstOrDefault()?.OperationIdentity} body:{_recipient.Body.Id} instance:{_recipient.InstanceId} sibling-preserved:true");
		}
		public void Dispose() { _recipient.Body.RemoveEffect(Probe); _recipient.Body.RemoveEffect(_sibling); }
	}

	private static int RunEmotionalCessation(IFuturemud world, Func<string, MudSharp.NPC.NPC> person)
	{
		// This partial host has no production CombatBase.SetupCombat bootstrap.
		typeof(CombatBase).GetProperty("RecoveryTimeExpression", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new TraitExpression("1", world));
		var recovery = new Mock<ICheck>();
		recovery.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<IPerceivable>(),
			It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.CombatRecoveryCheck, Outcome.Pass));
		Mock.Get(world).Setup(x => x.GetCheck(CheckType.CombatRecoveryCheck)).Returns(recovery.Object);
		foreach (var scenario in new[] { "pair", "third-party", "leave-replacement", "target-aba", "combat-aba", "prog-replacement" })
		{
			var a = person("emotional attacker"); var b = person("emotional subject"); var c = person("emotional third");
			var leave = new Mock<IFutureProg>();
			CombatBase combat = scenario == "prog-replacement"
				? new ProgCombat("native acceptance", "fight", "emotional-pair", false, null!, leave.Object, null!, null!, null!) { Gameworld = world }
				: new SimpleMeleeCombat(world);
			combat.JoinCombat(a); combat.JoinCombat(b); a.CombatTarget = b; b.CombatTarget = a;
			if (scenario == "third-party") { combat.JoinCombat(c); c.CombatTarget = a; b.CombatTarget = c; }
			var queued = SelectedCombatAction.GetEffectCharge(b, a); b.AddEffect(queued);
			var ticket = combat.PrepareCessation(b, b.CombatTarget, combat);
			Require(ticket is { IsCurrent: true }, "Native exact-pair admission must capture body, instance, target and combat without callbacks.");
			var replacement = new SimpleMeleeCombat(world);
			void Replace() { replacement.JoinCombat(b); b.CombatTarget = c; b.AddEffect(SelectedCombatAction.GetEffectCharge(b, c)); }
			PerceivableEvent onLeave = _ => Replace();
			if (scenario == "leave-replacement") b.OnLeaveCombat += onLeave;
			if (scenario == "prog-replacement") leave.Setup(x => x.Execute(It.IsAny<object[]>())).Callback<object[]>(args => { if (ReferenceEquals(args[0], b)) Replace(); });
			if (scenario == "target-aba") { b.CombatTarget = null!; b.CombatTarget = a; }
			if (scenario == "combat-aba") { b.Combat = combat; }
			try
			{
				var changes = combat.CeaseCombatFor(ticket!);
				if (scenario.EndsWith("aba", StringComparison.Ordinal))
					Require(changes == CombatCessationChanges.None && ReferenceEquals(b.Combat, combat) && combat.Combatants.Any(x => ReferenceEquals(x, b)), "Same-reference target/combat ABA must refuse the prepared admission.");
				else
				{
					Require(changes.HasFlag(CombatCessationChanges.SubjectRemoved) && !combat.Combatants.Any(x => ReferenceEquals(x, b)) &&
						!b.Effects.Any(x => ReferenceEquals(x, queued)), "One captured native subject and its old selected work must be detached.");
					if (scenario == "pair") Require(a.Combat is null && b.Combat is null && !combat.Combatants.Any() &&
						world.Scheduler.RemainingDuration(a, ScheduleType.Combat) == TimeSpan.MinValue && world.Scheduler.RemainingDuration(b, ScheduleType.Combat) == TimeSpan.MinValue, "Actual two-actor native cessation must remove both old schedules and members.");
					if (scenario == "third-party") Require(ReferenceEquals(a.Combat, combat) && ReferenceEquals(c.Combat, combat) &&
						ReferenceEquals(a.CombatTarget, c) && ReferenceEquals(c.CombatTarget, a) && combat.Combatants.Count() == 2,
						"Native incoming acquisition must preserve the attacker and actual third-party pair.");
					if (scenario.EndsWith("replacement", StringComparison.Ordinal)) Require(ReferenceEquals(b.Combat, replacement) &&
						ReferenceEquals(b.CombatTarget, c) && b.EffectsOfType<ISelectedCombatAction>().Count() == 1 &&
						world.Scheduler.RemainingDuration(b, ScheduleType.Combat) != TimeSpan.MinValue, "Callback-created native combat must retain its schedule, target and newly queued action.");
				}
				Require(!ticket!.IsCurrent && combat.CeaseCombatFor(ticket) == CombatCessationChanges.None, "A stale or consumed native ticket must remain unusable.");
				Console.WriteLine($"ARMEmotional-cessation={scenario} passed changes:{changes} original-members:{combat.Combatants.Count()} subject-instance:{b.InstanceId} subject-body:{b.Body.Id}");
			}
			finally
			{
				b.OnLeaveCombat -= onLeave;
				using var independent = CommandExecutionScope.EnterIndependent();
				foreach (var actor in new[] { a, b, c }) { actor.Combat?.LeaveCombat(actor); world.Scheduler.Destroy(actor); actor.RemoveAllEffects<ISelectedCombatAction>(fireRemovalAction: true); }
			}
		}
		return 0;
	}

	private static void RunEmotionalFirearmControls(ICharacter actor, ICharacter recipient, InternalMagazineGunGameItemComponent gun)
	{
		var first = gun.ChamberedRound!.Parent; var second = gun.MagazineContents.Single();
		var stamina = actor.CurrentStamina; var condition = gun.Parent.Condition;
		var wounds = recipient.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
		Require(gun.Unready(actor), "Prepare a real native empty trigger.");
		using (var empty = new EmotionalProbeLease(recipient))
		{
			gun.Fire(actor, recipient, Outcome.MajorFail, Outcome.MajorPass, new OpposedOutcome(Outcome.Fail, Outcome.Pass), null!, null!, recipient);
			empty.Verify("firearm-empty-trigger", false);
			Require(gun.ChamberedRound is null && gun.MagazineContents.Single() == second && ReferenceEquals(first.GetItemType<MudSharp.GameItems.Interfaces.IHoldable>()!.HeldBy, actor.Body), "Empty trigger must conserve exact ammunition custody.");
		}
		gun.Load(actor);
		Require(gun.Ready(actor), "Chamber the remaining native magazine round after returning held ammunition to the magazine.");
		Require(gun.ChamberedRound!.Parent == second && gun.MagazineContents.Single() == first, "Declare exact accepted and replacement ammunition identities.");
		using (var replacement = new EmotionalProbeLease(recipient, _ =>
		{
			using var independent = CommandExecutionScope.EnterIndependent();
			Require(gun.Unready(actor), "Independent admission callback must return the accepted round.");
			gun.Load(actor);
			Require(gun.Ready(actor), "Independent admission callback must replace the native chamber through actual operations with a free readying hand.");
		}))
		{
			gun.Fire(actor, recipient, Outcome.MajorFail, Outcome.MajorPass, new OpposedOutcome(Outcome.Fail, Outcome.Pass), null!, null!, recipient);
			replacement.Verify("firearm-notification-replacement-ammo", true);
			Require(gun.ChamberedRound!.Parent == first && gun.MagazineContents.Single() == second && ReferenceEquals(second.ContainedIn, gun.Parent) &&
				!first.Deleted && !second.Deleted && Same(actor.CurrentStamina, stamina) && Same(gun.Parent.Condition, condition) &&
				Same(recipient.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun), wounds), "Notification already delivered; stale firing must consume neither accepted nor replacement ammunition and commit no damage or shot costs.");
		}
		Require(gun.ChamberedRound is not null && gun.MagazineContents.Count() == 1, "Retain the declared loaded native fixture after bounded controls.");
	}

	private static void RunEmotionalMultiTarget(IFuturemud world, RetirementHost host, ICharacter caster, ICharacter foe,
		Func<ScriptedAiCharacterInstance> cast, Action<ScriptedAiCharacterInstance> restored,
		Action<ScriptedAiCharacterInstance, ICharacter, string> order, Func<string, MudSharp.NPC.NPC> person,
		IWeaponAttack attack, HashSet<CommandableAI> configured)
	{
		var maximum = attack.MaximumTargets;
		Require(attack.BuildingCommand(caster, new StringStack("multitargets 2")), "Author native two-target attack through the actual builder.");
		try
		{
			foreach (var skipped in new[] { false, true })
			{
				var actor = cast(); var secondary = person("emotional secondary");
				actor.CombatSettings = caster.CombatSettings;
				secondary.CombatSettings = caster.CombatSettings; secondary.PreferredDefenseType = DefenseType.Dodge;
				var ai = actor.AIs.OfType<CommandableAI>().Single();
				if (configured.Add(ai)) Require(ai.BuildingCommand(caster, new StringStack("included learningstrike")), "Allowlist actual multi-target manual command.");
				var weapon = host.Prototypes.Values.Single(x => x.Name == "ARM03B2B flame knife").CreateNew(caster);
				world.Add(weapon); caster.Location.Insert(weapon, true); weapon.Login(); actor.Body.Get(weapon, silent: true); actor.Body.Wield(weapon, silent: true);
				order(actor, caster, "hit opponent"); actor.RemoveAllEffects<IdleCombatant>(fireRemovalAction: true);
				var combat = actor.Combat!; combat.JoinCombat(secondary); secondary.CombatTarget = actor; secondary.MeleeRange = true;
				((MudSharp.Body.Implementations.Body)actor.Body).CurrentStamina = 100;
				((MudSharp.Body.Implementations.Body)foe.Body).CurrentStamina = 100;
				((MudSharp.Body.Implementations.Body)secondary.Body).CurrentStamina = 100;
				using var first = new EmotionalProbeLease(foe); using var second = new EmotionalProbeLease(secondary);
				try
				{
					order(actor, caster, "learningstrike opponent"); var move = actor.ChooseMove();
					Require(move is MultiTargetWeaponAttackMove && move.CharacterTargets.Count() == 2, "Actual manual resolver must construct two native melee child moves.");
					if (skipped) combat.LeaveCombat(secondary);
					combat.CombatAction(actor, move);
					first.Verify("multitarget-first-" + skipped, true); second.Verify("multitarget-secondary-" + skipped, !skipped);
					if (!skipped) Require(first.Probe.Attacks.Single().OperationIdentity != second.Probe.Attacks.Single().OperationIdentity, "Actual admitted children require distinct identities; the wrapper must add no delivery.");
					Require(Same(actor.CurrentStamina, 100 - move.StaminaCost), "Native multi-target wrapper charges its one accepted attack cost.");
					Console.WriteLine($"ARMEmotional-multitarget={(skipped ? "skipped-child" : "two-admitted-children")} passed primary:{first.Probe.Attacks.Count} secondary:{second.Probe.Attacks.Count} actual-wrapper:{move.GetType().Name}");
				}
				finally
				{
					using var independent = CommandExecutionScope.EnterIndependent();
					secondary.Combat?.LeaveCombat(secondary);
					Require(world.SpellOwnedCorpseAnimations!.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var why), why);
					actor.Body.Take(weapon); weapon.Delete(); world.SaveManager.Flush(); restored(actor);
				}
			}
		}
		finally { Require(attack.BuildingCommand(caster, new StringStack("multitargets " + maximum)), "Restore authored maximum target count."); }
	}
}
