using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionSlumped : PositionState
{
	protected static PositionSlumped _instance = new();

	private PositionSlumped()
	{
		_id = 12;
		_name = "Slumped";
	}

	public static PositionSlumped Instance => _instance;
	public override string DescribeLocationMovementParticiple => "slumped";

	public override string DefaultDescription()
	{
		return "slumped";
	}

	public override PositionHeightComparison CompareTo(PositionKneeling state)
	{
		return PositionHeightComparison.Lower;
	}

	public override PositionHeightComparison CompareTo(PositionProne state)
	{
		return PositionHeightComparison.Higher;
	}

	public override PositionHeightComparison CompareTo(PositionSitting state)
	{
		return PositionHeightComparison.Equivalent;
	}

	public override PositionHeightComparison CompareTo(PositionLounging state)
	{
		return PositionHeightComparison.Equivalent;
	}

	public override PositionHeightComparison CompareTo(PositionStanding state)
	{
		return PositionHeightComparison.Lower;
	}

	public override PositionHeightComparison CompareTo(PositionLeaning state)
	{
		return PositionHeightComparison.Lower;
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
		return PositionHeightComparison.Lower;
	}

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		string text;
		switch (CompareTo(originalState))
		{
			case PositionHeightComparison.Equivalent:
				text = "slump|slumps";
				break;
			case PositionHeightComparison.Lower:
				text = "sit|sits up into a slumped position";
				break;
			case PositionHeightComparison.Higher:
				text = "relax|relaxes into a slumping position";
				break;
			default:
				text = "slump|slumps";
				break;
		}

		return DressTransition(positionee, text, originalState, originalModifier, newModifier, originalTarget,
			newTarget);
	}

	public override string Describe(IPerceiver voyeur, IPerceivable target, PositionModifier modifier, IEmote emote,
		bool useHere = true)
	{
		if (target != null && modifier == PositionModifier.None)
		{
			return $"slumped against {target.HowSeen(voyeur)}{(emote != null ? $", {emote.ParseFor(voyeur)}" : "")}";
		}

		return base.Describe(voyeur, target, modifier, emote, useHere);
	}
}