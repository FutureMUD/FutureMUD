using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
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

internal class InstallDoor : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "InstallDoor".ToLowerInvariant(),
                new[]
                {
                    ProgVariableTypes.Exit, ProgVariableTypes.Item, ProgVariableTypes.Location,
                    ProgVariableTypes.Location
                },
                (pars, gameworld) => new InstallDoor(pars, gameworld),
                new List<string> { "exit", "door", "origin", "destination" },
                new List<string> { "The exit that should receive the door.", "The item with a door component to install on the exit.", "The room on the hinge side; if null, the exit origin is used.", "The room on the opening side; if null, the exit destination is used." },
                "Installs a door item onto an exit between two rooms, uninstalling it from any previous exit first. Returns false if the exit is null, the item is null, the item is not a door, or the exit already has a door.",
                "Rooms",
                ProgVariableTypes.Boolean
            )
        );
    }

    #endregion

    #region Constructors

    protected InstallDoor(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

        ICellExit exit = (ICellExit)ParameterFunctions[0].Result?.GetObject;
        if (exit == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        IGameItem item = (IGameItem)ParameterFunctions[1].Result?.GetObject;
        if (item == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        IDoor door = item.GetItemType<IDoor>();
        if (door == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        if (exit.Exit.Door != null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        ICell hingecell = (ICell)ParameterFunctions[2].Result?.GetObject;
        if (hingecell == null)
        {
            hingecell = exit.Origin;
        }

        ICell opencell = (ICell)ParameterFunctions[3].Result?.GetObject;
        if (opencell == null)
        {
            opencell = exit.Destination;
        }

        if (door.InstalledExit != null)
        {
            IExit otherExit = door.InstalledExit;
            otherExit.Door = null;
            otherExit.Changed = true;
            door.OpenDirectionCell = null;
            door.HingeCell = null;
            door.State = DoorState.Uninstalled;
            door.InstalledExit = null;
        }

        door.Parent.Get(null);
        door.InstalledExit = exit.Exit;
        door.HingeCell = hingecell;
        door.OpenDirectionCell = opencell;
        door.State = DoorState.Open;
        exit.Exit.Door = door;
        exit.Exit.Changed = true;

        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }
}
