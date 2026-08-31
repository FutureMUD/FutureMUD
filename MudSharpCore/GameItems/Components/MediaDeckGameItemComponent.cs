#nullable enable

using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

/// <summary>
/// Generic physical recording and playback transport. It deliberately has no transport position, rewind, editing
/// or transcoding state in version one; playback starts at the beginning of the immutable recording.
/// </summary>
public class MediaDeckGameItemComponent : MediaBoundSinkPoweredComponentBase, IMediaDeck, IMediaSource
{
	private MediaDeckGameItemComponentProto _prototype;
	private Guid _streamId = Guid.NewGuid();
	private long _sequence;
	private long _recordingId;
	private string? _recordingName;
	private IMediaStorageMedium? _recordingMedium;
	private DateTime _recordingStartedAtUtc;
	private TimeSpan _recordingCapacity;
	private bool _recordingHeartbeatSubscribed;
	private IReadOnlyList<MediaPacket>? _playbackPackets;
	private DateTime _playbackStartedAtUtc;
	private DateTime _firstPlaybackPacketAtUtc;
	private int _nextPlaybackPacket;
	private bool _playbackHeartbeatSubscribed;

	public MediaDeckGameItemComponent(MediaDeckGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public MediaDeckGameItemComponent(MudSharp.Models.GameItemComponent component,
		MediaDeckGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadRuntimeState(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public MediaDeckGameItemComponent(MediaDeckGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_streamId = Guid.NewGuid();
		_sequence = rhs._sequence;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public override MediaEndpointAddress MediaEndpoint => new(Parent.Id, Id, $"{_prototype.EndpointKey}:out", MediaEndpointDirection.Output);
	public override MediaEndpointAddress MediaInputEndpoint => new(Parent.Id, Id, $"{_prototype.EndpointKey}:in", MediaEndpointDirection.Input);
	public override MediaCapabilities MediaCapabilities => _prototype.Capabilities;
	public override bool MediaAvailable => IsPowered && Parent.TrueLocations.Any();
	public string FormatKey => _prototype.FormatKey;
	public bool CanRecord => _prototype.CanRecord;
	public bool CanPlayback => _prototype.CanPlayback;
	public bool IsRecording => _recordingId > 0L;
	public bool IsPlaying => _playbackPackets is not null;
	protected override int MediaOutputPorts => _prototype.OutputPorts;
	protected override bool AcceptSiblingSources => _prototype.AcceptSiblingSources;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new MediaDeckGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return description;
		}

		var medium = GetInsertedMedium();
		var sb = new StringBuilder(description);
		sb.AppendLine();
		sb.AppendLine();
		sb.AppendLine($"Its {FormatKey.ColourName()} media deck is {(MediaAvailable ? "powered".ColourValue() : "not powered".ColourError())}.");
		sb.AppendLine(medium is null
			? "No compatible physical medium is inserted."
			: $"It has {medium.Parent.HowSeen(voyeur).ColourName()} inserted.");
		if (IsRecording)
		{
			sb.AppendLine($"It is recording {(_recordingName ?? "an unnamed recording").ColourCommand()}.");
		}
		else if (IsPlaying)
		{
			sb.AppendLine("It is playing a recording from the beginning.");
		}
		else
		{
			sb.AppendLine("It is stopped.");
		}

		return sb.ToString();
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (MediaDeckGameItemComponentProto)newProto;
	}

	protected override XElement SaveToXml(XElement root)
	{
		return SaveMediaSinkState(root);
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		LoadRuntimeState(root);
	}

	protected override void OnPowerCutInAction()
	{
		ActivateMediaSink();
	}

	protected override void OnPowerCutOutAction()
	{
		StopInternal(MediaRecordingStatus.Interrupted, out _);
		DeactivateMediaSink();
	}

	public override void Delete()
	{
		StopInternal(MediaRecordingStatus.Interrupted, out _);
		ReleasePlaybackHeartbeat();
		base.Delete();
	}

	public override void Quit()
	{
		StopInternal(MediaRecordingStatus.Interrupted, out _);
		ReleasePlaybackHeartbeat();
		base.Quit();
	}

	public bool StartRecording(string name, out string error)
	{
		if (!MediaAvailable)
		{
			error = "That media deck is not powered and available.";
			return false;
		}

		if (!CanRecord)
		{
			error = "That media deck cannot record.";
			return false;
		}

		if (IsRecording || IsPlaying)
		{
			error = "That media deck is already recording or playing.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			error = "You must give the recording a name.";
			return false;
		}

		var medium = GetCompatibleInsertedMedium(out error);
		if (medium is null)
		{
			return false;
		}

		if (medium.HasRecording(name))
		{
			error = "That medium already has a recording with that name. Erase it first.";
			return false;
		}

		var availableCapacity = medium.RemainingCapacity;
		if (availableCapacity <= TimeSpan.Zero)
		{
			error = "That physical medium has no remaining recording capacity.";
			return false;
		}

		var now = DateTime.UtcNow;
		var recording = Gameworld.MediaRecordingService.CreateRecording(new MediaRecordingCreateRequest(MediaCapabilities,
			name.Trim(), now, medium.Id));
		if (!medium.StoreRecording(recording, out error))
		{
			Gameworld.MediaRecordingService.FinaliseRecording(recording.RecordingId, MediaRecordingStatus.Failed,
				DateTime.UtcNow, out _);
			return false;
		}

		_recordingId = recording.RecordingId;
		_recordingName = name.Trim();
		_recordingMedium = medium;
		_recordingStartedAtUtc = now;
		_recordingCapacity = availableCapacity;
		EnsureRecordingHeartbeat();
		error = string.Empty;
		return true;
	}

	public bool StartPlayback(string name, out string error)
	{
		if (!MediaAvailable)
		{
			error = "That media deck is not powered and available.";
			return false;
		}

		if (!CanPlayback)
		{
			error = "That media deck cannot play recordings.";
			return false;
		}

		if (IsRecording || IsPlaying)
		{
			error = "That media deck is already recording or playing.";
			return false;
		}

		var medium = GetCompatibleInsertedMedium(out error);
		if (medium is null)
		{
			return false;
		}

		var reference = medium.GetRecording(name);
		if (reference is null)
		{
			error = "That inserted medium has no recording with that name.";
			return false;
		}

		var recording = Gameworld.MediaRecordingService.GetRecording(reference.RecordingId);
		if (recording is null || recording.Status == MediaRecordingStatus.Recording)
		{
			error = "That recording is not complete enough to play.";
			return false;
		}

		if ((recording.Capabilities & ~MediaCapabilities) != MediaCapabilities.None)
		{
			error = "That recording contains media this deck cannot play.";
			return false;
		}

		var packets = Gameworld.MediaRecordingService.ReadPackets(recording.RecordingId)
			.OrderBy(x => x.TimestampUtc)
			.ThenBy(x => x.Sequence)
			.ToList();
		if (!packets.Any())
		{
			var scene = Gameworld.MediaRecordingService.GetSceneAt(recording.RecordingId, TimeSpan.Zero);
			if (scene is not null)
			{
				packets.Add(new MediaPacket(Guid.NewGuid(), 0L, DateTime.UtcNow, MediaCapabilities.Video,
					MediaEventKind.SceneSnapshot, MediaEndpoint, [MediaEndpoint], scene));
			}
		}

		if (!packets.Any())
		{
			error = "That recording contains no playable media events.";
			return false;
		}

		_playbackPackets = packets;
		_nextPlaybackPacket = 0;
		_playbackStartedAtUtc = DateTime.UtcNow;
		_firstPlaybackPacketAtUtc = packets[0].TimestampUtc;
		EnsurePlaybackHeartbeat();
		error = string.Empty;
		return true;
	}

	public bool Stop(out string error)
	{
		return StopInternal(MediaRecordingStatus.Finalised, out error);
	}

	public override void ReceiveMedia(MediaPacket packet)
	{
		if (!IsRecording || _recordingId <= 0L)
		{
			return;
		}

		var capacityEnd = _recordingStartedAtUtc + _recordingCapacity;
		if (_recordingCapacity > TimeSpan.Zero && packet.TimestampUtc > capacityEnd)
		{
			StopInternal(MediaRecordingStatus.Finalised, out _, capacityEnd);
			return;
		}

		if (!Gameworld.MediaRecordingService.AppendPacket(_recordingId, packet, out var error))
		{
			StopInternal(MediaRecordingStatus.Failed, out _);
			return;
		}

		if (packet.Payload is not MediaScenePayload scene)
		{
			return;
		}

		var offset = packet.TimestampUtc - _recordingStartedAtUtc;
		if (!Gameworld.MediaRecordingService.AppendSceneFrame(_recordingId, offset, offset, scene.CanonicalScene,
			out error))
		{
			StopInternal(MediaRecordingStatus.Failed, out _);
		}
	}

	private bool StopInternal(MediaRecordingStatus recordingStatus, out string error, DateTime? completedAtUtc = null)
	{
		error = string.Empty;
		var didWork = false;
		if (_recordingId > 0L)
		{
			didWork = true;
			var recordingId = _recordingId;
			var recordingName = _recordingName ?? string.Empty;
			var medium = _recordingMedium;
			var recordingCompletedAtUtc = completedAtUtc ?? DateTime.UtcNow;
			if (_recordingCapacity > TimeSpan.Zero)
			{
				var capacityEnd = _recordingStartedAtUtc + _recordingCapacity;
				if (recordingCompletedAtUtc > capacityEnd)
				{
					recordingCompletedAtUtc = capacityEnd;
				}
			}

			_recordingId = 0L;
			_recordingName = null;
			_recordingMedium = null;
			_recordingCapacity = TimeSpan.Zero;
			ReleaseRecordingHeartbeat();
			if (!Gameworld.MediaRecordingService.FinaliseRecording(recordingId, recordingStatus, recordingCompletedAtUtc,
				out error))
			{
				return false;
			}

			var descriptor = Gameworld.MediaRecordingService.GetRecording(recordingId);
			if (descriptor is null || medium is null || !medium.StoreRecording(descriptor, out error))
			{
				if (medium is not null)
				{
					Gameworld.MediaRecordingService.DeleteReference(medium.Id, recordingName, out _);
				}

				error = string.IsNullOrWhiteSpace(error)
					? "The recording was discarded because its physical medium was no longer available."
					: error;
				return false;
			}
		}

		if (_playbackPackets is not null)
		{
			didWork = true;
			ClearPlayback();
		}

		if (!didWork)
		{
			error = "That media deck is already stopped.";
			return false;
		}

		return true;
	}

	private IMediaStorageMedium? GetCompatibleInsertedMedium(out string error)
	{
		var media = GetInsertedMedia();
		if (media.Count == 0)
		{
			error = "That media deck requires a compatible physical medium inserted in its container.";
			return null;
		}

		if (media.Count > 1)
		{
			error = "That media deck can only use one physical medium at a time. Remove the extra media first.";
			return null;
		}

		var medium = media[0];

		if (!medium.FormatKey.EqualTo(FormatKey))
		{
			error = $"That deck accepts {FormatKey.ColourCommand()} media, not {medium.FormatKey.ColourCommand()} media.";
			return null;
		}

		if ((medium.MediaCapabilities & ~MediaCapabilities) != MediaCapabilities.None)
		{
			error = "That physical medium carries media this deck does not support.";
			return null;
		}

		error = string.Empty;
		return medium;
	}

	private IMediaStorageMedium? GetInsertedMedium()
	{
		return GetInsertedMedia().FirstOrDefault();
	}

	private IReadOnlyList<IMediaStorageMedium> GetInsertedMedia()
	{
		return Parent.Components
			.OfType<IContainer>()
			.SelectMany(x => x.Contents)
			.Select(x => x.GetItemType<IMediaStorageMedium>())
			.Where(x => x is not null)
			.Cast<IMediaStorageMedium>()
			.ToList();
	}

	private void EnsurePlaybackHeartbeat()
	{
		if (_playbackHeartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.SecondHeartbeat += PlaybackHeartbeat;
		_playbackHeartbeatSubscribed = true;
	}

	private void EnsureRecordingHeartbeat()
	{
		if (_recordingHeartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.SecondHeartbeat += RecordingHeartbeat;
		_recordingHeartbeatSubscribed = true;
	}

	private void ReleaseRecordingHeartbeat()
	{
		if (!_recordingHeartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.SecondHeartbeat -= RecordingHeartbeat;
		_recordingHeartbeatSubscribed = false;
	}

	private void RecordingHeartbeat()
	{
		if (!IsRecording)
		{
			ReleaseRecordingHeartbeat();
			return;
		}

		var capacityEnd = _recordingStartedAtUtc + _recordingCapacity;
		if (_recordingCapacity > TimeSpan.Zero && DateTime.UtcNow >= capacityEnd)
		{
			StopInternal(MediaRecordingStatus.Finalised, out _, capacityEnd);
		}
	}

	private void ReleasePlaybackHeartbeat()
	{
		if (!_playbackHeartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.SecondHeartbeat -= PlaybackHeartbeat;
		_playbackHeartbeatSubscribed = false;
	}

	private void PlaybackHeartbeat()
	{
		if (!MediaAvailable || _playbackPackets is null)
		{
			ClearPlayback();
			return;
		}

		var elapsed = DateTime.UtcNow - _playbackStartedAtUtc;
		while (_nextPlaybackPacket < _playbackPackets.Count &&
		       _playbackPackets[_nextPlaybackPacket].TimestampUtc - _firstPlaybackPacketAtUtc <= elapsed)
		{
			var packet = _playbackPackets[_nextPlaybackPacket++];
			var outgoing = packet with
			{
				StreamId = _streamId,
				Sequence = ++_sequence,
				TimestampUtc = DateTime.UtcNow,
				Capabilities = packet.Capabilities & MediaCapabilities,
				Source = MediaEndpoint,
				Provenance = packet.Provenance.Append(MediaEndpoint).ToArray()
			};
			if (outgoing.Capabilities != MediaCapabilities.None && !packet.HasVisited(MediaEndpoint))
			{
				Gameworld.MediaChannelService.Publish(outgoing);
			}
		}

		if (_nextPlaybackPacket >= _playbackPackets.Count)
		{
			ClearPlayback();
		}
	}

	private void ClearPlayback()
	{
		_playbackPackets = null;
		_nextPlaybackPacket = 0;
		ReleasePlaybackHeartbeat();
	}

	private void LoadRuntimeState(XElement root)
	{
		LoadMediaSinkState(root);
	}
}
