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
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.Location;

internal class LayerItems : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"LayerItems".ToLowerInvariant(),
				new[] { ProgVariableTypes.Location, ProgVariableTypes.Text },
				(pars, gameworld) => new LayerItems(pars, gameworld),
				new List<string> { "Location", "Layer" },
				new List<string>
				{
					"The location whose items you want to return",
					"The layer where you want items from"
				},
				"This function returns the items that are present at a specified layer in a room. See the ROOMLAYERS function for information on how to determine what layers are present.",
				"Rooms",
				ProgVariableTypes.Item | ProgVariableTypes.Collection
			)
		);
	}

	#endregion

	#region Constructors

	protected LayerItems(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Item | ProgVariableTypes.Collection;
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
			Result = new CollectionVariable(new List<IGameItem>(), ProgVariableTypes.Item);
			return StatementResult.Normal;
		}

		var layerText = ParameterFunctions[1].Result?.GetObject?.ToString();
		if (string.IsNullOrEmpty(layerText))
		{
			Result = new CollectionVariable(new List<IGameItem>(), ProgVariableTypes.Item);
			return StatementResult.Normal;
		}

		if (!Utilities.TryParseEnum<RoomLayer>(layerText, out var layer))
		{
			Result = new CollectionVariable(new List<IGameItem>(), ProgVariableTypes.Item);
			return StatementResult.Normal;
		}

		Result = new CollectionVariable(location.LayerGameItems(layer).ToList(), ProgVariableTypes.Item);
		return StatementResult.Normal;
	}
}