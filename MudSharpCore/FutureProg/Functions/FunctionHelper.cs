using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions.BuiltIn;
using MudSharp.FutureProg.Functions.DateTime;
using MudSharp.FutureProg.Functions.Logical;
using MudSharp.FutureProg.Functions.Mathematical;
using MudSharp.FutureProg.Functions.Textual;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions;

public static class FunctionHelper
{
	private static readonly IEnumerable<string> _binaryLogicCombinerSplitStrings = new[] { " and ", " or ", " xor " };

	private static readonly IEnumerable<string> _binaryLogicOperationSplitString = new[]
		{ ">=", "<=", "!=", "<>", "~=", "==", "<", ">" };

	private static readonly IEnumerable<string> _binaryOperatorSplitString = new[] { "+", "-", "/", "*", "%", "^" };

	private static readonly Regex _collectionExtensionFunctionRegex =
		new(@"^([a-z][\w\d]*)\(([a-z][\w\d]*), (.+)\)$", RegexOptions.IgnoreCase);

	private static readonly Regex _indexerRegex =
		new(@"^\s*@?(?<variable>[a-z][\w]*)\[(?<key>[\""@\w]+)\](?:\s*\=\s* (?<assignment>[^=].+))*",
			RegexOptions.Multiline | RegexOptions.IgnoreCase);

	public static readonly Regex TimespanRegex =
		new(
			@"^ *(?:(?<days>[0-9]+)d ?)?(?:(?<hours>[0-9]+)h ?)?(?:(?<minutes>[0-9]+)m ?)?(?:(?<seconds>[0-9]+)s ?)?(?:(?<milliseconds>[0-9]+)f)? *$");

	public static string ColouriseFunction(string line, bool isDarkMode = false)
	{
		if (line.Length > 2 && line.First() == '(' && line.Last() == ')')
		{
			return $"({ColouriseFunction(line.Substring(1, line.Length - 2), isDarkMode)})";
		}

		using (var binaryLogicCombiners = BinaryStringSplit(line, _binaryLogicCombinerSplitStrings))
		{
			if (binaryLogicCombiners.IsError)
			{
				if (isDarkMode)
				{
					return line.ColourBold(Telnet.Magenta);
				}
				return line.ColourBold(Telnet.Magenta, Telnet.Black);
			}

			if (binaryLogicCombiners.IsFound)
			{
				return
					$"{ColouriseFunction(binaryLogicCombiners.LHS, isDarkMode)} {binaryLogicCombiners.MatchedSplitString.Colour(Telnet.KeywordBlue)} {ColouriseFunction(binaryLogicCombiners.RHS, isDarkMode)}";
			}
		}

		using (var binaryLogicComparers = BinaryStringSplit(line, _binaryLogicOperationSplitString))
		{
			if (binaryLogicComparers.IsFound)
			{
				return
					$"{ColouriseFunction(binaryLogicComparers.LHS, isDarkMode)} {binaryLogicComparers.MatchedSplitString} {ColouriseFunction(binaryLogicComparers.RHS, isDarkMode)}";
			}
		}

		using (var binaryOperators = BinaryStringSplit(line, _binaryOperatorSplitString))
		{
			if (binaryOperators.IsFound)
			{
				return
					$"{ColouriseFunction(binaryOperators.LHS, isDarkMode)} {binaryOperators.MatchedSplitString} {ColouriseFunction(binaryOperators.RHS, isDarkMode)}";
			}
		}

		using (var dotReferences = ReverseBinarySplit(line, '.'))
		{
			if (dotReferences.IsError)
			{
				if (isDarkMode)
				{
					return line.ColourBold(Telnet.Magenta);
				}
				return line.ColourBold(Telnet.Magenta, Telnet.Black);
			}

			if (dotReferences.IsFound && dotReferences.LHS.GetDouble() is null && dotReferences.RHS.GetDouble() is null)
			{
				var match = _collectionExtensionFunctionRegex.Match(dotReferences.RHS);
				if (match.Success)
				{
					if (isDarkMode)
					{
						return $"{ColouriseFunction(dotReferences.LHS, true)}{dotReferences.MatchedSplitString}{match.Groups[1].Value.Colour(Telnet.FunctionYellow)}({match.Groups[2].Value.Colour(Telnet.VariableCyan)}, {ColouriseFunction(match.Groups[3].Value, isDarkMode)})";
					}

					return $"{ColouriseFunction(dotReferences.LHS)}{dotReferences.MatchedSplitString}{match.Groups[1].Value}({match.Groups[2].Value}, {ColouriseFunction(match.Groups[3].Value)})";
				}

				if (isDarkMode)
				{
					return
						$"{ColouriseFunction(dotReferences.LHS, true)}{dotReferences.MatchedSplitString}{dotReferences.RHS.Colour(Telnet.VariableCyan)}";
				}

				return
					$"{ColouriseFunction(dotReferences.LHS)}{dotReferences.MatchedSplitString}{dotReferences.RHS}";
			}
		}

		using (var unaryOperators = UnaryStringSplit(line))
		{
			if (unaryOperators.IsFound)
			{
				var parameters = ParameterStringSplit(unaryOperators.FunctionContents, ',');
				if (parameters.IsError)
				{
					if (isDarkMode)
					{
						return line.ColourBold(Telnet.Magenta);
					}

					return line.ColourBold(Telnet.Magenta, Telnet.Black);
				}

				if (isDarkMode)
				{
					return $"{(unaryOperators.Type == UnaryFunctionType.UserDefinedFunction ? $"@{unaryOperators.FunctionName}" : unaryOperators.FunctionName).Colour(Telnet.FunctionYellow)}({(parameters.IsFound ? parameters.ParameterStrings.Select(x => ColouriseFunction(x, isDarkMode)).ListToString(conjunction: "", twoItemJoiner: ", ") : "")})";
				}
				return  $"{(unaryOperators.Type == UnaryFunctionType.UserDefinedFunction ? $"@{unaryOperators.FunctionName}" : unaryOperators.FunctionName)}({(parameters.IsFound ? parameters.ParameterStrings.Select(x => ColouriseFunction(x, isDarkMode)).ListToString(conjunction: "", twoItemJoiner: ", ") : "")})";
			}
		}

		using (var nonFunctions = NonFunctionStringSplit(line))
		{
			if (nonFunctions.IsError)
			{
				if (isDarkMode)
				{
					return line.ColourBold(Telnet.Magenta);
				}
				return line.ColourBold(Telnet.Magenta, Telnet.Black);
			}

			if (nonFunctions.IsFound)
			{
				switch (nonFunctions.Type)
				{
					case NonFunctionType.BooleanLiteral:
						if (isDarkMode)
						{
							return nonFunctions.StringValue.Colour(Telnet.KeywordBlue);
						}
						return $"{nonFunctions.StringValue.Colour(Telnet.Blue, Telnet.Black)}";

					case NonFunctionType.NumberLiteral:
						if (isDarkMode)
						{
							return nonFunctions.StringValue.Colour(Telnet.Green);
						}
						return $"{nonFunctions.StringValue.Colour(Telnet.Green, Telnet.Black)}";

					case NonFunctionType.TextLiteral:
						if (isDarkMode)
						{
							return $"{("\"" + nonFunctions.StringValue + "\"").Colour(Telnet.TextRed)}";
						}
						return $"{("\"" + nonFunctions.StringValue + "\"").Colour(Telnet.Red, Telnet.Black)}";

					case NonFunctionType.VariableReference:
						if (isDarkMode)
						{
							return $"@{nonFunctions.StringValue}".Colour(Telnet.VariableCyan);
						}
						return $"@{nonFunctions.StringValue}".Colour(Telnet.Cyan, Telnet.Black);

					case NonFunctionType.TimeSpanLiteral:
						if (isDarkMode)
						{
							return nonFunctions.StringValue.Colour(Telnet.FunctionYellow);
						}
						return nonFunctions.StringValue.Colour(Telnet.Blue, Telnet.Black);

					default:
						if (isDarkMode)
						{
							return line.ColourBold(Telnet.Magenta);
						}
						return line.ColourBold(Telnet.Magenta, Telnet.Black);
				}
			}
		}

		if (isDarkMode)
		{
			return line.ColourBold(Telnet.Magenta);
		}
		return line.ColourBold(Telnet.Magenta, Telnet.Black);
	}

	public static ICompileInfo CompileFunction(string line,
		IDictionary<string, FutureProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		// Create Factory Class for Return Information
		var compileInfoFactory = CompileInfo.GetFactory();

		#region Non Function Operations

		// Check to see whether the line is a Non Function Operation (e.g. literal, variable reference, etc.)
		var nonFunctions = NonFunctionStringSplit(line);

		// If we get an error, this function should return a compile error
		if (!nonFunctions.IsError && nonFunctions.IsFound)
		{
			switch (nonFunctions.Type)
			{
				case NonFunctionType.BooleanLiteral:
					return
						compileInfoFactory.CreateNew(
							new ConstantFunction(new BooleanVariable(Convert.ToBoolean(nonFunctions.StringValue))),
							lineNumber);

				case NonFunctionType.NumberLiteral:
					return
						compileInfoFactory.CreateNew(
							new ConstantFunction(new NumberVariable(Convert.ToDecimal(nonFunctions.StringValue))),
							lineNumber);

				case NonFunctionType.TextLiteral:
					return
						compileInfoFactory.CreateNew(
							new ConstantFunction(new TextVariable(nonFunctions.StringValue)), lineNumber);

				case NonFunctionType.TimeSpanLiteral:
					var tsMatch = TimespanRegex.Match(nonFunctions.StringValue);
					return compileInfoFactory.CreateNew(new ConstantFunction(new TimeSpanVariable(new TimeSpan(
							tsMatch.Groups["days"].Length > 0 ? int.Parse(tsMatch.Groups["days"].Value) : 0,
							tsMatch.Groups["hours"].Length > 0 ? int.Parse(tsMatch.Groups["hours"].Value) : 0,
							tsMatch.Groups["minutes"].Length > 0 ? int.Parse(tsMatch.Groups["minutes"].Value) : 0,
							tsMatch.Groups["seconds"].Length > 0 ? int.Parse(tsMatch.Groups["seconds"].Value) : 0,
							tsMatch.Groups["milliseconds"].Length > 0
								? int.Parse(tsMatch.Groups["milliseconds"].Value)
								: 0
						))),
						lineNumber);

				case NonFunctionType.VariableReference:
					if (variableSpace.ContainsKey(nonFunctions.StringValue.ToLowerInvariant()))
					{
						return
							compileInfoFactory.CreateNew(
								new VariableReferenceFunction(nonFunctions.StringValue.ToLowerInvariant(),
									variableSpace[nonFunctions.StringValue.ToLowerInvariant()]), lineNumber);
					}

					return
						compileInfoFactory.CreateError(
							$"Variable {nonFunctions.StringValue} is not defined.", lineNumber);
			}
		}

		#endregion Non Function Operations

		#region Binary Logic Combiners

		// Determine if this is a Binary Logic Combiner Function (e.g. and, or, xor)
		var binaryLogicCombiners = BinaryStringSplit(line, _binaryLogicCombinerSplitStrings);

		// If we get an error, this function should return a compile error
		if (binaryLogicCombiners.IsError)
		{
			return compileInfoFactory.CreateError("Unbalanced brackets or string literals", lineNumber);
		}

		// Binary Logic Combiners found
		if (binaryLogicCombiners.IsFound)
		{
			// Compile LHS and RHS and check for errors
			var lhsCompileInfo = CompileFunction(binaryLogicCombiners.LHS, variableSpace, lineNumber, gameworld);
			var rhsCompileInfo = CompileFunction(binaryLogicCombiners.RHS, variableSpace, lineNumber, gameworld);
			if (lhsCompileInfo.IsError)
			{
				return compileInfoFactory.CreateError($"Error with LHS of {binaryLogicCombiners.MatchedSplitString}:\n{lhsCompileInfo.ErrorMessage}", lineNumber);
			}

			if (rhsCompileInfo.IsError)
			{
				return compileInfoFactory.CreateError($"Error with RHS of {binaryLogicCombiners.MatchedSplitString}:\n{rhsCompileInfo.ErrorMessage}", lineNumber);
			}

			// No errors found, create and return logic function
			switch (binaryLogicCombiners.MatchedSplitString.Trim())
			{
				case "and":
					return
						compileInfoFactory.CreateNew(
							new AndFunction((IFunction)lhsCompileInfo.CompiledStatement,
								(IFunction)rhsCompileInfo.CompiledStatement), lineNumber);

				case "or":
					return
						compileInfoFactory.CreateNew(
							new OrFunction((IFunction)lhsCompileInfo.CompiledStatement,
								(IFunction)rhsCompileInfo.CompiledStatement), lineNumber);

				case "xor":
				default:
					throw new NotSupportedException();
			}
		}

		#endregion Binary Logic Combiners

		#region Binary Logic Comparers

		// Determine if this is a Binary Logic Comparer Function (e.g. <=, =, !=, etc.)
		var binaryLogicComparers = BinaryStringSplit(line, _binaryLogicOperationSplitString);

		// If we get an error, this function should return a compile error
		if (binaryLogicComparers.IsError)
		{
			return compileInfoFactory.CreateError("Unbalanced brackets or string literals", lineNumber);
		}

		// Binary Logic Combiners found
		if (binaryLogicComparers.IsFound)
		{
			// Compile LHS and RHS and check for errors
			var lhsCompileInfo = CompileFunction(binaryLogicComparers.LHS, variableSpace, lineNumber, gameworld);
			var rhsCompileInfo = CompileFunction(binaryLogicComparers.RHS, variableSpace, lineNumber, gameworld);
			if (lhsCompileInfo.IsError)
			{
				return compileInfoFactory.CreateError($"Error with LHS of {binaryLogicComparers.MatchedSplitString}:\n{lhsCompileInfo.ErrorMessage}", lineNumber);
			}

			if (rhsCompileInfo.IsError)
			{
				return compileInfoFactory.CreateError($"Error with RHS of {binaryLogicComparers.MatchedSplitString}:\n{rhsCompileInfo.ErrorMessage}", lineNumber);
			}

			// Statements Compiled, Determine Functor Type
			var lhs = (IFunction)lhsCompileInfo.CompiledStatement;
			var rhs = (IFunction)rhsCompileInfo.CompiledStatement;

			FutureProgVariableTypes functorType;
			switch (binaryLogicComparers.MatchedSplitString)
			{
				case "==":
				case "!=":
				case "<>":
					functorType = FutureProgVariableTypes.CollectionItem;
					break;

				case "~=":
					functorType = FutureProgVariableTypes.Text;
					break;

				case ">":
				case ">=":
				case "<":
				case "<=":
					functorType = FutureProgVariableTypes.Number | FutureProgVariableTypes.DateTime |
					              FutureProgVariableTypes.TimeSpan | FutureProgVariableTypes.MudDateTime;
					break;

				default:
					throw new NotSupportedException();
			}

			// Check Functor Types are appropriate for the operator and also mutually equal
			if (!lhs.ReturnType.CompatibleWith(functorType))
			{
				return compileInfoFactory.CreateError("LHS is not correct return type.", lineNumber);
			}

			if (!rhs.ReturnType.CompatibleWith(functorType))
			{
				return compileInfoFactory.CreateError("RHS is not correct return type.", lineNumber);
			}

			if ((lhs.ReturnType & ~FutureProgVariableTypes.Literal) !=
			    (rhs.ReturnType & ~FutureProgVariableTypes.Literal))
			{
				return compileInfoFactory.CreateError("LHS and RHS do not compare the same values.", lineNumber);
			}

			// Statements are comparable, return success
			switch (binaryLogicComparers.MatchedSplitString)
			{
				case "==":
					return compileInfoFactory.CreateNew(new EqualityFunction(lhs, rhs), lineNumber);

				case "!=":
				case "<>":
					return
						compileInfoFactory.CreateNew(
							new LogicalNotFunction(new IFunction[] { new EqualityFunction(lhs, rhs) }), lineNumber);

				case "~=":
					return compileInfoFactory.CreateNew(new StringStartsWithFunction(lhs, rhs), lineNumber);

				case ">":
					return compileInfoFactory.CreateNew(new GreaterThanFunction(lhs, rhs), lineNumber);

				case ">=":
					return compileInfoFactory.CreateNew(new GreaterThanEqualToFunction(lhs, rhs), lineNumber);

				case "<":
					return compileInfoFactory.CreateNew(new LessThanFunction(lhs, rhs), lineNumber);

				case "<=":
					return compileInfoFactory.CreateNew(new LessThanEqualToFunction(lhs, rhs), lineNumber);

				default:
					throw new NotSupportedException();
			}
		}

		#endregion Binary Logic Comparers


		#region Indexers

		if (_indexerRegex.IsMatch(line))
		{
			var match = _indexerRegex.Match(line);
			var variable = match.Groups["variable"].Value.ToLowerInvariant();
			if (!variableSpace.ContainsKey(variable))
			{
				return compileInfoFactory.CreateError($"Indexer specified a non-existent variable: {variable}",
					lineNumber);
			}

			var indexFunction = CompileFunction(match.Groups["key"].Value, variableSpace, lineNumber, gameworld);
			if (indexFunction.IsError)
			{
				return compileInfoFactory.CreateError($"Index key in indexer had error: {indexFunction.ErrorMessage}",
					lineNumber);
			}

			ICompileInfo assignmentFunction = null;
			if (match.Groups["assignment"].Length > 0)
			{
				assignmentFunction =
					CompileFunction(match.Groups["assignment"].Value, variableSpace, lineNumber, gameworld);
				if (assignmentFunction.IsError)
				{
					return compileInfoFactory.CreateError(
						$"RHS in indexer had error: {assignmentFunction.ErrorMessage}", lineNumber);
				}
			}

			var variableType = variableSpace[variable];
			if (variableType.HasFlag(FutureProgVariableTypes.Collection))
			{
				if (!((IFunction)indexFunction.CompiledStatement).ReturnType.CompatibleWith(FutureProgVariableTypes
					    .Number))
				{
					return compileInfoFactory.CreateError($"Index key in indexer was not a number", lineNumber);
				}

				if (assignmentFunction == null)
				{
					return compileInfoFactory.CreateNew(
						new CollectionIndexFunction(variable, (IFunction)indexFunction.CompiledStatement,
							variableType & ~FutureProgVariableTypes.Collection), lineNumber);
				}

				return compileInfoFactory.CreateNew(
					new CollectionIndexAssigner(variable, (IFunction)indexFunction.CompiledStatement,
						(IFunction)assignmentFunction.CompiledStatement), lineNumber);
			}

			if (variableType.HasFlag(FutureProgVariableTypes.Dictionary))
			{
				var indexType = ((IFunction)indexFunction.CompiledStatement).ReturnType;
				if (!indexType.CompatibleWith(FutureProgVariableTypes.Number) &&
				    !indexType.CompatibleWith(FutureProgVariableTypes.Text))
				{
					return compileInfoFactory.CreateError($"Index key in indexer was not a text or number", lineNumber);
				}

				if (assignmentFunction == null)
				{
					return compileInfoFactory.CreateNew(
						new DictionaryIndexFunction(variable, (IFunction)indexFunction.CompiledStatement,
							variableType & ~FutureProgVariableTypes.Dictionary), lineNumber);
				}

				return compileInfoFactory.CreateNew(
					new DictionaryIndexAssigner(variable, (IFunction)indexFunction.CompiledStatement,
						(IFunction)assignmentFunction.CompiledStatement), lineNumber);
			}

			if (variableType.HasFlag(FutureProgVariableTypes.CollectionDictionary))
			{
				var indexType = ((IFunction)indexFunction.CompiledStatement).ReturnType;
				if (!indexType.CompatibleWith(FutureProgVariableTypes.Number) &&
				    !indexType.CompatibleWith(FutureProgVariableTypes.Text))
				{
					return compileInfoFactory.CreateError($"Index key in indexer was not a text or number", lineNumber);
				}

				if (assignmentFunction == null)
				{
					return compileInfoFactory.CreateNew(
						new CollectionDictionaryIndexFunction(variable, (IFunction)indexFunction.CompiledStatement,
							variableType & ~FutureProgVariableTypes.CollectionDictionary), lineNumber);
				}

				return compileInfoFactory.CreateError(
					$"CollectionDictionaries indexers can only be used to access the data for a key, they cannot be used for assignment",
					lineNumber);
			}

			return compileInfoFactory.CreateError($"Indexers are not supported for that type", lineNumber);
		}

		#endregion

		#region Binary Operators

		// Determine if this is a Binary Operator (e.g. +, -, *, etc.)
		var binaryOperators = BinaryStringSplit(line, _binaryOperatorSplitString);

		// If we get an error, this function should return a compile error
		if (binaryOperators.IsError)
		{
			return compileInfoFactory.CreateError("Unbalanced brackets or string literals", lineNumber);
		}

		// Binary Operators found
		if (binaryOperators.IsFound)
		{
			// Compile LHS and RHS and check for errors
			var lhsCompileInfo = CompileFunction(binaryOperators.LHS, variableSpace, lineNumber, gameworld);
			var rhsCompileInfo = CompileFunction(binaryOperators.RHS, variableSpace, lineNumber, gameworld);
			if (lhsCompileInfo.IsError)
			{
				return compileInfoFactory.CreateError($"Error with LHS of {binaryOperators.MatchedSplitString}:\n{lhsCompileInfo.ErrorMessage}", lineNumber);
			}

			if (rhsCompileInfo.IsError)
			{
				return compileInfoFactory.CreateError($"Error with RHS of {binaryOperators.MatchedSplitString}:\n{rhsCompileInfo.ErrorMessage}", lineNumber);
			}

			// Statements Compiled, Determine Functor Type
			var lhs = (IFunction)lhsCompileInfo.CompiledStatement;
			var rhs = (IFunction)rhsCompileInfo.CompiledStatement;

			switch (lhs.ReturnType & ~FutureProgVariableTypes.Literal)
			{
				case FutureProgVariableTypes.Number:
					if (!rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.Number) &&
					    !rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.Text))
					{
						return
							compileInfoFactory.CreateError(
								"A number on the LHS of a binary operator must have a number or text on the RHS.",
								lineNumber);
					}

					// Statements are comparable, return success
					switch (binaryOperators.MatchedSplitString)
					{
						case "+":
							return rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.Text)
								? compileInfoFactory.CreateNew(
									new StringConcatenationFunction(new ToTextFunction(new List<IFunction> { lhs }),
										rhs), lineNumber)
								: compileInfoFactory.CreateNew(new AdditionFunction(lhs, rhs), lineNumber);

						case "-":
							return compileInfoFactory.CreateNew(new SubtractionFunction(lhs, rhs), lineNumber);

						case "/":
							return compileInfoFactory.CreateNew(new DivisionFunction(lhs, rhs), lineNumber);

						case "*":
							return compileInfoFactory.CreateNew(new MultiplicationFunction(lhs, rhs), lineNumber);

						case "%":
							return compileInfoFactory.CreateNew(new ModulusFunction(lhs, rhs), lineNumber);

						case "^":
							return compileInfoFactory.CreateNew(new PowerFunction(lhs, rhs), lineNumber);

						default:
							throw new NotSupportedException();
					}

				case FutureProgVariableTypes.Text:
					if (binaryOperators.MatchedSplitString != "+")
					{
						return compileInfoFactory.CreateError("Text functions can only use the + binary operator.",
							lineNumber);
					}

					var rhsCoreType = rhs.ReturnType & ~FutureProgVariableTypes.Literal;
					if (rhsCoreType != FutureProgVariableTypes.Text &&
					    rhsCoreType != FutureProgVariableTypes.Number &&
					    rhsCoreType != FutureProgVariableTypes.Boolean &&
					    rhsCoreType != FutureProgVariableTypes.DateTime &&
					    rhsCoreType != FutureProgVariableTypes.TimeSpan)
					{
						return
							compileInfoFactory.CreateError(
								"Text on the LHS of a binary operator must have text, boolean, timespan, datetime or a number on the RHS.",
								lineNumber);
					}

					return
						compileInfoFactory.CreateNew(
							new StringConcatenationFunction(lhs,
								rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.Text)
									? rhs
									: new ToTextFunction(new List<IFunction> { rhs })), lineNumber);

				case FutureProgVariableTypes.DateTime:
					if (binaryOperators.MatchedSplitString != "+" && binaryOperators.MatchedSplitString != "-")
					{
						return
							compileInfoFactory.CreateError(
								"DateTime functions on the LHS can only use the - and + binary operators.",
								lineNumber);
					}

					if (binaryOperators.MatchedSplitString == "+" &&
					    !rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.TimeSpan))
					{
						return
							compileInfoFactory.CreateError(
								"DateTime functions can only have TimeSpans added to them.", lineNumber);
					}

					if (binaryOperators.MatchedSplitString == "-" &&
					    !rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.TimeSpan) &&
					    !rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.DateTime))
					{
						return
							compileInfoFactory.CreateError(
								"DateTime functions can only have TimeSpans or DateTimes subtracted from them.",
								lineNumber);
					}

					switch (binaryOperators.MatchedSplitString)
					{
						case "+":
							return compileInfoFactory.CreateNew(new DateTimeAdditionFunction(lhs, rhs), lineNumber);
						case "-":
							if (rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.DateTime))
							{
								return compileInfoFactory.CreateNew(new DateTimeDifferenceFunction(lhs, rhs),
									lineNumber);
							}

							return compileInfoFactory.CreateNew(new DateTimeSubtractionFunction(lhs, rhs),
								lineNumber);

						default:
							throw new NotSupportedException();
					}

				case FutureProgVariableTypes.MudDateTime:
					if (binaryOperators.MatchedSplitString != "+" && binaryOperators.MatchedSplitString != "-")
					{
						return
							compileInfoFactory.CreateError(
								"MudDateTime functions on the LHS can only use the - and + binary operators.",
								lineNumber);
					}

					if (binaryOperators.MatchedSplitString == "+" &&
					    !rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.TimeSpan))
					{
						return
							compileInfoFactory.CreateError(
								"MudDateTime functions can only have TimeSpans added to them.", lineNumber);
					}

					if (binaryOperators.MatchedSplitString == "-" &&
					    !rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.TimeSpan) &&
					    !rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.MudDateTime))
					{
						return
							compileInfoFactory.CreateError(
								"MudDateTime functions can only have TimeSpans or MudDateTime subtracted from them.",
								lineNumber);
					}

					switch (binaryOperators.MatchedSplitString)
					{
						case "+":
							return compileInfoFactory.CreateNew(new MudDateTimeAdditionFunction(lhs, rhs),
								lineNumber);
						case "-":
							if (rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.MudDateTime))
							{
								return compileInfoFactory.CreateNew(new MudDateTimeDifferenceFunction(lhs, rhs),
									lineNumber);
							}

							return compileInfoFactory.CreateNew(new MudDateTimeSubtractionFunction(lhs, rhs),
								lineNumber);

						default:
							throw new NotSupportedException();
					}

				case FutureProgVariableTypes.TimeSpan:

					switch (binaryOperators.MatchedSplitString)
					{
						case "+":
							if (!rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.TimeSpan))
							{
								return
									compileInfoFactory.CreateError(
										"TimeSpan functions can only have TimeSpans added to them.", lineNumber);
							}

							return compileInfoFactory.CreateNew(new TimeSpanAdditionFunction(lhs, rhs), lineNumber);

						case "-":
							if (!rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.TimeSpan))
							{
								return
									compileInfoFactory.CreateError(
										"TimeSpan functions can only have TimeSpans subtracted from them.",
										lineNumber);
							}

							return compileInfoFactory.CreateNew(new TimeSpanSubtractionFunction(lhs, rhs),
								lineNumber);

						case "*":
							if (!rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
							{
								return
									compileInfoFactory.CreateError(
										"TimeSpan functions can only be multiplied by Numbers.", lineNumber);
							}

							return compileInfoFactory.CreateNew(
								new TimeSpanMultiplicationByNumberFunction(lhs, rhs), lineNumber);

						case "/":
							if (!rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.Number) &&
							    !rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.TimeSpan))
							{
								return
									compileInfoFactory.CreateError(
										"TimeSpan functions can only be divided by Numbers or TimeSpans.",
										lineNumber);
							}

							if (rhs.ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
							{
								return compileInfoFactory.CreateNew(new TimeSpanDivisionByNumberFunction(lhs, rhs),
									lineNumber);
							}

							return compileInfoFactory.CreateNew(new TimeSpanDivisionByTimeSpanFunction(lhs, rhs),
								lineNumber);
					}

					break;
				default:
					return
						compileInfoFactory.CreateError(
							$"{lhs.ReturnType.Describe().Proper()} variables cannot be used for binary operators.",
							lineNumber);
			}
		}

		#endregion Binary Operators


		#region Dot References

		var dotReferences = ReverseBinarySplit(line, '.');

		// If we get an error, this function should return a compile error
		if (dotReferences.IsError)
		{
			return compileInfoFactory.CreateError("Unbalanced brackets or string literals", lineNumber);
		}

		// Dot References found
		if (dotReferences.IsFound && dotReferences.LHS.GetDouble() == null &&
		    dotReferences.RHS.GetDouble() == null)
		{
			// Compile LHS and check for errors
			var lhsCompileInfo = CompileFunction(dotReferences.LHS, variableSpace, lineNumber, gameworld);
			if (lhsCompileInfo.IsError)
			{
				return compileInfoFactory.CreateError($"Error with LHS of dot-reference function:\n{lhsCompileInfo.ErrorMessage}", lineNumber);
			}

			// Statements Compiled, Determine Functor Type and Return Type
			var lhs = (IFunction)lhsCompileInfo.CompiledStatement;

			// If the LHS is a collection, check for Collection Extensions first
			if (lhs.ReturnType.HasFlag(FutureProgVariableTypes.Collection))
			{
				var match = _collectionExtensionFunctionRegex.Match(dotReferences.RHS);
				if (!match.Success)
				{
					var drType = FutureProgVariable.DotReferenceReturnTypeFor(lhs.ReturnType,
						dotReferences.RHS);
					if (drType == FutureProgVariableTypes.Error)
					{
						return
							compileInfoFactory.CreateError(
								"Collection extension functions must be in the format func(varname, somefunction)",
								lineNumber);
					}

					return
						compileInfoFactory.CreateNew(
							new VariableDotReferenceFunction(lhs, dotReferences.RHS, drType), lineNumber);
				}

				var result =
					CollectionExtensionFunction.GetCollectionExtensionFunctionCompiler(match.Groups[1].Value,
						match.Groups[2].Value, match.Groups[3].Value, variableSpace, lhs, lineNumber, gameworld);
				return result.Success
					? compileInfoFactory.CreateNew(result.CompiledFunction, lineNumber)
					: compileInfoFactory.CreateError(result.ErrorMessage, lineNumber);
			}

			var returnType = FutureProgVariable.DotReferenceReturnTypeFor(lhs.ReturnType, dotReferences.RHS);

			switch (returnType)
			{
				case FutureProgVariableTypes.Error:
					return compileInfoFactory.CreateError($"Property was not valid: {dotReferences.RHS}", lineNumber);
				case FutureProgVariableTypes.Void:
					return compileInfoFactory.CreateError("Functions cannot return void.", lineNumber);
			}

			return compileInfoFactory.CreateNew(
				new VariableDotReferenceFunction(lhs, dotReferences.RHS, returnType), lineNumber);
		}

		#endregion Dot References

		#region Unary Operators

		// Check to see whether the line is a Unary Operator
		var unaryOperators = UnaryStringSplit(line);

		// If we get an error, this function should return a compile error
		if (unaryOperators.IsError)
		{
			return compileInfoFactory.CreateError("Malformed unary function.", lineNumber);
		}

		// Unary Operator Found
		if (unaryOperators.IsFound)
		{
			// Extract parameters
			var parameterSplit = ParameterStringSplit(unaryOperators.FunctionContents, ',');

			// No parameters supplied
			if (parameterSplit.IsError)
			{
				return
					compileInfoFactory.CreateError(
						$"Erroneous parameters were supplied to function {unaryOperators.FunctionName}", lineNumber);
			}

			var parameters = parameterSplit.ParameterStrings ?? Enumerable.Empty<string>();

			// Compile each of the function strings and check for errors
			var parameterCompileResults =
				parameters.Select(x => CompileFunction(x, variableSpace, lineNumber, gameworld));
			if (parameterCompileResults.Any(x => x.IsError))
			{
				return
					compileInfoFactory.CreateError(
						$"Parameter error: {parameterCompileResults.First(x => x.IsError).ErrorMessage}", lineNumber);
			}

			var parameterFunctions = parameterCompileResults.Select(x => (IFunction)x.CompiledStatement).ToList();

			// Handle built in vs user defined functions
			switch (unaryOperators.Type)
			{
				case UnaryFunctionType.BuiltInFunction:
					var result =
						FutureProg.GetBuiltInFunctionCompiler(unaryOperators.FunctionName.ToLowerInvariant(),
							parameterFunctions, gameworld);
					return result.Success
						? compileInfoFactory.CreateNew(result.CompiledFunction, lineNumber)
						: compileInfoFactory.CreateError(result.ErrorMessage, lineNumber);

				case UnaryFunctionType.UserDefinedFunction:
					var udfResult =
						gameworld.FutureProgs.FirstOrDefault(
							x =>
								string.Equals(x.FunctionName, unaryOperators.FunctionName,
									StringComparison.InvariantCultureIgnoreCase) &&
								x.MatchesParameters(parameterFunctions.Select(y => y.ReturnType)));
					if (udfResult == null)
					{
						return compileInfoFactory.CreateError(
							$"No such User Defined Function as {unaryOperators.FunctionName}.", lineNumber);
					}

					return compileInfoFactory.CreateNew(
						new FutureProgInvokerFunction(udfResult, parameterFunctions), lineNumber);

				default:
					throw new NotSupportedException();
			}
		}

		#endregion Unary Operators

		return compileInfoFactory.CreateError("Line does not match known syntax", lineNumber);
	}

	#region Splitter Functions

	public enum NonFunctionType
	{
		None,
		BooleanLiteral,
		NumberLiteral,
		TextLiteral,
		VariableReference,
		TimeSpanLiteral
	}

	public enum UnaryFunctionType
	{
		None,
		BuiltInFunction,
		UserDefinedFunction
	}

	public static ParameterSplit ParameterStringSplit(string input, char splitChar)
	{
		int nestedLevel = 0, position = 0;
		var inStringLiteral = false;

		input = input.Trim();
		if (!input.Any())
		{
			return ParameterSplit.CreateNotFound();
		}

		var parameters = new List<string>();
		var lastStartIndex = 0;

		for (position = 0; position < input.Length; position++)
		{
			if (input[position] == '(' && !inStringLiteral)
			{
				nestedLevel++;
			}

			if (input[position] == ')' && !inStringLiteral)
			{
				nestedLevel--;
			}

			if (input[position] == '"')
			{
				if (inStringLiteral)
				{
					if (position > 0 && input[position - 1] != '\\')
					{
						inStringLiteral = false;
					}
				}
				else
				{
					inStringLiteral = true;
				}
			}

			if (!inStringLiteral && nestedLevel == 0 && input[position] == splitChar)
			{
				parameters.Add(input.Substring(lastStartIndex, position - lastStartIndex));
				lastStartIndex = position + 1;
			}
		}

		if (lastStartIndex < position)
		{
			parameters.Add(input.Substring(lastStartIndex));
		}

		return parameters.Any() ? ParameterSplit.CreateNew(parameters) : ParameterSplit.CreateError();
	}

	public static BinarySplit BinaryStringSplit(string input, IEnumerable<string> splitStrings)
	{
		while (true)
		{
			int nestedLevel = 0, position = 0;
			var inStringLiteral = false;

			input = input.Trim();
			if (!input.Any())
			{
				return BinarySplit.CreateError();
			}

			for (position = 0; position < input.Length; position++)
			{
				if (input[position] == '(' && !inStringLiteral)
				{
					nestedLevel++;
				}

				if (input[position] == ')' && !inStringLiteral)
				{
					nestedLevel--;
				}

				if (input[position] == '"')
				{
					if (inStringLiteral)
					{
						if (position > 0 && input[position - 1] != '\\')
						{
							inStringLiteral = false;
						}
					}
					else
					{
						inStringLiteral = true;
					}
				}

				if (!inStringLiteral && nestedLevel == 0)
				{
					var lookahead = input.Substring(position);
					var splitterFound =
						splitStrings.FirstOrDefault(
							x => lookahead.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
					if (splitterFound != null)
					{
						if (position == 0 || position == input.Length - 1)
						{
							continue;
						}

						var lhsLeading = new string(input.TakeWhile(x => x == '(').ToArray());
						var rhsTrailing =
							new string(input.Reverse().TakeWhile(x => x == ')').Take(lhsLeading.Length).ToArray());
						return BinarySplit.CreateNew(lhsLeading, input.Substring(0, position).Trim(),
							lookahead.Substring(splitterFound.Length).Trim(), rhsTrailing, splitterFound.Trim());
					}
				}
			}

			if (!inStringLiteral && nestedLevel == 0)
			{
				if (input.StartsWith("(", StringComparison.Ordinal) && input.EndsWith(")", StringComparison.Ordinal))
				{
					input = input.Substring(1, input.Length - 2);
					continue;
				}

				return BinarySplit.CreateNotFound();
			}

			return BinarySplit.CreateError();
		}
	}

	public static NonFunctionSplit NonFunctionStringSplit(string input)
	{
		var inStringLiteral = false;
		input = input.Trim();
		string lhsLeading = "", rhsTrailing = "";

		while (input.Any() && input[0] == '(')
		{
			lhsLeading += '(';
			input = input.Substring(1);
		}

		if (lhsLeading.Any())
		{
			while (input.Any() && input.Last() == ')')
			{
				rhsTrailing += ')';
				input = input.Substring(0, input.Length - 1);
			}
		}

		if (lhsLeading.Length != rhsTrailing.Length)
		{
			return NonFunctionSplit.CreateError();
		}

		if (!input.Any())
		{
			return NonFunctionSplit.CreateError();
		}

		switch (input[0])
		{
			case '"':
				for (var position = 0; position < input.Length; position++)
				{
					if (input[position] == '"')
					{
						if (inStringLiteral && input[position - 1] != '\\')
						{
							inStringLiteral = false;
							continue;
						}

						if (position == 0)
						{
							inStringLiteral = true;
							continue;
						}

						if (!inStringLiteral)
						{
							inStringLiteral = true;
						}
					}

					if (!inStringLiteral)
					{
						return NonFunctionSplit.CreateError();
					}
				}

				return NonFunctionSplit.CreateNew(NonFunctionType.TextLiteral,
					input.Substring(1, input.Length - 2).ParseSpecialCharacters(), lhsLeading, rhsTrailing);
			case '@':
				input = input.Substring(1);
				if (!input.Any())
				{
					return NonFunctionSplit.CreateError();
				}

				for (var position = 0; position < input.Length; position++)
				{
					if (position == 0 && !char.IsLetter(input[position]))
					{
						return NonFunctionSplit.CreateError();
					}

					if (!char.IsLetterOrDigit(input[position]) && input[position] != '_')
					{
						return NonFunctionSplit.CreateError();
					}
				}

				return NonFunctionSplit.CreateNew(NonFunctionType.VariableReference, input, lhsLeading, rhsTrailing);
			default:
				var match = TimespanRegex.Match(input);
				if (match.Success)
				{
					return NonFunctionSplit.CreateNew(NonFunctionType.TimeSpanLiteral, input, lhsLeading,
						rhsTrailing);
				}

				bool outBool;
				if (bool.TryParse(input, out outBool))
				{
					return NonFunctionSplit.CreateNew(NonFunctionType.BooleanLiteral, input, lhsLeading, rhsTrailing);
				}

				decimal outDouble;
				return decimal.TryParse(input, out outDouble)
					? NonFunctionSplit.CreateNew(NonFunctionType.NumberLiteral, input, lhsLeading, rhsTrailing)
					: NonFunctionSplit.CreateError();
		}
	}

	/// <summary>
	///     Performs the same operation as BinaryStringSplit, but begins with the end of the string and works backwards. Finds
	///     the last split.
	/// </summary>
	/// <param name="input"></param>
	/// <param name="splitChar"></param>
	/// <returns></returns>
	public static BinarySplit ReverseBinarySplit(string input, char splitChar)
	{
		while (true)
		{
			int nestedLevel = 0, position = 0;
			var inStringLiteral = false;

			input = input.Trim();
			if (!input.Any())
			{
				return BinarySplit.CreateError();
			}

			for (position = input.Length - 1; position >= 0; position--)
			{
				if (input[position] == '(' && !inStringLiteral)
				{
					nestedLevel--;
				}

				if (input[position] == ')' && !inStringLiteral)
				{
					nestedLevel++;
				}

				if (input[position] == '"')
				{
					if (inStringLiteral)
					{
						if (position == 0 || input[position - 1] != '\\')
						{
							inStringLiteral = false;
						}
					}
					else
					{
						inStringLiteral = true;
					}
				}

				if (!inStringLiteral && nestedLevel == 0 && input[position] == splitChar && position > 0)
				{
					var lhsLeading = new string(input.TakeWhile(x => x == '(').ToArray());
					var rhsTrailing =
						new string(input.Reverse().TakeWhile(x => x == ')').Take(lhsLeading.Length).ToArray());
					return BinarySplit.CreateNew(lhsLeading, input.Substring(0, position).Trim(),
						input.Substring(position + 1), rhsTrailing, ".");
				}
			}

			if (!inStringLiteral && nestedLevel == 0)
			{
				if (input.StartsWith("(", StringComparison.Ordinal) && input.EndsWith(")", StringComparison.Ordinal))
				{
					input = input.Substring(1, input.Length - 2);
					continue;
				}

				return BinarySplit.CreateNotFound();
			}

			return BinarySplit.CreateError();
		}
	}

	private static UnarySplit UnaryStringSplit(string input)
	{
		int nestedLevel = 0, position = 0, firstBracketPosition = 0, lastBracketPosition = 0;
		bool inStringLiteral = false, function = false, udf = false, atEnd = false;

		input = input.Trim();
		if (!input.Any())
		{
			return UnarySplit.CreateError();
		}

		for (position = 0; position < input.Length; position++)
		{
			if (!char.IsWhiteSpace(input[position]) && atEnd)
			{
				return UnarySplit.CreateError();
			}

			if (input[position] != '@' && !char.IsWhiteSpace(input[position]) &&
			    !char.IsLetterOrDigit(input[position]) && input[position] != '(' && input[position] != '_' &&
			    nestedLevel == 0 && !function)
			{
				return UnarySplit.CreateNotFound();
			}

			if (input[position] == '@' && position == 0)
			{
				udf = true;
			}

			if (input[position] == '(' && !inStringLiteral)
			{
				if (position == 0)
				{
					return UnarySplit.CreateNotFound();
				}

				if (nestedLevel == 0)
				{
					firstBracketPosition = position;
					function = true;
				}

				nestedLevel++;
			}

			if (input[position] == ')' && !inStringLiteral)
			{
				nestedLevel--;
				if (nestedLevel == 0)
				{
					atEnd = true;
					lastBracketPosition = position;
				}
			}

			if (input[position] == '"')
			{
				if (inStringLiteral)
				{
					if (position > 0 && input[position - 1] != '\\')
					{
						inStringLiteral = false;
					}
				}
				else
				{
					inStringLiteral = true;
				}
			}
		}

		if (nestedLevel > 0 || inStringLiteral)
		{
			return UnarySplit.CreateError();
		}

		if (!function)
		{
			return UnarySplit.CreateNotFound();
		}

		var lhsLeading = new string(input.TakeWhile(x => x == '(').ToArray());
		var rhsTrailing = new string(input.Reverse().TakeWhile(x => x == ')').Take(lhsLeading.Length).ToArray());
		return
			UnarySplit.CreateNew(
				(udf ? input.Substring(1, firstBracketPosition - 1) : input.Substring(0, firstBracketPosition)).Trim
					(), udf ? UnaryFunctionType.UserDefinedFunction : UnaryFunctionType.BuiltInFunction,
				input.Substring(firstBracketPosition + 1, lastBracketPosition - firstBracketPosition - 1).Trim(),
				lhsLeading, rhsTrailing);
	}

	public class ParameterSplit : IDisposable
	{
		private ParameterSplit(bool isFound, bool isError, IEnumerable<string> parameterStrings)
		{
			IsFound = isFound;
			IsError = isError;
			ParameterStrings = parameterStrings;
		}

		public bool IsError { get; protected set; }

		public bool IsFound { get; protected set; }

		public IEnumerable<string> ParameterStrings { get; protected set; }

		#region IDisposable Members

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Members

		public static ParameterSplit CreateError()
		{
			return new ParameterSplit(false, true, null);
		}

		public static ParameterSplit CreateNew(IEnumerable<string> parameterStrings)
		{
			return new ParameterSplit(true, false, parameterStrings);
		}

		public static ParameterSplit CreateNotFound()
		{
			return new ParameterSplit(false, false, null);
		}
	}

	public class BinarySplit : IDisposable
	{
		private BinarySplit(string lhsLeading, string lhs, string rhs, string rhsTrailing, string matchedSplitString,
			bool isFound = false, bool isError = false)
		{
			LHSLeading = lhsLeading;
			LHS = lhs;
			RHS = rhs;
			RHSTrailing = rhsTrailing;
			MatchedSplitString = matchedSplitString;
			IsError = isError;
			IsFound = isFound;
		}

		public bool IsError { get; }

		public bool IsFound { get; }

		public string LHS { get; }

		private string LHSLeading { get; set; }

		public string MatchedSplitString { get; }

		public string RHS { get; }

		private string RHSTrailing { get; set; }

		#region IDisposable Members

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Members

		public static BinarySplit CreateError()
		{
			return new BinarySplit("", "", "", "", "", false, true);
		}

		public static BinarySplit CreateNew(string lhsLeading, string lhs, string rhs, string rhsTrailing,
			string matchedSplitString)
		{
			return new BinarySplit(lhsLeading, lhs, rhs, rhsTrailing, matchedSplitString, true);
		}

		public static BinarySplit CreateNotFound()
		{
			return new BinarySplit("", "", "", "", "");
		}
	}

	public class NonFunctionSplit : IDisposable
	{
		private NonFunctionSplit(bool isFound, bool isError, NonFunctionType type, string stringValue,
			string lhsLeading, string rhsLeading)
		{
			IsError = isError;
			IsFound = isFound;
			Type = type;
			StringValue = stringValue;
			LHSLeading = lhsLeading;
			RHSTrailing = rhsLeading;
		}

		public bool IsError { get; }

		public bool IsFound { get; }

		private string LHSLeading { get; set; }

		private string RHSTrailing { get; set; }

		public string StringValue { get; }

		public NonFunctionType Type { get; }

		#region IDisposable Members

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Members

		public static NonFunctionSplit CreateError()
		{
			return new NonFunctionSplit(false, true, NonFunctionType.None, "", "", "");
		}

		public static NonFunctionSplit CreateNew(NonFunctionType type, string stringValue, string lhsLeading,
			string rhsLeading)
		{
			return new NonFunctionSplit(true, false, type, stringValue, lhsLeading, rhsLeading);
		}

		public static NonFunctionSplit CreateNotFound()
		{
			return new NonFunctionSplit(false, false, NonFunctionType.None, "", "", "");
		}
	}

	private class UnarySplit : IDisposable
	{
		private UnarySplit(bool isFound, bool isError, string functionName, UnaryFunctionType type,
			string functionContents, string lhsLeading, string rhsLeading)
		{
			IsFound = isFound;
			IsError = isError;
			FunctionName = functionName;
			FunctionContents = functionContents;
			Type = type;
			LHSLeading = lhsLeading;
			RHSTrailing = rhsLeading;
		}

		public string FunctionContents { get; }

		public string FunctionName { get; }

		public bool IsError { get; }

		public bool IsFound { get; }

		private string LHSLeading { get; set; }

		private string RHSTrailing { get; set; }

		public UnaryFunctionType Type { get; }

		#region IDisposable Members

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Members

		public static UnarySplit CreateError()
		{
			return new UnarySplit(false, true, "", UnaryFunctionType.None, "", "", "");
		}

		public static UnarySplit CreateNew(string functionName, UnaryFunctionType type, string functionContents,
			string lhsLeading, string rhsLeading)
		{
			return new UnarySplit(true, false, functionName, type, functionContents, lhsLeading, rhsLeading);
		}

		public static UnarySplit CreateNotFound()
		{
			return new UnarySplit(false, false, "", UnaryFunctionType.None, "", "", "");
		}
	}

	#endregion Splitter Functions
}