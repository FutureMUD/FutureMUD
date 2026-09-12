# Vancian magic runtime

Implementation baseline: `0d5163c5d8ec06cc70a3af65100be3316dc8c97d`. The complete approved contract is [Vancian Magic Design and Implementation](Vancian_Magic_Design_and_Implementation.md).

## State and persistence strategy

`CharacterMagicCapabilityStates` owns one versioned aggregate keyed by the primary identity's character ID and capability ID. Reads do not create records. Each mutation works on a detached copy, validates its expected version, and writes it with optimistic concurrency. The character's periodic save path does not own this aggregate and cannot overwrite it. Capability grants and instance creation never initialise capacity.

`VancianMagicOperations` records consumption and callback intent before world side effects. Slots, qualifications and plans are committed together. Callbacks move from Pending to Invoking before invocation and then Completed or NeedsReview; an indeterminate callback is never replayed. Same-aggregate mutation is rejected while a callback is running, while queries remain available.

Item production uses identity/capability and item guards. A production reservation captures versions and exact slot/item identities. Before irreversible material work, a durable operation records the debit intent; failed or interrupted precommit work leaves the original ledger and item payloads. A postcommit failure leaves an inspectable operation and cannot be retried to produce a free charge. Charged scroll consumption is durably tombstoned before normal item deletion or effects. Every activation/transcription availability check consults tombstones, so a stale periodic item save cannot restore a usable consumed charge. Arbitrary spell effects and external FutureProg side effects are never replayed after a process failure.

Spellbook formulae and scroll snapshots are component instance XML. Prototype revisions carry capacity, policy, time, materials and presentation only. New instances are blank. Snapshot numerical evaluation is isolated from the reader's traits and from shared formula objects; the actual reader remains the actor for targets, attribution and persistent-effect ownership.

## Types and configuration

`IVancianMagicCapability` extends the existing capability contract. `VancianMagicCapability` reuses the non-inserting `skilllevel` loader for concentration, inherent powers and resource regenerators; the factory registers the separate `vancian` token. It neither installs stock spells nor assigns a capability to characters. Incomplete configurations have visible errors and grant no Vancian access. Corrupt/unknown-version configuration is preserved and cannot be silently rewritten by the normal editor.

Repertoire definitions specify Selected or Spellbook sources, candidate bounds and a pure predicate. Selected limits are per base spell level. Allowances explicitly link repertoire GUIDs and specify Memorised, Spontaneous or AtWill use, level bounds and optional eligibility. Finite capacity is a pure numeric prog result; at-will has neither a slot level nor a capacity prog. Book sources support memorisation only. Names/aliases/order are presentation. New rules, allowances and clones receive new GUIDs. Mode/slot-level/link changes increase the structural allowance version after confirmation. Historical slots remain suspended until an authorised refresh replaces the ledger.

`VancianPolicy` validates exact FutureProg signatures and rejects Any-parameter contracts. Numbers must be finite, non-negative and fit an integer after flooring. Reentrant policy evaluation fails closed. The protective implementation limits are 128 rules, 256 allowances, 1,000 saved plans, 100,000 total finite positions and an 8 MB state/snapshot payload. These are integrity limits, not progression tables or gameplay stockpile limits.

## Identity, selection and generations

The primary identity character owns progression, known-change and recovery arguments and the persisted aggregate. The acting instance owns physical inventory, targeting, costs, checks, output and numerical capture. Duplicate capability grants resolve to the same `(owner, capability)` key. Different capability IDs remain independent even within one school. Loss/reacquisition, switching forms, reconnecting or increasing a count never fills slots.

Selected choices persist in their committed order. A reduced per-level limit suspends excess choices deterministically without deleting them. `KnownSpells` exposes distinct committed selections for policy bookkeeping; actual cast/list availability uses active eligible selections. A whole-capability commit validates all supplied buckets before replacing any. Omitted buckets become empty. Permission runs once for a genuine per-rule set change; reordered sets are a no-op, but moving a spell between rules runs permission even if the aggregate union is unchanged. Callbacks receive separate read-only, distinct spell lists sorted by ID. Callback uncertainty blocks further changes regardless of the 1,000-row display limit on audit history.

