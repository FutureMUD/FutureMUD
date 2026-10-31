#nullable enable
using MudSharp.Body;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public sealed class EquipmentBonusesGameItemComponent : GameItemComponent, IEquippedItemBonusProvider
{
	private EquipmentBonusesGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;
	public EquipmentBonusesGameItemComponent(EquipmentBonusesGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary) => _prototype = proto;
	public EquipmentBonusesGameItemComponent(MudSharp.Models.GameItemComponent model, EquipmentBonusesGameItemComponentProto proto, IGameItem parent) : base(model, parent) => _prototype = proto;
	public override IGameItemComponent Copy(IGameItem parent, bool temporary = false) => new EquipmentBonusesGameItemComponent(_prototype, parent, temporary);
	protected override string SaveToXml() => "<Definition />";
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto proto)
	{
		_prototype = (EquipmentBonusesGameItemComponentProto)proto;
		if (Parent.InInventoryOf?.Actor is { } actor)
		{
			if (actor.Body is MudSharp.Body.Implementations.Body body) body.ReconcileEquippedSpellBonuses();
			actor.Gameworld.MagicCasting?.NotifyCapacityChange(actor);
		}
	}
	internal bool ActiveFor(ICharacter actor)
	{
		var body = actor.Body;
		if (body is null || Parent.Deleted || Parent.Destroyed || !ReferenceEquals(Parent.InInventoryOf, body)) return false;
		if (_prototype.WorksWielded && body.WieldedItems.Any(item => ReferenceEquals(item, Parent))) return true;
		if (_prototype.WorksHeld && body.HeldItems.Any(item => ReferenceEquals(item, Parent)) && body.HoldOrWieldLocFor(Parent) is IGrab { Unary: true }) return true;
		return _prototype.WorksWorn && body.DirectWornItems.Any(item => ReferenceEquals(item, Parent)) && Parent.GetItemType<IWearable>()?.CurrentProfile is { } profile && !_prototype.ExcludedProfiles.Contains(profile.Id);
	}
	public IEnumerable<EquippedItemBonus> BonusesFor(ICharacter actor) => ActiveFor(actor) ? _prototype.Bonuses : [];
	public override bool DescriptionDecorator(DescriptionType type) => type == DescriptionType.Evaluate;
	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour, PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Evaluate) return description;
		var lines = _prototype.Bonuses.Select(x =>
		{
			var target = x.Kind == MudSharp.Magic.EquippedItemBonusKind.Trait ? Gameworld.Traits.Get(x.TargetId)?.Name :
				x.Kind == MudSharp.Magic.EquippedItemBonusKind.MagicResource ? Gameworld.MagicResources.Get(x.TargetId)?.Name + " capacity" : x.Kind is MudSharp.Magic.EquippedItemBonusKind.Health or MudSharp.Magic.EquippedItemBonusKind.Stun or MudSharp.Magic.EquippedItemBonusKind.Stamina
					? x.Kind.DescribeEnum() + " capacity" : x.Kind.DescribeEnum();
			return $"When actively equipped, this provides {x.Bonus.ToBonusString(voyeur)} to {target.ColourName()}.";
		});
		return description + "\n" + string.Join("\n", lines) + "\nCarrying it in an inventory slot or storage attachment does not activate these bonuses.";
	}
}
