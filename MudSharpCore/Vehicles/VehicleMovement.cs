using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework.Scheduling;
using MudSharp.Movement;

namespace MudSharp.Vehicles;

internal enum VehicleMovementCommandResult
{
	NotVehicleController,
	Failed,
	StartedOrQueued
}

internal static class VehicleMovementCommand
{
	public static VehicleMovementCommandResult TryMoveControlledVehicle(ICharacter actor, string rawInput,
		bool requireVehicle)
	{
		var vehicle = actor.Gameworld.Vehicles.FirstOrDefault(x =>
			x.Controller?.SamePhysicalInstance(actor) == true &&
			(x.Prototype.Scale == VehicleScale.RoomScale
				? x is Vehicle concrete && concrete.IsHostedInterior(actor.Location)
				: x.Location == actor.Location && x.RoomLayer == actor.RoomLayer));
		if (vehicle is null)
		{
			if (requireVehicle)
			{
				actor.OutputHandler.Send("You are not controlling a vehicle.");
			}

			return VehicleMovementCommandResult.NotVehicleController;
		}

		if (rawInput.Contains("!!", StringComparison.Ordinal))
		{
			actor.OutputHandler.Send("Hostile guard challenges are not supported while controlling a vehicle.");
			return VehicleMovementCommandResult.Failed;
		}

		if (VehicleRouteMovementCommand.IsRouteAction(rawInput) &&
			(vehicle.Location?.RouteDefinition is not null ||
			 new StringStack(rawInput).PeekSpeech().EqualToAny("route", "stop")))
		{
			return VehicleRouteMovementCommand.Execute(vehicle, actor, rawInput);
		}

		if (actor.Movement is not null)
		{
			QueueOrCancelMove(actor, rawInput);
			return VehicleMovementCommandResult.StartedOrQueued;
		}

		var ss = new StringStack(rawInput);
		var direction = ss.PopSpeech();
		if (string.IsNullOrWhiteSpace(direction))
		{
			actor.OutputHandler.Send("Which direction do you want to drive?");
			return VehicleMovementCommandResult.Failed;
		}

		if (ss.Peek().EqualTo("!"))
		{
			ss.PopSpeech();
		}

		var target = ss.PopSafe();
		var emote = new PlayerEmote(ss.PopParentheses(), actor.Body);
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			actor.QueuedMoveCommands.Clear();
			return VehicleMovementCommandResult.Failed;
		}

		var exit = vehicle.Location?.GetExit(direction, target, actor);
		if (exit is null && string.IsNullOrWhiteSpace(target) &&
		    Constants.CardinalDirectionStringToDirection.TryGetValue(direction, out var cardinal))
		{
			exit = vehicle.Location?.GetExit(cardinal, actor);
		}

		if (exit is null || !actor.Body.CanSee(vehicle.Location, exit))
		{
			actor.OutputHandler.Send("There is no such exit for the vehicle to use.");
			actor.QueuedMoveCommands.Clear();
			return VehicleMovementCommandResult.Failed;
		}

		if (!vehicle.CanMove(actor, exit, out var reason))
		{
			actor.OutputHandler.Send(reason);
			actor.QueuedMoveCommands.Clear();
			return VehicleMovementCommandResult.Failed;
		}

		var misdirected = false;
		if (exit is not INonCardinalRoomExit &&
		    Constants.CardinalDirectionStringToDirection.TryGetValue(direction, out var manualDirection) &&
		    DirectionSenseMovement.IsCompassDirection(manualDirection) &&
		    DirectionSenseMovement.IsActive(actor, vehicle.Location))
		{
			// Use the same full tow/propulsion/motive-cohort readiness as execution, without committing it.
			if (!VehicleMovement.CanStartManualMove(vehicle, actor, exit, out reason))
			{
				actor.OutputHandler.Send(reason);
				actor.QueuedMoveCommands.Clear();
				return VehicleMovementCommandResult.Failed;
			}

			var selected = DirectionSenseMovement.Resolve(actor, exit,
				vehicle.Location.ExitsFor(vehicle.ExteriorItem ?? (IPerceiver)actor),
				candidate => vehicle.CanMove(actor, candidate, out _) &&
				             VehicleMovement.CanStartManualMove(vehicle, actor, candidate, out _, requireSafe: true),
				vehicle.Prototype.Scale == VehicleScale.RoomScale ? vehicle.ExteriorItem : actor);
			if (selected is null)
			{
				DirectionSenseMovement.NoProgress(actor);
				return VehicleMovementCommandResult.Failed;
			}

			misdirected = !ReferenceEquals(selected, exit);
			exit = selected;
		}

