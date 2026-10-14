#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete.SpellEffects;

public abstract class SpellOwnedItemMarkEffect : SimpleSpellStatusEffectBase
{
	protected SpellOwnedItemMarkEffect(IGameItem owner, IMagicSpellEffectParent parent,
		ItemOwnershipReference ownerReference) : base(owner, parent)
	{
		OwnerReference = ownerReference;
		ItemId = owner.Id;
	}

	protected SpellOwnedItemMarkEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var xml = root.Element("Effect") ?? throw new FormatException("Missing owned item mark.");
		ItemId = (long?)xml.Element("ItemId") ?? 0;
		OwnerReference = new ItemOwnershipReference(
			(string?)xml.Element("OwnerType") ?? string.Empty, (long?)xml.Element("OwnerId") ?? 0);
	}

	public long ItemId { get; }
	public ItemOwnershipReference OwnerReference { get; }
	public bool IsActive => Owner is IGameItem item && item.Id == ItemId && ItemId > 0 &&
		OwnerReference.Id > 0 && !string.IsNullOrWhiteSpace(OwnerReference.FrameworkItemType) &&
		item.OwnershipReference == OwnerReference && !item.Deleted && !item.Destroyed &&
		ParentEffect is not null && ReferenceEquals(ParentEffect.Owner, item) &&
		item.Effects.Any(x => ReferenceEquals(x, this)) &&
		item.Effects.Any(x => ReferenceEquals(x, ParentEffect)) &&
		ParentEffect.SpellEffects.Any(x => ReferenceEquals(x, this));

	protected override XElement SaveDefinition() => SimpleSaveDefinition(
		new XElement("ItemId", ItemId),
		new XElement("OwnerType", OwnerReference.FrameworkItemType),
		new XElement("OwnerId", OwnerReference.Id));
}

public sealed class SpellCustodianGlyphEffect : SpellOwnedItemMarkEffect, IItemTakeInterdictionEffect
{
	public static void InitialiseEffectType() => RegisterFactory("SpellCustodianGlyph",
		(root, owner) => new SpellCustodianGlyphEffect(root, owner));

	public SpellCustodianGlyphEffect(IGameItem owner, IMagicSpellEffectParent parent,
		ItemOwnershipReference ownerReference) : base(owner, parent, ownerReference) { }

	private SpellCustodianGlyphEffect(XElement root, IPerceivable owner) : base(root, owner) { }

	public bool AllowsTake(ICharacter actor) => !IsActive ||
		Owner is IGameItem item && item.IsOwnedBy(actor);

	public override string Describe(IPerceiver voyeur) => "A custodian glyph protects this item from unauthorised taking.";
	protected override string SpecificEffectType => "SpellCustodianGlyph";
}

public sealed class SpellAnchorRuneEffect : SpellOwnedItemMarkEffect, ISpellTravelAnchorEffect
{
	public static void InitialiseEffectType() => RegisterFactory("SpellAnchorRune",
		(root, owner) => new SpellAnchorRuneEffect(root, owner));

	public SpellAnchorRuneEffect(IGameItem owner, IMagicSpellEffectParent parent,
		ItemOwnershipReference ownerReference) : base(owner, parent, ownerReference) { }

	private SpellAnchorRuneEffect(XElement root, IPerceivable owner) : base(root, owner) { }

	public override string Describe(IPerceiver voyeur) => "This owned item is marked as a travel anchor.";
	protected override string SpecificEffectType => "SpellAnchorRune";
}
