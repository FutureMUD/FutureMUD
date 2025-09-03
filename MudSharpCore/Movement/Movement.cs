using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using MoreLinq.Extensions;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Movement;

public class Movement : IMovement
{
	#region Implementation of IMovement

	/// <inheritdoc />
	public bool Cancelled { get; private set; }

	/// <inheritdoc />
	public bool CanBeVoluntarilyCancelled => Phase == MovementPhase.OriginalRoom;

	/// <inheritdoc />
	public double StaminaMultiplier { get; }

	/// <inheritdoc />
	public MovementType MovementTypeForMover(ICharacter mover)
	{
		return mover.CurrentSpeed.Position switch
		{
			PositionStanding => MovementType.Upright,
			PositionClimbing => MovementType.Climbing,
			PositionSwimming => MovementType.Swimming,
			PositionProne => MovementType.Crawling,
			PositionProstrate => MovementType.Prostrate,
			PositionFlying => MovementType.Flying,
			_ => MovementType.Upright
		};
	}

	/// <inheritdoc />
	public ICellExit Exit { get; }

	public RoomLayer OriginLayer { get; }

	/// <inheritdoc />
	public MovementPhase Phase { get; private set; }

	/// <inheritdoc />
	public IEnumerable<ICharacter> CharacterMovers { get; }

	public IParty Party { get; }
	public List<Dragging> DragEffects { get; } = new();
	public List<ICharacter> Draggers { get; } = new();
	public List<ICharacter> Helpers { get; } = new();
	public List<ICharacter> NonDraggers { get; } = new();
	public List<ICharacter> NonConsensualMovers { get; } = new();
	public List<ICharacter> Mounts { get; } = new();
	public List<IPerceivable> Targets { get; } = new();
	public DictionaryWithDefault<ICharacter, ISneakMoveEffect> SneakMoveEffects { get; } = new();

	#region IMovement Explicit Implementations
	IEnumerable<IDragging> IMovement.DragEffects => DragEffects;
	IEnumerable<ICharacter> IMovement.Draggers => Draggers;
	IEnumerable<ICharacter> IMovement.Helpers => Helpers;
	IEnumerable<ICharacter> IMovement.NonDraggers => NonDraggers;
	IEnumerable<ICharacter> IMovement.NonConsensualMovers => NonConsensualMovers;
	IEnumerable<ICharacter> IMovement.Mounts => Mounts;
	IEnumerable<IPerceivable> IMovement.Targets => Targets;
	IReadOnlyDictionary<ICharacter, ISneakMoveEffect> IMovement.SneakMoveEffects => SneakMoveEffects.AsReadOnly();
	#endregion

	public TimeSpan Duration { get; }

