using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Location;

internal class ExitsForOverlay : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "ExitsForOverlay".ToLowerInvariant(),
                new[] { ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage },
                (pars, gameworld) => new ExitsForOverlay(pars, gameworld),
                new List<string>
                {
                    "room",
                    "package"
                },
                new List<string>
                {
                    "The room you want to get exits for",
                    "The package that you want exits for. If null, uses currently approved package."
                },
                "Returns a collection of the exits for a room, in the specified package.",
                "Rooms",
                ProgVariableTypes.Collection | ProgVariableTypes.Exit
            )
        );
    }

    #endregion

    #region Constructors

    protected ExitsForOverlay(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
    {
        Gameworld = gameworld;
    }

    #endregion

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Exit | ProgVariableTypes.Collection;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        IRoom room = (IRoom)ParameterFunctions[0].Result?.GetObject;
        if (room is null)
        {
            Result = new CollectionVariable(new List<IRoomExit>(), ProgVariableTypes.Exit);
            return StatementResult.Normal;
        }

        IRoomOverlayPackage package = (IRoomOverlayPackage)ParameterFunctions[1].Result?.GetObject;
        if (package is null)
        {
            package = room.CurrentOverlay.Package;
            if (package is null)
            {
                Result = new CollectionVariable(new List<IRoomExit>(), ProgVariableTypes.Exit);
                return StatementResult.Normal;
            }
        }

        Result = new CollectionVariable(Gameworld.ExitManager.GetExitsFor(room, package).ToList(),
            ProgVariableTypes.Exit);
        return StatementResult.Normal;
    }
}