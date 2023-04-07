using System;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class DateTimeAdditionFunction : BinaryFunction
{
	public DateTimeAdditionFunction(IFunction lhs, IFunction rhs)
		: base(lhs, rhs)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.DateTime;
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
			ErrorMessage = "Attempted to add null datetime and timespan";
			return StatementResult.Error;
		}

		Result = new DateTimeVariable((System.DateTime)LHS.Result.GetObject + (TimeSpan)RHS.Result.GetObject);
		return StatementResult.Normal;
	}
}

internal class MudDateTimeAdditionFunction : BinaryFunction
{
	public MudDateTimeAdditionFunction(IFunction lhs, IFunction rhs)
		: base(lhs, rhs)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.MudDateTime;
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
			ErrorMessage = "Attempted to add null muddatetime and timespan";
			return StatementResult.Error;
		}

		Result = (MudDateTime)LHS.Result.GetObject + (TimeSpan)RHS.Result.GetObject;
		return StatementResult.Normal;
	}
}