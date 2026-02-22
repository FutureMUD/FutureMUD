# System Design Document: Combat Arenas (FutureMUD) - **Repository-Agnostic Revision**

> **Note for Codex Agent**
> This document intentionally **omits concrete interface/class definitions and method signatures**. Implementations must align with the existing FutureMUD codebase patterns (projects, namespaces, base interfaces, event/callback mechanisms, EF models, scheduler, finance, prog hooks, combat events). Where this document mentions concepts like "interfaces," "events," or "models," treat them as **contracts and responsibilities**, not prescriptive code.

## 1) Purpose & Summary
Arenas are configurable venues that host structured combats - manual or automated - between PCs and/or NPCs, with registration windows, inventory handling, NPC backfill, spectator viewing, betting, ratings, and post-fight cleanup. Arenas are businesses tied to Economic Zones (banking, solvency, taxation), can be managed by non-admins, and integrate with FutureProg hooks at key stages.

This feature must remain **data-driven**, **extensible**, and **safe under failure** (cancel and refund on reboot).

### 1.1 Implementation Snapshot (2026-02-22)
- Event types now expose explicit elimination terms via `ArenaEliminationMode`: `NoElimination`, `PointsElimination`, `KnockDown`, `Knockout`, `Death`.
- Event types now include an explicit `AllowSurrender` toggle.
- Player surrender is implemented as `arena surrender [<event>]`; while in combat, this is the only allowed `arena` subcommand.
- Live events are monitored on a short scheduler interval and auto-transition to resolving when elimination conditions are met (instead of waiting for timeout only).
- NPC cleanup now respects dead combatants: dead NPCs are never forced to a non-dead state, and corpses are moved to stable/after-fight locations.
- Arena no-quit/no-timeout phase effects now self-prune if their referenced event is no longer active, preventing stale saved effects from blocking players after crashes.

## 2) Core Concepts (finalized)
- **Combat Arena** = venue + operator. Belongs to an **Economic Zone**; behaves like a business (P&L, tax per period; bank/virtual cash; solvency enforced).
- **Managers**: configure event types, schedules, fees, funds, NPC policies, launch/abort events, and reserve slots; no admin needed except to wire NPC loader progs.
- **Combatant Classes**: name/description; eligibility prog; optional admin-only NPC loader prog; "resurrect NPC on death" flag (**full restoration**, like guest avatars); per-combatant identity metadata (stage name, signature colour/items).
- **Event Types**: immutable templates (multi-side, multi-room support); define side capacities (fixed at type), eligible classes per side, fees (appearance/victory), BYO toggle, per-side outfit prog, scoring prog, elimination mode, surrender policy, NPC signup/auto-fill flags, and registration/prep/time-limit durations. Managers can **clone** types for quick variants (for example, different capacities).
- **Elimination Terms**: event types define explicit elimination modes (`NoElimination`, `PointsElimination`, `KnockDown`, `Knockout`, `Death`). Higher-severity outcomes still count (for example, death satisfies knockout/knockdown conditions).
- **Surrender**: event types can explicitly enable/disable surrender. Combatants can surrender with `arena surrender`.
- **Events (instances)**: created from an Event Type, optionally with **reserved slots** for characters/clans when **manually launched**; progress via a lifecycle (below).
- **Observation**: **remote viewing only**, via an arena-scoped watcher effect mirroring in-arena output with appropriate audio "quietening" and notice checks for subtle actions.
- **Betting**: **custodied** wagers (stake moved immediately). Two selectable models: **Fixed-Odds** (snapshot at bet time) and **Pari-Mutuel** (pool). **Draw** outcome supported. If arena lacks payout funds, **payout is blocked** and can be collected later.
- **Ratings**: Elo-style per **Combatant Class** (not global), driven by a **prog hook**; supply a default Elo rating prog.
- **Crash/Reboot**: mid-match -> **cancel event** and **refund wagers** (no state restore).
- **Disconnects**: PCs cannot quit (effect); no linkdead auto-logout in an event. If disconnected, they **remain** in the match.
- **Mercy Stoppage**: allowed when **all other sides** are incapacitated; managers can always stop a fight manually.

## 3) Spatial Model & Requirements
Arena configuration must include room sets:
- **Waiting Rooms** (>=1, ideally per side)
- **Arena Rooms** (1..N). For multi-room arenas, different teams **start in different rooms** when available; fights may traverse rooms.
- **Observation Rooms** (remote watcher effect)
- **Infirmary** (PC destination on elimination)
- **NPC Stables** (spawn/return/resurrect)
- **After-Fight Rooms** (PC dressing/debrief)

