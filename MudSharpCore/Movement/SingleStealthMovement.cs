using System;
using System.Linq;
using MudSharp.Body.Traits;
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
using MudSharp.RPG.Checks;

namespace MudSharp.Movement;

public class SingleStealthMovement : SingleMovement
{
	public SingleStealthMovement(ICellExit exit, ICharacter mover, TimeSpan duration, IEmote playerEmote,
		bool subtle = false)
		: base(exit, mover, duration, playerEmote)
	{
		StealthOutcome = mover.Gameworld.GetCheck(CheckType.SneakCheck)
		                      .Check(mover, mover.Location.Terrain(mover).HideDifficulty);
		SneakMoveEffect = new SneakMove(mover) { Subtle = subtle };
		mover.AddEffect(SneakMoveEffect);
	}

	public Outcome StealthOutcome { get; }
	public ISneakMoveEffect SneakMoveEffect { get; }

	protected override MixedEmoteOutput GetMovementOutput
		=>
			new(
				new Emote(
					$"@ begin|begins {(SneakMoveEffect.Subtle ? "" : "sneakily ")}{Mover.CurrentSpeed.PresentParticiple} {Exit.OutboundMovementSuffix}",
					Mover), flags: OutputFlags.SuppressObscured);

	public override bool SeenBy(IPerceiver voyeur, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if (voyeur == Mover)
		{
			return true;
		}

		if (!(voyeur is IPerceivableHaveTraits voyeurHasTraits))
		{
			return false;
		}

		if (voyeur.AffectedBy<IAdminSightEffect>())
		{
			return true;
		}

		var sawSneakEffect = voyeur.EffectsOfType<ISawSneakerEffect>().FirstOrDefault(x => x.Sneaker == Mover);

		if (sawSneakEffect == null && flags.HasFlag(PerceiveIgnoreFlags.IgnoreSpotting))
		{
			var voyeurCheck = Mover.Gameworld.GetCheck(CheckType.SpotSneakCheck)
			                       .Check(voyeurHasTraits,
				                       voyeur.Location.SpotDifficulty(voyeur).StageDown(SneakMoveEffect.Subtle ? 2 : 0),
				                       Mover);
			var opposed = new OpposedOutcome(StealthOutcome, voyeurCheck);
			sawSneakEffect = new SawSneaker(voyeur, Mover, opposed.Outcome != OpposedOutcomeDirection.Proponent);
			voyeur.AddEffect(sawSneakEffect);
			SneakMoveEffect.RegisterSawSneaker(sawSneakEffect);
		}

		return base.SeenBy(voyeur, flags) && (sawSneakEffect?.Success ?? false);
	}

	public override string Describe(IPerceiver voyeur)
	{
		switch (Phase)
		{
			case MovementPhase.OriginalRoom:
				return
					$"{Mover.HowSeen(voyeur, true, DescriptionType.Short, false)} is {Mover.CurrentSpeed.PresentParticiple} {(SneakMoveEffect.Subtle ? "" : "sneakily ")}{Exit.OutboundMovementSuffix}.";
			case MovementPhase.NewRoom:
				return
					$"{Mover.HowSeen(voyeur, true, DescriptionType.Short, false)} is {Mover.CurrentSpeed.PresentParticiple} {(SneakMoveEffect.Subtle ? "" : "sneakily ")}{Exit.InboundMovementSuffix}.";
			default:
				throw new NotSupportedException("SingleStealthMovement.Describe Invalid Phase.");
		}
	}

	public override void FinalStep()
	{
		base.FinalStep();
		Mover.RemoveEffect(SneakMoveEffect);
	}

	public override bool Cancel()
	{
		Mover.RemoveEffect(SneakMoveEffect);
		return base.Cancel();
	}

	public override void IntermediateStep()
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
				foreach (var character in Exit.Origin.Characters.Where(x => SeenBy(x)))
				{
					character.OutputHandler.Send(whyNot);
				}

				Mover.HandleEvent(EventType.CharacterStopMovementClosedDoor, Mover, Exit.Origin, Exit);
				foreach (var witness in Mover.Location.EventHandlers.Except(Mover))
				{
					witness.HandleEvent(EventType.CharacterStopMovementClosedDoorWitness, Mover, Exit.Origin, Exit,
						witness);
				}
				Cancel();
			}
			else
			{
				Mover.ExecuteMove(this);
				Phase = MovementPhase.NewRoom;
				Exit.Destination.RegisterMovement(this);
				Exit.Origin.ResolveMovement(this);
				CreateArrivalTracks(Mover, MovementTrackCircumstances);
				var output =
					new MixedEmoteOutput(
						new Emote(
							$"@ {Mover.CurrentSpeed.FirstPersonVerb}|{Mover.CurrentSpeed.ThirdPersonVerb} {(SneakMoveEffect.Subtle ? "" : "sneakily ")}{Exit.InboundMovementSuffix}",
							Mover),
						flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource).Append(MoverEmote);

				foreach (var character in Exit.Destination.Characters.Except(Mover).Where(x => SeenBy(x)))
				{
					if (Mover.AffectedBy<IHideEffect>())
					{
						character.AddEffect(new SawHider(character, Mover), TimeSpan.FromSeconds(300));
					}

					character.OutputHandler.Send(output);
				}

				Mover.Body.Look(true);

				var finalDelay = TimeSpan.FromTicks(Duration.Ticks / 5);
				if (TimeSpan.Zero.CompareTo(finalDelay) < 0)
				{
					Mover.Gameworld.Scheduler.AddSchedule(new Schedule(FinalStep, ScheduleType.Movement,
						finalDelay.TotalSeconds > 5 ? TimeSpan.FromSeconds(5) : finalDelay,
						"SingleStealthMovement Final Step"));
				}
				else
				{
					FinalStep();
				}

				return;
			}
		}

		TurnaroundTracks();
		Mover.Send(Mover.WhyCannotMove());
		Cancel();
	}

	/// <inheritdoc />
	protected override TrackCircumstances MovementTrackCircumstances => TrackCircumstances.Careful;
}