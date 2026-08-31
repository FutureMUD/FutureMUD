#nullable enable

using System.IO;
using System.Threading;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Computers;

/// <summary>
/// Owns the volatile jobs started by the Media computer application and computer programs. Recordings and files
/// are persistent; jobs deliberately are not, so an unclean host shutdown can only leave a bounded recording
/// buffer behind.
/// </summary>
public sealed class ComputerMediaService : IComputerMediaService
{
	private abstract class MediaJob
	{
		public required long Id { get; init; }
		public required IComputerHost Host { get; init; }
		public required IComputerMediaInterface Interface { get; init; }
		public required string Endpoint { get; init; }
		public required DateTime StartedAtUtc { get; init; }
	}

	private sealed class RecordingJob : MediaJob
	{
		public long RecordingId { get; set; }
		public string FileName { get; set; } = string.Empty;
		public required string BaseFileName { get; init; }
		public required ComputerMediaPacketReceived Handler { get; init; }
		public ComputerMediaJobKind Kind { get; init; } = ComputerMediaJobKind.Recording;
		public TimeSpan? SegmentDuration { get; init; }
		public TimeSpan? Retention { get; init; }
		public TimeSpan? EventDuration { get; init; }
		public DateTime SegmentStartedAtUtc { get; set; }
		public DateTime? EventDeadlineUtc { get; set; }
		public long SegmentSequence { get; set; }
		public List<(string FileName, DateTime FinalisedAtUtc)> FinalisedSegments { get; } = [];

		public bool IsEventArmed => EventDuration is not null && RecordingId <= 0L;
	}

	private sealed class PlaybackJob : MediaJob
	{
		public required IReadOnlyList<MediaPacket> Packets { get; init; }
		public required DateTime FirstPacketAtUtc { get; init; }
		public int NextPacketIndex { get; set; }
	}

	private readonly object _sync = new();
	private readonly IFuturemud _gameworld;
	private readonly Func<DateTime> _utcNow;
	private readonly Func<IComputerHost, string> _inCharacterTimestamp;
	private readonly Dictionary<long, MediaJob> _jobs = [];
	private readonly Dictionary<MediaEndpointAddress, MediaPacket> _latestPackets = [];
	private long _nextJobId;

	public ComputerMediaService(IFuturemud gameworld, Func<DateTime>? utcNow = null,
		Func<IComputerHost, string>? inCharacterTimestamp = null)
	{
		_gameworld = gameworld;
		_utcNow = utcNow ?? (() => DateTime.UtcNow);
		_inCharacterTimestamp = inCharacterTimestamp ?? GetInCharacterTimestamp;
		_gameworld.HeartbeatManager.SecondHeartbeat += ProcessPlaybackJobs;
	}

	public IEnumerable<string> GetMediaInputs(IComputerHost host)
	{
		return GetInterfaces(host)
			.SelectMany(x => x.InputNames.Append(x.MediaInputEndpoint.EndpointKey))
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.OrderBy(x => x)
			.ToList();
	}

	public IEnumerable<string> GetMediaOutputs(IComputerHost host)
	{
		return GetInterfaces(host)
			.SelectMany(x => x.OutputNames.Append(x.MediaEndpoint.EndpointKey))
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.OrderBy(x => x)
			.ToList();
	}

	public bool TryResolveMediaInput(IComputerHost host, string input, out MediaEndpointAddress endpoint,
		out string error)
	{
		endpoint = MediaEndpointAddress.Empty;
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(input))
		{
			error = "You must specify a media input.";
			return false;
		}

		var mediaInterface = ResolveInput(host, input);
		if (mediaInterface is null || !mediaInterface.MediaAvailable)
		{
			error = "There is no powered, connected media input with that name.";
			return false;
		}

