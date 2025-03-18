using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Statements.Manipulation;

namespace MudSharp.NPC.AI;

public class CombatEndAI : ArtificialIntelligenceBase
{
	public IFutureProg WillAcceptTruce { get; set; }
	public IFutureProg WillAcceptTargetIncapacitated { get; set; }

	public IFutureProg OnOfferedTruce { get; set; }

	public IFutureProg OnTargetIncapacitated { get; set; }
	public IFutureProg OnNoNaturalTargets { get; set; }

	private CombatEndAI()
	{

	}

	private CombatEndAI(IFuturemud gameworld, string name) : base(gameworld, name, "CombatEnd")
	{
		WillAcceptTruce = Gameworld.AlwaysTrueProg;
		WillAcceptTargetIncapacitated = Gameworld.AlwaysTrueProg;
		DatabaseInitialise();
	}

	protected CombatEndAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
		var definition = XElement.Parse(ai.Definition);
		WillAcceptTruce = Gameworld.FutureProgs.Get(long.Parse(definition.Element("WillAcceptTruce").Value));
		WillAcceptTargetIncapacitated =
			Gameworld.FutureProgs.Get(long.Parse(definition.Element("WillAcceptTargetIncapacitated").Value));
		OnOfferedTruce = Gameworld.FutureProgs.Get(long.Parse(definition.Element("OnOfferedTruce").Value));
		OnTargetIncapacitated =
			Gameworld.FutureProgs.Get(long.Parse(definition.Element("OnTargetIncapacitated").Value));
		OnNoNaturalTargets = Gameworld.FutureProgs.Get(long.Parse(definition.Element("OnNoNaturalTargets").Value));
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("WillAcceptTruce", WillAcceptTruce?.Id ?? 0L),
			new XElement("WillAcceptTargetIncapacitated", WillAcceptTargetIncapacitated?.Id ?? 0L),
			new XElement("OnOfferedTruce", OnOfferedTruce?.Id ?? 0L),
			new XElement("OnTargetIncapacitated", OnTargetIncapacitated?.Id ?? 0L),
			new XElement("OnNoNaturalTargets", OnNoNaturalTargets?.Id ?? 0L)
		).ToString();
	}

	public static void RegisterLoader()
	{
		RegisterAIType("CombatEnd", (ai, gameworld) => new CombatEndAI(ai, gameworld));
		RegisterAIBuilderInformation("combatend", (gameworld,name) => new CombatEndAI(gameworld, name), new CombatEndAI().HelpText);
	}

	#region Overrides of ArtificialIntelligenceBase

	private void HandleNoNaturalTargets(ICharacter actor)
	{
		OnNoNaturalTargets?.Execute(actor);
		actor.Combat?.TruceRequested(actor);
	}

	private void HandleTargetIncapacitated(ICharacter actor, ICharacter target)
	{
		OnTargetIncapacitated?.Execute(actor, target);
		if (actor.Combat == null || target.Combat == null)
		{
			return;
		}

		if (WillAcceptTargetIncapacitated?.ExecuteBool(actor, target) ?? false)
		{
			actor.CombatTarget = actor.Combat.Combatants.Where(x => x.CombatTarget == actor)
			                          .Except(target)
			                          .GetRandomElement();
			if (actor.Combat?.CanFreelyLeaveCombat(actor) == true)
			{
				actor.Combat.LeaveCombat(actor);
			}

			if (target.Combat?.CanFreelyLeaveCombat(target) == true)
			{
				target.Combat.LeaveCombat(target);
			}
		}
	}

	private void HandleTruceOffered(ICharacter actor, ICharacter target)
	{
		OnOfferedTruce?.Execute(actor, target);
		if (WillAcceptTruce?.ExecuteBool(actor, target) ?? false)
		{
			actor.Combat.TruceRequested(actor);
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var ch = arguments[0] as ICharacter;
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		switch (type)
		{
			case EventType.NoNaturalTargets:
				HandleNoNaturalTargets(arguments[0]);
				return true;
			case EventType.TargetIncapacitated:
				HandleTargetIncapacitated(arguments[0], arguments[1]);
				return true;
			case EventType.TruceOffered:
				HandleTruceOffered(arguments[0], arguments[1]);
				return true;
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.NoNaturalTargets:
				case EventType.TargetIncapacitated:
				case EventType.TruceOffered:
					return true;
			}
		}

		return false;
	}

	#endregion

	#region Overrides of ArtificialIntelligenceBase
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {AIType.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Will Accept Truce: {WillAcceptTruce.MXPClickableFunctionName()}");
		sb.AppendLine($"Will End Incapacitated: {WillAcceptTargetIncapacitated.MXPClickableFunctionName()}");
		sb.AppendLine($"On Truce Offered: {OnOfferedTruce?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"On Target Incapacitated: {OnTargetIncapacitated?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"On No Targets: {OnNoNaturalTargets?.MXPClickableFunctionName() ?? "None".ColourError()}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText =>
		@"	#3accept <prog>#0 - the prog that controls whether they will accept a truce
	#3acceptincapacitated <prog>#0 - the prog that controls whether they end combat when their foe is incapacitated
	#3ontruce <prog>#0 - sets a prog to execute when a truce is offered
	#3ontruce clear#0 - clears the on truce prog
	#3onincapacitated <prog>#0 - sets a prog to execute when a target is incapacitated
	#3onincapacitated clear#0 - clears the on incapacitated prog
	#3notargets <prog>#0 - sets a prog to execute when there are no valid targets
	#3notargets clear#0 - clears the no targets prog";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "accept":
			case "acceptprog":
			case "willaccept":
			case "willacceptprog":
				return BuildingCommandWillAcceptProg(actor, command);
			case "acceptincapacitated":
			case "acceptuncon":
			case "acceptincapacitatedprog":
			case "acceptunconprog":
				return BuildingCommandWillAcceptInCapacitatedProg(actor, command);
			case "ontruce":
			case "ontruceprog":
			case "truceprog":
				return BuildingCommandOnTruceProg(actor, command);
			case "notargets":
			case "notargetsprog":
			case "notarget":
			case "notargetprog":
				return BuildingCommandNoTargetsProg(actor, command);
			case "ontargetincapacitated":
			case "onincapacitated":
			case "ontargetuncon":
			case "onuncon":
			case "ontargetincapacitatedprog":
			case "onincapacitatedprog":
			case "ontargetunconprog":
			case "onunconprog":
				return BuildingCommandOnTargetIncapacitatedProg(actor, command);

		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandOnTargetIncapacitatedProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog or use #3clear#0 to remove the existing prog.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			OnTargetIncapacitated = null;
			Changed = true;
			actor.OutputHandler.Send("This NPC will no longer execute any prog when its target is incapacitated.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Void, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		OnTargetIncapacitated = prog;
		Changed = true;
		actor.OutputHandler.Send($"The NPC will now execute the {prog.MXPClickableFunctionName()} prog when its target is incapacitated.");
		return true;
	}

	private bool BuildingCommandNoTargetsProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog or use #3clear#0 to remove the existing prog.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			OnNoNaturalTargets = null;
			Changed = true;
			actor.OutputHandler.Send("This NPC will no longer execute any prog when it has no more valid targets.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Void, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Character,
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		OnNoNaturalTargets = prog;
		Changed = true;
		actor.OutputHandler.Send($"The NPC will now execute the {prog.MXPClickableFunctionName()} prog when it has no more valid targets.");
		return true;
	}

	private bool BuildingCommandOnTruceProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog or use #3clear#0 to remove the existing prog.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			OnOfferedTruce = null;
			Changed = true;
			actor.OutputHandler.Send("This NPC will no longer execute any prog when its target offers a truce.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Void, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		OnOfferedTruce = prog;
		Changed = true;
		actor.OutputHandler.Send($"The NPC will now execute the {prog.MXPClickableFunctionName()} prog when offered a truce.");
		return true;
	}

	private bool BuildingCommandWillAcceptInCapacitatedProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WillAcceptTruce = prog;
		Changed = true;
		actor.OutputHandler.Send($"The NPC will now use the {prog.MXPClickableFunctionName()} prog to control whether it will end the combat if its target is incapacitated.");
		return true;
	}

	private bool BuildingCommandWillAcceptProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WillAcceptTruce = prog;
		Changed = true;
		actor.OutputHandler.Send($"The NPC will now use the {prog.MXPClickableFunctionName()} prog to control whether it will accept a truce.");
		return true;
	}

	#endregion
}