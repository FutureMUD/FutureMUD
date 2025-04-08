using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.ThirdPartyCode;

namespace MudSharp.Character;

public partial class Character
{
	protected override IPositionState DefaultPosition => PositionStanding.Instance;
	public override IEnumerable<IPositionState> ValidPositions => Body.ValidPositions;

	public bool CanMovePosition(IPositionState whichPosition, bool ignoreMovement = false)
	{
		return PositionState != whichPosition &&
		       EffectsOfType<IPreventPositionChange>().All(x => !x.PreventsChange(PositionState, whichPosition)) &&
		       CanMovePosition(whichPosition, PositionModifier, PositionTarget, true, ignoreMovement);
	}

	public string WhyCannotMovePosition(IPositionState whichPosition, bool ignoreMovement = false)
	{
		if (PositionState == whichPosition)
		{
			return "You are already " + whichPosition.DefaultDescription() + ".";
		}

		if (EffectsOfType<IPreventPositionChange>().Any(x => x.PreventsChange(PositionState, whichPosition)))
		{
			return EffectsOfType<IPreventPositionChange>().First(x => x.PreventsChange(PositionState, whichPosition))
			                                              .WhyPreventsChange(PositionState, whichPosition);
		}

		return WhyCannotMovePosition(whichPosition, PositionModifier, PositionTarget, true, ignoreMovement);
	}

	public override bool CanBePositionedAgainst(IPositionState whichPosition, PositionModifier modifier)
	{
		switch (modifier)
		{
			case PositionModifier.On:
				return !PositionState.Upright && !whichPosition.Upright;
			case PositionModifier.In:
			case PositionModifier.Under:
				return false;
		}

		return true;
	}

	public override IEmote PositionEmote => Body.PositionEmote;

	public override PositionModifier PositionModifier
	{
		get => Body.PositionModifier;
		set
		{
			Body.PositionModifier = value;
			PositionChanged = true;
		}
	}

	public override IPositionState PositionState
	{
		get => Body.PositionState;
		set
		{
			var removeNoSave = false;
			if (_noSave && !Body.GetNoSave())
			{
				Body.SetNoSave(true);
				removeNoSave = true;
			}

			Body.PositionState = value;

			if (!_noSave)
			{
				PositionChanged = true;
			}

			if (removeNoSave)
			{
				Body.SetNoSave(false);
			}
		}
	}

	public override IPerceivable PositionTarget
	{
		get => Body.PositionTarget;
		set
		{
			Body.PositionTarget = value;
			PositionChanged = true;
		}
	}

	public override void SetTarget(IPerceivable target)
	{
		Body.SetTarget(target);
	}

	public void ResetPositionTarget(IEmote playerEmote, IEmote playerPmote)
	{
		if (PositionTarget == null)
		{
			OutputHandler.Send("You are not " + PositionState.DefaultDescription() + " near anything.");
			return;
		}

		string text;
		switch (PositionModifier)
		{
			case PositionModifier.On:
				text = " down off of ";
				break;
			case PositionModifier.Under:
				text = " out from under ";
				break;
			case PositionModifier.In:
				text = " out of ";
				break;
			case PositionModifier.Behind:
				text = " out from behind ";
				break;
			case PositionModifier.Around:
				text = " away from around ";
				break;
			default:
				text = " away from ";
				break;
		}

		var output =
			new MixedEmoteOutput(
				new Emote("@ " + PositionState.DescribePositionMovement + text + "$0", this, PositionTarget),
				flags: OutputFlags.SuppressObscured);
		output.Append(playerEmote);
		OutputHandler.Handle(output);
		SetTarget(null);
		SetEmote(playerPmote);
		SetModifier(PositionModifier.None);
		PositionHasChanged();
	}

