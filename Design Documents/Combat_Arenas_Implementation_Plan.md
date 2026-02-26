# Implementation Plan: Combat Arenas (FutureMUD)

Status: Draft for implementation handoff  
Audience: Specialized implementers (engine, DB, finance, combat, perception, commands, QA, docs)  
Source Design: `Design Documents/Combat_Arenas_Design.md`

Current Runtime Snapshot (2026-02-25)
- Elimination terms are now explicit via `ArenaEliminationMode` (`NoElimination`, `PointsElimination`, `KnockDown`, `Knockout`, `Death`).
- Event types now include `AllowSurrender`, and surrender is exposed as `arena surrender [<event>]`.
- Live events now poll for elimination conditions during combat and transition to resolving without waiting only for timeout.
- NPC cleanup now handles dead participants safely, including corpse relocation to stable/after-fight locations.
- Combatant classes now expose a separate stable-only full-recovery toggle for post-event NPC reset.
- BYO events now tag participant direct items with a saveable ownership effect and reclaim those tagged items to the original owner during cleanup.
- Reclaimed BYO items are repaired when the tagged owner is an NPC participant whose combatant class has full-recovery enabled.
- Completed-event rating settlement now keys participant updates by persistent character IDs, so ratings are persisted even when character objects are unloaded at completion.
- Event types now persist rating strategy controls: `EloStyle` (`TeamAverage`, `PairwiseIndividual`, `PairwiseSide`) and `EloKFactor`.
- Arena lifecycle text announcements now use watcher-suppressed output flags to avoid duplicate mirrored spam to observers.
- Participation/preparation/staging cleanup now includes actor-wide orphan sweeps, and stale no-quit/no-timeout arena effects self-prune on load/login when their event no longer exists or is no longer in the expected state.
- Auto reacquire target selection now ignores incapacitated combatants, preventing spurious target-switch echoes immediately before knockout-resolved arena conclusions.

---

## 0) Goals, Scope, Assumptions

Goals
- Deliver Combat Arenas as a data‑driven, crash‑safe engine feature with configurable event types, NPC auto‑fill, observer mirroring, betting (fixed‑odds and pari‑mutuel), ratings, and business/finance integration.

Scope
- Engine, persistence, commands, seeder, tests, and docs for the arenas feature. Not a game‑specific content pack beyond minimal examples in the seeder.

Assumptions
- .NET 9, EF Core 9, MySQL (Pomelo).
- Interface‑first design; shared interfaces in `FutureMUDLibrary` and EF in `MudsharpDatabaseLibrary`.
- Follow `AGENTS.md`: tabs, file‑scoped namespaces, `#nullable enable`, LINQ style, StringUtilities helpers, colour conventions, Emote markup rules.
- Avoid cross‑reboot state restoration; mid‑fight reboot cancels matches and refunds wagers.

Non‑Goals
- No bespoke client UI; players interact via existing telnet client UX and Discord bot optionally.

Success Criteria
- A manager launches a scheduled or manual event; players/NPCs compete; observers watch; payouts and ratings settle; crash during live triggers safe cancel/refund.

---

## 1) High‑Level Architecture

Modules
- Contracts (interfaces, enums, events): `FutureMUDLibrary/Arenas/*`
- EF Core models/migrations: `MudsharpDatabaseLibrary/Models/*` and Migrations
- Engine services and runtime: `MudSharpCore/Arenas/*`
- Commands: `MudSharpCore/Commands/Arenas/*`
- Seeder flows: `DatabaseSeeder/*`
- Tests: `MudSharpCore Unit Tests/*`

Boundaries
- Arenas act like a business tied to an `IEconomicZone`, holding funds (bank/virtual) and liable for taxes.
- Event lifecycle is stateful and persisted; transitions are idempotent and scheduler‑driven.
- Betting and ratings integrate via services and prog hooks.
- Observation mirrors echoes with hearing/notice constraints, never leaking hidden info.

---

## 2) Interface‑First Contracts (Sketched Members)

Notes
- These are member sketches to guide implementation; minor adjustments are allowed to align with existing patterns after integration spikes.
- Use file‑scoped namespaces, tabs, `#nullable enable`, XML docs.

