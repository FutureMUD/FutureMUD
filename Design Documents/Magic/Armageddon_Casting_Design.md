# Armageddon-inspired casting: ARM-01 design contract

Task: `MAGIC-ARMAGEDDON-ARM-01`. Edition: 1, 27 September 2026.

ARM-02 implementation follow-up: [runtime and authoring guide](Configurable_Casting.md)
and [acceptance handover](Configurable_Casting_Handover.md). The proposal below remains
the wider design; later slices and the candidate stock repertoire are not installed by ARM-02.

**Status: design proposal; not authority to implement or seed.** Luke authorised this
design/audit package and selected the architecture and optional features recorded below.
The numerical stock profile, candidate repertoire, adaptations and proposed omissions
still require approval. No production implementation, database change, merge or deployment
is part of ARM-01.

Audited runtime: `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`. The assignment cited
`f2adb2cd43a6f993e141d488db91fea6ed1cc486`; the integration map records current evidence
instead of silently treating that earlier revision as current.

## Package and reading order

1. This contract: model, proposed balance, workflows, commands and acceptance examples.
2. [Repertoire register](Armageddon_Repertoire.md) and [canonical JSON](Armageddon_Repertoire.json).
3. [Evidence and runtime integration](Armageddon_Casting_Integration.md).
4. [Proposed ARM-02 implementation brief](ARM02_Configurable_Casting_Implementation_Brief.md).
5. [Owner decision summary](Armageddon_Casting_Decisions.md).

The register describes a complete **candidate** inventory, not installed stock or a claim
of live-game parity. Historical school associations are evidence about the supplied dump;
FutureMUD memberships, names, prerequisites and balance are separate proposed content.

## Decision register

| ID | Contract | Authority and alternatives |
| --- | --- | --- |
| A-D01 | Extend `skilllevel` with an optional repertoire-casting policy and explicit cross-school admission. One home command school remains. | User selected extending existing capabilities where feasible. A new `channelling` factory type is unnecessary; types per element are rejected. |
| A-D02 | Acquisition and controlled grade belong to canonical identity + spell. Native skills hold proficiency; a route resolves a trait per admitted spell. | User selected both individual-spell and shared-tradition proficiency configurations. One uniform acquisition store avoids making shared skill presence equal spell knowledge. |
| A-D03 | Seven ordered stock grades map to existing `SpellPower`; builders can replace the ordered map. | User-approved design direction; mapping and thresholds below are proposed balance. Do not use Vancian slot levels as grades. |
| A-D04 | Lower controlled grades are allowed. Overreach requires explicit consent and exactly the next grade. One progression opportunity per invocation. | User-approved direction; probabilities, clocks and eligibility details below await balance approval. |
| A-D05 | Rebind one designated energy cost to the selected route's reserve, preserving secondary fixed costs and the spell's native school. | User-approved direction. Duplicate spell catalogues and changing shared definitions during a cast are rejected. |
| A-D06 | Event-driven, capability-scoped acquisition edges; native improvement and lesson hooks remain the skill system. | User-approved direction; proposed acquisition thresholds and teaching policy below. A generic shared trait must not branch into unrelated traditions. |
| A-D07 | Target-free timed practice uses full configured energy cost, with both proficiency and mastery eligibility and shared limits. | User explicitly selected both forms of progress. Calling the real spell and discarding its output is rejected. |
| A-D08 | Names and configurable formulas are equivalent command inputs. Normal speech, explicitly mapped quiet and area variants are in release scope. | User explicitly selected the broader scope and names-or-formulas default. Quiet is not undetectable; area is not a universal target multiplier. |
| A-D09 | Scrolls/substances plus charged and focus roles for wands/staves; trained non-casters may activate charged items. | User selected both roles and trained users. Payload lists, production values, disabled scroll-learning and disabled production-improvement defaults are proposals. |
| A-D10 | One optional Armageddon-inspired installer with stable keys, ownership manifests and safe additive reruns. | Brief requirement. No new preset is advertised until implemented. Never refresh player state during an installer rerun. |

These are configuration variations in one model, not two parallel progression systems.
The Earth/Fire/Water/Wind/Shadow/Lightning/Void/Sorcerer labels are preset data. Core code
must not test those names, assume seven schools globally, or hard-code preservation as
good and defiling as evil.

