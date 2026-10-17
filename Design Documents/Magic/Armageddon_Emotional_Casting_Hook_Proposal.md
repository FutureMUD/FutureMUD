# Emotional casting hook proposal (unapplied)

This is an additive integration proposal against source checkpoint `da9af18a57634cf77b92999810a9aad0102fa41c`. It does not edit live casting or combat methods, implement emotional stock, select an endurance default, or select a Calm attack-break policy. The [caller patch](Armageddon_Emotional_Casting_Callers_Proposal.patch) is a review artifact. Its new private methods/types are proposed contracts, not existing callable code: applying it alone would not compile.

The unchanged caller blob is `MudSharpCore/Magic/MagicSpell.cs`: `6d6ab01e83dcd0e8333b0aae1cf39ceca11bcbff`. The unchanged casting-service blob is `MudSharpCore/Magic/Casting/MagicCastingService.cs`: `3a59be69720e9bb7182a09c7b30762cd53dc2397`. The main owner would apply the agreed caller changes; this lane would own a separate `MagicSpell.Emotional.cs` partial and a builder-editable target template. No casting-service, selection-payment, combat-authority or native-harness dispatch edit is proposed.

## Exact caller changes

1. Inside the existing prepayment admission `try`, call `PrepareEmotionalInvocation(caster, target, power, mayReflect)` after existing lifetime captures. Immediately after `ConfirmPendingLifetimeAdmissions()` in `Pay`, call `ConfirmPendingEmotionalAdmissions()`. Both are no-ops for untagged spells.
2. Skip the generic duration-expression evaluation only for tagged emotional spells. Their admitted source policy derives duration from residual grade, terrain and native remaining time. Untagged duration evaluation is unchanged.
3. Give the existing local `TargetResisted` optional difficulty and self-save parameters. Existing callers keep their old defaults. Calm explicitly includes self and supplies its residual-grade difficulty. This preserves the native resistance check and the already-selected caster check; it does not make a second casting roll or change the paid invocation grade.
4. In `TargetWasRejected`, retain ward routing and the non-reflecting rejection first. The emotional branch then emits an existing reflection message, begins the admitted counter resolution, attempts a save only when that phase requires one and retains positive grade, and completes resolution after the save. It returns before generic effect application/cleanup. Untagged spells continue through their original branch.
5. In `ResolvePreparedSpell`, branch after its null-target guard and before generic lifetime/duration/save callbacks to the new partial resolver. It uses the existing prepared attack/check outcome and native resistance API. This method currently does not route wards; this proposal does not silently add ward routing, charge a resource again, or enable emotional attack payloads/carriers.

Reflection messages are callbacks: the emotional branch emits one before `BeginEmotionalResolution` revalidates the admitted actual recipient. A callback must never redirect to an uncaptured target or silently recapture after payment. The untagged reflection-message ordering stays unchanged.

## New partial contract

The following signatures describe the proposed private surface; no new public/shared API is required by these caller changes.

```csharp
private bool HasEmotionalEffects { get; }
private void PrepareEmotionalInvocation(ICharacter caster, IPerceivable? target,
    SpellPower power, bool mayReflect);
private void ConfirmPendingEmotionalAdmissions();
private EmotionalRecipientResolution BeginEmotionalResolution(ICharacter caster,
    IPerceivable recipient, SpellPower power);
private MagicEffectOperation CompleteEmotionalResolution(
    EmotionalRecipientResolution phase, bool targetSaved, OpposedOutcomeDegree outcome);
private void ResolvePreparedEmotionalSpell(ICharacter caster, IPerceivable target,
    SpellPower power, CheckOutcome attackOutcome, bool attackPayload);
```

`EmotionalRecipientResolution` exposes `Continue`, `RequiresSave` and `SaveDifficulty`; internally it retains the sealed recipient, residual grade, source kind, admitted cohorts and verified counter changes. `RequiresSave` is false for source Fury and true for surviving source Calm. These source definitions reject an opposed trait on Fury rather than silently ignoring it; a future builder variant would need an explicit save-policy change. A missing Calm opposed trait or incomplete residual-grade native difficulty table refuses during admission. The table is an explicit builder-editable engine adaptation of the historical modifier `-5 * (grade - 4)`, not a claim that FutureMUD opposed checks reproduce the old saving-throw probabilities.

Admission accepts a bound grade1..7 invocation with exactly one emotional target template, a single character/self target, no area or caster effects, and readable applicable policy/state. It rejects unsupported combinations before payment. Source grade remains separate from `SpellPower` and editable intensity. Source eligibility's early-return branch is terminal before counter/save/cessation; any Quickening, Insomnia, undead or Mul-rage bindings need real source/native evidence. Do not invent status/race IDs or remove arbitrary rage effects to approximate them.