	public static bool EvaluateCharacterForAdditionToMovement(
		IParty party,
		ICharacter character,
		ICellExit exit,
		List<ICharacter> considered,
		List<ICharacter> nonDraggers,
		List<ICharacter> mounts,
		List<ICharacter> draggers,
		List<ICharacter> helpers,
		List<ICharacter> nonConsensualMovers,
		List<IPerceivable> targets,
		List<Dragging> dragEffects,
		bool ignoreSafeMovement,
		bool ignoreNotLeadDragger
		)
	{
		if (character is null)
		{
			return true;
		}

		var effect = character.EffectsOfType<IDragParticipant>().FirstOrDefault();
		// Early exit if this character is a drag target or drag helper. If the draggers end up as part of the movement, they will add their drag targets in.
		// We exit here so that we don't have opposing draggers being obliged to join a party move just because their drag target is a part of a party
		if (!ignoreNotLeadDragger)
		{
			if (effect is Dragging.DragTarget or Dragging.DragHelper)
			{
				return false;
			}
		}

		// Exclude if we've already considered
		if (considered.Contains(character))
		{
			return true;
		}

		considered.Add(character);

		if (effect is Dragging.DragTarget)
		{
			targets.Add(character);
			nonConsensualMovers.Add(character);
			return true;
		}

		// Exclude all the reasons why this character would not participate in a move
		if (character.Movement is not null)
		{
			return false;
		}

		if (character.Combat is not null && character.IsEngagedInMelee)
		{
			return false;
		}

		if (!ignoreSafeMovement && 
		    character.RidingMount is null && 
		    !character.CanMove(exit, CanMoveFlags.None).Result)
		{
			return false;
		}

		if (character.RidingMount is not null)
		{
			nonConsensualMovers.Add(character);
			EvaluateCharacterForAdditionToMovement(party, character.RidingMount, exit, considered, nonDraggers, mounts, draggers, helpers, nonConsensualMovers, targets, dragEffects, ignoreSafeMovement, ignoreNotLeadDragger);
		}

		if (character.Riders.Any())
		{
			mounts.Add(character);
		}

		foreach (var rider in character.Riders)
		{
			EvaluateCharacterForAdditionToMovement(party, rider, exit, considered, nonDraggers, mounts, draggers, helpers, nonConsensualMovers, targets, dragEffects, ignoreSafeMovement, ignoreNotLeadDragger);
		}

		if (effect is null)
		{
			nonDraggers.Add(character);
			return true;
		}

		switch (effect)
		{
			case Dragging dragging:
				dragEffects.Add(dragging);
				draggers.Add(character);
				foreach (var helper in dragging.Helpers)
				{
					EvaluateCharacterForAdditionToMovement(party, helper, exit, considered, nonDraggers, mounts, draggers, helpers, nonConsensualMovers, targets, dragEffects, ignoreSafeMovement, true);
				}

				EvaluateCharacterForAdditionToMovement(party, dragging.Target as ICharacter, exit, considered, nonDraggers, mounts, draggers, helpers, nonConsensualMovers, targets, dragEffects, ignoreSafeMovement, true);
				if (dragging.Target is IGameItem)
				{
					targets.Add(dragging.Target);
				}
				break;
			case Dragging.DragHelper:
				helpers.Add(character);
				break;
		}

		return true;
	}

