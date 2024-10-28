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

namespace MudSharp.FutureProg.Functions.Location;

internal class RoomLayers : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"RoomLayers".ToLowerInvariant(),
				new[] { ProgVariableTypes.Location },
				(pars, gameworld) => new RoomLayers(pars, gameworld),
				new List<string> { "Location" },
				new List<string> { "The location whose layers you want to determine" },
				"This function returns a collection of text values representing all the layers in the specified cell. Possible values for layers are VeryDeepUnderwater, DeepUnderwater, Underwater, GroundLevel, OnRooftops, InTrees, HighInTrees, InAir, HighInAir.",
				"Rooms",
				ProgVariableTypes.Text | ProgVariableTypes.Collection
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"RoomLayers".ToLowerInvariant(),
				new[] { ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage },
				(pars, gameworld) => new RoomLayers(pars, gameworld),
				new List<string> { "Location", "Package" },
				new List<string>
				{
					"The location whose layers you want to determine",
					"The package you want to use to evaluate what terrain type this location is"
				},
				"This function returns a collection of text values representing all the layers in the specified cell. Possible values for layers are VeryDeepUnderwater, DeepUnderwater, Underwater, GroundLevel, OnRooftops, InTrees, HighInTrees, InAir, HighInAir.",
				"Rooms",
				ProgVariableTypes.Text | ProgVariableTypes.Collection
			)
		);
	}

	#endregion

	#region Constructors

	protected RoomLayers(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Text | ProgVariableTypes.Collection;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result is not ICell location)
		{
			Result = new CollectionVariable(new List<TextVariable>(), ProgVariableTypes.Text);
			return StatementResult.Normal;
		}

		ITerrain terrain;
		if (ParameterFunctions.Count == 2)
		{
			var package = (ICellOverlayPackage)ParameterFunctions[1].Result?.GetObject;
			if (package == null)
			{
				Result = new CollectionVariable(new List<TextVariable>(), ProgVariableTypes.Text);
				return StatementResult.Normal;
			}

			var overlay = location.GetOverlay(package);
			if (overlay == null)
			{
				Result = new CollectionVariable(new List<TextVariable>(), ProgVariableTypes.Text);
				return StatementResult.Normal;
			}

			terrain = overlay.Terrain;
		}
		else
		{
			terrain = location.Terrain(null);
		}

		Result = new CollectionVariable(terrain.TerrainLayers.Select(x => new TextVariable(x.DescribeEnum())).ToList(),
			ProgVariableTypes.Text);
		return StatementResult.Normal;
	}
}