#nullable enable

using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Combat.Strategies;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects;
using MudSharp.NPC.AI.Groups;
using MudSharp.RPG.Checks;
using MudSharp.Movement;

namespace MudSharp.NPC.AI;

public partial class AnimalAI
{
	private bool RespondToHuntThreat(ICharacter actor, AnimalHuntEffect hunt)
	{
		var threat = ObservedCharacters(actor).Where(x => x.Id != hunt.TargetId && !IsSociallyTrusted(actor, x))
			.FirstOrDefault(x => x.CombatTarget == actor || AwarenessThreatProg.ExecuteBool(false, actor, x) &&
			                    AssessPrey(actor, x).Score < HuntThreshold(actor, true));
		if (threat is null) return false;
		if (actor.Combat is not null) { hunt.Phase = AnimalHuntPhase.Abandoned; return false; }
		if (!TryAwarenessResponse(actor, threat) && !TryThreatResponse(actor, threat)) return false;
		actor.RemoveEffect(hunt);
		return true;
	}
	internal static bool BeginGroupHunt(ICharacter actor, ICharacter target) => actor is INPC npc &&
		npc.AIs.OfType<AnimalAI>().Any(ai => ai.Hunting.Enabled ? ai.BeginHunt(actor, target) : ai.TryHungryPredatorAttack(actor, target));

	private bool BeginHunt(ICharacter actor, ICharacter target)
	{
		if (actor.EffectsOfType<AnimalHuntEffect>().Any() || !CanHuntTarget(actor, target)) return false;
		var hunt = new AnimalHuntEffect(actor, this, target);
		actor.RemoveAllEffects<FollowingPath>(fireRemovalAction: true);
		actor.AddEffect(hunt, Hunting.PursuitTimeout);
		return AdvanceHunt(actor, hunt);
	}

	private bool TickHunt(ICharacter actor)
	{
		var hunt = actor.EffectsOfType<AnimalHuntEffect>().FirstOrDefault(x => x.AiId == Id);
		if (hunt is not null) return AdvanceHunt(actor, hunt);
		if (actor.Combat is not null || actor.Movement is not null || !CharacterState.Able.HasFlag(actor.State) ||
		    !IsHungry(actor) || NpcSurvivalAIHelpers.IsThirsty(actor) ||
		    IsGroupControlled(actor, GroupAIControlScope.Feeding) ||
		    actor.CombinedEffectsOfType<IEffect>().Any(x => x.IsBlockingEffect("general"))) return false;
		if (PredatorAIHelpers.FindLocalEdibleCorpse(actor) is not null) return false;
		if (PrepareHuntingSite(actor)) return true;
		AcquireRangedTargets(actor);
		var prey = RankPrey(actor, actor.Location.Characters.Concat(actor.SeenTargets.OfType<ICharacter>())).FirstOrDefault();
		if (prey is not null) return BeginHunt(actor, prey);
		// Waiting predators stay at their prepared site instead of wandering after rejected prey.
		return Hunting.Opening != AnimalHuntOpening.Direct;
	}

	private bool PrepareHuntingSite(ICharacter actor)
	{
		if (Hunting.Opening == AnimalHuntOpening.Direct) return false;
		if (Hunting.Opening == AnimalHuntOpening.TrapWait &&
		    ResolveHomeBase(actor).HomeCell is { } homeCell && !ReferenceEquals(homeCell, actor.Location) &&
		    !IsGroupControlled(actor, GroupAIControlScope.Movement))
		{
			// A completed or abandoned pursuit must not turn the retreat cell into a new waiting site.
			CheckPathingEffect(actor, true);
			return true;
		}
		if (Hunting.Opening == AnimalHuntOpening.TrapWait && !ResolveHomeBase(actor).HasAnchor)
		{
			// Constructing the hunting site is how a hungry trap predator obtains food.
			// The ordinary denning idle strategy deliberately waits until hunger is satisfied.
			if (HomeStrategy == AnimalHomeStrategyType.Denning) EvaluateBurrowLifecycle(actor);
			else HomeStrategyHandler.EvaluateIdle(this, actor);
			if (actor.CombinedEffectsOfType<IEffect>().Any(x => x.IsBlockingEffect("general"))) return true;
		}
		if (Hunting.PreferredLayer is { } layer && actor.RoomLayer != layer &&
		    actor.Location.Terrain(actor).TerrainLayers.Contains(layer) && actor.CouldTransitionToLayer(layer))
		{
			var path = new FollowingMultiLayerPath(actor, [], layer, layer);
			actor.AddEffect(path);
			FollowPathAction(actor, path);
			return true;
		}
		if (!actor.AffectedBy<IHideEffect>() && !actor.CombinedEffectsOfType<IEffect>().Any(x => x.IsBlockingEffect("general")))
		{
			actor.ExecuteCommand("hide");
			// A failed hide must not permanently prevent an otherwise legal hunt.
			return actor.Effects.Any(x => x.IsBlockingEffect("general"));
		}
		return false;
	}

