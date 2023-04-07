using System;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Mathematical;

internal class PowerFunction : BinaryFunction
{
	public PowerFunction(IFunction lhs, IFunction rhs)
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

		Result =
			new NumberVariable(Math.Pow(Convert.ToDouble(LHS.Result.GetObject),
				Convert.ToDouble(RHS.Result.GetObject)));
		return StatementResult.Normal;
	}
}