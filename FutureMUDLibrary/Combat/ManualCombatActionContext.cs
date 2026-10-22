#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

/// <summary>
/// Immutable optional input captured when a manual combat action is selected.
/// It is retained by queued actions so execution cannot silently change the
/// choice the player made while waiting for their turn.
/// </summary>
public sealed record ManualCombatActionContext(string Selection, long? TargetBodyId = null, long? TargetLimbId = null);

/// <summary>
/// Optional contract for auxiliary effects which need a player-selected
/// context such as a body location. Legacy auxiliary effects remain unchanged.
/// </summary>
public interface IContextualAuxiliaryEffect : IAuxiliaryEffect
{
	bool TryParseContext(ICharacter actor, ICharacter target, StringStack arguments,
		out ManualCombatActionContext? context, out string error);
	bool IsValidContext(ICharacter actor, ICharacter target, ManualCombatActionContext context,
		out string error);
	void ApplyEffect(ICharacter attacker, IPerceiver target, CheckOutcome outcome,
		ManualCombatActionContext context, bool opposedSuccess = false);
}
