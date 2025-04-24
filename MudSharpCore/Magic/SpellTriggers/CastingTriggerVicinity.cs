using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Magic.SpellTriggers;

public class CastingTriggerVicinity : CastingTriggerBase
{
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("vicinity", DoBuilderLoad,
			"Targets all items and characters in the vicinity of a target",
			"character",
			new CastingTriggerVicinity().BuildingCommandHelp
		);
		SpellTriggerFactory.RegisterLoadTimeFactory("vicinity",
			(root, spell) => new CastingTriggerVicinity(root, spell));
	}

	private static (IMagicTrigger Trigger, string Error) DoBuilderLoad(StringStack command, IMagicSpell spell)
	{
		return (
			new CastingTriggerVicinity(
				new XElement("Trigger", new XElement("MinimumPower", (int)SpellPower.Insignificant),
					new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful),
					new XElement("CanTargetSelf", true), 
					new XElement("TargetFilterProg", 0L),
					new XElement("Proximity", (int)Proximity.Proximate)
					), spell), string.Empty);
	}

	public IFutureProg TargetFilterProg { get; private set; }
	public bool CanTargetSelf { get; private set; }
	public Proximity Proximity { get; private set; }

	protected CastingTriggerVicinity(XElement root, IMagicSpell spell) : base(root, spell)
	{
		TargetFilterProg = spell.Gameworld.FutureProgs.Get(long.Parse(root.Element("TargetFilterProg").Value));
		CanTargetSelf = bool.Parse(root.Element("CanTargetSelf").Value);
		Proximity = (Proximity)int.Parse(root.Element("Proximity").Value);
	}

	protected CastingTriggerVicinity() : base(){}

	#region Overrides of CastingTriggerBase

	public override XElement SaveToXml()
	{
		return new XElement("Trigger",
			new XAttribute("type", "proximity"),
			new XElement("MinimumPower", (int)MinimumPower),
			new XElement("MaximumPower", (int)MaximumPower),
			new XElement("TargetFilterProg", TargetFilterProg?.Id ?? 0L),
			new XElement("CanTargetSelf", CanTargetSelf),
			new XElement("Proximity", (int)Proximity)
		);
	}

	public override IMagicTrigger Clone()
	{
		return new CastingTriggerVicinity(SaveToXml(), Spell);
	}

	public override string SubtypeBuildingCommandHelp =>
		@"
	#3filterprog <prog>#0 - sets the optional prog to filter targets by
	#3filterprog clear#0 - clears the filter prog
	#3self#0 - toggles whether the self is a valid target
	#3proximity <value>#0 - sets the maximum proximity to the base target";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "filter":
			case "filterprog":
			case "prog":
				return BuildingCommandFilterProg(actor, command);
			case "self":
				return BuildingCommandSelf(actor, command);
			case "proximity":
				return BuildingCommandProximity(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandProximity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"How close to the base target will this spell affect? The valid options are {Enum.GetValues<Proximity>().Select(x => x.Describe().ColourValue()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Proximity>(out var value))
		{
			actor.OutputHandler.Send($"That is not a valid proximity. The valid options are {Enum.GetValues<Proximity>().Select(x => x.Describe().ColourValue()).ListToString()}.");
			return false;
		}

		Proximity = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This spell will now affect all targets as a proximity of {Proximity.Describe().ColourValue()} to the target or closer.");
		return true;
	}

	private bool BuildingCommandSelf(ICharacter actor, StringStack command)
	{
		CanTargetSelf = !CanTargetSelf;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"You can {(CanTargetSelf ? "now" : "no longer")} target yourself when casting this spell.");
		return true;
	}

	private bool BuildingCommandFilterProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify a prog to use as a filter prog, or use {"none".ColourCommand()} to clear and existing one.");
			return false;
		}

		var text = command.SafeRemainingArgument;
		if (text.EqualToAny("none", "clear", "delete"))
		{
			TargetFilterProg = null;
			Spell.Changed = true;
			actor.OutputHandler.Send(
				$"This spell will no longer use a prog to filter which things can be targeted by it.");
			return true;
		}

		var prog = actor.Gameworld.FutureProgs.GetByIdOrName(text);
		if (prog == null)
		{
			actor.OutputHandler.Send($"There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes>
				{ ProgVariableTypes.Perceivable, ProgVariableTypes.Character }) &&
			!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Perceivable }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts either a single perceivable (the target), or a perceivable and a character (the target and the caster), whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		TargetFilterProg = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now use the {prog.MXPClickableFunctionNameWithId()} prog to filter valid targets.");
		return true;
	}

	public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
	{
		if (!CheckBaseTriggerCase(actor, additionalArguments, out var power))
		{
			return;
		}

		if (additionalArguments.IsFinished)
		{
			actor.OutputHandler.Send("That spell requires a target. You must specify a thing to target.");
			return;
		}

		var target = actor.TargetLocal(additionalArguments.SafeRemainingArgument);
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anything like that to target.");
			return;
		}

		if (target == actor && !CanTargetSelf)
		{
			actor.OutputHandler.Send("That spell cannot be used to target yourself.");
			return;
		}

		var targets = new List<IPerceivable>();
		foreach (var (other, proximity) in target.LocalThingsAndProximities() ?? new[] { (target, Proximity.Intimate) })
		{
			if (proximity > Proximity)
			{
				continue;
			}

			if (TargetFilterProg?.Execute<bool?>(other, actor) == false)
			{
				continue;
			}

			targets.Add(other);
		}
		
		Spell.CastSpell(actor, new PerceivableGroup(targets), power);
	}

	public override bool TriggerYieldsTarget => true;

	public override string TargetTypes => "perceivables";

	public override string Show(ICharacter actor)
	{
		return
			$"{"Cast@Proximity".ColourName()} {Proximity.Describe().ColourValue()} {(CanTargetSelf ? "" : " [noself]".ColourName())} - {base.Show(actor)}{(TargetFilterProg != null ? $" Filter: {TargetFilterProg.MXPClickableFunctionName()}" : "")}";
	}

	public override string ShowPlayer(ICharacter actor)
	{
		return
			$"Cast Command - {Spell.School.SchoolVerb} cast {(Spell.Name.Contains(' ') ? Spell.Name.ToLowerInvariant().DoubleQuotes() : Spell.Name.ToLowerInvariant())} <power> <target>";
	}

	#endregion
}