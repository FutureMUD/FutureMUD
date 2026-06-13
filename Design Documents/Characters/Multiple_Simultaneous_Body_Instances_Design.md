# Multiple Simultaneous Character Bodies / Character Instances

**Status:** Draft design proposal  
**Area:** Characters, bodies, forms, controllers, movement, combat, effects, persistence  
**Related document:** `Design Documents/Characters/Multiple_Body_Form_System.md`  
**Primary design goal:** Extend the existing multiple-body-form system from exclusive body switching to simultaneous embodied presences, while preserving current single-body behaviour and existing form-switch content.

---

## 1. Executive Summary

FutureMUD currently supports one `ICharacter` owning multiple `IBody` forms, with logic for provisioning, maintaining, and switching between those forms. The current system is deliberately conservative: there is only one active body in the world at a time, and inactive bodies are dormant form records rather than independent room occupants. This is appropriate for werewolf-style transformation, robot/organic switching, ghost forms, animal polymorphs, and death backup flows, but it is not sufficient for astral projection, mirror images, magical copies, multiple controllable shells, or simultaneous clone bodies.

The core change proposed by this document is to introduce a world-presence layer:

```text
Character Identity
  owns durable identity, account/NPC template, name, skills, culture, roles, knowledge,
  character-scoped merits, form ownership, and long-term state

Character Instance / Presence
  represents one active world actor in one location/layer, with one embodied body,
  movement, combat, position, perception, command focus, immediate state, and death policy

Body
  owns anatomy, health, wounds, body-scoped attributes, inventory slots, implants,
  prosthetics, breathing, stamina, body descriptions, and physical state
```

The current `ICharacter` type is used throughout the engine as both durable identity and world actor. The least risky path is therefore to treat future `ICharacter` implementations as **world actor instances** during the transition, while adding an explicit identity relationship. In world, movement, combat, perception, and targeting contexts, `ICharacter` should mean an active character instance. Identity-scoped systems should move to a new identity abstraction over time.

The existing body-form switch pipeline should remain intact. It is still the right mechanism for exclusive transformations. Simultaneous bodies should be implemented as a second lifecycle beside switching: spawn/attach an active instance to an owned form body, insert that instance into the world, route control/perception according to policy, and later retire/kill/despawn that instance.

---

## 2. Current State

### 2.1 Existing Multiple Body Form System

The existing multiple body form system lets one character own multiple bodies and switch which one is active. It includes:

- `ICharacter.Forms`
- `ICharacter.Bodies`
- `ICharacter.CurrentBody`
- `ICharacter.EnsureForm(...)`
- `ICharacter.CanSwitchBody(...)`
- `ICharacter.SwitchToBody(...)`
- `CharacterBodies` form metadata
- `CharacterBodySources` stable source mappings
- form aliases, sort order, trauma mode, voluntary switching gates, visibility progs, and transformation echoes
- body switch trauma transfer/stash logic
- death backup body transfer logic
- non-final body remains support

The existing design explicitly models the character as stable identity and the body as physical embodiment, but the runtime still exposes one active body at a time.

### 2.2 Important Current Implementation Assumptions

The following assumptions are embedded throughout the codebase:

1. **One active `ICharacter` represents one world presence.**
   Locations store `ICharacter` instances directly in their character lists.

2. **One active body exists per character.**
   `ICharacter.CurrentBody` is currently just the character's `Body`.

3. **`IBody` is not an independent world occupant.**
   The concrete `Body` class proxies location and output through its actor. `Body.Location` returns `Actor.Location`, and `Body.MoveTo(...)` delegates to `Actor.MoveTo(...)`.

4. **Switching replaces the character's body.**
   The switch pipeline sets `Body = newBody`, activates the new body, applies a switch plan, resets the old body to dormant state, emits a transformation echo, and fires `CurrentBodyChanged`.

5. **Dormant forms must be dormant.**
   The switch pipeline rejects target bodies that are not dormant shells, especially bodies with direct inventory or transfer-incompatible state.

6. **Persistence has one character location and one active body.**
   `Characters.BodyId`, `Characters.Location`, and `Characters.RoomLayer` represent the active in-world character.

7. **Death is character death by default.**
   `Character.Die()` finalises character death unless a body backup intercepts it. It creates corpse/remains, changes character state/status, moves the controller away, calls `Body.Die()`, and destroys the character from the gameworld.

These assumptions are not bugs in the current design. They are the correct simplifications for exclusive transformations. They become blockers only when multiple bodies need to exist in the world at the same time.

---

## 3. Problem Statement

Future content needs one actual character identity to support multiple simultaneous embodied presences, such as:

- astral projection while the physical body remains in the world
- magical copies or mirror images
- physical clones
- remote bodies, puppets, shells, or sleeves
- possession-style control of another body
- NPCs with multiple bodies or distributed presences
- boss mechanics involving multiple simultaneous instances

These presences may need independent locations, perception, combat targeting, health, inventory, movement, death policies, and controller focus. Some may be fully physical and autonomous. Others may be intangible, planar, helpless, passive, or tethered to a primary body.

The current model cannot support this by simply activating several `IBody`s because `IBody` depends on a single owning `ICharacter` for location, output, event handling, and much of combat identity. The engine needs a new distinction between:

```text
Who is this person/identity?
Which physical or metaphysical presence is acting right now?
Which body supplies anatomy and physical state for that presence?
```

---

## 4. Goals and Non-Goals

### 4.1 Goals

1. Preserve all existing single-body character behaviour.
2. Preserve the existing exclusive body-form switch system.
3. Add support for more than one active in-world presence for one identity.
4. Allow active presences to be PCs or NPCs.
5. Support player focus switching between owned active presences.
6. Support NPC AI on multiple active presences.
7. Support distinct death policies for physical body death, projection death, clone death, and final identity death.
8. Support identity-scoped skills/knowledge/social state and body-scoped anatomy/health/attributes/inventory.
9. Allow builder and prog content to create, configure, inspect, and retire active instances.
10. Keep migration of existing data deterministic and reversible as far as practical.

### 4.2 Non-Goals for Initial Release

1. Do not rewrite the entire engine to replace `ICharacter` everywhere at once.
2. Do not make `IBody` itself the primary world actor.
3. Do not allow one PC to act through multiple bodies simultaneously by default.
4. Do not implement fully autonomous clone armies before the instance architecture is proven.
5. Do not remove `Characters.BodyId`, `Characters.Location`, or `Characters.RoomLayer` immediately.
6. Do not change exclusive transformation semantics except where needed for compatibility with the instance model.

---

## 5. Finished-State Architecture

### 5.1 Core Concepts

#### Character Identity

The character identity is the durable person/entity. It owns identity-level state:

- account ownership or NPC template provenance
- personal name and aliases
- culture
- roles
- community/clan membership
- skills and theoretical skills
- knowledge and scripts
- character-scoped merits
- social memory and dubs where appropriate
- magic resource pools if configured identity-wide
- form ownership and source mappings
- long-term plans and administrative metadata

The identity is not, by itself, a room occupant.

#### Character Instance / Presence

A character instance is one active presence in the world. It owns or references instance-level state:

- embodied body
- location and room layer
- position state, target, modifier, and emote
- movement queue and active movement
- combat participation and target
- aim, cover, melee range, targeted bodypart
- command context and controller focus eligibility
- immediate state such as awake/asleep/unconscious/stasis for that presence
- instance effects such as hiding, guarding, dragging, grappling, action effects, projection tether, and focus locks
- death policy
- control policy
- perception policy
- persistence policy

During transition, active instances should implement `ICharacter` so existing world code continues to work.

#### Body

The body remains the anatomy and physical-state object. It owns:

