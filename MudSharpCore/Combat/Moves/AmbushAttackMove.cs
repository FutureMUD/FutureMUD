#nullable enable

using MudSharp.Body.Position.PositionStates;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Combat.Moves;

public sealed class AmbushAttackMove(ICharacter owner, INaturalAttack attack, ICharacter target)
	: NaturalAttackMove(owner, attack, target)
{
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.AmbushAttack;
	public override string Description => "Ambushing and attempting to seize prey";

	public static bool CanAmbush(ICharacter actor, ICharacter target, IWeaponAttack attack)
	{
		if (actor == target || target.State.IsDead() || attack is not IAmbushAttack ambush || actor.Location != target.Location || !actor.CanSee(target) ||
		    !ambush.SourceLayers.Contains(actor.RoomLayer) || !ambush.DestinationLayers.Contains(target.RoomLayer) ||
		    !actor.Location.Terrain(actor).TerrainLayers.Contains(target.RoomLayer) ||
		    actor.Location.RouteDefinition is not null && RouteSpatialService.Instance.GetProximity(actor, target) > Proximity.Proximate ||
		    !VehicleCombatService.Instance.CanCrossVehicleBoundary(actor, target, false, false, out _) ||
		    actor.RidingMount is not null || !CharacterState.Able.HasFlag(actor.State) || actor.Movement is not null) return false;
		if (actor.RoomLayer == target.RoomLayer) return true;
		if (actor.CombinedEffectsOfType<IEffect>().Any(x => x.IsBlockingEffect("movement") || x.IsBlockingEffect("move") || x is BlockLayerChange) ||
		    actor.Body.AllItems.Any(x => x.PreventsMovement())) return false;
		if (actor.RoomLayer.IsUnderwater())
			return actor.Race.CanSwim && target.RoomLayer.IsHigherThan(actor.RoomLayer) && ((ISwim)actor).CanAscend().Truth;
		if (actor.PositionState == PositionFlying.Instance)
			return actor.CanContinueFlying().Truth && (target.RoomLayer.IsLowerThan(actor.RoomLayer) ? ((IFly)actor).CanDive().Truth : ((IFly)actor).CanAscend().Truth);
		return target.RoomLayer.IsLowerThan(actor.RoomLayer) ? actor.CanClimbDown().Truth : actor.CanClimbUp().Truth;
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterMove(this);
		if (!CanContinueCommand()) return CombatMoveResult.Irrelevant;
		if (!CanAmbush(Assailant, target, Attack) || !Assailant.CanSpendStamina(StaminaCost)) return CombatMoveResult.Irrelevant;
		if (Assailant.RoomLayer != target.RoomLayer)
		{
			// Commit the ingress once. A miss stays on the prey's layer; no free return to cover.
			Assailant.Teleport(Assailant.Location, target.RoomLayer, false, false, target.RoutePositionMetres);
			Assailant.PositionState = Assailant.Location.IsSwimmingLayer(Assailant.RoomLayer)
				? PositionSwimming.Instance : PositionStanding.Instance;
		}
		Assailant.RemoveAllEffects<HideInvis>(fireRemovalAction: true);
		Assailant.MeleeRange = true;
		var result = base.ResolveMove(defenderMove);
		if (!CanContinueCommand()) return result;
		if (!result.MoveWasSuccessful || Attack is not IAmbushAttack { AttemptSeize: true } ambush ||
		    target.State.IsDead() || !Assailant.ColocatedWith(target) || CombatForcedMovementUtilities.HasAnyGrapple(Assailant, target)) return result;
		var offset = (Assailant.CurrentContextualSize(SizeContext.GrappleAttack) - target.CurrentContextualSize(SizeContext.GrappleDefense)) *
		             Gameworld.GetStaticDouble("InitiateGrappleOffsetPerSizeDifference");
		var difficulty = CheckDifficulty.ApplyBonus(offset);
		var resistance = ambush.SeizeDifficulty.ApplyBonus(-offset);
		var seize = Gameworld.GetCheck(CheckType.InitiateGrapple).CheckAgainstAllDifficulties(Assailant, difficulty, null, target);
		var counter = Gameworld.GetCheck(CheckType.CounterGrappleCheck).CheckAgainstAllDifficulties(target,
			target.State.IsDisabled() ? Difficulty.Impossible : resistance, null, Assailant);
		var success = seize[difficulty].IsPass() && (target.State.IsDisabled() ||
			new OpposedOutcome(seize, counter, difficulty, resistance).Outcome == OpposedOutcomeDirection.Proponent);
		if (success) Assailant.AddEffect(new Grappling(Assailant, target));
		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(success
			? "@ seize|seizes hold of $1 following &0's ambush."
			: "@ fail|fails to secure a hold on $1 following &0's ambush.", Assailant, Assailant, target), style: OutputStyle.CombatMessage));
		return result;
	}
}
