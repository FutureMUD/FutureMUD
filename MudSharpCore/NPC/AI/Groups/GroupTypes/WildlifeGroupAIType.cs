#nullable enable
using MudSharp.Body.Needs;
using MudSharp.Celestial;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;

namespace MudSharp.NPC.AI.Groups.GroupTypes;

/// <summary>
/// Broad, composition-first wildlife coordination. Individual AnimalAI instances continue to
/// own direct survival and combat reactions unless this type explicitly claims a control scope.
/// </summary>
public enum WildlifeGroupKind
{
	Herd,
	Pack,
	Family,
	Pride,
	Flock,
	School,
	Pod,
	Colony,
	Swarm,
	Managed
}

public enum WildlifeGroupTactic
{
	Timid,
	Defensive,
	Territorial,
	SeasonalGrazing,
	Hunting,
	Scavenging,
	Roosting,
	Aquatic,
	Amphibious,
	Managed
}

public sealed class WildlifeGroupAIType : GroupAIType, IGroupAIControlPolicy, IEditableGroupAIType
{
	private const int DefaultMovementRange = 12;

	public WildlifeGroupKind Kind { get; private set; }
	public WildlifeGroupTactic Tactic { get; private set; }
	public GroupAIControlScope ControlScope { get; private set; }
	public IFutureProg PreferredCellProg { get; private set; } = null!;
	public IFutureProg ShelterCellProg { get; private set; } = null!;
	public int MovementRange { get; private set; }
	public double WanderChancePerMinute { get; private set; }

	public static void RegisterGroupAIType()
	{
		GroupAITypeFactory.RegisterGroupAIType("wildlife", DatabaseLoader, BuilderLoader);
	}

	private static IGroupAIType DatabaseLoader(XElement root, IFuturemud gameworld)
	{
		return new WildlifeGroupAIType(root, gameworld);
	}

	private static (IGroupAIType Type, string Error) BuilderLoader(string builderArgs, IFuturemud gameworld)
	{
		StringStack command = new(builderArgs);
		if (command.IsFinished || !Utilities.TryParseEnum(command.PopSpeech(), out Gender gender))
		{
			return (null!, $"You must supply a dominant gender. Valid values are {Enum.GetValues<Gender>().ListToColouredString()}.");
		}

		if (command.IsFinished || !command.PopSpeech().TryParseEnum(out WildlifeGroupKind kind))
		{
			return (null!, $"You must supply a wildlife group kind. Valid values are {Enum.GetValues<WildlifeGroupKind>().ListToColouredString()}.");
		}

		if (command.IsFinished || !command.PopSpeech().TryParseEnum(out WildlifeGroupTactic tactic))
		{
			return (null!, $"You must supply a wildlife tactic. Valid values are {Enum.GetValues<WildlifeGroupTactic>().ListToColouredString()}.");
		}

		(bool success, string error, IEnumerable<TimeOfDay> activeTimes) =
			ParseBuilderArgument(command.SafeRemainingArgument.ToLowerInvariant());
		if (!success)
		{
			return (null!, error);
		}

		return (new WildlifeGroupAIType(gender, kind, tactic, activeTimes, gameworld), string.Empty);
	}

	private WildlifeGroupAIType(Gender dominantGender, WildlifeGroupKind kind, WildlifeGroupTactic tactic,
		IEnumerable<TimeOfDay> activeTimes, IFuturemud gameworld)
		: base(dominantGender, activeTimes, gameworld)
	{
		Kind = kind;
		Tactic = tactic;
		ControlScope = DefaultScopeFor(tactic);
		PreferredCellProg = gameworld.AlwaysTrueProg;
		ShelterCellProg = gameworld.AlwaysFalseProg;
		MovementRange = DefaultMovementRange;
		WanderChancePerMinute = 0.25;
	}

