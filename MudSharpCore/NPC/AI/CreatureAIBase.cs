#nullable enable
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Celestial;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.NPC.AI.Groups;
using MudSharp.Work.Crafts;
using MoreLinq;

namespace MudSharp.NPC.AI;

public abstract partial class CreatureAIBase : PathingAIBase
{
	protected const int DefaultGroundRange = 10;
	protected const int DefaultSwimRange = 15;
	protected const int DefaultFlyRange = 30;
	protected const int DefaultArborealRange = 10;
	protected const int DefaultNeedRange = 20;

	public AnimalMovementStrategyType MovementStrategy { get; protected set; }
	public AnimalHomeStrategyType HomeStrategy { get; protected set; }
	public AnimalWaterStrategyType WaterStrategy { get; protected set; }
	public AnimalAwarenessStrategyType AwarenessStrategy { get; protected set; }
	public AnimalRefugeStrategyType RefugeStrategy { get; protected set; }
	public AnimalSensesStrategyType SensesStrategy { get; protected set; }
	public bool WaterEnabled => WaterStrategy != AnimalWaterStrategyType.Off;

	public IFutureProg MovementEnabledProg { get; protected set; } = null!;
	public IFutureProg MovementCellProg { get; protected set; } = null!;
	public IFutureProg PreferredHabitatProg { get; protected set; } = null!;
	public IFutureProg ToleratedHabitatProg { get; protected set; } = null!;
	public IFutureProg AmphibiousLandCellProg { get; protected set; } = null!;
	public IFutureProg AmphibiousWaterCellProg { get; protected set; } = null!;
	public IFutureProg AllowDescentProg { get; protected set; } = null!;
	public IFutureProg SuitableTerritoryProg { get; protected set; } = null!;
	public IFutureProg DesiredTerritorySizeProg { get; protected set; } = null!;
	public IFutureProg BurrowSiteProg { get; protected set; } = null!;
	public IFutureProg BuildEnabledProg { get; protected set; } = null!;
	public IFutureProg AwarenessThreatProg { get; protected set; } = null!;
	public IFutureProg AwarenessAvoidCellProg { get; protected set; } = null!;
	public IFutureProg RefugeCellProg { get; protected set; } = null!;
	public IFutureProg? HomeLocationProg { get; protected set; }
	public IFutureProg? AnchorItemProg { get; protected set; }
	protected long _burrowCraftId;
	// AIs are loaded before crafts. Retain the reference even during that startup gap.
	public ICraft? BurrowCraft => _burrowCraftId > 0 ? Gameworld.Crafts.Get(_burrowCraftId) : null;
	public int MovementRange { get; protected set; }
	public double AmphibiousWaterBias { get; protected set; }
	public double WanderChancePerMinute { get; protected set; }
	public string WanderEmote { get; protected set; } = string.Empty;
	public string EngageDelayDiceExpression { get; protected set; } = "1000+1d1000";
	public string EngageEmote { get; protected set; } = string.Empty;
	public string PostureEmote { get; protected set; } = string.Empty;
	public string PostureDurationDiceExpression { get; protected set; } = "1d20+20";
	public int AwarenessRange { get; protected set; }
	public int AwarenessMemoryMinutes { get; protected set; }
	public int RefugeReturnSeconds { get; protected set; }
	public RoomLayer TargetFlyingLayer { get; protected set; }
	public RoomLayer TargetRestingLayer { get; protected set; }
	public RoomLayer PreferredTreeLayer { get; protected set; }
	public RoomLayer SecondaryTreeLayer { get; protected set; }
	public RoomLayer RefugeLayer { get; protected set; }
	public bool WillShareTerritory { get; protected set; }
	public bool WillShareTerritoryWithOtherRaces { get; protected set; }
	public bool AllowGroupShelterSharing { get; protected set; }

	protected CreatureAIBase(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld) { }
	protected CreatureAIBase(IFuturemud gameworld, string name, string type) : base(gameworld, name, type) { }
	protected CreatureAIBase() { }