Block event start until the required rooms for the Event Type are present and reachable.

## 4) Event Lifecycle (state machine)
```
Draft -> Scheduled -> RegistrationOpen -> Preparing -> Staged -> Live -> Resolving -> Cleanup -> Completed
                          \-> (Abort) -> Aborted
```
- **Draft**: instance created from type; capacities fixed by type; optional reserved slots configured on manual launch.
- **Scheduled**: waiting for registration open time.
- **RegistrationOpen**: PCs/NPCs sign up (per-side policies). Bets accepted. Reserved slots enforced. If NPC auto-fill is enabled but the side has **no loader prog**, auto-fill is prohibited.
- **Preparing**: close registration; NPC backfill; inventory bundle/strip; outfit progs; signature items/colour; teleport to waiting rooms (per side).
- **Staged**: intro prog/echoes; teleport to arena rooms (teams start separately when available); countdown.
- **Live**: combat proceeds; scoring and elimination receive combat callbacks; observers see mirrored output with hearing/notice rules.
- **Live (termination)**: scheduler polling checks elimination conditions continuously; time limit remains a fallback/end-cap.
- **Resolving**: decide win/draw (draws supported on time tie); award victory/appearance fees; settle bets (block payouts if insolvent); update ratings via prog.
- **Cleanup**: reclaim signature items (PC/NPC); confiscate illegal kit per policy; restore bundled inventory; teleport PCs to after-fight/infirmary; NPCs to stables with full restore if flagged. Dead NPCs are handled as corpses for relocation and are not forced to non-dead states.
- **Completed**: immutable record.
- **Aborted**: manual stop or pre-live failure; default: refund wagers, no appearance/victory fees unless arena policy says otherwise.

**Timers/Drivers**: use the game scheduler/heartbeat; arenas may attach schedules (for example via `IProgSchedule` or equivalent). No cross-reboot restoration.

## 5) Strategies (plugin design; responsibilities only)
### 5.1 Elimination
Current built-in elimination modes:
1. **NoElimination**
2. **PointsElimination**
3. **KnockDown**
4. **Knockout**
5. **Death**

Current behaviour rules:
- Death satisfies knockout/knockdown conditions.
- Surrender is treated as elimination only when event type `AllowSurrender` is enabled.
- Points elimination requires a scoring prog and can resolve via either explicit winning side indexes or by highest side score.
- Mercy stoppage still applies when only one side remains non-eliminated.

**Responsibilities** (to be mapped to repo constructs):
- Subscribe to combat events (hit/wound/KO/death/surrender).
- Maintain per-combatant elimination state.
- Expose terminal-condition and winner-resolution state to lifecycle services.

### 5.2 Scoring
Minimum strategies:
1. **TotalHits** (landed legal strikes; configurable: contact vs damage>0)
2. **HitsToVitals** (uses "Vital" bodypart flag; counts damaging hits)
3. **HitsWithSeverity(>=X)**
4. **NoScore**

**Responsibilities**:
- Subscribe to combat events.
- Maintain per-side tallies.
- Expose score per side and "victory reached?" based on strategy rules.

## 6) Betting & Finance
**Models**:
- **Fixed‑Odds**: odds fixed at bet time from ratings; configurable house edge; stake **custodied** at placement.
- **Pari‑Mutuel**: pool per outcome; dynamic payouts at close; configurable takeout.

**Solvency & Settlement**:
- Custody stake to arena account immediately.
- Settlement **blocks payouts** if insolvent; players may collect later when funds exist.
- Maintain a **withholding ledger** for unsettled liabilities to avoid taxing phantom profit.

**Outcomes**:
- Sides + **Draw** supported across both models.


## 7) Ratings & Records
- **Per Combatant Class** Elo‑style rating (not global).  
- Use a **rating‑delta prog hook** with event details; supply a **default Elo prog** configurable for K/decay.  
- Performance record fields per combatant (per class, optionally per event type): W/L/D, rating, last N bouts, stage name, signature colour/items.


## 8) Inventory & Equipment
- **BYO=false**: bundle/strip to secure bundle (same mechanism as jail); apply outfit prog; assign signature gear; always reclaim signature gear.
- **BYO=true**: optional screening prog may block contraband; assign signature items (override precedence); reclaim on cleanup for **all** combatants.


