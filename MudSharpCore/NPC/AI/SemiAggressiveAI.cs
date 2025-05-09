﻿using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public class SemiAggressiveAI : PathingAIWithProgTargetsBase
{
	public override bool CountsAsAggressive => true;

	protected SemiAggressiveAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private SemiAggressiveAI(IFuturemud gameworld, string name) : base(gameworld, name, "SemiAggressive")
	{
		WillAttackProg = Gameworld.AlwaysFalseProg;
		WillPostureProg = Gameworld.AlwaysFalseProg;
		WillFleeProg = Gameworld.AlwaysFalseProg;
		WillAttackPostureEscalationProg = Gameworld.AlwaysFalseProg;
		AttackEmoteProg = Gameworld.EmptyTextProg;
		FleeEmoteProg = Gameworld.EmptyTextProg;
		PostureEmoteProg = Gameworld.EmptyTextProg;
		PostureTimeSpanDiceExpression = "30+1d20";
		ThreatPerHostilePreCombatAction = 1.0;
		ThreatEscalationPerAdditionalTarget = 0.3;
		ThreatPerEscalationTick = 0.5;
		ThreatPerInventoryChange = 0.05;
		DatabaseInitialise();
	}

	private SemiAggressiveAI()
	{

	}

	public static void RegisterLoader()
	{
		RegisterAIType("SemiAggressive", (ai, gameworld) => new SemiAggressiveAI(ai, gameworld));
		RegisterAIBuilderInformation("semiaggressive", (game, name) => new SemiAggressiveAI(game, name), new SemiAggressiveAI().HelpText);
	}

	protected override void LoadFromXML(XElement root)
	{
		base.LoadFromXML(root);
		var element = root.Element("WillAttackProg");
		if (element != null)
		{
			WillAttackProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);
			if (WillAttackProg == null)
			{
				WillAttackProg = Gameworld.AlwaysFalseProg;
			}
		}
		else
		{
			throw new ApplicationException($"SemiAggressive #{Id} ({Name}) did not supply a WillAttackProg element.");
		}

		element = root.Element("WillPostureProg");
		if (element != null)
		{
			WillPostureProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);
			if (WillPostureProg == null)
			{
				WillPostureProg = Gameworld.AlwaysFalseProg;
			}
		}
		else
		{
			throw new ApplicationException($"SemiAggressive #{Id} ({Name}) did not supply a WillPostureProg element.");
		}

		element = root.Element("WillFleeProg");
		if (element != null)
		{
			WillFleeProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);
			if (WillFleeProg == null)
			{
				WillFleeProg = Gameworld.AlwaysFalseProg;
			}
		}
		else
		{
			throw new ApplicationException($"SemiAggressive #{Id} ({Name}) did not supply a WillFleeProg element.");
		}

		element = root.Element("WillAttackPostureEscalationProg");
		if (element != null)
		{
			WillAttackPostureEscalationProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);
			if (WillAttackPostureEscalationProg == null)
			{
				WillAttackPostureEscalationProg = Gameworld.AlwaysFalseProg;
			}
		}
		else
		{
			throw new ApplicationException(
				$"SemiAggressive #{Id} ({Name}) did not supply a WillAttackPostureEscalationProg element.");
		}

		element = root.Element("PostureEmoteProg");
		if (element != null)
		{
			PostureEmoteProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);
			if (PostureEmoteProg == null)
			{
				PostureEmoteProg = Gameworld.EmptyTextProg;
			}
		}
		else
		{
			throw new ApplicationException($"SemiAggressive #{Id} ({Name}) did not supply a PostureEmoteProg element.");
		}

		element = root.Element("AttackEmoteProg");
		if (element != null)
		{
			AttackEmoteProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);
			if (AttackEmoteProg == null)
			{
				AttackEmoteProg = Gameworld.EmptyTextProg;
			}
		}
		else
		{
			throw new ApplicationException($"SemiAggressive #{Id} ({Name}) did not supply a AttackEmoteProg element.");
		}

		element = root.Element("FleeEmoteProg");
		if (element != null)
		{
			FleeEmoteProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);
			if (FleeEmoteProg == null)
			{
				FleeEmoteProg = Gameworld.EmptyTextProg;
			}
		}
		else
		{
			throw new ApplicationException($"SemiAggressive #{Id} ({Name}) did not supply a FleeEmoteProg element.");
		}

		element = root.Element("FleeLocationsProg");
		if (element != null)
		{
			FleeLocationsProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);
			if (FleeLocationsProg == null)
			{
				FleeLocationsProg = Gameworld.EmptyTextProg;
			}
		}
		else
		{
			throw new ApplicationException(
				$"SemiAggressive #{Id} ({Name}) did not supply a FleeLocationsProg element.");
		}

		element = root.Element("PostureTimeSpanDiceExpression");
		if (element != null)
		{
			if (!Dice.IsDiceExpression(element.Value))
			{
				PostureTimeSpanDiceExpression = "0";
			}
			else
			{
				PostureTimeSpanDiceExpression = element.Value;
			}
		}
		else
		{
			throw new ApplicationException(
				$"SemiAggressive #{Id} ({Name}) did not supply a PostureTimeSpanDiceExpression element.");
		}

		element = root.Element("ThreatPerEscalationTick");
		if (element != null)
		{
			if (!double.TryParse(element.Value, out var value))
			{
				throw new ApplicationException(
					$"SemiAggressive #{Id} ({Name}) supplied a ThreatPerEscalationTick that was not a number.");
			}

			ThreatPerEscalationTick = value;
		}
		else
		{
			throw new ApplicationException(
				$"SemiAggressive #{Id} ({Name}) did not supply a ThreatPerEscalationTick element.");
		}

		element = root.Element("ThreatPerInventoryChange");
		if (element != null)
		{
			if (!double.TryParse(element.Value, out var value))
			{
				throw new ApplicationException(
					$"SemiAggressive #{Id} ({Name}) supplied a ThreatPerInventoryChange that was not a number.");
			}

			ThreatPerInventoryChange = value;
		}
		else
		{
			throw new ApplicationException(
				$"SemiAggressive #{Id} ({Name}) did not supply a ThreatPerInventoryChange element.");
		}

		element = root.Element("ThreatPerHostilePreCombatAction");
		if (element != null)
		{
			if (!double.TryParse(element.Value, out var value))
			{
				throw new ApplicationException(
					$"SemiAggressive #{Id} ({Name}) supplied a ThreatPerHostilePreCombatAction that was not a number.");
			}

			ThreatPerHostilePreCombatAction = value;
		}
		else
		{
			throw new ApplicationException(
				$"SemiAggressive #{Id} ({Name}) did not supply a ThreatPerHostilePreCombatAction element.");
		}

		element = root.Element("ThreatEscalationPerAdditionalTarget");
		if (element != null)
		{
			if (!double.TryParse(element.Value, out var value))
			{
				throw new ApplicationException(
					$"SemiAggressive #{Id} ({Name}) supplied a ThreatEscalationPerAdditionalTarget that was not a number.");
			}

			ThreatEscalationPerAdditionalTarget = value;
		}
		else
		{
			throw new ApplicationException(
				$"SemiAggressive #{Id} ({Name}) did not supply a ThreatEscalationPerAdditionalTarget element.");
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("WillAttackProg", WillAttackProg?.Id ?? 0L),
			new XElement("WillPostureProg", WillPostureProg?.Id ?? 0L),
			new XElement("WillFleeProg", WillFleeProg?.Id ?? 0L),
			new XElement("WillAttackPostureEscalationProg", WillAttackPostureEscalationProg?.Id ?? 0L),
			new XElement("PostureEmoteProg", PostureEmoteProg?.Id ?? 0L),
			new XElement("AttackEmoteProg", AttackEmoteProg?.Id ?? 0L),
			new XElement("FleeEmoteProg", FleeEmoteProg?.Id ?? 0L),
			new XElement("PostureTimeSpanDiceExpression", new XCData(PostureTimeSpanDiceExpression)),
			new XElement("ThreatPerEscalationTick", ThreatPerEscalationTick),
			new XElement("ThreatPerInventoryChange", ThreatPerInventoryChange),
			new XElement("ThreatPerHostilePreCombatAction", ThreatPerHostilePreCombatAction),
			new XElement("ThreatEscalationPerAdditionalTarget", ThreatEscalationPerAdditionalTarget),
			new XElement("FleeLocationsProg", FleeLocationsProg?.Id ?? 0L),
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

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		ICharacter ch = null;
		switch (type)
		{
			case EventType.CharacterEnterCellWitness:
			case EventType.CharacterLeaveCellWitness:
			case EventType.CharacterGotItemContainerWitness:
				ch = (ICharacter)arguments[3];
				break;
			case EventType.EngagedInCombat:
			case EventType.TargetIncapacitated:
				ch = (ICharacter)arguments[1];
				break;
			case EventType.LeaveCombat:
				ch = (ICharacter)arguments[0];
				break;
			case EventType.CharacterGotItemWitness:
				ch = (ICharacter)arguments[2];
				break;
		}

		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		switch (type)
		{
			case EventType.CharacterEnterCellWitness:
				return CharacterEnterCellWitness((ICharacter)arguments[0], (ICell)arguments[1], (ICellExit)arguments[2],
					(ICharacter)arguments[3]);
			case EventType.CharacterLeaveCellWitness:
				return CharacterLeaveCellWitness((ICharacter)arguments[0], (ICell)arguments[1], (ICellExit)arguments[2],
					(ICharacter)arguments[3]);
			case EventType.EngagedInCombat:
				return EngagedInCombat((ICharacter)arguments[0], (ICharacter)arguments[1]);
			case EventType.LeaveCombat:
				return LeaveCombat((ICharacter)arguments[0]);
			case EventType.TargetIncapacitated:
				return TargetIncapacitated((ICharacter)arguments[0], (ICharacter)arguments[1]);
			case EventType.CharacterGotItemWitness:
				return InventoryChangeWitness((ICharacter)arguments[0], (ICharacter)arguments[2]);
			case EventType.CharacterGotItemContainerWitness:
				return InventoryChangeWitness((ICharacter)arguments[0], (ICharacter)arguments[3]);
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.CharacterEnterCellWitness:
				case EventType.CharacterLeaveCellWitness:
				case EventType.EngagedInCombat:
				case EventType.LeaveCombat:
				case EventType.TargetIncapacitated:
				case EventType.CharacterGotItemContainerWitness:
				case EventType.CharacterGotItemWitness:
					return true;
			}
		}

		return base.HandlesEvent(types);
	}

	protected IFutureProg WillAttackProg { get; set; }
	protected IFutureProg WillPostureProg { get; set; }
	protected IFutureProg WillFleeProg { get; set; }
	protected IFutureProg WillAttackPostureEscalationProg { get; set; }
	protected IFutureProg PostureEmoteProg { get; set; }
	protected IFutureProg AttackEmoteProg { get; set; }
	protected IFutureProg FleeEmoteProg { get; set; }
	protected IFutureProg FleeLocationsProg { get; set; }

	protected string PostureTimeSpanDiceExpression { get; set; }

	protected double ThreatPerEscalationTick { get; set; }
	protected double ThreatPerInventoryChange { get; set; }
	protected double ThreatPerHostilePreCombatAction { get; set; }
	protected double ThreatEscalationPerAdditionalTarget { get; set; }

	protected double ExistingThreatLevel(ICharacter character)
	{
		return character.EffectsOfType<AIPosturingEffect>().FirstOrDefault()?.ThreatLevel ?? 0.0;
	}

	protected virtual bool WillPosture(ICharacter character, ICell cell, ICharacter specific,
		params ICharacter[] targets)
	{
		return WillPostureProg.ExecuteBool(character, cell, specific, targets, ExistingThreatLevel(character));
	}

	protected virtual bool WillAttack(ICharacter character, ICell cell, ICharacter specific,
		params ICharacter[] targets)
	{
		return WillAttackProg.ExecuteBool(character, cell, specific, targets, ExistingThreatLevel(character));
	}

	protected virtual bool WillFlee(ICharacter character, ICell cell, params ICharacter[] targets)
	{
		return WillFleeProg.ExecuteBool(character, cell, targets, ExistingThreatLevel(character));
	}

	protected virtual (ICell Target, IEnumerable<ICellExit>) FleeRouteForCharacter(ICharacter character)
	{
		var locations = ((IList)((CollectionVariable)FleeLocationsProg.Execute(character)).GetObject).OfType<ICell>();

		foreach (var loc in locations)
		{
			var path = character.PathBetween(loc, 12, OpenDoors).ToList();
			if (path.Any() == true)
			{
				return (loc, path);
			}
		}

		return (null, Enumerable.Empty<ICellExit>());
	}

	private void BeginAttack(ICharacter attacker, ICharacter target)
	{
		var emoteText = AttackEmoteProg.Execute(attacker, target)?.ToString();
		if (!string.IsNullOrEmpty(emoteText))
		{
			var emote = new Emote(emoteText, attacker, attacker, target);
			if (emote.Valid)
			{
				attacker.OutputHandler.Handle(new EmoteOutput(emote, flags: OutputFlags.InnerWrap));
			}
			else
			{
				Gameworld.SystemMessage(
					$"There was an error with the SemiAggressive [#{Id} - {Name}] attack emote for NPC {attacker.HowSeen(attacker, flags: PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee)} in location {attacker.Location.Id} ({attacker.Location.HowSeen(attacker, flags: PerceiveIgnoreFlags.IgnoreCanSee)}).\nThe raw emote was:\n\n{emoteText}");
			}
		}

		attacker.Engage(target);
	}

	protected virtual void BeginFlee(ICharacter character)
	{
		var effect = character.EffectsOfType<AIPosturingEffect>().First();
		var emoteText = FleeEmoteProg.Execute(character, effect.PosturingTargets, effect.ThreatLevel)?.ToString();
		if (!string.IsNullOrEmpty(emoteText))
		{
			var emoteArray = new IPerceivable[] { character }.Concat(effect.PosturingTargets).ToArray();
			var index = 1;
			var emote = new Emote(
				string.Format(emoteText, effect.PosturingTargets.Select(x => "$" + index++).ListToString()), character,
				emoteArray);
			if (emote.Valid)
			{
				character.OutputHandler.Handle(new EmoteOutput(emote, flags: OutputFlags.InnerWrap));
			}
			else
			{
				Gameworld.SystemMessage(
					$"There was an error with the SemiAggressive [#{Id} - {Name}] flee emote for NPC {character.HowSeen(character, flags: PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee)} in location {character.Location.Id} ({character.Location.HowSeen(character, flags: PerceiveIgnoreFlags.IgnoreCanSee)}).\nThe raw emote was:\n\n{emoteText}");
			}
		}

		var fleeEffect = new AIRecentlyFledPosturing(character, effect.ThreatLevel);
		character.AddEffect(fleeEffect, TimeSpan.FromSeconds(60));
		character.RemoveEffect(effect);
		var (target, route) = FleeRouteForCharacter(character);
		if (route.Any())
		{
			var pathEffect = new FollowingPath(character, route);
			character.AddEffect(pathEffect);
			FollowPathAction(character, pathEffect);
		}
	}

	protected virtual void BeginPosturing(ICharacter character, ICharacter target)
	{
		(double Threat, bool StillPosturing, TimeSpan PostureLength) OnPostureEffectExpire(double currentThreat,
			IEnumerable<ICharacter> targets)
		{
			var newThreat = currentThreat + ThreatPerEscalationTick;
			var targetList = targets.ToList();
			if (WillAttackPostureEscalationProg.ExecuteBool(character, targetList, newThreat))
			{
				return (0, false, TimeSpan.Zero);
			}

			if (WillFlee(character, character.Location, targetList.ToArray()))
			{
				BeginFlee(character);
				return (0, false, TimeSpan.Zero);
			}

			var emoteText = PostureEmoteProg.Execute(character, targetList, newThreat)?.ToString();
			if (!string.IsNullOrEmpty(emoteText))
			{
				var emoteArray = new IPerceivable[] { character }.Concat(targetList).ToArray();
				var index = 1;
				var emote = new Emote(string.Format(emoteText, targetList.Select(x => "$" + index++).ListToString()),
					character, emoteArray);
				if (emote.Valid)
				{
					character.OutputHandler.Handle(new EmoteOutput(emote, flags: OutputFlags.InnerWrap));
				}
				else
				{
					Gameworld.SystemMessage(
						$"There was an error with the SemiAggressive [#{Id} - {Name}] posturing emote for NPC {character.HowSeen(character, flags: PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee)} in location {character.Location.Id} ({character.Location.HowSeen(character, flags: PerceiveIgnoreFlags.IgnoreCanSee)}).\nThe raw emote was:\n\n{emoteText}");
				}
			}

			return (ThreatPerEscalationTick, true, TimeSpan.FromSeconds(Dice.Roll(PostureTimeSpanDiceExpression)));
		}

		var effect = character.EffectsOfType<AIPosturingEffect>().FirstOrDefault();
		if (effect == null)
		{
			effect = new AIPosturingEffect(character, new[] { target }, OnPostureEffectExpire);
			var fledEffect = character.EffectsOfType<AIRecentlyFledPosturing>().FirstOrDefault();
			character.AddEffect(effect, TimeSpan.FromSeconds(Dice.Roll(PostureTimeSpanDiceExpression)));
			effect.ThreatLevel = fledEffect?.PreviousThreat ?? 0.0;
			character.RemoveAllEffects(x => x.IsEffectType<AIRecentlyFledPosturing>());
		}
		else
		{
			effect.PosturingTargets.Add(target);
			effect.ThreatLevel += ThreatEscalationPerAdditionalTarget;
		}
	}

	private bool WillHandleAttackOrPosture(ICharacter character)
	{
		return CharacterState.Able.HasFlag(character.State) && character.Combat != null &&
		       !character.AffectedBy<AIFleeing>();
	}

	private bool WillHandleFlee(ICharacter character)
	{
		return CharacterState.Able.HasFlag(character.State) && character.CanMove(true) &&
		       !character.AffectedBy<AIFleeing>();
	}

	private ICharacter[] GetCharacterTargets(ICharacter character, ICharacter additional)
	{
		var targets = new HashSet<ICharacter>();
		if (character.Combat != null)
		{
			if (character.CombatTarget is ICharacter cht)
			{
				targets.Add(cht);
			}

			foreach (var tch in character.Combat.Combatants.Where(x => x.CombatTarget == character)
			                             .OfType<ICharacter>())
			{
				targets.Add(tch);
			}
		}

		var posturing = character.EffectsOfType<AIPosturingEffect>().FirstOrDefault();
		if (posturing != null)
		{
			foreach (var target in posturing.PosturingTargets)
			{
				targets.Add(target);
			}
		}

		targets.Add(additional);
		return targets.ToArray();
	}

	private bool CharacterEnterCellWitness(ICharacter mover, ICell cell, ICellExit cellExit, ICharacter witness)
	{
		var targets = GetCharacterTargets(witness, mover);
		if (WillHandleFlee(witness) && WillFlee(witness, cell, targets))
		{
			BeginFlee(witness);
			return true;
		}

		if (WillHandleAttackOrPosture(witness))
		{
			if (WillAttack(witness, cell, mover, targets))
			{
				BeginAttack(witness, mover);
				return true;
			}

			if (WillPosture(witness, cell, mover, targets))
			{
				BeginPosturing(witness, mover);
				return true;
			}
		}

		return false;
	}

	private bool CharacterLeaveCellWitness(ICharacter mover, ICell cell, ICellExit cellExit, ICharacter witness)
	{
		var posturing = witness.EffectsOfType<AIPosturingEffect>().FirstOrDefault();
		if (posturing != null && posturing.PosturingTargets.Contains(mover))
		{
			if (WillPosture(witness, cellExit.Destination, mover))
			{
				return false;
			}

			posturing.PosturingTargets.Remove(mover);
			if (!posturing.PosturingTargets.Any())
			{
				witness.RemoveEffect(posturing);
			}
		}

		return false;
	}

	private bool EngagedInCombat(ICharacter aggressor, ICharacter target)
	{
		target.RemoveAllEffects(x => x.IsEffectType<AIPosturingEffect>());
		return false;
	}

	private bool TargetIncapacitated(ICharacter aggressor, ICharacter target)
	{
		// Check to see if anyone else is already attacking and switch to them first
		foreach (var other in aggressor.Combat?.Combatants.Where(x => x.CombatTarget == aggressor)
		                               .OrderByDescending(x => x.Location == aggressor.Location).ToList())
		{
			if (aggressor.CanEngage(other))
			{
				aggressor.Engage(other);
				return true;
			}
		}

		return false;
	}

	private bool LeaveCombat(ICharacter aggressor)
	{
		var targets = GetCharacterTargets(aggressor, null);
		if (WillHandleFlee(aggressor) && WillFlee(aggressor, aggressor.Location, targets))
		{
			BeginFlee(aggressor);
			return true;
		}

		if (WillHandleAttackOrPosture(aggressor))
		{
			var loctargets = aggressor.Location.Characters.Except(aggressor).ToList();
			foreach (var ch in loctargets)
			{
				if (WillAttack(aggressor, aggressor.Location, ch, targets))
				{
					BeginAttack(aggressor, ch);
					return true;
				}
			}

			foreach (var ch in loctargets)
			{
				if (WillPosture(aggressor, aggressor.Location, ch, targets))
				{
					BeginPosturing(aggressor, ch);
					return true;
				}
			}
		}

		return base.HandleEvent(EventType.LeaveCombat, aggressor);
	}

	private bool InventoryChangeWitness(ICharacter inventoryChanger, ICharacter witness)
	{
		if (witness.EffectsOfType<AIPosturingEffect>().Any(x => x.PosturingTargets.Contains(inventoryChanger)))
		{
			witness.EffectsOfType<AIPosturingEffect>().First().ThreatLevel += ThreatPerInventoryChange;
		}

		return false;
	}

	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		if (!ch.AffectedBy<AIFleeing>())
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		return FleeRouteForCharacter(ch);
	}
}