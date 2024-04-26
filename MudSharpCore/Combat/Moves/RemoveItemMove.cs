using System.Linq;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Combat.Moves;

public class RemoveItemMove : CombatMoveBase
{
	public IGameItem Item { get; init; }

	public IEmote PlayerEmote { get; init; }

	#region Overrides of CombatMoveBase

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (!Assailant.Body.WornItems.Contains(Item))
		{
			Assailant.Send("You are no longer wearing the item that you wanted to remove.");
			return new CombatMoveResult();
		}

		Assailant.Body.RemoveItem(Item, PlayerEmote, ignoreFlags: ItemCanGetIgnore.IgnoreWeight);
		return new CombatMoveResult
		{
			MoveWasSuccessful = true
		};
	}

	#endregion

	#region Overrides of CombatMoveBase

	public override string Description { get; } = "Removing an item";
	public override double BaseDelay => 0.1;

	#endregion
}