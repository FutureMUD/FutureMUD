#nullable enable

using MudSharp.Computers;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class MediaCableGameItemComponent : PassiveMediaRelayGameItemComponentBase
{
	private MediaCableGameItemComponentProto _prototype;

	public MediaCableGameItemComponent(MediaCableGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public MediaCableGameItemComponent(MudSharp.Models.GameItemComponent component,
		MediaCableGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadRuntimeState(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public MediaCableGameItemComponent(MediaCableGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public override MediaEndpointAddress MediaEndpoint => new(Parent.Id, Id, $"{_prototype.EndpointKey}:out", MediaEndpointDirection.Output);
	public override MediaEndpointAddress MediaInputEndpoint => new(Parent.Id, Id, $"{_prototype.EndpointKey}:in", MediaEndpointDirection.Input);
	public override MediaCapabilities MediaCapabilities => _prototype.Capabilities;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new MediaCableGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (MediaCableGameItemComponentProto)newProto;
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

	private void LoadRuntimeState(XElement root)
	{
		LoadMediaSinkState(root);
	}
}