## Capability and invocation configuration

Propose an optional versioned `Casting` section in the existing capability XML. Omission
means existing behaviour. Its home school uses the existing `School` reference. An enabled
policy exposes these ordinary builder-editable settings:

| Configuration | Meaning |
| --- | --- |
| Stable admission key, spell ID, enabled flag | Explicit spell whitelist; school ancestry is not admission. |
| Proficiency trait binding | Entry override, otherwise capability default. Resolve to one native character-owned skill. Many entries may select the same trait. |
| Energy binding | Identify exactly one source cost resource on the spell and the route's destination reserve. Reject a missing or ambiguous source line. |
| Grade profile | Ordered `SpellPower` mapping, next-grade thresholds, difficulty and cost policy, mastery chance and opportunity interval. Shared entries for the same spell must use one compatible progression profile. |
| Acquisition rules | Starting grants, prerequisite edges, teaching permission and approved explicit grant hooks. Each edge is owned by this capability admission. |
| Physical/action policy | Speech, free manipulation, retained focus plan, normal component plan and a separate practice plan; acting-body permission prog. |
| Variant/formula rules | Permitted normal/quiet/area modes, exact target adapter, pronunciation tokens and mode-specific checks/costs. |
| Production permissions | Explicit carrier eligibility, manufacturing/recharging policy and optional learning/improvement flags. |

Malformed policy disables this route with an actionable builder error; it must not fall
back to legacy casting. Unconfigured `skilllevel` capabilities, Vancian allowances and
independent spell-backed powers retain their current semantics. Since `vancian` inherits
`skilllevel`, shared XML loading must not accidentally enable a second casting route on
existing Vancian definitions. ARM-02 enables this policy only for `skilllevel`; combining
it with Vancian on one capability is outside that slice. Separate capabilities may coexist.

Proposed shared contracts are an optional policy-provider interface, immutable resolved
invocation data and a world-local casting/progression service. The invocation includes
actor physical-instance identity, canonical owner, capability/admission key, spell,
resolved skill, resource holder/bindings, requested grade, variant, quote and permission
version. It must be passed explicitly through command, trigger and FutureProg paths.
Ordinary mutable catalogue spell objects are not invocation contexts.

The new service must distinguish `HasAcquiredSpell`, `AvailableRoutes`, `Preview` and
`Invoke`. The first three are pure: no branch evaluation that grants, trait improvement,
resource regeneration, payment, starting grants, cooldown reset or persistence writes.
`CharacterKnowsSpell` may report a union of legitimate knowledge sources, but cannot
authorise execution. A book, scroll, Vancian retained pattern or inspection query is not
a repertoire casting route. Existing legacy knowledge routes must explicitly exclude a
configured admission capability unless an independent legitimate legacy grant exists.

The spell keeps its native school for wards, resistance, dispels, detection and effects.
The home school selects commands. An Earth spell cast through a Sorcerer command is still
Earth magic. If several routes are possible, require `via <capability>` unless the command
home school and actor's explicit saved preference select one uniquely. Never choose the
cheapest route or a route with available energy silently.

## Permanent progress, bodies and reserves

Propose one durable record per `(CanonicalCharacterId, SpellId)` containing acquired
status, controlled grade, acquisition provenance/time and next mastery opportunity UTC.
An unacquired spell has no grade; acquisition creates grade 1. A separate durable
`(CanonicalCharacterId, TraitDefinitionId)` opportunity deadline throttles native skill
use across all spells sharing that trait. These are timestamps, not a second skill value.
Capability enrolment records make starting grants once-only by stable capability key.
Deleting or temporarily suppressing a capability does not delete any of these records.

The native implementation already distinguishes character-owned and body-owned traits.
Ordinary body-form switching retains character traits. Secondary simultaneous character
instances currently copy the character-trait list: existing trait objects are shared,
but later additions/removals are not consistently redirected to `Identity`. ARM-02 must
make character-owned skill acquisition and lookups coherent across those instances.
Do not migrate body-owned attributes into identity storage or clone proficiency per body.

