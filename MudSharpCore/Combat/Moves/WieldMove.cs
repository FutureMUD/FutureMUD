using System.Linq;
using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Combat.Moves;

public class WieldMove : CombatMoveBase
{
	public IWieldable Item { get; set; }

	public IWield SpecificHand { get; set; }

	public IEmote PlayerEmote { get; set; }

	public ItemCanWieldFlags Flags { get; set; }

	#region Overrides of CombatMoveBase

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (Assailant.Body.HeldOrWieldedItems.All(x => Item.Parent != x))
		{
			Assailant.Send("You no longer possess the item that you wanted to wield.");
			return new CombatMoveResult();
		}

		Assailant.Body.Wield(Item.Parent, SpecificHand, PlayerEmote, flags: Flags);
		return new CombatMoveResult
		{
			MoveWasSuccessful = true
		};
	}

	#endregion

	#region Overrides of CombatMoveBase

	public override string Description { get; } = "Wielding an item";
	public override double BaseDelay => 0.1;

	#endregion
}