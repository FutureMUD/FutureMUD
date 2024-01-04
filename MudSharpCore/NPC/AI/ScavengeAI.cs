using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Text;

namespace MudSharp.NPC.AI;

public class ScavengeAI : ArtificialIntelligenceBase
{
	public ScavengeAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	private ScavengeAI()
	{
	}

	private ScavengeAI(IFuturemud gameworld, string name) : base(gameworld, name, "Scavenge")
	{
		WillScavengeItemProg = Gameworld.AlwaysFalseProg;
		ScavengeDelayDiceExpression = "30+1d30";
		DatabaseInitialise();
	}

	/// <summary>
	///     Prog returns true, parameters character, item
	/// </summary>
	public IFutureProg WillScavengeItemProg { get; set; }

	/// <summary>
	///     Prog returns void, parameters character, item
	/// </summary>
	public IFutureProg OnScavengeItemProg { get; set; }

	public string ScavengeDelayDiceExpression { get; set; }

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("WillScavengeItemProg", WillScavengeItemProg?.Id ?? 0L),
			new XElement("ScavengeDelayDiceExpression", new XCData(ScavengeDelayDiceExpression)),
			new XElement("OnScavengeItemProg", OnScavengeItemProg?.Id ?? 0L)
		).ToString();
	}

	public static void RegisterLoader()
	{
		RegisterAIType("Scavenge", (ai, gameworld) => new ScavengeAI(ai, gameworld));
		RegisterAIBuilderInformation("scavenge", (gameworld, name) => new ScavengeAI(gameworld, name), new ScavengeAI().HelpText);
	}

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {AIType.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Will Scavenge Prog: {WillScavengeItemProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"On Scavenge Prog: {WillScavengeItemProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Delay Seconds Prog: {ScavengeDelayDiceExpression.ColourCommand()}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => @"	#3will <prog>#0 - sets a prog to evaluate items for scavenging purposes
	#3scavenge <prog>#0 - sets the prog that is executed when an item is chosen for scavenging
	#3delay <expression#0 - sets a dice expression for seconds between scavenge attempts";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "will":
			case "willprog":
				return BuildingCommandWillProg(actor, command);
			case "onscavenge":
			case "scavenge":
			case "onscavengeprog":
			case "scavengeprog":
				return BuildingCommandOnScavengeProg(actor, command);
			case "delay":
				return BuildingCommandDelay(actor, command);

		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid dice expression for seconds between scavenge attempts.");
			return false;
		}

		if (!Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid dice expression.");
			return false;
		}

		ScavengeDelayDiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now check for scavenging items every {ScavengeDelayDiceExpression.ColourValue()} seconds.");
		return true;
	}

	private bool BuildingCommandOnScavengeProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Void, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Item
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		OnScavengeItemProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This NPC will now execute the {prog.MXPClickableFunctionName()} prog when it picks a target for scavenging.");
		return true;
	}

	private bool BuildingCommandWillProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Item
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WillScavengeItemProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This NPC will now use the {prog.MXPClickableFunctionName()} prog to determine whether it will scavenge an item.");
		return true;
	}

	private void LoadFromXml(XElement root)
	{
		WillScavengeItemProg =
			long.TryParse(root.Element("WillScavengeItemProg").Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(root.Element("WillScavengeItemProg").Value);

		if (WillScavengeItemProg == null)
		{
			throw new ApplicationException(
				$"AI {Id} pointed to a WillScavengeItem Prog that was not correct - {root.Element("WillScavengeItemProg").Value}.");
		}

		if (WillScavengeItemProg.ReturnType != FutureProgVariableTypes.Boolean)
		{
			throw new ApplicationException(
				$"AI {Id} WillScavengeItem prog returns {WillScavengeItemProg.ReturnType.Describe()} - expected Boolean");
		}

		if (
			!WillScavengeItemProg.MatchesParameters(new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Item
			}))
		{
			throw new ApplicationException(
				$"AI {Id} WillScavengeItem prog does not accept the right parameters - should be character, item");
		}

		OnScavengeItemProg =
			long.TryParse(root.Element("OnScavengeItemProg").Value, out value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(root.Element("OnScavengeItemProg").Value);

		if (WillScavengeItemProg == null)
		{
			throw new ApplicationException(
				$"AI {Id} pointed to a OnScavengeItemProg Prog that was not correct - {root.Element("OnScavengeItemProg").Value}.");
		}

		if (
			!WillScavengeItemProg.MatchesParameters(new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Item
			}))
		{
			throw new ApplicationException(
				$"AI {Id} OnScavengeItemProg prog does not accept the right parameters - should be character, item");
		}

		ScavengeDelayDiceExpression = root.Element("ScavengeDelayDiceExpression").Value;
		if (!Dice.IsDiceExpression(ScavengeDelayDiceExpression))
		{
			throw new ApplicationException($"AI {Id} had an illegal dice expression.");
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.CharacterEntersGame:
			case EventType.CharacterEnterCellFinish:
			case EventType.CharacterStopMovement:
			case EventType.CharacterStopMovementClosedDoor:
			case EventType.MinuteTick:
				var character = arguments[0];
				CreateScavengeEvent(character);
				return false;
			case EventType.CharacterBeginMovement:
				character = arguments[0];
				PurgeScavengeEvent(character);
				return false;
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.CharacterEntersGame:
				case EventType.CharacterEnterCellFinish:
				case EventType.CharacterStopMovement:
				case EventType.CharacterStopMovementClosedDoor:
				case EventType.CharacterBeginMovement:
				case EventType.MinuteTick:
					return true;
			}
		}

		return false;
	}

	private void CreateScavengeEvent(ICharacter character)
	{
		if (character.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		if (
			character.Effects.Any(x => x.IsEffectType<WaitingForScavenge>()))
		{
			return;
		}

		character.AddEffect(
			new WaitingForScavenge(character, x => { CheckScavengeEvent(character); }),
			TimeSpan.FromSeconds(Dice.Roll(ScavengeDelayDiceExpression)));
	}

	private void PurgeScavengeEvent(ICharacter character)
	{
		character.RemoveAllEffects(x => x.IsEffectType<WaitingForScavenge>());
	}

	private void CheckScavengeEvent(ICharacter character)
	{
		if (OnScavengeItemProg is null)
		{
			return;
		}

		if (character.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		var targetItem =
			character.Location.LayerGameItems(character.RoomLayer).FirstOrDefault(
				x => character.CanSee(x) && ((bool?)WillScavengeItemProg.Execute(character, x) ?? false));
		if (targetItem != null)
		{
			OnScavengeItemProg.Execute(character, targetItem);
		}

		CreateScavengeEvent(character);
	}
}