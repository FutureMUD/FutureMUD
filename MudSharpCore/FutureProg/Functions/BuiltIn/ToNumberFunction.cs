using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToNumberFunction : BuiltInFunction
{
    public ToNumberFunction(IList<IFunction> parameters)
        : base(parameters)
    {
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

        Result = decimal.TryParse(Convert.ToString(ParameterFunctions.First().Result.GetObject), out decimal value)
            ? new NumberVariable(value)
            : ParameterFunctions.Last().Result;

        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "tonumber",
            new[] { ProgVariableTypes.Text },
            (pars, gameworld) =>
                new ToNumberFunction(
                    pars.Concat(new IFunction[] { new ConstantFunction(new NumberVariable(0)) }).ToList()),
            new List<string> { "text" },
            new List<string> { "The text to parse." },
            "Parses text into a number. If parsing fails, the result is 0.",
            "Built-In",
            ProgVariableTypes.Number
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "tonumber",
            new[] { ProgVariableTypes.Text, ProgVariableTypes.Number },
            (pars, gameworld) => new ToNumberFunction(pars),
            new List<string> { "text", "fallback" },
            new List<string> { "The text to parse.", "The number to return if the text cannot be parsed." },
            "Parses text into a number, returning the supplied fallback number when parsing fails.",
            "Built-In",
            ProgVariableTypes.Number
        ));
    }
}