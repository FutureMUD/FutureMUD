#nullable enable

using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.Movement;

/// <summary>
/// Continuous character movement within a single RouteCell. The cell and layer never change;
/// RouteSpatialService supplies the effective lazy coordinate between durable checkpoints.
/// </summary>
public sealed class LinearRouteMovement : ILinearRouteMovement
{
	private static readonly object ActiveMovementLock = new();
	private static readonly HashSet<LinearRouteMovement> ActiveMovements = [];
	private readonly ICharacter _rootMover;
	private readonly RouteSpatialService _spatialService;
	private readonly IRouteMotionPersistence _persistence;
	private readonly Action<Action, TimeSpan> _schedule;
	private readonly TimeSpan _checkpointInterval;
	private readonly List<ICharacter> _characterMovers;
	private readonly List<ICharacter> _draggers;
	private readonly List<ICharacter> _helpers;
	private readonly List<ICharacter> _nonDraggers;
	private readonly List<ICharacter> _nonConsensualMovers;
	private readonly List<ICharacter> _mounts;
	private readonly List<IPerceivable> _targets;
	private readonly List<IDragging> _dragEffects;
	private readonly List<ILocateable> _locateables;
	private readonly List<ICharacter> _staminaMovers;
	private readonly long _topologyVersion;
	private readonly SpatialLocation _operationOrigin;
	private readonly TimeSpan _operationDuration;
	private readonly double _operationDistanceMetres;
	private readonly double _targetMinimumMetres;
	private readonly double _targetMaximumMetres;
	private readonly long? _selectedExitId;
	private readonly bool _createTracks;
	private readonly RouteMovementHookContext _hookContext;
	private long _checkpointSequence;
	private long _scheduleGeneration;
	private double _lastCheckpointPosition;
	private bool _started;
	private bool _finished;

	private LinearRouteMovement(
		ICharacter rootMover,
		LinearRouteMovementSegment segment,
		Cohort cohort,
		double targetMinimumMetres,
		double targetMaximumMetres,
		long? selectedExitId,
		RouteSpatialService spatialService,
		IRouteMotionPersistence persistence,
		Action<Action, TimeSpan> schedule,
		TimeSpan checkpointInterval,
		bool createTracks)
	{
		_rootMover = rootMover;
		Segment = segment;
		_spatialService = spatialService;
		_persistence = persistence;
		_schedule = schedule;
		_checkpointInterval = checkpointInterval;
		_characterMovers = cohort.Characters;
		_draggers = cohort.Draggers;
		_helpers = cohort.Helpers;
		_nonDraggers = cohort.NonDraggers;
		_nonConsensualMovers = cohort.NonConsensualMovers;
		_mounts = cohort.Mounts;
		_targets = cohort.Targets;
		_dragEffects = cohort.DragEffects;
		_locateables = cohort.Locateables;
		_staminaMovers = cohort.StaminaMovers;
		_targetMinimumMetres = targetMinimumMetres;
		_targetMaximumMetres = targetMaximumMetres;
		_selectedExitId = selectedExitId;
		_createTracks = createTracks;
		_topologyVersion = segment.Origin.Cell.RouteDefinition!.TopologyVersion;
		_operationOrigin = segment.Origin;
		_operationDuration = segment.Duration;
		_operationDistanceMetres = segment.DistanceMetres;
		_lastCheckpointPosition = segment.Origin.RoutePositionMetres!.Value;
		OperationId = Guid.NewGuid();
		_hookContext = new RouteMovementHookContext(
			OperationId,
			segment.Origin.Cell,
			segment.Origin.RoutePositionMetres.Value,
			segment.Destination.RoutePositionMetres!.Value,
			segment.Direction,
			segment.SpeedMetresPerSecond);
	}

