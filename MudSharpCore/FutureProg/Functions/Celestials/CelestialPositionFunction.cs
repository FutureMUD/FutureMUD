using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Celestials;

internal class CelestialPositionFunction : BuiltInFunction
{
    public CelestialPositionFunction(IList<IFunction> parameters) : base(parameters) { }

    public override ProgVariableTypes ReturnType => ProgVariableTypes.Text;
    public override string ErrorMessage => ParameterFunctions.First().ErrorMessage;

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        var obj = ParameterFunctions[0].Result?.GetObject;
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

        var id = Convert.ToInt64(ParameterFunctions[1].Result?.GetObject ?? 0L);
        var celestial = zone.Celestials.FirstOrDefault(x => x.Id == id);
        if (celestial == null)
        {
            Result = new TextVariable(string.Empty);
            return StatementResult.Normal;
        }

        var info = zone.GetInfo(celestial);
        Result = new TextVariable(celestial.Describe(info));
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "celestialposition",
            new[] { ProgVariableTypes.Location, ProgVariableTypes.Number },
            (pars, gameworld) => new CelestialPositionFunction(pars)
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "celestialposition",
            new[] { ProgVariableTypes.Zone, ProgVariableTypes.Number },
            (pars, gameworld) => new CelestialPositionFunction(pars)
        ));
    }
}
