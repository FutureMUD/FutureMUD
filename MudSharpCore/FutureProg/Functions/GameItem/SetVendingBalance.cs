using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SetVendingBalance : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "SetVendingBalance".ToLowerInvariant(),
                new[] { ProgVariableTypes.Item, ProgVariableTypes.Number },
                (pars, gameworld) => new SetVendingBalance(pars, gameworld),
                new List<string> { "item", "balance" },
                new List<string> { "The vending machine item whose stored balance should be changed.", "The new numeric balance to store on the machine." },
                "Sets the current balance stored in a vending-machine item component. Returns false if the item is null, is not a vending machine, or the balance is not numeric.",
                "Items",
                ProgVariableTypes.Boolean
            )
        );
    }

    #endregion

    #region Constructors

    protected SetVendingBalance(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

        if (ParameterFunctions[0].Result?.GetObject is not IGameItem item)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        if (ParameterFunctions[1].Result?.GetObject is not decimal balance)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        IVendingMachine vendingMachine = item.GetItemType<IVendingMachine>();
        if (vendingMachine == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        vendingMachine.CurrentBalance = balance;
        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }
}
