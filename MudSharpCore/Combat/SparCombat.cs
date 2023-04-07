using System.Linq;
using System.Text;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

public class SparCombat : CombatBase
{
	#region Overrides of CombatBase

	public override void JoinCombat(IPerceiver character, Difficulty initialDelayDifficulty = Difficulty.Automatic)
	{
		if (!_combatants.Contains(character))
		{
			_combatants.Add(character);
			Gameworld.Scheduler.AddSchedule(new Schedule<IPerceiver>(character, CombatAction, ScheduleType.Combat,
				GetCombatDelay(character, initialDelayDifficulty, 0.1),
				$"SparCombat Initial Action {character.HowSeen(character, flags: PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee)}"
			));
			character.Combat = this;
			character.RemoveAllEffects(x => x.IsEffectType<IRemoveOnCombatStart>(), true);
			character.HandleEvent(EventType.JoinCombat, character);
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
		foreach (var combatant in Combatants.Where(x => x.CombatTarget == character).ToList())
		{
			combatant.CombatTarget = null;
			combatant.AcquireTarget();
			if (combatant.CombatTarget == null && !combatant.CheckCombatStatus())
			{
				LeaveCombat(combatant);
			}
		}

		Gameworld.Scheduler.Destroy(character, ScheduleType.Combat);
		character.HandleEvent(EventType.LeaveCombat, character);
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
	}

	public override bool CanFreelyLeaveCombat(IPerceiver who)
	{
		return true;
	}

	public override void TruceRequested(IPerceiver combatant)
	{
		combatant.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ motion|motions for the sparring bout to end, and bow|bows out of combat.",
				combatant)));
		LeaveCombat(combatant);
	}

	public override string CombatHeaderDescription =>
		$"This is a {"Sparring Bout".Colour(Telnet.Green)}, and will automatically end by choice or unconsciousness of combatants.";

	public override string LDescAddendumFor(ICombatant combatant, IPerceiver voyeur)
	{
		if (combatant.CombatTarget == null)
		{
			return "sparring, but unengaged";
		}

		if (combatant.MeleeRange)
		{
			return $"sparring {combatant.CombatTarget.HowSeen(voyeur)}";
		}

		return combatant.Location == combatant.CombatTarget.Location
			? $"sparring {combatant.CombatTarget.HowSeen(voyeur)} at range"
			: $"sparring {"a distant target".ColourCharacter()}";
	}

	#region Overrides of CombatBase

	public override void CombatAction(IPerceiver perceiver)
	{
		base.CombatAction(perceiver);
		if (perceiver.CombatTarget is ICharacter targetChar)
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

		if (perceiver is ICharacter perceiverChar)
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

	/// <summary>
	///     If true, this combat would be considered "Friendly", e.g. a spar, training bout, competition etc.
	/// </summary>
	public override bool Friendly => true;

	#endregion
}