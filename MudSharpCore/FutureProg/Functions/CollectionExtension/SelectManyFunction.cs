using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.CollectionExtension;

internal class SelectManyFunction : CollectionExtensionFunction
{
	public SelectManyFunction(string variableName, IFunction collectionItemFunction, IFunction collectionFunction)
		: base(variableName, collectionItemFunction, collectionFunction)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => CollectionItemFunction.ReturnType;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (CollectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "The collection function in the SelectMany statement returned an error: " +
			               CollectionFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var resultCollection = new List<IFutureProgVariable>();
		var localVariables = new LocalVariableSpace(variables);
		foreach (IFutureProgVariable item in (IEnumerable)CollectionFunction.Result)
		{
			localVariables.SetVariable(VariableName, item);
			if (CollectionItemFunction.Execute(localVariables) == StatementResult.Error)
			{
				ErrorMessage = "Encountered error while enumerating Collection in SelectMany function: " +
				               CollectionItemFunction.ErrorMessage;
				return StatementResult.Error;
			}

			resultCollection.AddRange(((IEnumerable)CollectionItemFunction.Result).Cast<IFutureProgVariable>());
		}

		Result = new CollectionVariable(resultCollection,
			CollectionFunction.ReturnType ^ FutureProgVariableTypes.Collection);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterCollectionExtensionFunctionCompiler(
			new CollectionExtensionFunctionCompilerInformation(
				"selectmany",
				FutureProgVariableTypes.Collection,
				(varName, collectionFunction, innerFunction) =>
					new SelectManyFunction(varName, innerFunction, collectionFunction),
				@"The SELECTMANY function is used when you have a collection of things that themselves contain another collection, and you want to aggregate them all together.

For example, if you have a ROOM COLLECTION and you want to get all of the characters in all of those rooms, you could do .SelectMany(x, @x.Characters) and you would have a CHARACTER COLLECTION with all these characters.",
				"Collection (Inner Type)"
			)
		);
	}
}