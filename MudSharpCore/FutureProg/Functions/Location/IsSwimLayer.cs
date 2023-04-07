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

internal class IsSwimLayer : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"IsSwimLayer".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Location, FutureProgVariableTypes.Text },
				(pars, gameworld) => new IsSwimLayer(pars, gameworld),
				new List<string> { "Location", "Layer" },
				new List<string>
				{
					"The location whose layers you want to check for being a swimming layer",
					"The room layer that you want to test for being a swimming layer"
				},
				"This function allows you to test whether a particular room layer in a location is currently a swimming layer. Possible values for layers are VeryDeepUnderwater, DeepUnderwater, Underwater, GroundLevel, OnRooftops, InTrees, HighInTrees, InAir, HighInAir. See function ROOMLAYERS for how to obtain the list of room layers for a location.",
				"Rooms",
				FutureProgVariableTypes.Boolean
			)
		);
	}

	#endregion

	#region Constructors

	protected IsSwimLayer(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		if (ParameterFunctions[0].Result is not ICell location)
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

		Result = new BooleanVariable(location.IsSwimmingLayer(layer));
		return StatementResult.Normal;
	}
}