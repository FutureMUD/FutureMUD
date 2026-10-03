# Spell-owned lifecycle foundation

Phase3B1 adds the conservative canonical NPC archival boundary described below. Phase3B2A
connects explicit `createnpc` lifecycle provenance to native simple-template construction and
persisted death/remains recovery. Ordinary corpse deletion/decay, foreign-item evacuation and
the scheduled retirement worker remain later adapter work. Full N14-N16 remains outstanding.

## Native NPC creation and death recovery

The optional world `SpellOwnedNpcs` service and template `CreateSpellOwnedCharacter` overload
create an accountless native NPC from an approved simple template. Legacy `createnpc` XML
keeps its existing creation behavior. Builders opt in with `lifecycle permanent`,
`lifecycle temporarycleanup` or `lifecycle deathonexpiry`, set `family <name>`, and give
temporary modes a positive finite `lifetime <expression>` in real seconds. The selected
casting grade binds that formula; this absolute lifetime is independent of spell/control
duration. Missing grades, bad schema/mode, invalid formulas, unapproved or variable templates,
prostheses, role trait adjustments and merit-provided extra bodies refuse until their
creation adapters exist. Role trait adjustments currently mutate independently queued
traits and invoke native callbacks; refusing them preserves their ordinary creation semantics.

Configured casting checks deterministic creation eligibility before preparing inventory or
committing payment. Admission and native Character/Body constructors share scoped merit
composition: selected merits, role additions, body-merit deduplication and one-level combo
children in authored order. Effective additional-body merits refuse before spending resources
or materials, opening a casting operation or advancing skill/mastery opportunities. The
service revalidates the same rules before construction; the deferred constructor retains its
additional-body guard. Withdrawn approval, a mismatched template world and an unavailable
native lifecycle service also refuse during configuration validation. Ordinary creation keeps
its existing role/trait timing and merit composition; extra-body ownership remains deferred.

Resolved temporary lifetime and spawn validation remain effect-application checks after the
casting result. Lifetime expressions may depend on opposed outcome. This admission correction
does not move their evaluation or qualify a complete installed gameplay session.

Preparation allocates a stable creation key and canonical creator ID. A fresh serializable
transaction inserts the production NPC/Character/Body/primary instance graph and exact
autonomous-character/body claims with UTC creation/deadline, source spell, selected grade,
family, template/invocation provenance and an activation-pending diagnostic. The private
graph has no queued initialization, cell/world presence, controller or hook subscriptions
before that transaction commits. Starting CharacterKnowledge children also suppress their
own queues and ID-triggered flush until the character graph assigns committed row IDs.
No unrelated pending saves are flushed.

The persisted actor and primary instance begin in Stasis. After the ownership commit, native
needs/vitals/language/default hooks and controller activation run; a second atomic transaction
saves that private graph, preserves authored raw character skills even when a dynamic cap
exposes a lower usable value, resumes and saves starting knowledge privately, and clears
activation-pending before returning it for world insertion,
template additions, both on-load programs, cell login and game-load events. A failed activation
releases partial hooks, heartbeats, controller and save roots, retains the owned inactive graph
with a durable diagnostic, and refuses creation replay. Startup and `TryGetCharacter` exclude
activation-pending identities. Crash recovery does not recreate the NPC or rerun activation.
Permanent creation completes its journal after activation and becomes an ordinary durable NPC
without a deadline. Temporary creation retains active ownership evidence.

`NPC.Die` correlates only after the native method has persisted death and created its optional
remains. Actual corpse insertion is saved before recording its ID, and the journal independently
checks dead state and the exact body in persisted remains XML. Pre-death events are not death
proof. An already-dead `Character.Die` returns existing remains without repeating death events
or creating another corpse. Failed correlation retains a bounded retry diagnostic.

After startup NPC load callbacks and on the minute heartbeat, a bounded persisted query finds
unobserved dead owned NPCs, including early deaths whose original deadline is still future.
It rotates its cursor, reads exact body ownership and correlates zero or one persisted remains
reference without materializing a dead Character/Body/controller/AI graph. Multiple matches,
malformed candidates, changed bodies or a census over 256 candidate component rows hold.
The candidate census includes numeric XML character references, whose decoded body IDs must
not be mistaken for absence merely because the raw serialized digits differ.
Observation cancels the need for a second native death; retries do not change existing remains,
recast, delete rows or replay callbacks. This service does not evacuate custody or retire anything.

