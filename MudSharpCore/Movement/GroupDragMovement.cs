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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Movement;

public class GroupDragMovement : MovementBase
{
	public override double StaminaMultiplier => 2.0;
	protected IParty Party { get; set; }
	public List<Dragging> DragEffects { get; } = new();
	public List<ICharacter> Draggers { get; } = new();
	public List<ICharacter> Helpers { get; } = new();
	public List<ICharacter> NonDraggers { get; } = new();
	public List<IPerceivable> Targets { get; } = new();

	private readonly List<ICharacter> _characterMovers = new();

	public override IEnumerable<ICharacter> CharacterMovers => _characterMovers;

	public GroupDragMovement(IParty party,
		IEnumerable<(Dragging Effect, ICharacter Dragger, IEnumerable<ICharacter> Helpers, IPerceivable Target)>
			draggings, ICellExit exit, TimeSpan duration) : base(exit, duration)
	{
		Party = party;
		Party.Movement = this;
		foreach (var dragging in draggings)
		{
			DragEffects.Add(dragging.Effect);
			Draggers.Add(dragging.Dragger);
			_characterMovers.Add(dragging.Dragger);
			Helpers.AddRange(dragging.Helpers);
			_characterMovers.AddRange(dragging.Helpers);
			Targets.Add(dragging.Target);
			if (dragging.Target is ICharacter tch)
			{
				_characterMovers.Add(tch);
			}
		}

		foreach (var ch in Party.CharacterMembers
			.Where(x =>
				x.InRoomLocation == Party.Leader.InRoomLocation &&
				x.Movement is null &&
				x.CanMove())
		)
		{
			if (!_characterMovers.Contains(ch))
			{
				_characterMovers.Add(ch);
			}
		}

		foreach (var mover in _characterMovers)
		{
			mover.Movement?.CancelForMoverOnly(mover);
			mover.Movement = this;
		}

		Exit.Origin.RegisterMovement(this);
	}

	public void RemoveMover(ICharacter mover)
	{
		if (mover == null)
		{
			return;
		}

		_characterMovers.Remove(mover);
		Draggers.Remove(mover);
		Helpers.Remove(mover);
		Targets.Remove(mover);
	}

	public override bool IsMovementLeader(ICharacter character)
	{
		return Party.Leader == character;
	}

	public override bool IsConsensualMover(ICharacter character)
	{
		return Targets.All(x => x != character);
	}

