using MudSharp.Construction;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Location;

internal class CreateRoom : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "CreateCell".ToLowerInvariant(),
                new[] { ProgVariableTypes.OverlayPackage, ProgVariableTypes.Zone },
                (pars, gameworld) => new CreateRoom(pars, gameworld),
                new List<string>
                {
                    "package",
                    "zone",
                },
                new List<string>
                {
                    "The package that you want to approve",
                    "The zone to create the room in",
                },
                "Creates a new blank room in the specified package and zone",
                "Rooms",
                ProgVariableTypes.Location
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "CreateCell".ToLowerInvariant(),
                new[]
                {
                    ProgVariableTypes.OverlayPackage, ProgVariableTypes.Zone,
                    ProgVariableTypes.Location
                },
                (pars, gameworld) => new CreateRoom(pars, gameworld),
                new List<string>
                {
                    "package",
                    "zone",
                    "template"
                },
                new List<string>
                {
                    "The package that you want to approve",
                    "The zone to create the room in",
                    "A room to copy as the basic template of the new room"
                },
                "Creates a new room based on the template room in the specified package and zone",
                "Rooms",
                ProgVariableTypes.Location
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "CreateRoom".ToLowerInvariant(),
                new[] { ProgVariableTypes.OverlayPackage, ProgVariableTypes.Zone },
                (pars, gameworld) => new CreateRoom(pars, gameworld),
                new List<string>
                {
                    "package",
                    "zone",
                },
                new List<string>
                {
                    "The package that you want to approve",
                    "The zone to create the room in",
                },
                "Creates a new blank room in the specified package and zone. Alias for CreateCell.",
                "Rooms",
                ProgVariableTypes.Location
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "CreateRoom".ToLowerInvariant(),
                new[]
                {
                    ProgVariableTypes.OverlayPackage, ProgVariableTypes.Zone,
                    ProgVariableTypes.Location
                },
                (pars, gameworld) => new CreateRoom(pars, gameworld),
                new List<string>
                {
                    "package",
                    "zone",
                    "template"
                },
                new List<string>
                {
                    "The package that you want to approve",
                    "The zone to create the room in",
                    "A room to copy as the basic template of the new room"
                },
                "Creates a new room based on the template room in the specified package and zone. Alias for CreateCell.",
                "Rooms",
                ProgVariableTypes.Location
            )
        );
    }

    #endregion

    #region Constructors

    protected CreateRoom(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
    {
        Gameworld = gameworld;
    }

    #endregion

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Location;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        IRoomOverlayPackage package = (IRoomOverlayPackage)ParameterFunctions[0].Result?.GetObject;
        if (package == null)
        {
            Result = null;
            return StatementResult.Normal;
        }

        IZone zone = (IZone)ParameterFunctions[1].Result?.GetObject;
        if (zone == null)
        {
            Result = null;
            return StatementResult.Normal;
        }

        if (package.Status != RevisionStatus.UnderDesign && package.Status != RevisionStatus.PendingRevision)
        {
            Result = null;
            return StatementResult.Normal;
        }

        IRoom room = ParameterFunctions.Count == 3 ? (IRoom)ParameterFunctions[2].Result?.GetObject : default;
        Result = room is null ? new Room(package, zone) : new Room(package, zone, room, false);
        return StatementResult.Normal;
    }
}
