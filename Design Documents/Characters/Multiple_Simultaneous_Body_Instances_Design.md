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
- Completed: generated migration `20260612134150_CharacterInstances` with the table, indexes, generated uniqueness guards, and a deterministic backfill that creates one primary persistent instance from each existing `Characters` row. The first-upgrade migration deliberately does not add hard `CharacterInstances` foreign keys because pre-branch `Characters.BodyId` and `Characters.Location` were compatibility mirrors; stale locations are imported as null and stale bodies are reported by the Phase 9 audit tooling rather than blocking boot.
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

Implementation progress:

- Completed: generalized secondary instance spawning with `SecondaryCharacterInstanceSpawnOptions`; passive, player-focusable, and NPC AI paths now wrap the shared options flow, and astral projection uses `InstanceKind=AstralProjection`, `ControlPolicy=PlayerFocusable`, `PerceptionPolicy=PlanarProjection`, `DeathPolicy=CollapseToAnchor`, and `PersistencePolicy=DespawnOnReboot`.
- Completed: added astral projection instance metadata in `CharacterInstances.EffectData`, exposed as `ICharacterInstance.InstanceEffectData`, with staff-readable anchor, body, plane, source spell, form key, and anchor policy details.
- Completed: added `astralprojection` spell effect template with keyed form provisioning options (`formkey`, `race`, `ethnicity`, `gender`, `alias`, `sort`), plane selection, anchor policy (`helpless`, `sleep`, `stasis`, `none`), private projection/anchor/collapse echoes, observer-facing anchor/projection room echoes, optional backlash echo, and an optional projection short-description override template using `$desc`/`$sdesc` for the anchor body's current short description.
- Completed: added `SpellAstralProjectionEffect`/`IAstralProjectionEffect` to own the runtime tether, focus shift, projection planar overlay, anchor helpless/sleep/stasis policy, cleanup on removal, and projection retirement without final-killing the primary identity.
- Completed: refined astral projection initial echo ordering so default spells produce one private focus-shift echo to the caster, room echoes are available for observers, and the projection look happens only after focus and observer echo handling have completed.
- Completed: logout, reboot load cleanup, projection retire, projection death, and anchor death now collapse astral projections and return focus to the primary body when viable. Projection death under `CollapseToAnchor` does not create ordinary abandoned-body remains.
- Completed: astral projections receive a normal `PlanarStateEffect` overlay that is present on the configured astral plane, perceives the material/default plane, and blocks ordinary physical, inventory, combat, and medical interactions with material-only targets.
- Completed: `instance list` now marks astral projection rows with anchor instance, plane, and anchor policy metadata while preserving NPC AI/controller details for Phase 6 instances.
- Verified: `dotnet test 'FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance` passed 5 tests.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter "CharacterInstance|AstralProjection|Planar"` passed 21 tests.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `dotnet build MudsharpDatabaseLibrary\MudsharpDatabaseLibrary.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `git diff --check` passed with no whitespace errors.
- Follow-up verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:UseAppHost=false -p:OutDir=bin\CodexVerify\ -p:NoWarn=NU1902%3BNU1510%3BRS2008` passed with 0 warnings.
- Follow-up verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter BodyFormProvisioningTests -p:UseAppHost=false -p:OutDir=bin\CodexVerifyTests\ -p:NoWarn=NU1902%3BNU1510%3BRS2008` passed 24 tests.

Next-phase reflection:

Phase 8 should build on the spell-owned projection path but treat magical copies and physical clones as deliberately different products. The important work is to define clone inventory transfer/copy rules, whether clones are tangible local actors or planar/illusory actors, who controls them, whether they can run AI or scripts, and what death/remains policy applies to each kind. Phase 8 should also harden observer-facing identity presentation so "same identity, different body" is clear without leaking staff-only instance ids to ordinary players.

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

Implementation progress:

- Completed: extended `CharacterInstances.EffectData` metadata with `MagicalCopy` and `PhysicalClone` records, including anchor instance id, body id, source spell id, form key, focusability, intangibility/plane for copies, and persistence policy.
- Completed: added `IMagicalCopyEffect` and `IPhysicalCloneEffect` marker interfaces plus `SpellMagicalCopyEffect` and `SpellPhysicalCloneEffect` runtime effects. These effects own effect expiry/dispel cleanup, logout cleanup for temporary policies, focused-instance return, and non-final secondary retirement.
- Completed: added `CharacterInstanceService.CreateMagicalCopySpawnOptions(...)` and `CreatePhysicalCloneSpawnOptions(...)`. Magical copies use `InstanceKind=MagicalCopy`, `DeathPolicy=CollapseToAnchor`, optional `PlayerFocusable`, optional planar intangibility, and no corpse on collapse. Physical clones use `InstanceKind=PhysicalClone`, ordinary embodied perception, `DeathPolicy=DestroyInstanceOnly`, body-specific clone remains, optional player focusability, and configurable persistence.
- Completed: added `createcopy` and `createclone` spell effect templates with keyed form provisioning options (`formkey`, `race`, `ethnicity`, `gender`, `alias`, `sort`), focusability, persistence controls, copy `plane`/`intangible` controls, collapse/death echoes, and optional backlash echoes. Both spawn from owned dormant forms and do not copy inventory.
- Completed: copy/clone lifecycle cleanup now hooks projection-style retirement paths: logout and stale-login cleanup remove temporary copy/clone effects, anchor death removes active copy/clone effects before normal primary death handling, and secondary retire/death delegates to the owning effect without recursive cleanup.
- Completed: passive secondary death now treats `PhysicalClone` instances as `BodyRemainsContext.SpentClone`, while `CollapseToAnchor` magical copies retire without creating abandoned-body remains.
- Completed: `instance list` now marks `MagicalCopy` and `PhysicalClone` rows with anchor, source spell, form key, plane/intangibility, focusability, persistence, and clone body metadata for staff acceptance testing.
- Verified: `dotnet test 'FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance` passed 5 tests.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter "CharacterInstance|MagicalCopy|PhysicalClone|Planar"` passed 28 tests.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `dotnet build MudsharpDatabaseLibrary\MudsharpDatabaseLibrary.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.

