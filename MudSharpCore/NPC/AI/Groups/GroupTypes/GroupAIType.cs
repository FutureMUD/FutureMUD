using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
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
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Celestial;
using MudSharp.Character.Heritage;

namespace MudSharp.NPC.AI.Groups.GroupTypes;

public abstract class GroupAIType : IGroupAIType, IHaveFuturemud
{
	public IFuturemud Gameworld { get; }

	private static TimeSpan _minimumEmoteFrequency = TimeSpan.MinValue;

	public static TimeSpan MinimumEmoteFrequency
	{
		get
		{
			if (_minimumEmoteFrequency == TimeSpan.MinValue)
			{
				_minimumEmoteFrequency =
					TimeSpan.FromSeconds(Futuremud.Games.First().GetStaticDouble("MinimumGroupAIEmoteFrequency"));
			}

			return _minimumEmoteFrequency;
		}
	}

	private static double _emoteChance = double.NaN;

	public static double EmoteChance
	{
		get
		{
			if (double.IsNaN(_emoteChance))
			{
				_emoteChance = Futuremud.Games.First().GetStaticDouble("GroupAIEmoteChancePer10Seconds");
			}

			return _emoteChance;
		}
	}

	protected GroupAIType(Gender dominantGender, IEnumerable<TimeOfDay> activeTimes, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		DominantGender = dominantGender;
		ActiveTimesOfDay.AddRange(activeTimes);
	}

	protected GroupAIType(XElement root, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		DominantGender = (Gender)short.Parse(root.Element("Gender").Value);
		ActiveTimesOfDay.AddRange(root.Element("ActiveTimes").Elements().Select(x => (TimeOfDay)int.Parse(x.Value)));
	}

	protected class BaseGroupTypeData : IGroupTypeData
	{
		public IFuturemud Gameworld { get; }
		public List<ICell> KnownWaterLocations { get; } = new();
		public Dictionary<ICell, DateTime> KnownThreatLocations { get; } = new();
		public DateTime LastEmote { get; set; }

		public BaseGroupTypeData(IFuturemud gameworld)
		{
			Gameworld = gameworld;
			LastEmote = DateTime.UtcNow;
		}

