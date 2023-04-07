using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class RetrieveItemMove : CombatMoveBase
{
	public RetrieveItemMove(ICharacter owner, IGameItem targetItem)
	{
		Assailant = owner;
		TargetItem = targetItem;
	}

	public override string Description => "Retrieving a lost item";

	#region Overrides of CombatMoveBase

	public override double BaseDelay => 0.5;

	#endregion

	public IGameItem TargetItem { get; }

	public Emote PlayerEmote { get; set; }

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (TargetItem.Location != Assailant.Location || TargetItem.Destroyed || TargetItem.InInventoryOf != null)
		{
			Assailant.Send("The item that you wanted to get is no longer there.");
			return new CombatMoveResult();
		}

		var contestors =
			Assailant.Combat.Combatants.Where(
				         x => x.CombatTarget == Assailant &&
				              x.ResponseToMove(this, Assailant) is OpposeRetrieveItemMove)
			         .ToList();
		// TODO - contested

		TargetItem.RemoveAllEffects(x => x is CombatNoGetEffect);
		Assailant.Body.Get(TargetItem, playerEmote: PlayerEmote);
		Assailant.RemoveAllEffects(x => (x as ICombatGetItemEffect)?.TargetItem == TargetItem);

		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			RecoveryDifficulty = Difficulty.Normal
		};
	}
}