### 2.1 Core Enums (Library)

File: `FutureMUDLibrary/Arenas/ArenaEnums.cs`

```csharp
#nullable enable
namespace MudSharp.Arenas;

public enum ArenaEventState {
	Draft,
	Scheduled,
	RegistrationOpen,
	Preparing,
	Staged,
	Live,
	Resolving,
	Cleanup,
	Completed,
	Aborted
}

public enum BettingModel {
	FixedOdds,
	PariMutuel
}

public enum EliminationReason {
	Knockout,
	Death,
	Surrender,
	Time,
	Disqualification
}

public enum ArenaOutcome {
	Win,
	Draw,
	Aborted
}

public enum ArenaSidePolicy {
	Open,
	ManagersOnly,
	ReservedOnly,
	Closed
}

public enum ArenaFeeType {
	Appearance,
	Victory
}
```

### 2.2 Support Contracts (Library)

File: `FutureMUDLibrary/Arenas/IArenaSupport.cs`

```csharp
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Arenas;

/// <summary>
/// Participant identity in an Arena Event.
/// Represents a slot for either a PC or NPC with a chosen CombatantClass and side assignment.
/// </summary>
public interface IArenaParticipant : IProgVariable {
	long CharacterId { get; }
	ICharacter Character { get; }
	ICombatantClass CombatantClass { get; }
	int SideIndex { get; }
	bool IsNpc { get; }
	string? StageName { get; }
	string? SignatureColour { get; }
	decimal? StartingRating { get; }
}

/// <summary>
/// Per-side definition for an Event Type.
/// </summary>
public interface IArenaEventTypeSide : IProgVariable {
	int Index { get; }
	int Capacity { get; }
	ArenaSidePolicy Policy { get; }
	IEnumerable<ICombatantClass> EligibleClasses { get; }
	IFutureProg? OutfitProg { get; }
	bool AllowNpcSignup { get; }
	bool AutoFillNpc { get; }
	IFutureProg? NpcLoaderProg { get; }
}
```

### 2.3 Combatant Class (Library)

File: `FutureMUDLibrary/Arenas/ICombatantClass.cs`

```csharp
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Arenas;

public interface ICombatantClass : IFrameworkItem, ISaveable, IProgVariable {
	/// <summary>Eligibility prog returns bool; params: Character.</summary>
	IFutureProg EligibilityProg { get; }
	/// <summary>Optional NPC loader prog (admin-configured); params: Number slotsNeeded; returns collection of Characters.</summary>
	IFutureProg? AdminNpcLoaderProg { get; }
	/// <summary>If true, resurrect NPCs immediately when they die in an arena event.</summary>
	bool ResurrectNpcOnDeath { get; }
	/// <summary>If true, fully restore NPC health/status after returning to NPC stables at event completion.</summary>
	bool FullyRestoreNpcOnCompletion { get; }
	/// <summary>Optional identity metadata defaults (e.g., stage-name random profile, signature colour, signature item set id).</summary>
	IRandomNameProfile? DefaultStageNameProfile { get; }
	string? DefaultSignatureColour { get; }
}
```

### 2.4 Arena (Library)

File: `FutureMUDLibrary/Arenas/ICombatArena.cs`

```csharp
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Arenas;

public interface ICombatArena : IFrameworkItem, ISaveable, IProgVariable {
	IEconomicZone EconomicZone { get; }
	ICurrency Currency { get; }
	IBankAccount? BankAccount { get; set; }
	IEnumerable<ICharacter> Managers { get; }

	IEnumerable<IRoom> WaitingRooms { get; }
	IEnumerable<IRoom> ArenaRooms { get; }
	IEnumerable<IRoom> ObservationRooms { get; }
	IEnumerable<IRoom> InfirmaryRooms { get; }
	IEnumerable<IRoom> NpcStablesRooms { get; }
	IEnumerable<IRoom> AfterFightRooms { get; }

	IEnumerable<IArenaEventType> EventTypes { get; }
	IEnumerable<IArenaEvent> ActiveEvents { get; }

	bool IsManager(ICharacter actor);
	void AddManager(ICharacter actor);
	void RemoveManager(ICharacter actor);

	(bool Truth, string Reason) IsReadyToHost(IArenaEventType eventType);
	IArenaEvent CreateEvent(IArenaEventType eventType, DateTime when, IEnumerable<IArenaReservation>? reservations = null);
	void AbortEvent(IArenaEvent arenaEvent, string reason, ICharacter? byManager = null);

	decimal AvailableFunds();
	(bool Truth, string Reason) EnsureFunds(decimal amount);
	void Credit(decimal amount, string reference);
	void Debit(decimal amount, string reference);

	string ShowToManager(ICharacter actor);
	bool BuildingCommand(ICharacter actor, StringStack command);
}
```

