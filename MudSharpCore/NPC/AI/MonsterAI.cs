#nullable enable

using System.Globalization;
using MudSharp.Body.Position;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.GameItems;
using MudSharp.Magic.Powers;
using ArtificialIntelligence = MudSharp.Models.ArtificialIntelligence;
using MudSharp.NPC.AI.Groups.GroupTypes;

namespace MudSharp.NPC.AI;

/// <summary>An individual creature controller whose motives are independent of an ecological life cycle.</summary>
public sealed partial class MonsterAI : CreatureAIBase
{
	public HashSet<MonsterMotive> Motives { get; } = [];
	public MonsterActivityWindow ActivityWindow { get; private set; } = new();
	public MonsterFeedingMode Feeding { get; private set; }
	public bool SameRaceAllies { get; private set; } = true;
	public long AllyProgId { get; private set; }
	public bool DefenceUsesWindow { get; private set; }
	public bool TrapProvokes { get; private set; }
	public bool ReturnHome { get; private set; } = true;
	public int GuardRange { get; private set; }
	public TimeSpan WarningDelay { get; private set; } = TimeSpan.FromSeconds(20);
	public TimeSpan ProvocationDuration { get; private set; } = TimeSpan.FromMinutes(5);
	public TimeSpan Cooldown { get; private set; } = TimeSpan.FromMinutes(1);
	public int FeedingBites { get; private set; } = 3;
	public TimeSpan FeedingDuration { get; private set; } = TimeSpan.FromSeconds(30);
	private string? _loadError;