		public virtual string ShowText(ICharacter voyeur)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"Last Emote: {LastEmote.GetLocalDateString(voyeur, true).ColourValue()}");
			sb.AppendLine(
				$"Known Water Locations: {KnownWaterLocations.Select(x => x.Id.ToString("N0", voyeur).ColourValue()).ListToCommaSeparatedValues()}");
			sb.AppendLine(
				$"Known Threat Locations: {KnownThreatLocations.Select(x => x.Key.Id.ToString("N0", voyeur).ColourValue()).ListToCommaSeparatedValues()}");
			return sb.ToString();
		}

		public BaseGroupTypeData(XElement root, IFuturemud gameworld)
		{
			Gameworld = gameworld;
			LastEmote = DateTime.Parse(root.Element("LastEmote").Value);

			foreach (var item in root.Element("Water").Elements())
			{
				var cell = gameworld.Cells.Get(long.Parse(item.Value));
				if (cell == null)
				{
					continue;
				}

				KnownWaterLocations.Add(cell);
			}

			foreach (var item in root.Element("Threats").Elements())
			{
				var cell = gameworld.Cells.Get(long.Parse(item.Attribute("id").Value));
				if (cell == null)
				{
					continue;
				}

				KnownThreatLocations[cell] = DateTime.Parse(item.Value);
			}
		}

		public virtual XElement SaveToXml()
		{
			return new XElement("Data",
				new XElement("LastEmote", LastEmote.ToString("o")),
				new XElement("Water", from cell in KnownWaterLocations select new XElement("Cell", cell.Id)),
				new XElement("Threats",
					from cell in KnownThreatLocations
					select new XElement("Cell", new XAttribute("id", cell.Key.Id), cell.Value.ToString("o")))
			);
		}
	}

	public abstract string Name { get; }

	public virtual void HandleTenSecondTick(IGroupAI herd)
	{
		EvaluateGroupRolesAndMemberships(herd);
		CheckHerdEmotes(herd);
	}

	public virtual bool ConsidersThreat(ICharacter ch, IGroupAI group, GroupAlertness alertness)
	{
		return false;
	}

	public abstract void HandleMinuteTick(IGroupAI herd);
	public abstract IGroupTypeData LoadData(XElement root, IFuturemud gameworld);
	public abstract IGroupTypeData GetInitialData(IFuturemud gameworld);
	public abstract XElement SaveToXml();

	public Gender DominantGender { get; }
	protected List<TimeOfDay> ActiveTimesOfDay { get; } = new();

	protected string GroupActivityTimeDescription
	{
		get
		{
			var dawn = ActiveTimesOfDay.Contains(TimeOfDay.Dawn);
			var morning = ActiveTimesOfDay.Contains(TimeOfDay.Morning);
			var afternoon = ActiveTimesOfDay.Contains(TimeOfDay.Afternoon);
			var dusk = ActiveTimesOfDay.Contains(TimeOfDay.Dusk);
			var night = ActiveTimesOfDay.Contains(TimeOfDay.Night);

			if (dawn && morning && afternoon && dusk && night)
			{
				return "Always Active";
			}

			if (dawn && morning && afternoon && dusk)
			{
				return "Long Diurnal";
			}

			if (dawn && morning && afternoon)
			{
				return "Early Diurnal";
			}

			if (morning && afternoon && dusk)
			{
				return "Late Diurnal";
			}

			if (morning && afternoon)
			{
				return "Diurnal";
			}

			if (dawn && dusk && night)
			{
				return "Long Nocturnal";
			}

			if (dusk && night)
			{
				return "Early Nocturnal";
			}

			if (night && dawn)
			{
				return "Late Nocturnal";
			}

			if (night)
			{
				return "Nocturnal";
			}

			if (dawn && dusk)
			{
				return "Crepuscular";
			}

			if (dawn)
			{
				return "Matutinal";
			}

			if (dusk)
			{
				return "Vespertine";
			}

			return "Unknown";
		}
	}

	protected virtual void CheckHerdEmotes(IGroupAI herd)
	{
		var data = (BaseGroupTypeData)herd.Data;
		if (DateTime.UtcNow - data.LastEmote < MinimumEmoteFrequency)
		{
			return;
		}

		if (RandomUtilities.DoubleRandom(0.0, 1.0) > EmoteChance)
		{
			return;
		}

		var emote = herd.GroupEmotes.Where(x => x.Applies(herd)).GetRandomElement();
		if (emote == null)
		{
			return;
		}

		emote.DoEmote(herd);
		data.LastEmote = DateTime.UtcNow;
		herd.Changed = true;
	}

	public virtual void EvaluateGroupRolesAndMemberships(IGroupAI group)
	{
		// Ensure any new group members have a role
		foreach (var ch in group.GroupMembers)
		{
			if (group.GroupRoles.ContainsKey(ch))
			{
				continue;
			}

			group.Changed = true;
			var age = ch.Race.AgeCategory(ch);
			switch (age)
			{
				case AgeCategory.Baby:
				case AgeCategory.Child:
				case AgeCategory.Youth:
					group.GroupRoles[ch] = GroupRole.Child;
					continue;
				case AgeCategory.Elder:
				case AgeCategory.Venerable:
					group.GroupRoles[ch] = GroupRole.Elder;
					continue;
			}

			if (CanBePretender(ch))
			{
				group.GroupRoles[ch] = GroupRole.Pretender;
				continue;
			}
			else
			{
				group.GroupRoles[ch] = GroupRole.Adult;
			}
		}

		// Remove any group members who have since left the group
		foreach (var ch in group.GroupRoles.Keys.ToList())
		{
			if (!group.GroupMembers.Contains(ch))
			{
				group.GroupRoles.Remove(ch);
				group.Changed = true;
			}
		}

		// If there are no other adults left, outsiders become pretenders or adults
		if (group.GroupRoles.Values.All(x =>
			    !x.In(GroupRole.Leader, GroupRole.Elder, GroupRole.Pretender, GroupRole.Adult)))
		{
			var outsiders = group.GroupRoles.Where(x => x.Value == GroupRole.Outsider).Select(x => x.Key).ToList();
			foreach (var ch in outsiders)
			{
				group.GroupRoles[ch] = CanBePretender(ch) ? GroupRole.Pretender : GroupRole.Adult;
				group.Changed = true;
			}
		}

		EnsureLeaderExists(group);
	}

	public virtual bool CanBePretender(ICharacter ch)
	{
		return true;
	}

	protected virtual void EnsureLeaderExists(IGroupAI group)
	{
		// Try to ensure that there is at least 1 leader
		if (group.GroupRoles.Values.All(x => x != GroupRole.Leader))
		{
			// Pretenders first
			var newLeader = group.GroupRoles.Where(x => x.Value == GroupRole.Pretender).Select(x => x.Key)
			                     .GetRandomElement();

			if (newLeader == null)
				// Try adults as a backup
			{
				newLeader = group.GroupRoles.Where(x => x.Value == GroupRole.Adult).Select(x => x.Key)
				                 .GetRandomElement();
			}

			if (newLeader == null)
				// Try dominant elders as a backup
			{
				newLeader = group.GroupRoles.Where(x => x.Value == GroupRole.Elder && CanBePretender(x.Key))
				                 .Select(x => x.Key).GetRandomElement();
			}

			if (newLeader == null)
				// Try non-dominant elders next
			{
				newLeader = group.GroupRoles.Where(x => x.Value == GroupRole.Elder && !CanBePretender(x.Key))
				                 .Select(x => x.Key).GetRandomElement();
			}

			if (newLeader == null)
				// Just pick randomly at this point
			{
				newLeader = group.GroupMembers.GetRandomElement();
			}

			if (newLeader != null)
			{
				group.GroupRoles[newLeader] = GroupRole.Leader;
				group.Changed = true;
			}
		}
	}

	protected Func<ICellExit, bool> CanMoveExitFunctionFor(ICharacter ch, IGroupAI group)
	{
		return exit => ch.CouldMove(false, null).Success && ch.CanMove(exit) &&
		               !group.AvoidCell(exit.Destination, group.Alertness);
	}

	protected bool PathIndividualToLocation(ICharacter ch, IGroupAI group, ICell location)
	{
		var path = ch.PathBetween(location, 20, CanMoveExitFunctionFor(ch, group)).ToList();
		if (!path.Any())
		{
			return false;
		}

		var fp = new FollowingPath(ch, path);
		ch.AddEffect(fp);
		fp.FollowPathAction();
		return true;
	}

	protected void PathIndividualToLayer(ICharacter ch, RoomLayer targetLayer)
	{
		if (ch.RoomLayer.IsLowerThan(targetLayer))
		{
			if (ch.PositionState != PositionSwimming.Instance && ch.Location.IsUnderwaterLayer(ch.RoomLayer))
			{
				ch.MovePosition(PositionSwimming.Instance, PositionModifier.None, null, null, null);
				return;
			}

			if (ch.Location.IsUnderwaterLayer(ch.RoomLayer))
			{
				((ISwim)ch).Ascend();
				return;
			}

			if (ch.PositionState != PositionFlying.Instance && ch.CanFly().Truth)
			{
				ch.Fly();
				return;
			}

			if (ch.PositionState == PositionFlying.Instance)
			{
				((IFly)ch).Ascend();
				return;
			}

			if (ch.CanClimbUp().Truth)
			{
				ch.ClimbUp();
				return;
			}

			return;
		}

		if (ch.PositionState != PositionSwimming.Instance && ch.Location.IsSwimmingLayer(ch.RoomLayer))
		{
			ch.MovePosition(PositionSwimming.Instance, PositionModifier.None, null, null, null);
			return;
		}

		if (ch.Location.IsSwimmingLayer(ch.RoomLayer))
		{
			((ISwim)ch).Dive();
			return;
		}

		if (ch.PositionState == PositionFlying.Instance)
		{
			if (ch.RoomLayer == RoomLayer.GroundLevel)
			{
				ch.Land();
				return;
			}

			((IFly)ch).Dive();
			return;
		}

		if (ch.CanClimbDown().Truth)
		{
			ch.ClimbDown();
			return;
		}
	}

	protected bool IgnoreTickAI(ICharacter ch)
	{
		return
			ch.State.IsDisabled() ||
			ch.Movement != null ||
			ch.IsEngagedInMelee ||
			ch.CombinedEffectsOfType<IEffect>()
			  .Any(x => x.IsBlockingEffect("general") || x.IsBlockingEffect("movement")) ||
			ch.CombinedEffectsOfType<FollowingPath>().Any();
	}

	protected List<ILiquidContainer> LocalLiquids(ICell location, RoomLayer layer)
	{
		return location
		       .LayerGameItems(layer)
		       .SelectNotNull(x => x.GetItemType<ILiquidContainer>())
		       .Where(x => (x.LiquidMixture?.Instances.Sum(y => y.Liquid.DrinkSatiatedHoursPerLitre) ?? 0.0) > 0.0)
		       .ToList();
	}

	protected virtual void HandleGeneral(IGroupAI group)
	{
		foreach (var ch in group.GroupMembers)
		{
			if (ch.State.IsAsleep() && group.CurrentAction != GroupAction.Sleep && Constants.Random.Next(0, 15) == 0)
			{
				ch.Awaken();
				if (Constants.Random.Next(0, 2) > 0)
				{
					continue;
				}
			}

			if (IgnoreTickAI(ch))
			{
				// Leaders and outsiders move more slowly than others
				if (ch.AffectedBy<FollowingPath>() && (!group.GroupRoles[ch].In(GroupRole.Leader, GroupRole.Outsider) ||
				                                       Constants.Random.Next(0, 3) == 0))
				{
					ch.EffectsOfType<FollowingPath>().First().FollowPathAction();
				}

				continue;
			}

			if (!group.CurrentAction.In(GroupAction.Rest, GroupAction.Sleep) && !ch.PositionState.Upright &&
			    ch.PositionState != PositionSwimming.Instance && ch.PositionState != PositionFlying.Instance &&
			    ch.PositionState != PositionClimbing.Instance)
			{
				var upright = ch.MostUprightMobilePosition();
				if (upright != null)
				{
					ch.MovePosition(upright, null, null);
					continue;
				}
			}
		}
	}

	protected static (bool Truth, string Error, IEnumerable<TimeOfDay> ActiveTimes) ParseBuilderArgument(
		string argument)
	{
		var validTypes = new[]
		{
			"always", "diurnal", "early diurnal", "late diurnal", "long diurnal", "nocturnal", "early nocturnal",
                       "late nocturnal", "long nocturnal", "crepuscular", "matutinal", "vespertine"
		};
		var activeTimes = new List<TimeOfDay>();
		switch (argument)
		{
			case "nocturnal":
				activeTimes.Add(TimeOfDay.Night);
				break;
			case "early nocturnal":
				activeTimes.Add(TimeOfDay.Dusk);
				activeTimes.Add(TimeOfDay.Night);
				break;
			case "late nocturnal":
				activeTimes.Add(TimeOfDay.Night);
				activeTimes.Add(TimeOfDay.Dawn);
				break;
			case "long nocturnal":
				activeTimes.Add(TimeOfDay.Dusk);
				activeTimes.Add(TimeOfDay.Night);
				activeTimes.Add(TimeOfDay.Dawn);
				break;
			case "diurnal":
				activeTimes.Add(TimeOfDay.Morning);
				activeTimes.Add(TimeOfDay.Afternoon);
				break;
			case "early diurnal":
				activeTimes.Add(TimeOfDay.Dawn);
				activeTimes.Add(TimeOfDay.Morning);
				activeTimes.Add(TimeOfDay.Afternoon);
				break;
                       case "late diurnal":
				activeTimes.Add(TimeOfDay.Morning);
				activeTimes.Add(TimeOfDay.Afternoon);
				activeTimes.Add(TimeOfDay.Dusk);
				break;
			case "long diurnal":
				activeTimes.Add(TimeOfDay.Dawn);
				activeTimes.Add(TimeOfDay.Morning);
				activeTimes.Add(TimeOfDay.Afternoon);
				activeTimes.Add(TimeOfDay.Dusk);
				break;
			case "crepuscular":
				activeTimes.Add(TimeOfDay.Dawn);
				activeTimes.Add(TimeOfDay.Dusk);
				break;
			case "matutinal":
				activeTimes.Add(TimeOfDay.Dawn);
				break;
			case "vespertine":
				activeTimes.Add(TimeOfDay.Dusk);
				break;
			case "always":
				activeTimes.Add(TimeOfDay.Dawn);
				activeTimes.Add(TimeOfDay.Morning);
				activeTimes.Add(TimeOfDay.Afternoon);
				activeTimes.Add(TimeOfDay.Dusk);
				activeTimes.Add(TimeOfDay.Night);
				break;
			default:
				return (false,
					$"You must specify a daily activity pattern. Valid options are {validTypes.Select(x => x.ColourValue()).ListToString()}.",
					Enumerable.Empty<TimeOfDay>());
		}

		return (true, string.Empty, activeTimes);
	}
}