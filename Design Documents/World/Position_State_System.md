# Position State System

## Purpose

The position state system describes how a perceivable thing is physically arranged in a cell: standing, sitting, swimming, flying, floating, hanging, riding, and similar states. It affects:

- long descriptions and movement echoes;
- whether a character can move immediately or must first change posture;
- which movement speed entry is used;
- contextual height for combat, cover, sleep, infection recovery, and size checks;
- fall safety, swimming, climbing, flying, and zero-gravity behavior;
- relative placement of characters and items against other perceivables.

Position states are posture and movement-mode abstractions. They are not meant to encode every locomotor anatomy. For example, a wheeled or tracked robot can still use an upright/mobile position state, while its body prototype and move speed rows provide "roll", "trundle", or similar verbs. Add a new position state only when the state has distinct engine-wide posture, height, movement, or targeting rules.

## Core Types

`IPositionState` lives in `FutureMUDLibrary/Body/Position/IPositionState.cs`. Concrete runtime implementations live under `MudSharpCore/Body/Position/PositionStates`.

The main contract members are:

- `Upright`: whether the position counts as upright for effects such as exit size checks, standing combat assumptions, and falling decisions.
- `MoveRestrictions`: one of `Free`, `FreeIfNotInOn`, `Restricted`, `Climbing`, `Swimming`, `Flying`, or `ZeroGravity`.
- `TransitionOnMovement`: the state a character should use when movement begins from this state, usually standing for free upright variants.
- `DescribePositionMovement`: the intralocal transition verb pair such as `step|steps` or `crawl|crawls`.
- `DescribeLocationMovementParticiple`: the label used by move speed builders and position lookup.
- `SafeFromFalling`: whether the state protects against fall exits or fall layer transitions.
- `Describe(...)`: produces the long-description suffix for an `IPositionable`.
- `DescribeTransition(...)`: produces the emote used when a character changes position.
- `CompareTo(dynamic state)`: reports relative height for sleep, combat, cover, infection recovery, and contextual size logic.

`IPositionable` is implemented by characters, bodies, items, perceived items, and temporary perceivables. It stores the current `PositionState`, `PositionTarget`, `PositionModifier`, optional `PositionEmote`, and reverse `TargetedBy` references.

`PositionModifier` represents target relationship: `In`, `On`, `Under`, `Before`, `Behind`, `None`, or `Around`. `None` means "by" for the base position description, but some states override it. For example, leaning and slumped states describe `None` against a target as "against", and hanging describes `None` as "from".

## Registry and Persistence

`PositionState.SetupPositions()` registers singleton state instances by numeric ID. Those IDs are persisted directly in database rows and should not be renumbered.

Current registered IDs:

| ID | Class | Label | Movement restriction | Height model |
| --- | --- | --- | --- | --- |
| 0 | `PositionUndefined` | existing | restricted by default | undefined |
| 1 | `PositionStanding` | standing | free | standing tier |
| 2 | `PositionSitting` | sitting | restricted by default | sitting tier |
| 3 | `PositionKneeling` | kneeling | restricted by default | kneeling tier |
| 4 | `PositionLounging` | lounging | restricted by default | sitting/low tier |
| 5 | `PositionLyingDown` | lying down | restricted by default | low tier |
| 6 | `PositionProne` | prone | free | low/crawling tier |
| 7 | `PositionProstrate` | prostrate | free if not in/on a target | low/crawling tier |
| 8 | `PositionSprawled` | sprawled | restricted by default | lowest tier |
| 9 | `PositionStandingAttention` | standing at attention | free | standing tier |
| 10 | `PositionStandingEasy` | standing at ease | free | standing tier |
| 11 | `PositionLeaning` | leaning | free | standing tier |
| 12 | `PositionSlumped` | slumped | restricted by default | sitting tier |
| 13 | `PositionHanging` | hanging | restricted by default | undefined |
| 14 | `PositionSquatting` | squatting | free | standing/kneeling boundary |
| 15 | `PositionClimbing` | climbing | climbing | climbing tier |
| 16 | `PositionSwimming` | swimming | swimming | standing/swimming tier |
| 17 | `PositionFloatingInWater` | floating | restricted by default | swimming tier |
| 18 | `PositionFlying` | flying | flying | standing/flying tier |
| 19 | `PositionRiding` | riding | restricted | sitting tier for height comparisons |
| 20 | `PositionFloatingInZeroGravity` | floating | zero gravity | standing/floating tier |

