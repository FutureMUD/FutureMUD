using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class RoomAtmosphereEffect : IMagicSpellEffectTemplate
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("roomatmosphere", (root, spell) => new RoomAtmosphereEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("roomatmosphere", BuilderFactory,
            "Changes the atmosphere of a room",
            HelpText,
            true,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        var gas = spell.Gameworld.Gases.First();
        return (new RoomAtmosphereEffect(new XElement("Effect",
                new XAttribute("type", "roomatmosphere"),
                new XElement("AtmosphereId", gas.Id),
                new XElement("AtmosphereType", "gas"),
                new XElement("DescAddendum", new XCData("")),
                new XElement("AddendumColour", "bold cyan")
            ), spell), string.Empty);
    }

    protected RoomAtmosphereEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        var id = long.Parse(root.Element("AtmosphereId").Value);
        var type = root.Element("AtmosphereType").Value;
        Atmosphere = type.Equals("gas", StringComparison.InvariantCultureIgnoreCase)
            ? (IFluid)Gameworld.Gases.Get(id)
            : Gameworld.Liquids.Get(id);
        DescAddendum = root.Element("DescAddendum").Value;
        AddendumColour = Telnet.GetColour(root.Element("AddendumColour").Value);
    }

    public IMagicSpell Spell { get; }
    public IFluid Atmosphere { get; set; }
    public string DescAddendum { get; set; }
    public ANSIColour AddendumColour { get; set; }
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "roomatmosphere"),
            new XElement("AtmosphereId", Atmosphere.Id),
            new XElement("AtmosphereType", Atmosphere is IGas ? "gas" : "liquid"),
            new XElement("DescAddendum", new XCData(DescAddendum)),
            new XElement("AddendumColour", AddendumColour.Name)
        );
    }

    public const string HelpText = @"Options:
    #3gas <which>#0 - set a gas atmosphere
    #3liquid <which>#0 - set a liquid atmosphere
    #3desc <desc>#0 - room description addendum
    #3colour <colour>#0 - colour of addendum";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "gas":
            case "liquid":
                return BuildingCommandAtmosphere(actor, command, command.Last.Equals("gas") ? "gas" : "liquid");
            case "desc":
                return BuildingCommandDesc(actor, command);
            case "colour":
            case "color":
                return BuildingCommandColour(actor, command);
        }
        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandColour(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Colours: {Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
            return false;
        }
        var colour = Telnet.GetColour(command.SafeRemainingArgument);
        if (colour == null)
        {
            actor.OutputHandler.Send($"Invalid colour. Options: {Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
            return false;
        }
        AddendumColour = colour;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Colour set to {colour.Name.Colour(colour)}.");
        return true;
    }

    private bool BuildingCommandDesc(ICharacter actor, StringStack command)
    {
        DescAddendum = command.SafeRemainingArgument.SanitiseExceptNumbered(0);
        Spell.Changed = true;
        actor.OutputHandler.Send("Description addendum set.");
        return true;
    }

    private bool BuildingCommandAtmosphere(ICharacter actor, StringStack command, string type)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Specify which atmosphere.");
            return false;
        }
        if (type == "gas")
        {
            var gas = Gameworld.Gases.GetByIdOrName(command.SafeRemainingArgument);
            if (gas == null)
            {
                actor.OutputHandler.Send("No such gas.");
                return false;
            }
            Atmosphere = gas;
        }
        else
        {
            var liq = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
            if (liq == null)
            {
                actor.OutputHandler.Send("No such liquid.");
                return false;
            }
            Atmosphere = liq;
        }
        Spell.Changed = true;
        actor.OutputHandler.Send($"Atmosphere set to {Atmosphere.Name.ColourValue()}.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        return $"RoomAtmosphere {Atmosphere.Name.ColourValue()}";
    }

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
        if (target is not ILocation loc)
        {
            return null;
        }
        return new SpellRoomAtmosphereEffect(loc, parent, null, Atmosphere, DescAddendum, AddendumColour);
    }

    public IMagicSpellEffectTemplate Clone() => new RoomAtmosphereEffect(SaveToXml(), Spell);
}
