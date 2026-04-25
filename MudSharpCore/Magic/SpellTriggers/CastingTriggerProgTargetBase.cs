using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.Magic.SpellTriggers;

public abstract class CastingTriggerProgTargetBase : CastingTriggerBase
{
	protected CastingTriggerProgTargetBase(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
		TargetProg = spell.Gameworld.FutureProgs.Get(long.Parse(root.Element("TargetProg")!.Value));
	}

	protected CastingTriggerProgTargetBase()
		: base()
	{
	}

	public IFutureProg? TargetProg { get; protected set; }

	protected abstract string TriggerKeyword { get; }
	protected abstract string TriggerDisplayName { get; }
	protected abstract ProgVariableTypes TargetProgReturnType { get; }
	protected abstract string TargetTypeName { get; }

	protected virtual IEnumerable<IEnumerable<ProgVariableTypes>> TargetProgParameterSets =>
		[
			[ProgVariableTypes.Character],
			[ProgVariableTypes.Character, ProgVariableTypes.MagicSpell],
			[ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.Text | ProgVariableTypes.Collection],
			[ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.Text | ProgVariableTypes.Collection, ProgVariableTypes.Number]
		];

	protected virtual XElement SaveExtraElements()
	{
		return new XElement("Extra");
	}

	protected virtual bool HandleExtraBuildingCommand(ICharacter actor, StringStack command)
	{
		return base.BuildingCommand(actor, command);
	}

	protected virtual SpellAdditionalParameter[] GetAdditionalParameters(ICharacter actor, StringStack additionalArguments,
		SpellPower power)
	{
		return [];
	}

	protected virtual string BuildingCommandText =>
		@"
	#3prog <prog>#0 - sets the prog used to resolve the target";

	public override string SubtypeBuildingCommandHelp => BuildingCommandText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "prog":
				return BuildingCommandProg(actor, command);
			default:
				return HandleExtraBuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which prog should determine the {TargetTypeName} target?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument,
			TargetProgReturnType, TargetProgParameterSets).LookupProg();
		if (prog is null)
		{
			return false;
		}

		TargetProg = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This trigger will now use the {prog.MXPClickableFunctionName()} prog to determine the {TargetTypeName} target.");
		return true;
	}

	public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
	{
		if (!CheckBaseTriggerCase(actor, additionalArguments, out SpellPower power))
		{
			return;
		}

		IPerceivable? target = TargetProg?.Execute<IPerceivable?>(actor, Spell, additionalArguments, (int)power);
		Spell.CastSpell(actor, target, power, GetAdditionalParameters(actor, additionalArguments, power));
	}

	public override bool TriggerYieldsTarget => true;
	public override bool TriggerMayFailToYieldTarget => true;

	public override string Show(ICharacter actor)
	{
		return $"{TriggerDisplayName.ColourName()}[{TargetProg?.MXPClickableFunctionName() ?? "Unknown".ColourError()}] - {base.Show(actor)}";
	}

	public override string ShowPlayer(ICharacter actor)
	{
		return
			$"Cast Command - {Spell.School.SchoolVerb} cast {(Spell.Name.Contains(' ') ? Spell.Name.ToLowerInvariant().DoubleQuotes() : Spell.Name.ToLowerInvariant())} <power> [prog arguments]";
	}

	protected XElement SaveBaseXml()
	{
		XElement root = new("Trigger",
			new XAttribute("type", TriggerKeyword),
			new XElement("MinimumPower", (int)MinimumPower),
			new XElement("MaximumPower", (int)MaximumPower),
			new XElement("TargetProg", TargetProg?.Id ?? 0L)
		);

		foreach (XElement element in SaveExtraElements().Elements())
		{
			root.Add(new XElement(element));
		}

		return root;
	}
}
