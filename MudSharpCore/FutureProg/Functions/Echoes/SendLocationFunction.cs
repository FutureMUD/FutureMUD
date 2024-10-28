using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.Echoes;

internal class SendLocationFunction : BuiltInFunction
{
	protected SendLocationFunction(IList<IFunction> parameters, bool useLayerArgument, IFuturemud gameworld)
		: base(parameters)
	{
		Gameworld = gameworld;
		UseLayerArgument = useLayerArgument;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	private IFuturemud Gameworld { get; }

	public bool FixedFormat { get; init; }
	public bool UseLayerArgument { get; }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result is not ICell target)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var layer = RoomLayer.GroundLevel;
		if (UseLayerArgument)
		{
			var layerText = ParameterFunctions[1].Result?.GetObject?.ToString();
			if (string.IsNullOrEmpty(layerText))
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			if (!Utilities.TryParseEnum<RoomLayer>(layerText, out layer))
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}
		}

		var textResult = ParameterFunctions[1 + (UseLayerArgument ? 1 : 0)].Result;
		if (textResult?.GetObject == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var text = textResult.GetObject.ToString().SubstituteANSIColour();

		var perceivables = new List<IPerceivable>();
		foreach (var parameter in ParameterFunctions.Skip(2 + (UseLayerArgument ? 1 : 0)))
		{
			if (!(parameter.Result is IPerceivable perceivable))
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			perceivables.Add(perceivable);
		}

		if (UseLayerArgument)
		{
			target.Handle(layer, FixedFormat
				? new EmoteOutput(new NoFormatEmote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, perceivables.ToArray()))
				: new EmoteOutput(new Emote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, perceivables.ToArray())));
		}
		else
		{
			target.Handle(FixedFormat
				? new EmoteOutput(new NoFormatEmote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, perceivables.ToArray()))
				: new EmoteOutput(new Emote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, perceivables.ToArray())));
		}


		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[] { ProgVariableTypes.Location, ProgVariableTypes.Text },
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld),
				new List<string> { "Location", "Text" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location"
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld),
				new List<string> { "Location", "Text", "Perceivable1" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld),
				new List<string> { "Location", "Text", "Perceivable1", "Perceivable2" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld),
				new List<string> { "Location", "Text", "Perceivable1", "Perceivable2", "Perceivable3" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld),
				new List<string> { "Location", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld),
				new List<string>
				{
					"Location", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4", "Perceivable5"
				},
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld),
				new List<string>
				{
					"Location", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4", "Perceivable5",
					"Perceivable6"
				},
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld),
				new List<string>
				{
					"Location", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4", "Perceivable5",
					"Perceivable6", "Perceivable7"
				},
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer.",
					"A perceivable to use dynamically in the echoes. Use $7 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld),
				new List<string>
				{
					"Location", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4", "Perceivable5",
					"Perceivable6", "Perceivable7", "Perceivable8"
				},
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer.",
					"A perceivable to use dynamically in the echoes. Use $7 to refer.",
					"A perceivable to use dynamically in the echoes. Use $8 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		// Room layer versions
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[] { ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text },
				(pars, gameworld) => new SendLocationFunction(pars, true, gameworld),
				new List<string> { "Location", "Layer", "Text" },
				new List<string>
				{
					"The location to send the message to", "The layer to send the message to",
					"The message to send to the location"
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, true, gameworld),
				new List<string> { "Location", "Layer", "Text", "Perceivable1" },
				new List<string>
				{
					"The location to send the message to", "The layer to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, true, gameworld),
				new List<string> { "Location", "Layer", "Text", "Perceivable1", "Perceivable2" },
				new List<string>
				{
					"The location to send the message to", "The layer to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, true, gameworld),
				new List<string> { "Location", "Layer", "Text", "Perceivable1", "Perceivable2", "Perceivable3" },
				new List<string>
				{
					"The location to send the message to", "The layer to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, true, gameworld),
				new List<string>
					{ "Location", "Layer", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4" },
				new List<string>
				{
					"The location to send the message to", "The layer to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, true, gameworld),
				new List<string>
				{
					"Location", "Layer", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4",
					"Perceivable5"
				},
				new List<string>
				{
					"The location to send the message to", "The layer to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, true, gameworld),
				new List<string>
				{
					"Location", "Layer", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4",
					"Perceivable5", "Perceivable6"
				},
				new List<string>
				{
					"The location to send the message to", "The layer to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, true, gameworld),
				new List<string>
				{
					"Location", "Layer", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4",
					"Perceivable5", "Perceivable6", "Perceivable7"
				},
				new List<string>
				{
					"The location to send the message to", "The layer to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer.",
					"A perceivable to use dynamically in the echoes. Use $7 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloc",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, true, gameworld),
				new List<string>
				{
					"Location", "Layer", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4",
					"Perceivable5", "Perceivable6", "Perceivable7", "Perceivable8"
				},
				new List<string>
				{
					"The location to send the message to", "The layer to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer.",
					"A perceivable to use dynamically in the echoes. Use $7 to refer.",
					"A perceivable to use dynamically in the echoes. Use $8 to refer."
				},
				"This function sends a specified message to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		// Fixed versions
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlocfixed",
				new[] { ProgVariableTypes.Location, ProgVariableTypes.Text },
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld) { FixedFormat = true },
				new List<string> { "Location", "Text" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location"
				},
				"This function sends a specified message to every perceiver in the location - the format is fixed, it does not wrap / process in any way. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlocfixed",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld) { FixedFormat = true },
				new List<string> { "Location", "Text", "Perceivable1" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer."
				},
				"This function sends a specified message to every perceiver in the location - the format is fixed, it does not wrap / process in any way. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlocfixed",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld) { FixedFormat = true },
				new List<string> { "Location", "Text", "Perceivable1", "Perceivable2" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer."
				},
				"This function sends a specified message to every perceiver in the location - the format is fixed, it does not wrap / process in any way. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlocfixed",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld) { FixedFormat = true },
				new List<string> { "Location", "Text", "Perceivable1", "Perceivable2", "Perceivable3" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer."
				},
				"This function sends a specified message to every perceiver in the location - the format is fixed, it does not wrap / process in any way. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlocfixed",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld) { FixedFormat = true },
				new List<string> { "Location", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer."
				},
				"This function sends a specified message to every perceiver in the location - the format is fixed, it does not wrap / process in any way. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlocfixed",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld) { FixedFormat = true },
				new List<string>
				{
					"Location", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4", "Perceivable5"
				},
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer."
				},
				"This function sends a specified message to every perceiver in the location - the format is fixed, it does not wrap / process in any way. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlocfixed",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld) { FixedFormat = true },
				new List<string>
				{
					"Location", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4", "Perceivable5",
					"Perceivable6"
				},
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer."
				},
				"This function sends a specified message to every perceiver in the location - the format is fixed, it does not wrap / process in any way. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlocfixed",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld) { FixedFormat = true },
				new List<string>
				{
					"Location", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4", "Perceivable5",
					"Perceivable6", "Perceivable7"
				},
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer.",
					"A perceivable to use dynamically in the echoes. Use $7 to refer."
				},
				"This function sends a specified message to every perceiver in the location - the format is fixed, it does not wrap / process in any way. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlocfixed",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationFunction(pars, false, gameworld) { FixedFormat = true },
				new List<string>
				{
					"Location", "Text", "Perceivable1", "Perceivable2", "Perceivable3", "Perceivable4", "Perceivable5",
					"Perceivable6", "Perceivable7", "Perceivable8"
				},
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer.",
					"A perceivable to use dynamically in the echoes. Use $7 to refer.",
					"A perceivable to use dynamically in the echoes. Use $8 to refer."
				},
				"This function sends a specified message to every perceiver in the location - the format is fixed, it does not wrap / process in any way. You can use the colour tags (#0, #1 etc) in this echo.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);
	}
}