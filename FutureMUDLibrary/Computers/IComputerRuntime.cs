#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction.Grids;
using MudSharp.FutureProg;

namespace MudSharp.Computers;

public sealed class ComputerFileSystemChange
{
	public string FileName { get; init; } = string.Empty;
	public ComputerFileSystemChangeType ChangeType { get; init; }
}

public enum ComputerFileSystemChangeType
{
	Written = 0,
	Appended = 1,
	Deleted = 2,
	PublicAccessChanged = 3
}

public delegate void ComputerFileSystemChanged(IComputerFileSystem fileSystem, ComputerFileSystemChange change);

public interface IComputerFileOwner
{
	string Name { get; }
	long FileOwnerId { get; }
	IComputerFileSystem? FileSystem { get; }
}

public interface IComputerExecutableOwner : IComputerFileOwner
{
	long? OwnerCharacterId { get; }
	long? OwnerHostItemId { get; }
	long? OwnerStorageItemId { get; }
	IComputerHost ExecutionHost { get; }
	IEnumerable<IComputerExecutableDefinition> Executables { get; }
	IEnumerable<IComputerProcess> Processes { get; }
}

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
	long? WaitingCharacterId { get; }
	long? WaitingTerminalItemId { get; }
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
	bool PubliclyAccessible { get; }
}

public interface IComputerFileSystem
{
	long CapacityInBytes { get; }
	long UsedBytes { get; }
	IEnumerable<IComputerFile> Files { get; }
	event ComputerFileSystemChanged? FileChanged;
	bool FileExists(string fileName);
	IComputerFile? GetFile(string fileName);
	string ReadFile(string fileName);
	void WriteFile(string fileName, string textContents);
	void AppendFile(string fileName, string textContents);
	bool DeleteFile(string fileName);
	bool SetFilePubliclyAccessible(string fileName, bool isPublic);
}

public interface IComputerBuiltInApplication : IComputerProgramDefinition
{
	string ApplicationId { get; }
	string Summary { get; }
	bool IsNetworkService { get; }
}

public interface IComputerHost : IComputerExecutableOwner
{
	bool Powered { get; }
	IEnumerable<IComputerBuiltInApplication> BuiltInApplications { get; }
	IEnumerable<IComputerStorage> MountedStorage { get; }
	IEnumerable<IComputerTerminal> ConnectedTerminals { get; }
	IEnumerable<INetworkAdapter> NetworkAdapters { get; }
	IEnumerable<string> EnabledNetworkServices { get; }
	IEnumerable<string> HostedVpnNetworkIds { get; }
	IComputerProcess? GetProcess(long processId);
	bool IsNetworkServiceEnabled(string applicationId);
	bool SetNetworkServiceEnabled(string applicationId, bool enabled, out string error);
	bool AddHostedVpnNetwork(string networkId, out string error);
	bool RemoveHostedVpnNetwork(string networkId, out string error);
}

public interface IComputerStorage : IComputerExecutableOwner
{
	long CapacityInBytes { get; }
	bool Mounted { get; }
	IComputerHost? MountedHost { get; }
}

public interface IComputerTerminalSession
{
	ICharacter User { get; }
	IComputerTerminal Terminal { get; }
	IComputerHost Host { get; }
	IComputerExecutableOwner CurrentOwner { get; }
	DateTime ConnectedAtUtc { get; }
	IReadOnlyCollection<ComputerNetworkTunnelInfo> ActiveTunnels { get; }
	IReadOnlyCollection<string> ActiveRouteKeys { get; }
}

public interface IComputerTerminal
{
	long TerminalItemId { get; }
	IComputerHost? ConnectedHost { get; }
	IEnumerable<IComputerTerminalSession> Sessions { get; }
}

