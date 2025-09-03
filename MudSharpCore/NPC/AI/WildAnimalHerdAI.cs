using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Needs;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.NPC.AI;

public enum WildAnimalHerdRole
{
	Follower,
	Alpha,
	Elderly,
	Child,
	Outsider,
	ChildProtector,
	Sentry
}

public enum WildAnimalHerdState
{
	None,
	Alert,
	Spooked,
	Desperate
}

public enum WildAnimalHerdPriority
{
	Graze,
	SeekWater,
	Sleep,
	Posture,
	Fight,
	Flee,
	Rest
}

#region Reaction Classes

public abstract class WildAnimalHerdAIReaction
{
	public abstract string Name { get; }
	public abstract void DoReaction(ICharacter character, List<(INPC Animal, WildAnimalHerdRole Role)> herd,
		List<ICharacter> stressors, ICharacter mostRecentStressor);
	public abstract XElement SaveReaction(WildAnimalHerdState state);
}

public class WildAnimalHerdEmoteReaction : WildAnimalHerdAIReaction
{
	/// <inheritdoc />
	public override string Name => "Emote";

	public override XElement SaveReaction(WildAnimalHerdState state)
	{
		return new XElement("Reaction", 
			new XElement("state", (int)state),
			new XElement("Role", (int)_targetEmoterRole),
			new XElement("Delay", _delayDiceExpression),
			from emote in _emoteTexts
			select new XElement("Emote", new XCData(emote))
		);
	}

	public WildAnimalHerdEmoteReaction(XElement root)
	{
		_targetEmoterRole =
			(WildAnimalHerdRole)int.Parse(root.Element("Role")?.Value ??
										  throw new ApplicationException("Missing Role element"));
		_delayDiceExpression = root.Element("Delay")?.Value ?? throw new ApplicationException("Missing Delay element");
		if (!Dice.IsDiceExpression(_delayDiceExpression))
		{
			throw new ApplicationException("Invalid Delay element");
		}

		_emoteTexts = root.Elements("Emote").Select(x => x.Value).ToList();
	}

	private WildAnimalHerdRole _targetEmoterRole;
	private readonly List<string> _emoteTexts;
	private string _delayDiceExpression;

	public override void DoReaction(ICharacter character, List<(INPC Animal, WildAnimalHerdRole Role)> herd,
		List<ICharacter> stressors, ICharacter mostRecentStressor)
	{
		var emoter = herd.Where(x => x.Role == _targetEmoterRole).GetRandomElement().Animal;
		if (emoter == null)
		{
			return;
		}

		emoter.AddEffect(new DelayedAction(emoter, perc =>
		{
			if (!CharacterState.Able.HasFlag(emoter.State))
			{
				return;
			}

			emoter.OutputHandler.Handle(new EmoteOutput(new Emote(_emoteTexts.GetRandomElement(), emoter, emoter,
				mostRecentStressor ?? stressors.GetRandomElement(), new PerceivableGroup(stressors))));
		}, "reacting to an intruder"), TimeSpan.FromMilliseconds(Dice.Roll(_delayDiceExpression)));
	}
}

public class WildAnimalHerdFleeReaction : WildAnimalHerdAIReaction
{
	/// <inheritdoc />
	public override string Name => "Flee";

	public override XElement SaveReaction(WildAnimalHerdState state)
	{
		return new XElement("Reaction",
			new XElement("state", (int)state),
			new XElement("Role", (int)_targetEmoterRole),
			new XElement("Delay", _delayDiceExpression),
			new XElement("ExitProg", _directionEvaluationProg?.Id ?? 0),
			from emote in _emoteTexts
			select new XElement("Emote", new XCData(emote))
		);
	}

	public WildAnimalHerdFleeReaction(XElement root, IFuturemud gameworld)
	{
		_targetEmoterRole =
			(WildAnimalHerdRole)int.Parse(root.Element("Role")?.Value ??
										  throw new ApplicationException("Missing Role element"));
		_delayDiceExpression = root.Element("Delay")?.Value ?? throw new ApplicationException("Missing Delay element");
		if (!Dice.IsDiceExpression(_delayDiceExpression))
		{
			throw new ApplicationException("Invalid Delay element");
		}

		_emoteTexts = root.Elements("Emote").Select(x => x.Value).ToList();
		_directionEvaluationProg = gameworld.FutureProgs.Get(long.Parse(root.Element("ExitProg").Value));
	}

	private WildAnimalHerdRole _targetEmoterRole;
	private readonly List<string> _emoteTexts;
	private string _delayDiceExpression;
	private IFutureProg _directionEvaluationProg;

	public override void DoReaction(ICharacter character, List<(INPC Animal, WildAnimalHerdRole Role)> herd,
		List<ICharacter> stressors, ICharacter mostRecentStressor)
	{
		var effect = character.EffectsOfType<WildAnimalHerdEffect>().First();
		if (!effect.AvoidedLocations.Contains(character.Location))
		{
			effect.AvoidedLocations.Add(character.Location);
		}

		var emoter = herd.Where(x => x.Role == _targetEmoterRole).GetRandomElement().Animal ??
					 herd.GetRandomElement().Animal;
		var delay = Dice.Roll(_delayDiceExpression);
		if (emoter != null)
		{
			emoter.AddEffect(new DelayedAction(emoter, perc =>
			{
				if (!CharacterState.Able.HasFlag(emoter.State))
				{
					return;
				}

				emoter.OutputHandler.Handle(new EmoteOutput(new Emote(_emoteTexts.GetRandomElement(), emoter, emoter,
					mostRecentStressor, new PerceivableGroup(stressors))));
			}, "reacting to an intruder"), TimeSpan.FromMilliseconds(delay));
		}

		var mover = effect.HerdLeader.Location == character.Location ? effect.HerdLeader : character;

		mover.AddEffect(new DelayedAction(mover, perc =>
		{
			var exits = mover.Location.ExitsFor(character).ToList();
			var exit = exits.Where(x => mover.CanMove(x))
							.GetWeightedRandom(x => _directionEvaluationProg.ExecuteDouble(0.0, mover, x));
			if (exit == null)
			{
				effect.State = WildAnimalHerdState.Desperate;
				return;
			}

			mover.Move(exit);
		}, "fleeing from an intruder"), TimeSpan.FromMilliseconds(delay + 1));
	}
}

public class WildAnimalAttackReaction : WildAnimalHerdAIReaction
{
	/// <inheritdoc />
	public override string Name => "Attack";

	public override XElement SaveReaction(WildAnimalHerdState state)
	{
		return new XElement("Reaction",
			new XElement("state", (int)state),
			new XElement("Delay", _delayDiceExpression),
			from emote in _emoteTexts
			select new XElement("Emote", new XCData(emote))
		);
	}

	public WildAnimalAttackReaction(XElement root)
	{
		_delayDiceExpression = root.Element("Delay")?.Value ?? throw new ApplicationException("Missing Delay element");
		if (!Dice.IsDiceExpression(_delayDiceExpression))
		{
			throw new ApplicationException("Invalid Delay element");
		}

		_emoteTexts = root.Elements("Emote").Select(x => x.Value).ToList();
	}

	private readonly List<string> _emoteTexts;
	private string _delayDiceExpression;

	public override void DoReaction(ICharacter character, List<(INPC Animal, WildAnimalHerdRole Role)> herd,
		List<ICharacter> stressors, ICharacter mostRecentStressor)
	{
		var aggressor = herd
						.Where(x => x.Animal.Location == character.Location &&
									!x.Role.In(WildAnimalHerdRole.Child, WildAnimalHerdRole.ChildProtector))
						.GetRandomElement()
						.Animal;
		if (aggressor == null)
		{
			return;
		}

		var target = stressors.Where(x => x.Location == character.Location && aggressor.CanEngage(x))
							  .GetRandomElement();
		if (target == null)
		{
			return;
		}

		aggressor.AddEffect(new DelayedAction(aggressor, perc =>
		{
			if (!aggressor.CanEngage(target))
			{
				return;
			}

			aggressor.OutputHandler.Handle(new EmoteOutput(new Emote(_emoteTexts.GetRandomElement(), aggressor,
				aggressor, target, new PerceivableGroup(stressors))));
			aggressor.Engage(target);
		}, "reacting to an intruder"), TimeSpan.FromMilliseconds(Dice.Roll(_delayDiceExpression)));
	}
}

