using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Echoes;

internal class SendTerrainLangFunction : BuiltInFunction
{
    protected SendTerrainLangFunction(IList<IFunction> parameters, IFuturemud gameworld)
        : base(parameters)
    {
        Gameworld = gameworld;
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

        if (ParameterFunctions[0].Result is not ITerrain target)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        IProgVariable textResult = ParameterFunctions[1].Result;
        if (textResult?.GetObject == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        string text = textResult.GetObject.ToString().SubstituteANSIColour();

        if (ParameterFunctions[2].Result?.GetObject is not ILanguage language)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        if (ParameterFunctions[3].Result?.GetObject is not IAccent accent)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        List<IPerceivable> perceivables = new();
        foreach (IFunction parameter in ParameterFunctions.Skip(4))
        {
            if (parameter.Result is not IPerceivable perceivable)
            {
                Result = new BooleanVariable(false);
                return StatementResult.Normal;
            }

            perceivables.Add(perceivable);
        }

        EmoteOutput output = new(new FixedLanguageEmote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, language, accent, perceivables.ToArray()),
            flags: OutputFlags.IgnoreWatchers);
        foreach (ICell cell in Gameworld.Cells)
        {
            if (cell.Terrain(null) == target)
            {
                cell.Handle(output);
            }
        }

        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendterrainlang",
                new[]
                {
                    ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
                    ProgVariableTypes.Accent
                },
                (pars, gameworld) => new SendTerrainLangFunction(pars, gameworld),
                new List<string> { "terrain", "message", "language", "accent" },
                new List<string> { "The terrain that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "The language used for a language-aware echo.", "The accent used for a language-aware echo." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. The language variants use the supplied language and accent, equivalent to sending a language-aware echo through the perception system. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendterrainlang",
                new[]
                {
                    ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
                    ProgVariableTypes.Accent, ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendTerrainLangFunction(pars, gameworld),
                new List<string> { "terrain", "message", "language", "accent", "reference1" },
                new List<string> { "The terrain that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "The language used for a language-aware echo.", "The accent used for a language-aware echo.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. The language variants use the supplied language and accent, equivalent to sending a language-aware echo through the perception system. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendterrainlang",
                new[]
                {
                    ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
                    ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendTerrainLangFunction(pars, gameworld),
                new List<string> { "terrain", "message", "language", "accent", "reference1", "reference2" },
                new List<string> { "The terrain that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "The language used for a language-aware echo.", "The accent used for a language-aware echo.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. The language variants use the supplied language and accent, equivalent to sending a language-aware echo through the perception system. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendterrainlang",
                new[]
                {
                    ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
                    ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendTerrainLangFunction(pars, gameworld),
                new List<string> { "terrain", "message", "language", "accent", "reference1", "reference2", "reference3" },
                new List<string> { "The terrain that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "The language used for a language-aware echo.", "The accent used for a language-aware echo.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. The language variants use the supplied language and accent, equivalent to sending a language-aware echo through the perception system. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendterrainlang",
                new[]
                {
                    ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
                    ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendTerrainLangFunction(pars, gameworld),
                new List<string> { "terrain", "message", "language", "accent", "reference1", "reference2", "reference3", "reference4" },
                new List<string> { "The terrain that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "The language used for a language-aware echo.", "The accent used for a language-aware echo.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. The language variants use the supplied language and accent, equivalent to sending a language-aware echo through the perception system. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendterrainlang",
                new[]
                {
                    ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
                    ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendTerrainLangFunction(pars, gameworld),
                new List<string> { "terrain", "message", "language", "accent", "reference1", "reference2", "reference3", "reference4", "reference5" },
                new List<string> { "The terrain that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "The language used for a language-aware echo.", "The accent used for a language-aware echo.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. The language variants use the supplied language and accent, equivalent to sending a language-aware echo through the perception system. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendterrainlang",
                new[]
                {
                    ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
                    ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendTerrainLangFunction(pars, gameworld),
                new List<string> { "terrain", "message", "language", "accent", "reference1", "reference2", "reference3", "reference4", "reference5", "reference6" },
                new List<string> { "The terrain that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "The language used for a language-aware echo.", "The accent used for a language-aware echo.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. The language variants use the supplied language and accent, equivalent to sending a language-aware echo through the perception system. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendterrainlang",
                new[]
                {
                    ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
                    ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendTerrainLangFunction(pars, gameworld),
                new List<string> { "terrain", "message", "language", "accent", "reference1", "reference2", "reference3", "reference4", "reference5", "reference6", "reference7" },
                new List<string> { "The terrain that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "The language used for a language-aware echo.", "The accent used for a language-aware echo.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. The language variants use the supplied language and accent, equivalent to sending a language-aware echo through the perception system. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendterrainlang",
                new[]
                {
                    ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
                    ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendTerrainLangFunction(pars, gameworld),
                new List<string> { "terrain", "message", "language", "accent", "reference1", "reference2", "reference3", "reference4", "reference5", "reference6", "reference7", "reference8" },
                new List<string> { "The terrain that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "The language used for a language-aware echo.", "The accent used for a language-aware echo.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. The language variants use the supplied language and accent, equivalent to sending a language-aware echo through the perception system. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );
    }
}