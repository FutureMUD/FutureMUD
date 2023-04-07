using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionProstrate : PositionState
{
	protected static PositionProstrate _instance = new();

	private PositionProstrate()
	{
		_id = 7;
		_name = "Prostrate";
	}

	public static PositionProstrate Instance => _instance;
	public override string DescribeLocationMovementParticiple => "prostrate";
	public override MovementAbility MoveRestrictions => MovementAbility.FreeIfNotInOn;

	public override string DescribePositionMovement => "crawl|crawls";

	public override string DefaultDescription()
	{
		return "prostrate";
	}

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		string text;
		switch (CompareTo(originalState))
		{
			case PositionHeightComparison.Equivalent:
				text = "become|becomes prostrate";
				break;
			case PositionHeightComparison.Lower:
				text = "fall|falls prostrate";
				break;
			case PositionHeightComparison.Higher:
				text = "rise|rises to a prostrate position";
				break;
			default:
				text = "become|becomes prostrate";
				break;
		}

		return DressTransition(positionee, text, originalState, originalModifier, newModifier, originalTarget,
			newTarget);
	}

	public override PositionHeightComparison CompareTo(PositionSitting state)
	{
		return PositionHeightComparison.Higher;
	}

	public override PositionHeightComparison CompareTo(PositionStanding state)
	{
		return PositionHeightComparison.Lower;
	}

	public override PositionHeightComparison CompareTo(PositionLeaning state)
	{
		return PositionHeightComparison.Lower;
	}

	public override PositionHeightComparison CompareTo(PositionKneeling state)
	{
		return PositionHeightComparison.Lower;
	}

	public override PositionHeightComparison CompareTo(PositionProne state)
	{
		return PositionHeightComparison.Higher;
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
		return PositionHeightComparison.Equivalent;
	}
}