	private bool HuntExpired(ICharacter actor, AnimalHuntEffect hunt)
	{
		var origin = Gameworld.Cells.Get(hunt.OriginCellId);
		return RuntimeClock.UtcNow >= hunt.Deadline || RuntimeClock.UtcNow - hunt.LastSeen > Hunting.LostTimeout ||
		       origin is null || actor.DistanceBetween(origin, (uint)Hunting.PursuitRange) < 0 ||
		       NpcSurvivalAIHelpers.IsThirsty(actor) || !IsHungry(actor);
	}

	private bool AdvanceHunt(ICharacter actor, AnimalHuntEffect hunt)
	{
		if (HuntExpired(actor, hunt)) hunt.Phase = AnimalHuntPhase.Abandoned;
		if (hunt.Phase == AnimalHuntPhase.Abandoned)
		{
			if (actor.Combat is not null) return true; // The tactic hook performs an ordinary disengagement.
			actor.RemoveEffect(hunt);
			return false;
		}
		if (actor.Movement is not null) return true;
		var target = hunt.Target;
		var observed = target is not null && CanObserveTarget(actor, target);
		if (observed)
		{
			hunt.Observe(target!);
			if (target!.State.IsDead())
			{
				actor.RemoveEffect(hunt);
				return PredatorAIHelpers.EatLocalCorpseIfHungry(actor);
			}
			if (PreyRejection(actor, target, true) is not null || AssessPrey(actor, target).Score < HuntThreshold(actor, true))
			{
				hunt.Phase = AnimalHuntPhase.Abandoned;
				return true;
			}
		}
		if (actor.Combat is not null)
		{
			if (actor.CombatTarget is ICharacter opponent && opponent.Id != hunt.TargetId)
			{
				actor.RemoveEffect(hunt); // Self-defence against a new attacker owns this fight.
				return false;
			}
			return true;
		}
		if (hunt.Phase == AnimalHuntPhase.Withdrawing) hunt.Phase = AnimalHuntPhase.Shadowing;
		if (actor.CombinedEffectsOfType<IEffect>().Any(x => x.IsBlockingEffect("general") || x.IsBlockingEffect("movement"))) return true;
		if (hunt.Phase == AnimalHuntPhase.Shadowing && observed && !target!.State.IsDisabled())
		{
			// Stay outside melee reach while maintaining a real sighting; never inspect drug doses or hidden health.
			if (actor.ColocatedWith(target)) TryMoveAwayFromAwarenessThreats(actor, [target]);
			return true;
		}
		if (observed && ReferenceEquals(actor.Location, target!.Location))
		{
			QueueHuntEngagement(actor, target, hunt);
			return true;
		}
		if (FollowHuntObservation(actor, hunt)) return true;
		return true; // Bounded by lost-sighting and total deadlines.
	}