- race, ethnicity, gender, body prototype
- body-scoped attributes and derived attributes
- height and weight
- limbs, bodyparts, organs, bones
- wounds, infections, scars, tattoos
- implants and prosthetics
- wearable/wield/hold inventory slots
- breathing strategy and physiology
- body stamina and exertion if body-local
- body description patterns
- body effects

A body may be dormant, embodied by one active instance, retired, dead, or destroyed.

### 5.2 Suggested Interfaces

The exact names are negotiable. The key requirement is the responsibility split.

```csharp
public interface ICharacterIdentity : IFrameworkItem
{
    IEnumerable<ICharacterInstance> Instances { get; }
    ICharacterInstance PrimaryInstance { get; }
    ICharacterInstance? FocusedInstance { get; }

    IEnumerable<ICharacterForm> Forms { get; }
    IEnumerable<IBody> Bodies { get; }

    IAccount Account { get; }
    IPersonalName PersonalName { get; }
    ICulture Culture { get; }
    IEnumerable<ITrait> CharacterTraits { get; }
    IEnumerable<IKnowledge> Knowledges { get; }
    IEnumerable<IMerit> CharacterMerits { get; }
}
```

```csharp
public interface ICharacterInstance : ICharacter
{
    ICharacterIdentity Identity { get; }
    long InstanceId { get; }
    IBody Body { get; }
    CharacterInstanceKind InstanceKind { get; }
    CharacterInstanceControlPolicy ControlPolicy { get; }
    CharacterInstanceDeathPolicy DeathPolicy { get; }
    CharacterInstancePerceptionPolicy PerceptionPolicy { get; }
    CharacterInstancePersistencePolicy PersistencePolicy { get; }

    bool IsPrimaryInstance { get; }
    bool IsControllable { get; }
    bool IsEmbodied { get; }

    bool SameIdentity(ICharacter other);
    bool SamePhysicalInstance(IPerceivable other);
}
```

### 5.3 Suggested Enums

```csharp
public enum CharacterInstanceKind
{
    Primary = 0,
    AstralProjection = 1,
    MagicalCopy = 2,
    PhysicalClone = 3,
    Puppet = 4,
    PossessedBody = 5,
    RemoteShell = 6,
    Other = 99
}
```

```csharp
public enum CharacterInstanceControlPolicy
{
    NotControllable = 0,
    PlayerFocusable = 1,
    PlayerRemoteCommandable = 2,
    NpcAiControlled = 3,
    IdentityCoordinatorControlled = 4,
    ScriptOnly = 5
}
```

```csharp
public enum CharacterInstanceDeathPolicy
{
    FinalCharacterDeath = 0,
    DestroyInstanceOnly = 1,
    DestroyInstanceAndDamageAnchor = 2,
    CollapseToAnchor = 3,
    TransferControlToAnchor = 4,
    TransferControlToBackup = 5,
    KillIdentityIfNoPrimaryCapableInstance = 6
}
```

```csharp
public enum CharacterInstancePerceptionPolicy
{
    OrdinaryEmbodied = 0,
    FocusedOnly = 1,
    RemoteFeedToIdentity = 2,
    SilentUnlessCritical = 3,
    MergedWithFocusedOutput = 4,
    PlanarProjection = 5
}
```

```csharp
public enum CharacterInstancePersistencePolicy
{
    Persistent = 0,
    TemporaryEffectBound = 1,
    RecreateFromEffectOnLoad = 2,
    DespawnOnLogout = 3,
    DespawnOnReboot = 4
}
```

```csharp
public enum BodyEmbodimentState
{
    DormantForm = 0,
    Embodied = 1,
    Suspended = 2,
    Retired = 3,
    Destroyed = 4
}
```

### 5.4 Finished-State Invariants

The implementation should maintain these invariants:

1. Every active world presence is a character instance.
2. Every active character instance has exactly one embodied body.
3. A body can be embodied by at most one live instance at a time.
4. A dormant form body has no live instance.
5. Every identity has exactly one primary instance unless final-dead or in a special transition state.
6. Existing single-body characters load with one primary instance and behave as they do today.
7. Exclusive body switching operates on one instance and does not create simultaneous bodies.
8. Simultaneous body spawning does not use the body switch transfer/stash pipeline unless explicitly requested.
9. Identity-scoped traits are not duplicated per instance.
10. Instance-scoped heartbeats and body-scoped heartbeats cannot multiply identity-level regeneration or periodic effects unless deliberately configured.
11. Death of an instance only causes final identity death if its death policy says so.
12. “Self” in physical targeting means same instance, not merely same identity.
13. “Same character/person” in social or identity logic means same identity, not necessarily same instance.

---

## 6. Persistence Design

### 6.1 New Table: `CharacterInstances`

`CharacterBodies` should remain the ownership/form metadata table. It should not be overloaded to mean active world presence.

Add a new table:

```sql
CharacterInstances
(
    Id bigint primary key,
    CharacterId bigint not null,
    BodyId bigint not null,

    InstanceName nvarchar(100) null,
    InstanceKind int not null,
    ControlPolicy int not null,
    DeathPolicy int not null,
    PerceptionPolicy int not null,
    PersistencePolicy int not null,

    LocationId bigint null,
    RoomLayer int not null,
    PositionId int not null,
    PositionModifier int not null,
    PositionTargetId bigint null,
    PositionTargetType nvarchar(50) null,
    PositionEmote nvarchar(max) null,

    State int not null,
    Status int not null,
    IsPrimary bit not null,
    IsEmbodied bit not null,
    IsControllable bit not null,

    AnchorInstanceId bigint null,

    CreatedBySourceType int null,
    CreatedBySourceId bigint null,
    CreatedBySourceKey nvarchar(200) null,
    CreatedDateTime datetime2 not null,
    ExpiryDateTime datetime2 null,

    EffectData nvarchar(max) null
)
```

### 6.2 Indexes and Constraints

Recommended constraints:

```text
Unique live primary instance per CharacterId.
Unique live embodied BodyId.
Index CharacterId + IsPrimary.
Index BodyId.
Index LocationId + RoomLayer for active instance lookup if needed.
```

Where the database cannot easily represent “live only” constraints due to status flags, enforce at load/save and via diagnostics.

### 6.3 Compatibility Columns

Keep these existing character columns during migration:

- `Characters.BodyId`
- `Characters.Location`
- `Characters.RoomLayer`
- `Characters.PositionId`
- `Characters.PositionModifier`
- `Characters.PositionTargetId`
- `Characters.PositionTargetType`
- `Characters.PositionEmote`
- `Characters.State`
- `Characters.Status`

During the migration period, these should mirror the primary or focused instance according to the compatibility policy. Recommended policy:

```text
Characters.BodyId, Location, RoomLayer, Position*, State, Status mirror the primary instance.
Current command focus is stored separately for online controllers, not necessarily persisted at first.
```

This makes old queries and admin tools less likely to break. It also avoids confusing database records where the durable character's row appears to teleport whenever the player focuses on an astral body.

### 6.4 Existing Form Tables

Keep:

- `CharacterBodies`
- `CharacterBodySources`
- `Bodies`
- `CharacterTraits`
- `TraitDefinitions.OwnerScope`

The existing form source mapping remains useful for merits, spells, and progs. A form body may be dormant or embodied. Source mapping should not imply active presence.

### 6.5 Effect XML Compatibility

Existing effect XML often assumes the owner is the character or the body. The migration should classify effects into three scopes:

```text
Identity-scoped effects: remain on identity/character root.
Instance-scoped effects: move to primary instance by default.
Body-scoped effects: remain on body.
```

