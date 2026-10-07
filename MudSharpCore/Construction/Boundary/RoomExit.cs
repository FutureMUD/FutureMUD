using MudSharp.Body.Position.PositionStates;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Checks;

namespace MudSharp.Construction.Boundary;

public class RoomExit : IRoomExit
{
    protected Lazy<List<string>> _keywords;

    public RoomExit(IExit parent, MudSharp.Models.Exit exit, bool firstRoomExit)
    {
        Exit = parent;
        Origin = parent.Gameworld.Rooms.Get(firstRoomExit ? exit.RoomId1 : exit.RoomId2);
        Destination = parent.Gameworld.Rooms.Get(firstRoomExit ? exit.RoomId2 : exit.RoomId1);
        InboundDirection = (CardinalDirection)(firstRoomExit ? exit.Direction2 : exit.Direction1);
        OutboundDirection = (CardinalDirection)(firstRoomExit ? exit.Direction1 : exit.Direction2);
        _keywords = new Lazy<List<string>>(() => new List<string> { OutboundDirection.Describe().ToLowerInvariant() });
    }

    public RoomExit(IExit parent, IRoomExit exit, IRoom origin, IRoom destination)
    {
        Exit = parent;
        Origin = origin;
        Destination = destination;
        InboundDirection = exit.InboundDirection;
        OutboundDirection = exit.OutboundDirection;
        _keywords = new Lazy<List<string>>(() => new List<string> { OutboundDirection.Describe().ToLowerInvariant() });
    }

    public RoomExit(IExit parent, IRoom origin, IRoom destination, CardinalDirection outboundDirection,
        CardinalDirection inboundDirection)
    {
        Exit = parent;
        Origin = origin;
        Destination = destination;
        InboundDirection = inboundDirection;
        OutboundDirection = outboundDirection;
        _keywords = new Lazy<List<string>>(() => new List<string> { OutboundDirection.Describe().ToLowerInvariant() });
    }

    public override string ToString()
    {
        return $"CellExit {OutboundDirection.Describe()} to {Destination.Name} ({Destination.Id:N0})";
    }

    public IExit Exit { get; protected set; }

    public IRoom Origin { get; protected set; }

    public IEnumerable<RoomLayer> WhichLayersExitAppears()
    {
        List<RoomLayer> layers = new();
        IEnumerable<RoomLayer> originLayers = Origin.Terrain(null).TerrainLayers;

        if (OutboundDirection == CardinalDirection.Up)
        {
            layers.Add(originLayers.HighestLayer());
            return layers;
        }

        if (OutboundDirection == CardinalDirection.Down)
        {
            layers.Add(originLayers.LowestLayer());
            return layers;
        }

        IEnumerable<RoomLayer> destinationLayers = Destination.Terrain(null).TerrainLayers;
        foreach (RoomLayer layer in originLayers)
        {
            if (Exit.BlockedLayers.Contains(layer))
            {
                continue;
            }

            if (!destinationLayers.Contains(layer))
            {
                continue;
            }

            layers.Add(layer);
        }

        return layers;
    }

    public IRoom Destination { get; protected set; }

    public CardinalDirection OutboundDirection { get; protected set; }

    public CardinalDirection InboundDirection { get; protected set; }

    public IRoomExit Opposite => Exit.RoomExitFor(Destination);

    public virtual string OutboundMovementSuffix => "away towards the " + OutboundDirection.Describe();

    public virtual string InboundMovementSuffix => "in from the " + InboundDirection.Describe();

    public virtual string OutboundDirectionSuffix => "from the " + OutboundDirection.Describe();

    public virtual string InboundDirectionSuffix => "from the " + InboundDirection.Describe();

    public virtual string OutboundDirectionDescription => "the " + OutboundDirection.Describe();

    public bool IsFallExit => Exit.FallRoom == Destination;
    public bool IsFlyExit => Exit.FallRoom == Origin && !IsClimbExit;
    public bool IsClimbExit => Exit.IsClimbExit;
    public Difficulty ClimbDifficulty => Exit.ClimbDifficulty;

