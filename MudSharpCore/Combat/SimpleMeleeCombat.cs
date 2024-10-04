using System;
using System.Linq;
using System.Text;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

public class SimpleMeleeCombat : CombatBase
{
	public SimpleMeleeCombat(IFuturemud gameworld)
	{
		Gameworld = gameworld;
	}

	public override bool Friendly => false;

	public override void JoinCombat(IPerceiver character, Difficulty initialDelayDifficulty = Difficulty.Automatic)
	{
		if (!_combatants.Contains(character))
		{
			_combatants.Add(character);
			if (character.Combat == null)
			{
				Gameworld.Scheduler.AddSchedule(new Schedule<IPerceiver>(character,
					CombatAction, ScheduleType.Combat,
					TimeSpan.FromSeconds(0.1 + (int)initialDelayDifficulty),
					$"SimpleMeleeCombat Initial Action {character.HowSeen(character, flags: PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee)}"
				));
			}

			character.Combat = this;
			character.RemoveAllEffects(x => x.IsEffectType<IRemoveOnCombatStart>(), true);
			character.HandleEvent(EventType.JoinCombat, character);
		}

		CombatCells.Add(character.Location);
	}

	public override bool LeaveCombat(IPerceiver character)
	{
		_combatants.Remove(character);
		if (character.Combat == this)
		{
			character.CombatStrategyMode = CombatStrategyMode.StandardMelee;
			character.DefensiveAdvantage = 0;
			character.OffensiveAdvantage = 0;
			character.Combat = null;
			character.CombatTarget = null;
			character.MeleeRange = false;
			character.TargettedBodypart = null;
			character.Aim = null;

			character.RemoveAllEffects(x => x.IsEffectType<IRemoveOnCombatEnd>(), true);
			Gameworld.Scheduler.Destroy(character, ScheduleType.Combat);
			character.HandleEvent(EventType.LeaveCombat, character);
			foreach (var combatant in Combatants.Where(x => x.CombatTarget == character).ToList())
			{
				combatant.CombatTarget = null;
				combatant.AcquireTarget();
				if (combatant.CombatTarget == null && !combatant.CheckCombatStatus())
				{
					LeaveCombat(combatant);
				}
			}

			character.AddEffect(new EngageDelay(character), TimeSpan.FromSeconds(Gameworld.GetStaticDouble("PostCombatEngageDelaySeconds")));
		}

		return CheckForCombatEnd();
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

	public override void TruceRequested(IPerceiver combatant)
	{
		// In a simple melee combat, we apply an effect to the requester, and if all of their opponents or the leader of their opponent's group accept, they are removed from the combat
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
				x.CombatTarget == combatant && x != combatant && x != combatant.CombatTarget && combatant.CanEngage(x));
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
	}

	public bool CheckForCombatEnd()
	{
		if (_combatants.Count < 2)
		{
			EndCombat(true);
			return true;
		}

		if (_combatants.All(x => x.CombatTarget == null || !_combatants.Contains(x.CombatTarget)))
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

	public override string CombatHeaderDescription =>
		$"This is an {"Unrestricted Combat".ColourValue()}, and has no rules or victory conditions.";

	public override string LDescAddendumFor(ICombatant combatant, IPerceiver voyeur)
	{
		if (combatant.CombatTarget == null)
		{
			return $"fighting, but unengaged";
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
			return $"fighting {combatant.CombatTarget.HowSeen(voyeur)} at range";
		}

		var perceivable = combatant as IPerceivable;
		var direction =
			perceivable.PathBetween(combatant.CombatTarget, 10, false, false, true)
			           .DescribeExitDirection();

		return $"fighting {"a distant target".ColourCharacter()} to {direction}";
	}
}