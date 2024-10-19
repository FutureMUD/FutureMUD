using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class RemoveItem : Statement
{
	private static readonly Regex RemoveItemCompileRegex = new(@"^\s*removeitem\s*(?<collection>@{0,1}[a-z][\w\d]*)\s*(?<item>.+)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected string CollectionVariable;
	protected IFunction ItemFunction;

	public RemoveItem(string collectionVariable, IFunction itemFunction)
	{
		CollectionVariable = collectionVariable;
		ItemFunction = itemFunction;
	}

	private static ICompileInfo RemoveItemCompile(IEnumerable<string> lines,
		IDictionary<string, FutureProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = RemoveItemCompileRegex.Match(lines.First());
		FutureProgVariableTypes collectionType;
		if (match.Groups["collection"].Value[0] == '@')
		{
			var collectionFunctionInfo = FunctionHelper.CompileFunction(match.Groups["collection"].Value.Substring(1),
				variableSpace, lineNumber, gameworld);
			if (collectionFunctionInfo.IsError)
			{
				return CompileInfo.GetFactory()
				                  .CreateError($"The collection function in removeitem returned the following error: {collectionFunctionInfo.ErrorMessage}", lineNumber);
			}

			if (collectionFunctionInfo.CompiledStatement is not VariableReferenceFunction)
			{
				return CompileInfo.GetFactory()
				                  .CreateError($"The collection function in removeitem was not a variable reference. Only variable references can be used in removeitem.", lineNumber);
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
						           $"There is no collection called {match.Groups[1].Value} to remove an item from.",
						           lineNumber);
			}

			collectionType = variableSpace[match.Groups["collection"].Value.ToLowerInvariant()];
		}
		
		if (!collectionType.HasFlag(FutureProgVariableTypes.Collection))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError($"The variable {match.Groups["collection"].Value.TrimStart('@')} is not a collection.", lineNumber);
		}
		
		var removeItemFunctionInfo = FunctionHelper.CompileFunction(match.Groups["item"].Value, variableSpace, lineNumber,
			gameworld);
		if (removeItemFunctionInfo.IsError)
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"The item function in remitem returned the following error: {removeItemFunctionInfo.ErrorMessage}", lineNumber);
		}

		var removeItemFunction = (IFunction)removeItemFunctionInfo.CompiledStatement;
		if (
			(collectionType ^ FutureProgVariableTypes.Collection).CompatibleWith(removeItemFunction.ReturnType))
		{
			return CompileInfo.GetFactory()
			                  .CreateError("The item is not of the same type as the collection.", lineNumber);
		}

		return
			CompileInfo.GetFactory()
			           .CreateNew(new RemoveItem(match.Groups["collection"].Value.TrimStart('@').ToLowerInvariant(), removeItemFunction),
				           variableSpace, lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = RemoveItemCompileRegex.Match(line);
		return
			$"{"removeitem".Colour(Telnet.Blue, Telnet.Black)} {(match.Groups["collection"].Value[0] == '@' ? FunctionHelper.ColouriseFunction(match.Groups["collection"].Value) : match.Groups["collection"].Value)} {FunctionHelper.ColouriseFunction(match.Groups["item"].Value)}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = RemoveItemCompileRegex.Match(line);
		return
			$"{"removeitem".Colour(Telnet.KeywordPink)} {(match.Groups["collection"].Value[0] == '@' ? FunctionHelper.ColouriseFunction(match.Groups["collection"].Value, true) : match.Groups["collection"].Value.Colour(Telnet.VariableCyan))} {FunctionHelper.ColouriseFunction(match.Groups["item"].Value, true)}";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, FutureProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				RemoveItemCompileRegex, RemoveItemCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(RemoveItemCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(RemoveItemCompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("removeitem", @"The REMOVEITEM function is used to remove an item from a collection. Note that you can only remove items from collections that are local variables. Collections that are properties of other objects are read-only and usually have other ways of interacting with them.

There are two possible versions of the syntax:

#Oremoveitem#0 collectionname #Mitem#0 - removes the item from the collection variable so-named
#Oremoveitem#0 #M@collection#0 #Mitem#0 - removes the item from the specific collection (a local variable)

For example, if you had a collection called #3numbers#0, you could do either of the following:

#Oremoveitem#0 numbers #22#0
#Oremoveitem#0 #M@numbers#0 #22#0".SubstituteANSIColour());
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (ItemFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = $"Item Function returned error: {ItemFunction.ErrorMessage}";
			return StatementResult.Error;
		}

		var collection = variables.GetVariable(CollectionVariable);
		((IList<IFutureProgVariable>)collection.GetObject).Remove(ItemFunction.Result);
		return StatementResult.Normal;
	}
}