#nullable enable

using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class ComputerStorageGameItemComponent : GameItemComponent, IComputerStorage, IComputerMutableOwner, IConnectable
{
	private readonly Dictionary<long, ComputerRuntimeExecutableBase> _executables = new();
	private readonly Dictionary<long, ComputerRuntimeProcess> _processes = new();
	private readonly List<long> _pendingConnectionIds = [];
	private ComputerStorageGameItemComponentProto _prototype;
	private readonly ComputerMutableFileSystem _fileSystem;
	private IConnectable? _connectedHost;
	private long _nextExecutableId;
	private long _nextProcessId;

	public ComputerStorageGameItemComponent(ComputerStorageGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		_fileSystem = new ComputerMutableFileSystem(proto.StorageCapacityInBytes);
		_fileSystem.FileChanged += FileSystemOnFileChanged;
	}

	public ComputerStorageGameItemComponent(MudSharp.Models.GameItemComponent component,
		ComputerStorageGameItemComponentProto proto,
		IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_fileSystem = new ComputerMutableFileSystem(proto.StorageCapacityInBytes);
		_fileSystem.FileChanged += FileSystemOnFileChanged;
		LoadRuntimeState(XElement.Parse(component.Definition));
	}

	public ComputerStorageGameItemComponent(ComputerStorageGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_fileSystem = new ComputerMutableFileSystem(rhs._fileSystem.CapacityInBytes);
		_fileSystem.FileChanged += FileSystemOnFileChanged;
		_fileSystem.LoadFiles(rhs._fileSystem.MutableFiles.Select(x => new ComputerMutableTextFile
		{
			FileName = x.FileName,
			TextContents = x.TextContents,
			CreatedAtUtc = x.CreatedAtUtc,
			LastModifiedAtUtc = x.LastModifiedAtUtc,
			PubliclyAccessible = x.PubliclyAccessible
		}));
		foreach (var executable in rhs._executables.Values)
		{
			ComputerRuntimeExecutableBase clone;
			if (executable.ExecutableKind == ComputerExecutableKind.Function)
			{
				clone = new ComputerMutableFunction(executable.Id, Gameworld);
			}
			else
			{
				clone = new ComputerMutableProgram(executable.Id, Gameworld)
				{
					AutorunOnBoot = executable is IComputerProgramDefinition { AutorunOnBoot: true }
				};
			}
			CopyExecutableState(executable, clone);
			_executables[clone.Id] = clone;
		}

		foreach (var process in rhs._processes.Values)
		{
			if (!_executables.TryGetValue(process.Program.Id, out var executable) ||
			    executable is not ComputerRuntimeProgramBase program)
			{
				continue;
			}

			_processes[process.Id] = new ComputerRuntimeProcess
			{
				Id = process.Id,
				ProcessName = process.ProcessName,
				OwnerCharacterId = process.OwnerCharacterId,
				Program = program,
				Host = ExecutionHost,
				Status = process.Status,
				WaitType = process.WaitType,
				WakeTimeUtc = process.WakeTimeUtc,
				WaitArgument = process.WaitArgument,
				WaitingCharacterId = process.WaitingCharacterId,
				WaitingTerminalItemId = process.WaitingTerminalItemId,
				PowerLossBehaviour = process.PowerLossBehaviour,
				Result = process.Result,
				LastError = process.LastError,
				StartedAtUtc = process.StartedAtUtc,
				LastUpdatedAtUtc = process.LastUpdatedAtUtc,
				EndedAtUtc = process.EndedAtUtc
			};
		}

		_nextExecutableId = rhs._nextExecutableId;
		_nextProcessId = rhs._nextProcessId;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public string Name => Parent.Name;
	public long FileOwnerId => Parent.Id;
	public long? OwnerCharacterId => null;
	public long? OwnerHostItemId => null;
	public long? OwnerStorageItemId => Parent.Id;
	public long CapacityInBytes => _fileSystem.CapacityInBytes;
	public bool Mounted => MountedHost is not null;
	public IComputerHost? MountedHost => _connectedHost as IComputerHost;
	public IComputerHost ExecutionHost => MountedHost ?? new ComputerHostDefinition { Name = $"{Parent.Name} (offline)", Powered = false };
	public IComputerFileSystem? FileSystem => _fileSystem;
	public IEnumerable<IComputerExecutableDefinition> Executables => _executables.Values.OrderBy(x => x.Name).ThenBy(x => x.Id);
	public IEnumerable<IComputerProcess> Processes => _processes.Values.OrderByDescending(x => x.LastUpdatedAtUtc).ThenByDescending(x => x.Id);
	public IEnumerable<ConnectorType> Connections => [ComputerConnectionTypes.StoragePlug];
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems =>
		_connectedHost is null ? Enumerable.Empty<Tuple<ConnectorType, IConnectable>>() : [Tuple.Create(ComputerConnectionTypes.StoragePlug, _connectedHost)];
	public IEnumerable<ConnectorType> FreeConnections => _connectedHost is null ? Connections : Enumerable.Empty<ConnectorType>();
	public bool Independent => true;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ComputerStorageGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour,
		PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return description;
		}

		var sb = new StringBuilder(description);
		sb.AppendLine();
		sb.AppendLine();
		sb.AppendLine(
			$"Its computer storage is {(Mounted ? $"mounted to {MountedHost!.Name.ColourName()}".ColourValue() : "not mounted".ColourError())}, containing {_executables.Count.ToString("N0", voyeur).ColourValue()} stored executables and {_fileSystem.Files.Count().ToString("N0", voyeur).ColourValue()} files.");
		return sb.ToString();
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ComputerStorageGameItemComponentProto)newProto;
		_fileSystem.CapacityInBytes = _prototype.StorageCapacityInBytes;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("NextExecutableId", _nextExecutableId),
			new XElement("NextProcessId", _nextProcessId),
			ComputerMutableOwnerXmlPersistence.SaveFiles(_fileSystem.MutableFiles),
			ComputerMutableOwnerXmlPersistence.SaveExecutables(_executables.Values),
			ComputerMutableOwnerXmlPersistence.SaveProcesses(_processes.Values),
			new XElement("ConnectedItems",
				_connectedHost is null
					? null
					: new XElement("Connection", new XAttribute("id", _connectedHost.Parent.Id)))).ToString();
	}

	public override void FinaliseLoad()
	{
		foreach (var id in _pendingConnectionIds.ToList())
		{
			var item = Gameworld.TryGetItem(id, true);
			var connectable = item?.GetItemTypes<IConnectable>().FirstOrDefault(x => CanConnect(null, x));
			if (connectable is null)
			{
				continue;
			}

			Connect(null, connectable);
		}

		_pendingConnectionIds.Clear();
	}

	public override void Quit()
	{
		Gameworld.ComputerExecutionService.DeactivateOwner(this);
		base.Quit();
	}

	public override void Delete()
	{
		Gameworld.ComputerExecutionService.DeactivateOwner(this);
		if (_connectedHost is not null)
		{
			RawDisconnect(_connectedHost, true);
		}

		base.Delete();
	}

	public IComputerExecutableDefinition CreateExecutableDefinition(ComputerExecutableKind kind, string name)
	{
		var now = DateTime.UtcNow;
		var id = NextExecutableId();
		ComputerRuntimeExecutableBase executable = kind == ComputerExecutableKind.Function
			? new ComputerMutableFunction(id, Gameworld)
			: new ComputerMutableProgram(id, Gameworld);
		executable.Name = string.IsNullOrWhiteSpace(name) ? "Unnamed" : name.Trim();
		executable.ReturnType = ProgVariableTypes.Void;
		executable.SourceCode = string.Empty;
		executable.Parameters = Array.Empty<ComputerExecutableParameter>();
		executable.CompilationStatus = ComputerCompilationStatus.NotCompiled;
		executable.OwnerStorageItemId = Parent.Id;
		executable.CreatedAtUtc = now;
		executable.LastModifiedAtUtc = now;
		_executables[id] = executable;
		Changed = true;
		return executable;
	}

	public void SaveExecutableDefinition(IComputerExecutableDefinition executable)
	{
		if (executable is not ComputerRuntimeExecutableBase runtime)
		{
			return;
		}

		runtime.OwnerStorageItemId = Parent.Id;
		runtime.OwnerHostItemId = null;
		_executables[runtime.Id] = runtime;
		Changed = true;
	}

	public bool DeleteExecutableDefinition(IComputerExecutableDefinition executable, out string error)
	{
		if (!_executables.Remove(executable.Id))
		{
			error = "There is no such storage executable to delete.";
			return false;
		}

		Changed = true;
		error = string.Empty;
		return true;
	}

	public ComputerRuntimeProcess CreateProcessDefinition(ICharacter? actor, IComputerProgramDefinition program)
	{
		var now = DateTime.UtcNow;
		var process = new ComputerRuntimeProcess
		{
			Id = NextProcessId(),
			ProcessName = program.Name,
			OwnerCharacterId = actor?.Id ?? 0L,
			Program = program,
			Host = ExecutionHost,
			Status = ComputerProcessStatus.Running,
			WaitType = ComputerProcessWaitType.None,
			PowerLossBehaviour = ComputerPowerLossBehaviour.PersistSuspended,
			StartedAtUtc = now,
			LastUpdatedAtUtc = now
		};
		_processes[process.Id] = process;
		Changed = true;
		return process;
	}

	public void SaveProcessDefinition(ComputerRuntimeProcess process)
	{
		process.Host = ExecutionHost;
		_processes[process.Id] = process;
		Changed = true;
	}

	public void DeleteProcessDefinition(IComputerProcess process)
	{
		if (_processes.Remove(process.Id))
		{
			Changed = true;
		}
	}

	public bool CanBeConnectedTo(IConnectable other)
	{
		return other is IComputerHost;
	}

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
		return _connectedHost is null &&
		       other is IComputerHost &&
		       other.FreeConnections.Any(x => x.CompatibleWith(ComputerConnectionTypes.StoragePlug));
	}

	public void Connect(ICharacter actor, IConnectable other)
	{
		if (!CanConnect(actor, other))
		{
			return;
		}

		_connectedHost = other;
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(ComputerConnectionTypes.StoragePlug)));
		Changed = true;
		Parent.ConnectedItem(other, ComputerConnectionTypes.StoragePlug);
		RebindProcessesToCurrentHost();
		if (MountedHost?.Powered == true)
		{
			Gameworld.ComputerExecutionService.ActivateOwner(this);
		}
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		_connectedHost = other;
		Changed = true;
		RebindProcessesToCurrentHost();
	}

	public string WhyCannotConnect(ICharacter actor, IConnectable other)
	{
		return _connectedHost is not null
			? $"{Parent.HowSeen(actor)} is already mounted to another computer host."
			: $"{Parent.HowSeen(actor)} cannot mount to {other.Parent.HowSeen(actor)}.";
	}

	public bool CanBeDisconnectedFrom(IConnectable other)
	{
		return true;
	}

	public bool CanDisconnect(ICharacter actor, IConnectable other)
	{
		return ReferenceEquals(_connectedHost, other);
	}

	public void Disconnect(ICharacter actor, IConnectable other)
	{
		RawDisconnect(other, true);
	}

	public void RawDisconnect(IConnectable other, bool handleEvents)
	{
		if (!ReferenceEquals(_connectedHost, other))
		{
			return;
		}

		Gameworld.ComputerExecutionService.DeactivateOwner(this);
		_connectedHost = null;
		RebindProcessesToCurrentHost();
		if (handleEvents)
		{
			other.RawDisconnect(this, false);
			Parent.DisconnectedItem(other, ComputerConnectionTypes.StoragePlug);
			other.Parent.DisconnectedItem(this, ComputerConnectionTypes.StoragePlug);
		}

		Changed = true;
	}

	public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
	{
		return $"{Parent.HowSeen(actor)} is not mounted to {other.Parent.HowSeen(actor)}.";
	}

	private void LoadRuntimeState(XElement root)
	{
		_fileSystem.LoadFiles(ComputerMutableOwnerXmlPersistence.LoadFiles(root.Element("Files")));
		foreach (var executable in ComputerMutableOwnerXmlPersistence.LoadExecutables(
			         root.Element("Executables"),
			         Gameworld,
			         null,
			         Parent.Id))
		{
			_executables[executable.Key] = executable.Value;
		}

		foreach (var process in ComputerMutableOwnerXmlPersistence.LoadProcesses(
			         root.Element("Processes"),
			         _executables,
			         ExecutionHost,
			         Gameworld))
		{
			_processes[process.Key] = process.Value;
		}

		_nextExecutableId = long.TryParse(root.Element("NextExecutableId")?.Value, out var nextExecutableId)
			? nextExecutableId
			: 0L;
		_nextProcessId = long.TryParse(root.Element("NextProcessId")?.Value, out var nextProcessId)
			? nextProcessId
			: 0L;
		_pendingConnectionIds.AddRange(root.Element("ConnectedItems")?.Elements("Connection")
			.Select(x => long.TryParse(x.Attribute("id")?.Value, out var id) ? id : 0L)
			.Where(x => x > 0) ?? Enumerable.Empty<long>());
	}

	private void RebindProcessesToCurrentHost()
	{
		foreach (var process in _processes.Values)
		{
			process.Host = ExecutionHost;
		}
	}

	private long NextExecutableId()
	{
		if (_nextExecutableId >= 0)
		{
			_nextExecutableId = -((Parent.Id * 10000L) + 2001L);
		}

		return _nextExecutableId--;
	}

	private long NextProcessId()
	{
		if (_nextProcessId >= 0)
		{
			_nextProcessId = -((Parent.Id * 10000L) + 7001L);
		}

		return _nextProcessId--;
	}

	private static void CopyExecutableState(ComputerRuntimeExecutableBase source, ComputerRuntimeExecutableBase target)
	{
		target.Name = source.Name;
		target.SourceCode = source.SourceCode;
		target.ReturnType = source.ReturnType;
		target.Parameters = source.Parameters.ToList();
		target.CompilationStatus = source.CompilationStatus;
		target.CompileError = source.CompileError;
		target.OwnerCharacterId = source.OwnerCharacterId;
		target.OwnerHostItemId = source.OwnerHostItemId;
		target.OwnerStorageItemId = source.OwnerStorageItemId;
		target.CreatedAtUtc = source.CreatedAtUtc;
		target.LastModifiedAtUtc = source.LastModifiedAtUtc;
	}

	private void FileSystemOnFileChanged(IComputerFileSystem fileSystem, ComputerFileSystemChange change)
	{
		Changed = true;
	}
}
