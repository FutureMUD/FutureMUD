#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.FutureProg;

namespace MudSharp.Computers;

public interface IComputerProcess
{
	long Id { get; }
	string ProcessName { get; }
	long OwnerCharacterId { get; }
	IComputerProgramDefinition Program { get; }
	IComputerHost Host { get; }
	ComputerProcessStatus Status { get; }
	ComputerProcessWaitType WaitType { get; }
	DateTime? WakeTimeUtc { get; }
	string? WaitArgument { get; }
	bool IsRunning { get; }
	ComputerPowerLossBehaviour PowerLossBehaviour { get; }
	object? Result { get; }
	string? LastError { get; }
	DateTime StartedAtUtc { get; }
	DateTime LastUpdatedAtUtc { get; }
	DateTime? EndedAtUtc { get; }
}

public interface IComputerFile
{
	string FileName { get; }
	string TextContents { get; }
	long SizeInBytes { get; }
	DateTime CreatedAtUtc { get; }
	DateTime LastModifiedAtUtc { get; }
}

public interface IComputerFileSystem
{
	long CapacityInBytes { get; }
	long UsedBytes { get; }
	IEnumerable<IComputerFile> Files { get; }
}

public interface IComputerBuiltInApplication
{
	string Id { get; }
	string Name { get; }
	string Summary { get; }
	bool IsNetworkService { get; }
}

public interface IComputerHost
{
	bool Powered { get; }
	IComputerFileSystem? FileSystem { get; }
	IEnumerable<IComputerExecutable> Executables { get; }
	IEnumerable<IComputerProcess> Processes { get; }
	IEnumerable<IComputerBuiltInApplication> BuiltInApplications { get; }
}

public interface ICharacterComputerWorkspace
{
	ICharacter Owner { get; }
	IEnumerable<IComputerExecutableDefinition> Executables { get; }
	IEnumerable<IComputerProcess> Processes { get; }
}

public interface IComputerExecutionService
{
	ICharacterComputerWorkspace GetWorkspace(ICharacter owner);
	IEnumerable<IComputerExecutableDefinition> GetExecutables(ICharacter owner);
	IComputerExecutableDefinition? GetExecutable(ICharacter owner, string identifier);
	IComputerExecutableDefinition? GetExecutable(long id);
	IComputerExecutableDefinition CreateExecutable(ICharacter owner, ComputerExecutableKind kind, string name);
	void SaveExecutable(IComputerExecutableDefinition executable);
	bool DeleteExecutable(ICharacter owner, IComputerExecutableDefinition executable, out string error);
	ComputerCompilationResult CompileExecutable(IComputerExecutableDefinition executable);
	ComputerExecutionResult Execute(ICharacter owner, IComputerExecutableDefinition executable,
		IEnumerable<object?> parameters);
	IEnumerable<IComputerProcess> GetProcesses(ICharacter owner);
	IComputerProcess? GetProcess(ICharacter owner, long processId);
	bool KillProcess(ICharacter owner, long processId, out string error);
}

public interface IComputerHelpService
{
	IEnumerable<ProgVariableTypes> GetAvailableTypes(FutureProgCompilationContext context);
	IEnumerable<KeyValuePair<string, (string HelpText, string Related)>> GetStatementHelp(
		FutureProgCompilationContext context);
	(string HelpText, string Related)? GetStatementHelp(string statement, FutureProgCompilationContext context);
	IEnumerable<ComputerFunctionHelpInfo> GetFunctionHelp(
		FutureProgCompilationContext context);
	IEnumerable<ComputerCollectionHelpInfo> GetCollectionHelp(
		FutureProgCompilationContext context);
}

public sealed class ComputerFunctionHelpInfo
{
	public string FunctionName { get; init; } = string.Empty;
	public IEnumerable<ProgVariableTypes> Parameters { get; init; } = Array.Empty<ProgVariableTypes>();
	public IEnumerable<string> ParameterNames { get; init; } = Array.Empty<string>();
	public IEnumerable<string> ParameterHelp { get; init; } = Array.Empty<string>();
	public string FunctionHelp { get; init; } = string.Empty;
	public string Category { get; init; } = string.Empty;
	public ProgVariableTypes ReturnType { get; init; } = ProgVariableTypes.Error;
	public IEnumerable<FutureProgCompilationContext> AllowedContexts { get; init; } =
		Array.Empty<FutureProgCompilationContext>();
}

public sealed class ComputerCollectionHelpInfo
{
	public string FunctionName { get; init; } = string.Empty;
	public ProgVariableTypes InnerFunctionReturnType { get; init; } = ProgVariableTypes.Error;
	public string FunctionReturnInfo { get; init; } = string.Empty;
	public string FunctionHelp { get; init; } = string.Empty;
}