		new VehicleMovement(vehicle, actor, exit, misdirected).InitialAction();
		return VehicleMovementCommandResult.StartedOrQueued;
	}

	private static void QueueOrCancelMove(ICharacter actor, string rawInput)
	{
		var ss = new StringStack(rawInput);
		var direction = ss.PopSpeech();
		if (string.IsNullOrWhiteSpace(direction))
		{
			actor.OutputHandler.Send("Which direction do you want to drive?");
			return;
		}

		if (Constants.CardinalDirectionStringToDirection.ContainsKey(direction) && !actor.QueuedMoveCommands.Any())
		{
			var targetDirection = Constants.CardinalDirectionStringToDirection[direction];
			if (actor.Movement.Exit is { } currentExit &&
			    targetDirection.IsOpposingDirection(currentExit.OutboundDirection) &&
			    actor.Movement.CanBeVoluntarilyCancelled &&
			    actor.Movement.IsMovementLeader(actor))
			{
				actor.Movement.Cancel();
				actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ turn|turns around.", actor)));
				return;
			}
		}

		if (actor.Combat is not null)
		{
			actor.OutputHandler.Send("You cannot stack movement commands in combat. Wait until you stop moving first.");
			return;
		}

		actor.QueuedMoveCommands.Enqueue(rawInput);
		actor.OutputHandler.Send("");
	}
}

public class VehicleMovement : IMovement
{
	private readonly RoomExitVehicleMovementStrategy _strategy = new();
	private readonly VehicleMotiveCohortService _motiveCohortService = new();
	private readonly IVehicle _vehicle;
	private readonly ICharacter _originalMover;
	private readonly List<ICharacter> _characterMovers;
	private IReadOnlyList<IVehicle> _towTrain = [];
	private (RoomMovementTransition TransitionType, RoomLayer TargetLayer) _transition;
	private VehicleMovementReadinessResult _readiness;
	private VehiclePropulsionMovePlan _propulsionPlan;
	private VehicleMotiveCohort _motiveCohort;
	private readonly bool _misdirected;

	public VehicleMovement(IVehicle vehicle, ICharacter originalMover, IRoomExit exit, bool misdirected = false)
	{
		_misdirected = misdirected;
		_vehicle = vehicle;
		_originalMover = originalMover;
		Exit = exit;
		Phase = MovementPhase.OriginalRoom;
		Party = originalMover.Party?.Leader is not null &&
		        CharacterInstanceIdentityComparer.SamePhysicalInstance(originalMover.Party.Leader, originalMover)
			? originalMover.Party
			: null;
		_characterMovers = vehicle.Occupants.Distinct().ToList();
		if (!_characterMovers.Contains(originalMover))
		{
			_characterMovers.Add(originalMover);
		}

		var duration = Math.Max(0.0, originalMover.MoveSpeed(exit));
		var maximum = originalMover.Gameworld.GetStaticDouble("MaximumMoveTimeMilliseconds");
		if (maximum > 0.0 && duration > maximum)
		{
			duration = maximum;
		}

		Duration = TimeSpan.FromMilliseconds(duration);
	}

