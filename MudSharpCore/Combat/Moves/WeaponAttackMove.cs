using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public abstract class WeaponAttackMove : CombatMoveBase, IWeaponAttackMove
{
	protected WeaponAttackMove(IWeaponAttack attack)
	{
		Attack = attack;
	}

	public IMeleeWeapon Weapon { get; init; }

	public virtual double BloodSprayMultiplier => 0.0;

	protected void CheckLodged(IEnumerable<IWound> wounds)
	{
		foreach (var item in wounds.Where(x => x.Lodged != null)
		                           .Select(x => (Item: x.Lodged, Bodypart: x.Bodypart, Owner: x.Parent)).ToList())
		{
			Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ has become lodged in {(item.Bodypart != null ? $"$0's {item.Bodypart.FullDescription()}" : "$0")}!",
				item.Item, item.Owner)));
			item.Item.InInventoryOf?.Take(item.Item);
		}
	}

	protected double CalculateRelativeHardnessForWeapon(IMeleeWeapon weapon, DamageType damageType, Outcome outcome,
		IPerceivable target, [CanBeNull] IBodypart targetBodypart)
	{
		var damageTypeMultiplier = 0.0;
		switch (damageType)
		{
			case DamageType.Slashing:
			case DamageType.Chopping:
			case DamageType.Shearing:
				damageTypeMultiplier = 5.0;
				break;
			case DamageType.Crushing:
				break;
			case DamageType.Piercing:
				damageTypeMultiplier = 1.0;
				break;
			case DamageType.Ballistic:
			case DamageType.BallisticArmourPiercing:
			case DamageType.ArmourPiercing:
			case DamageType.Wrenching:
			case DamageType.Bite:
			case DamageType.Claw:
				damageTypeMultiplier = 10.0;
				break;
			case DamageType.Burning:
			case DamageType.Freezing:
			case DamageType.Chemical:
			case DamageType.Shockwave:
			case DamageType.Electrical:
			case DamageType.Hypoxia:
			case DamageType.Cellular:
			case DamageType.Sonic:
			case DamageType.Shrapnel:
			case DamageType.Necrotic:
			case DamageType.Falling:
			case DamageType.Eldritch:
			case DamageType.Arcane:
				// All of these damage types shouldn't use any physical relative hardness, they are different kinds of energy/interaction
				return 1.0;
		}

		var successMultiplier = 1.0;
		switch (outcome)
		{
			case Outcome.MajorFail:
				successMultiplier = 1.0;
				break;
			case Outcome.Fail:
				successMultiplier = 2.0;
				break;
			case Outcome.MinorFail:
				successMultiplier = 3.0;
				break;
			case Outcome.MinorPass:
				successMultiplier = 5.0;
				break;
			case Outcome.Pass:
				successMultiplier = 10.0;
				break;
			case Outcome.MajorPass:
				successMultiplier = 15.0;
				break;
		}

		var attackerAverageHardness = weapon.Parent.Material.Density;

		var defenderAverageHardness = target is ICharacter tch && targetBodypart != null
			? tch.Body.WornItemsProfilesFor(targetBodypart)
			     .Where(x => !x.Item2.NoArmour && x.Item1.IsItemType<IArmour>())
			     .SelectNotNull(x => x.Item1.Material as ISolid)
			     .Plus(tch.Body.GetMaterial(targetBodypart) as ISolid)
			     .Max(x => x?.Density ?? 0)
			: (target as IGameItem)?.Material.Density ?? 0;

		return attackerAverageHardness * (successMultiplier + damageTypeMultiplier) /
		       (attackerAverageHardness * (successMultiplier + damageTypeMultiplier) + defenderAverageHardness);
	}

	//Return a factor that compares the hardness of the attacking material and the target material
	protected double CalculateRelativeHardness(ICharacter assailant, ICharacter target,
		IBodypart attackingBodypart, IBodypart targetBodypart, ISolid targetMaterial,
		Outcome attackOutcome, DamageType damageType)
	{
		var attackerAverageHardness = Assailant.Body.WornItemsProfilesFor(attackingBodypart)
		                                       .Where(x => !x.Item2.NoArmour && x.Item1.IsItemType<IArmour>())
		                                       .SelectNotNull(x => x.Item1.Material as ISolid)
		                                       .Plus(Assailant.Body.GetMaterial(attackingBodypart) as ISolid)
		                                       .Max(x => x?.Density ?? 1.0);

		var successMultiplier = 1.0;
		switch (attackOutcome)
		{
			case Outcome.MajorFail:
				successMultiplier = 0.5;
				break;
			case Outcome.Fail:
				successMultiplier = 1.0;
				break;
			case Outcome.MinorFail:
				successMultiplier = 1.5;
				break;
			case Outcome.MinorPass:
				successMultiplier = 2.75;
				break;
			case Outcome.Pass:
				successMultiplier = 4.0;
				break;
			case Outcome.MajorPass:
				successMultiplier = 10.0;
				break;
		}

		var defenderAverageHardness = targetBodypart != null
			? target.Body.WornItemsProfilesFor(targetBodypart)
			        .Where(x => !x.Item2.NoArmour && x.Item1.IsItemType<IArmour>())
			        .SelectNotNull(x => x.Item1.Material as ISolid)
			        .Plus(target.Body.GetMaterial(targetBodypart) as ISolid)
			        .Max(x => x?.Density ?? 0)
			: targetMaterial?.Density ?? 0;

		var relativeHardness = 1.0;
		switch (damageType)
		{
			case DamageType.Crushing:
				relativeHardness = attackerAverageHardness * successMultiplier /
				                   (attackerAverageHardness * successMultiplier + defenderAverageHardness);
				break;
			case DamageType.Claw:
				relativeHardness = attackerAverageHardness * successMultiplier * 20 /
				                   (attackerAverageHardness * successMultiplier * 20 + defenderAverageHardness);
				break;
		}

		return relativeHardness;
	}

	protected (IDamage TargetDamage, IDamage SelfDamage) GetDamagePlusSelfDamageForWeapon(IMeleeWeapon weapon,
		Outcome outcome,
		OpposedOutcomeDegree degree, IPerceivable target, [CanBeNull] IBodypart targetBodypart, double attackAngle)
	{
		var relativeHardness =
			CalculateRelativeHardnessForWeapon(weapon, Attack.Profile.DamageType, outcome, target, targetBodypart);

		Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)degree;
		Attack.Profile.DamageExpression.Formula.Parameters["quality"] = (int)weapon.Parent.Quality;
		Attack.Profile.StunExpression.Formula.Parameters["degree"] = (int)degree;
		Attack.Profile.StunExpression.Formula.Parameters["quality"] = (int)weapon.Parent.Quality;
		Attack.Profile.PainExpression.Formula.Parameters["degree"] = (int)degree;
		Attack.Profile.PainExpression.Formula.Parameters["quality"] = (int)weapon.Parent.Quality;

		var damageResult =
			Attack.Profile.DamageExpression.Evaluate(Assailant, context: TraitBonusContext.ArmedDamageCalculation);
		var stunResult =
			Attack.Profile.DamageExpression.Evaluate(Assailant, context: TraitBonusContext.ArmedDamageCalculation);
		var painResult =
			Attack.Profile.DamageExpression.Evaluate(Assailant, context: TraitBonusContext.ArmedDamageCalculation);

		if (!Gameworld.GetStaticBool("WeaponsTakeDamageFromAttacks"))
		{
			return (new Damage
				{
					ActorOrigin = Assailant,
					LodgableItem = null,
					ToolOrigin = null,
					AngleOfIncidentRadians = attackAngle,
					Bodypart = targetBodypart,
					DamageAmount = damageResult * 2.0 * attackAngle / Math.PI,
					DamageType = Attack.Profile.DamageType,
					PainAmount = painResult * 2.0 * attackAngle / Math.PI,
					PenetrationOutcome =
						Gameworld.GetCheck(CheckType.MeleeWeaponPenetrateCheck)
						         .Check(Assailant, GetPenetrationDifficulty(Attack.Profile.DamageType), target),
					ShockAmount = 0,
					StunAmount = stunResult * 2.0 * attackAngle / Math.PI
				},
				null);
		}

		var finalDamage = new Damage
		{
			ActorOrigin = Assailant,
			LodgableItem = null,
			ToolOrigin = null,
			AngleOfIncidentRadians = attackAngle,
			Bodypart = targetBodypart,
			DamageAmount = damageResult * relativeHardness * 2.0 * attackAngle / Math.PI,
			DamageType = Attack.Profile.DamageType,
			PainAmount = painResult * relativeHardness * 2.0 * attackAngle / Math.PI,
			PenetrationOutcome =
				Gameworld.GetCheck(CheckType.MeleeWeaponPenetrateCheck)
				         .Check(Assailant, GetPenetrationDifficulty(Attack.Profile.DamageType), target),
			ShockAmount = 0,
			StunAmount = stunResult * relativeHardness * 2.0 * attackAngle / Math.PI
		};

		var selfDamage = new Damage
		{
			ActorOrigin = Assailant,
			LodgableItem = null,
			ToolOrigin = null,
			AngleOfIncidentRadians = attackAngle,
			DamageAmount = damageResult * (1.0 - relativeHardness) * 2 * attackAngle / Math.PI,
			DamageType = DamageType.Crushing,
			PainAmount = painResult * (1.0 - relativeHardness) * 2 * attackAngle / Math.PI,
			PenetrationOutcome = new CheckOutcome { Outcome = Outcome.MajorFail },
			ShockAmount = 0,
			StunAmount = stunResult * (1.0 - relativeHardness) * 2 * attackAngle / Math.PI
		};

		return (finalDamage, selfDamage);
	}

	protected Tuple<IDamage, IDamage> GetDamagePlusSelfDamage(ICharacter target,
		IBodypart attackingBodypart, IBodypart targetBodypart, ISolid targetMaterial,
		Outcome attackOutcome, DamageType damageType, double attackAngle, INaturalAttack attack,
		OpposedOutcomeDegree degree)
	{
		var relativeHardness = CalculateRelativeHardness(Assailant, target, attackingBodypart, targetBodypart,
			targetMaterial, attackOutcome, damageType);

		attack.Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)degree;
		attack.Attack.Profile.DamageExpression.Formula.Parameters["quality"] =
			(int)Assailant.NaturalWeaponQuality(attack);
		attack.Attack.Profile.StunExpression.Formula.Parameters["degree"] = (int)degree;
		attack.Attack.Profile.StunExpression.Formula.Parameters["quality"] =
			(int)Assailant.NaturalWeaponQuality(attack);
		attack.Attack.Profile.PainExpression.Formula.Parameters["degree"] = (int)degree;
		attack.Attack.Profile.PainExpression.Formula.Parameters["quality"] =
			(int)Assailant.NaturalWeaponQuality(attack);

		var damageResult =
			attack.Attack.Profile.DamageExpression.Evaluate(Assailant,
				context: TraitBonusContext.UnarmedDamageCalculation);
		var stunResult =
			attack.Attack.Profile.DamageExpression.Evaluate(Assailant,
				context: TraitBonusContext.UnarmedDamageCalculation);
		var painResult =
			attack.Attack.Profile.DamageExpression.Evaluate(Assailant,
				context: TraitBonusContext.UnarmedDamageCalculation);

		var finalDamage = new Damage
		{
			ActorOrigin = Assailant,
			LodgableItem = null,
			ToolOrigin = null,
			AngleOfIncidentRadians = attackAngle,
			Bodypart = targetBodypart,
			DamageAmount = damageResult * relativeHardness * 2 * attackAngle / Math.PI,
			DamageType = attack.Attack.Profile.DamageType,
			PainAmount = painResult * relativeHardness * 2 * attackAngle / Math.PI,
			PenetrationOutcome =
				Gameworld.GetCheck(CheckType.MeleeWeaponPenetrateCheck)
				         .Check(Assailant, GetPenetrationDifficulty(Attack.Profile.DamageType), target),
			ShockAmount = 0,
			StunAmount = stunResult * relativeHardness * 2 * attackAngle / Math.PI
		};

		var selfDamage = new Damage
		{
			ActorOrigin = Assailant,
			LodgableItem = null,
			ToolOrigin = null,
			AngleOfIncidentRadians = attackAngle,
			Bodypart = attackingBodypart,
			DamageAmount = damageResult * (1.0 - relativeHardness) * 2 * attackAngle / Math.PI,
			DamageType = DamageType.Crushing,
			PainAmount = painResult * (1.0 - relativeHardness) * 2 * attackAngle / Math.PI,
			PenetrationOutcome = new CheckOutcome { Outcome = Outcome.MajorFail },
			ShockAmount = 0,
			StunAmount =
				stunResult * (1.0 - relativeHardness) * 2 * attackAngle / Math.PI
		};

		return new Tuple<IDamage, IDamage>(finalDamage, selfDamage);
	}

	protected void SelectWardFreeAttack(ICharacter counterAttacker, ICharacter target, WardResult wardResult,
		ref IWeaponAttack weaponAttack, ref INaturalAttack naturalAttack)
	{
		var weaponAttacks = wardResult.WardWeapon?.WeaponType.UsableAttacks(
			counterAttacker, wardResult.WardWeapon?.Parent,
			target, wardResult.WardWeapon.HandednessForWeapon(counterAttacker), false,
			BuiltInCombatMoveType.WardFreeAttack).ToList();
		var naturalAttacks = counterAttacker.Race.UsableNaturalWeaponAttacks(counterAttacker, target, false,
			BuiltInCombatMoveType.WardFreeUnarmedAttack).ToList();

		//Check attack preference flags
		var preferredWeapon = weaponAttacks
		                      ?.Where(x => x.Intentions.HasFlag(counterAttacker.CombatSettings.PreferredIntentions))
		                      .ToList();
		if (preferredWeapon?.Any() == true && Dice.Roll(1, 2) == 1)
		{
			weaponAttacks = preferredWeapon;
		}

		var preferredNatural = naturalAttacks
		                       .Where(x => x.Attack.Intentions.HasFlag(counterAttacker.CombatSettings
			                       .PreferredIntentions)).ToList();
		if (preferredNatural.Any() && Dice.Roll(1, 2) == 1)
		{
			naturalAttacks = preferredNatural;
		}

		//Get our random attacks
		var wAttack = weaponAttacks?.GetWeightedRandom(x => x.Weighting);
		var nAttack = naturalAttacks.GetWeightedRandom(x => x.Attack.Weighting);

		//If we have both options now, respect percentage settings
		if (wAttack != null && nAttack != null)
		{
			//Try weapon
			var roll = Constants.Random.NextDouble();
			if (counterAttacker.CombatSettings.WeaponUsePercentage > 0 &&
			    roll <= counterAttacker.CombatSettings.WeaponUsePercentage)
			{
				weaponAttack = wAttack;
				return;
			}

			//Try natural attack
			roll -= counterAttacker.CombatSettings.WeaponUsePercentage;
			if (counterAttacker.CombatSettings.NaturalWeaponPercentage > 0 &&
			    roll <= counterAttacker.CombatSettings.NaturalWeaponPercentage)
			{
				naturalAttack = nAttack;
			}
		}
		else
		{
			weaponAttack = wAttack;
			naturalAttack = nAttack;
		}
	}

	//Execute a weapon based ward counter attack
	//Sticking to the conventions in ProcessWardFreeAttack(), aggressor is the one that attacked the
	//warder and defender is the warder taking their free counter attack. I.e., defender is attacking
	//aggressor in the following code.
	protected IEnumerable<IWound> AttemptWardFreeWeaponAttack(ICharacter aggressor, ICharacter defender,
		WardResult wardResult, IWeaponAttack weaponAttack)
	{
		var targetBodypart = aggressor.Body.RandomBodyPartGeometry(weaponAttack.Orientation,
			weaponAttack.Alignment, Facing.Front);
		var attackRoll = Gameworld.GetCheck(CheckType.MeleeWeaponCheck)
		                          .Check(defender, weaponAttack.Profile.BaseAttackerDifficulty,
			                          wardResult.WardWeapon.WeaponType.AttackTrait, aggressor);
		var defenseRoll = Gameworld.GetCheck(CheckType.DodgeCheck)
		                           .Check(aggressor, weaponAttack.Profile.BaseDodgeDifficulty, defender);
		var result = new OpposedOutcome(attackRoll, defenseRoll);
		var attackEmote = Gameworld.CombatMessageManager
		                           .GetMessageFor(defender, aggressor,
			                           wardResult.WardWeapon.Parent, weaponAttack,
			                           BuiltInCombatMoveType.WardFreeAttack,
			                           attackRoll, null);

		if (result.Outcome == OpposedOutcomeDirection.Opponent)
		{
			var dodgeEmote =
				Gameworld.CombatMessageManager.GetMessageFor(
					aggressor, defender, null, weaponAttack, BuiltInCombatMoveType.Dodge, defenseRoll,
					null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{string.Format(dodgeEmote, "", targetBodypart.FullDescription())}"
							.Fullstop(), defender,
						defender, aggressor, wardResult.WardWeapon.Parent), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap));
			return Enumerable.Empty<IWound>();
		}
		else
		{
			var dodgeEmote =
				Gameworld.CombatMessageManager.GetFailMessageFor(
					aggressor, defender, null, weaponAttack, BuiltInCombatMoveType.Dodge, defenseRoll,
					null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{string.Format(dodgeEmote, "", targetBodypart.FullDescription())}".Fullstop(),
						defender,
						defender, aggressor, wardResult.WardWeapon.Parent), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap));
		}