#endregion

public class WildAnimalHerdAI : PathingAIBase
{
	private readonly Dictionary<WildAnimalHerdState, WildAnimalHerdAIReaction> _stateReactionDictionary = new();
	private IFutureProg _escalateThreatProg;
	private IFutureProg _considersThreatProg;
	private IFutureProg _herdRoleProg;
	private IFutureProg _fightOrFlightProg;
	private IFutureProg _willMoveCalmProg;
	private IFutureProg _willMoveAgitatedProg;
	private IFutureProg _willMoveIntoRoomProg;
	private IPositionState _positionStateWhenResting;
	private double _randomEchoChancePerMinute;
	private double _sentryScanChancePerMinute;
	private bool _fleersWillEngageInCombatIfCornered;
	private uint _threatAwarenessDistance;
	private uint _minimumDistanceForOutsiders;
	private uint _maximumDistanceForOutsiders;
	private uint _maximumHerdDispersement;
	private readonly Dictionary<WildAnimalHerdRole, string> _attackWhenAttackedEmotes = new();
	private readonly Counter<WildAnimalHerdRole> _minimumCountsForEachRole = new();
	private readonly List<TimeOfDay> _activeTimes = new();

	private readonly CollectionDictionary<(WildAnimalHerdState State, WildAnimalHerdRole Role), string>
		_randomEmoteDictionary = new();

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Escalate Threat Prog: {_escalateThreatProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Considers Threat Prog: {_considersThreatProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Herd Role Prog: {_herdRoleProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Fight Or Flight Prog: {_fightOrFlightProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Will Move Calm Prog: {_willMoveCalmProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Will Move Agitated Prog: {_willMoveAgitatedProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Will Move Into Room Prog: {_willMoveIntoRoomProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Outsider Min Separation: {_minimumDistanceForOutsiders.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Outsider Max Separation: {_maximumDistanceForOutsiders.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Maximum Herd Dispersement: {_maximumHerdDispersement.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Threat Awareness Distance: {_threatAwarenessDistance.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Random Echo Chance Per Minute: {_randomEchoChancePerMinute.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Scan Chance Per Minuten: {_sentryScanChancePerMinute.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Fleers Engage If Cornered: {_fleersWillEngageInCombatIfCornered.ToColouredString()}");
		sb.AppendLine($"Resting Position: {_positionStateWhenResting.Name.ColourValue()}");
		sb.AppendLine($"Active Times: {_activeTimes.Select(x => x.DescribeColour()).ListToString()}");
		sb.AppendLine();
		sb.AppendLine("Role Counts:\n");
		foreach (var role in _minimumCountsForEachRole)
		{
			sb.AppendLine($"\t{role.Key.DescribeEnum().ColourName()}: {role.Value.ToString("N0", actor).ColourValue()}");
		}

		sb.AppendLine();
		sb.AppendLine("Attack When Attacked Emotes:");
		sb.AppendLine();
		foreach (var (role, emote) in _attackWhenAttackedEmotes)
		{
			sb.AppendLine($"\t{role.DescribeEnum().ColourName()}: {emote.ColourCommand()}");
		}

		sb.AppendLine();
		sb.AppendLine($"State Reactions:");
		sb.AppendLine();
		foreach (var (state, reaction) in _stateReactionDictionary)
		{
			sb.AppendLine($"\t{state.DescribeEnum().ColourName()}: {reaction.Name.ColourValue()}"); // TODO - more than just name
		}

		sb.AppendLine();
		sb.AppendLine("Random Emotes:");
		sb.AppendLine();
		foreach (var (key,emotes) in _randomEmoteDictionary)
		{
			foreach (var emote in emotes)
			{
				sb.AppendLine($"\t[{key.State.DescribeEnum().ColourName()},{key.Role.DescribeEnum().ColourName()}]: {emote.ColourCommand()}");
			}
		}
		return sb.ToString();
	}

	public static void RegisterLoader()
	{
		RegisterAIType("WildAnimalHerd", (ai, gameworld) => new WildAnimalHerdAI(ai, gameworld));
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Reactions",
				from sr in _stateReactionDictionary
				select sr.Value.SaveReaction(sr.Key)
			),
			new XElement("RandomEmotes",
				from reg in _randomEmoteDictionary
				select 
					from re in reg.Value
					select new XElement("Emote", new XAttribute("role", (int)reg.Key.Role), new XAttribute("state", (int)reg.Key.State), new XCData(re))
			),
			new XElement("AttackWhenAttackedEmotes",
				from awae in _attackWhenAttackedEmotes
				select new XElement("Emote", new XAttribute("role", (int)awae.Key), new XCData(awae.Value))
			),
			new XElement("MinimumRoleCounts",
				from mrc in _minimumCountsForEachRole
				select new XElement("Count", new XAttribute("role", (int)mrc.Key), mrc.Value)
			),
			new XElement("ActiveTimes",
				from at in _activeTimes
				select new XElement("ActiveTime", (int)at)
			),
			new XElement("RandomEchoChancePerMinute", _randomEchoChancePerMinute),
			new XElement("SentryScanChancePerMinute", _sentryScanChancePerMinute),
			new XElement("FleersWillEngageInCombatIfCornered", _fleersWillEngageInCombatIfCornered),
			new XElement("ThreatAwarenessDistance", _threatAwarenessDistance),
			new XElement("MinimumDistanceForOutsiders", _minimumDistanceForOutsiders),
			new XElement("MaximumDistanceForOutsiders", _maximumDistanceForOutsiders),
			new XElement("MaximumHerdDispersement", _maximumHerdDispersement),
			new XElement("PositionStateWhenResting", _positionStateWhenResting.Id),
			new XElement("WillMoveIntoRoomProg", _willMoveIntoRoomProg?.Id ?? 0),
			new XElement("EscalateThreatProg", _escalateThreatProg?.Id ?? 0),
			new XElement("ConsidersThreatProg", _considersThreatProg?.Id ?? 0),
			new XElement("HerdRoleProg", _herdRoleProg?.Id ?? 0),
			new XElement("FightOrFlightProg", _fightOrFlightProg?.Id ?? 0), 
			new XElement("WillMoveCalmProg", _willMoveCalmProg?.Id ?? 0),
			new XElement("WillMoveAgitatedProg", _willMoveAgitatedProg?.Id ?? 0)
		).ToString();
	}

	protected WildAnimalHerdAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
		var root = XElement.Parse(ai.Definition);
		var element = root.Element("Reactions");
		if (element == null)
		{
			throw new ApplicationException($"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" did not have a Reactions element.");
		}

		foreach (var sub in element.Elements())
		{
			var state = (WildAnimalHerdState)int.Parse(sub.Attribute("state")?.Value ??
													   throw new ApplicationException(
														   "Expected a state attribute for the Reaction element in WildAnimalHerdAI."));
			if (!Enum.IsDefined(typeof(WildAnimalHerdState), state))
			{
				throw new ApplicationException(
					$"Invalid WildAnimalHerdState value for the Reaction element in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
			}

			try
			{
				switch (sub.Attribute("type")?.Value)
				{
					case "emote":
						_stateReactionDictionary[state] = new WildAnimalHerdEmoteReaction(sub);
						break;
					case "flee":
						_stateReactionDictionary[state] = new WildAnimalHerdFleeReaction(sub, gameworld);
						break;
					case "attack":
						_stateReactionDictionary[state] = new WildAnimalAttackReaction(sub);
						break;
					default:
						throw new ApplicationException("Invalid Reaction type");
				}
			}
			catch (ApplicationException e)
			{
				throw new ApplicationException(
					$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had an exception while loading one of its reactions:\n{e.Message}");
			}
		}

		element = root.Element("RandomEmotes");
		if (element == null)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" did not have an RandomEmotes element.");
		}

		foreach (var sub in element.Elements())
		{
			if (!Enum.TryParse<WildAnimalHerdRole>(sub.Attribute("role")?.Value ?? "", true, out var rvalue))
			{
				throw new ApplicationException(
					$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had an invalid RandomEmote element with a bad role attribute - {sub.Attribute("role")?.Value ?? ""}");
			}

			if (!Enum.TryParse<WildAnimalHerdState>(sub.Attribute("state")?.Value ?? "", true, out var svalue))
			{
				throw new ApplicationException(
					$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had an invalid RandomEmote element with a bad state attribute - {sub.Attribute("state")?.Value ?? ""}");
			}

			if (string.IsNullOrEmpty(sub.Value))
			{
				throw new ApplicationException(
					$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had an invalid RandomEmote element with an empty emote.");
			}

			var emote = new Emote(sub.Value, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
				null);
			if (!emote.Valid)
			{
				throw new ApplicationException(
					$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had an invalid RandomEmote element with an invalid emote: {emote.ErrorMessage}");
			}

			_randomEmoteDictionary.Add((svalue, rvalue), sub.Value);
		}

		element = root.Element("AttackWhenAttackedEmotes");
		if (element == null)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" did not have an AttackWhenAttackedEmotes element.");
		}

		foreach (var sub in element.Elements())
		{
			if (!Enum.TryParse<WildAnimalHerdRole>(sub.Attribute("role")?.Value ?? "", true, out var rvalue))
			{
				throw new ApplicationException(
					$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had an invalid AttackWhenAttackedEmote element with a bad role attribute - {sub.Attribute("role")?.Value ?? ""}");
			}

			if (string.IsNullOrEmpty(sub.Value))
			{
				throw new ApplicationException(
					$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had an invalid AttackWhenAttackedEmote element with an empty emote.");
			}

			var emote = new Emote(sub.Value, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
				new DummyPerceivable());
			if (!emote.Valid)
			{
				throw new ApplicationException(
					$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had an invalid AttackWhenAttackedEmote element with an invalid emote: {emote.ErrorMessage}");
			}

			_attackWhenAttackedEmotes[rvalue] = sub.Value;
		}

		element = root.Element("MinimumRoleCounts");
		if (element == null)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" did not have a MinimumRoleCounts element.");
		}

		foreach (var sub in element.Elements())
		{
			if (!Enum.TryParse<WildAnimalHerdRole>(sub.Attribute("role")?.Value ?? "", true, out var rvalue))
			{
				throw new ApplicationException(
					$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had an invalid MinimumRoleCount element with a bad role attribute - {sub.Attribute("role")?.Value ?? ""}");
			}

			if (!int.TryParse(sub.Value, out var ivalue))
			{
				throw new ApplicationException(
					$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had an invalid MinimumRoleCount element (was expecting count) - {sub.Value}");
			}

			_minimumCountsForEachRole[rvalue] = ivalue;
		}

		element = root.Element("ActiveTimes");
		if (element == null)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" did not have an ActiveTimes element.");
		}

		foreach (var sub in element.Elements())
		{
			if (!Enum.TryParse<TimeOfDay>(sub.Value, true, out var tvalue))
			{
				throw new ApplicationException(
					$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had an invalid ActiveTime element - {sub.Value}");
			}

			if (!_activeTimes.Contains(tvalue))
			{
				_activeTimes.Add(tvalue);
			}
		}

		element = root.Element("RandomEchoChancePerMinute");
		if (element == null)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" did not have a RandomEchoChancePerMinute element.");
		}

		try
		{
			_randomEchoChancePerMinute = double.Parse(element.Value);
		}
		catch (ArgumentNullException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a RandomEchoChancePerMinute element that was empty.");
		}
		catch (FormatException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a RandomEchoChancePerMinute element that was not a number. It should be a number between 0 and 1.");
		}
		catch (OverflowException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a RandomEchoChancePerMinute element that was not a valid number. It should be a number between 0 and 1.");
		}

		element = root.Element("SentryScanChancePerMinute");
		if (element == null)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" did not have a SentryScanChancePerMinute element.");
		}

		try
		{
			_sentryScanChancePerMinute = double.Parse(element.Value);
		}
		catch (ArgumentNullException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a SentryScanChancePerMinute element that was empty.");
		}
		catch (FormatException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a SentryScanChancePerMinute element that was not a number. It should be a number between 0 and 1.");
		}
		catch (OverflowException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a SentryScanChancePerMinute element that was not a valid number. It should be a number between 0 and 1.");
		}

		element = root.Element("FleersWillEngageInCombatIfCornered");
		if (element == null)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" did not have a FleersWillEngageInCombatIfCornered element.");
		}

		try
		{
			_fleersWillEngageInCombatIfCornered = bool.Parse(element.Value);
		}
		catch (ArgumentNullException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a FleersWillEngageInCombatIfCornered element that was empty.");
		}
		catch (FormatException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a FleersWillEngageInCombatIfCornered element that was not a truth value. It should be either true or false.");
		}

		element = root.Element("ThreatAwarenessDistance");
		if (element == null)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" did not have a ThreatAwarenessDistance element.");
		}

		try
		{
			_threatAwarenessDistance = uint.Parse(element.Value);
		}
		catch (ArgumentNullException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a ThreatAwarenessDistance element that was empty.");
		}
		catch (FormatException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a ThreatAwarenessDistance element that was not a number 0 or greater.");
		}
		catch (OverflowException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a ThreatAwarenessDistance element that was not a number 0 or greater.");
		}

		element = root.Element("MinimumDistanceForOutsiders");
		if (element == null)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" did not have a MinimumDistanceForOutsiders element.");
		}

		try
		{
			_minimumDistanceForOutsiders = uint.Parse(element.Value);
		}
		catch (ArgumentNullException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a MinimumDistanceForOutsiders element that was empty.");
		}
		catch (FormatException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a MinimumDistanceForOutsiders element that was not a number 0 or greater.");
		}
		catch (OverflowException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a MinimumDistanceForOutsiders element that was not a number 0 or greater.");
		}

		element = root.Element("MaximumDistanceForOutsiders");
		if (element == null)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" did not have a MaximumDistanceForOutsiders element.");
		}

		try
		{
			_maximumDistanceForOutsiders = uint.Parse(element.Value);
		}
		catch (ArgumentNullException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a MaximumDistanceForOutsiders element that was empty.");
		}
		catch (FormatException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a MaximumDistanceForOutsiders element that was not a number 0 or greater.");
		}
		catch (OverflowException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a MaximumDistanceForOutsiders element that was not a number 0 or greater.");
		}

		element = root.Element("MaximumHerdDispersement");
		if (element == null)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" did not have a MaximumHerdDispersement element.");
		}

		try
		{
			_maximumHerdDispersement = uint.Parse(element.Value);
		}
		catch (ArgumentNullException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a MaximumHerdDispersement element that was empty.");
		}
		catch (FormatException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a MaximumHerdDispersement element that was not a number 0 or greater.");
		}
		catch (OverflowException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a MaximumHerdDispersement element that was not a number 0 or greater.");
		}

		element = root.Element("PositionStateWhenResting");
		if (element == null)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" did not have a PositionStateWhenResting element.");
		}

		try
		{
			_positionStateWhenResting = PositionState.GetState(long.Parse(element.Value));
			if (_positionStateWhenResting == null)
			{
				throw new ApplicationException(
					$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" specified an invalid PositionState in its PositionStateWhenResting element.");
			}
		}
		catch (ArgumentNullException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a PositionStateWhenResting element that was empty.");
		}
		catch (FormatException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a PositionStateWhenResting element that was not a number 0 or greater.");
		}
		catch (OverflowException)
		{
			throw new ApplicationException(
				$"WildAnimalHerdAI {ai.Id} \"{ai.Name}\" had a PositionStateWhenResting element that was not a number 0 or greater.");
		}

		element = root.Element("WillMoveIntoRoomProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"No WillMoveIntoRoomProg element in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		_willMoveIntoRoomProg = long.TryParse(element.Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);
		if (_willMoveIntoRoomProg == null)
		{
			throw new ApplicationException(
				$"WillMoveIntoRoomProg was not found in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		if (!_willMoveIntoRoomProg.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) ||
			!_willMoveIntoRoomProg.MatchesParameters(new[]
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Location,
				ProgVariableTypes.Number
			}))
		{
			throw new ApplicationException(
				$"WillMoveIntoRoomProg was not compatible in WildAnimalHerdAI {ai.Id} \"{ai.Name}\". It must return Boolean and accept Character, Location, Number as parameters.");
		}

		element = root.Element("EscalateThreatProg");
		if (element == null)
		{
			throw new ApplicationException($"No EscalateThreatProg element in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		_escalateThreatProg = long.TryParse(element.Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);
		if (_escalateThreatProg == null)
		{
			throw new ApplicationException(
				$"EscalateThreatProg was not found in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		if (!_escalateThreatProg.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) ||
			!_escalateThreatProg.MatchesParameters(new[]
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Character | ProgVariableTypes.Collection,
				ProgVariableTypes.Character | ProgVariableTypes.Collection,
				ProgVariableTypes.Number
			}))
		{
			throw new ApplicationException(
				$"EscalateThreatProg was not compatible in WildAnimalHerdAI {ai.Id} \"{ai.Name}\". It must return Boolean and accept Character, Character Collection, Character Collection, Number as parameters.");
		}

		element = root.Element("ConsidersThreatProg");
		if (element == null)
		{
			throw new ApplicationException($"No ConsidersThreatProg element in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		_considersThreatProg = long.TryParse(element.Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);
		if (_considersThreatProg == null)
		{
			throw new ApplicationException(
				$"ConsidersThreatProg was not found in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		if (!_considersThreatProg.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) ||
			!_considersThreatProg.MatchesParameters(new[]
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Character
			}))
		{
			throw new ApplicationException(
				$"ConsidersThreatProg was not compatible in WildAnimalHerdAI {ai.Id} \"{ai.Name}\". It must return Boolean and accept Character, Character as parameters.");
		}

		element = root.Element("HerdRoleProg");
		if (element == null)
		{
			throw new ApplicationException($"No HerdRoleProg element in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		_herdRoleProg = long.TryParse(element.Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);
		if (_herdRoleProg == null)
		{
			throw new ApplicationException($"HerdRoleProg was not found in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		if (!_herdRoleProg.ReturnType.CompatibleWith(ProgVariableTypes.Text) || !_herdRoleProg.MatchesParameters(
				new[]
				{
					ProgVariableTypes.Character
				}))
		{
			throw new ApplicationException(
				$"HerdRoleProg was not compatible in WildAnimalHerdAI {ai.Id} \"{ai.Name}\". It must return Text and accept Character as a parameter.");
		}

		element = root.Element("FightOrFlightProg");
		if (element == null)
		{
			throw new ApplicationException($"No FightOrFlightProg element in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		_fightOrFlightProg = long.TryParse(element.Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);
		if (_fightOrFlightProg == null)
		{
			throw new ApplicationException(
				$"FightOrFlightProg was not found in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		if (!_fightOrFlightProg.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) ||
			!_fightOrFlightProg.MatchesParameters(new[]
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Character,
				ProgVariableTypes.Character | ProgVariableTypes.Collection,
				ProgVariableTypes.Character | ProgVariableTypes.Collection,
				ProgVariableTypes.Character
			}))
		{
			throw new ApplicationException(
				$"FightOrFlightProg was not compatible in WildAnimalHerdAI {ai.Id} \"{ai.Name}\". It must return Boolean and accept Character, Character, Character Collection, Character Collection, Character as parameters.");
		}

		element = root.Element("WillMoveCalmProg");
		if (element == null)
		{
			throw new ApplicationException($"No WillMoveCalmProg element in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		_willMoveCalmProg = long.TryParse(element.Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);
		if (_willMoveCalmProg == null)
		{
			throw new ApplicationException($"WillMoveCalmProg was not found in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		if (!_willMoveCalmProg.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) ||
			!_willMoveCalmProg.MatchesParameters(new[]
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Exit
			}))
		{
			throw new ApplicationException(
				$"WillMoveCalmProg was not compatible in WildAnimalHerdAI {ai.Id} \"{ai.Name}\". It must return Boolean and accept Character, Exit as parameters.");
		}

		element = root.Element("WillMoveAgitatedProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"No WillMoveAgitatedProg element in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		_willMoveAgitatedProg = long.TryParse(element.Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);
		if (_willMoveAgitatedProg == null)
		{
			throw new ApplicationException(
				$"WillMoveAgitatedProg was not found in WildAnimalHerdAI {ai.Id} \"{ai.Name}\"");
		}

		if (!_willMoveAgitatedProg.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) ||
			!_willMoveAgitatedProg.MatchesParameters(new[]
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Exit
			}))
		{
			throw new ApplicationException(
				$"WillMoveAgitatedProg was not compatible in WildAnimalHerdAI {ai.Id} \"{ai.Name}\". It must return Boolean and accept Character, Exit as parameters.");
		}
	}

	private WildAnimalHerdAI()
	{

	}

	private WildAnimalHerdAI(IFuturemud gameworld, string name) : base(gameworld, name, "WildAnimalHerd")
	{
		DatabaseInitialise();
	}

	private List<ICharacter> GetStressors(ICharacter character, List<INPC> herd)
	{
		return character.Location.CellsInVicinity(_threatAwarenessDistance, true, false)
						.SelectMany(x => x.Characters)
						.Where(x => ConsidersThreat(character, x) && herd.Any(y => y.CanSee(x)))
						.ToList();
	}

	private List<(ICharacter Character, IEnumerable<ICellExit> Directions)> GetStressorsAndDirections(
		ICharacter character, List<INPC> herd)
	{
		bool EvaluateFunc(ICharacter target)
		{
			return _considersThreatProg.Execute<bool?>(character, target) == true;
		}

		return character
			   .AcquireAllTargetsAndPaths<ICharacter>(EvaluateFunc, _threatAwarenessDistance,
				   PathSearch.RespectClosedDoors)
			   .ToList();
	}

	private List<(INPC Animal, WildAnimalHerdRole Role)> GetHerdRoles(ICharacter character)
	{
		return character.EffectsOfType<WildAnimalHerdEffect>().First().GetHerdMembers();
	}

	private (WildAnimalHerdRole Role, WildAnimalHerdPriority Priority, WildAnimalHerdState State,
		List<(INPC Animal, WildAnimalHerdRole Role)> Herd) GetBasicHerdInfo(ICharacter character)
	{
		var charEffect = character.EffectsOfType<WildAnimalHerdEffect>().First();
		var effect = charEffect.HerdLeaderEffect ?? charEffect;
		return (charEffect.Role, effect.Priority, effect.State, effect.GetHerdMembers());
	}

	private bool IsFighter(WildAnimalHerdRole role, WildAnimalHerdState state)
	{
		switch (state)
		{
			case WildAnimalHerdState.Desperate:
				return true;
			case WildAnimalHerdState.Spooked:
				return role.In(WildAnimalHerdRole.Alpha, WildAnimalHerdRole.Elderly, WildAnimalHerdRole.Follower,
					WildAnimalHerdRole.Outsider, WildAnimalHerdRole.Sentry);
			case WildAnimalHerdState.Alert:
				return role.In(WildAnimalHerdRole.Alpha, WildAnimalHerdRole.Outsider, WildAnimalHerdRole.Sentry);
			default:
				return role.In(WildAnimalHerdRole.Outsider, WildAnimalHerdRole.Sentry);
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var ch =
				type switch
				{
					EventType.CharacterEnterCellFinishWitness => (ICharacter)arguments[3],
					EventType.CharacterBeginMovementWitness => (ICharacter)arguments[3],
					EventType.EngagedInCombatWitness => (ICharacter)arguments[2],
					EventType.EngagedInCombat => (ICharacter)arguments[1],
					_ => arguments[0] as ICharacter,
				}
			;
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		switch (type)
		{
			case EventType.CharacterEnterCellFinish:
				return CharacterEnterCellFinish((ICharacter)arguments[0], (ICell)arguments[1],
					(ICellExit)arguments[2]) || base.HandleEvent(type, arguments);
			case EventType.CharacterEnterCellFinishWitness:
				return CharacterEnterCellFinishWitness((ICharacter)arguments[0], (ICell)arguments[1],
					(ICellExit)arguments[2], (ICharacter)arguments[3]);
			case EventType.CharacterBeginMovementWitness:
				return CharacterBeginMovementWitness((ICharacter)arguments[0], (ICellExit)arguments[2],
					(ICharacter)arguments[3]);
			case EventType.NPCOnGameLoadFinished:
				return NPCLoadedCheck((ICharacter)arguments[0]);
			case EventType.EngagedInCombat:
				return EngagedInCombat((ICharacter)arguments[0], (ICharacter)arguments[1]);
			case EventType.LeaveCombat:
				return LeaveCombat((ICharacter)arguments[0]) || base.HandleEvent(type, arguments);
			case EventType.NoNaturalTargets:
				return NoNaturalTargets((ICharacter)arguments[0]);
			case EventType.MinuteTick:
				return MinuteTick((ICharacter)arguments[0]) || base.HandleEvent(type, arguments);
			case EventType.EngagedInCombatWitness:
				return EngagedInCombatWitness((ICharacter)arguments[0], (ICharacter)arguments[1],
					(ICharacter)arguments[2]);
			default:
				return base.HandleEvent(type, arguments);
		}
	}

	private bool CharacterBeginMovementWitness(ICharacter character, ICellExit exit, ICharacter witness)
	{
		var effect = witness.EffectsOfType<WildAnimalHerdEffect>().FirstOrDefault();
		if (effect == null)
		{
			return false;
		}

		if (character == effect.HerdLeader && witness.CanMove(exit) && witness.Combat == null &&
			witness.Movement == null)
		{
			witness.Move(exit);
			return true;
		}

		var moverEffect = character.EffectsOfType<WildAnimalHerdEffect>().FirstOrDefault();
		if (moverEffect == null)
		{
			return false;
		}

		if (((moverEffect.Role == WildAnimalHerdRole.Child && effect.Role == WildAnimalHerdRole.ChildProtector) ||
			 (moverEffect.Role == WildAnimalHerdRole.ChildProtector && effect.Role == WildAnimalHerdRole.Child))
			&& witness.CanMove(exit) && witness.Combat == null && witness.Movement == null)
		{
			witness.Move(exit);
			return true;
		}

		return false;
	}

	private bool MinuteTick(ICharacter character)
	{
		var (role, priority, state, herd) = GetBasicHerdInfo(character);
		var effect = character.EffectsOfType<WildAnimalHerdEffect>().First().HerdLeaderEffectOrSelf;
		MinuteTickForHerd(effect.HerdLeader, priority, state, herd, effect);

		if (!CharacterState.Able.HasFlag(character.State) || character.Combat != null ||
			character.Movement != null)
		{
			return false;
		}

		var doneSomething = false;
		if (role == WildAnimalHerdRole.Sentry)
		{
			doneSomething = DoMinuteTickSentry(character, priority, state, herd);
		}

		doneSomething |= DoMinuteTickForAnimal(character, role, priority, state, herd);

		if (doneSomething)
		{
			return true;
		}

		if (RandomUtilities.DoubleRandom(0.0, 1.0) <= _randomEchoChancePerMinute)
		{
			var echo = _randomEmoteDictionary[(state, role)].GetRandomElement();
			if (!string.IsNullOrEmpty(echo))
			{
				character.OutputHandler.Handle(new EmoteOutput(new Emote(echo, character, character,
					new PerceivableGroup(herd.Select(x => x.Animal)),
					(IPerceivable)herd.Select(x => x.Animal).Except(character).GetRandomElement() ??
					new DummyPerceivable("a herd member only they can see", location: character.Location))));
				return true;
			}
		}

		return false;
	}

	private bool DoMinuteTickSentry(ICharacter character, WildAnimalHerdPriority priority, WildAnimalHerdState state,
		List<(INPC Animal, WildAnimalHerdRole Role)> herd)
	{
		var multiplier = 1.0;
		switch (state)
		{
			case WildAnimalHerdState.Alert:
				multiplier = 3.0;
				break;
			case WildAnimalHerdState.Spooked:
				multiplier = 6.0;
				break;
			case WildAnimalHerdState.Desperate:
				multiplier = 0.0;
				break;
		}

		if (RandomUtilities.DoubleRandom(0.0, 1.0) <= multiplier * _sentryScanChancePerMinute)
		{
			character.OutOfContextExecuteCommand("longscan");
			return true;
		}

		return false;
	}

	private bool DoMinuteTickForAnimal(ICharacter character, WildAnimalHerdRole role, WildAnimalHerdPriority priority,
		WildAnimalHerdState state, List<(INPC Animal, WildAnimalHerdRole Role)> herd)
	{
		if (character.Combat != null)
		{
			if (priority == WildAnimalHerdPriority.Flee && !IsFighter(role, state))
			{
				character.CombatStrategyMode = Combat.CombatStrategyMode.Flee;
			}

			return false;
		}

		if (character.Movement != null)
		{
			return false;
		}

		if (priority != WildAnimalHerdPriority.Sleep && character.State.HasFlag(CharacterState.Sleeping))
		{
			character.Awaken();
			character.MovePosition(PositionStanding.Instance, PositionModifier.None, null, null, null);
		}

		if (!character.PositionState.Upright && (state >= WildAnimalHerdState.Alert ||
												 !priority.In(WildAnimalHerdPriority.Rest,
													 WildAnimalHerdPriority.Sleep)))
		{
			character.MovePosition(PositionStanding.Instance, null, null);
		}

		bool SuitabilityFunction(ICellExit exit)
		{
			return _willMoveIntoRoomProg?.Execute<bool?>(character, exit.Destination, state) != false &&
				   exit.Exit.Door?.IsOpen != false && character.CanMove(exit);
		}

		var effect = character.EffectsOfType<WildAnimalHerdEffect>().First().HerdLeaderEffectOrSelf;
		if (effect.HerdLeader.Location != character.Location && role != WildAnimalHerdRole.Outsider)
		{
			var pathToLeader = character.PathBetween(effect.HerdLeader, _maximumHerdDispersement, SuitabilityFunction)
										.ToList();
			if (pathToLeader.Any())
			{
				var fp = new FollowingPath(character, pathToLeader);
				character.AddEffect(fp);
				FollowPathAction(character, fp);
				return true;
			}

			return false;
		}

		if (role == WildAnimalHerdRole.Outsider)
		{
			if (character.CanMove(CanMoveFlags.IgnoreCancellableActionBlockers | CanMoveFlags.IgnoreSafeMovement | CanMoveFlags.IgnoreWhetherExitCanBeCrossed))
			{
				var pathToLeader = character
								   .PathBetween(effect.HerdLeader, _maximumHerdDispersement, SuitabilityFunction)
								   .ToList();

				// Even outsiders join the herd to drink
				if (priority == WildAnimalHerdPriority.SeekWater && character.Location != effect.HerdLeader.Location)
				{
					var fp = new FollowingPath(character,
						pathToLeader.Take(pathToLeader.Count - (int)_maximumDistanceForOutsiders).ToList());
					character.AddEffect(fp);
					FollowPathAction(character, fp);
					return true;
				}

				// Move away from herd if too close
				if (pathToLeader.Count < _minimumDistanceForOutsiders)
				{
					var validZone = effect.HerdLeader.Location
										  .CellsAndDistancesInVicinity(_maximumDistanceForOutsiders,
											  SuitabilityFunction,
											  cell => _willMoveIntoRoomProg.Execute<bool?>(character, cell, state) !=
													  false)
										  .Where(x => x.Item2 >= _minimumDistanceForOutsiders)
										  .OrderBy(x => x.Item1.EstimatedDirectDistanceTo(character.Location))
										  .ToList();
					foreach (var cell in validZone)
					{
						var path = character.PathBetween(cell.Cell, _maximumHerdDispersement, SuitabilityFunction)
											.ToList();
						if (path.Any())
						{
							var fp = new FollowingPath(character, path);
							character.AddEffect(fp);
							FollowPathAction(character, fp);
							return true;
						}
					}
				}

				// Move towards herd if too far
				else if (pathToLeader.Count > _maximumDistanceForOutsiders)
				{
					var fp = new FollowingPath(character,
						pathToLeader.Take(pathToLeader.Count - (int)_maximumDistanceForOutsiders).ToList());
					character.AddEffect(fp);
					FollowPathAction(character, fp);
					return true;
				}
			}
		}

		if (IsFighter(role, state) &&
			herd.Any(x => x.Animal.Combat != null && x.Animal.Location == character.Location))
		{
			var herdattacker = herd.Where(x => x.Animal.Location == character.Location)
								   .SelectNotNull(x => x.Animal.Combat)
								   .SelectMany(x => x.Combatants)
								   .Distinct()
								   .Where(character.CanEngage)
								   .GetRandomElement();
			if (herdattacker != null)
			{
				character.Engage(herdattacker);
				return true;
			}
		}

		switch (priority)
		{
			case WildAnimalHerdPriority.SeekWater:
				if (role != WildAnimalHerdRole.Alpha)
				{
					goto case WildAnimalHerdPriority.Graze;
				}

				var lcons = LocalLiquids(character, character.Location);
				if (lcons.Any() && !effect.KnownWater.Contains(character.Location))
				{
					effect.KnownWater.Add(character.Location);
				}

				if (herd.Where(x => x.Animal.Location == character.Location)
						.All(x => !x.Animal.Body.NeedsModel.Status.IsThirsty()))
				{
#if DEBUG
					Gameworld.DebugMessage($"Herd {Name} has finished drinking and is now grazing.");
#endif
					effect.Priority = WildAnimalHerdPriority.Graze;
					return true;
				}

				var (path, target) =
					effect.KnownWater.Select(
							  x => (Path:
								  character.PathBetween(
											   x, _maximumHerdDispersement, SuitabilityFunction)
										   .ToList(), x))
						  .FirstOrDefault(x => x.Path.Count > 0);
				if (target != null)
				{
#if DEBUG
					Gameworld.DebugMessage($"Herd {Name} is headed to known waterhole {target.HowSeen(character)} to find a drink.");
#endif
					var fp = new FollowingPath(character, path);
					character.AddEffect(fp);
					FollowPathAction(character, fp);
					return true;
				}

				// No known water sources, wander randomly
				var recent = character.EffectsOfType<AdjacentToExit>().FirstOrDefault();
				var random = character.Location.ExitsFor(character)
									  .Where(SuitabilityFunction)
									  .GetWeightedRandom(x => recent?.Exit == x ? 1.0 : 100.0);
				if (random != null && character.CanMove(random))
				{
#if DEBUG
					Gameworld.DebugMessage(
						$"Herd {Name} is thirsty but knows of no water. Wandering randomly to {random.Destination.HowSeen(character)}.");
#endif
					character.Move(random);
					return true;
				}

#if DEBUG
				Gameworld.DebugMessage($"Herd {Name} is thirsty but knows of no water, and Alpha cannot move.");
#endif
				break;
			case WildAnimalHerdPriority.Graze:
				var yields = character.Race.EdibleForagableYields
									  .Where(x => character.Location.GetForagableYield(x.YieldType) > 0.0).ToList();
				lcons = LocalLiquids(character, character.Location);
				var lcon = lcons.GetRandomElement();
				if (lcons.Any() && !effect.KnownWater.Contains(character.Location))
				{
					effect.KnownWater.Add(character.Location);
				}

				if (character.NeedsModel.Status.IsThirsty())
				{
					if (lcon != null)
					{
						character.Drink(lcon, null, Sip, null);
						return true;
					}

					var yield = yields.Where(x => x.ThirstPerYield > 0.0).GetRandomElement();
					if (yield != null)
					{
						character.Eat(yield.YieldType, 1.0, null);
						return true;
					}
				}

				if (character.NeedsModel.Status.IsHungry())
				{
					var yield = yields.Where(x => x.HungerPerYield > 0.0).GetRandomElement();
					if (yield != null)
					{
						character.Eat(yield.YieldType, 1.0, null);
						return true;
					}
				}

				if (role == WildAnimalHerdRole.Alpha || role == WildAnimalHerdRole.Outsider)
				{
					if (character.NeedsModel.Status.IsThirsty())
					{
#if DEBUG
						Gameworld.DebugMessage($"Herd {Name} is thirsty but has no water at {character.Location.HowSeen(character)}.");
#endif
						// If we got this far, they wanted to drink but couldn't
						effect.KnownWater.Remove(character.Location);
						if (role == WildAnimalHerdRole.Alpha)
						{
							effect.Priority = WildAnimalHerdPriority.SeekWater;
						}
						else
						{
							(path, target) =
								effect.KnownWater.Select(
										  x => (Path:
											  character.PathBetween(
														   x, _maximumHerdDispersement, SuitabilityFunction)
													   .ToList(), x))
									  .FirstOrDefault(x => x.Path.Count > 0);
							if (target != null)
							{
								var fp = new FollowingPath(character, path);
								character.AddEffect(fp);
								FollowPathAction(character, fp);
								return true;
							}
						}
					}

					if (character.NeedsModel.Status.IsHungry())
					{
#if DEBUG
						Gameworld.DebugMessage($"Herd {Name} is hungry but no food at {character.Location.HowSeen(character)}.");
#endif
						random = character.Location.ExitsFor(character)
										  .Where(x => character.CanMove(x) && SuitabilityFunction(x))
										  .Where(x => character.Race.EdibleForagableYields.Any(
											  y => x.Destination.GetForagableYield(
												  y.YieldType) > 0.0))
										  .GetRandomElement();
						if (random != null && character.CanMove(random))
						{
#if DEBUG
							Gameworld.DebugMessage($"Herd {Name} is wandering to {random.Destination.HowSeen(character)} to graze.");
#endif
							character.Move(random);
							return true;
						}

						recent = character.EffectsOfType<AdjacentToExit>().FirstOrDefault();
						random = character.Location.ExitsFor(character)
										  .Where(SuitabilityFunction)
										  .GetWeightedRandom(x => recent?.Exit == x ? 1.0 : 100.0);
						if (random != null && character.CanMove(random))
						{
#if DEBUG
							Gameworld.DebugMessage($"Herd {Name} is hungry but couldn't find any nearby food. Wandering randomly to {random.Destination.HowSeen(character)}.");
#endif
							character.Move(random);
							return true;
						}
					}
				}

				break;
			case WildAnimalHerdPriority.Sleep:
				if (role == WildAnimalHerdRole.Sentry)
				{
					return false;
				}

				if (Constants.Random.Next(0, 3) > 0)
				{
					return false;
				}

				if (CharacterState.Able.HasFlag(character.State))
				{
					if (character.PositionState.CompareTo(character.Race.MinimumSleepingPosition) ==
						PositionHeightComparison.Higher)
					{
						if (!character.CanMovePosition(character.Race.MinimumSleepingPosition))
						{
							return false;
						}

						character.MovePosition(character.Race.MinimumSleepingPosition, null, null);
					}

					character.Sleep();
					return true;
				}

				break;
			case WildAnimalHerdPriority.Posture:
				// TODO
				break;
			case WildAnimalHerdPriority.Fight:
				// TODO
				break;
			case WildAnimalHerdPriority.Flee:
				// TODO - what should we do?
				break;
		}

		return false;
	}

	private bool MinuteTickForHerd(ICharacter alpha, WildAnimalHerdPriority priority, WildAnimalHerdState state,
		List<(INPC Animal, WildAnimalHerdRole Role)> herd, WildAnimalHerdEffect effect)
	{
		if (DateTime.UtcNow - effect.LastAlphaMinuteTick < TimeSpan.FromSeconds(30))
		{
			return false;
		}

		if (herd.Any(x => x.Animal.Combat != null))
		{
			return false;
		}

		switch (effect.State)
		{
			case WildAnimalHerdState.Alert:
				if (DateTime.UtcNow - effect.LastStateChange > TimeSpan.FromMinutes(5))
				{
					effect.State--;
				}

				break;
			case WildAnimalHerdState.Spooked:
				if (DateTime.UtcNow - effect.LastStateChange > TimeSpan.FromMinutes(20))
				{
					effect.State--;
				}

				break;
			case WildAnimalHerdState.Desperate:
				if (DateTime.UtcNow - effect.LastStateChange > TimeSpan.FromMinutes(120))
				{
					effect.State--;
				}

				break;
			case WildAnimalHerdState.None:
				if (DateTime.UtcNow - effect.LastStateChange > TimeSpan.FromHours(24))
				{
					effect.AvoidedLocations.Clear();
				}

				break;
		}

		state = effect.State;

		if (priority == WildAnimalHerdPriority.Flee && state > WildAnimalHerdState.Alert)
		{
			var potentialThreats = GetStressorsAndDirections(alpha, herd.Select(x => x.Animal).ToList());
			if (!potentialThreats.Any())
			{
				effect.Priority = WildAnimalHerdPriority.Graze;
			}

			ICellExit exitToMove = null;

			var directions = potentialThreats.Select(x => x.Directions)
											 .CountTotalDirections<IEnumerable<IEnumerable<ICellExit>>,
												 IEnumerable<ICellExit>>()
											 .ContainedDirections();
			var potentialExits = alpha.Location.ExitsFor(alpha)
									  .Where(x => _willMoveIntoRoomProg.Execute<bool?>(alpha, x.Destination, state) !=
												  false)
									  .ToList();
			var preferredExits = potentialExits.Where(x => directions.Contains(x.OutboundDirection)).ToList();
			if (preferredExits.Any())
			{
				exitToMove = preferredExits.GetRandomElement();
			}
			else
			{
				exitToMove = potentialExits.GetRandomElement();
			}

			if (exitToMove == null)
			{
				effect.State = WildAnimalHerdState.Desperate;
			}
			else
			{
				effect.DesignatedExits.Add(exitToMove);
			}

			return false;
		}

		// If we've made it this far and we were previously fighting or fleeing, we should clear the old flee exit info
		if (effect.Priority.In(WildAnimalHerdPriority.Fight, WildAnimalHerdPriority.Flee))
		{
			effect.DesignatedExits.Clear();
		}

		var tod = alpha.Location.CurrentTimeOfDay;
		if (_activeTimes.Contains(tod) || effect.State > WildAnimalHerdState.None)
		{
			var localHerd = herd.Where(x => x.Animal.Location == alpha.Location).ToList();
			var hungry = localHerd.Any(x => x.Animal.Body.NeedsModel.Status.IsHungry());
			var thirsty = localHerd.Any(x => x.Animal.Body.NeedsModel.Status.IsThirsty());
			if (hungry || thirsty)
			{
				effect.Priority = WildAnimalHerdPriority.Graze;
			}
			else
			{
				effect.Priority = WildAnimalHerdPriority.Rest;
			}
		}
		else
		{
			effect.Priority = WildAnimalHerdPriority.Sleep;
		}

		return false;
	}

	private bool NoNaturalTargets(ICharacter character)
	{
		if (character.Combat == null)
		{
			return false;
		}

		var (role, priority, state, herd) = GetBasicHerdInfo(character);
		var stressors = GetStressors(character, herd.Select(x => x.Animal).ToList());
		var engagable = stressors.Where(x => character.CanEngage(x)).ToList();
		var fighter = IsFighter(role, state);
		if (fighter)
		{
			if (!engagable.Any())
			{
				var existing = character.Combat.Combatants
										.Where(x => herd.Any(y => x.CombatTarget == y.Animal) && character.CanEngage(x))
										.ToList();
				if (existing.Any())
				{
					character.Engage(existing.GetRandomElement());
					return false;
				}

				character.CombatStrategyMode = Combat.CombatStrategyMode.Flee;
				return false;
			}

			character.Engage(engagable.GetRandomElement());
			return false;
		}
		else
		{
			character.CombatStrategyMode = Combat.CombatStrategyMode.Flee;
			return false;
		}
	}

	private bool EngagedInCombatWitness(ICharacter aggressor, ICharacter target, ICharacter witness)
	{
		var (role, priority, state, herd) = GetBasicHerdInfo(witness);
		if (herd.All(x => x.Animal != target) || herd.Any(x => x.Animal == aggressor))
		{
			return false;
		}

		if (role == WildAnimalHerdRole.Alpha && state <= WildAnimalHerdState.Spooked)
		{
			var effect = witness.EffectsOfType<WildAnimalHerdEffect>().First();
			effect.State = WildAnimalHerdState.Spooked;
			effect.Priority = WildAnimalHerdPriority.Fight;
		}

		var targetRole = herd.First(x => x.Animal == target).Role;
		if (role == WildAnimalHerdRole.ChildProtector && targetRole == WildAnimalHerdRole.Child)
		{
			if (witness.CanEngage(aggressor) && witness.CombatTarget != aggressor)
			{
				witness.OutputHandler.Handle(new EmoteOutput(new Emote(_attackWhenAttackedEmotes[role], witness,
					witness, aggressor, target)));
				witness.Engage(aggressor);
			}

			return false;
		}

		if (IsFighter(role, state) && witness.CombatTarget == null && witness.CanEngage(aggressor))
		{
			witness.OutputHandler.Handle(new EmoteOutput(new Emote(_attackWhenAttackedEmotes[role], witness, witness,
				aggressor, target)));
			witness.Engage(aggressor);
		}

		return false;
	}

	private bool LeaveCombat(ICharacter character)
	{
		var (role, priority, state, herd) = GetBasicHerdInfo(character);
		if (!IsFighter(role, state) || priority != WildAnimalHerdPriority.Fight)
		{
			return false;
		}

		var stressors = GetStressors(character, herd.Select(x => x.Animal).ToList()).Where(x => character.CanEngage(x))
			.ToList();
		if (stressors.Any())
		{
			character.Engage(stressors.GetRandomElement());
		}

		return false;
	}

	private bool EngagedInCombat(ICharacter aggressor, ICharacter character)
	{
		var effect = character.EffectsOfType<WildAnimalHerdEffect>().FirstOrDefault();
		if (effect == null)
		{
			return false;
		}

		var herd = character.Location.Characters.OfType<INPC>().Where(x => x.AIs.Contains(this)).ToList();
		var leader = effect.HerdLeader;
		var stressors = GetStressors(character, herd);
		var roles = GetHerdRoles(character);
		var escapes = character.Location.ExitsFor(character)
							   .Where(x => _willMoveAgitatedProg.Execute<bool?>(character, x) == true).ToList();
		var chosenEscape = escapes.GetRandomElement();

		void TryEngageInCombat(ICharacter animal)
		{
			if (animal.Combat != null)
			{
				return;
			}

			if (animal.CanEngage(aggressor) && animal.Engage(aggressor))
			{
				return;
			}

			foreach (var target in stressors)
			{
				if (animal.CanEngage(target) && animal.Engage(target))
				{
					return;
				}
			}
		}

		// Fight reaction
		if (_fightOrFlightProg.Execute<bool?>(leader, character, herd, stressors, aggressor) == true)
		{
			var fighters = roles.Where(x => IsFighter(x.Role, effect.State)).ToList();
			foreach (var fighter in fighters)
			{
				TryEngageInCombat(fighter.Animal);
			}

			var fleers = roles.Except(fighters);
			// ChosenEscape can only be null if there is no valid escape
			if (chosenEscape == null)
			{
				foreach (var fleer in fleers)
				{
					TryEngageInCombat(fleer.Animal);
				}
			}
			else
			{
				foreach (var fleer in fleers)
				{
					if (fleer.Animal.Movement != null)
					{
						continue;
					}

					if (fleer.Animal.CanMove(chosenEscape))
					{
						fleer.Animal.Move(chosenEscape);
						continue;
					}

					TryEngageInCombat(fleer.Animal);
				}
			}
		}
		// Flight reaction
		else
		{
			foreach (var fleer in roles)
			{
				if (fleer.Animal.Movement != null)
				{
					continue;
				}

				if (fleer.Animal.CanMove(chosenEscape))
				{
					fleer.Animal.Move(chosenEscape);
					continue;
				}

				if (!_fleersWillEngageInCombatIfCornered)
				{
					continue;
				}

				TryEngageInCombat(fleer.Animal);
			}
		}

		return true;
	}

	private bool NPCLoadedCheck(ICharacter character)
	{
		DoCheckForHerdRoles(character);
		return false;
	}

	private bool CharacterEnterCellFinishWitness(ICharacter mover, ICell cell, ICellExit cellExit, ICharacter character)
	{
		var effect = character.EffectsOfType<WildAnimalHerdEffect>().FirstOrDefault();
		if (effect == null)
		{
			return false;
		}

		if (effect.EntryReactionCooldown > DateTime.UtcNow)
		{
			return false;
		}

		if (!effect.Role.In(WildAnimalHerdRole.Outsider, WildAnimalHerdRole.Alpha))
		{
			return false;
		}

		if (!ConsidersThreat(character, mover))
		{
			return false;
		}

		var herd = cell.Location.Characters.OfType<INPC>().Where(x => x.AIs.Contains(this)).ToList();
		var stressors = GetStressors(character, herd);

		if (EscalateThreat(character, herd, stressors, effect.State))
		{
			effect.State = (WildAnimalHerdState)((int)effect.State + 1);
			effect.EntryReactionCooldown = DateTime.UtcNow + TimeSpan.FromSeconds(10);
		}

		_stateReactionDictionary[effect.State]?.DoReaction(character,
			effect.GetHerdMembers().Where(x => x.Animal.Location == character.Location).ToList(), stressors, mover);
		return true;
	}

	private bool CharacterEnterCellFinish(ICharacter character, ICell cell, ICellExit cellExit)
	{
		var effect = character.EffectsOfType<WildAnimalHerdEffect>().FirstOrDefault();
		if (effect?.Role.In(WildAnimalHerdRole.Outsider, WildAnimalHerdRole.Alpha) != true)
		{
			return false;
		}

		var herd = cell.Location.Characters.OfType<INPC>().Where(x => x.AIs.Contains(this)).ToList();
		var stressors = GetStressors(character, herd);

		if (EscalateThreat(character, herd, stressors, effect.State))
		{
			effect.State++;
		}

		_stateReactionDictionary[effect.State]
			?.DoReaction(character, effect.GetHerdMembers(), stressors, stressors.GetRandomElement());
		return true;
	}

	private bool EscalateThreat(ICharacter character, List<INPC> herd, List<ICharacter> stressors,
		WildAnimalHerdState current)
	{
		return _escalateThreatProg.Execute<bool?>(character, herd, stressors, current) ?? false;
	}

	private WildAnimalHerdRole StringToRole(string text)
	{
		switch (text?.ToLowerInvariant())
		{
			case "alpha":
				return WildAnimalHerdRole.Alpha;
			case "follower":
				return WildAnimalHerdRole.Follower;
			case "child":
				return WildAnimalHerdRole.Child;
			case "child protector":
				return WildAnimalHerdRole.ChildProtector;
			case "outsider":
				return WildAnimalHerdRole.Outsider;
			case "elderly":
				return WildAnimalHerdRole.Elderly;
			case "sentry":
				return WildAnimalHerdRole.Sentry;
			default:
				return WildAnimalHerdRole.Follower;
		}
	}

	private void DoCheckForHerdRoles(ICharacter character)
	{
		var herdExtent = character
						 .CellsInVicinity(_maximumHerdDispersement, false, false).ToList();
		var potentialHerd = herdExtent
							.SelectMany(x => x.Characters.OfType<INPC>().Where(y => y.AIs.Contains(this)))
							.Select(x => (Animal: x, SuggestedRole: StringToRole(_herdRoleProg.Execute<string>(x))))
							.ToList();

		WildAnimalHerdEffect leadereffect;

		if (potentialHerd.Any(x => x.Animal.AffectedBy<WildAnimalHerdEffect>()))
		{
			leadereffect = potentialHerd.Select(x => x.Animal.EffectsOfType<WildAnimalHerdEffect>().FirstOrDefault())
										.First(x => x != null);
			// Join the existing herd instead of creating a new one
			foreach (var (animal, suggestedRole) in potentialHerd)
			{
				if (animal.EffectsOfType<WildAnimalHerdEffect>().Any())
				{
					continue;
				}

				if (suggestedRole == WildAnimalHerdRole.Alpha)
				{
					animal.AddEffect(new WildAnimalHerdEffect(animal, leadereffect.HerdLeader, leadereffect)
					{
						Role = WildAnimalHerdRole.Follower // Only one alpha per herd
					});
				}
				else
				{
					animal.AddEffect(new WildAnimalHerdEffect(animal, leadereffect.HerdLeader, leadereffect)
					{
						Role = suggestedRole
					});
				}
			}

			return;
		}

		// Check to see there is only one alpha
		var alphaFound = false;
		INPC leader = null;
		for (var i = 0; i < potentialHerd.Count; i++)
		{
			if (potentialHerd[i].SuggestedRole == WildAnimalHerdRole.Alpha)
			{
				if (alphaFound)
				{
					potentialHerd[i] = (potentialHerd[i].Animal, WildAnimalHerdRole.Follower);
				}
				else
				{
					leader = potentialHerd[i].Animal;
				}

				alphaFound = true;
			}
		}

		// Check to see there is at least one alpha
		if (!alphaFound)
		{
			var randomIndex = Dice.Roll(1, potentialHerd.Count) - 1;
			leader = potentialHerd[randomIndex].Animal;
			potentialHerd[randomIndex] = (leader, WildAnimalHerdRole.Alpha);
		}

		foreach (var entry in _minimumCountsForEachRole.Where(x => x.Value > 0))
		{
			var actual = potentialHerd.Count(x => x.SuggestedRole == entry.Key);
			for (var i = 0; i < actual - entry.Value; i++)
			{
				var random = potentialHerd.Where(x => x.SuggestedRole == WildAnimalHerdRole.Follower)
										  .GetRandomElement();
				if (random == default)
				{
					break;
				}

				potentialHerd[potentialHerd.IndexOf(random)] = (random.Animal, entry.Key);
			}
		}

		leadereffect = new WildAnimalHerdEffect(leader, leader, null)
		{
			Role = WildAnimalHerdRole.Alpha,
			State = WildAnimalHerdState.None,
			Priority = WildAnimalHerdPriority.Graze
		};
		leader.AddEffect(leadereffect);

		foreach (var (animal, suggestedRole) in potentialHerd)
		{
			if (animal == leader)
			{
				continue;
			}

			animal.AddEffect(new WildAnimalHerdEffect(animal, leader, leadereffect)
			{
				Role = suggestedRole
			});
		}

		// At load time, we'll assume they know about their local area
		foreach (var cell in herdExtent)
		{
			if (LocalLiquids(character, cell).Any())
			{
				leadereffect.KnownWater.Add(cell);
			}
		}
	}

	private List<ILiquidContainer> LocalLiquids(ICharacter character, ICell location)
	{
		return location.LayerGameItems(character.RoomLayer).SelectNotNull(x => x.GetItemType<ILiquidContainer>())
					   .Where(x => x.LiquidMixture.Instances.Sum(
									   y => y.Liquid.DrinkSatiatedHoursPerLitre) > 0 &&
								   character.Body.CanDrink(x, null, Sip)).ToList();
	}

	private double _sip;

	protected double Sip
	{
		get
		{
			if (_sip <= 0.0)
			{
				_sip = Gameworld.GetStaticDouble("DefaultSipAmount");
			}

			return _sip;
		}
	}

	private bool ConsidersThreat(ICharacter animal, ICharacter potentialThreat)
	{
		return _considersThreatProg.Execute<bool?>(animal, potentialThreat) ?? false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		return types.Any(type =>
		{
			switch (type)
			{
				case EventType.CharacterEnterCellFinish:
				case EventType.CharacterEnterCellFinishWitness:
				case EventType.CharacterBeginMovementWitness:
				case EventType.NPCOnGameLoadFinished:
				case EventType.EngagedInCombat:
				case EventType.LeaveCombat:
				case EventType.NoNaturalTargets:
				case EventType.MinuteTick:
				case EventType.EngagedInCombatWitness:
					return true;
				default:
					return base.HandlesEvent(types);
			}
		});
	}

	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		// This AI always supplies its own paths through other means
		return (null, Enumerable.Empty<ICellExit>());
	}
}