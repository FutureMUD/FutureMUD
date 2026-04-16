#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;

namespace MudSharp.Computers;

public class ComputerHelpService : IComputerHelpService
{
	private static readonly ProgVariableTypes[] ComputerBaseTypes =
	[
		ProgVariableTypes.Boolean,
		ProgVariableTypes.MudDateTime,
		ProgVariableTypes.Number,
		ProgVariableTypes.Text,
		ProgVariableTypes.TimeSpan
	];

	public IEnumerable<ProgVariableTypes> GetAvailableTypes(FutureProgCompilationContext context)
	{
		if (!context.IsComputerContext())
		{
			return ProgVariableTypes.Anything
				.GetAllFlags()
				.Where(x => x != ProgVariableTypes.Collection)
				.Where(x => x != ProgVariableTypes.Dictionary)
				.Where(x => x != ProgVariableTypes.CollectionDictionary)
				.OrderBy(x => x.DescribeEnum());
		}

		return ComputerBaseTypes
			.Concat([
				ProgVariableTypes.Collection,
				ProgVariableTypes.Dictionary,
				ProgVariableTypes.CollectionDictionary
			]);
	}

	public IEnumerable<KeyValuePair<string, (string HelpText, string Related)>> GetStatementHelp(
		FutureProgCompilationContext context)
	{
		return MudSharp.FutureProg.FutureProg.GetStatementHelpTexts(context)
			.Where(x => ComputerCompilationRestrictions.IsStatementHelpVisible(x.Key, context))
			.OrderBy(x => x.Key);
	}

	public (string HelpText, string Related)? GetStatementHelp(string statement, FutureProgCompilationContext context)
	{
		if (!ComputerCompilationRestrictions.IsStatementHelpVisible(statement, context))
		{
			return null;
		}

		return MudSharp.FutureProg.FutureProg.TryGetStatementHelp(statement, context, out var help)
			? help
			: null;
	}

	public IEnumerable<ComputerFunctionHelpInfo> GetFunctionHelp(FutureProgCompilationContext context)
	{
		return MudSharp.FutureProg.FutureProg.GetFunctionCompilerInformations()
			.Where(x => ComputerCompilationRestrictions.IsBuiltInFunctionHelpVisible(x, context))
			.OrderBy(x => x.Category)
			.ThenBy(x => x.FunctionName)
			.ThenBy(x => x.Parameters.Count())
			.Select(x => new ComputerFunctionHelpInfo
			{
				FunctionName = x.FunctionName,
				Parameters = x.Parameters.ToList(),
				ParameterNames = x.ParameterNames?.ToList() ?? [],
				ParameterHelp = x.ParameterHelp?.ToList() ?? [],
				FunctionHelp = x.FunctionHelp ?? string.Empty,
				Category = x.Category ?? string.Empty,
				ReturnType = x.ReturnType,
				AllowedContexts = x.AllowedContexts.ToList()
			});
	}

	public IEnumerable<ComputerCollectionHelpInfo> GetCollectionHelp(
		FutureProgCompilationContext context)
	{
		return CollectionExtensionFunction.FunctionCompilerInformations
			.Where(x => ComputerCompilationRestrictions.IsCollectionHelpVisible(x, context))
			.OrderBy(x => x.FunctionName)
			.Select(x => new ComputerCollectionHelpInfo
			{
				FunctionName = x.FunctionName,
				InnerFunctionReturnType = x.InnerFunctionReturnType,
				FunctionReturnInfo = x.FunctionReturnInfo,
				FunctionHelp = x.FunctionHelp
			});
	}
}
