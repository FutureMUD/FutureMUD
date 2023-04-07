using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Combat.Strategies;

public class StandardRangeStrategy : CoverSeekingRangedStrategy
{
	public override CombatStrategyMode Mode => CombatStrategyMode.StandardRange;

	protected StandardRangeStrategy()
	{
	}

	public static StandardRangeStrategy Instance { get; } = new();

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

	#region Overrides of StrategyBase

	protected override bool ShouldCharacterStand(ICharacter ch)
	{
		if (ch.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
		      .Any(rw =>
			      (rw.ReadyToFire || (!rw.IsLoaded && rw.CanLoad(ch, true)) || (!rw.IsReadied && rw.CanReady(ch))) &&
			      rw.WeaponType.RangedWeaponType.MinimumFiringPosition().CompareTo(ch.PositionState)
			        .In(PositionHeightComparison.Higher, PositionHeightComparison.Undefined)))
		{
		}

		return base.ShouldCharacterStand(ch);
	}

	#endregion
}