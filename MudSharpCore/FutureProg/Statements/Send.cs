using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Statements;

internal class Send : Statement, IHaveFuturemud
{
	private static readonly Regex SendCompileRegex =
		new(
			@"^\s*send(?: (all|local|room|location|zone|shard|surrounds)){0,1}((?: (?:hidden|no1st|gods)){0,3})\s+(.+)\s*$",
			RegexOptions.IgnoreCase);

	protected OutputFlags Flags;
	protected IList<IFunction> ParameterFunctions;
	protected OutputRange Range;

	protected bool SendRaw;
	protected IFunction TargetFunction;
	protected IFunction TextFunction;

	public Send(string rangeType, string modifiers, IFunction textFunction, IFunction targetFunction,
		IList<IFunction> parameters, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		TextFunction = textFunction;
		TargetFunction = targetFunction;
		ParameterFunctions = parameters;

		switch (rangeType.ToLowerInvariant())
		{
			case "":
				Range = OutputRange.Personal;
				break;
			case "local":
				Range = OutputRange.Local;
				break;
			case "all":
				Range = OutputRange.All;
				break;
			case "location":
				Range = OutputRange.Local;
				break;
			case "surrounds":
				Range = OutputRange.Surrounds;
				break;
			case "zone":
				Range = OutputRange.Zone;
				break;
			case "shard":
				Range = OutputRange.Shard;
				break;
			default:
				throw new NotSupportedException();
		}

		Flags = OutputFlags.Normal;
		modifiers = modifiers.ToLowerInvariant();
		if (modifiers.Contains("hidden"))
		{
			Flags = Flags | OutputFlags.SuppressObscured;
		}

		if (modifiers.Contains("no1st"))
		{
			Flags = Flags | OutputFlags.SuppressSource;
		}

		if (modifiers.Contains("gods"))
		{
			Flags = Flags | OutputFlags.WizOnly;
		}

		if (!parameters.Any())
		{
			SendRaw = true;
		}
	}

	public IFuturemud Gameworld { get; protected set; }

	private static ICompileInfo SendCompile(IEnumerable<string> lines,
		IDictionary<string, FutureProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = SendCompileRegex.Match(lines.First());
		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[3].Value, ' ');
		if (splitArgs.IsError)
		{
			return CompileInfo.GetFactory().CreateError("Error with arguments of send statement.", lineNumber);
		}

