#nullable enable

using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private CombatBalanceProfile _combatBalanceProfile = CombatBalanceProfile.Stock;

	private bool UsesCombatRebalance => _combatBalanceProfile == CombatBalanceProfile.CombatRebalance;

	private IReadOnlyDictionary<string, string> BuildAnimalDamageExpressions()
	{
		if (UsesCombatRebalance)
		{
			return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				["Small Animal Bite Damage"] = $"0.28 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1)",
				["Fish Bite Damage"] = $"0.32 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1)",
				["Herbivorous Animal Bite Damage"] = $"0.34 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1)",
				["Carnivorous Animal Bite Damage"] = $"0.50 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1)",
				["Shark Bite Damage"] = $"0.62 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1)",
				["Animal Claw Damage"] = $"0.46 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1)",
				["Animal Peck Damage"] = $"0.22 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1)",
				["Animal Talon Damage"] = $"0.40 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1)",
				["Animal Mandible Damage"] = $"0.24 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1)",
				["Animal Ram Damage"] = $"0.48 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1)",
				["Animal Smash Damage"] = $"0.44 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1)",
				["Dragonfire Breath Damage"] = "0.44 * (24 + (3 * quality)) * sqrt(degree+1)",
				["Animal Coup De Grace Damage"] = $"0.90 * str:{_strengthTrait.Id} * quality * sqrt(degree+1)",
				["Snake Bite Damage"] = $"0.30 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1)"
			};
		}

		string randomPortion = "";
		switch (_questionAnswers["random"].ToLowerInvariant())
		{
			case "partial":
				randomPortion = " * rand(0.7,1.0)";
				break;
			case "random":
				randomPortion = " * rand(0.2,1.0)";
				break;
		}

		return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["Small Animal Bite Damage"] = $"0.5 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}",
			["Fish Bite Damage"] = $"0.5 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}",
			["Herbivorous Animal Bite Damage"] = $"0.5 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1){randomPortion}",
			["Carnivorous Animal Bite Damage"] = $"1.0 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1){randomPortion}",
			["Shark Bite Damage"] = $"1.0 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1){randomPortion}",
			["Animal Claw Damage"] = $"1.0 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1){randomPortion}",
			["Animal Peck Damage"] = $"0.45 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}",
			["Animal Talon Damage"] = $"0.8 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}",
			["Animal Mandible Damage"] = $"0.35 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}",
			["Animal Ram Damage"] = $"0.9 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}",
			["Animal Smash Damage"] = $"0.8 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}",
			["Dragonfire Breath Damage"] = $"0.65 * (24 + (3 * quality)) * sqrt(degree+1){randomPortion}",
			["Animal Coup De Grace Damage"] = $"1.5 * str:{_strengthTrait.Id} * quality * sqrt(degree+1){randomPortion}",
			["Snake Bite Damage"] = $"0.5 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1){randomPortion}"
		};
	}

	private TraitExpression EnsureAnimalDamageExpression(string name, string expression)
	{
		TraitExpression? existing = _context.TraitExpressions.FirstOrDefault(x => x.Name == name);
		if (existing is not null)
		{
			existing.Expression = expression;
			return existing;
		}

		TraitExpression created = new()
		{
			Name = name,
			Expression = expression
		};
		_context.TraitExpressions.Add(created);
		_context.SaveChanges();
		return created;
	}

	private void ConfigureAnimalNaturalArmours()
	{
		_naturalArmour = EnsureAnimalArmourType(
			"Non-Human Natural Armour",
			UsesCombatRebalance
				? BuildAnimalRebalanceNaturalArmour()
				: BuildNaturalArmourDefinition(
					RelaxedAnimalFleshDamageTransforms(),
					type => AnimalNaturalDissipateExpression(type, "damage"),
					type => AnimalNaturalDissipateExpression(type, "pain"),
					type => AnimalNaturalDissipateExpression(type, "stun"),
					type => AnimalNaturalDamageAbsorbExpression(type, "damage"),
					type => AnimalNaturalDamageAbsorbExpression(type, "pain"),
					type => AnimalNaturalStunAbsorbExpression(type, "stun")));
	}

	private ArmourType EnsureAnimalArmourType(string name, string definition)
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

	private static string BuildAnimalRebalanceNaturalArmour()
	{
		return BuildNaturalArmourDefinition(
			RelaxedAnimalFleshDamageTransforms(),
			type => BuildAnimalRebalanceDissipateExpression(type, "damage", 10.0, 1.55),
			type => BuildAnimalRebalanceDissipateExpression(type, "pain", 10.0, 1.55),
			type => BuildAnimalRebalanceDissipateExpression(type, "stun", 8.0, 1.20),
			type => BuildAnimalRebalanceAbsorbExpression(type, "damage", 0.45, 0.88),
			type => BuildAnimalRebalanceAbsorbExpression(type, "pain", 0.52, 0.94),
			type => BuildAnimalRebalanceAbsorbExpression(type, "stun", 0.70, 1.0));
	}

	private static string BuildAnimalRebalanceDissipateExpression(DamageType damageType, string valueName,
		double qualityFactor, double damageFactor)
	{
		if (damageType is DamageType.Hypoxia or DamageType.Cellular)
		{
			return "0";
		}

		return
			$"if(rand(1,100)<=min(68,max(0,(quality*{qualityFactor:0.0}) + (strength/4200) - ({valueName}*{damageFactor:0.00}))),0,{valueName})";
	}

	private static string BuildAnimalRebalanceAbsorbExpression(DamageType damageType, string valueName,
		double reducedFactor, double fullFactor)
	{
		if (damageType is DamageType.Hypoxia or DamageType.Cellular)
		{
			return "0";
		}

		return
			$"{valueName} * if(rand(1,100)<=min(82,max(15,(quality*11) + (strength/3200) - {valueName})),{reducedFactor:0.00},{fullFactor:0.00})";
	}

	private double ResolveAnimalRaceHealthMultiplier(AnimalRaceTemplate? template, SizeCategory size, double fallback)
	{
		if (!UsesCombatRebalance)
		{
			return fallback;
		}

		NonHumanAttributeProfile profile = template is not null
			? GetAnimalAttributeProfile(template)
			: GetAnimalSizeProfile(size);
		double sizeFactor = size switch
		{
			SizeCategory.Tiny => 0.45,
			SizeCategory.VerySmall => 0.65,
			SizeCategory.Small => 0.85,
			SizeCategory.Normal => 1.0,
			SizeCategory.Large => 1.35,
			SizeCategory.VeryLarge => 1.75,
			SizeCategory.Huge => 2.15,
			SizeCategory.Enormous => 2.65,
			SizeCategory.Gigantic => 3.1,
			SizeCategory.Titanic => 3.6,
			_ => fallback
		};
		double constitutionFactor = 1.0 + (profile.ConstitutionBonus * 0.04);
		return Math.Round(sizeFactor * constitutionFactor, 2);
	}

	private int ResolveAnimalBodypartLife(string alias, SizeCategory size, int fallback)
	{
		if (!UsesCombatRebalance)
		{
			return fallback;
		}

		double sizeFactor = size switch
		{
			SizeCategory.Tiny => 0.45,
			SizeCategory.VerySmall => 0.6,
			SizeCategory.Small => 0.8,
			SizeCategory.Normal => 1.0,
			SizeCategory.Large => 1.3,
			SizeCategory.VeryLarge => 1.7,
			SizeCategory.Huge => 2.1,
			SizeCategory.Enormous => 2.6,
			SizeCategory.Gigantic => 3.0,
			SizeCategory.Titanic => 3.5,
			_ => 1.0
		};

		double roleFactor = alias.Contains("eye", StringComparison.OrdinalIgnoreCase) ||
		                    alias.Contains("ear", StringComparison.OrdinalIgnoreCase) ||
		                    alias.Contains("claw", StringComparison.OrdinalIgnoreCase) ||
		                    alias.Contains("hoof", StringComparison.OrdinalIgnoreCase) ||
		                    alias.Contains("fang", StringComparison.OrdinalIgnoreCase)
			? 0.6
			: alias.Contains("head", StringComparison.OrdinalIgnoreCase) ||
			  alias.Contains("neck", StringComparison.OrdinalIgnoreCase) ||
			  alias.Contains("wing", StringComparison.OrdinalIgnoreCase) ||
			  alias.Contains("tail", StringComparison.OrdinalIgnoreCase)
				? 0.85
				: 1.0;

		return Math.Max(4, (int)Math.Round(fallback * sizeFactor * roleFactor, MidpointRounding.AwayFromZero));
	}

	private int ResolveAnimalRelativeHitChance(BodyProto body, string alias, BodypartTypeEnum type, SizeCategory size,
		int fallback)
	{
		int baseChance = GetAnimalRelativeHitChance(body, alias, fallback);
		if (!UsesCombatRebalance)
		{
			return baseChance;
		}

		double modifier = type switch
		{
			BodypartTypeEnum.Eye or BodypartTypeEnum.Ear or BodypartTypeEnum.Tongue => 0.65,
			BodypartTypeEnum.Wing or BodypartTypeEnum.Fin => 0.9,
			_ => 1.0
		};

		modifier *= size switch
		{
			SizeCategory.Tiny => 0.85,
			SizeCategory.VerySmall => 0.9,
			SizeCategory.Large => 1.1,
			SizeCategory.VeryLarge => 1.2,
			SizeCategory.Huge => 1.3,
			SizeCategory.Enormous => 1.4,
			SizeCategory.Gigantic => 1.5,
			SizeCategory.Titanic => 1.6,
			_ => 1.0
		};

		return Math.Max(1, (int)Math.Round(baseChance * modifier, MidpointRounding.AwayFromZero));
	}

	private string? ResolveAnimalSeverFormula(string alias, SizeCategory size)
	{
		if (!UsesCombatRebalance)
		{
			return null;
		}

		if (AnimalMinorSeverKeywords.Any(alias.Contains))
		{
			return "if(damage < 8, 0, if(damage >= 18, 1, if(rand(1,100) <= ((damage-8) * 9), 1, 0)))";
		}

		if (alias.Contains("tail", StringComparison.OrdinalIgnoreCase) ||
		    alias.Contains("wing", StringComparison.OrdinalIgnoreCase))
		{
			return size >= SizeCategory.Large
				? "if(damage < 16, 0, if(damage >= 34, 1, if(rand(1,100) <= ((damage-16) * 5), 1, 0)))"
				: "if(damage < 12, 0, if(damage >= 26, 1, if(rand(1,100) <= ((damage-12) * 6), 1, 0)))";
		}

		return null;
	}

	private void RefreshExistingAnimalCombatBalance()
	{
		ConfigureAnimalNaturalArmours();
		IReadOnlyDictionary<string, string> expressions = BuildAnimalDamageExpressions();
		foreach (KeyValuePair<string, string> formula in expressions)
		{
			EnsureAnimalDamageExpression(formula.Key, formula.Value);
		}

		RefreshDragonfireBreathDamageExpression(expressions);

		foreach (string bodyName in new[]
		         {
			         "Quadruped Base",
			         "Ungulate",
			         "Toed Quadruped",
			         "Pinniped",
			         "Avian",
			         "Vermiform",
			         "Serpentine",
			         "Piscine",
			         "Cetacean",
			         "Cephalopod",
			         "Jellyfish",
			         "Insectoid",
			         "Winged Insectoid",
			         "Beetle",
			         "Centipede"
		         })
		{
			BodyProto? body = _context.BodyProtos.FirstOrDefault(x => x.Name == bodyName);
			if (body is null)
			{
				continue;
			}

			foreach (BodypartProto bodypart in _context.BodypartProtos.Where(x => x.BodyId == body.Id && x.IsOrgan != 1).ToList())
			{
				BodypartTypeEnum type = (BodypartTypeEnum)bodypart.BodypartType;
				SizeCategory size = (SizeCategory)bodypart.Size;
				bodypart.MaxLife = ResolveAnimalBodypartLife(bodypart.Name, size, bodypart.MaxLife);
				bodypart.RelativeHitChance = ResolveAnimalRelativeHitChance(body, bodypart.Name, type, size, bodypart.RelativeHitChance);
				bodypart.ArmourType = _naturalArmour;
				bodypart.SeverFormula = ResolveAnimalSeverFormula(bodypart.Name, size);
			}
		}

		ApplyDefaultCombatSettingsToSeededRaces();
		_context.SaveChanges();
	}

	private void RefreshDragonfireBreathDamageExpression(IReadOnlyDictionary<string, string> expressions)
	{
		TraitExpression dragonfireDamage = EnsureAnimalDamageExpression(
			"Dragonfire Breath Damage",
			expressions["Dragonfire Breath Damage"]);
		WeaponAttack? dragonfire = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Dragonfire Breath");
		if (dragonfire is null)
		{
			return;
		}

		dragonfire.DamageExpression = dragonfireDamage;
		dragonfire.PainExpression = dragonfireDamage;
		dragonfire.StunExpression = dragonfireDamage;
	}
}
