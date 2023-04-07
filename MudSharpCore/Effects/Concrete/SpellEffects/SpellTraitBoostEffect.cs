using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellTraitBoostEffect : MagicSpellEffectBase, ITraitBonusEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellTraitBoost", (effect, owner) => new SpellTraitBoostEffect(effect, owner));
	}

	public SpellTraitBoostEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog) : base(owner,
		parent, prog)
	{
	}

	protected SpellTraitBoostEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		Trait = Gameworld.Traits.Get(long.Parse(trueRoot.Element("Trait").Value));
		Bonus = double.Parse(trueRoot.Element("Bonus").Value);
		TraitBonusContext = (TraitBonusContext)int.Parse(trueRoot.Element("Context").Value);
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("Trait", Trait.Id),
			new XElement("Bonus", Bonus),
			new XElement("Context", (int)TraitBonusContext)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Trait Boosted - {Trait.Name.ColourValue()} {Bonus.ToBonusString(voyeur)} (context: {TraitBonusContext.DescribeEnum().ColourValue()})";
	}

	protected override string SpecificEffectType => "SpellTraitBoost";

	public ITraitDefinition Trait { get; init; }
	public double Bonus { get; init; }
	public TraitBonusContext TraitBonusContext { get; init; }

	#region Implementation of ITraitBonusEffect

	/// <inheritdoc />
	public bool AppliesToTrait(ITraitDefinition trait)
	{
		return trait == Trait;
	}

	/// <inheritdoc />
	public bool AppliesToTrait(ITrait trait)
	{
		return trait is not null && trait.Definition == Trait;
	}

	/// <inheritdoc />
	public double GetBonus(ITrait trait, TraitBonusContext context = TraitBonusContext.None)
	{
		if (!AppliesToTrait(trait))
		{
			return 0.0;
		}

		if (TraitBonusContext != TraitBonusContext.None && context != TraitBonusContext)
		{
			return 0.0;
		}

		return Bonus;
	}

	#endregion
}