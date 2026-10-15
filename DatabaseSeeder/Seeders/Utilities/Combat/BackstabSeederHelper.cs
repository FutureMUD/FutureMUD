#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

internal sealed record BackstabSeedConfiguration(
	double Reel = 0.0,
	double AttackerOffence = 0.0,
	double AttackerDefence = 0.0,
	double TargetOffence = 0.0,
	double TargetDefence = 0.0,
	double Penetration = 0.0,
	double ArmourBypass = 0.0)
{
	internal string ToXml() => new XElement("Backstab",
		new XElement("Resist", Difficulty.Normal), new XElement("Reel", Reel),
		new XElement("AttackerOffence", AttackerOffence), new XElement("AttackerDefence", AttackerDefence),
		new XElement("TargetOffence", TargetOffence), new XElement("TargetDefence", TargetDefence),
		new XElement("Penetration", Penetration), new XElement("ArmourBypass", ArmourBypass)).ToString();

	internal CombatMoveIntentions Intentions => CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
		CombatMoveIntentions.Kill |
		(AttackerOffence + AttackerDefence > 0 ? CombatMoveIntentions.Advantage : CombatMoveIntentions.None) |
		(TargetOffence + TargetDefence > 0 ? CombatMoveIntentions.Disadvantage : CombatMoveIntentions.None) |
		(Reel > 0 ? CombatMoveIntentions.Stun : CombatMoveIntentions.None);
}

internal static class BackstabSeederHelper
{
	internal const string PredatoryBackRake = "Predatory Back Rake";
	internal const string ManticoreBackstab = "Manticore Raking Backstab";
	internal const string WerewolfBackstab = "Werewolf Savaging Backstab";
	internal static readonly BackstabSeedConfiguration PredatorConfiguration = new(AttackerOffence: 5, TargetDefence: 5);
	internal static readonly BackstabSeedConfiguration ManticoreConfiguration = new(Reel: 0.5, TargetOffence: 5);
	internal static readonly BackstabSeedConfiguration WerewolfConfiguration = new(TargetOffence: 5, TargetDefence: 5);

	internal static TraitExpression ScaleExpression(FuturemudDatabaseContext context, string name,
		TraitExpression source, double multiplier, IEnumerable<string>? stockSourceExpressions = null)
	{
		string Scale(string formula) => $"{multiplier.ToString(System.Globalization.CultureInfo.InvariantCulture)} * ({formula})";
		var expression = context.TraitExpressions.FirstOrDefault(x => x.Name == name);
		if (expression is not null)
		{
			// Only replace an exact known stock formula. Builder-authored formulas keep their value.
			if ((stockSourceExpressions ?? [source.Expression]).Any(x => expression.Expression == Scale(x)))
				expression.Expression = Scale(source.Expression);
			return expression;
		}
		expression = new TraitExpression
		{
			Name = name,
			Expression = Scale(source.Expression)
		};
		context.TraitExpressions.Add(expression);
		context.SaveChanges();
		return expression;
	}

