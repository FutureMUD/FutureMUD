using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.Vehicles;

namespace MudSharp.Combat.Moves;

public sealed class ForcedMovementAttackChoice
{
	public required IForcedMovementAttack Attack { get; init; }
	public IMeleeWeapon Weapon { get; init; }
	public INaturalAttack NaturalAttack { get; init; }

	public bool IsNatural => NaturalAttack is not null;

	public double StaminaCost(ICharacter actor)
	{
		return IsNatural
			? NaturalAttackMove.MoveStaminaCost(actor, NaturalAttack.Attack)
			: MeleeWeaponAttack.MoveStaminaCost(actor, Attack);
	}
}

public static class CombatForcedMovementUtilities
{
	private const double DefaultRouteCellPushbackMetresPerSuccessDegree = 1.0;

	/// <summary>Current full burden and flight anatomy, independently of the ordinary command's already-flying/melee guards.</summary>
	public static bool CanCarryFlying(ICharacter actor, ICharacter target)
	{
		return CanHaulTarget(actor, target) && actor.CanContinueFlying().Truth;
	}

	public static bool CanHaulTarget(ICharacter actor, ICharacter target)
	{
		return HasHaulingCapacity(actor, target) && actor.Movement is null &&
		       !actor.CombinedEffectsOfType<IEffect>().Any(x => x.IsBlockingEffect("movement") || x.IsBlockingEffect("move") || x is BlockLayerChange) &&
		       !actor.Body.AllItems.Any(x => x.PreventsMovement());
	}

	private static bool HasHaulingCapacity(ICharacter actor, ICharacter target)
	{
		var burden = target.Weight + target.Body.ExternalItems.Sum(x => x.Weight) + actor.Body.ExternalItems.Sum(x => x.Weight);
		return CharacterState.Able.HasFlag(actor.State) && actor.RidingMount is null && actor.ColocatedWith(target) &&
		       double.IsFinite(burden) && burden >= 0 && actor.MaximumDragWeight >= burden;
	}

	/// <summary>A held victim is supported by a capable carrier, without pretending the victim can climb or fly.</summary>
	public static bool IsSupportedByGrapple(ICharacter target)
	{
		return target.CombinedEffectsOfType<IBeingGrappled>().Any(x => x.UnderControl &&
			x.Grappling.CharacterOwner is { } carrier && !ReferenceEquals(carrier, target) &&
			carrier.RoomLayer == target.RoomLayer && carrier.PositionState?.SafeFromFalling == true &&
			HasHaulingCapacity(carrier, target) &&
			(carrier.PositionState != PositionFlying.Instance || carrier.CanContinueFlying().Truth) &&
			(carrier.PositionState != PositionClimbing.Instance || carrier.Race.CanClimb));
	}

	public static ForcedMovementAttackChoice FindBestForcedMovementAttack(
		ICharacter actor,
		ICharacter target,
		ForcedMovementVerbs verb,
		ForcedMovementTypes type)
	{
		return GetForcedMovementAttacks(actor, target, verb, type)
			.Where(x => actor.CanSpendStamina(x.StaminaCost(actor)))
			.GetWeightedRandom(x => x.Attack.Weighting);
	}

	public static ForcedMovementAttackChoice FindAnyForcedMovementAttack(
		ICharacter actor,
		ICharacter target,
		ForcedMovementVerbs verb,
		ForcedMovementTypes type)
	{
		return GetForcedMovementAttacks(actor, target, verb, type)
			.GetWeightedRandom(x => x.Attack.Weighting);
	}