	protected virtual bool SurvivalNeedsSatisfied(ICharacter actor) => true;
	protected abstract bool IsSociallyTrusted(ICharacter actor, ICharacter target);
	protected virtual bool IsWithinPreferredHabitat(ICharacter actor, ICell cell) =>
		PreferredHabitatProg.ExecuteBool(false, actor, cell, actor.Location);
	internal virtual bool CanHuntTarget(ICharacter actor, ICharacter target) =>
		PreyRejection(actor, target) is null && AssessPrey(actor, target).Score >= HuntThreshold(actor, false);
	protected virtual bool TryGuardingAttack(ICharacter actor, ICharacter target) =>
		PredatorAIHelpers.CheckForAttack(actor, target, AwarenessThreatProg, EngageDelayDiceExpression, EngageEmote, false);
	protected static TEnum ParseEnum<TEnum>(string? text, TEnum fallback) where TEnum : struct
	{
		return !string.IsNullOrWhiteSpace(text) && Enum.TryParse(text, true, out TEnum value)
			? value
			: fallback;
	}

	protected static int DefaultRangeFor(AnimalMovementStrategyType strategy)
	{
		return strategy switch
		{
			AnimalMovementStrategyType.Swim => DefaultSwimRange,
			AnimalMovementStrategyType.Fly => DefaultFlyRange,
			AnimalMovementStrategyType.Arboreal => DefaultArborealRange,
			AnimalMovementStrategyType.Amphibious => DefaultSwimRange,
			_ => DefaultGroundRange
		};
	}

	protected ICreatureMovementStrategy MovementStrategyHandler => MovementStrategy switch
	{
		AnimalMovementStrategyType.Swim => SwimmingMovementStrategy.Instance,
		AnimalMovementStrategyType.Fly => FlyingMovementStrategy.Instance,
		AnimalMovementStrategyType.Arboreal => ArborealMovementStrategy.Instance,
		AnimalMovementStrategyType.Amphibious => AmphibiousMovementStrategy.Instance,
		_ => GroundMovementStrategy.Instance
	};

	protected ICreatureHomeStrategy HomeStrategyHandler => HomeStrategy switch
	{
		AnimalHomeStrategyType.Territorial => TerritorialHomeStrategy.Instance,
		AnimalHomeStrategyType.Denning => DenningHomeStrategy.Instance,
		_ => NoHomeStrategy.Instance
	};

	protected ICreatureAwarenessStrategy AwarenessStrategyHandler => AwarenessStrategy switch
	{
		AnimalAwarenessStrategyType.Wary => WaryAwarenessStrategy.Instance,
		AnimalAwarenessStrategyType.Wimpy => WimpyAwarenessStrategy.Instance,
		AnimalAwarenessStrategyType.Skittish => SkittishAwarenessStrategy.Instance,
		AnimalAwarenessStrategyType.Guarding => GuardingAwarenessStrategy.Instance,
		_ => NoAwarenessStrategy.Instance
	};

	protected ICreatureRefugeStrategy RefugeStrategyHandler => RefugeStrategy switch
	{
		AnimalRefugeStrategyType.Home => HomeRefugeStrategy.Instance,
		AnimalRefugeStrategyType.Den => DenRefugeStrategy.Instance,
		AnimalRefugeStrategyType.Trees => TreesRefugeStrategy.Instance,
		AnimalRefugeStrategyType.Sky => SkyRefugeStrategy.Instance,
		AnimalRefugeStrategyType.Water => WaterRefugeStrategy.Instance,
		AnimalRefugeStrategyType.Prog => ProgRefugeStrategy.Instance,
		_ => NoRefugeStrategy.Instance
	};

	protected static ICharacter? CharacterForEvent(EventType type, dynamic[] arguments)
	{
		return type switch
		{
			EventType.CharacterEnterCellWitness => arguments[3] as ICharacter,
			EventType.CharacterDiesWitness => arguments[1] as ICharacter,
			EventType.EngagedInCombat => arguments[1] as ICharacter,
			EventType.TrapCaughtPrey => arguments[0] as ICharacter,
			_ => arguments.Length > 0 ? arguments[0] as ICharacter : null
		};
	}

	protected void EvaluateHomeAndTerritory(ICharacter character)
	{
		HomeStrategyHandler.Evaluate(this, character);
	}

	protected TimeSpan AwarenessMemory => TimeSpan.FromMinutes(AwarenessMemoryMinutes);