### 2.5 Event Type (Library)

File: `FutureMUDLibrary/Arenas/IArenaEventType.cs`

```csharp
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Arenas;

public interface IArenaEventType : IFrameworkItem, ISaveable, IProgVariable {
	ICombatArena Arena { get; }
	IEnumerable<IArenaEventTypeSide> Sides { get; }
	bool BringYourOwn { get; }
	TimeSpan RegistrationDuration { get; }
	TimeSpan PreparationDuration { get; }
	TimeSpan? TimeLimit { get; }
	BettingModel BettingModel { get; }
	decimal AppearanceFee { get; }
	decimal VictoryFee { get; }

	/// <summary>Optional intro/countdown prog; side-aware text.</summary>
	IFutureProg? IntroProg { get; }
	/// <summary>Prog for scoring hook calls; see scoring bridge for params.</summary>
	IFutureProg? ScoringProg { get; }
	/// <summary>Override resolution; returns (Outcome, WinningSides).</summary>
	IFutureProg? ResolutionOverrideProg { get; }
	ArenaEloStyle EloStyle { get; }
	decimal EloKFactor { get; }

	IArenaEvent CreateInstance(DateTime scheduledTime, IEnumerable<IArenaReservation>? reservations = null);
	IArenaEventType Clone(string newName, ICharacter originator);
}
```

### 2.6 Event Instance (Library)

File: `FutureMUDLibrary/Arenas/IArenaEvent.cs`

```csharp
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Arenas;

public interface IArenaReservation : IProgVariable {
	int SideIndex { get; }
	long? CharacterId { get; }
	long? ClanId { get; }
	DateTime ExpiresAt { get; }
}

public interface IArenaEvent : IFrameworkItem, ISaveable, IProgVariable {
	ICombatArena Arena { get; }
	IArenaEventType Type { get; }
	ArenaEventState State { get; }
	DateTime CreatedAt { get; }
	DateTime ScheduledAt { get; }
	DateTime? RegistrationOpensAt { get; }
	DateTime? StartedAt { get; }
	DateTime? ResolvedAt { get; }
	DateTime? CompletedAt { get; }

	IEnumerable<IArenaParticipant> Participants { get; }
	IEnumerable<IArenaReservation> Reservations { get; }

	void OpenRegistration();
	void CloseRegistration();
	void StartPreparation();
	void Stage();
	void StartLive();
	void MercyStop();
	void Resolve();
	void Cleanup();
	void Complete();
	void Abort(string reason);

	(bool Truth, string Reason) CanSignUp(ICharacter character, int sideIndex, ICombatantClass combatantClass);
	void SignUp(ICharacter character, int sideIndex, ICombatantClass combatantClass);
	void Withdraw(ICharacter character);

	void AddReservation(IArenaReservation reservation);
	void RemoveReservation(IArenaReservation reservation);

	/// <summary>Idempotent transition enforcement for reboot safety.</summary>
	void EnforceState(ArenaEventState state);
}
```

### 2.7 Services (Library)

File: `FutureMUDLibrary/Arenas/IArenaServices.cs`

