# Spell-owned lifecycle foundation

Phase3B1 adds the conservative canonical NPC archival boundary described below. Phase3B2A
connects explicit `createnpc` lifecycle provenance to native simple-template construction and
persisted death/remains recovery. Phase3B2B connects bounded simple-NPC retirement to
native death, ordinary corpse deletion/decay, conserved foreign custody and canonical
archival. Complete installed-world N14-N16 and the other creation adapters remain outstanding.

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

Configured lifecycle NPC casting now prepares an invocation-local token for each recipient.
Known invalid spawn rooms, route coordinates and world identity refuse before payment. The
token retains the approved native template identity/revision/definition, lifecycle service,
caster body/instance and raw room/layer/route/movement frame. Fresh casting copies reuse those
tokens. Callback-free structural fences run after all live selection confirmations so a later
confirmation cannot invalidate an earlier NPC admission and still take payment.

Temporary lifetime preflight uses the casting copy's actual captured expression. Only formulas
whose parsed inputs are fixed grade, power and mastery, with no trait parameters or functions,
are evaluated before the roll. Nonpositive, nonfinite, strict-evaluation failures and unrepresentable
UTC deadlines refuse before payment/reservation/proficiency. Outcome, variable/trait and function
formulas remain deferred. All formulas retain their existing post-roll evaluation and native
trait-read timing; the pure precheck does not substitute a cached lifetime. Spawn position is
resolved again at application, preserving clock-based effective movement rather than freezing
a prepared coordinate. Legacy creation and permanent lifetimes retain their existing policy.
The [NPC prepayment checkpoint](Armageddon_Npc_Prepayment_Checkpoint_20261009.json) records 152
focused managed passes and real payment/refusal/creation cases in the owned restored world.
The [archival continuation](Armageddon_Npc_Archival_Checkpoint_20261009.json) adds verified Agriculture/Armour
codecs and registration-only Body shutdown, retaining its historical 5,980 Core passes.
The [stock armour repair checkpoint](Armageddon_Npc_Armour_Repair_Checkpoint_20261009.json)
fixes both seeded chopping defaults and qualifies their exact owned-world correction with
1,613 Seeder and 102 focused Core passes plus native transaction/preservation checks.
The [AI reference checkpoint](Armageddon_Npc_AI_Reference_Checkpoint_20261009.json) adds strict
row-Type-aware Judge/Mount/Animal/Monster classification, with 6,038 full Core passes.
The unchanged native gate clears the AI hold and next holds on
`AutobuilderAreaTemplate.Definition`. Both heavy graphs remain; physical release and the
completed cold retry remain unqualified. Exact observed Autobuilder contracts are the
next bounded investigation; unknown/malformed holds and all original retirement assertions
remain enabled. Previous failed receipts remain historical evidence.
Earlier creation/retirement receipts do not establish full current lifecycle readiness.

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
recast, delete rows or replay callbacks. Persisted death recovery itself does not evacuate
custody; the separate retirement pass below consumes that correlation.

Native acceptance uses real maintained-snapshot MySQL, `SimpleNPCTemplate`, production new
NPC/Body/primary insertion, the real SaveManager, native `NPC.Die`, `GameItemProto.CreateNew`
and corpse factory/component persistence, plus a separate-process persisted reader. World,
catalogue and cell hosts are controlled. It qualifies creation ordering, rollback, activation
quarantine and early-death correlation. The retirement fixture additionally exercises
actual `GameItem.Delete`, positive minute decay and scheduled morph-to-nothing,
`Character.Quit`, expiry policies, nested foreign custody and separate-process retries.
Installed-world command sessions, other topology adapters and full N14/N15/N16 remain pending.

## Bounded simple-NPC retirement and foreign custody

Startup and the minute heartbeat call `ReconcileRetirements` after death recovery. Each
UTC pass requires writable authority before loading graphs or invoking native callbacks,
accepts a limit of 1-1000 and rotates an autonomous-character ID cursor. A stable held
row does not repeatedly occupy the first slot or require diagnostic/version churn to
let a later eligible row progress. Only one exact accountless native NPC/body ownership
pair is admitted. Activation-pending, ambiguous and unsupported claims retain their graph.

Expiry first commits retirement intent. Connected controllers, runtime relationships,
dependent instances and bodyguard references hold before native death or evacuation.
`temporarycleanup` conserves foreign goods before `Die` and suppresses the ordinary
corpse, including early native death. `deathonexpiry` calls native death once and keeps
its configured ordinary corpse and exact body until decay/removal releases them.
Original expiry never kills an already-dead NPC again or removes a retained corpse.
Permanent creations remain ordinary durable NPCs without an expiry deadline.