Unknown old effects should default to their current owner, with defensive load handling and diagnostics. It is better to preserve too much old state on the primary instance than to drop effects silently.

---

## 7. Runtime Loading and Saving

### 7.1 Loading Existing Characters

On first migration or fallback load:

1. Load the character identity from `Characters`.
2. Load owned forms and source mappings as today.
3. If no `CharacterInstances` rows exist for the character, create one primary instance using:
   - `CharacterId = Characters.Id`
   - `BodyId = Characters.BodyId`
   - `LocationId = Characters.Location`
   - `RoomLayer = Characters.RoomLayer`
   - existing position fields
   - `InstanceKind = Primary`
   - `ControlPolicy = PlayerFocusable` for PCs, `NpcAiControlled` for NPCs
   - `DeathPolicy = FinalCharacterDeath`
   - `PerceptionPolicy = OrdinaryEmbodied`
   - `PersistencePolicy = Persistent`
4. Attach the body to the primary instance.
5. Expose compatibility properties so current single-body call sites still behave.

### 7.2 Loading Active Simultaneous Instances

For each active instance row:

1. Resolve identity.
2. Resolve body from owned forms/bodies.
3. Validate that no other live instance has the same body.
4. Attach body to instance.
5. Load instance effects.
6. Insert persistent instances into their locations during login/world load.
7. For temporary effect-bound instances, validate the owning effect still exists. If the effect no longer exists, retire/despawn the instance.

### 7.3 Saving

Each active instance should save:

- location and room layer
- position state/target/modifier/emote
- state/status
- effect data
- changed body id if the instance transformed exclusively
- control/death/perception/persistence policy changes

The identity saves:

- identity-level fields
- form ownership and source mappings
- character-scoped skills and knowledge
- long-term state
- compatibility mirror fields for primary instance

### 7.4 Logout and Reboot

Persistence policies should define behaviour:

```text
Persistent:
  save and reload normally.

TemporaryEffectBound:
  save only if the effect persists and can restore/own the instance.

RecreateFromEffectOnLoad:
  do not persist full instance; effect recreates it.

DespawnOnLogout:
  retire/despawn when the owning identity logs out.

DespawnOnReboot:
  do not reload after server restart.
```

Astral projections probably start as `TemporaryEffectBound` or `DespawnOnLogout`. Physical clones may be `Persistent` or `TemporaryEffectBound` depending game design.

---

## 8. Body Lifecycle

### 8.1 Existing Lifecycle

Current `Body` activation is based on whether the body is `Actor.CurrentBody` and the actor is neither dead nor in stasis. This must change for simultaneous bodies.

### 8.2 New Lifecycle

A body should know whether it is dormant, embodied, suspended, retired, or destroyed. Its physiology should run when:

```text
body is embodied by an active instance
and that instance permits body processes
and the identity/instance is not in a state that suppresses the body
```

Replace checks equivalent to:

```csharp
Actor.CurrentBody == this
```

with an embodied-instance check:

```csharp
EmbodiedBy?.Body == this && EmbodiedBy.CanRunBodyProcesses
```

### 8.3 Location and Output

`Body.Location` must no longer proxy through the identity-level actor. It should either:

1. proxy through the embodied instance, or
2. become unavailable for dormant bodies and require callers to use the instance for world location.

Recommended transitional implementation:

```csharp
public override ICell Location => EmbodiedBy?.Location;
public override IOutputHandler OutputHandler => EmbodiedBy?.OutputHandler ?? NonPlayerOutputHandler.Instance;
```

Then audit high-risk call sites and move them toward using the instance directly.

### 8.4 Spawn/Attach Flow

Add a new lifecycle operation distinct from body switching:

```csharp
bool TrySpawnBodyInstance(
    ICharacterForm form,
    CharacterInstanceSpawnOptions options,
    out ICharacterInstance instance,
    out string whyNot);
```

Expected flow:

1. Validate form exists and belongs to identity.
2. Validate body is not already embodied.
3. Validate location/layer/policy.
4. Create instance runtime object and database row if persistent.
5. Attach body to instance.
6. Initialise instance state/position.
7. Insert instance into the target cell.
8. Activate body processes as policy permits.
9. Register controller or AI if policy permits.
10. Fire spawn/projection events and echoes.

### 8.5 Retire/Detach Flow

Add:

```csharp
bool TryRetireInstance(ICharacterInstance instance, CharacterInstanceRetireReason reason, out string whyNot);
```

Expected flow:

1. Cancel movement/action effects.
2. Leave combat as the instance.
3. Detach controller focus or redirect focus.
4. Remove from location.
5. Stop instance effects and heartbeats.
6. Stop or suspend body processes.
7. Set body to dormant, retired, dead, or destroyed based on policy.
8. Create non-final remains if appropriate.
9. Clean up source mappings only if policy says the form is consumed.
10. Save identity and instance/body changes.

---

## 9. Switching vs Spawning

The existing body switch pipeline is still required. It handles exclusive transformation and trauma transfer/stash between bodies. It should remain the implementation for:

- `form <alias>`
- forced transformations
- werewolf-style changes
- spells that transform the current body
- death backup transfer if no simultaneous instance exists

The new spawn pipeline should be used for:

- astral projection
- magical copies
- physical clones
- remote shell activation
- puppets
- any effect where the old body stays in the world as an active or vulnerable body

Do not make `SwitchToBody` create extra bodies. Do not make `TryPrepareSwitchFrom` accept active bodies just to support projections. Its dormant-shell safety checks are valuable and should be preserved.

---

## 10. Controllers and Player Experience

### 10.1 Control Focus

A PC controller should remain associated with the identity/account, but its command focus should point to one active instance at a time.

The existing `ICharacterController.UpdateControlFocus(ICharacter newFocus)` is a useful entry point. Future implementation should use it for instance focus changes.

Recommended commands:

```text
instances
  Lists active instances/presences for this identity.

focus <instance>
  Changes command focus to a controllable instance.

focus primary
  Returns focus to the primary instance.

instance <instance> look
  Optional remote-inspection command if policy allows it.
```

### 10.2 Output Routing

Do not merge all body output by default. It will be confusing and may leak information.

Recommended initial policy:

```text
Focused instance:
  normal command output and sensory output.

Unfocused instances:
  silent unless a critical event occurs, such as damage, death, attack, forced movement,
  projection collapse, or policy-specific warning.
```

Later policies may support tagged feeds:

```text
[Astral] You see ...
[Physical] Someone shakes your body awake.
[Clone 2] You are struck by ...
```

### 10.3 Action Economy

For PCs, one controller should not be able to freely act through multiple bodies in the same normal command flow unless a specific game design opts into it.

Initial rule:

```text
Only the focused instance receives normal commands.
Other player-owned instances are passive, AI-controlled, or script-controlled according to policy.
```

This avoids major balance problems with clones, combat, crafting, movement, and social interaction.

### 10.4 NPC Control

NPCs need first-class support.

Recommended first implementation:

- each active NPC instance can have its own NPC controller/AI heartbeat if `ControlPolicy = NpcAiControlled`
- identity-level NPC state is shared
- instance-level AI state is not shared unless explicitly configured

Later enhancement:

- add an identity-level coordinator AI that can command multiple instances as a group

---

## 11. Movement and Location Semantics

### 11.1 Transitional Rule

Keep `ILocation.Characters` and `Enter(ICharacter)`/`Leave(ICharacter)` initially. Make active instances implement `ICharacter`. This keeps the location, room echo, movement, event, and perception stack largely intact.

Longer-term names could be improved to `CharacterInstances`, but renaming now would touch too much code.

### 11.2 Movement Should Be Instance-Level

All immediate movement state belongs to the instance:

- active movement object
- queued move commands
- dragging/dragged effects
- following/party movement participation
- mount/rider relationship
- vehicle occupancy where appropriate
- falls, swimming, flying, climbing

A projection moving east should not move the physical body. A clone being dragged should not drag every other instance of the identity.

### 11.3 Movement Policy Flags

Each instance kind should define movement capabilities:

```text
CanUseOrdinaryExits
CanUsePlanarExits
CanOpenDoors
CanBeBlockedByGuards
CanBeDragged
CanDragOthers
CanFollow
CanLeadParty
CanRideMount
CanHaveRiders
CanFall
CanSwim
CanFly
CanUseVehicles
MovementAffectsAnchor
```

Astral projection may ignore some physical barriers but obey planar barriers. Physical clones use normal movement. Mirror images may be unable to leave a room.

---

## 12. Perception, Identity, and Selfhood

### 12.1 Required Distinctions

The implementation must distinguish:

```text
same physical instance
same body
same durable identity
same controller focus
known to be same identity
```

These are not interchangeable.

### 12.2 `IsSelf` Semantics

Current `IsSelf` semantics conflate a character with its current body. With multiple bodies, `IsSelf` should generally mean same physical instance for ordinary targeting and emotes.

Add explicit helpers:

```csharp
bool SamePhysicalInstance(IPerceivable other);
bool SameIdentity(ICharacter other);
bool IsControlledBySameController(ICharacter other);
bool IsKnownSameIdentityTo(ICharacter observer, ICharacter other);
```

Use cases:

```text
look me:
  same physical/control-focus instance

attack self:
  same physical instance

recognise this clone as the same person:
  identity/perception check

social permissions / allies:
  identity-scoped unless content says otherwise

admin diagnostics:
  show both identity id and instance id
```

### 12.3 Keywords and Descriptions

Two active instances may have identical descriptions. Targeting should support disambiguation:

```text
2.man
clone 2
astral
body #12345
instance #987
```

Form aliases and instance aliases should be separate. A form alias identifies an owned body form; an instance alias identifies an active presence.

### 12.4 Dubs and Recognition

Dubs currently refer to perceivable ids/types. With instances, a dub may apply to:

- a specific instance
- the durable identity
- a body/form appearance

Initial implementation can keep dubs as perceivable-specific. Later work can add identity-linked dubs once recognition mechanics are clarified.

---

## 13. Combat and Targeting

### 13.1 Combat Is Instance-Level

Combat state belongs to the active instance, not the identity:

- combat object
- combat target
- aim
- targetted bodypart
- defensive/offensive advantage
- melee engagement
- cover
- selected combat action

The target's body supplies bodyparts, wounds, armour, and anatomy.

### 13.2 Same-Identity Combat

The combat system must not assume same identity means same combatant. Depending game rules, a character may be able to attack their own clone, body, astral form, or possessed shell.

Rules should be policy-driven:

```text
CanTargetSameIdentity
CanDamageAnchor
CanFriendlyFireOwnInstances
CanGrappleProjection
CanOrdinaryWeaponsHitThisInstance
CanPlanarWeaponsHitThisInstance
```

### 13.3 Targeted Bodyparts

`TargettedBodypart` must always refer to the target instance's current body. If an instance transforms exclusively or a target body is retired, existing targeted bodyparts must be cleared or remapped.

The existing body switch post-process already clears invalid targeted bodyparts for combatants targeting the character. Equivalent logic must operate on instances.

---

## 14. Health, Death, and Remains

### 14.1 Split Death Concepts

The engine needs separate concepts for:

```text
Body death
Instance death
Identity death
```

A body can die without final identity death. An instance can collapse without corpse creation. The identity dies only when the relevant policy says so.

### 14.2 Instance Death Policies

Suggested behaviours:

```text
FinalCharacterDeath:
  current ordinary primary body behaviour.

DestroyInstanceOnly:
  destroy this instance; identity survives.

DestroyInstanceAndDamageAnchor:
  destroy this instance; apply backlash to anchor/primary body.

CollapseToAnchor:
  projection or copy collapses; focus returns to anchor.

TransferControlToAnchor:
  instance dies; controller moves to anchor body.

TransferControlToBackup:
  instance dies; body backup or prepared vessel becomes focused/primary.

KillIdentityIfNoPrimaryCapableInstance:
  identity survives while at least one qualifying instance remains.
```

### 14.3 Corpses and Non-Final Remains

The existing remains model already distinguishes final character death from body-specific remains. Continue using body-specific remains for non-final body deaths. `OriginalBody` should remain authoritative for anatomy, wounds, organs, implants, inventory, and corpse descriptions. `OriginalCharacter` remains provenance.

New remains contexts may be useful:

```text
ProjectionCollapse
MirrorImageDissipation
CloneDeath
RemoteShellAbandoned
PossessedBodyDeath
PuppetDestroyed
```

If adding contexts is too disruptive, use `Other` with metadata until the content surface stabilises.

### 14.4 Death Backup Interaction

Current death backup transfer changes the active body of the character and retires the old form. Under the instance model, it should become:

```text
old instance dies or retires
backup body becomes embodied by new or existing instance
identity primary/focus updates according to policy
old body remains are created according to remains context
```

Do not assume there is only one active instance when selecting or consuming backups. Backups may apply to:

- identity globally
- a specific instance
- a specific body
- only primary instance death
- only physical body death

---

## 15. Inventory and Physical Manipulation

### 15.1 Inventory Is Body-Local

Inventory should remain body-local. This is one of the existing architecture's strengths.

A clone should not automatically share held items with the primary body. An astral projection should not carry physical inventory unless explicitly designed to do so.

### 15.2 Manipulation Policies

Add instance interaction policies:

```text
CanManipulatePhysicalItems
CanManipulatePlanarItems
CanUseAnchorInventory
CanUseIdentityCurrency
CanUseHeldComponents
CanBeSearched
CanBeRestrained
CanBeOperatedOn
CanBeDragged
CanPickUpItems
CanWearItems
CanWieldItems
```

Default examples:

```text
Primary physical body:
  ordinary manipulation.

Astral projection:
  cannot manipulate ordinary physical items; can manipulate astral items if supported.

Mirror image:
  cannot manipulate inventory.

Physical clone:
  ordinary manipulation with its own inventory.
```

### 15.3 Avoiding Item Duplication

The spawn pipeline must never copy inventory by default. A newly spawned instance should either:

- use a clean dormant form body
- use a body with preconfigured inventory
- receive explicitly created items from the spawning effect
- share no items with the anchor unless a special item-sharing mechanic exists

The existing switch pipeline moves or drops items because it replaces the active body. Simultaneous spawn should not do that.

---

## 16. Effects, Heartbeats, and Schedulers

### 16.1 Effect Scope Taxonomy

Every effect should eventually declare scope:

```text
Identity
Instance
Body
Item
Room/Area/Zone
```

Initial migration can use conventions and explicit marker interfaces.

### 16.2 Identity Effects

Examples:

- long-term curses
- global merits
- identity-level magic resource regeneration
- account/social/legal states
- knowledge or skill modifiers
- effects that provision forms

### 16.3 Instance Effects

Examples:

- movement/action effects
- hiding/invisibility if tied to presence
- guarding
- dragging/grappling
- aiming
- combat effects
- projection tether
- focus lock
- remote perception feed
- temporary command restrictions

### 16.4 Body Effects

Examples:

- wounds
- bleeding
- infections
- drugs
- breathing and suffocation
- stamina/exertion where body-local
- prosthetics and implants
- restraints
- treatments
- bodypart effects

### 16.5 Heartbeat Multiplication Risks

