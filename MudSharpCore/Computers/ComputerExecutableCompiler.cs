#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Computers;

public static class ComputerExecutableCompiler
{
	private static readonly Regex VariableNameRegex = new(@"^[a-z][\w]*$", RegexOptions.IgnoreCase);

	public static bool IsValidVariableName(string variableName)
	{
		return !string.IsNullOrWhiteSpace(variableName) && VariableNameRegex.IsMatch(variableName);
	}

	public static FutureProgCompilationContext GetCompilationContext(ComputerExecutableKind kind)
	{
		return kind switch
		{
			ComputerExecutableKind.Function => FutureProgCompilationContext.ComputerFunction,
			ComputerExecutableKind.Program => FutureProgCompilationContext.ComputerProgram,
			_ => FutureProgCompilationContext.ComputerProgram
		};
	}

	public static (MudSharp.FutureProg.FutureProg? Prog, string Error) Compile(
		IFuturemud gameworld,
		string functionName,
		ComputerExecutableKind kind,
		ProgVariableTypes returnType,
		IEnumerable<ComputerExecutableParameter> parameters,
		string sourceCode)
	{
		var normalisedParameters = parameters
			.Select(x => new ComputerExecutableParameter((x.Name ?? string.Empty).Trim().ToLowerInvariant(), x.Type))
			.ToList();

		var invalidName = normalisedParameters.FirstOrDefault(x => !IsValidVariableName(x.Name));
		if (invalidName != default)
		{
			return (null, $"The variable name {invalidName.Name.ColourCommand()} is not valid.");
		}

		if (normalisedParameters.Count != normalisedParameters
			    .Select(x => x.Name)
			    .Distinct(StringComparer.InvariantCultureIgnoreCase)
			    .Count())
		{
			return (null, "You cannot have duplicate computer executable parameter names.");
		}

		var body = sourceCode?.Trim() ?? string.Empty;
		if (string.IsNullOrEmpty(body))
		{
			return (null, "You must enter some source code.");
		}

		var prog = new MudSharp.FutureProg.FutureProg(
			gameworld,
			functionName,
			returnType,
			normalisedParameters.Select(x => Tuple.Create(x.Type, x.Name)),
			body,
			GetCompilationContext(kind));

		return prog.Compile()
			? (prog, string.Empty)
			: (null, prog.CompileError ?? "Unknown compilation error.");
	}
}
