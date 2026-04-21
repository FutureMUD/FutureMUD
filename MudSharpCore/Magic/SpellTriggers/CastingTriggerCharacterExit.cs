using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellTriggers;

public class CastingTriggerCharacterExit : CastingTriggerBase
{
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("characterexit", DoBuilderLoad,
			"Targets a same-room character and a local exit",
			"character&exit",
			new CastingTriggerCharacterExit().BuildingCommandHelp
		);
		SpellTriggerFactory.RegisterLoadTimeFactory("characterexit", (root, spell) => new CastingTriggerCharacterExit(root, spell));
	}

	private static (IMagicTrigger Trigger, string Error) DoBuilderLoad(StringStack command, IMagicSpell spell)
	{
		return (
			new CastingTriggerCharacterExit(
				new XElement("Trigger",
					new XElement("MinimumPower", (int)SpellPower.Insignificant),
					new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful),
					new XElement("CanTargetSelf", false),
					new XElement("TargetFilterProg", 0L)
				), spell), string.Empty);
	}

	protected CastingTriggerCharacterExit(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
		TargetFilterProg = spell.Gameworld.FutureProgs.Get(long.Parse(root.Element("TargetFilterProg").Value));
		CanTargetSelf = bool.Parse(root.Element("CanTargetSelf").Value);
	}

	protected CastingTriggerCharacterExit()
		: base()
	{
	}

	public IFutureProg? TargetFilterProg { get; private set; }
	public bool CanTargetSelf { get; private set; }

	public override XElement SaveToXml()
	{
		return new XElement("Trigger",
			new XAttribute("type", "characterexit"),
			new XElement("MinimumPower", (int)MinimumPower),
			new XElement("MaximumPower", (int)MaximumPower),
			new XElement("TargetFilterProg", TargetFilterProg?.Id ?? 0L),
			new XElement("CanTargetSelf", CanTargetSelf)
		);
	}

	public override IMagicTrigger Clone()
	{
		return new CastingTriggerCharacterExit(SaveToXml(), Spell);
	}

	public override string Show(ICharacter actor)
	{
		return
			$"{"Cast@Character&Exit".ColourName()}{(CanTargetSelf ? "" : " [noself]".ColourName())} - {base.Show(actor)}{(TargetFilterProg is not null ? $" Filter: {TargetFilterProg.MXPClickableFunctionName()}" : "")}";
	}

	public override string SubtypeBuildingCommandHelp =>
		@"
	#3filterprog <prog>#0 - sets the optional prog to filter character targets by
	#3filterprog clear#0 - clears the filter prog
	#3self#0 - toggles whether the caster can be targeted";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "filter":
			case "filterprog":
			case "prog":
				return BuildingCommandFilterProg(actor, command);
			case "self":
				return BuildingCommandSelf(actor);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandSelf(ICharacter actor)
	{
		CanTargetSelf = !CanTargetSelf;
		Spell.Changed = true;
		actor.OutputHandler.Send($"You can {(CanTargetSelf ? "now" : "no longer")} target yourself with this trigger.");
		return true;
	}

	private bool BuildingCommandFilterProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must specify a prog to use as a filter prog, or use {"none".ColourCommand()} to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "delete"))
		{
			TargetFilterProg = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This trigger no longer uses a target filter prog.");
			return true;
		}

		IFutureProg? prog = actor.Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog is null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Character }) &&
		    !prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts either a single character, or the target and caster characters, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		TargetFilterProg = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This trigger will now use the {prog.MXPClickableFunctionNameWithId()} prog to filter character targets.");
		return true;
	}

	public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
	{
		if (!CheckBaseTriggerCase(actor, additionalArguments, out SpellPower power))
		{
			return;
		}

		if (additionalArguments.IsFinished)
		{
			actor.OutputHandler.Send("That spell requires a target character.");
			return;
		}

		string targetText = additionalArguments.PopSpeech();
		ICharacter? target = actor.TargetActorOrCorpse(targetText);
		if (target is null)
		{
			actor.OutputHandler.Send("You do not see anyone like that to target.");
			return;
		}

		if (target == actor && !CanTargetSelf)
		{
			actor.OutputHandler.Send("That spell cannot be used to target yourself.");
			return;
		}

		if (TargetFilterProg?.Execute<bool?>(target, actor) == false)
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} is not a valid target for that spell.");
			return;
		}

		if (additionalArguments.IsFinished)
		{
			actor.OutputHandler.Send("That spell also requires a local exit or direction.");
			return;
		}

		ICellExit? exit = CastingTriggerExitHelper.ResolveExit(actor, additionalArguments.PopSpeech());
		if (exit is null)
		{
			actor.OutputHandler.Send("You do not see any exit like that here.");
			return;
		}

		Spell.CastSpell(actor, target, power, new SpellAdditionalParameter { ParameterName = "exit", Item = exit });
	}

	public override bool TriggerYieldsTarget => true;
	public override bool TriggerMayFailToYieldTarget => false;
	public override string TargetTypes => "character&exit";

	public override string ShowPlayer(ICharacter actor)
	{
		return
			$"Cast Command - {Spell.School.SchoolVerb} cast {(Spell.Name.Contains(' ') ? Spell.Name.ToLowerInvariant().DoubleQuotes() : Spell.Name.ToLowerInvariant())} <power> <target> <exit>";
	}
}