The custody census walks ordinary inventory, nested containers, installed locks, belts,
firearm attachments and severed-part contents by reference and persisted ID. It refuses
cycles, duplicate IDs, unregistered/deleted items, foreign worlds, connected/projection
topology, unsettled structural edits and any unloaded persisted possession. Body-root
implants, prosthetics and lodged items require separate detachment adapters. Census
reachability proves what must be conserved, never that those items belong to the spell.

Evacuation requires a validated same-world persisted destination. Missing destinations
retain the live body or remains and all custody; no automatic fallback is invented.
A serializable independent transaction revalidates the exact lifecycle/version/body,
moves only foreign roots through native `Take`/`Get`/spatial insertion, rechecks callbacks,
saves the empty body and roots, and commits exactly one cell join per root. Children,
locks, attachments, legal ownership and foreign item identities are conserved without
recursive deletion or subtree saves. Before transfer, verified native adapters capture
body inventory and authoritative body position, item custody/position, structural
component fields and exact enclosing location membership. Failed provider writes restore
that runtime graph before ordinary gameplay or save flushing can continue. Every captured
full body/item/component save journals pending section flags before consuming them, including
callback saves and direct item `SaveMagic` calls. Rollback rearms the thirteen body sections,
item resources/surface/effects/hooks/position and whole-component definitions, preserving the
current unflushed resource, stamina and other live values. Needs-save batching is restored
through raw metadata and an update revision; later live batching progress is retained. The
journal unwinds independently in reverse order and explicitly requeues missing participants
without aborting queues. Global flush, direct initialisation and lazy queue draining are refused
inside the custody transaction before touching unrelated participants. Compensation
attempts every restore even if explicit provider rollback fails; post-commit activation
is outside the compensation catch. A restart reloads the original joins ordinarily.
Post-callback proof also compares exact containment pointers and typed relations by
component, attachment slot and wound. The same item IDs cannot authorize a child moving
between containers or between installed locks and contents. Original relations remain
in the same-process retry snapshot, and native fields and structural flags are restored on refusal,
including callbacks that explicitly saved components and cleared their flags before rollback.
The synchronous transfer scope refuses uncaptured items, bodies or cell destinations,
destructive child deletion, merging, splitting and quantity/currency mutation before their
native side effects. Firearm/severed-part structural custody, effects, wounds and position
relationships without a verified rollback adapter hold before any transfer. The controlled
native cell fixture supplies an explicit membership adapter; production cells capture
their native cell/room/zone/shard collections while preserving unrelated pending work.

Owned corpse deletion checks conservation both before and after `OnDeleted`. Observers
are attempted once; reentrant deletion returns, and an observer exception latches a
hold without replaying its possibly partial side effects. A failed second check keeps
the corpse, live event subscriptions and minute decay. Event release, destructive flags,
health tick shutdown and component deletion occur only after both checks pass.
Before observers or runtime teardown, removal admission persists the exact correlated
remains ID with `RemainsRemovalRequestedUtc`. Separate attempted/completed observer UTC
markers prevent replay after a same-process or restart retry. An incomplete observer
attempt remains held for review. A final `GameItems` DELETE refusal retains the typed
removal intent even after native teardown has stopped decay or a diagnostic changes;
bounded reconciliation reloads the exact remains and resumes removal without another
death or completed notification. The generated EF 9 migration adds three nullable
`datetime(6)` columns; existing journal rows retain null progress until removal admission.
`MorphTargetId` exposes positive replacement targets so owned remains refuse a replacement
morph before new-item construction, output, transfer or activation. Morph-to-nothing uses
the conserved ordinary deletion path; replacement transfer remains an explicit adapter gap.

After persisted remains are gone and foreign custody is committed, unadapted effects or
remaining runtime dependants hold. Eligible graphs run actual `Quit`, the existing
canonical archive boundary and journal completion. If completion fails after archival,
a retry verifies the archive's exact original body/lifecycle, releases any cached graph
and completes without another death, corpse, item creation or custody transfer.

