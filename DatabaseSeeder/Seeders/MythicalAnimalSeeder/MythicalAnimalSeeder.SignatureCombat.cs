#nullable enable

using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class MythicalAnimalSeeder
{
	private static readonly string[] MythicalSignatureAttackNames =
	[
		"Western Dragonfire Breath",
		"Eastern Dragonfire Breath"
	];

	private void EnsureMythicalSignatureAttacks()
	{
		WeaponAttack donor = _context.WeaponAttacks.First(x => x.Name == "Dragonfire Breath");
		BodypartShape mouth = _context.BodypartShapes.First(x => x.Name == "Mouth");
		long waterTagId = _context.Tags.First(x => x.Name == "Water").Id;
		TraitExpression westernDamage = EnsureMythicalExpression(
			"Western Dragonfire Breath Damage",
			$"1.2 * (aura:{_auraTrait.Id} + (4 * quality)) * sqrt(degree+1)");
		TraitExpression easternDamage = EnsureMythicalExpression(
			"Eastern Dragonfire Breath Damage",
			$"0.95 * (aura:{_auraTrait.Id} + (4 * quality)) * sqrt(degree+1)");

		UpsertDragonBreath(
			"Western Dragonfire Breath",
			donor,
			westernDamage,
			mouth,
			5.5,
			28.0,
			BuildDragonBreathData(2, 0.45, "Western Dragonfire", 0.55, 0.4, 0.12, 3.0, 0.22, waterTagId),
			"@ rear|rears up and unleash|unleashes a roaring cone of furnace-hot dragonfire at $1.");
		UpsertDragonBreath(
			"Eastern Dragonfire Breath",
			donor,
			easternDamage,
			mouth,
			4.8,
			32.0,
			BuildDragonBreathData(3, 0.3, "Eastern Dragonfire", 0.38, 0.45, 0.2, 2.4, 0.16, waterTagId),
			"@ coil|coils through the air and breathe|breathes a long, searing river of dragonfire toward $1.");
		_context.SaveChanges();
	}

	private TraitExpression EnsureMythicalExpression(string name, string expression)
	{
		TraitExpression? existing = _context.TraitExpressions.FirstOrDefault(x => x.Name == name);
		if (existing is not null)
		{
			existing.Expression = expression;
			return existing;
		}

		TraitExpression created = new() { Name = name, Expression = expression };
		_context.TraitExpressions.Add(created);
		_context.SaveChanges();
		return created;
	}

	private void UpsertDragonBreath(
		string name,
		WeaponAttack donor,
		TraitExpression expression,
		BodypartShape mouth,
		double delay,
		double weighting,
		string additionalInfo,
		string message)
	{
		WeaponAttack? attack = _context.WeaponAttacks.FirstOrDefault(x => x.Name == name);
		attack ??= new WeaponAttack();
		bool created = attack.Id == 0;
		attack.Name = name;
		attack.WeaponTypeId = donor.WeaponTypeId;
		attack.Verb = (int)MeleeWeaponVerb.Blast;
		attack.FutureProgId = donor.FutureProgId;
		attack.BaseAttackerDifficulty = (int)Difficulty.Hard;
		attack.BaseBlockDifficulty = (int)Difficulty.Hard;
		attack.BaseDodgeDifficulty = (int)Difficulty.Hard;
		attack.BaseParryDifficulty = (int)Difficulty.Hard;
		attack.BaseAngleOfIncidence = donor.BaseAngleOfIncidence;
		attack.RecoveryDifficultySuccess = donor.RecoveryDifficultySuccess;
		attack.RecoveryDifficultyFailure = donor.RecoveryDifficultyFailure;
		attack.MoveType = (int)BuiltInCombatMoveType.BreathWeaponAttack;
		attack.Intentions = (long)(CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
			CombatMoveIntentions.Burning | CombatMoveIntentions.Hard | CombatMoveIntentions.Slow);
		attack.ExertionLevel = donor.ExertionLevel;
		attack.DamageType = (int)DamageType.Burning;
		attack.DamageExpressionId = expression.Id;
		attack.PainExpressionId = expression.Id;
		attack.StunExpressionId = expression.Id;
		attack.Weighting = weighting;
		attack.MaximumTargets = 4;
		attack.BodypartShapeId = mouth.Id;
		attack.StaminaCost = 12.0;
		attack.BaseDelay = delay;
		attack.Orientation = (int)Orientation.High;
		attack.Alignment = (int)Alignment.Front;
		attack.AdditionalInfo = additionalInfo;
		attack.HandednessOptions = donor.HandednessOptions;
		attack.RequiredPositionStateIds = donor.RequiredPositionStateIds;
		attack.OnUseProgId = donor.OnUseProgId;
		if (created)
		{
			_context.WeaponAttacks.Add(attack);
		}
		_context.SaveChanges();

		CombatMessage? combatMessage = _context.CombatMessages.FirstOrDefault(x =>
			x.Priority == 50 && x.CombatMessagesWeaponAttacks.Any(y => y.WeaponAttackId == attack.Id));
		combatMessage ??= new CombatMessage();
		bool messageCreated = combatMessage.Id == 0;
		combatMessage.Type = attack.MoveType;
		combatMessage.Message = message;
		combatMessage.FailureMessage = message;
		combatMessage.Priority = 50;
		combatMessage.Verb = attack.Verb;
		combatMessage.Chance = 1.0;
		if (messageCreated)
		{
			combatMessage.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
			{
				CombatMessage = combatMessage,
				WeaponAttack = attack
			});
			_context.CombatMessages.Add(combatMessage);
		}
	}

	private static string BuildDragonBreathData(
		int range,
		double igniteChance,
		string fireName,
		double damagePerTick,
		double painPerTick,
		double stunPerTick,
		double thermalLoad,
		double spreadChance,
		long waterTagId)
	{
		return new XElement("Data",
			new XElement("RangeInRooms", range),
			new XElement("ScatterType", RangedScatterType.Light.ToString()),
			new XElement("AdditionalTargetLimit", 3),
			new XElement("BodypartsHitPerTarget", 2),
			new XElement("IgniteChance", igniteChance),
			new XElement("FireProfile",
				new XElement("Name", new XCData(fireName)),
				new XElement("DamageType", (int)DamageType.Burning),
				new XElement("DamagePerTick", damagePerTick),
				new XElement("PainPerTick", painPerTick),
				new XElement("StunPerTick", stunPerTick),
				new XElement("ThermalLoadPerTick", thermalLoad),
				new XElement("SpreadChance", spreadChance),
				new XElement("MinimumOxidation", 0.05),
				new XElement("SelfOxidising", true),
				new XElement("TickFrequencySeconds", 10),
				new XElement("ExtinguishTags", new XElement("Tag", waterTagId)))).ToString();
	}

	private void ReconcileMythicalNaturalAttackLinks()
	{
		HashSet<string> managedNames = Templates.Values
			.SelectMany(x => x.Attacks)
			.Select(x => x.AttackName)
			.Concat(MythicalSignatureAttackNames)
			.Append("Behemoth Charge")
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (MythicalRaceTemplate template in Templates.Values)
		{
			Race? race = _context.Races.FirstOrDefault(x => x.Name == template.Name);
			if (race is null)
			{
				continue;
			}

			List<SeededNaturalAttackLink> expected = [];
			foreach (MythicalAttackTemplate attackTemplate in template.Attacks)
			{
				string attackName = attackTemplate.AttackName == "Dragonfire Breath" &&
				                    template.CombatBalance.SignatureActionKey is not null
					? template.CombatBalance.SignatureActionKey
					: attackTemplate.AttackName;
				WeaponAttack? attack = _context.WeaponAttacks.FirstOrDefault(x => x.Name == attackName);
				if (attack is null)
				{
					continue;
				}

				var quality = (ItemQuality)Math.Max((int)attackTemplate.Quality,
					(int)template.CombatBalance.NaturalArmourQuality);
				expected.Add(new(attack, quality, attackTemplate.BodypartAliases));
			}

			if (template.CombatBalance.GrantBehemothCharge &&
			    _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Behemoth Charge") is { } charge)
			{
				expected.Add(new(charge, template.CombatBalance.NaturalArmourQuality));
			}

			NonHumanNaturalAttackReconciler.Reconcile(_context, race, expected, managedNames);
		}

		_context.SaveChanges();
	}
}
