using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class AddRange : Statement
{
	private static readonly Regex AddRangeCompileRegex = new(@"^\s*addrange\s*(?<collection>@{0,1}[a-z][\w\d]*)\s*(?<item>.+)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected IFunction CollectionFunction;

	protected string CollectionVariable;

	public AddRange(string collectionVariable, IFunction itemFunction)
	{
		CollectionVariable = collectionVariable;
		CollectionFunction = itemFunction;
	}

	private static ICompileInfo AddRangeCompile(IEnumerable<string> lines,
		IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = AddRangeCompileRegex.Match(lines.First());
		ProgVariableTypes collectionType;
		if (match.Groups["collection"].Value[0] == '@')
		{
			var collectionFunctionInfo = FunctionHelper.CompileFunction(match.Groups["collection"].Value.Substring(1),
				variableSpace, lineNumber, gameworld);
			if (collectionFunctionInfo.IsError)
			{
				return CompileInfo.GetFactory()
				                  .CreateError(
					                  $"The collection function in addrange returned the following error: {collectionFunctionInfo.ErrorMessage}",
					                  lineNumber);
			}

			if (collectionFunctionInfo.CompiledStatement is not VariableReferenceFunction)
			{
				return CompileInfo.GetFactory()
				                  .CreateError(
					                  $"The collection function in addrange was not a variable reference. Only variable references can be used in addrange.",
					                  lineNumber);
			}

			collectionType = ((IFunction)collectionFunctionInfo.CompiledStatement).ReturnType;
		}
		else
		{
			if (!variableSpace.ContainsKey(match.Groups["collection"].Value.ToLowerInvariant()))
			{
				return
					CompileInfo.GetFactory()
					           .CreateError(
						           $"There is no collection called {match.Groups["collection"].Value} to add a range to.",
						           lineNumber);
			}

			collectionType = variableSpace[match.Groups["collection"].Value.ToLowerInvariant()];
		}

		if (!collectionType.HasFlag(ProgVariableTypes.Collection))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError($"The variable {match.Groups["collection"].Value.TrimStart('@')} is not a collection.", lineNumber);
		}

		var addRangeFunctionInfo = FunctionHelper.CompileFunction(match.Groups["item"].Value, variableSpace, lineNumber,
			gameworld);
		if (addRangeFunctionInfo.IsError)
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"The range function in addrange returned the following error: {addRangeFunctionInfo.ErrorMessage}", lineNumber);
		}

		var addRangeFunction = (IFunction)addRangeFunctionInfo.CompiledStatement;
		if (!collectionType.CompatibleWith(addRangeFunction.ReturnType ^ ProgVariableTypes.Collection))
		{
			return CompileInfo.GetFactory()
			                  .CreateError("The range is not of the same type as the collection.", lineNumber);
		}

		return
			CompileInfo.GetFactory()
			           .CreateNew(new AddRange(match.Groups["collection"].Value.TrimStart('@').ToLowerInvariant(), addRangeFunction),
				           variableSpace,
				           lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = AddRangeCompileRegex.Match(line);
		return
			$"{"addrange".Colour(Telnet.Blue, Telnet.Black)} {(match.Groups["collection"].Value[0] == '@' ? FunctionHelper.ColouriseFunction(match.Groups["collection"].Value) : match.Groups["collection"].Value)} {FunctionHelper.ColouriseFunction(match.Groups["item"].Value)}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = AddRangeCompileRegex.Match(line);
		return
			$"{"addrange".Colour(Telnet.KeywordPink)} {(match.Groups["collection"].Value[0] == '@' ? FunctionHelper.ColouriseFunction(match.Groups["collection"].Value, true) : match.Groups["collection"].Value.Colour(Telnet.VariableCyan))} {FunctionHelper.ColouriseFunction(match.Groups["item"].Value, true)}";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				AddRangeCompileRegex, AddRangeCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(AddRangeCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(AddRangeCompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("addrange", @"The ADDRANGE function is used to add one collection to another collection. Note that you can only add items to collections that are local variables. Collections that are properties of other objects are read-only and usually have other ways of interacting with them.

There are two possible versions of the syntax:

#Oaddrange#0 collectionname #Mrange#0 - adds the ramge to the collection variable so-named
#Oaddrange#0 #M@collection#0 #Mrange#0 - adds the range to the specific collection (a local variable)

For example, if you had a collection called #3numbers#0 and a collection called #3bonusnumbers#0, you could do either of the following:

#Oaddrange#0 numbers #M@bonusnumbers#0
#Oaddrange#0 #M@numbers#0 #M@bonusnumbers#0".SubstituteANSIColour());
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (CollectionFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = $"Add Range Function returned error: {CollectionFunction.ErrorMessage}";
			return StatementResult.Error;
		}

		var collection = variables.GetVariable(CollectionVariable);
		foreach (IProgVariable item in (IList)CollectionFunction.Result.GetObject)
		{
			((IList<IProgVariable>)collection.GetObject).Add(item);
		}

		return StatementResult.Normal;
	}
}