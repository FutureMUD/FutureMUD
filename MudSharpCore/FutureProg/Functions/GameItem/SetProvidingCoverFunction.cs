using System.Collections.Generic;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SetProvidingCoverFunction : BuiltInFunction
{
	private SetProvidingCoverFunction(IList<IFunction> parameters)
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
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result is not IGameItem itemFunction)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var cover = itemFunction.GetItemType<IProvideCover>();
		if (cover != null)
		{
			var targetState = (bool?)ParameterFunctions[1].Result.GetObject ?? true;
			if (cover.IsProvidingCover == targetState)
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			cover.IsProvidingCover = targetState;
			Result = new BooleanVariable(true);
			return StatementResult.Normal;
		}

		Result = new BooleanVariable(false);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"setprovidingcover",
			new[] { ProgVariableTypes.Item, ProgVariableTypes.Boolean },
			(pars, gameworld) => new SetProvidingCoverFunction(pars)
		));
	}
}