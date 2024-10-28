using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;

namespace MudSharp.FutureProg.Statements;

internal class Break : Statement
{
	private static readonly Regex BreakCompileRegex = new(@"^\s*break\s*$");

	public override StatementResult ExpectedResult => StatementResult.Break;

	private static ICompileInfo BreakCompile(IEnumerable<string> lines,
		IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		return CompileInfo.GetFactory().CreateNew(new Break(), variableSpace, lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		return line.Colour(Telnet.Blue, Telnet.Black);
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		return line.Colour(Telnet.KeywordPink);
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				BreakCompileRegex, BreakCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(BreakCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(BreakCompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("break", @"The BREAK statement is used to break out of a loop or control structure. If used in a loop, it will stop looping and continue on from the end of the loop. If used in a switch statement, it will exit the switch block and continue on from the end statement.", "switch, for, while, foreach");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		return StatementResult.Break;
	}
}