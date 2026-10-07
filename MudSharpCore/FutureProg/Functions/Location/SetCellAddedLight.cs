using MudSharp.Construction;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Location;

internal class SetRoomAddedLight : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "SetCellAddedLight".ToLowerInvariant(),
                new[]
                {
                    ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage,
                    ProgVariableTypes.Number
                },
                (pars, gameworld) => new SetRoomAddedLight(pars, gameworld),
                new List<string>
                {
                    "room",
                    "package",
                    "light",
                },
                new List<string>
                {
                    "The room you want to edit",
                    "The package that the edit belongs to",
                    "The added light in lux",
                },
                "Sets the added light level of a room as if you had done CELL SET LIGHTLEVEL.",
                "Rooms",
                ProgVariableTypes.Boolean
            )
        );
    }

    #endregion

    #region Constructors

    protected SetRoomAddedLight(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
    {
        Gameworld = gameworld;
    }

    #endregion

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

        double addition = Convert.ToDouble(ParameterFunctions[2].Result?.GetObject ?? 0.0);
        if (package.Status != RevisionStatus.UnderDesign && package.Status != RevisionStatus.PendingRevision)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        IEditableRoomOverlay overlay = room.GetOrCreateOverlay(package);
        overlay.AddedLight = addition;

        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }
}