    public virtual string DescribeFor(IPerceiver voyeur, bool colour)
    {
        string startColourString = "";
        string endColourString = "";
        if (colour)
        {
            (RoomMovementTransition transition, RoomLayer _) = MovementTransition(voyeur);
            switch (transition)
            {
                case RoomMovementTransition.SwimOnly:
                    startColourString = Telnet.BoldBlue.ToString();
                    endColourString = Telnet.RESET + Telnet.Green.ToString();
                    break;
                case RoomMovementTransition.FallExit:
                    if (IsClimbExit)
                    {
                        startColourString = Telnet.Yellow.ToString();
                        endColourString = Telnet.Green.ToString();
                        break;
                    }

                    startColourString = Telnet.Red.ToString();
                    endColourString = Telnet.Green.ToString();
                    break;
                case RoomMovementTransition.FlyOnly:
                    if (IsClimbExit)
                    {
                        startColourString = Telnet.Yellow.ToString();
                        endColourString = Telnet.Green.ToString();
                        break;
                    }

                    startColourString = Telnet.BoldCyan.ToString();
                    endColourString = Telnet.RESET + Telnet.Green.ToString();
                    break;
            }

            if (string.IsNullOrEmpty(startColourString))
            {
                (startColourString, endColourString) = DoorCapableExitDescriptionColour();
            }
        }

        string dirString = OutboundDirection.Describe();
        string doorString = Exit.Door != null
            ? $" ({Exit.Door.State.Describe().ToLowerInvariant()} {Exit.Door.InstalledExitDescription(voyeur)})"
            : "";
        return $"{startColourString}{dirString}{doorString}{endColourString}";
    }

    protected (string StartColourString, string EndColourString) DoorCapableExitDescriptionColour()
    {
        return Exit.AcceptsDoor && Exit.Door is null
            ? (Telnet.BoldWhite.ToString(), Telnet.RESET + Telnet.Green.ToString())
            : ("", "");
    }

    public virtual string BuilderInformationString(IPerceiver voyeur)
    {
        return string.Format("Cardinal {4}{5}Exit #{0:N0} - {1} to {3} at x{2:N} speed", Exit.Id,
            OutboundDirection.Describe(), Exit.TimeMultiplier, Destination.HowSeen(voyeur, colour: false),
            IsFallExit ? "Fall " : "", IsClimbExit ? $"{ClimbDifficulty.Describe()} Climb " : "");
    }

    public virtual bool IsExit(string verb)
    {
        return CardinalDirectionExtensions.CardinalExitStrings.TryGetValue(verb, out var direction) &&
               direction == OutboundDirection;
    }

    public virtual bool IsExitKeyword(string keyword)
    {
        return IsExit(keyword);
    }

    public IEnumerable<string> Keywords => _keywords.Value;

    public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
    {
        return _keywords.Value;
    }