	public override void StopMovement()
	{
		Party.Leader.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ call|calls the group to a halt.", Party.Leader)));
		TurnaroundTracks();
		foreach (var mover in CharacterMovers)
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

	public string DescribeBeginMove(IPerceiver voyeur)
	{
		var nonDraggers = NonDraggers.Where(x => voyeur.CanSee(x)).ToList();
		var subgroupStrings = new List<string>();
		foreach (var effect in DragEffects)
		{
			var draggers = Draggers.Concat(Helpers).Where(x => effect.CharacterDraggers.Contains(x) && voyeur.CanSee(x))
			                       .ToList();
			if (!draggers.Any() && !voyeur.CanSee(effect.Target))
			{
				continue;
			}

			subgroupStrings.Add(
				$"{draggers.Select(x => x.HowSeen(voyeur)).DefaultIfEmpty("Someone".ColourCharacter()).ListToString()} {(draggers.Count <= 1 ? "begins" : "begins")} dragging {effect.Target.HowSeen(voyeur)}");
		}

		if (nonDraggers.Any())
		{
			if (subgroupStrings.Any())
			{
				if (Phase == MovementPhase.OriginalRoom)
				{
					return
						$"{subgroupStrings.ListToString()} {Exit.OutboundMovementSuffix}, followed by {nonDraggers.Select(x => x.HowSeen(voyeur)).ListToString()}."
							.Proper();
				}

				return
					$"{subgroupStrings.ListToString()} {Exit.InboundMovementSuffix}, followed by {nonDraggers.Select(x => x.HowSeen(voyeur)).ListToString()}."
						.Proper();
			}

			if (Phase == MovementPhase.OriginalRoom)
			{
				return
					$"{nonDraggers.Select(x => x.HowSeen(voyeur, false, DescriptionType.Short, false)).ListToString().Proper()} {(nonDraggers.Count == 1 ? "begins" : "begin")} {nonDraggers.Select(x => x.CurrentSpeed).Distinct().Select(x => x?.PresentParticiple ?? "moving").ListToString()} {Exit.OutboundMovementSuffix}."
						.Proper();
			}

			return
				$"{nonDraggers.Select(x => x.HowSeen(voyeur, false, DescriptionType.Short, false)).ListToString().Proper()} {(nonDraggers.Count == 1 ? "begins" : "begin")} {nonDraggers.Select(x => x.CurrentSpeed).Distinct().Select(x => x?.PresentParticiple ?? "moving").ListToString()} {Exit.InboundMovementSuffix}."
					.Proper();
		}

		if (Phase == MovementPhase.OriginalRoom)
		{
			return $"{subgroupStrings.ListToString()} {Exit.OutboundMovementSuffix}.".Proper();
		}

		return $"{subgroupStrings.ListToString()} {Exit.InboundMovementSuffix}.".Proper();
	}

	public string DescribeEntry(IPerceiver voyeur)
	{
		var nonDraggers = NonDraggers.Where(x => voyeur.CanSee(x)).ToList();
		var subgroupStrings = new List<string>();
		foreach (var effect in DragEffects)
		{
			var draggers = Draggers.Concat(Helpers).Where(x => effect.CharacterDraggers.Contains(x) && voyeur.CanSee(x))
			                       .ToList();
			if (!draggers.Any())
			{
				continue;
			}

			subgroupStrings.Add(
				$"{draggers.Select(x => x.HowSeen(voyeur)).ListToString()} {(draggers.Count == 1 ? "drag" : "drags")} {effect.Target.HowSeen(voyeur)}");
		}

		if (nonDraggers.Any())
		{
			if (subgroupStrings.Any())
			{
				if (Phase == MovementPhase.OriginalRoom)
				{
					return
						$"{subgroupStrings.ListToString()} {Exit.OutboundMovementSuffix}, followed by {nonDraggers.Select(x => x.HowSeen(voyeur)).ListToString()}."
							.Proper();
				}

				return
					$"{subgroupStrings.ListToString()} {Exit.InboundMovementSuffix}, followed by {nonDraggers.Select(x => x.HowSeen(voyeur)).ListToString()}."
						.Proper();
			}

			if (Phase == MovementPhase.OriginalRoom)
			{
				return
					$"{nonDraggers.Select(x => x.HowSeen(voyeur, false, DescriptionType.Short, false)).ListToString().Proper()} {(nonDraggers.Count == 1 ? "drag" : "drags")} {nonDraggers.Select(x => x.CurrentSpeed).Distinct().Select(x => x?.PresentParticiple ?? "moving").ListToString()} {Exit.OutboundMovementSuffix}."
						.Proper();
			}

			return
				$"{nonDraggers.Select(x => x.HowSeen(voyeur, false, DescriptionType.Short, false)).ListToString().Proper()} {(nonDraggers.Count == 1 ? "drag" : "drags")} {nonDraggers.Select(x => x.CurrentSpeed).Distinct().Select(x => x?.PresentParticiple ?? "moving").ListToString()} {Exit.InboundMovementSuffix}."
					.Proper();
		}

		if (Phase == MovementPhase.OriginalRoom)
		{
			return $"{subgroupStrings.ListToString()} {Exit.OutboundMovementSuffix}.".Proper();
		}

		return $"{subgroupStrings.ListToString()} {Exit.InboundMovementSuffix}.".Proper();
	}

	public override string Describe(IPerceiver voyeur)
	{
		var nonDraggers = NonDraggers.Where(x => voyeur.CanSee(x)).ToList();
		var subgroupStrings = new List<string>();
		foreach (var effect in DragEffects)
		{
			var draggers = Draggers.Concat(Helpers).Where(x => effect.CharacterDraggers.Contains(x) && voyeur.CanSee(x))
			                       .ToList();
			if (!draggers.Any())
			{
				continue;
			}

			subgroupStrings.Add(
				$"{draggers.Select(x => x.HowSeen(voyeur)).ListToString()} {(draggers.Count == 1 ? "is" : "are")} dragging {effect.Target.HowSeen(voyeur)}");
		}

		if (nonDraggers.Any())
		{
			if (subgroupStrings.Any())
			{
				if (Phase == MovementPhase.OriginalRoom)
				{
					return
						$"{subgroupStrings.ListToString()} {Exit.OutboundMovementSuffix}, followed by {nonDraggers.Select(x => x.HowSeen(voyeur)).ListToString()}."
							.Proper();
				}

				return
					$"{subgroupStrings.ListToString()} {Exit.InboundMovementSuffix}, followed by {nonDraggers.Select(x => x.HowSeen(voyeur)).ListToString()}."
						.Proper();
			}

			if (Phase == MovementPhase.OriginalRoom)
			{
				return
					$"{nonDraggers.Select(x => x.HowSeen(voyeur, false, DescriptionType.Short, false)).ListToString().Proper()} {(nonDraggers.Count == 1 ? "is" : "are")} {nonDraggers.Select(x => x.CurrentSpeed).Distinct().Select(x => x?.PresentParticiple ?? "moving").ListToString()} {Exit.OutboundMovementSuffix}."
						.Proper();
			}

			return
				$"{nonDraggers.Select(x => x.HowSeen(voyeur, false, DescriptionType.Short, false)).ListToString().Proper()} {(nonDraggers.Count == 1 ? "is" : "are")} {nonDraggers.Select(x => x.CurrentSpeed).Distinct().Select(x => x?.PresentParticiple ?? "moving").ListToString()} {Exit.InboundMovementSuffix}."
					.Proper();
		}

		if (Phase == MovementPhase.OriginalRoom)
		{
			return $"{subgroupStrings.ListToString()} {Exit.OutboundMovementSuffix}.".Proper();
		}

		return $"{subgroupStrings.ListToString()} {Exit.InboundMovementSuffix}.".Proper();
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

		if (Party.Leader == null)
		{
			TurnaroundTracks();
			foreach (var ch in CharacterMovers)
			{
				ch.Send("You stop moving because your leader has disappeared.");
			}

			foreach (var mover in CharacterMovers)
			{
				mover.HandleEvent(EventType.CharacterStopMovementClosedDoor, mover, Exit.Origin, Exit);
				foreach (var witness in mover.Location.EventHandlers.Except(mover))
				{
					witness.HandleEvent(EventType.CharacterStopMovementClosedDoorWitness, mover, Exit.Origin,
						Exit, witness);
				}

				foreach (var witness in mover.Body.ExternalItems)
				{
					witness.HandleEvent(EventType.CharacterStopMovementClosedDoorWitness, mover, Exit.Origin,
						Exit, witness);
				}
			}

			Cancel();
			return;
		}

		if (!CharacterMovers.All(x => x.CanMove(Exit)))
		{
			TurnaroundTracks();
			var nonmovers = CharacterMovers.Where(x => !x.CanMove(Exit)).ToList();
			var count = 0;
			var output =
				new EmoteOutput(
					new Emote(
						"The party stops as " + nonmovers.Select(x => "$" + count++).ListToString() +
						" cannot move.", Party.Leader, nonmovers.ToArray()), flags: OutputFlags.InnerWrap);
			foreach (var person in CharacterMovers)
			{
				person.OutputHandler.Handle(output, OutputRange.Personal);
			}

			Cancel();
			return;
		}

		if (!Party.Leader.CanMove(Exit))
		{
			TurnaroundTracks();
			Party.Leader.OutputHandler.Handle(
				new EmoteOutput(
					new Emote("@ stop|stops &0's group because #0 can't move.", Party.Leader),
					flags: OutputFlags.SuppressObscured | OutputFlags.InnerWrap));
			Cancel();
			return;
		}

		var (canCross, whyNot) = Party.Leader.CanCross(Exit);
		if (!canCross)
		{
			TurnaroundTracks();
			Party.Leader.OutputHandler.Handle(whyNot);
			Cancel();
			return;
		}

		foreach (var effect in DragEffects)
		{
			var dragLeader = Draggers.First(x => effect.CharacterDraggers.Contains(x));
			var target = effect.Target;
			var draggers = effect.CharacterDraggers.Except(dragLeader).ToList();
			if (!dragLeader.CanMove(Exit))
			{
				TurnaroundTracks();
				dragLeader.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ stop|stops dragging $1 because #0 can't move.", dragLeader, dragLeader, target),
						flags: OutputFlags.SuppressObscured | OutputFlags.InnerWrap));
				Cancel();
				return;
			}

			(canCross, whyNot) = dragLeader.CanCross(Exit);
			if (!canCross)
			{
				TurnaroundTracks();
				dragLeader.OutputHandler.Handle(whyNot);
				Cancel();
				return;
			}

			var nonMovingHelpers = draggers.Where(x => !x.CanMove(Exit, true) || !x.CanCross(Exit).Success).ToList();
			foreach (var person in nonMovingHelpers)
			{
				person.OutputHandler.Handle(person.CanCross(Exit).FailureOutput);
				person.StopMovement(this);
				person.Movement = null;
				Helpers.Remove(person);
				_characterMovers.Remove(person);
				effect.RemoveHelper(person);
				TurnaroundTrack(person);
			}

			var dragCapacity = effect.Draggers.Sum(x =>
				(x.Character.MaximumDragWeight - x.Character.Body.ExternalItems.Sum(y => y.Weight)) *
				(x.Aid?.EffortMultiplier ?? 1.0));
			var weightThing = (IHaveWeight)target;
			if (dragCapacity < weightThing.Weight)
			{
				TurnaroundTracks();
				dragLeader.OutputHandler.Handle(new EmoteOutput(new Emote(
					$"@{(Helpers.Any() ? " and &0's helpers are" : " are|is")} unable to drag $1 {Exit.OutboundMovementSuffix} as #1 is too heavy.",
					dragLeader, dragLeader, target), flags: OutputFlags.InnerWrap));
				Cancel();
				return;
			}
		}

