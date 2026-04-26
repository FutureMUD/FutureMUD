using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
#nullable disable warnings

namespace MudSharp.FutureProg.Functions.Celestials;

internal class MoonPhaseFunction : BuiltInFunction
{
    public MoonPhaseFunction(IList<IFunction> parameters) : base(parameters) { }

    public override ProgVariableTypes ReturnType => ProgVariableTypes.Text;
    public override string ErrorMessage => ParameterFunctions.First().ErrorMessage;

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        object obj = ParameterFunctions.First().Result?.GetObject;
        IZone? zone = obj as IZone;
        if (zone == null && obj is ICell cell)
        {
            zone = cell.Zone;
        }

        if (zone == null)
        {
            Result = new TextVariable(string.Empty);
            return StatementResult.Normal;
        }

        PlanetaryMoon moon = zone.Celestials.OfType<PlanetaryMoon>().FirstOrDefault();
        if (moon == null)
        {
            Result = new TextVariable(string.Empty);
            return StatementResult.Normal;
        }

        Result = new TextVariable(moon.CurrentPhase().Describe());
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "moonphase",
            new[] { ProgVariableTypes.Location },
            (pars, gameworld) => new MoonPhaseFunction(pars),
            new List<string> { "location" },
            new List<string> { "The room whose zone should be used to determine the current moon phase." },
            "Looks up the first planetary moon associated with the supplied room's zone or zone and returns its current phase text. Returns an empty string if the zone or moon cannot be found.",
            "Celestials",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "moonphase",
            new[] { ProgVariableTypes.Zone },
            (pars, gameworld) => new MoonPhaseFunction(pars),
            new List<string> { "zone" },
            new List<string> { "The zone to use when determining the current moon phase." },
            "Looks up the first planetary moon associated with the supplied room's zone or zone and returns its current phase text. Returns an empty string if the zone or moon cannot be found.",
            "Celestials",
            ProgVariableTypes.Text
        ));
    }
}
