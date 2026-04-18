#nullable enable

using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
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

public class ComputerHostGameItemComponent : PoweredMachineBaseGameItemComponent, IComputerHost, IComputerMutableOwner, IComputerFtpAccountStore, IConnectable
{
	private readonly List<IConnectable> _connectedItems = [];
	private readonly HashSet<string> _enabledNetworkServices = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly Dictionary<string, ComputerMutableFtpAccount> _ftpAccounts = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly List<long> _pendingConnectionIds = [];
	private readonly Dictionary<long, ComputerRuntimeExecutableBase> _executables = new();
	private readonly Dictionary<long, ComputerRuntimeProcess> _processes = new();
	private ComputerHostGameItemComponentProto _prototype;
	private ComputerMutableFileSystem _fileSystem;
	private long _nextExecutableId;
	private long _nextProcessId;

	public ComputerHostGameItemComponent(ComputerHostGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
		_fileSystem = new ComputerMutableFileSystem(proto.StorageCapacityInBytes);
	}

	public ComputerHostGameItemComponent(MudSharp.Models.GameItemComponent component,
		ComputerHostGameItemComponentProto proto,
		IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_fileSystem = new ComputerMutableFileSystem(proto.StorageCapacityInBytes);
		LoadRuntimeState(XElement.Parse(component.Definition));
	}

	public ComputerHostGameItemComponent(ComputerHostGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_fileSystem = new ComputerMutableFileSystem(rhs._fileSystem.CapacityInBytes);
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
			var program = _executables.TryGetValue(process.Program.Id, out var executable)
				? executable as IComputerProgramDefinition
				: BuiltInApplications.FirstOrDefault(x => x.Id == process.Program.Id);
			if (program is null)
			{
				continue;
			}

			_processes[process.Id] = new ComputerRuntimeProcess
			{
				Id = process.Id,
				ProcessName = process.ProcessName,
				OwnerCharacterId = process.OwnerCharacterId,
				Program = program,
				Host = this,
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

		_enabledNetworkServices.UnionWith(rhs._enabledNetworkServices);
		foreach (var account in rhs._ftpAccounts.Values)
		{
			_ftpAccounts[account.UserName] = new ComputerMutableFtpAccount
			{
				UserName = account.UserName,
				PasswordHash = account.PasswordHash,
				PasswordSalt = account.PasswordSalt,
				Enabled = account.Enabled
			};
		}
		_nextExecutableId = rhs._nextExecutableId;
		_nextProcessId = rhs._nextProcessId;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public string Name => Parent.Name;
	public long? OwnerCharacterId => null;
	public long? OwnerHostItemId => Parent.Id;
	public long? OwnerStorageItemId => null;
	public IComputerHost ExecutionHost => this;
	public bool Powered => IsPowered;
	public IComputerFileSystem? FileSystem => _fileSystem;
	public IEnumerable<IComputerExecutableDefinition> Executables => _executables.Values.OrderBy(x => x.Name).ThenBy(x => x.Id);
	public IEnumerable<IComputerProcess> Processes => _processes.Values.OrderByDescending(x => x.LastUpdatedAtUtc).ThenByDescending(x => x.Id);
	public IEnumerable<IComputerBuiltInApplication> BuiltInApplications => ComputerBuiltInApplications.ForHost(Parent.Id);
	public IEnumerable<IComputerStorage> MountedStorage => _connectedItems.OfType<IComputerStorage>().ToList();
	public IEnumerable<IComputerTerminal> ConnectedTerminals => _connectedItems.OfType<IComputerTerminal>().ToList();
	public IEnumerable<INetworkAdapter> NetworkAdapters => _connectedItems.OfType<INetworkAdapter>().ToList();
	public IEnumerable<string> EnabledNetworkServices => _enabledNetworkServices.OrderBy(x => x).ToList();
	public IEnumerable<IComputerFtpAccount> FtpAccounts => _ftpAccounts.Values.OrderBy(x => x.UserName).ToList();
	public IEnumerable<ConnectorType> Connections => Enumerable.Range(0, _prototype.StoragePorts).Select(_ => ComputerConnectionTypes.HostStoragePort)
		.Concat(Enumerable.Range(0, _prototype.TerminalPorts).Select(_ => ComputerConnectionTypes.HostTerminalPort))
		.Concat(Enumerable.Range(0, _prototype.NetworkPorts).Select(_ => ComputerConnectionTypes.HostNetworkPort))
		.ToList();
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems
		.Select(x => Tuple.Create(GetConnectorTypeFor(x), x))
		.ToList();
	public IEnumerable<ConnectorType> FreeConnections
	{
		get
		{
			var connectors = Connections.ToList();
			foreach (var connection in ConnectedItems)
			{
				connectors.Remove(connection.Item1);
			}

			return connectors;
		}
	}

	public bool Independent => true;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ComputerHostGameItemComponent(this, newParent, temporary);
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
			$"Its computer host is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())}, {(IsPowered ? "powered".ColourValue() : "not powered".ColourError())}, with {_executables.Count.ToString("N0", voyeur).ColourValue()} stored executables and {_fileSystem.Files.Count().ToString("N0", voyeur).ColourValue()} files.");
		sb.AppendLine(
			$"It has {MountedStorage.Count().ToString("N0", voyeur).ColourValue()} storage {"device".Pluralise(MountedStorage.Count() != 1)}, {ConnectedTerminals.Count().ToString("N0", voyeur).ColourValue()} terminal {"connection".Pluralise(ConnectedTerminals.Count() != 1)}, and {NetworkAdapters.Count().ToString("N0", voyeur).ColourValue()} network {"adapter".Pluralise(NetworkAdapters.Count() != 1)} connected.");
		if (_enabledNetworkServices.Any())
		{
			sb.AppendLine(
				$"It is advertising the following network services: {_enabledNetworkServices.Select(x => x.ColourName()).ListToString()}.");
		}
		return sb.ToString();
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (ComputerHostGameItemComponentProto)newProto;
		_fileSystem.CapacityInBytes = _prototype.StorageCapacityInBytes;
	}

