using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Combat.Strategies;

public class SkirmishStrategy : StandardMeleeStrategy
{
	protected SkirmishStrategy()
	{
	}

	public new static SkirmishStrategy Instance { get; } = new();

	#region Overrides of StandardRangedStrategy

	public override CombatStrategyMode Mode => CombatStrategyMode.Skirmish;

	protected override ICombatMove ResponseToChargeToMelee(ChargeToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
		var rangedWeapons = GetReadyRangedWeapons(defender).ToList();
		if (defender.CanSpendStamina(SkirmishResponseMove.MoveStaminaCost(defender)) && !defender.MeleeRange &&
		    defender.EffectsOfType<IRageEffect>().All(x => !x.IsSuperRaging))
		{
			if (rangedWeapons.Any())
			{
				return new SkirmishAndFire(defender, move.Assailant, rangedWeapons.First());
			}

			return new SkirmishResponseMove { Assailant = defender };
		}

		return base.ResponseToChargeToMelee(move, defender, assailant);
	}

	protected override ICombatMove ResponseToMoveToMelee(MoveToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
		var rangedWeapons = GetReadyRangedWeapons(defender).ToList();
		if (defender.CanSpendStamina(SkirmishResponseMove.MoveStaminaCost(defender)) && !defender.MeleeRange &&
		    defender.EffectsOfType<IRageEffect>().All(x => !x.IsSuperRaging))
		{
			if (rangedWeapons.Any())
			{
				return new SkirmishAndFire(defender, move.Assailant, rangedWeapons.First());
			}

			return new SkirmishResponseMove { Assailant = defender };
		}

		return base.ResponseToMoveToMelee(move, defender, assailant);
	}

	protected override ICombatMove HandleCombatMovement(IPerceiver combatant)
	{
		ICombatMove move;
		if ((move = base.HandleCombatMovement(combatant)) != null)
		{
			return move;
		}

		switch (combatant.CombatSettings.MovementManagement)
		{
			case AutomaticMovementSettings.FullyAutomatic:
			case AutomaticMovementSettings.KeepRange:
				break;
			case AutomaticMovementSettings.FullyManual:
			case AutomaticMovementSettings.SeekCoverOnly:
				return null;
		}

		if (combatant is ICharacter ch)
		{
			if (ch.MeleeRange || ch.Combat.Combatants.Except(ch).Any(x => x.CombatTarget == ch && x.MeleeRange))
			{
				if (ch.EffectsOfType<IRageEffect>().All(x => !x.IsSuperRaging))
				{
					return new FleeMove { Assailant = ch };
				}
			}
		}

		return null;
	}

	protected override ICombatMove CheckWeaponryLoadout(ICharacter ch)
	{
		if (!ch.IsEngagedInMelee && ch.CombatSettings.WeaponUsePercentage > 0.0 && ch.Race.CombatSettings.CanUseWeapons)
		{
			return ch.Body.WieldedItems.Any(x =>
				x.GetItemType<IRangedWeapon>() is IRangedWeapon rw &&
				ch.CombatSettings.ClassificationsAllowed.Contains(rw.Classification) &&
				(rw.ReadyToFire || (!rw.IsLoaded && rw.CanLoad(ch, true)) || (!rw.IsReadied && rw.CanReady(ch))))
				? null
				: AttemptGetRangedWeapon(ch) ?? base.CheckWeaponryLoadout(ch);
		}

		return base.CheckWeaponryLoadout(ch);
	}

	#endregion
}