	public void MovePosition(IPositionState whichPosition, PositionModifier whichModifier, IPerceivable target,
		IEmote playerEmote, IEmote playerPmote, bool ignoreMovementRestrictions = false, bool ignoreMovement = false)
	{
		if (!ignoreMovementRestrictions && Combat != null)
		{
			if (TakeOrQueueCombatAction(
				    SelectedCombatAction.GetEffectReposition(
					    this, whichPosition, whichModifier, target, playerEmote, playerPmote)) &&
			    Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				OutputHandler.Send(
					$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}{whichPosition.DefaultDescription()}.");
			}

			return;
		}

		if (!CanMovePosition(whichPosition, whichModifier, target, ignoreMovementRestrictions, ignoreMovement))
		{
			OutputHandler.Send(WhyCannotMovePosition(whichPosition, whichModifier, target,
				ignoreMovementRestrictions));
			return;
		}

		var output =
			new MixedEmoteOutput(
				whichPosition.DescribeTransition(this, PositionState, PositionModifier, whichModifier,
					PositionTarget, target), flags: OutputFlags.SuppressObscured);
		output.Append(playerEmote);
		OutputHandler.Handle(output);

		if (!PositionState.Upright && whichPosition.Upright)
		{
			foreach (var thing in TargetedBy.Where(x =>
				         x.PositionTarget == this && x.Location == Location &&
				         x.PositionModifier == PositionModifier.On))
			{
				thing.SetModifier(PositionModifier.None);
				if (!(thing is ICharacter body))
				{
					thing.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ fall|falls to the ground.", thing as IPerceiver)));
				}
				else
				{
					body.MovePosition(PositionStanding.Instance, null, null);
				}
			}
		}

		SetState(whichPosition);
		SetModifier(whichModifier);
		SetTarget(target);
		SetEmote(playerPmote);
		PositionHasChanged();
	}

	public void MovePosition(IPositionState whichPosition, IEmote playerEmote, IEmote playerPmote)
	{
		if (!CanMovePosition(whichPosition))
		{
			OutputHandler.Send(WhyCannotMovePosition(whichPosition));
			return;
		}

		var output =
			new MixedEmoteOutput(
				whichPosition.DescribeTransition(this, PositionState, PositionModifier, PositionModifier,
					PositionTarget, PositionTarget), flags: OutputFlags.SuppressObscured);
		output.Append(playerEmote);
		OutputHandler.Handle(output);
		if (!PositionState.Upright && whichPosition.Upright)
		{
			foreach (var thing in TargetedBy.Where(x =>
				         x.PositionTarget == this && x.Location == Location &&
				         x.PositionModifier == PositionModifier.On))
			{
				thing.SetModifier(PositionModifier.None);
				if (!(thing is ICharacter body))
				{
					thing.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ fall|falls to the ground.", thing as IPerceiver)));
				}
				else
				{
					body.MovePosition(PositionStanding.Instance, null, null);
				}
			}
		}

		SetState(whichPosition);
		SetEmote(playerPmote);
		PositionHasChanged();
	}

	#region Overrides of PerceivedItem

	public override void SetEmote(IEmote emote)
	{
		Body.SetEmote(emote);
	}

	#endregion

	public override IEnumerable<(IPerceivable Thing, Proximity Proximity)> LocalThingsAndProximities()
	{
		var proximityEffects = CombinedEffectsOfType<IAffectProximity>().ToList();
		foreach (var item in Location.GameItems)
		{
			if (item.RoomLayer != RoomLayer)
			{
				yield return (item, Proximity.VeryDistant);
			}

			if (Cover?.CoverItem?.Parent == item)
			{
				yield return (item, Proximity.Immediate);
			}

			if (PositionTarget == item || item.PositionTarget == this)
			{
				yield return (item, Proximity.Immediate);
			}

			if (InVicinity(item))
			{
				yield return (item, Proximity.Immediate);
			}

			var proximities = proximityEffects.Select(x => x.GetProximityFor(item)).Where(x => x.Affects).ToList();
			if (proximities.Any())
			{
				yield return (item, proximities.Select(x => x.Proximity).Min());
			}

			yield return (item, Proximity.Distant);
		}

		foreach (var actor in Location.Characters)
		{
			if (actor == this)
			{
				continue;
			}

			if (actor.RoomLayer != RoomLayer)
			{
				yield return (actor, Proximity.VeryDistant);
			}

			if (PositionTarget == actor || actor.PositionTarget == this)
			{
				yield return (actor, Proximity.Immediate);
			}

			if (InVicinity(actor))
			{
				yield return (actor, Proximity.Immediate);
			}

			if (Party != null && actor.Party == Party)
			{
				yield return (actor, Proximity.Proximate);
			}

			var proximities = proximityEffects.Select(x => x.GetProximityFor(actor)).Where(x => x.Affects).ToList();
			if (proximities.Any())
			{
				yield return (actor, proximities.Select(x => x.Proximity).Min());
			}
		}

		foreach (var effect in CombinedEffectsOfType<AdjacentToExit>().Where(x => x.Exit.Exit.Door != null).ToList())
		{
			yield return (effect.Exit.Exit.Door.Parent, Proximity.Proximate);
		}

		foreach (var effect in CombinedEffectsOfType<GuardingExit>().Where(x => x.Exit.Exit.Door != null).ToList())
		{
			yield return (effect.Exit.Exit.Door.Parent, Proximity.Proximate);
		}
	}

