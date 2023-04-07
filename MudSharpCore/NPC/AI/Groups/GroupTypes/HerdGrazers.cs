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
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.NPC.AI.Groups.GroupTypes;

public abstract class HerdGrazers : GroupAIType
{
	protected HerdGrazers(Gender dominantGender, IEnumerable<TimeOfDay> activeTimesOfDay, IFuturemud gameworld) : base(
		dominantGender, activeTimesOfDay, gameworld)
	{
	}

	protected HerdGrazers(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
	}

	protected class HerdGrazerData : BaseGroupTypeData
	{
		public List<IRace> UntrustedRaces { get; } = new();
		public ICharacter AppointedSentry { get; set; }

		public HerdGrazerData(IFuturemud gameworld) : base(gameworld)
		{
		}

		public HerdGrazerData(XElement root, IFuturemud gameworld) : base(root, gameworld)
		{
			foreach (var item in root.Element("UntrustedRaces").Elements())
			{
				var race = gameworld.Races.Get(long.Parse(item.Value));
				if (race == null)
				{
					continue;
				}

				UntrustedRaces.Add(race);
			}
		}

		public override XElement SaveToXml()
		{
			var item = base.SaveToXml();
			item.Add(new XElement("UntrustedRaces", from race in UntrustedRaces select new XElement("Race", race.Id)));
			return item;
		}

		public override string ShowText(ICharacter voyeur)
		{
			var sb = new StringBuilder();
			sb.Append(base.ShowText(voyeur));
			sb.AppendLine(
				$"Untrusted Races: {UntrustedRaces.Select(x => x.Name.ColourValue()).ListToCommaSeparatedValues()}");
			sb.AppendLine($"Appointed Sentry: {AppointedSentry?.HowSeen(voyeur) ?? "None".Colour(Telnet.Red)}");
			return sb.ToString();
		}
	}

	protected abstract void EvaluateAlertLevel(IGroupAI group);

	public sealed override void HandleTenSecondTick(IGroupAI group)
	{
		base.HandleTenSecondTick(group);
		EvaluateAlertLevel(group);
		GatherStragglers(group);
		HandleGeneral(group);
		var data = (HerdGrazerData)group.Data;
		switch (group.CurrentAction)
		{
			case GroupAction.Graze:
				HandleGraze(group);
				return;
			case GroupAction.FindFood:
				HandleFindFood(group);
				return;
			case GroupAction.FindWater:
				HandleFindWater(group);
				return;
			case GroupAction.Rest:
				HandleRest(group);
				return;
			case GroupAction.Sleep:
				HandleSleep(group);
				return;
			default:
				if (HandleTenSecondAction(group, data, group.CurrentAction))
				{
					return;
				}

				break;
		}

		throw new ApplicationException(
			$"HerdGrazer GroupAIType was asked to handle a CurrentAction that a derived class should have handled: {group.CurrentAction.DescribeEnum()}");
	}

	protected abstract bool HandleTenSecondAction(IGroupAI group, HerdGrazerData data, GroupAction action);

