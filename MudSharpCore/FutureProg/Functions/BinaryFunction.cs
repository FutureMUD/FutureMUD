namespace MudSharp.FutureProg.Functions;

internal abstract class BinaryFunction : Function
{
    protected IFunction LHS;
    protected IFunction RHS;

    protected BinaryFunction(IFunction lhs, IFunction rhs)
    {
        LHS = lhs;
        RHS = rhs;
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        StatementResult lhsResult = LHS.Execute(variables);
        if (lhsResult == StatementResult.Error)
        {
            ErrorMessage = LHS.ErrorMessage;
            return StatementResult.Error;
        }

        StatementResult rhsResult = RHS.Execute(variables);
        if (rhsResult == StatementResult.Error)
        {
            ErrorMessage = RHS.ErrorMessage;
            return StatementResult.Error;
        }

        return StatementResult.Normal;
    }
}