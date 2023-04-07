using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SetLiquidFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private SetLiquidFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
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

		var liquidcontainer = itemFunction.GetItemType<ILiquidContainer>();
		if (liquidcontainer == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var liquid = _gameworld.Liquids.Get((long)((decimal?)ParameterFunctions[1].Result.GetObject ?? 0));
		if (liquid == null)
		{
			liquidcontainer.ReduceLiquidQuantity(liquidcontainer.LiquidCapacity, null, "futurescript");
			Result = new BooleanVariable(true);
			return StatementResult.Normal;
		}

		liquidcontainer.LiquidMixture =
			new Form.Material.LiquidMixture(liquid, liquidcontainer.LiquidCapacity, liquidcontainer.Gameworld);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"setliquid",
			new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Number },
			(pars, gameworld) => new SetLiquidFunction(pars, gameworld)
		));
	}
}