using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Magic.SpellTriggers;

public class CastingTriggerCharacterProgRoom : CastingTriggerBase
{
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("characterprogroom", DoBuilderLoad,
			"Targets a character in the same room and a room via prog",
			"character&room",
			new CastingTriggerCharacterProgRoom().BuildingCommandHelp
		);
		SpellTriggerFactory.RegisterLoadTimeFactory("characterprogroom", (root, spell) => new CastingTriggerCharacterProgRoom(root, spell));
	}

	private static (IMagicTrigger Trigger, string Error) DoBuilderLoad(StringStack command, IMagicSpell spell)
	{
		return (
			new CastingTriggerCharacterProgRoom(
				new XElement("Trigger",
					new XElement("MinimumPower", (int)SpellPower.Insignificant),
					new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful),
					new XElement("TargetRoomProg", 0L),
					new XElement("CanTargetSelf", false), 
					new XElement("TargetFilterProg", 0L)
				), spell), string.Empty);
	}

	protected CastingTriggerCharacterProgRoom(XElement root, IMagicSpell spell) : base(root, spell)
	{
		TargetFilterProg = spell.Gameworld.FutureProgs.Get(long.Parse(root.Element("TargetFilterProg").Value));
		CanTargetSelf = bool.Parse(root.Element("CanTargetSelf").Value);
	}

	protected CastingTriggerCharacterProgRoom() : base() { }

	#region Implementation of IXmlSavable

	public override XElement SaveToXml()
	{
		return new XElement("Trigger",
			new XAttribute("type", "characterprogroom"),
			new XElement("MinimumPower", (int)MinimumPower),
			new XElement("MaximumPower", (int)MaximumPower),
			new XElement("TargetRoomProg", TargetRoomProg?.Id ?? 0L),
			new XElement("TargetFilterProg", TargetFilterProg?.Id ?? 0L),
			new XElement("CanTargetSelf", CanTargetSelf)
		);
	}

	#endregion

	#region Implementation of IMagicTrigger

	public override IMagicTrigger Clone()
	{
		return new CastingTriggerCharacterProgRoom(SaveToXml(), Spell);
	}

	public override string Show(ICharacter actor)
	{
		return $"{$"Cast@Character&Room[{TargetRoomProg?.MXPClickableFunctionName() ?? "Unknown".ColourError()}]".ColourName()} - {base.Show(actor)}";
	}

	#endregion

	#region Overrides of CastingTriggerBase

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "room":
			case "roomprog":
				return BuildingCommandRoomProg(actor, command);
			case "filter":
			case "filterprog":
				return BuildingCommandTargetFilterProg(actor, command);
			case "self":
				return BuildingCommandSelf(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandSelf(ICharacter actor, StringStack command)
	{
		CanTargetSelf = !CanTargetSelf;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"You can {(CanTargetSelf ? "now" : "no longer")} target yourself when casting this spell.");
		return true;
	}

	private bool BuildingCommandTargetFilterProg(ICharacter actor, StringStack command)
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
				$"This spell will no longer use a prog to filter which characters can be targeted by it.");
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
				{ ProgVariableTypes.Character, ProgVariableTypes.Character }) &&
			!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts either a single character (the target), or two characters (the target and the caster), whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		TargetFilterProg = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now use the {prog.MXPClickableFunctionNameWithId()} prog to filter valid character targets.");
		return true;
	}

	private bool BuildingCommandRoomProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use to determine the target room?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(
			actor,
			command.SafeRemainingArgument,
			ProgVariableTypes.Location,
			[
				[ProgVariableTypes.Character],
				[ProgVariableTypes.Character, ProgVariableTypes.MagicSpell],
				[ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.Text | ProgVariableTypes.Collection],
				[ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.Text | ProgVariableTypes.Collection, ProgVariableTypes.Number],
			]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		TargetRoomProg = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This trigger will now use the {prog.MXPClickableFunctionName()} prog to determine which room to target.");
		return true;
	}

	public override string SubtypeBuildingCommandHelp => string.Empty;

	public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
	{
		if (!CheckBaseTriggerCase(actor, additionalArguments, out var power))
		{
			return;
		}

		if (additionalArguments.IsFinished)
		{
			actor.OutputHandler.Send("That spell requires a target character. You must specify a character to target.");
			return;
		}

		var target = actor.TargetActorOrCorpse(additionalArguments.SafeRemainingArgument);
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that to target.");
			return;
		}

		if (target == actor && !CanTargetSelf)
		{
			actor.OutputHandler.Send("That spell cannot be used to target yourself.");
			return;
		}

		if (TargetFilterProg?.Execute<bool?>(target, actor) == false)
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} is not a valid target for that spell.");
			return;
		}

		var cell = TargetRoomProg?.Execute<ICell>(actor, Spell, additionalArguments, (int)power);
		Spell.CastSpell(actor, target, power, new SpellAdditionalParameter{ParameterName="room", Item = cell});
	}

	public override bool TriggerYieldsTarget => true;

	public override bool TriggerMayFailToYieldTarget => true;

	public override string TargetTypes => "character&room";

	public override string ShowPlayer(ICharacter actor)
	{
		return $"Cast Command - {Spell.School.SchoolVerb} cast {(Spell.Name.Contains(' ') ? Spell.Name.ToLowerInvariant().DoubleQuotes() : Spell.Name.ToLowerInvariant())} <power> <target character>";
	}

	#endregion

	public IFutureProg TargetRoomProg { get; private set; }
	public IFutureProg TargetFilterProg { get; private set; }
	public bool CanTargetSelf { get; private set; }
}