using MudSharp.Character;
using MudSharp.Construction.Grids;
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

namespace MudSharp.FutureProg.Functions.Grids;

internal class ConnectToGrid : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "ConnectToGrid".ToLowerInvariant(),
                new[] { ProgVariableTypes.Number, ProgVariableTypes.Item },
                (pars, gameworld) => new ConnectToGrid(pars, gameworld),
                new List<string> { "grid", "item" },
                new List<string>
                {
                    "The ID of the grid you want to connect the item to",
                    "The grid-interfacing item you want to connect"
                },
                "This function takes a grid-interfacing item like a grid feeder or grid outlet and connects it. Returns true if it succeeded.",
                "Grids"
            )
        );
    }

    #endregion

    #region Constructors

    protected ConnectToGrid(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

        long gridid = Convert.ToInt64(ParameterFunctions[0].Result?.GetObject ?? 0);
        IGrid grid = Gameworld.Grids.Get(gridid);
        if (grid == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        IGameItem item = (IGameItem)ParameterFunctions[1].Result?.GetObject;
        ICanConnectToGrid gridItem = item?.GetItemType<ICanConnectToGrid>();
        if (gridItem == null || !gridItem.GridType.EqualTo(grid.GridType))
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        gridItem.Grid = grid;
        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }
}