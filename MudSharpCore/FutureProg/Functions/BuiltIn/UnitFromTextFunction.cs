using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg.Variables;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class UnitFromTextFunction : BuiltInFunction, IHaveFuturemud
{
    protected UnitType Type;

    public UnitFromTextFunction(IList<IFunction> parameters, IFuturemud gameworld, UnitType type)
        : base(parameters)
    {
        Gameworld = gameworld;
        Type = type;
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Number;
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

        double result = Gameworld.UnitManager.GetBaseUnits((string)ParameterFunctions.First().Result.GetObject, Type,
            out bool success);
        if (!success)
        {
            ErrorMessage = "The text " + (string)ParameterFunctions.First().Result.GetObject +
                           " is not a valid expression unit expression.";
            return StatementResult.Error;
        }

        Result = new NumberVariable(result);
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "lengthfromtext",
            new[] { ProgVariableTypes.Text },
            (pars, gameworld) => new UnitFromTextFunction(pars, gameworld, UnitType.Length),
            new List<string> { "text" },
            new List<string> { "The text to parse." },
            "Parses length text into base units using the unit manager. Returns an error if the text is not a valid unit expression.",
            "Built-In",
            ProgVariableTypes.Number
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "massfromtext",
            new[] { ProgVariableTypes.Text },
            (pars, gameworld) => new UnitFromTextFunction(pars, gameworld, UnitType.Mass),
            new List<string> { "text" },
            new List<string> { "The text to parse." },
            "Parses mass text into base units using the unit manager. Returns an error if the text is not a valid unit expression.",
            "Built-In",
            ProgVariableTypes.Number
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "fluidfromtext",
            new[] { ProgVariableTypes.Text },
            (pars, gameworld) => new UnitFromTextFunction(pars, gameworld, UnitType.FluidVolume),
            new List<string> { "text" },
            new List<string> { "The text to parse." },
            "Parses fluid volume text into base units using the unit manager. Returns an error if the text is not a valid unit expression.",
            "Built-In",
            ProgVariableTypes.Number
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "areafromtext",
            new[] { ProgVariableTypes.Text },
            (pars, gameworld) => new UnitFromTextFunction(pars, gameworld, UnitType.Area),
            new List<string> { "text" },
            new List<string> { "The text to parse." },
            "Parses area text into base units using the unit manager. Returns an error if the text is not a valid unit expression.",
            "Built-In",
            ProgVariableTypes.Number
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "volumefromtext",
            new[] { ProgVariableTypes.Text },
            (pars, gameworld) => new UnitFromTextFunction(pars, gameworld, UnitType.Volume),
            new List<string> { "text" },
            new List<string> { "The text to parse." },
            "Parses volume text into base units using the unit manager. Returns an error if the text is not a valid unit expression.",
            "Built-In",
            ProgVariableTypes.Number
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "tempfromtext",
            new[] { ProgVariableTypes.Text },
            (pars, gameworld) => new UnitFromTextFunction(pars, gameworld, UnitType.Temperature),
            new List<string> { "text" },
            new List<string> { "The text to parse." },
            "Parses temperature text into base units using the unit manager. Returns an error if the text is not a valid unit expression.",
            "Built-In",
            ProgVariableTypes.Number
        ));
    }
}