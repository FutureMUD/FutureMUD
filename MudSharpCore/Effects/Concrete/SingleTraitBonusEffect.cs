using System;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class SingleTraitBonusEffect : Effect, ITraitBonusEffect
{
	public ITraitDefinition Trait { get; set; }
	public double Bonus { get; set; }

	public SingleTraitBonusEffect(IPerceiver owner, ITraitDefinition trait, double bonus, IFutureProg prog) :
		base(owner, prog)
	{
		Trait = trait;
		Bonus = bonus;
	}

	public SingleTraitBonusEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromDatabase(effect.Element("Effect"));
	}

	private void LoadFromDatabase(XElement root)
	{
		Trait = Gameworld.Traits.Get(long.Parse(root.Element("Trait").Value));
		Bonus = double.Parse(root.Element("Bonus").Value);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("Single Trait Bonus", (effect, owner) => new SingleTraitBonusEffect(effect, owner));
	}

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect", new XElement("Trait", Trait.Id), new XElement("Bonus", Bonus));
	}

	protected override string SpecificEffectType => "Single Trait Bonus";

	public bool AppliesToTrait(ITrait trait)
	{
		return trait.Definition == Trait;
	}

	public bool AppliesToTrait(ITraitDefinition trait)
	{
		return trait == Trait;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"A {(Bonus > 0 ? "bonus" : "penalty")} of {Math.Abs(Bonus):N3} to the {Trait.Name} trait.";
	}

	public double GetBonus(ITrait trait, TraitBonusContext context = TraitBonusContext.None)
	{
		return Bonus;
	}
}