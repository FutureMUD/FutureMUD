# Fury and Calm: final integration proposal

This is a proposal-only checkpoint on the isolated `codex/armageddon-five-stock-spells` lane, based on `e380f3ebc03eff886092639ccc79e361f328d1c4`. No live casting, combat, effect, seeder, installer or harness callsite is changed. The coordinator independently cleared See the Unbodied code `25446524` and receipt `e380f3eb`; that is the immutable cleared content boundary. This later plan is not a release-qualified Fury/Calm implementation and must not enter a release candidate by pulling the lane tip wholesale.

The authoritative completion brief was previously read through supported Library access (`libfile_a4606cd0097081918d6c9d9e220e6da0`, `Armageddon_Magic_Completion_Implementation_Brief.md`). Historical rules below use the supplied `codedump.c`, Library `libfile_befb4ae78d1c8191aaa2640c49912d9d`, file `file_00000000438871faac40c5042679ce3a`. This plan needs no new transfer and claims no executor-local cloud path. Committed completion progress remains historical evidence and is unchanged.

## Settled choices and evidence boundaries

Luke approved an attribute bonus with seeder inference in `Sentinel_782e8d38f3188191b2829f4dcce1b8c1`. Fury uses an explicitly selected native attribute and positive editable units per source endurance point. It changes effective attributes and derived maximum stamina without refilling current stamina. The reviewed departing-effect correction at `da9af18a57634cf77b92999810a9aad0102fa41c` remains the lifecycle foundation. Native immediate removal clamping is an adaptation: historical `CHAR_APPLY_END` changed temporary endurance, `move_limit` computed maximum on demand, and affect attachment/removal did not write current move.

Luke approved Calm breaking on an **admitted incoming hostile attack attempt, including a miss**, in `Sentinel_17ba194d1fe481918a659d2d5b9a0972`. Refused and stale attempts do not break it. The historical comment says until attacked, but the precise incoming removal hook was not recovered in the bounded source search. The chosen attempt rule is an approved engine adaptation, not reconstructed historical code.

These approvals supersede pending-choice wording in the preserved [boundary note](Armageddon_Fury_Calm_Boundary_Note.md), [groundwork integration](Armageddon_Emotional_Policy_Integration.md), [casting proposal](Armageddon_Emotional_Casting_Hook_Proposal.md) and [cessation review](Armageddon_Emotional_Cessation_Review_Note.md). Their bytes and receipts remain unchanged. The old caster-wide leave-after-victim-removal proposal is superseded. Main owns admitted attacks, queued-action authority and cessation.

| Source contract | Roused Fury | Still Anger |
| --- | --- | --- |
| Stable key | `arm.spell.roused_fury` | `arm.spell.still_anger` |
| Canonical prerequisite | Unravel Enchantment acquired, raw80 | Roused Fury acquired, raw80 |
| Opening/raw cap/branch | 30 / 90 / raw80 | 30 / 90 / raw80 |
| Printed minimum energy | 20 | 7 |
| Delivery in this checkpoint | One character, including explicit self | One character, including explicit self |
| Counter | Subtract incoming grade from opposing Calm before further application | Reciprocal subtraction from opposing Fury before save |
| Recast | Accumulate duration, cap36, retain old grade/power/intensity/endurance | Accumulate duration, cap24, retain strongest source grade |
| Source unit | 600 seconds | 600 seconds |
| Surviving duration | Caster terrain table below | `2 * residualGrade` units |
| Endurance/save | Inclusive endurance roll; no opposed save | Native opposed save using residual-grade table; self saves too |

Calm raw80 opens the existing Mend Flesh at30 with **cap60**. Its qualified practice to controlled grade7 must remain intact. No grade2/raw20 catalogue prerequisite or fixed `60*grade` catalogue duration replaces these contracts. Source grade1..7 remains independent of native `SpellPower` and intensity. Existing relative acquisition/practice policy handles cap90; do not invent a raw95 requirement for grade7.

Fury source lines87470-87590 establish counters, duration and retained recast fields. Its endurance draw is inclusive `[g, floor(g*g/2)]`; grade1 returns1 without a random draw when the bounds invert. Earth adds `g`. Duration is Air `2g`, City/Inside `floor(5g/2)`, Hills/Mountain `floor(7g/2)`, Thornlands `floor(33g/10)`, Earth `4g`, otherwise `3g`, minimum1. Source terrain/plane tests must be mapped explicitly; a native terrain proxy for a source plane is a declared adaptation, not a new topology.

