using System.Xml.Linq;
using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Magic.SpellEffects;

public class NeedRateSpellEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("needrate", (root, spell) => new NeedRateSpellEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("needrate", BuilderFactory,
            "Alters need gain or loss rates",
            HelpText,
            false,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new NeedRateSpellEffect(new XElement("Effect",
            new XAttribute("type", "needrate"),
            new XElement("HungerMult", 1.0),
            new XElement("ThirstMult", 1.0),
            new XElement("DrunkMult", 1.0),
            new XElement("Passive", true),
            new XElement("Active", false)
        ), spell), string.Empty);
    }

    protected NeedRateSpellEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        HungerMultiplier = double.Parse(root.Element("HungerMult")!.Value);
        ThirstMultiplier = double.Parse(root.Element("ThirstMult")!.Value);
        DrunkennessMultiplier = double.Parse(root.Element("DrunkMult")!.Value);
        AppliesToPassive = bool.Parse(root.Element("Passive")!.Value);
        AppliesToActive = bool.Parse(root.Element("Active")!.Value);
    }

    public IMagicSpell Spell { get; }
    public double HungerMultiplier { get; set; }
    public double ThirstMultiplier { get; set; }
    public double DrunkennessMultiplier { get; set; }
    public bool AppliesToPassive { get; set; }
    public bool AppliesToActive { get; set; }

    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "needrate"),
            new XElement("HungerMult", HungerMultiplier),
            new XElement("ThirstMult", ThirstMultiplier),
            new XElement("DrunkMult", DrunkennessMultiplier),
            new XElement("Passive", AppliesToPassive),
            new XElement("Active", AppliesToActive)
        );
    }

    public const string HelpText = @"Options:
    #3hunger <%>#0 - multiplier to hunger changes
    #3thirst <%>#0 - multiplier to thirst changes
    #3drunk <%>#0 - multiplier to drunkenness changes
    #3passive|active#0 - set whether this affects passive or active changes";

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
            case "passive":
                AppliesToPassive = true;
                AppliesToActive = false;
                Spell.Changed = true;
                actor.OutputHandler.Send("This effect will now modify passive need loss rates.");
                return true;
            case "active":
                AppliesToPassive = false;
                AppliesToActive = true;
                Spell.Changed = true;
                actor.OutputHandler.Send("This effect will now modify active need fulfilment.");
                return true;
        }
        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandHunger(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
        {
            actor.OutputHandler.Send("Enter a valid percentage.");
            return false;
        }
        HungerMultiplier = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Hunger multiplier now {value:P2}.");
        return true;
    }

    private bool BuildingCommandThirst(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
        {
            actor.OutputHandler.Send("Enter a valid percentage.");
            return false;
        }
        ThirstMultiplier = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Thirst multiplier now {value:P2}.");
        return true;
    }

    private bool BuildingCommandDrunk(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
        {
            actor.OutputHandler.Send("Enter a valid percentage.");
            return false;
        }
        DrunkennessMultiplier = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Drunkenness multiplier now {value:P2}.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        var type = AppliesToPassive ? "Passive" : "Active";
        return $"NeedRate {type} - H:{HungerMultiplier:P2} T:{ThirstMultiplier:P2} D:{DrunkennessMultiplier:P2}";
    }

    public bool IsInstantaneous => false;
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
        return new SpellNeedRateEffect(ch, parent, null, HungerMultiplier, ThirstMultiplier, DrunkennessMultiplier, AppliesToPassive, AppliesToActive);
    }

    public IMagicSpellEffectTemplate Clone() => new NeedRateSpellEffect(SaveToXml(), Spell);
}
