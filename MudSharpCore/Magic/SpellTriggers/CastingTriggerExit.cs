using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using System.Xml.Linq;

#nullable enable
#nullable disable warnings

namespace MudSharp.Magic.SpellTriggers;

public class CastingTriggerExit : CastingTriggerBase
{
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("exit", DoBuilderLoad,
			"Targets a local exit or direction",
			"exit",
			new CastingTriggerExit().BuildingCommandHelp
		);
		SpellTriggerFactory.RegisterLoadTimeFactory("exit", (root, spell) => new CastingTriggerExit(root, spell));
	}

	private static (IMagicTrigger Trigger, string Error) DoBuilderLoad(StringStack command, IMagicSpell spell)
	{
		return (
			new CastingTriggerExit(
				new XElement("Trigger",
					new XElement("MinimumPower", (int)SpellPower.Insignificant),
					new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful)
				), spell), string.Empty);
	}

	protected CastingTriggerExit(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
	}

	protected CastingTriggerExit()
		: base()
	{
	}

	public override XElement SaveToXml()
	{
		return new XElement("Trigger",
			new XAttribute("type", "exit"),
			new XElement("MinimumPower", (int)MinimumPower),
			new XElement("MaximumPower", (int)MaximumPower)
		);
	}

	public override IMagicTrigger Clone()
	{
		return new CastingTriggerExit(SaveToXml(), Spell);
	}

	public override string Show(ICharacter actor)
	{
		return $"{"Cast@Exit".ColourName()} - {base.Show(actor)}";
	}

	public override string SubtypeBuildingCommandHelp => string.Empty;

	public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
	{
		if (!CheckBaseTriggerCase(actor, additionalArguments, out SpellPower power))
		{
			return;
		}

		if (additionalArguments.IsFinished)
		{
			actor.OutputHandler.Send("That spell requires an exit target. You must specify a local exit or direction.");
			return;
		}

		ICellExit? exit = CastingTriggerExitHelper.ResolveExit(actor, additionalArguments.PopSpeech());
		if (exit is null)
		{
			actor.OutputHandler.Send("You do not see any exit like that here.");
			return;
		}

		Spell.CastSpell(actor, exit.Exit, power, new SpellAdditionalParameter { ParameterName = "exit", Item = exit });
	}

	public override bool TriggerYieldsTarget => true;
	public override bool TriggerMayFailToYieldTarget => false;
	public override string TargetTypes => "exit";

	public override string ShowPlayer(ICharacter actor)
	{
		return
			$"Cast Command - {Spell.School.SchoolVerb} cast {(Spell.Name.Contains(' ') ? Spell.Name.ToLowerInvariant().DoubleQuotes() : Spell.Name.ToLowerInvariant())} <power> <exit>";
	}
}
