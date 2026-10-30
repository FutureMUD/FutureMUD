using MudSharp.Form.Material;
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

        if (ParameterFunctions.Count == 3)
        {
            Result = new BooleanVariable(TrySetLiquidVolume(liquidcontainer));
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
            new LiquidMixture(liquid, liquidcontainer.LiquidCapacity, liquidcontainer.Gameworld);
        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }

    private bool TrySetLiquidVolume(ILiquidContainer container)
    {
        if (!TryGetNumber(ParameterFunctions[1].Result?.GetObject, out var liquidIdNumber) ||
            !System.Double.IsFinite(liquidIdNumber) || liquidIdNumber < 0.0 ||
            liquidIdNumber >= (double)long.MaxValue || System.Math.Truncate(liquidIdNumber) != liquidIdNumber ||
            !TryGetNumber(ParameterFunctions[2].Result?.GetObject, out var volume))
        {
            return false;
        }

        if (!System.Double.IsFinite(volume) || volume < 0.0 ||
            !System.Double.IsFinite(container.LiquidCapacity) || container.LiquidCapacity < 0.0 ||
            volume > container.LiquidCapacity)
        {
            return false;
        }

        var liquidId = (long)liquidIdNumber;
        if (liquidId == 0)
        {
            if (volume != 0.0) return false;
            container.LiquidMixture = null;
            return true;
        }

        var liquid = _gameworld.Liquids.Get(liquidId);
        if (liquid is null) return false;

        if (volume == 0.0)
        {
            container.LiquidMixture = null;
            return true;
        }

        container.LiquidMixture = new LiquidMixture(liquid, volume, _gameworld);
        return true;
    }

    private static bool TryGetNumber(object value, out double number)
    {
        switch (value)
        {
            case decimal decimalValue:
                number = (double)decimalValue;
                return true;
            case double doubleValue:
                number = doubleValue;
                return true;
            case float floatValue:
                number = floatValue;
                return true;
            case long longValue:
                number = longValue;
                return true;
            case int intValue:
                number = intValue;
                return true;
            default:
                number = 0.0;
                return false;
        }
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

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "setliquid",
            new[] { ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Number },
            (pars, gameworld) => new SetLiquidFunction(pars, gameworld),
            new List<string> { "item", "liquidId", "volume" },
            new List<string>
            {
                "The liquid container item to fill or empty.",
                "The liquid ID to set, or 0 to empty only when volume is also 0.",
                "The desired final volume in base fluid-volume units; 0 empties the container."
            },
            "Sets a liquid container to the requested final volume. Invalid liquid IDs and invalid, negative, non-finite or over-capacity volumes return false without changing the container.",
            "Items",
            ProgVariableTypes.Boolean
        ));
    }
}
