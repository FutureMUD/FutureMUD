using System.Xml.Linq;
using System.Linq;
using MudSharp.Body.Needs;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.PerceptionEngine;
using MudSharp.Effects.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class NeedDeltaEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("needdelta", (root, spell) => new NeedDeltaEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("needdelta", BuilderFactory,
            "Instantly alters hunger, thirst or drunkenness",
            HelpText,
            true,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new NeedDeltaEffect(new XElement("Effect",
            new XAttribute("type", "needdelta"),
            new XElement("Hunger", 0.0),
            new XElement("Thirst", 0.0),
            new XElement("Drunk", 0.0)
        ), spell), string.Empty);
    }

    protected NeedDeltaEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        HungerDelta = double.Parse(root.Element("Hunger")!.Value);
        ThirstDelta = double.Parse(root.Element("Thirst")!.Value);
        DrunkDelta = double.Parse(root.Element("Drunk")!.Value);
    }

    public IMagicSpell Spell { get; }
    public double HungerDelta { get; set; }
    public double ThirstDelta { get; set; }
    public double DrunkDelta { get; set; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "needdelta"),
            new XElement("Hunger", HungerDelta),
            new XElement("Thirst", ThirstDelta),
            new XElement("Drunk", DrunkDelta)
        );
    }

    public const string HelpText = @"Options:
    #3hunger <hours>#0 - change hunger satiation hours
    #3thirst <hours>#0 - change thirst satiation hours
    #3drunk <litres>#0 - change alcohol litres";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "hunger":
                return BuildingCommandHunger(actor, command);
            case "thirst":
                return BuildingCommandThirst(actor, command);
            case "drunk":
            case "alcohol":
                return BuildingCommandDrunk(actor, command);
        }
        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandHunger(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
        {
            actor.OutputHandler.Send("You must enter a valid value in hours.");
            return false;
        }
        HungerDelta = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Hunger change set to {value:N2} hours.");
        return true;
    }

    private bool BuildingCommandThirst(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
        {
            actor.OutputHandler.Send("You must enter a valid value in hours.");
            return false;
        }
        ThirstDelta = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Thirst change set to {value:N2} hours.");
        return true;
    }

    private bool BuildingCommandDrunk(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
        {
            actor.OutputHandler.Send("You must enter a valid value in litres of alcohol.");
            return false;
        }
        DrunkDelta = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Drunkenness change set to {value:N2} litres.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        return $"NeedDelta H:{HungerDelta:N2} T:{ThirstDelta:N2} D:{DrunkDelta:N2}";
    }

    public bool IsInstantaneous => true;
    public bool RequiresTarget => true;

    public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => IsCompatibleWithTrigger(trigger.TargetTypes);
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
            return null;
        ch.Body.FulfilNeeds(new NeedFulfiller
        {
            SatiationPoints = HungerDelta,
            ThirstPoints = ThirstDelta,
            AlcoholLitres = DrunkDelta
        });
        return null;
    }

    public IMagicSpellEffectTemplate Clone() => new NeedDeltaEffect(SaveToXml(), Spell);
}
