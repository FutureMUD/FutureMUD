#nullable enable

using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Events;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.NPC.AI;

namespace MudSharp.Combat;

public abstract partial class CombatBase : IAdditionalCombatActionResolver
{
	private readonly HashSet<ICharacter> _additionalActions = new(ReferenceEqualityComparer.Instance);
	private readonly HashSet<ICharacter> _attackNotifications = new(ReferenceEqualityComparer.Instance);

	private void NotifyAttackResolved(IPerceiver perceiver, ICombatMove move, CombatMoveResult result)
	{
		static bool Physical(ICombatMove candidate) => candidate is MeleeWeaponAttack or NaturalAttackMove;
		var physical = Physical(move) || move is ICombatMoveSequence sequence && sequence.Resolutions.Any(x => x.Executed && Physical(x.Move)) ||
			move is MultiTargetCombatMove multi && multi.Resolutions.Any(x => Physical(x.Move));
		if (!physical || ReferenceEquals(result, CombatMoveResult.Irrelevant) || perceiver is not ICharacter actor ||
			actor.CombatTarget is not ICharacter target || target.State.IsDead() || actor.State.IsDisabled() ||
			!ReferenceEquals(actor.Combat, this) || _additionalActions.Contains(actor) || !_attackNotifications.Add(actor)) return;
		try { actor.HandleEvent(EventType.CharacterCombatAttackResolved, actor, target, result.MoveWasSuccessful); }
		finally { _attackNotifications.Remove(actor); }
	}

	public bool TryResolveAdditionalAction(ICharacter actor, ICombatMove move)
		=> TryResolveAdditionalAction(actor, () => move);

	public bool TryResolveAdditionalAction(ICharacter actor, Func<ICombatMove?> selectMove)
	{
		bool Usable() => ReferenceEquals(actor.Combat, this) && !actor.State.IsDisabled() &&
			!actor.State.HasFlag(CharacterState.Paralysed) && actor.Movement is null && actor.RidingMount?.Movement is null &&
			actor.CombatSettings is not null && !actor.Effects.Any(x => x.IsBlockingEffect("combat")) &&
			!actor.AffectedBy<AdditionalCombatActionRecovery>() && _combatants.Any(x => ReferenceEquals(x, actor));
		if (!_additionalActions.Add(actor)) return false;
		try
		{
			if (!Usable()) return false;
			var body = actor.Body;
			var room = actor.Location; var layer = actor.RoomLayer; var instance = actor.InstanceId;
			bool Current() => Usable() && ReferenceEquals(actor.Body, body) && ReferenceEquals(actor.Location, room) &&
				actor.RoomLayer == layer && actor.InstanceId == instance;
			var move = selectMove();
			if (move is null || !Current() || !ReferenceEquals(move.Assailant, actor) || !CommandExecutionAuthority.MayExecute(move, actor)) return false;
			using var execution = CommandExecutionScope.EnterMove(move);
			actor.RemoveAllEffects(x => x is IEndOnCombatMove ending && ending.CausesToEnd(move), true);
			if (!Current() || !CommandExecutionScope.TryContinue()) return false;
			var target = move.CharacterTargets.FirstOrDefault();
			var targetBody = target?.Body;
			var response = move is MultiTargetCombatMove or ICombatMoveSequence ? null : target?.ResponseToMove(move, actor);
			if (!CommandExecutionScope.TryContinue() || !Current() ||
				target is not null && !ReferenceEquals(target.Body, targetBody)) return false;
			var result = move is ICombatMoveSequence sequence ? ResolveSequence(sequence) : CommandExecutionScope.Resolve(move, response);
			if (ReferenceEquals(result, CombatMoveResult.Irrelevant)) return false;
			if (move is not ICombatMoveSequence)
				CompleteMoveResolution(actor, move, result.DefenderResponseWasUsed ? response : null, result, true, body, targetBody, target);
			foreach (var tactic in actor.EffectsOfType<ICombatTacticEffect>().ToList())
			{
				if (!Current() || !CommandExecutionScope.TryContinue()) break;
				tactic.MoveResolved(move, result);
			}
			if (Current())
				actor.AddEffect(new AdditionalCombatActionRecovery(actor), GetCombatDelay(actor, result.RecoveryDifficulty, move.BaseDelay));
			if (ReferenceEquals(actor.Combat, this) && !actor.CheckCombatStatus()) LeaveCombat(actor);
			if (target is not null && ReferenceEquals(target.Combat, this) && !target.CheckCombatStatus()) LeaveCombat(target);
			return true;
		}
		finally { _additionalActions.Remove(actor); }
	}
}