Native acceptance uses real maintained-snapshot MySQL, `SimpleNPCTemplate`, production new
NPC/Body/primary insertion, the real SaveManager, native `NPC.Die`, `GameItemProto.CreateNew`
and corpse factory/component persistence, plus a separate-process persisted reader. World,
catalogue and cell hosts are controlled. It qualifies creation ordering, rollback, activation
quarantine and early-death correlation only. Full `GameItem.Delete`, timed decay, foreign
goods/container/occupant evacuation, actual `Character.Quit`, expiry death/dissipation,
installed-world command sessions and full N14/N15/N16 remain not run.

## Canonical NPC archival boundary

The world's optional `CharacterArchives` service supplies historical readers; missing-victim
display in a world without that service does not create a database connection.
`ICharacterArchiveService.Find` reads immutable attribution without constructing a character,
body, controller or AI. `TryArchiveNpc` requires a native accountless NPC, one exact autonomous
character/body ownership pair, a non-permanent retiring lifecycle, persisted native death and
its post-death correlation. A pre-death notification provides no removal authority. Permanent
entities, borrowed identities, secondary instances, projections and mismatched claims refuse
without acquiring compaction authority.

The boundary uses a fresh serializable FMDB transaction and restores the caller's context.
It holds possessions, generated/remains rows, effects, additional forms, live instances,
foreign form/source/retirement ownership, anchored dependants, polymorphic item custody,
lodged items, infections and unclassified foreign-key relationships. Runtime group roles,
guards, followers, mounts, position/seen-target links, combat, movement, additional loaded
copies and connected player focus also hold the graph. Refusals within the proven lifecycle
persist a bounded diagnostic and leave physical state, saves and foreign goods untouched.

Metadata-driven FK checks allow removal only of audited directly owned NPC/body state.
Canonical command logs, finalized crime attribution and external wound-origin IDs
survive. Every unfinished criminal crime holds, including stale crimes that a witness could
report again. Writing (including composite/graffiti author caches), drawings,
hospital/bank/estate/clan/track and other unsupported relationships hold until their own
history/runtime adapters are reviewed. Inventory reachability does not establish ownership.

The bounded serialized scan covers definitions, effects, data/value fields, route motion,
computer process state/result/wait arguments, tattoos, injury extras, procedure parameters,
employment payload/arguments, strategy data and land-detail JSON. XML leaf/attribute values
and decoded JSON values/keys are checked for canonical/body/instance/wound references;
malformed or oversized payloads hold. Numeric namespace collisions may conservatively hold.
This is explicit support for the audited persisted surfaces, not proof about arbitrary new
extension encodings. New identity-bearing surfaces require classification before enabling
compaction for them.

Unmapped numeric ID properties are checked as well; non-receipt references without a
classified FK/key hold rather than relying on their field name to imply ownership.
The instance's generated `EmbodiedBodyId` and `PrimaryCharacterId` uniqueness keys are
classified only when every matching row is already in the audited removal set.

On success, `Characters` retains its canonical ID, bounded identity fields and dead state,
sets `IsArchived`, clears its physical body pointer and active outfit/position data, and gains
a bounded `CharacterArchives` record with original body, lifecycle, UTC time, name,
descriptions, wound summary and provenance. Only audited heavy rows and the empty body are
removed. The body FK is restrictive; a check permits a missing body only for archived rows.
The canonical archive FK prevents deleting historical identity through a cascade.

Runtime save abort (including duplicate and delayed queue entries), heartbeat/effect/scheduler release and final removal from active/cached
actor and body roots occur after commit. Archived constructors/loaders refuse materialization;
fresh save guards plus the canonical marker's concurrency token protect stale tracked writers.
NPC death disposes its original controller after persisted death; archival also disposes a
controller attached by a dead-graph reload. Both observer directions, context, actor and output
perceiver references are detached, so retained monitors cannot expose or root the heavy actor.
Crime's criminal cache is weak and its history display uses the immutable archive when needed.
Already-archived retries must match the exact receipt and missing heavy graph. Lifecycle
completion requires that same canonical/archive/body proof, rather than treating a tombstone
as a live owned actor or inferring death from an absent row.

The generated schema migration, designer, model snapshot and maintained blank-database dump
form one release unit. Downgrade is safe only before archives exist: removed bodies cannot be
recreated by the generated `Down` migration. Recover a pre-compaction backup when reverting
an already-used archival schema.

Phase3A supplies the shared persistence and safe body-retirement boundary for the approved
Armageddon completion plan. Phase3B2A attaches explicit `createnpc` creation and death
correlation to it; `createitem`, corpse animation, projections and destructive retirement
still need their native adapters and acceptance. Legacy spell creation behavior is retained.

### Staff corpse maintenance

