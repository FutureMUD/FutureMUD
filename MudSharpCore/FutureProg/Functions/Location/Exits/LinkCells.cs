using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.FutureProg.Functions.Location.Exits;

internal class LinkCells : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "LinkCells".ToLowerInvariant(),
                new[]
                {
                    ProgVariableTypes.Location, ProgVariableTypes.Location,
                    ProgVariableTypes.OverlayPackage, ProgVariableTypes.Number
                },
                (pars, gameworld) => new LinkCells(pars, gameworld)
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "LinkCells".ToLowerInvariant(),
                new[]
                {
                    ProgVariableTypes.Location, ProgVariableTypes.Location,
                    ProgVariableTypes.OverlayPackage, ProgVariableTypes.Text
                },
                (pars, gameworld) => new LinkCells(pars, gameworld)
            )
        );
    }

    #endregion

    #region Constructors

    protected LinkCells(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
    {
        Gameworld = gameworld;
    }

    #endregion

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Exit;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        ICell origin = (ICell)ParameterFunctions[0].Result?.GetObject;
        if (origin == null)
        {
            Result = null;
            return StatementResult.Normal;
        }

        ICell destination = (ICell)ParameterFunctions[1].Result?.GetObject;
        if (destination == null)
        {
            Result = null;
            return StatementResult.Normal;
        }

        ICellOverlayPackage package = (ICellOverlayPackage)ParameterFunctions[2].Result?.GetObject;
        if (package == null)
        {
            Result = null;
            return StatementResult.Normal;
        }

        if (package.Status != RevisionStatus.UnderDesign && package.Status != RevisionStatus.PendingRevision)
        {
            Result = null;
            return StatementResult.Normal;
        }

        CardinalDirection direction;
        if (ParameterFunctions[3].ReturnType.CompatibleWith(ProgVariableTypes.Number))
        {
            direction = (CardinalDirection)Convert.ToInt32(
                ParameterFunctions[3].Result?.GetObject ?? (int)CardinalDirection.Unknown);
            if (!Enum.IsDefined(typeof(CardinalDirection), direction))
            {
                Result = null;
                return StatementResult.Normal;
            }
        }
        else
        {
            if (!Constants.CardinalDirectionStringToDirection.TryGetValue(
                    ParameterFunctions[3].Result?.GetObject?.ToString() ?? "unknown", out direction))
            {
                Result = null;
                return StatementResult.Normal;
            }
        }

        if (origin == destination)
        {
            Result = null;
            return StatementResult.Normal;
        }

        IEditableCellOverlay overlay = origin.GetOrCreateOverlay(package);
        if (Gameworld.ExitManager.GetExitsFor(origin, overlay)
                     .Any(x => x.OutboundDirection == direction || x.Destination == destination))
        {
            Result = null;
            return StatementResult.Normal;
        }

        IEditableCellOverlay otherOverlay = destination.GetOrCreateOverlay(package);
        CardinalDirection oppositeDirection = direction.Opposite();
        if (Gameworld.ExitManager.GetExitsFor(destination, otherOverlay)
                     .Any(x => x.OutboundDirection == oppositeDirection || x.Destination == origin))
        {
            Result = null;
            return StatementResult.Normal;
        }

        Exit newExit = new(Gameworld, origin, destination, direction, oppositeDirection, 1.0);
        overlay.AddExit(newExit);
        otherOverlay.AddExit(newExit);
        Gameworld.ExitManager.UpdateCellOverlayExits(origin, overlay);
        Gameworld.ExitManager.UpdateCellOverlayExits(destination, otherOverlay);

        Result = newExit.CellExitFor(origin);
        return StatementResult.Normal;
    }
}