	public LinearRouteMovementSegment Segment { get; private set; }
	public bool Cancelled { get; private set; }
	public bool CanBeVoluntarilyCancelled => !_finished;
	public ICellExit? Exit => null;
	public MovementPhase Phase => MovementPhase.OriginalRoom;
	public IEnumerable<ICharacter> CharacterMovers => _characterMovers.ToArray();
	public IParty Party => _rootMover.Party;
	public IEnumerable<IDragging> DragEffects => _dragEffects.ToArray();
	public IEnumerable<ICharacter> Draggers => _draggers.ToArray();
	public IEnumerable<ICharacter> Helpers => _helpers.ToArray();
	public IEnumerable<ICharacter> NonDraggers => _nonDraggers.ToArray();
	public IEnumerable<ICharacter> NonConsensualMovers => _nonConsensualMovers.ToArray();
	public IEnumerable<ICharacter> Mounts => _mounts.ToArray();
	public IEnumerable<IPerceivable> Targets => _targets.ToArray();
	public IReadOnlyDictionary<ICharacter, ISneakMoveEffect> SneakMoveEffects { get; } =
		new Dictionary<ICharacter, ISneakMoveEffect>();
	public TimeSpan Duration => _operationDuration;
	public double StaminaMultiplier => _operationDistanceMetres /
	                                   Segment.Origin.Cell.RouteDefinition!.MetresPerRoomEquivalent;
	public SpatialLocation Origin => _operationOrigin;
	public SpatialLocation Destination => Segment.Destination;
	public RouteCellDirection Direction => Segment.Direction;
	public double SpeedMetresPerSecond => Segment.SpeedMetresPerSecond;
	public Guid OperationId { get; }

	public static bool TryCreate(
		ICharacter rootMover,
		double targetPositionMetres,
		out LinearRouteMovement? movement,
		out string error,
		double? speedMetresPerSecond = null,
		double? targetMinimumMetres = null,
		double? targetMaximumMetres = null,
		long? selectedExitId = null,
		RouteSpatialService? spatialService = null,
		IRouteMotionPersistence? persistence = null,
		Action<Action, TimeSpan>? schedule = null,
		TimeSpan? checkpointInterval = null,
		bool createTracks = true)
	{
		ArgumentNullException.ThrowIfNull(rootMover);
		spatialService ??= RouteSpatialService.Instance;
		var origin = spatialService.GetEffectiveLocation(rootMover);
		var route = origin.Cell.RouteDefinition;
		if (route is null || !origin.RoutePositionMetres.HasValue)
		{
			movement = null;
			error = "You can only travel longitudinally while you are in a RouteCell.";
			return false;
		}

		if (!double.IsFinite(targetPositionMetres) ||
			targetPositionMetres < 0.0 || targetPositionMetres > route.LengthMetres)
		{
			movement = null;
			error = $"The destination must be between 0 and {route.LengthMetres:N3} metres.";
			return false;
		}

		if (Math.Abs(targetPositionMetres - origin.RoutePositionMetres.Value) < 0.0005)
		{
			movement = null;
			error = "You are already at that route position.";
			return false;
		}

		if (rootMover.Movement is not null)
		{
			movement = null;
			error = "You are already moving.";
			return false;
		}

		var canMove = rootMover.CanMove(CanMoveFlags.None);
		if (!canMove.Result)
		{
			movement = null;
			error = canMove.ErrorMessage;
			return false;
		}

		if (!TryBuildCohort(rootMover, origin, spatialService, out var cohort, out error))
		{
			movement = null;
			return false;
		}

		var speed = speedMetresPerSecond ?? ResolveCharacterSpeed(rootMover, route);
		LinearRouteMovementSegment segment;
		try
		{
			segment = new LinearRouteMovementSegment(
				origin,
				new SpatialLocation(origin.Cell, origin.Layer, targetPositionMetres),
				speed);
		}
		catch (ArgumentException ex)
		{
			movement = null;
			error = ex.Message;
			return false;
		}

		var interval = checkpointInterval ?? TimeSpan.FromSeconds(Math.Max(1.0,
			rootMover.Gameworld.GetStaticDouble("RouteCellMovementCheckpointSeconds")));
		if (interval <= TimeSpan.Zero)
		{
			movement = null;
			error = "The RouteCell checkpoint interval must be positive.";
			return false;
		}

		persistence ??= new DatabaseRouteMotionPersistence();
		schedule ??= (action, delay) => rootMover.Gameworld.Scheduler.AddSchedule(
			new Schedule(action, ScheduleType.Movement, delay, "Linear RouteCell movement checkpoint"));

		movement = new LinearRouteMovement(
			rootMover,
			segment,
			cohort,
			targetMinimumMetres ?? targetPositionMetres,
			targetMaximumMetres ?? targetPositionMetres,
			selectedExitId,
			spatialService,
			persistence,
			schedule,
			interval,
			createTracks);
		error = string.Empty;
		return true;
	}