```csharp
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Arenas;

public interface IArenaScheduler {
	void Schedule(IArenaEvent evt);
	void Cancel(IArenaEvent evt);
	/// <summary>Called on boot to cancel/refund unsafe states.</summary>
	void RecoverAfterReboot();
}

public interface IArenaLifecycleService {
	void Transition(IArenaEvent evt, ArenaEventState toState);
	void RebootRecovery();
}

public interface IArenaObservationService {
	(bool Truth, string Reason) CanObserve(ICharacter observer, IArenaEvent evt);
	void StartObserving(ICharacter observer, IArenaEvent evt, IRoom observationRoom);
	void StopObserving(ICharacter observer, IArenaEvent evt);
}

public interface IArenaBettingService {
	(bool Truth, string Reason) CanBet(ICharacter actor, IArenaEvent evt);
	void PlaceBet(ICharacter actor, IArenaEvent evt, int? sideIndex, decimal stake);
	void CancelBet(ICharacter actor, IArenaEvent evt);
	void Settle(IArenaEvent evt, ArenaOutcome outcome, IEnumerable<int> winningSides);
	void RefundAll(IArenaEvent evt, string reason);
	(decimal? FixedOdds, (decimal Pool, decimal TakeRate)? PariMutuel) GetQuote(IArenaEvent evt, int? sideIndex);
}

public interface IArenaRatingsService {
	decimal GetRating(ICharacter character, ICombatantClass cclass);
	void UpdateRatings(IArenaEvent evt, IReadOnlyDictionary<ICharacter, decimal> deltas);
	/// <summary>Invokes default Elo implementation; params: participants, outcomes, current ratings.</summary>
	void ApplyDefaultElo(IArenaEvent evt);
}

public interface IArenaFinanceService {
	(bool Truth, string Reason) IsSolvent(ICombatArena arena, decimal required);
	void WithholdTax(ICombatArena arena, decimal amount, string reference);
	void PostProfitLoss(ICombatArena arena, string reference);
	void BlockPayout(ICombatArena arena, IArenaEvent evt, IEnumerable<(ICharacter Winner, decimal Amount)> payouts);
	void UnblockPayouts(ICombatArena arena);
}

public interface IArenaNpcService {
	IEnumerable<ICharacter> AutoFill(IArenaEvent evt, int sideIndex, int slotsNeeded);
	void PrepareNpc(ICharacter npc, IArenaEvent evt, int sideIndex, ICombatantClass cclass);
	void ReturnNpc(ICharacter npc, IArenaEvent evt, bool resurrect);
}

public interface IArenaCommandService {
	void ShowArena(ICharacter actor, ICombatArena arena);
	void ShowEvent(ICharacter actor, IArenaEvent evt);
	void ShowEventType(ICharacter actor, IArenaEventType type);
}
```

### 2.8 Strategy Interfaces (Library)

File: `FutureMUDLibrary/Arenas/IArenaStrategies.cs`

```csharp
using MudSharp.Character;

namespace MudSharp.Arenas;

public interface IArenaScoringStrategy {
	void OnDamage(IArenaEvent evt, ICharacter source, ICharacter target, double amount);
	void OnKill(IArenaEvent evt, ICharacter source, ICharacter target);
	void OnSurrender(IArenaEvent evt, ICharacter source);
	IReadOnlyDictionary<int, double> GetScores(IArenaEvent evt);
}

public interface IArenaEliminationStrategy {
	bool IsEliminated(IArenaEvent evt, ICharacter participant);
	bool MercyStopAllowed(IArenaEvent evt);
}
```

### 2.9 Domain Events (Library)

File: `FutureMUDLibrary/Arenas/Events/ArenaDomainEvents.cs`

```csharp
namespace MudSharp.Arenas.Events;

public record ArenaEventStateChanged(long EventId, ArenaEventState OldState, ArenaEventState NewState);
public record ArenaScoreEvent(long EventId, long SourceId, long? TargetId, string Action, double Value);
public record ArenaPayoutEvent(long EventId, ArenaOutcome Outcome, IReadOnlyCollection<int> WinningSides);
public record ArenaParticipantEliminated(long EventId, long CharacterId, EliminationReason Reason);
```

---

## 3) EF Core Data Model & Migrations

Owner: Database Engineer  
Location: `MudsharpDatabaseLibrary`

Entities (sketch)
- `Arena`
  - Id, Name, EconomicZoneId, BankAccountId (nullable), CurrencyId
  - Funds (virtual cash) if bank absent
  - Rooms mapping tables (see below)
  - Soft‑delete flag
