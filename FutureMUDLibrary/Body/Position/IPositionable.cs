using System.Collections.Generic;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Body.Position {
    /// <summary>
    ///     An IPositionable is something that can be given an RPG position relative to another in the same ILocation
    /// </summary>
    public interface IPositionable {
        IPositionState PositionState { get; set; }
        IPerceivable PositionTarget { get; set; }
        PositionModifier PositionModifier { get; set; }
        IEmote PositionEmote { get; }

        IEnumerable<IPerceivable> TargetedBy { get; }

        /// <summary>
        ///     A collection of all currently valid positions for this IPositionable
        /// </summary>
        IEnumerable<IPositionState> ValidPositions { get; }

        string DescribePosition(IPerceiver voyeur, bool useHere = true);

        bool InVicinity(IPerceivable target);
        IEnumerable<(IPerceivable Thing, Proximity Proximity)> LocalThingsAndProximities();
        void SetPosition(IPositionState state, PositionModifier modifier, IPerceivable target, IEmote emote);
        void SetState(IPositionState state);
        void SetTarget(IPerceivable target);
        void SetModifier(PositionModifier modifier);
        void SetEmote(IEmote emote);
        void SetTargetedBy(IPerceivable targeter);
        void RemoveTargetedBy(IPerceivable targeter);

        /// <summary>
        ///     Determines whether another IPositionable can use this IPositionable as a target in the specified way
        /// </summary>
        /// <param name="whichPosition">The proposed position against this IPositionable</param>
        /// <param name="modifier">The proposed modifier against this IPositionable</param>
        /// <returns>True if the calling IPositionable can legally be positioned in that way against this IPositionable</returns>
        bool CanBePositionedAgainst(IPositionState whichPosition, PositionModifier modifier);

        event PerceivableEvent OnPositionChanged;
    }
}