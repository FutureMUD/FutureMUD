using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class WordyNumberFunction : BuiltInFunction
{
	private readonly Func<int, string> _wordyNumberFunction;

	public WordyNumberFunction(IList<IFunction> parameters, Func<int, string> wordyNumberFunction)
		: base(parameters)
	{
		_wordyNumberFunction = wordyNumberFunction;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Text;
		protected set { }
	}

	public override string ErrorMessage
	{
		get => ParameterFunctions.First().ErrorMessage;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var result = _wordyNumberFunction((int)(decimal)ParameterFunctions.First().Result.GetObject);
		Result = new TextVariable(result);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tonumberwords",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new WordyNumberFunction(pars, NumberUtilities.ToWordyNumber)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toordinalwords",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new WordyNumberFunction(pars, NumberUtilities.ToWordyOrdinal)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toordinal",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new WordyNumberFunction(pars, NumberUtilities.ToOrdinal)
		));
	}
}