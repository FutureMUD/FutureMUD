using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.FutureProg.Variables;

#nullable enable
#nullable disable warnings

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

        object obj = ParameterFunctions[0].Result?.GetObject;
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

        var celestialParameter = ParameterFunctions[1].Result?.GetObject;
        ICelestialObject celestial = celestialParameter is ICelestialObject directCelestial
            ? zone.Celestials.FirstOrDefault(x => x.Id == directCelestial.Id)
            : zone.Celestials.FirstOrDefault(x => x.Id == Convert.ToInt64(celestialParameter ?? 0L));
        if (celestial == null)
        {
            Result = new TextVariable(string.Empty);
            return StatementResult.Normal;
        }

        CelestialInformation info = zone.GetInfo(celestial);
        Result = new TextVariable(celestial.Describe(info));
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "celestialposition",
            new[] { ProgVariableTypes.Location, ProgVariableTypes.Number },
            (pars, gameworld) => new CelestialPositionFunction(pars),
            new List<string> { "locationOrZone", "celestialId" },
            new List<string> { "The room or zone whose celestial collection is searched.", "The ID of the celestial object to describe." },
            "Looks up a celestial object by ID in the supplied room or zone and returns the same descriptive position text used by the celestial system. Returns an empty string if the zone or celestial object cannot be found.",
            "Celestials",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "celestialposition",
            new[] { ProgVariableTypes.Zone, ProgVariableTypes.Number },
            (pars, gameworld) => new CelestialPositionFunction(pars),
            new List<string> { "locationOrZone", "celestialId" },
            new List<string> { "The room or zone whose celestial collection is searched.", "The ID of the celestial object to describe." },
            "Looks up a celestial object by ID in the supplied room or zone and returns the same descriptive position text used by the celestial system. Returns an empty string if the zone or celestial object cannot be found.",
            "Celestials",
            ProgVariableTypes.Text
        ));

        RegisterTypedCelestial(ProgVariableTypes.Location);
        RegisterTypedCelestial(ProgVariableTypes.Zone);
    }

    private static void RegisterTypedCelestial(ProgVariableTypes locationType)
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "celestialposition",
            [locationType, ProgVariableTypes.CelestialObject],
            (pars, _) => new CelestialPositionFunction(pars),
            ["locationOrZone", "celestial"],
            ["The room or zone whose celestial collection is searched.", "The resolved celestial object to describe."],
            "Returns the same descriptive position text used by the celestial system. Returns an empty string if the zone or celestial object cannot be found.",
            "Celestials",
            ProgVariableTypes.Text));
    }
}
