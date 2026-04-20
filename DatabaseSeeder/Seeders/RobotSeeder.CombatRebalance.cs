#nullable enable

using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.GameItems;
using MudSharp.Health;
using System;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
	private CombatBalanceProfile _combatBalanceProfile = CombatBalanceProfile.Stock;

	private bool UsesCombatRebalance => _combatBalanceProfile == CombatBalanceProfile.CombatRebalance;

	private string BuildRobotFrameArmourDefinition()
	{
		return UsesCombatRebalance
			? BuildNaturalArmourDefinition(
				RobotFrameTransforms(),
				type => BuildRobotRebalanceDissipateExpression(type, "damage", 14.0, 0.90, 1.05, 0.55),
				type => BuildRobotRebalanceDissipateExpression(type, "pain", 14.0, 0.90, 1.05, 0.55),
				type => BuildRobotRebalanceDissipateExpression(type, "stun", 12.0, 0.75, 0.90, 0.50),
				type => BuildRobotRebalanceAbsorbExpression(type, "damage", 0.24, 0.68, 0.48),
				type => BuildRobotRebalanceAbsorbExpression(type, "pain", 0.30, 0.72, 0.54),
				type => BuildRobotRebalanceAbsorbExpression(type, "stun", 0.42, 0.78, 0.66))
			: BuildNaturalArmourDefinition(
				RobotFrameTransforms(),
				type => RobotNaturalDissipateExpression(type, "damage", 0.18, 0.15, 0.75),
				type => RobotNaturalDissipateExpression(type, "pain", 0.18, 0.15, 0.75),
				type => RobotNaturalDissipateExpression(type, "stun", 0.18, 0.15, 0.75),
				type => RobotFrameDamageAbsorbExpression(type, "damage"),
				type => RobotFrameDamageAbsorbExpression(type, "pain"),
				type => RobotNaturalStunAbsorbExpression(type, "stun"));
	}

	private string BuildRobotPlatingArmourDefinition(bool light)
	{
		return UsesCombatRebalance
			? BuildNaturalArmourDefinition(
				RobotPlatingTransforms(),
				type => BuildRobotRebalanceDissipateExpression(type, "damage", light ? 12.0 : 16.0, light ? 1.00 : 0.78, light ? 1.18 : 0.92, light ? 0.65 : 0.48),
				type => BuildRobotRebalanceDissipateExpression(type, "pain", light ? 12.0 : 16.0, light ? 1.00 : 0.78, light ? 1.18 : 0.92, light ? 0.65 : 0.48),
				type => BuildRobotRebalanceDissipateExpression(type, "stun", light ? 10.0 : 13.0, light ? 0.88 : 0.68, light ? 1.05 : 0.85, light ? 0.60 : 0.44),
				type => BuildRobotRebalanceAbsorbExpression(type, "damage", light ? 0.34 : 0.18, light ? 0.74 : 0.56, light ? 0.58 : 0.40),
				type => BuildRobotRebalanceAbsorbExpression(type, "pain", light ? 0.40 : 0.24, light ? 0.78 : 0.60, light ? 0.64 : 0.46),
				type => BuildRobotRebalanceAbsorbExpression(type, "stun", light ? 0.48 : 0.30, light ? 0.82 : 0.66, light ? 0.70 : 0.54))
			: BuildNaturalArmourDefinition(
				RobotPlatingTransforms(),
				type => RobotNaturalDissipateExpression(type, "damage", light ? 0.28 : 0.45, light ? 0.18 : 0.28, light ? 0.85 : 1.0),
				type => RobotNaturalDissipateExpression(type, "pain", light ? 0.28 : 0.45, light ? 0.18 : 0.28, light ? 0.85 : 1.0),
				type => RobotNaturalDissipateExpression(type, "stun", light ? 0.28 : 0.45, light ? 0.18 : 0.28, light ? 0.85 : 1.0),
				type => RobotPlatingDamageAbsorbExpression(type, "damage", light),
				type => RobotPlatingDamageAbsorbExpression(type, "pain", light),
				type => RobotNaturalStunAbsorbExpression(type, "stun"));
	}

	private string BuildRobotInternalArmourDefinition()
	{
		return UsesCombatRebalance
			? BuildNaturalArmourDefinition(
				RobotFrameTransforms(),
				type => BuildRobotRebalanceDissipateExpression(type, "damage", 9.0, 1.15, 1.25, 0.75),
				type => BuildRobotRebalanceDissipateExpression(type, "pain", 9.0, 1.15, 1.25, 0.75),
				type => BuildRobotRebalanceDissipateExpression(type, "stun", 8.0, 0.95, 1.05, 0.65),
				type => BuildRobotRebalanceAbsorbExpression(type, "damage", 0.46, 0.86, 0.62),
				type => BuildRobotRebalanceAbsorbExpression(type, "pain", 0.52, 0.88, 0.68),
				type => BuildRobotRebalanceAbsorbExpression(type, "stun", 0.60, 0.92, 0.76))
			: BuildNaturalArmourDefinition(
				RobotFrameTransforms(),
				type => RobotNaturalDissipateExpression(type, "damage", 0.1, 0.08, 0.6),
				type => RobotNaturalDissipateExpression(type, "pain", 0.1, 0.08, 0.6),
				type => RobotNaturalDissipateExpression(type, "stun", 0.1, 0.08, 0.6),
				type => RobotInternalDamageAbsorbExpression(type, "damage"),
				type => RobotInternalDamageAbsorbExpression(type, "pain"),
				type => RobotNaturalStunAbsorbExpression(type, "stun"));
	}

	private static string BuildRobotRebalanceDissipateExpression(DamageType damageType, string valueName,
		double qualityFactor, double impactFactor, double cutFactor, double defaultFactor)
	{
		if (damageType is DamageType.Hypoxia or DamageType.Cellular)
		{
			return "0";
		}

		double damageFactor = damageType == DamageType.Electrical
			? defaultFactor
			: IsRobotCutLikeDamage(damageType)
				? cutFactor
				: IsRobotImpactLikeDamage(damageType)
					? impactFactor
					: defaultFactor;
		return
			$"if(rand(1,100)<=min(92,max(12,(quality*{qualityFactor:0.0}) + (strength/2600) - ({valueName}*{damageFactor:0.00}))),0,{valueName})";
	}

	private static string BuildRobotRebalanceAbsorbExpression(DamageType damageType, string valueName,
		double reducedFactor, double fullFactor, double electricalFactor)
	{
		if (damageType is DamageType.Hypoxia or DamageType.Cellular)
		{
			return "0";
		}

		if (damageType == DamageType.Electrical)
		{
			return $"{valueName} * if(rand(1,100)<=min(88,max(20,(quality*10) + (strength/3200) - {valueName})),{electricalFactor:0.00},0.92)";
		}

		return
			$"{valueName} * if(rand(1,100)<=min(90,max(14,(quality*11) + (strength/3000) - {valueName})),{reducedFactor:0.00},{fullFactor:0.00})";
	}

	private double ResolveRobotHealthMultiplier(SizeCategory size, double fallback)
	{
		if (!UsesCombatRebalance)
		{
			return fallback;
		}

		return size switch
		{
			SizeCategory.Tiny => 0.55,
			SizeCategory.VerySmall => 0.75,
			SizeCategory.Small => 0.9,
			SizeCategory.Normal => 1.15,
			SizeCategory.Large => 1.55,
			SizeCategory.VeryLarge => 2.0,
			SizeCategory.Huge => 2.45,
			SizeCategory.Enormous => 2.95,
			SizeCategory.Gigantic => 3.4,
			SizeCategory.Titanic => 3.9,
			_ => fallback
		};
	}

	private int ResolveRobotBodypartLife(string alias, BodypartTypeEnum type, SizeCategory size, int fallback)
	{
		if (!UsesCombatRebalance)
		{
			return fallback;
		}

		double sizeFactor = size switch
		{
			SizeCategory.Tiny => 0.55,
			SizeCategory.VerySmall => 0.75,
			SizeCategory.Small => 0.9,
			SizeCategory.Normal => 1.0,
			SizeCategory.Large => 1.3,
			SizeCategory.VeryLarge => 1.7,
			SizeCategory.Huge => 2.05,
			SizeCategory.Enormous => 2.4,
			SizeCategory.Gigantic => 2.8,
			SizeCategory.Titanic => 3.2,
			_ => 1.0
		};

		double roleFactor = type switch
		{
			BodypartTypeEnum.Eye => 0.45,
			BodypartTypeEnum.Wing or BodypartTypeEnum.Fin => 0.8,
			_ => alias.Contains("sensor", StringComparison.OrdinalIgnoreCase) ||
			     alias.Contains("antenna", StringComparison.OrdinalIgnoreCase)
				? 0.55
				: alias.Contains("wheel", StringComparison.OrdinalIgnoreCase) ||
				  alias.Contains("track", StringComparison.OrdinalIgnoreCase)
					? 1.15
					: 1.0
		};

		return Math.Max(5, (int)Math.Round(fallback * sizeFactor * roleFactor, MidpointRounding.AwayFromZero));
	}

	private int ResolveRobotRelativeHitChance(BodypartTypeEnum type, SizeCategory size, int fallback)
	{
		if (!UsesCombatRebalance)
		{
			return fallback;
		}

		double modifier = type switch
		{
			BodypartTypeEnum.Eye => 0.7,
			BodypartTypeEnum.Wing or BodypartTypeEnum.Fin => 0.9,
			_ => 1.0
		};
		modifier *= size switch
		{
			SizeCategory.Tiny => 0.9,
			SizeCategory.VerySmall => 0.95,
			SizeCategory.Large => 1.08,
			SizeCategory.VeryLarge => 1.15,
			SizeCategory.Huge => 1.22,
			SizeCategory.Enormous => 1.3,
			SizeCategory.Gigantic => 1.38,
			SizeCategory.Titanic => 1.45,
			_ => 1.0
		};
		return Math.Max(1, (int)Math.Round(fallback * modifier, MidpointRounding.AwayFromZero));
	}

	private string? ResolveRobotSeverFormula(string alias, SizeCategory size, BodypartTypeEnum type)
	{
		if (!UsesCombatRebalance)
		{
			return null;
		}

		if (type == BodypartTypeEnum.Eye ||
		    alias.Contains("sensor", StringComparison.OrdinalIgnoreCase) ||
		    alias.Contains("antenna", StringComparison.OrdinalIgnoreCase))
		{
			return "if(damage < 8, 0, if(damage >= 18, 1, if(rand(1,100) <= ((damage-8) * 8), 1, 0)))";
		}

		if (alias.Contains("wheel", StringComparison.OrdinalIgnoreCase) ||
		    alias.Contains("track", StringComparison.OrdinalIgnoreCase) ||
		    alias.Contains("wing", StringComparison.OrdinalIgnoreCase) ||
		    alias.Contains("mandible", StringComparison.OrdinalIgnoreCase))
		{
			return size >= SizeCategory.Large
				? "if(damage < 18, 0, if(damage >= 38, 1, if(rand(1,100) <= ((damage-18) * 5), 1, 0)))"
				: "if(damage < 13, 0, if(damage >= 28, 1, if(rand(1,100) <= ((damage-13) * 6), 1, 0)))";
		}

		return null;
	}
}
