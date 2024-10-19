using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class ForEachLoop : Statement
{
	private static readonly Regex ForEachLoopCompileRegex = new(@"^\s*foreach \((.+) in (.+)\)\s*$");
	private static readonly Regex ForEachLoopEndCompileRegex = new(@"^\s*end for(?:each)?\s*$");

	protected IFunction CollectionFunction;
	protected IEnumerable<IStatement> ContainedBlock;
	protected string VarName;

	public override bool IsReturnOrContainsReturnOnAllBranches() => ContainedBlock.LastOrDefault()?.IsReturnOrContainsReturnOnAllBranches() ?? false;

	public ForEachLoop(string varName, IFunction collectionFunction, IEnumerable<IStatement> containedStatements)
	{
		VarName = varName;
		CollectionFunction = collectionFunction;
		ContainedBlock = containedStatements;
	}

	private static ICompileInfo ForEachLoopCompile(IEnumerable<string> lines,
		IDictionary<string, FutureProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = ForEachLoopCompileRegex.Match(lines.First());
		var varName = match.Groups[1].Value.ToLowerInvariant();
		var collectionExpression = match.Groups[2].Value;

		lines = lines.Skip(1);
		var containedStatements = new List<IStatement>();
		IDictionary<string, FutureProgVariableTypes> localVariables =
			new Dictionary<string, FutureProgVariableTypes>(variableSpace);

		var collectionFunctionInfo = FunctionHelper.CompileFunction(collectionExpression, variableSpace, lineNumber,
			gameworld);
		if (collectionFunctionInfo.IsError)
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"The expression for the collection in the ForEach statement did not compile: {collectionFunctionInfo.ErrorMessage}.", lineNumber);
		}

		var collectionFunction = (IFunction)collectionFunctionInfo.CompiledStatement;

		if (!collectionFunction.ReturnType.HasFlag(FutureProgVariableTypes.Collection))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           "The expression for the collection in the ForEach statement does not return a collection.",
					           lineNumber);
		}

		if (!localVariables.ContainsKey(varName))
		{
			localVariables[varName] = collectionFunction.ReturnType ^ FutureProgVariableTypes.Collection;
		}

		if (localVariables.ContainsKey(varName) &&
		    !localVariables[varName].CompatibleWith(collectionFunction.ReturnType ^
		                                            FutureProgVariableTypes.Collection))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("For Loop's variable was already declared and the type does not match.",
					           lineNumber);
		}

		var currentLine = lineNumber;
		while (lines.Any())
		{
			var line = lines.First();
			if (ForEachLoopEndCompileRegex.IsMatch(line))
			{
				return
					CompileInfo.GetFactory()
					           .CreateNew(new ForEachLoop(varName, collectionFunction, containedStatements),
						           variableSpace,
						           lines.Skip(1), lineNumber, currentLine + 1);
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
		                  .CreateError("ForEach Loop did not have a matching end for statement.", lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = ForEachLoopCompileRegex.Match(line);
		return
			$"{"foreach".Colour(Telnet.Blue, Telnet.Black)} ({match.Groups[1].Value} {"in".Colour(Telnet.Blue, Telnet.Black)} {FunctionHelper.ColouriseFunction(match.Groups[2].Value)})";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = ForEachLoopCompileRegex.Match(line);
		return
			$"{"foreach".Colour(Telnet.KeywordPink)} ({match.Groups[1].Value.Colour(Telnet.VariableCyan)} {"in".Colour(Telnet.KeywordBlue)} {FunctionHelper.ColouriseFunction(match.Groups[2].Value, true)})";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, FutureProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				ForEachLoopCompileRegex, ForEachLoopCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(ForEachLoopCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(ForEachLoopCompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("foreach",
			@"The FOREACH statement is used to create a loop that loops through all of the items of a collection once.

A FOREACH loop is ended by a corresponding #Hend foreach#0 statement. All the lines in between the FOREACH statement and the END statement are the code that is run on each item in the collection.

You can also exit the loop early with a #Hbreak#0 statement, or you can skip to the next iteration of the loop with a #Hcontinue#0 statement.

The syntax for this statement is:

	#Lforeach#0 (itemvariablename #Lin#0 #Mcollection#0)
		#2// Loop payload
	#Lend foreach#0

For example:

	#Lforeach#0 (victim #Lin#0 #M@potentialTargets#0)
		#Lif#0(#J@WillAttackVictim#0(#M@ch#0, #M@victim#0))
			#JAttackVictim#0(#M@ch#0, #M@victim#0)
			#Lbreak#0
		#Lend if#0
	#Lend foreach#0

	#Lforeach#0 (item #Lin#0 #JToLocation#0(#2123#0).#MItems#0)
		#M@totalweight#0 = #M@totalweight#0 + #M@item#0.#MWeight#0
	#Lend foreach#0

The scope of the variable declared as the item variable is limited to being used inside the code block of the loop.", 
			"break, continue, end");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var collectionResult = CollectionFunction.Execute(variables);
		if (collectionResult == StatementResult.Error)
		{
			ErrorMessage = $"Collection Function returned an error: {CollectionFunction.ErrorMessage}";
			return StatementResult.Error;
		}

		foreach (var item in (IList)CollectionFunction.Result.GetObject)
		{
			var localVariables = new LocalVariableSpace(variables);
			localVariables.SetVariable(VarName, (IFutureProgVariable)item);
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