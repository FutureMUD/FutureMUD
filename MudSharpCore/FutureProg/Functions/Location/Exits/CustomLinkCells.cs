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

internal class CustomLinkCells : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "CustomLinkCells".ToLowerInvariant(),
                new[]
                {
                    ProgVariableTypes.Location, ProgVariableTypes.Location,
                    ProgVariableTypes.OverlayPackage, ProgVariableTypes.Number,
                    ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text,
                    ProgVariableTypes.Text
                },
                (pars, gameworld) => new CustomLinkCells(pars, gameworld),
                new List<string> { "origin", "destination", "overlay", "template", "outboundKeyword", "outboundTarget", "inboundKeyword", "inboundTarget" },
                new List<string> { "The origin room for the new exit.", "The destination room for the new exit.", "The editable overlay package where the exit should be created or copied.", "The non-cardinal exit template ID to use.", "The primary keyword for the origin-to-destination side.", "The target/sdesc displayed for the origin-to-destination side.", "The primary keyword for the destination-to-origin side.", "The target/sdesc displayed for the destination-to-origin side." },
                "Creates a custom non-cardinal exit between two rooms in an overlay package, using explicit keywords and targets for both directions. This is equivalent to creating a non-cardinal builder exit from a template. Returns the created exit or null if the rooms, editable package, template, or target text are invalid, or if a conflicting exit already exists.",
                "Rooms",
                ProgVariableTypes.Exit
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "CustomLinkCells".ToLowerInvariant(),
                new[]
                {
                    ProgVariableTypes.Location, ProgVariableTypes.Location,
                    ProgVariableTypes.OverlayPackage, ProgVariableTypes.Text, ProgVariableTypes.Text,
                    ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text
                },
                (pars, gameworld) => new CustomLinkCells(pars, gameworld),
                new List<string> { "origin", "destination", "overlay", "template", "outboundKeyword", "outboundTarget", "inboundKeyword", "inboundTarget" },
                new List<string> { "The origin room for the new exit.", "The destination room for the new exit.", "The editable overlay package where the exit should be created or copied.", "The non-cardinal exit template name to use.", "The primary keyword for the origin-to-destination side.", "The target/sdesc displayed for the origin-to-destination side.", "The primary keyword for the destination-to-origin side.", "The target/sdesc displayed for the destination-to-origin side." },
                "Creates a custom non-cardinal exit between two rooms in an overlay package, using explicit keywords and targets for both directions. This is equivalent to creating a non-cardinal builder exit from a template. Returns the created exit or null if the rooms, editable package, template, or target text are invalid, or if a conflicting exit already exists.",
                "Rooms",
                ProgVariableTypes.Exit
            )
        );
    }

    #endregion

    #region Constructors

    protected CustomLinkCells(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

        if (origin == destination)
        {
            Result = null;
            return StatementResult.Normal;
        }

        INonCardinalExitTemplate template;
        if (ParameterFunctions[3].ReturnType.CompatibleWith(ProgVariableTypes.Number))
        {
            template = Gameworld.NonCardinalExitTemplates.Get(
                Convert.ToInt64(ParameterFunctions[3].Result?.GetObject ?? 0));
        }
        else
        {
            template = Gameworld.NonCardinalExitTemplates.GetByName(
                ParameterFunctions[3].Result?.GetObject?.ToString() ?? string.Empty);
        }

        if (template == null)
        {
            Result = null;
            return StatementResult.Normal;
        }

        string outboundkey = ParameterFunctions[4].Result?.GetObject?.ToString() ?? string.Empty;
        string outboundName = ParameterFunctions[5].Result?.GetObject?.ToString() ?? string.Empty;
        string inboundKey = ParameterFunctions[6].Result?.GetObject?.ToString() ?? string.Empty;
        string inboundName = ParameterFunctions[7].Result?.GetObject?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(outboundkey) || string.IsNullOrWhiteSpace(inboundKey) ||
            string.IsNullOrWhiteSpace(outboundName) || string.IsNullOrWhiteSpace(inboundName))
        {
            Result = null;
            return StatementResult.Normal;
        }

        IEditableCellOverlay overlay = origin.GetOrCreateOverlay(package);
        if (Gameworld.ExitManager.GetExitsFor(origin, overlay).Any(x => x.Destination == destination))
        {
            Result = null;
            return StatementResult.Normal;
        }

        IEditableCellOverlay otherOverlay = destination.GetOrCreateOverlay(package);
        if (Gameworld.ExitManager.GetExitsFor(destination, otherOverlay)
                     .Any(x => x.Destination == origin))
        {
            Result = null;
            return StatementResult.Normal;
        }

        Exit newExit = new(Gameworld, origin, destination, 1.0, template, outboundkey, inboundKey, outboundName,
            inboundName);
        overlay.AddExit(newExit);
        otherOverlay.AddExit(newExit);
        Gameworld.ExitManager.UpdateCellOverlayExits(origin, overlay);
        Gameworld.ExitManager.UpdateCellOverlayExits(destination, otherOverlay);
        Result = newExit.CellExitFor(origin);
        return StatementResult.Normal;
    }
}
