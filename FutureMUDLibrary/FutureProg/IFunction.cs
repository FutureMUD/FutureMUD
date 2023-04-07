namespace MudSharp.FutureProg {
    public enum FunctionType {
        BinaryLogicComparer,
        BinaryLogicCombiner,
        BinaryOperation,
        BuiltInFunction,
        UserDefinedFunction,
        CollectionExtensionFunction,
        DotReferenceOperator,
        VariableReference,
        TextLiteral,
        NumberLiteral,
        BooleanLiteral,
        Error
    }

    public interface IFunction : IStatement {
        IFutureProgVariable Result { get; }
        FutureProgVariableTypes ReturnType { get; }
    }
}