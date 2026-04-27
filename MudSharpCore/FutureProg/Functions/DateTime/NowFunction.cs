using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class NowFunction : BuiltInFunction
{
    public NowFunction(IList<IFunction> parameters)
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
        Result = new DateTimeVariable(System.DateTime.UtcNow);
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "now",
            new ProgVariableTypes[] { },
            (pars, gameworld) => new NowFunction(pars),
            new List<string>(),
            new List<string>(),
            "Returns the current real-world UTC date and time, equivalent to System.DateTime.UtcNow. Use this for real-time expiry or logging rather than in-game calendar time.",
            "Date/Time",
            ProgVariableTypes.DateTime
        ));
    }
}

internal class MudNowFunction : BuiltInFunction
{
    public MudNowFunction(IList<IFunction> parameters)
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

        var date = calendar.CurrentDate;

        var clock = ParameterFunctions.Count > 1
            ? ParameterFunctions[1].Result?.GetObject as Clock ?? calendar.FeedClock
            : calendar.FeedClock;
        if (clock == null)
        {
            Result = MudDateTime.Never;
            return StatementResult.Normal;
        }

        var time = clock.CurrentTime;

        var timezone = clock.PrimaryTimezone;
        if (ParameterFunctions.Count > 2)
        {
            var timezoneText = ParameterFunctions[2].Result?.GetObject?.ToString();
            if (!string.IsNullOrWhiteSpace(timezoneText))
            {
                timezone = clock.Timezones.GetByIdOrName(timezoneText) ?? clock.PrimaryTimezone;
            }
        }

        if (timezone != clock.PrimaryTimezone)
        {
            time = MudTime.CopyOf(time).GetTimeByTimezone(timezone);
            if (time.DaysOffsetFromDatum != 0)
            {
                date = new MudDate(date);
                date.AdvanceDays(time.DaysOffsetFromDatum);
            }
        }

        Result = new MudDateTime(date, time, timezone);
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "now",
            new[] { ProgVariableTypes.Calendar },
            (pars, gameworld) => new MudNowFunction(pars),
            new List<string> { "calendar" },
            new List<string> { "The in-game calendar to use. Its feed clock supplies the time when no clock is specified." },
            "Returns the current in-game date and time for a calendar using the calendar's feed clock and that clock's primary timezone. Returns the special Never mud datetime if the calendar or clock is null.",
            "Date/Time",
            ProgVariableTypes.MudDateTime
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "now",
            new[] { ProgVariableTypes.Calendar, ProgVariableTypes.Clock },
            (pars, gameworld) => new MudNowFunction(pars),
            new List<string> { "calendar", "clock" },
            new List<string>
            {
                "The in-game calendar whose current date is used.",
                "The in-game clock whose current time is used. If null, the calendar's feed clock is used."
            },
            "Returns the current in-game date and time for a calendar and clock using the clock's primary timezone. Returns the special Never mud datetime if the calendar or resolved clock is null.",
            "Date/Time",
            ProgVariableTypes.MudDateTime
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "now",
            new[] { ProgVariableTypes.Calendar, ProgVariableTypes.Clock, ProgVariableTypes.Text },
            (pars, gameworld) => new MudNowFunction(pars),
            new List<string> { "calendar", "clock", "timezone" },
            new List<string>
            {
                "The in-game calendar whose current date is used.",
                "The in-game clock whose current time is used. If null, the calendar's feed clock is used.",
                "Optional timezone alias or ID from the supplied clock. If omitted or invalid, the clock's primary timezone is used."
            },
            "Returns the current in-game date and time for a calendar and clock, adjusted to the supplied timezone when it matches one of the clock's timezones. Returns the special Never mud datetime if the calendar or resolved clock is null.",
            "Date/Time",
            ProgVariableTypes.MudDateTime
        ));
    }
}
