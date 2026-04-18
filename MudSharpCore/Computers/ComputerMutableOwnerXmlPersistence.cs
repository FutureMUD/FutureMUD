#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg;

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
					new XCData(ComputerProgramExecutor.SerializeValue(process.Program.ReturnType, process.Result))),
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

	private static DateTime? TryParseDateTime(string? value)
	{
		return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result)
			? result
			: null;
	}
}
