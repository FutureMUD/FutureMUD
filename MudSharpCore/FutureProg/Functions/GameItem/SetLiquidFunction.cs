using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SetLiquidFunction : BuiltInFunction
{
    private readonly IFuturemud _gameworld;

    private SetLiquidFunction(IList<IFunction> parameters, IFuturemud gameworld)
        : base(parameters)
    {
        _gameworld = gameworld;
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

        ILiquidContainer liquidcontainer = itemFunction.GetItemType<ILiquidContainer>();
        if (liquidcontainer == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        ILiquid liquid = _gameworld.Liquids.Get((long)((decimal?)ParameterFunctions[1].Result.GetObject ?? 0));
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
            new[] { ProgVariableTypes.Item, ProgVariableTypes.Number },
            (pars, gameworld) => new SetLiquidFunction(pars, gameworld),
            new List<string> { "item", "liquidId" },
            new List<string> { "The liquid container item to fill or empty.", "The liquid ID to fill the container with. Use 0 or an invalid ID to empty the container." },
            "Sets a liquid container's contents to a full container of the selected liquid, or empties it if the ID is invalid. Returns false if the item is null or is not a liquid container.",
            "Items",
            ProgVariableTypes.Boolean
        ));
    }
}
