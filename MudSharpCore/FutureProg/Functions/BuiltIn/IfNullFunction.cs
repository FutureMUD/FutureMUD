using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class IfNullFunction : BuiltInFunction
{
	public IfNullFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var testFunction = ParameterFunctions.ElementAt(0);
		if (testFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = testFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if (testFunction.Result?.GetObject == null)
		{
			var alternateFunction = ParameterFunctions.ElementAt(1);
			if (alternateFunction.Execute(variables) == StatementResult.Error)
			{
				ErrorMessage = alternateFunction.ErrorMessage;
				return StatementResult.Error;
			}

			Result = alternateFunction.Result;
			return StatementResult.Normal;
		}

		Result = testFunction.Result;
		return StatementResult.Normal;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ParameterFunctions[1].ReturnType;
		protected set => base.ReturnType = value;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ifnull",
				new[] { ProgVariableTypes.CollectionItem, ProgVariableTypes.CollectionItem },
				(pars, gameworld) => new IfNullFunction(pars),
				new[] { "item", "fallback" },
				new List<string>
				{
					"The item that you want to test to see if it is null",
					"The item you want to return if the first item is null"
				},
				"This function accepts an item of a broad variety of types, and tests to see if it is currently null. If it is not null, it returns the item you supplied in the first parameter. If it is null, it returns the item specified in the second parameter.",
				"Null Handling",
				ProgVariableTypes.CollectionItem,
				(x, y) =>
					x.Count() == 2 &&
					FutureProgVariableComparer.Instance.Equals(x.ElementAt(0), x.ElementAt(1))
			)
		);
	}
}