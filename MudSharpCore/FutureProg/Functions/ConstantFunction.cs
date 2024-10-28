using System;

namespace MudSharp.FutureProg.Functions;

internal class ConstantFunction : IFunction
{
	public ConstantFunction(IProgVariable variable)
	{
		Result = variable;
	}

	public IProgVariable Result { get; protected set; }

	public ProgVariableTypes ReturnType => Result.Type | ProgVariableTypes.Literal;

	public StatementResult ExpectedResult => StatementResult.Normal;

	public string ErrorMessage => throw new NotSupportedException();

	public StatementResult Execute(IVariableSpace variables)
	{
		return StatementResult.Normal;
	}

	public virtual bool IsReturnOrContainsReturnOnAllBranches() => false;
}