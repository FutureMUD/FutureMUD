#nullable enable

using MudSharp.Combat;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class HumanSeeder
{
	private CombatBalanceProfile _combatBalanceProfile = CombatBalanceProfile.Stock;

	private bool UsesCombatRebalance => _combatBalanceProfile == CombatBalanceProfile.CombatRebalance;

	private int ResolveHumanRelativeHitChance(string alias, int fallback)
	{
		if (!UsesCombatRebalance)
		{
			return GetHumanRelativeHitChance(alias, fallback);
		}

		return alias switch
		{
			"abdomen" or "rbreast" or "lbreast" or "uback" or "belly" or "lback" => 250,
			"rbuttock" or "lbuttock" => 70,
			"rshoulder" or "lshoulder" => 42,
			"rshoulderblade" or "lshoulderblade" => 34,
			"neck" or "bneck" => 20,
			"throat" => 12,
			"face" or "bhead" or "scalp" or "forehead" => 40,
			"chin" or "mouth" => 22,
			"rcheek" or "lcheek" => 18,
			"nose" => 10,
			"reyesocket" or "leyesocket" => 10,
			"reye" or "leye" => 7,
			"rear" or "lear" => 7,
			"rbrow" or "lbrow" => 6,
			"rtemple" or "ltemple" => 8,
			"rupperarm" or "lupperarm" or "rforearm" or "lforearm" => 55,
			"relbow" or "lelbow" or "rwrist" or "lwrist" => 16,
			"rhand" or "lhand" => 14,
			"rthumb" or "lthumb" or "rindexfinger" or "lindexfinger" or "rmiddlefinger" or "lmiddlefinger" or
				"rringfinger" or "lringfinger" or "rpinkyfinger" or "lpinkyfinger" => 3,
			"rhip" or "lhip" => 25,
			"rthigh" or "lthigh" or "rthighback" or "lthighback" => 75,
			"rknee" or "lknee" or "rkneeback" or "lkneeback" => 18,
			"rshin" or "lshin" or "rcalf" or "lcalf" => 40,
			"rankle" or "lankle" or "rheel" or "lheel" => 12,
			"rfoot" or "lfoot" => 12,
			"rbigtoe" or "lbigtoe" or "rindextoe" or "lindextoe" or "rmiddletoe" or "lmiddletoe" or "rringtoe" or
				"lringtoe" or "rpinkytoe" or "lpinkytoe" => 3,
			"groin" => 8,
			"testicles" or "penis" => 2,
			_ => GetHumanRelativeHitChance(alias, fallback)
		};
	}

	private int ResolveHumanMaxLife(string alias, int fallback)
	{
		if (!UsesCombatRebalance)
		{
			return fallback;
		}

		return alias switch
		{
			"abdomen" or "rbreast" or "lbreast" or "uback" or "belly" or "lback" => 40,
			"rbuttock" or "lbuttock" => 30,
			"rshoulder" or "lshoulder" or "rshoulderblade" or "lshoulderblade" => 30,
			"neck" or "bneck" => 25,
			"throat" => 20,
			"face" or "bhead" or "scalp" or "forehead" or "rcheek" or "lcheek" => 25,
			"chin" or "mouth" => 20,
			"nose" => 10,
			"reyesocket" or "leyesocket" or "reye" or "leye" => 10,
			"rear" or "lear" => 12,
			"rbrow" or "lbrow" or "rtemple" or "ltemple" => 10,
			"rupperarm" or "lupperarm" or "rforearm" or "lforearm" => 30,
			"relbow" or "lelbow" or "rwrist" or "lwrist" => 20,
			"rhand" or "lhand" => 20,
			"rthumb" or "lthumb" or "rindexfinger" or "lindexfinger" or "rmiddlefinger" or "lmiddlefinger" or
				"rringfinger" or "lringfinger" or "rpinkyfinger" or "lpinkyfinger" => 8,
			"rhip" or "lhip" or "rthigh" or "lthigh" or "rthighback" or "lthighback" => 30,
			"rknee" or "lknee" or "rkneeback" or "lkneeback" or "rshin" or "lshin" or "rcalf" or "lcalf" or
				"rankle" or "lankle" or "rheel" or "lheel" => 25,
			"rfoot" or "lfoot" => 25,
			"rbigtoe" or "lbigtoe" or "rindextoe" or "lindextoe" or "rmiddletoe" or "lmiddletoe" or "rringtoe" or
				"lringtoe" or "rpinkytoe" or "lpinkytoe" => 8,
			"groin" => 15,
			"testicles" or "penis" or "rnipple" or "lnipple" or "tongue" => 8,
			_ => fallback
		};
	}

	private string? ResolveHumanSeverFormula(string alias)
	{
		if (!UsesCombatRebalance)
		{
			return null;
		}

		return alias switch
		{
			"rear" or "lear" or "nose" or "reye" or "leye" or "rthumb" or "lthumb" or "rindexfinger" or
				"lindexfinger" or "rmiddlefinger" or "lmiddlefinger" or "rringfinger" or "lringfinger" or
				"rpinkyfinger" or "lpinkyfinger" or "rbigtoe" or "lbigtoe" or "rindextoe" or "lindextoe" or
				"rmiddletoe" or "lmiddletoe" or "rringtoe" or "lringtoe" or "rpinkytoe" or "lpinkytoe" =>
				"if(damage < 8, 0, if(damage >= 18, 1, if(rand(1,100) <= ((damage-8) * 10), 1, 0)))",
			"rhand" or "lhand" or "rfoot" or "lfoot" =>
				"if(damage < 15, 0, if(damage >= 30, 1, if(rand(1,100) <= ((damage-15) * 6), 1, 0)))",
			_ => null
		};
	}

	private void RefreshExistingHumanCombatBalance()
	{
		ConfigureHumanNaturalArmours();
		RefreshExistingHumanBodyparts("Humanoid");
		RefreshExistingHumanBodyparts("Organic Humanoid");

		foreach (Race race in _context.Races
			         .Where(x => x.Name == "Humanoid" || x.Name == "Organic Humanoid" || x.Name == "Human")
			         .ToList())
		{
			race.NaturalArmourType = _racialNaturalArmour;
			race.BodypartHealthMultiplier = 1.0;
		}

		_context.SaveChanges();
	}

	private void RefreshExistingHumanBodyparts(string bodyName)
	{
		BodyProto? body = _context.BodyProtos.FirstOrDefault(x => x.Name == bodyName);
		if (body is null)
		{
			return;
		}

		foreach (BodypartProto bodypart in _context.BodypartProtos.Where(x => x.BodyId == body.Id).ToList())
		{
			if (bodypart.IsOrgan == 1)
			{
				bodypart.ArmourType = _organArmour;
				continue;
			}

			bodypart.MaxLife = ResolveHumanMaxLife(bodypart.Name, bodypart.MaxLife);
			bodypart.RelativeHitChance = ResolveHumanRelativeHitChance(bodypart.Name, bodypart.RelativeHitChance);
			bodypart.ArmourType = GetDefaultNaturalArmour(bodypart.Name);
			bodypart.SeverFormula = ResolveHumanSeverFormula(bodypart.Name);
		}
	}

	private void ConfigureHumanNaturalArmours()
	{
		_racialNaturalArmour = EnsureHumanArmourType(
			"Human Racial Tissue Armour",
			UsesCombatRebalance
				? BuildHumanRacialRebalanceArmourDefinition()
				: BuildNaturalArmourDefinition(
					RelaxedFleshDamageTransforms(),
					HumanRacialTissueDamageDissipateExpression,
					damageType => HumanSharedFleshDissipateExpression(damageType, "pain"),
					damageType => HumanSharedFleshDissipateExpression(damageType, "stun"),
					HumanRacialTissueDamageAbsorbExpression,
					damageType => HumanCurrentFleshDamageAbsorbExpression(damageType, "pain"),
					damageType => HumanCurrentFleshStunAbsorbExpression(damageType, "stun")));

		_bodypartNaturalArmour = EnsureHumanArmourType(
			"Human Natural Flesh Armour",
			UsesCombatRebalance
				? BuildHumanBodypartRebalanceArmourDefinition()
				: BuildNaturalArmourDefinition(
					RelaxedFleshDamageTransforms(),
					damageType => HumanSharedFleshDissipateExpression(damageType, "damage"),
					damageType => HumanSharedFleshDissipateExpression(damageType, "pain"),
					damageType => HumanSharedFleshDissipateExpression(damageType, "stun"),
					damageType => HumanCurrentFleshDamageAbsorbExpression(damageType, "damage"),
					damageType => HumanCurrentFleshDamageAbsorbExpression(damageType, "pain"),
					damageType => HumanCurrentFleshStunAbsorbExpression(damageType, "stun")));

		_cranialNaturalArmour = EnsureHumanArmourType(
			"Human Cranial Flesh Armour",
			UsesCombatRebalance
				? BuildHumanCranialRebalanceArmourDefinition()
				: BuildNaturalArmourDefinition(
					RelaxedFleshDamageTransforms(),
					damageType => HumanSharedFleshDissipateExpression(damageType, "damage"),
					damageType => HumanSharedFleshDissipateExpression(damageType, "pain"),
					damageType => HumanSharedFleshDissipateExpression(damageType, "stun"),
					HumanCranialFleshDamageAbsorbExpression,
					damageType => HumanCurrentFleshDamageAbsorbExpression(damageType, "pain"),
					damageType => HumanCurrentFleshStunAbsorbExpression(damageType, "stun")));

		_organArmour = EnsureHumanArmourType("Human Natural Organ Armour", BuildHumanOrganArmourDefinition());
	}

	private ArmourType EnsureHumanArmourType(string name, string definition)
	{
		ArmourType? existing = _context.ArmourTypes.FirstOrDefault(x => x.Name == name);
		if (existing is not null)
		{
			existing.MinimumPenetrationDegree = 1;
			existing.BaseDifficultyDegrees = 0;
			existing.StackedDifficultyDegrees = 0;
			existing.Definition = definition;
			return existing;
		}

		ArmourType armour = new()
		{
			Name = name,
			MinimumPenetrationDegree = 1,
			BaseDifficultyDegrees = 0,
			StackedDifficultyDegrees = 0,
			Definition = definition
		};
		_context.ArmourTypes.Add(armour);
		_context.SaveChanges();
		return armour;
	}

	private static string BuildHumanRacialRebalanceArmourDefinition()
	{
		return BuildNaturalArmourDefinition(
			RelaxedFleshDamageTransforms(),
			damageType => RebalanceSoftTissueDissipateExpression(damageType, "damage", 9.0, 1.65),
			damageType => RebalanceSoftTissueDissipateExpression(damageType, "pain", 9.0, 1.65),
			damageType => RebalanceSoftTissueDissipateExpression(damageType, "stun", 8.0, 1.35),
			damageType => RebalanceSoftTissueAbsorbExpression(damageType, "damage", 0.40, 0.82),
			damageType => RebalanceSoftTissueAbsorbExpression(damageType, "pain", 0.45, 0.88),
			damageType => RebalanceSoftTissueAbsorbExpression(damageType, "stun", 0.55, 1.0));
	}

	private static string BuildHumanBodypartRebalanceArmourDefinition()
	{
		return BuildNaturalArmourDefinition(
			RelaxedFleshDamageTransforms(),
			damageType => RebalanceSoftTissueDissipateExpression(damageType, "damage", 7.0, 1.45),
			damageType => RebalanceSoftTissueDissipateExpression(damageType, "pain", 7.0, 1.45),
			damageType => RebalanceSoftTissueDissipateExpression(damageType, "stun", 6.0, 1.20),
			damageType => RebalanceSoftTissueAbsorbExpression(damageType, "damage", 0.52, 0.9),
			damageType => RebalanceSoftTissueAbsorbExpression(damageType, "pain", 0.58, 0.95),
			damageType => RebalanceSoftTissueAbsorbExpression(damageType, "stun", 0.75, 1.0));
	}

	private static string BuildHumanCranialRebalanceArmourDefinition()
	{
		return BuildNaturalArmourDefinition(
			RelaxedFleshDamageTransforms(),
			damageType => RebalanceSoftTissueDissipateExpression(damageType, "damage", 8.5, 1.55),
			damageType => RebalanceSoftTissueDissipateExpression(damageType, "pain", 8.5, 1.55),
			damageType => RebalanceSoftTissueDissipateExpression(damageType, "stun", 7.0, 1.25),
			damageType => RebalanceSoftTissueAbsorbExpression(damageType, "damage", 0.45, 0.86),
			damageType => RebalanceSoftTissueAbsorbExpression(damageType, "pain", 0.52, 0.92),
			damageType => RebalanceSoftTissueAbsorbExpression(damageType, "stun", 0.68, 1.0));
	}

	private static string BuildHumanOrganArmourDefinition()
	{
		return BuildNaturalArmourDefinition(
			RelaxedFleshDamageTransforms(),
			damageType => HumanSharedFleshDissipateExpression(damageType, "damage"),
			damageType => HumanSharedFleshDissipateExpression(damageType, "pain"),
			damageType => HumanSharedFleshDissipateExpression(damageType, "stun"),
			damageType => HumanCurrentFleshDamageAbsorbExpression(damageType, "damage"),
			damageType => HumanCurrentFleshDamageAbsorbExpression(damageType, "pain"),
			damageType => HumanCurrentFleshStunAbsorbExpression(damageType, "stun"));
	}

	private static string RebalanceSoftTissueDissipateExpression(DamageType damageType, string valueName,
		double qualityFactor, double damageFactor)
	{
		if (damageType is DamageType.Hypoxia or DamageType.Cellular)
		{
			return "0";
		}

		return
			$"if(rand(1,100)<=min(65,max(0,(quality*{qualityFactor:0.0}) + (strength/4500) - ({valueName}*{damageFactor:0.00}))),0,{valueName})";
	}

	private static string RebalanceSoftTissueAbsorbExpression(DamageType damageType, string valueName,
		double reducedFactor, double fullPassFactor)
	{
		if (damageType is DamageType.Hypoxia or DamageType.Cellular)
		{
			return "0";
		}

		return
			$"{valueName} * if(rand(1,100)<=min(80,max(15,(quality*10) + (strength/3500) - {valueName})),{reducedFactor:0.00},{fullPassFactor:0.00})";
	}
}
