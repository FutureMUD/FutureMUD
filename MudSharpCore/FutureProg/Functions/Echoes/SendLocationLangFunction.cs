using MudSharp.Communication.Language;
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

internal class SendLocationLangFunction : BuiltInFunction
{
	public bool UseLayerArgument { get; }

	protected SendLocationLangFunction(IList<IFunction> parameters, bool useLayerArgument, IFuturemud gameworld)
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

		if (ParameterFunctions[2 + (UseLayerArgument ? 1 : 0)].Result?.GetObject is not Language language)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[3 + (UseLayerArgument ? 1 : 0)].Result?.GetObject is not Accent accent)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var perceivables = new List<IPerceivable>();
		foreach (var parameter in ParameterFunctions.Skip(4 + (UseLayerArgument ? 1 : 0)))
		{
			if (parameter.Result is not IPerceivable perceivable)
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			perceivables.Add(perceivable);
		}

		if (UseLayerArgument)
		{
			target.Handle(layer,
				new EmoteOutput(new FixedLanguageEmote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, language, accent, perceivables.ToArray())));
		}
		else
		{
			target.Handle(
				new EmoteOutput(new FixedLanguageEmote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, language, accent, perceivables.ToArray())));
		}

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclang",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, false, gameworld),
				new List<string> { "Location", "Text", "Language", "Accent" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message"
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclang",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, false, gameworld),
				new List<string> { "Location", "Text", "Language", "Accent", "Perceivable1" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclang",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, false, gameworld),
				new List<string> { "Location", "Text", "Language", "Accent", "Perceivable1", "Perceivable2" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclang",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, false, gameworld),
				new List<string>
					{ "Location", "Text", "Language", "Accent", "Perceivable1", "Perceivable2", "Perceivable3" },
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclang",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, false, gameworld),
				new List<string>
				{
					"Location", "Text", "Language", "Accent", "Perceivable1", "Perceivable2", "Perceivable3",
					"Perceivable4"
				},
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclang",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, false, gameworld),
				new List<string>
				{
					"Location", "Text", "Language", "Accent", "Perceivable1", "Perceivable2", "Perceivable3",
					"Perceivable4", "Perceivable5"
				},
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclang",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, false, gameworld),
				new List<string>
				{
					"Location", "Text", "Language", "Accent", "Perceivable1", "Perceivable2", "Perceivable3",
					"Perceivable4", "Perceivable5", "Perceivable6"
				},
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclang",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, false, gameworld),
				new List<string>
				{
					"Location", "Text", "Language", "Accent", "Perceivable1", "Perceivable2", "Perceivable3",
					"Perceivable4", "Perceivable5", "Perceivable6", "Perceivable7"
				},
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer.",
					"A perceivable to use dynamically in the echoes. Use $7 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclang",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, false, gameworld)
				,
				new List<string>
				{
					"Location", "Text", "Language", "Accent", "Perceivable1", "Perceivable2", "Perceivable3",
					"Perceivable4", "Perceivable5", "Perceivable6", "Perceivable7", "Perceivable8"
				},
				new List<string>
				{
					"The location to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer.",
					"A perceivable to use dynamically in the echoes. Use $7 to refer.",
					"A perceivable to use dynamically in the echoes. Use $8 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		// Room Layer versions
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclanglayer",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Language, ProgVariableTypes.Accent
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, true, gameworld),
				new List<string> { "Location", "Layer", "Text", "Language", "Accent" },
				new List<string>
				{
					"The location to send the message to",
					"The layer to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message"
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclanglayer",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Language, ProgVariableTypes.Accent,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, true, gameworld),
				new List<string> { "Location", "Layer", "Text", "Language", "Accent", "Perceivable1" },
				new List<string>
				{
					"The location to send the message to",
					"The layer to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclanglayer",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Language, ProgVariableTypes.Accent,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, true, gameworld),
				new List<string> { "Location", "Layer", "Text", "Language", "Accent", "Perceivable1", "Perceivable2" },
				new List<string>
				{
					"The location to send the message to",
					"The layer to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclanglayer",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Language, ProgVariableTypes.Accent,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, true, gameworld),
				new List<string>
				{
					"Location", "Layer", "Text", "Language", "Accent", "Perceivable1", "Perceivable2", "Perceivable3"
				},
				new List<string>
				{
					"The location to send the message to",
					"The layer to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclanglayer",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Language, ProgVariableTypes.Accent,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, true, gameworld),
				new List<string>
				{
					"Location", "Layer", "Text", "Language", "Accent", "Perceivable1", "Perceivable2", "Perceivable3",
					"Perceivable4"
				},
				new List<string>
				{
					"The location to send the message to",
					"The layer to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclanglayer",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Language, ProgVariableTypes.Accent,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, true, gameworld),
				new List<string>
				{
					"Location", "Layer", "Text", "Language", "Accent", "Perceivable1", "Perceivable2", "Perceivable3",
					"Perceivable4", "Perceivable5"
				},
				new List<string>
				{
					"The location to send the message to",
					"The layer to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclanglayer",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Language, ProgVariableTypes.Accent,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, true, gameworld),
				new List<string>
				{
					"Location", "Layer", "Text", "Language", "Accent", "Perceivable1", "Perceivable2", "Perceivable3",
					"Perceivable4", "Perceivable5", "Perceivable6"
				},
				new List<string>
				{
					"The location to send the message to", "The layer to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclanglayer",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Language, ProgVariableTypes.Accent,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, true, gameworld),
				new List<string>
				{
					"Location", "Layer", "Text", "Language", "Accent", "Perceivable1", "Perceivable2", "Perceivable3",
					"Perceivable4", "Perceivable5", "Perceivable6", "Perceivable7"
				},
				new List<string>
				{
					"The location to send the message to",
					"The layer to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer.",
					"A perceivable to use dynamically in the echoes. Use $7 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendloclanglayer",
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.Text, ProgVariableTypes.Text,
					ProgVariableTypes.Language, ProgVariableTypes.Accent,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLocationLangFunction(pars, true, gameworld)
				,
				new List<string>
				{
					"Location", "Layer", "Text", "Language", "Accent", "Perceivable1", "Perceivable2", "Perceivable3",
					"Perceivable4", "Perceivable5", "Perceivable6", "Perceivable7", "Perceivable8"
				},
				new List<string>
				{
					"The location to send the message to",
					"The layer to send the message to",
					"The message to send to the location",
					"The language that the language content of the message should be understood in",
					"The spoken accent of the language content of the message",
					"A perceivable to use dynamically in the echoes. Use $1 to refer.",
					"A perceivable to use dynamically in the echoes. Use $2 to refer.",
					"A perceivable to use dynamically in the echoes. Use $3 to refer.",
					"A perceivable to use dynamically in the echoes. Use $4 to refer.",
					"A perceivable to use dynamically in the echoes. Use $5 to refer.",
					"A perceivable to use dynamically in the echoes. Use $6 to refer.",
					"A perceivable to use dynamically in the echoes. Use $7 to refer.",
					"A perceivable to use dynamically in the echoes. Use $8 to refer."
				},
				"This function sends a specified message (including potentially language/accent bound spoken information) to every perceiver in the location. You can use the colour tags (#0, #1 etc) in this echo. Anything inside double quotes will be interpreted as language.",
				"Echoes",
				ProgVariableTypes.Boolean
			)
		);
	}
}