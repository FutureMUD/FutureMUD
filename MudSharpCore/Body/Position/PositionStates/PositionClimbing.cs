using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionClimbing : PositionState
{
	public static PositionClimbing Instance => _instance;
	protected static PositionClimbing _instance = new();

	private PositionClimbing()
	{
		_id = 15;
		_name = "Climbing";
	}

	public override bool Upright => true;

	public override string DescribePositionMovement => "climb|climbs";

	public override string DescribeLocationMovementParticiple => "climbing";

	public override string DefaultDescription()
	{
		return "climbing";
	}

	public override MovementAbility MoveRestrictions => MovementAbility.Climbing;

	public override IPositionState TransitionOnMovement => PositionStanding.Instance;

	public override bool SafeFromFalling => true;

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier,
		PositionModifier newModifier, IPerceivable originalTarget, IPerceivable newTarget)
	{
		var text = "begin|begins to climb";
		return DressTransition(positionee, text, originalState, originalModifier, newModifier, originalTarget,
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
		return PositionHeightComparison.Higher;
	}
}