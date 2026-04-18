#nullable enable

using MudSharp.Combat;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Models;
using System;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
	private static string BuildNaturalArmourDefinition(
		System.Collections.Generic.IEnumerable<(DamageType From, DamageType To, WoundSeverity Threshold)> transforms,
		Func<DamageType, string> damageDissipate,
		Func<DamageType, string> painDissipate,
		Func<DamageType, string> stunDissipate,
		Func<DamageType, string> damageAbsorb,
		Func<DamageType, string> painAbsorb,
		Func<DamageType, string> stunAbsorb)
	{
		static XElement BuildExpressionSet(string name, Func<DamageType, string> factory)
		{
			return new XElement(name,
				Enum.GetValues(typeof(DamageType))
					.OfType<DamageType>()
					.Select(type => new XElement("Expression",
						new XAttribute("damagetype", (int)type),
						factory(type))));
		}

		var root = new XElement("ArmourType",
			new XElement("DamageTransformations",
				transforms.Select(x => new XElement("Transform",
					new XAttribute("fromtype", (int)x.From),
					new XAttribute("totype", (int)x.To),
					new XAttribute("severity", (int)x.Threshold)))),
			BuildExpressionSet("DissipateExpressions", damageDissipate),
			BuildExpressionSet("DissipateExpressionsPain", painDissipate),
			BuildExpressionSet("DissipateExpressionsStun", stunDissipate),
			BuildExpressionSet("AbsorbExpressions", damageAbsorb),
			BuildExpressionSet("AbsorbExpressionsPain", painAbsorb),
			BuildExpressionSet("AbsorbExpressionsStun", stunAbsorb)
		);

		return root.ToString(SaveOptions.DisableFormatting);
	}

	private static System.Collections.Generic.IEnumerable<(DamageType From, DamageType To, WoundSeverity Threshold)>
		RobotFrameTransforms()
	{
		yield return (DamageType.Slashing, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Chopping, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Piercing, DamageType.Crushing, WoundSeverity.Superficial);
		yield return (DamageType.Ballistic, DamageType.Crushing, WoundSeverity.Superficial);
		yield return (DamageType.Bite, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Claw, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Shearing, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Wrenching, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Shrapnel, DamageType.Crushing, WoundSeverity.Superficial);
		yield return (DamageType.ArmourPiercing, DamageType.ArmourPiercing, WoundSeverity.Horrifying);
	}

	private static System.Collections.Generic.IEnumerable<(DamageType From, DamageType To, WoundSeverity Threshold)>
		RobotPlatingTransforms()
	{
		yield return (DamageType.Slashing, DamageType.Crushing, WoundSeverity.VerySevere);
		yield return (DamageType.Chopping, DamageType.Crushing, WoundSeverity.VerySevere);
		yield return (DamageType.Piercing, DamageType.Crushing, WoundSeverity.Moderate);
		yield return (DamageType.Ballistic, DamageType.Crushing, WoundSeverity.Moderate);
		yield return (DamageType.Bite, DamageType.Crushing, WoundSeverity.Severe);
		yield return (DamageType.Claw, DamageType.Crushing, WoundSeverity.VerySevere);
		yield return (DamageType.Shearing, DamageType.Crushing, WoundSeverity.VerySevere);
		yield return (DamageType.Wrenching, DamageType.Crushing, WoundSeverity.Severe);
		yield return (DamageType.Shrapnel, DamageType.Crushing, WoundSeverity.Moderate);
		yield return (DamageType.ArmourPiercing, DamageType.ArmourPiercing, WoundSeverity.Horrifying);
	}

	private static bool IsRobotCutLikeDamage(DamageType damageType)
	{
		return damageType is DamageType.Slashing or
			DamageType.Chopping or
			DamageType.Piercing or
			DamageType.Ballistic or
			DamageType.Bite or
			DamageType.Claw or
			DamageType.Shearing or
			DamageType.BallisticArmourPiercing or
			DamageType.ArmourPiercing or
			DamageType.Shrapnel;
	}

	private static bool IsRobotImpactLikeDamage(DamageType damageType)
	{
		return damageType is DamageType.Crushing or
			DamageType.Shockwave or
			DamageType.Sonic or
			DamageType.Wrenching or
			DamageType.Falling;
	}

	private static string RobotNaturalDissipateExpression(DamageType damageType, string valueName, double cutFactor,
		double impactFactor, double defaultFactor)
	{
		if (IsRobotCutLikeDamage(damageType))
		{
			return $"{valueName} - (quality * strength/25000 * {cutFactor.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
		}

		if (IsRobotImpactLikeDamage(damageType))
		{
			return $"{valueName} - (quality * strength/10000 * {impactFactor.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
		}

		return $"{valueName} - (quality * {defaultFactor.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
	}

	private static string RobotFrameDamageAbsorbExpression(DamageType damageType, string valueName)
	{
		return damageType switch
		{
			DamageType.Hypoxia or DamageType.Cellular => "0",
			DamageType.Slashing or
			DamageType.Chopping or
			DamageType.Piercing or
			DamageType.Ballistic or
			DamageType.Bite or
			DamageType.Claw or
			DamageType.Shearing or
			DamageType.BallisticArmourPiercing or
			DamageType.ArmourPiercing or
			DamageType.Shrapnel => $"{valueName}*0.95",
			DamageType.Crushing or
			DamageType.Shockwave or
			DamageType.Sonic or
			DamageType.Wrenching or
			DamageType.Falling => $"{valueName}*0.9",
			_ => $"{valueName}*0.8"
		};
	}

	private static string RobotPlatingDamageAbsorbExpression(DamageType damageType, string valueName, bool light)
	{
		return damageType switch
		{
			DamageType.Hypoxia or DamageType.Cellular => "0",
			DamageType.Slashing or
			DamageType.Chopping or
			DamageType.Claw or
			DamageType.Shearing => light ? $"{valueName}*0.84" : $"{valueName}*0.76",
			DamageType.Piercing or
			DamageType.Ballistic or
			DamageType.BallisticArmourPiercing or
			DamageType.ArmourPiercing or
			DamageType.Shrapnel => light ? $"{valueName}*0.9" : $"{valueName}*0.84",
			DamageType.Crushing or
			DamageType.Shockwave or
			DamageType.Sonic or
			DamageType.Wrenching or
			DamageType.Falling => light ? $"{valueName}*0.92" : $"{valueName}*0.88",
			DamageType.Electrical => light ? $"{valueName}*0.7" : $"{valueName}*0.6",
			_ => light ? $"{valueName}*0.82" : $"{valueName}*0.75"
		};
	}

	private static string RobotInternalDamageAbsorbExpression(DamageType damageType, string valueName)
	{
		return damageType switch
		{
			DamageType.Hypoxia or DamageType.Cellular => "0",
			DamageType.Electrical => $"{valueName}*0.65",
			DamageType.Burning or DamageType.Freezing or DamageType.Chemical => $"{valueName}*0.75",
			_ => $"{valueName}*0.85"
		};
	}

	private static string RobotNaturalStunAbsorbExpression(DamageType damageType, string valueName)
	{
		return damageType switch
		{
			DamageType.Hypoxia or DamageType.Cellular => "0",
			_ => valueName
		};
	}

	private ArmourType EnsureArmourDefinition(string name, string definition)
	{
		ArmourType? existing = _context.ArmourTypes.FirstOrDefault(x => x.Name == name);
		if (existing is not null)
		{
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

    private CorpseModel EnsureCorpseClone(string name, CorpseModel template, string replacementSubject)
    {
        CorpseModel? existing = _context.CorpseModels.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing;
        }

        CorpseModel corpse = new()
        {
            Name = name,
            Type = template.Type,
            Description = $"a {replacementSubject}",
            Definition = template.Definition
                .Replace("corpse", "wreck", StringComparison.OrdinalIgnoreCase)
                .Replace("flesh", "plating", StringComparison.OrdinalIgnoreCase)
                .Replace("skeletal remains", "component remains", StringComparison.OrdinalIgnoreCase)
                .Replace("skin", "panelling", StringComparison.OrdinalIgnoreCase)
        };
        _context.CorpseModels.Add(corpse);
        _context.SaveChanges();
        return corpse;
    }

    private FutureProg EnsureFutureProg(string functionName, string category, string subcategory, string comment,
        long returnType, string functionText, params (string Name, ProgVariableTypes Type)[] parameters)
    {
        FutureProg? existing = _context.FutureProgs.FirstOrDefault(x => x.FunctionName == functionName);
        if (existing is not null)
        {
            return existing;
        }

        FutureProg prog = new()
        {
            FunctionName = functionName,
            Category = category,
            Subcategory = subcategory,
            FunctionComment = comment,
            ReturnType = returnType,
            AcceptsAnyParameters = false,
            Public = true,
            StaticType = 0,
            FunctionText = functionText
        };
        for (int i = 0; i < parameters.Length; i++)
        {
            prog.FutureProgsParameters.Add(new FutureProgsParameter
            {
                FutureProg = prog,
                ParameterIndex = i,
                ParameterName = parameters[i].Name,
                ParameterTypeDefinition = parameters[i].Type.ToStorageString()
            });
        }

        _context.FutureProgs.Add(prog);
        _context.SaveChanges();
        return prog;
    }

    private TraitExpression EnsureTraitExpression(string name, string expression)
    {
        TraitExpression? existing = _context.TraitExpressions.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing;
        }

        TraitExpression traitExpression = new()
        {
            Name = name,
            Expression = expression
        };
        _context.TraitExpressions.Add(traitExpression);
        _context.SaveChanges();
        return traitExpression;
    }

    private HealthStrategy EnsureHealthStrategy(string name, string type, string definition)
    {
        HealthStrategy? existing = _context.HealthStrategies.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing;
        }

        HealthStrategy strategy = new()
        {
            Name = name,
            Type = type,
            Definition = definition
        };
        _context.HealthStrategies.Add(strategy);
        _context.SaveChanges();
        return strategy;
    }

    private WeaponAttack EnsureAttackClone(string name, string donorName, string shapeName, string message)
    {
        WeaponAttack? existing = _context.WeaponAttacks.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing;
        }

        WeaponAttack donor = _context.WeaponAttacks.First(x => x.Name == donorName);
        BodypartShape shape = _context.BodypartShapes.First(x => x.Name == shapeName);
        WeaponAttack attack = new()
        {
            Name = name,
            WeaponTypeId = donor.WeaponTypeId,
            Verb = donor.Verb,
            FutureProgId = donor.FutureProgId,
            BaseAttackerDifficulty = donor.BaseAttackerDifficulty,
            BaseBlockDifficulty = donor.BaseBlockDifficulty,
            BaseDodgeDifficulty = donor.BaseDodgeDifficulty,
            BaseParryDifficulty = donor.BaseParryDifficulty,
            BaseAngleOfIncidence = donor.BaseAngleOfIncidence,
            RecoveryDifficultySuccess = donor.RecoveryDifficultySuccess,
            RecoveryDifficultyFailure = donor.RecoveryDifficultyFailure,
            MoveType = donor.MoveType,
            Intentions = donor.Intentions,
            ExertionLevel = donor.ExertionLevel,
            DamageType = donor.DamageType,
            DamageExpressionId = donor.DamageExpressionId,
            StunExpressionId = donor.StunExpressionId,
            PainExpressionId = donor.PainExpressionId,
            Weighting = donor.Weighting,
            BodypartShapeId = shape.Id,
            StaminaCost = donor.StaminaCost,
            BaseDelay = donor.BaseDelay,
            Orientation = donor.Orientation,
            Alignment = donor.Alignment,
            AdditionalInfo = donor.AdditionalInfo,
            HandednessOptions = donor.HandednessOptions,
            RequiredPositionStateIds = donor.RequiredPositionStateIds
        };
        _context.WeaponAttacks.Add(attack);
        _context.SaveChanges();

        CombatMessage combatMessage = new()
        {
            Type = donor.MoveType,
            Message = message,
            Priority = 50,
            Verb = donor.Verb,
            Chance = 1.0,
            FailureMessage = message
        };
        combatMessage.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
        {
            CombatMessage = combatMessage,
            WeaponAttack = attack
        });
        _context.CombatMessages.Add(combatMessage);
        _context.SaveChanges();
        return attack;
    }

    private Material FindTemplateMaterial(MudSharp.Form.Material.MaterialBehaviourType behaviour)
    {
        return _context.Materials.FirstOrDefault(x => x.BehaviourType == (int)behaviour) ??
               _context.Materials.First();
    }
}
