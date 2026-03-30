#nullable enable
using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace $rootnamespace$;

internal class $safeitemrootname$ : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private $safeitemrootname$(IList<IFunction> parameterFunctions, IFuturemud gameworld)
		: base(parameterFunctions)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		// Use _gameworld and ParameterFunctions here.
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"$safeitemrootname$".ToLowerInvariant(),
				[ProgVariableTypes.Number],
				(pars, gameworld) => new $safeitemrootname$(pars, gameworld),
				["value"],
				["The input value for this function."],
				"Describe what this function returns.",
				"General",
				ProgVariableTypes.Boolean
			)
		);
	}
}
