using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Framework;
using MudSharp.Magic.Powers;

namespace MudSharp.Combat.Strategies;

public class MeleeMagicStrategy : StandardMeleeStrategy
{
	public new static MeleeMagicStrategy Instance => new();

	protected MeleeMagicStrategy()
	{
	}

	public override CombatStrategyMode Mode => CombatStrategyMode.MeleeMagic;

	protected override ICombatMove AttemptUseMagic(ICharacter combatant)
	{
		var possiblePowers = combatant.Powers
		                              .OfType<IMagicAttackPower>()
		                              .Where(x =>
			                              !combatant.CombatSettings.ForbiddenSchools.Contains(x.School) &&
			                              (x.PowerIntentions & combatant.CombatSettings.ForbiddenIntentions) == 0 &&
			                              x.PowerIntentions.HasFlag(combatant.CombatSettings.RequiredIntentions) &&
			                              x.CanInvokePower(combatant, combatant.CombatTarget as ICharacter))
		                              .ToList();
		var preferredPowers = possiblePowers
		                      .Where(x => x.PowerIntentions.HasFlag(combatant.CombatSettings.PreferredIntentions))
		                      .ToList();
		if (possiblePowers.Any())
		{
			if (preferredPowers.Any() && Dice.Roll(1, 2) == 1)
			{
				return new MagicPowerAttackMove(combatant, combatant.CombatTarget as ICharacter,
					preferredPowers.GetWeightedRandom(x => x.Weighting));
			}

			return new MagicPowerAttackMove(combatant, combatant.CombatTarget as ICharacter,
				possiblePowers.GetWeightedRandom(x => x.Weighting));
		}

		return HandleWeaponAttackRolled(combatant);
	}

	protected override ICombatMove AttemptUsePsychicAbility(ICharacter combatant)
	{
		var possiblePowers = combatant.Powers
		                              .OfType<IMagicAttackPower>()
		                              .Where(x =>
			                              !combatant.CombatSettings.ForbiddenSchools.Contains(x.School) &&
			                              (x.PowerIntentions & combatant.CombatSettings.ForbiddenIntentions) == 0 &&
			                              x.PowerIntentions.HasFlag(combatant.CombatSettings.RequiredIntentions) &&
			                              x.CanInvokePower(combatant, combatant.CombatTarget as ICharacter))
		                              .ToList();
		var preferredPowers = possiblePowers
		                      .Where(x => x.PowerIntentions.HasFlag(combatant.CombatSettings.PreferredIntentions))
		                      .ToList();
		if (possiblePowers.Any())
		{
			if (preferredPowers.Any() && Dice.Roll(1, 2) == 1)
			{
				return new MagicPowerAttackMove(combatant, combatant.CombatTarget as ICharacter,
					preferredPowers.GetWeightedRandom(x => x.Weighting));
			}

			return new MagicPowerAttackMove(combatant, combatant.CombatTarget as ICharacter,
				possiblePowers.GetWeightedRandom(x => x.Weighting));
		}

		return HandleWeaponAttackRolled(combatant);
	}
}