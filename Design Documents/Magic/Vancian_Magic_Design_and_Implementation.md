# FutureMUD: Vancian Magic
## Full Feature Design and Implementation-Agent Brief

**Document version:** 1.0
**Date:** 12 September 2026
**Repository:** `FutureMUD/FutureMUD`
**Reviewed baseline:** `2b31d786c95deb2899adc7d3e9f26e63bea0e55b` (`master` when checked)
**Status:** Approved gameplay design, translated into a normative implementation contract. This document does not assert that the proposed feature exists or has been tested.
**Suggested repository destination:** `Design Documents/Magic/Vancian_Magic_Design_and_Implementation.md`

---

## Contents

- [Full Feature Design and Implementation-Agent Brief](#full-feature-design-and-implementation-agent-brief)
- [0. Agent assignment and how to use this document](#0-agent-assignment-and-how-to-use-this-document)
- [1. Feature overview](#1-feature-overview)
- [2. Approved decisions and non-negotiable invariants](#2-approved-decisions-and-non-negotiable-invariants)
- [3. Capability configuration](#3-capability-configuration)
- [4. Character state, finite slots and ownership](#4-character-state-finite-slots-and-ownership)
- [5. Selected known spells and builder policy hooks](#5-selected-known-spells-and-builder-policy-hooks)
- [6. Saved loadouts and the last committed pattern](#6-saved-loadouts-and-the-last-committed-pattern)
- [7. Refresh and recovery](#7-refresh-and-recovery)
- [8. Casting, upcasting and integration with existing spells](#8-casting-upcasting-and-integration-with-existing-spells)
- [9. Spellbooks and transcription](#9-spellbooks-and-transcription)
- [10. Scroll inscription and production costs](#10-scroll-inscription-and-production-costs)
- [11. Scroll activation](#11-scroll-activation)
- [12. Stored creator potency and spell-snapshot compatibility](#12-stored-creator-potency-and-spell-snapshot-compatibility)
- [13. Player and staff command contract](#13-player-and-staff-command-contract)
- [14. Builder contract and configuration validation](#14-builder-contract-and-configuration-validation)
- [15. FutureProg and service API](#15-futureprog-and-service-api)
- [16. Required worked examples and expected gameplay](#16-required-worked-examples-and-expected-gameplay)
- [17. Implementation architecture and work breakdown](#17-implementation-architecture-and-work-breakdown)
- [18. Persistence, migrations and operation integrity](#18-persistence-migrations-and-operation-integrity)
- [19. Compatibility and cross-cutting requirements](#19-compatibility-and-cross-cutting-requirements)
- [20. Required regression and integration tests](#20-required-regression-and-integration-tests)
- [21. Verification, documentation deliverables and completion report](#21-verification-documentation-deliverables-and-completion-report)
- [22. Definition of done](#22-definition-of-done)
- [Appendix A. Source grounding and repository references](#appendix-a-source-grounding-and-repository-references)
- [Appendix B. Handoff summary](#appendix-b-handoff-summary)

---

## 0. Agent assignment and how to use this document

Implement the complete feature described here: a configurable Vancian magic capability, capability-local selected repertoires, memorised and spontaneous spell slots, at-will allowances, saved loadouts, refresh/recovery, instance-owned spellbooks, transcription, and prepaid single-use spell scrolls with stored creator potency.

This is a self-contained assignment. No other planning conversation or project roadmap is required. Sections 1–16 are the full feature specification; sections 17–22 define implementation, documentation, tests, verification and completion. The source appendix distinguishes verified existing interfaces from proposed additions.

The existing multi-target spell-resolution correction is already present in the reviewed baseline. Preserve its behaviour and regression tests; do not reimplement that fix or restore the earlier short-circuiting group loop. Work against the actual checkout and reconcile subsequent code changes by behaviour, not by copying historical line numbers.

Read the root `AGENTS.md` and the applicable project/module instructions before making changes, including the library, core engine, database, item-component and test-project instructions. Follow the actual project target frameworks and repository toolchain rather than an older framework version mentioned in prose.

### Authority and permitted implementation discretion

The approved product decisions are recorded in section 2. The rest of this document resolves engineering details needed to implement them, such as stable keys, validation, command grammar, transaction boundaries and failure handling. These details are requirements for this handoff, not claims about pre-existing engine behaviour.

C# type names and proposed table names may be adjusted to repository conventions if their responsibilities and semantics are preserved. Player commands, persisted tokens, prog contracts and behaviour must remain consistent across implementation, help, tests and documentation. Do not silently substitute a simpler mechanic, a placeholder, or a raw register-variable implementation for a required first-class feature.

### Explicitly outside scope

No stock magic-system catalogue or `DatabaseSeeder` module; no automatic grants to existing characters; no XP/class/character-level system; no changes to unrelated resource regeneration; no new spell-effect catalogue; no rechargeable wands or multi-charge scrolls; no natural-language spell recognition from ordinary writing; no generic offline-rest simulation; and no broad rewrite of all magic, combat, crafting or inventory systems.

Small deterministic fixtures and a documented disposable-world example are required. They are not a production catalogue or an installer package.

---

## 1. Feature overview

A Vancian capability is a route by which a character accesses and pays for spells. It is not a hard-coded Wizard, Cleric or Sorcerer class.

One capability can combine different rules. A Wizard can select a small repertoire of level-zero cantrips that can be cast at will, while preparing finite copies of higher-level spells from a borrowed spellbook. A Cleric can select a capability-local repertoire and prepare copies from that repertoire. A Sorcerer can spend unassigned slots on spells in a selected repertoire.

The same `MagicSpell` can be available through different capabilities without duplicating its effects. Each invocation selects one coherent route: one capability, one repertoire rule and one casting allowance. Access from one route cannot be combined with payment from another.

### Terminology

| Term | Meaning |
|---|---|
| Spell level | New non-negative integer on `MagicSpell`. Zero is a valid level. Existing rows default to zero, but receive no automatic Vancian access. |
| Caster level | A non-negative integer derived from a capability-configured numeric FutureProg. The MUD decides what progression means. |
| Repertoire rule | A subordinate capability definition determining candidate spells and whether access comes from selected knowledge or spellbook formulae. |
| Candidate spell | A spell the repertoire permits the character to select or use as a formula, subject to the rule's bounds and prog. It is not automatically selected, prepared or castable. |
| Selected known spell | A committed choice belonging to a specific identity, capability and selected-repertoire rule. It is distinct from legacy `SpellKnownProg` knowledge. |
| Formula | A structured reference to a spell stored on a spellbook item instance. It supplies information for preparation/transcription, not a free casting. |
| Casting allowance | A subordinate capability definition providing memorised, spontaneous or at-will casting, linked to one or more repertoire rules. |
| Slot | One finite allowance unit at a specified slot level. |
| Memorised copy | One specific spell assigned to one finite slot by a successful refresh. Duplicate preparations are separate copies. |
| Loadout | A named editable plan for assigning memorised slots. It contains no usable charges. |
| Selected loadout | The plan requested for the next refresh. Selecting or editing it does not change current memorisation. |
| Last committed pattern | An immutable snapshot of the complete assignment established by the last successful refresh, including copies subsequently spent. |
| Refresh | A successful all-or-nothing operation that restores finite capacity and applies the chosen memorisation pattern. |
| At-will | An explicit allowance mode with no finite slot debit and no memorisation; other casting requirements still apply. |
| Stored casting | A prepaid scroll payload containing a spell snapshot, casting level, power and creator-derived numerical potency. |
| Acting character | The actual character/body performing the action in the world. |
| State owner | The canonical character identity owning the capability state. It is not necessarily the acting body instance. |

### Example configurations

| Configuration | Repertoire | Allowance |
|---|---|---|
| Wizard cantrips | Selected level-zero candidates | At-will |
| Wizard levelled magic | Book formulae | Finite memorised slots |
| Cleric cantrips | Selected level-zero candidates | At-will |
| Cleric levelled magic | Selected candidates | Finite memorised slots |
| Sorcerer cantrips | Selected level-zero candidates | At-will |
| Sorcerer levelled magic | Selected candidates | Finite spontaneous slots |

These examples demonstrate configuration. The engine must not branch on capability names or class labels.

---

## 2. Approved decisions and non-negotiable invariants

| ID | Requirement |
|---|---|
| VAN-01 | Introduce one configurable capability type with canonical persisted/builder token `vancian`, with repertoire rules and casting allowances inside it. |
| VAN-02 | Selected known spells are local to this capability and repertoire rule; they do not modify or depend on legacy known-spell state. |
| VAN-03 | Builders control permission to change selected known spells through `CanChangeKnownSpells`; `OnChangesKnownSpells` runs after an actual committed change. No hard-coded daily or class-based restriction. |
| VAN-04 | Players can browse candidates, build a draft and explicitly commit a whole-capability selected repertoire. |
| VAN-05 | Support multiple named loadouts; loadout changes take effect only through refresh. No rearrangement of unspent memorised slots between refreshes. |
| VAN-06 | Keep the last committed memorisation pattern independently of editable loadouts and remaining prepared copies. |
| VAN-07 | Support books-required-for-every-refresh and books-required-only-to-change-the-pattern policies. |
| VAN-08 | Borrowed and stolen books are usable when physically accessible and allowed by configured progs. Formulae are item-instance data, never prototype content. |
| VAN-09 | Support book-to-book copying and scroll-to-book transcription. Book copying preserves its source; successful scroll transcription consumes/destroys its source. |
| VAN-10 | Add `SpellLevel`, default zero. Zero is an ordinary numerical level; being level zero never automatically grants at-will access. |
| VAN-11 | Read caster progression from a numeric capability prog. Do not introduce XP or global class levels. |
| VAN-12 | Memorised, spontaneous and at-will allowances may coexist inside one capability. Unlimited uses must not be represented by an arbitrary large counter. |
| VAN-13 | Permit lower-level spells in higher-level slots when the allowance permits them. Default power is Standard plus one step per excess slot level, capped at RecklesslyPowerful. |
| VAN-14 | At-will cantrips cast at their base level/Standard power; permitted higher-slot versions consume the corresponding finite casting. |
| VAN-15 | Scroll inscription prepays the casting, spell materials and builder-configured production costs, without applying any spell effects. |
| VAN-16 | Scroll activation consumes no personal slot or repeated spell materials. A compatible caster need not know or have prepared the spell. |
| VAN-17 | Scrolls above a reader's normal casting capacity may be attempted using a configured activation check. Committed failures destroy the scroll. |
| VAN-18 | Scroll potency is stored from the creator. The reader supplies targeting, location, effect ownership and responsibility, not replacement numerical potency. |
| VAN-19 | Scrolls contain one spell and one use; committed activation destroys them whether it succeeds, fizzles, is resisted or is blocked. |
| VAN-20 | Ordinary at-will scroll inscription pays no fictitious finite slot, but still pays all applicable production and spell-material costs. |
| VAN-21 | Identity-wide ownership prevents projection, copies, possession, reconnects and repeated capability grants from duplicating allowances. |
| VAN-22 | Preserve legitimate legacy casting and prepared attack/trap/substance delivery. A Vancian route may not bypass slots through the legacy command. |
| VAN-23 | Ordinary Vancian casting is reliable once validly committed, while retaining target resistance, wards, targeting and action restrictions. |
| VAN-24 | Production costs, repertoire policies, recovery timings and progression are builder-configurable. Ship the mechanisms and working examples, not a mandatory balance table. |

---

## 3. Capability configuration

### 3.1 Runtime shape

Add a specialised contract such as `IVancianMagicCapability : IMagicCapability`, a runtime implementation `VancianMagicCapability`, and typed subordinate definitions. Register `vancian` through `MagicCapabilityFactory` for both builder creation and database loading.

Retain normal school membership, concentration, prompt visibility, regenerators and optional inherent-power rules. Shared code may be extracted from `SkillLevelBasedMagicCapability`, but do not inherit behaviour by invoking a constructor that inserts a `skilllevel` database record. Vancian slot accounting is not a magic-resource generator.

The existing persisted `PowerLevel` field must not be repurposed as the character's caster level. Caster level is per character and obtained through the new prog.

Store capability configuration as versioned XML in the existing capability definition. Preserve unrelated existing capability types and their XML contracts.

### 3.2 General configuration

| Field | Contract |
|---|---|
| `DefinitionVersion` | Schema version for the new capability payload. |
| `CasterLevelProg` | Required number-returning prog `(character, capability)`. The character is the canonical state owner. |
| `CanChangeKnownSpellsProg` | Required boolean hook described in section 5; absent or invalid denies changes. |
| `OnChangesKnownSpellsProg` | Optional void hook described in section 5. |
| `CanCastSpellProg` | Optional boolean `(actor, capability, spell)`, an ongoing hard prohibition independent of selected knowledge; absent means no additional restriction. |
| `BasePower` | Default `SpellPower.Standard`; a defined enum value. |
| `PowerStepPerSlotLevel` | Non-negative integer, default 1. |
| `ReliableOutcome` | Successful `Outcome`, default `Pass`; allowed values MinorPass, Pass or MajorPass. Supplies the reliable cast's base result. |
| `MaximumSavedLoadouts` | Positive integer; default 10. Limit is protective/configurable, not a slot mechanic. |
| Recovery fields | Section 7. |
| Book, transcription and inscription settings | Sections 9 and 10. |
| Scroll eligibility/check settings | Section 11. |
| Concentration/support fields | Existing capability responsibilities, with their existing meanings and validation. |

Caster level uses `floor(value)` after validating that the returned number is finite and non-negative. Null, errors, infinities and negative values are configuration failures, not a new unlimited mode. Return zero to express a valid minimum level. Pure calculations may be cached within one operation; never cache forever when a builder prog can depend on live state.

### 3.3 Repertoire-rule definition

Each rule has:

- Immutable `Key` (a generated GUID, persisted in XML), editable unique `Alias` and `Name`, and presentation `SortOrder`.
- `Source`: `Selected` or `Spellbook`.
- Non-negative inclusive `MinimumSpellLevel` and `MaximumSpellLevel`.
- Required boolean `CandidateProg(character, capability, spell)`.
- For Selected rules, required numeric `SelectionLimitProg(character, capability, casterLevel, spellLevel)`. Limits apply separately at each base spell level in this rule. Zero permits no selections; finite non-negative numbers are floored; invalid results fail closed.
- For Spellbook rules, `BookPolicy`: `EveryRefresh` or `PatternChangesOnly`.

All candidates must belong to the capability's exact school, use an ordinary cast trigger, and be ready under the Vancian invocation requirements. Prepared attack and substance payloads are never candidates. Parent-school taxonomy alone does not grant access.

A candidate prog may permit a spell before the character has a slot capable of casting it; this supports owning/copying high-level formulae in advance. Slot capacity is checked separately. A selected rule's count limit may nevertheless prevent choosing a particular level until progression permits it.

A spell can be eligible in several rules, but every selection and invocation must carry an explicit rule key. Selections in different rules consume their respective allowances. The aggregate known-spell hook uses distinct spell sets: moving an unchanged selection between rule buckets must not bypass the permission hook (section 5).

### 3.4 Casting-allowance definition

Each allowance has immutable `Key`, editable unique `Alias`/`Name`, presentation order, linked repertoire keys, base-spell-level bounds, and optional boolean `SpellEligibilityProg(character, capability, spell)`.

| Mode | Additional configuration |
|---|---|
| `Memorised` | Non-negative `SlotLevel`; numeric `SlotCountProg(character, capability, casterLevel, slotLevel)`; linked Selected and/or Spellbook rules. |
| `Spontaneous` | Non-negative `SlotLevel`; numeric `SlotCountProg(...)`; linked Selected rules only. |
| `AtWill` | No finite slot count and no slot level; linked Selected rules only. Effective casting level is the selected spell's own level. |

Finite allowances require `spell.SpellLevel <= SlotLevel`. Their level bounds and eligibility prog can impose narrower constraints. An at-will allowance's unlimited count is not an unlimited level ceiling.

Validate unsupported source/mode combinations in the builder and loader: do not allow spontaneous casting directly from arbitrary book contents or at-will casting from an unselected book repertoire in this version. These are not needed by the agreed configurations.

### 3.5 Stable keys and safe configuration changes

Names, aliases and presentation order may change without changing keys or resetting character state. Reusing a deleted alias creates a new key, not a resurrection of the old allowance. Deleted definitions leave orphaned historical state visible to staff rather than reassociating it by name.

Changing an allowance's mode, slot level or linked-rule structure increments a structural version. Existing slot entries belonging to the old structure are suspended until a successful refresh against the new structure; they are not converted into fresh capacity. Capability cloning copies configuration but creates an independent capability ID and remapped subordinate keys, never character state.

No global hard-coded maximum of nine spell levels is necessary. Validate arithmetic overflow and bounded configuration sizes, but do not encode a specific game's spell progression into the engine.

---

## 4. Character state, finite slots and ownership

### 4.1 Ownership

Resolve state by `(CharacterIdentityId, MagicCapabilityId)`, using the canonical identity facilities already used by the engine. Do not key it by `BodyId`, secondary `InstanceId`, merit ID or grant source. Multiple applicable merits/effects granting the same capability expose the same state once.

The acting body still determines physical access, target location, hands, speech and other action restrictions. Losing access suspends use; it does not delete known selections, plans, expenditure or recovery history. Reacquiring access restores only the unspent state that remains valid.

PC and NPC actors use the same service contracts. Staff need deliberate administrative mutation commands; the normal fact that administrators can see all capabilities must not silently mint prepared slots during a test.

### 4.2 Required persistent state

Maintain:

1. Committed selected spell sets per repertoire key.
2. Named saved loadouts and selected-next-loadout ID.
3. Last committed memorisation pattern, stored as a value snapshot.
4. Finite slot ledger for the current refresh generation.
5. Last successful refresh time, recovery generation, qualification state and a state concurrency version.
6. Operation identifiers/terminal states needed to prevent duplicate commits and to identify failed callbacks or unresolved item operations.

A loadout assignment includes allowance key/version, slot ordinal, repertoire key, spell ID and intended slot level. The committed pattern also records the spell level and computed power when prepared, sufficient to detect subsequently incompatible changes.

### 4.3 Slot ledger

Finite slots have a stable ordinal within `(allowance key, refresh generation)`. An entry is `Unassigned`, `Prepared`, `AvailableSpontaneous`, `Reserved`, `Spent` or `Suspended`. Reservation is temporary operation ownership, not another charge.

A memorised slot becomes Prepared only during refresh. An intentionally empty slot stays Unassigned until another refresh; it cannot be filled mid-cycle. A spontaneous slot is not assigned to a spell until a cast/inscription uses it. A spent slot records its consumption and cannot be cleared by editing a loadout.

Two copies of one spell occupy two entries. Casting a higher-slot preparation consumes that exact entry and uses its stored preparation power. Never silently substitute a different spell, higher-level slot or another capability.

### 4.4 Progression and reduced capacity

Slot capacity at a refresh defines that generation's ledger. Later capacity increases create no usable slots before the next successful refresh. Current capacity reductions restrict use immediately without deleting the ledger.

For a finite allowance with refreshed capacity 4 that now has capacity 2, only ordinals 1 and 2 may be used; entries 3 and 4 are suspended. Their state is retained. If the capacity returns to 4, those original entries resume with their original spent/unspent state, not replenished values. A structural configuration change uses the stricter version rule in section 3.5.

Selected-repertoire reductions similarly preserve data without arbitrary deletions. Apply a deterministic active subset in stored selection order up to each current limit and display excess entries as suspended. New commits must satisfy current limits. Builders can separately impose a hard casting prohibition using the configured prog.

Deletion or incompatible mutation of a referenced spell suspends its entries and invalidates loadout validation. A zero-level migration must not populate any repertoire or book.

### 4.5 Access routes and legacy isolation

A normal Vancian invocation validates this complete route:

`active capability -> linked repertoire -> eligible spell -> selected/book-derived preparation or selected repertoire -> allowance -> available finite entry or at-will permission`.

For a memorised copy, later removal from the selected known set or loss of the source book does not erase that copy. It remains usable until spent/refreshed, provided the capability, structural rule, school, spell definition, candidate eligibility and hard casting permissions remain valid. A book is not needed merely to release an already prepared copy.

A legacy route is separate: an applicable **non-Vancian** capability for that school plus the existing `SpellKnownProg` contract. The presence of a Vancian capability and a broadly true `SpellKnownProg` must not create a legacy escape route. A character genuinely holding both types may deliberately use either valid route.

---

## 5. Selected known spells and builder policy hooks

### 5.1 Drafting and committing

Provide browse, help, draft-add, draft-remove, draft-show, commit and cancel operations. Drafts are editing state, not access. They may be session-local; cancel them on logout rather than reconstructing an automatic pending change. Committed selections persist.

One draft covers all Selected repertoire rules in one capability. Validate each rule's candidates and per-level limits, as well as the character's current access. Re-read capacity and permission at commit, not only when the draft was opened. Reject stale drafts if the base state version changed through another instance or staff action.

Allow intentionally incomplete selections. Adding a new slot allowance or caster level never automatically picks spells.

### 5.2 Prog contracts

Proposed signatures, using existing FutureProg types:

```text
CanChangeKnownSpells(character, capability, currentSpells, proposedSpells) -> boolean
OnChangesKnownSpells(character, capability, previousSpells, newSpells) -> void
```

Each spell parameter is `Collection<MagicSpell>`. `character` is the canonical state owner, so a register variable used as a cooldown cannot be bypassed by changing active bodies. Capability is the actual `MagicCapability` variable.

Collections are distinct spell sets aggregated across Selected rules, passed in deterministic spell-ID order. Because a transfer between rule buckets can change access without changing that union, the permission hook still runs for any actual per-rule set change. The notification hook runs once for that change as well. A completely unchanged per-rule selection, including cosmetic reordering, is a no-op: it does not execute either hook or consume a builder-defined change opportunity.

The hooks receive immutable collection snapshots. They do not permit a prog to mutate the live draft by aliasing a list.

### 5.3 Commit order and errors

For a genuine change:

1. Resolve owner and active capability; acquire the identity/capability operation guard.
2. Validate version, candidates, buckets, levels and limits.
3. Execute the permission prog once with the current proposal; false/null/error denies the change.
4. Commit the complete per-rule selection and operation record together.
5. Invoke the optional notification hook once, with old/new snapshots.
6. Mark notification completion and release the guard.

Do not invoke notification for browsing, edits, rejected proposals, unchanged commits, refresh or reload. Do not grant a special first-selection or new-level bypass: a false permission prog denies player changes, including the initial selection. A builder can permit `currentSpells` being empty or grant a fixed initial repertoire through explicit staff/script operations.

Arbitrary FutureProg side effects cannot be advertised as transactionally exactly-once with every external system. Provide a bounded, durable callback status (`Pending`, `Invoking`, `Completed`, `NeedsReview`). Mark Invoking before calling; a crash during invocation or callback failure leaves the selection committed but blocks further player repertoire changes until staff resolve the bookkeeping. Never automatically rerun an indeterminate callback after reload. Surface the operation, old/new selection and error for repair.

A permission prog is expected to be side-effect free. The builder documentation must say so. The engine must not use it for routine candidate browsing, which could otherwise run a poorly authored mutation repeatedly.

### 5.4 Effect of changing selection

At-will/spontaneous access changes immediately on a successful selected-repertoire commit. It does not refill any slots. Existing memorised copies and the last committed pattern remain unchanged. Subsequent refreshes must validate the requested pattern against the current repertoire; a now-invalid saved plan is reported, not silently repaired.

This means a Cleric who removes a spell from their selected repertoire may still expend a previously memorised copy, but cannot prepare it again without restoring the selection. A hard builder prohibition may separately forbid even that retained copy.

### 5.5 Required builder examples

Document working, compiler-tested examples of:

- Choose once, then reject future changes.
- Allow changes only after a real-world interval, recording the timestamp in `OnChangesKnownSpells`.
- A MUD-supplied policy based on its own game calendar or progression flag.
- A fixed repertoire with permission always false and an explicit administrative initial grant.

Use actual supported register/time functions discovered in the checkout. Do not publish pseudo-FutureProg as executable code. A cooldown compares elapsed time since the stored timestamp; it does not compare the timestamp itself to a duration.

---

## 6. Saved loadouts and the last committed pattern

### 6.1 Player operations

Players can create, inspect, rename, copy, edit, delete, select and validate loadouts. Names are case-insensitively unique within the owner's capability. Display names may be reused across different capabilities. A copied loadout is a new plan, not duplicated prepared slots.

Assignments refer to allowance, repertoire, spell and quantity/slot ordinal. Provide clear display of slot level, base spell level and resulting power. Allow incomplete plans with deliberately empty memorised slots. Do not silently auto-fill empty slots or choose missing spells.

Selecting a loadout changes only `SelectedNextLoadoutId`. Editing that plan after selection still changes only the next request. Existing memorisation stays unchanged until refresh succeeds.

Deleting the selected-next plan clears that selection; it does not alter the committed pattern. An explicit `refresh last` option uses the last committed pattern. With no selected plan and no committed pattern, a prepared capability must request/confirm an initial empty or populated loadout rather than guess.

### 6.2 Validation and pattern comparison

Validation reports all discoverable problems together: unavailable spell, missing source rule, incompatible slot level, excess copies, inaccessible formula, changed configuration, disallowed spell, or insufficient capacity. It consumes nothing and does not run change-notification hooks.

Canonicalise book-dependent subpatterns as multisets of `(repertoire key, allowance key/version, slot level, spell ID, quantity)`. Cosmetic ordering, plan names, source-book item IDs and equivalent interchangeable slot ordinals do not change a pattern. Moving a spell to a higher-level allowance or changing its count does.

For `PatternChangesOnly`, compare the requested subpattern for that book rule against the last committed subpattern. If unchanged, do not require a source book. If changed, require accessible formulae for every book-derived spell in that requested subpattern. Changing only a separate Selected rule does not make an unchanged book subpattern require a book.

For `EveryRefresh`, require accessible formulae for every requested preparation linked to that book rule on every refresh. A single formula entry can support multiple prepared copies, and different accessible books can collectively supply one pattern.

### 6.3 Successful refresh

Commit the new slot ledger, complete last-pattern snapshot, refresh generation, timestamps and consumed qualification together. Do not retain a live object reference to an editable loadout. Changing/deleting a loadout after refresh cannot rewrite that snapshot.

Retain spent positions in the last pattern so `refresh last` can replenish them. For a wholly spent memorisation, the pattern still contains every intended copy.

---

## 7. Refresh and recovery

### 7.1 Supported modes and configuration

| Mode | Behaviour |
|---|---|
| `PreparationAction` | A timed conscious preparation action, then refresh. |
| `SleepAutomatic` | Qualifying continuous sleep awards one qualification; on waking, attempt refresh without another timed preparation action. |
| `SleepThenPreparation` | Qualifying continuous sleep awards one qualification; a subsequent timed preparation action consumes it on success. |

Configure `PreparationDuration` where applicable, `RequiredSleepDuration` where applicable, `MinimumRefreshInterval`, and optional `CanRefreshProg(character, capability)` and `OnRefreshProg(character, capability)` hooks. Durations use explicit elapsed seconds/TimeSpans, not implicit game hours. A builder may use progs to add game-calendar restrictions. There is no automatic midnight reset.

Preparation and required sleep durations must be positive for the modes using them. The minimum refresh interval is non-negative and may be zero when the builder deliberately wants duration-only recovery. The engine's elapsed-time tracking uses an injectable clock; durable wall-clock timestamps are stored in UTC.

### 7.2 Qualification

Track qualifying **observed** continuous sleep, not simply the difference between current time and a login timestamp. Ordinary logout or server downtime earns no sleep credit. An interrupted partial episode does not count as a completed sleep. Do not treat unconsciousness, death, stasis or an immobilised projection anchor as ordinary sleep.

The identity must not simultaneously earn sleep credit from one body while actively playing/commanding an awake projection or other controllable instance. Integrate with identity/control state rather than checking only the primary body's sleeping flag. A default qualification additionally requires the normal character process to be active and the identity not finally dead.

Persist an already-earned qualification, but not an assumption that unobserved sleep continued. At most one outstanding qualification exists per capability; qualifications do not accumulate into several future refills. One sleep episode cannot automatically refresh the same capability repeatedly. A new episode must qualify again after its preceding qualification is consumed.

For SleepAutomatic, execute on the wake transition, not while the character is unable to read/use required books. If the attempt is blocked by an invalid loadout, missing books, cooldown or permission prog, retain the qualification and report the reason. An explicit refresh retry uses the same validation and consumes it only on success. Do not retry on every heartbeat or spam repeated errors.

For SleepThenPreparation, a completed qualification survives logout until used. This is retained earned credit, not offline-rest simulation. A timed preparation interrupted by logout is cancelled and gives no refresh; the earned sleep qualification remains because no refresh committed.

### 7.3 Timed preparation

Capture the requested pattern and base state version when preparation starts. Use normal blocking/action infrastructure. Movement, combat engagement, disabling state, loss of required physical access, changing focus away from the acting instance, cancellation and logout cancel the action. A builder may add prerequisites, but cancellation must not replace the old ledger with partial new data.

Recheck active capability, candidate permissions, capacity, formula access when required, version, elapsed interval and qualification immediately before completion. A mid-action edit does not silently replace the requested pattern. A stale draft/version requires restart with an explanatory message.

A successful refresh is all-or-nothing across this capability's finite allowances. Invalid memorised assignments do not allow only its spontaneous allowances to refill. At-will selections are unaffected. Refresh does not implicitly commit a known-spell draft or execute `OnChangesKnownSpells`.

`OnRefreshProg` is optional bookkeeping after a committed refresh, not an alternative mechanism for granting slots. Use the same callback error visibility/at-most-once discipline as section 5. An indeterminate callback must not cause another refill. Do not introduce a general workflow engine solely for these hooks.

### 7.4 Initial and administrative state

An acquired capability begins with no selected knowledge unless explicitly granted, no committed pattern and no free restored finite capacity. A normal first preparation/refresh establishes it. Staff can explicitly initialise/refresh test or NPC state using the same invariant-preserving services; this must be distinguishable and logged as an administrative action.

A removed/re-added capability resumes its old state; it is not treated as a first acquisition. Staff reset commands require confirmation and cannot be reached through ordinary player routes.

---

## 8. Casting, upcasting and integration with existing spells

### 8.1 Spell metadata

Add `SpellLevel` as a non-null non-negative integer to the model, runtime and `IMagicSpell`, with database default 0 and backwards-compatible loading. Add `magic spell set level <number>`, show/player-help display where relevant, and `spell.level` FutureProg exposure.

Level zero is usable at any mode whose explicit rules permit it. Migration alone does not mark every existing spell as a candidate, grant it to a book or selected repertoire, or enable unlimited casting. No existing non-Vancian mechanic should start interpreting this field implicitly.

### 8.2 Power calculation

For a finite allowance:

```text
excess = slotLevel - spell.SpellLevel
computed = BasePower + excess * PowerStepPerSlotLevel
power = min(SpellPower.RecklesslyPowerful, computed)
```

Require `excess >= 0`; use overflow-safe arithmetic before the enum cap. Defaults are Standard and one step. The result must also lie within the spell trigger's supported power range. Do not silently clamp to a narrower trigger range: show incompatibility before preparation/casting and let the builder change that spell or allowance.

At-will uses effective casting level equal to the spell's level and therefore BasePower. It never obtains unlimited upcast power. An allowed cantrip in a level-one finite slot uses one excess step and consumes that finite entry. Higher-than-necessary slots remain legal at the enum cap; UI must show that extra levels provide no further power increase.

A prepared copy fixes its slot level and power at refresh. A spontaneous cast fixes them when choosing its allowance. If the referenced spell's level/power range changes incompatibly, suspend the stale preparation rather than silently reprice it.

### 8.3 Reliable Vancian resolution

Normal valid Vancian casts do not randomly fizzle. Build a successful base `CheckOutcome` using the capability's `ReliableOutcome` (default Pass) and the engine's existing outcome/degree helpers. Where the resolver expects an all-difficulties map, supply consistent synthetic successful outcomes without improving/branching traits. Do not invent numerical degree constants inconsistent with those helpers.

The legacy minimum casting-success threshold does not reintroduce random failure on this route. Target resistance and ward logic continue normally against the supplied result. Creator/actor traits can still affect authorised spell formulae. Ordinary physical requirements, hard eligibility, cooldowns and targeting are unchanged unless a specific Vancian entry point expressly substitutes its cost source.

This rule applies only to Vancian direct casting/inscription. Legacy casting retains its existing checks. The separate over-level scroll activation check is real, as described in section 11.

### 8.4 Invocation request and target resolution

Introduce a typed request/context for these routes, with at least: actual actor, owner identity, capability/rule/allowance keys, spell, source kind, finite entry or at-will entitlement, effective level, power, resolved target(s), trigger additional parameters, numerical context and operation ID.

Source kinds must distinguish `VancianDirect`, `ScrollInscription` and `ScrollActivation` from legitimate legacy/prepared-payload calls. They are not selectable arbitrary client flags.

Separate target parsing/validation from effect application. Prefer a typed trigger resolution result that existing `ICastMagicTrigger` adapters can use. If a compatibility bridge must call a legacy parser with a computed power argument, scope it narrowly and prove that the parser cannot replace the computed power or execute an uncharged secondary cast. Do not construct gameplay by executing a player command string recursively.

### 8.5 Commitment rules

For a direct cast:

1. Validate active route, finite entry/at-will permission, spell readiness, trigger/target, normal lockouts, resources, materials and physical requirements.
2. Reserve the specific finite entry and material plan; duplicate invocation attempts fail while reserved.
3. Revalidate immediately before commit.
4. Commit the finite debit (none for at-will), normal spell resource/material costs and normal spell lockouts once.
5. Resolve reliable casting, wards, target resistance and target/caster effects using the existing corrected independent-target behaviour.

Preflight refusals consume nothing. A committed attempt remains spent if every target resists, a ward blocks it, or an effect has no applicable work. Do not refund because no persistent child effect was created: instantaneous effects commonly return no retained child.

This assignment does not introduce a new wind-up/combat action model for every legacy spell. Integrate with existing scheduling where present. Any new delayed entry point must reserve and revalidate before commitment, and must not expose a free cast during cancellation.

### 8.6 All access surfaces must agree

Audit and update `MagicModule`, `CharacterKnowsSpell`, `CharacterCanCast`, spell help, ordinary spell lists, FutureProg `knownspells`/`castablespells`/`cancastspell*`, spell-backed powers and any direct `CastSpell` callers.

Ordinary displays may expose the union of available routes for compatibility, but must distinguish candidate, selected, prepared, at-will and depleted states on the Vancian UI. Payload-only spells stay hidden. No global `SpellKnownProg = true` hack is allowed.

When the old school cast syntax encounters a Vancian-only spell, route through proper Vancian selection or refuse with the canonical new syntax. Do not treat a legacy free-form power argument as authority to select extra power. When several possible routes exist, require an explicit route unless the player has deliberately selected a stored default; never choose the cheapest or strongest automatically.

Prepared attack, trap and substance resolvers continue using their existing pre-paid contracts. Do not add a slot debit to them or open them as player-cast spells. Legitimate legacy and Vancian routes can coexist in the same school and on the same character.

---

## 9. Spellbooks and transcription

### 9.1 Item components

Introduce `ISpellbook` and `SpellbookGameItemComponent` with a builder-creatable prototype token `spellbook`. This is a component-based item feature, not a special parser for ordinary book prose.

Prototype configuration includes capacity/eligibility settings, optional normal readable-item composition, usability requirements and presentation. **No spell formula list belongs on the prototype.** New blank instances have no formulae. The instance payload contains a versioned collection of spell IDs, stable entry identifiers and optional provenance (copy time/source), all saved through normal item-component persistence.

Deep-copy mutable collections when an authorised staff item copy is created. Normal inventory moves, saves, loading and prototype revision must preserve instance formulae. Instantiating two copies of one prototype must not share a formula list.

Formulae contain no prepared copies, slot reservations or scroll numerical snapshot. A scroll of an upcast level-one spell teaches the level-one formula, not a new fifth-level version.

### 9.2 Access and visibility

Preparation/copying requires an accessible, usable book. Resolve actual inventory/container access and normal perception. Do not search the entire world's books, inspect other characters' hidden inventories, or access locked/closed contents through `Gameworld` lookups.

A borrowed or stolen accessible book is usable by default; ownership is not an inherent restriction. Optional `CanUseSpellbookProg(actor, capability, book)` can impose world-specific restrictions. Where a separate readable component/language/script requirement is configured, honour it; possession alone does not imply the ability to read that component. The feature itself does not mandate personal attunement or a particular language.

An accessible local placed book can be used if the ordinary item-manipulation/readability rules permit it. For explicit copying, source and destination must remain accessible throughout the action. Source formulae do not need to remain accessible after a successful preparation to cast existing copies.

### 9.3 Transcription

Provide a timed operation from a book or charged scroll to a destination spellbook. Check capability, source readability/usability, formula eligibility, destination capacity, non-duplication and combined production requirements before starting and again at completion.

Transcription does not consume a personal spell slot. Its cost is the configured copying materials/time. It is not a spell activation and must not apply either target or caster effects, roll target resistance, or perform the over-level scroll activation check. A compatible caster may copy an eligible high-level formula in advance even if they cannot yet prepare it.

On successful book-to-book copying: add one formula to the destination and leave the source unchanged. Source and destination must be distinct items. On successful scroll-to-book transcription: add the formula and irreversibly consume/destroy the source scroll as one guarded operation. The scroll cannot be cast concurrently with copying. A duplicate destination formula is refused without consuming it.

Use a combined inventory plan for configured copying costs. Builders control costs and duration; the engine owns atomic formula addition/source consumption. A failed or interrupted precommit action adds no formula and consumes no scroll. Do not recreate a lost formula from loadout metadata or a memorised copy.

### 9.4 Book policy applies to preparations, not all magic

Book loss does not prevent selected at-will cantrips or unrelated capabilities. `EveryRefresh`/`PatternChangesOnly` applies only to the book-derived subpatterns described in section 6. Both modes must be configurable and tested in the same engine build.

---

## 10. Scroll inscription and production costs

### 10.1 Item model

Introduce `ISpellScroll`, `SpellScrollGameItemComponent` and builder prototype token `spellscroll`. One instance is either a blank suitable for inscription or a single charged stored casting. It is not rechargeable after use and cannot contain multiple spells.

The prototype configures eligible schools/spells or an eligibility prog, operation durations, presentation and production inventory-plan templates. The spell/slot/power/potency payload is entirely instance data. A newly created prototype instance must not inherit another scroll's charge.

Disallow combinations that make destructive consumption ambiguous or duplicate charges: stackable/quantity-splitting charged items and container-held contents within a scroll item are not supported. A scroll can be stored inside an ordinary container; it must not itself become a container whose unrelated contents are destroyed on use. Audit morph, copy, split, merge and item-removal paths for conservation.

### 10.2 Choosing a casting to store

Inscription takes an explicit capability, repertoire, allowance and spell. For Memorised, choose an available prepared copy; it fixes effective level/power. For Spontaneous, choose an available slot and a selected spell. For AtWill, choose an eligible selected spell at its base level without a finite debit.

A higher-level finite casting of an otherwise at-will spell is legal when the allowance permits it, and spends the higher-level casting. The normal at-will version requires production costs but no fictitious slot.

The caster must currently have this route. A book formula without preparation does not permit a Wizard to inscribe a finite casting. Another character cannot pay the slot while a second author's traits supply potency in this version.

### 10.3 Fully prepaid contract

At inscription, pay:

- The selected prepared copy or spontaneous slot, when finite.
- The spell's configured resource costs and consumable material requirements.
- Additional builder-authored scroll production requirements.

At activation, none of those costs is repeated. Non-consumed tools/foci required by the inscription plans must be available for production but are not magically transferred into the scroll. Normal spell component requirements are considered prepaid at release; the scroll's own reader/manipulation/activation requirements remain enforceable.

Merge production and spell-material requirements into one feasibility/reservation plan so the same unit of material cannot satisfy two independently checked costs and then be spent twice or underpaid. Support existing plan tool-use/consumption semantics rather than adding a currency-only shortcut.

Costs and durations can vary by spell and stored level using supported builder formula/prog inputs. A blank scroll, ink, tools and expensive components are builder choices. No stock prices, universal coin charge or mandatory recipe list is introduced.

### 10.4 Timed transaction

For clarity, the normal player inscription operation performs its timed work **before irreversible commitment**:

1. Preflight the route, blank, snapshot compatibility and combined costs.
2. Reserve the blank and selected finite entry; start the timed work.
3. Interrupt/cancel before completion: release reservations, grant no charge, retain the unspent slot and unconsumed materials.
4. At completion, revalidate access, source entry, costs and relevant object/state versions.
5. Capture creator numerical potency, commit the personal expenditure/material debit and write one charged payload as one guarded, durable operation.
6. Apply applicable spell lockouts and emit inscription-specific output; complete the operation.

This completion-time boundary is an engineering resolution of the approved prepayment model. There is no intermediate usable scroll and no repeatable free charge. A script/craft adapter performing an immediate completion must use the same validation/debit/charge service, not set XML directly.

Never execute target or caster spell-effect lists during inscription. Do not target a dummy creature, heal the author, summon an NPC, or execute a spell's `executeprog` effect as part of capturing a formula. Ordinary spell casting emotes that describe the released spell must not be reused for inscription. Record production/magical action attribution once using appropriate existing hooks rather than creating multiple world events for one operation.

### 10.5 Production integration

Expose safe service/FutureProg entry points for beginning inscription and for a trusted crafting completion to commit it. A crafting caller must provide an engine-issued reservation/operation token, proving which casting and blank were reserved. It cannot claim that slot costs were prepaid through a boolean argument.

The standard timed player command and crafting adapter share the same core service. A builder can price production through the item/operation inventory plan or a craft, but cannot accidentally produce a charge without the core slot and spell-material debit. Document which external craft inputs are already consumed by the craft and which spell inputs the service still requires, avoiding duplicate charging.

An optional `CanInscribeScrollProg(actor, capability, spell, blank, castingLevel)` can prohibit production. An optional notification runs only after success, following the callback discipline; it is not responsible for charging slots. No automatic stockpile cap or expiry is added. Stored castings can accumulate across recovery cycles, intentionally balanced by production costs.

---

## 11. Scroll activation

### 11.1 Eligibility

A normal reader must possess an applicable Vancian capability for the scroll spell's school. They need not have the spell selected, prepared, in a book or currently covered by an unspent slot. A mundane non-caster cannot activate it. The prototype and optional capability `CanActivateScrollProg(actor, capability, spell, scroll)` may impose additional restrictions.

Apply ongoing hard spell prohibitions and the scroll's supported-reader/physical restrictions, but do not accidentally require its source author's repertoire or `SpellKnownProg` to return true. Do not require candidate membership that depends on the reader already being high enough level: that would eliminate the agreed over-level attempt.

The scroll must be accessible and usable through ordinary item rules. Merely reading its description or inspecting it does not consume it. The explicit activation command selects targets through the stored trigger; it does not target whichever person was present at inscription.

### 11.2 Normal casting ceiling

Compute the reader's normal finite casting ceiling from active, structurally valid finite allowances with a current slot count greater than zero, without subtracting spent slots. An allowance that cannot normally support any permitted spells is not a source of arbitrary ceiling values.

For a scroll of a spell specifically usable through an active at-will route, that route additionally supports that spell at its own base level. It does not establish unlimited capacity for other spell levels or stored upcasts. Do not interpret missing/zero finite capacity as an infinite ceiling.

Use the greater applicable normal ceiling from the selected reader capability. Other capabilities cannot silently lend their maximum level; the user may explicitly select a different eligible capability instead.

Compare against the scroll's **stored casting level**. A level-one spell inscribed in a level-five slot requires level-five handling, even when the enum power has saturated. Slot exhaustion does not lower the normal ceiling.

### 11.3 Over-level check

Within normal capacity, there is no additional scroll-control check. Above capacity, perform one check for the reader using a configured trait and difficulty.

Configuration:

| Field | Meaning |
|---|---|
| `ScrollCheckTrait` | Trait used to control an over-level scroll; defaults to the capability's configured concentration trait. |
| `ScrollDifficultyProg` | Optional number-returning `(actor, capability, spell, storedLevel, normalMaximumLevel)` producing a valid `Difficulty` value. |
| `ScrollMinimumOutcome` | Successful outcome threshold; default MinorPass. |
| `CanActivateScrollProg` | Optional additional eligibility predicate, independent of known selections and available slots. |

Without a difficulty prog, use the existing difficulty scale: Normal for one level above capacity, one harder stage per further level, capped at Impossible. This is a configurable engine default, not an asserted tabletop rule. Reject invalid/null configured prog results before commitment rather than converting errors into automatic success. Reuse the existing check machinery and an appropriate existing casting check contract; do not introduce a new check ID that existing worlds cannot load without separately seeded check data. Document the chosen check type and ensure the reader's live trait is used only for this activation-control check.

A failed check fizzles the scroll with no spell effects. No mishap table or extra backlash is included. It is distinct from target resistance, which is resolved afterwards using stored creator potency.

### 11.4 Destructive commitment

Resolve the target, validate the stored payload and references, test normal action/lockout conditions, and reserve the exact charged scroll before irreversible use. Invalid target, incompatible reader, corrupted/unsupported snapshot, missing runtime references, or inability to manipulate/read the item are preflight refusals and leave the scroll intact.

At commitment, make the charge irreversibly spent and persist that state, then destroy the item through normal item-removal hooks. Hold the already-validated immutable invocation context in memory to resolve the effect afterwards. Do not leave a live scroll reference as the only owner of numerical data needed by a persistent effect.

Committed over-level check failure, target resistance, ward rejection and ordinary effect non-applicability all leave the scroll destroyed. A second activation or simultaneous transcription cannot reserve the same charge. Do not reroll until success while preserving the item.

Activation spends no slot, repeats no spell resource/material costs, and does not require the author's original non-consumed production tools. It still applies the spell's normal casting cooldowns once and the reader's explicit activation/physical requirements. Use the reader for crime responsibility, target selection, effect ownership and communication. Do not attribute the release as an active command by the absent creator.

If a process crashes after durable charge consumption but before applying all effects, the charge remains spent and effects are not replayed on restart. The guarantee is no duplicate/free casting, not fictional exactly-once replay of arbitrary world side effects. This failure must be diagnosable; do not claim a general database transaction can atomically encompass all spell effects and external prog actions.

---

## 12. Stored creator potency and spell-snapshot compatibility

### 12.1 Required separation

Scrolls must implement the approved stored-spell model, not merely hold `(SpellId, SpellPower)` and call `CastSpell(reader, ...)`.

| Frozen at inscription | Resolved live at activation |
|---|---|
| Source spell ID and base spell level | Actual reader/acting instance and owner identity |
| Stored casting level and final power | Actual targets and trigger-supplied exit/room parameters |
| Effective creator caster level | Target resistance, current wounds, armour and target-state applicability |
| Reliable source casting outcome and needed degree inputs | Current wards, source/target location, plane and visibility |
| Creator-dependent numerical formula bindings and relevant modifiers | Physical ability to activate and over-level scroll-control check |
| Spell/trigger/effect formula configuration needed for the stored casting | Reader ownership of self effects, summons, portals and concentration |
| Required casting timing/cooldown payload settings | Current legitimate runtime entities referenced by templates |

The creator ID may be retained as inert provenance, but must not be used to load a live creator in order to activate the scroll. The creator may be offline, changed, dead or deleted. A reader's unusually high or low magical statistics must not alter the stored magnitude/duration/resistance potency.

### 12.2 Numeric context, not a fake character

Introduce a typed immutable numerical context, such as `ISpellNumericalContext`/`StoredSpellNumericalContext`, alongside the actual actor. It must resolve numeric expression bindings from a snapshot while the real reader remains the actor for all world interactions.

Do not create a dummy `ICharacter` impersonating the creator, temporarily overwrite the reader's traits, mutate shared formula parameters on the spell, or install an unscoped global override. Nested spells and concurrent invocations must remain isolated.

`TraitExpression` accepts trait owners, variable traits, bonus contexts, named variables and extended options. Capture relevant creator-bound values per expression location/bonus context, including the special `variable` value and configured race/culture/role/merit numerical options. The same trait can have different effective values in different bonus contexts; a flat trait-ID-to-number map alone is not sufficient.

Implement a central spell formula evaluation/capture path with two modes: live numerical context for ordinary casts and frozen bindings for scrolls. Preserve dynamic variables such as per-target opposed `outcome`; they must be supplied at activation, not precomputed against a dummy target at inscription. Preserve existing meaning of `power`, `degrees` and `success`. Expose additional named `spelllevel`, `castinglevel` and `casterlevel` values to this feature's supported expression contexts with explicitly documented meanings; do not change legacy formula binding behaviour.

For reliable casting, store the synthetic successful casting outcome and the helper-derived degree inputs. Use that saved casting result against each target's live resistance. The over-level reader check must not replace the stored offensive outcome with the reader's roll.

### 12.3 Snapshot payload

Store a versioned payload with at least:

- Unique charge/operation identifier and immutable snapshot schema version.
- Logical spell ID, school ID, source-definition fingerprint, source base level, stored casting level and power.
- Relevant spell metadata, trigger/effect configuration, ordered effect keys and expression texts, including target and caster lists.
- Reliable source outcome inputs, effective creator caster level, and typed numerical bindings keyed by expression/effect location and context.
- Required reference identifiers and compatibility-adapter/version information.
- Creator identity/provenance, inscription time, and current item operation/charge state.

Use canonical serialisation for fingerprints and values, culture-invariant numeric XML, finite numeric validation and explicit schema versions. A source spell's later formula edits must not silently rewrite an existing charged scroll. Spell templates used for resolution must be instantiated from the snapshot without adding a new permanent spell record to the global catalogue.

Logical spell ID remains available for dispelling/classification and staff inspection. Persist the snapshot or a durable snapshot reference with any retained parent/child effect that will need its bindings after reload. Reloading such an effect must not fall back to the live creator or the source spell's newly edited numeric formula. The effect's caster reference is the reader.

Normal referenced game entities (for example an item prototype used by a conjuration) are still references, not complete frozen copies of the world. Validate that they exist and are usable. A source spell's explicit revocation/deletion or a required incompatible runtime type makes the scroll inert with a diagnostic; do not silently substitute another ID or effect type. Provide a builder-controlled scroll-eligibility/revocation flag on the spell, default false for existing spells, and include it in `magic spell set scroll <true|false>`.

### 12.4 Compatibility is explicit

Build a scroll compatibility analyser and support manifest. An effect can be scrollable only if its creator-dependent numerical work is captured correctly and its live actor/target requirements are defined. `executeprog` and arbitrary custom effects are not automatically snapshot-safe merely because they accept an actor parameter.

The analyser must inspect both target and caster effects, trigger additional parameters, duration expressions and nested configurations. It must report individual unsupported components before costs are paid. Unknown/new effect types default to unsupported until an adapter is supplied. The builder must see the exact reason in `magic spell show/check` and a scroll-specific compatibility inspection.

This must not become an excuse to mark all useful effects unsupported. Minimum required supported families are ordinary character damage and healing, applicable self/caster effects, trait boosts, light/glow, basic invisibility and senses, ordinary timed statuses/removals, spell armour, and effects using ordinary room/exit/character target contexts where their existing contracts are sufficient. Also prove a representative actor-owned conjuration/movement effect to demonstrate that reader ownership and frozen numbers are separated.

For every registered effect type, ship an inventory row classifying it as supported, actor-context-only supported, or unsupported with a concrete reason. Cover every supported numeric evaluation field, not a handpicked subset of one class. Programmatically test that no registered type is missing from the inventory. Any mandatory family above found to need a small adapter must receive that adapter in this assignment.

Custom script effects can later opt into a typed snapshot adapter. Do not claim to freeze arbitrary FutureProg behaviour by saving its ID or by evaluating it during inscription. Formula capture must not execute target/caster spell effects, source-changing progs, random gameplay, or NPC/item creation.

### 12.5 Randomness and ongoing effects

Capture bindings/configuration, not an evaluated damage result for an unspecified future target. Random formula terms retain their authored evaluation timing at release or subsequent ticks. Do not run them repeatedly during a preview to select the best result. Any creator-dependent numeric input used by a later tick must remain frozen; actual target state and authored randomness remain live.

A scroll does not waive existing caster-linked/concentration requirements. The reader must satisfy and own those relationships. Where an effect lacks a supported way to perform that transfer while retaining numerical potency, refuse its scroll compatibility rather than turn it into a free permanent effect.

---

## 13. Player and staff command contract

### 13.1 Command namespace

Use existing school routing for capability-local actions:

```text
<schoolverb> vancian <capability> <operation> ...
```

A capability may be selected by unique name or ID. Alias convenience is acceptable, but the full form must always work and appear in help. Only capabilities applicable to the actor and selected school can be used. Use `StringStack`/`PopSpeech` for quoted spell, plan and item names. Do not overload trailing target text with ambiguous slot/power options.

The canonical command forms below are normative. Optional concise aliases may be added without replacing them. Player-facing alias names refer to stable keys resolved before processing.

### 13.2 Capability commands

| Suffix after `<schoolverb> vancian <capability>` | Meaning |
|---|---|
| `status` | Caster level, rule/allowance summaries, known limits, current prepared/spontaneous/at-will availability, recovery and errors. |
| `spells [<repertoire>] [<level>]` | Browse permitted candidates with selected/book/prepared/castable markers; never includes hidden payloads. |
| `spell <spell>` | Spell help with base level, permitted routes and computed powers. |
| `known show` | Committed selected repertoire grouped by rule and level. |
| `known begin` | Start a draft of all Selected rules from committed state. |
| `known add <repertoire> <spell>` | Add to draft; no live change. |
| `known remove <repertoire> <spell>` | Remove from draft; no live change. |
| `known draft` | Show old/proposed differences and limit validation. |
| `known commit` | Validate and commit through the two policy hooks. |
| `known cancel` | Discard draft. |
| `loadouts` | List named plans and mark selected-next versus last-committed name for information. |
| `loadout new <name>` | Create an empty plan. |
| `loadout show <name>` | Inspect planned assignments and current validation. |
| `loadout copy <source> <newname>` | Duplicate a plan, not charges. |
| `loadout rename <oldname> <newname>` | Rename a plan without altering current memorisation. |
| `loadout assign <name> <allowance> <ordinal> <repertoire> <spell>` | Assign one memorised slot in a plan. |
| `loadout clear <name> <allowance> <ordinal>` | Make one planned slot empty. |
| `loadout select <name>` | Choose the next plan; no refresh. |
| `loadout validate <name>` | Full side-effect-free validation. |
| `loadout delete <name>` | Delete with normal confirmation; retain the independent committed pattern. |
| `prepared` | Show current copies, ordinals, powers, spent/suspended state and last pattern. |
| `refresh [last]` | Start/perform the configured refresh using selected-next plan or explicitly last pattern. |
| `cancel` | Cancel this actor's current preparation/inscription/transcription operation where applicable; no reversal of a committed cast. |
| `cast <repertoire> <allowance> <spell> <ordinal\|next\|atwill> [target arguments]` | Explicit route, fixed power. `next` selects the lowest available suitable ordinal in that one finite allowance only. |

For spontaneous slots, `next` chooses a slot in the explicitly selected allowance. For at-will, require the `atwill` marker. For memorised slots, the specified spell/rule must match the prepared copy. Do not silently upcast, downcast or switch routes on failure.

### 13.3 Item commands

```text
spellbook show <book>
spellbook copy <capability> <source-book-or-scroll> <spell> <destination-book>
spellscroll show <scroll>
spellscroll inscribe <capability> <repertoire> <allowance> <spell> <ordinal|next|atwill> <blank-scroll>
spellscroll cast <scroll> <capability> [target arguments]
```

Resolve capability independently of a school verb for item commands, then validate school compatibility. If a command name conflicts in the current repository, use a single documented namespaced alternative consistently and retain these operations; do not hijack an existing unrelated command.

`show` is informational. A scroll shows its spell, base and stored casting levels, power, compatibility, and whether the viewer faces an over-level check where their permitted knowledge/readability allows inspection. It must not dump private numerical bindings or creator secrets to arbitrary viewers. Detailed snapshot diagnostics are staff-only.

### 13.4 Staff commands

Add an administrative `magic vancian` inspection/mutation family:

```text
magic vancian show <character> <capability>
magic vancian known grant|revoke <character> <capability> <repertoire> <spell>
magic vancian refresh <character> <capability>
magic vancian reset <character> <capability>
magic vancian operations <character> [<capability>]
magic vancian resolve <operation-id> <acknowledge|cancel>
magic vancian book add|remove <book> <spell>
magic vancian scroll show <scroll>
```

Use proper permissions, audit attribution and confirmation for destructive resets/forced operations. Administrative grants may bypass player permission hooks deliberately, but not structural validation or corruption checks. They must not notify a builder's daily-change hook unless the command explicitly requests a policy-respecting normal commit. Spellbook add/remove is an explicit staff authoring operation, not a way players bypass transcription.

Operation repair never grants a replacement casting merely by acknowledging a failure. A separate explicit staff restore/reset is required when intended. Warn when resolving an indeterminate callback requires the builder to repair its register-variable side effects.

### 13.5 Output quality

All output must distinguish: candidate versus selected; saved plan versus current memorisation; finite availability versus at-will; missing book versus unknown formula; insufficient level versus exhausted slots; preflight refusal versus spent failed scroll. Include exact usable syntax and relevant school/capability names in errors.

Use existing colour/localisation/table helpers, readable wrapping, and builder-authored emotes where relevant. Empty descriptions, placeholder emotes and documentation-only commands do not meet acceptance.

---

## 14. Builder contract and configuration validation

### 14.1 Capability authoring

Retain the normal `magic capability` editable-item workflow. Add builder creation equivalent to:

```text
magic capability edit new vancian <name> <school> <concentration-trait>
```

The following `magic capability set` settings must have executable handlers, show output, detailed help and round-trip persistence:

| Setting | Meaning |
|---|---|
| `casterlevel <prog>` | Set numeric progression provider. |
| `canchangeknown <prog>` | Set selected-repertoire permission hook. |
| `onchangeknown <prog\|none>` | Set/clear post-change callback. |
| `cancast <prog\|none>` | Set/clear hard casting permission. |
| `basepower <power>` | Set Vancian base power. |
| `upcaststep <integer>` | Set power increment per excess slot level. |
| `reliableoutcome <MinorPass\|Pass\|MajorPass>` | Configure successful base resolution outcome. |
| `maxloadouts <count>` | Configure saved-plan limit. |
| `repertoire add <alias> <Selected\|Spellbook>` | Add a new rule with generated immutable key and initially incomplete settings. |
| `repertoire <alias> name <text>` | Rename only. |
| `repertoire <alias> levels <minimum> <maximum>` | Set candidate base-level bounds. |
| `repertoire <alias> candidates <prog>` | Set candidate predicate. |
| `repertoire <alias> limit <prog>` | Set Selected per-level selection count. |
| `repertoire <alias> bookpolicy <EveryRefresh\|PatternChangesOnly>` | Set Spellbook recovery rule. |
| `repertoire remove <alias>` | Confirm removal; show affected allowance/state references. |
| `allowance add <alias> <Memorised\|Spontaneous\|AtWill> <slotlevel\|none>` | Create allowance. |
| `allowance <alias> name <text>` | Rename only. |
| `allowance <alias> repertoire add\|remove <repertoire>` | Change explicit links. |
| `allowance <alias> levels <minimum> <maximum>` | Set base-spell-level bounds. |
| `allowance <alias> eligibility <prog\|none>` | Set optional extra spell predicate. |
| `allowance <alias> count <prog>` | Set finite capacity provider; rejected for AtWill. |
| `allowance remove <alias>` | Confirm removal; suspend historical references rather than retarget them. |
| `recovery mode <PreparationAction\|SleepAutomatic\|SleepThenPreparation>` | Configure refresh arrangement. |
| `recovery preparetime <timespan>` | Set timed preparation duration. |
| `recovery sleeptime <timespan>` | Set observed sleep requirement. |
| `recovery interval <timespan>` | Set minimum interval between successful refreshes. |
| `recovery canrefresh <prog\|none>` | Add permission/temporal restriction. |
| `recovery onrefresh <prog\|none>` | Set post-refresh callback. |
| `bookuse <prog\|none>` | Additional usability restriction for accessible books. |
| `transcribe <prog\|none>` | Additional transcription permission `(actor, capability, spell, source, destination)`. |
| `inscribe <prog\|none>` | Additional inscription permission. |
| `scrolluse <prog\|none>` | Additional activation permission. |
| `scrolltrait <trait>` | Over-level reader check trait. |
| `scrolldifficulty <prog\|none>` | Difficulty calculation override. |
| `scrollthreshold <successful-outcome>` | Minimum activation-control outcome. |
| `check` | Enumerate all configuration errors without database mutation. |

Existing applicable concentration, regenerator and inherent-power editing must remain supported. The new commands may delegate to shared helpers rather than create a monolithic switch.

Fields controlling destructive structural changes should require confirmation and show their state-suspension consequences. Invalid intermediate builder definitions may be saved for editing, but cannot grant functional access until ready; world startup should report a disabled invalid capability rather than crash on an optional incomplete stock-free definition. Corrupted persistence still needs actionable diagnostics, not silent data substitution.

### 14.2 Item prototype authoring

Under the usual item-component prototype editor, implement these settings for `spellbook`:

- Finite non-negative formula capacity, with an explicitly documented unlimited-capacity option if provided (never confused with casting slots).
- Optional accepted-formula prog `(book, spell)` and usable-book prog `(actor, book)`.
- Transcription duration expression using at least `spelllevel` and `casterlevel`.
- Transcription inventory plan, using existing plan editing/serialization.
- Start, completion and cancellation emotes.

For `spellscroll`:

- Blank-item inscription eligibility `(actor, spell, storedLevel)` and optional reader/readability restriction.
- Inscription duration expression using `spelllevel`, `castinglevel` and `casterlevel`.
- Additional inscription inventory plan.
- Start, completed, cancelled, successful-release and fizzle/destruction presentation.
- Snapshot/instance status display; no prototype spell or charge default.

Destination book prototype owns the copying time/cost definition; blank scroll prototype owns its additional production time/cost definition. Capability progs may veto an action but do not ambiguously replace those cost owners. Spell costs remain on the spell and are added to scroll production as specified.

Reuse normal physical item components where meaningful. Do not require an ordinary Book component to parse formulae, and do not make it the authoritative formula store. All proto settings must clone/load/save correctly, while mutable formulae and stored casts remain on instances.

### 14.3 Emote contracts

Document and validate available perceivables for every new emote. Suggested stable conventions: `$0` actor, `$1` destination/scroll, `$2` source book/scroll where present. Use literal formatter fields for spell names/levels only through a safe supported formatting path; do not interpret user spell/loadout names as emote markup.

Do not announce a spell effect while preparing or transcribing. Private numerical state stays in personal output. Normal casting target echoes and resistance output are emitted by the spell resolver at release, not duplicated by the item command.

### 14.4 Readiness checks

Reject or disable configurations with missing progs, invalid return/parameter types, unresolved rule links, negative/non-finite counts, incompatible source/mode pairs, invalid powers, impossible duration values or scroll snapshot incompatibility. Show errors by component/rule/allowance name and stable key.

New capability defaults must be safe and visibly incomplete: no all-spells candidate grant and no all-characters access. Sample documents may provide complete fixture progs. Runtime diagnostics must not rely on the reader inspecting raw XML.

---

## 15. FutureProg and service API

### 15.1 Configuration hooks

Use existing Character, MagicCapability, MagicSpell, Item, Number, Boolean, Void and spell-collection types. There is no need to consume new enum bits for every internal DTO.

| Hook | Return | Parameters |
|---|---|---|
| Caster level | Number | owner, capability |
| Candidate | Boolean | owner, capability, spell |
| Selection limit | Number | owner, capability, casterLevel, spellLevel |
| Finite slot count | Number | owner, capability, casterLevel, slotLevel |
| Allowance eligibility | Boolean | owner, capability, spell |
| Can change known | Boolean | owner, capability, currentSpells, proposedSpells |
| On changed known | Void | owner, capability, previousSpells, newSpells |
| Can cast | Boolean | actor, capability, spell |
| Can refresh / on refresh | Boolean / Void | owner, capability |
| Can use book | Boolean | actor, capability, book |
| Can transcribe | Boolean | actor, capability, spell, sourceItem, destinationBook |
| Can inscribe | Boolean | actor, capability, spell, blankScroll, castingLevel |
| Can activate scroll | Boolean | actor, capability, spell, scroll |
| Scroll difficulty | Number | actor, capability, spell, storedLevel, normalMaximumLevel |

`owner` means canonical identity character for stable register/progression policy; `actor` means the actual operating body. State that distinction in in-game prog help. An accepted overload may add context but must not replace the required forms above. Validate exact signatures rather than accepting arbitrary parameters as a workaround.

### 15.2 Query functions

Implement and document typed queries equivalent to:

```text
vanciancasterlevel(character, capability) -> number
vancianknownspells(character, capability) -> collection<spell>
vancianknownspells(character, capability, repertoireAlias) -> collection<spell>
vanciancandidates(character, capability, repertoireAlias) -> collection<spell>
vancianpreparedspells(character, capability) -> collection<spell>
vancianslotsremaining(character, capability, allowanceAlias) -> number
vancianslotcapacity(character, capability, allowanceAlias) -> number
vancianisatwill(character, capability, allowanceAlias) -> boolean
vancianselectedloadout(character, capability) -> text
vanciancanrefresh(character, capability) -> boolean
vanciancancast(character, capability, spell, repertoireAlias, allowanceAlias) -> boolean
spellbookspells(item) -> collection<spell>
scrollspell(item) -> spell or null
scrollcastinglevel(item) -> number
scrollpower(item) -> number
scrollischarged(item) -> boolean
```

Prepared-spell query returns one entry per unspent prepared copy; document duplicates. Known-spell query returns distinct committed entries, with a separate active/suspended inspection when needed. Slot remaining for AtWill must not return a misleading infinite/negative number: reject that finite query with the documented sentinel zero and use `vancianisatwill` to distinguish it. Prefer an explicit availability DTO in C#.

Queries do not create ledgers, roll checks, invoke notification hooks, consume qualification or start actions. Formula/repertoire candidate predicates are builder-required pure queries. New active/known queries must not recursively call themselves through generic `CharacterKnowsSpell` aggregation.

### 15.3 Mutation entry points

Provide supported normal-policy and explicit trusted-administration operations. Required normal-policy operations include committing a whole selected repertoire, selecting a saved loadout, requesting refresh, starting transcription, and starting inscription. The C# service accepts typed assignments/keys; a FutureProg wrapper may use an existing collection-dictionary representation for per-rule spell collections.

Illustrative public names:

```text
setvancianknownspells(character, capability, selectionsByRepertoire) -> boolean
selectvancianloadout(character, capability, name) -> boolean
refreshvancian(character, capability) -> boolean
copyspellformula(actor, capability, source, spell, destination) -> boolean
inscribespellscroll(actor, capability, repertoireAlias, allowanceAlias, spell, slotOrdinal, blank) -> boolean
```

The boolean means accepted/started for timed operations, not that production already finished. Document this explicitly and expose operation status to callers. At-will uses a separate validated mode/overload, not a negative slot index magic value.

Trusted initialization/grant helpers must use a clearly administrative API and record their bypass. Do not expose an innocently named helper that silently ignores `CanChangeKnownSpells`, refills slots or charges a scroll for free. Script operations that change actual gameplay state must call the same services as player commands; raw register variables store builder policy, not the authoritative slot ledger.

Register variables used by hooks remain normal register variables. The engine must not reserve names such as `LastClericSpellChange` or silently register/update them on behalf of a class. The full builder examples show how to create them.

---

## 16. Required worked examples and expected gameplay

These examples use illustrative spell names and small fixture values. They define behaviour, not a mandatory stock catalogue.

### 16.1 Mixed Wizard with at-will cantrips

Configure a Wizard capability with Selected cantrips (limit 2 at level zero), a Spellbook rule for levels 1–6, an AtWill cantrip allowance and memorised level-one/level-two allowances. Link cantrips to the level-one allowance as well to demonstrate permitted finite upcasting. Candidate progs explicitly permit fixture spells rather than all zero-level database rows.

The Wizard selects Light and Spark as cantrips. Light is cast at will at Standard power. They create a plan with two Sleep copies in first-level slots and Shield in a second-level slot. The plan display shows Shield's computed upcast power. They successfully refresh using a borrowed book containing Sleep and Shield. Two Sleep casts spend two copies, while Light remains at will.

Selecting another plan does nothing immediately. `prepared` still shows the old spent pattern. `refresh last` with PatternChangesOnly can restore the identical book subpattern without the book. Changing Sleep to another book-derived spell requires its accessible formula. Switching to EveryRefresh makes even the unchanged recovery require the book.

### 16.2 Known-selection hook

A Cleric makes a single draft changing spells across several levels, then commits. The permission hook runs once for the whole change; the post-change hook records one timestamp. Another attempted change inside the configured interval is refused. Reordering or reconfirming the same per-rule sets is a no-op and does not reset the timestamp. Removing/reacquiring the capability or switching to another body does not bypass it.

A separate Sorcerer uses a permission prog that allows an empty initial repertoire but no later replacements. Their spontaneous casts spend slots without preparing a specific spell. A staff-granted fixed repertoire also works with an always-false player permission prog.

### 16.3 Upcast cantrip and scroll

The Wizard memorises Light in a first-level slot using the cantrip repertoire. It is a finite Strong casting, separate from at-will Standard Light. Inscribing it consumes that prepared copy, spell materials and the blank's production requirements, producing one Strong scroll. Inscribing ordinary at-will Light instead costs production/spell materials but no finite slot.

Neither operation produces light during inscription. A later reader releases the scroll without losing a personal slot or supplying the prepaid spell materials. The scroll is destroyed.

### 16.4 Learning versus casting a scroll

A compatible Wizard obtains a charged scroll of a new higher-level spell. They may attempt to release it using the over-level rule, or transcribe its formula into a book at the configured copying cost. Transcription requires no personal slot or over-level activation check and produces no spell effects. Successful transcription destroys the source scroll. The copied formula remains at the spell's base level.

### 16.5 Stored potency and exhaustion

Creator A inscribes a damage spell whose formula depends on power, an author trait and a per-target outcome. Reader B has different traits and no remaining daily slots. B may activate within normal level capacity. Damage uses A's captured numerical trait bindings and stored power with the target's live opposed outcome, not B's traits. A being offline/deleted does not prevent activation.

A third-level caster attempting a fifth-level stored casting makes the configured over-level check even if the underlying spell is only first-level. Failure destroys the scroll without effects. A ward or target resistance on success also does not restore it. A pure invalid-target refusal leaves it intact.

### 16.6 Recovery and body ownership

An identity uses a prepared copy from one instance. Another instance immediately sees it spent. Sleeping the primary while actively controlling an awake projection earns no simultaneous refill. Restarting does not reset the ledger, replay a callback, re-charge a scroll, or turn offline elapsed time into qualifying sleep.

### Documentation quality for examples

The implementation must supply an end-to-end builder walkthrough using real syntax and compiler-tested progs, with all prerequisites identified. Illustrative snippets in this design are not proof those functions/commands already exist. The final walkthrough must be exercised against a disposable local world or reported unexecuted with the precise environmental blocker.

---

## 17. Implementation architecture and work breakdown

### 17.1 Suggested components

Names are recommended; responsibilities are required.

| Component | Responsibility |
|---|---|
| `IVancianMagicCapability` / `VancianMagicCapability` | Configured capability, factory registration, builder workflow, XML and readiness. |
| `VancianRepertoireDefinition` / `VancianCastingAllowanceDefinition` | Stable-key immutable configuration records with explicit source/mode semantics. |
| `IVancianMagicService` | Access route resolution, known-selection commits, plan validation, slot reservation/spending, refresh and operation coordination. |
| `VancianCapabilityState` | Identity-owned authoritative state, versioned serialization, dirty tracking and concurrency version. |
| `VancianRefreshAction` / qualification tracker | Timed preparation, observed sleep accounting and cancellation using existing action/scheduler infrastructure. |
| `ISpellbook` / spellbook component/prototype | Instance formulae, accessibility, copying configuration and display. |
| `ISpellScroll` / scroll component/prototype | Blank/charged/spent state, stored snapshot and single-use conservation. |
| `SpellTranscriptionService` / `ScrollInscriptionService` | Shared player/script/craft workflows and guarded commits. |
| `SpellInvocationContext` / resolver adapters | Real actor, explicit cost source, targets and numerical context without duplicated spell resolution. |
| `ISpellNumericalContext` / snapshot evaluators | Live-versus-frozen formula binding and outcome inputs. |
| `ScrollSpellCompatibility` / adapters | Explicit support inventory and per-template capture/application semantics. |
| New command/prog handlers | Thin adapters into the services; no competing slot logic. |

Shared contracts belong in `FutureMUDLibrary`; concrete runtime services in `MudSharpCore`; persistence-only records/mappings in `MudsharpDatabaseLibrary`. Use partial files or domain subfolders to keep large command/runtime classes manageable.

### 17.2 Implementation sequence

1. Read source/instructions; record actual checkout SHA and existing spell-resolution tests. Add the feature document before coding.
2. Implement shared definitions, validation, `vancian` registration, and `SpellLevel`/scroll eligibility metadata with migration tests.
3. Implement identity-owned state and slot generation/concurrency services with deterministic tests.
4. Implement selected repertoires/hooks, saved plans, qualification and refresh.
5. Implement spellbook component, access, transcription and persistence.
6. Integrate Vancian access/casting routes across commands, helpers and legacy bridges.
7. Implement numerical snapshot/effect support adapters and their complete coverage inventory.
8. Implement scroll component, inscription/activation/craft integration, durability and cleanup.
9. Complete player/builder/FutureProg surfaces, help, examples and integration tests.
10. Run verification gates and deliver the full feature with a requirement-to-test report.

Use reviewable commits. Do not call the feature complete after only adding slot storage while leaving scroll potency, sleep, commands or hooks as TODOs. Do not expand into unrelated spell catalogue or economy work.

### 17.3 Existing code touchpoints to inspect

The reviewed files confirm the central capability, command, expression and identity interfaces; see sources S1–S9. Inspect their surrounding implementations in the actual checkout before editing.

- `FutureMUDLibrary/Magic/IMagicCapability.cs`, `IMagicSpell.cs`, `SpellPower.cs`, trigger/effect contracts.
- `MudSharpCore/Magic/Capabilities/SkillLevelBasedMagicCapability.cs`, `MagicCapabilityFactory.cs`, `MagicSpell.cs`, `SpellPowerInvocation.cs`, and `Powers/SpellBackedPower.cs`.
- `MudSharpCore/Commands/Modules/MagicModule.cs` and `Commands/Helpers/EditableItemHelperMagic.cs`.
- `MudSharpCore/Character/CharacterMagic.cs`, identity/instance ownership and load/save facilities.
- `FutureMUDLibrary/Character/ICharacterInstance.cs` and canonical identity comparer utilities.
- `MudSharpCore/Body/Traits/TraitExpression.cs`, `FutureMUDLibrary/Body/Traits/ITraitExpression.cs`, and each supported spell-effect numerical evaluation site.
- `MudSharpCore/Effects/Concrete/MagicSpellParent.cs` and spell-owned persistent child serialization.
- Existing item component/prototype manager, inventory plans, manipulation/access, timed actions, sleep/wake and crafting extension points.
- Existing magic FutureProg functions and variable register integration.
- `MudsharpDatabaseLibrary/Models/MagicSpell.cs`, capability model, character/item models and EF mappings/migrations.

The existing school command currently calls `SpellKnownProg` directly in cast/help paths, while ordinary spell listing has its own filtering. These call sites need a shared route-aware implementation; changing only `CharacterKnowsSpell` is not sufficient. The existing trait evaluator resolves trait and extended-option values from its owner, so storing only a scroll's `SpellPower` is also insufficient. [S2, S3, S4]

---

## 18. Persistence, migrations and operation integrity

### 18.1 Schema boundaries

Use the existing capability record for versioned definition XML. Add `SpellLevel` (integer, non-null, database default 0) and a scroll-inscription eligibility/revocation flag (boolean, default false) to spell persistence. The latter flag is an explicit opt-in, not a replacement for snapshot compatibility checks.

Recommended new state table:

```text
CharacterMagicCapabilityStates
    CharacterId        bigint   -- canonical persistent identity backing key
    MagicCapabilityId  bigint
    StateVersion       bigint
    Definition         longtext -- versioned state XML
    primary key (CharacterId, MagicCapabilityId)
```

Use the actual existing identity-to-character persistence mapping after inspecting the checkout; do not invent a second identity table or attach the state to secondary body rows. One typed, versioned state aggregate is sufficient; separate relational tables for loadouts/slots are permissible if they preserve atomic aggregate transitions and unique keys.

The state payload includes selected spell entries, rule keys, plans, last pattern, finite ledger, refresh-generation metadata, UTC timestamps, completed qualifications and callback/operation status references. Store identifiers, not serialized runtime characters. Validate referenced entities on load and expose orphaned entries as invalid rather than silently remapping by name.

Spellbook formulae and scroll snapshots remain in existing item-component instance XML. Prototype revisions do not own, overwrite or broadcast that data. Add durable narrow operation records/tombstones as required to coordinate cross-entity commits; do not rely exclusively on an in-memory lock or a scroll field saved on the next periodic tick.

A suggested operation record holds an operation GUID, kind, owner/capability, affected item IDs, status, expected state version, payload/checksum, UTC timestamps and diagnostic outcome. It is a guard and repair record, not a second competing source of remaining slot counts or scroll charges.

### 18.2 Atomic transitions and crash policy

The following transitions must be coordinated:

| Operation | State that must agree |
|---|---|
| Known selection | Per-rule sets, state version and post-change hook status. |
| Refresh | New ledger, complete committed pattern, generation, qualification consumption and refresh time. |
| Direct finite cast | Reserved entry becomes spent before target-side application; no second debit after resolution. |
| Scroll inscription | Finite expenditure when applicable, required resource/material debit, blank-to-charged payload and completed operation marker. |
| Scroll activation | Charged-to-irreversibly-spent marker and item destruction before effect application. |
| Scroll transcription | Destination formula addition and source-scroll consumption/destruction. |

Follow normal `FMDB`/save-manager conventions, but explicitly inspect how affected runtime entities are persisted. A database transaction around a new state row does not by itself make an inventory plan's in-memory consumption atomic. Ensure stale dirty objects cannot subsequently overwrite a committed state version or resurrect a consumed scroll.

Before implementing the item workflows, document the concrete unit-of-work strategy and affected save paths. Use transaction-enlisted writes where available and bounded reservations/operation tombstones where necessary. A failed reservation or precommit operation grants no charge/formula and releases locks. An indeterminate **postcommit** operation is never automatically replayed into fresh effects or a second item; surface it for repair.

Do not claim global exactly-once semantics for arbitrary FutureProg, external notifications or irreversible damage. Required guarantees are: no duplicate charge, no uncharged stored spell, no simultaneous activation/transcription, no slot refill from reload, and a recoverable audit trail for incomplete commits. Ordinary runtime failures after consumption may lose a casting but must not create repeatable free ones.

### 18.3 Load/save compatibility

Unknown future schema versions must produce actionable diagnostics and disable that affected state/item rather than erase it. Missing optional new fields on legacy spells retain legacy behaviour. Old capability models must load unchanged.

Capture and persist remaining action/qualification state according to the cancellation policy. Timed player editing/production actions do not silently resume on login and complete a second time. Already earned qualification and committed state do survive. Abort stale reservations on load without minting refunds that were already committed.

Guard runtime reentrancy by owner/capability and affected item. Read-only queries from hooks are allowed; nested state mutation of the same aggregate must fail with a clear message rather than deadlock or apply two commits. Order multi-item guards consistently to avoid deadlocks during copying.

### 18.4 Migration requirements

Generate EF migrations, designers and model snapshot through EF tooling; do not hand-author generated artifacts. The repository database instructions provide the standard command:

```text
dotnet ef migrations add VancianMagic \
  --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj \
  --startup-project MudSharpCore/MudSharpCore.csproj
```

Use the checkout's actual EF tool version and platform-appropriate line continuation. Inspect generated defaults, foreign keys, collation and composite keys. Update any maintained blank-database snapshot through the repository's supported generated-delta/export workflow. Do not hand-insert a new incompatible blank schema while leaving its migration history inconsistent. [S7]

Migration must not create abilities, fill selected repertoires, annotate all old spells as cantrips, create spellbook contents or alter existing resource balances. Test both upgrading an existing schema and creating a new empty schema in disposable databases when available.

---

## 19. Compatibility and cross-cutting requirements

### 19.1 Existing magic and information APIs

Maintain the difference between knowing/about a spell and being able to cast it now. For general read-only compatibility aggregation:

- A legitimate non-Vancian route retains its existing known-spell predicate.
- A Vancian route can report a spell in an active selected repertoire, an eligible accessible book formula, or a retained valid memorisation pattern as known/inspectable through that route.
- That information never grants a usable finite copy. Current availability still requires a Prepared/AvailableSpontaneous slot or active at-will permission.
- A pattern may therefore support help/recovery information even when every copy is spent. It never becomes a transcription source.

Expose explicit capability-scoped query functions so builders do not have to infer selected knowledge from that general union. Tests must prove that `cancastspellnow` and the actual command agree on slot/material/lockout refusals while both remain side-effect free until commitment.

Do not leak hidden attack/substance payloads, private spells outside the authorised candidate/prog surface, or formulae from inaccessible books. The new candidate browser is not permission to expose all spells in the global catalogue.

### 19.2 Persistent effects and ownership

Existing effect cleanup, reflection, dispelling and target resistance must retain their semantics. Multi-target casting processes all targets independently and applies caster-side effects according to the already-corrected aggregate outcome rules. A scroll's caster is its reader even if its numerical source is someone else.

An area scroll is consumed once, not once per target. One target's rejection does not preserve it or prevent later targets from being processed. Per-target outcome is live; frozen creator values are shared by the invocation. Temporary and persistent child effects must retain their relevant numerical snapshot across save/reload without retaining the scroll item.

### 19.3 Items, action constraints and identity

Maintain normal container, ownership, accessibility, plane and room-layer restrictions. An astral or possessed actor cannot bypass item-manipulation restrictions merely because their identity owns a Wizard capability. Use actual actor context for physical work and identity context for budgets/policy.

A charged scroll may be transferred or traded without changing its stored potency. A copied item instance cannot share mutable formula/snapshot state by reference. Ordinary stack/split/morph paths may not multiply charges. Staff clones, if supported, are explicit authored duplication with new charge identity and audit, not something permitted by normal item movement.

### 19.4 Performance and safety

Resolve candidate lists by explicit school and filters before expensive checks. Cache within one operation when safe; invalidate on relevant configuration/state changes. Avoid scanning every actor, body, book or spell on every heartbeat. Sleep tracking should subscribe only for affected identities/capabilities and unsubscribe on loss of applicability/shutdown.

Bound saved-plan counts and parsing payload sizes through sane configuration limits. Validate integer conversions, quantity multiplication, power saturation, GUID uniqueness and all persisted numerical values. User-provided names or emote data must not be interpreted as untrusted code.

---

## 20. Required regression and integration tests

Use deterministic tests, injected time/check outcomes and ordinary runtime constructors/loaders wherever practical. Mock services at real boundaries; do not mock away the `VancianMagicService`, actual `MagicSpell` resolution or item-component behaviour being asserted. Add failing/positive boundary tests, not only XML snapshots.

The following matrix is the minimum behavioural inventory. Several rows can be data-driven cases within one test method, but every row must map to executable coverage in the completion report.

### 20.1 Capability, access and selections

| ID | Required assertion |
|---|---|
| CAP-01 | `vancian` builder/runtime registration, clone and XML round trip; old `skilllevel` still loads unchanged. |
| CAP-02 | Mixed Selected-at-will and Spellbook-memorised rules work inside one capability without class-name checks. |
| CAP-03 | Spontaneous/at-will with unsupported book source fails validation; unresolved rule links are reported. |
| CAP-04 | Caster-level, candidate, count and callback progs reject bad signatures; invalid numeric returns fail closed. |
| CAP-05 | Level-zero migration grants no access; explicit level-zero candidates can be selected and cast at will. |
| CAP-06 | Duplicate grants of one capability and two instances of one identity share one budget; different capabilities remain separate. |
| CAP-07 | Rename/reorder keeps keys and state; structural changes suspend rather than refill; delete/re-add alias does not attach old slots. |
| CAP-08 | Increased capacity waits for refresh; reduced/restored capacity preserves individual spent states. |
| KNW-01 | Browsing/draft editing provides no access and invokes no notification; whole-capability multi-level commit works. |
| KNW-02 | Permission false denies initial choice, additions and replacement; always-false fixed repertoire can be staff-granted. |
| KNW-03 | Actual commit invokes hooks once with immutable old/new sets and canonical identity character. |
| KNW-04 | Unchanged/permutation-only per-rule sets are no-ops; cross-rule reassignment still runs permission even if union unchanged. |
| KNW-05 | Stale draft/version, duplicate entry, ineligible candidate and over-limit selection all fail without partial change. |
| KNW-06 | Register-based interval cannot be bypassed by another body, reconnect or capability reacquisition. |
| KNW-07 | Callback error/indeterminate crash state is visible and blocks further edits; reload never blindly repeats callback. |
| KNW-08 | Removing a selected spell changes at-will/spontaneous access but leaves an existing prepared copy usable; no slots refill. |

### 20.2 Loadouts, recovery and power

| ID | Required assertion |
|---|---|
| MEM-01 | Duplicate memorisation produces distinct copies; spending one preserves the others and the full committed pattern. |
| MEM-02 | Create/copy/edit/select/delete/rename plan never changes current slots; deleting a selected plan retains last pattern. |
| MEM-03 | No mid-cycle reassignment or filling an Unassigned slot; valid empty/incomplete plans behave explicitly. |
| MEM-04 | PatternChangesOnly restores an identical pattern without a book but requires formulae for a changed book subpattern. |
| MEM-05 | EveryRefresh requires books even for identical pattern; losing a book does not erase current memorised copies/cantrips. |
| MEM-06 | Equivalent reordered plans and replacement source-book IDs compare correctly; changed count or slot level is a real change. |
| MEM-07 | Several books supply one plan; one formula can supply several copies; inaccessible books cannot supply it. |
| REF-01 | All three recovery modes execute with configured durations and interval, no hard-coded day or midnight reset. |
| REF-02 | Interrupted timed preparation preserves the old ledger/pattern and does not consume earned sleep qualification. |
| REF-03 | Completion revalidates access/books/counts/version and refuses stale or invalid plan without partial refill. |
| REF-04 | Observed continuous sleep qualifies; unconsciousness, stasis, death, partial interruption and unobserved offline time do not. |
| REF-05 | Active awake projection prevents primary-body sleep exploitation; one episode does not repeatedly refill one capability. |
| REF-06 | SleepAutomatic retries correctly after blocked wake refresh without heartbeat spam; completed credit persists without duplication. |
| REF-07 | Restart/capability loss does not clear expenditure or generate fresh credit; new capability has no implicit full ledger. |
| POW-01 | Standard at matching level, +1 per excess level by default, capped at RecklesslyPowerful with overflow safety. |
| POW-02 | Level-zero at-will uses Standard; permitted finite upcast uses its slot and cannot enhance subsequent at-will casts. |
| POW-03 | Trigger power-range incompatibility is reported before spending; no unrestricted power argument bypass. |
| POW-04 | Reliable outcome uses existing outcome helpers; target resistance still functions; legacy checks remain unchanged. |

### 20.3 Books and inscription

| ID | Required assertion |
|---|---|
| BOK-01 | Two instances of one book prototype have independent formulae; prototype edit/save does not replace contents. |
| BOK-02 | Borrowed/stolen usable book works; hidden/closed/locked/inaccessible or configured unreadable source does not. |
| BOK-03 | Book-to-book copy adds one formula, charges configured costs once and preserves source. |
| BOK-04 | Duplicate/self-copy/destination-full/invalid-formula refuses without cost; interruption gives no formula. |
| BOK-05 | Successful scroll transcription destroys source and stores only base formula; no slot, activation check or effects. |
| BOK-06 | Concurrent scroll cast/transcription cannot both succeed; high-level eligible formula copying does not require current slot capacity. |
| INS-01 | Prepared inscription spends the exact copy/level/power; spontaneous inscription spends its chosen slot. |
| INS-02 | At-will inscription spends production/material costs but no finite slot; an upcast version does spend its finite casting. |
| INS-03 | Inscription never invokes target or caster effect templates; a fixture that would spawn/heal/execute an effect is not run. |
| INS-04 | Combined material planning prevents the same item quantity satisfying separate spell and production costs incorrectly. |
| INS-05 | Precommit interruption releases reservations with no charge/debit; successful commit creates exactly one prepaid charge. |
| INS-06 | Script/craft and player paths use the same validated debit service; no unchecked set-XML/mark-prepaid bypass. |
| INS-07 | Lost access, changed slot, moved blank or unavailable component at completion produces no free scroll. |
| INS-08 | Multiple recovery cycles may legitimately create a stockpile; no hidden expiry or stockpile cap. |

### 20.4 Scroll activation and potency

| ID | Required assertion |
|---|---|
| SCR-01 | Compatible caster can use an unknown/unprepared spell with zero slots remaining; non-caster cannot. |
| SCR-02 | Normal level uses capacity rather than remaining slots; at-will infinity is not an unlimited level ceiling. |
| SCR-03 | Stored casting level, including upcast low-level spell, determines over-level check; difficulty/trait settings are honoured. |
| SCR-04 | Invalid target/reader/payload/access refuses without destruction; inspect/read-description never activates. |
| SCR-05 | Committed over-level failure destroys scroll with no effects; no retry-until-success on an intact item. |
| SCR-06 | Successful activation destroys item and pays no second slot, materials or spell resource cost. |
| SCR-07 | Resistance, reflection/ward handling and partial group success still consume exactly one scroll; later targets are processed. |
| SCR-08 | Creator and reader different numerical stats: damage/duration/source outcome use stored creator values, not reader values. |
| SCR-09 | Creator changed, offline, dead or deleted: no live creator lookup is needed; reader still owns attribution and effects. |
| SCR-10 | Per-target outcome remains live; inscription uses no dummy target and does not pre-roll target-dependent results. |
| SCR-11 | Numerical options, `variable` and different TraitBonusContexts are captured correctly, not only raw trait IDs. |
| SCR-12 | Snapshot effect/formula updates are immutable across source numeric edits; explicit revocation/missing references fail safely. |
| SCR-13 | Supported persistent effect reload retains numerical snapshot and reader ownership after scroll deletion. |
| SCR-14 | Unsupported/custom effect fails compatibility before inscription/activation cost; every registered type has a manifest entry. |
| SCR-15 | Required effect families have real adapters and end-to-end tests; actor-owned conjuration/movement uses reader, not author. |
| SCR-16 | Nested/concurrent live and snapshot casts cannot contaminate shared expression bindings. |
| SCR-17 | Scroll transfer preserves potency; normal copy/split/merge/morph paths cannot multiply charges. |

### 20.5 Integration, persistence and documentation

| ID | Required assertion |
|---|---|
| INT-01 | Old school cast/help and direct invocation entry points cannot bypass Vancian expenditure; legitimate legacy route remains available. |
| INT-02 | Vancian global read-only aggregation does not grant legacy knowledge or expose hidden payloads/inaccessible books. |
| INT-03 | Spell-backed power does not duplicate payment; prepared attack/trap/substance paths receive no new slot debit. |
| INT-04 | `CharacterCanCast`/FutureProg availability agrees with actual command refusals and has no effects/charges/randomness. |
| INT-05 | Self, character, group and an additional-parameter trigger retain target semantics under computed power and scroll context. |
| PER-01 | Known choices, plans, last pattern, slots, callback status and qualification survive save/load with stable identities. |
| PER-02 | Inject failures around charge/debit/formula/source-removal commit boundaries; no free charge, double transcription or resurrected scroll. |
| PER-03 | Crash after scroll consumption never replays target effects or restores charge; diagnostic operation remains available. |
| PER-04 | Simultaneous/reentrant operations from two instances do not double-spend or overwrite newer state. |
| PER-05 | Legacy schema upgrade defaults are correct; no automatic zero-level or scroll eligibility grant; fresh migration succeeds. |
| PER-06 | Unknown schema/missing references suspend safely with repair output; no guessed IDs, erased state or silent substitutions. |
| DOC-01 | Every public command/setting/prog in this contract has matching implementation and help; samples compile/run where claimed. |
| DOC-02 | Non-admin worked walkthrough covers mixed Wizard, both book policies, known hook, spontaneous casting, copying and stored-potency scroll. |

Also run the existing regression suites for corrected group spell resolution, spell FutureProg functions, spell-backed powers, combat payloads, magical substances, item persistence and identity/body instances when touching those surfaces. Do not reduce old tests or skip them simply because new fixture tests pass.

---

## 21. Verification, documentation deliverables and completion report

### 21.1 Build/test gates

Use the repository's current supported SDK and test scripts. The following are intended from repository root; restore first when package state is not already available.

```bash
# Focused iteration; adjust the filter to actual new test class names.
dotnet test "MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj" \
  -c Debug -m:1 --filter "FullyQualifiedName~Vancian|FullyQualifiedName~SpellScroll|FullyQualifiedName~Spellbook"

# Required product builds.
dotnet build MudSharpCore/MudSharpCore.csproj -c Debug --no-restore -m:1
dotnet build MudSharpCore/MudSharpCore.csproj -c Release --no-restore -m:1
dotnet build DatabaseSeeder/DatabaseSeeder.csproj -c Debug --no-restore -m:1
dotnet build DatabaseSeeder/DatabaseSeeder.csproj -c Release --no-restore -m:1

# Repository-wide normal regression gate.
bash ./scripts/test-unit.sh

# Patch hygiene.
git diff --check
```

On Windows use the PowerShell test-script counterpart and native argument/line-continuation syntax. A filter matching zero tests is not a pass: report matched counts. The standard gate should cover the applicable shared-library, expression, core, database and seeder suites, not long-running unrelated climate simulations.

Generate migrations with EF tooling and verify designer/snapshot consistency. Exercise upgrade and fresh schema creation against uniquely named disposable local databases when available; never use a reachable production/game database for the walkthrough. Do not reset or overwrite a database to make a test pass.

A live walkthrough is required as a verification activity where the environment supports it. If unavailable, explicitly report which runtime scenarios remain unexecuted and why. Do not invent successful live results from source inspection or unit tests.

### 21.2 Full feature documentation

The delivered repository documentation must describe the implemented feature, not merely repeat a task checklist. Retain this complete design/decision reference and add/update:

1. **Runtime/developer reference:** type ownership, capability/rule/allowance model, route authorization, slot generations, timing, hooks, numerical snapshot evaluation, transactions, schema versions and extension contracts.
2. **Builder guide:** all capability and item settings, complete valid Wizard/Cleric/Sorcerer examples, compiler-tested policy progs, item production/craft integration, readiness diagnostics and staff repair procedures.
3. **Player guide:** browsing/selecting known spells, saved versus active loadouts, preparation/recovery, both book policies, borrowing/copying, cantrips/upcasting, scroll inscription/activation and expenditure consequences.
4. **Scroll compatibility inventory:** every registered effect type with support status, numerical fields, live actor/target semantics and unsupported reasons.
5. **Verification/coverage report:** test IDs above mapped to test methods and actual results, migration checks and live walkthrough transcript/limitations.

Update the magic overview, implemented-type inventory, spell/capability documentation and relevant item/FutureProg documentation so they no longer imply the feature is absent. Do not describe general caster-side effects as unconditional scroll costs; they remain effects applied at release under normal resolution rules.

### 21.3 Agent completion report

Report the actual base/head SHA, changed projects, migrations, command/prog additions, implementation choices differing only in naming/organization, test commands/counts/results, live verification results and known limitations. Include a clear statement that no production content or character grants were installed.

Any genuinely unsupported scroll effect must appear in the compatibility inventory and builder readiness output. Any missing **mandatory** feature in this document must be listed as incomplete, not obscured as a future enhancement. Do not declare completion using only a count of changed files or passing tests unrelated to the new paths.

---

## 22. Definition of done

The assignment is complete when a builder can configure a single capability with selected at-will cantrips and finite book-based memorisation; create distinct Cleric/Sorcerer configurations without new class code; define progression and known-change policies through progs; and exercise the complete book/loadout/refresh/scroll workflows through supported commands.

Specifically, the delivered engine must demonstrate all of the following together:

- Two copies of one prepared spell are distinct; at-will casting is unlimited in uses but not power.
- Saved plans do not change live memorisation; the last committed pattern survives both expenditure and plan edits.
- Both book-recovery policies work with borrowed/stolen accessible books and instance-owned formulae.
- Whole-capability selected-known changes respect builder permission/notification hooks with identity-stable state.
- Caster progression is supplied externally, with no refill from capability/body/configuration cycling.
- Lower-level spells—including eligible cantrips—can occupy higher-level slots using the agreed capped power mapping.
- Book/scroll transcription works; successful scroll copying consumes the scroll without casting it.
- Inscription prepays the correct casting and materials without resolving any spell effects.
- Scroll release uses stored creator numerical potency and reader-world context, supports destructive over-level failure, and charges no second slot/material cost.
- Save/load, interruption, concurrent commands and process-failure boundaries cannot create duplicate/free castings.
- Existing legacy, attack, trap, substance and corrected multi-target behaviour remains compatible.
- The feature is fully documented for players, builders and developers and the verification report is factual.

**Suggested PR title:** `Add configurable Vancian capabilities, spellbooks and prepaid spell scrolls`.

---

## Appendix A. Source grounding and repository references

The design decisions come from the approved specification recorded in section 2. The following sources establish the existing code interfaces and baseline; they are not evidence that the new feature already exists. These files were inspected through the GitHub connection at the pinned revision unless noted otherwise.

| Source | Verified relevance |
|---|---|
| S1 | Baseline commit merges the independent group-spell-resolution correction. |
| S2 | `MagicModule` registers school verbs, dispatches casts/help and directly consults `SpellKnownProg`; access integration must include these paths. |
| S3 | `TraitExpression` parses trait and extended race/culture/role/merit option bindings. |
| S4 | Trait evaluation obtains named/variable trait values with a `TraitBonusContext` and evaluates extended options from the owner. |
| S5 | `ITraitExpression` exposes live-owner evaluation and named variables, relevant to scroll numerical-context design. |
| S6 | `ICharacterIdentity` distinguishes identity/primary/focused instances and shared identity traits/knowledge/merits. |
| S7 | Database project instructions require generated EF migrations, designers and model snapshot. |
| S8 | `SkillLevelBasedMagicCapability` demonstrates current capability persistence, concentration and factory registration. |
| S9 | `ICheck`/`CheckOutcome` provide the existing outcome scale, all-difficulty checks and synthetic outcome helpers. |

Pinned references:

- **S1:** [Reviewed commit](https://github.com/FutureMUD/FutureMUD/commit/2b31d786c95deb2899adc7d3e9f26e63bea0e55b).
- **S2:** [MagicModule.cs](https://github.com/FutureMUD/FutureMUD/blob/2b31d786c95deb2899adc7d3e9f26e63bea0e55b/MudSharpCore/Commands/Modules/MagicModule.cs), especially the school verb registration and `MagicGeneric` dispatch.
- **S3:** [TraitExpression.cs](https://github.com/FutureMUD/FutureMUD/blob/2b31d786c95deb2899adc7d3e9f26e63bea0e55b/MudSharpCore/Body/Traits/TraitExpression.cs), construction/parsing around lines 30–240.
- **S4:** [TraitExpression.cs evaluation](https://github.com/FutureMUD/FutureMUD/blob/2b31d786c95deb2899adc7d3e9f26e63bea0e55b/MudSharpCore/Body/Traits/TraitExpression.cs#L330-L363).
- **S5:** [ITraitExpression.cs](https://github.com/FutureMUD/FutureMUD/blob/2b31d786c95deb2899adc7d3e9f26e63bea0e55b/FutureMUDLibrary/Body/Traits/ITraitExpression.cs).
- **S6:** [ICharacterInstance.cs / ICharacterIdentity](https://github.com/FutureMUD/FutureMUD/blob/2b31d786c95deb2899adc7d3e9f26e63bea0e55b/FutureMUDLibrary/Character/ICharacterInstance.cs).
- **S7:** [Database project AGENTS.md](https://github.com/FutureMUD/FutureMUD/blob/2b31d786c95deb2899adc7d3e9f26e63bea0e55b/MudsharpDatabaseLibrary/AGENTS.md).
- **S8:** [SkillLevelBasedMagicCapability.cs](https://github.com/FutureMUD/FutureMUD/blob/2b31d786c95deb2899adc7d3e9f26e63bea0e55b/MudSharpCore/Magic/Capabilities/SkillLevelBasedMagicCapability.cs).
- **S9:** [ICheck.cs](https://github.com/FutureMUD/FutureMUD/blob/2b31d786c95deb2899adc7d3e9f26e63bea0e55b/FutureMUDLibrary/RPG/Checks/ICheck.cs).

Further existing integration paths listed in section 17.3 are investigation targets in the checkout. Do not assume line numbers, private helper names or all unrelated subsystem details from an older source snapshot.

## Appendix B. Handoff summary

Implement the complete Vancian feature in this document. Use one configurable capability type; separate repertoire source from allowance mode; persist budgets by identity and capability; preserve legacy routes without providing a bypass; and treat spellbooks and scrolls as real instance-owned item features.

The difficult requirements are not optional: last committed patterns, whole-capability known hooks, mixed at-will/finite rules, stored creator numerical context, destruction on committed scroll failure, and durable cross-item/slot transitions must all be delivered with executable tests and user-facing documentation.
