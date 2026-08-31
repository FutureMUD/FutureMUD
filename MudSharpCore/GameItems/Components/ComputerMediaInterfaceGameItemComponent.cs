#nullable enable

using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class ComputerMediaInterfaceGameItemComponent : MediaBoundSinkPoweredComponentBase, IComputerMediaInterface
{
	private ComputerMediaInterfaceGameItemComponentProto _prototype;
	private long _sequence;
	private MediaPacket? _latestPacket;

	public ComputerMediaInterfaceGameItemComponent(ComputerMediaInterfaceGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public ComputerMediaInterfaceGameItemComponent(MudSharp.Models.GameItemComponent component,
		ComputerMediaInterfaceGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadRuntimeState(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ComputerMediaInterfaceGameItemComponent(ComputerMediaInterfaceGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_sequence = rhs._sequence;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public override MediaEndpointAddress MediaEndpoint => new(Parent.Id, Id, $"{_prototype.EndpointKey}:out", MediaEndpointDirection.Output);
	public override MediaEndpointAddress MediaInputEndpoint => new(Parent.Id, Id, $"{_prototype.EndpointKey}:in", MediaEndpointDirection.Input);
	public override MediaCapabilities MediaCapabilities => _prototype.Capabilities;
	public override bool MediaAvailable => IsPowered && ConnectedHost?.Powered == true && Parent.TrueLocations.Any();
	public IComputerHost? ConnectedHost => Parent.Components.OfType<IComputerHost>().FirstOrDefault();
	public IEnumerable<string> InputNames => [_prototype.InputName];
	public IEnumerable<string> OutputNames => [_prototype.OutputName];
	public MediaPacket? LatestPacket => _latestPacket;
	protected override bool AcceptSiblingSources => _prototype.AcceptSiblingSources;
	public event ComputerMediaPacketReceived? MediaPacketReceived;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ComputerMediaInterfaceGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (ComputerMediaInterfaceGameItemComponentProto)newProto;
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
		_latestPacket = null;
		DeactivateMediaSink();
	}

	public override void ReceiveMedia(MediaPacket packet)
	{
		if (!MediaAvailable)
		{
			return;
		}

		_latestPacket = packet;
		MediaPacketReceived?.Invoke(this, packet);
	}

	public bool PublishOutput(string endpoint, MediaPacket packet, out string error)
	{
		if (!MediaAvailable)
		{
			error = "That computer media interface is not currently powered and available.";
			return false;
		}

		if (!OutputNames.Any(x => x.Equals(endpoint, StringComparison.InvariantCultureIgnoreCase)) &&
		    !MediaEndpoint.EndpointKey.Equals(endpoint, StringComparison.InvariantCultureIgnoreCase))
		{
			error = "That is not an output on this computer media interface.";
			return false;
		}

		if (packet.HasVisited(MediaEndpoint))
		{
			error = "That media stream has already visited this output endpoint.";
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
			error = "That output cannot carry this recording's media type.";
			return false;
		}

		Gameworld.MediaChannelService.Publish(outgoing);
		error = string.Empty;
		return true;
	}

	private void LoadRuntimeState(XElement root)
	{
		LoadMediaSinkState(root);
	}
}
