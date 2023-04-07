using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionStandingEasy : PositionState
{
	protected static PositionStandingEasy _instance = new();

	private PositionStandingEasy()
	{
		_id = 10;
		_name = "Standing at Ease";
	}

	public static PositionStandingEasy Instance => _instance;
	public override string DescribeLocationMovementParticiple => "standing at ease";
	public override bool Upright => true;

	public override IPositionState TransitionOnMovement => PositionStanding.Instance;

	public override MovementAbility MoveRestrictions => MovementAbility.Free;

	public override string DescribePositionMovement => "step|steps";

	public override string DefaultDescription()
	{
		return "standing easy";
	}

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		return DressTransition(positionee, "stand|stands easy", originalState, originalModifier, newModifier,
			originalTarget, newTarget);
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
}