using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using System.Numerics;

namespace MudSharp.FutureProg.Functions.Characters;

internal class Teleport : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"teleport",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Location
				}, // the parameters the function takes
				(pars, gameworld) => new Teleport(pars, gameworld, false),
				new List<string>
				{
					"Character",
					"Destination"
				}, // parameter names
				new List<string>
				{
					"The character who you want to teleport",
					"The destination room to teleport them to"
				}, // parameter help text
				"Teleports a character to the ground level (or closest layer) in a new room. Returns false if the teleportation fails (if invalid character, room, or layer is specified).", // help text for the function,
				"Character", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"teleportnoecho",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Location
				}, // the parameters the function takes
				(pars, gameworld) => new Teleport(pars, gameworld, true),
				new List<string>
				{
					"Character",
					"Destination"
				}, // parameter names
				new List<string>
				{
					"The character who you want to teleport",
					"The destination room to teleport them to"
				}, // parameter help text
				"Teleports a character to the ground level (or closest layer) in a new room, with no echoes. Returns false if the teleportation fails (if invalid character, room, or layer is specified).", // help text for the function,
				"Character", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"teleport",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Text
				}, // the parameters the function takes
				(pars, gameworld) => new Teleport(pars, gameworld, false),
				new List<string>
				{
					"Character",
					"Destination",
					"Layer"
				}, // parameter names
				new List<string>
				{
					"The character who you want to teleport",
					"The destination room to teleport them to",
					"The room layer to teleport to"
				}, // parameter help text
				"Teleports a character to the specified layer in a new room. Returns false if the teleportation fails (if invalid character, room, or layer is specified).", // help text for the function,
				"Character", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"teleportnoecho",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Text
				}, // the parameters the function takes
				(pars, gameworld) => new Teleport(pars, gameworld, true),
				new List<string>
				{
					"Character",
					"Destination",
					"Layer"
				}, // parameter names
				new List<string>
				{
					"The character who you want to teleport",
					"The destination room to teleport them to",
					"The room layer to teleport to"
				}, // parameter help text
				"Teleports a character to the specified layer in a new room, with no echoes. Returns false if the teleportation fails (if invalid character, room, or layer is specified).", // help text for the function,
				"Character", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);
	}

	#endregion

	public bool IsSilent { get; }

	#region Constructors

	protected Teleport(IList<IFunction> parameterFunctions, IFuturemud gameworld, bool isSilent) : base(
		parameterFunctions)
	{
		Gameworld = gameworld;
		IsSilent = isSilent;
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

		if (ParameterFunctions[0].Result is not ICharacter target)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[1].Result is not ICell location)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var layer = RoomLayer.GroundLevel;
		if (ParameterFunctions.Count == 3)
		{
			var layerText = ParameterFunctions[2].Result?.GetObject?.ToString();
			if (string.IsNullOrEmpty(layerText))
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			if (!layerText.TryParseEnum(out layer))
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}
		}

		if (!location.Terrain(target).TerrainLayers.Contains(layer))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		target.Movement?.CancelForMoverOnly(target);
		target.RemoveAllEffects(x => x.IsEffectType<IActionEffect>());

		if (!IsSilent)
		{
			target.OutputHandler.Handle(new EmoteOutput(new Emote("@ leaves the area.", target),
				flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
		}

		target.Location.Leave(target);
		target.RoomLayer = layer;
		location.Enter(target);
		if (!IsSilent)
		{
			target.OutputHandler.Handle(new EmoteOutput(new Emote("@ enters the area.", target),
				flags: OutputFlags.SuppressObscured));
		}

		target.Body.Look(true);

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}