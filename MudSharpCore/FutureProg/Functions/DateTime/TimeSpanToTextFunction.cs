using MudSharp.Accounts;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class TimeSpanToTextFunction : BuiltInFunction
{
    private TimeSpanToTextFunction(IList<IFunction> parameters)
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


        TimeSpan timespan = (TimeSpan)ParameterFunctions[0].Result.GetObject;
        Result = ParameterFunctions.Count == 1
            ? new TextVariable(timespan.Describe())
            : new TextVariable(timespan.Describe(((IHaveAccount)ParameterFunctions[1].Result.GetObject).Account));
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "totext",
            new[] { ProgVariableTypes.TimeSpan },
            (pars, gameworld) => new TimeSpanToTextFunction(pars),
            new List<string> { "value" },
            new List<string> { "The value to convert or inspect." },
            "Formats a duration as readable text, optionally using the supplied character account culture and formatting preferences.",
            "Date/Time",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "totext",
            new[] { ProgVariableTypes.TimeSpan, ProgVariableTypes.Toon },
            (pars, gameworld) => new TimeSpanToTextFunction(pars),
            new List<string> { "value", "voyeur" },
            new List<string> { "The value to convert or inspect.", "The character whose account culture and formatting preferences should be used." },
            "Formats a duration as readable text, optionally using the supplied character account culture and formatting preferences.",
            "Date/Time",
            ProgVariableTypes.Text
        ));
    }
}