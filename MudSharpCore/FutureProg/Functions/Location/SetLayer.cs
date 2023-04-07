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

internal class SetLayer : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetLayer".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Text },
				(pars, gameworld) => new SetLayer(pars, gameworld),
				new List<string> { "Character", "Layer" },
				new List<string>
				{
					"The character whose location you are setting",
					"The layer to change them to"
				},
				"This function changes the layer a character is currently at. Returns true if successful. See the ROOMLAYERS function for information on how to determine what layers are present.",
				"Rooms",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetLayer".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Text },
				(pars, gameworld) => new SetLayer(pars, gameworld),
				new List<string> { "Item", "Layer" },
				new List<string>
				{
					"The item whose location you are setting",
					"The layer to change them to"
				},
				"This function changes the layer an item is currently at. Returns true if successful. See the ROOMLAYERS function for information on how to determine what layers are present.",
				"Rooms",
				FutureProgVariableTypes.Boolean
			)
		);
	}

	#endregion

	#region Constructors

	protected SetLayer(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		if (ParameterFunctions[0].Result?.GetObject is not IPerceiver perceiver)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var layerText = ParameterFunctions[1].Result?.GetObject?.ToString();
		if (string.IsNullOrEmpty(layerText))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (!Utilities.TryParseEnum<RoomLayer>(layerText, out var layer))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		perceiver.RoomLayer = layer;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}