using MudSharp.Construction.Boundary;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class CombatMoveRoom : CombatMoveBase
{
	#region Overrides of CombatMoveBase

	public ICellExit Direction { get; set; }

	public override string Description => $"Moving towards {Direction.DescribeFor(Assailant, false)}.";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (Assailant.CanMove(Direction))
		{
			if (!(Direction.Exit.Door?.IsOpen ?? true))
			{
				if (Direction.Exit.Door.CanOpen(Assailant.Body))
				{
					Assailant.Body.Open(Direction.Exit.Door, null, null);
				}
			}

			Assailant.Move(Direction);
			return new CombatMoveResult
			{
				MoveWasSuccessful = true,
				RecoveryDifficulty = Difficulty.Normal
			};
		}

		return CombatMoveResult.Irrelevant;
	}

	#endregion
}