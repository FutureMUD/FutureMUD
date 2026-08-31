#nullable enable

using MudSharp.Computers;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Audio;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;
using System;
using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces;

public interface IMediaEndpoint : IGameItemComponent
{
	MediaEndpointAddress MediaEndpoint { get; }
	MediaCapabilities MediaCapabilities { get; }
	bool MediaAvailable { get; }
}

public interface IMediaSource : IMediaEndpoint
{
}

public interface IMediaCaptureSource : IMediaSource
{
	bool TryCapture(ILocation location, IOutput output, out MediaPacket packet);
	bool TryCaptureCrime(ICrime crime, out MediaPacket packet);
	string? CaptureCanonicalScene();
}

public interface IMediaSink : IMediaEndpoint
{
	/// <summary>
	/// The endpoint that receives a packet. Source/sink components use a distinct input address so a gateway can
	/// safely receive a stream and publish it again without being mistaken for a feedback loop.
	/// </summary>
	MediaEndpointAddress MediaInputEndpoint => MediaEndpoint;
	bool Accepts(MediaPacket packet);
	void ReceiveMedia(MediaPacket packet);
}

/// <summary>
/// A sink that can persist an explicit local source endpoint. Implementations use this for direct media connectors
/// and for component siblings in a composite device; packet routing itself remains entirely separate from the
/// numeric automation-signal bus.
/// </summary>
public interface IMediaBoundSink : IMediaSink
{
	MediaEndpointAddress? SourceBinding { get; }
	bool BindSource(MediaEndpointAddress source, out string error);
	void ClearSourceBinding();
}

public interface IMediaStorageMedium : IGameItemComponent
{
	string FormatKey { get; }
	MediaCapabilities MediaCapabilities { get; }
	TimeSpan Capacity { get; }
	TimeSpan UsedCapacity { get; }
	TimeSpan RemainingCapacity { get; }
	bool WriteProtected { get; set; }
	IReadOnlyCollection<MediaRecordingReference> Recordings { get; }
	bool HasRecording(string name);
	MediaRecordingReference? GetRecording(string name);
	bool CanStoreRecording(MediaRecordingDescriptor recording, out string error);
	bool StoreRecording(MediaRecordingDescriptor recording, out string error);
	bool DeleteRecording(string name, out string error);
}

public interface IMediaAudioSink : IMediaBoundSink
{
	AudioVolume OutputVolume { get; }
	bool SetOutputVolume(AudioVolume volume, out string error);
}

public interface IMediaMonitor : IMediaAudioSink
{
	bool AmbientPresentation { get; }
	bool AudioEnabled { get; }
	string? LatestFrame { get; }
	bool Watch(ICharacter actor, out string error);
	bool StopWatching(ICharacter actor);
}

public interface IMediaDeck : IGameItemComponent
{
	MediaCapabilities MediaCapabilities { get; }
	string FormatKey { get; }
	bool CanRecord { get; }
	bool CanPlayback { get; }
	bool IsRecording { get; }
	bool IsPlaying { get; }
	bool StartRecording(string name, out string error);
	bool StartPlayback(string name, out string error);
	bool Stop(out string error);
}

public interface IDigitalMediaRecorder : IMediaDeck, IComputerFileOwner
{
	IComputerFileSystem RecordingFileSystem { get; }
	bool CaptureStill(string name, out string error);
	string? GetStill(string name, TimeSpan? offset, out string error);
	IEnumerable<IComputerFile> MediaFiles { get; }
}

public delegate void ComputerMediaPacketReceived(IComputerMediaInterface mediaInterface, MediaPacket packet);

public interface IComputerMediaInterface : IMediaSink, IMediaSource
{
	IComputerHost? ConnectedHost { get; }
	IEnumerable<string> InputNames { get; }
	IEnumerable<string> OutputNames { get; }
	MediaPacket? LatestPacket { get; }
	event ComputerMediaPacketReceived? MediaPacketReceived;
	bool PublishOutput(string endpoint, MediaPacket packet, out string error);
}
