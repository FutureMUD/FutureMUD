using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position.PositionStates;

public class PositionLyingDown : PositionState
{
	protected static PositionLyingDown _instance = new();

	private PositionLyingDown()
	{
		_id = 5;
		_name = "Lying Down";
	}

	public static PositionLyingDown Instance => _instance;
	public override string DescribeLocationMovementParticiple => "lying down";

	public override string DefaultDescription()
	{
		return "lying";
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

	public override IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		string text;
		switch (CompareTo(originalState))
		{
			case PositionHeightComparison.Equivalent:
				text = "lie|lies";
				break;
			case PositionHeightComparison.Lower:
				text = "lie|lies down";
				break;
			case PositionHeightComparison.Higher:
				text = "roll|rolls into a supine position";
				break;
			default:
				text = "lie|lies";
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

	public override PositionHeightComparison CompareTo(PositionProstrate state)
	{
		return PositionHeightComparison.Lower;
	}
}