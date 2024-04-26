using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.NPC.AI.Groups.GroupTypes;

public class NeutralHerdGrazers : HerdGrazers
{
	public static void RegisterGroupAIType()
	{
		GroupAITypeFactory.RegisterGroupAIType("neutralherdgrazer", DatabaseLoader, BuilderLoader);
	}

	private static IGroupAIType DatabaseLoader(XElement root, IFuturemud gameworld)
	{
		return new NeutralHerdGrazers(root, gameworld);
	}

	private static (IGroupAIType Type, string Error) BuilderLoader(string builderArgs, IFuturemud gameworld)
	{
		var ss = new StringStack(builderArgs);
		if (ss.IsFinished)
		{
			return (null, "You must supply a dominant gender.");
		}

		if (!Utilities.TryParseEnum<Gender>(ss.PopSpeech(), out var gender))
		{
			return (null, $"The supplied value '{ss.Last}' is not a valid gender.");
		}

		if (ss.IsFinished || !ss.PopSpeech().TryParsePercentage(out var confidence))
		{
			return (null,
				"You must supply a percentage confidence level that determines how often they will posture versus avoid threats.");
		}

		var (success, error, activeTimes) = ParseBuilderArgument(ss.PopSpeech().ToLowerInvariant());
		if (!success)
		{
			return (null, error);
		}

		return (new NeutralHerdGrazers(gender, activeTimes, confidence, gameworld), string.Empty);
	}

	protected class NeutralHerdGrazerData : HerdGrazerData
	{
		public DateTime? LastAdultDeath { get; set; }

		public bool ChildAttacked { get; set; }

		public NeutralHerdGrazerData(IFuturemud gameworld) : base(gameworld)
		{
		}

		public NeutralHerdGrazerData(XElement root, IFuturemud gameworld) : base(root, gameworld)
		{
			var ladText = root.Element("LastAdultDeath")?.Value;
			if (!string.IsNullOrEmpty(ladText))
			{
				LastAdultDeath = DateTime.Parse(ladText, CultureInfo.InvariantCulture);
			}

			ChildAttacked = bool.Parse(root.Element("ChildAttacked")?.Value ?? "false");
		}
	}

	public NeutralHerdGrazers(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
		Confidence = double.Parse(root.Element("Confidence").Value);
	}

	public NeutralHerdGrazers(Gender dominanentGender, IEnumerable<TimeOfDay> activeTimesOfDay, double confidence,
		IFuturemud gameworld) : base(dominanentGender, activeTimesOfDay, gameworld)
	{
		Confidence = confidence;
	}

	public override IGroupTypeData LoadData(XElement root, IFuturemud gameworld)
	{
		return new NeutralHerdGrazerData(root, gameworld);
	}

	public override IGroupTypeData GetInitialData(IFuturemud gameworld)
	{
		return new NeutralHerdGrazerData(gameworld);
	}

	public double Confidence { get; protected init; }

	public override string Name
	{
		get
		{
			if (DominantGender == Gender.Indeterminate)
			{
				return $"Egalitarian {GroupActivityTimeDescription} Neutral Grazers";
			}

			return $"{DominantGender.DescribeEnum()}-Dominant {GroupActivityTimeDescription} Neutral Grazers";
		}
	}

	public override XElement SaveToXml()
	{
		return new XElement("GroupType",
			new XAttribute("typename", "neutralherdgrazer"),
			new XElement("ActiveTimes",
				from time in ActiveTimesOfDay
				select new XElement("Time", (int)time)
			),
			new XElement("Confidence", Confidence),
			new XElement("Gender", (short)DominantGender)
		);
	}

	protected override bool HandleTenSecondAction(IGroupAI group, HerdGrazerData data, GroupAction action)
	{
		var (main, stragglers, outsiders) = GetGroups(group);
		switch (action)
		{
			case GroupAction.AvoidThreat:
				HandleAvoidThreat(group, data, GetThreats(group, data), main, stragglers, outsiders);
				return true;
			case GroupAction.Flee:
				HandleFlee(group, data, GetThreats(group, data), main, stragglers, outsiders);
				return true;
			case GroupAction.Posture:
				HandlePosture(group, data, GetThreats(group, data), main, stragglers, outsiders);
				return true;
			case GroupAction.ControlledRetreat:
				HandleControlledRetreat(group, data, GetThreats(group, data), main, stragglers, outsiders);
				return true;
			case GroupAction.AttackThreats:
				HandleAttackThreats(group, data, GetThreats(group, data), main, stragglers, outsiders);
				return true;
		}

		return false;
	}

