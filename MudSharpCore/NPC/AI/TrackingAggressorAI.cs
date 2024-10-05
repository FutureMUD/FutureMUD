using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.NPC.AI.Strategies;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Construction;
using System.Text;

namespace MudSharp.NPC.AI;

public class TrackingAggressorAI : PathingAIWithProgTargetsBase
{
	public IFutureProg WillAttackProg { get; set; }
	public string EngageDelayDiceExpression { get; set; }
	public string EngageEmote { get; set; }

	public uint MaximumRange { get; set; }
	public override bool CountsAsAggressive => true;

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Will Attack Prog: {WillAttackProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Maximum Range: {MaximumRange.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Engage Delay: {EngageDelayDiceExpression.ColourValue()} milliseconds");
		sb.AppendLine($"Engage Emote: {EngageEmote?.ColourCommand() ?? ""}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3willattack <prog>#0 - sets the prog that controls if an NPC will attack
	#3range <##>#0 - sets the range in rooms
	#3delay <expression>#0 - sets a dice expression for engage delay in milliseconds
	#3emote <emote>#0 - sets an engage emote when engaging an enemy ($0 = aggressor, $1 = target)
	#3emote clear#0 - clears the engage emote";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "will":
			case "willattack":
			case "attack":
			case "willprog":
			case "willattackprog":
			case "attackprog":
				return BuildingCommandWillAttackProg(actor, command);
			case "range":
				return BuildingCommandRange(actor, command);
			case "delay":
				return BuildingCommandDelay(actor, command);
			case "emote":
			case "engage":
			case "engageemote":
				return BuildingCommandEngageEmote(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandEngageEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify an emote or use #3clear#0 to clear it."
				.SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "delete", "remove", "none"))
		{
			EngageEmote = string.Empty;
			Changed = true;
			actor.OutputHandler.Send($"This AI will no longer do any emote when engaging a target.");
			return true;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EngageEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"When engaging a target, this AI will do the following emote: {EngageEmote.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid dice expression for delay in milliseconds before engaging.");
			return false;
		}

		if (!Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid dice expression.");
			return false;
		}

		EngageDelayDiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now wait {EngageDelayDiceExpression.ColourValue()} milliseconds before engaging a valid target.");
		return true;
	}

	private bool BuildingCommandRange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !uint.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid range in rooms for this AI to be aggressive towards.");
			return false;
		}

		MaximumRange = value;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now scan {value.ToString("N0", actor).ColourValue()} {"room".Pluralise(value != 1)} around for targets.");
		return true;
	}

	private bool BuildingCommandWillAttackProg(ICharacter actor, StringStack command)
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
		actor.OutputHandler.Send($"This AI will now use the {prog.MXPClickableFunctionName()} prog to determine whether to attack a target.");
		return true;
	}

	protected TrackingAggressorAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private TrackingAggressorAI()
	{

	}

	private TrackingAggressorAI(IFuturemud gameworld, string name) : base(gameworld, name, "TrackingAggressor")
	{
		WillAttackProg = Gameworld.AlwaysTrueProg;
		EngageDelayDiceExpression = "250+1d1500";
		EngageEmote = string.Empty;
		MaximumRange = 2;
		DatabaseInitialise();
	}

	protected override void LoadFromXML(XElement root)
	{
		base.LoadFromXML(root);
		WillAttackProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WillAttackProg").Value));
		EngageDelayDiceExpression = root.Element("EngageDelayDiceExpression").Value;
		EngageEmote = root.Element("EngageEmote")?.Value;
		MaximumRange = uint.Parse(root.Element("MaximumRange").Value);
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
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay),
			new XElement("MaximumRange", MaximumRange)
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
			                      .SelectMany(x => x.Characters).ToList())
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
				return CheckAllTargetsForAttack(ch);
			case EventType.CharacterEnterCellWitness:
				return CheckForAttack((ICharacter)arguments[3], (ICharacter)arguments[0]);
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
		RegisterAIType("TrackingAggressor", (ai, gameworld) => new TrackingAggressorAI(ai, gameworld));
	}

	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		var target = ch.AcquireTargetAndPath(GetTargetFunction(ch), MaximumRange, GetSuitabilityFunction(ch));
		if (target.Item1 == null || !target.Item2.Any())
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		return (target.Item1.Location, target.Item2);
	}
}