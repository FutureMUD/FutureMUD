using MudSharp.Body.Needs;
using MudSharp.Body.Position;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.NPC.AI.Groups.GroupTypes;

public abstract class PredatorGroupBase : GroupAIType
{
    /// <inheritdoc />
    protected PredatorGroupBase(Gender dominantGender, IEnumerable<TimeOfDay> activeTimes, IFuturemud gameworld) : base(
        dominantGender, activeTimes, gameworld)
    {
    }

    /// <inheritdoc />
    protected PredatorGroupBase(XElement root, IFuturemud gameworld) : base(root, gameworld)
    {
    }

    /// <inheritdoc />
    public override void HandleTenSecondTick(IGroupAI group)
    {
        base.HandleTenSecondTick(group);
        if (!group.GroupMembers.Any())
        {
            return;
        }

        PredatorGroupData data = (PredatorGroupData)group.Data;
        EvaluateAlertLevel(group, data);
        GatherStragglers(group, data);
        HandleGeneral(group, data);

        switch (group.CurrentAction)
        {
            // For predators, graze means eat what is present
            case GroupAction.Graze:
                HandleGraze(group, data);
                return;
            case GroupAction.FindFood:
                HandleFindFood(group, data);
                return;
            case GroupAction.FindWater:
                HandleFindWater(group, data);
                return;
            case GroupAction.Rest:
                HandleRest(group, data);
                return;
            case GroupAction.Sleep:
                HandleSleep(group, data);
                return;
            default:
                if (HandleTenSecondAction(group, data, group.CurrentAction))
                {
                    return;
                }

                break;
        }

        throw new ApplicationException(
            $"PredatorGroup GroupAIType was asked to handle a CurrentAction that a derived class should have handled: {group.CurrentAction.DescribeEnum()}");
    }

    protected abstract bool HandleTenSecondAction(IGroupAI group, PredatorGroupData data,
        GroupAction groupCurrentAction);

