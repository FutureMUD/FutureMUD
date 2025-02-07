﻿using System;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class TimeSpanMultiplicationByNumberFunction : BinaryFunction
{
	public TimeSpanMultiplicationByNumberFunction(IFunction lhs, IFunction rhs)
		: base(lhs, rhs)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.TimeSpan;
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
			ErrorMessage = "Attempted to multiply null timespan by number (or reverse)";
			return StatementResult.Error;
		}

		Result =
			new TimeSpanVariable(
				TimeSpan.FromTicks((long)(((TimeSpan)LHS.Result.GetObject).Ticks * (decimal)RHS.Result.GetObject)));
		return StatementResult.Normal;
	}
}