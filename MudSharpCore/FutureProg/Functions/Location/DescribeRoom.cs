using MudSharp.Construction;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Location;

internal class DescribeRoom : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "DescribeCell".ToLowerInvariant(),
                new[]
                {
                    ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage,
                    ProgVariableTypes.Text
                },
                (pars, gameworld) => new DescribeRoom(pars, gameworld),
                new List<string>
                {
                    "room",
                    "package",
                    "description",
                },
                new List<string>
                {
                    "The room you want to describe",
                    "The package that the description change belongs to",
                    "The description that you are setting for that room",
                },
                "Sets the description of a room as if you had done CELL SET DESCRIPTION.",
                "Rooms",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "DescribeRoom".ToLowerInvariant(),
                new[]
                {
                    ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage,
                    ProgVariableTypes.Text
                },
                (pars, gameworld) => new DescribeRoom(pars, gameworld),
                new List<string>
                {
                    "room",
                    "package",
                    "description",
                },
                new List<string>
                {
                    "The room you want to describe",
                    "The package that the description change belongs to",
                    "The description that you are setting for that room",
                },
                "Sets the description of a room as if you had done CELL SET DESCRIPTION. Alias for DescribeCell.",
                "Rooms",
                ProgVariableTypes.Boolean
            )
        );
    }

    #endregion

    #region Constructors

    protected DescribeRoom(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

        string text = ParameterFunctions[2].Result?.GetObject?.ToString();
        if (string.IsNullOrWhiteSpace(text))
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
        overlay.RoomDescription = text;
        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }
}