Cast evaluation still uses the acting body, its equipment and live modifiers. Improvement
is applied once to the canonical native skill owner; its existing `NoTraitGain` effect
must be identity-owned for this route. Add an invocation-local improvement-owner/suppression
seam, not a global change to what body supplies a check. Native branch-on-missing during
this cast is disabled: explicit acquisition supplies the configured proficiency trait.

For configured casting reserves, the canonical character is also the resource holder.
The acting body pays bodily costs and supplies inventory. Extend the existing gathering
holder resolution narrowly for these reserves so quote, cap, destination credit and
casting debit agree on that same holder. Do not copy balances into secondary bodies.
Unconfigured resource users keep their existing ownership rules.

Reconcile passive generators once per identity when capability grants, temporary effects,
focus/active-body selection or relevant configuration changes. Use existing generators;
do not add a new heartbeat. Reconciliation cannot refill a pool, add its cap to its balance,
or apply offline catch-up. Reattachment resumes eligibility against the saved amount.

Proposed stock: elemental reserves have cap 100 energy units, start at 0, and regenerate
2 units per real minute while the eligible focused body is conscious, doubled at rest.
The Sorcerer reserve also caps at 100 and starts at 0, but has no passive regenerator.
Self/Gentle/Land routes remain explicitly configured through the delivered gathering
service; preservation permits chosen non-Land routes, defiling permits chosen Land routes,
and a builder may permit both. Casting never selects a gathering method automatically.
Incompatible passive and gather-only entitlements on the same destination reserve are a
configuration error. An identity legitimately holding both traditions has two distinct
pools; it cannot fund a Sorcerer invocation from the Earth pool without a separately
configured, authorised route.

## Proposed numerical profile

Everything in this section is proposed stock data, not a historical reconstruction or
an engine-wide default. Skill values are raw native trait units in a stock range 0–100;
opening value is 10. Durations use real seconds; opportunity timestamps use UTC. Resource
costs are finite non-negative native resource units, retained without display rounding.
Chance values in configuration are fractions from 0 to 1. A difficulty step is one native
`Difficulty` enum step, not a percentage-point penalty.

| Grade | Existing `SpellPower` | Enum integer | Raw proficiency needed to overreach into grade | Difficulty steps above spell base |
| --- | --- | --- | --- | --- |
| 1 | ExtremelyWeak | 2 | Acquisition | 0 |
| 2 | VeryWeak | 3 | 20 | 0 |
| 3 | Weak | 4 | 40 | 1 |
| 4 | Standard | 5 | 55 | 1 |
| 5 | Strong | 6 | 70 | 2 |
| 6 | VeryStrong | 7 | 85 | 2 |
| 7 | ExtremelyStrong | 8 | 95 | 3 |

Raw proficiency thresholds gate only overreach, not use of an already controlled grade.
Temporary bonuses cannot satisfy them. Stock spell base difficulty is `Easy` (4).
Stock normal and completed-practice checks require at least `MinorPass`. Production and
charged activation also require `MinorPass` at `Normal` difficulty. Practice uses a
standalone native cast-skill check with the same route/grade difficulty, never calls the
spell trigger or manifestation pipeline. These thresholds are builder-configured data.
Overreach adds one difficulty step and multiplies designated energy cost by 1.5.
Quiet adds one step and multiplies that cost by 2. Area adds one step and multiplies it
by 2. Multipliers compound; secondary fixed costs are unchanged. An out-of-range or
`Impossible` final difficulty is refused before commitment, not silently clamped.

Each register entry assigns an energy base `B`. Normal designated energy price is
`B * requestedGrade`. Full price is `B * grade * overreachMultiplier * quietMultiplier *
areaMultiplier`. A spell intrinsically targeting a group already has its own base price;
the area multiplier applies only when selecting a separately approved area variant.
Target count never creates extra proficiency/mastery opportunities. The default normal
spell lockouts are 10 seconds exclusive and 2 seconds non-exclusive, through existing
lockout semantics. The preset introduces no universal casting delay for normal casts.

Native skill policy proposal: use the existing classic improver with 0.10 chance on
success or failure, gain 0.5 skill units, cap 100, difficulty-threshold interval 0, and
600 seconds native no-gain after an actual improvement. In addition, the shared trait
opportunity gate permits at most one routed skill-use attempt per 60 seconds, whether
or not the improvement roll succeeds. Existing learning merits still affect the native
improver; no independent second skill-growth roll is added.

