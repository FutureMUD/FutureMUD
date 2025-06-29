using System.Xml.Linq;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class WeatherFreezeEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("weatherfreeze", (root, spell) => new WeatherFreezeEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("weatherfreeze", BuilderFactory,
            "Prevents weather from changing", "", false, true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new WeatherFreezeEffect(new XElement("Effect",
            new XAttribute("type", "weatherfreeze")
        ), spell), string.Empty);
    }

    protected WeatherFreezeEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
    }

    public IMagicSpell Spell { get; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml() => new XElement("Effect", new XAttribute("type", "weatherfreeze"));

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        actor.OutputHandler.Send("No options for this effect.");
        return false;
    }

    public string Show(ICharacter actor) => "WeatherFreeze";

    public bool IsInstantaneous => false;
    public bool RequiresTarget => true;

    public bool IsCompatibleWithTrigger(IMagicTrigger types) => IsCompatibleWithTrigger(types.TargetTypes);
    public static bool IsCompatibleWithTrigger(string types)
    {
        switch (types)
        {
            case "room":
            case "rooms":
                return true;
            default:
                return false;
        }
    }

    public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
    {
        if (target is not ILocation loc || loc.WeatherController is null)
        {
            return null;
        }
        loc.WeatherController.FreezeWeather();
        return new SpellWeatherFreezeEffect(loc, parent, null);
    }

    public IMagicSpellEffectTemplate Clone() => new WeatherFreezeEffect(SaveToXml(), Spell);
}
