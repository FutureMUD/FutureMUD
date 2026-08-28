#nullable enable

using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.NPC.Templates;

namespace MudSharp.FutureProg.Functions.Characters;

internal sealed class GetNPCSkillPackageFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;
	private readonly bool _byName;

	private GetNPCSkillPackageFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld, bool byName)
		: base(parameterFunctions)
	{
		_gameworld = gameworld;
		_byName = byName;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.NPCSkillPackage;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = _byName
			? _gameworld.NpcSkillPackages.GetByIdOrName(ParameterFunctions[0].Result?.GetObject?.ToString() ?? string.Empty)
			: _gameworld.NpcSkillPackages.Get((long)(decimal)ParameterFunctions[0].Result!.GetObject);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		foreach (var type in new[] { ProgVariableTypes.Number, ProgVariableTypes.Text })
		{
			var byName = type == ProgVariableTypes.Text;
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
				"npcskillpackage", [type],
				(pars, gameworld) => new GetNPCSkillPackageFunction(pars, gameworld, byName),
				[byName ? "name" : "id"],
				[byName ? "The package name or ID." : "The package ID."],
				"Gets an NPC skill package by ID or name, or returns null when none matches.",
				"NPCs", ProgVariableTypes.NPCSkillPackage));
		}
	}
}

internal sealed class ApplyNPCSkillPackageFunction : BuiltInFunction
{
	private ApplyNPCSkillPackageFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var package = ParameterFunctions[1].Result?.GetObject as INPCSkillPackage;
		if (character is null || package is null)
		{
			Result = new NumberVariable(0);
			return StatementResult.Normal;
		}

		var changed = 0;
		foreach (var entry in package.Skills)
		{
			if (Constants.Random.NextDouble() > entry.Chance)
			{
				continue;
			}

			var value = Math.Max(0.0,
				RandomUtilities.RandomSkewNormal(entry.Mean, entry.StandardDeviation, entry.Skewness));
			if (value <= 0.0 || character.HasTrait(entry.Skill) && character.TraitRawValue(entry.Skill) >= value)
			{
				continue;
			}

			if (character.HasTrait(entry.Skill))
			{
				character.SetTraitValue(entry.Skill, value);
			}
			else
			{
				character.AddTrait(entry.Skill, value);
			}

			changed++;
		}

		Result = new NumberVariable(changed);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"applyskillpackage", [ProgVariableTypes.Character, ProgVariableTypes.NPCSkillPackage],
			(pars, _) => new ApplyNPCSkillPackageFunction(pars),
			["character", "package"],
			["The character to receive the rolled package.", "The NPC skill package to apply."],
			"Rolls an NPC skill package and adds or raises skills without lowering existing values. Returns the number changed.",
			"NPCs", ProgVariableTypes.Number));
	}
}
