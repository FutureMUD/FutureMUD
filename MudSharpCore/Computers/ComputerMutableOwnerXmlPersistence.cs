#nullable enable

using System.Globalization;

namespace MudSharp.Computers;

internal static class ComputerMutableOwnerXmlPersistence
{
	public static IReadOnlyDictionary<long, ComputerRuntimeExecutableBase> LoadExecutables(
		XElement? element,
		IFuturemud gameworld,
		long? ownerHostItemId,
		long? ownerStorageItemId)
	{
		Dictionary<long, ComputerRuntimeExecutableBase> executables = new();
		if (element is null)
		{
			return executables;
		}

		foreach (var child in element.Elements("Executable"))
		{
			if (!long.TryParse(child.Attribute("id")?.Value, out var id))
			{
				continue;
			}

			var kind = child.Attribute("kind")?.Value.EqualTo("function") == true
				? ComputerExecutableKind.Function
				: ComputerExecutableKind.Program;
			ComputerRuntimeExecutableBase executable = kind == ComputerExecutableKind.Function
				? new ComputerMutableFunction(id, gameworld)
				: new ComputerMutableProgram(id, gameworld)
				{
					AutorunOnBoot = bool.TryParse(child.Attribute("autorun")?.Value, out var autorun) && autorun
				};
			executable.Name = child.Attribute("name")?.Value ?? $"Executable{id}";
			executable.SourceCode = child.Element("Source")?.Value ?? string.Empty;
			executable.ReturnType = ProgVariableTypes.FromStorageString(
				child.Attribute("return")?.Value ?? ProgVariableTypes.Void.ToStorageString());
			executable.Parameters = child.Element("Parameters")?.Elements("Parameter")
				.Select(x => new ComputerExecutableParameter(
					x.Attribute("name")?.Value ?? string.Empty,
					ProgVariableTypes.FromStorageString(
						x.Attribute("type")?.Value ?? ProgVariableTypes.Text.ToStorageString())))
				.ToList() ?? [];
			executable.CompilationStatus =
				Enum.TryParse<ComputerCompilationStatus>(child.Attribute("status")?.Value, true, out var status)
					? status
					: ComputerCompilationStatus.NotCompiled;
			executable.CompileError = child.Element("CompileError")?.Value ?? string.Empty;
			executable.OwnerHostItemId = ownerHostItemId;
			executable.OwnerStorageItemId = ownerStorageItemId;
			executable.CreatedAtUtc = TryParseDateTime(child.Attribute("created")?.Value) ?? DateTime.UtcNow;
			executable.LastModifiedAtUtc = TryParseDateTime(child.Attribute("modified")?.Value) ?? DateTime.UtcNow;
			executables[id] = executable;
		}

		return executables;
	}

	public static XElement SaveExecutables(IEnumerable<ComputerRuntimeExecutableBase> executables)
	{
		return new XElement("Executables",
			from executable in executables.OrderBy(x => x.Id)
			select new XElement("Executable",
				new XAttribute("id", executable.Id),
				new XAttribute("kind", executable.ExecutableKind == ComputerExecutableKind.Function ? "function" : "program"),
				new XAttribute("name", executable.Name),
				new XAttribute("return", executable.ReturnType.ToStorageString()),
				new XAttribute("status", executable.CompilationStatus),
				new XAttribute("created", executable.CreatedAtUtc.ToString("O")),
				new XAttribute("modified", executable.LastModifiedAtUtc.ToString("O")),
				new XAttribute("autorun",
					executable is IComputerProgramDefinition { AutorunOnBoot: true }),
				new XElement("CompileError", new XCData(executable.CompileError ?? string.Empty)),
				new XElement("Parameters",
					from parameter in executable.Parameters
					select new XElement("Parameter",
						new XAttribute("name", parameter.Name),
						new XAttribute("type", parameter.Type.ToStorageString()))),
				new XElement("Source", new XCData(executable.SourceCode ?? string.Empty))));
	}

