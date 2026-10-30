#nullable enable

namespace MudSharp.Effects.Concrete;

/// <summary>A saved flat damage multiplier, optionally limited to a number of qualifying hits.</summary>
public sealed class DamageProtectionStatus : Effect, IDamageProtectionEffect
{
	public DamageProtectionStatus(IPerceivable owner, double multiplier, int hits) : base(owner)
	{
		Multiplier = ValidateMultiplier(multiplier);
		ConfiguredHits = RemainingHits = hits >= 0 ? hits : throw new ArgumentOutOfRangeException(nameof(hits));
	}
	private DamageProtectionStatus(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var data = xml.Element("Protection")!;
		Multiplier = ValidateMultiplier((double)data.Attribute("multiplier")!);
		ConfiguredHits = (int)data.Attribute("hits")!; RemainingHits = (int)data.Attribute("remaining")!;
		IsSpent = (bool?)data.Attribute("spent") ?? false;
		if (ConfiguredHits < 0 || RemainingHits < 0 || RemainingHits > ConfiguredHits ||
			(ConfiguredHits > 0 && RemainingHits == 0 && !IsSpent))
			throw new FormatException("Invalid saved damage-protection hit count.");
	}
	public double Multiplier { get; }
	public int ConfiguredHits { get; }
	public int RemainingHits { get; private set; }
	public bool IsSpent { get; private set; }
	public double DamageMultiplier => IsSpent ? 1 : Multiplier;
	public override bool Applies() => !IsSpent && base.Applies();
	public bool TryCommitProtection()
	{
		if (IsSpent) return false;
		if (ConfiguredHits == 0) return true;
		RemainingHits--;
		if (RemainingHits == 0) IsSpent = true;
		// Mark spent before removal: removing an effect can synchronously settle exposure.
		Owner.EffectsChanged = true;
		if (IsSpent) Owner.RemoveEffect(this);
		return true;
	}
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "DamageProtectionStatus";
	protected override XElement SaveDefinition() => new("Protection", new XAttribute("multiplier", Multiplier),
		new XAttribute("hits", ConfiguredHits), new XAttribute("remaining", RemainingHits), new XAttribute("spent", IsSpent));
	public override string Describe(IPerceiver voyeur) => IsSpent ? "Spent damage protection." :
		$"Damage x{Multiplier.ToString("N2", voyeur)}; {(ConfiguredHits == 0 ? "unlimited hits" : $"{RemainingHits.ToString("N0", voyeur)} hits remaining")}.";
	public static void InitialiseEffectType() => RegisterFactory("DamageProtectionStatus", (xml, owner) => new DamageProtectionStatus(xml, owner));
	private static double ValidateMultiplier(double value) => double.IsFinite(value) && value is >= 0 and <= 1
		? value : throw new ArgumentOutOfRangeException(nameof(value));
}
