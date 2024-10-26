using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Movement;

public class SingleMovement : MovementBase
{
	public SingleMovement(ICellExit exit, ICharacter mover, TimeSpan duration, IEmote playerEmote = null)
		: base(exit, duration)
	{
		Mover = mover;
		Mover.Movement = this;
		MoverEmote = playerEmote;
		Exit.Origin.RegisterMovement(this);
		Duration = duration;
	}

	/// <summary>
	///     This constructor is used when a GroupMovement becomes a SingleMovement when the party is disbanded.
	/// </summary>
	/// <param name="movement"></param>
	public SingleMovement(GroupMovement movement, ICharacter mover)
		: base(movement.Exit, movement.Duration)
	{
		Mover = mover;
		Mover.Movement = this;
		Exit.Origin.RegisterMovement(this);
		IntermediateStep();
	}

	protected ICharacter Mover { get; set; }
	protected IEmote MoverEmote { get; set; }

	protected virtual MixedEmoteOutput GetMovementOutput
		=>
			new(
				new Emote("@ begin|begins " + Mover.CurrentSpeed.PresentParticiple + " " + Exit.OutboundMovementSuffix,
					Mover), flags: OutputFlags.SuppressObscured);

	public virtual void IntermediateStep()
	{
		if (Cancelled)
		{
			return;
		}

		if (Mover.CanMove(Exit))
		{
			var (canCross, whyNot) = Mover.CanCross(Exit);
			if (!canCross)
			{
				TurnaroundTracks();
				Mover.OutputHandler.Handle(whyNot);
				Mover.HandleEvent(EventType.CharacterStopMovementClosedDoor, Mover, Exit.Origin, Exit);
				foreach (var witness in Mover.Location.EventHandlers.Except(Mover))
				{
					witness.HandleEvent(EventType.CharacterStopMovementClosedDoorWitness, Mover, Exit.Origin, Exit,
						witness);
				}

				foreach (var witness in Mover.Body.ExternalItems)
				{
					witness.HandleEvent(EventType.CharacterStopMovementClosedDoorWitness, Mover, Exit.Origin, Exit,
						witness);
				}

				Cancel();
				return;
			}

			Mover.ExecuteMove(this);
			// Mover.ExecuteMove() can cause the movement to become cancelled if someone tumbles off a cliff for example
			if (Cancelled)
			{
				return;
			}

			Phase = MovementPhase.NewRoom;
			Exit.Destination.RegisterMovement(this);
			Exit.Origin.ResolveMovement(this);
			HandleMovementEcho();

			CreateArrivalTracks(Mover, MovementTrackCircumstances);

			// If they have the hide effect but are not sneaking, everyone sees them by default
			if (Mover.AffectedBy<IHideEffect>())
			{
				foreach (var witness in Mover.Location.Characters.Except(Mover))
				{
					witness.AddEffect(new SawHider(witness, Mover), TimeSpan.FromSeconds(300));
				}
			}

			Mover.Body.Look(true);

			var finalDelay = TimeSpan.FromTicks(Duration.Ticks / 5);
			if (TimeSpan.Zero.CompareTo(finalDelay) < 0)
			{
				Mover.Gameworld.Scheduler.AddSchedule(new Schedule(FinalStep, ScheduleType.Movement,
					finalDelay.TotalSeconds > 5 ? TimeSpan.FromSeconds(5) : finalDelay,
					"SingleMovement Final Step"));
			}
			else
			{
				FinalStep();
			}

			return;
		}

		TurnaroundTracks();
		Mover.Send(Mover.WhyCannotMove());
		Cancel();
	}

	protected virtual void HandleMovementEcho()
	{
		Mover.OutputHandler.Handle(
			new MixedEmoteOutput(
				new Emote(
					$"@ {Mover.CurrentSpeed.FirstPersonVerb}|{Mover.CurrentSpeed.ThirdPersonVerb} {Exit.InboundMovementSuffix}",
					Mover),
				flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource).Append(MoverEmote));
	}

	public virtual void FinalStep()
	{
		if (Cancelled)
		{
			return;
		}

		Exit.Destination.ResolveMovement(this);
		if (Mover.Movement == this)
		{
			Mover.Movement = null;
			if (Mover.QueuedMoveCommands.Count > 0)
			{
				Mover.Move(Mover.QueuedMoveCommands.Dequeue());
			}
		}
	}

	protected virtual TrackCircumstances MovementTrackCircumstances => TrackCircumstances.None;

	public override void InitialAction()
	{
		Mover.StartMove(this);
		var output = GetMovementOutput.Append(MoverEmote);
		foreach (var character in Mover.Location.Characters.Where(x => SeenBy(x)))
		{
			character.OutputHandler.Send(output);
		}

		CreateDepartureTracks(Mover, MovementTrackCircumstances);

		if (TimeSpan.Zero.CompareTo(Duration) < 0)
		{
			Mover.Gameworld.Scheduler.AddSchedule(new Schedule(IntermediateStep, ScheduleType.Movement, Duration,
				"SingleMovement Intermediate Step"));
		}
		else
		{
			IntermediateStep();
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		switch (Phase)
		{
			case MovementPhase.OriginalRoom:
				return Mover.HowSeen(voyeur, true, DescriptionType.Short, false) + " is " +
				       Mover.CurrentSpeed.PresentParticiple + " " + Exit.OutboundMovementSuffix + ".";
			case MovementPhase.NewRoom:
				return Mover.HowSeen(voyeur, true, DescriptionType.Short, false) + " is " +
				       Mover.CurrentSpeed.PresentParticiple + " " + Exit.InboundMovementSuffix + ".";
			default:
				return "SingleMovement.Describe Error";
		}
	}

	public override bool SeenBy(IPerceiver voyeur, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return voyeur.CanSee(Mover, flags);
	}

	#region IMovement Members

	public override void StopMovement()
	{
		Mover.OutputHandler.Handle(new EmoteOutput(new Emote("@ stop|stops moving.", Mover)));
		Mover.HandleEvent(EventType.CharacterStopMovement, Mover, Exit.Origin, Exit);
		foreach (var witness in Mover.Location.EventHandlers.Except(Mover))
		{
			witness.HandleEvent(EventType.CharacterStopMovementWitness, Mover, Exit.Origin, Exit, witness);
		}

		foreach (var witness in Mover.Body.ExternalItems)
		{
			witness.HandleEvent(EventType.CharacterStopMovementWitness, Mover, Exit.Origin, Exit, witness);
		}

		TurnaroundTracks();
		Cancel();
	}

	public override bool IsMovementLeader(ICharacter character)
	{
		return character == Mover;
	}

	public override IEnumerable<ICharacter> CharacterMovers => new[] { Mover };

	public override bool Cancel()
	{
		Cancelled = true;
		Mover.StopMovement(this);
		Mover.Movement = null;
		Mover.QueuedMoveCommands.Clear();
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

	#endregion
}