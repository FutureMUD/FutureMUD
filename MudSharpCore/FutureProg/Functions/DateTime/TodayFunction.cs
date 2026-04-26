using MudSharp.FutureProg.Variables;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class TodayFunction : BuiltInFunction
{
    public TodayFunction(IList<IFunction> parameters)
        : base(parameters)
    {
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.DateTime;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        Result = new DateTimeVariable(System.DateTime.UtcNow.Date);
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "today",
            new ProgVariableTypes[] { },
            (pars, gameworld) => new TodayFunction(pars),
            new List<string>(),
            new List<string>(),
            "Returns the current real-world UTC date with the time component set to midnight. Use now() when you need the current instant rather than just the date.",
            "Date/Time",
            ProgVariableTypes.DateTime
        ));
    }
}