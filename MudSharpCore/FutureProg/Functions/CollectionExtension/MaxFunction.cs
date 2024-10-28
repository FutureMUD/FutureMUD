using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.CollectionExtension;

internal class MaxFunction : CollectionExtensionFunction
{
	public MaxFunction(string variableName, IFunction collectionItemFunction, IFunction collectionFunction)
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
				"max",
				ProgVariableTypes.Number,
				(varName, collectionFunction, innerFunction) =>
					new MaxFunction(varName, innerFunction, collectionFunction),
				@"The MAX function runs the inner function (which itself returns a number) over all elements of the collection, and then returns the maximum value of the results. Will return 0 if the collection is empty.

Note: If the collection itself is a collection of numbers, you can use the pattern .Max(x, @x)",
				"Number"
			)
		);
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (CollectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "The collection function in the Max statement returned an error: " +
			               CollectionFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var localVariables = new LocalVariableSpace(variables);
		var resultCollection = new List<decimal>();
		foreach (IProgVariable item in (IList)CollectionFunction.Result.GetObject)
		{
			localVariables.SetVariable(VariableName, item);
			if (CollectionItemFunction.Execute(localVariables) == StatementResult.Error)
			{
				ErrorMessage = "Encountered error while enumerating Collection in Max function: " +
				               CollectionItemFunction.ErrorMessage;
				return StatementResult.Error;
			}

			resultCollection.Add((decimal)CollectionItemFunction.Result.GetObject);
		}

		Result = new NumberVariable(resultCollection.DefaultIfEmpty(0).Max());
		return StatementResult.Normal;
	}
}