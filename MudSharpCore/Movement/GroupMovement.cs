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
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Movement;

public class GroupMovement : MovementBase
{
	public GroupMovement(ICellExit exit, IParty movers, TimeSpan duration)
		: base(exit, duration)
	{
		Party = movers;
		Party.Movement = this;
		// todo - consider this next step handled in the Party.Movement setter
		_characterMovers = Party.ActiveCharacterMembers.ToList();
		foreach (var mover in _characterMovers)
		{
			mover.Movement = this;
		}

		Exit.Origin.RegisterMovement(this);
		Duration = duration;
	}

	protected IParty Party { get; set; }

	public void RemoveMover(ICharacter mover)
	{
		if (mover == null)
		{
			return;
		}

		if (_characterMovers.Contains(mover))
		{
			_characterMovers.Remove(mover);
		}
	}

	public void IntermediateStep()
	{
		if (Party.Leader == null)
		{
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

		if (!Cancelled && CharacterMovers.All(x => x.CanMove(Exit)))
		{
			var (canCross, whyNot) = Party.Leader.CanCross(Exit);
			if (!canCross)
			{
				Party.Leader.OutputHandler.Handle(whyNot);

				foreach (var ch in CharacterMovers.Except(Party.Leader))
				{
					ch.Send("You stop moving because your leader has stopped.");
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

			var nonMovers = CharacterMovers.Where(x => !x.CanCross(Exit).Success).ToList();
			if (nonMovers.Any())
			{
				foreach (var nonMover in nonMovers)
				{
					nonMover.OutputHandler.Handle(nonMover.CanCross(Exit).FailureOutput);
					_characterMovers.Remove(nonMover);
					nonMover.StopMovement(this);
					nonMover.Movement = null;
				}

				if (!_characterMovers.Any())
				{
					Cancel();
					return;
				}
			}

			var partyMoveSpeed = Party.SlowestSpeed(Exit);
			foreach (var mover in CharacterMovers.ToList())
			{
				mover.ExecuteMove(partyMoveSpeed);
			}

			Exit.Origin.ResolveMovement(this);
			Exit.Destination.RegisterMovement(this);
			Phase = MovementPhase.NewRoom;
			var partyHiders = CharacterMovers.Where(x => x.AffectedBy<IHideEffect>()).ToList();

			foreach (var ch in Exit.Destination.Characters.Except(CharacterMovers).Where(x => SeenBy(x)))
			{
				if (partyHiders.Any())
				{
					foreach (var hider in partyHiders)
					{
						ch.AddEffect(new SawHider(ch, hider), TimeSpan.FromSeconds(300));
					}
				}

				var members = CharacterMovers.Where(x => ch.CanSee(x)).ToList();
				ch.OutputHandler.Send(members.Select(x => x.HowSeen(ch)).ListToString() + " " +
				                      (members.Count == 1
					                      ? members.Select(x => x.CurrentSpeed)
					                               .Distinct()
					                               .Select(x => x.ThirdPersonVerb)
					                               .ListToString()
					                      : members.Select(x => x.CurrentSpeed)
					                               .Distinct()
					                               .Select(x => x.FirstPersonVerb)
					                               .ListToString())
				                      + " " + Exit.InboundMovementSuffix + ".");
			}

			foreach (var mover in CharacterMovers)
			{
				mover?.Body.Look(true);
			}

			var finalDelay = TimeSpan.FromTicks(Duration.Ticks / 5);
			if (TimeSpan.Zero.CompareTo(finalDelay) < 0)
			{
				Exit.Origin.Gameworld.Scheduler.AddSchedule(new Schedule(FinalStep, ScheduleType.Movement,
					finalDelay.TotalSeconds > 5 ? TimeSpan.FromSeconds(5) : finalDelay, "GroupMovement Final Step"));
			}
			else
			{
				FinalStep();
			}
		}
		else if (!Cancelled)
		{
			var nonmovers = CharacterMovers.Where(x => !x.CanMove(Exit)).ToList();
			var count = 0;
			var output =
				new EmoteOutput(
					new Emote(
						"The party stops as " + nonmovers.Select(x => "$" + count++).ListToString() +
						" cannot move.", Party.Leader, nonmovers.ToArray()));
			foreach (var person in CharacterMovers)
			{
				person.OutputHandler.Handle(output, OutputRange.Personal);
			}

			Cancel();
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

		Party.Movement = null;
		Exit.Destination.ResolveMovement(this);
		if (Party.QueuedMoveCommands.Count > 0)
		{
			Party.Move(Party.QueuedMoveCommands.Dequeue());
		}
	}

	public override void InitialAction()
	{
		Party.StartMove(this);
		// TODO - this output needs to be reviewed

		foreach (var ch in Exit.Origin.Characters.Except(CharacterMovers).Where(x => SeenBy(x)))
		{
			var movers = CharacterMovers.Where(x => ch.CanSee(x));
			var members = movers.Select(x => x.HowSeen(ch)).ToList();
			var moveString = movers.Select(x => x.CurrentSpeed).Distinct()
			                       .Select(y => y.PresentParticiple)
			                       .ListToString();
			ch.OutputHandler.Send(members.ListToString() + (members.Count == 1 ? " begins " : " begin ") +
			                      moveString + " " + Exit.OutboundMovementSuffix + ".");
		}

		foreach (var ch in CharacterMovers)
		{
			var moveString = CharacterMovers.Select(x => x.CurrentSpeed).Distinct()
			                                .Select(y => y.PresentParticiple)
			                                .ListToString();
			ch.OutputHandler.Send("You and your party begin " + moveString + " " +
			                      Exit.OutboundMovementSuffix + ".");
		}

		if (TimeSpan.Zero.CompareTo(Duration) < 0)
		{
			Exit.Origin.Gameworld.Scheduler.AddSchedule(new Schedule(IntermediateStep, ScheduleType.Movement,
				Duration, "GroupMovement Intermediate Step"));
		}
		else
		{
			IntermediateStep();
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		var members = CharacterMovers.Except(voyeur).Where(x => voyeur.CanSee(x)).OfType<ICharacter>().ToList();
		switch (Phase)
		{
			case MovementPhase.OriginalRoom:
				return
					members.Select(x => x.HowSeen(voyeur, false, DescriptionType.Short, false))
					       .ListToString()
					       .Proper() + (members.Count == 1 ? " is " : " are ") +
					members.Select(x => x.CurrentSpeed).Distinct().Select(x => x?.PresentParticiple ?? "moving")
					       .ListToString() +
					" " + Exit.OutboundMovementSuffix + ".";
			case MovementPhase.NewRoom:
				return
					members.Select(x => x.HowSeen(voyeur, false, DescriptionType.Short, false))
					       .ListToString()
					       .Proper() + (members.Count == 1 ? " is " : " are ") +
					members.Select(x => x.CurrentSpeed).Distinct().Select(x => x?.PresentParticiple ?? "moving")
					       .ListToString() +
					" " + Exit.InboundMovementSuffix + ".";
			default:
				return "GroupMovement.Describe Error";
		}
	}

	public override bool SeenBy(IPerceiver voyeur, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return CharacterMovers.Except(voyeur).Any(x => voyeur.CanSee(x, flags));
	}

	#region IMovement Members

	public override void StopMovement()
	{
		Party.Leader.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ call|calls the group to a halt.", Party.Leader)));

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

	public override bool IsMovementLeader(ICharacter character)
	{
		return Party.Leader == character;
	}

	private readonly List<ICharacter> _characterMovers;

	public override IEnumerable<ICharacter> CharacterMovers => _characterMovers;

	public override bool Cancel()
	{
		Cancelled = true;
		foreach (var mover in CharacterMovers)
		{
			mover.StopMovement(this);
			mover.Movement = null;
		}

		Party.Movement = null;
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
		if (mover == Party.Leader || !(mover is ICharacter ch))
		{
			return Cancel();
		}

		RemoveMover(ch);
		ch.Movement = null;
		return true;
	}

	#endregion
}