	public static IMovement CreateMovement(ICharacter originalMover, ICellExit exit, IEmote emote = null, bool ignoreSafeMovement = false)
	{
		// First get all of the party members who are present
		var primaryMovers = new List<ICharacter>();
		if (originalMover.Party is not null && originalMover.Party.Leader == originalMover)
		{
			primaryMovers.AddRange(originalMover.Party.CharacterMembers.Where(x => x.InRoomLocation == originalMover.InRoomLocation));
		}
		else
		{
			primaryMovers.Add(originalMover);
		}

		var failedMovers = new List<ICharacter>();
		var mounts = new List<ICharacter>();
		var nonConsensualMovers = new List<ICharacter>();
		var draggers = new List<ICharacter>();
		var nondraggers = new List<ICharacter>();
		var helpers = new List<ICharacter>();
		var targets = new List<IPerceivable>();
		var effects = new List<Dragging>();
		var consideredMovers = new List<ICharacter>();

		foreach (var mover in primaryMovers)
		{
			if (!EvaluateCharacterForAdditionToMovement(originalMover.Party, mover, exit, consideredMovers, nondraggers, mounts, draggers, helpers, nonConsensualMovers, targets, effects, false, false))
			{
				failedMovers.Add(mover);
			}
		}

		if (failedMovers.Contains(originalMover))
		{
			originalMover.OutputHandler.Send(originalMover.WhyCannotMove());
			return null;
		}

		// The Leave None Behind setting will fail to move if anybody at all can't move
		if (!ignoreSafeMovement && originalMover.Party?.LeaveNoneBehind == true && failedMovers.Count > 0)
		{
			// It's possible someone could have later been added back in as a non-consensual mover after failing initially
			foreach (var mover in failedMovers.ToList())
			{
				if (nonConsensualMovers.Contains(mover))
				{
					failedMovers.Remove(mover);
					continue;
				}
			}

			if (failedMovers.Count > 0)
			{
				originalMover.OutputHandler.Send($"You decide not to move because {failedMovers.Select(x => x.HowSeen(originalMover)).ListToString()} cannot move with you.\n{"Note: To disable this setting, turn off your party's leave none behind setting.".ColourCommand()}");
				return null;
			}
		}

		return new Movement(originalMover, originalMover.Party, draggers, helpers, nondraggers, mounts, targets, effects, exit);
	}

	public Movement(
			ICharacter originalMover,
			IParty party,
			IEnumerable<ICharacter> draggers,
			IEnumerable<ICharacter> helpers,
			IEnumerable<ICharacter> nonDraggers,
			IEnumerable<ICharacter> mounts,
			IEnumerable<IPerceivable> targets,
			IEnumerable<Dragging> dragEffects,
			ICellExit exit
		)
	{
		Exit = exit;
		Phase = MovementPhase.OriginalRoom;
		Party = party;
		Draggers.AddRange(draggers);
		Helpers.AddRange(helpers);
		NonDraggers.AddRange(nonDraggers);
		Mounts.AddRange(mounts);
		Targets.AddRange(targets);
		DragEffects.AddRange(dragEffects);
		CharacterMovers = [
			..Draggers,
			..Helpers,
			..NonDraggers,
			..Mounts,
			..Targets.OfType<ICharacter>()
		];
		OriginLayer = originalMover.RoomLayer;

		foreach (var mover in NonDraggers)
		{
			var sneak = mover.EffectsOfType<ISneakEffect>().FirstOrDefault();
			if (sneak is null)
			{
				continue;
			}

			var stealthOutcome = mover.Gameworld.GetCheck(CheckType.SneakCheck)
			                      .Check(mover, mover.Location.Terrain(mover).HideDifficulty);
			var effect = new SneakMove(mover)
			{
				Subtle = sneak.Subtle,
				StealthOutcome = stealthOutcome
			};
			mover.AddEffect(effect);
			SneakMoveEffects[mover] = effect;
		}

		var duration = TimeSpan.Zero;
		if (originalMover.AffectedBy<Immwalk>())
		{
			Duration = duration;
			return;
		}

		foreach (var ch in CharacterMovers)
		{
			if (ch.RidingMount is not null)
			{
				continue;
			}

			if (Targets.Contains(ch))
			{
				continue;
			}
			var speed = TimeSpan.FromMilliseconds(ch.MoveSpeed(exit));
			if (speed > duration)
			{
				duration = speed;
			}
		}

		if (duration.TotalMilliseconds > originalMover.Gameworld.GetStaticDouble("MaximumMoveTimeMilliseconds"))
		{
			duration = TimeSpan.FromMilliseconds(originalMover.Gameworld.GetStaticDouble("MaximumMoveTimeMilliseconds"));
		}

		Duration = duration;
	}

	/// <inheritdoc />
	public bool Cancel()
	{
		Cancelled = true;
		foreach (var mover in CharacterMovers)
		{
			mover.StopMovement(this);
			mover.Movement = null;
		}

		if (Party is not null)
		{
			Party.Movement = null;
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

	public void RemoveMover(ICharacter mover)
	{
		if (mover is null)
		{
			return;
		}

		Draggers.Remove(mover);
		NonDraggers.Remove(mover);
		Mounts.Remove(mover);
		Targets.Remove(mover);
		Helpers.Remove(mover);
		NonConsensualMovers.Remove(mover);

		mover.StopMovement(this);
		mover.Movement = null;
	}

	/// <inheritdoc />
	public bool CancelForMoverOnly(IMove mover, bool echo = false)
	{
		if (mover is not ICharacter ch)
		{
			return false;
		}

		// This can happen when supporters are removed automatically but also later called into this function (such as in loops)
		if (!CharacterMovers.Contains(mover))
		{
			return false;
		}

		if (mover == Party?.Leader)
		{
			return Cancel();
		}

		RemoveMover(ch);
		if (ch.RidingMount is not null)
		{
			RemoveMover(ch.RidingMount);
			if (echo)
			{
				ch.RidingMount.OutputHandler.Send(new EmoteOutput(new Emote("You stop moving because your rider ($0) has stopped.", ch, ch)));
			}

			foreach (var rider in ch.RidingMount.Riders)
			{
				RemoveMover(rider);
				if (echo)
				{
					rider.OutputHandler.Send(new EmoteOutput(new Emote("You stop moving because your mount ($0) has stopped.", ch.RidingMount, ch.RidingMount)));
				}
			}
		}

		foreach (var rider in ch.Riders)
		{
			RemoveMover(rider);
			if (echo)
			{
				rider.OutputHandler.Send(new EmoteOutput(new Emote("You stop moving because your mount ($0) has stopped.", ch, ch)));
			}
		}

		if (Draggers.Contains(ch))
		{
			var effect = DragEffects.First(x => x.TheDrag.CharacterOwner == ch);
			DragEffects.Remove(effect);
			foreach (var helper in effect.Helpers)
			{
				RemoveMover(helper);
				if (echo)
				{
					helper.OutputHandler.Send(new EmoteOutput(new Emote("You stop moving because your drag leader ($0) has stopped.", ch, ch)));
				}
			}

			RemoveMover(effect.Target as ICharacter);
			Targets.Remove(effect.Target);
			if (echo)
			{
				effect.Target.OutputHandler.Send(new EmoteOutput(new Emote("$0 &0|has|have stopped dragging you.", ch, new PerceivableGroup(effect.CharacterDraggers))));
			}
		}

		return true;
	}

	/// <inheritdoc />
	public void StopMovement()
	{
		if (Party is not null)
		{
			Party.Leader.OutputHandler.Handle(new EmoteOutput(new Emote("@ call|calls the group to a halt.", Party.Leader)));
		}
		else
		{
			if (CharacterMovers.Count() == 1)
			{
				CharacterMovers.First().OutputHandler.Handle(new EmoteOutput(new Emote("@ stop|stops moving.", CharacterMovers.First())));
			}
			else
			{
				var pg = new PerceivableGroup(CharacterMovers);
				CharacterMovers.First().OutputHandler.Handle(new EmoteOutput(new Emote("$0 $0|stop|stops moving.", CharacterMovers.First(), pg)));
			}
		}

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

	/// <inheritdoc />
	public bool IsMovementLeader(ICharacter character)
	{
		if (Party is not null)
		{
			return Party.Leader == character;
		}

		if (Draggers.Contains(character))
		{
			return true;
		}

		if (Draggers.Any())
		{
			return false;
		}

		return true;
	}

	/// <inheritdoc />
	public bool IsConsensualMover(ICharacter character)
	{
		return !NonConsensualMovers.Contains(character);
	}

	public string DescribeEnterMove(IPerceiver voyeur)
	{
		var sb = new StringBuilder();
		var suffix = Phase == MovementPhase.OriginalRoom ? Exit.OutboundMovementSuffix : Exit.InboundMovementSuffix;
		var seenMovers = CharacterMovers
		                 .Where(x => voyeur.CanSee(x))
		                 .Where(x => !Mounts.Contains(x) || !NonDraggers.Contains(x))
		                 .ToList();
		foreach (var effect in DragEffects)
		{
			var draggers = Draggers.Concat(Helpers).Where(x => effect.CharacterDraggers.Contains(x) && voyeur.CanSee(x))
			                       .ToList();
			if (!draggers.Any())
			{
				continue;
			}

			sb.AppendLine($"{draggers.Select(x => x.RidingMount is not null ? $"{x.HowSeen(voyeur)} (riding {x.RidingMount.HowSeen(voyeur)})" : x.HowSeen(voyeur)).ListToString()} {(draggers.Count == 1 ? "drags" : "drag")} {effect.Target.HowSeen(voyeur)} {suffix}.".Proper());
			seenMovers.RemoveAll(x => draggers.Contains(x));
		}

		if (seenMovers.Count > 0)
		{
			foreach (var group in seenMovers.GroupBy(x => (Speed: x.RidingMount?.CurrentSpeed ?? x.CurrentSpeed, Sneaking: (x.RidingMount ?? x).AffectedBy<ISneakMoveEffect>(y => y.Subtle))))
			{

				sb.AppendLine($"{group.Select(x => x.RidingMount is not null ? $"{x.HowSeen(voyeur)} (riding {x.RidingMount.HowSeen(voyeur)})" : x.HowSeen(voyeur)).ListToString()} {(group.Key.Sneaking ? "sneakily " : "")}{group.Key.Speed.ThirdPersonVerb} {suffix}.".Proper());
			}
		}

		return sb.ToString();
	}

	public string DescribeBeginMove(IPerceiver voyeur)
	{
		var sb = new StringBuilder();
		var suffix = Phase == MovementPhase.OriginalRoom ? Exit.OutboundMovementSuffix : Exit.InboundMovementSuffix;
		var seenMovers = CharacterMovers
		                 .Where(x => voyeur.CanSee(x))
		                 .Where(x => !Mounts.Contains(x) || !NonDraggers.Contains(x))
		                 .ToList();
		foreach (var effect in DragEffects)
		{
			var draggers = Draggers.Concat(Helpers).Where(x => effect.CharacterDraggers.Contains(x) && voyeur.CanSee(x))
			                       .ToList();
			if (!draggers.Any())
			{
				continue;
			}

			sb.AppendLine($"{draggers.Select(x => x.RidingMount is not null ? $"{x.HowSeen(voyeur)} (riding {x.RidingMount.HowSeen(voyeur)})" : x.HowSeen(voyeur)).ListToString()} {(draggers.Count == 1 ? "begins" : "begin")} dragging {effect.Target.HowSeen(voyeur)} {suffix}.".Proper());
			seenMovers.RemoveAll(x => draggers.Contains(x));
		}

		if (seenMovers.Count > 0)
		{
			foreach (var group in seenMovers.GroupBy(x => (Speed: x.RidingMount?.CurrentSpeed ?? x.CurrentSpeed, Sneaking: (x.RidingMount ?? x).AffectedBy<ISneakMoveEffect>(y => y.Subtle))))
			{

				sb.AppendLine($"{group.Select(x => x.RidingMount is not null ? $"{x.HowSeen(voyeur)} (riding {x.RidingMount.HowSeen(voyeur)})" : x.HowSeen(voyeur)).ListToString()} {(group.Count() == 1 ? "begins" : "begin")} {(group.Key.Sneaking ? "sneakily " : "")}{group.Key.Speed.PresentParticiple} {suffix}.".Proper());
			}
		}

		return sb.ToString();
	}

	/// <inheritdoc />
	public string Describe(IPerceiver voyeur)
	{
		var sb = new StringBuilder();
		var suffix = Phase == MovementPhase.OriginalRoom ? Exit.OutboundMovementSuffix : Exit.InboundMovementSuffix;
		var seenMovers = CharacterMovers
		                 .Where(x => voyeur.CanSee(x))
		                 .Where(x => !Mounts.Contains(x) || !NonDraggers.Contains(x))
		                 .ToList();
		foreach (var effect in DragEffects)
		{
			var draggers = Draggers.Concat(Helpers).Where(x => effect.CharacterDraggers.Contains(x) && voyeur.CanSee(x))
			                       .ToList();
			if (!draggers.Any())
			{
				continue;
			}

			sb.AppendLine($"{draggers.Select(x => x.RidingMount is not null ? $"{x.HowSeen(voyeur)} (riding {x.RidingMount.HowSeen(voyeur)})" : x.HowSeen(voyeur)).ListToString()} {(draggers.Count == 1 ? "is" : "are")} dragging {effect.Target.HowSeen(voyeur)} {suffix}.".Proper());
			seenMovers.RemoveAll(x => draggers.Contains(x));
		}

		if (seenMovers.Count > 0)
		{
			foreach (var group in seenMovers.GroupBy(x => (Speed: x.RidingMount?.CurrentSpeed ?? x.CurrentSpeed, Sneaking: (x.RidingMount ?? x).AffectedBy<ISneakMoveEffect>(y => y.Subtle))))
			{

				sb.AppendLine($"{group.Select(x => x.RidingMount is not null ? $"{x.HowSeen(voyeur)} (riding {x.RidingMount.HowSeen(voyeur)})" : x.HowSeen(voyeur)).ListToString()} {(group.Count() == 1 ? "is" : "are")} {(group.Key.Sneaking ? "sneakily " : "")}{group.Key.Speed.PresentParticiple} {suffix}.".Proper());
			}
		}

		return sb.ToString().StripANSIColour().Colour(Telnet.Yellow);
	}

	/// <inheritdoc />
	public bool SeenBy(IPerceiver voyeur, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		foreach (var mover in CharacterMovers)
		{
			if (voyeur == mover || voyeur is not ICharacter voyeurCh)
			{
				return true;
			}

			var sneakMoveEffect = SneakMoveEffects[mover];
			if (sneakMoveEffect is null)
			{
				continue;
			}
			
			var sawSneakEffect = voyeur.EffectsOfType<ISawSneakerEffect>().FirstOrDefault(x => x.Sneaker == mover);

			if (sawSneakEffect == null && flags.HasFlag(PerceiveIgnoreFlags.IgnoreSpotting))
			{
				var voyeurCheck = mover.Gameworld.GetCheck(CheckType.SpotSneakCheck)
				                       .Check(voyeurCh,
					                       voyeur.Location.SpotDifficulty(voyeur).StageDown(sneakMoveEffect.Subtle ? 2 : 0),
					                       mover);
				var opposed = new OpposedOutcome(sneakMoveEffect.StealthOutcome, voyeurCheck);
				sawSneakEffect = new SawSneaker(voyeur, mover, opposed.Outcome != OpposedOutcomeDirection.Proponent);
				voyeur.AddEffect(sawSneakEffect);
				sneakMoveEffect.RegisterSawSneaker(sawSneakEffect);
			}
		}

		return CharacterMovers.Any(x => voyeur.CanSee(x, flags));
	}

	/// <inheritdoc />
	public void InitialAction()
	{
		foreach (var ch in CharacterMovers)
		{
			ch.StartMove(this);
		}

		foreach (var ch in Exit.Origin.LayerCharacters(OriginLayer).Where(x => SeenBy(x)))
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
					$"You{(helpers.Count > 1 ? " and your fellow helpers" : "")} begin to assist {leader.HowSeen(ch)} with dragging {target.HowSeen(ch)} {Exit.OutboundMovementSuffix}.".Wrap(ch.InnerLineFormatLength));
			}

			leader.OutputHandler.Send(
				$"You{(helpers.Any() ? " and your helpers" : "")} begin to drag {target.HowSeen(leader)} {Exit.OutboundMovementSuffix}.".Wrap(leader.InnerLineFormatLength));
		}

		foreach (var mover in CharacterMovers)
		{
			CreateDepartureTracks(mover, DragEffects.Any(x => x.Target == mover) ? TrackCircumstances.Dragged : TrackCircumstances.None);
		}

		if (TimeSpan.Zero.CompareTo(Duration) < 0)
		{
			Exit.Origin.Gameworld.Scheduler.AddSchedule(new Schedule(IntermediateStep, ScheduleType.Movement, Duration, "Movement Intermediate Step"));
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

		var nonMovers = new List<ICharacter>();
		nonMovers.AddRange(Mounts.Where(mount => !mount.CanMove(Exit, CanMoveFlags.None).Result));
		nonMovers.AddRange(Draggers.Where(dragger => dragger.RidingMount is null).Where(dragger => !dragger.CanMove(Exit, CanMoveFlags.None).Result));
		nonMovers.AddRange(Helpers.Where(dragger => dragger.RidingMount is null).Where(dragger => !dragger.CanMove(Exit, CanMoveFlags.None).Result));
		nonMovers.AddRange(NonDraggers.Where(ch => ch.RidingMount is null).Where(ch => !ch.CanMove(Exit, CanMoveFlags.None).Result));
		if (nonMovers.Count > 0)
		{
			if (Party is not null && Party.LeaveNoneBehind)
			{
				TurnaroundTracks();
				var count = 0;
				var output =
					new EmoteOutput(
						new Emote(
							$"The party stops as {nonMovers.Select(x => "$" + count++).ListToString()} cannot move.", Party.Leader, nonMovers.ToArray<IPerceivable>()), flags: OutputFlags.InnerWrap);
				foreach (var person in CharacterMovers)
				{
					person.OutputHandler.Handle(output, OutputRange.Personal);
					person.HandleEvent(EventType.CharacterStopMovement);
					foreach (var witness in person.Location.EventHandlers.Except(person))
					{
						witness.HandleEvent(EventType.CharacterStopMovementWitness, person, Exit.Origin, Exit, witness);
					}

					foreach (var witness in person.Body.ExternalItems)
					{
						witness.HandleEvent(EventType.CharacterStopMovementWitness, person, Exit.Origin, Exit, witness);
					}
				}

				Cancel();
				return;
			}

			foreach (var mover in nonMovers)
			{
				mover.OutputHandler.Send("You are unable to follow your group any longer and stop moving.");
				CancelForMoverOnly(mover);
			}

			if (!CharacterMovers.Any())
			{
				Cancel();
				return;
			}
		}

		// Next try the same process but with crossing the threshold
		var stuck = CharacterMovers.Where(x => !x.CanCross(Exit).Success).ToList();
		if (stuck.Any())
		{
			foreach (var ch in stuck)
			{
				var (_, why) = ch.CanCross(Exit);
				ch.OutputHandler.Handle(why);
				CancelForMoverOnly(ch);
			}

			if (Party is not null && Party.LeaveNoneBehind)
			{
				TurnaroundTracks();
				var count = 0;
				var output =
					new EmoteOutput(
						new Emote(
							$"The party stops as {nonMovers.Select(x => "$" + count++).ListToString()} cannot cross to the other location.", Party.Leader, nonMovers.ToArray<IPerceivable>()), flags: OutputFlags.InnerWrap);
				foreach (var person in CharacterMovers)
				{
					person.OutputHandler.Handle(output, OutputRange.Personal);
					person.HandleEvent(EventType.CharacterStopMovement);
					foreach (var witness in person.Location.EventHandlers.Except(person))
					{
						witness.HandleEvent(EventType.CharacterStopMovementWitness, person, Exit.Origin, Exit, witness);
					}

					foreach (var witness in person.Body.ExternalItems)
					{
						witness.HandleEvent(EventType.CharacterStopMovementWitness, person, Exit.Origin, Exit, witness);
					}
				}

				Cancel();
				return;
			}

			if (!CharacterMovers.Any())
			{
				Cancel();
				return;
			}
		}

		var movers = CharacterMovers.ToList();
		foreach (var effect in DragEffects)
		{
			var dragLeader = Draggers.First(x => effect.CharacterDraggers.Contains(x));
			var target = effect.Target;
			var draggers = effect.Draggers.Where(x => movers.Contains(x.Character)).ToList();
			var dragCapacity = draggers.Sum(x =>
				(x.Character.MaximumDragWeight - x.Character.Body.ExternalItems.Sum(y => y.Weight)) *
				(x.Aid?.EffortMultiplier ?? 1.0));
			var weightThing = (IHaveWeight)target;
			if ((dragCapacity >= weightThing.Weight))
			{
				continue;
			}

			foreach (var dragger in draggers)
			{
				TurnaroundTrack(dragger.Character);
			}
			TurnaroundTrack(target as ICharacter);
			dragLeader.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@{(Helpers.Any() ? " and &0's helpers are" : " are|is")} unable to drag $1 {Exit.OutboundMovementSuffix} as #1 is too heavy.",
				dragLeader, dragLeader, target), flags: OutputFlags.InnerWrap));
			CancelForMoverOnly(dragLeader);
		}

