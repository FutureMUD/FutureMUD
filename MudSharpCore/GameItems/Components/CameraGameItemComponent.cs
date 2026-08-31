#nullable enable

using MudSharp.Communication.Language;
using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Form.Audio;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;

namespace MudSharp.GameItems.Components;

public class CameraGameItemComponent : MediaEndpointPoweredComponentBase, IMediaCaptureSource
{
	private CameraGameItemComponentProto _prototype;
	private Guid _streamId = Guid.NewGuid();
	private long _sequence;
	private DateTime _lastSnapshotAtUtc;
	private bool _snapshotHeartbeatSubscribed;

	public CameraGameItemComponent(CameraGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public CameraGameItemComponent(MudSharp.Models.GameItemComponent component, CameraGameItemComponentProto proto,
		IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadRuntimeState(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public CameraGameItemComponent(CameraGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_streamId = Guid.NewGuid();
		_sequence = rhs._sequence;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public MediaEndpointAddress MediaEndpoint => new(Parent.Id, Id, _prototype.EndpointKey, MediaEndpointDirection.Output);
	public MediaCapabilities MediaCapabilities => _prototype.Capabilities;
	public bool MediaAvailable => IsPowered && Parent.TrueLocations.Any();
	protected override int MediaOutputPorts => _prototype.OutputPorts;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new CameraGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (CameraGameItemComponentProto)newProto;
	}

	protected override XElement SaveToXml(XElement root)
	{
		root.Add(new XElement("LastSnapshotAtUtc", _lastSnapshotAtUtc.ToString("O")));
		return SaveMediaEndpointState(root);
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		LoadRuntimeState(root);
	}

	protected override void OnPowerCutInAction()
	{
		if (!_snapshotHeartbeatSubscribed)
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += SnapshotHeartbeat;
			_snapshotHeartbeatSubscribed = true;
		}

		PublishSnapshotIfConsumed();
	}

	protected override void OnPowerCutOutAction()
	{
		ReleaseSnapshotHeartbeat();
	}

	public override void Delete()
	{
		ReleaseSnapshotHeartbeat();
		base.Delete();
	}

	public override void Quit()
	{
		ReleaseSnapshotHeartbeat();
		base.Quit();
	}

	public bool TryCapture(ILocation location, IOutput output, out MediaPacket packet)
	{
		packet = null!;
		if (!MediaAvailable || !Parent.TrueLocations.Any(x => ReferenceEquals(x, location)))
		{
			return false;
		}

		if (location is not ICell cell)
		{
			return false;
		}

		var sensor = new MediaSensorPerceiver(Parent, cell, _prototype.SensorSensitivity);
		if (!output.ShouldSee(sensor))
		{
			return false;
		}

		if (output is IMediaPacketOutput mediaOutput)
		{
			var sourcePacket = mediaOutput.MediaPacket;
			if (sourcePacket.HasVisited(MediaEndpoint) || sourcePacket.Source == MediaEndpoint ||
			    (sourcePacket.Capabilities & MediaCapabilities) == MediaCapabilities.None)
			{
				if (MediaCapabilities.HasFlag(MediaCapabilities.Audio) &&
				    MediaComponentUtilities.IsLoudFeedbackLoop(sourcePacket, MediaEndpoint))
				{
					MediaAudioPresentation.EmitFeedback(mediaOutput.PresentationSource as IGameItem ?? Parent);
				}

				return false;
			}

			packet = sourcePacket with
			{
				Sequence = NextSequence(),
				TimestampUtc = DateTime.UtcNow,
				Capabilities = sourcePacket.Capabilities & MediaCapabilities,
				Source = MediaEndpoint,
				Provenance = sourcePacket.Provenance.Append(MediaEndpoint).ToArray()
			};
			return true;
		}

		var source = GetOutputSource(output);
		if (MediaComponentUtilities.IsAudible(output) && source is not null && !sensor.CanHear(source))
		{
			return false;
		}

		if (MediaComponentUtilities.GetAudioVolume(output) is { } volume && source is not null &&
		    cell.LocalAudioDifficulty(sensor, volume, Parent.GetProximity(source)) == Difficulty.Impossible)
		{
			return false;
		}

		if (output is IRecordableLanguageOutput languageOutput)
		{
			var signed = languageOutput.LanguageInfo.Form == LanguageForm.Signed;
			var capabilities = signed ? MediaCapabilities.Video : MediaCapabilities.Audio;
			if ((capabilities & MediaCapabilities) == MediaCapabilities.None)
			{
				return false;
			}

			packet = CreatePacket(capabilities, signed ? MediaEventKind.Video : MediaEventKind.Audio,
				MediaComponentUtilities.CreateLanguagePayload(languageOutput));
			return true;
		}

		var requestedCapabilities =
			(MediaComponentUtilities.IsAudible(output) ? MediaCapabilities.Audio : MediaCapabilities.None) |
			(MediaComponentUtilities.IsVisual(output) ? MediaCapabilities.Video : MediaCapabilities.None);
		requestedCapabilities &= MediaCapabilities;
		if (requestedCapabilities == MediaCapabilities.None)
		{
			return false;
		}

		var kind = requestedCapabilities == (MediaCapabilities.Audio | MediaCapabilities.Video)
			? MediaEventKind.AudioVideo
			: requestedCapabilities == MediaCapabilities.Audio
				? MediaEventKind.Audio
				: MediaEventKind.Video;
		packet = CreatePacket(requestedCapabilities, kind,
			new MediaTextPayload(output.ParseFor(sensor), requestedCapabilities.HasFlag(MediaCapabilities.Audio),
				requestedCapabilities.HasFlag(MediaCapabilities.Video),
				requestedCapabilities.HasFlag(MediaCapabilities.Audio)
					? (int)(MediaComponentUtilities.GetAudioVolume(output) ?? AudioVolume.Decent)
					: null));
		return true;
	}

	public bool TryCaptureCrime(ICrime crime, out MediaPacket packet)
	{
		packet = null!;
		if (!MediaAvailable || !MediaCapabilities.HasFlag(MediaCapabilities.Video) ||
		    crime.CrimeLocation is not { } crimeLocation ||
		    !Parent.TrueLocations.Any(x => ReferenceEquals(x, crimeLocation)))
		{
			return false;
		}

		var sensor = new MediaSensorPerceiver(Parent, crimeLocation, _prototype.SensorSensitivity);
		if (!sensor.CanSee(crime.Criminal))
		{
			return false;
		}

		packet = CreatePacket(MediaCapabilities.Video, MediaEventKind.CrimeWitness, new MediaCrimePayload(0L)) with
		{
			TimestampUtc = crime.RealTimeOfCrime
		};
		return true;
	}

	public string? CaptureCanonicalScene()
	{
		if (!MediaAvailable || !MediaCapabilities.HasFlag(MediaCapabilities.Video))
		{
			return null;
		}

		var cell = Parent.TrueLocations.FirstOrDefault();
		if (cell is null)
		{
			return null;
		}

		var sensor = new MediaSensorPerceiver(Parent, cell, _prototype.SensorSensitivity);
		return cell.HowSeen(sensor, type: DescriptionType.Full, colour: false,
			flags: PerceiveIgnoreFlags.IgnoreNamesSetting | PerceiveIgnoreFlags.IgnoreLiquidsAndFlags)
			.NormaliseSpacing();
	}

	private void SnapshotHeartbeat()
	{
		if (DateTime.UtcNow - _lastSnapshotAtUtc < _prototype.SnapshotInterval)
		{
			return;
		}

		PublishSnapshotIfConsumed();
	}

	private void PublishSnapshotIfConsumed()
	{
		if (!MediaAvailable || !MediaCapabilities.HasFlag(MediaCapabilities.Video))
		{
			return;
		}

		var scene = CaptureCanonicalScene();
		if (string.IsNullOrWhiteSpace(scene))
		{
			return;
		}

		var packet = CreatePacket(MediaCapabilities.Video, MediaEventKind.SceneSnapshot,
			new MediaScenePayload(scene, string.Empty));
		var location = Parent.TrueLocations.FirstOrDefault();
		if (location is null || !Gameworld.MediaChannelService.GetSinks().Any(x => x.Accepts(packet)))
		{
			return;
		}

		_lastSnapshotAtUtc = DateTime.UtcNow;
		Gameworld.MediaChannelService.Publish(packet);
	}

	internal void PublishCurrentSnapshot()
	{
		PublishSnapshotIfConsumed();
	}

	private MediaPacket CreatePacket(MediaCapabilities capabilities, MediaEventKind kind, MediaPayload payload)
	{
		return new MediaPacket(_streamId, NextSequence(), DateTime.UtcNow, capabilities, kind, MediaEndpoint,
			[MediaEndpoint], payload);
	}

	private long NextSequence()
	{
		return ++_sequence;
	}

	private static IPerceivable? GetOutputSource(IOutput output)
	{
		return output switch
		{
			IRecordableLanguageOutput language => language.DefaultSource,
			IEmoteOutput emote => emote.DefaultSource,
			_ => null
		};
	}

	private void ReleaseSnapshotHeartbeat()
	{
		if (!_snapshotHeartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= SnapshotHeartbeat;
		_snapshotHeartbeatSubscribed = false;
	}

	private void LoadRuntimeState(XElement root)
	{
		LoadMediaEndpointState(root);
		_lastSnapshotAtUtc = DateTime.TryParse(root.Element("LastSnapshotAtUtc")?.Value, out var snapshotAt)
			? snapshotAt.ToUniversalTime()
			: DateTime.MinValue;
	}
}
