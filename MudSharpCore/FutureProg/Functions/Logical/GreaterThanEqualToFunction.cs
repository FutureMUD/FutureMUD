using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;

namespace MudSharp.FutureProg.Functions.Logical;

internal class GreaterThanEqualToFunction : BinaryFunction
{
	private readonly ProgVariableTypeCode _comparisonType;

    public GreaterThanEqualToFunction(IFunction lhs, IFunction rhs)
        : base(lhs, rhs)
    {
		_comparisonType = (lhs.ReturnType & ~ProgVariableTypes.Literal).LegacyCode;
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

		switch (_comparisonType)
        {
            case ProgVariableTypeCode.Number:
                Result = new BooleanVariable((decimal)LHS.Result.GetObject >= (decimal)RHS.Result.GetObject);
                break;
            case ProgVariableTypeCode.TimeSpan:
                Result = new BooleanVariable((TimeSpan)LHS.Result.GetObject >= (TimeSpan)RHS.Result.GetObject);
                break;
            case ProgVariableTypeCode.DateTime:
                Result =
                    new BooleanVariable((System.DateTime)LHS.Result.GetObject >=
                                        (System.DateTime)RHS.Result.GetObject);
                break;
            case ProgVariableTypeCode.MudDateTime:
                Result =
                    new BooleanVariable((MudDateTime)LHS.Result.GetObject >= (MudDateTime)RHS.Result.GetObject);
                break;
        }

        return StatementResult.Normal;
    }
}
