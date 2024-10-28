using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Statements;

internal class ForLoop : Statement
{
	private static readonly Regex ForLoopCompileRegex = new(@"^\s*for \((.+) : (\d+)\)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	private static readonly Regex ForLoopEndCompileRegex = new(@"^\s*end for\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected IEnumerable<IStatement> ContainedBlock;
	protected string LoopVarName;
	
	protected IFunction RepetitionsFunction;

	public override bool IsReturnOrContainsReturnOnAllBranches() => ContainedBlock.LastOrDefault()?.IsReturnOrContainsReturnOnAllBranches() ?? false;

	public ForLoop(IFunction repetitions, string loopvarname, IEnumerable<IStatement> containedBlock)
	{
		RepetitionsFunction = repetitions;
		LoopVarName = loopvarname;
		ContainedBlock = containedBlock;
	}

	private static ICompileInfo ForLoopCompile(IEnumerable<string> lines,
		IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = ForLoopCompileRegex.Match(lines.First());
		var varName = match.Groups[1].Value.ToLowerInvariant();
		var functionInfo = FunctionHelper.CompileFunction(match.Groups[2].Value, variableSpace, lineNumber, gameworld);
		if (functionInfo.IsError)
		{
			return CompileInfo.GetFactory()
			                  .CreateError($"For Loop's repetitions argument had an error:\n\n{functionInfo.ErrorMessage}", lineNumber);
		}

		var function = (IFunction)functionInfo.CompiledStatement;
		if (!function.ReturnType.CompatibleWith(ProgVariableTypes.Number))
		{
			return CompileInfo.GetFactory()
			                  .CreateError($"For Loop's repetitions argument did not return a number.", lineNumber);
		}
		
		lines = lines.Skip(1);
		var containedStatements = new List<IStatement>();
		IDictionary<string, ProgVariableTypes> localVariables =
			new Dictionary<string, ProgVariableTypes>(variableSpace);

		if (!localVariables.ContainsKey(varName))
		{
			localVariables[varName] = ProgVariableTypes.Number;
		}

		if (localVariables.ContainsKey(varName) &&
		    !localVariables[varName].CompatibleWith(ProgVariableTypes.Number))
		{
			return CompileInfo.GetFactory()
			                  .CreateError("For Loop's variable was already declared as a non-number.", lineNumber);
		}

		var currentLine = lineNumber;
		while (lines.Any())
		{
			var line = lines.First();
			if (ForLoopEndCompileRegex.IsMatch(line))
			{
				return CompileInfo.GetFactory()
				                  .CreateNew(new ForLoop(function, varName, containedStatements), variableSpace,
					                  lines.Skip(1),
					                  lineNumber, currentLine + 1);
			}

			var statementInfo = FutureProg.CompileNextStatement(lines, localVariables, currentLine + 1, gameworld);
			if (statementInfo.IsError)
			{
				return CompileInfo.GetFactory()
				                  .CreateError(statementInfo.ErrorMessage, statementInfo.ErrorLineNumber);
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
		                  .CreateError("For loop did not have a matching end for statement.", lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = ForLoopCompileRegex.Match(line);
		return
			$"{"for".Colour(Telnet.Blue, Telnet.Black)} ({match.Groups[1].Value} : {FunctionHelper.ColouriseFunction(match.Groups[2].Value)})";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = ForLoopCompileRegex.Match(line);
		return
			$"{"for".Colour(Telnet.KeywordPink)} ({match.Groups[1].Value.Colour(Telnet.VariableCyan)} : {FunctionHelper.ColouriseFunction(match.Groups[2].Value, true)})";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				ForLoopCompileRegex, ForLoopCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(ForLoopCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(ForLoopCompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("for",
			@"The FOR statement is used to create a loop that loops a certain number of times, and stores a variable that records the number of the iteration.

A FOR loop is ended by a corresponding #Hend for#0 statement. All the lines in between the FOR statement and the END statement are the code that is run each iteration.

You can also exit the loop early with a #Hbreak#0 statement, or you can skip to the next iteration of the loop with a #Hcontinue#0 statement.

Within each iteration, a variable with the specified name holds the current iteration number. The iteration starts at 1, and goes through to the target number (inclusive).

The syntax for this statement is:

	#Ofor#0 (iterationvariablename : #2numberfunction#0)
		#2// Loop payload
	#Oend for#0

For example:

	#Ovar#0 sum #Las#0 number
	#Msum#0 = #20#0
	#Ofor#0 (i : #210#0)
		#Msum#0 = #M@sum#0 + #M@i#0
	#Oend for#0
	#Oconsole#0 (#N""The sum of all the numbers from 1 to 10 is ""#0 + #M@sum#0)

The scope of the variable declared as the iteration variable is limited to being used inside the code block of the loop.",
			"break, continue, end");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var repetitionsResult = RepetitionsFunction.Execute(variables);
		if (repetitionsResult != StatementResult.Normal)
		{
			return repetitionsResult;
		}

		var repetitions = Convert.ToInt32((decimal?)RepetitionsFunction.Result?.GetObject ?? 0.0M);
		if (repetitions <= 0)
		{
			return StatementResult.Normal;
		}

		for (var i = 1; i <= repetitions; i++)
		{
			var localVariables = new LocalVariableSpace(variables);
			localVariables.SetVariable(LoopVarName, new NumberVariable(i));

			foreach (var statement in ContainedBlock)
			{
				var result = statement.Execute(localVariables);
				if (result == StatementResult.Break)
				{
					return StatementResult.Normal;
				}

				if (result == StatementResult.Continue)
				{
					break;
				}

				switch (result)
				{
					case StatementResult.Return:
						return StatementResult.Return;
					case StatementResult.Error:
						ErrorMessage = statement.ErrorMessage;
						return StatementResult.Error;
				}
			}
		}

		return StatementResult.Normal;
	}
}