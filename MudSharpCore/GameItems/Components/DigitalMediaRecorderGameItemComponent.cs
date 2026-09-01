#nullable enable

using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class DigitalMediaRecorderGameItemComponent : ComputerHostGameItemComponent, IDigitalMediaRecorder,
	IComputerMediaInterface
{
	private DigitalMediaRecorderGameItemComponentProto _recorderPrototype;
	private MediaPacket? _latestPacket;
	private long _sequence;

	public DigitalMediaRecorderGameItemComponent(DigitalMediaRecorderGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_recorderPrototype = proto;
	}

	public DigitalMediaRecorderGameItemComponent(MudSharp.Models.GameItemComponent component,
		DigitalMediaRecorderGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_recorderPrototype = proto;
	}

	protected DigitalMediaRecorderGameItemComponent(DigitalMediaRecorderGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_recorderPrototype = rhs._recorderPrototype;
	}

	public override IGameItemComponentProto Prototype => _recorderPrototype;
	public MediaEndpointAddress MediaEndpoint => new(Parent.Id, Id, $"{_recorderPrototype.EndpointKey}:out",
		MediaEndpointDirection.Output);
	public MediaEndpointAddress MediaInputEndpoint => new(Parent.Id, Id, $"{_recorderPrototype.EndpointKey}:in",
		MediaEndpointDirection.Input);
	public MediaCapabilities MediaCapabilities => _recorderPrototype.Capabilities;
	public virtual bool MediaAvailable => Powered && Parent.TrueLocations.Any();
	public MediaEndpointAddress? SourceBinding => null;
	public string FormatKey => "digital";
	public bool CanRecord => true;
	public bool CanPlayback => true;
	public bool IsRecording => Gameworld.ComputerMediaService.GetJobs(this).Any(x => x.Kind != ComputerMediaJobKind.Playback);
	public bool IsPlaying => Gameworld.ComputerMediaService.GetJobs(this).Any(x => x.Kind == ComputerMediaJobKind.Playback);
	public IComputerFileSystem RecordingFileSystem => FileSystem!;
	public IEnumerable<IComputerFile> MediaFiles => RecordingFileSystem.Files.Where(x => x.Kind == ComputerFileKind.Media);
	protected virtual IComputerHost? MediaConnectedHost => this;
	IComputerHost? IComputerMediaInterface.ConnectedHost => MediaConnectedHost;
	public IEnumerable<string> InputNames => [_recorderPrototype.InputName];
	public IEnumerable<string> OutputNames => [_recorderPrototype.OutputName];
	public MediaPacket? LatestPacket => _latestPacket;
	public event ComputerMediaPacketReceived? MediaPacketReceived;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) =>
		new DigitalMediaRecorderGameItemComponent(this, newParent, temporary);

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_recorderPrototype = (DigitalMediaRecorderGameItemComponentProto)newProto;
	}

	public bool Accepts(MediaPacket packet)
	{
		return MediaAvailable && packet.Source.ItemId == Parent.Id && packet.Source != MediaInputEndpoint &&
		       !packet.HasVisited(MediaInputEndpoint) && (packet.Capabilities & MediaCapabilities) != MediaCapabilities.None;
	}

	public bool BindSource(MediaEndpointAddress source, out string error)
	{
		error = "Digital recorders accept their sibling camera automatically.";
		return false;
	}

	public void ClearSourceBinding()
	{
	}

	public void ReceiveMedia(MediaPacket packet)
	{
		if (!Accepts(packet))
		{
			return;
		}
		_latestPacket = packet;
		MediaPacketReceived?.Invoke(this, packet);
	}

	public virtual bool PublishOutput(string endpoint, MediaPacket packet, out string error)
	{
		if (!MediaAvailable || !OutputNames.Append(MediaEndpoint.EndpointKey).Any(x => x.EqualTo(endpoint)))
		{
			error = "That recorder output is not powered and available.";
			return false;
		}
		if (packet.HasVisited(MediaEndpoint))
		{
			error = "That media stream has already visited this recorder output.";
			return false;
		}
		var outgoing = packet with
		{
			Source = MediaEndpoint,
			Sequence = ++_sequence,
			TimestampUtc = DateTime.UtcNow,
			Capabilities = packet.Capabilities & MediaCapabilities,
			Provenance = packet.Provenance.Append(MediaEndpoint).ToArray()
		};
		if (outgoing.Capabilities == MediaCapabilities.None)
		{
			error = "That recording cannot be published through this output.";
			return false;
		}
		Gameworld.MediaChannelService.Publish(outgoing);
		error = string.Empty;
		return true;
	}

	public bool StartRecording(string name, out string error)
	{
		if (IsRecording || IsPlaying)
		{
			error = "That recorder is already recording or playing.";
			return false;
		}
		return Gameworld.ComputerMediaService.StartRecording(this, _recorderPrototype.InputName, name, out error) > 0L;
	}

	public bool StartPlayback(string name, out string error)
	{
		if (IsRecording || IsPlaying)
		{
			error = "That recorder is already recording or playing.";
			return false;
		}
		return Gameworld.ComputerMediaService.StartPlayback(this, name, _recorderPrototype.OutputName, out error) > 0L;
	}

	public bool Stop(out string error)
	{
		var job = Gameworld.ComputerMediaService.GetJobs(this).FirstOrDefault();
		if (job is null)
		{
			error = "That recorder is already stopped.";
			return false;
		}
		return Gameworld.ComputerMediaService.StopJob(this, job.JobId, out error);
	}

	public bool CaptureStill(string name, out string error) =>
		Gameworld.ComputerMediaService.CaptureStill(this, _recorderPrototype.InputName, name, out error);

	public string? GetStill(string name, TimeSpan? offset, out string error)
	{
		var file = RecordingFileSystem.GetFile(name);
		if (file?.Kind != ComputerFileKind.Media || file.MediaRecordingId is null)
		{
			error = "There is no media file with that name on this recorder.";
			return null;
		}
		var scene = Gameworld.MediaRecordingService.GetSceneAt(file.MediaRecordingId.Value, offset ?? TimeSpan.Zero);
		if (scene is null)
		{
			error = "That recording has no video frame at that time.";
			return null;
		}
		error = string.Empty;
		return scene.CanonicalScene;
	}

	protected override void OnPowerCutInAction()
	{
		base.OnPowerCutInAction();
		Gameworld.MediaChannelService.RegisterSink(this);
	}

	protected override void OnPowerCutOutAction()
	{
		InterruptMediaJobs();
		Gameworld.MediaChannelService.UnregisterSink(this);
		_latestPacket = null;
		base.OnPowerCutOutAction();
	}

	public override void Delete()
	{
		InterruptMediaJobs();
		Gameworld.MediaChannelService.UnregisterSink(this);
		base.Delete();
	}

	public override void Quit()
	{
		InterruptMediaJobs();
		Gameworld.MediaChannelService.UnregisterSink(this);
		base.Quit();
	}

	private void InterruptMediaJobs()
	{
		Gameworld.ComputerMediaService.InterruptJobs(this);
		if (MediaConnectedHost is { } host && !ReferenceEquals(host, this))
		{
			Gameworld.ComputerMediaService.InterruptJobs(host, this);
		}
	}
}
