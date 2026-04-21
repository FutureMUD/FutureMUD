using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellTriggers;

public abstract class CastingTriggerProgTargetWithRoomBase : CastingTriggerProgTargetBase
{
	protected CastingTriggerProgTargetWithRoomBase(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
		TargetRoomProg = spell.Gameworld.FutureProgs.Get(long.Parse(root.Element("TargetRoomProg").Value));
	}

	protected CastingTriggerProgTargetWithRoomBase()
	{
	}

	public IFutureProg? TargetRoomProg { get; protected set; }

	protected override IEnumerable<IEnumerable<ProgVariableTypes>> TargetProgParameterSets =>
		[
			[ProgVariableTypes.Character],
			[ProgVariableTypes.Character, ProgVariableTypes.MagicSpell],
			[ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.Text | ProgVariableTypes.Collection],
			[ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.Text | ProgVariableTypes.Collection, ProgVariableTypes.Number]
		];

	protected override string BuildingCommandText =>
		@"
	#3prog <prog>#0 - sets the prog used to resolve the target
	#3roomprog <prog>#0 - sets the prog used to resolve the room";

	protected override bool HandleExtraBuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "room":
			case "roomprog":
				return BuildingCommandRoomProg(actor, command);
			default:
				return base.HandleExtraBuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandRoomProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should determine the room target?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument,
			ProgVariableTypes.Location,
			[
				[ProgVariableTypes.Character],
				[ProgVariableTypes.Character, ProgVariableTypes.MagicSpell],
				[ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.Text | ProgVariableTypes.Collection],
				[ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.Text | ProgVariableTypes.Collection, ProgVariableTypes.Number]
			]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		TargetRoomProg = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This trigger will now use the {prog.MXPClickableFunctionName()} prog to determine the room target.");
		return true;
	}

	protected override SpellAdditionalParameter[] GetAdditionalParameters(ICharacter actor, StringStack additionalArguments,
		SpellPower power)
	{
		ICell? room = TargetRoomProg?.Execute<ICell?>(actor, Spell, additionalArguments, (int)power);
		return [new SpellAdditionalParameter { ParameterName = "room", Item = room }];
	}

	protected override XElement SaveExtraElements()
	{
		return new XElement("Extra", new XElement("TargetRoomProg", TargetRoomProg?.Id ?? 0L));
	}
}
