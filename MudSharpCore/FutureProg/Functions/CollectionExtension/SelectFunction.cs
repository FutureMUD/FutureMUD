using System.Collections;
using System.Collections.Generic;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.CollectionExtension;

internal class SelectFunction : CollectionExtensionFunction
{
	public SelectFunction(string variableName, IFunction collectionItemFunction, IFunction collectionFunction)
		: base(variableName, collectionItemFunction, collectionFunction)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => CollectionItemFunction.ReturnType | FutureProgVariableTypes.Collection;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (CollectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "The collection function in the Select statement returned an error: " +
			               CollectionFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var resultCollection = new List<IFutureProgVariable>();
		var localVariables = new LocalVariableSpace(variables);
		foreach (IFutureProgVariable item in (IList)CollectionFunction.Result.GetObject)
		{
			localVariables.SetVariable(VariableName, item);
			if (CollectionItemFunction.Execute(localVariables) == StatementResult.Error)
			{
				ErrorMessage = "Encountered error while enumerating Collection in Select function: " +
				               CollectionItemFunction.ErrorMessage;
				return StatementResult.Error;
			}

			resultCollection.Add(CollectionItemFunction.Result);
		}

		Result = new CollectionVariable(resultCollection,
			CollectionFunction.ReturnType ^ FutureProgVariableTypes.Collection);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterCollectionExtensionFunctionCompiler(
			new CollectionExtensionFunctionCompilerInformation(
				"select",
				FutureProgVariableTypes.CollectionItem,
				(varName, collectionFunction, innerFunction) =>
					new SelectFunction(varName, innerFunction, collectionFunction),
				@"The SELECT function is used to transform a collection of one type of thing into a collection of something else. It will run the inner function over each item in the collection and return a collection of those items.

For example, if you have a CHARACTER COLLECTION and you run .Select(x, @x.Name) over the collection, it will instead be a TEXT COLLECTION containing all the character names.",
				"Collection (Inner Type)"
			)
		);
	}
}