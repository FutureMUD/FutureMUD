#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Computers;

public abstract class ComputerWorkspaceExecutableBase : IComputerExecutableDefinition
{
	protected ComputerWorkspaceExecutableBase(long id, IFuturemud gameworld)
	{
		Id = id;
		Gameworld = gameworld;
	}

	protected IFuturemud Gameworld { get; }
	internal MudSharp.FutureProg.FutureProg? CompiledProg { get; set; }

	public long Id { get; protected set; }
	public string Name { get; set; } = string.Empty;
	public string FrameworkItemType => "ComputerExecutable";
	public string SourceCode { get; set; } = string.Empty;
	public ProgVariableTypes ReturnType { get; set; } = ProgVariableTypes.Void;
	public IReadOnlyCollection<ComputerExecutableParameter> Parameters { get; set; } =
		Array.Empty<ComputerExecutableParameter>();
	public FutureProgCompilationContext CompilationContext => ComputerExecutableCompiler.GetCompilationContext(ExecutableKind);
	public ComputerCompilationStatus CompilationStatus { get; set; }
	public string CompileError { get; set; } = string.Empty;
	public long? OwnerCharacterId { get; set; }
	public long? OwnerHostItemId { get; set; }
	public long? OwnerStorageItemId { get; set; }
	public abstract ComputerExecutableKind ExecutableKind { get; }
	public DateTime CreatedAtUtc { get; set; }
	public DateTime LastModifiedAtUtc { get; set; }
}

public sealed class ComputerWorkspaceFunction : ComputerWorkspaceExecutableBase, IComputerFunction
{
	public ComputerWorkspaceFunction(long id, IFuturemud gameworld)
		: base(id, gameworld)
	{
	}

	public override ComputerExecutableKind ExecutableKind => ComputerExecutableKind.Function;
}

public sealed class ComputerWorkspaceProgram : ComputerWorkspaceExecutableBase, IComputerProgramDefinition
{
	public ComputerWorkspaceProgram(long id, IFuturemud gameworld)
		: base(id, gameworld)
	{
	}

	public override ComputerExecutableKind ExecutableKind => ComputerExecutableKind.Program;
	public bool AutorunOnBoot { get; set; }
}

public sealed class ComputerWorkspaceProcess : IComputerProcess, IFrameworkItem
{
	public long Id { get; set; }
	public string Name => ProcessName;
	public string FrameworkItemType => "ComputerProcess";
	public string ProcessName { get; set; } = string.Empty;
	public long OwnerCharacterId { get; set; }
	public required IComputerProgramDefinition Program { get; init; }
	public required IComputerHost Host { get; init; }
	public ComputerProcessStatus Status { get; set; }
	public ComputerProcessWaitType WaitType { get; set; }
	public DateTime? WakeTimeUtc { get; set; }
	public string? WaitArgument { get; set; }
	public bool IsRunning => Status is ComputerProcessStatus.Running or ComputerProcessStatus.Sleeping;
	public ComputerPowerLossBehaviour PowerLossBehaviour { get; set; } = ComputerPowerLossBehaviour.Terminate;
	public object? Result { get; set; }
	public string? LastError { get; set; }
	public DateTime StartedAtUtc { get; set; }
	public DateTime LastUpdatedAtUtc { get; set; }
	public DateTime? EndedAtUtc { get; set; }
	internal string StateJson { get; set; } = string.Empty;
}

public sealed class CharacterComputerWorkspace : ICharacterComputerWorkspace
{
	private readonly Func<IEnumerable<IComputerExecutableDefinition>> _executableSource;
	private readonly Func<IEnumerable<IComputerProcess>> _processSource;

	public CharacterComputerWorkspace(ICharacter owner,
		Func<IEnumerable<IComputerExecutableDefinition>> executableSource,
		Func<IEnumerable<IComputerProcess>> processSource)
	{
		Owner = owner;
		_executableSource = executableSource;
		_processSource = processSource;
	}

	public ICharacter Owner { get; }
	public IEnumerable<IComputerExecutableDefinition> Executables => _executableSource();
	public IEnumerable<IComputerProcess> Processes => _processSource();
}

public sealed class CharacterWorkspaceHost : IComputerHost
{
	private readonly Func<IEnumerable<IComputerExecutable>> _executables;
	private readonly Func<IEnumerable<IComputerProcess>> _processes;

	public CharacterWorkspaceHost(IFuturemud gameworld, long ownerCharacterId,
		Func<IEnumerable<IComputerExecutable>> executables,
		Func<IEnumerable<IComputerProcess>> processes)
	{
		Gameworld = gameworld;
		OwnerCharacterId = ownerCharacterId;
		_executables = executables;
		_processes = processes;
	}

	public IFuturemud Gameworld { get; }
	public long OwnerCharacterId { get; }
	public bool Powered => true;
	public IComputerFileSystem? FileSystem => null;
	public IEnumerable<IComputerExecutable> Executables => _executables();
	public IEnumerable<IComputerProcess> Processes => _processes();
	public IEnumerable<IComputerBuiltInApplication> BuiltInApplications => ComputerBuiltInApplications.All;
}
