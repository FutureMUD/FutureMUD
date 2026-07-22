using Humanizer;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Movement;
using MudSharp.NPC.AI.Strategies;

#nullable enable annotations

namespace MudSharp.Effects.Concrete;

public class FollowingPath : Effect, IEffectSubtype, IRemoveOnCombatStart
{
	private const double CoordinateToleranceMetres = 0.0005;
    private readonly HashSet<IExit> _unlockedExits = new();
	private readonly Queue<ISpatialPathStep>? _spatialSteps;
	private readonly Func<ICellExit, bool>? _spatialExitSuitability;
	private readonly Dictionary<ICell, RouteTopologyPin> _routeTopologyPins =
		new(ReferenceEqualityComparer.Instance);

    public Queue<ICellExit> Exits { get; set; }
	public bool IsSpatialPath => _spatialSteps is not null;
	public IReadOnlyList<ISpatialPathStep> SpatialSteps => _spatialSteps?.ToArray() ?? [];

    public FollowingPath(ICharacter owner, IEnumerable<ICellExit> exits) : base(owner, null)
    {
        Exits = new Queue<ICellExit>(exits);
    }

	public FollowingPath(
		ICharacter owner,
		ISpatialPath path,
		Func<ICellExit, bool>? exitSuitability = null) : base(owner, null)
	{
		ArgumentNullException.ThrowIfNull(path);
		_spatialSteps = new Queue<ISpatialPathStep>(path.Steps);
		_spatialExitSuitability = exitSuitability ??
			(exit => owner.CanCross(exit).Success && owner.CanMove(exit));
		Exits = new Queue<ICellExit>(path.Steps
			.OfType<IExitTraversalPathStep>()
			.Select(x => x.Exit));

		foreach (var cell in path.Steps
			.SelectMany(x => new[] { x.Origin.Cell, x.Destination.Cell })
			.Distinct<ICell>(ReferenceEqualityComparer.Instance))
		{
			if (cell.RouteDefinition is { } route)
			{
				_routeTopologyPins[cell] = new RouteTopologyPin(route, route.TopologyVersion);
			}
		}
	}

    protected override string SpecificEffectType => "FollowingPath";

    protected virtual bool RemoveWhenExitsEmpty => true;

    public override string Describe(IPerceiver voyeur)
    {
		if (_spatialSteps is not null)
		{
			var steps = _spatialSteps.Select(x => x switch
			{
				ILinearRoutePathStep linear =>
					$"{linear.Direction.DescribeEnum()} {linear.DistanceMetres.ToString("N0", voyeur)}m",
				IExitTraversalPathStep traversal when traversal.Exit.OutboundDirection != CardinalDirection.Unknown =>
					traversal.Exit.OutboundDirection.DescribeBrief(),
				IExitTraversalPathStep traversal when traversal.Exit is NonCardinalCellExit nc =>
					$"'{nc.Verb} {nc.PrimaryKeyword}'".ToLowerInvariant(),
				_ => "??"
			}).Humanize();
			return $"Following a spatial path: {steps}";
		}

        string exitStrings = Exits.Select(x =>
        {
            if (x.OutboundDirection != CardinalDirection.Unknown)
            {
                return x.OutboundDirection.DescribeBrief();
            }

            return x is NonCardinalCellExit nc ? $"'{nc.Verb} {nc.PrimaryKeyword}'".ToLowerInvariant() : "??";
        }).Humanize();
        return $"Following a path: {exitStrings}";
    }

    public bool OpenDoors { get; set; }
    public bool UseKeys { get; set; }
    public bool SmashLockedDoors { get; set; }
    public bool UseDoorguards { get; set; }
	public bool CloseDoorsBehind { get; set; }

	public static FollowingPath CreateFullFriendlyPath(ICharacter owner, IEnumerable<ICellExit> exits,
		bool closeDoorsBehind = false)
	{
		return new FollowingPath(owner, exits)
		{
			UseDoorguards = true,
			UseKeys = true,
			OpenDoors = true,
			CloseDoorsBehind = closeDoorsBehind
		};
	}

	public static FollowingPath CreateFullFriendlyPath(
		ICharacter owner,
		ISpatialPath path,
		Func<ICellExit, bool> exitSuitability,
		bool closeDoorsBehind = false)
	{
		return new FollowingPath(owner, path, exitSuitability)
		{
			UseDoorguards = true,
			UseKeys = true,
			OpenDoors = true,
			CloseDoorsBehind = closeDoorsBehind
		};
	}

