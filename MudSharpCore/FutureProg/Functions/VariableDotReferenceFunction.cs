namespace MudSharp.FutureProg.Functions;

/// <summary>
///     Handles a "Dot" reference to a variable property, e.g. @Target.Describe
/// </summary>
internal class VariableDotReferenceFunction : UnaryFunction
{
	protected string TargetProperty;

	public VariableDotReferenceFunction(IFunction lhs, string targetProperty, FutureProgVariableTypes returnType)
		: base(lhs)
	{
		TargetProperty = targetProperty;
		ReturnType = returnType;
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (InnerFunction.Result == null)
		{
			Result = null;
			return StatementResult.Normal;
		}

		Result = InnerFunction.Result.GetProperty(TargetProperty);
		return StatementResult.Normal;
	}
}