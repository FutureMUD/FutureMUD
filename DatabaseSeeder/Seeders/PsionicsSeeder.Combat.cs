#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public sealed partial class PsionicsSeeder
{
	private static IEnumerable<(MagicPower Power, int Band)> InstallCombatPowers(FuturemudDatabaseContext context,
		MagicSchool school, TraitDefinition trait, MagicResource resource, long allowed, long error)
	{
		long Expression(string name, string formula)
		{
			var expression = context.TraitExpressions.SingleOrDefault(x => x.Name == name);
			if (expression is not null) return expression.Id;
			expression = new TraitExpression { Name = name, Expression = formula };
			context.TraitExpressions.Add(expression); context.SaveChanges(); return expression.Id;
		}
		foreach (var stock in PsionicStockContent.CombatPowers)
		{
			var name = $"{school.Name}: {stock.Name}";
			var model = stock.Defense.HasValue ? "magicdefense" : "magicattack";
			var power = context.MagicPowers.SingleOrDefault(x => x.Name == name);
			if (power is not null)
			{
				if (power.PowerModel != model || power.MagicSchoolId != school.Id) throw new InvalidOperationException($"Conflicting power identity: {name}.");
				yield return (power, stock.Band);
				continue;
			}
			long attackId = 0;
			if (!stock.Defense.HasValue)
			{
				var attack = context.WeaponAttacks.SingleOrDefault(x => x.Name == name);
				if (attack is not null && (attack.MoveType != (int)BuiltInCombatMoveType.MagicPowerAttack || attack.WeaponTypeId is not null))
					throw new InvalidOperationException($"Conflicting psychic attack identity: {name}.");
				if (attack is null)
				{
					attack = new WeaponAttack
					{
						Name = name, MoveType = (int)BuiltInCombatMoveType.MagicPowerAttack,
						DamageType = (int)MudSharp.Health.DamageType.Crushing, Verb = (int)MeleeWeaponVerb.Bash,
						BaseAttackerDifficulty = (int)Difficulty.Normal, BaseBlockDifficulty = (int)Difficulty.Normal,
						BaseDodgeDifficulty = (int)Difficulty.Normal, BaseParryDifficulty = (int)Difficulty.Normal,
						RecoveryDifficultySuccess = (int)Difficulty.Normal, RecoveryDifficultyFailure = (int)Difficulty.Hard,
						DamageExpressionId = Expression("Psionic Combat Damage", PsionicStockContent.CombatDamageExpression),
						PainExpressionId = Expression("Psionic Combat Pain", PsionicStockContent.CombatPainExpression),
						StunExpressionId = Expression("Psionic Combat Stun", PsionicStockContent.CombatStunExpression),
						BaseAngleOfIncidence = Math.PI * 0.5, BaseDelay = PsionicStockContent.CombatAttackDelay,
						StaminaCost = PsionicStockContent.CombatAttackStamina, Weighting = 100, MaximumTargets = 1,
						ExertionLevel = (int)ExertionLevel.Heavy, Alignment = (int)Alignment.Front, Orientation = (int)Orientation.Centre,
						HandednessOptions = (int)AttackHandednessOptions.Any, RequiredPositionStateIds = "1",
						Intentions = (long)CombatMoveIntentions.Attack, AdditionalInfo = "<Definition />"
					};
					context.WeaponAttacks.Add(attack); context.SaveChanges();
				}
				attackId = attack.Id;
			}
			var help = stock.Defense.HasValue
				? $"Use {stock.Verb} to maintain {stock.Name}; end{stock.Verb} releases it. combat powerdefense lists remaining protection; combat powerdefense off releases all defenses."
				: $"Use {stock.Verb} <visible target> to queue {stock.Name}. Automatic use follows the psychic combat channel.";
			power = new MagicPower { Name = name, MagicSchoolId = school.Id, PowerModel = model, Blurb = help, ShowHelp = help,
				Definition = PsionicStockContent.CombatDefinition(stock, trait.Id, resource.Id, allowed, error, attackId).ToString() };
			context.MagicPowers.Add(power); context.SaveChanges();
			yield return (power, stock.Band);
		}
	}
}
