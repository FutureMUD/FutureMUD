#nullable enable
using MudSharp.Body.Traits;
using MudSharp.Effects.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public sealed class PsionicDisorientation : Effect, ITraitBonusEffect, IMagicEffect
{
	public PsionicDisorientation(ICharacter owner, PsionicDisorientPower power, double penalty) : base(owner)
	{
		Power = power;
		Penalty = penalty;
	}
	private PsionicDisorientation(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!;
		Power = Gameworld.MagicPowers.Get((long)root.Element("Power")!) as PsionicDisorientPower
			?? throw new ApplicationException("Missing disorientation power.");
		Penalty = (double)root.Element("Penalty")!;
		if (!double.IsFinite(Penalty) || Penalty < 0) throw new ApplicationException("Invalid disorientation penalty.");
	}
	public static void InitialiseEffectType() => RegisterFactory("PsionicDisorientation", (xml, owner) => new PsionicDisorientation(xml, owner));
	public PsionicDisorientPower Power { get; }
	public double Penalty { get; }
	public IMagicSchool School => Power.School;
	public IMagicPower PowerOrigin => Power;
	public Difficulty DetectMagicDifficulty => Power.DetectableWithDetectMagic;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "PsionicDisorientation";
	public bool AppliesToTrait(ITrait trait) => AppliesToTrait(trait.Definition);
	public bool AppliesToTrait(ITraitDefinition trait) => trait == Power.AffectedTrait;
	public double GetBonus(ITrait trait, TraitBonusContext context = TraitBonusContext.None) => -Penalty;
	public override string Describe(IPerceiver voyeur) => $"Disoriented: {Penalty.ToString("N2", voyeur)} penalty to {Power.AffectedTrait.Name}.";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Power", Power.Id), new XElement("Penalty", Penalty));
}