- `ArenaManager` (ArenaId, CharacterId)
- `ArenaRoomLink` (ArenaId, RoomId, Role: Waiting|Arena|Observation|Infirmary|Stables|AfterFight)
- `ArenaCombatantClass`
  - Id, Name, EligibilityProgId, AdminNpcLoaderProgId (nullable), ResurrectNpcOnDeath (bool), FullyRestoreNpcOnCompletion (bool), DefaultStageNameProfileId (nullable FK `RandomNameProfiles.Id`), DefaultSignatureColour
- `ArenaEventType`
  - Id, ArenaId, Name, BringYourOwn (bool), RegistrationDuration, PreparationDuration, TimeLimit (nullable)
  - BettingModel, AppearanceFee, VictoryFee
  - IntroProgId (nullable), ScoringProgId (nullable), ResolutionOverrideProgId (nullable), EloStyle (int), EloKFactor (decimal)
- `ArenaEventTypeSide`
  - Id, EventTypeId, Index, Capacity, Policy, OutfitProgId (nullable), AllowNpcSignup (bool), AutoFillNpc (bool), NpcLoaderProgId (nullable)
- `ArenaEvent`
  - Id, ArenaId, EventTypeId, State, CreatedAt, ScheduledAt, RegistrationOpensAt (nullable), StartedAt (nullable), ResolvedAt (nullable), CompletedAt (nullable)
  - CancellationReason (nullable)
- `ArenaReservation`
  - Id, EventId, SideIndex, CharacterId (nullable), ClanId (nullable), ExpiresAt
- `ArenaSignup`
  - Id, EventId, CharacterId, CombatantClassId, SideIndex, IsNpc (bool), StageName (nullable), SignatureColour (nullable), StartingRating (nullable)
- `ArenaElimination`
  - Id, EventId, CharacterId, Reason, OccurredAt
- `ArenaRating`
  - Id, ArenaId, CharacterId, CombatantClassId, Rating, LastUpdatedAt
- `ArenaBet`
  - Id, EventId, BettorCharacterId (nullable if via account), SideIndex (nullable for Draw), Stake, PlacedAt, ModelSnapshot (json), IsCancelled, CancelledAt (nullable)
- `ArenaBetPool`
  - Id, EventId, SideIndex (nullable for Draw), TotalStake, TakeRate
- `ArenaBetPayout`
  - Id, EventId, BettorCharacterId, Amount, CreatedAt, IsBlocked (bool), CollectedAt (nullable)
- `ArenaFinanceSnapshot`
  - Id, ArenaId, EventId, Period, Revenue, Costs, TaxWithheld, Profit

Indexes
- Lookups by (ArenaId, State), (EventId, SideIndex), (CharacterId, CombatantClassId), and time‑based schedulers.

Migrations
- One initial migration; subsequent diffs as needed during integration.

---

## 4) Engine Runtime & Lifecycle

Owner: Engine Runtime Specialist  
Location: `MudSharpCore/Arenas/*`

Services
- `ArenaScheduler`: queues transitions based on event timestamps; recovers on boot.
- `ArenaLifecycleService`: enforces idempotent state transitions; publishes domain events.
- `CombatEventBridge`: subscribes to combat callbacks to feed scoring/elimination.

Lifecycle (state machine)
- Draft → Scheduled → RegistrationOpen → Preparing → Staged → Live → Resolving → Cleanup → Completed
- Abort path to Aborted from any non‑terminal.

Crash/Reboot Handling
- On boot: fetch events in {Preparing, Staged, Live, Resolving} and transition to Aborted then invoke `BettingService.RefundAll` and inventory unbundle.

---

## 5) NPC Backfill, Inventory & Teleport

Owner: NPC/Character Systems Engineer  
Location: `MudSharpCore/Arenas/Npc/*`

Responsibilities
- Resolve side loaders; spawn or resurrect NPCs; equip via outfit progs when BYO=false.
- Snapshot/bundle inventory pre‑fight for BYO=false; for BYO=true, tag participant direct items with owner/event metadata and reclaim tagged items on cleanup.
- Repair reclaimed BYO items when the tagged owner is an NPC participant with `FullyRestoreNpcOnCompletion`.
- Teleport to waiting rooms per side; then stage to arena rooms per side; post‑fight to after‑fight rooms; eliminated PCs to infirmary.

