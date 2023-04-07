using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionFlying : PositionState
{
	public static PositionFlying Instance => _instance;
	protected static PositionFlying _instance = new();

	private PositionFlying()
	{
		_id = 18;
		_name = "Flying";
	}

	public override bool Upright => true;

	public override string DescribePositionMovement => "fly|flies";

	public override string DescribeLocationMovementParticiple => "flying";

	public override string DefaultDescription()
	{
		return "flying";
	}

	public override MovementAbility MoveRestrictions => MovementAbility.Flying;

	public override bool SafeFromFalling => true;

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier,
		PositionModifier newModifier, IPerceivable originalTarget, IPerceivable newTarget)
	{
		var text = "begin|begins to fly";
		if (originalState == PositionSwimming.Instance)
		{
			text = "fly|flies out of the water";
		}
		else if (originalState == PositionClimbing.Instance)
		{
			text = "let|lets go and begin|begins to fly";
		}

		return DressTransition(positionee, text, originalState, originalModifier, newModifier, originalTarget,
			newTarget);
	}
}