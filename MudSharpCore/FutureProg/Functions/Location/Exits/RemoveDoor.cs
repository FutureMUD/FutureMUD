using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Interfaces;
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

internal class RemoveDoor : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "RemoveDoor".ToLowerInvariant(),
                new[] { ProgVariableTypes.Exit },
                (pars, gameworld) => new RemoveDoor(pars, gameworld),
                new List<string> { "exit" },
                new List<string> { "The exit whose installed door should be removed." },
                "Removes the installed door from an exit and returns the door item. Returns null if the exit is null or has no installed door.",
                "Rooms",
                ProgVariableTypes.Item
            )
        );
    }

    #endregion

    #region Constructors

    protected RemoveDoor(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
    {
        Gameworld = gameworld;
    }

    #endregion

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Item;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        ICellExit exit = (ICellExit)ParameterFunctions[0].Result?.GetObject;
        if (exit == null)
        {
            Result = null;
            return StatementResult.Normal;
        }

        if (exit.Exit.Door == null)
        {
            Result = null;
            return StatementResult.Normal;
        }

        IDoor door = exit.Exit.Door;
        exit.Exit.Door = null;
        exit.Exit.Changed = true;
        door.OpenDirectionCell = null;
        door.HingeCell = null;
        door.State = DoorState.Uninstalled;
        door.InstalledExit = null;

        Result = door.Parent;
        return StatementResult.Normal;
    }
}
