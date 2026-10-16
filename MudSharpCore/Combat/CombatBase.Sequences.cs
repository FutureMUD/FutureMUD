#nullable enable

using MudSharp.Combat.Moves;
using MudSharp.Body;
using MudSharp.NPC.AI;

namespace MudSharp.Combat;

public abstract partial class CombatBase : ICombatSequenceResolver
{
	public CombatMoveResult ResolveSequence(ICombatMoveSequence sequence)
	{
		using var execution = CommandExecutionScope.EnterMove(sequence);
		if (!ReferenceEquals(sequence.OriginalCombat, this) || !CommandExecutionScope.TryContinue() ||
		    !sequence.TryPrepare()) return CombatMoveResult.Irrelevant;
		foreach (var child in sequence.Children)
		{
			CommandExecutionAuthority.Inherit(sequence, child);
			using var childScope = CommandExecutionScope.EnterMove(child);
			if (!CommandExecutionScope.TryContinue() || !sequence.CanExecute(child)) break;
			var attackerBody = child.Assailant.Body;
			var defenderBody = sequence.Target.Body;
			var response = sequence.Target.ResponseToMove(child, sequence.Assailant);
			if (!CommandExecutionScope.TryContinue() || !sequence.CanExecute(child)) break;
			var result = CommandExecutionScope.Resolve(child, response);
			var committed = CommandExecutionScope.HasCommitted;
			var attack = child as MeleeWeaponAttack;
			var executed = committed || attack?.AttackWasAttempted == true;
			var usedResponse = response;
			if ((!result.DefenderResponseWasUsed || ReferenceEquals(result, CombatMoveResult.Irrelevant)) &&
			    attack?.DefenceWasAttempted != true && response is not MagicDefenseMove { Committed: true }) usedResponse = null;
			var attackerCost = executed && child.UsesStaminaWithResult(result) && child.Assailant.Race.RaceUsesStamina ? child.StaminaCost : 0;
			var defenderCost = usedResponse?.Assailant.Race.RaceUsesStamina == true &&
			                   usedResponse.UsesStaminaWithResult(result) ? usedResponse.StaminaCost : 0;
			sequence.RecordResolution(new CombatMoveResolution(child, usedResponse, result, executed, committed, attackerCost, defenderCost));
			if (!executed) break;
			CompleteMoveResolution(child.Assailant, child, usedResponse, result, true, attackerBody, defenderBody, sequence.Target);
			if (!CommandExecutionScope.TryContinue()) break;
		}
		return sequence.CompleteResolution();
	}

	private void CompleteMoveResolution(IPerceiver perceiver, ICombatMove move, ICombatMove? response,
		CombatMoveResult result, bool executed = true, IBody? attackerBody = null, IBody? defenderBody = null,
		ICharacter? capturedTarget = null)
	{
		if (executed && perceiver is ICharacter actor && actor.Race.RaceUsesStamina && move.UsesStaminaWithResult(result))
		{
			if (attackerBody is not null) attackerBody.SpendStamina(move.StaminaCost);
			else actor.SpendStamina(move.StaminaCost);
		}
		if (response?.Assailant is ICharacter defender && defender.Race.RaceUsesStamina && response.UsesStaminaWithResult(result))
		{
			if (defenderBody is not null) defenderBody.SpendStamina(response.StaminaCost);
			else defender.SpendStamina(response.StaminaCost);
		}
		bool Continue() => CommandExecutionScope.TryContinue() && ReferenceEquals(perceiver.Combat, this) &&
			_combatants.Any(x => ReferenceEquals(x, perceiver)) &&
			(attackerBody is null || perceiver is ICharacter current && ReferenceEquals(current.Body, attackerBody)) &&
			(capturedTarget is null || ReferenceEquals(capturedTarget.Body, defenderBody));
		if (!Continue()) return;
		if (!ReferenceEquals(result, CombatMoveResult.Irrelevant)) FireOnUseProg(move);
		if (Continue()) HandleCombatResult(perceiver, move, response!, result);
		if (Continue()) (move as WeaponAttackMove)?.ResolveBloodSpray(result);
	}
}