	public override Proximity GetProximity(IPerceivable thing)
	{
		if (thing == null)
		{
			return Proximity.Unapproximable;
		}

		if (thing.IsSelf(this))
		{
			return Proximity.Intimate;
		}

		if (Party?.Members.Contains(thing) == true)
		{
			return Proximity.Proximate;
		}

		if ((PositionTarget != null &&
		     (PositionTarget.IsSelf(thing.PositionTarget) || PositionTarget.IsSelf(thing))) ||
		    thing.PositionTarget?.IsSelf(this) == true)
		{
			return Proximity.Immediate;
		}

		if (Cover == thing)
		{
			return Proximity.Immediate;
		}

		var ptGameItem = PositionTarget as IGameItem;
		if (ptGameItem?.IsItemType<IChair>() == true)
		{
			var chair = ptGameItem.GetItemType<IChair>();
			if (chair.Table != null)
			{
				if (thing.IsSelf(chair.Table.Parent))
				{
					return Proximity.Immediate;
				}

				if (thing.PositionTarget?.IsSelf(chair.Table.Parent) == true)
				{
					return Proximity.Immediate;
				}

				if (thing is IGameItem taig && taig.IsItemType<IChair>())
				{
					var otherChair = taig.GetItemType<IChair>();
					if (otherChair.Table == chair.Table)
					{
						return Proximity.Immediate;
					}
				}
			}
		}

		return base.GetProximity(thing);
	}

	public bool CanMovePosition(IPositionState whichPosition, PositionModifier whichModifier, IPerceivable target,
		bool ignoreMovementRestrictions = false, bool ignoreMovement = false)
	{
		if (!ValidPositions.Contains(whichPosition))
		{
			return false;
		}

		if (!ignoreMovementRestrictions && (whichModifier != PositionModifier || target != PositionTarget) &&
		    PositionState.MoveRestrictions == MovementAbility.Restricted)
		{
			return false;
		}

		if (target?.CanBePositionedAgainst(whichPosition, whichModifier) == false)
		{
			return false;
		}

		if (!ignoreMovement && Movement != null)
		{
			return false;
		}

		switch (whichPosition)
		{
			case PositionStanding _:
			case PositionStandingAttention _:
			case PositionStandingEasy _:
			case PositionSquatting _:
			case PositionLeaning _:
				if (!Body.CanStand(false))
				{
					return false;
				}

				break;
			case PositionKneeling _:
			case PositionProstrate _:
				if (!Body.CanKneel(false))
				{
					return false;
				}

				break;
			case PositionSitting _:
			case PositionLyingDown _:
			case PositionLounging _:
			case PositionSlumped _:
				if (PositionState.CompareTo(whichPosition) == PositionHeightComparison.Lower && !Body.CanSitUp())
				{
					return false;
				}

				break;
		}

		return true;
	}

