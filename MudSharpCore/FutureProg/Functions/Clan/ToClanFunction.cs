using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Clan;

internal class ToClanFunction : BuiltInFunction
{
    private readonly IFuturemud _gameworld;

    public ToClanFunction(IList<IFunction> parameters, IFuturemud gameworld)
        : base(parameters)
    {
        _gameworld = gameworld;
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Clan;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        Result = ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text)
            ? _gameworld.Clans.Get((string)ParameterFunctions[0].Result.GetObject).FirstOrDefault()
            : _gameworld.Clans.Get((long)(decimal)ParameterFunctions[0].Result.GetObject);

        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "toclan",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new ToClanFunction(pars, gameworld),
            new List<string> { "id" },
            new List<string> { "The numeric ID of the clan to look up." },
            "Looks up a clan by ID or name. Returns null if no clan matches.",
            "Clans",
            ProgVariableTypes.Clan
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "toclan",
            new[] { ProgVariableTypes.Text },
            (pars, gameworld) => new ToClanFunction(pars, gameworld),
            new List<string> { "name" },
            new List<string> { "The clan name or alias to look up." },
            "Looks up a clan by ID or name. Returns null if no clan matches.",
            "Clans",
            ProgVariableTypes.Clan
        ));
    }
}
