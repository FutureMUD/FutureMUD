using System;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Textual;

internal class StringConcatenationFunction : BinaryFunction
{
	public StringConcatenationFunction(IFunction lhs, IFunction rhs)
		: base(lhs, rhs)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Text;
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
			Result = new TextVariable(string.Empty);
			Console.WriteLine("Attempted to concatenate null strings in prog.");
			return StatementResult.Normal;
		}

		Result = new TextVariable(LHS.Result.GetObject + RHS.Result.GetObject.ToString());
		return StatementResult.Normal;
	}
}