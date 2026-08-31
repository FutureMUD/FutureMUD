#nullable enable


namespace MudSharp.Computers;

public sealed class ComputerFileSystemCapacityException : InvalidOperationException
{
	public ComputerFileSystemCapacityException(string message) : base(message)
	{
	}
}

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
	public ComputerFileKind Kind => ComputerFileKind.Text;
	public string TextContents { get; set; } = string.Empty;
	public long? MediaRecordingId => null;
	public long SizeInBytes => Encoding.UTF8.GetByteCount(TextContents ?? string.Empty);
	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
	public DateTime LastModifiedAtUtc { get; set; } = DateTime.UtcNow;
	public bool PubliclyAccessible { get; set; }
}

public sealed class ComputerMutableMediaFile : IComputerFile
{
	public string FileName { get; set; } = string.Empty;
	public ComputerFileKind Kind => ComputerFileKind.Media;
	public string TextContents => string.Empty;
	public long? MediaRecordingId { get; set; }
	public long SizeInBytes { get; set; }
	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
	public DateTime LastModifiedAtUtc { get; set; } = DateTime.UtcNow;
	public bool PubliclyAccessible { get; set; }
}

public sealed class ComputerFileKindException : InvalidOperationException
{
	public ComputerFileKindException(string message) : base(message)
	{
	}
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
	private readonly List<ComputerMutableMediaFile> _mediaFiles = [];

	public ComputerMutableFileSystem(long capacityInBytes)
	{
		CapacityInBytes = capacityInBytes;
	}

	public long CapacityInBytes { get; set; }
	public long UsedBytes => _files.Sum(x => x.SizeInBytes) + _mediaFiles.Sum(x => x.SizeInBytes);
	public IEnumerable<IComputerFile> Files => _files.Cast<IComputerFile>().Concat(_mediaFiles);
	public event ComputerFileSystemChanged? FileChanged;

	public IReadOnlyCollection<ComputerMutableTextFile> MutableFiles => _files.AsReadOnly();
	public IReadOnlyCollection<ComputerMutableMediaFile> MutableMediaFiles => _mediaFiles.AsReadOnly();
	public IReadOnlyCollection<IComputerFile> AllFiles => Files.ToList().AsReadOnly();

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
		var file = GetFile(fileName);
		if (file?.Kind == ComputerFileKind.Media)
		{
			throw new ComputerFileKindException($"{fileName} is a media file and cannot be read as text.");
		}

