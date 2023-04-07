using System.Collections.Generic;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class NullFunction : BuiltInFunction
{
	private NullFunction(IList<IFunction> parameters)
		: base(parameters)
	{
		ReturnType = FutureProg.GetTypeByName(parameters[0].Result.GetObject.ToString());
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		Result = FutureProgVariableTypes.ValueType.HasFlag(ReturnType) ? new NullVariable(ReturnType) : null;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"null",
			new[] { FutureProgVariableTypes.Text | FutureProgVariableTypes.Literal },
			(pars, gameworld) => new NullFunction(pars),
			new List<string> { "type" },
			new List<string> { "A text literal specifying the type of null variable." },
			"Returns a null variable of the type specified.",
			"Null Handling",
			FutureProgVariableTypes.Anything
		));
	}
}