`BodyPrototype` loads valid positions from `BodyProtosPositions`. If an older prototype has no position rows, it falls back to a broad default set. Move speeds load from `MoveSpeeds` and point to a position ID. `Body.CurrentSpeeds` is then keyed by `IPositionState`, so a body can only move in positions for which it has speed rows.

The zero-gravity bootstrap path ensures every body prototype has a zero-gravity floating move speed, copying flying, standing, or any available speed as a template.

## Command Flow

`PositionModule` owns player-facing posture commands:

- `stand`, `stand easy`, `stand attention`;
- `sit`, `rest`, `lounge`, `sprawl`, `prone`, `kneel`, `prostrate`, `squat`;
- `lean` and `slump` for characters, or for items when the next argument resolves to a local item;
- `position`, `hang`, `lean`, and `slump` for item positioning;
- `fly`, `land`, `swim`, `climb`, `dive`, and `ascend` for movement-mode positions;
- `pmote` and `omote` for long-description position emotes.

For character positions, `_positionRegex` parses the desired state, optional modifier, optional target, optional pmote in square brackets, and optional transition emote in parentheses. `Position_General` then:

1. chooses the desired `PositionState`;
2. validates emote and pmote text;
3. maps textual modifiers onto `PositionModifier`;
4. resolves the target, including table and chair special handling;
5. calls `MovePosition(...)` or `ResetPositionTarget(...)`.

For item positions, `_positionItemRegex` parses command, item, optional modifier, optional target, optional omote, and optional actor emote. `position <item> <target>` uses the default `by` modifier. `hang`, `lean`, and `slump` set the item's position state as well as its target relationship. Reset clears the target, modifier, state, and omote through `SetPosition(PositionUndefined.Instance, PositionModifier.None, null, imote)`.

## Movement Flow

`CharacterMovement.GetRequiredMovementPosition` chooses mandatory movement states from exits and terrain:

- climb exits require `PositionClimbing`;
- fly exits require `PositionFlying`;
- swim transitions require `PositionSwimming`;
- zero-gravity cells require `PositionFloatingInZeroGravity` unless the transition is swim-only, swim-to-land, or fly-only.

`CanMoveInternal` rejects `Restricted` positions and `FreeIfNotInOn` positions that are currently in or on a target. It then chooses a moving position:

- swimming, climbing, flying, and zero-gravity floating keep their own state;
- other states use `TransitionOnMovement` if present, otherwise the current state;
- zero gravity can replace that with `PositionFloatingInZeroGravity`.

`CouldMove` follows similar rules and finds an available `IMoveSpeed` for the moving position. The ordinary fallback path only tries standing, prostrate, prone, and zero-gravity floating. This is why non-legged actors should generally be modeled with appropriate standing/upright move speeds rather than a new "wheeled" or "tracked" posture state.

Movement display text comes primarily from `IMoveSpeed` (`FirstPersonVerb`, `ThirdPersonVerb`, `PresentParticiple`), not from `IPositionState.DescribeLocationMovement3rd`.

## Runtime Consumers

Position states are consumed outside the position module in several important places:

