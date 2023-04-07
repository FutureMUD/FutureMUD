using System;
using System.Collections.Generic;
using MudSharp.Accounts;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class TimeSpanToTextFunction : BuiltInFunction
{
	private TimeSpanToTextFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Text;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}


		var timespan = (TimeSpan)ParameterFunctions[0].Result.GetObject;
		Result = ParameterFunctions.Count == 1
			? new TextVariable(timespan.Describe())
			: new TextVariable(timespan.Describe(((IHaveAccount)ParameterFunctions[1].Result.GetObject).Account));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"totext",
			new[] { FutureProgVariableTypes.TimeSpan },
			(pars, gameworld) => new TimeSpanToTextFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"totext",
			new[] { FutureProgVariableTypes.TimeSpan, FutureProgVariableTypes.Toon },
			(pars, gameworld) => new TimeSpanToTextFunction(pars)
		));
	}
}