	protected override XElement SaveToXml(XElement root)
	{
		root.Add(new XElement("NextExecutableId", _nextExecutableId));
		root.Add(new XElement("NextProcessId", _nextProcessId));
		root.Add(new XElement("EnabledNetworkServices",
			from service in _enabledNetworkServices.OrderBy(x => x)
			select new XElement("Service", new XAttribute("id", service))));
		root.Add(ComputerMutableOwnerXmlPersistence.SaveFtpAccounts(_ftpAccounts.Values));
		root.Add(ComputerMutableOwnerXmlPersistence.SaveFiles(_fileSystem.MutableFiles));
		root.Add(ComputerMutableOwnerXmlPersistence.SaveExecutables(_executables.Values));
		root.Add(ComputerMutableOwnerXmlPersistence.SaveProcesses(_processes.Values));
		root.Add(new XElement("ConnectedItems",
			from item in _connectedItems
			select new XElement("Connection", new XAttribute("id", item.Parent.Id))));
		return root;
	}

	public override void FinaliseLoad()
	{
		base.FinaliseLoad();
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
		foreach (var storage in MountedStorage.Cast<IComputerExecutableOwner>().ToList())
		{
			Gameworld.ComputerExecutionService.DeactivateOwner(storage);
		}

		base.Quit();
	}

	public override void Delete()
	{
		foreach (var item in _connectedItems.ToList())
		{
			RawDisconnect(item, true);
		}

		Gameworld.ComputerExecutionService.DeactivateOwner(this);
		foreach (var storage in MountedStorage.Cast<IComputerExecutableOwner>().ToList())
		{
			Gameworld.ComputerExecutionService.DeactivateOwner(storage);
		}

		base.Delete();
	}

	protected override void OnPowerCutInAction()
	{
		Gameworld.ComputerExecutionService.ActivateOwner(this);
		foreach (var storage in MountedStorage.Cast<IComputerExecutableOwner>().ToList())
		{
			Gameworld.ComputerExecutionService.ActivateOwner(storage);
		}

		foreach (var program in _executables.Values
			         .OfType<ComputerRuntimeProgramBase>()
			         .Where(x => x.AutorunOnBoot)
			         .Where(x => !_processes.Values.Any(y =>
				         y.Program.Id == x.Id &&
				         y.Status is ComputerProcessStatus.Running or ComputerProcessStatus.Sleeping))
			         .ToList())
		{
			Gameworld.ComputerExecutionService.Execute(null, this, program, Array.Empty<object?>());
		}
	}

