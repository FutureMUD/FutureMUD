using System.Collections.Generic;
using System.Linq;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToTextFunction : BuiltInFunction
{
	public ToTextFunction(IList<IFunction> parameters)
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

		Result = new TextVariable(ParameterFunctions.First().Result.GetObject.ToString());
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"totext",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new ToTextFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"totext",
			new[] { FutureProgVariableTypes.Boolean },
			(pars, gameworld) => new ToTextFunction(pars)
		));
	}
}