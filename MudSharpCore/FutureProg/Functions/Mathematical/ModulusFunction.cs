using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Mathematical;

internal class ModulusFunction : BinaryFunction
{
	public ModulusFunction(IFunction lhs, IFunction rhs)
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

		Result = new NumberVariable((decimal)LHS.Result.GetObject % (decimal)RHS.Result.GetObject);
		return StatementResult.Normal;
	}
}