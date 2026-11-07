#nullable enable

namespace MudSharp.Effects.Concrete;

/// <summary>Saved flat absorption from qualifying physical damage and stun packets.</summary>
public sealed class PhysicalDamageAbsorptionEffect : Effect, IPhysicalDamageAbsorptionEffect
{
	public PhysicalDamageAbsorptionEffect(IPerceivable owner, double absorption) : base(owner)
	{
		PhysicalAbsorption = Validate(absorption);
	}

	private PhysicalDamageAbsorptionEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		PhysicalAbsorption = Validate((double)xml.Element("Absorption")!.Attribute("amount")!);
	}

	public double PhysicalAbsorption { get; }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "PhysicalDamageAbsorption";
	protected override XElement SaveDefinition() => new("Absorption", new XAttribute("amount", PhysicalAbsorption));
	public override string Describe(IPerceiver voyeur) => $"Physical damage and stun absorption {PhysicalAbsorption.ToString("N2", voyeur)}.";
	public static void InitialiseEffectType() => RegisterFactory("PhysicalDamageAbsorption", (xml, owner) => new PhysicalDamageAbsorptionEffect(xml, owner));

	private static double Validate(double absorption) => double.IsFinite(absorption) && absorption >= 0
		? absorption
		: throw new ArgumentOutOfRangeException(nameof(absorption));
}