    public (RoomMovementTransition TransitionType, RoomLayer TargetLayer) MovementTransition(IPerceiver perceiver)
    {
        if (!WhichLayersExitAppears().Contains(perceiver.RoomLayer))
        {
            return (RoomMovementTransition.NoViableTransition, RoomLayer.GroundLevel);
        }

        ITerrain originTerrain = Origin.Terrain(perceiver);
        ITerrain destinationTerrain = Destination.Terrain(perceiver);
        if (IsClimbExit || IsFlyExit || IsFallExit)
        {
            if (OutboundDirection == CardinalDirection.Up)
            {
                if (originTerrain.TerrainLayers.HighestLayer() != perceiver.RoomLayer)
                {
                    return (RoomMovementTransition.NoViableTransition, RoomLayer.GroundLevel);
                }

                if (perceiver.RoomLayer.IsUnderwater())
                {
                    return (RoomMovementTransition.SwimOnly, destinationTerrain.TerrainLayers.LowestLayer());
                }

                return (RoomMovementTransition.FlyOnly, destinationTerrain.TerrainLayers.LowestLayer());
            }

            if (OutboundDirection == CardinalDirection.Down)
            {
                if (originTerrain.TerrainLayers.LowestLayer() != perceiver.RoomLayer)
                {
                    return (RoomMovementTransition.NoViableTransition, RoomLayer.GroundLevel);
                }

                if (perceiver.RoomLayer.IsUnderwater())
                {
                    if (destinationTerrain.TerrainLayers.Any(x => !x.IsUnderwater()))
                    {
                        return (RoomMovementTransition.FallExit, destinationTerrain.TerrainLayers.HighestLayer());
                    }

                    return (RoomMovementTransition.SwimOnly, destinationTerrain.TerrainLayers.HighestLayer());
                }

                return (RoomMovementTransition.FallExit, destinationTerrain.TerrainLayers.HighestLayer());
            }
        }

        if (destinationTerrain.TerrainLayers.Contains(perceiver.RoomLayer))
        {
            if (perceiver.Location.IsSwimmingLayer(perceiver.RoomLayer))
            {
                if (Destination.Location.IsSwimmingLayer(perceiver.RoomLayer))
                {
                    return (RoomMovementTransition.SwimOnly, perceiver.RoomLayer);
                }

                if (!Destination.Location.IsUnderwaterLayer(perceiver.RoomLayer) &&
                    perceiver.PositionState == PositionFlying.Instance)
                {
                    return (RoomMovementTransition.FlyOnly, perceiver.RoomLayer);
                }

                if (destinationTerrain.TerrainLayers.Any(x => Destination.Location.IsSwimmingLayer(x)))
                {
                    return (RoomMovementTransition.FallExit, perceiver.RoomLayer);
                }

                if (Destination.ExitsFor(perceiver, true).Any(x => x.IsFallExit))
                {
                    return (RoomMovementTransition.FallExit, perceiver.RoomLayer);
                }

                return (RoomMovementTransition.SwimToLand, perceiver.RoomLayer);
            }

            if (Destination.Location.IsSwimmingLayer(perceiver.RoomLayer))
            {
                if (!Destination.Location.IsUnderwaterLayer(perceiver.RoomLayer) &&
                    perceiver.PositionState == PositionFlying.Instance)
                {
                    return (RoomMovementTransition.FlyOnly, perceiver.RoomLayer);
                }

                return (RoomMovementTransition.SwimOnly, perceiver.RoomLayer);
            }

            if (Destination.ExitsFor(perceiver, true).Any(x => x.IsFallExit))
            {
                return (RoomMovementTransition.FallExit, perceiver.RoomLayer);
            }

            if (perceiver.PositionState == PositionFlying.Instance)
            {
                return (RoomMovementTransition.FlyOnly, perceiver.RoomLayer);
            }

            switch (perceiver.RoomLayer)
            {
                case RoomLayer.GroundLevel:
                    return (RoomMovementTransition.GroundToGround, RoomLayer.GroundLevel);
                case RoomLayer.InTrees:
                    return (RoomMovementTransition.TreesToTrees, RoomLayer.InTrees);
                case RoomLayer.HighInTrees:
                    return (RoomMovementTransition.TreesToTrees, RoomLayer.HighInTrees);
                case RoomLayer.OnRooftops:
                    return (RoomMovementTransition.TreesToTrees, RoomLayer.OnRooftops);
            }
        }

        return (RoomMovementTransition.NoViableTransition, RoomLayer.GroundLevel);
    }

    #region IFutureProgVariable Members

