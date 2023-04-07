using System;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class TimeSpanDivisionByTimeSpanFunction : BinaryFunction
{
	public TimeSpanDivisionByTimeSpanFunction(IFunction lhs, IFunction rhs)
		: base(lhs, rhs)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Number;
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
			ErrorMessage = "Attempted to divide null timespans";
			return StatementResult.Error;
		}

		Result = new NumberVariable(((TimeSpan)LHS.Result.GetObject).Ticks / ((TimeSpan)RHS.Result.GetObject).Ticks);
		return StatementResult.Normal;
	}
}