Calm lines83142-83244 establish terminal undead/Quickening/Insomnia handling before counter/save/cessation, reciprocal counters, failed-save application, and cessation after a completed save even when resisted. The latter two source affects are removed in that terminal branch. `stack_spell_affect` lines81574-81592 establishes strongest-grade accumulation. Mul/Thodeliv rage, undead, Quickening and Insomnia bindings require genuine native definitions; absent mappings must be visible as unimplemented historical exceptions, never guessed IDs or generic removal of all rage effects.

## Stock definitions and builder surface

The lane can author new `ArmageddonRousedFuryStock.cs`, `ArmageddonStillAngerStock.cs`, `ArmageddonEmotionalStockProfile.cs` and `EditableItemHelperMagic.EmotionalStock.cs`. Construction reuses `ArmageddonUtilityStock.Create` and its independent transaction, ordinary persisted spell/filter/cost/duration rows, `ControlledPower` source efficiency and native practice. No private cast command, resource debit, acquisition engine or installer registry is introduced.

Proposed factory signatures (new internal profile records; not callable yet):

```csharp
internal static MagicSpell Create(IFuturemud world, IMagicSchool school,
    ITraitDefinition skill, IMagicResource resource, RousedFuryStockProfile profile);
internal static MagicSpell Create(IFuturemud world, IMagicSchool school,
    ITraitDefinition skill, IMagicResource resource, StillAngerStockProfile profile);
```

Both profiles require an editable `(target, caster) -> boolean` eligibility prog, a native intensity, and explicit source-exception mappings or an explicitly recorded unsupported-exception disposition. Fury additionally requires the actual attribute, positive finite units per source point, and explicit terrain/plane predicates with an ordered fallback. Calm requires the native opposed trait and all seven residual-grade difficulties. Multiple matched environment predicates must either have an authored order matching source evaluation or refuse ambiguity before payment. Missing/null/invalid prog results refuse; no arbitrary world attribute or terrain ID is inferred by the runtime.

Proposed stock construction syntax:

```text
magic spell edit new stock roused-fury <school> <skill> <resource> <profile>
magic spell edit new stock still-anger <school> <skill> <resource> <profile>
```

Here `<profile>` is notation for the required configuration fields, not an installable entity or registry. Proposed actual Fury arguments after resource are `<attribute> <units> <intensity> <eligibility-prog> <Air terrain> <City terrain> <Inside terrain> <Hills terrain> <Mountain terrain> <Thornlands terrain> <Earth terrain>`; native terrain IDs adapt the two source plane classifications explicitly. Proposed Calm arguments after resource are `<opposed-trait> <intensity> <eligibility-prog> <save-grade1> ... <save-grade7>`. Initial Calm `attackbreak` is on. Normal effect builders expose `attribute <trait>`, `units <positive number>`, `intensity <number>`, `terrain ...`, `lifetime <group> <seconds> <cap>`, `save <grade> <difficulty>`, `attackbreak on|off`, and explicit source eligibility/counter bindings. The factory receives the validated profile records constructed by those ordinary arguments; no profile table, file import or private registry is proposed. Save, clone, show and native reload preserve each field, including malformed persisted XML for builder repair.

Proposed effect XML types are `sourcefury` and `sourcecalm`, separate from generic `rage`/`pacifism`. Initial source policy uses groups `armageddon.roused_fury` / `armageddon.still_anger`, unit600, caps36/24, and duration expression `0`; emotional resolution supplies the actual residual duration. Do not implement `IMagicSpellEffectLifetimePolicy` simply to feed the detection-specific resolver. The emotional template uses its own existing `EmotionalSpellPolicy` profile and the native parent scheduler. Untagged spells retain their current duration path.

Recommended visible initial attribute scale is1 native attribute unit/source point, explicitly supplied by the stock/seeder profile and editable thereafter. This is a proposed native scale, not historical scale parity or an already-authored default. Native intensity also stays explicit: illustrative proof fixtures may use Fury5 (rage threshold reached) and Calm11 (both peaceful thresholds reached), without claiming source grade equals intensity or those numbers are historical. A selected capacity prog must actually consume the effective inferred attribute; a prog ignoring it cannot qualify the stamina effect.

