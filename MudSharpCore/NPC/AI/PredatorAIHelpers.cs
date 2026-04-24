#nullable enable
using MudSharp.Body.Needs;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Linq;

namespace MudSharp.NPC.AI;

internal static class PredatorAIHelpers
{
	public static bool IsHungry(ICharacter character)
	{
		return character.NeedsModel.Status.IsHungry();
	}

	public static bool CouldEatAfterKilling(ICharacter predator, ICharacter target)
	{
		if (!predator.Race.CanEatCorpses)
		{
			return false;
		}

		if (target.Race.CorpseModel?.CreateCorpse != true)
		{
			return false;
		}

		return predator.Race.CanEatCorpseMaterial(target.Race.CorpseModel.CorpseMaterial(0.0));
	}

	public static bool WillAttack(ICharacter aggressor, ICharacter target, IFutureProg willAttackProg,
		bool requireHungryPredator)
	{
		if (aggressor.State.HasFlag(CharacterState.Dead) || aggressor.State.HasFlag(CharacterState.Stasis))
		{
			return false;
		}

		if (aggressor.Combat != null)
		{
			return false;
		}

		if (aggressor == target)
		{
			return false;
		}

		if (!CharacterState.Able.HasFlag(aggressor.State))
		{
			return false;
		}

		if (requireHungryPredator && (!IsHungry(aggressor) || !CouldEatAfterKilling(aggressor, target)))
		{
			return false;
		}

		if (!aggressor.CanSee(target))
		{
			return false;
		}

		if (!willAttackProg.ExecuteBool(aggressor, target))
		{
			return false;
		}

		if (aggressor.CombatTarget != target && !aggressor.CanEngage(target))
		{
			return false;
		}

		if (aggressor.Effects.Any(x => x.IsBlockingEffect("combat-engage")) ||
		    aggressor.Effects.Any(x => x.IsBlockingEffect("general")))
		{
			return false;
		}

		if (!aggressor.CombatSettings.AttackCriticallyInjured && target.HealthStrategy.IsCriticallyInjured(target))
		{
			return false;
		}

		if (!aggressor.CombatSettings.AttackHelpless && target.State.IsDisabled())
		{
			return false;
		}

		return true;
	}

	public static void EngageTarget(ICharacter ch, IPerceiver tp, string engageEmote)
	{
		if (ch.State.HasFlag(CharacterState.Dead) || ch.Corpse != null)
		{
			return;
		}

		if (tp is ICharacter tch && tch.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		if (!ch.CanEngage(tp))
		{
			return;
		}

		if (!string.IsNullOrWhiteSpace(engageEmote))
		{
			ch.OutputHandler.Handle(new EmoteOutput(new Emote(engageEmote, ch, ch, tp)));
		}

		ch.Engage(tp, ch.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
					 .Where(x => x.IsReadied || x.CanReady(ch))
					 .Select(x => (int)x.WeaponType.DefaultRangeInRooms)
					 .DefaultIfEmpty(-1)
					 .Max() > 0);
	}

	public static bool CheckForAttack(ICharacter aggressor, ICharacter target, IFutureProg willAttackProg,
		string engageDelayDiceExpression, string engageEmote, bool requireHungryPredator)
	{
		if (!WillAttack(aggressor, target, willAttackProg, requireHungryPredator))
		{
			return false;
		}

		aggressor.AddEffect(
			new BlockingDelayedAction(aggressor,
				_ => EngageTarget(aggressor, target, engageEmote),
				"preparing to engage in combat",
				new[] { "general", "combat-engage", "movement" },
				null),
			TimeSpan.FromMilliseconds(Dice.Roll(engageDelayDiceExpression)));

		return true;
	}
}