	protected (IEnumerable<ICharacter> MainGroup, IEnumerable<ICharacter> Stragglers, IEnumerable<ICharacter> Outsiders)
		GetGroups(IGroupAI group)
	{
		if (!group.GroupRoles.Any(x => x.Value == GroupRole.Leader))
		{
			EnsureLeaderExists(group);
		}

		var main = new List<ICharacter>();
		var stragglers = new List<ICharacter>();
		var outsiders = new List<ICharacter>();
		var data = (HerdGrazerData)group.Data;

		if (group.GroupRoles.Any(x => x.Value == GroupRole.Leader))
		{
			var leader = group.GroupRoles.First(x => x.Value == GroupRole.Leader).Key;
			foreach (var ch in group.GroupMembers)
				// Outsiders don't count as outsiders when they're thirsty
			{
				if (group.GroupRoles[ch] == GroupRole.Outsider && (!ch.NeedsModel.Status.IsThirsty() ||
				                                                   !data.KnownWaterLocations.Contains(leader.Location)))
				{
					outsiders.Add(ch);
				}
				else if (ch.Location == leader.Location && ch.RoomLayer == leader.RoomLayer)
				{
					main.Add(ch);
				}
				else
				{
					stragglers.Add(ch);
				}
			}

			return (main, stragglers, outsiders);
		}

		var mainLocation = group.GroupMembers.GroupBy(x => (Location: x.Location, Layer: x.RoomLayer))
		                        .Select(x => (x.Key, Count: x.Count())).FirstMax(x => x.Count).Key;
		if (mainLocation.Location == null)
		{
			return (Enumerable.Empty<ICharacter>(), Enumerable.Empty<ICharacter>(), Enumerable.Empty<ICharacter>());
		}

		foreach (var ch in group.GroupMembers)
		{
			if (group.GroupRoles[ch] == GroupRole.Outsider)
			{
				outsiders.Add(ch);
			}
			else if (ch.Location == mainLocation.Location && ch.RoomLayer == mainLocation.Layer)
			{
				main.Add(ch);
			}
			else
			{
				stragglers.Add(ch);
			}
		}

		return (main, stragglers, outsiders);
	}

	private void GatherStragglers(IGroupAI group)
	{
		if (!group.GroupMembers.Any())
		{
			return;
		}

		var (main, stragglers, outsiders) = GetGroups(group);
		if (!stragglers.Any() && !outsiders.Any())
		{
			return;
		}

		var targetLocation = (main.First().Location, main.First().RoomLayer);
		foreach (var ch in stragglers)
		{
			if (!ch.CouldMove(false, null).Success)
			{
				continue;
			}

			if (ch.AffectedBy<FollowingPath>())
			{
				ch.CombinedEffectsOfType<FollowingPath>().First().FollowPathAction();
				continue;
			}

			if (ch.Location == targetLocation.Location)
			{
				PathIndividualToLayer(ch, targetLocation.RoomLayer);
				continue;
			}

			PathIndividualToLocation(ch, group, targetLocation.Location);
		}

		foreach (var ch in outsiders)
		{
			var leaderPath = ch.PathBetween(targetLocation.Location, 20, PathSearch.PathRespectClosedDoors(ch))
			                   .ToList();
			if (leaderPath.Count < 2)
			{
				var validZone = targetLocation.Location
				                              .CellsAndDistancesInVicinity(6,
					                              exit => ch.CouldMove(false, null).Success && ch.CanMove(exit),
					                              cell => !group.AvoidCell(cell, group.Alertness))
				                              .Where(x => x.Distance >= 3)
				                              .OrderBy(x => x.Cell.EstimatedDirectDistanceTo(ch.Location))
				                              .ToList();
				foreach (var cell in validZone)
				{
					var path = ch.PathBetween(cell.Cell, 6,
						exit => ch.CouldMove(false, null).Success && ch.CanMove(exit)).ToList();
					if (path.Any())
					{
						var fp = new FollowingPath(ch, path);
						ch.AddEffect(fp);
						fp.FollowPathAction();
						break;
					}
				}
			}
			else if (leaderPath.Count > 6)
			{
				var fp = new FollowingPath(ch, leaderPath.Take(leaderPath.Count - 6).ToList());
				ch.AddEffect(fp);
				fp.FollowPathAction();
			}
		}
	}

	private void CheckForSentryAppointment(IGroupAI group, HerdGrazerData data)
	{
		if (data.AppointedSentry == null || IgnoreTickAI(data.AppointedSentry))
		{
			data.AppointedSentry = GetGroups(group).MainGroup
			                                       .Where(x => group.GroupRoles[x] != GroupRole.Child &&
			                                                   !IgnoreTickAI(x)).GetRandomElement();
		}
	}

	protected override void HandleGeneral(IGroupAI group)
	{
		var data = (HerdGrazerData)group.Data;
		CheckForSentryAppointment(group, data);
		base.HandleGeneral(group);
	}

