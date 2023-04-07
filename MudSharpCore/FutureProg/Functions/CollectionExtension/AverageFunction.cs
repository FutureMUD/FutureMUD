using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.CollectionExtension;

internal class AverageFunction : CollectionExtensionFunction
{
	public AverageFunction(string variableName, IFunction collectionItemFunction, IFunction collectionFunction)
		: base(variableName, collectionItemFunction, collectionFunction)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Number;
		protected set { }
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterCollectionExtensionFunctionCompiler(
			new CollectionExtensionFunctionCompilerInformation(
				"average",
				FutureProgVariableTypes.Number,
				(varName, collectionFunction, innerFunction) =>
					new AverageFunction(varName, innerFunction, collectionFunction),
				@"The AVERAGE function runs the inner function (which itself returns a number) over all elements of the collection, and then returns the numerical average (mean) of the results. Will return 0 if the collection is empty.

Note: If the collection itself is a collection of numbers, you can use the pattern .Average(x, @x)",
				"Number"
			));
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (CollectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "The collection function in the Average statement returned an error: " +
			               CollectionFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var localVariables = new LocalVariableSpace(variables);
		var resultCollection = new List<decimal>();
		foreach (IFutureProgVariable item in (IList)CollectionFunction.Result.GetObject)
		{
			localVariables.SetVariable(VariableName, item);
			if (CollectionItemFunction.Execute(localVariables) == StatementResult.Error)
			{
				ErrorMessage = "Encountered error while enumerating Collection in Average function: " +
				               CollectionItemFunction.ErrorMessage;
				return StatementResult.Error;
			}

			resultCollection.Add((decimal)CollectionItemFunction.Result.GetObject);
		}

		Result = new NumberVariable(resultCollection.DefaultIfEmpty(0).Average());
		return StatementResult.Normal;
	}
}