Next-phase reflection:

Phase 9 should harden the compatibility boundaries that Phases 4-8 intentionally preserved. The most valuable next work is auditing identity-sensitive APIs that still consume `Character` as both identity and instance, strengthening admin diagnostics for stale or duplicate instance rows, clarifying observer-facing same-identity presentation without leaking staff ids, and deciding which compatibility mirrors can remain permanently versus which should become explicitly primary-instance-only.

### Phase 0-8 Retrospective

Original design goals:

- Achieved: existing single-body behaviour and the exclusive body-form switch pipeline remain intact. Simultaneous bodies were added as a separate spawn/retire lifecycle rather than by weakening dormant-form switching safety checks.
- Achieved: the runtime now has enough separation between durable identity, active instance, and physical body to support passive forms, PC focus switching, NPC AI instances, astral projection, magical copies, and physical clones.
- Achieved: secondary instances are cell-local world actors and deliberately remain outside `Gameworld.Actors`, `Gameworld.Characters`, `Gameworld.NPCs`, and cached actor collections, avoiding duplicate identity-id collisions.
- Achieved: one live embodied instance per body is guarded by diagnostics, database uniqueness, spawn validation, and retire/death cleanup.
- Achieved: primary compatibility fields still mirror the primary instance, preserving old callers that read `Characters.BodyId`, location, room layer, state, status, and position fields.
- Retained compromise: `Character` still acts as both durable identity and the primary `ICharacterInstance`, and `ICharacter` remains the world actor facade. That was intentional for incremental safety, but it leaves identity-sensitive code audits as continuing work.
- Remaining risk: older systems still compare `actor.Id`, `target == actor`, or `IsSelf(...)` in places where the intended meaning may be identity, account/ownership, or physical instance. Phase 9 starts making these seams visible rather than attempting a high-risk global rewrite.

Phase summary:

- Phases 0-3 established the audit vocabulary, `CharacterInstances` persistence, identity/instance interfaces, and primary-instance compatibility mirroring.
- Phase 4 proved passive secondary actors could exist in rooms without global cache collisions or primary identity death.
- Phase 5 proved runtime PC focus switching, player-safe `instances`/`focus` commands, prompt markers, and logout guardrails.
- Phase 6 proved NPC secondary instances with per-instance controllers and AI subscriptions.
- Phase 7 proved spell-owned astral projection with planar restrictions and collapse-to-anchor lifecycle.
- Phase 8 proved magical copies and physical clones without implicit inventory duplication or final identity death.

### Phase 9: Hardening and Cleanup

Deliverables:

- migrate more identity-scoped systems away from instance facade
- rename or clarify APIs where feasible
- add admin diagnostics
- add data integrity reports
- remove compatibility mirrors only if safe and desirable

Implementation progress:

- Completed: added structured diagnostic subject metadata and grouped diagnostic reports while preserving existing diagnostic codes.
- Completed: added `AuditLoadedIdentity(...)` for loaded identity/instance checks, including duplicate primary instances, duplicate embodied bodies, primary policy mismatches, secondary rows carrying primary kind, embodied live instances without locations, and controllable rows with `NotControllable` policy.
- Completed: extended persisted-row diagnostics to report stale body references, stale location references, malformed `EffectData`, primary policy mismatches, secondary-primary flag mismatches, embodied live rows without locations, and controllable rows with `NotControllable` policy. Phase 9 remains report-only and does not repair rows.
- Completed: added `CharacterInstanceDiagnostics.RenderDiagnosticsTable(...)` so staff commands and tests use one consistent diagnostic table shape.
- Completed: extended the staff `instance` command with `instance audit <character>` for loaded identity diagnostics and `instance audit all` for persisted `CharacterInstances` integrity checks.
- Completed: added small compatibility-boundary comments at the global-cache exclusion, primary compatibility mirror save, and same-identity versus same-physical-instance comparer seams.
- Completed: post-review hardening now sweeps loaded secondary actors on primary logout. Temporary secondaries are retired/deleted, while persistent secondaries are saved, removed from live cell membership, detached from AI/controller/body heartbeat/scheduler state, and left as persisted rows for the next owner load; persistent intangible copy effects reapply their planar overlay when they load again.
- Completed: hardened the initial `CharacterInstances` migration for pre-branch databases by removing first-upgrade hard foreign keys and nulling stale legacy location mirrors during primary-instance backfill so `instance audit all` can report bad persisted data after startup instead of the server failing mid-migration.
- Completed: player-facing `instances`/`focus` and prompt output no longer expose raw internal instance IDs; ordinary players continue to use the display index from the `instances` table or form names, while staff-only IDs remain in the admin `instance` tooling.
- Verified: `dotnet test 'FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance` passed 6 tests.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter "CharacterInstance|InstanceAudit|MagicalCopy|PhysicalClone|AstralProjection|NPCAIEventSubscription"` passed 38 tests.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `dotnet build MudsharpDatabaseLibrary\MudsharpDatabaseLibrary.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `git diff --check` passed; it reported only line-ending normalization warnings for touched files.

Next-phase reflection:

Phase 10 should audit identity-sensitive subsystems outside the instance lifecycle itself. The highest-value targets are combat targeting, parties/movement following, economy/property/employment ownership checks, crime/legal identity, communications and telepathy, and medical/inventory commands that still use direct actor reference or identity-id comparisons. The goal should be to classify each comparison as identity-scoped or physical-instance-scoped and replace ambiguous checks with `SameIdentity(...)`, `SamePhysicalInstance(...)`, or explicit account/ownership checks where appropriate.

