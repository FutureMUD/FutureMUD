using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellStaminaExpenditureEffect : MagicSpellEffectBase, IStaminaExpenditureEffect
{
    public static void InitialiseEffectType()
    {
        RegisterFactory("SpellStaminaExpenditure", (effect, owner) => new SpellStaminaExpenditureEffect(effect, owner));
    }

    public SpellStaminaExpenditureEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog, double multiplier) : base(owner, parent, prog)
    {
        Multiplier = multiplier;
    }

    protected SpellStaminaExpenditureEffect(XElement root, IPerceivable owner) : base(root, owner)
    {
        var trueRoot = root.Element("Effect");
        Multiplier = double.Parse(trueRoot.Element("Multiplier")!.Value);
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
            new XElement("Multiplier", Multiplier)
        );
    }

    public override string Describe(IPerceiver voyeur)
    {
        return $"Stamina Cost x{Multiplier:N2}";
    }

    protected override string SpecificEffectType => "SpellStaminaExpenditure";

    public double Multiplier { get; set; }
}
