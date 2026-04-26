using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Currency;

internal class CountCurrencyFunction : BuiltInFunction
{
    public CountCurrencyFunction(IList<IFunction> parameters, bool respectGetRules)
        : base(parameters)
    {
        RespectGetRules = respectGetRules;
    }

    public bool RespectGetRules { get; set; }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Number;
        protected set { }
    }

    private static decimal CountItem(IGameItem item, ICurrency whichCurrency, bool respectGetRules)
    {
        ICurrencyPile currency = item.GetItemType<ICurrencyPile>();
        if (currency != null)
        {
            if (currency.Currency == whichCurrency)
            {
                return currency.Coins.Sum(x => x.Item2 * x.Item1.Value);
            }
        }

        if (respectGetRules && item.IsItemType<IOpenable>() && !item.GetItemType<IOpenable>().IsOpen)
        {
            return 0.0M;
        }

        IContainer container = item.GetItemType<IContainer>();
        return container?.Contents.Sum(contained => CountItem(contained, whichCurrency, respectGetRules)) ?? 0.0M;
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        IProgVariable parameter1 = ParameterFunctions[0].Result;
        if (parameter1 == null)
        {
            ErrorMessage = "Null Character, Location or Item given to CountCurrency function.";
            return StatementResult.Error;
        }

        ICurrency currency = (ICurrency)ParameterFunctions[1].Result;
        if (currency == null)
        {
            ErrorMessage = "Currency was null in CountCurrency function.";
            return StatementResult.Error;
        }

        if (parameter1 is IGameItem item)
        {
            Result = new NumberVariable(CountItem(item, currency, RespectGetRules));
            return StatementResult.Normal;
        }

        if (parameter1 is ICell location)
        {
            Result = new NumberVariable(location.GameItems.Sum(x => CountItem(x, currency, RespectGetRules)));
            return StatementResult.Normal;
        }

        if (parameter1 is ICharacter character)
        {
            Result = new NumberVariable(character.Body.ExternalItems.Sum(x => CountItem(x, currency, RespectGetRules)));
            return StatementResult.Normal;
        }

        throw new ApplicationException("Invalid target type in CountCurrency function.");
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "countcurrency",
                new[] { ProgVariableTypes.Character, ProgVariableTypes.Currency },
                (pars, gameworld) => new CountCurrencyFunction(pars, false),
                new List<string> { "character", "currency" },
                new List<string> { "The character whose worn, held, and carried items should be searched.", "The currency definition whose coins should be counted." },
                "Counts all coins of a currency found on a character, inside an item, or in a room, recursively searching containers without applying get-access rules. Errors if the target or currency is null.",
                "Currency",
                ProgVariableTypes.Number
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "countcurrency",
                new[] { ProgVariableTypes.Item, ProgVariableTypes.Currency },
                (pars, gameworld) => new CountCurrencyFunction(pars, false),
                new List<string> { "item", "currency" },
                new List<string> { "The item to search, including nested container contents.", "The currency definition whose coins should be counted." },
                "Counts all coins of a currency found on a character, inside an item, or in a room, recursively searching containers without applying get-access rules. Errors if the target or currency is null.",
                "Currency",
                ProgVariableTypes.Number
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "countcurrency",
                new[] { ProgVariableTypes.Location, ProgVariableTypes.Currency },
                (pars, gameworld) => new CountCurrencyFunction(pars, false),
                new List<string> { "location", "currency" },
                new List<string> { "The room whose loose items should be searched.", "The currency definition whose coins should be counted." },
                "Counts all coins of a currency found on a character, inside an item, or in a room, recursively searching containers without applying get-access rules. Errors if the target or currency is null.",
                "Currency",
                ProgVariableTypes.Number
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "countaccessiblecurrency",
                new[] { ProgVariableTypes.Character, ProgVariableTypes.Currency },
                (pars, gameworld) => new CountCurrencyFunction(pars, true),
                new List<string> { "character", "currency" },
                new List<string> { "The character whose accessible worn, held, and carried items should be searched.", "The currency definition whose coins should be counted." },
                "Counts coins of a currency found on a character, inside an item, or in a room while respecting normal get-access limits such as closed containers. Errors if the target or currency is null.",
                "Currency",
                ProgVariableTypes.Number
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "countaccessiblecurrency",
                new[] { ProgVariableTypes.Item, ProgVariableTypes.Currency },
                (pars, gameworld) => new CountCurrencyFunction(pars, true),
                new List<string> { "item", "currency" },
                new List<string> { "The item to search, skipping closed containers that normal get rules would block.", "The currency definition whose coins should be counted." },
                "Counts coins of a currency found on a character, inside an item, or in a room while respecting normal get-access limits such as closed containers. Errors if the target or currency is null.",
                "Currency",
                ProgVariableTypes.Number
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "countaccessiblecurrency",
                new[] { ProgVariableTypes.Location, ProgVariableTypes.Currency },
                (pars, gameworld) => new CountCurrencyFunction(pars, true),
                new List<string> { "location", "currency" },
                new List<string> { "The room whose accessible loose items should be searched.", "The currency definition whose coins should be counted." },
                "Counts coins of a currency found on a character, inside an item, or in a room while respecting normal get-access limits such as closed containers. Errors if the target or currency is null.",
                "Currency",
                ProgVariableTypes.Number
            )
        );
    }
}
