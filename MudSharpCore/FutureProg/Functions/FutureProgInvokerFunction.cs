using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions;

internal class FutureProgInvokerFunction : Function
{
	protected IEnumerable<IFunction> ParameterFunctions;
	protected IFutureProg TargetProg;

	public FutureProgInvokerFunction(IFutureProg targetProg, IEnumerable<IFunction> parameterFunctions)
	{
		TargetProg = targetProg;
		ParameterFunctions = parameterFunctions;
		ReturnType = targetProg.ReturnType;
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		foreach (var par in ParameterFunctions.Where(par => par.Execute(variables) == StatementResult.Error))
		{
			ErrorMessage = "Parameter Error: " + par.ErrorMessage;
			return StatementResult.Error;
		}

		var resultObject = TargetProg.Execute(ParameterFunctions.Select(x => x.Result.GetObject).ToArray());
		Result = FutureProg.GetVariable(ReturnType, resultObject);
		return StatementResult.Normal;
	}
}