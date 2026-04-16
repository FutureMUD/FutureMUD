#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class SleepStatement : Statement
{
	private static readonly Regex SleepCompileRegex = new(@"^\s*sleep(?:\s*\((.+)\)|\s+(.+))\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	public SleepStatement(IFunction durationFunction)
	{
		DurationFunction = durationFunction;
	}

	internal IFunction DurationFunction { get; }

	private static ICompileInfo SleepCompile(IEnumerable<string> lines,
		IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = SleepCompileRegex.Match(lines.First());
		var durationText = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
		var functionInfo = FunctionHelper.CompileFunction(durationText, variableSpace, lineNumber, gameworld);
		if (functionInfo.IsError)
		{
			return CompileInfo.GetFactory()
				.CreateError($"Sleep statement duration had an error: {functionInfo.ErrorMessage}", lineNumber);
		}

		var function = (IFunction)functionInfo.CompiledStatement;
		if (!function.ReturnType.CompatibleWith(ProgVariableTypes.TimeSpan))
		{
			return CompileInfo.GetFactory()
				.CreateError("Sleep statements must use an expression that returns a timespan.", lineNumber);
		}

		return CompileInfo.GetFactory()
			.CreateNew(new SleepStatement(function), variableSpace, lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = SleepCompileRegex.Match(line);
		var durationText = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
		return $"{"sleep".Colour(Telnet.Blue, Telnet.Black)} {FunctionHelper.ColouriseFunction(durationText)}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = SleepCompileRegex.Match(line);
		var durationText = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
		return $"{"sleep".Colour(Telnet.KeywordPink)} {FunctionHelper.ColouriseFunction(durationText, true)}";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple<Regex,
				Func<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				SleepCompileRegex, SleepCompile),
			FutureProgCompilationContext.ComputerProgram);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(SleepCompileRegex, ColouriseStatement));

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(SleepCompileRegex, ColouriseStatementDarkMode), true);

		FutureProg.RegisterStatementHelp("sleep", @"The SLEEP statement is used by computer programs to suspend their execution for a period of real time and then resume from the next statement.

The syntax is:

	#Osleep#0 #M<timespan expression>#0

For example:

	#Osleep#0 #260s#0
	#Osleep#0 #M@retrydelay#0",
			"",
			FutureProgCompilationContext.ComputerProgram);
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		ErrorMessage = "Sleep statements can only execute inside the computer program runtime.";
		return StatementResult.Error;
	}
}