#if DEBUG
		Console.WriteLine(
			$"WeaponAttackMove ProcessWardFreeAttack Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif
		var finalDegree = result.Outcome == OpposedOutcomeDirection.Opponent ? 0 : (int)result.Degree;
		var angleMultiplier = 1.0;
		switch (result.Degree)
		{
			case OpposedOutcomeDegree.None:
				angleMultiplier = 0.5;
				break;
			case OpposedOutcomeDegree.Marginal:
				angleMultiplier = 0.6;
				break;
			case OpposedOutcomeDegree.Minor:
				angleMultiplier = 0.7;
				break;
			case OpposedOutcomeDegree.Moderate:
				angleMultiplier = 0.8;
				break;
			case OpposedOutcomeDegree.Major:
				angleMultiplier = 0.9;
				break;
			case OpposedOutcomeDegree.Total:
				angleMultiplier = 1.0;
				break;
		}

		var finalAngle = weaponAttack.Profile.BaseAngleOfIncidence * angleMultiplier;
		weaponAttack.Profile.DamageExpression.Formula.Parameters["degree"] = finalDegree;
		weaponAttack.Profile.DamageExpression.Formula.Parameters["quality"] =
			(int)wardResult.WardWeapon.Parent.Quality;
		weaponAttack.Profile.StunExpression.Formula.Parameters["degree"] = finalDegree;
		weaponAttack.Profile.StunExpression.Formula.Parameters["quality"] =
			(int)wardResult.WardWeapon.Parent.Quality;
		weaponAttack.Profile.PainExpression.Formula.Parameters["degree"] = finalDegree;
		weaponAttack.Profile.PainExpression.Formula.Parameters["quality"] =
			(int)wardResult.WardWeapon.Parent.Quality;

		var finalDamage = new Damage
		{
			ActorOrigin = defender,
			LodgableItem = null,
			ToolOrigin = wardResult.WardWeapon.Parent,
			AngleOfIncidentRadians = finalAngle,
			Bodypart = targetBodypart,
			DamageAmount =
				weaponAttack.Profile.DamageExpression.Evaluate(defender,
					context: TraitBonusContext.ArmedDamageCalculation) * 2 * finalAngle / Math.PI,
			DamageType = weaponAttack.Profile.DamageType,
			PainAmount =
				weaponAttack.Profile.PainExpression.Evaluate(defender,
					context: TraitBonusContext.ArmedDamageCalculation) * 2 * finalAngle / Math.PI,
			PenetrationOutcome =
				Gameworld.GetCheck(CheckType.MeleeWeaponPenetrateCheck)
				         .Check(defender, GetPenetrationDifficulty(weaponAttack.Profile.DamageType),
					         wardResult.WardWeapon.WeaponType.AttackTrait, aggressor),
			ShockAmount = 0,
			StunAmount =
				weaponAttack.Profile.StunExpression.Evaluate(defender,
					context: TraitBonusContext.ArmedDamageCalculation) * 2 * finalAngle / Math.PI
		};

		var wounds = aggressor.PassiveSufferDamage(finalDamage).ToList();
		defender.Body?.SetExertion(weaponAttack.ExertionLevel);
		return wounds;
	}

	//Execute a natural weapon based ward counter attack
	//Sticking to the conventions in ProcessWardFreeAttack(), aggressor is the one that attacked the
	//warder and defender is the warder taking their free counter attack. I.e., defender is attacking
	//aggressor in the following code.
	protected IEnumerable<IWound> AttemptWardFreeNaturalAttack(ICharacter aggressor, ICharacter defender,
		WardResult wardResult, INaturalAttack naturalAttack)
	{
		var targetBodypart = aggressor.Body.RandomBodyPartGeometry(naturalAttack.Attack.Orientation,
			naturalAttack.Attack.Alignment, Facing.Front);
		var attackRoll = Gameworld.GetCheck(CheckType.NaturalWeaponAttack)
		                          .Check(defender, naturalAttack.Attack.Profile.BaseAttackerDifficulty, aggressor);
		var defenseRoll = Gameworld.GetCheck(CheckType.DodgeCheck)
		                           .Check(aggressor, naturalAttack.Attack.Profile.BaseDodgeDifficulty, defender);
		var result = new OpposedOutcome(attackRoll, defenseRoll);

		var attackEmote = string.Format(Gameworld.CombatMessageManager
		                                         .GetMessageFor(defender, aggressor,
			                                         null, naturalAttack.Attack,
			                                         BuiltInCombatMoveType.WardFreeUnarmedAttack,
			                                         attackRoll, naturalAttack.Bodypart),
			naturalAttack.Bodypart.FullDescription(),
			targetBodypart.FullDescription()).Replace("@hand",
			naturalAttack.Bodypart.Alignment.LeftRightOnly().Describe().ToLowerInvariant());

		if (result.Outcome == OpposedOutcomeDirection.Opponent)
		{
			//Counter attack got dodged
			var dodgeEmote = Gameworld.CombatMessageManager.GetMessageFor(
				aggressor, defender, null, naturalAttack.Attack, BuiltInCombatMoveType.Dodge,
				defenseRoll, naturalAttack.Bodypart);

			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{string.Format(dodgeEmote, naturalAttack.Bodypart.FullDescription(), targetBodypart.FullDescription())}"
							.Fullstop(),
						defender, defender, aggressor, null, null, null),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			return Enumerable.Empty<IWound>();
		}
		else
		{
			var dodgeEmote = Gameworld.CombatMessageManager.GetFailMessageFor(
				aggressor, defender, null, naturalAttack.Attack, BuiltInCombatMoveType.Dodge, defenseRoll,
				naturalAttack.Bodypart);

			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{string.Format(dodgeEmote, naturalAttack.Bodypart.FullDescription(), targetBodypart.FullDescription())}"
							.Fullstop(),
						defender, defender, aggressor, null, null, null),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

