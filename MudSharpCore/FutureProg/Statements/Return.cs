using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class Return : Statement
{
	private static readonly Regex ReturnCompileRegex = new(@"^\s*return(\s+.+)*\s*$");

	private readonly IFunction InnerFunction;

	private Return()
	{
	}

	private Return(IFunction innerFunction)
	{
		InnerFunction = innerFunction;
	}

	public override StatementResult ExpectedResult => StatementResult.Return;

	public override bool IsReturnOrContainsReturnOnAllBranches() => true;

	private static ICompileInfo ReturnCompile(IEnumerable<string> lines,
		IDictionary<string, FutureProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = ReturnCompileRegex.Match(lines.First());

		if (match.Groups[1].Success)
		{
			if (!variableSpace.ContainsKey("return"))
			{
				return
					CompileInfo.GetFactory()
					           .CreateError("Return statement with a function in a program without a return type.",
						           lineNumber);
			}

			var lhs = FunctionHelper.CompileFunction(match.Groups[1].Value, variableSpace, lineNumber, gameworld);
			if (lhs.IsError)
			{
				return CompileInfo.GetFactory()
				                  .CreateError($"Return Function error: {lhs.ErrorMessage}", lineNumber);
			}

			var lhsFunction = (IFunction)lhs.CompiledStatement;
			var returnVar = variableSpace["return"];

			return !lhsFunction.ReturnType.CompatibleWith(returnVar)
				? CompileInfo.GetFactory().CreateError(
					$"Return type of {returnVar.Describe()} ({returnVar}) does not match function of type {lhsFunction.ReturnType.Describe()} ({lhsFunction.ReturnType}).",
					lineNumber)
				: CompileInfo.GetFactory()
				             .CreateNew(new Return(lhsFunction), variableSpace, lines.Skip(1), lineNumber, lineNumber);
		}

		return CompileInfo.GetFactory()
		                  .CreateNew(new Return(), variableSpace, lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = ReturnCompileRegex.Match(line);
		if (match.Groups[1].Success)
		{
			return "return ".Colour(Telnet.Blue, Telnet.Black) +
			       FunctionHelper.ColouriseFunction(match.Groups[1].Value);
		}

		return "return".Colour(Telnet.Blue, Telnet.Black);
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = ReturnCompileRegex.Match(line);
		if (match.Groups[1].Success)
		{
			return "return ".Colour(Telnet.KeywordPink) +
			       FunctionHelper.ColouriseFunction(match.Groups[1].Value, true);
		}

		return "return".Colour(Telnet.KeywordPink);
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, FutureProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				ReturnCompileRegex, ReturnCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(ReturnCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(ReturnCompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("return", @"The RETURN statement is used to exit the prog, and ""return"" a value to the calling code, if that's what the prog does.

There are two versions of the command:

	#Oreturn#0 - exits the program and doesn't return a variable
	#Oreturn#0 #Msomevalue#0 - exits the program and returns the value

If a prog has been set to have a return type other than VOID, it must use the second form of the syntax, the first would be an error. Progs with VOID return types can only use the first syntax format.");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (InnerFunction == null)
		{
			return StatementResult.Return;
		}

		if (InnerFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = $"Return function had error: {InnerFunction.ErrorMessage}";
			return StatementResult.Error;
		}

		if (variables.HasVariable("return"))
		{
			variables.SetVariable("return", InnerFunction.Result);
		}

		return StatementResult.Return;
	}
}