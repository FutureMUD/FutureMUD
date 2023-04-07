using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class RemoveAndWieldMove : CombatMoveBase
{
	public IWieldable Item { get; set; }
	public override double BaseDelay => 0.3;

	#region Overrides of CombatMoveBase

	public override string Description { get; } = "Removing and wielding an item";

	#endregion

	#region Overrides of CombatMoveBase

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		Assailant.Body.RemoveItem(Item.Parent, null, ignoreFlags: ItemCanGetIgnore.IgnoreWeight);
		Assailant.Body.Wield(Item.Parent);
		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			RecoveryDifficulty = Difficulty.Normal
		};
	}

	#endregion
}