	private void QueueHuntEngagement(ICharacter actor, ICharacter target, AnimalHuntEffect hunt)
	{
		if (!actor.CanEngage(target)) { hunt.Phase = AnimalHuntPhase.Abandoned; return; }
		BlockingDelayedAction? delay = null;
		delay = new BlockingDelayedAction(actor, _ =>
		{
			actor.RemoveEffect(delay!);
			var returning = hunt.Phase == AnimalHuntPhase.Shadowing;
			if (!actor.EffectsOfType<AnimalHuntEffect>().Contains(hunt) || HuntExpired(actor, hunt) ||
			    !actor.CanEngage(target) || PreyRejection(actor, target, returning) is not null ||
			    AssessPrey(actor, target).Score < HuntThreshold(actor, returning))
			{
				hunt.Phase = AnimalHuntPhase.Abandoned;
				return;
			}
			if (returning && !target.State.IsDisabled()) return;
			if (hunt.Phase == AnimalHuntPhase.Approach) hunt.Phase = AnimalHuntPhase.Opening;
			else hunt.Phase = AnimalHuntPhase.Fighting;
			PredatorAIHelpers.EngageTarget(actor, target, EngageEmote);
		}, "preparing to hunt", ["general", "combat-engage", "movement"], null);
		actor.AddEffect(delay, TimeSpan.FromMilliseconds(Math.Max(1, Dice.Roll(EngageDelayDiceExpression))));
	}

	private bool FollowHuntObservation(ICharacter actor, AnimalHuntEffect hunt)
	{
		var cell = Gameworld.Cells.Get(hunt.LastCellId);
		if (cell is not null && cell != actor.Location)
		{
			var path = actor.PathBetween(cell, (uint)Hunting.PursuitRange, GetAnimalSuitabilityFunction(actor)).ToList();
			return path.Count > 0 && actor.CanMove(path[0]) && actor.Move(path[0]);
		}
		// Use the same physical track checks as a player search, only at the last observed site.
		// Route-cell tracks require an exact-coordinate path and are deliberately not reduced to an exit.
		if (actor.Location.RouteDefinition is not null) return false;
		var vision = Gameworld.GetCheck(CheckType.SearchForTracksCheck).CheckAgainstAllDifficulties(actor, Difficulty.Normal, null);
		var smell = Gameworld.GetCheck(CheckType.SearchForTracksByScentScheck).CheckAgainstAllDifficulties(actor, Difficulty.Normal, null);
		var exits = actor.Location.Tracks.Where(x => !x.Deleted && x.RoomLayer == actor.RoomLayer && !x.TurnedAround &&
			        x.ToCellExit is not null && (vision[x.VisualTrackDifficulty(actor)].Outcome == Outcome.MajorPass || smell[x.OlfactoryTrackDifficulty(actor)].IsPass()))
			// Those outcomes reveal race, not exact identity. Ambiguous same-race trails must not reveal the prey's route.
			.Where(x => x.Character?.Race.Id == hunt.LastRaceId).Select(x => x.ToCellExit!).Distinct().ToList();
		if (exits.Count != 1) return false;
		var exit = exits[0];
		if (!GetAnimalSuitabilityFunction(actor)(exit) || !actor.CanMove(exit)) return false;
		hunt.FollowDetectedTrail(exit.Destination);
		return actor.Move(exit);
	}

