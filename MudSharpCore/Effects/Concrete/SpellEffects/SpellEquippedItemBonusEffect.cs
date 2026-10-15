#nullable enable
using System.Globalization;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete.SpellEffects;

/// <summary>The ordinary spell parent owns expiry, exclusivity, dispel and persistence.</summary>
public sealed class SpellEquippedItemBonusEffect : MagicSpellEffectBase, IEquippedItemBonusEffect
{
	public static void InitialiseEffectType() => RegisterFactory("SpellEquippedItemBonus",
		(root, owner) => new SpellEquippedItemBonusEffect(root, owner));

	public SpellEquippedItemBonusEffect(IPerceivable owner, IMagicSpellEffectParent parent,
		EquippedItemBonusKind kind, long targetId, double bonus) : base(owner, parent, null)
	{
		BonusKind = kind; TargetId = targetId; Bonus = bonus;
		Validate();
	}

	private SpellEquippedItemBonusEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var xml = root.Element("Effect")!;
		BonusKind = Enum.Parse<EquippedItemBonusKind>((string)xml.Element("Kind")!, false);
		TargetId = (long)xml.Element("TargetId")!;
		Bonus = double.Parse((string)xml.Element("Bonus")!, CultureInfo.InvariantCulture);
		Validate();
	}

	private void Validate()
	{
		if (!Enum.IsDefined(BonusKind) || !double.IsFinite(Bonus) || Bonus <= 0 ||
			(BonusKind is EquippedItemBonusKind.Trait or EquippedItemBonusKind.MagicResource ? TargetId <= 0 : TargetId != 0))
			throw new FormatException("Invalid retained equipped-item bonus.");
	}

	public EquippedItemBonusKind BonusKind { get; }
	public long TargetId { get; }
	public double Bonus { get; }
	protected override string SpecificEffectType => "SpellEquippedItemBonus";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Kind", BonusKind),
		new XElement("TargetId", TargetId), new XElement("Bonus", Bonus));
	public override string Describe(IPerceiver voyeur) =>
		$"Equipped item enchantment: {BonusKind.DescribeEnum().ColourName()} {Bonus.ToBonusString(voyeur)}.";
}
