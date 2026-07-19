using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Discord;

internal class SendDiscordMessageFunction : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    protected SendDiscordMessageFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(
        parameterFunctions)
    {
        Gameworld = gameworld;
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Boolean;
        protected set { }
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "senddiscord",
                new[] { ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Text },
                (pars, gameworld) => new SendDiscordMessageFunction(pars, gameworld),
                new List<string> { "channelId", "title", "message" },
                new List<string> { "The numeric Discord channel ID to send to.", "The title or heading of the outgoing message.", "The body text of the outgoing Discord message." },
                "Sends a Discord message through the configured Discord connection using channel ID, title, and message text. Returns false if any argument is null; returns true after queueing the send even if no Discord connection is currently configured.",
                "Discord",
                ProgVariableTypes.Boolean,
                allowedContexts: new[] { FutureProgCompilationContext.StandardFutureProg }
            )
        );
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        IProgVariable channelResult = ParameterFunctions[0].Result;
        if (channelResult?.GetObject == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        var channelId = (ulong)Math.Truncate(Convert.ToDecimal(channelResult.GetObject));

        IProgVariable titleResult = ParameterFunctions[1].Result;
        if (titleResult?.GetObject == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        string title = titleResult.GetObject.ToString();

        IProgVariable textResult = ParameterFunctions[2].Result;
        if (textResult?.GetObject == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        string text = textResult.GetObject.ToString();

        Gameworld.DiscordConnection?.SendMessageFromProg(channelId, title, text);
        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }
}
