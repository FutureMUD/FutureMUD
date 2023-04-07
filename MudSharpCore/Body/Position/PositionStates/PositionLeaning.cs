using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionLeaning : PositionState
{
	protected static PositionLeaning _instance = new();

	private PositionLeaning()
	{
		_id = 11;
		_name = "Leaning";
	}

	public static PositionLeaning Instance => _instance;

	public override bool Upright => true;
	public override string DescribeLocationMovementParticiple => "leaning";
	public override MovementAbility MoveRestrictions => MovementAbility.Free;

	#region Overrides of IPositionState

	/// <summary>
	///     If the IPositionable moves while in this state, which state they should transition to
	/// </summary>
	public override IPositionState TransitionOnMovement => PositionStanding.Instance;

	#endregion

	public override string DescribePositionMovement => "step|steps";

	public override PositionHeightComparison CompareTo(PositionKneeling state)
	{
		return PositionHeightComparison.Higher;
	}

	public override PositionHeightComparison CompareTo(PositionProne state)
	{
		return PositionHeightComparison.Higher;
	}

	public override PositionHeightComparison CompareTo(PositionSitting state)
	{
		return PositionHeightComparison.Higher;
	}

	public override PositionHeightComparison CompareTo(PositionStanding state)
	{
		return PositionHeightComparison.Equivalent;
	}

	public override PositionHeightComparison CompareTo(PositionLeaning state)
	{
		return PositionHeightComparison.Equivalent;
	}

	public override PositionHeightComparison CompareTo(PositionLyingDown state)
	{
		return PositionHeightComparison.Higher;
	}

	public override PositionHeightComparison CompareTo(PositionSprawled state)
	{
		return PositionHeightComparison.Higher;
	}

	public override PositionHeightComparison CompareTo(PositionProstrate state)
	{
		return PositionHeightComparison.Higher;
	}

	public override PositionHeightComparison CompareTo(PositionStandingAttention state)
	{
		return PositionHeightComparison.Equivalent;
	}

	public override PositionHeightComparison CompareTo(PositionStandingEasy state)
	{
		return PositionHeightComparison.Equivalent;
	}

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		string text;
		switch (CompareTo(originalState))
		{
			case PositionHeightComparison.Equivalent:
			case PositionHeightComparison.Lower:
				text = "lean|leans";
				break;
			case PositionHeightComparison.Higher:
				text = "stand|stands into a lean";
				break;
			default:
				text = "lean|leans";
				break;
		}

		return DressTransition(positionee, text, originalState, originalModifier, newModifier, originalTarget,
			newTarget);
	}

	public override string DefaultDescription()
	{
		return "leaning";
	}

	#region Overrides of IPositionState

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
	public override string Describe(IPerceiver voyeur, IPerceivable target, PositionModifier modifier, IEmote emote,
		bool useHere = true)
	{
		if (target != null && modifier == PositionModifier.None)
		{
			return $"leaning against {target.HowSeen(voyeur)}{(emote != null ? $", {emote.ParseFor(voyeur)}" : "")}";
		}

		return base.Describe(voyeur, target, modifier, emote, useHere);
	}

	#endregion
}