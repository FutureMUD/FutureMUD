using MudSharp.GameItems.Prototypes;

#nullable enable

namespace MudSharp.GameItems.Components;

public class FirearmAttachmentGameItemComponent : GameItemComponent, IFirearmAttachment
{
	private FirearmAttachmentGameItemComponentProto _prototype;

	public FirearmAttachmentGameItemComponent(FirearmAttachmentGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public FirearmAttachmentGameItemComponent(MudSharp.Models.GameItemComponent component,
		FirearmAttachmentGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
	}

	public FirearmAttachmentGameItemComponent(FirearmAttachmentGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public FirearmAttachmentSlotType SlotType => _prototype.SlotType;
	public IReadOnlyCollection<string> FormFactors => _prototype.FormFactors;
	public FirearmAttachmentModifiers Modifiers => _prototype.Modifiers;
	public string? FireEmote => _prototype.FireEmote;
	public IFirearmAttachmentHost? InstalledIn { get; set; }

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (FirearmAttachmentGameItemComponentProto)newProto;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new FirearmAttachmentGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}
}
