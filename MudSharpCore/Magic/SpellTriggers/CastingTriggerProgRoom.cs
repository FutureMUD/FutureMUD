using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp.Magic.SpellTriggers;

public class CastingTriggerProgRoom : CastingTriggerBase
{
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("progroom", DoBuilderLoad,
			"Targets a room supplied by a prog",
			"room",
			new CastingTriggerProgRoom().BuildingCommandHelp
		);
		SpellTriggerFactory.RegisterLoadTimeFactory("progroom", (root, spell) => new CastingTriggerProgRoom(root, spell));
	}

	private static (IMagicTrigger Trigger, string Error) DoBuilderLoad(StringStack command, IMagicSpell spell)
	{
		return (
			new CastingTriggerProgRoom(
				new XElement("Trigger", 
					new XElement("MinimumPower", (int)SpellPower.Insignificant),
					new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful),
					new XElement("TargetRoomProg", 0L)
					), spell), string.Empty);
	}

	protected CastingTriggerProgRoom(XElement root, IMagicSpell spell) : base(root, spell)
	{
		TargetRoomProg = spell.Gameworld.FutureProgs.Get(long.Parse(root.Element("TargetRoomProg").Value));
	}

	protected CastingTriggerProgRoom() : base() { }

	#region Implementation of IXmlSavable

	public override XElement SaveToXml()
	{
		return new XElement("Trigger",
			new XAttribute("type", "progroom"),
			new XElement("MinimumPower", (int)MinimumPower),
			new XElement("MaximumPower", (int)MaximumPower),
			new XElement("TargetRoomProg", TargetRoomProg?.Id ?? 0L)
		);
	}

	#endregion

	#region Implementation of IMagicTrigger

	public override IMagicTrigger Clone()
	{
		return new CastingTriggerProgRoom(SaveToXml(), Spell);
	}

	public override string Show(ICharacter actor)
	{
		return $"{$"Cast@Room[{TargetRoomProg?.MXPClickableFunctionName() ?? "Unknown".ColourError()}]".ColourName()} - {base.Show(actor)}";
	}

	#endregion

	#region Overrides of CastingTriggerBase

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "prog":
				return BuildingCommandProg(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
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
		
		Spell.CastSpell(actor, TargetRoomProg?.Execute<ICell>(actor, Spell, additionalArguments, (int)power), power);
	}

	public override bool TriggerYieldsTarget => true;

	public override bool TriggerMayFailToYieldTarget => true;

	public override string TargetTypes => "room";

	public override string ShowPlayer(ICharacter actor)
	{
		return
			$"Cast Command - {Spell.School.SchoolVerb} cast {(Spell.Name.Contains(' ') ? Spell.Name.ToLowerInvariant().DoubleQuotes() : Spell.Name.ToLowerInvariant())} <power>";
	}

	#endregion

	public IFutureProg TargetRoomProg { get; private set; }
}