using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class DateTimeDifferenceFunction : BinaryFunction
{
	public DateTimeDifferenceFunction(IFunction lhs, IFunction rhs)
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
			ErrorMessage = "Attempted to find difference between null datetimes";
			return StatementResult.Error;
		}

		Result =
			new TimeSpanVariable((System.DateTime)LHS.Result.GetObject - (System.DateTime)RHS.Result.GetObject);
		return StatementResult.Normal;
	}
}

internal class MudDateTimeDifferenceFunction : BinaryFunction
{
	public MudDateTimeDifferenceFunction(IFunction lhs, IFunction rhs)
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
			ErrorMessage = "Attempted to find difference between null muddatetimes";
			return StatementResult.Error;
		}

		Result = new TimeSpanVariable((MudDateTime)LHS.Result.GetObject - (MudDateTime)RHS.Result.GetObject);
		return StatementResult.Normal;
	}
}