	public static IEnumerable<ForcedMovementAttackChoice> GetForcedMovementAttacks(
		ICharacter actor,
		ICharacter target,
		ForcedMovementVerbs verb,
		ForcedMovementTypes type)
	{
		if (actor.CombatTarget != target)
		{
			actor.CombatTarget = target;
		}

		var weaponAttacks = actor.Body.WieldedItems
			.SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
			.SelectMany(x => x.WeaponType
				.UsableAttacks(actor, x.Parent, target, x.HandednessForWeapon(actor), false,
					BuiltInCombatMoveType.ForcedMovement)
				.OfType<IForcedMovementAttack>()
				.Select(y => new ForcedMovementAttackChoice
				{
					Weapon = x,
					Attack = y
				}));

		var naturalAttacks = actor.Race
			.UsableNaturalWeaponAttacks(actor, target, false,
				BuiltInCombatMoveType.ForcedMovementUnarmed,
				BuiltInCombatMoveType.ForcedMovementClinch)
			.Where(x => x.Attack is IForcedMovementAttack)
			.Select(x => new ForcedMovementAttackChoice
			{
				NaturalAttack = x,
				Attack = (IForcedMovementAttack)x.Attack
			});

		return weaponAttacks
			.Concat(naturalAttacks)
			.Where(x => x.Attack.ForcedMovementVerbs.HasFlag(verb))
			.Where(x => x.Attack.ForcedMovementTypes.HasFlag(type))
			.Where(x => IsInRange(actor, target, x.Attack.RequiredRange));
	}

	public static bool HasControlledGrapple(ICharacter actor, ICharacter target)
	{
		return actor.EffectsOfType<IGrappling>()
		            .Any(x => x.Target == target && x.TargetEffect.UnderControl);
	}

	public static bool HasAnyGrapple(ICharacter actor, ICharacter target)
	{
		return actor.EffectsOfType<IGrappling>().Any(x => x.Target == target) ||
		       target.CombinedEffectsOfType<IBeingGrappled>().Any(x => x.Grappling.CharacterOwner == actor);
	}

	public static bool HasClinch(ICharacter actor, ICharacter target)
	{
		return actor.EffectsOfType<ClinchEffect>().Any(x => x.Target == target) ||
		       target.EffectsOfType<ClinchEffect>().Any(x => x.Target == actor);
	}

	public static bool IsInRange(ICharacter actor, ICharacter target, ForcedMovementRange range)
	{
		switch (range)
		{
			case ForcedMovementRange.Melee:
				return actor.ColocatedWith(target) &&
				       actor.RoomLayer == target.RoomLayer &&
				       (actor.MeleeRange || target.MeleeRange || HasClinch(actor, target) || HasAnyGrapple(actor, target));
			case ForcedMovementRange.Clinch:
				return actor.ColocatedWith(target) &&
				       actor.RoomLayer == target.RoomLayer &&
				       HasClinch(actor, target);
			case ForcedMovementRange.Grapple:
				return actor.ColocatedWith(target) &&
				       actor.RoomLayer == target.RoomLayer &&
				       HasAnyGrapple(actor, target);
			default:
				return false;
		}
	}

	public static void ApplyPushback(ICharacter actor, ICharacter target, int degrees)
	{
		if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue()) return;
		var overboard = MudSharp.NPC.AI.CommandExecutionScope.InvokeOwned(target, () => VehicleCombatService.Instance.ResolveDisplacement(target,
			VehicleCombatDisplacementType.Push, Math.Max(1, degrees)));
		if (overboard.FellOverboard) MudSharp.NPC.AI.CommandExecutionScope.MarkCommitted();
		if (overboard.FellOverboard)
		{
			BreakCloseContact(actor, target);
			return;
		}

		if (!overboard.WasApplicable)
		{
			TryApplyRouteCellPushback(actor, target, degrees);
		}

		BreakCloseContact(actor, target);
		target.MeleeRange = false;
		if (actor.CombatTarget == target)
		{
			actor.MeleeRange = false;
		}

		if (actor.Combat is not null)
		{
			foreach (var combatant in actor.Combat.Combatants.OfType<ICharacter>()
			                             .Where(x => x.CombatTarget == target && x.MeleeRange)
			                             .ToList())
			{
				combatant.MeleeRange = false;
			}
		}