	private WildlifeGroupAIType(XElement root, IFuturemud gameworld)
		: base(root, gameworld)
	{
		Kind = root.Element("Kind")?.Value.TryParseEnum(out WildlifeGroupKind kind) == true
			? kind
			: WildlifeGroupKind.Herd;
		Tactic = root.Element("Tactic")?.Value.TryParseEnum(out WildlifeGroupTactic tactic) == true
			? tactic
			: WildlifeGroupTactic.Timid;
		ControlScope = ParseScope(root.Element("ControlScope")?.Value, DefaultScopeFor(Tactic));
		PreferredCellProg = gameworld.FutureProgs.Get(long.Parse(root.Element("PreferredCellProg")?.Value ?? "0")) ??
			gameworld.AlwaysTrueProg;
		ShelterCellProg = gameworld.FutureProgs.Get(long.Parse(root.Element("ShelterCellProg")?.Value ?? "0")) ??
			gameworld.AlwaysFalseProg;
		MovementRange = Math.Max(1, int.Parse(root.Element("MovementRange")?.Value ?? DefaultMovementRange.ToString()));
		WanderChancePerMinute = Math.Clamp(double.Parse(root.Element("WanderChancePerMinute")?.Value ?? "0.25"), 0.0, 1.0);
	}

	public override string Name => $"{Kind.DescribeEnum()} {GroupActivityTimeDescription} {Tactic.DescribeEnum()} Wildlife";

	public override bool ConsidersThreat(ICharacter ch, IGroupAI group, GroupAlertness alertness)
	{
		if (group.GroupMembers.ContainsPhysicalInstance(ch) ||
		    group.GroupMembers.Any(x => x.Race.SameRace(ch.Race)))
		{
			return false;
		}

		if (!Tactic.In(WildlifeGroupTactic.Timid, WildlifeGroupTactic.Defensive,
			    WildlifeGroupTactic.Territorial, WildlifeGroupTactic.Hunting, WildlifeGroupTactic.Scavenging) ||
		    alertness < GroupAlertness.Wary)
		{
			return false;
		}

		// The template owns the species-facing definition of an intruder or prey. This keeps a
		// hunting pack from treating every visible person as prey merely because it is a pack.
		return group.Template.ConsidersThreat(ch, alertness);
	}

	public override IGroupTypeData LoadData(XElement root, IFuturemud gameworld)
	{
		return new WildlifeGroupData(root, gameworld);
	}

	public override IGroupTypeData GetInitialData(IFuturemud gameworld)
	{
		return new WildlifeGroupData(gameworld);
	}

	public override XElement SaveToXml()
	{
		return new XElement("GroupType",
			new XAttribute("typename", "wildlife"),
			new XElement("Gender", (short)DominantGender),
			new XElement("ActiveTimes", ActiveTimesOfDay.Select(x => new XElement("Time", (int)x))),
			new XElement("Kind", Kind),
			new XElement("Tactic", Tactic),
			new XElement("ControlScope", ControlScope),
			new XElement("PreferredCellProg", PreferredCellProg?.Id ?? 0),
			new XElement("ShelterCellProg", ShelterCellProg?.Id ?? 0),
			new XElement("MovementRange", MovementRange),
			new XElement("WanderChancePerMinute", WanderChancePerMinute));
	}

	public override void HandleTenSecondTick(IGroupAI group)
	{
		base.HandleTenSecondTick(group);
		List<ICharacter> members = group.GroupMembers
			.Where(x => !x.State.IsDead() && !x.State.IsInStatis())
			.ToList();
		if (!members.Any())
		{
			return;
		}

		ICharacter leader = group.GroupLeader ?? members.First();
		bool membersRequireRest = members.All(MemberRequiresActivityRest);
		bool active = !membersRequireRest && ActiveTimesOfDay.Contains(leader.Location.CurrentTimeOfDay);
		bool survivalNeed = members.Any(HasUrgentSurvivalNeed);
		if (survivalNeed)
		{
			AwakenMembersForUrgentSurvivalNeed(members);
		}

		CoordinateSentriesAndScout(group, members, leader);
		List<ICharacter> threats = GetThreats(group, members);
		if (threats.Any() && ControlScope.HasFlag(GroupAIControlScope.Threats))
		{
			HandleThreats(group, members, leader, threats);
			return;
		}

		if ((!active || membersRequireRest) && ControlScope.HasFlag(GroupAIControlScope.Activity) && !survivalNeed)
		{
			SetAction(group, GroupAction.Sleep);
			foreach (ICharacter member in members.Where(x => !x.State.IsAsleep() && x.Combat is null && x.Movement is null))
			{
				member.Sleep(null);
			}

			return;
		}

		GatherStragglers(group, members, leader);
	}

