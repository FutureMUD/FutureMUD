using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg.Variables;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class TextFromUnitFunction : BuiltInFunction, IHaveFuturemud
{
    protected UnitType Type;

    public TextFromUnitFunction(IList<IFunction> parameters, IFuturemud gameworld, UnitType type)
        : base(parameters)
    {
        Gameworld = gameworld;
        Type = type;
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Text;
        protected set { }
    }

    public override string ErrorMessage
    {
        get => ParameterFunctions.First().ErrorMessage;
        protected set { }
    }

    public IFuturemud Gameworld { get; protected set; }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        string result = Gameworld.UnitManager.Describe((double)(decimal)ParameterFunctions.First().Result.GetObject,
            Type, (string)ParameterFunctions.ElementAt(1).Result.GetObject);
        Result = new TextVariable(result);
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "textfromlength",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new TextFromUnitFunction(pars, gameworld, UnitType.Length),
            new List<string> { "amount" },
            new List<string> { "The amount to use. Text amounts are parsed using the target system's normal builder/player parsing rules." },
            "Formats a base-unit length value as player-facing text using the unit manager. This is the inverse of lengthfromtext for builder scripts that need readable units.",
            "Built-In",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "textfrommass",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new TextFromUnitFunction(pars, gameworld, UnitType.Mass),
            new List<string> { "amount" },
            new List<string> { "The amount to use. Text amounts are parsed using the target system's normal builder/player parsing rules." },
            "Formats a base-unit mass value as player-facing text using the unit manager. This is the inverse of massfromtext for builder scripts that need readable units.",
            "Built-In",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "textfromfluid",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new TextFromUnitFunction(pars, gameworld, UnitType.FluidVolume),
            new List<string> { "amount" },
            new List<string> { "The amount to use. Text amounts are parsed using the target system's normal builder/player parsing rules." },
            "Formats a base-unit fluid volume value as player-facing text using the unit manager. This is the inverse of fluid volumefromtext for builder scripts that need readable units.",
            "Built-In",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "textfromarea",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new TextFromUnitFunction(pars, gameworld, UnitType.Area),
            new List<string> { "amount" },
            new List<string> { "The amount to use. Text amounts are parsed using the target system's normal builder/player parsing rules." },
            "Formats a base-unit area value as player-facing text using the unit manager. This is the inverse of areafromtext for builder scripts that need readable units.",
            "Built-In",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "textfromvolume",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new TextFromUnitFunction(pars, gameworld, UnitType.Volume),
            new List<string> { "amount" },
            new List<string> { "The amount to use. Text amounts are parsed using the target system's normal builder/player parsing rules." },
            "Formats a base-unit volume value as player-facing text using the unit manager. This is the inverse of volumefromtext for builder scripts that need readable units.",
            "Built-In",
            ProgVariableTypes.Text
        ));
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "textfromtemp",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new TextFromUnitFunction(pars, gameworld, UnitType.Temperature),
            new List<string> { "amount" },
            new List<string> { "The amount to use. Text amounts are parsed using the target system's normal builder/player parsing rules." },
            "Formats a base-unit temperature value as player-facing text using the unit manager. This is the inverse of temperaturefromtext for builder scripts that need readable units.",
            "Built-In",
            ProgVariableTypes.Text
        ));
    }
}