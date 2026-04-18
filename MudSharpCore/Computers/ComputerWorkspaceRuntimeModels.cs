#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Computers;

public abstract class ComputerWorkspaceExecutableBase : ComputerRuntimeExecutableBase
{
	protected ComputerWorkspaceExecutableBase(long id, IFuturemud gameworld)
		: base(id, gameworld)
	{
	}
}

public sealed class ComputerWorkspaceFunction : ComputerRuntimeFunctionBase
{
	public ComputerWorkspaceFunction(long id, IFuturemud gameworld)
		: base(id, gameworld)
	{
	}
}

public sealed class ComputerWorkspaceProgram : ComputerRuntimeProgramBase
{
	public ComputerWorkspaceProgram(long id, IFuturemud gameworld)
		: base(id, gameworld)
	{
	}
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
	public long? WaitingCharacterId { get; set; }
	public long? WaitingTerminalItemId { get; set; }
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
	public string Name => $"{Owner.HowSeen(Owner, true)} workspace";
	public long? OwnerCharacterId => Owner.Id;
	public long? OwnerHostItemId => null;
	public long? OwnerStorageItemId => null;
	public IComputerHost ExecutionHost => new CharacterWorkspaceHost(Owner.Gameworld, Owner.Id,
		() => Executables,
		() => Processes);
	public IComputerFileSystem? FileSystem => null;
	public IEnumerable<IComputerExecutableDefinition> Executables => _executableSource();
	public IEnumerable<IComputerProcess> Processes => _processSource();
}

public sealed class CharacterWorkspaceHost : IComputerHost
{
	private readonly Func<IEnumerable<IComputerExecutableDefinition>> _executables;
	private readonly Func<IEnumerable<IComputerProcess>> _processes;

	public CharacterWorkspaceHost(IFuturemud gameworld, long ownerCharacterId,
		Func<IEnumerable<IComputerExecutableDefinition>> executables,
		Func<IEnumerable<IComputerProcess>> processes)
	{
		Gameworld = gameworld;
		OwnerCharacterId = ownerCharacterId;
		_executables = executables;
		_processes = processes;
	}

	public IFuturemud Gameworld { get; }
	public long OwnerCharacterId { get; }
	long? IComputerExecutableOwner.OwnerCharacterId => OwnerCharacterId;
	public long? OwnerHostItemId => null;
	public long? OwnerStorageItemId => null;
	public string Name => $"Workspace Host [{OwnerCharacterId:N0}]";
	public IComputerHost ExecutionHost => this;
	public bool Powered => true;
	public IComputerFileSystem? FileSystem => null;
	public IEnumerable<IComputerExecutableDefinition> Executables => _executables();
	public IEnumerable<IComputerProcess> Processes => _processes();
	public IEnumerable<IComputerBuiltInApplication> BuiltInApplications => Enumerable.Empty<IComputerBuiltInApplication>();
	public IEnumerable<IComputerStorage> MountedStorage => Enumerable.Empty<IComputerStorage>();
	public IEnumerable<IComputerTerminal> ConnectedTerminals => Enumerable.Empty<IComputerTerminal>();
	public IEnumerable<INetworkAdapter> NetworkAdapters => Enumerable.Empty<INetworkAdapter>();
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
		error = "Workspace hosts cannot advertise network services.";
		return false;
	}
}
