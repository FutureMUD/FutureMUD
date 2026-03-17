using System.Collections.Generic;
using System.Linq;
using System;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class BreathWeaponAttackMove : NaturalRangedAttackMoveBase
{
	public BreathWeaponAttackMove(ICharacter owner, INaturalAttack attack, ICharacter target) : base(owner, attack, target)
	{
	}

	private IBreathWeaponAttack BreathAttack => Attack.GetAttackType<IBreathWeaponAttack>();

	public override string Description => "Using a breath weapon";
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.BreathWeaponAttack;
	protected override CheckType RangedCheck => CheckType.BreathWeaponAttack;

	private IEnumerable<IWound> ApplyBreathToVictims(IEnumerable<ICharacter> victims, CheckOutcome attackOutcome,
		OpposedOutcomeDegree degree)
	{
		var wounds = new List<IWound>();
		foreach (var victim in victims)
		{
			for (var i = 0; i < BreathAttack.BodypartsHitPerTarget; i++)
			{
				var victimBodypart = victim.Body.RandomBodyPartGeometry(Attack.Orientation, Attack.Alignment,
					Assailant.GetFacingFor(victim, true), false);
				var damages = GetDamagePlusSelfDamage(victim, Bodypart, victimBodypart, null, attackOutcome,
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
		var victims = target.LocalThingsAndProximities()
		                  .Where(x => x.Thing is ICharacter)
		                  .OrderBy(x => x.Proximity)
		                  .Select(x => x.Thing)
		                  .OfType<ICharacter>()
		                  .Distinct()
		                  .Take(BreathAttack.AdditionalTargetLimit + 1)
		                  .ToList();
		return ApplyBreathToVictims(victims, attackOutcome, degree);
	}

	protected override void HandleScatterImpact(RangedScatterResult scatter, CheckOutcome attackOutcome)
	{
		var victims = scatter.Cell.LayerCharacters(scatter.RoomLayer)
		                   .Where(x => x != Assailant)
		                   .Take(BreathAttack.AdditionalTargetLimit + 1)
		                   .ToList();
		ApplyBreathToVictims(victims, attackOutcome, OpposedOutcomeDegree.None).ProcessPassiveWounds();
	}
}