The most dangerous bug class is multiplying identity-level heartbeats once per instance. Examples:

- magic resource regeneration
- needs if identity-wide
- forced transformation reevaluation
- NPC AI if intended identity-wide
- project labour
- periodic progs on character effects

Each heartbeat registration should be audited and assigned to identity, instance, or body.

---

## 17. Planar and Astral Projection Semantics

Astral projection is the recommended first production feature after passive multi-instance support because it avoids many physical inventory and action-economy issues.

### 17.1 Suggested Astral Projection Behaviour

Physical body:

```text
stays in original room
is visible and vulnerable unless content says otherwise
cannot act while projection is active
may continue breathing/bleeding/needs, or may enter stasis based on spell config
receives critical danger notifications to the player if policy allows
```

Astral body:

```text
spawned as active instance from astral form body
focus moves to astral instance
uses astral planar presence
cannot manipulate ordinary physical objects
cannot be hit by ordinary physical attacks
can interact with astral/planar entities according to planar rules
collapse/death returns focus to physical body and applies configured backlash
```

### 17.2 Projection Tether

Astral projection likely needs a tether effect that records:

- identity id
- anchor instance id
- projection instance id
- max distance or plane rules
- collapse conditions
- backlash rules
- output policy
- whether physical body physiology continues
- whether projection persists through logout/reboot

### 17.3 Physical Body Death While Projecting

This must be explicitly policy-driven. Possible behaviours:

```text
identity dies and projection collapses
projection becomes ghost-like surviving identity
projection transfers into backup body
projection becomes stranded and cannot return
projection becomes the new primary instance
```

Do not hard-code one behaviour into the instance framework.

---

## 18. Builder, Admin, and Prog Surface

### 18.1 Admin Commands

Suggested staff commands:

```text
body instance list <character>
body instance spawn <character> <form> [location]
body instance retire <character> <instance>
body instance focus <character> <instance>
body instance set <character> <instance> kind <kind>
body instance set <character> <instance> control <policy>
body instance set <character> <instance> death <policy>
body instance set <character> <instance> perception <policy>
body instance set <character> <instance> persistence <policy>
body instance set <character> <instance> anchor <instance>
```

Existing `body addform`, `body formset`, and `body switch` remain form/switch commands.

### 18.2 Player Commands

Initial player commands:

```text
instances
focus <instance>
focus primary
```

The existing `form` command remains an exclusive transformation command, not a projection command.

### 18.3 FutureProg Functions

Suggested progs/functions:

```text
EnsureForm(character, specification, source)
SpawnBodyInstance(character, form, location, layer, options)
DespawnBodyInstance(instance, reason)
GetInstances(character)
GetPrimaryInstance(character)
GetFocusedInstance(character)
SetControlFocus(character, instance)
HasActiveInstance(character, formOrKind)
GetInstanceBody(instance)
GetInstanceIdentity(instance)
SameIdentity(characterA, characterB)
SameInstance(characterA, characterB)
```

### 18.4 Magic Effects

Add spell effects after the core instance system exists:

```text
projectbody
  Creates an astral/remote projection instance.

createcopy
  Creates a magical copy/mirror image instance.

createclone
  Creates a physical clone instance.

bindshell
  Prepares a remote shell instance.

possessbody
  Transfers control/focus to another instance/body under policy.
```

---

## 19. Implementation Phases

### Phase 0: Audit and Safety Rails

Deliverables:

- add design doc
- add diagnostic helpers for current assumptions
- identify direct uses of `Character.Body`, `CurrentBody`, `Location`, `RoomLayer`, `State`, `Die`, `Quit`, and `LoginCharacter`
- classify systems as identity, instance, or body scoped

Implementation progress:

- Completed: the design document now anchors the simultaneous-instance rollout.
- Completed: `CharacterInstanceStateScope` classifies state as identity, instance, body, compatibility mirror, or unknown.
- Completed: `CharacterInstanceDiagnostics.AuditPrimaryInstance(...)` verifies the current compatibility assumptions that `Body`, `CurrentBody`, body actor, body location, body room layer, owned bodies, and form metadata all agree for the primary instance.
- Confirmed current seams: `Character.Save()` mirrors active body/location/layer/state/status into `Characters`, `Character.LoadFromDatabase(...)` restores those fields from `Characters`, `CharacterForms.SwitchToBody(...)` is the exclusive transformation point, `Body.Location` and `Body.RoomLayer` currently proxy through the actor, and `Die()`, `Quit()`, and `LoginCharacter()` operate on the active `Character` world presence.

Next-phase reflection:

Phase 1 should preserve those audited assumptions while adding `CharacterInstances` persistence. The immediate goal is not to spawn a second presence; it is to create a durable primary-instance row that mirrors the current character row and can be audited against the old compatibility fields.

Useful grep/search terms:

```text
CurrentBody
.Body
Actor.Location
Actor.OutputHandler
Character.Die
Body.Die
Location.Enter(ICharacter
Location.Leave(ICharacter
Characters.BodyId
PositionTarget
CombatTarget
IsSelf
CanRunCharacterOngoingProcesses
```

### Phase 1: Persistence Skeleton and Compatibility Primary Instance

Deliverables:

- create `CharacterInstances` table/model
- add runtime instance object or internal representation
- migration creates one primary instance for every existing character
- compatibility fields mirror primary instance
- no behavioural changes for existing characters

Implementation progress:

- Completed: added the EF `CharacterInstance` model, `CharacterInstances` DbSet, navigation properties from `Character`, `Body`, and `Cell`, and explicit model configuration.
- Completed: generated migration `20260612134150_CharacterInstances` with the table, indexes, foreign keys, and a deterministic backfill that creates one primary persistent instance from each existing `Characters` row.
- Completed: `Character` now loads primary-instance rows when present, falls back to legacy `Characters` fields when absent, inserts a primary instance for new characters, and mirrors body/location/layer/position/state/status into the primary instance on save and final death persistence.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` succeeded after the phase-1 wiring.

Next-phase reflection:

Phase 2 should make the identity/instance split visible to callers without changing command semantics. `ICharacter` remains the active world actor facade, while new identity and instance contracts expose `Identity`, `InstanceId`, policy fields, and explicit same-identity versus same-physical-instance comparisons.

Acceptance tests:

- existing characters load and save
- existing NPCs spawn and act
- existing PCs login/logout
- existing form switching works
- existing death and resurrection flows work

### Phase 2: Identity/Instance Split in Interfaces

Deliverables:

- introduce identity and instance interfaces
- add `Identity`, `InstanceId`, `SameIdentity`, and `SamePhysicalInstance`
- make existing `Character` act as primary instance for compatibility
- move or delegate identity-scoped accessors deliberately

Implementation progress:

- Completed: introduced `ICharacterIdentity`, `ICharacterInstance`, `CharacterInstanceKind`, control/death/perception/persistence policy enums, and `BodyEmbodimentState`.
- Completed: added instance compatibility members directly to `ICharacter`, preserving existing `ICharacter` call sites while making active world actors explicitly instance-aware.
- Completed: `Character` now implements `ICharacterIdentity` and `ICharacterInstance`; for the compatibility milestone it exposes itself as `Identity`, `PrimaryInstance`, and `FocusedInstance`, with `Instances` containing the primary instance only.
- Completed: added `CharacterInstanceIdentityComparer` and delegated `Character.SameIdentity(...)` / `SamePhysicalInstance(...)` to it so same-identity and same-physical-presence semantics are explicit and testable.
- Verified: `dotnet test 'FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstanceIdentityComparerTests` passed 4 tests.

Next-phase reflection:

Phase 3 should keep the old world-facing `Character` object as the primary instance while treating its location, room layer, position, state, status, and current body as the persisted primary-instance mirror. The important hardening work is to make load/save and lifecycle paths consistently sync through that bridge, and to document that body location still resolves through the embodied instance actor until Phase 4 introduces a second actor.

Acceptance tests:

- no command output change
- `SameIdentity` works for the primary instance
- old `ICharacter` call sites compile and behave

### Phase 3: Move World Presence State to Instance

Deliverables:

- instance owns location/layer/position
- instance owns movement/combat immediate state
- body resolves location/output through embodied instance
- primary instance still mirrors old character fields
- login/quit/save use instance state

Implementation progress:

- Completed: the primary `Character` object is now the compatibility `ICharacterInstance`; its body, location, room layer, position, state, status, movement, combat, aim, and targeting state remain on the active world actor rather than on a separate durable identity object.
- Completed: load uses `CharacterInstances` primary-instance rows as the source for body/location/layer/position/state/status when present, falling back to legacy `Characters` fields for pre-migration data.
- Completed: save centralises legacy world-presence fields through `SaveCompatibilityWorldPresence(...)` and then mirrors the same state into the primary instance row.
- Completed: logout now persists the legacy compatibility fields and primary-instance mirror at the same point it already writes logout metadata, and final death persistence updates both the old character row and primary instance row.
- Completed: `Body` exposes `EmbodiedInstance` and resolves location, room layer, movement, event forwarding, and location-change events through the embodied actor/instance bridge instead of assuming the durable character identity is the physical world actor.
- Verified: focused identity comparer tests, primary-instance diagnostic tests, and `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` all passed.

Next-phase reflection:

Phase 4 is the first behavioural expansion. It should add a staff-only passive second-instance spawn/retire path from an owned dormant form, enforce one live embodied instance per body, place the passive instance into a room as a visible/targetable actor, and prove that moving or killing the passive non-final instance does not move or final-kill the primary identity.

Acceptance tests:

- movement, teleport, fall, swimming/flying/climbing still work
- combat targeting and bodypart targeting still work
- body location no longer depends on identity-level actor

### Phase 4: Passive Second Instance

Deliverables:

- staff command to spawn a second instance from an owned form
- second instance enters a room and is visible/lookable/targetable
- primary instance remains in its room
- second instance can be retired/despawned
- no PC focus yet

Implementation progress:

- Completed: added `PassiveCharacterInstance`, a non-controllable secondary `ICharacterInstance` that delegates identity to the owning primary `Character` while owning its own instance id, body, location, room layer, position, state, status, and policy fields.
- Completed: added `CharacterInstanceService` as the lifecycle owner for passive spawn, move, retire, save, load, and passive death handling, keeping command parsing separate from lifecycle state changes.
- Completed: persistent non-primary embodied rows now load as cell-local passive actors; `DespawnOnReboot` rows are pruned during character load.
- Completed: passive instances are inserted only into cells and are not added to `Gameworld.Characters`, `Gameworld.NPCs`, or `Gameworld.Actors`, avoiding duplicate identity ids in the global character caches.
- Completed: passive spawn uses owned non-current forms only, rejects bodies that already have live embodied instances, defaults to `NotControllable`, `DestroyInstanceOnly`, `OrdinaryEmbodied`, and `DespawnOnReboot`, and supports a staff-selected persistent policy.
- Completed: passive move and retire operations clear movement/combat/position targets, remove the passive actor from its cell, free the form body for later use, and persist or delete the row according to policy.
- Completed: passive death creates body-level remains with a non-final remains context, marks only the passive instance dead/retired, and leaves the primary identity and primary instance alive.
- Completed: `instance list`, `instance spawn`, `instance move`, and `instance retire` are available as staff-only storyteller commands for acceptance testing and administrative control.
- Completed: the Phase 1-3 review fixes were folded into this phase: resurrection now mirrors both `Characters` compatibility fields and the primary `CharacterInstances` row, diagnostics report duplicate primary rows and duplicate live embodied-body rows, and MySQL uniqueness uses generated nullable guard columns for primary identity and embodied body uniqueness.
- Verified: `dotnet test 'FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance` passed 4 tests.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance` passed 4 tests.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `dotnet build MudsharpDatabaseLibrary\MudsharpDatabaseLibrary.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `git diff --check` passed; it reported only existing line-ending normalization warnings.

Next-phase reflection:

Phase 5 should introduce player-facing focus switching only after the passive actor model has had staff acceptance testing. The main design goal is to move controller focus from the durable identity/primary object to a selected controllable instance without letting ordinary commands accidentally manipulate an unfocused body. The next phase should therefore concentrate on `instances`/`focus` discovery, command routing through `FocusedInstance`, prompt/output policy for focused and unfocused presences, and guardrails that keep primary-body death, stasis, and login/logout semantics coherent while a player is focused elsewhere.

Acceptance tests:

- two instances of same identity can be in different rooms
- moving one does not move the other
- looking at each uses the correct body description
- killing passive non-final instance does not final-kill identity
- saving/reloading preserves or despawns according to policy

### Phase 5: PC Focus Switching

Deliverables:

- `instances` and `focus` commands
- controller focus points to selected instance
- output policy implemented for focused/unfocused instances
- prompt indicates focused instance where useful
- action effects and movement commands apply to focused instance only

Implementation progress:

- Completed: added `CharacterInstanceFocusService` as the runtime focus coordinator. It validates same-identity, loaded, embodied, controllable, non-dead, non-stasis `PlayerFocusable` targets and switches command routing through the existing controller `SetContext(...)` path.
- Completed: primary `Character` identities now track a runtime-only focused secondary instance; focus resets to the primary on login, primary quit, linkdead detach, secondary retire, and focused-secondary death.
- Completed: staff `instance spawn` keeps passive/non-controllable as the default and adds an explicit `focusable` option for Phase 5 acceptance testing before projection mechanics exist.
- Completed: focusable secondary instances remain cell-local and outside global character/NPC caches, but mirror the owning player's permission level and command tree while controlled.
- Completed: player-facing `instances` and `focus` commands list own instances, switch to numbered focus targets, and return to `focus primary` without exposing staff-only cell ids.
- Completed: focused instances receive the player output handler and normal command/sensory output; unfocused instances use `NonPlayerOutputHandler` by default, with only explicit lifecycle focus-return messages emitted in Phase 5.
- Completed: non-primary prompt output now includes a compact focus marker in full, classic, brief, and full-brief prompt modes.
- Completed: `quit` while focused on a secondary validates and logs out through the primary identity instead of retiring the secondary, preserving account/logout persistence semantics.
- Verified: `dotnet test 'FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance` passed 4 tests.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance` passed 12 tests.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `dotnet build MudsharpDatabaseLibrary\MudsharpDatabaseLibrary.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `git diff --check` passed; it reported only existing line-ending normalization warnings.

Next-phase reflection:

Phase 6 should extend the now-proven cell-local secondary actor model to NPCs without reusing PC focus semantics. The important work is to give each AI-controlled NPC instance its own controller/heartbeat surface, audit identity-level effects so shared regeneration or periodic state does not multiply per instance, and keep staff clone/projection spawning tied to explicit instance policies rather than hidden global actor-cache assumptions.

Acceptance tests:

- player can focus projection and move it independently
- player can return focus to primary body
- physical body receives critical notifications only if configured
- commands do not accidentally manipulate the unfocused body

### Phase 6: NPC Multi-Instance Support

Deliverables:

- NPC instances can be AI-controlled
- AI heartbeat is instance-safe
- identity-level regeneration/effects are not multiplied
- staff can spawn NPC clones/projections

