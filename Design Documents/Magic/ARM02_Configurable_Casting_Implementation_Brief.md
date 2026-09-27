# ARM-02 — Capability-configured casting and progression foundation

Task: `MAGIC-ARMAGEDDON-ARM-02`. Edition: 1, 27 September 2026.

**PROPOSED — AWAITING DESIGN AND IMPLEMENTATION-BRIEF APPROVAL.** This file is an ARM-01
deliverable, not permission to execute ARM-02. Implement only after Luke approves the
[casting contract](Armageddon_Casting_Design.md), the fixture/balance decisions below and
this exact brief. Rebase its evidence against the then-current checkout before work.

Reference revision: `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

## Outcome and boundary

A non-admin can acquire a configured spell, see that knowledge independently of their
energy balance, and cast it through an explicitly authorised `skilllevel` capability at
a controlled grade or its single next grade. Two capabilities can admit the same native
spell while choosing different native proficiency traits and energy reserves. Progress
and opportunity clocks survive body switching, temporary capability loss and restart.

Deliver runtime, builder/player/FutureProg documentation, real command/service integration,
focused regressions and disposable native persistence evidence for this slice. Do not
ship only interfaces, source-text tests or admin-only fixtures.

Excluded from ARM-02: practice execution, formula parsing, quiet/area variant adapters,
spell lessons, scroll learning, manufacturing, wand/staff components, new repertoire
manifestation primitives, full stock seeding and 04C re-review. Their contracts already
constrain this foundation, but they require separate approved briefs. Existing native
group casting still needs preservation tests. No automatic merge or deployment.

## Required model and public seams

### Capability policy

Extend `SkillLevelBasedMagicCapability` with an optional versioned `Casting` XML section.
Expose it through a new optional shared `IMagicCastingCapability` contract in
`FutureMUDLibrary/Magic`. Use separate partial/helper files for policy/load/builder code;
do not expand the existing capability file into another monolith. No new factory token.
Old definitions without the section retain byte-equivalent semantic configuration.

The section stores enabled state, default proficiency trait ID, designated source energy
resource ID, destination reserve ID, explicit passive entitlement flag and entries with
stable admission key, spell ID, optional trait override, once-only starting-grant flag,
and capability-scoped prerequisite edges. Each edge stores required spell ID, minimum
controlled grade and minimum proficiency in that prerequisite's binding for this route.
The home school remains the capability's existing school. Do not admit child-school
spells implicitly. Core validation uses IDs/keys, never preset display names.

Use a versioned controlled-power profile on the spell definition for the shared grade
meaning, thresholds and mastery policy. Capability entries may narrow allowed grades but
must not redefine the same identity/spell's grade meaning. Author the approved seven-grade
fixture profile from ARM-01; its normal cost uses the spell's designated cost expression,
with a named `grade` argument in addition to unchanged native `power`.

Expose immutable policy definitions, validation errors and a resolved invocation record.
That record carries actor instance/body IDs, canonical character ID, capability/admission
IDs, spell ID/native school, effective trait, holder/resource bindings, grade, overreach,
target parameters, costs and configuration version. It must not be stored on a global
spell instance or inferred from ambient possession of an item.

`VancianMagicCapability` inherits this base: reject enabling the new policy on a Vancian
definition in ARM-02, without changing its existing state, XML or direct/power routes.
Independent Vancian and repertoire capabilities may coexist on a character.

### Service and inspection

Introduce one world-local `IMagicCastingService` with these responsibilities; commands
and FutureProg must call the same service:

| Operation | Inputs and result | Mutation rule |
| --- | --- | --- |
| Acquisition query | Character identity + spell -> acquired, grade, acquisition provenance | Pure, including empty reserve/inactive capability. |
| Route query | Acting character + optional spell -> permitted route descriptions and refusal reasons | Pure; never grants starting spells. |
| Quote | Acting character, capability, spell, grade, explicit overreach, resolved target specification -> immutable price/check context or refusal | Pure; uses physical body and canonical progress/resource owner. |
| Cast | Same intent as quote; recompute/revalidate under owner guard -> refused, failed, succeeded or needs-review | Exactly one paid invocation pipeline. |
| Enrol/grant | Authorised staff/Prog actor, target identity, capability/spell and reason -> changed/already present/refused | Explicit durable, idempotent acquisition; never usable as a normal cast fallback. |
| Progress notification | Identity, changed trait/spell -> affected edge evaluation | Bounded reverse index, no world scan or polling. |

Treat quoted output as advisory, not a spendable token. Execution must revalidate live
capability, body/focus, grade, target, component plan, price and current balance.

Update the actual legacy admission guard in `MagicSpell.HasLegacyRoute`, not only the
school command dispatcher. A policy-configured capability plus a true `SpellKnownProg`
must not count as an independent legacy grant. Direct cast triggers also reach
`MagicSpell.CastSpell`; require an authorised invocation there. Preserve a separately
granted unconfigured legacy capability and independent spell-backed-power/Vancian routes.

### State and native integration

Use canonical character IDs for acquired-spell records, controlled grades and mastery
deadlines. Use canonical character + trait for the shared skill-opportunity deadline.
Store enrolment markers by canonical character + stable capability identity. One acquired
spell record is authoritative in both proficiency configurations; native skill presence
alone neither grants nor revokes acquisition.

Proposed persistence entities, to be generated through the repository EF workflow:

- `CharacterAcquiredSpells`: composite character/spell key, acquired UTC/provenance,
  controlled grade, profile version, next mastery opportunity UTC and concurrency version.
- `CharacterMagicSkillOpportunities`: composite character/trait key, next opportunity UTC
  and concurrency version; no stored skill percentage.
- `CharacterCastingEnrolments`: character/capability key, enrolment UTC and completed
  starting-grant version; no grants on temporary effect attachment.
- `MagicCastingOperations`: unique invocation ID, owner/actor/body/route/spell, stages,
  bounded payment/material/progression intent and outcome, timestamps and diagnostic.
  Only paid mutation boundaries use this receipt, not queries or ordinary calculations.

Use existing character resource persistence and native skill storage. Initialise new
tables empty; do not infer historical acquisition from `SpellKnownProg`, skill presence,
Vancian books or available administrative powers. Existing native Vancian data remains
unchanged. If an existing world opts into this policy, staff enrolment/grants establish
its explicit starting state without a silent bulk conversion.

Make character-owned skill additions/lookups coherent for secondary simultaneous instances
and canonical identity. Retain body-owned trait routing. Evaluate checks against the
acting body, but route this invocation's one native `TraitUsed`/improver call and
`NoTraitGain` ownership to the canonical identity. Suppress the original automatic
improvement/branch call for this invocation so the service cannot improve twice. Missing
skills are opened by acquisition; a failed cast must not acquire a spell via native
branch-on-missing. Do not change unrelated check consumers' defaults.

For configured reserves only, resolve the canonical holder consistently in casting,
gathering destination quoting/credit and passive generator reconciliation. Run entitlement
reconciliation on grant/removal/expiry and focus/body transitions. New body instances or
reattachment must not receive copied balances, capacity grants or duplicate generators.
Physical gathering costs continue to affect the acting body, through the delivered
gathering service; no replacement gathering implementation or environmental scheduler.

## Casting behaviour and commit sequence

1. Resolve acquisition, explicit admitted route and a ready ordinary cast-trigger spell.
   Refuse ambiguity and invalid configuration. Keep the native spell school unchanged.
2. Resolve grade using ARM-01. Controlled 2 permits 1/2; explicit grade-3 overreach requires
   raw skill 40; grade 4 or higher refuses. Require the current body/plane/focus/speech/
   manipulation state and normal target permission.
3. Create invocation-local trait/numerical bindings and substitute only the designated
   energy cost resource. Preserve secondary fixed resource costs. Aggregate duplicate
   destinations before checking balances. Reject non-finite/negative amounts before mutation.
4. Use the actual inventory-plan feasibility and execution path. Under a canonical-owner
   guard, revalidate the resolved intent and record the narrow paid-operation intent.
5. Commit payment, materials, normal lockouts and eligible opportunity deadlines exactly
   once at the existing `MagicSpell` commitment boundary. The new service must not debit
   again after its callback returns. No cast check occurs on a preflight refusal.
6. Perform one normal cast check with the resolved trait and difficulty; failed checks
   remain paid. Resolve each target's current ward/resistance and existing effect admission,
   preserving group aggregation and once-only caster effects. Keep independent
   `SpellPowerInvocation` semantics and Vancian guarded routing unchanged.
7. Apply the one allowed canonical native skill-use attempt. For an eligible next-grade
   success, sample mastery once, record the result, apply/persist at most one grade and
   evaluate only affected capability edges. All-target rejection and empty groups cannot
   earn mastery. A native failed skill check can still improve under its improver.
8. Finalise the receipt and release the owner guard. An indeterminate paid stage remains
   `NeedsReview`; expose its diagnostic. Recovery does not replay effects, refund uncertain
   costs or roll mastery again. Staff resolution acknowledges/reconciles, never auto-casts.
   Quarantine every affected canonical spell/trait/reserve record and uncertain physical
   input across routes and bodies until reconciliation. An alternate tradition admitting
   the same spell cannot evade the quarantine; unrelated untouched state remains usable.

The existing detached invocation-copy/numerical-wrapper work is a precedent, not a complete
adapter. Bind the resolved casting trait for check, cost and duration and every numeric
field used by the fixture effects. Never mutate shared cost dictionaries or trait fields.
Reject an unsupported route-bound numeric expression at readiness with its exact field;
do not silently evaluate it with a default/zero/other route's trait. Do not restrict
ordinary legacy spells to scroll-compatible effects as an accidental consequence.

Support the fixture's fixed scalar explicitly: the spell grade-profile XML has optional
`ScalarBindings` entries with list (`target` or `caster`), zero-based effect index,
expected effect token, named field and expression. ARM-02 permits `boost.Bonus` only;
Stone Skin binds its agility effect to `-grade`. Resolve this through a typed adapter,
evaluate with the acting body's route-resolved numerical context, and replace the
`bonus` scalar only in that invocation's detached effect XML before constructing it.
Validate index/token/field, finite value and trait reference. Missing bindings retain
the fixed authored scalar; invalid configured bindings disable readiness. Builder show/
edit and cloning preserve bindings; effect reordering must revalidate them. Broader
scalar adapters await their approved content slice; never use reflective arbitrary setters.

Mastery also needs an explicit effect-operation result. Add an optional shared reporting
contract whose application returns `Applied`, `NoChange`, `Rejected` or `Unknown` plus
the optional persistent child. Invoke each effect through exactly one path, not once
for reporting and again for effects. Adapt `damage`, `spellarmour`, `boost`,
`personaltagward` and `removeblindness` for the small fixtures. For damage, `Applied`
requires delivered positive damage/pain/stun through native wound results, not just a
positive requested formula. New or changed active protection/trait/ward state qualifies;
removing zero blindness effects is `NoChange`. A native admission refusal is `Rejected`.
Legacy effects without a report retain ordinary behavior and yield `Unknown`, which
does not prove mastery eligibility. Aggregate at most one opportunity from an eligible
applied intended operation; caster effects alone cannot qualify a rejected targeted cast.
Preserve existing `MagicInvocationResult` success/failure semantics independently of this
progression signal. Successful instantaneous effects need no child to qualify.

## Required command, builder and FutureProg surface

Use the established school command dispatcher and `StringStack`. Player help and listings
must display acquisition separately from current route availability and energy. Extend
only the configured route's normal-cast syntax in this slice:

```text
<schoolverb> spells
<schoolverb> spell <spell>
<schoolverb> cast <spell> grade <1..7> [overreach] on <target> [via <capability>]
```

The optional `via` may be omitted only when one route is unambiguous; no automatic
cheapest/funded selection. Legacy commands keep their existing grammar. Practice,
formula, quiet and area input must clearly report that their later slice is unavailable,
not fall through to a normal manifestation.

Builder editing must provide `magic capability set casting` subcommands for enable,
resource binding, default trait, entry add/remove/trait/starting, prerequisite add/remove,
and validation/show; spell editing supplies the versioned grade profile. Publish exact
syntax and refusal examples in the implementation's builder guide. Cloning remaps
capability-local admission/edge identities while retaining referenced shared spell IDs;
it does not clone player progress or enrol anyone. Removal disables admission, not progress.

Proposed FutureProg functions:

- `hasacquiredspell(character, spell)` -> boolean, pure.
- `controlledspellgrade(character, spell)` -> number, 0 if unacquired, pure.
- `canchannelspell(character, capability, spell, grade, overreach)` -> boolean for route/
  grade/body preflight only; document that target/components and payment still need Cast.
- `channelspell(character, capability, spell, grade, overreach, target)` -> result text
  from the guarded normal casting service; no bypass route.
- `grantchannelspell(character, capability, spell, reason)` -> boolean, explicit approved
  grant/admission operation; never called by inspection. Record provenance.

Resolve spell/capability IDs using established FutureProg typing conventions. These are
new proposed functions, not existing API names. Add help/registration and normal actor
permission tests; do not imply a read-only query can guarantee a later successful cast.

## Fixture and acceptance specification

Use uniquely named builder-created fixtures, never production stock. Two non-admin
characters represent Earth and Sorcerer routes. Configure distinct 100-unit reserves,
Earth passive generation and Sorcerer gathering-only entitlement. Both admit one Earth
Stone Skin template (`spellarmour` + configured agility `boost`); Fire Ember Lance uses
`damage`; Wardcraft uses a configured tag ward and supplies Earth/Voids' independent
prerequisite-edge fixture. Use both per-spell traits and a separate shared-trait fixture.
These few templates test architecture; they do not approve the full candidate roster.

Required deterministic regressions:

- Old capability XML, Vancian allowances and independent spell-backed powers remain valid.
- With only a configured same-school capability and a true `SpellKnownProg`, both command
  and direct-trigger attempts without acquired/admitted paid invocation refuse. Adding a
  legitimate independent unconfigured legacy grant restores only its ordinary paid route.
- Pure queries at zero reserve or without current capability cause no saves/grants/checks.
- Shared school spell: route-specific trait, costs, duration and preserved native school;
  overlapping invocations cannot contaminate one another or the catalogue definition.
- Stone Skin grades 1 and 3 produce agility modifiers -1 and -3 through the scalar binding;
  concurrent Earth/Sorcerer invocations and save/reload leave the catalogue scalar intact.
  Invalid index/token/field bindings refuse before payment.
- Controlled 2/skill 42: grades 1/2 allowed, explicit 3 permitted at 22.5 Stone Skin energy,
  grade 4/7 refused; unsuccessful rolls do not raise mastery; bounded success raises once.
- Combined fixed+rebound same-resource costs cannot undercharge; missing materials fail
  before commitment; paid check failure/all-resisted group retains costs and lockouts.
- Area-capable native group resolution never grants per-target improvement/mastery, and
  caster effects remain once-only. Empty group success cannot advance mastery.
- Successful instantaneous damage reports `Applied` despite no persistent child; a
  nonempty-target blindness removal with no matching effect reports `NoChange` and earns
  no mastery. Actual removal qualifies; wholly absorbed damage and unknown legacy reports
  do not qualify. No reporting path repeats the effect.
- Two bodies, shared native trait, acquisition/branching through the second, persisted
  shared clocks, capability expiry/restoration, focus change and generator reconciliation.
- Shared-trait acquisition and Earth prerequisite do not acquire another spell or grant a
  Void branch without its own capability/admission/edge.
- FutureProg invocation takes the same payment and physical checks as player commands;
  knowledge, book and scroll possession cannot create an unpaid route.
- Configuration validation catches missing IDs, duplicate edges/admissions, cycles,
  incompatible grade profiles and unsupported bindings; clones preserve correct ownership.

Native MySQL/persistence acceptance on new uniquely named disposable targets only:

1. Acquire, cast/pay, advance under controlled randomness, save, restart and verify exact
   native skill, acquired state, grade, deadlines, enrolment marker and resource balance.
2. Repeat capability detach/restore and a second-body cast across restart; confirm no
   duplicate roots, generators, balance or opportunity windows.
3. Inject failures before payment, between payment/material mutation, after effect
   execution and around progression/result persistence. Verify receipt stages and lack
   of replay/refund/re-roll after restart; retain actual provider diagnostics.
   Specifically fail after sampling mastery on route A but before saving its result,
   attempt route B for the same spell and a second spell sharing the trait/reserve,
   then restart/reconcile. Affected records stay blocked; no second roll or duplicate
   grade is permitted. Recover a durably recorded sample; otherwise retain the proven
   pre-advance state and explicit uncertainty for staff resolution. Proven untouched
   routes continue independently.
4. Verify EF model/migration/designer/model snapshot and blank-database snapshot parity.

Use the repository EF and MUD-testing skills when implementing those surfaces. Run focused
core/library/database tests first, then justified dependants; do not default to a whole
solution build. Record native TRX and command/persistence transcripts separately. A blocked
database or smoke environment is an unexecuted acceptance item, not a pass.

## Stop and handover

Deliver changed runtime/interfaces/persistence, real commands/Progs, player/builder docs,
focused regressions, native receipts and a requirement-by-requirement acceptance table.
Keep all proposed later features disabled rather than approximating them through dangerous
fallthroughs. Report blockers without claiming full-preset acceptance or 04C closure.
Publish a review PR only if subsequently requested; never merge/deploy automatically.
