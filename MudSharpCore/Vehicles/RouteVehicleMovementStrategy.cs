#nullable enable

using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.Movement;

namespace MudSharp.Vehicles;

/// <summary>
/// Executes continuous longitudinal RouteCell movement for vehicles and compiled vehicle-route
/// legs. The root vehicle, recursive tow train, physical hitch gear, and external motive cohort
/// share one v1 reference coordinate.
/// </summary>
public sealed class RouteVehicleMovementStrategy : IVehicleRouteLegExecutor
{
	private static readonly object ActiveLock = new();
	private static readonly HashSet<RouteVehicleMovement> ActiveMovements = [];
	private readonly Dictionary<long, RouteVehicleMovement> _vehicleMovements = [];
	private readonly Dictionary<long, LegExecution> _journeyExecutions = [];
	private readonly RouteSpatialService _spatialService;
	private readonly IVehicleOperationalReadinessService _readinessService;
	private readonly IVehicleHitchGraphService _graphService;
	private readonly IVehicleRouteMotionPersistence _persistence;
	private readonly CellExitVehicleMovementStrategy _cellExitStrategy;
	private readonly Action<IFuturemud, Action, TimeSpan> _schedule;

	public RouteVehicleMovementStrategy(
		RouteSpatialService? spatialService = null,
		IVehicleOperationalReadinessService? readinessService = null,
		IVehicleHitchGraphService? graphService = null,
		IVehicleRouteMotionPersistence? persistence = null,
		CellExitVehicleMovementStrategy? cellExitStrategy = null,
		Action<IFuturemud, Action, TimeSpan>? schedule = null)
	{
		_spatialService = spatialService ?? RouteSpatialService.Instance;
		_graphService = graphService ?? new VehicleHitchGraphService();
		_readinessService = readinessService ?? new VehicleOperationalReadinessService(_graphService);
		_persistence = persistence ?? new DatabaseVehicleRouteMotionPersistence();
		_cellExitStrategy = cellExitStrategy ?? new CellExitVehicleMovementStrategy(
			new VehicleTowService(), _graphService, _readinessService);
		_schedule = schedule ?? ((gameworld, action, delay) => gameworld.Scheduler.AddSchedule(
			new Schedule(action, ScheduleType.Movement, delay,
				"Longitudinal RouteCell vehicle movement checkpoint")));
	}

	public static RouteVehicleMovementStrategy Instance { get; } = new();

	public bool IsMoving(IVehicle vehicle)
	{
		return vehicle is not null && _vehicleMovements.ContainsKey(vehicle.Id);
	}

	public double? EffectivePositionMetres(IVehicle vehicle)
	{
		if (vehicle?.ExteriorItem is null)
		{
			return vehicle?.RoutePositionMetres;
		}

		return _spatialService.GetEffectiveLocation(vehicle.ExteriorItem).RoutePositionMetres;
	}

	public bool TryBeginManualMove(
		IVehicle vehicle,
		ICharacter actor,
		double targetPositionMetres,
		out string reason,
		double? targetMinimumMetres = null,
		double? targetMaximumMetres = null,
		long? selectedExitId = null,
		TimeSpan? checkpointInterval = null)
	{
		return TryBeginMove(
			vehicle,
			actor,
			targetPositionMetres,
			false,
			targetMinimumMetres ?? targetPositionMetres,
			targetMaximumMetres ?? targetPositionMetres,
			selectedExitId,
			null,
			null,
			null,
			checkpointInterval,
			null,
			out reason);
	}

	public bool TryStop(IVehicle vehicle, ICharacter? actor, out string reason)
	{
		if (vehicle is null || !_vehicleMovements.TryGetValue(vehicle.Id, out var movement))
		{
			reason = "That vehicle is not travelling along a RouteCell.";
			return false;
		}

		if (actor is not null && vehicle.Controller?.SamePhysicalInstance(actor) != true &&
			!actor.IsAdministrator())
		{
			reason = "You must control that vehicle to stop it.";
			return false;
		}

		movement.Cancel("The vehicle stops at its current route position.");
		reason = string.Empty;
		return true;
	}

	public bool TryBeginLeg(
		IVehicleJourney journey,
		IVehicleRouteLeg leg,
		Action<VehicleJourneyLegResult> completion,
		out string reason)
	{
		ArgumentNullException.ThrowIfNull(completion);
		if (journey is null || leg is null)
		{
			reason = "There is no route journey or leg to execute.";
			return false;
		}

		if (journey.Vehicle is null)
		{
			reason = "The journey has no assigned vehicle.";
			return false;
		}

		if (_journeyExecutions.ContainsKey(journey.Id) || _vehicleMovements.ContainsKey(journey.Vehicle.Id))
		{
			reason = "That journey's vehicle is already moving.";
			return false;
		}

		var execution = new LegExecution(this, journey, leg, completion);
		_journeyExecutions[journey.Id] = execution;
		if (!execution.TryStart(out reason))
		{
			_journeyExecutions.Remove(journey.Id);
			return false;
		}

		return true;
	}

	public void Cancel(IVehicleJourney journey)
	{
		if (journey is null || !_journeyExecutions.TryGetValue(journey.Id, out var execution))
		{
			return;
		}

		execution.Cancel();
	}

	public bool TryBeginManualRoute(IVehicle vehicle, ICharacter actor, IVehicleRoute route,
		Action<VehicleJourneyLegResult> completion, out string reason)
	{
		ArgumentNullException.ThrowIfNull(completion);
		if (route is null || route.Status != RevisionStatus.Current)
		{
			reason = "Only an approved current vehicle-route revision can be driven.";
			return false;
		}

		if (_vehicleMovements.ContainsKey(vehicle.Id))
		{
			reason = "That vehicle is already moving.";
			return false;
		}

		var current = new SpatialLocation(vehicle.Location, vehicle.RoomLayer,
			EffectivePositionMetres(vehicle));
		var start = route.Stops
			.Where(x => ReferenceEquals(x.Location.Cell, current.Cell) && x.Location.Layer == current.Layer)
			.OrderBy(x => SpatialDistance(x.Location, current))
			.FirstOrDefault(x => SpatialDistance(x.Location, current) <= 0.002);
		IReadOnlyList<IVehicleRouteLeg> legs;
		var firstStepIndex = 0;
		if (start is not null)
		{
			legs = BuildManualLegChain(route, start);
		}
		else
		{
			var candidates = ManualRouteResumeCandidates(route, current);
			if (!candidates.Any())
			{
				reason = "The vehicle is not at a stop or on a pinned leg of that route.";
				return false;
			}

			if (candidates.Count > 1)
			{
				reason = "The vehicle's current position matches more than one pinned route continuation.";
				return false;
			}

			var candidate = candidates.Single();
			legs = BuildManualLegChain(route, candidate.Leg);
			firstStepIndex = candidate.StepIndex;
		}

		if (!legs.Any())
		{
			reason = "That route has no compiled leg departing from the vehicle's current stop.";
			return false;
		}

		var execution = new ManualRouteExecution(this, vehicle, actor, legs, completion, firstStepIndex);
		return execution.TryStart(out reason);
	}

	private static IReadOnlyList<IVehicleRouteLeg> BuildManualLegChain(IVehicleRoute route,
		IVehicleRouteStop originStop)
	{
		var firstLeg = route.Legs
			.OrderBy(x => x.Sequence)
			.FirstOrDefault(x => x.OriginStop.Id == originStop.Id);
		return firstLeg is null ? [] : BuildManualLegChain(route, firstLeg);
	}

	private static IReadOnlyList<IVehicleRouteLeg> BuildManualLegChain(IVehicleRoute route,
		IVehicleRouteLeg firstLeg)
	{
		var legs = new List<IVehicleRouteLeg>();
		var nextLeg = firstLeg;
		var seen = new HashSet<long>();
		while (nextLeg is not null && seen.Add(nextLeg.Id))
		{
			legs.Add(nextLeg);
			nextLeg = route.Legs
				.OrderBy(x => x.Sequence)
				.FirstOrDefault(x => x.OriginStop.Id == nextLeg.DestinationStop.Id);
		}

		return legs;
	}

