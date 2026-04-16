#nullable enable

using System;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp.Computers;

public static class MicrocontrollerLogicCompiler
{
	private static readonly Regex VariableNameRegex = new(@"^[a-z][\w]*$", RegexOptions.IgnoreCase);

	public static bool IsValidVariableName(string variableName)
	{
		return !string.IsNullOrWhiteSpace(variableName) && VariableNameRegex.IsMatch(variableName);
	}

	public static (IFutureProg? Prog, string Error) Compile(
		IFuturemud gameworld,
		string functionName,
		IEnumerable<string> variableNames,
		string logicText)
	{
		var names = variableNames
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Trim().ToLowerInvariant())
			.ToList();

		var invalidName = names.FirstOrDefault(x => !IsValidVariableName(x));
		if (invalidName is not null)
		{
			return (null, $"The variable name {invalidName.ColourCommand()} is not valid.");
		}

		if (names.Count != names.Distinct(StringComparer.InvariantCultureIgnoreCase).Count())
		{
			return (null, "You cannot have duplicate microcontroller input variable names.");
		}

		var body = logicText?.Trim() ?? string.Empty;
		if (string.IsNullOrEmpty(body))
		{
			return (null, "You must enter some controller logic.");
		}

		var prog = new MudSharp.FutureProg.FutureProg(
			gameworld,
			functionName,
			ProgVariableTypes.Number,
			names.Select(x => Tuple.Create(ProgVariableTypes.Number, x)),
			body,
			FutureProgCompilationContext.ComputerFunction);

		return prog.Compile()
			? (prog, string.Empty)
			: (null, prog.CompileError ?? "Unknown compilation error.");
	}
}
