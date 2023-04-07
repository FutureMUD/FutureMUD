using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Logical;

internal class AndFunction : BinaryFunction
{
	public AndFunction(IFunction lhs, IFunction rhs)
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
		var lhsResult = LHS.Execute(variables);
		if (lhsResult == StatementResult.Error)
		{
			ErrorMessage = LHS.ErrorMessage;
			return StatementResult.Error;
		}

		// do not evaluate RHS if LHS evaluates to false
		if (!(bool)LHS.Result.GetObject)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var rhsResult = RHS.Execute(variables);
		if (rhsResult == StatementResult.Error)
		{
			ErrorMessage = RHS.ErrorMessage;
			return StatementResult.Error;
		}

		if (!(bool)RHS.Result.GetObject)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}