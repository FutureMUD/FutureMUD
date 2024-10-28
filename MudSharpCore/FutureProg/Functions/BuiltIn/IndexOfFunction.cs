using System.Collections.Generic;
using System.Linq;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class IndexOfFunction : BuiltInFunction
{
	public IndexOfFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var collectionFunction = ParameterFunctions.ElementAt(0);
		var itemFunction = ParameterFunctions.ElementAt(1);

		if (collectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Collection Function in IndexOfFunction Function returned an error: " +
			               collectionFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if (itemFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Item Function in IndexOfFunction Function returned an error: " +
			               itemFunction.ErrorMessage;
			return StatementResult.Error;
		}

		Result =
			new NumberVariable(
				((IList<IProgVariable>)collectionFunction.Result.GetObject).IndexOf(
					(IProgVariable)itemFunction.Result.GetObject));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"indexof",
				new[] { ProgVariableTypes.Collection, ProgVariableTypes.CollectionItem },
				(pars, gameworld) => new IndexOfFunction(pars),
				new[] { "collection", "item" },
				new[] { "A collection of anything", "The item whose position in the collection you want to know" },
				"This function returns the zero-based index of a specified item in the collection, if present. If not present it returns -1.",
				"Collections",
				ProgVariableTypes.Number
			)
		);
	}
}