	internal MonsterAI(ArtificialIntelligence ai, IFuturemud world) : base(ai, world) { }
	private MonsterAI(IFuturemud world, string name) : base(world, name, "Monster")
	{
		SetDefaults();
		DatabaseInitialise();
	}
	private MonsterAI() { }
	public static void RegisterLoader()
	{
		RegisterAIType("Monster", (ai, world) => new MonsterAI(ai, world));
		RegisterAIBuilderInformation("monster", (world, name) => new MonsterAI(world, name), new MonsterAI().HelpText);
	}
	private void SetDefaults()
	{
		SetCreatureDefaults();
		WaterStrategy = AnimalWaterStrategyType.Off;
		WanderChancePerMinute = 0;
		Hunting = new AnimalHuntingSettings { Enabled = true, People = AnimalPeoplePreyPolicy.Eligible, StarvationAdjustment = 0 };
	}
	protected override void LoadFromXML(XElement root)
	{
		SetDefaults();
		base.LoadFromXML(root);
		LoadCreatureDefinition(root);
		if (root.Element("Hunting") is null)
			Hunting = new AnimalHuntingSettings { Enabled = true, People = AnimalPeoplePreyPolicy.Eligible, StarvationAdjustment = 0 };
		var xml = root.Element("Monster") ?? new XElement("Monster");
		_loadError = xml.Element("LoadError")?.Value;
		if (xml.Attribute("version")?.Value is { } version && version != "1") _loadError = "unsupported Monster definition version";
		Motives.Clear();
		foreach (var element in xml.Elements("Motive"))
			if (Enum.TryParse<MonsterMotive>(element.Value, true, out var motive) && Enum.IsDefined(motive) &&
			    motive is not (MonsterMotive.None or MonsterMotive.SelfDefence)) Motives.Add(motive);
			else _loadError = "invalid configured monster motive";
		ActivityWindow = MonsterActivityWindow.Load(xml.Element("ActivityWindow"));
		bool Flag(string name, bool fallback) => bool.TryParse(xml.Element(name)?.Value, out var value) ? value : fallback;
		double Number(string name, double fallback, double min, double max) => double.TryParse(xml.Element(name)?.Value,
			NumberStyles.Float, CultureInfo.InvariantCulture, out var value) && double.IsFinite(value) ? Math.Clamp(value, min, max) : fallback;
		Feeding = ParseEnum(xml.Element("Feeding")?.Value, MonsterFeedingMode.Off);
		SameRaceAllies = Flag("SameRaceAllies", true); DefenceUsesWindow = Flag("DefenceUsesWindow", false);
		TrapProvokes = Flag("TrapProvokes", false); ReturnHome = Flag("ReturnHome", true);
		AllyProgId = long.TryParse(xml.Element("AllyProg")?.Value, out var id) ? id : 0;
		GuardRange = (int)Number("GuardRange", 0, 0, 20);
		WarningDelay = TimeSpan.FromSeconds(Number("WarningSeconds", 20, 0, 3600));
		ProvocationDuration = TimeSpan.FromSeconds(Number("ProvocationSeconds", 300, 1, 86400));
		Cooldown = TimeSpan.FromSeconds(Number("CooldownSeconds", 60, 0, 86400));
		FeedingBites = (int)Number("FeedingBites", 3, 1, 100);
		FeedingDuration = TimeSpan.FromSeconds(Number("FeedingSeconds", 30, 1, 3600));
		EngageDelayDiceExpression = xml.Element("EngageDelay")?.Value ?? "1000+1d1000";
		EngageEmote = xml.Element("EngageEmote")?.Value ?? "";
		PostureEmote = xml.Element("WarningEmote")?.Value ?? "";
		// The shared movement format defaults to wildlife roaming; an absent Monster movement section stays idle.
		if (root.Element("Movement") is null) WanderChancePerMinute = 0;
	}
	internal XElement SaveDefinition() => new("Definition", SaveCreatureDefinition(),
		new XElement("Monster", new XAttribute("version", 1), Motives.Order().Select(x => new XElement("Motive", x)),
			ActivityWindow.Save(), new XElement("Feeding", Feeding), new XElement("SameRaceAllies", SameRaceAllies),
			new XElement("AllyProg", AllyProgId), new XElement("DefenceUsesWindow", DefenceUsesWindow),
			new XElement("TrapProvokes", TrapProvokes), new XElement("ReturnHome", ReturnHome), new XElement("GuardRange", GuardRange),
			new XElement("WarningSeconds", WarningDelay.TotalSeconds), new XElement("ProvocationSeconds", ProvocationDuration.TotalSeconds),
			new XElement("CooldownSeconds", Cooldown.TotalSeconds), new XElement("FeedingBites", FeedingBites),
			new XElement("FeedingSeconds", FeedingDuration.TotalSeconds), new XElement("EngageDelay", EngageDelayDiceExpression),
			new XElement("EngageEmote", new XCData(EngageEmote)), new XElement("WarningEmote", new XCData(PostureEmote)),
			_loadError is null ? null : new XElement("LoadError", _loadError)),
		new XElement("OpenDoors", OpenDoors), new XElement("UseKeys", UseKeys), new XElement("SmashLockedDoors", SmashLockedDoors),
		new XElement("CloseDoorsBehind", CloseDoorsBehind), new XElement("UseDoorguards", UseDoorguards),
		new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay));
	protected override string SaveToXml() => SaveDefinition().ToString();
	public override bool CountsAsAggressive => Hunting.Enabled && Motives.Any();
	public override bool IsReadyToBeUsed => ConfigurationError() is null;
	internal string? ConfigurationError()
	{
		if (_loadError is not null) return _loadError;
		if (ActivityWindow.ConfigurationError(Gameworld) is { } error) return error;
		if (Motives.Contains(MonsterMotive.Condition) && ActivityWindow.ConditionProgId == 0) return "the Condition motive requires an activity condition prog";
		if (Motives.Contains(MonsterMotive.Hunger) && Feeding != MonsterFeedingMode.Needs) return "the Hunger motive requires feeding Needs";
		if (AllyProgId != 0)
		{
			var prog = Gameworld.FutureProgs.Get(AllyProgId);
			if (prog is null || !prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) ||
			    !prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character]))
				return "the ally prog must return Boolean and accept monster and target characters";
		}
		var readiness = HuntingReadiness();
		return readiness.Ready ? null : readiness.Reason;
	}
	internal string? ControllerConflict(ICharacter actor)
	{
		if (actor is not INPC npc) return null;
		if (npc.AIs.OfType<CreatureAIBase>().Count() != 1 || !npc.AIs.Contains(this))
			return "attach exactly one primary Animal or Monster AI";
		return npc.GroupAI?.GroupAIType is WildlifeGroupAIType ? "Monster AI cannot participate in a Wildlife group" : null;
	}
	internal static string? AttachmentError(IEnumerable<IArtificialIntelligence> existing, IArtificialIntelligence added)
	{
		var primaries = existing.OfType<CreatureAIBase>().ToList();
		return added is CreatureAIBase && (added is MonsterAI && primaries.Count > 0 || primaries.OfType<MonsterAI>().Any())
			? "Use exactly one primary Animal or Monster AI. Remove the current creature controller before adding this one."
			: null;
	}
	private MonsterStateEffect? State(ICharacter actor) => actor.EffectsOfType<MonsterStateEffect>().FirstOrDefault(x => x.AiId == Id);
	private MonsterStateEffect EnsureState(ICharacter actor)
	{
		if (State(actor) is { } state) return state;
		state = new MonsterStateEffect(actor, Id);
		actor.AddEffect(state);
		return state;
	}
	protected override bool IsSociallyTrusted(ICharacter actor, ICharacter target) => ReferenceEquals(actor, target) ||
		SameRaceAllies && actor.Race.SameRace(target.Race) ||
		AllyProgId != 0 && Gameworld.FutureProgs.Get(AllyProgId)?.ExecuteBool(false, actor, target) == true;
	protected override bool TryGuardingAttack(ICharacter actor, ICharacter target) => BeginHunt(actor, target);
	private bool InDefendedArea(ICharacter actor, ICharacter target)
	{
		var home = ResolveHomeBase(actor).HomeRoom;
		if (home is null) return false;
		// Called only for an observed target. Radius is topology distance, with zero meaning the home room.
		return target.DistanceBetween(home, (uint)GuardRange) is var distance && distance >= 0 && distance <= GuardRange;
	}
	internal MonsterMotive SelectMotive(ICharacter actor, ICharacter target)
	{
		if (!CanObserveTarget(actor, target)) return MonsterMotive.None;
		if (target.CombatTarget == actor) return MonsterMotive.SelfDefence;
		var state = State(actor);
		var active = ActivityWindow.InactiveReason(actor) is null;
		if (state?.CooldownUntil > RuntimeClock.UtcNow) return MonsterMotive.None;
		if ((!DefenceUsesWindow || active) && Motives.Contains(MonsterMotive.Provocation) &&
		    state?.ProvokerId == target.Id && state.ProvokedUntil > RuntimeClock.UtcNow) return MonsterMotive.Provocation;
		if ((!DefenceUsesWindow || active) && Motives.Contains(MonsterMotive.Territory) && InDefendedArea(actor, target)) return MonsterMotive.Territory;
		if (!active || state?.CooldownUntil > RuntimeClock.UtcNow) return MonsterMotive.None;
		if (Motives.Contains(MonsterMotive.Condition)) return MonsterMotive.Condition;
		if (Motives.Contains(MonsterMotive.Scheduled)) return MonsterMotive.Scheduled;
		if (Motives.Contains(MonsterMotive.Hunger) && PredatorAIHelpers.IsHungry(actor)) return MonsterMotive.Hunger;
		return MonsterMotive.None;
	}
	protected override string? HuntPolicyRejection(ICharacter actor, ICharacter target, bool continuing)
	{
		if (ControllerConflict(actor) is { } conflict) return conflict;
		if (ConfigurationError() is { } error) return error;
		var intent = actor.EffectsOfType<MonsterIntentEffect>().FirstOrDefault(x => x.AiId == Id && x.TargetId == target.Id);
		var motive = continuing && intent is not null ? intent.Motive : SelectMotive(actor, target);
		if (motive == MonsterMotive.None) return "no active motive for this target";
		if (intent is not null && HuntPolicyExpired(actor, intent)) return "the intent's motive or leash no longer permits pursuit";
		if (motive == MonsterMotive.Hunger && !PredatorAIHelpers.CouldEatAfterKilling(actor, target)) return "inedible prey for hunger motive";
		return null;
	}
	protected override bool CanSeekHunt(ICharacter actor) => Hunting.Enabled && ConfigurationError() is null &&
		ControllerConflict(actor) is null && (Feeding != MonsterFeedingMode.Needs || !NpcSurvivalAIHelpers.IsThirsty(actor)) &&
		(Motives.Any() || actor.CombatTarget is not null);
	protected override CreaturePursuitEffect CreateHunt(ICharacter actor, ICharacter target) => new MonsterIntentEffect(actor, this, target, SelectMotive(actor, target));
	protected override bool HuntPolicyExpired(ICharacter actor, CreaturePursuitEffect hunt)
	{
		if (!Hunting.Enabled || ControllerConflict(actor) is not null || ConfigurationError() is not null || hunt is not MonsterIntentEffect intent)
			return true;
		if (intent.Motive == MonsterMotive.None) return true;
		if (Feeding == MonsterFeedingMode.Needs && NpcSurvivalAIHelpers.IsThirsty(actor)) return true;
		if (intent.Motive == MonsterMotive.SelfDefence) return actor.CombatTarget is not ICharacter opponent || opponent.Id != hunt.TargetId;
		if (!Motives.Contains(intent.Motive)) return true;
		if ((intent.Motive is MonsterMotive.Scheduled or MonsterMotive.Condition or MonsterMotive.Hunger || DefenceUsesWindow) &&
		    ActivityWindow.InactiveReason(actor) is not null) return true;
		if (intent.Motive == MonsterMotive.Provocation && (State(actor)?.ProvokerId != hunt.TargetId || State(actor)?.ProvokedUntil <= RuntimeClock.UtcNow)) return true;
		if (intent.Motive == MonsterMotive.Hunger && (!PredatorAIHelpers.IsHungry(actor) || NpcSurvivalAIHelpers.IsThirsty(actor))) return true;
		if (intent.Motive == MonsterMotive.Territory)
		{
			var home = ResolveHomeBase(actor).HomeRoom;
			if (home is null || actor.DistanceBetween(home, (uint)Hunting.PursuitRange) < 0) return true;
		}
		return false;
	}
	protected override void ClearHuntPaths(ICharacter actor)
	{
		actor.RemoveAllEffects<BreakDownDoor>(x => ReferenceEquals(x.PathingEpisode?.PathingOwner, this), true);
		actor.RemoveAllEffects<FollowingPath>(x => ReferenceEquals(x.PathingOwner, this), true);
	}
	internal void Detach(ICharacter actor)
	{
		ClearHuntPaths(actor);
		actor.RemoveAllEffects<CreatureEngagementDelay>(x => x.AiId == Id, true);
		actor.RemoveAllEffects<MonsterIntentEffect>(x => x.AiId == Id, true);
		actor.RemoveAllEffects<MonsterStateEffect>(x => x.AiId == Id, true);
	}
	internal void FinishIntent(ICharacter actor, CreaturePursuitEffect hunt, string reason)
	{
		ClearHuntPaths(actor);
		actor.RemoveAllEffects<CreatureEngagementDelay>(x => x.AiId == Id, true);
		actor.RemoveEffect(hunt);
		EnsureState(actor).Finish(Cooldown, reason);
	}
	protected override void EndHunt(ICharacter actor, CreaturePursuitEffect hunt) => FinishIntent(actor, hunt, "pursuit ended");
	protected override bool HuntCompleted(ICharacter actor, CreaturePursuitEffect hunt)
	{
		FinishIntent(actor, hunt, "target died");
		if (Feeding == MonsterFeedingMode.Needs) return PredatorAIHelpers.EatLocalCorpseIfHungry(actor);
		if (Feeding == MonsterFeedingMode.AfterKill && hunt.Target?.Corpse is { } corpse)
			EnsureState(actor).Feed(corpse.Parent.Id, FeedingBites, FeedingDuration);
		return TryFeed(actor);
	}
	private bool TryFeed(ICharacter actor)
	{
		if (actor.Combat is not null || actor.Movement is not null || !CharacterState.Able.HasFlag(actor.State) ||
		    actor.CombinedEffectsOfType<IEffect>().Any(x => x.IsBlockingEffect("general") || x.IsBlockingEffect("combat-engage"))) return false;
		if (Feeding != MonsterFeedingMode.AfterKill || State(actor) is not { BitesRemaining: > 0 } state ||
		    state.FeedUntil <= RuntimeClock.UtcNow) return false;
		var corpse = actor.Location.LayerGameItems(actor.RoomLayer).SelectMany(x => x.ShallowAccessibleItems(actor))
			.FirstOrDefault(x => x.Id == state.CorpseId)?.GetItemType<ICorpse>();
		if (corpse is null || !actor.CanEat(corpse, actor.Race.BiteWeight).Success) { state.Feed(0, 0, TimeSpan.Zero); return false; }
		state.AteBite();
		actor.SetTarget(corpse.Parent); actor.SetModifier(PositionModifier.None); actor.SetEmote(null);
		actor.Eat(corpse, actor.Race.BiteWeight, null);
		return true;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var actor = CharacterForEvent(type, arguments);
		if (actor is null || actor.State.IsDead() || actor.State.IsInStatis()) return false;
		if (ControllerConflict(actor) is not null) { Detach(actor); return false; }
		if (type == EventType.NPCOnGameLoadFinished)
		{
			// Validate durable intent only. Loading never initiates an attack or feeds a corpse.
			foreach (var saved in actor.EffectsOfType<MonsterIntentEffect>().Where(x => x.AiId == Id).ToList())
				if (saved.Target is null || HuntExpired(actor, saved)) FinishIntent(actor, saved, "saved intent is no longer valid");
			return false;
		}
		if (type == EventType.EngagedInCombat || type == EventType.TrapCaughtPrey && TrapProvokes)
		{
			ICharacter? target = (type == EventType.EngagedInCombat ? arguments[0] : arguments[1]) as ICharacter;
			if (target is not null && CanObserveTarget(actor, target) && !IsSociallyTrusted(actor, target))
				EnsureState(actor).Provoke(target.Id, ProvocationDuration);
			if (type == EventType.EngagedInCombat) { HandleCombatAwareness(actor); return false; }
		}
		var hunt = actor.EffectsOfType<MonsterIntentEffect>().FirstOrDefault(x => x.AiId == Id);
		if (hunt is not null)
		{
			// Revalidate before every path callback, including deferred door actions, not only heartbeat ticks.
			if (HuntExpired(actor, hunt))
			{
				hunt.Phase = AnimalHuntPhase.Abandoned;
				ClearHuntPaths(actor);
				return AdvanceHunt(actor, hunt);
			}
			if (type.In(EventType.FiveSecondTick, EventType.TenSecondTick, EventType.MinuteTick, EventType.LeaveCombat,
				EventType.CharacterStopMovement, EventType.CharacterEnterRoomFinish, EventType.CharacterEnterRoomWitness,
				EventType.CharacterDiesWitness, EventType.TrapCaughtPrey))
				return AdvanceHunt(actor, hunt);
			return base.HandleEvent(type, arguments);
		}
		if (!CharacterState.Able.HasFlag(actor.State) || actor.Combat is not null || actor.Movement is not null ||
		    actor.CombinedEffectsOfType<IEffect>().Any(x => x.IsBlockingEffect("general") || x.IsBlockingEffect("movement") || x.IsBlockingEffect("combat-engage"))) return false;
		if (type.In(EventType.TenSecondTick, EventType.MinuteTick, EventType.CharacterEnterRoomWitness, EventType.TrapCaughtPrey,
			EventType.LeaveCombat, EventType.CharacterDiesWitness))
		{
			if (Feeding == MonsterFeedingMode.Needs && (NpcSurvivalAIHelpers.TryDrinkIfThirsty(actor) || PredatorAIHelpers.EatLocalCorpseIfHungry(actor))) return true;
			if (TryFeed(actor)) return true;
			if (TryAwarenessResponse(actor, null)) return true;
			if (TryStartIntent(actor, type)) return true;
			if (type == EventType.TenSecondTick && ActivityWindow.InactiveReason(actor) is null && EvaluateCreatureSenses(actor)) return true;
		}
		if (type == EventType.MinuteTick) HomeStrategyHandler.Evaluate(this, actor);
		return base.HandleEvent(type, arguments);
	}
	private bool TryStartIntent(ICharacter actor, EventType type)
	{
		if (!CanSeekHunt(actor)) return false;
		var active = ActivityWindow.InactiveReason(actor) is null;
		var needsDefensiveScan = Motives.Contains(MonsterMotive.Territory) && (!DefenceUsesWindow || active) ||
		                        State(actor)?.ProvokedUntil > RuntimeClock.UtcNow;
		if (!active && !needsDefensiveScan) return false;
		if (!needsDefensiveScan && State(actor)?.CooldownUntil > RuntimeClock.UtcNow) return false;
		if (Hunting.Opening != AnimalHuntOpening.Direct && PrepareHuntingSite(actor)) return true;
		if (type.In(EventType.TenSecondTick, EventType.MinuteTick)) AcquireRangedTargets(actor);
		// The shared hunt visibility gate also admits visible local targets on another layer.
		// Awareness's same-layer enumeration would strand submerged and aerial ambushers.
		var target = RankPrey(actor, actor.Location.Characters.Concat(actor.SeenTargets.OfType<ICharacter>()))
			.OrderBy(x => SelectMotive(actor, x)).FirstOrDefault();
		if (target is null) { if (State(actor) is { WarningTargetId: > 0 } state) state.Warn(0, TimeSpan.Zero); return false; }
		if (SelectMotive(actor, target) == MonsterMotive.Territory && WarningDelay > TimeSpan.Zero)
		{
			var state = EnsureState(actor);
			if (state.WarningTargetId != target.Id) { state.Warn(target.Id, WarningDelay); EmitPosture(actor, target); return true; }
			if (state.WarningUntil > RuntimeClock.UtcNow) return true;
		}
		return BeginHunt(actor, target);
	}
	public override bool HandlesEvent(params EventType[] types) => types.Any(x => x.In(EventType.TenSecondTick,
		EventType.CharacterEnterRoomWitness, EventType.EngagedInCombat, EventType.CharacterDiesWitness, EventType.TrapCaughtPrey)) || base.HandlesEvent(types);

	protected override bool IsPathingEnabled(ICharacter actor) => ControllerConflict(actor) is null && ConfigurationError() is null &&
		!actor.EffectsOfType<MonsterIntentEffect>().Any(x => x.AiId == Id && (x.Phase == AnimalHuntPhase.Abandoned || HuntExpired(actor, x)));
	protected override bool PermitsPursuitRoom(ICharacter actor, IRoom room)
	{
		var hunt = actor.EffectsOfType<MonsterIntentEffect>().FirstOrDefault(x => x.AiId == Id);
		if (hunt is null) return true;
		var origin = hunt.Motive == MonsterMotive.Territory ? ResolveHomeBase(actor).HomeRoom : Gameworld.Rooms.Get(hunt.OriginRoomId);
		return origin is not null && room.DistanceBetween(origin, (uint)Hunting.PursuitRange) >= 0;
	}
	protected override bool WouldMove(ICharacter actor) => IsPathingEnabled(actor) &&
		(actor.EffectsOfType<MonsterIntentEffect>().Any(x => x.AiId == Id && x.Phase != AnimalHuntPhase.Abandoned) ||
		 // Completing an owned preparation path is independent of starting a random wander.
		 actor.EffectsOfType<FollowingPath>().Any(x => ReferenceEquals(x.PathingOwner, this)) && ActivityWindow.InactiveReason(actor) is null ||
		 Feeding == MonsterFeedingMode.Needs && NpcSurvivalAIHelpers.IsThirsty(actor) ||
		 ReturnHome && !(State(actor)?.ReturnRetryUntil > RuntimeClock.UtcNow) && ResolveHomeBase(actor).HomeRoom is { } home && home != actor.Location ||
		 MovementEnabledProg.ExecuteBool(false, actor) && RandomUtilities.DoubleRandom(0, 1) <= WanderChancePerMinute);
	protected override (IRoom? Target, IEnumerable<IRoomExit>) GetPath(ICharacter actor)
	{
		var hunt = actor.EffectsOfType<MonsterIntentEffect>().FirstOrDefault(x => x.AiId == Id);
		if (hunt is null && Feeding == MonsterFeedingMode.Needs && NpcSurvivalAIHelpers.IsThirsty(actor))
			return NpcSurvivalAIHelpers.GetPathToWater(actor, GetAnimalSuitabilityFunction(actor), DefaultNeedRange);
		var destination = hunt is not null ? Gameworld.Rooms.Get(hunt.LastRoomId) : ReturnHome ? ResolveHomeBase(actor).HomeRoom : null;
		if (destination is not null && (hunt is not null || destination != actor.Location))
		{
			if (hunt is null && State(actor)?.ReturnRetryUntil > RuntimeClock.UtcNow) return (null, []);
			var path = actor.PathBetween(destination, (uint)(hunt is null ? ReturnSearchRange : Hunting.PursuitRange), GetAnimalSuitabilityFunction(actor)).ToList();
			if (hunt is null && path.Count == 0) EnsureState(actor).DeferHomeReturn();
			return (destination, path);
		}
		return MovementStrategyHandler.GetAmbientPath(this, actor);
	}
	internal int ReturnSearchRange => (int)Math.Min(int.MaxValue, (long)MovementRange + Hunting.PursuitRange + GuardRange);
	protected override bool FollowHuntObservation(ICharacter actor, CreaturePursuitEffect hunt)
	{
		if (Gameworld.Rooms.Get(hunt.LastRoomId) is not { } room) return false;
		if (room != actor.Location || room.RouteDefinition is not null)
		{
			CheckPathingEffect(actor, true);
			return true;
		}
		return base.FollowHuntObservation(actor, hunt);
	}
	protected override (IRoom? Target, ISpatialPath? Path) GetSpatialPath(ICharacter actor)
	{
		var hunt = actor.EffectsOfType<MonsterIntentEffect>().FirstOrDefault(x => x.AiId == Id);
		if (hunt is null || Gameworld.Rooms.Get(hunt.LastRoomId) is not { } room) return (null, null);
		if (room.RouteDefinition is not null && hunt.LastRoutePosition is null) return (null, null);
		var destination = new SpatialLocation(room, hunt.LastLayer, hunt.LastRoutePosition);
		return TryFindSpatialPath(actor, destination, Hunting.PursuitRange, GetAnimalSuitabilityFunction(actor), out var path) ? (room, path) : (null, null);
	}
}
