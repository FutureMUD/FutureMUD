using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Statements;

internal class RemoveItemIndex : Statement
{
	private static readonly Regex RemoveItemCompileRegex =
		new(@"^\s*removeitemindex\s*(?<collection>@{0,1}[a-z][\w\d]*)\s*(?<index>.+)\s*$",
			RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected string CollectionVariable;
	protected IFunction ItemFunction;

	public RemoveItemIndex(string collectionVariable, IFunction itemFunction)
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
				                  .CreateError($"The collection function in additem returned the following error: {collectionFunctionInfo.ErrorMessage}", lineNumber);
			}

			if (collectionFunctionInfo.CompiledStatement is not VariableReferenceFunction)
			{
				return CompileInfo.GetFactory()
				                  .CreateError($"The collection function in additem was not a variable reference. Only variable references can be used in additem.", lineNumber);
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
						           $"There is no collection called {match.Groups["collection"].Value} to remove an item from.",
						           lineNumber);
			}

			collectionType = variableSpace[match.Groups["collection"].Value.ToLowerInvariant()];
		}
		

		if (!collectionType.HasFlag(FutureProgVariableTypes.Collection))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError($"The variable {match.Groups["collection"].Value.TrimStart('@').ToLowerInvariant()} is not a collection.", lineNumber);
		}

		var removeItemFunctionInfo = FunctionHelper.CompileFunction(match.Groups["index"].Value, variableSpace, lineNumber,
			gameworld);
		if (removeItemFunctionInfo.IsError)
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"The index function in removeitemindex returned the following error: {removeItemFunctionInfo.ErrorMessage}", lineNumber);
		}

		var removeItemFunction = (IFunction)removeItemFunctionInfo.CompiledStatement;
		if (!removeItemFunction.ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
		{
			return CompileInfo.GetFactory().CreateError("The index function may only be a number.", lineNumber);
		}

		return
			CompileInfo.GetFactory()
			           .CreateNew(new RemoveItemIndex(match.Groups["collection"].Value.TrimStart('@').ToLowerInvariant(), removeItemFunction),
				           variableSpace, lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = RemoveItemCompileRegex.Match(line);
		return
			$"{"removeitemindex".Colour(Telnet.Blue, Telnet.Black)} {(match.Groups["collection"].Value[0] == '@' ? FunctionHelper.ColouriseFunction(match.Groups["collection"].Value) : match.Groups["collection"].Value)} {FunctionHelper.ColouriseFunction(match.Groups["index"].Value)}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = RemoveItemCompileRegex.Match(line);
		return
			$"{"removeitemindex".Colour(Telnet.KeywordPink)} {(match.Groups["collection"].Value[0] == '@' ? FunctionHelper.ColouriseFunction(match.Groups["collection"].Value, true) : match.Groups["collection"].Value.Colour(Telnet.VariableCyan))} {FunctionHelper.ColouriseFunction(match.Groups["index"].Value, true)}";
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

		FutureProg.RegisterStatementHelp("removeitemindex", @"The REMOVEITEMINDEX function is used to remove an item from a collection at a specific 0-based index. Note that you can only remove items to collections that are local variables. Collections that are properties of other objects are read-only and usually have other ways of interacting with them.

There are two possible versions of the syntax:

#Hremoveitemindex#0 collectionname #2index#0 - removes the item at the specified index (zero based)
#Hremoveitemindex#0 @collection #2index#0 - removes the item at the specified index (zero based)

For example, if you had a collection called #3numbers#0, you could do either of the following:

#Hremoveitemindex#0 numbers #22#0
#Hremoveitemindex#0 #6@numbers#0 #22#0".SubstituteANSIColour());
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (ItemFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = $"Index Function returned error: {ItemFunction.ErrorMessage}";
			return StatementResult.Error;
		}

		var index = (int)(decimal)ItemFunction.Result.GetObject;
		var collection = variables.GetVariable(CollectionVariable);
		var newList = ((IList<IFutureProgVariable>)collection.GetObject).ToList();
		if (index >= 0 && index < newList.Count)
		{
			newList.RemoveAt(index);
		}

		variables.SetVariable(CollectionVariable,
			new CollectionVariable(newList, collection.Type ^ FutureProgVariableTypes.Collection));
		return StatementResult.Normal;
	}
}