Disposable native acceptance qualifies early configured death/remains across original
expiry, a separate-process dead-body/item reload, positive decay and scheduled deletion,
plus separate default dissipation. It also covers all three policies on the same real
simple template, read-only refusal, controller/destination holds, callback conservation,
replacement refusal, provider rollback and post-archive completion failure/retry.
Same-ID reparenting with explicit native component saves is refused before custody commit
or death; a separate owned process reloads and conserves the original persisted topology.
The corrective fixture performs ordinary save flushing before retry or inspection,
without manually dropping item/component save queues. It also verifies outside room-item
acquisition, captured child deletion, partial stack splitting, native body position and
provider rollback failure, plus final corpse DELETE refusal in-process and after restart.
Sixteen repeated native cycles conserve the heavy-row, runtime-root, scheduler and
heartbeat census while retaining explained permanent identity and canonical history.
World/catalogue/cell hosts are controlled; high-volume N16, every-cycle restart,
installed stock commands, occupied hosts and cross-body detachment remain unqualified.

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
report again. Writing's `AuthorId`/`TrueAuthorId` and drawing's `AuthorId` are historical
canonical references: their rows survive compaction. Hospital/bank/estate/clan/track
and other unsupported relationships hold until their own
history/runtime adapters are reviewed. Inventory reachability does not establish ownership.

Simple and composite/graffiti writing and drawings retain raw canonical author IDs and
weak live caches. `Author`/`TrueAuthor` return null for archived identities;
`ArchivedAuthor`/`ArchivedTrueAuthor` supply immutable attribution without loading a physical
actor. Every live-author lookup checks the durable archive first, including when another
process commits archival before this host releases its cached actor. Copying preserves
IDs and content without resolving the author; ordinary insertion/save retains those IDs.
Historical artifacts cannot keep a physical Character/Body/controller graph alive.
Staff lists, author-ID filters, `character.writings` and book prototype descriptions use
canonical IDs or archived display names. Archived handwriting retains its handwriting
header. Full-name filters use the retained canonical full name independently of the
nickname-inclusive archive display name. Anonymous/printed writing keeps its configured
provenance. Accountless live NPC
authors display without requiring an account.

The bounded serialized scan covers definitions, effects, data/value fields, route motion,
computer process state/result/wait arguments, tattoos, injury extras, procedure parameters,
employment payload/arguments, strategy data and land-detail JSON. XML leaf/attribute values
and decoded JSON values/keys are checked for canonical/body/instance/wound references;
malformed or oversized payloads hold. Numeric namespace collisions may conservatively hold.
This is explicit support for the audited persisted surfaces, not proof about arbitrary new
extension encodings. New identity-bearing surfaces require classification before enabling
compaction for them.

The exact `AgricultureOperation.Definition`, `AgricultureCropDefinition.Definition` and
`AgricultureFieldProfile.Definition` contracts have schema-aware classification.
Recognized Operation/Crop/Profile XML contains tuning
scalars and static score/material/tag/planting-window names, so coincident character, body,
instance or wound numbers in those fields are not physical references. Empty legacy defaults
are supported. Unknown roots, nodes, attributes, namespaces, duplicate singleton containers,
mixed text and invalid scalar values hold even without a matching ID. Other Agriculture
columns retain the generic scan; row-count and payload-size limits remain authoritative.

`ArmourType.Definition` also classifies its exact current codec and seeded `ArmourType`
root alongside the runtime writer's `Definition` root: six scalar expression
maps and damage/severity enum transformations. Formula text is parsed without execution;
only the existing scalar combat inputs and supported expression functions are recognized.
Unknown shape, invalid enums/formulas, duplicate keys or additional inputs retain a hold.
Damage-type values such as 10/11 and numeric formula literals are not physical identities.
An armour hold reports the rejected section, integer damage type and validation category;
it does not expose formula contents or execute them to diagnose the failure.

The observed `ArtificialIntelligence.Definition` codecs for exact row `Type` values
`Judge`, `Mount`, `Animal` and `Monster` also have schema-aware classification. The bounded
query keeps each discriminator paired with its definition; the common `Definition` root
alone cannot select a codec. These configurations contain static prog, race, craft,
calendar and celestial references, tuning values, dice expressions and narrative text.
Live actors are supplied at runtime. Consequently, matching numbers in those verified
fields do not retain an otherwise qualified physical graph. Judge supports its loader's
legacy prog-name fields; Animal/Monster preserve absent optional sections and legacy
Water `enabled` configuration. Current version 1 and unversioned legacy containers are
supported where the loader supports them. Required Judge/Mount fields, leaf-only text,
finite numbers, defined enums, parse-only dice syntax and known nested/list contracts are
validated. Unknown structure, malformed values and unsupported versions hold, including
payloads without matching IDs. Other AI types retain the generic reference scan. Actor
effects, runtime dependants, row-count and payload-size limits remain authoritative;
this classification neither executes progs nor asserts that an AI's external static
definitions exist or that its gameplay configuration is ready.

