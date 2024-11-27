using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body.Implementations;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class LookingForTracks : Effect, IActionEffect, ILDescSuffixEffect, IRemoveOnStateChange
{
	private static ITraitExpression _lookingForTracksTime;

	public static ITraitExpression LookingForTracksTime
	{
		get { return _lookingForTracksTime ??= new TraitExpression(Futuremud.Games.First().GetStaticConfiguration("LookingForTracksTimeExpression"), Futuremud.Games.First()); }
	}

	public static TimeSpan GetLookingForTracksTime(ICharacter actor)
	{
		return TimeSpan.FromSeconds(LookingForTracksTime.Evaluate(actor));
	}

	/// <inheritdoc />
	protected override string SpecificEffectType => "LookingForTracks";

	/// <inheritdoc />
	public LookingForTracks(ICharacter owner) : base(owner, null)
	{
		CharacterOwner = owner;
		_blocks.Add("general");
	}

	private readonly List<string> _blocks = new();
	public ICharacter CharacterOwner { get; set; }

	private readonly List<ITrack> _alreadyFoundTracks = new();

	public string ActionDescription => "searching for tracks";
	public string LDescAddendumEmote => "searching for tracks";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Searching for Tracks";
	}

	public override bool CanBeStoppedByPlayer => true;

	public override void ExpireEffect()
	{
		OnCompletionAction(CharacterOwner);
		Owner.Reschedule(this, GetLookingForTracksTime(CharacterOwner));
	}

	public override IEnumerable<string> Blocks => _blocks;

	public override bool IsBlockingEffect(string blockingType)
	{
		return string.IsNullOrEmpty(blockingType) || _blocks.Contains(blockingType);
	}

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return ActionDescription;
	}

	public virtual string SuffixFor(IPerceiver voyeur)
	{
		return new EmoteOutput(new Emote(LDescAddendumEmote, CharacterOwner, CharacterOwner)).ParseFor(voyeur)
		                                                                                     .ToLowerInvariant();
	}

	public bool SuffixApplies()
	{
		return !string.IsNullOrEmpty(LDescAddendumEmote);
	}

	public bool ShouldRemove(CharacterState newState)
	{
		return newState.HasFlag(CharacterState.Dead) || !CharacterState.Able.HasFlag(newState);
	}

	Action<IPerceivable> IActionEffect.Action => _ => OnCompletionAction(CharacterOwner);

	public void OnCompletionAction(ICharacter actor)
	{

		var checkVision = actor.Gameworld.GetCheck(CheckType.SearchForTracksCheck);
		var checkSmell = actor.Gameworld.GetCheck(CheckType.SearchForTracksByScentScheck);

		var visionResult = checkVision.CheckAgainstAllDifficulties(actor, Difficulty.Normal, null);
		var smellResult = checkSmell.CheckAgainstAllDifficulties(actor, Difficulty.Normal, null);

		var tracks = actor.Location.Tracks
		                  .Except(_alreadyFoundTracks)
		                  .Where(x => x.RoomLayer == actor.RoomLayer)
		                  .ToList();
		var difficulties = tracks
		                   .Select(x => (Track: x, Visual: x.VisualTrackDifficulty(actor), Olfactory: x.OlfactoryTrackDifficulty(actor)))
		                   .ToList();
		var successfulTracks = difficulties
		                       .Where(x => visionResult[x.Visual].IsPass() || smellResult[x.Olfactory].IsPass())
		                       .ToList();
		var track = successfulTracks.GetRandomElement();
		if (track.Track is null)
		{
			actor.OutputHandler.Send("You couldn't find any tracks, but you continue to search.");
			return;
		}

		var passedVisual = visionResult[track.Visual].IsPass();
		var passedSmell = smellResult[track.Olfactory].IsPass();

		var sb = new StringBuilder();
		sb.AppendLine("You found a track...");
		if (passedSmell)
		{
			sb.AppendLine($"...It was left by a #5{track.Track.Character.Gender.GenderClass()} {track.Track.Character.Race.Name}#0.".SubstituteANSIColour());
			sb.AppendLine($"...You can smell that its exertion level was {track.Track.ExertionLevel.DescribeEnum()} at the time.");
			if (visionResult[track.Olfactory].Outcome == Outcome.MajorPass)
			{
				if (actor.Dubs.Any(x => x.Owner == track.Track.Character))
				{

				}
			}
		}
		else if (visionResult[track.Visual].Outcome == Outcome.MajorPass)
		{
			sb.AppendLine($"...It was left by {track.Track.Character.Race.Name.A_An_RespectPlurals().ColourCharacter()}.");
		}
		else
		{
			sb.AppendLine($"...It was left by {track.Track.BodyProtoType.NameForTracking.A_An_RespectPlurals(true).ColourCharacter()}.");
		}

		if (passedVisual)
		{
			if (track.Track.TrackCircumstances.HasFlag(TrackCircumstances.Dragged))
			{
				if (track.Track.TurnedAround)
				{
					sb.AppendLine($"...It was dragged {track.Track.FromCellExit?.InboundMovementSuffix ?? track.Track.ToCellExit?.OutboundMovementSuffix} but turned around.");
				}
				else
				{
					sb.AppendLine($"...It was dragged {track.Track.FromCellExit?.InboundMovementSuffix ?? track.Track.ToCellExit?.OutboundMovementSuffix}.");
				}
			}
			else
			{
				var speed = track.Track.FromSpeed ?? track.Track.ToSpeed;
				if (track.Track.TurnedAround)
				{
					sb.AppendLine($"...It {speed!.PresentParticiple} {track.Track.FromCellExit?.InboundMovementSuffix ?? track.Track.ToCellExit?.OutboundMovementSuffix} but turned around.");
				}
				else
				{
					sb.AppendLine($"...It {speed!.PresentParticiple} {track.Track.FromCellExit?.InboundMovementSuffix ?? track.Track.ToCellExit?.OutboundMovementSuffix}.");
				}
			}


			if (track.Track.TrackCircumstances.HasFlag(TrackCircumstances.Careful))
			{
				sb.AppendLine($"...It was moving carefully and stealthily.");
			}

			if (track.Track.TrackCircumstances.HasFlag(TrackCircumstances.Fleeing))
			{
				sb.AppendLine($"...It was moving erratically, as if fleeing.");
			}
		}
		else
		{
			if (track.Track.TurnedAround)
			{
				sb.AppendLine($"...It went {track.Track.FromCellExit?.InboundMovementSuffix ?? track.Track.ToCellExit?.OutboundMovementSuffix} but turned around.");
			}
			else
			{
				sb.AppendLine($"...It went {track.Track.FromCellExit?.InboundMovementSuffix ?? track.Track.ToCellExit?.OutboundMovementSuffix}.");
			}
		}

		if (track.Track.TrackCircumstances.HasFlag(TrackCircumstances.Bleeding))
		{
			sb.AppendLine($"...It was bleeding as it went.");
		}
		sb.AppendLine($"...It was left #2approximately {(actor.Location.DateTime()-track.Track.MudDateTime).Describe(actor)} ago#0.".SubstituteANSIColour());
		sb.AppendLine($"...It was {track.Visual.DescribeColoured()} to see and {track.Visual.DescribeColoured()} to smell for you.");

		actor.OutputHandler.Send(sb.ToString());

	}
}