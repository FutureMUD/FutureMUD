#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using MudSharp.Character;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Computers;

internal interface IComputerBuiltInApplicationExecutor
{
	string ApplicationId { get; }
	ComputerProgramExecutionOutcome Execute(IFuturemud gameworld, ICharacter? actor, IComputerExecutableOwner owner,
		IComputerTerminalSession? session, ComputerRuntimeProcess process, IComputerBuiltInApplication application);
}

internal static class ComputerBuiltInApplicationExecutors
{
	private static readonly IReadOnlyDictionary<string, IComputerBuiltInApplicationExecutor> _executors =
		new IComputerBuiltInApplicationExecutor[]
		{
			new FileManagerBuiltInApplicationExecutor(),
			new DirectoryBuiltInApplicationExecutor(),
			new SysMonBuiltInApplicationExecutor()
		}.ToDictionary(x => x.ApplicationId, StringComparer.InvariantCultureIgnoreCase);

	public static ComputerProgramExecutionOutcome Execute(IFuturemud gameworld, ICharacter? actor,
		IComputerExecutableOwner owner,
		IComputerTerminalSession? session, ComputerRuntimeProcess process, IComputerBuiltInApplication application)
	{
		if (!_executors.TryGetValue(application.ApplicationId, out var executor))
		{
			return new ComputerProgramExecutionOutcome
			{
				Status = ComputerProcessStatus.Failed,
				Error = $"{application.Name} is not implemented yet."
			};
		}

		return executor.Execute(gameworld, actor, owner, session, process, application);
	}
}

internal sealed class FileManagerBuiltInApplicationExecutor : IComputerBuiltInApplicationExecutor
{
	private sealed class FileManagerState
	{
		public long? TargetStorageItemId { get; set; }
	}

	public string ApplicationId => "filemanager";

	public ComputerProgramExecutionOutcome Execute(IFuturemud gameworld, ICharacter? actor, IComputerExecutableOwner owner,
		IComputerTerminalSession? session, ComputerRuntimeProcess process, IComputerBuiltInApplication application)
	{
		if (session is null)
		{
			return new ComputerProgramExecutionOutcome
			{
				Status = ComputerProcessStatus.Failed,
				Error = $"{application.Name} requires an active computer terminal session."
			};
		}

		var state = LoadState(process.StateJson, owner);
		var input = ComputerExecutionContextScope.Current?.ConsumePendingTerminalInput();
		if (string.IsNullOrWhiteSpace(input))
		{
			SendOverview(session, application, process.Host, state, owner);
			return WaitForInput(session, state);
		}

		var response = HandleCommand(session, application, process.Host, state, input!);
		if (!string.IsNullOrWhiteSpace(response.Output))
		{
			session.User.OutputHandler.Send(response.Output, nopage: true);
		}

		return response.Exit
			? new ComputerProgramExecutionOutcome
			{
				Status = ComputerProcessStatus.Completed
			}
			: WaitForInput(session, state);
	}

	private static (string Output, bool Exit) HandleCommand(IComputerTerminalSession session,
		IComputerBuiltInApplication application,
		IComputerHost host,
		FileManagerState state,
		string input)
	{
		var ss = new StringStack(input.Trim());
		var command = ss.PopSpeech().ToLowerInvariant();
		return command switch
		{
			"" => (RenderPrompt(session.User, host, state, null), false),
			"help" => (RenderHelp(session.User, application, host, state), false),
			"list" or "ls" or "dir" => (RenderFileList(session.User, host, state, null), false),
			"owners" or "targets" => (RenderOwners(session.User, host, state), false),
			"show" or "read" or "cat" => HandleShow(session, host, state, ss),
			"edit" => HandleEdit(session, host, state, ss),
			"write" => HandleWrite(session, host, state, ss, append: false),
			"append" => HandleWrite(session, host, state, ss, append: true),
			"delete" or "del" or "rm" => HandleDelete(session, host, state, ss),
			"copy" or "cp" => HandleCopy(session, host, state, ss),
			"use" or "owner" => HandleUse(session, host, state, ss),
			"exit" or "quit" => ($"{application.Name.ColourName()} closing.", true),
			_ => ($"That is not a valid {application.Name.ColourName()} command.\n\n{RenderPrompt(session.User, host, state, null)}", false)
		};
	}

	private static (string Output, bool Exit) HandleShow(IComputerTerminalSession session, IComputerHost host,
		FileManagerState state, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ("Which file do you want to show?\n\n" + RenderPrompt(session.User, host, state, null), false);
		}

