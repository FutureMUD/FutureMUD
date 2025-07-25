using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellStaminaRegenerationEffect : MagicSpellEffectBase, IStaminaRegenerationRateEffect
{
    public static void InitialiseEffectType()
    {
        RegisterFactory("SpellStaminaRegen", (effect, owner) => new SpellStaminaRegenerationEffect(effect, owner));
    }

    public SpellStaminaRegenerationEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog, double multiplier) : base(owner, parent, prog)
    {
        Multiplier = multiplier;
    }

    protected SpellStaminaRegenerationEffect(XElement root, IPerceivable owner) : base(root, owner)
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
        return $"Stamina Regen x{Multiplier:N2}";
    }

    protected override string SpecificEffectType => "SpellStaminaRegen";

    public double Multiplier { get; set; }
}