	protected bool EvaluateCreatureSenses(ICharacter character)
	{
		if (SensesStrategy.In(AnimalSensesStrategyType.Hiding, AnimalSensesStrategyType.Stalking) &&
		    !character.AffectedBy<ISneakEffect>())
		{
			character.AddEffect(new Sneak(character));
			return true;
		}

		if (SensesStrategy == AnimalSensesStrategyType.Hiding &&
		    !character.AffectedBy<IHideEffect>() &&
		    character.Location.CharactersInSpatialVicinity(character).Except(character).Any(x => character.CanSee(x)))
		{
			character.ExecuteCommand("hide");
			return true;
		}

		return false;
	}

	protected int EffectiveAwarenessRange => SensesStrategy == AnimalSensesStrategyType.Vigilant
		? AwarenessRange + 2
		: AwarenessRange;

	protected uint EffectiveScanRange(ICharacter character)
	{
		return (uint)Math.Min(Math.Max(0, EffectiveAwarenessRange), (int)character.MaximumPerceptionRange);
	}

	/// <summary>
	/// Performs the animal equivalent of a silent scan. Group-led animals are intentionally scanned by their
	/// leader and sentries instead, avoiding one identical check per group member.
	/// </summary>
	internal IReadOnlyList<ICharacter> AcquireRangedTargets(ICharacter character)
	{
		uint range = EffectiveScanRange(character);
		if (range == 0 || character.Location is null || character.Combat is not null || character.Movement is not null)
		{
			return [];
		}

		return ScanTargetAcquisition.AcquireVisibleCharacters(character, range);
	}

	/// <summary>
	/// Gives a group member a sentry's already-successful scan only when that member presently has
	/// the same unobstructed, in-range view. This shares group intelligence without granting sight
	/// through doors, around corners, across layers or beyond the normal targeting range.
	/// </summary>
	internal void ReceiveGroupSighting(ICharacter character, ICharacter target)
	{
		if (ScanTargetAcquisition.IsVisibleRangedTarget(character, target, EffectiveScanRange(character), false))
		{
			character.SeeTarget(target);
		}
	}

	protected static bool IsLocalHuntTarget(ICharacter character, ICharacter target)
	{
		if (character.Location is null || target.Location is null ||
			!ReferenceEquals(character.Location, target.Location) || character.RoomLayer != target.RoomLayer)
		{
			return false;
		}

		return character.Location.RouteDefinition is null ||
		       RouteSpatialService.Instance.GetProximity(character, target) <= Proximity.Proximate;
	}

	protected bool IsGroupControlled(ICharacter character, GroupAIControlScope scope)
	{
		return character is INPC npc &&
		       npc.GroupAI?.GroupAIType is IGroupAIControlPolicy policy &&
		       policy.ControlScope.HasFlag(scope);
	}

	protected bool IsWithinToleratedHabitat(ICharacter character, ICell cell)
	{
		return ToleratedHabitatProg.ExecuteBool(false, character, cell, character.Location);
	}

	protected bool IsSuitableTerritory(ICharacter character, ICell cell)
	{
		if (SuitableTerritoryProg.MatchesParameters(
			    new[] { ProgVariableTypes.Character, ProgVariableTypes.Location }))
		{
			return SuitableTerritoryProg.ExecuteBool(false, character, cell);
		}

		return SuitableTerritoryProg.ExecuteBool(false, cell, character);
	}

	protected Func<ICellExit, bool> GetAnimalSuitabilityFunction(ICharacter character, bool ignoreSafeMovement = false)
	{
		Func<ICellExit, bool> baseSuitability = base.GetSuitabilityFunction(character, ignoreSafeMovement);
		return exit => baseSuitability(exit) &&
		               PermitsPursuitCell(character, exit.Destination) &&
		               MovementStrategyHandler.CellMatches(this, character, exit.Destination) &&
		               IsWithinToleratedHabitat(character, exit.Destination) &&
		               !ShouldAvoidCell(character, exit.Destination);
	}

	protected virtual bool PermitsPursuitCell(ICharacter character, ICell cell) => true;