	protected override void OnPowerCutOutAction()
	{
		foreach (var storage in MountedStorage.Cast<IComputerExecutableOwner>().ToList())
		{
			Gameworld.ComputerExecutionService.DeactivateOwner(storage);
		}

		Gameworld.ComputerExecutionService.DeactivateOwner(this);
	}

	public IComputerProcess? GetProcess(long processId)
	{
		return _processes.TryGetValue(processId, out var process) ? process : null;
	}

	public bool IsNetworkServiceEnabled(string applicationId)
	{
		return _enabledNetworkServices.Contains(applicationId);
	}

	public bool SetNetworkServiceEnabled(string applicationId, bool enabled, out string error)
	{
		error = string.Empty;
		var application = BuiltInApplications.FirstOrDefault(x => x.ApplicationId.EqualTo(applicationId));
		if (application is null)
		{
			error = $"There is no built-in application named {applicationId.ColourCommand()} on {Parent.Name.ColourName()}.";
			return false;
		}

		if (!application.IsNetworkService)
		{
			error = $"{application.Name.ColourName()} is not a network service application.";
			return false;
		}

		if (enabled)
		{
			_enabledNetworkServices.Add(application.ApplicationId);
		}
		else
		{
			_enabledNetworkServices.Remove(application.ApplicationId);
		}

		Changed = true;
		return true;
	}

	public bool CreateFtpAccount(string userName, string passwordHash, long passwordSalt, out string error)
	{
		error = string.Empty;
		if (_ftpAccounts.ContainsKey(userName))
		{
			error = $"There is already an FTP account named {userName.ColourName()} on {Parent.Name.ColourName()}.";
			return false;
		}

		_ftpAccounts[userName] = new ComputerMutableFtpAccount
		{
			UserName = userName,
			PasswordHash = passwordHash,
			PasswordSalt = passwordSalt,
			Enabled = true
		};
		Changed = true;
		return true;
	}

	public bool SetFtpAccountEnabled(string userName, bool enabled, out string error)
	{
		if (!_ftpAccounts.TryGetValue(userName, out var account))
		{
			error = $"There is no FTP account named {userName.ColourName()} on {Parent.Name.ColourName()}.";
			return false;
		}

		account.Enabled = enabled;
		Changed = true;
		error = string.Empty;
		return true;
	}

	public bool SetFtpAccountPassword(string userName, string passwordHash, long passwordSalt, out string error)
	{
		if (!_ftpAccounts.TryGetValue(userName, out var account))
		{
			error = $"There is no FTP account named {userName.ColourName()} on {Parent.Name.ColourName()}.";
			return false;
		}

		account.PasswordHash = passwordHash;
		account.PasswordSalt = passwordSalt;
		Changed = true;
		error = string.Empty;
		return true;
	}

	public IComputerExecutableDefinition CreateExecutableDefinition(ComputerExecutableKind kind, string name)
	{
		var now = DateTime.UtcNow;
		var id = NextExecutableId();
		ComputerRuntimeExecutableBase executable = kind == ComputerExecutableKind.Function
			? new ComputerMutableFunction(id, Gameworld)
			: new ComputerMutableProgram(id, Gameworld);
		executable.Name = string.IsNullOrWhiteSpace(name) ? "Unnamed" : name.Trim();
		executable.SourceCode = string.Empty;
		executable.ReturnType = ProgVariableTypes.Void;
		executable.Parameters = Array.Empty<ComputerExecutableParameter>();
		executable.CompilationStatus = ComputerCompilationStatus.NotCompiled;
		executable.CompileError = string.Empty;
		executable.OwnerHostItemId = Parent.Id;
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

		runtime.OwnerHostItemId = Parent.Id;
		runtime.OwnerStorageItemId = null;
		_executables[runtime.Id] = runtime;
		Changed = true;
	}

	public bool DeleteExecutableDefinition(IComputerExecutableDefinition executable, out string error)
	{
		if (!_executables.Remove(executable.Id))
		{
			error = "There is no such host executable to delete.";
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
			Host = this,
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
		process.Host = this;
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
		return other is IComputerStorage or IComputerTerminal or INetworkAdapter;
	}

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
		if (other is not IComputerStorage && other is not IComputerTerminal && other is not INetworkAdapter)
		{
			return false;
		}

		return FreeConnections.Any(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
	}

	public void Connect(ICharacter actor, IConnectable other)
	{
		if (!CanConnect(actor, other))
		{
			return;
		}

		var connection = FreeConnections.First(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
		_connectedItems.Add(other);
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(connection)));
		Parent.ConnectedItem(other, connection);
		Changed = true;

		if (other is IComputerStorage storage && IsPowered)
		{
			Gameworld.ComputerExecutionService.ActivateOwner(storage);
		}
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		if (_connectedItems.Contains(other))
		{
			return;
		}

		_connectedItems.Add(other);
		Changed = true;
	}

