using System;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Mathematical;

internal class DivisionFunction : BinaryFunction
{
	public DivisionFunction(IFunction lhs, IFunction rhs)
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

		try
		{
			Result = new NumberVariable((decimal)LHS.Result.GetObject / (decimal)RHS.Result.GetObject);
		}
		catch (DivideByZeroException)
		{
			Result = new NumberVariable(0);
		}

		return StatementResult.Normal;
	}
}