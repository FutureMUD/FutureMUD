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

public class RoomLightEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("roomlight", (root, spell) => new RoomLightEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("roomlight", BuilderFactory,
			"Creates a light that illuminates the room",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new RoomLightEffect(new XElement("Effect",
			new XAttribute("type", "roomlight"),
			new XElement("AddedLuxPerPower", 10),
			new XElement("DescAddendum", new XCData("A ball of {0} white light illuminates the area.")),
			new XElement("GlowAddendumColour", "bold white")), spell), string.Empty);
	}

	public IMagicSpell Spell { get; }
	public double AddedLuxPerPower { get; set; }
	public string DescAddendum { get; set; }
	public ANSIColour GlowAddendumColour { get; set; }

	protected RoomLightEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		AddedLuxPerPower = double.Parse(root.Element("AddedLuxPerPower").Value);
		DescAddendum = root.Element("DescAddendum").Value;
		GlowAddendumColour = Telnet.GetColour(root.Element("GlowAddendumColour").Value);
	}

	#region Implementation of IXmlSavable

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "roomlight"),
			new XElement("AddedLuxPerPower", AddedLuxPerPower),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("GlowAddendumColour", GlowAddendumColour.Name)
		);
	}

	#endregion

	#region Implementation of IHaveFuturemud

	public IFuturemud Gameworld => Spell.Gameworld;

	#endregion

	#region Implementation of IEditableItem

	public const string HelpText = @"You can use the following options with this effect:

	#3light <lux>#0 - sets the amount of added light per power level
	#3desc <desc>#0 - sets the room desc addendum e.g. There is a big ball of light here
	#3desc none#0 - sets there to be no room desc addendum (effect is invisible)
	#3colour <colour>#0 - sets the colour of the desc addenda

Note: You can use {0} in the desc addenda to have a light-level description (dim, bright, etc) inserted, e.g. ({0}ly glowing) might resolve as (brighly glowing)";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "lux":
			case "glow":
			case "light":
				return BuildingCommandLux(actor, command);
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

		GlowAddendumColour = colour;
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
				"What addendum should be made to the room desc from this effect? You can use the argument 'none' to have no addendum, and use {0} to substitute a light level adjective like 'dim', 'bright' etc.");
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
			$"Rooms with this effect will now have this room desc addendum:\n\n{DescAddendum.Colour(GlowAddendumColour)}");
		return true;
	}

	private bool BuildingCommandLux(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number of lux for this glow effect.");
			return false;
		}

		AddedLuxPerPower = value;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now cause {$"{value.ToString("N3", actor)} lux".ColourValue()} of illumination per spell power.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return
			$"RoomLightEffect - {AddedLuxPerPower.ToString("N3", actor)} lux - {DescAddendum.Colour(GlowAddendumColour)}";
	}

	#endregion

	#region Implementation of IMagicSpellEffectTemplate

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

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ILocation)
		{
			return null;
		}

		var lightLevel = Gameworld.LightModel.GetIlluminationDescription(AddedLuxPerPower * (int)power)
								  .ToLowerInvariant();
		return new SpellRoomLightEffect(target, parent, null, AddedLuxPerPower * (int)power,
			string.Format(DescAddendum, lightLevel), GlowAddendumColour);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new RoomLightEffect(SaveToXml(), Spell);
	}

	#endregion
}