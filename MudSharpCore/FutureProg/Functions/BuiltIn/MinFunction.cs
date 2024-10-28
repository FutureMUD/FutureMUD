using System;
using System.Collections.Generic;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class MinFunction : BuiltInFunction
{
	public MinFunction(IList<IFunction> parameters)
		: base(parameters)
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

		Result =
			new NumberVariable(Math.Min((decimal)ParameterFunctions[0].Result.GetObject,
				(decimal)ParameterFunctions[1].Result.GetObject));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"min",
			new[] { ProgVariableTypes.Number, ProgVariableTypes.Number },
			(pars, gameworld) => new MinFunction(pars)
		));
	}
}