	public void InitialAction()
	{
		if (_started || _finished)
		{
			return;
		}

		if (!CanAffordNextInterval())
		{
			_rootMover.OutputHandler.Send("You are too exhausted to begin travelling that way.");
			Cancelled = true;
			_finished = true;
			return;
		}

		_persistence.Start(
			_rootMover,
			Segment,
			OperationId,
			_targetMinimumMetres,
			_targetMaximumMetres,
			_selectedExitId,
			_locateables);
		_started = true;
		lock (ActiveMovementLock)
		{
			ActiveMovements.Add(this);
		}

		foreach (var mover in _characterMovers)
		{
			mover.Movement = this;
			mover.StartMove(this);
		}

		BeginLazySegment(Segment);
		RouteMovementFutureProgEvents.Begin(HookTargets(), _hookContext);
		if (_finished)
		{
			return;
		}

		CreateTracksAtCheckpoint();
		RouteMovementOutput.CharacterBegin(_rootMover, DirectionName());
		ScheduleNextCheckpoint();
	}

	public bool Cancel()
	{
		if (_finished)
		{
			return false;
		}

		Cancelled = true;
		_scheduleGeneration++;
		if (_started)
		{
			CommitEffectivePosition();
		}
		Finish(false, "stop|stops travelling");
		return true;
	}

	public bool CancelForMoverOnly(IMove mover, bool echo = false)
	{
		if (mover is not ICharacter character ||
			!_characterMovers.Any(x => CharacterInstanceIdentityComparer.SamePhysicalInstance(x, character)))
		{
			return false;
		}

		// Route cohorts have one reference coordinate and therefore stop atomically.
		return Cancel();
	}

	public void StopMovement()
	{
		Cancel();
	}

	public bool IsMovementLeader(ICharacter character)
	{
		return CharacterInstanceIdentityComparer.SamePhysicalInstance(_rootMover, character);
	}

	public bool IsConsensualMover(ICharacter character)
	{
		return !_nonConsensualMovers.Any(x =>
			CharacterInstanceIdentityComparer.SamePhysicalInstance(x, character));
	}

	public string Describe(IPerceiver voyeur)
	{
		return $"{_rootMover.HowSeen(voyeur, true)} is travelling {DirectionName()}.";
	}

