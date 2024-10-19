using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class ConsoleEcho : Statement
{
	private static readonly Regex ConsoleCompileRegex = new(@"^console ?(.*)",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected IFunction MessageFunction;

	public ConsoleEcho(IFunction messageFunction)
	{
		MessageFunction = messageFunction;
	}

	private static ICompileInfo ConsoleCompile(IEnumerable<string> lines,
		IDictionary<string, FutureProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = ConsoleCompileRegex.Match(lines.First());
		var messageInfo = FunctionHelper.CompileFunction(match.Groups[1].Value, variableSpace, lineNumber, gameworld);
		if (messageInfo.IsError)
		{
			return CompileInfo.GetFactory().CreateError(messageInfo.ErrorMessage, lineNumber);
		}

		var function = (IFunction)messageInfo.CompiledStatement;
		if (!function.ReturnType.CompatibleWith(FutureProgVariableTypes.Text))
		{
			return CompileInfo.GetFactory().CreateError("Message Function is not a Text Type", lineNumber);
		}

		return CompileInfo.GetFactory()
		                  .CreateNew(new ConsoleEcho((IFunction)messageInfo.CompiledStatement), variableSpace,
			                  lines.Skip(1),
			                  lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		return
			$"{"console".Colour(Telnet.Blue, Telnet.Black)} {FunctionHelper.ColouriseFunction(ConsoleCompileRegex.Match(line).Groups[1].Value)}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		return
			$"{"console".Colour(Telnet.KeywordPink)} {FunctionHelper.ColouriseFunction(ConsoleCompileRegex.Match(line).Groups[1].Value, true)}";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, FutureProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				ConsoleCompileRegex, ConsoleCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(ConsoleCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(ConsoleCompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("console", @"The CONSOLE statement is used to echo a message to the MUD's console window on the server. This also means that it will be written into the console log. This might be used to notify about a critical error or provide otherwise useful debugging information.

The syntax for this statement is:

	#Oconsole <text>#0

For example:

	#Oconsole#0 #M@errortext#0
	#Oconsole#0 #J@GetSomeErrorTextProg#0(#M@ch#0)
	#Oconsole#0 #N""Critical Error - the guy didn't have the thing!""#0");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var result = MessageFunction.Execute(variables);
		if (result == StatementResult.Error)
		{
			ErrorMessage = MessageFunction.ErrorMessage;
			return StatementResult.Error;
		}

		Console.WriteLine((string)MessageFunction.Result.GetObject);
		return StatementResult.Normal;
	}
}