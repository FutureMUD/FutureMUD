# Fury and Calm: independent profile preparation

This checkpoint follows the proposal at `cc4ca6570408b84334d07609e6379b2f8054120b` and the coordinator's authorization for independent catalogue work. Cleared content `e380f3ebc03eff886092639ccc79e361f328d1c4` remains available unchanged in history. **The magic package is disabled for release until ready.** These new definitions/templates are preparation, not usable Fury/Calm stock or permission to enable that package.

## Implemented independently

New `ArmageddonRousedFuryStock` and `ArmageddonStillAngerStock` supply stable identities, canonical opening30/cap90/raw80 acquisition metadata, source minimum energy20/7 and ordinary editable character/self definition XML. Fury uses600-second units/cap36; Calm uses600-second units/cap24 and points to existing Mend Flesh with cap60. Their `Definition(resourceId, costExpressionId, profile)` methods produce ordinary spell definitions through the reviewed utility definition writer. No database construction factory, stock-command dispatch, installer registration, shared harness dispatch or central ledger update is supplied yet.

New immutable `EmotionalStockProfile` stores explicit native trait/prog IDs, intensity, editable lifetime, rational terrain rules, positive Fury attribute units, complete seven-grade Calm save table and Calm's editable admitted-attack break choice. It snapshots input collections, refuses duplicate terrain/save mappings, invalid kinds/versions, missing sections, overflow/nonfinite values, unsupported historical-exception mappings and incompatible Fury/Calm fields. Its calculations delegate to the reviewed `EmotionalSpellPolicy`; it adds no casting or lifecycle implementation. Source grade remains separate from native power/intensity. XML persists the declared `unmapped` setting-exception disposition rather than guessing undead, Quickening, Insomnia or Mul/Thodeliv IDs.

The Fury stock profile binds explicit native terrain identities for Air, City, Inside, Hills, Mountain, Thornlands and Earth; default durations are2g, floor(2.5g), floor(3.5g), floor(3.3g),4g and fallback3g. Earth adds the residual grade to the inclusive endurance draw. A native terrain proxy for a historical plane remains a declared adaptation. Duplicate identity mappings refuse ambiguity. Builders may edit these rules normally. The stock helper initially requires seven distinct positive identities; the editable underlying profile can remove a rule and use its explicit fallback.

`SourceFuryEffect` and `SourceCalmEffect` clone/save their profiles and expose normal effect builder methods for intensity, eligibility, trait, units, terrain/fallback, lifetime, saves and attack-break. Their explicit `RegisterPreparationFactory` methods exercise normal factory loading only in an isolated test scope; they are deliberately **not** discovered by the engine's automatic `RegisterFactory` scan. Invalid persisted profiles retain their XML for repair. `reset` explicitly discards an invalid profile; incomplete builder profiles can then be repaired field by field. Invalid edits to an already-valid profile leave it unchanged.

The first broad run exposed the shared scroll-compatibility requirement for every registered effect type. Integration needs explicit unsupported-scroll rows for `sourcefury` and `sourcecalm` in main-owned `ScrollSpellCompatibility`, with reason: source-ordered emotional counters, residual saves, retained source/attribute state, admission notification and pair cessation are not qualified for stored scroll delivery. No compatibility manifest or registry source file is changed here. Automatic registration waits for that allocation and the live hooks. The unit factory scope is nonparallel and restores the exact previous load/builder registries after each test, so prepared catalogue types do not contaminate other regression tests.

**Every new template execution entry refuses.** Existing configured selection capture refuses with `RuntimeIntegrationError` before payment; confirmation/reuse refuse too. Operation application and legacy effect application throw that same integration error rather than silently substituting generic pacifism/rage or a partial paid implementation. No readiness toggle or builder flag bypasses it. The configured refusal is tested; a direct legacy cast can reach its generic payment before a template application error and therefore remains unsupported. No acquisition/installer row makes these preparations available to players. Main's package-disable gate remains the release authority.

These templates do not implement the detection-only lifetime interface. Existing reviewed `SpellSourceFuryEffect`, `SpellSourceCalmEffect`, parent ownership, source policy, departing-Fury capacity reconciliation and all existing live files remain byte-for-byte unchanged. Profile-derived detached children are tested for persisted state and counter weakening using those actual reviewed classes. This is fixture/XML persistence, not independent-process database reload or combat qualification.

## Installer-owned trait binding

Seeder inference remains **installer-owned**; this supersedes the independently implementable seeder-helper allocation suggested in the preceding plan. No seeder helper, inference implementation or existing seeder file is changed here.

The trait-binding contract is `EmotionalStockProfile.TraitId` plus positive finite `UnitsPerSourcePoint`. Installer inference supplies the actual catalogue definition using Luke's approved semantic approach, reports ambiguity/missing mappings and accepts an explicit builder override. Runtime `ValidateBindings(world)` resolves the configured ID, requires an ordinary `TraitType.Attribute` for Fury, validates the explicit compiling boolean `(target, caster)` eligibility prog, and resolves all mapped terrain identities. A derived-only inference result cannot be silently passed to the reviewed Fury runtime. Runtime never repeats name-based inference. It does not rewrite a world's capacity prog or refill current stamina.

