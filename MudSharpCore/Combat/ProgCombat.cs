using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat;

public class ProgCombat : CombatBase
{
	public IFutureProg OnJoinProg { get; }
	public IFutureProg OnLeaveProg { get; }
	public IFutureProg OnCombatEndProg { get; }
	public IFutureProg OnCombatMove { get; }
	public IFutureProg OnCombatHit { get; }
	public string CombatReference { get; }
	public string CombatDescription { get; }
	public string CombatActionWord { get; }

	public ProgCombat(string combatDescription, string combatActionWord, string combatReference, bool friendly,
		IFutureProg onJoinProg, IFutureProg onLeaveProg, IFutureProg onCombatEndProg, IFutureProg onCombatMove,
		IFutureProg onCombatHit)
	{
		CombatActionWord = combatActionWord;
		CombatDescription = combatDescription;
		CombatReference = combatReference;
		_friendly = friendly;
		OnJoinProg = onJoinProg;
		OnLeaveProg = onLeaveProg;
		OnCombatEndProg = onCombatEndProg;
		OnCombatMove = onCombatMove;
		OnCombatHit = onCombatHit;
	}

	#region Overrides of CombatBase

	public override void JoinCombat(IPerceiver character, Difficulty initialDelayDifficulty = Difficulty.Automatic)
	{
		if (!_combatants.Contains(character))
		{
			_combatants.Add(character);
			Gameworld.Scheduler.AddSchedule(new Schedule<IPerceiver>(character, CombatAction, ScheduleType.Combat,
				GetCombatDelay(character, initialDelayDifficulty, 0.1),
				$"ProgCombat Initial Action {character.HowSeen(character, flags: PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee)}"));
			character.Combat = this;
			character.RemoveAllEffects(x => x.IsEffectType<IRemoveOnCombatStart>(), true);
			character.HandleEvent(EventType.JoinCombat, character);
			OnJoinProg?.Execute(character, CombatReference);
		}

		CombatCells.Add(character.Location);
	}

	/// <summary>
	///     Removes a specified combatant from the combat
	/// </summary>
	/// <param name="character">The character leaving the combat</param>
	/// <returns>True if combat ended entirely because of this action</returns>
	public override bool LeaveCombat(IPerceiver character)
	{
		_combatants.Remove(character);
		character.CombatStrategyMode = CombatStrategyMode.StandardMelee;
		character.DefensiveAdvantage = 0;
		character.OffensiveAdvantage = 0;
		character.Combat = null;
		character.CombatTarget = null;
		character.MeleeRange = false;
		character.Aim = null;
		character.RemoveAllEffects(x => x.IsEffectType<IRemoveOnCombatEnd>(), true);
		Gameworld.Scheduler.Destroy(character, ScheduleType.Combat);
		character.HandleEvent(EventType.LeaveCombat, character);
		OnLeaveProg?.Execute(character, CombatReference);
		foreach (var combatant in Combatants.Where(x => x.CombatTarget == character).ToList())
		{
			combatant.CombatTarget = null;
			combatant.AcquireTarget();
			if (combatant.CombatTarget == null && !combatant.CheckCombatStatus())
			{
				LeaveCombat(combatant);
			}
		}

		return CheckForCombatEnd();
	}

	public bool CheckForCombatEnd()
	{
		if (_combatants.Count < 2)
		{
			EndCombat(true);
			return true;
		}

		return false;
	}

	public override void EndCombat(bool echo)
	{
		foreach (var combatant in _combatants.ToList())
		{
			if (echo)
			{
				combatant.Send("The combat has ended.");
			}

			LeaveCombat(combatant);
		}

		EndCombatEvent();
		OnCombatEndProg?.Execute(CombatReference);
	}

	public override bool CanFreelyLeaveCombat(IPerceiver who)
	{
		return Friendly || base.CanFreelyLeaveCombat(who);
	}

