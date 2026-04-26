using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class WordyNumberFunction : BuiltInFunction
{
    private readonly Func<int, string> _wordyNumberFunction;

    public WordyNumberFunction(IList<IFunction> parameters, Func<int, string> wordyNumberFunction)
        : base(parameters)
    {
        _wordyNumberFunction = wordyNumberFunction;
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Text;
        protected set { }
    }

    public override string ErrorMessage
    {
        get => ParameterFunctions.First().ErrorMessage;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        string result = _wordyNumberFunction((int)(decimal)ParameterFunctions.First().Result.GetObject);
        Result = new TextVariable(result);
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "tonumberwords",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new WordyNumberFunction(pars, NumberUtilities.ToWordyNumber),
            new List<string> { "number" },
            new List<string> { "The number to render as words." },
            "Converts a number to cardinal words, such as one hundred and five.",
            "Built-In",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "toordinalwords",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new WordyNumberFunction(pars, NumberUtilities.ToWordyOrdinal),
            new List<string> { "number" },
            new List<string> { "The number to render as words." },
            "Converts a number to ordinal words, such as one hundred and fifth.",
            "Built-In",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "toordinal",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new WordyNumberFunction(pars, NumberUtilities.ToOrdinal),
            new List<string> { "number" },
            new List<string> { "The number to render as words." },
            "Converts a number to ordinal text, such as 1st or 22nd.",
            "Built-In",
            ProgVariableTypes.Text
        ));
    }
}
