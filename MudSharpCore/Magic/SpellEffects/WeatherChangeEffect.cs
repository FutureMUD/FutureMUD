using System.Xml.Linq;
using System.Linq;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class WeatherChangeEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("weatherchange", (root, spell) => new WeatherChangeEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("weatherchange", BuilderFactory,
            "Changes the current weather event", HelpText, true, true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        var weather = spell.Gameworld.WeatherEvents.First();
        return (new WeatherChangeEffect(new XElement("Effect",
                new XAttribute("type", "weatherchange"),
                new XElement("WeatherEvent", weather.Id),
                new XElement("NextTransition", false)
            ), spell), string.Empty);
    }

    protected WeatherChangeEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        WeatherEvent = Gameworld.WeatherEvents.Get(long.Parse(root.Element("WeatherEvent").Value));
        NextTransition = bool.Parse(root.Element("NextTransition").Value);
    }

    public IMagicSpell Spell { get; }
    public IWeatherEvent WeatherEvent { get; set; }
    public bool NextTransition { get; set; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "weatherchange"),
            new XElement("WeatherEvent", WeatherEvent.Id),
            new XElement("NextTransition", NextTransition)
        );
    }

    public const string HelpText = "Options:\n    #3event <which>#0 - set the weather event\n    #3next|immediate#0 - whether to set on next transition";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "event":
                return BuildingCommandEvent(actor, command);
            case "next":
            case "immediate":
                NextTransition = command.Last.EqualTo("next");
                Spell.Changed = true;
                actor.OutputHandler.Send($"Weather change will occur {(NextTransition ? "on the next transition" : "immediately")}.");
                return true;
        }
        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandEvent(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which weather event?");
            return false;
        }
        var we = Gameworld.WeatherEvents.GetByIdOrName(command.SafeRemainingArgument);
        if (we == null)
        {
            actor.OutputHandler.Send("No such weather event.");
            return false;
        }
        WeatherEvent = we;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Weather event set to {we.Name.ColourValue()}.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        return $"WeatherChange {WeatherEvent.Name.ColourValue()} {(NextTransition ? "next" : "immediate")}";
    }

    public bool IsInstantaneous => true;
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

        if (NextTransition)
        {
            void Handler(IWeatherController sender, IWeatherEvent oldw, IWeatherEvent neww)
            {
                sender.WeatherChanged -= Handler;
                sender.SetWeather(WeatherEvent);
            }
            loc.WeatherController.WeatherChanged += Handler;
        }
        else
        {
            loc.WeatherController.SetWeather(WeatherEvent);
        }
        return null;
    }

    public IMagicSpellEffectTemplate Clone() => new WeatherChangeEffect(SaveToXml(), Spell);
}
