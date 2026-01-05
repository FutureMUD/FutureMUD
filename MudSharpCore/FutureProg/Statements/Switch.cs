using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class Switch : Statement
{
	private static readonly Regex SwitchCompileRegex = new(@"^\s*switch\s*\((.+)\)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	private static readonly Regex SwitchEndCompileRegex = new(@"^\s*end switch\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	private static readonly Regex SwitchCaseCompileRegex = new(@"^\s*case \((.+)\)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	private static readonly Regex SwitchDefaultCompileRegex = new(@"^\s*default\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected List<Tuple<IFunction, IEnumerable<IStatement>>> Cases;
	protected IEnumerable<IStatement> DefaultCase;
	protected IFunction SwitchFunction;

	public override bool IsReturnOrContainsReturnOnAllBranches() => Cases.All(x => x.Item2.LastOrDefault()?.IsReturnOrContainsReturnOnAllBranches() ?? false) &&
	                                                                (DefaultCase.LastOrDefault()?.IsReturnOrContainsReturnOnAllBranches() ?? false);

	public Switch(IFunction switchFunction, IEnumerable<IStatement> defaultCase,
		List<Tuple<IFunction, IEnumerable<IStatement>>> cases)
	{
		SwitchFunction = switchFunction;
		DefaultCase = defaultCase;
		Cases = cases;
	}

	private static ICompileInfo SwitchCompile(IEnumerable<string> lines,
		IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = SwitchCompileRegex.Match(lines.First());
		var switchFunctionTest = match.Groups[1].Value;

		var switchFunctionCompileInfo = FunctionHelper.CompileFunction(switchFunctionTest, variableSpace, lineNumber,
			gameworld);
		if (switchFunctionCompileInfo.IsError)
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"Switch Statement's switch function block returned an error: {switchFunctionCompileInfo.ErrorMessage}", lineNumber);
		}

		var switchFunctionCompiled = (IFunction)switchFunctionCompileInfo.CompiledStatement;

		IFunction currentCaseFunction = null;
		var cases = new List<Tuple<IFunction, IEnumerable<IStatement>>>();
		var defaultCase = new List<IStatement>();

		lines = lines.Skip(1);
		var containedStatements = new List<IStatement>();
		IDictionary<string, ProgVariableTypes> localVariables =
			new Dictionary<string, ProgVariableTypes>(variableSpace);
		var inDefaultMode = false;

		var currentLine = lineNumber;
		while (lines.Any())
		{
			var line = lines.First();
			if (SwitchEndCompileRegex.IsMatch(line))
			{
				if (inDefaultMode)
				{
					defaultCase = containedStatements;
				}
				else
				{
					if (currentCaseFunction == null)
					{
						return CompileInfo.GetFactory().CreateError("Switch statement was empty.", lineNumber);
					}

					cases.Add(Tuple.Create(currentCaseFunction, (IEnumerable<IStatement>)containedStatements));
				}

				return CompileInfo.GetFactory()
				                  .CreateNew(new Switch(switchFunctionCompiled, defaultCase, cases), variableSpace,
					                  lines.Skip(1),
					                  lineNumber, currentLine + 1);
			}

			if (SwitchCaseCompileRegex.IsMatch(line))
			{
				match = SwitchCaseCompileRegex.Match(line);
				var caseFunctionCompileInfo = FunctionHelper.CompileFunction(match.Groups[1].Value, variableSpace,
					currentLine + 1, gameworld);
				if (caseFunctionCompileInfo.IsError)
				{
					return
						CompileInfo.GetFactory()
						           .CreateError(
							           $"Case statement returned an error: {caseFunctionCompileInfo.ErrorMessage}",
							           currentLine + 1);
				}

				if (currentCaseFunction != null)
				{
					cases.Add(Tuple.Create(currentCaseFunction, (IEnumerable<IStatement>)containedStatements));
					containedStatements = new List<IStatement>();
					localVariables = new Dictionary<string, ProgVariableTypes>(variableSpace);
				}

				currentCaseFunction = (IFunction)caseFunctionCompileInfo.CompiledStatement;
				if (!switchFunctionCompiled.ReturnType.CompatibleWith(currentCaseFunction.ReturnType))
				{
					return
						CompileInfo.GetFactory()
						           .CreateError(
							           "Case statement return type does not match switch statement return type.",
							           currentLine + 1);
				}

				lines = lines.Skip(1);
				currentLine = caseFunctionCompileInfo.EndingLineNumber;
				continue;
			}

			if (SwitchDefaultCompileRegex.IsMatch(line))
			{
				if (inDefaultMode)
				{
					return CompileInfo.GetFactory()
					                  .CreateError("Multiple Default statements defined.", currentLine + 1);
				}

				if (currentCaseFunction != null)
				{
					cases.Add(Tuple.Create(currentCaseFunction, (IEnumerable<IStatement>)containedStatements));
					containedStatements = new List<IStatement>();
					localVariables = new Dictionary<string, ProgVariableTypes>(variableSpace);
				}

				inDefaultMode = true;
				lines = lines.Skip(1);
				continue;
			}

			if (!inDefaultMode && currentCaseFunction == null)
			{
				return
					CompileInfo.GetFactory()
					           .CreateError("Statements exist outside a case block inside the switch statement.",
						           currentLine + 1);
			}

			var statementInfo = FutureProg.CompileNextStatement(lines, localVariables, currentLine + 1, gameworld);
			if (statementInfo.IsError)
			{
				return
					CompileInfo.GetFactory()
					           .CreateError($"Error in Switch Statement: {statementInfo.ErrorMessage}",
						           statementInfo.ErrorLineNumber);
			}

			if (!statementInfo.IsComment)
			{
				containedStatements.Add(statementInfo.CompiledStatement);
			}

			lines = statementInfo.RemainingLines;
			currentLine = statementInfo.EndingLineNumber;
			localVariables = statementInfo.VariableSpace;
		}

		return CompileInfo.GetFactory()
		                  .CreateError("Switch statement did not have a matching switch end statement.", lineNumber);
	}

	private static string ColouriseSwitchStatement(string line)
	{
		var match = SwitchCompileRegex.Match(line);
		return
			$"{"switch".Colour(Telnet.Blue, Telnet.Black)} ({FunctionHelper.ColouriseFunction(match.Groups[1].Value)})";
	}

	private static string ColouriseSwitchStatementDarkMode(string line)
	{
		var match = SwitchCompileRegex.Match(line);
		return
			$"{"switch".Colour(Telnet.KeywordPink)} ({FunctionHelper.ColouriseFunction(match.Groups[1].Value, true)})";
	}

	private static string ColouriseCaseStatement(string line)
	{
		var match = SwitchCaseCompileRegex.Match(line);
		return
			$"{"case".Colour(Telnet.Blue, Telnet.Black)} ({FunctionHelper.ColouriseFunction(match.Groups[1].Value)})";
	}

	private static string ColouriseCaseStatementDarkMode(string line)
	{
		var match = SwitchCaseCompileRegex.Match(line);
		return
			$"{"case".Colour(Telnet.KeywordPink)} ({FunctionHelper.ColouriseFunction(match.Groups[1].Value, true)})";
	}

	private static string ColouriseDefaultStatement(string line)
	{
		return "default".Colour(Telnet.Blue, Telnet.Black);
	}

	private static string ColouriseDefaultStatementDarkMode(string line)
	{
		return "default".Colour(Telnet.KeywordPink);
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(new Tuple
		<Regex,
			Func<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
			SwitchCompileRegex, SwitchCompile
		));

		FutureProg.RegisterStatementColouriser(new Tuple<Regex, Func<string, string>>(
			SwitchCompileRegex, ColouriseSwitchStatement
		));

		FutureProg.RegisterStatementColouriser(new Tuple<Regex, Func<string, string>>(
			SwitchCompileRegex, ColouriseSwitchStatementDarkMode
		), true);

		FutureProg.RegisterStatementColouriser(new Tuple<Regex, Func<string, string>>(
			SwitchCaseCompileRegex, ColouriseCaseStatement
		));

		FutureProg.RegisterStatementColouriser(new Tuple<Regex, Func<string, string>>(
			SwitchCaseCompileRegex, ColouriseCaseStatementDarkMode
		), true);

		FutureProg.RegisterStatementColouriser(new Tuple<Regex, Func<string, string>>(
			SwitchDefaultCompileRegex, ColouriseDefaultStatement
		));

		FutureProg.RegisterStatementColouriser(new Tuple<Regex, Func<string, string>>(
			SwitchDefaultCompileRegex, ColouriseDefaultStatementDarkMode
		), true);

		FutureProg.RegisterStatementHelp("switch", @"The SWITCH statement is used to handle complex logical branches beyond just simple IF statements.

The variable supplied to the SWITCH is compared against all of the contained CASE values, and if one is found, it executes that block. If none is found, it will execute a DEFAULT block if one exists, or simply do nothing if there is none.

The syntax for this command is as follows:

	#Oswitch#0(#Mswitchvalue#0)
		#Ocase#0(#Mcasevalue#0)
			#2// Code if this case
		#Ocase#0(#Mothervalue#0)
			#2// Code if other case
		...as many other cases as you want...
		#Odefault#0
			#2// Code if default
	#Oend switch#0", "break, case, default, end");
		FutureProg.RegisterStatementHelp("case", @"The CASE statement is used to mark a case block within a SWITCH statement. 

Each case is associated with a value, which must be the same type as the switch value. If the case matches the switch variable, the code within the CASE statement block is executed.

A CASE statement block is ended by a #Obreak#0, #Odefault#0 or #Oend#0 statement.

You can have as many cases as you want for each switch statement. Cases are evaluated in the order that they appear in the prog, in case the logic is at all dependent on the order in which things are executed.", "break, default, end, switch");
		FutureProg.RegisterStatementHelp("default", @"The DEFAULT statement is used to mark a default case block within a SWITCH statement. The DEFAULT block must be the last block within the SWITCH.

The default block is executed only if none of the other cases match.

A DEFAULT statement block is ended by a #Obreak#0 or #Oend#0 statement.", "break, case, end, switch");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (SwitchFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = $"Switch statement returned an error: {SwitchFunction.ErrorMessage}";
			return StatementResult.Error;
		}

		IEnumerable<IStatement> statements = null;
		foreach (var item in Cases)
		{
			if (item.Item1.Execute(variables) == StatementResult.Error)
			{
				ErrorMessage = $"One of the cases returned an error: {item.Item1.ErrorMessage}";
				return StatementResult.Error;
			}

			if (item.Item1.Result?.GetObject == null ||
			    !item.Item1.Result.GetObject.Equals(SwitchFunction.Result.GetObject))
			{
				continue;
			}

			statements = item.Item2;
			break;
		}

		if (statements == null)
		{
			statements = DefaultCase;
		}

		var localVariables = new LocalVariableSpace(variables);
		foreach (var statement in statements)
		{
			var result = statement.Execute(localVariables);
			switch (result)
			{
				case StatementResult.Break:
					return StatementResult.Normal;
				case StatementResult.Continue:
					return StatementResult.Continue;
				case StatementResult.Return:
					return StatementResult.Return;
				case StatementResult.Error:
					ErrorMessage = statement.ErrorMessage;
					return StatementResult.Error;
			}
		}

		return StatementResult.Normal;
	}
}