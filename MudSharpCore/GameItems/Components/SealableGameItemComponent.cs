#nullable enable

using MudSharp.Events;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class SealableGameItemComponent : GameItemComponent, ISealable
{
	private SealableGameItemComponentProto _prototype;
	private bool _isSealed;
	private bool _sealBroken;
	private bool _hasSealResidue;

	public override IGameItemComponentProto Prototype => _prototype;
	public bool IsSealed => _isSealed;
	public bool SealBroken => _sealBroken;
	public bool HasSealResidue => _hasSealResidue;
	public SealImpression? CurrentSeal { get; private set; }
	public Difficulty InspectionDifficulty => _prototype.InspectionDifficulty;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (SealableGameItemComponentProto)newProto;
	}

	public SealableGameItemComponent(SealableGameItemComponentProto proto, IGameItem parent, bool temporary = false) :
		base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public SealableGameItemComponent(MudSharp.Models.GameItemComponent component, SealableGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public SealableGameItemComponent(SealableGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_isSealed = rhs._isSealed;
		_sealBroken = rhs._sealBroken;
		_hasSealResidue = rhs._hasSealResidue;
		CurrentSeal = rhs.CurrentSeal;
	}

	private void LoadFromXml(XElement root)
	{
		_isSealed = bool.Parse(root.Element("IsSealed")?.Value ?? "false");
		_sealBroken = bool.Parse(root.Element("SealBroken")?.Value ?? "false");
		_hasSealResidue = bool.Parse(root.Element("HasSealResidue")?.Value ?? "false");
		var seal = root.Element("Seal");
		if (seal is null)
		{
			return;
		}

		CurrentSeal = new SealImpression(
			seal.Element("SealDesign")?.Value ?? string.Empty,
			seal.Element("IssuerText")?.Value ?? string.Empty,
			seal.Element("OwnerText")?.Value ?? string.Empty,
			seal.Element("ClanText")?.Value ?? string.Empty,
			seal.Element("OfficeText")?.Value ?? string.Empty,
			seal.Element("StampMaterial")?.Value ?? string.Empty,
			(Difficulty)int.Parse(seal.Element("ForgeryDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString()),
			long.Parse(seal.Element("SealingCharacterId")?.Value ?? "0"),
			seal.Element("SealingCharacterName")?.Value ?? string.Empty,
			DateTime.Parse(seal.Element("SealedAt")?.Value ?? DateTime.MinValue.ToString("O")),
			seal.Element("SealMedium")?.Value ?? string.Empty);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SealableGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("IsSealed", _isSealed),
			new XElement("SealBroken", _sealBroken),
			new XElement("HasSealResidue", _hasSealResidue),
			CurrentSeal is null ? null :
				new XElement("Seal",
					new XElement("SealDesign", new XCData(CurrentSeal.SealDesign)),
					new XElement("IssuerText", new XCData(CurrentSeal.IssuerText)),
					new XElement("OwnerText", new XCData(CurrentSeal.OwnerText)),
					new XElement("ClanText", new XCData(CurrentSeal.ClanText)),
					new XElement("OfficeText", new XCData(CurrentSeal.OfficeText)),
					new XElement("StampMaterial", new XCData(CurrentSeal.StampMaterial)),
					new XElement("ForgeryDifficulty", (int)CurrentSeal.ForgeryDifficulty),
					new XElement("SealingCharacterId", CurrentSeal.SealingCharacterId),
					new XElement("SealingCharacterName", new XCData(CurrentSeal.SealingCharacterName)),
					new XElement("SealedAt", CurrentSeal.SealedAt.ToString("O")),
					new XElement("SealMedium", new XCData(CurrentSeal.SealMedium)))).ToString();
	}

	public bool CanSeal(ICharacter actor, ISealStamp stamp, IGameItem? medium, out string error)
	{
		if (_isSealed)
		{
			error = $"{Parent.HowSeen(actor, true)} is already sealed.";
			return false;
		}

		if (!_prototype.IsAllowedMedium(medium))
		{
			error = $"{medium?.HowSeen(actor, true) ?? "That"} is not an allowed seal medium for {Parent.HowSeen(actor)}.";
			return false;
		}

		if (!stamp.CanSeal(actor, Parent, medium, out error))
		{
			return false;
		}

		error = string.Empty;
		return true;
	}

	public void Seal(ICharacter actor, ISealStamp stamp, IGameItem? medium)
	{
		CurrentSeal = stamp.CreateImpression(actor, Parent, medium);
		_isSealed = true;
		_sealBroken = false;
		_hasSealResidue = false;
		Changed = true;
	}

	public bool BreakSeal(ICharacter? actor, string reason)
	{
		if (!_isSealed)
		{
			return false;
		}

		_isSealed = false;
		_sealBroken = true;
		_hasSealResidue = _prototype.BrokenSealLeavesResidue;
		Changed = true;

		if (actor is not null)
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ break|breaks the seal on $0.", actor, Parent)));
		}
		else
		{
			Parent.Handle("The seal breaks.");
		}

		return true;
	}

	public string InspectSeal(ICharacter actor)
	{
		if (CurrentSeal is null)
		{
			return _hasSealResidue
				? $"{Parent.HowSeen(actor, true)} has broken seal residue, but no intact impression."
				: $"{Parent.HowSeen(actor, true)} is not sealed.";
		}

		var passed = actor.IsAdministrator() ||
		             actor.Gameworld.GetCheck(CheckType.AppraiseItemCheck).Check(actor, InspectionDifficulty, Parent).IsPass();
		if (!passed)
		{
			return _isSealed
				? $"{Parent.HowSeen(actor, true)} is sealed, but you cannot make out reliable details of the impression."
				: $"{Parent.HowSeen(actor, true)} has a broken seal, but you cannot make out reliable details of the impression.";
		}

		var sb = new StringBuilder();
		sb.AppendLine(_isSealed
			? $"{Parent.HowSeen(actor, true)} is sealed with the following impression:"
			: $"{Parent.HowSeen(actor, true)} has a broken seal with the following impression:");
		sb.AppendLine($"Design: {CurrentSeal.SealDesign.ColourValue()}");
		if (!string.IsNullOrWhiteSpace(CurrentSeal.IssuerText))
		{
			sb.AppendLine($"Issuer: {CurrentSeal.IssuerText.ColourName()}");
		}

		if (!string.IsNullOrWhiteSpace(CurrentSeal.OwnerText))
		{
			sb.AppendLine($"Owner: {CurrentSeal.OwnerText.ColourName()}");
		}

		if (!string.IsNullOrWhiteSpace(CurrentSeal.ClanText))
		{
			sb.AppendLine($"Clan: {CurrentSeal.ClanText.ColourName()}");
		}

		if (!string.IsNullOrWhiteSpace(CurrentSeal.OfficeText))
		{
			sb.AppendLine($"Office: {CurrentSeal.OfficeText.ColourName()}");
		}

		if (!string.IsNullOrWhiteSpace(CurrentSeal.StampMaterial))
		{
			sb.AppendLine($"Stamp Material: {CurrentSeal.StampMaterial.ColourValue()}");
		}

		if (!string.IsNullOrWhiteSpace(CurrentSeal.SealMedium))
		{
			sb.AppendLine($"Seal Medium: {CurrentSeal.SealMedium.ColourValue()}");
		}

		sb.AppendLine($"Forgery Difficulty: {CurrentSeal.ForgeryDifficulty.DescribeColoured()}");
		return sb.ToString();
	}

	public bool SealMatches(ISealStamp stamp)
	{
		return CurrentSeal is not null &&
		       CurrentSeal.SealDesign.EqualTo(stamp.SealDesign) &&
		       CurrentSeal.IssuerText.EqualTo(stamp.IssuerText) &&
		       CurrentSeal.OwnerText.EqualTo(stamp.OwnerText) &&
		       CurrentSeal.ClanText.EqualTo(stamp.ClanText) &&
		       CurrentSeal.OfficeText.EqualTo(stamp.OfficeText) &&
		       CurrentSeal.StampMaterial.EqualTo(stamp.StampMaterial);
	}

	public bool SealMatches(ISealable sealable)
	{
		return CurrentSeal is not null &&
		       sealable.CurrentSeal is not null &&
		       CurrentSeal.SealDesign.EqualTo(sealable.CurrentSeal.SealDesign) &&
		       CurrentSeal.IssuerText.EqualTo(sealable.CurrentSeal.IssuerText) &&
		       CurrentSeal.OwnerText.EqualTo(sealable.CurrentSeal.OwnerText) &&
		       CurrentSeal.ClanText.EqualTo(sealable.CurrentSeal.ClanText) &&
		       CurrentSeal.OfficeText.EqualTo(sealable.CurrentSeal.OfficeText) &&
		       CurrentSeal.StampMaterial.EqualTo(sealable.CurrentSeal.StampMaterial);
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type == EventType.ItemOpened && _isSealed)
		{
			BreakSeal(arguments.OfType<ICharacter>().FirstOrDefault(), "opened");
			return true;
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		return types.Contains(EventType.ItemOpened);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Full or DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Evaluate && voyeur is ICharacter ch)
		{
			return InspectSeal(ch);
		}

		if (type != DescriptionType.Full)
		{
			return description;
		}

		if (_isSealed && CurrentSeal is not null)
		{
			return $"{description}\n\nIt is sealed with an impression of {CurrentSeal.SealDesign.ColourValue()}.";
		}

		if (_hasSealResidue)
		{
			return $"{description}\n\nIt has the broken residue of a seal.";
		}

		return description;
	}
}