		foreach (var mover in CharacterMovers)
		{
			mover.ExecuteMove(this);
		}

		foreach (var target in Targets)
		{
			if (target is IGameItem targetItem)
			{
				Exit.Origin.Extract(targetItem);
				targetItem.RoomLayer = Draggers.First().RoomLayer;
				Exit.Destination.Insert(targetItem, true);
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
			ch.OutputHandler.Send(DescribeEnterMove(ch));
		}

		foreach (var ch in CharacterMovers)
		{
			ch?.Body.Look(true);
		}

		var finalDelay = TimeSpan.FromTicks(Duration.Ticks / 5);
		if (TimeSpan.Zero.CompareTo(finalDelay) < 0)
		{
			Exit.Origin.Gameworld.Scheduler.AddSchedule(new Schedule(FinalStep, ScheduleType.Movement,
				finalDelay.TotalSeconds > 5 ? TimeSpan.FromSeconds(5) : finalDelay, "Movement FinalStep"));
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

		if (Party is not null)
		{
			Party.Movement = null;
		}

		Exit.Destination.ResolveMovement(this);
		if (Party?.Leader.QueuedMoveCommands.Count > 0)
		{
			Party.Leader.Move(Party.Leader.QueuedMoveCommands.Dequeue());
		}
	}


	#endregion

	#region Tracks
	protected void TurnaroundTrack(ICharacter mover)
	{
		var track = DepartureTracks[mover];
		if (track is null)
		{
			return;
		}

		track.TurnedAround = true;
		track.Changed = true;
	}

	protected void TurnaroundTracks()
	{
		foreach (var mover in CharacterMovers)
		{
			TurnaroundTrack(mover);
		}
	}

	protected DictionaryWithDefault<ICharacter, ITrack> DepartureTracks { get; } = new();

	protected static TrackCircumstances ApplyTrackCircumstances(ICharacter actor, TrackCircumstances circumstance)
	{
		if (actor.Body.Wounds.Any(x => x.BleedStatus == BleedStatus.Bleeding))
		{
			circumstance |= TrackCircumstances.Bleeding;
		}

		if (actor.Combat is not null && actor.CombatStrategyMode == CombatStrategyMode.Flee)
		{
			circumstance |= TrackCircumstances.Fleeing;
		}

		return circumstance;
	}

	protected void CreateDepartureTracks(ICharacter actor, TrackCircumstances circumstance)
	{
		// Check for global tracking system disabled
		if (!actor.Gameworld.GetStaticBool("TrackingEnabled"))
		{
			return;
		}

		// Check to see if terrain type permits tracks
		var location = actor.Location;
		if (!location.Terrain(actor).CanHaveTracks)
		{
			return;
		}

		// Admin avatars do not leave tracks
		if (actor.IsAdministrator())
		{
			return;
		}

		// Apply circumstantial modifiers
		circumstance = ApplyTrackCircumstances(actor, circumstance);

		// Check to see if any visual or olfactory tracks are left
		if (GetTrackIntensities(actor, location, out var visual, out var olfactory))
		{
			return;
		}

		// Tracks were left - create new tracks
		var track = new MudSharp.Movement.Track(actor.Gameworld, actor, Exit, circumstance, true, visual, olfactory);
		actor.Gameworld.Add(track);
		DepartureTracks[actor] = track;
		location.AddTrack(track);
	}

	private static bool GetTrackIntensities(ICharacter actor, ICell location, out double visual, out double olfactory)
	{
		visual = 1.0 * location.Terrain(actor).TrackIntensityMultiplierVisual * actor.Race.TrackIntensityVisual;
		olfactory = 1.0 * location.Terrain(actor).TrackIntensityMultiplierOlfactory * actor.Race.TrackIntensityOlfactory;

		// Don't leave tracks if swimming, flying, or riding a mount
		if (
			location.IsSwimmingLayer(actor.RoomLayer) || 
			actor.RoomLayer.In(RoomLayer.InAir, RoomLayer.HighInAir) ||
			actor.RidingMount is not null
			)
		{
			visual = 0.0;
		}

		// If visual and olfactory intensity is zero or less, no tracks are left
		if (visual <= 0.0 && olfactory <= 0.0)
		{
			return true;
		}

		return false;
	}

	protected void CreateArrivalTracks(ICharacter actor, TrackCircumstances circumstance)
	{
		if (!actor.Gameworld.GetStaticBool("TrackingEnabled"))
		{
			return;
		}
		var location = actor.Location;
		if (!location.Terrain(actor).CanHaveTracks)
		{
			return;
		}

		if (actor.IsAdministrator())
		{
			return;
		}

		if (GetTrackIntensities(actor, location, out var visual, out var olfactory))
		{
			return;
		}

		circumstance = ApplyTrackCircumstances(actor, circumstance);
		var track = new MudSharp.Movement.Track(actor.Gameworld, actor, Exit.Opposite, circumstance, false, visual, olfactory);
		actor.Gameworld.Add(track);
		location.AddTrack(track);
	}
	#endregion
}