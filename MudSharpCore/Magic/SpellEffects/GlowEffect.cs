using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class GlowEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("glow", (root, spell) => new GlowEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("glow", BuilderFactory);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new GlowEffect(new XElement("Effect",
			new XAttribute("type", "glow"),
			new XElement("GlowLuxPerPower", 10),
			new XElement("SDescAddendum", new XCData("(glowing)")),
			new XElement("DescAddendum", new XCData("A {0} white glow emanates from this individual.")),
			new XElement("GlowAddendumColour", "bold white")), spell), string.Empty);
	}

	public IMagicSpell Spell { get; }
	public double GlowLuxPerPower { get; set; }
	public string SDescAddendum { get; set; }
	public string DescAddendum { get; set; }
	public ANSIColour GlowAddendumColour { get; set; }

	public GlowEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		GlowLuxPerPower = double.Parse(root.Element("GlowLuxPerPower").Value);
		SDescAddendum = root.Element("SDescAddendum").Value;
		DescAddendum = root.Element("DescAddendum").Value;
		GlowAddendumColour = Telnet.GetColour(root.Element("GlowAddendumColour").Value);
	}

	#region Implementation of IXmlSavable

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "glow"),
			new XElement("GlowLuxPerPower", GlowLuxPerPower),
			new XElement("SDescAddendum", new XCData(SDescAddendum)),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("GlowAddendumColour", GlowAddendumColour.Name)
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
			case "lux":
			case "glow":
				return BuildingCommandLux(actor, command);
			case "sdesc":
				return BuildingCommandSDesc(actor, command);
			case "desc":
				return BuildingCommandDesc(actor, command);
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
		}

		actor.OutputHandler.Send(@"You can use the following options with this effect:

    glow <lux> - sets the amount of glow per power level
    sdesc <sdesc> - sets the sdesc addendum e.g. (glowing)
    desc <desc> - sets the look desc addendum e.g. This person is glowing
    colour <colour> - sets the colour of the sdesc/desc addenda

Note: You can use {0} in the sdesc/desc addenda to have a light-level description (dim, bright, etc) inserted, e.g. ({0}ly glowing) might resolve as (brightly glowing)");
		return false;
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What colour do you want to set for the echoes? The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
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
			$"The description addenda for this effect will now be coloured {colour.Name.Colour(colour)}.");
		return true;
	}

	private bool BuildingCommandDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What addendum should be made to the full desc (when looking at) of the target from this effect? Use {0} to substitute a light level adjective like 'dim', 'bright' etc.");
			return false;
		}

		var text = command.SafeRemainingArgument.SanitiseExceptNumbered(0);
		DescAddendum = text;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"Targets with this effect will now have this full desc addendum: {DescAddendum.Colour(GlowAddendumColour)}");
		return true;
	}

	private bool BuildingCommandSDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What addendum should be made to the sdesc of the target from this effect? Use {0} to substitute a light level adjective like 'dim', 'bright' etc.");
			return false;
		}

		var text = command.SafeRemainingArgument.SanitiseExceptNumbered(0);
		SDescAddendum = text;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"Targets with this effect will now have this sdesc addendum: {SDescAddendum.Colour(GlowAddendumColour)}");
		return true;
	}

	private bool BuildingCommandLux(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number of lux for this glow effect.");
			return false;
		}

		GlowLuxPerPower = value;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now cause {$"{value.ToString("N3", actor)} lux".ColourValue()} of illumination per spell power.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return
			$"GlowEffect - {GlowLuxPerPower.ToString("N3", actor)} lux - {SDescAddendum.Colour(GlowAddendumColour)} - {DescAddendum.Colour(GlowAddendumColour)}";
	}

	#endregion

	#region Implementation of IMagicSpellEffectTemplate

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent)
	{
		if (target is not IGameItem && target is not ICharacter)
		{
			return null;
		}

		var lightLevel = Gameworld.LightModel.GetIlluminationDescription(GlowLuxPerPower * (int)power)
		                          .ToLowerInvariant();
		return new SpellGlowEffect(target, parent, null, GlowLuxPerPower * (int)power,
			string.Format(SDescAddendum, lightLevel), string.Format(DescAddendum, lightLevel), GlowAddendumColour);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new GlowEffect(SaveToXml(), Spell);
	}

	#endregion
}