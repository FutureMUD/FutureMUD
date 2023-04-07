using System.Collections;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.CollectionExtension;

internal class AnyFunction : CollectionExtensionFunction
{
	public AnyFunction(string variableName, IFunction collectionItemFunction, IFunction collectionFunction)
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
				"any",
				FutureProgVariableTypes.Boolean,
				(varName, collectionFunction, innerFunction) =>
					new AnyFunction(varName, innerFunction, collectionFunction),
				"The ANY function runs the supplied inner function (which itself returns a boolean value) over all elements of a collection, and returns true if any of the items return true. Otherwise, returns false. Returns false if there are no elements.",
				"Boolean"
			)
		);
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (CollectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "The collection function in the Any statement returned an error: " +
			               CollectionFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var localVariables = new LocalVariableSpace(variables);
		foreach (IFutureProgVariable item in (IList)CollectionFunction.Result.GetObject)
		{
			localVariables.SetVariable(VariableName, item);
			if (CollectionItemFunction.Execute(localVariables) == StatementResult.Error)
			{
				ErrorMessage = "Encountered error while enumerating Collection in Any function: " +
				               CollectionItemFunction.ErrorMessage;
				return StatementResult.Error;
			}

			if ((bool?)CollectionItemFunction.Result.GetObject ?? false)
			{
				Result = new BooleanVariable(true);
				return StatementResult.Normal;
			}
		}

		Result = new BooleanVariable(false);
		return StatementResult.Normal;
	}
}