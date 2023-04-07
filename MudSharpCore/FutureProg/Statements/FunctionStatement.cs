namespace MudSharp.FutureProg.Statements;

internal class FunctionStatement : Statement
{
	public FunctionStatement(IFunction function)
	{
		Function = function;
	}

	public IFunction Function { get; set; }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (Function.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = Function.ErrorMessage;
			return StatementResult.Error;
		}

		return StatementResult.Normal;
	}
}