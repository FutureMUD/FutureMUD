using MudSharp.Body.Needs;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            foreach (XElement item in root.Element("UntrustedRaces").Elements())
            {
                IRace race = gameworld.Races.Get(long.Parse(item.Value));
                if (race == null)
                {
                    continue;
                }

                UntrustedRaces.Add(race);
            }
        }

        public override XElement SaveToXml()
        {
            XElement item = base.SaveToXml();
            item.Add(new XElement("UntrustedRaces", from race in UntrustedRaces select new XElement("Race", race.Id)));
            return item;
        }

        public override string ShowText(ICharacter voyeur)
        {
            StringBuilder sb = new();
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
        HerdGrazerData data = (HerdGrazerData)group.Data;
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

        List<ICharacter> main = new();
        List<ICharacter> stragglers = new();
        List<ICharacter> outsiders = new();
        HerdGrazerData data = (HerdGrazerData)group.Data;

        if (group.GroupRoles.Any(x => x.Value == GroupRole.Leader))
        {
            ICharacter leader = group.GroupRoles.First(x => x.Value == GroupRole.Leader).Key;
            foreach (ICharacter ch in group.GroupMembers)
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

        (ICell Location, RoomLayer Layer) mainLocation = group.GroupMembers.GroupBy(x => (Location: x.Location, Layer: x.RoomLayer))
                                .Select(x => (x.Key, Count: x.Count())).FirstMax(x => x.Count).Key;
        if (mainLocation.Location == null)
        {
            return (Enumerable.Empty<ICharacter>(), Enumerable.Empty<ICharacter>(), Enumerable.Empty<ICharacter>());
        }

        foreach (ICharacter ch in group.GroupMembers)
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

        (IEnumerable<ICharacter> main, IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> outsiders) = GetGroups(group);
        if (!stragglers.Any() && !outsiders.Any())
        {
            return;
        }

        (ICell Location, RoomLayer RoomLayer) targetLocation = (main.First().Location, main.First().RoomLayer);
        foreach (ICharacter ch in stragglers)
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

        foreach (ICharacter ch in outsiders)
        {
            List<ICellExit> leaderPath = ch.PathBetween(targetLocation.Location, 20, PathSearch.PathRespectClosedDoors(ch))
                               .ToList();
            if (leaderPath.Count < 2)
            {
                List<(ICell Cell, int Distance)> validZone = targetLocation.Location
                                              .CellsAndDistancesInVicinity(6,
                                                  exit => ch.CouldMove(false, null).Success && ch.CanMove(exit),
                                                  cell => !group.AvoidCell(cell, group.Alertness))
                                              .Where(x => x.Distance >= 3)
                                              .OrderBy(x => x.Cell.EstimatedDirectDistanceTo(ch.Location))
                                              .ToList();
                foreach ((ICell Cell, int Distance) cell in validZone)
                {
                    List<ICellExit> path = ch.PathBetween(cell.Cell, 6,
                        exit => ch.CouldMove(false, null).Success && ch.CanMove(exit)).ToList();
                    if (path.Any())
                    {
                        FollowingPath fp = new(ch, path);
                        ch.AddEffect(fp);
                        fp.FollowPathAction();
                        break;
                    }
                }
            }
            else if (leaderPath.Count > 6)
            {
                FollowingPath fp = new(ch, leaderPath.Take(leaderPath.Count - 6).ToList());
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
        HerdGrazerData data = (HerdGrazerData)group.Data;
        CheckForSentryAppointment(group, data);
        base.HandleGeneral(group);
    }

    private void HandleRest(IGroupAI group)
    {
        (IEnumerable<ICharacter> main, IEnumerable<ICharacter> _, IEnumerable<ICharacter> outsiders) = GetGroups(group);
        HerdGrazerData data = (HerdGrazerData)group.Data;

        foreach (ICharacter ch in main.Concat(outsiders))
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
        (IEnumerable<ICharacter> main, IEnumerable<ICharacter> _, IEnumerable<ICharacter> outsiders) = GetGroups(group);
        HerdGrazerData data = (HerdGrazerData)group.Data;

        foreach (ICharacter ch in main.Concat(outsiders))
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
        (IEnumerable<ICharacter> main, IEnumerable<ICharacter> _, IEnumerable<ICharacter> outsiders) = GetGroups(group);
        HerdGrazerData data = (HerdGrazerData)group.Data;
        if (!main.Any())
        {
            return;
        }

        IEnumerable<IRace> races = main.Concat(outsiders).Select(x => x.Race).Distinct();
        IEnumerable<(ICell Location, RoomLayer Layer)> locations = main.Concat(outsiders).Select(x => (Location: x.Location, Layer: x.RoomLayer)).Distinct();
        CollectionDictionary<(ICell Location, RoomLayer Layer, IRace Race), EdibleForagableYield> localYields =
            new();
        CollectionDictionary<(ICell Location, RoomLayer Layer), ILiquidContainer> lcons = new();
        ICell mainLocation = main.First().Location;
        ICharacter leader = main.FirstOrDefault(x => group.GroupRoles[x] == GroupRole.Leader) ?? main.First();

        foreach ((ICell Location, RoomLayer Layer) location in locations)
        {
            foreach (IRace race in races)
            {
                localYields.AddRange((location.Location, location.Layer, race),
                    race.EdibleForagableYields.Where(x => location.Location.GetForagableYield(x.YieldType) > 0));
            }

            lcons.AddRange((location.Location, location.Layer), LocalLiquids(location.Location, location.Layer));
        }

        foreach (KeyValuePair<(ICell Location, RoomLayer Layer), List<ILiquidContainer>> location in lcons)
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

        foreach (ICell water in data.KnownWaterLocations)
        {
            IEnumerable<ICellExit> path = mainLocation.PathBetween(water, 20, CanMoveExitFunctionFor(leader, group));
            if (!path.Any())
            {
                continue;
            }

            FollowingPath fp = new(leader, path);
            leader.AddEffect(fp);
            fp.FollowPathAction();
            return;
        }

        // No known water sources, wander randomly
        if (leader.CouldMove(false, null).Success)
        {
            AdjacentToExit recent = leader.EffectsOfType<AdjacentToExit>().FirstOrDefault();
            ICellExit random = mainLocation.ExitsFor(leader)
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
        (IEnumerable<ICharacter> main, IEnumerable<ICharacter> _, IEnumerable<ICharacter> _) = GetGroups(group);
        HerdGrazerData data = (HerdGrazerData)group.Data;
        if (!main.Any())
        {
            return;
        }

        ICell mainLocation = main.First().Location;
        ICharacter leader = main.FirstOrDefault(x => group.GroupRoles[x] == GroupRole.Leader) ?? main.First();

        IEnumerable<IRace> races = main.Select(x => x.Race).Distinct();
        CollectionDictionary<IRace, EdibleForagableYield> yields = new();
        foreach (IRace race in races)
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
            AdjacentToExit recent = leader.EffectsOfType<AdjacentToExit>().FirstOrDefault();
            ICellExit random = mainLocation.ExitsFor(leader)
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
        (IEnumerable<ICharacter> main, IEnumerable<ICharacter> _, IEnumerable<ICharacter> outsiders) = GetGroups(group);
        HerdGrazerData data = (HerdGrazerData)group.Data;

        IEnumerable<IRace> races = main.Concat(outsiders).Select(x => x.Race).Distinct();
        IEnumerable<(ICell Location, RoomLayer Layer)> locations = main.Concat(outsiders).Select(x => (Location: x.Location, Layer: x.RoomLayer)).Distinct();
        CollectionDictionary<(ICell Location, RoomLayer Layer, IRace Race), EdibleForagableYield> localYields =
            new();
        CollectionDictionary<(ICell Location, RoomLayer Layer), ILiquidContainer> lcons = new();

        foreach ((ICell Location, RoomLayer Layer) location in locations)
        {
            foreach (IRace race in races)
            {
                localYields.AddRange((location.Location, location.Layer, race),
                    race.EdibleForagableYields.Where(x => location.Location.GetForagableYield(x.YieldType) > 0));
            }

            lcons.AddRange((location.Location, location.Layer), LocalLiquids(location.Location, location.Layer));
        }

        foreach (KeyValuePair<(ICell Location, RoomLayer Layer), List<ILiquidContainer>> location in lcons)
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

        bool unsatisfiedThirst = false;
        bool unsatisfiedHunger = false;

        foreach (ICharacter ch in main.Concat(outsiders))
        {
            if (Constants.Random.Next(0, 12) > 0)
            {
                continue;
            }

            if (ch.NeedsModel.Status.IsThirsty())
            {
                ILiquidContainer lcon = lcons[(ch.Location, ch.RoomLayer)].GetRandomElement();
                if (lcon != null)
                {
                    ch.SetTarget(lcon.Parent);
                    ch.SetModifier(PositionModifier.None);
                    ch.SetEmote(null);
                    ch.Drink(lcon, null, ch.Gameworld.GetStaticDouble("DefaultAnimalDrinkAmount"), null);
                    continue;
                }

                EdibleForagableYield yield = localYields[(ch.Location, ch.RoomLayer, ch.Race)].Where(x =>
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
                EdibleForagableYield yield = localYields[(ch.Location, ch.RoomLayer, ch.Race)].Where(x =>
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
        HerdGrazerData data = (HerdGrazerData)group.Data;
        EstablishPriorities(group, data);
        CheckForSentryAppointment(group, data);
        HandleSentryMinuteTick(group, data);
    }

    protected void EstablishPriorities(IGroupAI group, HerdGrazerData data)
    {
        (IEnumerable<ICharacter> main, IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> outsiders) = GetGroups(group);
        if (!main.Any())
        {
            return;
        }

        PruneStaleThreatLocations(group, data);

        if (EstablishPrioritiesHandled(group, data, main, stragglers, outsiders))
        {
            return;
        }

        TimeOfDay time = main.First().Location.CurrentTimeOfDay;
        bool active = ActiveTimesOfDay.Contains(time);
        ICharacter leader = main.FirstOrDefault(x => group.GroupRoles[x] == GroupRole.Leader) ?? main.GetRandomElement();

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

        ICharacter sentry = data.AppointedSentry;
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
        foreach (KeyValuePair<ICell, DateTime> location in data.KnownThreatLocations.ToList())
        {
            if (DateTime.UtcNow - location.Value > TimeSpan.FromHours(12))
            {
                data.KnownThreatLocations.Remove(location.Key);
                group.Changed = true;
            }
        }
    }
}