	public bool Cancelled { get; private set; }
	public bool CanBeVoluntarilyCancelled => Phase == MovementPhase.OriginalRoom;
	public IRoomExit Exit { get; }
	public MovementPhase Phase { get; private set; }
	public IEnumerable<ICharacter> CharacterMovers => _characterMovers.ToArray();
	public IParty Party { get; }
	public IEnumerable<IDragging> DragEffects => _motiveCohort?.Roots
		.SelectMany(x => x.EffectsOfType<IDragging>())
		.Distinct() ?? [];
	public IEnumerable<ICharacter> Draggers => DragEffects.SelectMany(x => x.CharacterDraggers).Distinct();
	public IEnumerable<ICharacter> Helpers => DragEffects.SelectMany(x => x.Helpers).Distinct();
	public IEnumerable<ICharacter> NonDraggers => _characterMovers.ToArray();
	public IEnumerable<ICharacter> NonConsensualMovers => [];
	public IEnumerable<ICharacter> Mounts => _motiveCohort?.Characters.Where(x => x.Riders.Any()) ?? [];
	public IEnumerable<IPerceivable> Targets => _vehicle.ExteriorItem is null ? [] : [_vehicle.ExteriorItem];
	public IReadOnlyDictionary<ICharacter, ISneakMoveEffect> SneakMoveEffects { get; } =
		new Dictionary<ICharacter, ISneakMoveEffect>();
	public TimeSpan Duration { get; private set; }
	public double StaminaMultiplier => _motiveCohort is null ? 0.0 : 1.0;

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

	public bool Cancel()
	{
		Cancelled = true;
		foreach (var mover in _characterMovers.ToList())
		{
			ClearMover(mover);
		}

		if (Party is not null)
		{
			Party.Movement = null;
		}
		foreach (var vehicle in _towTrain.DefaultIfEmpty(_vehicle))
		{
			vehicle.RecoverInterruptedMovement();
		}

		if (Phase == MovementPhase.OriginalRoom)
		{
			Exit.Origin.ResolveMovement(this);
		}
		else
		{
			Exit.Destination.ResolveMovement(this);
		}

		return true;
	}

	public bool CancelForMoverOnly(IMove mover, bool echo = false)
	{
		if (mover is not ICharacter ch || !_characterMovers.ContainsPhysicalInstance(ch))
		{
			return false;
		}

		if (!Cancelled &&
		    (CharacterInstanceIdentityComparer.SamePhysicalInstance(ch, _originalMover) ||
		     _motiveCohort?.Characters.ContainsPhysicalInstance(ch) == true))
		{
			return Cancel();
		}

		ClearMover(ch);
		return true;
	}

	private void ClearMover(ICharacter mover)
	{
		_characterMovers.RemovePhysicalInstance(mover);
		mover.StopMovement(this);
		if (mover.Movement == this)
		{
			mover.Movement = null;
		}
	}

	public void StopMovement()
	{
		Exit.Origin.HandleRoomEcho($"{_vehicle.ExteriorItem?.HowSeen(_originalMover, flags: PerceiveIgnoreFlags.IgnoreSelf) ?? _vehicle.Name} stops moving.", _vehicle.RoomLayer);
		Cancel();
	}

	public bool IsMovementLeader(ICharacter character)
	{
		return Party?.Leader is not null &&
		       CharacterInstanceIdentityComparer.SamePhysicalInstance(Party.Leader, character) ||
		       CharacterInstanceIdentityComparer.SamePhysicalInstance(character, _originalMover);
	}

	public bool IsConsensualMover(ICharacter character)
	{
		return true;
	}

	public string Describe(IPerceiver voyeur)
	{
		var riders = VisibleVehicleOccupants(voyeur);
		if (riders.Any())
		{
			return $"{RiderDescription(riders, voyeur)} {BeVerb(riders, voyeur)} riding {VehicleDescription(voyeur)} {Exit.OutboundMovementSuffix}{TowEchoSuffix(voyeur)}.".Proper().Colour(Telnet.Yellow);
		}

		return $"{VehicleDescription(voyeur)} is moving {Exit.OutboundMovementSuffix}{TowEchoSuffix(voyeur)}.".Proper().Colour(Telnet.Yellow);
	}

