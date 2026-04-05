using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SetOnFunction : BuiltInFunction
{
    private SetOnFunction(IList<IFunction> parameters)
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

        if (ParameterFunctions[0].Result is not IGameItem itemFunction)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        IOnOff openable = itemFunction.GetItemType<IOnOff>();
        if (openable != null)
        {
            bool targetState = (bool?)ParameterFunctions[1].Result.GetObject ?? true;
            bool echo = (bool?)ParameterFunctions[2].Result.GetObject ?? true;
            if (openable.SwitchedOn == targetState)
            {
                Result = new BooleanVariable(false);
                return StatementResult.Normal;
            }

            openable.SwitchedOn = targetState;
            Result = new BooleanVariable(true);
            return StatementResult.Normal;
        }

        Result = new BooleanVariable(false);
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "seton",
            new[] { ProgVariableTypes.Item, ProgVariableTypes.Boolean, ProgVariableTypes.Boolean },
            (pars, gameworld) => new SetOnFunction(pars)
        ));
    }
}