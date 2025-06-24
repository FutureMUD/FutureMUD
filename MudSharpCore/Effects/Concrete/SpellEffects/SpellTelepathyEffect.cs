using System;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellTelepathyEffect : MagicSpellEffectBase, ITelepathyEffect
{
    public static void InitialiseEffectType()
    {
        RegisterFactory("SpellTelepathy", (effect, owner) => new SpellTelepathyEffect(effect, owner));
    }

    public SpellTelepathyEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog, bool showThinks, bool showFeels, bool showEmote) : base(owner, parent, prog)
    {
        ShowThinks = showThinks;
        ShowFeels = showFeels;
        ShowThinkEmoteFlag = showEmote;
    }

    protected SpellTelepathyEffect(XElement root, IPerceivable owner) : base(root, owner)
    {
        var tr = root.Element("Effect");
        ShowThinks = bool.Parse(tr.Element("Thinks").Value);
        ShowFeels = bool.Parse(tr.Element("Feels").Value);
        ShowThinkEmoteFlag = bool.Parse(tr.Element("Emote").Value);
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
            new XElement("Thinks", ShowThinks),
            new XElement("Feels", ShowFeels),
            new XElement("Emote", ShowThinkEmoteFlag)
        );
    }

    public override string Describe(IPerceiver voyeur)
    {
        return "Telepathic link";
    }

    protected override string SpecificEffectType => "SpellTelepathy";

    public bool ShowThinks { get; set; }
    public bool ShowFeels { get; set; }
    private bool ShowThinkEmoteFlag { get; set; }

    public bool ShowDescription(ICharacter thinker) => true;
    public bool ShowName(ICharacter thinker) => false;
    public bool ShowThinkEmote(ICharacter thinker) => ShowThinkEmoteFlag;
}
