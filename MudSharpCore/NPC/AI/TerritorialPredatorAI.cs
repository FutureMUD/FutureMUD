using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public class TerritorialPredatorAI : TerritorialWanderer
{
	public IFutureProg WillAttackProg { get; protected set; }
	public string EngageDelayDiceExpression { get; protected set; }
	public string EngageEmote { get; protected set; }
	public override bool CountsAsAggressive => true;

	protected TerritorialPredatorAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	protected TerritorialPredatorAI(IFuturemud gameworld, string name, string type = "TerritorialPredator") :
		base(gameworld, name, type)
	{
		WillAttackProg = Gameworld.AlwaysFalseProg;
		EngageDelayDiceExpression = "1000+1d1000";
		EngageEmote = string.Empty;
		if (type == "TerritorialPredator")
		{
			DatabaseInitialise();
		}
	}

	protected TerritorialPredatorAI()
	{
	}

	public static void RegisterLoader()
	{
		RegisterAIType("TerritorialPredator", (ai, gameworld) => new TerritorialPredatorAI(ai, gameworld));
		RegisterAIBuilderInformation("territorialpredator",
			(gameworld, name) => new TerritorialPredatorAI(gameworld, name),
			new TerritorialPredatorAI().HelpText);
	}

	protected override void LoadFromXML(XElement root)
	{
		base.LoadFromXML(root);
		WillAttackProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WillAttackProg")?.Value ?? "0")) ??
		                 Gameworld.AlwaysFalseProg;
		EngageDelayDiceExpression = root.Element("EngageDelayDiceExpression")?.Value ?? "1000+1d1000";
		EngageEmote = root.Element("EngageEmote")?.Value ?? string.Empty;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("SuitableTerritoryProg", SuitableTerritoryProg?.Id ?? 0),
			new XElement("DesiredTerritorySizeProg", DesiredTerritorySizeProg?.Id ?? 0),
			new XElement("WanderChancePerMinute", WanderChancePerMinute),
			new XElement("WanderEmote", new XCData(WanderEmote)),
			new XElement("WillAttackProg", WillAttackProg?.Id ?? 0),
			new XElement("EngageDelayDiceExpression", new XCData(EngageDelayDiceExpression)),
			new XElement("EngageEmote", new XCData(EngageEmote)),
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay),
			new XElement("WillShareTerritory", WillShareTerritory),
			new XElement("WillShareTerritoryWithOtherRaces", WillShareTerritoryWithOtherRaces)
		).ToString();
	}

	public override string Show(ICharacter actor)
	{
		StringBuilder sb = new(base.Show(actor));
		sb.AppendLine($"Will Attack Prog: {WillAttackProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Engage Delay: {EngageDelayDiceExpression.ColourValue()} milliseconds");
		sb.AppendLine($"Engage Emote: {EngageEmote?.ColourCommand() ?? ""}");
		sb.AppendLine("Predation: Only attacks while hungry, and only targets edible corpses.");
		return sb.ToString();
	}

	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3attackprog <prog>#0 - sets the prog that controls target selection
	#3emote <emote>#0 - sets the engage emote ($0 = npc, $1 = target)
	#3emote clear#0 - clears the emote (won't do an emote when engaging)
	#3delay <dice expression>#0 - sets the delay (in ms) before attacking when spotting a target";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "attackprog":
				return BuildingCommandAttackProg(actor, command);
			case "emote":
			case "engageemote":
			case "attackemote":
				return BuildingCommandEngageEmote(actor, command);
			case "delay":
			case "engagedelay":
			case "attackdelay":
				return BuildingCommandEngageDelay(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandEngageDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must supply a dice expression for a number of milliseconds.");
			return false;
		}

		if (!Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid dice expression.");
			return false;
		}

		EngageDelayDiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"The NPC will now wait {EngageDelayDiceExpression.ColourValue()} milliseconds before attacking.");
		return true;
	}

	private bool BuildingCommandEngageEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either supply an emote or use {"clear".ColourCommand()} to remove the emote.");
			return false;
		}

		string text = command.SafeRemainingArgument;
		if (text.EqualToAny("remove", "clear", "delete"))
		{
			EngageEmote = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This NPC will no longer do any emote when engaging targets.");
			return true;
		}

		Emote emote = new(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EngageEmote = text;
		Changed = true;
		actor.OutputHandler.Send($"The NPC will now do the following emote when engaging:\n{text.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandAttackProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to control whether this NPC will attack a target?");
			return false;
		}

		IFutureProg prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WillAttackProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This NPC will now use the {prog.MXPClickableFunctionName()} prog to determine whether to attack a target.");
		return true;
	}

	protected bool CheckAllTargetsForAttack(ICharacter ch)
	{
		if (ch.State.HasFlag(CharacterState.Dead) || ch.State.HasFlag(CharacterState.Stasis))
		{
			return false;
		}

		if (!CharacterState.Able.HasFlag(ch.State))
		{
			return false;
		}

		if (ch.Combat != null && ch.CombatTarget is ICharacter ctch &&
		    PredatorAIHelpers.WillAttack(ch, ctch, WillAttackProg, true))
		{
			return false;
		}

		if (ch.Effects.Any(x => x.IsBlockingEffect("combat-engage") || x.IsBlockingEffect("general")))
		{
			return false;
		}

		foreach (ICharacter tch in ch.Location.LayerCharacters(ch.RoomLayer).Except(ch).Shuffle())
		{
			if (CheckForAttack(ch, tch))
			{
				return true;
			}
		}

		uint range = (uint)ch.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
		                  .Where(x => x.IsReadied || x.CanReady(ch))
		                  .Select(x => (int)x.WeaponType.DefaultRangeInRooms)
		                  .DefaultIfEmpty(0)
		                  .Max();
		if (range > 0)
		{
			foreach (ICharacter tch in ch.Location.CellsInVicinity(range, true, true)
			                             .Except(ch.Location)
			                             .SelectMany(x => x.Characters)
			                             .ToList())
			{
				if (CheckForAttack(ch, tch))
				{
					return true;
				}
			}
		}

		return false;
	}

	public virtual bool CheckForAttack(ICharacter aggressor, ICharacter target)
	{
		return PredatorAIHelpers.CheckForAttack(aggressor, target, WillAttackProg, EngageDelayDiceExpression,
			EngageEmote, true);
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.TenSecondTick:
				ICharacter ch = (ICharacter)arguments[0];
				if (ch.State.IsDead() || ch.State.IsInStatis())
				{
					return false;
				}

				return CheckAllTargetsForAttack(ch);
			case EventType.CharacterEnterCellWitness:
				ch = (ICharacter)arguments[3];
				if (ch.State.IsDead() || ch.State.IsInStatis())
				{
					return false;
				}

				return CheckForAttack(ch, (ICharacter)arguments[0]);
		}

		return base.HandleEvent(type, arguments);
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (EventType type in types)
		{
			switch (type)
			{
				case EventType.CharacterEnterCellWitness:
				case EventType.TenSecondTick:
					return true;
			}
		}

		return base.HandlesEvent(types);
	}
}
