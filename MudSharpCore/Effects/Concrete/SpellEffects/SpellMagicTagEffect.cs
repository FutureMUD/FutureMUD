#nullable enable


using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellMagicTagEffect : SimpleSpellStatusEffectBase, IMagicTagEffect, ISpellMarkedRoomEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellMagicTag", (effect, owner) => new SpellMagicTagEffect(effect, owner));
	}

	public SpellMagicTagEffect(IPerceivable owner, IMagicSpellEffectParent parent, string tag, string value,
		IFutureProg? prog = null, long savedRoomId = 0, int remainingUses = 0) : base(owner, parent, prog)
	{
		Tag = tag.Trim();
		Value = value;
		SavedRoomId = savedRoomId;
		ItemId = savedRoomId > 0 && owner is IGameItem item ? item.Id : 0;
		RemainingUses = remainingUses;
	}

	private SpellMagicTagEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		Tag = trueRoot?.Element("Tag")?.Value ?? string.Empty;
		Value = trueRoot?.Element("Value")?.Value ?? string.Empty;
		SavedRoomId = (long?)trueRoot?.Element("SavedRoomId") ?? 0;
		ItemId = (long?)trueRoot?.Element("ItemId") ?? 0;
		RemainingUses = (int?)trueRoot?.Element("RemainingUses") ?? 0;
	}

	public string Tag { get; }
	public string Value { get; }
	public long ItemId { get; }
	public long SavedRoomId { get; }
	public int RemainingUses { get; private set; }
	public bool IsActive => SavedRoomId > 0 && ItemId > 0 && RemainingUses > 0 &&
		Tag == ArmageddonSimpleStockContent.ArcaneMarkTag &&
		Owner is IGameItem item && item.Id == ItemId && !item.Deleted && !item.Destroyed &&
		ParentEffect?.Spell is MagicSpell { StockIdentity: "arm.spell.arcane_mark" } &&
		ReferenceEquals(ParentEffect.Owner, item) &&
		item.Effects.Any(x => ReferenceEquals(x, this)) &&
		item.Effects.Any(x => ReferenceEquals(x, ParentEffect)) &&
		ParentEffect.SpellEffects.Any(x => ReferenceEquals(x, this));
	public ICharacter? Caster => ParentEffect?.Caster;

	/// <summary>Mirror a use already debited in the same durable transaction as its travel creation.</summary>
	internal void AcceptCommittedMarkedRoomUse(int previousUses)
	{
		if (!IsActive || RemainingUses != previousUses || previousUses <= 0)
			throw new InvalidOperationException("The live mark no longer matches its committed use debit.");
		RemainingUses = previousUses - 1;
		if (RemainingUses == 0)
		{
			Owner.RemoveEffect(this, true);
			return;
		}
		Changed = true;
		Owner.EffectsChanged = true;
	}

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("Tag", new XCData(Tag)),
			new XElement("Value", new XCData(Value)),
			SavedRoomId > 0 ? new XElement("SavedRoomId", SavedRoomId) : null,
			ItemId > 0 ? new XElement("ItemId", ItemId) : null,
			SavedRoomId > 0 ? new XElement("RemainingUses", RemainingUses) : null
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Magic tag {Tag.ColourName()} = {Value.ColourValue()}";
	}

	protected override string SpecificEffectType => "SpellMagicTag";
}