	protected IEnumerable<ICharacter> VisibleAwarenessThreats(ICharacter character, ICharacter? witnessedTarget)
	{
		HashSet<ICharacter> threats = new();
		if (witnessedTarget is not null &&
		    !IsSociallyTrusted(character, witnessedTarget) &&
		    AwarenessThreatProg.ExecuteBool(false, character, witnessedTarget) &&
		    CanObserveTarget(character, witnessedTarget))
		{
			threats.Add(witnessedTarget);
		}

		foreach (ICharacter target in ObservedCharacters(character))
		{
			if (IsSociallyTrusted(character, target) ||
			    !AwarenessThreatProg.ExecuteBool(false, character, target))
			{
				continue;
			}

			threats.Add(target);
		}

		return threats;
	}

	/// <summary>
	/// Returns immediate same-layer targets plus remote targets that a prior silent scan registered and
	/// that remain visible under the normal scan rules. This prevents secondary awareness and ecology
	/// behaviours from treating vicinity enumeration as omniscient sight.
	/// </summary>
	protected IEnumerable<ICharacter> ObservedCharacters(ICharacter character)
	{
		if (character.Location is null)
		{
			return [];
		}

		return character.Location
		                .LayerCharacters(character.RoomLayer)
		                .Where(x => IsLocalHuntTarget(character, x))
		                .Concat(character.SeenTargets.OfType<ICharacter>())
		                .Where(x => !ReferenceEquals(character, x) && CanObserveTarget(character, x))
		                .DistinctPhysicalInstances();
	}

	protected bool CanObserveTarget(ICharacter character, ICharacter target)
	{
		if (ReferenceEquals(character, target) || target.Location is null)
		{
			return false;
		}

		return IsLocalHuntTarget(character, target) || Hunting.Enabled &&
		       ReferenceEquals(character.Location, target.Location) && character.Location.RouteDefinition is null
			? character.CanSee(target)
			: ScanTargetAcquisition.IsCurrentVisibleRangedTarget(character, target, EffectiveScanRange(character));
	}

	protected bool ShouldAvoidCell(ICharacter character, ICell cell)
	{
		if (AwarenessStrategy == AnimalAwarenessStrategyType.None)
		{
			return false;
		}

		if (AwarenessAvoidCellProg.ExecuteBool(false, character, cell, character.Location))
		{
			return true;
		}

		return NpcKnownThreatLocationsEffect.Get(character)?.Knows(cell, AwarenessMemory) == true;
	}

	protected void RememberThreats(ICharacter character, IEnumerable<ICharacter> threats)
	{
		List<ICell> cells = threats
		                    .Select(x => x.Location)
		                    .WhereNotNull(x => x)
		                    .Distinct()
		                    .ToList();
		if (!cells.Any())
		{
			return;
		}

		NpcKnownThreatLocationsEffect memory = NpcKnownThreatLocationsEffect.GetOrCreate(character);
		foreach (ICell cell in cells)
		{
			memory.Remember(cell);
		}
	}

	protected bool TryAwarenessResponse(ICharacter character, ICharacter? witnessedTarget)
	{
		if (IsGroupControlled(character, GroupAIControlScope.Senses) ||
		    character.Combat is not null ||
		    character.Movement is not null ||
		    character.Effects.Any(x => x.IsBlockingEffect("general") || x.IsBlockingEffect("movement")))
		{
			return false;
		}

		return AwarenessStrategyHandler.TryRespond(this, character, witnessedTarget);
	}

	protected void HandleCombatAwareness(ICharacter character)
	{
		if (!AwarenessStrategy.In(AnimalAwarenessStrategyType.Wimpy, AnimalAwarenessStrategyType.Skittish))
		{
			return;
		}

		character.CombatStrategyMode = CombatStrategyMode.Flee;
		if (character.CombatTarget is ICharacter target)
		{
			NpcKnownThreatLocationsEffect.GetOrCreate(character).Remember(target.Location);
		}
	}

	protected bool TryMoveAwayFromAwarenessThreats(ICharacter character, IEnumerable<ICharacter> threats)
	{
		List<ICell> threatCells = threats.Select(x => x.Location).Distinct().ToList();
		ICellExit? exit = character.Location.ExitsFor(character)
		                           .Where(GetAnimalSuitabilityFunction(character))
			.Where(x => !threatCells.Contains(x.Destination))
			.Where(x => !x.Destination.Characters.Any(y =>
			                           !IsSociallyTrusted(character, y) &&
			                           AwarenessThreatProg.ExecuteBool(false, character, y)))
		                           .GetRandomElement();
		return exit is not null && character.CanMove(exit) && character.Move(exit);
	}

