using System.Collections;
using System.Collections.Generic;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.CollectionExtension;

internal class WhereFunction : CollectionExtensionFunction
{
	public WhereFunction(string variableName, IFunction collectionItemFunction, IFunction collectionFunction)
		: base(variableName, collectionItemFunction, collectionFunction)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => CollectionFunction.ReturnType;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (CollectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "The collection function in the Where statement returned an error: " +
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
				ErrorMessage = "Encountered error while enumerating Collection in Where function: " +
				               CollectionItemFunction.ErrorMessage;
				return StatementResult.Error;
			}

			if ((bool)CollectionItemFunction.Result.GetObject)
			{
				resultCollection.Add(item);
			}
		}

		Result = new CollectionVariable(resultCollection,
			CollectionFunction.ReturnType ^ ProgVariableTypes.Collection);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterCollectionExtensionFunctionCompiler(
			new CollectionExtensionFunctionCompilerInformation(
				"where",
				ProgVariableTypes.Boolean,
				(varName, collectionFunction, innerFunction) =>
					new WhereFunction(varName, innerFunction, collectionFunction),
				@"The WHERE function runs the inner function (which must return a boolean) over all items in the collection, and returns a new collection that only contain whichever items were TRUE from the inner function.

For example, if you had a CHARACTER COLLECTION, you could run .Where(x, @x.Age >= 21) to get all characters who are 21 years or older.",
				"Collection (Same Type)"
			)
		);
	}
}