---

## 6) Observation Layer

Owner: Perception/Emote Engineer  
Location: `MudSharpCore/Arenas/Observation/*`

Responsibilities
- `ArenaWatcherEffect`: applies to observers in Observation rooms; mirrors arena output with hearing/notice rules and noise quietening, while honouring output-level ignore-watcher flags for non-mirrored announcements.
- Ensure no leakage of hidden or subtle actions based on perceiver capabilities.

---

## 7) Betting & Finance

Owner: Economy/Finance Engineer  
Location: `MudSharpCore/Arenas/Betting/*`, `MudSharpCore/Arenas/Finance/*`

Betting
- Custody funds on placement; allow cancel only before close if policy permits.
- Fixed‑odds: snapshot odds per bet.
- Pari‑mutuel: pool stakes per side (incl. Draw); apply take rate at settlement.
- Payout blocking: if insolvent, create blocked payouts for later collection.

Finance
- Use `IBankAccount` if present; otherwise arena virtual cash store.
- Withhold tax entries and post period P&L to ledgers.

---

## 8) Ratings

Owner: Ratings/Math Engineer  
Location: `MudSharpCore/Arenas/Ratings/*`

Responsibilities
- Maintain per-class ratings with pluggable built-in Elo strategies (`TeamAverage`, `PairwiseIndividual`, `PairwiseSide`).
- Apply ratings only after Completed; Draw outcome handled per selected Elo strategy policy.
- Use participant character-ID snapshots so rating settlement works even when participant `ICharacter` objects are not loaded.

---

## 9) Commands & UX

Owner: Command UX Engineer  
Location: `MudSharpCore/Commands/Arenas/*`

Manager Commands
- `arena create|show|edit|rooms|managers|fund|types|events|abort`
- `arena type create|clone|edit|sides|fees|durations|policies|betting|outfits`
- `arenaeventtype set elostyle <TeamAverage|PairwiseIndividual|PairwiseSide>` and `arenaeventtype set elok <value>`
- `arena event schedule|launch|reserve|close|prepare|stage|start|stop|resolve|cleanup`

Player Commands
- `arena signup|withdraw`
- `arena observe enter|leave|list`
- `arena bet place|cancel|odds|pools`
- `arena ratings show`

UX Conventions
- `ColourValue` for numbers; `ColourError` for errors; `ColourCommand` for syntax; `ColourName` for names.
- Use `StringUtilities.GetTextTable()` for tabular output.

---

## 10) Participation Controls & Disconnects

Owner: Reliability/Session Engineer  
Location: `MudSharpCore/Arenas/Participation/*`

Responsibilities
- Apply a participation effect that blocks quitting; disconnected PCs remain present until match end.
- Managers can always issue a mercy stop.

---

## 11) Seeder

Owner: Installer/Seeder Engineer  
Location: `DatabaseSeeder/*`

Flows
- Create an Arena and map room sets.
- Create example Combatant Classes and Event Types (1v1, 2v2).
- Register default rating prog and example outfit progs.

---

## 12) Tests

Owner: QA Engineer  
Location: `MudSharpCore Unit Tests/*`

Unit Tests
- Lifecycle transitions (idempotency, timestamps).
- Betting math (fixed‑odds/pari‑mutuel) and payout blocking.
- Ratings updates (win/loss/draw; Elo deltas).
- Strategy hooks fire on combat callbacks.

Integration Tests
- NPC backfill, inventory snapshot/restore.
- Observation mirroring respects notice/hearing.
- Full event scenario: schedule → register → live → resolve → payouts.
- Reboot during live cancels and refunds exactly once.

---

## 13) Documentation

Owner: Technical Writer

Deliverables
- Admin guide: configuring arenas, event types, policies, betting, ratings.
- Player guide: signing up, observing, betting, payouts, ratings.
- Prog hooks and parameters: outfit, npc loader, scoring, resolution override, rating update.
- Architecture diagrams: component, state, and sequence.

---

## 14) Final Review & Validation

Owner: Tech Lead

