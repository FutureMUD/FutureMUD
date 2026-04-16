#nullable enable

using MudSharp.FutureProg;
using System.Collections.Generic;

namespace MudSharp.Computers;

public interface IComputerExecutable
{
	string Name { get; }
	string SourceCode { get; }
	ProgVariableTypes ReturnType { get; }
	IReadOnlyCollection<ComputerExecutableParameter> Parameters { get; }
	FutureProgCompilationContext CompilationContext { get; }
	ComputerCompilationStatus CompilationStatus { get; }
	string CompileError { get; }
}

public interface IComputerFunction : IComputerExecutable
{
}

public interface IComputerProgramDefinition : IComputerExecutable
{
	bool AutorunOnBoot { get; }
}