Factory validation must happen before row creation: same-world catalogue identities, skill/resource scope, runtime-supported attribute type, positive finite scale/products, complete profiles and spell shape. Duplicate/malformed construction must conserve spell, expression, prog and component rows. Ordinary single character/self delivery is the supported shape; area, caster siblings, charged carriers, attack payloads and traps are excluded. Material requirements not established in the recovered function/brief must not be invented; unresolved shared delivery rules remain an explicit qualification limit.

## Seeder inference contribution

Reuse the semantic attribute vocabulary in `SkillPackageSeeder.cs`563-573 and1426-1436: Constitution, Physique, Endurance, Body (case-insensitive). Those seeders examine types1/3; the reviewed Fury runtime currently accepts only `TraitType.Attribute` (type1). Do not silently seed a type3 definition that the runtime refuses or broaden derived-attribute semantics in this lane.

The independently implementable pure helper is `ArmageddonEmotionalAttributeInference` in a new seeder utility file. Input is the world catalogue plus optional explicit override; output is selected definition or a diagnostic listing candidates and repair instructions. A valid explicit same-world type1 override wins. Automatic inference succeeds when one distinct supported semantic candidate exists. Duplicate names or several plausible supported candidates are ambiguous and require an override; report the names/IDs. Missing mappings and derived-only candidates produce diagnostics and no Fury rows. This reuses the existing vocabulary while deliberately tightening silent fallback selection. It is idempotent, reads only the catalogue and never rewrites an existing builder selection.

The installer owner supplies the inferred attribute and explicit scale/profile to the new factory, then registers ordinary admission and canonical raw80 prerequisites. It must preserve existing edits on rerun. No existing `SkillPackageSeeder`, stock registry, central repertoire disposition or progress ledger edit belongs to this lane. A capacity-prog probe and explanation of its observed effective-trait behavior are acceptance evidence, not a new capacity-only substitute or seeder rewrite of the user's prog.

## Proposed main-owned additive contracts

Main supplied `Armageddon_EmotionalRuntime_Hook_Contract.md` at `fdba0e03f6b6cf3a3a4df9197e9ff42c2697c255` in the 2026-10-02 task worktree. It was read with `git show`, without modifying that worktree. Its final per-attack admission and exact-pair rules govern this plan. The following exact signatures are offered as a narrow binding of that prospective contract; main allocates final names/types before implementation. They are not existing callable APIs.

```csharp
// FutureMUDLibrary/Combat/AdmittedHostileAttack.cs
public sealed record AdmittedHostileAttack(IPerceiver Attacker,
    ICharacter Recipient, Guid OperationIdentity);

// FutureMUDLibrary/Effects/Interfaces/IAdmittedHostileAttackEffect.cs
public interface IAdmittedHostileAttackEffect : IEffectSubtype
{
    void OnAdmittedHostileAttack(AdmittedHostileAttack attack);
}

// FutureMUDLibrary/Combat/ICombatSelectiveCessation.cs
[Flags]
public enum CombatCessationChanges
{
    None = 0,
    SubjectRemoved = 1,
    OpponentPairCleared = 2
}

public interface ICombatSelectiveCessation
{
    CombatCessationChanges CeaseCombatFor(IPerceiver subject, IPerceiver opponent,
        ICombat expectedCombat);
}
```

The optional cessation capability belongs on the combat instance, preserving existing `ICombat.LeaveCombat` semantics (its bool means the entire combat ended, not proof that a subject was removed). `expectedCombat` must be this exact captured combat. The emotional admission requires this capability before payment when the selected subject is in combat. Absence refuses a cast requiring cessation; do not pay and silently omit it. Main owns the implementation and authoritative registration/queued-action interpretation. Exceptions after mutation propagate as uncertainty; result flags are returned only after observing the claimed changes. `OpponentPairCleared` denotes the passed opponent's exact pair, not permission to clear all of its fights.

Main's incoming-attack notification enumerates a snapshot of applicable recipient character/body effects, deduplicates references, and calls only opted-in `IAdmittedHostileAttackEffect` instances with the exact attacker, recipient and nonempty admitted operation identity. `SpellSourceCalmEffect` implements the callback; its persisted `attackbreak` choice controls removal of **itself** through `Owner.RemoveEffect(this, true)`. It checks its physical owner/recipient binding, not a guessed actor ID. Generic pacifism and `PsychicEmotionEffect` remain unchanged. Normal child `RemovalEffect` updates the parent and removes an empty parent, preserving unrelated siblings. Invalid persisted profiles are not silently deleted as a repair strategy. Main owns once-per-operation/recipient delivery; stock introduces no global identity cache. Callback/applicability failures propagate to main's existing authority failure handling.

