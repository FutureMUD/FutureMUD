using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class DeafnessEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("deafness", (root, spell) => new DeafnessEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("deafness", BuilderFactory,
            "Temporarily deafens the target",
            "",
            false,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new DeafnessEffect(new XElement("Effect",
            new XAttribute("type", "deafness")
        ), spell), string.Empty);
    }

    protected DeafnessEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
    }

    public IMagicSpell Spell { get; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml() => new XElement("Effect", new XAttribute("type", "deafness"));

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        actor.OutputHandler.Send("No options for this effect.");
        return false;
    }

    public string Show(ICharacter actor) => "Deafness";

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
        return new SpellDeafnessEffect(ch, parent, null);
    }

    public IMagicSpellEffectTemplate Clone() => new DeafnessEffect(SaveToXml(), Spell);
}