	internal bool SelectHuntMove(ICharacter actor, AnimalHuntEffect hunt, out ICombatMove? move)
	{
		move = null;
		var target = hunt.Target;
		if (target is null || actor.CombatTarget != target) return false;
		if (HuntExpired(actor, hunt) || HuntingAttackPermissionRejection(actor, target) is not null)
			hunt.Phase = AnimalHuntPhase.Abandoned;
		if (CanObserveTarget(actor, target))
		{
			if (PreyRejection(actor, target, true) is not null || AssessPrey(actor, target).Score < HuntThreshold(actor, true))
				hunt.Phase = AnimalHuntPhase.Abandoned;
			else hunt.Observe(target);
		}
		if (hunt.Phase is AnimalHuntPhase.Withdrawing or AnimalHuntPhase.Abandoned)
		{
			if (actor.EffectsOfType<IGrappling>().Any(x => x.Target == target)) move = new DropGrappledTargetMove(actor, target);
			else if (CombatForcedMovementUtilities.HasClinch(actor, target) && actor.CanSpendStamina(BreakClinchMove.MoveStaminaCost(actor)))
				move = new BreakClinchMove(actor, target);
			else if (!actor.CombinedEffectsOfType<IBeingGrappled>().Any() && actor.CanMove(CanMoveFlags.IgnoreCancellableActionBlockers))
				move = new FleeMove { Assailant = actor };
			return true;
		}
		if (hunt.Phase == AnimalHuntPhase.Opening && Hunting.Opening == AnimalHuntOpening.Ambush &&
		    Hunting.Followup != AnimalHuntFollowup.VenomWithdrawal)
		{
			var attack = actor.Race.UsableNaturalWeaponAttacks(actor, target, false, BuiltInCombatMoveType.AmbushAttack)
				.Where(x => actor.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(actor, x.Attack)))
				.FirstOrDefault(x => AmbushAttackMove.CanAmbush(actor, target, x.Attack));
			if (attack is not null) { move = new AmbushAttackMove(actor, attack, target); return true; }
			hunt.Phase = AnimalHuntPhase.Fighting;
		}
		if (Hunting.Followup == AnimalHuntFollowup.Extract && actor.ColocatedWith(target))
		{
			move = SelectExtraction(actor, target);
			if (move is not null) return true;
		}
		if (Hunting.Followup == AnimalHuntFollowup.VenomWithdrawal && !hunt.VenomDelivered && actor.ColocatedWith(target) && actor.MeleeRange)
		{
			var clinch = CombatForcedMovementUtilities.HasClinch(actor, target);
			var attack = actor.Race.UsableNaturalWeaponAttacks(actor, target, false,
					clinch ? BuiltInCombatMoveType.EnvenomingAttackClinch : BuiltInCombatMoveType.EnvenomingAttack)
				.Where(x => actor.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(actor, x.Attack))).GetWeightedRandom(x => x.Attack.Weighting);
			if (attack is not null)
			{
				move = clinch ? new EnvenomingClinchAttack(actor, target, attack, null) : new EnvenomingAttackMove(actor, attack, target);
				return true;
			}
		}
		return false;
	}

	private ICombatMove? SelectExtraction(ICharacter actor, ICharacter target)
	{
		if (!Hunting.PreferredLayer.HasValue) return null;
		if (Hunting.PreferredLayer.Value.IsUnderwater() && DrownerStrategy.TargetIsInnatelyWaterSafe(target)) return null;
		if (!CombatForcedMovementUtilities.CanHaulTarget(actor, target)) return null;
		if (!CombatForcedMovementUtilities.HasControlledGrapple(actor, target))
			return GrappleForControlStrategy.Instance.AttemptGrappleForControlOnly(actor);
		var layer = Hunting.PreferredLayer.Value;
		if (actor.RoomLayer == layer) return null;
		if (actor.Location.Terrain(actor).TerrainLayers.Contains(layer) && actor.CouldTransitionToLayer(layer))
		{
			var choice = CombatForcedMovementUtilities.FindBestForcedMovementAttack(actor, target, ForcedMovementVerbs.Pull, ForcedMovementTypes.Layer);
			return choice is null ? null : new ForcedMovementMove(actor, target, choice.Attack, ForcedMovementVerbs.Pull, layer)
				{ Weapon = choice.Weapon, NaturalAttack = choice.NaturalAttack };
		}
		var exit = actor.Location.ExitsFor(actor).Where(GetAnimalSuitabilityFunction(actor))
			.FirstOrDefault(x => x.Destination.Terrain(actor).TerrainLayers.Contains(layer));
		if (exit is null) return null;
		var exitChoice = CombatForcedMovementUtilities.FindBestForcedMovementAttack(actor, target, ForcedMovementVerbs.Pull, ForcedMovementTypes.Exit);
		return exitChoice is null ? null : new ForcedMovementMove(actor, target, exitChoice.Attack, ForcedMovementVerbs.Pull, exit)
			{ Weapon = exitChoice.Weapon, NaturalAttack = exitChoice.NaturalAttack };
	}

	internal void HuntMoveResolved(AnimalHuntEffect hunt, ICombatMove move, CombatMoveResult result)
	{
		if (move is AmbushAttackMove) hunt.Phase = AnimalHuntPhase.Fighting;
		if (Hunting.Followup == AnimalHuntFollowup.VenomWithdrawal && !hunt.VenomDelivered && result.EnvenomDelivered &&
		    move.CharacterTargets.Any(x => x.Id == hunt.TargetId))
		{
			hunt.RecordVenomDelivery();
			hunt.Phase = AnimalHuntPhase.Withdrawing;
		}
	}
}