Notification belongs **once per admitted attempt and distinct actual recipient**, after that operation's final hostile-action admission and before attack outcome/damage resolution. It includes misses and zero damage and excludes refused, stale, merely queued, unrelated and unclassified nonattack actions. Main's contract explicitly rules out unconditional notification in `CombatBase.CombatAction` before `ResolveMove`: individual moves can still refuse. The precise allocated seams follow the supplied contract:

| Main-owned route | Final notification seam |
| --- | --- |
| `MeleeWeaponAttack`89-113 / `NaturalAttackMove`78-98 | After vehicle/authority admission, before attack check; failed rolls still notify. |
| `NaturalRangedAttackMoveBase`195-233 | After actual target/range admission, before attack check. |
| `MagicPowerAttackMove`46-62 | After target/duplicate/CanInvokePower admission, before committed payment/use/check. |
| `RangedWeaponAttackBase` / firearm component460-493 | Actual chambered-shot authority/commit/detach seam; early move checks and empty-gun refusal cannot notify. Preserve independent countershots. |
| `MultiTargetCombatMove`59-74 | Each actual admitted child receives its own identity/notification; wrapper target enumeration grants none. |
| Direct component/spell routes | Need explicit final admission receipt and identity before claiming coverage. |

Main rechecks authority and captured physical identities after callbacks; attacker retirement/revocation or replacement target/combat cannot be overwritten. Defender independent attacks remain independent. The lane adds no command-name classification, second authority framework or `OnWounded`/damage-only notification. Stock acceptance may qualify only the main routes with their own reviewed receipts; universal attack coverage must not be inferred from one representative seam.

`CeaseCombatFor(B, A, expectedCombat)` semantics:

1. Confirm B still belongs to this admitted combat and matches the lane's captured subject relation. Drift before mutation refuses; use main's current registration generation/attempt identity where available, not just matching old references.
2. Remove B through native leave/queue cleanup. Observe B's registration, schedule and stale queued-target invalidation. Main handles incoming combatants targeting B through native acquisition.
3. **After B is removed and native acquisition callbacks finish**, inspect A's current registration and current target. If A now validly targets C, preserve A's entire A/C combat state. Never call `LeaveCombat(A)` because a pre-save snapshot once had A -> B.
4. If A still targets B, clear only that now-invalid pair and its admitted queued moves using main's native authority. If A has no valid opponents, use native ending conditions. Do not end a valid unrelated fight, request voluntary truce, or call `EndCombat` for the whole registration.
5. Return only observed flags. A prior A -> B snapshot alone cannot justify `OpponentPairCleared`. A legitimate acquisition of C is expected native behavior, not drift requiring its removal.

The lane captures exact physical combat pairs before save, revalidates after save/output callbacks, then invokes the capability on the expected combat only. If B fights C while caster A has no hostility with B, Calm's source victim-cessation requires the captured B/C pair, not a fabricated B/A pair. If A independently still fights B after that operation, only that still-current admitted A/B pair may be cleared. A newly acquired A/C fight is preserved. Main must allocate whether one cessation helper handles subject removal and all incoming stale pairs, or the lane submits the separately captured remaining A/B pair; no second caster-wide leave is permitted. This exact call arrangement is an allocation item, not permission to broaden the API to global cessation.

Save/output-created unrelated registrations are not recaptured and treated as the original authorization. Main's implementation owns the intentional changes caused by B's own removal. Its contract requires guards around callback-bearing setters/removal: `PerceiverItem.CombatTarget` removes target-change effects before writing, its Combat setter invokes `OnLeaveCombat` before replacing the field, and `ProgCombat.LeaveCombat` runs a prog. Checking only before/after the entire `LeaveCombat` call cannot justify overwriting a callback-created replacement registration at an intermediate commit. Main guards those exact commits; stock does not edit them.

## Narrow casting allocation

Keep the existing [unapplied caller patch](Armageddon_Emotional_Casting_Callers_Proposal.patch) as historical context; do not apply it verbatim. Its cessation wording is superseded, and its prepared-payload branch is unnecessary for this ordinary-only checkpoint. Main retains live `MagicSpell.cs` and casting-service files. The lane owns new `MagicSpell.Emotional.cs` and `SourceEmotionalEffect*.cs` partials after signatures are allocated.

