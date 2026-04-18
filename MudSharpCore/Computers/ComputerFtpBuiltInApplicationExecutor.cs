#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Computers;

internal sealed class FtpBuiltInApplicationExecutor : IComputerBuiltInApplicationExecutor
{
	private sealed class FtpState
	{
		public long? RemoteHostItemId { get; set; }
		public string RemoteHostName { get; set; } = string.Empty;
		public string RemoteAccountUserName { get; set; } = string.Empty;
		public string SelectedRemoteOwnerIdentifier { get; set; } = "host";

		public void ClearAuthentication()
		{
			RemoteAccountUserName = string.Empty;
			SelectedRemoteOwnerIdentifier = "host";
		}

		public void ClearConnection()
		{
			RemoteHostItemId = null;
			RemoteHostName = string.Empty;
			ClearAuthentication();
		}
	}

	public string ApplicationId => "ftp";

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
		var remoteHost = ResolveConnectedHost(gameworld, session.Host, state, out var warning);
		var account = ResolveLoggedInAccount(gameworld, session.Host, remoteHost, state, out var accountWarning);
		var input = ComputerExecutionContextScope.Current?.ConsumePendingTerminalInput();
		if (string.IsNullOrWhiteSpace(input))
		{
			SendOverview(gameworld, session, application, state, remoteHost, account, warning ?? accountWarning);
			return WaitForInput(session, state);
		}

