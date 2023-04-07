using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions;

/// <summary>
///     A built-in function is a type of Function that can be invoked with a FNAME(PARAMS) type invocation. They must
///     implement a GetFutureProgCompiler method.
/// </summary>
internal abstract class BuiltInFunction : Function
{
	protected IList<IFunction> ParameterFunctions;

	protected BuiltInFunction(IList<IFunction> parameterFunctions)
	{
		ParameterFunctions = parameterFunctions;
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		foreach (var par in ParameterFunctions.Where(par => par.Execute(variables) == StatementResult.Error))
		{
			ErrorMessage = par.ErrorMessage;
			return StatementResult.Error;
		}

		return StatementResult.Normal;
	}
}