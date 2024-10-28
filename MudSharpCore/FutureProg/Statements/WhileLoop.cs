using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class WhileLoop : Statement
{
	private static readonly Regex WhileLoopCompileRegex = new(@"^\s*while \((.+)\)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	private static readonly Regex WhileLoopEndCompileRegex = new(@"^\s*end while\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected IEnumerable<IStatement> ContainedBlock;
	protected IFunction WhileFunction;

	public override bool IsReturnOrContainsReturnOnAllBranches() => ContainedBlock.LastOrDefault()?.IsReturnOrContainsReturnOnAllBranches() ?? false;

	public WhileLoop(IFunction whileFunction, IEnumerable<IStatement> containedBlock)
	{
		WhileFunction = whileFunction;
		ContainedBlock = containedBlock;
	}

	private static ICompileInfo WhileLoopCompile(IEnumerable<string> lines,
		IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = WhileLoopCompileRegex.Match(lines.First());
		var functionText = match.Groups[1].Value;

		var functionCompileInfo = FunctionHelper.CompileFunction(functionText, variableSpace, lineNumber, gameworld);
		if (functionCompileInfo.IsError)
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"While Loop's logic block contained an error: {functionCompileInfo.ErrorMessage}",
					           lineNumber);
		}

		var compiledFunction = (IFunction)functionCompileInfo.CompiledStatement;
		if (!compiledFunction.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			return CompileInfo.GetFactory()
			                  .CreateError("While Loop's logic block must return a boolean.", lineNumber);
		}

		lines = lines.Skip(1);
		var containedStatements = new List<IStatement>();
		IDictionary<string, ProgVariableTypes> localVariables =
			new Dictionary<string, ProgVariableTypes>(variableSpace);
		var currentLine = lineNumber;
		while (lines.Any())
		{
			var line = lines.First();
			if (WhileLoopEndCompileRegex.IsMatch(line))
			{
				return CompileInfo.GetFactory()
				                  .CreateNew(new WhileLoop(compiledFunction, containedStatements), variableSpace,
					                  lines.Skip(1),
					                  lineNumber, currentLine + 1);
			}

			var statementInfo = FutureProg.CompileNextStatement(lines, localVariables, currentLine + 1, gameworld);
			if (statementInfo.IsError)
			{
				return CompileInfo.GetFactory()
				                  .CreateError($"Error in While Block: {statementInfo.ErrorMessage}",
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
		                  .CreateError("While loop did not have a matching end while statement.", lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = WhileLoopCompileRegex.Match(line);
		return
			$"{"while".Colour(Telnet.Blue, Telnet.Black)} ({FunctionHelper.ColouriseFunction(match.Groups[1].Value)})";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = WhileLoopCompileRegex.Match(line);
		return
			$"{"while".Colour(Telnet.KeywordPink)} ({FunctionHelper.ColouriseFunction(match.Groups[1].Value, true)})";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				WhileLoopCompileRegex, WhileLoopCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(WhileLoopCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(WhileLoopCompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("while",
			@"The WHILE statement is used to create a loop that loops as long as the condition it monitors remains TRUE.

A WHILE loop is ended by a corresponding #Hend while#0 statement. All the lines in between the WHILE statement and the END statement are the code that is run on each iteration.

You can also exit the loop early with a #Hbreak#0 statement, or you can skip to the next iteration of the loop with a #Hcontinue#0 statement.

The syntax for this statement is:

	#Owhile#0 (#Lcondition#0)
		#2// Loop payload
	#Oend while#0

For example:

	#Owhile#0 (#Ltrue#0)
		#Oif#0(#Jnot#0(#M@room#0.#MExits#0.#MAny#0))
			#Obreak#0
		#Oend if#0
		#Cvar#0 north #Cas#0 location
		north = #Mroom#0.#MExits#0.#JFirst#0(x, #M@x#0.#MDirection#0 == #1""north""#0)
		#Cif#0(#JIsNull#0(#M@north#0)
			#Obreak#0
		#Cend if#0
		room = #M@north#0
		#Ocontinue#0
	#Oend while#0",
			"break, continue, end");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		while (WhileFunction.Execute(variables) != StatementResult.Error &&
		       ((bool?)WhileFunction.Result.GetObject ?? false))
		{
			var localVariables = new LocalVariableSpace(variables);
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