	public static IReadOnlyDictionary<long, ComputerRuntimeProcess> LoadProcesses(
		XElement? element,
		IReadOnlyDictionary<long, ComputerRuntimeExecutableBase> executables,
		IComputerHost host,
		IFuturemud gameworld,
		IEnumerable<IComputerBuiltInApplication>? additionalPrograms = null)
	{
		Dictionary<long, ComputerRuntimeProcess> processes = new();
		if (element is null)
		{
			return processes;
		}

		foreach (var child in element.Elements("Process"))
		{
			if (!long.TryParse(child.Attribute("id")?.Value, out var processId))
			{
				continue;
			}

			if (!long.TryParse(child.Attribute("executable")?.Value, out var executableId))
			{
				continue;
			}

			var program = executables.TryGetValue(executableId, out var executable)
				? executable as IComputerProgramDefinition
				: additionalPrograms?.FirstOrDefault(x => x.Id == executableId);
			if (program is null)
			{
				continue;
			}

			var process = new ComputerRuntimeProcess
			{
				Id = processId,
				ProcessName = child.Attribute("name")?.Value ?? program.Name,
				OwnerCharacterId = long.TryParse(child.Attribute("owner")?.Value, out var ownerId) ? ownerId : 0L,
				Program = program,
				Host = host,
				Status = Enum.TryParse<ComputerProcessStatus>(child.Attribute("status")?.Value, true, out var status)
					? status
					: ComputerProcessStatus.NotStarted,
				WaitType = Enum.TryParse<ComputerProcessWaitType>(child.Attribute("waittype")?.Value, true, out var waitType)
					? waitType
					: ComputerProcessWaitType.None,
				WakeTimeUtc = TryParseDateTime(child.Attribute("wake")?.Value),
				WaitArgument = child.Attribute("waitarg")?.Value,
				PowerLossBehaviour =
					Enum.TryParse<ComputerPowerLossBehaviour>(child.Attribute("powerloss")?.Value, true, out var powerLoss)
						? powerLoss
						: ComputerPowerLossBehaviour.Terminate,
				Result = ComputerProgramExecutor.DeserializeValue(program.ReturnType, child.Element("Result")?.Value, gameworld),
				LastError = child.Element("LastError")?.Value,
				StartedAtUtc = TryParseDateTime(child.Attribute("started")?.Value) ?? DateTime.UtcNow,
				LastUpdatedAtUtc = TryParseDateTime(child.Attribute("updated")?.Value) ?? DateTime.UtcNow,
				EndedAtUtc = TryParseDateTime(child.Attribute("ended")?.Value)
			};
			if (ComputerProcessWaitArguments.TryParseUserInput(process.WaitArgument, out var waitingCharacterId,
				    out var waitingTerminalItemId))
			{
				process.WaitingCharacterId = waitingCharacterId;
				process.WaitingTerminalItemId = waitingTerminalItemId;
			}
			process.StateJson = child.Element("State")?.Value ?? string.Empty;
			processes[processId] = process;
		}

		return processes;
	}

	public static XElement SaveProcesses(IEnumerable<ComputerRuntimeProcess> processes)
	{
		return new XElement("Processes",
			from process in processes.OrderBy(x => x.Id)
			select new XElement("Process",
				new XAttribute("id", process.Id),
				new XAttribute("executable", process.Program.Id),
				new XAttribute("name", process.ProcessName),
				new XAttribute("owner", process.OwnerCharacterId),
				new XAttribute("status", process.Status),
				new XAttribute("waittype", process.WaitType),
				new XAttribute("wake", process.WakeTimeUtc?.ToString("O") ?? string.Empty),
				new XAttribute("waitarg", process.WaitArgument ?? string.Empty),
				new XAttribute("powerloss", process.PowerLossBehaviour),
				new XAttribute("started", process.StartedAtUtc.ToString("O")),
				new XAttribute("updated", process.LastUpdatedAtUtc.ToString("O")),
				new XAttribute("ended", process.EndedAtUtc?.ToString("O") ?? string.Empty),
				new XElement("Result",
					new XCData(ComputerProgramExecutor.SerializeValue(process.Program.ReturnType, process.Result) ?? string.Empty)),
				new XElement("LastError", new XCData(process.LastError ?? string.Empty)),
				new XElement("State", new XCData(process.StateJson ?? string.Empty))));
	}

