using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Construction.Boundary;

public class CellExit : ICellExit
{
	protected Lazy<List<string>> _keywords;

	public CellExit(IExit parent, MudSharp.Models.Exit exit, bool firstCellExit)
	{
		Exit = parent;
		Origin = parent.Gameworld.Cells.Get(firstCellExit ? exit.CellId1 : exit.CellId2);
		Destination = parent.Gameworld.Cells.Get(firstCellExit ? exit.CellId2 : exit.CellId1);
		InboundDirection = (CardinalDirection)(firstCellExit ? exit.Direction2 : exit.Direction1);
		OutboundDirection = (CardinalDirection)(firstCellExit ? exit.Direction1 : exit.Direction2);
		_keywords = new Lazy<List<string>>(() => new List<string> { OutboundDirection.Describe().ToLowerInvariant() });
	}

	public CellExit(IExit parent, ICellExit exit, ICell origin, ICell destination)
	{
		Exit = parent;
		Origin = origin;
		Destination = destination;
		InboundDirection = exit.InboundDirection;
		OutboundDirection = exit.OutboundDirection;
		_keywords = new Lazy<List<string>>(() => new List<string> { OutboundDirection.Describe().ToLowerInvariant() });
	}

	public override string ToString()
	{
		return $"CellExit {OutboundDirection.Describe()} to {Destination.Name} ({Destination.Id:N0})";
	}

	public IExit Exit { get; protected set; }

	public ICell Origin { get; protected set; }

