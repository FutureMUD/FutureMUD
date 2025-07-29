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

public class StaminaExpenditureSpellEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("staminaexpendrate", (root, spell) => new StaminaExpenditureSpellEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("staminaexpendrate", BuilderFactory,
            "Modifies stamina expenditure",
            HelpText,
            false,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new StaminaExpenditureSpellEffect(new XElement("Effect",
            new XAttribute("type", "staminaexpendrate"),
            new XElement("Multiplier", 1.0)
        ), spell), string.Empty);
    }

    protected StaminaExpenditureSpellEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        Multiplier = double.Parse(root.Element("Multiplier")!.Value);
    }

    public IMagicSpell Spell { get; }
    public double Multiplier { get; set; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "staminaexpendrate"),
            new XElement("Multiplier", Multiplier)
        );
    }

    public const string HelpText = @"Options:
    #3multiplier <##>#0 - sets the stamina expenditure multiplier";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
        {
            actor.OutputHandler.Send("You must enter a valid multiplier.");
            return false;
        }
        Multiplier = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Stamina expenditure multiplier set to {value.ToStringP2(actor)}.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        return $"StaminaExpendRate x{Multiplier.ToStringP2(actor)}";
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
        return new SpellStaminaExpenditureEffect(ch, parent, null, Multiplier);
    }

    public IMagicSpellEffectTemplate Clone() => new StaminaExpenditureSpellEffect(SaveToXml(), Spell);
}
