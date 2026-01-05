using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class SetVariable : Statement
{
	private static readonly Regex SetVariableCompileRegex = new(@"^\s*([a-z0-9_]+?)\s*=\s*(.+)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected string NameToSet;
	protected ProgVariableTypes TypeToSet;
	protected IFunction ValueFunction;

	public SetVariable(string nameToSet, ProgVariableTypes typeToSet, IFunction valueFunction)
	{
		NameToSet = nameToSet;
		TypeToSet = typeToSet;
		ValueFunction = valueFunction;
	}

	private static ICompileInfo SetVariableCompile(IEnumerable<string> lines,
		IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = SetVariableCompileRegex.Match(lines.First());

		if (!variableSpace.ContainsKey(match.Groups[1].Value.ToLowerInvariant()))
		{
			return
				CompileInfo.GetFactory()
						   .CreateError($"Variable {match.Groups[1].Value} has not been declared.", lineNumber);
		}

		var rhsInfo = FunctionHelper.CompileFunction(match.Groups[2].Value, variableSpace, lineNumber, gameworld);
		if (rhsInfo.IsError)
		{
			return
				CompileInfo.GetFactory()
						   .CreateError($"Error with RHS of set variable statement: {rhsInfo.ErrorMessage}",
							   lineNumber);
		}

		var function = (IFunction)rhsInfo.CompiledStatement;
		if (!function.ReturnType.CompatibleWith(variableSpace[match.Groups[1].Value.ToLowerInvariant()]))
		{
			return CompileInfo.GetFactory().CreateError(
				$"Tried to set a variable to an incorrect type. Expected {variableSpace[match.Groups[1].Value.ToLowerInvariant()].Describe()} and got {function.ReturnType.Describe()}.",
				lineNumber);
		}

		return
			CompileInfo.GetFactory()
					   .CreateNew(
						   new SetVariable(match.Groups[1].Value.ToLowerInvariant(), function.ReturnType, function),
						   variableSpace, lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = SetVariableCompileRegex.Match(line);
		return $"{match.Groups[1].Value} = {FunctionHelper.ColouriseFunction(match.Groups[2].Value)}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = SetVariableCompileRegex.Match(line);
		return $"{match.Groups[1].Value.Colour(Telnet.VariableCyan)} = {FunctionHelper.ColouriseFunction(match.Groups[2].Value, true)}";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				SetVariableCompileRegex, SetVariableCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(SetVariableCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(SetVariableCompileRegex, ColouriseStatementDarkMode), true
		);

        FutureProg.RegisterStatementHelp("=", @"The = statement is used to assign a value to a variable that has already been declared.

The core syntax is as follows:

	name = #J<new value>#0

For example:

	number = #242#0
	ch = #M@room#0.#MCharacters#0.#MFirst#0

See also #3+=#0, #3-=#0, #3*=#0, #3/=#0, #3%=#0 and #3^=#0 for other ways of assigning values combined with an operation.");
    }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (variables.HasVariable(NameToSet) && !TypeToSet.CompatibleWith(variables.GetVariable(NameToSet).Type))
		{
			ErrorMessage =
				$"Set Variable Statement tried to set variable {NameToSet} to type {TypeToSet}, but is defined as {variables.GetVariable(NameToSet).Type}";
			return StatementResult.Error;
		}

		var result = ValueFunction.Execute(variables);
		if (result == StatementResult.Error)
		{
			ErrorMessage = ValueFunction.ErrorMessage;
			return StatementResult.Error;
		}

		variables.SetVariable(NameToSet, ValueFunction.Result);
		return StatementResult.Normal;
	}
}