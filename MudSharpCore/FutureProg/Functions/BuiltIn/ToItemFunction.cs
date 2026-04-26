using MudSharp.GameItems;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToItemFunction : BuiltInFunction
{
    protected ToItemFunction(IList<IFunction> parameters)
        : base(parameters)
    {
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Item;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        Result = ParameterFunctions[0].Result as IGameItem;
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "toitem",
            new[] { ProgVariableTypes.Perceiver },
            (pars, gameworld) => new ToItemFunction(pars),
            new List<string> { "value" },
            new List<string> { "The value to convert or inspect." },
            "Attempts to treat a perceiver, perceivable, or collection item as an item. Returns null when the supplied value is not an item.",
            "Built-In",
            ProgVariableTypes.Item
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "toitem",
            new[] { ProgVariableTypes.Perceivable },
            (pars, gameworld) => new ToItemFunction(pars),
            new List<string> { "value" },
            new List<string> { "The value to convert or inspect." },
            "Attempts to treat a perceiver, perceivable, or collection item as an item. Returns null when the supplied value is not an item.",
            "Built-In",
            ProgVariableTypes.Item
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "toitem",
            new[] { ProgVariableTypes.CollectionItem },
            (pars, gameworld) => new ToItemFunction(pars),
            new List<string> { "value" },
            new List<string> { "The value to convert or inspect." },
            "Attempts to treat a perceiver, perceivable, or collection item as an item. Returns null when the supplied value is not an item.",
            "Built-In",
            ProgVariableTypes.Item
        ));
    }
}