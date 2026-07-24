using MudSharp.GameItems.Prototypes;

#nullable enable

namespace MudSharp.GameItems.Components;

public class BayonetAttachmentGameItemComponent : GameItemComponent, IBayonetAttachment
{
	private BayonetAttachmentGameItemComponentProto _prototype;

	public BayonetAttachmentGameItemComponent(BayonetAttachmentGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public BayonetAttachmentGameItemComponent(MudSharp.Models.GameItemComponent component,
		BayonetAttachmentGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
	}

	public BayonetAttachmentGameItemComponent(BayonetAttachmentGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public BayonetAttachmentStyle Style => _prototype.Style;
	public double MinimumBore => _prototype.MinimumBore;
	public double MaximumBore => _prototype.MaximumBore;
	public bool BlocksFiring => _prototype.BlocksFiring;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BayonetAttachmentGameItemComponentProto)newProto;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BayonetAttachmentGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}
}
