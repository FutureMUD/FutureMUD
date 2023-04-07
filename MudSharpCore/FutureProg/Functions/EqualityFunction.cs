using System;
using MudSharp.Form.Shape;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions;

internal class EqualityFunction : BinaryFunction
{
	public EqualityFunction(IFunction lhs, IFunction rhs)
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
			case FutureProgVariableTypes.Boolean:
				Result =
					new BooleanVariable((bool)(LHS?.Result?.GetObject ?? false) ==
					                    (bool)(RHS?.Result?.GetObject ?? false));
				break;
			case FutureProgVariableTypes.Number:
				Result = new BooleanVariable((decimal)(LHS?.Result?.GetObject ?? decimal.MinValue) ==
				                             (decimal)(RHS?.Result?.GetObject ?? decimal.MinValue));
				break;
			case FutureProgVariableTypes.Text:
				Result =
					new BooleanVariable(string.Equals((string)LHS?.Result?.GetObject,
						(string)RHS?.Result?.GetObject, StringComparison.InvariantCultureIgnoreCase));
				break;
			case FutureProgVariableTypes.Gender:
				Result = new BooleanVariable((Gender)(LHS?.Result?.GetObject ?? Gender.Indeterminate) ==
				                             (Gender)(RHS?.Result?.GetObject ?? Gender.Indeterminate));
				break;
			case FutureProgVariableTypes.DateTime:
				Result =
					new BooleanVariable(
						((System.DateTime)(LHS?.Result?.GetObject ?? System.DateTime.MinValue)).Equals(
							(System.DateTime)(RHS?.Result?.GetObject ?? System.DateTime.MinValue)));
				break;
			case FutureProgVariableTypes.TimeSpan:
				Result =
					new BooleanVariable(((TimeSpan)LHS.Result.GetObject).Equals((TimeSpan)RHS.Result.GetObject));
				break;
			default:
				Result = new BooleanVariable(LHS?.Result?.GetObject == RHS?.Result?.GetObject);
				break;
		}

		return StatementResult.Normal;
	}
}