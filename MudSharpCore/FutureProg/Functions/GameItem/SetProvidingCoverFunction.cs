using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;

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

        IProvideCover cover = itemFunction.GetItemType<IProvideCover>();
        if (cover != null)
        {
            bool targetState = (bool?)ParameterFunctions[1].Result.GetObject ?? true;
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
            (pars, gameworld) => new SetProvidingCoverFunction(pars),
            new List<string> { "item", "state" },
            new List<string> { "The item with a cover-providing component to update.", "Whether the item should actively provide cover after the call." },
            "Sets whether an item that can provide cover is actively providing cover. Returns false if the item is null, lacks the component, or was already in the requested state.",
            "Items",
            ProgVariableTypes.Boolean
        ));
    }
}
