using System.Collections;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.CollectionExtension;

internal class FirstFunction : CollectionExtensionFunction
{
	public FirstFunction(string variableName, IFunction collectionItemFunction, IFunction collectionFunction)
		: base(variableName, collectionItemFunction, collectionFunction)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => CollectionFunction.ReturnType ^ FutureProgVariableTypes.Collection;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (CollectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "The collection function in the First statement returned an error: " +
			               CollectionFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var localVariables = new LocalVariableSpace(variables);
		foreach (IFutureProgVariable item in (IList)CollectionFunction.Result.GetObject)
		{
			localVariables.SetVariable(VariableName, item);
			if (CollectionItemFunction.Execute(localVariables) == StatementResult.Error)
			{
				ErrorMessage = "Encountered error while enumerating Collection in First function: " +
				               CollectionItemFunction.ErrorMessage;
				return StatementResult.Error;
			}

			if ((bool)CollectionItemFunction.Result.GetObject)
			{
				Result = item;
				return StatementResult.Normal;
			}
		}

		Result = new NullVariable(CollectionFunction.ReturnType ^ FutureProgVariableTypes.Collection);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterCollectionExtensionFunctionCompiler(
			new CollectionExtensionFunctionCompilerInformation(
				"first",
				FutureProgVariableTypes.CollectionItem,
				(varName, collectionFunction, innerFunction) =>
					new FirstFunction(varName, innerFunction, collectionFunction),
				@"The FIRST function runs the inner function (which must return a boolean) over the collection and returns the first element from that collection that matches the criteria. If it doesn't find anything, it returns NULL, so you must protect against this by checking ISNULL or using IFNULL.

For example, if you had a CHARACTER COLLECTION and you ran .First(x, @x.Age > 35) it would either return the first character in the collection who was aged over 35 years old, or it would return null if nothing was found.",
				"Collection Item Type"
			)
		);
	}
}