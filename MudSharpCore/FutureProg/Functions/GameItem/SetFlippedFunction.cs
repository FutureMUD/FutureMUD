using System.Collections.Generic;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SetFlippedFunction : BuiltInFunction
{
	private SetFlippedFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result is not IGameItem itemFunction)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var flippable = itemFunction.GetItemType<IFlip>();
		if (flippable != null)
		{
			var targetState = (bool?)ParameterFunctions[1].Result.GetObject ?? true;
			var echo = (bool?)ParameterFunctions[2].Result.GetObject ?? true;
			if (flippable.Flipped == targetState)
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			flippable.Flipped = targetState;
			Result = new BooleanVariable(true);
			return StatementResult.Normal;
		}

		Result = new BooleanVariable(false);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"setflipped",
			new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Boolean },
			(pars, gameworld) => new SetFlippedFunction(pars),
			new List<string> { "item", "flipped" },
			new List<string>
			{
				"The item that you want to flip or unflip", "A boolean representing whether this item is flipped or not"
			},
			"If an item is a flippable (e.g. a table), this command sets the flipped state according to an argument you pass in. Returns true if the item was changed, false if it was already in the desired state or wasn't a flippable.",
			"Items",
			FutureProgVariableTypes.Boolean
		));
	}
}