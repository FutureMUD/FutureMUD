# Spell-owned lifecycle foundation

Phase3A supplies the shared persistence and safe body-retirement boundary for the approved
Armageddon completion plan. It does not yet attach this contract to `createnpc`, `createitem`,
corpse animation, projections or the game heartbeat. Their native adapters and acceptance
remain later dependency stages. Existing spell creation behavior is unchanged in this slice.

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

Full summon retirement still needs durable native creation/materialization, post-death and
corpse-release integration, foreign goods/container/occupant evacuation, safe destination
fallback, AI/subscription release, count/control policy and economy/salvage guards. Current
ordinary corpse deletion still deletes external inventory; this foundation does not qualify
temporary summon corpse behavior or change that path.

Canonical NPC deletion is unsafe today: Bodies -> Characters and Characters -> Crimes/logs
cascade, while corpse, witness, dub and effect identities can remain serialized outside FKs.
Do not use cascading deletes for temporary NPCs. The next slice must provide bounded
compaction/archival accommodation for the required primary-body FK, meaningful attribution
and non-loading tombstones. A retained full Character/Body graph is not final acceptance.
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
