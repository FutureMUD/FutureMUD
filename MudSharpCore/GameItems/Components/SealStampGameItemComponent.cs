#nullable enable

using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class SealStampGameItemComponent : GameItemComponent, ISealStamp
{
	private SealStampGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (SealStampGameItemComponentProto)newProto;
	}

	public SealStampGameItemComponent(SealStampGameItemComponentProto proto, IGameItem parent, bool temporary = false) :
		base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public SealStampGameItemComponent(MudSharp.Models.GameItemComponent component, SealStampGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public SealStampGameItemComponent(SealStampGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	private void LoadFromXml(XElement root)
	{
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SealStampGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	public string SealDesign => _prototype.SealDesign;
	public string IssuerText => _prototype.IssuerText;
	public string OwnerText => _prototype.OwnerText;
	public string ClanText => _prototype.ClanText;
	public string OfficeText => _prototype.OfficeText;
	public string StampMaterial => _prototype.StampMaterial;
	public Difficulty ForgeryDifficulty => _prototype.ForgeryDifficulty;

	public bool CanSeal(ICharacter actor, IGameItem target, IGameItem? medium, out string error)
	{
		return _prototype.CanSeal(actor, Parent, target, medium, out error);
	}

	public SealImpression CreateImpression(ICharacter actor, IGameItem target, IGameItem? medium)
	{
		return new SealImpression(
			SealDesign,
			IssuerText,
			OwnerText,
			ClanText,
			OfficeText,
			string.IsNullOrWhiteSpace(StampMaterial) ? Parent.Material?.Name ?? string.Empty : StampMaterial,
			ForgeryDifficulty,
			CharacterInstanceIdentityComparer.IdentityId(actor),
			actor.HowSeen(actor),
			DateTime.UtcNow,
			medium?.HowSeen(actor) ?? string.Empty);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Full or DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		return type switch
		{
			DescriptionType.Full => $"{description}\n\nIt bears a seal design of {SealDesign.ColourValue()}.",
			DescriptionType.Evaluate => $"It is a seal stamp with design {SealDesign.ColourValue()} and {(string.IsNullOrWhiteSpace(StampMaterial) ? "unspecified" : StampMaterial).ColourValue()} material metadata.",
			_ => description
		};
	}
}
