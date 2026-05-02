using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public class BlindnessEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("blindness", (root, spell) => new BlindnessEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("blindness", BuilderFactory,
            "Temporarily blinds the target",
            "",
            false,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new BlindnessEffect(new XElement("Effect",
            new XAttribute("type", "blindness")
        ), spell), string.Empty);
    }

    protected BlindnessEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
    }

    public IMagicSpell Spell { get; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "blindness")
        );
    }

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        actor.OutputHandler.Send("No options for this effect.");
        return false;
    }

    public string Show(ICharacter actor)
    {
        return "Blindness";
    }

    public bool IsInstantaneous => false;
    public bool RequiresTarget => true;

    public bool IsCompatibleWithTrigger(IMagicTrigger types)
    {
        return IsCompatibleWithTrigger(types.TargetTypes);
    }

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
        return new SpellBlindnessEffect(ch, parent, null);
    }

    public IMagicSpellEffectTemplate Clone()
    {
        return new BlindnessEffect(SaveToXml(), Spell);
    }
}

public class RemoveBlindnessEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("removeblindness", (root, spell) => new RemoveBlindnessEffect(root, spell));
        SpellEffectFactory.RegisterLoadTimeFactory("cureblindness", (root, spell) => new RemoveBlindnessEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("removeblindness", BuilderFactory,
            "Removes magical blindness from the target",
            "",
            true,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => BlindnessEffect.IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
        SpellEffectFactory.RegisterBuilderFactory("cureblindness", BuilderFactory,
            "Removes magical blindness from the target",
            "",
            true,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => BlindnessEffect.IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new RemoveBlindnessEffect(new XElement("Effect",
            new XAttribute("type", "removeblindness")
        ), spell), string.Empty);
    }

    protected RemoveBlindnessEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
    }

    public IMagicSpell Spell { get; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "removeblindness")
        );
    }

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        actor.OutputHandler.Send("No options for this effect.");
        return false;
    }

    public string Show(ICharacter actor)
    {
        return "Remove Blindness";
    }

    public bool IsInstantaneous => true;
    public bool RequiresTarget => true;

    public bool IsCompatibleWithTrigger(IMagicTrigger types)
    {
        return BlindnessEffect.IsCompatibleWithTrigger(types.TargetTypes);
    }

    public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
    {
        if (target is ICharacter ch)
        {
            ch.RemoveAllEffects<SpellBlindnessEffect>(null, true);
        }

        return null;
    }

    public IMagicSpellEffectTemplate Clone()
    {
        return new RemoveBlindnessEffect(SaveToXml(), Spell);
    }
}
