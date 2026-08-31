#nullable enable

using System.Text;
using System.Text.Json;
using TimeSpanParserUtil;

namespace MudSharp.Computers;

/// <summary>
/// Interactive terminal front-end for host-owned media jobs. The process is only the UI; recording and playback
/// jobs live in <see cref="ComputerMediaService"/> and therefore continue after this application is closed.
/// </summary>
internal sealed class MediaBuiltInApplicationExecutor : IComputerBuiltInApplicationExecutor
{
	private sealed class MediaState
	{
		public long? LoggedInAccountId { get; set; }
		public string LoggedInAddress { get; set; } = string.Empty;

		public void ClearLogin()
		{
			LoggedInAccountId = null;
			LoggedInAddress = string.Empty;
		}
	}

	public string ApplicationId => "media";

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
		var account = ResolveLoggedInAccount(gameworld, process.Host, state, out var accountWarning);
		var input = ComputerExecutionContextScope.Current?.ConsumePendingTerminalInput();
		if (string.IsNullOrWhiteSpace(input))
		{
			SendOverview(session, application, process.Host, state, account, accountWarning);
			return WaitForInput(session, state);
		}

		var response = HandleCommand(gameworld, session, application, process.Host, state, account, input!);
		if (!string.IsNullOrWhiteSpace(response.Output))
		{
			session.User.OutputHandler.Send(response.Output, nopage: true);
		}

