#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using MudSharp.Construction;
using MudSharp.Character;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;

namespace MudSharp.Computers;

[Flags]
public enum MediaCapabilities
{
	None = 0,
	Audio = 1,
	Video = 2
}

public enum MediaEndpointDirection
{
	Input = 0,
	Output = 1
}

public enum MediaEventKind
{
	Audio = 0,
	Video = 1,
	AudioVideo = 2,
	SceneSnapshot = 3,
	PlaybackState = 4,
	CrimeWitness = 5
}

public enum MediaRecordingStatus
{
	Recording = 0,
	Finalised = 1,
	Interrupted = 2,
	Failed = 3
}

public enum ComputerFileKind
{
	Text = 0,
	Media = 1
}

public enum ComputerMediaJobKind
{
	Recording = 0,
	Playback = 1,
	RollingRecording = 2,
	SegmentedRecording = 3,
	EventRecording = 4
}

/// <summary>
/// Identifies a media endpoint independently of object reference identity. Item and component identifiers are
/// persisted so a local binding can reconnect after a world load.
/// </summary>
public sealed record MediaEndpointAddress(long ItemId, long ComponentId, string EndpointKey,
	MediaEndpointDirection Direction = MediaEndpointDirection.Output)
{
	public static readonly MediaEndpointAddress Empty = new(0L, 0L, string.Empty, MediaEndpointDirection.Output);

	public bool IsValid => ItemId > 0L && ComponentId > 0L && !string.IsNullOrWhiteSpace(EndpointKey);
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(MediaLanguagePayload), "language")]
[JsonDerivedType(typeof(MediaTextPayload), "text")]
[JsonDerivedType(typeof(MediaScenePayload), "scene")]
[JsonDerivedType(typeof(MediaCrimePayload), "crime")]
public abstract record MediaPayload;

public sealed record MediaLanguagePayload(
	bool IsSigned,
	long LanguageId,
	long AccentOrVarietyId,
	string RawText,
	int Volume,
	int Outcome,
	long? SpeakerCharacterId,
	string SpeakerName,
	short SpeakerGender,
	string PreLanguageEmote,
	string OptionalEmote,
	long? DurationMilliseconds = null) : MediaPayload;

public sealed record MediaTextPayload(string Text, bool IsAudible, bool IsVisual, int? Volume = null) : MediaPayload;

public sealed record MediaScenePayload(string CanonicalScene, string ContentHash) : MediaPayload;

/// <summary>
/// Records that a video sensor had an unobstructed view of a crime at this point in the stream. The crime id is
/// deliberately the only legal detail carried by media; the authoritative crime remains in the law subsystem.
/// </summary>
public sealed record MediaCrimePayload(long CrimeId) : MediaPayload;

/// <summary>
/// A single ordered event on a live media stream. Packets remain immutable after publication so the same data can
/// be routed to recorders, displays and network gateways without per-sink mutation.
/// </summary>
public sealed record MediaPacket(
	Guid StreamId,
	long Sequence,
	DateTime TimestampUtc,
	MediaCapabilities Capabilities,
	MediaEventKind Kind,
	MediaEndpointAddress Source,
	IReadOnlyCollection<MediaEndpointAddress> Provenance,
	MediaPayload Payload)
{
	public bool HasVisited(MediaEndpointAddress endpoint)
	{
		return Provenance.Any(x => x == endpoint);
	}

	public MediaPacket WithProvenance(MediaEndpointAddress endpoint)
	{
		return this with { Provenance = Provenance.Append(endpoint).ToArray() };
	}
}

public sealed record MediaRecordingDescriptor(
	long RecordingId,
	MediaCapabilities Capabilities,
	MediaRecordingStatus Status,
	DateTime CreatedAtUtc,
	DateTime? FinalisedAtUtc,
	TimeSpan Duration,
	long LogicalSizeInBytes,
	string Name);

public sealed record MediaRecordingReference(
	long OwnerGameItemComponentId,
	string Name,
	long RecordingId,
	bool PubliclyAccessible,
	DateTime CreatedAtUtc,
	DateTime LastModifiedAtUtc);

public sealed record MediaRecordingCreateRequest(
	MediaCapabilities Capabilities,
	string Name,
	DateTime StartedAtUtc,
	long OwnerGameItemComponentId);

public sealed record ComputerMediaJobInfo(
	long JobId,
	ComputerMediaJobKind Kind,
	string Endpoint,
	string FileName,
	DateTime StartedAtUtc,
	string Policy);

/// <summary>
/// Persisted, host-owned feed definition. It contains endpoint names and stable account ids only; it never stores
/// credentials or a live object reference.
/// </summary>
public sealed record MediaFeedConfiguration(
	string FeedName,
	string InputName,
	bool IsPublic,
	IReadOnlyCollection<long> AllowedAccountIds);

/// <summary>
/// Persisted subscription configuration. The account id is optional for public feeds and is revalidated every time
/// delivery is attempted.
/// </summary>
public sealed record MediaSubscriptionConfiguration(
	string SubscriptionName,
	long SourceHostItemId,
	string SourceAddress,
	string FeedName,
	string OutputName,
	long? AccountId,
	bool Enabled);

public sealed record ComputerMediaFeedInfo(
	string FeedName,
	string InputName,
	bool IsPublic,
	IReadOnlyCollection<long> AllowedAccountIds,
	bool Active);

public sealed record ComputerMediaSubscriptionInfo(
	string SubscriptionName,
	string SourceAddress,
	string FeedName,
	string OutputName,
	long? AccountId,
	bool Active,
	bool Persisted);