Physical holds identify the first retained condition, distinguishing runtime effects from
persisted or unrecognized effect XML. NPC Quit and post-archive body release unregister
health heartbeats without generating fresh damage effects during teardown. Existing
effects, possessions, forms, body users and owned remains still hold archival; shutdown
does not clear those guards.

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
correlation to it; Phase3B2B supplies the simple-NPC retirement adapter above.
Phase3C1 supplies the bounded plain-item `createitem` adapter described below. Corpse
animation, projections and other topology still need their native adapters and acceptance.
Legacy spell creation behavior is retained.

### Created leaf items

`createitem` optionally persists a version-one lifecycle definition. Builders select
`effect <n> lifecycle permanent` or `temporarycleanup`, set `family <name>` and, for timed
output, `lifetime <expression>` in real seconds. `permanent <grade> <prototype>` selects a
distinct permanent prototype at one exact grade from 1 through 7; `permanent none` clears
that override. `lifecycle legacy` retains existing item creation. Malformed stored lifecycle
XML remains intact for repair and refuses admission.

The adapter admits one current, loadable native item with no skin, load string, morph,
on-load program or default item hook. Its component graph must contain `Holdable` and may
also contain the native `MeleeWeapon` and `Salvageable` components. Wield programs and other
component families require a further adapter. A finite positive lifetime is sampled once
from the selected-grade casting copy before payment and reused for the absolute UTC
deadline. Historical event delays are not implicitly converted to seconds.

Private item and component inserts, quality and the exact creation claim commit together
before world publication or placement. Caller write suppression is authoritative; creation
does not flush unrelated dirty state. An origin cannot replay creation. Temporary output
retains its immutable origin and deadline through ordinary custody, equipment and legal
title changes. Permanent output completes its lifecycle after activation and has no expiry.

Temporary output refuses copy, merge, split, morph/replacement, prototype update and
conversion into ordinary crafting inputs, casting material, salvage products, shop stock,
sale proceeds or auction lots. Value guards also inspect temporary items inside or attached
to an otherwise ordinary host. These refusals occur before payment or input reservation.
The Phase3C1 simple adapter deliberately refuses unsupported conversions. The bounded
food consumption, liquid mixing and worn-light adapters are described in the Phase3C2
section below; installed stock profiles and their full acceptance remain pending.

Exact temporary leaf deletion commits custodian changes and the item row deletion in one
private transaction. A loaded native container or sheath, native body, or location with a
custody rollback adapter is required. Typed snapshots and the save journal restore runtime
membership after a detach callback or provider failure. Dependencies and raw custodian
membership are checked again after detachment, so callback-introduced foreign material or
same-body reacquisition prevents deletion. Unknown effects, hooks, attachments, lodged
goods, implants, prostheses or cold persisted custody retain an explained retirement hold.
Loading the exact persisted custodian permits retry without resetting the deadline.

`Die()` performs the first removal check before native destruction flags or observers. It
rechecks after death observers, and a live held attempt invokes those observers only once.
This once-only state is an in-memory guard; manual death callback behavior across a restart
is not qualified by this slice. After commit, reconciliation retries runtime release from
the exact durable intent. Post-payment publication uncertainty remains a casting review
case, with no refund or automatic replay. Permanent post-placement recovery is not qualified.

Controlled native acceptance exercises a paid grade-three timed melee weapon and a distinct
paid grade-seven permanent staff, native holding/wielding, callback holds, actual SQL delete
failure and a second-process custody reload before retry. These are replacement fixtures,
not installed Armageddon stock, combat statistics or elemental payload acceptance. Food,
water/wine capacity and mixing, Hovering Light and Storm Spear have subsequent bounded
checkpoints below; the remaining item families remain separate completion requirements.

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
The typed corpse-animation adapter is the narrow exception to the existing-row rule:
it suspends only exact dead/stasis holders of the borrowed body and removes the exact
source cell join inside that same transaction. It snapshots those permitted changes
and refuses subsequent mutation or any other modified/deleted row. Ownership still
covers only the newly added secondary instance.
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

### Durable final-corpse animation

