using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Statements;

internal class DeclareAndAssignVariable : Statement
{
	private static readonly Regex DeclareAndAssignVariableCompileRegex =
		new(@"^\s*var (?<varname>.+)\s*=\s*(?<function>.+)\s*$",
			RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected string NameToDeclare;
	protected FutureProgVariableTypes TypeToDeclare;
	protected IFunction ValueFunction;

	protected DeclareAndAssignVariable(string name, FutureProgVariableTypes type, IFunction valueFunction)
	{
		NameToDeclare = name;
		TypeToDeclare = type;
		ValueFunction = valueFunction;
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (variables.HasVariable(NameToDeclare))
		{
			ErrorMessage =
				$"Declare Variable with name {NameToDeclare} of type {TypeToDeclare} failed due to it being already defined.";
			return StatementResult.Error;
		}

		if (TypeToDeclare.HasFlag(FutureProgVariableTypes.Collection))
		{
			variables.SetVariable(NameToDeclare,
				new CollectionVariable(new List<IFutureProgVariable>(),
					TypeToDeclare ^ FutureProgVariableTypes.Collection));
		}
		else if (TypeToDeclare.HasFlag(FutureProgVariableTypes.Dictionary))
		{
			variables.SetVariable(NameToDeclare,
				new DictionaryVariable(new Dictionary<string, IFutureProgVariable>(),
					TypeToDeclare ^ FutureProgVariableTypes.Dictionary));
		}
		else if (TypeToDeclare.HasFlag(FutureProgVariableTypes.CollectionDictionary))
		{
			variables.SetVariable(NameToDeclare,
				new CollectionDictionaryVariable(new CollectionDictionary<string, IFutureProgVariable>(),
					TypeToDeclare ^ FutureProgVariableTypes.CollectionDictionary));
		}
		else
		{
			variables.SetVariable(NameToDeclare, new NullVariable(TypeToDeclare));
		}

		var result = ValueFunction.Execute(variables);
		if (result == StatementResult.Error)
		{
			ErrorMessage = ValueFunction.ErrorMessage;
			return StatementResult.Error;
		}

		variables.SetVariable(NameToDeclare, ValueFunction.Result);
		return StatementResult.Normal;
	}

	private static ICompileInfo DeclareAndAssignVariableCompile(IEnumerable<string> lines,
		IDictionary<string, FutureProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = DeclareAndAssignVariableCompileRegex.Match(lines.First());
		var variableName = match.Groups["varname"].Value.Trim().ToLowerInvariant();

		if (variableName.EqualTo("return"))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The variable name \"Return\" is a reserved keyword and cannot be used.",
					           lineNumber);
		}

		if (variableSpace.ContainsKey(variableName))
		{
			return CompileInfo.GetFactory().CreateError("Variable name is already declared.", lineNumber);
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

		var type = function.ReturnType ^ FutureProgVariableTypes.Literal;

		var newVar = new DeclareAndAssignVariable(variableName, type, function);
		variableSpace.Add(newVar.NameToDeclare, newVar.TypeToDeclare);
		return CompileInfo.GetFactory().CreateNew(newVar, variableSpace, lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = DeclareAndAssignVariableCompileRegex.Match(line);
		return
			$"{"var".Colour(Telnet.Blue, Telnet.Black)} {match.Groups["varname"].Value} = {FunctionHelper.ColouriseFunction(match.Groups[2].Value)}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = DeclareAndAssignVariableCompileRegex.Match(line);
		return
			$"{"var".Colour(Telnet.KeywordBlue)} {match.Groups["varname"].Value.Colour(Telnet.VariableCyan)} = {FunctionHelper.ColouriseFunction(match.Groups[2].Value, true)}";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, FutureProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				DeclareAndAssignVariableCompileRegex, DeclareAndAssignVariableCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(DeclareAndAssignVariableCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(DeclareAndAssignVariableCompileRegex, ColouriseStatementDarkMode), true
		);

		// Note - help for var is on DeclareVariable
	}
}