Exact local anchors are unchanged at this checkpoint: `MagicSpell.cs` blob `6d6ab01e83dcd0e8333b0aae1cf39ceca11bcbff`; `MagicCastingService.cs` blob `3a59be69720e9bb7182a09c7b30762cd53dc2397`. These are proposal anchors, not assertions about main's evolving authority tree. The new helper signatures reuse the earlier proposal:

```csharp
private bool HasEmotionalEffects { get; }
private void PrepareEmotionalInvocation(ICharacter caster, IPerceivable? target,
    SpellPower power, bool mayReflect);
private void ConfirmPendingEmotionalAdmissions();
private EmotionalRecipientResolution BeginEmotionalResolution(ICharacter caster,
    IPerceivable recipient, SpellPower power);
private MagicEffectOperation CompleteEmotionalResolution(
    EmotionalRecipientResolution phase, bool targetSaved, OpposedOutcomeDegree outcome);
```

Main's allocated caller edits are:

| Existing location | Additive behavior |
| --- | --- |
| `CastSpellCore` prepayment admission `try`, after lifetime captures | Call `PrepareEmotionalInvocation`; shape/configuration/capability/state errors refuse before debit. |
| `Pay`, after `ConfirmPendingLifetimeAdmissions` | Callback-free `ConfirmPendingEmotionalAdmissions`; structural comparison only below the existing final fence. |
| Generic duration block | Skip only tagged emotional spells; their profile requires duration expression0. |
| Local `TargetResisted` | Optional residual difficulty and include-self parameters; keep native resistance API and original caster result, no second cast roll. |
| Local `TargetWasRejected`, after ward refusal/routing | Reflection output first, then revalidate/begin emotional phase; counter before save; complete even when save resisted; return before generic child creation. |
| Prepared/trap/attack entry guards | Explicitly reject emotional templates in these unsupported routes. Do not add an unqualified paid-bypass resolver. |

The emotional template implements existing `IMagicSpellEffectPreparedSelection` and `IMagicSpellEffectOperation`, using the current selection-payment capture/reuse/confirmation and reporting journal. A single token bundles original target and potential reflected caster, so shared selection dispatch is unchanged. Freeze bound grade, body/cell identity, profile bytes, terrain/predicates, applicable source cohorts, parent identity, child state and native scheduled expiry. Freeze an endurance draw per possible positive-residual recipient; reuse it across confirmation, never reroll. Callback/prog work occurs above the final structural fence. The inside-Pay confirmation adds no callbacks/checks. After paid output/save/reflection callbacks, validate again before each mutation. Never recapture after debit.

Source terminal eligibility handling precedes counters. Stronger opposition is weakened in place and ends the phase; equal opposition is removed and ends it; weaker opposition is removed and positive residual continues. Counter exhaustion performs no save, child creation or Calm cessation. A completed positive Calm save may lead to child application and selective cessation; a resisted save still permits proved cessation. A thrown save/output callback is uncertainty, not a completed resistant outcome to continue from.

Use normal `MagicSpellParent` ownership and scheduler, with emotional state authoritative in the persisted child and `LifetimeState` unset (the latter currently represents accumulated detection/water semantics). Counter reduction then has no duplicate parent-grade field to update. Confirm compatibility with the main stored-spell/dispel consumers before implementation. Existing `EffectScheduler.Reschedule` supports in-place Fury duration extension without remove/recreate or temporary baseline-stamina clamping. `ResolvedDuration` is init-only and not presently consumed elsewhere in this checkout; do not invent a mutable parent setter simply to mirror the scheduler. If main requires new authoritative metadata, coordinate its narrow API rather than modifying shared parent state unilaterally.

Calm strongest-grade refresh can use a normally constructed replacement parent/child, carrying the resolved strongest grade/power/intensity and accumulated duration. It has no Fury attribute bonus requiring uninterrupted attachment. Parent `Power` must agree with retained child power for persistence/dispel; do not update only child strength and leave stale parent power. Fury in-place refresh retains both its old parent power and complete old child state. Counter weakening preserves old intensity/power/endurance, as the cleared child already does. Verify parent/child references, state and actual scheduler deadline before reporting. If a future in-place Calm update is preferred, allocate the paired parent/child metadata API first. Native saved remaining time/offline restoration and whole-unit normalization are declared clock adaptations, not historical offline parity.

Report `Applied` only for observed counter, child/state/deadline or cessation change; valid unchanged results are `NoChange`; pure refused/resisted outcomes are `Rejected`. `Unknown` uses current paid uncertainty/quarantine handling with no replay/refund. Only intended non-reflected useful application sets `AppliedIntendedOperation` for mastery. Reflection, pure resistance and unchanged cap casts cannot earn intended application mastery. Costs remain the existing quote/payment/source-efficiency calculation, including already-paid no-change outcomes.