	private void HandleAttackThreats(IGroupAI group, HerdGrazerData data, List<ICharacter> list,
		IEnumerable<ICharacter> main, IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> outsiders)
	{
		var ndata = (NeutralHerdGrazerData)data;
		var fighters = main.Where(x => group.GroupRoles[x] != GroupRole.Child).ToList();
		var threats = GetThreats(group, data);
		foreach (var fighter in fighters)
		{
			if (fighter.Combat == null || fighter.CombatTarget == null)
			{
				foreach (var threat in threats.Shuffle())
				{
					if (!fighter.CanSee(threat))
					{
						continue;
					}

					if (fighter.CanEngage(threat))
					{
						fighter.Engage(threat);
						continue;
					}
				}
			}
		}
	}

	private void HandleControlledRetreat(IGroupAI group, HerdGrazerData data, List<ICharacter> list,
		IEnumerable<ICharacter> main, IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> outsiders)
	{
		var ndata = (NeutralHerdGrazerData)data;
		var fighters = main.Where(x => group.GroupRoles[x] == GroupRole.Elder).ToList();
		var nonFighters = main.Except(fighters).ToList();
		var threats = GetThreats(group, data);
		foreach (var fighter in fighters)
		{
			if (fighter.Combat == null || fighter.CombatTarget == null)
			{
				foreach (var threat in threats.Shuffle())
				{
					if (!fighter.CanSee(threat))
					{
						continue;
					}

					if (fighter.CanEngage(threat))
					{
						fighter.Engage(threat);
						continue;
					}
				}
			}
		}

		if (nonFighters.Any())
		{
			HandleAvoidThreatForSubgroup(group, data, threats, nonFighters,
				nonFighters.FirstOrDefault(x => group.GroupRoles[x] == GroupRole.Leader) ??
				nonFighters.GetRandomElement());
		}
	}

	private void HandlePosture(IGroupAI group, HerdGrazerData data, List<ICharacter> list, IEnumerable<ICharacter> main,
		IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> outsiders)
	{
		var emotes = group.GroupEmotes.Where(x => x.Applies(group)).ToList();
		if (!emotes.Any() || Dice.Roll(1, 6) != 1)
		{
			return;
		}

		var threats = GetThreats(group, data);

		var emote = emotes.GetRandomElement();
		var emoter = main.Where(x =>
			x.State.IsAble() && threats.Any(y => y.ColocatedWith(x)) &&
			(!emote.RequiredRole.HasValue || group.GroupRoles[x] == emote.RequiredRole)).GetRandomElement();
		emoter.OutputHandler.Handle(new EmoteOutput(new Emote(emote.EmoteText, emoter, emoter,
			threats.Where(x => x.ColocatedWith(emoter)).GetRandomElement())));
	}

	protected bool ShouldHerdBreak(IGroupAI group, IEnumerable<ICharacter> main, IEnumerable<ICharacter> threats)
	{
		var data = (NeutralHerdGrazerData)group.Data;
		if (data.LastAdultDeath.HasValue &&
		    DateTime.UtcNow - data.LastAdultDeath.Value < TimeSpan.FromMinutes(10))
		{
			return true;
		}

		return false;
	}

