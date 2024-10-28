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

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		switch (LHS.ReturnType & ~ProgVariableTypes.Literal)
		{
			case ProgVariableTypes.Boolean:
				Result =
					new BooleanVariable((bool)(LHS?.Result?.GetObject ?? false) ==
					                    (bool)(RHS?.Result?.GetObject ?? false));
				break;
			case ProgVariableTypes.Number:
				Result = new BooleanVariable((decimal)(LHS?.Result?.GetObject ?? decimal.MinValue) ==
				                             (decimal)(RHS?.Result?.GetObject ?? decimal.MinValue));
				break;
			case ProgVariableTypes.Text:
				Result =
					new BooleanVariable(string.Equals((string)LHS?.Result?.GetObject,
						(string)RHS?.Result?.GetObject, StringComparison.InvariantCultureIgnoreCase));
				break;
			case ProgVariableTypes.Gender:
				Result = new BooleanVariable((Gender)(LHS?.Result?.GetObject ?? Gender.Indeterminate) ==
				                             (Gender)(RHS?.Result?.GetObject ?? Gender.Indeterminate));
				break;
			case ProgVariableTypes.DateTime:
				Result =
					new BooleanVariable(
						((System.DateTime)(LHS?.Result?.GetObject ?? System.DateTime.MinValue)).Equals(
							(System.DateTime)(RHS?.Result?.GetObject ?? System.DateTime.MinValue)));
				break;
			case ProgVariableTypes.TimeSpan:
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