	protected bool TryMoveToRefuge(ICharacter character)
	{
		if (TryMoveToRefugeLayer(character))
		{
			return true;
		}

		(ICell? target, IEnumerable<ICellExit> path) = RefugeStrategyHandler.GetPath(this, character);
		List<ICellExit> exits = path.ToList();
		if (target is null || !exits.Any())
		{
			return false;
		}

		FollowingPath effect = CreatePathingEffect(character, exits);
		character.AddEffect(effect);
		FollowPathAction(character, effect);
		return true;
	}

	protected bool TryMoveToRefugeLayer(ICharacter character)
	{
		if (!RefugeStrategy.In(AnimalRefugeStrategyType.Sky, AnimalRefugeStrategyType.Trees) ||
		    character.RoomLayer == RefugeLayer)
		{
			return false;
		}

		if (RefugeStrategy == AnimalRefugeStrategyType.Trees &&
		    !ArborealWandererAI.CellSupportsTreeLayers(character, character.Location))
		{
			return false;
		}

		FollowingMultiLayerPath effect = new(character, Enumerable.Empty<ICellExit>(), RefugeLayer, RefugeLayer);
		character.AddEffect(effect);
		FollowPathAction(character, effect);
		return true;
	}

	protected bool IsAtRefuge(ICharacter character)
	{
		return RefugeStrategy switch
		{
			AnimalRefugeStrategyType.None => true,
			AnimalRefugeStrategyType.Home => ResolveHomeBase(character).HomeCell is ICell home &&
			                                  ReferenceEquals(home, character.Location),
			AnimalRefugeStrategyType.Den => ResolveHomeBase(character).HomeCell is ICell home &&
			                                 ReferenceEquals(home, character.Location),
			AnimalRefugeStrategyType.Trees => ArborealWandererAI.CellSupportsTreeLayers(character, character.Location) &&
			                                  character.RoomLayer.In(RoomLayer.InTrees, RoomLayer.HighInTrees),
			AnimalRefugeStrategyType.Sky => character.RoomLayer == RefugeLayer,
			AnimalRefugeStrategyType.Water => WaterStrategy == AnimalWaterStrategyType.Drink
				? NpcSurvivalAIHelpers.HasLocalWaterSource(character)
				: NpcSurvivalAIHelpers.HasAquaticWaterSource(character, character.Location,
					WaterStrategy == AnimalWaterStrategyType.Surface),
			AnimalRefugeStrategyType.Prog => RefugeCellProg.ExecuteBool(false, character, character.Location),
			_ => true
		};
	}

	protected (ICell? Target, IEnumerable<ICellExit> Path) GetRefugePath(ICharacter character)
	{
		if (IsAtRefuge(character))
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		return RefugeStrategyHandler.GetPath(this, character);
	}

	protected bool ShouldReturnToRefuge(ICharacter character)
	{
		return RefugeStrategy != AnimalRefugeStrategyType.None &&
		       SurvivalNeedsSatisfied(character) &&
		       !IsAtRefuge(character);
	}

	protected (ICell? Target, IEnumerable<ICellExit> Path) GetAvoidancePath(ICharacter character)
	{
		Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = character.AcquireTargetAndPath(
			x => x is ICell cell &&
			     !ShouldAvoidCell(character, cell) &&
			     !cell.Characters.Any(y => AwarenessThreatProg.ExecuteBool(false, character, y)),
			(uint)Math.Max(1, AwarenessRange),
			GetAnimalSuitabilityFunction(character));
		return targetPath.Item1 is ICell target && targetPath.Item2.Any()
			? (target, targetPath.Item2)
			: (null, Enumerable.Empty<ICellExit>());
	}

	protected void EmitPosture(ICharacter character, ICharacter target)
	{
		if (string.IsNullOrWhiteSpace(PostureEmote))
		{
			return;
		}

		Emote emote = new(PostureEmote, character, character, target);
		if (emote.Valid)
		{
			character.OutputHandler.Handle(new EmoteOutput(emote, flags: OutputFlags.InnerWrap));
		}
	}

