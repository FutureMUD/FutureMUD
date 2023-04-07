using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Position;

public abstract class PositionState : FrameworkItem, IPositionState
{
	protected static Dictionary<long, IPositionState> StateDictionary = new();

	public static IEnumerable<IPositionState> States => StateDictionary.Values;

	private static bool _positionsInitialised;

	public sealed override string FrameworkItemType => "PositionState";

	/// <summary>
	///     Whether or not this position is considered Upright
	/// </summary>
	public virtual bool Upright => false;

	/// <summary>
	///     What sort of restrictions being in this position state imposes upon Movement
	/// </summary>
	public virtual MovementAbility MoveRestrictions => MovementAbility.Restricted;

	/// <summary>
	///     If the IPositionable moves while in this state, which state they should transition to
	/// </summary>
	public virtual IPositionState TransitionOnMovement => null;

	/// <summary>
	///     Returns a string in the form "1st|3rd" that describes the motion when one moves in this position intralocally
	/// </summary>
	public virtual string DescribePositionMovement => "move|moves";

	public virtual string DescribeLocationMovement3rd => throw new NotImplementedException();

	public abstract string DescribeLocationMovementParticiple { get; }

	public static IPositionState GetState(long id)
	{
		return StateDictionary.ContainsKey(id) ? StateDictionary[id] : PositionUndefined.Instance;
	}

	[CanBeNull]
	public static IPositionState GetState(string stateText)
	{
		if (long.TryParse(stateText, out var value))
		{
			return GetState(value);
		}

		foreach (var position in StateDictionary.Values)
		{
			if (position.DescribeLocationMovementParticiple.EqualTo(stateText) ||
			    position.DescribeLocationMovementParticiple.StartsWith(stateText,
				    StringComparison.InvariantCultureIgnoreCase))
			{
				return position;
			}
		}

		return null;
	}

	public static void SetupPositions()
	{
		if (!_positionsInitialised)
		{
			PositionUndefined.Instance.Initialise();
			PositionStanding.Instance.Initialise();
			PositionSitting.Instance.Initialise();
			PositionKneeling.Instance.Initialise();
			PositionLounging.Instance.Initialise();
			PositionLyingDown.Instance.Initialise();
			PositionProne.Instance.Initialise();
			PositionProstrate.Instance.Initialise();
			PositionSprawled.Instance.Initialise();
			PositionStandingAttention.Instance.Initialise();
			PositionStandingEasy.Instance.Initialise();
			PositionLeaning.Instance.Initialise();
			PositionSlumped.Instance.Initialise();
			PositionHanging.Instance.Initialise();
			PositionSquatting.Instance.Initialise();
			PositionClimbing.Instance.Initialise();
			PositionSwimming.Instance.Initialise();
			PositionFloatingInWater.Instance.Initialise();
			PositionFlying.Instance.Initialise();
		}

		_positionsInitialised = true;
	}

	protected void Initialise()
	{
		StateDictionary.Add(_id, this);
	}

	/// <summary>
	///     This function allows the various position states to add a unique description suffix to any IPositionable, for
	///     instance, the PositionStanding position state might by default return an "standing here" fragment
	///     Punctuation is omitted so it can be used in any way the client likes.
	///     The verb form of "to be" is omitted
	///     Override this function on a state if it has some specific combination of targets/modifiers that is different,
	///     otherwise just use base
	/// </summary>
	/// <param name="voyeur"></param>
	/// <param name="target">A target IDescribable that the position refers to. For example, a table</param>
	/// <param name="modifier">An enum representing a modifier to the target, having unique meanings</param>
	/// <param name="emote"></param>
	/// <returns></returns>
	public virtual string Describe(IPerceiver voyeur, IPerceivable target, PositionModifier modifier, IEmote emote,
		bool useHere = true)
	{
		var emoteText = emote != null ? ", " + emote.ParseFor(voyeur) : "";
		if (target == null)
		{
			return $"{DefaultDescription()}{(useHere ? " here" : "")}{emoteText}";
		}

		switch (modifier)
		{
			case PositionModifier.Before:
				return DefaultDescription() + " before " + target.HowSeen(voyeur) + emoteText;
			case PositionModifier.Behind:
				return DefaultDescription() + " behind " + target.HowSeen(voyeur) + emoteText;
			case PositionModifier.In:
				return DefaultDescription() + " in " + target.HowSeen(voyeur) + emoteText;
			case PositionModifier.On:
				return DefaultDescription() + " on " + target.HowSeen(voyeur) + emoteText;
			case PositionModifier.Under:
				return DefaultDescription() + " under " + target.HowSeen(voyeur) + emoteText;
			case PositionModifier.Around:
				return DefaultDescription() + " around " + target.HowSeen(voyeur) + emoteText;
			default:
				return DefaultDescription() + " by " + target.HowSeen(voyeur) + emoteText;
		}
	}