    public virtual void FollowPathAction()
    {
        ICharacter ch = (ICharacter)Owner;

        if (ch.State.HasFlag(CharacterState.Dead) || ch.Corpse != null)
        {
            ch.RemoveEffect(this);
            return;
        }

		if (_spatialSteps is not null)
		{
			FollowSpatialPathAction(ch);
			return;
		}

        if (Exits.Count == 0)
        {
            ch.RemoveEffect(this);
            return;
        }

        ICellExit exit = Exits.Peek();
        if (ZeroGravityMovementHelper.IsZeroGravity(ch.Location, ch.RoomLayer, ch) &&
            !ZeroGravityMovementHelper.CanManeuver(ch))
        {
            ch.RemoveEffect(this);
            return;
        }

        if (exit.Origin != ch.Location)
        {
	        if (exit.Destination == ch.Location)
	        {
		        Exits.Dequeue();
		        if (CloseDoorsBehind)
		        {
			        CloseDoorBehind(ch, exit, UseKeys, ConsumeUnlockedExit(exit));
		        }

		        if (Exits.Count == 0 && RemoveWhenExitsEmpty)
		        {
			        ch.RemoveEffect(this);
		        }

		        return;
	        }

	        ch.RemoveEffect(this);
	        return;
        }

        if (!exit.WhichLayersExitAppears().Contains(ch.RoomLayer))
        {
            RoomLayer targetLayer = exit.WhichLayersExitAppears()
                                  .OrderBy(x => Math.Abs(ch.RoomLayer.LayerHeight() - x.LayerHeight()))
                                  .FirstOrDefault();
            if (!PathTowardsLayer(targetLayer))
            {
                ch.RemoveEffect(this);
                return;
            }
        }

        if (!ch.PositionState.Upright &&
            !ch.PositionState.MoveRestrictions.In(MovementAbility.Flying, MovementAbility.Swimming, MovementAbility.ZeroGravity))
        {
            IPositionState mobile = ch.MostUprightMobilePosition();
            if (mobile == null || !ch.CanMovePosition(mobile))
            {
                ch.RemoveEffect(this);
                return;
            }

            ch.MovePosition(mobile, null, null);
        }

        IMovementStrategy strategy = MovementStrategyFactory.GetStrategy(OpenDoors, UseKeys, SmashLockedDoors, UseDoorguards);
		switch (strategy.TryToMove(ch, exit))
		{
			case MovementStrategyResult.Moved:
				Exits.Dequeue();
				CloseDoorBehindAfterMovement(ch, exit);
				if (Exits.Count == 0 && RemoveWhenExitsEmpty)
				{
					ch.RemoveEffect(this);
				}

				return;
			case MovementStrategyResult.Waiting:
				return;
			default:
				ch.RemoveEffect(this);
				return;
		}
    }

	private void FollowSpatialPathAction(ICharacter ch)
	{
		if (_spatialSteps is null || _spatialSteps.Count == 0)
		{
			ch.RemoveEffect(this);
			return;
		}

		if (ch.Movement is not null)
		{
			return;
		}

		if (!RevalidateTopologyPins())
		{
			ch.RemoveEffect(this);
			return;
		}

		var step = _spatialSteps.Peek();
		var current = RouteSpatialService.Instance.GetEffectiveLocation(ch);
		if (LocationsMatch(current, step.Destination))
		{
			ConsumeSpatialStep(step);
			if (_spatialSteps.Count == 0)
			{
				ch.RemoveEffect(this);
			}

			return;
		}

		if (!LocationsMatch(current, step.Origin))
		{
			ch.RemoveEffect(this);
			return;
		}

		switch (step)
		{
			case ILinearRoutePathStep linear:
				if (!RevalidateLinearStep(linear) || !TryBeginLinearRouteMovement(ch, linear))
				{
					ch.RemoveEffect(this);
				}

				return;
			case IExitTraversalPathStep traversal:
				FollowSpatialExitStep(ch, traversal);
				return;
			default:
				ch.RemoveEffect(this);
				return;
		}
	}