Checklist
- Matches requirements in `Design Documents/Combat_Arenas_Design.md` (features, safety, data‑driven design, business integration).
- Crash safety verified; no double‑payouts/refunds; inventory restored.
- Performance reasonable under typical event sizes; echo mirroring does not overwhelm observers.
- Docs and seeder enable end‑to‑end run on a fresh database.

---

## 15) Phase Plan (Sequential with Parallel Opportunities)

Strict Sequence (must be sequential)
1) Architecture & interfaces (this section)  
2) Data model & migrations  
3) Lifecycle engine & scheduler  

Parallelizable After 1–3 stabilize contracts
- Observation (6) in parallel with NPC Backfill (5) and Commands (9)
- Betting & Finance (7) parallel to Ratings (8); both consume lifecycle outcomes
- Seeder (11) starts after data model (2) + minimal commands (9)

Wrap‑Up
- Tests (12), Documentation (13), Final Review (14)

---

## 16) Prog Hooks (Signatures Sketch)

Naming and registration follow existing FutureProg conventions. Signatures indicate parameter order and return type expectations.

- `Arena.OutfitSide(event: IArenaEvent, sideIndex: number, participants: collection<Character>) -> void`
- `Arena.NpcLoader(event: IArenaEvent, sideIndex: number, slotsNeeded: number) -> collection<Character>`
- `Arena.Scoring(event: IArenaEvent, source: Character, target: Character|Null, action: text, value: number) -> void`
- `Arena.ResolutionOverride(event: IArenaEvent) -> (outcome: text, winningSides: collection<number>)`
- `Arena.RatingUpdate(class: ICombatantClass, participants: collection<Character>, outcomes: collection<text>, currentRatings: collection<number>) -> collection<number>`

Return value conventions
- Boolean as number (1/0) if required by existing patterns; otherwise true bool where supported.

---

## 17) Developer Notes & Conventions

- Use interface‑first approach; engine services depend on interfaces only.
- Persist state transitions and ensure each handler is idempotent to support crash recovery.
- Keep all money operations atomic with clear references; use existing `IBankAccount` and currency abstractions.
- Mirror echoes through Emote with notice/hearing checks; never emit raw `{` in Emote text; follow Emote rules.
- Prefer `StringUtilities.GetTextTable` and `ListToString` for presentation.
- Use `ColourValue`, `ColourError`, `ColourCommand`, `ColourName` conventions for output.

---

## 18) Acceptance Criteria by Phase (Condensed)

Architecture & Interfaces
- All interfaces compile in isolation, documented, placed in `FutureMUDLibrary`.

Data Model
- Migrations apply/rollback cleanly; constraints and indices correct.

Lifecycle
- Transitions are persisted and idempotent; boot recovery aborts unsafe events and issues refunds/inventory restores.

NPC Backfill
- Only fills when loaders exist; BYO respected; inventory restored.
- BYO tagging/reclaim returns pre-event tagged items to owners during cleanup, and applies NPC full-recovery item repair where configured.

Observation
- Observers see mirrored output with correct gating; no hidden info leak.

Betting & Finance
- Stakes custodied; settlements correct; insolvency blocks payouts and allows later collection.

Ratings
- Ratings update on completion using the configured event-type Elo style and K-factor.

Commands
- Managers and players can operate end‑to‑end flows with clear feedback.

Seeder
- Fresh install yields working example arena and event types.

Tests
- Unit/integration suite passes locally and in CI.

Docs & Review
- Guides complete; checklist signed off; design goals met or deviations documented.

---

## 19) Open Questions (Track & Resolve Early)

- Exact mapping of signature items/colour: store as text and ANSI colour name, or references to item templates?
- Tax accounting specifics: align with existing economy tax period model.
- Minimum odds/house edge parameters for betting: per arena vs per event type.
- Multi‑room traversal rules for staging (spawn separation only or spawn + enforced initial delay?)

---

## 20) Next Actions

1) Confirm interface set above and adjust names to match any pre‑existing conventions (e.g., `IRoom` usage confirmed).  
2) Draft EF models aligned to interfaces and generate initial migration.  
3) Implement lifecycle scheduler + reboot recovery skeletons with logging.  
4) Start parallel spikes for Observation mirror and Betting math with mock data.