## 9) Visibility (Watcher Effect)
- Arena‑scoped watcher effect (separate from Admin Spy) mirroring **all** in‑arena outputs to observation rooms.
- Hearing model reduces audio volume; **notice** checks gate subtle actions (via EmoteOutput flags or equivalent).
- Private whispers remain private unless normal audibility would allow overhearing.


## 10) Roles & Permissions
- **Admin**: can set NPC loader progs; full control.
- **Property Owner/Lease**: hires/fires managers.
- **Manager (unrestricted)**: configure event types, fees, schedules; launch/abort events; reserve slots; manage funds (deposit/withdraw/transfer); choose betting model; toggle NPC signup/backfill.


## 11) Taxation (Economic Zones)
- Tax on **net profit per period**.
- Track **withheld liabilities** for unsettled wagers so P&L reflects exposure.


## 12) Failure Modes & Policy
- **Reboot mid‑event**: cancel event; refund wagers; no restoration.
- **Disconnect**: participants remain; quitting/linkdead prevention effects applied.
- **Missing NPC loader with auto‑fill**: do not fill; event may start under capacity if allowed by type (per‑type flag).


## 13) Integration Contracts (to be defined by Codex)
> The following are **capabilities** and **responsibilities** to implement, not fixed signatures. Map them to the existing abstractions (e.g., ICharacter, finance, banking, rooms/locations, combat events, FutureProg, scheduler, effects).

### 13.1 Arena Core
- **Arena entity** with: economic zone, bank/virtual cash, solvency check (including withheld liabilities), managers list, combatant classes, event types, room set, betting/ledger facade, status string.
- **Event instance** that exposes: state, schedules/timestamps, per‑side rosters and capacity, reserve/withdraw/registration APIs, betting API, lifecycle transitions (start/abort), and a tick/heartbeat driver for timed transitions.
- **Room set** abstraction pointing to configured waiting/arena/observation/infirmary/stables/after‑fight locations.
- **Combatant identity** abstraction (PC/NPC) with stage name, class, performance record, signature colour/items; helpers to bundle/restore inventory and equip signature items.

### 13.2 Strategies
- **Scoring** and **Elimination** abstractions that receive combat events and maintain internal state; factories to build them from serialized config (data‑driven).

### 13.3 Betting & Ledger
- **Betting book** with pluggable model (fixed‑odds/pari‑mutuel), solvency checks, custody on placement, settlement on outcome, payout blocking when insolvent, and withheld liabilities ledger. 

### 13.4 Hooks & Effects
- **FutureProg hooks** at: event creation, registration open, preparing, staged, live, resolve, cleanup; per‑actor elimination/kill/crit‑injury; rating adjust (Δ). 
- **Watcher effect** scoped to arena; mirrors output respecting hearing/notice rules.


## 14) Persistence & Data Model Outline (repo‑agnostic)
Treat the following as **storage shape suggestions**; map to existing EF conventions and composed models in the repo. Use compact serialized fields (XML/JSON) for strategy configs and tallies to avoid schema churn.

- **CombatArena**: zone, property/lease, bank account?, use virtual cash, managers, room sets.
- **CombatantClass**: arena, name/description, eligibility prog, NPC loader prog?, resurrect‑on‑death flag.
- **EventType**: arena, name/description, allow BYO, registration/prep/time‑limit, scoring config, elimination config, hooks, per‑side definitions (name/desc, capacity, appearance/victory fees, eligible classes, allow NPC signup, auto‑fill, outfit prog).
- **EventInstance**: type, arena, state, timestamps, rosters (incl. reservations), scores, outcome, per‑side data.
- **Wager**: event, bettor, outcome, stake, odds snapshot/pool share, payout, state, payment method snapshot.
- **Performance**: combatant, class, rating, W/L/D, signature colour/items, stage name.


## 15) Command Surface (no signatures; user‑facing spec)
**Admin/Manager**
- `arena create <name>`
- `arena set <arena> economiczone|property|bank|virtualcash <...>`
- `arena rooms set <arena> waiting|arena|observe|infirmary|stable|after <room ids...>`
- `arena class add|edit|remove …`
- `arena eventtype add|edit|remove …` (sides, capacities, fees, BYO, outfit prog, scoring, elimination, NPC policies)
- `arena eventtype clone <arena> <eventtype> as <newname> [mutations…]`
- `arena schedule <arena> <eventtype> open <time> start <time>`
- `arena start <event> [reserve side:<n> whom:<char|clan> [ttl:<span>] …]`
- `arena abort <event> <reason>`
- `arena fund <arena> deposit|withdraw <amount> [bankaccount]`
- `arena status <arena>` (balances, withheld, upcoming, live, liabilities)