	public override void HandleMinuteTick(IGroupAI group)
	{
		List<ICharacter> members = group.GroupMembers
			.Where(x => !x.State.IsDead() && !x.State.IsInStatis())
			.ToList();
		if (!members.Any())
		{
			return;
		}

		ICharacter leader = group.GroupLeader ?? members.First();
		WildlifeGroupData data = (WildlifeGroupData)group.Data;
		bool membersRequireRest = members.All(MemberRequiresActivityRest);
		bool active = !membersRequireRest && ActiveTimesOfDay.Contains(leader.Location.CurrentTimeOfDay);
		bool survivalNeed = members.Any(HasUrgentSurvivalNeed);
		if (survivalNeed)
		{
			AwakenMembersForUrgentSurvivalNeed(members);
		}

		if ((!active || membersRequireRest) && ControlScope.HasFlag(GroupAIControlScope.Activity) && !survivalNeed)
		{
			SetAction(group, GroupAction.Sleep);
			if (ControlScope.HasFlag(GroupAIControlScope.Shelter) && data.HomeCell is not null &&
			    !ReferenceEquals(leader.Location, data.HomeCell))
			{
				PathMemberToLocation(leader, group, data.HomeCell);
			}

			return;
		}

		if (ControlScope.HasFlag(GroupAIControlScope.Feeding) &&
		    CoordinateForaging(group, members, leader, data))
		{
			return;
		}

		if (ControlScope.HasFlag(GroupAIControlScope.Shelter) && data.HomeCell is null &&
			(Tactic.In(WildlifeGroupTactic.Territorial, WildlifeGroupTactic.Roosting) ||
			 Kind.In(WildlifeGroupKind.Family, WildlifeGroupKind.Colony)) &&
			ShelterCellProg.ExecuteBool(false, leader, leader.Location))
		{
			data.HomeCell = leader.Location;
			group.Changed = true;
		}

		if (!ControlScope.HasFlag(GroupAIControlScope.Movement) ||
		    RandomUtilities.DoubleRandom(0.0, 1.0) > WanderChancePerMinute ||
		    leader.Movement is not null || leader.Combat is not null)
		{
			return;
		}

		IEnumerable<ICellExit> exits = leader.Location.ExitsFor(leader)
			.Where(CanMoveExitFunctionFor(leader, group));
		ICellExit? destination = exits
			.Where(x => PreferredCellProg.ExecuteBool(false, leader, x.Destination))
			.GetRandomElement() ?? exits.GetRandomElement();
		if (destination is not null && leader.CanMove(destination))
		{
			leader.Move(destination);
			SetAction(group, Tactic.In(WildlifeGroupTactic.Hunting, WildlifeGroupTactic.Scavenging)
				? GroupAction.FindFood
				: GroupAction.Graze);
		}
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "kind":
				return SetEnum<WildlifeGroupKind>(actor, command, x => Kind = x, "wildlife group kind");
			case "tactic":
				return SetEnum<WildlifeGroupTactic>(actor, command, x =>
				{
					Tactic = x;
					ControlScope = DefaultScopeFor(x);
				}, "wildlife tactic");
			case "scope":
				return SetScope(actor, command);
			case "preferred":
				return SetCellProg(actor, command, x => PreferredCellProg = x, "preferred wildlife habitat");
			case "shelter":
				return SetCellProg(actor, command, x => ShelterCellProg = x, "wildlife shelter or roost");
			case "range":
				return SetRange(actor, command);
			case "wander":
			case "chance":
				return SetWanderChance(actor, command);
			case "activity":
				return SetActivity(actor, command);
		}

