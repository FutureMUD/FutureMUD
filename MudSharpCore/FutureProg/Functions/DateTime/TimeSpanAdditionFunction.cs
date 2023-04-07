using System;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class TimeSpanAdditionFunction : BinaryFunction
{
	public TimeSpanAdditionFunction(IFunction lhs, IFunction rhs)
		: base(lhs, rhs)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.TimeSpan;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (LHS.Result?.GetObject == null || RHS.Result?.GetObject == null)
		{
			ErrorMessage = "Attempted to add null timespans";
			return StatementResult.Error;
		}

		Result = new TimeSpanVariable((TimeSpan)LHS.Result.GetObject + (TimeSpan)RHS.Result.GetObject);
		return StatementResult.Normal;
	}
}