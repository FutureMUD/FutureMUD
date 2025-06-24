using System;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellWeightEffect : MagicSpellEffectBase, IEffectAddsWeight
{
    public static void InitialiseEffectType()
    {
        RegisterFactory("SpellWeight", (effect, owner) => new SpellWeightEffect(effect, owner));
    }

    public SpellWeightEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog, double weight) : base(owner, parent, prog)
    {
        AddedWeight = weight;
    }

    protected SpellWeightEffect(XElement root, IPerceivable owner) : base(root, owner)
    {
        var tr = root.Element("Effect");
        AddedWeight = double.Parse(tr.Element("Weight").Value);
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
            new XElement("Weight", AddedWeight)
        );
    }

    public override string Describe(IPerceiver voyeur)
    {
        return $"Adds {Gameworld.UnitManager.DescribeExact(AddedWeight, UnitType.Mass, voyeur)} of weight.";
    }

    protected override string SpecificEffectType => "SpellWeight";

    public double AddedWeight { get; set; }
}