	protected override void EvaluateAlertLevel(IGroupAI group)
	{
		// Neutral Herd Grazers are threatened by anything that is from an untrusted race
		var data = (NeutralHerdGrazerData)group.Data;
		var (main, _, outsiders) = GetGroups(group);

		if (!main.Any())
		{
			return;
		}

		if (group.GroupMembers.Any(x => x.Combat != null))
		{
			foreach (var race in group.GroupMembers
			                          .SelectMany(x => x.Combat?.Combatants ?? Enumerable.Empty<IPerceiver>())
			                          .OfType<ICharacter>()
			                          .Except(group.GroupMembers).Select(x => x.Race).Distinct())
			{
				if (!data.UntrustedRaces.Contains(race))
				{
					data.UntrustedRaces.Add(race);
					group.Changed = true;
				}
			}
		}

		if (group.Alertness < GroupAlertness.VeryAgitated &&
		    group.GroupMembers.Any(x => x.Combat != null && !group.GroupMembers.Contains(x.CombatTarget)))
		{
			group.Alertness = GroupAlertness.VeryAgitated;
			group.Changed = true;
			EstablishPriorities(group, data);
			return;
		}

		var threats = GetThreats(group, data);
		if (threats.Any())
		{
			switch (group.Alertness)
			{
				case GroupAlertness.NotAlert:
					group.Alertness = GroupAlertness.Wary;
					group.Changed = true;
					break;
				case GroupAlertness.Wary:
					if (Constants.Random.Next(0, 100) > threats.Count)
					{
						return;
					}

					group.Alertness = GroupAlertness.Agitated;
					group.Changed = true;
					break;
				case GroupAlertness.Agitated:
					if (!group.CurrentAction.In(GroupAction.Posture, GroupAction.AvoidThreat))
					{
						return;
					}

					if (Constants.Random.Next(0, 100) > threats.Count)
					{
						return;
					}

					group.Alertness = GroupAlertness.VeryAgitated;
					group.Changed = true;
					break;
				case GroupAlertness.VeryAgitated:
					if (!group.CurrentAction.In(GroupAction.Posture, GroupAction.AvoidThreat))
					{
						return;
					}

					if (Constants.Random.Next(0, 100) > threats.Count)
					{
						return;
					}

					group.Alertness = GroupAlertness.Aggressive;
					group.Changed = true;
					break;
				case GroupAlertness.Aggressive:
					if (!ShouldHerdBreak(group, main, threats))
					{
						break;
					}

					group.Alertness = GroupAlertness.Broken;
					group.Changed = true;
					break;
				case GroupAlertness.Broken:
					break;
			}

			return;
		}

		switch (group.Alertness)
		{
			case GroupAlertness.NotAlert:
				break;
			case GroupAlertness.Wary:
				if (Constants.Random.Next(0, 120) > main.Count())
				{
					return;
				}

				group.Alertness = GroupAlertness.NotAlert;
				group.Changed = true;
				break;
			case GroupAlertness.Agitated:
				if (Constants.Random.Next(0, 120) > main.Count())
				{
					return;
				}

				group.Alertness = GroupAlertness.Wary;
				group.Changed = true;
				break;
			case GroupAlertness.VeryAgitated:
				if (Constants.Random.Next(0, 120) > main.Count())
				{
					return;
				}

				group.Alertness = GroupAlertness.Agitated;
				group.Changed = true;
				break;
			case GroupAlertness.Aggressive:
				if (Constants.Random.Next(0, 60) > main.Count())
				{
					return;
				}

				group.Alertness = GroupAlertness.VeryAgitated;
				group.Changed = true;
				break;
			case GroupAlertness.Broken:
				group.Alertness = GroupAlertness.Aggressive;
				group.Changed = true;
				break;
		}
	}

	protected override bool EstablishPrioritiesHandled(IGroupAI group, HerdGrazerData data,
		IEnumerable<ICharacter> main, IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> outsiders)
	{
		if (group.GroupMembers.Any(x => x.Combat != null))
		{
			foreach (var race in group.GroupMembers
			                          .SelectMany(x => x.Combat?.Combatants ?? Enumerable.Empty<IPerceiver>())
			                          .OfType<ICharacter>()
			                          .Except(group.GroupMembers).Select(x => x.Race).Distinct())
			{
				if (!data.UntrustedRaces.Contains(race))
				{
					data.UntrustedRaces.Add(race);
					group.Changed = true;
				}
			}
		}

		var threats = GetThreats(group, data);

		if (threats.Any() && group.Alertness > GroupAlertness.Agitated)
		{
			foreach (var location in threats.Select(x => x.Location).Distinct())
			{
				data.KnownThreatLocations[location] = DateTime.UtcNow;
				group.Changed = true;
			}
		}

		if (group.GroupMembers.Any(x => x.Combat != null))
		{
			if (ShouldHerdBreak(group, main, threats))
			{
				if (!main.Any(x => group.GroupRoles[x] == GroupRole.Child))
				{
					group.CurrentAction = GroupAction.Flee;
				}
				else
				{
					group.CurrentAction = GroupAction.ControlledRetreat;
				}

				group.Changed = true;
				return true;
			}

			group.CurrentAction = GroupAction.AttackThreats;
			group.Changed = true;
			return true;
		}

		if (threats.Any() && group.Alertness >= GroupAlertness.Agitated && !group.CurrentAction.In(
			    GroupAction.AvoidThreat, GroupAction.Flee, GroupAction.Posture, GroupAction.AttackThreats,
			    GroupAction.ControlledRetreat))
		{
			var fighters = main.Where(x => group.GroupRoles[x] != GroupRole.Child).ToList();
			if (fighters.Count / threats.Count >= 3 && RandomUtilities.Roll(1.0, Confidence))
			{
				group.CurrentAction = GroupAction.Posture;
			}
			else
			{
				group.CurrentAction = GroupAction.AvoidThreat;
			}

			group.Changed = true;
			return true;
		}

		if (group.Alertness >= GroupAlertness.VeryAgitated &&
		    group.CurrentAction.In(GroupAction.AvoidThreat, GroupAction.Posture))
		{
			if (ShouldHerdBreak(group, main, threats))
			{
				if (!main.Any(x => group.GroupRoles[x] == GroupRole.Child))
				{
					group.CurrentAction = GroupAction.Flee;
				}
				else
				{
					group.CurrentAction = GroupAction.ControlledRetreat;
				}

				group.Changed = true;
				return true;
			}

			group.CurrentAction = GroupAction.AttackThreats;
			group.Changed = true;
			return true;
		}

		if (group.Alertness <= GroupAlertness.Wary && group.CurrentAction.In(GroupAction.AvoidThreat, GroupAction.Flee,
			    GroupAction.Posture, GroupAction.AttackThreats, GroupAction.ControlledRetreat))
		{
			group.CurrentAction = GroupAction.Graze;
			group.Changed = true;
			return true;
		}


		return false;
	}

