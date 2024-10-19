using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class Force : Statement
{
	private static readonly Regex CompileRegex = new(@"^\s*force\s+(.+)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected IFunction CharacterFunction;
	protected IFunction TextFunction;

	public Force(IFunction characterFunction, IFunction textFunction)
	{
		CharacterFunction = characterFunction;
		TextFunction = textFunction;
	}

	private static ICompileInfo Compile(IEnumerable<string> lines,
		IDictionary<string, FutureProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = CompileRegex.Match(lines.First());

		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[1].Value, ' ');
		if (splitArgs.IsError)
		{
			return CompileInfo.GetFactory().CreateError("Error with arguments of force statement.", lineNumber);
		}

		if (splitArgs.ParameterStrings.Count() != 2)
		{
			return CompileInfo.GetFactory()
			                  .CreateError("The force statement requires exactly two parameters.", lineNumber);
		}

		var compiledArgs =
			splitArgs.ParameterStrings.Select(
				x => FunctionHelper.CompileFunction(x, variableSpace, lineNumber, gameworld)).ToList();
		if (compiledArgs.Any(x => x.IsError))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"Compile error with force statement arguments: {compiledArgs.First(x => x.IsError).ErrorMessage}", lineNumber);
		}

		if (
			!((IFunction)compiledArgs[0].CompiledStatement).ReturnType.CompatibleWith(
				FutureProgVariableTypes.Character))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The first argument of the force statement must be a character.", lineNumber);
		}

		if (!((IFunction)compiledArgs[1].CompiledStatement).ReturnType.CompatibleWith(FutureProgVariableTypes.Text))
		{
			return CompileInfo.GetFactory()
			                  .CreateError("The second argument of the force statement must be text.", lineNumber);
		}

		return
			CompileInfo.GetFactory()
			           .CreateNew(
				           new Force((IFunction)compiledArgs[0].CompiledStatement,
					           (IFunction)compiledArgs[1].CompiledStatement), variableSpace, lines.Skip(1), lineNumber,
				           lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = CompileRegex.Match(line);
		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[1].Value, ' ');
		if (splitArgs.IsError || splitArgs.ParameterStrings.Count() != 2)
		{
			return line;
		}

		return
			$"{"force".Colour(Telnet.Cyan, Telnet.Black)} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(0))} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(1))}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = CompileRegex.Match(line);
		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[1].Value, ' ');
		if (splitArgs.IsError || splitArgs.ParameterStrings.Count() != 2)
		{
			return line;
		}

		return
			$"{"force".Colour(Telnet.KeywordPink)} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(0), true)} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(1), true)}";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, FutureProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				CompileRegex, Compile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(CompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(CompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("force", @"The FORCE statement is used to force a character to execute a command as if they had typed it.

The syntax is as follows:

	#Lforce#0 #Mcharacter#0 #Ntext#0

For example:

	#Lforce#0 #M@guard#0 #N""say Golly, the weather sure is nice today!""#0");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (TextFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = TextFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if (CharacterFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = CharacterFunction.ErrorMessage;
			return StatementResult.Error;
		}

		((IControllable)CharacterFunction.Result).ExecuteCommand(TextFunction.Result.GetObject.ToString());
		return StatementResult.Normal;
	}
}