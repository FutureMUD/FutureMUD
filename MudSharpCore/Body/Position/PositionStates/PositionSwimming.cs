using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionSwimming : PositionState
{
	public static PositionSwimming Instance => _instance;
	protected static PositionSwimming _instance = new();

	private PositionSwimming()
	{
		_id = 16;
		_name = "Swimming";
	}

	public override bool Upright => true;
	public override string DescribeLocationMovementParticiple => "swimming";
	public override string DescribePositionMovement => "swim|swims";

	public override string DefaultDescription()
	{
		return "swimming";
	}

	public override MovementAbility MoveRestrictions => MovementAbility.Swimming;

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		if (originalState == PositionFlying.Instance)
		{
			return DressTransition(positionee, "land|lands in the water and begin|begins to swim", originalState,
				originalModifier, newModifier, originalTarget, newTarget);
		}

		if (positionee.RoomLayer == Construction.RoomLayer.GroundLevel)
		{
			return DressTransition(positionee, "wade|wades into the water", originalState, originalModifier,
				newModifier, originalTarget,
				newTarget);
		}

		return DressTransition(positionee, "begin|begins to swim", originalState, originalModifier, newModifier,
			originalTarget,
			newTarget);
	}

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

	public override PositionHeightComparison CompareTo(PositionSwimming state)
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
		return PositionHeightComparison.Lower;
	}

	public override PositionHeightComparison CompareTo(PositionStandingEasy state)
	{
		return PositionHeightComparison.Lower;
	}
}