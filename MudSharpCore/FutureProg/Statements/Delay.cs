using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class Delay : Statement
{
	private static readonly Regex CompileRegex = new(@"^\s*delay\s+(.+)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected IFunction CharacterFunction;
	protected IFunction DelayFunction;
	protected IFunction TextFunction;

	public Delay(IFunction delayFunction, IFunction characterFunction, IFunction textFunction)
	{
		DelayFunction = delayFunction;
		CharacterFunction = characterFunction;
		TextFunction = textFunction;
	}

	private static ICompileInfo Compile(IEnumerable<string> lines,
		IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = CompileRegex.Match(lines.First());

		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[1].Value, ' ');
		if (splitArgs.IsError)
		{
			return CompileInfo.GetFactory().CreateError("Error with arguments of delay statement.", lineNumber);
		}

		if (splitArgs.ParameterStrings.Count() != 3)
		{
			return CompileInfo.GetFactory()
			                  .CreateError("The delay statement requires exactly three parameters.", lineNumber);
		}

		var compiledArgs =
			splitArgs.ParameterStrings.Select(
				x => FunctionHelper.CompileFunction(x, variableSpace, lineNumber, gameworld)).ToList();
		if (compiledArgs.Any(x => x.IsError))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"Compile error with delay statement arguments: {compiledArgs.First(x => x.IsError).ErrorMessage}", lineNumber);
		}

		if (
			!((IFunction)compiledArgs[0].CompiledStatement).ReturnType.CompatibleWith(
				ProgVariableTypes.Number))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The first argument of the delay statement must be a number.", lineNumber);
		}

		if (
			!((IFunction)compiledArgs[1].CompiledStatement).ReturnType.CompatibleWith(
				ProgVariableTypes.Character))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The second argument of the delay statement must be a character.", lineNumber);
		}

		if (!((IFunction)compiledArgs[2].CompiledStatement).ReturnType.CompatibleWith(ProgVariableTypes.Text))
		{
			return CompileInfo.GetFactory()
			                  .CreateError("The third argument of the delay statement must be text.", lineNumber);
		}

		return
			CompileInfo.GetFactory()
			           .CreateNew(
				           new Delay((IFunction)compiledArgs[0].CompiledStatement,
					           (IFunction)compiledArgs[1].CompiledStatement,
					           (IFunction)compiledArgs[2].CompiledStatement),
				           variableSpace, lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = CompileRegex.Match(line);
		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[1].Value, ' ');
		if (splitArgs.IsError || splitArgs.ParameterStrings.Count() != 3)
		{
			return line;
		}

		return
			$"{"delay".Colour(Telnet.Cyan, Telnet.Black)} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(0))} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(1))} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(2))}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = CompileRegex.Match(line);
		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[1].Value, ' ');
		if (splitArgs.IsError || splitArgs.ParameterStrings.Count() != 3)
		{
			return line;
		}

		return
			$"{"delay".Colour(Telnet.KeywordPink)} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(0), true)} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(1), true)} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(2), true)}";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				CompileRegex, Compile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(CompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(CompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("delay", @"The DELAY statement is used to force a character to execute a command as if they had typed it after a set delay. This would mostly be used for relatively low-criticality NPC AI routines where you can tolerate the possibility that the command fails (it might not be eligible by the time it's run, the NPC could be knocked out, etc).

The syntax is as follows:

	#Odelay#0 #2milliseconds#0 #Mcharacter#0 #Ntext#0

For example:

	#Odelay#0 #25000#0 #M@guard#0 #N""say Golly, the weather sure is nice today!""#0

If you specify a delay of 0 the command will be executed immediately - this is really an alternative to the FORCE statement in this usage.");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (DelayFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = DelayFunction.ErrorMessage;
			return StatementResult.Error;
		}

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

		var character = (ICharacter)CharacterFunction.Result;
		if (character == null)
		{
			ErrorMessage = "Character in Delay command resulted in null.";
			return StatementResult.Error;
		}

		var delay = Convert.ToDouble(DelayFunction.Result.GetObject);
		if (delay <= 0)
		{
			((IControllable)CharacterFunction.Result).ExecuteCommand(TextFunction.Result.GetObject.ToString());
			return StatementResult.Normal;
		}

		var text = TextFunction.Result.GetObject.ToString();
		character.AddEffect(new DelayedAction(character,
			perceivable => { character.ExecuteCommand(text); },
			"delayed command"), TimeSpan.FromMilliseconds(delay));
		return StatementResult.Normal;
	}
}