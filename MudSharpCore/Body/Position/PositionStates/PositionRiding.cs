using System;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Body.Position.PositionStates;

public class PositionRiding : PositionState
{
	public static PositionRiding Instance => _instance;
	protected static PositionRiding _instance = new();

	private PositionRiding()
	{
		_id = 19;
		_name = "Riding";
	}

	public override bool Upright => true;

	public override string DescribePositionMovement => "ride|rides";

	public override string DescribeLocationMovementParticiple => "riding";

	public override string DefaultDescription()
	{
		return "riding";
	}

	public override MovementAbility MoveRestrictions => MovementAbility.Restricted;

	public override bool SafeFromFalling => true;

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier,
		PositionModifier newModifier, IPerceivable originalTarget, IPerceivable newTarget)
	{
		throw new NotImplementedException("This should never happen. PositionRiding should only be set by functions that do not call DescribeTransition.");
	}
}