	public string WhyCannotMovePosition(IPositionState whichPosition, PositionModifier whichModifier,
		IPerceivable target, bool ignoreMovementRestrictions = false, bool ignoreMovement = false)
	{
		if (!ValidPositions.Contains(whichPosition))
		{
			return "That is not a valid position for you.";
		}

		if (!ignoreMovement && Movement != null)
		{
			return "You must first stop moving.";
		}

		if ((whichModifier != PositionModifier || target != PositionTarget) && !ignoreMovementRestrictions &&
		    PositionState.MoveRestrictions == MovementAbility.Restricted)
		{
			return "You must first get up before you can move about.";
		}

		if (target != null && !target.CanBePositionedAgainst(whichPosition, whichModifier))
		{
			return target.HowSeen(this, true) + " cannot be positioned against in that manner.";
		}

		switch (whichPosition)
		{
			case PositionStanding _:
			case PositionStandingAttention _:
			case PositionStandingEasy _:
			case PositionSquatting _:
			case PositionLeaning _:
				if (!Body.CanStand(false))
				{
					return
						$"You need at least {Body.Prototype.MinimumLegsToStand:N0} working {(Body.Prototype.MinimumLegsToStand == 1 ? Body.Prototype.LegDescriptionSingular : Body.Prototype.LegDescriptionPlural).ToLowerInvariant()} to take that position.";
				}

				break;
			case PositionKneeling _:
			case PositionProstrate _:
				if (!Body.CanKneel(false))
				{
					return
						$"You need at least {Body.Prototype.MinimumLegsToStand:N0} working {(Body.Prototype.MinimumLegsToStand == 1 ? Body.Prototype.LegDescriptionSingular : Body.Prototype.LegDescriptionPlural).ToLowerInvariant()}, or  {Body.Prototype.MinimumLegsToStand - 1:N0} working {(Body.Prototype.MinimumLegsToStand == 1 ? Body.Prototype.LegDescriptionSingular : Body.Prototype.LegDescriptionPlural).ToLowerInvariant()} and 1 working arm to take that position.";
				}

				break;
			case PositionSitting _:
			case PositionLyingDown _:
			case PositionLounging _:
			case PositionSlumped _:
				if (PositionState.CompareTo(whichPosition) == PositionHeightComparison.Lower && !Body.CanSitUp())
				{
					return
						$"You need at least 1 working {Body.Prototype.LegDescriptionSingular} or 1 working arm to take that position from your current position.";
				}

				break;
		}

		return "You cannot take that position.";
	}

	public void Awaken(IEmote emote = null)
	{
		State |= CharacterState.Awake;
		State &= ~CharacterState.Sleeping;
		OutputHandler.Handle(new MixedEmoteOutput(new Emote("@ awaken|awakens", this)).Append(emote));
		if (EffectsOfType<IDreamingEffect>().Any())
		{
			var dream = EffectsOfType<IDreamingEffect>().First();
			dream.Dream?.OnWakeDuringDreamProg?.Execute(this, dream.Dream.Id);
		}

		RemoveAllEffects(
			x =>
				x.IsEffectType<IDreamingEffect>() || x.IsEffectType<INoDreamEffect>() ||
				x.IsEffectType<PendingRestedness>() ||
				x.IsEffectType<INoWakeEffect>());

		//If we have an active WellRested, give it a chance to renew its duration if the character has earned
		//a longer buff
		if (EffectsOfType<WellRested>().Any())
		{
			var rested = EffectsOfType<WellRested>().First();
			rested.RenewRest();
		}
	}

	public void Sleep(IEmote emote = null)
	{
		var result = Gameworld.GetCheck(CheckType.GoToSleepCheck).Check(this, Difficulty.Normal);
		// TODO - difficulty based on tiredness?
		var delay = Dice.Roll($"1d10+{50 + result.FailureDegrees() * 10}"); // TODO - soft coded somewhere?
		AddEffect(new NoWake(this, "You cannot wake up so soon after falling asleep."),
			TimeSpan.FromSeconds(delay));
		if (IsPlayerCharacter && !IsGuest)
		{
			AddEffect(new NoDreamEffect(this), TimeSpan.FromSeconds(RandomUtilities.Random(200, 300)));
		}

		OutputHandler.Handle(
			new MixedEmoteOutput(new Emote("@ drift|drifts off to sleep", this)).Append(emote));
		State |= CharacterState.Sleeping;
		State &= ~CharacterState.Awake;
	}
}