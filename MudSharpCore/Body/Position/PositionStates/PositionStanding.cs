using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionStanding : PositionState
{
	protected static PositionStanding _instance = new();

	private PositionStanding()
	{
		_id = 1;
		_name = "Standing";
	}

	public static PositionStanding Instance => _instance;
	public override string DescribeLocationMovementParticiple => "standing";
	public override bool Upright => true;

	public override MovementAbility MoveRestrictions => MovementAbility.Free;

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
		if (originalState == PositionFlying.Instance)
		{
			text = $"land|lands {positionee.RoomLayer.PositionalDescription()}";
		}
		else if (originalState == PositionClimbing.Instance)
		{
			text = "stop|stops climbing";
		}
		else
		{
			switch (CompareTo(originalState))
			{
				case PositionHeightComparison.Equivalent:
					text = "stand|stands";
					break;
				case PositionHeightComparison.Higher:
					text = "stand|stands up";
					break;
				case PositionHeightComparison.Lower:
					text = "return|returns to a standing position";
					break;
				default:
					text = "stand|stands";
					break;
			}
		}

		return DressTransition(positionee, text, originalState, originalModifier, newModifier, originalTarget,
			newTarget);
	}

	public override string DefaultDescription()
	{
		return "standing";
	}
}