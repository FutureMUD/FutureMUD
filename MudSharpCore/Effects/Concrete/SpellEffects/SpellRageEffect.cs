using System;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellRageEffect : MagicSpellEffectBase, IRageEffect
{
    public static void InitialiseEffectType()
    {
        RegisterFactory("SpellRage", (effect, owner) => new SpellRageEffect(effect, owner));
    }

    public SpellRageEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog, double intensity) : base(owner, parent, prog)
    {
        IntensityPerGramMass = intensity;
    }

    protected SpellRageEffect(XElement root, IPerceivable owner) : base(root, owner)
    {
        var tr = root.Element("Effect");
        IntensityPerGramMass = double.Parse(tr.Element("Intensity").Value);
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
            new XElement("Intensity", IntensityPerGramMass)
        );
    }

    public override string Describe(IPerceiver voyeur)
    {
        return $"Rage Intensity {IntensityPerGramMass.ToString("N2", voyeur)}";
    }

    protected override string SpecificEffectType => "SpellRage";

    public double IntensityPerGramMass { get; set; }

    public bool IsRaging => IntensityPerGramMass >= 5.0;
    public bool IsSuperRaging => IntensityPerGramMass >= 10.0;
}