`animatecorpse lifecycle durable` opts into `ISpellOwnedCorpseAnimationService`;
existing definitions default to `legacy`. Builders supply a family and a positive,
finite lifetime in real seconds, evaluated with the selected grade before payment.
Invalid schema, unavailable/unready AIs, excessive persisted echoes and unsuitable
corpse custody refuse before payment. Generic lifetimes are builder-authored real
seconds; historical EVENT units receive no implicit conversion. The stock Raise
Servitor profile below uses separately recovered affect timing.

This adapter currently accepts final-death corpses whose exact canonical owner is
still dead in runtime and persistence. It refuses abandoned/nonfinal bodies pending
a cold-load adapter. Runtime and saved ownership checks reject foreign body pointers,
instances, forms, sources and retirement records, external attachments and route
positions. Source identity, body, corpse and carried goods are borrowed, never owned.
Only one active animation may borrow a particular corpse; there is no universal
one-summon cap. Existing AI metadata and shared canonical resources remain native.

Expiry, dispel, dismissal, actor death and actor Quit converge on durable retirement.
The durable animation child schedules its exact journal deadline independently of the
ordinary spell duration. At the ordinary duration, the parent expires ordinary siblings
but retains independently timed children as a save/dispel wrapper. A longer ordinary
sibling retains its original duration after animation expiry. Explicit dispel still
removes the whole matched parent immediately, including after its ordinary duration.
Legacy animation has no independent deadline and retains parent-controlled expiry.
The child installs its timer before room-output callbacks, so a presentation failure
cannot leave a partially applied animation without its deadline schedule.
An exact serializable transaction restores the source corpse's cell/layer and deletes
only its created secondary row. A bounded journal checkpoint retains the committed
destination independently of deferred cell saves. Runtime teardown and placement
guard custody callbacks; unavailable dependencies or callback/provider failures hold
for retry. Selected-AI heartbeat/event execution stops while retirement is held.
The borrowed corpse is never recorded as owned death remains. Its deletion, death,
morph, prototype replacement and copying are refused while borrowed. Body and foreign
inventory rows survive. Collapse/restore output is attempted at most once after the
restoration commit; a crash or callback failure can omit that presentation.

The active actor intentionally collapses on restart. Loading keeps its exact saved row
for the recovery worker without rematerializing its AI. Recovery needs no saved child
effect, does not reset the absolute deadline or replay the cast, and restores the same
corpse. Lost final destinations use a safe loaded fallback or retain recoverable holding.

A saved durable animation child is inert during corpse construction and early login:
it neither resolves its original character nor attempts restoration inline. Its deferred
recovery runs only after boot completes and its exact owner is registered in the item
catalogue. This also removes stale child/empty-parent XML after the restoration journal
has already completed, without moving the restored corpse. Transient unregistered reads
drop their timer after boot; a later login can rearm it. Initial provider lookup failures
retain a bounded recovery retry. Fresh-cast deadline scheduling and legacy animation
remain separate from this saved-effect recovery path.

Corpse morph schedules retain their saved duration on cold login. A borrowed corpse
without a world-item placement can instead load lazily during restoration. That path
starts its retained CachedMorphTime after exact placement and animation-effect removal,
without repeating component/effect Login. Successful start clears the cache, so a held
completion retry keeps the running deadline. Native MorphSaving marks elapsed progress
for normal saves every thirty seconds. Checkpoint22 qualifies four fresh actual
production-loader cases with bounded fixture catalogues; full-server boot remains separate. Scheduler diagnostic
labels use the item ID without rendering a corpse description, so their creation does
not resolve the canonical character during the blocked boot phase. Saved paid active
parents recover as Logout before their spell deadline or Expiry at/after it; recovery
preserves the independent morph deadline and does not rematerialise the scripted AI.

The focused scheduler correction uses actual paid casts and the native EffectScheduler
with 30/180/300-second ordinary durations, a 180-second animation and an ordinary glow
sibling. Early dispel is checked both before and after ordinary parent expiry. The
duplicate activation check is sequential; it does not qualify concurrent activation
or cleanup races. Historical D1 logs retain their original marker text; the correction
receipt supersedes that marker's former concurrency wording.

Phase3D1 native evidence uses real corpse/body/item/instance, selected CombatEnd AI,
compiled resource FutureProg, paid casting, dispel, death, provider triggers, ordinary
saves and fresh processes. Anatomy and room catalogues are controlled, and the fixture
character uses the inherited specialized Save override. This is not installed Raise
Servitor, a commandable/combat AI profile, PC corpse qualification, nonfinal support,
arbitrary callback safety, simultaneous-host recovery or full N14-N16 acceptance.