	protected static bool SetFleeCombatStrategy(ICharacter character)
	{
		character.CombatStrategyMode = CombatStrategyMode.Flee;
		return true;
	}

	protected NpcHomeBaseEffect ResolveHomeBase(ICharacter character)
	{
		NpcHomeBaseEffect home = NpcHomeBaseEffect.GetOrCreate(character);
		if (home.HomeCell is not null)
		{
			return home;
		}

		if (HomeLocationProg?.Execute<ICell?>(character) is ICell location)
		{
			home.SetHomeCell(location);
		}

		return home;
	}

	protected void EvaluateBurrowLifecycle(ICharacter character)
	{
		if (character.Movement is not null ||
		    character.Combat is not null ||
		    character.Effects.Any(x => x.IsBlockingEffect("movement")) ||
		    character.EffectsOfType<IActiveCraftEffect>().Any(x => !ReferenceEquals(x.Component.Craft, BurrowCraft)))
		{
			return;
		}

		NpcHomeBaseEffect home = ResolveHomeBase(character);
		if (home.HomeCell is null)
		{
			if (BurrowSiteProg.ExecuteBool(false, character, character.Location))
			{
				home.SetHomeCell(character.Location);
			}
			else
			{
				CheckPathingEffect(character, true);
				return;
			}
		}

		if (!ReferenceEquals(home.HomeCell, character.Location))
		{
			CheckPathingEffect(character, true);
			return;
		}

		RefreshAnchorItem(character, home);
		if (home.AnchorItem is not null || BurrowCraft is null || !BuildEnabledProg.ExecuteBool(true, character))
		{
			return;
		}

		IActiveCraftGameItemComponent? interruptedCraft = character.Location!.LayerGameItems(character.RoomLayer)
			.SelectNotNull(x => x!.GetItemType<IActiveCraftGameItemComponent>())
			.FirstOrDefault(x => ReferenceEquals(x.Craft, BurrowCraft));
		if (interruptedCraft is not null)
		{
			(bool canResume, string _) = BurrowCraft.CanResumeCraft(character, interruptedCraft);
			if (canResume)
			{
				BurrowCraft.ResumeCraft(character, interruptedCraft);
			}

			return;
		}

		(bool canDoCraft, string _) = BurrowCraft.CanDoCraft(character, null, true, true);
		if (canDoCraft)
		{
			BurrowCraft.BeginCraft(character);
		}
	}

	protected void RefreshAnchorItem(ICharacter character, NpcHomeBaseEffect home)
	{
		if (home.AnchorItem is not null && ReferenceEquals(home.AnchorItem.Location, home.HomeCell))
		{
			if (AnchorItemProg is null)
			{
				// Legacy AnimalAI XML had no explicit anchor policy. Retain its remembered
				// anchor without claiming an arbitrary item under the new shelter model.
				return;
			}

			if (AnchorItemProg.ExecuteBool(character, home.AnchorItem) &&
			    WildlifeShelterClaimEffect.ClaimOrRefresh(home.AnchorItem, character, AllowGroupShelterSharing))
			{
				return;
			}
		}

		home.ClearAnchorItem();
		IGameItem? anchor = DenBuilderAI.SelectAnchorItem(character, AnchorItemProg, AllowGroupShelterSharing);
		if (anchor is not null &&
		    (AnchorItemProg is null || WildlifeShelterClaimEffect.ClaimOrRefresh(anchor, character, AllowGroupShelterSharing)))
		{
			home.SetAnchorItem(anchor);
		}
	}

