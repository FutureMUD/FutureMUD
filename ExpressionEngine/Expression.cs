﻿using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using NCalc;
using NCalc.Exceptions;
using NCalc.Handlers;

namespace ExpressionEngine
{
		public class Expression : IExpression
	{
		public static event EventHandler<string> ExpressionError;
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

			try
			{
				return _expression.Evaluate();
			}
			catch (NCalcFunctionNotFoundException e)
			{
				Console.WriteLine($"Exception in expression {OriginalExpression}:\n\nFunction Not Found: {e.FunctionName}\n\n{e.ToString()}");
				ExpressionError?.Invoke(this, $"Exception in expression {OriginalExpression}:\n\nFunction Not Found: {e.FunctionName}\n\n{e.ToString()}");
				return 0.0;
			}
			catch (NCalcParameterNotDefinedException e)
			{
				Console.WriteLine($"Exception in expression {OriginalExpression}:\n\nParameter Not Defined: {e.ParameterName}\n\n{e.ToString()}");
				ExpressionError?.Invoke(this, $"Exception in expression {OriginalExpression}:\n\nParameter Not Defined: {e.ParameterName}\n\n{e.ToString()}");
				return 0.0;
			}
			catch (NCalcParserException e)
			{
				Console.WriteLine($"Exception in expression {OriginalExpression}:\n\n{e.ToString()}");
				ExpressionError?.Invoke(this, $"Exception in expression {OriginalExpression}:\n\n{e.ToString()}");
				return 0.0;
			}
			catch (NCalcEvaluationException e)
			{
				Console.WriteLine($"Exception in expression {OriginalExpression}:\n\n{e.ToString()}");
				ExpressionError?.Invoke(this, $"Exception in expression {OriginalExpression}:\n\n{e.ToString()}");
				return 0.0;
			}
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

			try
			{
				return _expression.Evaluate();
			}
			catch (NCalcFunctionNotFoundException e)
			{
				Console.WriteLine($"Exception in expression {OriginalExpression}:\n\nFunction Not Found: {e.FunctionName}\n\n{e.ToString()}");
				ExpressionError?.Invoke(this, $"Exception in expression {OriginalExpression}:\n\nFunction Not Found: {e.FunctionName}\n\n{e.ToString()}");
				return 0.0;
			}
			catch (NCalcParameterNotDefinedException e)
			{
				Console.WriteLine($"Exception in expression {OriginalExpression}:\n\nParameter Not Defined: {e.ParameterName}\n\n{e.ToString()}");
				ExpressionError?.Invoke(this, $"Exception in expression {OriginalExpression}:\n\nParameter Not Defined: {e.ParameterName}\n\n{e.ToString()}");
				return 0.0;
			}
			catch (NCalcParserException e)
			{
				Console.WriteLine($"Exception in expression {OriginalExpression}:\n\n{e.ToString()}");
				ExpressionError?.Invoke(this, $"Exception in expression {OriginalExpression}:\n\n{e.ToString()}");
				return 0.0;
			}
			catch (NCalcEvaluationException e)
			{
				Console.WriteLine($"Exception in expression {OriginalExpression}:\n\n{e.ToString()}");
				ExpressionError?.Invoke(this, $"Exception in expression {OriginalExpression}:\n\n{e.ToString()}");
				return 0.0;
			}
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

		public string Error => _expression.Error?.Message ?? string.Empty;

		public IEnumerable<string> ParameterNames => _expression.GetParameterNames();

		#region Constructors
		public Expression(string expression) : this(expression, ExpressionOptions.CaseInsensitiveStringComparer | ExpressionOptions.IgnoreCaseAtBuiltInFunctions | ExpressionOptions.AllowBooleanCalculation)
		{
		}

		protected Expression(string expression, ExpressionOptions options)
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
			_expression.EvaluateFunction += NotFunction;

			if (!_expression.HasErrors())
			{
				foreach (var parameter in _expression.GetParameterNames())
				{
					_expression.Parameters[parameter] = 0.0;
				}
			}
		}
		#endregion

		#region In-built functions

		private void NotFunction(string name, FunctionArgs args)
		{
			if (!name.Equals("not", StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			if (args.Parameters.Length != 1)
			{
				throw new ArgumentException("Not() takes exactly 1 argument");
			}

			var value = Convert.ToDouble(args.Parameters[0].Evaluate());
			args.Result = value == 0.0 ? 1.0 : 0.0;
		}
		private void DRandFunction(string name, FunctionArgs args)
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

		private void RandFunction(string name, FunctionArgs args)
		{
			if (!name.Equals("rand", StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			if (args.Parameters.Length != 2)
			{
				throw new ArgumentException("Rand() takes exactly 2 arguments");
			}

			var arg1 = args.Parameters[0].Evaluate();
			var arg2 = args.Parameters[1].Evaluate();

			if (arg1 is null || arg2 is null)
			{
				args.Result = 0;
				return;
			}

			if (arg1 is double arg1d && arg2 is double arg2d)
			{
				args.Result = (RandomInstance.NextDouble() * (arg2d - arg1d)) + arg1d;
				return;
			}

			if (
				arg1 is string arg1s && arg2 is string arg2s && 
				!int.TryParse(arg1s, out _) && !int.TryParse(arg2s, out _) &&
				double.TryParse(arg1s, out var arg1d2) && double.TryParse(arg2s, out var arg2d2)
			)
			{
				args.Result = (RandomInstance.NextDouble() * (arg2d2 - arg1d2)) + arg1d2;
				return;
			}

			var randleft = Convert.ToInt32(args.Parameters[0].Evaluate());
			var randright = Convert.ToInt32(args.Parameters[1].Evaluate());
			args.Result = RandomInstance.Next(randleft, randright + 1);
		}

		private void DiceFunction(string name, FunctionArgs args)
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
