using System;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

/// <summary>
///     PositionUndefined is used as a default case where no more specific position is appropriate, and would also be the
///     default position for "dropped" game items
/// </summary>
public class PositionUndefined : PositionState
{
	protected static PositionUndefined _instance = new();

	private PositionUndefined()
	{
		_id = 0;
		_name = "Undefined";
	}

	public static PositionUndefined Instance => _instance;

	public override string DescribeLocationMovementParticiple => "existing";

	public override string Describe(IPerceiver voyeur, IPerceivable target, PositionModifier modifier, IEmote emote,
		bool useHere = true)
	{
		var emoteText = emote != null ? $", {emote.ParseFor(voyeur)}" : "";
		if (target == null)
		{
			return $"{(useHere ? "here" : "")}{emoteText}";
		}

		switch (modifier)
		{
			case PositionModifier.Before:
				return $"{(useHere ? "here" : "")} before {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.Behind:
				return $"{(useHere ? "here" : "")} behind {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.In:
				return $"{(useHere ? "here" : "")} in {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.On:
				return $"{(useHere ? "here" : "")} on {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.Under:
				return $"{(useHere ? "here" : "")} under {target.HowSeen(voyeur)}{emoteText}";
			case PositionModifier.Around:
				return $"{(useHere ? "here" : "")} around {target.HowSeen(voyeur)}{emoteText}";
			default:
				return $"{(useHere ? "here" : "")} by {target.HowSeen(voyeur)}{emoteText}";
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
			"This should never happen. PositionUndefined should only be set by functions that do not call DescribeTransition.");
	}
}