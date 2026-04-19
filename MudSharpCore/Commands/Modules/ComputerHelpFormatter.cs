#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Computers;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Commands.Modules;

internal static class ComputerHelpFormatter
{
	public static string DescribeAvailability(IEnumerable<FutureProgCompilationContext> contexts)
	{
		var contextSet = contexts.ToHashSet();
		var function = contextSet.Contains(FutureProgCompilationContext.ComputerFunction);
		var program = contextSet.Contains(FutureProgCompilationContext.ComputerProgram);
		if (function && program)
		{
			return "Both";
		}

		if (function)
		{
			return "Function";
		}

		if (program)
		{
			return "Program";
		}

		return "Unknown";
	}

	public static string GetTypesText(IEnumerable<ProgVariableTypes> types, bool colour)
	{
		StringBuilder sb = new();
		sb.AppendLine("There are the following programming-safe types:");
		foreach (var type in types.OrderBy(x => x.DescribeEnum()))
		{
			if (type == ProgVariableTypes.Collection)
			{
				sb.AppendLine("\t<OtherType> Collection".FluentColour(Telnet.VariableGreen, colour));
				continue;
			}

			if (type == ProgVariableTypes.Dictionary)
			{
				sb.AppendLine("\t<OtherType> Dictionary".FluentColour(Telnet.VariableGreen, colour));
				continue;
			}

			if (type == ProgVariableTypes.CollectionDictionary)
			{
				sb.AppendLine("\t<OtherType> CollectionDictionary".FluentColour(Telnet.VariableGreen, colour));
				continue;
			}

			sb.AppendLine($"\t{type.DescribeEnum().FluentColour(Telnet.VariableGreen, colour)}");
		}

		return sb.ToString();
	}