Mastery proposal: a permitted next-grade invocation reserves one mastery opportunity at
commit if its spell deadline has elapsed, then sets the next deadline to commit UTC +
600 seconds. On a qualifying success, one roll has probability 0.25 to set controlled
grade to the attempted grade. Failure does not advance; the opportunity stays spent.
At or below the controlled grade there is no mastery roll. Grade 7 has no next grade.
An invocation may cast while its progression gates are closed, but cannot train through
them. Clock passage does not produce progress by itself.

A manifested success qualifies when the cast check meets the spell's success threshold
and at least one intended manifestation reports an actual applied operation; a valid
non-targeted utility invocation may qualify on its explicit applied-operation report.
The proposed result distinguishes `Applied`, `NoChange`, `Rejected` and `Unknown`.
Existing target-resolution success and a non-null persistent child are insufficient:
instantaneous damage returns no child, while removing blindness from an unaffected
target also returns none. Adapters report the operation once; unknown legacy effects
retain their ordinary casting behavior but cannot establish mastery eligibility.
Empty groups, no-op removals, every target warded or resistant, or rejected effect
admission do not advance mastery. Caster effects alone cannot rescue a rejected/no-op
targeted invocation. Native
skill use may still learn from a paid failed check. A practice check qualifies without a
target; it never inspects target resistance or wards. Production and charged activation
do not qualify for spell mastery in the stock profile.

The register's scaling formulas use `g` for requested grade, distinct from the engine's
`power` integer. The integration binds `grade` explicitly in invocation expressions;
existing `power` keeps its enum integer meaning. Damage/health values are native engine
damage/healing units, not percentages of a victim's health. Litres and kilograms in the
register must be converted through the game's configured unit system at content authoring.
Binary effects scale duration, not an invented magnitude. Item/NPC creation specifies
quantity and lifetime; native type admission and body applicability remain authoritative.

Some effect magnitudes are fixed XML scalars rather than expressions. Grade-dependent
scalars therefore require a validated invocation-local binding; passing `grade` only to
existing expressions would leave Stone Skin's `TraitBoostEffect.Bonus` unchanged. Store
typed field bindings on the versioned spell grade profile, addressed by effect list,
index, expected token and named supported scalar. Evaluate against the selected grade
and route skill, validate finite/domain-appropriate output, then apply to the detached
invocation definition before execution. ARM-02 supports the fixture's `boost.Bonus =
-grade`; later slices add only the fields their approved roster requires. Unknown fields
or stale indices fail readiness. No arbitrary property setter, shared-template mutation
or duplicate learnable spell per grade is permitted.

## Acquisition, branching and teaching

Starting roots are explicit per membership in the register. Proposed non-root edges use
the same-tradition root at controlled grade 2 and route-resolved raw proficiency 20.
These deliberately simple candidate edges are not the missing historical guild trees.
The Sorcerer has a separately curated whitelist and its own edges, even when a shared
prerequisite spell is globally acquired. A rule requires a current authorised capability,
its own admitted destination, all prerequisite spell acquisitions/grades and configured
proficiency thresholds. Skill identity alone is never an edge.

Run a bounded reverse-index lookup after acquisition, relevant trait change, controlled
grade change, explicit permanent enrolment or successful teaching. Inspect only affected
edges, make each grant idempotent, and process each newly unlocked node once. Reject
cycles at configuration validation. Temporary capability gains expose already learned
spells but do not enrol, auto-grant roots or evaluate automatic grants. Restoring a
permanent capability evaluates still-unfulfilled eligible edges without resetting state.

Native `teach` remains a skill lesson and does not automatically acquire every spell
using that skill. Add a spell lesson through the same proposal/acceptance/fatigue hooks:
teacher has acquired the named spell, a current admitted route, raw proficiency at least
40 and controlled grade at least 2; student has an admitted route and its prerequisites.
Use native teach/learn outcomes and configured `CanTeach`/`CanLearn` progs. A successful
spell lesson acquires grade 1 and opens a missing proficiency trait at 10, never lowers
an existing skill and never transfers the teacher's mastery. Proposal expiry is the
existing 90 seconds. Revalidate both participants' physical interaction and permissions
at acceptance. Skill teaching in shared-trait mode remains useful without granting spells.

Capability policy edits do not rewrite learned state or trait values. Changing a trait
binding selects an existing trait or opens the configured starting value on explicit
training/enrolment, never copies the old skill percentage. Grade-map changes require an
explicit versioned migration decision; no automatic rank reinterpretation on load.

## Invocation and failure workflows

```text
Pure inspect -> owner/acquisition/routes/grade/quote -> display only