	private void FollowSpatialExitStep(ICharacter ch, IExitTraversalPathStep traversal)
	{
		var exit = traversal.Exit;
		if (!RevalidateExitStep(ch, traversal))
		{
			ch.RemoveEffect(this);
			return;
		}

		if (ZeroGravityMovementHelper.IsZeroGravity(ch.Location, ch.RoomLayer, ch) &&
			!ZeroGravityMovementHelper.CanManeuver(ch))
		{
			ch.RemoveEffect(this);
			return;
		}

		if (!ch.PositionState.Upright &&
			!ch.PositionState.MoveRestrictions.In(
				MovementAbility.Flying,
				MovementAbility.Swimming,
				MovementAbility.ZeroGravity))
		{
			var mobile = ch.MostUprightMobilePosition();
			if (mobile is null || !ch.CanMovePosition(mobile))
			{
				ch.RemoveEffect(this);
				return;
			}

			ch.MovePosition(mobile, null, null);
		}

		if (_spatialExitSuitability?.Invoke(exit) != true)
		{
			ch.RemoveEffect(this);
			return;
		}

		switch (TryMoveThroughExit(ch, exit))
		{
			case MovementStrategyResult.Moved:
				ConsumeSpatialStep(traversal);
				CloseDoorBehindAfterMovement(ch, exit);
				if (_spatialSteps!.Count == 0)
				{
					ch.RemoveEffect(this);
				}

				return;
			case MovementStrategyResult.Waiting:
				return;
			default:
				ch.RemoveEffect(this);
				return;
		}
	}

	protected virtual MovementStrategyResult TryMoveThroughExit(ICharacter ch, ICellExit exit)
	{
		var strategy = MovementStrategyFactory.GetStrategy(OpenDoors, UseKeys, SmashLockedDoors, UseDoorguards);
		return strategy.TryToMove(ch, exit);
	}

	protected virtual bool TryBeginLinearRouteMovement(ICharacter ch, ILinearRoutePathStep step)
	{
		if (!LinearRouteMovement.TryCreate(
				ch,
				step.Destination.RoutePositionMetres!.Value,
				out var movement,
				out _,
				targetMinimumMetres: step.Destination.RoutePositionMetres.Value,
				targetMaximumMetres: step.Destination.RoutePositionMetres.Value) ||
			movement is null)
		{
			return false;
		}

		movement.InitialAction();
		return ReferenceEquals(ch.Movement, movement);
	}

	private bool RevalidateTopologyPins()
	{
		return _routeTopologyPins.All(x =>
			ReferenceEquals(x.Key.RouteDefinition, x.Value.Definition) &&
			x.Value.Definition.TopologyVersion == x.Value.TopologyVersion);
	}

	private static bool RevalidateLinearStep(ILinearRoutePathStep step)
	{
		var route = step.Origin.Cell.RouteDefinition;
		if (route is null ||
			!ReferenceEquals(route, step.RouteCell) ||
			!double.IsFinite(route.LengthMetres) || route.LengthMetres <= 0.0 ||
			!double.IsFinite(route.MetresPerRoomEquivalent) || route.MetresPerRoomEquivalent <= 0.0 ||
			!ReferenceEquals(step.Origin.Cell, step.Destination.Cell) ||
			step.Origin.Layer != step.Destination.Layer ||
			!step.Origin.RoutePositionMetres.HasValue ||
			!step.Destination.RoutePositionMetres.HasValue)
		{
			return false;
		}

		var origin = step.Origin.RoutePositionMetres.Value;
		var destination = step.Destination.RoutePositionMetres.Value;
		if (!double.IsFinite(origin) || !double.IsFinite(destination) ||
			!double.IsFinite(step.DistanceMetres) || step.DistanceMetres <= 0.0 ||
			origin < 0.0 || destination < 0.0 ||
			origin > route.LengthMetres || destination > route.LengthMetres ||
			Math.Abs(Math.Abs(destination - origin) - step.DistanceMetres) >= CoordinateToleranceMetres)
		{
			return false;
		}

		return destination > origin
			? step.Direction == RouteCellDirection.Positive
			: destination < origin && step.Direction == RouteCellDirection.Negative;
	}

