using System;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionHanging : PositionState
{
	protected static PositionHanging _instance = new();

	private PositionHanging()
	{
		_id = 13;
		_name = "Hanging";
	}

	public static PositionHanging Instance => _instance;

	public override string DescribeLocationMovementParticiple => "hanging";

	public override string Describe(IPerceiver voyeur, IPerceivable target, PositionModifier modifier, IEmote emote,
		bool useHere = true)
	{
		var emoteText = emote != null ? $", {emote.ParseFor(voyeur)}" : "";
		if (target == null)
		{
			return $"hanging{(useHere ? " here" : "")}{emoteText}";
		}

		switch (modifier)
		{
			case PositionModifier.Before:
				return $"hanging{(useHere ? " here" : "")}{target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.Behind:
				return $"hanging{(useHere ? " here" : "")}behind {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.In:
				return $"hanging{(useHere ? " here" : "")} in {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.On:
				return $"hanging{(useHere ? " here" : "")} on {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.Under:
				return $"hanging{(useHere ? " here" : "")} under {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.Around:
				return $"hanging{(useHere ? " here" : "")} around {target.HowSeen(voyeur)}{emoteText}";
			default:
				return $"hanging from {target.HowSeen(voyeur)}{emoteText}";
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
			"This should never happen. PositionHanging should only be set by functions that do not call DescribeTransition.");
	}
}