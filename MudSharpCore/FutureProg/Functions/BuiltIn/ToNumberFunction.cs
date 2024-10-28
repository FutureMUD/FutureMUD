using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToNumberFunction : BuiltInFunction
{
	public ToNumberFunction(IList<IFunction> parameters)
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

		Result = decimal.TryParse(Convert.ToString(ParameterFunctions.First().Result.GetObject), out var value)
			? new NumberVariable(value)
			: ParameterFunctions.Last().Result;

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tonumber",
			new[] { ProgVariableTypes.Text },
			(pars, gameworld) =>
				new ToNumberFunction(
					pars.Concat(new IFunction[] { new ConstantFunction(new NumberVariable(0)) }).ToList())
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tonumber",
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number },
			(pars, gameworld) => new ToNumberFunction(pars)
		));
	}
}