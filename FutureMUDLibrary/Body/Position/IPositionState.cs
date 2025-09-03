using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Body.Position
{
    public enum PositionModifier {
        In,
        On,
        Under,
        Before,
        Behind,
        None,
        Around
    }

    public enum PositionHeightComparison {
        Lower,
        Higher,
        Equivalent,
        Undefined
    }

    public enum MovementAbility {
        Free,
        FreeIfNotInOn,
        Restricted,
        Climbing,
        Swimming,
        Flying
    }

    public interface IPositionState : IFrameworkItem
    {
        /// <summary>
        ///     Whether or not this position is considered Upright
        /// </summary>
        bool Upright { get; }

        bool IgnoreTerrainStaminaCostsForMovement { get; }

        /// <summary>
        ///     What sort of restrictions being in this position state imposes upon Movement
        /// </summary>
        MovementAbility MoveRestrictions { get; }

        /// <summary>
        ///     If the IPositionable moves while in this state, which state they should transition to
        /// </summary>
        IPositionState TransitionOnMovement { get; }

        /// <summary>
        ///     Returns a string in the form "1st|3rd" that describes the motion when one moves in this position intralocally
        /// </summary>
        string DescribePositionMovement { get; }

        string DescribeLocationMovement3rd { get; }
        string DescribeLocationMovementParticiple { get; }
        bool SafeFromFalling { get; }

        /// <summary>
        ///     This function allows the various position states to add a unique description suffix to any IPositionable, for
        ///     instance, the PositionStanding position state might by default return an "standing here" fragment
        ///     Punctuation is omitted so it can be used in any way the client likes.
        ///     The verb form of "to be" is omitted
        ///     Override this function on a state if it has some specific combination of targets/modifiers that is different,
        ///     otherwise just use base
        /// </summary>
        /// <param name="voyeur"></param>
        /// <param name="target">A target IDescribable that the position refers to. For example, a table</param>
        /// <param name="modifier">An enum representing a modifier to the target, having unique meanings</param>
        /// <param name="emote"></param>
        /// <returns></returns>
        string Describe(IPerceiver voyeur, IPerceivable target, PositionModifier modifier, IEmote emote,
            bool useHere = true);

        string DefaultDescription();

        IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
            PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
            IPerceivable newTarget);

        /// <summary>
        ///     Returns an enum description of the height of the current position as compared to the state. A result of Higher
        ///     implies that this state is HIGHER than the compared state, for instance.
        /// </summary>
        /// <param name="state">The state to compare to the current state</param>
        /// <returns>An enum representing the result of the comparison</returns>
        PositionHeightComparison CompareTo(dynamic state);
    }
}