		var compiledArgs =
			splitArgs.ParameterStrings.Select(
				x => FunctionHelper.CompileFunction(x, variableSpace, lineNumber, gameworld));
		if (compiledArgs.Any(x => x.IsError))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"Compile error with send statement arguments: {compiledArgs.First(x => x.IsError).ErrorMessage}", lineNumber);
		}

		var sendAll = match.Groups[1].Value.ToLowerInvariant() == "all";
		if (compiledArgs.Count() < 2 && !sendAll)
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("Send statement requires at least one perceivable for this mode.", lineNumber);
		}

		var textArg = (IFunction)compiledArgs.First().CompiledStatement;
		if (!textArg.ReturnType.CompatibleWith(FutureProgVariableTypes.Text))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The first argument to the send statement must be of type text.", lineNumber);
		}

		var targetArg = sendAll ? null : (IFunction)compiledArgs.Skip(1).FirstOrDefault().CompiledStatement;
		FutureProgVariableTypes targetArgType;
		switch (match.Groups[1].Value.ToLowerInvariant())
		{
			case "":
				targetArgType = FutureProgVariableTypes.Character | FutureProgVariableTypes.Item;
				break;
			case "local":
				targetArgType = FutureProgVariableTypes.Character | FutureProgVariableTypes.Item;
				break;
			case "room":
			case "location":
				targetArgType = FutureProgVariableTypes.Location;
				break;
			case "zone":
				targetArgType = FutureProgVariableTypes.Zone;
				break;
			case "shard":
				targetArgType = FutureProgVariableTypes.Shard;
				break;
			case "surrounds":
				targetArgType = FutureProgVariableTypes.Location;
				break;
			case "all":
				targetArgType = FutureProgVariableTypes.Anything;
				break;
			default:
				throw new NotSupportedException();
		}

		if (!targetArg?.ReturnType.CompatibleWith(targetArgType) == true)
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The type of the second argument does not match the expected type.",
					           lineNumber);
		}

		var perceivableArgs = compiledArgs.Skip(sendAll ? 1 : 2).Select(x => (IFunction)x.CompiledStatement);
		if (perceivableArgs.Any(x => !x.ReturnType.CompatibleWith(FutureProgVariableTypes.Perceiver)))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           "All arguments but the first and second passed to the send statement must resolve to perceivers.",
					           lineNumber);
		}

		return
			CompileInfo.GetFactory()
			           .CreateNew(
				           new Send(match.Groups[1].Value, match.Groups[2].Value, textArg, targetArg,
					           perceivableArgs.ToList(), gameworld), variableSpace, lines.Skip(1), lineNumber,
				           lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = SendCompileRegex.Match(line);
		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[3].Value, ' ');
		return
			$"{"send".Colour(Telnet.Cyan, Telnet.Black)}{match.Groups[1].Value.LeadingSpaceIfNotEmpty()}{match.Groups[2].Value.LeadingSpaceIfNotEmpty().Trim()} {(splitArgs.IsError ? match.Groups[3].Value.ColourBold(Telnet.Magenta, Telnet.Black) : splitArgs.ParameterStrings.Select(x => FunctionHelper.ColouriseFunction(x)).ListToString(separator: " ", conjunction: ""))}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = SendCompileRegex.Match(line);
		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[3].Value, ' ');
		return
			$"{"send".Colour(Telnet.KeywordPink)}{match.Groups[1].Value.LeadingSpaceIfNotEmpty()}{match.Groups[2].Value.LeadingSpaceIfNotEmpty().Trim()} {(splitArgs.IsError ? match.Groups[3].Value.ColourBold(Telnet.Magenta) : splitArgs.ParameterStrings.Select(x => FunctionHelper.ColouriseFunction(x, true)).ListToString(separator: " ", conjunction: ""))}";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, FutureProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				SendCompileRegex, SendCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(SendCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(SendCompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("send", @"The SEND statement is used to send echoes to players.

#1Note: This statement is mostly deprecated now. You should instead prefer to use the send built-in functions instead of the statement.#0

The syntax for using this statement is as follows:

	#Hsend#0 #6target#0 #1echo#0 - sends a message to a specific character
	#Hsend#0 range #6target#0 #1echo#0 - sends an echo to all players in the relevant 'range'
	#Hsend#0 range flags #6target#0 #1echo#0 - sends an echo to all players in the relevant 'range' with some output flags

The values for the range argument can be as follows:

	#3local#0 - sends to all in the room and layer
	#3room#0 - sends to all in the room (all layers)
	#3surrounds#0 - sends to adjacent rooms as well
	#3zone#0 - sends to all in the zone
	#3shard#0 - sends to all in the shard
	#3all#0 - sends to all characters in the game

The values for the flags argument can be as follows:

	#3no1st#0 - don't echo to the target themselves, just everyone else
	#3gods#0 - echo can only be seen by admins
	#3hidden#0 - if others can't see target, don't echo to them instead of echoing with 'someone'

The text that is sent will parse ANSI colour codes using ##s (e.g. ##5a tall man##0) and newline/tab characters (\n or \t). It does not do any in-game language parsing.");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (TextFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = TextFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if (TargetFunction != null && TargetFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = TargetFunction.ErrorMessage;
			return StatementResult.Error;
		}

		foreach (var func in ParameterFunctions)
		{
			if (func.Execute(variables) == StatementResult.Error)
			{
				ErrorMessage = func.ErrorMessage;
				return StatementResult.Error;
			}
		}

		Output output;
		if (SendRaw)
		{
			output = new RawOutput(((string)TextFunction.Result.GetObject).SubstituteANSIColour(), flags: Flags);
		}
		else
		{
			output =
				new EmoteOutput(
					new Emote(((string)TextFunction.Result.GetObject).SubstituteANSIColour(),
						(IPerceiver)ParameterFunctions.First().Result.GetObject,
						ParameterFunctions.Select(x => (IPerceiver)x.Result.GetObject).ToArray()), flags: Flags);
		}

		if (TargetFunction != null)
		{
			switch (TargetFunction.ReturnType)
			{
				case FutureProgVariableTypes.Character:
				case FutureProgVariableTypes.Item:
					switch (Range)
					{
						case OutputRange.Personal:
							((IPerceivable)TargetFunction.Result.GetObject).OutputHandler.Send(output);
							break;
						case OutputRange.Local:
							((IPerceivable)TargetFunction.Result.GetObject).OutputHandler.Handle(output);
							break;
					}

					break;
				case FutureProgVariableTypes.Location:
					if (Range == OutputRange.Local)
					{
						((ILocation)TargetFunction.Result.GetObject).Handle(output);
						break;
					}

					if (Range == OutputRange.Surrounds)
					{
						foreach (var location in ((ICell)TargetFunction.Result.GetObject).Surrounds)
						{
							location.Handle(output);
						}

						break;
					}

					throw new NotSupportedException();
				case FutureProgVariableTypes.Shard:
				case FutureProgVariableTypes.Zone:
					((ILocation)TargetFunction.Result.GetObject).Handle(output);
					break;
				default:
					throw new NotSupportedException();
			}
		}
		else
		{
			foreach (var ch in Gameworld.Characters)
			{
				ch.OutputHandler.Handle(output, OutputRange.Personal);
			}
		}

		return StatementResult.Normal;
	}
}