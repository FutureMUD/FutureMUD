using NCalc;
using NCalc.Exceptions;
using NCalc.Extensions;
using NCalc.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;

namespace ExpressionEngine
{
    public class Expression : IExpression
    {
        public const int MaximumDiceCount = 10000;
        public const int MaximumDiceSides = 100000;

        public static event EventHandler<string> ExpressionError;
        private static readonly Random SystemRandom = new();
        private static readonly AsyncLocal<Random> AmbientRandom = new();
        public static Random RandomInstance => AmbientRandom.Value ?? SystemRandom;

        public static IDisposable PushRandom(Random random)
        {
            ArgumentNullException.ThrowIfNull(random);
            var previous = AmbientRandom.Value;
            AmbientRandom.Value = random;
            return new AmbientRandomScope(previous);
        }

        private sealed class AmbientRandomScope(Random previous) : IDisposable
        {
            private bool _disposed;

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                AmbientRandom.Value = previous;
                _disposed = true;
            }
        }

        private readonly NCalc.Expression _parsedExpression;
        private readonly ExpressionOptions _options;
        private readonly IReadOnlyList<string> _parameterNames;
        private readonly bool _hasErrors;

        public string OriginalExpression { get; private set; }

        private static readonly Regex _regex = new(@"(?:(?<numdice>\d+)d(?<sides>\d+))", RegexOptions.IgnoreCase);

        public object Evaluate()
        {
            return EvaluateWith(Array.Empty<(string Name, object Value)>());
        }

        public object EvaluateWith(IReadOnlyDictionary<string, object> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return EvaluateWith(values.Select(x => (x.Key, x.Value)).ToArray());
        }

        public object EvaluateWith(params (string Name, object Value)[] values)
        {
            ArgumentNullException.ThrowIfNull(values);

            if (_hasErrors)
            {
                return ReportError(new NCalcParserException(_parsedExpression.Error?.Message ?? "The expression has parsing errors."));
            }

            var expression = CreateEvaluationExpression(values);

            try
            {
                return expression.Evaluate();
            }
            catch (NCalcFunctionNotFoundException e)
            {
                return ReportError($"Exception in expression {OriginalExpression}:\n\nFunction Not Found: {e.FunctionName}\n\n{e}");
            }
            catch (NCalcParameterNotDefinedException e)
            {
                return ReportError($"Exception in expression {OriginalExpression}:\n\nParameter Not Defined: {e.ParameterName}\n\n{e}");
            }
            catch (NCalcParserException e)
            {
                return ReportError($"Exception in expression {OriginalExpression}:\n\n{e}");
            }
            catch (NCalcEvaluationException e)
            {
                return ReportError($"Exception in expression {OriginalExpression}:\n\n{e}");
            }
            catch (Exception e) when (e is ArgumentException or OverflowException or InvalidOperationException)
            {
                return ReportError($"Exception in expression {OriginalExpression}:\n\n{e}");
            }
        }

        public double EvaluateDouble()
        {
            return Convert.ToDouble(Evaluate());
        }

        public double EvaluateDoubleWith(IReadOnlyDictionary<string, object> values)
        {
            return Convert.ToDouble(EvaluateWith(values));
        }

        public decimal EvaluateDecimal()
        {
            return Convert.ToDecimal(Evaluate());
        }

        public double EvaluateDoubleWith(params (string Name, object Value)[] values)
        {
            return Convert.ToDouble(EvaluateWith(values));
        }

        public decimal EvaluateDecimalWith(params (string Name, object Value)[] values)
        {
            return Convert.ToDecimal(EvaluateWith(values));
        }

        public decimal EvaluateDecimalWith(IReadOnlyDictionary<string, object> values)
        {
            return Convert.ToDecimal(EvaluateWith(values));
        }

        public bool HasErrors()
        {
            return _hasErrors;
        }

        public string Error => _parsedExpression.Error?.Message ?? string.Empty;

        public IEnumerable<string> ParameterNames => _parameterNames;

        #region Constructors
        public Expression(string expression) : this(expression, ExpressionOptions.CaseInsensitiveStringComparer | ExpressionOptions.IgnoreCaseAtBuiltInFunctions | ExpressionOptions.AllowBooleanCalculation)
        {
        }

        protected Expression(string expression, ExpressionOptions options)
        {
            OriginalExpression = expression;
            _options = options;
            string parsed = _regex.Replace(expression, m =>
            {
                return $"dice({m.Groups["numdice"].Value},{m.Groups["sides"].Value})";
            });
            _parsedExpression = new NCalc.Expression(parsed, options);
            _hasErrors = _parsedExpression.HasErrors();
            _parameterNames = _hasErrors
                ? Array.Empty<string>()
                : _parsedExpression.GetParameterNames();
        }
        #endregion

