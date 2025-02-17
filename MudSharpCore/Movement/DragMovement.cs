using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Movement;

public class DragMovement : MovementBase
{
	public override double StaminaMultiplier => 1.5;
	public string DragAddendum { get; init; } = string.Empty;
	public string DragVerb1stPerson { get; init; } = "drag";
	public string DragVerb3rdPerson { get; init; } = "drags";
	public string DragVerbGerund { get; init; } = "dragging";
	public Dragging DragEffect { get; set; }

	public ICharacter Dragger { get; set; }

	public ICharacter Mover => Dragger;

	private readonly List<ICharacter> _helpers = new();
	public IEnumerable<ICharacter> Helpers => _helpers;

	public IEnumerable<ICharacter> Draggers => Helpers.Concat(new[] { Dragger });

	public IPerceivable Target { get; set; }

	public override IEnumerable<ICharacter> CharacterMovers
	{
		get
		{
			if (Target is ICharacter tChar)
			{
				return Helpers.Concat(new[] { Dragger, tChar });
			}

			return Helpers.Concat(new[] { Dragger });
		}
	}

	public DragMovement(ICharacter dragger, IEnumerable<ICharacter> helpers, IPerceivable target, Dragging effect,
		ICellExit exit, TimeSpan duration) : base(exit, duration)
	{
		Dragger = dragger;
		_helpers.AddRange(helpers);
		Target = target;
		DragEffect = effect;
		Dragger.Movement = this;
		foreach (var helper in _helpers)
		{
			helper.Movement = this;
		}

		if (Target is IMove move)
		{
			move.Movement = this;
		}

		Exit.Origin.RegisterMovement(this);
	}

	public override bool IsMovementLeader(ICharacter character)
	{
		return character == Dragger;
	}


	public override bool IsConsensualMover(ICharacter character)
	{
		return character != Target;
	}

	public override void StopMovement()
	{
		var movers = CharacterMovers.ToList();

		Dragger.OutputHandler.Handle(
			movers.Count == 1
				? new EmoteOutput(new Emote("@ stop|stops moving.", Dragger))
				: new EmoteOutput(new Emote("@ call|calls the group to a halt.", Dragger)));

		TurnaroundTracks();

		foreach (var mover in movers)
		{
			mover.HandleEvent(EventType.CharacterStopMovement, mover, Exit.Origin, Exit);
			foreach (var witness in mover.Location.EventHandlers.Except(mover))
			{
				witness.HandleEvent(EventType.CharacterStopMovementWitness, mover, Exit.Origin, Exit, witness);
			}

			foreach (var witness in mover.Body.ExternalItems)
			{
				witness.HandleEvent(EventType.CharacterStopMovementWitness, mover, Exit.Origin, Exit, witness);
			}
		}

		Cancel();
	}

	public override MovementType MovementType => Mover.CurrentSpeed.Position switch
	{
		PositionStanding => MovementType.Upright,
		PositionClimbing => MovementType.Climbing,
		PositionSwimming => MovementType.Swimming,
		PositionProne => MovementType.Crawling,
		PositionProstrate => MovementType.Prostrate,
		PositionFlying => MovementType.Flying,
		_ => MovementType.Upright
	};

	public override string Describe(IPerceiver voyeur)
	{
		var draggers = Helpers.Concat(new[] { Dragger }).Where(x => voyeur.CanSee(x)).ToList();
		switch (Phase)
		{
			case MovementPhase.OriginalRoom:
				return
					$"{draggers.Select(x => x.HowSeen(voyeur, false, DescriptionType.Short, false)).ListToString().Proper()} {(draggers.Count == 1 && !draggers.Contains(voyeur) ? "is" : "are")} {DragVerbGerund} {Target.HowSeen(voyeur, colour: false)} {Exit.OutboundMovementSuffix}.";
			case MovementPhase.NewRoom:
				return
					$"{draggers.Select(x => x.HowSeen(voyeur, false, DescriptionType.Short, false)).ListToString().Proper()} {(draggers.Count == 1 && !draggers.Contains(voyeur) ? "is" : "are")} {DragVerbGerund} {Target.HowSeen(voyeur, colour: false)} {Exit.InboundMovementSuffix}.";
			default:
				return "GroupMovement.Describe Error";
		}
	}

	public override bool SeenBy(IPerceiver voyeur, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return CharacterMovers.Any(x => voyeur.CanSee(x, flags));
	}

