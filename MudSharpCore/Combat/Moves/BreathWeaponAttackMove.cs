using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Combat.Moves;

public class BreathWeaponAttackMove : NaturalRangedAttackMoveBase
{
    public BreathWeaponAttackMove(ICharacter owner, INaturalAttack attack, IPerceiver target) : base(owner, attack, target)
    {
    }

    private IBreathWeaponAttack BreathAttack => Attack.GetAttackType<IBreathWeaponAttack>();

    public override string Description => "Using a breath weapon";
    public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.BreathWeaponAttack;
    protected override CheckType RangedCheck => CheckType.BreathWeaponAttack;

    private IEnumerable<IWound> ApplyBreathToVictims(IEnumerable<ICharacter> victims, CheckOutcome attackOutcome,
        OpposedOutcomeDegree degree)
    {
        List<IWound> wounds = new();
        foreach (ICharacter victim in victims)
        {
            for (int i = 0; i < BreathAttack.BodypartsHitPerTarget; i++)
            {
                IBodypart victimBodypart = victim.Body.RandomBodyPartGeometry(Attack.Orientation, Attack.Alignment,
                    Assailant.GetFacingFor(victim, true), false);
                Tuple<IDamage, IDamage> damages = GetDamagePlusSelfDamage(victim, Bodypart, victimBodypart, null, attackOutcome,
                    Attack.Profile.DamageType, Attack.Profile.BaseAngleOfIncidence, NaturalAttack, degree);
                wounds.AddRange(victim.PassiveSufferDamage(damages.Item1));
            }

            if (BreathAttack.FireProfile is not null && BreathAttack.IgniteChance > 0.0 &&
                RandomUtilities.Roll(1.0, BreathAttack.IgniteChance))
            {
                OnFire.Apply(victim, BreathAttack.FireProfile);
            }
        }

        return wounds;
    }

	protected override IEnumerable<IWound> ApplySuccessfulHit(IPerceiver target, CheckOutcome attackOutcome,
		OpposedOutcomeDegree degree, IBodypart bodypart)
	{
		if (target is not ICharacter primaryTarget)
		{
			List<IWound> wounds = base.ApplySuccessfulHit(target, attackOutcome, degree, bodypart).ToList();
			if (BreathAttack.FireProfile is not null && BreathAttack.IgniteChance > 0.0 &&
				RandomUtilities.Roll(1.0, BreathAttack.IgniteChance))
			{
				OnFire.Apply(target, BreathAttack.FireProfile);
			}

			return wounds;
		}

		List<ICharacter> victims = SelectBreathVictims(primaryTarget, Assailant,
			BreathAttack.AdditionalTargetLimit).ToList();
		return ApplyBreathToVictims(victims, attackOutcome, degree);
	}

	internal static IEnumerable<ICharacter> SelectBreathVictims(ICharacter primaryTarget, ICharacter assailant,
		int additionalTargetLimit)
	{
		return new[] { primaryTarget }
			.Concat(primaryTarget.LocalThingsAndProximities()
				.Where(x => x.Proximity <= Proximity.Proximate)
				.Select(x => x.Thing)
				.OfType<ICharacter>())
			.Where(x => x != assailant)
			.Distinct()
			.Take(Math.Max(0, additionalTargetLimit) + 1);
	}

    protected override void HandleScatterImpact(RangedScatterResult scatter, CheckOutcome attackOutcome)
    {
        List<ICharacter> victims = scatter.Cell.LayerCharacters(scatter.RoomLayer)
                           .Where(x => x != Assailant)
                           .Take(BreathAttack.AdditionalTargetLimit + 1)
                           .ToList();
        ApplyBreathToVictims(victims, attackOutcome, OpposedOutcomeDegree.None).ProcessPassiveWounds();
    }
}
