using MudSharp.Community;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToClanPaygradeFunction : BuiltInFunction
{
    private readonly IFuturemud _gameworld;

    protected ToClanPaygradeFunction(IList<IFunction> parameters, IFuturemud gameworld)
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

        if (ParameterFunctions.Any(x => x.Result?.GetObject is null))
        {
            Result = new NullVariable(ReturnType);
            return StatementResult.Normal;
        }

        if (ParameterFunctions.Count == 1)
        {
            Result = _gameworld.Clans.SelectMany(x => x.Paygrades).Get((long)(decimal)(ParameterFunctions[0].Result?.GetObject ?? 0.0M)) ?? (IProgVariable)new NullVariable(ReturnType);
            return StatementResult.Normal;
        }

        IClan clan = ParameterFunctions[0].Result?.GetObject as IClan;
        if (clan is null)
        {
            Result = new NullVariable(ProgVariableTypes.ClanPaygrade);
            return StatementResult.Normal;
        }

        if (ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Number))
        {
            Result = clan.Paygrades.Get((long)(decimal)(ParameterFunctions[1].Result?.GetObject ?? 0.0M)) ??
                     (IProgVariable)new NullVariable(ReturnType);
            return StatementResult.Normal;
        }

        string text = ParameterFunctions[1].Result?.GetObject?.ToString();
        if (text is null)
        {
            Result = new NullVariable(ProgVariableTypes.ClanPaygrade);
            return StatementResult.Normal;
        }

        Result = clan.Paygrades.FirstOrDefault(x => x.Name.EqualTo(text)) ??
            clan.Paygrades.FirstOrDefault(x => x.Abbreviation.EqualTo(text)) ?? (IProgVariable)new NullVariable(ReturnType);

        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "topaygrade",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new ToClanPaygradeFunction(pars, gameworld),
            new List<string> { "id" },
            new List<string> { "The ID to look up" },
            "Converts an ID number into the specified type, if one exists",
            "Lookup",
            ProgVariableTypes.ClanPaygrade
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "topaygrade",
            new[] { ProgVariableTypes.Clan, ProgVariableTypes.Text },
            (pars, gameworld) => new ToClanPaygradeFunction(pars, gameworld),
            new List<string> { "clan", "name" },
            new List<string> { "The clan in which you want to search", "The name to look up" },
            "Converts a name into the specified type, if one exists",
            "Lookup",
            ProgVariableTypes.ClanPaygrade
        ));
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "topaygrade", [ProgVariableTypes.Clan, ProgVariableTypes.Number],
            (pars, world) => new ToClanPaygradeFunction(pars, world),
            ["clan", "id"], ["The clan to search within.", "The paygrade ID."],
            "Returns a paygrade by ID within a clan, or null.", "Lookup", ProgVariableTypes.ClanPaygrade));

    }
}