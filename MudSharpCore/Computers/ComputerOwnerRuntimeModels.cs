#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Computers;

public abstract class ComputerRuntimeExecutableBase : IComputerExecutableDefinition
{
	protected ComputerRuntimeExecutableBase(long id, IFuturemud gameworld)
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

public abstract class ComputerRuntimeFunctionBase : ComputerRuntimeExecutableBase, IComputerFunction
{
	protected ComputerRuntimeFunctionBase(long id, IFuturemud gameworld)
		: base(id, gameworld)
	{
	}

	public override ComputerExecutableKind ExecutableKind => ComputerExecutableKind.Function;
}

public abstract class ComputerRuntimeProgramBase : ComputerRuntimeExecutableBase, IComputerProgramDefinition
{
	protected ComputerRuntimeProgramBase(long id, IFuturemud gameworld)
		: base(id, gameworld)
	{
	}

	public override ComputerExecutableKind ExecutableKind => ComputerExecutableKind.Program;
	public bool AutorunOnBoot { get; set; }
}

public sealed class ComputerMutableFunction : ComputerRuntimeFunctionBase
{
	public ComputerMutableFunction(long id, IFuturemud gameworld)
		: base(id, gameworld)
	{
	}
}

public sealed class ComputerMutableProgram : ComputerRuntimeProgramBase
{
	public ComputerMutableProgram(long id, IFuturemud gameworld)
		: base(id, gameworld)
	{
	}
}

public sealed class ComputerRuntimeProcess : IComputerProcess, IFrameworkItem
{
	public long Id { get; set; }
	public string Name => ProcessName;
	public string FrameworkItemType => "ComputerProcess";
	public string ProcessName { get; set; } = string.Empty;
	public long OwnerCharacterId { get; set; }
	public required IComputerProgramDefinition Program { get; init; }
	public required IComputerHost Host { get; set; }
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

public sealed class ComputerMutableTextFile : IComputerFile
{
	public string FileName { get; set; } = string.Empty;
	public string TextContents { get; set; } = string.Empty;
	public long SizeInBytes => Encoding.UTF8.GetByteCount(TextContents ?? string.Empty);
	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
	public DateTime LastModifiedAtUtc { get; set; } = DateTime.UtcNow;
	public bool PubliclyAccessible { get; set; }
}

public sealed class ComputerMutableFtpAccount : IComputerFtpAccount
{
	public string UserName { get; set; } = string.Empty;
	public string PasswordHash { get; set; } = string.Empty;
	public long PasswordSalt { get; set; }
	public bool Enabled { get; set; }
}

public sealed class ComputerMutableFileSystem : IComputerFileSystem
{
	private readonly List<ComputerMutableTextFile> _files = [];

	public ComputerMutableFileSystem(long capacityInBytes)
	{
		CapacityInBytes = capacityInBytes;
	}

	public long CapacityInBytes { get; set; }
	public long UsedBytes => _files.Sum(x => x.SizeInBytes);
	public IEnumerable<IComputerFile> Files => _files;
	public event ComputerFileSystemChanged? FileChanged;

	public IReadOnlyCollection<ComputerMutableTextFile> MutableFiles => _files.AsReadOnly();

	public bool FileExists(string fileName)
	{
		return _files.Any(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
	}

	public IComputerFile? GetFile(string fileName)
	{
		return _files.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
	}

	public string ReadFile(string fileName)
	{
		return _files.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
			?.TextContents ?? string.Empty;
	}

	public void WriteFile(string fileName, string textContents)
	{
		var now = DateTime.UtcNow;
		var existing = _files.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
		if (existing is null)
		{
			_files.Add(new ComputerMutableTextFile
			{
				FileName = fileName,
				TextContents = textContents ?? string.Empty,
				CreatedAtUtc = now,
				LastModifiedAtUtc = now
			});
			FileChanged?.Invoke(this, new ComputerFileSystemChange
			{
				FileName = fileName,
				ChangeType = ComputerFileSystemChangeType.Written
			});
			return;
		}

		existing.TextContents = textContents ?? string.Empty;
		existing.LastModifiedAtUtc = now;
		FileChanged?.Invoke(this, new ComputerFileSystemChange
		{
			FileName = fileName,
			ChangeType = ComputerFileSystemChangeType.Written
		});
	}

	public void AppendFile(string fileName, string textContents)
	{
		var now = DateTime.UtcNow;
		var existing = _files.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
		if (existing is null)
		{
			_files.Add(new ComputerMutableTextFile
			{
				FileName = fileName,
				TextContents = textContents ?? string.Empty,
				CreatedAtUtc = now,
				LastModifiedAtUtc = now
			});
			FileChanged?.Invoke(this, new ComputerFileSystemChange
			{
				FileName = fileName,
				ChangeType = ComputerFileSystemChangeType.Appended
			});
			return;
		}

		existing.TextContents += textContents ?? string.Empty;
		existing.LastModifiedAtUtc = now;
		FileChanged?.Invoke(this, new ComputerFileSystemChange
		{
			FileName = fileName,
			ChangeType = ComputerFileSystemChangeType.Appended
		});
	}

	public bool DeleteFile(string fileName)
	{
		var existing = _files.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
		if (existing is null)
		{
			return false;
		}

		_files.Remove(existing);
		FileChanged?.Invoke(this, new ComputerFileSystemChange
		{
			FileName = fileName,
			ChangeType = ComputerFileSystemChangeType.Deleted
		});
		return true;
	}

	public bool SetFilePubliclyAccessible(string fileName, bool isPublic)
	{
		var existing = _files.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
		if (existing is null)
		{
			return false;
		}

		existing.PubliclyAccessible = isPublic;
		existing.LastModifiedAtUtc = DateTime.UtcNow;
		FileChanged?.Invoke(this, new ComputerFileSystemChange
		{
			FileName = fileName,
			ChangeType = ComputerFileSystemChangeType.PublicAccessChanged
		});
		return true;
	}

	public void LoadFiles(IEnumerable<ComputerMutableTextFile> files)
	{
		_files.Clear();
		_files.AddRange(files);
	}
}

public sealed class ComputerTerminalSession : IComputerTerminalSession
{
	private readonly List<ComputerNetworkTunnelInfo> _activeTunnels = [];

	public required ICharacter User { get; init; }
	public required IComputerTerminal Terminal { get; init; }
	public required IComputerHost Host { get; init; }
	public required IComputerExecutableOwner CurrentOwner { get; set; }
	public DateTime ConnectedAtUtc { get; init; } = DateTime.UtcNow;
	public IReadOnlyCollection<ComputerNetworkTunnelInfo> ActiveTunnels => _activeTunnels.AsReadOnly();
	public IReadOnlyCollection<string> ActiveRouteKeys => _activeTunnels
		.Select(x => x.RouteKey)
		.Distinct(StringComparer.InvariantCultureIgnoreCase)
		.OrderBy(x => x)
		.ToList();

	public void AddOrReplaceTunnel(ComputerNetworkTunnelInfo tunnel)
	{
		_activeTunnels.RemoveAll(x => x.RouteKey.Equals(tunnel.RouteKey, StringComparison.InvariantCultureIgnoreCase));
		_activeTunnels.Add(tunnel);
	}

	public bool RemoveTunnel(string routeKey)
	{
		return _activeTunnels.RemoveAll(x => x.RouteKey.Equals(routeKey, StringComparison.InvariantCultureIgnoreCase)) > 0;
	}

	public void ClearTunnels()
	{
		_activeTunnels.Clear();
	}
}
