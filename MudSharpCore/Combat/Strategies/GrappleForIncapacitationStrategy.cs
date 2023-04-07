using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Strategies;

public class GrappleForIncapacitationStrategy : GrappleForControlStrategy
{
	public override CombatStrategyMode Mode => CombatStrategyMode.GrappleForIncapitation;

	protected GrappleForIncapacitationStrategy()
	{
	}

	public new static GrappleForIncapacitationStrategy Instance { get; } = new();

	protected override ICombatMove AttemptGrappleInProgress(ICharacter ch, IGrappling grapple,
		IEnumerable<(ILimb Limb, bool Grappled)> limbStates)
	{
		if (RandomUtilities.DoubleRandom(0.0, 1.0) <= Math.Pow(0.5, limbStates.Count(x => x.Grappled)))
		{
			return base.AttemptGrappleInProgress(ch, grapple, limbStates);
		}

		var tch = grapple.Target;
		var possibleWrenches = ch.Race
		                         .UsableNaturalWeaponAttacks(ch, tch, false, BuiltInCombatMoveType.WrenchAttack)
		                         .Select(x => (Attack: x, TargetAttack: (ITargetLimbWeaponAttack)x.Attack))
		                         .Where(x => x.TargetAttack.TargetLimbType != LimbType.Head &&
		                                     limbStates.Any(y =>
			                                     y.Limb.LimbType == x.TargetAttack.TargetLimbType && y.Grappled))
		                         .ToList();
		var wrenchAttacks = possibleWrenches
		                    .Where(x => ch.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(ch, x.TargetAttack)))
		                    .ToList();
		if (!wrenchAttacks.Any())
		{
			if (possibleWrenches.Any())
			{
				return new TooExhaustedMove { Assailant = ch };
			}

			return base.AttemptGrappleInProgress(ch, grapple, limbStates);
		}

		var (attack, targetAttack) = wrenchAttacks.GetWeightedRandom(x => x.TargetAttack.Weighting);
		var targetLimb = limbStates.Where(x => x.Grappled && x.Limb.LimbType == targetAttack.TargetLimbType)
		                           .GetRandomElement().Limb;
		var bodypart = tch.Body.BodypartsForLimb(targetLimb).Where(x => x is JointProto)
		                  .GetWeightedRandom(x => x.RelativeHitChance);
		if (bodypart == null)
		{
			return base.AttemptGrappleInProgress(ch, grapple, limbStates);
		}

		return new WrenchingAttack(ch, attack, tch, bodypart);
	}
}