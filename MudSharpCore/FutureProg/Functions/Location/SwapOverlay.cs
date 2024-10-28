using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Location;

internal class SwapOverlay : BuiltInFunction
{
    private readonly IFuturemud _gameworld;
    private readonly bool _package;

    private SwapOverlay(IList<IFunction> parameters, IFuturemud gameworld, bool package)
        : base(parameters)
    {
        _gameworld = gameworld;
        _package = package;
    }

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

        ICellOverlayPackage package = null;

        if (ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text))
        {
            package = _gameworld.CellOverlayPackages.GetByName(
                ParameterFunctions[0].Result?.GetObject?.ToString() ?? "");
        }
        else if (ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.OverlayPackage))
        {
            package = (ICellOverlayPackage)ParameterFunctions[0].Result?.GetObject;
        }
        else
        {
            package = _gameworld.CellOverlayPackages.Get(Convert.ToInt64(ParameterFunctions[0].Result?.GetObject ?? 0));
        }

        if (package == null || package.Status != RevisionStatus.Current)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        if (_package)
        {
            foreach (var cell in _gameworld.Cells.Where(x => x.Overlays.Any(y => y.Package == package)).ToList())
            {
                cell.SetCurrentOverlay(package);
                _gameworld.ExitManager.UpdateCellOverlayExits(cell, cell.CurrentOverlay);
            }
        }
        else
        {
            var location = (ICell)ParameterFunctions[1].Result;
            if (location == null)
            {
                Result = new BooleanVariable(false);
                return StatementResult.Normal;
            }

            if (location.Overlays.All(x => x.Package != package))
            {
                Result = new BooleanVariable(false);
                return StatementResult.Normal;
            }

            location.SetCurrentOverlay(package);
            _gameworld.ExitManager.UpdateCellOverlayExits(location, location.CurrentOverlay);
        }

        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "swapoverlay",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new SwapOverlay(pars, gameworld, true),
            new List<string>
            {
                "overlay"
            },
            new List<string>
            {
                "The Id number of the overlay you want to swap"
            },
            "Swaps an approved overlay into the worldfile as if you had run the CELL SWAP command. Returns true if successful.",
            "Rooms",
            ProgVariableTypes.Boolean
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "swapoverlay",
            new[] { ProgVariableTypes.Text },
            (pars, gameworld) => new SwapOverlay(pars, gameworld, true),
            new List<string>
            {
                "overlay"
            },
            new List<string>
            {
                "The name of the overlay you want to swap"
            },
            "Swaps an approved overlay into the worldfile as if you had run the CELL SWAP command. Returns true if successful.",
            "Rooms",
            ProgVariableTypes.Boolean
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "swapoverlay",
            new[] { ProgVariableTypes.Number, ProgVariableTypes.Location },
            (pars, gameworld) => new SwapOverlay(pars, gameworld, false),
            new List<string>
            {
                "overlay",
                "room"
            },
            new List<string>
            {
                "The Id number of the overlay you want to swap",
                "The room you want to swap this package in for only"
            },
            "Swaps an approved overlay for one room only into the worldfile as if you had run the CELL SWAP command. Returns true if successful. This use case is only recommended for advanced scenarios - in most cases you should swap the entire overlay in at once",
            "Rooms",
            ProgVariableTypes.Boolean
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "swapoverlay",
            new[] { ProgVariableTypes.Text, ProgVariableTypes.Location },
            (pars, gameworld) => new SwapOverlay(pars, gameworld, false),
            new List<string>
            {
                "overlay",
                "room"
            },
            new List<string>
            {
                "The name of the overlay you want to swap",
                "The room you want to swap this package in for only"
            },
            "Swaps an approved overlay for one room only into the worldfile as if you had run the CELL SWAP command. Returns true if successful. This use case is only recommended for advanced scenarios - in most cases you should swap the entire overlay in at once",
            "Rooms",
            ProgVariableTypes.Boolean
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "swapoverlay",
            new[] { ProgVariableTypes.OverlayPackage },
            (pars, gameworld) => new SwapOverlay(pars, gameworld, true),
            new List<string>
            {
                "overlay"
            },
            new List<string>
            {
                "The overlay package that you want to swap"
            },
            "Swaps an approved overlay into the worldfile as if you had run the CELL SWAP command. Returns true if successful.",
            "Rooms",
            ProgVariableTypes.Boolean
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "swapoverlay",
            new[] { ProgVariableTypes.OverlayPackage, ProgVariableTypes.Location },
            (pars, gameworld) => new SwapOverlay(pars, gameworld, false),
            new List<string>
            {
                "overlay",
                "room"
            },
            new List<string>
            {
                "The overlay package that you want to swap",
                "The room you want to swap this package in for only"
            },
            "Swaps an approved overlay for one room only into the worldfile as if you had run the CELL SWAP command. Returns true if successful. This use case is only recommended for advanced scenarios - in most cases you should swap the entire overlay in at once",
            "Rooms",
            ProgVariableTypes.Boolean
        ));
    }
}