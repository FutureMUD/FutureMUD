#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using MudSharp.Character;
using MudSharp.Community.Boards;
using MudSharp.Construction.Grids;
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
			new FtpBuiltInApplicationExecutor(),
			new BoardsBuiltInApplicationExecutor(),
			new MailBuiltInApplicationExecutor(),
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

	public static bool IsImplemented(IComputerBuiltInApplication application)
	{
		return _executors.ContainsKey(application.ApplicationId);
	}
}

internal sealed class FileManagerBuiltInApplicationExecutor : IComputerBuiltInApplicationExecutor
{
	private sealed class FileManagerState
	{
		public long? TargetFileOwnerId { get; set; }
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

		var response = HandleCommand(gameworld, session, application, process.Host, state, input!);
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

	private static (string Output, bool Exit) HandleCommand(IFuturemud gameworld, IComputerTerminalSession session,
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
			"list" or "ls" or "dir" when !ss.IsFinished && ss.PeekSpeech().EqualTo("public") => HandleListPublic(gameworld, session, host, state, ss),
			"list" or "ls" or "dir" => (RenderFileList(session.User, host, state, null), false),
			"public" => HandleListPublic(gameworld, session, host, state, ss),
			"owners" or "targets" => (RenderOwners(session.User, host, state), false),
			"show" or "read" or "cat" when !ss.IsFinished && ss.PeekSpeech().EqualTo("public") => HandleShowPublic(gameworld, session, host, state, ss),
			"show" or "read" or "cat" => HandleShow(session, host, state, ss),
			"edit" => HandleEdit(session, host, state, ss),
			"write" => HandleWrite(session, host, state, ss, append: false),
			"append" => HandleWrite(session, host, state, ss, append: true),
			"delete" or "del" or "rm" => HandleDelete(session, host, state, ss),
			"copy" or "cp" when !ss.IsFinished && ss.PeekSpeech().EqualTo("public") => HandleCopyPublic(gameworld, session, host, state, ss),
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
		sb.AppendLine($"Public: {file.PubliclyAccessible.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine(file.TextContents);
		sb.AppendLine();
		sb.Append(RenderPrompt(session.User, host, state, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleListPublic(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, FileManagerState state, StringStack ss)
	{
		if (!ss.IsFinished && ss.PeekSpeech().EqualTo("public"))
		{
			ss.PopSpeech();
		}

		if (ss.IsFinished)
		{
			return ($"Which reachable host do you want to inspect public files on?\n\n{RenderPrompt(session.User, host, state, null)}",
				false);
		}

		var summary = gameworld.ComputerExecutionService.ResolveReachableHost(host, ss.SafeRemainingArgument);
		if (summary is null || !summary.Host.IsNetworkServiceEnabled("ftp"))
		{
			return ($"There is no reachable host advertising public FTP files that matches {ss.SafeRemainingArgument.ColourName()}.\n\n{RenderPrompt(session.User, host, state, null)}",
				false);
		}

		var publicFiles = GetAllPublicRemoteFiles(gameworld, host, summary.Host, out var error);
		if (!string.IsNullOrEmpty(error))
		{
			return ($"{error}\n\n{RenderPrompt(session.User, host, state, null)}", false);
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Public Network Files on {summary.Host.Name.ColourName()} ({summary.CanonicalAddress.ColourName()}):");
		if (!publicFiles.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				publicFiles.Select(file => new List<string>
				{
					file.OwnerDisplayName,
					file.FileName,
					file.SizeInBytes.ToString("N0", session.User),
					file.LastModifiedAtUtc.ToString(session.User)
				}),
				new List<string>
				{
					"Owner",
					"File",
					"Size",
					"Modified"
				},
				session.User.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				session.User.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.Append(RenderPrompt(session.User, host, state, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleShowPublic(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, FileManagerState state, StringStack ss)
	{
		if (!ss.IsFinished && ss.PeekSpeech().EqualTo("public"))
		{
			ss.PopSpeech();
		}

		if (ss.IsFinished)
		{
			return ($"Which reachable host do you want to read from?\n\n{RenderPrompt(session.User, host, state, null)}",
				false);
		}

		var hostIdentifier = ss.PopSpeech();
		if (ss.IsFinished)
		{
			return ($"Which public network file do you want to show?\n\n{RenderPrompt(session.User, host, state, null)}",
				false);
		}

		var summary = gameworld.ComputerExecutionService.ResolveReachableHost(host, hostIdentifier);
		if (summary is null || !summary.Host.IsNetworkServiceEnabled("ftp"))
		{
			return ($"There is no reachable host advertising public FTP files that matches {hostIdentifier.ColourName()}.\n\n{RenderPrompt(session.User, host, state, null)}",
				false);
		}

		var details = ResolvePublicRemoteFile(gameworld, host, summary.Host, ss.SafeRemainingArgument, out var error);
		if (details is null)
		{
			return ($"{error}\n\n{RenderPrompt(session.User, host, state, null)}", false);
		}

		var sb = new StringBuilder();
		sb.AppendLine($"{details.Summary.FileName.ColourName()} on {summary.Host.Name.ColourName()}");
		sb.AppendLine($"Size: {details.Summary.SizeInBytes.ToString("N0", session.User).ColourValue()} bytes");
		sb.AppendLine($"Modified: {details.Summary.LastModifiedAtUtc.ToString(session.User).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine(details.TextContents);
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
			return ($"You must target one of the available local file owners on {host.Name.ColourName()}.\n\n{RenderPrompt(session.User, host, state, warning)}", false);
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
		targetFileSystem.SetFilePubliclyAccessible(sourceFile.FileName, sourceFile.PubliclyAccessible);
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

	private static (string Output, bool Exit) HandleCopyPublic(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, FileManagerState state, StringStack ss)
	{
		if (!ss.IsFinished && ss.PeekSpeech().EqualTo("public"))
		{
			ss.PopSpeech();
		}

		if (ss.IsFinished)
		{
			return ($"Which reachable host do you want to copy a public file from?\n\n{RenderPrompt(session.User, host, state, null)}",
				false);
		}

		var hostIdentifier = ss.PopSpeech();
		if (ss.IsFinished)
		{
			return ($"Which public file do you want to copy?\n\n{RenderPrompt(session.User, host, state, null)}", false);
		}

		var remoteFileName = ss.PopSpeech();
		if (!ss.IsFinished && ss.PeekSpeech().EqualTo("to"))
		{
			ss.PopSpeech();
		}

		var summary = gameworld.ComputerExecutionService.ResolveReachableHost(host, hostIdentifier);
		if (summary is null || !summary.Host.IsNetworkServiceEnabled("ftp"))
		{
			return ($"There is no reachable host advertising public FTP files that matches {hostIdentifier.ColourName()}.\n\n{RenderPrompt(session.User, host, state, null)}",
				false);
		}

		var details = ResolvePublicRemoteFile(gameworld, host, summary.Host, remoteFileName, out var error);
		if (details is null)
		{
			return ($"{error}\n\n{RenderPrompt(session.User, host, state, null)}", false);
		}

		var (targetOwner, warning) = ResolveTargetOwner(host, state);
		if (!ss.IsFinished)
		{
			var explicitTarget = ResolveSelectableOwner(host, ss.SafeRemainingArgument);
			if (explicitTarget is null)
			{
				return ($"You must target one of the available local file owners on {host.Name.ColourName()}.\n\n{RenderPrompt(session.User, host, state, warning)}",
					false);
			}

			targetOwner = explicitTarget;
			warning = null;
		}

		var targetFileSystem = targetOwner.FileSystem;
		if (targetFileSystem is null)
		{
			return ($"{DescribeOwner(targetOwner).ColourName()} does not expose a writable file system.\n\n{RenderPrompt(session.User, host, state, warning)}",
				false);
		}

		targetFileSystem.WriteFile(details.Summary.FileName, details.TextContents);
		targetFileSystem.SetFilePubliclyAccessible(details.Summary.FileName, false);
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(warning))
		{
			sb.AppendLine(warning);
			sb.AppendLine();
		}

		sb.AppendLine($"Copied public network file {details.Summary.FileName.ColourName()} from {summary.Host.Name.ColourName()} to {DescribeOwner(targetOwner).ColourName()}.");
		sb.Append(RenderPrompt(session.User, host, state, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleUse(IComputerTerminalSession session, IComputerHost host,
		FileManagerState state, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ("Which local file owner do you want to use?\n\n" + RenderPrompt(session.User, host, state, null), false);
		}

		var targetText = ss.SafeRemainingArgument;
		var targetOwner = ResolveSelectableOwner(host, targetText);
		if (targetOwner is null)
		{
			return ($"You must target one of the available local file owners on {host.Name.ColourName()}.\n\n{RenderPrompt(session.User, host, state, null)}", false);
		}

		state.TargetFileOwnerId = targetOwner.FileOwnerId > 0L ? targetOwner.FileOwnerId : null;
		return ($"Now managing files on {DescribeOwner(targetOwner).ColourName()}.\n\n{RenderPrompt(session.User, host, state, null)}", false);
	}

	private static void SendOverview(IComputerTerminalSession session, IComputerBuiltInApplication application,
		IComputerHost host, FileManagerState state, IComputerFileOwner initialOwner)
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
		sb.AppendLine($"\t{"list public <host>".ColourCommand()} - list published public files on a reachable host");
		sb.AppendLine($"\t{"show <file>".ColourCommand()} - display a file");
		sb.AppendLine($"\t{"show public <host> <file>".ColourCommand()} - display a published public network file");
		sb.AppendLine($"\t{"edit <file>".ColourCommand()} - open a file in the normal multiline editor");
		sb.AppendLine($"\t{"write <file> <text>".ColourCommand()} - overwrite a file");
		sb.AppendLine($"\t{"append <file> <text>".ColourCommand()} - append text to a file");
		sb.AppendLine($"\t{"delete <file>".ColourCommand()} - delete a file");
		sb.AppendLine($"\t{"copy <file> <target>".ColourCommand()} - copy a file to any available local file owner");
		sb.AppendLine($"\t{"copy public <host> <file> [to <target>]".ColourCommand()} - copy a published public network file to a local target");
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
				file.PubliclyAccessible.ToColouredString(),
				file.LastModifiedAtUtc.ToString(user)
			}),
			new List<string>
			{
				"File",
				"Size",
				"Public",
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
		var owners = ComputerFileTransferUtilities.EnumerateOwners(host).ToList();
		sb.AppendLine(StringUtilities.GetTextTable(
			owners.Select(owner => new List<string>
			{
				DescribeOwner(owner),
				ComputerFileTransferUtilities.GetOwnerIdentifier(host, owner),
				(owner.FileSystem?.Files.Count(x => x.PubliclyAccessible) ?? 0).ToString("N0", user),
				owner.FileSystem?.UsedBytes.ToString("N0", user) ?? "0",
				owner.FileSystem?.CapacityInBytes.ToString("N0", user) ?? "0"
			}),
			new List<string>
			{
				"Owner",
				"Use",
				"Public Files",
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

	private static List<ComputerRemoteFileSummary> GetAllPublicRemoteFiles(IFuturemud gameworld, IComputerHost sourceHost,
		IComputerHost targetHost, out string error)
	{
		error = string.Empty;
		var owners = gameworld.ComputerFileTransferService.GetAccessibleOwners(sourceHost, targetHost, null, out error)
			.ToList();
		if (!string.IsNullOrEmpty(error))
		{
			return [];
		}

		var files = new List<ComputerRemoteFileSummary>();
		foreach (var owner in owners)
		{
			var ownerFiles = gameworld.ComputerFileTransferService.GetFiles(sourceHost, targetHost, null, owner.OwnerIdentifier,
				out error).ToList();
			if (!string.IsNullOrEmpty(error))
			{
				return [];
			}

			files.AddRange(ownerFiles);
		}

		return files
			.OrderBy(x => x.OwnerDisplayName)
			.ThenBy(x => x.FileName)
			.ThenBy(x => x.CreatedAtUtc)
			.ToList();
	}

	private static ComputerRemoteFileDetails? ResolvePublicRemoteFile(IFuturemud gameworld, IComputerHost sourceHost,
		IComputerHost targetHost, string fileName, out string error)
	{
		error = string.Empty;
		var files = GetAllPublicRemoteFiles(gameworld, sourceHost, targetHost, out error);
		if (!string.IsNullOrEmpty(error))
		{
			return null;
		}

		var exact = files
			.Where(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (exact.Count > 1)
		{
			error = $"More than one public network file on {targetHost.Name.ColourName()} is named {fileName.ColourName()}. Use FTP to disambiguate by owner.";
			return null;
		}

		ComputerRemoteFileSummary? summary = null;
		if (exact.Count == 1)
		{
			summary = exact.Single();
		}
		else
		{
			var partial = files
				.Where(x => x.FileName.StartsWith(fileName, StringComparison.InvariantCultureIgnoreCase))
				.OrderBy(x => x.FileName.Length)
				.ThenBy(x => x.FileName)
				.ThenBy(x => x.OwnerDisplayName)
				.ToList();
			if (partial.Count > 1)
			{
				error = $"More than one public network file on {targetHost.Name.ColourName()} starts with {fileName.ColourName()}. Use FTP to disambiguate by owner.";
				return null;
			}

			summary = partial.SingleOrDefault();
		}

		if (summary is null)
		{
			error = $"{targetHost.Name.ColourName()} does not expose a public network file named {fileName.ColourName()}.";
			return null;
		}

		return gameworld.ComputerFileTransferService.ReadFile(sourceHost, targetHost, null, summary.OwnerIdentifier,
			summary.FileName, out error);
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
			TargetFileOwnerId = owner.FileOwnerId > 0L ? owner.FileOwnerId : null
		};
	}

	private static (IComputerFileOwner Owner, string? Warning) ResolveTargetOwner(IComputerHost host,
		FileManagerState state)
	{
		if (state.TargetFileOwnerId is not > 0L)
		{
			return (host, null);
		}

		var owner = ComputerFileTransferUtilities.EnumerateOwners(host)
			.FirstOrDefault(x => x.FileOwnerId == state.TargetFileOwnerId.Value);
		if (owner is not null)
		{
			return (owner, null);
		}

		state.TargetFileOwnerId = null;
		return (host, "The previously selected file target is no longer available, so FileManager has reverted to the host.");
	}

	private static IComputerFileOwner? ResolveSelectableOwner(IComputerHost host, string identifier)
	{
		return ComputerFileTransferUtilities.ResolveSelectableOwner(host, identifier, out _);
	}

	private static string DescribeOwner(IComputerFileOwner owner)
	{
		return ComputerFileTransferUtilities.DescribeOwner(owner);
	}
}

internal sealed class BoardsBuiltInApplicationExecutor : IComputerBuiltInApplicationExecutor
{
	private sealed class BoardsState
	{
		public long? TargetHostItemId { get; set; }
		public long? LoggedInAccountId { get; set; }
		public string LoggedInAddress { get; set; } = string.Empty;
		public long? SelectedBoardId { get; set; }

		public void ClearLogin()
		{
			LoggedInAccountId = null;
			LoggedInAddress = string.Empty;
		}

		public void SelectTargetHost(IComputerHost host)
		{
			TargetHostItemId = host.OwnerHostItemId;
			SelectedBoardId = null;
			ClearLogin();
		}
	}

	public string ApplicationId => "boards";

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

		var state = LoadState(process.StateJson, process.Host);
		var targetHost = ResolveTargetHost(gameworld, session, process.Host, state, out var hostWarning);
		var account = ResolveLoggedInAccount(gameworld, process.Host, targetHost, state, out var accountWarning);
		var board = ResolveSelectedBoard(gameworld, targetHost, state, out var boardWarning);
		var warning = hostWarning ?? accountWarning ?? boardWarning;
		var input = ComputerExecutionContextScope.Current?.ConsumePendingTerminalInput();
		if (string.IsNullOrWhiteSpace(input))
		{
			SendOverview(gameworld, session, application, state, targetHost, account, board, warning);
			return WaitForInput(session, state);
		}

		var response = HandleCommand(gameworld, session, application, process, state, targetHost, account, board, input!);
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

	private static (string Output, bool Exit) HandleCommand(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerBuiltInApplication application, ComputerRuntimeProcess process, BoardsState state, IComputerHost? targetHost,
		IComputerNetworkAccount? account, IBoard? board, string input)
	{
		var ss = new StringStack(input.Trim());
		var command = ss.PopSpeech().ToLowerInvariant();
		return command switch
		{
			"" => (RenderPrompt(session.User, state, targetHost, account, board, null), false),
			"help" => (RenderHelp(session.User, application, state, targetHost, account, board), false),
			"hosts" => (RenderHosts(gameworld, session, process.Host), false),
			"open" => HandleOpen(gameworld, session, state, process.Host, ss),
			"login" => HandleLogin(gameworld, session, state, process.Host, targetHost, ss),
			"logout" => HandleLogout(session, state, targetHost),
			"boards" or "listboards" => HandleBoards(gameworld, session, state, targetHost, account, board),
			"use" or "select" => HandleUse(gameworld, session, state, targetHost, ss),
			"list" => HandleList(gameworld, session, state, targetHost, board),
			"read" or "show" => HandleRead(gameworld, session, state, targetHost, board, ss),
			"post" or "write" => HandlePost(gameworld, session, process, state, targetHost, account, board, ss),
			"delete" or "del" => HandleDelete(gameworld, session, state, targetHost, account, board, ss),
			"exit" or "quit" => ($"{application.Name.ColourName()} closing.", true),
			_ => ($"That is not a valid {application.Name.ColourName()} command.\n\n{RenderPrompt(session.User, state, targetHost, account, board, null)}", false)
		};
	}

	private static (string Output, bool Exit) HandleOpen(IFuturemud gameworld, IComputerTerminalSession session,
		BoardsState state, IComputerHost localHost, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ($"Which boards service host do you want to open?\n\n{RenderPrompt(session.User, state, ResolveTargetHost(gameworld, session, localHost, state, out _), null, null, null)}",
				false);
		}

		var identifier = ss.SafeRemainingArgument.Trim();
		var targetHost = ResolveServiceHost(gameworld, session, localHost, identifier, out var error);
		if (targetHost is null)
		{
			return ($"{error}\n\n{RenderPrompt(session.User, state, ResolveTargetHost(gameworld, session, localHost, state, out _), null, null, null)}",
				false);
		}

		state.SelectTargetHost(targetHost);
		return ($"Opened the boards service on {targetHost.Name.ColourName()}.\n\n{RenderPrompt(session.User, state, targetHost, null, null, null)}",
			false);
	}

	private static (string Output, bool Exit) HandleLogin(IFuturemud gameworld, IComputerTerminalSession session,
		BoardsState state, IComputerHost localHost, IComputerHost? targetHost, StringStack ss)
	{
		if (targetHost is null)
		{
			return ($"Open a boards service first with {"type open <host>".ColourCommand()}.\n\n{RenderPrompt(session.User, state, null, null, null, null)}",
				false);
		}

		if (ss.IsFinished)
		{
			return ($"Which network account do you want to use?\n\n{RenderPrompt(session.User, state, targetHost, null, null, null)}",
				false);
		}

		var address = ss.PopSpeech();
		if (ss.IsFinished)
		{
			return ($"What password do you want to use for {address.ColourName()}?\n\n{RenderPrompt(session.User, state, targetHost, null, null, null)}",
				false);
		}

		var password = ss.SafeRemainingArgument;
		var authentication = gameworld.ComputerNetworkIdentityService.Authenticate(localHost, address, password);
		if (!authentication.Success || authentication.Account is null)
		{
			return ($"{authentication.ErrorMessage}\n\n{RenderPrompt(session.User, state, targetHost, null, null, null)}", false);
		}

		if (!HostAcceptsAccount(gameworld, targetHost, authentication.Account))
		{
			return ($"{targetHost.Name.ColourName()} does not accept boards logins for {authentication.Account.Address.ColourName()}.\n\n{RenderPrompt(session.User, state, targetHost, null, null, null)}",
				false);
		}

		state.LoggedInAccountId = authentication.Account.Id;
		state.LoggedInAddress = authentication.Account.Address;
		return ($"You log in to {authentication.Account.Address.ColourName()} on {targetHost.Name.ColourName()}.\n\n{RenderPrompt(session.User, state, targetHost, authentication.Account, ResolveSelectedBoard(gameworld, targetHost, state, out _), null)}",
			false);
	}

	private static (string Output, bool Exit) HandleLogout(IComputerTerminalSession session, BoardsState state,
		IComputerHost? targetHost)
	{
		if (state.LoggedInAccountId is not > 0L)
		{
			return ($"You are not currently logged in to any network board identity.\n\n{RenderPrompt(session.User, state, targetHost, null, null, null)}",
				false);
		}

		var address = state.LoggedInAddress;
		state.ClearLogin();
		return ($"You log out of {address.ColourName()}.\n\n{RenderPrompt(session.User, state, targetHost, null, null, null)}",
			false);
	}

	private static (string Output, bool Exit) HandleBoards(IFuturemud gameworld, IComputerTerminalSession session,
		BoardsState state, IComputerHost? targetHost, IComputerNetworkAccount? account, IBoard? board)
	{
		if (targetHost is null)
		{
			return ($"There is no current boards service selected. Use {"type hosts".ColourCommand()} and {"type open <host>".ColourCommand()} first.\n\n{RenderPrompt(session.User, state, null, account, board, null)}",
				false);
		}

		var hostedBoards = gameworld.ComputerBoardService.GetHostedBoardDetails(targetHost).ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"Hosted Boards on {targetHost.Name.ColourName()}:");
		if (!hostedBoards.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				hostedBoards.Select(x => new List<string>
				{
					x.BoardId.ToString("N0", session.User),
					x.BoardName,
					x.PostCount.ToString("N0", session.User)
				}),
				new List<string>
				{
					"Id",
					"Board",
					"Posts"
				},
				session.User.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				session.User.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.Append(RenderPrompt(session.User, state, targetHost, account, board, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleUse(IFuturemud gameworld, IComputerTerminalSession session,
		BoardsState state, IComputerHost? targetHost, StringStack ss)
	{
		if (targetHost is null)
		{
			return ($"Open a boards service first with {"type open <host>".ColourCommand()}.\n\n{RenderPrompt(session.User, state, null, null, null, null)}",
				false);
		}

		if (ss.IsFinished)
		{
			return ($"Which hosted board do you want to select?\n\n{RenderPrompt(session.User, state, targetHost, null, null, null)}",
				false);
		}

		var selectedBoard = gameworld.ComputerBoardService.ResolveHostedBoard(targetHost, ss.SafeRemainingArgument, out var error);
		if (selectedBoard is null)
		{
			return ($"{error}\n\n{RenderPrompt(session.User, state, targetHost, null, null, null)}", false);
		}

		state.SelectedBoardId = selectedBoard.Id;
		return ($"Selected the hosted board {selectedBoard.Name.ColourName()} on {targetHost.Name.ColourName()}.\n\n{RenderPrompt(session.User, state, targetHost, ResolveLoggedInAccount(gameworld, session.Host, targetHost, state, out _), selectedBoard, null)}",
			false);
	}

	private static (string Output, bool Exit) HandleList(IFuturemud gameworld, IComputerTerminalSession session,
		BoardsState state, IComputerHost? targetHost, IBoard? board)
	{
		if (targetHost is null)
		{
			return ($"Open a boards service first with {"type open <host>".ColourCommand()}.\n\n{RenderPrompt(session.User, state, null, null, null, null)}",
				false);
		}

		if (board is null)
		{
			return ($"Select one hosted board first with {"type use <board>".ColourCommand()}.\n\n{RenderPrompt(session.User, state, targetHost, ResolveLoggedInAccount(gameworld, session.Host, targetHost, state, out _), null, null)}",
				false);
		}

		var posts = gameworld.ComputerBoardService.GetPosts(targetHost, board).ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"{board.Name.ColourName()} on {targetHost.Name.ColourName()}:");
		if (!posts.Any())
		{
			sb.AppendLine("There are no posts on this board.");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				posts.Select(x => new List<string>
				{
					x.Id.ToString("N0", session.User),
					x.AuthorName,
					x.Title,
					x.PostTime.ToString(session.User)
				}),
				new List<string>
				{
					"Id",
					"Author",
					"Title",
					"Posted"
				},
				session.User.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				session.User.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.Append(RenderPrompt(session.User, state, targetHost, ResolveLoggedInAccount(gameworld, session.Host, targetHost, state, out _), board, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleRead(IFuturemud gameworld, IComputerTerminalSession session,
		BoardsState state, IComputerHost? targetHost, IBoard? board, StringStack ss)
	{
		if (targetHost is null)
		{
			return ($"Open a boards service first with {"type open <host>".ColourCommand()}.\n\n{RenderPrompt(session.User, state, null, null, null, null)}",
				false);
		}

		if (board is null)
		{
			return ($"Select one hosted board first with {"type use <board>".ColourCommand()}.\n\n{RenderPrompt(session.User, state, targetHost, ResolveLoggedInAccount(gameworld, session.Host, targetHost, state, out _), null, null)}",
				false);
		}

		if (ss.IsFinished || !long.TryParse(ss.PopSpeech(), out var postId))
		{
			return ($"Which board post do you want to read?\n\n{RenderPrompt(session.User, state, targetHost, ResolveLoggedInAccount(gameworld, session.Host, targetHost, state, out _), board, null)}",
				false);
		}

		var post = gameworld.ComputerBoardService.ReadPost(targetHost, board, postId, out var error);
		if (post is null)
		{
			return ($"{error}\n\n{RenderPrompt(session.User, state, targetHost, ResolveLoggedInAccount(gameworld, session.Host, targetHost, state, out _), board, null)}",
				false);
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Post {post.Id.ToString("N0", session.User).ColourValue()} :: {board.Name.ColourName()}");
		sb.AppendLine($"Author: {post.AuthorName.ColourName()}");
		sb.AppendLine($"Posted: {post.PostTime.ToString(session.User).ColourValue()}");
		if (post.InGameDateTime is not null)
		{
			sb.AppendLine($"In-Game: {post.InGameDateTime.GetDateTimeString().ColourValue()}");
		}

		sb.AppendLine($"Title: {post.Title.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine(post.Text);
		sb.AppendLine();
		sb.Append(RenderPrompt(session.User, state, targetHost, ResolveLoggedInAccount(gameworld, session.Host, targetHost, state, out _), board, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandlePost(IFuturemud gameworld, IComputerTerminalSession session,
		ComputerRuntimeProcess process, BoardsState state, IComputerHost? targetHost, IComputerNetworkAccount? account,
		IBoard? board, StringStack ss)
	{
		if (targetHost is null)
		{
			return ($"Open a boards service first with {"type open <host>".ColourCommand()}.\n\n{RenderPrompt(session.User, state, null, null, null, null)}",
				false);
		}

		if (account is null)
		{
			return ($"Log in first with {"type login <user@domain> <password>".ColourCommand()} before posting.\n\n{RenderPrompt(session.User, state, targetHost, null, board, null)}",
				false);
		}

		if (board is null)
		{
			return ($"Select one hosted board first with {"type use <board>".ColourCommand()}.\n\n{RenderPrompt(session.User, state, targetHost, account, null, null)}",
				false);
		}

		if (ss.IsFinished)
		{
			return ($"What title do you want to give to your board post?\n\n{RenderPrompt(session.User, state, targetHost, account, board, null)}",
				false);
		}

		var title = ss.SafeRemainingArgument.Trim();
		if (title.Length > 200)
		{
			return ($"Board post titles must be 200 characters or fewer.\n\n{RenderPrompt(session.User, state, targetHost, account, board, null)}",
				false);
		}

		var boardName = board.Name;
		session.User.EditorMode(
			(text, handler, _) =>
			{
				if (!gameworld.ComputerBoardService.CreatePost(targetHost, account, board, title, text, out var error))
				{
					handler.Send(error);
					return;
				}

				handler.Send(
					$"You post {title.ColourCommand()} to {boardName.ColourName()}. Continue with {"type list".ColourCommand()} to review the board.");
			},
			(handler, _) =>
			{
				handler.Send($"You decide not to post to {boardName.ColourName()}.");
			},
			1.0,
			string.Empty,
			EditorOptions.PermitEmpty);
		return (string.Empty, false);
	}

	private static (string Output, bool Exit) HandleDelete(IFuturemud gameworld, IComputerTerminalSession session,
		BoardsState state, IComputerHost? targetHost, IComputerNetworkAccount? account, IBoard? board, StringStack ss)
	{
		if (targetHost is null)
		{
			return ($"Open a boards service first with {"type open <host>".ColourCommand()}.\n\n{RenderPrompt(session.User, state, null, null, null, null)}",
				false);
		}

		if (account is null)
		{
			return ($"Log in first with {"type login <user@domain> <password>".ColourCommand()} before deleting a post.\n\n{RenderPrompt(session.User, state, targetHost, null, board, null)}",
				false);
		}

		if (board is null)
		{
			return ($"Select one hosted board first with {"type use <board>".ColourCommand()}.\n\n{RenderPrompt(session.User, state, targetHost, account, null, null)}",
				false);
		}

		if (ss.IsFinished || !long.TryParse(ss.PopSpeech(), out var postId))
		{
			return ($"Which board post do you want to delete?\n\n{RenderPrompt(session.User, state, targetHost, account, board, null)}",
				false);
		}

		if (!gameworld.ComputerBoardService.DeletePost(targetHost, account, board, postId, out var error))
		{
			return ($"{error}\n\n{RenderPrompt(session.User, state, targetHost, account, board, null)}", false);
		}

		return ($"Deleted board post {postId.ToString("N0", session.User).ColourValue()} from {board.Name.ColourName()}.\n\n{RenderPrompt(session.User, state, targetHost, account, board, null)}",
			false);
	}

	private static void SendOverview(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerBuiltInApplication application, BoardsState state, IComputerHost? targetHost, IComputerNetworkAccount? account,
		IBoard? board, string? warning)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{application.Name.ColourName()} :: {session.Host.Name.ColourName()}");
		sb.AppendLine(RenderHelp(session.User, application, state, targetHost, account, board));
		if (!string.IsNullOrEmpty(warning))
		{
			sb.AppendLine();
			sb.AppendLine(warning);
		}

		session.User.OutputHandler.Send(sb.ToString(), nopage: true);
	}

	private static string RenderHelp(ICharacter user, IComputerBuiltInApplication application, BoardsState state,
		IComputerHost? targetHost, IComputerNetworkAccount? account, IBoard? board)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{application.Name.ColourName()} commands:");
		sb.AppendLine($"\t{"hosts".ColourCommand()} - list reachable hosts advertising Boards");
		sb.AppendLine($"\t{"open <host>".ColourCommand()} - choose which boards service host to use");
		sb.AppendLine($"\t{"login <user@domain> <password>".ColourCommand()} - authenticate for posting and deletion");
		sb.AppendLine($"\t{"logout".ColourCommand()} - log out of the current network identity");
		sb.AppendLine($"\t{"boards".ColourCommand()} - list boards exposed by the selected host");
		sb.AppendLine($"\t{"use <board>".ColourCommand()} - select one hosted board");
		sb.AppendLine($"\t{"list".ColourCommand()} - list posts on the selected board");
		sb.AppendLine($"\t{"read <id>".ColourCommand()} - read one post from the selected board");
		sb.AppendLine($"\t{"post <title>".ColourCommand()} - open the multiline editor to create a new post");
		sb.AppendLine($"\t{"delete <id>".ColourCommand()} - delete one of your own network-authored posts");
		sb.AppendLine($"\t{"help".ColourCommand()} - show this help");
		sb.AppendLine($"\t{"exit".ColourCommand()} - close Boards");
		sb.AppendLine();
		sb.Append(RenderPrompt(user, state, targetHost, account, board, null));
		return sb.ToString();
	}

	private static string RenderHosts(IFuturemud gameworld, IComputerTerminalSession session, IComputerHost localHost)
	{
		var rows = new List<List<string>>();
		if (gameworld.ComputerBoardService.IsBoardsServiceEnabled(localHost) &&
		    gameworld.ComputerBoardService.GetHostedBoardDetails(localHost).Any())
		{
			rows.Add(new List<string>
			{
				localHost.Name,
				"local",
				"Local",
				gameworld.ComputerBoardService.GetHostedBoardDetails(localHost).Count().ToString("N0", session.User)
			});
		}

		rows.AddRange(gameworld.ComputerExecutionService.GetReachableHosts(localHost, session)
			.Where(x => x.Host.IsNetworkServiceEnabled("boards"))
			.Where(x => gameworld.ComputerBoardService.GetHostedBoardDetails(x.Host).Any())
			.Where(x => x.Host.OwnerHostItemId != localHost.OwnerHostItemId)
			.Select(x => new List<string>
			{
				x.Host.Name,
				x.CanonicalAddress,
				x.IsLocalGrid ? "Local Grid" : "Linked Grid",
				gameworld.ComputerBoardService.GetHostedBoardDetails(x.Host).Count().ToString("N0", session.User)
			}));

		var sb = new StringBuilder();
		sb.AppendLine("Reachable Boards Hosts:");
		if (!rows.Any())
		{
			sb.AppendLine("\tNone");
			sb.AppendLine();
			sb.Append("There are no reachable hosts currently advertising Boards.");
			return sb.ToString();
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			rows,
			new List<string>
			{
				"Host",
				"Address",
				"Scope",
				"Boards"
			},
			session.User.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			session.User.Account.UseUnicode));
		return sb.ToString();
	}

	private static string RenderPrompt(ICharacter user, BoardsState state, IComputerHost? targetHost,
		IComputerNetworkAccount? account, IBoard? board, string? warning)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(warning))
		{
			sb.AppendLine(warning);
		}

		if (targetHost is null)
		{
			sb.Append($"Use {"type hosts".ColourCommand()} to discover reachable boards services.");
			return sb.ToString();
		}

		sb.Append($"Boards host: {targetHost.Name.ColourName()}.");
		sb.Append(account is null
			? $" Not logged in. Use {"type login <user@domain> <password>".ColourCommand()} to authenticate for posting."
			: $" Logged in as {account.Address.ColourName()}.");
		sb.Append(board is null
			? $" Select a board with {"type use <board>".ColourCommand()}."
			: $" Current board: {board.Name.ColourName()}.");
		return sb.ToString();
	}

	private static BoardsState LoadState(string? stateJson, IComputerHost localHost)
	{
		if (!string.IsNullOrWhiteSpace(stateJson))
		{
			try
			{
				var state = JsonSerializer.Deserialize<BoardsState>(stateJson);
				if (state is not null)
				{
					return state;
				}
			}
			catch (JsonException)
			{
			}
		}

		return new BoardsState
		{
			TargetHostItemId = localHost.IsNetworkServiceEnabled("boards") &&
			                   localHost.HostedBoardIds.Any()
				? localHost.OwnerHostItemId
				: null
		};
	}

	private static IComputerHost? ResolveTargetHost(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost localHost, BoardsState state, out string? warning)
	{
		warning = null;
		if (state.TargetHostItemId is > 0L)
		{
			if (localHost.OwnerHostItemId == state.TargetHostItemId &&
			    gameworld.ComputerBoardService.IsBoardsServiceEnabled(localHost) &&
			    gameworld.ComputerBoardService.GetHostedBoardDetails(localHost).Any())
			{
				return localHost;
			}

			var remote = gameworld.ComputerExecutionService.GetReachableHosts(localHost, session)
				.Select(x => x.Host)
				.FirstOrDefault(x =>
					x.OwnerHostItemId == state.TargetHostItemId &&
					x.IsNetworkServiceEnabled("boards") &&
					gameworld.ComputerBoardService.GetHostedBoardDetails(x).Any());
			if (remote is not null)
			{
				return remote;
			}

			warning = "The previously selected boards service host is no longer reachable.";
			state.TargetHostItemId = null;
			state.SelectedBoardId = null;
			state.ClearLogin();
		}

		if (gameworld.ComputerBoardService.IsBoardsServiceEnabled(localHost) &&
		    gameworld.ComputerBoardService.GetHostedBoardDetails(localHost).Any())
		{
			state.TargetHostItemId = localHost.OwnerHostItemId;
			return localHost;
		}

		return null;
	}

	private static IComputerHost? ResolveServiceHost(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost localHost, string identifier, out string error)
	{
		error = string.Empty;
		if (identifier.EqualTo("local") &&
		    gameworld.ComputerBoardService.IsBoardsServiceEnabled(localHost) &&
		    gameworld.ComputerBoardService.GetHostedBoardDetails(localHost).Any())
		{
			return localHost;
		}

		if ((localHost.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase) ||
		     localHost.OwnerHostItemId?.ToString() == identifier) &&
		    gameworld.ComputerBoardService.IsBoardsServiceEnabled(localHost) &&
		    gameworld.ComputerBoardService.GetHostedBoardDetails(localHost).Any())
		{
			return localHost;
		}

		var summary = gameworld.ComputerExecutionService.ResolveReachableHost(localHost, identifier, session);
		if (summary is null || !summary.Host.IsNetworkServiceEnabled("boards"))
		{
			error = $"There is no reachable host advertising Boards that matches {identifier.ColourName()}.";
			return null;
		}

		if (!gameworld.ComputerBoardService.GetHostedBoardDetails(summary.Host).Any())
		{
			error = $"{summary.Host.Name.ColourName()} is not currently exposing any network boards.";
			return null;
		}

		return summary.Host;
	}

	private static IComputerNetworkAccount? ResolveLoggedInAccount(IFuturemud gameworld, IComputerHost executionHost,
		IComputerHost? targetHost, BoardsState state, out string? warning)
	{
		warning = null;
		if (state.LoggedInAccountId is not > 0L)
		{
			return null;
		}

		var account = gameworld.ComputerNetworkIdentityService.GetAccount(executionHost, state.LoggedInAccountId.Value,
			out var error);
		if (account is null)
		{
			warning = error;
			state.ClearLogin();
			return null;
		}

		if (targetHost is not null && !HostAcceptsAccount(gameworld, targetHost, account))
		{
			warning = $"{targetHost.Name.ColourName()} no longer accepts {account.Address.ColourName()} for board posting.";
			state.ClearLogin();
			return null;
		}

		state.LoggedInAddress = account.Address;
		return account;
	}

	private static IBoard? ResolveSelectedBoard(IFuturemud gameworld, IComputerHost? targetHost, BoardsState state,
		out string? warning)
	{
		warning = null;
		if (targetHost is null || state.SelectedBoardId is not > 0L)
		{
			return null;
		}

		var board = gameworld.ComputerBoardService.GetHostedBoards(targetHost)
			.FirstOrDefault(x => x.Id == state.SelectedBoardId.Value);
		if (board is not null)
		{
			return board;
		}

		warning = "The previously selected board is no longer exposed by that host.";
		state.SelectedBoardId = null;
		return null;
	}

	private static bool HostAcceptsAccount(IFuturemud gameworld, IComputerHost targetHost, IComputerNetworkAccount account)
	{
		return gameworld.ComputerNetworkIdentityService.GetHostedDomains(targetHost)
			.Any(x => x.Enabled && x.DomainName.Equals(account.DomainName, StringComparison.InvariantCultureIgnoreCase));
	}

	private static ComputerProgramExecutionOutcome WaitForInput(IComputerTerminalSession session, BoardsState state)
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
}

internal sealed class MailBuiltInApplicationExecutor : IComputerBuiltInApplicationExecutor
{
	private sealed class MailState
	{
		public long? LoggedInAccountId { get; set; }
		public string LoggedInAddress { get; set; } = string.Empty;
		public string ComposeRecipient { get; set; } = string.Empty;
		public string ComposeSubject { get; set; } = string.Empty;
		public string ComposeBody { get; set; } = string.Empty;

		public void ClearCompose()
		{
			ComposeRecipient = string.Empty;
			ComposeSubject = string.Empty;
			ComposeBody = string.Empty;
		}
	}

	public string ApplicationId => "mail";

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

		var state = LoadState(process.StateJson);
		var account = ResolveLoggedInAccount(gameworld, process.Host, state, out var warning);
		var input = ComputerExecutionContextScope.Current?.ConsumePendingTerminalInput();
		if (string.IsNullOrWhiteSpace(input))
		{
			SendOverview(session, application, process, state, account, warning);
			return WaitForInput(session, state);
		}

		var response = HandleCommand(gameworld, session, application, process, state, account, input!);
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

	private static (string Output, bool Exit) HandleCommand(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerBuiltInApplication application, ComputerRuntimeProcess process, MailState state, IComputerMailAccount? account,
		string input)
	{
		var ss = new StringStack(input.Trim());
		var command = ss.PopSpeech().ToLowerInvariant();
		return command switch
		{
			"" => (RenderPrompt(session.User, application, state, account, null), false),
			"help" => (RenderHelp(session.User, application, state, account), false),
			"login" => HandleLogin(gameworld, session, application, process.Host, state, ss),
			"logout" => HandleLogout(session, application, state, account),
			"inbox" or "list" => HandleInbox(gameworld, session, application, process.Host, state, account),
			"read" or "show" => HandleRead(gameworld, session, application, process.Host, state, account, ss),
			"delete" or "del" => HandleDelete(gameworld, session, application, process.Host, state, account, ss),
			"send" => HandleSend(session, application, state, account, ss),
			"subject" => HandleSubject(session, application, state, account, ss),
			"body" => HandleBody(session, application, process, state, account),
			"post" => HandlePost(gameworld, session, application, process.Host, state, account),
			"cancel" => HandleCancel(session, application, state, account),
			"exit" or "quit" => ($"{application.Name.ColourName()} closing.", true),
			_ => ($"That is not a valid {application.Name.ColourName()} command.\n\n{RenderPrompt(session.User, application, state, account, null)}", false)
		};
	}

	private static (string Output, bool Exit) HandleLogin(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerBuiltInApplication application, IComputerHost host, MailState state, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ($"Which mailbox do you want to log in to?\n\n{RenderPrompt(session.User, application, state, null, null)}", false);
		}

		var address = ss.PopSpeech();
		if (ss.IsFinished)
		{
			return ($"What password do you want to use for {address.ColourName()}?\n\n{RenderPrompt(session.User, application, state, null, null)}", false);
		}

		var password = ss.SafeRemainingArgument;
		var authentication = gameworld.ComputerMailService.Authenticate(host, address, password);
		if (!authentication.Success || authentication.Account is null)
		{
			return ($"{authentication.ErrorMessage}\n\n{RenderPrompt(session.User, application, state, null, null)}", false);
		}

		state.LoggedInAccountId = authentication.Account.Id;
		state.LoggedInAddress = authentication.Account.Address;
		state.ClearCompose();
		return ($"You log in to {authentication.Account.Address.ColourName()}.\n\n{RenderPrompt(session.User, application, state, authentication.Account, null)}", false);
	}

	private static (string Output, bool Exit) HandleLogout(IComputerTerminalSession session,
		IComputerBuiltInApplication application, MailState state, IComputerMailAccount? account)
	{
		if (account is null)
		{
			return ($"You are not currently logged in to any mailbox.\n\n{RenderPrompt(session.User, application, state, null, null)}", false);
		}

		var address = account.Address;
		state.LoggedInAccountId = null;
		state.LoggedInAddress = string.Empty;
		state.ClearCompose();
		return ($"You log out of {address.ColourName()}.\n\n{RenderPrompt(session.User, application, state, null, null)}", false);
	}

	private static (string Output, bool Exit) HandleInbox(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerBuiltInApplication application, IComputerHost host, MailState state, IComputerMailAccount? account)
	{
		if (account is null)
		{
			return ($"You must log in before you can read mail.\n\n{RenderPrompt(session.User, application, state, null, null)}", false);
		}

		var headers = gameworld.ComputerMailService.GetMailboxHeaders(host, account).ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"{account.Address.ColourName()} mailbox:");
		if (!headers.Any())
		{
			sb.AppendLine("There are no messages in this mailbox.");
			sb.AppendLine();
			sb.Append(RenderPrompt(session.User, application, state, account, null));
			return (sb.ToString(), false);
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			headers.Select(header => new List<string>
			{
				header.MailboxEntryId.ToString("N0", session.User),
				header.IsSentFolder ? "Sent" : "Inbox",
				header.SenderAddress,
				header.RecipientAddress,
				header.Subject,
				header.DeliveredAtUtc.ToString(session.User),
				header.IsRead ? "Read" : "Unread"
			}),
			new List<string>
			{
				"Id",
				"Folder",
				"From",
				"To",
				"Subject",
				"Delivered",
				"State"
			},
			session.User.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			session.User.Account.UseUnicode));
		sb.AppendLine();
		sb.Append(RenderPrompt(session.User, application, state, account, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleRead(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerBuiltInApplication application, IComputerHost host, MailState state, IComputerMailAccount? account,
		StringStack ss)
	{
		if (account is null)
		{
			return ($"You must log in before you can read mail.\n\n{RenderPrompt(session.User, application, state, null, null)}", false);
		}

		if (ss.IsFinished || !long.TryParse(ss.PopSpeech(), out var mailboxEntryId))
		{
			return ($"Which mailbox entry do you want to read?\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
		}

		var message = gameworld.ComputerMailService.ReadMessage(host, account, mailboxEntryId, out var error);
		if (message is null)
		{
			return ($"{error}\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Message {message.Header.MailboxEntryId.ToString("N0", session.User).ColourValue()}");
		sb.AppendLine($"Folder: {(message.Header.IsSentFolder ? "Sent" : "Inbox").ColourValue()}");
		sb.AppendLine($"From: {message.Header.SenderAddress.ColourName()}");
		sb.AppendLine($"To: {message.Header.RecipientAddress.ColourName()}");
		sb.AppendLine($"Sent: {message.Header.SentAtUtc.ToString(session.User).ColourValue()}");
		sb.AppendLine($"Subject: {message.Header.Subject.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine(message.Body);
		sb.AppendLine();
		sb.Append(RenderPrompt(session.User, application, state, account, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleDelete(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerBuiltInApplication application, IComputerHost host, MailState state, IComputerMailAccount? account,
		StringStack ss)
	{
		if (account is null)
		{
			return ($"You must log in before you can delete mail.\n\n{RenderPrompt(session.User, application, state, null, null)}", false);
		}

		if (ss.IsFinished || !long.TryParse(ss.PopSpeech(), out var mailboxEntryId))
		{
			return ($"Which mailbox entry do you want to delete?\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
		}

		if (!gameworld.ComputerMailService.DeleteMessage(host, account, mailboxEntryId, out var error))
		{
			return ($"{error}\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
		}

		return ($"Deleted mailbox entry {mailboxEntryId.ToString("N0", session.User).ColourValue()}.\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
	}

	private static (string Output, bool Exit) HandleSend(IComputerTerminalSession session,
		IComputerBuiltInApplication application, MailState state, IComputerMailAccount? account, StringStack ss)
	{
		if (account is null)
		{
			return ($"You must log in before you can compose mail.\n\n{RenderPrompt(session.User, application, state, null, null)}", false);
		}

		if (ss.IsFinished)
		{
			return ($"Who do you want to send mail to?\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
		}

		state.ComposeRecipient = ss.PopSpeech().Trim().ToLowerInvariant();
		state.ComposeSubject = string.Empty;
		state.ComposeBody = string.Empty;
		return ($"Composing a new message to {state.ComposeRecipient.ColourName()}. Set a subject with {"type subject <text>".ColourCommand()}, edit the body with {"type body".ColourCommand()}, and send it with {"type post".ColourCommand()}.\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
	}

	private static (string Output, bool Exit) HandleSubject(IComputerTerminalSession session,
		IComputerBuiltInApplication application, MailState state, IComputerMailAccount? account, StringStack ss)
	{
		if (account is null)
		{
			return ($"You must log in before you can compose mail.\n\n{RenderPrompt(session.User, application, state, null, null)}", false);
		}

		if (ss.IsFinished)
		{
			return ($"What subject do you want to use?\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
		}

		state.ComposeSubject = ss.SafeRemainingArgument.Trim();
		return ($"Set the subject to {state.ComposeSubject.ColourCommand()}.\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
	}

	private static (string Output, bool Exit) HandleBody(IComputerTerminalSession session,
		IComputerBuiltInApplication application, ComputerRuntimeProcess process, MailState state, IComputerMailAccount? account)
	{
		if (account is null)
		{
			return ($"You must log in before you can compose mail.\n\n{RenderPrompt(session.User, application, state, null, null)}", false);
		}

		if (string.IsNullOrWhiteSpace(state.ComposeRecipient))
		{
			return ($"Begin a draft first with {"type send <user@domain>".ColourCommand()}.\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
		}

		var recipient = state.ComposeRecipient;
		session.User.EditorMode(
			(text, handler, _) =>
			{
				state.ComposeBody = text;
				SaveState(process, state);
				handler.Send($"Updated the body of your draft to {recipient.ColourName()}. Continue with {"type post".ColourCommand()} when you are ready to send it.");
			},
			(handler, _) =>
			{
				handler.Send($"You leave the draft body for {recipient.ColourName()} unchanged.");
			},
			1.0,
			state.ComposeBody,
			EditorOptions.PermitEmpty);
		return (string.Empty, false);
	}

	private static (string Output, bool Exit) HandlePost(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerBuiltInApplication application, IComputerHost host, MailState state, IComputerMailAccount? account)
	{
		if (account is null)
		{
			return ($"You must log in before you can send mail.\n\n{RenderPrompt(session.User, application, state, null, null)}", false);
		}

		if (string.IsNullOrWhiteSpace(state.ComposeRecipient))
		{
			return ($"Begin a draft first with {"type send <user@domain>".ColourCommand()}.\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
		}

		if (!gameworld.ComputerMailService.SendMessage(
			    host,
			    account,
			    state.ComposeRecipient,
			    state.ComposeSubject,
			    state.ComposeBody,
			    out var error))
		{
			return ($"{error}\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
		}

		var recipient = state.ComposeRecipient;
		state.ClearCompose();
		return ($"Sent mail to {recipient.ColourName()}.\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
	}

	private static (string Output, bool Exit) HandleCancel(IComputerTerminalSession session,
		IComputerBuiltInApplication application, MailState state, IComputerMailAccount? account)
	{
		if (string.IsNullOrWhiteSpace(state.ComposeRecipient) &&
		    string.IsNullOrWhiteSpace(state.ComposeSubject) &&
		    string.IsNullOrWhiteSpace(state.ComposeBody))
		{
			return ($"There is no active draft to cancel.\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
		}

		state.ClearCompose();
		return ($"Cancelled the current draft.\n\n{RenderPrompt(session.User, application, state, account, null)}", false);
	}

	private static void SendOverview(IComputerTerminalSession session, IComputerBuiltInApplication application,
		ComputerRuntimeProcess process, MailState state, IComputerMailAccount? account, string? warning)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{application.Name.ColourName()} :: {process.Host.Name.ColourName()}");
		if (!string.IsNullOrEmpty(warning))
		{
			sb.AppendLine(warning);
			sb.AppendLine();
		}

		sb.AppendLine(RenderHelp(session.User, application, state, account));
		session.User.OutputHandler.Send(sb.ToString(), nopage: true);
	}

	private static string RenderHelp(ICharacter user, IComputerBuiltInApplication application, MailState state,
		IComputerMailAccount? account)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{application.Name.ColourName()} commands:");
		sb.AppendLine($"\t{"login <user@domain> <password>".ColourCommand()} - log in to a reachable mailbox");
		sb.AppendLine($"\t{"logout".ColourCommand()} - log out of the current mailbox");
		sb.AppendLine($"\t{"inbox".ColourCommand()} - list inbox and sent messages");
		sb.AppendLine($"\t{"read <id>".ColourCommand()} - read one mailbox entry");
		sb.AppendLine($"\t{"delete <id>".ColourCommand()} - delete one mailbox entry");
		sb.AppendLine($"\t{"send <user@domain>".ColourCommand()} - begin a new draft");
		sb.AppendLine($"\t{"subject <text>".ColourCommand()} - set the current draft subject");
		sb.AppendLine($"\t{"body".ColourCommand()} - edit the current draft body");
		sb.AppendLine($"\t{"post".ColourCommand()} - send the current draft");
		sb.AppendLine($"\t{"cancel".ColourCommand()} - cancel the current draft");
		sb.AppendLine($"\t{"help".ColourCommand()} - show this help");
		sb.AppendLine($"\t{"exit".ColourCommand()} - close Mail");
		sb.AppendLine();
		sb.Append(RenderPrompt(user, application, state, account, null));
		return sb.ToString();
	}

	private static string RenderPrompt(ICharacter user, IComputerBuiltInApplication application, MailState state,
		IComputerMailAccount? account, string? warning)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(warning))
		{
			sb.AppendLine(warning);
		}

		if (account is null)
		{
			sb.Append($"Use {"type login <user@domain> <password>".ColourCommand()} to authenticate with a reachable mail domain.");
			return sb.ToString();
		}

		sb.Append($"Logged in as {account.Address.ColourName()}.");
		if (!string.IsNullOrWhiteSpace(state.ComposeRecipient))
		{
			sb.Append(
				$" Draft -> {state.ComposeRecipient.ColourName()}, subject {(string.IsNullOrWhiteSpace(state.ComposeSubject) ? "unset".ColourError() : state.ComposeSubject.ColourCommand())}, body {(string.IsNullOrWhiteSpace(state.ComposeBody) ? "empty".ColourError() : "set".ColourValue())}.");
		}
		else
		{
			sb.Append($" Begin a draft with {"type send <user@domain>".ColourCommand()}.");
		}

		return sb.ToString();
	}

	private static MailState LoadState(string? stateJson)
	{
		if (!string.IsNullOrWhiteSpace(stateJson))
		{
			try
			{
				var state = JsonSerializer.Deserialize<MailState>(stateJson);
				if (state is not null)
				{
					return state;
				}
			}
			catch (JsonException)
			{
			}
		}

		return new MailState();
	}

	private static IComputerMailAccount? ResolveLoggedInAccount(IFuturemud gameworld, IComputerHost host, MailState state,
		out string? warning)
	{
		warning = null;
		if (state.LoggedInAccountId is not > 0L)
		{
			return null;
		}

		var account = gameworld.ComputerMailService.GetAccount(host, state.LoggedInAccountId.Value, out var error);
		if (account is not null)
		{
			state.LoggedInAddress = account.Address;
			return account;
		}

		warning = error;
		state.LoggedInAccountId = null;
		state.LoggedInAddress = string.Empty;
		state.ClearCompose();
		return null;
	}

	private static ComputerProgramExecutionOutcome WaitForInput(IComputerTerminalSession session, MailState state)
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

	private static void SaveState(ComputerRuntimeProcess process, MailState state)
	{
		process.StateJson = JsonSerializer.Serialize(state);
		process.LastUpdatedAtUtc = DateTime.UtcNow;
		if (process.Host is IComputerMutableOwner mutableOwner)
		{
			mutableOwner.SaveProcessDefinition(process);
		}
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
			SendOverview(gameworld, session, application, process.Host);
			return WaitForInput(session);
		}

		var response = HandleCommand(gameworld, session, application, process.Host, input!);
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

	private static (string Output, bool Exit) HandleCommand(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerBuiltInApplication application,
		IComputerHost host,
		string input)
	{
		var ss = new StringStack(input.Trim());
		var command = ss.PopSpeech().ToLowerInvariant();
		return command switch
		{
			"" or "summary" or "host" => (RenderSummary(gameworld, session.User, application, session, host), false),
			"help" => (RenderHelp(application), false),
			"services" or "apps" when ss.IsFinished => (RenderLocalServices(gameworld, session.User, session, host), false),
			"services" or "apps" => HandleRemoteServices(gameworld, session.User, session, host, ss.SafeRemainingArgument),
			"storage" or "drives" => (RenderStorage(session.User, host), false),
			"terminals" or "sessions" => (RenderTerminals(session.User, host), false),
			"adapters" or "network" => (RenderAdapters(session.User, host), false),
			"routes" => (RenderRoutes(session.User, session, host), false),
			"gateways" => (RenderGateways(gameworld, session), false),
			"tunnel" => HandleTunnel(gameworld, session, ss),
			"hosts" => (RenderReachableHosts(gameworld, session.User, session, host), false),
			"show" => HandleShowRemoteHost(gameworld, session.User, session, host, ss),
			"exit" or "quit" => ($"{application.Name.ColourName()} closing.", true),
			_ => ($"That is not a valid {application.Name.ColourName()} command.\n\n{RenderPrompt()}", false)
		};
	}

	private static void SendOverview(IFuturemud gameworld, IComputerTerminalSession session, IComputerBuiltInApplication application,
		IComputerHost host)
	{
		var sb = new StringBuilder();
		sb.AppendLine(RenderSummary(gameworld, session.User, application, session, host));
		sb.AppendLine();
		sb.Append(RenderHelp(application));
		session.User.OutputHandler.Send(sb.ToString(), nopage: true);
	}

	private static string RenderSummary(IFuturemud gameworld, ICharacter user, IComputerBuiltInApplication application,
		IComputerTerminalSession session, IComputerHost host)
	{
		var computerService = gameworld.ComputerExecutionService;
		var reachableHosts = computerService is null
			? []
			: computerService.GetReachableHosts(host, session).ToList();
		var gateways = gameworld.ComputerNetworkTunnelService.GetReachableVpnGateways(session).ToList();
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
		sb.AppendLine(
			$"Active Tunnels: {session.ActiveTunnels.Count.ToString("N0", user).ColourValue()}");
		sb.AppendLine(
			$"Reachable VPN Gateways: {gateways.Count.ToString("N0", user).ColourValue()}");
		sb.AppendLine(
			$"Reachable Hosts: {reachableHosts.Count.ToString("N0", user).ColourValue()}");
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
		sb.AppendLine($"\t{"services <host>".ColourCommand()} - list advertised network services for a reachable host");
		sb.AppendLine($"\t{"storage".ColourCommand()} - list mounted storage devices");
		sb.AppendLine($"\t{"terminals".ColourCommand()} - list connected terminals and sessions");
		sb.AppendLine($"\t{"adapters".ColourCommand()} - list local network adapters");
		sb.AppendLine($"\t{"routes".ColourCommand()} - show hardware and active tunnel routes for this session");
		sb.AppendLine($"\t{"gateways".ColourCommand()} - list reachable VPN gateways");
		sb.AppendLine($"\t{"tunnel connect <host> <user@domain> <password> [vpn]".ColourCommand()} - authenticate to a reachable VPN gateway");
		sb.AppendLine($"\t{"tunnel disconnect [vpn|all]".ColourCommand()} - close one or all active tunnels");
		sb.AppendLine($"\t{"hosts".ColourCommand()} - list reachable hosts on the telecom-backed network");
		sb.AppendLine($"\t{"show <host>".ColourCommand()} - show a reachable host summary");
		sb.AppendLine($"\t{"help".ColourCommand()} - show this help");
		sb.AppendLine($"\t{"exit".ColourCommand()} - close Directory");
		sb.AppendLine();
		sb.AppendLine("Directory is discovery-only in this slice. It can inspect the local host and discover reachable remote hosts, but it does not yet launch remote services.");
		sb.AppendLine();
		sb.Append(RenderPrompt());
		return sb.ToString();
	}

	private static string RenderLocalServices(IFuturemud gameworld, ICharacter user, IComputerTerminalSession session,
		IComputerHost host)
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

		var advertisedServices = gameworld.ComputerExecutionService?.GetAdvertisedServices(host, host, session)
			.ToDictionary(x => x.ApplicationId, StringComparer.InvariantCultureIgnoreCase) ??
			new Dictionary<string, ComputerNetworkServiceSummary>(StringComparer.InvariantCultureIgnoreCase);
		sb.AppendLine(StringUtilities.GetTextTable(
			applications.Select(application => new List<string>
			{
				application.Name,
				application.IsNetworkService
					? host.IsNetworkServiceEnabled(application.ApplicationId) ? "network-enabled" : "network-disabled"
					: "local",
				application.Summary,
				advertisedServices.TryGetValue(application.ApplicationId, out var summary)
					? summary.ServiceDetails.Any()
						? summary.ServiceDetails.Select(x => x.ColourName()).ListToString()
						: "-"
					: "-"
			}),
			new List<string>
			{
				"Service",
				"Scope",
				"Summary",
				"Details"
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

	private static string RenderRoutes(ICharacter user, IComputerTerminalSession session, IComputerHost host)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Effective Session Routes:");
		var baseRoutes = host.NetworkAdapters
			.SelectMany(x => x.NetworkRouteKeys)
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.OrderBy(x => x)
			.ToList();
		sb.AppendLine($"Hardware Routes: {(baseRoutes.Any() ? ComputerNetworkRoutingUtilities.DescribeRoutes(baseRoutes).ColourValue() : "None".ColourError())}");
		sb.AppendLine($"Active Tunnel Routes: {(session.ActiveRouteKeys.Any() ? ComputerNetworkRoutingUtilities.DescribeRoutes(session.ActiveRouteKeys).ColourValue() : "None".ColourError())}");
		if (session.ActiveTunnels.Any())
		{
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				session.ActiveTunnels.Select(tunnel => new List<string>
				{
					tunnel.RouteDescription,
					tunnel.GatewayHostName,
					tunnel.AuthenticatedAddress,
					tunnel.ConnectedAtUtc.ToString(user)
				}),
				new List<string>
				{
					"Tunnel",
					"Gateway",
					"Identity",
					"Connected"
				},
				user.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				user.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.Append(RenderPrompt());
		return sb.ToString();
	}

	private static string RenderGateways(IFuturemud gameworld, IComputerTerminalSession session)
	{
		var user = session.User;
		var gateways = gameworld.ComputerNetworkTunnelService.GetReachableVpnGateways(session).ToList();
		var sb = new StringBuilder();
		sb.AppendLine("Reachable VPN Gateways:");
		if (!gateways.Any())
		{
			sb.AppendLine("No reachable VPN gateways are currently available from this session.");
			sb.AppendLine();
			sb.Append(RenderPrompt());
			return sb.ToString();
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			gateways.Select(gateway => new List<string>
			{
				gateway.Host.Name,
				gateway.CanonicalAddress,
				gateway.DeviceIdentifier,
				ComputerNetworkRoutingUtilities.DescribeRoutes(gateway.SharedRouteKeys),
				gateway.VpnNetworkIds.Select(x => x.ColourName()).ListToString()
			}),
			new List<string>
			{
				"Gateway",
				"Address",
				"Device",
				"Access",
				"VPNs"
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

	private static (string Output, bool Exit) HandleTunnel(IFuturemud gameworld, IComputerTerminalSession session,
		StringStack ss)
	{
		if (ss.IsFinished)
		{
			return (RenderRoutes(session.User, session, session.Host), false);
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "connect":
			case "open":
				if (ss.IsFinished)
				{
					return ($"Which reachable VPN gateway do you want to use?\n\n{RenderPrompt()}", false);
				}

				var hostIdentifier = ss.PopSpeech();
				if (ss.IsFinished)
				{
					return ($"Which network identity should authenticate to that VPN gateway?\n\n{RenderPrompt()}", false);
				}

				var address = ss.PopSpeech();
				if (ss.IsFinished)
				{
					return ($"What password should be used for {address.ColourName()}?\n\n{RenderPrompt()}", false);
				}

				var password = ss.PopSpeech();
				var requestedVpn = ss.IsFinished ? null : ss.SafeRemainingArgument;
				if (!gameworld.ComputerNetworkTunnelService.TryOpenTunnel(session, hostIdentifier, address, password,
					    requestedVpn, out var connectError))
				{
					return ($"{connectError}\n\n{RenderPrompt()}", false);
				}

				var tunnelName = string.IsNullOrWhiteSpace(requestedVpn)
					? "the selected VPN tunnel"
					: ComputerNetworkRoutingUtilities.DescribeRouteKey(
						ComputerNetworkRoutingUtilities.GetVpnRouteKey(requestedVpn));
				return ($"You authenticate as {address.ColourName()} and open {tunnelName.ColourValue()} through {hostIdentifier.ColourName()}.\n\n{RenderPrompt()}",
					false);

			case "disconnect":
			case "close":
				var routeIdentifier = ss.IsFinished ? null : ss.SafeRemainingArgument;
				if (!gameworld.ComputerNetworkTunnelService.TryCloseTunnel(session, routeIdentifier, out var disconnectError))
				{
					return ($"{disconnectError}\n\n{RenderPrompt()}", false);
				}

				return ((string.IsNullOrWhiteSpace(routeIdentifier) || routeIdentifier.EqualTo("all")
						? "All active VPN tunnels on this session are now closed."
						: $"The tunnel {routeIdentifier.ColourCommand()} is now closed.") +
					$"\n\n{RenderPrompt()}",
					false);

			default:
				return ($"Use {"tunnel connect <host> <user@domain> <password> [vpn]".ColourCommand()} or {"tunnel disconnect [vpn|all]".ColourCommand()}.\n\n{RenderPrompt()}",
					false);
		}
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
				DescribeAdapter(user, adapter),
				adapter.DeviceIdentifier,
				adapter.NetworkReady.ToColouredString(),
				DescribeGrid(user, adapter.TelecommunicationsGrid),
				adapter.NetworkAddress ?? "-",
				ComputerNetworkRoutingUtilities.DescribeRoutes(adapter.NetworkRouteKeys)
			}),
			new List<string>
			{
				"Adapter",
				"Device",
				"Ready",
				"Grid",
				"Address",
				"Access"
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
		return $"Use {"type <command>".ColourCommand()} to browse the local and network directory.";
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

	private static string DescribeAdapter(ICharacter user, INetworkAdapter adapter)
	{
		return adapter is IGameItemComponent component
			? component.Parent.HowSeen(user, true)
			: adapter.NetworkAddress ?? "Network Adapter";
	}

	private static string DescribeGrid(ICharacter user, ITelecommunicationsGrid? grid)
	{
		return grid is null
			? "None"
			: $"#{grid.Id.ToString("N0", user)} ({grid.Prefix})";
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

	private static string RenderReachableHosts(IFuturemud gameworld, ICharacter user, IComputerTerminalSession session,
		IComputerHost host)
	{
		var computerService = gameworld.ComputerExecutionService;
		var hosts = computerService is null
			? []
			: computerService.GetReachableHosts(host, session).ToList();
		var sb = new StringBuilder();
		sb.AppendLine("Reachable Hosts:");
		if (!hosts.Any())
		{
			sb.AppendLine("No reachable hosts are currently available on the telecom-backed network from this host.");
			sb.AppendLine();
			sb.Append(RenderPrompt());
			return sb.ToString();
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			hosts.Select(summary => new List<string>
			{
				summary.Host.Name,
				summary.CanonicalAddress,
				summary.DeviceIdentifier,
				DescribeGrid(user, summary.Grid),
				summary.IsLocalGrid ? "local" : "linked",
				summary.AccessDescription,
				summary.AdvertisedServiceCount.ToString("N0", user)
			}),
			new List<string>
			{
				"Host",
				"Address",
				"Device",
				"Grid",
				"Scope",
				"Access",
				"Services"
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

	private static (string Output, bool Exit) HandleShowRemoteHost(IFuturemud gameworld, ICharacter user,
		IComputerTerminalSession session, IComputerHost host,
		StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ("Which reachable host do you want to inspect?\n\n" + RenderPrompt(), false);
		}

		if (!TryResolveReachableHost(gameworld, session, host, ss.SafeRemainingArgument, out var summary, out var error))
		{
			return ($"{error}\n\n{RenderPrompt()}", false);
		}

		var services = gameworld.ComputerExecutionService is null
			? []
			: gameworld.ComputerExecutionService.GetAdvertisedServices(host, summary!.Host, session).ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"{summary.Host.Name.ColourName()} :: {summary.CanonicalAddress.ColourCommand()}");
		sb.AppendLine($"Device Id: {summary.DeviceIdentifier.ColourName()}");
		sb.AppendLine($"Scope: {(summary.IsLocalGrid ? "local" : "linked").ColourValue()} via {DescribeGrid(user, summary.Grid).ColourValue()}");
		sb.AppendLine($"Network Access: {summary.AccessDescription.ColourValue()}");
		sb.AppendLine($"Availability: {(summary.Available ? "reachable".ColourValue() : "offline".ColourError())}");
		sb.AppendLine($"Host Power: {summary.Host.Powered.ToColouredString()}");
		sb.AppendLine($"Advertised Services: {services.Count.ToString("N0", user).ColourValue()}");
		if (!services.Any())
		{
			sb.AppendLine($"{summary.Host.Name.ColourName()} is reachable but does not currently advertise any implemented network services.");
		}

		sb.AppendLine();
		sb.Append(RenderPrompt());
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleRemoteServices(IFuturemud gameworld, ICharacter user,
		IComputerTerminalSession session, IComputerHost host,
		string identifier)
	{
		if (!TryResolveReachableHost(gameworld, session, host, identifier, out var summary, out var error))
		{
			return ($"{error}\n\n{RenderPrompt()}", false);
		}

		var services = gameworld.ComputerExecutionService is null
			? []
			: gameworld.ComputerExecutionService.GetAdvertisedServices(host, summary!.Host, session).ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"Advertised Services for {summary.Host.Name.ColourName()} ({summary.CanonicalAddress.ColourCommand()}):");
		sb.AppendLine($"Device Id: {summary.DeviceIdentifier.ColourName()}");
		sb.AppendLine($"Network Access: {summary.AccessDescription.ColourValue()}");
		if (!services.Any())
		{
			sb.AppendLine($"{summary.Host.Name.ColourName()} does not currently advertise any implemented network services.");
			sb.AppendLine();
			sb.Append(RenderPrompt());
			return (sb.ToString(), false);
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			services.Select(service => new List<string>
			{
				service.Name,
				service.ApplicationId,
				service.Summary,
				service.ServiceDetails.Any()
					? service.ServiceDetails.Select(x => x.ColourName()).ListToString()
					: "-"
			}),
			new List<string>
			{
				"Service",
				"Id",
				"Summary",
				"Details"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
		sb.AppendLine();
		sb.Append(RenderPrompt());
		return (sb.ToString(), false);
	}

	private static bool TryResolveReachableHost(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost sourceHost, string identifier,
		out ComputerNetworkHostSummary? summary, out string error)
	{
		summary = null;
		var computerService = gameworld.ComputerExecutionService;
		var hosts = computerService is null
			? []
			: computerService.GetReachableHosts(sourceHost, session).ToList();
		if (!hosts.Any())
		{
			error = "No reachable hosts are currently available on the telecom-backed network from this host.";
			return false;
		}

		var exactAddress = hosts
			.Where(x => x.CanonicalAddress.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (exactAddress.Count == 1)
		{
			summary = exactAddress.Single();
			error = string.Empty;
			return true;
		}

		if (exactAddress.Count > 1)
		{
			error = $"More than one reachable host matches {identifier.ColourCommand()} by address.";
			return false;
		}

		var exactDevice = hosts
			.Where(x => x.DeviceIdentifier.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (exactDevice.Count == 1)
		{
			summary = exactDevice.Single();
			error = string.Empty;
			return true;
		}

		if (exactDevice.Count > 1)
		{
			error = $"More than one reachable host matches {identifier.ColourCommand()} by device identifier.";
			return false;
		}

		var exactHost = hosts
			.Where(x => x.Host.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (exactHost.Count == 1)
		{
			summary = exactHost.Single();
			error = string.Empty;
			return true;
		}

		if (exactHost.Count > 1)
		{
			error = $"More than one reachable host matches {identifier.ColourCommand()} by name. Use its canonical network address instead: {exactHost.Select(x => x.CanonicalAddress.ColourCommand()).ListToString()}.";
			return false;
		}

		var partial = hosts
			.Where(x => x.CanonicalAddress.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase) ||
			            x.DeviceIdentifier.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase) ||
			            x.Host.Name.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (partial.Count == 1)
		{
			summary = partial.Single();
			error = string.Empty;
			return true;
		}

		error = partial.Count > 1
			? $"More than one reachable host matches {identifier.ColourCommand()}. Use a more specific name, device id, or one of these canonical addresses: {partial.Select(x => x.CanonicalAddress.ColourCommand()).ListToString()}."
			: $"There is no reachable host matching {identifier.ColourCommand()}.";
		return false;
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
			$"Active Tunnels: {session.ActiveTunnels.Count.ToString("N0", session.User).ColourValue()}");
		sb.AppendLine(
			$"Built-In Applications: {host.BuiltInApplications.Select(x => x.Name.ColourName()).ListToString()}");

		AppendStorageSection(sb, session.User, host);
		AppendTerminalSection(sb, session.User, host);
		AppendNetworkSection(sb, session.User, session, host);
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

	private static void AppendNetworkSection(StringBuilder sb, ICharacter user, IComputerTerminalSession session,
		IComputerHost host)
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
				adapter.DeviceIdentifier,
				adapter.NetworkReady.ToColouredString(),
				DescribeGrid(user, adapter.TelecommunicationsGrid),
				adapter.NetworkAddress ?? "-",
				ComputerNetworkRoutingUtilities.DescribeRoutes(adapter.NetworkRouteKeys)
			}),
			new List<string>
			{
				"Adapter",
				"Device",
				"Ready",
				"Grid",
				"Address",
				"Access"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));

		sb.AppendLine();
		sb.AppendLine($"Hosted VPN Networks: {(host.HostedVpnNetworkIds.Any() ? host.HostedVpnNetworkIds.Select(x => x.ColourName()).ListToString() : "None".ColourError())}");
		sb.AppendLine($"Active Tunnel Routes: {(session.ActiveRouteKeys.Any() ? ComputerNetworkRoutingUtilities.DescribeRoutes(session.ActiveRouteKeys).ColourValue() : "None".ColourError())}");
		if (session.ActiveTunnels.Any())
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				session.ActiveTunnels.Select(tunnel => new List<string>
				{
					tunnel.RouteDescription,
					tunnel.GatewayHostName,
					tunnel.AuthenticatedAddress,
					tunnel.ConnectedAtUtc.ToString(user)
				}),
				new List<string>
				{
					"Tunnel",
					"Gateway",
					"Identity",
					"Connected"
				},
				user.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				user.Account.UseUnicode));
		}
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

	private static string DescribeGrid(ICharacter user, ITelecommunicationsGrid? grid)
	{
		return grid is null
			? "None"
			: $"#{grid.Id.ToString("N0", user)} ({grid.Prefix})";
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