#### Stock Raise Servitor

`magic spell edit new stock raise-servitor <school> <casting trait> <resource>`
creates an editable spell, two native AI definitions and compiled support progs
atomically. Quote names containing spaces. Builders select existing world definitions
and separately configure capability membership and acquisition. Duplicate creation in
the same school refuses; this command is not the complete Armageddon installer.

Selected grade 1–7 maps explicitly to historical level. Animation lasts
`600*(grade+10)-1` real seconds and command authority lasts `600*(grade+6)-1`.
These follow `codedump.c` fresh affects: `time(NULL) + RT_ZAL_HOUR*duration - 1`,
with `RT_ZAL_HOUR = 600`. They supersede the earlier `60*g` proposal/fixture for this
stock profile. Neither formula converts EVENT units. Source windows and version/hash
qualifications are retained in `Armageddon_Stage3D1Stock_HistoricalTiming_ReadReceipt.json`
beside the execution workspace; the stock verification receipt records its fingerprint.

Optional durable-animation builder settings are `control <seconds expression>|off`,
`controlprog <boolean (character, item) prog>` and `followcaster true|false`.
Control must be finite, positive and no longer than animation; its formula and
eligibility are prepared before payment. Legacy defaults remain unchanged.

The stock eligibility prog excludes terrain names Silt and Shallows, case-insensitively.
This is an editable name-based adaptation; worlds using other names must change the
prog. Animation and native following still occur without a command grant there.
`cancommandanimation(animation, commander)` checks the creator's canonical identity,
exact owned secondary instance, Active journal and both absolute UTC deadlines.
Control expiry denies further orders while leaving animation and its independent
follower link intact. No runtime charm effect is installed.

Commandable AI executes resolved included commands through the ordinary command tree
with the actor's current state, permissions and native checks. The stock allowlist is
`follow`, `hit`, `flee`, `stop`, `stand`, `sit`, `kneel`, `emote`, `get`, `drop`, `give`,
`wear`, `remove`, `wield` and `unwield`. Guard and rescue are excluded pending repair
of existing native ownership/removal behavior. CombatEnd AI accepts truce and target
incapacitation; the profile does not reproduce historical autonomous aggression.

Retirement releases following and combat, clears movement commands, cancels both
schedulers and cleans selected-action subscriptions even while restoration is held.
Ordinary selected-action consumption also unsubscribes it from the continuing combat.
Only the secondary is owned: the canonical identity, same corpse/body and foreign gear
remain borrowed. Restart still collapses the active actor, preserving deadlines without
replaying creation or AI activation.

The stock native packet covers paid builder-created casts, real cell insertion,
extraction and saving, actual follow/hit orders, native selected inventory action in
combat, separate deadlines, terrain eligibility, database-refused retirement/retry and
fresh durable-control readers. World/anatomy catalogues and the inherited specialized
caster Save remain controlled. Damage/defence delivery, PC-corpse stock casts, arbitrary
callbacks, full server boot/Telnet, concurrency and complete N14–N16 remain unqualified.
See [the stock verification receipt](Armageddon_Stage3D1Stock_RaiseServitor_Verification.json).

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
generated-possession and occupant/topology conservation, qualified safe destinations,
count/control policy and economy/salvage guards. The simple-NPC adapter conserves proven
foreign custody before owned corpse deletion. Ordinary corpses without removal-authorized
spell ownership retain their existing component deletion behavior.
The approved brief permits a configured safe fallback or recoverable holding with
diagnostics when no destination is safe; automatic fallback selection is not mandatory.

Canonical NPC deletion remains unsafe: crime/log history and serialized corpse, witness,
dub and effect identities can outlive physical graphs. The archival boundary above keeps
canonical identity and restricts body deletion; no maintenance caller may bypass it with
a body-only or cascading canonical delete. Writing/drawing authorship has a bounded native
retirement/copy/restart adapter; other creation/remains/restart integration and unsupported
historical-reference adapters remain pending. A retained full Character/Body graph is a
conservative hold, not final high-volume retirement acceptance.
The lightweight lifecycle/claim journal deliberately has no entity FKs and does not authorize
canonical deletion. Bounded journal retention/archival is a later explicit policy; these
receipt tables currently retain ownership evidence rather than purging it automatically.

N14 and N15 have the bounded controlled native coverage described above. N16 has sixteen
native steady-state cycles and separate fault/decay readers; high-volume installed-world
acceptance and restart on every cycle remain pending. Earlier Phase3A persistence/body
checks remain supporting evidence, not substitutes for complete installed gameplay.