		var nonMovers = NonDraggers.Where(x => !x.CanCross(Exit).Success).ToList();
		if (nonMovers.Any())
		{
			foreach (var nonMover in nonMovers)
			{
				nonMover.OutputHandler.Handle(nonMover.CanCross(Exit).FailureOutput);
				_characterMovers.Remove(nonMover);
				nonMover.StopMovement(this);
				nonMover.Movement = null;
				TurnaroundTrack(nonMover);
			}

			if (!_characterMovers.Any())
			{
				Cancel();
				return;
			}
		}

		foreach (var mover in Draggers)
		{
			mover.ExecuteMove(this);
		}

		foreach (var mover in Helpers)
		{
			mover.ExecuteMove(this);
		}

		foreach (var mover in NonDraggers)
		{
			mover.ExecuteMove(this);
		}

		foreach (var target in Targets)
		{
			if (target is ICharacter targetCharacter)
			{
				Exit.Origin.Leave(targetCharacter);
				targetCharacter.RoomLayer = Draggers.First().RoomLayer;
				Exit.Destination.Enter(targetCharacter);
			}
			else if (target is IGameItem targetItem)
			{
				Exit.Origin.Extract(targetItem);
				targetItem.RoomLayer = Draggers.First().RoomLayer;
				Exit.Destination.Insert(targetItem);
			}
		}

