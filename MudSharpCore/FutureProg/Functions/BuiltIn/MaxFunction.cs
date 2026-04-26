using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class MaxFunction : BuiltInFunction
{
    public MaxFunction(IList<IFunction> parameters)
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

        Result =
            new NumberVariable(Math.Max((decimal)ParameterFunctions[0].Result.GetObject,
                (decimal)ParameterFunctions[1].Result.GetObject));
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "max",
            new[] { ProgVariableTypes.Number, ProgVariableTypes.Number },
            (pars, gameworld) => new MaxFunction(pars),
            new List<string> { "minimum", "maximum" },
            new List<string> { "The lower numeric bound or first number.", "The upper numeric bound or second number." },
            "Returns the higher of two numbers.",
            "Built-In",
            ProgVariableTypes.Number
        ));
    }
}