#nullable enable

using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.GameItems.Components;

public class MediaSpeakerGameItemComponent : MediaBoundSinkPoweredComponentBase
{
	private MediaSpeakerGameItemComponentProto _prototype;

	public MediaSpeakerGameItemComponent(MediaSpeakerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public MediaSpeakerGameItemComponent(MudSharp.Models.GameItemComponent component,
		MediaSpeakerGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadRuntimeState(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public MediaSpeakerGameItemComponent(MediaSpeakerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public override MediaEndpointAddress MediaEndpoint => new(Parent.Id, Id, _prototype.EndpointKey, MediaEndpointDirection.Input);
	public override MediaCapabilities MediaCapabilities => MediaCapabilities.Audio;
	public override bool MediaAvailable => IsPowered && Parent.TrueLocations.Any();
	protected override bool AcceptSiblingSources => _prototype.AcceptSiblingSources;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new MediaSpeakerGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (MediaSpeakerGameItemComponentProto)newProto;
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
		DeactivateMediaSink();
	}

	public override void ReceiveMedia(MediaPacket packet)
	{
		if (!MediaAvailable || !packet.Capabilities.HasFlag(MediaCapabilities.Audio))
		{
			return;
		}

		var audioPacket = packet with { Capabilities = MediaCapabilities.Audio };
		Parent.Handle(new MediaPlaybackOutput(Gameworld, Parent, audioPacket), OutputRange.Local);
	}

	private void LoadRuntimeState(XElement root)
	{
		LoadMediaSinkState(root);
	}
}
