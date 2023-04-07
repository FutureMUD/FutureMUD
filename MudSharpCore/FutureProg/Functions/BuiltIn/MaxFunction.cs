using System;
using System.Collections.Generic;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class MaxFunction : BuiltInFunction
{
	public MaxFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result =
			new NumberVariable(Math.Max((decimal)ParameterFunctions[0].Result.GetObject,
				(decimal)ParameterFunctions[1].Result.GetObject));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"max",
			new[] { FutureProgVariableTypes.Number, FutureProgVariableTypes.Number },
			(pars, gameworld) => new MaxFunction(pars)
		));
	}
}