	public bool SeenBy(IPerceiver voyeur, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if (voyeur is ICharacter ch && _characterMovers.Contains(ch))
		{
			return true;
		}

		return (_vehicle.ExteriorItem is not null && voyeur.CanSee(_vehicle.ExteriorItem, flags)) ||
		       _characterMovers.Any(x => voyeur.CanSee(x, flags));
	}

	public void InitialAction()
	{
		if (_misdirected &&
		    (!_vehicle.Location.ExitsFor(_vehicle.ExteriorItem ?? (IPerceiver)_originalMover).Contains(Exit) ||
		     !DirectionSenseMovement.IsSafeVisibleCompassExit(_originalMover, _vehicle.Location, Exit,
			     _vehicle.Prototype.Scale == VehicleScale.RoomScale ? _vehicle.ExteriorItem : _originalMover) ||
		     !CanStartManualMove(_vehicle, _originalMover, Exit, out _, requireSafe: true)))
		{
			DirectionSenseMovement.NoProgress(_originalMover);
			return;
		}

		if (!RefreshMove(false, out var reason))
		{
			SendFailure(reason);
			return;
		}

		if (_readiness.ExternalPullers?.Any() == true)
		{
			var origin = RouteSpatialService.Instance.GetEffectiveLocation(_vehicle.ExteriorItem);
			if (!_motiveCohortService.TryBuild(_readiness.ExternalPullers, origin, out _motiveCohort, out reason) ||
			    !VehicleMotiveCohortService.CanTraverseExit(_motiveCohort, _readiness.MovePlan!, Exit,
				    _originalMover, out reason))
			{
				SendFailure(reason);
				return;
			}

			foreach (var character in _motiveCohort.Characters)
			{
				if (!_characterMovers.ContainsPhysicalInstance(character))
				{
					_characterMovers.Add(character);
				}
			}

			var motiveDuration = _motiveCohort.StaminaMovers
				.Select(x => Math.Max(0.0, x.MoveSpeed(Exit)))
				.DefaultIfEmpty(Duration.TotalMilliseconds)
				.Max();
			Duration = TimeSpan.FromMilliseconds(motiveDuration);
		}

		if (!_strategy.TryCommitPropulsion(_readiness, out _propulsionPlan, out reason))
		{
			SendFailure(reason);
			return;
		}

		if (_propulsionPlan is not null)
		{
			if (_motiveCohort is null)
			{
				Duration = _propulsionPlan.Duration;
			}

			foreach (var engine in (_propulsionPlan.Engines ?? []).Where(x => x.Available))
			{
				engine.Engine?.EmitOperatingNoise();
			}
		}

		Exit.Origin.RegisterMovement(this);
		foreach (var mover in _characterMovers.ToList())
		{
			mover.Movement = this;
			mover.StartMove(this);
		}

		if (Party is not null)
		{
			Party.Movement = this;
		}

		_strategy.BeginMove(_vehicle, Exit, _towTrain, _transition);
		if (_vehicle is Vehicle roomScaleVehicle && _vehicle.Prototype.Scale == VehicleScale.RoomScale)
		{
			roomScaleVehicle.EchoHostedInteriors("The vehicle shudders as it begins moving.");
		}
		foreach (var witness in Exit.Origin.LayerCharacters(_vehicle.RoomLayer).Where(x => SeenBy(x)).ToList())
		{
			witness.OutputHandler.Send(DescribeBeginMove(witness).Wrap(witness.InnerLineFormatLength));
		}

		if (TimeSpan.Zero.CompareTo(Duration) < 0)
		{
			Exit.Origin.Gameworld.Scheduler.AddSchedule(new Schedule(IntermediateStep, ScheduleType.Movement, Duration,
				"Vehicle Movement Intermediate Step"));
		}
		else
		{
			IntermediateStep();
		}
	}