	private void HandleRest(IGroupAI group)
	{
		var (main, _, outsiders) = GetGroups(group);
		var data = (HerdGrazerData)group.Data;

		foreach (var ch in main.Concat(outsiders))
		{
			if (IgnoreTickAI(ch) || data.AppointedSentry == ch || !ch.PositionState.Upright)
			{
				continue;
			}

			if (Constants.Random.Next(0, 15) > 0)
			{
				continue;
			}

			if (ch.PositionState.CompareTo(ch.Race.MinimumSleepingPosition) == PositionHeightComparison.Higher)
			{
				if (!ch.CanMovePosition(ch.Race.MinimumSleepingPosition))
				{
					continue;
				}

				ch.MovePosition(ch.Race.MinimumSleepingPosition, null, null);
				continue;
			}
		}
	}

	private void HandleSleep(IGroupAI group)
	{
		var (main, _, outsiders) = GetGroups(group);
		var data = (HerdGrazerData)group.Data;

		foreach (var ch in main.Concat(outsiders))
		{
			if (IgnoreTickAI(ch) || ch == data.AppointedSentry)
			{
				continue;
			}

			if (ch.State.IsAsleep())
			{
				continue;
			}

			if (Constants.Random.Next(0, 8) > 0)
			{
				continue;
			}

			if (ch.PositionState.CompareTo(ch.Race.MinimumSleepingPosition) == PositionHeightComparison.Higher)
			{
				if (!ch.CanMovePosition(ch.Race.MinimumSleepingPosition))
				{
					continue;
				}

				ch.MovePosition(ch.Race.MinimumSleepingPosition, null, null);
				continue;
			}

			ch.Sleep();
		}
	}

	private void HandleFindWater(IGroupAI group)
	{
		var (main, _, outsiders) = GetGroups(group);
		var data = (HerdGrazerData)group.Data;
		if (!main.Any())
		{
			return;
		}

		var races = main.Concat(outsiders).Select(x => x.Race).Distinct();
		var locations = main.Concat(outsiders).Select(x => (Location: x.Location, Layer: x.RoomLayer)).Distinct();
		var localYields =
			new CollectionDictionary<(ICell Location, RoomLayer Layer, IRace Race), EdibleForagableYield>();
		var lcons = new CollectionDictionary<(ICell Location, RoomLayer Layer), ILiquidContainer>();
		var mainLocation = main.First().Location;
		var leader = main.FirstOrDefault(x => group.GroupRoles[x] == GroupRole.Leader) ?? main.First();

		foreach (var location in locations)
		{
			foreach (var race in races)
			{
				localYields.AddRange((location.Location, location.Layer, race),
					race.EdibleForagableYields.Where(x => location.Location.GetForagableYield(x.YieldType) > 0));
			}

			lcons.AddRange((location.Location, location.Layer), LocalLiquids(location.Location, location.Layer));
		}

		foreach (var location in lcons)
		{
			if (location.Value.Any() && !data.KnownWaterLocations.Contains(location.Key.Location))
			{
				data.KnownWaterLocations.Add(location.Key.Location);
				group.Changed = true;
			}
			else if (!location.Value.Any())
			{
				data.KnownWaterLocations.Remove(location.Key.Location);
				group.Changed = true;
			}
		}

		if (lcons[(mainLocation, leader.RoomLayer)].Any())
		{
			group.CurrentAction = GroupAction.Graze;
			return;
		}

		foreach (var water in data.KnownWaterLocations)
		{
			var path = mainLocation.PathBetween(water, 20, CanMoveExitFunctionFor(leader, group));
			if (!path.Any())
			{
				continue;
			}

			var fp = new FollowingPath(leader, path);
			leader.AddEffect(fp);
			fp.FollowPathAction();
			return;
		}

		// No known water sources, wander randomly
		if (leader.CouldMove(false, null).Success)
		{
			var recent = leader.EffectsOfType<AdjacentToExit>().FirstOrDefault();
			var random = mainLocation.ExitsFor(leader)
			                         .Where(x => CanMoveExitFunctionFor(leader, group).Invoke(x))
			                         .GetWeightedRandom(x => recent?.Exit == x ? 1.0 : 100.0);
			if (random != null && leader.CanMove(random))
			{
				leader.Move(random);
			}
		}
	}

