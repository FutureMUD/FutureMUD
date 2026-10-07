using MudSharp.Construction;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Location;

internal class SetIndoors : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "SetIndoors".ToLowerInvariant(),
                new[] { ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage },
                (pars, gameworld) => new SetIndoors(pars, gameworld, RoomOutdoorsType.Indoors),
                new List<string>
                {
                    "room",
                    "package",
                },
                new List<string>
                {
                    "The room you want to edit",
                    "The package that the edit belongs to"
                },
                "Sets the indoors-type of the room to 'Indoors', as if you had done CELL SET TYPE INDOORS.",
                "Rooms",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "SetIndoorsWithWindows".ToLowerInvariant(),
                new[] { ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage },
                (pars, gameworld) => new SetIndoors(pars, gameworld, RoomOutdoorsType.IndoorsWithWindows),
                new List<string>
                {
                    "room",
                    "package",
                },
                new List<string>
                {
                    "The room you want to edit",
                    "The package that the edit belongs to"
                },
                "Sets the indoors-type of the room to 'Indoors With Windows', as if you had done CELL SET TYPE WINDOWS.",
                "Rooms",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "SetIndoorsNoLight".ToLowerInvariant(),
                new[] { ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage },
                (pars, gameworld) => new SetIndoors(pars, gameworld, RoomOutdoorsType.IndoorsNoLight),
                new List<string>
                {
                    "room",
                    "package",
                },
                new List<string>
                {
                    "The room you want to edit",
                    "The package that the edit belongs to"
                },
                "Sets the indoors-type of the room to 'Indoors With No Light', as if you had done CELL SET TYPE CAVE.",
                "Rooms",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "SetIndoorsClimateExposed".ToLowerInvariant(),
                new[] { ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage },
                (pars, gameworld) => new SetIndoors(pars, gameworld, RoomOutdoorsType.IndoorsClimateExposed),
                new List<string>
                {
                    "room",
                    "package",
                },
                new List<string>
                {
                    "The room you want to edit",
                    "The package that the edit belongs to"
                },
                "Sets the indoors-type of the room to 'Indoors But Climate Exposed', as if you had done CELL SET TYPE EXPOSED.",
                "Rooms",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "SetOutdoors".ToLowerInvariant(),
                new[] { ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage },
                (pars, gameworld) => new SetIndoors(pars, gameworld, RoomOutdoorsType.Outdoors),
                new List<string>
                {
                    "room",
                    "package",
                },
                new List<string>
                {
                    "The room you want to edit",
                    "The package that the edit belongs to"
                },
                "Sets the indoors-type of the room to 'Outdoors', as if you had done CELL SET TYPE OUTDOORS.",
                "Rooms",
                ProgVariableTypes.Boolean
            )
        );
    }

    #endregion

    #region Constructors

    protected SetIndoors(IList<IFunction> parameterFunctions, IFuturemud gameworld, RoomOutdoorsType outdoorsType) :
        base(parameterFunctions)
    {
        Gameworld = gameworld;
        OutdoorsType = outdoorsType;
    }

    #endregion

    public RoomOutdoorsType OutdoorsType { get; set; }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Boolean;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        IRoom room = (IRoom)ParameterFunctions[0].Result?.GetObject;
        if (room == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        IRoomOverlayPackage package = (IRoomOverlayPackage)ParameterFunctions[1].Result?.GetObject;
        if (package == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        if (package.Status != RevisionStatus.UnderDesign && package.Status != RevisionStatus.PendingRevision)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        IEditableRoomOverlay overlay = room.GetOrCreateOverlay(package);
        overlay.OutdoorsType = OutdoorsType;
        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }
}