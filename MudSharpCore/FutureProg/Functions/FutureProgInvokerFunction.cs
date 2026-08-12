
namespace MudSharp.FutureProg.Functions;

internal class FutureProgInvokerFunction : Function
{
	protected IFunction[] ParameterFunctions;
    protected IFutureProg TargetProg;

    public FutureProgInvokerFunction(IFutureProg targetProg, IEnumerable<IFunction> parameterFunctions)
    {
        TargetProg = targetProg;
		ParameterFunctions = parameterFunctions.ToArray();
        ReturnType = targetProg.ReturnType;
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
		var parameterValues = new object[ParameterFunctions.Length];
		for (var i = 0; i < ParameterFunctions.Length; i++)
        {
			var parameter = ParameterFunctions[i];
			if (parameter.Execute(variables) == StatementResult.Error)
			{
				ErrorMessage = "Parameter Error: " + parameter.ErrorMessage;
				return StatementResult.Error;
			}

			parameterValues[i] = parameter.Result.GetObject;
        }

		object resultObject = TargetProg.ExecuteWithRecursionProtection(parameterValues);
        Result = FutureProg.GetVariable(ReturnType, resultObject);
        return StatementResult.Normal;
    }
}