**Players**
- `arena list` / `arena events <arena>` / `arena view <event>`
- `arena signup <event> side <n>` / `arena withdraw <event>`
- `arena surrender [<event>]` (allowed only for current live participants when event type permits surrender)
- `arena bet <event> <side|draw> <amount> from <cash|account>`
- `arena collect <event|all>`
- `arena spectate <arena>`


## 16) Prog Hooks (context & intent only)
- Arena/Event lifecycle: `OnEventCreated`, `OnRegistrationOpened`, `OnPreparing`, `OnStaged`, `OnLive`, `OnResolve`, `OnCleanup`
- Per‑actor: `OnEliminate`, `OnKill`, `OnCritInjury`
- Ratings: `OnRatingAdjust` (returns Δ/override)
- Hooks receive a context map (event, arena, side, combatant, cause, etc.) using existing **ProgVariableTypes**.


## 17) Implementation Plan (tasks for Codex)
1. Define repository‑conformant abstractions for: Arena, Event Type, Event Instance, Combatant Class, Room Set, Scoring/Elimination strategies, Betting Book & Models, Performance Record.  
2. Wire them into the gameworld registry (`All`/`IUneditableAll` patterns) and loading/saving flow.  
3. Add persistence models & migrations; serialize strategy configs and tallies.  
4. Implement **ArenaModule** commands (list/view/status/signup/surrender/bet/spectate first; creation/edit next).  
5. Integrate scheduler/heartbeat to drive lifecycle transitions; **no** cross‑reboot restore—cancel & refund instead.  
6. Hook combat events to strategy engines; enforce elimination/score rules and mercy stoppage by incapacitation.  
7. Build arena‑scoped watcher effect honoring hearing/notice/privacy rules.  
8. Implement inventory flows (bundle/restore), outfit progs, signature gear reclaim.  
9. Implement betting (custody, settlement, payout blocking), withheld liabilities ledger, and period tax integration.  
10. Supply default **Elo rating prog**; document K/decay configuration.  
11. QA: multi‑room starts, reservations, missing NPC loaders, insolvent payouts, disconnects, mercy stoppage, draws across both betting models.


## 18) Remaining Risks & Notes
- **Reserved slot no‑show**: default to convert to normal slot and allow NPC auto‑fill (if enabled and loaders exist).  
- **Under‑capacity starts**: allow when manually started; otherwise enforce full or min‑per‑side (per‑type setting).  
- **Whisper leakage**: ensure mirrored stream runs *after* normal audibility/notice checks.  
- **Draw pricing**: Fixed‑odds requires explicit draw price; pari‑mutuel requires draw pool.  


---

## 19) User Stories

### Story A — PC: Register & Fight in Multi‑Room Arena
**As a** player  
**I want** to sign up for a team bout in a multi‑room arena, get safely stripped/outfitted if required, start in a different room from the other team, and have my gear restored after  
**So that** I can enjoy fair, high‑stakes PvP without risking persistent gear.  
**Acceptance**  
- Fails if I’m ineligible by class.  
- On BYO=false, my inventory is bundled, I’m outfitted via `OutfitProg`, and signature items are assigned.  
- I start in my team’s starting room; observers see the mirrored fight.  
- On elimination, I’m sent to the infirmary; at Cleanup my inventory is restored.  
- W/L/D recorded; rating updated via prog; I can collect winnings if any.

### Story B — Manager: Manual Launch with Reserved Slots & NPC Backfill
**As a** manager  
**I want** to clone a popular 3v3 event type at a different capacity, reserve one slot per side for headliners, and auto‑fill remaining slots with NPCs (where loader progs exist)  
**So that** I can run a marquee fight on schedule.  
**Acceptance**  
- `arena eventtype clone` produces a new type with adjusted capacities.  
- `arena start` accepts `reserve` arguments for specific characters/clans, with TTL.  
- Registration opens; non‑reserved slots fill normally; at close, NPC auto‑fill occurs where permitted and loaders exist; missing loaders prevent fill but do not block the event.  
- Event completes; fixed‑odds or pari‑mutuel payouts settle; blocked if insolvent.  
- Tax ledgers reflect withheld liabilities; period profit shows correctly.
