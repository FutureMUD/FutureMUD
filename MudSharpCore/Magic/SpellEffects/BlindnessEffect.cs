using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

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

    public string Show(ICharacter actor) => "Blindness";

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
        return new SpellBlindnessEffect(ch, parent, null);
    }

    public IMagicSpellEffectTemplate Clone() => new BlindnessEffect(SaveToXml(), Spell);
}