	private bool RevalidateExitStep(ICharacter ch, IExitTraversalPathStep step)
	{
		var exit = step.Exit;
		if (exit is null ||
			!ReferenceEquals(exit.Origin, step.Origin.Cell) ||
			!ReferenceEquals(exit.Destination, step.Destination.Cell) ||
			!step.Origin.Cell.ExitsFor(ch, true).Any(x => ExitSidesMatch(x, exit)) ||
			!exit.WhichLayersExitAppears().Contains(step.Origin.Layer))
		{
			return false;
		}

		var transition = exit.MovementTransition(ch);
		if (transition.TransitionType == CellMovementTransition.NoViableTransition ||
			transition.TargetLayer != step.Destination.Layer)
		{
			return false;
		}

		if (step.Origin.Cell.RouteDefinition is { } sourceRoute)
		{
			var anchor = sourceRoute.ExitAnchors.FirstOrDefault(x => ExitSidesMatch(x.Exit, exit));
			if (anchor is null || !step.Origin.RoutePositionMetres.HasValue ||
				!anchor.Contains(step.Origin.RoutePositionMetres.Value))
			{
				return false;
			}
		}
		else if (step.Origin.RoutePositionMetres.HasValue)
		{
			return false;
		}

		if (step.Destination.Cell.RouteDefinition is { } destinationRoute)
		{
			var anchor = destinationRoute.ExitAnchors.FirstOrDefault(x =>
				ReferenceEquals(x.Exit, exit.Opposite) || ExitSidesShareUnderlyingExit(x.Exit, exit));
			return anchor is not null &&
			       step.Destination.RoutePositionMetres.HasValue &&
			       Math.Abs(anchor.ArrivalPositionMetres - step.Destination.RoutePositionMetres.Value) <
			       CoordinateToleranceMetres;
		}

		return !step.Destination.RoutePositionMetres.HasValue;
	}

	private void ConsumeSpatialStep(ISpatialPathStep step)
	{
		_spatialSteps!.Dequeue();
		if (step is not IExitTraversalPathStep traversal || Exits.Count == 0)
		{
			return;
		}

		if (ExitSidesMatch(Exits.Peek(), traversal.Exit))
		{
			Exits.Dequeue();
		}
	}

	private static bool LocationsMatch(SpatialLocation first, SpatialLocation second)
	{
		if (!ReferenceEquals(first.Cell, second.Cell) || first.Layer != second.Layer)
		{
			return false;
		}

		if (!first.RoutePositionMetres.HasValue || !second.RoutePositionMetres.HasValue)
		{
			return first.RoutePositionMetres.HasValue == second.RoutePositionMetres.HasValue;
		}

		return Math.Abs(first.RoutePositionMetres.Value - second.RoutePositionMetres.Value) <
		       CoordinateToleranceMetres;
	}

	private static bool ExitSidesMatch(ICellExit first, ICellExit second)
	{
		return ReferenceEquals(first, second) ||
		       ReferenceEquals(first.Exit, second.Exit) && ReferenceEquals(first.Origin, second.Origin);
	}

	private static bool ExitSidesShareUnderlyingExit(ICellExit first, ICellExit second)
	{
		return ReferenceEquals(first.Exit, second.Exit) ||
		       ReferenceEquals(first, second.Opposite) ||
		       ReferenceEquals(first.Opposite, second);
	}

	private sealed record RouteTopologyPin(IRouteCellDefinition Definition, long TopologyVersion);

	private void CloseDoorBehindAfterMovement(ICharacter ch, ICellExit exit)
	{
		if (!CloseDoorsBehind)
		{
			return;
		}

		var mustSecure = ConsumeUnlockedExit(exit);
		if (ch.Location == exit.Destination)
		{
			CloseDoorBehind(ch, exit, UseKeys, mustSecure);
			return;
		}

		var movement = ch.Movement;
		var delay = movement is null
			? TimeSpan.FromSeconds(1)
			: movement.Duration + TimeSpan.FromTicks(movement.Duration.Ticks / 5) + TimeSpan.FromMilliseconds(100);
		ch.AddEffect(new DelayedAction(ch, _ =>
		{
			if (ch.Location == exit.Destination)
			{
				CloseDoorBehind(ch, exit, UseKeys, mustSecure);
			}
		}, "closing a door behind them"), delay);
	}

	public void RecordUnlockedExit(ICellExit exit)
	{
		if (exit?.Exit is not null)
		{
			_unlockedExits.Add(exit.Exit);
		}
	}

