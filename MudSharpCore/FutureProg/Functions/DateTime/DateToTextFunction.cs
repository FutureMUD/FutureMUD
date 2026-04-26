using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class DateToTextFunction : BuiltInFunction
{
    public DateToTextFunction(IList<IFunction> parameters)
        : base(parameters)
    {
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Text;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        System.DateTime date = (System.DateTime)ParameterFunctions[0].Result.GetObject;
        switch (ParameterFunctions.Count)
        {
            case 1:
                Result = new TextVariable(date.ToString(CultureInfo.InvariantCulture));
                break;
            case 2:
                Result = new TextVariable(date.ToString((IFormatProvider)ParameterFunctions[1].Result.GetObject));
                break;
            case 3:
                Result =
                    new TextVariable(date.ToString((string)ParameterFunctions[1].Result.GetObject,
                        (IFormatProvider)ParameterFunctions[2].Result.GetObject));
                break;
        }

        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "totext",
            new[] { ProgVariableTypes.DateTime },
            (pars, gameworld) => new DateToTextFunction(pars),
            new List<string> { "value" },
            new List<string> { "The value to convert or inspect." },
            "Formats a real-world datetime, optionally with a format string and a character whose culture and account formatting preferences should be used.",
            "Date/Time",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "totext",
            new[] { ProgVariableTypes.DateTime, ProgVariableTypes.Toon },
            (pars, gameworld) => new DateToTextFunction(pars),
            new List<string> { "value", "voyeur" },
            new List<string> { "The value to convert or inspect.", "The character whose account culture and formatting preferences should be used." },
            "Formats a real-world datetime, optionally with a format string and a character whose culture and account formatting preferences should be used.",
            "Date/Time",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "totext",
            new[] { ProgVariableTypes.DateTime, ProgVariableTypes.Text, ProgVariableTypes.Toon },
            (pars, gameworld) => new DateToTextFunction(pars),
            new List<string> { "value", "format", "voyeur" },
            new List<string> { "The value to convert or inspect.", "A .NET date/time format string used to format or parse the date.", "The character whose account culture and formatting preferences should be used." },
            "Formats a real-world datetime, optionally with a format string and a character whose culture and account formatting preferences should be used.",
            "Date/Time",
            ProgVariableTypes.Text
        ));
    }
}

internal class MudDateToTextFunction : BuiltInFunction
{
    public MudDateToTextFunction(IList<IFunction> parameters, CalendarDisplayMode mode, TimeDisplayTypes type)
        : base(parameters)
    {
        _mode = mode;
        _type = type;
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Text;
        protected set { }
    }

    private CalendarDisplayMode _mode { get; }
    private TimeDisplayTypes _type { get; }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        MudDateTime date = (MudDateTime)ParameterFunctions[0].Result.GetObject;
        Result = new TextVariable(date?.ToString(_mode, _type) ?? "Never");
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "totext",
            new[] { ProgVariableTypes.MudDateTime },
            (pars, gameworld) => new MudDateToTextFunction(pars, CalendarDisplayMode.Long, TimeDisplayTypes.Long),
            new List<string> { "value" },
            new List<string> { "The value to convert or inspect." },
            "Formats an in-game mud datetime with long calendar and time display rules. Null or Never mud datetimes display as Never.",
            "Date/Time",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "totextshort",
            new[] { ProgVariableTypes.MudDateTime },
            (pars, gameworld) => new MudDateToTextFunction(pars, CalendarDisplayMode.Short, TimeDisplayTypes.Short),
            new List<string> { "value" },
            new List<string> { "The in-game mud datetime to format using short calendar and short time display rules." },
            "Formats an in-game mud datetime using short calendar and short time display rules. Null or Never mud datetimes display as Never.",
            "Date/Time",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "totextwordy",
            new[] { ProgVariableTypes.MudDateTime },
            (pars, gameworld) => new MudDateToTextFunction(pars, CalendarDisplayMode.Wordy, TimeDisplayTypes.Long),
            new List<string> { "value" },
            new List<string> { "The in-game mud datetime to format with wordy calendar and long time display rules." },
            "Formats an in-game mud datetime using wordy calendar and long time display rules. Null or Never mud datetimes display as Never.",
            "Date/Time",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "totextcrude",
            new[] { ProgVariableTypes.MudDateTime },
            (pars, gameworld) => new MudDateToTextFunction(pars, CalendarDisplayMode.Long, TimeDisplayTypes.Crude),
            new List<string> { "value" },
            new List<string> { "The in-game mud datetime to format with long calendar and crude time display rules." },
            "Formats an in-game mud datetime using long calendar and crude time display rules. Null or Never mud datetimes display as Never.",
            "Date/Time",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "totextvague",
            new[] { ProgVariableTypes.MudDateTime },
            (pars, gameworld) => new MudDateToTextFunction(pars, CalendarDisplayMode.Long, TimeDisplayTypes.Vague),
            new List<string> { "value" },
            new List<string> { "The in-game mud datetime to format with long calendar and vague time display rules." },
            "Formats an in-game mud datetime using long calendar and vague time display rules. Null or Never mud datetimes display as Never.",
            "Date/Time",
            ProgVariableTypes.Text
        ));
    }
}
