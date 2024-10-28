using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Form.Shape;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ContainsFunction : BuiltInFunction
{
	public ContainsFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var collectionFunction = ParameterFunctions.ElementAt(0);
		var itemFunction = ParameterFunctions.ElementAt(1);

		if (collectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Collection Function in Contains Function returned an error: " +
			               collectionFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if (itemFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Item Function in Contains Function returned an error: " + itemFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if (!itemFunction.ReturnType.CompatibleWith(collectionFunction.ReturnType ^
		                                            ProgVariableTypes.Collection))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (collectionFunction.Result?.GetObject == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var iResult = itemFunction.Result;
		switch (itemFunction.ReturnType & ~ProgVariableTypes.Literal)
		{
			case ProgVariableTypes.Boolean:
				var targetBool = (bool?)iResult?.GetObject;
				Result = new BooleanVariable(((IList)collectionFunction.Result.GetObject).OfType<IProgVariable>()
					.Any(x => x.GetObject as bool? == targetBool));
				break;
			case ProgVariableTypes.Number:
				var targetNumber = (decimal?)iResult?.GetObject;
				Result = new BooleanVariable(((IList)collectionFunction.Result.GetObject).OfType<IProgVariable>()
					.Any(x => x.GetObject as decimal? == targetNumber));
				break;
			case ProgVariableTypes.Text:
				var targetText = (string)iResult?.GetObject;
				Result = new BooleanVariable(((IList)collectionFunction.Result.GetObject).OfType<IProgVariable>()
					.Any(x => string.Equals(x.GetObject as string, targetText,
						StringComparison.InvariantCultureIgnoreCase)));
				break;
			case ProgVariableTypes.Gender:
				var targetGender = (Gender?)iResult?.GetObject;
				Result = new BooleanVariable(((IList)collectionFunction.Result.GetObject).OfType<IProgVariable>()
					.Any(x => x.GetObject as Gender? == targetGender));
				break;
			case ProgVariableTypes.DateTime:
				var targetDateTime = (System.DateTime?)iResult?.GetObject;
				Result = new BooleanVariable(((IList)collectionFunction.Result.GetObject).OfType<IProgVariable>()
					.Any(x => (x.GetObject as System.DateTime?)?.Equals(targetDateTime) ?? false));
				break;
			case ProgVariableTypes.TimeSpan:
				var targetTimeSpan = (TimeSpan?)iResult?.GetObject;
				Result = new BooleanVariable(((IList)collectionFunction.Result.GetObject).OfType<IProgVariable>()
					.Any(x => (x.GetObject as TimeSpan?)?.Equals(targetTimeSpan) ?? false));
				break;
			case ProgVariableTypes.MudDateTime:
				var targetMudDateTime = (MudDateTime)iResult?.GetObject;
				Result = new BooleanVariable(((IList)collectionFunction.Result.GetObject).OfType<IProgVariable>()
					.Any(x => (x.GetObject as MudDateTime)?.Equals(targetMudDateTime) ?? false));
				break;
			default:
				Result = new BooleanVariable(((IList)collectionFunction.Result.GetObject).OfType<IProgVariable>()
					.Any(x => x.GetObject == iResult?.GetObject));
				break;
		}

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"contains",
				new[] { ProgVariableTypes.Collection, ProgVariableTypes.CollectionItem },
				(pars, gameworld) => new ContainsFunction(pars),
				new List<string> { "collection", "item" },
				new List<string>
					{ "The collection you want to check", "The item you are looking for in the collection" },
				"This function allows you to test whether a given item is in a collection. True if the collection contains the item.",
				"Collections",
				ProgVariableTypes.Boolean
			)
		);
	}
}