		var fileName = ss.PopSpeech();
		var (owner, warning) = ResolveTargetOwner(host, state);
		var fileSystem = owner.FileSystem;
		var file = fileSystem?.GetFile(fileName);
		if (file is null)
		{
			return ($"{DescribeOwner(owner).ColourName()} does not have a file named {fileName.ColourName()}.\n\n{RenderPrompt(session.User, host, state, warning)}", false);
		}

		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(warning))
		{
			sb.AppendLine(warning);
			sb.AppendLine();
		}

		sb.AppendLine($"{file.FileName.ColourName()} on {DescribeOwner(owner).ColourName()}");
		sb.AppendLine($"Size: {file.SizeInBytes.ToString("N0", session.User).ColourValue()} bytes");
		sb.AppendLine($"Modified: {file.LastModifiedAtUtc.ToString(session.User).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine(file.TextContents);
		sb.AppendLine();
		sb.Append(RenderPrompt(session.User, host, state, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleWrite(IComputerTerminalSession session, IComputerHost host,
		FileManagerState state, StringStack ss, bool append)
	{
		if (ss.IsFinished)
		{
			return ($"You must specify a file to {(append ? "append to" : "write")}.\n\n{RenderPrompt(session.User, host, state, null)}", false);
		}

		var fileName = ss.PopSpeech();
		if (ss.IsFinished)
		{
			return ($"You must specify the text to {(append ? "append" : "write")}.\n\n{RenderPrompt(session.User, host, state, null)}", false);
		}

		var text = ss.SafeRemainingArgument;
		var (owner, warning) = ResolveTargetOwner(host, state);
		var fileSystem = owner.FileSystem;
		if (fileSystem is null)
		{
			return ($"{DescribeOwner(owner).ColourName()} does not expose a writable file system.\n\n{RenderPrompt(session.User, host, state, warning)}", false);
		}

		if (append)
		{
			fileSystem.AppendFile(fileName, text);
		}
		else
		{
			fileSystem.WriteFile(fileName, text);
		}

		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(warning))
		{
			sb.AppendLine(warning);
			sb.AppendLine();
		}

		sb.AppendLine($"{(append ? "Appended".ColourValue() : "Wrote".ColourValue())} {fileName.ColourName()} on {DescribeOwner(owner).ColourName()}.");
		sb.Append(RenderPrompt(session.User, host, state, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleEdit(IComputerTerminalSession session, IComputerHost host,
		FileManagerState state, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ("Which file do you want to edit?\n\n" + RenderPrompt(session.User, host, state, null), false);
		}

		var fileName = ss.PopSpeech();
		var (owner, warning) = ResolveTargetOwner(host, state);
		var fileSystem = owner.FileSystem;
		if (fileSystem is null)
		{
			return ($"{DescribeOwner(owner).ColourName()} does not expose a writable file system.\n\n{RenderPrompt(session.User, host, state, warning)}", false);
		}

		if (!string.IsNullOrEmpty(warning))
		{
			session.User.OutputHandler.Send(warning, nopage: true);
		}

		var existingText = fileSystem.ReadFile(fileName);
		var ownerDescription = DescribeOwner(owner);
		session.User.EditorMode(
			(text, handler, _) =>
			{
				fileSystem.WriteFile(fileName, text);
				handler.Send($"Saved {fileName.ColourName()} on {ownerDescription.ColourName()}. Continue with {"type <command>".ColourCommand()} to keep using FileManager.");
			},
			(handler, _) =>
			{
				handler.Send($"You leave {fileName.ColourName()} unchanged on {ownerDescription.ColourName()}.");
			},
			1.0,
			existingText,
			EditorOptions.PermitEmpty);
		return (string.Empty, false);
	}

	private static (string Output, bool Exit) HandleDelete(IComputerTerminalSession session, IComputerHost host,
		FileManagerState state, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ("Which file do you want to delete?\n\n" + RenderPrompt(session.User, host, state, null), false);
		}

		var fileName = ss.PopSpeech();
		var (owner, warning) = ResolveTargetOwner(host, state);
		var fileSystem = owner.FileSystem;
		if (fileSystem is null)
		{
			return ($"{DescribeOwner(owner).ColourName()} does not expose a writable file system.\n\n{RenderPrompt(session.User, host, state, warning)}", false);
		}

		if (!fileSystem.DeleteFile(fileName))
		{
			return ($"{DescribeOwner(owner).ColourName()} does not have a file named {fileName.ColourName()}.\n\n{RenderPrompt(session.User, host, state, warning)}", false);
		}

		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(warning))
		{
			sb.AppendLine(warning);
			sb.AppendLine();
		}

		sb.AppendLine($"Deleted {fileName.ColourName()} from {DescribeOwner(owner).ColourName()}.");
		sb.Append(RenderPrompt(session.User, host, state, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleCopy(IComputerTerminalSession session, IComputerHost host,
		FileManagerState state, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ("Which file do you want to copy?\n\n" + RenderPrompt(session.User, host, state, null), false);
		}

		var fileName = ss.PopSpeech();
		if (!ss.IsFinished && ss.PeekSpeech().EqualTo("to"))
		{
			ss.PopSpeech();
		}

		if (ss.IsFinished)
		{
			return ("Where do you want to copy that file?\n\n" + RenderPrompt(session.User, host, state, null), false);
		}

		var targetText = ss.SafeRemainingArgument;
		var (sourceOwner, warning) = ResolveTargetOwner(host, state);
		var sourceFile = sourceOwner.FileSystem?.GetFile(fileName);
		if (sourceFile is null)
		{
			return ($"{DescribeOwner(sourceOwner).ColourName()} does not have a file named {fileName.ColourName()}.\n\n{RenderPrompt(session.User, host, state, warning)}", false);
		}

		var targetOwner = ResolveSelectableOwner(host, targetText);
		if (targetOwner is null)
		{
			return ($"You must target either {host.Name.ColourName()} or one of its mounted storage devices.\n\n{RenderPrompt(session.User, host, state, warning)}", false);
		}

		if (ReferenceEquals(targetOwner, sourceOwner))
		{
			return ($"That file is already on {DescribeOwner(sourceOwner).ColourName()}.\n\n{RenderPrompt(session.User, host, state, warning)}", false);
		}

		var targetFileSystem = targetOwner.FileSystem;
		if (targetFileSystem is null)
		{
			return ($"{DescribeOwner(targetOwner).ColourName()} does not expose a writable file system.\n\n{RenderPrompt(session.User, host, state, warning)}", false);
		}

		targetFileSystem.WriteFile(sourceFile.FileName, sourceFile.TextContents);
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(warning))
		{
			sb.AppendLine(warning);
			sb.AppendLine();
		}

		sb.AppendLine($"Copied {sourceFile.FileName.ColourName()} from {DescribeOwner(sourceOwner).ColourName()} to {DescribeOwner(targetOwner).ColourName()}.");
		sb.Append(RenderPrompt(session.User, host, state, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleUse(IComputerTerminalSession session, IComputerHost host,
		FileManagerState state, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ("Which host or mounted storage device do you want to use?\n\n" + RenderPrompt(session.User, host, state, null), false);
		}

		var targetText = ss.SafeRemainingArgument;
		var targetOwner = ResolveSelectableOwner(host, targetText);
		if (targetOwner is null)
		{
			return ($"You must target either {host.Name.ColourName()} or one of its mounted storage devices.\n\n{RenderPrompt(session.User, host, state, null)}", false);
		}

		state.TargetStorageItemId = targetOwner is IComputerStorage storage ? storage.OwnerStorageItemId : null;
		return ($"Now managing files on {DescribeOwner(targetOwner).ColourName()}.\n\n{RenderPrompt(session.User, host, state, null)}", false);
	}

	private static void SendOverview(IComputerTerminalSession session, IComputerBuiltInApplication application,
		IComputerHost host, FileManagerState state, IComputerExecutableOwner initialOwner)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{application.Name.ColourName()} :: {host.Name.ColourName()}");
		sb.AppendLine($"Current Target: {DescribeOwner(ResolveTargetOwner(host, state).Owner).ColourName()}");
		sb.AppendLine($"Launched For: {DescribeOwner(initialOwner).ColourName()}");
		sb.AppendLine();
		sb.AppendLine(RenderHelp(session.User, application, host, state));
		session.User.OutputHandler.Send(sb.ToString(), nopage: true);
	}

	private static string RenderHelp(ICharacter user, IComputerBuiltInApplication application, IComputerHost host,
		FileManagerState state)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{application.Name.ColourName()} commands:");
		sb.AppendLine($"\t{"list".ColourCommand()} - list files on the current target");
		sb.AppendLine($"\t{"show <file>".ColourCommand()} - display a file");
		sb.AppendLine($"\t{"edit <file>".ColourCommand()} - open a file in the normal multiline editor");
		sb.AppendLine($"\t{"write <file> <text>".ColourCommand()} - overwrite a file");
		sb.AppendLine($"\t{"append <file> <text>".ColourCommand()} - append text to a file");
		sb.AppendLine($"\t{"delete <file>".ColourCommand()} - delete a file");
		sb.AppendLine($"\t{"copy <file> <target>".ColourCommand()} - copy a file to the host or a mounted storage device");
		sb.AppendLine($"\t{"owners".ColourCommand()} - list available file owners");
		sb.AppendLine($"\t{"use <target>".ColourCommand()} - switch the current target");
		sb.AppendLine($"\t{"help".ColourCommand()} - show this help");
		sb.AppendLine($"\t{"exit".ColourCommand()} - close FileManager");
		sb.AppendLine();
		sb.Append(RenderFileList(user, host, state, null));
		return sb.ToString();
	}

	private static string RenderFileList(ICharacter user, IComputerHost host, FileManagerState state, string? warning)
	{
		var (owner, resolvedWarning) = ResolveTargetOwner(host, state);
		var fileSystem = owner.FileSystem;
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(warning))
		{
			sb.AppendLine(warning);
			sb.AppendLine();
		}

		if (!string.IsNullOrEmpty(resolvedWarning))
		{
			sb.AppendLine(resolvedWarning);
			sb.AppendLine();
		}

		sb.AppendLine($"Current Target: {DescribeOwner(owner).ColourName()}");
		if (fileSystem is null)
		{
			sb.AppendLine("This target does not expose a file system.");
			sb.AppendLine();
			sb.Append(RenderPrompt(user, host, state, null));
			return sb.ToString();
		}

		sb.AppendLine(
			$"Storage Usage: {fileSystem.UsedBytes.ToString("N0", user).ColourValue()} / {fileSystem.CapacityInBytes.ToString("N0", user).ColourValue()} bytes");
		var files = fileSystem.Files
			.OrderBy(x => x.FileName)
			.ThenBy(x => x.CreatedAtUtc)
			.ToList();
		if (!files.Any())
		{
			sb.AppendLine("Files: None");
			sb.AppendLine();
			sb.Append(RenderPrompt(user, host, state, null));
			return sb.ToString();
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			files.Select(file => new List<string>
			{
				file.FileName,
				file.SizeInBytes.ToString("N0", user),
				file.LastModifiedAtUtc.ToString(user)
			}),
			new List<string>
			{
				"File",
				"Size",
				"Modified"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
		sb.AppendLine();
		sb.Append(RenderPrompt(user, host, state, null));
		return sb.ToString();
	}

	private static string RenderOwners(ICharacter user, IComputerHost host, FileManagerState state)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Available File Targets:");
		var owners = new List<IComputerExecutableOwner> { host };
		owners.AddRange(host.MountedStorage);
		sb.AppendLine(StringUtilities.GetTextTable(
			owners.Select(owner => new List<string>
			{
				DescribeOwner(owner),
				owner is IComputerStorage storage ? storage.OwnerStorageItemId?.ToString("N0", user) ?? "-" : "host",
				owner.FileSystem?.UsedBytes.ToString("N0", user) ?? "0",
				owner.FileSystem?.CapacityInBytes.ToString("N0", user) ?? "0"
			}),
			new List<string>
			{
				"Owner",
				"Selector",
				"Used",
				"Capacity"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
		sb.AppendLine();
		sb.Append(RenderPrompt(user, host, state, null));
		return sb.ToString();
	}

	private static string RenderPrompt(ICharacter user, IComputerHost host, FileManagerState state, string? warning)
	{
		var (owner, resolvedWarning) = ResolveTargetOwner(host, state);
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(warning))
		{
			sb.AppendLine(warning);
		}

		if (!string.IsNullOrEmpty(resolvedWarning))
		{
			sb.AppendLine(resolvedWarning);
		}

		sb.Append($"Use {"type <command>".ColourCommand()} for {DescribeOwner(owner).ColourName()}.");
		return sb.ToString();
	}

	private static ComputerProgramExecutionOutcome WaitForInput(IComputerTerminalSession session,
		FileManagerState state)
	{
		return new ComputerProgramExecutionOutcome
		{
			Status = ComputerProcessStatus.Sleeping,
			WaitType = ComputerProcessWaitType.UserInput,
			WaitArgument = ComputerProcessWaitArguments.CreateUserInput(session.User.Id, session.Terminal.TerminalItemId),
			WaitingCharacterId = session.User.Id,
			WaitingTerminalItemId = session.Terminal.TerminalItemId,
			StateJson = JsonSerializer.Serialize(state)
		};
	}

	private static FileManagerState LoadState(string? stateJson, IComputerExecutableOwner owner)
	{
		if (!string.IsNullOrWhiteSpace(stateJson))
		{
			try
			{
				var state = JsonSerializer.Deserialize<FileManagerState>(stateJson);
				if (state is not null)
				{
					return state;
				}
			}
			catch (JsonException)
			{
			}
		}

		return new FileManagerState
		{
			TargetStorageItemId = owner is IComputerStorage storage ? storage.OwnerStorageItemId : null
		};
	}

	private static (IComputerExecutableOwner Owner, string? Warning) ResolveTargetOwner(IComputerHost host,
		FileManagerState state)
	{
		if (state.TargetStorageItemId is not > 0L)
		{
			return (host, null);
		}

		var storage = host.MountedStorage
			.FirstOrDefault(x => x.OwnerStorageItemId == state.TargetStorageItemId.Value);
		if (storage is not null)
		{
			return (storage, null);
		}

		state.TargetStorageItemId = null;
		return (host, "The previously selected storage device is no longer mounted, so FileManager has reverted to the host.");
	}

	private static IComputerExecutableOwner? ResolveSelectableOwner(IComputerHost host, string identifier)
	{
		if (identifier.EqualTo("host") || host.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
		{
			return host;
		}

		var storages = host.MountedStorage.ToList();
		if (long.TryParse(identifier, out var id))
		{
			return storages.FirstOrDefault(x => x.OwnerStorageItemId == id);
		}

		var exact = storages.FirstOrDefault(x => x.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase));
		if (exact is not null)
		{
			return exact;
		}

		return storages.FirstOrDefault(x => x.Name.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase));
	}

	private static string DescribeOwner(IComputerExecutableOwner owner)
	{
		return owner switch
		{
			IComputerStorage storage => storage.Name,
			IComputerHost host => host.Name,
			_ => owner.Name
		};
	}
}

internal sealed class DirectoryBuiltInApplicationExecutor : IComputerBuiltInApplicationExecutor
{
	public string ApplicationId => "directory";

	public ComputerProgramExecutionOutcome Execute(IFuturemud gameworld, ICharacter? actor, IComputerExecutableOwner owner,
		IComputerTerminalSession? session, ComputerRuntimeProcess process, IComputerBuiltInApplication application)
	{
		if (session is null)
		{
			return new ComputerProgramExecutionOutcome
			{
				Status = ComputerProcessStatus.Failed,
				Error = $"{application.Name} requires an active computer terminal session."
			};
		}

		var input = ComputerExecutionContextScope.Current?.ConsumePendingTerminalInput();
		if (string.IsNullOrWhiteSpace(input))
		{
			SendOverview(session, application, process.Host);
			return WaitForInput(session);
		}

		var response = HandleCommand(session, application, process.Host, input!);
		if (!string.IsNullOrWhiteSpace(response.Output))
		{
			session.User.OutputHandler.Send(response.Output, nopage: true);
		}

		return response.Exit
			? new ComputerProgramExecutionOutcome
			{
				Status = ComputerProcessStatus.Completed
			}
			: WaitForInput(session);
	}

	private static (string Output, bool Exit) HandleCommand(IComputerTerminalSession session,
		IComputerBuiltInApplication application,
		IComputerHost host,
		string input)
	{
		var ss = new StringStack(input.Trim());
		var command = ss.PopSpeech().ToLowerInvariant();
		return command switch
		{
			"" or "summary" or "host" => (RenderSummary(session.User, application, host), false),
			"help" => (RenderHelp(application), false),
			"services" or "apps" => (RenderServices(session.User, host), false),
			"storage" or "drives" => (RenderStorage(session.User, host), false),
			"terminals" or "sessions" => (RenderTerminals(session.User, host), false),
			"adapters" or "network" => (RenderAdapters(session.User, host), false),
			"exit" or "quit" => ($"{application.Name.ColourName()} closing.", true),
			_ => ($"That is not a valid {application.Name.ColourName()} command.\n\n{RenderPrompt()}", false)
		};
	}

	private static void SendOverview(IComputerTerminalSession session, IComputerBuiltInApplication application,
		IComputerHost host)
	{
		var sb = new StringBuilder();
		sb.AppendLine(RenderSummary(session.User, application, host));
		sb.AppendLine();
		sb.Append(RenderHelp(application));
		session.User.OutputHandler.Send(sb.ToString(), nopage: true);
	}

	private static string RenderSummary(ICharacter user, IComputerBuiltInApplication application, IComputerHost host)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{application.Name.ColourName()} :: {host.Name.ColourName()}");
		sb.AppendLine(
			$"Host Power: {(host.Powered ? "powered".ColourValue() : "not powered".ColourError())}");
		if (host.FileSystem is not null)
		{
			sb.AppendLine(
				$"Host Storage: {host.FileSystem.UsedBytes.ToString("N0", user).ColourValue()} / {host.FileSystem.CapacityInBytes.ToString("N0", user).ColourValue()} bytes used");
		}

		sb.AppendLine(
			$"Local Services: {host.BuiltInApplications.Count().ToString("N0", user).ColourValue()}");
		sb.AppendLine(
			$"Mounted Storage: {host.MountedStorage.Count().ToString("N0", user).ColourValue()}");
		sb.AppendLine(
			$"Connected Terminals: {host.ConnectedTerminals.Count().ToString("N0", user).ColourValue()}");
		sb.AppendLine(
			$"Network Adapters: {host.NetworkAdapters.Count().ToString("N0", user).ColourValue()}");
		sb.AppendLine();
		sb.Append(RenderPrompt());
		return sb.ToString();
	}

	private static string RenderHelp(IComputerBuiltInApplication application)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{application.Name.ColourName()} commands:");
		sb.AppendLine($"\t{"summary".ColourCommand()} - show the local host summary");
		sb.AppendLine($"\t{"services".ColourCommand()} - list built-in applications on the current host");
		sb.AppendLine($"\t{"storage".ColourCommand()} - list mounted storage devices");
		sb.AppendLine($"\t{"terminals".ColourCommand()} - list connected terminals and sessions");
		sb.AppendLine($"\t{"adapters".ColourCommand()} - list local network adapters");
		sb.AppendLine($"\t{"help".ColourCommand()} - show this help");
		sb.AppendLine($"\t{"exit".ColourCommand()} - close Directory");
		sb.AppendLine();
		sb.AppendLine("This first shipped Directory slice is local-only. It shows the current host and its directly connected services and devices.");
		sb.AppendLine();
		sb.Append(RenderPrompt());
		return sb.ToString();
	}

	private static string RenderServices(ICharacter user, IComputerHost host)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Local Services:");
		var applications = host.BuiltInApplications
			.OrderBy(x => x.Name)
			.ThenBy(x => x.Id)
			.ToList();
		if (!applications.Any())
		{
			sb.AppendLine("\tNone");
			sb.AppendLine();
			sb.Append(RenderPrompt());
			return sb.ToString();
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			applications.Select(application => new List<string>
			{
				application.Name,
				application.IsNetworkService ? "network-ready" : "local",
				application.Summary
			}),
			new List<string>
			{
				"Service",
				"Scope",
				"Summary"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
		sb.AppendLine();
		sb.Append(RenderPrompt());
		return sb.ToString();
	}

	private static string RenderStorage(ICharacter user, IComputerHost host)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Mounted Storage:");
		var storage = host.MountedStorage.ToList();
		if (!storage.Any())
		{
			sb.AppendLine("\tNone");
			sb.AppendLine();
			sb.Append(RenderPrompt());
			return sb.ToString();
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			storage.Select(device => new List<string>
			{
				device.Name,
				device.OwnerStorageItemId?.ToString("N0", user) ?? "-",
				device.FileSystem?.UsedBytes.ToString("N0", user) ?? "0",
				device.CapacityInBytes.ToString("N0", user),
				device.Executables.Count().ToString("N0", user)
			}),
			new List<string>
			{
				"Storage",
				"Selector",
				"Used",
				"Capacity",
				"Executables"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
		sb.AppendLine();
		sb.Append(RenderPrompt());
		return sb.ToString();
	}

	private static string RenderTerminals(ICharacter user, IComputerHost host)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Connected Terminals:");
		var terminals = host.ConnectedTerminals.ToList();
		if (!terminals.Any())
		{
			sb.AppendLine("\tNone");
			sb.AppendLine();
			sb.Append(RenderPrompt());
			return sb.ToString();
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			terminals.SelectMany(terminal =>
			{
				var terminalName = DescribeTerminal(user, terminal);
				var sessions = terminal.Sessions.ToList();
				return sessions.Any()
					? sessions.Select(session => new List<string>
					{
						terminalName,
						session.User.HowSeen(user),
						DescribeOwner(session.CurrentOwner),
						session.ConnectedAtUtc.ToString(user)
					})
					: new[]
					{
						new List<string>
						{
							terminalName,
							"Idle",
							"-",
							"-"
						}
					};
			}),
			new List<string>
			{
				"Terminal",
				"User",
				"Owner",
				"Connected"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
		sb.AppendLine();
		sb.Append(RenderPrompt());
		return sb.ToString();
	}

	private static string RenderAdapters(ICharacter user, IComputerHost host)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Network Adapters:");
		var adapters = host.NetworkAdapters.ToList();
		if (!adapters.Any())
		{
			sb.AppendLine("\tNone");
			sb.AppendLine();
			sb.Append(RenderPrompt());
			return sb.ToString();
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			adapters.Select(adapter => new List<string>
			{
				DescribeAdapter(adapter),
				adapter.Powered.ToColouredString(),
				adapter.NetworkReady.ToColouredString(),
				adapter.NetworkAddress ?? "-"
			}),
			new List<string>
			{
				"Adapter",
				"Powered",
				"Ready",
				"Address"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
		sb.AppendLine();
		sb.Append(RenderPrompt());
		return sb.ToString();
	}

	private static string RenderPrompt()
	{
		return $"Use {"type <command>".ColourCommand()} to browse the local directory.";
	}

	private static ComputerProgramExecutionOutcome WaitForInput(IComputerTerminalSession session)
	{
		return new ComputerProgramExecutionOutcome
		{
			Status = ComputerProcessStatus.Sleeping,
			WaitType = ComputerProcessWaitType.UserInput,
			WaitArgument = ComputerProcessWaitArguments.CreateUserInput(session.User.Id, session.Terminal.TerminalItemId),
			WaitingCharacterId = session.User.Id,
			WaitingTerminalItemId = session.Terminal.TerminalItemId,
			StateJson = string.Empty
		};
	}

	private static string DescribeTerminal(ICharacter user, IComputerTerminal terminal)
	{
		return terminal is IGameItemComponent component
			? component.Parent.HowSeen(user, true)
			: $"Terminal #{terminal.TerminalItemId.ToString("N0", user)}";
	}

	private static string DescribeAdapter(INetworkAdapter adapter)
	{
		return adapter is IGameItemComponent component
			? component.Parent.Name
			: adapter.NetworkAddress ?? "Network Adapter";
	}

	private static string DescribeOwner(IComputerExecutableOwner owner)
	{
		return owner switch
		{
			IComputerStorage storage => storage.Name,
			IComputerHost host => host.Name,
			_ => owner.Name
		};
	}
}

internal sealed class SysMonBuiltInApplicationExecutor : IComputerBuiltInApplicationExecutor
{
	public string ApplicationId => "sysmon";

	public ComputerProgramExecutionOutcome Execute(IFuturemud gameworld, ICharacter? actor, IComputerExecutableOwner owner,
		IComputerTerminalSession? session, ComputerRuntimeProcess process, IComputerBuiltInApplication application)
	{
		if (session is null)
		{
			return new ComputerProgramExecutionOutcome
			{
				Status = ComputerProcessStatus.Failed,
				Error = $"{application.Name} requires an active computer terminal session."
			};
		}

		var host = process.Host;
		var sb = new StringBuilder();
		sb.AppendLine($"{application.Name.ColourName()} :: {host.Name.ColourName()}");
		sb.AppendLine($"Selected Owner: {DescribeOwner(session.User, owner)}");
		sb.AppendLine(
			$"Host Power: {(host.Powered ? "powered".ColourValue() : "not powered".ColourError())}");
		if (host.FileSystem is not null)
		{
			sb.AppendLine(
				$"Host Storage: {host.FileSystem.UsedBytes.ToString("N0", session.User).ColourValue()} / {host.FileSystem.CapacityInBytes.ToString("N0", session.User).ColourValue()} bytes used");
		}

		sb.AppendLine(
			$"Mounted Storage: {host.MountedStorage.Count().ToString("N0", session.User).ColourValue()}");
		sb.AppendLine(
			$"Connected Terminals: {host.ConnectedTerminals.Count().ToString("N0", session.User).ColourValue()}");
		sb.AppendLine(
			$"Network Adapters: {host.NetworkAdapters.Count().ToString("N0", session.User).ColourValue()}");
		sb.AppendLine(
			$"Built-In Applications: {host.BuiltInApplications.Select(x => x.Name.ColourName()).ListToString()}");

		AppendStorageSection(sb, session.User, host);
		AppendTerminalSection(sb, session.User, host);
		AppendNetworkSection(sb, session.User, host);
		AppendProcessSection(sb, session.User, host);
		AppendSignalSection(sb, gameworld, session.User, host);

		session.User.OutputHandler.Send(sb.ToString(), nopage: true);
		return new ComputerProgramExecutionOutcome
		{
			Status = ComputerProcessStatus.Completed
		};
	}

	private static void AppendStorageSection(StringBuilder sb, ICharacter user, IComputerHost host)
	{
		sb.AppendLine();
		sb.AppendLine("Storage Devices:");
		var storage = host.MountedStorage.ToList();
		if (!storage.Any())
		{
			sb.AppendLine("\tNone");
			return;
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			storage.Select(x => new List<string>
			{
				x.Name,
				x.CapacityInBytes.ToString("N0", user),
				x.FileSystem?.UsedBytes.ToString("N0", user) ?? "0",
				x.Executables.Count().ToString("N0", user)
			}),
			new List<string>
			{
				"Name",
				"Capacity",
				"Used",
				"Executables"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
	}

	private static void AppendTerminalSection(StringBuilder sb, ICharacter user, IComputerHost host)
	{
		sb.AppendLine();
		sb.AppendLine("Terminal Sessions:");
		var terminals = host.ConnectedTerminals.ToList();
		if (!terminals.Any())
		{
			sb.AppendLine("\tNone");
			return;
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			terminals.SelectMany(terminal =>
			{
				var terminalName = DescribeTerminal(user, terminal);
				var sessions = terminal.Sessions.ToList();
				return sessions.Any()
					? sessions.Select(session => new List<string>
					{
						terminalName,
						session.User.HowSeen(user),
						DescribeOwner(user, session.CurrentOwner),
						session.ConnectedAtUtc.ToString(user)
					})
					: new[]
					{
						new List<string>
						{
							terminalName,
							"Idle",
							"-",
							"-"
						}
					};
			}),
			new List<string>
			{
				"Terminal",
				"User",
				"Owner",
				"Connected"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
	}

	private static void AppendNetworkSection(StringBuilder sb, ICharacter user, IComputerHost host)
	{
		sb.AppendLine();
		sb.AppendLine("Network Adapters:");
		var adapters = host.NetworkAdapters.ToList();
		if (!adapters.Any())
		{
			sb.AppendLine("\tNone");
			return;
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			adapters.Select(adapter => new List<string>
			{
				DescribeAdapter(adapter),
				adapter.Powered.ToColouredString(),
				adapter.NetworkReady.ToColouredString(),
				adapter.NetworkAddress ?? "-"
			}),
			new List<string>
			{
				"Adapter",
				"Powered",
				"Ready",
				"Address"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
	}

	private static void AppendProcessSection(StringBuilder sb, ICharacter user, IComputerHost host)
	{
		sb.AppendLine();
		sb.AppendLine("Processes:");
		var processes = host.Processes
			.OrderByDescending(x => x.LastUpdatedAtUtc)
			.ThenByDescending(x => x.Id)
			.ToList();
		if (!processes.Any())
		{
			sb.AppendLine("\tNone");
			return;
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			processes.Select(process => new List<string>
			{
				process.Id.ToString("N0", user),
				process.ProcessName,
				process.Status.DescribeEnum(),
				DescribeProcessWait(user, process),
				process.LastUpdatedAtUtc.ToString(user)
			}),
			new List<string>
			{
				"Id",
				"Name",
				"Status",
				"Waiting",
				"Updated"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
	}

	private static void AppendSignalSection(StringBuilder sb, IFuturemud gameworld, ICharacter user, IComputerHost host)
	{
		sb.AppendLine();
		sb.AppendLine("Signals:");
		if (host.OwnerHostItemId is not > 0L)
		{
			sb.AppendLine("\tNo real in-world host item is available.");
			return;
		}

		var hostItem = gameworld.TryGetItem(host.OwnerHostItemId.Value, true);
		if (hostItem is null)
		{
			sb.AppendLine("\tThe in-world host item is no longer available.");
			return;
		}

		var signalItems = SignalComponentUtilities.EnumerateAccessibleSignalItems(hostItem)
			.Distinct()
			.ToList();
		var sources = signalItems
			.SelectMany(x => x.Components.OfType<ISignalSourceComponent>())
			.Distinct()
			.ToList();
		var sinks = signalItems
			.SelectMany(x => x.Components.OfType<ISignalSinkComponent>())
			.Distinct()
			.ToList();

		sb.AppendLine("  Sources:");
		if (!sources.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				sources.Select(source => new List<string>
				{
					((IGameItemComponent)source).Parent.HowSeen(user, true),
					((IGameItemComponent)source).Name,
					SignalComponentUtilities.NormaliseSignalEndpointKey(source.EndpointKey),
					source.CurrentSignal.Value.ToString("N2", user),
					DescribeMachineState(source as IGameItemComponent)
				}),
				new List<string>
				{
					"Item",
					"Source",
					"Endpoint",
					"Signal",
					"State"
				},
				user.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				user.Account.UseUnicode));
		}

		sb.AppendLine("  Sinks:");
		if (!sinks.Any())
		{
			sb.AppendLine("\tNone");
			return;
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			sinks.Select(sink =>
			{
				var binding = sink is IRuntimeConfigurableSignalSinkComponent configurable
					? configurable.CurrentBinding
					: new LocalSignalBinding(
						sink.Parent.Id,
						sink.Parent.Name,
						sink.SourceComponentId,
						sink.SourceComponentName,
						sink.SourceEndpointKey);
				var source = SignalComponentUtilities.FindSignalSource(
					sink.Parent,
					binding,
					sink);
				return new List<string>
				{
					sink.Parent.HowSeen(user, true),
					((IGameItemComponent)sink).Name,
					SignalComponentUtilities.DescribeSignalComponent(binding),
					source?.CurrentSignal.Value.ToString("N2", user) ?? "-",
					DescribeMachineState(sink)
				};
			}),
			new List<string>
			{
				"Item",
				"Sink",
				"Binding",
				"Source Signal",
				"State"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
	}

	private static string DescribeOwner(ICharacter user, IComputerExecutableOwner owner)
	{
		return owner switch
		{
			ICharacterComputerWorkspace => "Workspace",
			IComputerStorage => owner.Name,
			IComputerHost => owner.Name,
			_ => owner.Name
		};
	}

	private static string DescribeTerminal(ICharacter user, IComputerTerminal terminal)
	{
		return terminal is IGameItemComponent component
			? component.Parent.HowSeen(user, true)
			: terminal.GetType().Name;
	}

	private static string DescribeAdapter(INetworkAdapter adapter)
	{
		return adapter is IGameItemComponent component
			? component.Parent.Name
			: adapter.GetType().Name;
	}

	private static string DescribeProcessWait(ICharacter user, IComputerProcess process)
	{
		if (process.WaitType == ComputerProcessWaitType.Sleep && process.WakeTimeUtc.HasValue)
		{
			return process.WakeTimeUtc.Value.ToString(user);
		}

		if (process.WaitType == ComputerProcessWaitType.UserInput &&
		    process.WaitingTerminalItemId.HasValue)
		{
			return $"User Input ({process.WaitingTerminalItemId.Value.ToString("N0", user)})";
		}

		if (process.WaitType == ComputerProcessWaitType.Signal &&
		    ComputerProcessWaitArguments.TryParseSignal(process.WaitArgument, out var binding))
		{
			return $"Signal ({SignalComponentUtilities.DescribeSignalComponent(binding)})";
		}

		return process.WaitType.DescribeEnum();
	}

	private static string DescribeMachineState(IGameItemComponent? component)
	{
		return component switch
		{
			PoweredMachineBaseGameItemComponent machine => machine.SwitchedOn
				? machine.IsPowered ? "on, powered" : "on, unpowered"
				: "off",
			IOnOff onOff => onOff.SwitchedOn ? "on" : "off",
			_ => "-"
		};
	}
}
