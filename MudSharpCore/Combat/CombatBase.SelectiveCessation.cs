#nullable enable
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using NativeCharacter = MudSharp.Character.Character;

namespace MudSharp.Combat;

public abstract partial class CombatBase
{
	private sealed class CessationParticipant(NativeCharacter actor)
	{
		internal readonly NativeCharacter Actor = actor;
		private readonly IBody _body = actor.Body;
		private readonly ICharacter _focus = actor.Body.Actor;
		private readonly long _instance = actor.CombatInstanceIdentity;
		private readonly CharacterState _state = actor.State;
		private readonly bool _embodied = actor.IsEmbodied;
		private readonly object _room = actor.Location;
		internal ICombat? Combat = actor.Combat;
		internal IPerceiver? Target = actor.CombatTarget;
		internal long Version = actor.CombatMutationVersion;
		internal bool IsCurrent => ReferenceEquals(Actor.Body, _body) && ReferenceEquals(_body.Actor, _focus) &&
			Actor.CombatInstanceIdentity == _instance && Actor.IsEmbodied == _embodied && Actor.State == _state && ReferenceEquals(Actor.Location, _room) &&
			ReferenceEquals(Actor.Combat, Combat) && ReferenceEquals(Actor.CombatTarget, Target) &&
			Actor.CombatMutationVersion == Version;
	}

	private sealed class CessationAdmission(CombatBase issuer, CessationParticipant subject,
		CessationParticipant? opponent, CessationParticipant[] incoming) : ICombatCessationAdmission
	{
		internal readonly CombatBase Issuer = issuer;
		internal readonly CessationParticipant Subject = subject;
		internal readonly CessationParticipant? Opponent = opponent;
		internal readonly CessationParticipant[] Incoming = incoming;
		internal bool Used;
		public bool IsCurrent => !Used && Subject.IsCurrent &&
			ReferenceEquals(Subject.Combat, Issuer) && Issuer._combatants.Any(x => ReferenceEquals(x, Subject.Actor)) &&
			(Opponent is null || Opponent.IsCurrent && Issuer._combatants.Any(x => ReferenceEquals(x, Opponent.Actor))) &&
			Incoming.All(x => x.IsCurrent && Issuer._combatants.Any(y => ReferenceEquals(x.Actor, y))) &&
			Issuer._combatants.Where(x => ReferenceEquals(x.CombatTarget, Subject.Actor)).All(x =>
				Incoming.Any(y => ReferenceEquals(x, y.Actor)));
	}

	public ICombatCessationAdmission? PrepareCessation(IPerceiver subject, IPerceiver? opponent, ICombat expectedCombat)
	{
		if (this is not ICombatSelectiveCessation || !ReferenceEquals(this, expectedCombat) ||
			subject is not NativeCharacter actor || !ReferenceEquals(actor.Combat, this) ||
			!ReferenceEquals(actor.CombatTarget, opponent) || !_combatants.Any(x => ReferenceEquals(x, actor)) ||
			ReferenceEquals(subject, opponent)) return null;
		if (opponent is not null && (opponent is not NativeCharacter || !ReferenceEquals(opponent.Combat, this) ||
			!_combatants.Any(x => ReferenceEquals(x, opponent)))) return null;
		var incoming = _combatants.Where(x => ReferenceEquals(x.CombatTarget, subject) && !ReferenceEquals(x, subject)).ToArray();
		// Native field commits are required. Arbitrary IPerceiver implementations retain ordinary LeaveCombat.
		if (incoming.Any(x => x is not NativeCharacter || !ReferenceEquals(x.Combat, this))) return null;
		var ticket = new CessationAdmission(this, new CessationParticipant(actor),
			opponent is NativeCharacter target ? new CessationParticipant(target) : null,
			incoming.Cast<NativeCharacter>().Select(x => new CessationParticipant(x)).ToArray());
		return ticket.IsCurrent ? ticket : null;
	}

	public CombatCessationChanges CeaseCombatFor(ICombatCessationAdmission admission)
	{
		if (admission is not CessationAdmission ticket || !ReferenceEquals(ticket.Issuer, this) || !ticket.IsCurrent)
			return CombatCessationChanges.None;
		ticket.Used = true;
		Detach(ticket.Subject);
		var changes = CombatCessationChanges.SubjectRemoved;
		foreach (var incoming in ticket.Incoming)
		{
			if (!incoming.IsCurrent || !_combatants.Any(x => ReferenceEquals(x, incoming.Actor))) continue;
			var effects = incoming.Actor.SelectiveTargetEffects();
			incoming.Actor.CommitSelectiveTargetClear();
			incoming.Target = null;
			incoming.Version++;
			if (ReferenceEquals(incoming.Actor, ticket.Opponent?.Actor)) changes |= CombatCessationChanges.OpponentPairCleared;
			RemoveCapturedEffects(incoming, effects);
			if (!incoming.IsCurrent) continue;
			// Native acquisition may choose a third party or replace combat. No write follows such a callback.
			incoming.Actor.AcquireTarget();
			if (!incoming.IsCurrent) continue;
			if (!_combatants.Any(x => ReferenceEquals(x.CombatTarget, incoming.Actor))) Detach(incoming);
		}
		// Only this operation's captured participants may leave; ordinary EndCombat recursively leaves others.
		if (_combatants.Count == 0)
		{
			EndCombatEvent();
			if (_combatants.Count == 0 && this is ProgCombat prog) prog.OnCombatEndProg?.Execute(prog.CombatReference);
		}
		return changes;
	}

	private void Detach(CessationParticipant participant)
	{
		if (!participant.IsCurrent) return;
		var index = _combatants.FindIndex(x => ReferenceEquals(x, participant.Actor));
		if (index < 0) return;
		_combatants.RemoveAt(index);
		var actor = participant.Actor;
		var effects = actor.SelectiveLeaveEffects();
		Gameworld.Scheduler.Destroy(actor, ScheduleType.Combat);
		var releaseAim = actor.CommitSelectiveDetach(actor);
		participant.Combat = null;
		participant.Target = null;
		participant.Version++;
		// All raw fields and old schedules are gone before a callback can join replacement combat.
		try { RemoveCapturedEffects(participant, effects); }
		finally { releaseAim(); }
		if (!participant.IsCurrent) return;
		actor.NotifySelectiveLeave();
		if (!participant.IsCurrent) return;
		actor.HandleEvent(EventType.NoLongerEngagedInMelee, actor);
		if (!participant.IsCurrent) return;
		actor.HandleEvent(EventType.LeaveCombat, actor);
		if (!participant.IsCurrent) return;
		if (this is ProgCombat prog) prog.OnLeaveProg?.Execute(actor, prog.CombatReference);
		if (!participant.IsCurrent) return;
		if (this is SimpleMeleeCombat)
			actor.AddEffect(new EngageDelay(actor), TimeSpan.FromSeconds(Gameworld.GetStaticDouble("PostCombatEngageDelaySeconds")));
	}

	private static void RemoveCapturedEffects(CessationParticipant participant, IEnumerable<IEffect> effects)
	{
		foreach (var effect in effects)
		{
			if (!participant.IsCurrent) return;
			if (!participant.Actor.Effects.Any(x => ReferenceEquals(x, effect))) continue;
			var applicable = effect.Applies();
			if (!participant.IsCurrent) return;
			if (applicable && participant.Actor.Effects.Any(x => ReferenceEquals(x, effect))) participant.Actor.RemoveEffect(effect, true);
		}
	}
}