		return response.Exit
			? new ComputerProgramExecutionOutcome { Status = ComputerProcessStatus.Completed }
			: WaitForInput(session, state);
	}

	private static (string Output, bool Exit) HandleCommand(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerBuiltInApplication application, IComputerHost host, MediaState state, IComputerNetworkAccount? account,
		string input)
	{
		var ss = new StringStack(input.Trim());
		var command = ss.PopSpeech().ToLowerInvariant();
		return command switch
		{
			"" => (RenderPrompt(application), false),
			"help" => (RenderHelp(application), false),
			"inputs" => (RenderInputs(gameworld, host, application), false),
			"outputs" => (RenderOutputs(gameworld, host, application), false),
			"files" => (RenderFiles(session.User, host, application), false),
			"jobs" => (RenderJobs(session.User, gameworld, host, application), false),
			"feeds" => (RenderFeeds(session.User, gameworld, host, application, state, account), false),
			"login" => Login(gameworld, session, host, application, state, ss),
			"logout" => Logout(application, state),
			"record" => Record(gameworld, session, host, application, ss),
			"recordloop" => RecordLoop(gameworld, session, host, application, ss),
			"recordsplit" => RecordSplit(gameworld, session, host, application, ss),
			"recordevent" => RecordEvent(gameworld, session, host, application, ss),
			"snapshot" => Snapshot(gameworld, session, host, application, ss),
			"play" => Play(gameworld, session, host, application, ss),
			"stop" => Stop(gameworld, session, host, application, ss),
			"still" => Still(gameworld, session, host, application, ss),
			"publish" => Publish(gameworld, session, host, application, ss),
			"acl" => Acl(gameworld, session, host, application, ss),
			"subscribe" => Subscribe(gameworld, session, host, application, account, ss),
			"unsubscribe" => Unsubscribe(gameworld, session, host, application, ss),
			"exit" or "quit" => ($"{application.Name.ColourName()} closing.", true),
			_ => ($"That is not a valid {application.Name.ColourName()} command.\n\n{RenderPrompt(application)}", false)
		};
	}

	private static (string Output, bool Exit) Record(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, IComputerBuiltInApplication application, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ($"Which input do you want to record?\n\n{RenderPrompt(application)}", false);
		}

		var input = ss.PopSpeech();
		if (ss.IsFinished || !ss.PopSpeech().EqualTo("as") || ss.IsFinished)
		{
			return ($"Use {"record <input> as <file>".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
		}

		var fileName = ss.SafeRemainingArgument.Trim();
		var jobId = gameworld.ComputerMediaService.StartRecording(host, input, fileName, out var error);
		return jobId > 0L
			? ($"Recording {input.ColourCommand()} as {fileName.ColourName()} with job #{jobId.ToString("N0", session.User).ColourValue()}.\n\n{RenderPrompt(application)}", false)
			: ($"{error}\n\n{RenderPrompt(application)}", false);
	}

	private static (string Output, bool Exit) RecordLoop(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, IComputerBuiltInApplication application, StringStack ss)
	{
		const string syntax = "recordloop <input> as <base-file> retain <duration> segments <duration>";
		if (!TryParsePolicyPrefix(ss, "retain", out var input, out var baseFileName) || ss.IsFinished ||
		    !TryParseDuration(ss.PopSpeech(), out var retention) || ss.IsFinished ||
		    !ss.PopSpeech().EqualTo("segments") || ss.IsFinished ||
		    !TryParseDuration(ss.PopSpeech(), out var segmentDuration) || !ss.IsFinished)
		{
			return ($"Use {syntax.ColourCommand()}, for example {"recordloop camera-in as lobby retain 24h segments 1h".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
		}

		var jobId = gameworld.ComputerMediaService.StartRollingRecording(host, input, baseFileName, retention,
			segmentDuration, out var error);
		return jobId > 0L
			? ($"Started rolling recording of {input.ColourCommand()} as timestamped {baseFileName.ColourName()} segments, retaining {retention.Describe(session.User).ColourValue()}, with job #{jobId.ToString("N0", session.User).ColourValue()}.\n\n{RenderPrompt(application)}", false)
			: ($"{error}\n\n{RenderPrompt(application)}", false);
	}

	private static (string Output, bool Exit) RecordSplit(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, IComputerBuiltInApplication application, StringStack ss)
	{
		const string syntax = "recordsplit <input> as <base-file> every <duration>";
		if (!TryParsePolicyPrefix(ss, "every", out var input, out var baseFileName) || ss.IsFinished ||
		    !TryParseDuration(ss.PopSpeech(), out var segmentDuration) || !ss.IsFinished)
		{
			return ($"Use {syntax.ColourCommand()}, for example {"recordsplit camera-in as lobby every 12h".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
		}

		var jobId = gameworld.ComputerMediaService.StartSegmentedRecording(host, input, baseFileName, segmentDuration,
			out var error);
		return jobId > 0L
			? ($"Started segmented recording of {input.ColourCommand()} every {segmentDuration.Describe(session.User).ColourValue()} with job #{jobId.ToString("N0", session.User).ColourValue()}.\n\n{RenderPrompt(application)}", false)
			: ($"{error}\n\n{RenderPrompt(application)}", false);
	}

	private static (string Output, bool Exit) RecordEvent(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, IComputerBuiltInApplication application, StringStack ss)
	{
		const string syntax = "recordevent <input> as <base-file> for <duration>";
		if (!TryParsePolicyPrefix(ss, "for", out var input, out var baseFileName) || ss.IsFinished ||
		    !TryParseDuration(ss.PopSpeech(), out var activeDuration) || !ss.IsFinished)
		{
			return ($"Use {syntax.ColourCommand()}, for example {"recordevent camera-in as lobby for 5m".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
		}

		var jobId = gameworld.ComputerMediaService.StartEventRecording(host, input, baseFileName, activeDuration,
			out var error);
		return jobId > 0L
			? ($"Armed event recording on {input.ColourCommand()} for {activeDuration.Describe(session.User).ColourValue()} after each observed event with job #{jobId.ToString("N0", session.User).ColourValue()}.\n\n{RenderPrompt(application)}", false)
			: ($"{error}\n\n{RenderPrompt(application)}", false);
	}

	private static bool TryParsePolicyPrefix(StringStack ss, string policyKeyword, out string input,
		out string baseFileName)
	{
		input = string.Empty;
		baseFileName = string.Empty;
		if (ss.IsFinished)
		{
			return false;
		}

		input = ss.PopSpeech();
		if (ss.IsFinished || !ss.PopSpeech().EqualTo("as") || ss.IsFinished)
		{
			return false;
		}

		baseFileName = ss.PopSpeech();
		return !ss.IsFinished && ss.PopSpeech().EqualTo(policyKeyword);
	}

	private static bool TryParseDuration(string text, out TimeSpan duration)
	{
		return TimeSpanParser.TryParse(text, TimeSpanParserUtil.Units.Days, TimeSpanParserUtil.Units.Seconds,
			out duration) && duration > TimeSpan.Zero;
	}

	private static (string Output, bool Exit) Snapshot(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, IComputerBuiltInApplication application, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ($"Which input do you want to capture?\n\n{RenderPrompt(application)}", false);
		}

		var input = ss.PopSpeech();
		if (ss.IsFinished || !ss.PopSpeech().EqualTo("as") || ss.IsFinished)
		{
			return ($"Use {"snapshot <input> as <file>".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
		}

		var fileName = ss.SafeRemainingArgument.Trim();
		return gameworld.ComputerMediaService.CaptureStill(host, input, fileName, out var error)
			? ($"Captured a still from {input.ColourCommand()} as {fileName.ColourName()}.\n\n{RenderPrompt(application)}", false)
			: ($"{error}\n\n{RenderPrompt(application)}", false);
	}

	private static (string Output, bool Exit) Play(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, IComputerBuiltInApplication application, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ($"Which recording do you want to play?\n\n{RenderPrompt(application)}", false);
		}

		var fileName = ss.PopSpeech();
		if (ss.IsFinished || !ss.PopSpeech().EqualTo("to") || ss.IsFinished)
		{
			return ($"Use {"play <file> to <output>".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
		}

		var output = ss.SafeRemainingArgument.Trim();
		var jobId = gameworld.ComputerMediaService.StartPlayback(host, fileName, output, out var error);
		return jobId > 0L
			? ($"Playing {fileName.ColourName()} to {output.ColourCommand()} with job #{jobId.ToString("N0", session.User).ColourValue()}.\n\n{RenderPrompt(application)}", false)
			: ($"{error}\n\n{RenderPrompt(application)}", false);
	}

	private static (string Output, bool Exit) Stop(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, IComputerBuiltInApplication application, StringStack ss)
	{
		if (ss.IsFinished || !long.TryParse(ss.PopSpeech(), out var jobId) || jobId <= 0L)
		{
			return ($"Which positive media job number do you want to stop?\n\n{RenderPrompt(application)}", false);
		}

		return gameworld.ComputerMediaService.StopJob(host, jobId, out var error)
			? ($"Stopped media job #{jobId.ToString("N0", session.User).ColourValue()}.\n\n{RenderPrompt(application)}", false)
			: ($"{error}\n\n{RenderPrompt(application)}", false);
	}

	private static (string Output, bool Exit) Login(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, IComputerBuiltInApplication application, MediaState state, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ($"Which network account do you want to use?\n\n{RenderPrompt(application)}", false);
		}

		var address = ss.PopSpeech();
		if (ss.IsFinished)
		{
			return ($"What password do you want to use for {address.ColourName()}?\n\n{RenderPrompt(application)}", false);
		}

		var authentication = gameworld.ComputerNetworkIdentityService.Authenticate(host, address, ss.SafeRemainingArgument);
		if (!authentication.Success || authentication.Account is null)
		{
			return ($"{authentication.ErrorMessage}\n\n{RenderPrompt(application)}", false);
		}

		state.LoggedInAccountId = authentication.Account.Id;
		state.LoggedInAddress = authentication.Account.Address;
		return ($"You log in to {authentication.Account.Address.ColourName()} for Media feeds.\n\n{RenderPrompt(application)}", false);
	}

	private static (string Output, bool Exit) Logout(IComputerBuiltInApplication application, MediaState state)
	{
		if (state.LoggedInAccountId is not > 0L)
		{
			return ($"You are not currently logged in to a network account for Media feeds.\n\n{RenderPrompt(application)}", false);
		}

		var address = state.LoggedInAddress;
		state.ClearLogin();
		return ($"You log out of {address.ColourName()}.\n\n{RenderPrompt(application)}", false);
	}

	private static (string Output, bool Exit) Publish(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, IComputerBuiltInApplication application, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ($"Which input do you want to publish?\n\n{RenderPrompt(application)}", false);
		}

		var input = ss.PopSpeech();
		if (ss.IsFinished || !ss.PopSpeech().EqualTo("as") || ss.IsFinished)
		{
			return ($"Use {"publish <input> as <feed> public|private".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
		}

		var feedName = ss.PopSpeech();
		if (ss.IsFinished)
		{
			return ($"Specify whether that feed is {"public".ColourCommand()} or {"private".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
		}

		var visibility = ss.PopSpeech();
		if (!ss.IsFinished || !(visibility.EqualTo("public") || visibility.EqualTo("private")))
		{
			return ($"Use {"publish <input> as <feed> public|private".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
		}

		return gameworld.ComputerMediaNetworkService.PublishFeed(host, input, feedName, visibility.EqualTo("public"),
			out var error)
			? ($"Published {input.ColourCommand()} as the {(visibility.EqualTo("public") ? "public" : "private").ColourValue()} feed {feedName.ColourName()}. Enable the Media network service to advertise it.\n\n{RenderPrompt(application)}", false)
			: ($"{error}\n\n{RenderPrompt(application)}", false);
	}

	private static (string Output, bool Exit) Acl(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, IComputerBuiltInApplication application, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ($"Which feed's access list do you want to edit?\n\n{RenderPrompt(application)}", false);
		}

		var feedName = ss.PopSpeech();
		if (ss.IsFinished)
		{
			return ($"Use {"acl <feed> add|remove <user@domain>".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
		}

		var operation = ss.PopSpeech();
		if (!(operation.EqualTo("add") || operation.EqualTo("remove")) || ss.IsFinished)
		{
			return ($"Use {"acl <feed> add|remove <user@domain>".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
		}

		var address = ss.SafeRemainingArgument;
		return gameworld.ComputerMediaNetworkService.SetFeedAcl(host, feedName, address, operation.EqualTo("add"),
			out var error)
			? ($"Updated the access list for {feedName.ColourName()}.\n\n{RenderPrompt(application)}", false)
			: ($"{error}\n\n{RenderPrompt(application)}", false);
	}

	private static (string Output, bool Exit) Subscribe(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, IComputerBuiltInApplication application, IComputerNetworkAccount? account, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ($"Which host-address/feed do you want to subscribe to?\n\n{RenderPrompt(application)}", false);
		}

		if (!TrySplitFeedAddress(ss.PopSpeech(), out var hostAddress, out var feedName))
		{
			return ($"Use {"subscribe <host-address>/<feed> to <output> [save <name>]".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
		}

		if (ss.IsFinished || !ss.PopSpeech().EqualTo("to") || ss.IsFinished)
		{
			return ($"Use {"subscribe <host-address>/<feed> to <output> [save <name>]".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
		}

		var output = ss.PopSpeech();
		string? savedName = null;
		if (!ss.IsFinished)
		{
			if (!ss.PopSpeech().EqualTo("save") || ss.IsFinished)
			{
				return ($"Use {"subscribe <host-address>/<feed> to <output> [save <name>]".ColourCommand()}.\n\n{RenderPrompt(application)}", false);
			}

			savedName = ss.SafeRemainingArgument.Trim();
		}

		return gameworld.ComputerMediaNetworkService.SubscribeFeed(host, hostAddress, feedName, output, account,
			savedName, session, out var subscriptionName, out var error)
			? ($"Subscribed to {hostAddress.ColourName()}/{feedName.ColourName()} on {output.ColourCommand()} as {subscriptionName.ColourName()}.\n\n{RenderPrompt(application)}", false)
			: ($"{error}\n\n{RenderPrompt(application)}", false);
	}

	private static (string Output, bool Exit) Unsubscribe(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, IComputerBuiltInApplication application, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ($"Which saved media subscription do you want to remove?\n\n{RenderPrompt(application)}", false);
		}

		var name = ss.SafeRemainingArgument;
		return gameworld.ComputerMediaNetworkService.UnsubscribeFeed(host, name, out var error)
			? ($"Removed the media subscription {name.ColourName()}.\n\n{RenderPrompt(application)}", false)
			: ($"{error}\n\n{RenderPrompt(application)}", false);
	}

	private static (string Output, bool Exit) Still(IFuturemud gameworld, IComputerTerminalSession session,
		IComputerHost host, IComputerBuiltInApplication application, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return ($"Which media file do you want to inspect?\n\n{RenderPrompt(application)}", false);
		}

		var file = host.FileSystem?.GetFile(ss.PopSpeech());
		if (file?.Kind != ComputerFileKind.Media || file.MediaRecordingId is not { } recordingId)
		{
			return ($"That is not a media file on this host.\n\n{RenderPrompt(application)}", false);
		}

		var offset = gameworld.MediaRecordingService.GetRecording(recordingId)?.Duration ?? TimeSpan.Zero;
		if (!ss.IsFinished)
		{
			if (!double.TryParse(ss.PopSpeech(), out var seconds) || seconds < 0.0)
			{
				return ($"The optional timestamp must be a non-negative number of seconds.\n\n{RenderPrompt(application)}", false);
			}

			offset = TimeSpan.FromSeconds(seconds);
		}

		var scene = gameworld.MediaRecordingService.GetSceneAt(recordingId, offset);
		if (scene is not null && gameworld.MediaRecordingService.GetRecording(recordingId) is { } recording)
		{
			var frameStart = offset > TimeSpan.FromSeconds(5.0)
				? offset - TimeSpan.FromSeconds(5.0)
				: TimeSpan.Zero;
			foreach (var packet in gameworld.MediaRecordingService.ReadPackets(recordingId)
				         .Where(x => x.Payload is MediaCrimePayload &&
				                     x.TimestampUtc - recording.CreatedAtUtc >= frameStart &&
				                     x.TimestampUtc - recording.CreatedAtUtc <= offset))
			{
				gameworld.MediaChannelService.AddViewerAsCrimeWitness(session.User, packet);
			}
		}

		return scene is null
			? ($"That media file has no stored scene at that time.\n\n{RenderPrompt(application)}", false)
			: ($"Still from {file.FileName.ColourName()} at {offset.Describe(session.User).ColourValue()}:\n\n{scene.CanonicalScene}\n\n{RenderPrompt(application)}", false);
	}

	private static string RenderInputs(IFuturemud gameworld, IComputerHost host, IComputerBuiltInApplication application)
	{
		var inputs = gameworld.ComputerMediaService.GetMediaInputs(host).ToList();
		return inputs.Any()
			? $"Media Inputs:\n\t{inputs.Select(x => x.ColourCommand()).ListToString()}\n\n{RenderPrompt(application)}"
			: $"No powered media inputs are connected.\n\n{RenderPrompt(application)}";
	}

	private static string RenderOutputs(IFuturemud gameworld, IComputerHost host, IComputerBuiltInApplication application)
	{
		var outputs = gameworld.ComputerMediaService.GetMediaOutputs(host).ToList();
		return outputs.Any()
			? $"Media Outputs:\n\t{outputs.Select(x => x.ColourCommand()).ListToString()}\n\n{RenderPrompt(application)}"
			: $"No powered media outputs are connected.\n\n{RenderPrompt(application)}";
	}

	private static string RenderFiles(ICharacter user, IComputerHost host, IComputerBuiltInApplication application)
	{
		var files = (host.FileSystem?.Files ?? Enumerable.Empty<IComputerFile>())
			.Where(x => x.Kind == ComputerFileKind.Media)
			.OrderBy(x => x.FileName)
			.ToList();
		if (!files.Any())
		{
			return $"No media files are stored on this host.\n\n{RenderPrompt(application)}";
		}

		return StringUtilities.GetTextTable(files.Select(x => new List<string>
		{
			x.FileName,
			x.SizeInBytes.ToString("N0", user),
			x.LastModifiedAtUtc.ToString(user)
		}), ["File", "Logical Bytes", "Modified"], user) + "\n" + RenderPrompt(application);
	}

	private static string RenderJobs(ICharacter user, IFuturemud gameworld, IComputerHost host,
		IComputerBuiltInApplication application)
	{
		var jobs = gameworld.ComputerMediaService.GetJobs(host).ToList();
		if (!jobs.Any())
		{
			return $"No active media jobs.\n\n{RenderPrompt(application)}";
		}

		return StringUtilities.GetTextTable(jobs.Select(x => new List<string>
		{
			x.JobId.ToString("N0", user),
			x.Kind.DescribeEnum(),
			x.Endpoint,
			x.FileName,
			x.Policy,
			x.StartedAtUtc.ToString(user)
		}), ["Job", "Type", "Endpoint", "File/Base", "Policy", "Started"], user) + "\n" + RenderPrompt(application);
	}


	private static string RenderFeeds(ICharacter user, IFuturemud gameworld, IComputerHost host,
		IComputerBuiltInApplication application, MediaState state, IComputerNetworkAccount? account)
	{
		var feeds = gameworld.ComputerMediaNetworkService.GetFeeds(host).ToList();
		var subscriptions = gameworld.ComputerMediaNetworkService.GetSubscriptions(host).ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"Media feeds on {host.Name.ColourName()}:");
		if (!feeds.Any())
		{
			sb.AppendLine("\tNo local feeds are published.");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(feeds.Select(x => new List<string>
			{
				x.FeedName,
				x.InputName,
				x.IsPublic ? "Public" : $"Private ({x.AllowedAccountIds.Count})",
				x.Active.ToColouredString()
			}), ["Feed", "Input", "Access", "Active"], user));
		}

		sb.AppendLine();
		sb.AppendLine("Media subscriptions:");
		if (!subscriptions.Any())
		{
			sb.AppendLine("\tNo active or saved subscriptions.");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(subscriptions.Select(x => new List<string>
			{
				x.SubscriptionName,
				$"{x.SourceAddress}/{x.FeedName}",
				x.OutputName,
				x.AccountId?.ToString("N0", user) ?? "Public",
				x.Persisted ? "Saved" : "Live",
				x.Active.ToColouredString()
			}), ["Name", "Feed", "Output", "Account", "Mode", "Active"], user));
		}

		sb.AppendLine();
		sb.AppendLine(account is null
			? "No network account is currently logged in. Private feeds require login."
			: $"Logged in as {account.Address.ColourName()}.");
		sb.AppendLine();
		sb.Append(RenderPrompt(application));
		return sb.ToString();
	}

	private static void SendOverview(IComputerTerminalSession session, IComputerBuiltInApplication application,
		IComputerHost host, MediaState state, IComputerNetworkAccount? account, string? warning)
	{
		var notice = string.IsNullOrWhiteSpace(warning) ? string.Empty : $"\n{warning}\n";
		session.User.OutputHandler.Send($"{application.Name.ColourName()} :: {host.Name.ColourName()}{notice}\n{RenderHelp(application)}", nopage: true);
	}

	private static string RenderHelp(IComputerBuiltInApplication application)
	{
		return $"{application.Name.ColourName()} commands:\n" +
		       $"\t{"inputs".ColourCommand()}, {"outputs".ColourCommand()}, {"files".ColourCommand()}, {"jobs".ColourCommand()}, {"feeds".ColourCommand()}\n" +
		       $"\t{"record <input> as <file>".ColourCommand()}\n" +
		       $"\t{"recordloop <input> as <base-file> retain <duration> segments <duration>".ColourCommand()}\n" +
		       $"\t{"recordsplit <input> as <base-file> every <duration>".ColourCommand()}\n" +
		       $"\t{"recordevent <input> as <base-file> for <duration>".ColourCommand()}\n" +
		       $"\t{"snapshot <input> as <file>".ColourCommand()}\n" +
		       $"\t{"play <file> to <output>".ColourCommand()}\n" +
		       $"\t{"still <file> [timestamp]".ColourCommand()}\n" +
		       $"\t{"stop <job>".ColourCommand()}\n" +
		       $"\t{"publish <input> as <feed> public|private".ColourCommand()}\n" +
		       $"\t{"acl <feed> add|remove <user@domain>".ColourCommand()}\n" +
		       $"\t{"login <user@domain> <password>".ColourCommand()}, {"logout".ColourCommand()}\n" +
		       $"\t{"subscribe <host-address>/<feed> to <output> [save <name>]".ColourCommand()}\n" +
		       $"\t{"unsubscribe <subscription>".ColourCommand()}\n" +
		       $"\t{"exit".ColourCommand()}\n\n{RenderPrompt(application)}";
	}

	private static string RenderPrompt(IComputerBuiltInApplication application)
	{
		return $"Use {"type <command>".ColourCommand()} for {application.Name.ColourName()}.";
	}

	private static ComputerProgramExecutionOutcome WaitForInput(IComputerTerminalSession session, MediaState state)
	{
		return new ComputerProgramExecutionOutcome
		{
			Status = ComputerProcessStatus.Sleeping,
			WaitType = ComputerProcessWaitType.UserInput,
			WaitArgument = ComputerProcessWaitArguments.CreateUserInput(
				CharacterInstanceIdentityComparer.IdentityId(session.User), session.Terminal.TerminalItemId),
			WaitingCharacterId = CharacterInstanceIdentityComparer.IdentityId(session.User),
			WaitingTerminalItemId = session.Terminal.TerminalItemId,
			StateJson = JsonSerializer.Serialize(state)
		};
	}

	private static MediaState LoadState(string? stateJson)
	{
		if (string.IsNullOrWhiteSpace(stateJson))
		{
			return new MediaState();
		}

		try
		{
			return JsonSerializer.Deserialize<MediaState>(stateJson) ?? new MediaState();
		}
		catch (JsonException)
		{
			return new MediaState();
		}
	}

	private static IComputerNetworkAccount? ResolveLoggedInAccount(IFuturemud gameworld, IComputerHost host,
		MediaState state, out string? warning)
	{
		warning = null;
		if (state.LoggedInAccountId is not > 0L)
		{
			return null;
		}

		var account = gameworld.ComputerNetworkIdentityService.GetAccount(host, state.LoggedInAccountId.Value,
			out var error);
		if (account is not null)
		{
			state.LoggedInAddress = account.Address;
			return account;
		}

		state.ClearLogin();
		warning = $"The saved Media login is no longer valid: {error}";
		return null;
	}

	private static bool TrySplitFeedAddress(string value, out string hostAddress, out string feedName)
	{
		hostAddress = string.Empty;
		feedName = string.Empty;
		var slashIndex = value.LastIndexOf('/');
		if (slashIndex <= 0 || slashIndex >= value.Length - 1)
		{
			return false;
		}

		hostAddress = value[..slashIndex].Trim();
		feedName = value[(slashIndex + 1)..].Trim();
		return !string.IsNullOrWhiteSpace(hostAddress) && !string.IsNullOrWhiteSpace(feedName);
	}
}
