#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.FutureProg;

namespace MudSharp.Computers;

public class ComputerExecutableDefinition : IComputerExecutableDefinition
{
	public long Id { get; init; }
	public string Name { get; init; } = string.Empty;
	public string FrameworkItemType { get; init; } = "ComputerExecutable";
	public string SourceCode { get; init; } = string.Empty;
	public ProgVariableTypes ReturnType { get; init; } = ProgVariableTypes.Void;
	public IReadOnlyCollection<ComputerExecutableParameter> Parameters { get; init; } =
		Array.Empty<ComputerExecutableParameter>();
	public FutureProgCompilationContext CompilationContext { get; init; } =
		FutureProgCompilationContext.ComputerProgram;
	public ComputerCompilationStatus CompilationStatus { get; init; } = ComputerCompilationStatus.NotCompiled;
	public string CompileError { get; init; } = string.Empty;
	public long? OwnerCharacterId { get; init; }
	public long? OwnerHostItemId { get; init; }
	public long? OwnerStorageItemId { get; init; }
	public ComputerExecutableKind ExecutableKind { get; init; } = ComputerExecutableKind.Program;
	public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
	public DateTime LastModifiedAtUtc { get; init; } = DateTime.UtcNow;
}

public sealed class ComputerFunctionDefinition : ComputerExecutableDefinition, IComputerFunction
{
}

public sealed class ComputerProgramDefinition : ComputerExecutableDefinition, IComputerProgramDefinition
{
	public bool AutorunOnBoot { get; init; }
}

public sealed class ComputerBuiltInApplicationProgramDefinition : ComputerExecutableDefinition, IComputerBuiltInApplication
{
	public string ApplicationId { get; init; } = string.Empty;
	public string Summary { get; init; } = string.Empty;
	public bool IsNetworkService { get; init; }
	public bool AutorunOnBoot => false;
}

public sealed class ComputerProcessDefinition : IComputerProcess
{
	public long Id { get; init; }
	public string ProcessName { get; init; } = string.Empty;
	public long OwnerCharacterId { get; init; }
	public required IComputerProgramDefinition Program { get; init; }
	public required IComputerHost Host { get; init; }
	public ComputerProcessStatus Status { get; init; } = ComputerProcessStatus.NotStarted;
	public ComputerProcessWaitType WaitType { get; init; }
	public DateTime? WakeTimeUtc { get; init; }
	public string? WaitArgument { get; init; }
	public long? WaitingCharacterId { get; init; }
	public long? WaitingTerminalItemId { get; init; }
	public bool IsRunning => Status is ComputerProcessStatus.Running or ComputerProcessStatus.Sleeping;
	public ComputerPowerLossBehaviour PowerLossBehaviour { get; init; } = ComputerPowerLossBehaviour.Terminate;
	public object? Result { get; init; }
	public string? LastError { get; init; }
	public DateTime StartedAtUtc { get; init; } = DateTime.UtcNow;
	public DateTime LastUpdatedAtUtc { get; init; } = DateTime.UtcNow;
	public DateTime? EndedAtUtc { get; init; }
}

public sealed class ComputerTextFile : IComputerFile
{
	public string FileName { get; init; } = string.Empty;
	public string TextContents { get; init; } = string.Empty;
	public long SizeInBytes => Encoding.UTF8.GetByteCount(TextContents ?? string.Empty);
	public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
	public DateTime LastModifiedAtUtc { get; init; } = DateTime.UtcNow;
	public bool PubliclyAccessible { get; init; }
}

public sealed class ComputerFileSystemDefinition : IComputerFileSystem
{
	public long CapacityInBytes { get; init; }
	public IEnumerable<IComputerFile> Files { get; init; } = Enumerable.Empty<IComputerFile>();
	public long UsedBytes => Files.Sum(x => x.SizeInBytes);
	public bool FileExists(string fileName)
	{
		return Files.Any(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
	}

	public IComputerFile? GetFile(string fileName)
	{
		return Files.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
	}

	public string ReadFile(string fileName)
	{
		return GetFile(fileName)?.TextContents ?? string.Empty;
	}

	public void WriteFile(string fileName, string textContents)
	{
		throw new NotSupportedException("ComputerFileSystemDefinition is read-only.");
	}

	public void AppendFile(string fileName, string textContents)
	{
		throw new NotSupportedException("ComputerFileSystemDefinition is read-only.");
	}

	public bool DeleteFile(string fileName)
	{
		throw new NotSupportedException("ComputerFileSystemDefinition is read-only.");
	}

	public bool SetFilePubliclyAccessible(string fileName, bool isPublic)
	{
		throw new NotSupportedException("ComputerFileSystemDefinition is read-only.");
	}
}

public sealed class ComputerHostDefinition : IComputerHost
{
	public string Name { get; init; } = string.Empty;
	public long? OwnerCharacterId { get; init; }
	public long? OwnerHostItemId { get; init; }
	public long? OwnerStorageItemId { get; init; }
	public IComputerHost ExecutionHost => this;
	public bool Powered { get; init; }
	public IComputerFileSystem? FileSystem { get; init; }
	public IEnumerable<IComputerExecutableDefinition> Executables { get; init; } =
		Enumerable.Empty<IComputerExecutableDefinition>();
	public IEnumerable<IComputerProcess> Processes { get; init; } = Enumerable.Empty<IComputerProcess>();
	public IEnumerable<IComputerBuiltInApplication> BuiltInApplications { get; init; } =
		Enumerable.Empty<IComputerBuiltInApplication>();
	public IEnumerable<IComputerStorage> MountedStorage { get; init; } = Enumerable.Empty<IComputerStorage>();
	public IEnumerable<IComputerTerminal> ConnectedTerminals { get; init; } = Enumerable.Empty<IComputerTerminal>();
	public IEnumerable<INetworkAdapter> NetworkAdapters { get; init; } = Enumerable.Empty<INetworkAdapter>();
	public IEnumerable<string> EnabledNetworkServices => Enumerable.Empty<string>();

	public IComputerProcess? GetProcess(long processId)
	{
		return Processes.FirstOrDefault(x => x.Id == processId);
	}

	public bool IsNetworkServiceEnabled(string applicationId)
	{
		return false;
	}

	public bool SetNetworkServiceEnabled(string applicationId, bool enabled, out string error)
	{
		error = "This computer host definition is read-only.";
		return false;
	}
}
