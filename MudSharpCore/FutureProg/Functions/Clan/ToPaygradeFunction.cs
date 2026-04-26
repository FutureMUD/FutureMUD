using MudSharp.Community;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Clan;

internal class ToPaygradeFunction : BuiltInFunction
{
    private readonly IFuturemud _gameworld;

    public ToPaygradeFunction(IList<IFunction> parameters, IFuturemud gameworld)
        : base(parameters)
    {
        _gameworld = gameworld;
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.ClanPaygrade;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        IClan clan = (IClan)ParameterFunctions.ElementAt(0).Result;
        if (clan == null)
        {
            ErrorMessage = "Clan Function in ToPaygrade returned null.";
            return StatementResult.Error;
        }

        Result = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Text)
            ? clan.Paygrades.FirstOrDefault(
                  x =>
                      x.Name.Equals((string)ParameterFunctions.ElementAt(1).Result.GetObject,
                          StringComparison.InvariantCultureIgnoreCase)) ??
              clan.Paygrades.FirstOrDefault(
                  x =>
                      x.Abbreviation.Equals((string)ParameterFunctions.ElementAt(1).Result.GetObject,
                          StringComparison.InvariantCultureIgnoreCase))
            : clan.Paygrades.FirstOrDefault(
                x => x.Id == (int)(decimal)ParameterFunctions.ElementAt(1).Result.GetObject);

        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "topaygrade",
            new[] { ProgVariableTypes.Clan, ProgVariableTypes.Number },
            (pars, gameworld) => new ToPaygradeFunction(pars, gameworld),
            new List<string> { "clan", "id" },
            new List<string> { "The clan whose paygrades should be searched.", "The numeric ID of the paygrade to find." },
            "Looks up a paygrade within a clan by ID, name, or abbreviation. Errors if the clan is null; returns null if no paygrade matches.",
            "Clans",
            ProgVariableTypes.ClanPaygrade
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "topaygrade",
            new[] { ProgVariableTypes.Clan, ProgVariableTypes.Text },
            (pars, gameworld) => new ToPaygradeFunction(pars, gameworld),
            new List<string> { "clan", "name" },
            new List<string> { "The clan whose paygrades should be searched.", "The paygrade name or abbreviation to find, matched case-insensitively." },
            "Looks up a paygrade within a clan by ID, name, or abbreviation. Errors if the clan is null; returns null if no paygrade matches.",
            "Clans",
            ProgVariableTypes.ClanPaygrade
        ));
    }
}