- `CharacterPosition` validates whether a target can accept a position and whether a character can change state, including body ability checks such as sitting up.
- `BodyMovement` exposes current and available speeds.
- `BodyBiology.CurrentContextualSize` changes effective size for swimming, prone-like states, exits, explosive damage, and mounted contexts.
- `CharacterMovement` uses `SafeFromFalling`, `MoveRestrictions`, `TransitionOnMovement`, and move speed availability.
- `ZeroGravityMovementHelper` enforces floating state for characters and items in zero gravity.
- `Cell` changes items and characters to floating-in-water or zero-gravity floating when room layers demand it.
- combat strategies and combat moves use `CompareTo` for cover height, firing posture, ranged bonuses, melee assumptions, and forced movement decisions.
- sleep commands and animal AI compare the current state to a race's `MinimumSleepingPosition`.
- infection recovery checks posture height against lounging.
- mount code sets rider and mount states through riding or mounted movement calls.
- builder combat actions and ranged cover builders use `PositionState.GetState(string)` for position lookup.

## Description Rules

Base `PositionState.Describe` uses:

- no target: `<default description> here`;
- target with `Before`, `Behind`, `In`, `On`, `Under`, or `Around`: explicit preposition plus target;
- target with `None`: `by <target>`.

State overrides handle special English:

- leaning/slumped plus `None` target: "against";
- hanging plus `None` target: "from";
- undefined omits a posture word;
- floating-in-water and hanging do not inject "here" before a target phrase.

`DescribeTransition` is only valid for states that characters can directly move into through normal posture commands. Runtime-set states such as undefined, hanging, floating-in-water, and riding deliberately throw if a caller asks them for a transition emote.

## Height Comparison

Height comparison is not a strict numeric ordering. It is a posture relationship used by game rules. Standing-like states are higher than sitting and prone-like states; sprawled is effectively lowest; flying, swimming, climbing, and zero-gravity floating map into existing comparison tiers.

The dynamic fallback must never recursively call itself. When adding a new `PositionState` class, add an explicit comparison overload or choose a base-state mapping in `PositionState.CompareTo(dynamic state)`.

Current base mappings:

- flying and zero-gravity floating compare as standing;
- climbing compares as sitting;
- swimming and floating-in-water compare as swimming/standing tier;
- slumped and lounging compare as sitting tier;
- leaning, standing attention, standing easy, and squatting compare through standing;
- riding compares as sitting tier;
- hanging and undefined return `Undefined`.

## Physiology Guidance

Valid positions and movement verbs belong on body prototypes and move speed data. This is the right extension point for most animals and robots:

- legged humanoids and many quadrupeds use standing/prostrate/prone plus race-appropriate movement verbs;
- swimming bodies need swimming speeds and valid swimming positions;
- flying bodies need flying speeds and enough wings to satisfy flight checks;
- climbing-capable bodies need climbing speeds and usable limbs that make sense for the body plan;
- wheeled or tracked robots should usually keep an upright/mobile posture state and use move speeds such as "roll", "drive", "trundle", or "crawl";
- robots that cannot crouch, kneel, sit, or go prone should omit those valid positions from their body prototype;
- robots that can brace, dock, fold, or park in a way that has distinct height/combat/movement consequences may justify a future generic state, but that should be modeled as posture rather than locomotion media.

Avoid adding separate states such as "wheeled", "tracked", or "legged" unless engine rules genuinely need to distinguish them after move speeds, limbs, and body prototype positions have been considered.

## Known Gaps and Future Work

- `PositionFloatingInWater` and `PositionFloatingInZeroGravity` both use the lookup text `floating`. `PositionState.GetState("floating")` returns the first registered match, which is water floating. If builders need to select zero-gravity floating by text, introduce unique aliases or a stricter lookup flow before changing persisted IDs.
- `DescribeLocationMovement3rd` is effectively unused and only zero-gravity floating overrides it. Either remove it from the contract in a breaking cleanup or give every state a meaningful value.
- `CheckClimbingStillValid` is still a placeholder. Climbing validity should eventually re-check room layer, climb exits, limb usability, and whether the character still has a legal climbing target.
- `MostUprightMobilePosition` only considers standing, prostrate, and prone outside the current fall-safety state. That is suitable for current movement rules, but any future generic "braced/mobile" state would need to participate there and in `CouldMove`.
- Body prototype tooling should make it easier to audit valid positions and move speeds together, especially for non-humanoid animals and robots.
- Combat builder position lookup should be revisited when duplicate or alias-heavy state labels are introduced.