The ordinary backup regression fixture separately exercises the native backup transfer
helper, an atomic form/remains save failure and retry, a separate-process real corpse reload
without a retired-body preload, database possession/reference refusal and the real corpse
component's final release callback. Its item host and world services are controlled fixtures;
the persisted foreign inventory join is deliberately unloaded. It does not qualify a full
command session, timed decay, native NPC death or the later summon adapters.


## Native created consumables and worn lights

The bounded Storm Spear adapter uses the existing exact native leaf-item journal, adding explicit primary-hand placement and persisted carried component rank. It admits one approved plain melee weapon, validates the recipient's dominant wield location and capacity before payment, and checks custody after native Get/Wield callbacks. Redirected custody after payment retains a NeedsReview operation and owned output for reconciliation; it cannot become a successful conflicting wield record. Every stock grade expires, including seven. Lifetime uses the recovered loop's nominal 0.75 seconds per event-heap unit as an exact native deadline; historical batching and object 488 JavaScript damage are not reproduced. The builder must select an authored native electrical weapon profile. Native acceptance and its controlled-world limits are recorded in the Storm Spear checkpoint receipt; full boot, ordinary autonomous attack selection/defences, concurrency and complete installer qualification remain separate gates.

The bounded item adapter also admits exact unscripted Holdable + Food and Holdable + Wearable + ProgLight graphs. Food requires positive finite bites, nonnegative finite nutrition and a native decorator. Lights require native wear profiles without wear scripts and positive finite illumination. Other graphs still require an adapter. Existing ownership, permanent value policy, callback-free retirement, exact single-leaf claims and conservative foreign-dependency holds apply to each output.

`createitem count grade` produces one separate plain food object per selected configured grade (1–7), with a separate stable origin for each and one prepared absolute deadline across its batch. It does not use a stack or reset bites. A partial exposure failure retains its paid casting uncertainty; already committed outputs retain exact ownership and cannot be replayed. `count single` retains the existing behavior. Grade-count food cannot select a permanent-grade alternate. Stock Sustain Meal uses TemporaryCleanup at every grade; an independently configured permanent food remains an ordinary consumable, rather than becoming indestructible.

`createitem placement wornlight` admits a character recipient's default wear profile before payment and revalidates it at application. The service persists Lit=true in the private light graph before exposure. Placement uses native Get/WearExternally without requiring a free held-item location; actual worn membership is required. Recipient illumination and saved wear-profile IDs use ordinary engine mechanisms. A permanent-grade alternate cannot select a light and bypass this admission. Temporary expiry detaches only the exact owned light; foreign worn gear survives. Food and ProgLight add raw component snapshots to the existing native removal rollback journal.

Food component XML reads fractional bites as doubles and loads an exhausted zero remainder without deleting an incomplete parent. Exhaustion marks the remainder dirty even when retirement is held. Native eating refuses invalid or exhausted portions before applying nutrition. Explicit needs fulfilment queues the Character whose row stores active needs as well as its Body. Reconciliation never recreates missing food. Ordinary save/reload and failed-removal retry conserve consumed quantities; this is not a new atomic transaction spanning every possible eating-event callback.

Exact created-item removal admits a native holder carrying passive detection effects: the exact native magick, invisible, ethereal and infravision classes, with no applicability prog, their original custodian, and their exact native `MagicSpellParent` containing only those registered children. Custom subclasses, unknown effects, mismatched or incomplete effect graphs and scripted applicability still require a custody adapter. Detection effects remain active throughout removal. The existing captured-graph transaction, dependency/reacquisition checks and compensation still govern native callbacks. A genuine retirement hold preserves the consumed zero remainder and already credited needs; retry removes that same item without another meal.

`createliquid containerfill on` supplies a prepayment route-bound `litres` formula, optional `bonusplane <plane> <multiplier>`, and `compatible <liquid>` toggles. It fills an accessible open container that owns its mixture, clamps to remaining capacity and initializes empty mixtures. The source liquid is always compatible; every existing original and effective liquid must be the source or an explicitly allowed ID. This recipe policy supplies genuine refusal while the engine's general CanMerge predicate remains permissive. Existing constituents are merged, never silently converted. Legacy wetting/puddles/native-unit formulas retain their behavior, with empty-container initialization corrected. Created water/wine have no magical lifecycle or expiry. Normal drinking, transfer and mixture XML own subsequent consumption/persistence. Arbitrary adjustment callbacks and stored-device adapters remain separate qualification.

