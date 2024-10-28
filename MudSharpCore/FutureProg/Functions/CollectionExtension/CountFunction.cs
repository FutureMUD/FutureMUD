using System.Collections;
using System.Collections.Generic;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.CollectionExtension;

internal class CountFunction : CollectionExtensionFunction
{
	public CountFunction(string variableName, IFunction collectionItemFunction, IFunction collectionFunction)
		: base(variableName, collectionItemFunction, collectionFunction)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterCollectionExtensionFunctionCompiler(
			new CollectionExtensionFunctionCompilerInformation(
				"count",
				ProgVariableTypes.Boolean,
				(varName, collectionFunction, innerFunction) =>
					new CountFunction(varName, innerFunction, collectionFunction),
				@"The COUNT function runs the supplied inner function (which itself returns a boolean value) over all elements of a collection, and counts the total number of elements that return true.

If you just want to take the count of the number of elements in a collection, use the .Count property instead of this function.",
				"Number"
			)
		);
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (CollectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "The collection function in the Count statement returned an error: " +
			               CollectionFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var resultCollection = new List<IProgVariable>();
		var localVariables = new LocalVariableSpace(variables);
		foreach (IProgVariable item in (IList)CollectionFunction.Result.GetObject)
		{
			localVariables.SetVariable(VariableName, item);
			if (CollectionItemFunction.Execute(localVariables) == StatementResult.Error)
			{
				ErrorMessage = "Encountered error while enumerating Collection in Count function: " +
				               CollectionItemFunction.ErrorMessage;
				return StatementResult.Error;
			}

			if ((bool)CollectionItemFunction.Result.GetObject)
			{
				resultCollection.Add(item);
			}
		}

		Result = new NumberVariable(resultCollection.Count);
		return StatementResult.Normal;
	}
}