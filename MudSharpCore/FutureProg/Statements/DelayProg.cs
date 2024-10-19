using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class DelayProg : Statement
{
	private static readonly Regex CompileRegex = new(@"^\s*delayprog\s+(\S+)\s+(.+)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected IFunction DelayFunction;
	protected IFuturemud Gameworld;
	protected List<IFunction> ParameterFunctions;
	protected IFutureProg TargetProg;

	public DelayProg(IFunction delayFunction, IFutureProg targetProg, List<IFunction> parameterFunctions,
		IFuturemud gameworld)
	{
		DelayFunction = delayFunction;
		ParameterFunctions = parameterFunctions;
		TargetProg = targetProg;
		Gameworld = gameworld;
	}

	private static ICompileInfo Compile(IEnumerable<string> lines,
		IDictionary<string, FutureProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = CompileRegex.Match(lines.First());

		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[2].Value, ' ');
		if (splitArgs.IsError)
		{
			return CompileInfo.GetFactory().CreateError("Error with arguments of delayprog statement.", lineNumber);
		}

		if (!splitArgs.ParameterStrings.Any())
		{
			return CompileInfo.GetFactory()
			                  .CreateError("The delayprog statement requires at least one argument.", lineNumber);
		}

		var compiledArgs =
			splitArgs.ParameterStrings.Select(
				x => FunctionHelper.CompileFunction(x, variableSpace, lineNumber, gameworld)).ToList();
		if (compiledArgs.Any(x => x.IsError))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"Compile error with delayprog statement arguments: {compiledArgs.First(x => x.IsError).ErrorMessage}", lineNumber);
		}

		if (
			!((IFunction)compiledArgs[0].CompiledStatement).ReturnType.CompatibleWith(
				FutureProgVariableTypes.Number))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The first argument of the delayprog statement must be a number.", lineNumber);
		}

		var parameters = compiledArgs.Skip(1).Select(x => x.CompiledStatement).OfType<IFunction>().ToList();
		var otherTypes = parameters.Select(x => x.ReturnType).ToList();
		var progArg = match.Groups[1].Value;

		var targetProg = gameworld.FutureProgs.FirstOrDefault(
			x => x.FunctionName.Equals(progArg, StringComparison.InvariantCultureIgnoreCase) &&
			     x.MatchesParameters(otherTypes)
		);

		if (targetProg == null)
		{
			return CompileInfo.GetFactory()
			                  .CreateError("There is no matching FutureProg with the same parameters.", lineNumber);
		}

		return
			CompileInfo.GetFactory()
			           .CreateNew(
				           new DelayProg((IFunction)compiledArgs[0].CompiledStatement, targetProg, parameters,
					           gameworld),
				           variableSpace, lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = CompileRegex.Match(line);
		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[2].Value, ' ');
		if (splitArgs.IsError || !splitArgs.ParameterStrings.Any())
		{
			return line;
		}

		var progArg = match.Groups[1].Value;
		return
			$"{"delayprog".Colour(Telnet.Cyan, Telnet.Black)} {progArg} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(0))} {splitArgs.ParameterStrings.Skip(1).Select(x => FunctionHelper.ColouriseFunction(x)).ListToString(separator: " ", conjunction: "")}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = CompileRegex.Match(line);
		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[2].Value, ' ');
		if (splitArgs.IsError || !splitArgs.ParameterStrings.Any())
		{
			return line;
		}

		var progArg = match.Groups[1].Value;
		return
			$"{"delayprog".Colour(Telnet.KeywordPink)} {progArg} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(0), true)} {splitArgs.ParameterStrings.Skip(1).Select(x => FunctionHelper.ColouriseFunction(x, true)).ListToString(separator: " ", conjunction: "")}";
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

		FutureProg.RegisterStatementHelp("delayprog", @"The DELAYPROG statement is used to execute another prog (or this one recursively) after a delay. It will put an action on the MUD's scheduler to execute the prog after the specified delay, but it does not stop or halt execution of this prog (this prog can continue or even launch multiple progs).

The syntax is as follows:

	#Odelayprog#0 #Jprogname#0 #2milliseconds#0 (parameters of the function, separated by spaces)

For example:

	#Odelayprog#0 IntroVideo3 #215000#0 (#M@step#0 + #21#0)

The above example is a real example that recursively calls itself after a 15 second delay with an incremented ""step"" variable. 

Notice also that the parameter was wrapped in parentheses because it contained spaces.");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (DelayFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = DelayFunction.ErrorMessage;
			return StatementResult.Error;
		}

		foreach (var function in ParameterFunctions)
		{
			if (function.Execute(variables) == StatementResult.Error)
			{
				ErrorMessage = function.ErrorMessage;
				return StatementResult.Error;
			}
		}

		var delay = (double)(decimal)DelayFunction.Result.GetObject;
		if (delay <= 0)
		{
			TargetProg.Execute(ParameterFunctions.Select(x => x.Result.GetObject).ToArray());
			return StatementResult.Normal;
		}

		var parameters = ParameterFunctions.Select(x => x.Result.GetObject).ToArray();
		Gameworld.Scheduler.AddSchedule(new Schedule(() => { TargetProg.Execute(parameters); },
			ScheduleType.FutureProg, TimeSpan.FromMilliseconds(delay), $"Delayed Prog {TargetProg.FunctionName}"));
		return StatementResult.Normal;
	}
}