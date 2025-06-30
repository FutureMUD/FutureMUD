using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Construction.Boundary;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.NPC.AI.Groups.GroupTypes;

public class FamilyPredatorGroup : PredatorGroupBase
{
	public static void RegisterGroupAIType()
	{
		GroupAITypeFactory.RegisterGroupAIType("familypredator", DatabaseLoader, BuilderLoader);
	}

	private static IGroupAIType DatabaseLoader(XElement root, IFuturemud gameworld)
	{
		return new FamilyPredatorGroup(root, gameworld);
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

		return (new FamilyPredatorGroup(gender, activeTimes, gameworld), string.Empty);
	}

	/// <inheritdoc />
	public FamilyPredatorGroup(Gender dominantGender, IEnumerable<TimeOfDay> activeTimes, IFuturemud gameworld) : base(
		dominantGender, activeTimes, gameworld)
	{
	}

	/// <inheritdoc />
	public FamilyPredatorGroup(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
	}

	#region Overrides of GroupAIType

	/// <inheritdoc />
	public override string Name
	{
		get
		{
			if (DominantGender == Gender.Indeterminate)
			{
				return $"Egalitarian {GroupActivityTimeDescription} Family Predators";
			}

			return $"{DominantGender.DescribeEnum()}-Dominant {GroupActivityTimeDescription} Family Predators";
		}
	}

	protected class FamilyPredatorData : PredatorGroupData
	{
		public List<IRace> UntrustedRaces { get; } = new();
		public ICell HomeLocation { get; set; }

		public FamilyPredatorData(IFuturemud gameworld) : base(gameworld)
		{
		}

		public FamilyPredatorData(XElement root, IFuturemud gameworld) : base(root, gameworld)
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
			sb.AppendLine(
				$"Home Location: {HomeLocation?.GetFriendlyReference(voyeur).ColourName() ?? "None".Colour(Telnet.Red)}");
			return sb.ToString();
		}

		#region Overrides of PredatorGroupData

		/// <inheritdoc />
		protected override string ShowTextBeforeTerritory(ICharacter voyeur)
		{
			return
				$@"Untrusted Races: {UntrustedRaces.Select(x => x.Name.ColourValue()).ListToCommaSeparatedValues()}
Home Location: {HomeLocation?.GetFriendlyReference(voyeur).ColourName() ?? "None".Colour(Telnet.Red)}";
		}

