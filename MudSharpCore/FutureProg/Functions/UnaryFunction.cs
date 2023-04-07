namespace MudSharp.FutureProg.Functions;

internal abstract class UnaryFunction : Function
{
	protected IFunction InnerFunction;

	protected UnaryFunction(IFunction innerFunction)
	{
		InnerFunction = innerFunction;
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var functionResult = InnerFunction.Execute(variables);
		if (functionResult == StatementResult.Error)
		{
			ErrorMessage = InnerFunction.ErrorMessage;
			return StatementResult.Error;
		}

		return StatementResult.Normal;
	}
}