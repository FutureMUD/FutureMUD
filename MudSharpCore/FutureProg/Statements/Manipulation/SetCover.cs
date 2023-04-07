using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Statements.Manipulation;

internal class SetCover : Statement
{
	private static readonly Regex CompileRegex = new(@"^\s*setcover\s+(.+)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	private readonly IFunction ItemFunction;
	private readonly IFunction ValueFunction;

	private SetCover(IFunction itemFunction, IFunction valueFunction)
	{
		ItemFunction = itemFunction;
		ValueFunction = valueFunction;
	}

	private static ICompileInfo Compile(IEnumerable<string> lines,
		IDictionary<string, FutureProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = CompileRegex.Match(lines.First());

		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[1].Value, ' ');
		if (splitArgs.IsError)
		{
			return CompileInfo.GetFactory().CreateError("Error with arguments of SetCover statement.", lineNumber);
		}

		if (splitArgs.ParameterStrings.Count() != 2)
		{
			return CompileInfo.GetFactory()
			                  .CreateError("The SetCover statement requires exactly two parameters.", lineNumber);
		}

		var compiledArgs =
			splitArgs.ParameterStrings.Select(
				x => FunctionHelper.CompileFunction(x, variableSpace, lineNumber, gameworld)).ToList();
		if (compiledArgs.Any(x => x.IsError))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"Compile error with SetCover statement arguments: {compiledArgs.First(x => x.IsError).ErrorMessage}", lineNumber);
		}

		if (!((IFunction)compiledArgs[0].CompiledStatement).ReturnType.CompatibleWith(FutureProgVariableTypes.Item))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The first argument of the SetCover statement must be an item.", lineNumber);
		}

		if (
			!((IFunction)compiledArgs[1].CompiledStatement).ReturnType.CompatibleWith(
				FutureProgVariableTypes.Boolean))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The second argument of the SetCover statement must be a boolean.", lineNumber);
		}

		return
			CompileInfo.GetFactory()
			           .CreateNew(
				           new SetCover((IFunction)compiledArgs[0].CompiledStatement,
					           (IFunction)compiledArgs[1].CompiledStatement), variableSpace, lines.Skip(1), lineNumber,
				           lineNumber);
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
			$"{"setcover".Colour(Telnet.Cyan, Telnet.Black)} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(0))} {FunctionHelper.ColouriseFunction(splitArgs.ParameterStrings.ElementAt(1))}";
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
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (ItemFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = ItemFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if (ValueFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = ValueFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if (ItemFunction.Result is not IGameItem item)
		{
			ErrorMessage = "SetCover recieved a null item.";
			return StatementResult.Error;
		}

		var coverItem = item.GetItemType<IProvideCover>();
		if (coverItem == null)
		{
			ErrorMessage = "SetCover received an item that is not a cover item.";
			return StatementResult.Error;
		}

		var truth = (bool)ValueFunction.Result.GetObject;
		coverItem.IsProvidingCover = truth;
		return StatementResult.Normal;
	}
}