        private NCalc.Expression CreateEvaluationExpression(IEnumerable<(string Name, object Value)> values)
        {
            var expression = new NCalc.Expression(_parsedExpression.LogicalExpression!, _options);
            expression.EvaluateFunction += DRandFunction;
            expression.EvaluateFunction += RandFunction;
            expression.EvaluateFunction += DiceFunction;
            expression.EvaluateFunction += NotFunction;

            foreach (var parameter in _parameterNames)
            {
                expression.Parameters[parameter] = 0.0;
            }

            foreach (var (name, value) in values)
            {
                expression.Parameters[name] = value is Enum ? Convert.ToInt64(value) : value;
            }

            return expression;
        }

        private object ReportError(Exception exception)
        {
            return ReportError($"Exception in expression {OriginalExpression}:\n\n{exception}");
        }

        private object ReportError(string message)
        {
            Console.WriteLine(message);
            ExpressionError?.Invoke(this, message);
            return 0.0;
        }

        #region In-built functions

        private void NotFunction(string name, FunctionEventArgs args)
        {
            if (!name.Equals("not", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (args.Parameters.Count != 1)
            {
                throw new ArgumentException("Not() takes exactly 1 argument");
            }

            double value = Convert.ToDouble(args.Parameters[0].Evaluate(args.Context));
            if (!double.IsFinite(value))
            {
                throw new ArgumentException("Not() requires a finite numeric argument");
            }

            args.Result = value == 0.0 ? 1.0 : 0.0;
        }
        private void DRandFunction(string name, FunctionEventArgs args)
        {
            if (!name.Equals("drand", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (args.Parameters.Count != 2)
            {
                throw new ArgumentException("DRand() takes exactly 2 arguments");
            }

            double randleft = Convert.ToDouble(args.Parameters[0].Evaluate(args.Context));
            double randright = Convert.ToDouble(args.Parameters[1].Evaluate(args.Context));
            if (!double.IsFinite(randleft) || !double.IsFinite(randright))
            {
                throw new ArgumentException("DRand() requires finite numeric arguments");
            }

            args.Result = (RandomInstance.NextDouble() * (randright - randleft)) + randleft;
        }

        private void RandFunction(string name, FunctionEventArgs args)
        {
            if (!name.Equals("rand", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (args.Parameters.Count != 2)
            {
                throw new ArgumentException("Rand() takes exactly 2 arguments");
            }

            var arg1 = args.Parameters[0].Evaluate(args.Context);
            var arg2 = args.Parameters[1].Evaluate(args.Context);

            if (arg1 is null || arg2 is null)
            {
                args.Result = 0;
                return;
            }

            if (arg1 is double arg1d && arg2 is double arg2d)
            {
                if (!double.IsFinite(arg1d) || !double.IsFinite(arg2d))
                {
                    throw new ArgumentException("Rand() requires finite numeric arguments");
                }

                args.Result = (RandomInstance.NextDouble() * (arg2d - arg1d)) + arg1d;
                return;
            }

            if (
                arg1 is string arg1s && arg2 is string arg2s &&
                !int.TryParse(arg1s, out _) && !int.TryParse(arg2s, out _) &&
                double.TryParse(arg1s, out double arg1d2) && double.TryParse(arg2s, out double arg2d2)
            )
            {
                args.Result = (RandomInstance.NextDouble() * (arg2d2 - arg1d2)) + arg1d2;
                return;
            }

            int randleft = Convert.ToInt32(args.Parameters[0].Evaluate(args.Context));
            int randright = Convert.ToInt32(args.Parameters[1].Evaluate(args.Context));
            args.Result = RandomInstance.Next(randleft, randright + 1);
        }

        private void DiceFunction(string name, FunctionEventArgs args)
        {
            if (!name.Equals("dice", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (args.Parameters.Count != 2)
            {
                throw new ArgumentException("Dice() takes exactly 2 arguments");
            }

            int left = Convert.ToInt32(args.Parameters[0].Evaluate(args.Context));
            int right = Convert.ToInt32(args.Parameters[1].Evaluate(args.Context));
            if (left < 0 || left > MaximumDiceCount || right <= 0 || right > MaximumDiceSides)
            {
                throw new ArgumentException($"Dice() requires 0-{MaximumDiceCount} dice and 1-{MaximumDiceSides} sides");
            }

            int result = 0;
            if (left > 0)
            {
                for (int i = 0; i < left; i++)
                {
                    result += RandomInstance.Next(1, right + 1);
                }
            }
            args.Result = result;
        }
        #endregion
    }
}