`debug cleanupcorpses` retains ordinary dead NPC identities, bodies and history when no
unique creation-proven retiring lifecycle exists. It reports each retained identity and
its reason. Permanent, active, uncorrelated, ambiguous, foreign-body, projection and
topology cases do not reach archival. An exact eligible NPC is passed to the existing
archive service, whose reference and transaction guards remain authoritative. A service
hold retains the graph and diagnostic; no lifecycle or death evidence is fabricated.
The summary distinguishes archived, retained and failed attempts. A post-commit runtime
failure is reported as failed, without claiming that the physical graph survived.

Maintenance never removes a canonical NPC's body alone. The restrictive body FK and
body-or-archive check remain unchanged. Both corpse cleanup and the active orphan report
track temporary offline PCs before registration, clean them in `finally`, attempt every
quit even if one fails, and restore statistics recording. Partial preload failures use
the same cleanup. Destructive orphan-item cleanup remains disabled. This is a manual
staff maintenance boundary, not automatic summon creation/remains/evacuation integration.

## Creation and ownership

`ISpellOwnedLifecycleStore` exposes immutable lifecycle origin and exact created entities.
Origin includes a stable key, source spell and grade, creator **canonical** identity, family,
created UTC time, absolute optional deadline, permanent/temporary-cleanup/death-on-expiry
mode and bounded provenance. Control duration and authority are separate from lifetime.

`SpellOwnedLifecycleStore.Create` executes its row factory in a fresh independent FMDB
session and serializable transaction. Register only EF `Added` rows before the atomic save.
The entity rows and ownership journal commit together; world insertion, on-load programs,
event subscriptions and runtime activation belong **after** the method returns successfully.
Callbacks must not save or expose runtime objects, mutate existing rows, or perform external
side effects. A duplicate lifecycle key refuses creation before calling the factory.
Rollback disposes the fresh context and restores an isolated caller's untouched session.
Write-suppressed callers cannot create, transition or retire bodies through an independent
scope. Read-only lookups inherit suppression and restore the caller's context.

Claims distinguish generated possessions from created entities and never follow inventory
or other references to infer ownership. The database key `(kind, entityId)` permits one
lifecycle claim per exact entity. Existing players, builder NPCs, borrowed bodies and primary
instances cannot be claimed through this creation API. An autonomous-character claim needs
a newly added NPC row and an accountless new identity. A projection owns only its new
secondary instance and explicitly created physical rows; its canonical identity stays intact.

One actor is permitted per lifecycle so early death cannot cancel the lifetime of its living
siblings. A multi-creature cast creates one lifecycle per actor under the shared casting
operation's provenance. This is a journal granularity rule, not a spell/summon count limit.
Cells, exits, items and bodies may share a lifecycle where their cleanup dependency is shared.

## Durable transitions

Active -> Retiring records the first reason before an adapter begins cleanup. Absolute expiry
cannot fire early; dispel, dismissal, capability loss and configured logout can retire earlier.
Permanent creation has no expiry and releases to ordinary durable existence at completion.
Repeated transitions retain the original decision. Expected versions and the EF concurrency
token reject stale writers; transition times must be nondecreasing UTC.

`ObserveDeath` belongs after persisted native death and remains creation, not the current
pre-death `OnDeath` notification. It requires the claimed actor's persisted dead state and
correlates optional remains to its exact body. Active/Retiring -> RemainsPending prevents
later expiry from requesting a second death. Missing actor rows alone do not prove death.
Death-on-expiry cannot complete without that correlation. A pending death intent is not an
instruction to call `Die` repeatedly: the future native adapter must reconcile actual native
life/death/remains state before doing anything, including recovery after a crash.

`Complete` refuses temporary lifecycles with remaining owned rows or dependent remains.
`Hold` retains a bounded diagnostic and retryable state. `Pending(now, limit)` returns overdue
Active plus Retiring/RemainsPending entries in bounded batches, including records whose
caster or source spell later disappears. It owns no per-character timer and replays no cast.
No cleanup worker or heartbeat registration is introduced before its native adapter is ready.

## Retired physical bodies

`Character.TryCleanupRetiredBody` refuses any live/runtime form, backup, instance or physical
remains reference. A body containing runtime items is refused intact before any database
operation. Persistent inventory, implant, prosthetic and lodged-item joins also block removal;
an adapter must explicitly conserve/rehome foreign goods before calling the boundary.

