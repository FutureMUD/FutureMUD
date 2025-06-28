using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellBlindnessEffect : MagicSpellEffectBase, IBlindnessEffect
{
    public static void InitialiseEffectType()
    {
        RegisterFactory("SpellBlindness", (effect, owner) => new SpellBlindnessEffect(effect, owner));
    }

    public SpellBlindnessEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog) : base(owner, parent, prog)
    {
    }

    protected SpellBlindnessEffect(XElement root, IPerceivable owner) : base(root, owner)
    {
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0)
        );
    }

    public override string Describe(IPerceiver voyeur)
    {
        return "Blinded";
    }

    protected override string SpecificEffectType => "SpellBlindness";
}