    public IProgVariable GetProperty(string property)
    {
        switch (property.ToLowerInvariant())
        {
            case "origin":
                return Origin;
            case "destination":
                return Destination;
            case "direction":
                return new TextVariable(OutboundDirection.Describe());
            case "keyword":
                if (this is NonCardinalRoomExit nce)
                {
                    return new TextVariable(nce.PrimaryKeyword);
                }

                return new TextVariable(OutboundDirection.DescribeBrief());
            case "opposite":
                return Exit.RoomExitFor(Destination);
            case "door":
                return Exit.Door?.Parent;
            case "acceptsdoor":
                return new BooleanVariable(Exit.AcceptsDoor);
            case "isclimbexit":
                return new BooleanVariable(IsClimbExit);
            case "isfallexit":
                return new BooleanVariable(IsFallExit);
            case "isflyexit":
                return new BooleanVariable(IsFlyExit);
            case "climbdifficulty":
                return new NumberVariable((int)ClimbDifficulty);
            case "outboundmovementsuffix":
                return new TextVariable(OutboundMovementSuffix);
            case "inboundmovementsuffix":
                return new TextVariable(InboundMovementSuffix);
            case "outbounddirectionsuffix":
                return new TextVariable(OutboundDirectionSuffix);
            case "inbounddirectionsuffix":
                return new TextVariable(InboundDirectionSuffix);
            case "outbounddirectiondescription":
                return new TextVariable(OutboundDirectionDescription);
            case "doorsize":
                return new NumberVariable((int)Exit.DoorSize);
            case "maximumsize":
                return new NumberVariable((int)Exit.MaximumSizeToEnter);
            case "maximumsizeupright":
                return new NumberVariable((int)Exit.MaximumSizeToEnterUpright);
            case "slowdown":
                return new NumberVariable(Exit.TimeMultiplier);
        }

        throw new NotSupportedException("Invalid Property in CellExit.GetProperty");
    }

    public ProgVariableTypes Type => ProgVariableTypes.Exit;

    public object GetObject => this;

    private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
    {
        return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "origin", ProgVariableTypes.Location },
            { "destination", ProgVariableTypes.Location },
            { "keyword", ProgVariableTypes.Text },
            { "direction", ProgVariableTypes.Text },
            { "opposite", ProgVariableTypes.Exit },
            { "door", ProgVariableTypes.Item },
            { "acceptsdoor", ProgVariableTypes.Boolean },
            { "isclimbexit", ProgVariableTypes.Boolean },
            { "isfallexit", ProgVariableTypes.Boolean },
            { "isflyexit", ProgVariableTypes.Boolean },
            { "climbdifficulty", ProgVariableTypes.Number },
            { "outboundmovementsuffix", ProgVariableTypes.Text },
            { "inboundmovementsuffix", ProgVariableTypes.Text },
            { "outbounddirectionsuffix", ProgVariableTypes.Text },
            { "inbounddirectionsuffix", ProgVariableTypes.Text },
            { "outbounddirectiondescription", ProgVariableTypes.Text },
            { "doorsize", ProgVariableTypes.Number },
            { "maximumsize", ProgVariableTypes.Number },
            { "maximumsizeupright", ProgVariableTypes.Number },
            { "slowdown", ProgVariableTypes.Number }
        };
    }

    private static IReadOnlyDictionary<string, string> DotReferenceHelp()
    {
        return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "origin", "The origin room for this exit" },
            { "destination", "The destination room for this exit" },
            {
                "keyword",
                "The best keyword for targeting the exit. A direction for cardinal exits and a keyword for non cardinal exits."
            },
            { "direction", "A text conversion of the cardinal direction" },
            { "opposite", "The corresponding exit from the destination room's perspective" },
            { "door", "If not null, the door item in this exit" },
            { "acceptsdoor", "True if this exit can have a door" },
            { "isclimbexit", "True if this exit is a climb exit" },
            { "isfallexit", "True if this exit is a fall exit" },
            { "isflyexit", "True if this exit is a fly exit" },
            { "climbdifficulty", "The numerical conversion of the difficulty of climbing" },
            { "outboundmovementsuffix", "e.g. out towards the East" },
            { "inboundmovementsuffix", "e.g. in from the East" },
            { "outbounddirectionsuffix", "e.g. to the East" },
            { "inbounddirectionsuffix", "e.g. from the East" },
            { "outbounddirectiondescription", "e.g. the North" },
            { "doorsize", "A numerical representation of the size of door that go in this exit" },
            { "maximumsize", "A numerical representation of the maximum size for someone to go through an exit" },
            {
                "maximumsizeupright",
                "A numerical representation of the maximum size for someone to go through an exit while standing"
            },
            { "slowdown", "The speed multiplier of the exit" }
        };
    }

    public static void RegisterFutureProgCompiler()
    {
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Exit, DotReferenceHandler(),
            DotReferenceHelp());
    }

    #endregion
}
