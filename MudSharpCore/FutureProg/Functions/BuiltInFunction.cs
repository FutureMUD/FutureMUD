
namespace MudSharp.FutureProg.Functions;

/// <summary>
///     A built-in function is a type of Function that can be invoked with a FNAME(PARAMS) type invocation. They must
///     implement a GetFutureProgCompiler method.
/// </summary>
internal abstract class BuiltInFunction : Function
{
    protected IList<IFunction> ParameterFunctions;

    protected BuiltInFunction(IList<IFunction> parameterFunctions)
    {
        ParameterFunctions = parameterFunctions;
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
		for (var i = 0; i < ParameterFunctions.Count; i++)
        {
			var parameter = ParameterFunctions[i];
			if (parameter.Execute(variables) == StatementResult.Error)
			{
				ErrorMessage = parameter.ErrorMessage;
				return StatementResult.Error;
			}
        }

        return StatementResult.Normal;
    }
}