	internal static void EnsureMessage(FuturemudDatabaseContext context, WeaponAttack attack, string message)
	{
		if (context.CombatMessages.Any(x => x.Type == attack.MoveType &&
		                                  x.CombatMessagesWeaponAttacks.Any(y => y.WeaponAttackId == attack.Id))) return;
		var row = new CombatMessage
		{
			Type = attack.MoveType, Verb = attack.Verb, Message = message, FailureMessage = message,
			Priority = 50, Chance = 1.0
		};
		row.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks { CombatMessage = row, WeaponAttack = attack });
		context.CombatMessages.Add(row);
		context.SaveChanges();
	}

	internal static WeaponAttack EnsureNaturalAttack(FuturemudDatabaseContext context, string name,
		WeaponAttack donor, BackstabSeedConfiguration configuration, double multiplier,
		double stamina, double delay, double weighting, string message,
		TraitExpression? damageSource = null, TraitExpression? painSource = null, TraitExpression? stunSource = null)
	{
		var attack = context.WeaponAttacks.FirstOrDefault(x => x.Name == name && x.WeaponTypeId == null);
		TraitExpression Expression(long id) => context.TraitExpressions.Find(id)!;
		// Animal stock profiles differ only by these explicitly authored randomness suffixes.
		IEnumerable<string> StockSources(TraitExpression source)
		{
			if (source.Name != "Animal Claw Damage") return [source.Expression];
			var strength = context.TraitDefinitions.Where(x => x.Type == 1).AsEnumerable()
				.FirstOrDefault(x => x.Name.In("Strength", "Physique", "Body", "Upper Body Strength"));
			if (strength is null) return [source.Expression];
			var basis = $"1.0 * (str:{strength.Id} + (3 * quality)) * sqrt(degree+1)";
			string[] variants = [basis, basis + " * rand(0.7,1.0)", basis + " * rand(0.2,1.0)"];
			return variants.Contains(source.Expression) ? variants : [source.Expression];
		}
		var damage = damageSource ?? Expression(donor.DamageExpressionId);
		var pain = painSource ?? Expression(donor.PainExpressionId);
		var stun = stunSource ?? Expression(donor.StunExpressionId);
		var scaledDamage = ScaleExpression(context, $"{name} Damage", damage, multiplier, StockSources(damage));
		var scaledPain = ScaleExpression(context, $"{name} Pain", pain, multiplier, StockSources(pain));
		var scaledStun = ScaleExpression(context, $"{name} Stun", stun, multiplier, StockSources(stun));
		if (attack is null)
		{
			attack = new WeaponAttack
			{
				Name = name, MoveType = (int)BuiltInCombatMoveType.BackstabAttackUnarmed,
				Verb = donor.Verb, DamageType = donor.DamageType,
				BaseAttackerDifficulty = donor.BaseAttackerDifficulty, BaseDodgeDifficulty = donor.BaseDodgeDifficulty,
				BaseParryDifficulty = donor.BaseParryDifficulty, BaseBlockDifficulty = donor.BaseBlockDifficulty,
				BaseAngleOfIncidence = donor.BaseAngleOfIncidence,
				RecoveryDifficultySuccess = donor.RecoveryDifficultySuccess, RecoveryDifficultyFailure = donor.RecoveryDifficultyFailure,
				BodypartShapeId = donor.BodypartShapeId, Orientation = donor.Orientation, Alignment = donor.Alignment,
				RequiredPositionStateIds = donor.RequiredPositionStateIds, HandednessOptions = donor.HandednessOptions,
				ExertionLevel = donor.ExertionLevel, FutureProgId = donor.FutureProgId, OnUseProgId = donor.OnUseProgId,
				MaximumTargets = 1, StaminaCost = stamina, BaseDelay = delay, Weighting = weighting,
				Intentions = (long)configuration.Intentions, AdditionalInfo = configuration.ToXml(),
				DamageExpression = scaledDamage, PainExpression = scaledPain, StunExpression = scaledStun
			};
			context.WeaponAttacks.Add(attack);
			context.SaveChanges();
		}
		EnsureMessage(context, attack, message);
		context.SaveChanges();
		return attack;
	}

	internal static void EnsureAnimalBackstabs(FuturemudDatabaseContext context)
	{
		var links = context.RacesWeaponAttacks.Include(x => x.Race).Include(x => x.WeaponAttack)
			.Include(x => x.Bodypart)
			.Where(x => (x.Race.Name == "Leopard" || x.Race.Name == "Jaguar") && x.WeaponAttack.Name == "Claw Swipe")
			.ToList();
		if (links.Count == 0) return;
		var attack = EnsureNaturalAttack(context, PredatoryBackRake, links[0].WeaponAttack, PredatorConfiguration,
			1.5, 5, 1.3, 200, "@ twist|twists behind $1 and rake|rakes at &1's back with &0's {0}");
		foreach (var source in links)
		{
			if (context.RacesWeaponAttacks.Any(x => x.RaceId == source.RaceId && x.BodypartId == source.BodypartId &&
			                                      x.WeaponAttackId == attack.Id)) continue;
			context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
			{
				Race = source.Race, Bodypart = source.Bodypart, WeaponAttack = attack, Quality = source.Quality
			});
		}
		context.SaveChanges();
	}
}
