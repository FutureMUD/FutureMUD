using System;
using System.Collections;
using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.CollectionExtension;

internal class StdDevFunction : CollectionExtensionFunction
{
	public StdDevFunction(string variableName, IFunction collectionItemFunction, IFunction collectionFunction)
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
				"stddev",
				FutureProgVariableTypes.Number,
				(varName, collectionFunction, innerFunction) =>
					new StdDevFunction(varName, innerFunction, collectionFunction),
				@"The STDDEV function runs the inner function (which itself returns a number) over all elements of the collection, and then returns the standard deviation of the results. Will return 0 if the collection is empty.

Note: If the collection itself is a collection of numbers, you can use the pattern .StdDev(x, @x)",
				"Number"
			)
		);
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (CollectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "The collection function in the StdDev statement returned an error: " +
			               CollectionFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var localVariables = new LocalVariableSpace(variables);
		var resultCollection = new List<double>();
		foreach (IFutureProgVariable item in (IList)CollectionFunction.Result.GetObject)
		{
			localVariables.SetVariable(VariableName, item);
			if (CollectionItemFunction.Execute(localVariables) == StatementResult.Error)
			{
				ErrorMessage = "Encountered error while enumerating Collection in StdDev function: " +
				               CollectionItemFunction.ErrorMessage;
				return StatementResult.Error;
			}

			resultCollection.Add(Convert.ToDouble(CollectionItemFunction.Result.GetObject));
		}

		Result = new NumberVariable(resultCollection.StdDev(x => x));
		return StatementResult.Normal;
	}
}