#if DEBUG
			Console.WriteLine(
				$"WeaponAttackMove ProcessWardFreeAttack Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif

			var finalDegree = result.Outcome == OpposedOutcomeDirection.Opponent ? 0 : (int)result.Degree;
			var angleMultiplier = 1.0;
			switch (result.Degree)
			{
				case OpposedOutcomeDegree.None:
					angleMultiplier = 0.5;
					break;
				case OpposedOutcomeDegree.Marginal:
					angleMultiplier = 0.6;
					break;
				case OpposedOutcomeDegree.Minor:
					angleMultiplier = 0.7;
					break;
				case OpposedOutcomeDegree.Moderate:
					angleMultiplier = 0.8;
					break;
				case OpposedOutcomeDegree.Major:
					angleMultiplier = 0.9;
					break;
				case OpposedOutcomeDegree.Total:
					angleMultiplier = 1.0;
					break;
			}

			var finalAngle = naturalAttack.Attack.Profile.BaseAngleOfIncidence * angleMultiplier;
			naturalAttack.Attack.Profile.DamageExpression.Formula.Parameters["degree"] = finalDegree;
			naturalAttack.Attack.Profile.DamageExpression.Formula.Parameters["quality"] =
				(int)defender.NaturalWeaponQuality(naturalAttack);
			naturalAttack.Attack.Profile.StunExpression.Formula.Parameters["degree"] = finalDegree;
			naturalAttack.Attack.Profile.StunExpression.Formula.Parameters["quality"] =
				(int)defender.NaturalWeaponQuality(naturalAttack);
			naturalAttack.Attack.Profile.PainExpression.Formula.Parameters["degree"] = finalDegree;
			naturalAttack.Attack.Profile.PainExpression.Formula.Parameters["quality"] =
				(int)defender.NaturalWeaponQuality(naturalAttack);

			var relativeHardness = CalculateRelativeHardness(defender, aggressor, naturalAttack.Bodypart,
				targetBodypart,
				null, attackRoll, naturalAttack.Attack.Profile.DamageType);

			var finalDamage = new Damage
			{
				ActorOrigin = defender,
				LodgableItem = null,
				ToolOrigin = null,
				AngleOfIncidentRadians = finalAngle,
				Bodypart = targetBodypart,
				DamageAmount =
					naturalAttack.Attack.Profile.DamageExpression.Evaluate(defender,
						context: TraitBonusContext.UnarmedDamageCalculation) * 2 * finalAngle / Math.PI *
					relativeHardness,
				DamageType = naturalAttack.Attack.Profile.DamageType,
				PainAmount =
					naturalAttack.Attack.Profile.PainExpression.Evaluate(defender,
						context: TraitBonusContext.UnarmedDamageCalculation) * 2 * finalAngle / Math.PI *
					relativeHardness,
				PenetrationOutcome = Gameworld.GetCheck(CheckType.MeleeWeaponPenetrateCheck)
				                              .Check(defender,
					                              GetPenetrationDifficulty(naturalAttack.Attack.Profile.DamageType),
					                              aggressor),
				ShockAmount = 0,
				StunAmount =
					naturalAttack.Attack.Profile.StunExpression.Evaluate(defender,
						context: TraitBonusContext.UnarmedDamageCalculation) * 2 * finalAngle / Math.PI *
					relativeHardness
			};

			var selfDamage = new Damage
			{
				ActorOrigin = defender,
				LodgableItem = null,
				ToolOrigin = null,
				AngleOfIncidentRadians = finalAngle,
				Bodypart = naturalAttack.Bodypart,
				DamageAmount =
					naturalAttack.Attack.Profile.DamageExpression.Evaluate(defender,
						context: TraitBonusContext.UnarmedDamageCalculation) * 2 * finalAngle / Math.PI *
					(1.0 - relativeHardness),
				DamageType = DamageType.Crushing,
				PainAmount =
					naturalAttack.Attack.Profile.PainExpression.Evaluate(defender,
						context: TraitBonusContext.UnarmedDamageCalculation) * 2 * finalAngle / Math.PI *
					(1.0 - relativeHardness),
				PenetrationOutcome = new CheckOutcome { Outcome = Outcome.MajorFail },
				ShockAmount = 0,
				StunAmount =
					naturalAttack.Attack.Profile.StunExpression.Evaluate(defender,
						context: TraitBonusContext.UnarmedDamageCalculation) * 2 * finalAngle / Math.PI *
					(1.0 - relativeHardness)
			};

			//Apply self-inflicted natural weapon wounds here, since this whole exchange is happening instantly within
			//an entirely unrelated CombatMove and won't be applied elsewhere
			defender.PassiveSufferDamage(selfDamage);

			var wounds = aggressor.PassiveSufferDamage(finalDamage).ToList();
			defender.Body?.SetExertion(naturalAttack.Attack.ExertionLevel);
			return wounds;
		}
	}

	//For this function, the aggressor is the person who attacked the warding individual,
	//defender is the one who was warding who got a chance to counter attack. So the attack is
	//coming from the defender toward the aggressor
	protected IEnumerable<IWound> ProcessWardFreeAttack(ICharacter aggressor, ICharacter defender,
		WardResult wardResult)
	{
		if (wardResult == null || wardResult.WardAttack == false)
		{
			return Enumerable.Empty<IWound>();
		}

		IWeaponAttack weaponAttack = null;
		INaturalAttack naturalAttack = null;

		SelectWardFreeAttack(defender, aggressor, wardResult, ref weaponAttack, ref naturalAttack);

		if (weaponAttack != null)
		{
			return AttemptWardFreeWeaponAttack(aggressor, defender, wardResult, weaponAttack);
		}


		if (naturalAttack != null)
		{
			return AttemptWardFreeNaturalAttack(aggressor, defender, wardResult, naturalAttack);
		}


		return Enumerable.Empty<IWound>();
	}

	protected WardResult ResolveWard(WardDefenseMove ward)
	{
		var warderDifficulty = ward.GetWarderDifficulty(this);
		var wardCheck = Gameworld.GetCheck(CheckType.Ward)
		                         .CheckAgainstAllDifficulties(ward.Assailant, warderDifficulty, null, Assailant,
			                         ward.Assailant.DefensiveAdvantage -
			                         GetPositionPenalty(Assailant.GetFacingFor(ward.Assailant)));
		ward.Assailant.DefensiveAdvantage = 0;
		var wardEmote = Gameworld.CombatMessageManager.GetMessageFor(ward.Assailant, Assailant,
			ward.WardWeapon?.Parent, Attack, BuiltInCombatMoveType.WardDefense, wardCheck[warderDifficulty], null);
		var wardeeDifficulty = ward.GetWardeeDifficulty(this);
		var wardOpponentCheck = Gameworld.GetCheck(CheckType.WardDefense)
		                                 .CheckAgainstAllDifficulties(Assailant, wardeeDifficulty, null,
			                                 ward.Assailant);
		var result = new OpposedOutcome(wardCheck, wardOpponentCheck, warderDifficulty, wardeeDifficulty);
		string wardDefenseEmote;
		if (result.Outcome == OpposedOutcomeDirection.Proponent || result.Outcome == OpposedOutcomeDirection.Stalemate)
		{
			var wardIgnoreCheck = Gameworld.GetCheck(CheckType.WardIgnore)
			                               .Check(Assailant, Difficulty.Normal, ward.Assailant);
			if (Assailant.CombatSettings.ForbiddenIntentions.HasFlag(CombatMoveIntentions.Aggressive) ||
			    Assailant.CombatSettings.ForbiddenIntentions.HasFlag(CombatMoveIntentions.Risky) ||
			    wardIgnoreCheck.IsFail())
			{
				wardDefenseEmote = Gameworld.CombatMessageManager.GetFailMessageFor(ward.Assailant, Assailant,
					ward.WardWeapon?.Parent, Attack, BuiltInCombatMoveType.WardCounter, wardCheck[warderDifficulty],
					null);
				return new WardResult
				{
					WardAttack = false,
					WardEmotes = $"{wardEmote}{wardDefenseEmote}",
					WardSucceeded = true,
					WardWeapon = ward.WardWeapon
				};
			}

			// If we get this far, they are ignoring the ward and the attacker gets a counter attack
			wardDefenseEmote = Gameworld.CombatMessageManager.GetMessageFor(ward.Assailant, Assailant,
				ward.WardWeapon?.Parent, Attack, BuiltInCombatMoveType.WardCounter, wardCheck[warderDifficulty], null);
			return new WardResult
			{
				WardAttack = true,
				WardEmotes = $"{wardEmote}{wardDefenseEmote}",
				WardSucceeded = false,
				WardWeapon = ward.WardWeapon
			};
		}

		wardDefenseEmote = Gameworld.CombatMessageManager.GetMessageFor(ward.Assailant, Assailant,
			ward.WardWeapon?.Parent, Attack, BuiltInCombatMoveType.WardCounter, wardCheck[warderDifficulty], null);
		return new WardResult
		{
			WardAttack = false,
			WardEmotes = $"{wardEmote}{wardDefenseEmote}",
			WardSucceeded = false,
			WardWeapon = ward.WardWeapon
		};
	}

	protected class WardResult
	{
		public bool WardSucceeded { get; init; }
		public IMeleeWeapon WardWeapon { get; init; }
		public string WardEmotes { get; init; }
		public bool WardAttack { get; init; }
	}

	#region Implementation of IWeaponAttackMove

	public IWeaponAttack Attack { get; init; }
	public IBodypart TargetBodypart { get; set; }
	public abstract int Reach { get; }

	public abstract BuiltInCombatMoveType MoveType { get; }

	#endregion

	protected Difficulty GetPenetrationDifficulty(DamageType type)
	{
		switch (type)
		{
			case DamageType.Slashing:
				return Difficulty.Hard;
			case DamageType.Chopping:
				return Difficulty.Normal;
			case DamageType.Crushing:
				return Difficulty.ExtremelyHard;
			case DamageType.Piercing:
				return Difficulty.Easy;
			case DamageType.Ballistic:
				return Difficulty.Hard;
			case DamageType.BallisticArmourPiercing:
			case DamageType.ArmourPiercing:
				return Difficulty.VeryEasy;
			case DamageType.Burning:
				return Difficulty.ExtremelyHard;
			case DamageType.Freezing:
				return Difficulty.ExtremelyHard;
			case DamageType.Chemical:
				return Difficulty.ExtremelyHard;
			case DamageType.Shockwave:
				return Difficulty.ExtremelyHard;
			case DamageType.Bite:
				return Difficulty.Easy;
			case DamageType.Eldritch:
			case DamageType.Arcane:
			case DamageType.Claw:
				return Difficulty.Normal;
			case DamageType.Electrical:
				return Difficulty.ExtremelyHard;
			case DamageType.Hypoxia:
				return Difficulty.Insane;
			case DamageType.Cellular:
				return Difficulty.Insane;
			case DamageType.Shearing:
				return Difficulty.ExtremelyHard;
			case DamageType.Sonic:
				return Difficulty.Insane;
			case DamageType.Wrenching:
				return Difficulty.Impossible;
			case DamageType.Shrapnel:
				return Difficulty.ExtremelyHard;
			case DamageType.Necrotic:
				return Difficulty.Impossible;
			case DamageType.Falling:
				return Difficulty.Impossible;
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}

	public virtual void ResolveBloodSpray(CombatMoveResult result)
	{
		//check the result and add blood to the weapon if appropriate. 
		if (Weapon?.Parent == null)
		{
			return;
		}

		foreach (var curWound in result.WoundsCaused)
		{
			if (curWound.BleedStatus == BleedStatus.Bleeding || (curWound.Severity >= WoundSeverity.VerySevere &&
			                                                     curWound.DamageType == DamageType.Crushing))
			{
				var bloodSource = (curWound.Parent as ICharacter)?.Body;

				if (bloodSource == null)
				{
					return;
				}

				double bloodVolume = 0;

				switch (curWound.Severity)
				{
					case WoundSeverity.Moderate:
						bloodVolume = 0.010;
						break;
					case WoundSeverity.Severe:
						bloodVolume = 0.020;
						break;
					case WoundSeverity.VerySevere:
						bloodVolume = 0.035;
						break;
					case WoundSeverity.Grievous:
						bloodVolume = 0.05;
						break;
					case WoundSeverity.Horrifying:
						bloodVolume = 0.1;
						break;
					default:
						break;
				}

				bloodVolume *= BloodSprayMultiplier;
				if (bloodVolume > 0.0)
				{
					Weapon.Parent.ExposeToLiquid(
						new LiquidMixture(new BloodLiquidInstance(bloodSource.Actor, bloodVolume), Gameworld), null,
						LiquidExposureDirection.Irrelevant);
				}
			}
		}
	}

	protected void DetermineTargetBodypart(ICombatMove move, Outcome outcome)
	{
		var target = move.Assailant;
		if (target.Body == null)
		{
			return;
		}

		if (Assailant.TargettedBodypart is not null && !target.Body.Bodyparts.Contains(Assailant.TargettedBodypart))
		{
			Assailant.TargettedBodypart = null;
		}

		var defending = !(move is TooExhaustedMove) &&
		                !(move is HelplessDefenseMove) &&
		                !(this is ClinchAttackMove) &&
		                !(this is ClinchNaturalAttackMove) &&
		                !target.IsHelpless &&
		                target.Race.CombatSettings.CanDefend;

		if (Assailant.TargettedBodypart == null)
		{
			var facing = Assailant.GetFacingFor(target, true);
			if (!Assailant.Combat.Friendly && target.IsHelpless)
			{
				TargetBodypart = target.Body.RandomVitalBodypart(facing);
			}
			else
			{
				TargetBodypart =
					target.Body.RandomBodyPartGeometry(Attack.Orientation, Attack.Alignment, facing, defending);
			}
#if DEBUG
			Console.WriteLine(
				$"Target Bodypart = {TargetBodypart?.Name ?? "none"} - {facing.Describe()} {Attack.Orientation.Describe()} {Attack.Alignment.Describe()}");
#endif
			return;
		}

		if (!defending && outcome != Outcome.MajorFail)
		{
			TargetBodypart = Assailant.TargettedBodypart;
			return;
		}

		switch (outcome)
		{
			case Outcome.MinorPass:
				TargetBodypart =
					target.Body.RandomBodyPartGeometry(
						Assailant.TargettedBodypart.Orientation.RaiseUp(Dice.Roll(1, 2) == 1 ? 1 : -1),
						Assailant.TargettedBodypart.Alignment, Assailant.GetFacingFor(target, true), false);
				return;
			case Outcome.Pass:
				TargetBodypart =
					target.Body.RandomBodyPartGeometry(Assailant.TargettedBodypart.Orientation,
						Assailant.TargettedBodypart.Alignment, Assailant.GetFacingFor(target, true), false);
				return;
			case Outcome.MajorPass:
				TargetBodypart = Assailant.TargettedBodypart;
				return;
			default:
				if (!Assailant.Combat.Friendly && !target.IsHelpless)
				{
					TargetBodypart = target.Body.RandomVitalBodypart(Assailant.GetFacingFor(target, true));
				}
				else
				{
					TargetBodypart = target.Body.RandomBodyPartGeometry(Attack.Orientation, Attack.Alignment,
						Assailant.GetFacingFor(target, true), defending);
				}

				return;
		}
	}
}