### Phase 10: Instance-Aware Legacy Subsystems Investigation, Project Labour, NPC Operations, Vehicle, Hitch, Arena, and Reporting Slices

Purpose:

Phase 10 began as an investigation and design-hardening pass for legacy systems that were deliberately left identity-scoped, guarded, or partially compatible after Phases 4-9. The first implementation slice was project labour: secondary instances can now hold active work state as physical actors while personal project ownership and labour queues remain identity-owned compatibility surfaces. The second implementation slice covered NPC patrols, group AI membership, bodyguard caches, and stable stays. The final Phase 10 slice adds the shared actor-reference helper, removes the vehicle and arena guardrails that were waiting on instance-aware persistence, and extends staff diagnostics so stale physical-instance references are visible rather than silently collapsing to the primary body.

Source observations:

- Before the Phase 10 project-labour slice, project labour stored active work and queued labour through `Characters.CurrentProjectId`, `Characters.CurrentProjectLabourId`, project-hour compatibility fields, and `ProjectLabourQueues`. `ActiveProject.ActiveLabour` was runtime-physical, but persisted reload resolved workers through `TryGetCharacter(...)`, which meant it reloaded to the primary identity facade rather than a specific secondary instance.
- After the project-labour slice, `CharacterInstances.CurrentProjectId`, `CharacterInstances.CurrentProjectLabourId`, `CurrentProjectHours`, and `CurrentProjectProjectHours` carry active work state for the physical instance. The legacy `Characters.CurrentProject*` columns remain primary-instance compatibility mirrors only.
- NPC patrols and group AI still keep durable identity ids for compatibility, but now also persist optional physical instance ids. Old rows without instance ids fall back to the primary NPC; new rows with explicit instance ids resolve the loaded physical instance and do not silently fall back when that specific body is unavailable.
- Vehicle occupancy and persistent character hitches now persist nullable `CharacterInstanceId` endpoints where a character endpoint needs physical-instance semantics. Legacy rows without instance ids remain primary-compatible, while explicit instance rows resolve the loaded physical actor and can report staleness when that actor is gone.
- Arena signup rows now keep `CharacterId` for identity-owned signup, rating, betting, and payout semantics, plus nullable `ActiveCharacterInstanceId` for the physical competitor that is staged, equipped, moved, and resolved during the event.
- Staff-facing vehicle, hitch, arena, and instance-audit output now has a shared actor-reference formatter for physical-instance-sensitive reports. Discord and older flat text reports may still need additional audit coverage where they are not part of these specific paths.

Recommended persisted actor reference:

```text
CharacterId: durable identity row; required for characters and NPCs.
CharacterInstanceId: optional physical instance row; required when the subsystem needs the same loaded body after save/load.
BodyId: optional body row; useful for corpse/remains, clone, vehicle, medical, and equipment contexts.
ReferenceKind: IdentityOnly | PrimaryInstance | SpecificInstance | BodyOnly | FrameworkItem.
```

The resolver for this shape should be centralized rather than reimplemented in every subsystem. It should return one of:

```text
LoadedSpecificInstance
LoadedPrimaryFallback
PersistentInstanceUnavailable
TransientInstanceExpired
BodyUnavailable
IdentityUnavailable
```

The default compatibility rule should be conservative: old rows without `CharacterInstanceId` resolve to the primary instance, while newly instance-aware subsystems must write `CharacterInstanceId` whenever physical location, movement, combat, equipment, body state, or AI membership matters.

Subsystem decision matrix:

