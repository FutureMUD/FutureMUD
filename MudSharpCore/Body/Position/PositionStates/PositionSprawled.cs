using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionSprawled : PositionState
{
	protected static PositionSprawled _instance = new();

	private PositionSprawled()
	{
		_id = 8;
		_name = "Sprawled";
	}

	public static PositionSprawled Instance => _instance;
	public override string DescribeLocationMovementParticiple => "sprawled";

	public override string DefaultDescription()
	{
		return "sprawled";
	}

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		return DressTransition(positionee, "sprawl|sprawls out", originalState, originalModifier, newModifier,
			originalTarget, newTarget);
	}

	public override PositionHeightComparison CompareTo(PositionSprawled state)
	{
		return PositionHeightComparison.Equivalent;
	}

	public override PositionHeightComparison CompareTo(dynamic state)
	{
		return PositionHeightComparison.Lower;
	}
}