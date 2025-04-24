using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Magic.SpellTriggers;

public class CastingTriggerSelf : CastingTriggerBase
{
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("self", DoBuilderLoad,
			"Targets the caster",
			"character",
			new CastingTriggerSelf().BuildingCommandHelp
		);
		SpellTriggerFactory.RegisterLoadTimeFactory("self", (root, spell) => new CastingTriggerSelf(root, spell));
	}

	private static (IMagicTrigger Trigger, string Error) DoBuilderLoad(StringStack command, IMagicSpell spell)
	{
		return (
			new CastingTriggerSelf(
				new XElement("Trigger", new XElement("MinimumPower", (int)SpellPower.Insignificant),
					new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful)), spell), string.Empty);
	}

	protected CastingTriggerSelf(XElement definition, IMagicSpell spell) : base(definition, spell)
	{
	}

	protected CastingTriggerSelf() : base() { }

	#region Implementation of IXmlSavable

	public override XElement SaveToXml()
	{
		return new XElement("Trigger",
			new XAttribute("type", "self"),
			new XElement("MinimumPower", (int)MinimumPower),
			new XElement("MaximumPower", (int)MaximumPower)
		);
	}

	#endregion

	#region Implementation of IMagicTrigger

	public override IMagicTrigger Clone()
	{
		return new CastingTriggerSelf(SaveToXml(), Spell);
	}

	public override string Show(ICharacter actor)
	{
		return $"{"Cast@Self".ColourName()} - {base.Show(actor)}";
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

		Spell.CastSpell(actor, actor, power);
	}

	public override bool TriggerYieldsTarget => true;

	/// <inheritdoc />
	public override bool TriggerMayFailToYieldTarget => false;

	public override string TargetTypes => "character";

	public override string ShowPlayer(ICharacter actor)
	{
		return
			$"Cast Command - {Spell.School.SchoolVerb} cast {(Spell.Name.Contains(' ') ? Spell.Name.ToLowerInvariant().DoubleQuotes() : Spell.Name.ToLowerInvariant())} <power>";
	}

	#endregion
}