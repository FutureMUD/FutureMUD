using System.Collections;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.CollectionExtension;

internal class FindFunction : CollectionExtensionFunction
{
	public FindFunction(string variableName, IFunction collectionItemFunction, IFunction collectionFunction)
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
				"find",
				ProgVariableTypes.Number,
				(varName, collectionFunction, innerFunction) =>
					new FindFunction(varName, innerFunction, collectionFunction),
				@"The FIND function accepts an inner function (which must return a boolean) and returns the index position of the first item in the collection that is TRUE for the inner function. If it finds no matches, it will return -1.

For example if you had a CHARACTER COLLECTION, .Find(x, @x.Age > 50) would return the index (position in collection starting at zero) of the first character over the age of 50.",
				"Number (index)"
			)
		);
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (CollectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "The collection function in the Find statement returned an error: " +
			               CollectionFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var localVariables = new LocalVariableSpace(variables);
		var count = 0;
		foreach (IProgVariable item in (IList)CollectionFunction.Result.GetObject)
		{
			localVariables.SetVariable(VariableName, item);
			if (CollectionItemFunction.Execute(localVariables) == StatementResult.Error)
			{
				ErrorMessage = "Encountered error while enumerating Collection in Find function: " +
				               CollectionItemFunction.ErrorMessage;
				return StatementResult.Error;
			}

			if ((bool)CollectionItemFunction.Result.GetObject)
			{
				Result = new NumberVariable(count);
				return StatementResult.Normal;
			}

			count++;
		}

		Result = new NumberVariable(-1);
		return StatementResult.Normal;
	}
}