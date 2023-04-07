using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using System.Collections.Generic;

namespace MudSharp.Construction.Boundary {
    public enum CellMovementTransition
    {
        NoViableTransition,
        GroundToGround,
        TreesToTrees,
        FlyOnly,
        SwimOnly,
        FallExit,
        SwimToLand,   
    }

    public interface INonCardinalCellExit : ICellExit
    {
	    string Verb { get; }
	    string PrimaryKeyword { get; }
	    string InboundDescription { get; }
	    string InboundTarget { get; }
	    string OutboundDescription { get; }
	    string OutboundTarget { get; }
    }

    /// <summary>
    ///     A cell exit is the one-sided implementation of a particular IExit
    /// </summary>
    public interface ICellExit : IKeyworded, IFutureProgVariable {
        IExit Exit { get; }
        ICell Origin { get; }
        ICell Destination { get; }
        ICellExit Opposite { get; }

        CardinalDirection OutboundDirection { get; }
        CardinalDirection InboundDirection { get; }

        /// <summary>
        ///     In the style of "out towards the street", or "out to the east"
        /// </summary>
        string OutboundMovementSuffix { get; }

        /// <summary>
        ///     In the style of "in from the street", or "in from the west"
        /// </summary>
        string InboundMovementSuffix { get; }

        /// <summary>
        ///     In the style of "from the street" or "from the north"
        /// </summary>
        string OutboundDirectionSuffix { get; }

        /// <summary>
        ///     In the style of "from the street" or "from the south"
        /// </summary>
        string InboundDirectionSuffix { get; }

        /// <summary>
        ///     In the style of "the north" or "the Shaggy Duck Tavern"
        /// </summary>
        string OutboundDirectionDescription { get; }

        string DescribeFor(IPerceiver voyeur, bool colour = true);
        string BuilderInformationString(IPerceiver voyeur);
        bool IsExit(string verb);
        bool IsExitKeyword(string keyword);
        bool IsFallExit { get; }
        bool IsFlyExit { get; }
        bool IsClimbExit { get; }
        Difficulty ClimbDifficulty { get; }
        (CellMovementTransition TransitionType, RoomLayer TargetLayer) MovementTransition(IPerceiver perceiver);
        IEnumerable<RoomLayer> WhichLayersExitAppears();
    }
}