		return file?.TextContents ?? string.Empty;
	}

	private void EnsureCapacityForWrite(ComputerMutableTextFile? existing, string replacementContents)
	{
		var replacementSize = Encoding.UTF8.GetByteCount(replacementContents ?? string.Empty);
		var resultingSize = UsedBytes - (existing?.SizeInBytes ?? 0L) + replacementSize;
		if (resultingSize <= CapacityInBytes)
		{
			return;
		}

		throw new ComputerFileSystemCapacityException(
			$"That write would use {resultingSize:N0} bytes, exceeding the file system capacity of {CapacityInBytes:N0} bytes.");
	}

	private void EnsureCapacityForAppend(string appendedContents)
	{
		var resultingSize = UsedBytes + Encoding.UTF8.GetByteCount(appendedContents ?? string.Empty);
		if (resultingSize <= CapacityInBytes)
		{
			return;
		}

		throw new ComputerFileSystemCapacityException(
			$"That append would use {resultingSize:N0} bytes, exceeding the file system capacity of {CapacityInBytes:N0} bytes.");
	}

	public void WriteFile(string fileName, string textContents)
	{
		var now = DateTime.UtcNow;
		var contents = textContents ?? string.Empty;
		var existingMedia = _mediaFiles.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
		if (existingMedia is not null)
		{
			throw new ComputerFileKindException($"{fileName} is a media file and cannot be overwritten with text.");
		}

		var existing = _files.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
		EnsureCapacityForWrite(existing, contents);
		if (existing is null)
		{
			_files.Add(new ComputerMutableTextFile
			{
				FileName = fileName,
				TextContents = contents,
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

		existing.TextContents = contents;
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
		var contents = textContents ?? string.Empty;
		if (_mediaFiles.Any(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)))
		{
			throw new ComputerFileKindException($"{fileName} is a media file and cannot be appended as text.");
		}

		var existing = _files.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
		EnsureCapacityForAppend(contents);
		if (existing is null)
		{
			_files.Add(new ComputerMutableTextFile
			{
				FileName = fileName,
				TextContents = contents,
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

		existing.TextContents += contents;
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
		if (existing is not null)
		{
			_files.Remove(existing);
			FileChanged?.Invoke(this, new ComputerFileSystemChange
			{
				FileName = fileName,
				ChangeType = ComputerFileSystemChangeType.Deleted
			});
			return true;
		}

		var media = _mediaFiles.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
		if (media is null)
		{
			return false;
		}

		_mediaFiles.Remove(media);
		FileChanged?.Invoke(this, new ComputerFileSystemChange
		{
			FileName = fileName,
			ChangeType = ComputerFileSystemChangeType.Deleted,
			Kind = ComputerFileKind.Media,
			MediaRecordingId = media.MediaRecordingId
		});
		return true;
	}

	public bool SetFilePubliclyAccessible(string fileName, bool isPublic)
	{
		var existing = _files.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
		if (existing is not null)
		{
			existing.PubliclyAccessible = isPublic;
			existing.LastModifiedAtUtc = DateTime.UtcNow;
			FileChanged?.Invoke(this, new ComputerFileSystemChange
			{
				FileName = fileName,
				ChangeType = ComputerFileSystemChangeType.PublicAccessChanged
			});
			return true;
		}

		var media = _mediaFiles.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
		if (media is null)
		{
			return false;
		}

		media.PubliclyAccessible = isPublic;
		media.LastModifiedAtUtc = DateTime.UtcNow;
		FileChanged?.Invoke(this, new ComputerFileSystemChange
		{
			FileName = fileName,
			ChangeType = ComputerFileSystemChangeType.PublicAccessChanged,
			Kind = ComputerFileKind.Media,
			MediaRecordingId = media.MediaRecordingId
		});
		return true;
	}

	public void LoadFiles(IEnumerable<ComputerMutableTextFile> files)
	{
		_files.Clear();
		_files.AddRange(files);
	}

	public void LoadMediaFiles(IEnumerable<ComputerMutableMediaFile> files)
	{
		_mediaFiles.Clear();
		_mediaFiles.AddRange(files);
	}

	public bool WriteMediaFile(string fileName, long recordingId, long sizeInBytes, bool publiclyAccessible,
		out string error)
	{
		error = string.Empty;
		if (recordingId <= 0L)
		{
			error = "A media file must reference a valid recording.";
			return false;
		}

		if (sizeInBytes < 0L)
		{
			error = "A media file cannot have a negative size.";
			return false;
		}

		if (FileExists(fileName))
		{
			error = $"A file named {fileName} already exists.";
			return false;
		}

		if (UsedBytes + sizeInBytes > CapacityInBytes)
		{
			error = $"That media file would exceed the file system capacity of {CapacityInBytes:N0} bytes.";
			return false;
		}

		var now = DateTime.UtcNow;
		_mediaFiles.Add(new ComputerMutableMediaFile
		{
			FileName = fileName,
			MediaRecordingId = recordingId,
			SizeInBytes = sizeInBytes,
			PubliclyAccessible = publiclyAccessible,
			CreatedAtUtc = now,
			LastModifiedAtUtc = now
		});
		FileChanged?.Invoke(this, new ComputerFileSystemChange
		{
			FileName = fileName,
			ChangeType = ComputerFileSystemChangeType.Written,
			Kind = ComputerFileKind.Media,
			MediaRecordingId = recordingId
		});
		return true;
	}

	public bool UpdateMediaFileSize(string fileName, long sizeInBytes, out string error)
	{
		error = string.Empty;
		if (sizeInBytes < 0L)
		{
			error = "A media file cannot have a negative size.";
			return false;
		}

		var media = _mediaFiles.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
		if (media is null)
		{
			error = "There is no media file with that name.";
			return false;
		}

		if (UsedBytes - media.SizeInBytes + sizeInBytes > CapacityInBytes)
		{
			error = $"That media file would exceed the file system capacity of {CapacityInBytes:N0} bytes.";
			return false;
		}

		media.SizeInBytes = sizeInBytes;
		media.LastModifiedAtUtc = DateTime.UtcNow;
		FileChanged?.Invoke(this, new ComputerFileSystemChange
		{
			FileName = fileName,
			ChangeType = ComputerFileSystemChangeType.Written,
			Kind = ComputerFileKind.Media,
			MediaRecordingId = media.MediaRecordingId
		});
		return true;
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
