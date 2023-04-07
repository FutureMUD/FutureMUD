using System.Collections;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.CollectionExtension;

internal class AllFunction : CollectionExtensionFunction
{
	public AllFunction(string variableName, IFunction collectionItemFunction, IFunction collectionFunction)
		: base(variableName, collectionItemFunction, collectionFunction)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterCollectionExtensionFunctionCompiler(
			new CollectionExtensionFunctionCompilerInformation(
				"all",
				FutureProgVariableTypes.Boolean,
				(varName, collectionFunction, innerFunction) =>
					new AllFunction(varName, innerFunction, collectionFunction),
				"The ALL function runs the supplied inner function (which itself returns a boolean value) over all elements of a collection, and returns true if all the items return true. Otherwise, returns false. Also returns true if there are no elements.",
				"Boolean"
			)
		);
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (CollectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "The collection function in the All statement returned an error: " +
			               CollectionFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var localVariables = new LocalVariableSpace(variables);
		foreach (IFutureProgVariable item in (IList)CollectionFunction.Result.GetObject)
		{
			localVariables.SetVariable(VariableName, item);
			if (CollectionItemFunction.Execute(localVariables) == StatementResult.Error)
			{
				ErrorMessage = "Encountered error while enumerating Collection in All function: " +
				               CollectionItemFunction.ErrorMessage;
				return StatementResult.Error;
			}

			if (!(bool)CollectionItemFunction.Result.GetObject)
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}
		}

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}