	private void HandleFindFood(IGroupAI group)
	{
		var (main, _, _) = GetGroups(group);
		var data = (HerdGrazerData)group.Data;
		if (!main.Any())
		{
			return;
		}

		var mainLocation = main.First().Location;
		var leader = main.FirstOrDefault(x => group.GroupRoles[x] == GroupRole.Leader) ?? main.First();

		var races = main.Select(x => x.Race).Distinct();
		var yields = new CollectionDictionary<IRace, EdibleForagableYield>();
		foreach (var race in races)
		{
			yields.AddRange(race,
				race.EdibleForagableYields.Where(x =>
					mainLocation.GetForagableYield(x.YieldType) >= race.BiteYield(x.YieldType)));
		}

		if (yields.Any(x => x.Value.Count > 0))
		{
			group.CurrentAction = GroupAction.Graze;
			return;
		}

		// No food here, wander randomly
		if (leader.CouldMove(false, null).Success)
		{
			var recent = leader.EffectsOfType<AdjacentToExit>().FirstOrDefault();
			var random = mainLocation.ExitsFor(leader)
			                         .Where(x => CanMoveExitFunctionFor(leader, group).Invoke(x) && leader.CanMove(x))
			                         .GetWeightedRandom(x => recent?.Exit == x ? 1.0 : 100.0);
			if (random != null)
			{
				leader.Move(random);
			}
		}
	}

	private void HandleGraze(IGroupAI group)
	{
		var (main, _, outsiders) = GetGroups(group);
		var data = (HerdGrazerData)group.Data;

		var races = main.Concat(outsiders).Select(x => x.Race).Distinct();
		var locations = main.Concat(outsiders).Select(x => (Location: x.Location, Layer: x.RoomLayer)).Distinct();
		var localYields =
			new CollectionDictionary<(ICell Location, RoomLayer Layer, IRace Race), EdibleForagableYield>();
		var lcons = new CollectionDictionary<(ICell Location, RoomLayer Layer), ILiquidContainer>();

		foreach (var location in locations)
		{
			foreach (var race in races)
			{
				localYields.AddRange((location.Location, location.Layer, race),
					race.EdibleForagableYields.Where(x => location.Location.GetForagableYield(x.YieldType) > 0));
			}

			lcons.AddRange((location.Location, location.Layer), LocalLiquids(location.Location, location.Layer));
		}

		foreach (var location in lcons)
		{
			if (location.Value.Any() && !data.KnownWaterLocations.Contains(location.Key.Location))
			{
				data.KnownWaterLocations.Add(location.Key.Location);
				group.Changed = true;
			}
			else if (!location.Value.Any())
			{
				data.KnownWaterLocations.Remove(location.Key.Location);
				group.Changed = true;
			}
		}

		var unsatisfiedThirst = false;
		var unsatisfiedHunger = false;

		foreach (var ch in main.Concat(outsiders))
		{
			if (Constants.Random.Next(0, 12) > 0)
			{
				continue;
			}

			if (ch.NeedsModel.Status.IsThirsty())
			{
				var lcon = lcons[(ch.Location, ch.RoomLayer)].GetRandomElement();
				if (lcon != null)
				{
					ch.SetTarget(lcon.Parent);
					ch.SetModifier(PositionModifier.None);
					ch.SetEmote(null);
					ch.Drink(lcon, null, ch.Gameworld.GetStaticDouble("DefaultAnimalDrinkAmount"), null);
					continue;
				}

				var yield = localYields[(ch.Location, ch.RoomLayer, ch.Race)].Where(x =>
					x.ThirstPerYield > 0.0 &&
					ch.Location.GetForagableYield(x.YieldType) >= ch.Race.BiteYield(x.YieldType)).GetRandomElement();
				if (yield != null)
				{
					ch.Eat(yield.YieldType, 1.0, null);
					continue;
				}

				unsatisfiedThirst = true;
			}

			if (ch.NeedsModel.Status.IsHungry())
			{
				var yield = localYields[(ch.Location, ch.RoomLayer, ch.Race)].Where(x =>
					x.HungerPerYield > 0.0 &&
					ch.Location.GetForagableYield(x.YieldType) >= ch.Race.BiteYield(x.YieldType)).GetRandomElement();
				if (yield != null)
				{
					ch.Eat(yield.YieldType, 1.0, null);
					continue;
				}

				unsatisfiedHunger = true;
			}
		}

		if (unsatisfiedThirst)
		{
			group.CurrentAction = GroupAction.FindWater;
			group.Changed = true;
		}
		else if (unsatisfiedHunger)
		{
			group.CurrentAction = GroupAction.FindFood;
			group.Changed = true;
		}
	}