	public void IntermediateStep()
	{
		if (Cancelled)
		{
			return;
		}

		if (!RefreshMove(true, out var reason))
		{
			SendFailure(reason);
			StopMovement();
			return;
		}

		var admittedPlan = _readiness.MovePlan!;
		if (VehicleExitBarrierTraversal.Contact(admittedPlan, Exit, _motiveCohort?.Characters ?? [], this,
			    out reason, continuation: () => ReferenceEquals(_readiness.MovePlan, admittedPlan)))
		{
			SendFailure(reason);
			StopMovement();
			return;
		}

		if (_motiveCohort is not null)
		{
			VehicleMotiveCohortService.MoveAcrossExit(_motiveCohort, _readiness.MovePlan!, Exit, this);
		}

		_strategy.CompleteMove(_vehicle, Exit, _transition, _readiness, this);
		if (_motiveCohort is not null)
		{
			VehicleMotiveCohortService.MoveExtraItemsAcrossExit(_motiveCohort, _readiness.MovePlan!, Exit,
				_transition.TargetLayer);
		}
		if (_vehicle is Vehicle roomScaleVehicle && _vehicle.Prototype.Scale == VehicleScale.RoomScale)
		{
			roomScaleVehicle.EchoHostedInteriors("The vehicle settles as it arrives at its next location.");
		}
		Exit.Origin.ResolveMovement(this);
		Exit.Destination.RegisterMovement(this);
		Phase = MovementPhase.NewRoom;

		foreach (var witness in Exit.Destination.LayerCharacters(_transition.TargetLayer)
		                            .Where(x => !_characterMovers.ContainsPhysicalInstance(x))
		                            .Where(x => SeenBy(x))
		                            .ToList())
		{
			witness.OutputHandler.Send(DescribeEnterMove(witness).Wrap(witness.InnerLineFormatLength));
		}

		foreach (var mover in _characterMovers.ToList())
		{
			mover.Body.Look(true);
		}

		var finalDelay = TimeSpan.FromTicks(Duration.Ticks / 5);
		if (TimeSpan.Zero.CompareTo(finalDelay) < 0)
		{
			Exit.Origin.Gameworld.Scheduler.AddSchedule(new Schedule(FinalStep, ScheduleType.Movement,
				finalDelay.TotalSeconds > 5 ? TimeSpan.FromSeconds(5) : finalDelay, "Vehicle Movement Final Step"));
		}
		else
		{
			FinalStep();
		}
	}

	public void FinalStep()
	{
		if (Cancelled)
		{
			return;
		}

		foreach (var mover in _characterMovers.ToList())
		{
			_characterMovers.Remove(mover);
			if (mover.Movement == this)
			{
				mover.Movement = null;
			}
		}

		if (Party is not null)
		{
			Party.Movement = null;
		}

		Exit.Destination.ResolveMovement(this);
		if (_originalMover.QueuedMoveCommands.Count > 0)
		{
			_originalMover.Move(_originalMover.QueuedMoveCommands.Dequeue());
		}
	}

	private bool RefreshMove(bool rollTowCatastrophe, out string reason)
	{
		return _strategy.TryPrepareMove(_vehicle, _originalMover, Exit, rollTowCatastrophe, out _towTrain,
			out _transition, out _readiness, out reason, _propulsionPlan);
	}

	private void SendFailure(string reason)
	{
		if (_misdirected) DirectionSenseMovement.NoProgress(_originalMover);
		else _originalMover.OutputHandler.Send(reason);
	}