	private bool ConsumeUnlockedExit(ICellExit exit)
	{
		return exit?.Exit is not null && _unlockedExits.Remove(exit.Exit);
	}

	internal static bool HasOtherWalkers(ICharacter ch, ICellExit exit)
	{
		if (ch is null || exit?.Exit is null)
		{
			return false;
		}

		return exit.Origin.Characters
		           .Concat(exit.Destination.Characters)
		           .Where(x => !ch.SamePhysicalInstance(x))
		           .Select(x => x.Movement)
		           .Where(x => x is not null && !x.Cancelled)
		           .Any(x => ReferenceEquals(x.Exit.Exit, exit.Exit));
	}

	public static void CloseDoorBehind(ICharacter ch, ICellExit exit, bool useKeys, bool mustSecure = false)
	{
		var door = exit?.Exit.Door;
		if (door is null)
		{
			return;
		}

		if (door.IsOpen)
		{
			if (!mustSecure && HasOtherWalkers(ch, exit))
			{
				return;
			}

			ch.Body.Close(door, null, null);
		}
		else if (!mustSecure)
		{
			return;
		}

		if (!useKeys)
		{
			return;
		}

		var keys = ch.Body.ExternalItems
		             .SelectMany(x => x.ShallowAccessibleItems(ch))
		             .SelectNotNull(x => x.GetItemType<IKey>())
		             .ToList();
		var usableKeys = keys
		                 .Where(x => door.Locks.Any(y => !y.IsLocked && y.CanLock(ch, x)))
		                 .Select(x => Tuple.Create(x, GetHoldPlanForItem(ch, x.Parent)))
		                 .Where(x => x.Item2.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		                 .ToList();
		if (!usableKeys.Any())
		{
			return;
		}

		LockDoor effect = new(ch, door, usableKeys);
		ch.AddEffect(effect);
		ch.RemoveEffect(effect, true);
	}

	private static IInventoryPlan GetHoldPlanForItem(ICharacter ch, IGameItem item)
	{
		InventoryPlanTemplate template = new(ch.Gameworld, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[]
			{
				new InventoryPlanActionHold(ch.Gameworld, 0, 0, x => x == item, null)
			})
		});
		return template.CreatePlan(ch);
	}

    public bool PathTowardsLayer(RoomLayer targetLayer)
    {
        ICharacter ch = (ICharacter)Owner;
        if (ZeroGravityMovementHelper.IsZeroGravity(ch.Location, ch.RoomLayer, ch) &&
            !ZeroGravityMovementHelper.HasIndependentPropulsion(ch))
        {
            return false;
        }

        if (ch.RoomLayer.IsLowerThan(targetLayer))
        {
            if (ch.PositionState != PositionSwimming.Instance && ch.Location.IsUnderwaterLayer(ch.RoomLayer))
            {
                ch.MovePosition(PositionSwimming.Instance, PositionModifier.None, null, null, null);
                return true;
            }

            if (ch.Location.IsUnderwaterLayer(ch.RoomLayer))
            {
                ((ISwim)ch).Ascend();
                return true;
            }

            if (ch.PositionState != PositionFlying.Instance && ch.CanFly().Truth)
            {
                ch.Fly();
                return true;
            }

            if (ch.PositionState == PositionFlying.Instance)
            {
                ((IFly)ch).Ascend();
                return true;
            }

            if (ch.CanClimbUp().Truth)
            {
                ch.ClimbUp();
                return true;
            }

            return false;
        }

        if (ch.PositionState != PositionSwimming.Instance && ch.Location.IsSwimmingLayer(ch.RoomLayer))
        {
            ch.MovePosition(PositionSwimming.Instance, PositionModifier.None, null, null, null);
            return true;
        }

        if (ch.Location.IsSwimmingLayer(ch.RoomLayer))
        {
            ((ISwim)ch).Dive();
            return true;
        }

        if (ch.PositionState == PositionFlying.Instance)
        {
            if (ch.RoomLayer == RoomLayer.GroundLevel)
            {
                ch.Land();
                return true;
            }

            ((IFly)ch).Dive();
            return true;
        }

        if (ch.CanClimbDown().Truth)
        {
            ch.ClimbDown();
            return true;
        }

        return false;
    }
}
