using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.RPG.Checks;
using MoreLinq;

namespace MudSharp.Combat.Moves;

public class StandMove : CombatMoveBase
{
    public StandMove() : base()
    {
    }

    public override string Description { get; } = "Attempting to stand up.";
    public override double BaseDelay => 0.1;
    public override double StaminaCost => StandStaminaCost(Gameworld);

    public static double StandStaminaCost(IFuturemud gameworld)
    {
        return gameworld.GetStaticDouble("StandMoveStaminaCost");
    }

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
		using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterMove(this);
		if (!CanContinueCommand()) return CombatMoveResult.Irrelevant;
        // Determine if anybody in the combat opposes them standing up
		var combat = Assailant.Combat;
		var opponent = InternalResponses(combat).Shuffle(Constants.Random).FirstOrDefault();
		if (!CanContinueAfterInternalResponse(combat)) return CombatMoveResult.Irrelevant;
        if (opponent == null || opponent is HelplessDefenseMove)
        {
            // Unopposed
            Assailant.MovePosition(PositionStanding.Instance, PositionModifier.None, null, null, null, true);
            return new CombatMoveResult
            {
                MoveWasSuccessful = true,
                RecoveryDifficulty = Difficulty.Hard
            };
        }

        throw new NotImplementedException();
    }
}