| Subsystem | Current storage key | Intended owner | Required model/API change | Migration/backfill | Cleanup and stale handling | Test focus |
| --- | --- | --- | --- | --- | --- | --- |
| Project active labour | `CharacterInstances.CurrentProject*` plus primary `Characters.CurrentProject*` compatibility mirrors | Physical instance for active work; identity for personal project ownership | Implemented for current work state; broader actor-reference helper still recommended for other systems | Backfilled current active work to the primary instance; preserved existing project progress rows | Retiring/death/logout of a working secondary must leave project work, clear current work state, and rerun queue matching | Secondary can work/tick/leave without changing primary; temporary worker retires cleanly; primary compatibility still loads old rows |
| Project labour queue | `ProjectLabourQueues.CharacterId` | Identity by default, optionally physical instance for "this body should work here" | Identity-owned queue implemented with candidate-body readiness checks; optional body-specific queue scope remains deferred | Existing queues remain identity-wide | If a queued instance expires, identity queue can be claimed by another valid focused body; body-specific stale policy is deferred | Identity queue can be claimed by any valid focused body; later body-specific queues should resolve to the chosen instance |
| Agriculture project ticks | Active project worker actor | Physical instance | Use active labour actor reference rather than identity fallback for trait checks and labour impacts | No data migration beyond active labour | If worker disappears mid-tick, skip and clean stale worker entry | Labour effects apply to focused/working body only |
| NPC patrol membership | `PatrolLeaderId`/`PatrolLeaderInstanceId` and `PatrolMember.CharacterId`/`CharacterInstanceId` | Physical NPC instance for active patrol; identity for reporting | Implemented optional instance references and physical-instance removal/selection checks | Existing patrol rows resolve to primary NPC identities | Missing explicit instance references are pruned rather than silently primary-fallbacking | Persistent secondary patrol reloads same instance; removing one body does not remove another same-identity body |
| Group AI membership | XML member actor refs with legacy identity-id compatibility | Physical NPC instance for herd/group movement and combat | Implemented `<Member character="" instance="">` XML, physical-instance `GroupRoles`, and physical threat/member exclusion | Existing member ids resolve to primary NPCs | Missing explicit members are pruned from loaded membership | Group roles can distinguish same-identity instances; threat scans do not exclude the wrong body |
| NPC bodyguard cache | `Npc.BodyguardCharacterId` and `CachedBodyguards` keyed by guarded identity | Guarded target identity, guarding actor physical instance | Implemented duplicate-safe physical-instance cache entries while preserving identity-scoped guarded target | Existing rows keep target identity id | Login drains distinct physical guards only; secondary guards are not forced into global actor caches | Two same-identity NPC instances do not duplicate or collapse cache entries |
| Stable stays | `StableStays.MountId`/`MountInstanceId` | Physical mount instance; owner identity for tickets and accounting | Implemented optional mount instance ids, boot suppression limited to legacy/primary rows, and persistent-secondary restore | Existing stays without instance id are treated as primary/legacy | Temporary secondary mounts are rejected; persistent secondaries rematerialize by instance row on redeem | Stabled secondary does not suppress primary NPC boot; redeem restores the lodged physical body |
| Vehicle occupancy | `VehicleOccupancy.CharacterId` | Physical instance | Add `CharacterInstanceId` to occupancy rows and update `IsOccupant`, `Board`, `Leave`, controller assignment, movement ejection, and save/load | Existing occupants map to primary instance | Retiring/death/logout while aboard must leave/eject before secondary cleanup | Focusable secondary can board/leave; primary is not treated as aboard; persistent secondary occupancy reloads |
| Persistent hitch character endpoints | source/target character ids | Physical instance endpoint when endpoint is a character | Add optional instance ids to character endpoints; keep framework item endpoints unchanged | Existing character hitches map to primary endpoints | Missing transient endpoint breaks or stales the hitch link | Secondary can be a hitch endpoint only when persistent enough to resolve |
| Arena participant rows | `ArenaSignup.CharacterId`, reservations, ratings, payouts | Identity for signup/rating/betting; physical instance for live competitor | Split participant identity from active competitor instance; add active physical actor binding during staging/prep/live | Existing signups bind to primary instance | Retire/death during event withdraws, forfeits, or replaces by explicit arena policy | Secondary can compete without moving primary; rating/payout remains identity-scoped |
| Arena equipment/staging | Effects and BYO equipment owner identity | Physical competitor for equipment movement and staging; identity for ownership/provenance | Bind staging/preparation effects to active competitor instance id | Existing effects resolve to primary competitor | Missing competitor cleans staging effects and releases equipment | Equipment is prepared on the competing body only |
| Logs, Discord, diagnostics | Free text and assorted ids | Explicit display of identity and instance where relevant | Add staff-only formatter for actor references: identity id, instance id, body id, instance kind, and location | No migration | Missing instance displays stale/resolved status rather than pretending it is a character id | Staff output distinguishes primary and secondary in reports |

Recommended implementation slices:

1. **Actor reference infrastructure and diagnostics** - implemented as the final Phase 10 slice with a shared `CharacterActorReference`, loaded resolver, staff renderer, and `instance audit all` checks for stale vehicle, hitch, and arena instance references.
2. **Project labour instance state** - implemented for active work and current-project hours in `CharacterInstances`, while leaving personal project ownership, labour queue rows, and old compatibility fields identity/primary scoped.
3. **NPC patrol, group AI, bodyguard, and stable instance references** - implemented as the second Phase 10 slice with optional instance ids and physical-instance runtime comparisons.
4. **Vehicle occupancy and hitch endpoints** - implemented nullable instance ids for vehicle occupancy and character hitch endpoints, physical-instance runtime comparisons, staff display, and retire cleanup.
5. **Arena physical competitor binding** - implemented identity-level participant rows with nullable active physical instance binding for staging, equipment, movement, participation effects, surrender, and NPC AI opponent resolution.
6. **Reporting standardization** - implemented shared staff actor-reference rendering for the new vehicle/hitch/arena/diagnostic surfaces; broader Discord and legacy flat-report coverage remains a Phase 11 audit item.

Acceptance criteria for the investigation output:

- Every investigated subsystem is classified as identity-scoped, physical-instance-scoped, body-scoped, or mixed.
- Every mixed subsystem has a recommended storage shape and load/cleanup policy.
- Existing guardrails remain documented until their corresponding storage model is implemented.
- Old data without instance references has a deterministic primary-instance fallback.
- Transient secondary references have an explicit stale/expired outcome rather than falling back silently to the primary when physical semantics matter.

Tests to require when these slices are implemented:

- A secondary project worker can join, tick, leave, retire, and die without altering the primary's current-project state.
- Persistent NPC secondary patrol/group membership reloads to the same instance, while temporary members are pruned or abort the activity by policy.
- Secondary vehicle occupancy survives save/load for persistent instances and clears on retire/logout for temporary instances.
- Arena ratings, bets, and payouts remain identity-scoped while staging, equipment, scoring, and death target the active physical competitor.
- Staff reports show identity id and instance id distinctly, while player-facing outputs continue to hide staff-only ids.

Investigation result:

- Completed: mapped the remaining design-sensitive areas to concrete current storage and runtime behaviours: project labour/current-project fields, NPC patrol/group membership, NPC bodyguard cache, vehicle occupancy/hitch endpoints, arena participant state, and staff/reporting output.
- Completed: identified the shared prerequisite for further work: a reusable persisted actor reference and resolver that can express both identity ownership and physical instance binding.
- Completed: preserved the current runtime guardrails as deliberate compatibility boundaries until their corresponding storage model was implemented, then removed them for project labour, NPC operations, vehicles, hitches, and arena participation.
- Verified: `git diff --check` passed for the documentation update; it reported only line-ending normalization warnings.

Project labour implementation result:

- Completed: added per-instance active work fields to `CharacterInstances` and a narrow migration that backfills primary instance rows from the legacy `Characters.CurrentProject*` columns without adding first-upgrade foreign-key blockers.
- Completed: primary character save/load now keeps legacy `Characters.CurrentProject*` compatibility mirrors aligned with the primary instance row, while secondary instance save/load persists current work only to that secondary `CharacterInstances` row.
- Completed: active project reload now reconstructs active workers from `(CharacterId, CharacterInstanceId)` where available, falling back to primary identity only for legacy rows without an instance id.
- Completed: personal and local project ownership now normalizes to the durable identity, while join/leave/tick/progress state remains on the physical actor instance that actually works.
- Completed: project labour queues remain identity-owned for this slice, but queue readiness can be evaluated against the candidate physical instance trying to claim the work.
- Deferred: optional body-specific labour queues, shared persisted actor-reference infrastructure, and the vehicle, hitch, arena, and reporting upgrades remain later Phase 11+ slices.
- Verified: `dotnet build MudsharpDatabaseLibrary\MudsharpDatabaseLibrary.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter ProjectLabourQueueEntry` passed 1 test.
- Verified: `git diff --check` passed; it reported only line-ending normalization warnings.

NPC patrol, group AI, bodyguard cache, and stable implementation result:

- Completed: added nullable `PatrolLeaderInstanceId`, `StableStays.MountInstanceId`, and defaulted `PatrolMembers.CharacterInstanceId` columns via EF migration `20260614233932_CharacterInstanceNpcPatrolStableInstances`.
- Follow-up: hardened `20260614233932_CharacterInstanceNpcPatrolStableInstances` for mature MySQL upgrades by keeping the legacy `PatrolMembers` primary key on `(PatrolId, CharacterId)` and treating `CharacterInstanceId` as indexed actor-reference metadata. This avoids the MySQL `Cannot drop index 'PRIMARY': needed in a foreign key constraint` startup crash while preserving legacy patrol membership uniqueness.
- Completed: extended `CharacterInstanceIdentityComparer` with physical instance id helpers, loaded instance resolution, physical list operations, and a physical-instance dictionary comparer for systems like `GroupRoles`.
- Completed: patrol save/load now writes identity plus instance ids for leaders and members, resolves explicit instance refs without unsafe primary fallback, and removes patrol members by physical instance.
- Completed: group AI save/load now supports legacy `<Id>` members and new `<Member character="" instance="">` refs; threat exclusion, group role cleanup, and emote selection now use physical-instance checks.
- Completed: bodyguard caches remain keyed by guarded identity but now store distinct physical guard actors, avoid duplicate cache entries, and do not add secondary guards to global actor caches.
- Completed: stable stays now remember the lodged mount instance, reject temporary secondary mounts, avoid suppressing the primary NPC at boot when only a secondary body is stabled, and restore persistent secondary mounts through the instance service.
- Completed: local room membership add/remove now uses physical instance equality so same-identity bodies can coexist in a cell without `List<ICharacter>` equality collapsing them.
- Verified: `dotnet test 'FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance` passed 16 tests.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter "CharacterInstance|Stable|BootLoading|LegalPatrol|NPCAIEventSubscription|ProjectLabourQueueEntry"` passed 59 tests.
- Verified: `dotnet build MudsharpDatabaseLibrary\MudsharpDatabaseLibrary.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings after rerunning sequentially; one earlier parallel build hit a transient compiler output file lock while the database project was building separately.
- Verified: `git diff --check` passed; it reported only line-ending normalization warnings.

Actor reference, vehicle, hitch, arena, and reporting implementation result:

- Completed: added `CharacterActorReference`, `CharacterActorReferenceResolution`, a loaded actor resolver, and a staff renderer that displays identity id, instance id, body id, instance kind, location, and stale resolution status.
- Completed: added nullable `VehicleOccupancies.CharacterInstanceId`, stored generated `CharacterInstanceKey`, nullable `VehicleHitchLinks.SourceCharacterInstanceId`, nullable `VehicleHitchLinks.TargetCharacterInstanceId`, and nullable `ArenaSignups.ActiveCharacterInstanceId` via EF migration `20260615024353_CharacterInstanceActorReferences`.
- Follow-up: hardened `20260615024353_CharacterInstanceActorReferences` for mature database upgrades by keeping the new actor-reference columns indexed but removing hard `CharacterInstances` foreign keys. Stale vehicle, hitch, and arena instance references remain reportable through `instance audit all` instead of blocking startup when a mature database contains legacy or missing instance rows.
- Completed: vehicle occupancy now uses physical-instance equality for boarding, controller checks, leave/eject flows, and persistence, so a secondary can board without making the primary appear aboard.
- Completed: persistent character hitches now store optional source/target instance ids, resolve explicit instance endpoints without unsafe primary fallback, and reject non-persistent secondary endpoints.
- Completed: arena signups remain identity-owned for reservations, ratings, betting, and payouts, but bind the active physical competitor for preparation, BYO/equipment movement, arena teleporting, participation effects, surrender checks, NPC AI opponent selection, and arena prog participant lists.
- Completed: retiring or unloading a secondary now clears project, vehicle, hitch, and arena participation bindings before the instance is removed from its cell and persistence row.
- Completed: `instance audit all` now reports stale or contradictory vehicle occupancy, hitch endpoint, and arena active-competitor references in the existing structured diagnostic table.
- Completed: staff vehicle/hitch/arena output uses the shared actor-reference renderer where physical instance identity matters.
- Verified: `dotnet test 'FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance` passed 20 tests.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter "CharacterInstance|InstanceAudit|Vehicle|Hitch|Arena"` passed 162 tests.
- Verified: `dotnet build MudsharpDatabaseLibrary\MudsharpDatabaseLibrary.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `git diff --check` passed; it reported only line-ending normalization warnings.

Next-phase reflection:

Phase 11 should focus on broader identity-sensitive subsystem audits now that the main Phase 10 guarded behaviours have storage and runtime support. The highest-value remaining targets are combat helper edge cases, party and movement-following state, medical and inventory command paths, crime/legal reports, communications/telepathy, Discord/exported staff reports, and any old helper still comparing `actor.Id`, `target == actor`, or `IsSelf` without making identity-vs-physical semantics explicit. The goal should be steady hardening rather than new gameplay: classify each path, replace ambiguous comparisons with `SameIdentity(...)`, `SamePhysicalInstance(...)`, or explicit ownership/account checks, and add diagnostics where stale explicit instance references can occur.

### Phase 11: Identity-Semantics Hardening and Legacy Command Audit

Purpose:

Phase 11 is a hardening slice rather than a new gameplay phase. It takes the broader Phase 10 reflection and applies it to legacy paths that still used direct object equality, `actor.Id`, or implicit self checks where the intended meaning was either physical actor instance or durable identity. The goal is not to make every old subsystem fully multi-instance-native in one pass; it is to make the most exposed player/staff command and movement paths explicit enough that passive bodies, focused PC instances, NPC secondaries, astral projections, magical copies, and physical clones do not collapse into the primary identity by accident.

Implementation result:

- Completed: extended `CharacterInstanceIdentityComparer` with `PhysicalInstanceKey(...)`, `SamePhysicalInstanceOrBody(...)`, and `SameIdentityOrPrimaryOwner(...)` helpers so call sites can choose physical-instance, body-owner, or durable-identity semantics explicitly.
- Completed: party, follower, ordinary movement, vehicle movement, and tollkeeper prospective-mover checks now use physical-instance comparison for leader, member, witness, and same-mover decisions instead of direct object/list equality.
- Completed: combat helper paths for surrender, self-aiming, visible target lists, guard permit/forbid, interpose, spar, and hit now treat "self" and "fighting me" as physical-instance semantics.
- Completed: spell cast triggers with `CanTargetSelf` now evaluate self-targeting against the active physical instance or body, so a spell can distinguish another same-identity body from the focused caster body.
- Completed: medical, inventory, and manipulation command gates for CPR, vitals consent, wound repair, surgery consent, restraint, dress/strip consent, outfit teaching, dragging, feeding, container delegation, open/close delegation, and prosthetic install/remove now use physical-instance semantics where they refer to the body being acted on.
- Completed: employment task item custody now keys carried task resources and persisted collect/deliver resource metadata by physical instance. Finance authorisation/reservation strings remain identity-scoped because they record authority rather than body custody.
- Completed: board post delete permissions now compare the post author to `IdentityId(actor)`, preserving identity-owned communications while the player is focused into another owned body.
- Completed: `instance audit all` now includes loaded global actor-cache diagnostics for secondary instances leaking into `Actors`, `Characters`, `NPCs`, or `CachedActors`, and for multiple physical actors for the same identity in those global caches.
- Completed: added focused regressions for the new comparer helpers and loaded global cache diagnostics.
- Deferred: crime/legal reports, telepathy/channel/Discord/export reports, social recognition, disguise, and broader item/container owner displays still need a Phase 12 audit where the correct meaning may vary between legal identity, account identity, visible physical body, and staff reporting context.
- Verified: `dotnet test "FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj" -c Debug --no-restore -m:1 --filter CharacterInstance` passed 24 tests.
- Verified: `dotnet test "MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj" -c Debug --no-restore -m:1 --filter "CharacterInstance|InstanceAudit|Movement|Party|Combat|MagicTrigger|Health|Inventory|EmploymentTask|Communications"` passed 137 tests.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `dotnet build MudsharpDatabaseLibrary\MudsharpDatabaseLibrary.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `git diff --check` passed; it reported only line-ending normalization warnings for touched files.

Next-phase reflection:

Phase 12 should continue the same classification work in systems where "same character" is not naturally one thing. Crime, law, warrants, witnesses, reports, telepathy, channels, Discord exports, elections, staff dashboards, social recognition, disguise, and ownership displays may need a mixed model: some checks should follow durable identity, some should follow the physical witnessable body, and some should render both for staff. The next slice should begin with an audit table of these paths, then implement the highest-risk runtime paths first rather than adding new instance gameplay.

### Phase 12: Identity Policy Decisions, Recognition, Legal Reporting, Telepathy, and FutureProg APIs

Purpose:

Phase 12 is the final V1 foundation pass for the multi-instance stack. It turns the remaining design-sensitive areas into explicit policies, implements the low-risk API and runtime hardening needed now, and leaves broader gameplay content for later phases. It does not add a new instance kind or schema migration.

Accepted policy matrix:

| Area | V1 Policy | Implementation Notes |
| --- | --- | --- |
| Crime and legal liability | Mixed identity/body model. Crimes are identity-scoped when the criminal identity is known; otherwise the observed physical body/form is the enforcement target until identity is established. | This maps onto the existing `CriminalIdentityIsKnown` and `WitnessProfile.IdentityKnownProg` functionality. Witness/building progs can decide whether an astral projector, clone, disguised body, or ordinary actor is recognised as the wider identity. |
| Arrest, custody, prison, release | Identity-known crimes may justify identity-wide enforcement policy, but the immediate arrest/custody action is performed on the physical actor body present to the enforcer. If identity is not known, only the physical body/form is the enforcement subject. | Existing custody commands continue to mutate the passed physical actor. Known-crime lookups remain identity-id based. Staff views now expose actor identity/instance/body context where available. |
| Telepathy, think/feel, channels | Mental communication follows the identity and delivers to the currently focused controller context only. | `think` and `feel` still evaluate telepathy effects on loaded identity actors, but output now goes to the identity's focused instance when focus is on a secondary body. It does not echo to every loaded body. Ordinary account/channel communication remains identity/account scoped. |
| Dubs, recognition, disguise | Option C now: ordinary character dubs remain identity dubs; body/form dubs are separate physical recognition keys. | `dub <target> <keyword>` keeps old character-identity behaviour. `dub body <target> <keyword>` and `dub form <target> <keyword>` create body-scoped dubs keyed to `Body` + body id. Recognition lookup checks both identity and body keys, so later disguise/projection systems can choose the correct key without rewriting existing dubs. |
| Staff, Discord, and reports | Privacy split. Staff/admin reports may show identity, instance, and body context; player/public outputs do not leak staff ids. | Shared staff actor references remain the preferred report shape. Legal Discord enforcement tokens now carry compact identity/instance/body ids without changing the bot command shape. Crime info shows staff actor references for administrators. |
| Community, clans, elections, payroll | Identity roles and entitlements stay identity-scoped; visible actions still use the physical actor taking the action. | No schema change in this phase. Existing clan/community/payroll semantics remain durable-identity based unless a later feature explicitly introduces body-specific office or legal personhood. |
| FutureProg and builder APIs | Backward-compatible explicit APIs. Existing character `.id` semantics remain stable; new progs and dot references expose identity, instance, body, and comparison semantics. | Added character dot references `identityid`, `instanceid`, `physicalinstancekey`, `bodyid`, `isprimaryinstance`, `instancekind`, `instancekindid`, `primaryinstance`, and `focusedinstance`. Added built-in functions `sameidentity`, `samephysicalinstance`, `characteridentityid`, `characterinstanceid`, `characterbodyid`, and `tocharacterinstance`. |

Implementation result:

- Completed: `CharacterInstanceIdentityComparer` now exposes explicit recognition keys for durable identity (`Character` + identity id) and physical body/form recognition (`Body` + body id).
- Completed: character and body perception dub lookups now consult identity and body recognition keys, so old identity dubs and new body/form dubs can both resolve.
- Completed: `dub body` / `dub form` creates body-scoped physical recognition while preserving `dub <target>` as the legacy identity-scoped character dub path.
- Completed: legal Discord enforcement messages keep the existing positional protocol but replace the plain character id token with a compact staff token containing identity id, current instance id, and body id.
- Completed: administrator crime info includes staff actor references so legal investigations can distinguish identity-known records from the loaded physical actor currently associated with that identity.
- Completed: `think` and `feel` deliver telepathy output to the recipient identity's currently focused instance instead of silently targeting only the primary character object.
- Completed: FutureProg has explicit identity/instance/body dot references and comparison/lookup functions for builder-authored instance-aware logic.
- Completed: focused unit coverage now verifies identity recognition keys and physical body recognition keys remain distinct.

Verification:

- Verified: `dotnet build FutureMUDLibrary\FutureMUDLibrary.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `dotnet test 'FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance` passed 27 tests.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter "CharacterInstance|Crime|Legal|Communication|Telepathy|Dub|FutureProg|InstanceAudit"` passed 116 tests.
- Verified: `dotnet build MudsharpDatabaseLibrary\MudsharpDatabaseLibrary.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed with 0 warnings.
- Verified: `git diff --check` passed; it reported only line-ending normalization warnings.