	private static IReadOnlyList<ManualRouteResumeCandidate> ManualRouteResumeCandidates(
		IVehicleRoute route,
		SpatialLocation current)
	{
		const double tolerance = 0.002;
		var candidates = new List<ManualRouteResumeCandidate>();
		foreach (var leg in route.Legs.OrderBy(x => x.Sequence))
		{
			for (var index = 0; index < leg.Steps.Count; index++)
			{
				var step = leg.Steps[index];
				switch (step)
				{
					case IVehicleRouteLinearStep linear when
						ReferenceEquals(linear.RouteCell.Cell, current.Cell) &&
						linear.Origin.Layer == current.Layer &&
						current.RoutePositionMetres.HasValue:
					{
						var position = current.RoutePositionMetres.Value;
						var minimum = Math.Min(linear.Origin.RoutePositionMetres!.Value,
							linear.Destination.RoutePositionMetres!.Value);
						var maximum = Math.Max(linear.Origin.RoutePositionMetres.Value,
							linear.Destination.RoutePositionMetres.Value);
						if (position < minimum - tolerance || position > maximum + tolerance)
						{
							break;
						}

						var stepIndex = Math.Abs(position - linear.Destination.RoutePositionMetres.Value) <=
						                tolerance
							? index + 1
							: index;
						candidates.Add(new ManualRouteResumeCandidate(leg, stepIndex));
						break;
					}
					case IVehicleRouteExitStep _ when SpatialDistance(step.Origin, current) <= tolerance:
						candidates.Add(new ManualRouteResumeCandidate(leg, index));
						break;
				}
			}
		}

		return candidates
			.DistinctBy(x => (x.Leg.Id, x.StepIndex))
			.ToList();
	}

	private sealed record ManualRouteResumeCandidate(IVehicleRouteLeg Leg, int StepIndex);

	public static int MaterialiseAllForShutdown(IFuturemud gameworld)
	{
		RouteVehicleMovement[] movements;
		lock (ActiveLock)
		{
			movements = ActiveMovements
				.Where(x => ReferenceEquals(x.Vehicle.Gameworld, gameworld))
				.ToArray();
		}

		foreach (var movement in movements)
		{
			movement.Cancel("The vehicle stops for shutdown.");
		}

		return movements.Length;
	}

	private bool TryBeginMove(
		IVehicle vehicle,
		ICharacter? actor,
		double targetPositionMetres,
		bool automaticOperation,
		double targetMinimumMetres,
		double targetMaximumMetres,
		long? selectedExitId,
		long? journeyId,
		long? legId,
		int? stepSequence,
		TimeSpan? checkpointInterval,
		Action<VehicleJourneyLegResult>? completion,
		out string reason)
	{
		if (vehicle is null)
		{
			reason = "There is no such vehicle.";
			return false;
		}

		if (_vehicleMovements.ContainsKey(vehicle.Id))
		{
			reason = "That vehicle is already moving along a RouteCell.";
			return false;
		}

		var origin = vehicle.ExteriorItem is null
			? vehicle.SpatialLocation
			: _spatialService.GetEffectiveLocation(vehicle.ExteriorItem);
		var route = origin.Cell.RouteDefinition;
		if (route is null || !origin.RoutePositionMetres.HasValue)
		{
			reason = "That vehicle is not positioned in a RouteCell.";
			return false;
		}

		if (!double.IsFinite(targetPositionMetres) || targetPositionMetres < 0.0 ||
			targetPositionMetres > route.LengthMetres ||
			!double.IsFinite(targetMinimumMetres) || !double.IsFinite(targetMaximumMetres) ||
			targetMinimumMetres < 0.0 || targetMaximumMetres > route.LengthMetres ||
			targetMinimumMetres > targetMaximumMetres)
		{
			reason = $"The route destination must be between 0 and {route.LengthMetres:N3} metres.";
			return false;
		}

		if (Math.Abs(targetPositionMetres - origin.RoutePositionMetres.Value) < 0.0005)
		{
			reason = "That vehicle is already at the requested route position.";
			return false;
		}

		var profile = RouteProfile(vehicle);
		if (profile is null)
		{
			reason = "That vehicle does not have a RouteCell movement profile.";
			return false;
		}

		var speed = ResolveSpeed(vehicle, profile);
		if (!double.IsFinite(speed) || speed <= 0.0)
		{
			reason = profile.RoutePropulsionMode == RouteVehiclePropulsionMode.ExternallyPulled
				? "That vehicle has no able external motive character or mount."
				: "That vehicle's RouteCell movement speed is invalid.";
			return false;
		}

		LinearRouteMovementSegment segment;
		try
		{
			segment = new LinearRouteMovementSegment(origin,
				new SpatialLocation(origin.Cell, origin.Layer, targetPositionMetres), speed);
		}
		catch (ArgumentException ex)
		{
			reason = ex.Message;
			return false;
		}

		var interval = checkpointInterval ?? TimeSpan.FromSeconds(Math.Max(1.0,
			vehicle.Gameworld.GetStaticDouble("RouteCellMovementCheckpointSeconds")));
		if (interval <= TimeSpan.Zero)
		{
			reason = "The RouteCell vehicle checkpoint interval must be positive.";
			return false;
		}

		var firstDuration = TimeSpan.FromSeconds(Math.Min(interval.TotalSeconds, segment.Duration.TotalSeconds));
		var firstDistance = Math.Min(segment.DistanceMetres, speed * firstDuration.TotalSeconds);
		var readiness = _readinessService.BuildLongitudinalMovementReadiness(
			new VehicleLongitudinalReadinessRequest(vehicle, actor, profile, firstDistance, firstDuration,
				automaticOperation));
		if (!readiness.CanMove || readiness.MovePlan is null)
		{
			reason = readiness.Reason;
			return false;
		}

		if (!TryBuildCohort(vehicle, readiness.MovePlan, profile, origin, out var cohort, out reason))
		{
			return false;
		}

		if (!CanAffordStamina(cohort.StaminaMovers, origin, firstDistance))
		{
			reason = "The external motive character or mount is too exhausted to begin pulling that vehicle.";
			return false;
		}

		var movement = new RouteVehicleMovement(
			this,
			vehicle,
			actor,
			profile,
			segment,
			readiness.MovePlan,
			cohort,
			interval,
			targetMinimumMetres,
			targetMaximumMetres,
			selectedExitId,
			journeyId,
			legId,
			stepSequence,
			automaticOperation,
			completion);
		_vehicleMovements[vehicle.Id] = movement;
		lock (ActiveLock)
		{
			ActiveMovements.Add(movement);
		}

		movement.Start();
		reason = string.Empty;
		return true;
	}

	private void MovementFinished(RouteVehicleMovement movement)
	{
		_vehicleMovements.Remove(movement.Vehicle.Id);
		lock (ActiveLock)
		{
			ActiveMovements.Remove(movement);
		}
	}

	private bool ExecuteExitStep(IVehicle vehicle, ICharacter? actor, IVehicleRouteExitStep step,
		bool automaticOperation, out string reason)
	{
		var exit = step.Exit;
		VehicleRouteCohort? movedCohort = null;
		VehicleHitchGraphMovePlan? movedPlan = null;
		if (!ReferenceEquals(vehicle.Location, exit.Origin) || vehicle.RoomLayer != step.Origin.Layer)
		{
			reason = "The vehicle is no longer at the pinned exit step's origin.";
			return false;
		}

		if (exit.Origin.RouteDefinition is not null &&
			!exit.Origin.ExitsFor(vehicle.ExteriorItem).Any(x => x.Exit.Id == exit.Exit.Id))
		{
			reason = "The vehicle is outside that exit's authored RouteCell access band.";
			return false;
		}

		if (!automaticOperation)
		{
			if (actor is null)
			{
				reason = "A manual pinned exit step requires the vehicle's controller.";
				return false;
			}

			var profile = RouteProfile(vehicle);
			var motiveCharacter = profile?.RoutePropulsionMode == RouteVehiclePropulsionMode.ExternallyPulled
				? IncomingMotiveLink(vehicle)?.Source.Character
				: null;
			if (profile?.RoutePropulsionMode == RouteVehiclePropulsionMode.ExternallyPulled && motiveCharacter is null)
			{
				reason = "That vehicle has no external motive character or mount.";
				return false;
			}

			IReadOnlyCollection<ICharacter> externalPullers = motiveCharacter is null ? [] : [motiveCharacter];
			if (!_cellExitStrategy.TryPrepareMove(vehicle, actor, exit, true, out var towTrain,
					out var transition, out var readiness, out reason, externalPullers: externalPullers,
					movementProfile: profile))
			{
				return false;
			}
			movedPlan = readiness.MovePlan;

			if (motiveCharacter is not null)
			{
				var origin = vehicle.ExteriorItem is null
					? vehicle.SpatialLocation
					: _spatialService.GetEffectiveLocation(vehicle.ExteriorItem);
				if (!TryBuildCohort(vehicle, readiness.MovePlan!, profile!, origin, out movedCohort, out reason) ||
				    !CanCohortTraverseExit(movedCohort, readiness.MovePlan!, exit, motiveCharacter, out reason))
				{
					return false;
				}
			}

			if (!_cellExitStrategy.TryCommitPropulsion(readiness, out _, out reason))
			{
				return false;
			}

			_cellExitStrategy.EchoDeparture(vehicle, actor, exit, towTrain);
			_cellExitStrategy.BeginMove(vehicle, exit, towTrain, transition);
			VehicleRouteExitMovement? movement = null;
			if (movedCohort is not null && motiveCharacter is not null)
			{
				movement = new VehicleRouteExitMovement(motiveCharacter, movedCohort.Characters, exit,
					readiness.MovePlan!.Vehicles);
				MoveExternalCohortAcrossExit(movedCohort, readiness.MovePlan, exit, movement);
			}
			_cellExitStrategy.CompleteMove(vehicle, exit, transition, readiness, movement);
			MoveExtraCohortItemsAcrossExit(movedCohort, readiness.MovePlan, exit, transition.TargetLayer);
			_cellExitStrategy.EchoArrival(vehicle, actor, exit, towTrain, transition.TargetLayer);
		}
		else
		{
			var profile = RouteProfile(vehicle);
			if (profile is null)
			{
				reason = "That vehicle does not have a RouteCell movement profile.";
				return false;
			}

			if (!_cellExitStrategy.TryPrepareMove(vehicle, null!, exit, true, out var towTrain,
					out var transition, out var readiness, out reason, movementProfile: profile,
					automaticOperation: true))
			{
				return false;
			}
			movedPlan = readiness.MovePlan;

			if (!_cellExitStrategy.TryCommitPropulsion(readiness, out _, out reason))
			{
				return false;
			}

			_cellExitStrategy.EchoDeparture(vehicle, null!, exit, towTrain);
			_cellExitStrategy.BeginMove(vehicle, exit, towTrain, transition);
			_cellExitStrategy.CompleteMove(vehicle, exit, transition, readiness);
			_cellExitStrategy.EchoArrival(vehicle, null!, exit, towTrain, transition.TargetLayer);
		}

		if (step.Destination.Cell.RouteDefinition is not null && step.Destination.RoutePositionMetres.HasValue)
		{
			foreach (var member in movedPlan?.Vehicles ?? [vehicle])
			{
				member.MaterialiseRoutePosition(step.Destination.RoutePositionMetres.Value, true);
			}

			foreach (var locateable in movedCohort?.Locateables ?? [])
			{
				locateable.SetRoutePosition(step.Destination.RoutePositionMetres.Value);
			}
		}

		reason = string.Empty;
		return true;
	}