	protected void EvaluateTerritory(ICharacter character)
	{
		Territory? territoryEffect = character.CombinedEffectsOfType<Territory>().FirstOrDefault();
		if (territoryEffect is null)
		{
			territoryEffect = new Territory(character);
			character.AddEffect(territoryEffect);
		}

		List<ICell> cells = territoryEffect.Cells.ToList();
		if (cells.Count >= DesiredTerritorySizeProg.ExecuteInt(0, character))
		{
			return;
		}

		ICollection<ICell> claimedTerritory = GetClaimedTerritory(character);
		if (cells.Count == 0)
		{
			if (IsSuitableTerritory(character, character.Location) &&
			    !claimedTerritory.Contains(character.Location))
			{
				territoryEffect.AddCell(character.Location);
				return;
			}

			(IPerceivable target, IEnumerable<ICellExit> _) = character.AcquireTargetAndPath(
				loc => loc is ICell candidate && IsSuitableTerritory(character, candidate) &&
				       !claimedTerritory.Contains(loc),
				20,
				GetAnimalSuitabilityFunction(character));
			if (target is ICell cell)
			{
				territoryEffect.AddCell(cell);
			}

			return;
		}

		foreach (ICell cell in territoryEffect.Cells)
		{
			ICell expand = cell
			               .ExitsFor(character, true)
				               .Where(x => IsSuitableTerritory(character, x.Destination) &&
			                           !claimedTerritory.Contains(x.Destination))
			               .Select(x => x.Destination)
			               .GetRandomElement();
			if (expand is not null && !territoryEffect.Cells.Contains(expand))
			{
				territoryEffect.AddCell(expand);
				return;
			}
		}
	}

	protected ICollection<ICell> GetClaimedTerritory(ICharacter character)
	{
		if (WillShareTerritory)
		{
			return new List<ICell>();
		}

		IEnumerable<ICharacter> npcs = character.Gameworld.NPCs;
		if (WillShareTerritoryWithOtherRaces)
		{
			npcs = npcs.Where(x => !x.Race.SameRace(character.Race));
		}

		return npcs
		       .SelectNotNull(x => x!.CombinedEffectsOfType<Territory>().FirstOrDefault())
		       .SelectMany(x => x.Cells)
		       .Distinct()
		       .ToList();
	}

	protected (ICell? Target, IEnumerable<ICellExit> Path) GetBurrowHomePath(ICharacter ch)
	{
		NpcHomeBaseEffect home = ResolveHomeBase(ch);
		if (home.HomeCell is not null && !ReferenceEquals(home.HomeCell, ch.Location))
		{
			List<ICellExit> homePath = ch.PathBetween(home.HomeCell, DefaultNeedRange, GetAnimalSuitabilityFunction(ch)).ToList();
			return homePath.Any()
				? (home.HomeCell, homePath)
				: (null, Enumerable.Empty<ICellExit>());
		}

		if (home.HomeCell is not null)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = ch.AcquireTargetAndPath(
			x => x is ICell cell && BurrowSiteProg.ExecuteBool(false, ch, cell),
			DefaultNeedRange,
			GetAnimalSuitabilityFunction(ch));
		return targetPath.Item1 is ICell burrowCell && targetPath.Item2.Any()
			? (burrowCell, targetPath.Item2)
			: (null, Enumerable.Empty<ICellExit>());
	}

	protected (ICell? Target, IEnumerable<ICellExit> Path) GetTerritoryPath(ICharacter ch)
	{
		Territory? territory = ch.CombinedEffectsOfType<Territory>().FirstOrDefault();
		if (territory is null)
		{
			territory = new Territory(ch);
			ch.AddEffect(territory);
		}

		if (!territory.Cells.Any())
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		if (!territory.Cells.Contains(ch.Location))
		{
			List<ICellExit> path = ch.PathBetween(territory.Cells.Cast<IPerceivable>(), DefaultNeedRange,
				GetAnimalSuitabilityFunction(ch, true)).ToList();
			return path.Any()
				? (path.Last().Destination, path)
				: (null, Enumerable.Empty<ICellExit>());
		}

		List<ICell> targets = territory.Cells
		                               .Where(x => !ReferenceEquals(x, ch.Location))
		                               .Where(x => MovementStrategyHandler.CellMatches(this, ch, x))
		                               .ToList();
		if (!targets.Any())
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		List<ICellExit> targetPath = ch.PathBetween(targets.Cast<IPerceivable>(), (uint)MovementRange,
			GetAnimalSuitabilityFunction(ch, true)).ToList();
		return targetPath.Any()
			? (targetPath.Last().Destination, targetPath)
			: (null, Enumerable.Empty<ICellExit>());
	}

	protected override FollowingPath CreatePathingEffect(ICharacter ch, IEnumerable<ICellExit> path)
	{
		return MovementStrategyHandler.CreatePathingEffect(this, ch, path);
	}

}
