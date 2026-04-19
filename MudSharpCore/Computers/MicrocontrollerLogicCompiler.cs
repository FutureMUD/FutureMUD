#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Computers;

public static class MicrocontrollerLogicCompiler
{
	public static bool IsValidVariableName(string variableName)
	{
		return ComputerExecutableCompiler.IsValidVariableName(variableName);
	}

	public static (IFutureProg? Prog, string Error) Compile(
		IFuturemud gameworld,
		string functionName,
		IEnumerable<string> variableNames,
		string logicText)
	{
		return ComputerExecutableCompiler.Compile(
			gameworld,
			functionName,
			ComputerExecutableKind.Function,
			ProgVariableTypes.Number,
			variableNames.Select(x => new ComputerExecutableParameter(x, ProgVariableTypes.Number)),
			logicText);
	}
}