	public override void HandleMinuteTick(IGroupAI group)
	{
		var data = (HerdGrazerData)group.Data;
		EstablishPriorities(group, data);
		CheckForSentryAppointment(group, data);
		HandleSentryMinuteTick(group, data);
	}

	protected void EstablishPriorities(IGroupAI group, HerdGrazerData data)
	{
		var (main, stragglers, outsiders) = GetGroups(group);
		if (!main.Any())
		{
			return;
		}

		PruneStaleThreatLocations(group, data);

		if (EstablishPrioritiesHandled(group, data, main, stragglers, outsiders))
		{
			return;
		}

		var time = main.First().Location.CurrentTimeOfDay;
		var active = ActiveTimesOfDay.Contains(time);
		var leader = main.FirstOrDefault(x => group.GroupRoles[x] == GroupRole.Leader) ?? main.GetRandomElement();

		switch (group.CurrentAction)
		{
			case GroupAction.FindWater:
				break;
			case GroupAction.FindFood:
			case GroupAction.Graze:
				if (active || Constants.Random.Next(0, 4) > 0)
				{
					return;
				}

				group.CurrentAction = GroupAction.Rest;
				group.Changed = true;
				break;
			case GroupAction.Sleep:
				if (!active)
				{
					return;
				}

				group.CurrentAction = GroupAction.Rest;
				group.Changed = true;
				return;
			case GroupAction.Rest:
				if (!active)
				{
					if (Constants.Random.Next(0, 12) == 0)
					{
						group.CurrentAction = GroupAction.Sleep;
						group.Changed = true;
					}

					return;
				}

				if (Constants.Random.Next(0, 10) > 0)
				{
					return;
				}

				group.CurrentAction = GroupAction.Graze;
				group.Changed = true;
				return;
			default:
				// If we got here, we got an action that the AI isn't set up to handle. Re-establish our priorities.
				group.CurrentAction = GroupAction.Graze;
				group.Changed = true;
				return;
		}
	}

	protected abstract bool EstablishPrioritiesHandled(IGroupAI group, HerdGrazerData data,
		IEnumerable<ICharacter> main, IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> outsiders);

	private void HandleSentryMinuteTick(IGroupAI group, HerdGrazerData data)
	{
		if (data.AppointedSentry == null || IgnoreTickAI(data.AppointedSentry))
		{
			return;
		}

		var sentry = data.AppointedSentry;
		sentry.OutOfContextExecuteCommand("qs");
		sentry.AddEffect(
			new DelayedAction(sentry, perc => sentry.OutOfContextExecuteCommand("scan"), "Sentry Scanning"),
			TimeSpan.FromSeconds(Constants.Random.Next(1, 30)));
	}

	public override IGroupTypeData LoadData(XElement root, IFuturemud gameworld)
	{
		return new HerdGrazerData(root, gameworld);
	}

	public override IGroupTypeData GetInitialData(IFuturemud gameworld)
	{
		return new HerdGrazerData(gameworld);
	}

	protected void PruneStaleThreatLocations(IGroupAI group, HerdGrazerData data)
	{
		foreach (var location in data.KnownThreatLocations.ToList())
		{
			if (DateTime.UtcNow - location.Value > TimeSpan.FromHours(12))
			{
				data.KnownThreatLocations.Remove(location.Key);
				group.Changed = true;
			}
		}
	}
}