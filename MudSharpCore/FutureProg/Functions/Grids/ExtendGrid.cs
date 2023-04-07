using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Construction;

namespace MudSharp.FutureProg.Functions.Grids;

internal class ExtendGrid : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ExtendGrid".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Number, FutureProgVariableTypes.Location },
				(pars, gameworld) => new ExtendGrid(pars, gameworld),
				new List<string> { "grid", "location" },
				new List<string>
				{
					"The ID of the grid you want to extend",
					"The location you want to extend the grid to"
				},
				"This function allows you to extend a grid (electrical, gas, liquid etc) to a new location. Returns true if the extension happened, false implies the grid was already present or there was another error.",
				"Grids"
			)
		);
	}

	#endregion

	#region Constructors

	protected ExtendGrid(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var gridid = Convert.ToInt64(ParameterFunctions[0].Result?.GetObject ?? 0);
		var grid = Gameworld.Grids.Get(gridid);
		if (grid == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var location = (ICell)ParameterFunctions[1].Result?.GetObject;
		if (location == null || grid.Locations.Contains(location))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		grid.ExtendTo(location);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}