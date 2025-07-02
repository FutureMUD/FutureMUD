namespace ExpressionEngine
{
    public interface IExpression
    {
        string OriginalExpression { get; }
        System.Collections.Generic.Dictionary<string, object> Parameters { get; set; }
        object Evaluate();
        object EvaluateWith(params (string Name, object Value)[] values);
        double EvaluateDouble();
        double EvaluateDoubleWith(params (string Name, object Value)[] values);
        decimal EvaluateDecimal();
        decimal EvaluateDecimalWith(params (string Name, object Value)[] values);
        bool HasErrors();
        string Error { get; }
        System.Collections.Generic.IEnumerable<string> ParameterNames { get; }
    }
}