	public static IEnumerable<ComputerMutableTextFile> LoadFiles(XElement? element)
	{
		if (element is null)
		{
			return Enumerable.Empty<ComputerMutableTextFile>();
		}

		return element.Elements("File")
			.Where(x => !string.Equals(x.Attribute("kind")?.Value, "media", StringComparison.InvariantCultureIgnoreCase))
			.Select(x => new ComputerMutableTextFile
			{
				FileName = x.Attribute("name")?.Value ?? string.Empty,
				TextContents = x.Value ?? string.Empty,
				CreatedAtUtc = TryParseDateTime(x.Attribute("created")?.Value) ?? DateTime.UtcNow,
				LastModifiedAtUtc = TryParseDateTime(x.Attribute("modified")?.Value) ?? DateTime.UtcNow,
				PubliclyAccessible = bool.TryParse(x.Attribute("public")?.Value, out var isPublic) && isPublic
			})
			.ToList();
	}

	public static IEnumerable<ComputerMutableMediaFile> LoadMediaFiles(XElement? element)
	{
		if (element is null)
		{
			return Enumerable.Empty<ComputerMutableMediaFile>();
		}

		return element.Elements("File")
			.Where(x => x.Attribute("kind")?.Value.EqualTo("media") == true)
			.Select(x => new ComputerMutableMediaFile
			{
				FileName = x.Attribute("name")?.Value ?? string.Empty,
				MediaRecordingId = long.TryParse(x.Attribute("recording")?.Value, out var recordingId) && recordingId > 0L
					? recordingId
					: null,
				SizeInBytes = long.TryParse(x.Attribute("size")?.Value, out var size) && size >= 0L ? size : 0L,
				CreatedAtUtc = TryParseDateTime(x.Attribute("created")?.Value) ?? DateTime.UtcNow,
				LastModifiedAtUtc = TryParseDateTime(x.Attribute("modified")?.Value) ?? DateTime.UtcNow,
				PubliclyAccessible = bool.TryParse(x.Attribute("public")?.Value, out var isPublic) && isPublic
			})
			.Where(x => x.MediaRecordingId.HasValue)
			.ToList();
	}

	public static XElement SaveFiles(IEnumerable<ComputerMutableTextFile> files)
	{
		return new XElement("Files",
			from file in files.OrderBy(x => x.FileName)
			select new XElement("File",
				new XAttribute("name", file.FileName),
				new XAttribute("created", file.CreatedAtUtc.ToString("O")),
				new XAttribute("modified", file.LastModifiedAtUtc.ToString("O")),
				new XAttribute("public", file.PubliclyAccessible),
				new XCData(file.TextContents ?? string.Empty)));
	}

	public static XElement SaveFiles(IEnumerable<ComputerMutableTextFile> textFiles,
		IEnumerable<ComputerMutableMediaFile> mediaFiles)
	{
		return new XElement("Files",
			textFiles
				.Select(file => new XElement("File",
					new XAttribute("name", file.FileName),
					new XAttribute("created", file.CreatedAtUtc.ToString("O")),
					new XAttribute("modified", file.LastModifiedAtUtc.ToString("O")),
					new XAttribute("public", file.PubliclyAccessible),
					new XCData(file.TextContents ?? string.Empty)))
				.Concat(mediaFiles.Select(file => new XElement("File",
					new XAttribute("name", file.FileName),
					new XAttribute("kind", "media"),
					new XAttribute("recording", file.MediaRecordingId ?? 0L),
					new XAttribute("size", file.SizeInBytes),
					new XAttribute("created", file.CreatedAtUtc.ToString("O")),
					new XAttribute("modified", file.LastModifiedAtUtc.ToString("O")),
					new XAttribute("public", file.PubliclyAccessible))))
				.OrderBy(x => x.Attribute("name")?.Value));
	}

	public static IEnumerable<ComputerMutableFtpAccount> LoadFtpAccounts(XElement? element)
	{
		if (element is null)
		{
			return Enumerable.Empty<ComputerMutableFtpAccount>();
		}

		return element.Elements("Account")
			.Select(x => new ComputerMutableFtpAccount
			{
				UserName = x.Attribute("name")?.Value ?? string.Empty,
				PasswordHash = x.Attribute("hash")?.Value ?? string.Empty,
				PasswordSalt = long.TryParse(x.Attribute("salt")?.Value, out var salt) ? salt : 0L,
				Enabled = !bool.TryParse(x.Attribute("enabled")?.Value, out var enabled) || enabled
			})
			.Where(x => !string.IsNullOrWhiteSpace(x.UserName))
			.ToList();
	}