A fresh serializable transaction validates current/other canonical character pointers,
persisted instance rows, form/source ownership, durable ordinary retirement and
creation-proven retiring ownership. The
method does not infer ownership from `Body.Actor`, which secondary retirement can reassign.
It scans persisted component and character/instance/body/item effect XML for physical-body
reference fields, including unloaded corpses and secondary effects. Anatomy/prototype IDs
such as `Bodypart` and `OverridenBodypart` are distinct. Malformed XML blocks deletion.
These are conservative checks of known persisted surfaces, not a claim that arbitrary new
component or Prog references have been exhaustively inventoried.

Only after validation does it remove allowed form/source metadata and the empty retired body
in one transaction. Save-abort and scheduler/world removal happen after the commit. No
inventory item is deleted by this method. The canonical owner's current body and identity
survive. Refused cleanup leaves inventory and scheduled save participation untouched.
Staff dormant-form deletion now requires possessions to be moved out first.

Ordinary backup death records exact body/character/UTC retirement provenance before removing
the runtime form. The next form save validates persisted ownership and commits
`CharacterBodyRetirements` together with canonical-body movement and form/source pruning.
A failed save leaves those persisted mappings and pointers intact. This ordinary provenance
does not create a spell lifecycle or claim. Borrowed bodies and the current body cannot be
registered merely because their `Body.Actor` points to the character.

After restart, remains loaders read their exact positive body ID separately from cleanup
authorization. A legacy body can therefore supply anatomy and inventory even when its
form/source mappings were pruned before retirement records existed. The read rejects
known foreign ownership and non-final corpse references to current or embodied bodies,
including cached bodies. Severed parts may still reference their living owner's exact
body, and final-death corpses may reference their owner's exact canonical body. Reading
does not create a form, retirement record or spell claim. Positive IDs never redirect to
a different body. The durable retirement record is still required for ordinary cleanup
and is consumed with successful validated deletion; possessions and other references
keep both body and provenance intact. Legacy reads do not backfill missing authority.

If the body or owner cannot be resolved safely, remains keep their saved ID and present
an unidentifiable corpse or part. Missing anatomy contributes zero weight, buoyancy and
edible mass; a severed part's known contained items still contribute and release normally.
Corpse illumination and wound/damage delegates tolerate the missing body. Butchery,
skinning, transplantation, replantation and resurrection refuse unresolved anatomy;
staff final-corpse cleanup skips it. Corpse-target surgery also requires a resolved owner
whose body matches the corpse's exact body, so it cannot operate on a surviving form.
Release does not touch a survivor's current inventory
or authorize deletion of an unproven body.

The runtime boundary also preserves unresolved remains during exposure registration and
refresh, scavenger eating, gifting, staff body targeting, wound inspection and morgue
recovery. Anatomy-dependent operations refuse before consumption or transfers; ordinary
item exposure continues. Legacy character operations require a final corpse with a
matching resolved current body. Morgue refusal leaves custody, effects, estate state and
the assigned recovery report unchanged. These guards create no retirement or spell proof.

## Remaining acceptance and integration

Full summon retirement still needs additional native creation/materialization adapters,
corpse-release integration, foreign goods/container/occupant evacuation, safe destination
fallback, AI/subscription release, count/control policy and economy/salvage guards. Current
ordinary corpse deletion still deletes external inventory; this foundation does not qualify
temporary summon corpse behavior or change that path.

Canonical NPC deletion remains unsafe: crime/log history and serialized corpse, witness,
dub and effect identities can outlive physical graphs. The archival boundary above keeps
canonical identity and restricts body deletion; no maintenance caller may bypass it with
a body-only or cascading canonical delete. Additional creation/remains/restart integration and
historical author adapters remain pending. A retained full Character/Body graph is a
conservative hold, not final high-volume retirement acceptance.
The lightweight lifecycle/claim journal deliberately has no entity FKs and does not authorize
canonical deletion. Bounded journal retention/archival is a later explicit policy; these
receipt tables currently retain ownership evidence rather than purging it automatically.

Native N14-N16 remain unqualified until actual NPC death/remains/decay/restart and repeated
heavy actor/AI/item/timer/subscription steady-state scenarios pass. The Phase3A harness
qualifies the durable protocol and real Character/Body retirement boundary on disposable
MySQL, with a separate process for restart reconciliation; it does not substitute for those
full gameplay scenarios.

The ordinary backup regression fixture separately exercises the native backup transfer
helper, an atomic form/remains save failure and retry, a separate-process real corpse reload
without a retired-body preload, database possession/reference refusal and the real corpse
component's final release callback. Its item host and world services are controlled fixtures;
the persisted foreign inventory join is deliberately unloaded. It does not qualify a full
command session, timed decay, native NPC death or the later summon adapters.
