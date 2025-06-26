using System;
using System.Xml.Linq;
using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class HealingRateSpellEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("healingrate", (root, spell) => new HealingRateSpellEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("healingrate", BuilderFactory,
            "Modifies natural healing rate and difficulty",
            HelpText,
            false,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new HealingRateSpellEffect(new XElement("Effect",
                        new XAttribute("type", "healingrate"),
                        new XElement("Multiplier", 1.0),
                        new XElement("Stages", 0)
                    ), spell), string.Empty);
    }

    protected HealingRateSpellEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        Multiplier = double.Parse(root.Element("Multiplier")?.Value ?? "1.0");
        Stages = int.Parse(root.Element("Stages")?.Value ?? "0");
    }

    public IMagicSpell Spell { get; }

    public double Multiplier { get; set; }
    public int Stages { get; set; }

    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "healingrate"),
            new XElement("Multiplier", Multiplier),
            new XElement("Stages", Stages)
        );
    }

    public const string HelpText = @"You can use the following options with this effect:
    #3multiplier <##>#0 - sets the healing rate multiplier
    #3stages <##>#0 - sets the bonus or penalty stages to healing checks";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "multiplier":
            case "rate":
                return BuildingCommandMultiplier(actor, command);
            case "stages":
            case "difficulty":
                return BuildingCommandStages(actor, command);
        }

        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandMultiplier(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
        {
            actor.OutputHandler.Send("You must enter a valid multiplier.");
            return false;
        }

        Multiplier = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"The healing rate multiplier is now {Multiplier.ToString("N3", actor).ColourValue()}.");
        return true;
    }

    private bool BuildingCommandStages(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value))
        {
            actor.OutputHandler.Send("You must enter a valid integer number of stages.");
            return false;
        }

        Stages = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"The healing difficulty adjustment is now {Stages.ToString("N0", actor).ColourValue()} stages.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        return $"HealingRate - x{Multiplier.ToString("N2", actor)} {(Stages>=0?"+":"")}{Stages}";
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

        return new SpellHealingRateEffect(ch, parent, null, Multiplier, Stages);
    }

    public IMagicSpellEffectTemplate Clone() => new HealingRateSpellEffect(SaveToXml(), Spell);
}
