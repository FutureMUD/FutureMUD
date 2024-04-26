using System.Linq;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Combat.Moves;

public class WearItemMove : CombatMoveBase
{
	public IGameItem Item { get; init; }

	public IEmote PlayerEmote { get; init; }

	public string SpecificProfile { get; init; }

	#region Overrides of CombatMoveBase

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (!Assailant.Body.HeldOrWieldedItems.Contains(Item))
		{
			Assailant.Send("You are no longer in possession of the item that you wanted to wear.");
			return new CombatMoveResult();
		}

		Assailant.Body.Wear(Item, SpecificProfile, PlayerEmote);
		return new CombatMoveResult
		{
			MoveWasSuccessful = true
		};
	}

	#endregion

	#region Overrides of CombatMoveBase

	public override string Description { get; } = "Wearing an item";
	public override double BaseDelay => 0.1;

	#endregion
}