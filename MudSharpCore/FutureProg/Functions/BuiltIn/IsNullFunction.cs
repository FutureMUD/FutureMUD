using System.Collections.Generic;
using System.Linq;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class IsNullFunction : BuiltInFunction
{
	protected IFunction InnerFunction;

	public IsNullFunction(IList<IFunction> parameters)
		: base(parameters)
	{
		InnerFunction = parameters.First();
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (InnerFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = InnerFunction.ErrorMessage;
			return StatementResult.Error;
		}

		Result = new BooleanVariable(InnerFunction.Result?.GetObject == null);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"isnull",
				new[] { ProgVariableTypes.CollectionItem },
				(pars, gameworld) => new IsNullFunction(pars),
				new[] { "item" },
				new List<string> { "The item that you want to test to see if it is null" },
				"This function accepts an item of a broad variety of types, and tests to see if it is currently null.",
				"Null Handling",
				ProgVariableTypes.Boolean
			)
		);
	}
}