		endpoint = mediaInterface.MediaInputEndpoint;
		return true;
	}

	public bool IsMediaInput(IComputerHost host, MediaEndpointAddress endpoint)
	{
		return endpoint.IsValid && endpoint.Direction == MediaEndpointDirection.Input &&
		       GetInterfaces(host).Any(x => x.MediaAvailable && x.MediaInputEndpoint == endpoint);
	}

	public bool PublishToOutput(IComputerHost host, string output, MediaPacket packet, out string error)
	{
		var mediaInterface = ResolveOutput(host, output);
		if (mediaInterface is null || !mediaInterface.MediaAvailable)
		{
			error = "There is no powered, connected media output with that name.";
			return false;
		}

		return mediaInterface.PublishOutput(output, packet, out error);
	}

	public IEnumerable<ComputerMediaJobInfo> GetJobs(IComputerHost host)
	{
		lock (_sync)
		{
			return _jobs.Values
				.Where(x => ReferenceEquals(x.Host, host))
				.OrderBy(x => x.Id)
				.Select(x => new ComputerMediaJobInfo(x.Id,
					x is RecordingJob recordingJob ? recordingJob.Kind : ComputerMediaJobKind.Playback,
					x.Endpoint,
					x is RecordingJob recording
						? string.IsNullOrEmpty(recording.FileName) ? recording.BaseFileName : recording.FileName
						: string.Empty,
					x.StartedAtUtc,
					x is RecordingJob policyJob ? DescribePolicy(policyJob) : "Playback"))
				.ToList();
		}
	}

	public long StartRecording(IComputerHost host, string input, string fileName, out string error)
	{
		error = string.Empty;
		if (!TryGetHostComponent(host, out var component, out error) || host.FileSystem is null)
		{
			return 0L;
		}

		if (!host.Powered)
		{
			error = "That computer host is not powered.";
			return 0L;
		}

		if (string.IsNullOrWhiteSpace(fileName))
		{
			error = "You must supply a file name for the recording.";
			return 0L;
		}

		var mediaInterface = ResolveInput(host, input);
		if (mediaInterface is null)
		{
			error = "There is no connected media input with that name.";
			return 0L;
		}

		if (host.FileSystem.FileExists(fileName))
		{
			error = $"A file named {fileName} already exists.";
			return 0L;
		}

		var createdAt = _utcNow();
		var recording = _gameworld.MediaRecordingService.CreateRecording(new MediaRecordingCreateRequest(
			mediaInterface.MediaCapabilities,
			fileName,
			createdAt,
			component.Id));
		var reference = new MediaRecordingReference(component.Id, fileName.Trim(), recording.RecordingId, false,
			createdAt, createdAt);
		if (!_gameworld.MediaRecordingService.CreateReference(reference, out error))
		{
			_gameworld.MediaRecordingService.FinaliseRecording(recording.RecordingId, MediaRecordingStatus.Failed,
				_utcNow(), out _);
			return 0L;
		}

		if (!host.FileSystem.WriteMediaFile(fileName.Trim(), recording.RecordingId, 0L, false, out error))
		{
			_gameworld.MediaRecordingService.DeleteReference(component.Id, fileName.Trim(), out _);
			return 0L;
		}

		var id = Interlocked.Increment(ref _nextJobId);
		ComputerMediaPacketReceived? handler = null;
		handler = (source, packet) => ReceiveForRecording(id, source, packet);
		var job = new RecordingJob
		{
			Id = id,
			Host = host,
			Interface = mediaInterface,
			Endpoint = input,
			StartedAtUtc = createdAt,
			RecordingId = recording.RecordingId,
			FileName = fileName.Trim(),
			BaseFileName = fileName.Trim(),
			SegmentStartedAtUtc = createdAt,
			Handler = handler
		};
		lock (_sync)
		{
			_jobs[id] = job;
		}

		mediaInterface.MediaPacketReceived += handler;
		return id;
	}

	public long StartRollingRecording(IComputerHost host, string input, string baseFileName, TimeSpan retention,
		TimeSpan segmentDuration, out string error)
	{
		if (retention < segmentDuration)
		{
			error = "The retention period must be at least as long as one segment.";
			return 0L;
		}

		return StartPolicyRecording(host, input, baseFileName, ComputerMediaJobKind.RollingRecording,
			segmentDuration, retention, null, out error);
	}

	public long StartSegmentedRecording(IComputerHost host, string input, string baseFileName,
		TimeSpan segmentDuration, out string error)
	{
		return StartPolicyRecording(host, input, baseFileName, ComputerMediaJobKind.SegmentedRecording,
			segmentDuration, null, null, out error);
	}

	public long StartEventRecording(IComputerHost host, string input, string baseFileName, TimeSpan activeDuration,
		out string error)
	{
		return StartPolicyRecording(host, input, baseFileName, ComputerMediaJobKind.EventRecording,
			null, null, activeDuration, out error);
	}

	private long StartPolicyRecording(IComputerHost host, string input, string baseFileName,
		ComputerMediaJobKind kind, TimeSpan? segmentDuration, TimeSpan? retention, TimeSpan? eventDuration,
		out string error)
	{
		error = string.Empty;
		if (!TryGetHostComponent(host, out _, out error) || host.FileSystem is null)
		{
			return 0L;
		}

		if (!host.Powered)
		{
			error = "That computer host is not powered.";
			return 0L;
		}

		if (string.IsNullOrWhiteSpace(baseFileName))
		{
			error = "You must supply a base file name for the recordings.";
			return 0L;
		}

		if (segmentDuration is not null && segmentDuration < TimeSpan.FromSeconds(5))
		{
			error = "Media segments must be at least five seconds long.";
			return 0L;
		}

		if (eventDuration is not null && eventDuration < TimeSpan.FromSeconds(5))
		{
			error = "An event recording window must be at least five seconds long.";
			return 0L;
		}

		var mediaInterface = ResolveInput(host, input);
		if (mediaInterface is null)
		{
			error = "There is no connected media input with that name.";
			return 0L;
		}

		var id = Interlocked.Increment(ref _nextJobId);
		ComputerMediaPacketReceived? handler = null;
		handler = (source, packet) => ReceiveForRecording(id, source, packet);
		var now = _utcNow();
		var job = new RecordingJob
		{
			Id = id,
			Host = host,
			Interface = mediaInterface,
			Endpoint = input,
			StartedAtUtc = now,
			BaseFileName = baseFileName.Trim(),
			SegmentDuration = segmentDuration,
			Retention = retention,
			EventDuration = eventDuration,
			SegmentStartedAtUtc = now,
			Kind = kind,
			Handler = handler
		};

		if (eventDuration is null && !StartRecordingSegment(job, now, out error))
		{
			return 0L;
		}

		lock (_sync)
		{
			_jobs[id] = job;
		}

		mediaInterface.MediaPacketReceived += handler;
		return id;
	}

	public long StartPlayback(IComputerHost host, string fileName, string output, out string error)
	{
		error = string.Empty;
		if (!TryGetHostComponent(host, out _, out error) || host.FileSystem is null)
		{
			return 0L;
		}

		if (!host.Powered)
		{
			error = "That computer host is not powered.";
			return 0L;
		}

		var file = host.FileSystem.GetFile(fileName);
		if (file is null)
		{
			error = "There is no file with that name.";
			return 0L;
		}

		if (file.Kind != ComputerFileKind.Media || file.MediaRecordingId is not { } recordingId)
		{
			error = "That is a text file, not a media recording.";
			return 0L;
		}

		var mediaInterface = ResolveOutput(host, output);
		if (mediaInterface is null)
		{
			error = "There is no connected media output with that name.";
			return 0L;
		}

		var descriptor = _gameworld.MediaRecordingService.GetRecording(recordingId);
		if (descriptor is null || descriptor.Status == MediaRecordingStatus.Recording)
		{
			error = "That media file is not a completed recording.";
			return 0L;
		}

		var packets = _gameworld.MediaRecordingService.ReadPackets(recordingId)
			.OrderBy(x => x.TimestampUtc)
			.ThenBy(x => x.Sequence)
			.ToList();
		if (!packets.Any())
		{
			var scene = _gameworld.MediaRecordingService.GetSceneAt(recordingId, TimeSpan.Zero);
			if (scene is not null)
			{
				packets.Add(new MediaPacket(Guid.NewGuid(), 0L, _utcNow(), MediaCapabilities.Video,
					MediaEventKind.SceneSnapshot, mediaInterface.MediaEndpoint, [mediaInterface.MediaEndpoint], scene));
			}
		}

		if (!packets.Any())
		{
			error = "That media file contains no playable events.";
			return 0L;
		}

		var id = Interlocked.Increment(ref _nextJobId);
		var job = new PlaybackJob
		{
			Id = id,
			Host = host,
			Interface = mediaInterface,
			Endpoint = output,
			StartedAtUtc = _utcNow(),
			Packets = packets,
			FirstPacketAtUtc = packets[0].TimestampUtc
		};
		lock (_sync)
		{
			_jobs[id] = job;
		}

		return id;
	}

	public bool CaptureStill(IComputerHost host, string input, string fileName, out string error)
	{
		error = string.Empty;
		if (!TryGetHostComponent(host, out var component, out error) || host.FileSystem is null)
		{
			return false;
		}

		if (!host.Powered)
		{
			error = "That computer host is not powered.";
			return false;
		}

		var mediaInterface = ResolveInput(host, input);
		if (mediaInterface is null)
		{
			error = "There is no connected media input with that name.";
			return false;
		}

		if (!TryGetLatestScene(mediaInterface, out var scene))
		{
			error = "That input has not received a video frame yet.";
			return false;
		}

		if (host.FileSystem.FileExists(fileName))
		{
			error = $"A file named {fileName} already exists.";
			return false;
		}

		var createdAt = _utcNow();
		var recording = _gameworld.MediaRecordingService.CreateRecording(new MediaRecordingCreateRequest(
			MediaCapabilities.Video, fileName, createdAt, component.Id));
		if (!_gameworld.MediaRecordingService.AppendSceneFrame(recording.RecordingId, TimeSpan.Zero,
			TimeSpan.Zero, scene.CanonicalScene, out error) ||
			!_gameworld.MediaRecordingService.FinaliseRecording(recording.RecordingId, MediaRecordingStatus.Finalised,
				_utcNow(), out error))
		{
			return false;
		}

		var finalised = _gameworld.MediaRecordingService.GetRecording(recording.RecordingId)!;
		var reference = new MediaRecordingReference(component.Id, fileName.Trim(), recording.RecordingId, false,
			createdAt, _utcNow());
		if (!_gameworld.MediaRecordingService.CreateReference(reference, out error))
		{
			return false;
		}

		if (host.FileSystem.WriteMediaFile(fileName.Trim(), recording.RecordingId, finalised.LogicalSizeInBytes,
			false, out error))
		{
			return true;
		}

		_gameworld.MediaRecordingService.DeleteReference(component.Id, fileName.Trim(), out _);
		return false;
	}

	public bool StopJob(IComputerHost host, long jobId, out string error)
	{
		error = string.Empty;
		MediaJob? job;
		lock (_sync)
		{
			if (!_jobs.TryGetValue(jobId, out job) || !ReferenceEquals(job.Host, host))
			{
				error = "There is no active media job with that identifier on this host.";
				return false;
			}

			_jobs.Remove(jobId);
		}

		return StopJob(job, MediaRecordingStatus.Finalised, out error);
	}

	public void StopJobsForHost(IComputerHost host, MediaRecordingStatus recordingStatus = MediaRecordingStatus.Interrupted)
	{
		List<MediaJob> jobs;
		lock (_sync)
		{
			jobs = _jobs.Values.Where(x => ReferenceEquals(x.Host, host)).ToList();
			foreach (var job in jobs)
			{
				_jobs.Remove(job.Id);
			}
		}

		foreach (var job in jobs)
		{
			StopJob(job, recordingStatus, out _);
		}
	}

	public void InterruptJobs(IComputerHost host)
	{
		StopJobsForHost(host);
	}

	private IEnumerable<IComputerMediaInterface> GetInterfaces(IComputerHost host)
	{
		return _gameworld.Items
			.SelectMany(x => x.GetItemTypes<IComputerMediaInterface>())
			.Where(x => ReferenceEquals(x.ConnectedHost, host))
			.ToList();
	}

	private IComputerMediaInterface? ResolveInput(IComputerHost host, string input)
	{
		return GetInterfaces(host)
			.FirstOrDefault(x => x.InputNames.Append(x.MediaInputEndpoint.EndpointKey)
				.Any(y => y.Equals(input, StringComparison.InvariantCultureIgnoreCase)));
	}

	private IComputerMediaInterface? ResolveOutput(IComputerHost host, string output)
	{
		return GetInterfaces(host)
			.FirstOrDefault(x => x.OutputNames.Append(x.MediaEndpoint.EndpointKey)
				.Any(y => y.Equals(output, StringComparison.InvariantCultureIgnoreCase)));
	}

	private void ReceiveForRecording(long jobId, IComputerMediaInterface source, MediaPacket packet)
	{
		_latestPackets[source.MediaInputEndpoint] = packet;
		RecordingJob? job;
		lock (_sync)
		{
			job = _jobs.GetValueOrDefault(jobId) as RecordingJob;
		}

		if (job is null || !ReferenceEquals(job.Interface, source))
		{
			return;
		}

		var now = _utcNow();
		if (job.EventDuration is { } eventDuration && IsRecordingTrigger(packet))
		{
			job.EventDeadlineUtc = now + eventDuration;
			if (job.IsEventArmed && !StartRecordingSegment(job, now, out _))
			{
				StopJob(job.Host, job.Id, out _);
				return;
			}
		}

		if (job.RecordingId <= 0L)
		{
			return;
		}

		if (!_gameworld.MediaRecordingService.AppendPacket(job.RecordingId, packet, out _))
		{
			StopJob(job.Host, job.Id, out _);
			return;
		}

		if (packet.Payload is MediaScenePayload scene)
		{
			var offset = packet.TimestampUtc - job.StartedAtUtc;
			if (!_gameworld.MediaRecordingService.AppendSceneFrame(job.RecordingId, offset, offset, scene.CanonicalScene,
				out _))
			{
				StopJob(job.Host, job.Id, out _);
				return;
			}
		}

		if (!SynchroniseRecordingFileSize(job, out _))
		{
			StopJob(job.Host, job.Id, out _);
		}
	}

	private bool StopJob(MediaJob job, MediaRecordingStatus recordingStatus, out string error)
	{
		error = string.Empty;
		if (job is not RecordingJob recording)
		{
			return true;
		}

		recording.Interface.MediaPacketReceived -= recording.Handler;
		if (recording.RecordingId <= 0L)
		{
			return true;
		}

		return FinaliseRecordingSegment(recording, recordingStatus, _utcNow(), out error);
	}

	private bool FinaliseRecordingSegment(RecordingJob recording, MediaRecordingStatus recordingStatus,
		DateTime finalisedAtUtc, out string error)
	{
		if (!_gameworld.MediaRecordingService.FinaliseRecording(recording.RecordingId, recordingStatus, finalisedAtUtc,
			out error))
		{
			return false;
		}

		var descriptor = _gameworld.MediaRecordingService.GetRecording(recording.RecordingId);
		if (descriptor is null || recording.Host.FileSystem is null)
		{
			error = "The media recording finalised but its host file system is no longer available.";
			return false;
		}

		if (recording.Host.FileSystem.UpdateMediaFileSize(recording.FileName, descriptor.LogicalSizeInBytes, out error))
		{
			if (recording.Kind != ComputerMediaJobKind.Recording)
			{
				recording.FinalisedSegments.Add((recording.FileName, finalisedAtUtc));
			}
			recording.RecordingId = 0L;
			recording.FileName = string.Empty;
			recording.EventDeadlineUtc = null;
			return true;
		}

		if (recording.Host is IGameItemComponent)
		{
			recording.Host.FileSystem.DeleteFile(recording.FileName);
		}

		error = $"The recording was discarded because its final compressed size exceeds the host storage capacity: {error}";
		return false;
	}

	private void ProcessPlaybackJobs()
	{
		ProcessRecordingStorageJobs();
		List<PlaybackJob> jobs;
		lock (_sync)
		{
			jobs = _jobs.Values.OfType<PlaybackJob>().ToList();
		}

		foreach (var job in jobs)
		{
			if (!job.Host.Powered)
			{
				lock (_sync)
				{
					_jobs.Remove(job.Id);
				}
				continue;
			}

			var elapsed = _utcNow() - job.StartedAtUtc;
			while (job.NextPacketIndex < job.Packets.Count &&
			       job.Packets[job.NextPacketIndex].TimestampUtc - job.FirstPacketAtUtc <= elapsed)
			{
				var packet = job.Packets[job.NextPacketIndex++];
				job.Interface.PublishOutput(job.Endpoint, packet, out _);
			}

			if (job.NextPacketIndex >= job.Packets.Count)
			{
				lock (_sync)
				{
					_jobs.Remove(job.Id);
				}
			}
		}
	}

	private void ProcessRecordingStorageJobs()
	{
		List<RecordingJob> jobs;
		lock (_sync)
		{
			jobs = _jobs.Values.OfType<RecordingJob>().ToList();
		}

		foreach (var job in jobs)
		{
			if (!job.Host.Powered)
			{
				StopJob(job.Host, job.Id, out _);
				continue;
			}

			var now = _utcNow();
			if (job.EventDeadlineUtc is { } deadline && now >= deadline)
			{
				if (!FinaliseRecordingSegment(job, MediaRecordingStatus.Finalised, now, out _))
				{
					StopJob(job.Host, job.Id, out _);
				}
				continue;
			}

			if (job.SegmentDuration is { } segmentDuration &&
			    now - job.SegmentStartedAtUtc >= segmentDuration)
			{
				if (!FinaliseRecordingSegment(job, MediaRecordingStatus.Finalised, now, out _) ||
				    !ExpireRollingSegments(job, now) ||
				    !StartRecordingSegment(job, now, out _))
				{
					StopJob(job.Host, job.Id, out _);
				}
				continue;
			}

			if (job.RecordingId > 0L && !SynchroniseRecordingFileSize(job, out _))
			{
				StopJob(job.Host, job.Id, out _);
			}
		}
	}

	private bool StartRecordingSegment(RecordingJob job, DateTime startedAtUtc, out string error)
	{
		error = string.Empty;
		if (!TryGetHostComponent(job.Host, out var component, out error) || job.Host.FileSystem is null)
		{
			return false;
		}

		var fileName = GetNextSegmentFileName(job);
		var recording = _gameworld.MediaRecordingService.CreateRecording(new MediaRecordingCreateRequest(
			job.Interface.MediaCapabilities, fileName, startedAtUtc, component.Id));
		var reference = new MediaRecordingReference(component.Id, fileName, recording.RecordingId, false,
			startedAtUtc, startedAtUtc);
		if (!_gameworld.MediaRecordingService.CreateReference(reference, out error))
		{
			_gameworld.MediaRecordingService.FinaliseRecording(recording.RecordingId, MediaRecordingStatus.Failed,
				_utcNow(), out _);
			return false;
		}

		if (!job.Host.FileSystem.WriteMediaFile(fileName, recording.RecordingId, 0L, false, out error))
		{
			_gameworld.MediaRecordingService.DeleteReference(component.Id, fileName, out _);
			return false;
		}

		job.RecordingId = recording.RecordingId;
		job.FileName = fileName;
		job.SegmentStartedAtUtc = startedAtUtc;
		return true;
	}

	private bool ExpireRollingSegments(RecordingJob job, DateTime now)
	{
		if (job.Retention is not { } retention || job.Host.FileSystem is null)
		{
			return true;
		}

		var expired = job.FinalisedSegments
			.Where(x => x.FinalisedAtUtc <= now - retention)
			.ToList();
		foreach (var segment in expired)
		{
			// A user may have already moved or deleted an old segment. Either way it no longer belongs to this
			// rolling namespace, so expiry remains successful and the surveillance job keeps running.
			job.Host.FileSystem.DeleteFile(segment.FileName);
			job.FinalisedSegments.Remove(segment);
		}

		return true;
	}

	private static bool IsRecordingTrigger(MediaPacket packet)
	{
		return packet.Kind is not MediaEventKind.SceneSnapshot and not MediaEventKind.PlaybackState;
	}

	private static string DescribePolicy(RecordingJob job)
	{
		return job.Kind switch
		{
			ComputerMediaJobKind.RollingRecording =>
				$"retain {job.Retention!.Value.Describe()} / {job.SegmentDuration!.Value.Describe()} segments",
			ComputerMediaJobKind.SegmentedRecording => $"split every {job.SegmentDuration!.Value.Describe()}",
			ComputerMediaJobKind.EventRecording => job.IsEventArmed
				? $"armed / {job.EventDuration!.Value.Describe()} after event"
				: $"recording until quiet for {job.EventDuration!.Value.Describe()}",
			_ => "Continuous"
		};
	}

	private string GetNextSegmentFileName(RecordingJob job)
	{
		var extension = Path.GetExtension(job.BaseFileName);
		if (string.IsNullOrWhiteSpace(extension))
		{
			extension = ".av";
		}

		var stem = string.IsNullOrWhiteSpace(Path.GetExtension(job.BaseFileName))
			? job.BaseFileName
			: job.BaseFileName[..^Path.GetExtension(job.BaseFileName).Length];
		string candidate;
		do
		{
			candidate = $"{stem}-{_inCharacterTimestamp(job.Host)}-{++job.SegmentSequence:D4}{extension}";
		} while (job.Host.FileSystem?.FileExists(candidate) == true);

		return candidate;
	}

	private string GetInCharacterTimestamp(IComputerHost host)
	{
		var dateTime = (host as IGameItemComponent)?.Parent.Location?.DateTime() ??
		               _gameworld.Calendars.FirstOrDefault()?.CurrentDateTime;
		if (dateTime?.Date is null || dateTime.Time is null)
		{
			return "ic-undated";
		}

		var date = dateTime.Date;
		var time = dateTime.Time;
		var year = date.Year < 0 ? $"m{Math.Abs(date.Year):D4}" : date.Year.ToString("D4");
		var month = new string(date.Month.Alias
			.Where(char.IsLetterOrDigit)
			.Select(char.ToLowerInvariant)
			.ToArray());
		if (string.IsNullOrEmpty(month))
		{
			month = $"month{date.Month.NominalOrder:D2}";
		}

		return $"{year}{month}{date.Day:D2}T{time.Hours:D2}{time.Minutes:D2}{time.Seconds:D2}";
	}

	private bool SynchroniseRecordingFileSize(RecordingJob job, out string error)
	{
		error = string.Empty;
		var descriptor = _gameworld.MediaRecordingService.GetRecording(job.RecordingId);
		if (descriptor is null || job.Host.FileSystem is null)
		{
			error = "The media recording or its host file system is no longer available.";
			return false;
		}

		var file = job.Host.FileSystem.GetFile(job.FileName);
		if (file is null || file.Kind != ComputerFileKind.Media)
		{
			error = "The media file is no longer available.";
			return false;
		}

		return file.SizeInBytes == descriptor.LogicalSizeInBytes ||
		       job.Host.FileSystem.UpdateMediaFileSize(job.FileName, descriptor.LogicalSizeInBytes, out error);
	}

	private bool TryGetLatestScene(IComputerMediaInterface mediaInterface, out MediaScenePayload scene)
	{
		scene = null!;
		var packet = mediaInterface.LatestPacket;
		if (packet is null && !_latestPackets.TryGetValue(mediaInterface.MediaInputEndpoint, out packet))
		{
			return false;
		}

		if (packet is null ||
			packet.Payload is not MediaScenePayload latest)
		{
			return false;
		}

		scene = latest;
		return true;
	}

	private static bool TryGetHostComponent(IComputerHost host, out IGameItemComponent component, out string error)
	{
		if (host is IGameItemComponent itemComponent && itemComponent.Id > 0L)
		{
			component = itemComponent;
			error = string.Empty;
			return true;
		}

		component = null!;
		error = "Media jobs require a physical computer-host component.";
		return false;
	}
}
