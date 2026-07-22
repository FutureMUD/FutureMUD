#nullable enable

using MudSharp.Construction;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

/// <summary>
/// Advances a combatant longitudinally inside a RouteCell. RouteCell occupants can share a
/// concrete cell while still being kilometres apart, so ordinary room advance cannot simply
/// toggle melee range.
/// </summary>
public sealed class AdvanceAlongRouteMove : CombatMoveBase
{
	public override string Description => "Advancing along the route towards a combat target";

	public override double StaminaCost => MoveToMeleeMove.MoveStaminaCost(Assailant);

	public override double BaseDelay => 1.0;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (Assailant.CombatTarget is not ICharacter target ||
			!ReferenceEquals(Assailant.Location, target.Location) ||
			Assailant.Location.RouteDefinition is null ||
			Assailant.RoomLayer != target.RoomLayer)
		{
			return CombatMoveResult.Irrelevant;
		}

		if (!RouteCombatMovementUtilities.TryAdvanceTowardImmediate(Assailant, target))
		{
			return CombatMoveResult.Irrelevant;
		}

		Assailant.OutputHandler.Handle(new EmoteOutput(
			new Emote("@ advance|advances along the route towards $1.", Assailant, Assailant, target),
			style: OutputStyle.CombatMessage,
			flags: OutputFlags.InnerWrap));
		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			RecoveryDifficulty = Difficulty.Normal
		};
	}
}
