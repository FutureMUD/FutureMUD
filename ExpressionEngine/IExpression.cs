namespace ExpressionEngine
{
    public interface IExpression
    {
        string OriginalExpression { get; }
        object Evaluate();
        object EvaluateWith(System.Collections.Generic.IReadOnlyDictionary<string, object> values);
        object EvaluateWith(params (string Name, object Value)[] values);
        double EvaluateDouble();
        double EvaluateDoubleWith(System.Collections.Generic.IReadOnlyDictionary<string, object> values);
        double EvaluateDoubleWith(params (string Name, object Value)[] values);
        decimal EvaluateDecimal();
        decimal EvaluateDecimalWith(System.Collections.Generic.IReadOnlyDictionary<string, object> values);
        decimal EvaluateDecimalWith(params (string Name, object Value)[] values);
        bool HasErrors();
        string Error { get; }
        System.Collections.Generic.IEnumerable<string> ParameterNames { get; }
		System.Collections.Generic.IEnumerable<string> FunctionNames { get; }
		/// <summary>Evaluates a finite numeric result without substituting zero or logging on failure.</summary>
		bool TryEvaluateDoubleWith(System.Collections.Generic.IReadOnlyDictionary<string, object> values,
			out double result, out string error);
    }
}