	private static IVehicleMovementProfilePrototype? RouteProfile(IVehicle vehicle)
	{
		return vehicle.Prototype?.MovementProfiles
			.Where(x => x.MovementType == VehicleMovementProfileType.Route)
			.OrderByDescending(x => x.IsDefault)
			.ThenBy(x => x.Id)
			.FirstOrDefault();
	}

	private double ResolveSpeed(IVehicle vehicle, IVehicleMovementProfilePrototype profile)
	{
		if (profile.RoutePropulsionMode == RouteVehiclePropulsionMode.Powered)
		{
			return profile.RouteSpeedMetresPerSecond;
		}

		var puller = IncomingMotiveLink(vehicle)?.Source.Character;
		if (puller is null)
		{
			return 0.0;
		}

		var gaitMultiplier = Math.Max(0.05, puller.CurrentSpeed?.Multiplier ?? 1.0);
		return Math.Max(0.05, 1.4 / gaitMultiplier);
	}

	private VehicleHitchGraphLink? IncomingMotiveLink(IVehicle vehicle)
	{
		return vehicle.ExteriorItem is null
			? null
			: _graphService.LinksInvolving(vehicle.Gameworld, vehicle.ExteriorItem)
				.Where(x => x.Target.Vehicle?.Id == vehicle.Id)
				.FirstOrDefault(x => x.Source.NodeType == VehicleHitchGraphNodeType.Character);
	}

	private bool TryBuildCohort(
		IVehicle root,
		VehicleHitchGraphMovePlan movePlan,
		IVehicleMovementProfilePrototype profile,
		SpatialLocation origin,
		out VehicleRouteCohort cohort,
		out string reason)
	{
		var vehicles = movePlan.Vehicles.DefaultIfEmpty(root).DistinctBy(x => x.Id).ToList();
		var items = vehicles.Select(x => x.ExteriorItem)
			.Concat(movePlan.HitchItems)
			.Where(x => x is not null)
			.Cast<IGameItem>()
			.DistinctBy(x => x.Id)
			.ToList();
		var characters = new HashSet<ICharacter>(CharacterPhysicalInstanceEqualityComparer.Instance);
		var staminaMovers = new HashSet<ICharacter>(CharacterPhysicalInstanceEqualityComparer.Instance);
		var extraLocateables = new HashSet<ILocateable>(ReferenceEqualityComparer.Instance);

		foreach (var vehicle in vehicles)
		{
			if (vehicle.Prototype.Scale == VehicleScale.RoomScale)
			{
				continue;
			}

			foreach (var occupant in vehicle.Occupants)
			{
				characters.Add(occupant);
			}
		}

		if (profile.RoutePropulsionMode == RouteVehiclePropulsionMode.ExternallyPulled)
		{
			var puller = IncomingMotiveLink(root)?.Source.Character;
			if (puller is null)
			{
				cohort = default!;
				reason = "That vehicle has no external motive character or mount.";
				return false;
			}

			if (!TryBuildCharacterCohort(puller, origin, out var motiveCharacters,
					out var motiveLocateables, out var motiveStamina, out reason))
			{
				cohort = default!;
				return false;
			}

			characters.UnionWith(motiveCharacters);
			staminaMovers.UnionWith(motiveStamina);
			extraLocateables.UnionWith(motiveLocateables);
		}

		var locateables = items.Cast<ILocateable>()
			.Concat(characters)
			.Concat(extraLocateables)
			.Distinct<ILocateable>(ReferenceEqualityComparer.Instance)
			.ToList();
		var immediate = RouteSpatialConfiguration.FromGameworld(root.Gameworld).ImmediateDistanceMetres;
		foreach (var locateable in locateables)
		{
			var location = _spatialService.GetEffectiveLocation(locateable);
			var isCloseEnough = ReferenceEquals(location.Cell, origin.Cell) && location.Layer == origin.Layer &&
			                    (origin.Cell.RouteDefinition is null ||
			                     location.RoutePositionMetres.HasValue && origin.RoutePositionMetres.HasValue &&
			                     Math.Abs(location.RoutePositionMetres.Value - origin.RoutePositionMetres.Value) <=
			                     immediate);
			if (!isCloseEnough)
			{
				cohort = default!;
				reason = $"{locateable.Name} is not close enough to move with the vehicle's hitch cohort.";
				return false;
			}
		}

		cohort = new VehicleRouteCohort(vehicles, items, characters.ToList(), locateables,
			staminaMovers.ToList());
		reason = string.Empty;
		return true;
	}

	private bool TryBuildCharacterCohort(
		ICharacter root,
		SpatialLocation origin,
		out IReadOnlyCollection<ICharacter> characters,
		out IReadOnlyCollection<ILocateable> locateables,
		out IReadOnlyCollection<ICharacter> staminaMovers,
		out string reason)
	{
		var cohort = new HashSet<ICharacter>(CharacterPhysicalInstanceEqualityComparer.Instance);
		var targets = new HashSet<ILocateable>(ReferenceEqualityComparer.Instance);
		var nonStamina = new HashSet<ICharacter>(CharacterPhysicalInstanceEqualityComparer.Instance);
		var queue = new Queue<ICharacter>();
		var immediate = RouteSpatialConfiguration.FromGameworld(root.Gameworld).ImmediateDistanceMetres;
		var nearby = _spatialService.GetPerceivablesWithin(origin, immediate).OfType<ICharacter>().ToList();

		void Add(ICharacter? character)
		{
			if (character is not null && cohort.Add(character))
			{
				queue.Enqueue(character);
			}
		}

		Add(root);
		if (root.Party?.Leader is { } leader && leader.SamePhysicalInstance(root))
		{
			foreach (var member in root.Party.ActiveCharacterMembers)
			{
				Add(member);
			}
		}

		while (queue.Count > 0)
		{
			var character = queue.Dequeue();
			foreach (var follower in nearby.Where(x => x.Following is ICharacter followed &&
				followed.SamePhysicalInstance(character)))
			{
				Add(follower);
			}

			if (character.RidingMount is not null)
			{
				Add(character.RidingMount);
				nonStamina.Add(character);
			}

			foreach (var rider in character.Riders)
			{
				Add(rider);
				nonStamina.Add(rider);
			}

			foreach (var dragging in character.EffectsOfType<Dragging>())
			{
				foreach (var dragger in dragging.CharacterDraggers)
				{
					Add(dragger);
				}
				foreach (var helper in dragging.Helpers)
				{
					Add(helper);
				}
				if (dragging.Target is ICharacter targetCharacter)
				{
					Add(targetCharacter);
					nonStamina.Add(targetCharacter);
				}
				else if (dragging.Target is ILocateable target)
				{
					targets.Add(target);
				}
			}

			foreach (var hitch in character.EffectsOfType<CharacterHitch>())
			{
				if (hitch.Target is ICharacter targetCharacter)
				{
					Add(targetCharacter);
				}
				else if (hitch.Target is ILocateable target)
				{
					targets.Add(target);
				}
			}
		}

		foreach (var character in cohort)
		{
			if (character.Movement is not null)
			{
				characters = [];
				locateables = [];
				staminaMovers = [];
				reason = $"{character.HowSeen(root, true)} is already moving.";
				return false;
			}

			if (nonStamina.Contains(character) || character.RidingMount is not null)
			{
				continue;
			}

			var canMove = character.CanMove(CanMoveFlags.None);
			if (!canMove.Result)
			{
				characters = [];
				locateables = [];
				staminaMovers = [];
				reason = $"{character.HowSeen(root, true)} cannot move: {canMove.ErrorMessage}";
				return false;
			}
		}

		characters = cohort.ToList();
		locateables = cohort.Cast<ILocateable>().Concat(targets).ToList();
		staminaMovers = cohort.Where(x => !nonStamina.Contains(x) && x.RidingMount is null).ToList();
		reason = string.Empty;
		return true;
	}