	public bool SeenBy(IPerceiver voyeur, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if (voyeur is not ILocateable locateable)
		{
			return false;
		}

		return _spatialService.GetProximity(locateable, _rootMover) <= Proximity.Distant &&
		       voyeur.CanSee(_rootMover, flags);
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

	private void ScheduleNextCheckpoint()
	{
		if (_finished)
		{
			return;
		}

		var remaining = Math.Abs(Destination.RoutePositionMetres!.Value - _lastCheckpointPosition) /
		                SpeedMetresPerSecond;
		if (remaining <= 0.000001)
		{
			CommitEffectivePosition();
			if (!_finished)
			{
				Finish(true, "arrive|arrives at the destination");
			}
			return;
		}

		var delay = TimeSpan.FromSeconds(Math.Min(_checkpointInterval.TotalSeconds, remaining));
		var generation = ++_scheduleGeneration;
		_schedule(() => Checkpoint(generation), delay);
	}

	private void Checkpoint(long generation)
	{
		if (_finished || generation != _scheduleGeneration)
		{
			return;
		}

		if (Origin.Cell.RouteDefinition?.TopologyVersion != _topologyVersion)
		{
			CommitEffectivePosition();
			if (_finished)
			{
				return;
			}

			Cancelled = true;
			Finish(false, "stop|stops because the route topology has changed");
			return;
		}

		CommitEffectivePosition();
		if (_finished)
		{
			return;
		}

		if (Math.Abs(Destination.RoutePositionMetres!.Value - _lastCheckpointPosition) < 0.0005)
		{
			Finish(true, "arrive|arrives at the destination");
			return;
		}

		if (!CanAffordNextInterval())
		{
			Cancelled = true;
			Finish(false, "stop|stops, too exhausted to continue");
			return;
		}

		var remainingSegment = new LinearRouteMovementSegment(
			new SpatialLocation(Origin.Cell, Origin.Layer, _lastCheckpointPosition),
			Destination,
			SpeedMetresPerSecond);
		Segment = remainingSegment;
		BeginLazySegment(remainingSegment);
		CreateTracksAtCheckpoint();
		ScheduleNextCheckpoint();
	}

	private void CommitEffectivePosition()
	{
		var previousPosition = _lastCheckpointPosition;
		var effective = _spatialService.GetEffectiveLocation(_rootMover);
		var position = effective.RoutePositionMetres ?? _lastCheckpointPosition;
		foreach (var locateable in _locateables)
		{
			_spatialService.MaterialiseActiveMovement(locateable);
			if (locateable.RoutePositionMetres != position)
			{
				locateable.SetRoutePosition(position);
			}
		}

		var distance = Math.Abs(position - _lastCheckpointPosition);
		var charges = BuildCharges(distance);
		var staminaSnapshots = charges
			.Select(x => x.Character)
			.Distinct()
			.ToDictionary(x => x, x => x.Body.CurrentStamina);
		var saveQueueStates = RouteCheckpointSaveQueue.Capture(charges
			.Select(x => (ISaveable)x.Character.Body));
		_checkpointSequence++;
		var remaining = TimeSpan.FromSeconds(
			Math.Abs(Destination.RoutePositionMetres!.Value - position) / SpeedMetresPerSecond);
		try
		{
			_persistence.CommitCheckpoint(new RouteMotionCheckpoint(
				OperationId,
				_checkpointSequence,
				position,
				remaining,
				charges), committedCharges =>
			{
				foreach (var charge in committedCharges)
				{
					charge.Character.SpendStamina(charge.Amount);
				}
			});
		}
		catch (Exception exception)
		{
			var rollbackFailures = new List<Exception>();
			foreach (var (character, stamina) in staminaSnapshots)
			{
				try
				{
					character.Body.CurrentStamina = stamina;
				}
				catch (Exception rollbackException)
				{
					rollbackFailures.Add(rollbackException);
				}
			}
			foreach (var locateable in _locateables)
			{
				try
				{
					_spatialService.MaterialiseActiveMovement(locateable);
					locateable.SetRoutePosition(previousPosition);
				}
				catch (Exception rollbackException)
				{
					rollbackFailures.Add(rollbackException);
				}
			}
			RouteCheckpointSaveQueue.PreserveRollback(saveQueueStates, rollbackFailures.Add);
			_lastCheckpointPosition = previousPosition;
			var rollbackSuffix = rollbackFailures.Count == 0
				? string.Empty
				: $" Rollback also reported {rollbackFailures.Count:N0} error(s): {rollbackFailures[0].Message}";
			Cancelled = true;
			_rootMover.Gameworld.SystemMessage(
				$"RouteCell movement {OperationId:N} stopped after checkpoint {_checkpointSequence:N0} failed: {exception.Message}{rollbackSuffix}",
				true);
			Finish(false, "stop|stops because the durable movement checkpoint could not be committed");
			return;
		}

		RouteCheckpointSaveQueue.Restore(saveQueueStates, exception =>
			_rootMover.Gameworld.SystemMessage(
				$"RouteCell movement {OperationId:N} committed checkpoint {_checkpointSequence:N0}, but could not restore an affected save-queue entry: {exception.Message}",
				true));

		_lastCheckpointPosition = position;
		RouteMovementFutureProgEvents.Progress(
			HookTargets(),
			_hookContext,
			previousPosition,
			position);
		if (distance >= 0.0005)
		{
			RouteMovementOutput.CharacterProgress(_rootMover, DirectionName());
		}
	}

	private void BeginLazySegment(LinearRouteMovementSegment segment)
	{
		foreach (var locateable in _locateables)
		{
			_spatialService.BeginActiveMovement(locateable, segment);
		}
	}

	private void Finish(bool completed, string echo)
	{
		if (_finished)
		{
			return;
		}

		_finished = true;
		_scheduleGeneration++;
		lock (ActiveMovementLock)
		{
			ActiveMovements.Remove(this);
		}
		try
		{
			_persistence.Complete(OperationId);
		}
		catch (Exception exception)
		{
			_rootMover.Gameworld.SystemMessage(
				$"RouteCell movement {OperationId:N} could not clear its durable motion row: {exception.Message}",
				true);
		}
		foreach (var mover in _characterMovers)
		{
			if (completed)
			{
				mover.Moved(this);
			}

			mover.StopMovement(this);
			if (ReferenceEquals(mover.Movement, this))
			{
				mover.Movement = null!;
			}
		}

		if (completed)
		{
			RouteMovementFutureProgEvents.Complete(HookTargets(), _hookContext);
		}
		else
		{
			RouteMovementFutureProgEvents.Cancelled(HookTargets(), _hookContext, _lastCheckpointPosition, echo);
		}

		RouteMovementOutput.CharacterFinished(_rootMover, echo);
	}

	private IEnumerable<IPerceivable> HookTargets()
	{
		return _characterMovers.Cast<IPerceivable>();
	}

	private bool CanAffordNextInterval()
	{
		var remainingDistance = Math.Abs(Destination.RoutePositionMetres!.Value - _lastCheckpointPosition);
		var intervalDistance = Math.Min(remainingDistance,
			SpeedMetresPerSecond * _checkpointInterval.TotalSeconds);
		return BuildCharges(intervalDistance).All(x => x.Character.CanSpendStamina(x.Amount));
	}

	private IReadOnlyCollection<RouteMotionResourceCharge> BuildCharges(double distanceMetres)
	{
		if (distanceMetres <= 0.0)
		{
			return Array.Empty<RouteMotionResourceCharge>();
		}

		var route = Origin.Cell.RouteDefinition!;
		var roomEquivalents = distanceMetres / route.MetresPerRoomEquivalent;
		return _staminaMovers
			.Select(character =>
			{
				var terrainCost = character.PositionState.IgnoreTerrainStaminaCostsForMovement
					? 1.0
					: character.Location.Terrain(character)?.StaminaCost ??
					  character.Gameworld.GetStaticDouble("DefaultTerrainStaminaCost");
				var encumbrance = 1.0 + character.EncumbrancePercentage *
					character.Gameworld.GetStaticDouble("StaminaMultiplierPerEncumbrancePercentage");
				var speedCost = character.CurrentSpeed?.StaminaMultiplier ?? 1.0;
				return new RouteMotionResourceCharge(
					character,
					Math.Max(0.0, roomEquivalents * terrainCost * encumbrance * speedCost),
					"stamina");
			})
			.ToArray();
	}

	private void CreateTracksAtCheckpoint()
	{
		if (!_createTracks || !_rootMover.Gameworld.GetStaticBool("TrackingEnabled") ||
			!Origin.Cell.Terrain(_rootMover).CanHaveTracks)
		{
			return;
		}

		foreach (var mover in _staminaMovers.Where(x => !x.IsAdministrator()))
		{
			var circumstances = _dragEffects.Any(x => ReferenceEquals(x.Target, mover))
				? TrackCircumstances.Dragged
				: TrackCircumstances.None;
			if (Movement.GetTrackIntensities(mover, Origin.Cell, ref circumstances, out var visual, out var olfactory))
			{
				continue;
			}

			var track = new Track(
				mover.Gameworld,
				mover,
				_lastCheckpointPosition,
				Direction,
				circumstances,
				visual,
				olfactory);
			mover.Gameworld.Add(track);
			Origin.Cell.AddTrack(track);
		}
	}

	private string DirectionName()
	{
		var route = Origin.Cell.RouteDefinition!;
		return Direction == RouteCellDirection.Positive
			? route.PositiveDirectionName
			: route.NegativeDirectionName;
	}

	private static double ResolveCharacterSpeed(ICharacter character, IRouteCellDefinition route)
	{
		// A normal walking gait is approximately 1.4m/s. Existing movement-speed multipliers
		// represent relative duration, so smaller values produce faster longitudinal travel.
		var gaitMultiplier = Math.Max(0.05, character.CurrentSpeed?.Multiplier ?? 1.0);
		var speed = 1.4 / gaitMultiplier;
		return Math.Clamp(speed, 0.05, Math.Max(0.05, route.MetresPerRoomEquivalent));
	}

	/// <summary>
	/// Graceful-shutdown boundary: materialise every lazy character movement before the final
	/// save flush. Crash recovery instead uses the last durable database checkpoint.
	/// </summary>
	public static int MaterialiseAllForShutdown(IFuturemud gameworld)
	{
		LinearRouteMovement[] movements;
		lock (ActiveMovementLock)
		{
			movements = ActiveMovements
				.Where(x => ReferenceEquals(x._rootMover.Gameworld, gameworld))
				.ToArray();
		}

		foreach (var movement in movements)
		{
			movement.Cancel();
		}

		return movements.Length;
	}

	private static bool TryBuildCohort(
		ICharacter root,
		SpatialLocation origin,
		RouteSpatialService spatialService,
		out Cohort cohort,
		out string error)
	{
		var characters = new HashSet<ICharacter>(CharacterPhysicalInstanceEqualityComparer.Instance);
		var targets = new HashSet<IPerceivable>(ReferenceEqualityComparer.Instance);
		var dragEffects = new HashSet<IDragging>(ReferenceEqualityComparer.Instance);
		var nonConsensual = new HashSet<ICharacter>(CharacterPhysicalInstanceEqualityComparer.Instance);
		var draggers = new HashSet<ICharacter>(CharacterPhysicalInstanceEqualityComparer.Instance);
		var helpers = new HashSet<ICharacter>(CharacterPhysicalInstanceEqualityComparer.Instance);
		var queue = new Queue<ICharacter>();
		var immediate = RouteSpatialConfiguration.FromGameworld(root.Gameworld).ImmediateDistanceMetres;
		var nearbyCharacters = spatialService
			.GetPerceivablesWithin(origin, immediate)
			.OfType<ICharacter>()
			.ToList();

		void AddCharacter(ICharacter? character)
		{
			if (character is not null && characters.Add(character))
			{
				queue.Enqueue(character);
			}
		}

		AddCharacter(root);
		if (root.Party is not null &&
			CharacterInstanceIdentityComparer.SamePhysicalInstance(root.Party.Leader, root))
		{
			foreach (var member in root.Party.ActiveCharacterMembers)
			{
				AddCharacter(member);
			}
		}

		while (queue.Count > 0)
		{
			var character = queue.Dequeue();
			foreach (var follower in nearbyCharacters.Where(x =>
				x.Following is ICharacter followed &&
				CharacterInstanceIdentityComparer.SamePhysicalInstance(character, followed)))
			{
				AddCharacter(follower);
			}

			if (character.RidingMount is not null)
			{
				AddCharacter(character.RidingMount);
				nonConsensual.Add(character);
			}

			foreach (var rider in character.Riders)
			{
				AddCharacter(rider);
				nonConsensual.Add(rider);
			}

			foreach (var dragging in character.EffectsOfType<Dragging>())
			{
				dragEffects.Add(dragging);
				foreach (var dragger in dragging.CharacterDraggers)
				{
					AddCharacter(dragger);
					draggers.Add(dragger);
				}

				foreach (var helper in dragging.Helpers)
				{
					AddCharacter(helper);
					helpers.Add(helper);
				}

				targets.Add(dragging.Target);
				if (dragging.Target is ICharacter targetCharacter)
				{
					AddCharacter(targetCharacter);
					nonConsensual.Add(targetCharacter);
				}
			}

			foreach (var hitch in character.EffectsOfType<CharacterHitch>())
			{
				targets.Add(hitch.Target);
				if (hitch.Target is ICharacter hitchedCharacter)
				{
					AddCharacter(hitchedCharacter);
				}
			}
		}

		var locateables = characters.Cast<ILocateable>()
			.Concat(targets.OfType<ILocateable>())
			.Distinct<ILocateable>(ReferenceEqualityComparer.Instance)
			.ToList();
		foreach (var locateable in locateables)
		{
			var location = spatialService.GetEffectiveLocation(locateable);
			if (!ReferenceEquals(location.Cell, origin.Cell) || location.Layer != origin.Layer ||
				!location.RoutePositionMetres.HasValue ||
				Math.Abs(location.RoutePositionMetres.Value - origin.RoutePositionMetres!.Value) > immediate)
			{
				cohort = default!;
				error = $"{locateable.Name} is not close enough to move with the route-travel cohort.";
				return false;
			}
		}

		foreach (var character in characters)
		{
			if (character.Movement is not null)
			{
				cohort = default!;
				error = $"{character.HowSeen(root, true)} is already moving.";
				return false;
			}

			if (nonConsensual.Contains(character) || character.RidingMount is not null)
			{
				continue;
			}

			var canMove = character.CanMove(CanMoveFlags.None);
			if (!canMove.Result)
			{
				cohort = default!;
				error = $"{character.HowSeen(root, true)} cannot move: {canMove.ErrorMessage}";
				return false;
			}
		}

		var mounts = characters.Where(x => x.Riders.Any()).ToList();
		var nonDraggers = characters.Where(x => !draggers.Contains(x) && !helpers.Contains(x)).ToList();
		var staminaMovers = characters
			.Where(x => x.RidingMount is null)
			.Where(x => !nonConsensual.Contains(x))
			.ToList();
		cohort = new Cohort(
			characters.ToList(),
			draggers.ToList(),
			helpers.ToList(),
			nonDraggers,
			nonConsensual.ToList(),
			mounts,
			targets.ToList(),
			dragEffects.ToList(),
			locateables,
			staminaMovers);
		error = string.Empty;
		return true;
	}

	private sealed record Cohort(
		List<ICharacter> Characters,
		List<ICharacter> Draggers,
		List<ICharacter> Helpers,
		List<ICharacter> NonDraggers,
		List<ICharacter> NonConsensualMovers,
		List<ICharacter> Mounts,
		List<IPerceivable> Targets,
		List<IDragging> DragEffects,
		List<ILocateable> Locateables,
		List<ICharacter> StaminaMovers);
}
