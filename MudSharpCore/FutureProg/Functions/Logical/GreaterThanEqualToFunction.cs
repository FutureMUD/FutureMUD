using System;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;

namespace MudSharp.FutureProg.Functions.Logical;

internal class GreaterThanEqualToFunction : BinaryFunction
{
	public GreaterThanEqualToFunction(IFunction lhs, IFunction rhs)
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

		switch (LHS.ReturnType & ~FutureProgVariableTypes.Literal)
		{
			case FutureProgVariableTypes.Number:
				Result = new BooleanVariable((decimal)LHS.Result.GetObject >= (decimal)RHS.Result.GetObject);
				break;
			case FutureProgVariableTypes.TimeSpan:
				Result = new BooleanVariable((TimeSpan)LHS.Result.GetObject >= (TimeSpan)RHS.Result.GetObject);
				break;
			case FutureProgVariableTypes.DateTime:
				Result =
					new BooleanVariable((System.DateTime)LHS.Result.GetObject >=
					                    (System.DateTime)RHS.Result.GetObject);
				break;
			case FutureProgVariableTypes.MudDateTime:
				Result =
					new BooleanVariable((MudDateTime)LHS.Result.GetObject >= (MudDateTime)RHS.Result.GetObject);
				break;
		}

		return StatementResult.Normal;
	}
}