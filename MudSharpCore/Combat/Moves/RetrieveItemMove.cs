using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
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
		using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterMove(this);
		if (!CanContinueCommand()) return CombatMoveResult.Irrelevant;
		if (!TargetItem.ColocatedWith(Assailant) || TargetItem.Destroyed || TargetItem.InInventoryOf != null)
        {
            Assailant.Send("The item that you wanted to get is no longer there.");
            return new CombatMoveResult();
        }

		var combat = Assailant.Combat;
		var contestors = InternalResponses(combat).OfType<OpposeRetrieveItemMove>().ToList();
		if (!CanContinueAfterInternalResponse(combat)) return CombatMoveResult.Irrelevant;
        // TODO - contested

        TargetItem.RemoveAllEffects(x => x is CombatNoGetEffect);
		if (!CanContinueAfterInternalResponse(combat)) return CombatMoveResult.Irrelevant;
        Assailant.Body.Get(TargetItem, playerEmote: PlayerEmote);
        Assailant.RemoveAllEffects(x => (x as ICombatGetItemEffect)?.TargetItem == TargetItem);

        return new CombatMoveResult
        {
            MoveWasSuccessful = true,
            RecoveryDifficulty = Difficulty.Normal
        };
    }
}
