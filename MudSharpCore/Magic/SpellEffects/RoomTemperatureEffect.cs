using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class RoomTemperatureEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("roomtemperature",
			(root, spell) => new RoomTemperatureEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("roomtemperature", BuilderFactory);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new GlowEffect(new XElement("Effect",
			new XAttribute("type", "roomtemperature"),
			new XElement("TemperatureDeltaPerPower", 1.0),
			new XElement("DescAddendum", new XCData("")),
			new XElement("AddendumColour", "bold red")), spell), string.Empty);
	}

	public IMagicSpell Spell { get; }
	public double TemperatureDeltaPerPower { get; set; }
	public string DescAddendum { get; set; }
	public ANSIColour AddendumColour { get; set; }

	public RoomTemperatureEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		TemperatureDeltaPerPower = double.Parse(root.Element("TemperatureDeltaPerPower").Value);
		DescAddendum = root.Element("DescAddendum").Value;
		AddendumColour = Telnet.GetColour(root.Element("AddendumColour").Value);
	}

	#region Implementation of IXmlSavable

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "roomtemperature"),
			new XElement("TemperatureDeltaPerPower", TemperatureDeltaPerPower),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("AddendumColour", AddendumColour.Name)
		);
	}

	#endregion

	#region Implementation of IHaveFuturemud

	public IFuturemud Gameworld => Spell.Gameworld;

	#endregion

	#region Implementation of IEditableItem

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "temperature":
			case "temp":
			case "heat":
			case "delta":
				return BuildingCommandTemperature(actor, command);
			case "desc":
				return BuildingCommandDesc(actor, command);
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
		}

		actor.OutputHandler.Send(@"You can use the following options with this effect:

    #3temp <degrees>#0 - sets the amount of temperature change per power in degrees
    #3desc <desc>#0 - sets the room desc addendum e.g. An unholy chill sits over this area
    #3desc none#0 - sets there to be no room desc addendum (effect is invisible)
    #3colour <colour>#0 - sets the colour of the desc addenda");
		return false;
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What colour do you want to set for the addendum? The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour == null)
		{
			actor.OutputHandler.Send(
				$"That is not a valid colour. The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		AddendumColour = colour;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"The room description addenda for this effect will now be coloured {colour.Name.Colour(colour)}.");
		return true;
	}

	private bool BuildingCommandDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What addendum should be made to the room desc from this effect? You can use the argument 'none' to set no addendum.");
			return false;
		}

		var text = command.SafeRemainingArgument.SanitiseExceptNumbered(0);
		if (text.EqualTo("none"))
		{
			DescAddendum = string.Empty;
			Spell.Changed = true;
			actor.OutputHandler.Send($"Rooms with this effect will no longer have a room desc addendum.");
			return true;
		}

		DescAddendum = text;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"Rooms with this effect will now have this room desc addendum:\n\n{DescAddendum.Colour(AddendumColour)}");
		return true;
	}

	private bool BuildingCommandTemperature(ICharacter actor, StringStack command)
	{
		var result = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument,
			Framework.Units.UnitType.TemperatureDelta, out var success);
		if (!success)
		{
			actor.OutputHandler.Send("You must enter a valid number of degrees difference for this effect.");
			return false;
		}

		TemperatureDeltaPerPower = result;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now cause a temperature delta of {Gameworld.UnitManager.DescribeBonus(result, Framework.Units.UnitType.TemperatureDelta, actor).ColourValue()} per spell power.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return
			$"RoomTemperatureEffect - {TemperatureDeltaPerPower.ToString("N3", actor)} lux - {DescAddendum.Colour(AddendumColour)}";
	}

	#endregion

	#region Implementation of IMagicSpellEffectTemplate

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent)
	{
		if (target is not ILocation)
		{
			return null;
		}

		return new SpellRoomLightEffect(target, parent, null, TemperatureDeltaPerPower * (int)power, DescAddendum,
			AddendumColour);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new RoomLightEffect(SaveToXml(), Spell);
	}

	#endregion
}