using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System.Xml.Linq;
using MudSharp.Body.Traits;

namespace MudSharp.Effects.Concrete;

public class NoTraitGain : Effect, INoTraitGainEffect
{
	public NoTraitGain(IPerceivable owner, ITraitDefinition definition)
		: base(owner)
	{
		Trait = definition;
	}

	public NoTraitGain(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
		Trait = owner.Gameworld.Traits.Get(long.Parse(effect.Element("Trait").Value));
	}

	protected override string SpecificEffectType => "NoTraitGain";

	public ITraitDefinition Trait { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Not Gaining Points With {Trait.Name}.";
	}

	public override bool SavingEffect => true;

	public override void ExpireEffect()
	{
		Owner.RemoveEffect(this);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("NoTraitGain", (effect, owner) => new NoTraitGain(effect, owner));
	}

	public override string ToString()
	{
		return "NoTraitGain Effect";
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Trait", Trait.Id);
	}

	public override bool Applies(object target)
	{
		return base.Applies(target) && target == Trait;
	}
}