using System;
using System.Collections.Generic;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SetCondition : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "setcondition",
                new[] { ProgVariableTypes.Item, ProgVariableTypes.Number },
                (pars, gameworld) => new SetCondition(pars, gameworld),
                new List<string> { "item", "condition" },
                new List<string>
                {
                    "The item whose condition you wish to set", 
                    "The condition of the item, between 0.0 and 1.0"
                },
                "This function sets the condition percentage of an item to whatever amount you specify.",
                "Items",
                ProgVariableTypes.Boolean
            )
        );
    }

    #endregion

    #region Constructors

    protected SetCondition(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
    {
        Gameworld = gameworld;
    }

    #endregion

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

        var item = (IGameItem)ParameterFunctions[0].Result?.GetObject;
        if (item == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        var condition = Convert.ToDouble(ParameterFunctions[1].Result?.GetObject ?? 0.0);
        if (condition < 0.0)
        {
            condition = 0.0;
        }
        if (condition > 1.0)
        {
            condition = 1.0;
        }

        item.Condition = condition;
        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }
}