	public string WhyCannotConnect(ICharacter actor, IConnectable other)
	{
		return $"{Parent.HowSeen(actor)} has no compatible free computer connection ports.";
	}

	public bool CanBeDisconnectedFrom(IConnectable other)
	{
		return true;
	}

	public bool CanDisconnect(ICharacter actor, IConnectable other)
	{
		return _connectedItems.Contains(other);
	}

	public void Disconnect(ICharacter actor, IConnectable other)
	{
		RawDisconnect(other, true);
	}

	public void RawDisconnect(IConnectable other, bool handleEvents)
	{
		if (!_connectedItems.Remove(other))
		{
			return;
		}

		if (other is IComputerStorage storage)
		{
			Gameworld.ComputerExecutionService.DeactivateOwner(storage);
		}

		if (handleEvents)
		{
			other.RawDisconnect(this, false);
			Parent.DisconnectedItem(other, GetConnectorTypeFor(other));
			other.Parent.DisconnectedItem(this, GetConnectorTypeFor(other));
		}

		Changed = true;
	}

	public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
	{
		return $"{other.Parent.HowSeen(actor)} is not currently connected to {Parent.HowSeen(actor)}.";
	}

	private void LoadRuntimeState(XElement root)
	{
		_fileSystem.CapacityInBytes = _prototype.StorageCapacityInBytes;
		_fileSystem.LoadFiles(ComputerMutableOwnerXmlPersistence.LoadFiles(root.Element("Files")));
		foreach (var executable in ComputerMutableOwnerXmlPersistence.LoadExecutables(
			         root.Element("Executables"),
			         Gameworld,
			         Parent.Id,
			         null))
		{
			_executables[executable.Key] = executable.Value;
		}

		foreach (var process in ComputerMutableOwnerXmlPersistence.LoadProcesses(
			         root.Element("Processes"),
			         _executables,
			         this,
			         Gameworld,
			         BuiltInApplications))
		{
			_processes[process.Key] = process.Value;
		}

		_nextExecutableId = long.TryParse(root.Element("NextExecutableId")?.Value, out var nextExecutableId)
			? nextExecutableId
			: 0L;
		_nextProcessId = long.TryParse(root.Element("NextProcessId")?.Value, out var nextProcessId)
			? nextProcessId
			: 0L;
		foreach (var service in root.Element("EnabledNetworkServices")?.Elements("Service")
			         .Select(x => x.Attribute("id")?.Value)
			         .Where(x => !string.IsNullOrWhiteSpace(x))
			         .Cast<string>() ?? Enumerable.Empty<string>())
		{
			_enabledNetworkServices.Add(service);
		}

		foreach (var account in ComputerMutableOwnerXmlPersistence.LoadFtpAccounts(root.Element("FtpAccounts")))
		{
			_ftpAccounts[account.UserName] = account;
		}

		_pendingConnectionIds.AddRange(root.Element("ConnectedItems")?.Elements("Connection")
			.Select(x => long.TryParse(x.Attribute("id")?.Value, out var id) ? id : 0L)
			.Where(x => x > 0) ?? Enumerable.Empty<long>());
	}

	private long NextExecutableId()
	{
		if (_nextExecutableId >= 0)
		{
			_nextExecutableId = -((Parent.Id * 10000L) + 1L);
		}

		return _nextExecutableId--;
	}

	private long NextProcessId()
	{
		if (_nextProcessId >= 0)
		{
			_nextProcessId = -((Parent.Id * 10000L) + 5001L);
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

	private static ConnectorType GetConnectorTypeFor(IConnectable other)
	{
		return other switch
		{
			IComputerStorage => ComputerConnectionTypes.HostStoragePort,
			IComputerTerminal => ComputerConnectionTypes.HostTerminalPort,
			INetworkAdapter => ComputerConnectionTypes.HostNetworkPort,
			_ => ComputerConnectionTypes.HostTerminalPort
		};
	}
}