    protected virtual void HandleSleep(IGroupAI group, PredatorGroupData data)
    {
        KeyValuePair<ICharacter, GroupRole> leader = group.GroupRoles.First(x => x.Value == GroupRole.Leader);
        foreach (ICharacter ch in group.GroupMembers)
        {
            if (IgnoreTickAI(ch))
            {
                continue;
            }

            if (ch.State.IsAsleep())
            {
                continue;
            }

            if (!ch.ColocatedWith(leader.Key))
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

    protected virtual void HandleRest(IGroupAI group, PredatorGroupData data)
    {
        KeyValuePair<ICharacter, GroupRole> leader = group.GroupRoles.First(x => x.Value == GroupRole.Leader);
        foreach (ICharacter ch in group.GroupMembers)
        {
            if (IgnoreTickAI(ch))
            {
                continue;
            }

            if (ch.State.IsAsleep())
            {
                continue;
            }

            if (!ch.ColocatedWith(leader.Key))
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

    protected virtual void HandleFindWater(IGroupAI group, PredatorGroupData data)
    {
        ICharacter leader = group.GroupRoles.First(x => x.Value == GroupRole.Leader).Key;
        List<ICharacter> main = group.GroupMembers.Where(x => x.ColocatedWith(leader)).ToList();
        if (!main.Any())
        {
            return;
        }

        CollectionDictionary<(ICell Location, RoomLayer Layer), ILiquidContainer> lcons = new();
        ICell mainLocation = leader.Location;

        lcons.AddRange((leader.Location, leader.RoomLayer), LocalLiquids(leader.Location, leader.RoomLayer));

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
            List<ICellExit> path = mainLocation.PathBetween(water, 20, CanMoveExitFunctionFor(leader, group)).ToList();
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

    protected abstract void HandleFindFood(IGroupAI group, PredatorGroupData data);

    protected virtual void HandleGraze(IGroupAI group, PredatorGroupData data)
    {
        IEnumerable<IRace> races = group.GroupMembers.Select(x => x.Race).Distinct();
        IEnumerable<(ICell Location, RoomLayer Layer)> locations = group.GroupMembers.Select(x => (Location: x.Location, Layer: x.RoomLayer)).Distinct();
        CollectionDictionary<(ICell Location, RoomLayer Layer), ILiquidContainer> lcons = new();
        CollectionDictionary<(ICell Location, RoomLayer Layer), ICorpse> corpses = new();
        CollectionDictionary<(ICell Location, RoomLayer Layer), ISeveredBodypart> bodyparts = new();
        CollectionDictionary<(ICell Location, RoomLayer Layer), IEdible> foodItems = new();

        foreach ((ICell Location, RoomLayer Layer) location in locations)
        {
            List<IGameItem> edibleCorpsesAndBodyparts = new();
            List<IGameItem> items = location.Location.LayerGameItems(location.Layer)
                                .SelectMany(x => x.ShallowAccessibleItems(group.GroupLeader)).ToList();
            foreach (IGameItem item in items)
            {
                if (item.IsItemType<ICorpse>())
                {
                    edibleCorpsesAndBodyparts.Add(item);
                    continue;
                }

                if (item.IsItemType<ISeveredBodypart>())
                {
                    edibleCorpsesAndBodyparts.Add(item);
                    continue;
                }

                if (item.IsItemType<IEdible>())
                {
                    foodItems.Add((location.Location, location.Layer), item.GetItemType<IEdible>());
                    continue;
                }
            }

            foreach (IRace race in races)
            {
                if (!race.CanEatCorpses)
                {
                    continue;
                }

                foreach (IGameItem item in edibleCorpsesAndBodyparts.ToList())
                {
                    if (race.CanEatCorpseMaterial(item.Material))
                    {
                        edibleCorpsesAndBodyparts.Remove(item);
                        if (item.IsItemType<ICorpse>())
                        {
                            corpses.Add((location.Location, location.Layer), item.GetItemType<ICorpse>());
                            continue;
                        }

                        bodyparts.Add((location.Location, location.Layer), item.GetItemType<ISeveredBodypart>());
                        continue;
                    }
                }
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

        foreach (ICharacter ch in group.GroupMembers)
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

                unsatisfiedThirst = true;
            }

            if (ch.NeedsModel.Status.IsHungry())
            {
                if (ch.PositionTarget is IGameItem ptItem)
                {
                    if (ptItem.GetItemType<IEdible>() is IEdible edible && ch.CanEat(edible,
                            edible.Parent.ContainedIn?.GetItemType<IContainer>(), null, 1.0))
                    {
                        ch.Eat(edible, edible.Parent.ContainedIn?.GetItemType<IContainer>(), null, 1.0, null);
                        continue;
                    }

                    if (ptItem.GetItemType<ICorpse>() is ICorpse corpse &&
                        ch.CanEat(corpse, ch.Race.BiteWeight).Success)
                    {
                        ch.Eat(corpse, ch.Race.BiteWeight, null);
                        continue;
                    }

                    if (ptItem.GetItemType<ISeveredBodypart>() is ISeveredBodypart severed &&
                        ch.CanEat(severed, ch.Race.BiteWeight).Success)
                    {
                        ch.Eat(severed, ch.Race.BiteWeight, null);
                        continue;
                    }
                }

                IEdible food = foodItems[(ch.Location, ch.RoomLayer)]
                           .Where(edible => ch.CanEat(edible, edible.Parent.ContainedIn?.GetItemType<IContainer>(),
                               null, 1.0))
                           .GetRandomElement();
                if (food is not null)
                {
                    ch.SetTarget(food.Parent);
                    ch.SetModifier(PositionModifier.None);
                    ch.SetEmote(null);
                    ch.Eat(food, food.Parent.ContainedIn?.GetItemType<IContainer>(), null, 1.0, null);
                    continue;
                }

                ICorpse foodCorpse = corpses[(ch.Location, ch.RoomLayer)]
                                 .Where(corpse => ch.CanEat(corpse, ch.Race.BiteWeight).Success)
                                 .GetRandomElement();
                if (foodCorpse is not null)
                {
                    ch.SetTarget(foodCorpse.Parent);
                    ch.SetModifier(PositionModifier.None);
                    ch.SetEmote(null);
                    ch.Eat(foodCorpse, ch.Race.BiteWeight, null);
                    continue;
                }

                ISeveredBodypart foodBodypart = bodyparts[(ch.Location, ch.RoomLayer)]
                                   .Where(bodypart => ch.CanEat(bodypart, ch.Race.BiteWeight).Success)
                                   .GetRandomElement();
                if (foodBodypart is not null)
                {
                    ch.SetTarget(foodBodypart.Parent);
                    ch.SetModifier(PositionModifier.None);
                    ch.SetEmote(null);
                    ch.Eat(foodBodypart, ch.Race.BiteWeight, null);
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

    protected virtual void HandleGeneral(IGroupAI group, PredatorGroupData data)
    {
        base.HandleGeneral(group);
    }

    protected abstract void GatherStragglers(IGroupAI group, PredatorGroupData data);

    protected virtual void EvaluateAlertLevel(IGroupAI group, PredatorGroupData data)
    {
        throw new NotImplementedException();
    }

    protected (IEnumerable<ICharacter> MainGroup, IEnumerable<ICharacter> Stragglers, IEnumerable<ICharacter> Vulnerable
        ) GetGroups(IGroupAI group, PredatorGroupData data)
    {
        if (!group.GroupRoles.Any(x => x.Value == GroupRole.Leader))
        {
            EnsureLeaderExists(group);
        }

        List<ICharacter> main = new();
        List<ICharacter> stragglers = new();
        List<ICharacter> vulnerable = new();

        if (group.GroupRoles.Any(x => x.Value == GroupRole.Leader))
        {
            ICharacter leader = group.GroupRoles.First(x => x.Value == GroupRole.Leader).Key;
            foreach (ICharacter ch in group.GroupMembers)
            {
                if (group.GroupRoles[ch] == GroupRole.Child || group.GroupRoles[ch] == GroupRole.Elder)
                {
                    vulnerable.Add(ch);
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

            return (main, stragglers, vulnerable);
        }

        (ICell Location, RoomLayer Layer) mainLocation = group.GroupMembers.GroupBy(x => (Location: x.Location, Layer: x.RoomLayer))
                                .Select(x => (x.Key, Count: x.Count())).FirstMax(x => x.Count).Key;
        if (mainLocation.Location == null)
        {
            return (Enumerable.Empty<ICharacter>(), Enumerable.Empty<ICharacter>(), Enumerable.Empty<ICharacter>());
        }

        foreach (ICharacter ch in group.GroupMembers)
        {
            if (group.GroupRoles[ch] == GroupRole.Child || group.GroupRoles[ch] == GroupRole.Elder)
            {
                vulnerable.Add(ch);
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

        return (main, stragglers, vulnerable);
    }

    protected void EstablishPriorities(IGroupAI group, PredatorGroupData data)
    {
        (IEnumerable<ICharacter> main, IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> vulnerable) = GetGroups(group, data);
        if (!main.Any())
        {
            return;
        }

        if (EstablishPrioritiesHandled(group, data, main, stragglers, vulnerable))
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

    protected void PruneStaleThreatLocations(IGroupAI group, PredatorGroupData data)
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

    public override void HandleMinuteTick(IGroupAI group)
    {
        PredatorGroupData data = (PredatorGroupData)group.Data;
        PruneStaleThreatLocations(group, data);
        EstablishPriorities(group, data);
    }

    protected virtual List<ICharacter> GetThreats(IGroupAI group, PredatorGroupData data)
    {
        List<ICharacter> knownCharacters = group.GroupMembers
                                   .SelectMany(x =>
                                       x.SeenTargets.Concat(x.Location.LayerCharacters(x.RoomLayer))
                                        .Where(y => x.CanSee(y)))
                                   .Distinct().OfType<ICharacter>().Except(group.GroupMembers).ToList();
        List<ICharacter> threats = knownCharacters.Where(x => group.ConsidersThreat(x, group.Alertness)).ToList();
        return threats;
    }

    protected virtual bool ShouldGroupBreak(IGroupAI group, PredatorGroupData data, IEnumerable<ICharacter> main,
        IEnumerable<ICharacter> threats)
    {
        if (main.Any(x => group.GroupRoles[x] != GroupRole.Child && x.HealthStrategy.IsCriticallyInjured(x)))
        {
            return true;
        }

        return false;
    }

    protected virtual bool EstablishPrioritiesHandled(IGroupAI group, PredatorGroupData data,
        IEnumerable<ICharacter> main, IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> vulnerable)
    {
        List<ICharacter> threats = GetThreats(group, data);

        if (threats.Any() && group.Alertness > GroupAlertness.Agitated)
        {
            foreach (ICell location in threats.Select(x => x.Location).Distinct())
            {
                data.KnownThreatLocations[location] = DateTime.UtcNow;
                group.Changed = true;
            }
        }

        if (group.GroupMembers.Any(x => x.Combat != null))
        {
            if (ShouldGroupBreak(group, data, main, threats))
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
            List<ICharacter> fighters = main.Where(x => group.GroupRoles[x] != GroupRole.Child).ToList();
            if (fighters.Count / threats.Count >= 2 && RandomUtilities.Roll(1.0, 0.5))
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
            if (ShouldGroupBreak(group, data, main, threats))
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

    protected class PredatorGroupData : BaseGroupTypeData
    {
        public List<ICell> Territory { get; } = new();

        public PredatorGroupData(IFuturemud gameworld) : base(gameworld)
        {
        }

        public PredatorGroupData(XElement root, IFuturemud gameworld) : base(root, gameworld)
        {
            foreach (XElement item in root.Element("Territory").Elements())
            {
                ICell location = gameworld.Cells.Get(long.Parse(item.Value));
                if (location is not null)
                {
                    Territory.Add(location);
                }
            }
        }

        public override XElement SaveToXml()
        {
            XElement item = base.SaveToXml();
            item.Add(new XElement("Territory", from cell in Territory select new XElement("Cell", cell.Id)));
            return item;
        }

        public override string ShowText(ICharacter voyeur)
        {
            StringBuilder sb = new();
            sb.Append(base.ShowText(voyeur));
            string childText = ShowTextBeforeTerritory(voyeur);
            if (!string.IsNullOrEmpty(childText))
            {
                sb.Append(childText);
            }

            sb.AppendLine($"Territory:");
            sb.AppendLine();
            foreach (ICell cell in Territory)
            {
                sb.AppendLine($"\t{cell.GetFriendlyReference(voyeur).ColourName()}");
            }

            return sb.ToString();
        }

        protected virtual string ShowTextBeforeTerritory(ICharacter voyeur)
        {
            return string.Empty;
        }
    }
}