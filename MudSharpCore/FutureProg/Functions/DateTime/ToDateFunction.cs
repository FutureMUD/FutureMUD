using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Time;
using System.Collections.Generic;
using System.Globalization;
using Calendar = MudSharp.TimeAndDate.Date.Calendar;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class ToDateFunction : BuiltInFunction
{
    private ToDateFunction(IList<IFunction> parameters)
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
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        string dateText = (string)ParameterFunctions[0].Result.GetObject;
        string dateMask = (string)ParameterFunctions[1].Result.GetObject;

        if (
            !System.DateTime.TryParseExact(dateText, dateMask, CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal, out System.DateTime date))
        {
            ErrorMessage = "Invalid date in ToDate function";
            return StatementResult.Error;
        }

        Result = new DateTimeVariable(date);
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "todate",
            new[] { ProgVariableTypes.Text, ProgVariableTypes.Text },
            (pars, gameworld) => new ToDateFunction(pars),
            new List<string> { "dateText", "format" },
            new List<string> { "The real-world date text to parse.", "A .NET date/time format string used to format or parse the date." },
            "Parses real-world date text using a .NET format mask and invariant culture. Returns an error if the text does not match the supplied format.",
            "Date/Time",
            ProgVariableTypes.DateTime
        ));
    }
}

internal class ToMudDateFunction : BuiltInFunction
{
    private ToMudDateFunction(IList<IFunction> parameters)
        : base(parameters)
    {
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.MudDateTime;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        if (ParameterFunctions[0].Result?.GetObject is not Calendar calendar)
        {
            Result = MudDateTime.Never;
            return StatementResult.Normal;
        }

        IClock clock = ParameterFunctions[1].Result?.GetObject as Clock ?? calendar.FeedClock;
        string text = ParameterFunctions?[2].Result?.GetObject?.ToString() ??
                   ParameterFunctions[1].Result?.GetObject?.ToString();

        if (string.IsNullOrEmpty(text))
        {
            Result = MudDateTime.Never;
            return StatementResult.Normal;
        }

        Result = MudDateTime.TryParse(text, calendar, clock, null, out MudDateTime dt, out _) ? dt : MudDateTime.Never;
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "todate",
            new[] { ProgVariableTypes.Calendar, ProgVariableTypes.Clock, ProgVariableTypes.Text },
            (pars, gameworld) => new ToMudDateFunction(pars),
            new List<string> { "calendar", "clock", "text" },
            new List<string> { "The in-game calendar to use.", "The in-game clock to use, or the clock name where this function accepts text.", "The text to parse." },
            "Parses text into an in-game mud datetime using the supplied calendar and clock. Returns the special Never mud datetime when the calendar is null, the text is empty, or parsing fails.",
            "Date/Time",
            ProgVariableTypes.MudDateTime
        ));
    }
}