		Exit.Origin.ResolveMovement(this);
		Exit.Destination.RegisterMovement(this);
		Phase = MovementPhase.NewRoom;

		foreach (var mover in CharacterMovers)
		{
			CreateArrivalTracks(mover, DragEffects.Any(x => x.Target == mover) ? TrackCircumstances.Dragged : TrackCircumstances.None);
		}

		foreach (var ch in Exit.Destination.Characters.Except(CharacterMovers).Where(x => SeenBy(x)).ToList())
		{
			ch.OutputHandler.Send(DescribeEntry(ch));
		}

		foreach (var ch in CharacterMovers)
		{
			ch?.Body.Look(true);
		}

		var finalDelay = TimeSpan.FromTicks(Duration.Ticks / 5);
		if (TimeSpan.Zero.CompareTo(finalDelay) < 0)
		{
			Exit.Origin.Gameworld.Scheduler.AddSchedule(new Schedule(FinalStep, ScheduleType.Movement,
				finalDelay.TotalSeconds > 5 ? TimeSpan.FromSeconds(5) : finalDelay, "GroupDragMovement FinalStep"));
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

		foreach (var target in Targets)
		{
			if (target is IMove move)
			{
				move.Movement = null;
			}
		}

		Exit.Destination.ResolveMovement(this);
		if (Party.Leader.QueuedMoveCommands.Count > 0)
		{
			Party.Leader.Move(Party.Leader.QueuedMoveCommands.Dequeue());
		}
	}