		actor.OutputHandler.Send("You can set #3kind <kind>#0, #3tactic <tactic>#0, #3scope <scopes>#0, #3preferred <prog>#0, #3shelter <prog>#0, #3range <cells>#0, #3wander <percent>#0 or #3activity <pattern>#0.".SubstituteANSIColour());
		return false;
	}

	public string Show(ICharacter actor)
	{
		return $"Wildlife Group Settings\n\nKind: {Kind.DescribeEnum().ColourName()}\nTactic: {Tactic.DescribeEnum().ColourName()}\nControl Scope: {ControlScope.ToString().ColourValue()}\nPreferred Habitat Prog: {PreferredCellProg.MXPClickableFunctionName()}\nShelter Prog: {ShelterCellProg.MXPClickableFunctionName()}\nMovement Range: {MovementRange.ToString("N0", actor).ColourValue()}\nWander Chance: {WanderChancePerMinute.ToString("P2", actor).ColourValue()}\nActivity: {GroupActivityTimeDescription.ColourName()}";
	}

	/// <summary>
	/// Produces non-mutating group state useful for live wildlife diagnosis. It deliberately reports
	/// each filtering stage so a game-specific template prog can be distinguished from group and
	/// individual activity gating.
	/// </summary>
	public string DebugSummary(IGroupAI group)
	{
		List<ICharacter> members = group.GroupMembers
			.Where(x => !x.State.IsDead() && !x.State.IsInStatis())
			.ToList();
		if (!members.Any())
		{
			return "No live group members are available for wildlife coordination.";
		}

		ICharacter leader = group.GroupLeader ?? members.First();
		bool membersRequireRest = members.All(MemberRequiresActivityRest);
		bool urgentNeed = members.Any(HasUrgentSurvivalNeed);
		bool scheduleActive = ActiveTimesOfDay.Contains(leader.Location.CurrentTimeOfDay);
		List<ICharacter> visibleOthers = members
			.SelectMany(x => x.Location.CharactersInSpatialVicinity(x).Where(y => x.CanSee(y)))
			.OfType<ICharacter>()
			.DistinctPhysicalInstances()
			.Where(x => !group.GroupMembers.ContainsPhysicalInstance(x))
			.Where(x => !group.GroupMembers.Any(y => y.Race.SameRace(x.Race)))
			.ToList();
		int templateMatches = visibleOthers.Count(x => group.Template.ConsidersThreat(x, group.Alertness));
		int groupMatches = visibleOthers.Count(x => group.ConsidersThreat(x, group.Alertness));
		int actionableThreats = GetThreats(group, members).Count;

		return $"Leader #{leader.Id}; time {leader.Location.CurrentTimeOfDay.DescribeEnum()}; " +
		       $"schedule {(scheduleActive ? "active" : "inactive")}; members require rest {membersRequireRest}; " +
		       $"urgent survival {urgentNeed}; visible non-group candidates {visibleOthers.Count}; " +
		       $"template matches {templateMatches}; group matches {groupMatches}; actionable threats {actionableThreats}.";
	}

	private List<ICharacter> GetThreats(IGroupAI group, IEnumerable<ICharacter> members)
	{
		if (Tactic.In(WildlifeGroupTactic.Hunting, WildlifeGroupTactic.Scavenging))
		{
			List<ICharacter> hungryMembers = members.Where(PredatorAIHelpers.IsHungry).ToList();
			if (!hungryMembers.Any())
			{
				return [];
			}

			// Carrion is a cheaper and safer meal than a fresh attack. If eating could not begin this
			// instant, keep the live hunt suppressed while the accessible corpse remains available.
			if (hungryMembers.Any(PredatorAIHelpers.EatLocalCorpseIfHungry) ||
			    hungryMembers.Any(x => PredatorAIHelpers.FindLocalEdibleCorpse(x) is not null))
			{
				return [];
			}

			return HuntingTargets(group, hungryMembers);
		}

		return members
			.SelectMany(member => member.Location
				.LayerCharacters(member.RoomLayer)
				.Concat(member.SeenTargets.OfType<ICharacter>())
				.Where(target => AnimalAI.CanGroupObserveTarget(member, target)))
			.OfType<ICharacter>()
			.DistinctPhysicalInstances()
			.Where(x => !group.GroupMembers.ContainsPhysicalInstance(x))
			.Where(x => !group.GroupMembers.Any(y => y.Race.SameRace(x.Race)))
			.Where(x => group.ConsidersThreat(x, group.Alertness))
			.ToList();
	}

	private static List<ICharacter> HuntingTargets(IGroupAI group, IEnumerable<ICharacter> members)
	{
		return members
			.SelectMany(member => member.Location
				.LayerCharacters(member.RoomLayer)
				.Concat(member.SeenTargets.OfType<ICharacter>())
				.Where(target => AnimalAI.CanGroupObserveTarget(member, target) &&
				                 AnimalAI.CanGroupHuntTarget(member, target)))
			.DistinctPhysicalInstances()
			.Where(x => !group.GroupMembers.ContainsPhysicalInstance(x))
			.Where(x => !group.GroupMembers.Any(y => y.Race.SameRace(x.Race)))
			.Where(x => group.ConsidersThreat(x, group.Alertness))
			.ToList();
	}

	/// <summary>
	/// Wildlife groups normally contain animals with the same stock activity policy. If all live
	/// members require rest, group activity cannot override that authoritative individual policy.
	/// Mixed custom groups continue to use their configured group schedule.
	/// </summary>
	private static bool MemberRequiresActivityRest(ICharacter member)
	{
		return member is INPC npc &&
		       npc.AIs.OfType<AnimalAI>().Any(ai => ai.IsActivityRestRequired(member));
	}

	private void HandleThreats(IGroupAI group, List<ICharacter> members, ICharacter leader,
		List<ICharacter> threats)
	{
		if (group.Alertness < GroupAlertness.Agitated)
		{
			group.Alertness = GroupAlertness.Agitated;
		}

		switch (Tactic)
		{
			case WildlifeGroupTactic.Timid:
				SetAction(group, GroupAction.Flee);
				MoveAway(leader, group, threats);
				foreach (ICharacter member in members)
				{
					member.CombatStrategyMode = Combat.CombatStrategyMode.Flee;
				}

				break;
			case WildlifeGroupTactic.Defensive:
				if (group.CurrentAction != GroupAction.Posture)
				{
					SetAction(group, GroupAction.Posture);
					break;
				}

				if (HasYoung(group, members))
				{
					SetAction(group, GroupAction.ControlledRetreat);
					FocusAttack(group, members.Where(x => !ReferenceEquals(x, leader)), threats);
					GatherStragglers(group, members, leader);
					MoveAway(leader, group, threats);
					break;
				}

				SetAction(group, GroupAction.AttackThreats);
				FocusAttack(group, members, threats);
				break;
			case WildlifeGroupTactic.Territorial:
				if (group.CurrentAction != GroupAction.Posture)
				{
					SetAction(group, GroupAction.Posture);
					break;
				}

				SetAction(group, GroupAction.AttackThreats);
				FocusAttack(group, members, threats);
				break;
			case WildlifeGroupTactic.Hunting:
			case WildlifeGroupTactic.Scavenging:
				SetAction(group, GroupAction.AttackThreats);
				FocusAttack(group, members, threats);
				break;
			default:
				SetAction(group, GroupAction.AvoidThreat);
				MoveAway(leader, group, threats);
				break;
		}
	}

	private void FocusAttack(IGroupAI group, IEnumerable<ICharacter> members, IEnumerable<ICharacter> threats)
	{
		ICharacter? target = threats.GetRandomElement();
		if (target is null)
		{
			return;
		}

		foreach (ICharacter member in members.Where(x => RoleFor(group, x) != GroupRole.Child &&
		                                                  x.Combat is null && AnimalAI.CanGroupObserveTarget(x, target) &&
		                                                  (!Tactic.In(WildlifeGroupTactic.Hunting,
				                                                   WildlifeGroupTactic.Scavenging) ||
			                                           AnimalAI.CanGroupHuntTarget(x, target))))
		{
			PredatorAIHelpers.CheckForAttack(member, target, Gameworld.AlwaysTrueProg,
				"1d600+900", string.Empty, false);
		}
	}

	/// <summary>
	/// The leader scouts the route while the leader and any elders act as sentries. This makes the
	/// existing persisted leadership and age-derived roles useful to every finished wildlife group
	/// without inventing a second incompatible role system.
	/// </summary>
	private void CoordinateSentriesAndScout(IGroupAI group, IEnumerable<ICharacter> members, ICharacter leader)
	{
		if (!ControlScope.HasFlag(GroupAIControlScope.Senses))
		{
			return;
		}

		List<ICharacter> sentries = members.Where(x => ReferenceEquals(x, leader) ||
		                                           RoleFor(group, x) == GroupRole.Elder).ToList();
		List<ICharacter> sightings = [];
		foreach (ICharacter sentry in sentries)
		{
			if (sentry is not INPC npc)
			{
				continue;
			}

			foreach (AnimalAI animalAi in npc.AIs.OfType<AnimalAI>())
			{
				sightings.AddRange(animalAi.AcquireRangedTargets(sentry));
			}
		}

		foreach (ICharacter target in sightings.DistinctPhysicalInstances())
		{
			foreach (INPC member in members.OfType<INPC>())
			{
				foreach (AnimalAI animalAi in member.AIs.OfType<AnimalAI>())
				{
					animalAi.ReceiveGroupSighting(member, target);
				}
			}
		}

		bool sightedThreat = sentries
			.SelectMany(x => x.Location
				.LayerCharacters(x.RoomLayer)
				.Concat(x.SeenTargets.OfType<ICharacter>())
				.Where(y => AnimalAI.CanGroupObserveTarget(x, y)))
			.OfType<ICharacter>()
			.DistinctPhysicalInstances()
			.Where(x => !group.GroupMembers.ContainsPhysicalInstance(x))
			.Where(x => !group.GroupMembers.Any(y => y.Race.SameRace(x.Race)))
			.Any(x => group.Template.ConsidersThreat(x, GroupAlertness.Wary));
		if (!sightedThreat || group.Alertness >= GroupAlertness.Wary)
		{
			return;
		}

		group.Alertness = GroupAlertness.Wary;
		SetAction(group, GroupAction.Alert);
	}

	private static bool HasYoung(IGroupAI group, IEnumerable<ICharacter> members)
	{
		return members.Any(x => RoleFor(group, x) == GroupRole.Child);
	}

	private static GroupRole RoleFor(IGroupAI group, ICharacter member)
	{
		return group.GroupRoles.TryGetValue(member, out GroupRole role) ? role : GroupRole.Adult;
	}

	private void MoveAway(ICharacter leader, IGroupAI group, IEnumerable<ICharacter> threats)
	{
		HashSet<ICell> threatCells = threats.Select(x => x.Location).ToHashSet();
		ICellExit? exit = leader.Location.ExitsFor(leader)
			.Where(CanMoveExitFunctionFor(leader, group))
			.Where(x => !threatCells.Contains(x.Destination))
			.GetRandomElement();
		if (exit is not null && leader.CanMove(exit))
		{
			leader.Move(exit);
		}
	}

	private void GatherStragglers(IGroupAI group, IEnumerable<ICharacter> members, ICharacter leader)
	{
		foreach (ICharacter member in members.Where(x => !ReferenceEquals(x, leader) && x.Location != leader.Location))
		{
			if (IgnoreTickAI(member) || member.AffectedBy<FollowingPath>())
			{
				continue;
			}

			PathMemberToLocation(member, group, leader.Location);
		}
	}

	private bool PathMemberToLocation(ICharacter member, IGroupAI group, ICell destination)
	{
		List<ICellExit> path = member.PathBetween(destination, (uint)MovementRange,
			CanMoveExitFunctionFor(member, group)).ToList();
		if (!path.Any())
		{
			return false;
		}

		FollowingPath effect = new(member, path);
		member.AddEffect(effect);
		effect.FollowPathAction();
		return true;
	}

	private static bool HasUrgentSurvivalNeed(ICharacter character)
	{
		return character.NeedsModel.Status.IsHungry() || character.NeedsModel.Status.IsThirsty();
	}

	private static void AwakenMembersForUrgentSurvivalNeed(IEnumerable<ICharacter> members)
	{
		foreach (ICharacter member in members.Where(x => HasUrgentSurvivalNeed(x) && x.State.IsAsleep()))
		{
			member.Awaken();
		}
	}

	/// <summary>
	/// Lets a group that owns Feeding consume the same live forage yields as an individual
	/// AnimalAI. Each successful bite depletes the cell normally; when a hungry or thirsty member
	/// can no longer feed locally, the leader moves the group to a reachable preferred patch.
	/// </summary>
	private bool CoordinateForaging(IGroupAI group, IEnumerable<ICharacter> members, ICharacter leader,
		WildlifeGroupData data)
	{
		List<ICharacter> hungry = members.Where(x => x.NeedsModel.Status.IsHungry()).ToList();
		List<ICharacter> thirsty = members.Where(x => x.NeedsModel.Status.IsThirsty()).ToList();
		if (!hungry.Any() && !thirsty.Any())
		{
			return false;
		}

		bool consumedForage = false;
		foreach (ICharacter member in hungry)
		{
			consumedForage |= ForagerAIHelpers.TrySatisfyHunger(member);
		}

		foreach (ICharacter member in thirsty)
		{
			_ = NpcSurvivalAIHelpers.TryDrinkIfThirsty(member) ||
			    NpcSurvivalAIHelpers.TryHydrateFromAquaticEnvironmentIfThirsty(member, false) ||
			    NpcSurvivalAIHelpers.TryHydrateFromAquaticEnvironmentIfThirsty(member, true);
		}

		if (consumedForage)
		{
			data.RecordForage(leader.Location);
			group.Changed = true;
			SetAction(group, GroupAction.Graze);
			return true;
		}

		if (!members.Any(HasUrgentSurvivalNeed))
		{
			SetAction(group, GroupAction.Graze);
			return true;
		}

		ICellExit? destination = leader.Location.ExitsFor(leader)
			.Where(CanMoveExitFunctionFor(leader, group))
			.Where(x => PreferredCellProg.ExecuteBool(false, leader, x.Destination))
			.Where(x => !data.WasRecentlyForaged(x.Destination))
			.Where(x => members.Any(member =>
				ForagerAIHelpers.HasFoodOpportunity(member, x.Destination) ||
				NpcSurvivalAIHelpers.HasWaterSource(member, x.Destination) ||
				NpcSurvivalAIHelpers.HasAquaticWaterSource(member, x.Destination, false)))
			.GetRandomElement();
		if (destination is null || !leader.CanMove(destination))
		{
			return false;
		}

		SetAction(group, GroupAction.FindFood);
		leader.Move(destination);
		return true;
	}

	private static GroupAIControlScope DefaultScopeFor(WildlifeGroupTactic tactic)
	{
		return tactic switch
		{
			WildlifeGroupTactic.Managed => GroupAIControlScope.Movement | GroupAIControlScope.Activity |
			                                 GroupAIControlScope.Shelter | GroupAIControlScope.Senses,
			WildlifeGroupTactic.Roosting => GroupAIControlScope.Movement | GroupAIControlScope.Activity |
			                                  GroupAIControlScope.Shelter | GroupAIControlScope.Senses,
			WildlifeGroupTactic.Aquatic or WildlifeGroupTactic.Amphibious or WildlifeGroupTactic.SeasonalGrazing =>
				GroupAIControlScope.Movement | GroupAIControlScope.Activity | GroupAIControlScope.Senses,
			_ => GroupAIControlScope.Movement | GroupAIControlScope.Threats |
			     GroupAIControlScope.Activity | GroupAIControlScope.Senses
		};
	}

	private static GroupAIControlScope ParseScope(string? value, GroupAIControlScope fallback)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return fallback;
		}

		return Enum.TryParse(value, true, out GroupAIControlScope scope) ? scope : fallback;
	}

	private void SetAction(IGroupAI group, GroupAction action)
	{
		if (group.CurrentAction != action)
		{
			group.CurrentAction = action;
		}
	}

	private bool SetEnum<TEnum>(ICharacter actor, StringStack command, Action<TEnum> setter, string label)
		where TEnum : struct, Enum
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must specify a {label}. Valid values are {Enum.GetValues<TEnum>().ListToColouredString()}.");
			return false;
		}

		string valueText = command.SafeRemainingArgument;
		if (!valueText.TryParseEnum(out TEnum value))
		{
			actor.OutputHandler.Send($"The text {valueText.ColourCommand()} is not a valid {label}. Valid values are {Enum.GetValues<TEnum>().ListToColouredString()}.");
			return false;
		}

		setter(value);
		actor.OutputHandler.Send($"This wildlife group will now use {value.DescribeEnum().ColourName()} for its {label}.");
		return true;
	}

	private bool SetScope(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must enter one or more comma-separated group AI control scopes. Valid values are {Enum.GetValues<GroupAIControlScope>().ListToColouredString()}.");
			return false;
		}

		string scopeText = command.SafeRemainingArgument;
		if (!Enum.TryParse(scopeText, true, out GroupAIControlScope scope))
		{
			actor.OutputHandler.Send($"The text {scopeText.ColourCommand()} is not a valid group AI control scope. Valid values are {Enum.GetValues<GroupAIControlScope>().ListToColouredString()}.");
			return false;
		}

		ControlScope = scope;
		actor.OutputHandler.Send($"This wildlife group now controls {scope.ToString().ColourValue()}.");
		return true;
	}

	private bool SetCellProg(ICharacter actor, StringStack command, Action<IFutureProg> setter, string label)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which prog should identify {label} cells?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[]
			{
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Location }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		setter(prog);
		actor.OutputHandler.Send($"This wildlife group will now use {prog.MXPClickableFunctionName()} for {label}.");
		return true;
	}

	private bool SetRange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int range) || range < 1)
		{
			actor.OutputHandler.Send("You must specify a positive movement range.");
			return false;
		}

		MovementRange = range;
		actor.OutputHandler.Send($"This wildlife group will now use a {range.ToString("N0", actor).ColourValue()} cell movement range.");
		return true;
	}

	private bool SetWanderChance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !TerritorialWanderer.TryParseWanderChance(command.SafeRemainingArgument, out double chance))
		{
			actor.OutputHandler.Send("You must specify a percentage between 0% and 100%.");
			return false;
		}

		WanderChancePerMinute = chance;
		actor.OutputHandler.Send($"This wildlife group will now have a {chance.ToString("P2", actor).ColourValue()} movement chance each minute.");
		return true;
	}

	private bool SetActivity(ICharacter actor, StringStack command)
	{
		(bool success, string error, IEnumerable<TimeOfDay> activeTimes) =
			ParseBuilderArgument(command.SafeRemainingArgument.ToLowerInvariant());
		if (!success)
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		ActiveTimesOfDay.Clear();
		ActiveTimesOfDay.AddRange(activeTimes);
		actor.OutputHandler.Send($"This wildlife group will now be {GroupActivityTimeDescription.ColourName()}.");
		return true;
	}

	private sealed class WildlifeGroupData : BaseGroupTypeData
	{
		private long _homeCellId;
		private long _lastForageCellId;
		private DateTime _lastForageUtc;
		public ICell? HomeCell
		{
			get => _homeCellId > 0 ? Gameworld.Cells.Get(_homeCellId) : null;
			set => _homeCellId = value?.Id ?? 0;
		}

		public WildlifeGroupData(IFuturemud gameworld) : base(gameworld)
		{
			_lastForageUtc = DateTime.MinValue;
		}

		public WildlifeGroupData(XElement root, IFuturemud gameworld) : base(root, gameworld)
		{
			_homeCellId = long.Parse(root.Element("HomeCellId")?.Value ?? "0");
			_lastForageCellId = long.Parse(root.Element("LastForageCellId")?.Value ?? "0");
			_lastForageUtc = DateTime.TryParse(root.Element("LastForageUtc")?.Value, out DateTime parsed)
				? parsed
				: DateTime.MinValue;
		}

		public void RecordForage(ICell cell)
		{
			_lastForageCellId = cell.Id;
			_lastForageUtc = DateTime.UtcNow;
		}

		public bool WasRecentlyForaged(ICell cell)
		{
			return cell.Id == _lastForageCellId && DateTime.UtcNow - _lastForageUtc < TimeSpan.FromMinutes(5);
		}

		public override XElement SaveToXml()
		{
			XElement root = base.SaveToXml();
			root.Add(new XElement("HomeCellId", _homeCellId));
			root.Add(new XElement("LastForageCellId", _lastForageCellId));
			root.Add(new XElement("LastForageUtc", _lastForageUtc.ToString("o")));
			return root;
		}

		public override string ShowText(ICharacter voyeur)
		{
			return base.ShowText(voyeur) +
			       $"Home / Roost: {HomeCell?.GetFriendlyReference(voyeur).ColourName() ?? "None".ColourError()}\n" +
			       $"Last Forage Cell: {_lastForageCellId.ToString("N0", voyeur).ColourValue()}\n";
		}
	}
}