	public static XElement SaveFtpAccounts(IEnumerable<ComputerMutableFtpAccount> accounts)
	{
		return new XElement("FtpAccounts",
			from account in accounts.OrderBy(x => x.UserName)
			select new XElement("Account",
				new XAttribute("name", account.UserName),
				new XAttribute("hash", account.PasswordHash),
				new XAttribute("salt", account.PasswordSalt),
				new XAttribute("enabled", account.Enabled)));
	}

	public static (IReadOnlyCollection<MediaFeedConfiguration> Feeds,
		IReadOnlyCollection<MediaSubscriptionConfiguration> Subscriptions) LoadMediaConfiguration(XElement? element)
	{
		if (element is null)
		{
			return (Array.Empty<MediaFeedConfiguration>(), Array.Empty<MediaSubscriptionConfiguration>());
		}

		var feeds = element.Element("Feeds")?.Elements("Feed")
			.Select(x => new MediaFeedConfiguration(
				x.Attribute("name")?.Value?.Trim() ?? string.Empty,
				x.Attribute("input")?.Value?.Trim() ?? string.Empty,
				!bool.TryParse(x.Attribute("public")?.Value, out var isPublic) || isPublic,
				x.Elements("Account")
					.Select(account => long.TryParse(account.Attribute("id")?.Value, out var id) ? id : 0L)
					.Where(id => id > 0L)
					.Distinct()
					.OrderBy(id => id)
					.ToList()))
			.Where(x => !string.IsNullOrWhiteSpace(x.FeedName) && !string.IsNullOrWhiteSpace(x.InputName))
			.GroupBy(x => x.FeedName, StringComparer.InvariantCultureIgnoreCase)
			.Select(x => x.Last())
			.ToList() ?? [];

		var subscriptions = element.Element("Subscriptions")?.Elements("Subscription")
			.Select(x => new MediaSubscriptionConfiguration(
				x.Attribute("name")?.Value?.Trim() ?? string.Empty,
				long.TryParse(x.Attribute("source")?.Value, out var sourceHostItemId) ? sourceHostItemId : 0L,
				x.Attribute("address")?.Value?.Trim() ?? string.Empty,
				x.Attribute("feed")?.Value?.Trim() ?? string.Empty,
				x.Attribute("output")?.Value?.Trim() ?? string.Empty,
				long.TryParse(x.Attribute("account")?.Value, out var accountId) && accountId > 0L ? accountId : null,
				!bool.TryParse(x.Attribute("enabled")?.Value, out var enabled) || enabled))
			.Where(x => !string.IsNullOrWhiteSpace(x.SubscriptionName) && x.SourceHostItemId > 0L &&
			            !string.IsNullOrWhiteSpace(x.FeedName) && !string.IsNullOrWhiteSpace(x.OutputName))
			.GroupBy(x => x.SubscriptionName, StringComparer.InvariantCultureIgnoreCase)
			.Select(x => x.Last())
			.ToList() ?? [];

		return (feeds, subscriptions);
	}

	public static XElement SaveMediaConfiguration(IEnumerable<MediaFeedConfiguration> feeds,
		IEnumerable<MediaSubscriptionConfiguration> subscriptions)
	{
		return new XElement("MediaConfiguration",
			new XElement("Feeds",
				from feed in feeds.OrderBy(x => x.FeedName)
				select new XElement("Feed",
					new XAttribute("name", feed.FeedName),
					new XAttribute("input", feed.InputName),
					new XAttribute("public", feed.IsPublic),
					from accountId in feed.AllowedAccountIds.Where(x => x > 0L).Distinct().OrderBy(x => x)
					select new XElement("Account", new XAttribute("id", accountId)))),
			new XElement("Subscriptions",
				from subscription in subscriptions.OrderBy(x => x.SubscriptionName)
				select new XElement("Subscription",
					new XAttribute("name", subscription.SubscriptionName),
					new XAttribute("source", subscription.SourceHostItemId),
					new XAttribute("address", subscription.SourceAddress),
					new XAttribute("feed", subscription.FeedName),
					new XAttribute("output", subscription.OutputName),
					new XAttribute("account", subscription.AccountId ?? 0L),
					new XAttribute("enabled", subscription.Enabled))));
	}

	private static DateTime? TryParseDateTime(string? value)
	{
		return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result)
			? result
			: null;
	}
}
