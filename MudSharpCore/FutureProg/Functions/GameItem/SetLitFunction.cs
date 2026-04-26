using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SetLitFunction : BuiltInFunction
{
    private SetLitFunction(IList<IFunction> parameters)
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

        ILightable lightable = itemFunction.GetItemType<ILightable>();
        if (lightable == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        lightable.Lit = (bool?)ParameterFunctions[1].Result.GetObject ?? true;
        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "setlit",
            new[] { ProgVariableTypes.Item, ProgVariableTypes.Boolean },
            (pars, gameworld) => new SetLitFunction(pars),
            new List<string> { "item", "state" },
            new List<string> { "The lightable item to update.", "Whether the item should be lit after the call." },
            "Sets the lit state of a lightable item. Returns false if the item is null or does not have a lightable component.",
            "Items",
            ProgVariableTypes.Boolean
        ));
    }
}