	private static double SpatialDistance(SpatialLocation lhs, SpatialLocation rhs)
	{
		if (!ReferenceEquals(lhs.Cell, rhs.Cell) || lhs.Layer != rhs.Layer)
		{
			return double.PositiveInfinity;
		}

		if (!lhs.RoutePositionMetres.HasValue && !rhs.RoutePositionMetres.HasValue)
		{
			return 0.0;
		}

		return lhs.RoutePositionMetres.HasValue && rhs.RoutePositionMetres.HasValue
			? Math.Abs(lhs.RoutePositionMetres.Value - rhs.RoutePositionMetres.Value)
			: double.PositiveInfinity;
	}

	private static bool CanCohortTraverseExit(
		VehicleRouteCohort cohort,
		VehicleHitchGraphMovePlan movePlan,
		ICellExit exit,
		ICharacter motiveCharacter,
		out string reason)
	{
		var vehicleOccupants = movePlan.Vehicles
			.SelectMany(x => x.Occupants)
			.ToHashSet(CharacterPhysicalInstanceEqualityComparer.Instance);
		foreach (var character in cohort.Characters)
		{
			if (vehicleOccupants.Contains(character) ||
			    character.RidingMount is not null && cohort.Characters.ContainsPhysicalInstance(character.RidingMount))
			{
				continue;
			}

			var transition = exit.MovementTransition(character);
			if (transition.TransitionType == CellMovementTransition.NoViableTransition)
			{
				reason = $"{character.HowSeen(motiveCharacter, true)} cannot use that exit.";
				return false;
			}

			var canMove = character.CanMove(exit, CanMoveFlags.None);
			if (!canMove.Result)
			{
				reason = $"{character.HowSeen(motiveCharacter, true)} cannot move through that exit: {canMove.ErrorMessage}";
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	private static void MoveExternalCohortAcrossExit(
		VehicleRouteCohort cohort,
		VehicleHitchGraphMovePlan movePlan,
		ICellExit exit,
		VehicleRouteExitMovement movement)
	{
		var vehicleOccupants = movePlan.Vehicles
			.SelectMany(x => x.Occupants)
			.ToHashSet(CharacterPhysicalInstanceEqualityComparer.Instance);
		var moved = new HashSet<ICharacter>(CharacterPhysicalInstanceEqualityComparer.Instance);
		foreach (var character in cohort.StaminaMovers.Where(x =>
			         !vehicleOccupants.Contains(x) && x.RidingMount is null))
		{
			character.ExecuteMove(movement);
			moved.Add(character);
			foreach (var rider in character.Riders)
			{
				moved.Add(rider);
			}
		}

		foreach (var character in cohort.Characters.Where(x =>
			         !vehicleOccupants.Contains(x) && !moved.Contains(x)))
		{
			if (character.RidingMount is not null && moved.Contains(character.RidingMount))
			{
				continue;
			}

			exit.Origin.Leave(character);
			var targetLayer = exit.MovementTransition(character).TargetLayer;
			character.RoomLayer = targetLayer;
			character.Moved(movement);
			exit.Destination.Enter(character, exit, roomLayer: targetLayer);
		}
	}

	private static void MoveExtraCohortItemsAcrossExit(
		VehicleRouteCohort? cohort,
		VehicleHitchGraphMovePlan? movePlan,
		ICellExit exit,
		RoomLayer targetLayer)
	{
		if (cohort is null || movePlan is null)
		{
			return;
		}

		var graphItems = movePlan.Vehicles
			.Select(x => x.ExteriorItem)
			.Concat(movePlan.HitchItems)
			.Where(x => x is not null)
			.Cast<IGameItem>()
			.ToHashSet(ReferenceEqualityComparer.Instance);
		foreach (var item in cohort.Locateables
			         .OfType<IGameItem>()
			         .Where(x => !graphItems.Contains(x) &&
			                     ReferenceEquals(x.Location, exit.Origin) &&
			                     x.InInventoryOf is null && x.ContainedIn is null)
			         .DistinctBy(x => x.Id))
		{
			exit.Origin.Extract(item);
			item.RoomLayer = targetLayer;
			exit.Destination.Insert(item, true);
			item.ForceMove();
		}
	}

	private static IReadOnlyCollection<VehicleRouteResourceCharge> StaminaCharges(
		IEnumerable<ICharacter> characters,
		SpatialLocation origin,
		double distanceMetres)
	{
		if (distanceMetres <= 0.0)
		{
			return [];
		}

		var roomEquivalents = distanceMetres / origin.Cell.RouteDefinition!.MetresPerRoomEquivalent;
		return characters.Select(character =>
		{
			var terrainCost = character.PositionState.IgnoreTerrainStaminaCostsForMovement
				? 1.0
				: character.Location.Terrain(character)?.StaminaCost ??
				  character.Gameworld.GetStaticDouble("DefaultTerrainStaminaCost");
			var encumbrance = 1.0 + character.EncumbrancePercentage *
				character.Gameworld.GetStaticDouble("StaminaMultiplierPerEncumbrancePercentage");
			var speedCost = character.CurrentSpeed?.StaminaMultiplier ?? 1.0;
			var amount = Math.Max(0.0, roomEquivalents * terrainCost * encumbrance * speedCost);
			var staminaBefore = character.Body.CurrentStamina;
			return new VehicleRouteResourceCharge(
				0,
				CharacterInstanceIdentityComparer.IdentityId(character),
				VehicleRouteResourceKind.Stamina,
				CharacterInstanceIdentityComparer.InstanceId(character),
				"stamina",
				amount,
				character.Gameworld,
				[character.Body],
				() =>
				{
					staminaBefore = character.Body.CurrentStamina;
					character.SpendStamina(amount);
				},
				() => character.Body.CurrentStamina = staminaBefore);
		}).ToArray();
	}

	private static bool CanAffordStamina(IEnumerable<ICharacter> characters, SpatialLocation origin,
		double distanceMetres)
	{
		return StaminaCharges(characters, origin, distanceMetres)
			.All(x => characters.First(c => CharacterInstanceIdentityComparer.IdentityId(c) == x.OwnerId)
				.CanSpendStamina(x.Amount));
	}

	private sealed record VehicleRouteCohort(
		IReadOnlyCollection<IVehicle> Vehicles,
		IReadOnlyCollection<IGameItem> Items,
		IReadOnlyCollection<ICharacter> Characters,
		IReadOnlyCollection<ILocateable> Locateables,
		IReadOnlyCollection<ICharacter> StaminaMovers);

	private sealed class VehicleRouteExitMovement : IMovement
	{
		private readonly ICharacter _motiveCharacter;
		private readonly IReadOnlyCollection<ICharacter> _characters;
		private readonly IReadOnlyCollection<IVehicle> _vehicles;

		public VehicleRouteExitMovement(ICharacter motiveCharacter,
			IReadOnlyCollection<ICharacter> characters,
			ICellExit exit,
			IReadOnlyCollection<IVehicle> vehicles)
		{
			_motiveCharacter = motiveCharacter;
			_characters = characters;
			_vehicles = vehicles;
			Exit = exit;
		}

		public bool Cancelled => false;
		public bool CanBeVoluntarilyCancelled => false;
		public ICellExit Exit { get; }
		public MovementPhase Phase => MovementPhase.NewRoom;
		public IEnumerable<ICharacter> CharacterMovers => _characters;
		public IParty Party => _motiveCharacter.Party!;
		public IEnumerable<IDragging> DragEffects => _motiveCharacter.EffectsOfType<Dragging>();
		public IEnumerable<ICharacter> Draggers => DragEffects.SelectMany(x => x.CharacterDraggers).Distinct();
		public IEnumerable<ICharacter> Helpers => DragEffects.SelectMany(x => x.Helpers).Distinct();
		public IEnumerable<ICharacter> NonDraggers => _characters.Except(Draggers);
		public IEnumerable<ICharacter> NonConsensualMovers => [];
		public IEnumerable<ICharacter> Mounts => _characters.Where(x => x.Riders.Any());
		public IEnumerable<IPerceivable> Targets => _vehicles
			.Select(x => x.ExteriorItem)
			.Where(x => x is not null)
			.Cast<IPerceivable>();
		public IReadOnlyDictionary<ICharacter, ISneakMoveEffect> SneakMoveEffects { get; } =
			new Dictionary<ICharacter, ISneakMoveEffect>();
		public TimeSpan Duration => TimeSpan.Zero;
		public double StaminaMultiplier => 1.0;

		public bool Cancel()
		{
			return false;
		}

		public bool CancelForMoverOnly(IMove mover, bool echo = false)
		{
			return false;
		}

		public void StopMovement()
		{
		}

		public bool IsMovementLeader(ICharacter character)
		{
			return _motiveCharacter.SamePhysicalInstance(character);
		}

		public bool IsConsensualMover(ICharacter character)
		{
			return _characters.ContainsPhysicalInstance(character);
		}

		public string Describe(IPerceiver voyeur)
		{
			return $"{_motiveCharacter.HowSeen(voyeur, true)} is pulling a vehicle cohort {Exit.OutboundMovementSuffix}.";
		}

		public bool SeenBy(IPerceiver voyeur, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
		{
			return voyeur.CanSee(_motiveCharacter, flags) || Targets.Any(x => voyeur.CanSee(x, flags));
		}

		public void InitialAction()
		{
		}

		public MovementType MovementTypeForMover(ICharacter mover)
		{
			return mover.CurrentSpeed?.Position switch
			{
				PositionClimbing => MovementType.Climbing,
				PositionSwimming => MovementType.Swimming,
				PositionProne => MovementType.Crawling,
				PositionProstrate => MovementType.Prostrate,
				PositionFlying => MovementType.Flying,
				PositionFloatingInZeroGravity => MovementType.Floating,
				_ => MovementType.Upright
			};
		}
	}

	private sealed class RouteVehicleMovement : IMovement
	{
		private readonly RouteVehicleMovementStrategy _owner;
		private readonly ICharacter? _actor;
		private readonly IVehicleMovementProfilePrototype _profile;
		private readonly VehicleRouteCohort _cohort;
		private readonly TimeSpan _checkpointInterval;
		private readonly double _targetMinimumMetres;
		private readonly double _targetMaximumMetres;
		private readonly long? _selectedExitId;
		private readonly long? _journeyId;
		private readonly long? _legId;
		private readonly int? _stepSequence;
		private readonly bool _automaticOperation;
		private readonly Action<VehicleJourneyLegResult>? _completion;
		private readonly long _topologyVersion;
		private readonly double _destinationMetres;
		private readonly TimeSpan _operationDuration;
		private readonly double _operationDistanceMetres;
		private readonly RouteMovementHookContext _hookContext;
		private long _checkpointSequence;
		private long _scheduleGeneration;
		private double _lastCheckpointMetres;
		private bool _started;
		private bool _finished;
		private bool _completionInvoked;

		public RouteVehicleMovement(
			RouteVehicleMovementStrategy owner,
			IVehicle vehicle,
			ICharacter? actor,
			IVehicleMovementProfilePrototype profile,
			LinearRouteMovementSegment segment,
			VehicleHitchGraphMovePlan movePlan,
			VehicleRouteCohort cohort,
			TimeSpan checkpointInterval,
			double targetMinimumMetres,
			double targetMaximumMetres,
			long? selectedExitId,
			long? journeyId,
			long? legId,
			int? stepSequence,
			bool automaticOperation,
			Action<VehicleJourneyLegResult>? completion)
		{
			_owner = owner;
			Vehicle = vehicle;
			_actor = actor;
			_profile = profile;
			Segment = segment;
			MovePlan = movePlan;
			_cohort = cohort;
			_checkpointInterval = checkpointInterval;
			_targetMinimumMetres = targetMinimumMetres;
			_targetMaximumMetres = targetMaximumMetres;
			_selectedExitId = selectedExitId;
			_journeyId = journeyId;
			_legId = legId;
			_stepSequence = stepSequence;
			_automaticOperation = automaticOperation;
			_completion = completion;
			_topologyVersion = segment.Origin.Cell.RouteDefinition!.TopologyVersion;
			_destinationMetres = segment.Destination.RoutePositionMetres!.Value;
			_operationDuration = segment.Duration;
			_operationDistanceMetres = segment.DistanceMetres;
			_lastCheckpointMetres = segment.Origin.RoutePositionMetres!.Value;
			OperationId = Guid.NewGuid();
			_hookContext = new RouteMovementHookContext(
				OperationId,
				segment.Origin.Cell,
				segment.Origin.RoutePositionMetres.Value,
				segment.Destination.RoutePositionMetres!.Value,
				segment.Direction,
				segment.SpeedMetresPerSecond);
		}

		public IVehicle Vehicle { get; }
		public LinearRouteMovementSegment Segment { get; private set; }
		public VehicleHitchGraphMovePlan MovePlan { get; private set; }
		public Guid OperationId { get; }
		public bool Cancelled { get; private set; }
		public bool CanBeVoluntarilyCancelled => !_finished;
		public ICellExit? Exit => null;
		public MovementPhase Phase => MovementPhase.OriginalRoom;
		public IEnumerable<ICharacter> CharacterMovers => _cohort.Characters.ToArray();
		public IParty Party => _cohort.StaminaMovers.FirstOrDefault()?.Party ?? _actor?.Party!;
		public IEnumerable<IDragging> DragEffects => [];
		public IEnumerable<ICharacter> Draggers => [];
		public IEnumerable<ICharacter> Helpers => [];
		public IEnumerable<ICharacter> NonDraggers => _cohort.Characters.ToArray();
		public IEnumerable<ICharacter> NonConsensualMovers => [];
		public IEnumerable<ICharacter> Mounts => _cohort.StaminaMovers.Where(x => x.Riders.Any()).ToArray();
		public IEnumerable<IPerceivable> Targets => _cohort.Items.Cast<IPerceivable>().ToArray();
		public IReadOnlyDictionary<ICharacter, ISneakMoveEffect> SneakMoveEffects { get; } =
			new Dictionary<ICharacter, ISneakMoveEffect>();
		public TimeSpan Duration => _operationDuration;
		public double StaminaMultiplier => _operationDistanceMetres /
		                                   Segment.Origin.Cell.RouteDefinition!.MetresPerRoomEquivalent;

		public void Start()
		{
			if (_started || _finished)
			{
				return;
			}

			_owner._persistence.Start(new VehicleRouteMotionStart(
				OperationId,
				Vehicle,
				Segment,
				_targetMinimumMetres,
				_targetMaximumMetres,
				_selectedExitId,
				_cohort.Vehicles,
				_cohort.Items,
				_cohort.Characters,
				_journeyId,
				_legId,
				_stepSequence));
			_started = true;

			foreach (var member in _cohort.Vehicles)
			{
				member.BeginMoveAlongRoute(_destinationMetres);
			}
			foreach (var mover in _cohort.Characters)
			{
				mover.Movement = this;
				mover.StartMove(this);
			}
			BeginLazySegment();
			RouteMovementFutureProgEvents.Begin(HookTargets(), _hookContext);
			if (_finished)
			{
				return;
			}

			CreateTracksAtCheckpoint();
			RouteMovementOutput.VehicleBegin(Vehicle, DirectionName());
			_actor?.OutputHandler.Send($"You begin driving {Vehicle.Name.ColourName()} {DirectionName()} along the route.");
			ScheduleNextCheckpoint();
		}

		public void InitialAction()
		{
			Start();
		}

		public bool Cancel()
		{
			if (_finished)
			{
				return false;
			}

			Cancelled = true;
			Cancel("The vehicle stops at its current route position.");
			return true;
		}

		public bool CancelForMoverOnly(IMove mover, bool echo = false)
		{
			return mover is ICharacter character && _cohort.Characters.Any(x => x.SamePhysicalInstance(character)) &&
			       Cancel();
		}

		public void StopMovement()
		{
			Cancel();
		}

		public bool IsMovementLeader(ICharacter character)
		{
			var leader = _cohort.StaminaMovers.FirstOrDefault() ?? _actor;
			return leader?.SamePhysicalInstance(character) == true;
		}

		public bool IsConsensualMover(ICharacter character)
		{
			return _cohort.Characters.Any(x => x.SamePhysicalInstance(character));
		}

		public string Describe(IPerceiver voyeur)
		{
			return $"{Vehicle.ExteriorItem.HowSeen(voyeur, true)} is travelling {DirectionName()}.";
		}

		public bool SeenBy(IPerceiver voyeur, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
		{
			return voyeur is ILocateable locateable &&
			       _owner._spatialService.GetProximity(locateable, Vehicle.ExteriorItem) <= Proximity.Distant &&
			       voyeur.CanSee(Vehicle.ExteriorItem, flags);
		}

		public MovementType MovementTypeForMover(ICharacter mover)
		{
			return mover.CurrentSpeed?.Position switch
			{
				PositionClimbing => MovementType.Climbing,
				PositionSwimming => MovementType.Swimming,
				PositionProne => MovementType.Crawling,
				PositionProstrate => MovementType.Prostrate,
				PositionFlying => MovementType.Flying,
				PositionFloatingInZeroGravity => MovementType.Floating,
				_ => MovementType.Upright
			};
		}

		public void Cancel(string reason)
		{
			if (_finished)
			{
				return;
			}

			_scheduleGeneration++;
			Cancelled = true;
			if (!CommitEffectivePosition(out var failure))
			{
				reason = failure;
			}
			Finish(false, reason);
		}

		private void ScheduleNextCheckpoint()
		{
			if (_finished)
			{
				return;
			}

			var remainingSeconds = Math.Abs(_destinationMetres - _lastCheckpointMetres) /
			                       Segment.SpeedMetresPerSecond;
			if (remainingSeconds <= 0.000001)
			{
				Finish(true, string.Empty);
				return;
			}

			var delay = TimeSpan.FromSeconds(Math.Min(_checkpointInterval.TotalSeconds, remainingSeconds));
			var generation = ++_scheduleGeneration;
			_owner._schedule(Vehicle.Gameworld, () => Checkpoint(generation), delay);
		}

		private void Checkpoint(long generation)
		{
			if (_finished || generation != _scheduleGeneration)
			{
				return;
			}

			if (Segment.Origin.Cell.RouteDefinition?.TopologyVersion != _topologyVersion)
			{
				CommitEffectivePosition(out _);
				if (!_finished)
				{
					Finish(false, "The vehicle stops because the RouteCell topology changed.");
				}
				return;
			}

			if (!CommitEffectivePosition(out var failureReason))
			{
				if (!_finished)
				{
					Finish(false, failureReason);
				}
				return;
			}
			if (_finished)
			{
				return;
			}

			if (Math.Abs(_destinationMetres - _lastCheckpointMetres) < 0.0005)
			{
				Finish(true, string.Empty);
				return;
			}

			var remaining = Math.Abs(_destinationMetres - _lastCheckpointMetres);
			var nextDuration = TimeSpan.FromSeconds(Math.Min(_checkpointInterval.TotalSeconds,
				remaining / Segment.SpeedMetresPerSecond));
			var nextDistance = Math.Min(remaining, Segment.SpeedMetresPerSecond * nextDuration.TotalSeconds);
			var readiness = _owner._readinessService.BuildLongitudinalMovementReadiness(
				new VehicleLongitudinalReadinessRequest(Vehicle, _actor, _profile, nextDistance,
					nextDuration, _automaticOperation, this));
			if (!readiness.CanMove || readiness.MovePlan is null)
			{
				Finish(false, readiness.Reason);
				return;
			}

			MovePlan = readiness.MovePlan;
			var catastropheActor = _actor ?? Vehicle.Controller ?? Vehicle.Occupants.FirstOrDefault();
			if (catastropheActor is not null)
			{
				var catastrophe = _owner._readinessService.RollTowCatastrophe(MovePlan, catastropheActor);
				if (catastrophe.Catastrophe)
				{
					Finish(false, catastrophe.Reason);
					return;
				}
			}

			if (!CanAffordStamina(_cohort.StaminaMovers, Segment.Origin, nextDistance))
			{
				Finish(false, "The external motive character or mount is too exhausted to continue pulling the vehicle.");
				return;
			}

			Segment = new LinearRouteMovementSegment(
				new SpatialLocation(Segment.Origin.Cell, Segment.Origin.Layer, _lastCheckpointMetres),
				new SpatialLocation(Segment.Origin.Cell, Segment.Origin.Layer, _destinationMetres),
				Segment.SpeedMetresPerSecond);
			BeginLazySegment();
			CreateTracksAtCheckpoint();
			ScheduleNextCheckpoint();
		}

		private bool CommitEffectivePosition(out string reason)
		{
			var previousPosition = _lastCheckpointMetres;
			var effective = _owner._spatialService.GetEffectiveLocation(Vehicle.ExteriorItem);
			var position = effective.RoutePositionMetres ?? _lastCheckpointMetres;
			var distance = Math.Abs(position - _lastCheckpointMetres);
			var duration = TimeSpan.FromSeconds(distance / Segment.SpeedMetresPerSecond);
			var resourcePlan = _owner._readinessService.BuildLongitudinalResourcePlan(Vehicle, _profile,
				distance, duration);
			if (!resourcePlan.IsSatisfied)
			{
				Materialise(position);
				_checkpointSequence++;
				if (!TryPersistCheckpoint(new VehicleRouteMotionCheckpoint(
					OperationId,
					_checkpointSequence,
					position,
					TimeSpan.FromSeconds(Math.Abs(_destinationMetres - position) /
					                     Segment.SpeedMetresPerSecond),
					[]), _ => { }, previousPosition, () => { }, null, out reason))
				{
					return false;
				}
				PublishProgress(previousPosition, position);
				reason = resourcePlan.Reason.IfNullOrWhiteSpace(
					"The vehicle stops because its route movement resources are no longer available.");
				return false;
			}

			var charges = BuildResourceCharges(resourcePlan)
				.Concat(StaminaCharges(_cohort.StaminaMovers, Segment.Origin, distance))
				.Where(x => x.Amount >= 0.0000005)
				.ToArray();
			var saveQueueStates = RouteCheckpointSaveQueue.Capture(charges
				.SelectMany(x => x.Saveables));
			if (charges.Where(x => x.ResourceKind == VehicleRouteResourceKind.Stamina)
				.Any(x => !_cohort.StaminaMovers
					.First(c => CharacterInstanceIdentityComparer.IdentityId(c) == x.OwnerId)
					.CanSpendStamina(x.Amount)))
			{
				Materialise(position);
				_checkpointSequence++;
				if (!TryPersistCheckpoint(new VehicleRouteMotionCheckpoint(
					OperationId,
					_checkpointSequence,
					position,
					TimeSpan.FromSeconds(Math.Abs(_destinationMetres - position) /
					                     Segment.SpeedMetresPerSecond),
					[]), _ => { }, previousPosition, () => { }, null, out reason))
				{
					return false;
				}
				PublishProgress(previousPosition, position);
				reason = "The external motive character or mount is too exhausted to pay the movement checkpoint.";
				return false;
			}

			Materialise(position);
			_checkpointSequence++;
			var remaining = TimeSpan.FromSeconds(Math.Abs(_destinationMetres - position) /
			                                     Segment.SpeedMetresPerSecond);
			var appliedCharges = new List<VehicleRouteResourceCharge>();
			if (!TryPersistCheckpoint(new VehicleRouteMotionCheckpoint(
				OperationId,
				_checkpointSequence,
				position,
				remaining,
				charges), committedCharges =>
			{
				foreach (var charge in committedCharges)
				{
					appliedCharges.Add(charge);
					charge.Apply();
				}
			}, previousPosition, () =>
			{
				var failures = new List<Exception>();
				foreach (var charge in appliedCharges.AsEnumerable().Reverse())
				{
					try
					{
						charge.Rollback();
					}
					catch (Exception exception)
					{
						failures.Add(exception);
					}
				}

				if (failures.Count > 0)
				{
					throw new AggregateException("One or more vehicle route resource rollbacks failed.", failures);
				}
			}, saveQueueStates, out reason))
			{
				return false;
			}

			PublishProgress(previousPosition, position);
			reason = string.Empty;
			return true;
		}

		private bool TryPersistCheckpoint(
			VehicleRouteMotionCheckpoint checkpoint,
			Action<IReadOnlyCollection<VehicleRouteResourceCharge>> applyCharges,
			double previousPosition,
			Action rollbackCharges,
			IReadOnlyCollection<RouteCheckpointSaveState>? saveQueueStates,
			out string reason)
		{
			try
			{
				_owner._persistence.CommitCheckpoint(checkpoint, applyCharges);
				RouteCheckpointSaveQueue.Restore(saveQueueStates ?? [], exception =>
					Vehicle.Gameworld.SystemMessage(
						$"Vehicle RouteCell movement {OperationId:N} committed checkpoint {checkpoint.Sequence:N0}, but could not restore an affected save-queue entry: {exception.Message}",
						true));
				reason = string.Empty;
				return true;
			}
			catch (Exception exception)
			{
				var rollbackFailures = new List<Exception>();
				try
				{
					rollbackCharges();
				}
				catch (Exception rollbackException)
				{
					rollbackFailures.Add(rollbackException);
				}

				try
				{
					Materialise(previousPosition);
				}
				catch (Exception rollbackException)
				{
					rollbackFailures.Add(rollbackException);
				}
				RouteCheckpointSaveQueue.PreserveRollback(saveQueueStates ?? [], rollbackFailures.Add);
				_lastCheckpointMetres = previousPosition;
				var rollbackSuffix = rollbackFailures.Count == 0
					? string.Empty
					: $" Rollback also reported {rollbackFailures.Count:N0} error(s): {rollbackFailures[0].Message}";
				Vehicle.Gameworld.SystemMessage(
					$"Vehicle RouteCell movement {OperationId:N} stopped after checkpoint {checkpoint.Sequence:N0} failed: {exception.Message}{rollbackSuffix}",
					true);
				reason = "The vehicle stops because its durable movement checkpoint could not be committed.";
				return false;
			}
		}

		private void PublishProgress(double previousPosition, double currentPosition)
		{
			_lastCheckpointMetres = currentPosition;
			RouteMovementFutureProgEvents.Progress(
				HookTargets(),
				_hookContext,
				previousPosition,
				currentPosition);
			if (Math.Abs(currentPosition - previousPosition) >= 0.0005)
			{
				RouteMovementOutput.VehicleProgress(Vehicle, DirectionName());
			}
		}

		private IReadOnlyCollection<VehicleRouteResourceCharge> BuildResourceCharges(
			VehicleResourceReadinessPlan plan)
		{
			return plan.Uses.Select(use =>
			{
				var amount = use.ResourceType == VehicleResourceUseType.Fuel
					? use.FuelVolume
					: use.PowerSpikeInWatts;
				var kind = use.ResourceType == VehicleResourceUseType.Fuel
					? VehicleRouteResourceKind.Fuel
					: VehicleRouteResourceKind.Power;
				var referenceId = use.ResourceType == VehicleResourceUseType.Fuel
					? use.FuelLiquidId
					: null;
				var key = use.ResourceType == VehicleResourceUseType.Fuel
					? $"fuel:{use.FuelLiquidId}"
					: "power";
				var singleUsePlan = new VehicleResourceReadinessPlan(
					plan.Vehicle,
					plan.Profile,
					[use],
					plan.FuelCandidates,
					plan.PowerCandidates,
					true,
					true,
					string.Empty);
				var saveables = new List<ISaveable>();
				if (use.FuelContainer is ISaveable fuelSaveable)
				{
					saveables.Add(fuelSaveable);
				}
				if (use.PowerProducer is ISaveable powerSaveable)
				{
					saveables.Add(powerSaveable);
				}

				var rollbackActions = new List<Action>();
				if (use.FuelContainer is not null)
				{
					var fuelSnapshot = use.FuelContainer.LiquidMixture?.Clone();
					rollbackActions.Add(() => use.FuelContainer.LiquidMixture = fuelSnapshot?.Clone()!);
				}
				if (use.PowerProducer is BatteryPoweredGameItemComponent batteryPower)
				{
					var batterySnapshots = batteryPower.RouteCheckpointBatteries
						.Select(x => (Battery: x, x.WattHoursRemaining))
						.ToList();
					saveables.AddRange(batterySnapshots
						.Select(x => x.Battery)
						.OfType<ISaveable>());
					rollbackActions.Add(() =>
					{
						foreach (var (battery, wattHours) in batterySnapshots)
						{
							battery.WattHoursRemaining = wattHours;
						}
					});
				}
				else if (use.PowerProducer is FuelGeneratorGameItemComponent fuelGenerator)
				{
					var spike = fuelGenerator.RouteCheckpointSpikeDrawdown;
					rollbackActions.Add(() => fuelGenerator.RouteCheckpointSpikeDrawdown = spike);
				}
				else if (use.PowerProducer is UnlimitedGeneratorGameItemComponent unlimitedGenerator)
				{
					var spike = unlimitedGenerator.RouteCheckpointSpikeDrawdown;
					rollbackActions.Add(() => unlimitedGenerator.RouteCheckpointSpikeDrawdown = spike);
				}
				else if (use.ResourceType == VehicleResourceUseType.Power && use.PowerProducer is null)
				{
					var rollbackUse = use with { PowerSpikeInWatts = -use.PowerSpikeInWatts };
					var rollbackPlan = singleUsePlan with { Uses = [rollbackUse] };
					rollbackActions.Add(() => _owner._readinessService.ConsumeMovementResources(rollbackPlan));
				}

				return new VehicleRouteResourceCharge(
					1,
					use.Item?.Id ?? use.Installation.Id,
					kind,
					referenceId,
					key,
					amount,
					Vehicle.Gameworld,
					saveables.Distinct().ToArray(),
					() => _owner._readinessService.ConsumeMovementResources(singleUsePlan),
					() =>
					{
						foreach (var rollback in rollbackActions.AsEnumerable().Reverse())
						{
							rollback();
						}
					});
			}).ToArray();
		}

		private void Materialise(double position)
		{
			// The effective coordinate was sampled once by CommitEffectivePosition. Do not ask the
			// spatial service to materialise each participant independently: a live monotonic clock can
			// advance between those calls, splitting a v1 common-reference cohort and making an exterior
			// appear to have been force-moved outside its canonical vehicle guard.
			foreach (var locateable in _cohort.Locateables)
			{
				_owner._spatialService.ClearActiveMovement(locateable);
			}

			// Characters, mounts, riders and dragged perceivables lead the physical items so hitch
			// components revalidate against participants that are already at the shared coordinate.
			foreach (var locateable in _cohort.Locateables.Where(x => x is not IGameItem))
			{
				locateable.SetRoutePosition(position);
			}

			// Canonical vehicle state leads each exterior projection. The exterior component suppresses
			// only its own forced-move callback for this write; other item components still revalidate.
			foreach (var member in _cohort.Vehicles)
			{
				member.MaterialiseRoutePosition(position);
			}

			var exteriorItems = _cohort.Vehicles
				.Select(x => x.ExteriorItem)
				.Where(x => x is not null)
				.ToHashSet(ReferenceEqualityComparer.Instance);
			foreach (var item in _cohort.Locateables
				         .OfType<IGameItem>()
				         .Where(x => !exteriorItems.Contains(x)))
			{
				item.SetRoutePosition(position);
			}
		}

		private void BeginLazySegment()
		{
			foreach (var locateable in _cohort.Locateables)
			{
				_owner._spatialService.BeginActiveMovement(locateable, Segment);
			}
		}

		private void CreateTracksAtCheckpoint()
		{
			if (!Vehicle.Gameworld.GetStaticBool("TrackingEnabled") ||
				!Segment.Origin.Cell.Terrain(Vehicle.ExteriorItem).CanHaveTracks)
			{
				return;
			}

			foreach (var member in _cohort.Vehicles)
			{
				var visual = Math.Max(
					1.0,
					(int)member.ExteriorItem.Size - (int)SizeCategory.Normal + 1.0);
				var olfactory = Math.Max(0.25, visual * 0.25);
				var track = new Track(
					member.Gameworld,
					member,
					_lastCheckpointMetres,
					Segment.Direction,
					visual,
					olfactory);
				member.Gameworld.Add(track);
				Segment.Origin.Cell.AddTrack(track);
			}

			foreach (var mover in _cohort.StaminaMovers.Where(x => !x.IsAdministrator()))
			{
				var circumstances = TrackCircumstances.None;
				if (MudSharp.Movement.Movement.GetTrackIntensities(
						mover,
						Segment.Origin.Cell,
						ref circumstances,
						out var visual,
						out var olfactory))
				{
					continue;
				}

				var track = new Track(
					mover.Gameworld,
					mover,
					_lastCheckpointMetres,
					Segment.Direction,
					circumstances,
					visual,
					olfactory);
				mover.Gameworld.Add(track);
				Segment.Origin.Cell.AddTrack(track);
			}
		}

		private void Finish(bool succeeded, string reason)
		{
			if (_finished)
			{
				return;
			}

			_finished = true;
			_scheduleGeneration++;
			foreach (var member in _cohort.Vehicles)
			{
				member.MaterialiseRoutePosition(_lastCheckpointMetres, true);
			}
			foreach (var mover in _cohort.Characters)
			{
				if (succeeded)
				{
					mover.Moved(this);
				}
				mover.StopMovement(this);
				if (ReferenceEquals(mover.Movement, this))
				{
					mover.Movement = null!;
				}
			}
			try
			{
				_owner._persistence.Complete(OperationId);
			}
			catch (Exception exception)
			{
				Vehicle.Gameworld.SystemMessage(
					$"Vehicle RouteCell movement {OperationId:N} could not clear its durable motion row: {exception.Message}",
					true);
			}
			_owner.MovementFinished(this);
			if (succeeded)
			{
				RouteMovementFutureProgEvents.Complete(HookTargets(), _hookContext);
			}
			else
			{
				RouteMovementFutureProgEvents.Cancelled(
					HookTargets(),
					_hookContext,
					_lastCheckpointMetres,
					reason);
			}
			RouteMovementOutput.VehicleFinished(Vehicle, succeeded);

			if (succeeded)
			{
				_actor?.OutputHandler.Send($"{Vehicle.Name.ColourName()} arrives at the requested route position.");
				InvokeCompletion(VehicleJourneyLegResult.Success);
				return;
			}

			if (_completion is null)
			{
				_actor?.OutputHandler.Send(reason);
			}
			InvokeCompletion(new VehicleJourneyLegResult(false, reason));
		}

		private IEnumerable<IPerceivable> HookTargets()
		{
			return _cohort.Characters
				.Cast<IPerceivable>()
				.Concat(_cohort.Vehicles.Select(x => (IPerceivable)x.ExteriorItem));
		}

		private void InvokeCompletion(VehicleJourneyLegResult result)
		{
			if (_completionInvoked)
			{
				return;
			}

			_completionInvoked = true;
			_completion?.Invoke(result);
		}

		private string DirectionName()
		{
			var route = Segment.Origin.Cell.RouteDefinition!;
			return Segment.Direction == RouteCellDirection.Positive
				? route.PositiveDirectionName
				: route.NegativeDirectionName;
		}
	}

	private sealed class LegExecution
	{
		private readonly RouteVehicleMovementStrategy _owner;
		private readonly IVehicleJourney _journey;
		private readonly IVehicleRouteLeg _leg;
		private readonly Action<VehicleJourneyLegResult> _completion;
		private int _index;
		private bool _finished;

		public LegExecution(RouteVehicleMovementStrategy owner, IVehicleJourney journey,
			IVehicleRouteLeg leg, Action<VehicleJourneyLegResult> completion)
		{
			_owner = owner;
			_journey = journey;
			_leg = leg;
			_completion = completion;
		}

		public bool TryStart(out string reason)
		{
			reason = string.Empty;
			foreach (var pin in _journey.Route.TopologyPins)
			{
				if (pin.RouteCell.RouteDefinition?.TopologyVersion != pin.TopologyVersion)
				{
					reason = $"RouteCell #{pin.RouteCell.Id:N0} no longer matches the route's pinned topology version.";
					return false;
				}
			}

			if (_leg.Steps.Count == 0)
			{
				reason = "That route leg has no compiled movement steps.";
				return false;
			}

			var current = _journey.Vehicle.ExteriorItem is null
				? _journey.Vehicle.SpatialLocation
				: _owner._spatialService.GetEffectiveLocation(_journey.Vehicle.ExteriorItem);
			if (VehicleJourneyStateRules.IsPhysicallyInFlight(_journey.State))
			{
				if (!VehicleRouteRecoveryRules.TryResolveStepIndex(_leg.Steps, current, out _index, out reason))
				{
					return false;
				}
			}
			else
			{
				_index = 0;
				if (!VehicleRouteRecoveryRules.IsAtLocation(current, _leg.Steps[0].Origin))
				{
					reason = "The vehicle is not at the pinned origin of its departing route leg.";
					return false;
				}
			}
			if (_index == _leg.Steps.Count)
			{
				Complete(VehicleJourneyLegResult.Success);
				reason = string.Empty;
				return true;
			}

			return TryStartCurrent(out reason);
		}

		public void Cancel()
		{
			if (_finished)
			{
				return;
			}

			if (_owner._vehicleMovements.TryGetValue(_journey.Vehicle.Id, out var movement))
			{
				movement.Cancel("The route journey is cancelled.");
				return;
			}

			Complete(new VehicleJourneyLegResult(false, "The route journey is cancelled."));
		}

		private bool TryStartCurrent(out string reason)
		{
			reason = string.Empty;
			var step = _leg.Steps[_index];
			var automatic = _journey.Service.OperatorMode == VehicleServiceOperatorMode.Automatic;
			var actor = automatic ? null : _journey.Vehicle.Controller;
			if (step is IVehicleRouteLinearStep linear)
			{
				if (linear.RouteCell.TopologyVersion != linear.PinnedTopologyVersion)
				{
					reason = "The current RouteCell topology no longer matches the compiled linear step.";
					return false;
				}

				return _owner.TryBeginMove(
					_journey.Vehicle,
					actor,
					linear.Destination.RoutePositionMetres!.Value,
					automatic,
					linear.Destination.RoutePositionMetres.Value,
					linear.Destination.RoutePositionMetres.Value,
					null,
					_journey.Id,
					_leg.Id,
					linear.Sequence,
					null,
					StepCompleted,
					out reason);
			}

			if (step is IVehicleRouteExitStep exit &&
				_owner.ExecuteExitStep(_journey.Vehicle, actor, exit, automatic, out reason))
			{
				StepCompleted(VehicleJourneyLegResult.Success);
				return true;
			}

			if (string.IsNullOrWhiteSpace(reason))
			{
				reason = "The compiled vehicle-route step type is unsupported.";
			}
			return false;
		}

		private void StepCompleted(VehicleJourneyLegResult result)
		{
			if (_finished)
			{
				return;
			}

			if (!result.Succeeded)
			{
				Complete(result);
				return;
			}

			_index++;
			if (_index >= _leg.Steps.Count)
			{
				Complete(VehicleJourneyLegResult.Success);
				return;
			}

			if (!TryStartCurrent(out var reason))
			{
				Complete(new VehicleJourneyLegResult(false, reason));
			}
		}

		private void Complete(VehicleJourneyLegResult result)
		{
			if (_finished)
			{
				return;
			}

			_finished = true;
			_owner._journeyExecutions.Remove(_journey.Id);
			_completion(result);
		}
	}

	private sealed class ManualRouteExecution
	{
		private readonly RouteVehicleMovementStrategy _owner;
		private readonly IVehicle _vehicle;
		private readonly ICharacter _actor;
		private readonly IReadOnlyList<IVehicleRouteLeg> _legs;
		private readonly Action<VehicleJourneyLegResult> _completion;
		private int _legIndex;
		private int _stepIndex;
		private bool _finished;

		public ManualRouteExecution(RouteVehicleMovementStrategy owner, IVehicle vehicle, ICharacter actor,
			IReadOnlyList<IVehicleRouteLeg> legs, Action<VehicleJourneyLegResult> completion,
			int firstStepIndex = 0)
		{
			_owner = owner;
			_vehicle = vehicle;
			_actor = actor;
			_legs = legs;
			_completion = completion;
			_stepIndex = firstStepIndex;
		}

		public bool TryStart(out string reason)
		{
			while (_legIndex < _legs.Count && _stepIndex >= _legs[_legIndex].Steps.Count)
			{
				_stepIndex = 0;
				_legIndex++;
			}

			if (_legIndex >= _legs.Count)
			{
				reason = "That vehicle is already at the end of the remaining pinned route.";
				return false;
			}

			return TryStartCurrent(out reason);
		}

		private bool TryStartCurrent(out string reason)
		{
			reason = string.Empty;
			var step = _legs[_legIndex].Steps[_stepIndex];
			if (step is IVehicleRouteLinearStep linear)
			{
				if (linear.RouteCell.TopologyVersion != linear.PinnedTopologyVersion)
				{
					reason = "The current RouteCell topology no longer matches the compiled route step.";
					return false;
				}

				return _owner.TryBeginMove(_vehicle, _actor,
					linear.Destination.RoutePositionMetres!.Value, false,
					linear.Destination.RoutePositionMetres.Value,
					linear.Destination.RoutePositionMetres.Value,
					null, null, _legs[_legIndex].Id, linear.Sequence, null,
					StepCompleted, out reason);
			}

			if (step is IVehicleRouteExitStep exit &&
				_owner.ExecuteExitStep(_vehicle, _actor, exit, false, out reason))
			{
				StepCompleted(VehicleJourneyLegResult.Success);
				return true;
			}

			if (string.IsNullOrWhiteSpace(reason))
			{
				reason = "The compiled vehicle-route step type is unsupported.";
			}
			return false;
		}

		private void StepCompleted(VehicleJourneyLegResult result)
		{
			if (_finished)
			{
				return;
			}

			if (!result.Succeeded)
			{
				Complete(result);
				return;
			}

			_stepIndex++;
			if (_stepIndex >= _legs[_legIndex].Steps.Count)
			{
				_stepIndex = 0;
				_legIndex++;
			}

			if (_legIndex >= _legs.Count)
			{
				Complete(VehicleJourneyLegResult.Success);
				return;
			}

			if (!TryStartCurrent(out var reason))
			{
				Complete(new VehicleJourneyLegResult(false, reason));
			}
		}

		private void Complete(VehicleJourneyLegResult result)
		{
			if (_finished)
			{
				return;
			}

			_finished = true;
			_completion(result);
		}
	}
}