Each finite position has allowance GUID/version, ordinal, casting level, state, optional preparation, and optional reservation with previous state. The public view derives suspension without overwriting the underlying state. Capacity falling from two to one and rising to two cannot restore a previously spent second position. An increase beyond the last refreshed capacity creates nothing until refresh.

Saved plans, selected next plan and last committed pattern are separate objects. Spending preserves every assignment in the last pattern. Plan CRUD never changes live slots. Empty plans are explicit; remaining memorised positions become Unassigned, and spontaneous positions become AvailableSpontaneous. There is no mid-cycle slot reassignment. A selected spell removed later may still have an unspent prepared copy, subject to current hard restrictions.

## Recovery and books

`VancianSleepTracker` observes normal active primary-character processes and state changes across the identity's instances. It rejects unconsciousness, death, stasis, awake focus/controllable projections and unobserved gaps. A gap exceeding 15 seconds between observations resets an unfinished episode; there is no inference that sleep continued through a server pause. Logout drops unfinished episodes; earned qualification is durable. One episode awards at most one outstanding qualification per capability. Awake secondary-instance commands and focus changes interrupt magical work.

PreparationAction uses a timed conscious action. SleepThenPreparation first earns sleep credit and consumes it only on successful timed preparation. SleepAutomatic attempts once on waking; a refusal retains credit and the player can explicitly retry. It does not emit an error every heartbeat. Every mode respects the configured elapsed minimum interval. Refresh commits ledger, last pattern, generation, UTC timestamp and qualification together, then runs the optional callback with the same no-replay discipline as known changes.

`VancianTimedAction` integrates movement, combat, state, logout, death, focus and item-access interruptions. Explicit script completion requires an issued token and the full elapsed production time; it removes the completed action. Completion revalidates versions, current capability, physical access, policies, counts, source items and material plans. No persisted reservation resumes itself after reboot; staff inspect and cancel precommit reservations.

Book policy compares a multiset of `(rule, allowance GUID/version, slot level, base spell ID)` over each entire book subpattern. Ordinals, plan names and physical source-book IDs do not affect equality. EveryRefresh requires accessible formulae each time. PatternChangesOnly requires them when that subpattern changes. A single formula supplies repeated copies; several usable books can supply a plan. Access uses normal visibility/manipulation, open containers, take permission, optional readability and explicit policy progs. Possession/ownership is not required. Already memorised copies do not require a book at release.

## Invocation and stored potency

Ordinary `ICastMagicTrigger` parsing remains authoritative. `SpellTargetCapture` scopes the existing trigger invocation, captures its resolved target/additional parameters, and prevents effects/payment during parsing. Only an engine-created `SpellInvocationContext` can enter the Vancian core route. Explicit source selection resolves a prepared ordinal, a spontaneous ordinal, or AtWill; players cannot inject arbitrary spell power. Matching levels use configured BasePower (Standard by default), with `step × excess levels`, calculated with wide arithmetic and capped at RecklesslyPowerful. Trigger range incompatibility refuses before debit.

Direct casts debit the exact finite position before normal resource/material payment and resolution. At-will skips only finite slot debit. Synthetic successful check results use `CheckOutcome`/outcome helpers; target resistance, wards, reflection and ordinary release effects remain live. The corrected group loop resolves every member before aggregation. Legacy known-prog access requires a non-Vancian capability in the school; a Vancian-only character cannot bypass expenditure through the old school cast or public `CastSpell`. An independently valid legacy route and authorised spell-backed power contexts remain available. Attack/trap/substance triggers receive no new preparation or debit route.

`SpellNumericalContext` supplies spelllevel, castinglevel, casterlevel, power, degrees and success. For scrolls it captures each supported numerical field's original and processed formula, all named trait bindings, `variable`, resolved extended options and required `TraitBonusContext` from the creator. Capture never evaluates a spell effect, a dummy target or the authored random formula. At release, a new expression evaluates frozen creator bindings with the actual target outcome and fresh authored randomness. Detached invocation copies prevent shared-catalogue expression mutation. The [compatibility inventory](Vancian_Scroll_Compatibility.md) names every registered effect and its explicit support decision.

