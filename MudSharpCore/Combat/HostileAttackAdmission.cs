#nullable enable
using System.Threading;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.NPC.AI;

namespace MudSharp.Combat;

/// <summary>Delivery lives only for the executing attempt; no queued or persistent identity cache.</summary>
internal sealed class HostileAttackAdmission : IDisposable
{
	private static readonly AsyncLocal<HostileAttackAdmission?> Current = new();
	private readonly HostileAttackAdmission? _previous;
	private readonly ICharacter _attacker;
	private readonly object? _execution;
	private readonly HashSet<ICharacter> _recipients = new(ReferenceEqualityComparer.Instance);
	private readonly Func<bool> _initialIdentity;
	private Guid _identity;

	private HostileAttackAdmission(ICharacter attacker, ICharacter? recipient)
	{
		_previous = Current.Value;
		_attacker = attacker;
		_execution = CommandExecutionScope.LearningContext;
		var attackerPin = Pin(attacker);
		var recipientPin = recipient is null ? null : Pin(recipient);
		_initialIdentity = () => attackerPin() && recipientPin?.Invoke() != false;
		Current.Value = this;
	}

	internal static IDisposable BeginAttempt(ICharacter attacker, ICharacter? recipient = null) => new HostileAttackAdmission(attacker, recipient);
	internal static IDisposable EnterComponent(ICharacter attacker, ICharacter? recipient = null) =>
		Current.Value is { } current && ReferenceEquals(current._attacker, attacker) &&
		ReferenceEquals(current._execution, CommandExecutionScope.LearningContext)
			? NoScope.Instance : BeginAttempt(attacker, recipient);

	private static Func<bool> Pin(ICharacter actor)
	{
		var body = actor.Body;
		var focus = body?.Actor;
		var instance = actor.InstanceId;
		var embodied = actor.IsEmbodied;
		var state = actor.State;
		var combat = actor.Combat;
		var target = actor.CombatTarget;
		var cell = actor.Location;
		var layer = actor.RoomLayer;
		var version = (actor as PerceiverItem)?.CombatMutationVersion;
		return () => ReferenceEquals(actor.Body, body) && ReferenceEquals(body?.Actor, focus) &&
			actor.InstanceId == instance && actor.IsEmbodied == embodied && actor.State == state &&
			ReferenceEquals(actor.Combat, combat) && ReferenceEquals(actor.CombatTarget, target) &&
			ReferenceEquals(actor.Location, cell) && actor.RoomLayer == layer &&
			(actor as PerceiverItem)?.CombatMutationVersion == version;
	}

	internal static bool TryNotify(ICharacter attacker, ICharacter? recipient, Func<bool>? exactAdmission = null)
	{
		using var fallback = EnterComponent(attacker, recipient);
		var current = Current.Value!;
		if (!current._initialIdentity() || !CommandExecutionScope.TryContinue(attacker) ||
			exactAdmission?.Invoke() == false || !current._initialIdentity()) return false;
		if (recipient is null) return CommandExecutionScope.TryContinue(attacker) && current._initialIdentity();
		var attackerBody = attacker.Body;
		var recipientBody = recipient.Body;
		var attackerFocus = attackerBody?.Actor;
		var recipientFocus = recipientBody?.Actor;
		var attackerState = attacker.State;
		var recipientState = recipient.State;
		var attackerInstance = attacker.InstanceId;
		var recipientInstance = recipient.InstanceId;
		var attackerCombat = attacker.Combat;
		var recipientCombat = recipient.Combat;
		var attackerCell = attacker.Location;
		var recipientCell = recipient.Location;
		var attackerVersion = (attacker as PerceiverItem)?.CombatMutationVersion;
		var recipientVersion = (recipient as PerceiverItem)?.CombatMutationVersion;
		bool CurrentIdentity() => ReferenceEquals(attacker.Body, attackerBody) && ReferenceEquals(recipient.Body, recipientBody) &&
			ReferenceEquals(attackerBody?.Actor, attackerFocus) && ReferenceEquals(recipientBody?.Actor, recipientFocus) &&
			attacker.State == attackerState && recipient.State == recipientState &&
			attacker.InstanceId == attackerInstance && recipient.InstanceId == recipientInstance &&
			ReferenceEquals(attacker.Combat, attackerCombat) && ReferenceEquals(recipient.Combat, recipientCombat) &&
			ReferenceEquals(attacker.Location, attackerCell) && ReferenceEquals(recipient.Location, recipientCell) &&
			(attacker as PerceiverItem)?.CombatMutationVersion == attackerVersion &&
			(recipient as PerceiverItem)?.CombatMutationVersion == recipientVersion;
		bool CanContinue() => current._initialIdentity() && CurrentIdentity() && CommandExecutionScope.TryContinue(attacker) &&
			CurrentIdentity() && exactAdmission?.Invoke() != false && current._initialIdentity() && CurrentIdentity();
		if (!CanContinue()) return false;
		if (!current._recipients.Add(recipient)) return CanContinue();
		if (current._identity == Guid.Empty) current._identity = Guid.NewGuid();
		var attack = new AdmittedHostileAttack(attacker, recipient, current._identity);
		var effects = recipient.CombinedEffectsOfType<IAdmittedHostileAttackEffect>()
			.Distinct<IAdmittedHostileAttackEffect>(ReferenceEqualityComparer.Instance).ToArray();
		foreach (var effect in effects)
		{
			if (!CanContinue()) return false;
			var owner = effect.Owner;
			if (!ReferenceEquals(owner, recipient) && !ReferenceEquals(owner, recipientBody)) continue;
			if (!owner.EffectsOfType<IAdmittedHostileAttackEffect>().Any(x => ReferenceEquals(x, effect))) continue;
			var applies = effect.Applies();
			if (!CanContinue()) return false;
			if (!applies) continue;
			if (!ReferenceEquals(effect.Owner, owner) || !owner.EffectsOfType<IAdmittedHostileAttackEffect>().Any(x => ReferenceEquals(x, effect))) continue;
			effect.OnAdmittedHostileAttack(attack);
			if (!CanContinue()) return false;
		}
		return CanContinue();
	}

	public void Dispose() => Current.Value = _previous;
	private sealed class NoScope : IDisposable
	{
		internal static readonly NoScope Instance = new();
		public void Dispose() { }
	}
}
