#nullable enable

using System;
using System.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
	private ArmourType EnsureArmourClone(string name, string templateName)
	{
		var existing = _context.ArmourTypes.FirstOrDefault(x => x.Name == name);
		if (existing is not null)
		{
			return existing;
		}

		var template = _context.ArmourTypes.First(x => x.Name == templateName);
		var armour = new ArmourType
		{
			Name = name,
			MinimumPenetrationDegree = template.MinimumPenetrationDegree,
			BaseDifficultyDegrees = template.BaseDifficultyDegrees,
			StackedDifficultyDegrees = template.StackedDifficultyDegrees,
			Definition = template.Definition
		};
		_context.ArmourTypes.Add(armour);
		_context.SaveChanges();
		return armour;
	}

	private CorpseModel EnsureCorpseClone(string name, CorpseModel template, string replacementSubject)
	{
		var existing = _context.CorpseModels.FirstOrDefault(x => x.Name == name);
		if (existing is not null)
		{
			return existing;
		}

		var corpse = new CorpseModel
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
		long returnType, string functionText, params (string Name, long Type)[] parameters)
	{
		var existing = _context.FutureProgs.FirstOrDefault(x => x.FunctionName == functionName);
		if (existing is not null)
		{
			return existing;
		}

		var prog = new FutureProg
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
		for (var i = 0; i < parameters.Length; i++)
		{
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = i,
				ParameterName = parameters[i].Name,
				ParameterType = parameters[i].Type
			});
		}

		_context.FutureProgs.Add(prog);
		_context.SaveChanges();
		return prog;
	}

	private TraitExpression EnsureTraitExpression(string name, string expression)
	{
		var existing = _context.TraitExpressions.FirstOrDefault(x => x.Name == name);
		if (existing is not null)
		{
			return existing;
		}

		var traitExpression = new TraitExpression
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
		var existing = _context.HealthStrategies.FirstOrDefault(x => x.Name == name);
		if (existing is not null)
		{
			return existing;
		}

		var strategy = new HealthStrategy
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
		var existing = _context.WeaponAttacks.FirstOrDefault(x => x.Name == name);
		if (existing is not null)
		{
			return existing;
		}

		var donor = _context.WeaponAttacks.First(x => x.Name == donorName);
		var shape = _context.BodypartShapes.First(x => x.Name == shapeName);
		var attack = new WeaponAttack
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

		var combatMessage = new CombatMessage
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