		#endregion
	}

        /// <inheritdoc />
        public override void HandleMinuteTick(IGroupAI herd)
        {
                base.HandleMinuteTick(herd);
        }

	/// <inheritdoc />
        public override IGroupTypeData LoadData(XElement root, IFuturemud gameworld)
        {
                return new FamilyPredatorData(root, gameworld);
        }

	/// <inheritdoc />
        public override IGroupTypeData GetInitialData(IFuturemud gameworld)
        {
                return new FamilyPredatorData(gameworld);
        }

	/// <inheritdoc />
        public override XElement SaveToXml()
        {
                return new XElement("GroupType",
                        new XAttribute("typename", "familypredator"),
                        new XElement("ActiveTimes",
                                from time in ActiveTimesOfDay
                                select new XElement("Time", (int)time)
                        ),
                        new XElement("Gender", (short)DominantGender)
                );
        }

	public override bool ConsidersThreat(ICharacter ch, IGroupAI group, GroupAlertness alertness)
	{
		var data = (FamilyPredatorData)group.Data;
		if (alertness > GroupAlertness.NotAlert && data.UntrustedRaces.Any(x => x.SameRace(ch.Race)))
		{
			return true;
		}

		return false;
	}

	protected override void GatherStragglers(IGroupAI group, PredatorGroupData data)
	{
		if (!group.GroupMembers.Any())
		{
			return;
		}

		var (main, stragglers, _) = GetGroups(group, data);
		if (!stragglers.Any())
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
	}

        protected override bool HandleTenSecondAction(IGroupAI group, PredatorGroupData data,
                GroupAction groupCurrentAction)
        {
                var (main, stragglers, vulnerable) = GetGroups(group, data);
                var threats = GetThreats(group, data);
                switch (groupCurrentAction)
                {
                        case GroupAction.AvoidThreat:
                                HandleAvoidThreat(group, data, threats, main, stragglers, vulnerable);
                                return true;
                        case GroupAction.Flee:
                                HandleFlee(group, data, threats, main, stragglers, vulnerable);
                                return true;
                        case GroupAction.Posture:
                                HandlePosture(group, data, threats, main, stragglers, vulnerable);
                                return true;
                        case GroupAction.ControlledRetreat:
                                HandleControlledRetreat(group, data, threats, main, stragglers, vulnerable);
                                return true;
                        case GroupAction.AttackThreats:
                                HandleAttackThreats(group, data, threats, main, stragglers, vulnerable);
                                return true;
                }

                return false;
        }

	/// <inheritdoc />
        protected override void HandleFindFood(IGroupAI group, PredatorGroupData data)
        {
                var leader = group.GroupRoles.First(x => x.Value == GroupRole.Leader).Key;
                var items = leader.Location.LayerGameItems(leader.RoomLayer)
                                   .SelectMany(x => x.ShallowAccessibleItems(leader)).ToList();
                var hasFood = items.Any(item =>
                        (item.IsItemType<IEdible>() &&
                         leader.CanEat(item.GetItemType<IEdible>(), item.ContainedIn?.GetItemType<IContainer>(), null, 1.0)) ||
                        (item.IsItemType<ICorpse>() &&
                         leader.CanEat(item.GetItemType<ICorpse>(), leader.Race.BiteWeight).Success) ||
                        (item.IsItemType<ISeveredBodypart>() &&
                         leader.CanEat(item.GetItemType<ISeveredBodypart>(), leader.Race.BiteWeight).Success));

                if (hasFood)
                {
                        group.CurrentAction = GroupAction.Graze;
                        return;
                }

                if (leader.CouldMove(false, null).Success)
                {
                        var recent = leader.EffectsOfType<AdjacentToExit>().FirstOrDefault();
                        var random = leader.Location.ExitsFor(leader)
                                                 .Where(x => CanMoveExitFunctionFor(leader, group).Invoke(x))
                                                 .GetWeightedRandom(x => recent?.Exit == x ? 1.0 : 100.0);
                        if (random != null && leader.CanMove(random))
                        {
                                leader.Move(random);
                        }
                }
        }

        protected override void EvaluateAlertLevel(IGroupAI group, PredatorGroupData pdata)
        {
                var data = (FamilyPredatorData)pdata;
                var (main, _, _) = GetGroups(group, data);

                if (!main.Any())
                {
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

                                        group.Alertness = GroupAlertness.Aggressive;
                                        group.Changed = true;
                                        break;
                                case GroupAlertness.Aggressive:
                                        group.Alertness = GroupAlertness.Broken;
                                        group.Changed = true;
                                        break;
                        }

                        return;
                }

                switch (group.Alertness)
                {
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

        private void HandleAvoidThreat(IGroupAI group, PredatorGroupData data, List<ICharacter> threats,
                IEnumerable<ICharacter> main, IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> vulnerable)
        {
                if (!threats.Any())
                {
                        group.CurrentAction = GroupAction.Graze;
                        group.Changed = true;
                        return;
                }

                var leader = main.FirstOrDefault(x => group.GroupRoles[x] == GroupRole.Leader) ?? main.FirstOrDefault();
                HandleAvoidThreatForSubgroup(group, data, threats, main, leader);
                foreach (var vg in vulnerable.GroupBy(x => (x.Location, x.RoomLayer)))
                {
                        HandleAvoidThreatForSubgroup(group, data, threats, vg, vg.GetRandomElement());
                }
        }

        private void HandleAvoidThreatForSubgroup(IGroupAI group, PredatorGroupData data, IEnumerable<ICharacter> threats,
                IEnumerable<ICharacter> characters, ICharacter leader)
        {
                var threatPaths = threats
                                  .Select(x => (Character: x, Path: leader.PathBetween(x, 5, PathSearch.RespectClosedDoors)))
                                  .ToList();
                var threatDirections = threatPaths.Select(x => x.Path)
                                                  .CountTotalDirections<IEnumerable<IEnumerable<ICellExit>>, IEnumerable<ICellExit>>()
                                                  .ContainedDirections();
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

        private void HandleFlee(IGroupAI group, PredatorGroupData data, List<ICharacter> threats, IEnumerable<ICharacter> main,
                IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> vulnerable)
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

                HandleAvoidThreat(group, data, threats, main, stragglers, vulnerable);
        }

        private void HandleAttackThreats(IGroupAI group, PredatorGroupData data, List<ICharacter> threats,
                IEnumerable<ICharacter> main, IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> vulnerable)
        {
                var fighters = main.Where(x => group.GroupRoles[x] != GroupRole.Child).ToList();
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

        private void HandlePosture(IGroupAI group, PredatorGroupData data, List<ICharacter> threats,
                IEnumerable<ICharacter> main, IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> vulnerable)
        {
                var emotes = group.GroupEmotes.Where(x => x.Applies(group)).ToList();
                if (!emotes.Any() || Dice.Roll(1, 6) != 1)
                {
                        return;
                }

                var emote = emotes.GetRandomElement();
                var emoter = main.Where(x =>
                        x.State.IsAble() && threats.Any(y => y.ColocatedWith(x)) &&
                        (!emote.RequiredRole.HasValue || group.GroupRoles[x] == emote.RequiredRole)).GetRandomElement();
                emoter?.OutputHandler.Handle(new EmoteOutput(new Emote(emote.EmoteText, emoter, emoter,
                        threats.Where(x => x.ColocatedWith(emoter)).GetRandomElement())));
        }

        private void HandleControlledRetreat(IGroupAI group, PredatorGroupData data, List<ICharacter> threats,
                IEnumerable<ICharacter> main, IEnumerable<ICharacter> stragglers, IEnumerable<ICharacter> vulnerable)
        {
                var fighters = main.Where(x => group.GroupRoles[x] == GroupRole.Elder).ToList();
                var nonFighters = main.Except(fighters).ToList();
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
	

	#endregion
}