No numeric scale is inferred from the historical formula. The profile accepts an explicit positive scale; tests use0.5 to distinguish source points from native units. Installer can supply a visible editable scale1 as the proposed stock adaptation after reviewing the world attribute/capacity convention. Actual capacity behavior needs its configured effective-trait prog and native acceptance. Save difficulties and native intensities remain explicit adaptations, not reconstructed saving probabilities or source-grade aliases.

## Final proposed hook binding for main allocation

The unchanged [final plan](Armageddon_Fury_Calm_Final_Integration_Plan.md) binds main's prospective `Armageddon_EmotionalRuntime_Hook_Contract.md` at `fdba0e03f6b6cf3a3a4df9197e9ff42c2697c255`. This preparation adds no shared declarations. The proposed final signature packet remains:

```csharp
public sealed record AdmittedHostileAttack(IPerceiver Attacker,
    ICharacter Recipient, Guid OperationIdentity);
public interface IAdmittedHostileAttackEffect : IEffectSubtype
{
    void OnAdmittedHostileAttack(AdmittedHostileAttack attack);
}
public interface ICombatSelectiveCessation
{
    CombatCessationChanges CeaseCombatFor(IPerceiver subject, IPerceiver opponent,
        ICombat expectedCombat);
}
```

`CombatCessationChanges` has observed flags `None`, `SubjectRemoved`, `OpponentPairCleared`; uncertainty throws into existing handling. Main issues a nonempty unique operation identity at **each actual child attack's final admission**, not at wrapper enumeration. It delivers once for that operation/physical recipient, with exact physical attacker/recipient references. Direct component attacks require their own admitted operation identity/receipt; a move's proposed targets are not evidence of admission. Stock adds no global identity cache, move dispatcher or attack classification.

The allocated Calm callback will remove only its own admitted/applicable child through the native owner/parent lifecycle when its persisted break policy opts in. Generic pacifism and PsychicEmotionEffect remain unchanged. Main rechecks attacker authority and captured physical identities after callbacks, including retirement/revocation, without suppressing independent defender attacks.

The coordinator's further review requires melee admission **after the last pre-check callback**, exact chambered-round validation at firearm commit/detach, and one notification for every actual admitted multi-target child. Shared `CombatBase` pre-resolution notification is premature because moves/components can still refuse. These details appear as explicit pending cases in the dedicated harness acceptance plan; no standalone mock test is presented as proof of main-owned callsites.

Cessation guards must run **inside callback-bearing target/combat commits**. Checking around ordinary setters/LeaveCombat is insufficient: a removal/target setter can create a replacement fight before the original assignment. Main must preserve that replacement rather than overwrite it. For A -> B with C present, B removal may cause A to acquire C; re-read A after removal and preserve A/C. Never caster-wide leave from the old A/B snapshot. The selected subject/opponent/caster pair arrangement, expected combat and stale queue authority remain main allocation items.

The private casting signatures remain the preceding plan's `PrepareEmotionalInvocation`, callback-free `ConfirmPendingEmotionalAdmissions`, `BeginEmotionalResolution` and `CompleteEmotionalResolution`. Main owns allocation and live ward/save/caller edits. Positive Calm completes cessation even after a completed resistant save; an exception is uncertainty. Native Fury recast should retain its existing parent/child in place; Calm can replace a normal parent/child while carrying resolved strongest strength and matching parent power. No shared parent setter is added.

## Dedicated harness preparation and remaining acceptance

New `FuryCalmStockNativeHarness` is an independent project, entrypoint, runner and explicit acceptance plan. It links no shared native dispatcher or another lane's harness source. `profile-preflight` performs nine bounded checks using actual new profiles, source definitions and reviewed policy, emitting profile XML and SHA256 hashes of executed assemblies. The runner captures complete source/assembly manifests before and after, writes a uniquely named lane output directory and refuses to overwrite an entry receipt. It starts no database or MySQL process.

`paid-native` returns exit2 with a structured **blocked** receipt listing the unallocated hooks and pending acceptance cases. It cannot start a database or produce a passing gameplay receipt. This is intentional separation of preparation from future native qualification; it is not a skipped passing test. A later allocated native implementation must use marked disposable `futuremud_fury_calm_stock_<timestamp>_<random>` databases and `futuremud-fury-calm-stock-mysql_<UUID>` process/datadir guards, never user or other-lane state.

Pending real acceptance includes ordinary builder/database construction; paid low/high casts and refusal/cost/application/mastery; reciprocal counters; self saves and resisted cessation; admitted misses/refused attempts; all main route identities/callback checks; third-opponent and replacement-combat conservation; actual inferred attribute capacity/no refill; independent-process reload, expiry/dispel/sibling cleanup; canonical prerequisite acquisition and Mend cap60/grade7 practice; cleared See/Pierce/water/provision/five-stock and four reviewed weapon regressions. The prior source/native receipts remain historical records. This checkpoint contributes no central completion claim and must remain separate from the release candidate.

Execution receipts, preserved preliminary failures, exact source/assembly fingerprints and final review commit are recorded in the separate profile-preparation receipt. Native fixture limits remain explicit; preparation evidence cannot qualify ordinary paid Fury/Calm or universal attack coverage.
