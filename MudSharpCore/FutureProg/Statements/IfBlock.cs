using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class IfBlock : Statement
{
	private static readonly Regex IfBlockCompileRegex = new(@"^\s*if\s*\((.+)\)$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	private static readonly Regex IfBlockEndCompileRegex = new(@"^\s*end if\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	private static readonly Regex IfBlockElseCompileRegex = new(@"^\s*else\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	private static readonly Regex IfBlockElseIfCompileRegex = new(@"^\s*elseif\s*\((.+)\)$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected IEnumerable<IStatement> FalseBlock;
	protected IFunction LogicFunction;

	protected IEnumerable<IStatement> TrueBlock;
	protected IEnumerable<(IFunction ElseLogic, IEnumerable<IStatement> Statements)> ElseIfBlocks;

	public override bool IsReturnOrContainsReturnOnAllBranches() => (TrueBlock.LastOrDefault()?.IsReturnOrContainsReturnOnAllBranches() ?? false) &&
	                                                                (FalseBlock.LastOrDefault()?.IsReturnOrContainsReturnOnAllBranches() ?? false) &&
	                                                                (ElseIfBlocks.All(x => x.Statements.LastOrDefault()?.IsReturnOrContainsReturnOnAllBranches() ?? false));

	public IfBlock(IEnumerable<IStatement> trueBlock, IEnumerable<IStatement> falseBlock, IFunction logicFunction, IEnumerable<(IFunction ElseLogic, IEnumerable<IStatement> Statements)> elseIfBlocks)
	{
		TrueBlock = trueBlock;
		FalseBlock = falseBlock;
		LogicFunction = logicFunction;
		ElseIfBlocks = elseIfBlocks;
	}

	private static ICompileInfo IfBlockCompile(IEnumerable<string> lines,
		IDictionary<string, FutureProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = IfBlockCompileRegex.Match(lines.First());
		var funcInfo = FunctionHelper.CompileFunction(match.Groups[1].Value, variableSpace, lineNumber, gameworld);
		if (funcInfo.IsError)
		{
			return new CompileInfo(null, null, null, $"Error Message with If Function - {funcInfo.ErrorMessage}");
		}

		var function = (IFunction)funcInfo.CompiledStatement;
		if (!function.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			return CompileInfo.GetFactory()
			                  .CreateError("If Block's logic statement returned a non boolean value.", lineNumber);
		}

		lines = lines.Skip(1);
		var trueStatements = new List<IStatement>();
		var falseStatements = new List<IStatement>();
		var elseIfBlocks = new List<(IFunction ElseLogic, IEnumerable<IStatement> Statements)>();
		IDictionary<string, FutureProgVariableTypes> localVariablesTrue =
			new Dictionary<string, FutureProgVariableTypes>(variableSpace);
		IDictionary<string, FutureProgVariableTypes> localVariablesFalse =
			new Dictionary<string, FutureProgVariableTypes>(variableSpace);
		var inFalseBlock = false;

		var inElseIfBlock = false;
		IFunction elseIfLogic = null;
		List<IStatement> elseIfStatements = null;
		IDictionary<string, FutureProgVariableTypes> localVariablesElseIf = new Dictionary<string, FutureProgVariableTypes>(variableSpace);

		var currentLine = lineNumber;
		while (lines.Any())
		{
			var line = lines.First();
			if (IfBlockEndCompileRegex.IsMatch(line))
			{
				if (inElseIfBlock)
				{
					elseIfBlocks.Add((elseIfLogic, elseIfStatements));
				}
				return CompileInfo.GetFactory()
				                  .CreateNew(new IfBlock(trueStatements, falseStatements, function, elseIfBlocks), variableSpace,
					                  lines.Skip(1),
					                  lineNumber, currentLine + 1);
			}

			if (IfBlockElseCompileRegex.IsMatch(line))
			{
				if (inElseIfBlock)
				{
					elseIfBlocks.Add((elseIfLogic, elseIfStatements));
					inElseIfBlock = false;
				}
				
				inFalseBlock = true;
				lines = lines.Skip(1);
				currentLine++;
				continue;
			}

			if (IfBlockElseIfCompileRegex.IsMatch(line))
			{
				if (inFalseBlock)
				{
					return CompileInfo.GetFactory()
					                  .CreateError("Else If Block appears after an Else Block for the same If Statement.", lineNumber);
				}

				if (inElseIfBlock)
				{
					elseIfBlocks.Add((elseIfLogic, elseIfStatements));
				}

				inElseIfBlock = true;
				var elseifmatch = IfBlockElseIfCompileRegex.Match(lines.First());
				var elseiffuncInfo = FunctionHelper.CompileFunction(elseifmatch.Groups[1].Value, variableSpace, lineNumber, gameworld);
				if (elseiffuncInfo.IsError)
				{
					return new CompileInfo(null, null, null, $"Error Message with Else If Function - {elseiffuncInfo.ErrorMessage}");
				}

				var elseiffunction = (IFunction)elseiffuncInfo.CompiledStatement;
				if (!elseiffunction.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
				{
					return CompileInfo.GetFactory()
					                  .CreateError("Else If Block's logic statement returned a non boolean value.", lineNumber);
				}

				elseIfLogic = elseiffunction;
				localVariablesElseIf = new Dictionary<string, FutureProgVariableTypes>(variableSpace);
				lines = lines.Skip(1);
				elseIfStatements = new();
				continue;
			}

			var statementInfo = FutureProg.CompileNextStatement(lines,
				inFalseBlock ? localVariablesFalse : (inElseIfBlock ? localVariablesElseIf : localVariablesTrue), 
				currentLine + 1, 
				gameworld);
			if (statementInfo.IsError)
			{
				return CompileInfo.GetFactory()
				                  .CreateError(statementInfo.ErrorMessage, statementInfo.ErrorLineNumber);
			}

			if (!statementInfo.IsComment)
			{
				if (inFalseBlock)
				{
					falseStatements.Add(statementInfo.CompiledStatement);
				}
				else if (inElseIfBlock)
				{
					elseIfStatements.Add(statementInfo.CompiledStatement);
				}
				else
				{
					trueStatements.Add(statementInfo.CompiledStatement);
				}
			}

			lines = statementInfo.RemainingLines;
			if (inFalseBlock)
			{
				localVariablesFalse = statementInfo.VariableSpace;
			}
			else if (inElseIfBlock)
			{
				localVariablesElseIf = statementInfo.VariableSpace;
			}
			else
			{
				localVariablesTrue = statementInfo.VariableSpace;
			}
		}

		return CompileInfo.GetFactory()
		                  .CreateError("If block did not have a matching end if statement.", lineNumber);
	}

	private static string ColouriseIfStatement(string line)
	{
		var match = IfBlockCompileRegex.Match(line);
		return
			$"{"if".Colour(Telnet.Blue, Telnet.Black)} ({FunctionHelper.ColouriseFunction(match.Groups[1].Value)})";
	}

	private static string ColouriseIfStatementDarkMode(string line)
	{
		var match = IfBlockCompileRegex.Match(line);
		return
			$"{"if".Colour(Telnet.KeywordPink)} ({FunctionHelper.ColouriseFunction(match.Groups[1].Value, true)})";
	}

	private static string ColouriseElseStatement(string line)
	{
		return "else".Colour(Telnet.Blue, Telnet.Black);
	}

	private static string ColouriseElseStatementDarkMode(string line)
	{
		return "else".Colour(Telnet.KeywordPink);
	}

	private static string ColouriseElseIfStatement(string line)
	{
		var match = IfBlockElseIfCompileRegex.Match(line);
		return
			$"{"elseif".Colour(Telnet.Blue, Telnet.Black)} ({FunctionHelper.ColouriseFunction(match.Groups[1].Value)})";
	}

	private static string ColouriseElseIfStatementDarkMode(string line)
	{
		var match = IfBlockElseIfCompileRegex.Match(line);
		return
			$"{"elseif".Colour(Telnet.KeywordPink)} ({FunctionHelper.ColouriseFunction(match.Groups[1].Value, true)})";
	}

	private static string ColouriseEndStatement(string line)
	{
		return
			("end " + new Regex(@"^\s*end (.+)", RegexOptions.IgnoreCase).Match(line).Groups[1].Value).Colour(
				Telnet.Blue, Telnet.Black);
	}

	private static string ColouriseEndStatementDarkMode(string line)
	{
		return
			("end " + new Regex(@"^\s*end (.+)", RegexOptions.IgnoreCase).Match(line).Groups[1].Value).Colour(
				Telnet.KeywordPink);
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, FutureProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				IfBlockCompileRegex, IfBlockCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(IfBlockCompileRegex, ColouriseIfStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(IfBlockCompileRegex, ColouriseIfStatementDarkMode), true
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(IfBlockElseCompileRegex, ColouriseElseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(IfBlockElseCompileRegex, ColouriseElseStatementDarkMode), true
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(IfBlockElseIfCompileRegex, ColouriseElseIfStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(IfBlockElseIfCompileRegex, ColouriseElseIfStatementDarkMode), true
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(new Regex(@"^\s*end .+", RegexOptions.IgnoreCase),
				ColouriseEndStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(new Regex(@"^\s*end .+", RegexOptions.IgnoreCase),
				ColouriseEndStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("if", 
			@"The IF statement is used to do different code actions depending on some condition. It takes a boolean function that determines whether the main branch executes or not.

An IF statement block is ended by either an ELSEIF statement, an ELSE statement or an END IF statement. Everything between the IF and the first of these three is the main branch. The ELSEIF and ELSE blocks are optional. There can be more than one ELSEIF branch.

The syntax for this statement is:

	#Hif#0 (#4condition#0)
		#2// Do if true#0
	#Helseif#0 (#4condition#0)
		#2// Do if elseif is true
	#Helse#0
		#2// Do if false#0
	#Hend if#0

For example:

	#Hif#0 (#3IsAdmin#0(#6@ch#0))
		#5@GiveAdminItem#0(#6@ch#0)
	#Helse#0
		#3Echo#0(#6@ch#0, #1""Only admins can use this command.""#0)
	#Hend if#0", 
			"else, elseif end");

		FutureProg.RegisterStatementHelp("else",
			@"The ELSE statement is used to end the main branch of an IF statement and open the branch that executes if the main branch doesn't. The ELSE statement is optional.

See the IF statement for full syntax for the else command",
			"if, elseif, end");

		FutureProg.RegisterStatementHelp("elseif",
			@"The ELSE statement is used to open a branch that executes if the main branch doesn't and additional conditions are met. The ELSEIF statement is optional. An IF statement can have more than one ELSEIF branch.

See the IF statement for full syntax for the elseif command",
			"if, elseif, end");

		FutureProg.RegisterStatementHelp("end",
			@"The END statement is used to end several different ""block"" statements that have a block of code lines that belong to them. In some respects, it takes the place of closing curly braces in other languages.

The various forms this can take are as follows:

	#Hend if#0 - closes an #Hif#0 statement block or an #Helse#0 statement block
	#Hend switch#0 - closes a #Hswitch#0 statement block
	#Hend for#0 - closes a #Hfor#0 loop statement block
	#Hend foreach#0 - closes a #Hforeach#0 loop statement block
	#Hend while#0 - closes a #Hwhile#0 loop statement block

Execution of the prog resumes from the END statement after the conditional/loop block finishes.",
			"if, else, elseif, switch, for, foreach, while");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (LogicFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = LogicFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if ((bool)LogicFunction.Result.GetObject)
		{
			if (TrueBlock.Any())
			{
				var localVariables = new LocalVariableSpace(variables);
				foreach (var statement in TrueBlock)
				{
					var result = statement.Execute(localVariables);
					if (result == StatementResult.Error)
					{
						ErrorMessage = statement.ErrorMessage;
						return result;
					}

					if (result != StatementResult.Normal)
					{
						return result;
					}
				}
			}
		}
		else
		{
			foreach (var (elseIfLogic, elseIfBlock) in ElseIfBlocks)
			{
				if (elseIfLogic.Execute(variables) == StatementResult.Error)
				{
					ErrorMessage = elseIfLogic.ErrorMessage;
					return StatementResult.Error;
				}

				if (!((bool)elseIfLogic.Result.GetObject))
				{
					continue;
				}

				var localVariables = new LocalVariableSpace(variables);
				foreach (var statement in elseIfBlock)
				{
					var result = statement.Execute(localVariables);
					if (result == StatementResult.Error)
					{
						ErrorMessage = statement.ErrorMessage;
						return result;
					}

					if (result != StatementResult.Normal)
					{
						return result;
					}
				}

				return StatementResult.Normal;
			}

			if (FalseBlock.Any())
			{
				var localVariables = new LocalVariableSpace(variables);
				foreach (var statement in FalseBlock)
				{
					var result = statement.Execute(localVariables);
					if (result == StatementResult.Error)
					{
						ErrorMessage = statement.ErrorMessage;
						return result;
					}

					if (result != StatementResult.Normal)
					{
						return result;
					}
				}
			}
		}

		return StatementResult.Normal;
	}
}