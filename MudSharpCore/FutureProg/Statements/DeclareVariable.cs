using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Statements;

internal class DeclareVariable : Statement
{
	private static readonly Regex DeclareVariableCompileRegex =
		new(@"^\s*var (?<varname>.+) as (?<type>(?<specifictype>\w{1,}) *(?<typemodifier>\w{0,}))\s*$",
			RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected string NameToDeclare;
	protected ProgVariableTypes TypeToDeclare;

	public DeclareVariable(string name, ProgVariableTypes type)
	{
		NameToDeclare = name;
		TypeToDeclare = type;
	}

	private static ICompileInfo DeclareVariableCompile(IEnumerable<string> lines,
		IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = DeclareVariableCompileRegex.Match(lines.First());

		if (match.Groups["varname"].Value.Trim().EqualTo("return"))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The variable name \"Return\" is a reserved keyword and cannot be used.",
					           lineNumber);
		}

		if (variableSpace.ContainsKey(match.Groups["varname"].Value.ToLowerInvariant()))
		{
			return CompileInfo.GetFactory().CreateError("Variable name is already declared.", lineNumber);
		}

		var type = FutureProg.GetTypeByName(match.Groups["type"].Value);

		if (type == ProgVariableTypes.Error)
		{
			if (
				!match.Groups["typemodifier"].Value.EqualToAny("collection", "dictionary", "collectiondictionary"))
			{
				return
					CompileInfo.GetFactory()
					           .CreateError(
						           $"{match.Groups["typemodifier"].Value} is not a valid variable declaration modifier type.", lineNumber);
			}

			return
				CompileInfo.GetFactory()
				           .CreateError($"{match.Groups["specifictype"].Value} is not a valid variable type.",
					           lineNumber);
		}

		var newVar = new DeclareVariable(match.Groups["varname"].Value.ToLowerInvariant(), type);
		variableSpace.Add(newVar.NameToDeclare, newVar.TypeToDeclare);
		return CompileInfo.GetFactory().CreateNew(newVar, variableSpace, lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = DeclareVariableCompileRegex.Match(line);
		return
			$"{"var".Colour(Telnet.Blue, Telnet.Black)} {match.Groups["varname"].Value} {"as".Colour(Telnet.Blue, Telnet.Black)} {match.Groups["specifictype"].Value}{(string.IsNullOrEmpty(match.Groups["typemodifier"].Value) ? "" : $" {match.Groups["typemodifier"].Value}")}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = DeclareVariableCompileRegex.Match(line);
		return
			$"{"var".Colour(Telnet.KeywordBlue)} {match.Groups["varname"].Value.Colour(Telnet.VariableCyan)} {"as".Colour(Telnet.KeywordBlue)} {$"{match.Groups["specifictype"].Value}{(string.IsNullOrEmpty(match.Groups["typemodifier"].Value) ? "" : $" {match.Groups["typemodifier"].Value}")}".Colour(Telnet.VariableGreen)}";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				DeclareVariableCompileRegex, DeclareVariableCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(DeclareVariableCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(DeclareVariableCompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("var", @"The VAR statement is used to declare a variable and tell the compiler what type it is going to be. All variables must have a type and that type cannot be changed once declared. Variable names must also be unique.

The core syntax is as follows:

	#Lvar#0 name #Las#0 #Ktype#0
	#Lvar#0 name = #J<initial value>#0

For example:

	#Lvar#0 index #Las#0 number
	#Lvar#0 targets #Las#0 character collection
	#Lvar#0 ch = #M@room#0.#MCharacters#0.#MFirst#0

The modifier types that can be used are #3collection#0, #3dictionary#0, and #3collectiondictionary#0. See PROG HELP COLLECTIONS for more info on those.

Another important thing to note about variable declarations is that they are limited to the scope in which they are declared; this means that if you declare a variable inside a loop for example, it will not be available once you exit the loop. Variables declared inside loops, switch statements, if statements, and collection extensions cannot be used and are discarded once the structure is exited.");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (variables.HasVariable(NameToDeclare))
		{
			ErrorMessage =
				$"Declare Variable with name {NameToDeclare} of type {TypeToDeclare} failed due to it being already defined.";
			return StatementResult.Error;
		}

		if (TypeToDeclare.HasFlag(ProgVariableTypes.Collection))
		{
			variables.SetVariable(NameToDeclare,
				new CollectionVariable(new List<IProgVariable>(),
					TypeToDeclare ^ ProgVariableTypes.Collection));
		}
		else if (TypeToDeclare.HasFlag(ProgVariableTypes.Dictionary))
		{
			variables.SetVariable(NameToDeclare,
				new DictionaryVariable(new Dictionary<string, IProgVariable>(),
					TypeToDeclare ^ ProgVariableTypes.Dictionary));
		}
		else if (TypeToDeclare.HasFlag(ProgVariableTypes.CollectionDictionary))
		{
			variables.SetVariable(NameToDeclare,
				new CollectionDictionaryVariable(new CollectionDictionary<string, IProgVariable>(),
					TypeToDeclare ^ ProgVariableTypes.CollectionDictionary));
		}
		else
		{
			variables.SetVariable(NameToDeclare, new NullVariable(TypeToDeclare));
		}

		return StatementResult.Normal;
	}
}