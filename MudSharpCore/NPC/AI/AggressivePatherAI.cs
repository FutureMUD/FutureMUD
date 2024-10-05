using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.NPC.AI;

public class AggressivePatherAI : PathingAIWithProgTargetsBase
{
	public IFutureProg WillAttackProg { get; set; }
	public string EngageDelayDiceExpression { get; set; }
	public string EngageEmote { get; set; }
	public override bool CountsAsAggressive => true;

	protected AggressivePatherAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private AggressivePatherAI(IFuturemud gameworld, string name) : base(gameworld, name, "AggressivePather")
	{
		WillAttackProg = Gameworld.AlwaysFalseProg;
		EngageDelayDiceExpression = "1000+1d1000";
		EngageEmote = string.Empty;
		PathingEnabledProg = Gameworld.AlwaysFalseProg;
		DatabaseInitialise();
	}

	private AggressivePatherAI()
	{
	}

	protected override void LoadFromXML(XElement root)
	{
		base.LoadFromXML(root);
		WillAttackProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WillAttackProg").Value));
		EngageDelayDiceExpression = root.Element("EngageDelayDiceExpression").Value;
		EngageEmote = root.Element("EngageEmote")?.Value;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("WillAttackProg", WillAttackProg?.Id ?? 0L),
			new XElement("EngageDelayDiceExpression", new XCData(EngageDelayDiceExpression)),
			new XElement("EngageEmote", new XCData(EngageEmote)),
			new XElement("PathingEnabledProg", PathingEnabledProg?.Id ?? 0L),
			new XElement("OnStartToPathProg", OnStartToPathProg?.Id ?? 0L),
			new XElement("TargetLocationProg", TargetLocationProg?.Id ?? 0L),
			new XElement("FallbackLocationProg", FallbackLocationProg?.Id ?? 0L),
			new XElement("WayPointsProg", WayPointsProg?.Id ?? 0L),
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay)
		).ToString();
	}

	private bool CheckAllTargetsForAttack(ICharacter ch)
	{
		if (ch.State.HasFlag(CharacterState.Dead) || ch.State.HasFlag(CharacterState.Stasis))
		{
			return false;
		}

		if (!CharacterState.Able.HasFlag(ch.State))
		{
			return false;
		}

		if (ch.Combat != null && ch.CombatTarget is ICharacter ctch && WillAttack(ch, ctch))
		{
			return false;
		}

		if (ch.Effects.Any(x => x.IsBlockingEffect("combat-engage") || x.IsBlockingEffect("general")))
		{
			return false;
		}

		foreach (var tch in ch.Location.Characters.Except(ch).Shuffle())
		{
			if (CheckForAttack(ch, tch))
			{
				return true;
			}
		}

		var range = (uint)ch.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
		                    .Where(x => x.IsReadied || x.CanReady(ch))
		                    .Select(x => (int)x.WeaponType.DefaultRangeInRooms)
		                    .DefaultIfEmpty(0).Max();
		// TODO - natural attacks
		if (range > 0)
			//TODO: With this, AI can find you through doorways it doesn't have direct LOS into, which doesn't seem fair
			//Worth revisiting at some point.
		{
			foreach (var tch in ch.Location.CellsInVicinity(range, true, true).Except(ch.Location)
			                      .SelectMany(x => x.Characters).Shuffle().ToList())
			{
				if (CheckForAttack(ch, tch))
				{
					return true;
				}
			}
		}

		return false;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var ch = type == EventType.CharacterEnterCellWitness ?
			(ICharacter)arguments[3] :
			(ICharacter)arguments[0];
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		switch (type)
		{
			case EventType.TenSecondTick:
				ch = (ICharacter)arguments[0];
				return CheckAllTargetsForAttack(ch);
			case EventType.CharacterEnterCellWitness:
				return CheckForAttack((ICharacter)arguments[0], (ICharacter)arguments[3]);
		}

		return base.HandleEvent(type, arguments);
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.TenSecondTick:
				case EventType.CharacterEnterCellFinishWitness:
					return true;
			}
		}

		return base.HandlesEvent(types);
	}

	public bool CheckForAttack(ICharacter aggressor, ICharacter target)
	{
		if (!WillAttack(aggressor, target))
		{
			return false;
		}

		aggressor.AddEffect(
			new BlockingDelayedAction(aggressor, perceiver => { EngageTarget(aggressor, target); },
				"preparing to engage in combat", new[] { "general", "combat-engage", "movement" }, null),
			TimeSpan.FromMilliseconds(Dice.Roll(EngageDelayDiceExpression)));

		return true;
	}

	private bool WillAttack(ICharacter aggressor, ICharacter target)
	{
		if (aggressor.State.HasFlag(CharacterState.Dead) || aggressor.State.HasFlag(CharacterState.Stasis))
		{
			return false;
		}

		if (aggressor.Combat != null)
		{
			return false;
		}

		if (aggressor == target)
		{
			return false;
		}

		if (!CharacterState.Able.HasFlag(aggressor.State))
		{
			return false;
		}

		if (!aggressor.CanSee(target))
		{
			return false;
		}

		if (!((bool?)WillAttackProg.Execute(aggressor, target) ?? false))
		{
			return false;
		}

		if (aggressor.CombatTarget != target && !aggressor.CanEngage(target))
		{
			return false;
		}

		if (aggressor.Effects.Any(x => x.IsBlockingEffect("combat-engage")) ||
		    aggressor.Effects.Any(x => x.IsBlockingEffect("general")))
		{
			return false;
		}

		return true;
	}

	private Func<IPerceivable, bool> GetTargetFunction(ICharacter ch)
	{
		return x =>
		{
			if (x is not ICharacter xCh)
			{
				return false;
			}

			if (!(bool?)WillAttackProg.Execute(ch, x) ?? false)
			{
				return false;
			}

			if (!ch.CanSee(x))
			{
				return false;
			}

			return true;
		};
	}

	private void EngageTarget(ICharacter ch, IPerceiver tp)
	{
		if (ch.State.HasFlag(CharacterState.Dead) || ch.Corpse != null)
		{
			return;
		}

		if (tp is ICharacter tch && tch.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		if (!ch.CanEngage(tp))
		{
			return;
		}

		if (!string.IsNullOrWhiteSpace(EngageEmote))
		{
			ch.OutputHandler.Handle(new EmoteOutput(new Emote(EngageEmote, ch, ch, tp)));
		}

		ch.Engage(tp, ch.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
		                .Where(x => x.IsReadied || x.CanReady(ch)).Select(x => (int)x.WeaponType.DefaultRangeInRooms)
		                .DefaultIfEmpty(-1).Max() > 0);
	}

	public static void RegisterLoader()
	{
		RegisterAIType("AggressivePather", (ai, gameworld) => new AggressivePatherAI(ai, gameworld));
		RegisterAIBuilderInformation("aggressivepather", (gameworld,name) => new AggressivePatherAI(gameworld, name), new AggressivePatherAI().HelpText);
	}

	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		if (ch.Effects.Any(x => x.IsBlockingEffect("movement")))
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		var target = ch.AcquireTargetAndPath(GetTargetFunction(ch), 5, GetSuitabilityFunction(ch));
		if (target.Item1 != null && target.Item2.Any())
		{
			return (target.Item1.Location, target.Item2);
		}

		var location = TargetLocationProg?.Execute<ICell>(ch);
		if (location == null || Equals(location, ch.Location))
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		// First try to find a path to the primary target
		var path = ch.PathBetween(location, 12, GetSuitabilityFunction(ch));
		if (path.Any())
		{
			return (location, path);
		}

		if (MoveEvenIfObstructionInWay)
		{
			path = ch.PathBetween(location, 12, GetSuitabilityFunction(ch, false));
			if (path.Any())
			{
				return (location, path);
			}
		}

		// If we can't find a path to the primary target, check if there is a fallback target
		location = FallbackLocationProg?.Execute<ICell>(ch);
		if (location == null || location == ch.Location)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		path = ch.PathBetween(location, 12, GetSuitabilityFunction(ch));
		if (path.Any())
		{
			return (location, path);
		}

		if (MoveEvenIfObstructionInWay)
		{
			path = ch.PathBetween(location, 12, GetSuitabilityFunction(ch, false));
			if (path.Any())
			{
				return (location, path);
			}
		}

		// If the fallback target can't be reached, see if we can reach of any of the way points
		if (WayPointsProg is not null)
		{
			path = ch.PathBetween((WayPointsProg.ExecuteCollection<ICell>(ch)).ToList(), 12,
				GetSuitabilityFunction(ch));
			if (path.Any())
			{
				return (location, path);
			}
		}
		

		return (null, Enumerable.Empty<ICellExit>());
	}

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Will Attack Prog: {WillAttackProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Engage Delay: {EngageDelayDiceExpression.ColourValue()} milliseconds");
		sb.AppendLine($"Engage Emote: {EngageEmote?.ColourCommand() ?? ""}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => @$"{base.TypeHelpText}
	#3attackprog <prog>#0 - sets the prog that controls target selection
	#3emote <emote>#0 - sets the engage emote ($0 = npc, $1 = target)
	#3emote clear#0 - clears the emote (won't do an emote when engaging)
	#3delay <dice expression>#0 - sets the delay (in ms) before attacking when spotting a target";

	#region Overrides of PathingAIWithProgTargetsBase

	/// <inheritdoc />
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
		actor.OutputHandler.Send($"The NPC will now wait {EngageDelayDiceExpression.ColourValue()} milliseconds before attacking.");
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

		var text = command.SafeRemainingArgument;
		if (text.EqualToAny("remove", "clear", "delete"))
		{
			EngageEmote = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This NPC will no longer do any emote when engaging targets.");
			return true;
		}

		var emote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
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

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WillAttackProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This NPC will now use the {prog.MXPClickableFunctionName()} prog to determine whether to attack a target.");
		return true;
	}

	#endregion
}