	/// <summary>Silent, read-only full readiness; never commits propulsion or rolls tow catastrophe.</summary>
	public static bool CanStartManualMove(IVehicle vehicle, ICharacter actor, IRoomExit exit,
		out string reason, bool requireSafe = false)
	{
		var strategy = new RoomExitVehicleMovementStrategy();
		if (!strategy.TryPrepareMove(vehicle, actor, exit, false, out _, out var transition,
			    out var readiness, out reason))
		{
			return false;
		}

		if (requireSafe && (exit.IsFallExit || transition.TransitionType is
		    RoomMovementTransition.FallExit or RoomMovementTransition.NoViableTransition))
		{
			reason = DirectionSenseMovement.NoProgressMessage;
			return false;
		}

		if (readiness.ExternalPullers?.Any() != true) return true;
		var service = new VehicleMotiveCohortService();
		if (!service.TryBuild(readiness.ExternalPullers,
			    RouteSpatialService.Instance.GetEffectiveLocation(vehicle.ExteriorItem), out var cohort, out reason)) return false;
		if (requireSafe && cohort.StaminaMovers.Any(x => exit.MovementTransition(x).TransitionType is
		    RoomMovementTransition.FallExit or RoomMovementTransition.NoViableTransition))
		{
			reason = DirectionSenseMovement.NoProgressMessage;
			return false;
		}
		return VehicleMotiveCohortService.CanTraverseExit(cohort, readiness.MovePlan!, exit, actor, out reason);
	}

	private string DescribeBeginMove(IPerceiver voyeur)
	{
		var riders = VisibleVehicleOccupants(voyeur);
		if (riders.Any())
		{
			return $"{RiderDescription(riders, voyeur)} {BeginVerb(riders, voyeur)} riding {VehicleDescription(voyeur)} {Exit.OutboundMovementSuffix}{TowEchoSuffix(voyeur)}.".Proper();
		}

		return $"{VehicleDescription(voyeur)} begins moving {Exit.OutboundMovementSuffix}{TowEchoSuffix(voyeur)}.".Proper();
	}

	private string DescribeEnterMove(IPerceiver voyeur)
	{
		var riders = VisibleVehicleOccupants(voyeur);
		if (riders.Any())
		{
			return $"{RiderDescription(riders, voyeur)} {RideVerb(riders, voyeur)} {Exit.InboundMovementSuffix} on {VehicleDescription(voyeur)}{TowEchoSuffix(voyeur)}.".Proper();
		}

		return $"{VehicleDescription(voyeur)} arrives {Exit.InboundMovementSuffix}{TowEchoSuffix(voyeur)}.".Proper();
	}

	private string VehicleDescription(IPerceiver voyeur)
	{
		return _vehicle.ExteriorItem?.HowSeen(voyeur) ?? _vehicle.Name;
	}

	private List<ICharacter> VisibleVehicleOccupants(IPerceiver voyeur)
	{
		if (_vehicle.Prototype.Scale == VehicleScale.RoomScale)
		{
			return [];
		}

		if (_vehicle.ExteriorItem is not null && !voyeur.CanSee(_vehicle.ExteriorItem))
		{
			return [];
		}

		return _characterMovers
		       .Where(_vehicle.IsOccupant)
		       .Where(x => voyeur.CanSee(x))
		       .Distinct()
		       .ToList();
	}

	private static string RiderDescription(List<ICharacter> riders, IPerceiver voyeur)
	{
		return riders
		       .OrderBy(x => x == voyeur)
		       .Select(x => x.HowSeen(voyeur))
		       .ListToString();
	}

	private static string BeginVerb(List<ICharacter> riders, IPerceiver voyeur)
	{
		return riders.Count == 1 && !riders[0].IsSelf(voyeur) ? "begins" : "begin";
	}

	private static string RideVerb(List<ICharacter> riders, IPerceiver voyeur)
	{
		return riders.Count == 1 && !riders[0].IsSelf(voyeur) ? "rides" : "ride";
	}

	private static string BeVerb(List<ICharacter> riders, IPerceiver voyeur)
	{
		return riders.Count == 1 && !riders[0].IsSelf(voyeur) ? "is" : "are";
	}

	private string TowEchoSuffix(IPerceiver voyeur)
	{
		var towed = _towTrain.Skip(1).ToList();
		return towed.Any()
			? $" towing {towed.Select(x => x.ExteriorItem?.HowSeen(voyeur) ?? x.Name).ListToString()}"
			: string.Empty;
	}
}
