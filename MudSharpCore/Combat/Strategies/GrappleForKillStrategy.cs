using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Body.PartProtos;

namespace MudSharp.Combat.Strategies;

public class GrappleForKillStrategy : GrappleForIncapacitationStrategy
{
	public override CombatStrategyMode Mode => CombatStrategyMode.GrappleForKill;

	protected GrappleForKillStrategy()
	{
	}

	public new static GrappleForKillStrategy Instance { get; } = new();

	protected override ICombatMove AttemptGrappleInProgress(ICharacter ch, IGrappling grapple,
		IEnumerable<(ILimb Limb, bool Grappled)> limbStates)
	{
		if (RandomUtilities.DoubleRandom(0.0, 1.0) < Math.Pow(0.5, limbStates.Count(x => x.Grappled)))
		{
			return base.AttemptGrappleInProgress(ch, grapple, limbStates);
		}

		var tch = grapple.Target;
		var possibleAttacks = ch.Race
		                        .UsableNaturalWeaponAttacks(ch, tch, false, BuiltInCombatMoveType.WrenchAttack,
			                        BuiltInCombatMoveType.StrangleAttack)
		                        .Select(x => (Attack: x, TargetAttack: x.Attack as ITargetLimbWeaponAttack))
		                        .Where(x => limbStates.Any(y =>
			                        ((x.TargetAttack != null && y.Limb.LimbType == x.TargetAttack.TargetLimbType) ||
			                         (x.TargetAttack == null && y.Limb.LimbType == LimbType.Head)) && y.Grappled))
		                        .ToList();
		var attacks = possibleAttacks
		              .Where(x => ch.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(ch, x.Attack.Attack))).ToList();
		if (!attacks.Any())
		{
			if (possibleAttacks.Any())
			{
				return new TooExhaustedMove { Assailant = ch };
			}

			return base.AttemptGrappleInProgress(ch, grapple, limbStates);
		}

		var (attack, targetAttack) = attacks.GetWeightedRandom(x => x.Attack.Attack.Weighting);
		if (attack.Attack.MoveType == BuiltInCombatMoveType.WrenchAttack)
		{
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
		else
		{
			return new StrangleAttack(ch, attack, tch);
		}
	}
}