public sealed class ComputerNetworkHostSummary
{
	public required IComputerHost Host { get; init; }
	public required INetworkAdapter Adapter { get; init; }
	public required ITelecommunicationsGrid Grid { get; init; }
	public string CanonicalAddress { get; init; } = string.Empty;
	public string DeviceIdentifier { get; init; } = string.Empty;
	public bool IsLocalGrid { get; init; }
	public bool Available { get; init; }
	public int AdvertisedServiceCount { get; init; }
	public IReadOnlyCollection<string> SharedRouteKeys { get; init; } = Array.Empty<string>();
	public string AccessDescription { get; init; } = string.Empty;
}

public sealed class ComputerNetworkServiceSummary
{
	public string ApplicationId { get; init; } = string.Empty;
	public string Name { get; init; } = string.Empty;
	public string Summary { get; init; } = string.Empty;
	public IReadOnlyCollection<string> ServiceDetails { get; init; } = Array.Empty<string>();
}

public interface INetworkAdapter
{
	IComputerHost? ConnectedHost { get; }
	bool Powered { get; }
	bool NetworkReady { get; }
	bool PublicNetworkEnabled { get; }
	string? ExchangeSubnetId { get; }
	IEnumerable<string> VpnNetworkIds { get; }
	IEnumerable<string> NetworkRouteKeys { get; }
	string DeviceIdentifier { get; }
	string? PreferredNetworkAddress { get; }
	string? NetworkAddress { get; }
	long NetworkAdapterItemId { get; }
	ITelecommunicationsGrid? TelecommunicationsGrid { get; }
}

public interface INetworkInfrastructure
{
	ITelecommunicationsGrid? TelecommunicationsGrid { get; }
	bool NetworkTransportReady { get; }
}

public interface ICharacterComputerWorkspace : IComputerExecutableOwner
{
	ICharacter Owner { get; }
}

public interface IComputerExecutionService
{
	ICharacterComputerWorkspace GetWorkspace(ICharacter owner);
	IEnumerable<IComputerExecutableDefinition> GetExecutables(IComputerExecutableOwner owner);
	IComputerExecutableDefinition? GetExecutable(IComputerExecutableOwner owner, string identifier);
	IEnumerable<IComputerBuiltInApplication> GetBuiltInApplications(IComputerExecutableOwner owner);
	IComputerBuiltInApplication? GetBuiltInApplication(IComputerExecutableOwner owner, string identifier);
	IComputerExecutableDefinition CreateExecutable(IComputerExecutableOwner owner, ComputerExecutableKind kind,
		string name);
	void SaveExecutable(IComputerExecutableOwner owner, IComputerExecutableDefinition executable);
	bool DeleteExecutable(IComputerExecutableOwner owner, IComputerExecutableDefinition executable, out string error);
	ComputerExecutionResult Execute(ICharacter? actor, IComputerExecutableOwner owner,
		IComputerExecutableDefinition executable, IEnumerable<object?> parameters, IComputerTerminalSession? session = null);
	ComputerExecutionResult ExecuteBuiltInApplication(ICharacter? actor, IComputerExecutableOwner owner,
		IComputerBuiltInApplication application, IComputerTerminalSession? session = null);
	IEnumerable<IComputerProcess> GetProcesses(IComputerExecutableOwner owner);
	IComputerProcess? GetProcess(IComputerExecutableOwner owner, long processId);
	bool KillProcess(IComputerExecutableOwner owner, long processId, out string error);
	void ActivateOwner(IComputerExecutableOwner owner);
	void DeactivateOwner(IComputerExecutableOwner owner);
	bool TrySubmitTerminalInput(IComputerTerminalSession session, string text, out string error);
	IEnumerable<ComputerNetworkHostSummary> GetReachableHosts(IComputerHost sourceHost, IComputerTerminalSession? session = null);
	ComputerNetworkHostSummary? ResolveReachableHost(IComputerHost sourceHost, string identifier,
		IComputerTerminalSession? session = null);
	IEnumerable<ComputerNetworkServiceSummary> GetAdvertisedServices(IComputerHost sourceHost, IComputerHost targetHost,
		IComputerTerminalSession? session = null);
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
