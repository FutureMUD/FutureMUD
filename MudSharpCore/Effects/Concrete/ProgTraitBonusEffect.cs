using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;
public class ProgTraitBonusEffect : Effect, ITraitBonusEffect
{
	#region Static Initialisation
	public static void InitialiseEffectType()
	{
		RegisterFactory("ProgTraitBonusEffect", (effect, owner) => new ProgTraitBonusEffect(effect, owner));
	}
	#endregion

	#region Constructors
	public ProgTraitBonusEffect(ICharacter owner, ITraitDefinition trait, double bonus, string originalReference, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		Trait = trait;
		Bonus = bonus;
		OriginalReference = originalReference;
	}

	protected ProgTraitBonusEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		Trait = Gameworld.Traits.Get(long.Parse(root.Element("Trait").Value));
		Bonus = double.Parse(root.Element("Bonus").Value);
		OriginalReference = root.Element("OriginalReference").Value;
	}
	#endregion

	#region Saving and Loading
	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Trait", Trait.Id),
			new XElement("Bonus", Bonus),
			new XElement("OriginalReference", new XCData(OriginalReference))
		);
	}
	#endregion

	#region Overrides of Effect
	protected override string SpecificEffectType => "ProgTraitBonusEffect";

	public override string Describe(IPerceiver voyeur)
	{
		return $"A {Bonus.ToBonusString(voyeur)} bonus to the {Trait.Name.ColourName()} trait from a prog (ref: {OriginalReference.ColourCommand()}).";
	}

	public override bool SavingEffect => true;
	#endregion

	#region Implementation of ITraitBonusEffect
	public ITraitDefinition Trait { get; private set; }
	public double Bonus { get; private set; }
	public string OriginalReference { get; private set; }

	/// <inheritdoc />
	public bool AppliesToTrait(ITraitDefinition trait)
	{
		return trait == Trait;
	}

	/// <inheritdoc />
	public bool AppliesToTrait(ITrait trait)
	{
		return trait?.Definition == Trait;
	}

	/// <inheritdoc />
	public double GetBonus(ITrait trait, TraitBonusContext context = TraitBonusContext.None)
	{
		if (!AppliesToTrait(trait))
		{
			return 0.0;
		}

		return Bonus;
	}

	#endregion
}
