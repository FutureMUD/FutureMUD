using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellNeedRateEffect : MagicSpellEffectBase, INeedRateEffect
{
    public static void InitialiseEffectType()
    {
        RegisterFactory("SpellNeedRate", (effect, owner) => new SpellNeedRateEffect(effect, owner));
    }

    public SpellNeedRateEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog,
        double hunger, double thirst, double drunk, bool passive, bool active) : base(owner, parent, prog)
    {
        HungerMultiplier = hunger;
        ThirstMultiplier = thirst;
        DrunkennessMultiplier = drunk;
        AppliesToPassive = passive;
        AppliesToActive = active;
    }

    protected SpellNeedRateEffect(XElement root, IPerceivable owner) : base(root, owner)
    {
        var trueRoot = root.Element("Effect");
        HungerMultiplier = double.Parse(trueRoot.Element("HungerMult")!.Value);
        ThirstMultiplier = double.Parse(trueRoot.Element("ThirstMult")!.Value);
        DrunkennessMultiplier = double.Parse(trueRoot.Element("DrunkMult")!.Value);
        AppliesToPassive = bool.Parse(trueRoot.Element("Passive")!.Value);
        AppliesToActive = bool.Parse(trueRoot.Element("Active")!.Value);
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
            new XElement("HungerMult", HungerMultiplier),
            new XElement("ThirstMult", ThirstMultiplier),
            new XElement("DrunkMult", DrunkennessMultiplier),
            new XElement("Passive", AppliesToPassive),
            new XElement("Active", AppliesToActive)
        );
    }

    public override string Describe(IPerceiver voyeur)
    {
        return $"Need Rate x{HungerMultiplier:N2}/{ThirstMultiplier:N2}/{DrunkennessMultiplier:N2}";
    }

    protected override string SpecificEffectType => "SpellNeedRate";

    public double HungerMultiplier { get; set; }
    public double ThirstMultiplier { get; set; }
    public double DrunkennessMultiplier { get; set; }
    public bool AppliesToPassive { get; set; }
    public bool AppliesToActive { get; set; }
}
