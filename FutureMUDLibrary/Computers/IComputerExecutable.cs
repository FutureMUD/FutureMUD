#nullable enable

using MudSharp.FutureProg;
using System.Collections.Generic;
using MudSharp.Framework;

namespace MudSharp.Computers;

public interface IComputerExecutable : IFrameworkItem
{
	string SourceCode { get; }
	ProgVariableTypes ReturnType { get; }
	IReadOnlyCollection<ComputerExecutableParameter> Parameters { get; }
	FutureProgCompilationContext CompilationContext { get; }
	ComputerCompilationStatus CompilationStatus { get; }
	string CompileError { get; }
}

public interface IComputerExecutableDefinition : IComputerExecutable
{
	long? OwnerCharacterId { get; }
	long? OwnerHostItemId { get; }
	long? OwnerStorageItemId { get; }
	ComputerExecutableKind ExecutableKind { get; }
	System.DateTime CreatedAtUtc { get; }
	System.DateTime LastModifiedAtUtc { get; }
}

public interface IComputerFunction : IComputerExecutableDefinition
{
}

public interface IComputerProgramDefinition : IComputerExecutableDefinition
{
	bool AutorunOnBoot { get; }
}

public sealed class ComputerCompilationResult
{
	public bool Success { get; init; }
	public string ErrorMessage { get; init; } = string.Empty;
	public IComputerExecutableDefinition Executable { get; init; } = null!;
}

public sealed class ComputerExecutionResult
{
	public bool Success { get; init; }
	public string ErrorMessage { get; init; } = string.Empty;
	public ComputerProcessStatus Status { get; init; }
	public object? Result { get; init; }
	public IComputerProcess? Process { get; init; }
}
