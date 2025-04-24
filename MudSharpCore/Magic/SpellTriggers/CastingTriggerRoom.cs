using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Magic.SpellTriggers;

public class CastingTriggerRoom : CastingTriggerBase
{
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("room", DoBuilderLoad,
			"Targets the caster's room",
			"room",
			new CastingTriggerRoom().BuildingCommandHelp
		);
		SpellTriggerFactory.RegisterLoadTimeFactory("room", (root, spell) => new CastingTriggerRoom(root, spell));
	}

	private static (IMagicTrigger Trigger, string Error) DoBuilderLoad(StringStack command, IMagicSpell spell)
	{
		return (
			new CastingTriggerRoom(
				new XElement("Trigger", new XElement("MinimumPower", (int)SpellPower.Insignificant),
					new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful)), spell), string.Empty);
	}

	protected CastingTriggerRoom(XElement definition, IMagicSpell spell) : base(definition, spell)
	{
	}

	protected CastingTriggerRoom() : base() { }

	#region Implementation of IXmlSavable

	public override XElement SaveToXml()
	{
		return new XElement("Trigger",
			new XAttribute("type", "room"),
			new XElement("MinimumPower", (int)MinimumPower),
			new XElement("MaximumPower", (int)MaximumPower)
		);
	}

	#endregion

	#region Implementation of IMagicTrigger

	public override IMagicTrigger Clone()
	{
		return new CastingTriggerRoom(SaveToXml(), Spell);
	}

	public override string Show(ICharacter actor)
	{
		return $"{"Cast@Room".ColourName()} - {base.Show(actor)}";
	}

	#endregion

	#region Overrides of CastingTriggerBase

	public override string SubtypeBuildingCommandHelp => string.Empty;

	public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
	{
		if (!CheckBaseTriggerCase(actor, additionalArguments, out var power))
		{
			return;
		}

		Spell.CastSpell(actor, actor.Location, power);
	}

	public override bool TriggerYieldsTarget => true;

	public override string TargetTypes => "room";

	public override string ShowPlayer(ICharacter actor)
	{
		return
			$"Cast Command - {Spell.School.SchoolVerb} cast {(Spell.Name.Contains(' ') ? Spell.Name.ToLowerInvariant().DoubleQuotes() : Spell.Name.ToLowerInvariant())} <power>";
	}

	#endregion
}