		var response = HandleCommand(gameworld, session, application, state, remoteHost, account, input!);
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
		IComputerBuiltInApplication application, FtpState state, IComputerHost? remoteHost, IComputerFtpAccount? account,
		string input)
	{
		var ss = new StringStack(input.Trim());
		var command = ss.PopSpeech().ToLowerInvariant();
		return command switch
		{
			"" => (RenderPrompt(session.User, state, remoteHost, account, null), false),
			"help" => (RenderHelp(application, state, remoteHost, account), false),
			"hosts" => (RenderReachableHosts(gameworld, session.User, session.Host, state, remoteHost, account), false),
			"open" => HandleOpen(gameworld, session, application, state, ss),
			"close" or "disconnect" => HandleClose(session, application, state, remoteHost),
			"status" => (RenderStatus(session.User, state, remoteHost, account, null), false),
			"owners" or "targets" => HandleOwners(gameworld, session, state, remoteHost, account),
			"use" or "owner" => HandleUse(gameworld, session, state, remoteHost, account, ss),
			"login" => HandleLogin(gameworld, session, application, state, remoteHost, ss),
			"logout" => HandleLogout(session, application, state, remoteHost, account),
			"list" or "ls" or "dir" => HandleList(gameworld, session, state, remoteHost, account),
			"show" or "read" or "cat" => HandleShow(gameworld, session, state, remoteHost, account, ss),
			"get" => HandleGet(gameworld, session, state, remoteHost, account, ss),
			"put" => HandlePut(gameworld, session, state, remoteHost, account, ss),
			"delete" or "del" or "rm" => HandleDelete(gameworld, session, state, remoteHost, account, ss),
			"exit" or "quit" => ($"{application.Name.ColourName()} closing.", true),
			_ => ($"That is not a valid {application.Name.ColourName()} command.\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}", false)
		};
	}

	private static (string Output, bool Exit) HandleOpen(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerBuiltInApplication application, FtpState state, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ($"Which reachable host do you want to open?\n\n{RenderPrompt(session.User, state, null, null, null)}",
				false);
		}

		var identifier = ss.SafeRemainingArgument;
		var summary = ResolveFtpHost(gameworld, session.Host, identifier);
		if (summary is null)
		{
			return ($"There is no reachable host advertising FTP that matches {identifier.ColourName()}.\n\n{RenderPrompt(session.User, state, null, null, null)}",
				false);
		}

		state.RemoteHostItemId = summary.Host.OwnerHostItemId;
		state.RemoteHostName = summary.Host.Name;
		state.ClearAuthentication();
		return ($"You open an FTP session to {summary.Host.Name.ColourName()} ({summary.CanonicalAddress.ColourName()}).\n\n{RenderPrompt(session.User, state, summary.Host, null, null)}",
			false);
	}

	private static (string Output, bool Exit) HandleClose(IComputerTerminalSession session, IComputerBuiltInApplication application,
		FtpState state, IComputerHost? remoteHost)
	{
		if (remoteHost is null)
		{
			return ($"You do not currently have any remote FTP connection open.\n\n{RenderPrompt(session.User, state, null, null, null)}",
				false);
		}

		var hostName = remoteHost.Name;
		state.ClearConnection();
		return ($"You close the FTP session to {hostName.ColourName()}.\n\n{RenderPrompt(session.User, state, null, null, null)}",
			false);
	}

	private static (string Output, bool Exit) HandleOwners(IFuturemud gameworld, IComputerTerminalSession session,
		FtpState state, IComputerHost? remoteHost, IComputerFtpAccount? account)
	{
		if (remoteHost is null)
		{
			return ($"You must open a remote FTP host first.\n\n{RenderPrompt(session.User, state, null, null, null)}",
				false);
		}

		var owners = gameworld.ComputerFileTransferService.GetAccessibleOwners(session.Host, remoteHost, account, out var error)
			.ToList();
		if (!string.IsNullOrEmpty(error))
		{
			return ($"{error}\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}", false);
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Remote Owners on {remoteHost.Name.ColourName()}:");
		if (!owners.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				owners.Select(x => new List<string>
				{
					x.DisplayName,
					x.OwnerIdentifier,
					x.AnonymousAccessible.ToColouredString(),
					x.OwnerIdentifier.EqualTo(state.SelectedRemoteOwnerIdentifier) ? "selected".ColourValue() : "-"
				}),
				new List<string>
				{
					"Owner",
					"Selector",
					"Public",
					"State"
				},
				session.User.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				session.User.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.Append(RenderPrompt(session.User, state, remoteHost, account, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleUse(IFuturemud gameworld, IComputerTerminalSession session, FtpState state,
		IComputerHost? remoteHost, IComputerFtpAccount? account, StringStack ss)
	{
		if (remoteHost is null)
		{
			return ($"You must open a remote FTP host first.\n\n{RenderPrompt(session.User, state, null, null, null)}",
				false);
		}

		if (ss.IsFinished)
		{
			return ($"Which remote owner do you want to use?\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}",
				false);
		}

		var owner = ResolveAccessibleOwner(gameworld, session.Host, remoteHost, account, ss.SafeRemainingArgument, out var error);
		if (owner is null)
		{
			return ($"{error}\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}", false);
		}

		state.SelectedRemoteOwnerIdentifier = owner.OwnerIdentifier;
		return ($"Now managing remote files on {owner.DisplayName.ColourName()}.\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}",
			false);
	}

	private static (string Output, bool Exit) HandleLogin(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerBuiltInApplication application, FtpState state, IComputerHost? remoteHost, StringStack ss)
	{
		if (remoteHost is null)
		{
			return ($"You must open a remote FTP host before logging in.\n\n{RenderPrompt(session.User, state, null, null, null)}",
				false);
		}

		if (ss.IsFinished)
		{
			return ($"Which FTP user name do you want to log in as?\n\n{RenderPrompt(session.User, state, remoteHost, null, null)}",
				false);
		}

		var userName = ss.PopSpeech();
		if (ss.IsFinished)
		{
			return ($"What password do you want to use for {userName.ColourName()}?\n\n{RenderPrompt(session.User, state, remoteHost, null, null)}",
				false);
		}

		var authentication = gameworld.ComputerFileTransferService.Authenticate(session.Host, remoteHost, userName,
			ss.SafeRemainingArgument);
		if (!authentication.Success || authentication.Account is null)
		{
			return ($"{authentication.ErrorMessage}\n\n{RenderPrompt(session.User, state, remoteHost, null, null)}", false);
		}

		state.RemoteAccountUserName = authentication.Account.UserName;
		state.SelectedRemoteOwnerIdentifier = "host";
		return ($"You log in to FTP on {remoteHost.Name.ColourName()} as {authentication.Account.UserName.ColourName()}.\n\n{RenderPrompt(session.User, state, remoteHost, authentication.Account, null)}",
			false);
	}

	private static (string Output, bool Exit) HandleLogout(IComputerTerminalSession session, IComputerBuiltInApplication application,
		FtpState state, IComputerHost? remoteHost, IComputerFtpAccount? account)
	{
		if (remoteHost is null || account is null)
		{
			return ($"You are not currently logged in to any remote FTP account.\n\n{RenderPrompt(session.User, state, remoteHost, null, null)}",
				false);
		}

		var userName = account.UserName;
		state.ClearAuthentication();
		return ($"You log out of {userName.ColourName()} on {remoteHost.Name.ColourName()}.\n\n{RenderPrompt(session.User, state, remoteHost, null, null)}",
			false);
	}

	private static (string Output, bool Exit) HandleList(IFuturemud gameworld, IComputerTerminalSession session, FtpState state,
		IComputerHost? remoteHost, IComputerFtpAccount? account)
	{
		if (remoteHost is null)
		{
			return ($"You must open a remote FTP host first.\n\n{RenderPrompt(session.User, state, null, null, null)}",
				false);
		}

		var files = gameworld.ComputerFileTransferService.GetFiles(
			session.Host,
			remoteHost,
			account,
			state.SelectedRemoteOwnerIdentifier,
			out var error).ToList();
		if (!string.IsNullOrEmpty(error))
		{
			return ($"{error}\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}", false);
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Remote Files on {remoteHost.Name.ColourName()} :: {state.SelectedRemoteOwnerIdentifier.ColourName()}");
		if (!files.Any())
		{
			sb.AppendLine("Files: None");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				files.Select(file => new List<string>
				{
					file.FileName,
					file.SizeInBytes.ToString("N0", session.User),
					file.PubliclyAccessible.ToColouredString(),
					file.LastModifiedAtUtc.ToString(session.User)
				}),
				new List<string>
				{
					"File",
					"Size",
					"Public",
					"Modified"
				},
				session.User.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				session.User.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.Append(RenderPrompt(session.User, state, remoteHost, account, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleShow(IFuturemud gameworld, IComputerTerminalSession session, FtpState state,
		IComputerHost? remoteHost, IComputerFtpAccount? account, StringStack ss)
	{
		if (remoteHost is null)
		{
			return ($"You must open a remote FTP host first.\n\n{RenderPrompt(session.User, state, null, null, null)}",
				false);
		}

		if (ss.IsFinished)
		{
			return ($"Which remote file do you want to show?\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}",
				false);
		}

		var details = gameworld.ComputerFileTransferService.ReadFile(
			session.Host,
			remoteHost,
			account,
			state.SelectedRemoteOwnerIdentifier,
			ss.PopSpeech(),
			out var error);
		if (details is null)
		{
			return ($"{error}\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}", false);
		}

		var sb = new StringBuilder();
		sb.AppendLine($"{details.Summary.FileName.ColourName()} on {details.Summary.HostName.ColourName()}::{details.Summary.OwnerDisplayName.ColourName()}");
		sb.AppendLine($"Size: {details.Summary.SizeInBytes.ToString("N0", session.User).ColourValue()} bytes");
		sb.AppendLine($"Modified: {details.Summary.LastModifiedAtUtc.ToString(session.User).ColourValue()}");
		sb.AppendLine($"Access: {(details.Summary.ReadOnly ? "read-only".ColourError() : "writable".ColourValue())}");
		sb.AppendLine();
		sb.AppendLine(details.TextContents);
		sb.AppendLine();
		sb.Append(RenderPrompt(session.User, state, remoteHost, account, null));
		return (sb.ToString(), false);
	}

	private static (string Output, bool Exit) HandleGet(IFuturemud gameworld, IComputerTerminalSession session, FtpState state,
		IComputerHost? remoteHost, IComputerFtpAccount? account, StringStack ss)
	{
		if (remoteHost is null)
		{
			return ($"You must open a remote FTP host first.\n\n{RenderPrompt(session.User, state, null, null, null)}",
				false);
		}

		if (ss.IsFinished)
		{
			return ($"Which remote file do you want to copy down?\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}",
				false);
		}

		var remoteFileName = ss.PopSpeech();
		if (!ss.IsFinished && ss.PeekSpeech().EqualTo("to"))
		{
			ss.PopSpeech();
		}

		IComputerFileOwner targetOwner = session.CurrentOwner;
		if (!ss.IsFinished)
		{
			var target = ComputerFileTransferUtilities.ResolveSelectableOwner(session.Host, ss.SafeRemainingArgument, out var targetError);
			if (target is null)
			{
				return ($"{targetError ?? "That is not a valid local target."}\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}",
					false);
			}

			targetOwner = target;
		}

		var details = gameworld.ComputerFileTransferService.ReadFile(
			session.Host,
			remoteHost,
			account,
			state.SelectedRemoteOwnerIdentifier,
			remoteFileName,
			out var error);
		if (details is null)
		{
			return ($"{error}\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}", false);
		}

		if (targetOwner.FileSystem is null)
		{
			return ($"{ComputerFileTransferUtilities.DescribeOwner(targetOwner).ColourName()} does not expose a writable file system.\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}",
				false);
		}

		targetOwner.FileSystem.WriteFile(details.Summary.FileName, details.TextContents);
		targetOwner.FileSystem.SetFilePubliclyAccessible(details.Summary.FileName, false);
		return ($"Copied {details.Summary.FileName.ColourName()} from {remoteHost.Name.ColourName()} to {ComputerFileTransferUtilities.DescribeOwner(targetOwner).ColourName()}.\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}",
			false);
	}

	private static (string Output, bool Exit) HandlePut(IFuturemud gameworld, IComputerTerminalSession session, FtpState state,
		IComputerHost? remoteHost, IComputerFtpAccount? account, StringStack ss)
	{
		if (remoteHost is null)
		{
			return ($"You must open a remote FTP host first.\n\n{RenderPrompt(session.User, state, null, null, null)}",
				false);
		}

		if (account is null)
		{
			return ($"You must log in before you can upload files.\n\n{RenderPrompt(session.User, state, remoteHost, null, null)}",
				false);
		}

		if (ss.IsFinished)
		{
			return ($"Which local file do you want to upload?\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}",
				false);
		}

		var localFileName = ss.PopSpeech();
		var localFile = session.CurrentOwner.FileSystem?.GetFile(localFileName);
		if (localFile is null)
		{
			return ($"{ComputerFileTransferUtilities.DescribeOwner(session.CurrentOwner).ColourName()} does not have a file named {localFileName.ColourName()}.\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}",
				false);
		}

		var remoteFileName = ss.IsFinished ? localFileName : ss.SafeRemainingArgument;
		if (!gameworld.ComputerFileTransferService.WriteFile(
			    session.Host,
			    remoteHost,
			    account,
			    state.SelectedRemoteOwnerIdentifier,
			    remoteFileName,
			    localFile.TextContents,
			    out var error))
		{
			return ($"{error}\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}", false);
		}

		return ($"Uploaded {localFileName.ColourName()} from {ComputerFileTransferUtilities.DescribeOwner(session.CurrentOwner).ColourName()} to {remoteHost.Name.ColourName()} as {remoteFileName.ColourName()}.\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}",
			false);
	}

	private static (string Output, bool Exit) HandleDelete(IFuturemud gameworld, IComputerTerminalSession session,
		FtpState state, IComputerHost? remoteHost, IComputerFtpAccount? account, StringStack ss)
	{
		if (remoteHost is null)
		{
			return ($"You must open a remote FTP host first.\n\n{RenderPrompt(session.User, state, null, null, null)}",
				false);
		}

		if (account is null)
		{
			return ($"You must log in before you can delete remote files.\n\n{RenderPrompt(session.User, state, remoteHost, null, null)}",
				false);
		}

		if (ss.IsFinished)
		{
			return ($"Which remote file do you want to delete?\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}",
				false);
		}

		var remoteFileName = ss.PopSpeech();
		if (!gameworld.ComputerFileTransferService.DeleteFile(
			    session.Host,
			    remoteHost,
			    account,
			    state.SelectedRemoteOwnerIdentifier,
			    remoteFileName,
			    out var error))
		{
			return ($"{error}\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}", false);
		}

		return ($"Deleted {remoteFileName.ColourName()} from {remoteHost.Name.ColourName()}.\n\n{RenderPrompt(session.User, state, remoteHost, account, null)}",
			false);
	}

	private static void SendOverview(IFuturemud gameworld, IComputerTerminalSession session, IComputerBuiltInApplication application,
		FtpState state, IComputerHost? remoteHost, IComputerFtpAccount? account, string? warning)
	{
		var sb = new StringBuilder();
		sb.AppendLine(RenderStatus(session.User, state, remoteHost, account, warning));
		sb.AppendLine();
		sb.Append(RenderHelp(application, state, remoteHost, account));
		session.User.OutputHandler.Send(sb.ToString(), nopage: true);
	}

	private static string RenderStatus(ICharacter user, FtpState state, IComputerHost? remoteHost, IComputerFtpAccount? account,
		string? warning)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(warning))
		{
			sb.AppendLine(warning);
			sb.AppendLine();
		}

		sb.AppendLine($"Remote Host: {(remoteHost is null ? "not connected".ColourError() : remoteHost.Name.ColourName())}");
		if (remoteHost is not null)
		{
			sb.AppendLine($"Selected Remote Owner: {state.SelectedRemoteOwnerIdentifier.ColourName()}");
		}

		sb.AppendLine($"Authenticated User: {(account is null ? "anonymous".ColourError() : account.UserName.ColourValue())}");
		sb.Append(RenderPrompt(user, state, remoteHost, account, null));
		return sb.ToString();
	}

	private static string RenderHelp(IComputerBuiltInApplication application, FtpState state, IComputerHost? remoteHost,
		IComputerFtpAccount? account)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{application.Name.ColourName()} commands:");
		sb.AppendLine($"\t{"hosts".ColourCommand()} - list reachable hosts advertising FTP");
		sb.AppendLine($"\t{"open <host>".ColourCommand()} - open a remote FTP host");
		sb.AppendLine($"\t{"close".ColourCommand()} - close the current remote FTP host");
		sb.AppendLine($"\t{"status".ColourCommand()} - show the current remote FTP status");
		sb.AppendLine($"\t{"owners".ColourCommand()} - list accessible remote owners");
		sb.AppendLine($"\t{"use <owner>".ColourCommand()} - switch the selected remote owner");
		sb.AppendLine($"\t{"login <user> <password>".ColourCommand()} - authenticate for writable remote access");
		sb.AppendLine($"\t{"logout".ColourCommand()} - return to anonymous/public access");
		sb.AppendLine($"\t{"list".ColourCommand()} - list files on the selected remote owner");
		sb.AppendLine($"\t{"show <file>".ColourCommand()} - display a remote file");
		sb.AppendLine($"\t{"get <file> [to <target>]".ColourCommand()} - copy a remote file to the current local owner or another local target");
		sb.AppendLine($"\t{"put <local-file> [remote-file]".ColourCommand()} - upload a local file to the selected remote owner");
		sb.AppendLine($"\t{"delete <file>".ColourCommand()} - delete a remote file");
		sb.AppendLine($"\t{"help".ColourCommand()} - show this help");
		sb.AppendLine($"\t{"exit".ColourCommand()} - close FTP");
		sb.AppendLine();
		sb.AppendLine("Anonymous FTP access is limited to files published for public network access. Log in to gain full read/write access to the remote host and its mounted storage.");
		sb.AppendLine();
		sb.Append(RenderPrompt(null, state, remoteHost, account, null));
		return sb.ToString();
	}

	private static string RenderReachableHosts(IFuturemud gameworld, ICharacter user, IComputerHost sourceHost, FtpState state,
		IComputerHost? remoteHost, IComputerFtpAccount? account)
	{
		var hosts = gameworld.ComputerExecutionService.GetReachableHosts(sourceHost)
			.Where(x => x.Host.IsNetworkServiceEnabled("ftp"))
			.OrderBy(x => x.IsLocalGrid ? 0 : 1)
			.ThenBy(x => x.Host.Name)
			.ThenBy(x => x.CanonicalAddress)
			.ToList();

		var sb = new StringBuilder();
		sb.AppendLine("Reachable FTP Hosts:");
		if (!hosts.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				hosts.Select(summary =>
				{
					var service = gameworld.ComputerExecutionService
						.GetAdvertisedServices(sourceHost, summary.Host)
						.FirstOrDefault(x => x.ApplicationId.EqualTo("ftp"));
					var details = service?.ServiceDetails.Any() == true
						? service.ServiceDetails.ListToString()
						: "-";
					return new List<string>
					{
						summary.Host.Name,
						summary.CanonicalAddress,
						summary.IsLocalGrid ? "local" : "linked",
						details
					};
				}),
				new List<string>
				{
					"Host",
					"Address",
					"Scope",
					"Details"
				},
				user.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				user.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.Append(RenderPrompt(user, state, remoteHost, account, null));
		return sb.ToString();
	}

	private static ComputerProgramExecutionOutcome WaitForInput(IComputerTerminalSession session, FtpState state)
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

	private static FtpState LoadState(string? stateJson)
	{
		if (!string.IsNullOrWhiteSpace(stateJson))
		{
			try
			{
				var state = JsonSerializer.Deserialize<FtpState>(stateJson);
				if (state is not null)
				{
					return state;
				}
			}
			catch (JsonException)
			{
			}
		}

		return new FtpState();
	}

	private static IComputerHost? ResolveConnectedHost(IFuturemud gameworld, IComputerHost sourceHost, FtpState state,
		out string? warning)
	{
		warning = null;
		if (state.RemoteHostItemId is not > 0L)
		{
			return null;
		}

		if (sourceHost.OwnerHostItemId == state.RemoteHostItemId && gameworld.ComputerFileTransferService.IsFtpServiceEnabled(sourceHost))
		{
			return sourceHost;
		}

		var summary = gameworld.ComputerExecutionService.GetReachableHosts(sourceHost)
			.FirstOrDefault(x => x.Host.OwnerHostItemId == state.RemoteHostItemId.Value && x.Host.IsNetworkServiceEnabled("ftp"));
		if (summary?.Host is not null)
		{
			return summary.Host;
		}

		warning = $"{state.RemoteHostName.ColourName()} is no longer reachable over FTP. The current FTP session has been closed.";
		state.ClearConnection();
		return null;
	}

	private static IComputerFtpAccount? ResolveLoggedInAccount(IFuturemud gameworld, IComputerHost sourceHost,
		IComputerHost? remoteHost, FtpState state, out string? warning)
	{
		warning = null;
		if (remoteHost is null || string.IsNullOrWhiteSpace(state.RemoteAccountUserName))
		{
			return null;
		}

		var account = gameworld.ComputerFileTransferService.GetAccount(sourceHost, remoteHost, state.RemoteAccountUserName,
			out var error);
		if (account is not null)
		{
			return account;
		}

		state.ClearAuthentication();
		warning = error;
		return null;
	}

	private static ComputerNetworkHostSummary? ResolveFtpHost(IFuturemud gameworld, IComputerHost sourceHost, string identifier)
	{
		var summary = gameworld.ComputerExecutionService.ResolveReachableHost(sourceHost, identifier);
		if (summary is not null && summary.Host.IsNetworkServiceEnabled("ftp"))
		{
			return summary;
		}

		return null;
	}

	private static ComputerRemoteFileOwnerSummary? ResolveAccessibleOwner(IFuturemud gameworld, IComputerHost sourceHost,
		IComputerHost remoteHost, IComputerFtpAccount? account, string identifier, out string error)
	{
		error = string.Empty;
		var owners = gameworld.ComputerFileTransferService.GetAccessibleOwners(sourceHost, remoteHost, account, out error)
			.ToList();
		if (!string.IsNullOrEmpty(error))
		{
			return null;
		}

		var exact = owners
			.Where(x => x.OwnerIdentifier.Equals(identifier, StringComparison.InvariantCultureIgnoreCase) ||
			            x.DisplayName.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (exact.Count == 1)
		{
			return exact.Single();
		}

		if (exact.Count > 1)
		{
			error = $"More than one remote owner matches {identifier.ColourName()}.";
			return null;
		}

		var partial = owners
			.Where(x => x.OwnerIdentifier.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase) ||
			            x.DisplayName.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (partial.Count == 1)
		{
			return partial.Single();
		}

		if (partial.Count > 1)
		{
			error = $"More than one remote owner starts with {identifier.ColourName()}.";
			return null;
		}

		error = $"There is no accessible remote owner named {identifier.ColourName()} on {remoteHost.Name.ColourName()}.";
		return null;
	}

	private static string RenderPrompt(ICharacter? user, FtpState state, IComputerHost? remoteHost, IComputerFtpAccount? account,
		string? warning)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(warning))
		{
			sb.AppendLine(warning);
		}

		if (remoteHost is null)
		{
			sb.Append("Use ");
			sb.Append("type <command>".ColourCommand());
			sb.Append(" with no remote host open.");
			return sb.ToString();
		}

		sb.Append("Use ");
		sb.Append("type <command>".ColourCommand());
		sb.Append($" for {remoteHost.Name.ColourName()}::{state.SelectedRemoteOwnerIdentifier.ColourName()} as ");
		sb.Append((account?.UserName ?? "anonymous").ColourName());
		sb.Append('.');
		return sb.ToString();
	}
}
