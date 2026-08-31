#nullable enable

using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Audio;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.GameItems.Components;

public class MediaMonitorGameItemComponent : MediaBoundSinkPoweredComponentBase, IMediaMonitor
{
	private readonly HashSet<ICharacter> _watchers = [];
	private MediaMonitorGameItemComponentProto _prototype;
	private string? _latestFrame;
	private AudioVolume? _outputVolumeOverride;

	public MediaMonitorGameItemComponent(MediaMonitorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public MediaMonitorGameItemComponent(MudSharp.Models.GameItemComponent component,
		MediaMonitorGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadRuntimeState(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public MediaMonitorGameItemComponent(MediaMonitorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_latestFrame = rhs._latestFrame;
		_outputVolumeOverride = rhs._outputVolumeOverride;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public override MediaEndpointAddress MediaEndpoint => new(Parent.Id, Id, _prototype.EndpointKey, MediaEndpointDirection.Input);
	public override MediaCapabilities MediaCapabilities => _prototype.Capabilities;
	public override bool MediaAvailable => IsPowered && Parent.TrueLocations.Any();
	public bool AmbientPresentation => _prototype.AmbientPresentation;
	public bool AudioEnabled => _prototype.AudioEnabled && MediaCapabilities.HasFlag(MediaCapabilities.Audio);
	public AudioVolume OutputVolume => _outputVolumeOverride ?? _prototype.OutputVolume;
	public string? LatestFrame => _latestFrame;
	protected override bool AcceptSiblingSources => _prototype.AcceptSiblingSources;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new MediaMonitorGameItemComponent(this, newParent, temporary);
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

		var sb = new StringBuilder(description);
		sb.AppendLine();
		sb.AppendLine();
		sb.AppendLine($"Its monitor is {(MediaAvailable ? "powered".ColourValue() : "not powered".ColourError())} and uses {(AmbientPresentation ? "ambient".ColourValue() : "opt-in".ColourCommand())} presentation.");
		if (AudioEnabled)
		{
			sb.AppendLine($"Its audio output is set to {OutputVolume.DescribeEnum().ColourValue()}.");
		}
		if (SourceBinding is { } binding)
		{
			sb.AppendLine($"It is connected to media endpoint {binding.EndpointKey.ColourName()} on item #{binding.ItemId.ToString("N0", voyeur).ColourValue()}.");
		}
		else if (AcceptSiblingSources)
		{
			sb.AppendLine("It is accepting media from source components on the same composite item.");
		}
		else
		{
			sb.AppendLine("It is not connected to a media source.");
		}

		if (!string.IsNullOrWhiteSpace(_latestFrame))
		{
			sb.AppendLine();
			sb.AppendLine("Its current frame shows:");
			sb.AppendLine(_latestFrame.Wrap(voyeur.InnerLineFormatLength));
		}

		return sb.ToString();
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (MediaMonitorGameItemComponentProto)newProto;
	}

	protected override XElement SaveToXml(XElement root)
	{
		if (_outputVolumeOverride is { } volume)
		{
			root.Add(new XElement("OutputVolumeOverride", (int)volume));
		}

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
		DeactivateMediaSink();
		_watchers.Clear();
	}

	public override void ReceiveMedia(MediaPacket packet)
	{
		if (!MediaAvailable)
		{
			return;
		}

		var visibleCapabilities = packet.Capabilities & MediaCapabilities;
		if (!AudioEnabled)
		{
			visibleCapabilities &= ~MediaCapabilities.Audio;
		}

		if (visibleCapabilities == MediaCapabilities.None)
		{
			return;
		}

		if (packet.Payload is MediaCrimePayload)
		{
			if (!visibleCapabilities.HasFlag(MediaCapabilities.Video))
			{
				return;
			}

			var crimeOutput = new MediaPlaybackOutput(Gameworld, Parent,
				packet with { Capabilities = visibleCapabilities });
			var viewers = AmbientPresentation
				? Parent.TrueLocations
					.SelectMany(x => x.CharactersInSpatialVicinity(Parent))
					.Distinct()
					.Where(x => !x.AffectedBy<LinkdeadLogout>())
					.ToList()
				: _watchers.Where(x => !x.State.IsUnconscious() && !x.AffectedBy<LinkdeadLogout>()).ToList();
			foreach (var viewer in viewers.Where(crimeOutput.ShouldSee))
			{
				Gameworld.MediaChannelService.AddViewerAsCrimeWitness(viewer, packet);
			}

			return;
		}

		if (packet.Payload is MediaScenePayload scene)
		{
			if (visibleCapabilities.HasFlag(MediaCapabilities.Video))
			{
				_latestFrame = scene.CanonicalScene;
			}

			return;
		}

		var presented = MediaAudioPresentation.ApplyOutputVolume(
			packet with { Capabilities = visibleCapabilities }, OutputVolume);
		if (presented.Capabilities == MediaCapabilities.None)
		{
			return;
		}

		var output = new MediaPlaybackOutput(Gameworld, Parent, presented);
		if (AmbientPresentation)
		{
			Parent.Handle(output, OutputRange.Local);
			MediaAudioPresentation.EmitPlaybackNoise(Parent, presented);
			return;
		}

		foreach (var watcher in _watchers.Where(x => !x.State.IsUnconscious()).ToList())
		{
			watcher.OutputHandler?.Send(output, !output.Style.HasFlag(OutputStyle.NoNewLine),
				!output.Style.HasFlag(OutputStyle.NoPage));
		}

		MediaAudioPresentation.EmitPlaybackNoise(Parent, presented);
	}

	public bool SetOutputVolume(AudioVolume volume, out string error)
	{
		if (!Enum.IsDefined(volume))
		{
			error = "That is not a valid audio volume.";
			return false;
		}

		_outputVolumeOverride = volume;
		Changed = true;
		error = string.Empty;
		return true;
	}

	public bool Watch(ICharacter actor, out string error)
	{
		if (!MediaAvailable)
		{
			error = $"{Parent.HowSeen(actor)} is not powered.";
			return false;
		}

		_watchers.Add(actor);
		if (!string.IsNullOrWhiteSpace(_latestFrame))
		{
			actor.OutputHandler?.Send(_latestFrame);
		}

		error = string.Empty;
		return true;
	}

	public bool StopWatching(ICharacter actor)
	{
		return _watchers.Remove(actor);
	}

	private void LoadRuntimeState(XElement root)
	{
		LoadMediaSinkState(root);
		_outputVolumeOverride = int.TryParse(root.Element("OutputVolumeOverride")?.Value, out var rawVolume) &&
		                        Enum.IsDefined(typeof(AudioVolume), rawVolume)
			? (AudioVolume)rawVolume
			: null;
	}
}
