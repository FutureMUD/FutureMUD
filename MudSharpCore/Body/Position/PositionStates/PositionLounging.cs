using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionLounging : PositionState
{
	protected static PositionLounging _instance = new();

	private PositionLounging()
	{
		_id = 4;
		_name = "Lounging";
	}

	public static PositionLounging Instance => _instance;
	public override string DescribeLocationMovementParticiple => "lounging";

	public override string DefaultDescription()
	{
		return "lounging";
	}

	public override PositionHeightComparison CompareTo(PositionKneeling state)
	{
		return PositionHeightComparison.Lower;
	}

	public override PositionHeightComparison CompareTo(PositionProne state)
	{
		return PositionHeightComparison.Higher;
	}

	public override PositionHeightComparison CompareTo(PositionSitting state)
	{
		return PositionHeightComparison.Equivalent;
	}

	public override PositionHeightComparison CompareTo(PositionLounging state)
	{
		return PositionHeightComparison.Equivalent;
	}

	public override PositionHeightComparison CompareTo(PositionStanding state)
	{
		return PositionHeightComparison.Lower;
	}

	public override PositionHeightComparison CompareTo(PositionLeaning state)
	{
		return PositionHeightComparison.Lower;
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
		return PositionHeightComparison.Lower;
	}

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		string text;
		switch (CompareTo(originalState))
		{
			case PositionHeightComparison.Equivalent:
				text = "lounge|lounges";
				break;
			case PositionHeightComparison.Higher:
				text = "sit|sits up into a lounging position";
				break;
			case PositionHeightComparison.Lower:
				text = "relax|relaxes into a lounging position";
				break;
			default:
				text = "lounge|lounges";
				break;
		}

		return DressTransition(positionee, text, originalState, originalModifier, newModifier, originalTarget,
			newTarget);
	}

	public override string Describe(IPerceiver voyeur, IPerceivable target, PositionModifier modifier, IEmote emote,
		bool useHere = true)
	{
		var targetAsItem = target as IGameItem;
		var chair = targetAsItem?.GetItemType<IChair>();
		if (chair != null)
		{
			var suffix = "";
			if (chair.Table != null)
			{
				switch (chair.Parent.PositionModifier)
				{
					case PositionModifier.Behind:
						suffix = $" behind {chair.Table.Parent.HowSeen(voyeur)}";
						break;
					case PositionModifier.Before:
						suffix = $" before {chair.Table.Parent.HowSeen(voyeur)}";
						break;
					default:
						suffix = $" at {chair.Table.Parent.HowSeen(voyeur)}";
						break;
				}
			}

			var emoteText = emote != null ? $", {emote.ParseFor(voyeur)}" : "";

			switch (modifier)
			{
				case PositionModifier.Behind:
					return $"{DefaultDescription()} behind {target.HowSeen(voyeur)}{suffix}{emoteText}";
				case PositionModifier.Before:
					return $"{DefaultDescription()} before {target.HowSeen(voyeur)}{suffix}{emoteText}";
				case PositionModifier.Under:
					return $"{DefaultDescription()} under {target.HowSeen(voyeur)}{suffix}{emoteText}";
				case PositionModifier.Around:
					return $"{DefaultDescription()} around {target.HowSeen(voyeur)}{suffix}{emoteText}";
				default:
					return $"{DefaultDescription()} on {target.HowSeen(voyeur)}{suffix}{emoteText}";
			}
		}

		return base.Describe(voyeur, target, modifier, emote, useHere);
	}
}