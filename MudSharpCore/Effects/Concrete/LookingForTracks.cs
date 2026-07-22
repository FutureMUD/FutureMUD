using MudSharp.Body.Implementations;
using MudSharp.Body.Traits;
using MudSharp.Movement;
using MudSharp.RPG.Checks;

using MudSharp.Construction;
using MudSharp.Framework.Units;

namespace MudSharp.Effects.Concrete;

public class LookingForTracks : Effect, IActionEffect, ILDescSuffixEffect, IRemoveOnStateChange
{
    private static ITraitExpression _lookingForTracksTime;

    public static ITraitExpression LookingForTracksTime => _lookingForTracksTime ??= new TraitExpression(Futuremud.Games.First().GetStaticConfiguration("LookingForTracksTimeExpression"), Futuremud.Games.First());

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
    private readonly List<IScentTrailEffect> _alreadyFoundScents = new();

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

        ICheck checkVision = actor.Gameworld.GetCheck(CheckType.SearchForTracksCheck);
        ICheck checkSmell = actor.Gameworld.GetCheck(CheckType.SearchForTracksByScentScheck);

        Dictionary<Difficulty, CheckOutcome> visionResult = checkVision.CheckAgainstAllDifficulties(actor, Difficulty.Normal, null);
        Dictionary<Difficulty, CheckOutcome> smellResult = checkSmell.CheckAgainstAllDifficulties(actor, Difficulty.Normal, null);

		var spatialLocation = RouteSpatialService.Instance.GetEffectiveLocation(actor);
		var trackSearchRadius = RouteSpatialConfiguration.FromGameworld(actor.Gameworld).ProximateDistanceMetres;
		List<ITrack> tracks = actor.Location.Tracks
                          .Except(_alreadyFoundTracks)
                          .Where(x => x.RoomLayer == actor.RoomLayer)
		                  .Where(x => actor.Location.RouteDefinition is null ||
		                              spatialLocation.RoutePositionMetres.HasValue &&
		                              x.RoutePositionMetres.HasValue &&
		                              Math.Abs(x.RoutePositionMetres.Value -
		                                       spatialLocation.RoutePositionMetres.Value) <= trackSearchRadius)
                          .ToList();
        List<(ITrack Track, Difficulty Visual, Difficulty Olfactory)> difficulties = tracks
                           .Select(x => (Track: x, Visual: x.VisualTrackDifficulty(actor), Olfactory: x.OlfactoryTrackDifficulty(actor)))
                           .ToList();
        List<(ITrack Track, Difficulty Visual, Difficulty Olfactory)> successfulTracks = difficulties
                               .Where(x => visionResult[x.Visual].IsPass() || smellResult[x.Olfactory].IsPass())
                               .ToList();
        List<IScentTrailEffect> scents = ApplicableScentTrails(
                actor,
                actor.Location.EffectsOfType<IScentTrailEffect>()
                    .Except(_alreadyFoundScents))
            .ToList();
        List<IScentTrailEffect> successfulScents = scents
                                                   .Where(x => smellResult[x.ScentDifficulty(actor)].IsPass())
                                                   .ToList();
        if (successfulScents.Any() && (!successfulTracks.Any() || Constants.Random.Next(2) == 0))
        {
            IScentTrailEffect scent = successfulScents.GetRandomElement();
            _alreadyFoundScents.Add(scent);
            actor.OutputHandler.Send($"You found a scent trail...\n...{scent.DescribeForTracksCommand(actor)}");
            return;
        }

        (ITrack Track, Difficulty Visual, Difficulty Olfactory) track = successfulTracks.GetRandomElement();
        if (track.Track is null)
        {
            actor.OutputHandler.Send("You couldn't find any tracks, but you continue to search.");
            return;
        }

        _alreadyFoundTracks.Add(track.Track);

        bool passedVisual = visionResult[track.Visual].IsPass();
        bool passedSmell = smellResult[track.Olfactory].IsPass();