## Independently owned work and qualification gates

Before main hook allocation, the lane can finish pure stock-profile validation/XML/builders, attribute inference utility/tests, source policy tests, detached persisted-child fixtures and a **new** dedicated `FuryCalmStockNativeHarness` project/entrypoint. Native full casting/combat acceptance must wait for main's committed allocated contracts. New templates may compile in isolation, but no claim of ordinary paid functionality is justified while live routing is absent. A one-line existing stock-helper dispatch/name addition and the Calm child's partial declaration/callback require allocation before touching those existing files. Normal replacement Calm avoids requiring an in-place state-update API in this initial plan.

| Owned verification | Required evidence |
| --- | --- |
| `RousedFuryStockTests` / `StillAngerStockTests` | Profile validation, ordinary builder/create/duplicate conservation, clone/XML/reload, distinct native intensity/source grade, explicit units and all seven save grades. |
| `EmotionalAttributeInferenceTests` | One semantic match, explicit override, aliases, ambiguity/duplicates, missing/derived-only definitions, invalid scope, no implicit fallback/write. |
| Existing policy plus owned recast tests | Every terrain and grade endpoint, g1 no RNG, Earth g7 bounds14..31, stronger/equal/weaker counters, cap36/24, Fury old-field retention, Calm strongest/equal-grade conflict. |
| Paid native low/high stock casts | Grade1 and7, exact quote/debit and operation/mastery, body/terrain/target/profile/prog/capability refusal before payment; pure resistance, counter-only and resisted-cessation outcomes. |
| Main cessation acceptance | A -> B with C present: B leaves, A acquires C, A/C survive. Also A still targets B, no remaining opponent, stale queue, reflected self, and save/output relation drift. |
| Main attack-break acceptance | Admitted miss/hit/zero damage and classified hostile magic break Calm; refused/stale/queued/nonhostile actions do not; once per victim/attempt, character/body applicability and sibling conservation. |
| Capacity/persistence/lifecycle | Real selected world capacity prog observes effective attribute, attach/recast never refill, counter retains bonus, expiry/dispel/removal clamps correctly, independent-process reload preserves exact state and expiry, empty-parent cleanup. |
| Acquisition/regressions | Unravel79 refusal/80 opens Fury30 cap90; Fury80 opens Calm; Calm80 opens Mend30 cap60 and Mend practice reaches grade7. Preserve all cleared five stocks, See/Pierce/water/provision and Raise Servitor/Storm Spear/Flame Knife/Sand Knife evidence. |

Each native mode uses a newly marked disposable database and a lane-specific MySQL temp prefix such as `futuremud-fury-calm-stock-mysql_<UUID>`, random loopback port and exact owned process/datadir/port guards. Output belongs under `.artifacts/test-runs/fury-calm-stock-<run>`. Never attach to user databases, reuse another lane's instance, kill by process name, or delete a computed path without resolving and checking its lane root. No process or database was started for this plan checkpoint.

Final runtime qualification requires tests built from the actual committed source; source stability manifests, executed unit/native assembly SHA256 equality and entry-specific receipts. Preserve failures and native fixture limits. Broad strict checks become appropriate after actual runtime/hook integration; documentation validation here is not new spell test evidence.

## Integration handoff and release separation

The immediate coordination items are the main-owned operation-identity notification and expected-combat pair-cessation surface, exact subject/opponent/caster call arrangement, live caller allocation, existing Calm child declaration/callback allocation, and confirmation that child-owned source state plus normal Calm replacement needs no shared parent setter. Main also allocates identities/receipts for direct component/spell attack routes before coverage is claimed. None is silently implemented. Independent catalogue/profile/seeder utility work can proceed without those live changes. Real setting exception mappings and full-world stamina/save behavior remain qualification limits until evidenced.

Installer integration contributions are the two ordinary factories/profiles, admission opening30/rawCap90/relative grade policy, and the three canonical raw80 edges: Unravel -> Fury -> Calm -> existing Mend (cap60). Shared dispatch, registry, ledgers and disposition updates remain coordinator-owned integration notes. Main can freeze the release at cleared `e380f3eb` content; Fury/Calm only qualify after a later independently reviewed runtime/source/assembly checkpoint. This plan makes no completion claim for the pair and changes no historical record.
