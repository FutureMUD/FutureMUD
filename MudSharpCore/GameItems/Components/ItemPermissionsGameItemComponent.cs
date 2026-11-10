#nullable enable

using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public sealed class ItemPermissionsGameItemComponent : GameItemComponent, IItemInteractionPermissions, ICurseRemovableItem
{
	private ItemPermissionsGameItemComponentProto _prototype;
	private ItemInteractionRestriction? _override;
	private bool _pickupAttempted;
	private bool _curseRemoved;
	public bool IsCursed => _prototype.Cursed && !_curseRemoved;
	public override IGameItemComponentProto Prototype => _prototype;
	public ItemInteractionRestriction Restrictions => _override ?? _prototype.Restrictions;
	public ItemPermissionsGameItemComponent(ItemPermissionsGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary) => _prototype = proto;
	public ItemPermissionsGameItemComponent(MudSharp.Models.GameItemComponent component, ItemPermissionsGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		var root = XElement.Parse(component.Definition);
		_pickupAttempted = (bool?)root.Element("PickupAttempted") ?? false;
		_curseRemoved = (bool?)root.Element("CurseRemoved") ?? false;
		if ((int?)root.Element("Restrictions") is { } mask)
		{
			_override = (ItemInteractionRestriction)mask;
			if ((_override & ~ItemPermissionsGameItemComponentProto.AllRestrictions) != 0) throw new FormatException("Unknown saved item interaction restriction.");
		}
	}
	private ItemPermissionsGameItemComponent(ItemPermissionsGameItemComponent source, IGameItem parent, bool temporary) : base(source, parent, temporary) { _prototype = source._prototype; _override = source._override; _pickupAttempted = source._pickupAttempted; _curseRemoved = source._curseRemoved; }
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new ItemPermissionsGameItemComponent(this, newParent, temporary);
	public override bool PreventsMerging(IGameItemComponent component) =>
		component is ItemPermissionsGameItemComponent other && other.Prototype.Id == Prototype.Id &&
		(_override != other._override || _curseRemoved != other._curseRemoved || _pickupAttempted != other._pickupAttempted);
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) => _prototype = (ItemPermissionsGameItemComponentProto)newProto;
	protected override string SaveToXml() => new XElement("Definition", _override is { } mask ? new XElement("Restrictions", (int)mask) : null,
		new XElement("PickupAttempted", _pickupAttempted), new XElement("CurseRemoved", _curseRemoved)).ToString();
	public bool RemoveCurse()
	{
		if (!IsCursed) return false;
		_override = Restrictions & ~_prototype.CurseRemovableRestrictions;
		_curseRemoved = true;
		Changed = true;
		return true;
	}
	public bool HandlePickupAttempt(ICharacter actor)
	{
		if (_prototype.PickupAttemptProgId == 0 || _prototype.PickupAttemptOnceOnly && _pickupAttempted) return true;
		var prog = Gameworld.FutureProgs.Get(_prototype.PickupAttemptProgId);
		if (prog is null || prog.ReturnType != MudSharp.FutureProg.ProgVariableTypes.Boolean ||
			!prog.MatchesParameters([MudSharp.FutureProg.ProgVariableTypes.Character, MudSharp.FutureProg.ProgVariableTypes.Item])) return false;
		if (_prototype.PickupAttemptOnceOnly) { _pickupAttempted = true; Changed = true; }
		return prog.ExecuteBool(false, actor, Parent);
	}
	public void SetPermission(ItemInteractionRestriction interaction, bool permitted)
	{
		if (interaction == ItemInteractionRestriction.None || (interaction & ~ItemPermissionsGameItemComponentProto.AllRestrictions) != 0) throw new ArgumentOutOfRangeException(nameof(interaction));
		_override = permitted ? Restrictions & ~interaction : Restrictions | interaction;
		Changed = true;
	}
}