        StringBuilder sb = new();
        sb.AppendLine("You found a track...");
		var vehicle = track.Track.Vehicle;
		var character = track.Track.Character;
		if (vehicle is not null)
		{
			sb.AppendLine($"...It was left by {vehicle.Name.ColourName()}.");
		}
        else if (passedSmell && character is not null)
        {
            sb.AppendLine($"...It was left by a #5{character.Gender.GenderClass()} {character.Race.Name}#0.".SubstituteANSIColour());
            sb.AppendLine($"...You can smell that its exertion level was {track.Track.ExertionLevel.DescribeEnum()} at the time.");
            if (smellResult[track.Olfactory].Outcome == Outcome.MajorPass)
            {
                IDub dub = actor.Dubs.FirstOrDefault(x => x.Owner == character);
                if (dub is not null)
                {
                    sb.AppendLine($"...It smells like {dub.HowSeen(actor).ColourIncludingReset(Telnet.Magenta)}.");
                }
            }
        }
        else if (visionResult[track.Visual].Outcome == Outcome.MajorPass && character is not null)
        {
            sb.AppendLine($"...It was left by {character.Race.Name.A_An_RespectPlurals().ColourCharacter()}.");
        }
		else if (track.Track.BodyProtoType is not null)
        {
            sb.AppendLine($"...It was left by {track.Track.BodyProtoType.NameForTracking.A_An_RespectPlurals(true).ColourCharacter()}.");
        }
		else
		{
			sb.AppendLine("...Its source could not be identified.");
		}

        if (passedVisual)
        {
			if (track.Track.RoutePositionMetres.HasValue && track.Track.RouteDirection.HasValue)
			{
				var route = track.Track.Cell.RouteDefinition;
				var direction = track.Track.RouteDirection == RouteCellDirection.Positive
					? route?.PositiveDirectionName ?? "forward"
					: route?.NegativeDirectionName ?? "backward";
				var distance = actor.Gameworld.UnitManager.DescribeMostSignificantExact(
					track.Track.RoutePositionMetres.Value / actor.Gameworld.UnitManager.BaseHeightToMetres,
					UnitType.Length,
					actor);
				sb.AppendLine($"...It travelled {direction} past {distance}.");
			}
            else if (track.Track.TrackCircumstances.HasFlag(TrackCircumstances.Dragged))
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
                IMoveSpeed speed = track.Track.FromSpeed ?? track.Track.ToSpeed;
				if (speed is null)
				{
					sb.AppendLine("...Its exact movement gait could not be determined.");
				}
                else if (track.Track.TurnedAround)
                {
                    sb.AppendLine($"...It {speed.PresentParticiple} {track.Track.FromCellExit?.InboundMovementSuffix ?? track.Track.ToCellExit?.OutboundMovementSuffix} but turned around.");
                }
                else
                {
                    sb.AppendLine($"...It {speed.PresentParticiple} {track.Track.FromCellExit?.InboundMovementSuffix ?? track.Track.ToCellExit?.OutboundMovementSuffix}.");
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
			if (track.Track.RoutePositionMetres.HasValue && track.Track.RouteDirection.HasValue)
			{
				var route = track.Track.Cell.RouteDefinition;
				var direction = track.Track.RouteDirection == RouteCellDirection.Positive
					? route?.PositiveDirectionName ?? "forward"
					: route?.NegativeDirectionName ?? "backward";
				sb.AppendLine($"...It went {direction} along the route.");
			}
            else if (track.Track.TurnedAround)
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
        if (track.Track.TrackCircumstances.HasFlag(TrackCircumstances.MagicallyMarked))
        {
            sb.AppendLine($"...It carries an unnatural magical trace.");
        }
        sb.AppendLine($"...It was left #2approximately {(actor.Location.DateTime() - track.Track.MudDateTime).Describe(actor)} ago#0.".SubstituteANSIColour());
        sb.AppendLine($"...It was {track.Visual.DescribeColoured()} to see and {track.Olfactory.DescribeColoured()} to smell for you.");

        actor.OutputHandler.Send(sb.ToString());

    }

	internal static IEnumerable<IScentTrailEffect> ApplicableScentTrails(
		ICharacter actor,
		IEnumerable<IScentTrailEffect> scents)
	{
		return scents
			.Where(x => x.RoomLayer == actor.RoomLayer)
			.Where(x => x.Applies(actor));
	}
}
