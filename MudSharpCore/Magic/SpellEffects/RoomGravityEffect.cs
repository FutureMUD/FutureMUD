using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public class RoomGravityEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("roomgravity", (root, spell) => new RoomGravityEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("roomgravity", BuilderFactory,
			"Changes the gravity model of a room",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
	{
		return (new RoomGravityEffect(new XElement("Effect",
			new XAttribute("type", "roomgravity"),
			new XElement("GravityModel", (int)GravityModel.ZeroGravity),
			new XElement("DescAddendum", new XCData("The room seems strangely free of gravity.")),
			new XElement("AddendumColour", "bold cyan")
		), spell), string.Empty);
	}

	protected RoomGravityEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		GravityModel = (GravityModel)int.Parse(root.Element("GravityModel")!.Value);
		DescAddendum = root.Element("DescAddendum")?.Value ?? string.Empty;
		AddendumColour = Telnet.GetColour(root.Element("AddendumColour")?.Value ?? "bold cyan");
	}

	public IMagicSpell Spell { get; }
	public GravityModel GravityModel { get; set; }
	public string DescAddendum { get; set; }
	public ANSIColour AddendumColour { get; set; }
	public IFuturemud Gameworld => Spell.Gameworld;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "roomgravity"),
			new XElement("GravityModel", (int)GravityModel),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("AddendumColour", AddendumColour.Name)
		);
	}

	public const string HelpText = @"Options:
    #3gravity normal|zerog#0 - set the gravity model
    #3desc <desc>#0 - room description addendum
    #3colour <colour>#0 - colour of addendum";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "gravity":
				return BuildingCommandGravity(actor, command);
			case "desc":
				return BuildingCommandDesc(actor, command);
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandGravity(ICharacter actor, StringStack command)
	{
		switch (command.SafeRemainingArgument.ToLowerInvariant())
		{
			case "normal":
			case "standard":
			case "1g":
				GravityModel = MudSharp.Construction.GravityModel.Normal;
				break;
			case "zero":
			case "zero g":
			case "zero-g":
			case "zerog":
			case "zerogravity":
			case "microgravity":
				GravityModel = MudSharp.Construction.GravityModel.ZeroGravity;
				break;
			default:
				actor.OutputHandler.Send("Valid gravity models are normal and zerog.");
				return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send($"Gravity model set to {GravityModel.DescribeColour()}.");
		return true;
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

	public string Show(ICharacter actor)
	{
		return $"RoomGravity {GravityModel.DescribeColour()}";
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger types)
	{
		return IsCompatibleWithTrigger(types.TargetTypes);
	}

	public static bool IsCompatibleWithTrigger(string types)
	{
		return types is "room" or "rooms";
	}

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return target is ILocation loc
			? new SpellRoomGravityEffect(loc, parent, null, GravityModel, DescAddendum, AddendumColour)
			: null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new RoomGravityEffect(SaveToXml(), Spell);
	}
}
