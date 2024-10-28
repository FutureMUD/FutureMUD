using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class GetIndexFunction : BuiltInFunction
{
	public GetIndexFunction(IList<IFunction> parameters)
		: base(parameters)
	{
		CollectionFunction = parameters.ElementAtOrDefault(0);
	}

	private IFunction CollectionFunction { get; }

	public override ProgVariableTypes ReturnType
	{
		get => (CollectionFunction?.ReturnType ?? ProgVariableTypes.CollectionItem) ^
		       ProgVariableTypes.Collection;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var itemFunction = ParameterFunctions.ElementAt(1);

		if (CollectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Collection Function in GetIndex Function returned an error: " +
			               CollectionFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if (itemFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Item Function in GetIndex Function returned an error: " + itemFunction.ErrorMessage;
			return StatementResult.Error;
		}

		Result =
			((IList<IProgVariable>)CollectionFunction.Result.GetObject).ElementAtOrDefault(
				(int)(decimal)itemFunction.Result.GetObject);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"getindex",
				new[] { ProgVariableTypes.Collection, ProgVariableTypes.Number },
				(pars, gameworld) => new GetIndexFunction(pars),
				new[] { "collection", "index" },
				new[]
				{
					"A collection of anything",
					"The zero-based index of the element you want to retrieve from the collection"
				},
				"This function returns the element at the specified index of a collection, or null if an incorrect index is specified. The return type depends on the return type of the collection. You may need to convert the result. See CONVERT function.",
				"Collections",
				ProgVariableTypes.CollectionItem
			)
		);
	}
}