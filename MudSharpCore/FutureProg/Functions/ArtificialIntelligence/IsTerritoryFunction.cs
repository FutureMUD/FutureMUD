using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Effects.Concrete;

namespace MudSharp.FutureProg.Functions.ArtificialIntelligence;

internal class IsTerritoryFunction : BuiltInFunction
{
	protected IsTerritoryFunction(IList<IFunction> parameters, int flagCount = 0) : base(parameters)
	{
		FlagCount = flagCount;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public int FlagCount { get; protected set; }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var source = (ICharacter)ParameterFunctions[0].Result;
		if (source == null)
		{
			ErrorMessage = "Source Character was null in IsTerritory function.";
			return StatementResult.Error;
		}

		var target = (ICell)ParameterFunctions[1].Result;
		if (target == null)
		{
			ErrorMessage = "Target Cell was null in IsTerritory function.";
			return StatementResult.Error;
		}

		var effect = source.EffectsOfType<Territory>().FirstOrDefault();
		if (effect == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		for (var i = 0; i < FlagCount; i++)
		{
			if (!effect.HasFlag(target, ParameterFunctions[i + 2].Result?.GetObject?.ToString() ?? string.Empty))
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}
		}

		Result = new BooleanVariable(effect.Cells.Contains(target));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"isterritory",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Location },
			(pars, gameworld) => new IsTerritoryFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"isterritory",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Text },
			(pars, gameworld) => new IsTerritoryFunction(pars, 1)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"isterritory",
			new[]
			{
				ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Text,
				ProgVariableTypes.Text
			},
			(pars, gameworld) => new IsTerritoryFunction(pars, 2)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"isterritory",
			new[]
			{
				ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Text,
				ProgVariableTypes.Text, ProgVariableTypes.Text
			},
			(pars, gameworld) => new IsTerritoryFunction(pars, 3)
		));
	}
}