using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;

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
		var vehicle = actor.Gameworld.Vehicles.FirstOrDefault(x => x.Controller == actor);
		if (vehicle is null)
		{
			if (requireVehicle)
			{
				actor.OutputHandler.Send("You are not controlling a vehicle.");
			}

			return VehicleMovementCommandResult.NotVehicleController;
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

		if (exit is null)
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

		new VehicleMovement(vehicle, actor, exit).InitialAction();
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
			if (targetDirection.IsOpposingDirection(actor.Movement.Exit.OutboundDirection) &&
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
	private readonly CellExitVehicleMovementStrategy _strategy = new();
	private readonly IVehicle _vehicle;
	private readonly ICharacter _originalMover;
	private readonly List<ICharacter> _characterMovers;
	private IReadOnlyList<IVehicle> _towTrain = [];
	private (CellMovementTransition TransitionType, RoomLayer TargetLayer) _transition;

	public VehicleMovement(IVehicle vehicle, ICharacter originalMover, ICellExit exit)
	{
		_vehicle = vehicle;
		_originalMover = originalMover;
		Exit = exit;
		Phase = MovementPhase.OriginalRoom;
		Party = originalMover.Party?.Leader == originalMover ? originalMover.Party : null;
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
	public ICellExit Exit { get; }
	public MovementPhase Phase { get; private set; }
	public IEnumerable<ICharacter> CharacterMovers => _characterMovers.ToArray();
	public IParty Party { get; }
	public IEnumerable<IDragging> DragEffects => [];
	public IEnumerable<ICharacter> Draggers => [];
	public IEnumerable<ICharacter> Helpers => [];
	public IEnumerable<ICharacter> NonDraggers => _characterMovers.ToArray();
	public IEnumerable<ICharacter> NonConsensualMovers => [];
	public IEnumerable<ICharacter> Mounts => [];
	public IEnumerable<IPerceivable> Targets => _vehicle.ExteriorItem is null ? [] : [_vehicle.ExteriorItem];
	public IReadOnlyDictionary<ICharacter, ISneakMoveEffect> SneakMoveEffects { get; } =
		new Dictionary<ICharacter, ISneakMoveEffect>();
	public TimeSpan Duration { get; }
	public double StaminaMultiplier => 0.0;

	public MovementType MovementTypeForMover(ICharacter mover)
	{
		return MovementType.Upright;
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
		if (mover is not ICharacter ch || !_characterMovers.Contains(ch))
		{
			return false;
		}

		if (ch == _originalMover && !Cancelled)
		{
			return Cancel();
		}

		ClearMover(ch);
		return true;
	}

	private void ClearMover(ICharacter mover)
	{
		_characterMovers.Remove(mover);
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
		return Party?.Leader == character || character == _originalMover;
	}

	public bool IsConsensualMover(ICharacter character)
	{
		return true;
	}

	public string Describe(IPerceiver voyeur)
	{
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
		if (!RefreshMove(out var reason))
		{
			_originalMover.OutputHandler.Send(reason);
			return;
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

		if (!RefreshMove(out var reason))
		{
			_originalMover.OutputHandler.Send(reason);
			StopMovement();
			return;
		}

		_strategy.CompleteMove(_vehicle, Exit, _towTrain, _transition, this);
		Exit.Origin.ResolveMovement(this);
		Exit.Destination.RegisterMovement(this);
		Phase = MovementPhase.NewRoom;

		foreach (var witness in Exit.Destination.LayerCharacters(_transition.TargetLayer)
		                            .Except(_characterMovers)
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

	private bool RefreshMove(out string reason)
	{
		return _strategy.TryPrepareMove(_vehicle, _originalMover, Exit, out _towTrain, out _transition, out reason);
	}

	private string DescribeBeginMove(IPerceiver voyeur)
	{
		return $"{VehicleDescription(voyeur)} begins moving {Exit.OutboundMovementSuffix}{TowEchoSuffix(voyeur)}.".Proper();
	}

	private string DescribeEnterMove(IPerceiver voyeur)
	{
		return $"{VehicleDescription(voyeur)} arrives {Exit.InboundMovementSuffix}{TowEchoSuffix(voyeur)}.".Proper();
	}

	private string VehicleDescription(IPerceiver voyeur)
	{
		return _vehicle.ExteriorItem?.HowSeen(voyeur) ?? _vehicle.Name;
	}

	private string TowEchoSuffix(IPerceiver voyeur)
	{
		var towed = _towTrain.Skip(1).ToList();
		return towed.Any()
			? $" towing {towed.Select(x => x.ExteriorItem?.HowSeen(voyeur) ?? x.Name).ListToString()}"
			: string.Empty;
	}
}