Post-V1 hardening note, June 15, 2026:

- Fixed: database-loaded dormant bodies now initialise their public inventory helper views even before full inventory load, so look/targeting/perception code sees empty collections rather than null collections.
- Fixed: secondary instance materialisation now sets instance state, location, and room layer before hydrating the secondary body's inventory and activating the body. This allows invalid held/worn/wielded items to fall back into the correct cell and makes passive, focusable, NPC AI, astral, copy, and clone bodies look-ready before room exposure.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510 -p:OutDir=C:\Users\luker\.codex\worktrees\88f6\FutureMUD\.codex-build\MudSharpCore\` passed with 0 warnings and 0 errors. The scratch output path was used because the live MudSharp process was locking the normal `bin` outputs.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter BodyInventoryInitialisationTests -p:NoWarn=NU1902%3BNU1510 -p:OutDir=C:\Users\luker\.codex\worktrees\88f6\FutureMUD\.codex-build\MudSharpCoreTests\` passed 1 test.

Post-V1 hardening note, June 16, 2026:

- Fixed: secondary instance materialisation now initialises `NeedsModel` from the primary identity before the secondary can be focused or queried by health/needs commands. Existing V1 behaviour keeps needs identity-wide and avoids registering duplicate needs heartbeats for secondary actors.
- Fixed: primary load now creates the identity `NeedsModel` before persisted secondary rows are materialised, so reboot-loaded secondaries and newly spawned secondaries follow the same non-null needs initialisation path.
- Fixed: newly provisioned dormant form bodies now choose target-race height/weight when the form race differs from the source body, generate target-race characteristic values from the selected ethnicity before validating description patterns, and initialise blood volume, liver function, organ state, and stamina before the form can be embodied as a secondary instance. Description patterns are still rejected if variables remain unresolved after characteristic generation, and secondary materialisation also repairs older dormant form rows with zero, negative, or non-finite blood/stamina values before they enter a room.
- Added: `body delform <character> <form> confirm` lets founders permanently remove incorrectly provisioned dormant forms. The command requires explicit confirmation and refuses current bodies, live embodied instances, persisted instance references, body backup references, and corpse/remains-style physical references before deleting the form metadata, body source mappings, dormant body, and any items on that body.
- Added: `spawnbodyinstance(character, form, location, mode[, cloneInventory[, ais]])` exposes the secondary instance lifecycle to FutureProg. The function resolves owned dormant forms by alias or body id, supports passive, player-focusable, NPC AI, and scripted AI modes, optionally deep-clones the source body's direct inventory, and returns the spawned physical actor as a `Character` prog value.
- Added: `ScriptedAi` secondary instances provide a PC-capable AI-controlled body path for builder-authored scenarios such as evil twins. They use a cell-local secondary `Character`, NPC command tree/controller semantics, effect-data metadata for attached AI ids, non-final `DestroyInstanceOnly` death, and remain out of global character/NPC actor caches.
- Hardened: staff NPC possession now checks the target actor and its owning primary identity, so a non-player `ScriptedAi` or passive secondary attached to a PC identity cannot be possessed as if it were an ordinary NPC.
- Added: `CharacterInstanceService.CloneInventory(...)` deep-copies direct worn, wielded, and held items from a source actor onto a secondary body without moving or duplicating by reference. Items that cannot be worn, wielded, or held by the new body are left in the target room instead of failing the spawn.
- Added: staff `instance spawn` now accepts `ai` for NPC AI secondaries or PC scripted-AI secondaries, `scriptai` for explicit scripted-AI PC-style bodies, and `cloneinventory` for smoke-test inventory cloning. The AI builder `ai add/remove/list` path can now manage loaded `IArtificialIntelligenceControlledCharacter` secondaries as well as ordinary NPCs.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510 -p:UseSharedCompilation=false` passed with 0 warnings and 0 errors.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance -p:NoWarn=NU1902%3BNU1510` passed 35 tests.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter BodyFormProvisioningTests -p:NoWarn=NU1902%3BNU1510 -p:OutDir=C:\Users\luker\.codex\worktrees\88f6\FutureMUD\.codex-build\MudSharpCoreTests\` passed 19 tests.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter "BodyFormProvisioningTests|CharacterInstance" -p:NoWarn=NU1902%3BNU1510 -p:OutDir=C:\Users\luker\.codex\worktrees\88f6\FutureMUD\.codex-build\MudSharpCoreTests\` passed 55 tests.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510 -p:OutDir=C:\Users\luker\.codex\worktrees\88f6\FutureMUD\.codex-build\MudSharpCore\` passed with 0 warnings and 0 errors.

Final V1 documentation and smoke-test pass, June 16, 2026:

- Added: [Multiple Body Forms and Instances Builder Guide](./Multiple_Body_Forms_and_Instances_Builder_Guide.md) is the builder-facing runbook for exclusive forms, simultaneous instances, player focus, staff commands, spell effects, FutureProg APIs, diagnostics, and worked examples.
- Updated: the design-document index and the multiple body form system document now link builders to both this architecture document and the builder guide.
- Fixed: `PlayerConnection.PrepareOutgoing()` now snapshots the current control puppet and output handler for the send cycle, preventing a disconnect or focus-context transition from nulling `ControlPuppet.OutputHandler` between the initial guard and the buffered-output check.
- Fixed: race loading now uses split/no-tracking query materialisation for the large race include graph and explicitly includes additional bodypart prototypes. Mature demo database startup logs improved from approximately 31.6 seconds for `Loading Races` to approximately 0.7-0.8 seconds for 293 races.
- Verified live smoke: a prog-created `ScriptedAi` evil-twin secondary was spawned from an owned dormant form with inventory cloning enabled, appeared as a separate `ScriptedAi` instance, engaged the primary actor through a `trackingaggressor` AI using `SameIdentity` and `SamePhysicalInstance`, and was pruned on reboot as a `DespawnOnReboot` secondary.
- Verified: `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510 -p:UseSharedCompilation=false` passed with 0 warnings and 0 errors.
- Verified: `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance -p:NoWarn=NU1902%3BNU1510 -p:UseSharedCompilation=false` passed 35 tests.
- Verified: `dotnet test 'FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -m:1 --filter CharacterInstance -p:NoWarn=NU1902%3BNU1510 -p:UseSharedCompilation=false` passed 27 tests.
- Verified: `git diff --check` passed; it reported only line-ending normalization warnings for touched files.

