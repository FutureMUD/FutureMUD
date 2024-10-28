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
using MudSharp.Construction.Boundary;

namespace MudSharp.FutureProg.Functions.Location;

internal class LayerExits : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"LayerExits".ToLowerInvariant(),
				new[] { ProgVariableTypes.Location, ProgVariableTypes.Text },
				(pars, gameworld) => new LayerExits(pars, gameworld),
				new List<string> { "Location", "Layer" },
				new List<string>
				{
					"The location whose exits you are interested in",
					"The layer whose exits you want"
				},
				"This function returns the exits that exist at a particular layer in a location you specify. See the ROOMLAYERS function for information on how to determine what layers are present.",
				"Rooms",
				ProgVariableTypes.Exit | ProgVariableTypes.Collection
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"LayerExits".ToLowerInvariant(),
				new[] { ProgVariableTypes.Location, ProgVariableTypes.Character },
				(pars, gameworld) => new LayerExits(pars, gameworld),
				new List<string> { "Location", "Character" },
				new List<string>
				{
					"The location whose exits you are interested in",
					"The character whose layer exits you want"
				},
				"This function returns the exits that exist at a particular layer in a location you specify. See the ROOMLAYERS function for information on how to determine what layers are present.",
				"Rooms",
				ProgVariableTypes.Exit | ProgVariableTypes.Collection
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"LayerExits".ToLowerInvariant(),
				new[] { ProgVariableTypes.Location, ProgVariableTypes.Item },
				(pars, gameworld) => new LayerExits(pars, gameworld),
				new List<string> { "Location", "Item" },
				new List<string>
				{
					"The location whose exits you are interested in",
					"The item whose layer exits you want"
				},
				"This function returns the exits that exist at a particular layer in a location you specify. See the ROOMLAYERS function for information on how to determine what layers are present.",
				"Rooms",
				ProgVariableTypes.Exit | ProgVariableTypes.Collection
			)
		);
	}

	#endregion

	#region Constructors

	protected LayerExits(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		if (ParameterFunctions[0].Result is not ICell location)
		{
			Result = new CollectionVariable(new List<ICellExit>(), ProgVariableTypes.Exit);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[0].Result?.GetObject is IPerceiver voyeur)
		{
			Result = new CollectionVariable(location.ExitsFor(voyeur).ToList(), ProgVariableTypes.Exit);
		}

		var layerText = ParameterFunctions[1].Result?.GetObject?.ToString();
		if (string.IsNullOrEmpty(layerText))
		{
			Result = new CollectionVariable(new List<ICellExit>(), ProgVariableTypes.Exit);
			return StatementResult.Normal;
		}

		if (!Utilities.TryParseEnum<RoomLayer>(layerText, out var layer))
		{
			Result = new CollectionVariable(new List<ICellExit>(), ProgVariableTypes.Exit);
			return StatementResult.Normal;
		}

		Result = new CollectionVariable(
			location.ExitsFor(null, true).Where(x => x.WhichLayersExitAppears().Contains(layer)).ToList(),
			ProgVariableTypes.Exit);
		return StatementResult.Normal;
	}
}