	public IEnumerable<RoomLayer> WhichLayersExitAppears()
	{
		var layers = new List<RoomLayer>();
		var originLayers = Origin.Terrain(null).TerrainLayers;

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

		var destinationLayers = Destination.Terrain(null).TerrainLayers;
		foreach (var layer in originLayers)
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

	public ICell Destination { get; protected set; }

	public CardinalDirection OutboundDirection { get; protected set; }

	public CardinalDirection InboundDirection { get; protected set; }

	public ICellExit Opposite => Exit.CellExitFor(Destination);

	public virtual string OutboundMovementSuffix => "away towards the " + OutboundDirection.Describe();

	public virtual string InboundMovementSuffix => "in from the " + InboundDirection.Describe();

	public virtual string OutboundDirectionSuffix => "from the " + OutboundDirection.Describe();

	public virtual string InboundDirectionSuffix => "from the " + InboundDirection.Describe();

	public virtual string OutboundDirectionDescription => "the " + OutboundDirection.Describe();

	public bool IsFallExit => Exit.FallCell == Destination;
	public bool IsFlyExit => Exit.FallCell == Origin && !IsClimbExit;
	public bool IsClimbExit => Exit.IsClimbExit;
	public Difficulty ClimbDifficulty => Exit.ClimbDifficulty;

	public virtual string DescribeFor(IPerceiver voyeur, bool colour)
	{
		var startColourString = "";
		var endColourString = "";
		var (transition, _) = MovementTransition(voyeur);
		switch (transition)
		{
			case CellMovementTransition.SwimOnly:
				startColourString = Telnet.BoldBlue.ToString();
				endColourString = Telnet.RESET + Telnet.Green.ToString();
				break;
			case CellMovementTransition.FallExit:
				if (IsClimbExit)
				{
					startColourString = Telnet.Yellow.ToString();
					endColourString = Telnet.Green.ToString();
					break;
				}

				startColourString = Telnet.Red.ToString();
				endColourString = Telnet.Green.ToString();
				break;
			case CellMovementTransition.FlyOnly:
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

		var dirString = OutboundDirection.Describe();
		var doorString = Exit.Door != null
			? $" ({Exit.Door.State.Describe().ToLowerInvariant()} {Exit.Door.InstalledExitDescription(voyeur)})"
			: "";
		return $"{startColourString}{dirString}{doorString}{endColourString}";
	}

	public virtual string BuilderInformationString(IPerceiver voyeur)
	{
		return string.Format("Cardinal {4}{5}Exit #{0:N0} - {1} to {3} at x{2:N} speed", Exit.Id,
			OutboundDirection.Describe(), Exit.TimeMultiplier, Destination.HowSeen(voyeur, colour: false),
			IsFallExit ? "Fall " : "", IsClimbExit ? $"{ClimbDifficulty.Describe()} Climb " : "");
	}

	public virtual bool IsExit(string verb)
	{
		return CardinalDirectionExtensions.CardinalExitStrings.ContainsKey(verb) &&
		       CardinalDirectionExtensions.CardinalExitStrings[verb] == OutboundDirection;
	}

	public virtual bool IsExitKeyword(string keyword)
	{
		return IsExit(keyword);
	}

	public IEnumerable<string> Keywords => _keywords.Value;

	public virtual bool HasKeyword(string targetKeyword, IPerceiver voyeur, bool abbreviated = true)
	{
		return abbreviated
			? _keywords.Value.Any(x => x.StartsWith(targetKeyword, StringComparison.Ordinal))
			: _keywords.Value.Contains(targetKeyword);
	}

	public virtual bool HasKeywords(IEnumerable<string> targetKeywords, IPerceiver voyeur, bool abbreviated = true)
	{
		return targetKeywords.All(x => HasKeyword(x, voyeur, abbreviated));
	}


	public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return _keywords.Value;
	}

	public (CellMovementTransition TransitionType, RoomLayer TargetLayer) MovementTransition(IPerceiver perceiver)
	{
		if (!WhichLayersExitAppears().Contains(perceiver.RoomLayer))
		{
			return (CellMovementTransition.NoViableTransition, RoomLayer.GroundLevel);
		}

		var originTerrain = Origin.Terrain(perceiver);
		var destinationTerrain = Destination.Terrain(perceiver);
		if (OutboundDirection == CardinalDirection.Up)
		{
			if (originTerrain.TerrainLayers.HighestLayer() != perceiver.RoomLayer)
			{
				return (CellMovementTransition.NoViableTransition, RoomLayer.GroundLevel);
			}

			if (perceiver.RoomLayer.IsUnderwater())
			{
				return (CellMovementTransition.SwimOnly, destinationTerrain.TerrainLayers.LowestLayer());
			}

			return (CellMovementTransition.FlyOnly, destinationTerrain.TerrainLayers.LowestLayer());
		}

		if (OutboundDirection == CardinalDirection.Down)
		{
			if (originTerrain.TerrainLayers.LowestLayer() != perceiver.RoomLayer)
			{
				return (CellMovementTransition.NoViableTransition, RoomLayer.GroundLevel);
			}

			if (perceiver.RoomLayer.IsUnderwater())
			{
				if (destinationTerrain.TerrainLayers.Any(x => !x.IsUnderwater()))
				{
					return (CellMovementTransition.FallExit, destinationTerrain.TerrainLayers.HighestLayer());
				}

				return (CellMovementTransition.SwimOnly, destinationTerrain.TerrainLayers.HighestLayer());
			}

			return (CellMovementTransition.FallExit, destinationTerrain.TerrainLayers.HighestLayer());
		}

		if (destinationTerrain.TerrainLayers.Contains(perceiver.RoomLayer))
		{
			if (perceiver.Location.IsSwimmingLayer(perceiver.RoomLayer))
			{
				if (Destination.Location.IsSwimmingLayer(perceiver.RoomLayer))
				{
					return (CellMovementTransition.SwimOnly, perceiver.RoomLayer);
				}

				if (!Destination.Location.IsUnderwaterLayer(perceiver.RoomLayer) &&
				    perceiver.PositionState == PositionFlying.Instance)
				{
					return (CellMovementTransition.FlyOnly, perceiver.RoomLayer);
				}

				if (destinationTerrain.TerrainLayers.Any(x => Destination.Location.IsSwimmingLayer(x)))
				{
					return (CellMovementTransition.FallExit, perceiver.RoomLayer);
				}

				if (Destination.ExitsFor(perceiver, true).Any(x => x.IsFallExit))
				{
					return (CellMovementTransition.FallExit, perceiver.RoomLayer);
				}

				return (CellMovementTransition.SwimToLand, perceiver.RoomLayer);
			}

			if (Destination.Location.IsSwimmingLayer(perceiver.RoomLayer))
			{
				if (!Destination.Location.IsUnderwaterLayer(perceiver.RoomLayer) &&
				    perceiver.PositionState == PositionFlying.Instance)
				{
					return (CellMovementTransition.FlyOnly, perceiver.RoomLayer);
				}

				return (CellMovementTransition.SwimOnly, perceiver.RoomLayer);
			}

			if (Destination.ExitsFor(perceiver, true).Any(x => x.IsFallExit))
			{
				return (CellMovementTransition.FallExit, perceiver.RoomLayer);
			}

			if (perceiver.PositionState == PositionFlying.Instance)
			{
				return (CellMovementTransition.FlyOnly, perceiver.RoomLayer);
			}

			switch (perceiver.RoomLayer)
			{
				case RoomLayer.GroundLevel:
					return (CellMovementTransition.GroundToGround, RoomLayer.GroundLevel);
				case RoomLayer.InTrees:
					return (CellMovementTransition.TreesToTrees, RoomLayer.InTrees);
				case RoomLayer.HighInTrees:
					return (CellMovementTransition.TreesToTrees, RoomLayer.HighInTrees);
				case RoomLayer.OnRooftops:
					return (CellMovementTransition.TreesToTrees, RoomLayer.OnRooftops);
			}
		}

		return (CellMovementTransition.NoViableTransition, RoomLayer.GroundLevel);
	}

	#region IFutureProgVariable Members

	public IFutureProgVariable GetProperty(string property)
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
				if (this is NonCardinalCellExit nce)
				{
					return new TextVariable(nce.PrimaryKeyword);
				}

				return new TextVariable(OutboundDirection.DescribeBrief());
			case "opposite":
				return Exit.CellExitFor(Destination);
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

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Exit;

	public object GetObject => this;

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "origin", FutureProgVariableTypes.Location },
			{ "destination", FutureProgVariableTypes.Location },
			{ "keyword", FutureProgVariableTypes.Text },
			{ "direction", FutureProgVariableTypes.Text },
			{ "opposite", FutureProgVariableTypes.Exit },
			{ "door", FutureProgVariableTypes.Item },
			{ "acceptsdoor", FutureProgVariableTypes.Boolean },
			{ "isclimbexit", FutureProgVariableTypes.Boolean },
			{ "isfallexit", FutureProgVariableTypes.Boolean },
			{ "isflyexit", FutureProgVariableTypes.Boolean },
			{ "climbdifficulty", FutureProgVariableTypes.Number },
			{ "outboundmovementsuffix", FutureProgVariableTypes.Text },
			{ "inboundmovementsuffix", FutureProgVariableTypes.Text },
			{ "outbounddirectionsuffix", FutureProgVariableTypes.Text },
			{ "inbounddirectionsuffix", FutureProgVariableTypes.Text },
			{ "outbounddirectiondescription", FutureProgVariableTypes.Text },
			{ "doorsize", FutureProgVariableTypes.Number },
			{ "maximumsize", FutureProgVariableTypes.Number },
			{ "maximumsizeupright", FutureProgVariableTypes.Number },
			{ "slowdown", FutureProgVariableTypes.Number }
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
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Exit, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}