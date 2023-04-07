using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionSquatting : PositionState
{
	protected static PositionSquatting _instance = new();

	private PositionSquatting()
	{
		_id = 14;
		_name = "Squatting";
	}

	public static PositionSquatting Instance => _instance;
	public override string DescribeLocationMovementParticiple => "squatting";
	public override bool Upright => true;

	public override IPositionState TransitionOnMovement => PositionStanding.Instance;

	public override MovementAbility MoveRestrictions => MovementAbility.Free;

	public override string DescribePositionMovement => "step|steps";

	public override string DefaultDescription()
	{
		return "squatting";
	}

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		return DressTransition(positionee, "squat|squats", originalState, originalModifier, newModifier,
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

	public override PositionHeightComparison CompareTo(PositionStandingEasy state)
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