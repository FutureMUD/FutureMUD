using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;

namespace MudSharp.FutureProg.Statements;

internal class Continue : Statement
{
	private static readonly Regex ContinueCompileRegex = new(@"^\s*continue\s*$");

	public override StatementResult ExpectedResult => StatementResult.Continue;

	private static ICompileInfo ContinueCompile(IEnumerable<string> lines,
		IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		return CompileInfo.GetFactory()
		                  .CreateNew(new Continue(), variableSpace, lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		return "continue".Colour(Telnet.Blue, Telnet.Black);
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		return "continue".Colour(Telnet.KeywordPink);
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				ContinueCompileRegex, ContinueCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(ContinueCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(ContinueCompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("continue", @"The CONTINUE statement is used to skip the rest of the block inside a loop and proceed to the next iteration of the loop.", "for, while, foreach");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		return StatementResult.Continue;
	}
}