#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public partial class CombatSeeder
{
	internal static int EnsureBackstabAttacks(FuturemudDatabaseContext context, WeaponType blackjack,
		TraitExpression normal, TraitExpression good, SeedCombatMessageStyle messageStyle,
		IReadOnlyCollection<string>? normalFormulaVariants = null)
	{
		var head = context.BodypartShapes.FirstOrDefault(x => x.Name == "Head Back") ??
			throw new InvalidOperationException("Backstab stock requires the native Head Back bodypart shape. Install the supported human anatomy before combat content.");
		var count = 0;
		var lowDamage = BackstabSeederHelper.ScaleExpression(context, "Blackjack Damage", normal, 0.25, normalFormulaVariants);
		var pain = BackstabSeederHelper.ScaleExpression(context, "Blackjack Pain", normal, 0.5, normalFormulaVariants);
		var highStun = BackstabSeederHelper.ScaleExpression(context, "Blackjack Backstab Stun", normal, 2, normalFormulaVariants);
		const CombatMoveIntentions subdual = CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
		                                     CombatMoveIntentions.Stun;

		void Ensure(string name, WeaponType type, BuiltInCombatMoveType move, MeleeWeaponVerb verb,
			TraitExpression damage, TraitExpression painExpression, TraitExpression stun, double stamina, double delay,
			double weight, string message, BackstabSeedConfiguration? configuration = null, bool headOnly = false)
		{
			var attack = context.WeaponAttacks.FirstOrDefault(x => x.WeaponTypeId == type.Id && x.Name == name);
			if (attack is null)
			{
				attack = new WeaponAttack
				{
					Name = name, WeaponType = type, MoveType = (int)move, Verb = (int)verb,
					BaseAttackerDifficulty = (int)Difficulty.Normal, BaseDodgeDifficulty = (int)Difficulty.Easy,
					BaseParryDifficulty = (int)Difficulty.Easy, BaseBlockDifficulty = (int)Difficulty.VeryEasy,
					RecoveryDifficultySuccess = (int)Difficulty.Easy, RecoveryDifficultyFailure = (int)Difficulty.Hard,
					DamageType = (int)(type == blackjack ? DamageType.Crushing : DamageType.Piercing),
					DamageExpression = damage, PainExpression = painExpression, StunExpression = stun,
					StaminaCost = stamina, BaseDelay = delay, Weighting = weight, MaximumTargets = 1,
					Intentions = (long)(type == blackjack
						? subdual | (configuration is null ? CombatMoveIntentions.None : CombatMoveIntentions.Disadvantage)
						: configuration!.Intentions),
					ExertionLevel = (int)ExertionLevel.Heavy, Alignment = (int)Alignment.Front,
					Orientation = (int)(headOnly ? Orientation.Highest : Orientation.High),
					BodypartShapeId = headOnly ? head.Id : null,
					HandednessOptions = (int)(type == blackjack ? AttackHandednessOptions.OneHandedOnly : AttackHandednessOptions.Any),
					RequiredPositionStateIds = "1 16 17 18", AdditionalInfo = configuration?.ToXml()
				};
				context.WeaponAttacks.Add(attack);
				context.SaveChanges();
				count++;
			}
			BackstabSeederHelper.EnsureMessage(context, attack,
				CombatSeederMessageStyleHelper.FormatAttackMessage(message, messageStyle));
		}

		Ensure("Knife Backstab", context.WeaponTypes.First(x => x.Name == "Knife"), BuiltInCombatMoveType.BackstabAttack,
			MeleeWeaponVerb.Stab, normal, normal, normal, 5, 1.1, 200,
			"@ drive|drives $2 at $1 from behind", new(Reel: 0.25, TargetOffence: 5));
		Ensure("Dagger Backstab", context.WeaponTypes.First(x => x.Name == "Dagger"), BuiltInCombatMoveType.BackstabAttack,
			MeleeWeaponVerb.Stab, good, good, good, 4, 1, 200,
			"@ slip|slips $2 toward a gap in $1's defences from behind", new(Penetration: 10, ArmourBypass: 0.1));
		Ensure("Blackjack Bash", blackjack, BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Bash,
			lowDamage, pain, normal, 3, 0.8, 100, "@ swing|swings $2 at $1");
		Ensure("Blackjack Close Bash", blackjack, BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
			lowDamage, pain, normal, 3, 0.6, 100, "@ snap|snaps $2 at $1 in a short, close blow");
		Ensure("Blackjack Sapping Blow", blackjack, BuiltInCombatMoveType.BackstabAttack, MeleeWeaponVerb.Bash,
			lowDamage, pain, highStun, 4, 1.1, 300, "@ bring|brings $2 around toward the back of $1's head",
			new(Reel: 0.75, TargetOffence: 5), headOnly: true);
		context.SaveChanges();
		return count;
	}
}
