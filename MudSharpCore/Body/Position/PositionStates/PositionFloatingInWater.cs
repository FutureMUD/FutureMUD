using System;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionFloatingInWater : PositionState
{
	protected static PositionFloatingInWater _instance = new();

	private PositionFloatingInWater()
	{
		_id = 17;
		_name = "Floating";
	}

	public static PositionFloatingInWater Instance => _instance;

	public override string DescribeLocationMovementParticiple => "floating";

	public override string Describe(IPerceiver voyeur, IPerceivable target, PositionModifier modifier, IEmote emote,
		bool useHere = true)
	{
		var emoteText = emote != null ? $", {emote.ParseFor(voyeur)}" : "";
		if (target == null)
		{
			return $"floating {(useHere ? "here" : "")}{emoteText}";
		}

		switch (modifier)
		{
			case PositionModifier.Before:
				return $"floating {(useHere ? "here" : "")} before {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.Behind:
				return $"floating {(useHere ? "here" : "")} behind {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.In:
				return $"floating {(useHere ? "here" : "")} in {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.On:
				return $"floating {(useHere ? "here" : "")} on {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.Under:
				return $"floating {(useHere ? "here" : "")} under {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.Around:
				return $"floating {(useHere ? "here" : "")} around {target.HowSeen(voyeur)}{emoteText}";
			default:
				return $"floating {(useHere ? "here" : "")} by {target.HowSeen(voyeur)}{emoteText}";
		}
	}

	public override string DefaultDescription()
	{
		return "";
	}

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		throw new NotImplementedException(
			"This should never happen. PositionFloatingInWater should only be set by functions that do not call DescribeTransition.");
	}
}