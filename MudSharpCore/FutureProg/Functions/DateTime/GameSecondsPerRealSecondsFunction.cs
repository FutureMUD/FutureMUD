using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate.Time;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class GameSecondsPerRealSecondsFunction : BuiltInFunction
{
    private readonly IFuturemud _gameworld;

    public GameSecondsPerRealSecondsFunction(IList<IFunction> parameters, IFuturemud gameworld)
        : base(parameters)
    {
        _gameworld = gameworld;
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Number;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        IProgVariable textResult = ParameterFunctions[0].Result;
        if (textResult?.GetObject == null)
        {
            ErrorMessage = "gamesecondsperrealseconds - Unable to parse clock name parameter.";
            return StatementResult.Error;
        }

        string text = textResult.GetObject.ToString();

        IClock clock = _gameworld.Clocks.GetByName(text);
        if (clock == null)
        {
            ErrorMessage = "gamesecondsperrealseconds - Invalid clock name received.";
            return StatementResult.Error;
        }

        Result = new NumberVariable(clock.InGameSecondsPerRealSecond);

        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "gamesecondsperrealseconds",
            new[] { ProgVariableTypes.Text },
            (pars, gameworld) => new GameSecondsPerRealSecondsFunction(pars, gameworld),
            new List<string> { "clock" },
            new List<string> { "The in-game clock to use, or the clock name where this function accepts text." },
            "Looks up a clock by name and returns how many in-game seconds pass for each real second. Errors if the clock name is null or does not match a clock.",
            "Date/Time",
            ProgVariableTypes.Number
        ));
    }
}