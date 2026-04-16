#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.FutureProg;

namespace MudSharp.Computers;

public class ComputerExecutableDefinition : IComputerExecutable
{
	public string Name { get; init; } = string.Empty;
	public string SourceCode { get; init; } = string.Empty;
	public ProgVariableTypes ReturnType { get; init; } = ProgVariableTypes.Void;
	public IReadOnlyCollection<ComputerExecutableParameter> Parameters { get; init; } =
		Array.Empty<ComputerExecutableParameter>();
	public FutureProgCompilationContext CompilationContext { get; init; } =
		FutureProgCompilationContext.ComputerProgram;
	public ComputerCompilationStatus CompilationStatus { get; init; } = ComputerCompilationStatus.NotCompiled;
	public string CompileError { get; init; } = string.Empty;
}

public sealed class ComputerFunctionDefinition : ComputerExecutableDefinition, IComputerFunction
{
}

public sealed class ComputerProgramDefinition : ComputerExecutableDefinition, IComputerProgramDefinition
{
	public bool AutorunOnBoot { get; init; }
}

public sealed class ComputerProcessDefinition : IComputerProcess
{
	public string ProcessName { get; init; } = string.Empty;
	public required IComputerProgramDefinition Program { get; init; }
	public required IComputerHost Host { get; init; }
	public ComputerProcessWaitType WaitType { get; init; }
	public DateTime? WakeTimeUtc { get; init; }
	public string? WaitArgument { get; init; }
	public bool IsRunning { get; init; }
	public ComputerPowerLossBehaviour PowerLossBehaviour { get; init; } = ComputerPowerLossBehaviour.Terminate;
}

public sealed class ComputerTextFile : IComputerFile
{
	public string FileName { get; init; } = string.Empty;
	public string TextContents { get; init; } = string.Empty;
	public long SizeInBytes => Encoding.UTF8.GetByteCount(TextContents ?? string.Empty);
	public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
	public DateTime LastModifiedAtUtc { get; init; } = DateTime.UtcNow;
}

public sealed class ComputerFileSystemDefinition : IComputerFileSystem
{
	public long CapacityInBytes { get; init; }
	public IEnumerable<IComputerFile> Files { get; init; } = Enumerable.Empty<IComputerFile>();
	public long UsedBytes => Files.Sum(x => x.SizeInBytes);
}

public sealed class ComputerHostDefinition : IComputerHost
{
	public bool Powered { get; init; }
	public IComputerFileSystem? FileSystem { get; init; }
	public IEnumerable<IComputerExecutable> Executables { get; init; } = Enumerable.Empty<IComputerExecutable>();
	public IEnumerable<IComputerProcess> Processes { get; init; } = Enumerable.Empty<IComputerProcess>();
	public IEnumerable<IComputerBuiltInApplication> BuiltInApplications { get; init; } =
		ComputerBuiltInApplications.All;
}