	public static string GetStatementsText(
		IEnumerable<KeyValuePair<string, (string HelpText, string Related)>> helps,
		Func<string, string> availabilitySelector,
		int linewidth,
		bool unicode,
		bool colour)
	{
		StringBuilder sb = new();
		sb.AppendLine("Statement Help");
		sb.AppendLine();
		sb.AppendLine(
			@"Statements are lines within computer functions or programs that perform critical tasks such as branching, returning, declaring variables and, for programs, suspending with sleep.

Statements are either line statements or block statements. A built-in function can also be used as a statement on a line.");
		sb.AppendLine();
		sb.AppendLine("There are the following computer-programming statement types:");
		sb.AppendLine(StringUtilities.GetTextTable(
			helps.Select(item => new List<string>
			{
				item.Key,
				availabilitySelector(item.Key),
				item.Value.Related
			}),
			new List<string>
			{
				"Statement",
				"Availability",
				"Related Statements"
			},
			linewidth,
			colour,
			colour ? Telnet.BoldMagenta : null,
			2,
			unicode
		));

		return sb.ToString();
	}

	public static string GetStatementText(
		string statement,
		(string HelpText, string Related) help,
		string availability,
		int linewidth,
		bool colour)
	{
		var baseText = ProgModule.GetTextProgHelpStatement(help, statement, linewidth, colour);
		return $"{baseText.TrimEnd()}\nAvailability: {availability.ColourName(colour)}\n";
	}

	public static string GetFunctionText(
		IEnumerable<ComputerFunctionHelpInfo> functions,
		int linewidth,
		int innerwidth,
		bool unicode,
		bool colour)
	{
		StringBuilder sb = new();
		foreach (var function in functions)
		{
			sb.AppendLine();
			sb.AppendLine(GetFunctionDisplayForm(function, colour).StripANSIColour(!colour).GetLineWithTitle(
				linewidth,
				unicode,
				colour ? Telnet.BoldMagenta : null,
				null));
			sb.AppendLine();
			sb.AppendLine(function.FunctionHelp.Wrap(innerwidth).FluentColour(Telnet.Yellow, colour));
			sb.AppendLine();
			sb.AppendLine($"Availability: {DescribeAvailability(function.AllowedContexts).ColourName(colour)}");
			for (var i = 0; i < function.Parameters.Count(); i++)
			{
				sb.Append("\t");
				sb.Append(function.Parameters.ElementAt(i).Describe().FluentColour(Telnet.Cyan, colour));
				sb.Append(" ");
				sb.Append(function.ParameterNames.ElementAtOrDefault(i) ?? $"var{i}");
				sb.Append(": ");
				sb.AppendLine(function.ParameterHelp.ElementAtOrDefault(i)?.FluentColour(Telnet.Yellow, colour) ??
				              "no help available".ColourError(colour));
			}

			sb.AppendLine();
		}

		return sb.ToString();
	}

	public static string GetFunctionsText(IEnumerable<ComputerFunctionHelpInfo> infos, int linewidth, bool unicode,
		bool colour)
	{
		StringBuilder sb = new();
		sb.AppendLine("There are the following programming-safe built-in functions:");
		sb.AppendLine();
		foreach (var category in infos.GroupBy(x => x.Category))
		{
			sb.AppendLine();
			sb.AppendLine(category.Key.GetLineWithTitle(linewidth, unicode,
				colour ? Telnet.BoldGreen : null,
				colour ? Telnet.BoldYellow : null));
			sb.AppendLine();
			foreach (var function in category
				         .OrderBy(x => x.FunctionName)
				         .ThenBy(x => x.Parameters.Count()))
			{
				sb.AppendLine($"\t{GetFunctionDisplayForm(function, colour)} [{DescribeAvailability(function.AllowedContexts)}]");
			}
		}

		return sb.ToString();
	}

	public static string GetCollectionText(ComputerCollectionHelpInfo info, int linewidth, bool colour)
	{
		StringBuilder sb = new();
		sb.AppendLine($@"#5Collection Extension Function - {info.FunctionName.ToUpperInvariant()}#0

Inner Function Type: {info.InnerFunctionReturnType.DescribeEnum().ColourName(colour)}
Return Type Info: {info.FunctionReturnInfo.ColourName(colour)}");
		var line = info.FunctionHelp.Wrap(linewidth, "\t").FluentColourIncludingReset(Telnet.Yellow, colour)
			.SubstituteANSIColour();
		sb.AppendLine(colour ? line : line.StripANSIColour());
		return sb.ToString();
	}

	public static string GetCollectionsText(IEnumerable<ComputerCollectionHelpInfo> infos, int linewidth, bool unicode,
		bool colour)
	{
		StringBuilder sb = new();
		sb.AppendLine("Help on Collection Functions".ColourName());
		sb.AppendLine();
		sb.AppendLine(@"These functions are accessed by doing something in the following form after a collection variable:

	#`156;220;254;@CollectionVariable#0.#`220;220;170;FunctionName#0(ItemVariableName#0, InnerFunction#0)#0

Where:
	
	#3CollectionVariable#0 is any variable or function returning a collection
	#3FunctionName#0 is the specific collection extension function you want to run
	#3ItemVariableName#0 is a variable name that will be used inside the inner function to refer to each item in the collection
	#3InnerFunction#0 is a function run on each element in the collection".SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine("Collection Functions:");
		sb.AppendLine(StringUtilities.GetTextTable(
			infos.Select(item => new List<string>
			{
				item.FunctionName.ToUpperInvariant(),
				item.InnerFunctionReturnType.DescribeEnum(),
				item.FunctionReturnInfo
			}),
			new List<string>
			{
				"Function",
				"Inner Function",
				"Return Type"
			},
			linewidth,
			colour,
			colour ? Telnet.BoldMagenta : null,
			1,
			unicode
		));

		return colour ? sb.ToString() : sb.ToString().StripANSIColour();
	}

	private static string GetFunctionDisplayForm(ComputerFunctionHelpInfo function, bool colour)
	{
		StringBuilder sb = new();
		sb.Append(function.ReturnType.Describe().Colour(Telnet.Cyan));
		sb.Append(" ");
		sb.Append(function.FunctionName.ToUpperInvariant().Colour(Telnet.Yellow));
		sb.Append("(");
		for (var i = 0; i < function.Parameters.Count(); i++)
		{
			if (i > 0)
			{
				sb.Append(", ");
			}

			sb.Append(function.Parameters.ElementAt(i).Describe().Colour(Telnet.Cyan));
			sb.Append(" ");
			sb.Append(function.ParameterNames.ElementAtOrDefault(i) ?? $"var{i}");
		}

		sb.Append(")");
		return colour ? sb.ToString() : sb.ToString().StripANSIColour();
	}
}
