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

internal class SendShardFunction : BuiltInFunction
{
    protected SendShardFunction(IList<IFunction> parameters, IFuturemud gameworld)
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

    public bool FixedFormat { get; init; }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        if (ParameterFunctions[0].Result is not IShard target)
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

        List<IPerceivable> perceivables = new();
        foreach (IFunction parameter in ParameterFunctions.Skip(2))
        {
            if (!(parameter.Result is IPerceivable perceivable))
            {
                Result = new BooleanVariable(false);
                return StatementResult.Normal;
            }

            perceivables.Add(perceivable);
        }

        target.Handle(FixedFormat
            ? new EmoteOutput(new NoFormatEmote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, perceivables.ToArray()), flags: OutputFlags.IgnoreWatchers)
            : new EmoteOutput(new Emote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, perceivables.ToArray()), flags: OutputFlags.IgnoreWatchers));
        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshard",
                new[] { ProgVariableTypes.Shard, ProgVariableTypes.Text },
                (pars, gameworld) => new SendShardFunction(pars, gameworld),
                new List<string> { "shard", "message" },
                new List<string> { "The shard that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshard",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld),
                new List<string> { "shard", "message", "reference1" },
                new List<string> { "The shard that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshard",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld),
                new List<string> { "shard", "message", "reference1", "reference2" },
                new List<string> { "The shard that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshard",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld),
                new List<string> { "shard", "message", "reference1", "reference2", "reference3" },
                new List<string> { "The shard that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshard",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld),
                new List<string> { "shard", "message", "reference1", "reference2", "reference3", "reference4" },
                new List<string> { "The shard that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshard",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld),
                new List<string> { "shard", "message", "reference1", "reference2", "reference3", "reference4", "reference5" },
                new List<string> { "The shard that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshard",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld),
                new List<string> { "shard", "message", "reference1", "reference2", "reference3", "reference4", "reference5", "reference6" },
                new List<string> { "The shard that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshard",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld),
                new List<string> { "shard", "message", "reference1", "reference2", "reference3", "reference4", "reference5", "reference6", "reference7" },
                new List<string> { "The shard that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshard",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld),
                new List<string> { "shard", "message", "reference1", "reference2", "reference3", "reference4", "reference5", "reference6", "reference7", "reference8" },
                new List<string> { "The shard that receives or scopes the echo.", "The emote text to send. ANSI colour substitutions are applied and perceivable references can be used by the emote parser.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Non-fixed variants parse the text as an emote, so supplied perceivable references can be used for pronouns and placeholders. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );


        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshardfixed",
                new[] { ProgVariableTypes.Shard, ProgVariableTypes.Text },
                (pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true },
                new List<string> { "shard", "message" },
                new List<string> { "The shard that receives or scopes the echo.", "The fixed-format text to send. ANSI colour substitutions are applied, but emote parsing is bypassed." },
                "Sends a FutureProg echo to the supplied scope. Fixed variants bypass emote parsing and send the supplied text as fixed-format output. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshardfixed",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true },
                new List<string> { "shard", "message", "reference1" },
                new List<string> { "The shard that receives or scopes the echo.", "The fixed-format text to send. ANSI colour substitutions are applied, but emote parsing is bypassed.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Fixed variants bypass emote parsing and send the supplied text as fixed-format output. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshardfixed",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true },
                new List<string> { "shard", "message", "reference1", "reference2" },
                new List<string> { "The shard that receives or scopes the echo.", "The fixed-format text to send. ANSI colour substitutions are applied, but emote parsing is bypassed.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Fixed variants bypass emote parsing and send the supplied text as fixed-format output. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshardfixed",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true },
                new List<string> { "shard", "message", "reference1", "reference2", "reference3" },
                new List<string> { "The shard that receives or scopes the echo.", "The fixed-format text to send. ANSI colour substitutions are applied, but emote parsing is bypassed.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Fixed variants bypass emote parsing and send the supplied text as fixed-format output. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshardfixed",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true },
                new List<string> { "shard", "message", "reference1", "reference2", "reference3", "reference4" },
                new List<string> { "The shard that receives or scopes the echo.", "The fixed-format text to send. ANSI colour substitutions are applied, but emote parsing is bypassed.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Fixed variants bypass emote parsing and send the supplied text as fixed-format output. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshardfixed",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true },
                new List<string> { "shard", "message", "reference1", "reference2", "reference3", "reference4", "reference5" },
                new List<string> { "The shard that receives or scopes the echo.", "The fixed-format text to send. ANSI colour substitutions are applied, but emote parsing is bypassed.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Fixed variants bypass emote parsing and send the supplied text as fixed-format output. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshardfixed",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true },
                new List<string> { "shard", "message", "reference1", "reference2", "reference3", "reference4", "reference5", "reference6" },
                new List<string> { "The shard that receives or scopes the echo.", "The fixed-format text to send. ANSI colour substitutions are applied, but emote parsing is bypassed.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Fixed variants bypass emote parsing and send the supplied text as fixed-format output. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshardfixed",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true },
                new List<string> { "shard", "message", "reference1", "reference2", "reference3", "reference4", "reference5", "reference6", "reference7" },
                new List<string> { "The shard that receives or scopes the echo.", "The fixed-format text to send. ANSI colour substitutions are applied, but emote parsing is bypassed.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Fixed variants bypass emote parsing and send the supplied text as fixed-format output. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "sendshardfixed",
                new[]
                {
                    ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
                    ProgVariableTypes.Perceivable
                },
                (pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true },
                new List<string> { "shard", "message", "reference1", "reference2", "reference3", "reference4", "reference5", "reference6", "reference7", "reference8" },
                new List<string> { "The shard that receives or scopes the echo.", "The fixed-format text to send. ANSI colour substitutions are applied, but emote parsing is bypassed.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders.", "An optional perceivable reference for the message, used by the emote parser for pronouns and placeholders." },
                "Sends a FutureProg echo to the supplied scope. Fixed variants bypass emote parsing and send the supplied text as fixed-format output. Returns true when the scope, message, and every supplied perceivable reference are valid; returns false without sending if a required value is null.",
                "Echoes",
                ProgVariableTypes.Boolean
            )
        );
    }
}