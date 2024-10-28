using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Statements.Manipulation;

internal class CloseDoor : Statement
{
	private static readonly Regex CompileRegex = new(@"^\s*closedoor\s+(.+)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	private readonly IFunction CharacterFunction;
	private readonly IFunction EmoteFunction;
	private readonly IFunction ExitFunction;

	private CloseDoor(IFunction characterFunction, IFunction exitFunction, IFunction emoteFunction)
	{
		CharacterFunction = characterFunction;
		ExitFunction = exitFunction;
		EmoteFunction = emoteFunction;
	}

	private static ICompileInfo Compile(IEnumerable<string> lines,
		IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = CompileRegex.Match(lines.First());

		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[1].Value, ' ');
		if (splitArgs.IsError)
		{
			return CompileInfo.GetFactory().CreateError("Error with arguments of CloseDoor statement.", lineNumber);
		}

		if (splitArgs.ParameterStrings.Count() < 2 || splitArgs.ParameterStrings.Count() > 3)
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The CloseDoor statement requires exactly two or three parameters.",
					           lineNumber);
		}

		var compiledArgs =
			splitArgs.ParameterStrings.Select(
				x => FunctionHelper.CompileFunction(x, variableSpace, lineNumber, gameworld)).ToList();
		if (compiledArgs.Any(x => x.IsError))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"Compile error with CloseDoor statement arguments: {compiledArgs.First(x => x.IsError).ErrorMessage}", lineNumber);
		}

		if (
			!((IFunction)compiledArgs[0].CompiledStatement).ReturnType.CompatibleWith(
				ProgVariableTypes.Character))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The first argument of the CloseDoor statement must be a character.",
					           lineNumber);
		}

		if (!((IFunction)compiledArgs[1].CompiledStatement).ReturnType.CompatibleWith(ProgVariableTypes.Exit))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The second argument of the CloseDoor statement must be an exit.", lineNumber);
		}

		if (compiledArgs.Count == 3 &&
		    !((IFunction)compiledArgs[2].CompiledStatement).ReturnType.CompatibleWith(ProgVariableTypes.Text))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           "The third argument of the CloseDoor statement must be text, if specified at all.",
					           lineNumber);
		}

		return
			CompileInfo.GetFactory()
			           .CreateNew(
				           new CloseDoor((IFunction)compiledArgs[0].CompiledStatement,
					           (IFunction)compiledArgs[1].CompiledStatement,
					           compiledArgs.Count == 3 ? (IFunction)compiledArgs[2].CompiledStatement : null),
				           variableSpace, lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = CompileRegex.Match(line);
		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[1].Value, ' ');
		if (splitArgs.IsError || splitArgs.ParameterStrings.Count() < 2)
		{
			return line;
		}

		return
			$"{"closedoor".Colour(Telnet.Cyan, Telnet.Black)} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(0))} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(1))}{(splitArgs.ParameterStrings.Count() > 2 ? $" {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(2))}" : "")}";
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
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (CharacterFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = CharacterFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if (ExitFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = ExitFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if (EmoteFunction != null && EmoteFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = EmoteFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var exit = (ICellExit)ExitFunction.Result;
		if (exit == null)
		{
			ErrorMessage = "Exit was null in Opendoor statement.";
			return StatementResult.Error;
		}

		if (exit.Exit.Door?.IsOpen != true)
		{
			return StatementResult.Normal;
		}

		var character = (ICharacter)CharacterFunction.Result;
		if (character == null)
		{
			ErrorMessage = "Character was null in Opendoor statement.";
			return StatementResult.Error;
		}

		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(EmoteFunction?.Result.GetObject.ToString()))
		{
			emote = new PlayerEmote(EmoteFunction.Result.GetObject.ToString(), character);
			if (!emote.Valid)
			{
				emote = null;
			}
		}

		// TODO - locking as well?
		character.Body.Close(exit.Exit.Door, null, emote);
		return StatementResult.Normal;
	}
}