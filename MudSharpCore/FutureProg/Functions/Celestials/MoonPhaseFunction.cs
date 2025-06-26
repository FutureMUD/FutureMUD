using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.FutureProg.Variables;

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

        var obj = ParameterFunctions.First().Result?.GetObject;
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

        var moon = zone.Celestials.OfType<PlanetaryMoon>().FirstOrDefault();
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
            (pars, gameworld) => new MoonPhaseFunction(pars)
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "moonphase",
            new[] { ProgVariableTypes.Zone },
            (pars, gameworld) => new MoonPhaseFunction(pars)
        ));
    }
}
