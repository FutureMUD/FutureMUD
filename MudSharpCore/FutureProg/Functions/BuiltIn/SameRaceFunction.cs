using MudSharp.Character.Heritage;
using MudSharp.FutureProg.Variables;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class SameRaceFunction : BuiltInFunction
{
    public SameRaceFunction(IList<IFunction> parameters)
        : base(parameters)
    {
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Boolean;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        IRace race2 = ParameterFunctions[1].Result.GetObject as IRace;

        if (ParameterFunctions[0].Result.GetObject is not IRace race1)
        {
            ErrorMessage = "The first race in the SameRace function cannot be null";
            return StatementResult.Error;
        }

        Result = new BooleanVariable(race1.SameRace(race2));
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "samerace",
                new[] { ProgVariableTypes.Race, ProgVariableTypes.Race },
                (pars, gameworld) => new SameRaceFunction(pars),
                new List<string> { "race1", "race2" },
                new List<string> { "The first race to compare.", "The second race to compare." },
                "Returns true when both race references point to the same race object. Null races simply compare as not the same.",
                "Built-In",
                ProgVariableTypes.Boolean
            )
        );
    }
}
