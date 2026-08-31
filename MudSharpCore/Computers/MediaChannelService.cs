#nullable enable

using MudSharp.Construction;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;

namespace MudSharp.Computers;

/// <summary>
/// Routes typed media packets without using the numeric automation signal bus. Sources remain independent from
/// recorders and displays; sinks decide which explicit source endpoint they accept.
/// </summary>
public sealed class MediaChannelService : IMediaChannelService
{
	private const int MaximumProvenanceHops = 16;
	private readonly object _sync = new();
	private readonly IFuturemud _gameworld;
	private readonly HashSet<IMediaSink> _sinks = [];

	public MediaChannelService(IFuturemud gameworld)
	{
		_gameworld = gameworld;
	}

	public event MediaPacketDelivered? PacketDelivered;

	public void RegisterSink(IMediaSink sink)
	{
		lock (_sync)
		{
			_sinks.Add(sink);
		}
	}

	public void UnregisterSink(IMediaSink sink)
	{
		lock (_sync)
		{
			_sinks.Remove(sink);
		}
	}

	public void Publish(MediaPacket packet)
	{
		if (!packet.Source.IsValid || packet.Source.Direction != MediaEndpointDirection.Output ||
		    packet.Provenance.Count >= MaximumProvenanceHops)
		{
			return;
		}

		IMediaSink[] sinks;
		lock (_sync)
		{
			sinks = _sinks.ToArray();
		}

		foreach (var sink in sinks)
		{
			if (!sink.MediaAvailable || packet.HasVisited(sink.MediaInputEndpoint) || !sink.Accepts(packet))
			{
				continue;
			}

			var delivered = packet.WithProvenance(sink.MediaInputEndpoint);
			sink.ReceiveMedia(delivered);
			PacketDelivered?.Invoke(sink, delivered);
		}
	}

	public void CaptureOutput(ILocation location, IOutput output)
	{
		if (location is null || output is null || output.Visibility != OutputVisibility.Normal ||
		    output.Flags.HasFlag(OutputFlags.WizOnly) || output.Flags.HasFlag(OutputFlags.IgnoreWatchers))
		{
			return;
		}

		foreach (var source in GetSources(location).OfType<IMediaCaptureSource>())
		{
			if (!source.MediaAvailable || !source.TryCapture(location, output, out var packet))
			{
				continue;
			}

			Publish(packet);
		}
	}

	public void CaptureCrime(ICrime crime)
	{
		if (crime.CrimeLocation is null)
		{
			return;
		}

		var packets = GetSources(crime.CrimeLocation)
			.OfType<IMediaCaptureSource>()
			.Select(source => source.TryCaptureCrime(crime, out var packet) ? packet : null)
			.Where(packet => packet is not null)
			.Cast<MediaPacket>()
			.ToList();
		if (!packets.Any())
		{
			return;
		}

		// A persisted stream cannot safely refer to a late-initialising crime with id zero. Only filmed crimes pay
		// this synchronous initialisation cost, preserving packet order and durable referential provenance.
		if (crime is ILateInitialisingItem { IdHasBeenRegistered: false } lateCrime)
		{
			_gameworld.SaveManager.DirectInitialise(lateCrime);
		}

		if (crime.Id <= 0L)
		{
			return;
		}

		foreach (var packet in packets)
		{
			Publish(packet with { Payload = new MediaCrimePayload(crime.Id) });
		}
	}

	public bool AddViewerAsCrimeWitness(ICharacter viewer, MediaPacket packet)
	{
		if (!packet.Capabilities.HasFlag(MediaCapabilities.Video) ||
		    packet.Payload is not MediaCrimePayload { CrimeId: > 0L } payload)
		{
			return false;
		}

		// All<T> exposes late-initialising items through enumeration before their durable id has been copied into
		// its keyed lookup. Live camera delivery can occur in exactly that narrow registration window.
		var crime = _gameworld.Crimes.Get(payload.CrimeId) ??
		            _gameworld.Crimes.FirstOrDefault(x => x.Id == payload.CrimeId);
		var viewerId = CharacterInstanceIdentityComparer.IdentityId(viewer);
		if (crime is null || viewerId == crime.CriminalId || crime.WitnessIds.Contains(viewerId))
		{
			return false;
		}

		crime.AddWitness(viewerId);
		viewer.HandleEvent(EventType.WitnessedCrime, crime.Criminal, crime.Victim, viewer, crime);
		return true;
	}

	public IEnumerable<IMediaSource> GetSources(ILocation? location = null)
	{
		return _gameworld.Items
			.SelectMany(x => x.GetItemTypes<IMediaSource>())
			.Where(x => location is null || x.Parent.TrueLocations.Any(y => ReferenceEquals(y, location)))
			.ToList();
	}

	public IEnumerable<IMediaSink> GetSinks(ILocation? location = null)
	{
		lock (_sync)
		{
			return _sinks
				.Where(x => location is null || x.Parent.TrueLocations.Any(y => ReferenceEquals(y, location)))
				.ToList();
		}
	}
}
