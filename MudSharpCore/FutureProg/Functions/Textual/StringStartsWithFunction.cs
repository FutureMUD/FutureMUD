using System;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Textual;

internal class StringStartsWithFunction : BinaryFunction
{
	public StringStartsWithFunction(IFunction lhs, IFunction rhs)
		: base(lhs, rhs)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result =
			new BooleanVariable(((string)LHS.Result.GetObject).StartsWith((string)RHS.Result.GetObject,
				StringComparison.InvariantCultureIgnoreCase));
		return StatementResult.Normal;
	}
}