using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Movement;

namespace MudSharp.NPC.AI.Groups.GroupTypes;

public class WimpyHerdGrazers : HerdGrazers
{
	public static void RegisterGroupAIType()
	{
		GroupAITypeFactory.RegisterGroupAIType("wimpyherdgrazer", DatabaseLoader, BuilderLoader);
	}

	private static IGroupAIType DatabaseLoader(XElement root, IFuturemud gameworld)
	{
		return new WimpyHerdGrazers(root, gameworld);
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

		var (success, error, activeTimes) = ParseBuilderArgument(ss.PopSpeech().ToLowerInvariant());
		if (!success)
		{
			return (null, error);
		}

		return (new WimpyHerdGrazers(gender, activeTimes, gameworld), string.Empty);
	}

	public override XElement SaveToXml()
	{
		return new XElement("GroupType",
			new XAttribute("typename", "wimpyherdgrazer"),
			new XElement("ActiveTimes",
				from time in ActiveTimesOfDay
				select new XElement("Time", (int)time)
			),
			new XElement("Gender", (short)DominantGender)
		);
	}

	protected WimpyHerdGrazers(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
	}

	protected WimpyHerdGrazers(Gender dominantGender, IEnumerable<TimeOfDay> activeTimesOfDay, IFuturemud gameworld) :
		base(dominantGender, activeTimesOfDay, gameworld)
	{
	}

	protected override void EvaluateAlertLevel(IGroupAI group)
	{
		// Wimpy Herd Grazers are threatened by anything that is from an untrusted race
		var data = (HerdGrazerData)group.Data;
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
					if (Constants.Random.Next(0, 100) > threats.Count)
					{
						return;
					}

					group.Alertness = GroupAlertness.VeryAgitated;
					group.Changed = true;
					break;
				case GroupAlertness.VeryAgitated:
					if (Constants.Random.Next(0, 100) > threats.Count)
					{
						return;
					}

					group.Alertness = GroupAlertness.Broken;
					group.Changed = true;
					break;
				case GroupAlertness.Aggressive:
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
				group.Alertness = GroupAlertness.VeryAgitated;
				group.Changed = true;
				break;
			case GroupAlertness.Broken:
				group.Alertness = GroupAlertness.VeryAgitated;
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

			group.CurrentAction = GroupAction.Flee;
			group.Changed = true;
			return true;
		}

		if (group.Alertness > GroupAlertness.Wary && !group.CurrentAction.In(GroupAction.AvoidThreat, GroupAction.Flee))
		{
			group.CurrentAction = GroupAction.AvoidThreat;
			group.Changed = true;
			return true;
		}

		if (group.Alertness <= GroupAlertness.Wary && group.CurrentAction.In(GroupAction.AvoidThreat, GroupAction.Flee))
		{
			group.CurrentAction = GroupAction.Graze;
			group.Changed = true;
			return true;
		}

		var threats = GetThreats(group, data);
		if (!threats.Any())
		{
			return false;
		}

		if (group.Alertness > GroupAlertness.Agitated)
		{
			foreach (var location in threats.Select(x => x.Location).Distinct())
			{
				data.KnownThreatLocations[location] = DateTime.UtcNow;
				group.Changed = true;
			}
		}

		group.CurrentAction = GroupAction.AvoidThreat;
		group.Changed = true;
		return true;
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
				if (!ch.CanMove(targetExit, CanMoveFlags.IgnoreCancellableActionBlockers | CanMoveFlags.IgnoreSafeMovement))
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
				if (!ch.CanMove(targetExit, CanMoveFlags.IgnoreCancellableActionBlockers | CanMoveFlags.IgnoreSafeMovement))
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

	public override string Name
	{
		get
		{
			if (DominantGender == Gender.Indeterminate)
			{
				return $"Egalitarian {GroupActivityTimeDescription} Wimpy Grazers";
			}

			return $"{DominantGender.DescribeEnum()}-Dominant {GroupActivityTimeDescription} Wimpy Grazers";
		}
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
		}

		return false;
	}
}