The stored XML also contains a frozen spell model, duration formula, source IDs, school, base/casting levels, power, creator provenance, charge GUID, UTC creation time and configuration checksum. Source numeric edits do not rewrite existing scrolls. Explicit revocation, deleted source spell, changed school or missing required references make an unused charge inert. Retained effects reconstruct their snapshot even after source revocation/deletion. They use the reader's identity/instance for attribution; no creator character is loaded. Spell armour serializes its contextual absorption formula and captured bindings into its child effect, including after subsequent damage/save/load cycles.

## Writing and irreversible boundaries

Inscription chooses a real available casting and reserves its exact position plus the blank item. Completion combines the spell's own material plan and the component production plan, validating aggregate consumable quantities by persistent item ID. It evaluates and pays resource costs using the actual creator (`self = 0` for inscription), applies normal lockouts, and creates one charge. No target or caster-side effect is executed during inscription. Those effects are release behavior, never general production costs.

Transcription pays the destination book's configured time/materials, adds only a logical base formula, and preserves a source book. A source scroll is claimed and destroyed at commitment. Copying an eligible high-level formula requires no slot, known choice or activation check. Duplicate, self, full or inaccessible destinations refuse before expenditure.

Activation validates reader/capability, item, compatibility, target and policy first. Normal ceiling uses current positive finite capacity, ignoring how many slots are spent, plus the actual spell's eligible at-will level. It does not infer an infinite ceiling from at-will use. Over-level defaults to Normal for one excess level, staging once per additional level and capping at Impossible; a typed override and check trait/threshold are configurable. The charge GUID is inserted into the durable operation ledger before destroying the item or rolling the over-level control check. A committed failure fizzles destructively. Successful release pays no second spell resources, materials or slots.

| Operation stage | Failure/restart policy |
| --- | --- |
| Reserved | No casting/material debit; cancel releases the exact reservation. |
| Committing | Debit intent is durable. Do not refund, create a replacement or replay effects automatically. |
| Consumed | Charge tombstone prevents stale item reload from activating/transcribing again. A Cast or ScrollActivation still at this stage requires inspection; a ScrollConsumption row is the permanent source tombstone for separately journalled transcription. |
| Pending / Invoking callback | State is committed; staff inspect whether bookkeeping occurred. Never replay automatically. |
| NeedsReview | Inspect diagnostic, payload, item and owner state; repair external bookkeeping explicitly. |
| Completed / Cancelled | Terminal audit state. A completed consumption record remains a tombstone. |

The engine cannot atomically roll back arbitrary world effects or external prog side effects with every item/character save. It deliberately prefers a consumed/quarantined casting over a duplicated/free casting after an indeterminate failure. Isolated FMDB write scopes keep ledger intent independent of unrelated periodic saves. The database is not a general workflow engine. Staff `acknowledge` does not replay a callback or grant a replacement. Only Reserved work can be cancelled/refunded.

## Schema and extension work

Migration `20260912061254_VancianMagic` adds default-zero `MagicSpells.SpellLevel`, default-false `ScrollInscriptionAllowed`, the composite-key aggregate and GUID operation ledger. The aggregate version is an EF concurrency token. Owner/capability deletion cascades aggregate state; operation rows deliberately have no cascading foreign keys so charge tombstones and repair evidence survive item, capability and character deletion. XML schemas currently use version 1. Model, generated designer and EF snapshot agree. The installer SQL contains a reviewed, EF-generated idempotent migration delta using the maintained dump's lowercase table names; fresh import/upgrade is a separate verification gate.

New effect implementations default to unsupported for scrolls. Add a manifest entry, identify every creator-dependent numerical input and retained field, provide a typed adapter and missing-reference validation, then test release and persistence with different creator/reader stats. Do not fake a creator character, pre-roll results or install global ambient numerical bindings. The service's public validated writing entry points are also the crafting/FutureProg integration boundary; there is no unchecked public setter for a prepaid payload.

See the [builder guide](Vancian_Magic_Builder_Guide.md), [player guide](Vancian_Magic_Player_Guide.md), [full design](Vancian_Magic_Design_and_Implementation.md) and [verification report](Vancian_Magic_Verification.md).
