#nullable enable
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;

namespace MudSharp.Framework;

public abstract partial class PerceiverItem
{
	// Includes same-reference assignments: returning to an old combat/target is not its old admission.
	internal long CombatMutationVersion { get; private protected set; }

	internal Action CommitSelectiveDetach(MudSharp.Character.Character actor)
	{
		var aim = _aim;
		if (aim is not null) aim.AimInvalidated -= Aim_AimInvalidated;
		_combat = null;
		_combatTarget = null;
		_aim = null;
		TargettedBodypart = null;
		actor.ClearMeleeRangeForCessation();
		CombatStrategyMode = CombatStrategyMode.StandardMelee;
		DefensiveAdvantage = 0;
		OffensiveAdvantage = 0;
		CombatMutationVersion++;
		return () =>
		{
			if (aim is null) return;
			aim.ReleaseEvents();
		};
	}

	internal void NotifySelectiveLeave() => PerceiverLeaveCombat();

	internal void CommitSelectiveTargetClear()
	{
		_combatTarget = null;
		TargettedBodypart = null;
		CombatMutationVersion++;
	}

	internal IEffect[] SelectiveTargetEffects() => EffectHandler.Effects
		.Where(x => x is ICombatEffectRemovedOnTargetChange).ToArray();

	internal IEffect[] SelectiveLeaveEffects() => EffectHandler.Effects
		.Where(x => x is ISelectedCombatAction || x is IRemoveOnCombatEnd || x is ICombatEffectRemovedOnTargetChange)
		.OrderByDescending(x => x is ISelectedCombatAction).ToArray();
}