	private void HandleAvoidThreat(IGroupAI group, HerdGrazerData data, List<ICharacter> threats,
		IEnumerable<ICharacter> main, IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> outsiders)
	{
		if (!threats.Any())
		{
			group.CurrentAction = GroupAction.Graze;
			group.Changed = true;
			return;
		}

		var leader = main.FirstOrDefault(x => group.GroupRoles[x] == GroupRole.Leader) ?? main.FirstOrDefault();
		HandleAvoidThreatForSubgroup(group, data, threats, main, leader);
		foreach (var outsidergroup in outsiders.GroupBy(x => (x.Location, x.RoomLayer)))
		{
			HandleAvoidThreatForSubgroup(group, data, threats, outsidergroup, outsidergroup.GetRandomElement());
		}
	}

	private void HandleAvoidThreatForSubgroup(IGroupAI group, HerdGrazerData data, IEnumerable<ICharacter> threats,
		IEnumerable<ICharacter> characters, ICharacter leader)
	{
		var threatPaths = threats
		                  .Select(x => (Character: x, Path: leader.PathBetween(x, 5, PathSearch.RespectClosedDoors)))
		                  .ToList();
		var threatDirections = threatPaths.Select(x => x.Path)
		                                  .CountTotalDirections<IEnumerable<IEnumerable<ICellExit>>,
			                                  IEnumerable<ICellExit>>().ContainedDirections();
		var allExitsIncludingLayers = leader.Location
		                                    .ExitsFor(leader, true)
		                                    .Where(x => !x.MovementTransition(leader).TransitionType.In(
			                                    CellMovementTransition.FlyOnly,
			                                    CellMovementTransition.NoViableTransition))
		                                    .ToList();
		var allExits = leader.Location
		                     .ExitsFor(leader)
		                     .Where(x => !x.MovementTransition(leader).TransitionType.In(CellMovementTransition.FlyOnly,
			                     CellMovementTransition.NoViableTransition))
		                     .ToList();
		var preferredExits = allExits
		                     .Where(x =>
			                     !threatDirections.Contains(x.OutboundDirection) &&
			                     !group.AvoidCell(x.Destination, group.Alertness) &&
			                     !x.MovementTransition(leader).TransitionType
			                       .In(CellMovementTransition.FallExit, CellMovementTransition.SwimOnly)
		                     )
		                     .ToList();

		if (preferredExits.Any())
		{
			var targetExit = preferredExits.GetRandomElement();
			foreach (var ch in characters)
			{
				if (!ch.CanMove(targetExit, false, true))
				{
					continue;
				}

				ch.Move(targetExit, null, true);
			}

			return;
		}

		if (allExits.Any())
		{
			var targetExit = allExits.GetRandomElement();
			foreach (var ch in characters)
			{
				if (!ch.CanMove(targetExit, false, true))
				{
					continue;
				}

				ch.Move(targetExit, null, true);
			}

			return;
		}

		if (allExitsIncludingLayers.Any())
		{
			var targetLayer = allExitsIncludingLayers.SelectMany(x => x.WhichLayersExitAppears()).Distinct()
			                                         .ClosestLayer(leader.RoomLayer);
			foreach (var ch in characters)
			{
				PathIndividualToLayer(ch, targetLayer);
			}
		}
	}

	private void HandleFlee(IGroupAI group, HerdGrazerData data, List<ICharacter> threats, IEnumerable<ICharacter> main,
		IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> outsiders)
	{
		if (!threats.Any())
		{
			group.CurrentAction = GroupAction.Graze;
			group.Changed = true;
			return;
		}

		foreach (var ch in group.GroupMembers)
		{
			ch.CombatStrategyMode = Combat.CombatStrategyMode.Flee;
		}

		HandleAvoidThreat(group, data, threats, main, stragglers, outsiders);
	}

	protected List<ICharacter> GetThreats(IGroupAI group, HerdGrazerData data)
	{
		var knownCharacters = group.GroupMembers
		                           .SelectMany(x =>
			                           x.SeenTargets.Concat(x.Location.LayerCharacters(x.RoomLayer))
			                            .Where(y => x.CanSee(y)))
		                           .Distinct().OfType<ICharacter>().Except(group.GroupMembers).ToList();
		var threats = knownCharacters.Where(x =>
			data.UntrustedRaces.Any(y => x.Race.SameRace(y)) || group.ConsidersThreat(x, group.Alertness)).ToList();
		return threats;
	}
}