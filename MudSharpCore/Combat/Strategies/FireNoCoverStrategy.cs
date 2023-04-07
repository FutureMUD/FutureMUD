using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Combat.Strategies;

public class FireNoCoverStrategy : RangeBaseStrategy
{
	protected FireNoCoverStrategy()
	{
	}

	public static FireNoCoverStrategy Instance { get; } = new();

	#region Overrides of StandardRangedStrategy

	public override CombatStrategyMode Mode => CombatStrategyMode.FireNoCover;

	protected override ICombatMove ResponseToChargeToMelee(ChargeToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
		var rangedWeapons = GetReadyRangedWeapons(defender).ToList();
		if (!defender.IsEngagedInMelee && rangedWeapons.Any())
		{
			return new StandAndFireMove(defender, assailant as ICharacter, rangedWeapons.First());
		}

		return base.ResponseToChargeToMelee(move, defender, assailant);
	}

	protected override ICombatMove CheckWeaponryLoadout(ICharacter ch)
	{
		if (ch.CombatSettings.WeaponUsePercentage > 0.0 && ch.Race.CombatSettings.CanUseWeapons)
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