	protected IEmote DressTransition(ICharacter positionee, string positionText, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget)
	{
		IEmote emote = null;
		var newTargetItem = newTarget as IGameItem;
		if ((newTarget == originalTarget && newModifier == originalModifier) ||
		    (newTarget == originalTarget && newTargetItem == null))
		{
			emote = new Emote("@ " + positionText, positionee);
		}
		else
		{
			string prefix;
			if (originalTarget == null || newTarget == originalTarget ||
			    (newTargetItem?.IsItemType<IChair>() == true &&
			     newTargetItem.GetItemType<IChair>().Table?.Parent == originalTarget))
			{
				prefix = "";
			}
			else
			{
				switch (originalModifier)
				{
					case PositionModifier.Before:
						prefix = originalState.DescribePositionMovement + $" away from $0 and ";
						break;
					case PositionModifier.Behind:
						prefix = originalState.DescribePositionMovement + $" out from behind $0 and ";
						break;
					case PositionModifier.In:
						prefix = originalState.DescribePositionMovement + $" out of $0 and ";
						break;
					case PositionModifier.On:
						prefix = originalState.DescribePositionMovement + $" off of $0 and ";
						break;
					case PositionModifier.Under:
						prefix = originalState.DescribePositionMovement + $" out from under $0 and ";
						break;
					case PositionModifier.Around:
						prefix = originalState.DescribePositionMovement + " away from around $0 and ";
						break;
					case PositionModifier.None:
					default:
						prefix = originalState.DescribePositionMovement + $" away from $0 and ";
						break;
				}
			}

			var targetString = "";
			if (newTarget != null)
			{
				switch (newModifier)
				{
					case PositionModifier.Before:
						targetString = " before $1";
						break;
					case PositionModifier.Behind:
						targetString = " behind $1";
						break;
					case PositionModifier.In:
						targetString = " in $1";
						break;
					case PositionModifier.On:
						targetString = " on $1";
						break;
					case PositionModifier.Under:
						targetString = " beneath $1";
						break;
					case PositionModifier.Around:
						targetString = " around $1";
						break;
					default:
						targetString = " by $1";
						break;
				}
			}

			var newChair = newTargetItem?.GetItemType<IChair>();
			if (newChair != null && newModifier == PositionModifier.On)
			{
				if (newChair.Table != null)
				{
					targetString = " on $1 at $2";
					emote = new Emote("@ " + prefix + positionText + targetString, positionee, originalTarget,
						newTarget, newChair.Table.Parent);
				}
			}

			if (newChair != null && newModifier == PositionModifier.None &&
			    originalModifier == PositionModifier.On)
			{
				if (newChair.Table != null)
				{
					targetString = " from $1 at $2";
					emote = new Emote("@ " + prefix + positionText + targetString, positionee, originalTarget,
						newTarget, newChair.Table.Parent);
				}
				else
				{
					targetString = " from $1";
				}
			}

			if (emote == null)
			{
				emote = new Emote("@ " + prefix + positionText + targetString, positionee, originalTarget, newTarget);
			}
		}

		return emote;
	}

	public abstract string DefaultDescription();

	public abstract IEmote DescribeTransition(ICharacter positionee, IPositionState originalState,
		PositionModifier originalModifier, PositionModifier newModifier, IPerceivable originalTarget,
		IPerceivable newTarget);

	public virtual bool SafeFromFalling => false;

	#region CompareTo Overloads

	/// <summary>
	///     Returns an enum description of the height of the current position as compared to the state. A result of Higher
	///     implies that this state is HIGHER than the compared state, for instance.
	/// </summary>
	/// <param name="state">The state to compare to the current state</param>
	/// <returns>An enum representing the result of the comparison</returns>
	public virtual PositionHeightComparison CompareTo(dynamic state)
	{
		return CompareTo(state);
	}

	public virtual PositionHeightComparison CompareTo(PositionFlying state)
	{
		return CompareTo(PositionStanding.Instance);
	}

	public virtual PositionHeightComparison CompareTo(PositionClimbing state)
	{
		return CompareTo(PositionSitting.Instance);
	}

	public virtual PositionHeightComparison CompareTo(PositionSwimming state)
	{
		return CompareTo(PositionStanding.Instance);
	}

	public virtual PositionHeightComparison CompareTo(PositionSitting state)
	{
		return PositionHeightComparison.Undefined;
	}

	public virtual PositionHeightComparison CompareTo(PositionStanding state)
	{
		return PositionHeightComparison.Undefined;
	}

	public virtual PositionHeightComparison CompareTo(PositionKneeling state)
	{
		return PositionHeightComparison.Undefined;
	}

	public virtual PositionHeightComparison CompareTo(PositionSlumped state)
	{
		return CompareTo(PositionSitting.Instance);
	}

	public virtual PositionHeightComparison CompareTo(PositionProne state)
	{
		return PositionHeightComparison.Undefined;
	}

	public virtual PositionHeightComparison CompareTo(PositionLyingDown state)
	{
		return PositionHeightComparison.Undefined;
	}

	public virtual PositionHeightComparison CompareTo(PositionSprawled state)
	{
		return PositionHeightComparison.Higher;
	}

	public virtual PositionHeightComparison CompareTo(PositionProstrate state)
	{
		return PositionHeightComparison.Undefined;
	}

	public virtual PositionHeightComparison CompareTo(PositionLeaning state)
	{
		return CompareTo(PositionStanding.Instance);
	}

	public virtual PositionHeightComparison CompareTo(PositionStandingAttention state)
	{
		// Default is to handle as per standing
		return CompareTo(PositionStanding.Instance);
	}

	public virtual PositionHeightComparison CompareTo(PositionSquatting state)
	{
		// Default is to handle as per standing
		return CompareTo(PositionStanding.Instance);
	}

	public virtual PositionHeightComparison CompareTo(PositionStandingEasy state)
	{
		// Default is to handle as per standing
		return CompareTo(PositionStanding.Instance);
	}

	public virtual PositionHeightComparison CompareTo(PositionLounging state)
	{
		// Default is to handle as per sitting
		return CompareTo(PositionSitting.Instance);
	}

	#endregion
}