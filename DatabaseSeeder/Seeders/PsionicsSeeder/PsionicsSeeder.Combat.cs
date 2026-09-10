#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
			var legacyHelp = stock.Defense.HasValue
				? $"Use {stock.Verb} to maintain {stock.Name}; end{stock.Verb} releases it. combat powerdefense lists remaining protection; combat powerdefense off releases all defenses."
				: $"Use {stock.Verb} <visible target> to queue {stock.Name}. Automatic use follows the psychic combat channel.";
			var help = stock.Defense.HasValue
				? $"Syntax: {school.SchoolVerb} {stock.Verb}\n{school.SchoolVerb} end{stock.Verb}\nMaintain {stock.Name} until cancelled or depleted. Use combat powerdefense to inspect remaining protection and combat powerdefense off to release all defences."
				: $"Syntax: {school.SchoolVerb} {stock.Verb} <visible target>\nQueue {stock.Name} against a target within the power's range. Automatic use follows your psychic combat settings.";
			var power = FindPower(context, school, stock.Verb, stock.Name, name);
			if (power is not null)
			{
				if (power.PowerModel != model || power.MagicSchoolId != school.Id) throw new InvalidOperationException($"Conflicting power identity: {name}.");
				if (power.Name == name) power.Name = stock.Name;
				if (power.ShowHelp == legacyHelp) power.ShowHelp = help;
				if (power.Blurb == legacyHelp) power.Blurb = help;
				var definition = XElement.Parse(power.Definition);
				definition.SetElementValue("SeededIdentity", "psionics:" + stock.Verb);
				power.Definition = definition.ToString();
				context.SaveChanges();
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
			var newDefinition = PsionicStockContent.CombatDefinition(stock, trait.Id, resource.Id, allowed, error, attackId);
			newDefinition.SetElementValue("SeededIdentity", "psionics:" + stock.Verb);
			power = new MagicPower { Name = stock.Name, MagicSchoolId = school.Id, PowerModel = model, Blurb = help, ShowHelp = help,
				Definition = newDefinition.ToString() };
			context.MagicPowers.Add(power); context.SaveChanges();
			yield return (power, stock.Band);
		}
	}
}