Implementation progress:

- Completed: added `NpcCharacterInstance`, a secondary `Character` actor that implements `INPC`, delegates durable identity/template ownership to the primary NPC, and owns its own instance id, body, location, room layer, position, state, status, controller, and AI heartbeat subscriptions.
- Completed: `Character.MaterialiseSecondaryInstance(...)` now materialises persisted non-primary `NpcAiControlled` rows as NPC secondary actors, while passive and player-focusable rows continue to use the existing secondary actor path.
- Completed: staff `instance spawn` now uses a generalized `SpawnSecondaryInstance` options flow with `passive`, `focusable`, and `ai`/`npcai` modes. AI-controlled spawns are restricted to NPC identities, and player-focusable spawns remain PC-only.
- Completed: NPC secondaries remain cell-local and are not added to `Gameworld.NPCs`, `Gameworld.Actors`, or cached actor collections; staff lookup discovers them through owner instances and local cell membership.
- Completed: each NPC secondary receives its own `NPCController`, `NonPlayerOutputHandler`, and copied runtime AI list. The owner NPC refreshes already-materialised secondary AI lists after its template/AIs finish loading so persisted secondaries do not retain empty load-order copies.
- Completed: retire/death cleanup now handles all non-primary secondary actors, releases NPC AI heartbeat subscriptions, detaches the secondary controller, leaves combat and movement, removes the actor from its cell, frees the body, and persists or deletes the row according to policy.
- Completed: primary identity saves now flush loaded secondary instance rows, preserving persistent secondary location/state even though secondary NPCs deliberately stay out of the global actor save list.
- Completed: `instance list` marks AI-controlled instances with AI count and controller status for staff acceptance testing.
- Verified: `dotnet test 'FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance` passed 4 tests.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter "CharacterInstance|NPCAIEventSubscription"` passed 20 tests.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `dotnet build MudsharpDatabaseLibrary\MudsharpDatabaseLibrary.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.

Next-phase reflection:

Phase 7 should turn the staff-only instance architecture into a concrete astral projection feature. The important design work is to define the projection source effect, anchor/body vulnerability rules, astral form provisioning, planar interaction restrictions, tether/collapse behavior, and backlash/death policies without reopening the global actor-cache constraints that Phases 4-6 deliberately avoided. Builder-facing configuration should decide whether the physical body is helpless, asleep, stasis-locked, or still vulnerable while the projection is active.

Acceptance tests:

- two NPC instances can fight independently
- killing one NPC instance follows its death policy
- NPC identity state remains coherent

### Phase 7: Astral Projection Feature

Deliverables:

- projection spell/effect
- astral form provisioning
- anchor/projection tether
- planar interaction rules
- collapse/death/backlash policy
- builder settings for physical body vulnerability/stasis

Acceptance tests:

- physical body remains in room
- projection moves independently
- ordinary physical item manipulation is blocked
- projection death returns or resolves according to policy
- physical body death while projecting follows configured policy

### Phase 8: Magical Copies and Physical Clones

Deliverables:

- copy/clone spell effects
- clone inventory policy
- clone AI/control policy
- clone death/remains policy
- recognition/identity rules for observers

Acceptance tests:

- physical clone can fight and hold items if configured
- magical copy can be intangible/temporary if configured
- death/remains are body-specific and not always final identity death
- same-identity targeting works correctly

### Phase 9: Hardening and Cleanup

Deliverables:

- migrate more identity-scoped systems away from instance facade
- rename or clarify APIs where feasible
- add admin diagnostics
- add data integrity reports
- remove compatibility mirrors only if safe and desirable

---

## 20. Legacy Migration Considerations

### 20.1 Existing Characters

All existing characters should receive exactly one primary instance. The migration should be idempotent:

```text
If character has no instance rows, create primary instance.
If character already has a primary instance, do nothing.
If character has multiple primary instances, flag diagnostic and select one deterministically.
```

### 20.2 Existing Dormant Forms

Existing dormant forms remain dormant. Do not create instances for every form. Only the current active body becomes embodied by the primary instance.

### 20.3 Existing Form Source Mappings

`CharacterBodySources` remains unchanged. It maps content sources to owned form bodies, not active instances.

### 20.4 Existing Body Backups

Existing body backup effects should continue to work in single-body mode. During the transition, they can operate on the primary instance. Later they should gain instance scope.

Migration rule:

```text
Old backup effect without instance metadata applies to primary instance death.
```

### 20.5 Existing Corpses and Remains

Existing corpse and remains data should not need migration if it already stores body provenance. However, any logic that resolves corpse back to `OriginalCharacter.CurrentBody` must be audited. Body-specific operations should use the corpse's `OriginalBody`.

### 20.6 Existing Effects

Effects with no explicit scope should remain where they currently load. Add load-time warnings for effects that are known to be ambiguous.

Suggested initial classification:

```text
Character effect XML -> identity or primary instance depending effect type.
Body effect XML -> body.
Item effect XML -> item.
```

### 20.7 Existing Progs

Existing progs that accept `Character` should continue to receive the current/focused instance. Add new functions for identity access rather than changing old semantics immediately.

Potential new prog helpers:

```text
IdentityOf(character)
PrimaryInstance(character)
FocusedInstance(character)
InstancesOf(character)
SameIdentity(a, b)
SameInstance(a, b)
```

### 20.8 Existing Commands

Most existing commands should operate on the focused instance. Commands that are identity-scoped should be explicitly migrated:

```text
score: probably focused instance plus identity data
skills: identity data
health: focused body health
inventory: focused body inventory
who/account/admin: identity data
form: focused instance exclusive switch, or identity-level form list with clear semantics
```

### 20.9 Existing Admin Scripts and Builder Workflows

Add admin display of both identity id and instance id to reduce confusion:

```text
Character #123 / Instance #456 / Body #789
```

For a period, admin commands that take a character should default to the focused or primary instance, with explicit syntax for instance targeting where needed.

### 20.10 Rollback

A safe rollback strategy should be possible while only primary instances exist:

- leave `CharacterInstances` table unused
- continue using `Characters.BodyId`, `Location`, and `RoomLayer`
- ignore non-primary instance rows if none have been created in production

Once simultaneous instances are live in production, rollback becomes content-destructive unless non-primary instances are retired first.

---

## 21. Gotchas and Edge Cases

### 21.1 `Body.Location` Proxying Through Actor

This is the largest technical blocker. If any active second body still returns `Actor.Location`, simultaneous locations will fail. Fix this before spawning a second active body.

### 21.2 `IsSelf` and Equality

Same identity is not same physical actor. Bugs here can cause:

- attacks against clones treated as self-attacks
- emotes rendering the wrong body as “you”
- targeting commands selecting the focused body instead of the visible body
- dubs and recognition incorrectly merging bodies
- combat target bodyparts cleared or retained incorrectly

### 21.3 Output Handler Leakage

If all bodies share one output handler without policy, players may receive sensory output from every clone/projection. This can leak information and create noise. Use explicit output routing.

### 21.4 Heartbeat Multiplication

Identity-level heartbeats must not run once per instance. Audit magic regeneration, needs, forced transformations, project labour, and AI subscriptions.

### 21.5 Final Death vs Instance Death

Do not call `Character.Die()` for every body death. Introduce `Instance.Die()` or equivalent. Only final identity death should execute final-death side effects such as death board posts, estate creation, account menu routing, and gameworld destruction of the identity.

### 21.6 Controller Focus and Logout

If a player logs out while focused on a projection:

- does focus return to primary first?
- does the projection persist?
- does the physical body remain vulnerable?
- does the projection collapse?
- what is saved?

This must be dictated by persistence/control policy.

