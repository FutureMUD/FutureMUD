﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ExpressionEngine
{
    public class Expression
    {
        public static Random RandomInstance { get; } = new();

        private NCalc.Expression _expression;

        public string OriginalExpression { get; private set; }

        private Dictionary<string, object> _parameters;

        public Dictionary<string, object> Parameters
        {
            get { return _parameters ??= new Dictionary<string, object>(); }
            set { _parameters = value; }
        }

        private static readonly Regex _regex = new(@"(?:(?<numdice>\d+)d(?<sides>\d+))", RegexOptions.IgnoreCase);

        public object Evaluate()
        {
            foreach (var parameter in Parameters)
            {
                if (parameter.Value is Enum)
                {
                    _expression.Parameters[parameter.Key] = Convert.ToInt64(parameter.Value);
                    continue;
                }
                _expression.Parameters[parameter.Key] = parameter.Value;
            }
            return _expression.Evaluate();
        }

        public double EvaluateDouble()
        {
            return Convert.ToDouble(Evaluate());
        }

        public decimal EvaluateDecimal()
        {
            return Convert.ToDecimal(Evaluate());
        }

        public object EvaluateWith(params (string Name, object Value)[] values)
        {
            foreach (var parameter in Parameters)
            {
                _expression.Parameters[parameter.Key] = parameter.Value;
            }

            foreach (var parameter in values)
            {
                _expression.Parameters[parameter.Name] = parameter.Value;
            }

            return _expression.Evaluate();
        }

        public double EvaluateDoubleWith(params (string Name, object Value)[] values)
        {
            return Convert.ToDouble(EvaluateWith(values));
        }

        public decimal EvaluateDecimalWith(params (string Name, object Value)[] values)
        {
            return Convert.ToDecimal(EvaluateWith(values));
        }

        public bool HasErrors()
        {
            return _expression.HasErrors();
        }

        public string Error => _expression.Error;
        

        #region Constructors
        public Expression(string expression) : this(expression, NCalc.EvaluateOptions.IgnoreCase)
        {
        }

        public Expression(string expression, NCalc.EvaluateOptions options)
        {
            OriginalExpression = expression;
            var parsed = _regex.Replace(expression, m =>
            {
                return $"dice({m.Groups["numdice"].Value},{m.Groups["sides"].Value})";
            });
            _expression = new NCalc.Expression(parsed, options);
            _expression.EvaluateFunction += DRandFunction;
            _expression.EvaluateFunction += RandFunction;
            _expression.EvaluateFunction += DiceFunction;
        }
        #endregion

        #region In-built functions
        private void DRandFunction(string name, NCalc.FunctionArgs args)
        {
            if (!name.Equals("drand", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (args.Parameters.Length != 2)
            {
                throw new ArgumentException("DRand() takes exactly 2 arguments");
            }

            var randleft = Convert.ToDouble(args.Parameters[0].Evaluate());
            var randright = Convert.ToDouble(args.Parameters[1].Evaluate());
            args.Result = (RandomInstance.NextDouble() * (randright - randleft)) + randleft;
        }

        private void RandFunction(string name, NCalc.FunctionArgs args)
        {
            if (!name.Equals("rand", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (args.Parameters.Length != 2)
            {
                throw new ArgumentException("Rand() takes exactly 2 arguments");
            }

            var randleft = Convert.ToInt32(args.Parameters[0].Evaluate());
            var randright = Convert.ToInt32(args.Parameters[1].Evaluate());
            args.Result = RandomInstance.Next(randleft, randright + 1);
        }

        private void DiceFunction(string name, NCalc.FunctionArgs args)
        {
            if (!name.Equals("dice", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (args.Parameters.Length != 2)
            {
                throw new ArgumentException("Dice() takes exactly 2 arguments");
            }

            var left = Convert.ToInt32(args.Parameters[0].Evaluate());
            var right = Convert.ToInt32(args.Parameters[1].Evaluate());
            var result = 0;
            if (left > 0)
            {
                for (var i = 0; i < left; i++)
                {
                    result += RandomInstance.Next(1, right + 1);
                }
            }
            args.Result = result;
        }
        #endregion
    }
}