	public override void InitialAction()
	{
		foreach (var ch in CharacterMovers)
		{
			ch.StartMove(this);
		}

		var nonDraggers = NonDraggers.ToList();
		var draggers = Draggers.ToList();

		foreach (var ch in Exit.Origin.LayerCharacters(Party.Leader.RoomLayer).Where(x => SeenBy(x)))
		{
			ch.OutputHandler.Send(DescribeBeginMove(ch).Wrap(ch.InnerLineFormatLength));
		}

		foreach (var effect in DragEffects)
		{
			var target = effect.Target;
			var leader = Draggers.First(x => effect.CharacterDraggers.Contains(x));
			var helpers = Helpers.Where(x => effect.CharacterDraggers.Contains(x)).ToList();
			foreach (var ch in helpers)
			{
				ch.OutputHandler.Send(
					$"You and your party{(helpers.Count > 1 ? " and your fellow helpers" : "")} begin to assist {leader.HowSeen(ch)} with dragging {target.HowSeen(ch)} {Exit.OutboundMovementSuffix}.".Wrap(ch.InnerLineFormatLength));
			}

			leader.OutputHandler.Send(
				$"You and your party{(helpers.Any() ? " and your helpers" : "")} begin to drag {target.HowSeen(leader)} {Exit.OutboundMovementSuffix}.".Wrap(leader.InnerLineFormatLength));
		}

		foreach (var ch in nonDraggers)
		{
			ch.OutputHandler.Send(
				$"You and your party begin moving {Exit.OutboundMovementSuffix} along with those dragging {Targets.Select(x => x.HowSeen(ch)).ListToString()}.".Wrap(ch.InnerLineFormatLength));
		}

		foreach (var mover in CharacterMovers)
		{
			CreateDepartureTracks(mover, DragEffects.Any(x => x.Target == mover) ? TrackCircumstances.Dragged : TrackCircumstances.None);
		}

		if (TimeSpan.Zero.CompareTo(Duration) < 0)
		{
			Exit.Origin.Gameworld.Scheduler.AddSchedule(new Schedule(IntermediateStep, ScheduleType.Movement,
				Duration, "GroupDragMovement Intermediate Step"));
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
		if (mover == Party.Leader || Draggers.Contains(mover) || !(mover is ICharacter ch))
		{
			return Cancel();
		}

		RemoveMover(ch);
		ch.Movement = null;
		return true;
	}
}