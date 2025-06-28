using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellDeafnessEffect : MagicSpellEffectBase, IDeafnessEffect
{
    public static void InitialiseEffectType()
    {
        RegisterFactory("SpellDeafness", (effect, owner) => new SpellDeafnessEffect(effect, owner));
    }

    public SpellDeafnessEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog) : base(owner, parent, prog)
    {
    }

    protected SpellDeafnessEffect(XElement root, IPerceivable owner) : base(root, owner)
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
        return "Deafened";
    }

    protected override string SpecificEffectType => "SpellDeafness";
}