Acquire -> canonical acquired record + grade 1 -> ensure configured native skill
        -> persist -> affected capability edges -> idempotent new acquisitions

Command or guarded Prog
  -> resolve explicit route, actor and canonical owner
  -> acquisition + admission + grade/overreach + physical/plane checks
  -> resolve target/variant + inventory feasibility + complete finite cost vector
  -> acquire identity-local invocation guard; revalidate quote and live capability
  -> COMMIT payment/material plan + lockouts + progression opportunity deadlines
  -> exactly one acting-body cast check with canonical improvement ownership
  -> each target's ward/resistance/effect admission -> target effects
  -> existing once-per-cast caster effects and result aggregation
  -> at most one native skill-use attempt and one eligible mastery roll
  -> persist changed progress; affected prerequisite edges; release guard
```

The service and `MagicSpell` share one commit callback; it must not charge once in each.
Combine remapped primary and secondary costs if they address the same reserve, then
check the combined total. Preflight failures cost nothing, cause no check and spend no
opportunity. Once commitment begins, a failed cast, resistance or ward rejection is a
paid outcome and retains lockouts/opportunity deadlines. A group never pays per target.
Reuse current successful group aggregation and once-only caster effects; do not turn
empty groups into mastery farming opportunities merely because legacy aggregation treats
them as successful.

Do not claim an atomic transaction over physical inventory, arbitrary effects and MySQL.
The proposed ARM-02 route uses a narrow durable invocation receipt only for paid mutation
and progression boundaries: unique invocation ID, owner/body/route/spell, pre/post debit
and opportunity values, stage and diagnostic. Save intended/committed stages around the
existing mutation boundary. `Reserved` with proven no mutation can cancel; a partially
paid or externally applied result becomes `NeedsReview`. Automatic recovery never reruns
effects, refunds uncertain payment, retries a mastery roll or grants a replacement cast.
Quarantine the shared state touched by the uncertain operation, not merely its route:
the canonical identity/spell progress record, trait opportunity record, debited reserve
and any uncertain physical inputs. All routes and bodies that would read or mutate those
records for paid/progression work must refuse until explicit staff reconciliation. A
Sorcerer route cannot bypass an uncertain Earth result for the same spell. Unrelated
routes may proceed only if they touch none of the quarantined state. This is not a generic
transaction platform and does not journal quotes, inspections or every calculation.

Persist the sampled mastery result before exposing its advancement/branch grants; recovery
reuses a durably recorded result rather than sampling again. If the provider cannot prove
the sample was saved, its value may be unrecoverable: retain `NeedsReview`, the last proven
state and the diagnostic, with no automatic advance or replacement roll. Staff records
an explicit resolution; do not describe an uncommitted grade as durably earned. A provider
failure before a stage is proven committed is uncertain, not a clean failure or success. Native
failure-injection/restart probes are required in ARM-02; ARM-01 does not perform them.

### Practice

```text
practice preflight (no target) -> same route/grade/quote gates
  -> pay full energy + practice plan; reserve shared opportunities
  -> 30-second timed activity on the acting body
  -> revalidate body, focus, capability, speech/manipulation and posture
  -> one practice check -> native skill use / eligible mastery -> persist
