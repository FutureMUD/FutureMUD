using MudSharp.Form.Shape;
using MudSharp.FutureProg.Variables;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToTextFunction : BuiltInFunction
{
    public ToTextFunction(IList<IFunction> parameters)
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

        object outcome = ParameterFunctions.First().Result?.GetObject;
        if (outcome is null)
        {
            Result = new TextVariable(string.Empty);
            return StatementResult.Normal;
        }

        switch (outcome)
        {
            case bool b:
                Result = new TextVariable(b.ToString());
                return StatementResult.Normal;
            case decimal d:
                Result = new TextVariable(d.ToString());
                return StatementResult.Normal;
            case Gender g:
                Result = new TextVariable(Gendering.Get(g).GenderClass());
                return StatementResult.Normal;
        }

        Result = new TextVariable(outcome.ToString());
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "totext",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new ToTextFunction(pars),
            new List<string> { "value" },
            new List<string> { "The value to convert or inspect." },
            "Converts the supplied value to text using the standard FutureProg conversion rules.",
            "Built-In",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "totext",
            new[] { ProgVariableTypes.Boolean },
            (pars, gameworld) => new ToTextFunction(pars),
            new List<string> { "value" },
            new List<string> { "The value to convert or inspect." },
            "Converts the supplied value to text using the standard FutureProg conversion rules.",
            "Built-In",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "totext",
            new[] { ProgVariableTypes.Gender },
            (pars, gameworld) => new ToTextFunction(pars),
            new List<string> { "value" },
            new List<string> { "The value to convert or inspect." },
            "Converts the supplied value to text using the standard FutureProg conversion rules.",
            "Built-In",
            ProgVariableTypes.Text
        ));
    }
}