	public override void TruceRequested(IPerceiver combatant)
	{
		if (Friendly)
		{
			combatant.OutputHandler.Handle(
				new EmoteOutput(new Emote(
					$"@ motion|motions for the {CombatActionWord.ToLowerInvariant()} to end, and bow|bows out of combat.",
					combatant)));
			LeaveCombat(combatant);
		}
		else
		{
			if (!combatant.CanTruce())
			{
				combatant.Send(combatant.WhyCannotTruce());
				return;
			}

			if (CanFreelyLeaveCombat(combatant))
			{
				combatant.OutputHandler.Handle(
					new EmoteOutput(new Emote("Unopposed, @ leave|leaves the combat.", combatant)));
				LeaveCombat(combatant);
				return;
			}

			if (CanFreelyLeaveCombatAgainstOpponent(combatant, combatant.CombatTarget))
			{
				var oldTarget = combatant.CombatTarget;
				var nextTarget = Combatants.FirstOrDefault(x =>
					x.CombatTarget == combatant && x != combatant && x != combatant.CombatTarget &&
					combatant.CanEngage(x));
				if (nextTarget != null)
				{
					combatant.Engage(nextTarget,
						combatant.CombatSettings.PreferredRangedMode.IsRangedStartDesiringStrategy());
				}
				else
				{
					combatant.CombatTarget = null;
					combatant.CheckCombatStatus();
				}

				if (oldTarget?.CheckCombatStatus() == false)
				{
					LeaveCombat(oldTarget);
				}
			}

			if (combatant.EffectsOfType<ICombatTruceEffect>().Any())
			{
				combatant.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ motion|motions again for a truce.", combatant)));
				foreach (var ch in Combatants.OfType<ICharacter>().Where(x => x.CombatTarget == combatant).ToList())
				{
					ch.HandleEvent(EventType.TruceOffered, ch, combatant);
				}

				return;
			}

			// In this case, we will be accepting the truce of our combat target
			if (combatant.CombatTarget?.EffectsOfType<ICombatTruceEffect>().Any() ?? false)
			{
				combatant.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ accept|accepts $0's offer of a truce.", combatant,
						combatant.CombatTarget)));
				TruceAccepted(combatant, combatant.CombatTarget);
				return;
			}

			if (Combatants.Where(x => x.CombatTarget == combatant && x.EffectsOfType<ICombatTruceEffect>().Any())
			              .ToList().Any(target => TruceAccepted(combatant, target)))
			{
				return;
			}

			combatant.OutputHandler.Handle(new EmoteOutput(new Emote("@ motion|motions for a truce.", combatant)));
			combatant.AddEffect(new Truce(combatant));
			foreach (var ch in Combatants.OfType<ICharacter>().Where(x => x.CombatTarget == combatant).ToList())
			{
				ch.HandleEvent(EventType.TruceOffered, ch, combatant);
			}

			combatant.OutputHandler.Handle(
				new EmoteOutput(new Emote($"@ motion|motions for the {CombatActionWord.ToLowerInvariant()} to end.",
					combatant)));
		}
	}

	private bool TruceAccepted(IPerceiver accepter, IPerceiver requester)
	{
		if (Combatants.Except(accepter).All(x => x.CombatTarget != requester))
		{
			requester.RemoveAllEffects(x => x is ICombatTruceEffect);
			if (LeaveCombat(requester))
			{
				return true;
			}
		}

		if (Combatants.Except(requester).All(x => x.CombatTarget != accepter))
		{
			accepter.RemoveAllEffects(x => x is ICombatTruceEffect);
			if (LeaveCombat(accepter))
			{
				return true;
			}
		}

		if (Combatants.Contains(requester) && requester.CombatTarget == null)
		{
			requester.CombatTarget = Combatants.FirstOrDefault(x => x.CombatTarget == requester);
		}

		if (Combatants.Contains(accepter) && accepter.CombatTarget == null)
		{
			accepter.CombatTarget = Combatants.FirstOrDefault(x => x.CombatTarget == accepter);
		}

		return false;
	}

	public override string CombatHeaderDescription =>
		Friendly
			? $"This is a {CombatDescription.Colour(Telnet.Green)}, and will automatically end by choice or unconsciousness of combatants."
			: $"This is a {CombatDescription.Colour(Telnet.Green)}, and has no rules or victory conditions.";

	public override string LDescAddendumFor(ICombatant combatant, IPerceiver voyeur)
	{
		if (combatant.CombatTarget == null)
		{
			return $"{CombatActionWord}, but unengaged";
		}

		if (combatant.CombatStrategyMode == CombatStrategyMode.Flee)
		{
			return $"attempting to flee combat";
		}

		if (combatant.MeleeRange)
		{
			return !combatant.CombatStrategyMode.IsMeleeDesiredStrategy()
				? $"attempting to skirmish away from {combatant.CombatTarget.HowSeen(voyeur)}"
				: $"in melee with {combatant.CombatTarget.HowSeen(voyeur)}";
		}

		if (combatant.Aim != null)
		{
			if (combatant.Aim.Target.Location == combatant.Location)
			{
				return
					$"aiming {combatant.Aim.Weapon.Parent.HowSeen(voyeur)} at {combatant.Aim.Target.HowSeen(voyeur)}";
			}

			return $"aiming {combatant.Aim.Weapon.Parent.HowSeen(voyeur)} at {"a distant target".ColourCharacter()}";
		}

		if (combatant.Location == combatant.CombatTarget.Location)
		{
			return $"{CombatActionWord} {combatant.CombatTarget.HowSeen(voyeur)} at range";
		}

		var perceivable = combatant as IPerceivable;
		var direction =
			perceivable.PathBetween(combatant.CombatTarget, 10, false, false, true)
			           .Select(x => x.OutboundDirection)
			           .DescribeDirection();

		return $"{CombatActionWord} {"a distant target".ColourCharacter()} to the {direction}";
	}

	#region Overrides of CombatBase

	public override void CombatAction(IPerceiver perceiver)
	{
		base.CombatAction(perceiver);
		OnCombatMove?.Execute(perceiver, CombatReference);
		if (Friendly && perceiver.CombatTarget is ICharacter targetChar)
		{
			if (targetChar.State.HasFlag(CharacterState.Unconscious) ||
			    targetChar.State.HasFlag(CharacterState.Sleeping))
			{
				targetChar.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ leave|leaves the sparring bout due to incapacitation.", targetChar)));
				if (LeaveCombat(targetChar))
				{
					return;
				}
			}
		}

		if (Friendly && perceiver is ICharacter perceiverChar)
		{
			if (perceiverChar.State.HasFlag(CharacterState.Unconscious) ||
			    perceiverChar.State.HasFlag(CharacterState.Sleeping))
			{
				perceiverChar.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ leave|leaves the sparring bout due to incapacitation.",
						perceiverChar)));
				LeaveCombat(perceiverChar);
			}
		}
	}

	#endregion

	private bool _friendly;

	/// <summary>
	///     If true, this combat would be considered "Friendly", e.g. a spar, training bout, competition etc.
	/// </summary>
	public override bool Friendly => _friendly;

	#endregion
}