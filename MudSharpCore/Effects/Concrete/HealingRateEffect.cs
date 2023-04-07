using MudSharp.Effects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class HealingRateEffect : Effect, IHealingRateEffect
{
	public double HealingRateMultiplier { get; protected set; }

	public int HealingDifficultyStages { get; protected set; }

	protected override string SpecificEffectType => "HealingRateEffect";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Healing {1.0 - HealingRateMultiplier:P2} faster and {Math.Abs(HealingDifficultyStages):N0} stages {(HealingDifficultyStages > 0 ? "harder" : "easier")}.";
	}

	public HealingRateEffect(IPerceivable owner, double multiplier, int stages, IFutureProg prog) : base(owner, prog)
	{
		HealingRateMultiplier = multiplier;
		HealingDifficultyStages = stages;
	}

	protected HealingRateEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXml(effect.Element("Effect"));
	}

	private void LoadFromXml(XElement xElement)
	{
		HealingRateMultiplier = double.Parse(xElement.Element("Multiplier").Value);
		HealingDifficultyStages = int.Parse(xElement.Element("Stages").Value);
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Multiplier", HealingRateMultiplier),
			new XElement("Stages", HealingDifficultyStages)
		);
	}

	public override bool SavingEffect => true;

	public static void InitialiseEffectType()
	{
		RegisterFactory("HealingRateEffect", (effect, owner) => new HealingRateEffect(effect, owner));
	}
}