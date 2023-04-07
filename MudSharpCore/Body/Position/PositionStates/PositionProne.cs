using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

/// <summary>
///     Prone implies that someone is lying down flat on their stomach
/// </summary>
public class PositionProne : PositionState
{
	protected static PositionProne _instance = new();

	private PositionProne()
	{
		_id = 6;
		_name = "Prone";
	}

	public static PositionProne Instance => _instance;
	public override string DescribeLocationMovementParticiple => "prone";
	public override MovementAbility MoveRestrictions => MovementAbility.Free;

	public override string DescribePositionMovement => "crawl|crawls";

	public override string DefaultDescription()
	{
		return "lying prone";
	}

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		string text;
		switch (CompareTo(originalState))
		{
			case PositionHeightComparison.Equivalent:
				text = "roll|rolls into a prone position";
				break;
			case PositionHeightComparison.Lower:
				text = "fall|falls prone";
				break;
			case PositionHeightComparison.Higher:
				text = "roll|rolls into a prone position";
				break;
			default:
				text = "lie|lies prone";
				break;
		}

		return DressTransition(positionee, text, originalState, originalModifier, newModifier, originalTarget,
			newTarget);
	}

	public override PositionHeightComparison CompareTo(PositionProne state)
	{
		return PositionHeightComparison.Equivalent;
	}

	public override PositionHeightComparison CompareTo(PositionKneeling state)
	{
		return PositionHeightComparison.Lower;
	}

	public override PositionHeightComparison CompareTo(PositionLyingDown state)
	{
		return PositionHeightComparison.Equivalent;
	}

	public override PositionHeightComparison CompareTo(PositionSitting state)
	{
		return PositionHeightComparison.Lower;
	}

	public override PositionHeightComparison CompareTo(PositionSprawled state)
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
}