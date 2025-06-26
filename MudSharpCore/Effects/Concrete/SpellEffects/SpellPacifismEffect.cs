using System;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellPacifismEffect : MagicSpellEffectBase, IPacifismEffect
{
    public static void InitialiseEffectType()
    {
        RegisterFactory("SpellPacifism", (effect, owner) => new SpellPacifismEffect(effect, owner));
    }

    public SpellPacifismEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog, double intensity) : base(owner, parent, prog)
    {
        IntensityPerGramMass = intensity;
    }

    protected SpellPacifismEffect(XElement root, IPerceivable owner) : base(root, owner)
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
        return $"Pacifism Intensity {IntensityPerGramMass.ToString("N2", voyeur)}";
    }

    protected override string SpecificEffectType => "SpellPacifism";

    public double IntensityPerGramMass { get; set; }

    public bool IsPeaceful => IntensityPerGramMass > 5.0;
    public bool IsSuperPeaceful => IntensityPerGramMass > 10.0;
}
