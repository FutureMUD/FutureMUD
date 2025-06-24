using System;
using System.Xml.Linq;
using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class TelepathySpellEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("telepathy", (root, spell) => new TelepathySpellEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("telepathy", BuilderFactory,
            "Lets the caster hear the target's thoughts or feelings",
            HelpText,
            false,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new TelepathySpellEffect(new XElement("Effect",
                        new XAttribute("type", "telepathy"),
                        new XElement("Thinks", true),
                        new XElement("Feels", true),
                        new XElement("Emote", true)
                    ), spell), string.Empty);
    }

    protected TelepathySpellEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        ShowThinks = bool.Parse(root.Element("Thinks")?.Value ?? "true");
        ShowFeels = bool.Parse(root.Element("Feels")?.Value ?? "true");
        ShowEmote = bool.Parse(root.Element("Emote")?.Value ?? "true");
    }

    public IMagicSpell Spell { get; }
    public bool ShowThinks { get; set; }
    public bool ShowFeels { get; set; }
    public bool ShowEmote { get; set; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "telepathy"),
            new XElement("Thinks", ShowThinks),
            new XElement("Feels", ShowFeels),
            new XElement("Emote", ShowEmote)
        );
    }

    public const string HelpText = @"You can use the following options with this effect:
    #3thinks#0 - toggles showing target thoughts
    #3feels#0 - toggles showing target feelings
    #3emote#0 - toggles showing think emotes";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "thinks":
                ShowThinks = !ShowThinks;
                Spell.Changed = true;
                actor.OutputHandler.Send($"This spell will {(ShowThinks ? "now" : "no longer")} transmit thoughts.");
                return true;
            case "feels":
                ShowFeels = !ShowFeels;
                Spell.Changed = true;
                actor.OutputHandler.Send($"This spell will {(ShowFeels ? "now" : "no longer")} transmit feelings.");
                return true;
            case "emote":
                ShowEmote = !ShowEmote;
                Spell.Changed = true;
                actor.OutputHandler.Send($"This spell will {(ShowEmote ? "now" : "no longer")} show think emotes.");
                return true;
        }

        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    public string Show(ICharacter actor)
    {
        return $"Telepathy {(ShowThinks ? "[Thinks]" : "")} {(ShowFeels ? "[Feels]" : "")} {(ShowEmote ? "[Emote]" : "")}";
    }

    public bool IsInstantaneous => false;
    public bool RequiresTarget => true;

    public bool IsCompatibleWithTrigger(IMagicTrigger types) => IsCompatibleWithTrigger(types.TargetTypes);
    public static bool IsCompatibleWithTrigger(string types)
    {
        switch (types)
        {
            case "character":
            case "characters":
                return true;
            default:
                return false;
        }
    }

    public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
    {
        if (target is not ICharacter ch)
        {
            return null;
        }

        return new SpellTelepathyEffect(ch, parent, null, ShowThinks, ShowFeels, ShowEmote);
    }

    public IMagicSpellEffectTemplate Clone() => new TelepathySpellEffect(SaveToXml(), Spell);
}