	public void IntermediateStep()
	{
		if (Cancelled)
		{
			return;
		}

		if (!Dragger.CanMove(Exit))
		{
			TurnaroundTracks();
			Dragger.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"@ stop|stops {DragVerbGerund} $1{DragAddendum} because #0 can't move.", Dragger,
						Dragger, Target), flags: OutputFlags.SuppressObscured));
			Cancel();
			return;
		}

		var (canCross, whyNot) = Dragger.CanCross(Exit);
		if (!canCross)
		{
			TurnaroundTracks();
			Dragger.OutputHandler.Handle(whyNot);
			Cancel();
			return;
		}

		var nonMovers = Helpers.Where(x => !x.CanMove(Exit, true) || !x.CanCross(Exit).Success).ToList();
		foreach (var person in nonMovers)
		{
			TurnaroundTrack(person);
			person.OutputHandler.Handle(person.CanCross(Exit).FailureOutput);
			person.StopMovement(this);
			person.Movement = null;
			_helpers.Remove(person);
			DragEffect.RemoveHelper(person);
		}

		var dragCapacity = DragEffect.Draggers.Sum(x =>
			(x.Character.MaximumDragWeight - x.Character.Body.ExternalItems.Sum(y => y.Weight)) *
			(x.Aid?.EffortMultiplier ?? 1.0));
		var weightThing = (IHaveWeight)Target;
		if (dragCapacity < weightThing.Weight)
		{
			TurnaroundTracks();
			Dragger.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@{(Helpers.Any() ? " and &0's helpers are" : " are|is")} unable to {DragVerb1stPerson} $1{DragAddendum} {Exit.OutboundMovementSuffix} as #1 is too heavy.",
				Dragger, Dragger, Target)));
			Cancel();
			return;
		}

		foreach (var mover in Draggers)
		{
			mover.ExecuteMove(this);
		}

		if (Target is ICharacter targetCharacter)
		{
			Exit.Origin.Leave(targetCharacter);
			targetCharacter.RoomLayer = Draggers.First().RoomLayer;
			Exit.Destination.Enter(targetCharacter);
		}
		else if (Target is IGameItem targetItem)
		{
			Exit.Origin.Extract(targetItem);
			targetItem.RoomLayer = Draggers.First().RoomLayer;
			Exit.Destination.Insert(targetItem);
		}

		Exit.Origin.ResolveMovement(this);
		Exit.Destination.RegisterMovement(this);
		Phase = MovementPhase.NewRoom;

		foreach (var mover in CharacterMovers)
		{
			CreateArrivalTracks(mover, mover == Target ? TrackCircumstances.Dragged : TrackCircumstances.None);
		}

		foreach (var ch in Exit.Destination.Characters.Except(CharacterMovers).Where(x => SeenBy(x)).ToList())
		{
			var members = Draggers.Where(x => ch.CanSee(x)).Select(x => x.HowSeen(ch)).ToList();
			ch.OutputHandler.Handle(
				new EmoteOutput(new Emote(
					$"{members.ListToString()} {(members.Count == 1 ? DragVerb3rdPerson : DragVerb1stPerson)} $0{DragAddendum} {Exit.InboundMovementSuffix}.",
					Dragger, Target)), OutputRange.Personal);
		}

		foreach (var ch in CharacterMovers)
		{
			ch.Body.Look(true);
		}

		var finalDelay = TimeSpan.FromTicks(Duration.Ticks / 5);
		if (TimeSpan.Zero.CompareTo(finalDelay) < 0)
		{
			Exit.Origin.Gameworld.Scheduler.AddSchedule(new Schedule(FinalStep, ScheduleType.Movement,
				finalDelay.TotalSeconds > 5 ? TimeSpan.FromSeconds(5) : finalDelay, "DragMovementFinalStep"));
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

		foreach (var mover in CharacterMovers)
		{
			mover.Movement = null;
		}

		if (Target is IMove move)
		{
			move.Movement = null;
		}

		Exit.Destination.ResolveMovement(this);
		if (Dragger.QueuedMoveCommands.Count > 0)
		{
			Dragger.Move(Dragger.QueuedMoveCommands.Dequeue());
		}
	}

	public override void InitialAction()
	{
		foreach (var dragger in Draggers)
		{
			dragger.StartMove(this);
		}

		(Target as IMove)?.StartMove(this);
		var draggers = Helpers.Concat(new[] { Dragger }).ToList();
		foreach (var ch in Exit.Origin.Characters.Except(draggers).Where(x => SeenBy(x)))
		{
			var members = draggers.Where(x => ch.CanSee(x)).Select(x => x.HowSeen(ch))
			                      .DefaultIfEmpty("Someone".ColourCharacter()).ToList();
			ch.OutputHandler.Send(
				$"{members.ListToString()} {(members.Count <= 1 ? "begins" : "begin")} to {DragVerb1stPerson} {Target.HowSeen(ch)}{DragAddendum} {Exit.OutboundMovementSuffix}.");
		}

		foreach (var ch in Helpers)
		{
			ch.OutputHandler.Send(
				$"You{(Helpers.Count() > 1 ? " and your fellow helpers" : "")} begin to assist {Dragger.HowSeen(ch)} with {DragVerbGerund} {Target.HowSeen(ch)}{DragAddendum} {Exit.OutboundMovementSuffix}.");
		}

		Dragger.OutputHandler.Send(
			$"You{(Helpers.Any() ? " and your helpers" : "")} begin to {DragVerb1stPerson} {Target.HowSeen(Dragger)}{DragAddendum} {Exit.OutboundMovementSuffix}.");

		foreach (var mover in CharacterMovers)
		{
			CreateDepartureTracks(mover, mover == Target ? TrackCircumstances.Dragged : TrackCircumstances.None);
		}

		if (TimeSpan.Zero.CompareTo(Duration) < 0)
		{
			Exit.Origin.Gameworld.Scheduler.AddSchedule(new Schedule(IntermediateStep, ScheduleType.Movement,
				Duration, "DragMovement Intermediate Step"));
		}
		else
		{
			IntermediateStep();
		}
	}

	public override bool Cancel()
	{
		Cancelled = true;
		foreach (var mover in CharacterMovers)
		{
			mover.StopMovement(this);
			mover.Movement = null;
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

	public override bool CancelForMoverOnly(IMove mover)
	{
		return Cancel();
	}
}