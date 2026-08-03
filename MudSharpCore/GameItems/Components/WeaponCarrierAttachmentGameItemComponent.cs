using MudSharp.GameItems.Prototypes;
using MudSharp.Construction;

#nullable enable

namespace MudSharp.GameItems.Components;

public class WeaponCarrierAttachmentGameItemComponent : GameItemComponent, IWeaponCarrierAttachment
{
	private WeaponCarrierAttachmentGameItemComponentProto _prototype;
	private IGameItem? _attachedWeapon;
	private WeaponCarrierState _state;

	public WeaponCarrierAttachmentGameItemComponent(WeaponCarrierAttachmentGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary) => _prototype = proto;
	public WeaponCarrierAttachmentGameItemComponent(MudSharp.Models.GameItemComponent component,
		WeaponCarrierAttachmentGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		var root = XElement.Parse(component.Definition);
		_attachedWeapon = Gameworld.TryGetItem((long?)root.Element("AttachedWeapon") ?? 0, true);
		if (_attachedWeapon?.Deleted == true)
		{
			_attachedWeapon = null;
		}
		_state = root.Element("State")?.Value.TryParseEnum<WeaponCarrierState>(out var state) == true
			? state
			: _attachedWeapon is null ? WeaponCarrierState.Detached : WeaponCarrierState.Carried;
	}
	private WeaponCarrierAttachmentGameItemComponent(WeaponCarrierAttachmentGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary) => _prototype = rhs._prototype;

	public override IGameItemComponentProto Prototype => _prototype;
	public IGameItem? AttachedWeapon => _attachedWeapon;
	public WeaponCarrierState State => _state;
	public bool CanAttach(IGameItem weapon, ICharacter actor, out string reason)
	{
		if (_attachedWeapon is not null)
		{
			reason = "That carrier already has a weapon attached.";
			return false;
		}
		var rangedWeapon = weapon.GetItemType<IRangedWeapon>();
		if (rangedWeapon is null && weapon.GetItemType<IMeleeWeapon>() is null)
		{
			reason = "That carrier can only attach a weapon.";
			return false;
		}
		var profile = _prototype.CompatibleProfile;
		if (!profile.EqualTo("any") && !weapon.Name.Contains(profile, StringComparison.InvariantCultureIgnoreCase))
		{
			reason = $"That carrier only accepts {profile} weapons.";
			return false;
		}
		if (!_prototype.CompatibleWeaponType.EqualTo("any") &&
			(rangedWeapon is null || !rangedWeapon.WeaponType.RangedWeaponType.DescribeEnum()
				.EqualTo(_prototype.CompatibleWeaponType)))
		{
			reason = $"That carrier only accepts {_prototype.CompatibleWeaponType} weapons.";
			return false;
		}
		var requiredTags = _prototype.CompatibleTags.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		if (requiredTags.Length > 0 && !weapon.Tags.Any(x => requiredTags.Any(tag => x.Name.EqualTo(tag))))
		{
			reason = $"That carrier requires a weapon tagged {requiredTags.ListToString()}.";
			return false;
		}
		if (weapon.Size > _prototype.MaximumWeaponSize)
		{
			reason = $"That weapon is too large for this carrier.";
			return false;
		}
		if (!CarrierIsUsableBy(actor))
		{
			reason = $"You must have that carrier {_prototype.AttachmentPoint.Replace('-', ' ')} before attaching a weapon.";
			return false;
		}
		reason = string.Empty;
		return true;
	}
	public bool Attach(IGameItem weapon, ICharacter actor, out string reason)
	{
		if (!CanAttach(weapon, actor, out reason)) return false;
		if (!CarrierIsUsableBy(actor))
		{
			reason = "You must be wearing or holding that carrier before attaching a weapon to it.";
			return false;
		}
		_attachedWeapon = weapon;
		_state = actor.Body.WieldedItems.Contains(weapon) ? WeaponCarrierState.Wielded : WeaponCarrierState.Carried;
		Changed = true;
		return true;
	}
	public bool Detach(ICharacter actor, out string reason)
	{
		if (_attachedWeapon is null)
		{
			reason = "That carrier has no attached weapon.";
			return false;
		}
		if (_attachedWeapon.ContainedIn == Parent)
		{
			_attachedWeapon.ContainedIn = null;
			_attachedWeapon.InsertAtSource(actor);
		}
		_attachedWeapon = null;
		_state = WeaponCarrierState.Detached;
		Changed = true;
		reason = string.Empty;
		return true;
	}
	public bool TryRetain(IGameItem weapon, ICharacter actor)
	{
		if (!_prototype.RetainsDroppedWeapon || _attachedWeapon != weapon || !CarrierIsUsableBy(actor)) return false;
		weapon.ContainedIn = Parent;
		_state = WeaponCarrierState.Hanging;
		Changed = true;
		return true;
	}
	public bool Recover(ICharacter actor, out string reason)
	{
		if (_attachedWeapon is null || _state != WeaponCarrierState.Hanging)
		{
			reason = "That carrier is not retaining a hanging weapon.";
			return false;
		}
		if (actor.Body.CanGet(_attachedWeapon, 0))
		{
			actor.Body.Get(_attachedWeapon, silent: true);
		}
		else
		{
			_attachedWeapon.InsertAtSource(actor);
		}
		_state = WeaponCarrierState.Carried;
		Changed = true;
		reason = string.Empty;
		return true;
	}
	public bool Release(ICharacter actor, out string reason)
	{
		if (_attachedWeapon is null || _state != WeaponCarrierState.Hanging)
		{
			reason = "That carrier is not retaining a hanging weapon.";
			return false;
		}
		_attachedWeapon.ContainedIn = null;
		_attachedWeapon.InsertAtSource(actor);
		_attachedWeapon = null;
		_state = WeaponCarrierState.Detached;
		Changed = true;
		reason = string.Empty;
		return true;
	}

	public override bool Take(IGameItem item)
	{
		if (_attachedWeapon != item || item.ContainedIn != Parent)
		{
			return false;
		}
		item.ContainedIn = null;
		_state = WeaponCarrierState.Carried;
		Changed = true;
		return true;
	}
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new WeaponCarrierAttachmentGameItemComponent(_prototype, newParent, temporary);
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) => _prototype = (WeaponCarrierAttachmentGameItemComponentProto)newProto;
	protected override string SaveToXml() => new XElement("Definition", new XElement("AttachedWeapon", _attachedWeapon?.Id ?? 0), new XElement("State", _state)).ToString();

	public override void Delete()
	{
		if (_attachedWeapon?.ContainedIn == Parent)
		{
			_attachedWeapon.ContainedIn = null;
			_attachedWeapon.InsertAtSource(Parent.LocationLevelPerceivable ?? Parent);
		}
		_attachedWeapon = null;
		_state = WeaponCarrierState.Detached;
		base.Delete();
	}

	private bool CarrierIsUsableBy(ICharacter actor)
	{
		return _prototype.AttachmentPoint switch
		{
			"worn" => actor.Body.WornItems.Contains(Parent),
			"held" => actor.Body.HeldOrWieldedItems.Contains(Parent),
			_ => actor.Body.WornItems.Contains(Parent) || actor.Body.HeldOrWieldedItems.Contains(Parent)
		};
	}
}