The template implements the existing `IMagicSpellEffectPreparedSelection` capture/reuse/confirmation protocol. Its single selection token includes the original recipient and the caster as a possible reflected recipient. The existing selection-payment dispatcher captures templates against the original target; it does not independently capture all reflected cohorts. Bundling the reflected plan avoids a shared dispatcher change. Freeze body identity, applicable emotional references, parent identity/ownership, persisted source state, actual scheduled expiry, caster terrain, profile/configuration and the selected source endurance roll. Grade1's inverted upper bound returns1 without a random draw. Repeated selection reuses the token; drift refuses before debit. Applicability/prog callbacks belong in that existing confirmation protocol, followed by its structural fence. `ConfirmPendingEmotionalAdmissions` inside `Pay` must perform callback-free structural comparison, not introduce another prog/check below the final fence. After-payment callbacks require another validation before mutation. No global token cache, second payment planner, alternate scheduler or alternate receipt is introduced.

`BeginEmotionalResolution` validates again and performs reciprocal source-grade subtraction before the native save. A stronger opposing child loses only incoming source grade and ends this cast's application phase; equal grades remove that opposing child and end the phase; weaker opposition is removed and residual incoming grade continues. Counter weakening preserves the opposing Fury's original intensity, power and endurance bonus. Remove only the admitted managed child through existing owner/parent lifecycle APIs and prove the observed removal/reduction. Counter exhaustion skips save, child creation and Calm cessation; useful counter changes can still report `Applied`.

For a positive Calm phase, `CompleteEmotionalResolution` applies the child only if the completed save did not resist, then performs selective cessation even when the save resisted. Cessation uses existing `ICombat.LeaveCombat(selectedActor)` for the victim and, if it is fighting that victim, the caster. The phase snapshots the selected combat registrations and caster/victim target relationship before the save callback; completion validates those relationships and observes each leave. Callback-created unrelated combat must never be silently treated as the admitted relationship. It never calls `EndCombat`, requests voluntary truce, or rewrites combat authority. An exception during the save/callback is uncertainty handled by existing quarantine, not a completed-save outcome that should be assumed safe to continue.

Fury accumulation retains old source/native fields and cap36; Calm retains strongest source grade and cap24. Native remaining duration and native parent scheduling stay authoritative. A retained Fury should refresh its existing scheduled child/parent rather than remove and recreate it through a temporary baseline-capacity clamp. Parent/child state and deadlines must be observed before reporting a retained or extended lifetime. The new emotional resolver must not route its accumulated policy through the detection-only resolver. Any genuinely required shared parent-state API must be coordinated separately before implementation.

## Reporting and proof obligations

Use existing `MagicEffectOperation`: `Applied` only for observed counter reduction/removal, final child/deadline/strength change or proved selective cessation; `NoChange` for a valid unchanged final result; `Rejected` for refusal/resistance with no useful operation. `Unknown` throws into current execution/quarantine handling. Do not convert uncertainty into success, retry or refund. The configured caller marks `AppliedIntendedOperation` only for an intended, non-reflected `Applied` result. Counter-only application and cessation after a resisted save remain eligible useful operations; pure resistance/no change grant no application mastery. No new cost calculation or payment mutation is proposed.

Required later qualification includes builder-created low/high-grade paid casts; target/shape/state refusal before payment; stronger/equal/weaker counters; self saves; resisted Calm with observed cessation; reflected cohort changes without intended mastery; invalidated pre/post-payment callbacks; preserved unrelated combat and final queued-move refusal; duration caps and grade/power/endurance retention; actual world capacity without refill; save/independent-process reload and expiry/dispel. This patch has only syntax/context/diff validation as a proposal, not compilation or native execution qualification. The companion [Fury removal correction](Armageddon_Emotional_Stamina_Removal_Fix.md) covers the actual native removal lifecycle using an explicit fixture, independently of future stock/default decisions.

## Decisions preserved for the next user interaction

| Choice | Proposed option and tradeoff | Alternative and tradeoff |
|---|---|---|
| Fury endurance mapping | Explicit selected endurance attribute, editable multiplier initially1 native attribute unit/source point, existing capacity prog, no refill. Simple builder tuning; every legitimate effective-attribute consumer receives the bonus, and capacity conversion depends on the world prog. | Select another explicit scale after evaluating the world's trait ranges/prog. Better setting fit; requires that measurement before a stock default can be authored. |
| Calm break behavior | Expiry/dispel/Fury cancellation only. Follows recovered executable behavior; does not implement the source comment's incoming-attack promise. | Explicit native adaptation: break on an admitted hostile attack attempt, including misses, at the main owner's final execution boundary. Matches that comment's broad meaning; needs an agreed attack classification and single notification. Damage-only cancellation is not equivalent. |

Neither choice has been authored as a stock default or attack hook. The bounded Library search found no incoming-attack removal hook among all26 `SPELL_CALM`, all7 `CHAR_AFF_CALM` and all18 `IS_CALMED` matches; absence outside the supplied dump is not established. Source provenance and canonical acquisition links remain in [the boundary note](Armageddon_Fury_Calm_Boundary_Note.md) and [groundwork integration note](Armageddon_Emotional_Policy_Integration.md).

The next narrow candidate is **Water Breathing**: native apply/remove adapters exist in `StandaloneStatusSpellEffects.cs`, and the committed source tree links it from Draw Wine raw80. That is adapter availability and an acquisition lead, not stock readiness. Its complete historical rules, native breathing consumer, paid reporting and persistence still need qualification. No next-family implementation or broad infrastructure starts here.