```

Stock practice is stationary, out of combat, requires speech and a free manipulating
appendage; quiet/area practice variants are disabled. Movement, combat, lost consciousness,
lost focus/capability, body switch, logout or restart cancels it. There is no refund after
payment and no progress on cancellation. Only one practice/production activity per
canonical identity may run at once. The retained practice focus is the tradition's
builder-tagged focus; there are no consumed practice materials in this proposed profile.
Normal spell component requirements remain independent. Practice cannot invoke casting
emotes as spell effects, spell triggers, caster effects, target selectors or callbacks
that assume manifestation. It emits original practice action text only.

## Formulas, variants and portable magic

Proposed player forms (literal examples, not currently implemented commands):

```text
earth spells
earth spell "Stone Skin"
earth cast "Stone Skin" grade 2 on self via "Earth Tradition"
sorcery cast "Stone Skin" grade 3 overreach on self via "Sorcerous Tradition"
fire practice "Ember Lance" grade 3 overreach
fire cast 'third manifest ember-lance' on sentinel
fire cast "Ember Lance" grade 2 quiet on sentinel
fire cast "Ember Lance" grade 2 area
earth teach "Stone Skin" to apprentice
use oakwand focus for "Stone Skin" grade 2 on self via "Earth Tradition"
use oakwand charge on sentinel
```

The quoted formula contains configured grade, mode and spell tokens; it is an alternate
parse into the same invocation. Stock grade tokens are `first` through `seventh`, modes
`manifest`, `quiet`, `area`, and a stable editable incantation alias per spell. The normal
name command supplies the same required speech automatically. Generic wording is original;
do not copy proprietary element names or historical incantations into stock output.
Unknown/ambiguous tokens and unsupported variants fail before cost. Quiet still requires
the ability to speak and produces a whisper through native hearing/perception; it does
not defeat magical senses. Stock quiet+area combination is disabled, though a validated
explicit builder mapping may allow it with compounded costs.

The register permits area only where it names a concrete target mapping. Freeze and
deduplicate the current eligible target set at commitment; resolve each target using its
own current ward/resistance. Stock area includes eligible others in the physical cell,
not remote route occupants, and does not imply immunity for allies. An empty set fails
preflight. Native room/exit effects target the actual room/exit rather than becoming
character group casts.

Focus mode requires the user's acquired spell, current route, ordinary energy/components
and an accessible held/wielded item satisfying its focus tag. A focus never supplies
acquisition, extra grades, a reserve, passive recharge or stored potency by default.
Items may possess both focus and payload components, but depletion never falls through
from `charge` into `focus`, nor from focus into charge.

Charged payloads use the scroll capture/compatibility model as a starting point, not an
assumption that every spell is portable. Authorise only the explicit register whitelist
and only after carrier-specific readiness passes. Stock scroll holds one charge, wand
five, staff ten; each charge releases one snapshot at its captured grade. Production or
recharge must use an acquired controlled grade (no overreach), spend one normal cast's
energy and production component set per charge, and take 60 seconds per charge. No mixing
different snapshots in a stack. Stock production check uses the route skill at Normal;
failure consumes committed inputs and produces no charge. Recharge fills only missing
charges; an existing charged item retains its original snapshot and must be emptied before
changing payload/grade. No spell effects execute during production.

Activation uses a stock native `Magic Item Use` skill (opening 10, cap 100), Normal
difficulty and `MinorPass` minimum, and one committed charge is spent even on a failed
activation check or all-resisted result. A non-caster can learn that skill through normal
teaching. Current body, target, plane, ownership and manipulation gates still apply.
There is no casting reserve debit, spell acquisition, spell-skill improvement or mastery
from activation. Numeric capture includes creator's route-resolved skill and grade;
target resistance and random damage are resolved at use, not rolled at manufacture.
Use retains reader attribution, opt-in/revocation and source-reference validation.

Substances keep their dedicated trigger/dose lifecycle. For the limited proposed potion/oil
list, production creates one standard gram dose with magnitude calibrated to the captured
grade; carrier volume is derived from actual density. No room/exit, caster-bound,
concentration, extra-target or identity-control payload is admitted. A dedicated authored
substance template may be necessary; it references the one repertoire key and is not a
second learnable spell. New manufacturing/capture integration remains a gap even when
the exposure primitive exists. Stock scroll learning and production improvement are
disabled; their configuration cannot bypass admission, shared learning limits or payment.

## Installer and content ownership

One optional installer owns the `arm.*` stable-key namespace. Dependency order is explicit:
native skills/improvers and teaching checks; schools; resources and allowed generators;
referenced materials/tags/prototypes/NPCs/progs; spell definitions; casting policies;
grant merits; portable templates. Reuse already-owned compatible stock by key, not by
assuming a numeric ID or case-insensitive display name is identity.

Persist a content manifest recording key, target ID/revision, last installed content hash
and owned fields. Rerun creates missing stock, updates only unchanged stock-owned fields,
and reports edited conflicts for a separate explicit reconciliation. Never overwrite
builder customisations, recreate missing player charges, repeat enrolment, refill reserves,
clear learned progress or enable player access merely because stock exists. Clone/edit
workflows must distinguish a stock key from a builder-owned derivative. No catalogue or
manifest database changes are made in ARM-01.

## Acceptance examples

All proposed-route scenarios below are **NOT_RUN: implementation required**. Existing
primitive tests and their narrower coverage are recorded in the integration map.

| ID | Setup/action | Required observable result |
| --- | --- | --- |
| A01 | Earth and Sorcerer cast the same Stone Skin definition at grade 2, base energy 5. | Each pays 10 from its explicitly selected reserve; spell school remains Earth. Only eligible Earth reserve regenerates. |
| A02 | Acquired Stone Skin, reserve 0; inspect via normal query, book and scroll. | Remains acquired; no state writes or gains. Ordinary cast refuses without payment/check. A separately paid charge is only an item route. |
| A03 | Stone Skin skill 42, controlled 2; request grades 1, 2, 3 with explicit overreach, then 7. | Grades 1/2 cost 5/10 at Easy. Grade 3 costs 22.5 at Hard (4 + grade step 1 + overreach step 1); a qualifying sampled advance sets 3. Grade 7 fails preflight. Failure may improve skill, never automatically grade. |
| A04 | Remove and restore a temporary item/drug capability; repeat and reload. | Acquired state, grade, clocks and reserve remain; no repeated starting grant, cap multiplication or passive generator leak. |
| A05 | Two simultaneous bodies alternate casts/practice and learn a new skill through the second. | One canonical acquisition and native skill; shared trait/mastery deadlines. Current body's permissions and inventory still govern. |
| A06 | Practice Ember Lance with a warded bystander present, including failed check and interruption. | Full quoted energy paid; no fire/damage/caster effects/resistance/ward use. Only completed eligible practice may train. Cancelled work grants no progress. |
| A07 | Shared Wardcraft acquired through Earth; its skill also appears on a Void admission. | Only Earth-owned eligible edges unlock. No Void capability means no Void grant. |
| A08 | Configure two spells against one tradition trait, then individual traits in a separate fixture. | Shared mode improves one native trait but acquires only named spells; individual mode improves only the relevant trait. Grade remains per identity/spell in both. |
| A09 | Name and formula casts at identical grade, then quiet and mapped area. | Same quote/authorization for equivalent inputs. Quiet whispers; area checks each intended target, pays once and can advance at most once. |
| A10 | A wand has both roles but no charge; user knows the spell. | Explicit charge use refuses; no automatic reserve debit. Explicit focus use takes the normal route and pays its costs. |
| A11 | Trained non-caster activates a charged wand, then tries ordinary cast. | Eligible item use spends one charge at frozen potency; normal cast still refuses. No spell learning or mastery. |
| A12 | Native persistence provider fails after a mastery roll but before its save; attempt the same spell through another tradition/body, then restart. | Shared affected spell/trait/reserve state blocks both routes pending reconciliation; retain the last proven state and any durably recorded result. An unproven sample stays uncertain, with no replay, refund, duplicate effect, second roll or automatic clearance. |
| A13 | Seed, customise a stock spell, rerun, then suppress/restore its grant. | Rerun reports the edited conflict; no player-state reset, free charge or repeated grant. |

## Release boundary

The complete release includes the selected broader options, but ARM-02 is only the
normal-casting/progression foundation. Practice, variants, production and item roles
receive separate exact briefs after roster/design approval. Register entries with a
missing primitive, larger supporting system or unresolved source remain visible; no
proposed substitution or omission is labelled owner-approved.

Reuse the delivered environmental coordinator, native organic accounting and rejuvenation.
The repository records a 04C correction and verification run; ARM-01 does not re-review
or close that gate. Final preset acceptance still requires explicit confirmation of 04C
and each approved follow-on slice. Cyberpunk, spirit-service contracts, speculative drain
systems, another astral framework and production Vancian-fantasy content remain excluded.