### 21.7 Physical Body Death While Focused Elsewhere

The physical body may die while the player is controlling an astral body. This cannot silently use ordinary final death without considering projection policy.

### 21.8 Same Room Duplicate Descriptions

Two clones with identical short descriptions in one room need disambiguation. Targeting must handle ordinal targeting and instance aliases.

### 21.9 Form Alias vs Instance Alias

A form alias is not the same as an active instance alias. A character might have one “wolf” form but multiple wolf instances only if clone rules permit it. Avoid alias collisions by separating concepts.

### 21.10 One Body Embodied Twice

The spawn pipeline must reject embodying a body that is already active. This is a hard invariant.

### 21.11 Inventory Duplication

Never clone inventory implicitly. If copies should have duplicate items, those items must be newly created by content and clearly owned by the new body.

### 21.12 Lodged Items, Implants, Prosthetics

Existing body switch code carefully handles implants, prosthetics, lodged items, wounds, and scars. Simultaneous spawn should not transfer these unless explicitly designed. Active clones with implants need independent item ownership.

### 21.13 Restraints and Grapples

If the physical body is bound, can the astral body project? If the astral body is grappled, does the physical body suffer? These should be effect-specific policies, not global assumptions.

### 21.14 Mounts, Riders, Vehicles, and Dragging

Movement code currently handles mounts, riders, dragging, and parties for one actor. Instance movement must not accidentally move all same-identity instances.

### 21.15 Crimes and Legal Attribution

Crimes should be committed by an instance, attributed to an identity only if known or legally inferred. A clone may commit a crime while the primary body is elsewhere. Witnesses may or may not know they are the same person.

### 21.16 Healing and Offline Healing

Offline healing currently applies to the current body on login. With multiple persistent bodies, each active/persistent body needs an offline healing policy.

### 21.17 Forced Transformations

Forced transformation demands currently target the character's active body. Future semantics:

- apply to focused instance only?
- apply to primary instance only?
- apply to all instances?
- apply to bodies matching a filter?

This must be explicit per demand/effect.

### 21.18 Race/Gender/Ethnicity Properties

`Character.Race`, `Gender`, and `Ethnicity` currently route through current body. With instances, these are instance/body properties. Identity may have separate stable values or no single current value when multiple bodies exist.

### 21.19 Body Cleanup

Retired body cleanup must account for active instances, remains, severed parts, body backups, and source mappings. Do not delete a body referenced by any live or persisted instance.

### 21.20 Admin Tools and Diagnostics

Admin commands must show identity, instance, and body ids clearly. Otherwise staff will mutate the wrong presence.

---

## 22. Testing Strategy

### 22.1 Invariant Tests

Automated checks:

```text
one live primary instance per live identity
no live body embodied by more than one instance
dormant bodies have no live instance
instance body belongs to identity forms/bodies
primary compatibility fields match primary instance
no duplicate location membership for same instance
```

### 22.2 Regression Tests

Before enabling simultaneous bodies:

```text
login/logout
movement
teleport
combat
death
resurrection
form switching
forced transformation
body backup death transfer
corpse/remains operations
inventory wear/wield/hold
medical/surgery
NPC AI heartbeat
magic resource regeneration
needs heartbeat
```

### 22.3 Multi-Instance Scenario Tests

Passive second instance:

```text
spawn second instance in same room
spawn second instance in different room
look at both bodies
move primary only
move secondary only via admin
kill secondary with non-final death policy
save/reload with persistent policy
retire temporary instance
```

Astral projection:

```text
project while standing
project while bound
project while in combat
physical body attacked while projected
projection attacked by planar entity
projection collapses at expiry
projection dies
physical body dies while projected
logout while projected
```

Physical clone:

```text
clone holds item
clone fights
clone dies and leaves body-specific remains
clone commits crime
observer sees two identical bodies
same-identity targeting disambiguation works
```

### 22.4 Load/Save Tests

```text
server reboot with one primary instance
server reboot with temporary projection
server reboot with persistent clone
missing body row
missing location row
stale instance row referencing consumed form
old character with no CharacterInstances rows
old body backup effect with no instance metadata
```

---

## 23. Agent Implementation Guidance

When implementing this feature, every code change should ask:

```text
Is this state identity-scoped, instance-scoped, or body-scoped?
```

Do not patch around simultaneous bodies by adding special cases to the body switch pipeline. The correct abstraction is a new active instance lifecycle.

### 23.1 Preferred Approach

- Keep existing `ICharacter` compatibility for world actors.
- Add explicit identity access to `ICharacter`.
- Make multiple active bodies appear as multiple `ICharacter` instances in locations.
- Move location/output/body activation away from identity-level assumptions.
- Preserve existing form switching and body backup behaviour until the new instance model can replace or generalise it safely.

### 23.2 Avoid

- Making one `Character` object appear in two rooms.
- Making one `IBody` embody multiple instances.
- Treating same identity as same target.
- Running identity heartbeats per instance.
- Copying inventory by default.
- Calling final `Character.Die()` for projection or clone death.
- Breaking `form` command semantics by turning it into spawn semantics.

### 23.3 Safe First Milestone

The first meaningful milestone is:

```text
One identity can have two active loaded instances, both visible in the world,
one controllable/focused, one passive, with independent locations and body descriptions,
and with non-final retirement/death of the passive instance.
```

Once that works, astral projection can be built on top of it.

---

## 24. Open Design Questions

These should be resolved before content implementation, though the framework can support multiple answers through policy.

1. Can a PC ever command multiple instances in the same action cycle?
2. Are magic resource pools identity-wide, instance-wide, or configurable?
3. Do physical clones share identity-level cooldowns?
4. Can astral projections manipulate any physical items?
5. Does the physical body continue needs/breathing/bleeding while projected?
6. What happens to projection if physical body dies?
7. Can physical clones be independently arrested, searched, or legally identified?
8. Can an observer automatically recognise clones as the same identity?
9. How should disguises interact with identity recognition across instances?
10. Can forced transformations affect all instances at once?
11. Can a body backup be prepared for a non-primary instance?
12. What is the default logout behaviour for projections and clones?

---

## 25. Recommended Initial Feature: Astral Projection

After the passive multi-instance milestone, implement astral projection before physical clones.

Reasons:

- It exercises independent location and focus.
- It exercises anchor/projection death policy.
- It avoids ordinary inventory duplication.
- It can limit physical interaction through planar rules.
- It provides immediate gameplay value.
- It reveals the main architectural risks before full physical clones add combat/inventory/legal complexity.

Recommended first astral projection constraints:

```text
one projection per identity
projection is focused while active
physical body cannot act
projection cannot manipulate ordinary physical inventory
projection has non-final death/collapse policy
projection expires with effect
projection does not persist through logout initially
```

Once stable, expand to persistent projections, remote sensory feeds, projection combat, and configurable physical-body vulnerability.

---

## 26. Summary

The current body-form system is a strong foundation for exclusive transformations, but simultaneous bodies require a new world-presence abstraction. The central architectural move is to split durable identity from active character instances, while keeping `ICharacter` as the world actor interface during the transition.

The implementation should proceed incrementally:

1. Add primary instance persistence with no behaviour change.
2. Split identity/instance concepts in interfaces.
3. move world presence state to instances.
4. spawn passive second instances.
5. add PC focus switching.
6. support NPC multi-instance AI.
7. implement astral projection.
8. implement physical clones and more advanced content.

The most important technical risks are body location proxying, self/equality semantics, output routing, death policy, heartbeat multiplication, and legacy content that assumes `Character.Body` is the one physical body that matters. Addressing those deliberately will make the feature extensible rather than a one-off projection hack.
