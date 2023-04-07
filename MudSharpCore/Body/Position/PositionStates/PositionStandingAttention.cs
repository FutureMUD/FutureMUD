using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionStandingAttention : PositionState
{
	protected static PositionStandingAttention _instance = new();

	private PositionStandingAttention()
	{
		_id = 9;
		_name = "Standing at Attention";
	}

	public static PositionStandingAttention Instance => _instance;

	public override bool Upright => true;
	public override string DescribeLocationMovementParticiple => "standing at attention";
	public override IPositionState TransitionOnMovement => PositionStanding.Instance;

	public override MovementAbility MoveRestrictions => MovementAbility.Free;

	public override string DescribePositionMovement => "step|steps";

	public override string DefaultDescription()
	{
		return "standing at attention";
	}

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		return DressTransition(positionee, "stand|stands to attention", originalState, originalModifier, newModifier,
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