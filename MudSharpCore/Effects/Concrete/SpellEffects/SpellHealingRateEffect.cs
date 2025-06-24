using System;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellHealingRateEffect : MagicSpellEffectBase, IHealingRateEffect
{
    public static void InitialiseEffectType()
    {
        RegisterFactory("SpellHealingRate", (effect, owner) => new SpellHealingRateEffect(effect, owner));
    }

    public SpellHealingRateEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog, double multiplier, int stages) : base(owner, parent, prog)
    {
        HealingRateMultiplier = multiplier;
        HealingDifficultyStages = stages;
    }

    protected SpellHealingRateEffect(XElement root, IPerceivable owner) : base(root, owner)
    {
        var trueRoot = root.Element("Effect");
        HealingRateMultiplier = double.Parse(trueRoot.Element("Multiplier").Value);
        HealingDifficultyStages = int.Parse(trueRoot.Element("Stages").Value);
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
            new XElement("Multiplier", HealingRateMultiplier),
            new XElement("Stages", HealingDifficultyStages)
        );
    }

    public override string Describe(IPerceiver voyeur)
    {
        return $"Healing {1.0 - HealingRateMultiplier:P2} faster and {Math.Abs(HealingDifficultyStages):N0} stages {(HealingDifficultyStages > 0 ? "harder" : "easier")}.";
    }

    protected override string SpecificEffectType => "SpellHealingRate";

    public double HealingRateMultiplier { get; set; }
    public int HealingDifficultyStages { get; set; }
}