/// <summary>
/// Raised after a sink has accepted a live packet. This deliberately exposes packet metadata only through the
/// normal media pipeline; computer programs that wait for media receive a separate, content-free event dictionary.
/// </summary>
public delegate void MediaPacketDelivered(IMediaSink sink, MediaPacket packet);

public interface IMediaChannelService
{
	event MediaPacketDelivered? PacketDelivered;
	void RegisterSink(IMediaSink sink);
	void UnregisterSink(IMediaSink sink);
	void Publish(MediaPacket packet);
	void CaptureOutput(ILocation location, IOutput output);
	void CaptureCrime(ICrime crime);
	bool AddViewerAsCrimeWitness(ICharacter viewer, MediaPacket packet);
	IEnumerable<IMediaSource> GetSources(ILocation? location = null);
	IEnumerable<IMediaSink> GetSinks(ILocation? location = null);
}

public interface IMediaRecordingService
{
	MediaRecordingDescriptor CreateRecording(MediaRecordingCreateRequest request);
	bool AppendPacket(long recordingId, MediaPacket packet, out string error);
	bool AppendSceneFrame(long recordingId, TimeSpan startOffset, TimeSpan endOffset, string canonicalScene,
		out string error);
	bool FinaliseRecording(long recordingId, MediaRecordingStatus status, DateTime completedAtUtc, out string error);
	MediaRecordingDescriptor? GetRecording(long recordingId);
	IEnumerable<MediaPacket> ReadPackets(long recordingId);
	MediaScenePayload? GetSceneAt(long recordingId, TimeSpan offset);
	IEnumerable<MediaRecordingDescriptor> GetRecordings(long ownerGameItemComponentId);
	IEnumerable<MediaRecordingReference> GetReferences(long ownerGameItemComponentId);
	bool CreateReference(MediaRecordingReference reference, out string error);
	bool DeleteReference(long ownerGameItemComponentId, string name, out string error);
	bool SetReferencePubliclyAccessible(long ownerGameItemComponentId, string name, bool publiclyAccessible, out string error);
	MediaRecordingReference? GetReference(long ownerGameItemComponentId, string name);
}

public interface IComputerMediaService
{
	IEnumerable<string> GetMediaInputs(IComputerHost host);
	IEnumerable<string> GetMediaOutputs(IComputerHost host);
	bool TryResolveMediaInput(IComputerHost host, string input, out MediaEndpointAddress endpoint, out string error);
	bool IsMediaInput(IComputerHost host, MediaEndpointAddress endpoint);
	bool PublishToOutput(IComputerHost host, string output, MediaPacket packet, out string error);
	IEnumerable<ComputerMediaJobInfo> GetJobs(IComputerHost host);
	long StartRecording(IComputerHost host, string input, string fileName, out string error);
	long StartRollingRecording(IComputerHost host, string input, string baseFileName, TimeSpan retention,
		TimeSpan segmentDuration, out string error);
	long StartSegmentedRecording(IComputerHost host, string input, string baseFileName, TimeSpan segmentDuration,
		out string error);
	long StartEventRecording(IComputerHost host, string input, string baseFileName, TimeSpan activeDuration,
		out string error);
	long StartPlayback(IComputerHost host, string fileName, string output, out string error);
	bool CaptureStill(IComputerHost host, string input, string fileName, out string error);
	bool StopJob(IComputerHost host, long jobId, out string error);
	void InterruptJobs(IComputerHost host);
	void InterruptJobs(IComputerHost host, IComputerMediaInterface mediaInterface);
}

public interface IComputerMediaStorageTarget
{
	IComputerFileOwner ActiveMediaStorage { get; }
}

/// <summary>
/// A physical computer host can own durable media feed and subscription configuration. Concrete host components
/// serialize these records alongside their other computer runtime state.
/// </summary>
public interface IComputerMediaConfigurationHost : IComputerHost
{
	IEnumerable<MediaFeedConfiguration> MediaFeeds { get; }
	IEnumerable<MediaSubscriptionConfiguration> MediaSubscriptions { get; }
	bool UpsertMediaFeed(MediaFeedConfiguration configuration, out string error);
	bool RemoveMediaFeed(string feedName, out string error);
	bool UpsertMediaSubscription(MediaSubscriptionConfiguration configuration, out string error);
	bool RemoveMediaSubscription(string subscriptionName, out string error);
}

public interface IComputerMediaNetworkService
{
	IEnumerable<ComputerMediaFeedInfo> GetFeeds(IComputerHost host);
	IEnumerable<ComputerMediaSubscriptionInfo> GetSubscriptions(IComputerHost host);
	bool PublishFeed(IComputerHost host, string input, string feedName, bool isPublic, out string error);
	bool SetFeedAcl(IComputerHost host, string feedName, string accountAddress, bool add, out string error);
	bool SubscribeFeed(IComputerHost subscriberHost, string hostAddress, string feedName, string output,
		IComputerNetworkAccount? account, string? savedSubscriptionName, IComputerTerminalSession? session,
		out string subscriptionName, out string error);
	bool UnsubscribeFeed(IComputerHost subscriberHost, string subscriptionName, out string error);
	bool SubscribeFromProgram(IComputerHost subscriberHost, string addressAndFeed, string output,
		bool savedSubscription, out string error);
	void InterruptSubscriptions(IComputerHost host);
	IEnumerable<string> GetAdvertisedServiceDetails(IComputerHost host, string applicationId);
}