		if (target.Combat is not null)
		{
			target.AddEffect(new ClinchCooldown(target, target.Combat),
				TimeSpan.FromSeconds(30 * CombatBase.CombatSpeedMultiplier));
		}

		var delay = actor.Gameworld.GetStaticDouble("PushbackCombatDelayBaseSeconds") +
		            Math.Max(1, degrees) * actor.Gameworld.GetStaticDouble("PushbackCombatDelayPerDegreeSeconds");
		actor.Gameworld.Scheduler.DelayScheduleType(target, ScheduleType.Combat,
			TimeSpan.FromSeconds(delay * CombatBase.CombatSpeedMultiplier));
	}

	internal static bool TryApplyRouteCellPushback(ICharacter actor, ICharacter target, int degrees)
	{
		var actorLocation = RouteSpatialService.Instance.GetEffectiveLocation(actor);
		var targetLocation = RouteSpatialService.Instance.GetEffectiveLocation(target);
		if (actorLocation.Cell is null ||
			targetLocation.Cell is null ||
			!ReferenceEquals(actorLocation.Cell, targetLocation.Cell) ||
			actorLocation.Layer != targetLocation.Layer ||
			actorLocation.Cell.RouteDefinition is not { } route ||
			!actorLocation.RoutePositionMetres.HasValue ||
			!targetLocation.RoutePositionMetres.HasValue ||
			!double.IsFinite(route.LengthMetres) ||
			route.LengthMetres <= 0.0)
		{
			return false;
		}

		var metresPerDegree = actor.Gameworld?.GetStaticDouble(
			"RouteCellPushbackMetresPerSuccessDegree") ?? 0.0;
		if (!double.IsFinite(metresPerDegree) || metresPerDegree <= 0.0)
		{
			metresPerDegree = DefaultRouteCellPushbackMetresPerSuccessDegree;
		}

		var actorPosition = actorLocation.RoutePositionMetres.Value;
		var targetPosition = targetLocation.RoutePositionMetres.Value;
		var direction = targetPosition > actorPosition
			? 1.0
			: targetPosition < actorPosition
				? -1.0
				// Exact overlap has no natural away vector. Choose the farther endpoint;
				// the positive endpoint wins an exact midpoint tie.
				: route.LengthMetres - targetPosition >= targetPosition
					? 1.0
					: -1.0;
		var distance = Math.Max(1, degrees) * metresPerDegree;
		var destination = RouteSpatialService.Instance.ClampPosition(
			route,
			targetPosition + direction * distance);

		RouteSpatialService.Instance.MaterialiseActiveMovement(target);
		target.SetRoutePosition(destination);
		return true;
	}

	public static void BreakCloseContact(ICharacter actor, ICharacter target)
	{
		actor.RemoveAllEffects(x =>
			x.GetSubtype<ClinchEffect>() is ClinchEffect ce && ce.Target == target, true);
		target.RemoveAllEffects(x =>
			x.GetSubtype<ClinchEffect>() is ClinchEffect ce && ce.Target == actor, true);
		actor.RemoveAllEffects(x =>
			x.GetSubtype<Grappling>() is Grappling gr && gr.Target == target, true);
		target.RemoveAllEffects(x =>
			x.GetSubtype<Grappling>() is Grappling gr && gr.Target == actor, true);
		actor.Body.RemoveAllEffects(x =>
			x.GetSubtype<IBeingGrappled>()?.Grappling.CharacterOwner == target, true);
		target.Body.RemoveAllEffects(x =>
			x.GetSubtype<IBeingGrappled>()?.Grappling.CharacterOwner == actor, true);
	}

	public static bool TryForceExitMovement(
		ICharacter actor,
		ICharacter target,
		ICellExit exit,
		ForcedMovementVerbs verb,
		int successDegrees,
		out string why)
	{
		why = string.Empty;
		if (exit?.IsFallExit == true)
		{
			why = "That exit is a fall exit, and cannot be used for controlled forced movement.";
			return false;
		}

		if (exit?.Origin != target.Location)
		{
			why = "That exit is not present where the target is.";
			return false;
		}

		if (!exit.WhichLayersExitAppears().Contains(target.RoomLayer))
		{
			why = "That exit is not accessible from the target's current layer.";
			return false;
		}

		if (!CanForceThroughExit(actor, target, exit, true, out why))
		{
			return false;
		}

		var targetTransition = exit.MovementTransition(target);
		if (targetTransition.TransitionType == CellMovementTransition.NoViableTransition)
		{
			why = "The target cannot be forced through that exit from this layer.";
			return false;
		}

		var wasMelee = actor.MeleeRange || target.MeleeRange;
		var wasClinch = HasClinch(actor, target);
		var wasGrapple = HasAnyGrapple(actor, target);
		var displacementType = verb == ForcedMovementVerbs.Pull
			? VehicleCombatDisplacementType.Pull
			: VehicleCombatDisplacementType.Push;
		if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue()) return false;
		var overboard = MudSharp.NPC.AI.CommandExecutionScope.InvokeOwned(target, () => VehicleCombatService.Instance.ResolveDisplacement(target, displacementType,
			Math.Max(1, successDegrees)));
		if (overboard.FellOverboard) MudSharp.NPC.AI.CommandExecutionScope.MarkCommitted();
		if (overboard.FellOverboard)
		{
			BreakCloseContact(actor, target);
			return true;
		}

		if (overboard.WasApplicable)
		{
			why = $"{target.HowSeen(actor, true)} keeps their footing aboard {overboard.Vehicle?.ExteriorItem?.HowSeen(actor) ?? "the vehicle"}.";
			return false;
		}

		if (verb == ForcedMovementVerbs.Pull)
		{
			if (!CanHaulTarget(actor, target))
			{
				why = "You cannot haul that target with your current burden or movement restrictions.";
				return false;
			}
			if (exit.Origin != actor.Location)
			{
				why = "You are not at that exit.";
				return false;
			}

			if (!exit.WhichLayersExitAppears().Contains(actor.RoomLayer))
			{
				why = "That exit is not accessible from your current layer.";
				return false;
			}

			if (!CanForceThroughExit(actor, actor, exit, false, out why))
			{
				return false;
			}

			var actorTransition = exit.MovementTransition(actor);
			if (actorTransition.TransitionType == CellMovementTransition.NoViableTransition)
			{
				why = "You cannot move through that exit from this layer.";
				return false;
			}

			var actorLayer = actorTransition.TargetLayer;
			if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue()) return false;
			actor.Teleport(exit.Destination, actorLayer, false, false);
			if (!ReferenceEquals(actor.Location, exit.Destination) || actor.RoomLayer != actorLayer)
			{
				BreakCloseContact(actor, target);
				return false;
			}
			if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue())
			{
				BreakCloseContact(actor, target);
				return false;
			}
			if (!MudSharp.NPC.AI.CommandExecutionScope.InvokeOwned(target, () => { target.Teleport(exit.Destination, actorLayer, false, false); return MudSharp.NPC.AI.CommandExecutionScope.TryContinue(target) && ReferenceEquals(target.Location, exit.Destination) && target.RoomLayer == actorLayer; }))
			{
				BreakCloseContact(actor, target);
				return false;
			}
			ZeroGravityMovementHelper.EnsureFloating(actor);
			ZeroGravityMovementHelper.EnsureFloating(target);
			PreserveCloseContact(actor, target, wasMelee, wasClinch, wasGrapple);
			return true;
		}

		if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue()) return false;

			if (!MudSharp.NPC.AI.CommandExecutionScope.InvokeOwned(target, () => { target.Teleport(exit.Destination, targetTransition.TargetLayer, false, false); return MudSharp.NPC.AI.CommandExecutionScope.TryContinue(target) && ReferenceEquals(target.Location, exit.Destination) && target.RoomLayer == targetTransition.TargetLayer; }))
			{
				BreakCloseContact(actor, target);
				return false;
			}
		ZeroGravityMovementHelper.EnsureFloating(target);
		BreakCloseContact(actor, target);
		ApplyFallIfNeeded(target, false);
		return true;
	}

	public static bool TryForceLayerMovement(
		ICharacter actor,
		ICharacter target,
		RoomLayer layer,
		ForcedMovementVerbs verb,
		int successDegrees,
		out string why,
		bool requiresFlight = false)
	{
		why = string.Empty;
		var terrain = target.Location.Terrain(target);
		if (!terrain.TerrainLayers.Contains(layer))
		{
			why = "That layer does not exist in the target's current location.";
			return false;
		}

		var wasMelee = actor.MeleeRange || target.MeleeRange;
		var wasClinch = HasClinch(actor, target);
		var wasGrapple = HasAnyGrapple(actor, target);
		var displacementType = verb == ForcedMovementVerbs.Pull
			? VehicleCombatDisplacementType.Pull
			: VehicleCombatDisplacementType.Push;
		if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue()) return false;
		var overboard = MudSharp.NPC.AI.CommandExecutionScope.InvokeOwned(target, () => VehicleCombatService.Instance.ResolveDisplacement(target, displacementType,
			Math.Max(1, successDegrees)));
		if (overboard.FellOverboard) MudSharp.NPC.AI.CommandExecutionScope.MarkCommitted();
		if (overboard.FellOverboard)
		{
			BreakCloseContact(actor, target);
			return true;
		}

		if (overboard.WasApplicable)
		{
			why = $"{target.HowSeen(actor, true)} keeps their footing aboard {overboard.Vehicle?.ExteriorItem?.HowSeen(actor) ?? "the vehicle"}.";
			return false;
		}

		if (verb == ForcedMovementVerbs.Pull)
		{
			if (actor.Location != target.Location)
			{
				why = "You must be in the same location as the target to pull them to another layer.";
				return false;
			}

			var flying = requiresFlight || layer.In(RoomLayer.InAir, RoomLayer.HighInAir) ||
			             actor.PositionState == PositionFlying.Instance && layer.IsHigherThan(actor.RoomLayer);
			if (flying && (!HasControlledGrapple(actor, target) || !CanCarryFlying(actor, target)))
			{
				why = "You cannot carry that target in flight.";
				return false;
			}
			if (!flying && (!CanHaulTarget(actor, target) || !actor.CouldTransitionToLayer(layer)))
			{
				why = $"You cannot move to the {layer.DescribeEnum().ColourValue()} layer.";
				return false;
			}

			if (flying) actor.PositionState = PositionFlying.Instance;
			else if (actor.Location.IsSwimmingLayer(layer)) actor.PositionState = PositionSwimming.Instance;
			else if (layer.In(RoomLayer.InTrees, RoomLayer.HighInTrees, RoomLayer.OnRooftops)) actor.PositionState = PositionClimbing.Instance;
			var routePosition = actor.RoutePositionMetres;
			if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue()) return false;
			var destination = actor.Location;
			actor.Teleport(destination, layer, false, false, routePosition);
			if (!ReferenceEquals(actor.Location, destination) || actor.RoomLayer != layer)
			{
				BreakCloseContact(actor, target);
				return false;
			}
			if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue())
			{
				BreakCloseContact(actor, target);
				return false;
			}
			if (!MudSharp.NPC.AI.CommandExecutionScope.InvokeOwned(target, () => { target.Teleport(destination, layer, false, false, routePosition); return MudSharp.NPC.AI.CommandExecutionScope.TryContinue(target) && ReferenceEquals(target.Location, destination) && target.RoomLayer == layer; }))
			{
				BreakCloseContact(actor, target);
				return false;
			}
			ZeroGravityMovementHelper.EnsureFloating(actor);
			ZeroGravityMovementHelper.EnsureFloating(target);
			PreserveCloseContact(actor, target, wasMelee, wasClinch, wasGrapple);
			return true;
		}

		if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue()) return false;

			if (!MudSharp.NPC.AI.CommandExecutionScope.InvokeOwned(target, () => { var destination = target.Location; target.Teleport(destination, layer, false, false); return MudSharp.NPC.AI.CommandExecutionScope.TryContinue(target) && ReferenceEquals(target.Location, destination) && target.RoomLayer == layer; }))
			{
				BreakCloseContact(actor, target);
				return false;
			}
		ZeroGravityMovementHelper.EnsureFloating(target);
		BreakCloseContact(actor, target);
		ApplyFallIfNeeded(target, false);
		return true;
	}

	public static void ApplyFallIfNeeded(ICharacter target, bool held)
	{
		if (held)
		{
			return;
		}

		if (target.ShouldFall())
		{
			target.FallToGround();
		}
	}

	private static bool CanForceThroughExit(ICharacter actor, ICharacter mover, ICellExit exit, bool victim, out string why)
	{
		why = string.Empty;
		var flags = CanMoveFlags.IgnoreWhetherExitCanBeCrossed | CanMoveFlags.IgnoreCancellableActionBlockers;
		if (victim)
		{
			flags |= CanMoveFlags.IgnoreSafeMovement;
		}

		var canMove = mover.CanMove(exit, flags);
		if (!canMove.Result &&
		    !CanIgnoreGrappleMovementBlock(actor, mover, victim, canMove))
		{
			why = canMove.ErrorMessage;
			return false;
		}

		var zeroGravityMove = ZeroGravityMovementHelper.CanMoveInZeroGravity(mover, exit);
		if (!zeroGravityMove.Result)
		{
			why = zeroGravityMove.ErrorMessage;
			return false;
		}

		if (exit.Exit.MaximumSizeToEnter < mover.Body.CurrentContextualSize(SizeContext.CellExit))
		{
			why = $"Only something of size {exit.Exit.MaximumSizeToEnter.Describe().Colour(Telnet.Green)} or smaller can use that exit, and {mover.HowSeen(actor, true)} is size {mover.Body.CurrentContextualSize(SizeContext.CellExit).Describe().Colour(Telnet.Green)}.";
			return false;
		}

		if (exit.IsFlyExit && !mover.CanFly().Truth)
		{
			why = $"{mover.HowSeen(actor, true)} cannot move in that direction without flying.";
			return false;
		}

		if (exit.IsClimbExit && !mover.Race.CanClimb)
		{
			why = $"{mover.HowSeen(actor, true)} cannot climb through that exit.";
			return false;
		}

		var canCross = mover.CanCross(exit);
		if (!canCross.Success)
		{
			why = $"{mover.HowSeen(actor, true)} cannot cross that exit.";
			return false;
		}

		return true;
	}

	private static bool CanIgnoreGrappleMovementBlock(
		ICharacter actor,
		ICharacter mover,
		bool victim,
		CanMoveResponse canMove)
	{
		if (string.IsNullOrWhiteSpace(canMove.ErrorMessage) ||
		    !canMove.ErrorMessage.Contains("grappl", StringComparison.InvariantCultureIgnoreCase))
		{
			return false;
		}

		return (victim && HasAnyGrapple(actor, mover)) ||
		       (mover == actor && actor.EffectsOfType<IGrappling>().Any());
	}

	private static void PreserveCloseContact(ICharacter actor, ICharacter target, bool wasMelee, bool wasClinch, bool wasGrapple)
	{
		if (wasMelee || wasClinch || wasGrapple)
		{
			actor.MeleeRange = true;
			target.MeleeRange = true;
		}

		if (wasClinch)
		{
			if (actor.EffectsOfType<ClinchEffect>().All(x => x.Target != target))
			{
				actor.AddEffect(new ClinchEffect(actor, target));
			}

			if (target.EffectsOfType<ClinchEffect>().All(x => x.Target != actor))
			{
				target.AddEffect(new ClinchEffect(target, actor));
			}
		}
	}
}
