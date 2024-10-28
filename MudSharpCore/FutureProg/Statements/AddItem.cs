using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class AddItem : Statement
{
	private static readonly Regex AddItemCompileRegex = new(@"^\s*additem\s*(?<collection>@{0,1}[a-z][\w\d]*)\s*(?<item>.+)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected string CollectionVariable;
	protected IFunction ItemFunction;

	public AddItem(string collectionVariable, IFunction itemFunction)
	{
		CollectionVariable = collectionVariable;
		ItemFunction = itemFunction;
	}

	private static ICompileInfo AddItemCompile(IEnumerable<string> lines,
		IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = AddItemCompileRegex.Match(lines.First());
		ProgVariableTypes collectionType;
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
						           $"There is no collection called {match.Groups["collection"].Value} to add an item to.",
						           lineNumber);
			}

			collectionType = variableSpace[match.Groups["collection"].Value.ToLowerInvariant()];
		}

		if (!collectionType.HasFlag(ProgVariableTypes.Collection))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError($"The variable {match.Groups["collection"].Value.TrimStart('@')} is not a collection.",
					           lineNumber);
		}

		var addItemFunctionInfo = FunctionHelper.CompileFunction(match.Groups[2].Value, variableSpace, lineNumber,
			gameworld);
		if (addItemFunctionInfo.IsError)
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"The item function in additem returned the following error: {addItemFunctionInfo.ErrorMessage}", lineNumber);
		}

		var addItemFunction = (IFunction)addItemFunctionInfo.CompiledStatement;
		if (
			!(collectionType ^ ProgVariableTypes.Collection).CompatibleWith(addItemFunction.ReturnType))
		{
			return CompileInfo.GetFactory()
			                  .CreateError("The item is not of the same type as the collection.", lineNumber);
		}

		return
			CompileInfo.GetFactory()
			           .CreateNew(new AddItem(match.Groups["collection"].Value.TrimStart('@').ToLowerInvariant(), addItemFunction), variableSpace,
				           lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = AddItemCompileRegex.Match(line);
		return
			$"{"additem".Colour(Telnet.Blue, Telnet.Black)} {(match.Groups["collection"].Value[0] == '@' ? FunctionHelper.ColouriseFunction(match.Groups["collection"].Value) : match.Groups["collection"].Value.Colour(Telnet.VariableCyan))} {FunctionHelper.ColouriseFunction(match.Groups["item"].Value)}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = AddItemCompileRegex.Match(line);
		return
			$"{"additem".Colour(Telnet.KeywordPink)} {(match.Groups["collection"].Value[0] == '@' ? FunctionHelper.ColouriseFunction(match.Groups["collection"].Value, true) : match.Groups["collection"].Value.Colour(Telnet.VariableCyan))} {FunctionHelper.ColouriseFunction(match.Groups["item"].Value, true)}";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				AddItemCompileRegex, AddItemCompile)
		);

		FutureProg.RegisterStatementHelp("additem", @"The AddItem function is used to add an item to a collection. Note that you can only add items to collections that are local variables. Collections that are properties of other objects are read-only and usually have other ways of interacting with them.

There are two possible versions of the syntax:

#Oadditem#0 collectionname #Mitem#0 - adds the item collection variable so-named
#Oadditem#0 #M@collection#0 #Mitem#0 - adds the item to the specific collection (a local variable)

For example, if you had a collection called #3numbers#0, you could do either of the following:

#Oadditem#0 numbers #22#0
#Oadditem#0 #M@numbers#0 #22#0".SubstituteANSIColour());

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(AddItemCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(AddItemCompileRegex, ColouriseStatementDarkMode), true
		);
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (ItemFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = $"Item Function returned error: {ItemFunction.ErrorMessage}";
			return StatementResult.Error;
		}

		var collection = variables.GetVariable(CollectionVariable);
		((IList<IProgVariable>)collection.GetObject).Add(ItemFunction.Result);
		return StatementResult.Normal;
	}
}