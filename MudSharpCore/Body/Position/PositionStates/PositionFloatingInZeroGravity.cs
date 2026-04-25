using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionFloatingInZeroGravity : PositionState
{
	public static PositionFloatingInZeroGravity Instance => _instance;
	protected static PositionFloatingInZeroGravity _instance = new();

	private PositionFloatingInZeroGravity()
	{
		_id = 20;
		_name = "Floating in Zero Gravity";
	}

	public override bool Upright => true;

	public override string DescribePositionMovement => "float|floats";

	public override string DescribeLocationMovementParticiple => "floating";

	public override string DescribeLocationMovement3rd => "floating";

	public override string DefaultDescription()
	{
		return "floating";
	}

	public override MovementAbility MoveRestrictions => MovementAbility.ZeroGravity;

	public override bool SafeFromFalling => true;

	public override bool IgnoreTerrainStaminaCostsForMovement => true;

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		return DressTransition(positionee, "begin|begins to float", originalState, originalModifier, newModifier,
			originalTarget, newTarget);
	}
}
