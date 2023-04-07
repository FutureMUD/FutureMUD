using System;

namespace MudSharp.FutureProg.Functions;

internal class ConstantFunction : IFunction
{
	public ConstantFunction(IFutureProgVariable variable)
	{
		Result = variable;
	}

	public IFutureProgVariable Result { get; protected set; }

	public FutureProgVariableTypes ReturnType => Result.Type | FutureProgVariableTypes.Literal;

	public StatementResult ExpectedResult => StatementResult.Normal;

	public string ErrorMessage => throw new NotSupportedException();

	public StatementResult Execute(IVariableSpace variables)
	{
		return StatementResult.Normal;
	}
}