These are reusable runtime capabilities and controlled replacement fixtures, not installed stock profiles. Historical prototypes, stock installation and full native scenarios remain in the completion ledger.

Inventory plans that own no restoration effects now finalize without traversing the actor's live effect list. This shared no-op covers abandoned feasibility and unexecuted plans, whose destructor otherwise ran actor cleanup on the GC thread. Plans with associated restoration effects retain their existing restore/removal behavior. Bounded managed ownership checks and forced native garbage collection preserve foreign actor effects, needs and resources; broader nonempty-plan finalizer concurrency is not qualified by this guard.

Create Item lifecycle output pools are editable using output <grade 1-7> <prototype> ...|none. Each grade pool contains 1-32 distinct approved plain-item prototypes, all validated before payment. A casting copy retains one sampled prototype through its preparation/application. The permanent <grade> switch controls lifetime independently from the selected pool. Pools cannot bypass worn-light or count-grade admission. eligibility <boolean(character) prog>|none and lifetimemultiplier <number(character) prog>|none configure environment admission and a finite positive lifetime multiplier; malformed persisted policy retains its original editable XML and refuses casting.

Consumed material plan grade <number> <grade 1-7>|all is persisted, displayed and bound on an invocation copy. Other grades omit that requirement without deleting materials; runtime selectors are preserved. Missing grade-dependent tags keep their original ID and refuse before payment and consumption, preventing null-tag wildcard consumption. The existing legacy/custom plan path remains unchanged when no grade selection exists. Temporary weapons refuse salvage; permanent staff remains ordinary salvage-eligible material. Exact expiry/restart conserves foreign bag, permanent staff and sibling goods; source/native qualification limits are recorded in Armageddon_FlameKnife_Stock_Verification.json.

## Sand Knife source and qualification

Recovered spell_sand_jambiya uses component-free grades1-6 (objects456-461), then get_componentB(Creation,7) rank>=6 consumption and random permanent1378-1385 staff output at mon. Temporary heap duration is ((grade+1)*3000)/2, mapped using the shared event loop nominal0.75 seconds/unit to (grade+1)*1500*0.75 seconds. There is no shadow half-duration. Builder grade pools, permanent-grade switch, direct material selection and existing item ownership journal are reused without changing the shared lifecycle schema.

Enough-sand source includes five sectors plus room sandstorm flag or numeric weather condition>=2.5; wrapper refuses Inside/City first. This bounded native stock maps those to editable terrain names, a typed64-bit room tag and a builder-selected weather event, with a controlled controller in acceptance. It targets the caster and preserves foreign custody on restart/expiry. Historical sector/condition parity, other-recipient placement, privileged source component exemptions, charged staff payloads, all random outcomes and complete installed-world acceptance remain pending. See Armageddon_SandKnife_Stock_Verification.json for exact evidence and limits.

### Queued servitor command authority

The bounded queued-order repair binds accepted SelectedCombatAction to its exact actor/controller references, canonical IDs and immutable active animation origin. CommandGrant returns the current authorized origin only while the existing creator, exact secondary, Active state and independent absolute control/animation deadlines validate. Old orders refuse expiration, revocation, replacement origin, stale/reloaded physical identities, ambiguous canonical roots, changed or removed command policy and allowlist revocation. Checks surround executable policy and native defender callbacks, including multi-target children. Rejection after retirement adds no runtime work.

Control expiry preserves animation and its independent native follower link. Selected actions and accepted grants are ephemeral; fresh readers inspect durable origin/deadlines without materializing or replaying actors/orders. Diagnostic journal version updates alone do not constitute a new grant. No transfer/regrant API or PC-charm implementation is introduced. Spell catalogue, eight feature packages, devices, acquisition/installer and complete native release gates remain open; checkpoint receipts qualify the tested scope.

Parent review rejected the original queued-authority checkpoint because internal ResolveMove callbacks could run after its outer gates. The bounded correction revalidates retrieve and three posture paths after their internal responses, before native inventory/posture mutation, and suppresses rejected action stamina. Controlled paid-stock native retrieval proves a valid outer response followed by actual internal control expiry or combat departure, with independent animation/following preserved. Remaining attack/manual/message/defence-Prog callbacks are [open audit findings](Armageddon_QueuedCommand_Callback_Audit.md), rather than cleared lifecycle guarantees. N13 PC charm remains not_run.
