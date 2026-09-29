#nullable enable

using MudSharp.Character;
using MudSharp.Combat;

namespace MudSharp.Effects.Interfaces;

/// <summary>An optional ongoing tactic, evaluated after obligatory and explicitly selected actions.</summary>
public interface ICombatTacticEffect : IEffect
{
	bool TrySelectMove(ICharacter actor, out ICombatMove? move);
	void MoveResolved(ICombatMove move, CombatMoveResult result);
}
