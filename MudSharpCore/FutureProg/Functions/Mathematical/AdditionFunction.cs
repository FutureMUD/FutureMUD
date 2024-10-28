using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Mathematical;

internal class AdditionFunction : BinaryFunction
{
	public AdditionFunction(IFunction lhs, IFunction rhs)
		: base(lhs, rhs)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
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
			ErrorMessage = "Attempted to add null numbers";
			return StatementResult.Error;
		}

		Result = new NumberVariable((decimal)LHS.Result.GetObject + (decimal)RHS.Result.GetObject);
		return StatementResult.Normal;
	}
}