V1 reflection:

Phase 12 plus the final documentation and smoke-test pass makes the multi-instance architecture V1-foundation ready. The system now supports one durable identity with multiple loaded, cell-local physical actors; staff-created passive/focusable/scripted-AI instances; NPC AI secondaries; astral projection; magical copies; physical clones; physical-instance persistence in the highest-risk legacy actor-reference surfaces; explicit helper APIs for identity-vs-instance decisions; and a builder runbook that describes how to author and test the feature. Later work should be treated as feature expansion rather than architectural prerequisite.

Post-V1 design work:

Future phases can still add richer gameplay and deeper audits: body-specific legal personhood, disguise-aware identity discovery, observer memory and recognition decay, projection-specific criminal evidence, per-clone social consequences, channel/telepathy policy configuration, player-facing recognition controls, and broader FutureProg convenience APIs. Those are now content and policy layers on top of the V1 foundation rather than blockers for simultaneous body instances.

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

Phase 12 resolves the V1 defaults for legal identity, arrest/custody, telepathy focus, recognition keys, reporting privacy, community ownership, and FutureProg compatibility. The questions below remain useful for post-V1 gameplay expansion and content-specific policy decisions.

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
9. harden diagnostics, compatibility mirrors, and identity/instance boundaries.
10. investigate legacy identity-only actor references and implement the project-labour, NPC patrol/group AI, bodyguard cache, and stable instance-state slices.
11. harden identity-vs-physical-instance semantics in legacy movement, combat, medical, inventory, manipulation, employment, communications, and diagnostics paths.
12. settle remaining V1 policy decisions for law, recognition, reports, telepathy, community, and FutureProg APIs, then expose explicit identity/instance/body helper surfaces for builders and staff.

The V1 foundation now exists. The most important ongoing risks are older subsystem paths that still treat `Character.Body`, `actor.Id`, direct object equality, or player-facing recognition as if one durable identity can only have one visible body. Future phases should extend the policy layer rather than reopen the core instance model.
